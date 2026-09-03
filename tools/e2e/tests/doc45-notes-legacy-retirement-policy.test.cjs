'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..', '..', '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');
const annotationClass = read('workflow', 'Class_anotacion_tarea.vb');
const legacyService = read('webservice', 'WebServiceWorkflow.asmx.vb');
const modernService = read('webservice', 'WebServiceWorkflowNotesModern.asmx.vb');
const legacyPage = read('workflow', 'WebFormAnotacion.aspx');
const project = read('GestionDocumental-Docuarchi.net.vbproj');
const workflowClient = read('js', 'workflow', 'Webworkflow.js');
const inboundClient = read('js', 'radicacion', 'WebFormRadicacionEntrante.js');
const correspondenceClient = read('js', 'gestion_correspondencia', 'WebForm_interface_gestion_tramite.js');
const taskStyles = read('Styles', 'workflow-tareas-modernas.css');
const workCenterStyles = read('Styles', 'workflow-centro-trabajo-moderno.css');
const notesE2e = read('tools', 'e2e', 'tests', 'doc44-workflow-notes.spec.cjs');
const notesRunner = read('tools', 'e2e', 'scripts', 'run-doc44-workflow-notes-interactive.cjs');
const e2ePackage = JSON.parse(read('tools', 'e2e', 'package.json'));

test('DOC-45 retira únicamente la rutina duplicada sin consumidores', () => {
  assert.doesNotMatch(annotationClass, /Function\s+Eliminar_nota_tarea_workflow\b/i);
  assert.match(annotationClass, /Function\s+Eliminar_nota_service_workflow\b/i);
  assert.match(legacyService, /Service_delete_nota_tarea_workflow[\s\S]*Eliminar_nota_service_workflow/i);
  assert.match(modernService, /Public Function EliminarNota\(ByVal idTarea As Long/i);
});

test('DOC-45 conserva las superficies legacy con consumidores vivos', () => {
  assert.match(legacyPage, /Button_Show_Guardar/);
  assert.match(project, /Content Include="workflow\\WebFormAnotacion\.aspx"/i);
  for (const operation of [
    'Service_actualiza_nota_tarea_workflow',
    'Service_delete_nota_tarea_workflow',
    'Service_add_nota_tarea_workflow',
    'Service_contenido_nota_tarea_workflow'
  ]) {
    assert.match(legacyService, new RegExp(`Public Function ${operation}\\b`));
    assert.doesNotMatch(workflowClient, new RegExp(`${operation}\\(`));
    assert.match(inboundClient, new RegExp(`${operation}\\(`));
    assert.match(correspondenceClient, new RegExp(`${operation}\\(`));
  }
});

test('DOC-45 retira el fallback Workflow y preserva el consumidor moderno explícito', () => {
  const workflowPage = read('workflow', 'Webworkflow.aspx');
  const codeBehind = read('workflow', 'Webworkflow.aspx.vb');
  const taskSelection = read('workflow', 'Classselecciotarea.vb');
  assert.match(workflowPage, /data-workflow-notes-modern="true"/);
  assert.match(workflowPage, /Webworkflow\.js\?v=20260903-doc45-empty-ready1/);
  assert.match(workflowPage, /id="workflow-notes-modern-access"/);
  assert.match(codeBehind, /Panel_notas_modernas\.Visible = WorkflowCentroTrabajoModernPresentationEnabled/);
  assert.match(codeBehind, /Page\.Header IsNot Nothing AndAlso WorkflowCentroTrabajoModernPresentationEnabled/);
  assert.match(codeBehind, /workflow-notes-modern\.css\?v=20260902-doc45-ownership1/);
  assert.match(codeBehind, /If WorkflowCentroTrabajoModernPresentationEnabled Then[\s\S]*?workflowNotesModernBootstrap/);
  assert.doesNotMatch(codeBehind, /Panel_notas_modernas\.Visible = WorkflowCentroTrabajoModernActive/);
  for (const source of [workflowPage, codeBehind, taskSelection, workflowClient]) {
    assert.doesNotMatch(source, /Panel_Buttonanotacion|ImageButtonanotacion|GridView_lista_notas/);
  }
  assert.doesNotMatch(modernService, /Session(?:\.Item)?\s*\(\s*"ID_TAREA_SELECCIONDA"/i);
});

test('DOC-45 preserva color y glifos de acciones de tareas e índice', () => {
  const workflowPage = read('workflow', 'Webworkflow.aspx');
  assert.match(workflowPage, /workflow-tareas-modernas\.css\?v=20260903-doc45-icon-colors2/);
  assert.match(workflowPage, /workflow-centro-trabajo-moderno\.css\?v=20260902-doc45-icon-colors1/);
  for (const variant of ['primary', 'info', 'warning', 'success']) {
    assert.match(taskStyles, new RegExp(`#GridView2 td:first-child \\.btn-${variant} \\{[\\s\\S]*?background-color:[^;]+!important;`));
  }
  assert.match(taskStyles, /#GridView2 td:first-child \.btn > svg[\s\S]*?color: #fff !important;/);
  assert.match(workCenterStyles, /#da_show-sidebar_ \{[\s\S]*?background-color: #6d7fcc !important;/);
  assert.match(workCenterStyles, /#da_show-sidebar_ > svg[\s\S]*?color: #fff !important;/);
  assert.match(workCenterStyles, /#sidebarCollapse > svg[\s\S]*?color: currentColor !important;/);
  assert.match(notesE2e, /@doc45-unassigned-color[\s\S]*?\.btn-success:visible[\s\S]*?getComputedStyle\(element\)\.backgroundColor[\s\S]*?rgb\(33, 136, 56\)/);
  assert.match(notesRunner, /colorMode = process\.argv\[2\] === 'unassigned-color'/);
  assert.equal(e2ePackage.scripts['test:doc45:unassigned-color'], 'node scripts/run-doc44-workflow-notes-interactive.cjs unassigned-color');
});
