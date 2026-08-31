'use strict';

const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const { chromium, request } = require('playwright');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const { queryFingerprint } = require('./support/doc32-e2e-odbc.cjs');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');
const {
  executePlatformRun,
  preflightPlatform,
  requiredAuthorizationsFor
} = require('./support/workflow-e2e-platform.cjs');
const { loadProfile } = require('./support/workflow-e2e-platform-profile.cjs');
const { resolveScenario } = require('./support/workflow-e2e-platform-registry.cjs');

const e2eRoot = path.resolve(__dirname, '..');
const repositoryRoot = path.resolve(e2eRoot, '..', '..');
const SAFE_IDENTIFIER = /^[a-z][a-z0-9-]{1,79}$/;
const AUTHORIZATION_LABELS = Object.freeze({
  environment: '¿Autoriza este ambiente de pruebas?',
  'local-tls': '¿Autoriza temporalmente el certificado local autofirmado?',
  execution: '¿Autoriza la ejecución sobre un recurso descartable?',
  concurrency: '¿Autoriza la concurrencia sobre un recurso descartable?',
  'ui-lock': '¿Autoriza el bloqueo UI sobre un recurso descartable?',
  'discardable-resource': '¿Confirma que el recurso es descartable?'
});
const SECRET_LABELS = Object.freeze({
  'workflow-account': Object.freeze({ label: 'Cuenta Workflow autorizada', secret: false }),
  'workflow-password': Object.freeze({ label: 'Contraseña Workflow', secret: true }),
  'readonly-db-user': Object.freeze({ label: 'Usuario MySQL de solo lectura', secret: false }),
  'readonly-db-password': Object.freeze({ label: 'Contraseña MySQL de solo lectura', secret: true })
});

function fail(code) {
  const error = new Error(`La plataforma E2E no inició la corrida (${code}).`);
  error.code = code;
  throw error;
}

function parseArguments(argv) {
  const result = { scenarioId: null, profilePath: null, requestedAuthorizations: new Set() };
  for (let index = 0; index < argv.length; index += 1) {
    const argument = argv[index];
    if (!['--scenario', '--profile', '--authorize'].includes(argument)) fail('E2E_PLATFORM_ARGUMENT_INVALID');
    const value = argv[index + 1];
    if (!value || value.startsWith('--')) fail('E2E_PLATFORM_ARGUMENT_VALUE_REQUIRED');
    index += 1;
    if (argument === '--scenario') {
      if (!SAFE_IDENTIFIER.test(value) || result.scenarioId) fail('E2E_PLATFORM_ARGUMENT_INVALID');
      result.scenarioId = value;
    } else if (argument === '--profile') {
      if (result.profilePath) fail('E2E_PLATFORM_ARGUMENT_INVALID');
      result.profilePath = value;
    } else {
      for (const authorization of value.split(',')) {
        if (!SAFE_IDENTIFIER.test(authorization)) fail('E2E_PLATFORM_ARGUMENT_INVALID');
        result.requestedAuthorizations.add(authorization);
      }
    }
  }
  if (!result.scenarioId || !result.profilePath) fail('E2E_PLATFORM_ARGUMENT_REQUIRED');
  return result;
}

async function collectAuthorizations(required, requested) {
  if (requested.size !== required.length || required.some((authorization) => !requested.has(authorization))) {
    fail('E2E_PLATFORM_AUTHORIZATION_ARGUMENT_REQUIRED');
  }
  if (required.length === 0) return new Set();
  requireInteractiveConsole();
  const confirmations = {};
  for (const authorization of required) {
    const label = AUTHORIZATION_LABELS[authorization];
    if (!label) fail('E2E_PLATFORM_AUTHORIZATION_INVALID');
    await collectConfirmation(confirmations, authorization, label);
  }
  return new Set(required);
}

async function collectSecrets(plan) {
  if (plan.scenario.requiredSecrets.length === 0) return {};
  requireInteractiveConsole();
  const values = {};
  for (const secretName of plan.scenario.requiredSecrets) {
    const instruction = SECRET_LABELS[secretName];
    if (!instruction) fail('E2E_PLATFORM_SECRET_REGISTRY_INVALID');
    await collectValue(values, secretName, instruction.label, { secret: instruction.secret });
  }
  return values;
}

function endpoint(baseUrl, servicePath, operation) {
  return new URL(`${servicePath}/${operation}`, baseUrl).toString();
}

function assertPublicResponse(dto) {
  const serialized = JSON.stringify(dto);
  if (/System\.(?:Exception|Data)|(?:SELECT|INSERT|UPDATE|DELETE)\s/i.test(serialized)) fail('E2E_PLATFORM_RESPONSE_REJECTED');
}

async function createClient({ context, plan }) {
  if (context && !plan.profile.ignoreHttpsErrors) return { request: context.request, dispose: async () => {} };
  const options = context ? { storageState: await context.storageState() } : {};
  if (plan.profile.ignoreHttpsErrors) options.ignoreHTTPSErrors = true;
  const api = await request.newContext(options);
  return { request: api, dispose: () => api.dispose() };
}

async function invokeNotes({ client, servicePath, operation, payload, plan }) {
  const started = performance.now();
  const response = await client.request.post(endpoint(plan.profile.baseUrl, servicePath, operation), {
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    data: payload,
    timeout: Math.min(plan.profile.budgetMs, 60000)
  });
  const elapsedMs = Math.round(performance.now() - started);
  if (!response.ok()) fail('E2E_PLATFORM_HTTP_FAILED');
  const envelope = await response.json();
  if (!envelope || typeof envelope.d !== 'object' || envelope.d === null) fail('E2E_PLATFORM_RESPONSE_INVALID');
  assertPublicResponse(envelope.d);
  return { dto: envelope.d, elapsedMs };
}

async function writeEvidence(evidence) {
  const destination = path.join(e2eRoot, 'artifacts', `workflow-e2e-platform-${evidence.scenario}.json`);
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, `${JSON.stringify(evidence, null, 2)}\n`, 'utf8');
}

async function main() {
  const parsedArguments = parseArguments(process.argv.slice(2));
  const profile = await loadProfile(parsedArguments.profilePath);
  if (profile.scenarioId !== parsedArguments.scenarioId) fail('E2E_PLATFORM_SCENARIO_PROFILE_MISMATCH');
  const scenario = resolveScenario(profile.scenarioId);
  const required = requiredAuthorizationsFor(scenario, profile);
  const authorizations = await collectAuthorizations(required, parsedArguments.requestedAuthorizations);
  const plan = preflightPlatform({ profile, authorizations });
  const temporaryDirectory = await fs.mkdtemp(path.join(os.tmpdir(), 'workflow-e2e-platform-'));
  await executePlatformRun({
    profile,
    authorizations,
    temporaryDirectory,
    collectSecrets,
    createBrowser: async (selectedProfile) => chromium.launch(selectedProfile.browser || {}),
    createSession: async ({ browser, plan: currentPlan, environment }) => createAuthenticatedWorkflowSession(browser, {
      baseUrl: currentPlan.profile.baseUrl,
      environment,
      moduleEnvironmentVariable: 'WORKFLOW_E2E_PLATFORM_MODULE',
      userEnvironmentVariable: 'WORKFLOW_E2E_PLATFORM_AUTHORIZED_USER',
      passwordEnvironmentVariable: 'WORKFLOW_E2E_PLATFORM_AUTHORIZED_PASSWORD',
      ignoreHTTPSErrors: currentPlan.profile.ignoreHttpsErrors
    }),
    createClient,
    invoke: (requestOptions) => invokeNotes({ ...requestOptions, plan }),
    readControl: ({ control, taskId, environment }) => queryFingerprint(control.query, taskId, environment, 'NOTES_E2E'),
    writeEvidence
  });
}

main().catch((error) => {
  const code = /^[A-Z0-9_]{3,120}$/.test(error?.code || '') ? error.code : 'E2E_PLATFORM_RUNNER_FAILED';
  console.error(`La plataforma E2E se detuvo de forma segura (${code}). No se mostraron valores sensibles.`);
  process.exitCode = 2;
});

module.exports = {
  collectAuthorizations,
  parseArguments
};
