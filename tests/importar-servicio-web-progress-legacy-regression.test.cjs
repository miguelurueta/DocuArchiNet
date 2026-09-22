const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

test('progreso moderno queda aislado de ejecución y códigos legacy', () => {
  const files=['js/workflow/importar-servicio-web/importar-servicio-web-progress-adapter.js','js/workflow/importar-servicio-web/importar-servicio-web-progress-view.js'];
  const source=files.map(file=>fs.readFileSync(file,'utf8')).join('\n');
  assert.doesNotMatch(source,/JSProgresBar|AlmacenaDocumentoTareaWorkflow|ClassAlmacenamiento|dato_lista|CTRLRETURN|\bCTRL\b|\bYES\b/);
  assert.doesNotMatch(source,/setInterval|setTimeout|StopRequested|Reintentar fallidos/);
});

test('archivos legacy permanecen fuera del registro de módulos modernos', () => {
  const project=fs.readFileSync('GestionDocumental-Docuarchi.net.vbproj','utf8');
  const registration=fs.readFileSync('workflow/Webworkflow.aspx.vb','utf8');
  assert.match(project,/importar-servicio-web-progress-adapter\.js/);
  assert.match(project,/importar-servicio-web-progress-view\.js/);
  assert.doesNotMatch(registration,/RegisterImportarServicioWebScript\([^\r\n]*JSProgresBar/);
});

test('vista usa espera indeterminada accesible y no ofrece cancelación o reintento', () => {
  const view=fs.readFileSync('js/workflow/importar-servicio-web/importar-servicio-web-progress-view.js','utf8');
  const markup=fs.readFileSync('workflow/Webworkflow.aspx','utf8');
  const css=fs.readFileSync('Styles/importar-servicio-web-modern.css','utf8');
  assert.match(markup,/importar-servicio-web-progress-status[^>]*role="status"[^>]*aria-live="polite"/);
  assert.match(css,/importar-servicio-web__progress-indicator/);
  assert.doesNotMatch(view,/percentage|porcentaje|setInterval|setTimeout/);
  assert.doesNotMatch(markup,/Reintentar fallidos|Cancelar ejecución/);
});
