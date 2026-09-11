param()

$ErrorActionPreference = 'Stop'
$confirmation = Read-Host 'Inspeccion DOC-56 exclusivamente SELECT en DocuArchi PRUEBAS. Escriba SI para continuar'
if ($confirmation -cne 'SI') { throw 'DOC56_TELEMETRY_INSPECTION_NOT_AUTHORIZED' }
$user = Read-Host 'Usuario MySQL de solo lectura'
if ([string]::IsNullOrWhiteSpace($user)) { throw 'DOC56_TELEMETRY_USER_REQUIRED' }
$securePassword = Read-Host 'Contrasena MySQL de solo lectura' -AsSecureString
$passwordPointer = [IntPtr]::Zero
$connection = $null
try {
    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = 'workflowdocument'
    $builder['UID'] = $user.Trim()
    $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()

    $schemaCommand = $connection.CreateCommand()
    $schemaCommand.CommandText = "SELECT GROUP_CONCAT(DISTINCT TABLE_SCHEMA ORDER BY TABLE_SCHEMA SEPARATOR ',') FROM information_schema.tables WHERE table_name='ra_ser_intento_serviciointegracion'"
    $serviceDatabase = [string]$schemaCommand.ExecuteScalar()
    $schemaCommand.Dispose()
    if ([string]::IsNullOrWhiteSpace($serviceDatabase)) { throw 'DOC56_TELEMETRY_TABLE_MISSING' }
    if ($serviceDatabase.Contains(',')) { throw 'DOC56_TELEMETRY_SCHEMA_AMBIGUOUS' }
    $connection.ChangeDatabase($serviceDatabase)

    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT s.NombreServicio,a.Operacion,a.Exitoso,a.CodigoError,a.CategoriaError,a.CodigoDependencia,a.EstadoHttp,a.Reintentable,a.TaskId,a.Radicado,a.CodigoBarras,a.ReferenciaProveedor,a.IntentId,a.ClientItemId,a.OperationId,a.CorrelationId,a.DuracionMs,a.FechaInicioUtc,a.FechaFinUtc FROM ra_ser_intento_serviciointegracion a INNER JOIN ra_ser_serviciointegracion s ON s.Id_ser_servicioIntegracion=a.Id_ser_servicioIntegracion ORDER BY a.Id_ser_intentoServicioIntegracion DESC LIMIT 10"
    $reader = $command.ExecuteReader()
    $count = 0
    while ($reader.Read()) {
        $count++
        $values = for ($index = 0; $index -lt $reader.FieldCount; $index++) {
            if ($reader.IsDBNull($index)) { '[NULL]' } else { [string]$reader.GetValue($index) }
        }
        Write-Host ('DOC56_TELEMETRY_ATTEMPT_{0}={1}' -f $count, ($values -join '|'))
    }
    $reader.Dispose()
    $command.Dispose()
    Write-Host ('DOC56_TELEMETRY_ATTEMPT_COUNT={0}' -f $count)
    Write-Host 'DOC56_TELEMETRY_INSPECTION_COMPLETE'
} catch {
    $safeCode = if ($_.Exception.Message -match '^DOC56_TELEMETRY_[A-Z_]+$') { $_.Exception.Message } else { 'DOC56_TELEMETRY_INSPECTION_FAILED' }
    Write-Error "$safeCode. No se mostraron credenciales ni datos de conexion."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($passwordPointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer) }
    $password = $null
    $securePassword = $null
}
