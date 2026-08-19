'use strict';

const assert = require('node:assert/strict');
const test = require('node:test');
const { LOGIN_SELECTORS, createAuthenticatedWorkflowSession } = require('./support/authenticated-workflow-session.cjs');

function createPlaywrightDouble({ failAt } = {}) {
  const calls = { selectors: [], fills: [], closed: { context: false, page: false } };
  const page = {
    async goto(url, options) {
      calls.goto = { url, options };
      if (failAt === 'goto') throw new Error('navigation failed');
    },
    locator(selector) {
      calls.selectors.push(selector);
      return {
        async selectOption(value) { calls.module = value; },
        async fill(value) { calls.fills.push({ selector, value }); },
        async click() {
          calls.clicked = selector;
          if (failAt === 'click') throw new Error('click failed');
        }
      };
    },
    async waitForResponse(predicate, options) {
      calls.postback = {
        options,
        matchesLogin: predicate({
          request: () => ({ method: () => 'POST' }),
          url: () => `${calls.goto.url}?partial=1`
        })
      };
    },
    async waitForLoadState(value) { calls.loadState = value; },
    async close() { calls.closed.page = true; }
  };
  const context = {
    async newPage() { return page; },
    async close() { calls.closed.context = true; }
  };
  const browser = {
    async newContext(options) {
      calls.contextOptions = options;
      return context;
    }
  };
  return { browser, calls, context };
}

function configuration(environment, overrides = {}) {
  return {
    baseUrl: 'https://localhost/GestionDocumental-Docuarchi.net/',
    moduleEnvironmentVariable: 'E2E_MODULE',
    userEnvironmentVariable: 'E2E_USER',
    passwordEnvironmentVariable: 'E2E_PASSWORD',
    ignoreHTTPSErrors: true,
    timeoutMilliseconds: 1234,
    environment,
    ...overrides
  };
}

test('crea una sesión aislada con los selectores del formulario Web Forms', async () => {
  const { browser, calls, context } = createPlaywrightDouble();
  const environment = { E2E_MODULE: 'GESTOR', E2E_USER: 'usuario-prueba', E2E_PASSWORD: 'valor-con-punto.' };

  const result = await createAuthenticatedWorkflowSession(browser, configuration(environment));

  assert.equal(result, context);
  assert.deepEqual(calls.contextOptions, { ignoreHTTPSErrors: true });
  assert.equal(calls.goto.url, 'https://localhost/GestionDocumental-Docuarchi.net/gestor.aspx');
  assert.deepEqual(calls.selectors, [LOGIN_SELECTORS.module, LOGIN_SELECTORS.user, LOGIN_SELECTORS.password, LOGIN_SELECTORS.submit]);
  assert.equal(calls.module.value, 'GESTOR');
  assert.equal(calls.fills.length, 2);
  assert.equal(calls.postback.matchesLogin, true);
  assert.equal(calls.postback.options.timeout, 1234);
  assert.equal(calls.loadState, 'domcontentloaded');
  assert.equal(calls.closed.page, true);
  assert.equal(calls.closed.context, false);
});

test('requiere las variables de sesión sin exponer sus valores', async () => {
  const { browser } = createPlaywrightDouble();

  await assert.rejects(
    () => createAuthenticatedWorkflowSession(browser, configuration({ E2E_MODULE: 'GESTOR', E2E_USER: 'usuario-prueba', E2E_PASSWORD: '' })),
    /E2E_PASSWORD/
  );
});

test('cierra contexto y página si el bootstrap falla', async () => {
  const { browser, calls } = createPlaywrightDouble({ failAt: 'click' });
  const environment = { E2E_MODULE: 'GESTOR', E2E_USER: 'usuario-prueba', E2E_PASSWORD: 'valor-prueba' };

  await assert.rejects(
    () => createAuthenticatedWorkflowSession(browser, configuration(environment)),
    /No fue posible iniciar la sesión E2E autenticada/
  );

  assert.equal(calls.closed.page, true);
  assert.equal(calls.closed.context, true);
});
