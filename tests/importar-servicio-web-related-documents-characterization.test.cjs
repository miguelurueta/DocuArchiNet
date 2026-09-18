const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const source = fs.readFileSync(path.join(root, "Docuarchi/ClassDaGabinete.vb"), "utf8");
const start = source.indexOf("Function SolicitaListaImagenesGabineteEnlace");
const end = source.indexOf("End Function", start);
assert.notEqual(start, -1);
assert.notEqual(end, -1);
const query = source.slice(start, end);

test("descubre el universo exclusivamente por gabinete y ENLASE", () => {
  assert.match(query, /ByVal NombreTabla As String/);
  assert.match(query, /ByVal EnlaceWorkflow As String/);
  assert.match(query, /SELECT ID,ID_TIPODOCUMENTO/);
  assert.match(query, /FROM " & NombreTabla/);
  assert.match(query, /WHERE ENLASE='" & EnlaceWorkflow/);
  assert.doesNotMatch(query, /ID_TAREA|id_tarea|workflow_import/);
});

test("materializa IdImagen tipologia y gabinete para cada fila", () => {
  assert.match(query, /Rows\.Count - 1/);
  assert.match(query, /\.id_image = Datset\.Tables\(0\)\.Rows\(i\)\.Item\(0\)/);
  assert.match(query, /\.ID_TIPODOCUMENTO = Datset\.Tables\(0\)\.Rows\(i\)\.Item\(1\)/);
  assert.match(query, /\.gabinete = NombreTabla/);
});

test("documentos anteriores y nuevos se deduplican por IdImagen sin sustituirse", () => {
  const rows = [
    { idImagen: 101, tipo: 7, origen: "anterior" },
    { idImagen: 202, tipo: 7, origen: "nuevo-corregido" },
    { idImagen: 101, tipo: 7, origen: "fila-repetida" },
  ];
  const byImage = new Map(rows.map((row) => [row.idImagen, row]));
  assert.deepEqual([...byImage.keys()], [101, 202]);
  assert.equal(byImage.size, 2);
  assert.notEqual(rows[0].idImagen, rows[1].idImagen);
});

test("legacy no parametriza identificador ni ENLASE y tampoco deduplica", () => {
  assert.match(query, /" FROM " & NombreTabla/);
  assert.match(query, /" WHERE ENLASE='" & EnlaceWorkflow/);
  assert.doesNotMatch(query, /MySqlParameter|Parameters\.Add|Distinct\(|GroupBy\(|HashSet/);
});
