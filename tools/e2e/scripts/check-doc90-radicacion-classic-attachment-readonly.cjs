'use strict';

const fs = require('node:fs');
const mysql = require('mysql2/promise');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');

function profilePath() {
  const index = process.argv.indexOf('--profile');
  if (index < 0 || !process.argv[index + 1]) throw new Error('Debe indicar --profile <archivo-json-no-sensible>.');
  return path.resolve(process.argv[index + 1]);
}

function isSingleReadOnlyQuery(sql) {
  return /^\s*SELECT\b/i.test(sql || '') &&
    !/;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) &&
    (String(sql).match(/\?/g) || []).length === 1;
}

async function main() {
  requireInteractiveConsole();
  const profile = JSON.parse(fs.readFileSync(profilePath(), 'utf8'));
  if (!/^\d+$/.test(String(profile.expectedRadicado || ''))) throw new Error('El perfil no contiene un radicado válido.');
  for (const [name, query] of [['contextSql', profile.contextSql], ['documentCountSql', profile.documentCountSql]]) {
    if (!isSingleReadOnlyQuery(query)) throw new Error(`${name} debe ser un único SELECT parametrizado.`);
  }

  const secrets = {};
  await collectValue(secrets, 'DOC90_CHECK_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(secrets, 'DOC90_CHECK_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
  await collectConfirmation(secrets, 'DOC90_CHECK_AUTHORIZED', '¿Autoriza esta comprobación exclusivamente SELECT?');

  let connection;
  try {
    connection = await mysql.createConnection({
      host: String(profile.mysqlHost || ''),
      port: Number(profile.mysqlPort),
      database: String(profile.mysqlDatabase || ''),
      user: secrets.DOC90_CHECK_MYSQL_USER,
      password: secrets.DOC90_CHECK_MYSQL_PASSWORD,
      multipleStatements: false
    });
    const [contextRows] = await connection.execute(profile.contextSql, [String(profile.expectedRadicado)]);
    const [countRows] = await connection.execute(profile.documentCountSql, [String(profile.expectedRadicado)]);
    if (contextRows.length !== 1) throw new Error(`La consulta de contexto devolvió ${contextRows.length} filas.`);
    const total = Number(countRows[0]?.total);
    if (!Number.isSafeInteger(total) || total < 0) throw new Error('La consulta de conteo no devolvió un total válido.');
    const pending = Number(contextRows[0]?.estado_pendiente);
    const evidencePath = path.resolve(__dirname, '..', 'artifacts', 'doc90-radicacion-classic-attachment-readonly.json');
    fs.mkdirSync(path.dirname(evidencePath), { recursive: true });
    fs.writeFileSync(evidencePath, `${JSON.stringify({
      fechaUtc: new Date().toISOString(),
      estadoPendiente: pending,
      documentosActuales: total,
      soloLectura: true
    }, null, 2)}\n`, 'utf8');
    console.log('Comprobación DOC-90 exclusivamente SELECT completada.');
    console.log(`Estado pendiente actual: ${pending}.`);
    console.log(`Documentos actuales asociados al recurso autorizado: ${total}.`);
  } finally {
    secrets.DOC90_CHECK_MYSQL_PASSWORD = '';
    await connection?.end().catch(() => {});
  }
}

main().catch(error => {
  console.error(error?.message || 'La comprobación DOC-90 se detuvo sin mostrar valores sensibles.');
  process.exitCode = 1;
});
