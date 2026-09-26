'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const adapterFactory = require('../js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-adapter.js');

test('adaptador ENLASE fuerza la capacidad y reutiliza la consulta base', async () => {
  const calls = [];
  const base = { queryItems: async request => (calls.push(request), { Items: [] }) };
  const list = { render() {} };
  const adapter = adapterFactory.create({ base, list, contextFactory: () => ({ TaskId: 220587, ProviderId: 'INTEGRACIONSII' }) });
  await adapter.queryItems({ CorrelationId: 'corr-1' });
  assert.equal(calls.length, 1);
  assert.equal(calls[0].TaskId, 220587);
  assert.equal(calls[0].ProviderId, 'INTEGRACIONSII');
  assert.equal(calls[0].Capability, 'ANEXOS_RADICADO_ENLASE');
});

test('adaptador ENLASE delega presentación sin crear transporte', () => {
  let rendered = false;
  const adapter = adapterFactory.create({ base: { queryItems() {} }, list: { render(container, data) { rendered = container.id === 'target' && data.Items.length === 1; } } });
  adapter.renderItems({ id: 'target' }, { Items: [{}] });
  assert.equal(rendered, true);
  const source = fs.readFileSync('js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-adapter.js', 'utf8');
  assert.doesNotMatch(source, /fetch\s*\(|XMLHttpRequest|\$\.ajax|localStorage/);
});

test('contexto ENLASE obtiene TaskId del HiddenIdFlujo autoritativo', () => {
  const ui = fs.readFileSync('js/workflow/importar-servicio-web/importar-servicio-web-ui.js', 'utf8');
  const bootstrap = fs.readFileSync('workflow/Webworkflow.aspx.vb', 'utf8');
  assert.match(bootstrap, /enlaceTaskInputId[^\n]*HiddenIdFlujo\.ClientID/);
  assert.match(bootstrap, /data-import-task-input-id',isEnlase/);
  assert.match(ui, /parts = raw\.split\("\|"\)/);
  assert.match(ui, /parts\.length >= 4[\s\S]*parts\[3\][\s\S]*ENLASE[\s\S]*Number\(parts\[0\]\)/);
});
