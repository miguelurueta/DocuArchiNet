'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const { NOTES_WRITE_E2E_ADAPTER } = require('../scripts/adapters/notes-write-e2e-adapter.cjs');
const { createRestrictedInvoker, preflightPlatform } = require('../scripts/support/workflow-e2e-platform.cjs');
const { resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');

const root = path.resolve(__dirname, '..');
const writeProfile = JSON.parse(fs.readFileSync(path.join(root, 'profiles', 'doc42-notes-write.profile.example.json'), 'utf8'));
const concurrencyProfile = JSON.parse(fs.readFileSync(path.join(root, 'profiles', 'doc42-notes-concurrency.profile.example.json'), 'utf8'));

test('DOC-42 registra escenarios mutantes con autorizaciones y recursos descartables', () => {
  const write = resolveScenario('notes-write');
  const concurrency = resolveScenario('notes-concurrency');
  assert.equal(write.doc, 'doc42');
  assert.equal(write.stage, 'execution');
  assert.equal(concurrency.stage, 'concurrency');
  assert.equal(write.resource.mutating, true);
  assert.equal(concurrency.resource.mutating, true);
  assert.deepEqual(write.controls, ['notes-task-state', 'notes-audit']);
  assert.deepEqual(write.controlExpectations, { 'notes-task-state': 'unchanged', 'notes-audit': 'changed' });
  assert.deepEqual(concurrency.controlExpectations, { 'notes-task-state': 'changed', 'notes-audit': 'changed' });
  const writeSpec = fs.readFileSync(path.join(root, 'tests', 'notes-workflow.spec.cjs'), 'utf8');
  assert.match(writeSpec, /expect\(afterState, 'La secuencia crear-editar-eliminar no debe dejar una nota persistida\.'\)\.toBe\(beforeState\)/);
  assert.match(writeSpec, /expect\(afterAudit, 'Las escrituras autorizadas deben reflejarse en auditoría\.'\)\.not\.toBe\(beforeAudit\)/);
  assert.deepEqual(preflightPlatform({ profile: validateProfile(writeProfile), authorizations: ['environment', 'execution', 'discardable-resource'] }).scenario, write);
  assert.throws(() => preflightPlatform({ profile: validateProfile(writeProfile), authorizations: ['environment'] }), /E2E_PLATFORM_AUTHORIZATION_REQUIRED/);
});

test('los perfiles DOC-42 son no sensibles y separan tarea de nota semilla', () => {
  const write = validateProfile(writeProfile);
  const concurrency = validateProfile(concurrencyProfile);
  assert.equal(write.taskId, 1);
  assert.equal(concurrency.taskId, 1);
  assert.equal(concurrency.noteId, 1);
  for (const profile of [write, concurrency]) {
    assert.equal(profile.odbcDsn, 'workflowconta');
    assert.equal(profile.ignoreHttpsErrors, false);
    assert.ok(!Object.keys(profile).some((key) => /password|cookie|token|secret|sql|query|mysql|user/i.test(key)));
  }
});

test('el adaptador DOC-42 declara sólo operaciones de Notas y no infraestructura transversal', () => {
  const source = fs.readFileSync(path.join(root, 'scripts', 'adapters', 'notes-write-e2e-adapter.cjs'), 'utf8');
  assert.deepEqual(Object.values(NOTES_WRITE_E2E_ADAPTER.operations).map((operation) => operation.id), [
    'CrearNota', 'ConsultarNota', 'ActualizarNota', 'EliminarNota'
  ]);
  assert.doesNotMatch(source, /require\(|createAuthenticatedWorkflowSession|queryFingerprint|promptSecret|ignoreHTTPSErrors|writeFile|setx/i);
});

test('el invocador restringido valida payloads de escritura antes del transporte', async () => {
  let calls = 0;
  const invoke = createRestrictedInvoker({
    adapter: NOTES_WRITE_E2E_ADAPTER,
    client: {},
    invoke: async () => { calls += 1; return { dto: { Exito: true }, elapsedMs: 1 }; }
  });
  await assert.rejects(() => invoke('CrearNota', { idTarea: 1, contenido: 'x', clientRequestId: 'no-es-uuid' }), /E2E_PLATFORM_OPERATION_INVALID/);
  await assert.rejects(() => invoke('ActualizarNota', { idTarea: 1, idNota: 1, contenido: 'x', version: 'no-es-sha256' }), /E2E_PLATFORM_OPERATION_INVALID/);
  assert.equal(calls, 0);
});
