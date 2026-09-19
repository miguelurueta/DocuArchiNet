const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const service = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
const provider = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb', 'utf8');
const handler = fs.readFileSync('workflow/ImportarServicioWebPreview.ashx.vb', 'utf8');

test('GetPreview descarga una vez y persiste esos mismos bytes', () => {
  const method = provider.match(/Public Async Function GetPreviewContentAsync[\s\S]*?End Function/)[0];
  assert.equal((method.match(/DownloadSelectedAsync/g) || []).length, 1);
  assert.match(method, /\.Content = content/);
  const endpoint = service.match(/Public Function GetPreview[\s\S]*?End Function/)[0];
  assert.equal((endpoint.match(/GetPreviewContentAsync/g) || []).length, 1);
  assert.match(endpoint, /ImportPreviewCompositionFactory\.CreateDescriptorService/);
});

test('handler GET y HEAD tienen cero dependencias o llamadas SII externas', () => {
  assert.doesNotMatch(handler, /SiiExternalImportProviderClient|GetPreviewContentAsync|DownloadSelectedAsync|consultarInformacionSello|solicitarToken/);
  assert.match(handler, /ImportPreviewCompositionFactory\.CreateContentService/);
});

test('ASMX y handler reutilizan una única fábrica de composición preview', () => {
  const composition = fs.readFileSync('Infrastructure/Workflow/ImportarServicioWeb/Preview/ImportPreviewCompositionFactory.vb', 'utf8');
  assert.match(service, /ImportPreviewCompositionFactory\.CreateDescriptorService/);
  assert.match(handler, /ImportPreviewCompositionFactory\.CreateContentService/);
  assert.equal((composition.match(/New ImportPreviewDescriptorRepository/g) || []).length, 1);
});

test('el descriptor público ya no reutiliza ExternalKey', () => {
  const endpoint = service.match(/Public Function GetPreview[\s\S]*?End Function/)[0];
  assert.doesNotMatch(endpoint, /DescriptorId\s*=\s*request\.ExternalKey/);
  assert.match(endpoint, /DescriptorId = created\.DescriptorId/);
});
