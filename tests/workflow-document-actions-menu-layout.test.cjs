const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const page = fs.readFileSync(path.join(root, "workflow/Webworkflow.aspx"), "utf8");
const css = fs.readFileSync(path.join(root, "Styles/workflow-centro-trabajo-moderno.css"), "utf8");

test("el menu de acciones documentales conserva todos sus controles accesibles", () => {
  assert.match(page, /id="ctw-document-actions-menu" class="dropdown-menu"/);
  assert.match(
    css,
    /\.ctw-document-more-actions \.ctw-document-menu__panel\s*\{[^}]*max-height:\s*60vh;[^}]*overflow-x:\s*hidden;[^}]*overflow-y:\s*auto;/s
  );
});
