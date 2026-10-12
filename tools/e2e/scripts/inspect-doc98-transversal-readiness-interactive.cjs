'use strict';

const fs = require('node:fs');
const mysql = require('mysql2/promise');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');

const TARGET_TYPES = Object.freeze([
  'DOCUARCHI CONTENEDOR',
  'GESTOR DOCUMENTAL',
  'RADICACION DOCUMENTAL',
  'WORKFLOW DOCUMENTAL'
]);

const SCHEMA_SQL = `SELECT
  (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge'
      AND COLUMN_NAME IN ('Id','ChallengeId','AuthUserId','Provider','CodeHash','ExpiresAtUtc','Consumed','Attempts','CreatedAtUtc','AuthPayloadJson')) AS challenge_columns,
  (SELECT COUNT(*) FROM information_schema.STATISTICS
    WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge'
      AND INDEX_NAME='PRIMARY' AND COLUMN_NAME='Id' AND SEQ_IN_INDEX=1) AS primary_id,
  (SELECT COUNT(*) FROM information_schema.STATISTICS
    WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge'
      AND COLUMN_NAME='ChallengeId' AND NON_UNIQUE=0 AND SEQ_IN_INDEX=1) AS unique_challenge,
  (SELECT COUNT(*) FROM information_schema.STATISTICS
    WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ra_auth_second_factor_challenge'
      AND COLUMN_NAME='AuthUserId' AND SEQ_IN_INDEX=1) AS auth_user_index,
  (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='gestor_modulos'
      AND COLUMN_NAME IN ('ID_MODULO','EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA','NOMBRE_MODULO','TIPO_MODULO','ESTADO_MODULO','RequiereSegundoFactor','SecondFactorProviderType','SegundoFactorTiempoExpira')) AS module_columns,
  (SELECT COUNT(*) FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='Config_Smpt_Side'
      AND COLUMN_NAME IN ('SERV_SMTP','PUERTO_SERV_SMTP','USUARIO_SMTP','PASW_SMTP','DOMINIO_SMTP','SMTP_TIEMPO','ESTADO_SSL','ESTADO_ENVIO','ESTADO_CREDENCIAL')) AS smtp_columns`;

const MODULE_SQL = `SELECT TIPO_MODULO AS module_type,
  COUNT(*) AS total_count,
  SUM(CASE WHEN RequiereSegundoFactor=1 AND SecondFactorProviderType=1 AND SegundoFactorTiempoExpira BETWEEN 1 AND 10 THEN 1 ELSE 0 END) AS on_count,
  SUM(CASE WHEN COALESCE(RequiereSegundoFactor,0)=0 THEN 1 ELSE 0 END) AS off_count,
  SUM(CASE WHEN RequiereSegundoFactor NOT IN (0,1) THEN 1 ELSE 0 END) AS invalid_flag_count,
  SUM(CASE WHEN SegundoFactorTiempoExpira IS NULL OR SegundoFactorTiempoExpira NOT BETWEEN 1 AND 10 THEN 1 ELSE 0 END) AS expiry_gap_count
FROM gestor_modulos
WHERE ESTADO_MODULO=1
  AND TIPO_MODULO IN (?,?,?,?)
GROUP BY TIPO_MODULO
ORDER BY TIPO_MODULO`;

const AMBIGUITY_SQL = `SELECT COUNT(*) AS ambiguous_count
FROM (
  SELECT EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA, NOMBRE_MODULO
  FROM gestor_modulos
  WHERE ESTADO_MODULO=1 AND TIPO_MODULO IN (?,?,?,?)
  GROUP BY EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA, NOMBRE_MODULO
  HAVING COUNT(*)<>1
) ambiguous`;

const SMTP_SQL = `SELECT COUNT(*) AS active_count
FROM Config_Smpt_Side
WHERE ESTADO_ENVIO=1`;

function profilePath() {
  const index = process.argv.indexOf('--db-profile');
  if (index < 0 || !process.argv[index + 1]) throw new Error('DOC98_DB_PROFILE_REQUIRED');
  return path.resolve(process.argv[index + 1]);
}

function loadDatabaseMetadata(target) {
  const profile = JSON.parse(fs.readFileSync(target, 'utf8'));
  const forbidden = Object.keys(profile).filter(key => /(?:password|passwd|secret|credential|mysqluser|mysqlurl|connectionstring)/i.test(key));
  if (forbidden.length) throw new Error('DOC98_DB_PROFILE_CONTAINS_SECRET_FIELDS');
  const metadata = {
    host: String(profile.mysqlHost || '').trim(),
    port: Number(profile.mysqlPort),
    database: String(profile.mysqlDatabase || '').trim()
  };
  if (!metadata.host || !Number.isSafeInteger(metadata.port) || metadata.port < 1 || metadata.port > 65535 || !metadata.database) {
    throw new Error('DOC98_DB_PROFILE_INVALID');
  }
  return metadata;
}

function assertReadOnly() {
  for (const sql of [SCHEMA_SQL, MODULE_SQL, AMBIGUITY_SQL, SMTP_SQL]) {
    if (!/^\s*SELECT\b/i.test(sql) || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql)) {
      throw new Error('DOC98_PREFLIGHT_QUERY_NOT_READ_ONLY');
    }
  }
}

function safeFailureCode(error, stage) {
  const code = String(error?.code || '').trim().toUpperCase();
  if (code === 'ER_ACCESS_DENIED_ERROR') return 'DOC98_DB_ACCESS_DENIED';
  if (code === 'ER_BAD_DB_ERROR') return 'DOC98_DB_NOT_FOUND';
  if (['ECONNREFUSED', 'ETIMEDOUT', 'ENOTFOUND', 'EHOSTUNREACH'].includes(code)) return `DOC98_DB_${code}`;
  const message = String(error?.message || '').trim();
  if (/^DOC98_[A-Z0-9_]{3,100}$/.test(message)) return message;
  return stage === 'query' ? 'DOC98_PREFLIGHT_QUERY_FAILED' : 'DOC98_PREFLIGHT_CONNECTION_FAILED';
}

function evaluate(schema, modules, ambiguity, smtp) {
  const schemaOk = Number(schema.challenge_columns) === 10 && Number(schema.primary_id) === 1 &&
    Number(schema.unique_challenge) >= 1 && Number(schema.auth_user_index) >= 1 &&
    Number(schema.module_columns) === 8 && Number(schema.smtp_columns) === 9;
  const found = new Map(modules.map(row => [String(row.module_type), row]));
  const targetTypesOk = TARGET_TYPES.every(type => found.has(type));
  const invalidConfig = modules.reduce((sum, row) => sum + Number(row.invalid_flag_count || 0), 0);
  const expiryGaps = modules.reduce((sum, row) => sum + Number(row.expiry_gap_count || 0), 0);
  const total = modules.reduce((sum, row) => sum + Number(row.total_count || 0), 0);
  const on = modules.reduce((sum, row) => sum + Number(row.on_count || 0), 0);
  const off = modules.reduce((sum, row) => sum + Number(row.off_count || 0), 0);
  return {
    schemaOk,
    targetTypesOk,
    ambiguityOk: Number(ambiguity.ambiguous_count) === 0,
    smtpOk: Number(smtp.active_count) === 1,
    invalidConfig,
    expiryGaps,
    total,
    on,
    off,
    ready: schemaOk && targetTypesOk && Number(ambiguity.ambiguous_count) === 0 && Number(smtp.active_count) === 1 && invalidConfig === 0
  };
}

async function main() {
  requireInteractiveConsole();
  assertReadOnly();
  const metadata = loadDatabaseMetadata(profilePath());
  const secrets = {};
  await collectValue(secrets, 'user', 'Usuario MySQL autorizado para preflight DOC-98');
  await collectValue(secrets, 'password', 'Contraseña MySQL autorizada', { secret: true });
  await collectConfirmation(secrets, 'environment', '¿Autoriza el preflight DOC-98 exclusivamente con SELECT y salida saneada?');

  let connection;
  let stage = 'connection';
  try {
    connection = await mysql.createConnection({ ...metadata, user: secrets.user, password: secrets.password, multipleStatements: false });
    stage = 'query';
    const [[schema]] = await connection.execute(SCHEMA_SQL);
    const [modules] = await connection.execute(MODULE_SQL, TARGET_TYPES);
    const [[ambiguity]] = await connection.execute(AMBIGUITY_SQL, TARGET_TYPES);
    const [[smtp]] = await connection.execute(SMTP_SQL);
    const result = evaluate(schema, modules, ambiguity, smtp);
    console.log('DOC98_PREFLIGHT_SELECT_ONLY=true');
    console.log(`DOC98_SCHEMA_OK=${result.schemaOk}`);
    console.log(`DOC98_TARGET_TYPES_OK=${result.targetTypesOk}`);
    console.log(`DOC98_MODULE_ROWS=${result.total}`);
    console.log(`DOC98_MODULE_ROWS_ON=${result.on}`);
    console.log(`DOC98_MODULE_ROWS_OFF=${result.off}`);
    console.log(`DOC98_MODULE_INVALID=${result.invalidConfig}`);
    console.log(`DOC98_MODULE_EXPIRY_GAPS=${result.expiryGaps}`);
    console.log(`DOC98_MODULE_AMBIGUITY_OK=${result.ambiguityOk}`);
    console.log(`DOC98_SMTP_UNIQUE_OK=${result.smtpOk}`);
    console.log(`DOC98_PREFLIGHT_READY=${result.ready}`);
    if (!result.ready) process.exitCode = 2;
  } catch (error) {
    throw new Error(safeFailureCode(error, stage));
  } finally {
    secrets.password = '';
    await connection?.end().catch(() => {});
  }
}

if (require.main === module) {
  main().catch(error => {
    const code = /^DOC98_[A-Z0-9_]{3,100}$/.test(error?.message || '') ? error.message : 'DOC98_PREFLIGHT_FAILED';
    console.error(`Preflight DOC-98 detenido de forma segura (${code}). No se mostraron secretos ni filas.`);
    process.exitCode = 1;
  });
}

module.exports = { AMBIGUITY_SQL, MODULE_SQL, SCHEMA_SQL, SMTP_SQL, TARGET_TYPES, assertReadOnly, evaluate, loadDatabaseMetadata, safeFailureCode };
