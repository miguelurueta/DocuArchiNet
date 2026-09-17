const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const factory = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportItemResultFactory.vb', 'utf8');
const machine = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb', 'utf8');
const boundaries = JSON.parse(fs.readFileSync('Tests/Fixtures/Workflow/ImportarServicioWeb/failure-injection-v1/boundaries.json', 'utf8'));
const expedient = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/PhysicalImportExpedientRepository.vb', 'utf8');
const storage = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb', 'utf8');
const relation = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Expedients/DocumentExpedientRelationAdapter.vb', 'utf8');
const cache = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportDocumentLinkCacheRepository.vb', 'utf8');
const indices = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Expedients/SiiDocumentIndexAdapter.vb', 'utf8');

test('incertidumbre no es reintentable y los fallos conocidos se clasifican', () => {
  assert.match(factory, /Reintentable = resultado\.Reintentable AndAlso item\.PersistenciaConocida/);
  assert.match(factory, /ResultadoIncierto/);
  assert.match(factory, /FallidaAntesDePersistir/);
  assert.match(factory, /Parcial/);
});

test('matriz cubre cada frontera y conserva efectos confirmados', () => {
  assert.deepEqual(boundaries.map(x => x.boundary), ['expedient', 'storage', 'relation', 'cache', 'indices']);
  for (const [index, value] of boundaries.entries()) assert.equal(value.confirmedBefore.length, index, value.boundary);
});

test('cada frontera distingue fallo conocido o ResultadoIncierto', () => {
  assert.match(expedient, /ResultadoIncierto[\s\S]*EXPEDIENT_CREATE_RESULT_UNKNOWN/);
  assert.match(storage, /DOCUMENT_STORAGE_UNCERTAIN/);
  assert.match(relation, /DOCUMENT_RELATION_WRITE_UNKNOWN/);
  assert.match(cache, /DOCUMENT_LINK_CACHE_WRITE_UNKNOWN/);
  assert.match(indices, /DOCUMENT_INDEX_UPDATE_UNKNOWN/);
});

test('reintento reconsulta antes de repetir efectos', () => {
  assert.match(expedient, /Dim existente = Buscar/);
  assert.match(relation, /Dim before = Consultar/);
  assert.match(cache, /Dim current = ReadOne/);
  assert.match(indices, /Dim persisted = _gateway\.LeerCampos/);
});

test('cada fase mutadora puede terminar parcial o incierta', () => {
  for (const phase of ['RecursoObtenido', 'ExpedientePreparado', 'IndicesActualizados', 'DocumentoAlmacenado']) {
    const line = machine.split(/\r?\n/).find(x => new RegExp(`Agregar\\(t, FaseImportacionServicio\\.${phase},`).test(x));
    assert.match(line, /Parcial/);
    assert.match(line, /ResultadoIncierto/);
  }
});
