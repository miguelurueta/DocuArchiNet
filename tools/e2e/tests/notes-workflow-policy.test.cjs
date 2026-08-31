'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const repositoryRoot = path.resolve(root, '..', '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');
const config = read('scripts', 'assert-notes-workflow-config.cjs');
const spec = read('tests', 'notes-workflow.spec.cjs');
const support = read('tests', 'support', 'notes-workflow-e2e.cjs');
const runner = read('scripts', 'run-notes-workflow-concurrency.cjs');
const interactiveRunner = read('scripts', 'run-notes-workflow-interactive.cjs');
const interactiveConsole = read('scripts', 'support', 'interactive-e2e-console.cjs');
const packageJson = read('package.json');
const runbook = read('AGENT-RUNBOOK.md');
const notesAsmx = fs.readFileSync(path.join(repositoryRoot, 'webservice', 'WebServiceWorkflowNotesModern.asmx'), 'utf8');
const notesAsmxCodeBehind = fs.readFileSync(path.join(repositoryRoot, 'webservice', 'WebServiceWorkflowNotesModern.asmx.vb'), 'utf8');
const loginSelectors = /ContentPlacenter_(?:DropDownListmodulos|TextBoxuser|TextBoxpasw)|a\.da-login-submit/;
const { redactChildOutput } = require('../scripts/support/interactive-e2e-console.cjs');

function readConfiguration(overrides = {}) {
  return {
    PATH: process.env.PATH,
    NOTES_E2E_BASE_URL: 'https://workflow.example.invalid/app/',
    NOTES_E2E_MODULE: 'GESTOR',
    NOTES_E2E_AUTHORIZED_USER: 'cuenta-prueba',
    NOTES_E2E_AUTHORIZED_PASSWORD: 'secreto-prueba',
    NOTES_E2E_ENVIRONMENT: 'CERTIFICACION',
    NOTES_E2E_ENVIRONMENT_AUTHORIZED: 'true',
    NOTES_E2E_ODBC_DSN: 'workflowconta',
    NOTES_E2E_MYSQL_USER: 'lectura',
    NOTES_E2E_MYSQL_PASSWORD: 'secreto-lectura',
    NOTES_E2E_TASK_STATE_SQL: 'SELECT estado FROM tarea WHERE id_tarea = ?',
    NOTES_E2E_AUDIT_SQL: 'SELECT total FROM auditoria WHERE id_tarea = ?',
    NOTES_E2E_READ_TASK_ID: '708',
    NOTES_E2E_READ_MAX_MS: '10000',
    ...overrides
  };
}

test('Notas: configuración bloquea secretos, autorización o tareas ausentes sin imprimir valores', () => {
  const result = spawnSync(process.execPath, [path.join(root, 'scripts', 'assert-notes-workflow-config.cjs'), 'write'], {
    env: { PATH: process.env.PATH },
    encoding: 'utf8'
  });
  assert.equal(result.status, 2);
  assert.match(result.stderr, /Faltan variables Notes E2E requeridas/);
  assert.doesNotMatch(result.stderr, /undefined|null|mysql:\/\//i);
  assert.match(config, /NOTES_E2E_ENVIRONMENT_AUTHORIZED/);
  assert.match(config, /NOTES_E2E_EXECUTION_AUTHORIZED/);
  assert.match(config, /NOTES_E2E_CONCURRENCY_AUTHORIZED/);
  assert.match(config, /NOTES_E2E_CONCURRENCY_NOTE_ID/);
  assert.match(config, /No se mostraron valores/);
});

test('Notas: reutiliza el DSN ODBC y rechaza URLs con credenciales o MySQL', () => {
  const accepted = spawnSync(process.execPath, [path.join(root, 'scripts', 'assert-notes-workflow-config.cjs'), 'read'], {
    env: readConfiguration(),
    encoding: 'utf8'
  });
  assert.equal(accepted.status, 0);
  const rejected = spawnSync(process.execPath, [path.join(root, 'scripts', 'assert-notes-workflow-config.cjs'), 'read'], {
    env: readConfiguration({ NOTES_E2E_MYSQL_URL: 'mysql://valor-prohibido' }),
    encoding: 'utf8'
  });
  assert.equal(rejected.status, 2);
  assert.match(rejected.stderr, /NOTES_E2E_MYSQL_URL no está permitida/);
  const credentialUrl = spawnSync(process.execPath, [path.join(root, 'scripts', 'assert-notes-workflow-config.cjs'), 'read'], {
    env: readConfiguration({ NOTES_E2E_BASE_URL: 'https://usuario:clave@workflow.example.invalid/app/' }),
    encoding: 'utf8'
  });
  assert.equal(credentialUrl.status, 2);
  assert.match(credentialUrl.stderr, /NOTES_E2E_BASE_URL debe ser una URL absoluta válida y sin credenciales/);
  assert.match(config, /NOTES_E2E_ODBC_DSN/);
  assert.match(config, /NOTES_E2E_MYSQL_USER/);
  assert.match(config, /NOTES_E2E_MYSQL_PASSWORD/);
  assert.match(interactiveRunner, /NOTES_E2E_ODBC_DSN = 'workflowconta'/);
  assert.match(interactiveRunner, /const defaultBaseUrl = 'https:\/\/localhost\/GestionDocumental-Docuarchi\.net\/'/);
  assert.match(interactiveRunner, /const defaultModule = 'GESTOR'/);
  assert.match(interactiveRunner, /const defaultEnvironment = 'GESTOR'/);
  assert.match(interactiveRunner, /node_modules', '@playwright', 'test', 'cli\.js'/);
  assert.match(interactiveRunner, /command: process\.execPath/);
  assert.doesNotMatch(interactiveRunner, /playwright\.cmd/);
  assert.match(interactiveRunner, /--output', outputDirectory/);
  assert.match(interactiveRunner, /fs\.mkdtemp\(path\.join\(os\.tmpdir\(\), 'notes-workflow-e2e-'/);
  assert.match(interactiveRunner, /fs\.rm\(outputDirectory, \{ recursive: true, force: true \}\)/);
  assert.match(interactiveRunner, /Usuario MySQL de solo lectura/);
  assert.doesNotMatch(interactiveRunner, /URL MySQL de solo lectura/);
  assert.doesNotMatch(interactiveRunner, /collectValue\(values, 'NOTES_E2E_(?:BASE_URL|MODULE|ENVIRONMENT)'/);
  assert.match(interactiveRunner, /FROM ANOTACION_TAREA WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = \? ORDER BY ID_ANOTACION/);
  assert.match(interactiveRunner, /FROM wf_log_workflow WHERE ID_TAREA_WORKFLOW = \? AND descripcion_opcion = 'NOTA WORKFLOW'/);
  assert.doesNotMatch(interactiveRunner, /collectValue\(values, 'NOTES_E2E_(?:TASK_STATE_SQL|AUDIT_SQL)'/);
  assert.doesNotMatch(interactiveRunner, /DATO_ANOTACION|datos_operacion/);
  for (const source of [spec, runner]) {
    assert.match(source, /NOTES_E2E_ODBC_DSN/);
    assert.doesNotMatch(source, /mysql2\/promise|createPool|NOTES_E2E_MYSQL_URL/);
  }
  assert.match(support, /queryOdbcFingerprint/);
  assert.match(support, /request\.newContext\(/);
  assert.match(support, /storageState: await context\.storageState\(\)/);
  assert.match(support, /ignoreHTTPSErrors: true/);
  assert.doesNotMatch(support, /context\.request\.post/);
  assert.match(runbook, /DSN ODBC no sensible `workflowconta`/);
});

test('Notas: sanea cookies, autorización y contraseñas de la salida de subprocesos', () => {
  const output = redactChildOutput('cookie: session=ejemplo\nAuthorization: Bearer ejemplo\npassword: ejemplo\n.ASPXAUTH=ejemplo');
  assert.doesNotMatch(output, /session=ejemplo|Bearer ejemplo|password: ejemplo|\.ASPXAUTH=ejemplo/);
  assert.match(output, /\[oculto\]/);
});

test('Notas: solo reutiliza el helper de sesión autenticada compartido', () => {
  for (const source of [support, spec, runner]) {
    assert.match(source, /createAuthenticatedWorkflowSession|login\(/);
    assert.doesNotMatch(source, loginSelectors);
    assert.doesNotMatch(source, /page\.locator\([^)]*(?:password|user|module)/i);
  }
  assert.match(support, /createAuthenticatedWorkflowSession/);
});

test('Notas: controles de datos son SELECT de un parámetro y concurrencia fija de dos solicitudes', () => {
  for (const source of [config, support]) {
    assert.match(source, /SELECT\\b|SELECT\b/i);
    assert.match(source, /\?\/g/);
    assert.match(source, /INSERT\|UPDATE\|DELETE\|CALL/);
  }
  assert.match(runner, /assertReadOnlySql/);
  assert.match(runner, /Promise\.all\(contexts\.map/);
  assert.match(runner, /solicitudes:\s*2/);
  assert.doesNotMatch(runner, /(?:CONCURRENCY_LEVEL|VIRTUAL_USERS|LOAD_LEVEL|for\s*\(.*requests)/i);
});

test('Notas: evidencia saneada, gate y comandos quedan integrados al arnés', () => {
  assert.match(support, /password\|cookie\|token\|destino\|usuario\|mysql\|connection/i);
  for (const source of [spec, runner]) {
    assert.match(source, /writeEvidence/);
    assert.match(source, /assertLocalGateOff/);
  }
  assert.match(support, /assertLegacyPagesUnchanged/);
  assert.match(packageJson, /test:notes:anonymous/);
  assert.match(packageJson, /test:notes:read/);
  assert.match(packageJson, /test:notes:write/);
  assert.match(packageJson, /test:notes:concurrency/);
  assert.match(packageJson, /test:workflow:platform/);
  assert.match(packageJson, /"test:notes:read": "node scripts\/run-notes-workflow-interactive\.cjs read"/);
  assert.match(runbook, /## Notas Workflow/);
  assert.match(runbook, /test:notes:write/);
});

test('Notas: DOC-41 usa el ASMX especializado para las tres lecturas y conserva la E2E de solo lectura', () => {
  assert.match(notesAsmx, /WebServiceWorkflowNotesModern/);
  for (const operation of ['ListarNotas', 'ConsultarNota', 'ContarNotas']) {
    assert.match(notesAsmxCodeBehind, new RegExp(`Public Function ${operation}\\(`));
  }
  const readScenario = spec.match(/test\('@notes-read[\s\S]*?\n\}\);/);
  assert.ok(readScenario, 'falta el escenario E2E de lectura autorizada.');
  assert.doesNotMatch(readScenario[0], /CrearNota|ActualizarNota|EliminarNota/);
  assert.match(spec, /assertLocalGateOff/);
  assert.match(readScenario[0], /queryFingerprint/);
});

test('Notas: el iniciador solicita configuración por TTY y no persiste secretos', () => {
  const result = spawnSync(process.execPath, [path.join(root, 'scripts', 'run-notes-workflow-interactive.cjs'), 'read'], {
    stdio: ['pipe', 'pipe', 'pipe'],
    encoding: 'utf8'
  });
  assert.equal(result.status, 2);
  assert.match(result.stderr, /consola interactiva/i);
  assert.match(interactiveRunner, /interactive-e2e-console/);
  assert.match(interactiveConsole, /process\.stdin\.isTTY/);
  assert.match(interactiveConsole, /promptSecret/);
  assert.match(interactiveConsole, /setRawMode\(true\)/);
  assert.match(interactiveRunner, /delete environment\[name\]/);
  assert.doesNotMatch(interactiveConsole, /dotenv|setx|writeFile|appendFile/i);
  assert.match(packageJson, /run-notes-workflow-interactive\.cjs read/);
});
