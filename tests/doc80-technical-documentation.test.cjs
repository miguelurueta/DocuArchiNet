const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const root = path.resolve(__dirname, "..");
const docsRoot = path.join(root, "Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-80-capacidad-consulta-preview");
const manifest = JSON.parse(fs.readFileSync(path.join(docsRoot, "diagram-contract.json"), "utf8"));

test("DOC-80 conserva inventario explícito y sintaxis UML 2 en PlantUML", () => {
  assert.equal(manifest.notation, "PlantUML");
  assert.equal(manifest.umlStandard, "UML 2");
  assert.deepEqual(manifest.requiredDiagrams.map(x => x.path), ["Diagramas/01-componentes.puml", "Diagramas/02-consulta-anexos-secuencia.puml", "Diagramas/03-preview-descriptor-secuencia.puml", "Diagramas/04-streaming-actividad.puml"]);
  for (const diagram of manifest.requiredDiagrams) {
    const file = path.join(docsRoot, diagram.path);
    assert.ok(fs.existsSync(file), `${diagram.path}: diagrama obligatorio ausente`);
    const text = fs.readFileSync(file, "utf8");
    assert.match(text, /^@startuml\s+DOC80_D0[1-4]_/m, `${diagram.path}: inicio PlantUML inválido`);
    assert.equal((text.match(/@startuml/g) || []).length, 1, `${diagram.path}: @startuml duplicado`);
    assert.equal((text.match(/@enduml/g) || []).length, 1, `${diagram.path}: @enduml duplicado`);
    assert.match(text, /@enduml\s*$/, `${diagram.path}: cierre PlantUML inválido`);
    assert.match(text, /^' Convención: CODE.*EXT.*CONCEPT/m, `${diagram.path}: convención ausente`);
    assert.match(text, /^' Fuentes: /m, `${diagram.path}: fuentes ausentes`);
    assert.match(text, /^' Referencias CODE: /m, `${diagram.path}: referencias ausentes`);
    if (diagram.kind !== "activity") assert.match(text, /--?>|<\|\.\./, `${diagram.path}: relación UML ausente`);
    if (diagram.kind === "component") assert.match(text, /\b(component|interface|database|cloud)\b/, `${diagram.path}: componentes UML ausentes`);
    if (diagram.kind === "sequence") {
      assert.match(text, /\bparticipant\b/, `${diagram.path}: participantes UML ausentes`);
      assert.equal((text.match(/^\s*alt\b/gm) || []).length, (text.match(/^\s*end\s*$/gm) || []).length, `${diagram.path}: alt/end desbalanceados`);
    }
    if (diagram.kind === "activity") {
      assert.match(text, /^start$/m, `${diagram.path}: inicio de actividad ausente`);
      assert.match(text, /^\s*stop$/m, `${diagram.path}: fin de actividad ausente`);
      assert.equal((text.match(/^\s*if\b/gm) || []).length, (text.match(/^\s*endif\s*$/gm) || []).length, `${diagram.path}: if/endif desbalanceados`);
    }
    for (const source of text.match(/^' Fuentes: (.+)$/m)[1].split(" | ")) assert.ok(fs.existsSync(path.join(root, source)), `${diagram.path}: fuente inexistente ${source}`);    for (const symbol of diagram.symbols) assert.ok(text.includes(symbol), `${diagram.path}: símbolo no trazado ${symbol}`);
  }
});

test("DOC-80 declara referencias CODE y excluye solo EXT/CONCEPT", () => {
  const declared = new Set(Object.keys(manifest.dotnetSymbols));
  for (const symbol of manifest.requiredDiagrams.flatMap(x => x.symbols)) assert.ok(declared.has(symbol), `${symbol}: referencia sin contrato estructural`);
  assert.deepEqual(Object.keys(manifest.excludedKinds).sort(), ["conceptual", "external"]);
  for (const values of Object.values(manifest.excludedKinds)) for (const value of values) assert.match(value, /^(EXT|CONCEPT):/);
});

test("DOC-80 documenta alcance, casos, inventario, HTTP y límite de prueba", () => {
  for (const file of ["ALCANCE-Y-TRAZABILIDAD.md", "CASOS-DE-USO.md", "INVENTARIO-TECNICO.md", "FLUJO-TECNICO-DETALLADO.md"]) assert.ok(fs.existsSync(path.join(docsRoot, file)), `${file}: documento requerido ausente`);
  assert.match(fs.readFileSync(path.join(docsRoot, "ALCANCE-Y-TRAZABILIDAD.md"), "utf8"), /no se declara cobertura completa/i);
  assert.match(fs.readFileSync(path.join(docsRoot, "INVENTARIO-TECNICO.md"), "utf8"), /HTTP 200, 404, 405 o 503/);
  assert.match(fs.readFileSync(path.join(docsRoot, "README.md"), "utf8"), /no demuestra por sí sola/i);
});

test("CI ejecuta validación documental DOC-80 por Node y Roslyn", () => {
  const ci = fs.readFileSync(path.join(root, ".github/workflows/opsxj-validation.yml"), "utf8");
  assert.match(ci, /node --test tests\/doc80-technical-documentation\.test\.cjs/);
  assert.match(ci, /DOC-80 \.NET signatures with Roslyn/);
});
