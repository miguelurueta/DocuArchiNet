const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const read = (path) => fs.readFileSync(path, 'utf8');
const dto = read('DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb');
const provider = read('Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb');
const client = read('Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb');
const mapper = read('Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiEnlaseAnnexContractMapper.vb');
const service = read('webservice/WebServiceImportarServicioWebModern.asmx.vb');

test('publica capacidad aditiva sin crear otro proveedor', () => {
  assert.match(dto, /Public Property Capability As String/);
  assert.match(provider, /AnnexesEnlaseCapability As String = "ANEXOS_RADICADO_ENLASE"/);
  assert.equal((provider.match(/CanonicalProviderId As String/g) || []).length, 1);
  assert.match(provider, /SII_CAPABILITY_UNSUPPORTED/);
});

test('usa contexto ENLASE autoritativo y no acepta codigo de barras del navegador', () => {
  assert.match(service, /ID_TAREA_SELECCIONDA_ENLACE/);
  assert.match(service, /ENLASE_ACTIVITY_REQUIRED/);
  assert.match(service, /ENLASE_TASK_CONTEXT_MISMATCH/);
  assert.match(service, /request\.CodigoBarras = trustedBarcode/);
});

test('consulta consultarRadicado con transporte y telemetria compartidos', () => {
  assert.match(client, /Endpoint\("consultarRadicado"\)/);
  assert.match(client, /CONSULTAR_ANEXOS_RADICADO_ENLASE/);
  assert.match(client, /RequestTokenAsync/);
  assert.doesNotMatch(client, /New HttpClient/);
});

test('mapea idanexo y falla cerrado ante vacios o duplicados', () => {
  assert.match(mapper, /ExternalKey = annex\.IdAnexo/);
  assert.match(mapper, /HashSet\(Of String\)/);
  assert.match(mapper, /SII_ANNEX_ID_INVALID/);
  assert.doesNotMatch(mapper, /ExternalKey = annex\.Url/);
});

test('preview reconsulta identidad y conserva streaming seguro', () => {
  assert.match(client, /GetAnnexPreviewContentAsync/);
  assert.match(client, /SiiEnlaseAnnexContractMapper\.Resolve/);
  assert.match(client, /DownloadSelectedAsync/);
  assert.match(client, /IsAllowedDownloadHost/);
});

test('fixture saneado contiene una imagen contractual', () => {
  const fixture = JSON.parse(read('Tests/Fixtures/Workflow/ImportarServicioWeb/sii-enlase-annexes.json'));
  assert.equal(fixture.imagenes.length, 1);
  assert.equal(fixture.imagenes[0].idanexo, 'ANX-1001');
  assert.equal(/token|clave|password/i.test(JSON.stringify(fixture)), false);
});
