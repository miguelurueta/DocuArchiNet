const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const root = path.resolve(__dirname, '..');
const docsRoot = path.join(root, 'Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-81-preparacion-persistencia-reconciliacion');
const manifest = JSON.parse(fs.readFileSync(path.join(docsRoot, 'diagram-contract.json'), 'utf8'));

test('DOC-81 conserva inventario explícito y sintaxis UML 2 en PlantUML', () => {
  assert.equal(manifest.notation, 'PlantUML');
  assert.equal(manifest.umlStandard, 'UML 2');
  assert.deepEqual(manifest.requiredDiagrams.map(x => x.path), [
    'Diagramas/01-componentes.puml', 'Diagramas/02-ejecucion-secuencia.puml',
    'Diagramas/03-estados.puml', 'Diagramas/04-reconciliacion-actividad.puml'
  ]);
  for (const diagram of manifest.requiredDiagrams) {
    const file = path.join(docsRoot, diagram.path);
    assert.ok(fs.existsSync(file), `${diagram.path}: diagrama obligatorio ausente`);
    const text = fs.readFileSync(file, 'utf8');
    assert.match(text, /^@startuml\s+DOC81_D0[1-4]_/m);
    assert.equal((text.match(/@startuml/g) || []).length, 1);
    assert.equal((text.match(/@enduml/g) || []).length, 1);
    assert.match(text, /@enduml\s*$/);
    assert.match(text, /^' Convención: CODE.*EXT.*CONCEPT/m);
    assert.match(text, /^' Fuentes: /m);
    assert.match(text, /^' Referencias CODE: /m);
    if (diagram.kind === 'component') assert.match(text, /\b(component|database|cloud)\b/);
    if (diagram.kind === 'sequence') {
      assert.match(text, /\bparticipant\b/);
      assert.equal((text.match(/^\s*alt\b/gm) || []).length, (text.match(/^\s*end\s*$/gm) || []).length);
    }
    if (diagram.kind === 'activity') {
      assert.match(text, /^start$/m); assert.match(text, /^stop$/m);
      assert.equal((text.match(/^\s*if\b/gm) || []).length, (text.match(/^\s*endif\s*$/gm) || []).length);
    }
    for (const source of text.match(/^' Fuentes: (.+)$/m)[1].split(' | ')) assert.ok(fs.existsSync(path.join(root, source)), `${diagram.path}: fuente inexistente ${source}`);
    for (const symbol of diagram.symbols) assert.ok(text.includes(symbol), `${diagram.path}: símbolo no trazado ${symbol}`);
  }
});

test('DOC-81 declara referencias CODE y excluye solo EXT/CONCEPT', () => {
  const declared = new Set(Object.keys(manifest.dotnetSymbols));
  for (const symbol of manifest.requiredDiagrams.flatMap(x => x.symbols)) assert.ok(declared.has(symbol), `${symbol}: sin contrato`);
  assert.deepEqual(Object.keys(manifest.excludedKinds).sort(), ['conceptual', 'external']);
  for (const values of Object.values(manifest.excludedKinds)) for (const value of values) assert.match(value, /^(EXT|CONCEPT):/);
});

test('DOC-81 documenta flujo, estados, idempotencia, mapping, persistencia, pruebas y rollback', () => {
  for (const file of ['README.md','01-ARQUITECTURA-Y-FLUJO.md','02-ESTADOS-IDEMPOTENCIA-RECONCILIACION.md','03-INVENTARIO-Y-TRAZABILIDAD.md','04-PRUEBAS-SEGURIDAD-ROLLBACK.md']) assert.ok(fs.existsSync(path.join(docsRoot,file)), file);
  const docs = fs.readdirSync(docsRoot).filter(x => x.endsWith('.md')).map(x => fs.readFileSync(path.join(docsRoot,x),'utf8')).join('\n');
  for (const term of [/idempotencia/i,/mapping legacy/i,/persistencia/i,/reconciliación/i,/rollback/i,/no asigna/i]) assert.match(docs, term);
  assert.match(fs.readFileSync(path.join(docsRoot,'README.md'),'utf8'), /no demuestra por sí sola/i);
});

test('CI ejecuta validación documental DOC-81 por Node y Roslyn', () => {
  const ci=fs.readFileSync(path.join(root,'.github/workflows/opsxj-validation.yml'),'utf8');
  assert.match(ci,/node --test tests\/doc81-technical-documentation\.test\.cjs/);
  assert.match(ci,/DOC-81 \.NET signatures with Roslyn/);
});