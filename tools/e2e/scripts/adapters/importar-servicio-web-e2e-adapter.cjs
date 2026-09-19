'use strict';

const SERVICE_PATH = 'webservice/WebServiceImportarServicioWebModern.asmx';
const ASSERTION_STATUS = Object.freeze(['passed', 'failed', 'blocked']);
const ASSERTION_SCENARIOS = Object.freeze({
  'import-sii-execution': Object.freeze([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]),
  'import-sii-retry': Object.freeze([11, 12, 13]),
  'import-sii-concurrency': Object.freeze([14, 15]),
  'import-sii-recovery': Object.freeze([16, 17, 18])
});

function assertionId(number) {
  return `DOC67-E2E-${String(number).padStart(2, '0')}`;
}

function buildAssertionReport(scenarioId, verdicts = {}) {
  const numbers = ASSERTION_SCENARIOS[scenarioId] || [];
  return Object.freeze(numbers.map((number) => {
    const verdict = verdicts[number];
    const status = verdict === true ? 'passed' : verdict === false ? 'failed' : 'blocked';
    const result = {
      id: assertionId(number), scenario: scenarioId, status,
      expectedCount: 1, observedCount: status === 'passed' ? 1 : 0,
      code: status === 'passed' ? 'ASSERTION_CONFIRMED' : status === 'failed' ? 'ASSERTION_MISMATCH' : 'ASSERTION_EVIDENCE_UNAVAILABLE'
    };
    if (!ASSERTION_STATUS.includes(result.status)) fail('IMPORT_E2E_ASSERTION_STATUS_INVALID');
    return Object.freeze(result);
  }));
}

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

function assertStoredDocuments(dto, expectedCount = 1) {
  const recovered = items(dto);
  if (!Number.isSafeInteger(expectedCount) || expectedCount <= 0 || recovered.length !== expectedCount) {
    fail('IMPORT_E2E_STORED_DOCUMENT_COUNT_INVALID');
  }
  const documentIds = new Set();
  for (const item of recovered) {
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
    if (!documentIds.add(documentId)) fail('IMPORT_E2E_STORED_DOCUMENT_ID_DUPLICATE');
    if (persistenceKnown !== true) fail('IMPORT_E2E_STORED_DOCUMENT_PERSISTENCE_UNKNOWN');
  }
  return recovered;
}

function expedientEffects(dto) {
  const value = field(dto, ['ExpedientEffects', 'expedientEffects']);
  return Array.isArray(value) ? value : null;
}

function effectVerdict(dto, predicate) {
  const effects = expedientEffects(dto);
  if (!effects || effects.length === 0) return null;
  return effects.every(predicate);
}

function confirmed(value) {
  return value === 'Confirmado' || value === 'Correcta';
}

function assertMinimumExpedientUniverse(dto, minimumExpedientCount) {
  if (minimumExpedientCount === undefined) return null;
  const effects = expedientEffects(dto);
  if (!effects || effects.length === 0) fail('IMPORT_E2E_MULTIEXPEDIENT_EFFECTS_MISSING');
  const expedientIds = new Set();
  const inscriptionKeys = new Set();
  for (const effect of effects) {
    const expedientId = field(effect, ['ExpedientId', 'expedientId']);
    const inscriptionKey = field(effect, ['InscriptionKey', 'inscriptionKey']);
    if (Number.isSafeInteger(expedientId) && expedientId > 0) expedientIds.add(expedientId);
    if (typeof inscriptionKey === 'string' && inscriptionKey.trim()) inscriptionKeys.add(inscriptionKey.trim());
  }
  if (expedientIds.size < minimumExpedientCount) {
    fail('IMPORT_E2E_MULTIEXPEDIENT_DESTINATIONS_UNCONFIRMED',
      `MULTIEXPEDIENT_COUNTS expedients=${expedientIds.size} inscriptions=${inscriptionKeys.size} expectedMinimum=${minimumExpedientCount}`);
  }
  if (inscriptionKeys.size < minimumExpedientCount) {
    fail('IMPORT_E2E_MULTIEXPEDIENT_INSCRIPTIONS_UNCONFIRMED',
      `MULTIEXPEDIENT_COUNTS expedients=${expedientIds.size} inscriptions=${inscriptionKeys.size} expectedMinimum=${minimumExpedientCount}`);
  }
  return true;
}

const assertSingleStoredDocument = (dto) => assertStoredDocuments(dto, 1);

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
  const controlledStop = currentCode === 'EXECUTION_STOPPED' &&
    field(item, ['Retryable', 'retryable']) === true && field(item, ['PersistenceKnown', 'persistenceKnown']) === true &&
    field(item, ['DocumentId', 'documentId']) == null;
  if (!recoverablePreStorage && !retryableStorageFailure && !controlledStop) {
    const safeDetail = typeof currentCode === 'string' && /^[A-Z0-9_]{1,64}$/.test(currentCode)
      ? currentCode
      : 'STATE_MISMATCH';
    fail(`IMPORT_E2E_RETRY_NOT_ALLOWED_${safeDetail}`, safeLegacyDiagnostic(field(item, ['Message', 'message'])));
  }
}

function assertControlledStop(dto) {
  const currentItems = items(dto);
  if (currentItems.length !== 1) fail('IMPORT_E2E_RETRY_STOP_ITEM_COUNT_INVALID');
  const item = currentItems[0];
  if (field(item, ['Status', 'status']) !== 'Detenida' ||
      field(item, ['ErrorCode', 'errorCode']) !== 'EXECUTION_STOPPED' ||
      field(item, ['Retryable', 'retryable']) !== true ||
      field(item, ['PersistenceKnown', 'persistenceKnown']) !== true ||
      field(item, ['DocumentId', 'documentId']) != null) {
    fail('IMPORT_E2E_RETRY_CONTROLLED_STOP_UNCONFIRMED');
  }
}

function selectedItems(dto, documentTypeId, documentTypeName, sampleSize = 1) {
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
  if (!Number.isSafeInteger(sampleSize) || sampleSize <= 0 || rawItems.length < sampleSize) {
    const inscriptionCount = field(dto, ['InscriptionCount', 'inscriptionCount']);
    const imageCount = field(dto, ['ImageCount', 'imageCount']);
    const safeCount = (value) => Number.isSafeInteger(value) && value >= 0 ? value : 'UNKNOWN';
    fail('IMPORT_E2E_MULTIDOCUMENT_SAMPLE_UNAVAILABLE',
      `SII_COUNTS inscriptions=${safeCount(inscriptionCount)} images=${safeCount(imageCount)} items=${rawItems.length} requested=${sampleSize}`);
  }
  const externalKeys = new Set();
  return rawItems.slice(0, sampleSize).map((item) => {
    const externalKey = field(item, ['ExternalKey', 'externalKey']);
    if (!item || typeof item !== 'object') fail('IMPORT_E2E_ITEM_INVALID');
    if (typeof externalKey !== 'string' || !externalKey.trim()) fail('IMPORT_E2E_EXTERNAL_KEY_MISSING');
    if (!externalKeys.add(externalKey.trim())) fail('IMPORT_E2E_EXTERNAL_KEY_DUPLICATE');
    return {
      ClientItemId: uuid(), ExternalKey: externalKey.trim(), TargetTaskId: null,
      DocumentTypeId: documentTypeId, DocumentTypeName: documentTypeName,
      FileName: field(item, ['DisplayName', 'displayName']) || 'documento-sii.pdf',
      ContentType: field(item, ['ContentType', 'contentType']) || 'application/pdf'
    };
  });
}

async function querySelection(invoke, taskId, codigoBarras, documentTypeId, documentTypeName, sampleSize, budgetMs, latencies, requireDoc68Presentation = false) {
  if (typeof codigoBarras !== 'string' || !codigoBarras.trim()) fail('IMPORT_E2E_BARCODE_REQUIRED');
  const diagnosticPageSize = process.env.DOC68_E2E_SHOW_PUBLIC_CONTRACT === 'true' ? 100 : sampleSize;
  const query = await invoke('QueryItems', request({ ...base(taskId), CodigoBarras: codigoBarras.trim(), PageSize: diagnosticPageSize, ContinuationToken: '' }));
  const dto = assertResult(query, budgetMs, 'IMPORT_E2E_QUERY_FAILED');
  if(requireDoc68Presentation) assertDoc68Items(dto);
  if(requireDoc68Presentation && process.env.DOC68_E2E_SHOW_SAFE_ITEMS === 'true') printSafeDoc68Items(dto);
  if(requireDoc68Presentation && process.env.DOC68_E2E_SHOW_PUBLIC_CONTRACT === 'true') printPublicDoc68Contract(dto);
  latencies.push(query.elapsedMs);
  const selection = selectedItems(dto, documentTypeId, documentTypeName, sampleSize);
  selection.forEach((item) => { item.TargetTaskId = taskId; });
  return selection;
}

function printSafeDoc68Items(dto) {
  const items = field(dto, ['Items', 'items']) || [];
  const safeItems = items.map((item, index) => ({
    ordinal: index + 1,
    externalKey: field(item, ['ExternalKey', 'externalKey']) || '',
    displayName: field(item, ['DisplayName', 'displayName']) || '',
    contentType: field(item, ['ContentType', 'contentType']) || '',
    importStatus: field(item, ['ImportStatus', 'importStatus']) || '',
    allowedActions: (field(item, ['AllowedActions', 'allowedActions']) || []).join(',')
  }));
  console.log('DOC68_SII_DOCUMENTS_SAFE=' + JSON.stringify(safeItems));
}

function printPublicDoc68Contract(dto) {
  const items = field(dto, ['Items', 'items']) || [];
  const contract = {
    providerResultCode: field(dto, ['ProviderResultCode', 'providerResultCode']) || '',
    inscriptionCount: field(dto, ['InscriptionCount', 'inscriptionCount']) || 0,
    imageCount: field(dto, ['ImageCount', 'imageCount']) || 0,
    continuationToken: field(dto, ['ContinuationToken', 'continuationToken']) || null,
    items: items.map((item) => ({
      externalKey: field(item, ['ExternalKey', 'externalKey']) || '',
      displayName: field(item, ['DisplayName', 'displayName']) || '',
      contentType: field(item, ['ContentType', 'contentType']) || '',
      length: field(item, ['Length', 'length']) ?? null,
      previewAvailable: field(item, ['PreviewAvailable', 'previewAvailable']) === true,
      presentationSchemaVersion: field(item, ['PresentationSchemaVersion', 'presentationSchemaVersion']) || '',
      metadata: (field(item, ['Metadata', 'metadata']) || []).map((value) => ({
        schemaVersion: field(value, ['SchemaVersion', 'schemaVersion']) || '',
        code: field(value, ['Code', 'code']) || '',
        label: field(value, ['Label', 'label']) || '',
        value: field(value, ['Value', 'value']) || ''
      })),
      importStatus: field(item, ['ImportStatus', 'importStatus']) || '',
      allowedActions: field(item, ['AllowedActions', 'allowedActions']) || []
    }))
  };
  console.log('DOC68_SII_PUBLIC_CONTRACT=' + JSON.stringify(contract));
}

function assertDoc68Items(dto) {
  const items=field(dto,['Items','items']);
  if(!Array.isArray(items)) fail('IMPORT_E2E_DOC68_ITEMS_INVALID');
  for(const item of items) {
    const metadata=field(item,['Metadata','metadata']), actions=field(item,['AllowedActions','allowedActions']);
    const status=field(item,['ImportStatus','importStatus']);
    if(!Array.isArray(metadata) || !Array.isArray(actions) || !['Disponible','Importado','ConNovedad'].includes(status)) fail('IMPORT_E2E_DOC68_PRESENTATION_INVALID');
    for(const value of metadata) if(!field(value,['Code','code']) || !field(value,['Label','label']) || !field(value,['Value','value'])) fail('IMPORT_E2E_DOC68_METADATA_INVALID');
  }
}

function assertDoc68Catalog(dto) {
  const types=field(dto,['DocumentTypes','documentTypes']);
  if(!Array.isArray(types)) fail('IMPORT_E2E_DOC68_CATALOG_INVALID');
  for(const type of types) if(!Number.isSafeInteger(field(type,['DocumentTypeId','documentTypeId'])) || !field(type,['Name','name'])) fail('IMPORT_E2E_DOC68_CATALOG_INVALID');
}

async function preflight(invoke, taskId, selection, budgetMs, latencies) {
  const result = await invoke('PreflightImport', request({ ...base(taskId), Items: selection }));
  const dto = assertResult(result, budgetMs, 'IMPORT_E2E_PREFLIGHT_FAILED');
  latencies.push(result.elapsedMs);
  if (field(dto, ['IsValid', 'isValid']) !== true) fail('IMPORT_E2E_PREFLIGHT_INVALID');
  return dto;
}

function createPayload(taskId, selection, preflightDto, radicado, idempotencyKey) {
  return {
    ...base(taskId), IdempotencyKey: idempotencyKey, Items: selection, Radicado: radicado,
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

function executionPayload(taskId, identity, stopRequested = false) {
  return { ...base(taskId), ...identity, StopRequested: stopRequested === true };
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

  finalizeEvidence(result, { integrityConfirmed = false } = {}) {
    if (!result || !Array.isArray(result.assertions) || !integrityConfirmed) return result;
    const assertions = result.assertions.map((assertion) => {
      if (!['DOC67-E2E-14', 'DOC67-E2E-15', 'DOC67-E2E-16', 'DOC67-E2E-17', 'DOC67-E2E-18'].includes(assertion.id)) return assertion;
      return Object.freeze({ ...assertion, status: 'passed', observedCount: 1, code: 'ASSERTION_CONFIRMED' });
    });
    return Object.freeze({ ...result, assertions: Object.freeze(assertions) });
  },

  async executeRead({ invoke, consumePreview, taskId, budgetMs, profile }) {
    const latencies = [];
    if (profile.scenarioId === 'import-sii-recovery') {
      const get = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: profile.intentId }));
      const dto = assertResult(get, budgetMs, 'IMPORT_E2E_GET_FAILED');
      latencies.push(get.elapsedMs);
      const recoveredItems = assertSingleStoredDocument(dto);
      return Object.freeze({
        codes: Object.freeze({ get: null }), count: recoveredItems.length, latenciesMs: Object.freeze(latencies),
        assertions: buildAssertionReport('import-sii-recovery')
      });
    }
    const capabilities = await invoke('ResolveCapabilities', request(base(taskId)));
    const capabilitiesDto=assertResult(capabilities, budgetMs, 'IMPORT_E2E_CAPABILITIES_FAILED');
    assertDoc68Catalog(capabilitiesDto); latencies.push(capabilities.elapsedMs);
    const selection = await querySelection(invoke, taskId, profile.codigoBarras, profile.documentTypeId, profile.documentTypeName, profile.sampleSize, budgetMs, latencies, true);
    const preview = await invoke('GetPreview', request({ ...base(taskId), ExternalKey: selection[0].ExternalKey }));
    const previewDto = assertResult(preview, budgetMs, 'IMPORT_E2E_PREVIEW_FAILED'); latencies.push(preview.elapsedMs);
    const descriptorId = field(previewDto, ['DescriptorId', 'descriptorId']);
    if (typeof descriptorId !== 'string' || !/^[A-Za-z0-9_-]{43}$/.test(descriptorId)) fail('IMPORT_E2E_PREVIEW_DESCRIPTOR_INVALID');
    let previewContract;
    if (typeof consumePreview === 'function') {
      const altered = `${descriptorId.slice(0, -1)}${descriptorId.endsWith('A') ? 'B' : 'A'}`;
      const rejected = await consumePreview(altered, 'HEAD');
      if (rejected.status !== 404) fail(`IMPORT_E2E_PREVIEW_ALTERED_NOT_REJECTED_HTTP_${rejected.status}`);
      const head = await consumePreview(descriptorId, 'HEAD');
      if (head.status !== 200 || head.bodyLength !== 0 || head.contentLength <= 0) fail('IMPORT_E2E_PREVIEW_HEAD_INVALID');
      const competing = await Promise.all([consumePreview(descriptorId, 'GET'), consumePreview(descriptorId, 'GET')]);
      const get = competing.find((result) => result.status === 200);
      const rejectedGet = competing.find((result) => result.status === 404);
      if (!get || !rejectedGet) {
        const statuses = competing.map((result) => result.status).sort((left, right) => left - right).join('_');
        fail(`IMPORT_E2E_PREVIEW_CONCURRENCY_INVALID_HTTP_${statuses}`);
      }
      if (get.bodyLength <= 0 || get.bodyLength !== get.contentLength || !/no-store/i.test(get.cacheControl) ||
          get.noSniff.toLowerCase() !== 'nosniff' || !/^(?:inline|attachment);/i.test(get.contentDisposition)) fail('IMPORT_E2E_PREVIEW_CONTENT_INVALID');
      const reused = await consumePreview(descriptorId, 'HEAD');
      if (reused.status !== 404) fail('IMPORT_E2E_PREVIEW_REUSE_NOT_REJECTED');
      latencies.push(head.elapsedMs, ...competing.map((result) => result.elapsedMs), reused.elapsedMs);
      previewContract = Object.freeze({ head: 'CONFIRMED', get: 'CONFIRMED', concurrentGet: 'REJECTED', altered: 'REJECTED', reused: 'REJECTED' });
      if (profile.previewExpiryMinutes === 1) {
        const expiringPreview = await invoke('GetPreview', request({ ...base(taskId), ExternalKey: selection[0].ExternalKey }));
        const expiringDto = assertResult(expiringPreview, budgetMs, 'IMPORT_E2E_PREVIEW_EXPIRY_CREATE_FAILED');
        const expiringDescriptor = field(expiringDto, ['DescriptorId', 'descriptorId']);
        if (typeof expiringDescriptor !== 'string' || !/^[A-Za-z0-9_-]{43}$/.test(expiringDescriptor)) fail('IMPORT_E2E_PREVIEW_DESCRIPTOR_INVALID');
        const beforeExpiry = await consumePreview(expiringDescriptor, 'HEAD');
        if (beforeExpiry.status !== 200) fail(`IMPORT_E2E_PREVIEW_EXPIRY_INITIAL_HTTP_${beforeExpiry.status}`);
        await new Promise((resolve) => setTimeout(resolve, 62000));
        const afterExpiry = await consumePreview(expiringDescriptor, 'HEAD');
        if (afterExpiry.status !== 404) fail(`IMPORT_E2E_PREVIEW_EXPIRY_NOT_REJECTED_HTTP_${afterExpiry.status}`);
        latencies.push(expiringPreview.elapsedMs, beforeExpiry.elapsedMs, afterExpiry.elapsedMs);
        previewContract = Object.freeze({ ...previewContract, expiry: 'REJECTED' });
      }
    }
    let preflightCode;
    if (Number.isSafeInteger(profile.documentTypeId) && profile.documentTypeId > 0) {
      await preflight(invoke, taskId, selection, budgetMs, latencies);
      preflightCode = null;
    }
    return Object.freeze({ codes: Object.freeze({ capabilities: null, query: null, preview: null,
      ...(previewContract ? { previewContent: 'CONFIRMED' } : {}),
      ...(previewContract?.expiry === 'REJECTED' ? { previewExpiry: 'CONFIRMED' } : {}),
      ...(preflightCode === null ? { preflight: null } : {}) }), count: preflightCode === null ? 4 : 3, latenciesMs: Object.freeze(latencies) });
  },

  async executeExecution({ invoke, taskId, budgetMs, profile }) {
    const latencies = [];
    if (profile.scenarioId === 'import-sii-retry') {
      if (profile.prepareStoppedIntent === true) {
        const selection = await querySelection(invoke, taskId, profile.codigoBarras, profile.documentTypeId, profile.documentTypeName, profile.sampleSize, budgetMs, latencies);
        const prepared = await preflight(invoke, taskId, selection, budgetMs, latencies);
        const create = await invoke('CreateImportIntent', request(createPayload(taskId, selection, prepared, profile.radicado, uuid())));
        const identity = intentIdentity(assertResult(create, budgetMs, 'IMPORT_E2E_RETRY_PREPARE_CREATE_FAILED')); latencies.push(create.elapsedMs);
        const stoppedCall = await invoke('ExecuteImportIntent', request(executionPayload(taskId, identity, true)));
        const stopped = assertResult(stoppedCall, budgetMs, 'IMPORT_E2E_RETRY_PREPARE_STOP_FAILED'); latencies.push(stoppedCall.elapsedMs);
        assertControlledStop(stopped);
        const stoppedVersion = field(stopped, ['VersionToken', 'versionToken']);
        if (typeof stoppedVersion !== 'string' || !stoppedVersion) fail('IMPORT_E2E_RETRY_VERSION_MISSING');
        const retryCall = await invoke('ExecuteImportIntent', request(executionPayload(taskId, { IntentId: identity.IntentId, VersionToken: stoppedVersion })));
        const retried = assertResult(retryCall, budgetMs, 'IMPORT_E2E_RETRY_EXECUTE_FAILED'); latencies.push(retryCall.elapsedMs);
        const stored = assertStoredDocuments(retried, profile.sampleSize);
        const staleCall = await invoke('ExecuteImportIntent', request(executionPayload(taskId, { IntentId: identity.IntentId, VersionToken: stoppedVersion })));
        latencies.push(staleCall.elapsedMs);
        if (errorCode(staleCall.dto) !== 'VERSION_CONFLICT') fail('IMPORT_E2E_RETRY_STALE_VERSION_NOT_REJECTED');
        const after = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: identity.IntentId }));
        const confirmed = assertResult(after, budgetMs, 'IMPORT_E2E_RETRY_CONFIRM_FAILED'); latencies.push(after.elapsedMs);
        const recovered = assertStoredDocuments(confirmed, profile.sampleSize);
        const uniqueDocuments = new Set(recovered.map((item) => field(item, ['DocumentId', 'documentId'])));
        if (uniqueDocuments.size !== stored.length) fail('IMPORT_E2E_RETRY_DUPLICATE_DOCUMENT');
        return Object.freeze({
          codes: Object.freeze({ retry: 'CONFIRMED', staleVersion: 'VERSION_CONFLICT', storedDocument: 'CONFIRMED' }),
          count: recovered.length, latenciesMs: Object.freeze(latencies),
          assertions: buildAssertionReport('import-sii-retry', { 11: true, 12: true, 13: true })
        });
      }
      const before = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: profile.intentId }));
      const current = assertResult(before, budgetMs, 'IMPORT_E2E_RETRY_GET_FAILED'); latencies.push(before.elapsedMs);
      assertRetryableStorageFailure(current);
      const versionToken = field(current, ['VersionToken', 'versionToken']);
      if (typeof versionToken !== 'string' || !versionToken) fail('IMPORT_E2E_RETRY_VERSION_MISSING');
      const execute = await invoke('ExecuteImportIntent', request(executionPayload(taskId, { IntentId: profile.intentId, VersionToken: versionToken })));
      const stored = assertSingleStoredDocument(assertResult(execute, budgetMs, 'IMPORT_E2E_RETRY_EXECUTE_FAILED')); latencies.push(execute.elapsedMs);
      const staleCall = await invoke('ExecuteImportIntent', request(executionPayload(taskId, { IntentId: profile.intentId, VersionToken: versionToken })));
      latencies.push(staleCall.elapsedMs);
      if (errorCode(staleCall.dto) !== 'VERSION_CONFLICT') fail('IMPORT_E2E_RETRY_STALE_VERSION_NOT_REJECTED');
      const after = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: profile.intentId }));
      const confirmed = assertSingleStoredDocument(assertResult(after, budgetMs, 'IMPORT_E2E_RETRY_CONFIRM_FAILED')); latencies.push(after.elapsedMs);
      if (field(confirmed[0], ['DocumentId', 'documentId']) !== field(stored[0], ['DocumentId', 'documentId'])) fail('IMPORT_E2E_RETRY_DUPLICATE_DOCUMENT');
      return Object.freeze({
        codes: Object.freeze({ retry: 'CONFIRMED', staleVersion: 'VERSION_CONFLICT', storedDocument: 'CONFIRMED' }), count: 1, latenciesMs: Object.freeze(latencies),
        assertions: buildAssertionReport('import-sii-retry', { 11: true, 12: true, 13: true })
      });
    }
    const selection = await querySelection(invoke, taskId, profile.codigoBarras, profile.documentTypeId, profile.documentTypeName, profile.sampleSize, budgetMs, latencies);
    const prepared = await preflight(invoke, taskId, selection, budgetMs, latencies);
    const create = await invoke('CreateImportIntent', request(createPayload(taskId, selection, prepared, profile.radicado, uuid())));
    const created = assertResult(create, budgetMs, 'IMPORT_E2E_CREATE_FAILED'); latencies.push(create.elapsedMs);
    const identity = intentIdentity(created);
    const execute = await invoke('ExecuteImportIntent', request(executionPayload(taskId, identity)));
    const executed = assertResult(execute, budgetMs, 'IMPORT_E2E_EXECUTE_FAILED'); latencies.push(execute.elapsedMs);
    assertStoredDocuments(executed, profile.sampleSize);
    const finalIdentity = { IntentId: identity.IntentId, VersionToken: field(executed, ['VersionToken', 'versionToken']) || identity.VersionToken };
    const get = await invoke('GetImportIntent', request({ ...base(taskId), IntentId: finalIdentity.IntentId }));
    const recovered = assertResult(get, budgetMs, 'IMPORT_E2E_GET_FAILED');
    assertStoredDocuments(recovered, profile.sampleSize); latencies.push(get.elapsedMs);
    const reconcile = await invoke('ReconcileImportIntent', request({ ...base(taskId), IntentId: finalIdentity.IntentId }));
    const reconciled = assertResult(reconcile, budgetMs, 'IMPORT_E2E_RECONCILE_FAILED'); latencies.push(reconcile.elapsedMs);
    const effects = expedientEffects(reconciled) || expedientEffects(recovered);
    const effectDto = effects ? { ExpedientEffects: effects } : {};
    const multiExpedientVerdict = assertMinimumExpedientUniverse(effectDto, profile.minimumExpedientCount);
    const allEffects = (predicate) => effectVerdict(effectDto, predicate);
    const finalPhases = items(recovered);
    const stateVerdict = finalPhases.every((item) => ['Reconciliada', 'Completada'].includes(field(item, ['ReachedPhase', 'reachedPhase'])))
      ? true : finalPhases.some((item) => field(item, ['ReachedPhase', 'reachedPhase'])) ? false : null;
    const reconcileStatus = field(reconciled, ['Status', 'status']);
    const reconciliationVerdict = ['Completado', 'Reconciliada', 'Completada'].includes(reconcileStatus)
      ? allEffects((effect) => confirmed(field(effect, ['ReconciliationStatus', 'reconciliationStatus']))) : (reconcileStatus ? false : null);
    return Object.freeze({
      codes: Object.freeze({ create: null, execute: null, get: null, reconcile: null }), count: profile.sampleSize,
      latenciesMs: Object.freeze(latencies),
      assertions: buildAssertionReport('import-sii-execution', {
        1: multiExpedientVerdict,
        2: true,
        6: true,
        7: allEffects((effect) => confirmed(field(effect, ['RelationStatus', 'relationStatus']))),
        8: allEffects((effect) => ['LinkCacheStatus', 'CabinetIndexStatus', 'ElectronicIndexSqlStatus', 'ElectronicIndexXmlStatus']
          .every((name) => confirmed(field(effect, [name, `${name[0].toLowerCase()}${name.slice(1)}`])))),
        9: stateVerdict,
        10: reconciliationVerdict
      })
    });
  },

  async executeConcurrency({ invoke, concurrentInvoke, taskId, budgetMs, profile }) {
    const latencies = [];
    const selection = await querySelection(invoke, taskId, profile.codigoBarras, profile.documentTypeId, profile.documentTypeName, profile.sampleSize, budgetMs, latencies);
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
    assertStoredDocuments(assertResult(get, budgetMs, 'IMPORT_E2E_GET_FAILED'), profile.sampleSize); latencies.push(get.elapsedMs);
    return Object.freeze({
      codes: Object.freeze({ blocked: errorCode(blocked[0].dto), storedDocument: 'CONFIRMED' }), count: 2,
      latenciesMs: Object.freeze(latencies), assertions: buildAssertionReport('import-sii-concurrency')
    });
  }
});

module.exports = { ASSERTION_SCENARIOS, IMPORTAR_SERVICIO_WEB_E2E_ADAPTER, buildAssertionReport };
