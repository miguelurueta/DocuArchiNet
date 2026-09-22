const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const markup = fs.readFileSync("workflow/Webworkflow.aspx", "utf8");
const css = fs.readFileSync("Styles/importar-servicio-web-modern.css", "utf8");
const ui = fs.readFileSync("js/workflow/importar-servicio-web/importar-servicio-web-ui.js", "utf8");

test("el panel tiene nombre accesible, estado anunciado y navegación de retorno", () => {
  assert.match(markup, /id="importar-servicio-web-preview"[^>]+aria-labelledby="importar-servicio-web-preview-title"/);
  assert.match(markup, /id="importar-servicio-web-preview-status"[^>]+role="status"[^>]+aria-live="polite"/);
  assert.match(markup, />Volver a la lista</);
  assert.match(markup, /title="Recurso externo temporal"[^>]+sandbox="allow-same-origin"/);
  assert.match(markup, /Esta vista no representa un documento almacenado/);
});

test("la vista restaura foco y scroll sin solicitar de nuevo por resize", () => {
  assert.match(ui, /scrollTop: control\.body\.scrollTop/);
  assert.match(ui, /control\.body\.scrollTop = saved\.scrollTop/);
  assert.match(ui, /saved\.focus\.focus\(\)/);
  assert.match(ui, /function restoreTriggerFocus\(control\)/);
  assert.match(ui, /closest\("\.dropright"\)/);
  assert.match(ui, /querySelector\("\.dropdown-toggle"\)/);
  assert.doesNotMatch(ui, /addEventListener\(["']resize["'][\s\S]{0,300}(?:getPreview|preview\.open)/);
});

test("el CSS ofrece panel lateral y subvista de ancho reducido", () => {
  assert.match(css, /@media \(min-width: 761px\)[\s\S]+grid-template-columns/);
  assert.match(css, /@media \(max-width: 760px\)[\s\S]+100dvh/);
  assert.match(css, /importar-servicio-web__preview-title:focus/);
});

test("documento importado se resuelve solo contra fila autorizada renderizada por servidor", () => {
  assert.match(ui, /GridView_list_documento_relacion_wf/);
  assert.match(ui, /row\.getAttribute\("id_wf"\)/);
  assert.match(ui, /row\.getAttribute\("idd_wf"\)/);
  assert.match(ui, /text\(parts\[5\]\) !== trustedTaskId/);
  assert.match(ui, /Button_selecion_treview_documento/);
  assert.doesNotMatch(ui, /__doPostBack|Visualiza_documento_workflow_visor/);
});
