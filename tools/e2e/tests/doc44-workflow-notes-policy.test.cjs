'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..', '..', '..');
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), 'utf8');
const page = read('workflow', 'Webworkflow.aspx');
const codeBehind = read('workflow', 'Webworkflow.aspx.vb');
const taskSelection = read('workflow', 'Classselecciotarea.vb');
const client = read('js', 'workflow', 'Webworkflow.js');
const service = read('webservice', 'WebServiceWorkflowNotesModern.asmx.vb');
const configuration = read('Web.config');
const styles = read('Styles', 'workflow-notes-modern.css');
const runner = read('tools', 'e2e', 'scripts', 'run-doc44-workflow-notes-interactive.cjs');
const spec = read('tools', 'e2e', 'tests', 'doc44-workflow-notes.spec.cjs');
const packageJson = read('tools', 'e2e', 'package.json');
const contractSpec = read('tools', 'e2e', 'tests', 'notes-workflow.spec.cjs');
const noteModel = read('Modelo', 'Workflow', 'Notas', 'NotasWorkflowModels.vb');
const noteDto = read('DTOs', 'Workflow', 'Notas', 'NotasWorkflowDtos.vb');
const noteRepository = read('Infrastructure', 'Repositories', 'Workflow', 'MySqlNotasWorkflowRepository.vb');
const modernService = read('webservice', 'WebServiceWorkflowNotesModern.asmx.vb');
const modernBlock = client.slice(client.indexOf('window.WorkflowNotesModern'), client.indexOf('//REGISTRA EVENTOS GREDVIEW GRUPO'));

test('DOC-44 limita el consumidor moderno a un cliente y contrato explícito', () => {
  assert.equal((client.match(/window\.WorkflowNotesModern\s*=/g) || []).length, 1);
  assert.match(modernBlock, /body:\s*JSON\.stringify\(data\)/);
  assert.doesNotMatch(modernBlock, /innerHTML|ID_TAREA_SELECCIONDA|Session\s*\(/i);
  for (const operation of ['ListarNotas', 'ConsultarNota', 'ContarNotas', 'CrearNota', 'ActualizarNota', 'EliminarNota']) {
    assert.match(modernBlock, new RegExp(`invoke\\('${operation}'`));
    assert.match(service, new RegExp(`Public Function ${operation}\\(ByVal idTarea As Long`));
  }
  assert.match(modernBlock, /idNota:\s*editingNote\.IdNota/);
  assert.match(modernBlock, /version:\s*editingNote\.Version/);
});

test('DOC-44 conserva presentación moderna única y configuración segura de entrega', () => {
  assert.match(codeBehind, /Panel_notas_modernas\.Visible = WorkflowCentroTrabajoModernPresentationEnabled/);
  assert.doesNotMatch(codeBehind, /Panel_notas_modernas\.Visible = WorkflowCentroTrabajoModernActive/);
  assert.doesNotMatch(codeBehind, /Panel_Buttonanotacion/);
  assert.doesNotMatch(taskSelection, /Panel_Buttonanotacion|modernNotesEnabled/);
  assert.match(configuration, /WorkflowCentroTrabajoModernActive" value="false"/i);
  assert.match(configuration, /WorkflowCentroTrabajoModernUsers" value=""/i);
  assert.match(configuration, /WorkflowCentroTrabajoModernGroups" value=""/i);
  assert.doesNotMatch(page, /GridView_lista_notas|ImageButtonanotacion/);
  assert.match(page, /id="workflow-notes-modern-access"[\s\S]*?aria-controls="Panel_notas_modernas"[\s\S]*?aria-haspopup="dialog"/);
  assert.match(page, /id="workflow-notes-modern-access-count"/);
  assert.match(page, /id="workflow-notes-modern-access-label"> Nueva nota/);
  assert.match(page, /ID="Panel_notas_modernas"[\s\S]*?role="dialog"[\s\S]*?aria-modal="true"[\s\S]*?hidden="hidden"/);
  assert.match(modernBlock, /document\.addEventListener\('click'[\s\S]*?closest\('#workflow-notes-modern-access'\)[\s\S]*?root\.hidden = false/);
  assert.match(modernBlock, /totalNotesLoaded === 0[\s\S]*?openEditor\(null, access\)/);
  assert.match(modernBlock, /totalNotesLoaded === null[\s\S]*?selectedTaskLoad \|\| loadSelectedTask\(\)[\s\S]*?then\(reveal\)/);
  assert.match(modernBlock, /totalNotes === 0 \? ' Nueva nota' : ' Notas'/);
  assert.match(modernBlock, /closeNotes[\s\S]*?root\.hidden = true[\s\S]*?getAccess\(\)[\s\S]*?access\.focus/);
  assert.match(modernBlock, /PageRequestManager\.getInstance\(\)\.add_endRequest\(loadSelectedTask\)/);
});

test('DOC-44 cubre estados, texto seguro, conflicto y bloqueo de doble mutación', () => {
  for (const message of ['Cargando notas', 'Aún no hay notas', 'No fue posible cargar las notas', 'VersionConflict']) {
    assert.match(modernBlock, new RegExp(message));
  }
  assert.match(modernBlock, /content\.textContent = item\.Contenido/);
  assert.match(modernBlock, /if \(save\.disabled\) return/);
  assert.match(modernBlock, /save\.disabled = true/);
  assert.match(modernBlock, /if \(!deletingNote \|\| deleteAccept\.disabled\) return/);
  assert.match(modernBlock, /deleteAccept\.disabled = true/);
});

test('DOC-44 mantiene diálogo accesible y foco contenido', () => {
  assert.match(page, /role="dialog"/);
  assert.match(page, /aria-modal="true"/);
  assert.match(page, /aria-describedby="workflow-notes-modern-text-help workflow-notes-modern-character-count"/);
  assert.match(modernBlock, /event\.key !== 'Tab'/);
  assert.match(modernBlock, /event\.shiftKey/);
  assert.match(modernBlock, /returnFocus\.focus\(\)/);
  assert.match(styles, /focus-visible/);
  assert.match(page, /id="workflow-notes-modern-delete-confirm"[\s\S]*?role="alertdialog"[\s\S]*?aria-describedby="workflow-notes-modern-delete-description"/);
  assert.doesNotMatch(modernBlock, /window\.confirm/);
  assert.match(modernBlock, /statusTimer[\s\S]*?setTimeout\(\(\) => setStatus\('', 'idle'\), 3500\)/);
  assert.match(styles, /height: min\(720px, calc\(100vh - 36px\)\)/);
  assert.match(styles, /\.notes-list[^}]*overflow-y: auto/);
});

test('DOC-45 calcula propiedad en servidor y ofrece lectura ampliada segura', () => {
  assert.match(noteModel, /Public Property PuedeGestionar As Boolean/);
  assert.match(noteDto, /Public Property PuedeGestionar As Boolean/);
  assert.match(noteRepository, /PuedeGestionar = idAutor = contexto\.IdUsuarioWorkflow AndAlso idActividad = tarea\.IdActividadOrigen/);
  assert.match(noteRepository, /at\.ID_USUARIO=@idUsuario/);
  assert.match(noteRepository, /DiagnosticarMutacionNoAplicada/);
  assert.match(noteRepository, /CodigosResultadoNotasWorkflow\.NotOwner/);
  assert.match(modernService, /\.PuedeGestionar = nota\.PuedeGestionar/);
  assert.doesNotMatch(noteDto, /IdAutorWorkflow/);
  assert.match(page, /id="workflow-notes-modern-viewer"[\s\S]*?role="dialog"[\s\S]*?note-viewer-content/);
  assert.match(modernBlock, /item\.PuedeGestionar === true/);
  assert.match(modernBlock, /viewerContent\.textContent = item\.Contenido/);
  assert.match(styles, /\.note-viewer-body[^}]*overflow-y: auto/);
  assert.match(styles, /-webkit-line-clamp: 5/);
});

test('DOC-44 no incorpora mutaciones legacy ni cambia el servicio compartido', () => {
  assert.doesNotMatch(modernBlock, /WebFormAnotacion|Class_anotacion_tarea|Button_Show_Guardar|GridView_lista_notas/);
  assert.doesNotMatch(service, /Session\s*\(\s*"ID_TAREA_SELECCIONDA"|Session\.Item\(\s*"ID_TAREA_SELECCIONDA"/i);
});

test('DOC-44 integra una E2E exclusiva con autorizaciones y restauración segura', () => {
  assert.match(packageJson, /test:doc44:workflow-notes/);
  assert.match(packageJson, /test:doc45:empty-notes/);
  assert.match(spec, /createAuthenticatedWorkflowSession/);
  assert.match(spec, /tip_event="seleccion_tarea_wf"/);
  assert.match(spec, /window\.hide_area_workflow_seleccion\(\)/);
  assert.match(spec, /auto_complex:visible/);
  assert.doesNotMatch(spec, /page\.reload/);
  assert.match(spec, /@doc45-empty-notes/);
  assert.match(spec, /DOC44_E2E_EMPTY_MODE/);
  assert.match(spec, /workflow-notes-modern-access-count'[\s\S]*?toHaveText\('0'\)/);
  assert.doesNotMatch(spec, /Hidden_id_tarea_selecionada[^\n]*(?:\.value\s*=|setAttribute\()/);
  for (const authorization of ['ENVIRONMENT_AUTHORIZED', 'EXECUTION_AUTHORIZED']) {
    assert.match(runner, new RegExp(`collectConfirmation\\(values, 'DOC44_E2E_${authorization}'`));
    assert.match(spec, new RegExp(`DOC44_E2E_${authorization}`));
  }
  assert.match(runner, /finally\s*\{/);
  assert.doesNotMatch(runner, /writeFile\(webConfigPath/);
  assert.match(runner, /assertSafeGate\(await fs\.readFile/);
  assert.match(runner, /\['localhost', '127\.0\.0\.1', '::1'\]\.includes/);
  assert.match(runner, /localSelfSigned \? 'true' : 'false'/);
  for (const negativeId of ['FOREIGN_TASK_ID', 'INACTIVE_TASK_ID', 'FOREIGN_NOTE_ID', 'NON_OWNER_NOTE_ID']) {
    assert.match(runner, new RegExp(`DOC44_E2E_${negativeId}`));
    assert.match(spec, new RegExp(`DOC44_E2E_${negativeId}`));
  }
  assert.match(spec, /ConsultarNota/);
  assert.match(spec, /TaskNotActive/);
  assert.match(spec, /NoteNotFound/);
  assert.match(contractSpec, /cursor-invalido-e2e/);
  assert.match(contractSpec, /staleUpdate/);
  assert.doesNotMatch(runner, /console\.log\([^\n]*(PASSWORD|cookie|connection)/i);
});
