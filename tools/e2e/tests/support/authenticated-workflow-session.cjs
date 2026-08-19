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
    return new URL(baseUrl.endsWith('/') ? baseUrl : `${baseUrl}/`).toString();
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
  try {
    context = await browser.newContext({ ignoreHTTPSErrors: configuration.ignoreHTTPSErrors === true });
    page = await context.newPage();
    await page.goto(url, { waitUntil: 'domcontentloaded', timeout });
    await page.locator(LOGIN_SELECTORS.module).selectOption({ value: moduleValue });
    await page.locator(LOGIN_SELECTORS.user).fill(user);
    await page.locator(LOGIN_SELECTORS.password).fill(password);
    const postback = page.waitForResponse(loginPostbackFor(url), { timeout });
    await page.locator(LOGIN_SELECTORS.submit).click();
    await postback;
    await page.waitForLoadState('domcontentloaded');
    return context;
  } catch {
    await closeQuietly(context);
    throw new Error('No fue posible iniciar la sesión E2E autenticada.');
  } finally {
    await closeQuietly(page);
  }
}

module.exports = {
  LOGIN_SELECTORS,
  createAuthenticatedWorkflowSession
};
