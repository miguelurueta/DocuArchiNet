const test = require("node:test");
const assert = require("node:assert/strict");
const list = require("../js/workflow/importar-servicio-web/sii/importar-servicio-web-sii-list.js");

test("filtra y pagina una instantánea sin transporte", () => {
  const model = list.create({ pageSize: 1 }); model.replace([{ externalKey: "1", importable: true, importStatus: "AVAILABLE" }, { externalKey: "2", importable: false, importStatus: "IMPORTED" }]);
  assert.equal(model.snapshot().items.length, 1); assert.equal(model.setPage(2).items[0].externalKey, "2"); assert.equal(model.setFilter("available").total, 1); assert.equal(model.setFilter("imported").total, 1);
});

test("selección masiva excluye importados", () => {
  const model = list.create(); model.replace([{ externalKey: "1", importable: true }, { externalKey: "2", importable: false }]); assert.deepEqual(model.selectAll().selected, ["1"]); assert.deepEqual(model.select("2", true).selected, ["1"]);
});
