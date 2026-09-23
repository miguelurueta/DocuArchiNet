[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$tests = @(
    'tests/importar-servicio-web-ui-architecture.test.cjs',
    'tests/importar-servicio-web-gate.test.cjs',
    'tests/importar-servicio-web-legacy-ui-regression.test.cjs',
    'tests/importar-servicio-web-storage-invariance-ui.test.cjs',
    'tests/importar-servicio-web-progress-adapter.test.cjs',
    'tests/importar-servicio-web-progress-state-mapping.test.cjs',
    'tests/importar-servicio-web-reconciliation-ui.test.cjs',
    'tests/importar-servicio-web-task-context-guard.test.cjs'
)

Push-Location -LiteralPath $repositoryRoot
try {
    & node --test @tests
    if ($LASTEXITCODE -ne 0) { throw "IMPORTAR_SERVICIO_WEB_FRONTEND_VALIDATION_FAILED ($LASTEXITCODE)" }
}
finally {
    Pop-Location
}
