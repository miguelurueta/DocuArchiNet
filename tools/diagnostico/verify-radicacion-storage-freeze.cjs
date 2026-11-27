"use strict";

const fs = require("node:fs");
const path = require("node:path");
const crypto = require("node:crypto");
const { execFileSync } = require("node:child_process");

const normalize = (value) => String(value).replace(/\r\n/g, "\n");
const sha256 = (value) => crypto.createHash("sha256").update(normalize(value)).digest("hex");

function collectFunctions(source, name) {
  const lines = normalize(source).split("\n");
  const ranges = [];
  const declaration = /^\s*(?:(?:Public|Private|Protected|Friend|Shared|Overloads|Overrides|Overridable|NotOverridable|MustOverride|Shadows|Async|Iterator)\s+)*Function\s+(\w+)\s*\(/i;
  for (let i = 0; i < lines.length; i += 1) {
    const match = declaration.exec(lines[i]);
    if (!match || match[1].toLowerCase() !== name.toLowerCase()) continue;
    let end = i + 1;
    while (end < lines.length && !/^\s*End Function\s*(?:'.*)?$/i.test(lines[end])) end += 1;
    if (end === lines.length) throw new Error(`No se encontró End Function para ${name}.`);
    ranges.push({ startLine: i + 1, endLine: end + 1, sha256: sha256(lines.slice(i, end + 1).join("\n") + "\n") });
    i = end;
  }
  return ranges;
}

function verifyBaseline(root, baseline, readCurrent = (file) => fs.readFileSync(path.join(root, file), "utf8")) {
  const failures = [];
  let functions = 0;
  if (baseline.schemaVersion !== 1 || !/^[a-f0-9]{40}$/.test(baseline.baseCommit || "")) {
    throw new Error("Baseline de almacenamiento inválida.");
  }
  for (const target of baseline.targets) {
    const historical = execFileSync("git", ["show", `${baseline.baseCommit}:${target.file}`], {
      cwd: root, encoding: "utf8", maxBuffer: 16 * 1024 * 1024,
    });
    const current = readCurrent(target.file);
    if (target.fileSha256 && sha256(historical) !== target.fileSha256) failures.push(`${target.file}: baseline no coincide con el commit de origen.`);
    if (target.fileSha256 && sha256(current) !== target.fileSha256) failures.push(`${target.file}: archivo protegido modificado.`);
    for (const method of target.methods) {
      const original = collectFunctions(historical, method.name);
      const actual = collectFunctions(current, method.name);
      functions += method.variants.length;
      const expected = method.variants.map((item) => item.sha256);
      if (JSON.stringify(original.map((item) => item.sha256)) !== JSON.stringify(expected)) failures.push(`${target.file}:${method.name}: baseline de función inválida.`);
      if (JSON.stringify(actual.map((item) => item.sha256)) !== JSON.stringify(expected)) failures.push(`${target.file}:${method.name}: función o número de sobrecargas modificado.`);
    }
  }
  return { ok: failures.length === 0, decision: baseline.decision, baseCommit: baseline.baseCommit, files: baseline.targets.length, functions, failures };
}

if (require.main === module) {
  try {
    const root = path.resolve(__dirname, "../..");
    const baseline = JSON.parse(fs.readFileSync(path.join(root, "Doc/Diagnostico/radicacion-interna/almacenamiento-intocable-baseline.json"), "utf8"));
    const result = verifyBaseline(root, baseline);
    process.stdout.write(JSON.stringify(result, null, 2) + "\n");
    if (!result.ok) process.exitCode = 1;
  } catch (error) {
    process.stderr.write(`No se pudo verificar almacenamiento intocable: ${error.message}\n`);
    process.exitCode = 1;
  }
}

module.exports = { collectFunctions, sha256, verifyBaseline };
