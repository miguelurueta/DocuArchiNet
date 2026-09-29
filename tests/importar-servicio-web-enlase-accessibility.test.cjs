'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const ui = fs.readFileSync('js/workflow/importar-servicio-web/importar-servicio-web-ui.js', 'utf8');
const page = fs.readFileSync('workflow/Webworkflow.aspx', 'utf8');
const css = fs.readFileSync('Styles/importar-servicio-web-modern.css', 'utf8');

test('modal ENLASE conserva semántica, foco y anuncios accesibles', () => {
  assert.match(page, /role="dialog" aria-modal="true" aria-labelledby="importar-servicio-web-title"/);
  assert.ok((page.match(/aria-live="polite"/g) || []).length >= 3);
  assert.match(ui, /function onKeydown[\s\S]*event\.key === "Escape"[\s\S]*event\.key !== "Tab"/);
  assert.match(ui, /restoreTriggerFocus\(control\)/);
  assert.match(ui, /previewContext[\s\S]*scrollTop[\s\S]*\.focus\(\)/);
});

test('textos y capacidad visibles distinguen anexos de constancias', () => {
  assert.match(ui, /Importar anexos desde SII/);
  assert.match(ui, /antes de asignar la tarea/);
  assert.match(ui, /data-import-capability/);
});

test('diálogo y tabla mantienen scroll interno en escritorio y móvil', () => {
  assert.match(css, /\.importar-servicio-web__dialog\s*\{[^}]*max-height:\s*94vh[^}]*overflow:\s*hidden/s);
  assert.match(css, /\.importar-servicio-web__body\s*\{[^}]*overflow:\s*auto/s);
  assert.match(css, /\.importar-servicio-web-sii__table-scroll\s*\{[^}]*overflow:\s*auto/s);
  assert.match(css, /@media \(max-width:\s*760px\)[\s\S]*height:\s*100dvh/);
  assert.match(css, /\.importar-servicio-web-open\s*\{\s*overflow:\s*hidden/);
  assert.match(css, /\.importar-servicio-web-sii__data-heading, \.importar-servicio-web-sii__data-cell\s*\{[^}]*overflow:\s*hidden;[^}]*text-overflow:\s*ellipsis/s);
  assert.match(css, /importar-servicio-web-sii__actions-cell[^}]*background:\s*#fff/);
});

test('preparación masiva conserva acciones visibles y omite el plan redundante', () => {
  assert.doesNotMatch(page, /importar-servicio-web-preparation-plan/);
  assert.doesNotMatch(ui, /renderPreparationPlan|preparationPlan|Plan previsto/);
  assert.match(page, /id="importar-servicio-web-preparation-items"[^>]*tabindex="0"/);
  assert.match(css, /\.importar-servicio-web__preparation\s*\{[^}]*display:\s*flex;[^}]*height:\s*100%;[^}]*overflow:\s*hidden/s);
  assert.match(css, /\.importar-servicio-web__preparation-items\s*\{[^}]*flex:\s*1 1 auto;[^}]*overflow:\s*auto/s);
  assert.match(css, /\.importar-servicio-web__preparation-actions\s*\{[^}]*flex:\s*0 0 auto;[^}]*background:\s*#fff/s);
});

test('preview reemplaza la lista, muestra retorno y alinea verticalmente el visor', () => {
  assert.match(page, /id="importar-servicio-web-preview-back"[^>]*>[^<]*Volver a documentos/);
  assert.doesNotMatch(css, /importar-servicio-web__preview-back\s*\{\s*display:\s*none/);
  assert.doesNotMatch(css, /dialog--preview #importar-servicio-web-list[^}]*display:\s*block\s*!important/);
  assert.match(css, /\.importar-servicio-web__preview\s*\{[^}]*display:\s*flex;[^}]*height:\s*100%;[^}]*flex-direction:\s*column;[^}]*overflow:\s*hidden/s);
  assert.match(css, /\.importar-servicio-web__preview-frame\s*\{[^}]*min-height:\s*0;[^}]*flex:\s*1 1 auto/s);
  assert.match(css, /\.importar-servicio-web__dialog--preview \.importar-servicio-web__body\s*\{[^}]*overflow:\s*hidden/s);
});
