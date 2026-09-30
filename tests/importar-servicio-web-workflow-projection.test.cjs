'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const read = path => fs.readFileSync(path, 'utf8');

test('backend transporta una proyección Workflow propia desde almacenamiento hasta DTO', () => {
  const storage=read('Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb');
  const steps=read('Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb');
  const orchestrator=read('Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb');
  const reconciliation=read('Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb');
  assert.match(storage,/CrearProyeccionWorkflow\(comando, imagen, idImagen\)/);
  assert.match(steps,/item\.ProyeccionDocumentoWorkflow = resultado\.ProyeccionDocumentoWorkflow/);
  assert.match(orchestrator,/WorkflowProjection = If\(confirmed, MapWorkflowProjection\(item\), Nothing\)/);
  assert.match(reconciliation,/PreserveWorkflowExecutionProjection/);
});

test('la implementación no altera el adaptador de almacenamiento ENLASE ni recarga el servidor', () => {
  const status=read('js/workflow/importar-servicio-web/importar-servicio-web-ui.js');
  assert.doesNotMatch(status,/Button_actualiza_trevie_seleccion|PageRequestManager|refreshDocumentListPartial/);
  assert.match(status,/isEnlase\(control\) \? appendEnlaseDocument\(item\) : appendWorkflowDocument\(item\)/);
});
