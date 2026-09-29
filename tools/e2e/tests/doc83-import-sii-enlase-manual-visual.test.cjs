'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const { resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');
const { preflightPlatform, requiredAuthorizationsFor } = require('../scripts/support/workflow-e2e-platform.cjs');

const root = path.resolve(__dirname, '..', '..', '..');
const load = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');
const profile = JSON.parse(load('tools', 'e2e', 'profiles', 'doc83-import-sii-enlase-manual-visual.profile.example.json'));

test('DOC-83 registra aceptación visual mutadora con autorizaciones y controles completos', () => {
  const scenario = resolveScenario('import-sii-enlase-manual-visual');
  const validated = validateProfile(profile);
  assert.equal(scenario.stage, 'execution');
  assert.equal(scenario.resource.mutating, true);
  assert.equal(scenario.resource.role, 'manual-visual');
  assert.equal(scenario.controls.length, 7);
  assert.ok(scenario.expectations.includes('manual-visual-execution'));
  assert.deepEqual(requiredAuthorizationsFor(scenario, validated), [
    'environment', 'gate', 'execution', 'discardable-resource'
  ]);
  assert.doesNotThrow(() => preflightPlatform({
    profile: validated,
    authorizations: ['environment', 'gate', 'execution', 'discardable-resource']
  }));
});

test('DOC-83 limita la aceptación visual, abre navegador visible y restaura el gate', () => {
  const runner = load('tools', 'e2e', 'scripts', 'run-workflow-e2e-platform.cjs');
  const platform = load('tools', 'e2e', 'scripts', 'support', 'workflow-e2e-platform.cjs');
  const consoleSupport = load('tools', 'e2e', 'scripts', 'support', 'interactive-e2e-console.cjs');
  assert.match(runner, /inspectEnlaseManualVisual/);
  assert.match(runner, /Math\.min\(plan\.profile\.budgetMs, 600000\)/);
  assert.match(runner, /promptWithTimeout/);
  assert.match(runner, /IMPORT_E2E_MANUAL_VISUAL_TIMEOUT/);
  assert.match(runner, /window\.__doc83ManualVisualMarker/);
  assert.match(runner, /window\.__doc83ManualVisualProjectionCount = 0/);
  assert.match(runner, /window\.__doc83ManualVisualProjectionRows = \[\]/);
  assert.match(runner, /window\.__doc83ManualVisualPostbackCount = 0/);
  assert.match(runner, /window\.__doc83ManualVisualPostbackKinds = \[\]/);
  assert.match(runner, /DOCUMENT_REFRESH/);
  assert.match(runner, /DOCUMENT_INTERACTION/);
  assert.match(runner, /DOC83_MANUAL_VISUAL_POSTBACK_DIAGNOSTIC/);
  assert.match(runner, /originalInsert\.apply\(this, arguments\)/);
  assert.doesNotMatch(runner, /initialRows\.forEach\(\(row\) => row\.remove\(\)\)/);
  assert.match(runner, /IMPORT_E2E_MANUAL_VISUAL_ITEMS_UNAVAILABLE/);
  assert.match(runner, /IMPORT_E2E_MANUAL_VISUAL_POSTBACK_OBSERVED/);
  assert.match(runner, /data-import-modern-bound/);
  assert.match(runner, /IMPORT_E2E_MANUAL_VISUAL_PROJECTION_NOT_OBSERVED/);
  assert.match(runner, /#GridView_list_documento_relacion tr\[id_rad\]/);
  assert.match(runner, /parts\[1\] !== id/);
  assert.match(runner, /Number\(parts\[5\]\) !== Number\(taskId\)/);
  assert.match(runner, /headless: !plan\.scenario\.expectations\.some/);
  assert.match(runner, /\['manual-visual-execution', 'manual-layout-review'\]\.includes\(expectation\)/);
  assert.match(runner, /finally\s*\{\s*await restoreGate\(\)/);
  assert.match(platform, /expectations\.includes\('manual-visual-execution'\)/);
  assert.match(consoleSupport, /function promptWithTimeout/);
  assert.match(consoleSupport, /clearTimeout\(timer\)/);
  assert.match(consoleSupport, /process\.stdin\.off\('data', onData\)/);
});

test('DOC-83 no ejecuta una segunda importación API antes de la interacción visual', () => {
  const adapter = load('tools', 'e2e', 'scripts', 'adapters', 'importar-servicio-web-e2e-adapter.cjs');
  const start = adapter.indexOf("profile.scenarioId === 'import-sii-enlase-manual-visual'");
  const end = adapter.indexOf("profile.scenarioId === 'import-sii-enlase-execution'", start);
  assert.ok(start > 0 && end > start);
  const branch = adapter.slice(start, end);
  assert.match(branch, /manualVisual: 'PENDING'/);
  assert.doesNotMatch(branch, /invoke\(/);
});
