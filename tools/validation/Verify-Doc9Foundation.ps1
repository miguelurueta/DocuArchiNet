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

$expectedApplicationFiles = @(
    (Join-Path $SourceRoot "DTOs\Workflow\Terminar\TransicionWorkflowDtos.vb"),
    (Join-Path $SourceRoot "Services\Workflow\Terminar\ServicioTransicionTarea.vb"),
    (Join-Path $SourceRoot "Services\Workflow\Terminar\ValidadorTransicionTarea.vb"),
    (Join-Path $SourceRoot "Modelo\Workflow\Terminar\WorkflowModernModels.vb"),
    (Join-Path $SourceRoot "Modelo\Workflow\Terminar\WorkflowModernInterfaces.vb"),
    (Join-Path $SourceRoot "Domain\Shared\ContextoModulo.vb"),
    (Join-Path $SourceRoot "Infrastructure\Shared\Data\ModuleDataContracts.vb"),
    (Join-Path $SourceRoot "Infrastructure\Shared\Data\ModuleConnectionFactory.vb"),
    (Join-Path $SourceRoot "Infrastructure\Shared\Data\AdoNetDataInfrastructure.vb"),
    (Join-Path $SourceRoot "Infrastructure\Workflow\Terminar\WorkflowLegacyExecutorAdapter.vb"),
    (Join-Path $SourceRoot "Infrastructure\Workflow\Terminar\ConfiguracionWorkflowModernFeatureGate.vb"),
    (Join-Path $SourceRoot "Infrastructure\Repositories\Workflow\README.md")
)
foreach ($expectedApplicationFile in $expectedApplicationFiles) {
    if (-not (Test-Path -LiteralPath $expectedApplicationFile)) {
        throw "No se respeta la estructura Application por caso de uso: $expectedApplicationFile"
    }
}

$assembly = [System.Reflection.Assembly]::LoadFrom((Resolve-Path -LiteralPath $AssemblyPath))
$contextType = $assembly.GetType("GestionDocumental_Docuarchi.net.ContextoModuloWorkflow", $true)
$baseContextType = $assembly.GetType("GestionDocumental_Docuarchi.net.ContextoModulo", $true)
$gateType = $assembly.GetType("GestionDocumental_Docuarchi.net.ConfiguracionWorkflowModernFeatureGate", $true)
$adapterType = $assembly.GetType("GestionDocumental_Docuarchi.net.WorkflowLegacyExecutorAdapter", $true)
$requestType = $assembly.GetType("GestionDocumental_Docuarchi.net.SolicitudTransicionWorkflow", $true)
$connectionFactoryType = $assembly.GetType("GestionDocumental_Docuarchi.net.ModuleConnectionFactory", $true)

if (-not $baseContextType.IsAssignableFrom($contextType) -or $null -eq $connectionFactoryType) {
    throw "La infraestructura compartida no expone el contexto y factory general esperados."
}

$context = [Activator]::CreateInstance($contextType)
$context.IdUsuarioWorkflow = 1
$context.IdGrupoWorkflow = 1
$context.LoginUsuario = "doc9-foundation-test"

$gateResult = $gateType.GetMethod("Evaluar").Invoke([Activator]::CreateInstance($gateType), @($context))
if ($gateResult.Estado -ne "inactivo" -or $gateResult.Codigo -ne "WORKFLOW_MODERN_INACTIVE") {
    throw "El feature gate no fallo cerrado: $($gateResult.Estado)/$($gateResult.Codigo)"
}

$request = [Activator]::CreateInstance($requestType)
$request.IdTarea = 1
$request.IdConector = 1
$adapterResult = $adapterType.GetMethod("Ejecutar").Invoke([Activator]::CreateInstance($adapterType), @($context, $request))
if ($adapterResult.Exito -or $adapterResult.CodigoBloqueo -ne "WORKFLOW_MODERN_EXECUTION_PENDING") {
    throw "El adaptador ejecuto o no bloqueo la transicion: $($adapterResult.Exito)/$($adapterResult.CodigoBloqueo)"
}

$applicationBoundaryRoots = @(
    (Join-Path $SourceRoot "Domain\Shared"),
    (Join-Path $SourceRoot "Modelo\Workflow\Terminar"),
    (Join-Path $SourceRoot "DTOs\Workflow\Terminar"),
    (Join-Path $SourceRoot "Services\Workflow\Terminar")
)
$forbidden = & rg -n -g "*.vb" "^\s*Imports\s+System\.Web|\bHttpContext\.|\b(?:As|Inherits)\s+(?:[A-Za-z0-9_.]+\.)?(?:Page|GridView|UpdatePanel|ModalPopupExtender)\b" @applicationBoundaryRoots
if ($LASTEXITCODE -eq 0) {
    throw "Domain/Application no pueden depender de Web Forms:`n$forbidden"
}
if ($LASTEXITCODE -ne 1) {
    throw "No fue posible inspeccionar dependencias Web Forms."
}

$sharedDataCoupling = & rg -n -g "*.vb" "ContextoModuloWorkflow|WorkflowModuleConnectionFactory|WORKFLOW_" (Join-Path $SourceRoot "Infrastructure\Shared\Data")
if ($LASTEXITCODE -eq 0) {
    throw "Infrastructure/Shared/Data no puede depender de Workflow:`n$sharedDataCoupling"
}
if ($LASTEXITCODE -ne 1) {
    throw "No fue posible inspeccionar el acoplamiento de Infrastructure/Shared/Data."
}

$legacyCalls = & rg -n -g "*.vb" "Terminar_Tarea_Workflow\s*\(|Cambia_Estado\s*\(" `
    (Join-Path $SourceRoot "Infrastructure\Workflow\Terminar") `
    (Join-Path $SourceRoot "Services\Workflow\Terminar")
if ($LASTEXITCODE -eq 0) {
    throw "La fundacion contiene una llamada nueva al motor legacy:`n$legacyCalls"
}
if ($LASTEXITCODE -ne 1) {
    throw "No fue posible inspeccionar llamadas al motor legacy."
}

Write-Output "PASS DOC-9 foundation: feature-gate fail-closed; adapter inert; no Web Forms dependencies; no legacy calls."
