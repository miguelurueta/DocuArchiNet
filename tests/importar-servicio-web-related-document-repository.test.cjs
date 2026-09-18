const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const source = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportRelatedDocumentRepository.vb", "utf8");
const project = fs.readFileSync("GestionDocumental-Docuarchi.net.vbproj", "utf8");

test("consulta gabinete por ENLASE con valor parametrizado", () => {
  assert.match(source, /SELECT ID,ENLASE FROM `" & nombreGabinete & "` WHERE ENLASE=@radicado ORDER BY ID/);
  assert.match(source, /New MySqlParameter\("@radicado", radicadoSii\.Trim\(\)\)/);
  assert.doesNotMatch(source, /ENLASE='"|ENLASE=" & radicadoSii/);
});

test("gabinete dinámico exige identificador seguro", () => {
  assert.match(source, /Regex\.IsMatch\(value, "\^\[A-Za-z_\]\[A-Za-z0-9_\]\*\$"\)/);
  assert.match(source, /Not SafeIdentifier\(nombreGabinete\)/);
});

test("documentos anteriores y nuevos se deduplican por IdImagen", () => {
  const rows = [{ id: 11 }, { id: 12 }, { id: 11 }, { id: 13 }];
  const ids = [...new Set(rows.map((row) => row.id))];
  assert.deepEqual(ids, [11, 12, 13]);
  assert.match(source, /Dim seen As New HashSet\(Of Long\)\(\)/);
  assert.match(source, /seen\.Add\(row\.ImageId\)/);
  assert.match(source, /\.IdTarea = taskId/);
});

test("fallo es controlado y no expone SQL", () => {
  assert.match(source, /RELATED_DOCUMENT_QUERY_INVALID/);
  assert.match(source, /RELATED_DOCUMENT_QUERY_FAILED/);
  assert.match(source, /Throw New ImportRelatedDocumentRepositoryException/);
  const errorClass = source.slice(source.indexOf("Public NotInheritable Class ImportRelatedDocumentRepositoryException"));
  assert.doesNotMatch(errorClass, /InnerException|commandText|sql/i);
});

test("repositorio se registra una sola vez", () => {
  const include = "Infrastructure\\Repositories\\Workflow\\ImportarServicioWeb\\MySqlImportRelatedDocumentRepository.vb";
  assert.equal(project.split(include).length - 1, 1);
});
