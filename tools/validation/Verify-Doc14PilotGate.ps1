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

$compiler = Get-Command csc.exe -ErrorAction Stop
$temporaryRoot = Join-Path ([IO.Path]::GetTempPath()) ("doc14-gate-" + [Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $temporaryRoot | Out-Null

try {
    $assemblyDirectory = Split-Path -Parent (Resolve-Path -LiteralPath $AssemblyPath)
    Copy-Item -LiteralPath (Join-Path $assemblyDirectory "GestionDocumental-Docuarchi.net.dll") -Destination $temporaryRoot
    Get-ChildItem -LiteralPath $assemblyDirectory -Filter *.dll | Where-Object { $_.Name -ne "GestionDocumental-Docuarchi.net.dll" } |
        Copy-Item -Destination $temporaryRoot

    $probePath = Join-Path $temporaryRoot "GateProbe.cs"
    $probeExe = Join-Path $temporaryRoot "GateProbe.exe"
    @'
using System;
using GestionDocumental_Docuarchi.net;

public static class GateProbe
{
    public static int Main(string[] args)
    {
        var context = new ContextoModuloWorkflow {
            IdUsuarioWorkflow = 10,
            IdGrupoWorkflow = 20,
            IdRutaWorkflow = 30,
            LoginUsuario = "doc14-piloto-prueba"
        };
        var result = new ConfiguracionWorkflowModernFeatureGate().Evaluar(context);
        if (!String.Equals(result.Estado, args[0], StringComparison.Ordinal) ||
            !String.Equals(result.Codigo, args[1], StringComparison.Ordinal))
        {
            Console.Error.WriteLine("Resultado inesperado: " + result.Estado + "/" + result.Codigo);
            return 1;
        }
        Console.WriteLine(result.Estado + "/" + result.Codigo);
        return 0;
    }
}
'@ | Set-Content -LiteralPath $probePath -Encoding UTF8

    & $compiler.Source "/nologo" "/target:exe" "/out:$probeExe" "/reference:$temporaryRoot\GestionDocumental-Docuarchi.net.dll" $probePath
    if ($LASTEXITCODE -ne 0) { throw "No fue posible compilar el probe aislado DOC-14." }

    function Invoke-GateScenario {
        param([string]$Name, [hashtable]$Settings, [string]$ExpectedState, [string]$ExpectedCode)

        $configEntries = foreach ($key in $Settings.Keys) {
            "<add key=`"$key`" value=`"$($Settings[$key])`" />"
        }
        @"
<?xml version="1.0" encoding="utf-8" ?>
<configuration><appSettings>$($configEntries -join "")</appSettings></configuration>
"@ | Set-Content -LiteralPath "$probeExe.config" -Encoding UTF8

        $output = & $probeExe $ExpectedState $ExpectedCode 2>&1
        if ($LASTEXITCODE -ne 0) { throw "Escenario $Name falló: $output" }
        Write-Output "PASS DOC-14 gate: $Name ($output)"
    }

    $base = @{
        WorkflowCentroTrabajoModernActive = "false"
        WorkflowCentroTrabajoModernOfficialMode = "false"
        WorkflowCentroTrabajoModernUsers = ""
        WorkflowCentroTrabajoModernGroups = ""
        WorkflowCentroTrabajoModernExcludedUsers = ""
        WorkflowCentroTrabajoModernExcludedGroups = ""
        WorkflowCentroTrabajoModernPilotStartUtc = ""
        WorkflowCentroTrabajoModernPilotOwner = ""
        WorkflowCentroTrabajoModernPilotReason = ""
        WorkflowCentroTrabajoModernRollbackUtc = ""
        WorkflowCentroTrabajoModernRollbackOwner = ""
        WorkflowCentroTrabajoModernRollbackReason = ""
        WorkflowCentroTrabajoModernRollbackCorrelation = ""
    }
    function Merge-Settings { param([hashtable]$Changes) $result = @{}; $base.Keys | ForEach-Object { $result[$_] = $base[$_] }; $Changes.Keys | ForEach-Object { $result[$_] = $Changes[$_] }; return $result }

    Invoke-GateScenario "inactivo" (Merge-Settings @{}) "inactivo" "WORKFLOW_MODERN_INACTIVE"
    Invoke-GateScenario "alcance-vacio" (Merge-Settings @{ WorkflowCentroTrabajoModernActive = "true" }) "fallback-legacy" "WORKFLOW_MODERN_PILOT_SCOPE_REQUIRED"
    Invoke-GateScenario "metadatos-invalidos" (Merge-Settings @{ WorkflowCentroTrabajoModernActive = "true"; WorkflowCentroTrabajoModernUsers = "doc14-piloto-prueba" }) "fallback-legacy" "WORKFLOW_MODERN_PILOT_METADATA_INVALID"
    Invoke-GateScenario "exclusion" (Merge-Settings @{ WorkflowCentroTrabajoModernActive = "true"; WorkflowCentroTrabajoModernUsers = "doc14-piloto-prueba"; WorkflowCentroTrabajoModernExcludedUsers = "doc14-piloto-prueba"; WorkflowCentroTrabajoModernPilotStartUtc = "2026-08-18T00:00:00Z"; WorkflowCentroTrabajoModernPilotOwner = "rol-operacion"; WorkflowCentroTrabajoModernPilotReason = "prueba-aislada" }) "excluido" "WORKFLOW_MODERN_EXCLUDED"
    Invoke-GateScenario "usuario-incluido" (Merge-Settings @{ WorkflowCentroTrabajoModernActive = "true"; WorkflowCentroTrabajoModernUsers = "doc14-piloto-prueba"; WorkflowCentroTrabajoModernPilotStartUtc = "2026-08-18T00:00:00Z"; WorkflowCentroTrabajoModernPilotOwner = "rol-operacion"; WorkflowCentroTrabajoModernPilotReason = "prueba-aislada" }) "activo" "WORKFLOW_MODERN_ACTIVE"
    Invoke-GateScenario "grupo-incluido" (Merge-Settings @{ WorkflowCentroTrabajoModernActive = "true"; WorkflowCentroTrabajoModernGroups = "20"; WorkflowCentroTrabajoModernPilotStartUtc = "2026-08-18T00:00:00Z"; WorkflowCentroTrabajoModernPilotOwner = "rol-operacion"; WorkflowCentroTrabajoModernPilotReason = "prueba-aislada" }) "activo" "WORKFLOW_MODERN_ACTIVE"
    Invoke-GateScenario "oficial" (Merge-Settings @{ WorkflowCentroTrabajoModernActive = "true"; WorkflowCentroTrabajoModernOfficialMode = "true"; WorkflowCentroTrabajoModernPilotStartUtc = "2026-08-18T00:00:00Z"; WorkflowCentroTrabajoModernPilotOwner = "rol-operacion"; WorkflowCentroTrabajoModernPilotReason = "habilitacion-oficial" }) "activo" "WORKFLOW_MODERN_ACTIVE"
    Invoke-GateScenario "oficial-con-alcance" (Merge-Settings @{ WorkflowCentroTrabajoModernActive = "true"; WorkflowCentroTrabajoModernOfficialMode = "true"; WorkflowCentroTrabajoModernUsers = "doc14-piloto-prueba"; WorkflowCentroTrabajoModernPilotStartUtc = "2026-08-18T00:00:00Z"; WorkflowCentroTrabajoModernPilotOwner = "rol-operacion"; WorkflowCentroTrabajoModernPilotReason = "habilitacion-oficial" }) "fallback-legacy" "WORKFLOW_MODERN_OFFICIAL_SCOPE_CONFLICT"
    Invoke-GateScenario "rollback" (Merge-Settings @{ WorkflowCentroTrabajoModernRollbackUtc = "2026-08-18T00:00:00Z"; WorkflowCentroTrabajoModernRollbackOwner = "rol-operacion"; WorkflowCentroTrabajoModernRollbackReason = "prueba-aislada"; WorkflowCentroTrabajoModernRollbackCorrelation = "DOC14-ROLLBACK-001" }) "fallback-legacy" "WORKFLOW_MODERN_ROLLBACK_ACTIVE"
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) { Remove-Item -LiteralPath $temporaryRoot -Recurse -Force }
}
