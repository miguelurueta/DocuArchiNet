'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const { resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');

test('DOC-80 registra lectura ENLASE no mutadora con controles invariantes', () => {
  const scenario = resolveScenario('import-sii-enlase-read');
  assert.equal(scenario.doc, 'doc80');
  assert.equal(scenario.stage, 'read');
  assert.equal(scenario.resource.mutating, false);
  assert.deepEqual(scenario.requiredAuthorizations, ['environment', 'gate']);
  assert.ok(Object.values(scenario.controlExpectations).every((value) => value === 'unchanged'));
});

test('DOC-80 valida perfil saneado y el adaptador envía capacidad explícita', () => {
  const profile = validateProfile(JSON.parse(fs.readFileSync('tools/e2e/profiles/doc80-import-sii-enlase-read.profile.example.json', 'utf8')));
  assert.equal(profile.scenarioId, 'import-sii-enlase-read');
  const adapter = fs.readFileSync('tools/e2e/scripts/adapters/importar-servicio-web-e2e-adapter.cjs', 'utf8');
  assert.match(adapter, /Capability: 'ANEXOS_RADICADO_ENLASE'/);
  assert.match(adapter, /IMPORT_E2E_ENLASE_PREVIEW_FAILED/);
});

test('DOC-80 selecciona contexto ENLASE oficial sin exigir tarea estándar', () => {
  const runner = fs.readFileSync('tools/e2e/scripts/run-workflow-e2e-platform.cjs', 'utf8');
  const selector = runner.match(/async function selectWorkflowTask[\s\S]*?async function initializeWorkflowContext/)[0];
  assert.match(selector, /plan\.scenario\.id === 'import-sii-enlase-read'/);
  assert.match(selector, /#HiddenIdFlujo/);
  assert.match(selector, /parts\[0\] === expectedTaskId/);
  assert.match(selector, /parts\[3\]\.toUpperCase\(\) === 'ENLASE'/);
  assert.match(selector, /E2E_PLATFORM_ENLASE_CONTEXT_REJECTED/);
  const branchStart = selector.indexOf("if (plan.scenario.id === 'import-sii-enlase-read')");
  const branchEnd = selector.indexOf("const selectedTask = page.locator('#Hidden_id_tarea_selecionada')");
  assert.ok(branchStart >= 0 && branchEnd > branchStart);
  assert.doesNotMatch(selector.slice(branchStart, branchEnd), /Hidden_id_tarea_selecionada/);});