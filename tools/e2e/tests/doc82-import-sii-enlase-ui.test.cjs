'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const { resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');

test('DOC-82 registra recorrido UI ENLASE real, no mutador y con gate temporal', () => {
  const scenario = resolveScenario('import-sii-enlase-ui');
  const profile = validateProfile(JSON.parse(fs.readFileSync('tools/e2e/profiles/doc82-import-sii-enlase-ui.profile.example.json', 'utf8')));
  assert.equal(scenario.doc, 'doc82');
  assert.equal(scenario.stage, 'read');
  assert.equal(scenario.resource.mutating, false);
  assert.deepEqual(scenario.requiredAuthorizations, ['environment', 'gate']);
  assert.ok(scenario.expectations.includes('secure-preview-ui'));
  assert.ok(scenario.expectations.includes('enlase-ui'));
  assert.equal(profile.documentTypeId, 154);
  assert.equal(profile.sampleSize, 2);
  assert.ok(Object.values(scenario.controlExpectations).every((value) => value === 'unchanged'));
});

test('DOC-82 inspecciona disparador, capacidad, tabla, selección múltiple, preview y ausencia de mutación', () => {
  const runner = fs.readFileSync('tools/e2e/scripts/run-workflow-e2e-platform.cjs', 'utf8');
  const adapter = fs.readFileSync('tools/e2e/scripts/adapters/importar-servicio-web-e2e-adapter.cjs', 'utf8');
  assert.match(runner, /#a_adj_service_web/);
  assert.match(runner, /ANEXOS_RADICADO_ENLASE/);
  assert.match(runner, /IMPORT_E2E_ENLASE_UI_BOOTSTRAP_INACTIVE/);
  assert.match(runner, /IMPORT_E2E_ENLASE_UI_BINDING_UNAVAILABLE/);
  assert.match(runner, /const selectionTimeout = Math\.min\(plan\.profile\.budgetMs, 60000\)/);
  assert.match(runner, /E2E_PLATFORM_ENLASE_TASK_NOT_LISTED/);
  assert.match(runner, /IMPORT_E2E_PREPARATION_UI_MULTIPLE_ITEMS_UNAVAILABLE/);
  assert.match(runner, /IMPORT_E2E_PREPARATION_UI_SELECT_ALL_INVALID/);
  assert.match(runner, /IMPORT_E2E_PREVIEW_UI_CONTEXT_NOT_RESTORED/);
  assert.match(runner, /IMPORT_E2E_PREPARATION_UI_MUTATION_OBSERVED/);
  assert.match(adapter, /\['import-sii-enlase-read', 'import-sii-enlase-ui'\]\.includes\(profile\.scenarioId\)/);
});

test('DOC-82 invalida caché de los módulos UI modificados', () => {
  const registration = fs.readFileSync('workflow/Webworkflow.aspx.vb', 'utf8');
  assert.match(registration, /importar-servicio-web-task-context-guard\.js\?v=20260926-doc82ui1/);
  assert.match(registration, /importar-servicio-web-ui\.js\?v=20260926-doc82ui2/);
});

test('DOC-82 conserva la ejecución múltiple DOC-81 como prueba mutadora separada', () => {
  const execution = resolveScenario('import-sii-enlase-execution');
  const source = fs.readFileSync('tools/e2e/tests/doc81-import-sii-enlase-execution.test.cjs', 'utf8');
  assert.equal(execution.resource.mutating, true);
  assert.ok(execution.expectations.includes('single-intent'));
  assert.match(source, /conserva una intención y una ejecución para tres anexos/);
  assert.match(source, /creates\[0\]\.payload\.Items\.length,3/);
});
