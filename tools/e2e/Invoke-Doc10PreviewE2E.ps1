[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [Uri]$BaseUri,

    [Parameter(Mandatory = $true)]
    [string]$ModuleValue,

    [Parameter(Mandatory = $true)]
    [System.Management.Automation.PSCredential]$AuthorizedCredential,

    [Parameter(Mandatory = $true)]
    [System.Management.Automation.PSCredential]$UnauthorizedCredential,

    [Parameter(Mandatory = $true)]
    [Int64]$IdTarea,

    [Parameter(Mandatory = $true)]
    [string]$ReadOnlyConnectionString,

    [Parameter(Mandatory = $true)]
    [string]$AuditProbeSql,

    [Parameter(Mandatory = $true)]
    [string]$EvidencePath,

    [string]$TaskStateProbeSql = "SELECT ID_ESTADO, INICIO_TAREAS_WORKFLOW_ID_TAREA, ID_ACTIVIDAD, FECHA_INICIO, FECHA_SELECCION, FECHA_FIN, ESTADO_TAREA, ID_USUARIO, ID_FLUJO_TRABAJO, ID_ACTIVIDAD_FLUJO_TRABAJO FROM estados_tarea_workflow WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea ORDER BY ID_ESTADO",

    [string]$MySqlAssemblyPath
)

$ErrorActionPreference = "Stop"
$BaseUri = [Uri]::new($BaseUri.AbsoluteUri.TrimEnd('/') + '/')

function Get-HtmlAttribute {
    param([string]$Tag, [string]$Name)
    $match = [regex]::Match($Tag, '(?is)\b' + [regex]::Escape($Name) + '\s*=\s*([''"])(.*?)\1')
    if ($match.Success) { return [System.Net.WebUtility]::HtmlDecode($match.Groups[2].Value) }
    return $null
}

function Get-ControlName {
    param([string]$Html, [string]$Id)
    $pattern = '(?is)<(?:input|select)\b(?=[^>]*\bid\s*=\s*([''"])' + [regex]::Escape($Id) + '\1)[^>]*>'
    $match = [regex]::Match($Html, $pattern)
    if (-not $match.Success) { throw "No se encontro el control de login '$Id'." }
    $name = Get-HtmlAttribute -Tag $match.Value -Name "name"
    if ([string]::IsNullOrWhiteSpace($name)) { throw "El control de login '$Id' no tiene nombre de formulario." }
    return $name
}

function Get-HiddenFields {
    param([string]$Html)
    $fields = [ordered]@{}
    foreach ($tagMatch in [regex]::Matches($Html, '(?is)<input\b(?=[^>]*\btype\s*=\s*([''"])hidden\1)[^>]*>')) {
        $name = Get-HtmlAttribute -Tag $tagMatch.Value -Name "name"
        if (-not [string]::IsNullOrWhiteSpace($name)) {
            $fields[$name] = Get-HtmlAttribute -Tag $tagMatch.Value -Name "value"
        }
    }
    return $fields
}

function New-AuthenticatedSession {
    param([System.Management.Automation.PSCredential]$Credential)

    $loginUri = [Uri]::new($BaseUri, "gestor.aspx")
    $loginPage = Invoke-WebRequest -Uri $loginUri -SessionVariable loginSession -UseBasicParsing
    $form = Get-HiddenFields -Html $loginPage.Content
    $form[(Get-ControlName -Html $loginPage.Content -Id "ContentPlacenter_DropDownListmodulos")] = $ModuleValue
    $form[(Get-ControlName -Html $loginPage.Content -Id "ContentPlacenter_TextBoxuser")] = $Credential.UserName
    $form[(Get-ControlName -Html $loginPage.Content -Id "ContentPlacenter_TextBoxpasw")] = $Credential.GetNetworkCredential().Password
    $form[(Get-ControlName -Html $loginPage.Content -Id "ContentPlacenter_Buttonaceptar")] = ""

    $loginResult = Invoke-WebRequest -Uri $loginUri -Method Post -Body $form -WebSession $loginSession -UseBasicParsing
    if ($loginResult.BaseResponse.ResponseUri.AbsolutePath -match "(?i)(?:gestor|login)\.aspx$" -and
        $loginResult.Content -match "(?i)INICIAR SESI[ÓO]N") {
        throw "No fue posible autenticar el usuario de prueba '$($Credential.UserName)'."
    }

    return $loginSession
}

function Assert-ReadOnlySql {
    param([string]$Sql, [string]$Name)
    if ($Sql -notmatch "(?is)^\s*SELECT\b" -or $Sql -match ";|(?i)\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER)\b") {
        throw "$Name debe ser una unica consulta SELECT de solo lectura."
    }
    if ($Sql -notmatch "@idTarea") {
        throw "$Name debe filtrar por el parametro @idTarea."
    }
}

function Get-MySqlRows {
    param([string]$Sql)

    $connection = New-Object MySql.Data.MySqlClient.MySqlConnection($ReadOnlyConnectionString)
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = $Sql
        $parameter = $command.Parameters.Add("@idTarea", [MySql.Data.MySqlClient.MySqlDbType]::Int64)
        $parameter.Value = $IdTarea
        $reader = $command.ExecuteReader()
        try {
            $rows = @()
            while ($reader.Read()) {
                $row = [ordered]@{}
                for ($index = 0; $index -lt $reader.FieldCount; $index++) {
                    $value = $reader.GetValue($index)
                    $row[$reader.GetName($index)] = if ($value -is [System.DBNull]) { $null } else { [string]$value }
                }
                $rows += [pscustomobject]$row
            }
            return @($rows)
        }
        finally { $reader.Dispose() }
    }
    finally { $connection.Dispose() }
}

function Get-Fingerprint {
    param([object]$Rows)
    $json = ConvertTo-Json -InputObject $Rows -Depth 8 -Compress
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($json)
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    try { return ([System.BitConverter]::ToString($sha256.ComputeHash($bytes))).Replace("-", "") }
    finally { $sha256.Dispose() }
}

function Invoke-Preview {
    param([Microsoft.PowerShell.Commands.WebRequestSession]$Session)

    $endpointUri = [Uri]::new($BaseUri, "webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea")
    $payload = @{ idTarea = $IdTarea } | ConvertTo-Json -Compress
    $response = Invoke-WebRequest -Uri $endpointUri -Method Post -ContentType "application/json; charset=utf-8" -Headers @{ "X-Requested-With" = "XMLHttpRequest" } -Body $payload -WebSession $Session -UseBasicParsing
    $json = $response.Content | ConvertFrom-Json
    if ($null -eq $json.d) { throw "El ASMX no devolvio el contenedor JSON esperado." }
    return $json.d
}

Assert-ReadOnlySql -Sql $TaskStateProbeSql -Name "TaskStateProbeSql"
Assert-ReadOnlySql -Sql $AuditProbeSql -Name "AuditProbeSql"

if ([string]::IsNullOrWhiteSpace($MySqlAssemblyPath)) {
    $MySqlAssemblyPath = Join-Path (Split-Path -Parent $PSScriptRoot) "..\bin\MySql.Data.dll"
}
$MySqlAssemblyPath = [IO.Path]::GetFullPath($MySqlAssemblyPath)
if (-not (Test-Path -LiteralPath $MySqlAssemblyPath)) { throw "No existe MySql.Data.dll: $MySqlAssemblyPath" }
Add-Type -Path $MySqlAssemblyPath

$beforeTask = Get-Fingerprint (Get-MySqlRows -Sql $TaskStateProbeSql)
$beforeAudit = Get-Fingerprint (Get-MySqlRows -Sql $AuditProbeSql)
$authorizedSession = New-AuthenticatedSession -Credential $AuthorizedCredential
$authorized = Invoke-Preview -Session $authorizedSession
$unauthorizedSession = New-AuthenticatedSession -Credential $UnauthorizedCredential
$unauthorized = Invoke-Preview -Session $unauthorizedSession
$afterTask = Get-Fingerprint (Get-MySqlRows -Sql $TaskStateProbeSql)
$afterAudit = Get-Fingerprint (Get-MySqlRows -Sql $AuditProbeSql)

if ($null -ne $authorized.Error -or @($authorized.Destinos).Count -lt 1) {
    throw "El usuario autorizado no recibio destinos validos en el preview."
}
if ($null -eq $unauthorized.Error -or $unauthorized.Error.Codigo -ne "WORKFLOW_MODERN_INACTIVE" -or @($unauthorized.Destinos).Count -ne 0) {
    throw "El usuario fuera del piloto no recibio el bloqueo fail-closed esperado."
}
if ($beforeTask -ne $afterTask -or $beforeAudit -ne $afterAudit) {
    throw "La E2E detecto una mutacion de tarea, estado o auditoria."
}

$evidenceDirectory = Split-Path -Parent $EvidencePath
if (-not [string]::IsNullOrWhiteSpace($evidenceDirectory)) {
    New-Item -ItemType Directory -Path $evidenceDirectory -Force | Out-Null
}
$evidence = [ordered]@{
    fechaUtc = [DateTime]::UtcNow.ToString("o")
    endpoint = ([Uri]::new($BaseUri, "webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea")).AbsoluteUri
    idTarea = $IdTarea
    autorizado = [ordered]@{ destinos = @($authorized.Destinos).Count; tipoDecision = $authorized.TipoDecision; bloqueo = $null }
    noAutorizado = [ordered]@{ destinos = @($unauthorized.Destinos).Count; bloqueo = $unauthorized.Error.Codigo }
    estadoSinMutacion = $true
    auditoriaSinMutacion = $true
}
$evidence | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $EvidencePath -Encoding UTF8
Write-Output "PASS DOC-10 E2E: preview autorizado, bloqueo no autorizado y ausencia de mutacion comprobados."
