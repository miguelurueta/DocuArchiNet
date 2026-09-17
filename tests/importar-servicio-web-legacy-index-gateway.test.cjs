const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const gateway = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacySiiDocumentIndexPhysicalGateway.vb", "utf8");
const adapter = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/SiiDocumentIndexAdapter.vb", "utf8");

test("índices de gabinete resuelven estructura dinámica y parametrizan valores", () => {
  assert.match(gateway, /INFORMATION_SCHEMA\.COLUMNS/);
  assert.match(gateway, /c\.TABLE_SCHEMA=DATABASE\(\)/);
  assert.match(gateway, /c\.COLUMN_NAME=d\.CAMPO/);
  assert.match(gateway, /ResolveEffectiveFields\(connection, cabinet, campos\)/);
  assert.match(gateway, /assignments\.Add\("`" & pair\.Key & "`=" & parameterName\)/);
  assert.doesNotMatch(gateway, /SELECT NITCEDULA,RAZONSOCIAL,MATRICULA/);
  assert.match(gateway, /@imageId/);
  assert.match(gateway, /@expedientId/);
  assert.match(adapter, /MismatchedField\(fields, persisted\)/);
});

test("valores de texto respetan la longitud física y otros tipos no se recortan", () => {
  assert.match(gateway, /CHARACTER_MAXIMUM_LENGTH/);
  assert.match(gateway, /IsTextType\(metadata\.DataType\)/);
  assert.match(gateway, /clean\.Substring\(0, CInt\(Math\.Min\(metadata\.MaximumLength, Integer\.MaxValue\)\)\)/);
  assert.match(gateway, /Case "char", "varchar", "tinytext", "text", "mediumtext", "longtext"/);
});

test("errores MySQL del update se clasifican sin exponer el mensaje del servidor", () => {
  assert.match(gateway, /Catch ex As MySqlException/);
  assert.match(gateway, /SafeUpdateFailureCode\(ex\.Number\)/);
  for (const code of [
    "DOCUMENT_INDEX_UPDATE_PERMISSION_DENIED",
    "DOCUMENT_INDEX_UPDATE_COLUMN_INVALID",
    "DOCUMENT_INDEX_UPDATE_TABLE_UNAVAILABLE",
    "DOCUMENT_INDEX_UPDATE_TYPE_INVALID",
    "DOCUMENT_INDEX_UPDATE_TOO_LONG",
    "DOCUMENT_INDEX_UPDATE_CONSTRAINT_FAILED",
    "DOCUMENT_INDEX_UPDATE_CONCURRENCY_FAILED",
  ]) assert.match(gateway, new RegExp(code));
  assert.doesNotMatch(gateway, /ex\.Message/);
});

test("tipos dinámicos se validan antes del update y el código solo identifica el campo", () => {
  assert.match(gateway, /ValidForType\(fitted, metadata\.DataType\)/);
  assert.match(gateway, /DOCUMENT_INDEX_VALUE_TYPE_INVALID_" & SafeCodeField\(pair\.Key\)/);
  assert.match(gateway, /Decimal\.TryParse\(value/);
  assert.match(gateway, /DateTime\.TryParse\(value/);
  assert.doesNotMatch(gateway, /DOCUMENT_INDEX_VALUE_TYPE_INVALID_" & fitted/);
});

test("SQL y XML se verifican de manera independiente por documento", () => {
  assert.match(gateway, /Solicita_estructura_registro_relacion_expediente_indice/);
  assert.match(gateway, /File\.Exists\(xmlPath\)/);
  assert.match(gateway, /DocumentoIndizado/);
  assert.match(gateway, /productionId/);
  assert.match(adapter, /SqlConfirmado = _gateway\.ExisteIndiceSql/);
  assert.match(adapter, /XmlConfirmado = _gateway\.ExisteIndiceXml/);
});
