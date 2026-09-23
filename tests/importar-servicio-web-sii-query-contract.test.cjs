const test = require("node:test");
const assert = require("node:assert/strict");
const mapper = require("../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-contract-mapper.js");

test("mapea contrato normalizado sin interpretar ExternalKey", () => {
  const response = mapper.mapResponse({ Items: [{ ExternalKey: "SII2.opaco", DisplayName: "Acto", ImportStatus: "AVAILABLE", AllowedActions: ["IMPORT"], Metadata: [{ Code: "LIBRO", Value: "1" }, { Code: "INSCRIPCION", Value: "22" }] }] });
  assert.equal(response.items[0].externalKey, "SII2.opaco"); assert.equal(response.items[0].book, "1"); assert.equal(response.items[0].inscription, "22"); assert.equal(response.items[0].importable, true);
});

test("mapea registration del contrato SII como inscripción visible", () => {
  const response = mapper.mapResponse({ Items: [{ ExternalKey: "SII2.opaco", DisplayName: "Acto", ImportStatus: "Disponible", AllowedActions: ["Import"], Metadata: [{ Code: "book", Value: "RM15" }, { Code: "registration", Value: "12345" }] }] });
  assert.equal(response.items[0].book, "RM15");
  assert.equal(response.items[0].inscription, "12345");
  assert.equal(response.items[0].importable, true);
});

test("rechaza respuesta o item sin forma mínima", () => {
  assert.throws(() => mapper.mapResponse({ Error: { Codigo: "SERVER_BARCODE_UNAVAILABLE" }, Items: [] }), /SERVER_BARCODE_UNAVAILABLE/);
  assert.throws(() => mapper.mapResponse({}), /SII_QUERY_RESPONSE_INVALID/);
  assert.throws(() => mapper.mapResponse({ Items: [{ ExternalKey: "x" }] }), /SII_QUERY_RESPONSE_INVALID/);
});

test("QueryItems resuelve recibo y código de barras desde la tarea confiable", () => {
  const fs = require("node:fs");
  const service = fs.readFileSync("webservice/WebServiceImportarServicioWebModern.asmx.vb", "utf8");
  const start = service.indexOf("Public Function QueryItems");
  const end = service.indexOf("End Function", start);
  const body = service.slice(start, end);
  assert.match(body, /TryResolveTrustedSiiReferences\(importContext, trustedReceipt, trustedBarcode\)/);
  assert.match(body, /request\.CodigoBarras = trustedBarcode/);
  assert.match(body, /response\.Radicado = trustedReceipt/);
  assert.doesNotMatch(body, /request\.CodigoBarras\.Trim/);
  assert.match(service, /SolicitaReciboCodigoBarrasSII\(context\.IdTarea, context\.NombreRutaWorkflow/);
  const createStart = service.indexOf("Public Function CreateImportIntent");
  const createEnd = service.indexOf("End Function", createStart);
  assert.match(service.slice(createStart, createEnd), /request\.Radicado = trustedReceipt/);
});
