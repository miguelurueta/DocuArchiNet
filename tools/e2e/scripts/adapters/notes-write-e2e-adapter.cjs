'use strict';

const SAFE_SERVICE_PATH = 'webservice/WebServiceWorkflowNotesModern.asmx';
const SHA256 = /^[a-f0-9]{64}$/i;

function fail(code) {
  const error = new Error(`El adaptador transaccional de Notas no pudo completar la etapa (${code}).`);
  error.code = code;
  throw error;
}

function field(value, names) {
  if (!value || typeof value !== 'object') return undefined;
  for (const name of names) {
    if (Object.hasOwn(value, name)) return value[name];
  }
  return undefined;
}

function functionalCode(dto) {
  return field(dto, ['CodigoBloqueo', 'codigoBloqueo']) ||
    field(field(dto, ['Error', 'error']), ['Codigo', 'codigo']) || null;
}

function isSuccessful(dto) {
  return field(dto, ['Exito', 'exito', 'Success', 'success']) === true;
}

function noteFrom(dto) {
  return field(dto, ['Nota', 'nota']) || dto;
}

function noteId(dto) {
  const parsed = Number(field(noteFrom(dto), ['IdNota', 'idNota', 'ID_ANOTACION', 'idAnotacion']));
  if (!Number.isSafeInteger(parsed) || parsed <= 0) fail('NOTES_WRITE_NOTE_ID_INVALID');
  return parsed;
}

function noteVersion(dto) {
  const value = field(noteFrom(dto), ['Version', 'version']);
  if (typeof value !== 'string' || !SHA256.test(value)) fail('NOTES_WRITE_VERSION_INVALID');
  return value;
}

function assertWithinBudget(result, budgetMs, code) {
  if (!result || !Number.isSafeInteger(result.elapsedMs) || result.elapsedMs < 0 || result.elapsedMs > budgetMs) fail(code);
}

function assertSuccessful(result, budgetMs, code) {
  assertWithinBudget(result, budgetMs, `${code}_BUDGET`);
  if (!isSuccessful(result.dto) || functionalCode(result.dto)) fail(code);
}

function assertBlocked(result, budgetMs, code) {
  assertWithinBudget(result, budgetMs, `${code}_BUDGET`);
  if (isSuccessful(result.dto) || !functionalCode(result.dto)) fail(code);
}

function correlationId() {
  const value = globalThis.crypto?.randomUUID?.();
  if (typeof value !== 'string' || !/^[a-f0-9-]{36}$/i.test(value)) fail('NOTES_WRITE_CORRELATION_UNAVAILABLE');
  return value;
}

function requireInputs({ invoke, taskId, budgetMs }) {
  if (typeof invoke !== 'function' || !Number.isSafeInteger(taskId) || taskId <= 0 || !Number.isSafeInteger(budgetMs) || budgetMs <= 0) {
    fail('NOTES_WRITE_INPUT_INVALID');
  }
}

function writePayload(taskId, content) {
  return { idTarea: taskId, contenido: content, clientRequestId: correlationId() };
}

const NOTES_WRITE_E2E_ADAPTER = Object.freeze({
  id: 'notes-write',
  servicePath: SAFE_SERVICE_PATH,
  operations: Object.freeze({
    create: Object.freeze({ id: 'CrearNota', payload: Object.freeze(['idTarea', 'contenido', 'clientRequestId']) }),
    get: Object.freeze({ id: 'ConsultarNota', payload: Object.freeze(['idTarea', 'idNota']) }),
    update: Object.freeze({ id: 'ActualizarNota', payload: Object.freeze(['idTarea', 'idNota', 'contenido', 'version']) }),
    remove: Object.freeze({ id: 'EliminarNota', payload: Object.freeze(['idTarea', 'idNota', 'version']) })
  }),
  expectations: Object.freeze(['state-change', 'audit-change', 'idempotent-create', 'version-conflict', 'sanitized-evidence']),
  async executeExecution({ invoke, taskId, budgetMs }) {
    requireInputs({ invoke, taskId, budgetMs });
    const initialContent = 'Prueba temporal E2E de Notas';
    const updatedContent = 'Prueba temporal E2E de Notas actualizada';
    const createPayload = writePayload(taskId, initialContent);
    const created = await invoke('CrearNota', createPayload);
    assertSuccessful(created, budgetMs, 'NOTES_WRITE_CREATE_FAILED');
    const idNota = noteId(created.dto);
    const versionInicial = noteVersion(created.dto);

    const retry = await invoke('CrearNota', createPayload);
    assertSuccessful(retry, budgetMs, 'NOTES_WRITE_IDEMPOTENCY_FAILED');
    if (noteId(retry.dto) !== idNota || noteVersion(retry.dto) !== versionInicial) fail('NOTES_WRITE_IDEMPOTENCY_FAILED');

    const updated = await invoke('ActualizarNota', { idTarea: taskId, idNota, contenido: updatedContent, version: versionInicial });
    assertSuccessful(updated, budgetMs, 'NOTES_WRITE_UPDATE_FAILED');
    const versionActualizada = noteVersion(updated.dto);

    const stale = await invoke('ActualizarNota', { idTarea: taskId, idNota, contenido: initialContent, version: versionInicial });
    assertBlocked(stale, budgetMs, 'NOTES_WRITE_CONFLICT_EXPECTED');

    const removed = await invoke('EliminarNota', { idTarea: taskId, idNota, version: versionActualizada });
    assertSuccessful(removed, budgetMs, 'NOTES_WRITE_DELETE_FAILED');

    return Object.freeze({
      codes: Object.freeze({
        create: functionalCode(created.dto),
        retry: functionalCode(retry.dto),
        update: functionalCode(updated.dto),
        conflict: functionalCode(stale.dto),
        remove: functionalCode(removed.dto)
      }),
      count: 5,
      latenciesMs: Object.freeze([created.elapsedMs, retry.elapsedMs, updated.elapsedMs, stale.elapsedMs, removed.elapsedMs])
    });
  },
  async executeConcurrency({ invoke, concurrentInvoke, taskId, noteId: seedNoteId, budgetMs }) {
    requireInputs({ invoke, taskId, budgetMs });
    if (typeof concurrentInvoke !== 'function' || !Number.isSafeInteger(seedNoteId) || seedNoteId <= 0) fail('NOTES_CONCURRENCY_INPUT_INVALID');
    const current = await invoke('ConsultarNota', { idTarea: taskId, idNota: seedNoteId });
    assertSuccessful(current, budgetMs, 'NOTES_CONCURRENCY_NOTE_UNAVAILABLE');
    const versionInicial = noteVersion(current.dto);
    const results = await Promise.all([
      invoke('ActualizarNota', { idTarea: taskId, idNota: seedNoteId, contenido: 'Actualización concurrente uno', version: versionInicial }),
      concurrentInvoke('ActualizarNota', { idTarea: taskId, idNota: seedNoteId, contenido: 'Actualización concurrente dos', version: versionInicial })
    ]);
    const successful = results.filter((result) => isSuccessful(result.dto));
    const blocked = results.filter((result) => !isSuccessful(result.dto));
    if (successful.length !== 1 || blocked.length !== 1) fail('NOTES_CONCURRENCY_RESULT_INVALID');
    assertSuccessful(successful[0], budgetMs, 'NOTES_CONCURRENCY_SUCCESS_INVALID');
    assertBlocked(blocked[0], budgetMs, 'NOTES_CONCURRENCY_BLOCK_INVALID');

    return Object.freeze({
      codes: Object.freeze({
        blocked: functionalCode(blocked[0].dto),
        current: functionalCode(current.dto)
      }),
      count: 2,
      latenciesMs: Object.freeze([current.elapsedMs, results[0].elapsedMs, results[1].elapsedMs])
    });
  }
});

module.exports = { NOTES_WRITE_E2E_ADAPTER };
