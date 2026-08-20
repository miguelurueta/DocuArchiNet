[CmdletBinding()]
param(
    [string]$PackageVersion
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($PackageVersion)) {
    throw "El rollback por gate DOC-14 fue retirado. Indique la versión de paquete que se debe restaurar."
}

Write-Output "DOC-14 no modifica configuración ni deshabilita usuarios. Restaure el paquete $($PackageVersion.Trim()) mediante el procedimiento de despliegue aprobado."
