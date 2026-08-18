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

$assembly = [System.Reflection.Assembly]::LoadFrom((Resolve-Path -LiteralPath $AssemblyPath))
$namespace = "GestionDocumental_Docuarchi.net"
$serviceType = $assembly.GetType("$namespace.ServicioTransicionTarea", $true)
$validatorType = $assembly.GetType("$namespace.ValidadorTransicionTarea", $true)
$contextType = $assembly.GetType("$namespace.ContextoModuloWorkflow", $true)
$requestType = $assembly.GetType("$namespace.SolicitudTransicionWorkflow", $true)
$taskType = $assembly.GetType("$namespace.TareaWorkflow", $true)
$destinationType = $assembly.GetType("$namespace.DestinoEjecucionWorkflow", $true)
$auditType = $assembly.GetType("$namespace.AuditoriaTransicion", $true)

Add-Type -TypeDefinition @'
using System;
using GestionDocumental_Docuarchi.net;
namespace Doc14TelemetryVerification {
    public sealed class Gate : IWorkflowModernFeatureGate { public HabilitacionWorkflowModern Value; public HabilitacionWorkflowModern Evaluar(ContextoModuloWorkflow c) { return Value; } }
    public sealed class TaskRepository : ITareaWorkflowRepository { public TareaWorkflow Value; public TareaWorkflow ObtenerTarea(ContextoModuloWorkflow c, long id) { return Value; } }
    public sealed class ExecutionRepository : ITransicionEjecucionRepository { public ResultadoResolucionDestinoTransicion Value; public ResultadoResolucionDestinoTransicion ResolverDestino(ContextoModuloWorkflow c, TareaWorkflow t, int id) { return Value; } }
    public sealed class RequirementsRepository : IRequisitosTransicionRepository { public ResultadoRequisitosTransicion Value; public ResultadoRequisitosTransicion Evaluar(ContextoModuloWorkflow c, TareaWorkflow t, DestinoEjecucionWorkflow d) { return Value; } }
    public sealed class AuditRepository : IAuditoriaTransicionRepository { public bool Value = true; public bool ThrowOnRegister; public int Calls; public AuditoriaTransicion Last; public bool Registrar(AuditoriaTransicion a) { Calls++; Last = a; if (ThrowOnRegister) throw new InvalidOperationException(); return Value; } }
    public sealed class Lease : ITransicionConcurrencyLease { public void Dispose() { } }
    public sealed class Guard : ITransicionConcurrencyGuard { public ResultadoGuardTransicion Value; public ResultadoGuardTransicion Adquirir(ContextoModuloWorkflow c, long id, string token) { return Value; } }
    public sealed class Executor : IWorkflowLegacyExecutor { public ResultadoEjecucionWorkflow Value; public ResultadoEjecucionWorkflow Ejecutar(ContextoModuloWorkflow c, TareaWorkflow t, DestinoEjecucionWorkflow d) { return Value; } }
}
'@ -ReferencedAssemblies $AssemblyPath

function New-Scenario {
    $gate = New-Object Doc14TelemetryVerification.Gate
    $gate.Value = New-Object "$namespace.HabilitacionWorkflowModern" -Property @{ Estado = "activo"; Codigo = "WORKFLOW_MODERN_ACTIVE"; MensajeFuncional = "Activo" }
    $task = New-Object Doc14TelemetryVerification.TaskRepository
    $task.Value = New-Object $taskType -Property @{ IdTarea = 100; IdRuta = 30; IdFlujoTrabajo = 40; IdActividadOrigen = 11; TokenVersion = "version-prueba"; EstaActiva = $true }
    $destination = New-Object $destinationType -Property @{ IdConector = 7; IdActividadDestino = 12; TipoTransicion = "RUTA"; NombreActividadDestino = "Destino" }
    $execution = New-Object Doc14TelemetryVerification.ExecutionRepository
    $execution.Value = New-Object "$namespace.ResultadoResolucionDestinoTransicion" -Property @{ Destino = $destination }
    $requirements = New-Object Doc14TelemetryVerification.RequirementsRepository
    $requirements.Value = New-Object "$namespace.ResultadoRequisitosTransicion" -Property @{ Cumple = $true }
    $audit = New-Object Doc14TelemetryVerification.AuditRepository
    $guard = New-Object Doc14TelemetryVerification.Guard
    $guard.Value = New-Object "$namespace.ResultadoGuardTransicion" -Property @{ Adquirido = $true; Lease = (New-Object Doc14TelemetryVerification.Lease) }
    $executor = New-Object Doc14TelemetryVerification.Executor
    $executor.Value = New-Object "$namespace.ResultadoEjecucionWorkflow" -Property @{ Exito = $true; EstadoFinal = "completada"; MensajeFuncional = "Enviada" }
    $applicationExecutor = New-Object "$namespace.EjecutorTransicionTarea" -ArgumentList $executor.PSObject.BaseObject
    $constructor = $serviceType.GetConstructors() | Where-Object { $_.GetParameters().Count -eq 10 } | Select-Object -First 1
    $service = $constructor.Invoke(@($task.PSObject.BaseObject, $null, $null, $execution.PSObject.BaseObject, $requirements.PSObject.BaseObject, $audit.PSObject.BaseObject, $guard.PSObject.BaseObject, $gate.PSObject.BaseObject, [Activator]::CreateInstance($validatorType), $applicationExecutor.PSObject.BaseObject))
    return [pscustomobject]@{ Service = $service; Requirements = $requirements; Audit = $audit; Executor = $executor }
}

function New-Context {
    return New-Object $contextType -Property @{ IdUsuarioWorkflow = 10; IdGrupoWorkflow = 20; IdRutaWorkflow = 30; LoginUsuario = "doc14-prueba" }
}

function New-Request {
    return New-Object $requestType -Property @{ IdTarea = 100; IdConector = 7; TokenVersion = "version-prueba" }
}

$s = New-Scenario
$success = $s.Service.Ejecutar((New-Context), (New-Request))
if (-not $success.Exito -or $s.Audit.Calls -ne 1 -or $s.Audit.Last.Canal -ne "MODERNO" -or $s.Audit.Last.IdRutaWorkflow -ne 30 -or $s.Audit.Last.IdFlujoTrabajo -ne 40 -or $s.Audit.Last.IdConector -ne 7 -or $s.Audit.Last.Resultado -ne "EXITO" -or $s.Audit.Last.CodigoFuncional -ne "WORKFLOW_MODERN_SUCCESS" -or [string]::IsNullOrWhiteSpace($success.ReferenciaAuditoria)) { throw "La telemetría de éxito no conserva el contrato esperado." }

$s = New-Scenario
$s.Requirements.Value.Cumple = $false; $s.Requirements.Value.CodigoBloqueo = "WORKFLOW_REQUIREMENT_NOT_MET"; $s.Requirements.Value.MensajeFuncional = "Pendiente"
$blocked = $s.Service.Ejecutar((New-Context), (New-Request))
if ($blocked.Exito -or $s.Audit.Last.Resultado -ne "BLOQUEADO" -or $s.Audit.Last.CodigoFuncional -ne "WORKFLOW_REQUIREMENT_NOT_MET") { throw "La telemetría de bloqueo no conserva código y resultado." }

$s = New-Scenario
$s.Executor.Value.Exito = $false; $s.Executor.Value.EstadoFinal = "error"; $s.Executor.Value.CodigoBloqueo = "WORKFLOW_TRANSITION_REJECTED"; $s.Executor.Value.MensajeFuncional = "Error controlado"
$errorResult = $s.Service.Ejecutar((New-Context), (New-Request))
if ($errorResult.Exito -or $s.Audit.Last.Resultado -ne "ERROR" -or $s.Audit.Last.CodigoFuncional -ne "WORKFLOW_TRANSITION_REJECTED") { throw "La telemetría de error no conserva código y resultado." }

$s = New-Scenario
$s.Audit.Value = $false
$auditFailed = $s.Service.Ejecutar((New-Context), (New-Request))
if (-not $auditFailed.Exito -or $auditFailed.Advertencias.Count -ne 1 -or -not [string]::IsNullOrWhiteSpace($auditFailed.ReferenciaAuditoria)) { throw "El rechazo del repositorio de auditoría reemplazó el resultado funcional." }

$s = New-Scenario
$s.Audit.ThrowOnRegister = $true
$auditThrows = $s.Service.Ejecutar((New-Context), (New-Request))
if (-not $auditThrows.Exito -or $auditThrows.Advertencias.Count -ne 1) { throw "La excepción del repositorio de auditoría reemplazó el resultado funcional." }

$propertyNames = @($auditType.GetProperties() | ForEach-Object { $_.Name }) -join ","
if ($propertyNames -match "Login|Sql|Session|Token|Documento|Payload|Password|Clave") { throw "El modelo de auditoría expone campos sensibles." }
Write-Output "PASS DOC-14 telemetría: éxito, bloqueo, error y fallas de persistencia verificados sin datos reales."
