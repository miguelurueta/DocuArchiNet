[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
$testRoot = Join-Path $repoRoot "Tests"
$tests = Get-ChildItem -LiteralPath $testRoot -Filter "importar-servicio-web-*.test.cjs" -File |
    Sort-Object FullName |
    Select-Object -ExpandProperty FullName

if ($tests.Count -eq 0) {
    throw "No se encontraron pruebas de ImportarServicioWeb."
}

Push-Location $repoRoot
try {
    & node --test @tests
    if ($LASTEXITCODE -ne 0) {
        throw "La suite ImportarServicioWeb fallo con codigo $LASTEXITCODE."
    }
    Write-Host "ImportarServicioWeb: $($tests.Count) archivos de prueba completados."
}
finally {
    Pop-Location
}
