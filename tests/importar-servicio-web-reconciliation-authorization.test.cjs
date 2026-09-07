const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const service = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb', 'utf8');
const repository = fs.readFileSync('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb', 'utf8');

test('rechazo opaco usa el mismo error para ausente o no autorizado', () => {
  assert.match(service, /AuthorizedSnapshot[\s\S]*_validator\.Validar\(context\)/);
  assert.match(service, /IMPORT_INTENT_UNAVAILABLE/);
  assert.doesNotMatch(service, /FORBIDDEN|NOT_FOUND/);
});

test('autoridad proviene del contexto y no del navegador', () => {
  assert.match(repository, /intent\.user_id=@userId/);
  assert.match(repository, /intent\.task_id=@taskId/);
  assert.match(service, /context\.ProviderId/);
  assert.doesNotMatch(service, /request\.TaskId|request\.ProviderId/);
});

test('fixture wrong-task no expone documento', () => {
  const value = JSON.parse(fs.readFileSync('Tests/Fixtures/Workflow/ImportarServicioWeb/reconciliation-v1/wrong-task.json', 'utf8'));
  assert.equal(value.items[0].status, 'Inconsistente');
  assert.equal(value.items[0].documentId, null);
});
