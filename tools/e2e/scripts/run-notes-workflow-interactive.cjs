'use strict';

const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole,
  runChild
} = require('./support/interactive-e2e-console.cjs');

const mode = process.argv[2];
const supportedModes = new Set(['anonymous', 'read', 'write', 'concurrency']);
const servicePath = 'webservice/WebServiceWorkflowNotesModern.asmx';

function fail(message) {
  console.error(message);
  process.exitCode = 2;
}

async function collectConfiguration(selectedMode) {
  const values = {};
  await collectValue(values, 'NOTES_E2E_BASE_URL', 'URL base de Gestión');
  values.NOTES_E2E_SERVICE_PATH = servicePath;

  if (selectedMode === 'anonymous') return values;

  await collectValue(values, 'NOTES_E2E_MODULE', 'Módulo');
  await collectValue(values, 'NOTES_E2E_AUTHORIZED_USER', 'Cuenta Workflow autorizada');
  await collectValue(values, 'NOTES_E2E_AUTHORIZED_PASSWORD', 'Contraseña Workflow', { secret: true });
  await collectValue(values, 'NOTES_E2E_ENVIRONMENT', 'Ambiente autorizado');
  await collectConfirmation(values, 'NOTES_E2E_ENVIRONMENT_AUTHORIZED', '¿Autoriza este ambiente de pruebas?');
  await collectValue(values, 'NOTES_E2E_MYSQL_URL', 'URL MySQL de solo lectura', { secret: true });
  await collectValue(values, 'NOTES_E2E_TASK_STATE_SQL', 'SELECT de estado de la tarea');
  await collectValue(values, 'NOTES_E2E_AUDIT_SQL', 'SELECT de auditoría de la tarea');

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

function playwrightCommand(selectedMode) {
  const executable = path.resolve(__dirname, '..', 'node_modules', '.bin', process.platform === 'win32' ? 'playwright.cmd' : 'playwright');
  const tag = selectedMode === 'anonymous' ? '@notes-anonymous' : selectedMode === 'read' ? '@notes-read' : '@notes-write';
  return { command: executable, args: ['test', 'tests/notes-workflow.spec.cjs', '--grep', tag, '--reporter=list'] };
}

async function main() {
  if (!supportedModes.has(mode)) {
    fail('Modo Notes E2E inválido. Use anonymous, read, write o concurrency.');
    return;
  }

  requireInteractiveConsole();
  const values = await collectConfiguration(mode);
  const environment = { ...process.env, ...values };
  try {
    const validation = await runChild(process.execPath, [path.resolve(__dirname, 'assert-notes-workflow-config.cjs'), mode], path.resolve(__dirname, '..'), environment);
    if (validation.code !== 0) {
      process.exitCode = validation.code;
      return;
    }
    const target = mode === 'concurrency'
      ? { command: process.execPath, args: [path.resolve(__dirname, 'run-notes-workflow-concurrency.cjs')] }
      : playwrightCommand(mode);
    const result = await runChild(target.command, target.args, path.resolve(__dirname, '..'), environment);
    process.exitCode = result.code;
  } finally {
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
