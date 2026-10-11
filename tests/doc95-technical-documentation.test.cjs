'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');

const root = path.resolve(__dirname, '..');
const docsRoot = path.join(root, 'Doc', 'Actualizacion', 'Login', 'Implementacion', 'DOC-95');
const manifest = JSON.parse(fs.readFileSync(path.join(docsRoot, 'diagram-contract.json'), 'utf8'));

let mermaidPromise;
async function mermaidParser() {
  if (!mermaidPromise) mermaidPromise = (async () => {
    const { JSDOM } = await import('../tools/opsxj/node_modules/jsdom/lib/api.js');
    const dom = new JSDOM('<!doctype html><html><body></body></html>');
    global.window = dom.window;
    global.document = dom.window.document;
    Object.defineProperty(global, 'navigator', { value: dom.window.navigator, configurable: true });
    const { default: mermaid } = await import('../tools/opsxj/node_modules/mermaid/dist/mermaid.esm.min.mjs');
    mermaid.initialize({ startOnLoad: false, securityLevel: 'strict' });
    return mermaid;
  })();
  return mermaidPromise;
}

function commentValues(source, label, file) {
  const match = source.match(new RegExp(`^%% ${label}: (.+)$`, 'm'));
  assert.ok(match, `${file}: comentario ${label} ausente`);
  return match[1].split(' | ').map(value => value.trim());
}

test('DOC-95 exige inventario explícito y sintaxis Mermaid válida', async () => {
  assert.deepEqual(manifest.requiredDocuments, [
    'README.md', '01-ARQUITECTURA-Y-DIAGRAMAS.md', '02-CASOS-DE-USO.md',
    '03-INVENTARIO-TECNICO.md', '04-VALIDACION-Y-PENDIENTES.md'
  ]);
  assert.deepEqual(manifest.requiredDiagrams.map(item => item.path), [
    'Diagramas/01-componentes-clases.mmd', 'Diagramas/02-secuencia-inicio-estado.mmd',
    'Diagramas/03-secuencia-verificacion-concurrencia.mmd', 'Diagramas/04-reenvio-cancelacion-errores.mmd'
  ]);
  for (const document of manifest.requiredDocuments) {
    assert.ok(fs.existsSync(path.join(docsRoot, document)), `${document}: documento obligatorio ausente`);
  }
  const mermaid = await mermaidParser();
  for (const diagram of manifest.requiredDiagrams) {
    const absolute = path.join(docsRoot, diagram.path);
    assert.ok(fs.existsSync(absolute), `${diagram.path}: diagrama obligatorio ausente`);
    const source = fs.readFileSync(absolute, 'utf8');
    try { await mermaid.parse(source); } catch (error) { assert.fail(`${diagram.path}: sintaxis Mermaid inválida: ${error.message}`); }
    assert.match(source, /^%% Convención: CODE.*EXT.*CONCEPT/m);
    for (const relative of commentValues(source, 'Fuentes', diagram.path)) {
      assert.ok(fs.existsSync(path.join(root, relative)), `${diagram.path}: fuente inexistente ${relative}`);
    }
    assert.deepEqual(commentValues(source, 'Referencias CODE', diagram.path), diagram.symbols);
    for (const signature of diagram.signatures) assert.ok(source.includes(signature), `${diagram.path}: firma ausente ${signature}`);
  }
});

test('DOC-95 resuelve cada CODE y limita exclusiones a EXT/CONCEPT', () => {
  const declared = new Set([...Object.keys(manifest.dotnetDeclarations), ...Object.keys(manifest.dotnetSymbols)]);
  for (const symbol of manifest.requiredDiagrams.flatMap(item => item.symbols)) {
    assert.ok(declared.has(symbol), `${symbol}: referencia sin contrato estructural`);
  }
  assert.deepEqual(Object.keys(manifest.excludedKinds).sort(), ['conceptual', 'external']);
  for (const value of manifest.excludedKinds.external) assert.match(value, /^EXT:/);
  for (const value of manifest.excludedKinds.conceptual) assert.match(value, /^CONCEPT:/);
});

test('DOC-95 documenta endpoints, casos, estados físicos y asuntos abiertos', () => {
  const inventory = fs.readFileSync(path.join(docsRoot, '03-INVENTARIO-TECNICO.md'), 'utf8');
  for (const operation of ['ObtenerEstado', 'Verificar', 'Reenviar', 'Cancelar']) assert.match(inventory, new RegExp(operation));
  for (const dto of ['SegundoFactorEstadoDto', 'SegundoFactorResultadoDto']) assert.match(inventory, new RegExp(dto));
  const cases = fs.readFileSync(path.join(docsRoot, '02-CASOS-DE-USO.md'), 'utf8');
  for (let number = 1; number <= 6; number += 1) assert.match(cases, new RegExp(`## UC-${String(number).padStart(2, '0')}\\b`));
  const all = manifest.requiredDocuments.map(file => fs.readFileSync(path.join(docsRoot, file), 'utf8')).join('\n');
  assert.match(all, /no (?:se agregan|agrega).*tablas.*columnas/is);
  assert.match(all, /Consumed=1/);
  assert.match(all, /AuthPayloadJson.*nulo/i);
  assert.match(all, /Session.*InProc/i);
  assert.match(all, /no demuestra por sí sola/i);
  assert.match(all, /DOC95_E2E_RESULT=PASSED/i);
  assert.match(all, /no se registraron cuenta, contraseña, OTP, correo completo, cookie, challenge, host SMTP ni cadena de conexión/i);
  assert.match(all, /interfaz visual.*fuera/i);
});

test('DOC-95 conserva trazabilidad de composición y configuración reutilizada', () => {
  const architecture = fs.readFileSync(path.join(docsRoot, '01-ARQUITECTURA-Y-DIAGRAMAS.md'), 'utf8');
  assert.match(architecture, /ESTADO_ENVIO=1/);
  assert.match(architecture, /AppSettingsSecondFactorKeyProvider/);
  assert.match(architecture, /no están versionadas en `Web\.config`/);
  const inventory = fs.readFileSync(path.join(docsRoot, '03-INVENTARIO-TECNICO.md'), 'utf8');
  for (const declaration of Object.values(manifest.dotnetDeclarations)) {
    assert.ok(inventory.includes(declaration.type) || ['ISecondFactorChallengeRepository', 'ISecondFactorEmailSender', 'IPendingSecondFactorContextStore', 'ILegacyLoginFinalizer'].includes(declaration.type), `inventario: falta ${declaration.type}`);
  }
});

test('DOC-95 está integrado en CI con Mermaid, Roslyn, comportamiento y política E2E', () => {
  const ci = fs.readFileSync(path.join(root, '.github', 'workflows', 'opsxj-validation.yml'), 'utf8');
  assert.match(ci, /node --test tests\/doc95-technical-documentation\.test\.cjs/);
  assert.match(ci, /Doc72SourceValidator\.csproj -- \.\/Doc\/Actualizacion\/Login\/Implementacion\/DOC-95\/diagram-contract\.json \./);
  assert.match(ci, /node --test tests\/login-second-factor-authentication\.test\.cjs tests\/login-second-factor-asmx\.test\.cjs/);
  assert.match(ci, /npm\.cmd --prefix tools\/e2e run test:doc95:policy/);
});
