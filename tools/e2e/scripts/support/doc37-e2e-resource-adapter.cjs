'use strict';

const { queryFingerprint } = require('./doc32-e2e-odbc.cjs');
const { EMPTY_FINGERPRINT } = require('./doc32-e2e-resource-adapter.cjs');

function descriptorFor(profile, role) {
  if (role === 'execution') return { taskId: profile.uiExecutionTaskId };
  if (role === 'ui-lock') return { taskId: profile.uiLockTaskId };
  throw new Error('role-invalid');
}

async function observeGeneration({ profile, environment, descriptor }) {
  return queryFingerprint(profile.taskStateSql, descriptor.taskId, environment, 'DOC37_E2E');
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

const DOC37_RESOURCE_CONTRACT = Object.freeze({
  id: 'doc37-workflow-user-previous-ui-task',
  scope: 'local',
  resources: Object.freeze({
    execution: Object.freeze({ descriptor: (profile) => descriptorFor(profile, 'execution'), preflight, observeGeneration, consumeOnSuccess: true }),
    'ui-lock': Object.freeze({ descriptor: (profile) => descriptorFor(profile, 'ui-lock'), preflight, observeGeneration, consumeOnSuccess: true })
  })
});

module.exports = {
  DOC37_RESOURCE_CONTRACT,
  descriptorFor
};
