const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");

function source(relativePath) {
  return fs.readFileSync(path.resolve(__dirname, "..", relativePath), "utf8");
}

function methodBlock(contents, name) {
  const match = contents.match(new RegExp(`Public Function ${name}\\b[\\s\\S]*?\\r?\\n    End Function`));
  assert.ok(match, `falta el método ${name}`);
  return match[0];
}

const workflowService = source("webservice/WebServiceWorkflowModern.asmx.vb");
const notesService = source("webservice/WebServiceWorkflowNotesModern.asmx.vb");
const importService = source("webservice/WebServiceImportarServicioWebModern.asmx.vb");
const contextResolver = source("webservice/WorkflowPreviewSessionContextGate.vb");

test("preview y actualización comparten la misma política de origen de sesión", () => {
  const previewContext = methodBlock(contextResolver, "AsegurarContexto");
  const executionContext = methodBlock(contextResolver, "AsegurarContextoEjecucion");

  assert.match(previewContext, /Not esSesionGestion AndAlso Not EsSesionWorkflowAutenticada\(requestContext\)/);
  assert.match(executionContext, /Not EsOrigenSesionPermitido\(requestContext\)/);
  assert.match(contextResolver, /EsSesionGestionAutenticada\(requestContext\) OrElse EsSesionWorkflowAutenticada\(requestContext\)/);
});

test("todas las actualizaciones de tarea pasan por el contexto autenticado compartido", () => {
  const expectations = new Map([
    ["EjecutarEnvioTarea", /AsegurarContextoEjecucion\(\)/],
    ["EjecutarEnvioUsuario", /AsegurarContextoEnvioUsuario\(True\)/],
    ["EjecutarEnvioGrupo", /AsegurarContextoEnvioGrupo\(True\)/],
    ["EjecutarDevolverActividad", /AsegurarContextoDevolverActividad\(True\)/],
    ["EjecutarDevolverUsuarioAnterior", /AsegurarContextoDevolverUsuarioAnterior\(True\)/]
  ]);

  for (const [name, expectedContext] of expectations) {
    assert.match(methodBlock(workflowService, name), expectedContext, `${name} usa una frontera distinta`);
  }
});

test("lecturas y actualizaciones de notas usan el mismo contexto", () => {
  for (const name of ["ListarNotas", "ConsultarNota", "ContarNotas", "CrearNota", "ActualizarNota", "EliminarNota"]) {
    assert.match(methodBlock(notesService, name), /AsegurarContextoNotas\(\)/, `${name} usa una frontera distinta`);
  }
});

test("lecturas y actualizaciones de importación usan el mismo constructor de contexto", () => {
  for (const name of [
    "ResolveCapabilities", "QueryItems", "GetPreview", "PreflightImport", "CreateImportIntent",
    "ExecuteImportIntent", "GetImportIntent", "ReconcileImportIntent"
  ]) {
    assert.match(methodBlock(importService, name), /TryBuildImportContext\(/, `${name} usa una frontera distinta`);
  }
});

test("las fronteras modernas no reintroducen configuración de rollout retirada", () => {
  const productionBoundary = [workflowService, notesService, importService, contextResolver].join("\n");
  assert.doesNotMatch(productionBoundary, /WorkflowCentroTrabajoModern(?:Active|OfficialMode|Users|Groups|ExcludedUsers|ExcludedGroups)/);
  assert.doesNotMatch(productionBoundary, /FEATURE_DISABLED/);
});

test("las especificaciones canónicas no exigen respuestas ni modos de rollout retirados", () => {
  const specsRoot = path.resolve(__dirname, "../openspec/specs");
  const canonicalSpecs = fs.readdirSync(specsRoot, { withFileTypes: true })
    .filter((entry) => entry.isDirectory())
    .map((entry) => path.join(specsRoot, entry.name, "spec.md"))
    .filter((file) => fs.existsSync(file))
    .map((file) => fs.readFileSync(file, "utf8"))
    .join("\n");

  assert.doesNotMatch(canonicalSpecs, /FEATURE_DISABLED|WorkflowCentroTrabajoModernOfficialMode/);
  assert.doesNotMatch(canonicalSpecs, /WORKFLOW_MODERN_(?:ACTIVE|INACTIVE|EXCLUDED|OFFICIAL_SCOPE_CONFLICT)/);
});
