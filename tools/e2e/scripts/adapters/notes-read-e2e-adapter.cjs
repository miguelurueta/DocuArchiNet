'use strict';

const SAFE_SERVICE_PATH = 'webservice/WebServiceWorkflowNotesModern.asmx';

function fail(code) {
  const error = new Error(`El adaptador de Notas no pudo completar la lectura (${code}).`);
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

function notesFrom(dto) {
  const notes = field(dto, ['Notas', 'notas', 'Items', 'items']);
  return Array.isArray(notes) ? notes : [];
}

function noteId(note) {
  const parsed = Number(field(note, ['IdNota', 'idNota', 'ID_ANOTACION', 'idAnotacion']));
  if (!Number.isSafeInteger(parsed) || parsed <= 0) fail('NOTES_READ_NOTE_ID_INVALID');
  return parsed;
}

function isBlocked(dto) {
  return field(dto, ['Exito', 'exito', 'Success', 'success']) !== true && Boolean(functionalCode(dto));
}

function assertWithinBudget(result, budgetMs, code) {
  if (!result || typeof result !== 'object' || !Number.isSafeInteger(result.elapsedMs) || result.elapsedMs < 0 || result.elapsedMs > budgetMs) {
    fail(code);
  }
}

const NOTES_READ_E2E_ADAPTER = Object.freeze({
  id: 'notes-read',
  servicePath: SAFE_SERVICE_PATH,
  operations: Object.freeze({
    list: Object.freeze({ id: 'ListarNotas', payload: Object.freeze(['idTarea', 'cursor', 'tamanoPagina']) }),
    get: Object.freeze({ id: 'ConsultarNota', payload: Object.freeze(['idTarea', 'idNota']) }),
    invalidCursor: Object.freeze({ id: 'ListarNotas', payload: Object.freeze(['idTarea', 'cursor', 'tamanoPagina']) })
  }),
  expectations: Object.freeze(['no-state-change', 'no-audit-change', 'sanitized-evidence']),
  async executeAnonymous({ invoke, budgetMs }) {
    if (typeof invoke !== 'function' || !Number.isSafeInteger(budgetMs) || budgetMs <= 0) fail('NOTES_ANONYMOUS_INPUT_INVALID');
    const result = await invoke('ListarNotas', { idTarea: 1, cursor: '', tamanoPagina: 1 });
    assertWithinBudget(result, budgetMs, 'NOTES_ANONYMOUS_BUDGET');
    if (!isBlocked(result.dto) || notesFrom(result.dto).length !== 0) fail('NOTES_ANONYMOUS_NOT_BLOCKED');
    return Object.freeze({
      codes: Object.freeze({ list: functionalCode(result.dto) }),
      count: 0,
      latenciesMs: Object.freeze([result.elapsedMs])
    });
  },
  async executeRead({ invoke, taskId, budgetMs }) {
    if (typeof invoke !== 'function' || !Number.isSafeInteger(taskId) || taskId <= 0 || !Number.isSafeInteger(budgetMs) || budgetMs <= 0) {
      fail('NOTES_READ_INPUT_INVALID');
    }

    const listed = await invoke('ListarNotas', { idTarea: taskId, cursor: '', tamanoPagina: 1 });
    assertWithinBudget(listed, budgetMs, 'NOTES_READ_LIST_BUDGET');
    if (functionalCode(listed.dto)) fail('NOTES_READ_LIST_BLOCKED');
    const notes = notesFrom(listed.dto);
    if (notes.length === 0) fail('NOTES_READ_LIST_EMPTY');

    const queried = await invoke('ConsultarNota', { idTarea: taskId, idNota: noteId(notes[0]) });
    assertWithinBudget(queried, budgetMs, 'NOTES_READ_GET_BUDGET');
    if (functionalCode(queried.dto)) fail('NOTES_READ_GET_BLOCKED');

    const invalidCursor = await invoke('ListarNotas', { idTarea: taskId, cursor: 'cursor-invalido-e2e', tamanoPagina: 1 });
    assertWithinBudget(invalidCursor, budgetMs, 'NOTES_READ_CURSOR_BUDGET');
    if (!isBlocked(invalidCursor.dto)) fail('NOTES_READ_CURSOR_NOT_BLOCKED');

    return Object.freeze({
      codes: Object.freeze({
        list: functionalCode(listed.dto),
        get: functionalCode(queried.dto),
        invalidCursor: functionalCode(invalidCursor.dto)
      }),
      count: notes.length,
      latenciesMs: Object.freeze([listed.elapsedMs, queried.elapsedMs, invalidCursor.elapsedMs])
    });
  }
});

module.exports = { NOTES_READ_E2E_ADAPTER };
