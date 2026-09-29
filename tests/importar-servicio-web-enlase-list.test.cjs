'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const list = require('../js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-list.js');

function createDocument() {
  const doc = { createElement(tag) { const attrs = {}; return { tagName: tag.toUpperCase(), ownerDocument: doc, children: [], textContent: '', disabled: false, checked: false, indeterminate: false, appendChild(child) { this.children.push(child); return child; }, setAttribute(name, value) { attrs[name] = String(value); }, getAttribute(name) { return Object.hasOwn(attrs, name) ? attrs[name] : null; }, attrs }; } };
  return doc;
}
function all(node) { return [node].concat((node.children || []).flatMap(all)); }

test('lista ENLASE renderiza cero, uno y múltiples sin URL externa', () => {
  for (const items of [[], [{ externalKey: 'A-1', displayName: 'Anexo uno', importable: true, contentType: 'application/pdf', date: '2026-09-26', reference: 'R-1', importStatus: 'DISPONIBLE' }], [{ externalKey: 'A-1', displayName: 'Anexo uno', importable: true }, { externalKey: 'A-2', displayName: 'Anexo dos', importable: false, importStatus: 'IMPORTED' }]]) {
    const doc = createDocument(), container = doc.createElement('div');
    list.render(container, { Items: items });
    const nodes = all(container), text = nodes.map(node => node.textContent).join(' ');
    assert.match(text, /Anexos disponibles desde SII/);
    assert.doesNotMatch(text, /https?:\/\//i);
    assert.equal(nodes.filter(node => node.getAttribute && node.getAttribute('data-import-select') === 'true').length, items.length);
  }
});

test('selección general solo considera anexos importables y acciones permanecen visibles', () => {
  const doc = createDocument(), container = doc.createElement('div');
  list.render(container, { Items: [{ externalKey: 'A-1', displayName: 'Disponible', importable: true }, { externalKey: 'A-2', displayName: 'Importado', importable: false, importStatus: 'IMPORTED' }] });
  const nodes = all(container), selections = nodes.filter(node => node.getAttribute && node.getAttribute('data-import-select') === 'true');
  const selectAll = nodes.find(node => node.getAttribute && node.getAttribute('data-import-select-all') === 'true');
  assert.equal(selectAll.disabled, false);
  assert.equal(selections[0].disabled, false);
  assert.equal(selections[1].disabled, true);
  assert.equal(nodes.filter(node => node.getAttribute && node.getAttribute('data-import-preview') === 'true').length, 2);
  assert.equal(nodes.filter(node => node.getAttribute && node.getAttribute('data-import-prepare') === 'true' && node.disabled).length, 1);
});

test('un anexo importado confirmado muestra acción Reimportar habilitada', () => {
  const doc = createDocument(), container = doc.createElement('div');
  list.render(container, { Items: [{ externalKey: 'A-1', displayName: 'Recibo existente', importable: true, reimportable: true, importStatus: 'IMPORTADO' }] });
  const nodes = all(container), action = nodes.find(node => node.getAttribute && node.getAttribute('data-import-prepare') === 'true');
  assert.equal(action.disabled, false);
  assert.equal(action.textContent, 'Reimportar');
});

test('celdas largas usan columnas semánticas y conservan el texto completo como título', () => {
  const doc = createDocument(), container = doc.createElement('div');
  list.render(container, { Items: [{ externalKey: 'A-1', displayName: 'Nombre de anexo deliberadamente extenso', contentType: 'application/pdf', reference: 'Referencia extensa', importStatus: 'DISPONIBLE', importable: true }] });
  const nodes = all(container), dataCells = nodes.filter(node => String(node.className || '').includes('importar-servicio-web-sii__data-cell'));
  assert.equal(dataCells.length, 5);
  assert.equal(dataCells[0].title, 'Nombre de anexo deliberadamente extenso');
  assert.match(dataCells[0].className, /importar-servicio-web-sii__column--annex/);
  assert.match(dataCells[3].className, /importar-servicio-web-sii__column--reference/);
});
