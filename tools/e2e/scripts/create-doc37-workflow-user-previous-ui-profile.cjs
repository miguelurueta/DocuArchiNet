'use strict';

const fs = require('node:fs/promises');
const path = require('node:path');
const { loadProfile, validateProfile } = require('./support/workflow-e2e-orchestrator.cjs');

function fail(message) {
  throw new Error(message);
}

function positiveInteger(value, name) {
  const parsed = Number(value);
  if (!Number.isSafeInteger(parsed) || parsed <= 0) fail(`${name} debe ser un entero positivo.`);
  return parsed;
}

function parseArguments(argv) {
  const result = { source: null, destination: null, environment: null, executionTaskId: null, lockTaskId: null };
  for (let index = 0; index < argv.length; index += 1) {
    const name = argv[index];
    const value = argv[index + 1];
    if (!['--source', '--destination', '--environment', '--execution-task', '--lock-task'].includes(name) || !value || value.startsWith('--')) {
      fail('Use --source, --destination, --environment, --execution-task y --lock-task.');
    }
    index += 1;
    if (name === '--source') result.source = value;
    if (name === '--destination') result.destination = value;
    if (name === '--environment') result.environment = value.trim();
    if (name === '--execution-task') result.executionTaskId = positiveInteger(value, '--execution-task');
    if (name === '--lock-task') result.lockTaskId = positiveInteger(value, '--lock-task');
  }
  if (!result.source || !result.destination || !result.environment || !result.executionTaskId || !result.lockTaskId) {
    fail('Use --source, --destination, --environment, --execution-task y --lock-task.');
  }
  if (result.executionTaskId === result.lockTaskId) fail('Las tareas de ejecución y bloqueo UI deben ser distintas.');
  return result;
}

function createDoc37Profile(source, environment, executionTaskId, lockTaskId) {
  validateProfile(source, 'doc36');
  if (typeof environment !== 'string' || !environment.trim()) fail('environment debe identificar el ambiente de pruebas autorizado.');
  const profile = {
    doc: 'doc37',
    environment: environment.trim(),
    baseUrl: source.baseUrl,
    ignoreHttpsErrors: source.ignoreHttpsErrors,
    module: source.module,
    odbcDsn: source.odbcDsn,
    uiExecutionTaskId: positiveInteger(executionTaskId, 'uiExecutionTaskId'),
    uiLockTaskId: positiveInteger(lockTaskId, 'uiLockTaskId'),
    taskStateSql: source.taskStateSql,
    auditSql: source.auditSql,
    previewMaxMs: source.previewMaxMs,
    uiExecutionMaxMs: source.executionMaxMs,
    uiLockMaxMs: Math.max(source.executionMaxMs, 180000)
  };
  if (profile.uiExecutionTaskId === profile.uiLockTaskId) fail('Las tareas de ejecución y bloqueo UI deben ser distintas.');
  validateProfile(profile, 'doc37');
  return profile;
}

async function main(argv) {
  const options = parseArguments(argv);
  const source = await loadProfile(options.source);
  const profile = createDoc37Profile(source, options.environment, options.executionTaskId, options.lockTaskId);
  const destination = path.resolve(options.destination);
  await fs.writeFile(destination, `${JSON.stringify(profile, null, 2)}\n`, 'utf8');
  process.stdout.write('Perfil DOC-37 no sensible creado.\n');
}

if (require.main === module) {
  main(process.argv.slice(2)).catch((error) => {
    process.stderr.write(`${error?.message || 'No fue posible crear el perfil DOC-37.'}\n`);
    process.exitCode = 2;
  });
}

module.exports = {
  createDoc37Profile,
  parseArguments
};
