param(
    [Parameter(Mandatory = $true)]
    [ValidateRange(1, [Int64]::MaxValue)]
    [Int64]$TaskId,

    [ValidatePattern('^[A-Za-z0-9 _.-]+$')]
    [string]$Dsn = 'workflowconta'
)

$ErrorActionPreference = 'Stop'
$passwordPointer = [IntPtr]::Zero
$password = $null
$stage = 'input'

function Add-TaskParameter {
    param([System.Data.Odbc.OdbcCommand]$Command, [Int64]$Value)

    $parameterType = if ($Value -ge [Int32]::MinValue -and $Value -le [Int32]::MaxValue) {
        [System.Data.Odbc.OdbcType]::Integer
    } else {
        [System.Data.Odbc.OdbcType]::BigInt
    }
    $parameter = $Command.Parameters.Add('@task', $parameterType)
    $parameter.Value = $Value
}

function Invoke-QueryShape {
    param([System.Data.Odbc.OdbcConnection]$Connection, [string]$Sql, [Int64]$Value)

    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $Sql
        Add-TaskParameter $command $Value
        $reader = $command.ExecuteReader()
        try {
            # Sólo valida el esquema y la forma de la consulta: nunca muestra filas.
            [void]$reader.Read()
        } finally {
            $reader.Dispose()
        }
    } finally {
        $command.Dispose()
    }
}

function Get-FirstPositiveInteger {
    param([System.Data.Odbc.OdbcConnection]$Connection, [string]$Sql, [Int64]$Value)

    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $Sql
        Add-TaskParameter $command $Value
        $reader = $command.ExecuteReader()
        try {
            if (-not $reader.Read() -or $reader.IsDBNull(0)) { return 0 }
            $number = [Convert]::ToInt64($reader.GetValue(0), [Globalization.CultureInfo]::InvariantCulture)
            return [Math]::Max(0, $number)
        } finally {
            $reader.Dispose()
        }
    } finally {
        $command.Dispose()
    }
}

function Get-FlowPrerequisiteMarker {
    param([System.Data.Odbc.OdbcConnection]$Connection, [Int64]$ConnectorId)

    $sql = @'
SELECT Estado_soicita_autorizacion,
       Estado_soicita_autorizacion_firma_digital,
       Estado_copia_estructura_total,
       Estado_copia_estructura,
       Estado_firma_digital,
       Estado_asigna_expediente
FROM wf_registro_conectores_actividades_envio_flujo_trabajo
WHERE ID_REGISTRO_ACTIVIDAD_ENVIO = ?
'@
    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $sql
        Add-TaskParameter $command $ConnectorId
        $reader = $command.ExecuteReader()
        try {
            if (-not $reader.Read()) { return 'DOC32_RETURN_FLOW_CONNECTOR_CONFIG_UNAVAILABLE' }
            $flags = for ($column = 0; $column -lt 6; $column += 1) {
                if ($reader.IsDBNull($column)) { 0 } else { [Convert]::ToInt64($reader.GetValue($column), [Globalization.CultureInfo]::InvariantCulture) }
            }
            if ($flags[0] -ne 0) { return 'DOC32_RETURN_FLOW_PREREQUISITE_AUTHORIZATION_CONFIGURED' }
            if ($flags[1] -ne 0) { return 'DOC32_RETURN_FLOW_PREREQUISITE_SIGNATURE_AUTHORIZATION_CONFIGURED' }
            if ($flags[2] -ne 0) { return 'DOC32_RETURN_FLOW_PREREQUISITE_FULL_DOCUMENT_COPY_CONFIGURED' }
            if ($flags[3] -ne 0) { return 'DOC32_RETURN_FLOW_PREREQUISITE_DOCUMENT_COPY_CONFIGURED' }
            if ($flags[4] -ne 0) { return 'DOC32_RETURN_FLOW_PREREQUISITE_SIGNATURE_CONFIGURED' }
            if ($flags[5] -ne 0) { return 'DOC32_RETURN_FLOW_PREREQUISITE_EXPEDIENT_CONFIGURED' }
            return 'DOC32_RETURN_FLOW_PREREQUISITES_CLEAR'
        } finally {
            $reader.Dispose()
        }
    } finally {
        $command.Dispose()
    }
}

function Find-MissingFlowColumn {
    param([System.Data.Odbc.OdbcConnection]$Connection)

    $required = @(
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'ID_REGISTRO_ACTIVIDAD_ENVIO' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'ID_ACTIVIDAD_FUENTE' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'ID_USUARIO_WORKFLOW_FUENTE' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'Estado_evia_correo' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'Estado_soicita_autorizacion' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'Estado_soicita_autorizacion_firma_digital' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'Estado_copia_estructura_total' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'Estado_copia_estructura' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'Estado_firma_digital' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'Estado_asigna_expediente' },
        @{ Table = 'wf_registro_conectores_actividades_envio_flujo_trabajo'; Column = 'wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO' },
        @{ Table = 'wf_registro_actividaes_flujos_trabajo'; Column = 'ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO' },
        @{ Table = 'wf_registro_actividaes_flujos_trabajo'; Column = 'listado_actividades_workflow_id_Actividad' },
        @{ Table = 'wf_registro_actividaes_flujos_trabajo'; Column = 'wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO' },
        @{ Table = 'listado_actividades_workflow'; Column = 'ID_ACTIVIDAD' },
        @{ Table = 'listado_actividades_workflow'; Column = 'NOMBRE_ACTIVIDAD' },
        @{ Table = 'listado_actividades_workflow'; Column = 'RUTAS_WORKFLOW_ID_RUTA' },
        @{ Table = 'usuario_workflow'; Column = 'idU_suario' },
        @{ Table = 'usuario_workflow'; Column = 'ESTADO_USUARIO' },
        @{ Table = 'usuario_workflow'; Column = 'Nombre_Usuario' },
        @{ Table = 'usuario_workflow'; Column = 'Cargo_Usuario' },
        @{ Table = 'usuario_workflow'; Column = 'GRUPOS_WORKFLOW_ID_GRUPO' },
        @{ Table = 'grupos_workflow'; Column = 'ID_GRUPO' },
        @{ Table = 'grupos_workflow'; Column = 'ID_ACTIVIDAD' },
        @{ Table = 'grupos_workflow'; Column = 'RUTAS_WORKFLOW_ID_RUTA' },
        @{ Table = 'grupos_workflow'; Column = 'NOMBRE_GRUPO' }
    )
    $tables = @($required | ForEach-Object { $_.Table } | Select-Object -Unique)
    $quotedTables = ($tables | ForEach-Object { "'$_'" }) -join ', '
    $sql = "SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME IN ($quotedTables)"
    $available = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $sql
        $reader = $command.ExecuteReader()
        try {
            while ($reader.Read()) {
                [void]$available.Add(('{0}.{1}' -f $reader.GetString(0), $reader.GetString(1)))
            }
        } finally {
            $reader.Dispose()
        }
    } finally {
        $command.Dispose()
    }
    foreach ($item in $required) {
        $key = '{0}.{1}' -f $item.Table, $item.Column
        if (-not $available.Contains($key)) {
            return ('{0}_{1}' -f $item.Table.ToUpperInvariant(), $item.Column.ToUpperInvariant())
        }
    }
    return $null
}

$sqlEstado = @'
SELECT estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta AS ID_RUTA,
       estado.ID_FLUJO_TRABAJO AS ID_FLUJO,
       estado.ID_ACTIVIDAD_FLUJO_TRABAJO AS ID_ACTIVIDAD_FLUJO
FROM estados_tarea_workflow AS estado
WHERE estado.Inicio_Tareas_Workflow_id_Tarea = ?
  AND estado.FECHA_SELECCION IS NOT NULL
  AND estado.FECHA_FIN IS NULL
  AND estado.ESTADO_TAREA = 0
ORDER BY estado.id_Estado DESC
LIMIT 1
'@

$sqlRuta = @'
SELECT disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO AS ID_CONECTOR,
       origen.ID_ACTIVIDAD AS ID_ACTIVIDAD_ORIGEN,
       destino.ID_ACTIVIDAD AS ID_ACTIVIDAD_DESTINO,
       COALESCE(MIN(grupoDestino.ID_GRUPO), 0) AS ID_GRUPO_DESTINO,
       origen.NOMBRE_ACTIVIDAD AS NOMBRE_ACTIVIDAD,
       MIN(grupoDestino.NOMBRE_GRUPO) AS NOMBRE_GRUPO,
       COALESCE(disponible.Estado_evia_correo, 0) AS ESTADO_CORREO
FROM estados_tarea_workflow AS estado
INNER JOIN actividades_disponibles_envio AS disponible
  ON disponible.ID_ACTIVIDAD_SIGUIENTE = estado.Id_Actividad
 AND disponible.id_Ruta = estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta
INNER JOIN listado_actividades_workflow AS origen
  ON origen.ID_ACTIVIDAD = disponible.Listado_Actividades_Workflow_Id_Actividad
INNER JOIN listado_actividades_workflow AS destino
  ON destino.ID_ACTIVIDAD = disponible.ID_ACTIVIDAD_SIGUIENTE
LEFT JOIN grupos_workflow AS grupoDestino
  ON grupoDestino.ID_ACTIVIDAD = origen.ID_ACTIVIDAD
 AND grupoDestino.RUTAS_WORKFLOW_ID_RUTA = estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta
WHERE estado.Inicio_Tareas_Workflow_id_Tarea = ?
  AND estado.FECHA_SELECCION IS NOT NULL
  AND estado.FECHA_FIN IS NULL
  AND estado.ESTADO_TAREA = 0
  AND origen.RUTAS_WORKFLOW_ID_RUTA = estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta
  AND destino.RUTAS_WORKFLOW_ID_RUTA = estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta
GROUP BY disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO, origen.ID_ACTIVIDAD,
         destino.ID_ACTIVIDAD, origen.NOMBRE_ACTIVIDAD, disponible.Estado_evia_correo
ORDER BY origen.NOMBRE_ACTIVIDAD, disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO
LIMIT 51 OFFSET 0
'@

$sqlFlujo = @'
SELECT conector.ID_REGISTRO_ACTIVIDAD_ENVIO AS ID_CONECTOR,
       origenFlujo.listado_actividades_workflow_id_Actividad AS ID_ACTIVIDAD_ORIGEN,
       conector.ID_ACTIVIDAD_FUENTE AS ID_ACTIVIDAD_DESTINO,
       conector.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE AS ID_ACTIVIDAD_FLUJO_ORIGEN,
       conector.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO AS ID_ACTIVIDAD_FLUJO_DESTINO,
       COALESCE(conector.ID_USUARIO_WORKFLOW_FUENTE, 0) AS ID_USUARIO_DESTINO,
       COALESCE(MIN(grupoOrigen.ID_GRUPO), 0) AS ID_GRUPO_DESTINO,
       origen.NOMBRE_ACTIVIDAD AS NOMBRE_ACTIVIDAD,
       CONCAT_WS(' - ', usuario.Nombre_Usuario, usuario.Cargo_Usuario) AS NOMBRE_USUARIO,
       MIN(grupoOrigen.NOMBRE_GRUPO) AS NOMBRE_GRUPO,
       COALESCE(conector.Estado_evia_correo, 0) AS ESTADO_CORREO
FROM estados_tarea_workflow AS estado
INNER JOIN wf_registro_conectores_actividades_envio_flujo_trabajo AS conector
  ON conector.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = estado.ID_FLUJO_TRABAJO
 AND conector.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO = estado.ID_ACTIVIDAD_FLUJO_TRABAJO
INNER JOIN wf_registro_actividaes_flujos_trabajo AS origenFlujo
  ON origenFlujo.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = conector.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE
 AND origenFlujo.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = conector.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO
INNER JOIN listado_actividades_workflow AS origen
  ON origen.ID_ACTIVIDAD = origenFlujo.listado_actividades_workflow_id_Actividad
LEFT JOIN usuario_workflow AS usuario
  ON usuario.idU_suario = conector.ID_USUARIO_WORKFLOW_FUENTE
 AND usuario.ESTADO_USUARIO = 1
LEFT JOIN grupos_workflow AS grupoOrigen
  ON grupoOrigen.ID_ACTIVIDAD = origen.ID_ACTIVIDAD
 AND grupoOrigen.RUTAS_WORKFLOW_ID_RUTA = estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta
WHERE estado.Inicio_Tareas_Workflow_id_Tarea = ?
  AND estado.FECHA_SELECCION IS NOT NULL
  AND estado.FECHA_FIN IS NULL
  AND estado.ESTADO_TAREA = 0
  AND origen.RUTAS_WORKFLOW_ID_RUTA = estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta
  AND ((COALESCE(conector.ID_USUARIO_WORKFLOW_FUENTE, 0) > 0
        AND EXISTS (SELECT 1
                    FROM usuario_workflow AS usuarioValido
                    INNER JOIN grupos_workflow AS grupoValido
                      ON grupoValido.ID_GRUPO = usuarioValido.GRUPOS_WORKFLOW_ID_GRUPO
                    WHERE grupoValido.RUTAS_WORKFLOW_ID_RUTA = estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta
                      AND grupoValido.ID_ACTIVIDAD = origen.ID_ACTIVIDAD
                      AND usuarioValido.idU_suario = conector.ID_USUARIO_WORKFLOW_FUENTE
                      AND usuarioValido.ESTADO_USUARIO = 1)
       ) OR (COALESCE(conector.ID_USUARIO_WORKFLOW_FUENTE, 0) = 0
           AND EXISTS (SELECT 1
                       FROM grupos_workflow AS grupoValido
                       WHERE grupoValido.ID_ACTIVIDAD = origen.ID_ACTIVIDAD
                         AND grupoValido.RUTAS_WORKFLOW_ID_RUTA = estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta)))
GROUP BY conector.ID_REGISTRO_ACTIVIDAD_ENVIO,
         origenFlujo.listado_actividades_workflow_id_Actividad,
         conector.ID_ACTIVIDAD_FUENTE,
         conector.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE,
         conector.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,
         conector.ID_USUARIO_WORKFLOW_FUENTE,
         origen.NOMBRE_ACTIVIDAD,
         usuario.Nombre_Usuario,
         usuario.Cargo_Usuario,
         conector.Estado_evia_correo
ORDER BY origen.NOMBRE_ACTIVIDAD, conector.ID_REGISTRO_ACTIVIDAD_ENVIO
LIMIT 51 OFFSET 0
'@

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
    $stage = 'open'
    $connection.Open()
    try {
        $stage = 'state'
        $command = $connection.CreateCommand()
        try {
            $command.CommandText = $sqlEstado
            Add-TaskParameter $command $TaskId
            $reader = $command.ExecuteReader()
            try {
                if (-not $reader.Read()) {
                    [Console]::Out.WriteLine('DOC32_RETURN_NO_ACTIVE_STATE')
                    exit 0
                }
                $idFlujo = if ($reader.IsDBNull(1)) { 0 } else { [Convert]::ToInt64($reader.GetValue(1), [Globalization.CultureInfo]::InvariantCulture) }
                $idActividadFlujo = if ($reader.IsDBNull(2)) { 0 } else { [Convert]::ToInt64($reader.GetValue(2), [Globalization.CultureInfo]::InvariantCulture) }
            } finally {
                $reader.Dispose()
            }
        } finally {
            $command.Dispose()
        }

        if ($idFlujo -gt 0 -and $idActividadFlujo -gt 0) {
            $stage = 'flow-query'
            $missingFlowColumn = Find-MissingFlowColumn $connection
            if (-not [string]::IsNullOrWhiteSpace($missingFlowColumn)) {
                [Console]::Error.WriteLine("DOC32_RETURN_FLOW_COLUMN_MISSING_$missingFlowColumn")
                exit 1
            }
            $connectorId = Get-FirstPositiveInteger $connection $sqlFlujo $TaskId
            if ($connectorId -le 0) {
                [Console]::Error.WriteLine('DOC32_RETURN_FLOW_CONNECTOR_UNAVAILABLE')
                exit 1
            }
            [Console]::Out.WriteLine('DOC32_RETURN_FLOW_QUERY_OK')
            [Console]::Out.WriteLine((Get-FlowPrerequisiteMarker $connection $connectorId))
        } else {
            $stage = 'route-query'
            Invoke-QueryShape $connection $sqlRuta $TaskId
            [Console]::Out.WriteLine('DOC32_RETURN_ROUTE_QUERY_OK')
        }
    } finally {
        $connection.Dispose()
    }
} catch {
    $rootException = $_.Exception
    while ($rootException.InnerException -ne $null) { $rootException = $rootException.InnerException }
    $marker = "DOC32_RETURN_$stage`_FAILED"
    if ($rootException -is [System.Data.Odbc.OdbcException]) {
        $sqlStates = @($rootException.Errors | ForEach-Object { $_.SQLState })
        if ($sqlStates -contains '42S22') {
            $unknownColumn = $null
            foreach ($odbcError in $rootException.Errors) {
                if ($odbcError.Message -match "(?i)Unknown column ['\u0060](?<identifier>[A-Za-z0-9_]+(?:\.[A-Za-z0-9_]+)?)['\u0060]") {
                    $unknownColumn = $Matches.identifier
                    break
                }
            }
            if ([string]::IsNullOrWhiteSpace($unknownColumn)) {
                $marker = "DOC32_RETURN_$stage`_COLUMN_UNAVAILABLE"
            } else {
                $safeIdentifier = ($unknownColumn.ToUpperInvariant().Split('.')) -join '_'
                $marker = "DOC32_RETURN_$stage`_UNKNOWN_COLUMN_$safeIdentifier"
            }
        }
        elseif ($sqlStates -contains '42S02') { $marker = "DOC32_RETURN_$stage`_TABLE_UNAVAILABLE" }
        elseif ($sqlStates | Where-Object { $_ -like '42*' }) { $marker = "DOC32_RETURN_$stage`_SQL_UNSUPPORTED" }
    }
    [Console]::Error.WriteLine($marker)
    exit 1
} finally {
    if ($passwordPointer -ne [IntPtr]::Zero) {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer)
    }
    $password = $null
}
