const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const source = fs.readFileSync(path.join(root, "Infrastructure/Workflow/ImportarServicioWeb/MySqlSiiImportAuthorizationRepository.vb"), "utf8");
const service = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx.vb"), "utf8");

test("conserva el único permiso exigido por la importación SII legacy", () => {
  assert.match(source, /ADJUNTAR_IMAGENES_PREDETERMINADA/);
  assert.match(source, /SELECT ADJUNTAR_IMAGENES_PREDETERMINADA FROM PERMISOS_USUARIO_WORKFLOW/);
  assert.match(source, /Return Enabled\(reader\("ADJUNTAR_IMAGENES_PREDETERMINADA"\)\)/);
  assert.doesNotMatch(source, /UTIL_SII_GETION_TAREA/);
});

test("valida tarea y ruta activas con consultas parametrizadas", () => {
  assert.match(source, /Inicio_Tareas_Workflow_id_Tarea=@idTarea/);
  assert.match(source, /ID_USUARIO=@idUsuario/);
  assert.match(source, /Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta=@idRuta/);
  assert.doesNotMatch(source, /WHERE[^"\r\n]*"\s*&/i);
});

test("falla cerrada ante contexto, proveedor o consulta invalidos", () => {
  assert.match(source, /Return SameContext\(contexto\) AndAlso/);
  assert.match(source, /SiiImportProvider\.CanonicalProviderId/);
  assert.match(source, /Catch\s+Return False/);
});

test("preflight conserva el diagnóstico seguro del contexto antes de evaluar permisos", () => {
  const preflight = service.match(/Public Function PreflightImport[\s\S]*?End Function/)?.[0] || "";
  assert.match(preflight, /Dim contextFailure As String = Nothing/);
  assert.match(preflight, /TryBuildImportContext\(request, context, session, contextFailure\)/);
  assert.match(preflight, /ErrorDto\(contextFailure\)/);
  assert.doesNotMatch(preflight, /TryBuildImportContext\(request, context, session\).*ErrorDto\("FORBIDDEN"\)/s);
});

test("resuelve el tramite faltante desde tarea y ruta confiables del servidor", () => {
  const service = fs.readFileSync(path.join(root, "webservice/WebServiceImportarServicioWebModern.asmx.vb"), "utf8");
  assert.match(service, /Solicita_id_tipo_tramite_tarea_workflow\(\s*trustedTaskId, result\.Contexto\.IdRutaWorkflow, routeName, trustedProcedureId\)/);
  assert.match(service, /SERVER_PROCEDURE_UNAVAILABLE/);
  assert.match(service, /SESSION_HTTP_CONTEXT_UNAVAILABLE/);
  assert.match(service, /SESSION_WORKFLOW_USER_UNAVAILABLE/);
  assert.match(service, /SESSION_WORKFLOW_GROUP_UNAVAILABLE/);
  assert.match(service, /SESSION_WORKFLOW_ROUTE_UNAVAILABLE/);
  assert.match(service, /SESSION_WORKFLOW_LOGIN_UNAVAILABLE/);
  assert.match(service, /SESSION_WORKFLOW_CONNECTION_UNAVAILABLE/);
  assert.match(service, /SESSION_CONTEXT_GATE_REJECTED/);
  assert.doesNotMatch(service, /request\.IdTramite|request\.ProcedureId/i);
});
