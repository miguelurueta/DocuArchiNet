'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const {
  PlatformExecutionError,
  createManagedLifecycle,
  executePlatformRun,
  preflightPlatform
} = require('../scripts/support/workflow-e2e-platform.cjs');
const { createLocalLeaseStore } = require('../scripts/support/e2e-test-resource-lifecycle.cjs');

function profile(overrides = {}) {
  return {
    scenarioId: 'notes-read',
    baseUrl: 'https://workflow.example.invalid/app/',
    module: 'GESTOR',
    environment: 'CERTIFICACION',
    odbcDsn: 'workflowconta',
    taskId: 708,
    budgetMs: 10000,
    ignoreHttpsErrors: false,
    ...overrides
  };
}

function secrets() {
  return {
    'workflow-account': 'cuenta-simulada',
    'workflow-password': 'clave-simulada',
    'readonly-db-user': 'lectura-simulada',
    'readonly-db-password': 'clave-lectura-simulada'
  };
}

function dependencies(overrides = {}) {
  const events = [];
  const evidence = [];
  return {
    events,
    evidence,
    collectSecrets: async () => secrets(),
    createBrowser: async () => ({ close: async () => events.push('browser-close') }),
    createSession: async () => ({ close: async () => events.push('context-close') }),
    createClient: async () => ({ dispose: async () => events.push('client-dispose') }),
    readControl: async ({ control }) => control.id === 'notes-task-state' ? 'a'.repeat(64) : 'b'.repeat(64),
    invoke: async ({ operation, payload }) => {
      if (operation === 'ConsultarNota') return { dto: { Exito: true, Nota: { Version: 1 } }, elapsedMs: 4 };
      if (payload.cursor) return { dto: { Exito: false, Error: { Codigo: 'CURSOR_INVALIDO' } }, elapsedMs: 3 };
      return { dto: { Exito: true, Notas: [{ IdNota: 5 }] }, elapsedMs: 2 };
    },
    assertIntegrity: async () => events.push('integrity'),
    writeEvidence: async (entry) => evidence.push(entry),
    ...overrides
  };
}

test('el preflight bloquea autorizaciones incompletas antes de solicitar secretos o abrir recursos', async () => {
  let prompted = 0;
  let opened = 0;
  await assert.rejects(() => executePlatformRun({
    profile: profile(),
    authorizations: [],
    collectSecrets: async () => { prompted += 1; return secrets(); },
    createBrowser: async () => { opened += 1; return {}; }
  }), (error) => error instanceof PlatformExecutionError && error.code === 'E2E_PLATFORM_AUTHORIZATION_REQUIRED');
  assert.equal(prompted, 0);
  assert.equal(opened, 0);
});

test('el ciclo de lectura captura controles antes y después, sanea secretos y cierra recursos', async () => {
  const temporaryDirectory = await fs.mkdtemp(path.join(os.tmpdir(), 'workflow-e2e-platform-'));
  const providedSecrets = secrets();
  const config = dependencies({ collectSecrets: async () => providedSecrets, temporaryDirectory });
  const outcome = await executePlatformRun({ profile: profile(), authorizations: ['environment'], ...config });
  assert.equal(outcome.success, true);
  assert.deepEqual(outcome.controls, { checked: 2, unchanged: true });
  assert.deepEqual(config.events, ['client-dispose', 'context-close', 'browser-close', 'integrity']);
  assert.deepEqual(providedSecrets, {});
  assert.equal(config.evidence.length, 1);
  assert.doesNotMatch(JSON.stringify(config.evidence[0]), /clave-simulada|cuenta-simulada|IdNota|5/);
  await assert.rejects(() => fs.stat(temporaryDirectory), { code: 'ENOENT' });
});

test('un error de etapa mantiene controles posteriores, cierre y evidencia con código saneado', async () => {
  let controlCalls = 0;
  const config = dependencies({
    readControl: async ({ control }) => {
      controlCalls += 1;
      return control.id === 'notes-task-state' ? 'a'.repeat(64) : 'b'.repeat(64);
    },
    invoke: async ({ operation, payload }) => {
      if (operation === 'ConsultarNota') return { dto: { Exito: true }, elapsedMs: 1 };
      if (payload.cursor) return { dto: { Exito: false, Error: { Codigo: 'CURSOR_INVALIDO' } }, elapsedMs: 1 };
      return { dto: { Exito: false, Error: { Codigo: 'LECTURA_BLOQUEADA' }, Notas: [] }, elapsedMs: 1 };
    }
  });
  await assert.rejects(() => executePlatformRun({ profile: profile(), authorizations: ['environment'], ...config }),
    (error) => error instanceof PlatformExecutionError && error.code === 'NOTES_READ_LIST_BLOCKED');
  assert.equal(controlCalls, 4);
  assert.deepEqual(config.events, ['client-dispose', 'context-close', 'browser-close', 'integrity']);
  assert.equal(config.evidence[0].success, false);
  assert.equal(config.evidence[0].failureCode, 'NOTES_READ_LIST_BLOCKED');
});

test('la plataforma acepta el mínimo anónimo y exige autorización adicional para TLS local', () => {
  const anonymous = preflightPlatform({
    profile: { scenarioId: 'notes-anonymous', baseUrl: 'https://workflow.example.invalid/app/', ignoreHttpsErrors: false },
    authorizations: []
  });
  assert.equal(anonymous.profile.budgetMs, 10000);
  assert.throws(() => preflightPlatform({ profile: profile({ ignoreHttpsErrors: true }), authorizations: ['environment'] }),
    (error) => error instanceof PlatformExecutionError && error.code === 'E2E_PLATFORM_AUTHORIZATION_REQUIRED');
  assert.doesNotThrow(() => preflightPlatform({ profile: profile({ ignoreHttpsErrors: true }), authorizations: ['environment', 'local-tls'] }));
});

test('el kernel reutiliza el ciclo de recursos local para etapas mutantes registradas', async () => {
  const root = await fs.mkdtemp(path.join(os.tmpdir(), 'workflow-e2e-platform-resource-'));
  const contract = {
    id: 'sample-resource',
    scope: 'local',
    resources: {
      execution: {
        descriptor: (selectedProfile) => ({ recordId: selectedProfile.taskId }),
        preflight: async () => ({ available: true, code: 'E2E_RESOURCE_READY', resourceKey: 'sample-resource', generation: 'one' }),
        observeGeneration: async () => 'two',
        consumeOnSuccess: true
      }
    }
  };
  const plan = { profile: { taskId: 1 }, scenario: { resource: { mutating: true, contractId: 'sample-resource', role: 'execution' } } };
  try {
    const lifecycle = createManagedLifecycle({ resourceContracts: { 'sample-resource': contract }, leaseStore: createLocalLeaseStore({ root }) }, plan, {});
    const reservation = await lifecycle.prepare('execution');
    await lifecycle.finalize(reservation, true);
    assert.deepEqual(lifecycle.evidence().map((event) => event.phase), ['preflight', 'reserved', 'consumed']);
  } finally {
    await fs.rm(root, { recursive: true, force: true });
  }
});
