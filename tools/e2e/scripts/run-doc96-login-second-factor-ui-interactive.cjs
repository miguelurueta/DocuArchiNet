'use strict';

const fs = require('node:fs');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  promptSecret,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const {
  createWorkflowLoginPageSession
} = require('../tests/support/authenticated-workflow-session.cjs');

const allowedProfileFields = new Set([
  'baseUrl', 'ignoreHttpsErrors', 'moduleOff', 'moduleOn',
  'timeoutMilliseconds', 'maxExpiryWaitMilliseconds'
]);
const scenarios = new Set(['off', 'verify', 'refresh', 'invalid', 'blocked', 'resend', 'cancel', 'expiry', 'double-submit', 'matrix']);
const UI = Object.freeze({
  panel: '#docuarchi-segundo-factor-panel',
  destination: '#docuarchi-segundo-factor-destino',
  code: '#docuarchi-segundo-factor-codigo',
  verify: '#docuarchi-segundo-factor-validar',
  resend: '#docuarchi-segundo-factor-reenviar',
  cancel: '#docuarchi-segundo-factor-cancelar',
  error: '#docuarchi-segundo-factor-error',
  login: '#ContentPlacenter_Panel_sesion_privado_login'
});

function fail(code) {
  const error = new Error(code);
  error.code = code;
  throw error;
}

function requestedScenario(argv = process.argv) {
  const index = argv.indexOf('--scenario');
  const scenario = index < 0 ? 'verify' : String(argv[index + 1] || '').trim().toLowerCase();
  if (!scenarios.has(scenario)) fail('DOC96_SCENARIO_INVALID');
  return scenario;
}

function profilePath(argv = process.argv) {
  const index = argv.indexOf('--profile');
  if (index < 0 || !argv[index + 1]) fail('DOC96_PROFILE_REQUIRED');
  const supplied = argv[index + 1];
  const direct = path.resolve(supplied);
  if (fs.existsSync(direct)) return direct;
  const repositoryRelative = path.resolve(__dirname, '..', '..', '..', supplied);
  if (fs.existsSync(repositoryRelative)) return repositoryRelative;
  fail('DOC96_PROFILE_NOT_FOUND');
}

function loadProfile(target) {
  const profile = JSON.parse(fs.readFileSync(target, 'utf8').replace(/^\uFEFF/, ''));
  if (Object.keys(profile).some(key => !allowedProfileFields.has(key))) fail('DOC96_PROFILE_FIELD_NOT_ALLOWED');
  let baseUrl;
  try { baseUrl = new URL(profile.baseUrl); } catch { fail('DOC96_BASE_URL_INVALID'); }
  if (!/^https?:$/.test(baseUrl.protocol) || baseUrl.username || baseUrl.password) fail('DOC96_BASE_URL_INVALID');
  for (const name of ['moduleOff', 'moduleOn']) {
    if (typeof profile[name] !== 'string' || !profile[name].trim()) fail(`DOC96_${name.toUpperCase()}_REQUIRED`);
  }
  if (profile.moduleOff.trim() === profile.moduleOn.trim()) fail('DOC96_MODULE_MATRIX_INVALID');
  if (profile.ignoreHttpsErrors !== true && profile.ignoreHttpsErrors !== false) fail('DOC96_TLS_FLAG_INVALID');
  const timeoutMilliseconds = Number(profile.timeoutMilliseconds);
  const maxExpiryWaitMilliseconds = Number(profile.maxExpiryWaitMilliseconds);
  if (!Number.isSafeInteger(timeoutMilliseconds) || timeoutMilliseconds <= 0) fail('DOC96_TIMEOUT_INVALID');
  if (!Number.isSafeInteger(maxExpiryWaitMilliseconds) || maxExpiryWaitMilliseconds <= timeoutMilliseconds) fail('DOC96_EXPIRY_TIMEOUT_INVALID');
  return {
    baseUrl: baseUrl.toString(),
    ignoreHttpsErrors: profile.ignoreHttpsErrors,
    moduleOff: profile.moduleOff.trim(),
    moduleOn: profile.moduleOn.trim(),
    timeoutMilliseconds,
    maxExpiryWaitMilliseconds
  };
}

async function hasAuthenticationCookie(context) {
  return (await context.cookies()).some(cookie => cookie.name === '.ASPXAUTH');
}

function publicLoginCode(value) {
  const match = String(value || '').match(/\b(?:SECOND_FACTOR_[A-Z_]+|PREAUTHENTICATION_[A-Z_]+|LOGIN_[A-Z_]+)\b/);
  return match ? match[0] : '';
}

async function startLogin(browser, profile, environment, prefix, module) {
  environment[`${prefix}_MODULE`] = module;
  const observation = { publicCode: '', dialogObserved: false };
  try {
    const session = await createWorkflowLoginPageSession(browser, {
      baseUrl: profile.baseUrl,
      environment,
      moduleEnvironmentVariable: `${prefix}_MODULE`,
      userEnvironmentVariable: `${prefix}_ACCOUNT`,
      passwordEnvironmentVariable: `${prefix}_PASSWORD`,
      timeoutMilliseconds: profile.timeoutMilliseconds,
      ignoreHTTPSErrors: profile.ignoreHttpsErrors,
      onPostbackCompleted: async (_page, response) => {
        const publicBody = await response.text().catch(() => '');
        observation.publicCode = publicLoginCode(publicBody);
      },
      onDialogCompleted: async (_page, message) => {
        observation.dialogObserved = true;
        observation.publicCode = publicLoginCode(message) || observation.publicCode;
      }
    });
    return { ...session, loginObservation: observation };
  } catch (error) {
    const stage = String(error?.stage || 'unknown').replace(/[^a-z0-9]/gi, '_').toUpperCase();
    fail(`${prefix}_LOGIN_${stage}_FAILED`);
  }
}

async function assertOffFlow(browser, profile, environment) {
  const session = await startLogin(browser, profile, environment, 'DOC96_OFF', profile.moduleOff);
  try {
    try {
      await session.page.waitForURL(url => !/\/gestor\.aspx(?:\?|$)/i.test(url.toString()), { timeout: profile.timeoutMilliseconds });
    } catch {
      if (session.loginObservation.publicCode) fail(`DOC96_OFF_${session.loginObservation.publicCode}`);
      if (session.loginObservation.dialogObserved) fail('DOC96_OFF_LOGIN_REJECTED');
      if (await hasAuthenticationCookie(session.context)) fail('DOC96_OFF_REDIRECT_NOT_OBSERVED');
      fail('DOC96_OFF_FINALIZATION_NOT_COMPLETED');
    }
    if (!await hasAuthenticationCookie(session.context)) fail('DOC96_OFF_AUTH_COOKIE_NOT_OBSERVED');
    if (await session.page.locator(UI.panel).count()) fail('DOC96_OFF_OTP_PANEL_OBSERVED');
    console.log('DOC96_OFF_LOGIN_LEGACY_OK=true');
    console.log('DOC96_OFF_AUTH_COOKIE_OK=true');
  } finally {
    await session.context.close().catch(() => {});
  }
}

async function startPending(browser, profile, environment) {
  const session = await startLogin(browser, profile, environment, 'DOC96_ON', profile.moduleOn);
  try {
    try {
      await session.page.locator(UI.panel).waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
    } catch {
      fail('DOC96_PENDING_UI_NOT_OBSERVED');
    }
    const destination = String(await session.page.locator(UI.destination).textContent() || '');
    if (!destination.includes('*')) fail('DOC96_MASKED_DESTINATION_NOT_OBSERVED');
    if (await hasAuthenticationCookie(session.context)) fail('DOC96_AUTH_COOKIE_BEFORE_COMPLETION');
    console.log('DOC96_PENDING_UI_OK=true');
    console.log('DOC96_MASKED_DESTINATION_OK=true');
    return session;
  } catch (error) {
    await session.context.close().catch(() => {});
    throw error;
  }
}

async function enterCode(page, value) {
  if (!/^\d{6}$/.test(value)) fail('DOC96_OTP_FORMAT_INVALID');
  await page.locator(UI.code).fill(value);
}

function deriveDifferentCode(receivedOtp) {
  if (!/^\d{6}$/.test(receivedOtp)) fail('DOC96_OTP_FORMAT_INVALID');
  return String((Number(receivedOtp) + 1) % 1000000).padStart(6, '0');
}

async function submitAndObserveVerification(page, timeoutMilliseconds) {
  const responsePromise = page.waitForResponse(response =>
    response.request().method() === 'POST' &&
    /WebServiceLoginSegundoFactor\.asmx\/Verificar$/i.test(response.url()),
  { timeout: timeoutMilliseconds });
  await page.locator(UI.verify).click();
  let response;
  try {
    response = await responsePromise;
  } catch {
    fail('DOC96_VERIFY_RESPONSE_NOT_OBSERVED');
  }
  if (!response.ok()) fail('DOC96_VERIFY_HTTP_FAILED');
}

async function assertAutomationStillOwnsPending(session) {
  if (await hasAuthenticationCookie(session.context)) fail('DOC96_BROWSER_CHANGED_BEFORE_AUTOMATION');
  if (!await session.page.locator(UI.panel).isVisible().catch(() => false)) fail('DOC96_BROWSER_CHANGED_BEFORE_AUTOMATION');
}

async function waitForCompletion(session, profile) {
  await session.page.waitForURL(url => !/\/gestor\.aspx(?:\?|$)/i.test(url.toString()), { timeout: profile.timeoutMilliseconds });
  if (!await hasAuthenticationCookie(session.context)) fail('DOC96_AUTH_COOKIE_NOT_OBSERVED');
  const destination = new URL(session.page.url());
  const allowedOrigin = new URL(profile.baseUrl);
  if (destination.origin !== allowedOrigin.origin) fail('DOC96_NAVIGATION_NOT_LOCAL');
  console.log('DOC96_LOCAL_NAVIGATION_OK=true');
  console.log('DOC96_AUTH_COOKIE_AFTER_COMPLETION=true');
}

async function runActiveScenario(browser, profile, environment, scenario) {
  const session = await startPending(browser, profile, environment);
  try {
    if (scenario === 'refresh') {
      await session.page.reload({ waitUntil: 'domcontentloaded', timeout: profile.timeoutMilliseconds });
      await session.page.locator(UI.panel).waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
      if (await hasAuthenticationCookie(session.context)) fail('DOC96_AUTH_COOKIE_AFTER_REFRESH');
      console.log('DOC96_REFRESH_RESTORED_PENDING=true');
      return;
    }
    if (scenario === 'cancel') {
      await session.page.locator(UI.cancel).click();
      await session.page.locator(UI.panel).waitFor({ state: 'hidden', timeout: profile.timeoutMilliseconds });
      await session.page.locator(UI.login).waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
      if (await hasAuthenticationCookie(session.context)) fail('DOC96_AUTH_COOKIE_AFTER_CANCEL');
      console.log('DOC96_CANCEL_RETURNED_TO_LOGIN=true');
      return;
    }
    if (scenario === 'expiry') {
      let verifyRequests = 0;
      session.page.on('request', request => {
        if (request.method() === 'POST' && /WebServiceLoginSegundoFactor\.asmx\/Verificar$/i.test(request.url())) verifyRequests += 1;
      });
      console.log('DOC96_BROWSER_OBSERVATION_ONLY=true');
      await session.page.locator(UI.panel).waitFor({ state: 'hidden', timeout: profile.maxExpiryWaitMilliseconds });
      if (verifyRequests !== 0) fail('DOC96_EXPIRY_VERIFICATION_REQUEST_OBSERVED');
      if (await hasAuthenticationCookie(session.context)) fail('DOC96_AUTH_COOKIE_AFTER_EXPIRY');
      console.log('DOC96_EXPIRY_RETURNED_TO_LOGIN=true');
      return;
    }
    if (scenario === 'invalid' || scenario === 'blocked') {
      console.log('DOC96_BROWSER_OBSERVATION_ONLY=true');
      const receivedOtp = await promptSecret('OTP real recibido; se usará solo para calcular otro código y nunca se enviará');
      const invalid = deriveDifferentCode(receivedOtp);
      await assertAutomationStillOwnsPending(session);
      const attempts = scenario === 'blocked' ? 5 : 1;
      for (let index = 0; index < attempts; index += 1) {
        const attemptNumber = index + 1;
        try {
          await enterCode(session.page, invalid);
        } catch (error) {
          if (error && error.code) throw error;
          fail(`DOC96_${scenario.toUpperCase()}_ATTEMPT_${attemptNumber}_INPUT_FAILED`);
        }
        try {
          await submitAndObserveVerification(session.page, profile.timeoutMilliseconds);
        } catch (error) {
          if (error && error.code) throw error;
          fail(`DOC96_${scenario.toUpperCase()}_ATTEMPT_${attemptNumber}_SUBMIT_FAILED`);
        }
        const finalBlockedAttempt = scenario === 'blocked' && index === attempts - 1;
        if (!finalBlockedAttempt) {
          try {
            await session.page.locator(UI.error).waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
          } catch {
            if (await hasAuthenticationCookie(session.context)) {
              fail(`DOC96_${scenario.toUpperCase()}_ATTEMPT_${attemptNumber}_VALID_OTP_ACCEPTED`);
            }
            if (await session.page.locator(UI.login).isVisible().catch(() => false)) {
              fail(`DOC96_${scenario.toUpperCase()}_ATTEMPT_${attemptNumber}_TERMINAL_EARLY`);
            }
            fail(`DOC96_${scenario.toUpperCase()}_ATTEMPT_${attemptNumber}_ERROR_NOT_VISIBLE`);
          }
          try {
            await session.page.waitForFunction(selector => {
              const button = document.querySelector(selector);
              return button && !button.disabled;
            }, UI.verify, { timeout: profile.timeoutMilliseconds });
          } catch {
            fail(`DOC96_${scenario.toUpperCase()}_ATTEMPT_${attemptNumber}_BUTTON_NOT_READY`);
          }
        }
      }
      if (scenario === 'blocked') {
        try {
          await session.page.locator(UI.panel).waitFor({ state: 'hidden', timeout: profile.timeoutMilliseconds });
        } catch {
          fail('DOC96_BLOCKED_UI_NOT_CLOSED');
        }
      }
      else await session.page.locator(UI.panel).waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
      if (await hasAuthenticationCookie(session.context)) fail('DOC96_AUTH_COOKIE_AFTER_INVALID');
      console.log(scenario === 'blocked' ? 'DOC96_BLOCKED_RETURNED_TO_LOGIN=true' : 'DOC96_INVALID_KEPT_PENDING=true');
      return;
    }
    if (scenario === 'resend') {
      await session.page.locator(UI.resend).waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
      await session.page.locator(UI.resend).waitFor({ state: 'attached', timeout: profile.timeoutMilliseconds });
      await session.page.waitForFunction(selector => !document.querySelector(selector).disabled, UI.resend, { timeout: profile.maxExpiryWaitMilliseconds });
      const oldCode = await promptSecret('OTP anterior autorizado');
      await assertAutomationStillOwnsPending(session);
      await session.page.locator(UI.resend).click();
      await session.page.getByText('Se envió un código nuevo.', { exact: false }).waitFor({ state: 'visible', timeout: profile.timeoutMilliseconds });
      await enterCode(session.page, oldCode);
      await submitAndObserveVerification(session.page, profile.timeoutMilliseconds);
      await session.page.waitForFunction(selector => {
        const button = document.querySelector(selector);
        return button && !button.disabled;
      }, UI.verify, { timeout: profile.timeoutMilliseconds });
      if (!await session.page.locator(UI.panel).isVisible().catch(() => false)) fail('DOC96_RESEND_PREVIOUS_CODE_NOT_REJECTED');
      const newCode = await promptSecret('OTP nuevo autorizado');
      await assertAutomationStillOwnsPending(session);
      await enterCode(session.page, newCode);
      await session.page.locator(UI.verify).click();
      await waitForCompletion(session, profile);
      console.log('DOC96_RESEND_INVALIDATED_PREVIOUS=true');
      return;
    }

    console.log('DOC96_BROWSER_OBSERVATION_ONLY=true');
    const otp = await promptSecret('OTP recibido en el buzón autorizado; ingréselo solo aquí, no en Chromium');
    await assertAutomationStillOwnsPending(session);
    await enterCode(session.page, otp);
    let verifyRequests = 0;
    session.page.on('request', request => {
      if (/WebServiceLoginSegundoFactor\.asmx\/Verificar$/i.test(request.url())) verifyRequests += 1;
    });
    if (scenario === 'double-submit') {
      await session.page.locator(UI.verify).evaluate(button => { button.click(); button.click(); });
    } else {
      await session.page.locator(UI.verify).click();
    }
    await waitForCompletion(session, profile);
    if (verifyRequests !== 1) fail('DOC96_VERIFY_REQUEST_COUNT_INVALID');
    console.log(scenario === 'double-submit' ? 'DOC96_DOUBLE_SUBMIT_REQUESTS=1' : 'DOC96_VERIFY_UI_RESULT=PASSED');
  } finally {
    await session.context.close().catch(() => {});
  }
}

async function authorize(profile, scenario) {
  requireInteractiveConsole();
  const authorization = {};
  await collectConfirmation(authorization, 'environment', '¿Autoriza el ambiente de pruebas para DOC-96?');
  await collectConfirmation(authorization, 'accounts', '¿Autoriza las cuentas requeridas para la corrida DOC-96?');
  if (scenario !== 'off') await collectConfirmation(authorization, 'challenge', '¿Autoriza crear y consumir challenges descartables de DOC-96?');
  if (!['off', 'refresh', 'cancel', 'expiry'].includes(scenario)) {
    await collectConfirmation(authorization, 'mailbox', '¿Autoriza consultar el buzón de prueba y usar los códigos solicitados?');
  }
  await collectConfirmation(authorization, 'discardable', '¿Confirma que las cuentas, buzón y challenges son datos de prueba descartables?');
  if (profile.ignoreHttpsErrors) await collectConfirmation(authorization, 'tls', '¿Autoriza el certificado autofirmado solo para este ambiente?');
}

async function collectSecrets(profile, scenario) {
  const values = {};
  const environment = {};
  if (scenario === 'off' || scenario === 'matrix') {
    await collectValue(values, 'offAccount', `Cuenta autorizada para módulo 2FA apagado (${profile.moduleOff})`, { secret: false });
    await collectValue(values, 'offPassword', `Contraseña del módulo 2FA apagado (${profile.moduleOff})`, { secret: true });
    environment.DOC96_OFF_ACCOUNT = values.offAccount;
    environment.DOC96_OFF_PASSWORD = values.offPassword;
  }
  if (scenario !== 'off') {
    await collectValue(values, 'onAccount', `Cuenta autorizada para módulo 2FA activo (${profile.moduleOn})`, { secret: false });
    await collectValue(values, 'onPassword', `Contraseña del módulo 2FA activo (${profile.moduleOn})`, { secret: true });
    environment.DOC96_ON_ACCOUNT = values.onAccount;
    environment.DOC96_ON_PASSWORD = values.onPassword;
  }
  return { values, environment };
}

async function main() {
  const profile = loadProfile(profilePath());
  const scenario = requestedScenario();
  if (process.argv.includes('--check')) {
    console.log('DOC96_E2E_CHECK_OK=true');
    console.log('DOC96_E2E_REAL_EXECUTION=false');
    return;
  }
  await authorize(profile, scenario);
  const { chromium } = require('playwright');
  const secrets = await collectSecrets(profile, scenario);
  const browser = await chromium.launch({ headless: false });
  try {
    if (scenario === 'off' || scenario === 'matrix') await assertOffFlow(browser, profile, secrets.environment);
    if (scenario !== 'off') await runActiveScenario(browser, profile, secrets.environment, scenario === 'matrix' ? 'verify' : scenario);
    console.log('DOC96_E2E_RESULT=PASSED');
  } finally {
    Object.keys(secrets.values).forEach(key => { secrets.values[key] = ''; });
    Object.keys(secrets.environment).forEach(key => { secrets.environment[key] = ''; });
    await browser.close().catch(() => {});
  }
}

if (require.main === module) {
  main().catch(error => {
    const code = /^[A-Z0-9_]{3,100}$/.test(error?.code || '') ? error.code : 'DOC96_E2E_FAILED';
    console.error(`La E2E DOC-96 se detuvo de forma segura (${code}). No se mostraron secretos ni respuestas.`);
    process.exitCode = 2;
  });
}

module.exports = { UI, allowedProfileFields, deriveDifferentCode, loadProfile, publicLoginCode, requestedScenario, scenarios };
