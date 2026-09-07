const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const service = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb', 'utf8');
const repository = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb', 'utf8');

test('reconstruye intención completa o item focal desde persistencia', () => {
  assert.match(service, /Function GetImportIntent[\s\S]*_repository\.Obtener/);
  assert.match(service, /Function ReconcileImportIntent[\s\S]*_repository\.ObtenerItem/);
  assert.match(repository, /INNER JOIN workflow_import_intent_item item ON item\.intent_id=intent\.intent_id/);
  assert.match(repository, /intent\.intent_id=@intentId[\s\S]*intent\.user_id=@userId[\s\S]*intent\.task_id=@taskId/);
  assert.match(repository, /registro_producion_documental[\s\S]*ID_DOCUMENTO_DOCUARCHI_ALMACEN=item\.document_id/);
});

test('todas las lecturas del repositorio son parametrizadas y no mutan', () => {
  assert.match(repository, /@intentId/);
  assert.match(repository, /@providerId/);
  assert.match(repository, /@externalKey/);
  assert.doesNotMatch(repository, /ExecuteNonQuery|\bINSERT\b|\bUPDATE\b|\bDELETE\b/i);
});

for (const name of ['completed', 'partial', 'uncertain']) {
  test(`fixture ${name} conserva contrato v1`, () => {
    const value = JSON.parse(fs.readFileSync(`Tests/Fixtures/Workflow/ImportarServicioWeb/reconciliation-v1/${name}.json`, 'utf8'));
    assert.equal(value.schemaVersion, '1.0');
    assert.ok(Array.isArray(value.items));
  });
}
