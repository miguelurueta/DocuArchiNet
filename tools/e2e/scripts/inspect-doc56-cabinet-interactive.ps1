param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[a-fA-F0-9]{32}$')]
    [string]$IntentId,

    [Parameter(Mandatory = $true)]
    [ValidateRange(1, [long]::MaxValue)]
    [long]$TaskId,

    [ValidateRange(1, [long]::MaxValue)]
    [long]$CompareDocumentId = 9624
)

$ErrorActionPreference = 'Stop'
$dsn = 'workflowdocument'

function Add-Parameter([System.Data.Odbc.OdbcCommand]$Command, [object]$Value) {
    $parameter = $Command.CreateParameter()
    $parameter.Value = if ($null -eq $Value) { [DBNull]::Value } else { $Value }
    [void]$Command.Parameters.Add($parameter)
}

function Read-One([System.Data.Odbc.OdbcConnection]$Connection, [string]$Sql, [object[]]$Values) {
    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $Sql
        foreach ($value in $Values) { Add-Parameter $command $value }
        $reader = $command.ExecuteReader()
        try {
            if (-not $reader.Read()) { return $null }
            $row = [ordered]@{}
            for ($i = 0; $i -lt $reader.FieldCount; $i++) {
                $row[$reader.GetName($i)] = if ($reader.IsDBNull($i)) { $null } else { $reader.GetValue($i) }
            }
            return [pscustomobject]$row
        } finally { $reader.Dispose() }
    } finally { $command.Dispose() }
}

function Assert-Identifier([string]$Value, [string]$Code) {
    if ([string]::IsNullOrWhiteSpace($Value) -or $Value -notmatch '^[A-Za-z0-9_]+$') { throw $Code }
    return $Value
}

$confirmation = Read-Host 'Inspeccion directa del gabinete DOC-56 exclusivamente SELECT en PRUEBAS. Escriba SI para continuar'
if ($confirmation -cne 'SI') { throw 'DOC56_CABINET_INSPECTION_NOT_AUTHORIZED' }
$user = Read-Host 'Usuario MySQL de solo lectura'
if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC56_CABINET_USER_REQUIRED' }
$securePassword = Read-Host 'Contrasena MySQL de solo lectura' -AsSecureString
$passwordPointer = [IntPtr]::Zero
$connection = $null
try {
    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $dsn
    $builder['UID'] = $user.Trim()
    $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()

    $intent = Read-One $connection 'SELECT route_id,document_id,radicado FROM workflow_import_intent intent INNER JOIN workflow_import_intent_item item ON item.intent_id=intent.intent_id WHERE intent.intent_id=? AND intent.task_id=? LIMIT 1' @($IntentId, $TaskId)
    if ($null -eq $intent) { throw 'DOC56_CABINET_INTENT_NOT_FOUND' }
    if ($null -eq $intent.document_id -or [long]$intent.document_id -le 0) { throw 'DOC56_CABINET_DOCUMENT_ID_MISSING' }

    $route = Read-One $connection 'SELECT Nombre_Ruta FROM rutas_workflow WHERE ID_RUTA=? LIMIT 1' @([int]$intent.route_id)
    if ($null -eq $route) { throw 'DOC56_CABINET_ROUTE_NOT_FOUND' }
    $routeName = Assert-Identifier ([string]$route.Nombre_Ruta) 'DOC56_CABINET_ROUTE_INVALID'
    $taskTable = Assert-Identifier ('dat_adic_tar' + $routeName) 'DOC56_CABINET_TASK_TABLE_INVALID'
    $task = Read-One $connection ("SELECT ID_GABINETE,ID_IMAGEN FROM ``$taskTable`` WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=? LIMIT 1") @($TaskId)
    if ($null -eq $task) { throw 'DOC56_CABINET_TASK_NOT_FOUND' }

    $configuration = Read-One $connection 'SELECT NOMBRE_GABINETE,BASE_DATOS FROM CONFIGURACION_GABINETE WHERE ID_GABINETE=? LIMIT 1' @([int]$task.ID_GABINETE)
    if ($null -eq $configuration) { throw 'DOC56_CABINET_CONFIGURATION_NOT_FOUND' }
    $databaseName = Assert-Identifier ([string]$configuration.BASE_DATOS) 'DOC56_CABINET_DATABASE_INVALID'
    $cabinetName = Assert-Identifier ([string]$configuration.NOMBRE_GABINETE) 'DOC56_CABINET_NAME_INVALID'
    $qualifiedCabinet = "``$databaseName``.``$cabinetName``"

    $main = Read-One $connection ("SELECT ID,DISC,PAG,IDEX,ENLASE,DBT FROM $qualifiedCabinet WHERE ID=? LIMIT 1") @([long]$task.ID_IMAGEN)
    $columns = Read-One $connection "SELECT GROUP_CONCAT(COLUMN_NAME ORDER BY ORDINAL_POSITION SEPARATOR ',') AS cols FROM information_schema.columns WHERE TABLE_SCHEMA=? AND TABLE_NAME=? AND COLUMN_NAME IN ('ID','DISC','PAG','IDEX','ENLASE','DBT','CODBARRAS','RECIBOCAJA','MATRICULA','PROPONENTE','RAZONSOCIAL','NITCEDULA','LIBRO','INSCRIPCION','FECHAINSCRIP','FECHAREGISTR','ACTO','DESCRIPCIONA','DESCRIACTO','ID_AREA','ID_SERIE','ID_SUB_SERIE','ID_TIPODOCUMENTO','TIPODOCUMENTO','ID_CLASE_DOCUMENTO','CLASEDOCUMENTO','ID_EXPEDIENTE')" @($databaseName,$cabinetName)
    if ($null -eq $columns -or [string]::IsNullOrWhiteSpace([string]$columns.cols)) { throw 'DOC56_CABINET_COLUMNS_NOT_FOUND' }
    $indexColumns = [string]$columns.cols
    $imported = Read-One $connection ("SELECT $indexColumns FROM $qualifiedCabinet WHERE ID=? LIMIT 1") @([long]$intent.document_id)
    $compared = Read-One $connection ("SELECT $indexColumns FROM $qualifiedCabinet WHERE ID=? LIMIT 1") @($CompareDocumentId)
    $viewerCount = 0
    if ($null -ne $main) {
        $count = Read-One $connection ("SELECT COUNT(*) AS total FROM $qualifiedCabinet WHERE ENLASE=?") @([string]$main.ENLASE)
        $viewerCount = [int]$count.total
    }

    Write-Host ('DOC56_CABINET_TASK_ID={0}' -f $TaskId)
    Write-Host ('DOC56_CABINET_NAME={0}' -f $cabinetName)
    Write-Host ('DOC56_CABINET_DOCUMENT_ID={0}' -f [long]$intent.document_id)
    Write-Host ('DOC56_CABINET_MAIN_DOCUMENT_ID={0}' -f [long]$task.ID_IMAGEN)
    Write-Host ('DOC56_CABINET_MAIN_ROW_EXISTS={0}' -f ($null -ne $main))
    Write-Host ('DOC56_CABINET_IMPORTED_ROW_EXISTS={0}' -f ($null -ne $imported))
    Write-Host ('DOC56_CABINET_MAIN_LINK={0}' -f $(if ($null -eq $main) { '[MISSING]' } else { [string]$main.ENLASE }))
    Write-Host ('DOC56_CABINET_IMPORTED_LINK={0}' -f $(if ($null -eq $imported) { '[MISSING]' } else { [string]$imported.ENLASE }))
    Write-Host ('DOC56_CABINET_LINK_MATCH={0}' -f ($null -ne $main -and $null -ne $imported -and [string]$main.ENLASE -eq [string]$imported.ENLASE))
    Write-Host ('DOC56_CABINET_VIEWER_QUERY_COUNT={0}' -f $viewerCount)
    foreach ($rowName in @('COMPARED','IMPORTED')) {
        $row = if ($rowName -eq 'COMPARED') { $compared } else { $imported }
        if ($null -eq $row) { Write-Host ("DOC56_CABINET_${rowName}_MISSING=True"); continue }
        foreach ($property in $row.psobject.Properties) {
            $value = if ($null -eq $property.Value) { '[NULL]' } else { [string]$property.Value }
            Write-Host ("DOC56_CABINET_${rowName}_{0}={1}" -f $property.Name.ToUpperInvariant(), $value)
        }
    }
    Write-Host 'DOC56_CABINET_INSPECTION_COMPLETE'
} catch {
    $safeCode = if ($_.Exception.Message -match '^DOC56_CABINET_[A-Z_]+$') { $_.Exception.Message } else { 'DOC56_CABINET_INSPECTION_FAILED' }
    Write-Error "$safeCode. No se mostraron credenciales ni datos de conexion."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $password = $null
    $securePassword = $null
}
