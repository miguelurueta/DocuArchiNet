'use strict';

const { spawnSync } = require('node:child_process');
const fs = require('node:fs');
const mysql = require('mysql2/promise');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');

const SELECT_MODULE_SQL = `SELECT ID_MODULO, RequiereSegundoFactor, SecondFactorProviderType, SegundoFactorTiempoExpira
FROM gestor_modulos
WHERE NOMBRE_MODULO=?`;
const DISABLE_SQL = `UPDATE gestor_modulos
SET RequiereSegundoFactor=0
WHERE ID_MODULO=? AND NOMBRE_MODULO=? AND RequiereSegundoFactor=1`;
const RESTORE_SQL = `UPDATE gestor_modulos
SET RequiereSegundoFactor=?
WHERE ID_MODULO=? AND NOMBRE_MODULO=? AND RequiereSegundoFactor=0`;

function argument(name) {
  const index = process.argv.indexOf(name);
  if (index < 0 || !process.argv[index + 1]) throw new Error(`DOC96_${name.substring(2).replace(/-/g, '_').toUpperCase()}_REQUIRED`);
  return path.resolve(process.argv[index + 1]);
}

function loadDatabaseMetadata(target) {
  const profile = JSON.parse(fs.readFileSync(target, 'utf8'));
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

function assertSqlScope() {
  if (!/^\s*SELECT\b/i.test(SELECT_MODULE_SQL) || /;|\b(?:INSERT|DELETE|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|LOAD|OUTFILE)\b/i.test(SELECT_MODULE_SQL)) {
    throw new Error('DOC96_TEMPORARY_SELECT_SCOPE_INVALID');
  }
  for (const sql of [DISABLE_SQL, RESTORE_SQL]) {
    if (!/^\s*UPDATE\s+gestor_modulos\b/i.test(sql) || /;|\b(?:INSERT|DELETE|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|LOAD|OUTFILE)\b/i.test(sql)) {
      throw new Error('DOC96_TEMPORARY_UPDATE_SCOPE_INVALID');
    }
  }
}

function validSnapshot(rows) {
  if (!Array.isArray(rows) || rows.length !== 1) return false;
  const row = rows[0];
  return Number.isSafeInteger(Number(row.ID_MODULO)) && Number(row.ID_MODULO) > 0 &&
    Number(row.RequiereSegundoFactor) === 1 && Number(row.SecondFactorProviderType) === 1 &&
    Number.isSafeInteger(Number(row.SegundoFactorTiempoExpira)) && Number(row.SegundoFactorTiempoExpira) >= 1 && Number(row.SegundoFactorTiempoExpira) <= 10;
}

function safeFailureCode(error, stage) {
  const knownCode = String(error?.code || '').trim().toUpperCase();
  if (knownCode === 'ER_ACCESS_DENIED_ERROR') return 'DOC96_GESTOR_DB_ACCESS_DENIED';
  if (knownCode === 'ER_BAD_DB_ERROR') return 'DOC96_GESTOR_DB_NOT_FOUND';
  if (['ER_TABLEACCESS_DENIED_ERROR', 'ER_DBACCESS_DENIED_ERROR', 'ER_COLUMNACCESS_DENIED_ERROR'].includes(knownCode)) {
    return stage === 'restore' ? 'DOC96_GESTOR_RESTORE_PERMISSION_DENIED' : 'DOC96_GESTOR_UPDATE_PERMISSION_DENIED';
  }
  if (['ECONNREFUSED', 'ETIMEDOUT', 'ENOTFOUND', 'EHOSTUNREACH'].includes(knownCode)) {
    return `DOC96_GESTOR_DB_${knownCode}`;
  }
  const safeMessage = String(error?.message || '').trim();
  if (/^[A-Z0-9_]{3,100}$/.test(safeMessage)) return safeMessage;
  const suffix = String(stage || 'execution').replace(/[^a-z]/gi, '_').toUpperCase();
  return `DOC96_GESTOR_${suffix}_FAILED`;
}

async function main() {
  requireInteractiveConsole();
  assertSqlScope();
  const databaseMetadata = loadDatabaseMetadata(argument('--db-profile'));
  const e2eProfile = argument('--e2e-profile');
  if (!fs.existsSync(e2eProfile)) throw new Error('DOC96_E2E_PROFILE_NOT_FOUND');

  const secrets = {};
  await collectValue(secrets, 'dbUser', 'Usuario MySQL autorizado para cambio temporal');
  await collectValue(secrets, 'dbPassword', 'Contraseña MySQL autorizada', { secret: true });
  await collectConfirmation(secrets, 'temporaryChange', '¿Autoriza cambiar GESTOR de 2FA ON a OFF solo durante esta corrida y restaurarlo al finalizar?');
  await collectConfirmation(secrets, 'sharedEnvironment', '¿Confirma que el ambiente admite este cambio temporal controlado?');

  let connection;
  let snapshot;
  let changed = false;
  let e2eExitCode = 1;
  let stage = 'connection';
  try {
    connection = await mysql.createConnection({ ...databaseMetadata, user: secrets.dbUser, password: secrets.dbPassword, multipleStatements: false });
    stage = 'precondition';
    const [beforeRows] = await connection.execute(SELECT_MODULE_SQL, ['GESTOR']);
    if (!validSnapshot(beforeRows)) throw new Error('DOC96_GESTOR_ON_PRECONDITION_FAILED');
    snapshot = beforeRows[0];

    stage = 'disable';
    const [disabled] = await connection.execute(DISABLE_SQL, [Number(snapshot.ID_MODULO), 'GESTOR']);
    if (Number(disabled.affectedRows) !== 1) throw new Error('DOC96_GESTOR_TEMPORARY_DISABLE_FAILED');
    changed = true;
    const [offRows] = await connection.execute(SELECT_MODULE_SQL, ['GESTOR']);
    if (offRows.length !== 1 || Number(offRows[0].RequiereSegundoFactor) !== 0) throw new Error('DOC96_GESTOR_TEMPORARY_OFF_NOT_OBSERVED');
    console.log('DOC96_GESTOR_TEMPORARY_OFF=true');

    stage = 'e2e';
    const e2eRoot = path.resolve(__dirname, '..');
    const runner = path.join(__dirname, 'run-doc96-login-second-factor-ui-interactive.cjs');
    const child = spawnSync(process.execPath, [runner, '--scenario', 'off', '--profile', e2eProfile], {
      cwd: e2eRoot,
      stdio: 'inherit'
    });
    e2eExitCode = Number.isInteger(child.status) ? child.status : 2;
  } catch (error) {
    throw new Error(safeFailureCode(error, stage));
  } finally {
    try {
      if (changed && connection && snapshot) {
        stage = 'restore';
        let restored;
        try {
          [restored] = await connection.execute(RESTORE_SQL, [Number(snapshot.RequiereSegundoFactor), Number(snapshot.ID_MODULO), 'GESTOR']);
        } catch (error) {
          throw new Error(safeFailureCode(error, stage));
        }
        if (Number(restored.affectedRows) !== 1) throw new Error('DOC96_GESTOR_RESTORE_UPDATE_FAILED');
        const [afterRows] = await connection.execute(SELECT_MODULE_SQL, ['GESTOR']);
        if (!validSnapshot(afterRows)) throw new Error('DOC96_GESTOR_RESTORE_VERIFICATION_FAILED');
        console.log('DOC96_GESTOR_RESTORED_ON=true');
      }
    } finally {
      secrets.dbPassword = '';
      await connection?.end().catch(() => {});
    }
  }
  if (e2eExitCode !== 0) throw new Error('DOC96_GESTOR_OFF_E2E_FAILED_AFTER_RESTORE');
  console.log('DOC96_GESTOR_OFF_CONTROL=PASSED');
}

if (require.main === module) {
  main().catch(error => {
    const code = /^[A-Z0-9_]{3,100}$/.test(error?.message || '') ? error.message : 'DOC96_GESTOR_TEMPORARY_TEST_FAILED';
    console.error(`Control temporal DOC-96 detenido de forma segura (${code}). Verifique que GESTOR quedó restaurado antes de reintentar.`);
    process.exitCode = 2;
  });
}

module.exports = { DISABLE_SQL, RESTORE_SQL, SELECT_MODULE_SQL, assertSqlScope, safeFailureCode, validSnapshot };
