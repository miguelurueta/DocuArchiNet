param(
    [Parameter(Mandatory = $true)]
    [ValidateRange(1, [long]::MaxValue)]
    [long]$TaskId,
    [string]$Dsn = 'workflowdocument'
)

$ErrorActionPreference = 'Stop'
$connection = $null
$passwordPointer = [IntPtr]::Zero

function Read-One {
    param([System.Data.Odbc.OdbcConnection]$Connection, [string]$Sql, [object[]]$Values)
    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $Sql
        foreach ($value in $Values) {
            $parameter = $command.CreateParameter()
            $parameter.Value = $value
            [void]$command.Parameters.Add($parameter)
        }
        $reader = $command.ExecuteReader()
        try {
            if (-not $reader.Read()) { return $null }
            $result = [ordered]@{}
            for ($index = 0; $index -lt $reader.FieldCount; $index++) {
                $value = $reader.GetValue($index)
                $result[$reader.GetName($index)] = if ([Convert]::IsDBNull($value)) { $null } else { $value }
            }
            return [pscustomobject]$result
        } finally { $reader.Dispose() }
    } finally { $command.Dispose() }
}

function Number([object]$Value) {
    if ($null -eq $Value) { return 0 }
    return [int64]$Value
}

try {
    $confirmation = Read-Host 'Auditoria DOC-67 exclusivamente SELECT en CERTIFICACION. Escriba SI para continuar'
    if ($confirmation -cne 'SI') { throw 'DOC67_EVIDENCE_NOT_AUTHORIZED' }
    $user = Read-Host 'Usuario MySQL de solo lectura'
    if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC67_EVIDENCE_USER_REQUIRED' }
    $securePassword = Read-Host 'Contraseña MySQL de solo lectura' -AsSecureString
    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)

    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $Dsn
    $builder['UID'] = $user.Trim()
    $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()

    $intent = Read-One $connection 'SELECT intent_id,status,created_utc FROM workflow_import_intent WHERE task_id=? ORDER BY created_utc DESC LIMIT 1' @($TaskId)
    if ($null -eq $intent) { throw 'DOC67_EVIDENCE_INTENT_NOT_FOUND' }
    $intentId = [string]$intent.intent_id

    $items = Read-One $connection "SELECT COUNT(*) total, SUM(status='Completada') completed, SUM(document_id IS NOT NULL) stored, SUM(expedient_id IS NOT NULL) with_expedient, SUM(storage_status='Confirmado') storage_ok, SUM(relation_status='Confirmado') relation_ok, SUM(index_status='Confirmado') index_ok, SUM(cache_status='Confirmado') cache_ok, SUM(error_code IS NOT NULL AND error_code<>'') errors FROM workflow_import_intent_item WHERE intent_id=?" @($intentId)
    $inscriptions = Read-One $connection "SELECT COUNT(*) total, SUM(expedient_id IS NOT NULL) with_expedient, SUM(expedient_status='Confirmado') expedient_ok, SUM(cache_status='Confirmado') cache_ok FROM workflow_import_inscription WHERE intent_id=?" @($intentId)
    $related = Read-One $connection "SELECT COUNT(*) total, COUNT(DISTINCT image_id) distinct_images, SUM(expected_expedient_id IS NOT NULL) with_expedient, SUM(destination_status='Confirmado') destination_ok, SUM(relation_status='Correcta') relation_ok, SUM(cache_status='Confirmado') cache_ok, SUM(cabinet_index_status='Confirmado') cabinet_index_ok, SUM(electronic_index_status='Confirmado') sql_index_ok, SUM(xml_index_status='Confirmado') xml_index_ok, SUM(reconciliation_status='Confirmado') reconciliation_ok, SUM(error_code IS NOT NULL AND error_code<>'') errors FROM workflow_import_related_document WHERE intent_id=? AND task_id=?" @($intentId,$TaskId)
    $cache = Read-One $connection "SELECT COUNT(*) total, COUNT(DISTINCT image_id) distinct_images, SUM(expected_expedient_id IS NOT NULL) with_expedient, SUM(relation_status='Correcta') relation_ok, SUM(verified_utc IS NOT NULL) verified FROM workflow_import_document_link_cache WHERE task_id=?" @($TaskId)
    $transitions = Read-One $connection "SELECT COUNT(*) total, SUM(new_status='Reconciliada') reconciled, SUM(new_status='Completada') completed FROM workflow_import_intent_transition WHERE intent_id=?" @($intentId)

    $itemTotal = Number $items.total
    $relatedTotal = Number $related.total
    $checks = [ordered]@{
        IntentCompleted = ([string]$intent.status -eq 'Completada')
        ItemsCompleted = ($itemTotal -gt 0 -and (Number $items.completed) -eq $itemTotal)
        ItemsStored = ($itemTotal -gt 0 -and (Number $items.stored) -eq $itemTotal)
        ItemsHaveExpedient = ($itemTotal -gt 0 -and (Number $items.with_expedient) -eq $itemTotal)
        ItemEffectsConfirmed = ($itemTotal -gt 0 -and (Number $items.storage_ok) -eq $itemTotal -and (Number $items.relation_ok) -eq $itemTotal -and (Number $items.index_ok) -eq $itemTotal -and (Number $items.cache_ok) -eq $itemTotal)
        InscriptionsConfirmed = ((Number $inscriptions.total) -gt 0 -and (Number $inscriptions.with_expedient) -eq (Number $inscriptions.total) -and (Number $inscriptions.expedient_ok) -eq (Number $inscriptions.total) -and (Number $inscriptions.cache_ok) -eq (Number $inscriptions.total))
        RelatedUniverseUnique = ($relatedTotal -gt 0 -and (Number $related.distinct_images) -eq $relatedTotal)
        RelatedEffectsConfirmed = ($relatedTotal -gt 0 -and (Number $related.with_expedient) -eq $relatedTotal -and (Number $related.destination_ok) -eq $relatedTotal -and (Number $related.relation_ok) -eq $relatedTotal -and (Number $related.cache_ok) -eq $relatedTotal -and (Number $related.cabinet_index_ok) -eq $relatedTotal -and (Number $related.sql_index_ok) -eq $relatedTotal -and (Number $related.xml_index_ok) -eq $relatedTotal -and (Number $related.reconciliation_ok) -eq $relatedTotal)
        CacheMatchesUniverse = ($relatedTotal -gt 0 -and (Number $cache.total) -eq $relatedTotal -and (Number $cache.distinct_images) -eq $relatedTotal -and (Number $cache.with_expedient) -eq $relatedTotal -and (Number $cache.relation_ok) -eq $relatedTotal -and (Number $cache.verified) -eq $relatedTotal)
        NoErrors = ((Number $items.errors) -eq 0 -and (Number $related.errors) -eq 0)
        ReconciliationTransition = ((Number $transitions.reconciled) -gt 0 -and (Number $transitions.completed) -gt 0)
    }

    Write-Host ('DOC67_EVIDENCE_INTENT_ID=' + $intentId)
    Write-Host ('DOC67_EVIDENCE_CREATED_UTC=' + [string]$intent.created_utc)
    Write-Host ('DOC67_EVIDENCE_ITEM_COUNT=' + $itemTotal)
    Write-Host ('DOC67_EVIDENCE_RELATED_COUNT=' + $relatedTotal)
    Write-Host ('DOC67_EVIDENCE_ITEM_STORAGE_CONFIRMED=' + (Number $items.storage_ok))
    Write-Host ('DOC67_EVIDENCE_ITEM_RELATION_CONFIRMED=' + (Number $items.relation_ok))
    Write-Host ('DOC67_EVIDENCE_ITEM_INDEX_CONFIRMED=' + (Number $items.index_ok))
    Write-Host ('DOC67_EVIDENCE_ITEM_CACHE_CONFIRMED=' + (Number $items.cache_ok))
    foreach ($entry in $checks.GetEnumerator()) { Write-Host ("DOC67_EVIDENCE_{0}={1}" -f $entry.Key.ToUpperInvariant(), $entry.Value.ToString().ToUpperInvariant()) }
    $passed = -not ($checks.Values -contains $false)
    Write-Host ('DOC67_EVIDENCE_VERDICT=' + $(if ($passed) { 'PASSED' } else { 'FAILED' }))
    if (-not $passed) { exit 3 }
} catch {
    $safeCode = if ($_.Exception.Message -match '^DOC67_EVIDENCE_[A-Z_]+$') { $_.Exception.Message } else { 'DOC67_EVIDENCE_QUERY_FAILED' }
    Write-Error "$safeCode. No se mostraron credenciales ni detalles de conexión."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $password = $null
    $securePassword = $null
}
