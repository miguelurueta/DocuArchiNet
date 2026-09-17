const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const repository = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportRelatedDocumentRepository.vb", "utf8");
const coordinator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb", "utf8");

function discover(previous, currentQuery) {
  return [...new Map([...previous, ...currentQuery].map(item => [item.id, item])).values()];
}

test("universo por ENLASE incorpora IdImagen nuevo", () => {
  const previous = [{ id: 101, seal: "original" }];
  const current = [{ id: 101, seal: "original" }, { id: 102, seal: "corrected" }];
  assert.deepEqual(discover(previous, current).map(x => x.id), [101, 102]);
  assert.match(repository, /WHERE ENLASE=@radicado ORDER BY ID/);
  assert.match(coordinator, /_documents\.ObtenerPorEnlace\(contexto, nombreGabinete, radicadoSii\)/);
});

test("corrección es aditiva y conserva sello anterior", () => {
  const result = discover([{ id: 101, seal: "original" }], [{ id: 102, seal: "corrected" }]);
  assert.deepEqual(result, [{ id: 101, seal: "original" }, { id: 102, seal: "corrected" }]);
  assert.doesNotMatch(repository + coordinator, /DELETE\s+FROM|Eliminar|Sustituir|Reemplazar/i);
});

test("duplicados de consulta se eliminan solo por IdImagen", () => {
  const result = discover([], [{ id: 101 }, { id: 101 }, { id: 102 }]);
  assert.deepEqual(result.map(x => x.id), [101, 102]);
  assert.match(repository, /Dim seen As New HashSet\(Of Long\)\(\)/);
  assert.match(repository, /seen\.Add\(row\.ImageId\)/);
});

test("cada imagen del universo atraviesa el procesamiento secuencial", () => {
  assert.match(coordinator, /For Each document In plan\.Documentos/);
  assert.match(coordinator, /Not ProcessOne\(contexto, intencion\.Id, document, inscription, failureCode\)/);
  assert.match(coordinator, /RELATED_DOCUMENT_SQL_INDEX_MISSING/);
  assert.match(coordinator, /RELATED_DOCUMENT_XML_INDEX_MISSING/);
});
