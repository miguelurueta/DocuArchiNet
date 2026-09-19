param([string]$Dsn = 'workflowdocument')

$ErrorActionPreference = 'Stop'
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..'))
$migrationPath = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot 'Doc\Actualizacion\workflow\ImportarServicioWeb\DOC-69-lista-preview-incripciones\Sql\001-create-workflow-import-preview-descriptor-mysql51.sql'))

function Invoke-Scalar([System.Data.Odbc.OdbcConnection]$Connection, [string]$Sql) {
    $command = $Connection.CreateCommand()
    try { $command.CommandText = $Sql; return $command.ExecuteScalar() } finally { $command.Dispose() }
}

$confirmation = Read-Host 'Aplicar DOC-69 en workflowdocument de CERTIFICACION. Escriba SI'
if ($confirmation -cne 'SI') { throw 'DOC69_SCHEMA_NOT_AUTHORIZED' }
$user = Read-Host 'Usuario MySQL autorizado para migraciones'
if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC69_SCHEMA_USER_REQUIRED' }
$securePassword = Read-Host 'Contraseña MySQL' -AsSecureString
$passwordPointer = [IntPtr]::Zero
$connection = $null
$stage = 'CONNECT'
try {
    if (-not $migrationPath.StartsWith($repositoryRoot, [StringComparison]::OrdinalIgnoreCase) -or -not (Test-Path -LiteralPath $migrationPath)) { throw 'DOC69_SCHEMA_MIGRATION_MISSING' }
    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $Dsn; $builder['UID'] = $user.Trim(); $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()
    if ($connection.Database -ine 'workflowdocument') { throw 'DOC69_SCHEMA_DATABASE_INVALID' }

    $stage = 'PRECHECK'
    $exists = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=DATABASE() AND table_name='workflow_import_preview_descriptor'")
    if ($exists -eq 0) {
        $stage = 'APPLY'
        $sql = Get-Content -LiteralPath $migrationPath -Raw
        $command = $connection.CreateCommand()
        try { $command.CommandText = $sql; [void]$command.ExecuteNonQuery() } finally { $command.Dispose() }
    }

    $stage = 'VERIFY'
    $columns = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='workflow_import_preview_descriptor' AND column_name IN ('id','descriptor_hash','resource_hash','user_id','task_id','provider_id','content_type','content_length','content_disposition','safe_file_name','content','status','expires_utc','claimed_utc','consumed_utc','created_utc')")
    $indexes = [int](Invoke-Scalar $connection "SELECT COUNT(DISTINCT index_name) FROM information_schema.statistics WHERE table_schema=DATABASE() AND table_name='workflow_import_preview_descriptor' AND index_name IN ('PRIMARY','ux_import_preview_descriptor_hash','ix_import_preview_authority','ix_import_preview_expiry')")
    $foreignKeys = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.key_column_usage WHERE table_schema=DATABASE() AND table_name='workflow_import_preview_descriptor' AND referenced_table_name IS NOT NULL")
    $engine = [string](Invoke-Scalar $connection "SELECT engine FROM information_schema.tables WHERE table_schema=DATABASE() AND table_name='workflow_import_preview_descriptor'")
    if ($columns -ne 16 -or $indexes -ne 4 -or $foreignKeys -ne 0 -or $engine -ine 'InnoDB') { throw 'DOC69_SCHEMA_VERIFICATION_FAILED' }
    Write-Host 'DOC69_SCHEMA=workflowdocument'
    Write-Host "DOC69_SCHEMA_COLUMNS=$columns"
    Write-Host "DOC69_SCHEMA_INDEXES=$indexes"
    Write-Host "DOC69_SCHEMA_FOREIGN_KEYS=$foreignKeys"
    Write-Host 'DOC69_SCHEMA_VERDICT=PASS'
} catch {
    $safeCode = if ($_.Exception.Message -match '^DOC69_SCHEMA_[A-Z_]+$') { $_.Exception.Message } else { 'DOC69_SCHEMA_OPERATION_FAILED' }
    Write-Error "$safeCode; STAGE=$stage. No se mostraron credenciales ni detalles de conexión."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $password = $null; $securePassword = $null
}
