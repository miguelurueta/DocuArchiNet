const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const root = path.resolve(__dirname, "..");
const adapter = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiLegacyResultAdapter.vb"), "utf8");
const dto = fs.readFileSync(path.join(root, "DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb"), "utf8");

test("traduccion legacy queda confinada al adaptador", () => {
  for (const code of ["YES", "CTRL", "CTRLRETURN", "dato_lista"]) assert.match(adapter, new RegExp(code));
  assert.doesNotMatch(dto, /CTRLRETURN|dato_lista/);
});

test("adaptador elimina delimitadores inyectados", () => {
  assert.match(adapter, /Replace\("\|", String\.Empty\)/);
  assert.match(adapter, /IMPORT_ERROR/);
});
