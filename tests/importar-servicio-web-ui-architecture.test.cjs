'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const page = fs.readFileSync('workflow/Webworkflow.aspx', 'utf8');
const source = fs.readFileSync('workflow/Webworkflow.aspx.vb', 'utf8');
const ui = fs.readFileSync('js/workflow/importar-servicio-web/importar-servicio-web-ui.js', 'utf8');

test('la importación moderna conserva una composición y una entrada canónicas', () => {
  assert.equal((page.match(/id="ctw-document-action-service"/g) || []).length, 1);
  assert.equal((source.match(/RegisterImportarServicioWebModernBootstrap\(\)/g) || []).length, 2);
  assert.equal((ui.match(/trigger\.onclick\s*=/g) || []).length, 1);
  assert.match(source, /If ImportarServicioWebModernActive Then[\s\S]*RegisterImportarServicioWebModernAssets\(\)[\s\S]*RegisterImportarServicioWebModernBootstrap\(\)/);
});

test('la UI usa los adaptadores existentes sin arnés ni transporte paralelo', () => {
  for (const dependency of ['ImportarServicioWebApi', 'ImportarServicioWebProviderRegistry', 'ImportarServicioWebProgressAdapter', 'ImportarServicioWebReconciliation', 'ImportarServicioWebDocumentListAdapter']) {
    assert.match(ui, new RegExp(dependency));
  }
  assert.doesNotMatch(ui, /playwright|puppeteer|localStorage|XMLHttpRequest|new WebSocket/i);
});

test('ejecución, espera y proyección mantienen contratos únicos', () => {
  assert.equal((ui.match(/progressAdapter\.execute\(request\)/g) || []).length, 1);
  assert.match(ui, /renderPending\(\)[\s\S]*progressAdapter\.execute\(request\)/);
  assert.match(ui, /reconciliation\.complete\(snapshot, request\)[\s\S]*documentList\.synchronize\(reconciled\)/);
});
