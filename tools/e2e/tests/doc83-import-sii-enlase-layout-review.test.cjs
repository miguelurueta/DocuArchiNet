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
const profile = JSON.parse(load('tools', 'e2e', 'profiles', 'doc83-import-sii-enlase-layout-review.profile.example.json'));

test('DOC-83 registra la revisión visual de layout como lectura invariante', () => {
  const scenario = resolveScenario('import-sii-enlase-layout-review');
  const validated = validateProfile(profile);
  assert.equal(scenario.stage, 'read');
  assert.equal(scenario.resource.mutating, false);
  assert.equal(scenario.resource.role, 'read');
  assert.equal(scenario.controls.length, 7);
  assert.ok(scenario.expectations.includes('manual-layout-review'));
  assert.deepEqual(requiredAuthorizationsFor(scenario, validated), ['environment', 'gate']);
  assert.doesNotThrow(() => preflightPlatform({ profile: validated, authorizations: ['environment', 'gate'] }));
});

test('DOC-83 revisa contención, scroll y ausencia de mutaciones en navegador visible', () => {
  const runner = load('tools', 'e2e', 'scripts', 'run-workflow-e2e-platform.cjs');
  const platform = load('tools', 'e2e', 'scripts', 'support', 'workflow-e2e-platform.cjs');
  const adapter = load('tools', 'e2e', 'scripts', 'adapters', 'importar-servicio-web-e2e-adapter.cjs');
  assert.match(runner, /inspectEnlaseLayoutReview/);
  assert.match(runner, /manual-layout-review/);
  assert.match(runner, /importar-servicio-web-sii__data-cell/);
  assert.match(runner, /style\.textOverflow !== 'ellipsis'/);
  assert.match(runner, /style\.position !== 'sticky'/);
  assert.match(runner, /IMPORT_E2E_LAYOUT_REVIEW_MUTATION_OBSERVED/);
  assert.match(runner, /no hay textos superpuestos y que el scroll permanece dentro de la tabla/);
  assert.match(runner, /DOC83_PREPARATION_REVIEW_READY/);
  assert.match(runner, /PREPARATION_ACTIONS_INVALID/);
  assert.match(runner, /no aparece el bloque Plan previsto/);
  assert.match(runner, /DOC83_PREVIEW_REVIEW_READY/);
  assert.match(runner, /PREVIEW_ALIGNMENT_INVALID/);
  assert.match(runner, /puede volver a documentos y que el preview no presenta desajuste vertical/);
  assert.match(runner, /if \(await preview\.isVisible\(\)\)/);
  assert.match(runner, /#importar-servicio-web-preview-back:visible/);
  assert.match(runner, /IMPORT_E2E_LAYOUT_REVIEW_LIST_NOT_RESTORED/);
  assert.match(adapter, /'import-sii-enlase-layout-review'/);
  assert.match(platform, /expectations\.includes\('manual-layout-review'\)/);
});
