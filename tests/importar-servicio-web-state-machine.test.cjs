const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const read = (path) => fs.readFileSync(path, 'utf8');
const machine = read('Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb');
const models = read('Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb');

test('la maquina es pura y declara el recorrido normal completo', () => {
  assert.match(machine, /Creada[\s\S]*Validada/);
  assert.match(machine, /Validada[\s\S]*RecursoObtenido/);
  assert.match(machine, /RecursoObtenido[\s\S]*ExpedientePreparado/);
  assert.match(machine, /ExpedientePreparado[\s\S]*IndicesActualizados/);
  assert.match(machine, /IndicesActualizados[\s\S]*DocumentoAlmacenado/);
  assert.match(machine, /DocumentoAlmacenado[\s\S]*CacheActualizado/);
  assert.match(machine, /CacheActualizado[\s\S]*Completada/);
  assert.doesNotMatch(machine, /HttpContext|MySql|Session|ClassAlmacenamiento/);
});

test('rechaza saltos con codigo seguro y audita sin payload', () => {
  assert.match(machine, /INVALID_STATE_TRANSITION/);
  assert.match(machine, /_audit\.Registrar/);
  assert.doesNotMatch(machine, /ExternalKey|NombreArchivo|MensajeVisible\)/);
});

test('modela persistencia conocida, retry y correlacion', () => {
  for (const field of ['PersistenciaConocida', 'Reintentable', 'CorrelationId']) assert.match(models, new RegExp(`Property ${field}`));
});
