const test = require("node:test");
const assert = require("node:assert/strict");
const adapterFactory = require("../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-adapter.js");
const mapper = require("../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-contract-mapper.js");
const list = require("../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-list.js");

test("consulta capacidades e items una sola vez con contexto", async () => {
  const calls = []; const api = { resolveCapabilities: async r => (calls.push(["cap", r]), { ContextAllowed: true, DocumentTypes: [{ DocumentTypeId: 1 }] }), queryItems: async r => (calls.push(["query", r]), { Items: [{ ExternalKey: "opaque", DisplayName: "Uno", AllowedActions: ["IMPORT"] }] }) };
  const adapter = adapterFactory.create({ api, mapper, list, contextFactory: () => ({ TaskId: 73, ProviderId: "INTEGRACIONSII" }) }); const result = await adapter.queryItems({ CodigoBarras: "ABC" });
  assert.deepEqual(calls.map(x => x[0]), ["cap", "query"]); assert.equal(calls[1][1].TaskId, 73); assert.equal(result.Items.length, 1);
});

test("identidad canónica y fuente sin transporte ni logs", () => {
  assert.equal(adapterFactory.canonicalId, "INTEGRACIONSII"); const source = require("node:fs").readFileSync(require("node:path").resolve(__dirname, "../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-adapter.js"), "utf8"); assert.doesNotMatch(source, /fetch\s*\(|XMLHttpRequest|\$\.ajax|console\./);
});
