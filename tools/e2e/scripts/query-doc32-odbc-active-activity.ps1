param(
    [Parameter(Mandatory = $true)]
    [Int64]$TaskId,

    [ValidatePattern('^DOC[0-9]+_E2E$')]
    [string]$EnvironmentPrefix = 'DOC32_E2E'
)

$ErrorActionPreference = 'Stop'
$stage = 'input'

try {
    $dsn = [Environment]::GetEnvironmentVariable(('{0}_ODBC_DSN' -f $EnvironmentPrefix))
    $user = [Environment]::GetEnvironmentVariable(('{0}_MYSQL_USER' -f $EnvironmentPrefix))
    $password = [Environment]::GetEnvironmentVariable(('{0}_MYSQL_PASSWORD' -f $EnvironmentPrefix))
    if ($TaskId -le 0 -or [string]::IsNullOrWhiteSpace($dsn) -or [string]::IsNullOrWhiteSpace($user) -or [string]::IsNullOrWhiteSpace($password)) { throw 'missing-input' }
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
LIMIT 2
'@
        $parameterType = if ($TaskId -ge [Int32]::MinValue -and $TaskId -le [Int32]::MaxValue) { [System.Data.Odbc.OdbcType]::Integer } else { [System.Data.Odbc.OdbcType]::BigInt }
        $parameter = $command.Parameters.Add('@task', $parameterType)
        $parameter.Value = $TaskId
        $stage = 'execute'
        $reader = $command.ExecuteReader()
        try {
            $stage = 'read'
            $activities = New-Object System.Collections.Generic.List[string]
            while ($reader.Read() -and $activities.Count -lt 2) {
                if (-not $reader.IsDBNull(0)) { $activities.Add([Convert]::ToString($reader.GetValue(0), [Globalization.CultureInfo]::InvariantCulture)) }
            }
            if ($activities.Count -ne 1) {
                [Console]::Out.WriteLine('WORKFLOW_ODBC_ACTIVE_ACTIVITY_AMBIGUOUS')
            } else {
                $activity = $activities[0].Normalize([Text.NormalizationForm]::FormKC).Trim()
                if ([string]::IsNullOrWhiteSpace($activity) -or $activity.Length -gt 160 -or $activity -match '[\r\n]') { throw 'invalid-activity' }
                [Console]::Out.WriteLine("WORKFLOW_ODBC_ACTIVE_ACTIVITY=$activity")
            }
        } finally {
            $reader.Dispose()
            $command.Dispose()
        }
    } finally {
        $connection.Dispose()
    }
} catch {
    $marker = switch ($stage) {
        'open' { 'WORKFLOW_ODBC_ACTIVE_ACTIVITY_OPEN_FAILED' }
        'execute' { 'WORKFLOW_ODBC_ACTIVE_ACTIVITY_QUERY_FAILED' }
        'read' { 'WORKFLOW_ODBC_ACTIVE_ACTIVITY_READ_FAILED' }
        default { 'WORKFLOW_ODBC_ACTIVE_ACTIVITY_INPUT_FAILED' }
    }
    [Console]::Error.WriteLine($marker)
    exit 1
}
