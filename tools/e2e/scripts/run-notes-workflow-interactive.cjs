'use strict';

const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  redactChildOutput,
  requireInteractiveConsole,
  runChild
} = require('./support/interactive-e2e-console.cjs');

const mode = process.argv[2];
const supportedModes = new Set(['anonymous', 'read', 'write', 'concurrency']);
const servicePath = 'webservice/WebServiceWorkflowNotesModern.asmx';
const defaultBaseUrl = 'https://localhost/GestionDocumental-Docuarchi.net/';
const defaultModule = 'GESTOR';
const defaultEnvironment = 'GESTOR';
const taskStateSql = 'SELECT ID_ANOTACION, INICIO_TAREAS_WORKFLOW_ID_TAREA, ID_ACTIVIDAD, ID_USUARIO, FECHA_ANOTACION, ESTADO_TAREA FROM ANOTACION_TAREA WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = ? ORDER BY ID_ANOTACION';
const auditSql = "SELECT usuario_workflow_idU_suario, fecha_hora, operacion, ID_TAREA_WORKFLOW, opcion, descripcion_opcion, ip_transacion, id_operacion FROM wf_log_workflow WHERE ID_TAREA_WORKFLOW = ? AND descripcion_opcion = 'NOTA WORKFLOW' ORDER BY fecha_hora, id_operacion";

function nonSensitiveValue(name, fallback) {
  const value = process.env[name];
  return typeof value === 'string' && value.trim() ? value.trim() : fallback;
}

function fail(message) {
  console.error(message);
  process.exitCode = 2;
}

async function collectConfiguration(selectedMode) {
  const values = {};
  values.NOTES_E2E_BASE_URL = nonSensitiveValue('NOTES_E2E_BASE_URL', defaultBaseUrl);
  values.NOTES_E2E_SERVICE_PATH = servicePath;

  if (selectedMode === 'anonymous') return values;

  values.NOTES_E2E_MODULE = nonSensitiveValue('NOTES_E2E_MODULE', defaultModule);
  await collectValue(values, 'NOTES_E2E_AUTHORIZED_USER', 'Cuenta Workflow autorizada');
  await collectValue(values, 'NOTES_E2E_AUTHORIZED_PASSWORD', 'Contraseña Workflow', { secret: true });
  values.NOTES_E2E_ENVIRONMENT = nonSensitiveValue('NOTES_E2E_ENVIRONMENT', defaultEnvironment);
  await collectConfirmation(values, 'NOTES_E2E_ENVIRONMENT_AUTHORIZED', '¿Autoriza este ambiente de pruebas?');
  values.NOTES_E2E_ODBC_DSN = 'workflowconta';
  await collectValue(values, 'NOTES_E2E_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(values, 'NOTES_E2E_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
  values.NOTES_E2E_TASK_STATE_SQL = taskStateSql;
  values.NOTES_E2E_AUDIT_SQL = auditSql;

  if (selectedMode === 'read') {
    await collectValue(values, 'NOTES_E2E_READ_TASK_ID', 'ID de tarea de lectura');
    await collectValue(values, 'NOTES_E2E_READ_MAX_MS', 'Presupuesto de lectura en ms', { defaultValue: '10000' });
    return values;
  }

  await collectConfirmation(values, 'NOTES_E2E_EXECUTION_AUTHORIZED', '¿Autoriza la mutación sobre una tarea descartable?');
  if (selectedMode === 'write') {
    await collectValue(values, 'NOTES_E2E_WRITE_TASK_ID', 'ID de tarea descartable de escritura');
    await collectValue(values, 'NOTES_E2E_WRITE_MAX_MS', 'Presupuesto de escritura en ms', { defaultValue: '15000' });
    return values;
  }

  await collectConfirmation(values, 'NOTES_E2E_CONCURRENCY_AUTHORIZED', '¿Autoriza la carrera de exactamente dos actualizaciones?');
  await collectValue(values, 'NOTES_E2E_CONCURRENCY_TASK_ID', 'ID de tarea descartable de concurrencia');
  await collectValue(values, 'NOTES_E2E_CONCURRENCY_NOTE_ID', 'ID de nota semilla propia');
  await collectValue(values, 'NOTES_E2E_CONCURRENCY_MAX_MS', 'Presupuesto de concurrencia en ms', { defaultValue: '15000' });
  return values;
}

function playwrightCommand(selectedMode, outputDirectory) {
  const cli = path.resolve(__dirname, '..', 'node_modules', '@playwright', 'test', 'cli.js');
  const tag = selectedMode === 'anonymous' ? '@notes-anonymous' : selectedMode === 'read' ? '@notes-read' : '@notes-write';
  return { command: process.execPath, args: [cli, 'test', 'tests/notes-workflow.spec.cjs', '--grep', tag, '--reporter=list', '--output', outputDirectory] };
}

async function main() {
  if (!supportedModes.has(mode)) {
    fail('Modo Notes E2E inválido. Use anonymous, read, write o concurrency.');
    return;
  }

  requireInteractiveConsole();
  const values = await collectConfiguration(mode);
  const environment = { ...process.env, ...values };
  let outputDirectory;
  try {
    const validation = await runChild(process.execPath, [path.resolve(__dirname, 'assert-notes-workflow-config.cjs'), mode], path.resolve(__dirname, '..'), environment);
    if (validation.code !== 0) {
      process.exitCode = validation.code;
      return;
    }
    const target = mode === 'concurrency'
      ? { command: process.execPath, args: [path.resolve(__dirname, 'run-notes-workflow-concurrency.cjs')] }
      : playwrightCommand(mode, outputDirectory = await fs.mkdtemp(path.join(os.tmpdir(), 'notes-workflow-e2e-')));
    const result = await runChild(target.command, target.args, path.resolve(__dirname, '..'), environment, {
      nonInteractiveChild: true,
      redactOutput: redactChildOutput
    });
    process.exitCode = result.code;
  } finally {
    if (outputDirectory) await fs.rm(outputDirectory, { recursive: true, force: true });
    for (const name of Object.keys(values)) delete environment[name];
  }
}

main().catch((error) => {
  if (error?.message?.includes('consola interactiva')) {
    console.error(`${error.message} No se mostraron valores sensibles.`);
  } else if (/^NOTES_E2E_[A-Z_]+/.test(error?.message || '')) {
    console.error(error.message);
  } else {
    console.error('La E2E de Notas se detuvo antes de ejecutarse. No se mostraron valores sensibles.');
  }
  process.exitCode = 2;
});
