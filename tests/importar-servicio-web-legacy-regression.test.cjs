const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const progress = fs.readFileSync(path.join(root, "js/java_general/JSProgresBar.js"), "utf8");

test("JSProgresBar no conoce ni ejecuta la importacion moderna", () => {
  assert.doesNotMatch(progress, /WebServiceImportarServicioWebModern|ImportServiceOrchestrator|ExecuteImportIntent|CreateImportIntent/);
});

test("coexisten descriptor moderno y endpoints legacy", () => {
  for (const relative of [
    "webservice/WebServiceImportarServicioWebModern.asmx",
    "webservice/WebService_integracion_sii.asmx",
    "webservice/WebServiceGaExpediente.asmx",
  ]) assert.equal(fs.existsSync(path.join(root, relative)), true, relative);
});
