const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

test('cliente consume contrato autoritativo sin fabricar plan ni ExpedientId', () => {
  const source=fs.readFileSync('js/workflow/importar-servicio-web/importar-servicio-web-intent-client.js','utf8');
  assert.match(source,/response\.Requirements/);
  assert.match(source,/response\.EffectPlans/);
  assert.match(source,/response\.ContextFingerprint/);
  assert.doesNotMatch(source,/ExpedientId|executeImportIntent|AlmacenaDocumentoTareaWorkflow|ClassAlmacenamiento/);
});

test('preflight backend valida catálogo y plan sin consultar SII', () => {
  const source=fs.readFileSync('Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb','utf8');
  assert.match(source,/_documentTypes\.Resolver/);
  assert.match(source,/_planBuilder\.Build/);
  assert.match(source,/\.Executable = True/);
  assert.doesNotMatch(source,/SiiExternalImportProviderClient|QueryItems|Descargar/);
});
