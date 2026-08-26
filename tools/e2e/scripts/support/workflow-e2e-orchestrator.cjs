'use strict';

const { execFileSync } = require('node:child_process');
const fs = require('node:fs/promises');
const path = require('node:path');
const {
  collectValue,
  requireInteractiveConsole,
  runChild
} = require('./interactive-e2e-console.cjs');
const { assertDsn } = require('./doc32-e2e-odbc.cjs');
const {
  createResourceLifecycle,
  validateRegisteredResourceContracts,
  writeResourceLifecycleEvidence
} = require('./e2e-test-resource-lifecycle.cjs');
const { DOC32_RESOURCE_CONTRACT } = require('./doc32-e2e-resource-adapter.cjs');
const { DOC33_RESOURCE_CONTRACT } = require('./doc33-e2e-resource-adapter.cjs');
const { DOC36_RESOURCE_CONTRACT } = require('./doc36-e2e-resource-adapter.cjs');

const e2eRoot = path.resolve(__dirname, '..', '..');
const repositoryRoot = path.resolve(e2eRoot, '..', '..');
const sensitiveKey = /(passw(?:ord)?|cookie|token|secret|credential|credencial|connection|conexion|authorization|authorized|usuario|user)/i;
const forbiddenSql = /;|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b/i;

function playwrightCommand(mode) {
  // Do not spawn the Windows .cmd shim directly: with shell disabled Node can
  // reject it with EINVAL before Playwright receives the test command.
  const cli = path.resolve(e2eRoot, 'node_modules', '@playwright', 'test', 'cli.js');
  return {
    command: process.execPath,
    args: [cli, 'test', 'tests/doc32-return-activity.spec.cjs', '--grep', `@doc32-${mode}`, '--reporter=list']
  };
}

function doc33PlaywrightCommand(mode) {
  const cli = path.resolve(e2eRoot, 'node_modules', '@playwright', 'test', 'cli.js');
  return {
    command: process.execPath,
    args: [cli, 'test', 'tests/doc33-return-activity-ui.spec.cjs', '--grep', `@doc33-ui-${mode}`, '--reporter=list']
  };
}

function doc36PlaywrightCommand(mode) {
  const cli = path.resolve(e2eRoot, 'node_modules', '@playwright', 'test', 'cli.js');
  return {
    command: process.execPath,
    args: [cli, 'test', 'tests/doc36-return-user-previous.spec.cjs', '--grep', `@doc36-${mode}`, '--reporter=list']
  };
}

const DOC_REGISTRY = Object.freeze({
  doc32: Object.freeze({
    resourceContract: DOC32_RESOURCE_CONTRACT,
    profileKeys: Object.freeze([
      'doc',
      'environment',
      'baseUrl',
      'ignoreHttpsErrors',
      'module',
      'odbcDsn',
      'executionTaskId',
      'previewActivityNames',
      'executionActivityName',
      'executionFinalActivityName',
      'concurrencyTaskId',
      'concurrencyActivityName',
      'taskStateSql',
      'auditSql',
      'previewMaxMs',
      'executionMaxMs',
      'concurrencyMaxMs'
    ]),
    stages: Object.freeze([
      Object.freeze({ id: 'preview', authorizations: Object.freeze(['environment']), launch: () => playwrightCommand('preview') }),
      Object.freeze({ id: 'execution', resourceRole: 'execution', authorizations: Object.freeze(['environment', 'execution']), launch: () => playwrightCommand('execute') }),
      Object.freeze({
        id: 'concurrency',
        resourceRole: 'concurrency',
        authorizations: Object.freeze(['environment', 'execution', 'concurrency']),
        launch: () => ({ command: process.execPath, args: [path.resolve(e2eRoot, 'scripts', 'run-doc32-return-activity-concurrency.cjs')] })
      })
    ]),
    async collectSecrets() {
      requireInteractiveConsole();
      const values = {};
      await collectValue(values, 'DOC32_E2E_AUTHORIZED_USER', 'Cuenta Workflow autorizada');
      await collectValue(values, 'DOC32_E2E_AUTHORIZED_PASSWORD', 'Contraseña Workflow', { secret: true });
      await collectValue(values, 'DOC32_E2E_MYSQL_USER', 'Usuario MySQL de solo lectura');
      await collectValue(values, 'DOC32_E2E_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
      return values;
    },
    environment(profile, secrets, authorizations) {
      return {
        DOC32_E2E_BASE_URL: profile.baseUrl,
        DOC32_E2E_IGNORE_HTTPS_ERRORS: profile.ignoreHttpsErrors ? 'true' : 'false',
        DOC32_E2E_MODULE: profile.module,
        DOC32_E2E_ENVIRONMENT: profile.environment,
        DOC32_E2E_ODBC_DSN: profile.odbcDsn,
        DOC32_E2E_EXECUTION_TASK_ID: String(profile.executionTaskId),
        DOC32_E2E_PREVIEW_ACTIVITY_NAMES: JSON.stringify(profile.previewActivityNames.map((name) => name.trim())),
        DOC32_E2E_EXECUTION_ACTIVITY_NAME: profile.executionActivityName.trim(),
        DOC32_E2E_EXECUTION_FINAL_ACTIVITY_NAME: profile.executionFinalActivityName.trim(),
        DOC32_E2E_CONCURRENCY_TASK_ID: String(profile.concurrencyTaskId),
        DOC32_E2E_CONCURRENCY_ACTIVITY_NAME: profile.concurrencyActivityName.trim(),
        DOC32_E2E_TASK_STATE_SQL: profile.taskStateSql,
        DOC32_E2E_AUDIT_SQL: profile.auditSql,
        DOC32_E2E_PREVIEW_MAX_MS: String(profile.previewMaxMs),
        DOC32_E2E_EXECUTION_MAX_MS: String(profile.executionMaxMs),
        DOC32_E2E_CONCURRENCY_MAX_MS: String(profile.concurrencyMaxMs),
        DOC32_E2E_ENVIRONMENT_AUTHORIZED: authorizations.has('environment') ? 'true' : 'false',
        DOC32_E2E_EXECUTION_AUTHORIZED: authorizations.has('execution') ? 'true' : 'false',
        DOC32_E2E_CONCURRENCY_AUTHORIZED: authorizations.has('concurrency') ? 'true' : 'false',
        ...secrets
      };
    }
  }),
  doc33: Object.freeze({
    resourceContract: DOC33_RESOURCE_CONTRACT,
    profileKeys: Object.freeze([
      'doc',
      'environment',
      'baseUrl',
      'ignoreHttpsErrors',
      'module',
      'odbcDsn',
      'uiExecutionTaskId',
      'previewActivityNames',
      'uiExecutionActivityName',
      'uiExecutionFinalActivityName',
      'uiLockTaskId',
      'uiLockActivityName',
      'uiLockFinalActivityName',
      'taskStateSql',
      'auditSql',
      'previewMaxMs',
      'uiExecutionMaxMs',
      'uiLockMaxMs'
    ]),
    stages: Object.freeze([
      Object.freeze({ id: 'preview', authorizations: Object.freeze(['environment']), launch: () => doc33PlaywrightCommand('preview') }),
      Object.freeze({ id: 'execution', resourceRole: 'execution', authorizations: Object.freeze(['environment', 'execution']), launch: () => doc33PlaywrightCommand('execute') }),
      Object.freeze({ id: 'ui-lock', resourceRole: 'ui-lock', authorizations: Object.freeze(['environment', 'ui_lock']), launch: () => doc33PlaywrightCommand('lock') })
    ]),
    async collectSecrets() {
      requireInteractiveConsole();
      const values = {};
      await collectValue(values, 'DOC33_E2E_AUTHORIZED_USER', 'Cuenta Workflow autorizada');
      await collectValue(values, 'DOC33_E2E_AUTHORIZED_PASSWORD', 'Contraseña Workflow', { secret: true });
      await collectValue(values, 'DOC33_E2E_MYSQL_USER', 'Usuario MySQL de solo lectura');
      await collectValue(values, 'DOC33_E2E_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
      return values;
    },
    environment(profile, secrets, authorizations) {
      return {
        DOC33_E2E_BASE_URL: profile.baseUrl,
        DOC33_E2E_IGNORE_HTTPS_ERRORS: profile.ignoreHttpsErrors ? 'true' : 'false',
        DOC33_E2E_MODULE: profile.module,
        DOC33_E2E_ENVIRONMENT: profile.environment,
        DOC33_E2E_ODBC_DSN: profile.odbcDsn,
        DOC33_E2E_UI_EXECUTION_TASK_ID: String(profile.uiExecutionTaskId),
        DOC33_E2E_PREVIEW_ACTIVITY_NAMES: JSON.stringify(profile.previewActivityNames.map((name) => name.trim())),
        DOC33_E2E_UI_EXECUTION_ACTIVITY_NAME: profile.uiExecutionActivityName.trim(),
        DOC33_E2E_UI_EXECUTION_FINAL_ACTIVITY_NAME: profile.uiExecutionFinalActivityName.trim(),
        DOC33_E2E_UI_LOCK_TASK_ID: String(profile.uiLockTaskId),
        DOC33_E2E_UI_LOCK_ACTIVITY_NAME: profile.uiLockActivityName.trim(),
        DOC33_E2E_UI_LOCK_FINAL_ACTIVITY_NAME: profile.uiLockFinalActivityName.trim(),
        DOC33_E2E_TASK_STATE_SQL: profile.taskStateSql,
        DOC33_E2E_AUDIT_SQL: profile.auditSql,
        DOC33_E2E_PREVIEW_MAX_MS: String(profile.previewMaxMs),
        DOC33_E2E_UI_EXECUTION_MAX_MS: String(profile.uiExecutionMaxMs),
        DOC33_E2E_UI_LOCK_MAX_MS: String(profile.uiLockMaxMs),
        DOC33_E2E_ENVIRONMENT_AUTHORIZED: authorizations.has('environment') ? 'true' : 'false',
        DOC33_E2E_EXECUTION_AUTHORIZED: authorizations.has('execution') ? 'true' : 'false',
        DOC33_E2E_UI_LOCK_AUTHORIZED: authorizations.has('ui_lock') ? 'true' : 'false',
        ...secrets
      };
    }
  }),
  doc36: Object.freeze({
    resourceContract: DOC36_RESOURCE_CONTRACT,
    profileKeys: Object.freeze([
      'doc',
      'environment',
      'baseUrl',
      'ignoreHttpsErrors',
      'module',
      'odbcDsn',
      'executionTaskId',
      'previewActivityNames',
      'executionActivityName',
      'executionFinalActivityName',
      'concurrencyTaskId',
      'concurrencyActivityName',
      'taskStateSql',
      'auditSql',
      'previewMaxMs',
      'executionMaxMs',
      'concurrencyMaxMs'
    ]),
    stages: Object.freeze([
      Object.freeze({ id: 'preview', authorizations: Object.freeze(['environment']), launch: () => doc36PlaywrightCommand('preview') }),
      Object.freeze({ id: 'execution', resourceRole: 'execution', authorizations: Object.freeze(['environment', 'execution']), launch: () => doc36PlaywrightCommand('execute') }),
      Object.freeze({
        id: 'concurrency',
        resourceRole: 'concurrency',
        authorizations: Object.freeze(['environment', 'execution', 'concurrency']),
        launch: () => ({ command: process.execPath, args: [path.resolve(e2eRoot, 'scripts', 'run-doc36-return-user-previous-concurrency.cjs')] })
      })
    ]),
    async collectSecrets() {
      requireInteractiveConsole();
      const values = {};
      await collectValue(values, 'DOC36_E2E_AUTHORIZED_USER', 'Cuenta Workflow autorizada');
      await collectValue(values, 'DOC36_E2E_AUTHORIZED_PASSWORD', 'Contraseña Workflow', { secret: true });
      await collectValue(values, 'DOC36_E2E_MYSQL_USER', 'Usuario MySQL de solo lectura');
      await collectValue(values, 'DOC36_E2E_MYSQL_PASSWORD', 'Contraseña MySQL de solo lectura', { secret: true });
      return values;
    },
    environment(profile, secrets, authorizations) {
      return {
        DOC36_E2E_BASE_URL: profile.baseUrl,
        DOC36_E2E_IGNORE_HTTPS_ERRORS: profile.ignoreHttpsErrors ? 'true' : 'false',
        DOC36_E2E_MODULE: profile.module,
        DOC36_E2E_ENVIRONMENT: profile.environment,
        DOC36_E2E_ODBC_DSN: profile.odbcDsn,
        DOC36_E2E_EXECUTION_TASK_ID: String(profile.executionTaskId),
        DOC36_E2E_PREVIEW_ACTIVITY_NAMES: JSON.stringify(profile.previewActivityNames.map((name) => name.trim())),
        DOC36_E2E_EXECUTION_ACTIVITY_NAME: profile.executionActivityName.trim(),
        DOC36_E2E_EXECUTION_FINAL_ACTIVITY_NAME: profile.executionFinalActivityName.trim(),
        DOC36_E2E_CONCURRENCY_TASK_ID: String(profile.concurrencyTaskId),
        DOC36_E2E_CONCURRENCY_ACTIVITY_NAME: profile.concurrencyActivityName.trim(),
        DOC36_E2E_TASK_STATE_SQL: profile.taskStateSql,
        DOC36_E2E_AUDIT_SQL: profile.auditSql,
        DOC36_E2E_PREVIEW_MAX_MS: String(profile.previewMaxMs),
        DOC36_E2E_EXECUTION_MAX_MS: String(profile.executionMaxMs),
        DOC36_E2E_CONCURRENCY_MAX_MS: String(profile.concurrencyMaxMs),
        DOC36_E2E_ENVIRONMENT_AUTHORIZED: authorizations.has('environment') ? 'true' : 'false',
        DOC36_E2E_EXECUTION_AUTHORIZED: authorizations.has('execution') ? 'true' : 'false',
        DOC36_E2E_CONCURRENCY_AUTHORIZED: authorizations.has('concurrency') ? 'true' : 'false',
        ...secrets
      };
    }
  })
});

validateRegisteredResourceContracts({
  [DOC32_RESOURCE_CONTRACT.id]: DOC32_RESOURCE_CONTRACT,
  [DOC33_RESOURCE_CONTRACT.id]: DOC33_RESOURCE_CONTRACT,
  [DOC36_RESOURCE_CONTRACT.id]: DOC36_RESOURCE_CONTRACT
});

function fail(message) {
  throw new Error(message);
}

function assertPositiveInteger(value, field) {
  if (!Number.isSafeInteger(value) || value <= 0) fail(`${field} debe ser un entero positivo.`);
}

function assertActivityName(value, field) {
  if (typeof value !== 'string' || value.trim().length < 2 || value.trim().length > 160 || /[\u0000\r\n]/.test(value)) {
    fail(`${field} debe ser el nombre no sensible de una actividad Workflow.`);
  }
}

function assertActivityNames(value, field) {
  if (!Array.isArray(value) || value.length === 0 || value.length > 50) {
    fail(`${field} debe contener entre una y 50 actividades no sensibles.`);
  }
  const names = new Set();
  for (const activityName of value) {
    assertActivityName(activityName, field);
    const normalized = activityName.normalize('NFKC').trim().toLocaleLowerCase();
    if (names.has(normalized)) fail(`${field} no admite actividades repetidas.`);
    names.add(normalized);
  }
}

function assertReadOnlySql(sql, field) {
  if (typeof sql !== 'string' || !/^\s*SELECT\b/i.test(sql) || forbiddenSql.test(sql) || (sql.match(/\?/g) || []).length !== 1) {
    fail(`${field} debe ser una única consulta SELECT de solo lectura con exactamente un parámetro ?.`);
  }
}

function assertAbsoluteHttpUrl(value, field) {
  try {
    const url = new URL(value);
    if (!/^https?:$/.test(url.protocol) || url.username || url.password) throw new Error('invalid');
  } catch {
    fail(`${field} debe ser una URL HTTP(S) absoluta sin credenciales.`);
  }
}

function parseArguments(argv) {
  const result = { doc: null, profilePath: null, authorizations: new Set(), stages: null };
  for (let index = 0; index < argv.length; index += 1) {
    const argument = argv[index];
    if (argument === '--doc' || argument === '--profile' || argument === '--authorize' || argument === '--stages') {
      const value = argv[index + 1];
      if (!value || value.startsWith('--')) fail(`${argument} requiere un valor.`);
      index += 1;
      if (argument === '--doc') result.doc = value.trim().toLowerCase();
      if (argument === '--profile') result.profilePath = value;
      if (argument === '--authorize') {
        for (const entry of value.split(',')) {
          const authorization = entry.trim().toLowerCase();
          if (!authorization) fail('--authorize no admite valores vacíos.');
          result.authorizations.add(authorization);
        }
      }
      if (argument === '--stages') {
        const stages = value.split(',').map((entry) => entry.trim().toLowerCase());
        if (stages.some((stage) => !stage) || new Set(stages).size !== stages.length) fail('--stages no admite valores vacíos ni repetidos.');
        result.stages = stages;
      }
      continue;
    }
    fail(`Argumento no reconocido: ${argument}.`);
  }
  if (!result.doc || !result.profilePath || result.authorizations.size === 0) {
    fail('Use --doc, --profile y --authorize para iniciar una corrida E2E Workflow.');
  }
  return result;
}

async function loadProfile(profilePath) {
  let raw;
  try {
    raw = await fs.readFile(profilePath, 'utf8');
  } catch {
    fail('No fue posible leer el perfil E2E indicado.');
  }
  try {
    // Windows PowerShell commonly writes UTF-8 JSON with a BOM. It is not a
    // sensitive value and accepting it keeps manually maintained profiles
    // portable without relaxing the strict schema validation below.
    const profile = JSON.parse(raw.replace(/^\uFEFF/, ''));
    if (!profile || Array.isArray(profile) || typeof profile !== 'object') fail('El perfil E2E debe ser un objeto JSON.');
    return profile;
  } catch (error) {
    if (error instanceof SyntaxError) fail('El perfil E2E no contiene JSON válido.');
    throw error;
  }
}

function validateProfile(profile, docName) {
  const definition = DOC_REGISTRY[docName];
  if (!definition) fail(`DOC E2E no registrado: ${docName}.`);
  const keys = Object.keys(profile);
  for (const key of keys) {
    if (sensitiveKey.test(key) || !definition.profileKeys.includes(key)) fail('El perfil contiene una clave no permitida.');
  }
  for (const key of definition.profileKeys) {
    if (!Object.hasOwn(profile, key)) fail(`Falta el campo no sensible requerido: ${key}.`);
  }
  if (profile.doc !== docName) fail('El DOC del perfil no coincide con --doc.');
  if (typeof profile.environment !== 'string' || !profile.environment.trim()) fail('environment es obligatoria.');
  if (typeof profile.module !== 'string' || !profile.module.trim()) fail('module es obligatoria.');
  if (typeof profile.ignoreHttpsErrors !== 'boolean') fail('ignoreHttpsErrors debe ser booleano.');
  assertAbsoluteHttpUrl(profile.baseUrl, 'baseUrl');
  assertDsn({ [`${docName.toUpperCase()}_E2E_ODBC_DSN`]: profile.odbcDsn }, `${docName.toUpperCase()}_E2E`);
  if (docName === 'doc33') {
    assertPositiveInteger(profile.uiExecutionTaskId, 'uiExecutionTaskId');
    assertActivityNames(profile.previewActivityNames, 'previewActivityNames');
    assertActivityName(profile.uiExecutionActivityName, 'uiExecutionActivityName');
    assertActivityName(profile.uiExecutionFinalActivityName, 'uiExecutionFinalActivityName');
    assertPositiveInteger(profile.uiLockTaskId, 'uiLockTaskId');
    assertActivityName(profile.uiLockActivityName, 'uiLockActivityName');
    assertActivityName(profile.uiLockFinalActivityName, 'uiLockFinalActivityName');
    if (profile.uiExecutionTaskId === profile.uiLockTaskId) fail('Las tareas de ejecución y bloqueo UI deben ser distintas.');
    assertReadOnlySql(profile.taskStateSql, 'taskStateSql');
    assertReadOnlySql(profile.auditSql, 'auditSql');
    assertPositiveInteger(profile.previewMaxMs, 'previewMaxMs');
    assertPositiveInteger(profile.uiExecutionMaxMs, 'uiExecutionMaxMs');
    assertPositiveInteger(profile.uiLockMaxMs, 'uiLockMaxMs');
    return definition;
  }
  assertPositiveInteger(profile.executionTaskId, 'executionTaskId');
  assertActivityNames(profile.previewActivityNames, 'previewActivityNames');
  assertActivityName(profile.executionActivityName, 'executionActivityName');
  assertActivityName(profile.executionFinalActivityName, 'executionFinalActivityName');
  assertPositiveInteger(profile.concurrencyTaskId, 'concurrencyTaskId');
  assertActivityName(profile.concurrencyActivityName, 'concurrencyActivityName');
  if (profile.executionTaskId === profile.concurrencyTaskId) fail('Las tareas de ejecución y concurrencia deben ser distintas.');
  assertReadOnlySql(profile.taskStateSql, 'taskStateSql');
  assertReadOnlySql(profile.auditSql, 'auditSql');
  assertPositiveInteger(profile.previewMaxMs, 'previewMaxMs');
  assertPositiveInteger(profile.executionMaxMs, 'executionMaxMs');
  assertPositiveInteger(profile.concurrencyMaxMs, 'concurrencyMaxMs');
  return definition;
}

function selectedStages(definition, requestedStages) {
  if (!requestedStages) return definition.stages;
  const requested = new Set(requestedStages);
  const stages = definition.stages.filter((stage) => requested.has(stage.id));
  if (stages.length !== requested.size) fail('La selección contiene una etapa E2E no registrada para el DOC.');
  return stages;
}

function validateAuthorizations(definitionOrStages, authorizations) {
  const stages = Array.isArray(definitionOrStages) ? definitionOrStages : definitionOrStages.stages;
  const required = new Set(stages.flatMap((stage) => stage.authorizations || [stage.authorization]));
  for (const authorization of authorizations) {
    if (!required.has(authorization)) fail(`Autorización no reconocida: ${authorization}.`);
  }
  for (const authorization of required) {
    if (!authorizations.has(authorization)) fail(`Falta autorización explícita para ${authorization}.`);
  }
}

async function assertWorkflowIntegrity(root = repositoryRoot) {
  const configPath = path.join(root, 'Web.config');
  const configuration = await fs.readFile(configPath, 'utf8');
  const gateIsOff = /<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i.test(configuration) &&
    /<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i.test(configuration) &&
    /<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i.test(configuration);
  if (!gateIsOff) fail('El gate Workflow debe permanecer apagado y sin alcance.');
  const changedLegacyPages = execFileSync('git', ['diff', '--name-only', '--', 'workflow/Webworkflow.aspx', 'workflow/Webworkflow.aspx.vb'], {
    cwd: root,
    encoding: 'utf8'
  }).trim();
  if (changedLegacyPages) fail('Las páginas legacy de Workflow tienen cambios locales.');
}

function cleanEnvironment(environment, names) {
  for (const name of names) {
    if (Object.hasOwn(environment, name)) {
      environment[name] = '';
      delete environment[name];
    }
  }
}

async function executeSequence({
  definition,
  profile,
  authorizations,
  stages = definition.stages,
  assertIntegrity = assertWorkflowIntegrity,
  collectSecrets,
  stageRunner,
  resourceLifecycleFactory = null,
  evidenceWriter = writeResourceLifecycleEvidence
}) {
  await assertIntegrity();
  const secrets = await collectSecrets();
  const environment = { ...process.env, ...definition.environment(profile, secrets, authorizations) };
  const secretNames = Object.keys(secrets);
  let lifecycle = null;
  try {
    lifecycle = resourceLifecycleFactory ? await resourceLifecycleFactory({ definition, profile, environment, stages }) : null;
    for (const stage of stages) {
      const reservation = stage.resourceRole && lifecycle ? await lifecycle.prepare(stage.resourceRole) : null;
      if (stage.resourceRole && lifecycle && !reservation) fail(`No fue posible reservar el recurso E2E para ${stage.id}.`);
      const result = await stageRunner(stage, environment);
      if (!result || result.code !== 0) fail(`La etapa ${stage.id} no se completó; no se iniciarán etapas posteriores.`);
      if (reservation) await lifecycle.finalize(reservation, true);
    }
  } finally {
    try {
      await lifecycle?.close();
    } finally {
      try {
        if (lifecycle) {
          await evidenceWriter({
            root: repositoryRoot,
            doc: profile.doc,
            contractId: definition.resourceContract.id,
            events: lifecycle.evidence()
          });
        }
      } finally {
        cleanEnvironment(environment, secretNames);
        await assertIntegrity();
      }
    }
  }
}

async function executeFromArguments(argv) {
  const args = parseArguments(argv);
  const profile = await loadProfile(args.profilePath);
  const definition = validateProfile(profile, args.doc);
  const stages = selectedStages(definition, args.stages);
  validateAuthorizations(stages, args.authorizations);
  await executeSequence({
    definition,
    profile,
    authorizations: args.authorizations,
    stages,
    collectSecrets: () => definition.collectSecrets(),
    resourceLifecycleFactory: ({ environment }) => createResourceLifecycle({
      contract: definition.resourceContract,
      profile,
      environment
    }),
    stageRunner: async (stage, environment) => {
      const target = stage.launch();
      // The parent retains its TTY only to capture secrets. Playwright itself
      // does not need a terminal, and some legacy login pages fail to finish
      // rendering when the browser child inherits one.
      return runChild(target.command, target.args, e2eRoot, environment, { nonInteractiveChild: true });
    }
  });
}

module.exports = {
  DOC_REGISTRY,
  assertReadOnlySql,
  assertWorkflowIntegrity,
  cleanEnvironment,
  executeFromArguments,
  executeSequence,
  loadProfile,
  parseArguments,
  playwrightCommand,
  doc33PlaywrightCommand,
  doc36PlaywrightCommand,
  selectedStages,
  validateAuthorizations,
  validateProfile
};
