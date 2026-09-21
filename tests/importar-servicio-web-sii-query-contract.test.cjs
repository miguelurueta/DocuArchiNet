const test = require("node:test");
const assert = require("node:assert/strict");
const mapper = require("../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-contract-mapper.js");

test("mapea contrato normalizado sin interpretar ExternalKey", () => {
  const response = mapper.mapResponse({ Items: [{ ExternalKey: "SII2.opaco", DisplayName: "Acto", ImportStatus: "AVAILABLE", AllowedActions: ["IMPORT"], Metadata: [{ Code: "LIBRO", Value: "1" }, { Code: "INSCRIPCION", Value: "22" }] }] });
  assert.equal(response.items[0].externalKey, "SII2.opaco"); assert.equal(response.items[0].book, "1"); assert.equal(response.items[0].inscription, "22"); assert.equal(response.items[0].importable, true);
});

test("rechaza respuesta o item sin forma mínima", () => {
  assert.throws(() => mapper.mapResponse({}), /SII_QUERY_RESPONSE_INVALID/);
  assert.throws(() => mapper.mapResponse({ Items: [{ ExternalKey: "x" }] }), /SII_QUERY_RESPONSE_INVALID/);
});
