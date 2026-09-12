'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const { IMPORTAR_SERVICIO_WEB_E2E_ADAPTER } = require('../scripts/adapters/importar-servicio-web-e2e-adapter.cjs');
const { preflightPlatform, requiredAuthorizationsFor } = require('../scripts/support/workflow-e2e-platform.cjs');
const { resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');

const profiles = path.resolve(__dirname, '..', 'profiles');
const load = (name) => JSON.parse(fs.readFileSync(path.join(profiles, name), 'utf8'));

test('DOC-56 registra lectura, ejecución y concurrencia sobre la plataforma común', () => {
  const read = resolveScenario('import-sii-read');
  const recovery = resolveScenario('import-sii-recovery');
  const retry = resolveScenario('import-sii-retry');
  const execution = resolveScenario('import-sii-execution');
  const concurrency = resolveScenario('import-sii-concurrency');
  assert.equal(read.stage, 'read');
  assert.equal(recovery.stage, 'read');
  assert.equal(retry.stage, 'execution');
  assert.equal(execution.stage, 'execution');
  assert.equal(concurrency.stage, 'concurrency');
  assert.equal(read.adapterId, IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.id);
  assert.deepEqual(read.controlExpectations, {
    'import-intent-state': 'unchanged', 'import-item-state': 'unchanged', 'import-transition-audit': 'unchanged'
  });
  assert.deepEqual(recovery.controlExpectations, read.controlExpectations);
  assert.deepEqual(requiredAuthorizationsFor(execution, validateProfile(load('doc56-import-sii-execution.profile.example.json'))),
    ['environment', 'gate', 'execution', 'discardable-resource', 'local-tls']);
  assert.deepEqual(requiredAuthorizationsFor(concurrency, validateProfile(load('doc56-import-sii-concurrency.profile.example.json'))),
    ['environment', 'gate', 'execution', 'concurrency', 'discardable-resource', 'local-tls']);
  assert.deepEqual(requiredAuthorizationsFor(retry, validateProfile({ ...load('doc56-import-sii-execution.profile.example.json'), scenarioId: 'import-sii-retry', intentId: '0123456789abcdef0123456789abcdef' })),
    ['environment', 'gate', 'execution', 'discardable-resource', 'local-tls']);
});

test('recuperación DOC-56 sólo consulta una intención opaca existente', async () => {
  const calls = [];
  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeRead({
    taskId: 219887,
    budgetMs: 1000,
    profile: { scenarioId: 'import-sii-recovery', intentId: '0123456789abcdef0123456789abcdef' },
    invoke: async (operation, payload) => {
      calls.push({ operation, payload });
      return { dto: { Items: [{ DocumentId: 123, Status: 'Disponible', PersistenceKnown: true }] }, elapsedMs: 5 };
    }
  });
  assert.deepEqual(calls.map(({ operation }) => operation), ['GetImportIntent']);
  assert.equal(result.count, 1);
});

test('ejecución DOC-56 envía y confirma cuatro documentos en una sola intención', async () => {
  const calls = [];
  const externalItems = Array.from({ length: 4 }, (_, index) => ({
    ExternalKey: `SII2.item-${index + 1}`, DisplayName: `documento-${index + 1}.pdf`, ContentType: 'application/pdf'
  }));
  const storedItems = externalItems.map((item, index) => ({
    ExternalKey: item.ExternalKey, DocumentId: 1001 + index, Status: 'Disponible', PersistenceKnown: true
  }));
  const invoke = async (operation, payload) => {
    calls.push({ operation, payload });
    const responses = {
      QueryItems: { Items: externalItems },
      PreflightImport: { IsValid: true, Requirements: [] },
      CreateImportIntent: { IntentId: '0123456789abcdef0123456789abcdef', VersionToken: 'v1' },
      ExecuteImportIntent: { Items: storedItems, VersionToken: 'v2' },
      GetImportIntent: { Items: storedItems, VersionToken: 'v2' },
      ReconcileImportIntent: { Items: storedItems }
    };
    return { dto: responses[operation], elapsedMs: 5 };
  };
  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeExecution({
    invoke, taskId: 219887, budgetMs: 1000,
    profile: { scenarioId: 'import-sii-execution', codigoBarras: '18221381', radicado: 'S002188378', documentTypeId: 154, documentTypeName: 'Constancia de inscripción', sampleSize: 4 }
  });
  const preflight = calls.find(({ operation }) => operation === 'PreflightImport').payload.request;
  const create = calls.find(({ operation }) => operation === 'CreateImportIntent').payload.request;
  assert.equal(preflight.Items.length, 4);
  assert.equal(create.Items.length, 4);
  assert.equal(new Set(create.Items.map((item) => item.ExternalKey)).size, 4);
  assert.equal(result.count, 4);
});

test('ejecución y concurrencia exigen confirmación documental reconciliada', () => {
  const source = fs.readFileSync(path.resolve(__dirname, '..', 'scripts', 'adapters', 'importar-servicio-web-e2e-adapter.cjs'), 'utf8');
  assert.match(source, /function assertStoredDocuments[\s\S]*recovered\.length !== expectedCount/);
  assert.match(source, /IMPORT_E2E_STORED_DOCUMENT_STATUS_NOT_AVAILABLE_[\s\S]*IMPORT_E2E_STORED_DOCUMENT_ID_MISSING[\s\S]*IMPORT_E2E_STORED_DOCUMENT_PERSISTENCE_UNKNOWN/);
  assert.match(source, /safeLegacyDiagnostic/);
  const runner = fs.readFileSync(path.resolve(__dirname, '..', 'scripts', 'run-workflow-e2e-platform.cjs'), 'utf8');
  assert.match(runner, /LEGACY_FUNCTION_RESPONSE/);
  assert.match(source, /executeExecution[\s\S]*GetImportIntent[\s\S]*assertStoredDocuments/);
  assert.match(source, /executeConcurrency[\s\S]*GetImportIntent[\s\S]*storedDocument: 'CONFIRMED'/);
});

test('los perfiles DOC-56 contienen sólo ambiente, recurso y presupuestos no sensibles', () => {
  const read = validateProfile(load('doc56-import-sii-read.profile.example.json'));
  const execution = validateProfile(load('doc56-import-sii-execution.profile.example.json'));
  const concurrency = validateProfile(load('doc56-import-sii-concurrency.profile.example.json'));
  const multidocument = validateProfile(load('doc56-import-sii-multidocument.profile.example.json'));
  assert.equal(read.sampleSize, 1);
  assert.equal(read.module, 'WORKFLOW REGISTRO');
  assert.equal(execution.module, 'WORKFLOW REGISTRO');
  assert.equal(concurrency.module, 'WORKFLOW REGISTRO');
  assert.equal(read.odbcDsn, 'workflowdocument');
  assert.equal(execution.odbcDsn, 'workflowdocument');
  assert.equal(concurrency.odbcDsn, 'workflowdocument');
  assert.equal(read.taskId, 219887);
  assert.equal(execution.taskId, 219888);
  assert.equal(concurrency.taskId, 219888);
  assert.equal(read.radicado, 'S002188378');
  assert.equal(read.codigoBarras, '18221381');
  assert.equal(execution.radicado, 'S002188335');
  assert.equal(execution.codigoBarras, '18221365');
  assert.equal(execution.documentTypeId, 154);
  assert.equal(execution.documentTypeName, 'Constancia de inscripción');
  assert.equal(concurrency.documentTypeId, 154);
  assert.equal(concurrency.documentTypeName, 'Constancia de inscripción');
  assert.equal(concurrency.concurrencyLevel, 2);
  assert.equal(multidocument.sampleSize, 3);
  assert.equal(multidocument.taskId, 219887);
  for (const profile of [read, execution, concurrency, multidocument]) {
    assert.ok(!Object.keys(profile).some((key) => /password|cookie|token|secret|sql|query|connection|user/i.test(key)));
  }
});

test('adaptador DOC-56 declara ocho operaciones y no duplica infraestructura transversal', () => {
  assert.deepEqual(Object.values(IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.operations).map((entry) => entry.id), [
    'ResolveCapabilities', 'QueryItems', 'GetPreview', 'PreflightImport', 'CreateImportIntent',
    'ExecuteImportIntent', 'GetImportIntent', 'ReconcileImportIntent'
  ]);
  const source = fs.readFileSync(path.join(__dirname, '..', 'scripts', 'adapters', 'importar-servicio-web-e2e-adapter.cjs'), 'utf8');
  assert.match(source, /DocumentTypeId: documentTypeId, DocumentTypeName: documentTypeName/);
  assert.match(source, /profile\.documentTypeId, profile\.documentTypeName, profile\.sampleSize, budgetMs, latencies/);
  assert.doesNotMatch(source, /require\(|createAuthenticatedWorkflowSession|queryFingerprint|promptSecret|ignoreHTTPSErrors|writeFile|setx/i);
});

test('preflight bloquea escritura y concurrencia sin todas las autorizaciones', () => {
  const execution = validateProfile(load('doc56-import-sii-execution.profile.example.json'));
  const concurrency = validateProfile(load('doc56-import-sii-concurrency.profile.example.json'));
  assert.throws(() => preflightPlatform({ profile: execution, authorizations: ['environment'] }), /E2E_PLATFORM_AUTHORIZATION_REQUIRED/);
  assert.throws(() => preflightPlatform({ profile: concurrency, authorizations: ['environment', 'execution'] }), /E2E_PLATFORM_AUTHORIZATION_REQUIRED/);
});

test('runner restaura el gate y aplica integridad legacy desde finally', () => {
  const source = fs.readFileSync(path.join(__dirname, '..', 'scripts', 'run-workflow-e2e-platform.cjs'), 'utf8');
  const platform = fs.readFileSync(path.join(__dirname, '..', 'scripts', 'support', 'workflow-e2e-platform.cjs'), 'utf8');
  assert.match(source, /finally\s*\{\s*await restoreGate\(\)/);
  assert.match(source, /await restoreGate\(\);\s*await assertPlatformIntegrity/);
  assert.match(source, /workflow\/Webworkflow\.aspx/);
  assert.match(source, /initializeWorkflowContext\(context, currentPlan\)/);
  assert.doesNotMatch(source, /ID_TAREA_SELECCIONDA\s*=|DG_ID_TRAMITE\s*=/);
  assert.match(platform, /git', \['diff', '--name-only'.*workflow\/Webworkflow\.aspx/s);
  assert.match(platform, /WorkflowCentroTrabajoModernActive" value="false/);
});
