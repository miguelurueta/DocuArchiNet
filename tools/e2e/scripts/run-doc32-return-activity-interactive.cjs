'use strict';

const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole,
  runChild
} = require('./support/interactive-e2e-console.cjs');

const mode = process.argv[2];
const supportedModes = new Set(['anonymous', 'validation', 'preview', 'execute', 'concurrency']);

function fail(message) {
  console.error(message);
  process.exitCode = 2;
}

async function collectConfiguration(selectedMode) {
  const values = {};
  await collectValue(values, 'DOC32_E2E_BASE_URL', 'URL base de Gestión');
  if (selectedMode === 'anonymous') return values;

  await collectValue(values, 'DOC32_E2E_MODULE', 'Módulo');
  await collectValue(values, 'DOC32_E2E_AUTHORIZED_USER', 'Cuenta Workflow autorizada');
  await collectValue(values, 'DOC32_E2E_AUTHORIZED_PASSWORD', 'Contraseña Workflow', { secret: true });
  if (selectedMode === 'validation') return values;

  await collectValue(values, 'DOC32_E2E_ENVIRONMENT', 'Ambiente autorizado');
  await collectConfirmation(values, 'DOC32_E2E_ENVIRONMENT_AUTHORIZED', '¿Autoriza este ambiente de pruebas?');
  values.DOC32_E2E_ODBC_DSN = 'workflowconta';
  await collectValue(values, 'DOC32_E2E_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(values, 'DOC32_E2E_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
  await collectValue(values, 'DOC32_E2E_TASK_STATE_SQL', 'SELECT de estado de la tarea');
  await collectValue(values, 'DOC32_E2E_AUDIT_SQL', 'SELECT de auditoría DOC-32');

  if (selectedMode === 'preview' || selectedMode === 'execute') {
    await collectValue(values, 'DOC32_E2E_EXECUTION_TASK_ID', 'ID de tarea descartable de ejecución');
  }
  if (selectedMode === 'preview') {
    await collectValue(values, 'DOC32_E2E_PREVIEW_MAX_MS', 'Presupuesto de preview en ms', { defaultValue: '10000' });
    return values;
  }

  await collectConfirmation(values, 'DOC32_E2E_EXECUTION_AUTHORIZED', '¿Autoriza la devolución de actividad sobre la tarea descartable?');
  if (selectedMode === 'execute') {
    await collectValue(values, 'DOC32_E2E_EXECUTION_MAX_MS', 'Presupuesto de ejecución en ms', { defaultValue: '15000' });
    return values;
  }

  await collectConfirmation(values, 'DOC32_E2E_CONCURRENCY_AUTHORIZED', '¿Autoriza la carrera de exactamente dos devoluciones?');
  await collectValue(values, 'DOC32_E2E_CONCURRENCY_TASK_ID', 'ID de segunda tarea descartable de concurrencia');
  await collectValue(values, 'DOC32_E2E_CONCURRENCY_MAX_MS', 'Presupuesto de concurrencia en ms', { defaultValue: '15000' });
  return values;
}

function playwrightCommand(selectedMode) {
  const cli = path.resolve(__dirname, '..', 'node_modules', '@playwright', 'test', 'cli.js');
  return {
    command: process.execPath,
    args: [cli, 'test', 'tests/doc32-return-activity.spec.cjs', '--grep', `@doc32-${selectedMode}`, '--reporter=list']
  };
}

async function main() {
  if (!supportedModes.has(mode)) {
    fail('Modo DOC-32 inválido. Use anonymous, validation, preview, execute o concurrency.');
    return;
  }

  requireInteractiveConsole();
  const values = await collectConfiguration(mode);
  const environment = { ...process.env, ...values };
  try {
    const validation = await runChild(process.execPath, [path.resolve(__dirname, 'assert-doc32-return-activity-config.cjs'), mode], path.resolve(__dirname, '..'), environment);
    if (validation.code !== 0) {
      process.exitCode = validation.code;
      return;
    }
    const target = mode === 'concurrency'
      ? { command: process.execPath, args: [path.resolve(__dirname, 'run-doc32-return-activity-concurrency.cjs')] }
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
  } else if (/^DOC32_E2E_[A-Z_]+/.test(error?.message || '')) {
    console.error(error.message);
  } else {
    console.error('La E2E DOC-32 se detuvo antes de ejecutarse. No se mostraron valores sensibles.');
  }
  process.exitCode = 2;
});
