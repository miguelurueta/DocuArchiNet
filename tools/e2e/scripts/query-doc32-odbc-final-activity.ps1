param(
    [Parameter(Mandatory = $true)]
    [Int64]$TaskId
)

$ErrorActionPreference = 'Stop'
$stage = 'input'

function Normalize-ActivityName([object]$value) {
    if ($null -eq $value) { return '' }
    return ([string]$value).Normalize([Text.NormalizationForm]::FormKC).Trim().ToUpperInvariant()
}

try {
    $dsn = [Environment]::GetEnvironmentVariable('DOC32_E2E_ODBC_DSN')
    $user = [Environment]::GetEnvironmentVariable('DOC32_E2E_MYSQL_USER')
    $password = [Environment]::GetEnvironmentVariable('DOC32_E2E_MYSQL_PASSWORD')
    $expectedActivity = [Environment]::GetEnvironmentVariable('DOC32_E2E_EXPECTED_ACTIVITY_NAME')
    if ([string]::IsNullOrWhiteSpace($dsn) -or [string]::IsNullOrWhiteSpace($user) -or
        [string]::IsNullOrWhiteSpace($password) -or [string]::IsNullOrWhiteSpace($expectedActivity)) { throw 'missing-input' }
    if ($dsn -notmatch '^[A-Za-z0-9 _.-]+$') { throw 'invalid-dsn' }

    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = $dsn
    $builder['UID'] = $user
    $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $stage = 'open'
    $connection.Open()
    try {
        $command = $connection.CreateCommand()
        $command.CommandText = @'
SELECT actividad.NOMBRE_ACTIVIDAD
FROM estados_tarea_workflow AS estado
INNER JOIN listado_actividades_workflow AS actividad
  ON actividad.ID_ACTIVIDAD = estado.Id_Actividad
WHERE estado.Inicio_Tareas_Workflow_id_Tarea = ?
  AND estado.Fecha_Fin IS NULL
ORDER BY estado.id_Estado DESC
LIMIT 1
'@
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
            $activeActivities = New-Object System.Collections.Generic.List[string]
            while ($reader.Read()) {
                if (-not $reader.IsDBNull(0)) {
                    $activeActivities.Add([Convert]::ToString($reader.GetValue(0), [Globalization.CultureInfo]::InvariantCulture))
                }
            }
            $expected = Normalize-ActivityName $expectedActivity
            $marker = if ($activeActivities.Count -ne 1) {
                'DOC32_ODBC_FINAL_ACTIVITY_AMBIGUOUS'
            } elseif ((Normalize-ActivityName $activeActivities[0]) -eq $expected) {
                'DOC32_ODBC_FINAL_ACTIVITY_MATCH'
            } else {
                'DOC32_ODBC_FINAL_ACTIVITY_MISMATCH'
            }
            [Console]::Out.WriteLine($marker)
        } finally {
            $reader.Dispose()
            $command.Dispose()
        }
    } finally {
        $connection.Dispose()
    }
} catch {
    $marker = switch ($stage) {
        'open' { 'DOC32_ODBC_FINAL_ACTIVITY_OPEN_FAILED' }
        'execute' { 'DOC32_ODBC_FINAL_ACTIVITY_QUERY_FAILED' }
        'read' { 'DOC32_ODBC_FINAL_ACTIVITY_READ_FAILED' }
        default { 'DOC32_ODBC_FINAL_ACTIVITY_INPUT_FAILED' }
    }
    [Console]::Error.WriteLine($marker)
    exit 1
}
