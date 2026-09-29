'use strict';

const path = require('node:path');
const { collectConfirmation, collectValue, redactChildOutput, requireInteractiveConsole, runChild } = require('./support/interactive-e2e-console.cjs');

async function main() {
  requireInteractiveConsole();
  const values = {
    DOC43_E2E_BASE_URL: process.env.DOC43_E2E_BASE_URL || 'https://localhost/GestionDocumental-Docuarchi.net/',
    DOC43_E2E_MODULE: process.env.DOC43_E2E_MODULE || 'GESTOR',
    DOC43_E2E_IGNORE_HTTPS_ERRORS: process.env.DOC43_E2E_IGNORE_HTTPS_ERRORS || 'false'
  };
  await collectValue(values, 'DOC43_E2E_AUTHORIZED_USER', 'Cuenta Workflow autorizada');
  await collectValue(values, 'DOC43_E2E_AUTHORIZED_PASSWORD', 'Contraseña Workflow', { secret: true });
  await collectConfirmation(values, 'DOC43_E2E_ENVIRONMENT_AUTHORIZED', '¿Autoriza este ambiente de pruebas?');
  await collectConfirmation(values, 'DOC43_E2E_EXECUTION_AUTHORIZED', '¿Autoriza crear, editar y eliminar una nota sobre la tarea descartable?');
  await collectValue(values, 'DOC43_E2E_TASK_ID', 'ID de tarea descartable DOC-43');
  const environment = { ...process.env, ...values };
  try {
    const cli = path.resolve(__dirname, '..', 'node_modules', '@playwright', 'test', 'cli.js');
    const result = await runChild(process.execPath, [cli, 'test', 'tests/doc43-notes-ui.spec.cjs', '--grep', '@doc43-notes-ui', '--reporter=list'], path.resolve(__dirname, '..'), environment, { nonInteractiveChild: true, redactOutput: redactChildOutput });
    process.exitCode = result.code;
  } finally {
    for (const key of Object.keys(values)) delete environment[key];
  }
}

main().catch(() => { console.error('La E2E UI DOC-43 se detuvo. No se mostraron secretos.'); process.exitCode = 2; });
