'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const {
  createAsmxTransport,
  createClient,
  isSafeLocalRoute,
  resolveLocalRoute
} = require('../js/login/segundo-factor.js');

function pending(overrides = {}) {
  return {
    RequiereCodigo: true,
    EstadoPublico: 'PENDIENTE',
    DestinoEnmascarado: 'u***@d***.com',
    ExpiraEnSegundos: 120,
    ReenvioDisponibleEnSegundos: 30,
    ...overrides
  };
}

function fakeView() {
  return {
    busy: [], pending: [], messages: [], logins: [], times: [], cleared: 0,
    setBusy(value) { this.busy.push(value); },
    showPending(destination) { this.pending.push(destination); },
    showMessage(message, error) { this.messages.push({ message, error }); },
    showLogin(message) { this.logins.push(message); },
    renderTime(expiry, resend) { this.times.push({ expiry, resend }); },
    clearCode() { this.cleared += 1; },
    clearSensitive() { this.cleared += 1; }
  };
}

function clientWith(transport, view = fakeView(), overrides = {}) {
  const navigations = [];
  const intervals = [];
  const client = createClient({
    endpoint: 'https://example.test/app/webservice/WebServiceLoginSegundoFactor.asmx',
    currentLocation: 'https://example.test/app/gestor.aspx',
    transport,
    view,
    now: overrides.now || (() => 1000),
    setInterval: callback => { intervals.push(callback); return intervals.length; },
    clearInterval: () => {},
    navigate: url => navigations.push(url)
  });
  return { client, view, navigations, intervals };
}

test('adaptador ASMX envía exclusivamente codigo a Verificar y cuerpos vacíos al resto', async () => {
  const calls = [];
  const transport = createAsmxTransport({
    endpoint: '/app/webservice/WebServiceLoginSegundoFactor.asmx',
    request: async (url, data) => { calls.push({ url, data }); return { d: { ok: true } }; }
  });

  await transport('Verificar', { codigo: '123456', usuario: 'prohibido' });
  await transport('ObtenerEstado', { usuario: 'prohibido' });
  await transport('Reenviar', { challenge: 'prohibido' });
  await transport('Cancelar', { correo: 'prohibido' });

  assert.deepEqual(calls.map(call => call.data), [{ codigo: '123456' }, {}, {}, {}]);
  assert.match(calls[0].url, /\/Verificar$/);
});

test('adaptador rechaza un sobre ASMX inconsistente', async () => {
  const transport = createAsmxTransport({ endpoint: '/asmx', request: async () => ({}) });
  await assert.rejects(() => transport('ObtenerEstado', {}), /SECOND_FACTOR_CONTRACT_INVALID/);
});

test('restaura el estado pendiente desde el servidor', async () => {
  const setup = clientWith(async operation => {
    assert.equal(operation, 'ObtenerEstado');
    return pending();
  });

  assert.equal(await setup.client.restaurar(), true);
  assert.deepEqual(setup.view.pending, ['u***@d***.com']);
  assert.deepEqual(setup.view.busy, [true, false]);
  assert.equal(setup.view.times.at(-1).expiry, 120);
  assert.equal(setup.view.times.at(-1).resend, 30);
});

test('una doble verificación produce una sola solicitud mutante', async () => {
  let release;
  let calls = 0;
  const wait = new Promise(resolve => { release = resolve; });
  const setup = clientWith(async operation => {
    calls += 1;
    assert.equal(operation, 'Verificar');
    await wait;
    return { Exito: false, Codigo: 'SECOND_FACTOR_CODE_INVALID', MensajeVisible: 'El código no es válido.', Estado: pending() };
  });

  const first = setup.client.verificar('123456');
  const second = setup.client.verificar('123456');
  assert.equal(await second, false);
  assert.equal(calls, 1);
  release();
  await first;
  assert.equal(calls, 1);
});

test('OTP inválido conserva panel y un estado terminal restaura login', async () => {
  const responses = [
    { Exito: false, Codigo: 'SECOND_FACTOR_CODE_INVALID', MensajeVisible: 'El código no es válido.', Estado: pending() },
    { Exito: false, Codigo: 'SECOND_FACTOR_BLOCKED', MensajeVisible: 'Se agotaron los intentos.', Estado: { RequiereCodigo: false, EstadoPublico: 'NO_DISPONIBLE' } }
  ];
  const setup = clientWith(async () => responses.shift());

  await setup.client.verificar('123456');
  assert.equal(setup.view.pending.length, 1);
  assert.equal(setup.view.logins.length, 0);
  await setup.client.verificar('654321');
  assert.equal(setup.view.logins.at(-1), 'Se agotaron los intentos.');
  assert.equal(setup.navigations.length, 0);
});

test('solo completado con ruta local segura navega', async () => {
  const complete = route => ({
    Exito: true,
    Codigo: 'SECOND_FACTOR_COMPLETED',
    MensajeVisible: 'Verificación completada.',
    Estado: { RequiereCodigo: false, EstadoPublico: 'COMPLETADO' },
    RutaLocal: route
  });
  const safe = clientWith(async () => complete('~/Defaul/WebFormInicioDocuarchiGestion.aspx'));
  assert.equal(await safe.client.verificar('123456'), true);
  assert.deepEqual(safe.navigations, ['https://example.test/app/Defaul/WebFormInicioDocuarchiGestion.aspx']);

  const unsafe = clientWith(async () => complete('https://evil.test/'));
  assert.equal(await unsafe.client.verificar('123456'), false);
  assert.equal(unsafe.navigations.length, 0);
  assert.equal(unsafe.view.logins.length, 1);
});

test('validador de ruta bloquea redirecciones y traversal', () => {
  assert.equal(isSafeLocalRoute('~/Defaul/inicio.aspx'), true);
  for (const route of ['https://evil.test/', '//evil.test/', '~/../admin', '~/a\\b', '~/a?x=1', '~/a#x', '/absolute']) {
    assert.equal(isSafeLocalRoute(route), false, route);
  }
  assert.equal(
    resolveLocalRoute('~/Defaul/inicio.aspx', '/app/webservice/WebServiceLoginSegundoFactor.asmx', 'https://example.test/app/gestor.aspx'),
    'https://example.test/app/Defaul/inicio.aspx'
  );
});

test('formato OTP inválido no invoca transporte', async () => {
  let calls = 0;
  const setup = clientWith(async () => { calls += 1; });
  assert.equal(await setup.client.verificar('12ab'), false);
  assert.equal(calls, 0);
  assert.equal(setup.view.messages.at(-1).error, true);
});
