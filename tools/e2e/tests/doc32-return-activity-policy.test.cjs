'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');
const config = read('scripts', 'assert-doc32-return-activity-config.cjs');
const spec = read('tests', 'doc32-return-activity.spec.cjs');
const runner = read('scripts', 'run-doc32-return-activity-concurrency.cjs');
const interactiveRunner = read('scripts', 'run-doc32-return-activity-interactive.cjs');
const interactiveConsole = read('scripts', 'support', 'interactive-e2e-console.cjs');
const workflowRunner = read('scripts', 'run-workflow-e2e.cjs');
const workflowOrchestrator = read('scripts', 'support', 'workflow-e2e-orchestrator.cjs');
const resourceLifecycle = read('scripts', 'support', 'e2e-test-resource-lifecycle.cjs');
const doc32ResourceAdapter = read('scripts', 'support', 'doc32-e2e-resource-adapter.cjs');
const odbcSupport = read('scripts', 'support', 'doc32-e2e-odbc.cjs');
const odbcFinalActivity = read('scripts', 'query-doc32-odbc-final-activity.ps1');
const odbcReturnDiagnostic = read('scripts', 'inspect-doc32-return-query.ps1');
const workflowProfileTemplate = read('profiles', 'workflow-e2e.profile.example.json');
const packageJson = read('package.json');
const loginSelectors = /ContentPlacenter_(?:DropDownListmodulos|TextBoxuser|TextBoxpasw)|a\.da-login-submit/;

test('DOC-32: configuración bloquea autorizaciones o secretos ausentes sin imprimir valores', () => {
  const result = spawnSync(process.execPath, [path.join(root, 'scripts', 'assert-doc32-return-activity-config.cjs'), 'execute'], {
    env: { PATH: process.env.PATH },
    encoding: 'utf8'
  });
  assert.equal(result.status, 2);
  assert.match(result.stderr, /Faltan variables DOC-32 requeridas/);
  assert.doesNotMatch(result.stderr, /undefined|null|mysql:\/\//i);
  assert.match(config, /No se mostraron valores/);
  assert.match(config, /DOC32_E2E_ENVIRONMENT_AUTHORIZED/);
  assert.match(config, /DOC32_E2E_EXECUTION_AUTHORIZED/);
  assert.match(config, /DOC32_E2E_CONCURRENCY_AUTHORIZED/);
});

test('DOC-32: solo reutiliza el helper de sesión autenticada compartido', () => {
  for (const source of [spec, runner]) {
    assert.match(source, /createAuthenticatedWorkflowSession/);
    assert.doesNotMatch(source, loginSelectors);
    assert.doesNotMatch(source, /page\.locator\([^)]*(?:password|user|module)/i);
  }
});

test('DOC-32: controles ODBC solo aceptan SELECT de un parámetro y la carrera es fija', () => {
  for (const source of [config, spec, runner]) {
    assert.match(source, /SELECT\\b|SELECT\\b/i);
    assert.match(source, /\?\/g/);
    assert.match(source, /INSERT\|UPDATE\|DELETE\|CALL/);
  }
  assert.match(runner, /solicitudes:\s*2/);
  assert.match(runner, /Promise\.all\(contexts\.map/);
  assert.doesNotMatch(runner, /(?:CONCURRENCY_LEVEL|VIRTUAL_USERS|LOAD_LEVEL|for\s*\(.*requests)/i);
  for (const source of [spec, runner]) {
    assert.doesNotMatch(source, /console\.(?:log|error)\([^)]*(?:error|err)\b/i);
  }
  assert.match(odbcSupport, /No fue posible abrir el control ODBC de solo lectura/);
  assert.match(odbcSupport, /no pudo ejecutar la consulta SELECT de solo lectura/);
  assert.doesNotMatch(odbcSupport, /console\.(?:log|error)\([^)]*(?:error|err)\b/i);
  assert.match(odbcSupport, /queryFinalActivity/);
});

test('DOC-32: la actividad final se controla por ODBC sin exponer su nombre', () => {
  assert.match(odbcFinalActivity, /SELECT actividad\.NOMBRE_ACTIVIDAD/);
  assert.match(odbcFinalActivity, /DOC32_ODBC_FINAL_ACTIVITY_(?:MATCH|MISMATCH|AMBIGUOUS)/);
  assert.match(odbcFinalActivity, /Fecha_Fin IS NULL/);
  assert.match(odbcFinalActivity, /ORDER BY estado\.id_Estado DESC\s+LIMIT 1/);
  assert.doesNotMatch(odbcFinalActivity, /(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i);
  assert.doesNotMatch(odbcFinalActivity, /Write-(?:Output|Host).*?(?:activity|password|pwd|connectionstring)/i);
});

test('DOC-32: el diagnóstico ODBC de devolución no muestra filas ni secretos', () => {
  assert.match(odbcReturnDiagnostic, /Read-Host 'Contraseña MySQL de solo lectura' -AsSecureString/);
  assert.match(odbcReturnDiagnostic, /DOC32_RETURN_(?:FLOW|ROUTE)_QUERY_OK/);
  assert.match(odbcReturnDiagnostic, /DOC32_RETURN_FLOW_PREREQUISITES_CLEAR/);
  assert.match(odbcReturnDiagnostic, /DOC32_RETURN_FLOW_PREREQUISITE_(?:AUTHORIZATION|SIGNATURE_AUTHORIZATION|FULL_DOCUMENT_COPY|DOCUMENT_COPY|SIGNATURE|EXPEDIENT)_CONFIGURED/);
  assert.match(odbcReturnDiagnostic, /DOC32_RETURN_\$stage`_(?:FAILED|COLUMN_UNAVAILABLE|TABLE_UNAVAILABLE|SQL_UNSUPPORTED)/);
  assert.match(odbcReturnDiagnostic, /Find-MissingFlowColumn/);
  assert.match(odbcReturnDiagnostic, /INFORMATION_SCHEMA\.COLUMNS/);
  assert.match(odbcReturnDiagnostic, /DOC32_RETURN_FLOW_COLUMN_MISSING_/);
  assert.match(odbcReturnDiagnostic, /UNKNOWN_COLUMN_\$safeIdentifier/);
  assert.match(odbcReturnDiagnostic, /\[A-Za-z0-9_\]\+\(\?:\\\.\[A-Za-z0-9_\]\+\)\?/);
  assert.match(odbcReturnDiagnostic, /Sólo valida el esquema y la forma de la consulta: nunca muestra filas/);
  assert.match(odbcReturnDiagnostic, /WHERE estado\.Inicio_Tareas_Workflow_id_Tarea = \?/);
  assert.doesNotMatch(odbcReturnDiagnostic, /\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i);
  assert.doesNotMatch(odbcReturnDiagnostic, /Write-(?:Output|Host).*?(?:password|pwd|connectionstring)/i);
});

test('DOC-32: evidencia no serializa secretos, token ni destinos y los comandos están registrados', () => {
  for (const source of [spec, runner]) {
    assert.match(source, /password\|cookie\|token\|destino\|usuario\|mysql\|connection/i);
    assert.match(source, /estadoAntes/);
    assert.match(source, /auditoriaAntes/);
    assert.doesNotMatch(source, /writeEvidence\([^)]*,\s*(?:preview|result|payload|dto)\)/);
  }
  assert.match(packageJson, /test:doc32:preview/);
  assert.match(packageJson, /test:doc32:execute/);
  assert.match(packageJson, /test:doc32:concurrency/);
});

test('DOC-32: los comandos reciben secretos efímeros desde TTY y no los persisten', () => {
  const result = spawnSync(process.execPath, [path.join(root, 'scripts', 'run-doc32-return-activity-interactive.cjs'), 'execute'], {
    stdio: ['pipe', 'pipe', 'pipe'],
    encoding: 'utf8'
  });
  assert.equal(result.status, 2);
  assert.match(result.stderr, /consola interactiva/i);
  assert.match(interactiveRunner, /interactive-e2e-console/);
  assert.match(interactiveConsole, /promptSecret/);
  assert.match(interactiveConsole, /setRawMode\(true\)/);
  assert.doesNotMatch(interactiveConsole, /dotenv|setx|writeFile|appendFile/i);
  assert.match(packageJson, /run-doc32-return-activity-interactive\.cjs execute/);
  assert.match(packageJson, /run-doc32-return-activity-interactive\.cjs concurrency/);
});

test('DOC-32: el orquestador reutilizable rechaza perfiles sensibles y no incorpora un almacén de secretos', () => {
  assert.match(packageJson, /test:workflow:run/);
  assert.match(workflowRunner, /executeFromArguments/);
  assert.match(workflowOrchestrator, /requireInteractiveConsole/);
  assert.match(workflowOrchestrator, /sensitiveKey/);
  assert.match(workflowOrchestrator, /cleanEnvironment/);
  assert.doesNotMatch(workflowOrchestrator, /CredentialManager|WindowsCredential|setx|writeFile|appendFile/i);
  assert.match(workflowProfileTemplate, /"odbcDsn"/);
  assert.match(workflowProfileTemplate, /"previewActivityNames"/);
  assert.match(workflowProfileTemplate, /"executionActivityName"/);
  assert.match(workflowProfileTemplate, /"executionFinalActivityName"/);
  assert.match(workflowProfileTemplate, /"concurrencyActivityName"/);
  assert.match(workflowProfileTemplate, /"ignoreHttpsErrors": false/);
  assert.match(workflowProfileTemplate, /estados_tarea_workflow/);
  assert.match(workflowProfileTemplate, /log_usuario/);
  assert.match(workflowProfileTemplate, /ASMX_DEVOLVER_ACTIVIDAD/);
  assert.doesNotMatch(workflowProfileTemplate, /password|cookie|token|secret|credential|mysql(?:url|connection)|connection|authorized/i);
  assert.match(workflowProfileTemplate, /"doc": "doc32"/);
  assert.match(spec, /DOC32_E2E_EXECUTION_ACTIVITY_NAME/);
  assert.match(spec, /DOC32_E2E_EXECUTION_FINAL_ACTIVITY_NAME/);
  assert.match(runner, /DOC32_E2E_CONCURRENCY_ACTIVITY_NAME/);
  assert.match(spec, /DOC32_E2E_PREVIEW_ACTIVITY_NAMES/);
  assert.match(spec, /NombreActividad/);
  assert.doesNotMatch(spec, /NombreActividadDestino/);
  assert.match(spec, /PreviewDevolverActividad', \{ idTarea, termino: '', cursor: '', tamanoPagina: 50 \}/);
  assert.match(runner, /PreviewDevolverActividad', \{ idTarea, termino: '', cursor: '', tamanoPagina: 50 \}/);
  assert.match(runner, /selectedDestination\(preview, required\('DOC32_E2E_CONCURRENCY_ACTIVITY_NAME'\)\)/);
});

test('E2E: el ciclo de recursos usa contratos registrados, reservas opacas y no ejecuta comportamiento del perfil', () => {
  assert.match(workflowOrchestrator, /createResourceLifecycle/);
  assert.match(workflowOrchestrator, /resourceRole: 'execution'/);
  assert.match(workflowOrchestrator, /resourceRole: 'concurrency'/);
  assert.match(resourceLifecycle, /validateRegisteredResourceContracts/);
  assert.match(resourceLifecycle, /opaqueHash/);
  assert.match(resourceLifecycle, /E2E_RESOURCE_SHARED_COORDINATOR_REQUIRED/);
  assert.match(resourceLifecycle, /assertNonSensitiveDescriptor/);
  assert.match(resourceLifecycle, /writeResourceLifecycleEvidence/);
  assert.doesNotMatch(resourceLifecycle, /child_process|execFile|spawn\(/);
  assert.doesNotMatch(resourceLifecycle, /console\.(?:log|error)/);
  assert.match(doc32ResourceAdapter, /DOC32_RESOURCE_CONTRACT/);
  assert.match(doc32ResourceAdapter, /queryFingerprint/);
  assert.match(doc32ResourceAdapter, /E2E_RESOURCE_DESTINATION_UNAVAILABLE/);
  assert.doesNotMatch(doc32ResourceAdapter, /EjecutarDevolverActividad|\.post\(|invoke\(/);
});
