'use strict';

const LOGIN_PATH = 'gestor.aspx';
const LOGIN_SELECTORS = Object.freeze({
  module: '#ContentPlacenter_DropDownListmodulos',
  user: '#ContentPlacenter_TextBoxuser',
  password: '#ContentPlacenter_TextBoxpasw',
  submit: 'a.da-login-submit'
});
const DEFAULT_TIMEOUT_MILLISECONDS = 30000;

function requiredSessionValue(environment, name) {
  const value = environment && environment[name];
  if (typeof value !== 'string' || value.trim().length === 0) {
    throw new Error(`Falta la variable de sesión requerida: ${name}.`);
  }
  return value;
}

function normalizeBaseUrl(baseUrl) {
  if (typeof baseUrl !== 'string' || baseUrl.trim().length === 0) {
    throw new Error('La URL base de la sesión E2E es obligatoria.');
  }

  try {
    return new URL(baseUrl).toString();
  } catch {
    throw new Error('La URL base de la sesión E2E debe ser absoluta y válida.');
  }
}

function positiveTimeout(timeoutMilliseconds) {
  const value = timeoutMilliseconds === undefined ? DEFAULT_TIMEOUT_MILLISECONDS : Number(timeoutMilliseconds);
  if (!Number.isSafeInteger(value) || value <= 0) {
    throw new Error('El tiempo de espera de la sesión E2E debe ser un entero positivo.');
  }
  return value;
}

function loginUrl(baseUrl) {
  return new URL(LOGIN_PATH, baseUrl).toString();
}

function loginPostbackFor(url) {
  return (response) => {
    const request = response.request();
    return request.method() === 'POST' && response.url().split('?')[0] === url;
  };
}

async function closeQuietly(resource) {
  try {
    await resource?.close();
  } catch {
    // La limpieza no debe revelar ni reemplazar el error sanitizado de inicio de sesión.
  }
}

function sessionFailureMessage(stage) {
  switch (stage) {
    case 'navigate':
      return 'No fue posible abrir la página de inicio de sesión E2E autorizada.';
    case 'module-control':
      return 'El navegador no encontró el selector de módulo del inicio de sesión E2E.';
    case 'module-option':
      return 'El selector de módulo no contiene el valor configurado para el inicio de sesión E2E.';
    case 'credentials':
      return 'No fue posible preparar el formulario de inicio de sesión E2E.';
    case 'postback':
      return 'El inicio de sesión E2E no recibió el postback esperado.';
    case 'load':
      return 'El inicio de sesión E2E no completó la carga posterior al postback.';
    default:
      return 'No fue posible iniciar la sesión E2E autenticada.';
  }
}

/**
 * Crea un BrowserContext autenticado mediante el formulario Web Forms de Gestión.
 * El llamador es responsable de cerrar el contexto retornado.
 */
async function createAuthenticatedWorkflowSession(browser, options) {
  const configuration = options || {};
  const environment = configuration.environment || process.env;
  const baseUrl = normalizeBaseUrl(configuration.baseUrl);
  const moduleValue = requiredSessionValue(environment, configuration.moduleEnvironmentVariable);
  const user = requiredSessionValue(environment, configuration.userEnvironmentVariable);
  const password = requiredSessionValue(environment, configuration.passwordEnvironmentVariable);
  const timeout = positiveTimeout(configuration.timeoutMilliseconds);
  const url = loginUrl(baseUrl);

  if (!browser || typeof browser.newContext !== 'function') {
    throw new Error('El navegador Playwright es obligatorio para crear la sesión E2E.');
  }

  let context;
  let page;
  let stage = 'context';
  try {
    // La sonda pública y la configuración normal de Playwright crean el
    // contexto sin opciones. Solo se añade esta opción cuando el ambiente la
    // autoriza expresamente, para no alterar el transporte predeterminado.
    context = configuration.ignoreHTTPSErrors === true
      ? await browser.newContext({ ignoreHTTPSErrors: true })
      : await browser.newContext();
    page = await context.newPage();
    stage = 'navigate';
    await page.goto(url, { waitUntil: 'commit', timeout });
    stage = 'module-control';
    const moduleControl = page.locator(LOGIN_SELECTORS.module);
    await moduleControl.waitFor({ state: 'attached', timeout });
    stage = 'module-option';
    await moduleControl.selectOption({ value: moduleValue });
    if (configuration.preflightOnly === true) return context;
    stage = 'credentials';
    await page.locator(LOGIN_SELECTORS.user).fill(user);
    await page.locator(LOGIN_SELECTORS.password).fill(password);
    stage = 'postback';
    const postback = page.waitForResponse(loginPostbackFor(url), { timeout });
    await page.locator(LOGIN_SELECTORS.submit).click();
    await postback;
    return context;
  } catch (error) {
    await closeQuietly(context);
    throw new Error(sessionFailureMessage(stage));
  } finally {
    await closeQuietly(page);
  }
}

module.exports = {
  LOGIN_SELECTORS,
  createAuthenticatedWorkflowSession,
  sessionFailureMessage
};
