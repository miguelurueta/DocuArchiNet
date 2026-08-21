const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");

const modelsSource = fs.readFileSync(path.resolve(__dirname, "../Modelo/Workflow/Terminar/WorkflowModernModels.vb"), "utf8");
const interfacesSource = fs.readFileSync(path.resolve(__dirname, "../Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb"), "utf8");
const dtoSource = fs.readFileSync(path.resolve(__dirname, "../DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb"), "utf8");
const contextGateSource = fs.readFileSync(path.resolve(__dirname, "../webservice/WorkflowPreviewSessionContextGate.vb"), "utf8");
const repositorySource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Repositories/Workflow/MySqlEnvioUsuarioRepository.vb"), "utf8");
const validatorSource = fs.readFileSync(path.resolve(__dirname, "../Services/Workflow/Terminar/ValidadorEnvioUsuarioTarea.vb"), "utf8");
const serviceSource = fs.readFileSync(path.resolve(__dirname, "../Services/Workflow/Terminar/ServicioEnvioUsuarioTarea.vb"), "utf8");
const requirementsSource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuarioRequisitosAdapter.vb"), "utf8");
const executorSource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuarioExecutorAdapter.vb"), "utf8");
const authorizationSource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuarioAutorizacionAdapter.vb"), "utf8");
const auditSource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb"), "utf8");
const asmxSource = fs.readFileSync(path.resolve(__dirname, "../webservice/WebServiceWorkflowModern.asmx.vb"), "utf8");
const projectSource = fs.readFileSync(path.resolve(__dirname, "../GestionDocumental-Docuarchi.net.vbproj"), "utf8");

function classBlock(source, name) {
    const match = source.match(new RegExp(`Public Class ${name}[\\s\\S]*?End Class`));
    assert.ok(match, `falta ${name}`);
    return match[0];
}

test("los contratos exclusivos de enviar a usuario no exponen conector", () => {
    for (const name of [
        "SolicitudPreviewEnvioUsuario",
        "SolicitudEnvioUsuarioWorkflow",
        "DestinoEnvioUsuarioWorkflow",
        "ResultadoBusquedaDestinosEnvioUsuario",
        "ResultadoResolucionEnvioUsuario",
        "ResultadoRequisitosEnvioUsuario"
    ]) {
        assert.doesNotMatch(classBlock(modelsSource, name), /IdConector|Page|Session/);
    }
    for (const name of ["DestinoEnvioUsuarioDto", "PrevisualizacionEnvioUsuarioDto", "ResultadoEnvioUsuarioDto"]) {
        assert.doesNotMatch(classBlock(dtoSource, name), /IdConector|Page|Session/);
    }
    assert.match(interfacesSource, /Interface IEnvioUsuarioBusquedaRepository/);
    assert.match(interfacesSource, /Interface IEnvioUsuarioEjecucionRepository/);
    assert.match(interfacesSource, /Interface IEnvioUsuarioRequisitosRepository/);
    assert.match(interfacesSource, /Interface IEnvioUsuarioAutorizacionRepository/);
    assert.match(interfacesSource, /Interface IEnvioUsuarioLegacyExecutor/);
    assert.match(dtoSource, /PermisoCambioUsuarioDenegado As String = "WORKFLOW_USER_SEND_FORBIDDEN"/);
    assert.match(classBlock(modelsSource, "SolicitudTransicionWorkflow"), /IdConector/);
});

test("el proyecto incluye todos los tipos exclusivos de enviar a usuario", () => {
    for (const file of [
        "Services\\Workflow\\Terminar\\ServicioEnvioUsuarioTarea.vb",
        "Services\\Workflow\\Terminar\\ValidadorEnvioUsuarioTarea.vb",
        "Infrastructure\\Repositories\\Workflow\\MySqlEnvioUsuarioRepository.vb",
        "Infrastructure\\Workflow\\Terminar\\WorkflowLegacyEnvioUsuarioRequisitosAdapter.vb",
        "Infrastructure\\Workflow\\Terminar\\WorkflowLegacyEnvioUsuarioAutorizacionAdapter.vb",
        "Infrastructure\\Workflow\\Terminar\\WorkflowLegacyEnvioUsuarioExecutorAdapter.vb"
    ]) {
        assert.ok(projectSource.includes(`<Compile Include="${file}" />`), `falta incluir ${file} en el proyecto.`);
    }
});

test("el contexto de enviar a usuario calcula CAMBIO_USUARIO en servidor y falla cerrado", () => {
    const block = contextGateSource.match(/Public Function AsegurarContextoEnvioUsuario[\s\S]*?End Function/)[0];

    assert.match(block, /SolicitaPermisosUsuarioWorkflow/);
    assert.match(block, /permisos\.Length > 18/);
    assert.match(block, /permisos\(18\)/);
    assert.match(block, /PuedeCambioUsuario = False/);
    assert.doesNotMatch(block, /IWorkflowModernFeatureGate|WorkflowCentroTrabajoModernActive/);
});

test("el preview de usuario filtra y pagina destinos autorizados mediante solo lecturas", () => {
    assert.match(repositorySource, /Implements IEnvioUsuarioBusquedaRepository, IEnvioUsuarioEjecucionRepository/);
    assert.match(repositorySource, /usuario\.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA = @idRuta/);
    assert.match(repositorySource, /usuario\.ESTADO_USUARIO = 1 AND usuario\.UTIL_ASIGNA_TAREA = 1/);
    assert.match(repositorySource, /usuario\.NOMBRE_USUARIO LIKE CONCAT\('%', @consulta, '%'/);
    assert.match(repositorySource, /ORDER BY usuario\.NOMBRE_USUARIO, usuario\.IDU_SUARIO, grupo\.ID_ACTIVIDAD LIMIT @limite/);
    assert.match(repositorySource, /MachineKey\.Protect/);
    assert.match(repositorySource, /MachineKey\.Unprotect/);
    assert.match(repositorySource, /CursorUsuarioInvalido/);
    assert.match(repositorySource, /SELECT estado_ruta_open_close/);
    assert.match(repositorySource, /SELECT TIPO_RUTA_ABIERTA_CERRADA/);
    assert.doesNotMatch(repositorySource, /\b(?:INSERT|UPDATE|DELETE|CALL)\b/i);
});

test("el preview valida intención, permiso y tarea sin gate ni límite mutante", () => {
    const preview = serviceSource.match(/Public Function Previsualizar[\s\S]*?End Function/)[0];

    assert.match(validatorSource, /NormalizarPreview/);
    assert.match(validatorSource, /TamanoPaginaPredeterminado As Integer = 25/);
    assert.match(validatorSource, /TamanoPaginaMaximo As Integer = 50/);
    assert.match(validatorSource, /BusquedaUsuarioTerminoInvalido/);
    assert.match(validatorSource, /CursorUsuarioInvalido/);
    assert.match(preview, /_validadorSolicitud\.NormalizarPreview/);
    assert.match(preview, /TieneCambioUsuario/);
    assert.match(preview, /_tareaRepository\.ObtenerTarea/);
    assert.match(preview, /_busquedaRepository\.BuscarDestinos/);
    assert.doesNotMatch(preview, /IWorkflowModernFeatureGate|IEnvioUsuarioLegacyExecutor|IAuditoriaTransicionRepository|Terminar_Tarea_Workflow/);
});

test("el ASMX publica PreviewEnviarUsuario con contrato paginado y sin motor legacy", () => {
    const block = asmxSource.match(/Public Function PreviewEnviarUsuario[\s\S]*?End Function/)[0];

    assert.match(block, /ByVal idTarea As Long/);
    assert.match(block, /ByVal consulta As String/);
    assert.match(block, /ByVal cursor As String/);
    assert.match(block, /ByVal tamanoPagina As Integer/);
    assert.match(block, /AsegurarContextoEnvioUsuario\(\)/);
    assert.match(block, /New MySqlEnvioUsuarioRepository/);
    assert.match(block, /New SolicitudPreviewEnvioUsuario/);
    assert.doesNotMatch(block, /IdConector|Terminar_Tarea_Workflow|IWorkflowModernFeatureGate/);
});

test("la ejecución reautoriza y revalida el destino bajo el lock antes del adaptador mutante", () => {
    const guardPosition = serviceSource.indexOf("Using guard.Lease");
    const permissionPosition = serviceSource.indexOf("_autorizacionRepository.TieneCambioUsuario", guardPosition);
    const taskPosition = serviceSource.indexOf("_tareaRepository.ObtenerTarea", guardPosition);
    const destinationPosition = serviceSource.indexOf("_ejecucionRepository.ResolverDestino", guardPosition);
    const requirementsPosition = serviceSource.indexOf("_requisitosRepository.Evaluar", guardPosition);
    const executorPosition = serviceSource.indexOf("_ejecutor.Ejecutar", guardPosition);

    assert.ok(guardPosition >= 0 && permissionPosition > guardPosition && taskPosition > permissionPosition);
    assert.ok(destinationPosition > taskPosition && requirementsPosition > destinationPosition && executorPosition > requirementsPosition);
    assert.match(repositorySource, /ResolverDestino[\s\S]*?usuario\.ESTADO_USUARIO = 1 AND usuario\.UTIL_ASIGNA_TAREA = 1/);
    assert.match(serviceSource, /VersionConflicto/);
    assert.match(authorizationSource, /SolicitaPermisosUsuarioWorkflow/);
    assert.match(authorizationSource, /permisos\.Length > 18/);
    assert.match(authorizationSource, /permisos\(18\)/);
});

test("la política de respuesta bloquea sin reasignar y el motor recibe el destino directo", () => {
    assert.match(requirementsSource, /Verifica_respuesta_radicado_sin_respuesta/);
    assert.match(requirementsSource, /String\.Equals\(resultadoRespuesta, "YES"/);
    assert.doesNotMatch(requirementsSource, /After_envio_usuario_workflow|Reasigna_respuesta_envia_tarea_usuario|Cambia_Estado|HttpContext/);

    assert.match(executorSource, /Dim pagina As System\.Web\.UI\.Page = Nothing/);
    assert.equal((executorSource.match(/Terminar_Tarea_Workflow/g) || []).length, 1);
    assert.match(executorSource, /destino\.IdUsuarioWorkflowDestino\.ToString/);
    assert.match(executorSource, /destino\.IdActividadDestino\.ToString/);
    assert.match(executorSource, /If\(destino\.RequiereNotificacion, 1, 0\),\r?\n\s*0,\r?\n\s*contexto\.IdUsuarioWorkflow/);
    assert.doesNotMatch(executorSource, /After_envio_usuario_workflow|Reasigna_respuesta_envia_tarea_usuario|Cambia_Estado|HttpContext|IdConector/);
});

test("la auditoría identifica ASMX_ENVIO_USUARIO y una falla agrega advertencia", () => {
    assert.match(serviceSource, /\.Mecanismo = "ASMX_ENVIO_USUARIO"/);
    assert.match(serviceSource, /AgregarAdvertenciaAuditoria/);
    assert.match(auditSource, /ASMX_ENVIO_USUARIO/);
    assert.match(auditSource, /NormalizarMecanismo/);
    assert.doesNotMatch(serviceSource, /IWorkflowModernFeatureGate|IWorkflowLegacyExecutor/);
});

test("el ASMX ejecuta el contrato directo sin invocar el motor en la capa de transporte", () => {
    const block = asmxSource.match(/Public Function EjecutarEnvioUsuario[\s\S]*?End Function/)[0];

    assert.match(block, /ByVal idTarea As Long/);
    assert.match(block, /ByVal idUsuarioWorkflowDestino As Integer/);
    assert.match(block, /ByVal idActividadDestino As Integer/);
    assert.match(block, /ByVal tokenVersion As String/);
    assert.match(block, /AsegurarContextoEnvioUsuario\(True\)/);
    assert.match(block, /New MySqlTransicionConcurrencyGuard/);
    assert.match(block, /New WorkflowLegacyEnvioUsuarioRequisitosAdapter/);
    assert.match(block, /New WorkflowLegacyEnvioUsuarioAutorizacionAdapter/);
    assert.match(block, /New WorkflowLegacyEnvioUsuarioExecutorAdapter/);
    assert.match(block, /New SolicitudEnvioUsuarioWorkflow/);
    assert.doesNotMatch(block, /IdConector|Terminar_Tarea_Workflow|IWorkflowModernFeatureGate/);
});
