const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const acorn = require('../tools/opsxj/node_modules/acorn');

const root = path.resolve(__dirname, '..');
const docsRoot = path.join(root, 'Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-82-interfaz-integracion-asignacion');
const manifest = JSON.parse(fs.readFileSync(path.join(docsRoot, 'diagram-contract.json'), 'utf8'));

function collectFunctions(ast) {
  const result = [];
  function visit(node) {
    if (!node || typeof node !== 'object') return;
    if (node.type === 'FunctionDeclaration') result.push(node);
    for (const value of Object.values(node)) {
      if (Array.isArray(value)) value.forEach(visit);
      else if (value && typeof value === 'object') visit(value);
    }
  }
  visit(ast);
  return result;
}

test('DOC-82 exige cuatro diagramas UML 2 PlantUML con fuentes y referencias', () => {
  assert.equal(manifest.notation, 'PlantUML');
  assert.equal(manifest.umlStandard, 'UML 2');
  assert.deepEqual(manifest.requiredDiagrams.map(({ path: file }) => file), [
    'Diagramas/01-componentes.puml', 'Diagramas/02-interfaz-secuencia.puml',
    'Diagramas/03-estados.puml', 'Diagramas/04-asignacion-actividad.puml'
  ]);
  for (const diagram of manifest.requiredDiagrams) {
    const file = path.join(docsRoot, diagram.path);
    assert.ok(fs.existsSync(file), `${diagram.path}: diagrama obligatorio ausente`);
    const source = fs.readFileSync(file, 'utf8');
    assert.match(source, /^@startuml\s+DOC82_D0[1-4]_/m);
    assert.equal((source.match(/@startuml/g) || []).length, 1);
    assert.equal((source.match(/@enduml/g) || []).length, 1);
    assert.match(source, /@enduml\s*$/);
    assert.match(source, /^' Convención: CODE.*EXT.*CONCEPT/m);
    assert.match(source, /^' Fuentes: /m);
    assert.match(source, /^' Referencias CODE: /m);
    for (const relative of source.match(/^' Fuentes: (.+)$/m)[1].split(' | ')) {
      assert.ok(fs.existsSync(path.join(root, relative)), `${diagram.path}: fuente inexistente ${relative}`);
    }
    for (const symbol of diagram.symbols) assert.ok(source.includes(symbol), `${diagram.path}: símbolo no trazado ${symbol}`);
    if (diagram.kind === 'sequence') assert.equal((source.match(/^\s*alt\b/gm) || []).length, (source.match(/^\s*end\s*$/gm) || []).length);
    if (diagram.kind === 'activity') assert.equal((source.match(/^\s*if\b/gm) || []).length, (source.match(/^\s*endif\s*$/gm) || []).length);
  }
});

test('DOC-82 resuelve funciones JavaScript por AST y firma exacta', () => {
  const cache = new Map();
  for (const [id, expected] of Object.entries(manifest.javascriptSymbols)) {
    if (!cache.has(expected.file)) {
      const source = fs.readFileSync(path.join(root, expected.file), 'utf8');
      cache.set(expected.file, collectFunctions(acorn.parse(source, { ecmaVersion: 2020, sourceType: 'script' })));
    }
    const matches = cache.get(expected.file).filter((node) => node.id?.name === expected.function &&
      node.params.map((parameter) => parameter.name).join(',') === expected.parameters.join(','));
    assert.equal(matches.length, 1, `${id}: función inexistente, ambigua o firma inconsistente`);
  }
});

test('DOC-82 declara toda referencia CODE y limita exclusiones a EXT/CONCEPT', () => {
  const declared = new Set([...Object.keys(manifest.javascriptSymbols), ...Object.keys(manifest.dotnetSymbols)]);
  for (const symbol of manifest.requiredDiagrams.flatMap(({ symbols }) => symbols)) assert.ok(declared.has(symbol), `${symbol}: contrato ausente`);
  assert.deepEqual(Object.keys(manifest.excludedKinds).sort(), ['conceptual', 'external']);
  for (const value of manifest.excludedKinds.external) assert.match(value, /^EXT:/);
  for (const value of manifest.excludedKinds.conceptual) assert.match(value, /^CONCEPT:/);
});

test('DOC-82 documenta flujo, casos, inventario, E2E y límite de validación', () => {
  const required = ['README.md', '01-ARQUITECTURA-Y-FLUJO.md', '02-ESTADOS-Y-CASOS-DE-USO.md', '03-INVENTARIO-Y-TRAZABILIDAD.md', '04-PRUEBAS-SEGURIDAD-Y-OPERACION.md'];
  for (const file of required) assert.ok(fs.existsSync(path.join(docsRoot, file)), file);
  const content = required.map((file) => fs.readFileSync(path.join(docsRoot, file), 'utf8')).join('\n');
  for (const expected of [/Cliente → Controller → Service → Repository → Respuesta/i, /Buttonaceptar_Click/, /no publica `ValidateAssignment`/i, /una intención/i, /no demuestra por sí sola/i]) assert.match(content, expected);
});
