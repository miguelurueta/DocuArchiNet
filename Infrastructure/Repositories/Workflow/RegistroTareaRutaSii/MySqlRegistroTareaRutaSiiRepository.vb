Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

Public NotInheritable Class MySqlRegistroTareaRutaSiiRepository
    Implements IRegistroTareaRutaSiiRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections
        _executor = executor
    End Sub

    Public Function TienePermiso(ByVal contexto As ContextoModulo) As Boolean Implements IRegistroTareaRutaSiiRepository.TienePermiso
        Using connection = _connections.CreateOpenConnection(contexto)
            Dim value = _executor.ExecuteScalar(connection, Nothing,
                "SELECT UTIL_SII_REGISTRO_TAREA_RUTA FROM PERMISOS_USUARIO_WORKFLOW WHERE Usuario_Workflow_idU_suario=@userId LIMIT 1",
                Params(New MySqlParameter("@userId", contexto.IdUsuario)))
            Return Convert.ToInt32(If(value Is Nothing OrElse value Is DBNull.Value, 0, value), CultureInfo.InvariantCulture) = 1
        End Using
    End Function

    Public Function ResolverContexto(ByVal contexto As ContextoModulo,
                                     ByVal solicitud As SolicitudRegistroTareaRutaSii,
                                     ByVal sii As ConsultaAutoritativaRegistroRutaSii) As DatosAutoritativosRegistroRutaSii Implements IRegistroTareaRutaSiiRepository.ResolverContexto
        Using connection = _connections.CreateOpenConnection(contexto)
            Dim datos = _executor.ExecuteReader(connection, Nothing,
                "SELECT t.nombre_tramite,t.tipo_gabinete,t.descripcion_tramite,c.Nombre_Gabinete " &
                "FROM ws_tipotramitesii_determina_gabinete t INNER JOIN configuracion_gabinete c ON c.id_Gabinete=t.tipo_gabinete " &
                "WHERE t.id_tipotramiteSII=@procedureId",
                Params(New MySqlParameter("@procedureId", solicitud.id_tramite)),
                Function(reader)
                    If Not reader.Read() Then Return Nothing
                    Return New DatosAutoritativosRegistroRutaSii With {
                        .IdTramite = solicitud.id_tramite,
                        .NombreTramite = Convert.ToString(reader.GetValue(0)),
                        .IdGabinete = Convert.ToInt32(reader.GetValue(1), CultureInfo.InvariantCulture),
                        .DescripcionTramite = Convert.ToString(reader.GetValue(2)),
                        .NombreGabinete = Convert.ToString(reader.GetValue(3))}
                End Function)
            If datos Is Nothing Then Return Nothing
            If sii Is Nothing OrElse sii.Recibo Is Nothing OrElse sii.Radicado Is Nothing Then Return Nothing
            Dim subtipo = Convert.ToString(sii.Radicado.subtipotramite).Trim()
            Dim tipoRecibo = Convert.ToString(sii.Recibo.tipotramite).Trim()
            Dim tipoEfectivo = If(Not String.IsNullOrWhiteSpace(subtipo), subtipo, tipoRecibo)
            If String.IsNullOrWhiteSpace(tipoEfectivo) OrElse
               Not String.Equals(Convert.ToString(datos.NombreTramite).Trim(), tipoEfectivo.Trim(), StringComparison.OrdinalIgnoreCase) Then Return Nothing

            datos.CodigoSede = _executor.ExecuteReader(connection, Nothing,
                "SELECT s.codigo_sede_sii FROM LISTADO_ACTIVIDADES_WORKFLOW a INNER JOIN ws_sedes_workflow s ON s.listado_actividades_workflow_id_actividad=a.ID_ACTIVIDAD WHERE a.ID_ACTIVIDAD=@activityId",
                Params(New MySqlParameter("@activityId", solicitud.id_actividad)),
                Function(reader) If(reader.Read(), Convert.ToString(reader.GetValue(0)), Nothing))
            If String.IsNullOrWhiteSpace(datos.CodigoSede) Then Return Nothing

            Dim route = _executor.ExecuteReader(connection, Nothing,
                "SELECT id_Ruta,Nombre_Ruta FROM rutas_workflow WHERE Estado_Ruta=1 AND UPPER(Nombre_Ruta)='REGISTROPUBLICO' LIMIT 1",
                Params(),
                Function(reader) If(reader.Read(), New KeyValuePair(Of Integer, String)(Convert.ToInt32(reader.GetValue(0)), Convert.ToString(reader.GetValue(1))), New KeyValuePair(Of Integer, String)()))
            If route.Key <= 0 Then Return Nothing
            datos.IdRuta = route.Key
            datos.NombreRuta = route.Value
            datos.Recibo = solicitud.recibo
            datos.CodigoBarras = Convert.ToString(sii.Radicado.radicado).Trim()
            datos.Matricula = Convert.ToString(sii.Radicado.matricula).Trim()
            datos.RazonSocial = Convert.ToString(sii.Radicado.nombre).Trim()
            datos.SubtipoTramite = subtipo
            datos.IdActividad = solicitud.id_actividad
            Return datos
        End Using
    End Function

    Public Function RegistrarTareaConOutbox(ByVal contexto As ContextoModulo,
                                            ByVal datos As DatosAutoritativosRegistroRutaSii) As ResultadoRegistroTareaRutaSii Implements IRegistroTareaRutaSiiRepository.RegistrarTareaConOutbox
        If datos Is Nothing OrElse Not Regex.IsMatch(datos.NombreRuta, "^[A-Za-z0-9_]+$") Then Return Failure("INVALID_ROUTE")
        Using connection = DirectCast(_connections.CreateOpenConnection(contexto), MySqlConnection)
            Dim lockName = "doc87-route-" & datos.IdRuta.ToString(CultureInfo.InvariantCulture) & "-" & datos.Recibo
            Dim acquired As Boolean = False
            Try
                acquired = Convert.ToString(Scalar(connection, Nothing, "SELECT GET_LOCK(@lockName,5)", New MySqlParameter("@lockName", lockName)), CultureInfo.InvariantCulture) = "1"
                If Not acquired Then Return Failure("REGISTRATION_IN_PROGRESS")
                Dim existing = LoadEvent(connection, datos.IdRuta, datos.Recibo)
                If existing IsNot Nothing Then Return New ResultadoRegistroTareaRutaSii With {.Codigo = "ALREADY_REGISTERED", .Evento = existing}
                If ReceiptExists(connection, datos.Recibo) Then Return Failure("ALREADY_REGISTERED")

                Using transaction = connection.BeginTransaction()
                    Try
                        Dim nowValue = DateTime.Now
                        NonQuery(connection, transaction,
                            "INSERT INTO F_W_E_REGISTROPUBLICO (DATOS_RECIBO,CODIGO_BARRAS,SECUENCIA_DOCUMENTO,RAZON_SOCIAL,MATRICULA,COD_SEDE,SECUENCIA_SERVICIO,TIPO_DOCUMENTO,FECHA_DOCUMENTO,INSCRIPCION_DOCUMENTO,ID_GABINETE,NOMBRE_GABINETE,ID_RUTA,FLAG) " &
                            "VALUES (@receipt,@barcode,0,@name,@enrollment,@site,'0',@description,@created,'0',@cabinetId,@cabinetName,@routeId,'1')",
                            New MySqlParameter("@receipt", datos.Recibo), New MySqlParameter("@barcode", datos.CodigoBarras), New MySqlParameter("@name", Left(datos.RazonSocial, 120)), New MySqlParameter("@enrollment", NullIfEmpty(datos.Matricula)), New MySqlParameter("@site", datos.CodigoSede), New MySqlParameter("@description", Left(datos.DescripcionTramite, 120)), New MySqlParameter("@created", nowValue), New MySqlParameter("@cabinetId", datos.IdGabinete), New MySqlParameter("@cabinetName", datos.NombreGabinete), New MySqlParameter("@routeId", datos.IdRuta))
                        Dim taskId = Insert(connection, transaction,
                            "INSERT INTO INICIO_TAREAS_WORKFLOW (Rutas_Workflow_id_Ruta,Fecha_Ini_Workflow,Flag_sistema,id_dat_ext) VALUES (@routeId,@created,1,0)",
                            New MySqlParameter("@routeId", datos.IdRuta), New MySqlParameter("@created", nowValue))
                        Dim detailTable = "DAT_ADIC_TAR" & datos.NombreRuta
                        NonQuery(connection, transaction,
                            "INSERT INTO " & detailTable & " (DATOS_RECIBO,CODIGO_BARRAS,RAZON_SOCIAL,MATRICULA,SEDE,NOMBRE_GABINETE,FLUJO_INTERNO_WF,TRAMITE,DESCRIPCIONTRAMITE,INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_GABINETE,ID_IMAGEN,FLUJO_TRABAJO_WF) " &
                            "VALUES (@receipt,@barcode,@name,@enrollment,@site,@cabinetName,2,@procedure,@description,@taskId,@cabinetId,NULL,0)",
                            New MySqlParameter("@receipt", datos.Recibo), New MySqlParameter("@barcode", datos.CodigoBarras), New MySqlParameter("@name", Left(datos.RazonSocial, 120)), New MySqlParameter("@enrollment", NullIfEmpty(datos.Matricula)), New MySqlParameter("@site", datos.CodigoSede), New MySqlParameter("@cabinetName", datos.NombreGabinete), New MySqlParameter("@procedure", Left(datos.NombreTramite, 120)), New MySqlParameter("@description", Left(datos.DescripcionTramite, 120)), New MySqlParameter("@taskId", taskId), New MySqlParameter("@cabinetId", datos.IdGabinete))
                        NonQuery(connection, transaction,
                            "INSERT INTO ESTADOS_TAREA_WORKFLOW (Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta,Inicio_Tareas_Workflow_id_Tarea,Id_Actividad,FECHA_INICIO,ESTADO_PRIORIDAD,ESTADO_TAREA,Id_Usuario,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO) VALUES (@routeId,@taskId,@activityId,@created,0,0,NULL,0,0,0)",
                            New MySqlParameter("@routeId", datos.IdRuta), New MySqlParameter("@taskId", taskId), New MySqlParameter("@activityId", datos.IdActividad), New MySqlParameter("@created", nowValue))
                        Dim operationId = Guid.NewGuid().ToString("D")
                        NonQuery(connection, transaction,
                            "INSERT INTO workflow_registro_ruta_sii_outbox (operation_id,route_id,route_name,task_id,receipt,enrollment,cabinet_name,status,attempts,created_at,updated_at) VALUES (@operationId,@routeId,@routeName,@taskId,@receipt,@enrollment,@cabinetName,'PENDING',0,@created,@created)",
                            New MySqlParameter("@operationId", operationId), New MySqlParameter("@routeId", datos.IdRuta), New MySqlParameter("@routeName", datos.NombreRuta), New MySqlParameter("@taskId", taskId), New MySqlParameter("@receipt", datos.Recibo), New MySqlParameter("@enrollment", datos.Matricula), New MySqlParameter("@cabinetName", datos.NombreGabinete), New MySqlParameter("@created", nowValue))
                        transaction.Commit()
                        Return New ResultadoRegistroTareaRutaSii With {.Codigo = "REGISTERED", .Evento = New EventoRelacionRutaSii With {.OperationId = operationId, .IdTarea = taskId, .IdRuta = datos.IdRuta, .Recibo = datos.Recibo, .Matricula = datos.Matricula, .NombreGabinete = datos.NombreGabinete}}
                    Catch
                        transaction.Rollback()
                        Throw
                    End Try
                End Using
            Finally
                If acquired Then
                    Try
                        Scalar(connection, Nothing, "SELECT RELEASE_LOCK(@lockName)", New MySqlParameter("@lockName", lockName))
                    Catch
                    End Try
                End If
            End Try
        End Using
    End Function

    Public Sub ConfirmarEvento(ByVal contexto As ContextoModulo, ByVal operationId As String, ByVal estado As String) Implements IRegistroTareaRutaSiiRepository.ConfirmarEvento
        Using connection = DirectCast(_connections.CreateOpenConnection(contexto), MySqlConnection)
            NonQuery(connection, Nothing, "UPDATE workflow_registro_ruta_sii_outbox SET status=@status,attempts=attempts+1,last_error_code=NULL,updated_at=@updated,next_attempt_at=NULL WHERE operation_id=@operationId",
                     New MySqlParameter("@status", estado), New MySqlParameter("@updated", DateTime.Now), New MySqlParameter("@operationId", operationId))
        End Using
    End Sub

    Public Sub MarcarEventoPendiente(ByVal contexto As ContextoModulo, ByVal operationId As String, ByVal codigo As String) Implements IRegistroTareaRutaSiiRepository.MarcarEventoPendiente
        Using connection = DirectCast(_connections.CreateOpenConnection(contexto), MySqlConnection)
            NonQuery(connection, Nothing, "UPDATE workflow_registro_ruta_sii_outbox SET status='RETRYABLE',attempts=attempts+1,last_error_code=@code,updated_at=@updated,next_attempt_at=DATE_ADD(@updated, INTERVAL 5 MINUTE) WHERE operation_id=@operationId",
                     New MySqlParameter("@code", Left(codigo, 64)), New MySqlParameter("@updated", DateTime.Now), New MySqlParameter("@operationId", operationId))
        End Using
    End Sub

    Private Shared Function LoadEvent(ByVal connection As MySqlConnection, ByVal routeId As Integer, ByVal receipt As String) As EventoRelacionRutaSii
        Using command = connection.CreateCommand()
            command.CommandText = "SELECT operation_id,task_id,route_id,receipt,enrollment,cabinet_name FROM workflow_registro_ruta_sii_outbox WHERE route_id=@routeId AND receipt=@receipt LIMIT 1"
            command.Parameters.AddWithValue("@routeId", routeId) : command.Parameters.AddWithValue("@receipt", receipt)
            Using reader = command.ExecuteReader()
                If Not reader.Read() Then Return Nothing
                Return New EventoRelacionRutaSii With {.OperationId = reader.GetString(0), .IdTarea = reader.GetInt64(1), .IdRuta = reader.GetInt32(2), .Recibo = reader.GetString(3), .Matricula = reader.GetString(4), .NombreGabinete = reader.GetString(5)}
            End Using
        End Using
    End Function

    Private Shared Function ReceiptExists(ByVal connection As MySqlConnection, ByVal receipt As String) As Boolean
        Dim value = Scalar(connection, Nothing,
                           "SELECT 1 FROM F_W_E_REGISTROPUBLICO WHERE DATOS_RECIBO=@receipt LIMIT 1",
                           New MySqlParameter("@receipt", receipt))
        Return value IsNot Nothing AndAlso value IsNot DBNull.Value
    End Function

    Private Shared Function Failure(ByVal code As String) As ResultadoRegistroTareaRutaSii
        Return New ResultadoRegistroTareaRutaSii With {.Codigo = code}
    End Function
    Private Shared Function Params(ParamArray values() As IDataParameter) As IEnumerable(Of IDataParameter)
        Return values
    End Function
    Private Shared Function NullIfEmpty(ByVal value As String) As Object
        Return If(String.IsNullOrWhiteSpace(value), CType(DBNull.Value, Object), value)
    End Function
    Private Shared Function Scalar(ByVal c As MySqlConnection, ByVal t As MySqlTransaction, ByVal sql As String, ParamArray p() As MySqlParameter) As Object
        Using command = c.CreateCommand()
            command.Transaction = t : command.CommandText = sql : command.Parameters.AddRange(p)
            Return command.ExecuteScalar()
        End Using
    End Function
    Private Shared Sub NonQuery(ByVal c As MySqlConnection, ByVal t As MySqlTransaction, ByVal sql As String, ParamArray p() As MySqlParameter)
        Using command = c.CreateCommand()
            command.Transaction = t : command.CommandText = sql : command.Parameters.AddRange(p)
            If command.ExecuteNonQuery() <> 1 Then Throw New InvalidOperationException("DOC87_WRITE_NOT_APPLIED")
        End Using
    End Sub
    Private Shared Function Insert(ByVal c As MySqlConnection, ByVal t As MySqlTransaction, ByVal sql As String, ParamArray p() As MySqlParameter) As Long
        Using command = c.CreateCommand()
            command.Transaction = t : command.CommandText = sql : command.Parameters.AddRange(p)
            If command.ExecuteNonQuery() <> 1 Then Throw New InvalidOperationException("DOC87_INSERT_NOT_APPLIED")
            Return command.LastInsertedId
        End Using
    End Function
End Class

Public NotInheritable Class MySqlRelacionRutaSiiGateway
    Implements IRelacionRutaSiiGateway
    Private ReadOnly _connections As IModuleConnectionFactory
    Public Sub New(ByVal connections As IModuleConnectionFactory)
        _connections = connections
    End Sub
    Public Function Materializar(ByVal contexto As ContextoModulo, ByVal evento As EventoRelacionRutaSii) As String Implements IRelacionRutaSiiGateway.Materializar
        Using connection = DirectCast(_connections.CreateOpenConnection(contexto), MySqlConnection)
            Dim lockName = "doc87-relation-" & evento.Recibo
            Dim acquired As Boolean = False
            Try
                Using command = connection.CreateCommand()
                    command.CommandText = "SELECT GET_LOCK(@lockName,5)"
                    command.Parameters.AddWithValue("@lockName", lockName)
                    acquired = Convert.ToString(command.ExecuteScalar(), CultureInfo.InvariantCulture) = "1"
                End Using
                If Not acquired Then Throw New InvalidOperationException("DOC87_RELATION_LOCK_UNAVAILABLE")
                Dim enrollment = If(evento.Matricula, String.Empty).Replace("S0", "")
                Dim expedientId As Long = 0
                Using command = connection.CreateCommand()
                    command.CommandText = "SELECT IdExpediente FROM ra_sii_cache_exepediente WHERE Matricula=@enrollment AND NombreGabinete=@cabinet LIMIT 2"
                    command.Parameters.AddWithValue("@enrollment", enrollment) : command.Parameters.AddWithValue("@cabinet", evento.NombreGabinete)
                    Using reader = command.ExecuteReader()
                        If Not reader.Read() Then Return "NO_APLICA"
                        expedientId = reader.GetInt64(0)
                        If reader.Read() Then Return "CACHE_AMBIGUOUS"
                    End Using
                End Using
                Using command = connection.CreateCommand()
                    command.CommandText = "SELECT expediente_archivo_ID_EXPEDIENTE FROM ra_relacion_radicado_externo_expediente WHERE RadicadoExterno=@receipt LIMIT 2"
                    command.Parameters.AddWithValue("@receipt", evento.Recibo)
                    Using reader = command.ExecuteReader()
                        If reader.Read() Then Return If(reader.GetInt64(0) = expedientId AndAlso Not reader.Read(), "CONFIRMED", "RELATION_CONFLICT")
                    End Using
                End Using
                Using transaction = connection.BeginTransaction()
                    Using command = connection.CreateCommand()
                        command.Transaction = transaction
                        command.CommandText = "INSERT INTO ra_relacion_radicado_externo_expediente (expediente_archivo_ID_EXPEDIENTE,RadicadoExterno,FechaRegistro) VALUES (@expedientId,@receipt,@created)"
                        command.Parameters.AddWithValue("@expedientId", expedientId) : command.Parameters.AddWithValue("@receipt", evento.Recibo) : command.Parameters.AddWithValue("@created", DateTime.Now)
                        If command.ExecuteNonQuery() <> 1 Then Throw New InvalidOperationException("DOC87_RELATION_NOT_APPLIED")
                    End Using
                    transaction.Commit()
                End Using
                Return "CONFIRMED"
            Finally
                If acquired Then
                    Try
                        Using command = connection.CreateCommand()
                            command.CommandText = "SELECT RELEASE_LOCK(@lockName)"
                            command.Parameters.AddWithValue("@lockName", lockName)
                            command.ExecuteScalar()
                        End Using
                    Catch
                    End Try
                End If
            End Try
        End Using
    End Function
End Class
