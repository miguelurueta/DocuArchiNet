'use strict';

const mode = process.argv[2];
const authenticated = [
  'DOC32_E2E_BASE_URL',
  'DOC32_E2E_MODULE',
  'DOC32_E2E_AUTHORIZED_USER',
  'DOC32_E2E_AUTHORIZED_PASSWORD'
];
const readOnlyControls = [
  'DOC32_E2E_ODBC_DSN',
  'DOC32_E2E_MYSQL_USER',
  'DOC32_E2E_MYSQL_PASSWORD',
  'DOC32_E2E_TASK_STATE_SQL',
  'DOC32_E2E_AUDIT_SQL'
];
const approvedEnvironment = [
  'DOC32_E2E_ENVIRONMENT',
  'DOC32_E2E_ENVIRONMENT_AUTHORIZED'
];
const requiredByMode = {
  anonymous: ['DOC32_E2E_BASE_URL'],
  validation: authenticated,
  preview: [...authenticated, ...approvedEnvironment, 'DOC32_E2E_EXECUTION_TASK_ID', ...readOnlyControls, 'DOC32_E2E_PREVIEW_MAX_MS'],
  execute: [...authenticated, ...approvedEnvironment, 'DOC32_E2E_EXECUTION_AUTHORIZED', 'DOC32_E2E_EXECUTION_TASK_ID', ...readOnlyControls, 'DOC32_E2E_EXECUTION_MAX_MS'],
  concurrency: [...authenticated, ...approvedEnvironment, 'DOC32_E2E_EXECUTION_AUTHORIZED', 'DOC32_E2E_CONCURRENCY_AUTHORIZED', 'DOC32_E2E_CONCURRENCY_TASK_ID', ...readOnlyControls, 'DOC32_E2E_CONCURRENCY_MAX_MS']
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

if (!Object.hasOwn(requiredByMode, mode)) {
  fail('Modo DOC-32 inválido. Use anonymous, validation, preview, execute o concurrency.');
}

const missing = requiredByMode[mode].filter((name) => !required(name));
if (missing.length > 0) {
  fail(`Faltan variables DOC-32 requeridas: ${missing.join(', ')}. No se mostraron valores.`);
}

try {
  new URL(required('DOC32_E2E_BASE_URL'));
} catch {
  fail('DOC32_E2E_BASE_URL debe ser una URL absoluta válida.');
}

if (mode === 'preview' || mode === 'execute') {
  if (!isPositiveInteger(required('DOC32_E2E_EXECUTION_TASK_ID'))) {
    fail('DOC32_E2E_EXECUTION_TASK_ID debe ser un entero positivo.');
  }
}
if (mode === 'concurrency' && !isPositiveInteger(required('DOC32_E2E_CONCURRENCY_TASK_ID'))) {
  fail('DOC32_E2E_CONCURRENCY_TASK_ID debe ser un entero positivo.');
}

if (mode === 'preview' || mode === 'execute' || mode === 'concurrency') {
  if (required('DOC32_E2E_ENVIRONMENT_AUTHORIZED').toLowerCase() !== 'true') {
    fail('DOC32_E2E_ENVIRONMENT_AUTHORIZED debe ser exactamente true para la corrida protegida.');
  }
  assertReadOnlySql(required('DOC32_E2E_TASK_STATE_SQL'), 'DOC32_E2E_TASK_STATE_SQL');
  assertReadOnlySql(required('DOC32_E2E_AUDIT_SQL'), 'DOC32_E2E_AUDIT_SQL');
}

if (mode === 'execute' || mode === 'concurrency') {
  if (required('DOC32_E2E_EXECUTION_AUTHORIZED').toLowerCase() !== 'true') {
    fail('DOC32_E2E_EXECUTION_AUTHORIZED debe ser exactamente true para cambiar una tarea descartable.');
  }
}
if (mode === 'concurrency' && required('DOC32_E2E_CONCURRENCY_AUTHORIZED').toLowerCase() !== 'true') {
  fail('DOC32_E2E_CONCURRENCY_AUTHORIZED debe ser exactamente true para la carrera de dos solicitudes.');
}

for (const name of ['DOC32_E2E_PREVIEW_MAX_MS', 'DOC32_E2E_EXECUTION_MAX_MS', 'DOC32_E2E_CONCURRENCY_MAX_MS']) {
  if (required(name) && !isPositiveInteger(required(name))) {
    fail(`${name} debe ser un entero positivo cuando se configure.`);
  }
}
