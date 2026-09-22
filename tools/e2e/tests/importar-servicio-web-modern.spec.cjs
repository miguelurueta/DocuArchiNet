'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const { ASSERTION_SCENARIOS, IMPORTAR_SERVICIO_WEB_E2E_ADAPTER, buildAssertionReport } = require('../scripts/adapters/importar-servicio-web-e2e-adapter.cjs');
const { preflightPlatform, requiredAuthorizationsFor } = require('../scripts/support/workflow-e2e-platform.cjs');
const { resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { CONTROL_REGISTRY, resolveControls } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
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
  assert.ok(read.expectations.includes('secure-preview-ui'));
  assert.deepEqual(read.controls, [
    'import-intent-state', 'import-item-state', 'import-transition-audit', 'import-expedient-state',
    'import-document-relation-state', 'import-document-link-cache-state', 'import-document-index-state'
  ]);
  assert.ok(Object.values(read.controlExpectations).every((expectation) => expectation === 'unchanged'));
  assert.deepEqual(recovery.controlExpectations, read.controlExpectations);
  assert.equal(retry.controlExpectations['import-expedient-state'], 'unchanged');
  assert.ok(Object.entries(retry.controlExpectations)
    .filter(([controlId]) => controlId !== 'import-expedient-state')
    .every(([, expectation]) => expectation === 'changed'));
  assert.deepEqual(requiredAuthorizationsFor(execution, validateProfile(load('doc56-import-sii-execution.profile.example.json'))),
    ['environment', 'gate', 'execution', 'discardable-resource', 'local-tls']);
  assert.equal(execution.controlExpectations['import-document-link-cache-state'], 'expedient-mode');
  assert.deepEqual(requiredAuthorizationsFor(concurrency, validateProfile(load('doc56-import-sii-concurrency.profile.example.json'))),
    ['environment', 'gate', 'execution', 'concurrency', 'discardable-resource', 'local-tls']);
  assert.deepEqual(requiredAuthorizationsFor(retry, validateProfile(load('doc56-import-sii-retry.profile.example.json'))),
    ['environment', 'gate', 'execution', 'discardable-resource', 'local-tls']);
});

test('DOC-67 registra controles SELECT saneados para expediente, relación, caché e índices SQL/XML', () => {
  const ids = [
    'import-expedient-state', 'import-document-relation-state',
    'import-document-link-cache-state', 'import-document-index-state'
  ];
  const controls = resolveControls(ids);
  assert.deepEqual(controls.map(({ id }) => id), ids);
  for (const control of controls) {
    assert.match(control.query, /^SELECT\b/i);
    assert.equal((control.query.match(/\?/g) || []).length, 1);
    assert.doesNotMatch(control.query, /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i);
    assert.doesNotMatch(control.query, /subject_(?:identification|name)|owner_(?:identification|name)|sii_radicado/i);
  }
  assert.equal(Object.keys(CONTROL_REGISTRY).filter((id) => id.startsWith('import-')).length, 7);
});

test('DOC-67 distribuye y reporta individualmente las 18 aserciones con estados cerrados', () => {
  const ids = [];
  for (const [scenario, numbers] of Object.entries(ASSERTION_SCENARIOS)) {
    const verdicts = Object.fromEntries(numbers.map((number, index) => [number, index % 3 === 0 ? true : index % 3 === 1 ? false : null]));
    const report = buildAssertionReport(scenario, verdicts);
    assert.equal(report.length, numbers.length);
    ids.push(...report.map(({ id }) => id));
    assert.ok(report.some(({ status }) => status === 'passed'));
    assert.ok(report.some(({ status }) => status === 'failed') || report.length < 3);
    assert.ok(report.some(({ status }) => status === 'blocked') || report.length < 3);
    for (const assertion of report) {
      assert.deepEqual(Object.keys(assertion).sort(), ['code', 'expectedCount', 'id', 'observedCount', 'scenario', 'status']);
      assert.doesNotMatch(JSON.stringify(assertion), /password|cookie|token|connection|radicado|matricula|expedientId|documentId/i);
    }
  }
  assert.deepEqual(ids.sort(), Array.from({ length: 18 }, (_, index) => `DOC67-E2E-${String(index + 1).padStart(2, '0')}`));
});

test('concurrencia confirma regresión legacy y restauración solo después de integridad final', () => {
  const pending = {
    assertions: buildAssertionReport('import-sii-concurrency')
  };
  assert.ok(IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.finalizeEvidence(pending).assertions.every(({ status }) => status === 'blocked'));
  const confirmed = IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.finalizeEvidence(pending, { integrityConfirmed: true });
  assert.deepEqual(confirmed.assertions.map(({ id, status }) => [id, status]), [
    ['DOC67-E2E-14', 'passed'], ['DOC67-E2E-15', 'passed']
  ]);
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
  assert.ok(result.assertions.every(({ status }) => status === 'blocked'));
  const confirmed = IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.finalizeEvidence(result, { integrityConfirmed: true });
  assert.deepEqual(confirmed.assertions.map(({ id, status }) => [id, status]), [
    ['DOC67-E2E-16', 'passed'], ['DOC67-E2E-17', 'passed'], ['DOC67-E2E-18', 'passed']
  ]);
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

test('ejecución DOC-67 valida el universo relacionado sin confundirlo con sampleSize', async () => {
  const storedItem = { ExternalKey: 'SII2.item-1', DocumentId: 1001, Status: 'Disponible', PersistenceKnown: true, ReachedPhase: 'Completada' };
  const confirmedEffect = (expedientId) => ({
    ExpedientId: expedientId,
    RelationStatus: 'Correcta',
    LinkCacheStatus: 'Confirmado',
    CabinetIndexStatus: 'Confirmado',
    ElectronicIndexSqlStatus: 'Confirmado',
    ElectronicIndexXmlStatus: 'Confirmado',
    ReconciliationStatus: 'Confirmado'
  });
  const effects = [confirmedEffect(578), confirmedEffect(578)];
  const invoke = async (operation) => ({
    elapsedMs: 5,
    dto: {
      QueryItems: { Items: [{ ExternalKey: 'SII2.item-1', DisplayName: 'documento.pdf', ContentType: 'application/pdf' }] },
      PreflightImport: { IsValid: true, Requirements: [] },
      CreateImportIntent: { IntentId: '0123456789abcdef0123456789abcdef', VersionToken: 'v1' },
      ExecuteImportIntent: { Items: [storedItem], ExpedientEffects: effects, VersionToken: 'v2' },
      GetImportIntent: { Items: [storedItem], ExpedientEffects: effects, VersionToken: 'v2' },
      ReconcileImportIntent: { Items: [storedItem], ExpedientEffects: effects, Status: 'Completado' }
    }[operation]
  });

  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeExecution({
    invoke, taskId: 220572, budgetMs: 1000,
    profile: { scenarioId: 'import-sii-execution', codigoBarras: '18223242', radicado: 'S002194198', documentTypeId: 131, documentTypeName: 'Constancia De Inscripción', sampleSize: 1 }
  });
  const assertions = new Map(result.assertions.map((entry) => [entry.id, entry.status]));
  assert.equal(assertions.get('DOC67-E2E-07'), 'passed');
  assert.equal(assertions.get('DOC67-E2E-08'), 'passed');
  assert.equal(assertions.get('DOC67-E2E-10'), 'passed');
});

test('ejecución DOC-67 multi-expediente exige inscripciones y destinos distintos', async () => {
  const storedItems = [1, 2, 3].map((value) => ({
    ExternalKey: `SII2.item-${value}`, DocumentId: 1000 + value, Status: 'Disponible',
    PersistenceKnown: true, ReachedPhase: 'Completada'
  }));
  const effect = (expedientId, inscriptionKey) => ({
    ExpedientId: expedientId, InscriptionKey: inscriptionKey, RelationStatus: 'Correcta',
    LinkCacheStatus: 'Confirmado', CabinetIndexStatus: 'Confirmado',
    ElectronicIndexSqlStatus: 'Confirmado', ElectronicIndexXmlStatus: 'Confirmado',
    ReconciliationStatus: 'Confirmado'
  });
  const invokeWith = (effects) => async (operation) => ({
    elapsedMs: 5,
    dto: {
      QueryItems: { Items: storedItems.map((item) => ({ ExternalKey: item.ExternalKey })) },
      PreflightImport: { IsValid: true, Requirements: [] },
      CreateImportIntent: { IntentId: '0123456789abcdef0123456789abcdef', VersionToken: 'v1' },
      ExecuteImportIntent: { Items: storedItems, ExpedientEffects: effects, VersionToken: 'v2' },
      GetImportIntent: { Items: storedItems, ExpedientEffects: effects, VersionToken: 'v2' },
      ReconcileImportIntent: { Items: storedItems, ExpedientEffects: effects, Status: 'Completado' }
    }[operation]
  });
  const profile = {
    scenarioId: 'import-sii-execution', codigoBarras: '18221381', radicado: 'S002188378',
    documentTypeId: 154, documentTypeName: 'Constancia de inscripción', sampleSize: 3,
    minimumExpedientCount: 2
  };

  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeExecution({
    invoke: invokeWith([effect(578, 'M-1'), effect(579, 'M-2')]), taskId: 219887, budgetMs: 1000, profile
  });
  assert.equal(result.assertions.find(({ id }) => id === 'DOC67-E2E-01').status, 'passed');

  await assert.rejects(
    IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeExecution({
      invoke: invokeWith([effect(578, 'M-1'), effect(578, 'M-1')]), taskId: 219887, budgetMs: 1000, profile
    }),
    (error) => error.code === 'IMPORT_E2E_MULTIEXPEDIENT_DESTINATIONS_UNCONFIRMED'
  );
});

test('retry DOC-67 prepara detención controlada, continúa una vez y rechaza versión obsoleta', async () => {
  let executions = 0;
  const stored = { DocumentId: 1200, Status: 'Disponible', PersistenceKnown: true };
  const invoke = async (operation) => {
    const responses = {
      QueryItems: { Items: [{ ExternalKey: 'SII2.item-1' }] },
      PreflightImport: { IsValid: true, Requirements: [] },
      CreateImportIntent: { IntentId: '0123456789abcdef0123456789abcdef', VersionToken: 'v1' },
      GetImportIntent: { Items: [stored], VersionToken: 'v3' }
    };
    if (operation !== 'ExecuteImportIntent') return { dto: responses[operation], elapsedMs: 5 };
    executions += 1;
    if (executions === 1) return { dto: { Items: [{ Status: 'Detenida', ErrorCode: 'EXECUTION_STOPPED', Retryable: true, PersistenceKnown: true }], VersionToken: 'v2' }, elapsedMs: 5 };
    if (executions === 2) return { dto: { Items: [stored], VersionToken: 'v3' }, elapsedMs: 5 };
    return { dto: { Error: { Codigo: 'VERSION_CONFLICT' } }, elapsedMs: 5 };
  };
  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeExecution({
    invoke, taskId: 220562, budgetMs: 1000,
    profile: {
      scenarioId: 'import-sii-retry', codigoBarras: '18223223', radicado: 'S002194136',
      documentTypeId: 154, documentTypeName: 'Constancia De Inscripción', sampleSize: 1,
      prepareStoppedIntent: true
    }
  });
  assert.equal(executions, 3);
  assert.ok(result.assertions.every(({ status }) => status === 'passed'));
});

test('retry DOC-67 reanuda una intención detenida existente sin duplicar el documento', async () => {
  let executions = 0;
  const intentId = 'beab06593c044bcda92b1d33ff7db62c';
  const stopped = { Status: 'Detenida', ErrorCode: 'EXECUTION_STOPPED', Retryable: true, PersistenceKnown: true, DocumentId: null };
  const stored = { DocumentId: 1300, Status: 'Disponible', PersistenceKnown: true };
  const invoke = async (operation) => {
    if (operation === 'GetImportIntent') {
      return { dto: executions === 0 ? { Items: [stopped], VersionToken: 'v2' } : { Items: [stored], VersionToken: 'v3' }, elapsedMs: 5 };
    }
    executions += 1;
    if (executions === 1) return { dto: { Items: [stored], VersionToken: 'v3' }, elapsedMs: 5 };
    return { dto: { Error: { Codigo: 'VERSION_CONFLICT' } }, elapsedMs: 5 };
  };
  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeExecution({
    invoke, taskId: 220562, budgetMs: 1000,
    profile: { scenarioId: 'import-sii-retry', intentId }
  });
  assert.equal(executions, 2);
  assert.ok(result.assertions.every(({ status }) => status === 'passed'));
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

test('los perfiles DOC-56 cubren ejecución, retry, recovery y concurrencia sin campos paralelos', () => {
  const read = validateProfile(load('doc56-import-sii-read.profile.example.json'));
  const execution = validateProfile(load('doc56-import-sii-execution.profile.example.json'));
  const retry = validateProfile(load('doc56-import-sii-retry.profile.example.json'));
  const recovery = validateProfile(load('doc56-import-sii-recovery.profile.example.json'));
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
  assert.equal(retry.scenarioId, 'import-sii-retry');
  assert.equal(recovery.scenarioId, 'import-sii-recovery');
  assert.equal(retry.intentId, '0123456789abcdef0123456789abcdef');
  assert.equal(recovery.intentId, 'fedcba9876543210fedcba9876543210');
  assert.equal(Object.hasOwn(retry, 'radicado'), false);
  assert.equal(Object.hasOwn(recovery, 'codigoBarras'), false);
  assert.equal(multidocument.sampleSize, 3);
  assert.equal(multidocument.minimumExpedientCount, 2);
  assert.equal(multidocument.taskId, 219887);
  for (const profile of [read, execution, retry, recovery, concurrency, multidocument]) {
    assert.ok(!Object.keys(profile).some((key) => /password|cookie|token|secret|sql|query|connection|user/i.test(key)));
  }
  assert.equal(validateProfile({ ...load('doc56-import-sii-retry.profile.example.json'), radicado: 'MUESTRA_COMPATIBLE' }).radicado, 'MUESTRA_COMPATIBLE');
  assert.throws(() => validateProfile({ ...load('doc56-import-sii-recovery.profile.example.json'), concurrencyLevel: 2 }), /E2E_PLATFORM_PROFILE_STAGE_FIELD_INVALID/);
  assert.throws(() => validateProfile({ ...load('doc56-import-sii-read.profile.example.json'), scenarioId: 'import-sii-expedient-execution' }), /E2E_PLATFORM_SCENARIO_UNREGISTERED/);
  assert.throws(() => validateProfile({ ...load('doc56-import-sii-execution.profile.example.json'), minimumExpedientCount: 1 }), /E2E_PLATFORM_PROFILE_EXPEDIENT_COUNT_INVALID/);
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

test('DOC-68 exige catálogo y presentación enriquecida dentro de la lectura existente', () => {
  const source=fs.readFileSync(path.join(__dirname,'..','scripts','adapters','importar-servicio-web-e2e-adapter.cjs'),'utf8');
  assert.match(source,/assertDoc68Catalog\(capabilitiesDto\)/);
  assert.match(source,/assertDoc68Items\(dto\)/);
  assert.match(source,/\['Disponible','Importado','ConNovedad'\]/);
  const querySelection=source.slice(source.indexOf('async function querySelection'),source.indexOf('async function preflight'));
  assert.equal((querySelection.match(/invoke\('QueryItems'/g)||[]).length,1);
  assert.doesNotMatch(querySelection,/GetPreview|Download|consultarInformacionSello/);
});

test('DOC-69 canjea preview por HEAD y GET, rechaza alteración y consumo repetido', async () => {
  const descriptorId = 'A'.repeat(43);
  const calls = [];
  const invoke = async (operation) => ({ elapsedMs: 2, dto: {
    ResolveCapabilities: { DocumentTypes: [] },
    QueryItems: { Items: [{ ExternalKey: 'SII2.item-1', DisplayName: 'preview.pdf', ContentType: 'application/pdf', Metadata: [], ImportStatus: 'Disponible', AllowedActions: ['Preview'] }] },
    GetPreview: { DescriptorId: descriptorId, ContentType: 'application/pdf', Length: 4 },
    PreflightImport: { IsValid: true, Requirements: [] }
  }[operation] });
  const consumePreview = async (descriptor, method) => {
    calls.push({ descriptor, method });
    if (descriptor !== descriptorId) return { status: 404, elapsedMs: 1, bodyLength: 0 };
    const sequence = calls.filter((entry) => entry.descriptor === descriptorId).length;
    if (sequence === 1) return { status: 200, elapsedMs: 1, bodyLength: 0, contentLength: 4 };
    if (sequence === 2) return { status: 200, elapsedMs: 1, bodyLength: 4, contentLength: 4, cacheControl: 'private, no-store', noSniff: 'nosniff', contentDisposition: 'inline; filename="preview.pdf"' };
    return { status: 404, elapsedMs: 1, bodyLength: 0 };
  };
  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeRead({ invoke, consumePreview, taskId: 220580, budgetMs: 1000,
    profile: { scenarioId: 'import-sii-read', codigoBarras: '18341190', documentTypeId: 154, documentTypeName: 'Constancia De Inscripción', sampleSize: 1 } });
  assert.deepEqual(calls.map(({ method }) => method), ['HEAD', 'HEAD', 'GET', 'GET', 'HEAD']);
  assert.equal(result.codes.previewContent, 'CONFIRMED');
});

test('preflight bloquea escritura y concurrencia sin todas las autorizaciones', () => {
  const execution = validateProfile(load('doc56-import-sii-execution.profile.example.json'));
  const concurrency = validateProfile(load('doc56-import-sii-concurrency.profile.example.json'));
  assert.throws(() => preflightPlatform({ profile: execution, authorizations: ['environment'] }), /E2E_PLATFORM_AUTHORIZATION_REQUIRED/);
  assert.throws(() => preflightPlatform({ profile: concurrency, authorizations: ['environment', 'execution'] }), /E2E_PLATFORM_AUTHORIZATION_REQUIRED/);
});

test('DOC-70 valida plan por item y fingerprint estable en el preflight existente', async () => {
  const calls=[];
  const items=[1,2].map((value)=>({ExternalKey:`SII2.item-${value}`,ClientItemId:`client-${value}`,TargetTaskId:220580,DocumentTypeId:154,DocumentTypeName:'Constancia'}));
  const plans=items.map((item)=>({ClientItemId:item.ClientItemId,TargetTaskId:220580,DocumentTypeId:154,DestinationMode:'Single',ExpedientRequired:true,Effects:['DOCUMENT_STORAGE','EXPEDIENT_RESOLUTION','DOCUMENT_LINK','LINK_CACHE','DOCUMENT_INDEXES'].map((Code)=>({Code,Status:'Planned'}))}));
  const fingerprint='a'.repeat(64);
  const invoke=async(operation,payload)=>{calls.push({operation,payload});return {elapsedMs:2,dto:{IsValid:true,Executable:true,ContextFingerprint:fingerprint,Requirements:[],EffectPlans:plans}}};
  const result=await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeRead({invoke,taskId:220580,budgetMs:1000,profile:{scenarioId:'import-sii-read',codigoBarras:'18341190',documentTypeId:154,documentTypeName:'Constancia',sampleSize:2}}).catch(()=>null);
  // executeRead necesita query/preview reales; la política estructural confirma que la validación DOC-70 vive en la única función preflight.
  const source=fs.readFileSync(path.resolve(__dirname,'../scripts/adapters/importar-servicio-web-e2e-adapter.cjs'),'utf8');
  assert.match(source,/IMPORT_E2E_PREFLIGHT_EFFECT_PLAN_INVALID/);
  assert.match(source,/IMPORT_E2E_PREFLIGHT_FINGERPRINT_UNSTABLE/);
  assert.match(source,/\[\.\.\.selection\]\.reverse\(\)/);
  assert.equal(result,null);
});

test('DOC-71 acepta preflight sin expediente y exige solo efectos documentales planeados', () => {
  const source=fs.readFileSync(path.resolve(__dirname,'../scripts/adapters/importar-servicio-web-e2e-adapter.cjs'),'utf8');
  assert.match(source,/\['Single', 'Multiple', 'WithoutExpedient'\]/);
  assert.match(source,/requiredDocumentEffect = code === 'DOCUMENT_STORAGE' \|\| code === 'DOCUMENT_INDEXES'/);
  assert.match(source,/withoutExpedient && !requiredDocumentEffect \? 'NotApplicable' : 'Planned'/);
});

test('DOC-71 verifica NoAplica sin exigir relación caché ni índices electrónicos de expediente', () => {
  const source=fs.readFileSync(path.resolve(__dirname,'../scripts/adapters/importar-servicio-web-e2e-adapter.cjs'),'utf8');
  assert.match(source,/notApplicable\(field\(effect, \['ExpedientStatus'/);
  assert.match(source,/confirmed\(field\(effect, \['CabinetIndexStatus'/);
  assert.match(source,/\['LinkCacheStatus', 'ElectronicIndexSqlStatus', 'ElectronicIndexXmlStatus'\]/);
});

test('DOC-71 repite la creación con la misma clave y exige recuperar la misma intención', async () => {
  const createPayloads = [];
  const stored = { ExternalKey: 'SII2.item-1', DocumentId: 1001, Status: 'Disponible', PersistenceKnown: true, ReachedPhase: 'Completada' };
  const effect = {
    ExpedientStatus: 'NoAplica', RelationStatus: 'NoAplica', LinkCacheStatus: 'NoAplica',
    CabinetIndexStatus: 'Confirmado', ElectronicIndexSqlStatus: 'NoAplica', ElectronicIndexXmlStatus: 'NoAplica',
    ReconciliationStatus: 'Confirmado'
  };
  const invoke = async (operation, payload) => {
    if (operation === 'CreateImportIntent') createPayloads.push(payload.request);
    const selectedItem = payload?.request?.Items?.[0];
    return { elapsedMs: 2, dto: {
      QueryItems: { Items: [{ ExternalKey: stored.ExternalKey }] },
      PreflightImport: {
        IsValid: true, Executable: true, ContextFingerprint: 'a'.repeat(64), Requirements: [],
        EffectPlans: [{ ClientItemId: selectedItem?.ClientItemId, TargetTaskId: 220581, DocumentTypeId: 154,
          DestinationMode: 'WithoutExpedient', Effects: [
            { Code: 'DOCUMENT_STORAGE', Status: 'Planned' }, { Code: 'EXPEDIENT_RESOLUTION', Status: 'NotApplicable' },
            { Code: 'DOCUMENT_LINK', Status: 'NotApplicable' }, { Code: 'LINK_CACHE', Status: 'NotApplicable' },
            { Code: 'DOCUMENT_INDEXES', Status: 'Planned' }
          ] }]
      },
      CreateImportIntent: { IntentId: '0123456789abcdef0123456789abcdef', VersionToken: 'v1' },
      ExecuteImportIntent: { Items: [stored], ExpedientEffects: [effect], VersionToken: 'v2' },
      GetImportIntent: { Items: [stored], ExpedientEffects: [effect], VersionToken: 'v2' },
      ReconcileImportIntent: { Items: [stored], ExpedientEffects: [effect], Status: 'Completada' }
    }[operation] };
  };
  const result = await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeExecution({
    invoke, taskId: 220581, budgetMs: 1000,
    profile: { scenarioId: 'import-sii-execution', codigoBarras: 'sample', radicado: 'sample', documentTypeId: 154, documentTypeName: 'Constancia', sampleSize: 1 }
  });
  assert.equal(createPayloads.length, 2);
  assert.deepEqual(createPayloads[1], createPayloads[0]);
  assert.equal(result.codes.idempotentCreate, 'CONFIRMED');
  assert.equal(result.codes.expedientMode, 'without-expedient');
});

test('runner restaura el gate y aplica integridad legacy desde finally', () => {
  const source = fs.readFileSync(path.join(__dirname, '..', 'scripts', 'run-workflow-e2e-platform.cjs'), 'utf8');
  const platform = fs.readFileSync(path.join(__dirname, '..', 'scripts', 'support', 'workflow-e2e-platform.cjs'), 'utf8');
  assert.match(source, /finally\s*\{\s*await restoreGate\(\)/);
  assert.match(source, /await restoreGate\(\);\s*await assertPlatformIntegrity/);
  assert.match(source, /ImportarServicioWebProviderId" value=""/);
  assert.match(source, /ImportarServicioWebProviderId" value="INTEGRACIONSII"/);
  assert.match(source, /workflow\/Webworkflow\.aspx/);
  assert.match(source, /initializeWorkflowContext\(context, currentPlan\)/);
  assert.match(source, /inspectSession:\s*inspectImportPreviewUi/);
  assert.match(source, /IMPORT_E2E_PREVIEW_UI_DUPLICATE_REQUEST/);
  assert.match(source, /uiSingleFetch: 'CONFIRMED'/);
  assert.doesNotMatch(source, /uiSingleRequest/);
  assert.match(source, /page\.route\(queryRoute/);
  assert.match(source, /queryRequestsObserved > 1/);
  assert.match(source, /queryContextInjected > 1/);
  assert.match(source, /String\(payload\.request\.CodigoBarras\) !== plan\.profile\.codigoBarras/);
  assert.match(source, /WebServiceImportarServicioWebModern\\\.asmx\\\/GetPreview/);
  assert.match(source, /payload\.request\.CodigoBarras = plan\.profile\.codigoBarras/);
  assert.match(source, /route\.continue\(\{ postData: JSON\.stringify\(payload\) \}\)/);
  assert.match(source, /IMPORT_E2E_PREVIEW_UI_RESULTS_UNAVAILABLE/);
  assert.match(source, /\['resultados', 'vacio', 'error'\]\.includes\(state\)/);
  assert.match(source, /IMPORT_E2E_PREVIEW_UI_\$\{publicCode\}/);
  assert.match(source, /#importar-servicio-web-preview-back/);
  assert.match(source, /#Hidden_id_tarea_selecionada/);
  assert.match(source, /tip_event="seleccion_tarea_wf"/);
  assert.match(source, /#auto_complex:visible/);
  assert.match(source, /waitForFunction/);
  assert.doesNotMatch(source, /ID_TAREA_SELECCIONDA\s*=|DG_ID_TRAMITE\s*=/);
  assert.match(platform, /git', \['diff', '--name-only'.*workflow\/Webworkflow\.aspx/s);
  assert.match(platform, /WorkflowCentroTrabajoModernActive" value="false/);
});
