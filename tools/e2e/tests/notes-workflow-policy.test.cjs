'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');
const config = read('scripts', 'assert-notes-workflow-config.cjs');
const spec = read('tests', 'notes-workflow.spec.cjs');
const support = read('tests', 'support', 'notes-workflow-e2e.cjs');
const runner = read('scripts', 'run-notes-workflow-concurrency.cjs');
const interactiveRunner = read('scripts', 'run-notes-workflow-interactive.cjs');
const interactiveConsole = read('scripts', 'support', 'interactive-e2e-console.cjs');
const packageJson = read('package.json');
const runbook = read('AGENT-RUNBOOK.md');
const loginSelectors = /ContentPlacenter_(?:DropDownListmodulos|TextBoxuser|TextBoxpasw)|a\.da-login-submit/;

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
  assert.match(runbook, /## Notas Workflow/);
  assert.match(runbook, /test:notes:write/);
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
