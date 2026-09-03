Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text

'Persistencia moderna de Notas. Las mutaciones exigen preflight y condiciones atómicas fail-closed.
Public Class MySqlNotasWorkflowRepository
    Inherits MySqlWorkflowPreviewRepositoryBase
    Implements INotasWorkflowRepository

    Private Const TamanoPaginaMaximo As Integer = 50
    Private Const CondicionTareaOperativa As String = "EXISTS (SELECT 1 FROM estados_tarea_workflow AS et WHERE et.Inicio_Tareas_Workflow_id_Tarea=@idTarea AND et.ID_USUARIO=@idUsuario AND et.ID_ACTIVIDAD=@idActividad AND et.FECHA_SELECCION IS NOT NULL AND et.FECHA_FIN IS NULL AND et.ESTADO_TAREA=0)"
    Private ReadOnly _transactionFactory As ITransactionFactory

    Public Sub New()
        Me.New(New ModuleConnectionFactory("MyDbContext"), New AdoNetDataExecutor())
    End Sub

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory,
                   ByVal dataExecutor As IDataExecutor)
        MyBase.New(connectionFactory, dataExecutor)
        _transactionFactory = New DbTransactionFactory()
    End Sub

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory,
                   ByVal dataExecutor As IDataExecutor,
                   ByVal transactionFactory As ITransactionFactory)
        MyBase.New(connectionFactory, dataExecutor)
        If transactionFactory Is Nothing Then Throw New ArgumentNullException(NameOf(transactionFactory))
        _transactionFactory = transactionFactory
    End Sub

    Public Function Listar(ByVal contexto As ContextoModuloWorkflow,
                           ByVal tarea As TareaWorkflow,
                           ByVal solicitud As SolicitudListarNotasWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Listar
        If Not EsSolicitudListarValida(tarea, solicitud) Then Return NoDisponible()

        Const sql As String = "SELECT at.ID_ANOTACION AS ID_NOTA, " &
                              "at.INICIO_TAREAS_WORKFLOW_ID_TAREA AS ID_TAREA, " &
                              "COALESCE(at.ID_USUARIO, 0) AS ID_AUTOR, " &
                              "COALESCE(at.ID_ACTIVIDAD, 0) AS ID_ACTIVIDAD_ORIGEN, " &
                              "at.DATO_ANOTACION AS CONTENIDO_VERSION, " &
                              "at.FECHA_ANOTACION AS FECHA_CREACION " &
                              "FROM ANOTACION_TAREA AS at " &
                              "WHERE at.INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea " &
                              "AND at.ESTADO_TAREA = 1 " &
                              "AND (@tieneCursor = 0 OR at.FECHA_ANOTACION < @fechaCursor " &
                              "OR (at.FECHA_ANOTACION = @fechaCursor AND at.ID_ANOTACION < @idNotaCursor)) " &
                              "ORDER BY at.FECHA_ANOTACION DESC, at.ID_ANOTACION DESC " &
                              "LIMIT @limite"

        Dim tamanoPagina As Integer = Math.Max(1, Math.Min(TamanoPaginaMaximo, solicitud.TamanoPagina))
        Dim filas As IList(Of NotaWorkflow) = EjecutarLectura(Of IList(Of NotaWorkflow))(contexto, sql,
            New List(Of IDataParameter) From {
                Parametro("@idTarea", tarea.IdTarea),
                Parametro("@tieneCursor", If(solicitud.FechaCursorUtc.HasValue, 1, 0)),
                Parametro("@fechaCursor", If(solicitud.FechaCursorUtc.HasValue, solicitud.FechaCursorUtc.Value, DateTime.MinValue)),
                Parametro("@idNotaCursor", solicitud.IdNotaCursor),
                Parametro("@limite", tamanoPagina + 1)},
            Function(reader As IDataReader) As IList(Of NotaWorkflow)
                Dim resultado As New List(Of NotaWorkflow)()
                While reader.Read()
                    Dim idNota As Long = EnteroLargo(reader, "ID_NOTA")
                    Dim idTarea As Long = EnteroLargo(reader, "ID_TAREA")
                    Dim idAutor As Integer = Entero(reader, "ID_AUTOR")
                    Dim idActividad As Integer = Entero(reader, "ID_ACTIVIDAD_ORIGEN")
                    resultado.Add(New NotaWorkflow With {
                        .IdNota = idNota,
                        .IdTarea = idTarea,
                        .IdAutorWorkflow = idAutor,
                        .IdActividadOrigen = idActividad,
                        .Contenido = Texto(reader, "CONTENIDO_VERSION"),
                        .Version = VersionNota(idNota, idTarea, idAutor, idActividad, 1, Texto(reader, "CONTENIDO_VERSION")),
                        .FechaCreacionUtc = FechaUtc(reader, "FECHA_CREACION"),
                        .PuedeGestionar = idAutor = contexto.IdUsuarioWorkflow AndAlso idActividad = tarea.IdActividadOrigen
                    })
                End While
                Return resultado
            End Function)

        Dim respuesta As New ResultadoNotasWorkflow With {.TieneMas = filas.Count > tamanoPagina}
        For indice As Integer = 0 To Math.Min(tamanoPagina, filas.Count) - 1
            respuesta.Notas.Add(filas(indice))
        Next
        Return respuesta
    End Function

    Public Function Contar(ByVal contexto As ContextoModuloWorkflow,
                           ByVal tarea As TareaWorkflow,
                           ByVal solicitud As SolicitudContarNotasWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Contar
        If tarea Is Nothing OrElse solicitud Is Nothing OrElse solicitud.IdTarea <> tarea.IdTarea Then Return NoDisponible()

        Const sql As String = "SELECT COUNT(*) AS TOTAL " &
                              "FROM ANOTACION_TAREA AS at " &
                              "WHERE at.INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea " &
                              "AND at.ESTADO_TAREA = 1"
        Dim contador As Integer = EjecutarLectura(Of Integer)(contexto, sql,
            New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea)},
            Function(reader As IDataReader) As Integer
                If Not reader.Read() OrElse reader.IsDBNull(reader.GetOrdinal("TOTAL")) Then Return 0
                Return Convert.ToInt32(reader.GetValue(reader.GetOrdinal("TOTAL")), CultureInfo.InvariantCulture)
            End Function)
        Return New ResultadoNotasWorkflow With {.Contador = Math.Max(0, contador)}
    End Function

    Public Function Crear(ByVal contexto As ContextoModuloWorkflow,
                          ByVal tarea As TareaWorkflow,
                          ByVal solicitud As SolicitudCrearNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Crear
        If Not PreflightEscriturasDisponible(contexto) Then Return NoDisponible()
        If tarea Is Nothing OrElse solicitud Is Nothing OrElse solicitud.IdTarea <> tarea.IdTarea Then Return NoDisponible()
        Try
            Return EjecutarEnTransaccion(contexto, Function(connection As IDbConnection, transaction As IDbTransaction) As ResultadoNotasWorkflow
                Dim existente As NotaWorkflow = RespuestaIdempotente(EjecutorDatos.ExecuteScalar(connection, transaction, "SELECT CONCAT(COALESCE(Id_Anotacion, 0), '|', COALESCE(Version_Resultado, '')) FROM workflow_notas_idempotencia WHERE Inicio_Tareas_Workflow_id_Tarea=@idTarea AND Id_Usuario_Workflow=@idUsuario AND Client_Request_Id=@requestId AND Codigo_Resultado='OK' AND Fecha_Expiracion >= UTC_TIMESTAMP() LIMIT 1", New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@requestId", solicitud.IdSolicitudCliente)}), tarea, contexto)
                If existente IsNot Nothing Then Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.Exito, .Nota = existente}
                Const reservar As String = "INSERT INTO workflow_notas_idempotencia (Inicio_Tareas_Workflow_id_Tarea, Id_Usuario_Workflow, Client_Request_Id, Codigo_Resultado, Fecha_Creacion, Fecha_Expiracion) VALUES (@idTarea, @idUsuario, @requestId, 'PENDING', UTC_TIMESTAMP(), DATE_ADD(UTC_TIMESTAMP(), INTERVAL 30 DAY))"
                Try
                    If EjecutorDatos.ExecuteNonQuery(connection, transaction, reservar, New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@requestId", solicitud.IdSolicitudCliente)}) <> 1 Then Return NoDisponible()
                Catch
                    Dim concurrente As NotaWorkflow = RespuestaIdempotente(EjecutorDatos.ExecuteScalar(connection, transaction, "SELECT CONCAT(COALESCE(Id_Anotacion, 0), '|', COALESCE(Version_Resultado, '')) FROM workflow_notas_idempotencia WHERE Inicio_Tareas_Workflow_id_Tarea=@idTarea AND Id_Usuario_Workflow=@idUsuario AND Client_Request_Id=@requestId AND Codigo_Resultado='OK' AND Fecha_Expiracion >= UTC_TIMESTAMP() LIMIT 1", New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@requestId", solicitud.IdSolicitudCliente)}), tarea, contexto)
                    If concurrente IsNot Nothing Then Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.Exito, .Nota = concurrente}
                    Return NoDisponible()
                End Try
                Const insertar As String = "INSERT INTO ANOTACION_TAREA (INICIO_TAREAS_WORKFLOW_ID_TAREA, DATO_ANOTACION, ID_ACTIVIDAD, ID_USUARIO, FECHA_ANOTACION, ESTADO_TAREA) SELECT @idTarea, @contenido, @idActividad, @idUsuario, UTC_TIMESTAMP(), 1 FROM DUAL WHERE " & CondicionTareaOperativa
                If EjecutorDatos.ExecuteNonQuery(connection, transaction, insertar, New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea), Parametro("@contenido", solicitud.Contenido), Parametro("@idActividad", tarea.IdActividadOrigen), Parametro("@idUsuario", contexto.IdUsuarioWorkflow)}) <> 1 Then
                    Throw New ResultadoFuncionalNotasException(TareaNoActiva())
                End If
                Dim idNota As Long = Convert.ToInt64(EjecutorDatos.ExecuteScalar(connection, transaction, "SELECT LAST_INSERT_ID()", Nothing), CultureInfo.InvariantCulture)
                Dim version As String = VersionNota(idNota, tarea.IdTarea, contexto.IdUsuarioWorkflow, tarea.IdActividadOrigen, 1, solicitud.Contenido)
                Const registrarVersion As String = "INSERT INTO workflow_notas_version (Id_Anotacion, Inicio_Tareas_Workflow_id_Tarea, Id_Usuario_Workflow, Version_Nota, Fecha_Actualizacion) VALUES (@idNota, @idTarea, @idUsuario, @version, UTC_TIMESTAMP())"
                If EjecutorDatos.ExecuteNonQuery(connection, transaction, registrarVersion, New List(Of IDataParameter) From {Parametro("@idNota", idNota), Parametro("@idTarea", tarea.IdTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@version", version)}) <> 1 Then Throw New InvalidOperationException("NOTES_VERSION_LEDGER_WRITE_FAILED")
                Const auditar As String = "INSERT INTO wf_log_workflow (usuario_workflow_idU_suario, fecha_hora, operacion, ID_TAREA_WORKFLOW, datos_operacion, opcion, descripcion_opcion, ip_transacion, id_operacion) VALUES (@idUsuario, UTC_TIMESTAMP(), 'Agrega', @idTarea, @datos, 1, 'NOTA WORKFLOW', '', @idNota)"
                EjecutorDatos.ExecuteNonQuery(connection, transaction, auditar, New List(Of IDataParameter) From {Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@idTarea", tarea.IdTarea), Parametro("@datos", "correlacion=" & solicitud.IdSolicitudCliente & ";actividad=" & tarea.IdActividadOrigen.ToString(CultureInfo.InvariantCulture) & ";resultado=OK;version_resultante=" & version & ";longitud_nueva=" & solicitud.Contenido.Length.ToString(CultureInfo.InvariantCulture) & ";sha256_nuevo=" & HashSha256(solicitud.Contenido)), Parametro("@idNota", idNota)})
                EjecutorDatos.ExecuteNonQuery(connection, transaction, "UPDATE workflow_notas_idempotencia SET Id_Anotacion=@idNota, Version_Resultado=@version, Codigo_Resultado='OK' WHERE Inicio_Tareas_Workflow_id_Tarea=@idTarea AND Id_Usuario_Workflow=@idUsuario AND Client_Request_Id=@requestId", New List(Of IDataParameter) From {Parametro("@idNota", idNota), Parametro("@version", version), Parametro("@idTarea", tarea.IdTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@requestId", solicitud.IdSolicitudCliente)})
                Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.Exito, .Nota = New NotaWorkflow With {.IdNota = idNota, .IdTarea = tarea.IdTarea, .IdAutorWorkflow = contexto.IdUsuarioWorkflow, .IdActividadOrigen = tarea.IdActividadOrigen, .Contenido = solicitud.Contenido, .Version = version, .PuedeGestionar = True}}
            End Function)
        Catch ex As ResultadoFuncionalNotasException
            Return ex.Resultado
        Catch
            Return NoDisponible()
        End Try
    End Function

    Public Function Consultar(ByVal contexto As ContextoModuloWorkflow,
                              ByVal tarea As TareaWorkflow,
                              ByVal solicitud As SolicitudConsultarNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Consultar
        If tarea Is Nothing OrElse solicitud Is Nothing OrElse solicitud.IdTarea <> tarea.IdTarea OrElse solicitud.IdNota <= 0 Then
            Return NoDisponible()
        End If

        Const sql As String = "SELECT at.ID_ANOTACION AS ID_NOTA, " &
                              "at.INICIO_TAREAS_WORKFLOW_ID_TAREA AS ID_TAREA, " &
                              "COALESCE(at.ID_USUARIO, 0) AS ID_AUTOR, " &
                              "COALESCE(at.ID_ACTIVIDAD, 0) AS ID_ACTIVIDAD_ORIGEN, " &
                              "at.DATO_ANOTACION AS CONTENIDO, " &
                              "at.FECHA_ANOTACION AS FECHA_CREACION " &
                              "FROM ANOTACION_TAREA AS at " &
                              "WHERE at.ID_ANOTACION = @idNota " &
                              "AND at.INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea " &
                              "AND at.ESTADO_TAREA = 1 " &
                              "LIMIT 1"
        Dim nota As NotaWorkflow = EjecutarLectura(Of NotaWorkflow)(contexto, sql,
            New List(Of IDataParameter) From {Parametro("@idNota", solicitud.IdNota), Parametro("@idTarea", tarea.IdTarea)},
            Function(reader As IDataReader) As NotaWorkflow
                If Not reader.Read() Then Return Nothing
                Dim idNota As Long = EnteroLargo(reader, "ID_NOTA")
                Dim idTarea As Long = EnteroLargo(reader, "ID_TAREA")
                Dim idAutor As Integer = Entero(reader, "ID_AUTOR")
                Dim idActividad As Integer = Entero(reader, "ID_ACTIVIDAD_ORIGEN")
                Dim contenido As String = Texto(reader, "CONTENIDO")
                Return New NotaWorkflow With {
                    .IdNota = idNota,
                    .IdTarea = idTarea,
                    .IdAutorWorkflow = idAutor,
                    .IdActividadOrigen = idActividad,
                    .Version = VersionNota(idNota, idTarea, idAutor, idActividad, 1, contenido),
                    .Contenido = contenido,
                    .FechaCreacionUtc = FechaUtc(reader, "FECHA_CREACION"),
                    .PuedeGestionar = idAutor = contexto.IdUsuarioWorkflow AndAlso idActividad = tarea.IdActividadOrigen
                }
            End Function)
        If nota Is Nothing Then
            Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.NoteNotFound,
                                                    .MensajeFuncional = "La nota solicitada no está disponible."}
        End If
        Return New ResultadoNotasWorkflow With {.Nota = nota}
    End Function

    Public Function Actualizar(ByVal contexto As ContextoModuloWorkflow,
                               ByVal tarea As TareaWorkflow,
                               ByVal solicitud As SolicitudActualizarNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Actualizar
        If Not PreflightEscriturasDisponible(contexto) Then Return NoDisponible()
        If tarea Is Nothing OrElse solicitud Is Nothing OrElse solicitud.IdTarea <> tarea.IdTarea Then Return NoDisponible()
        Try
            Return EjecutarEnTransaccion(contexto, Function(connection As IDbConnection, transaction As IDbTransaction) As ResultadoNotasWorkflow
                Dim version As String = VersionNota(solicitud.IdNota, tarea.IdTarea, contexto.IdUsuarioWorkflow, tarea.IdActividadOrigen, 1, solicitud.Contenido)
                Const sql As String = "UPDATE ANOTACION_TAREA AS at INNER JOIN workflow_notas_version AS vn ON vn.Id_Anotacion=at.ID_ANOTACION AND vn.Inicio_Tareas_Workflow_id_Tarea=at.INICIO_TAREAS_WORKFLOW_ID_TAREA AND vn.Id_Usuario_Workflow=at.ID_USUARIO SET at.DATO_ANOTACION=@contenido, vn.Version_Nota=@versionResultante, vn.Fecha_Actualizacion=UTC_TIMESTAMP() WHERE at.ID_ANOTACION=@idNota AND at.INICIO_TAREAS_WORKFLOW_ID_TAREA=@idTarea AND at.ID_USUARIO=@idUsuario AND at.ID_ACTIVIDAD=@idActividad AND at.ESTADO_TAREA=1 AND vn.Version_Nota=@version AND " & CondicionTareaOperativa
                Dim afectados As Integer = EjecutorDatos.ExecuteNonQuery(connection, transaction, sql, New List(Of IDataParameter) From {Parametro("@contenido", solicitud.Contenido), Parametro("@versionResultante", version), Parametro("@idNota", solicitud.IdNota), Parametro("@idTarea", tarea.IdTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@idActividad", tarea.IdActividadOrigen), Parametro("@version", solicitud.Version)})
                If afectados <= 0 Then Return DiagnosticarMutacionNoAplicada(connection, transaction, contexto, tarea, solicitud.IdNota)
                EjecutorDatos.ExecuteNonQuery(connection, transaction, "INSERT INTO wf_log_workflow (usuario_workflow_idU_suario, fecha_hora, operacion, ID_TAREA_WORKFLOW, datos_operacion, opcion, descripcion_opcion, ip_transacion, id_operacion) VALUES (@idUsuario, UTC_TIMESTAMP(), 'Actualiza', @idTarea, @datos, 2, 'NOTA WORKFLOW', '', @idNota)", New List(Of IDataParameter) From {Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@idTarea", tarea.IdTarea), Parametro("@datos", "actividad=" & tarea.IdActividadOrigen.ToString(CultureInfo.InvariantCulture) & ";resultado=OK;version_anterior=" & solicitud.Version & ";version_resultante=" & version & ";longitud_nueva=" & solicitud.Contenido.Length.ToString(CultureInfo.InvariantCulture) & ";sha256_nuevo=" & HashSha256(solicitud.Contenido)), Parametro("@idNota", solicitud.IdNota)})
                Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.Exito, .Nota = New NotaWorkflow With {.IdNota = solicitud.IdNota, .IdTarea = tarea.IdTarea, .IdAutorWorkflow = contexto.IdUsuarioWorkflow, .IdActividadOrigen = tarea.IdActividadOrigen, .Contenido = solicitud.Contenido, .Version = version, .PuedeGestionar = True}}
            End Function)
        Catch
            Return NoDisponible()
        End Try
    End Function

    Public Function Eliminar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaWorkflow,
                             ByVal solicitud As SolicitudEliminarNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Eliminar
        If Not PreflightEscriturasDisponible(contexto) Then Return NoDisponible()
        If tarea Is Nothing OrElse solicitud Is Nothing OrElse solicitud.IdTarea <> tarea.IdTarea Then Return NoDisponible()
        Try
            Return EjecutarEnTransaccion(contexto, Function(connection As IDbConnection, transaction As IDbTransaction) As ResultadoNotasWorkflow
                Const sql As String = "DELETE at, vn FROM ANOTACION_TAREA AS at INNER JOIN workflow_notas_version AS vn ON vn.Id_Anotacion=at.ID_ANOTACION AND vn.Inicio_Tareas_Workflow_id_Tarea=at.INICIO_TAREAS_WORKFLOW_ID_TAREA AND vn.Id_Usuario_Workflow=at.ID_USUARIO WHERE at.ID_ANOTACION=@idNota AND at.INICIO_TAREAS_WORKFLOW_ID_TAREA=@idTarea AND at.ID_USUARIO=@idUsuario AND at.ID_ACTIVIDAD=@idActividad AND at.ESTADO_TAREA=1 AND vn.Version_Nota=@version AND " & CondicionTareaOperativa
                Dim afectados As Integer = EjecutorDatos.ExecuteNonQuery(connection, transaction, sql, New List(Of IDataParameter) From {Parametro("@idNota", solicitud.IdNota), Parametro("@idTarea", tarea.IdTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@idActividad", tarea.IdActividadOrigen), Parametro("@version", solicitud.Version)})
                If afectados <= 0 Then Return DiagnosticarMutacionNoAplicada(connection, transaction, contexto, tarea, solicitud.IdNota)
                EjecutorDatos.ExecuteNonQuery(connection, transaction, "INSERT INTO wf_log_workflow (usuario_workflow_idU_suario, fecha_hora, operacion, ID_TAREA_WORKFLOW, datos_operacion, opcion, descripcion_opcion, ip_transacion, id_operacion) VALUES (@idUsuario, UTC_TIMESTAMP(), 'Elimina', @idTarea, @datos, 3, 'NOTA WORKFLOW', '', @idNota)", New List(Of IDataParameter) From {Parametro("@idUsuario", contexto.IdUsuarioWorkflow), Parametro("@idTarea", tarea.IdTarea), Parametro("@datos", "actividad=" & tarea.IdActividadOrigen.ToString(CultureInfo.InvariantCulture) & ";resultado=OK;eliminada=1;version_anterior=" & solicitud.Version), Parametro("@idNota", solicitud.IdNota)})
                Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.Exito}
            End Function)
        Catch
            Return NoDisponible()
        End Try
    End Function

    Private Function PreflightEscriturasDisponible(ByVal contexto As ContextoModuloWorkflow) As Boolean
        Const sql As String = "SELECT " &
                              "(SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'ANOTACION_TAREA' AND ENGINE = 'InnoDB') AS NOTAS_INNODB, " &
                              "(SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WF_LOG_WORKFLOW' AND ENGINE = 'InnoDB') AS AUDIT_INNODB, " &
                              "(SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WORKFLOW_NOTAS_IDEMPOTENCIA' AND ENGINE = 'InnoDB') AS IDEMPOTENCIA_INNODB, " &
                              "(SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WORKFLOW_NOTAS_VERSION' AND ENGINE = 'InnoDB') AS VERSIONES_INNODB, " &
                              "(SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'ANOTACION_TAREA' AND UPPER(COLUMN_NAME) IN ('ID_ANOTACION','INICIO_TAREAS_WORKFLOW_ID_TAREA','DATO_ANOTACION','ID_ACTIVIDAD','ID_USUARIO','FECHA_ANOTACION','ESTADO_TAREA')) AS NOTAS_COLUMNAS, " &
                              "(SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'ANOTACION_TAREA' AND UPPER(COLUMN_NAME) = 'DATO_ANOTACION' AND LOWER(DATA_TYPE) = 'text' AND LOWER(CHARACTER_SET_NAME) = 'utf8') AS NOTAS_TEXTO_UTF8, " &
                              "(SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WF_LOG_WORKFLOW' AND UPPER(COLUMN_NAME) IN ('USUARIO_WORKFLOW_IDU_SUARIO','FECHA_HORA','OPERACION','ID_TAREA_WORKFLOW','DATOS_OPERACION','OPCION','DESCRIPCION_OPCION','IP_TRANSACION','ID_OPERACION')) AS AUDIT_COLUMNAS, " &
                              "(SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WORKFLOW_NOTAS_IDEMPOTENCIA' AND UPPER(COLUMN_NAME) IN ('INICIO_TAREAS_WORKFLOW_ID_TAREA','ID_USUARIO_WORKFLOW','CLIENT_REQUEST_ID','ID_ANOTACION','VERSION_RESULTADO','CODIGO_RESULTADO','FECHA_EXPIRACION')) AS IDEMPOTENCIA_COLUMNAS, " &
                              "(SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WORKFLOW_NOTAS_VERSION' AND UPPER(COLUMN_NAME) IN ('ID_ANOTACION','INICIO_TAREAS_WORKFLOW_ID_TAREA','ID_USUARIO_WORKFLOW','VERSION_NOTA','FECHA_ACTUALIZACION')) AS VERSIONES_COLUMNAS, " &
                              "(SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WORKFLOW_NOTAS_VERSION' AND UPPER(COLUMN_NAME) = 'VERSION_NOTA' AND LOWER(DATA_TYPE) = 'char' AND LOWER(CHARACTER_SET_NAME) = 'latin1') AS VERSIONES_TIPO, " &
                              "(SELECT COUNT(DISTINCT INDEX_NAME) FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'ANOTACION_TAREA' AND UPPER(INDEX_NAME) IN ('IX_ANOTACION_OPERATIVA_ORDEN','IX_ANOTACION_HISTORICO_ORDEN')) AS NOTAS_INDICES, " &
                              "(SELECT COUNT(DISTINCT INDEX_NAME) FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WF_LOG_WORKFLOW' AND UPPER(INDEX_NAME) = 'IX_WF_LOG_TAREA_FECHA') AS AUDIT_INDICES, " &
                              "(SELECT COUNT(*) FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WORKFLOW_NOTAS_IDEMPOTENCIA' AND UPPER(INDEX_NAME) = 'UX_NOTAS_IDEMPOTENCIA_INTENCION' AND NON_UNIQUE = 0 AND ((SEQ_IN_INDEX = 1 AND UPPER(COLUMN_NAME) = 'INICIO_TAREAS_WORKFLOW_ID_TAREA') OR (SEQ_IN_INDEX = 2 AND UPPER(COLUMN_NAME) = 'ID_USUARIO_WORKFLOW') OR (SEQ_IN_INDEX = 3 AND UPPER(COLUMN_NAME) = 'CLIENT_REQUEST_ID'))) AS IDEMPOTENCIA_UNICIDAD, " &
                              "(SELECT COUNT(DISTINCT INDEX_NAME) FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WORKFLOW_NOTAS_IDEMPOTENCIA' AND UPPER(INDEX_NAME) = 'IX_NOTAS_IDEMPOTENCIA_EXPIRACION') AS IDEMPOTENCIA_INDICES, " &
                              "(SELECT COUNT(DISTINCT INDEX_NAME) FROM information_schema.STATISTICS WHERE TABLE_SCHEMA = DATABASE() AND UPPER(TABLE_NAME) = 'WORKFLOW_NOTAS_VERSION' AND UPPER(INDEX_NAME) = 'IX_NOTAS_VERSION_TAREA_USUARIO') AS VERSIONES_INDICES"
        Try
            Return EjecutarLectura(Of Boolean)(contexto, sql, Nothing,
                Function(reader As IDataReader) As Boolean
                    If Not reader.Read() Then Return False
                    Return Entero(reader, "NOTAS_INNODB") = 1 AndAlso
                           Entero(reader, "AUDIT_INNODB") = 1 AndAlso
                           Entero(reader, "IDEMPOTENCIA_INNODB") = 1 AndAlso
                           Entero(reader, "VERSIONES_INNODB") = 1 AndAlso
                           Entero(reader, "NOTAS_COLUMNAS") = 7 AndAlso
                           Entero(reader, "NOTAS_TEXTO_UTF8") = 1 AndAlso
                           Entero(reader, "AUDIT_COLUMNAS") = 9 AndAlso
                           Entero(reader, "IDEMPOTENCIA_COLUMNAS") = 7 AndAlso
                           Entero(reader, "VERSIONES_COLUMNAS") = 5 AndAlso
                           Entero(reader, "VERSIONES_TIPO") = 1 AndAlso
                           Entero(reader, "NOTAS_INDICES") = 2 AndAlso
                           Entero(reader, "AUDIT_INDICES") = 1 AndAlso
                           Entero(reader, "IDEMPOTENCIA_UNICIDAD") = 3 AndAlso
                           Entero(reader, "IDEMPOTENCIA_INDICES") = 1 AndAlso
                           Entero(reader, "VERSIONES_INDICES") = 1
                End Function)
        Catch
            Return False
        End Try
    End Function

    Private Function EjecutarEnTransaccion(Of T)(ByVal contexto As ContextoModuloWorkflow,
                                                  ByVal operacion As Func(Of IDbConnection, IDbTransaction, T)) As T
        If _transactionFactory Is Nothing Then Throw New InvalidOperationException("TRANSACTION_FACTORY_UNAVAILABLE")
        Using connection As IDbConnection = FabricaConexion.CreateOpenConnection(contexto)
            Using transaction As IDbTransaction = _transactionFactory.BeginTransaction(connection)
                Try
                    Dim resultado As T = operacion(connection, transaction)
                    transaction.Commit()
                    Return resultado
                Catch
                    Try
                        transaction.Rollback()
                    Catch
                    End Try
                    Throw
                End Try
            End Using
        End Using
    End Function

    Private Shared Function EsSolicitudListarValida(ByVal tarea As TareaWorkflow,
                                                     ByVal solicitud As SolicitudListarNotasWorkflow) As Boolean
        Return tarea IsNot Nothing AndAlso solicitud IsNot Nothing AndAlso solicitud.IdTarea = tarea.IdTarea AndAlso
               solicitud.TamanoPagina >= 1 AndAlso solicitud.TamanoPagina <= TamanoPaginaMaximo AndAlso
               (Not solicitud.FechaCursorUtc.HasValue OrElse solicitud.IdNotaCursor > 0)
    End Function

    Private Shared Function FechaUtc(ByVal reader As IDataReader, ByVal fieldName As String) As DateTime
        Dim ordinal As Integer = reader.GetOrdinal(fieldName)
        If reader.IsDBNull(ordinal) Then Return DateTime.MinValue
        Return DateTime.SpecifyKind(Convert.ToDateTime(reader.GetValue(ordinal), CultureInfo.InvariantCulture), DateTimeKind.Utc)
    End Function

    Private Shared Function NoDisponible() As ResultadoNotasWorkflow
        Return New ResultadoNotasWorkflow With {
            .Codigo = CodigosResultadoNotasWorkflow.Unavailable,
            .MensajeFuncional = "La persistencia moderna de notas no está disponible."
        }
    End Function

    Private Shared Function TareaNoActiva() As ResultadoNotasWorkflow
        Return New ResultadoNotasWorkflow With {
            .Codigo = CodigosResultadoNotasWorkflow.TaskNotActive,
            .MensajeFuncional = "La tarea no está disponible para notas."
        }
    End Function

    Private Function DiagnosticarMutacionNoAplicada(ByVal connection As IDbConnection,
                                                      ByVal transaction As IDbTransaction,
                                                      ByVal contexto As ContextoModuloWorkflow,
                                                      ByVal tarea As TareaWorkflow,
                                                      ByVal idNota As Long) As ResultadoNotasWorkflow
        Const sql As String = "SELECT COALESCE(ID_USUARIO, 0) FROM ANOTACION_TAREA WHERE ID_ANOTACION=@idNota AND INICIO_TAREAS_WORKFLOW_ID_TAREA=@idTarea AND ESTADO_TAREA=1 LIMIT 1"
        Dim autor As Object = EjecutorDatos.ExecuteScalar(connection, transaction, sql,
            New List(Of IDataParameter) From {Parametro("@idNota", idNota), Parametro("@idTarea", tarea.IdTarea)})
        If autor Is Nothing OrElse autor Is DBNull.Value Then
            Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.NoteNotFound, .MensajeFuncional = "La nota solicitada no está disponible."}
        End If
        If Convert.ToInt32(autor, CultureInfo.InvariantCulture) <> contexto.IdUsuarioWorkflow Then
            Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.NotOwner, .MensajeFuncional = "La nota solo puede ser gestionada por su autor."}
        End If
        Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.VersionConflict, .MensajeFuncional = "La versión de la nota ya no está vigente."}
    End Function

    Private Shared Function RespuestaIdempotente(ByVal value As Object,
                                                  ByVal tarea As TareaWorkflow,
                                                  ByVal contexto As ContextoModuloWorkflow) As NotaWorkflow
        If value Is Nothing OrElse value Is DBNull.Value OrElse tarea Is Nothing OrElse contexto Is Nothing Then Return Nothing
        Dim serializado As String = Convert.ToString(value, CultureInfo.InvariantCulture)
        Dim separador As Integer = serializado.IndexOf("|", StringComparison.Ordinal)
        If separador <= 0 OrElse separador = serializado.Length - 1 Then Return Nothing

        Dim idNota As Long = 0L
        If Not Long.TryParse(serializado.Substring(0, separador), NumberStyles.None, CultureInfo.InvariantCulture, idNota) OrElse idNota <= 0 Then Return Nothing
        Dim version As String = serializado.Substring(separador + 1)
        If version.Length <> 64 Then Return Nothing

        Return New NotaWorkflow With {
            .IdNota = idNota,
            .IdTarea = tarea.IdTarea,
            .IdAutorWorkflow = contexto.IdUsuarioWorkflow,
            .IdActividadOrigen = tarea.IdActividadOrigen,
            .Version = version,
            .PuedeGestionar = True
        }
    End Function

    Private Shared Function VersionNota(ByVal idNota As Long,
                                         ByVal idTarea As Long,
                                         ByVal idAutor As Integer,
                                         ByVal idActividad As Integer,
                                         ByVal estado As Integer,
                                         ByVal contenido As String) As String
        Dim representacionCanonica As String = String.Join("|", New String() {
            idNota.ToString(CultureInfo.InvariantCulture),
            idTarea.ToString(CultureInfo.InvariantCulture),
            idAutor.ToString(CultureInfo.InvariantCulture),
            idActividad.ToString(CultureInfo.InvariantCulture),
            estado.ToString(CultureInfo.InvariantCulture),
            HashSha256(contenido)})
        Return HashSha256(representacionCanonica)
    End Function

    Private Shared Function HashSha256(ByVal value As String) As String
        Using sha As SHA256 = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", String.Empty).ToLowerInvariant()
        End Using
    End Function

    Private NotInheritable Class ResultadoFuncionalNotasException
        Inherits Exception

        Public Sub New(ByVal resultado As ResultadoNotasWorkflow)
            MyBase.New("NOTES_FUNCTIONAL_RESULT")
            Me.Resultado = resultado
        End Sub

        Public ReadOnly Property Resultado As ResultadoNotasWorkflow
    End Class
End Class
