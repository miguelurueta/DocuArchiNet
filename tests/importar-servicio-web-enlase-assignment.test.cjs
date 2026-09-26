'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const bridge = require('../js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-assignment-bridge.js');

test('puente expone controles pero nunca invoca la asignación', () => {
  const assign = { id: 'enlase-assign-action' }, submit = { id: 'Buttonaceptar', click() { throw new Error('no debe invocarse'); } };
  const instance = bridge.create({ document: { getElementById(id) { return id === assign.id ? assign : (id === submit.id ? submit : null); } } });
  assert.deepEqual(instance.controls(), [assign, submit]);
  assert.equal(instance.assignmentRemainsExplicit(), true);
  const source = fs.readFileSync('js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-assignment-bridge.js', 'utf8');
  assert.doesNotMatch(source, /\.click\s*\(|__doPostBack|fetch\s*\(|\$\.ajax/);
});

test('markup bloquea acciones de asignación durante escritura y servidor revalida obligatorios', () => {
  const page = fs.readFileSync('workflow/Webworkflow.aspx', 'utf8');
  const code = fs.readFileSync('workflow/Webworkflow.aspx.vb', 'utf8');
  assert.match(page, /id="enlase-assign-action"[^>]*data-import-context-action="true"/);
  assert.match(page, /ID="Buttonaceptar"[^>]*data-import-context-action="true"/);
  const start = code.indexOf('Protected Sub Buttonaceptar_Click');
  const fragment = code.slice(start, code.indexOf('End Sub', start));
  assert.match(fragment, /Verfica_existencia_tipo_documental_obligatorio_digitalizado/);
  assert.match(fragment, /If Result <> "YES" Then[\s\S]*Exit Sub/);
});

test('legacy permanece en código y el gate gobierna bootstrap moderno', () => {
  const legacy = fs.readFileSync('js/workflow/Webworkflow.js', 'utf8');
  const composition = fs.readFileSync('workflow/Webworkflow.aspx.vb', 'utf8');
  assert.match(legacy, /ActivaListaAnexosIntegracionSII/);
  assert.match(legacy, /ServiceRESTActivaAdjuntaDocumentoServicioIntegracionEnlace/);
  assert.match(composition, /If ImportarServicioWebModernActive Then[\s\S]*RegisterImportarServicioWebModernBootstrap/);
});
