'use strict';

const { test, expect } = require('@playwright/test');
const crypto = require('node:crypto');
const fs = require('node:fs/promises');
const path = require('node:path');
const mysql = require('mysql2/promise');
const { createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
const launchOptions = {};
if (process.env.DOC28_E2E_BROWSER_PATH && process.env.DOC28_E2E_BROWSER_PATH.trim()) {
  launchOptions.executablePath = process.env.DOC28_E2E_BROWSER_PATH.trim();
} else if (process.env.DOC28_E2E_BROWSER_CHANNEL && process.env.DOC28_E2E_BROWSER_CHANNEL.trim()) {
  launchOptions.channel = process.env.DOC28_E2E_BROWSER_CHANNEL.trim();
}
test.use({
  launchOptions,
  screenshot: 'off',
  trace: 'off',
  video: 'off'
});

function required(name) {
  const value = process.env[name];
  return value && value.trim() ? value.trim() : null;
}

function baseUrl() {
  const value = required('DOC28_E2E_BASE_URL');
  return value ? new URL(value.endsWith('/') ? value : `${value}/`).toString() : null;
}

function previewUrl() {
  return new URL('webservice/WebServiceWorkflowModern.asmx/PreviewEnviarUsuario', baseUrl()).toString();
}

function executionUrl() {
  return new URL('webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioUsuario', baseUrl()).toString();
}

function positiveInteger(name, fallback) {
  const value = required(name) || fallback;
  const parsed = Number(value);
  if (!Number.isSafeInteger(parsed) || parsed <= 0) throw new Error(`${name} debe ser un entero positivo.`);
  return parsed;
}

function pageSize() {
  const value = positiveInteger('DOC28_E2E_PAGE_SIZE', '1');
  if (value > 50) throw new Error('DOC28_E2E_PAGE_SIZE no puede superar 50.');
  return value;
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql || '') || /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) || (sql.match(/\?/g) || []).length !== 1) {
    throw new Error(`${name} debe ser una única consulta SELECT de solo lectura con un parámetro ?.`);
  }
}

function fingerprint(rows) {
  return crypto.createHash('sha256').update(JSON.stringify(rows)).digest('hex');
}

async function queryFingerprint(pool, sql, idTarea) {
  const [rows] = await pool.execute(sql, [idTarea]);
  return fingerprint(rows);
}

function missing(names) {
  return names.filter((name) => !required(name));
}

function requireSettings(names, label) {
  const missingNames = missing(names);
  if (missingNames.length > 0) throw new Error(`Faltan variables ${label}: ${missingNames.join(', ')}.`);
  if (!baseUrl()) throw new Error('DOC28_E2E_BASE_URL debe ser una URL absoluta válida.');
}

function authenticatedSettings() {
  return [
    'DOC28_E2E_BASE_URL',
    'DOC28_E2E_MODULE',
    'DOC28_E2E_AUTHORIZED_USER',
    'DOC28_E2E_AUTHORIZED_PASSWORD'
  ];
}

function previewSettings() {
  return [
    ...authenticatedSettings(),
    'DOC28_E2E_TASK_ID',
    'DOC28_E2E_MYSQL_URL',
    'DOC28_E2E_TASK_STATE_SQL',
    'DOC28_E2E_AUDIT_SQL'
  ];
}

function executionSettings() {
  return [
    ...previewSettings(),
    'DOC28_E2E_EXECUTION_AUTHORIZED',
    'DOC28_E2E_EXPECTED_OUTCOME'
  ];
}

function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: 'DOC28_E2E_MODULE',
    userEnvironmentVariable: 'DOC28_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'DOC28_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.DOC28_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
}

function assertPublicResponse(dto) {
  const serialized = JSON.stringify(dto);
  expect(serialized).not.toMatch(/System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE)\s/i);
}

async function invoke(context, url, payload) {
  const response = await context.request.post(url, {
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    data: payload,
    timeout: 60000
  });
  expect(response.ok(), 'El ASMX debe devolver HTTP 200.').toBeTruthy();
  const envelope = await response.json();
  expect(envelope, 'El ASMX debe devolver el contenedor JSON d.').toHaveProperty('d');
  expect(typeof envelope.d, 'El ASMX debe devolver un objeto JSON, no HTML ni una excepción.').toBe('object');
  assertPublicResponse(envelope.d);
  return envelope.d;
}

function previewPayload(idTarea, cursor) {
  return {
    idTarea,
    consulta: '',
    cursor: cursor || '',
    tamanoPagina: pageSize()
  };
}

async function invokePreview(context, idTarea, cursor) {
  return invoke(context, previewUrl(), previewPayload(idTarea, cursor));
}

function expectNoDestinations(preview) {
  expect(preview.Destinos || []).toHaveLength(0);
}

function expectValidDestination(preview) {
  expect(preview.Error, 'El preview de una tarea preparada no debe bloquearse.').toBeNull();
  expect(preview.TokenVersion, 'El preview debe proporcionar el token actual.').toBeTruthy();
  expect(Array.isArray(preview.Destinos), 'El preview debe devolver la lista de destinos.').toBeTruthy();
  expect(preview.Destinos.length, 'La tarea preparada debe tener al menos un destino autorizado.').toBeGreaterThan(0);
  const destination = preview.Destinos[0];
  expect(Number.isSafeInteger(destination.IdUsuarioWorkflowDestino) && destination.IdUsuarioWorkflowDestino > 0).toBeTruthy();
  expect(Number.isSafeInteger(destination.IdActividadDestino) && destination.IdActividadDestino > 0).toBeTruthy();
  return destination;
}

function expectPagination(preview) {
  expect(preview.TieneMas, 'La tarea configurada debía probar paginación.').toBeTruthy();
  expect(preview.CursorSiguiente, 'El preview paginado debe incluir cursor siguiente.').toBeTruthy();
}

function uniqueDestinationKey(destination) {
  return `${destination.IdUsuarioWorkflowDestino}:${destination.IdActividadDestino}`;
}

function expectCurrentPreview(preview, expectedCode) {
  if (expectedCode) {
    expect(preview.Error?.Codigo, 'El preview debe devolver el bloqueo funcional esperado.').toBe(expectedCode);
    expectNoDestinations(preview);
    return false;
  }

  expectValidDestination(preview);
  return true;
}

function evidencePath(kind) {
  const name = kind === 'execution' ? 'DOC28_E2E_EXECUTION_EVIDENCE_PATH' : 'DOC28_E2E_PREVIEW_EVIDENCE_PATH';
  const configured = required(name);
  const defaultName = kind === 'execution' ? 'doc28-user-send-execution-e2e.json' : 'doc28-user-send-preview-e2e.json';
  if (!configured) return path.join(__dirname, '..', 'artifacts', defaultName);
  return path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured);
}

async function writeEvidence(kind, evidence) {
  const destination = evidencePath(kind);
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function assertLocalGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i);
  expect(configuration).toMatch(/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i);
}

test.beforeAll(async () => {
  await assertLocalGateOff();
});

test.afterAll(async () => {
  await assertLocalGateOff();
});

test('@doc28-anonymous PreviewEnviarUsuario sin sesión bloquea el ASMX sin exponer destinos', async ({ browser }) => {
  requireSettings(['DOC28_E2E_BASE_URL'], 'para el borde anónimo');
  const context = await browser.newContext({ ignoreHTTPSErrors: process.env.DOC28_E2E_IGNORE_HTTPS_ERRORS === 'true' });
  try {
    const preview = await invokePreview(context, 1, '');
    expect(preview.Error?.Codigo).toBe('WORKFLOW_CONTEXT_INVALID');
    expectNoDestinations(preview);
  } finally {
    await context.close();
  }
});

test('@doc28-validation Una sesión Gestión válida bloquea parámetros inválidos sin transición', async ({ browser }) => {
  requireSettings(authenticatedSettings(), 'para validación autenticada');
  const context = await login(browser);
  try {
    const preview = await invoke(context, previewUrl(), { idTarea: 0, consulta: '', cursor: '', tamanoPagina: 1 });
    expect(preview.Error?.Codigo).toBe('WORKFLOW_TASK_INVALID');
    expectNoDestinations(preview);
  } finally {
    await context.close();
  }
});

test('@doc28-preview PreviewEnviarUsuario preserva estado y auditoría con paginación verificable', async ({ browser }) => {
  requireSettings(previewSettings(), 'para preview completo');
  const idTarea = positiveInteger('DOC28_E2E_TASK_ID');
  const stateSql = process.env.DOC28_E2E_TASK_STATE_SQL;
  const auditSql = process.env.DOC28_E2E_AUDIT_SQL;
  assertReadOnlySql(stateSql, 'DOC28_E2E_TASK_STATE_SQL');
  assertReadOnlySql(auditSql, 'DOC28_E2E_AUDIT_SQL');

  const pool = mysql.createPool(process.env.DOC28_E2E_MYSQL_URL);
  let context;
  let beforeState;
  let afterState;
  let beforeAudit;
  let afterAudit;
  let firstPreview;
  let secondPreview;
  try {
    beforeState = await queryFingerprint(pool, stateSql, idTarea);
    beforeAudit = await queryFingerprint(pool, auditSql, idTarea);
    context = await login(browser);
    firstPreview = await invokePreview(context, idTarea, '');

    if (expectCurrentPreview(firstPreview, required('DOC28_E2E_PREVIEW_EXPECTED_CODE'))) {
      if (firstPreview.TieneMas || required('DOC28_E2E_EXPECT_PAGINATION')?.toLowerCase() === 'true') {
        expectPagination(firstPreview);
        secondPreview = await invokePreview(context, idTarea, firstPreview.CursorSiguiente);
        expectValidDestination(secondPreview);
        const firstKeys = new Set(firstPreview.Destinos.map(uniqueDestinationKey));
        expect(secondPreview.Destinos.some((destination) => !firstKeys.has(uniqueDestinationKey(destination)))).toBeTruthy();
      }
    }
  } finally {
    afterState = await queryFingerprint(pool, stateSql, idTarea);
    afterAudit = await queryFingerprint(pool, auditSql, idTarea);
    await context?.close();
    await pool.end();
  }

  expect(afterState, 'El preview no debe modificar estado de tarea.').toBe(beforeState);
  expect(afterAudit, 'El preview no debe modificar auditoría.').toBe(beforeAudit);
  await writeEvidence('preview', {
    fechaUtc: new Date().toISOString(),
    endpoint: 'PreviewEnviarUsuario',
    codigo: firstPreview?.Error?.Codigo || null,
    destinos: Array.isArray(firstPreview?.Destinos) ? firstPreview.Destinos.length : 0,
    paginaAdicional: Boolean(secondPreview),
    estadoSinMutacion: true,
    auditoriaSinMutacion: true,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit }
  });
});

test('@doc28-execute EjecutarEnvioUsuario usa el destino y token del preview actual sobre tarea descartable', async ({ browser }) => {
  requireSettings(executionSettings(), 'para ejecución autorizada');
  if (process.env.DOC28_E2E_EXECUTION_AUTHORIZED.toLowerCase() !== 'true') {
    throw new Error('DOC28_E2E_EXECUTION_AUTHORIZED debe ser exactamente true para ejecutar una transición.');
  }

  const idTarea = positiveInteger('DOC28_E2E_TASK_ID');
  const expectedOutcome = required('DOC28_E2E_EXPECTED_OUTCOME').toLowerCase();
  const expectedCode = required('DOC28_E2E_EXPECTED_CODE');
  if (expectedOutcome === 'blocked' && !expectedCode) {
    throw new Error('DOC28_E2E_EXPECTED_CODE es obligatorio para el resultado blocked.');
  }

  const stateSql = process.env.DOC28_E2E_TASK_STATE_SQL;
  const auditSql = process.env.DOC28_E2E_AUDIT_SQL;
  assertReadOnlySql(stateSql, 'DOC28_E2E_TASK_STATE_SQL');
  assertReadOnlySql(auditSql, 'DOC28_E2E_AUDIT_SQL');

  const pool = mysql.createPool(process.env.DOC28_E2E_MYSQL_URL);
  let context;
  let beforeState;
  let afterState;
  let beforeAudit;
  let afterAudit;
  let result;
  let preview;
  try {
    beforeState = await queryFingerprint(pool, stateSql, idTarea);
    beforeAudit = await queryFingerprint(pool, auditSql, idTarea);
    context = await login(browser);
    preview = await invokePreview(context, idTarea, '');
    const destination = expectValidDestination(preview);
    result = await invoke(context, executionUrl(), {
      idTarea,
      idUsuarioWorkflowDestino: destination.IdUsuarioWorkflowDestino,
      idActividadDestino: destination.IdActividadDestino,
      tokenVersion: preview.TokenVersion
    });
  } finally {
    afterState = await queryFingerprint(pool, stateSql, idTarea);
    afterAudit = await queryFingerprint(pool, auditSql, idTarea);
    await context?.close();
    await pool.end();
  }

  const stateChanged = beforeState !== afterState;
  const auditChanged = beforeAudit !== afterAudit;
  if (expectedOutcome === 'success') {
    expect(result.Exito, 'La tarea descartable debía completar el envío a usuario.').toBeTruthy();
    expect(result.EstadoFinal, 'Un envío exitoso debe finalizar en el estado funcional completada.').toBe('completada');
    expect(stateChanged, 'El éxito debe reflejar cambio de estado.').toBeTruthy();
    expect(auditChanged, 'El éxito debe dejar una huella de auditoría.').toBeTruthy();
  } else {
    expect(result.Exito, 'El escenario bloqueado no debe confirmar una transición.').toBeFalsy();
    expect(result.CodigoBloqueo || result.Error?.Codigo).toBe(expectedCode);
    expect(stateChanged, 'Un bloqueo funcional no debe cambiar el estado de la tarea.').toBeFalsy();
  }

  await writeEvidence('execution', {
    fechaUtc: new Date().toISOString(),
    endpoint: 'EjecutarEnvioUsuario',
    resultadoEsperado: expectedOutcome,
    exito: Boolean(result.Exito),
    estadoFinal: result.EstadoFinal || null,
    codigo: result.CodigoBloqueo || result.Error?.Codigo || null,
    destinosPreview: Array.isArray(preview?.Destinos) ? preview.Destinos.length : 0,
    estadoCambio: stateChanged,
    auditoriaCambio: auditChanged,
    advertencias: Array.isArray(result.Advertencias) ? result.Advertencias.length : 0
  });
});
