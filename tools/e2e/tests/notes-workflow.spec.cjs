'use strict';

const { test, expect } = require('@playwright/test');
const crypto = require('node:crypto');
const mysql = require('mysql2/promise');
const {
  assertLatency,
  assertLegacyPagesUnchanged,
  assertLocalGateOff,
  assertReadOnlySql,
  functionalCode,
  invoke,
  isSuccessful,
  login,
  noteFrom,
  noteId,
  noteVersion,
  notesFrom,
  operationPayload,
  positiveInteger,
  queryFingerprint,
  requireNames,
  required,
  writeEvidence,
  writePayload
} = require('./support/notes-workflow-e2e.cjs');

const launchOptions = {};
if (required('NOTES_E2E_BROWSER_PATH')) launchOptions.executablePath = required('NOTES_E2E_BROWSER_PATH');
else if (required('NOTES_E2E_BROWSER_CHANNEL')) launchOptions.channel = required('NOTES_E2E_BROWSER_CHANNEL');
test.use({ launchOptions, screenshot: 'off', trace: 'off', video: 'off' });

function authenticatedNames() {
  return ['NOTES_E2E_BASE_URL', 'NOTES_E2E_MODULE', 'NOTES_E2E_AUTHORIZED_USER', 'NOTES_E2E_AUTHORIZED_PASSWORD'];
}

function protectedNames(taskVariable, authorizationNames, budgetVariable) {
  return [
    ...authenticatedNames(),
    'NOTES_E2E_ENVIRONMENT',
    'NOTES_E2E_ENVIRONMENT_AUTHORIZED',
    ...authorizationNames,
    taskVariable,
    'NOTES_E2E_MYSQL_URL',
    'NOTES_E2E_TASK_STATE_SQL',
    'NOTES_E2E_AUDIT_SQL',
    budgetVariable
  ];
}

function assertProtectedConfiguration(taskVariable, authorizationNames, budgetVariable) {
  requireNames(protectedNames(taskVariable, authorizationNames, budgetVariable));
  if (required('NOTES_E2E_ENVIRONMENT_AUTHORIZED').toLowerCase() !== 'true') {
    throw new Error('La autorización de ambiente debe ser true.');
  }
  for (const name of authorizationNames) {
    if (required(name).toLowerCase() !== 'true') throw new Error(`${name} debe ser true.`);
  }
  assertReadOnlySql(required('NOTES_E2E_TASK_STATE_SQL'), 'NOTES_E2E_TASK_STATE_SQL');
  assertReadOnlySql(required('NOTES_E2E_AUDIT_SQL'), 'NOTES_E2E_AUDIT_SQL');
}

function expectBlocked(dto) {
  expect(isSuccessful(dto), 'La operación bloqueada no debe confirmar éxito.').toBeFalsy();
  expect(functionalCode(dto), 'El bloqueo debe incluir un código funcional.').toBeTruthy();
  expect(notesFrom(dto), 'Un bloqueo no debe exponer notas.').toHaveLength(0);
}

function expectSuccessful(dto, label) {
  expect(isSuccessful(dto), `${label} debe confirmar éxito.`).toBeTruthy();
  expect(functionalCode(dto), `${label} no debe devolver bloqueo.`).toBeFalsy();
}

test.beforeAll(async () => {
  await assertLocalGateOff();
});

test.afterAll(async () => {
  await assertLocalGateOff();
  await assertLegacyPagesUnchanged();
});

test('@notes-anonymous ListarNotas sin sesión bloquea sin exponer notas', async ({ browser }) => {
  requireNames(['NOTES_E2E_BASE_URL']);
  const context = await browser.newContext({ ignoreHTTPSErrors: process.env.NOTES_E2E_IGNORE_HTTPS_ERRORS === 'true' });
  try {
    const { dto } = await invoke(context, 'ListarNotas', operationPayload(1, { cursor: '', tamanoPagina: 1 }));
    expectBlocked(dto);
  } finally {
    await context.close();
  }
});

test('@notes-read Lecturas autorizadas preservan estado, auditoría y aislamiento', async ({ browser }) => {
  const taskVariable = 'NOTES_E2E_READ_TASK_ID';
  const budgetVariable = 'NOTES_E2E_READ_MAX_MS';
  assertProtectedConfiguration(taskVariable, [], budgetVariable);
  const idTarea = positiveInteger(taskVariable);
  const budgetMs = positiveInteger(budgetVariable);
  const stateSql = required('NOTES_E2E_TASK_STATE_SQL');
  const auditSql = required('NOTES_E2E_AUDIT_SQL');
  const pool = mysql.createPool(required('NOTES_E2E_MYSQL_URL'));
  let context;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let listResult;
  let noteResult;
  let invalidCursorResult;
  try {
    beforeState = await queryFingerprint(pool, stateSql, idTarea);
    beforeAudit = await queryFingerprint(pool, auditSql, idTarea);
    context = await login(browser);
    listResult = await invoke(context, 'ListarNotas', operationPayload(idTarea, { cursor: '', tamanoPagina: 1 }));
    assertLatency(listResult.elapsedMs, budgetMs, 'El listado de Notas');
    expect(functionalCode(listResult.dto), 'El listado autorizado no debe bloquearse.').toBeFalsy();
    const notes = notesFrom(listResult.dto);
    expect(notes, 'La tarea de lectura debe tener una nota visible para validar consulta.').not.toHaveLength(0);
    const idNota = noteId(notes[0]);
    noteResult = await invoke(context, 'ConsultarNota', operationPayload(idTarea, { idNota }));
    assertLatency(noteResult.elapsedMs, budgetMs, 'La consulta de Nota');
    expect(functionalCode(noteResult.dto), 'La nota de la tarea autorizada debe consultarse.').toBeFalsy();
    invalidCursorResult = await invoke(context, 'ListarNotas', operationPayload(idTarea, { cursor: 'cursor-invalido-e2e', tamanoPagina: 1 }));
    assertLatency(invalidCursorResult.elapsedMs, budgetMs, 'La validación de cursor de Notas');
    expectBlocked(invalidCursorResult.dto);
  } finally {
    afterState = await queryFingerprint(pool, stateSql, idTarea);
    afterAudit = await queryFingerprint(pool, auditSql, idTarea);
    await context?.close();
    await pool.end();
  }
  expect(afterState, 'La lectura no debe modificar estado.').toBe(beforeState);
  expect(afterAudit, 'La lectura no debe modificar auditoría.').toBe(beforeAudit);
  await writeEvidence('read', {
    fechaUtc: new Date().toISOString(),
    modo: 'read',
    codigoListado: functionalCode(listResult?.dto),
    codigoConsulta: functionalCode(noteResult?.dto),
    codigoCursorInvalido: functionalCode(invalidCursorResult?.dto),
    cantidadListado: notesFrom(listResult?.dto).length,
    latenciasMs: [listResult?.elapsedMs, noteResult?.elapsedMs, invalidCursorResult?.elapsedMs],
    estadoSinCambio: true,
    auditoriaSinCambio: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

test('@notes-write Escrituras autorizadas verifican idempotencia, versión y eliminación', async ({ browser }) => {
  const taskVariable = 'NOTES_E2E_WRITE_TASK_ID';
  const budgetVariable = 'NOTES_E2E_WRITE_MAX_MS';
  assertProtectedConfiguration(taskVariable, ['NOTES_E2E_EXECUTION_AUTHORIZED'], budgetVariable);
  const idTarea = positiveInteger(taskVariable);
  const budgetMs = positiveInteger(budgetVariable);
  const stateSql = required('NOTES_E2E_TASK_STATE_SQL');
  const auditSql = required('NOTES_E2E_AUDIT_SQL');
  const pool = mysql.createPool(required('NOTES_E2E_MYSQL_URL'));
  const clientRequestId = crypto.randomUUID();
  const initialContent = 'Prueba E2E temporal de Notas';
  const updatedContent = 'Prueba E2E temporal de Notas actualizada';
  let context;
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let createFirst;
  let createRetry;
  let update;
  let staleUpdate;
  let deletion;
  try {
    beforeState = await queryFingerprint(pool, stateSql, idTarea);
    beforeAudit = await queryFingerprint(pool, auditSql, idTarea);
    context = await login(browser);
    createFirst = await invoke(context, 'CrearNota', writePayload(idTarea, initialContent, clientRequestId));
    assertLatency(createFirst.elapsedMs, budgetMs, 'La creación de Nota');
    expectSuccessful(createFirst.dto, 'La creación de Nota');
    const firstNote = noteFrom(createFirst.dto);
    const idNota = noteId(firstNote);
    const versionInicial = noteVersion(firstNote);
    createRetry = await invoke(context, 'CrearNota', writePayload(idTarea, initialContent, clientRequestId));
    assertLatency(createRetry.elapsedMs, budgetMs, 'El reintento idempotente de Nota');
    expectSuccessful(createRetry.dto, 'El reintento idempotente de Nota');
    expect(noteId(noteFrom(createRetry.dto)), 'El reintento debe devolver la nota original.').toBe(idNota);
    update = await invoke(context, 'ActualizarNota', operationPayload(idTarea, { idNota, contenido: updatedContent, version: versionInicial }));
    assertLatency(update.elapsedMs, budgetMs, 'La actualización de Nota');
    expectSuccessful(update.dto, 'La actualización de Nota');
    const versionActualizada = noteVersion(noteFrom(update.dto));
    staleUpdate = await invoke(context, 'ActualizarNota', operationPayload(idTarea, { idNota, contenido: initialContent, version: versionInicial }));
    assertLatency(staleUpdate.elapsedMs, budgetMs, 'El conflicto de versión de Nota');
    expectBlocked(staleUpdate.dto);
    deletion = await invoke(context, 'EliminarNota', operationPayload(idTarea, { idNota, version: versionActualizada }));
    assertLatency(deletion.elapsedMs, budgetMs, 'La eliminación de Nota');
    expectSuccessful(deletion.dto, 'La eliminación de Nota');
  } finally {
    afterState = await queryFingerprint(pool, stateSql, idTarea);
    afterAudit = await queryFingerprint(pool, auditSql, idTarea);
    await context?.close();
    await pool.end();
  }
  expect(afterState, 'Las escrituras autorizadas deben reflejarse en el estado esperado.').not.toBe(beforeState);
  expect(afterAudit, 'Las escrituras autorizadas deben reflejarse en auditoría.').not.toBe(beforeAudit);
  await writeEvidence('write', {
    fechaUtc: new Date().toISOString(),
    modo: 'write',
    codigos: [functionalCode(createFirst?.dto), functionalCode(createRetry?.dto), functionalCode(update?.dto), functionalCode(staleUpdate?.dto), functionalCode(deletion?.dto)],
    creacionIdempotente: noteId(noteFrom(createFirst?.dto)) === noteId(noteFrom(createRetry?.dto)),
    conflictoVersion: Boolean(functionalCode(staleUpdate?.dto)),
    eliminacionExitosa: isSuccessful(deletion?.dto),
    latenciasMs: [createFirst?.elapsedMs, createRetry?.elapsedMs, update?.elapsedMs, staleUpdate?.elapsedMs, deletion?.elapsedMs],
    estadoCambio: true,
    auditoriaCambio: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});
