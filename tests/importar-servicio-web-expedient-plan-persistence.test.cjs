const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const repository = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb", "utf8");
const orchestrator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb", "utf8");

test("el plan lógico se persiste antes del almacenamiento", () => {
  assert.match(orchestrator, /PersistirPlanExpedientes\(contexto, intent, expedientPlan\)/);
  assert.ok(orchestrator.indexOf("PersistirPlanExpedientes") < orchestrator.indexOf("For Each item In intent.Resultados"));
  assert.match(repository, /UPDATE workflow_import_inscription SET normalized_matricula=/);
  assert.match(repository, /UPDATE workflow_import_intent_item SET expedient_id=/);
});

test("los estados DOC-67 se escriben y rehidratan", () => {
  for (const column of ["expedient_id", "storage_status", "relation_status", "index_status", "cache_status"]) {
    assert.match(repository, new RegExp(column));
  }
  assert.match(repository, /EstadoAlmacenamiento=ParseEffect/);
  assert.match(repository, /item\.IdDocumento\.HasValue Then item\.EstadoAlmacenamiento = EstadoEfectoExpedienteImportacion\.Confirmado/);
});
