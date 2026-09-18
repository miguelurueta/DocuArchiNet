param(
    [string]$WorkflowSchema = "workflowdocument",
    [string]$Dsn = "workflowdocument"
)

$ErrorActionPreference = "Stop"

$confirmation = Read-Host "Auditoria DOC-67 exclusivamente SELECT sobre information_schema. Escriba SI"
if ($confirmation -ne "SI") {
    Write-Host "DOC67_SCHEMA_AUDIT_CANCELLED"
    exit 2
}

$mysqlUser = Read-Host "Usuario MySQL de solo lectura"
$securePassword = Read-Host "Contraseña MySQL de solo lectura" -AsSecureString
$passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
$connection = $null
$stage = "CONNECTION"

try {
    $mysqlPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder = New-Object System.Data.Odbc.OdbcConnectionStringBuilder
    $builder["DSN"] = $Dsn
    $builder["UID"] = $mysqlUser.Trim()
    $builder["PWD"] = $mysqlPassword

    $connection = New-Object System.Data.Odbc.OdbcConnection($builder.ConnectionString)
    $connection.Open()

    function Invoke-Select([string]$Sql) {
        $command = $connection.CreateCommand()
        $command.CommandText = $Sql
        $adapter = New-Object System.Data.Odbc.OdbcDataAdapter($command)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        Write-Output -NoEnumerate $table
    }

    $escapedSchema = $WorkflowSchema.Replace("'", "''")
    $expectedColumns = [ordered]@{
        workflow_import_intent = @('intent_id','idempotency_key','payload_hash','operation_id','correlation_id','user_id','group_id','user_login','task_id','route_id','procedure_id','provider_id','radicado','status','version_token','created_utc','updated_utc')
        workflow_import_intent_requirement = @('intent_id','requirement_code','is_satisfied','visible_message')
        workflow_import_inscription = @('intent_id','inscription_key','inscription_ordinal','book_code','registry_number','matricula','normalized_matricula','proponente','subject_identification','subject_name','owner_matricula','owner_identification','owner_name','cabinet_name','expedient_id','expedient_role','expedient_status','cache_status','created_utc','updated_utc')
        workflow_import_intent_item = @('intent_id','client_item_id','inscription_key','provider_id','external_key','target_task_id','document_type_id','document_type_name','file_name','content_type','status','document_id','expedient_id','storage_status','relation_status','index_status','cache_status','persistence_known','retryable','error_code','visible_message','correlation_id')
        workflow_import_intent_transition = @('transition_id','intent_id','client_item_id','previous_status','new_status','previous_version','new_version','occurred_utc','correlation_id','result_code')
        workflow_import_related_document = @('related_document_id','intent_id','task_id','image_id','cabinet_name','sii_radicado','document_type_id','inscription_key','expected_expedient_id','discovery_status','destination_status','relation_status','cache_status','cabinet_index_status','electronic_index_status','xml_index_status','reconciliation_status','retryable','error_code','created_utc','updated_utc')
        workflow_import_document_link_cache = @('document_link_cache_id','task_id','image_id','cabinet_name','expected_expedient_id','sii_radicado','relation_status','created_utc','verified_utc')
    }
    $requiredForeignKeys = @(
        'workflow_import_intent_requirement|fk_import_requirement_intent|workflow_import_intent',
        'workflow_import_inscription|fk_import_inscription_intent|workflow_import_intent',
        'workflow_import_intent_item|fk_import_item_intent|workflow_import_intent',
        'workflow_import_intent_item|fk_import_item_inscription|workflow_import_inscription',
        'workflow_import_intent_transition|fk_import_transition_intent|workflow_import_intent',
        'workflow_import_related_document|fk_import_related_document_intent|workflow_import_intent',
        'workflow_import_related_document|fk_import_related_document_inscription|workflow_import_inscription'
    )

    $stage = "TABLES"
    $tables = Invoke-Select "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA='$escapedSchema' AND TABLE_NAME LIKE 'workflow_import_%'"
    $stage = "COLUMNS"
    $columns = Invoke-Select "SELECT TABLE_NAME, COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='$escapedSchema' AND TABLE_NAME LIKE 'workflow_import_%'"
    $stage = "FOREIGN_KEYS"
    $foreignKeys = Invoke-Select "SELECT TABLE_NAME, CONSTRAINT_NAME, REFERENCED_TABLE_SCHEMA, REFERENCED_TABLE_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA='$escapedSchema' AND TABLE_NAME LIKE 'workflow_import_%' AND REFERENCED_TABLE_NAME IS NOT NULL"

    $actualTables = @{}
    foreach ($row in $tables.Rows) { $actualTables[$row.TABLE_NAME.ToString().ToLowerInvariant()] = $true }
    $actualColumns = @{}
    foreach ($row in $columns.Rows) { $actualColumns[("{0}|{1}" -f $row.TABLE_NAME, $row.COLUMN_NAME).ToLowerInvariant()] = $true }
    $actualForeignKeys = @{}
    $crossDatabaseReferences = 0
    foreach ($row in $foreignKeys.Rows) {
        $key = ("{0}|{1}|{2}" -f $row.TABLE_NAME, $row.CONSTRAINT_NAME, $row.REFERENCED_TABLE_NAME).ToLowerInvariant()
        $actualForeignKeys[$key] = $true
        if ($row.REFERENCED_TABLE_SCHEMA.ToString() -ine $WorkflowSchema) {
            Write-Host ("CROSS_DATABASE_FOREIGN_KEY={0}.{1}->{2}.{3}" -f $row.TABLE_NAME, $row.CONSTRAINT_NAME, $row.REFERENCED_TABLE_SCHEMA, $row.REFERENCED_TABLE_NAME)
            $crossDatabaseReferences++
        }
    }

    $missing = 0
    foreach ($entry in $expectedColumns.GetEnumerator()) {
        if (-not $actualTables.ContainsKey($entry.Key.ToLowerInvariant())) {
            Write-Host "MISSING_TABLE=$($entry.Key)"
            $missing++
            continue
        }
        foreach ($column in $entry.Value) {
            if (-not $actualColumns.ContainsKey(("$($entry.Key)|$column").ToLowerInvariant())) {
                Write-Host "MISSING_COLUMN=$($entry.Key).$column"
                $missing++
            }
        }
    }
    foreach ($foreignKey in $requiredForeignKeys) {
        if (-not $actualForeignKeys.ContainsKey($foreignKey.ToLowerInvariant())) {
            Write-Host "MISSING_FOREIGN_KEY=$foreignKey"
            $missing++
        }
    }

    $physicalNames = @('expediente_archivo','ra_sii_cache_exepediente','ra_relacion_radicado_externo_expediente','registro_producion_documental','logdocuarchi','DETALLE_GABIENETE','tipo_doc_entrante','ra_auto_campo_unico_expediente','ra_dig_tipos_docum_lista_chequeo','tipo_doc_series','configuracion_gabinete','ra_ser_serviciointegracion','ra_ser_intento_serviciointegracion')
    $physicalList = ($physicalNames | ForEach-Object { "'" + $_.Replace("'", "''") + "'" }) -join ','
    $stage = "PHYSICAL_LOCATIONS"
    $locations = Invoke-Select "SELECT TABLE_NAME, TABLE_SCHEMA FROM information_schema.TABLES WHERE TABLE_NAME IN ($physicalList) ORDER BY TABLE_NAME, TABLE_SCHEMA"
    foreach ($name in $physicalNames) {
        $schemas = @($locations.Rows | Where-Object { $_.TABLE_NAME.ToString() -ieq $name } | ForEach-Object { $_.TABLE_SCHEMA.ToString() })
        if ($schemas.Count -eq 0) { Write-Host "PHYSICAL_TABLE_NOT_FOUND=$name" }
        else { Write-Host ("PHYSICAL_TABLE_LOCATION={0}:{1}" -f $name, ($schemas -join ',')) }
    }

    Write-Host "DOC67_SCHEMA=$WorkflowSchema"
    Write-Host "DOC67_MODERN_TABLES_FOUND=$($actualTables.Count)"
    Write-Host "DOC67_CROSS_DATABASE_FOREIGN_KEYS=$crossDatabaseReferences"
    Write-Host "DOC67_SCHEMA_DIFFERENCES=$missing"
    if ($missing -eq 0 -and $crossDatabaseReferences -eq 0) { Write-Host "DOC67_SCHEMA_VERDICT=PASS"; exit 0 }
    Write-Host "DOC67_SCHEMA_VERDICT=FAIL"
    exit 1
}
catch {
    Write-Error "DOC67_SCHEMA_AUDIT_${stage}_FAILED. No se mostraron secretos ni datos personales."
    exit 1
}
finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $mysqlPassword = $null
    $securePassword = $null
}
