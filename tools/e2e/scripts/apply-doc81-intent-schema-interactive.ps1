param([string]$Dsn = 'workflowdocument')

$ErrorActionPreference = 'Stop'
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..'))
$migrationPath = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot 'Doc\Actualizacion\workflow\ImportarServiciWebEnlace\DOC-81-preparacion-persistencia-reconciliacion\Sql\001-add-intent-capability.sql'))

function Invoke-Scalar([System.Data.Odbc.OdbcConnection]$Connection, [string]$Sql) {
    $command = $Connection.CreateCommand()
    try { $command.CommandText = $Sql; return $command.ExecuteScalar() } finally { $command.Dispose() }
}

function Invoke-Statement([System.Data.Odbc.OdbcConnection]$Connection, [string]$Sql) {
    $command = $Connection.CreateCommand()
    try { $command.CommandText = $Sql; [void]$command.ExecuteNonQuery() } finally { $command.Dispose() }
}

$confirmation = Read-Host "Aplicar la migración DOC-81 en el esquema Workflow del DSN $Dsn. Escriba SI"
if ($confirmation -cne 'SI') { throw 'DOC81_SCHEMA_NOT_AUTHORIZED' }
$user = Read-Host 'Usuario MySQL autorizado para ALTER TABLE'
if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC81_SCHEMA_USER_REQUIRED' }
$securePassword = Read-Host 'Contraseña MySQL' -AsSecureString
$passwordPointer = [IntPtr]::Zero
$connection = $null
$stage = 'CONNECT'

try {
    if (-not $migrationPath.StartsWith($repositoryRoot, [StringComparison]::OrdinalIgnoreCase) -or -not (Test-Path -LiteralPath $migrationPath)) { throw 'DOC81_SCHEMA_MIGRATION_MISSING' }
    $migrationContract = Get-Content -LiteralPath $migrationPath -Raw
    if ($migrationContract -notmatch 'ADD COLUMN capability VARCHAR\(80\) NOT NULL DEFAULT') { throw 'DOC81_SCHEMA_MIGRATION_INVALID' }
    if ($migrationContract -notmatch 'ADD COLUMN provider_reference VARCHAR\(80\) NOT NULL DEFAULT') { throw 'DOC81_SCHEMA_MIGRATION_INVALID' }

    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $Dsn
    $builder['UID'] = $user.Trim()
    $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()

    $stage = 'PRECHECK'
    $tableCount = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=DATABASE() AND table_name='workflow_import_intent'")
    $anchorColumns = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='workflow_import_intent' AND column_name IN ('intent_id','provider_id','radicado')")
    if ($tableCount -ne 1 -or $anchorColumns -ne 3) { throw 'DOC81_SCHEMA_TARGET_INVALID' }

    $capabilityCount = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='workflow_import_intent' AND column_name='capability'")
    $providerReferenceCount = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='workflow_import_intent' AND column_name='provider_reference'")

    if ($capabilityCount -eq 0) {
        $stage = 'APPLY_CAPABILITY'
        Invoke-Statement $connection "ALTER TABLE workflow_import_intent ADD COLUMN capability VARCHAR(80) NOT NULL DEFAULT '' AFTER provider_id"
    }
    if ($providerReferenceCount -eq 0) {
        $stage = 'APPLY_PROVIDER_REFERENCE'
        Invoke-Statement $connection "ALTER TABLE workflow_import_intent ADD COLUMN provider_reference VARCHAR(80) NOT NULL DEFAULT '' AFTER radicado"
    }

    $stage = 'VERIFY'
    $verified = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='workflow_import_intent' AND ((column_name='capability' AND data_type='varchar' AND character_maximum_length=80 AND is_nullable='NO') OR (column_name='provider_reference' AND data_type='varchar' AND character_maximum_length=80 AND is_nullable='NO'))")
    if ($verified -ne 2) { throw 'DOC81_SCHEMA_VERIFICATION_FAILED' }

    Write-Host 'DOC81_SCHEMA_COLUMNS=2'
    Write-Host 'DOC81_SCHEMA_VERDICT=PASS'
} catch {
    $safeCode = if ($_.Exception.Message -match '^DOC81_SCHEMA_[A-Z_]+$') { $_.Exception.Message } else { 'DOC81_SCHEMA_OPERATION_FAILED' }
    Write-Error "$safeCode; STAGE=$stage. No se mostraron credenciales ni detalles de conexión."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $password = $null
    $securePassword = $null
}