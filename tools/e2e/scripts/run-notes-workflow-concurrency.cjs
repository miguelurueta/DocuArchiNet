'use strict';

const { chromium } = require('@playwright/test');
const {
  assertLatency,
  assertLegacyPagesUnchanged,
  assertLocalGateOff,
  assertReadOnlySql,
  createRequestClient,
  functionalCode,
  invoke,
  isSuccessful,
  login,
  noteFrom,
  noteVersion,
  operationPayload,
  positiveInteger,
  queryFingerprint,
  requireNames,
  required,
  writeEvidence
} = require('../tests/support/notes-workflow-e2e.cjs');

function launchOptions() {
  const options = { headless: true };
  if (required('NOTES_E2E_BROWSER_PATH')) options.executablePath = required('NOTES_E2E_BROWSER_PATH');
  else if (required('NOTES_E2E_BROWSER_CHANNEL')) options.channel = required('NOTES_E2E_BROWSER_CHANNEL');
  return options;
}

function assertConfiguration() {
  const names = [
    'NOTES_E2E_BASE_URL', 'NOTES_E2E_MODULE', 'NOTES_E2E_AUTHORIZED_USER', 'NOTES_E2E_AUTHORIZED_PASSWORD',
    'NOTES_E2E_ENVIRONMENT', 'NOTES_E2E_ENVIRONMENT_AUTHORIZED', 'NOTES_E2E_EXECUTION_AUTHORIZED',
    'NOTES_E2E_CONCURRENCY_AUTHORIZED', 'NOTES_E2E_CONCURRENCY_TASK_ID', 'NOTES_E2E_CONCURRENCY_NOTE_ID',
    'NOTES_E2E_ODBC_DSN', 'NOTES_E2E_MYSQL_USER', 'NOTES_E2E_MYSQL_PASSWORD',
    'NOTES_E2E_TASK_STATE_SQL', 'NOTES_E2E_AUDIT_SQL', 'NOTES_E2E_CONCURRENCY_MAX_MS'
  ];
  requireNames(names);
  for (const name of ['NOTES_E2E_ENVIRONMENT_AUTHORIZED', 'NOTES_E2E_EXECUTION_AUTHORIZED', 'NOTES_E2E_CONCURRENCY_AUTHORIZED']) {
    if (required(name).toLowerCase() !== 'true') throw new Error(`${name} debe ser true.`);
  }
  positiveInteger('NOTES_E2E_CONCURRENCY_TASK_ID');
  positiveInteger('NOTES_E2E_CONCURRENCY_NOTE_ID');
  positiveInteger('NOTES_E2E_CONCURRENCY_MAX_MS');
  assertReadOnlySql(required('NOTES_E2E_TASK_STATE_SQL'), 'NOTES_E2E_TASK_STATE_SQL');
  assertReadOnlySql(required('NOTES_E2E_AUDIT_SQL'), 'NOTES_E2E_AUDIT_SQL');
}

async function main() {
  assertConfiguration();
  await assertLocalGateOff();
  const idTarea = positiveInteger('NOTES_E2E_CONCURRENCY_TASK_ID');
  const idNota = positiveInteger('NOTES_E2E_CONCURRENCY_NOTE_ID');
  const budgetMs = positiveInteger('NOTES_E2E_CONCURRENCY_MAX_MS');
  const stateSql = required('NOTES_E2E_TASK_STATE_SQL');
  const auditSql = required('NOTES_E2E_AUDIT_SQL');
  const browser = await chromium.launch(launchOptions());
  let contexts = [];
  let clients = [];
  let beforeState;
  let beforeAudit;
  let afterState;
  let afterAudit;
  let results = [];
  try {
    beforeState = await queryFingerprint(stateSql, idTarea);
    beforeAudit = await queryFingerprint(auditSql, idTarea);
    contexts = [await login(browser), await login(browser)];
    clients = await Promise.all(contexts.map((context) => createRequestClient(context)));
    const current = await invoke(clients[0], 'ConsultarNota', operationPayload(idTarea, { idNota }));
    if (functionalCode(current.dto)) throw new Error('La nota semilla de concurrencia no está autorizada.');
    const version = noteVersion(noteFrom(current.dto));
    results = await Promise.all(clients.map((client, index) => invoke(client, 'ActualizarNota', operationPayload(idTarea, {
      idNota,
      contenido: `Prueba E2E de concurrencia ${index + 1}`,
      version
    }))));
  } finally {
    try {
      afterState = await queryFingerprint(stateSql, idTarea);
      afterAudit = await queryFingerprint(auditSql, idTarea);
    } finally {
      await Promise.all(clients.map((client) => client.dispose()));
      await Promise.all(contexts.map((context) => context.close()));
      await browser.close();
      await assertLocalGateOff();
      await assertLegacyPagesUnchanged();
    }
  }

  const successful = results.filter((result) => isSuccessful(result.dto));
  const blocked = results.filter((result) => !isSuccessful(result.dto));
  const approved = results.length === 2 && successful.length === 1 && blocked.length === 1 && Boolean(functionalCode(blocked[0].dto)) &&
    results.every((result) => result.elapsedMs <= budgetMs) && beforeState !== afterState && beforeAudit !== afterAudit;
  for (const result of results) assertLatency(result.elapsedMs, budgetMs, 'La actualización concurrente de Notas');
  await writeEvidence('concurrency', {
    fechaUtc: new Date().toISOString(),
    modo: 'concurrency',
    solicitudes: 2,
    exitos: successful.length,
    codigosBloqueo: blocked.map((result) => functionalCode(result.dto)),
    latenciasMs: results.map((result) => result.elapsedMs),
    presupuestoMs: budgetMs,
    estadoCambio: beforeState !== afterState,
    auditoriaCambio: beforeAudit !== afterAudit,
    huellas: { estadoAntes: beforeState, estadoDespues: afterState, auditoriaAntes: beforeAudit, auditoriaDespues: afterAudit },
    aprobada: approved
  });
  console.log(`Notes concurrencia: ${successful.length}/2 actualizaciones efectivas; evidencia saneada generada.`);
  if (!approved) process.exitCode = 1;
}

main().catch(() => {
  console.error('La carrera E2E de Notas no pudo completarse. No se mostraron secretos ni detalles internos.');
  process.exitCode = 1;
});
