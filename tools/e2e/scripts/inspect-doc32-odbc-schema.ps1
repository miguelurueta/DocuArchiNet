param(
    [ValidatePattern('^[A-Za-z0-9 _.-]+$')]
    [string]$Dsn = 'workflowconta'
)

$ErrorActionPreference = 'Stop'
$passwordPointer = [IntPtr]::Zero
$password = $null

try {
    $user = Read-Host 'Usuario MySQL de solo lectura'
    $securePassword = Read-Host 'Contraseña MySQL de solo lectura' -AsSecureString
    if ([string]::IsNullOrWhiteSpace($user) -or $securePassword.Length -eq 0) { throw 'missing-input' }

    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $Dsn
    $builder['UID'] = $user.Trim()
    $builder['PWD'] = $password

    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()
    try {
        $command = $connection.CreateCommand()
        $command.CommandText = @'
SELECT TABLE_NAME, COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('estados_tarea_workflow', 'log_usuario')
  AND ? = 1
ORDER BY TABLE_NAME, ORDINAL_POSITION
'@
        $parameter = $command.Parameters.Add('@readOnly', [System.Data.Odbc.OdbcType]::Integer)
        $parameter.Value = 1
        $reader = $command.ExecuteReader()
        try {
            $found = $false
            while ($reader.Read()) {
                $found = $true
                [Console]::Out.WriteLine(('{0}.{1}' -f $reader.GetString(0), $reader.GetString(1)))
            }
            if (-not $found) {
                [Console]::Out.WriteLine('No se encontraron las tablas DOC-32 en el catálogo autorizado.')
            }
        } finally {
            $reader.Dispose()
            $command.Dispose()
        }
    } finally {
        $connection.Dispose()
    }
} catch {
    [Console]::Error.WriteLine('No fue posible consultar el catálogo ODBC de solo lectura. No se mostraron credenciales, destino ni detalles internos.')
    exit 1
} finally {
    if ($passwordPointer -ne [IntPtr]::Zero) {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer)
    }
    $password = $null
}
