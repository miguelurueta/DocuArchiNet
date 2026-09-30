const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');
const assetVersion = '20260930-bootstrap-global-collision1';

function topLevelNames(source) {
  return Array.from(source.matchAll(/^(?:const|let|var|function)\s+([A-Za-z_$][\w$]*)/gm), match => match[1]);
}

test('los helpers compartidos de Bootstrap Table no redeclaran nombres globales', () => {
  const configNames = new Set(topLevelNames(read('js', 'table_boo', 'table_boot_config.js')));
  const bootstrapNames = new Set(topLevelNames(read('js', 'java_general', 'BootstrapTable.js')));
  const duplicated = Array.from(configNames).filter(name => bootstrapNames.has(name));

  assert.deepEqual(duplicated, []);
  assert.equal(bootstrapNames.has('destroy_table_bootstrap_table'), true);
  assert.equal(bootstrapNames.has('init_row_feld_table_boostrap_table'), true);
  assert.equal(bootstrapNames.has('table_reize_heigth'), true);
});

test('las páginas que cargan ambos helpers invalidan juntos la caché defectuosa', () => {
  const pages = [
    ['Docuarchi', 'WebFormDaPrincipal.aspx'],
    ['workflow', 'Webworkflow.aspx'],
    ['workflow', 'WebFormGestionFlujoTrabajoCamaras.aspx'],
    ['Gestion_migracion', 'Web_form_gestion_migracion_documento.aspx'],
    ['Gestion_migracion', 'Web_form_consulta_documentos_migrados.aspx'],
    ['Gestion_correspondencia', 'WebForm_interface_gestion_tramite.aspx'],
    ['Rues', 'WebFormListadoConsultaRue.aspx'],
    ['Gestion', 'WebFormProducionDocumental.aspx'],
    ['RadicadorSimplificado', 'Web_form_radicacion_simpilificada.aspx'],
    ['radicador', 'WebFormRadicacionEntranteInterna.aspx'],
    ['radicador', 'WebFormRadicacionEntrante.aspx'],
    ['radicador', 'WebFormConsultaRadicacion.aspx'],
    ['Publico', 'web_form_consulta_publica.aspx']
  ];

  for (const page of pages) {
    const source = read(...page);
    assert.match(source, new RegExp(`table_boot_config\\.js\\?v=${assetVersion}`), page.join('/'));
    assert.match(source, new RegExp(`BootstrapTable\\.js\\?v=${assetVersion}`), page.join('/'));
  }
});

test('Radicación Simplificada destruye la tabla con su contenedor real', () => {
  const source = read('js', 'RadicadorSimplificado', 'Web_form_radicacion_simpilificada.js');
  const calls = source.match(/destroy_table_bootstrap_table\([^)]+\)/g) || [];

  assert.equal(calls.length, 2);
  calls.forEach(call => assert.match(call, /['"]div_rad_simple_content_table['"]/));
});

test('destroy_table_bootstrap_table destruye aunque no reciba contenedor', () => {
  const source = read('js', 'java_general', 'BootstrapTable.js');
  const start = source.indexOf('const destroy_table_bootstrap_table');
  const end = source.indexOf('const table_reize_heigth', start);
  const helper = source.slice(start, end);

  assert.ok(start >= 0 && end > start);
  assert.equal((helper.match(/bootstrapTable\('destroy'\)/g) || []).length, 1);
  assert.match(helper, /bootstrapTable\('destroy'\);[\s\S]*if \(parent_table != null\)/);
  assert.match(helper, /return true;/);
});
