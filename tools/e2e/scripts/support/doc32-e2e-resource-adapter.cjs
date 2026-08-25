'use strict';

const { queryFingerprint } = require('./doc32-e2e-odbc.cjs');

const EMPTY_FINGERPRINT = 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855';

function normalizeActivityName(value) {
  return typeof value === 'string' ? value.normalize('NFKC').trim().toLocaleLowerCase() : '';
}

function descriptorFor(profile, role) {
  if (role === 'execution') {
    return { taskId: profile.executionTaskId, activity: profile.executionActivityName };
  }
  if (role === 'concurrency') {
    return { taskId: profile.concurrencyTaskId, activity: profile.concurrencyActivityName };
  }
  throw new Error('role-invalid');
}

async function observeGeneration({ profile, environment, descriptor }) {
  return queryFingerprint(profile.taskStateSql, descriptor.taskId, environment);
}

async function preflight(context) {
  const generation = await observeGeneration(context);
  if (generation === EMPTY_FINGERPRINT) {
    return { available: false, code: 'E2E_RESOURCE_TASK_UNAVAILABLE' };
  }
  return {
    available: true,
    code: 'E2E_RESOURCE_READY',
    resourceKey: `workflow-task:${context.descriptor.taskId}`,
    generation
  };
}

function assessPreviewAvailability(preview, activityName) {
  if (preview?.Error || !Array.isArray(preview?.Destinos)) {
    return { available: false, code: 'E2E_RESOURCE_TASK_UNAVAILABLE' };
  }
  const expected = normalizeActivityName(activityName);
  const matches = preview.Destinos.filter((candidate) => normalizeActivityName(candidate?.NombreActividad) === expected);
  if (!expected || matches.length !== 1 || !Number.isSafeInteger(matches[0]?.IdConector) || matches[0].IdConector <= 0) {
    return { available: false, code: 'E2E_RESOURCE_ACTIVITY_UNAVAILABLE' };
  }
  if (matches[0].BalanceoDisponible === false) {
    return { available: false, code: 'E2E_RESOURCE_DESTINATION_UNAVAILABLE' };
  }
  return { available: true, code: 'E2E_RESOURCE_READY' };
}

const DOC32_RESOURCE_CONTRACT = Object.freeze({
  id: 'doc32-workflow-task',
  scope: 'local',
  resources: Object.freeze({
    execution: Object.freeze({ descriptor: (profile) => descriptorFor(profile, 'execution'), preflight, observeGeneration, consumeOnSuccess: true }),
    concurrency: Object.freeze({ descriptor: (profile) => descriptorFor(profile, 'concurrency'), preflight, observeGeneration, consumeOnSuccess: true })
  })
});

module.exports = {
  DOC32_RESOURCE_CONTRACT,
  EMPTY_FINGERPRINT,
  assessPreviewAvailability,
  descriptorFor
};
