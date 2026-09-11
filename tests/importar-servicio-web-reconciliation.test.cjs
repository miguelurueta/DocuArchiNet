const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const service = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb', 'utf8');
const repository = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb', 'utf8');
const webmethod = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
const sessionGate = fs.readFileSync('webservice/WorkflowPreviewSessionContextGate.vb', 'utf8');

test('reconstruye intención completa o item focal desde persistencia', () => {
  assert.match(service, /Function GetImportIntent[\s\S]*_repository\.Obtener/);
  assert.match(service, /Function ReconcileImportIntent[\s\S]*_repository\.ObtenerItem/);
  assert.match(repository, /INNER JOIN workflow_import_intent_item item ON item\.intent_id=intent\.intent_id/);
  assert.match(repository, /intent\.intent_id=@intentId[\s\S]*intent\.user_id=@userId[\s\S]*intent\.task_id=@taskId/);
  assert.match(repository, /_docuarchiConnections\.CreateOpenConnection/);
  assert.match(repository, /registro_producion_documental WHERE ID_DOCUMENTO_DOCUARCHI_ALMACEN=@documentId/);
  assert.match(repository, /FROM logdocuarchi WHERE id_tran=@documentId AND desc_op='Registra' AND MODULO_REGISTRO='WORKFLOW'/);
  assert.match(repository, /item\.CantidadDocumentos = relation\.Total/);
  assert.match(repository, /item\.CantidadRelaciones = relation\.SameTask/);
  assert.match(repository, /item\.CantidadRelacionesOtraTarea = relation\.OtherTask/);
  assert.match(repository, /GABINETE=\(SELECT scoped\.GABINETE[\s\S]*scoped\.ID_TAREA_WF=@taskId/);
  assert.match(repository, /NOMBRE_GABINETE=\(SELECT scoped\.GABINETE/);
  assert.doesNotMatch(repository, /registro_producion_documental[\s\S]*item\.document_id/);
  assert.doesNotMatch(webmethod, /New ModuleConnectionFactory\("MyDbContext"\)/);
  assert.match(webmethod, /String\.IsNullOrWhiteSpace\(session\.CadenaConexionDocuarchi\)[\s\S]*DOCUARCHI_RECONCILIATION_UNAVAILABLE[\s\S]*New DocuarchiModuleConnectionFactory/);
  assert.match(repository, /WORKFLOW_RECONCILIATION_UNAVAILABLE/);
  assert.match(repository, /DOCUARCHI_RECONCILIATION_UNAVAILABLE/);
  assert.match(webmethod, /SafeReconciliationCode/);
  assert.match(sessionGate, /If Not EsSesionGestionAutenticada\(requestContext\)[\s\S]*CadenaConexionWorkflow = CrearCadenaConexion\(requestContext\)[\s\S]*CadenaConexionDocuarchi = CrearCadenaConexion\(requestContext, "DA_"\)[\s\S]*Return resultado/);
});

test('todas las lecturas del repositorio son parametrizadas y no mutan', () => {
  assert.match(repository, /@intentId/);
  assert.match(repository, /@providerId/);
  assert.match(repository, /@externalKey/);
  assert.doesNotMatch(repository, /ExecuteNonQuery|\bINSERT\b|\bUPDATE\b|\bDELETE\b/i);
});

test('reconciliación sólo expone retry conocido y sin documento', () => {
  const mapper = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb', 'utf8');
  assert.match(mapper, /Retryable=item\.Reintentable AndAlso item\.PersistenciaConocida AndAlso Not item\.IdDocumento\.HasValue/);
  assert.doesNotMatch(mapper, /item\.Reintentable AndAlso Not item\.PersistenciaConocida/);
});

for (const name of ['completed', 'partial', 'uncertain']) {
  test(`fixture ${name} conserva contrato v1`, () => {
    const value = JSON.parse(fs.readFileSync(`Tests/Fixtures/Workflow/ImportarServicioWeb/reconciliation-v1/${name}.json`, 'utf8'));
    assert.equal(value.schemaVersion, '1.0');
    assert.ok(Array.isArray(value.items));
  });
}
