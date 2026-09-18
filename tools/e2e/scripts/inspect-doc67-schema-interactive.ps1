param()

$ErrorActionPreference = 'Stop'
$dsn = 'workflowdocument'

function Invoke-Names {
    param([System.Data.Odbc.OdbcConnection]$Connection, [string]$Sql, [object[]]$Parameters = @())
    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $Sql
        foreach ($value in $Parameters) {
            $parameter = $command.CreateParameter()
            $parameter.Value = $value
            [void]$command.Parameters.Add($parameter)
        }
        $reader = $command.ExecuteReader()
        $values = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
        try {
            while ($reader.Read()) { [void]$values.Add([string]$reader.GetValue(0)) }
        } finally { $reader.Dispose() }
        return $values
    } finally { $command.Dispose() }
}

function Write-Missing {
    param([string]$Kind, [string[]]$Expected, [System.Collections.Generic.HashSet[string]]$Actual)
    foreach ($name in $Expected) {
        if (-not $Actual.Contains($name)) { Write-Host "MISSING_${Kind}=$name" }
    }
}

$confirmation = Read-Host 'Inspeccion DOC-67 exclusivamente SELECT en PRUEBAS. Escriba SI para continuar'
if ($confirmation -cne 'SI') { throw 'DOC67_SCHEMA_INSPECTION_NOT_AUTHORIZED' }
$user = Read-Host 'Usuario MySQL de solo lectura'
if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC67_SCHEMA_USER_REQUIRED' }
$securePassword = Read-Host 'Contrasena MySQL de solo lectura' -AsSecureString
$pointer = [IntPtr]::Zero
$connection = $null
try {
    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $dsn
    $builder['UID'] = $user.Trim()
    $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()

    $requiredTables = @(
        'workflow_import_intent', 'workflow_import_intent_requirement',
        'workflow_import_intent_item', 'workflow_import_intent_transition',
        'workflow_import_inscription', 'workflow_import_related_document',
        'workflow_import_document_link_cache'
    )
    $tables = Invoke-Names $connection 'SELECT table_name FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name LIKE ?' @('workflow_import_%')
    Write-Missing 'TABLE' $requiredTables $tables

    $requiredColumns = @{
        workflow_import_intent_item = @('inscription_key','expedient_id','storage_status','relation_status','index_status','cache_status')
        workflow_import_inscription = @('intent_id','inscription_key','inscription_ordinal','cabinet_name','expedient_id','expedient_role','expedient_status','cache_status','created_utc','updated_utc')
        workflow_import_related_document = @('related_document_id','intent_id','task_id','image_id','cabinet_name','sii_radicado','inscription_key','expected_expedient_id','discovery_status','destination_status','relation_status','cache_status','cabinet_index_status','electronic_index_status','xml_index_status','reconciliation_status','retryable','error_code','created_utc','updated_utc')
        workflow_import_document_link_cache = @('document_link_cache_id','task_id','image_id','cabinet_name','expected_expedient_id','sii_radicado','relation_status','created_utc','verified_utc')
    }
    foreach ($table in ($requiredColumns.Keys | Sort-Object)) {
        if (-not $tables.Contains($table)) { continue }
        $columns = Invoke-Names $connection 'SELECT column_name FROM information_schema.columns WHERE table_schema = DATABASE() AND table_name = ?' @($table)
        foreach ($column in $requiredColumns[$table]) {
            if (-not $columns.Contains($column)) { Write-Host "MISSING_COLUMN=$table.$column" }
        }
    }

    $requiredIndexes = @{
        workflow_import_intent_item = @('ix_import_item_inscription')
        workflow_import_inscription = @('PRIMARY','uq_import_inscription_ordinal','ix_import_inscription_expedient')
        workflow_import_related_document = @('PRIMARY','uq_import_related_document','ix_import_related_document_expedient','ix_import_related_document_inscription')
        workflow_import_document_link_cache = @('PRIMARY','uq_import_document_link_cache_identity','ix_import_document_link_cache_expedient','ix_import_document_link_cache_radicado')
    }
    foreach ($table in ($requiredIndexes.Keys | Sort-Object)) {
        if (-not $tables.Contains($table)) { continue }
        $indexes = Invoke-Names $connection 'SELECT DISTINCT index_name FROM information_schema.statistics WHERE table_schema = DATABASE() AND table_name = ?' @($table)
        foreach ($index in $requiredIndexes[$table]) {
            if (-not $indexes.Contains($index)) { Write-Host "MISSING_INDEX=$table.$index" }
        }
    }

    $requiredForeignKeys = @{
        workflow_import_intent_item = @('fk_import_item_inscription')
        workflow_import_inscription = @('fk_import_inscription_intent')
        workflow_import_related_document = @('fk_import_related_document_intent','fk_import_related_document_inscription')
    }
    foreach ($table in ($requiredForeignKeys.Keys | Sort-Object)) {
        if (-not $tables.Contains($table)) { continue }
        $foreignKeys = Invoke-Names $connection "SELECT constraint_name FROM information_schema.table_constraints WHERE constraint_schema = DATABASE() AND table_name = ? AND constraint_type = 'FOREIGN KEY'" @($table)
        foreach ($foreignKey in $requiredForeignKeys[$table]) {
            if (-not $foreignKeys.Contains($foreignKey)) { Write-Host "MISSING_FOREIGN_KEY=$table.$foreignKey" }
        }
    }
    Write-Host 'DOC67_SCHEMA_INSPECTION_COMPLETE'
} catch {
    $code = if ($_.Exception.Message -match '^DOC67_SCHEMA_[A-Z0-9_]+$') { $_.Exception.Message } else { 'DOC67_SCHEMA_INSPECTION_FAILED' }
    Write-Error "$code. No se mostraron credenciales, datos ni detalles de conexion."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($pointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }
    $password = $null
    $securePassword = $null
}
