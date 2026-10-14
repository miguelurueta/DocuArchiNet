param(
    [string]$AssemblyPath = "../../../bin/GestionDocumental-Docuarchi.net.dll"
)

$ErrorActionPreference = "Stop"
$resolvedAssembly = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot $AssemblyPath))
if (-not (Test-Path -LiteralPath $resolvedAssembly)) {
    throw "DOC104_ASSEMBLY_NOT_FOUND"
}

Add-Type -AssemblyName System.Drawing
$assembly = [Reflection.Assembly]::LoadFrom($resolvedAssembly)

$source = [Drawing.Bitmap]::new(3, 1, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
$resultBitmap = $null
try {
    $source.SetPixel(0, 0, [Drawing.Color]::FromArgb(255, 255, 255, 255))
    $source.SetPixel(1, 0, [Drawing.Color]::FromArgb(201, 250, 250, 250))
    $source.SetPixel(2, 0, [Drawing.Color]::FromArgb(177, 220, 220, 220))

    $processorType = $assembly.GetType("GestionDocumental_Docuarchi.net.Class_transparent", $true)
    $processor = [Activator]::CreateInstance($processorType)
    $arguments = [object[]]@($source, [Drawing.Color]::White, 10, "")
    $resultBitmap = $processorType.GetMethod("Transparent").Invoke($processor, $arguments)
    if ($arguments[3] -ne "YES" -or $null -eq $resultBitmap) { throw "DOC104_TRANSPARENCY_FAILED" }
    if ($resultBitmap.GetPixel(0, 0).A -ne 0) { throw "DOC104_EXACT_WHITE_NOT_TRANSPARENT" }
    if ($resultBitmap.GetPixel(1, 0).A -ne 0) { throw "DOC104_TOLERANCE_NOT_APPLIED" }
    if ($resultBitmap.GetPixel(2, 0).A -ne 177) { throw "DOC104_ALPHA_NOT_PRESERVED" }
    if ($resultBitmap.GetPixel(2, 0).R -ne 220) { throw "DOC104_COLOR_NOT_PRESERVED" }
    "DOC104_TRANSPARENCY_RESULT=PASSED"
}
finally {
    if ($null -ne $resultBitmap) { $resultBitmap.Dispose() }
    $source.Dispose()
}
