const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");

const page = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx"), "utf8");
const ui = fs.readFileSync(path.resolve(__dirname, "../js/workflow/importar-servicio-web/importar-servicio-web-ui.js"), "utf8");
const style = fs.readFileSync(path.resolve(__dirname, "../Styles/importar-servicio-web-modern.css"), "utf8");

test("modal tiene nombre accesible, estado vivo y cierre etiquetado", () => {
    assert.match(page, /role="dialog" aria-modal="true" aria-labelledby="importar-servicio-web-title"/);
    assert.match(page, /id="importar-servicio-web-status"[^>]*role="status"[^>]*aria-live="polite"/);
    assert.match(page, /id="importar-servicio-web-close"[^>]*aria-label="Cerrar importación"/);
});

test("UI gestiona Escape, Tab, foco inicial y restauración", () => {
    assert.match(ui, /event\.key === "Escape"/);
    assert.match(ui, /event\.key !== "Tab"/);
    assert.match(ui, /control\.closeButton\.focus\(\)/);
    assert.match(ui, /control\.trigger\.focus\(\)/);
    assert.match(ui, /getClientRects\(\)\.length > 0/);
});

test("espera global no expone porcentaje ni depende de JSProgresBar", () => {
    assert.match(ui, /Importando documentos\. Espere/);
    assert.doesNotMatch(ui, /JSProgresBar|porcentaje|percent/i);
    assert.match(style, /height: 100dvh/);
});
