param(
    [Parameter(Mandatory = $true)]
    [string]$Sql,

    [Parameter(Mandatory = $true)]
    [Int64]$TaskId,

    [ValidatePattern('^(?:DOC[0-9]+|NOTES)_E2E$')]
    [string]$EnvironmentPrefix = 'DOC32_E2E'
)

$ErrorActionPreference = 'Stop'
$stage = 'input'

try {
    $dsn = [Environment]::GetEnvironmentVariable(('{0}_ODBC_DSN' -f $EnvironmentPrefix))
    $user = [Environment]::GetEnvironmentVariable(('{0}_MYSQL_USER' -f $EnvironmentPrefix))
    $password = [Environment]::GetEnvironmentVariable(('{0}_MYSQL_PASSWORD' -f $EnvironmentPrefix))
    if ([string]::IsNullOrWhiteSpace($dsn) -or [string]::IsNullOrWhiteSpace($user) -or [string]::IsNullOrWhiteSpace($password)) { throw 'missing-input' }
    if ($dsn -notmatch '^[A-Za-z0-9 _.-]+$' -or $Sql -notmatch '^\s*SELECT\b' -or $Sql -match ';|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO)\b' -or ([regex]::Matches($Sql, '\?')).Count -ne 1) { throw 'invalid-input' }

    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $dsn
    $builder['UID'] = $user
    $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $stage = 'open'
    $connection.Open()
    try {
        $command = $connection.CreateCommand()
        $command.CommandText = $Sql
        # MySQL ODBC 5.2 accepts a 32-bit positional marker more reliably for
        # the normal Workflow task range, while preserving bigint support for
        # installations with larger identifiers.
        $parameterType = if ($TaskId -ge [Int32]::MinValue -and $TaskId -le [Int32]::MaxValue) {
            [System.Data.Odbc.OdbcType]::Integer
        } else {
            [System.Data.Odbc.OdbcType]::BigInt
        }
        $parameter = $command.Parameters.Add('@task', $parameterType)
        $parameter.Value = $TaskId
        $stage = 'execute'
        $reader = $command.ExecuteReader()
        try {
            $stage = 'read'
            $serializedRows = New-Object System.Text.StringBuilder
            while ($reader.Read()) {
                for ($column = 0; $column -lt $reader.FieldCount; $column += 1) {
                    [void]$serializedRows.Append($reader.GetName($column)).Append('=')
                    if ($reader.IsDBNull($column)) {
                        [void]$serializedRows.Append('<null>')
                    } elseif ($reader.GetFieldType($column) -eq [byte[]]) {
                        [void]$serializedRows.Append([Convert]::ToBase64String([byte[]]$reader.GetValue($column)))
                    } else {
                        [void]$serializedRows.Append([Convert]::ToString($reader.GetValue($column), [Globalization.CultureInfo]::InvariantCulture))
                    }
                    [void]$serializedRows.Append([char]31)
                }
                [void]$serializedRows.Append([char]30)
            }
            $stage = 'fingerprint'
            $bytes = [System.Text.Encoding]::UTF8.GetBytes($serializedRows.ToString())
            $hash = [System.Security.Cryptography.SHA256]::Create().ComputeHash($bytes)
            [Console]::Out.WriteLine(([System.BitConverter]::ToString($hash) -replace '-', '').ToLowerInvariant())
        } finally {
            $reader.Dispose()
            $command.Dispose()
        }
    } finally {
        $connection.Dispose()
    }
} catch {
    $rootException = $_.Exception
    while ($rootException.InnerException -ne $null) {
        $rootException = $rootException.InnerException
    }
    $marker = switch ($stage) {
        'open' { 'DOC32_ODBC_OPEN_FAILED' }
        'execute' { 'DOC32_ODBC_QUERY_FAILED' }
        'read' { 'DOC32_ODBC_RESULT_FAILED' }
        'fingerprint' { 'DOC32_ODBC_FINGERPRINT_FAILED' }
        default { 'DOC32_ODBC_INPUT_FAILED' }
    }
    if ($stage -eq 'execute' -and $rootException -is [System.Data.Odbc.OdbcException]) {
        $sqlStates = @($rootException.Errors | ForEach-Object { $_.SQLState })
        if ($sqlStates -contains '42S22') {
            $marker = 'DOC32_ODBC_COLUMN_UNAVAILABLE'
        } elseif ($sqlStates -contains '42S02') {
            $marker = 'DOC32_ODBC_TABLE_UNAVAILABLE'
        } elseif ($sqlStates | Where-Object { $_ -like '42*' }) {
            $marker = 'DOC32_ODBC_QUERY_UNSUPPORTED'
        }
    }
    [Console]::Error.WriteLine($marker)
    exit 1
}
