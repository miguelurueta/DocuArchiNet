const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const registry = fs.readFileSync(path.join(root, "Services/Workflow/ImportarServicioWeb/RegistroClientesProveedoresImportacion.vb"), "utf8");
const syncRegistry = fs.readFileSync(path.join(root, "Services/Workflow/ImportarServicioWeb/RegistroProveedoresImportacion.vb"), "utf8");
const interfaces = fs.readFileSync(path.join(root, "Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb"), "utf8");
const provider = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb"), "utf8");
const client = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb"), "utf8");

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
  assert.doesNotMatch(provider, /\.Result\b|\.Wait\s*\(|GetAwaiter\s*\(/);
  assert.match(provider, /Await _client\.QueryItemsAsync/);
});

test("proveedor SII expone contrato comun y cliente usa transporte B02", () => {
  assert.match(provider, /Implements IExternalImportProviderClient/);
  assert.match(provider, /CanonicalProviderId As String = "INTEGRACIONSII"/);
  assert.match(provider, /ResolveCapabilitiesAsync/);
  assert.match(provider, /QueryItemsAsync/);
  assert.match(provider, /GetPreviewAsync/);
  assert.match(client, /ExternalImportHttpTransport/);
  assert.match(client, /_transport\.SendAsync/);
  assert.match(client, /maximumResponseBytes/);
  assert.match(client, /cancellationToken/);
});
