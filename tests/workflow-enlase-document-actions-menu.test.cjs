const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const page = fs.readFileSync(path.join(root, "workflow/Webworkflow.aspx"), "utf8");
const css = fs.readFileSync(path.join(root, "Styles/workflow-centro-trabajo-moderno.css"), "utf8");

function occurrences(source, marker) {
  return source.split(marker).length - 1;
}

test("agrupa las acciones de ENLASE sin alterar sus contratos legacy", () => {
  assert.match(page, /ID="Panel_admon_documentos"[^>]*CssClass="modal_content_general ctw-enlase-document-modal"/);
  assert.match(page, /id="ctw-enlase-document-actions-toggle"[^>]*>\s*Acciones/);
  assert.match(
    page,
    /id="ctw-enlase-document-actions-menu"[\s\S]*?C-DW-ACTU-INDICE-ENLACE[\s\S]*?id="btnLoadFileEnlace"[\s\S]*?id="a_adj_service_web"[\s\S]*?C-DW-DEL-IMAGE-ENLACE/
  );
  assert.equal(occurrences(page, 'id="btnLoadFileEnlace"'), 1);
  assert.equal(occurrences(page, 'id="a_adj_service_web"'), 1);
});

test("mantiene el colapso fuera del menu y expone ambos estados", () => {
  assert.match(
    page,
    /id="ctw-enlase-document-actions-menu"[\s\S]*?<\/div>\s*<\/div>\s*<button id="sidebarCollapse_"/
  );
  assert.match(page, /id="sidebarCollapse_"[^>]*aria-controls="sidebar__"[^>]*aria-expanded="true"[^>]*aria-label="Ocultar lista de documentos"/);
  assert.match(page, /id="da_show-sidebar__"[^>]*aria-controls="sidebar__"[^>]*aria-expanded="false"[^>]*aria-label="Mostrar lista de documentos"/);
  assert.match(page, /fa-chevron-left/);
  assert.match(page, /fa-chevron-right/);
  assert.match(page, /#sidebarCollapse_.*?attr\('aria-expanded', 'false'\)/s);
  assert.match(page, /#da_show-sidebar__.*?attr\('aria-expanded', 'true'\)/s);
});

test("el menu de ENLASE limita su altura y permite desplazamiento interno", () => {
  assert.match(
    css,
    /\.ctw-enlase-document-modal #title_treview\s*\{[^}]*position:\s*relative;[^}]*z-index:\s*var\(--ctw-z-document-menu\);[^}]*overflow:\s*visible;/s
  );
  assert.match(
    css,
    /\.ctw-enlase-document-actions \.dropdown-menu\s*\{[^}]*max-height:\s*60vh;[^}]*overflow-x:\s*hidden;[^}]*overflow-y:\s*auto;/s
  );
  assert.match(
    css,
    /\.ctw-enlase-document-actions > \.dropdown-toggle\s*\{[^}]*color:\s*var\(--ctw-blue\) !important;/s
  );
});
