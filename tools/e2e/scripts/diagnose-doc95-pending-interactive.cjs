'use strict';

const fs = require('node:fs');
const mysql = require('mysql2/promise');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');

const CHALLENGE_SQL = `SELECT COUNT(*) AS total,
  SUM(CASE WHEN Consumed=0 AND ExpiresAtUtc>UTC_TIMESTAMP() THEN 1 ELSE 0 END) AS eligible,
  SUM(CASE WHEN Consumed=1 THEN 1 ELSE 0 END) AS consumed
FROM ra_auth_second_factor_challenge
WHERE CreatedAtUtc>=UTC_TIMESTAMP()-INTERVAL 15 MINUTE`;

const SMTP_SQL = `SELECT COUNT(*) AS active_rows,
  SUM(CASE WHEN NULLIF(TRIM(SERV_SMTP),'') IS NOT NULL THEN 1 ELSE 0 END) AS host_valid,
  SUM(CASE WHEN CAST(PUERTO_SERV_SMTP AS UNSIGNED) BETWEEN 1 AND 65535 THEN 1 ELSE 0 END) AS port_valid,
  SUM(CASE WHEN NULLIF(TRIM(USUARIO_SMTP),'') IS NOT NULL AND USUARIO_SMTP LIKE '%@%' THEN 1 ELSE 0 END) AS sender_valid,
  SUM(CASE WHEN CAST(SMTP_TIEMPO AS SIGNED)>0 THEN 1 ELSE 0 END) AS timeout_valid,
  SUM(CASE WHEN ESTADO_SSL IN (0,1) AND ESTADO_CREDENCIAL IN (0,1) THEN 1 ELSE 0 END) AS flags_valid,
  SUM(CASE WHEN ESTADO_CREDENCIAL=0 OR (ESTADO_CREDENCIAL=1 AND NULLIF(TRIM(PASW_SMTP),'') IS NOT NULL) THEN 1 ELSE 0 END) AS credentials_valid
FROM Config_Smpt_Side
WHERE ESTADO_ENVIO=1`;

const MODULE_SQL = `SELECT COUNT(*) AS matches,
  SUM(CASE WHEN ESTADO_MODULO=1 THEN 1 ELSE 0 END) AS enabled,
  SUM(CASE WHEN ESTADO_MODULO=1 AND RequiereSegundoFactor=1 AND SecondFactorProviderType=1 AND SegundoFactorTiempoExpira BETWEEN 1 AND 10 THEN 1 ELSE 0 END) AS second_factor_valid
FROM gestor_modulos
WHERE NOMBRE_MODULO=?`;

const PRINCIPAL_SQL = `SELECT COUNT(*) AS matches,
  SUM(CASE WHEN Id_Remit_Dest_Int>0 THEN 1 ELSE 0 END) AS identity_valid,
  SUM(CASE WHEN NULLIF(TRIM(Correo_Electronico),'') IS NOT NULL AND Correo_Electronico LIKE '%@%' THEN 1 ELSE 0 END) AS email_valid
FROM remit_dest_interno
WHERE Login_Usuario=?`;

function argument(name) {
  const index = process.argv.indexOf(name);
  if (index < 0 || !process.argv[index + 1]) throw new Error(`Falta ${name}.`);
  return path.resolve(process.argv[index + 1]);
}

function loadProfiles() {
  const database = JSON.parse(fs.readFileSync(argument('--db-profile'), 'utf8'));
  const e2e = JSON.parse(fs.readFileSync(argument('--e2e-profile'), 'utf8'));
  const metadata = {
    host: String(database.mysqlHost || '').trim(),
    port: Number(database.mysqlPort),
    database: String(database.mysqlDatabase || '').trim()
  };
  if (!metadata.host || !Number.isSafeInteger(metadata.port) || metadata.port < 1 || metadata.port > 65535 || !metadata.database) {
    throw new Error('El perfil de base no contiene metadatos válidos.');
  }
  if (typeof e2e.module !== 'string' || !e2e.module.trim()) throw new Error('El perfil E2E no contiene módulo.');
  return { metadata, module: e2e.module.trim() };
}

function assertReadOnly() {
  for (const sql of [CHALLENGE_SQL, SMTP_SQL, MODULE_SQL, PRINCIPAL_SQL]) {
    if (!/^\s*SELECT\b/i.test(sql) || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql)) {
      throw new Error('DOC95_DIAGNOSTIC_QUERY_NOT_READ_ONLY');
    }
  }
}

function number(row, field) {
  const value = Number(row?.[field]);
  if (!Number.isSafeInteger(value) || value < 0) throw new Error('DOC95_DIAGNOSTIC_RESULT_INVALID');
  return value;
}

async function main() {
  requireInteractiveConsole();
  assertReadOnly();
  const profiles = loadProfiles();
  const secrets = {};
  await collectValue(secrets, 'user', 'Usuario MySQL de solo lectura');
  await collectValue(secrets, 'password', 'Contraseña MySQL de solo lectura', { secret: true });
  await collectValue(secrets, 'account', 'Cuenta DOC-95 autorizada');
  await collectConfirmation(secrets, 'authorization', '¿Autoriza diagnóstico DOC-95 exclusivamente con SELECT y salida saneada?');

  let connection;
  try {
    connection = await mysql.createConnection({ ...profiles.metadata, user: secrets.user, password: secrets.password, multipleStatements: false });
    const [challengeRows] = await connection.execute(CHALLENGE_SQL);
    const [smtpRows] = await connection.execute(SMTP_SQL);
    const [moduleRows] = await connection.execute(MODULE_SQL, [profiles.module]);
    const [principalRows] = await connection.execute(PRINCIPAL_SQL, [secrets.account]);
    const challenges = challengeRows[0];
    const smtp = smtpRows[0];
    const module = moduleRows[0];
    const principal = principalRows[0];
    const activeRows = number(smtp, 'active_rows');
    const smtpStructurallyValid = activeRows === 1 &&
      ['host_valid', 'port_valid', 'sender_valid', 'timeout_valid', 'flags_valid', 'credentials_valid']
        .every(field => number(smtp, field) === 1);

    console.log('DOC95_DIAGNOSTIC_SELECT_ONLY=true');
    console.log(`DOC95_RECENT_CHALLENGES=${number(challenges, 'total')}`);
    console.log(`DOC95_RECENT_ELIGIBLE=${number(challenges, 'eligible')}`);
    console.log(`DOC95_RECENT_CONSUMED=${number(challenges, 'consumed')}`);
    console.log(`DOC95_SMTP_ACTIVE_ROWS=${activeRows}`);
    console.log(`DOC95_SMTP_STRUCTURE_VALID=${smtpStructurallyValid}`);
    console.log(`DOC95_MODULE_MATCHES=${number(module, 'matches')}`);
    console.log(`DOC95_MODULE_ENABLED=${number(module, 'enabled') === 1}`);
    console.log(`DOC95_MODULE_SECOND_FACTOR_VALID=${number(module, 'second_factor_valid') === 1}`);
    console.log(`DOC95_PRINCIPAL_MATCHES=${number(principal, 'matches')}`);
    console.log(`DOC95_PRINCIPAL_IDENTITY_VALID=${number(principal, 'identity_valid') === 1}`);
    console.log(`DOC95_PRINCIPAL_EMAIL_VALID=${number(principal, 'email_valid') === 1}`);
  } finally {
    secrets.password = '';
    secrets.account = '';
    await connection?.end().catch(() => {});
  }
}

main().catch(error => {
  const code = /^[A-Z0-9_]{3,100}$/.test(error?.message || '') ? error.message : 'DOC95_DIAGNOSTIC_FAILED';
  console.error(`Diagnóstico DOC-95 detenido de forma segura (${code}).`);
  process.exitCode = 1;
});

module.exports = { CHALLENGE_SQL, MODULE_SQL, PRINCIPAL_SQL, SMTP_SQL, assertReadOnly };
