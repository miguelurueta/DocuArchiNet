param(
    [ValidateRange(1, 9223372036854775807)][long]$TaskId = 219887,
    [ValidateRange(0, 2147483647)][int]$RouteId = 0
)

$ErrorActionPreference = 'Stop'
if ((Read-Host 'Consulta DOC-56 exclusivamente SELECT en PRUEBAS. Escriba SI para continuar') -cne 'SI') { throw 'DOC56_TYPES_NOT_AUTHORIZED' }
$dbUser = Read-Host 'Usuario MySQL de solo lectura'
$securePassword = Read-Host 'Contrasena MySQL de solo lectura' -AsSecureString
$pointer = [IntPtr]::Zero
$connection = $null
function New-Parameter($command, $value) {
    $parameter = $command.CreateParameter()
    $parameter.Value = $value
    [void]$command.Parameters.Add($parameter)
}
function Read-Single($connection, $sql, $values) {
    $command = $connection.CreateCommand()
    $command.CommandText = $sql
    foreach ($value in $values) { New-Parameter $command $value }
    try { return $command.ExecuteScalar() } finally { $command.Dispose() }
}
try {
    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = 'workflowdocument'; $builder['UID'] = $dbUser.Trim(); $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()

    if ($RouteId -eq 0) {
        $resolvedRoute = Read-Single $connection @'
SELECT Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta
FROM ESTADOS_TAREA_WORKFLOW
WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=?
ORDER BY (FECHA_FIN IS NULL) DESC, FECHA_INICIO DESC
LIMIT 1
'@ @($TaskId)
        if ($null -eq $resolvedRoute -or [Convert]::IsDBNull($resolvedRoute) -or [int]$resolvedRoute -le 0) { throw 'DOC56_TYPES_ROUTE_NOT_FOUND' }
        $RouteId = [int]$resolvedRoute
    }

    $routeName = [string](Read-Single $connection 'SELECT Nombre_Ruta FROM rutas_workflow WHERE ID_RUTA=? LIMIT 1' @($RouteId))
    $procedureField = [string](Read-Single $connection 'SELECT Nombre_Campo FROM configuracion_listado_ruta WHERE campo_tramite=1 AND Rutas_Workflow_id_Ruta=? LIMIT 1' @($RouteId))
    if ($routeName -notmatch '^[A-Za-z0-9_]+$' -or $procedureField -notmatch '^[A-Za-z0-9_]+$') { throw 'DOC56_TYPES_ROUTE_METADATA_INVALID' }
    $taskTable = "dat_adic_tar$routeName"
    $procedureName = [string](Read-Single $connection "SELECT ``$procedureField`` FROM ``$taskTable`` WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=? LIMIT 1" @($TaskId))
    if ([string]::IsNullOrWhiteSpace($procedureName)) { throw 'DOC56_TYPES_PROCEDURE_NOT_FOUND' }
    Write-Host ('DOC56_TASK_ROUTE_ID={0}' -f $RouteId)
    Write-Host ('DOC56_TASK_PROCEDURE_NAME={0}' -f $procedureName)

    $schemaCommand = $connection.CreateCommand()
    $schemaCommand.CommandText = @'
SELECT table_schema FROM information_schema.tables
WHERE table_name IN ('tipo_doc_entrante','ra_dig_tipos_docum_lista_chequeo','tipo_doc_series')
  AND table_schema IN (
    SELECT table_schema FROM information_schema.columns
    WHERE table_name='tipo_doc_entrante'
      AND column_name IN ('id_Tipo_Doc_Entrante','nombre_gabinete_workflow',
                          'util_Estado_Crea_ExpedienteSII','util_Estado_Multiple_expedienteSII')
    GROUP BY table_schema HAVING COUNT(DISTINCT column_name)=4
  )
GROUP BY table_schema HAVING COUNT(DISTINCT table_name)=3 ORDER BY table_schema
'@
    $schemas = [Collections.Generic.List[string]]::new()
    $reader = $schemaCommand.ExecuteReader()
    try { while ($reader.Read()) { $schemas.Add([string]$reader.GetValue(0)) } } finally { $reader.Dispose(); $schemaCommand.Dispose() }
    $found = 0
    foreach ($schema in $schemas) {
        if ($schema -notmatch '^[A-Za-z0-9_]+$') { continue }
        $headerCommand = $connection.CreateCommand()
        $headerCommand.CommandText = @"
SELECT id_Tipo_Doc_Entrante,nombre_gabinete_workflow,
       util_Estado_Crea_ExpedienteSII,util_Estado_Multiple_expedienteSII
FROM ``$schema``.tipo_doc_entrante
WHERE Descripcion_Doc=?
ORDER BY id_Tipo_Doc_Entrante
LIMIT 2
"@
        New-Parameter $headerCommand $procedureName
        $headers = $headerCommand.ExecuteReader()
        try {
            while ($headers.Read()) {
                Write-Host ('DOC56_TASK_CONFIG_SCHEMA={0}' -f $schema)
                Write-Host ('DOC56_TASK_CONFIG_PROCEDURE_ID={0}' -f [string]$headers.GetValue(0))
                Write-Host ('DOC56_TASK_CONFIG_CABINET={0}' -f [string]$headers.GetValue(1))
                Write-Host ('DOC56_TASK_CONFIG_CREATE_ENABLED={0}' -f [string]$headers.GetValue(2))
                Write-Host ('DOC56_TASK_CONFIG_MULTIPLE_ENABLED={0}' -f [string]$headers.GetValue(3))
            }
        } finally { $headers.Dispose(); $headerCommand.Dispose() }
        $command = $connection.CreateCommand()
        $command.CommandText = @"
SELECT DISTINCT tds.Id_Tipo_Doc_Series, tds.Descripcion_Documento
FROM ``$schema``.tipo_doc_entrante tde
INNER JOIN ``$schema``.ra_dig_tipos_docum_lista_chequeo rdt
  ON rdt.tipo_doc_entrante_id_Tipo_Doc_Entrante=tde.id_Tipo_Doc_Entrante
INNER JOIN ``$schema``.tipo_doc_series tds
  ON tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series
WHERE tde.Descripcion_Doc=?
ORDER BY tds.Descripcion_Documento, tds.Id_Tipo_Doc_Series
"@
        New-Parameter $command $procedureName
        $rows = $command.ExecuteReader()
        try {
            while ($rows.Read()) {
                $found++
                Write-Host ('DOC56_ALLOWED_TYPE_{0}_ID={1}' -f $found, [string]$rows.GetValue(0))
                Write-Host ('DOC56_ALLOWED_TYPE_{0}_NAME={1}' -f $found, [string]$rows.GetValue(1))
            }
        } finally { $rows.Dispose(); $command.Dispose() }
    }
    Write-Host ('DOC56_ALLOWED_TYPE_COUNT={0}' -f $found)
    if ($found -eq 0) { throw 'DOC56_TYPES_NONE_FOUND' }
} catch {
    $code = if ($_.Exception.Message -match '^DOC56_TYPES_[A-Z0-9_]+$') { $_.Exception.Message } else { 'DOC56_TYPES_QUERY_FAILED' }
    Write-Error "$code. No se mostraron credenciales ni cadenas de conexion."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($pointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }
    $password = $null; $securePassword = $null
}
