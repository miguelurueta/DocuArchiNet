'use strict';

const fs = require('node:fs');
const path = require('node:path');

const required = [
  'DOC85_E2E_BASE_URL', 'DOC85_E2E_MODULE', 'DOC85_E2E_AUTHORIZED_USER',
  'DOC85_E2E_AUTHORIZED_PASSWORD', 'DOC85_E2E_PAGE_PATH', 'DOC85_E2E_STATE_RECORD_ID',
  'DOC85_E2E_EXPECTED_RADICADO', 'DOC85_E2E_TYPOLOGY_TEXT',
  'DOC85_E2E_POSITIVE_FIXTURE_PATH', 'DOC85_E2E_NEGATIVE_FIXTURE_PATH',
  'DOC85_E2E_MYSQL_HOST', 'DOC85_E2E_MYSQL_PORT', 'DOC85_E2E_MYSQL_DATABASE',
  'DOC85_E2E_MYSQL_USER', 'DOC85_E2E_MYSQL_PASSWORD', 'DOC85_E2E_CONTEXT_SQL',
  'DOC85_E2E_DAT_ADIC_SQL', 'DOC85_E2E_DOCUMENT_COUNT_SQL',
  'DOC85_E2E_ENVIRONMENT_AUTHORIZED', 'DOC85_E2E_ACCOUNT_AUTHORIZED',
  'DOC85_E2E_EXECUTION_AUTHORIZED', 'DOC85_E2E_DISCARDABLE_RESOURCE_AUTHORIZED'
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

const missing = required.filter(name => !hasValue(name));
if (missing.length > 0) fail(`Faltan variables requeridas para DOC-85: ${missing.join(', ')}. No se mostraron valores.`);

try { new URL(process.env.DOC85_E2E_BASE_URL); } catch { fail('DOC85_E2E_BASE_URL debe ser absoluta y válida.'); }
if (!/^\d+$/.test(process.env.DOC85_E2E_STATE_RECORD_ID) || Number(process.env.DOC85_E2E_STATE_RECORD_ID) <= 0) {
  fail('DOC85_E2E_STATE_RECORD_ID debe ser un entero positivo.');
}
if (!/^\d+$/.test(process.env.DOC85_E2E_MYSQL_PORT) || Number(process.env.DOC85_E2E_MYSQL_PORT) <= 0) {
  fail('DOC85_E2E_MYSQL_PORT debe ser un entero positivo.');
}
for (const authorization of [
  'DOC85_E2E_ENVIRONMENT_AUTHORIZED', 'DOC85_E2E_ACCOUNT_AUTHORIZED',
  'DOC85_E2E_EXECUTION_AUTHORIZED', 'DOC85_E2E_DISCARDABLE_RESOURCE_AUTHORIZED'
]) {
  if (process.env[authorization].toLowerCase() !== 'true') fail(`${authorization} debe ser exactamente true.`);
}
for (const query of ['DOC85_E2E_CONTEXT_SQL', 'DOC85_E2E_DAT_ADIC_SQL', 'DOC85_E2E_DOCUMENT_COUNT_SQL']) {
  if (!isSingleReadOnlyQuery(process.env[query])) fail(`${query} debe ser un único SELECT con exactamente un parámetro ?.`);
}
for (const fixture of ['DOC85_E2E_POSITIVE_FIXTURE_PATH', 'DOC85_E2E_NEGATIVE_FIXTURE_PATH']) {
  const packageRoot = path.resolve(__dirname, '..');
  const resolved = path.resolve(packageRoot, process.env[fixture]);
  if (!resolved.startsWith(`${packageRoot}${path.sep}`)) fail(`${fixture} debe permanecer dentro del paquete E2E.`);
  if (!fs.existsSync(resolved) || path.extname(resolved).toLowerCase() !== '.pdf') fail(`${fixture} debe apuntar a un PDF existente dentro del paquete E2E.`);
}
