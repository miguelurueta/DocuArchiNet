param(
    [Parameter(Mandatory = $true)]
    [ValidateRange(1, [long]::MaxValue)]
    [long]$TaskId
)

$ErrorActionPreference = 'Stop'
$dsn = 'workflowdocument'
$safeResults = [System.Collections.Generic.List[string]]::new()
$safeResultPath = Join-Path $PSScriptRoot '..\artifacts\doc56-reconciliation-inspection.txt'
$recoveryProfilePath = Join-Path $PSScriptRoot '..\profiles\doc56-import-sii-recovery.runtime.json'

function Invoke-Probe {
    param(
        [System.Data.Odbc.OdbcConnection]$Connection,
        [string]$Name,
        [string]$Sql,
        [object[]]$Values = @()
    )
    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $Sql
        foreach ($value in $Values) {
            $parameter = $command.CreateParameter()
            $parameter.Value = $value
            [void]$command.Parameters.Add($parameter)
        }
        $reader = $command.ExecuteReader()
        try { while ($reader.Read()) { } } finally { $reader.Dispose() }
        $code = "DOC56_RECONCILIATION_{0}_OK" -f $Name
        $script:safeResults.Add($code)
        Write-Host $code
        return $true
    } catch {
        $code = "DOC56_RECONCILIATION_{0}_FAILED" -f $Name
        $script:safeResults.Add($code)
        Write-Host $code
        return $false
    } finally {
        $command.Dispose()
    }
}

$confirmation = Read-Host 'Inspección DOC-56 exclusivamente SELECT en PRUEBAS. Escriba SI para continuar'
if ($confirmation -cne 'SI') { throw 'DOC56_RECONCILIATION_INSPECTION_NOT_AUTHORIZED' }
$user = Read-Host 'Usuario MySQL de solo lectura'
if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC56_RECONCILIATION_USER_REQUIRED' }
$securePassword = Read-Host 'Contraseña MySQL de solo lectura' -AsSecureString
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

    $intentSql = 'SELECT intent_id,user_id FROM workflow_import_intent WHERE task_id=? ORDER BY created_utc DESC LIMIT 1'
    $intentCommand = $connection.CreateCommand()
    try {
        $intentCommand.CommandText = $intentSql
        $taskParameter = $intentCommand.CreateParameter()
        $taskParameter.Value = $taskId
        [void]$intentCommand.Parameters.Add($taskParameter)
        $intentReader = $intentCommand.ExecuteReader()
        try {
            if (-not $intentReader.Read()) { throw 'DOC56_RECONCILIATION_INTENT_NOT_FOUND' }
            $intentId = [string]$intentReader.GetValue(0)
            $userId = [int]$intentReader.GetValue(1)
        } finally { $intentReader.Dispose() }
    } finally { $intentCommand.Dispose() }

    [void](Invoke-Probe $connection 'BASE' 'SELECT intent.intent_id,item.client_item_id,item.document_id FROM workflow_import_intent intent INNER JOIN workflow_import_intent_item item ON item.intent_id=intent.intent_id WHERE intent.intent_id=? AND intent.user_id=? AND intent.task_id=?' @($intentId,$userId,$taskId))
    [void](Invoke-Probe $connection 'DOCUMENT_NAME' 'SELECT (SELECT rpd.SEGUNDO_NOMBRE_DOCUMENTO FROM registro_producion_documental rpd WHERE rpd.ID_DOCUMENTO_DOCUARCHI_ALMACEN=item.document_id LIMIT 1) FROM workflow_import_intent_item item WHERE item.intent_id=?' @($intentId))
    [void](Invoke-Probe $connection 'DOCUMENT_COUNT' 'SELECT (SELECT COUNT(*) FROM registro_producion_documental document_record WHERE document_record.ID_DOCUMENTO_DOCUARCHI_ALMACEN=item.document_id) FROM workflow_import_intent_item item WHERE item.intent_id=?' @($intentId))
    [void](Invoke-Probe $connection 'RELATION_COUNT' 'SELECT (SELECT COUNT(*) FROM workflow_import_intent_item same_item WHERE same_item.intent_id=item.intent_id AND same_item.document_id=item.document_id AND same_item.target_task_id=intent.task_id) FROM workflow_import_intent intent INNER JOIN workflow_import_intent_item item ON item.intent_id=intent.intent_id WHERE intent.intent_id=?' @($intentId))
    [void](Invoke-Probe $connection 'OTHER_TASK_COUNT' 'SELECT (SELECT COUNT(*) FROM workflow_import_intent_item other_item WHERE other_item.document_id=item.document_id AND other_item.target_task_id<>intent.task_id) FROM workflow_import_intent intent INNER JOIN workflow_import_intent_item item ON item.intent_id=intent.intent_id WHERE intent.intent_id=?' @($intentId))
    $safeResults.Add('DOC56_RECONCILIATION_INSPECTION_COMPLETE')
    [System.IO.File]::WriteAllLines([System.IO.Path]::GetFullPath($safeResultPath), $safeResults)
    $recoveryProfile = [ordered]@{
        scenarioId = 'import-sii-recovery'
        baseUrl = 'https://localhost/GestionDocumental-Docuarchi.net/'
        module = 'WORKFLOW REGISTRO'
        environment = 'CERTIFICACION'
        odbcDsn = $dsn
        taskId = $taskId
        radicado = 'S002188378'
        codigoBarras = '18221381'
        intentId = $intentId
        sampleSize = 1
        budgetMs = 60000
        browser = @{ channel = 'chrome' }
        ignoreHttpsErrors = $true
    }
    [System.IO.File]::WriteAllText([System.IO.Path]::GetFullPath($recoveryProfilePath), ($recoveryProfile | ConvertTo-Json -Depth 3))
    Write-Host 'DOC56_RECONCILIATION_INSPECTION_COMPLETE'
} catch {
    $safeCode = if ($_.Exception.Message -match '^DOC56_RECONCILIATION_[A-Z_]+$') { $_.Exception.Message } else { 'DOC56_RECONCILIATION_INSPECTION_FAILED' }
    [System.IO.File]::WriteAllLines([System.IO.Path]::GetFullPath($safeResultPath), @($safeCode))
    Write-Error "$safeCode. No se mostraron credenciales, identificadores ni datos."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $password = $null
    $securePassword = $null
}
