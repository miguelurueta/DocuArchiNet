const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');

const src = fs.readFileSync('webservice/WebServiceImportarServicioWebModern.asmx.vb', 'utf8');
const operations = ['ResolveCapabilities', 'QueryItems', 'GetPreview', 'PreflightImport', 'CreateImportIntent', 'ExecuteImportIntent', 'GetImportIntent', 'ReconcileImportIntent'];

test('el borde ASMX serializa DTOs concretos y no objetos Task', () => {
  assert.doesNotMatch(src, /Public\s+Async\s+Function\s+(?:ResolveCapabilities|QueryItems|GetPreview)/i);
  for (const operation of ['ResolveCapabilities', 'QueryItems', 'GetPreview']) {
    assert.match(src, new RegExp(`Public\\s+Function\\s+${operation}\\b`, 'i'));
  }
});

function body(name) {
  const start = src.indexOf(`Public Function ${name}`) >= 0 ? src.indexOf(`Public Function ${name}`) : src.indexOf(`Public Async Function ${name}`);
  assert.notEqual(start, -1, name);
  const end = src.indexOf('End Function', start);
  return src.slice(start, end);
}

test('ASMX publica exactamente las ocho operaciones modernas', () => {
  for (const operation of operations) {
    assert.match(src, new RegExp(`Public (?:Async )?Function ${operation}\\b`), operation);
  }
  const published = [...src.matchAll(/Public (?:Async )?Function (\w+)\([^)]*\) As/g)].map((match) => match[1]);
  assert.deepEqual(published.filter((name) => operations.includes(name)).sort(), [...operations].sort());
});

test('los cinco métodos persistentes validan feature y contexto antes de componer', () => {
  for (const operation of operations.slice(3)) {
    const source = body(operation);
    assert.ok(source.indexOf('FeatureEnabled()') < source.indexOf('TryBuildImportContext'), operation);
    assert.ok(source.indexOf('TryBuildImportContext') < source.indexOf('Compose('), operation);
    assert.match(source, /Catch[\s\S]*IMPORT_UNAVAILABLE/);
  }
});

test('composición productiva conecta autorización, persistencia, pasos y reconciliación', () => {
  assert.match(src, /MySqlSiiImportAuthorizationRepository\(connections, executor, trustedContext\)/);
  assert.match(src, /MySqlImportIntentRepository/);
  assert.match(src, /DownloadImportExecutionStep/);
  assert.match(src, /StoreImportExecutionStep/);
  assert.match(src, /MySqlImportReconciliationRepository/);
});
