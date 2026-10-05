'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { resolveScenario, resolveControls } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');
const {
  PlatformExecutionError,
  createRegisteredResourceContract,
  createRuntimeEnvironment,
  eraseSecrets,
  executePlatformRun,
  normalizeDocumentUrl,
  preflightPlatform
} = require('../scripts/support/workflow-e2e-platform.cjs');

const e2eRoot = path.resolve(__dirname, '..');
const example = () => JSON.parse(fs.readFileSync(path.join(e2eRoot, 'profiles', 'doc87-registro-tarea-ruta-sii.profile.example.json'), 'utf8'));

test('DOC-87 reutiliza la plataforma con recibo descartable, sesión y controles registrados', () => {
  const scenario = resolveScenario('registro-ruta-sii-execution');
  assert.equal(scenario.doc, 'doc87');
  assert.equal(scenario.resource.profileField, 'receipt');
  assert.equal(scenario.resource.contractId, 'workflow-route-receipt-controls');
  assert.deepEqual(scenario.requiredSecrets, ['workflow-account', 'workflow-password', 'readonly-db-user', 'readonly-db-password']);
  assert.equal(resolveControls(scenario.controls).length, 6);
  assert.equal(scenario.controlExpectations['registro-ruta-sii-outbox-unico'], 'unchanged');
  assert.equal(scenario.controlExpectations['registro-ruta-sii-relacion-unica'], 'unchanged');
  const profile = validateProfile(example());
  assert.equal(profile.module, 'WORKFLOW REGISTRO');
  assert.equal(profile.receipt, 'S000000001');
  assert.equal(profile.activityId, 1);
});

test('DOC-87 rechaza recibo, actividad o secretos persistidos en el perfil', () => {
  assert.throws(() => validateProfile({ ...example(), receipt: 'S-1' }), /RECEIPT_INVALID/);
  assert.throws(() => validateProfile({ ...example(), activityId: 0 }), /ACTIVITY_INVALID/);
  assert.throws(() => validateProfile({ ...example(), password: 'prohibido' }), /FORBIDDEN_FIELD/);
});

test('DOC-87 ejecuta la UI desde el runner común y no desde un login paralelo', () => {
  const runner = fs.readFileSync(path.join(e2eRoot, 'scripts', 'run-workflow-e2e-platform.cjs'), 'utf8');
  const platform = fs.readFileSync(path.join(e2eRoot, 'scripts', 'support', 'workflow-e2e-platform.cjs'), 'utf8');
  assert.match(runner, /createAuthenticatedWorkflowSession/);
  assert.match(runner, /inspectRegistroRutaSiiUi/);
  assert.match(runner, /#util_sii_registro_tarea_ruta a/);
  assert.match(runner, /REGISTRO_RUTA_SII_E2E_ACCOUNT_FORBIDDEN/);
  assert.match(runner, /REGISTRO_RUTA_SII_E2E_PERMISSION_QUERY_FAILED/);
  assert.match(runner, /REGISTRO_RUTA_SII_E2E_PERMISSION_HTTP_FAILED/);
  assert.match(runner, /Service_Solicita_permisos_usuario_workflow_intgracion_sii/);
  assert.match(runner, /normalizeDocumentUrl\(frame\.url\(\)\) !== documentUrl/);
  assert.match(runner, /routePanel\.waitFor\(\{ state: 'visible'/);
  assert.match(runner, /REGISTRO_RUTA_SII_E2E_STALE_CONTEXT_MUTATED/);
  assert.match(runner, /DOC87_DA_E2E/);
  assert.match(platform, /observedControlMutation/);
  assert.match(platform, /lifecycle\.finalize\(reservation, !failure \|\| observedControlMutation\)/);
});

test('DOC-87 ignora navegación por fragmento y detecta cambio real de documento', () => {
  const current = normalizeDocumentUrl('https://workflow.example.invalid/app/workflow/form.aspx#registro_ruta');
  assert.equal(current, normalizeDocumentUrl('https://workflow.example.invalid/app/workflow/form.aspx#'));
  assert.equal(current, normalizeDocumentUrl('https://workflow.example.invalid/app/workflow/form.aspx#otra-pestana'));
  assert.notEqual(current, normalizeDocumentUrl('https://workflow.example.invalid/app/workflow/otra.aspx#registro_ruta'));
});

test('DOC-87 consume la reserva si el control confirma mutación antes de un fallo UI', async () => {
  const selectedProfile = validateProfile(example());
  const finalized = [];
  const evidence = [];
  let controlReads = 0;
  const suppliedSecrets = {
    'workflow-account': 'cuenta-prueba', 'workflow-password': 'secreto-efimero',
    'readonly-db-user': 'lector', 'readonly-db-password': 'secreto-lector'
  };
  await assert.rejects(() => executePlatformRun({
    profile: selectedProfile,
    authorizations: ['environment', 'execution', 'discardable-resource'],
    collectSecrets: async () => suppliedSecrets,
    createBrowser: async () => ({ close: async () => {} }),
    createSession: async () => ({ close: async () => {} }),
    createClient: async () => ({ dispose: async () => {} }),
    invoke: async () => ({}),
    inspectSession: async () => { throw new PlatformExecutionError('REGISTRO_RUTA_SII_E2E_UI_CONTRACT_INVALID'); },
    readControl: async () => `${controlReads++ < 6 ? 'a' : 'b'}`.repeat(64),
    resourceLifecycleFactory: () => ({
      prepare: async () => Object.freeze({ role: 'execution' }),
      finalize: async (_reservation, consumed) => finalized.push(consumed),
      evidence: () => []
    }),
    assertIntegrity: async () => {},
    writeEvidence: async (entry) => evidence.push(entry)
  }), (error) => error instanceof PlatformExecutionError && error.code === 'REGISTRO_RUTA_SII_E2E_UI_CONTRACT_INVALID');
  assert.deepEqual(finalized, [true]);
  assert.equal(evidence[0].controls.unchanged, false);
});

test('DOC-87 reserva el recibo por huella y borra los alias de secretos', async () => {
  const profile = validateProfile(example());
  const plan = preflightPlatform({ profile, authorizations: ['environment', 'execution', 'discardable-resource'] });
  const secrets = {
    'workflow-account': 'cuenta-prueba', 'workflow-password': 'secreto-efimero',
    'readonly-db-user': 'lector', 'readonly-db-password': 'secreto-lector'
  };
  const environment = createRuntimeEnvironment(plan, secrets);
  const emptyFingerprint = 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855';
  const contract = createRegisteredResourceContract(plan, environment, async ({ control }) =>
    ['registro-ruta-sii-workflow', 'registro-ruta-sii-registro-publico', 'registro-ruta-sii-relacion'].includes(control.id)
      ? emptyFingerprint
      : 'a'.repeat(64));
  const descriptor = contract.resources.execution.descriptor(plan.profile);
  const preflight = await contract.resources.execution.preflight({ descriptor });
  assert.equal(preflight.available, true);
  assert.doesNotMatch(preflight.resourceKey, /S000000001/);
  assert.match(preflight.resourceKey, /^workflow-route-receipt:[a-f0-9]{64}$/);
  eraseSecrets(secrets, environment);
  assert.equal(environment.DOC87_E2E_MYSQL_PASSWORD, undefined);
  assert.equal(environment.DOC87_DA_E2E_MYSQL_PASSWORD, undefined);
});

test('DOC-87 bloquea antes de la UI un recibo que ya existe en cualquier almacén', async () => {
  const profile = validateProfile(example());
  const plan = preflightPlatform({ profile, authorizations: ['environment', 'execution', 'discardable-resource'] });
  const emptyFingerprint = 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855';
  for (const occupiedControl of ['registro-ruta-sii-workflow', 'registro-ruta-sii-registro-publico', 'registro-ruta-sii-relacion']) {
    const contract = createRegisteredResourceContract(plan, {}, async ({ control }) =>
      control.id === occupiedControl ? 'b'.repeat(64) :
        ['registro-ruta-sii-workflow', 'registro-ruta-sii-registro-publico', 'registro-ruta-sii-relacion'].includes(control.id)
          ? emptyFingerprint
          : 'a'.repeat(64));
    const descriptor = contract.resources.execution.descriptor(plan.profile);
    const preflight = await contract.resources.execution.preflight({ descriptor });
    assert.equal(preflight.available, false, occupiedControl);
    assert.equal(preflight.code, 'E2E_RESOURCE_ALREADY_EXISTS', occupiedControl);
  }
});
