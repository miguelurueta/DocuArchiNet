'use strict';

const mode = process.argv[2];
const authenticated = [
  'NOTES_E2E_BASE_URL',
  'NOTES_E2E_MODULE',
  'NOTES_E2E_AUTHORIZED_USER',
  'NOTES_E2E_AUTHORIZED_PASSWORD'
];
const approvedEnvironment = [
  'NOTES_E2E_ENVIRONMENT',
  'NOTES_E2E_ENVIRONMENT_AUTHORIZED'
];
const readOnlyControls = [
  'NOTES_E2E_ODBC_DSN',
  'NOTES_E2E_MYSQL_USER',
  'NOTES_E2E_MYSQL_PASSWORD',
  'NOTES_E2E_TASK_STATE_SQL',
  'NOTES_E2E_AUDIT_SQL'
];
const requiredByMode = {
  anonymous: ['NOTES_E2E_BASE_URL'],
  read: [...authenticated, ...approvedEnvironment, 'NOTES_E2E_READ_TASK_ID', ...readOnlyControls, 'NOTES_E2E_READ_MAX_MS'],
  write: [...authenticated, ...approvedEnvironment, 'NOTES_E2E_EXECUTION_AUTHORIZED', 'NOTES_E2E_WRITE_TASK_ID', ...readOnlyControls, 'NOTES_E2E_WRITE_MAX_MS'],
  concurrency: [...authenticated, ...approvedEnvironment, 'NOTES_E2E_EXECUTION_AUTHORIZED', 'NOTES_E2E_CONCURRENCY_AUTHORIZED', 'NOTES_E2E_CONCURRENCY_TASK_ID', 'NOTES_E2E_CONCURRENCY_NOTE_ID', ...readOnlyControls, 'NOTES_E2E_CONCURRENCY_MAX_MS']
};

function fail(message) {
  console.error(message);
  process.exit(2);
}

function required(name) {
  const value = process.env[name];
  return typeof value === 'string' && value.trim() ? value.trim() : null;
}

function isPositiveInteger(value) {
  return /^\d+$/.test(value || '') && Number(value) > 0;
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql || '') ||
      /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) ||
      (sql.match(/\?/g) || []).length !== 1) {
    fail(`${name} debe ser una única consulta SELECT de solo lectura con exactamente un parámetro ?. No se mostró su valor.`);
  }
}

function assertServicePath() {
  const servicePath = required('NOTES_E2E_SERVICE_PATH');
  if (servicePath && (!/^[A-Za-z0-9_./-]+\.asmx$/i.test(servicePath) || servicePath.includes('..'))) {
    fail('NOTES_E2E_SERVICE_PATH debe ser una ruta relativa segura hacia un ASMX. No se mostró su valor.');
  }
}

if (!Object.hasOwn(requiredByMode, mode)) {
  fail('Modo Notes E2E inválido. Use anonymous, read, write o concurrency.');
}

const missing = requiredByMode[mode].filter((name) => !required(name));
if (missing.length > 0) {
  fail(`Faltan variables Notes E2E requeridas: ${missing.join(', ')}. No se mostraron valores.`);
}

try {
  const baseUrl = new URL(required('NOTES_E2E_BASE_URL'));
  if (baseUrl.username || baseUrl.password) throw new Error('credential-url');
} catch {
  fail('NOTES_E2E_BASE_URL debe ser una URL absoluta válida y sin credenciales.');
}

assertServicePath();

if (mode === 'read' || mode === 'write' || mode === 'concurrency') {
  const dsn = required('NOTES_E2E_ODBC_DSN');
  if (!dsn || !/^[A-Za-z0-9 _.-]+$/.test(dsn)) {
    fail('NOTES_E2E_ODBC_DSN debe identificar un DSN ODBC permitido. No se mostró ningún valor.');
  }
  if (required('NOTES_E2E_MYSQL_URL')) {
    fail('NOTES_E2E_MYSQL_URL no está permitida; use el DSN ODBC y credenciales efímeras. No se mostró ningún valor.');
  }
}

if (mode === 'read' || mode === 'write' || mode === 'concurrency') {
  if (required('NOTES_E2E_ENVIRONMENT_AUTHORIZED').toLowerCase() !== 'true') {
    fail('NOTES_E2E_ENVIRONMENT_AUTHORIZED debe ser exactamente true para la corrida protegida.');
  }
  const taskVariable = mode === 'read' ? 'NOTES_E2E_READ_TASK_ID' : mode === 'write' ? 'NOTES_E2E_WRITE_TASK_ID' : 'NOTES_E2E_CONCURRENCY_TASK_ID';
  if (!isPositiveInteger(required(taskVariable))) {
    fail(`${taskVariable} debe ser un entero positivo.`);
  }
  if (mode === 'concurrency' && !isPositiveInteger(required('NOTES_E2E_CONCURRENCY_NOTE_ID'))) {
    fail('NOTES_E2E_CONCURRENCY_NOTE_ID debe ser un entero positivo.');
  }
  assertReadOnlySql(required('NOTES_E2E_TASK_STATE_SQL'), 'NOTES_E2E_TASK_STATE_SQL');
  assertReadOnlySql(required('NOTES_E2E_AUDIT_SQL'), 'NOTES_E2E_AUDIT_SQL');
}

if (mode === 'write' || mode === 'concurrency') {
  if (required('NOTES_E2E_EXECUTION_AUTHORIZED').toLowerCase() !== 'true') {
    fail('NOTES_E2E_EXECUTION_AUTHORIZED debe ser exactamente true para operar sobre una tarea descartable.');
  }
}

if (mode === 'concurrency' && required('NOTES_E2E_CONCURRENCY_AUTHORIZED').toLowerCase() !== 'true') {
  fail('NOTES_E2E_CONCURRENCY_AUTHORIZED debe ser exactamente true para la carrera de dos solicitudes.');
}

for (const name of ['NOTES_E2E_READ_MAX_MS', 'NOTES_E2E_WRITE_MAX_MS', 'NOTES_E2E_CONCURRENCY_MAX_MS']) {
  if (required(name) && !isPositiveInteger(required(name))) {
    fail(`${name} debe ser un entero positivo cuando se configure.`);
  }
}
