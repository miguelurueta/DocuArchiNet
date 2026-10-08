'use strict';

const fs = require('node:fs');
const path = require('node:path');

const required = [
  'DOC90_E2E_BASE_URL', 'DOC90_E2E_MODULE', 'DOC90_E2E_AUTHORIZED_USER',
  'DOC90_E2E_AUTHORIZED_PASSWORD', 'DOC90_E2E_PAGE_PATH',
  'DOC90_E2E_EXPECTED_RADICADO', 'DOC90_E2E_TYPOLOGY_TEXT', 'DOC90_E2E_FIXTURE_PATH',
  'DOC90_E2E_MYSQL_HOST', 'DOC90_E2E_MYSQL_PORT', 'DOC90_E2E_MYSQL_DATABASE',
  'DOC90_E2E_MYSQL_USER', 'DOC90_E2E_MYSQL_PASSWORD', 'DOC90_E2E_CONTEXT_SQL',
  'DOC90_E2E_DOCUMENT_COUNT_SQL', 'DOC90_E2E_ENVIRONMENT_AUTHORIZED',
  'DOC90_E2E_ACCOUNT_AUTHORIZED', 'DOC90_E2E_EXECUTION_AUTHORIZED',
  'DOC90_E2E_DISCARDABLE_RESOURCE_AUTHORIZED'
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
if (missing.length > 0) fail(`Faltan variables requeridas para DOC-90: ${missing.join(', ')}. No se mostraron valores.`);

try { new URL(process.env.DOC90_E2E_BASE_URL); } catch { fail('DOC90_E2E_BASE_URL debe ser absoluta y válida.'); }
for (const integer of ['DOC90_E2E_MYSQL_PORT']) {
  if (!/^\d+$/.test(process.env[integer]) || Number(process.env[integer]) <= 0) fail(`${integer} debe ser un entero positivo.`);
}
for (const authorization of [
  'DOC90_E2E_ENVIRONMENT_AUTHORIZED', 'DOC90_E2E_ACCOUNT_AUTHORIZED',
  'DOC90_E2E_EXECUTION_AUTHORIZED', 'DOC90_E2E_DISCARDABLE_RESOURCE_AUTHORIZED'
]) {
  if (process.env[authorization].toLowerCase() !== 'true') fail(`${authorization} debe ser exactamente true.`);
}
for (const query of ['DOC90_E2E_CONTEXT_SQL', 'DOC90_E2E_DOCUMENT_COUNT_SQL']) {
  if (!isSingleReadOnlyQuery(process.env[query])) fail(`${query} debe ser un único SELECT con exactamente un parámetro ?.`);
}

const packageRoot = path.resolve(__dirname, '..');
const fixture = path.resolve(packageRoot, process.env.DOC90_E2E_FIXTURE_PATH);
if (!fixture.startsWith(`${packageRoot}${path.sep}`)) fail('DOC90_E2E_FIXTURE_PATH debe permanecer dentro del paquete E2E.');
if (!fs.existsSync(fixture) || path.extname(fixture).toLowerCase() !== '.pdf') fail('DOC90_E2E_FIXTURE_PATH debe apuntar a un PDF existente dentro del paquete E2E.');
