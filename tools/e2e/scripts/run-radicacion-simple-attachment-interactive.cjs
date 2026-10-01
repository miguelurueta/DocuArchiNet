'use strict';

const fs = require('node:fs');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole,
  runChild
} = require('./support/interactive-e2e-console.cjs');

const allowedProfileFields = new Set([
  'baseUrl', 'ignoreHttpsErrors', 'module', 'pagePath', 'stateRecordId', 'expectedRadicado',
  'typologyText', 'positiveFixturePath', 'negativeFixturePath', 'mysqlHost', 'mysqlPort',
  'mysqlDatabase', 'contextSql', 'datAdicSql', 'documentCountSql',
  'timeoutMilliseconds'
]);

function fail(message) {
  console.error(message);
  process.exitCode = 2;
}

function profilePath() {
  const index = process.argv.indexOf('--profile');
  if (index < 0 || !process.argv[index + 1]) throw new Error('Debe indicar --profile <archivo-json-no-sensible>.');
  return path.resolve(process.argv[index + 1]);
}

function loadProfile(target) {
  const profile = JSON.parse(fs.readFileSync(target, 'utf8'));
  const unknown = Object.keys(profile).filter(key => !allowedProfileFields.has(key));
  if (unknown.length > 0) throw new Error(`El perfil DOC-85 contiene campos no permitidos: ${unknown.join(', ')}.`);
  if (profile.ignoreHttpsErrors !== true && profile.ignoreHttpsErrors !== false) throw new Error('ignoreHttpsErrors debe ser booleano.');
  return profile;
}

async function main() {
  requireInteractiveConsole();
  const profile = loadProfile(profilePath());
  const values = {
    DOC85_E2E_BASE_URL: String(profile.baseUrl || ''),
    DOC85_E2E_MODULE: String(profile.module || ''),
    DOC85_E2E_PAGE_PATH: String(profile.pagePath || ''),
    DOC85_E2E_STATE_RECORD_ID: String(profile.stateRecordId || ''),
    DOC85_E2E_EXPECTED_RADICADO: String(profile.expectedRadicado || ''),
    DOC85_E2E_TYPOLOGY_TEXT: String(profile.typologyText || ''),
    DOC85_E2E_POSITIVE_FIXTURE_PATH: String(profile.positiveFixturePath || ''),
    DOC85_E2E_NEGATIVE_FIXTURE_PATH: String(profile.negativeFixturePath || ''),
    DOC85_E2E_MYSQL_HOST: String(profile.mysqlHost || ''),
    DOC85_E2E_MYSQL_PORT: String(profile.mysqlPort || ''),
    DOC85_E2E_MYSQL_DATABASE: String(profile.mysqlDatabase || ''),
    DOC85_E2E_CONTEXT_SQL: String(profile.contextSql || ''),
    DOC85_E2E_DAT_ADIC_SQL: String(profile.datAdicSql || ''),
    DOC85_E2E_DOCUMENT_COUNT_SQL: String(profile.documentCountSql || ''),
    DOC85_E2E_TIMEOUT_MS: String(profile.timeoutMilliseconds || 120000),
    DOC85_E2E_IGNORE_HTTPS_ERRORS: String(profile.ignoreHttpsErrors)
  };

  await collectValue(values, 'DOC85_E2E_AUTHORIZED_USER', 'Cuenta autorizada de Radicación Simplificada');
  await collectValue(values, 'DOC85_E2E_AUTHORIZED_PASSWORD', 'Contraseña', { secret: true });
  await collectValue(values, 'DOC85_E2E_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(values, 'DOC85_E2E_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
  await collectConfirmation(values, 'DOC85_E2E_ENVIRONMENT_AUTHORIZED', '¿Autoriza el ambiente de pruebas indicado?');
  await collectConfirmation(values, 'DOC85_E2E_ACCOUNT_AUTHORIZED', '¿Autoriza esta cuenta para la corrida DOC-85?');
  await collectConfirmation(values, 'DOC85_E2E_EXECUTION_AUTHORIZED', '¿Autoriza las dos cargas reales de prueba?');
  await collectConfirmation(values, 'DOC85_E2E_DISCARDABLE_RESOURCE_AUTHORIZED', '¿Confirma que el registro y la tarea son descartables?');

  const environment = { ...process.env, ...values };
  try {
    const validation = await runChild(process.execPath, [path.resolve(__dirname, 'assert-radicacion-simple-attachment-config.cjs')], path.resolve(__dirname, '..'), environment);
    if (validation.code !== 0) {
      process.exitCode = validation.code;
      return;
    }
    const cli = path.resolve(__dirname, '..', 'node_modules', '@playwright', 'test', 'cli.js');
    const result = await runChild(process.execPath, [cli, 'test', 'tests/radicacion-simple-attachment.spec.cjs', '--reporter=list'], path.resolve(__dirname, '..'), environment);
    process.exitCode = result.code;
  } finally {
    for (const name of Object.keys(values)) delete environment[name];
  }
}

main().catch(error => {
  if (error?.message?.includes('consola interactiva') || /^DOC85_E2E_/.test(error?.message || '')) console.error(error.message);
  else console.error('La E2E DOC-85 se detuvo antes de ejecutarse. No se mostraron valores sensibles.');
  process.exitCode = 2;
});
