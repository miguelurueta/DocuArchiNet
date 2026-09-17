const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const source = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportDocumentLinkCacheRepository.vb", "utf8");
const project = fs.readFileSync("GestionDocumental-Docuarchi.net.vbproj", "utf8");

function register(current, candidate, physicalState = "Correcta") {
  if (physicalState !== "Correcta") return "failed";
  if (!current) return "insert";
  return current.expedient === candidate.expedient && current.radicado === candidate.radicado ? "confirmed" : "conflict";
}

test("lectura ausente no inventa autoridad", () => {
  assert.equal(register(null, { expedient: 10, radicado: "R1" }), "insert");
  assert.match(source, /If rows\.Count <> 1 Then Return Nothing/);
  assert.match(source, /WHERE task_id=@taskId AND image_id=@imageId AND cabinet_name=@cabinetName/);
});

test("entrada consistente se confirma sin sobrescribir", () => {
  const value = { expedient: 10, radicado: "R1" };
  assert.equal(register(value, value), "confirmed");
  assert.match(source, /If current IsNot Nothing Then Return Compare\(current, entrada\)/);
  assert.match(source, /DOCUMENT_LINK_CACHE_CONFIRMED/);
});

test("relación física ausente no se cachea", () => {
  assert.equal(register(null, { expedient: 10, radicado: "R1" }, "Ausente"), "failed");
  assert.match(source, /entrada\.EstadoRelacion <> EstadoRelacionDocumentoExpediente\.Correcta/);
  assert.match(source, /DOCUMENT_LINK_NOT_PHYSICALLY_CONFIRMED/);
});

test("expediente distinto produce conflicto", () => {
  assert.equal(register({ expedient: 10, radicado: "R1" }, { expedient: 11, radicado: "R1" }), "conflict");
  assert.match(source, /current\.IdExpedienteEsperado <> expected\.IdExpedienteEsperado/);
  assert.match(source, /DOCUMENT_LINK_CACHE_CONFLICT/);
});

test("carrera de inserción relee la clave única", () => {
  assert.match(source, /INSERT IGNORE INTO workflow_import_document_link_cache/);
  const reads = source.match(/ReadOne\(connection, entrada\.IdTarea, entrada\.IdImagen, entrada\.NombreGabinete\)/g) || [];
  assert.equal(reads.length, 2);
  assert.match(source, /DOCUMENT_LINK_CACHE_WRITE_UNKNOWN/);
});

test("todos los valores SQL son parámetros y el proyecto registra el repositorio", () => {
  for (const parameter of ["@taskId", "@imageId", "@cabinetName", "@expedientId", "@radicado", "@relationStatus", "@createdUtc", "@verifiedUtc"]) {
    assert.match(source, new RegExp(`P\\("${parameter}"`));
  }
  const include = "Infrastructure\\Repositories\\Workflow\\ImportarServicioWeb\\MySqlImportDocumentLinkCacheRepository.vb";
  assert.equal(project.split(include).length - 1, 1);
});
