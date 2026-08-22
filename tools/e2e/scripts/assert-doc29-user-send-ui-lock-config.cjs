'use strict';

const required = [
  'DOC28_E2E_BASE_URL',
  'DOC28_E2E_MODULE',
  'DOC28_E2E_AUTHORIZED_USER',
  'DOC28_E2E_AUTHORIZED_PASSWORD',
  'DOC28_E2E_EXECUTION_AUTHORIZED',
  'DOC29_E2E_UI_LOCK_AUTHORIZED',
  'DOC28_E2E_TASK_ID',
  'DOC28_E2E_MYSQL_URL',
  'DOC28_E2E_TASK_STATE_SQL',
  'DOC28_E2E_AUDIT_SQL'
];

function fail(message) {
  console.error(message);
  process.exit(2);
}

function hasValue(name) {
  return typeof process.env[name] === 'string' && process.env[name].trim().length > 0;
}

function isSingleReadOnlyQuery(sql) {
  return /^\s*SELECT\b/i.test(sql || '') &&
    !/;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) &&
    (sql.match(/\?/g) || []).length === 1;
}

const missing = required.filter((name) => !hasValue(name));
if (missing.length > 0) fail(`Faltan variables requeridas para el E2E UI DOC-29: ${missing.join(', ')}. No se mostraron valores.`);

try {
  new URL(process.env.DOC28_E2E_BASE_URL);
} catch {
  fail('DOC28_E2E_BASE_URL debe ser una URL absoluta válida.');
}

if (!/^\d+$/.test(process.env.DOC28_E2E_TASK_ID) || Number(process.env.DOC28_E2E_TASK_ID) <= 0) {
  fail('DOC28_E2E_TASK_ID debe ser un entero positivo.');
}

if (process.env.DOC28_E2E_EXECUTION_AUTHORIZED.toLowerCase() !== 'true' ||
    process.env.DOC29_E2E_UI_LOCK_AUTHORIZED.toLowerCase() !== 'true') {
  fail('Las banderas DOC28_E2E_EXECUTION_AUTHORIZED y DOC29_E2E_UI_LOCK_AUTHORIZED deben ser exactamente true.');
}

for (const name of ['DOC28_E2E_TASK_STATE_SQL', 'DOC28_E2E_AUDIT_SQL']) {
  if (!isSingleReadOnlyQuery(process.env[name])) {
    fail(`${name} debe ser una única consulta SELECT de solo lectura con exactamente un parámetro ?.`);
  }
}
