const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const orchestrator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb", "utf8");
const steps = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb", "utf8");
const storage = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb", "utf8");

test("orden moderno es expedientes almacenamiento y documentos relacionados", () => {
  const expedients = orchestrator.indexOf("_expedientCoordinator.Resolver");
  const itemLoop = orchestrator.indexOf("For Each item In intent.Resultados", expedients);
  const stepLoop = orchestrator.indexOf("For Each stepItem In _steps", itemLoop);
  const related = orchestrator.indexOf("_relatedDocumentCoordinator.Procesar", stepLoop);
  assert.ok(expedients >= 0 && itemLoop > expedients && stepLoop > itemLoop && related > stepLoop);
});

test("items continúan ejecutándose uno por uno", () => {
  assert.match(orchestrator, /For Each item In intent\.Resultados[\s\S]*For Each stepItem In _steps/);
  assert.match(orchestrator, /response\.Items\.Add\(MapItem\(item\)\)/);
  assert.match(orchestrator, /If intent\.DetencionSolicitada Then[\s\S]*Continue For/);
});

test("documentos relacionados esperan todos los IdImagen", () => {
  assert.match(orchestrator, /If Not AllItemsStored\(intent\) Then/);
  assert.match(orchestrator, /Not item\.IdDocumento\.HasValue OrElse item\.IdDocumento\.Value <= 0 Then Return False/);
  assert.match(orchestrator, /RELATED_DOCUMENTS_WAITING_FOR_STORAGE/);
});

test("la finalización persiste reconciliada antes de completada", () => {
  const related = orchestrator.indexOf("_relatedDocumentCoordinator.Procesar");
  const reconciled = orchestrator.indexOf("FaseImportacionServicio.Reconciliada", related);
  const completed = orchestrator.indexOf("FaseImportacionServicio.Completada", reconciled);
  assert.ok(related >= 0 && reconciled > related && completed > reconciled);
  assert.match(orchestrator, /Avanzar\(contexto, intent, item, FaseImportacionServicio\.Reconciliada/);
});

test("el plan físico confirmado se proyecta al item antes de reconciliar y completar", () => {
  const related = orchestrator.indexOf("_relatedDocumentCoordinator.Procesar");
  const relation = orchestrator.indexOf("item.EstadoRelacion = EstadoEfectoExpedienteImportacion.Confirmado", related);
  const index = orchestrator.indexOf("item.EstadoIndice = EstadoEfectoExpedienteImportacion.Confirmado", relation);
  const cache = orchestrator.indexOf("item.EstadoCache = EstadoEfectoExpedienteImportacion.Confirmado", index);
  const reconciled = orchestrator.indexOf("Avanzar(contexto, intent, item, FaseImportacionServicio.Reconciliada", cache);
  const completed = orchestrator.indexOf("Avanzar(contexto, intent, item, FaseImportacionServicio.Completada", reconciled);
  assert.ok(related >= 0 && relation > related && index > relation && cache > index && reconciled > cache && completed > reconciled);
});

test("almacenamiento existente conserva una sola frontera y llamada", () => {
  assert.equal((storage.match(/AlmacenaDocumentoTareaWorkflow\(/g) || []).length, 1);
  assert.equal((steps.match(/_storage\.Almacenar\(command\)/g) || []).length, 1);
  assert.doesNotMatch(orchestrator, /AlmacenaDocumentoTareaWorkflow|_storage\.Almacenar/);
});

test("constructor anterior mantiene compatibilidad y ruta moderna es optativa", () => {
  assert.match(orchestrator, /Me\.New\(validator, repository, machine, steps, Nothing, Nothing\)/);
  assert.match(orchestrator, /If _expedientCoordinator IsNot Nothing Then/);
  assert.match(orchestrator, /If _relatedDocumentCoordinator IsNot Nothing AndAlso expedientPlan IsNot Nothing Then/);
});
