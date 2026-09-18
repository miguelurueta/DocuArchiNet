const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const read = (file) => fs.readFileSync(file, "utf8");
const composition = read("webservice/WebServiceImportarServicioWebModern.asmx.vb");
const intent = read("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb");
const related = read("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportRelatedDocumentRepository.vb");
const cache = read("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportDocumentLinkCacheRepository.vb");
const reconciliation = read("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb");

test("cada puerto DOC-67 usado en producción tiene implementación concreta", () => {
  const implementations = [
    ["ISiiExpedientSubjectResolver", "LegacySiiExpedientSubjectResolver"],
    ["IPhysicalExpedientGateway", "ModernPhysicalExpedientGateway"],
    ["IImportExpedientCacheRepository", "MySqlImportExpedientCacheRepository"],
    ["IImportRelatedDocumentRepository", "MySqlImportRelatedDocumentRepository"],
    ["IDocumentExpedientPhysicalGateway", "ModernDocumentExpedientPhysicalGateway"],
    ["IImportDocumentLinkCacheRepository", "MySqlImportDocumentLinkCacheRepository"],
    ["ISiiDocumentIndexPhysicalGateway", "LegacySiiDocumentIndexPhysicalGateway"],
  ];
  for (const [port, concrete] of implementations) {
    const matches = [...fs.readdirSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients"), ...fs.readdirSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb")]
      .filter(x => x.endsWith(".vb")).map(x => {
        const base = fs.existsSync(`Infrastructure/Workflow/ImportarServicioWeb/Expedients/${x}`) ? "Infrastructure/Workflow/ImportarServicioWeb/Expedients" : "Infrastructure/Repositories/Workflow/ImportarServicioWeb";
        return read(`${base}/${x}`);
      }).join("\n");
    assert.match(matches, new RegExp(`Class ${concrete}`), concrete);
    assert.match(matches, new RegExp(`Implements ${port}`), port);
    assert.match(composition, new RegExp(`New ${concrete}`), `composición ${concrete}`);
  }
});

test("cada tabla y estado DOC-67 tiene escritor y lector", () => {
  assert.match(intent, /INSERT INTO workflow_import_inscription/);
  assert.match(intent, /UPDATE workflow_import_inscription/);
  assert.match(intent, /FROM workflow_import_inscription/);
  for (const column of ["expedient_id", "storage_status", "relation_status", "index_status", "cache_status"]) {
    assert.match(intent, new RegExp(`SET[^\n]*${column}|INSERT[^\n]*${column}`));
    assert.match(intent, new RegExp(`SELECT[^\n]*${column}`));
  }
  assert.match(related, /INSERT INTO workflow_import_related_document/);
  assert.match(reconciliation, /FROM workflow_import_related_document/);
  assert.match(cache, /INSERT(?: IGNORE)? INTO workflow_import_document_link_cache/);
  assert.match(cache, /FROM workflow_import_document_link_cache/);
});

test("la composición no permite coordinadores DOC-67 nulos", () => {
  assert.match(composition, /steps, expedientCoordinator, relatedCoordinator/);
  assert.doesNotMatch(composition, /New ImportServiceOrchestrator\([^\n]+steps\)\s*,/);
});
