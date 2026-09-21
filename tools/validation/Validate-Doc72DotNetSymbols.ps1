[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$ManifestPath,
    [Parameter(Mandatory=$true)][string]$AssemblyPath
)

$ErrorActionPreference = "Stop"

function Short-TypeName([Type]$type) {
    if ($type.IsByRef) { return "$(Short-TypeName $type.GetElementType())&" }
    if ($type.IsGenericType) {
        $genericName = $type.Name.Split('`')[0]
        $arguments = ($type.GetGenericArguments() | ForEach-Object { Short-TypeName $_ }) -join ','
        return "$genericName<$arguments>"
    }
    if ($type.Namespace -eq 'System') { return $type.FullName }
    return $type.Name
}

$manifest = Get-Content -LiteralPath $ManifestPath -Raw | ConvertFrom-Json
$assembly = [Reflection.Assembly]::LoadFrom((Resolve-Path -LiteralPath $AssemblyPath).Path)
$types = @($assembly.GetTypes())
$errors = New-Object System.Collections.Generic.List[string]

foreach ($property in $manifest.dotnetSymbols.PSObject.Properties) {
    $id = $property.Name
    $expected = $property.Value
    $type = $types | Where-Object Name -eq $expected.type | Select-Object -First 1
    if (-not $type) { $errors.Add("${id}: tipo $($expected.type) inexistente"); continue }
    $flags = [Reflection.BindingFlags]'Public,Instance,Static'
    if ($expected.visibility -eq 'nonpublic') { $flags = $flags -bor [Reflection.BindingFlags]::NonPublic }
    $candidates = @($type.GetMethods($flags) | Where-Object Name -eq $expected.method)
    $matches = @($candidates | Where-Object {
        $actualParameters = @($_.GetParameters() | ForEach-Object { Short-TypeName $_.ParameterType })
        $expectedParameters = @($expected.parameters)
        ($actualParameters.Count -eq $expectedParameters.Count) -and
        ((Compare-Object $actualParameters $expectedParameters -SyncWindow 0).Count -eq 0) -and
        ((Short-TypeName $_.ReturnType) -eq $expected.return)
    })
    if ($matches.Count -ne 1) {
        $available = ($candidates | ForEach-Object { "$($_.Name)(($( $_.GetParameters() | ForEach-Object { Short-TypeName $_.ParameterType }) -join ',')):$(Short-TypeName $_.ReturnType)" }) -join '; '
        $errors.Add("${id}: firma no encontrada o ambigua; disponibles=[$available]")
    }
}

foreach ($property in $manifest.dotnetTypes.PSObject.Properties) {
    $typeName = $property.Name
    $type = $types | Where-Object Name -eq $typeName | Select-Object -First 1
    if (-not $type) { $errors.Add("DTO ${typeName} inexistente"); continue }
    foreach ($expectedProperty in @($property.Value)) {
        $parts = $expectedProperty.Split(':', 2)
        $actual = $type.GetProperty($parts[0], [Reflection.BindingFlags]'Public,Instance,DeclaredOnly')
        if (-not $actual) { $errors.Add("DTO ${typeName}: propiedad $($parts[0]) inexistente"); continue }
        $actualType = Short-TypeName $actual.PropertyType
        if ($actualType -ne $parts[1]) { $errors.Add("DTO $typeName.$($parts[0]): tipo $actualType, esperado $($parts[1])") }
    }
}

if ($errors.Count -gt 0) { throw ($errors -join [Environment]::NewLine) }
Write-Output "DOC-72 .NET symbols: PASS ($(@($manifest.dotnetSymbols.PSObject.Properties).Count) firmas, $(@($manifest.dotnetTypes.PSObject.Properties).Count) tipos)."
