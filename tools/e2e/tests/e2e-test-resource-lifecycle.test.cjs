'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const {
  RESOURCE_CODES,
  ResourceLifecycleError,
  assertNonSensitiveDescriptor,
  createLocalLeaseStore,
  createResourceLifecycle,
  validateRegisteredResourceContracts,
  validateResourceContract,
  writeResourceLifecycleEvidence
} = require('../scripts/support/e2e-test-resource-lifecycle.cjs');
const {
  DOC32_RESOURCE_CONTRACT,
  assessPreviewAvailability,
  descriptorFor
} = require('../scripts/support/doc32-e2e-resource-adapter.cjs');
const { executeSequence } = require('../scripts/support/workflow-e2e-orchestrator.cjs');

function lifecycleError(code) {
  return (error) => error instanceof ResourceLifecycleError && error.code === code;
}

function contract(control, scope = 'local') {
  return {
    id: 'sample-resource',
    scope,
    resources: {
      mutate: {
        descriptor: (profile) => ({ recordId: profile.recordId }),
        preflight: async () => control.available
          ? { available: true, code: 'E2E_RESOURCE_READY', resourceKey: `record:${control.key}`, generation: control.generation }
          : { available: false, code: control.code || RESOURCE_CODES.UNAVAILABLE },
        observeGeneration: async () => control.generation,
        consumeOnSuccess: true
      }
    }
  };
}

async function temporaryStore() {
  const root = await fs.mkdtemp(path.join(os.tmpdir(), 'e2e-resource-lifecycle-'));
  return { root, store: createLocalLeaseStore({ root }) };
}

test('el contrato de recursos rechaza descriptores sensibles, SQL y proveedores no registrados', () => {
  const control = { available: true, key: '42', generation: 'uno' };
  const valid = contract(control);
  assert.equal(validateResourceContract(valid), valid);
  assert.equal(validateRegisteredResourceContracts({ 'sample-resource': valid })['sample-resource'], valid);
  assertNonSensitiveDescriptor({ recordId: 42, alias: 'descartable' });
  assert.throws(() => assertNonSensitiveDescriptor({ password: 'prohibido' }), lifecycleError(RESOURCE_CODES.DESCRIPTOR_INVALID));
  assert.throws(() => assertNonSensitiveDescriptor({ query: 'SELECT * FROM tarea' }), lifecycleError(RESOURCE_CODES.DESCRIPTOR_INVALID));
  assert.throws(() => assertNonSensitiveDescriptor({ provider: 'externo' }), lifecycleError(RESOURCE_CODES.DESCRIPTOR_INVALID));
  assert.throws(() => validateRegisteredResourceContracts({ different: valid }), lifecycleError(RESOURCE_CODES.CONTRACT_INVALID));
});

test('una reserva local es exclusiva y el marcador no revela el recurso de negocio', async () => {
  const { root, store } = await temporaryStore();
  try {
    const request = { contractId: 'sample-resource', scope: 'local', role: 'mutate', resourceKey: 'record:secreto-no-publicable', generation: 'uno' };
    const lease = await store.acquire(request);
    await assert.rejects(() => store.acquire(request), lifecycleError(RESOURCE_CODES.RESERVED));
    const [entry] = await fs.readdir(root);
    const content = await fs.readFile(path.join(root, entry), 'utf8');
    assert.doesNotMatch(entry, /secreto-no-publicable/);
    assert.doesNotMatch(content, /secreto-no-publicable/);
    await store.release(lease);
  } finally {
    await fs.rm(root, { recursive: true, force: true });
  }
});

test('un recurso consumido exige una nueva generación antes de reutilizarse', async () => {
  const { root, store } = await temporaryStore();
  const control = { available: true, key: '42', generation: 'uno' };
  try {
    const lifecycle = createResourceLifecycle({ contract: contract(control), profile: { recordId: 42 }, environment: {}, leaseStore: store });
    const first = await lifecycle.prepare('mutate');
    await lifecycle.finalize(first, true);
    await assert.rejects(() => lifecycle.prepare('mutate'), lifecycleError(RESOURCE_CODES.CONSUMED));
    control.generation = 'dos';
    const preparedAgain = await lifecycle.prepare('mutate');
    await lifecycle.finalize(preparedAgain, false);
    assert.deepEqual(lifecycle.evidence().map((event) => event.phase), ['preflight', 'reserved', 'consumed', 'preflight', 'preflight', 'reserved', 'released']);
  } finally {
    await fs.rm(root, { recursive: true, force: true });
  }
});

test('el preflight rechazado impide iniciar la etapa mutante y conserva evidencia saneada', async () => {
  const { root, store } = await temporaryStore();
  const control = { available: false, code: 'E2E_RESOURCE_DESTINATION_UNAVAILABLE', key: '42', generation: 'uno' };
  const definition = {
    resourceContract: contract(control),
    environment: () => ({})
  };
  const written = [];
  let childStarted = false;
  try {
    await assert.rejects(() => executeSequence({
      definition,
      profile: { doc: 'sample', recordId: 42 },
      authorizations: new Set(),
      stages: [{ id: 'mutate', resourceRole: 'mutate' }],
      assertIntegrity: async () => {},
      collectSecrets: async () => ({ SAMPLE_PASSWORD: 'valor-sensible-simulado' }),
      resourceLifecycleFactory: ({ environment }) => createResourceLifecycle({ contract: definition.resourceContract, profile: { recordId: 42 }, environment, leaseStore: store }),
      evidenceWriter: async (evidence) => written.push(evidence),
      stageRunner: async () => {
        childStarted = true;
        return { code: 0 };
      }
    }), lifecycleError('E2E_RESOURCE_DESTINATION_UNAVAILABLE'));
    assert.equal(childStarted, false);
    assert.deepEqual(written[0].events, [{ role: 'mutate', phase: 'preflight', code: 'E2E_RESOURCE_DESTINATION_UNAVAILABLE' }]);
    assert.doesNotMatch(JSON.stringify(written[0]), /valor-sensible-simulado|recordId|42/);
  } finally {
    await fs.rm(root, { recursive: true, force: true });
  }
});

test('un fallo de etapa libera su recurso y conserva el cierre de secretos', async () => {
  const { root, store } = await temporaryStore();
  const control = { available: true, key: '42', generation: 'uno' };
  const definition = { resourceContract: contract(control), environment: () => ({}) };
  let childEnvironment;
  try {
    await assert.rejects(() => executeSequence({
      definition,
      profile: { doc: 'sample', recordId: 42 },
      authorizations: new Set(),
      stages: [{ id: 'mutate', resourceRole: 'mutate' }],
      assertIntegrity: async () => {},
      collectSecrets: async () => ({ SAMPLE_PASSWORD: 'valor-sensible-simulado' }),
      resourceLifecycleFactory: ({ environment }) => createResourceLifecycle({ contract: definition.resourceContract, profile: { recordId: 42 }, environment, leaseStore: store }),
      evidenceWriter: async () => {},
      stageRunner: async (_stage, environment) => {
        childEnvironment = environment;
        return { code: 1 };
      }
    }), /mutate no se completó/);
    assert.equal(Object.hasOwn(childEnvironment, 'SAMPLE_PASSWORD'), false);
    const lifecycle = createResourceLifecycle({ contract: definition.resourceContract, profile: { recordId: 42 }, environment: {}, leaseStore: store });
    const reservation = await lifecycle.prepare('mutate');
    await lifecycle.finalize(reservation, false);
  } finally {
    await fs.rm(root, { recursive: true, force: true });
  }
});

test('un contrato compartido falla cerrado sin coordinador compartido registrado', async () => {
  const { root, store } = await temporaryStore();
  try {
    const lifecycle = createResourceLifecycle({
      contract: contract({ available: true, key: '42', generation: 'uno' }, 'shared'),
      profile: { recordId: 42 },
      environment: {},
      leaseStore: store
    });
    await assert.rejects(() => lifecycle.prepare('mutate'), lifecycleError(RESOURCE_CODES.SHARED_COORDINATOR_REQUIRED));
  } finally {
    await fs.rm(root, { recursive: true, force: true });
  }
});

test('el adaptador DOC-32 conserva recursos por rol y clasifica el preview sin datos sensibles', () => {
  assert.equal(DOC32_RESOURCE_CONTRACT.scope, 'local');
  assert.deepEqual(descriptorFor({ executionTaskId: 10, executionActivityName: 'Supervisor' }, 'execution'), { taskId: 10, activity: 'Supervisor' });
  assert.deepEqual(assessPreviewAvailability({ Destinos: [{ NombreActividad: 'Supervisor', IdConector: 4 }] }, 'Supervisor'), { available: true, code: 'E2E_RESOURCE_READY' });
  assert.deepEqual(assessPreviewAvailability({ Destinos: [] }, 'Supervisor'), { available: false, code: 'E2E_RESOURCE_ACTIVITY_UNAVAILABLE' });
  assert.deepEqual(assessPreviewAvailability({ Destinos: [{ NombreActividad: 'Supervisor', IdConector: 4, BalanceoDisponible: false }] }, 'Supervisor'), { available: false, code: 'E2E_RESOURCE_DESTINATION_UNAVAILABLE' });
  assert.deepEqual(assessPreviewAvailability({ Error: { Codigo: 'WORKFLOW_RETURN_TASK_UNAVAILABLE' }, Destinos: [] }, 'Supervisor'), { available: false, code: 'E2E_RESOURCE_TASK_UNAVAILABLE' });
});

test('la evidencia persistida del ciclo no expone el recurso ni los secretos', async () => {
  const root = await fs.mkdtemp(path.join(os.tmpdir(), 'e2e-resource-evidence-'));
  try {
    await writeResourceLifecycleEvidence({
      root,
      doc: 'sample',
      contractId: 'sample-resource',
      events: [{ role: 'mutate', phase: 'consumed', code: 'E2E_RESOURCE_CONSUMED' }]
    });
    const evidence = await fs.readFile(path.join(root, 'tools', 'e2e', 'artifacts', 'resource-lifecycle-sample-resource.json'), 'utf8');
    assert.match(evidence, /E2E_RESOURCE_CONSUMED/);
    assert.doesNotMatch(evidence, /password|token|record:|workflow-task:/i);
  } finally {
    await fs.rm(root, { recursive: true, force: true });
  }
});
