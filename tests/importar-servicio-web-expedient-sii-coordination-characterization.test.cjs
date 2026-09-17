const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const source = fs.readFileSync(path.join(root, "Gestion/ClassGaExpediente.vb"), "utf8");
const start = source.indexOf("Function CreaExpedienteIntegracionSII");
const end = source.indexOf("Function Auto_vincula_documentos_a_expediente_estructura", start);
assert.notEqual(start, -1);
assert.notEqual(end, -1);
const coordinator = source.slice(start, end);

test("valida configuracion y universo documental antes de crear", () => {
  assert.match(coordinator, /SolicitaEstructuraTramite\(IdTramite/);
  assert.match(coordinator, /SolicitaListaImagenesGabineteEnlace\([\s\S]*CIncripcionSII\(0\)\.RADICADO_SII/);
  assert.match(coordinator, /StruImageGabineteWorfkflow Is Nothing[\s\S]*No fue posible encontrar documentos/);
  assert.match(coordinator, /If ReciboSII = "" Then/);
  assert.match(coordinator, /If CodigoBarrasSII = "" Then/);
});

test("modo unico usa la primera inscripcion y asigna todas las imagenes al mismo expediente", () => {
  assert.match(coordinator, /util_Estado_Multiple_expedienteSII <> 1[\s\S]*CIncripcionSII\(0\)/);
  assert.match(coordinator, /HttpContext\.Current\.Session\.Item\("ID_TAREA_SELECCIONDA"\)/);
  assert.match(coordinator, /For z As Integer = 0 To StruImageGabineteWorfkflow\.Length - 1[\s\S]*IdExpedienteWeb = CStruSiiCahcheExpediente\.IdExpediente/);
});

test("modo multiple separa primario y secundarios mediante tipologia", () => {
  assert.match(coordinator, /util_Estado_Multiple_expedienteSII = 1/);
  assert.match(coordinator, /SolicitaListaTiposExpedienteSegundarioSii/);
  assert.match(coordinator, /For i As Integer = 0 To CIncripcionSII\.Count - 1/);
  assert.match(coordinator, /MatriculaPropietario = ""[\s\S]*If Testigo = 0 Then[\s\S]*ClsssStructureVinculaDocumento\.Add/);
  assert.match(coordinator, /MatriculaPropietario <> ""[\s\S]*ID_TIPODOCUMENTO = StruTiposExpedienteSegundarioSII\(k\)\.IdTipo[\s\S]*ClsssStructureVinculaDocumento\.Add/);
  assert.match(coordinator, /EstadoPadre = 1/);
  assert.match(coordinator, /EstadoPadre = 2/);
});

test("legacy no deduplica IdImagen ni garantiza un destino en modo multiple", () => {
  const addCount = (coordinator.match(/ClsssStructureVinculaDocumento\.Add/g) || []).length;
  assert.ok(addCount >= 3, `se esperaban varias rutas Add; encontradas ${addCount}`);
  assert.doesNotMatch(coordinator, /Dictionary\(Of Integer|HashSet\(Of Integer|Distinct\(|GroupBy\(/);
  assert.match(coordinator, /For i As Integer = 0 To CIncripcionSII\.Count - 1[\s\S]*For z As Integer = 0 To StruImageGabineteWorfkflow\.Length - 1/);
});
