'use strict';

const assert = require('node:assert/strict');
const test = require('node:test');
const { LOGIN_SELECTORS, createAuthenticatedWorkflowSession, sessionFailureMessage } = require('./support/authenticated-workflow-session.cjs');

function createPlaywrightDouble({ failAt } = {}) {
  const calls = { contextAttempts: 0, gotoAttempts: [], pageAttempts: 0, selectors: [], fills: [], waits: [], closed: { context: false, page: false } };
  const page = {
    async goto(url, options) {
      calls.goto = { url, options };
      calls.gotoAttempts.push({ url, options });
      if (failAt === 'goto') throw new Error('navigation failed');
    },
    locator(selector) {
      calls.selectors.push(selector);
      return {
        async waitFor(options) {
          calls.waits.push({ selector, options });
          if (selector === LOGIN_SELECTORS.module && failAt === 'module-control') throw new Error('missing module');
          if (selector === LOGIN_SELECTORS.module && failAt === 'module-control-once' && calls.waits.filter((wait) => wait.selector === LOGIN_SELECTORS.module).length === 1) {
            throw new Error('missing module once');
          }
        },
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
      if (failAt === 'postback') throw new Error('postback failed');
    },
    async waitForLoadState(value) { calls.loadState = value; },
    async close() { calls.closed.page = true; }
  };
  const context = {
    async newPage() { calls.pageAttempts += 1; return page; },
    async close() { calls.closed.context = true; }
  };
  const browser = {
    async newContext(options) {
      calls.contextAttempts += 1;
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
  assert.equal(calls.goto.options.waitUntil, 'commit');
  assert.deepEqual(calls.selectors, [LOGIN_SELECTORS.module, LOGIN_SELECTORS.user, LOGIN_SELECTORS.password, LOGIN_SELECTORS.submit]);
  assert.deepEqual(calls.waits, [{ selector: LOGIN_SELECTORS.module, options: { state: 'attached', timeout: 1234 } }]);
  assert.equal(calls.module.value, 'GESTOR');
  assert.equal(calls.fills.length, 2);
  assert.equal(calls.postback.matchesLogin, true);
  assert.equal(calls.postback.options.timeout, 1234);
  assert.equal(calls.loadState, undefined);
  assert.equal(calls.closed.page, true);
  assert.equal(calls.closed.context, false);
});

test('conserva el contexto predeterminado cuando HTTPS no requiere excepción', async () => {
  const { browser, calls } = createPlaywrightDouble();
  const environment = { E2E_MODULE: 'GESTOR', E2E_USER: 'usuario-prueba', E2E_PASSWORD: 'valor-prueba' };

  await createAuthenticatedWorkflowSession(browser, configuration(environment, { ignoreHTTPSErrors: false, preflightOnly: true }));

  assert.equal(calls.contextOptions, undefined);
});

test('preserva la resolución estándar de una URL base sin barra final', async () => {
  const { browser, calls } = createPlaywrightDouble();
  const environment = { E2E_MODULE: 'GESTOR', E2E_USER: 'usuario-prueba', E2E_PASSWORD: 'valor-prueba' };

  await createAuthenticatedWorkflowSession(browser, configuration(environment, {
    baseUrl: 'https://localhost/GestionDocumental-Docuarchi.net',
    preflightOnly: true
  }));

  assert.equal(calls.goto.url, 'https://localhost/gestor.aspx');
});

test('requiere las variables de sesión sin exponer sus valores', async () => {
  const { browser } = createPlaywrightDouble();

  await assert.rejects(
    () => createAuthenticatedWorkflowSession(browser, configuration({ E2E_MODULE: 'GESTOR', E2E_USER: 'usuario-prueba', E2E_PASSWORD: '' })),
    /E2E_PASSWORD/
  );
});

test('cierra contexto y página si el postback falla sin exponer valores', async () => {
  const { browser, calls } = createPlaywrightDouble({ failAt: 'click' });
  const environment = { E2E_MODULE: 'GESTOR', E2E_USER: 'usuario-prueba', E2E_PASSWORD: 'valor-prueba' };

  await assert.rejects(
    () => createAuthenticatedWorkflowSession(browser, configuration(environment)),
    /no recibió el postback esperado/i
  );

  assert.equal(calls.closed.page, true);
  assert.equal(calls.closed.context, true);
});

test('el preflight compartido valida el módulo sin rellenar ni enviar credenciales', async () => {
  const { browser, calls, context } = createPlaywrightDouble();
  const environment = { E2E_MODULE: 'GESTOR', E2E_USER: 'usuario-prueba', E2E_PASSWORD: 'valor-prueba' };

  const result = await createAuthenticatedWorkflowSession(browser, configuration(environment, { preflightOnly: true }));

  assert.equal(result, context);
  assert.deepEqual(calls.fills, []);
  assert.equal(calls.clicked, undefined);
  assert.equal(calls.postback, undefined);
});

test('falla sanitizado si el selector público no aparece', async () => {
  const { browser, calls } = createPlaywrightDouble({ failAt: 'module-control' });
  const environment = { E2E_MODULE: 'GESTOR', E2E_USER: 'usuario-prueba', E2E_PASSWORD: 'valor-prueba' };

  await assert.rejects(
    () => createAuthenticatedWorkflowSession(browser, configuration(environment)),
    /no encontró el selector/i
  );

  assert.equal(calls.gotoAttempts.length, 1);
  assert.equal(calls.fills.length, 0);
  assert.equal(calls.closed.context, true);
});

test('clasifica fases de inicio de sesión sin detalles internos', () => {
  assert.match(sessionFailureMessage('navigate'), /abrir la página/i);
  assert.match(sessionFailureMessage('module-control'), /no encontró el selector/i);
  assert.match(sessionFailureMessage('module-option'), /no contiene el valor/i);
  assert.match(sessionFailureMessage('credentials'), /preparar el formulario/i);
  assert.doesNotMatch(sessionFailureMessage('unknown'), /usuario|password|cookie|https?:/i);
});
