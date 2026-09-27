'use strict';

const assert = require('node:assert/strict');
const test = require('node:test');
const { IMPORTAR_SERVICIO_WEB_E2E_ADAPTER } = require('../scripts/adapters/importar-servicio-web-e2e-adapter.cjs');
const { requiredAuthorizationsFor } = require('../scripts/support/workflow-e2e-platform.cjs');
const { CONTROL_REGISTRY, resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');

function profile() {
  return {
    scenarioId: 'import-sii-enlase-assignment',
    baseUrl: 'http://localhost/GestionDocumental-Docuarchi.net/',
    module: 'WORKFLOW REGISTRO',
    environment: 'pruebas-autorizadas-local',
    odbcDsn: 'workflowdocument',
    taskId: 220588,
    budgetMs: 60000,
    ignoreHttpsErrors: false
  };
}

test('DOC-83 registra asignación como etapa mutadora separada y controlada', () => {
  const scenario = resolveScenario('import-sii-enlase-assignment');
  assert.equal(scenario.stage, 'assignment');
  assert.equal(scenario.resource.role, 'assignment');
  assert.equal(scenario.resource.mutating, true);
  assert.deepEqual(requiredAuthorizationsFor(scenario, profile()), ['environment', 'gate', 'execution', 'discardable-resource']);
  assert.deepEqual(scenario.controls, ['workflow-assignment-state']);
  assert.equal(scenario.controlExpectations['workflow-assignment-state'], 'assignment-mode');
  assert.match(CONTROL_REGISTRY['workflow-assignment-state'].query, /^SELECT\b/i);
  assert.doesNotMatch(CONTROL_REGISTRY['workflow-assignment-state'].query, /\b(?:INSERT|UPDATE|DELETE|CALL|EXEC)\b/i);
});

test('DOC-83 valida un perfil de asignación sin aceptar datos de importación', () => {
  const validated = validateProfile(profile());
  assert.equal(validated.taskId, 220588);
  assert.equal(validated.radicado, undefined);
  assert.throws(() => validateProfile({ ...profile(), radicado: 'S002469804' }), { code: 'E2E_PLATFORM_PROFILE_STAGE_FIELD_INVALID' });
});

test('DOC-83 no invoca el servicio SII durante la acción explícita de asignación', async () => {
  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeAssignment();
  assert.deepEqual(result.codes, { assignmentAction: 'EXPLICIT' });
  assert.equal(result.count, 0);
});

test('DOC-83 enruta la inspección al postback autoritativo de WebForms', async () => {
  const source = await require('node:fs/promises').readFile(require('node:path').resolve(__dirname, '../scripts/run-workflow-e2e-platform.cjs'), 'utf8');
  assert.match(source, /inspectEnlaseAssignmentUi[\s\S]*#enlase-assign-action:visible/);
  assert.match(source, /request\(\)\.method\(\) === 'POST'[\s\S]*Webworkflow/);
  assert.match(source, /assignmentResult = panelVisible \? 'BLOCKED' : 'ASSIGNED'/);
  assert.match(source, /assignmentResult === 'BLOCKED' && !dialogObserved/);
});
