'use strict';

const fs = require('node:fs/promises');
const path = require('node:path');
const { request } = require('playwright');
const { createAuthenticatedWorkflowSession } = require('./authenticated-workflow-session.cjs');
const { queryFingerprint: queryOdbcFingerprint } = require('../../scripts/support/doc32-e2e-odbc.cjs');

const repositoryRoot = path.resolve(__dirname, '..', '..', '..', '..');
const sensitiveEvidencePattern = /password|cookie|token|destino|usuario|mysql|connection|contenido|nota|request|response/i;

function required(name) {
  const value = process.env[name];
  return typeof value === 'string' && value.trim() ? value.trim() : null;
}

function requireNames(names) {
  const missing = names.filter((name) => !required(name));
  if (missing.length > 0) throw new Error(`Faltan variables Notes E2E: ${missing.join(', ')}.`);
}

function positiveInteger(name) {
  const value = Number(required(name));
  if (!Number.isSafeInteger(value) || value <= 0) throw new Error(`${name} debe ser un entero positivo.`);
  return value;
}

function baseUrl() {
  const value = required('NOTES_E2E_BASE_URL');
  if (!value) throw new Error('NOTES_E2E_BASE_URL es obligatoria.');
  return new URL(value.endsWith('/') ? value : `${value}/`).toString();
}

function servicePath() {
  return (required('NOTES_E2E_SERVICE_PATH') || 'webservice/WebServiceWorkflowNotesModern.asmx').replace(/^\/+/, '');
}

function endpoint(operation) {
  return new URL(`${servicePath()}/${operation}`, baseUrl()).toString();
}

function assertReadOnlySql(sql, name) {
  if (!/^\s*SELECT\b/i.test(sql || '') ||
      /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i.test(sql) ||
      (sql.match(/\?/g) || []).length !== 1) {
    throw new Error(`${name} debe ser una única consulta SELECT de solo lectura con un parámetro ?.`);
  }
}

async function queryFingerprint(sql, idTarea, environment = process.env) {
  return queryOdbcFingerprint(sql, idTarea, environment, 'NOTES_E2E');
}

function login(browser) {
  return createAuthenticatedWorkflowSession(browser, {
    baseUrl: baseUrl(),
    moduleEnvironmentVariable: 'NOTES_E2E_MODULE',
    userEnvironmentVariable: 'NOTES_E2E_AUTHORIZED_USER',
    passwordEnvironmentVariable: 'NOTES_E2E_AUTHORIZED_PASSWORD',
    ignoreHTTPSErrors: process.env.NOTES_E2E_IGNORE_HTTPS_ERRORS === 'true'
  });
}

async function createRequestClient(context) {
  if (process.env.NOTES_E2E_IGNORE_HTTPS_ERRORS !== 'true') {
    return { request: context.request, dispose: async () => {} };
  }
  const api = await request.newContext({
    storageState: await context.storageState(),
    ignoreHTTPSErrors: true
  });
  return { request: api, dispose: () => api.dispose() };
}

function assertPublicResponse(dto) {
  const serialized = JSON.stringify(dto);
  if (/System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE|DELETE)\s/i.test(serialized)) {
    throw new Error('El ASMX de Notas devolvió detalles internos no permitidos.');
  }
}

async function invoke(client, operation, payload) {
  const started = performance.now();
  const response = await client.request.post(endpoint(operation), {
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    data: payload,
    timeout: 60000
  });
  const elapsedMs = Math.round(performance.now() - started);
  if (!response.ok()) throw new Error(`El ASMX de Notas devolvió HTTP_${response.status()}.`);
  const envelope = await response.json();
  if (!envelope || typeof envelope.d !== 'object' || envelope.d === null) {
    throw new Error('El ASMX de Notas debe devolver un DTO dentro de d.');
  }
  assertPublicResponse(envelope.d);
  return { dto: envelope.d, elapsedMs };
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

function notesFrom(dto) {
  const notes = field(dto, ['Notas', 'notas', 'Items', 'items']);
  return Array.isArray(notes) ? notes : [];
}

function noteFrom(dto) {
  return field(dto, ['Nota', 'nota']) || dto;
}

function noteId(note) {
  const value = field(note, ['IdNota', 'idNota', 'ID_ANOTACION', 'idAnotacion']);
  const parsed = Number(value);
  if (!Number.isSafeInteger(parsed) || parsed <= 0) throw new Error('La respuesta debe incluir un identificador de nota válido.');
  return parsed;
}

function noteVersion(note) {
  const value = field(note, ['Version', 'version']);
  if (value === undefined || value === null || value === '') throw new Error('La respuesta debe incluir una versión de nota.');
  return value;
}

function assertLatency(elapsedMs, budgetMs, label) {
  if (elapsedMs > budgetMs) throw new Error(`${label} excedió el presupuesto configurado.`);
}

function safeArtifactPath(kind) {
  const configured = required(`NOTES_E2E_${kind.toUpperCase()}_EVIDENCE_PATH`);
  const fallback = path.join(repositoryRoot, 'tools', 'e2e', 'artifacts', `notes-workflow-${kind}.json`);
  const destination = configured ? (path.isAbsolute(configured) ? configured : path.resolve(repositoryRoot, configured)) : fallback;
  if (path.relative(repositoryRoot, destination).startsWith('..')) throw new Error('La evidencia de Notas debe permanecer dentro del repositorio.');
  return destination;
}

async function writeEvidence(kind, evidence) {
  const serialized = JSON.stringify(evidence);
  if (sensitiveEvidencePattern.test(serialized)) {
    throw new Error('La evidencia de Notas contiene un campo sensible no permitido.');
  }
  const destination = safeArtifactPath(kind);
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function assertLocalGateOff() {
  const configuration = await fs.readFile(path.join(repositoryRoot, 'Web.config'), 'utf8');
  if (!/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i.test(configuration)) {
    throw new Error('El gate local debe permanecer apagado y sin alcance para Notas.');
  }
}

async function assertLegacyPagesUnchanged() {
  const { execFile } = require('node:child_process');
  const { promisify } = require('node:util');
  const execute = promisify(execFile);
  const result = await execute('git', ['diff', '--name-only', '--', 'workflow/Webworkflow.aspx', 'workflow/Webworkflow.aspx.vb'], { cwd: repositoryRoot });
  if (result.stdout.trim()) throw new Error('Las páginas legacy Workflow tienen cambios pendientes; la corrida E2E se detiene.');
}

function operationPayload(idTarea, extra) {
  return { idTarea, ...extra };
}

function writePayload(idTarea, contenido, clientRequestId) {
  return operationPayload(idTarea, { contenido, clientRequestId });
}

module.exports = {
  assertLatency,
  assertLegacyPagesUnchanged,
  assertLocalGateOff,
  assertReadOnlySql,
  baseUrl,
  createRequestClient,
  endpoint,
  field,
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
};
