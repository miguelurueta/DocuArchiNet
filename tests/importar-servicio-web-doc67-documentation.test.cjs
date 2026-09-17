const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const docsRoot = path.join(root, "Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-67-creacion-vinculacion-expediente");
const diagramsRoot = path.join(docsRoot, "Diagramas");
const manifestPath = path.join(diagramsRoot, "manifest.json");
const codeRoots = ["DTOs", "Modelo", "Services", "Infrastructure", "webservice"];

function filesBelow(directory, extension, result = []) {
  for (const entry of fs.readdirSync(directory, { withFileTypes: true })) {
    const full = path.join(directory, entry.name);
    if (entry.isDirectory()) filesBelow(full, extension, result);
    else if (entry.name.toLowerCase().endsWith(extension)) result.push(full);
  }
  return result;
}

function logicalLines(source) {
  const lines = [];
  let pending = "";
  for (const raw of source.replace(/\r/g, "").split("\n")) {
    const withoutComment = raw.replace(/'.*$/, "").trim();
    if (!withoutComment && !pending) continue;
    const explicitContinuation = /_\s*$/.test(withoutComment);
    pending += `${pending ? " " : ""}${withoutComment.replace(/_\s*$/, "").trim()}`;
    const opening = (pending.match(/\(/g) || []).length;
    const closing = (pending.match(/\)/g) || []).length;
    const continued = explicitContinuation || opening > closing;
    if (!continued && pending) {
      lines.push(pending.replace(/\s+/g, " "));
      pending = "";
    }
  }
  if (pending) lines.push(pending);
  return lines;
}

function splitParameters(text) {
  if (!text.trim()) return [];
  const parts = [];
  let depth = 0;
  let current = "";
  for (const char of text) {
    if (char === "(") depth += 1;
    if (char === ")") depth -= 1;
    if (char === "," && depth === 0) {
      parts.push(current);
      current = "";
    } else current += char;
  }
  parts.push(current);
  return parts.map((parameter) => {
    const clean = parameter
      .replace(/\b(ByVal|ByRef|Optional|ParamArray)\b/gi, "")
      .replace(/\s*=.*$/, "")
      .trim();
    const match = clean.match(/\bAs\s+(.+)$/i);
    return match ? match[1].replace(/\s+/g, "").trim() : "Object";
  });
}

function parseVbFile(file) {
  const symbols = [];
  const stack = [];
  for (const line of logicalLines(fs.readFileSync(file, "utf8"))) {
    const type = line.match(/^(?:Public|Friend|Private|Protected)?\s*(?:Partial\s+|NotInheritable\s+|MustInherit\s+)*(Class|Interface|Structure)\s+(\w+)/i);
    if (type) {
      stack.push({ kind: type[1].toLowerCase(), name: type[2] });
      symbols.push({ kind: "type", name: type[2], file });
      continue;
    }
    if (/^End\s+(Class|Interface|Structure)\b/i.test(line)) {
      stack.pop();
      continue;
    }
    if (!stack.length) continue;
    const method = line.match(/^(?:Public|Friend|Private|Protected)?\s*(?:Shared\s+|Overloads\s+|Overrides\s+|Overridable\s+|MustOverride\s+|NotOverridable\s+|Async\s+)*(Function|Sub)\s+(\w+)(?:\s*\(Of\s+[^)]*\))?\s*\((.*)\)\s*(?:As\s+([^\s]+(?:\s*\(Of\s+[^)]*\))?))?/i);
    if (method) {
      symbols.push({
        kind: "method",
        owner: stack[stack.length - 1].name,
        name: method[2],
        parameters: splitParameters(method[3]),
        returns: method[1].toLowerCase() === "sub" ? "Void" : (method[4] || "Object").replace(/\s+/g, ""),
        file,
      });
    }
  }
  return symbols;
}

const vbFiles = codeRoots.flatMap((relative) => filesBelow(path.join(root, relative), ".vb"));
const symbols = vbFiles.flatMap(parseVbFile);

function parseReference(reference) {
  const method = reference.match(/^(\w+)\.(\w+)\((.*)\):(.*)$/);
  if (!method) return { kind: "type", owner: reference };
  return {
    kind: "method",
    owner: method[1],
    name: method[2],
    parameters: method[3].trim() ? method[3].split(",").map((value) => value.replace(/\s+/g, "")) : [],
    returns: method[4].replace(/\s+/g, ""),
  };
}

function referencesIn(markdown) {
  const line = markdown.split(/\r?\n/).find((value) => /^Referencias CODE:/i.test(value));
  assert.ok(line, "falta la línea Referencias CODE");
  return [...line.matchAll(/`([^`]+)`/g)].map((match) => parseReference(match[1]));
}

function assertMermaidSubset(file, markdown) {
  const blocks = [...markdown.matchAll(/```mermaid\s*\r?\n([\s\S]*?)```/g)];
  assert.equal(blocks.length, 1, `${file}: debe contener exactamente un bloque Mermaid`);
  const lines = blocks[0][1].split(/\r?\n/).map((line) => line.trim()).filter(Boolean);
  assert.match(lines[0] || "", /^(flowchart\s+(TD|LR|RL|BT)|sequenceDiagram|classDiagram|stateDiagram-v2)$/,
    `${file}: directiva Mermaid no soportada o ausente`);
  if (lines[0] === "sequenceDiagram") {
    let depth = 0;
    for (const line of lines.slice(1)) {
      if (/^(alt|opt|loop|par|critical|rect)\b/.test(line)) depth += 1;
      if (/^end\b/.test(line)) depth -= 1;
      assert.ok(depth >= 0, `${file}: cierre Mermaid sin bloque abierto`);
    }
    assert.equal(depth, 0, `${file}: bloque Mermaid sin cierre`);
  } else {
    assert.ok(lines.length > 1, `${file}: diagrama Mermaid vacío`);
  }
}

test("DOC-67 declara y conserva el inventario obligatorio de diagramas", () => {
  const manifest = JSON.parse(fs.readFileSync(manifestPath, "utf8"));
  assert.deepEqual(Object.keys(manifest.convention).sort(), ["CODE", "CONCEPT", "EXT"]);
  assert.deepEqual(manifest.required, [
    "01-frontera-endpoints.md",
    "02-expediente-antes-storage.md",
    "03-saga-documentos-relacionados.md",
    "04-retry-recovery.md",
    "05-componentes-persistencia.md",
  ]);
  for (const file of manifest.required) assert.ok(fs.existsSync(path.join(diagramsRoot, file)), `${file}: diagrama obligatorio ausente`);
});

test("DOC-67 valida Mermaid, fuentes y convención CODE/EXT/CONCEPT", () => {
  const manifest = JSON.parse(fs.readFileSync(manifestPath, "utf8"));
  for (const file of manifest.required) {
    const markdown = fs.readFileSync(path.join(diagramsRoot, file), "utf8");
    assertMermaidSubset(file, markdown);
    const sourceLine = markdown.split(/\r?\n/).find((line) => /^Fuentes:/i.test(line));
    assert.ok(sourceLine, `${file}: falta Fuentes`);
    for (const match of sourceLine.matchAll(/`([^`]+)`/g)) {
      assert.ok(fs.existsSync(path.join(root, match[1])), `${file}: fuente inexistente ${match[1]}`);
    }
    const references = referencesIn(markdown);
    const referencedTypes = new Set(references.map((reference) => reference.owner));
    for (const match of markdown.matchAll(/\bCODE:(\w+)/g)) {
      assert.ok(referencedTypes.has(match[1]), `${file}: CODE:${match[1]} no está declarado en Referencias CODE`);
    }
  }
});

test("DOC-67 resuelve estructuralmente tipos, propietarios, sobrecargas, parámetros y retornos VB", () => {
  const manifest = JSON.parse(fs.readFileSync(manifestPath, "utf8"));
  for (const file of manifest.required) {
    const markdown = fs.readFileSync(path.join(diagramsRoot, file), "utf8");
    for (const reference of referencesIn(markdown)) {
      const typeExists = symbols.some((symbol) => symbol.kind === "type" && symbol.name === reference.owner);
      assert.ok(typeExists, `${file}: tipo inexistente ${reference.owner}`);
      if (reference.kind === "type") continue;
      const overloads = symbols.filter((symbol) => symbol.kind === "method" && symbol.owner === reference.owner && symbol.name === reference.name);
      assert.ok(overloads.length, `${file}: ${reference.owner}.${reference.name} no pertenece al tipo indicado`);
      const exact = overloads.find((candidate) =>
        candidate.returns === reference.returns &&
        candidate.parameters.length === reference.parameters.length &&
        candidate.parameters.every((parameter, index) => parameter === reference.parameters[index]));
      assert.ok(exact, `${file}: firma inconsistente ${reference.owner}.${reference.name}(${reference.parameters.join(",")}):${reference.returns}; ` +
        `implementadas: ${overloads.map((candidate) => `(${candidate.parameters.join(",")}):${candidate.returns}`).join(" | ")}`);
    }
  }
});

test("el readiness DOC-67 incorpora automáticamente la prueba documental", () => {
  const runner = fs.readFileSync(path.join(root, "Scripts/run-doc67-readiness.cjs"), "utf8");
  assert.match(runner, /tests\/importar-servicio-web-\*\.test\.cjs/);
  assert.match(path.basename(__filename), /^importar-servicio-web-.*\.test\.cjs$/);
});
