const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const crypto = require("node:crypto");

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

test("superficies legacy conservan huellas canónicas", () => {
  const expected = {
    "workflow/ClassAlmacenamiento.vb": "b875d24f0a9ff63f24a4fff96f637cb04afb1405",
    "Gestion/ClassGaExpediente.vb": "2998523902ec2d455ac4b96a644297674d6d14b9",
    "webservice/WebServiceGaExpediente.asmx.vb": "11a60af88f9b3591e70ca3e91567d34fe39fce7f",
    "webservice/WebService_integracion_sii.asmx.vb": "580c3832343205f2246aa0acbfcc8f703f2a0ebe"
  };
  for (const [relative, hash] of Object.entries(expected)) {
    const content = Buffer.from(fs.readFileSync(path.join(root, relative), "utf8").replace(/\r\n/g, "\n"));
    const header = Buffer.from(`blob ${content.length}\0`);
    assert.equal(crypto.createHash("sha1").update(Buffer.concat([header, content])).digest("hex"), hash, relative);
  }
});

test("legacy no referencia coordinadores modernos", () => {
  for (const relative of ["Gestion/ClassGaExpediente.vb", "webservice/WebServiceGaExpediente.asmx.vb", "webservice/WebService_integracion_sii.asmx.vb"]) {
    assert.doesNotMatch(fs.readFileSync(path.join(root, relative), "utf8"), /ImportExpedientCoordinator|ImportRelatedDocumentCoordinator|ImportServiceOrchestrator/, relative);
  }
});
