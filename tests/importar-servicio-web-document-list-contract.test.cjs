const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const dto = fs.readFileSync('DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb', 'utf8');
const mapper = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb', 'utf8');
const service = fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb', 'utf8');
const model = fs.readFileSync('Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb', 'utf8');

test('contrato incluye mínimos documentales y excluye dato_lista', () => {
  for (const field of ['ReachedPhase', 'TaskId', 'DocumentName', 'ContentType']) assert.match(dto, new RegExp(`Property ${field}`));
  assert.doesNotMatch(dto, /dato_lista|physical|exception|sql/i);
});

test('mapper cubre enum y clasificaciones conservadoras', () => {
  const phases = [...model.matchAll(/^\s{4}([A-Za-z]+)\s*$/gm)].map(match => match[1]);
  for (const phase of ['Completada', 'Reconciliada', 'Parcial', 'Detenida', 'FallidaAntesDePersistir', 'ResultadoIncierto']) {
    assert.ok(phases.includes(phase));
    assert.match(mapper, new RegExp(`FaseImportacionServicio\\.${phase}`));
  }
  for (const code of ['DOCUMENT_RELATION_MISSING', 'DOCUMENT_RELATION_DUPLICATED', 'DOCUMENT_TASK_MISMATCH', 'IMPORT_RESULT_UNCERTAIN']) assert.match(mapper, new RegExp(code));
  assert.match(mapper, /If dto\.Status<>"Disponible" Then dto\.DocumentId=Nothing/);
});

test('lista deduplica por tarea y documento', () => {
  assert.match(service, /HashSet\(Of String\)/);
  assert.match(service, /mapped\.TaskId[\s\S]*mapped\.DocumentId/);
  assert.match(service, /Function AggregateStatus/);
  const duplicate = JSON.parse(fs.readFileSync('Tests/Fixtures/Workflow/ImportarServicioWeb/reconciliation-v1/duplicated-document.json', 'utf8'));
  assert.equal(new Set(duplicate.items.map(x => `${x.taskId}:${x.documentId}`)).size, 1);
});
