const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const coordinator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb", "utf8");
const plan = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentPlan.vb", "utf8");
const models = fs.readFileSync("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb", "utf8");
const project = fs.readFileSync("GestionDocumental-Docuarchi.net.vbproj", "utf8");

test("universo completo se consulta una vez por gabinete y ENLASE", () => {
  assert.equal((coordinator.match(/_documents\.ObtenerPorEnlace\(/g) || []).length, 1);
  assert.match(coordinator, /For Each document In discovered/);
});

test("cada IdImagen obtiene exactamente un destino", () => {
  assert.match(plan, /Dim seen As New HashSet\(Of Long\)\(\)/);
  assert.match(plan, /Not seen\.Add\(document\.IdImagen\) Then Continue For/);
  assert.match(plan, /If destination Is Nothing Then Return result/);
  assert.match(plan, /Return If\(matches\.Count = 1, matches\(0\), Nothing\)/);
  assert.match(plan, /\.IdExpedienteEsperado = destination\.IdExpediente/);
});

test("resuelve nuevos almacenados anteriores cacheados y destino único", () => {
  assert.match(plan, /storedItems\(0\)\.ClaveInscripcion/);
  assert.match(coordinator, /document\.IdExpedienteEsperado = cached\.IdExpedienteEsperado/);
  assert.match(plan, /If distinctExpedients\.Count = 1 Then Return plan\.Destinos\.First\(\)/);
});

test("procesamiento por imagen es vínculo cache índices SQL y XML", () => {
  const relation = coordinator.indexOf("_relations.Vincular");
  const cache = coordinator.indexOf("_cache.RegistrarVerificado", relation);
  const update = coordinator.indexOf("_indices.Actualizar", cache);
  const verify = coordinator.indexOf("_electronicIndex.Verificar", update);
  assert.ok(relation >= 0 && cache > relation && update > cache && verify > update);
  assert.match(coordinator, /If Not evidence\.SqlConfirmado Then failureCode = "RELATED_DOCUMENT_SQL_INDEX_MISSING"/);
  assert.match(coordinator, /If Not evidence\.XmlConfirmado Then failureCode = "RELATED_DOCUMENT_XML_INDEX_MISSING"/);
});

test("fallo detiene la secuencia sin declarar el plan confirmado", () => {
  assert.match(coordinator, /If inscription Is Nothing Then[\s\S]*If Not ProcessOne[\s\S]*Return plan/);
  assert.match(coordinator, /ResultadoIncierto/);
  assert.match(coordinator, /EstadoEfectoExpedienteImportacion\.Conflicto/);
});

test("modelo y proyecto registran el plan físico", () => {
  assert.match(models, /Class PlanDocumentosRelacionadosImportacion/);
  for (const include of [
    "Services\\Workflow\\ImportarServicioWeb\\ImportRelatedDocumentPlan.vb",
    "Services\\Workflow\\ImportarServicioWeb\\ImportRelatedDocumentCoordinator.vb"
  ]) assert.equal(project.split(include).length - 1, 1, include);
});
