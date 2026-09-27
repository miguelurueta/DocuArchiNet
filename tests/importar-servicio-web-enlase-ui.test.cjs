'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const read = path => fs.readFileSync(path, 'utf8');
const page = read('workflow/Webworkflow.aspx');
const composition = read('workflow/Webworkflow.aspx.vb');
const ui = read('js/workflow/importar-servicio-web/importar-servicio-web-ui.js');
const service = read('webservice/WebServiceImportarServicioWebModern.asmx.vb');

test('DOC-80 y DOC-81 publican las operaciones consumidas y no publican ValidateAssignment', () => {
  for (const operation of ['ResolveCapabilities', 'QueryItems', 'GetPreview', 'PreflightImport', 'CreateImportIntent', 'ExecuteImportIntent', 'GetImportIntent', 'ReconcileImportIntent']) {
    assert.match(service, new RegExp(`Function\\s+${operation}\\b`), operation);
  }
  assert.doesNotMatch(service, /Function\s+ValidateAssignment\b/);
});

test('bootstrap enlaza constancias y ENLASE al mismo modal con capacidades aisladas', () => {
  assert.equal((page.match(/id="importar-servicio-web-modal"/g) || []).length, 1);
  assert.match(composition, /ids=\['ctw-document-action-service','a_adj_service_web'\]/);
  assert.match(composition, /ANEXOS_RADICADO_ENLASE/);
  assert.match(composition, /importarServicioWebEnlaseAdapterScript/);
  assert.equal((composition.match(/RegisterImportarServicioWebModernBootstrap\(\)/g) || []).length, 2);
});

test('contexto frontend propaga capacidad y bloquea el legacy en el mismo evento', () => {
  assert.match(ui, /Capability:\s*text\(control\.trigger\.getAttribute\("data-import-capability"\)\)/);
  assert.match(ui, /stopImmediatePropagation/);
  assert.match(ui, /activeControl && activeControl\.modal === modal/);
  assert.match(ui, /triggers\.forEach\(function \(candidate\) \{ bindTrigger/);
});

test('asignación sigue siendo explícita y revalidada por servidor', () => {
  const click = composition.slice(composition.indexOf('Protected Sub Buttonaceptar_Click'), composition.indexOf('End Sub', composition.indexOf('Protected Sub Buttonaceptar_Click')));
  assert.match(click, /Verfica_existencia_tipo_documental_obligatorio_digitalizado/);
  assert.match(click, /If Result <> "YES" Then[\s\S]*Exit Sub/);
  assert.doesNotMatch(ui, /Buttonaceptar[^\n]*\.click\(/);
});

test('solo éxito total confirmado cierra; parcial o incierto permanece visible', () => {
  const previousWindow = global.window, previousDocument = global.document;
  try {
    global.window = global;
    global.document = { readyState: 'loading', addEventListener() {} };
    delete require.cache[require.resolve('../js/workflow/importar-servicio-web/importar-servicio-web-ui.js')];
    const module = require('../js/workflow/importar-servicio-web/importar-servicio-web-ui.js');
    assert.equal(module.shouldCloseAfterResult({ isTotalSuccess: true }, { items: [{ documentId: 9, status: 'Disponible' }] }), true);
    assert.equal(module.shouldCloseAfterResult({ isTotalSuccess: false }, { items: [{ documentId: 9, status: 'Disponible' }] }), false);
    assert.equal(module.shouldCloseAfterResult({ isTotalSuccess: true }, { items: [{ documentId: null, status: 'ResultadoIncierto' }] }), false);
  } finally { global.window = previousWindow; global.document = previousDocument; }
});
