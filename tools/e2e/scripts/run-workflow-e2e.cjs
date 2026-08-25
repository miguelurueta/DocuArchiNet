'use strict';

const { executeFromArguments } = require('./support/workflow-e2e-orchestrator.cjs');

executeFromArguments(process.argv.slice(2))
  .then(() => {
    console.log('Secuencia E2E Workflow completada con evidencia saneada.');
  })
  .catch((error) => {
    console.error(error?.message || 'La secuencia E2E Workflow no pudo iniciarse. No se mostraron valores sensibles.');
    process.exitCode = 2;
  });
