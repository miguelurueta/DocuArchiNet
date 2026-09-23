'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const page = fs.readFileSync('workflow/Webworkflow.aspx', 'utf8');
const source = fs.readFileSync('workflow/Webworkflow.aspx.vb', 'utf8');
const ui = fs.readFileSync('js/workflow/importar-servicio-web/importar-servicio-web-ui.js', 'utf8');

test('el árbol legacy permanece presente y queda declarado para ocultamiento reversible', () => {
  for (const id of ['btnloadservice', 'Panel_sube_documento_integra_sii', 'Panel_list_inscripciones_sii']) {
    assert.match(page, new RegExp(`ID?="${id}"[^>]*data-import-legacy-root="true"`, 'i'), id);
  }
  for (const id of ['GridView_list_inscripciones_sii', 'ModalPopupExtender_edition_list_inscripciones_sii', 'Button_Activa_guardar_Multiplex_Constancias_sii_']) {
    assert.match(page, new RegExp(id), id);
  }
  assert.match(ui, /querySelectorAll\('\[data-import-legacy-root="true"\]'\)/);
  assert.match(ui, /legacyRoot\.hidden = true/);
});

test('gate apagado no registra assets modernos y conserva handlers legacy', () => {
  assert.match(source, /If ImportarServicioWebModernActive Then[\s\S]*RegisterImportarServicioWebModernAssets/);
  assert.match(source, /Handles Button_acepta_sube_documento_integra_sii\.Click/);
  assert.match(source, /Handles GridView_list_inscripciones_sii\.RowCreated/);
  assert.match(source, /Handles GridView_list_inscripciones_sii\.PageIndexChanging/);
});
