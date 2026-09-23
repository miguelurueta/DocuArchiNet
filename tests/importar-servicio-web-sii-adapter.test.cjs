const test = require("node:test");
const assert = require("node:assert/strict");
const adapterFactory = require("../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-adapter.js");
const mapper = require("../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-contract-mapper.js");
const list = require("../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-list.js");

test("consulta capacidades e items una sola vez con contexto", async () => {
  const calls = []; const api = { resolveCapabilities: async r => (calls.push(["cap", r]), { ContextAllowed: true, DocumentTypes: [{ DocumentTypeId: 1 }] }), queryItems: async r => (calls.push(["query", r]), { Radicado: "S002188422", Items: [{ ExternalKey: "opaque", DisplayName: "Uno", AllowedActions: ["IMPORT"] }] }) };
  const adapter = adapterFactory.create({ api, mapper, list, contextFactory: () => ({ TaskId: 73, ProviderId: "INTEGRACIONSII" }) }); const result = await adapter.queryItems({ CodigoBarras: "ABC" });
  assert.deepEqual(calls.map(x => x[0]), ["cap", "query"]); assert.equal(calls[1][1].TaskId, 73); assert.equal(result.Items.length, 1); assert.deepEqual(result.DocumentTypes, [{ DocumentTypeId: 1 }]); assert.equal(result.Radicado, "S002188422");
});

test("catálogo de tipologías proviene de capacidades y no de QueryItems", async () => {
  const catalog = [{ DocumentTypeId: 154, Name: "Constancia De Inscripción" }];
  const api = { resolveCapabilities: async () => ({ ContextAllowed: true, DocumentTypes: catalog }), queryItems: async () => ({ Items: [{ ExternalKey: "item-1", DisplayName: "Constancia", AllowedActions: ["IMPORT"] }], DocumentTypes: [{ DocumentTypeId: 999 }] }) };
  const adapter = adapterFactory.create({ api, mapper, list, contextFactory: () => ({ TaskId: 219877, ProviderId: "INTEGRACIONSII" }) });
  const result = await adapter.queryItems({});
  assert.deepEqual(result.DocumentTypes, catalog);
});

test("identidad canónica y fuente sin transporte ni logs", () => {
  assert.equal(adapterFactory.canonicalId, "INTEGRACIONSII"); const source = require("node:fs").readFileSync(require("node:path").resolve(__dirname, "../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-adapter.js"), "utf8"); assert.doesNotMatch(source, /fetch\s*\(|XMLHttpRequest|\$\.ajax|console\./);
});

test("tabla presenta encabezados y mantiene visibles las acciones", () => {
  const fs = require("node:fs"); const path = require("node:path");
  const source = fs.readFileSync(path.resolve(__dirname, "../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-adapter.js"), "utf8");
  const css = fs.readFileSync(path.resolve(__dirname, "../Styles/importar-servicio-web-modern.css"), "utf8");
  assert.match(source, /createElement\("thead"\)/);
  ["Libro", "Inscripción", "Fecha", "Naturaleza \/ acto", "Noticia", "Referencia", "Estado", "Acciones"].forEach(label => assert.match(source, new RegExp(label)));
  assert.match(source, /data-import-select-all/);
  assert.match(source, /Seleccionar o deseleccionar todas las inscripciones disponibles/);
  assert.match(source, /importar-servicio-web-sii__actions-cell/);
  assert.match(css, /importar-servicio-web-sii__actions-cell[^}]*position:\s*sticky/);
  assert.match(css, /importar-servicio-web-sii__actions-cell[^}]*right:\s*0/);
});

test("preparación abandona la composición de vista previa", () => {
  const source = require("node:fs").readFileSync(require("node:path").resolve(__dirname, "../js/workflow/importar-servicio-web/importar-servicio-web-ui.js"), "utf8");
  const start = source.indexOf("function openPreparation");
  const fragment = source.slice(start, source.indexOf("function renderPreview", start));
  assert.match(fragment, /classList\.remove\("importar-servicio-web__dialog--preview"\)/);
  assert.match(fragment, /control\.preview\.close\(\)/);
});
