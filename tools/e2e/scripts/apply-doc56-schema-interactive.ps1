param()

$ErrorActionPreference = 'Stop'
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..'))
$dsn = 'workflowdocument'
$migrationPaths = @(
    'Doc\Actualizacion\workflow\ImportarServicioWeb\DOC-52-preflight-intencion-idempotencia\Sql\001-create-import-intents.sql',
    'Doc\Actualizacion\workflow\ImportarServicioWeb\DOC-53-orquestacion-estados-compensacion\Sql\001-extend-import-intent-execution-state.sql',
    'Doc\Actualizacion\workflow\ImportarServicioWeb\DOC-56-pruebas-backend-evidencia\Sql\001-add-import-intent-radicado.sql'
)
$documentTypeNameMigrationPath = 'Doc\Actualizacion\workflow\ImportarServicioWeb\DOC-56-pruebas-backend-evidencia\Sql\002-add-import-intent-document-type-name.sql'
$serviceAttemptMigrationPath = 'Doc\Actualizacion\workflow\ImportarServicioWeb\DOC-56-pruebas-backend-evidencia\Sql\003-create-ra-ser-intento-serviciointegracion.sql'
$serviceBusinessContextMigrationPath = 'Doc\Actualizacion\workflow\ImportarServicioWeb\DOC-56-pruebas-backend-evidencia\Sql\004-add-external-service-business-context.sql'

function Invoke-Scalar {
    param([System.Data.Odbc.OdbcConnection]$Connection, [string]$Sql)
    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $Sql
        return $command.ExecuteScalar()
    } finally {
        $command.Dispose()
    }
}

function Invoke-Migration {
    param([System.Data.Odbc.OdbcConnection]$Connection, [string]$RelativePath)
    $resolvedPath = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot $RelativePath))
    if (-not $resolvedPath.StartsWith($repositoryRoot, [StringComparison]::OrdinalIgnoreCase) -or -not (Test-Path -LiteralPath $resolvedPath)) { throw 'DOC56_SCHEMA_MIGRATION_MISSING' }
    $sql = Get-Content -LiteralPath $resolvedPath -Raw
    foreach ($statement in ($sql -split ';\s*(?:\r?\n|$)')) {
        if ([string]::IsNullOrWhiteSpace($statement)) { continue }
        $command = $Connection.CreateCommand()
        try {
            $command.CommandText = $statement
            [void]$command.ExecuteNonQuery()
        } finally {
            $command.Dispose()
        }
    }
}

$confirmation = Read-Host 'Esta operación modificará el esquema workflowdocument de PRUEBAS. Escriba SI para continuar'
if ($confirmation -cne 'SI') { throw 'DOC56_SCHEMA_NOT_AUTHORIZED' }
$user = Read-Host 'Usuario autorizado para migraciones'
if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC56_SCHEMA_USER_REQUIRED' }
$securePassword = Read-Host 'Contraseña' -AsSecureString
$passwordPointer = [IntPtr]::Zero
$connection = $null
$operationStage = 'CONNECT'
try {
    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $dsn
    $builder['UID'] = $user.Trim()
    $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()
    $workflowDatabase = $connection.Database

    $operationStage = 'VERIFY_BASE_SCHEMA'
    $requiredTables = @('workflow_import_intent', 'workflow_import_intent_requirement', 'workflow_import_intent_item', 'workflow_import_intent_transition')
    $existingTables = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name IN ('workflow_import_intent','workflow_import_intent_requirement','workflow_import_intent_item','workflow_import_intent_transition')")
    if ($existingTables -ne 0 -and $existingTables -ne $requiredTables.Count) { throw 'DOC56_SCHEMA_PARTIAL_STATE' }

    if ($existingTables -eq 0) {
        $operationStage = 'APPLY_BASE_SCHEMA'
        foreach ($relativePath in $migrationPaths) {
            Invoke-Migration $connection $relativePath
        }
    }

    $operationStage = 'APPLY_DOCUMENT_TYPE_NAME'
    $existingDocumentTypeName = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = DATABASE() AND table_name = 'workflow_import_intent_item' AND column_name = 'document_type_name'")
    if ($existingDocumentTypeName -eq 0) { Invoke-Migration $connection $documentTypeNameMigrationPath }

    $operationStage = 'APPLY_SERVICE_ATTEMPT_TELEMETRY'
    $serviceDatabase = [string](Invoke-Scalar $connection "SELECT GROUP_CONCAT(DISTINCT TABLE_SCHEMA ORDER BY TABLE_SCHEMA SEPARATOR ',') FROM information_schema.tables WHERE table_name = 'ra_ser_serviciointegracion'")
    if ([string]::IsNullOrWhiteSpace($serviceDatabase)) { throw 'DOC56_SCHEMA_SERVICE_CATALOG_MISSING' }
    if ($serviceDatabase.Contains(',')) { throw 'DOC56_SCHEMA_SERVICE_CATALOG_AMBIGUOUS' }
    $connection.ChangeDatabase($serviceDatabase)
    $existingServiceAttemptTable = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = 'ra_ser_intento_serviciointegracion'")
    if ($existingServiceAttemptTable -eq 0) {
        $serviceCatalogSchema = [string](Invoke-Scalar $connection "SELECT CONCAT(COALESCE(t.ENGINE, ''), '|', c.COLUMN_TYPE) FROM information_schema.tables t INNER JOIN information_schema.columns c ON c.TABLE_SCHEMA = t.TABLE_SCHEMA AND c.TABLE_NAME = t.TABLE_NAME WHERE t.TABLE_SCHEMA = DATABASE() AND t.TABLE_NAME = 'ra_ser_serviciointegracion' AND c.COLUMN_NAME = 'Id_ser_servicioIntegracion'")
        if ([string]::IsNullOrWhiteSpace($serviceCatalogSchema)) { throw 'DOC56_SCHEMA_SERVICE_CATALOG_MISSING' }
        Write-Host "DOC56_SERVICE_CATALOG_SCHEMA=$serviceCatalogSchema"
        Invoke-Migration $connection $serviceAttemptMigrationPath
    }
    $existingBusinessContextColumns = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='ra_ser_intento_serviciointegracion' AND column_name IN ('TaskId','Radicado','CodigoBarras','ReferenciaProveedor')")
    If ($existingBusinessContextColumns -ne 0 -and $existingBusinessContextColumns -ne 4) { throw 'DOC56_SCHEMA_SERVICE_CONTEXT_PARTIAL_STATE' }
    If ($existingBusinessContextColumns -eq 0) { Invoke-Migration $connection $serviceBusinessContextMigrationPath }
    $verifiedServiceAttemptTable = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = 'ra_ser_intento_serviciointegracion'")
    $verifiedServiceAttemptForeignKey = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.key_column_usage WHERE table_schema = DATABASE() AND table_name = 'ra_ser_intento_serviciointegracion' AND column_name = 'Id_ser_servicioIntegracion' AND referenced_table_name = 'ra_ser_serviciointegracion' AND referenced_column_name = 'Id_ser_servicioIntegracion'")
    $verifiedBusinessContextColumns = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema=DATABASE() AND table_name='ra_ser_intento_serviciointegracion' AND column_name IN ('TaskId','Radicado','CodigoBarras','ReferenciaProveedor')")
    $verifiedBusinessContextIndexes = [int](Invoke-Scalar $connection "SELECT COUNT(DISTINCT index_name) FROM information_schema.statistics WHERE table_schema=DATABASE() AND table_name='ra_ser_intento_serviciointegracion' AND index_name IN ('ix_ra_ser_intento_tarea','ix_ra_ser_intento_radicado','ix_ra_ser_intento_codigo_barras','ix_ra_ser_intento_referencia_proveedor')")
    $connection.ChangeDatabase($workflowDatabase)

    $operationStage = 'VERIFY_COMPLETE_SCHEMA'
    $verifiedTables = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name IN ('workflow_import_intent','workflow_import_intent_requirement','workflow_import_intent_item','workflow_import_intent_transition')")
    $verifiedRadicado = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = DATABASE() AND table_name = 'workflow_import_intent' AND column_name = 'radicado'")
    $verifiedDocumentTypeName = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM information_schema.columns WHERE table_schema = DATABASE() AND table_name = 'workflow_import_intent_item' AND column_name = 'document_type_name'")
    if ($verifiedTables -ne $requiredTables.Count -or $verifiedRadicado -ne 1 -or $verifiedDocumentTypeName -ne 1 -or $verifiedServiceAttemptTable -ne 1 -or $verifiedServiceAttemptForeignKey -ne 1 -or $verifiedBusinessContextColumns -ne 4 -or $verifiedBusinessContextIndexes -ne 4) { throw 'DOC56_SCHEMA_VERIFICATION_FAILED' }
    Write-Host 'DOC56_SCHEMA_APPLIED_AND_VERIFIED'
} catch {
    $safeCode = if ($_.Exception.Message -match '^DOC56_SCHEMA_[A-Z_]+$') { $_.Exception.Message } else { 'DOC56_SCHEMA_OPERATION_FAILED' }
    $diagnostic = ''
    if ($_.Exception -is [System.Data.Odbc.OdbcException]) {
        $details = @($_.Exception.Errors | ForEach-Object {
            $safeMessage = [string]$_.Message -replace '(?i)(PWD|PASSWORD|UID|USER)\s*=\s*[^;\s]+', '$1=***'
            "SQLSTATE=$($_.SQLState);NATIVE=$($_.NativeError);MESSAGE=$safeMessage"
        })
        $diagnostic = ' ' + ($details -join ' | ')
    } else {
        $safeMessage = [string]$_.Exception.Message -replace '(?i)(PWD|PASSWORD|UID|USER)\s*=\s*[^;\s]+', '$1=***'
        $diagnostic = " TYPE=$($_.Exception.GetType().FullName);MESSAGE=$safeMessage"
    }
    Write-Error "$safeCode; STAGE=$operationStage;$diagnostic No se mostraron credenciales ni detalles de conexión."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $password = $null
    $securePassword = $null
}
