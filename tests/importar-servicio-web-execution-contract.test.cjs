const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const read = (path) => fs.readFileSync(path, 'utf8');
const dto = read('DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb');
const repo = read('Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb');
const reconciliation = read('Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb');
const endpoint = read('webservice/WebServiceImportarServicioWebModern.asmx.vb');

test('Execute y Get comparten resultados v1 consultables', () => {
  for (const field of ['PersistenceKnown', 'Retryable', 'CorrelationId']) assert.match(dto, new RegExp(`Property ${field}`));
  assert.match(dto, /Class ExecuteImportIntentResponseDto[\s\S]*Items As IList\(Of ImportItemResultDto\)/);
  assert.match(dto, /SchemaVersion = "1\.0"/);
});

test('Execute proyecta solamente documentos confirmados para el frontend', () => {
  assert.match(endpoint, /Orchestrator\.Execute\(context, request\)[\s\S]*ProjectExecutionResult\(context, execution\)/);
  assert.match(reconciliation, /Function ProjectExecutionResult[\s\S]*AuthorizedSnapshot[\s\S]*For Each item In Project\(snapshot\)/);
  assert.match(reconciliation, /ProtectUnconfirmedExecution[\s\S]*DocumentId=Nothing[\s\S]*DocumentName=Nothing[\s\S]*ContentType=Nothing/);
});

test('la transicion usa parametros, transaccion y version optimista', () => {
  assert.match(repo, /Function ActualizarTransicion/);
  assert.match(repo, /version_token=@oldVersion/);
  assert.match(repo, /client_item_id=@clientId AND status=@previousStatus/);
  assert.match(repo, /transaction\.Commit/);
  assert.match(repo, /transaction\.Rollback/);
  for (const column of ['document_id', 'persistence_known', 'retryable', 'error_code', 'visible_message', 'correlation_id']) assert.match(repo, new RegExp(column));
  assert.match(repo, /INSERT INTO workflow_import_intent_transition/);
  assert.doesNotMatch(repo, /version_token='"\s*&/);
});

test('el DDL manual es reversible y solo amplía tablas modernas', () => {
  const up = read('Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-53-orquestacion-estados-compensacion/Sql/001-extend-import-intent-execution-state.sql');
  const down = read('Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-53-orquestacion-estados-compensacion/Sql/002-rollback-import-intent-execution-state.sql');
  assert.match(up, /CREATE TABLE workflow_import_intent_transition/);
  assert.match(down, /DROP TABLE IF EXISTS workflow_import_intent_transition/);
  assert.doesNotMatch(up, /\b(ra_|wf_log_|tareas_workflow)\w*/i);
});
