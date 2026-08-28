const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");

const gateSource = fs.readFileSync(
    path.resolve(__dirname, "../Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb"),
    "utf8"
);
const dtoSource = fs.readFileSync(
    path.resolve(__dirname, "../DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb"),
    "utf8"
);
const shellPageSource = fs.readFileSync(path.resolve(__dirname, "../Defaul/WebFormInicioDocuarchiGestion.aspx.vb"), "utf8");
const shellPageMarkup = fs.readFileSync(path.resolve(__dirname, "../Defaul/WebFormInicioDocuarchiGestion.aspx"), "utf8");
const pageSource = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx.vb"), "utf8");
const pageMarkup = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx"), "utf8");
const taskSelectionSource = fs.readFileSync(path.resolve(__dirname, "../workflow/Classselecciotarea.vb"), "utf8");
const serviceSource = fs.readFileSync(path.resolve(__dirname, "../Services/Workflow/Terminar/ServicioTransicionTarea.vb"), "utf8");
const asmxSource = fs.readFileSync(path.resolve(__dirname, "../webservice/WebServiceWorkflowModern.asmx.vb"), "utf8");
const auditModelSource = fs.readFileSync(path.resolve(__dirname, "../Modelo/Workflow/Terminar/WorkflowModernModels.vb"), "utf8");
const auditAdapterSource = fs.readFileSync(path.resolve(__dirname, "../Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb"), "utf8");
const pilotReportSource = fs.readFileSync(path.resolve(__dirname, "../tools/validation/Get-Doc14PilotReport.ps1"), "utf8");
const rollbackScriptSource = fs.readFileSync(path.resolve(__dirname, "../tools/validation/Invoke-Doc14Rollback.ps1"), "utf8");
const gateVerifierSource = fs.readFileSync(path.resolve(__dirname, "../tools/validation/Verify-Doc14PilotGate.ps1"), "utf8");
const telemetryVerifierSource = fs.readFileSync(path.resolve(__dirname, "../tools/validation/Verify-Doc14Telemetry.ps1"), "utf8");
const webConfig = fs.readFileSync(path.resolve(__dirname, "../Web.config"), "utf8");

function appSettingValue(key) {
    const escapedKey = key.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    const match = webConfig.match(new RegExp(`<add\\s+key="${escapedKey}"\\s+value="([^"]*)"\\s*/>`));
    assert.ok(match, `Falta la configuración ${key}.`);
    return match[1];
}

function hasAppSetting(key) {
    const escapedKey = key.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    return new RegExp(`<add\\s+key="${escapedKey}"\\s+value="[^"]*"\\s*/>`).test(webConfig);
}

function functionBody(source, signature) {
    const start = source.indexOf(signature);
    assert.ok(start >= 0, `No se encontró ${signature}.`);
    const end = source.indexOf("    End Function", start);
    assert.ok(end > start, `No se encontró el cierre de ${signature}.`);
    return source.slice(start, end);
}

test("la política oficial habilita todo contexto Workflow válido sin leer configuración", () => {
    assert.match(gateSource, /If contexto Is Nothing OrElse Not contexto\.EsValido\(\) Then[\s\S]*?WORKFLOW_CONTEXT_INVALID/);
    assert.match(gateSource, /Return Crear\("activo", "WORKFLOW_MODERN_OFFICIAL", "La experiencia moderna esta habilitada\."\)/);
    assert.doesNotMatch(gateSource, /ConfigurationManager|AppSettings|WorkflowCentroTrabajoModern|Pilot|Rollback|Excluded|Contiene\(/);
});

test("la configuración local conserva el registro inactivo sin gobernar la política oficial", () => {
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernActive"), "false");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernOfficialMode"), "false");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernUsers"), "");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernGroups"), "");
    assert.ok(hasAppSetting("WorkflowCentroTrabajoModernLayers"));
    assert.equal(hasAppSetting("WorkflowCentroTrabajoModernEnabled"), false);
    assert.equal(hasAppSetting("WorkflowCentroTrabajoModernPilotProfiles"), false);
});

test("el DTO publica el estado oficial y conserva los códigos de negocio", () => {
    const dtoBlock = dtoSource.match(/Public Class HabilitacionWorkflowModernDto[\s\S]*?End Class/)[0];

    assert.match(dtoBlock, /Public Property Estado As String/);
    assert.match(dtoBlock, /Public Property Codigo As String/);
    assert.match(dtoBlock, /Public Property MensajeFuncional As String/);
    assert.match(dtoBlock, /Public Property Activo As Boolean/);
    assert.doesNotMatch(dtoBlock, /Pilot|Usuarios|Grupos|Owner|Reason/i);
    assert.match(dtoSource, /Public Const ContextoInvalido As String = "WORKFLOW_CONTEXT_INVALID"/);
    assert.doesNotMatch(dtoSource, /WORKFLOW_MODERN_(?:INACTIVE|PILOT|ROLLBACK|OFFICIAL_SCOPE|EXCLUDED)/);
});

test("la presentación es constante y el contexto conserva únicamente los bootstraps operativos", () => {
    assert.match(pageSource, /Public ReadOnly Property WorkflowCentroTrabajoModernActive As Boolean\s+Get\s+Return WorkflowTransitionModernActive\s+End Get\s+End Property/);
    assert.match(pageSource, /Public ReadOnly Property WorkflowCentroTrabajoModernPresentationEnabled As Boolean\s+Get\s+Return True\s+End Get\s+End Property/);
    assert.match(pageSource, /_workflowTransitionModernActive = WorkflowModernPresentationBootstrap\.EstaActivaParaSolicitudActual\(\)/);
    assert.doesNotMatch(pageSource, /WorkflowCentroTrabajoModernEnabled|WorkflowCentroTrabajoModernPilotProfiles|CurrentWorkflowPilotIsEnabled/);
    assert.match(pageMarkup, /<link href="\.\.\/Styles\/workflow-centro-trabajo-moderno\.css\?v=20260820-modern-actions3/);
    assert.match(pageMarkup, /<script src="\.\.\/js\/workflow\/centro-trabajo-visual\.js\?v=20260821-modern-actions4/);
    assert.doesNotMatch(pageMarkup, /<% If WorkflowCentroTrabajoModernActive Then %>\s+<link href="\.\.\/Styles\/workflow-centro-trabajo-moderno\.css/);
    assert.match(pageSource, /RegisterWorkflowTransitionModernStyle\(\)\s+RegisterWorkflowTransitionPagePresentationScript\(\)\s+RegisterWorkflowEnvioUsuarioModernPresentation\(\)\s+RegisterWorkflowReturnActivityModernPresentation\(\)\s+RegisterWorkflowReturnUserPreviousModernPresentation\(\)\s+If Not WorkflowTransitionModernActive Then\s+Return\s+End If\s+RegisterConfirmationDialogStyle\(\)/);
    assert.match(pageSource, /If Not WorkflowTransitionModernActive Then[\s\S]*?Return[\s\S]*?RegisterWorkflowTransitionModernBootstrap\(\)[\s\S]*?RegisterWorkflowEnvioGrupoModernBootstrap\(\)/);
    assert.match(pageSource, /Private Sub RegisterWorkflowEnvioUsuarioModernPresentation\(\)[\s\S]*?RegisterWorkflowEnvioUsuarioModernBootstrap\(\)[\s\S]*?End Sub/);
    assert.match(pageSource, /Private Sub RegisterWorkflowReturnActivityModernPresentation\(\)[\s\S]*?RegisterWorkflowReturnActivityModernBootstrap\(\)[\s\S]*?End Sub/);
    assert.match(pageSource, /Private Sub RegisterWorkflowReturnUserPreviousModernPresentation\(\)[\s\S]*?RegisterWorkflowReturnUserPreviousModernBootstrap\(\)[\s\S]*?End Sub/);
    assert.match(pageSource, /workflow-user-send-ui\.js\?v=20260821-doc29ui1/);
    assert.match(pageSource, /workflow-return-activity-ui\.js\?v=20260827-doc33rebind1/);
    assert.match(pageSource, /workflow-return-user-previous-ui\.js\?v=20260827-doc37rebind1/);
    assert.match(pageSource, /data-workflow-user-send-active/);
    assert.match(pageSource, /workflowCentroTrabajoModernViewport\.Visible = WorkflowCentroTrabajoModernPresentationEnabled/);
    assert.match(pageSource, /If Not WorkflowCentroTrabajoModernPresentationEnabled Then\s+Return String\.Empty/);
    assert.match(taskSelectionSource, /WorkflowCentroTrabajoModernPresentationEnabled/);
});

test("continuar y enviar a grupo son controles modernos únicos sin respaldo legacy", () => {
    assert.match(pageSource, /Public ReadOnly Property WorkflowCentroTrabajoModernOperationDisabledAttribute As String\s+Get\s+If WorkflowCentroTrabajoModernActive Then\s+Return " aria-disabled=""false"""\s+End If\s+Return " disabled=""disabled"" aria-disabled=""true"""/);
    assert.match(pageMarkup, /<button id="workflow-group-send-trigger" type="button"[^>]*?WorkflowCentroTrabajoModernOperationDisabledAttribute[^>]*?>/);
    assert.match(pageMarkup, /<button id="workflow-transition-trigger" type="button"[^>]*?WorkflowCentroTrabajoModernOperationDisabledAttribute[^>]*?>/);
    assert.doesNotMatch(pageMarkup, /WorkflowCentroTrabajoModernActive/);
    assert.doesNotMatch(pageMarkup, /activa_boton_client_server\('ImageButton(?:EnviaActividad|terminar)'\)/);
    assert.doesNotMatch(pageMarkup, /id="workflow-(?:group-send|transition)-trigger"[^>]*\sonclick=/);
    assert.match(pageMarkup, /<button id="workflow-user-send-trigger" type="button"[^>]*?>/);
    assert.doesNotMatch(pageMarkup, /workflow-user-send-trigger[^>]*WorkflowCentroTrabajoModernOperationDisabledAttribute/);
    assert.doesNotMatch(pageMarkup, /ImageButtonEnviarUsuario|Button_tool_enviar_usuario|ModalPopupExtender_edition_lista_usuarios_ruta/);
});

test("el host inicial conserva un viewport pasivo sin gate ni perfil histórico", () => {
    assert.match(shellPageMarkup, /<meta id="workflowCentroTrabajoModernShellViewport" runat="server" name="viewport" content="width=device-width, initial-scale=1" visible="true"\s*\/>/);
    assert.doesNotMatch(shellPageSource, /WorkflowCentroTrabajoModern(?:Enabled|PilotProfiles|ShellActive)/);
    assert.doesNotMatch(shellPageSource, /GA_LOGINUSUARIOGESTION/);
    assert.doesNotMatch(shellPageSource, /ConfigurationManager\.AppSettings/);
    assert.doesNotMatch(shellPageSource, /WorkflowModernPresentationBootstrap|IWorkflowModernFeatureGate|WebServiceWorkflowModern/);
});

test("preview y ejecución validan contexto y negocio antes de consultar o invocar el flujo legado", () => {
    const preview = functionBody(serviceSource, "Public Function Previsualizar");
    const ejecucion = functionBody(serviceSource, "Public Function Ejecutar");

    assert.ok(preview.indexOf("Dim habilitacion As HabilitacionWorkflowModernDto = EvaluarHabilitacion(contexto)") < preview.indexOf("Dim tarea As TareaWorkflow"));
    assert.ok(preview.indexOf("If Not habilitacion.Activo Then") < preview.indexOf("Dim tarea As TareaWorkflow"));
    assert.ok(ejecucion.indexOf("Dim habilitacion As HabilitacionWorkflowModernDto = EvaluarHabilitacion(contexto)") < ejecucion.indexOf("Dim errorSolicitud As ErrorTransicionDto"));
    assert.ok(ejecucion.indexOf("If Not habilitacion.Activo Then") < ejecucion.indexOf("_concurrencyGuard.Adquirir"));
    assert.ok(ejecucion.indexOf("If Not habilitacion.Activo Then") < ejecucion.indexOf("_ejecutor.Ejecutar"));
    assert.match(asmxSource, /Public Function PreviewEnviarTarea[\s\S]*?New ConfiguracionWorkflowModernFeatureGate\(\)[\s\S]*?Return servicio\.Previsualizar/);
    assert.match(asmxSource, /Public Function EjecutarEnvioTarea[\s\S]*?New ConfiguracionWorkflowModernFeatureGate\(\)[\s\S]*?Return servicio\.Ejecutar/);
});

test("la auditoría moderna conserva solo el contrato mínimo sanitizado", () => {
    const auditBlock = auditModelSource.match(/Public Class AuditoriaTransicion[\s\S]*?End Class/)[0];

    for (const property of [
        "IdTarea", "IdUsuarioWorkflow", "IdRutaWorkflow", "IdFlujoTrabajo", "IdActividadOrigen",
        "IdActividadDestino", "IdConector", "Canal", "FechaUtc", "DuracionMilisegundos",
        "Resultado", "CodigoFuncional", "Referencia"
    ]) {
        assert.match(auditBlock, new RegExp(`Public Property ${property} `));
    }
    assert.doesNotMatch(auditBlock, /Login|Sql|Session|Token|Documento|Payload|Password|Clave/i);
    assert.match(auditAdapterSource, /Canal=\{1\}; Usuario=\{2\}; Tarea=\{3\}; Ruta=\{4\}; Flujo=\{5\}; Origen=\{6\}; Destino=\{7\}; Conector=\{8\}; DuracionMs=\{9\}; Resultado=\{10\}; Codigo=\{11\}/);
    assert.match(auditAdapterSource, /NormalizarCanal\(auditoria\.Canal\)/);
    assert.match(auditAdapterSource, /NormalizarResultado\(auditoria\.Resultado\)/);
    assert.match(auditAdapterSource, /NormalizarCodigo\(auditoria\.CodigoFuncional\)/);
});

test("una falla de auditoría agrega advertencia sin reemplazar el resultado funcional", () => {
    const execute = functionBody(serviceSource, "Public Function Ejecutar");
    const register = functionBody(serviceSource, "Private Function RegistrarAuditoria");

    assert.match(execute, /Dim cronometro As Stopwatch = Stopwatch\.StartNew\(\)/);
    assert.doesNotMatch(execute, /_ejecutor Is Nothing OrElse _auditoriaRepository Is Nothing/);
    assert.match(register, /\.DuracionMilisegundos = Math\.Max\(0, duracionMilisegundos\)/);
    assert.match(register, /\.Resultado = ResolverResultadoAuditoria\(respuesta\)/);
    assert.match(register, /\.CodigoFuncional = ResolverCodigoAuditoria\(respuesta\)/);
    assert.match(register, /Try[\s\S]*?_auditoriaRepository\.Registrar\(auditoria\)[\s\S]*?Catch[\s\S]*?AgregarAdvertenciaAuditoria\(respuesta\)/);
    assert.match(serviceSource, /If respuesta\.Advertencias Is Nothing Then respuesta\.Advertencias = New List\(Of String\)\(\)/);
});

test("el reporte DOC-14 agrega métricas por canal sin conectarse ni exponer datos personales", () => {
    assert.match(pilotReportSource, /\[string\]\$InputPath/);
    assert.match(pilotReportSource, /Group-Object -Property Canal/);
    assert.match(pilotReportSource, /Volumen = \$items\.Count/);
    assert.match(pilotReportSource, /Exitos =/);
    assert.match(pilotReportSource, /Bloqueos =/);
    assert.match(pilotReportSource, /Errores =/);
    assert.match(pilotReportSource, /DuracionP95Ms/);
    assert.match(pilotReportSource, /Abandonos =/);
    assert.match(pilotReportSource, /Divergencias =/);
    assert.match(pilotReportSource, /EstadoPromocion = if \(\$critical/);
    assert.doesNotMatch(pilotReportSource, /MySqlConnection|Invoke-Sqlcmd|SELECT\s+.+\s+FROM|HttpContext|ConnectionString|Password|IdUsuario/i);
});

test("la reversión se hace por paquete y no modifica la configuración del gate", () => {
    assert.match(rollbackScriptSource, /rollback por gate DOC-14 fue retirado/i);
    assert.match(rollbackScriptSource, /Restaure el paquete/i);
    assert.doesNotMatch(rollbackScriptSource, /Set-AppSetting|Web\.config|Copy-Item|Cambia_Estado|Terminar_Tarea_Workflow|INSERT\s|UPDATE\s|DELETE\s|Invoke-Sqlcmd/i);
});

test("la política oficial no conserva alcance, exclusiones ni rollback de despliegue", () => {
    assert.match(gateSource, /WORKFLOW_MODERN_OFFICIAL/);
    assert.doesNotMatch(gateSource, /ModoOficial|Usuario|Grupo|Piloto|Exclu|Rollback|AppSettings/i);
});

test("la verificación aislada cubre la política oficial sin usar configuración del ambiente", () => {
    for (const scenario of ["contexto-valido", "contexto-invalido"]) {
        assert.match(gateVerifierSource, new RegExp(`Invoke-PolicyScenario "${scenario}"`));
    }
    assert.match(gateVerifierSource, /\[IO\.Path\]::GetTempPath\(\)/);
    assert.doesNotMatch(gateVerifierSource, /WorkflowCentroTrabajoModern|Web\.config|IIS|localhost/i);
});

test("la verificación aislada de telemetría cubre resultados y persistencia fallida", () => {
    for (const expected of ["EXITO", "BLOQUEADO", "ERROR", "ThrowOnRegister", "WORKFLOW_MODERN_SUCCESS"]) {
        assert.match(telemetryVerifierSource, new RegExp(expected));
    }
    assert.doesNotMatch(telemetryVerifierSource, /MySqlConnection|HttpContext|Invoke-Sqlcmd|SELECT\s+.+\s+FROM/i);
});
