[CmdletBinding()]
param(
    [string]$AssemblyPath,
    [string]$SourceRoot
)

$ErrorActionPreference = "Stop"
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
if ([string]::IsNullOrWhiteSpace($SourceRoot)) { $SourceRoot = Join-Path $scriptDirectory "..\.." }
if ([string]::IsNullOrWhiteSpace($AssemblyPath)) { $AssemblyPath = Join-Path $SourceRoot "bin\GestionDocumental-Docuarchi.net.dll" }
if (-not (Test-Path -LiteralPath $AssemblyPath)) { throw "No existe el ensamblado compilado: $AssemblyPath" }

$requiredFiles = @(
    "webservice\WebServiceWorkflowModern.asmx.vb",
    "webservice\WorkflowPreviewSessionContextGate.vb",
    "Services\Workflow\Terminar\ServicioTransicionTarea.vb",
    "Services\Workflow\Terminar\ValidadorTransicionTarea.vb",
    "Infrastructure\Repositories\Workflow\MySqlTransicionEjecucionRepository.vb",
    "Infrastructure\Workflow\Terminar\MySqlTransicionConcurrencyGuard.vb",
    "Infrastructure\Workflow\Terminar\WorkflowLegacyExecutorAdapter.vb",
    "Infrastructure\Workflow\Terminar\WorkflowLegacyRequisitosAdapter.vb",
    "Infrastructure\Workflow\Terminar\WorkflowLegacyAuditoriaAdapter.vb"
)
foreach ($relativePath in $requiredFiles) {
    if (-not (Test-Path -LiteralPath (Join-Path $SourceRoot $relativePath))) { throw "Falta artefacto DOC-11: $relativePath" }
}

$assembly = [System.Reflection.Assembly]::LoadFrom((Resolve-Path -LiteralPath $AssemblyPath))
$namespace = "GestionDocumental_Docuarchi.net"
$endpointType = $assembly.GetType("$namespace.WebServiceWorkflowModern", $true)
$resultType = $assembly.GetType("$namespace.ResultadoTransicionDto", $true)
$serviceType = $assembly.GetType("$namespace.ServicioTransicionTarea", $true)
$validatorType = $assembly.GetType("$namespace.ValidadorTransicionTarea", $true)
$contextType = $assembly.GetType("$namespace.ContextoModuloWorkflow", $true)
$requestType = $assembly.GetType("$namespace.SolicitudTransicionWorkflow", $true)
$taskType = $assembly.GetType("$namespace.TareaWorkflow", $true)
$destinationType = $assembly.GetType("$namespace.DestinoEjecucionWorkflow", $true)

$method = $endpointType.GetMethod("EjecutarEnvioTarea")
if ($null -eq $method -or $method.ReturnType -ne $resultType) { throw "El ASMX no expone EjecutarEnvioTarea con ResultadoTransicionDto." }
$parameters = $method.GetParameters()
if ($parameters.Count -ne 3 -or $parameters[0].ParameterType -ne [Int64] -or $parameters[1].ParameterType -ne [Int32] -or $parameters[2].ParameterType -ne [String]) {
    throw "EjecutarEnvioTarea debe aceptar idTarea Long, idConector Integer y tokenVersion String."
}

Add-Type -TypeDefinition @'
using System;
using GestionDocumental_Docuarchi.net;
namespace Doc11Verification {
    public sealed class Gate : IWorkflowModernFeatureGate { public HabilitacionWorkflowModern Value; public int Calls; public HabilitacionWorkflowModern Evaluar(ContextoModuloWorkflow c) { Calls++; return Value; } }
    public sealed class TaskRepository : ITareaWorkflowRepository { public TareaWorkflow Value; public int Calls; public TareaWorkflow ObtenerTarea(ContextoModuloWorkflow c, long id) { Calls++; return Value; } }
    public sealed class FlowRepository : ITransicionFlujoRepository { public ResultadoDestinosTransicion ObtenerDestinos(ContextoModuloWorkflow c, TareaWorkflow t) { return new ResultadoDestinosTransicion(); } }
    public sealed class RouteRepository : ITransicionRutaRepository { public ResultadoDestinosTransicion ObtenerDestinos(ContextoModuloWorkflow c, TareaWorkflow t) { return new ResultadoDestinosTransicion(); } }
    public sealed class ExecutionRepository : ITransicionEjecucionRepository { public ResultadoResolucionDestinoTransicion Value; public int Calls; public ResultadoResolucionDestinoTransicion ResolverDestino(ContextoModuloWorkflow c, TareaWorkflow t, int id) { Calls++; return Value; } }
    public sealed class RequirementsRepository : IRequisitosTransicionRepository { public ResultadoRequisitosTransicion Value; public int Calls; public ResultadoRequisitosTransicion Evaluar(ContextoModuloWorkflow c, TareaWorkflow t, DestinoEjecucionWorkflow d) { Calls++; return Value; } }
    public sealed class AuditRepository : IAuditoriaTransicionRepository { public bool Value = true; public int Calls; public bool Registrar(AuditoriaTransicion a) { Calls++; return Value; } }
    public sealed class Lease : ITransicionConcurrencyLease { public bool Disposed; public void Dispose() { Disposed = true; } }
    public sealed class Guard : ITransicionConcurrencyGuard { public ResultadoGuardTransicion Value; public int Calls; public ResultadoGuardTransicion Adquirir(ContextoModuloWorkflow c, long id, string token) { Calls++; return Value; } }
    public sealed class Executor : IWorkflowLegacyExecutor { public ResultadoEjecucionWorkflow Value; public int Calls; public ResultadoEjecucionWorkflow Ejecutar(ContextoModuloWorkflow c, TareaWorkflow t, DestinoEjecucionWorkflow d) { Calls++; return Value; } }
}
'@ -ReferencedAssemblies $AssemblyPath

function New-EnabledGate {
    $gate = New-Object Doc11Verification.Gate
    $gate.Value = New-Object "$namespace.HabilitacionWorkflowModern"
    $gate.Value.Estado = "activo"; $gate.Value.Codigo = "WORKFLOW_MODERN_ACTIVE"; $gate.Value.MensajeFuncional = "Activo"
    return $gate
}
function New-Context {
    $context = New-Object $contextType
    $context.IdUsuarioWorkflow = 10; $context.IdGrupoWorkflow = 20; $context.IdRutaWorkflow = 30; $context.LoginUsuario = "doc11-test"
    return $context
}
function New-Task {
    $task = New-Object $taskType
    $task.IdTarea = 100; $task.IdActividadOrigen = 11; $task.IdRuta = 30; $task.TipoDecision = "RUTA"; $task.TokenVersion = "token-1"; $task.EstaActiva = $true
    return $task
}
function New-Destination {
    $destination = New-Object $destinationType
    $destination.IdConector = 7; $destination.IdActividadDestino = 12; $destination.TipoTransicion = "RUTA"; $destination.NombreActividadDestino = "Destino"; $destination.NombreGrupoDestino = "Grupo"
    return $destination
}
function New-Scenario {
    $gate = New-EnabledGate
    $tasks = New-Object Doc11Verification.TaskRepository; $tasks.Value = New-Task
    $execution = New-Object Doc11Verification.ExecutionRepository
    $execution.Value = New-Object "$namespace.ResultadoResolucionDestinoTransicion"; $execution.Value.Destino = New-Destination
    $requirements = New-Object Doc11Verification.RequirementsRepository
    $requirements.Value = New-Object "$namespace.ResultadoRequisitosTransicion"; $requirements.Value.Cumple = $true
    $audit = New-Object Doc11Verification.AuditRepository
    $lease = New-Object Doc11Verification.Lease
    $guard = New-Object Doc11Verification.Guard
    $guard.Value = New-Object "$namespace.ResultadoGuardTransicion"; $guard.Value.Adquirido = $true; $guard.Value.Lease = $lease
    $executor = New-Object Doc11Verification.Executor
    $executor.Value = New-Object "$namespace.ResultadoEjecucionWorkflow"; $executor.Value.Exito = $true; $executor.Value.EstadoFinal = "completada"; $executor.Value.MensajeFuncional = "Enviada"
    $applicationExecutor = New-Object -TypeName "$namespace.EjecutorTransicionTarea" -ArgumentList $executor.PSObject.BaseObject
    $constructor = $serviceType.GetConstructors() | Where-Object { $_.GetParameters().Count -eq 10 } | Select-Object -First 1
    if ($null -eq $constructor) { throw "No existe el constructor de ejecución compuesto." }
    $service = $constructor.Invoke(@($tasks.PSObject.BaseObject, $null, $null, $execution.PSObject.BaseObject, $requirements.PSObject.BaseObject, $audit.PSObject.BaseObject, $guard.PSObject.BaseObject, $gate.PSObject.BaseObject, [Activator]::CreateInstance($validatorType), $applicationExecutor.PSObject.BaseObject))
    return [pscustomobject]@{ Service=$service; Gate=$gate; Tasks=$tasks; Execution=$execution; Requirements=$requirements; Audit=$audit; Guard=$guard; Lease=$lease; Executor=$executor }
}
function New-Request([long]$TaskId = 100, [int]$ConnectorId = 7, [string]$Token = "token-1") {
    $request = New-Object $requestType; $request.IdTarea = $TaskId; $request.IdConector = $ConnectorId; $request.TokenVersion = $Token; return $request
}

$scenario = New-Scenario
$invalid = $scenario.Service.Ejecutar((New-Context), (New-Request 0 0 ""))
if ($invalid.CodigoBloqueo -ne "WORKFLOW_TASK_INVALID" -or $scenario.Tasks.Calls -ne 0 -or $scenario.Guard.Calls -ne 0) { throw "La validación no falla antes de consultar o bloquear." }

$scenario = New-Scenario
$scenario.Gate.Value.Estado = "inactivo"; $scenario.Gate.Value.Codigo = "WORKFLOW_MODERN_INACTIVE"
$inactive = $scenario.Service.Ejecutar((New-Context), (New-Request))
if ($inactive.CodigoBloqueo -ne "WORKFLOW_MODERN_INACTIVE" -or $scenario.Tasks.Calls -ne 0) { throw "El gate inactivo no falló cerrado." }

$scenario = New-Scenario
$scenario.Tasks.Value.TokenVersion = "otro-token"
$conflict = $scenario.Service.Ejecutar((New-Context), (New-Request))
if ($conflict.CodigoBloqueo -ne "WORKFLOW_VERSION_CONFLICT" -or $scenario.Executor.Calls -ne 0 -or -not $scenario.Lease.Disposed) { throw "El token vencido no se bloqueó y liberó correctamente." }

$scenario = New-Scenario
$scenario.Execution.Value = New-Object "$namespace.ResultadoResolucionDestinoTransicion"; $scenario.Execution.Value.CodigoBloqueo = "WORKFLOW_CONNECTOR_UNAVAILABLE"; $scenario.Execution.Value.MensajeFuncional = "No disponible"
$connector = $scenario.Service.Ejecutar((New-Context), (New-Request))
if ($connector.CodigoBloqueo -ne "WORKFLOW_CONNECTOR_UNAVAILABLE" -or $scenario.Executor.Calls -ne 0 -or $scenario.Audit.Calls -ne 1) { throw "El conector alterado alcanzó el motor o no fue auditado." }

$scenario = New-Scenario
$scenario.Requirements.Value.Cumple = $false; $scenario.Requirements.Value.CodigoBloqueo = "WORKFLOW_REQUIREMENT_NOT_MET"; $scenario.Requirements.Value.MensajeFuncional = "Pendiente"
$requirements = $scenario.Service.Ejecutar((New-Context), (New-Request))
if ($requirements.CodigoBloqueo -ne "WORKFLOW_REQUIREMENT_NOT_MET" -or $scenario.Executor.Calls -ne 0) { throw "Un requisito pendiente alcanzó el motor." }

$scenario = New-Scenario
$scenario.Executor.Value.Advertencias.Add("posterior")
$success = $scenario.Service.Ejecutar((New-Context), (New-Request))
if (-not $success.Exito -or $success.Destino.Id -ne 7 -or $success.Advertencias.Count -ne 1 -or $scenario.Executor.Calls -ne 1 -or -not $scenario.Lease.Disposed) { throw "El resultado exitoso no conserva destino, advertencia o liberación." }

$scenario = New-Scenario
$scenario.Guard.Value.Adquirido = $false; $scenario.Guard.Value.Lease = $null; $scenario.Guard.Value.CodigoBloqueo = "WORKFLOW_TRANSITION_IN_PROGRESS"; $scenario.Guard.Value.MensajeFuncional = "Ocupada"
$busy = $scenario.Service.Ejecutar((New-Context), (New-Request))
if ($busy.CodigoBloqueo -ne "WORKFLOW_TRANSITION_IN_PROGRESS" -or $scenario.Tasks.Calls -ne 0 -or $scenario.Executor.Calls -ne 0) { throw "El guard no impide la segunda ejecución." }

$sourceChecks = @{
    "webservice\WebServiceWorkflowModern.asmx.vb" = "EjecutarEnvioTarea"
    "webservice\WorkflowPreviewSessionContextGate.vb" = "AsegurarContextoEjecucion"
    "Infrastructure\Workflow\Terminar\MySqlTransicionConcurrencyGuard.vb" = "GET_LOCK"
}
foreach ($check in $sourceChecks.GetEnumerator()) {
    if ((Get-Content -LiteralPath (Join-Path $SourceRoot $check.Key) -Raw) -notmatch $check.Value) { throw "No se encontró '$($check.Value)' en $($check.Key)." }
}
$serviceSource = Get-Content -LiteralPath (Join-Path $SourceRoot "Services\Workflow\Terminar\ServicioTransicionTarea.vb") -Raw
$guardSource = Get-Content -LiteralPath (Join-Path $SourceRoot "Infrastructure\Workflow\Terminar\MySqlTransicionConcurrencyGuard.vb") -Raw
$gateSource = Get-Content -LiteralPath (Join-Path $SourceRoot "webservice\WorkflowPreviewSessionContextGate.vb") -Raw
$executionRepositorySource = Get-Content -LiteralPath (Join-Path $SourceRoot "Infrastructure\Repositories\Workflow\MySqlTransicionEjecucionRepository.vb") -Raw
if ($serviceSource -notmatch "Using guard.Lease" -or $guardSource -notmatch "RELEASE_LOCK" -or $gateSource -notmatch "SolicitaPermisosUsuarioWorkflow|CompilaScriptUsuario" -or $gateSource -match "InicializaSesionModuloWorkflow\s*\(" -or
    $executionRepositorySource -notmatch "ResolverRuta" -or $executionRepositorySource -notmatch "ResolverFlujo" -or $executionRepositorySource -notmatch "conector.ID_ACTIVIDAD_DESTINO" -or $executionRepositorySource -notmatch "IdActividadFlujoTrabajoDestino") { throw "No se preserva el contrato de guard, gate o mapeo RUTA/FLUJO." }

$workflowCalls = @(& rg -l -g "*.vb" "Terminar_Tarea_Workflow\s*\(" (Join-Path $SourceRoot "webservice") (Join-Path $SourceRoot "Services") (Join-Path $SourceRoot "Infrastructure"))
if ($LASTEXITCODE -ne 0 -or $workflowCalls.Count -ne 1 -or [System.IO.Path]::GetFileName($workflowCalls[0]) -ne "WorkflowLegacyExecutorAdapter.vb") { throw "Solo WorkflowLegacyExecutorAdapter puede llamar Terminar_Tarea_Workflow en las capas nuevas." }

Write-Output "PASS DOC-11: contrato ASMX; validación; gate; token; conector; requisitos; resultado; guard; límite legacy y fuentes RUTA/FLUJO verificados sin modificar datos."
