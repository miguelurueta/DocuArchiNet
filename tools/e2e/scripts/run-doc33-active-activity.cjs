'use strict';

const { collectValue, requireInteractiveConsole } = require('./support/interactive-e2e-console.cjs');
const { cleanEnvironment, loadProfile, validateProfile } = require('./support/workflow-e2e-orchestrator.cjs');
const { queryActiveActivity } = require('./support/doc32-e2e-odbc.cjs');

function fail(message) {
  throw new Error(message);
}

function parseArguments(argv) {
  const result = { profilePath: null, taskId: null };
  for (let index = 0; index < argv.length; index += 1) {
    const name = argv[index];
    const value = argv[index + 1];
    if (!['--profile', '--task'].includes(name) || !value || value.startsWith('--')) fail('Use --profile y --task.');
    index += 1;
    if (name === '--profile') result.profilePath = value;
    if (name === '--task') result.taskId = Number(value);
  }
  if (!result.profilePath || !Number.isSafeInteger(result.taskId) || result.taskId <= 0) fail('Use --profile y --task con una tarea positiva.');
  return result;
}

async function main(argv) {
  const options = parseArguments(argv);
  const profile = await loadProfile(options.profilePath);
  validateProfile(profile, 'doc33');
  if (![profile.uiExecutionTaskId, profile.uiLockTaskId].includes(options.taskId)) fail('La tarea de diagnóstico debe estar declarada en el perfil DOC-33.');
  requireInteractiveConsole();
  const secrets = {};
  await collectValue(secrets, 'DOC33_E2E_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(secrets, 'DOC33_E2E_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
  const environment = { ...process.env, DOC33_E2E_ODBC_DSN: profile.odbcDsn, ...secrets };
  try {
    const activity = await queryActiveActivity(options.taskId, environment, 'DOC33_E2E');
    process.stdout.write(activity ? `DOC33_${options.taskId}_ACTIVE_ACTIVITY=${activity}\n` : `DOC33_${options.taskId}_ACTIVE_ACTIVITY_AMBIGUOUS\n`);
  } finally {
    cleanEnvironment(environment, Object.keys(secrets));
  }
}

main(process.argv.slice(2)).catch((error) => {
  process.stderr.write(`${error?.message || 'No fue posible consultar la actividad activa. No se mostraron valores sensibles.'}\n`);
  process.exitCode = 2;
});
