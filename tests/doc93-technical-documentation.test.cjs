const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const docsRoot = path.join(root, 'Doc', 'Actualizacion', 'Login', 'Implementacion', 'DOC-93');
const manifest = JSON.parse(fs.readFileSync(path.join(docsRoot, 'diagram-contract.json'), 'utf8'));

let mermaidPromise;
async function mermaidParser() {
  if (!mermaidPromise) {
    mermaidPromise = (async () => {
      const { JSDOM } = await import('../tools/opsxj/node_modules/jsdom/lib/api.js');
      const dom = new JSDOM('<!doctype html><html><body></body></html>');
      global.window = dom.window; global.document = dom.window.document; global.navigator = dom.window.navigator;
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

test('DOC-93 exige documentos y diagramas Mermaid válidos', async () => {
  assert.equal(manifest.notation, 'Mermaid');
  assert.deepEqual(manifest.requiredDocuments, ['README.md', '01-ARQUITECTURA-Y-DIAGRAMAS.md', '02-CASOS-DE-USO.md', '03-INVENTARIO-TECNICO.md', '04-VALIDACION-Y-PENDIENTES.md']);
  assert.deepEqual(manifest.requiredDiagrams.map(item => item.path), [
    'Diagramas/01-componentes-clases.mmd',
    'Diagramas/02-secuencia-resolver-configuracion.mmd',
    'Diagramas/03-secuencia-enviar-otp.mmd',
    'Diagramas/04-secuencia-fachada.mmd'
  ]);
  for (const document of manifest.requiredDocuments) assert.ok(fs.existsSync(path.join(docsRoot, document)), `${document}: documento obligatorio ausente`);

  const mermaid = await mermaidParser();
  for (const diagram of manifest.requiredDiagrams) {
    const absolute = path.join(docsRoot, diagram.path);
    assert.ok(fs.existsSync(absolute), `${diagram.path}: diagrama obligatorio ausente`);
    const source = fs.readFileSync(absolute, 'utf8');
    try { await mermaid.parse(source); } catch (error) { assert.fail(`${diagram.path}: sintaxis Mermaid inválida: ${error.message}`); }
    assert.match(source, /^%% Convención: CODE.*EXT.*CONCEPT/m, `${diagram.path}: convención ausente`);
    for (const relative of commentValues(source, 'Fuentes', diagram.path)) assert.ok(fs.existsSync(path.join(root, relative)), `${diagram.path}: fuente inexistente ${relative}`);
    assert.deepEqual(commentValues(source, 'Referencias CODE', diagram.path), diagram.symbols, `${diagram.path}: referencias distintas del manifiesto`);
    for (const signature of diagram.signatures) assert.ok(source.includes(signature), `${diagram.path}: firma ausente: ${signature}`);
  }
});

test('DOC-93 resuelve CODE y limita exclusiones a EXT/CONCEPT', () => {
  const declared = new Set([...Object.keys(manifest.dotnetDeclarations), ...Object.keys(manifest.dotnetSymbols)]);
  for (const symbol of manifest.requiredDiagrams.flatMap(item => item.symbols)) assert.ok(declared.has(symbol), `${symbol}: referencia sin contrato estructural`);
  assert.deepEqual(Object.keys(manifest.excludedKinds).sort(), ['conceptual', 'external']);
  for (const actor of manifest.excludedKinds.external) assert.match(actor, /^EXT:/);
  for (const concept of manifest.excludedKinds.conceptual) assert.match(concept, /^CONCEPT:/);
  assert.equal(Object.keys(manifest.dotnetImplements).length, 5);
});

test('DOC-93 inventaría firmas, casos, columnas y límites sin inventar capas', () => {
  const inventory = fs.readFileSync(path.join(docsRoot, '03-INVENTARIO-TECNICO.md'), 'utf8');
  for (const declaration of Object.values(manifest.dotnetDeclarations)) assert.ok(inventory.includes(`\`${declaration.type}\``) || inventory.includes(declaration.type), `inventario: falta ${declaration.type}`);
  for (const symbol of Object.values(manifest.dotnetSymbols)) assert.ok(inventory.includes(symbol.method), `inventario: falta ${symbol.type}.${symbol.method}`);
  for (const column of ['SERV_SMTP','PUERTO_SERV_SMTP','USUARIO_SMTP','PASW_SMTP','DOMINIO_SMTP','SMTP_TIEMPO','ESTADO_SSL','ESTADO_ENVIO','ESTADO_BODY','ESTADO_CREDENCIAL']) assert.ok(inventory.includes(`\`${column}\``), `columna ausente: ${column}`);
  const cases = fs.readFileSync(path.join(docsRoot, '02-CASOS-DE-USO.md'), 'utf8');
  for (let number = 1; number <= 5; number += 1) assert.match(cases, new RegExp(`## UC-${String(number).padStart(2, '0')}\\b`));
  const docs = manifest.requiredDocuments.map(file => fs.readFileSync(path.join(docsRoot, file), 'utf8')).join('\n');
  assert.match(docs, /no (?:hay|existe).*Controller.*endpoint/is);
  assert.match(docs, /No aplica/);
  assert.match(docs, /no demuestra por sí sola/i);
  assert.match(docs, /correspondencia estructural/i);
  assert.match(docs, /SMTP real.*no (?:ejecutado|se ejecut)/i);
});

test('DOC-93 integra Mermaid y Roslyn en CI', () => {
  const ci = fs.readFileSync(path.join(root, '.github', 'workflows', 'opsxj-validation.yml'), 'utf8');
  assert.match(ci, /node --test tests\/doc93-technical-documentation\.test\.cjs/);
  assert.match(ci, /Doc72SourceValidator\.csproj -- \.\/Doc\/Actualizacion\/Login\/Implementacion\/DOC-93\/diagram-contract\.json \./);
});
