const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const registry = fs.readFileSync(path.join(root, "Services/Workflow/ImportarServicioWeb/RegistroClientesProveedoresImportacion.vb"), "utf8");
const syncRegistry = fs.readFileSync(path.join(root, "Services/Workflow/ImportarServicioWeb/RegistroProveedoresImportacion.vb"), "utf8");
const interfaces = fs.readFileSync(path.join(root, "Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb"), "utf8");

test("registro moderno resuelve clientes por identidad exacta sin fallback", () => {
  assert.match(registry, /IRegistroClientesProveedoresImportacion/);
  assert.match(registry, /StringComparer\.OrdinalIgnoreCase/);
  assert.match(registry, /_clientes\.TryGetValue\(Normalizar\(providerId\), cliente\)/);
  assert.match(registry, /PROVIDER_NOT_SUPPORTED/);
  assert.doesNotMatch(registry, /INTEGRACIONSII/);
});

test("preserva el registro sincrono y el contrato asincrono", () => {
  assert.match(syncRegistry, /Implements IRegistroProveedoresImportacion/);
  assert.match(interfaces, /Interface IExternalImportProviderClient/);
  assert.match(interfaces, /Task\(Of ResolveCapabilitiesResponseDto\)/);
  assert.match(interfaces, /Interface IRegistroClientesProveedoresImportacion/);
});

test("la resolucion moderna no bloquea tareas", () => {
  assert.doesNotMatch(registry, /\.Result\b|\.Wait\s*\(|GetAwaiter\s*\(/);
});
