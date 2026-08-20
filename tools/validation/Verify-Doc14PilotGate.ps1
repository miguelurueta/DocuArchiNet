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
$temporaryRoot = Join-Path ([IO.Path]::GetTempPath()) ("doc14-policy-" + [Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $temporaryRoot | Out-Null

try {
    $assemblyDirectory = Split-Path -Parent (Resolve-Path -LiteralPath $AssemblyPath)
    Copy-Item -LiteralPath (Join-Path $assemblyDirectory "GestionDocumental-Docuarchi.net.dll") -Destination $temporaryRoot
    Get-ChildItem -LiteralPath $assemblyDirectory -Filter *.dll | Where-Object { $_.Name -ne "GestionDocumental-Docuarchi.net.dll" } |
        Copy-Item -Destination $temporaryRoot

    $probePath = Join-Path $temporaryRoot "PolicyProbe.cs"
    $probeExe = Join-Path $temporaryRoot "PolicyProbe.exe"
    @'
using System;
using GestionDocumental_Docuarchi.net;

public static class PolicyProbe
{
    public static int Main(string[] args)
    {
        var context = new ContextoModuloWorkflow {
            IdUsuarioWorkflow = 10,
            IdGrupoWorkflow = 20,
            IdRutaWorkflow = 30,
            LoginUsuario = "doc14-policy-test"
        };
        if (String.Equals(args[2], "invalid", StringComparison.Ordinal))
        {
            context.IdGrupoWorkflow = 0;
        }

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

    function Invoke-PolicyScenario {
        param([string]$Name, [string]$ExpectedState, [string]$ExpectedCode, [string]$ContextKind)

        $output = & $probeExe $ExpectedState $ExpectedCode $ContextKind 2>&1
        if ($LASTEXITCODE -ne 0) { throw "Escenario $Name falló: $output" }
        Write-Output "PASS DOC-14 policy: $Name ($output)"
    }

    Invoke-PolicyScenario "contexto-valido" "activo" "WORKFLOW_MODERN_OFFICIAL" "valid"
    Invoke-PolicyScenario "contexto-invalido" "inactivo" "WORKFLOW_CONTEXT_INVALID" "invalid"
}
finally {
    if (Test-Path -LiteralPath $temporaryRoot) { Remove-Item -LiteralPath $temporaryRoot -Recurse -Force }
}
