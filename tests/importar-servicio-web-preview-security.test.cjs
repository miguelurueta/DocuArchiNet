const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const preview = require("../js/workflow/importar-servicio-web/importar-servicio-web-preview.js");
const source = fs.readFileSync("js/workflow/importar-servicio-web/importar-servicio-web-preview.js", "utf8");
const ui = fs.readFileSync("js/workflow/importar-servicio-web/importar-servicio-web-ui.js", "utf8");
const storage = fs.readFileSync("workflow/ClassAlmacenamiento.vb", "utf8");

test("el handler se deriva solo de un descriptor base64url válido", () => {
  const descriptor = "Abc_def-123456789012345678901234";
  assert.equal(preview.descriptorUrl(descriptor), "../workflow/ImportarServicioWebPreview.ashx?d=" + encodeURIComponent(descriptor));
  for (const unsafe of ["", "https://sii.example/recurso", "../../web.config", "token?x=1", "abc/def"]) {
    assert.throws(() => preview.descriptorUrl(unsafe), /PREVIEW_DESCRIPTOR_INVALID/);
  }
});

test("preview reutiliza api.getPreview y no crea transporte ni usa URL externa", () => {
  assert.match(source, /api\.getPreview/);
  assert.match(source, /ExternalKey/);
  assert.doesNotMatch(source, /\bfetch\s*\(|XMLHttpRequest|\.ExternalUrl|\.Url\b|["']data:[^"']*;base64,/i);
  assert.doesNotMatch(ui, /window\.open\s*\(/);
});

test("el cambio no incorpora almacenamiento legacy en la vista", () => {
  assert.doesNotMatch(source + ui, /ClassAlmacenamiento|AlmacenaDocumentoTareaWorkflow/);
  assert.match(storage, /Function AlmacenaDocumentoTareaWorkflow/);
});

test("la identidad interna no se convierte directamente en selección del visor", () => {
  assert.match(ui, /importedViewer\.canOpen/);
  assert.match(ui, /parts\[1\].*id/);
  assert.match(ui, /parts\[5\].*trustedTaskId/);
  assert.doesNotMatch(ui, /selection\s*=\s*internalDocumentId/);
});
