const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const acorn = require("../tools/opsxj/node_modules/acorn");

const root = path.resolve(__dirname, "..");
const docsRoot = path.join(root, "Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-72-nucleo-registro-adaptadores");
const manifest = JSON.parse(fs.readFileSync(path.join(docsRoot, "diagram-contract.json"), "utf8"));

function diagramBody(content, file) {
    const match = content.match(/```mermaid\s*([\s\S]*?)```/);
    assert.ok(match, `${file}: bloque Mermaid ausente`);
    return match[1].trim();
}

function validateMermaid(body, kind, file) {
    const lines = body.split(/\r?\n/).map(line => line.trim()).filter(Boolean);
    const header = kind === "sequence" ? "sequenceDiagram" : "flowchart TD";
    assert.equal(lines[0], header, `${file}: encabezado Mermaid esperado ${header}`);
    let blocks = 0;
    for (const [index, line] of lines.slice(1).entries()) {
        if (/^(alt|opt|loop|par)\b/.test(line)) blocks += 1;
        if (/^end$/.test(line)) blocks -= 1;
        assert.ok(blocks >= 0, `${file}:${index + 2}: end sin bloque`);
        if (kind === "sequence") {
            assert.match(line, /^(actor|participant|Note\s+(left|right|over)\s+of|alt\b|else\b|opt\b|loop\b|par\b|and\b|end$|[A-Za-z0-9_]+\s*(--?>?>|--x|-x)\s*[A-Za-z0-9_]+\s*:)/, `${file}:${index + 2}: sintaxis sequence no reconocida: ${line}`);
        } else {
            assert.match(line, /^(subgraph\b|end$|[A-Za-z0-9_]+(\[[^\]]+\]|\{[^}]+\})?\s*(-->|-.->)(\|[^|]+\|)?\s*[A-Za-z0-9_]+|[A-Za-z0-9_]+\[[^\]]+\])/, `${file}:${index + 2}: sintaxis flowchart no reconocida: ${line}`);
        }
    }
    assert.equal(blocks, 0, `${file}: bloque Mermaid sin cerrar`);
}

function collectFunctions(ast) {
    const functions = [];
    function visit(node) {
        if (!node || typeof node !== "object") return;
        if (node.type === "FunctionDeclaration") functions.push(node);
        for (const value of Object.values(node)) {
            if (Array.isArray(value)) value.forEach(visit);
            else if (value && typeof value === "object") visit(value);
        }
    }
    visit(ast);
    return functions;
}

test("inventario explícito exige todos los diagramas y sintaxis Mermaid válida", () => {
    assert.deepEqual(manifest.requiredDiagrams.map(item => item.path).sort(), [
        "Diagramas/01-Componentes.md", "Diagramas/02-AperturaConsulta.md",
        "Diagramas/03-EjecucionUnica.md", "Diagramas/04-ValidacionesErrores.md"
    ]);
    for (const diagram of manifest.requiredDiagrams) {
        const file = path.join(docsRoot, diagram.path);
        assert.ok(fs.existsSync(file), `${diagram.path}: diagrama requerido ausente`);
        const content = fs.readFileSync(file, "utf8");
        const body = diagramBody(content, diagram.path);
        validateMermaid(body, diagram.kind, diagram.path);
        assert.match(content, /## Convención/, `${diagram.path}: convención ausente`);
        assert.match(content, /## Fuentes/, `${diagram.path}: fuentes ausentes`);
        for (const symbol of diagram.symbols) assert.ok(content.includes(symbol), `${diagram.path}: símbolo ${symbol} no trazado`);
    }
});

test("referencias JavaScript resuelven por AST con parámetros exactos", () => {
    const cache = new Map();
    for (const [id, expected] of Object.entries(manifest.javascriptSymbols)) {
        if (!cache.has(expected.file)) {
            const source = fs.readFileSync(path.join(root, expected.file), "utf8");
            cache.set(expected.file, collectFunctions(acorn.parse(source, { ecmaVersion: 2020, sourceType: "script" })));
        }
        const candidates = cache.get(expected.file).filter(node => node.id && node.id.name === expected.function);
        const matches = candidates.filter(node => node.params.map(param => param.name).join(",") === expected.parameters.join(","));
        assert.equal(matches.length, 1, `${id}: función inexistente, ambigua o parámetros inconsistentes`);
    }
});

test("cada referencia de diagrama está declarada y solo se excluyen actores/conceptos explícitos", () => {
    const declared = new Set([...Object.keys(manifest.javascriptSymbols), ...Object.keys(manifest.dotnetSymbols)]);
    for (const id of manifest.requiredDiagrams.flatMap(item => item.symbols)) {
        assert.ok(declared.has(id), `${id}: referencia de diagrama sin contrato estructural`);
    }
    assert.deepEqual(Object.keys(manifest.excludedKinds).sort(), ["conceptual", "external"]);
});

test("documentación declara alcance, casos de uso, inventario y limitación de la prueba", () => {
    for (const required of ["08-InventarioTecnico.md", "09-CasosDeUso.md", "10-AlcanceRevision.md"]) {
        assert.ok(fs.existsSync(path.join(docsRoot, required)), `${required}: documento requerido ausente`);
    }
    const evidence = fs.readFileSync(path.join(docsRoot, "06-PruebasEvidencia.md"), "utf8");
    assert.match(evidence, /no demuestra por sí sola/i);
    assert.match(evidence, /correspondencia estructural/i);
});
