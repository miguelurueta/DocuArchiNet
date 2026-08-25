'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');
const spec = read('tests', 'doc33-return-activity-ui.spec.cjs');
const orchestrator = read('scripts', 'support', 'workflow-e2e-orchestrator.cjs');
const adapter = read('scripts', 'support', 'doc33-e2e-resource-adapter.cjs');
const profileCreator = read('scripts', 'create-doc33-workflow-ui-profile.cjs');
const activeActivityScript = read('scripts', 'query-doc32-odbc-active-activity.ps1');
const activeActivityRunner = read('scripts', 'run-doc33-active-activity.cjs');
const profileTemplate = read('profiles', 'doc33-workflow-ui.profile.example.json');
const ui = read('..', '..', 'js', 'workflow', 'workflow-return-activity-ui.js');
const confirmation = read('..', '..', 'js', 'workflow', 'workflow-return-activity-confirmation.js');

test('DOC-33: la UI conserva bloqueado el modal propio mientras la confirmación ejecuta', () => {
  assert.match(ui, /executionPending/);
  assert.match(ui, /establecerEjecucionPendiente/);
  assert.match(ui, /La devolución está en curso\. Espere la respuesta antes de cerrar\./);
  assert.match(confirmation, /executeWithLock/);
  assert.match(confirmation, /establecerEjecucionPendiente/);
  assert.doesNotMatch(confirmation, /WorkflowUserSend|EjecutarEnvio/);
});

test('DOC-33: la E2E UI reutiliza sesión, separa recursos y no revela respuestas', () => {
  assert.match(spec, /createAuthenticatedWorkflowSession/);
  assert.match(spec, /@doc33-ui-preview/);
  assert.match(spec, /@doc33-ui-execute/);
  assert.match(spec, /@doc33-ui-lock/);
  assert.match(spec, /route\.fetch\(\{ timeout: 180000 \}\)/);
  assert.match(spec, /ConfirmationDialog\.close\(\)/);
  assert.match(spec, /beforeunload/);
  assert.match(spec, /workflow-return-activity-close/);
  assert.match(spec, /getByRole\('link', \{ name: 'Devolver', exact: true \}\)/);
  assert.match(spec, /queryOdbcFingerprint/);
  assert.match(spec, /queryOdbcFinalActivity/);
  assert.match(spec, /queryOdbcActiveActivity/);
  assert.match(spec, /Actividad activa observada/);
  assert.match(spec, /writeEvidence/);
  assert.match(spec, /coincidenciaFinal: finalActivityMatched/);
  assert.doesNotMatch(spec, /observedFinalActivity:/);
  assert.doesNotMatch(spec, /console\.(?:log|error)\(/);
  assert.doesNotMatch(spec, /page\.locator\([^)]*(?:password|user|module)/i);
});

test('DOC-33: perfil y ciclo de recursos requieren dos tareas no sensibles', () => {
  assert.match(orchestrator, /doc33:/);
  assert.match(orchestrator, /uiExecutionTaskId/);
  assert.match(orchestrator, /uiLockTaskId/);
  assert.match(orchestrator, /ui_lock/);
  assert.match(adapter, /DOC33_RESOURCE_CONTRACT/);
  assert.match(adapter, /role === 'ui-lock'/);
  assert.match(profileTemplate, /"doc": "doc33"/);
  assert.match(profileTemplate, /"uiExecutionTaskId"/);
  assert.match(profileTemplate, /"uiLockTaskId"/);
  assert.doesNotMatch(profileTemplate, /password|cookie|token|secret|credential|mysql(?:url|connection)|connection|authorized/i);
  assert.match(profileCreator, /validateProfile\(source, 'doc32'\)/);
  assert.match(profileCreator, /Perfil DOC-33 no sensible creado/);
  assert.doesNotMatch(profileCreator, /promptSecret|setx|CredentialManager|WindowsCredential/i);
});

test('DOC-33: el diagnóstico de actividad activa es ODBC de solo lectura y no revela credenciales', () => {
  assert.match(activeActivityScript, /SELECT actividad\.NOMBRE_ACTIVIDAD/);
  assert.match(activeActivityScript, /WORKFLOW_ODBC_ACTIVE_ACTIVITY=/);
  assert.doesNotMatch(activeActivityScript, /(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i);
  assert.match(activeActivityRunner, /requireInteractiveConsole/);
  assert.match(activeActivityRunner, /queryActiveActivity/);
  assert.match(activeActivityRunner, /cleanEnvironment/);
  assert.doesNotMatch(activeActivityRunner, /setx|writeFile|appendFile|console\.log/i);
});
