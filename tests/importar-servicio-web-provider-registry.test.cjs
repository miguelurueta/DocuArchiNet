const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const source = fs.readFileSync(path.resolve(__dirname, "../Services/Workflow/ImportarServicioWeb/RegistroProveedoresImportacion.vb"), "utf8");

test("normaliza y compara identidades sin distinguir mayusculas", () => {
  assert.match(source, /StringComparer\.OrdinalIgnoreCase/);
  assert.match(source, /If\(providerId, String\.Empty\)\.Trim\(\)/);
});

test("rechaza coleccion, proveedor, identidad y duplicado invalidos", () => {
  assert.match(source, /ArgumentNullException\("proveedores"\)/);
  assert.match(source, /proveedores nulos/);
  assert.match(source, /identidad canónica del proveedor es obligatoria/i);
  assert.match(source, /_proveedores\.ContainsKey\(providerId\)/);
});

test("proveedor desconocido falla cerrado sin fallback SII", () => {
  assert.match(source, /PROVIDER_NOT_SUPPORTED/);
  assert.doesNotMatch(source, /INTEGRACIONSII/);
  assert.match(source, /_proveedores\.TryGetValue\(identidad, proveedor\)/);
});
