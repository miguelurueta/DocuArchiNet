'use strict';

const { queryFingerprint } = require('./doc32-e2e-odbc.cjs');

const EMPTY_FINGERPRINT = 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855';

function descriptorFor(profile, role) {
  if (role === 'execution') return { taskId: profile.executionTaskId, activity: profile.executionActivityName };
  if (role === 'concurrency') return { taskId: profile.concurrencyTaskId, activity: profile.concurrencyActivityName };
  throw new Error('role-invalid');
}

async function observeGeneration({ profile, environment, descriptor }) {
  return queryFingerprint(profile.taskStateSql, descriptor.taskId, environment, 'DOC36_E2E');
}

async function preflight(context) {
  const generation = await observeGeneration(context);
  if (generation === EMPTY_FINGERPRINT) return { available: false, code: 'E2E_RESOURCE_TASK_UNAVAILABLE' };
  return {
    available: true,
    code: 'E2E_RESOURCE_READY',
    resourceKey: `workflow-task:${context.descriptor.taskId}`,
    generation
  };
}

const DOC36_RESOURCE_CONTRACT = Object.freeze({
  id: 'doc36-workflow-user-previous-task',
  scope: 'local',
  resources: Object.freeze({
    execution: Object.freeze({ descriptor: (profile) => descriptorFor(profile, 'execution'), preflight, observeGeneration, consumeOnSuccess: true }),
    concurrency: Object.freeze({ descriptor: (profile) => descriptorFor(profile, 'concurrency'), preflight, observeGeneration, consumeOnSuccess: true })
  })
});

module.exports = {
  DOC36_RESOURCE_CONTRACT,
  EMPTY_FINGERPRINT,
  descriptorFor
};
