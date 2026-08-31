'use strict';

const assert = require('node:assert/strict');
const { spawnSync } = require('node:child_process');
const path = require('node:path');
const test = require('node:test');
const {
  PlatformExecutionError,
  createRestrictedInvoker,
  createSafeEvidence,
  preflightPlatform
} = require('../scripts/support/workflow-e2e-platform.cjs');
const { resolveAdapter } = require('../scripts/support/workflow-e2e-platform-registry.cjs');

function plan() {
  return preflightPlatform({
    profile: {
      scenarioId: 'notes-read',
      baseUrl: 'https://workflow.example.invalid/app/',
      module: 'GESTOR',
      environment: 'CERTIFICACION',
      odbcDsn: 'workflowconta',
      taskId: 708,
      budgetMs: 10000,
      ignoreHttpsErrors: false
    },
    authorizations: ['environment']
  });
}

test('el invocador restringido rechaza operaciones y payloads no declarados antes del transporte', async () => {
  let invoked = 0;
  const invoke = createRestrictedInvoker({
    adapter: resolveAdapter('notes-read'),
    client: {},
    invoke: async () => { invoked += 1; return { dto: {}, elapsedMs: 1 }; }
  });
  await assert.rejects(() => invoke('EliminarNota', { idTarea: 708, idNota: 1 }),
    (error) => error instanceof PlatformExecutionError && error.code === 'E2E_PLATFORM_OPERATION_INVALID');
  await assert.rejects(() => invoke('ListarNotas', { idTarea: 708, cursor: '', tamanoPagina: 1, contenido: 'prohibido' }),
    (error) => error instanceof PlatformExecutionError && error.code === 'E2E_PLATFORM_OPERATION_INVALID');
  assert.equal(invoked, 0);
});

test('la evidencia solo acepta el esquema saneado y no permite claves sensibles', () => {
  const clean = createSafeEvidence({
    plan: plan(),
    result: { codes: { list: null }, count: 0, latenciesMs: [1] },
    before: { 'notes-task-state': 'a'.repeat(64) },
    after: { 'notes-task-state': 'a'.repeat(64) },
    failureCode: null,
    resourceEvents: []
  });
  assert.equal(clean.controls.unchanged, true);
  assert.throws(() => createSafeEvidence({
    plan: plan(),
    result: { codes: { cookie: 'valor-prohibido' }, count: 0, latenciesMs: [1] },
    before: {},
    after: {},
    failureCode: null,
    resourceEvents: []
  }), (error) => error instanceof PlatformExecutionError && error.code === 'E2E_PLATFORM_EVIDENCE_INVALID');
});

test('el comando de plataforma rechaza argumentos libres antes de intentar una corrida', () => {
  const runner = path.join(__dirname, '..', 'scripts', 'run-workflow-e2e-platform.cjs');
  const result = spawnSync(process.execPath, [runner, '--script', 'externo.cjs'], { encoding: 'utf8' });
  assert.equal(result.status, 2);
  assert.match(result.stderr, /E2E_PLATFORM_ARGUMENT_INVALID/);
  assert.doesNotMatch(`${result.stdout}\n${result.stderr}`, /externo\.cjs|password|cookie|token/i);
});
