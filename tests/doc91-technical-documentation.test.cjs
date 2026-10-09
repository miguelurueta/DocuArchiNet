const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const docsRoot = path.join(root, 'Doc', 'Actualizacion', 'Login', 'Implementacion', 'DOC-91');
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

function commentValue(source, label, file) {
  const match = source.match(new RegExp(`^%% ${label}: (.+)$`, 'm'));
  assert.ok(match, `${file}: comentario ${label} ausente`);
  return match[1].split(' | ').map(value => value.trim());
}

test('DOC-91 exige inventario explícito y sintaxis Mermaid válida', async () => {
  assert.equal(manifest.notation, 'Mermaid');
  assert.deepEqual(manifest.requiredDiagrams.map(item => item.path), [
    'Diagramas/01-componentes-clases.mmd',
    'Diagramas/02-secuencia-seguridad.mmd',
    'Diagramas/03-validaciones-configuracion.mmd',
    'Diagramas/04-secuencia-session.mmd'
  ]);
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
    for (const relative of commentValue(source, 'Fuentes', diagram.path)) {
      assert.ok(fs.existsSync(path.join(root, relative)), `${diagram.path}: fuente inexistente ${relative}`);
    }
    const references = commentValue(source, 'Referencias CODE', diagram.path);
    assert.deepEqual(references, diagram.symbols, `${diagram.path}: referencias distintas del manifiesto`);
    for (const signature of diagram.signatures) {
      assert.ok(source.includes(signature), `${diagram.path}: firma ausente o inconsistente: ${signature}`);
    }
  }
});

test('DOC-91 resuelve todas las referencias CODE y restringe exclusiones', () => {
  const declared = new Set([
    ...Object.keys(manifest.dotnetDeclarations),
    ...Object.keys(manifest.dotnetSymbols)
  ]);
  for (const symbol of manifest.requiredDiagrams.flatMap(item => item.symbols)) {
    assert.ok(declared.has(symbol), `${symbol}: referencia CODE sin contrato estructural`);
  }
  assert.deepEqual(Object.keys(manifest.excludedKinds).sort(), ['conceptual', 'external']);
  for (const actor of manifest.excludedKinds.external) assert.match(actor, /^EXT:/, `${actor}: externo sin prefijo EXT`);
  for (const concept of manifest.excludedKinds.conceptual) assert.match(concept, /^CONCEPT:/, `${concept}: concepto sin prefijo CONCEPT`);
});

test('DOC-91 exige documentos, inventario de tipos y límites explícitos', () => {
  assert.deepEqual(manifest.requiredDocuments, [
    'README.md',
    '01-ARQUITECTURA-Y-DIAGRAMAS.md',
    '02-CASOS-DE-USO.md',
    '03-INVENTARIO-TECNICO.md',
    '04-VALIDACION-Y-PENDIENTES.md'
  ]);
  for (const document of manifest.requiredDocuments) {
    assert.ok(fs.existsSync(path.join(docsRoot, document)), `${document}: documento obligatorio ausente`);
  }
  const inventory = fs.readFileSync(path.join(docsRoot, '03-INVENTARIO-TECNICO.md'), 'utf8');
  for (const declaration of Object.values(manifest.dotnetDeclarations)) {
    assert.ok(inventory.includes(`\`${declaration.type}\``), `inventario: tipo ausente ${declaration.type}`);
  }
  const allDocs = manifest.requiredDocuments.map(file => fs.readFileSync(path.join(docsRoot, file), 'utf8')).join('\n');
  assert.match(allDocs, /no (existen|existe).*endpoint/is);
  assert.match(allDocs, /no demuestran? por sí solas?/i);
  assert.match(allDocs, /No aplica/);
});

test('DOC-91 integra validadores Mermaid y Roslyn en CI', () => {
  const ci = fs.readFileSync(path.join(root, '.github', 'workflows', 'opsxj-validation.yml'), 'utf8');
  assert.match(ci, /node --test tests\/doc91-technical-documentation\.test\.cjs/);
  assert.match(ci, /Doc72SourceValidator\.csproj -- \.\/Doc\/Actualizacion\/Login\/Implementacion\/DOC-91\/diagram-contract\.json \./);
});
