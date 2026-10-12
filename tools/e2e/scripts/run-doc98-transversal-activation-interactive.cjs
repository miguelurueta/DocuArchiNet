'use strict';

const crypto = require('node:crypto');
const fs = require('node:fs');
const mysql = require('mysql2/promise');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const readiness = require('./inspect-doc98-transversal-readiness-interactive.cjs');

const root = path.resolve(__dirname, '..', '..', '..');
const snapshotPath = path.join(root, '.opsxj', 'runtime', 'DOC-98-module-snapshot.json');
const TARGET_SELECT_SQL = `SELECT ID_MODULO AS module_id,
  EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA AS company_id,
  NOMBRE_MODULO AS module_name,
  TIPO_MODULO AS module_type,
  ESTADO_MODULO AS enabled,
  RequiereSegundoFactor AS required_flag,
  SecondFactorProviderType AS provider_type,
  SegundoFactorTiempoExpira AS expiry_minutes
FROM gestor_modulos
WHERE ESTADO_MODULO=1 AND TIPO_MODULO IN (?,?,?,?)
ORDER BY EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA, ID_MODULO`;
const TARGET_LOCK_SQL = `${TARGET_SELECT_SQL} FOR UPDATE`;
const OUTSIDE_SELECT_SQL = `SELECT ID_MODULO AS module_id,
  EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA AS company_id,
  NOMBRE_MODULO AS module_name,
  TIPO_MODULO AS module_type,
  ESTADO_MODULO AS enabled,
  RequiereSegundoFactor AS required_flag,
  SecondFactorProviderType AS provider_type,
  SegundoFactorTiempoExpira AS expiry_minutes
FROM gestor_modulos
WHERE NOT (ESTADO_MODULO=1 AND TIPO_MODULO IN (?,?,?,?))
ORDER BY EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA, ID_MODULO`;
const ACTIVATE_SQL = `UPDATE gestor_modulos
SET RequiereSegundoFactor=1, SecondFactorProviderType=1
WHERE ID_MODULO=?
  AND EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=?
  AND NOMBRE_MODULO=?
  AND TIPO_MODULO=?
  AND ESTADO_MODULO=1
  AND RequiereSegundoFactor <=> ?
  AND SecondFactorProviderType <=> ?
  AND SegundoFactorTiempoExpira <=> ?`;
const RESTORE_SQL = `UPDATE gestor_modulos
SET RequiereSegundoFactor=?, SecondFactorProviderType=?, SegundoFactorTiempoExpira=?
WHERE ID_MODULO=?
  AND EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=?
  AND NOMBRE_MODULO=?
  AND TIPO_MODULO=?
  AND ESTADO_MODULO=1
  AND RequiereSegundoFactor=1
  AND SecondFactorProviderType=1
  AND SegundoFactorTiempoExpira <=> ?`;

function argument(name, required = true) {
  const index = process.argv.indexOf(name);
  if (index < 0 || !process.argv[index + 1]) {
    if (required) throw new Error(`DOC98_${name.substring(2).replace(/-/g, '_').toUpperCase()}_REQUIRED`);
    return null;
  }
  return process.argv[index + 1];
}

function positiveIntegerArgument(name) {
  const value = Number(argument(name));
  if (!Number.isSafeInteger(value) || value < 0) throw new Error(`DOC98_${name.substring(2).replace(/-/g, '_').toUpperCase()}_INVALID`);
  return value;
}

function normalizeRow(row) {
  return {
    moduleId: Number(row.module_id),
    companyId: Number(row.company_id),
    moduleName: String(row.module_name || '').trim(),
    moduleType: String(row.module_type || '').trim(),
    enabled: Number(row.enabled),
    requiredFlag: row.required_flag === null ? null : Number(row.required_flag),
    providerType: row.provider_type === null ? null : Number(row.provider_type),
    expiryMinutes: row.expiry_minutes === null ? null : Number(row.expiry_minutes)
  };
}

function fingerprint(rows) {
  const normalized = rows.map(normalizeRow);
  return crypto.createHash('sha256').update(JSON.stringify(normalized), 'utf8').digest('hex');
}

function classify(rows) {
  const normalized = rows.map(normalizeRow);
  const foundTypes = new Set(normalized.map(row => row.moduleType));
  const structurallyValid = normalized.every(row => Number.isSafeInteger(row.moduleId) && row.moduleId > 0 &&
    Number.isSafeInteger(row.companyId) && row.companyId > 0 && row.moduleName &&
    readiness.TARGET_TYPES.includes(row.moduleType) && row.enabled === 1 &&
    (row.requiredFlag === null || row.requiredFlag === 0 || row.requiredFlag === 1) &&
    Number.isSafeInteger(row.expiryMinutes) && row.expiryMinutes >= 1 && row.expiryMinutes <= 10);
  const on = normalized.filter(row => row.requiredFlag === 1 && row.providerType === 1);
  const off = normalized.filter(row => row.requiredFlag === null || row.requiredFlag === 0);
  return {
    normalized,
    on,
    off,
    valid: structurallyValid && readiness.TARGET_TYPES.every(type => foundTypes.has(type)) && on.length + off.length === normalized.length
  };
}

function assertSqlScope() {
  for (const sql of [TARGET_SELECT_SQL, TARGET_LOCK_SQL, OUTSIDE_SELECT_SQL]) {
    const inspected = sql.replace(/\bFOR\s+UPDATE\s*$/i, '');
    if (!/^\s*SELECT\b/i.test(inspected) || /;|\b(?:INSERT|UPDATE|DELETE|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|LOAD|OUTFILE)\b/i.test(inspected)) {
      throw new Error('DOC98_ACTIVATION_SELECT_SCOPE_INVALID');
    }
  }
  for (const sql of [ACTIVATE_SQL, RESTORE_SQL]) {
    if (!/^\s*UPDATE\s+gestor_modulos\b/i.test(sql) || /;|\b(?:INSERT|DELETE|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|LOAD|OUTFILE)\b/i.test(sql)) {
      throw new Error('DOC98_ACTIVATION_UPDATE_SCOPE_INVALID');
    }
  }
}

function writeSnapshot(snapshot) {
  fs.mkdirSync(path.dirname(snapshotPath), { recursive: true });
  fs.writeFileSync(snapshotPath, `${JSON.stringify(snapshot, null, 2)}\n`, { encoding: 'utf8', flag: 'wx' });
}

function readSnapshot() {
  const snapshot = JSON.parse(fs.readFileSync(snapshotPath, 'utf8'));
  if (snapshot.version !== 1 || !Array.isArray(snapshot.targetRows) || !/^[a-f0-9]{64}$/.test(snapshot.outsideFingerprint || '')) {
    throw new Error('DOC98_SNAPSHOT_INVALID');
  }
  return snapshot;
}

function safeFailureCode(error, stage) {
  const code = String(error?.code || '').trim().toUpperCase();
  if (code === 'ER_ACCESS_DENIED_ERROR') return 'DOC98_DB_ACCESS_DENIED';
  if (code === 'ER_BAD_DB_ERROR') return 'DOC98_DB_NOT_FOUND';
  if (['ER_TABLEACCESS_DENIED_ERROR', 'ER_DBACCESS_DENIED_ERROR', 'ER_COLUMNACCESS_DENIED_ERROR'].includes(code)) return 'DOC98_UPDATE_PERMISSION_DENIED';
  if (['ECONNREFUSED', 'ETIMEDOUT', 'ENOTFOUND', 'EHOSTUNREACH'].includes(code)) return `DOC98_DB_${code}`;
  const message = String(error?.message || '').trim();
  if (/^DOC98_[A-Z0-9_]{3,100}$/.test(message)) return message;
  return `DOC98_${String(stage || 'execution').replace(/[^a-z]/gi, '_').toUpperCase()}_FAILED`;
}

async function collectSecretsAndAuthorization(mode) {
  const secrets = {};
  await collectValue(secrets, 'user', `Usuario MySQL autorizado para ${mode} DOC-98`);
  await collectValue(secrets, 'password', 'Contraseña MySQL autorizada', { secret: true });
  await collectConfirmation(secrets, 'environment', `¿Autoriza el ambiente configurado para ${mode} DOC-98?`);
  if (mode === 'activar') {
    await collectConfirmation(secrets, 'mutation', '¿Autoriza activar transversalmente las cuatro filas apagadas del manifiesto validado?');
    await collectConfirmation(secrets, 'rollback', '¿Confirma que el snapshot local se conservará para rollback hasta finalizar QA?');
  } else if (mode === 'activate-one') {
    await collectConfirmation(secrets, 'mutation', '¿Autoriza activar únicamente el módulo indicado para esta oleada DOC-98?');
    await collectConfirmation(secrets, 'rollback', '¿Confirma que se revertirá únicamente esta fila si la E2E falla?');
  } else if (mode === 'rollback' || mode === 'rollback-one') {
    await collectConfirmation(secrets, 'mutation', '¿Autoriza restaurar exactamente las filas modificadas por DOC-98?');
  }
  return secrets;
}

async function queryRows(connection, sql) {
  const [rows] = await connection.execute(sql, readiness.TARGET_TYPES);
  return rows;
}

async function assertCurrentReadiness(connection) {
  const [[schema]] = await connection.execute(readiness.SCHEMA_SQL);
  const [modules] = await connection.execute(readiness.MODULE_SQL, readiness.TARGET_TYPES);
  const [[ambiguity]] = await connection.execute(readiness.AMBIGUITY_SQL, readiness.TARGET_TYPES);
  const [[smtp]] = await connection.execute(readiness.SMTP_SQL);
  if (!readiness.evaluate(schema, modules, ambiguity, smtp).ready) throw new Error('DOC98_PREFLIGHT_CHANGED');
}

async function activate(connection, expected) {
  if (fs.existsSync(snapshotPath)) throw new Error('DOC98_SNAPSHOT_ALREADY_EXISTS');
  await assertCurrentReadiness(connection);
  await connection.beginTransaction();
  const targetRows = await queryRows(connection, TARGET_LOCK_SQL);
  const outsideRows = await queryRows(connection, OUTSIDE_SELECT_SQL);
  const state = classify(targetRows);
  if (!state.valid || state.normalized.length !== expected.total || state.on.length !== expected.on || state.off.length !== expected.off) {
    throw new Error('DOC98_MANIFEST_PRECONDITION_CHANGED');
  }
  const snapshot = {
    version: 1,
    recordedAtUtc: new Date().toISOString(),
    expected,
    targetRows: state.normalized,
    changedModuleIds: state.off.map(row => row.moduleId),
    outsideFingerprint: fingerprint(outsideRows)
  };
  writeSnapshot(snapshot);

  let affected = 0;
  for (const row of state.off) {
    const [result] = await connection.execute(ACTIVATE_SQL, [
      row.moduleId, row.companyId, row.moduleName, row.moduleType,
      row.requiredFlag, row.providerType, row.expiryMinutes
    ]);
    if (Number(result.affectedRows) !== 1) throw new Error('DOC98_WAVE_ROW_CONFLICT');
    affected += 1;
  }
  const after = classify(await queryRows(connection, TARGET_SELECT_SQL));
  if (!after.valid || after.normalized.length !== expected.total || after.on.length !== expected.total || affected !== expected.off) {
    throw new Error('DOC98_WAVE_POSTCONDITION_FAILED');
  }
  if (fingerprint(await queryRows(connection, OUTSIDE_SELECT_SQL)) !== snapshot.outsideFingerprint) throw new Error('DOC98_OUTSIDE_ROWS_CHANGED');
  await connection.commit();
  console.log('DOC98_ACTIVATION_COMMITTED=true');
  console.log(`DOC98_MANIFEST_ROWS=${expected.total}`);
  console.log(`DOC98_PREVIOUSLY_ON=${expected.on}`);
  console.log(`DOC98_ACTIVATED_ROWS=${affected}`);
  console.log('DOC98_ALL_TARGET_ROWS_ON=true');
  console.log('DOC98_OUTSIDE_ROWS_UNCHANGED=true');
  console.log('DOC98_SNAPSHOT_AVAILABLE=true');
}

async function verify(connection) {
  const snapshot = readSnapshot();
  await assertCurrentReadiness(connection);
  const current = classify(await queryRows(connection, TARGET_SELECT_SQL));
  const outsideOk = fingerprint(await queryRows(connection, OUTSIDE_SELECT_SQL)) === snapshot.outsideFingerprint;
  const targetOk = current.valid && current.normalized.length === snapshot.expected.total && current.on.length === snapshot.expected.total;
  console.log('DOC98_VERIFY_SELECT_ONLY=true');
  console.log(`DOC98_ALL_TARGET_ROWS_ON=${targetOk}`);
  console.log(`DOC98_OUTSIDE_ROWS_UNCHANGED=${outsideOk}`);
  console.log(`DOC98_VERIFY_READY=${targetOk && outsideOk}`);
  if (!targetOk || !outsideOk) process.exitCode = 2;
}

async function rollback(connection) {
  const snapshot = readSnapshot();
  await connection.beginTransaction();
  const current = classify(await queryRows(connection, TARGET_LOCK_SQL));
  if (!current.valid || current.normalized.length !== snapshot.expected.total || current.on.length !== snapshot.expected.total) {
    throw new Error('DOC98_ROLLBACK_PRECONDITION_CHANGED');
  }
  let affected = 0;
  for (const original of snapshot.targetRows.filter(row => snapshot.changedModuleIds.includes(row.moduleId))) {
    const [result] = await connection.execute(RESTORE_SQL, [
      original.requiredFlag, original.providerType, original.expiryMinutes,
      original.moduleId, original.companyId, original.moduleName, original.moduleType, original.expiryMinutes
    ]);
    if (Number(result.affectedRows) !== 1) throw new Error('DOC98_ROLLBACK_ROW_CONFLICT');
    affected += 1;
  }
  const restored = (await queryRows(connection, TARGET_SELECT_SQL)).map(normalizeRow);
  if (fingerprint(restored) !== fingerprint(snapshot.targetRows)) throw new Error('DOC98_ROLLBACK_POSTCONDITION_FAILED');
  if (fingerprint(await queryRows(connection, OUTSIDE_SELECT_SQL)) !== snapshot.outsideFingerprint) throw new Error('DOC98_OUTSIDE_ROWS_CHANGED');
  await connection.commit();
  console.log('DOC98_ROLLBACK_COMMITTED=true');
  console.log(`DOC98_ROLLBACK_ROWS=${affected}`);
  console.log('DOC98_SNAPSHOT_RESTORED=true');
  console.log('DOC98_OUTSIDE_ROWS_UNCHANGED=true');
}

function snapshotModule(snapshot, moduleName) {
  const normalizedName = String(moduleName || '').trim();
  const matches = snapshot.targetRows.filter(row => row.moduleName === normalizedName);
  if (matches.length !== 1 || !snapshot.changedModuleIds.includes(matches[0].moduleId)) {
    throw new Error('DOC98_WAVE_MODULE_INVALID');
  }
  return matches[0];
}

async function activateOne(connection, moduleName) {
  const snapshot = readSnapshot();
  const original = snapshotModule(snapshot, moduleName);
  await connection.beginTransaction();
  const currentRows = (await queryRows(connection, TARGET_LOCK_SQL)).map(normalizeRow);
  const current = currentRows.find(row => row.moduleId === original.moduleId);
  if (!current || fingerprint([current]) !== fingerprint([original])) throw new Error('DOC98_WAVE_PRECONDITION_CHANGED');
  const [result] = await connection.execute(ACTIVATE_SQL, [
    original.moduleId, original.companyId, original.moduleName, original.moduleType,
    original.requiredFlag, original.providerType, original.expiryMinutes
  ]);
  if (Number(result.affectedRows) !== 1) throw new Error('DOC98_WAVE_ROW_CONFLICT');
  const afterRows = (await queryRows(connection, TARGET_SELECT_SQL)).map(normalizeRow);
  const after = afterRows.find(row => row.moduleId === original.moduleId);
  if (!after || after.requiredFlag !== 1 || after.providerType !== 1 || after.expiryMinutes !== original.expiryMinutes) {
    throw new Error('DOC98_WAVE_POSTCONDITION_FAILED');
  }
  if (fingerprint(await queryRows(connection, OUTSIDE_SELECT_SQL)) !== snapshot.outsideFingerprint) throw new Error('DOC98_OUTSIDE_ROWS_CHANGED');
  await connection.commit();
  console.log('DOC98_WAVE_ACTIVATION_COMMITTED=true');
  console.log('DOC98_WAVE_TARGET_ON=true');
  console.log('DOC98_OUTSIDE_ROWS_UNCHANGED=true');
}

async function rollbackOne(connection, moduleName) {
  const snapshot = readSnapshot();
  const original = snapshotModule(snapshot, moduleName);
  await connection.beginTransaction();
  const currentRows = (await queryRows(connection, TARGET_LOCK_SQL)).map(normalizeRow);
  const current = currentRows.find(row => row.moduleId === original.moduleId);
  if (!current || current.requiredFlag !== 1 || current.providerType !== 1 || current.expiryMinutes !== original.expiryMinutes) {
    throw new Error('DOC98_WAVE_ROLLBACK_PRECONDITION_CHANGED');
  }
  const [result] = await connection.execute(RESTORE_SQL, [
    original.requiredFlag, original.providerType, original.expiryMinutes,
    original.moduleId, original.companyId, original.moduleName, original.moduleType, original.expiryMinutes
  ]);
  if (Number(result.affectedRows) !== 1) throw new Error('DOC98_WAVE_ROLLBACK_ROW_CONFLICT');
  const restoredRows = (await queryRows(connection, TARGET_SELECT_SQL)).map(normalizeRow);
  const restored = restoredRows.find(row => row.moduleId === original.moduleId);
  if (!restored || fingerprint([restored]) !== fingerprint([original])) throw new Error('DOC98_WAVE_ROLLBACK_POSTCONDITION_FAILED');
  if (fingerprint(await queryRows(connection, OUTSIDE_SELECT_SQL)) !== snapshot.outsideFingerprint) throw new Error('DOC98_OUTSIDE_ROWS_CHANGED');
  await connection.commit();
  console.log('DOC98_WAVE_ROLLBACK_COMMITTED=true');
  console.log('DOC98_WAVE_TARGET_RESTORED=true');
  console.log('DOC98_OUTSIDE_ROWS_UNCHANGED=true');
}

async function main() {
  requireInteractiveConsole();
  assertSqlScope();
  const mode = String(argument('--mode')).toLowerCase();
  if (!['activate', 'verify', 'rollback', 'activate-one', 'rollback-one'].includes(mode)) throw new Error('DOC98_MODE_INVALID');
  const metadata = readiness.loadDatabaseMetadata(path.resolve(argument('--db-profile')));
  const expected = mode === 'activate' ? {
    total: positiveIntegerArgument('--expected-total'),
    on: positiveIntegerArgument('--expected-on'),
    off: positiveIntegerArgument('--expected-off')
  } : null;
  if (expected && (expected.total < 1 || expected.on + expected.off !== expected.total || expected.off < 1)) throw new Error('DOC98_EXPECTED_COUNTS_INVALID');
  const moduleName = mode.endsWith('-one') ? argument('--module') : null;
  const label = mode === 'activate' ? 'activar' : mode;
  const secrets = await collectSecretsAndAuthorization(label);
  let connection;
  let stage = 'connection';
  let activationCommitted = false;
  try {
    connection = await mysql.createConnection({ ...metadata, user: secrets.user, password: secrets.password, multipleStatements: false });
    stage = mode;
    if (mode === 'activate') {
      await activate(connection, expected);
      activationCommitted = true;
    }
    else if (mode === 'verify') await verify(connection);
    else if (mode === 'rollback') await rollback(connection);
    else if (mode === 'activate-one') await activateOne(connection, moduleName);
    else await rollbackOne(connection, moduleName);
  } catch (error) {
    await connection?.rollback().catch(() => {});
    if (mode === 'activate' && !activationCommitted && fs.existsSync(snapshotPath)) {
      try { fs.unlinkSync(snapshotPath); } catch { /* se conserva para inspección segura */ }
    }
    throw new Error(safeFailureCode(error, stage));
  } finally {
    secrets.password = '';
    await connection?.end().catch(() => {});
  }
}

if (require.main === module) {
  main().catch(error => {
    const code = /^DOC98_[A-Z0-9_]{3,100}$/.test(error?.message || '') ? error.message : 'DOC98_ACTIVATION_FAILED';
    console.error(`Operación DOC-98 detenida de forma segura (${code}). No se mostraron secretos ni filas.`);
    process.exitCode = 1;
  });
}

module.exports = { ACTIVATE_SQL, OUTSIDE_SELECT_SQL, RESTORE_SQL, TARGET_LOCK_SQL, TARGET_SELECT_SQL, activateOne, assertSqlScope, classify, fingerprint, normalizeRow, rollbackOne, safeFailureCode, snapshotModule };
