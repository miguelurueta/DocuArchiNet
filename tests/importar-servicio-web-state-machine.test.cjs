const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const read = (path) => fs.readFileSync(path, 'utf8');
const machine = read('Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb');
const models = read('Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb');

test('la maquina es pura y conserva las fases de item existentes', () => {
  assert.match(machine, /Creada[\s\S]*Validada/);
  assert.match(machine, /Validada[\s\S]*RecursoObtenido/);
  assert.match(machine, /RecursoObtenido[\s\S]*ExpedientePreparado/);
  assert.match(machine, /ExpedientePreparado[\s\S]*IndicesActualizados/);
  assert.match(machine, /IndicesActualizados[\s\S]*DocumentoAlmacenado/);
  assert.match(machine, /DocumentoAlmacenado[\s\S]*CacheActualizado/);
  assert.doesNotMatch(machine, /HttpContext|MySql|Session|ClassAlmacenamiento/);
});

test('declara el recorrido agregado DOC-67 sin saltos', () => {
  const expected = [
    ['Validada', 'ExpedientesPlanificados'],
    ['ExpedientesPlanificados', 'ExpedientesResueltos'],
    ['ExpedientesResueltos', 'ItemsSiiAlmacenados'],
    ['ItemsSiiAlmacenados', 'UniversoDocumentalConsultado'],
    ['UniversoDocumentalConsultado', 'VinculacionesProcesadas'],
    ['VinculacionesProcesadas', 'IndicesYXmlActualizados'],
    ['IndicesYXmlActualizados', 'Reconciliada'],
    ['Reconciliada', 'Completada'],
  ];
  for (const [from, to] of expected) {
    const line = machine.split(/\r?\n/).find((value) => value.includes(`Agregar(t, FaseImportacionServicio.${from},`));
    assert.ok(line, from);
    assert.match(line, new RegExp(`FaseImportacionServicio\\.${to}`), `${from} -> ${to}`);
  }
});

test('Completada exige reconciliacion previa', () => {
  const lines = machine.split(/\r?\n/).filter((value) => value.includes('FaseImportacionServicio.Completada'));
  assert.ok(lines.length >= 2);
  const transitionLines = lines.filter((value) => value.includes('Agregar(t,'));
  assert.equal(transitionLines.length, 1);
  assert.match(transitionLines[0], /Agregar\(t, FaseImportacionServicio\.Reconciliada, FaseImportacionServicio\.Completada/);
  assert.match(machine, /Function PuedeCompletar[\s\S]*origen = FaseImportacionServicio\.Reconciliada/);
});

test('rechaza saltos con codigo seguro y audita sin payload', () => {
  assert.match(machine, /INVALID_STATE_TRANSITION/);
  assert.match(machine, /_audit\.Registrar/);
  assert.doesNotMatch(machine, /ExternalKey|NombreArchivo|MensajeVisible\)/);
});

test('modela persistencia conocida, retry y correlacion', () => {
  for (const field of ['PersistenciaConocida', 'Reintentable', 'CorrelationId']) assert.match(models, new RegExp(`Property ${field}`));
});

test('continuación agregada conserva cada frontera confirmada', () => {
  const expected = [
    'ExpedientesPlanificados', 'ExpedientesResueltos', 'ItemsSiiAlmacenados',
    'UniversoDocumentalConsultado', 'VinculacionesProcesadas', 'IndicesYXmlActualizados', 'Reconciliada'
  ];
  for (const phase of expected) {
    const line = machine.split(/\r?\n/).find(value => value.includes(`Agregar(t, FaseImportacionServicio.${phase},`));
    assert.ok(line, phase);
    assert.match(line, /ResultadoIncierto|Completada/, phase);
  }
});

test('ResultadoIncierto nunca salta directamente a completada', () => {
  const line = machine.split(/\r?\n/).find(value => value.includes('Agregar(t, FaseImportacionServicio.ResultadoIncierto,'));
  if (line) assert.doesNotMatch(line, /FaseImportacionServicio\.Completada/);
  assert.match(machine, /PuedeCompletar[\s\S]*FaseImportacionServicio\.Reconciliada/);
});
