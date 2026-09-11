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
const dtos = fs.readFileSync(path.join(root, "DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb"), "utf8");
const service = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx.vb"), "utf8");
const factory = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalImportHttpClientFactory.vb"), "utf8");

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

test("la consulta usa exactamente el codigo de barras autorizado del request", () => {
  assert.match(service, /String\.IsNullOrWhiteSpace\(request\.CodigoBarras\)/);
  assert.match(service, /request\.CodigoBarras = request\.CodigoBarras\.Trim\(\)/);
  assert.doesNotMatch(service, /ResolveTaskBarcode/);
  assert.doesNotMatch(dtos.slice(dtos.indexOf('Public Class CreateImportIntentRequestDto')), /Property CodigoBarras|Property Recibo/);
});

test("la consulta moderna usa token y consultarInformacionSello con codigo de barras", () => {
  assert.match(dtos, /Class QueryItemsRequestDto[\s\S]*Public Property CodigoBarras As String/);
  assert.match(client, /"solicitarToken"/);
  assert.match(client, /"consultarInformacionSello"/);
  assert.match(client, /_baseUri\.AbsoluteUri\.TrimEnd\("\/"c\)/);
  assert.match(client, /Uri\.UriSchemeHttp AndAlso baseUri\.Scheme <> Uri\.UriSchemeHttps/);
  assert.match(client, /uri\.Scheme <> Uri\.UriSchemeHttp AndAlso uri\.Scheme <> Uri\.UriSchemeHttps/);
  assert.match(client, /IsAllowedDownloadHost\(uri\.Host\)/);
  assert.match(client, /normalizedHost\.EndsWith\("\." & normalizedAllowed, StringComparison\.OrdinalIgnoreCase\)/);
  assert.match(client, /DnsSafeHost\.ToUpperInvariant\(\)/);
  assert.match(client, /SII_RESOURCE_HOST_NOT_ALLOWED_/);
  assert.match(client, /{"radicado", codigoBarras}/);
  assert.match(client, /CodigoBarras\.Trim\(\)\.Length > 15/);
  assert.match(client, /SII_TOKEN_INVALID_CREDENTIALS/);
  assert.match(client, /SiiImportContractMapper\.NormalizeSource/);
  assert.match(client, /SiiImportContractMapper\.ContentTypeForFormat/);
  assert.doesNotMatch(client, /items\?taskId|preview\/|resource\//);
  assert.match(service, /New ExternalImportHttpClientFactory\(True\)/);
  assert.match(factory, /UseDefaultCredentials = useDefaultCredentials/);
  assert.match(client, /Encoding\.ASCII\.GetBytes/);
  assert.match(client, /DownloadSelectedAsync\(selected, request\.CorrelationId, cancellationToken, Nothing, Nothing,[\s\S]*request\.OperationId, request\.TaskId, Nothing, selected\.CodigoBarras, request\.ExternalKey\)/);
  assert.match(client, /content\.LongLength/);
  assert.match(client, /DateTime\.UtcNow\.AddMinutes\(5\)/);
});
