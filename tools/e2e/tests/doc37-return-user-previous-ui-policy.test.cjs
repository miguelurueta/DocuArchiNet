'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');
const spec = read('tests', 'doc37-return-user-previous-ui.spec.cjs');
const orchestrator = read('scripts', 'support', 'workflow-e2e-orchestrator.cjs');
const adapter = read('scripts', 'support', 'doc37-e2e-resource-adapter.cjs');
const profileCreator = read('scripts', 'create-doc37-workflow-user-previous-ui-profile.cjs');
const profileTemplate = read('profiles', 'doc37-workflow-user-previous-ui.profile.example.json');
const ui = read('..', '..', 'js', 'workflow', 'workflow-return-user-previous-ui.js');
const confirmation = read('..', '..', 'js', 'workflow', 'workflow-return-user-previous-confirmation.js');

test('DOC-37: la UI y E2E usan exclusivamente los contratos de Usuario anterior', () => {
  assert.match(ui, /PreviewDevolverUsuarioAnterior/);
  assert.match(confirmation, /EjecutarDevolverUsuarioAnterior/);
  assert.match(spec, /@doc37-ui-preview/);
  assert.match(spec, /@doc37-ui-execute/);
  assert.match(spec, /@doc37-ui-lock/);
  assert.match(spec, /workflow-return-user-previous-modern-modal/);
  assert.match(spec, /workflow-return-user-previous-trigger/);
  assert.match(spec, /data-workflow-return-user-previous-close/);
  assert.match(spec, /Object\.keys\(payload\)\.sort\(\)\.join\(','\) === 'idTarea,tokenVersion'/);
  assert.doesNotMatch(spec, /EjecutarDevolverActividad|PreviewDevolverActividad|idConector|idUsuarioDestino|idActividadDestino/i);
});

test('DOC-37: perfil, recursos y etapas reutilizan DOC-36 sin persistir secretos', () => {
  assert.match(orchestrator, /doc37:/);
  assert.match(orchestrator, /DOC37_RESOURCE_CONTRACT/);
  assert.match(orchestrator, /doc37PlaywrightCommand/);
  assert.match(orchestrator, /DOC37_E2E_UI_EXECUTION_TASK_ID/);
  assert.match(orchestrator, /DOC37_E2E_UI_LOCK_TASK_ID/);
  assert.match(orchestrator, /DOC37_E2E_UI_LOCK_AUTHORIZED/);
  assert.match(orchestrator, /una sola etapa por invocación/);
  assert.match(adapter, /DOC37_RESOURCE_CONTRACT/);
  assert.match(adapter, /role === 'ui-lock'/);
  assert.match(profileTemplate, /"doc": "doc37"/);
  assert.match(profileTemplate, /"uiExecutionTaskId"/);
  assert.match(profileTemplate, /"uiLockTaskId"/);
  assert.doesNotMatch(profileTemplate, /password|cookie|token|secret|credential|mysql(?:url|connection)|connection|authorized/i);
  assert.match(profileCreator, /validateProfile\(source, 'doc36'\)/);
  assert.match(profileCreator, /--environment/);
  assert.match(profileCreator, /Perfil DOC-37 no sensible creado/);
  assert.doesNotMatch(profileCreator, /promptSecret|setx|CredentialManager|WindowsCredential/i);
});

test('DOC-37: preview usa controles SELECT y el bloqueo UI limita la mutación a una solicitud', () => {
  assert.match(spec, /assertReadOnlySql/);
  assert.match(spec, /queryOdbcFingerprint/);
  assert.match(spec, /queryOdbcFinalActivity/);
  assert.match(spec, /route\.fetch\(\{ timeout: 180000 \}\)/);
  assert.match(spec, /executionRequests\)\.toBe\(1\)/);
  assert.match(spec, /ConfirmationDialog\.close\(\)/);
  assert.match(spec, /beforeunload/);
  assert.match(spec, /assertLocalGateOff/);
  assert.match(spec, /assertWorkflowPagesCommitted/);
  assert.doesNotMatch(spec, /console\.(?:log|error)\(/);
  assert.doesNotMatch(spec, /endpoint:\s*'(?:Preview|Ejecutar)DevolverUsuarioAnterior'/);
  assert.doesNotMatch(spec, /(?:\bINSERT\b|\bUPDATE\b|\bDELETE\b|\bCREATE\b|\bDROP\b)\s+(?:INTO|SET|TABLE|DATABASE)/i);
});

test('DOC-37: la selección E2E usa el comando oficial antes de las huellas de control', () => {
  assert.match(spec, /async function selectAuthorizedTask/);
  assert.match(spec, /\[tip_event="seleccion_tarea_wf"\]\[idd="\$\{taskId\}"\]/);
  assert.match(spec, /selectCommand\.click\(\)/);
  assert.match(spec, /selectCommand[\s\S]{0,300}toHaveCount\(1\)/);
  assert.doesNotMatch(spec, /Hidden_id_tarea_selecionada[^\n]*(?:\.value\s*=|setAttribute\()/);
  const selection = spec.indexOf('await selectAuthorizedTask(page, taskId);');
  const baseline = spec.indexOf('beforeState = await queryFingerprint(stateSql, taskId);');
  assert.ok(selection >= 0 && baseline > selection, 'La línea base debe tomarse después de la selección oficial.');
});
