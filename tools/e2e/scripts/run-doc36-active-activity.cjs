'use strict';

const { collectValue, requireInteractiveConsole } = require('./support/interactive-e2e-console.cjs');
const { queryActiveActivity } = require('./support/doc32-e2e-odbc.cjs');

function parseTask(argv) {
  if (argv.length !== 2 || argv[0] !== '--task') throw new Error('Use --task <id>.');
  const taskId = Number(argv[1]);
  if (!Number.isSafeInteger(taskId) || taskId <= 0) throw new Error('La tarea debe ser un entero positivo.');
  return taskId;
}

async function main(argv) {
  const taskId = parseTask(argv);
  requireInteractiveConsole();
  const secrets = {};
  await collectValue(secrets, 'DOC36_E2E_MYSQL_USER', 'Usuario MySQL de solo lectura');
  await collectValue(secrets, 'DOC36_E2E_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
  const environment = {
    ...process.env,
    DOC36_E2E_ODBC_DSN: 'workflowconta',
    ...secrets
  };
  try {
    const activity = await queryActiveActivity(taskId, environment, 'DOC36_E2E');
    process.stdout.write(activity
      ? `DOC36_${taskId}_ACTIVE_ACTIVITY=${activity}\n`
      : `DOC36_${taskId}_ACTIVE_ACTIVITY_AMBIGUOUS\n`);
  } finally {
    for (const name of Object.keys(secrets)) delete environment[name];
  }
}

main(process.argv.slice(2)).catch((error) => {
  process.stderr.write(`${error?.message || 'No fue posible consultar la actividad activa. No se mostraron valores sensibles.'}\n`);
  process.exitCode = 2;
});
