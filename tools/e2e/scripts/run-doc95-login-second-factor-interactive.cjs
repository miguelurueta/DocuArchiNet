'use strict';

const fs = require('node:fs');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  promptSecret,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const { createAuthenticatedWorkflowSession } = require('../tests/support/authenticated-workflow-session.cjs');

const allowedProfileFields = new Set(['baseUrl', 'ignoreHttpsErrors', 'module', 'timeoutMilliseconds']);

function fail(code) {
  const error = new Error(code);
  error.code = code;
  throw error;
}

function requestedScenario(argv = process.argv) {
  const index = argv.indexOf('--scenario');
  const scenario = index < 0 ? 'concurrency' : String(argv[index + 1] || '').trim().toLowerCase();
  if (!['expiry', 'verify', 'concurrency', 'stability'].includes(scenario)) fail('DOC95_SCENARIO_INVALID');
  return scenario;
}

function profilePath() {
  const index = process.argv.indexOf('--profile');
  if (index < 0 || !process.argv[index + 1]) fail('DOC95_PROFILE_REQUIRED');
  const supplied = process.argv[index + 1];
  const direct = path.resolve(supplied);
  if (fs.existsSync(direct)) return direct;
  const repositoryRelative = path.resolve(__dirname, '..', '..', '..', supplied);
  if (fs.existsSync(repositoryRelative)) return repositoryRelative;
  fail('DOC95_PROFILE_NOT_FOUND');
}

function loadProfile(target) {
  const profile = JSON.parse(fs.readFileSync(target, 'utf8').replace(/^\uFEFF/, ''));
  const unknown = Object.keys(profile).filter(key => !allowedProfileFields.has(key));
  if (unknown.length > 0) fail('DOC95_PROFILE_FIELD_NOT_ALLOWED');

  let baseUrl;
  try { baseUrl = new URL(profile.baseUrl); } catch { fail('DOC95_BASE_URL_INVALID'); }
  if (!/^https?:$/.test(baseUrl.protocol) || baseUrl.username || baseUrl.password) fail('DOC95_BASE_URL_INVALID');
  if (typeof profile.module !== 'string' || !profile.module.trim()) fail('DOC95_MODULE_REQUIRED');
  if (profile.ignoreHttpsErrors !== true && profile.ignoreHttpsErrors !== false) fail('DOC95_TLS_FLAG_INVALID');
  const timeoutMilliseconds = Number(profile.timeoutMilliseconds);
  if (!Number.isSafeInteger(timeoutMilliseconds) || timeoutMilliseconds <= 0) fail('DOC95_TIMEOUT_INVALID');

  return {
    baseUrl: baseUrl.toString(),
    ignoreHttpsErrors: profile.ignoreHttpsErrors,
    module: profile.module.trim(),
    timeoutMilliseconds
  };
}

async function callAsmx(context, profile, operation, data = {}) {
  const endpoint = new URL(`webservice/WebServiceLoginSegundoFactor.asmx/${operation}`, profile.baseUrl).toString();
  const response = await context.request.post(endpoint, {
    data,
    headers: { 'X-Requested-With': 'XMLHttpRequest' },
    timeout: profile.timeoutMilliseconds
  });
  if (!response.ok()) fail('DOC95_ASMX_HTTP_FAILED');
  const envelope = await response.json();
  if (!envelope || typeof envelope !== 'object' || !Object.hasOwn(envelope, 'd')) fail('DOC95_ASMX_ENVELOPE_INVALID');
  return envelope.d;
}

function assertPending(state) {
  if (!state || state.RequiereCodigo !== true || state.EstadoPublico !== 'PENDIENTE') fail('DOC95_PENDING_STATE_NOT_OBSERVED');
  if (typeof state.DestinoEnmascarado !== 'string' || !state.DestinoEnmascarado.includes('*')) fail('DOC95_MASKED_DESTINATION_NOT_OBSERVED');
  if (!Number.isInteger(state.ExpiraEnSegundos) || state.ExpiraEnSegundos <= 0) fail('DOC95_EXPIRY_NOT_OBSERVED');
}

function assertConcurrency(results) {
  const completed = results.filter(result => result && result.Exito === true && result.Codigo === 'SECOND_FACTOR_COMPLETED');
  const rejected = results.filter(result => !result || result.Exito !== true);
  if (completed.length !== 1 || rejected.length !== 1) fail('DOC95_CONCURRENCY_RESULT_INVALID');
  if (typeof completed[0].RutaLocal !== 'string' || !completed[0].RutaLocal.startsWith('~/')) fail('DOC95_LOCAL_ROUTE_INVALID');
}

function assertCompleted(result) {
  if (!result || result.Exito !== true || result.Codigo !== 'SECOND_FACTOR_COMPLETED') fail('DOC95_OTP_VALIDATION_FAILED');
  if (typeof result.RutaLocal !== 'string' || !result.RutaLocal.startsWith('~/')) fail('DOC95_LOCAL_ROUTE_INVALID');
}

async function hasAuthenticationCookie(context) {
  const cookies = await context.cookies();
  return cookies.some(cookie => cookie.name === '.ASPXAUTH');
}

async function runExpiryScenario(context, profile, state) {
  if (await hasAuthenticationCookie(context)) fail('DOC95_AUTH_COOKIE_BEFORE_COMPLETION');
  console.log('DOC95_EXPIRY_WAIT_STARTED=true');
  await new Promise(resolve => setTimeout(resolve, (state.ExpiraEnSegundos + 3) * 1000));
  const expired = await callAsmx(context, profile, 'ObtenerEstado');
  if (!expired || expired.RequiereCodigo !== false || expired.EstadoPublico !== 'NO_DISPONIBLE' || expired.ExpiraEnSegundos !== 0) {
    fail('DOC95_EXPIRY_NOT_ENFORCED');
  }
  if (await hasAuthenticationCookie(context)) fail('DOC95_AUTH_COOKIE_AFTER_EXPIRY');
  console.log('DOC95_EXPIRY_STATE_CLEARED=true');
  console.log('DOC95_AUTH_COOKIE_AFTER_EXPIRY=false');
  console.log('DOC95_EXPIRY_RESULT=PASSED');
}

async function runVerifyScenario(context, profile) {
  const otp = await promptSecret('OTP recibido en el buzón autorizado');
  if (!/^\d{6}$/.test(otp)) fail('DOC95_OTP_FORMAT_INVALID');
  const result = await callAsmx(context, profile, 'Verificar', { codigo: otp });
  assertCompleted(result);
  if (!await hasAuthenticationCookie(context)) fail('DOC95_AUTH_COOKIE_NOT_OBSERVED');
  console.log('DOC95_OTP_VALIDATION_OK=true');
  console.log('DOC95_AUTH_COOKIE_AFTER_COMPLETION=true');
  console.log('DOC95_VERIFY_RESULT=PASSED');
}

async function runConcurrencyScenario(context, profile) {
  const otp = await promptSecret('OTP recibido en el buzón autorizado');
  if (!/^\d{6}$/.test(otp)) fail('DOC95_OTP_FORMAT_INVALID');
  const results = await Promise.all([
    callAsmx(context, profile, 'Verificar', { codigo: otp }),
    callAsmx(context, profile, 'Verificar', { codigo: otp })
  ]);
  assertConcurrency(results);
  if (!await hasAuthenticationCookie(context)) fail('DOC95_AUTH_COOKIE_NOT_OBSERVED');
  console.log('DOC95_CONCURRENCY_WINNERS=1');
  console.log('DOC95_AUTH_COOKIE_AFTER_COMPLETION=true');
  console.log('DOC95_E2E_RESULT=PASSED');
}

function publicLoginCode(responseText) {
  const match = String(responseText || '').match(/\b(?:SECOND_FACTOR_(?:REQUIRED|START_FAILED|DELIVERY_FAILED|EMAIL_UNAVAILABLE|PRINCIPAL_NOT_FOUND|UNAVAILABLE)|PREAUTHENTICATION_(?:FAILED|INVALID)|LOGIN_(?:MODULE_NOT_FOUND|MODULE_UNSUPPORTED|FINALIZATION_FAILED))\b/);
  return match ? match[0] : '';
}

async function createPendingSession(browser, profile, environment) {
  let observedLoginCode = '';
  const context = await createAuthenticatedWorkflowSession(browser, {
    baseUrl: profile.baseUrl,
    environment,
    moduleEnvironmentVariable: 'DOC95_E2E_MODULE',
    userEnvironmentVariable: 'DOC95_E2E_ACCOUNT',
    passwordEnvironmentVariable: 'DOC95_E2E_PASSWORD',
    timeoutMilliseconds: profile.timeoutMilliseconds,
    ignoreHTTPSErrors: profile.ignoreHttpsErrors,
    onPostbackCompleted: async (_page, response) => {
      observedLoginCode = publicLoginCode(await response.text());
    },
    onDialogCompleted: async (_page, message) => {
      const dialogCode = publicLoginCode(message);
      if (dialogCode) observedLoginCode = dialogCode;
    }
  });

  try {
    const state = await callAsmx(context, profile, 'ObtenerEstado');
    if (!state || state.RequiereCodigo !== true || state.EstadoPublico !== 'PENDIENTE') {
      if (observedLoginCode && observedLoginCode !== 'SECOND_FACTOR_REQUIRED') fail(observedLoginCode);
      const cookies = await context.cookies();
      if (cookies.some(cookie => cookie.name === '.ASPXAUTH')) fail('DOC95_SECOND_FACTOR_NOT_REQUIRED');
      fail(observedLoginCode === 'SECOND_FACTOR_REQUIRED' ? 'DOC95_PENDING_SESSION_NOT_AVAILABLE' : 'DOC95_PENDING_STATE_NOT_OBSERVED');
    }
    assertPending(state);
    return { context, state };
  } catch (error) {
    await context.close().catch(() => {});
    throw error;
  }
}

async function main() {
  const profile = loadProfile(profilePath());
  const scenario = requestedScenario();
  if (process.argv.includes('--check')) {
    console.log('DOC95_E2E_CHECK_OK=true');
    console.log('DOC95_E2E_REAL_EXECUTION=false');
    return;
  }

  const { chromium } = require('playwright');
  requireInteractiveConsole();
  const authorization = {};
  await collectConfirmation(authorization, 'environment', '¿Autoriza el ambiente de pruebas para DOC-95?');
  await collectConfirmation(authorization, 'account', '¿Autoriza la cuenta para la corrida DOC-95?');
  if (scenario !== 'expiry') await collectConfirmation(authorization, 'mailbox', '¿Autoriza consultar el buzón de prueba y usar su OTP?');
  await collectConfirmation(authorization, 'challenge', '¿Autoriza crear y consumir un challenge descartable?');
  if (scenario === 'concurrency') await collectConfirmation(authorization, 'concurrency', '¿Autoriza exactamente dos verificaciones concurrentes del mismo OTP?');
  if (scenario === 'stability') await collectConfirmation(authorization, 'stability', '¿Autoriza tres inicios completos consecutivos, cada uno con challenge, correo y OTP nuevos?');
  if (scenario === 'expiry') await collectConfirmation(authorization, 'expiryWait', '¿Autoriza esperar el vencimiento completo del challenge sin usar el OTP?');
  await collectConfirmation(authorization, 'discardableData', '¿Confirma que cuenta, buzón y challenge son datos de prueba descartables?');
  if (profile.ignoreHttpsErrors) {
    await collectConfirmation(authorization, 'localTls', '¿Autoriza temporalmente el certificado autofirmado de este ambiente?');
  }

  const secrets = {};
  await collectValue(secrets, 'account', 'Cuenta autorizada', { secret: false });
  await collectValue(secrets, 'password', 'Contraseña', { secret: true });
  const environment = {
    DOC95_E2E_MODULE: profile.module,
    DOC95_E2E_ACCOUNT: secrets.account,
    DOC95_E2E_PASSWORD: secrets.password
  };

  const browser = await chromium.launch({ headless: true });
  let context;
  try {
    if (scenario === 'stability') {
      for (let attempt = 0; attempt < 3; attempt += 1) {
        const pending = await createPendingSession(browser, profile, environment);
        context = pending.context;
        console.log('DOC95_LOGIN_POSTBACK_OK=true');
        console.log('DOC95_PENDING_STATE_OK=true');
        console.log('DOC95_MASKED_DESTINATION_OK=true');
        await runVerifyScenario(context, profile);
        await context.close();
        context = null;
        console.log('DOC95_STABILITY_ITERATION_PASSED=true');
      }
      console.log('DOC95_STABILITY_SESSIONS=3');
      console.log('DOC95_STABILITY_RESULT=PASSED');
      return;
    }

    const pending = await createPendingSession(browser, profile, environment);
    context = pending.context;
    const state = pending.state;
    console.log('DOC95_LOGIN_POSTBACK_OK=true');
    console.log('DOC95_PENDING_STATE_OK=true');
    console.log('DOC95_MASKED_DESTINATION_OK=true');
    if (scenario === 'expiry') await runExpiryScenario(context, profile, state);
    else if (scenario === 'verify') await runVerifyScenario(context, profile);
    else await runConcurrencyScenario(context, profile);
  } finally {
    secrets.password = '';
    environment.DOC95_E2E_PASSWORD = '';
    await context?.close().catch(() => {});
    await browser.close().catch(() => {});
  }
}

if (require.main === module) {
  main().catch(error => {
    const code = /^[A-Z0-9_]{3,100}$/.test(error?.code || '') ? error.code : 'DOC95_E2E_FAILED';
    console.error(`La E2E DOC-95 se detuvo de forma segura (${code}). No se mostraron secretos ni respuestas.`);
    process.exitCode = 2;
  });
}

module.exports = {
  allowedProfileFields,
  assertCompleted,
  assertConcurrency,
  assertPending,
  createPendingSession,
  loadProfile,
  publicLoginCode,
  requestedScenario,
  runVerifyScenario
};
