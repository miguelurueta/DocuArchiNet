'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const {
  CONTROL_REGISTRY,
  STAGES,
  resolveAdapter,
  resolveControls,
  resolveScenario,
  validateRegistry
} = require('../scripts/support/workflow-e2e-platform-registry.cjs');

test('el registro de plataforma solo resuelve escenarios, controles y adaptadores versionados', () => {
  assert.doesNotThrow(() => validateRegistry());
  const scenario = resolveScenario('notes-read');
  assert.equal(scenario.stage, 'read');
  assert.equal(resolveAdapter(scenario.adapterId).id, 'notes-read');
  assert.deepEqual(resolveControls(scenario.controls).map((control) => control.id), ['notes-task-state', 'notes-audit']);
  assert.throws(() => resolveScenario('script-arbitrario'), { code: 'E2E_PLATFORM_SCENARIO_UNREGISTERED' });
  assert.throws(() => resolveControls(['sql-libre']), { code: 'E2E_PLATFORM_CONTROL_UNREGISTERED' });
  assert.deepEqual(STAGES, ['anonymous', 'read', 'preview', 'execution', 'concurrency', 'ui-lock']);
});

test('los controles de Notas son SELECT registrados y no incluyen contenido de notas', () => {
  for (const control of Object.values(CONTROL_REGISTRY)) {
    assert.match(control.query, /^SELECT\b/i);
    assert.equal((control.query.match(/\?/g) || []).length, 1);
    assert.doesNotMatch(control.query, /DATO_ANOTACION|datos_operacion/i);
  }
});

test('el adaptador de Notas no incorpora infraestructura transversal', () => {
  const source = fs.readFileSync(path.join(__dirname, '..', 'scripts', 'adapters', 'notes-read-e2e-adapter.cjs'), 'utf8');
  assert.doesNotMatch(source, /require\(|createAuthenticatedWorkflowSession|promptSecret|queryFingerprint|ignoreHTTPSErrors|writeFile/i);
});
