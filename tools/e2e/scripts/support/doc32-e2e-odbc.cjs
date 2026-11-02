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
  if (typeof prefix !== 'string' || !/^(?:DOC\d+(?:_DA)?|NOTES)_E2E$/.test(prefix)) {
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

async function queryRows(sql, parameters, environment = process.env, prefix = 'DOC32_E2E', runtime = {}) {
  assertDsn(environment, prefix);
  if(typeof sql!=='string'||!/^\s*SELECT\b/i.test(sql)||/;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO|LOCK|SLEEP|BENCHMARK)\b/i.test(sql)||!Array.isArray(parameters)||parameters.length>128||(sql.match(/\?/g)||[]).length!==parameters.length||parameters.some(v=>v!==null&&!(typeof v==='string'&&v.length<=2048)&&!(typeof v==='number'&&Number.isSafeInteger(v))))throw new Error('DOC111_SELECT_REQUIRED');
  const childEnvironment={...environment,[prefixName(prefix,'SELECT_PARAMETERS')]:JSON.stringify(parameters)};
  try {
    const result=await (runtime.execute||execute)('powershell.exe',['-NoProfile','-NonInteractive','-ExecutionPolicy','Bypass','-File',scriptPath,'-Sql',sql,'-ResultMode','Rows','-EnvironmentPrefix',prefix],{env:childEnvironment,windowsHide:true,maxBuffer:8*1024*1024});
    const rows=JSON.parse(result.stdout.replace(/^\uFEFF/,''));
    if(!Array.isArray(rows)||rows.some(r=>!r||typeof r!=='object'||Array.isArray(r)))throw new Error('invalid-rows');
    return rows;
  }catch(error){
    const markers={AUTHENTICATION_FAILED:'AUTHENTICATION_FAILED',DSN_UNAVAILABLE:'DSN_UNAVAILABLE',DRIVER_UNAVAILABLE:'DRIVER_UNAVAILABLE',DATABASE_ACCESS_DENIED:'DATABASE_ACCESS_DENIED',DATABASE_UNAVAILABLE:'DATABASE_UNAVAILABLE',CONNECTION_UNAVAILABLE:'CONNECTION_UNAVAILABLE',OPEN_FAILED:'OPEN_FAILED',QUERY_FAILED:'QUERY_FAILED',COLUMN_UNAVAILABLE:'COLUMN_UNAVAILABLE',TABLE_UNAVAILABLE:'TABLE_UNAVAILABLE',QUERY_UNSUPPORTED:'QUERY_UNSUPPORTED',RESULT_FAILED:'RESULT_FAILED',INPUT_FAILED:'INPUT_FAILED'};
    const marker=String(error?.stderr||'').trim();
    const suffix=Object.keys(markers).find(key=>marker==='DOC32_ODBC_'+key);
    throw new Error(suffix?'DOC111_SELECT_'+markers[suffix]:'DOC111_SELECT_FAILED');
  }
  finally{delete childEnvironment[prefixName(prefix,'SELECT_PARAMETERS')];delete childEnvironment[prefixName(prefix,'MYSQL_USER')];delete childEnvironment[prefixName(prefix,'MYSQL_PASSWORD')];}
}

module.exports = {
  assertDsn,
  finalActivityMatches,
  prefixName,
  queryActiveActivity,
  queryFinalActivity,
  queryFingerprint,
  queryRows,
  safeFailureMessage
};
