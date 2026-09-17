const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const validator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ValidadorContextoImportacion.vb", "utf8");
const orchestrator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb", "utf8");
const reconciliation = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ServicioReconciliacionImportacion.vb", "utf8");
const repository = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportReconciliationRepository.vb", "utf8");
const dtos = fs.readFileSync("DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb", "utf8");

test("contexto actual se compara con toda la identidad persistida", () => {
  for (const property of ["IdUsuario", "IdGrupo", "IdTarea", "IdRuta", "IdTramite"]) {
    assert.match(validator, new RegExp(`contexto\\.${property} <> persistido\\.${property}`));
  }
  assert.match(validator, /contexto\.LoginUsuario, persistido\.LoginUsuario/);
  assert.match(validator, /contexto\.ProviderId, persistido\.ProviderId/);
  assert.match(validator, /PERSISTED_CONTEXT_MISMATCH/);
});

test("ejecución consulta y reconciliación rechazan discrepancias", () => {
  assert.match(orchestrator, /_validator\.Validar\(contexto, intent\.ContextoOriginal\)/);
  assert.match(orchestrator, /If Not persistedContextValidation\.Valido Then Return ErrorExecute/);
  assert.match(orchestrator, /Return ErrorGet\(response, "PERSISTED_CONTEXT_MISMATCH"\)/);
  assert.match(reconciliation, /_validator\.Validar\(context, snapshot\.ContextoOriginal\)/);
});

test("snapshot recupera contexto desde persistencia", () => {
  for (const column of ["group_id", "user_login", "route_id", "procedure_id"]) assert.match(repository, new RegExp(`intent\\.${column}`));
  assert.match(repository, /\.ContextoOriginal=New ContextoIntencionImportacion/);
});

test("cliente no puede elegir gabinete ni expediente", () => {
  for (const className of ["QueryItemsRequestDto", "CreateImportIntentRequestDto", "ExecuteImportIntentRequestDto", "ReconcileImportIntentRequestDto"]) {
    const start = dtos.indexOf(`Public Class ${className}`);
    const end = dtos.indexOf("End Class", start);
    const contract = dtos.slice(start, end);
    assert.doesNotMatch(contract, /NombreGabinete|Gabinete|IdExpediente|ExpedientId/i, className);
  }
});

test("adaptadores modernos confinan sesión al borde legacy", () => {
  const files = [
    "Infrastructure/Workflow/ImportarServicioWeb/Expedients/PhysicalExpedientGateway.vb",
    "Infrastructure/Workflow/ImportarServicioWeb/Expedients/DocumentExpedientRelationAdapter.vb",
    "Infrastructure/Workflow/ImportarServicioWeb/Expedients/SiiDocumentIndexAdapter.vb",
    "Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb"
  ];
  const source = files.map(file => fs.readFileSync(file, "utf8")).join("\n");
  assert.doesNotMatch(source, /HttpContext|Session\.|Session\(|Request\.Cookies/);
  assert.doesNotMatch(source, /\.asmx|HttpWebRequest|WebClient/);
});
