const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const docsRoot = path.join(root, 'Doc', 'Actualizacion', 'Login', 'Implementacion', 'DOC-92');
const manifestPath = path.join(docsRoot, 'diagram-contract.json');
const manifest = JSON.parse(fs.readFileSync(manifestPath, 'utf8'));

let mermaidPromise;
async function mermaidParser() {
  if (!mermaidPromise) {
    mermaidPromise = (async () => {
      const { JSDOM } = await import('../tools/opsxj/node_modules/jsdom/lib/api.js');
      const dom = new JSDOM('<!doctype html><html><body></body></html>');
      global.window = dom.window;
      global.document = dom.window.document;
      global.navigator = dom.window.navigator;
      const { default: mermaid } = await import('../tools/opsxj/node_modules/mermaid/dist/mermaid.esm.min.mjs');
      mermaid.initialize({ startOnLoad: false, securityLevel: 'strict' });
      return mermaid;
    })();
  }
  return mermaidPromise;
}

function commentValues(source, label, file) {
  const match = source.match(new RegExp(`^%% ${label}: (.+)$`, 'm'));
  assert.ok(match, `${file}: comentario ${label} ausente`);
  return match[1].split(' | ').map(value => value.trim());
}

test('DOC-92 exige el inventario completo de documentos y diagramas Mermaid válidos', async () => {
  assert.equal(manifest.notation, 'Mermaid');
  assert.deepEqual(manifest.requiredDocuments, [
    'README.md',
    '01-ARQUITECTURA-Y-DIAGRAMAS.md',
    '02-CASOS-DE-USO.md',
    '03-INVENTARIO-TECNICO.md',
    '04-VALIDACION-Y-PENDIENTES.md'
  ]);
  assert.deepEqual(manifest.requiredDiagrams.map(item => item.path), [
    'Diagramas/01-componentes-clases.mmd',
    'Diagramas/02-secuencia-crear-consultar.mmd',
    'Diagramas/03-secuencia-intentos-finalizacion.mmd',
    'Diagramas/04-secuencia-reenvio-expiracion.mmd'
  ]);

  for (const document of manifest.requiredDocuments) {
    assert.ok(fs.existsSync(path.join(docsRoot, document)), `${document}: documento obligatorio ausente`);
  }

  const mermaid = await mermaidParser();
  for (const diagram of manifest.requiredDiagrams) {
    const absolute = path.join(docsRoot, diagram.path);
    assert.ok(fs.existsSync(absolute), `${diagram.path}: diagrama obligatorio ausente`);
    const source = fs.readFileSync(absolute, 'utf8');
    try {
      await mermaid.parse(source);
    } catch (error) {
      assert.fail(`${diagram.path}: sintaxis Mermaid inválida: ${error.message}`);
    }
    assert.match(source, /^%% Convención: CODE.*EXT.*CONCEPT/m, `${diagram.path}: convención ausente`);
    for (const relative of commentValues(source, 'Fuentes', diagram.path)) {
      assert.ok(fs.existsSync(path.join(root, relative)), `${diagram.path}: fuente inexistente ${relative}`);
    }
    assert.deepEqual(commentValues(source, 'Referencias CODE', diagram.path), diagram.symbols, `${diagram.path}: referencias distintas del manifiesto`);
    for (const signature of diagram.signatures) {
      assert.ok(source.includes(signature), `${diagram.path}: firma ausente o inconsistente: ${signature}`);
    }
  }
});

test('DOC-92 resuelve referencias CODE y limita exclusiones explícitamente', () => {
  const declared = new Set([...Object.keys(manifest.dotnetDeclarations), ...Object.keys(manifest.dotnetSymbols)]);
  for (const symbol of manifest.requiredDiagrams.flatMap(item => item.symbols)) {
    assert.ok(declared.has(symbol), `${symbol}: referencia CODE sin contrato estructural`);
  }
  assert.deepEqual(Object.keys(manifest.excludedKinds).sort(), ['conceptual', 'external']);
  for (const actor of manifest.excludedKinds.external) assert.match(actor, /^EXT:/, `${actor}: externo sin prefijo EXT`);
  for (const concept of manifest.excludedKinds.conceptual) assert.match(concept, /^CONCEPT:/, `${concept}: concepto sin prefijo CONCEPT`);
  assert.ok(Object.keys(manifest.dotnetImplements).length > 0, 'Falta contrato interfaz/implementación');
});

test('DOC-92 inventaría tipos, firmas, casos de uso y límites sin inventar capas', () => {
  const inventory = fs.readFileSync(path.join(docsRoot, '03-INVENTARIO-TECNICO.md'), 'utf8');
  for (const declaration of Object.values(manifest.dotnetDeclarations)) {
    assert.ok(inventory.includes(`\`${declaration.type}\``), `inventario: tipo ausente ${declaration.type}`);
  }
  for (const symbol of Object.values(manifest.dotnetSymbols)) {
    assert.ok(inventory.includes(symbol.method), `inventario: método ausente ${symbol.type}.${symbol.method}`);
  }
  const useCases = fs.readFileSync(path.join(docsRoot, '02-CASOS-DE-USO.md'), 'utf8');
  for (let number = 1; number <= 10; number += 1) {
    assert.match(useCases, new RegExp(`## UC-${String(number).padStart(2, '0')}\\b`), `caso de uso UC-${number} ausente`);
  }
  const allDocs = manifest.requiredDocuments.map(file => fs.readFileSync(path.join(docsRoot, file), 'utf8')).join('\n');
  assert.match(allDocs, /no existe.*Controller.*Service.*endpoint/is);
  assert.match(allDocs, /No aplica/);
  assert.match(allDocs, /no demuestra por sí sola/i);
  assert.match(allDocs, /correspondencia estructural/i);
});

test('DOC-92 integra Mermaid y Roslyn en CI', () => {
  const ci = fs.readFileSync(path.join(root, '.github', 'workflows', 'opsxj-validation.yml'), 'utf8');
  assert.match(ci, /node --test tests\/doc92-technical-documentation\.test\.cjs/);
  assert.match(ci, /Doc72SourceValidator\.csproj -- \.\/Doc\/Actualizacion\/Login\/Implementacion\/DOC-92\/diagram-contract\.json \./);
});
