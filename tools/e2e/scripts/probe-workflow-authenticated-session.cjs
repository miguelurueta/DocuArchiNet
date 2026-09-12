'use strict';

const fs = require('node:fs/promises');
const path = require('node:path');
const { chromium } = require('playwright');
const { collectConfirmation, collectValue, requireInteractiveConsole } = require('./support/interactive-e2e-console.cjs');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');

const profilesRoot = path.resolve(__dirname, '..', 'profiles');

function fail(code) {
  const error = new Error(code);
  error.code = code;
  throw error;
}

async function loadProbeProfile(name) {
  if (typeof name !== 'string' || !/^[A-Za-z0-9_.-]{3,120}$/.test(name)) fail('SESSION_PROBE_PROFILE_INVALID');
  const target = path.resolve(profilesRoot, name);
  if (path.dirname(target) !== profilesRoot) fail('SESSION_PROBE_PROFILE_INVALID');
  const input = JSON.parse((await fs.readFile(target, 'utf8')).replace(/^\uFEFF/, ''));
  const baseUrl = new URL(input.baseUrl);
  if (!/^https?:$/.test(baseUrl.protocol) || baseUrl.username || baseUrl.password) fail('SESSION_PROBE_PROFILE_INVALID');
  if (typeof input.module !== 'string' || !input.module.trim()) fail('SESSION_PROBE_PROFILE_INVALID');
  return { baseUrl: baseUrl.toString(), module: input.module, browser: input.browser || {}, ignoreHttpsErrors: input.ignoreHttpsErrors === true };
}

async function call(context, baseUrl, operation) {
  const response = await context.request.post(new URL(`webservice/WebServiceInicioGestor.asmx/${operation}`, baseUrl).toString(), {
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    data: { DName: '' },
    timeout: 30000
  });
  if (!response.ok()) fail('SESSION_PROBE_HTTP_FAILED');
  const envelope = await response.json();
  return typeof envelope?.d === 'string' ? envelope.d.trim() : '';
}

async function main() {
  const args = process.argv.slice(2);
  if (args.length !== 2 || args[0] !== '--profile') fail('SESSION_PROBE_ARGUMENT_INVALID');
  const profile = await loadProbeProfile(args[1]);
  requireInteractiveConsole();
  const confirmations = {};
  await collectConfirmation(confirmations, 'environment', '¿Autoriza esta sonda de login en el ambiente de pruebas?');
  if (profile.ignoreHttpsErrors) await collectConfirmation(confirmations, 'local-tls', '¿Autoriza temporalmente el certificado local autofirmado?');
  const values = {};
  await collectValue(values, 'account', 'Cuenta Workflow autorizada', { secret: false });
  await collectValue(values, 'password', 'Contraseña Workflow', { secret: true });
  const environment = { PROBE_MODULE: profile.module, PROBE_ACCOUNT: values.account, PROBE_PASSWORD: values.password };
  const browser = await chromium.launch(profile.browser);
  let context;
  try {
    context = await createAuthenticatedWorkflowSession(browser, {
      baseUrl: profile.baseUrl,
      environment,
      moduleEnvironmentVariable: 'PROBE_MODULE',
      userEnvironmentVariable: 'PROBE_ACCOUNT',
      passwordEnvironmentVariable: 'PROBE_PASSWORD',
      ignoreHTTPSErrors: profile.ignoreHttpsErrors
    });
    const login = await call(context, profile.baseUrl, 'web_service_loguin_user');
    const session = await call(context, profile.baseUrl, 'web_service_sesion_user');
    const identityRecognized = login.localeCompare(values.account, undefined, { sensitivity: 'accent' }) === 0;
    const contextAvailable = identityRecognized && session.length > 0 && !/sin identificar|inconsistencia|imposible/i.test(session);
    console.log('LOGIN_POSTBACK_OK=true');
    console.log(`LOGIN_IDENTITY_RECOGNIZED=${identityRecognized}`);
    console.log(`WORKFLOW_CONTEXT_AVAILABLE=${contextAvailable}`);
    if (!contextAvailable) process.exitCode = 2;
  } finally {
    values.password = '';
    environment.PROBE_PASSWORD = '';
    await context?.close().catch(() => {});
    await browser.close().catch(() => {});
  }
}

main().catch((error) => {
  const code = /^[A-Z0-9_]{3,100}$/.test(error?.code || '') ? error.code : 'SESSION_PROBE_FAILED';
  console.error(`La sonda se detuvo de forma segura (${code}). No se mostraron valores sensibles.`);
  process.exitCode = 2;
});
