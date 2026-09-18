const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const read = (relative) => fs.readFileSync(path.join(root, relative), "utf8");

const compositionPath = "webservice/WebServiceImportarServicioWebModern.asmx.vb";
const projectPath = "GestionDocumental-Docuarchi.net.vbproj";
const orchestratorPath = "Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb";
const contextPath = "Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb";
const physicalGatewayPath = "Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyPhysicalExpedientGateway.vb";
const relationGatewayPath = "Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyDocumentExpedientPhysicalGateway.vb";
const mutationBridgePath = "Infrastructure/Workflow/ImportarServicioWeb/Expedients/ModernExpedientMutationBridge.vb";
const cachePath = "Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportExpedientCacheRepository.vb";

test("DOC-67 readiness: la composición productiva usa exclusivamente los puertos modernos migrados", () => {
  const source = read(compositionPath);
  for (const implementation of [
    "ModernSiiExpedientSubjectResolver",
    "ModernPhysicalExpedientGateway",
    "MySqlImportExpedientCacheRepository",
    "ModernDocumentExpedientPhysicalGateway",
    "MySqlImportRelatedDocumentRepository",
    "MySqlImportDocumentLinkCacheRepository",
  ]) assert.match(source, new RegExp(`New ${implementation}\\b`), implementation);

  for (const obsolete of [
    "New LegacyPhysicalExpedientGateway",
    "New LegacyImportExpedientCacheRepository",
    "New LegacyDocumentExpedientPhysicalGateway",
  ]) assert.doesNotMatch(source, new RegExp(obsolete), obsolete);
});

test("DOC-67 readiness: la ruta moderna no reinvoca las cuatro funciones legacy migradas", () => {
  const modernClosure = [compositionPath, physicalGatewayPath, relationGatewayPath, mutationBridgePath, cachePath]
    .map(read).join("\n");
  for (const invocation of [
    /\.AutoRegistraExpedienteTramite\s*\(/,
    /\.SolicitaCacheCreacionExpedienteSII\s*\(/,
    /\.RegistraCacheCreacionExpedienteSII\s*\(/,
    /\.VinculaDocumentoExpediente\s*\(/,
  ]) assert.doesNotMatch(modernClosure, invocation);
});

test("DOC-67 readiness: los gateways físicos no dependen de sesión implícita", () => {
  for (const relative of [physicalGatewayPath, relationGatewayPath, mutationBridgePath]) {
    const source = read(relative);
    assert.doesNotMatch(source, /HttpContext|\.Session\b/, relative);
  }
  const context = read(contextPath);
  for (const field of ["IdUsuarioGestion", "IdEmpresaGestion", "NombreRutaWorkflow"])
    assert.match(context, new RegExp(`Property ${field}\\b`), field);
  const boundary = read(compositionPath);
  assert.match(boundary, /result\.Contexto\.IdUsuarioGestion[\s\S]*result\.Contexto\.IdEmpresaGestion[\s\S]*result\.Contexto\.NombreRutaWorkflow/);
});

test("DOC-67 readiness: proyecto y composición contienen una sola implementación de cada pieza crítica", () => {
  const project = read(projectPath).replaceAll("/", "\\");
  for (const relative of [physicalGatewayPath, relationGatewayPath, mutationBridgePath, cachePath]) {
    const windowsPath = relative.replaceAll("/", "\\");
    assert.equal(project.split(windowsPath).length - 1, 1, relative);
  }
});

test("DOC-67 readiness: expediente precede almacenamiento y documentos relacionados preceden Completada", () => {
  const source = read(orchestratorPath);
  const expedient = source.indexOf("_expedientCoordinator.Resolver");
  const storageLoop = source.indexOf("For Each item In intent.Resultados");
  const related = source.indexOf("_relatedDocumentCoordinator.Procesar");
  const completed = source.lastIndexOf("FaseImportacionServicio.Completada");
  assert.ok(expedient >= 0 && expedient < storageLoop, "expediente antes del almacenamiento");
  assert.ok(related >= 0 && related < completed, "relacionados antes de Completada");
  const physicalEffects = source.slice(related, completed);
  for (const effect of ["EstadoRelacion", "EstadoIndice", "EstadoCache"])
    assert.match(physicalEffects, new RegExp(`item\\.${effect} = EstadoEfectoExpedienteImportacion\\.Confirmado`), effect);
});

test("DOC-67 readiness: la integración local general cubre MERCANTIL, ESAL y RUP", () => {
  const integration = read("tests/importar-servicio-web-full-saga-local-integration.test.cjs");
  for (const registry of ["MERCANTIL", "ESAL", "RUP"])
    assert.match(integration, new RegExp(`cabinet: "${registry}"`), registry);
  for (const effect of ["expedient", "storage", "related", "links", "cache", "indices", "xml"])
    assert.match(integration, new RegExp(`state\\.${effect}|"${effect}"`), effect);
});
