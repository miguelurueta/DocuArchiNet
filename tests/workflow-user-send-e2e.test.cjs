'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const repositoryRoot = path.resolve(__dirname, '..');
const e2eRoot = path.join(repositoryRoot, 'tools', 'e2e');

function read(relativePath) {
  return fs.readFileSync(path.join(e2eRoot, relativePath), 'utf8');
}

test('DOC-28 separa sus modos antes de que Playwright pueda crear una sesión', () => {
  const packageJson = JSON.parse(read('package.json'));
  const validator = read('scripts/assert-doc28-user-send-config.cjs');

  for (const [script, mode, tag] of [
    ['test:doc28:anonymous', 'anonymous', '@doc28-anonymous'],
    ['test:doc28:validation', 'validation', '@doc28-validation'],
    ['test:doc28:preview', 'preview', '@doc28-preview'],
    ['test:doc28:execute', 'execute', '@doc28-execute']
  ]) {
    const command = packageJson.scripts[script];
    assert.match(command, new RegExp(`^node scripts/assert-doc28-user-send-config\\.cjs ${mode} && playwright test tests/doc28-user-send\\.spec\\.cjs --grep ${tag} --reporter=list$`));
  }

  assert.match(validator, /anonymous:\s*\['DOC28_E2E_BASE_URL'\]/);
  assert.match(validator, /validation:\s*authenticated/);
  assert.match(validator, /preview:\s*\[/);
  assert.match(validator, /execute:\s*\[/);
  assert.match(validator, /concurrency:\s*\[/);
  assert.match(validator, /DOC28_E2E_EXECUTION_AUTHORIZED\.toLowerCase\(\) !== 'true'/);
  assert.match(validator, /DOC28_E2E_CONCURRENCY_AUTHORIZED\.toLowerCase\(\) !== 'true'/);
  assert.match(validator, /DOC28_E2E_EXPECTED_OUTCOME debe ser success o blocked/);
  assert.doesNotMatch(validator, /(?:playwright|createAuthenticatedWorkflowSession|fetch|axios|child_process)/i);
  assert.doesNotMatch(validator, /DOC28_E2E_(?:DESTINATION|TOKEN)/);
  assert.equal(packageJson.scripts['test:doc28:load'], undefined);
  assert.equal(
    packageJson.scripts['test:doc28:concurrency'],
    'node scripts/assert-doc28-user-send-config.cjs concurrency && node scripts/run-doc28-user-send-concurrency.cjs'
  );
});

test('DOC-28 conserva contratos ASMX exclusivos de destino usuario', () => {
  const service = fs.readFileSync(path.join(repositoryRoot, 'webservice', 'WebServiceWorkflowModern.asmx.vb'), 'utf8');
  const preview = service.match(/Public Function PreviewEnviarUsuario[\s\S]*?End Function/)[0];
  const execute = service.match(/Public Function EjecutarEnvioUsuario[\s\S]*?End Function/)[0];

  assert.match(preview, /ByVal idTarea As Long/);
  assert.match(preview, /ByVal consulta As String/);
  assert.match(preview, /ByVal cursor As String/);
  assert.match(preview, /ByVal tamanoPagina As Integer/);
  assert.match(execute, /ByVal idTarea As Long/);
  assert.match(execute, /ByVal idUsuarioWorkflowDestino As Integer/);
  assert.match(execute, /ByVal idActividadDestino As Integer/);
  assert.match(execute, /ByVal tokenVersion As String/);
  assert.doesNotMatch(preview, /IdConector|Terminar_Tarea_Workflow/);
  assert.doesNotMatch(execute, /IdConector|Terminar_Tarea_Workflow/);
});

test('la suite DOC-28 protege preview, consultas de control y evidencia', () => {
  const suite = read('tests/doc28-user-send.spec.cjs').replace(/\r\n/g, '\n');
  const readOnlySql = suite.match(/function assertReadOnlySql[\s\S]*?\n}\n/)[0];
  const previewEvidence = suite.match(/await writeEvidence\('preview',[\s\S]*?\n  }\);/)[0];
  const executionEvidence = suite.match(/await writeEvidence\('execution',[\s\S]*?\n  }\);/)[0];

  assert.match(suite, /test\('@doc28-anonymous PreviewEnviarUsuario sin sesión/);
  assert.match(suite, /test\('@doc28-validation Una sesión Gestión válida/);
  assert.match(suite, /test\('@doc28-preview PreviewEnviarUsuario preserva estado y auditoría/);
  assert.match(suite, /PreviewEnviarUsuario/);
  assert.match(suite, /idTarea: 0, consulta: '', cursor: '', tamanoPagina: 1/);
  assert.match(suite, /WORKFLOW_CONTEXT_INVALID/);
  assert.match(suite, /WORKFLOW_TASK_INVALID/);
  assert.match(readOnlySql, /SELECT\\b\/i\.test/);
  assert.match(readOnlySql, /INSERT\|UPDATE\|DELETE\|CALL/);
  assert.match(suite, /expect\(afterState, 'El preview no debe modificar estado de tarea\.'/);
  assert.match(suite, /expect\(afterAudit, 'El preview no debe modificar auditoría\.'/);
  assert.match(suite, /assertLocalGateOff/);
  assert.match(suite, /WorkflowCentroTrabajoModernActive/);
  assert.match(suite, /screenshot: 'off'/);
  assert.match(suite, /trace: 'off'/);
  assert.match(suite, /video: 'off'/);
  assert.doesNotMatch(previewEvidence, /TokenVersion|IdUsuarioWorkflowDestino|NombreUsuarioDestino|MYSQL_URL|AUTHORIZED_PASSWORD/);
  assert.doesNotMatch(executionEvidence, /TokenVersion|IdUsuarioWorkflowDestino|NombreUsuarioDestino|MYSQL_URL|AUTHORIZED_PASSWORD/);
});

test('la ejecución DOC-28 depende de un preview vigente y no incluye carga', () => {
  const packageJson = JSON.parse(read('package.json'));
  const suite = read('tests/doc28-user-send.spec.cjs');
  const execution = suite.match(/test\('@doc28-execute[\s\S]*?\n}\);/)[0];

  assert.match(execution, /preview = await invokePreview\(context, idTarea, ''\)/);
  assert.match(execution, /const destination = expectValidDestination\(preview\)/);
  assert.match(execution, /idUsuarioWorkflowDestino: destination\.IdUsuarioWorkflowDestino/);
  assert.match(execution, /idActividadDestino: destination\.IdActividadDestino/);
  assert.match(execution, /tokenVersion: preview\.TokenVersion/);
  assert.match(execution, /DOC28_E2E_EXECUTION_AUTHORIZED\.toLowerCase\(\) !== 'true'/);
  assert.match(execution, /result\.EstadoFinal.*toBe\('completada'\)/);
  assert.match(execution, /El éxito debe reflejar cambio de estado/);
  assert.match(execution, /El éxito debe dejar una huella de auditoría/);
  assert.equal(packageJson.scripts['test:doc28:load'], undefined);
  assert.doesNotMatch(suite, /(?:UPDATE|INSERT|DELETE|CALL)\s+\w+\s/i);
  assert.doesNotMatch(suite, /writeFile\(path\.join\(repositoryRoot, 'Web\.config'/);
});

test('la concurrencia DOC-28 es una carrera fija, autorizada y sin datos sensibles', () => {
  const runner = read('scripts/run-doc28-user-send-concurrency.cjs');
  const evidence = runner.match(/await writeEvidence\(\{[\s\S]*?\n  }\);/)[0];

  assert.match(runner, /DOC28_E2E_EXECUTION_AUTHORIZED/);
  assert.match(runner, /DOC28_E2E_CONCURRENCY_AUTHORIZED/);
  assert.match(runner, /assertRequiredConfiguration\(\);[\s\S]*?chromium\.launch/);
  assert.match(runner, /contexts = \[await login\(browser\), await login\(browser\)\];/);
  assert.match(runner, /const preview = await invokePreview\(contexts\[0\], idTarea\);/);
  assert.match(runner, /idUsuarioWorkflowDestino: destination\.IdUsuarioWorkflowDestino/);
  assert.match(runner, /idActividadDestino: destination\.IdActividadDestino/);
  assert.match(runner, /tokenVersion: preview\.TokenVersion/);
  assert.match(runner, /Promise\.all\(contexts\.map\(\(context\) => invokeExecution\(context, payload\)\)\)/);
  assert.match(runner, /WORKFLOW_TRANSITION_IN_PROGRESS/);
  assert.match(runner, /WORKFLOW_VERSION_CONFLICT/);
  assert.match(runner, /WORKFLOW_TASK_UNAVAILABLE/);
  assert.match(runner, /exitos\.length === 1/);
  assert.match(runner, /estadoFinal === 'completada'/);
  assert.match(runner, /stateChanged && auditChanged/);
  assert.match(runner, /assertLocalGateOff\(\);/);
  assert.doesNotMatch(runner, /process\.env\.DOC28_E2E_(?:DESTINATION|TOKEN)/);
  assert.doesNotMatch(runner, /DOC28_E2E_(?:LOAD|VUSERS|CONCURRENCY_LEVEL)/);
  assert.doesNotMatch(evidence, /MYSQL_URL|AUTHORIZED_PASSWORD|IdUsuarioWorkflowDestino|TokenVersion/);
});

test('la documentación DOC-28 conserva límites de ejecución y cierre', () => {
  const readme = read('README.md');
  const runbook = read('AGENT-RUNBOOK.md');

  assert.match(readme, /test:doc28:anonymous/);
  assert.match(readme, /test:doc28:preview/);
  assert.match(readme, /test:doc28:execute/);
  assert.match(readme, /DOC28_E2E_EXECUTION_AUTHORIZED = 'true'/);
  assert.match(readme, /test:doc28:concurrency/);
  assert.match(readme, /DOC28_E2E_CONCURRENCY_AUTHORIZED = 'true'/);
  assert.match(readme, /log_usuario/);
  assert.match(readme, /ASMX_ENVIO_USUARIO/);
  const auditSql = readme.match(/DOC28_E2E_AUDIT_SQL = "([^"]+)"/)[1];
  assert.match(auditSql, /^SELECT\b/i);
  assert.equal((auditSql.match(/\?/g) || []).length, 1);
  assert.doesNotMatch(auditSql, /;/);
  assert.match(readme, /nunca los recibe por variables de entorno/);
  assert.doesNotMatch(readme, /DOC28_E2E_(?:DESTINATION|TOKEN)/);
  assert.match(runbook, /DOC-28 envío a usuario/);
  assert.match(runbook, /test:doc28:concurrency/);
  assert.match(runbook, /carga masiva/);
  assert.match(runbook, /DOC28_E2E_CONCURRENCY_AUTHORIZED=true/);
  assert.match(runbook, /log_usuario/);
  assert.match(runbook, /wf_log_estados_workflow/);
  assert.match(runbook, /gate siguió apagado/);
  assert.match(runbook, /cuerpos de respuesta/);
});
