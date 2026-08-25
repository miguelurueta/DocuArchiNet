'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const {
  DOC_REGISTRY,
  executeSequence,
  loadProfile,
  parseArguments,
  playwrightCommand,
  selectedStages,
  validateAuthorizations,
  validateProfile
} = require('../scripts/support/workflow-e2e-orchestrator.cjs');
const { assertDsn, finalActivityMatches, safeFailureMessage } = require('../scripts/support/doc32-e2e-odbc.cjs');
const orchestratorSource = require('node:fs').readFileSync(path.join(__dirname, '..', 'scripts', 'support', 'workflow-e2e-orchestrator.cjs'), 'utf8');

function profile() {
  return {
    doc: 'doc32',
    environment: 'CERTIFICACION',
    baseUrl: 'https://workflow.example.invalid/app/',
    ignoreHttpsErrors: false,
    module: 'GESTOR',
    odbcDsn: 'workflowconta',
    executionTaskId: 100001,
    previewActivityNames: ['Actividad de prueba', 'Otra actividad de prueba'],
    executionActivityName: 'Actividad de prueba',
    executionFinalActivityName: 'Actividad final de prueba',
    concurrencyTaskId: 100002,
    concurrencyActivityName: 'Otra actividad de prueba',
    taskStateSql: 'SELECT estado FROM tarea WHERE id_tarea = ?',
    auditSql: 'SELECT total FROM auditoria_doc32 WHERE id_tarea = ?',
    previewMaxMs: 10000,
    executionMaxMs: 15000,
    concurrencyMaxMs: 15000
  };
}

test('el perfil DOC-32 acepta solo configuración no sensible registrada', () => {
  const definition = validateProfile(profile(), 'doc32');
  assert.equal(definition, DOC_REGISTRY.doc32);
  assert.throws(() => validateProfile({ ...profile(), password: 'valor-prohibido' }, 'doc32'), /clave no permitida/);
  assert.throws(() => validateProfile({ ...profile(), command: 'valor-prohibido' }, 'doc32'), /clave no permitida/);
  assert.throws(() => validateProfile({ ...profile(), ignoreHttpsErrors: 'true' }, 'doc32'), /debe ser booleano/);
  assert.throws(() => validateProfile({ ...profile(), baseUrl: 'https://usuario:valor-prohibido@example.invalid/' }, 'doc32'), /sin credenciales/);
  assert.throws(() => validateProfile({ ...profile(), executionActivityName: ' ' }, 'doc32'), /nombre no sensible de una actividad/);
  assert.throws(() => validateProfile({ ...profile(), previewActivityNames: [] }, 'doc32'), /actividades no sensibles/);
  assert.throws(() => validateProfile({ ...profile(), previewActivityNames: ['Actividad de prueba', 'Actividad de prueba'] }, 'doc32'), /actividades repetidas/);
  assert.throws(() => validateProfile({ ...profile(), executionFinalActivityName: ' ' }, 'doc32'), /nombre no sensible de una actividad/);
  assert.throws(() => validateProfile({ ...profile(), concurrencyActivityName: ' ' }, 'doc32'), /nombre no sensible de una actividad/);
  assert.throws(() => validateProfile({ ...profile(), concurrencyTaskId: 100001 }, 'doc32'), /deben ser distintas/);
});

test('el perfil controla de forma explícita la excepción TLS no sensible', () => {
  const authorizations = new Set(['environment', 'execution', 'concurrency']);
  assert.equal(DOC_REGISTRY.doc32.environment({ ...profile(), ignoreHttpsErrors: true }, {}, authorizations).DOC32_E2E_IGNORE_HTTPS_ERRORS, 'true');
  assert.equal(DOC_REGISTRY.doc32.environment(profile(), {}, authorizations).DOC32_E2E_IGNORE_HTTPS_ERRORS, 'false');
  assert.equal(DOC_REGISTRY.doc32.environment(profile(), {}, authorizations).DOC32_E2E_EXECUTION_ACTIVITY_NAME, 'Actividad de prueba');
  assert.equal(DOC_REGISTRY.doc32.environment(profile(), {}, authorizations).DOC32_E2E_EXECUTION_FINAL_ACTIVITY_NAME, 'Actividad final de prueba');
  assert.equal(DOC_REGISTRY.doc32.environment(profile(), {}, authorizations).DOC32_E2E_CONCURRENCY_ACTIVITY_NAME, 'Otra actividad de prueba');
  assert.deepEqual(JSON.parse(DOC_REGISTRY.doc32.environment(profile(), {}, authorizations).DOC32_E2E_PREVIEW_ACTIVITY_NAMES), ['Actividad de prueba', 'Otra actividad de prueba']);
});

test('la plantilla DOC-32 enlaza recursos de ejecución y concurrencia distintos mediante el contrato registrado', async () => {
  const templatePath = path.join(__dirname, '..', 'profiles', 'workflow-e2e.profile.example.json');
  const template = JSON.parse(await fs.readFile(templatePath, 'utf8'));
  const definition = validateProfile(template, 'doc32');
  assert.equal(definition.resourceContract.id, 'doc32-workflow-task');
  assert.equal(definition.stages.find((stage) => stage.id === 'execution').resourceRole, 'execution');
  assert.equal(definition.stages.find((stage) => stage.id === 'concurrency').resourceRole, 'concurrency');
  assert.notEqual(template.executionTaskId, template.concurrencyTaskId);
});

test('el lector acepta JSON de perfil con BOM de PowerShell', async () => {
  const directory = await fs.mkdtemp(path.join(os.tmpdir(), 'doc32-e2e-profile-'));
  const profilePath = path.join(directory, 'profile.json');
  try {
    await fs.writeFile(profilePath, `\uFEFF${JSON.stringify(profile())}`, 'utf8');
    assert.deepEqual(await loadProfile(profilePath), profile());
  } finally {
    await fs.rm(directory, { recursive: true, force: true });
  }
});

test('los argumentos requieren autorizaciones explícitas fuera del perfil', () => {
  const argumentsResult = parseArguments([
    '--doc', 'doc32',
    '--profile', 'C:\\cert\\contet.txt',
    '--authorize', 'environment,execution,concurrency'
  ]);
  assert.deepEqual([...argumentsResult.authorizations].sort(), ['concurrency', 'environment', 'execution']);
  const definition = validateProfile(profile(), 'doc32');
  validateAuthorizations(definition, argumentsResult.authorizations);
  assert.throws(() => validateAuthorizations(definition, new Set(['environment'])), /Falta autorización explícita/);
  const unknownAuthorization = parseArguments(['--doc', 'doc32', '--profile', 'perfil.json', '--authorize', 'environment,unknown']);
  assert.throws(() => validateAuthorizations(definition, unknownAuthorization.authorizations), /no reconocida/);
});

test('una invocación puede limitarse a etapas iniciales sin omitir sus autorizaciones', () => {
  const definition = validateProfile(profile(), 'doc32');
  const selected = selectedStages(definition, ['preview', 'execution']);

  assert.deepEqual(selected.map((stage) => stage.id), ['preview', 'execution']);
  validateAuthorizations(selected, new Set(['environment', 'execution']));
  assert.throws(() => validateAuthorizations(selected, new Set(['environment'])), /Falta autorización explícita para execution/);
  assert.throws(() => selectedStages(definition, ['preview', 'unknown']), /etapa E2E no registrada/);
  assert.deepEqual(parseArguments(['--doc', 'doc32', '--profile', 'perfil.json', '--stages', 'preview,execution', '--authorize', 'environment,execution']).stages, ['preview', 'execution']);
});

test('las etapas Playwright invocan el CLI JavaScript con Node, no el shim .cmd', () => {
  const launch = playwrightCommand('preview');
  assert.equal(launch.command, process.execPath);
  assert.match(launch.args[0], /node_modules[\\/]@playwright[\\/]test[\\/]cli\.js$/);
  assert.deepEqual(launch.args.slice(1), ['test', 'tests/doc32-return-activity.spec.cjs', '--grep', '@doc32-preview', '--reporter=list']);
  assert.match(orchestratorSource, /nonInteractiveChild:\s*true/);
});

test('el destino DOC-32 solo acepta un DSN ODBC no sensible', () => {
  assert.equal(assertDsn({ DOC32_E2E_ODBC_DSN: 'workflowconta' }), 'workflowconta');
  assert.throws(() => assertDsn({ DOC32_E2E_ODBC_DSN: 'workflowconta;PWD=prohibido' }), /DSN ODBC permitido/);
});

test('el diagnóstico ODBC conserva detalles internos fuera de la salida', () => {
  assert.match(safeFailureMessage({ stderr: 'DOC32_ODBC_COLUMN_UNAVAILABLE' }), /columna no disponible/i);
  assert.match(safeFailureMessage({ stderr: 'DOC32_ODBC_TABLE_UNAVAILABLE' }), /tabla requerida/i);
  assert.match(safeFailureMessage({ stderr: 'DOC32_ODBC_QUERY_UNSUPPORTED' }), /no admite/i);
  assert.match(safeFailureMessage({ stderr: 'DOC32_ODBC_RESULT_FAILED' }), /materializar/i);
  assert.match(safeFailureMessage({ stderr: 'DOC32_ODBC_FINGERPRINT_FAILED' }), /generar su huella/i);
  assert.match(safeFailureMessage({ stderr: 'DOC32_ODBC_OPEN_FAILED' }), /abrir el control ODBC/i);
  assert.match(safeFailureMessage({ stderr: 'DOC32_ODBC_QUERY_FAILED' }), /consulta SELECT/i);
  assert.doesNotMatch(safeFailureMessage({ stderr: 'valor-interno' }), /valor-interno/i);
});

test('el control ODBC de actividad final solo expone coincidencia segura', () => {
  assert.equal(finalActivityMatches('DOC32_ODBC_FINAL_ACTIVITY_MATCH'), true);
  assert.equal(finalActivityMatches('DOC32_ODBC_FINAL_ACTIVITY_MISMATCH'), false);
  assert.equal(finalActivityMatches('DOC32_ODBC_FINAL_ACTIVITY_AMBIGUOUS'), false);
  assert.throws(() => finalActivityMatches('valor-interno'), /No se mostraron credenciales, destino ni detalles internos/i);
});

test('la secuencia DOC-32 comparte secretos efímeros, conserva el orden y los elimina', async () => {
  const definition = validateProfile(profile(), 'doc32');
  const authorizations = new Set(['environment', 'execution', 'concurrency']);
  const events = [];
  let childEnvironment;
  await executeSequence({
    definition,
    profile: profile(),
    authorizations,
    assertIntegrity: async () => events.push('integrity'),
    collectSecrets: async () => ({
      DOC32_E2E_AUTHORIZED_USER: 'usuario-simulado',
      DOC32_E2E_AUTHORIZED_PASSWORD: 'valor-sensible-simulado',
      DOC32_E2E_MYSQL_USER: 'lector-simulado',
      DOC32_E2E_MYSQL_PASSWORD: 'valor-sensible-simulado'
    }),
    stageRunner: async (stage, environment) => {
      childEnvironment = environment;
      events.push(stage.id);
      assert.equal(environment.DOC32_E2E_ENVIRONMENT_AUTHORIZED, 'true');
      assert.equal(environment.DOC32_E2E_EXECUTION_AUTHORIZED, 'true');
      assert.equal(environment.DOC32_E2E_CONCURRENCY_AUTHORIZED, 'true');
      return { code: 0 };
    }
  });
  assert.deepEqual(events, ['integrity', 'preview', 'execution', 'concurrency', 'integrity']);
  assert.equal(Object.hasOwn(childEnvironment, 'DOC32_E2E_AUTHORIZED_PASSWORD'), false);
  assert.equal(Object.hasOwn(childEnvironment, 'DOC32_E2E_MYSQL_USER'), false);
  assert.equal(Object.hasOwn(childEnvironment, 'DOC32_E2E_MYSQL_PASSWORD'), false);
});

test('una etapa fallida bloquea las siguientes y ejecuta el cierre', async () => {
  const definition = validateProfile(profile(), 'doc32');
  const events = [];
  await assert.rejects(() => executeSequence({
    definition,
    profile: profile(),
    authorizations: new Set(['environment', 'execution', 'concurrency']),
    assertIntegrity: async () => events.push('integrity'),
    collectSecrets: async () => ({ DOC32_E2E_AUTHORIZED_PASSWORD: 'valor-sensible-simulado' }),
    stageRunner: async (stage) => {
      events.push(stage.id);
      return { code: 1 };
    }
  }), /preview no se completó/);
  assert.deepEqual(events, ['integrity', 'preview', 'integrity']);
});

test('una corrida parcial no inicia la concurrencia no solicitada', async () => {
  const definition = validateProfile(profile(), 'doc32');
  const events = [];
  await executeSequence({
    definition,
    profile: profile(),
    authorizations: new Set(['environment', 'execution']),
    stages: selectedStages(definition, ['preview', 'execution']),
    assertIntegrity: async () => {},
    collectSecrets: async () => ({}),
    stageRunner: async (stage) => {
      events.push(stage.id);
      return { code: 0 };
    }
  });
  assert.deepEqual(events, ['preview', 'execution']);
});
