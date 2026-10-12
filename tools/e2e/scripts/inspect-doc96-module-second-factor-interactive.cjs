'use strict';

const fs = require('node:fs');
const mysql = require('mysql2/promise');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');

const MODULES_SQL = `SELECT NOMBRE_MODULO AS module_name,
  CASE
    WHEN ESTADO_MODULO<>1 THEN 'DISABLED'
    WHEN COALESCE(RequiereSegundoFactor,0)=0 THEN 'OFF'
    WHEN RequiereSegundoFactor=1
      AND SecondFactorProviderType=1
      AND SegundoFactorTiempoExpira BETWEEN 1 AND 10 THEN 'ON'
    ELSE 'INVALID'
  END AS second_factor_state
FROM gestor_modulos
ORDER BY NOMBRE_MODULO`;

function profilePath() {
  const index = process.argv.indexOf('--db-profile');
  if (index < 0 || !process.argv[index + 1]) throw new Error('DOC96_DB_PROFILE_REQUIRED');
  return path.resolve(process.argv[index + 1]);
}

function loadDatabaseMetadata() {
  const profile = JSON.parse(fs.readFileSync(profilePath(), 'utf8'));
  const metadata = {
    host: String(profile.mysqlHost || '').trim(),
    port: Number(profile.mysqlPort),
    database: String(profile.mysqlDatabase || '').trim()
  };
  if (!metadata.host || !Number.isSafeInteger(metadata.port) || metadata.port < 1 || metadata.port > 65535 || !metadata.database) {
    throw new Error('DOC96_DB_PROFILE_INVALID');
  }
  return metadata;
}

function assertReadOnly() {
  if (!/^\s*SELECT\b/i.test(MODULES_SQL) || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(MODULES_SQL)) {
    throw new Error('DOC96_DIAGNOSTIC_QUERY_NOT_READ_ONLY');
  }
}

function safeFailureCode(error, stage) {
  const knownCode = String(error?.code || '').trim().toUpperCase();
  if (knownCode === 'ER_ACCESS_DENIED_ERROR') return 'DOC96_DB_ACCESS_DENIED';
  if (knownCode === 'ER_BAD_DB_ERROR') return 'DOC96_DB_NOT_FOUND';
  if (['ECONNREFUSED', 'ETIMEDOUT', 'ENOTFOUND', 'EHOSTUNREACH'].includes(knownCode)) {
    return `DOC96_DB_${knownCode}`;
  }
  const safeMessage = String(error?.message || '').trim();
  if (/^[A-Z0-9_]{3,100}$/.test(safeMessage)) return safeMessage;
  return stage === 'query' ? 'DOC96_DIAGNOSTIC_QUERY_FAILED' : 'DOC96_DIAGNOSTIC_CONNECTION_FAILED';
}

async function main() {
  requireInteractiveConsole();
  assertReadOnly();
  const metadata = loadDatabaseMetadata();
  const secrets = {};
  await collectValue(secrets, 'user', 'Usuario MySQL de solo lectura');
  await collectValue(secrets, 'password', 'Contraseña MySQL de solo lectura', { secret: true });
  await collectConfirmation(secrets, 'authorization', '¿Autoriza diagnóstico DOC-96 exclusivamente con SELECT y salida saneada?');

  let connection;
  let stage = 'connection';
  try {
    connection = await mysql.createConnection({ ...metadata, user: secrets.user, password: secrets.password, multipleStatements: false });
    stage = 'query';
    const [rows] = await connection.execute(MODULES_SQL);
    console.log('DOC96_MODULE_DIAGNOSTIC_SELECT_ONLY=true');
    for (const row of rows) {
      const moduleName = String(row.module_name || '').trim().replace(/[\r\n=]/g, '');
      const state = String(row.second_factor_state || '').trim();
      if (!moduleName || !['OFF', 'ON', 'INVALID', 'DISABLED'].includes(state)) throw new Error('DOC96_DIAGNOSTIC_RESULT_INVALID');
      console.log(`DOC96_MODULE=${moduleName}|2FA=${state}`);
    }
  } catch (error) {
    throw new Error(safeFailureCode(error, stage));
  } finally {
    secrets.password = '';
    await connection?.end().catch(() => {});
  }
}

if (require.main === module) {
  main().catch(error => {
    const code = /^[A-Z0-9_]{3,100}$/.test(error?.message || '') ? error.message : 'DOC96_DIAGNOSTIC_FAILED';
    console.error(`Diagnóstico DOC-96 detenido de forma segura (${code}).`);
    process.exitCode = 1;
  });
}

module.exports = { MODULES_SQL, assertReadOnly, safeFailureCode };
