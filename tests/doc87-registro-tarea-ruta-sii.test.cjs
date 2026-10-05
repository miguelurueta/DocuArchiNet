const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');

const root = path.resolve(__dirname, '..');
const pagePath = path.join(root, 'workflow', 'WebFormGestionFlujoTrabajoCamaras.aspx');
const modulePath = path.join(root, 'js', 'workflow', 'registro-tarea-ruta-sii.js');

function loadModule() {
  const context = { window: {} };
  vm.createContext(context);
  vm.runInContext(fs.readFileSync(modulePath, 'utf8'), context, { filename: modulePath });
  return context.window.RegistroTareaRutaSii;
}

test('DOC-87 limita el contrato HTML a los seis controles de registro de ruta', () => {
  const html = fs.readFileSync(pagePath, 'utf8');
  const tags = [...html.matchAll(/<(?:input|select)\b[^>]*>/gi)].map(match => match[0]);
  const controls = tags.filter(tag => /class="[^"]*\bconten_registro_ruta\b[^"]*"/i.test(tag));
  assert.equal(controls.length, 6);
  assert.deepEqual(controls.map(tag => tag.match(/atrib_campo_n="([^"]+)"/i)[1]),
    ['recibo', 'codigo_barras', 'matricula', 'rscocial', 'id_tramite', 'id_actividad']);
  controls.forEach(tag => assert.match(tag, /atrib_campo_beetwen="0"/i));
  assert.match(html, /registro-tarea-ruta-sii\.js\?v=20261005-doc87-4/);
  assert.match(html, /WebFormGestionFlujoTrabajoCamaras\.js\?v=20261005-doc87-4/);
  const enrollment = controls.find(tag => /atrib_campo_n="matricula"/i.test(tag));
  assert.match(enrollment, /atrib_campo_o="0"/i);
  assert.match(enrollment, /atrib_campo_nl="1"/i);
});

test('DOC-87 completa el contrato HTML de flujo y flujo SII sin modificar el validador compartido', () => {
  const html = fs.readFileSync(pagePath, 'utf8');
  const tags = [...html.matchAll(/<(?:input|select)\b[^>]*>/gi)].map(match => match[0]);
  const contracts = [
    {
      className: 'conten_registro_flujo',
      names: ['recibo', 'codigo_barras', 'matricula', 'rscocial', 'id_tramite', 'id_flujo', 'id_actividad', 'id_usuario']
    },
    {
      className: 'conten_registro_flujo_tarea_sii',
      names: ['recibo', 'codigo_barras', 'matricula', 'rscocial', 'id_tramite', 'id_flujo', 'id_actividad_fjujo', 'id_actividad']
    }
  ];

  for (const contract of contracts) {
    const controls = tags.filter(tag => new RegExp(`class="[^"]*\\b${contract.className}\\b[^"]*"`, 'i').test(tag));
    assert.equal(controls.length, 8, contract.className);
    assert.deepEqual(controls.map(tag => tag.match(/atrib_campo_n="([^"]+)"/i)[1]), contract.names);
    controls.forEach(tag => assert.match(tag, /atrib_campo_beetwen="0"/i));
  }

  const validator = fs.readFileSync(path.join(root, 'js', 'java_general', 'general_control_java.js'), 'utf8');
  assert.match(validator, /attributes\["atrib_campo_beetwen"\]\.value/);
});

test('DOC-87 canoniza S/R más nueve dígitos y rechaza entradas ambiguas', () => {
  const api = loadModule();
  assert.equal(api.canonicalize('s', ' 2470047 '), 'S002470047');
  assert.equal(api.canonicalize('R', 'R002470047'), 'R002470047');
  assert.equal(api.canonicalize('S', api.canonicalize('S', '2470047')), 'S002470047');
  for (const value of ['', 'S247', '12-3', '1234567890', 'ABC']) {
    assert.equal(api.canonicalize('S', value), null);
  }
  assert.equal(api.canonicalize('S', 'R002470047'), null);
});

function validResponse() {
  return { d: [{
    Error_gestion: 'YES',
    Class_parram_consultarRecibo: { tipotramite: 'REF' },
    Class_parram_consultarRadicado: { radicado: '18333837', matricula: '407564', nombre: "GRANERAS D'YCK/S.A.S", subtipotramite: 'INS' },
    Class_service_ilist_drowlist: [{ id_value: '12|INS', value_campo: 'Inscripción' }],
    Class_service_ilist_drowlist_actividad: [{ id_value: '0', value_campo: '' }, { id_value: '40', value_campo: 'VILLAVICENCIOAV40' }]
  }] };
}

test('DOC-87 conserva una instantánea autoritativa y construye comando mínimo', () => {
  const api = loadModule();
  const normalized = api.normalizeQuery('S002470047', validResponse());
  assert.equal(normalized.ok, true);
  assert.equal(normalized.snapshot.name, "GRANERAS D'YCK/S.A.S");
  assert.deepEqual(Array.from(normalized.snapshot.procedureIds), [12]);
  assert.deepEqual(Array.from(normalized.snapshot.activityIds), [40]);
  api.setSnapshot(normalized.snapshot);
  const result = api.buildCommand('S', 'S002470047', '12', '40');
  assert.equal(result.ok, true);
  assert.deepEqual(JSON.parse(JSON.stringify(result.command)), { recibo: 'S002470047', id_tramite: 12, id_actividad: 40 });
  assert.equal(Object.hasOwn(result.command, 'codigo_barras'), false);
  assert.equal(Object.hasOwn(result.command, 'matricula'), false);
  assert.equal(Object.hasOwn(result.command, 'rscocial'), false);
  assert.equal(api.buildCommand('S', 'S002470048', '12', '40').ok, false);
  assert.equal(api.buildCommand('S', 'S002470047', '12', '0').ok, false);
});

test('DOC-87 usa el tipo del recibo cuando matrícula y subtipo están vacíos', () => {
  const api = loadModule();
  const optional = validResponse();
  optional.d[0].Class_parram_consultarRadicado.matricula = '';
  optional.d[0].Class_parram_consultarRadicado.subtipotramite = '';
  optional.d[0].Class_parram_consultarRecibo.tipotramite = ' ins ';
  optional.d[0].Class_service_ilist_drowlist.push({ id_value: '13|REF', value_campo: 'Reforma' });
  const normalized = api.normalizeQuery('S002470047', optional);
  assert.equal(normalized.ok, true);
  assert.equal(normalized.snapshot.enrollment, '');
  assert.equal(normalized.snapshot.procedureSubtype, '');
  assert.equal(normalized.snapshot.procedureType, 'ins');
  assert.equal(normalized.snapshot.procedureId, 12);
  assert.equal(normalized.requiresProcedureSelection, false);
  assert.deepEqual(Array.from(normalized.snapshot.procedureIds), [12]);
  api.setSnapshot(normalized.snapshot);
  assert.equal(api.buildCommand('S', 'S002470047', '0', '40').ok, false);
  assert.equal(api.buildCommand('S', 'S002470047', '12', '40').ok, true);
});

test('DOC-87 falla cerrado ante cardinalidad, datos realmente obligatorios o trámite ambiguo', () => {
  const api = loadModule();
  assert.equal(api.normalizeQuery('S002470047', { d: [] }).ok, false);
  assert.equal(api.normalizeQuery('S002470047', { d: [validResponse().d[0], validResponse().d[0]] }).ok, false);
  const incomplete = validResponse();
  incomplete.d[0].Class_parram_consultarRadicado.radicado = '';
  assert.equal(api.normalizeQuery('S002470047', incomplete).ok, false);
  const ambiguous = validResponse();
  ambiguous.d[0].Class_service_ilist_drowlist.push({ id_value: '13|INS', value_campo: 'Duplicado' });
  assert.equal(api.normalizeQuery('S002470047', ambiguous).ok, false);
  const noProcedureType = validResponse();
  noProcedureType.d[0].Class_parram_consultarRadicado.subtipotramite = '';
  noProcedureType.d[0].Class_parram_consultarRecibo.tipotramite = '';
  assert.equal(api.normalizeQuery('S002470047', noProcedureType).ok, false);
});

test('DOC-87 bloquea doble envío y permite liberar el intento', () => {
  const api = loadModule();
  assert.equal(api.tryBeginSubmit(), true);
  assert.equal(api.tryBeginSubmit(), false);
  api.endSubmit();
  assert.equal(api.tryBeginSubmit(), true);
});
