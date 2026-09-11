const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const steps = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb', 'utf8');
const models = fs.readFileSync('Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb', 'utf8');
const machine = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb', 'utf8');
const storage = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb', 'utf8');

test('pipeline descarga, prepara y almacena en fases explícitas', () => {
  assert.match(steps, /Class DownloadImportExecutionStep[\s\S]*DownloadAsync/);
  assert.match(steps, /Class PrepareImportExecutionStep[\s\S]*File\.WriteAllBytes/);
  assert.match(steps, /Class StoreImportExecutionStep[\s\S]*_metadata\.Resolver\(contexto\)[\s\S]*_storage\.Almacenar\(command\)/);
  assert.match(steps, /RecursoObtenido/);
  assert.match(steps, /ExpedientePreparado/);
  assert.match(steps, /IndicesActualizados/);
  assert.match(steps, /DocumentoAlmacenado/);
});

test('reintento sólo reinicia fallos conocidos sin documento persistido', () => {
  const orchestrator = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb', 'utf8');
  assert.match(orchestrator, /EsReintentoSeguro[\s\S]*Reintentable[\s\S]*PersistenciaConocida[\s\S]*IdDocumento\.HasValue/);
  assert.match(orchestrator, /IMPORT_RETRY_NOT_ALLOWED/);
  assert.match(machine, /FaseImportacionServicio\.Parcial, FaseImportacionServicio\.Validada/);
});

test('el comando usa radicado persistido y metadatos resueltos en servidor', () => {
  assert.match(steps, /\.Radicado = intencion\.ContextoOriginal\.Radicado/);
  assert.match(steps, /\.NombreGabinete = metadata\.NombreGabinete/);
  assert.match(steps, /\.NombreRutaWorkflow = metadata\.NombreRutaWorkflow/);
  assert.match(steps, /\.NombreClaseFormatoDocumento = metadata\.NombreClaseFormatoDocumento/);
  assert.match(steps, /\.TipoAlmacenamiento = 2/);
});

test('estado efímero no entra al repositorio y el temporal se elimina', () => {
  assert.match(models, /Estado efímero de una ejecución/);
  assert.match(steps, /File\.Delete\(item\.RutaArchivoPreparado\)/);
  assert.match(steps, /item\.ContenidoDescargado = Nothing/);
  assert.doesNotMatch(storage, /File\.WriteAllBytes|DownloadAsync/);
});

test('preparación deriva extensión desde metadatos SII confiables', () => {
  assert.match(steps, /ResolveTrustedExtension\(item\)/);
  assert.match(steps, /item\.MetadatosSii[\s\S]*item\.MetadatosSii\.Formato/);
  assert.match(steps, /Case "pdf" : Return "\.pdf"/);
  assert.match(steps, /Case "tif", "tiff" : Return "\.tif"/);
  assert.doesNotMatch(steps, /Path\.GetExtension\(If\(item\.NombreArchivo/);
});

test('reloj y auditoría permanecen en cada transición persistida', () => {
  assert.match(machine, /\.FechaUtc = _clock\.UtcNow\(\)/);
  assert.match(machine, /_audit\.Registrar\(cambio, True/);
  assert.match(machine, /_audit\.Registrar\(Nothing, False/);
});
