'use strict';

const fs = require('node:fs/promises');
const path = require('node:path');
const { execFile } = require('node:child_process');
const { promisify } = require('node:util');
const { createResourceLifecycle } = require('./e2e-test-resource-lifecycle.cjs');
const { resolveAdapter, resolveControls, resolveScenario } = require('./workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('./workflow-e2e-platform-profile.cjs');

const execute = promisify(execFile);
const repositoryRoot = path.resolve(__dirname, '..', '..', '..', '..');
const SENSITIVE_EVIDENCE = /passw(?:ord)?|pwd|cookie|token|secret|credential|credencial|connection|conexion|authorization|authorized|usuario|user|contenido|nota|request|response|mysql|odbc/i;
const SAFE_CODE = /^(?:E2E_PLATFORM|E2E_RESOURCE|NOTES_READ|NOTES_ANONYMOUS)_[A-Z0-9_]{3,100}$/;
const SAFE_STAGE_AUTHORIZATIONS = Object.freeze({
  anonymous: Object.freeze([]),
  read: Object.freeze([]),
  preview: Object.freeze([]),
  execution: Object.freeze(['execution', 'discardable-resource']),
  concurrency: Object.freeze(['execution', 'concurrency', 'discardable-resource']),
  'ui-lock': Object.freeze(['ui-lock', 'discardable-resource'])
});
const SECRET_ENVIRONMENT = Object.freeze({
  'workflow-account': 'WORKFLOW_E2E_PLATFORM_AUTHORIZED_USER',
  'workflow-password': 'WORKFLOW_E2E_PLATFORM_AUTHORIZED_PASSWORD',
  'readonly-db-user': 'NOTES_E2E_MYSQL_USER',
  'readonly-db-password': 'NOTES_E2E_MYSQL_PASSWORD'
});

class PlatformExecutionError extends Error {
  constructor(code) {
    super(`La plataforma E2E detuvo la corrida de forma segura (${code}).`);
    this.name = 'PlatformExecutionError';
    this.code = code;
  }
}

function fail(code) {
  throw new PlatformExecutionError(code);
}

function safeCode(error, fallback = 'E2E_PLATFORM_STAGE_FAILED') {
  return SAFE_CODE.test(error?.code || '') ? error.code : fallback;
}

function normalizeAuthorizations(value) {
  const entries = value instanceof Set ? [...value] : Array.isArray(value) ? value : [];
  const normalized = new Set();
  for (const entry of entries) {
    if (typeof entry !== 'string' || !/^[a-z][a-z0-9-]{1,79}$/.test(entry)) fail('E2E_PLATFORM_AUTHORIZATION_INVALID');
    normalized.add(entry);
  }
  return normalized;
}

function requiredAuthorizationsFor(scenario, profile) {
  const required = [...scenario.requiredAuthorizations, ...SAFE_STAGE_AUTHORIZATIONS[scenario.stage]];
  if (profile.ignoreHttpsErrors) required.push('local-tls');
  return Object.freeze([...new Set(required)]);
}

function preflightPlatform({ profile, authorizations = [] }) {
  const validatedProfile = validateProfile(profile);
  const scenario = resolveScenario(validatedProfile.scenarioId);
  const adapter = resolveAdapter(scenario.adapterId);
  const controls = resolveControls(scenario.controls);
  const acceptedAuthorizations = normalizeAuthorizations(authorizations);
  for (const authorization of requiredAuthorizationsFor(scenario, validatedProfile)) {
    if (!acceptedAuthorizations.has(authorization)) fail('E2E_PLATFORM_AUTHORIZATION_REQUIRED');
  }
  if (scenario.resource?.mutating && !scenario.resource.profileField) fail('E2E_PLATFORM_RESOURCE_INVALID');
  return Object.freeze({
    profile: validatedProfile,
    scenario,
    adapter,
    controls,
    authorizations: acceptedAuthorizations
  });
}

function createManagedLifecycle(options, plan, environment) {
  if (!plan.scenario.resource?.mutating) return null;
  if (typeof options?.resourceLifecycleFactory === 'function') {
    return options.resourceLifecycleFactory({ plan, environment });
  }
  const contractId = plan.scenario.resource.contractId;
  const contract = options?.resourceContracts?.[contractId];
  if (!contract) fail('E2E_PLATFORM_RESOURCE_COORDINATOR_REQUIRED');
  return createResourceLifecycle({ contract, profile: plan.profile, environment, leaseStore: options?.leaseStore });
}

function assertSecretValue(value) {
  return typeof value === 'string' && value.trim().length > 0 && value.length <= 1024 && !/[\r\n\u0000]/.test(value);
}

function createRuntimeEnvironment(plan, secrets) {
  const values = secrets && typeof secrets === 'object' && !Array.isArray(secrets) ? secrets : {};
  const environment = {
    ...process.env,
    WORKFLOW_E2E_PLATFORM_MODULE: plan.profile.module,
    NOTES_E2E_ODBC_DSN: plan.profile.odbcDsn
  };
  for (const secretName of plan.scenario.requiredSecrets) {
    const target = SECRET_ENVIRONMENT[secretName];
    const value = values[secretName];
    if (!target || !assertSecretValue(value)) fail('E2E_PLATFORM_SECRET_REQUIRED');
    environment[target] = value;
  }
  return environment;
}

function eraseSecrets(values, environment) {
  if (values && typeof values === 'object') {
    for (const name of Object.keys(values)) delete values[name];
  }
  if (environment && typeof environment === 'object') {
    for (const name of Object.values(SECRET_ENVIRONMENT)) delete environment[name];
  }
}

function validatePayload(adapter, operation, payload) {
  const allowed = Object.values(adapter.operations).find((entry) => entry.id === operation);
  if (!allowed || !payload || typeof payload !== 'object' || Array.isArray(payload)) fail('E2E_PLATFORM_OPERATION_INVALID');
  const keys = Object.keys(payload).sort();
  const expected = [...allowed.payload].sort();
  if (keys.length !== expected.length || keys.some((key, index) => key !== expected[index])) fail('E2E_PLATFORM_OPERATION_INVALID');
  if (!Number.isSafeInteger(payload.idTarea) || payload.idTarea <= 0) fail('E2E_PLATFORM_OPERATION_INVALID');
  if (Object.hasOwn(payload, 'idNota') && (!Number.isSafeInteger(payload.idNota) || payload.idNota <= 0)) fail('E2E_PLATFORM_OPERATION_INVALID');
  if (Object.hasOwn(payload, 'cursor') && (typeof payload.cursor !== 'string' || payload.cursor.length > 160 || /[\r\n\u0000]/.test(payload.cursor))) fail('E2E_PLATFORM_OPERATION_INVALID');
  if (Object.hasOwn(payload, 'tamanoPagina') && (!Number.isSafeInteger(payload.tamanoPagina) || payload.tamanoPagina < 1 || payload.tamanoPagina > 100)) fail('E2E_PLATFORM_OPERATION_INVALID');
}

function createRestrictedInvoker({ adapter, client, invoke }) {
  if (typeof invoke !== 'function') fail('E2E_PLATFORM_TRANSPORT_INVALID');
  return async (operation, payload) => {
    validatePayload(adapter, operation, payload);
    const result = await invoke({ client, servicePath: adapter.servicePath, operation, payload });
    if (!result || typeof result !== 'object' || !Number.isSafeInteger(result.elapsedMs) || result.elapsedMs < 0 || !result.dto || typeof result.dto !== 'object') {
      fail('E2E_PLATFORM_TRANSPORT_INVALID');
    }
    return result;
  };
}

async function captureControls(controls, plan, environment, readControl) {
  if (controls.length === 0) return Object.freeze({});
  if (typeof readControl !== 'function') fail('E2E_PLATFORM_CONTROL_READER_REQUIRED');
  const fingerprints = {};
  for (const control of controls) {
    const value = await readControl({ control, taskId: plan.profile[plan.scenario.resource.profileField], environment, profile: plan.profile });
    if (typeof value !== 'string' || !/^[a-f0-9]{64}$/i.test(value)) fail('E2E_PLATFORM_CONTROL_FAILED');
    fingerprints[control.id] = value.toLowerCase();
  }
  return Object.freeze(fingerprints);
}

function controlsUnchanged(before, after) {
  return Object.keys(before).every((key) => after && before[key] === after[key]);
}

function createSafeEvidence({ plan, result, before, after, failureCode, resourceEvents }) {
  const codes = {};
  for (const [name, value] of Object.entries(result?.codes || {})) {
    if (!/^[a-z][A-Za-z0-9]{1,79}$/.test(name) || (value !== null && (typeof value !== 'string' || value.length > 100))) {
      fail('E2E_PLATFORM_EVIDENCE_INVALID');
    }
    codes[name] = value;
  }
  const latencies = result?.latenciesMs || [];
  if (!Array.isArray(latencies) || latencies.length > 20 || latencies.some((value) => !Number.isSafeInteger(value) || value < 0 || value > plan.profile.budgetMs)) {
    fail('E2E_PLATFORM_EVIDENCE_INVALID');
  }
  const evidence = {
    scenario: plan.scenario.id,
    stage: plan.scenario.stage,
    success: !failureCode,
    failureCode: failureCode || null,
    controls: Object.freeze({ checked: Object.keys(before || {}).length, unchanged: controlsUnchanged(before || {}, after || {}) }),
    result: Object.freeze({
      codes: Object.freeze(codes),
      count: Number.isSafeInteger(result?.count) && result.count >= 0 ? result.count : 0,
      latenciesMs: Object.freeze([...latencies])
    }),
    resources: Object.freeze((resourceEvents || []).map((event) => ({ role: event.role, phase: event.phase, code: event.code })))
  };
  if (SENSITIVE_EVIDENCE.test(JSON.stringify(evidence))) fail('E2E_PLATFORM_EVIDENCE_INVALID');
  return Object.freeze(evidence);
}

async function closeQuietly(resource, method = 'close') {
  try {
    await resource?.[method]?.();
  } catch {
    // El cierre no debe revelar ni reemplazar el código de una falla previa.
  }
}

async function removeTemporaryDirectory(directory) {
  if (!directory) return;
  const resolved = path.resolve(directory);
  const tempRoot = path.resolve(require('node:os').tmpdir());
  if (path.relative(tempRoot, resolved).startsWith('..')) fail('E2E_PLATFORM_TEMPORARY_PATH_INVALID');
  await fs.rm(resolved, { recursive: true, force: true });
}

async function assertPlatformIntegrity({ root = repositoryRoot } = {}) {
  const configuration = await fs.readFile(path.join(root, 'Web.config'), 'utf8');
  if (!/<add key="WorkflowCentroTrabajoModernActive" value="false"\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernUsers" value=""\s*\/>/i.test(configuration) ||
      !/<add key="WorkflowCentroTrabajoModernGroups" value=""\s*\/>/i.test(configuration)) {
    fail('E2E_PLATFORM_GATE_INTEGRITY_FAILED');
  }
  const result = await execute('git', ['diff', '--name-only', '--', 'workflow/Webworkflow.aspx', 'workflow/Webworkflow.aspx.vb'], { cwd: root, windowsHide: true });
  if (result.stdout.trim()) fail('E2E_PLATFORM_LEGACY_INTEGRITY_FAILED');
}

async function executePlatformRun(options) {
  const plan = preflightPlatform(options || {});
  const collectSecrets = options?.collectSecrets;
  const createBrowser = options?.createBrowser;
  const createSession = options?.createSession;
  const createClient = options?.createClient;
  const invoke = options?.invoke;
  const readControl = options?.readControl;
  const assertIntegrity = options?.assertIntegrity || assertPlatformIntegrity;
  const writeEvidence = options?.writeEvidence;
  let secrets = {};
  let environment;
  let browser;
  let context;
  let client;
  let reservation;
  let lifecycle;
  let before = Object.freeze({});
  let after = null;
  let adapterResult;
  let failure;
  try {
    if (plan.scenario.requiredSecrets.length > 0) {
      if (typeof collectSecrets !== 'function') fail('E2E_PLATFORM_TTY_REQUIRED');
      secrets = await collectSecrets(plan);
    }
    environment = createRuntimeEnvironment(plan, secrets);
    if (plan.scenario.resource?.mutating) {
      lifecycle = createManagedLifecycle(options, plan, environment);
      if (!lifecycle || typeof lifecycle.prepare !== 'function') fail('E2E_PLATFORM_RESOURCE_COORDINATOR_REQUIRED');
      reservation = await lifecycle.prepare(plan.scenario.resource.role);
    }
    before = await captureControls(plan.controls, plan, environment, readControl);
    if (plan.scenario.transport.session === 'workflow') {
      if (typeof createBrowser !== 'function' || typeof createSession !== 'function') fail('E2E_PLATFORM_SESSION_FACTORY_REQUIRED');
      browser = await createBrowser(plan.profile);
      context = await createSession({ browser, plan, environment });
    }
    if (typeof createClient !== 'function') fail('E2E_PLATFORM_TRANSPORT_INVALID');
    client = await createClient({ context, plan });
    const restrictedInvoke = createRestrictedInvoker({ adapter: plan.adapter, client, invoke });
    if (plan.scenario.stage === 'read') {
      adapterResult = await plan.adapter.executeRead({ invoke: restrictedInvoke, taskId: plan.profile.taskId, budgetMs: plan.profile.budgetMs });
    } else if (plan.scenario.stage === 'anonymous' && typeof plan.adapter.executeAnonymous === 'function') {
      adapterResult = await plan.adapter.executeAnonymous({ invoke: restrictedInvoke, budgetMs: plan.profile.budgetMs });
    } else {
      fail('E2E_PLATFORM_STAGE_UNSUPPORTED');
    }
    after = await captureControls(plan.controls, plan, environment, readControl);
    if (!controlsUnchanged(before, after)) fail('E2E_PLATFORM_NON_MUTATION_FAILED');
    if (reservation) await lifecycle.finalize(reservation, true);
  } catch (error) {
    failure = error;
  } finally {
    if (Object.keys(before).length > 0 && !after && environment) {
      try {
        after = await captureControls(plan.controls, plan, environment, readControl);
        if (!failure && !controlsUnchanged(before, after)) failure = new PlatformExecutionError('E2E_PLATFORM_NON_MUTATION_FAILED');
      } catch (error) {
        if (!failure) failure = error;
      }
    }
    if (reservation) await closeQuietly({ close: () => lifecycle.finalize(reservation, false) });
    await closeQuietly(client, 'dispose');
    await closeQuietly(context);
    await closeQuietly(browser);
    eraseSecrets(secrets, environment);
    try {
      await assertIntegrity({ root: repositoryRoot });
    } catch (error) {
      if (!failure) failure = error;
    }
    try {
      const evidence = createSafeEvidence({
        plan,
        result: adapterResult,
        before,
        after,
        failureCode: failure ? safeCode(failure) : null,
        resourceEvents: lifecycle?.evidence?.() || []
      });
      if (typeof writeEvidence === 'function') await writeEvidence(evidence);
    } catch (error) {
      if (!failure) failure = error;
    }
    try {
      await removeTemporaryDirectory(options?.temporaryDirectory);
    } catch (error) {
      if (!failure) failure = error;
    }
  }
  if (failure) {
    if (failure instanceof PlatformExecutionError) throw failure;
    fail(safeCode(failure));
  }
  return createSafeEvidence({ plan, result: adapterResult, before, after, failureCode: null, resourceEvents: lifecycle?.evidence?.() || [] });
}

module.exports = {
  PlatformExecutionError,
  assertPlatformIntegrity,
  createRestrictedInvoker,
  createRuntimeEnvironment,
  createSafeEvidence,
  createManagedLifecycle,
  eraseSecrets,
  executePlatformRun,
  preflightPlatform,
  requiredAuthorizationsFor
};
