const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const required = [
  "DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb",
  "Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb",
  "Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb",
  "Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalImportHttpTransport.vb",
  "Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb",
  "webservice/WebServiceImportarServicioWebModern.asmx.vb",
];

test("conserva las fronteras canonicas de B01-B06", () => {
  for (const relative of required) {
    assert.equal(fs.existsSync(path.join(root, relative)), true, relative);
  }
});

test("reutiliza un unico arbol de fixtures y el arnes e2e oficial", () => {
  assert.equal(fs.existsSync(path.join(root, "Tests/Fixtures/Workflow/ImportarServicioWeb")), true);
  assert.equal(fs.existsSync(path.join(root, "tools/e2e")), true);
  for (const forbidden of ["e2e", "playwright.config.js", "Tests/Fixtures/ImportarServicioWeb"]) {
    assert.equal(fs.existsSync(path.join(root, forbidden)), false, forbidden);
  }
});

test("mantiene las capas internas libres de estado web", () => {
  for (const relative of required.filter((item) => !item.startsWith("webservice/"))) {
    const source = fs.readFileSync(path.join(root, relative), "utf8");
    assert.doesNotMatch(source, /HttpContext\.Current|Session\.Item/i, relative);
  }
});

test("documentacion tecnica segmenta diagramas y referencia simbolos implementados", () => {
  const docs = path.join(root, "Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-56-pruebas-backend-evidencia");
  const expectations = {
    "01-frontera-composicion.md": ["TryBuildImportContext", "Compose", "ValidRequest"],
    "02-consulta-preview-sii.md": ["QueryItemsAsync", "GetPreviewAsync", "BuildExternalKey"],
    "03-preflight-intencion.md": ["CrearOReutilizar", "Adquirir", "Resolver"],
    "04-ejecucion-almacenamiento.md": ["DownloadImportExecutionStep", "StoreImportExecutionStep", "AlmacenaDocumentoTareaWorkflow"],
    "05-estados-reintento.md": ["CrearTabla", "EsReintentoSeguro"],
    "06-reconciliacion-salida.md": ["ProjectExecutionResult", "ImportItemResultMapper"],
    "07-persistencia-telemetria.md": ["workflow_import_intent", "ra_ser_intento_serviciointegracion"],
    "08-almacenamiento-legacy.md": ["SolicitaEstructuraExpedienteDocumentoVinculante", "SolicitaValoresCamposDocumentoGabinete", "Almacenamiento"]
  };
  const implementation = [
    "webservice/WebServiceImportarServicioWebModern.asmx.vb",
    "Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb",
    "Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb",
    "Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb",
    "Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb",
    "Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb",
    "Services/Workflow/ImportarServicioWeb/ImportItemResultMapper.vb",
    "Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb",
    "Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb",
    "Infrastructure/Workflow/ImportarServicioWeb/MySqlImportIntentConcurrencyGuard.vb",
    "Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb",
    "Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlExternalServiceTelemetryRepository.vb",
    "workflow/ClassAlmacenamiento.vb"
  ].map((relative) => fs.readFileSync(path.join(root, relative), "utf8")).join("\n");

  for (const [name, symbols] of Object.entries(expectations)) {
    const diagram = fs.readFileSync(path.join(docs, "Diagramas", name), "utf8");
    assert.match(diagram, /Fuente(?:s)?:/i, name);
    assert.match(diagram, /```mermaid[\s\S]+```/, name);
    for (const symbol of symbols) {
      assert.equal(diagram.includes(symbol), true, `${name}: ${symbol}`);
      assert.equal(implementation.includes(symbol), true, `codigo: ${symbol}`);
    }
  }
});
