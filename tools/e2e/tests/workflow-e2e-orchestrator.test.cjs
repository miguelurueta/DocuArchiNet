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
  doc33PlaywrightCommand,
  doc36PlaywrightCommand,
  playwrightCommand,
  selectedStages,
  validateAuthorizations,
  validateProfile
} = require('../scripts/support/workflow-e2e-orchestrator.cjs');
const { createDoc33Profile, parseArguments: parseDoc33ProfileArguments } = require('../scripts/create-doc33-workflow-ui-profile.cjs');
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

function doc33Profile() {
  return {
    doc: 'doc33',
    environment: 'CERTIFICACION',
    baseUrl: 'https://workflow.example.invalid/app/',
    ignoreHttpsErrors: false,
    module: 'GESTOR',
    odbcDsn: 'workflowconta',
    uiExecutionTaskId: 100002,
    previewActivityNames: ['Actividad de prueba', 'Otra actividad de prueba'],
    uiExecutionActivityName: 'Actividad de prueba',
    uiExecutionFinalActivityName: 'Actividad final de prueba',
    uiLockTaskId: 100001,
    uiLockActivityName: 'Actividad de prueba',
    uiLockFinalActivityName: 'Actividad final de prueba',
    taskStateSql: 'SELECT estado FROM tarea WHERE id_tarea = ?',
    auditSql: 'SELECT total FROM auditoria_doc33 WHERE id_tarea = ?',
    previewMaxMs: 10000,
    uiExecutionMaxMs: 15000,
    uiLockMaxMs: 180000
  };
}

function doc36Profile() {
  return {
    doc: 'doc36',
    environment: 'CERTIFICACION',
    baseUrl: 'https://workflow.example.invalid/app/',
    ignoreHttpsErrors: false,
    module: 'GESTOR',
    odbcDsn: 'workflowconta',
    executionTaskId: 100003,
    previewActivityNames: ['Usuario histórico de prueba'],
    executionActivityName: 'Usuario histórico de prueba',
    executionFinalActivityName: 'Usuario histórico de prueba',
    concurrencyTaskId: 100004,
    concurrencyActivityName: 'Usuario histórico de concurrencia',
    taskStateSql: 'SELECT estado FROM tarea WHERE id_tarea = ?',
    auditSql: 'SELECT total FROM auditoria_doc36 WHERE id_tarea = ?',
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

test('el perfil DOC-33 mantiene ejecución UI y bloqueo UI en tareas distintas', async () => {
  const definition = validateProfile(doc33Profile(), 'doc33');
  const templatePath = path.join(__dirname, '..', 'profiles', 'doc33-workflow-ui.profile.example.json');
  const template = JSON.parse(await fs.readFile(templatePath, 'utf8'));
  assert.equal(definition, DOC_REGISTRY.doc33);
  assert.equal(definition.resourceContract.id, 'doc33-workflow-ui-task');
  assert.equal(definition.stages.find((stage) => stage.id === 'execution').resourceRole, 'execution');
  assert.equal(definition.stages.find((stage) => stage.id === 'ui-lock').resourceRole, 'ui-lock');
  assert.notEqual(doc33Profile().uiExecutionTaskId, doc33Profile().uiLockTaskId);
  assert.equal(validateProfile(template, 'doc33').resourceContract.id, 'doc33-workflow-ui-task');
  assert.throws(() => validateProfile({ ...doc33Profile(), uiLockTaskId: 100002 }, 'doc33'), /deben ser distintas/);
  assert.throws(() => validateProfile({ ...doc33Profile(), uiLockActivityName: ' ' }, 'doc33'), /nombre no sensible de una actividad/);
});

test('el perfil DOC-36 registra preview, ejecución y concurrencia con recursos aislados', async () => {
  const templatePath = path.join(__dirname, '..', 'profiles', 'doc36-workflow-user-previous.profile.example.json');
  const template = JSON.parse(await fs.readFile(templatePath, 'utf8'));
  const definition = validateProfile(doc36Profile(), 'doc36');
  assert.equal(definition, DOC_REGISTRY.doc36);
  assert.equal(definition.resourceContract.id, 'doc36-workflow-user-previous-task');
  assert.deepEqual(definition.stages.map((stage) => stage.id), ['preview', 'execution', 'concurrency']);
  assert.equal(definition.stages.find((stage) => stage.id === 'execution').resourceRole, 'execution');
  assert.equal(definition.stages.find((stage) => stage.id === 'concurrency').resourceRole, 'concurrency');
  assert.equal(validateProfile(template, 'doc36').resourceContract.id, 'doc36-workflow-user-previous-task');
  assert.throws(() => validateProfile({ ...doc36Profile(), concurrencyTaskId: 100003 }, 'doc36'), /deben ser distintas/);
  assert.throws(() => validateProfile({ ...doc36Profile(), previewActivityNames: ['Usuario histórico de prueba', 'Usuario histórico de prueba'] }, 'doc36'), /actividades repetidas/);
});

test('el creador DOC-33 migra solo el perfil DOC-32 no sensible y exige dos tareas', () => {
  const migrated = createDoc33Profile(profile(), 100002, 100001);
  const migratedWithObservedFinalActivity = createDoc33Profile(profile(), 100002, 100001, 'Actividad final observada');
  assert.equal(migrated.doc, 'doc33');
  assert.equal(migrated.uiExecutionTaskId, 100002);
  assert.equal(migrated.uiLockTaskId, 100001);
  assert.equal(migrated.uiExecutionActivityName, profile().concurrencyActivityName);
  assert.equal(migrated.uiLockActivityName, profile().executionActivityName);
  assert.equal(migratedWithObservedFinalActivity.uiExecutionFinalActivityName, 'Actividad final observada');
  assert.throws(() => parseDoc33ProfileArguments(['--source', 'source.json', '--destination', 'target.json', '--execution-task', '100002', '--lock-task', '100002']), /deben ser distintas/);
  assert.throws(() => createDoc33Profile(profile(), 100002, 100001, ' '), /nombre no sensible de actividad/);
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
  const doc33Launch = doc33PlaywrightCommand('lock');
  assert.equal(doc33Launch.command, process.execPath);
  assert.deepEqual(doc33Launch.args.slice(1), ['test', 'tests/doc33-return-activity-ui.spec.cjs', '--grep', '@doc33-ui-lock', '--reporter=list']);
  const doc36Launch = doc36PlaywrightCommand('execute');
  assert.equal(doc36Launch.command, process.execPath);
  assert.deepEqual(doc36Launch.args.slice(1), ['test', 'tests/doc36-return-user-previous.spec.cjs', '--grep', '@doc36-execute', '--reporter=list']);
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

test('una corrida DOC-33 puede separar ejecución UI y bloqueo UI', async () => {
  const definition = validateProfile(doc33Profile(), 'doc33');
  const events = [];
  await executeSequence({
    definition,
    profile: doc33Profile(),
    authorizations: new Set(['environment', 'execution']),
    stages: selectedStages(definition, ['preview', 'execution']),
    assertIntegrity: async () => {},
    collectSecrets: async () => ({}),
    stageRunner: async (stage, environment) => {
      events.push(stage.id);
      assert.equal(environment.DOC33_E2E_UI_EXECUTION_TASK_ID, '100002');
      assert.equal(environment.DOC33_E2E_UI_LOCK_TASK_ID, '100001');
      assert.equal(environment.DOC33_E2E_EXECUTION_AUTHORIZED, 'true');
      assert.equal(environment.DOC33_E2E_UI_LOCK_AUTHORIZED, 'false');
      return { code: 0 };
    }
  });
  assert.deepEqual(events, ['preview', 'execution']);
  validateAuthorizations(selectedStages(definition, ['ui-lock']), new Set(['environment', 'ui_lock']));
});

test('una corrida DOC-36 propaga autorizaciones y elimina secretos al cierre', async () => {
  const definition = validateProfile(doc36Profile(), 'doc36');
  const events = [];
  let childEnvironment;
  await executeSequence({
    definition,
    profile: doc36Profile(),
    authorizations: new Set(['environment', 'execution']),
    stages: selectedStages(definition, ['preview', 'execution']),
    assertIntegrity: async () => {},
    collectSecrets: async () => ({
      DOC36_E2E_AUTHORIZED_USER: 'usuario-simulado',
      DOC36_E2E_AUTHORIZED_PASSWORD: 'valor-sensible-simulado',
      DOC36_E2E_MYSQL_USER: 'lector-simulado',
      DOC36_E2E_MYSQL_PASSWORD: 'valor-sensible-simulado'
    }),
    stageRunner: async (stage, environment) => {
      events.push(stage.id);
      childEnvironment = environment;
      assert.equal(environment.DOC36_E2E_ENVIRONMENT_AUTHORIZED, 'true');
      assert.equal(environment.DOC36_E2E_EXECUTION_AUTHORIZED, 'true');
      assert.equal(environment.DOC36_E2E_CONCURRENCY_AUTHORIZED, 'false');
      assert.equal(environment.DOC36_E2E_EXECUTION_TASK_ID, '100003');
      return { code: 0 };
    }
  });
  assert.deepEqual(events, ['preview', 'execution']);
  assert.equal(Object.hasOwn(childEnvironment, 'DOC36_E2E_AUTHORIZED_PASSWORD'), false);
  assert.equal(Object.hasOwn(childEnvironment, 'DOC36_E2E_MYSQL_USER'), false);
  assert.equal(Object.hasOwn(childEnvironment, 'DOC36_E2E_MYSQL_PASSWORD'), false);
});
