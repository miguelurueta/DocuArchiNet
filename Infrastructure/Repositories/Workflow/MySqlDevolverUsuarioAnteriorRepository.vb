Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports MySql.Data.MySqlClient

'Repositorio exclusivo de Usuario anterior. Todas sus consultas son SELECT parametrizados y se limitan a una tarea accesible.
Public Class MySqlDevolverUsuarioAnteriorRepository
    Implements IDevolverUsuarioAnteriorTareaRepository, IDevolverUsuarioAnteriorAutorizacionRepository, IDevolverUsuarioAnteriorHistorialRepository

    Private ReadOnly _connectionFactory As IModuleConnectionFactory
    Private ReadOnly _dataExecutor As IDataExecutor

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory, ByVal dataExecutor As IDataExecutor)
        If connectionFactory Is Nothing Then Throw New ArgumentNullException(NameOf(connectionFactory))
        If dataExecutor Is Nothing Then Throw New ArgumentNullException(NameOf(dataExecutor))
        _connectionFactory = connectionFactory
        _dataExecutor = dataExecutor
    End Sub

    Public Function ObtenerTarea(ByVal contexto As ContextoModuloWorkflow,
                                 ByVal idTarea As Long) As TareaDevolverUsuarioAnterior Implements IDevolverUsuarioAnteriorTareaRepository.ObtenerTarea
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse idTarea <= 0 Then Return Nothing
        Const sql As String = "SELECT estado.id_Estado, estado.Inicio_Tareas_Workflow_id_Tarea, " &
                              "estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta, estado.Id_Actividad, " &
                              "estado.ID_FLUJO_TRABAJO, estado.ID_ACTIVIDAD_FLUJO_TRABAJO " &
                              "FROM estados_tarea_workflow AS estado " &
                              "WHERE estado.Inicio_Tareas_Workflow_id_Tarea = @idTarea " &
                              "AND estado.ID_USUARIO = @idUsuario " &
                              "AND estado.FECHA_SELECCION IS NOT NULL AND estado.FECHA_FIN IS NULL " &
                              "AND estado.ESTADO_TAREA = 0 ORDER BY estado.id_Estado DESC LIMIT 1"
        Dim tarea As TareaDevolverUsuarioAnterior = Leer(Of TareaDevolverUsuarioAnterior)(contexto, sql,
            New List(Of IDataParameter) From {Parametro("@idTarea", idTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow)},
            Function(reader As IDataReader) As TareaDevolverUsuarioAnterior
                If Not reader.Read() Then Return Nothing
                Return New TareaDevolverUsuarioAnterior With {
                    .IdEstadoActual = EnteroLargo(reader, "id_Estado"),
                    .IdTarea = EnteroLargo(reader, "Inicio_Tareas_Workflow_id_Tarea"),
                    .IdRuta = Entero(reader, "Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta"),
                    .IdActividadActual = Entero(reader, "Id_Actividad"),
                    .IdFlujoTrabajo = Entero(reader, "ID_FLUJO_TRABAJO"),
                    .IdActividadFlujoActual = Entero(reader, "ID_ACTIVIDAD_FLUJO_TRABAJO"),
                    .EstaActiva = True}
            End Function)
        If tarea Is Nothing OrElse tarea.IdEstadoActual <= 0 OrElse tarea.IdTarea <> idTarea OrElse tarea.IdRuta <> contexto.IdRutaWorkflow OrElse
           tarea.IdActividadActual <= 0 OrElse (tarea.IdFlujoTrabajo > 0 AndAlso tarea.IdActividadFlujoActual <= 0) OrElse
           (tarea.IdFlujoTrabajo = 0 AndAlso tarea.IdActividadFlujoActual <> 0) Then Return Nothing
        Return tarea
    End Function

    Public Function Evaluar(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaDevolverUsuarioAnterior) As ResultadoAutorizacionDevolverUsuarioAnterior Implements IDevolverUsuarioAnteriorAutorizacionRepository.Evaluar
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
           tarea.IdTarea <= 0 OrElse tarea.IdRuta <> contexto.IdRutaWorkflow Then
            Return BloquearAutorizacion(CodigosBloqueoDevolverUsuarioAnterior.TareaNoDisponible, "La tarea no está disponible para devolución.")
        End If
        If Not contexto.PuedeDevolverUsuarioAnterior Then
            Return BloquearAutorizacion(CodigosBloqueoDevolverUsuarioAnterior.PermisoDenegado, "El usuario no tiene permiso para devolver la tarea.")
        End If
        Return New ResultadoAutorizacionDevolverUsuarioAnterior With {.Autorizado = True}
    End Function

    Public Function ObtenerAntecedente(ByVal contexto As ContextoModuloWorkflow,
                                       ByVal tarea As TareaDevolverUsuarioAnterior) As ResultadoHistorialDevolverUsuarioAnterior Implements IDevolverUsuarioAnteriorHistorialRepository.ObtenerAntecedente
        Dim autorizacion As ResultadoAutorizacionDevolverUsuarioAnterior = Evaluar(contexto, tarea)
        If autorizacion Is Nothing OrElse Not autorizacion.Autorizado Then
            Return BloquearHistorial(If(autorizacion Is Nothing, CodigosBloqueoDevolverUsuarioAnterior.TareaNoDisponible, autorizacion.CodigoBloqueo),
                                     If(autorizacion Is Nothing, "La tarea no está disponible para devolución.", autorizacion.MensajeFuncional))
        End If
        Const sql As String = "SELECT estado.id_Estado, estado.Inicio_Tareas_Workflow_id_Tarea, " &
                              "estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta, estado.Id_Actividad, estado.Id_Usuario, " &
                              "estado.ID_FLUJO_TRABAJO, estado.ID_ACTIVIDAD_FLUJO_TRABAJO, estado.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO " &
                              "FROM estados_tarea_workflow AS estado WHERE estado.Inicio_Tareas_Workflow_id_Tarea = @idTarea " &
                              "AND estado.id_Estado < @idEstadoActual AND estado.Id_Usuario > 0 " &
                              "AND estado.Id_Usuario <> @idUsuarioActual " &
                              "ORDER BY estado.id_Estado DESC LIMIT 1"
        Dim estados As IList(Of EstadoHistorico) = Leer(Of IList(Of EstadoHistorico))(contexto, sql,
            New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea), Parametro("@idEstadoActual", tarea.IdEstadoActual), Parametro("@idUsuarioActual", contexto.IdUsuarioWorkflow)},
            Function(reader As IDataReader) As IList(Of EstadoHistorico)
                Dim resultado As New List(Of EstadoHistorico)()
                While reader.Read()
                    resultado.Add(New EstadoHistorico With {
                        .IdEstado = EnteroLargo(reader, "id_Estado"),
                        .IdTarea = EnteroLargo(reader, "Inicio_Tareas_Workflow_id_Tarea"),
                        .IdRuta = Entero(reader, "Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta"),
                        .IdActividad = Entero(reader, "Id_Actividad"),
                        .IdUsuario = Entero(reader, "Id_Usuario"),
                        .IdFlujo = Entero(reader, "ID_FLUJO_TRABAJO"),
                        .IdActividadFlujo = Entero(reader, "ID_ACTIVIDAD_FLUJO_TRABAJO"),
                        .IdUsuarioFlujo = Entero(reader, "ID_USUARIO_WORKFLOW_FLUJO_TRABAJO")})
                End While
                Return resultado
            End Function)
        If estados Is Nothing OrElse estados.Count = 0 Then
            Return BloquearHistorial(CodigosBloqueoDevolverUsuarioAnterior.HistorialNoDisponible, "No existe un usuario anterior disponible para la tarea.")
        End If
        Dim anterior As EstadoHistorico = estados(0)
        If anterior.IdTarea <> tarea.IdTarea OrElse anterior.IdEstado <= 0 Then
            Return BloquearHistorial(CodigosBloqueoDevolverUsuarioAnterior.HistorialNoDisponible, "No existe un usuario anterior disponible para la tarea.")
        End If
        If anterior.IdUsuario <= 0 Then
            Return BloquearHistorial(CodigosBloqueoDevolverUsuarioAnterior.HistorialGrupo, "El registro anterior no corresponde a un usuario disponible.")
        End If
        If anterior.IdRuta <> tarea.IdRuta OrElse anterior.IdActividad <= 0 OrElse anterior.IdFlujo <> tarea.IdFlujoTrabajo OrElse
           (anterior.IdFlujo > 0 AndAlso anterior.IdActividadFlujo <= 0) OrElse
           (anterior.IdFlujo = 0 AndAlso (anterior.IdActividadFlujo <> 0 OrElse anterior.IdUsuarioFlujo <> 0)) Then
            Return BloquearHistorial(CodigosBloqueoDevolverUsuarioAnterior.DestinoNoDisponible, "El usuario anterior ya no está disponible para esta tarea.")
        End If
        Dim usuario As UsuarioHistoricoDevolverUsuarioAnterior = ObtenerUsuarioElegible(contexto, tarea, anterior)
        If usuario Is Nothing Then
            Return BloquearHistorial(CodigosBloqueoDevolverUsuarioAnterior.DestinoNoDisponible, "El usuario anterior ya no está disponible para esta tarea.")
        End If
        Return New ResultadoHistorialDevolverUsuarioAnterior With {.UsuarioHistorico = usuario}
    End Function

    Private Function ObtenerUsuarioElegible(ByVal contexto As ContextoModuloWorkflow,
                                            ByVal tarea As TareaDevolverUsuarioAnterior,
                                            ByVal anterior As EstadoHistorico) As UsuarioHistoricoDevolverUsuarioAnterior
        Const sql As String = "SELECT usuario.IDU_SUARIO AS ID_USUARIO, grupo.ID_ACTIVIDAD AS ID_ACTIVIDAD, " &
                              "usuario.NOMBRE_USUARIO AS NOMBRE_USUARIO, actividad.NOMBRE_ACTIVIDAD AS NOMBRE_ACTIVIDAD " &
                              "FROM usuario_workflow AS usuario " &
                              "INNER JOIN grupos_workflow AS grupo ON grupo.ID_GRUPO = usuario.GRUPOS_WORKFLOW_ID_GRUPO " &
                              "AND grupo.RUTAS_WORKFLOW_ID_RUTA = usuario.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA " &
                              "INNER JOIN listado_actividades_workflow AS actividad ON actividad.ID_ACTIVIDAD = grupo.ID_ACTIVIDAD " &
                              "AND actividad.RUTAS_WORKFLOW_ID_RUTA = grupo.RUTAS_WORKFLOW_ID_RUTA " &
                              "WHERE usuario.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                              "AND usuario.IDU_SUARIO = @idUsuario AND grupo.ID_ACTIVIDAD = @idActividad " &
                              "AND usuario.ESTADO_USUARIO = 1 AND usuario.UTIL_ASIGNA_TAREA = 1 LIMIT 1"
        Return Leer(Of UsuarioHistoricoDevolverUsuarioAnterior)(contexto, sql,
            New List(Of IDataParameter) From {
                Parametro("@idRuta", tarea.IdRuta), Parametro("@idUsuario", anterior.IdUsuario), Parametro("@idActividad", anterior.IdActividad)},
            Function(reader As IDataReader) As UsuarioHistoricoDevolverUsuarioAnterior
                If Not reader.Read() Then Return Nothing
                Dim idUsuarioFlujo As Integer = If(anterior.IdUsuarioFlujo > 0, anterior.IdUsuarioFlujo, anterior.IdUsuario)
                Return New UsuarioHistoricoDevolverUsuarioAnterior With {
                    .IdEstadoHistorico = anterior.IdEstado,
                    .IdUsuarioWorkflow = Entero(reader, "ID_USUARIO"),
                    .IdActividad = Entero(reader, "ID_ACTIVIDAD"),
                    .IdRuta = tarea.IdRuta,
                    .IdFlujoTrabajo = anterior.IdFlujo,
                    .IdActividadFlujoTrabajo = anterior.IdActividadFlujo,
                    .IdUsuarioWorkflowFlujoTrabajo = idUsuarioFlujo,
                    .NombreUsuario = Texto(reader, "NOMBRE_USUARIO"),
                    .NombreActividad = Texto(reader, "NOMBRE_ACTIVIDAD")}
            End Function)
    End Function

    Private Function Leer(Of T)(ByVal contexto As ContextoModuloWorkflow, ByVal sql As String,
                                ByVal parametros As IEnumerable(Of IDataParameter), ByVal projector As Func(Of IDataReader, T)) As T
        Using conexion As IDbConnection = _connectionFactory.CreateOpenConnection(contexto)
            Return _dataExecutor.ExecuteReader(conexion, Nothing, sql, parametros, projector)
        End Using
    End Function

    Private Shared Function Parametro(ByVal nombre As String, ByVal valor As Object) As IDataParameter
        Return New MySqlParameter(nombre, If(valor, DBNull.Value))
    End Function

    Private Shared Function Entero(ByVal reader As IDataReader, ByVal nombre As String) As Integer
        Dim ordinal As Integer = reader.GetOrdinal(nombre)
        Return If(reader.IsDBNull(ordinal), 0, Convert.ToInt32(reader.GetValue(ordinal), CultureInfo.InvariantCulture))
    End Function

    Private Shared Function EnteroLargo(ByVal reader As IDataReader, ByVal nombre As String) As Long
        Dim ordinal As Integer = reader.GetOrdinal(nombre)
        Return If(reader.IsDBNull(ordinal), 0L, Convert.ToInt64(reader.GetValue(ordinal), CultureInfo.InvariantCulture))
    End Function

    Private Shared Function Texto(ByVal reader As IDataReader, ByVal nombre As String) As String
        Dim ordinal As Integer = reader.GetOrdinal(nombre)
        Return If(reader.IsDBNull(ordinal), String.Empty, Convert.ToString(reader.GetValue(ordinal), CultureInfo.InvariantCulture))
    End Function

    Private Shared Function BloquearAutorizacion(ByVal codigo As String, ByVal mensaje As String) As ResultadoAutorizacionDevolverUsuarioAnterior
        Return New ResultadoAutorizacionDevolverUsuarioAnterior With {.Autorizado = False, .CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function

    Private Shared Function BloquearHistorial(ByVal codigo As String, ByVal mensaje As String) As ResultadoHistorialDevolverUsuarioAnterior
        Return New ResultadoHistorialDevolverUsuarioAnterior With {.CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function

    Private Class EstadoHistorico
        Public Property IdEstado As Long
        Public Property IdTarea As Long
        Public Property IdRuta As Integer
        Public Property IdActividad As Integer
        Public Property IdUsuario As Integer
        Public Property IdFlujo As Integer
        Public Property IdActividadFlujo As Integer
        Public Property IdUsuarioFlujo As Integer
    End Class
End Class
