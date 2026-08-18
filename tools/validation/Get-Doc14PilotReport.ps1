[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateScript({ Test-Path -LiteralPath $_ -PathType Leaf })]
    [string]$InputPath,
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

function Get-EventValue {
    param([object]$Event, [string]$Name, [object]$Fallback)

    $property = $Event.PSObject.Properties[$Name]
    if ($null -eq $property -or $null -eq $property.Value) { return $Fallback }
    return $property.Value
}

function Normalize-Channel {
    param([object]$Value)

    switch ("$Value".Trim().ToUpperInvariant()) {
        "MODERNO" { return "MODERNO" }
        "LEGACY" { return "LEGACY" }
        default { return "DESCONOCIDO" }
    }
}

function Normalize-Result {
    param([object]$Value)

    switch ("$Value".Trim().ToUpperInvariant()) {
        "EXITO" { return "EXITO" }
        "BLOQUEADO" { return "BLOQUEADO" }
        "ERROR" { return "ERROR" }
        default { return "ERROR" }
    }
}

function To-NonNegativeInt64 {
    param([object]$Value)

    [Int64]$parsed = 0
    if (-not [Int64]::TryParse("$Value", [ref]$parsed)) { return [Int64]0 }
    return [Math]::Max([Int64]0, $parsed)
}

function To-Boolean {
    param([object]$Value)

    return $Value -is [bool] -and $Value
}

$raw = Get-Content -LiteralPath $InputPath -Raw
$parsedEvents = $raw | ConvertFrom-Json
$inputEvents = if ($parsedEvents -is [System.Array]) {
    @($parsedEvents | ForEach-Object { $_ })
} else {
    @($parsedEvents)
}
$events = foreach ($event in $inputEvents) {
    if ($null -eq $event) { continue }

    [PSCustomObject]@{
        Canal = Normalize-Channel (Get-EventValue $event "Canal" "DESCONOCIDO")
        Resultado = Normalize-Result (Get-EventValue $event "Resultado" "ERROR")
        DuracionMilisegundos = To-NonNegativeInt64 (Get-EventValue $event "DuracionMilisegundos" 0)
        Abandonado = To-Boolean (Get-EventValue $event "Abandonado" $false)
        Divergencia = To-Boolean (Get-EventValue $event "Divergencia" $false)
        TransicionDuplicada = To-Boolean (Get-EventValue $event "TransicionDuplicada" $false)
        PerdidaContexto = To-Boolean (Get-EventValue $event "PerdidaContexto" $false)
        FiltracionSensible = To-Boolean (Get-EventValue $event "FiltracionSensible" $false)
        FalloAutorizacion = To-Boolean (Get-EventValue $event "FalloAutorizacion" $false)
        FalloRollback = To-Boolean (Get-EventValue $event "FalloRollback" $false)
    }
}

$channels = foreach ($group in @($events | Group-Object -Property Canal | Sort-Object Name)) {
    $items = @($group.Group)
    $durations = @($items | ForEach-Object { $_.DuracionMilisegundos } | Sort-Object)
    $durationAverage = if ($durations.Count -eq 0) { [Int64]0 } else { [Math]::Round((($durations | Measure-Object -Sum).Sum / $durations.Count), 2) }
    $p95Index = if ($durations.Count -eq 0) { 0 } else { [Math]::Ceiling($durations.Count * 0.95) - 1 }
    $durationP95 = if ($durations.Count -eq 0) { [Int64]0 } else { $durations[$p95Index] }
    $critical = @($items | Where-Object {
        $_.TransicionDuplicada -or $_.PerdidaContexto -or $_.FiltracionSensible -or $_.FalloAutorizacion -or $_.FalloRollback
    }).Count

    [PSCustomObject]@{
        Canal = $group.Name
        Volumen = $items.Count
        Exitos = @($items | Where-Object { $_.Resultado -eq "EXITO" }).Count
        Bloqueos = @($items | Where-Object { $_.Resultado -eq "BLOQUEADO" }).Count
        Errores = @($items | Where-Object { $_.Resultado -eq "ERROR" }).Count
        DuracionPromedioMs = $durationAverage
        DuracionP95Ms = $durationP95
        Abandonos = @($items | Where-Object { $_.Abandonado }).Count
        Divergencias = @($items | Where-Object { $_.Divergencia }).Count
        EventosCriticos = $critical
        EstadoPromocion = if ($critical -gt 0) { "BLOQUEADO" } else { "PENDIENTE_APROBACION" }
    }
}

$criticalTotal = @($events | Where-Object {
    $_.TransicionDuplicada -or $_.PerdidaContexto -or $_.FiltracionSensible -or $_.FalloAutorizacion -or $_.FalloRollback
}).Count
$report = [PSCustomObject]@{
    Esquema = "doc14-pilot-report-v1"
    GeneradoUtc = [DateTime]::UtcNow.ToString("o")
    VolumenTotal = @($events).Count
    EventosCriticos = $criticalTotal
    EstadoPromocion = if ($criticalTotal -gt 0) { "BLOQUEADO" } else { "PENDIENTE_APROBACION" }
    Canales = @($channels)
    Nota = "Reporte agregado; no incluye identidades, tareas, referencias, tokens, documentos ni payloads."
}

$json = $report | ConvertTo-Json -Depth 5
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $json
} else {
    Set-Content -LiteralPath $OutputPath -Value $json -Encoding UTF8
}
