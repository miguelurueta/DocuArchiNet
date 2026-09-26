const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const composition = fs.readFileSync("webservice/WebServiceImportarServicioWebModern.asmx.vb", "utf8");
const orchestrator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb", "utf8");

test("composición productiva conecta todos los puertos DOC-67", () => {
  for (const concrete of ["ModernSiiExpedientSubjectResolver", "ModernPhysicalExpedientGateway", "MySqlImportExpedientCacheRepository", "MySqlImportRelatedDocumentRepository", "ModernDocumentExpedientPhysicalGateway", "MySqlImportDocumentLinkCacheRepository", "LegacySiiDocumentIndexPhysicalGateway"]) assert.match(composition, new RegExp(`New ${concrete}`));
  assert.match(composition, /New ImportServiceOrchestrator\([^\n]+steps, executionExpedientCoordinator, executionRelatedCoordinator/);
});

test("expediente precede almacenamiento y completada sucede tras documentos relacionados", () => {
  assert.ok(orchestrator.indexOf("_expedientCoordinator.Resolver") < orchestrator.indexOf("For Each item In intent.Resultados"));
  assert.ok(orchestrator.indexOf("_relatedDocumentCoordinator.Procesar") < orchestrator.lastIndexOf("FaseImportacionServicio.Completada"));
  assert.match(composition, /If isEnlase Then[\s\S]*CompleteImportExecutionStep\(FaseImportacionServicio\.Completada\)[\s\S]*Else[\s\S]*CompleteImportExecutionStep\(FaseImportacionServicio\.CacheActualizado\)/);
});
