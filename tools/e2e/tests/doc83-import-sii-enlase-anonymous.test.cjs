'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const { IMPORTAR_SERVICIO_WEB_E2E_ADAPTER } = require('../scripts/adapters/importar-servicio-web-e2e-adapter.cjs');
const { resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');

test('DOC-83 registra acceso negativo ENLASE oficial sin sesión, tarea, controles ni secretos', () => {
  const scenario = resolveScenario('import-sii-enlase-anonymous');
  assert.equal(scenario.stage, 'anonymous');
  assert.equal(scenario.transport.session, 'none');
  assert.deepEqual(scenario.requiredAuthorizations, ['environment', 'gate']);
  assert.ok(scenario.expectations.includes('temporary-feature-gate'));
  const runner = fs.readFileSync(path.resolve('tools/e2e/scripts/run-workflow-e2e-platform.cjs'), 'utf8');
  assert.match(runner, /attempt < 6/);
  assert.match(runner, /consecutiveSuccesses >= 2/);
  assert.match(runner, /E2E_PLATFORM_RELOAD_STABILIZATION_FAILED/);
  assert.match(runner, /configureTemporaryScenario/);
  assert.deepEqual(scenario.requiredSecrets, []);
  assert.equal(scenario.resource, null);
  assert.deepEqual(scenario.controls, []);
});

test('DOC-83 valida un perfil anónimo sin aceptar contexto de tarea', () => {
  const file = path.resolve('tools/e2e/profiles/doc83-import-sii-enlase-anonymous.profile.example.json');
  const profile = validateProfile(JSON.parse(fs.readFileSync(file, 'utf8')));
  assert.equal(profile.scenarioId, 'import-sii-enlase-anonymous');
  assert.equal(profile.taskId, undefined);
  assert.throws(() => validateProfile({ ...profile, taskId: 1 }), { code: 'E2E_PLATFORM_PROFILE_STAGE_FIELD_INVALID' });
});

test('DOC-83 exige bloqueo funcional y cero elementos al consultar ENLASE sin sesión', async () => {
  let observed;
  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeAnonymous({
    budgetMs: 10000,
    invoke: async (operation, payload) => {
      observed = { operation, payload };
      return { elapsedMs: 4, dto: { Items: [], Error: { Codigo: 'SESSION_REQUIRED' } } };
    }
  });
  assert.equal(observed.operation, 'QueryItems');
  assert.equal(observed.payload.request.TaskId, 1);
  assert.equal(observed.payload.request.Capability, 'ANEXOS_RADICADO_ENLASE');
  assert.deepEqual(result.codes, { query: 'SESSION_REQUIRED' });
  assert.equal(result.count, 0);
});

test('DOC-83 rechaza respuesta anónima que exponga elementos', async () => {
  await assert.rejects(() => IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeAnonymous({
    budgetMs: 10000,
    invoke: async () => ({ elapsedMs: 4, dto: { Items: [{ ExternalKey: 'unexpected' }] } })
  }), { code: 'IMPORT_E2E_ENLASE_ANONYMOUS_NOT_BLOCKED' });
});
test('DOC-83 rechaza el código retirado FEATURE_DISABLED', async () => {
  await assert.rejects(() => IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeAnonymous({
    budgetMs: 10000,
    invoke: async () => ({ elapsedMs: 3, dto: { Items: [], Error: { Codigo: 'FEATURE_DISABLED' } } })
  }), { code: 'IMPORT_E2E_ENLASE_ANONYMOUS_NOT_BLOCKED' });
});
