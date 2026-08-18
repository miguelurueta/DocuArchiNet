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
const pageSource = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx.vb"), "utf8");
const pageMarkup = fs.readFileSync(path.resolve(__dirname, "../workflow/Webworkflow.aspx"), "utf8");
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

function functionBody(source, signature) {
    const start = source.indexOf(signature);
    assert.ok(start >= 0, `No se encontró ${signature}.`);
    const end = source.indexOf("    End Function", start);
    assert.ok(end > start, `No se encontró el cierre de ${signature}.`);
    return source.slice(start, end);
}

test("el gate exige alcance explícito y metadatos completos antes de habilitar el piloto", () => {
    const alcance = gateSource.indexOf("ModernoAlcancePilotoRequerido");
    const coincidencia = gateSource.indexOf("Not Contiene(usuarios, contexto.LoginUsuario)");
    const metadatos = gateSource.lastIndexOf("ModernoMetadatosPilotoInvalidos");

    assert.ok(alcance >= 0 && alcance < coincidencia, "El alcance vacío debe hacer fallback antes de evaluar inclusiones.");
    assert.ok(metadatos > coincidencia, "Los metadatos se validan solo después de incluir al contexto en el piloto.");
    assert.match(gateSource, /DateTime\.TryParseExact\(inicio,[\s\S]*?"yyyy-MM-ddTHH:mm:ssZ"/);
    assert.match(gateSource, /Not String\.IsNullOrWhiteSpace\(responsable\)[\s\S]*?Not String\.IsNullOrWhiteSpace\(motivo\)/);
    assert.match(gateSource, /Crear\("fallback-legacy", CodigosBloqueoPrevisualizacion\.ModernoAlcancePilotoRequerido, "La experiencia moderna no esta habilitada para este perfil\."\)/);
    assert.match(gateSource, /Crear\("fallback-legacy", CodigosBloqueoPrevisualizacion\.ModernoMetadatosPilotoInvalidos, "La experiencia moderna no esta habilitada para este perfil\."\)/);
});

test("la configuración oficial es explícita, conserva exclusiones y exige metadatos", () => {
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernActive"), "true");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernOfficialMode"), "true");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernUsers"), "");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernGroups"), "");
    assert.match(appSettingValue("WorkflowCentroTrabajoModernPilotStartUtc"), /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$/);
    assert.notEqual(appSettingValue("WorkflowCentroTrabajoModernPilotOwner"), "");
    assert.notEqual(appSettingValue("WorkflowCentroTrabajoModernPilotReason"), "");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernRollbackUtc"), "");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernRollbackOwner"), "");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernRollbackReason"), "");
    assert.equal(appSettingValue("WorkflowCentroTrabajoModernRollbackCorrelation"), "");
});

test("el DTO publica solo el estado seguro del gate y códigos estables", () => {
    const dtoBlock = dtoSource.match(/Public Class HabilitacionWorkflowModernDto[\s\S]*?End Class/)[0];

    assert.match(dtoBlock, /Public Property Estado As String/);
    assert.match(dtoBlock, /Public Property Codigo As String/);
    assert.match(dtoBlock, /Public Property MensajeFuncional As String/);
    assert.match(dtoBlock, /Public Property Activo As Boolean/);
    assert.doesNotMatch(dtoBlock, /Pilot|Usuarios|Grupos|Owner|Reason/i);
    assert.match(dtoSource, /Public Const ModernoAlcancePilotoRequerido As String = "WORKFLOW_MODERN_PILOT_SCOPE_REQUIRED"/);
    assert.match(dtoSource, /Public Const ModernoMetadatosPilotoInvalidos As String = "WORKFLOW_MODERN_PILOT_METADATA_INVALID"/);
    assert.match(dtoSource, /Public Const ModernoRollbackActivo As String = "WORKFLOW_MODERN_ROLLBACK_ACTIVE"/);
    assert.match(dtoSource, /Public Const ModernoAlcanceOficialInconsistente As String = "WORKFLOW_MODERN_OFFICIAL_SCOPE_CONFLICT"/);
});

test("Presentation obtiene la activación del bootstrap y no conserva el piloto visual paralelo", () => {
    assert.match(pageSource, /Public ReadOnly Property WorkflowCentroTrabajoModernActive As Boolean\s+Get\s+Return WorkflowTransitionModernActive\s+End Get\s+End Property/);
    assert.match(pageSource, /_workflowTransitionModernActive = WorkflowModernPresentationBootstrap\.EstaActivaParaSolicitudActual\(\)/);
    assert.doesNotMatch(pageSource, /WorkflowCentroTrabajoModernEnabled|WorkflowCentroTrabajoModernPilotProfiles|CurrentWorkflowPilotIsEnabled/);
    assert.match(pageMarkup, /<% If WorkflowCentroTrabajoModernActive Then %>\s+<link href="\.\.\/Styles\/workflow-centro-trabajo-moderno\.css/);
    assert.match(pageMarkup, /<% If WorkflowCentroTrabajoModernActive Then %>\s+<link[\s\S]*?centro-trabajo-visual\.js/);
});

test("preview y ejecución bloquean por el gate antes de consultar o invocar el flujo legado", () => {
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

test("el rollback es una operación explícita, conserva transiciones y deja el gate fail-closed", () => {
    assert.match(gateSource, /TieneMetadatosRollbackValidos\(\)[\s\S]*?CodigosBloqueoPrevisualizacion\.ModernoRollbackActivo/);
    assert.match(rollbackScriptSource, /SupportsShouldProcess = \$true, ConfirmImpact = "High"/);
    assert.match(rollbackScriptSource, /Set-AppSetting \$document \$settings "WorkflowCentroTrabajoModernActive" "false"/);
    assert.match(rollbackScriptSource, /Set-AppSetting \$document \$settings "WorkflowCentroTrabajoModernOfficialMode" "false"/);
    assert.match(rollbackScriptSource, /Set-AppSetting \$document \$settings "WorkflowCentroTrabajoModernUsers" ""/);
    assert.match(rollbackScriptSource, /Set-AppSetting \$document \$settings "WorkflowCentroTrabajoModernGroups" ""/);
    assert.match(rollbackScriptSource, /Set-AppSetting \$document \$settings "WorkflowCentroTrabajoModernRollbackCorrelation" \$Correlation\.Trim\(\)/);
    assert.match(rollbackScriptSource, /ReversionDeTransiciones = \$false/);
    assert.doesNotMatch(rollbackScriptSource, /Cambia_Estado|Terminar_Tarea_Workflow|INSERT\s|UPDATE\s|DELETE\s|Invoke-Sqlcmd/i);
});

test("el modo oficial requiere una activación explícita y no admite alcance piloto simultáneo", () => {
    assert.match(gateSource, /ClaveModoOficial As String = "WorkflowCentroTrabajoModernOfficialMode"/);
    assert.match(gateSource, /If EsBooleanoHabilitado\(Leer\(ClaveModoOficial\)\) Then[\s\S]*?ModernoAlcanceOficialInconsistente[\s\S]*?TieneMetadatosPilotoValidos\(\)[\s\S]*?Return Crear\("activo", "WORKFLOW_MODERN_ACTIVE"/);
});

test("la verificación aislada del gate cubre estados de piloto sin usar la configuración del ambiente", () => {
    for (const scenario of ["inactivo", "alcance-vacio", "metadatos-invalidos", "exclusion", "usuario-incluido", "grupo-incluido", "oficial", "oficial-con-alcance", "rollback"]) {
        assert.match(gateVerifierSource, new RegExp(`Invoke-GateScenario "${scenario}"`));
    }
    assert.match(gateVerifierSource, /\[IO\.Path\]::GetTempPath\(\)/);
    assert.doesNotMatch(gateVerifierSource, /Web\.config"\s*\)|IIS|localhost/i);
});

test("la verificación aislada de telemetría cubre resultados y persistencia fallida", () => {
    for (const expected of ["EXITO", "BLOQUEADO", "ERROR", "ThrowOnRegister", "WORKFLOW_MODERN_SUCCESS"]) {
        assert.match(telemetryVerifierSource, new RegExp(expected));
    }
    assert.doesNotMatch(telemetryVerifierSource, /MySqlConnection|HttpContext|Invoke-Sqlcmd|SELECT\s+.+\s+FROM/i);
});
