'use strict';

const { execFile } = require('node:child_process');
const path = require('node:path');
const { promisify } = require('node:util');

const execute = promisify(execFile);
const scriptPath = path.resolve(__dirname, '..', 'query-doc32-odbc-fingerprint.ps1');
const finalActivityScriptPath = path.resolve(__dirname, '..', 'query-doc32-odbc-final-activity.ps1');
const activeActivityScriptPath = path.resolve(__dirname, '..', 'query-doc32-odbc-active-activity.ps1');

function required(environment, name) {
  const value = environment[name];
  return typeof value === 'string' && value.trim() ? value.trim() : null;
}

function prefixName(prefix, suffix) {
  if (typeof prefix !== 'string' || !/^DOC\d+_E2E$/.test(prefix)) {
    throw new Error('El prefijo ODBC E2E no es válido. No se mostró ningún valor.');
  }
  return `${prefix}_${suffix}`;
}

function assertDsn(environment = process.env, prefix = 'DOC32_E2E') {
  const name = prefixName(prefix, 'ODBC_DSN');
  const dsn = required(environment, name);
  if (!dsn || !/^[A-Za-z0-9 _.-]+$/.test(dsn)) {
    throw new Error(`${name} debe identificar un DSN ODBC permitido. No se mostró ningún valor.`);
  }
  return dsn;
}

function safeFailureMessage(error) {
  const stderr = typeof error?.stderr === 'string' ? error.stderr : '';
  if (/DOC32_ODBC_COLUMN_UNAVAILABLE/.test(stderr)) {
    return 'El control ODBC encontró una columna no disponible para la consulta SELECT. Actualice el perfil con el catálogo autorizado. No se mostraron credenciales, destino ni detalles internos.';
  }
  if (/DOC32_ODBC_TABLE_UNAVAILABLE/.test(stderr)) {
    return 'El control ODBC no encontró una tabla requerida para la consulta SELECT. Verifique el DSN y el catálogo autorizado. No se mostraron credenciales, destino ni detalles internos.';
  }
  if (/DOC32_ODBC_QUERY_UNSUPPORTED/.test(stderr)) {
    return 'El controlador ODBC no admite la forma actual de la consulta SELECT. Use una consulta compatible con el catálogo autorizado. No se mostraron credenciales, destino ni detalles internos.';
  }
  if (/DOC32_ODBC_RESULT_FAILED/.test(stderr)) {
    return 'El control ODBC ejecutó la consulta SELECT, pero no pudo materializar su resultado. Simplifique las columnas del perfil. No se mostraron credenciales, destino ni detalles internos.';
  }
  if (/DOC32_ODBC_FINGERPRINT_FAILED/.test(stderr)) {
    return 'El control ODBC ejecutó la consulta SELECT, pero no pudo generar su huella. No se mostraron credenciales, destino ni detalles internos.';
  }
  if (/DOC32_ODBC_OPEN_FAILED/.test(stderr)) {
    return 'No fue posible abrir el control ODBC de solo lectura. Verifique en la TTY la cuenta de lectura y el DSN autorizado. No se mostraron credenciales, destino ni detalles internos.';
  }
  if (/DOC32_ODBC_QUERY_FAILED/.test(stderr)) {
    return 'El control ODBC se abrió, pero no pudo ejecutar la consulta SELECT de solo lectura. Verifique las consultas aprobadas. No se mostraron credenciales, destino ni detalles internos.';
  }
  return 'No fue posible preparar el control ODBC de solo lectura. No se mostraron credenciales, destino ni detalles internos.';
}

function finalActivityMatches(marker) {
  if (marker === 'DOC32_ODBC_FINAL_ACTIVITY_MATCH') return true;
  if (marker === 'DOC32_ODBC_FINAL_ACTIVITY_MISMATCH' || marker === 'DOC32_ODBC_FINAL_ACTIVITY_AMBIGUOUS') return false;
  throw new Error('No fue posible comprobar la actividad final mediante el control ODBC de solo lectura. No se mostraron credenciales, destino ni detalles internos.');
}

async function queryFingerprint(sql, idTarea, environment = process.env, prefix = 'DOC32_E2E') {
  assertDsn(environment, prefix);
  try {
    const result = await execute('powershell.exe', [
      '-NoProfile',
      '-NonInteractive',
      '-ExecutionPolicy', 'Bypass',
      '-File', scriptPath,
      '-Sql', sql,
      '-TaskId', String(idTarea),
      '-EnvironmentPrefix', prefix
    ], { env: environment, windowsHide: true, maxBuffer: 1024 * 1024 });
    const fingerprint = result.stdout.trim();
    if (!/^[a-f0-9]{64}$/i.test(fingerprint)) throw new Error('invalid-fingerprint');
    return fingerprint;
  } catch (error) {
    throw new Error(safeFailureMessage(error));
  }
}

async function queryFinalActivity(idTarea, expectedActivityName, environment = process.env, prefix = 'DOC32_E2E') {
  assertDsn(environment, prefix);
  if (typeof expectedActivityName !== 'string' || !expectedActivityName.trim()) {
    throw new Error('No fue posible comprobar la actividad final mediante el control ODBC de solo lectura. No se mostraron credenciales, destino ni detalles internos.');
  }
  try {
    const result = await execute('powershell.exe', [
      '-NoProfile',
      '-NonInteractive',
      '-ExecutionPolicy', 'Bypass',
      '-File', finalActivityScriptPath,
      '-TaskId', String(idTarea),
      '-EnvironmentPrefix', prefix
    ], {
      env: { ...environment, [prefixName(prefix, 'EXPECTED_ACTIVITY_NAME')]: expectedActivityName.trim() },
      windowsHide: true,
      maxBuffer: 1024 * 1024
    });
    return finalActivityMatches(result.stdout.trim());
  } catch (error) {
    if (error?.message?.includes('No fue posible comprobar la actividad final')) throw error;
    throw new Error('No fue posible comprobar la actividad final mediante el control ODBC de solo lectura. No se mostraron credenciales, destino ni detalles internos.');
  }
}

async function queryActiveActivity(idTarea, environment = process.env, prefix = 'DOC32_E2E') {
  assertDsn(environment, prefix);
  try {
    const result = await execute('powershell.exe', [
      '-NoProfile',
      '-NonInteractive',
      '-ExecutionPolicy', 'Bypass',
      '-File', activeActivityScriptPath,
      '-TaskId', String(idTarea),
      '-EnvironmentPrefix', prefix
    ], { env: environment, windowsHide: true, maxBuffer: 1024 * 1024 });
    const output = result.stdout.trim();
    if (output === 'WORKFLOW_ODBC_ACTIVE_ACTIVITY_AMBIGUOUS') return null;
    const match = /^WORKFLOW_ODBC_ACTIVE_ACTIVITY=([^\r\n]{1,160})$/.exec(output);
    if (!match || !match[1].trim()) throw new Error('invalid-activity');
    return match[1].normalize('NFKC').trim();
  } catch {
    throw new Error('No fue posible consultar la actividad activa mediante el control ODBC de solo lectura. No se mostraron credenciales, destino ni detalles internos.');
  }
}

module.exports = {
  assertDsn,
  finalActivityMatches,
  prefixName,
  queryActiveActivity,
  queryFinalActivity,
  queryFingerprint,
  safeFailureMessage
};
