'use strict';

const path = require('node:path');
const fs = require('node:fs/promises');
const {
  collectConfirmation,
  collectValue,
  redactChildOutput,
  requireInteractiveConsole,
  runChild
} = require('./support/interactive-e2e-console.cjs');

const SAFE_ACTIVE = /WorkflowCentroTrabajoModernActive" value="false"/i;
const SAFE_USERS = /WorkflowCentroTrabajoModernUsers" value=""/i;
const SAFE_GROUPS = /WorkflowCentroTrabajoModernGroups" value=""/i;

function assertSafeGate(configuration) {
  if (!SAFE_ACTIVE.test(configuration) || !SAFE_USERS.test(configuration) || !SAFE_GROUPS.test(configuration)) {
    throw new Error('La configuración inicial del gate DOC-44 no es segura. No se inició la prueba.');
  }
}

async function main() {
  requireInteractiveConsole();
  const emptyMode = process.argv[2] === 'empty';
  const colorMode = process.argv[2] === 'unassigned-color';
  const baseUrl = process.env.DOC44_E2E_BASE_URL || 'https://localhost/GestionDocumental-Docuarchi.net/';
  const localSelfSigned = ['localhost', '127.0.0.1', '::1'].includes(new URL(baseUrl).hostname);
  const values = {
    DOC44_E2E_BASE_URL: baseUrl,
    DOC44_E2E_MODULE: process.env.DOC44_E2E_MODULE || 'GESTOR',
    DOC44_E2E_IGNORE_HTTPS_ERRORS: process.env.DOC44_E2E_IGNORE_HTTPS_ERRORS || (localSelfSigned ? 'true' : 'false')
  };
  await collectValue(values, 'DOC44_E2E_AUTHORIZED_USER', 'Cuenta Workflow autorizada');
  await collectValue(values, 'DOC44_E2E_AUTHORIZED_PASSWORD', 'Contraseña Workflow', { secret: true });
  await collectConfirmation(values, 'DOC44_E2E_ENVIRONMENT_AUTHORIZED', '¿Autoriza este ambiente de pruebas?');
  if (!colorMode) {
    await collectConfirmation(values, 'DOC44_E2E_EXECUTION_AUTHORIZED', emptyMode ? '¿Autoriza crear y eliminar una nota para validar el estado vacío?' : '¿Autoriza crear, editar y eliminar una nota sobre una tarea descartable?');
    await collectValue(values, 'DOC44_E2E_TASK_ID', emptyMode ? 'ID de tarea descartable sin notas DOC-45' : 'ID de tarea descartable DOC-44');
  }
  if (emptyMode) {
    values.DOC44_E2E_EMPTY_MODE = 'true';
  } else if (!colorMode) {
    await collectValue(values, 'DOC44_E2E_FOREIGN_TASK_ID', 'ID de tarea ajena para lectura negativa');
    await collectValue(values, 'DOC44_E2E_INACTIVE_TASK_ID', 'ID de tarea inactiva para lectura negativa');
    await collectValue(values, 'DOC44_E2E_FOREIGN_NOTE_ID', 'ID de nota que no pertenece a la tarea descartable');
    await collectValue(values, 'DOC44_E2E_NON_OWNER_NOTE_ID', 'ID de nota ajena que sí pertenece a la tarea descartable');
  }

  const repositoryRoot = path.resolve(__dirname, '..', '..', '..');
  const e2eRoot = path.resolve(__dirname, '..');
  const webConfigPath = path.join(repositoryRoot, 'Web.config');
  const originalConfiguration = await fs.readFile(webConfigPath, 'utf8');
  const environment = { ...process.env, ...values };
  assertSafeGate(originalConfiguration);

  try {
    const cli = path.join(e2eRoot, 'node_modules', '@playwright', 'test', 'cli.js');
    const result = await runChild(
      process.execPath,
      [cli, 'test', 'tests/doc44-workflow-notes.spec.cjs', '--grep', colorMode ? '@doc45-unassigned-color' : (emptyMode ? '@doc45-empty-notes' : '@doc44-workflow-notes'), '--reporter=list'],
      e2eRoot,
      environment,
      { nonInteractiveChild: true, redactOutput: redactChildOutput }
    );
    process.exitCode = result.code;
  } finally {
    assertSafeGate(await fs.readFile(webConfigPath, 'utf8'));
    for (const key of Object.keys(values)) delete environment[key];
  }
}

main().catch(() => {
  console.error('La E2E DOC-44 se detuvo de forma segura. No se mostraron secretos ni se dejó el gate habilitado.');
  process.exitCode = 2;
});
