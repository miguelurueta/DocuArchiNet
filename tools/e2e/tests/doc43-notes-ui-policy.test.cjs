const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..', '..', '..');
const js = fs.readFileSync(path.join(root, 'js', 'workflow', 'Webworkflow.js'), 'utf8');
const page = fs.readFileSync(path.join(root, 'workflow', 'Webworkflow.aspx'), 'utf8');
const codeBehind = fs.readFileSync(path.join(root, 'workflow', 'Webworkflow.aspx.vb'), 'utf8');
const styles = fs.readFileSync(path.join(root, 'Styles', 'workflow-notes-modern.css'), 'utf8');
const uiSpec = fs.readFileSync(path.join(root, 'tools', 'e2e', 'tests', 'doc43-notes-ui.spec.cjs'), 'utf8');
const uiRunner = fs.readFileSync(path.join(root, 'tools', 'e2e', 'scripts', 'run-doc43-notes-ui-interactive.cjs'), 'utf8');
const modernBlock = js.slice(js.indexOf('window.WorkflowNotesModern'), js.indexOf('//REGISTRA EVENTOS GREDVIEW GRUPO'));

test('DOC-43 registra la UI de Notas detrás del gate y preserva legacy', () => {
  assert.match(page, /data-workflow-notes-modern="true"/);
  assert.match(codeBehind, /Panel_notas_modernas\.Visible = WorkflowCentroTrabajoModernActive/);
  assert.match(codeBehind, /Panel_Buttonanotacion\.Visible = Not WorkflowCentroTrabajoModernActive/);
  assert.match(page, /GridView_lista_notas/);
});

test('DOC-43 usa el contrato moderno completo con idTarea y versión explícitos', () => {
  for (const operation of ['ListarNotas', 'ContarNotas', 'CrearNota', 'ActualizarNota', 'EliminarNota']) {
    assert.match(modernBlock, new RegExp(operation));
  }
  assert.match(modernBlock, /idNota: editingNote\.IdNota/);
  assert.match(modernBlock, /version: editingNote\.Version/);
  assert.doesNotMatch(modernBlock, /ID_TAREA_SELECCIONDA|Session\s*\(/i);
});

test('DOC-43 renderiza texto seguro y maneja conflicto y doble envío', () => {
  assert.match(modernBlock, /textContent/);
  assert.doesNotMatch(modernBlock, /innerHTML/);
  assert.match(modernBlock, /VersionConflict/);
  assert.match(modernBlock, /save\.disabled = true/);
  assert.match(modernBlock, /remove\.disabled = true/);
});

test('DOC-43 cubre estados recuperables y contenido especial sin HTML dinámico', () => {
  assert.match(modernBlock, /Aún no hay notas/);
  assert.match(modernBlock, /No fue posible cargar las notas/);
  assert.match(modernBlock, /Nota guardada/);
  assert.match(modernBlock, /VersionConflict/);
  assert.match(modernBlock, /workflow-notes-modern-retry/);
  assert.match(modernBlock, /JSON\.stringify\(data\)/);
  assert.match(modernBlock, /textContent = item\.Contenido/);
  assert.doesNotMatch(modernBlock, /\.html\s*\(|innerHTML/);
});

test('DOC-43 recarga dos selecciones consecutivas con la tarea explícita vigente', () => {
  assert.match(modernBlock, /let idTarea = input \? Number\(input\.value\) : 0/);
  assert.match(modernBlock, /const selectedTaskId = input \? Number\(input\.value\) : 0/);
  assert.match(modernBlock, /idTarea = selectedTaskId/);
  assert.match(modernBlock, /input\.addEventListener\('change', loadSelectedTask\)/);
  assert.match(modernBlock, /if \(editor && !editor\.hidden\) closeEditor\(\)/);
});

test('DOC-43 implementa diálogo, foco, Escape y objetivos táctiles del modelo', () => {
  assert.match(page, /role="dialog"/);
  assert.match(page, /aria-modal="true"/);
  assert.match(page, /workflow-notes-modern-cancel/);
  assert.match(modernBlock, /event\.key === 'Escape'/);
  assert.match(modernBlock, /returnFocus\.focus\(\)/);
  assert.match(styles, /focus-visible/);
  assert.match(styles, /min-height: 40px/);
  assert.match(styles, /@media \(max-width: 680px\)/);
});

test('DOC-43 conserva la composición cronológica aprobada sin GridView moderno', () => {
  assert.match(page, /<ol id="workflow-notes-modern-list"/);
  assert.match(page, /notes-subtitle/);
  assert.match(page, /notes-footer/);
  assert.match(styles, /grid-template-columns: 40px minmax\(0, 1fr\) auto/);
  assert.doesNotMatch(page.slice(page.indexOf('Panel_notas_modernas'), page.indexOf('<!--nota_flujo-->')), /GridView/);
});

test('DOC-43 reutiliza sesión autenticada y captura secretos solo por consola', () => {
  assert.match(uiSpec, /createAuthenticatedWorkflowSession/);
  assert.match(uiRunner, /requireInteractiveConsole/);
  assert.match(uiRunner, /collectValue\([^\n]+AUTHORIZED_PASSWORD[^\n]+secret: true/);
  assert.match(uiSpec, /DOC43_E2E_ENVIRONMENT_AUTHORIZED/);
  assert.match(uiSpec, /DOC43_E2E_EXECUTION_AUTHORIZED/);
});
