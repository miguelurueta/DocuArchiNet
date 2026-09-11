'use strict';

const SERVICE_PATH = 'webservice/WebServiceImportarServicioWebModern.asmx';

function fail(code, diagnostic = '') {
  const error = new Error(`El adaptador DOC-56 detuvo la etapa (${code}).`);
  error.code = code;
  if (diagnostic) error.diagnostic = diagnostic;
  throw error;
}

function field(value, names) {
  if (!value || typeof value !== 'object') return undefined;
  for (const name of names) if (Object.hasOwn(value, name)) return value[name];
  return undefined;
}

function errorCode(dto) {
  return field(field(dto, ['Error', 'error']), ['Codigo', 'codigo']) || null;
}

function assertResult(result, budgetMs, code) {
  if (!result || !Number.isSafeInteger(result.elapsedMs) || result.elapsedMs < 0 || result.elapsedMs > budgetMs) fail(code);
  const functionalCode = errorCode(result.dto);
  if (functionalCode) {
    const safeCode = typeof functionalCode === 'string' && /^[A-Z0-9_]{1,80}$/.test(functionalCode)
      ? functionalCode
      : 'UNSAFE_OR_INVALID_ERROR_CODE';
    const diagnostic = safeLegacyDiagnostic(field(field(result.dto, ['Error', 'error']), ['MensajeVisible', 'mensajeVisible']));
    fail(`${code}_${safeCode}`, diagnostic);
  }
  return result.dto;
}

function uuid() {
  const value = globalThis.crypto?.randomUUID?.();
  if (!value) fail('IMPORT_E2E_CORRELATION_UNAVAILABLE');
  return value;
}

function base(taskId) {
  return { OperationId: uuid(), CorrelationId: uuid(), TaskId: taskId, ProviderId: 'INTEGRACIONSII' };
}

function request(payload) {
  return { request: payload };
}

function items(dto) {
  const value = field(dto, ['Items', 'items']);
  return Array.isArray(value) ? value : [];
}

function assertSingleStoredDocument(dto) {
  const recovered = items(dto);
  if (recovered.length !== 1) fail('IMPORT_E2E_STORED_DOCUMENT_COUNT_INVALID');
  const item = recovered[0];
  const documentId = field(item, ['DocumentId', 'documentId']);
  const status = field(item, ['Status', 'status']);
  const persistenceKnown = field(item, ['PersistenceKnown', 'persistenceKnown']);
  const reconciliationCode = field(item, ['ErrorCode', 'errorCode']);
  if (status !== 'Disponible') {
    const safeDetail = typeof reconciliationCode === 'string' && /^[A-Z0-9_]{1,64}$/.test(reconciliationCode)
      ? reconciliationCode
      : (typeof status === 'string' && /^[A-Za-z0-9_]{1,40}$/.test(status) ? status.toUpperCase() : 'UNKNOWN');
    const legacyMessage = safeLegacyDiagnostic(field(item, ['Message', 'message']));
    fail(`IMPORT_E2E_STORED_DOCUMENT_STATUS_NOT_AVAILABLE_${safeDetail}`, legacyMessage);
  }
  if (!Number.isSafeInteger(documentId) || documentId <= 0) fail('IMPORT_E2E_STORED_DOCUMENT_ID_MISSING');
  if (persistenceKnown !== true) fail('IMPORT_E2E_STORED_DOCUMENT_PERSISTENCE_UNKNOWN');
  return recovered;
}

function safeLegacyDiagnostic(value) {
  if (typeof value !== 'string' || value.trim() === '') return '';
  return value.replace(/[\r\n\t]+/g, ' ').trim().slice(0, 500);
}

function assertRetryableStorageFailure(dto) {
  const currentItems = items(dto);
  if (currentItems.length !== 1) fail('IMPORT_E2E_RETRY_ITEM_COUNT_INVALID');
  const item = currentItems[0];
  const currentCode = field(item, ['ErrorCode', 'errorCode']) || '';
  const reachedPhase = field(item, ['ReachedPhase', 'reachedPhase']);
  const recoverablePreStorage = ['RecursoObtenido', 'ExpedientePreparado', 'IndicesActualizados'].includes(reachedPhase) &&
    field(item, ['PersistenceKnown', 'persistenceKnown']) === true && field(item, ['DocumentId', 'documentId']) == null;
  const retryableStorageFailure = /^DOCUMENT_STORAGE_[A-Z0-9_]+$/.test(currentCode) &&
    field(item, ['Retryable', 'retryable']) === true && field(item, ['PersistenceKnown', 'persistenceKnown']) === true &&
    field(item, ['DocumentId', 'documentId']) == null;
  if (!recoverablePreStorage && !retryableStorageFailure) {
    const safeDetail = typeof currentCode === 'string' && /^[A-Z0-9_]{1,64}$/.test(currentCode)
      ? currentCode
      : 'STATE_MISMATCH';
    fail(`IMPORT_E2E_RETRY_NOT_ALLOWED_${safeDetail}`, safeLegacyDiagnostic(field(item, ['Message', 'message'])));
  }
}

function selectedItem(dto, documentTypeId, documentTypeName) {
  const rawItems = field(dto, ['Items', 'items']);
  if (!Array.isArray(rawItems)) fail('IMPORT_E2E_ITEMS_FIELD_MISSING');
  if (rawItems.length === 0) {
    const providerCode = field(dto, ['ProviderResultCode', 'providerResultCode']);
    const safe = {
      SII_INSCRIPTIONS_MISSING: 'IMPORT_E2E_SII_INSCRIPTIONS_MISSING',
      SII_INSCRIPTIONS_EMPTY: 'IMPORT_E2E_SII_INSCRIPTIONS_EMPTY',
      SII_IMAGES_EMPTY: 'IMPORT_E2E_SII_IMAGES_EMPTY'
    };
    fail(safe[providerCode] || 'IMPORT_E2E_ITEMS_EMPTY_WITHOUT_PROVIDER_DIAGNOSTIC');
  }
  const item = rawItems[0];
  const externalKey = field(item, ['ExternalKey', 'externalKey']);
  if (!item || typeof item !== 'object') fail('IMPORT_E2E_ITEM_INVALID');
  if (typeof externalKey !== 'string' || !externalKey.trim()) fail('IMPORT_E2E_EXTERNAL_KEY_MISSING');
  return {
    ClientItemId: uuid(), ExternalKey: externalKey.trim(), TargetTaskId: null,
    DocumentTypeId: documentTypeId, DocumentTypeName: documentTypeName,
    FileName: field(item, ['DisplayName', 'displayName']) || 'documento-sii.pdf',
    ContentType: field(item, ['ContentType', 'contentType']) || 'application/pdf'
  };
}

async function querySelection(invoke, taskId, codigoBarras, documentTypeId, documentTypeName, budgetMs, latencies) {
  if (typeof codigoBarras !== 'string' || !codigoBarras.trim()) fail('IMPORT_E2E_BARCODE_REQUIRED');
  const query = await invoke('QueryItems', request({ ...base(taskId), CodigoBarras: codigoBarras.trim(), PageSize: 1, ContinuationToken: '' }));
  const dto = assertResult(query, budgetMs, 'IMPORT_E2E_QUERY_FAILED');
  latencies.push(query.elapsedMs);
  const selection = selectedItem(dto, documentTypeId, documentTypeName);
  selection.TargetTaskId = taskId;
  return selection;
}

async function preflight(invoke, taskId, selection, budgetMs, latencies) {
  const result = await invoke('PreflightImport', request({ ...base(taskId), Items: [selection] }));
  const dto = assertResult(result, budgetMs, 'IMPORT_E2E_PREFLIGHT_FAILED');
  latencies.push(result.elapsedMs);
  if (field(dto, ['IsValid', 'isValid']) !== true) fail('IMPORT_E2E_PREFLIGHT_INVALID');
  return dto;
}

function createPayload(taskId, selection, preflightDto, radicado, idempotencyKey) {
  return {
    ...base(taskId), IdempotencyKey: idempotencyKey, Items: [selection], Radicado: radicado,
    Requirements: field(preflightDto, ['Requirements', 'requirements']) || [],
    ContextFingerprint: field(preflightDto, ['ContextFingerprint', 'contextFingerprint'])
  };
}

function intentIdentity(dto) {
  const IntentId = field(dto, ['IntentId', 'intentId']);
  const VersionToken = field(dto, ['VersionToken', 'versionToken']);
  if (typeof IntentId !== 'string' || !IntentId || typeof VersionToken !== 'string' || !VersionToken) fail('IMPORT_E2E_INTENT_INVALID');
  return { IntentId, VersionToken };
}

function executionPayload(taskId, identity) {
  return { ...base(taskId), ...identity, StopRequested: false };
}

const operations = Object.freeze({
  capabilities: Object.freeze({ id: 'ResolveCapabilities', payload: Object.freeze(['request']) }),
  query: Object.freeze({ id: 'QueryItems', payload: Object.freeze(['request']) }),
  preview: Object.freeze({ id: 'GetPreview', payload: Object.freeze(['request']) }),
  preflight: Object.freeze({ id: 'PreflightImport', payload: Object.freeze(['request']) }),
  create: Object.freeze({ id: 'CreateImportIntent', payload: Object.freeze(['request']) }),
  execute: Object.freeze({ id: 'ExecuteImportIntent', payload: Object.freeze(['request']) }),
  get: Object.freeze({ id: 'GetImportIntent', payload: Object.freeze(['request']) }),
  reconcile: Object.freeze({ id: 'ReconcileImportIntent', payload: Object.freeze(['request']) })
});

const IMPORTAR_SERVICIO_WEB_E2E_ADAPTER = Object.freeze({
  id: 'importar-servicio-web', servicePath: SERVICE_PATH, operations,
  expectations: Object.freeze(['real-sii', 'no-duplicate-intent', 'no-duplicate-document', 'sanitized-evidence']),

  async executeRead({ invoke, taskId, budgetMs, profile }) {
    const latencies = [];
    if (profile.scenarioId === 'import-sii-recovery') {
      const get = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: profile.intentId }));
      const dto = assertResult(get, budgetMs, 'IMPORT_E2E_GET_FAILED');
      latencies.push(get.elapsedMs);
      const recoveredItems = assertSingleStoredDocument(dto);
      return Object.freeze({ codes: Object.freeze({ get: null }), count: recoveredItems.length, latenciesMs: Object.freeze(latencies) });
    }
    const capabilities = await invoke('ResolveCapabilities', request(base(taskId)));
    assertResult(capabilities, budgetMs, 'IMPORT_E2E_CAPABILITIES_FAILED'); latencies.push(capabilities.elapsedMs);
    const selection = await querySelection(invoke, taskId, profile.codigoBarras, profile.documentTypeId, profile.documentTypeName, budgetMs, latencies);
    const preview = await invoke('GetPreview', request({ ...base(taskId), ExternalKey: selection.ExternalKey }));
    assertResult(preview, budgetMs, 'IMPORT_E2E_PREVIEW_FAILED'); latencies.push(preview.elapsedMs);
    return Object.freeze({ codes: Object.freeze({ capabilities: null, query: null, preview: null }), count: 3, latenciesMs: Object.freeze(latencies) });
  },

  async executeExecution({ invoke, taskId, budgetMs, profile }) {
    const latencies = [];
    if (profile.scenarioId === 'import-sii-retry') {
      const before = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: profile.intentId }));
      const current = assertResult(before, budgetMs, 'IMPORT_E2E_RETRY_GET_FAILED'); latencies.push(before.elapsedMs);
      assertRetryableStorageFailure(current);
      const versionToken = field(current, ['VersionToken', 'versionToken']);
      if (typeof versionToken !== 'string' || !versionToken) fail('IMPORT_E2E_RETRY_VERSION_MISSING');
      const execute = await invoke('ExecuteImportIntent', request(executionPayload(taskId, { IntentId: profile.intentId, VersionToken: versionToken })));
      assertSingleStoredDocument(assertResult(execute, budgetMs, 'IMPORT_E2E_RETRY_EXECUTE_FAILED')); latencies.push(execute.elapsedMs);
      const after = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: profile.intentId }));
      assertSingleStoredDocument(assertResult(after, budgetMs, 'IMPORT_E2E_RETRY_CONFIRM_FAILED')); latencies.push(after.elapsedMs);
      return Object.freeze({ codes: Object.freeze({ retry: 'CONFIRMED', storedDocument: 'CONFIRMED' }), count: 1, latenciesMs: Object.freeze(latencies) });
    }
    const selection = await querySelection(invoke, taskId, profile.codigoBarras, profile.documentTypeId, profile.documentTypeName, budgetMs, latencies);
    const prepared = await preflight(invoke, taskId, selection, budgetMs, latencies);
    const create = await invoke('CreateImportIntent', request(createPayload(taskId, selection, prepared, profile.radicado, uuid())));
    const created = assertResult(create, budgetMs, 'IMPORT_E2E_CREATE_FAILED'); latencies.push(create.elapsedMs);
    const identity = intentIdentity(created);
    const execute = await invoke('ExecuteImportIntent', request(executionPayload(taskId, identity)));
    const executed = assertResult(execute, budgetMs, 'IMPORT_E2E_EXECUTE_FAILED'); latencies.push(execute.elapsedMs);
    assertSingleStoredDocument(executed);
    const finalIdentity = { IntentId: identity.IntentId, VersionToken: field(executed, ['VersionToken', 'versionToken']) || identity.VersionToken };
    const get = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: finalIdentity.IntentId }));
    assertSingleStoredDocument(assertResult(get, budgetMs, 'IMPORT_E2E_GET_FAILED')); latencies.push(get.elapsedMs);
    const reconcile = await invoke('ReconcileImportIntent', request({ ...base(taskId), IntentId: finalIdentity.IntentId, ExternalKey: selection.ExternalKey }));
    assertResult(reconcile, budgetMs, 'IMPORT_E2E_RECONCILE_FAILED'); latencies.push(reconcile.elapsedMs);
    return Object.freeze({ codes: Object.freeze({ create: null, execute: null, get: null, reconcile: null }), count: 1, latenciesMs: Object.freeze(latencies) });
  },

  async executeConcurrency({ invoke, concurrentInvoke, taskId, budgetMs, profile }) {
    const latencies = [];
    const selection = await querySelection(invoke, taskId, profile.codigoBarras, profile.documentTypeId, profile.documentTypeName, budgetMs, latencies);
    const prepared = await preflight(invoke, taskId, selection, budgetMs, latencies);
    const payload = createPayload(taskId, selection, prepared, profile.radicado, uuid());
    const create = await invoke('CreateImportIntent', request(payload));
    latencies.push(create.elapsedMs);
    const identity = intentIdentity(assertResult(create, budgetMs, 'IMPORT_E2E_CONCURRENT_CREATE_FAILED'));
    const executions = await Promise.all([
      invoke('ExecuteImportIntent', request(executionPayload(taskId, identity))),
      concurrentInvoke('ExecuteImportIntent', request(executionPayload(taskId, identity)))
    ]);
    executions.forEach((result) => latencies.push(result.elapsedMs));
    const accepted = executions.filter((result) => field(result.dto, ['Accepted', 'accepted']) === true);
    const blocked = executions.filter((result) => errorCode(result.dto));
    if (accepted.length !== 1 || blocked.length !== 1) fail('IMPORT_E2E_CONCURRENCY_RESULT_INVALID');
    const get = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: identity.IntentId }));
    assertSingleStoredDocument(assertResult(get, budgetMs, 'IMPORT_E2E_GET_FAILED')); latencies.push(get.elapsedMs);
    return Object.freeze({ codes: Object.freeze({ blocked: errorCode(blocked[0].dto), storedDocument: 'CONFIRMED' }), count: 2, latenciesMs: Object.freeze(latencies) });
  }
});

module.exports = { IMPORTAR_SERVICIO_WEB_E2E_ADAPTER };
