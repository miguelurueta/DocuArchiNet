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
            $parameter = $command.CreateParameter(); $parameter.Value = $value
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

try {
    $stage = 'AUTHORIZATION'
    $confirmation = Read-Host 'Auditoria DOC-69 exclusivamente SELECT en CERTIFICACION. Escriba SI para continuar'
    if ($confirmation -cne 'SI') { throw 'DOC69_TELEMETRY_NOT_AUTHORIZED' }
    $user = Read-Host 'Usuario MySQL de solo lectura'
    if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC69_TELEMETRY_USER_REQUIRED' }
    $securePassword = Read-Host 'Contraseña MySQL de solo lectura' -AsSecureString
    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $Dsn; $builder['UID'] = $user.Trim(); $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString); $connection.Open()

    $stage = 'SCHEMA'
    $schema = Read-One $connection "SELECT COUNT(*) table_count, SUM(COLUMN_NAME='TaskId') task_id, SUM(COLUMN_NAME='OperationId') operation_id, SUM(COLUMN_NAME='Operacion') operation_name, SUM(COLUMN_NAME='Exitoso') success_flag, SUM(COLUMN_NAME='FechaFinUtc') finished_utc FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='docuarchi' AND TABLE_NAME=?" @('ra_ser_intento_serviciointegracion')
    if ($null -eq $schema -or [int64]$schema.table_count -eq 0) { throw 'DOC69_TELEMETRY_TABLE_NOT_FOUND' }
    if ([int64]$schema.task_id -ne 1) { throw 'DOC69_TELEMETRY_COLUMN_TASK_ID_MISSING' }
    if ([int64]$schema.operation_id -ne 1) { throw 'DOC69_TELEMETRY_COLUMN_OPERATION_ID_MISSING' }
    if ([int64]$schema.operation_name -ne 1 -or [int64]$schema.success_flag -ne 1 -or [int64]$schema.finished_utc -ne 1) { throw 'DOC69_TELEMETRY_REQUIRED_COLUMNS_MISSING' }

    $sql = @'
SELECT COUNT(*) total,
       SUM(Operacion='SOLICITAR_TOKEN') token_calls,
       SUM(Operacion='CONSULTAR_SELLO') seal_calls,
       SUM(Operacion='DESCARGAR_ANEXO') download_calls,
       SUM(Exitoso=1) successful_calls
FROM docuarchi.ra_ser_intento_serviciointegracion
WHERE TaskId=? AND OperationId=(
  SELECT latest.OperationId FROM (
    SELECT OperationId,MAX(FechaFinUtc) finished
    FROM docuarchi.ra_ser_intento_serviciointegracion
    WHERE TaskId=? AND OperationId IS NOT NULL AND OperationId<>''
    GROUP BY OperationId
    HAVING SUM(Operacion='DESCARGAR_ANEXO')>0
    ORDER BY finished DESC LIMIT 1
  ) latest
)
'@
    $stage = 'COUNT'
    $row = Read-One $connection $sql @($TaskId,$TaskId)
    if ($null -eq $row) { throw 'DOC69_TELEMETRY_NOT_FOUND' }
    $total=[int64]$row.total; $tokens=[int64]$row.token_calls; $seals=[int64]$row.seal_calls
    $downloads=[int64]$row.download_calls; $successful=[int64]$row.successful_calls
    $passed=($total -eq 3 -and $tokens -eq 1 -and $seals -eq 1 -and $downloads -eq 1 -and $successful -eq 3)
    Write-Host ('DOC69_TELEMETRY_TOTAL=' + $total)
    Write-Host ('DOC69_TELEMETRY_TOKEN_CALLS=' + $tokens)
    Write-Host ('DOC69_TELEMETRY_SEAL_CALLS=' + $seals)
    Write-Host ('DOC69_TELEMETRY_DOWNLOAD_CALLS=' + $downloads)
    Write-Host ('DOC69_TELEMETRY_SUCCESSFUL_CALLS=' + $successful)
    Write-Host ('DOC69_TELEMETRY_VERDICT=' + $(if ($passed) { 'PASSED' } else { 'FAILED' }))
    if (-not $passed) { exit 3 }
} catch {
    if ($_.Exception.Message -match '^DOC69_TELEMETRY_[A-Z0-9_]+$') {
        $safeCode = $_.Exception.Message
    } elseif ($_.Exception -is [System.Data.Odbc.OdbcException] -and $_.Exception.Errors.Count -gt 0) {
        $state = ($_.Exception.Errors[0].SQLState -replace '[^A-Za-z0-9]','').ToUpperInvariant()
        $safeCode = "DOC69_TELEMETRY_${stage}_FAILED_SQLSTATE_${state}"
    } else {
        $safeCode = "DOC69_TELEMETRY_${stage}_FAILED"
    }
    Write-Error "$safeCode. No se mostraron credenciales ni detalles de conexión."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $password=$null; $securePassword=$null
}
