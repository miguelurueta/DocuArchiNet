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
  const result = {
    source: null,
    destination: null,
    executionTaskId: null,
    lockTaskId: null,
    executionFinalActivityName: null
  };
  for (let index = 0; index < argv.length; index += 1) {
    const name = argv[index];
    const value = argv[index + 1];
    if (!['--source', '--destination', '--execution-task', '--lock-task', '--execution-final-activity'].includes(name) || !value || value.startsWith('--')) {
      fail('Use --source, --destination, --execution-task, --lock-task y opcionalmente --execution-final-activity.');
    }
    index += 1;
    if (name === '--source') result.source = value;
    if (name === '--destination') result.destination = value;
    if (name === '--execution-task') result.executionTaskId = positiveInteger(value, '--execution-task');
    if (name === '--lock-task') result.lockTaskId = positiveInteger(value, '--lock-task');
    if (name === '--execution-final-activity') result.executionFinalActivityName = value;
  }
  if (!result.source || !result.destination || !result.executionTaskId || !result.lockTaskId) {
    fail('Use --source, --destination, --execution-task y --lock-task.');
  }
  if (result.executionTaskId === result.lockTaskId) fail('Las tareas de ejecución y bloqueo UI deben ser distintas.');
  return result;
}

function createDoc33Profile(source, executionTaskId, lockTaskId, executionFinalActivityName = null) {
  validateProfile(source, 'doc32');
  if (executionFinalActivityName !== null && (typeof executionFinalActivityName !== 'string' || !executionFinalActivityName.trim())) {
    fail('--execution-final-activity debe ser un nombre no sensible de actividad.');
  }
  const activityForTask = (taskId) => taskId === source.concurrencyTaskId ? source.concurrencyActivityName : source.executionActivityName;
  const profile = {
    doc: 'doc33',
    environment: source.environment,
    baseUrl: source.baseUrl,
    ignoreHttpsErrors: source.ignoreHttpsErrors,
    module: source.module,
    odbcDsn: source.odbcDsn,
    uiExecutionTaskId: positiveInteger(executionTaskId, 'uiExecutionTaskId'),
    previewActivityNames: [...source.previewActivityNames],
    uiExecutionActivityName: activityForTask(executionTaskId),
    uiExecutionFinalActivityName: executionFinalActivityName || source.executionFinalActivityName,
    uiLockTaskId: positiveInteger(lockTaskId, 'uiLockTaskId'),
    uiLockActivityName: activityForTask(lockTaskId),
    uiLockFinalActivityName: source.executionFinalActivityName,
    taskStateSql: source.taskStateSql,
    auditSql: source.auditSql,
    previewMaxMs: source.previewMaxMs,
    uiExecutionMaxMs: source.executionMaxMs,
    uiLockMaxMs: Math.max(source.executionMaxMs, 180000)
  };
  if (profile.uiExecutionTaskId === profile.uiLockTaskId) fail('Las tareas de ejecución y bloqueo UI deben ser distintas.');
  validateProfile(profile, 'doc33');
  return profile;
}

async function main(argv) {
  const options = parseArguments(argv);
  const source = await loadProfile(options.source);
  const profile = createDoc33Profile(source, options.executionTaskId, options.lockTaskId, options.executionFinalActivityName);
  const destination = path.resolve(options.destination);
  await fs.writeFile(destination, `${JSON.stringify(profile, null, 2)}\n`, 'utf8');
  process.stdout.write('Perfil DOC-33 no sensible creado.\n');
}

if (require.main === module) {
  main(process.argv.slice(2)).catch((error) => {
    process.stderr.write(`${error?.message || 'No fue posible crear el perfil DOC-33.'}\n`);
    process.exitCode = 2;
  });
}

module.exports = {
  createDoc33Profile,
  parseArguments
};
