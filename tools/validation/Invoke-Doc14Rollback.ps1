[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = "High")]
param(
    [Parameter(Mandatory = $true)]
    [ValidateScript({ Test-Path -LiteralPath $_ -PathType Leaf })]
    [string]$ConfigPath,
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$Responsible,
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$Reason,
    [Parameter(Mandatory = $true)]
    [ValidatePattern("^[A-Za-z0-9-]{8,64}$")]
    [string]$Correlation,
    [Parameter(Mandatory = $true)]
    [string]$EvidencePath
)

$ErrorActionPreference = "Stop"

function Set-AppSetting {
    param([System.Xml.XmlDocument]$Document, [System.Xml.XmlElement]$Settings, [string]$Key, [string]$Value)

    $node = $Settings.SelectSingleNode("add[@key='$Key']")
    if ($null -eq $node) {
        $node = $Document.CreateElement("add")
        [void]$node.SetAttribute("key", $Key)
        [void]$Settings.AppendChild($node)
    }
    [void]$node.SetAttribute("value", $Value)
}

$resolvedConfigPath = (Resolve-Path -LiteralPath $ConfigPath).Path
$timestamp = [DateTime]::UtcNow
$timestampValue = $timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ")
$backupPath = "$resolvedConfigPath.doc14-rollback-$($timestamp.ToString('yyyyMMddHHmmss')).bak"

if (-not $PSCmdlet.ShouldProcess($resolvedConfigPath, "Desactivar el piloto moderno DOC-14 y vaciar su alcance")) {
    return
}

[System.Xml.XmlDocument]$document = New-Object System.Xml.XmlDocument
$document.PreserveWhitespace = $true
$document.Load($resolvedConfigPath)
$settings = $document.SelectSingleNode("/configuration/appSettings")
if ($null -eq $settings) { throw "No se encontró configuration/appSettings en el archivo objetivo." }

Copy-Item -LiteralPath $resolvedConfigPath -Destination $backupPath -ErrorAction Stop
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernActive" "false"
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernOfficialMode" "false"
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernUsers" ""
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernGroups" ""
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernPilotStartUtc" ""
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernPilotOwner" ""
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernPilotReason" ""
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernRollbackUtc" $timestampValue
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernRollbackOwner" $Responsible.Trim()
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernRollbackReason" $Reason.Trim()
Set-AppSetting $document $settings "WorkflowCentroTrabajoModernRollbackCorrelation" $Correlation.Trim()
$document.Save($resolvedConfigPath)

[System.Xml.XmlDocument]$verification = New-Object System.Xml.XmlDocument
$verification.Load($resolvedConfigPath)
$values = @{}
foreach ($key in @("WorkflowCentroTrabajoModernActive", "WorkflowCentroTrabajoModernOfficialMode", "WorkflowCentroTrabajoModernUsers", "WorkflowCentroTrabajoModernGroups", "WorkflowCentroTrabajoModernPilotStartUtc", "WorkflowCentroTrabajoModernPilotOwner", "WorkflowCentroTrabajoModernPilotReason")) {
    $node = $verification.SelectSingleNode("/configuration/appSettings/add[@key='$key']")
    $values[$key] = if ($null -eq $node) { $null } else { $node.GetAttribute("value") }
}
if ($values["WorkflowCentroTrabajoModernActive"] -ne "false" -or
    $values["WorkflowCentroTrabajoModernOfficialMode"] -ne "false" -or
    $values["WorkflowCentroTrabajoModernUsers"] -ne "" -or
    $values["WorkflowCentroTrabajoModernGroups"] -ne "" -or
    $values["WorkflowCentroTrabajoModernPilotStartUtc"] -ne "" -or
    $values["WorkflowCentroTrabajoModernPilotOwner"] -ne "" -or
    $values["WorkflowCentroTrabajoModernPilotReason"] -ne "") {
    throw "La verificación posterior del rollback no dejó el gate y el alcance en estado seguro. Restaure el respaldo: $backupPath"
}

$evidenceDirectory = Split-Path -Parent $EvidencePath
if (-not [string]::IsNullOrWhiteSpace($evidenceDirectory)) { New-Item -ItemType Directory -Path $evidenceDirectory -Force | Out-Null }
$evidence = [PSCustomObject]@{
    Evento = "DOC14_ROLLBACK"
    FechaUtc = $timestampValue
    Responsable = $Responsible.Trim()
    Motivo = $Reason.Trim()
    Correlacion = $Correlation.Trim()
    Gate = "false"
    UsuariosPiloto = 0
    GruposPiloto = 0
    ReversionDeTransiciones = $false
    RespaldoConfiguracion = $backupPath
}
$evidence | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath $EvidencePath -Encoding UTF8
$evidence
