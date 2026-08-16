[CmdletBinding()]
param(
    [string]$AssemblyPath,
    [string]$SourceRoot
)

$ErrorActionPreference = "Stop"
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path

if ([string]::IsNullOrWhiteSpace($SourceRoot)) {
    $SourceRoot = Join-Path $scriptDirectory "..\.."
}
if ([string]::IsNullOrWhiteSpace($AssemblyPath)) {
    $AssemblyPath = Join-Path $SourceRoot "bin\GestionDocumental-Docuarchi.net.dll"
}
if (-not (Test-Path -LiteralPath $AssemblyPath)) {
    throw "No existe el ensamblado compilado: $AssemblyPath"
}

$requiredFiles = @(
    (Join-Path $SourceRoot "webservice\WebServiceWorkflowModern.asmx"),
    (Join-Path $SourceRoot "webservice\WebServiceWorkflowModern.asmx.vb"),
    (Join-Path $SourceRoot "Infrastructure\Repositories\Workflow\MySqlWorkflowPreviewRepositories.vb"),
    (Join-Path $SourceRoot "Infrastructure\Shared\Data\AdoNetDataInfrastructure.vb"),
    (Join-Path $SourceRoot "Infrastructure\Shared\Data\WorkflowModuleConnectionFactory.vb"),
    (Join-Path $SourceRoot "DTOs\Workflow\Terminar\TransicionWorkflowDtos.vb"),
    (Join-Path $SourceRoot "Services\Workflow\Terminar\ServicioTransicionTarea.vb"),
    (Join-Path $SourceRoot "webservice\WorkflowPreviewSessionContextGate.vb")
)
foreach ($requiredFile in $requiredFiles) {
    if (-not (Test-Path -LiteralPath $requiredFile)) {
        throw "Falta un artefacto requerido por DOC-10: $requiredFile"
    }
}

$assembly = [System.Reflection.Assembly]::LoadFrom((Resolve-Path -LiteralPath $AssemblyPath))
$endpointType = $assembly.GetType("GestionDocumental_Docuarchi.net.WebServiceWorkflowModern", $true)
$previewType = $assembly.GetType("GestionDocumental_Docuarchi.net.PrevisualizacionTransicionDto", $true)
$serviceType = $assembly.GetType("GestionDocumental_Docuarchi.net.ServicioTransicionTarea", $true)
$gateType = $assembly.GetType("GestionDocumental_Docuarchi.net.ConfiguracionWorkflowModernFeatureGate", $true)
$contextType = $assembly.GetType("GestionDocumental_Docuarchi.net.ContextoModuloWorkflow", $true)
$sessionGateType = $assembly.GetType("GestionDocumental_Docuarchi.net.WorkflowPreviewSessionContextGate", $true)
$moduleConnectionFactoryType = $assembly.GetType("GestionDocumental_Docuarchi.net.WorkflowModuleConnectionFactory", $true)
$docuarchiModuleConnectionFactoryType = $assembly.GetType("GestionDocumental_Docuarchi.net.DocuarchiModuleConnectionFactory", $true)

$previewMethod = $endpointType.GetMethod("PreviewEnviarTarea")
if ($null -eq $previewMethod -or $previewMethod.ReturnType -ne $previewType) {
    throw "El ASMX no expone PreviewEnviarTarea con el DTO seguro esperado."
}
if ($previewMethod.GetParameters().Count -ne 1 -or $previewMethod.GetParameters()[0].ParameterType -ne [Int64]) {
    throw "PreviewEnviarTarea debe aceptar exclusivamente idTarea As Long."
}
if (-not $moduleConnectionFactoryType.GetInterfaces().Name.Contains("IModuleConnectionFactory")) {
    throw "La conexión del módulo Workflow no conserva el contrato de infraestructura compartida."
}
if (-not $docuarchiModuleConnectionFactoryType.GetInterfaces().Name.Contains("IModuleConnectionFactory")) {
    throw "La conexión del módulo Docuarchi no conserva el contrato de infraestructura compartida."
}

Add-Type -AssemblyName System.Web

function New-Doc10SessionContext {
    param([hashtable]$Values)

    $request = [System.Web.HttpRequest]::new("", "http://localhost/doc10", "")
    $response = [System.Web.HttpResponse]::new([System.IO.StringWriter]::new())
    $httpContext = [System.Web.HttpContext]::new($request, $response)
    $sessionContainer = [System.Web.SessionState.HttpSessionStateContainer]::new(
        "doc10-verification",
        [System.Web.SessionState.SessionStateItemCollection]::new(),
        [System.Web.HttpStaticObjectsCollection]::new(),
        20,
        $true,
        [System.Web.HttpCookieMode]::UseCookies,
        [System.Web.SessionState.SessionStateMode]::InProc,
        $false)
    [System.Web.SessionState.SessionStateUtility]::AddHttpSessionStateToContext($httpContext, $sessionContainer)
    foreach ($entry in $Values.GetEnumerator()) {
        $httpContext.Session.Item($entry.Key) = $entry.Value
    }
    return $httpContext
}

$sessionGate = [Activator]::CreateInstance($sessionGateType)
[System.Web.HttpContext]::Current = New-Doc10SessionContext @{}
$anonymousSessionResult = $sessionGateType.GetMethod("AsegurarContexto").Invoke($sessionGate, @())
if ($anonymousSessionResult.Contexto.EsValido()) {
    throw "El gate de sesión acepta una solicitud anónima."
}

[System.Web.HttpContext]::Current = New-Doc10SessionContext @{
    "Id_Usuario_Workflow" = "63"
    "Id_Grupo_Workflow" = "9"
    "Id_Ruta_Workflow" = "7"
    "Login_Usuario_Workfow" = "doc10-static-test"
    "IP_SERVER_MODULO" = "localhost"
    "DB_NAME_MODULO" = "workflow-test"
    "USER_DBMS_MODULO" = "readonly"
    "PASW_DBMS_MODULO" = "not-used"
    "TYPE_DBMS_MODULO" = "mysql"
    "ACTIVA_POOL_DBMS" = "1"
    "NUMERO_DBMS_CONEX" = "5"
    "DA_IP_SERVER_MODULO" = "localhost"
    "DA_DB_NAME_MODULO" = "docuarchi-test"
    "DA_USER_DBMS_MODULO" = "readonly"
    "DA_PASW_DBMS_MODULO" = "not-used"
    "DA_TYPE_DBMS_MODULO" = "mysql"
    "DA_ACTIVA_POOL_DBMS" = "1"
    "DA_NUMERO_DBMS_CONEX" = "5"
}
$existingSessionResult = $sessionGateType.GetMethod("AsegurarContexto").Invoke($sessionGate, @())
if (-not $existingSessionResult.Contexto.EsValido() -or
    $existingSessionResult.Contexto.IdUsuarioWorkflow -ne 63 -or
    $existingSessionResult.Contexto.IdGrupoWorkflow -ne 9 -or
    [string]::IsNullOrWhiteSpace($existingSessionResult.CadenaConexionWorkflow)) {
    throw "El gate de sesión no preserva el contexto Workflow ni compone su conexión."
}
[System.Web.HttpContext]::Current = $null

$context = [Activator]::CreateInstance($contextType)
$context.IdUsuarioWorkflow = 1
$context.IdGrupoWorkflow = 1
$context.IdRutaWorkflow = 1
$context.LoginUsuario = "doc10-static-test"
$gate = [Activator]::CreateInstance($gateType)
$gateResult = $gateType.GetMethod("Evaluar").Invoke($gate, @($context))
if ($gateResult.Activo -or $gateResult.Codigo -ne "WORKFLOW_MODERN_INACTIVE") {
    throw "El gate DOC-10 no permanece cerrado por defecto: $($gateResult.Codigo)."
}

$previewConstructor = $serviceType.GetConstructors() | Where-Object { $_.GetParameters().Count -eq 5 } | Select-Object -First 1
if ($null -eq $previewConstructor) {
    throw "El caso de uso de preview conserva una composicion obligatoria de escritura."
}
$previewService = $previewConstructor.Invoke(@($null, $null, $null, $gate, [Activator]::CreateInstance($assembly.GetType("GestionDocumental_Docuarchi.net.ValidadorTransicionTarea", $true))))
$previewResult = $serviceType.GetMethod("Previsualizar").Invoke($previewService, @($context, [Int64]1))
if ($previewResult.Error.Codigo -ne "WORKFLOW_MODERN_INACTIVE" -or $previewResult.Destinos.Count -ne 0) {
    throw "Previsualizar no falla cerrado antes de consultar repositorios."
}

Add-Type -TypeDefinition @'
using GestionDocumental_Docuarchi.net;
namespace Doc10Verification {
    public sealed class Gate : IWorkflowModernFeatureGate {
        public HabilitacionWorkflowModern Value;
        public HabilitacionWorkflowModern Evaluar(ContextoModuloWorkflow contexto) { return Value; }
    }
    public sealed class TaskRepository : ITareaWorkflowRepository {
        public TareaWorkflow Value;
        public int Calls;
        public TareaWorkflow ObtenerTarea(ContextoModuloWorkflow contexto, long idTarea) { Calls++; return Value; }
    }
    public sealed class FlowRepository : ITransicionFlujoRepository {
        public ResultadoDestinosTransicion Value;
        public int Calls;
        public ResultadoDestinosTransicion ObtenerDestinos(ContextoModuloWorkflow contexto, TareaWorkflow tarea) { Calls++; return Value; }
    }
    public sealed class RouteRepository : ITransicionRutaRepository {
        public ResultadoDestinosTransicion Value;
        public int Calls;
        public ResultadoDestinosTransicion ObtenerDestinos(ContextoModuloWorkflow contexto, TareaWorkflow tarea) { Calls++; return Value; }
    }
}
'@ -ReferencedAssemblies $AssemblyPath

function New-EnabledGate {
    $gate = New-Object Doc10Verification.Gate
    $gate.Value = New-Object GestionDocumental_Docuarchi.net.HabilitacionWorkflowModern
    $gate.Value.Estado = "activo"
    $gate.Value.Codigo = "WORKFLOW_MODERN_ACTIVE"
    $gate.Value.MensajeFuncional = "Activo para prueba"
    return $gate
}

function New-ActiveTask {
    param([string]$Decision)
    $task = New-Object GestionDocumental_Docuarchi.net.TareaWorkflow
    $task.IdTarea = 1
    $task.IdActividadOrigen = 10
    $task.IdActividadFlujoTrabajo = 20
    $task.IdFlujoTrabajo = if ($Decision -eq "FLUJO") { 30 } else { 0 }
    $task.IdRuta = 40
    $task.Radicado = "RAD-TEST"
    $task.GrupoActual = "Grupo de prueba"
    $task.TipoDecision = $Decision
    $task.TokenVersion = "estado-1"
    $task.EstaActiva = $true
    return $task
}

function New-Scenario {
    $gate = New-EnabledGate
    $taskRepository = New-Object Doc10Verification.TaskRepository
    $flowRepository = New-Object Doc10Verification.FlowRepository
    $routeRepository = New-Object Doc10Verification.RouteRepository
    $constructor = $serviceType.GetConstructors() | Where-Object { $_.GetParameters().Count -eq 5 } | Select-Object -First 1
    $service = $constructor.Invoke(@($taskRepository.PSObject.BaseObject, $flowRepository.PSObject.BaseObject, $routeRepository.PSObject.BaseObject, $gate.PSObject.BaseObject, [Activator]::CreateInstance($assembly.GetType("GestionDocumental_Docuarchi.net.ValidadorTransicionTarea", $true))))
    return [pscustomobject]@{ Gate = $gate; Task = $taskRepository; Flow = $flowRepository; Route = $routeRepository; Service = $service }
}

function New-DestinationsResult {
    param([string]$Code, [string]$Message, [string]$Kind)
    $result = New-Object GestionDocumental_Docuarchi.net.ResultadoDestinosTransicion
    $result.CodigoBloqueo = $Code
    $result.MensajeFuncional = $Message
    if (-not [string]::IsNullOrWhiteSpace($Kind)) {
        $destination = New-Object GestionDocumental_Docuarchi.net.DestinoTransicion
        $destination.IdConector = 7
        $destination.IdActividadDestino = 11
        $destination.Nombre = "Destino de prueba"
        $destination.TipoTransicion = $Kind
        $destination.Orden = 1
        $result.Destinos.Add($destination)
    }
    return $result
}

function Assert-PreviewCode {
    param($Result, [string]$ExpectedCode)
    if ($null -eq $Result.Error -or $Result.Error.Codigo -ne $ExpectedCode) {
        throw "Se esperaba $ExpectedCode y se obtuvo '$($Result.Error.Codigo)'."
    }
}

$invalidContext = [Activator]::CreateInstance($contextType)
$scenario = New-Scenario
Assert-PreviewCode -Result ($scenario.Service.Previsualizar($invalidContext, [Int64]1)) -ExpectedCode "WORKFLOW_CONTEXT_INVALID"
if ($scenario.Task.Calls -ne 0) { throw "El contexto invalido consulto la tarea." }

$scenario = New-Scenario
$scenario.Gate.Value.Estado = "inactivo"
$scenario.Gate.Value.Codigo = "WORKFLOW_MODERN_INACTIVE"
Assert-PreviewCode -Result ($scenario.Service.Previsualizar($context, [Int64]1)) -ExpectedCode "WORKFLOW_MODERN_INACTIVE"
if ($scenario.Task.Calls -ne 0) { throw "El gate inactivo consulto la tarea." }

$scenario = New-Scenario
Assert-PreviewCode -Result ($scenario.Service.Previsualizar($context, [Int64]0)) -ExpectedCode "WORKFLOW_TASK_INVALID"
if ($scenario.Task.Calls -ne 0) { throw "La tarea invalida consulto repositorios." }

$scenario = New-Scenario
$scenario.Task.Value = $null
Assert-PreviewCode -Result ($scenario.Service.Previsualizar($context, [Int64]1)) -ExpectedCode "WORKFLOW_TASK_UNAVAILABLE"

$scenario = New-Scenario
$scenario.Task.Value = New-ActiveTask -Decision "RUTA"
$scenario.Route.Value = New-DestinationsResult -Code "WORKFLOW_ROUTE_CLOSED" -Message "Cerrada" -Kind ""
Assert-PreviewCode -Result ($scenario.Service.Previsualizar($context, [Int64]1)) -ExpectedCode "WORKFLOW_ROUTE_CLOSED"

$scenario = New-Scenario
$scenario.Task.Value = New-ActiveTask -Decision "FLUJO"
$scenario.Flow.Value = New-DestinationsResult -Code "WORKFLOW_CONNECTOR_UNAVAILABLE" -Message "Ajeno" -Kind ""
Assert-PreviewCode -Result ($scenario.Service.Previsualizar($context, [Int64]1)) -ExpectedCode "WORKFLOW_CONNECTOR_UNAVAILABLE"

$scenario = New-Scenario
$scenario.Task.Value = New-ActiveTask -Decision "DESCONOCIDO"
Assert-PreviewCode -Result ($scenario.Service.Previsualizar($context, [Int64]1)) -ExpectedCode "WORKFLOW_TRANSITION_INCONSISTENT"

$scenario = New-Scenario
$scenario.Task.Value = New-ActiveTask -Decision "FLUJO"
$scenario.Flow.Value = New-DestinationsResult -Code "" -Message "" -Kind ""
Assert-PreviewCode -Result ($scenario.Service.Previsualizar($context, [Int64]1)) -ExpectedCode "WORKFLOW_NO_DESTINATIONS"

$scenario = New-Scenario
$scenario.Task.Value = New-ActiveTask -Decision "FLUJO"
$scenario.Flow.Value = New-DestinationsResult -Code "" -Message "" -Kind "FLUJO"
$flowPreview = $scenario.Service.Previsualizar($context, [Int64]1)
if ($null -ne $flowPreview.Error -or $flowPreview.Destinos.Count -ne 1 -or $flowPreview.Destinos[0].Tipo -ne "FLUJO") {
    throw "El escenario de flujo no devuelve el destino seguro esperado."
}

$scenario = New-Scenario
$scenario.Task.Value = New-ActiveTask -Decision "RUTA"
$scenario.Route.Value = New-DestinationsResult -Code "" -Message "" -Kind "RUTA"
$routePreview = $scenario.Service.Previsualizar($context, [Int64]1)
if ($null -ne $routePreview.Error -or $routePreview.Destinos.Count -ne 1 -or $routePreview.Destinos[0].Tipo -ne "RUTA") {
    throw "El escenario de ruta no devuelve el destino seguro esperado."
}

$endpointSource = Get-Content -LiteralPath (Join-Path $SourceRoot "webservice\WebServiceWorkflowModern.asmx.vb") -Raw
$sessionGateSource = Get-Content -LiteralPath (Join-Path $SourceRoot "webservice\WorkflowPreviewSessionContextGate.vb") -Raw
$moduleConnectionFactorySource = Get-Content -LiteralPath (Join-Path $SourceRoot "Infrastructure\Shared\Data\WorkflowModuleConnectionFactory.vb") -Raw
$previewSource = Get-Content -LiteralPath (Join-Path $SourceRoot "Services\Workflow\Terminar\ServicioTransicionTarea.vb") -Raw
$repositoriesRoot = Join-Path $SourceRoot "Infrastructure\Repositories\Workflow"
$flowRepositorySource = Get-Content -LiteralPath (Join-Path $repositoriesRoot "MySqlWorkflowPreviewRepositories.vb") -Raw

if ($endpointSource -match "\b(?:IdUsuario|IdGrupo|IdRuta|IdActividad)\s+As\s+(?:Integer|Long)" -or
    $endpointSource -match "Terminar_Tarea_Workflow|Cambia_Estado|PRETERMINARACTIVIAD|TERMINARACTIVIDAD") {
    throw "El ASMX recibe autorizacion del cliente o depende del flujo legacy."
}
if ($endpointSource -notmatch "WorkflowPreviewSessionContextGate" -or
    $endpointSource -notmatch "WorkflowModuleConnectionFactory" -or
    $endpointSource -notmatch "DocuarchiModuleConnectionFactory" -or
    $endpointSource -notmatch "CadenaConexionDocuarchi") {
    throw "El ASMX no compone el gate de contexto y las conexiones de Workflow y Docuarchi."
}
if ($sessionGateSource -notmatch "SolicitaDatosUsuarioGestionLogin" -or
    $sessionGateSource -notmatch "SolicitaIdUsuarIdRutaGrupoWorkflow" -or
    $sessionGateSource -notmatch 'CrearCadenaConexion\(requestContext, "DA_"\)' -or
    $sessionGateSource -match "InicializaSesionModuloWorkflow|RegistraLogSesionUsuarioWorkflow|ExecuteNonQuery") {
    throw "El gate de contexto no conserva el bootstrap de solo lectura ni el snapshot Docuarchi desde Gestión."
}
if ($moduleConnectionFactorySource -match "\bHttpContext\s*\.|\bSession\s*\.") {
    throw "La factoría del módulo Workflow no puede leer la sesión."
}
if ($previewSource -notmatch "La previsualizacion permanece libre de escritura, guard y adaptadores legacy" -or
    $previewSource -notmatch "Public Function Previsualizar" -or
    $previewSource -notmatch "If Not habilitacion.Activo Then[\s\S]{0,600}Return respuesta") {
    throw "El caso de uso no deja verificable la composicion de solo lectura o el gate previo."
}
if ($flowRepositorySource -match "TIPO_RUTA_ABIERTA_CERRADA|TIPO_ABIERTA_CERRADA_ACTIVIDAD") {
    throw "El preview de flujo no puede interpretar los campos de libertad de asignacion como bloqueo de envio."
}
if ($flowRepositorySource -notmatch "tipo_doc_entrante" -or
    $flowRepositorySource -notmatch "EjecutarLecturaDesde\(Of Boolean\)\(_docuarchiConnectionFactory") {
    throw "El repositorio de ruta no separa el estado documental de los datos Workflow."
}

$forbiddenRepositoryCoupling = & rg -n "\bHttpContext\.|\bSession\b|\bDataSet\b|\b(?:Page|GridView|UpdatePanel|ModalPopupExtender)\b|ExecuteNonQuery\s*\(|\b(?:INSERT|UPDATE|DELETE)\s+INTO\b" (Join-Path $repositoriesRoot "MySqlWorkflowPreviewRepositories.vb")
if ($LASTEXITCODE -eq 0) {
    throw "Los repositorios de preview tienen acoplamiento o escritura prohibida:`n$forbiddenRepositoryCoupling"
}
if ($LASTEXITCODE -ne 1) {
    throw "No fue posible verificar los repositorios de preview."
}

Write-Output "PASS DOC-10 preview: ASMX minimo; gate fail-closed; catalogos Workflow/Docuarchi aislados; destinos de flujo/ruta; semantica de asignacion preservada; repositorios sin Web Forms ni escritura."
