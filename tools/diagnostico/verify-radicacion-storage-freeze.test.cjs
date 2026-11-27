"use strict";

const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const { collectFunctions, verifyBaseline } = require("./verify-radicacion-storage-freeze.cjs");
const root = path.resolve(__dirname, "../..");
const baseline = JSON.parse(fs.readFileSync(path.join(root, "Doc/Diagnostico/radicacion-interna/almacenamiento-intocable-baseline.json"), "utf8"));
const read = (file) => fs.readFileSync(path.join(root, file), "utf8");

function mutateFunction(file, name) {
  return (requested) => {
    const source = read(requested);
    if (requested !== file) return source;
    const lines = source.replace(/\r\n/g, "\n").split("\n");
    const [method] = collectFunctions(source, name);
    lines[method.startLine - 1] += " ' cambio simulado solo en memoria";
    return lines.join("\n");
  };
}

test("la línea base actual conserva almacenamiento y metadatos protegidos", () => {
  const result = verifyBaseline(root, baseline);
  assert.equal(result.ok, true, result.failures.join("\n"));
  assert.equal(result.functions, 24);
});

test("un cambio en el almacenamiento central se rechaza sin editar el archivo", () => {
  const result = verifyBaseline(root, baseline, mutateFunction("workflow/ClassAlmacenamiento.vb", "Almacenamiento"));
  assert.equal(result.ok, false);
  assert.ok(result.failures.some((failure) => failure.includes("archivo protegido modificado")));
  assert.ok(result.failures.some((failure) => failure.includes("Almacenamiento: función")));
});

test("un cambio en el mapeo de metadatos también se rechaza", () => {
  const result = verifyBaseline(root, baseline, mutateFunction("Gestion/ClassGaProducionDocumental.vb", "Retorna_parametros_almacenamiento_documento_radicado_interno"));
  assert.equal(result.ok, false);
  assert.ok(result.failures.some((failure) => failure.includes("Retorna_parametros_almacenamiento_documento_radicado_interno")));
});

test("la protección de funciones permite cambios externos al motor en el generador de UI", () => {
  const result = verifyBaseline(root, baseline, mutateFunction("Gestion/ClassGaProducionDocumental.vb", "Genera_Interface_Radicacion_Entrante"));
  assert.equal(result.ok, true, result.failures.join("\n"));
});

test("CRLF y LF no producen una modificación ficticia", () => {
  const result = verifyBaseline(root, baseline, (file) => read(file).replace(/\r\n/g, "\n").replace(/\n/g, "\r\n"));
  assert.equal(result.ok, true, result.failures.join("\n"));
});

test("alterar la huella esperada no permite rebaselinar frente al commit de origen", () => {
  const altered = JSON.parse(JSON.stringify(baseline));
  altered.targets[1].methods[0].variants[0].sha256 = "0".repeat(64);
  const result = verifyBaseline(root, altered);
  assert.equal(result.ok, false);
  assert.ok(result.failures.some((failure) => failure.includes("baseline de función inválida")));
});
