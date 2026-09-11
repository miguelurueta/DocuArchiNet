param([long]$TaskId = 219888)

$ErrorActionPreference = 'Stop'
$dsn = 'workflowdocument'

$user = Read-Host 'Usuario MySQL de solo lectura'
if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC56_INTENT_LOOKUP_USER_REQUIRED' }
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
    $command = $connection.CreateCommand()
    try {
        $command.CommandText = 'SELECT intent.intent_id,item.client_item_id,item.status,item.document_type_id,item.document_type_name,item.error_code,item.persistence_known,item.document_id FROM workflow_import_intent intent INNER JOIN workflow_import_intent_item item ON item.intent_id=intent.intent_id WHERE intent.task_id=? ORDER BY intent.created_utc DESC LIMIT 1'
        [void]$command.Parameters.Add('@taskId', [System.Data.Odbc.OdbcType]::BigInt)
        $command.Parameters[0].Value = $TaskId
        $reader = $command.ExecuteReader()
        try {
            if (-not $reader.Read()) { throw 'DOC56_INTENT_LOOKUP_NOT_FOUND' }
            Write-Host ('DOC56_INTENT_ID=' + [string]$reader['intent_id'])
            Write-Host ('DOC56_INTENT_ITEM=' + [string]$reader['client_item_id'])
            Write-Host ('DOC56_INTENT_STATUS=' + [string]$reader['status'])
            Write-Host ('DOC56_DOCUMENT_TYPE_ID=' + [string]$reader['document_type_id'])
            Write-Host ('DOC56_DOCUMENT_TYPE_NAME=' + [string]$reader['document_type_name'])
            Write-Host ('DOC56_ERROR_CODE=' + [string]$reader['error_code'])
            Write-Host ('DOC56_PERSISTENCE_KNOWN=' + [string]$reader['persistence_known'])
            Write-Host ('DOC56_DOCUMENT_ID_PRESENT=' + (-not [Convert]::IsDBNull($reader['document_id'])))
        } finally { $reader.Dispose() }
    } finally { $command.Dispose() }
} catch {
    $safeCode = if ($_.Exception.Message -match '^DOC56_INTENT_LOOKUP_[A-Z_]+$') { $_.Exception.Message } else { 'DOC56_INTENT_LOOKUP_FAILED' }
    Write-Error "$safeCode. No se mostraron credenciales ni detalles de conexión."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $password = $null
    $securePassword = $null
}
