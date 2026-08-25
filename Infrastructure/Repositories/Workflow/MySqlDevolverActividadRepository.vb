Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

'Repositorio exclusivo de devolución. Todas las lecturas son parametrizadas y parten de la asignación activa del usuario autenticado.
Public Class MySqlDevolverActividadRepository
    Implements IDevolverActividadTareaRepository, IDevolverActividadAutorizacionRepository,
               IDevolverActividadPreviewRepository, IDevolverActividadEjecucionRepository

    Private ReadOnly _connectionFactory As IModuleConnectionFactory
    Private ReadOnly _dataExecutor As IDataExecutor

    Public Sub New()
        Me.New(New ModuleConnectionFactory("MyDbContext"), New AdoNetDataExecutor())
    End Sub

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory, ByVal dataExecutor As IDataExecutor)
        If connectionFactory Is Nothing Then Throw New ArgumentNullException(NameOf(connectionFactory))
        If dataExecutor Is Nothing Then Throw New ArgumentNullException(NameOf(dataExecutor))

        _connectionFactory = connectionFactory
        _dataExecutor = dataExecutor
    End Sub

    Public Function ObtenerTarea(ByVal contexto As ContextoModuloWorkflow,
                                 ByVal idTarea As Long) As TareaDevolverActividad Implements IDevolverActividadTareaRepository.ObtenerTarea
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse idTarea <= 0 Then Return Nothing

        Const sqlEstado As String = "SELECT estado.id_Estado, estado.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta, " &
                                    "estado.Inicio_Tareas_Workflow_id_Tarea, estado.Id_Actividad, estado.ID_FLUJO_TRABAJO, " &
                                    "estado.ID_ACTIVIDAD_FLUJO_TRABAJO " &
                                    "FROM estados_tarea_workflow AS estado " &
                                    "WHERE estado.Inicio_Tareas_Workflow_id_Tarea = @idTarea " &
                                    "AND estado.ID_USUARIO = @idUsuario " &
                                    "AND estado.FECHA_SELECCION IS NOT NULL " &
                                    "AND estado.FECHA_FIN IS NULL " &
                                    "AND estado.ESTADO_TAREA = 0 " &
                                    "ORDER BY estado.id_Estado DESC LIMIT 1"

        Dim tarea As TareaDevolverActividad = Leer(Of TareaDevolverActividad)(contexto, sqlEstado,
            New List(Of IDataParameter) From {Parametro("@idTarea", idTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow)},
            Function(reader As IDataReader) As TareaDevolverActividad
                If Not reader.Read() Then Return Nothing
                Return New TareaDevolverActividad With {
                    .IdEstado = EnteroLargo(reader, "id_Estado"),
                    .IdTarea = EnteroLargo(reader, "Inicio_Tareas_Workflow_id_Tarea"),
                    .IdRuta = Entero(reader, "Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta"),
                    .IdActividadActual = Entero(reader, "Id_Actividad"),
                    .IdFlujoTrabajo = Entero(reader, "ID_FLUJO_TRABAJO"),
                    .IdActividadFlujoActual = Entero(reader, "ID_ACTIVIDAD_FLUJO_TRABAJO"),
                    .IdGrupoActual = contexto.IdGrupoWorkflow,
                    .EstaActiva = True
                }
            End Function)
        If tarea Is Nothing OrElse tarea.IdRuta <= 0 OrElse tarea.IdRuta <> contexto.IdRutaWorkflow OrElse tarea.IdActividadActual <= 0 Then
            Return Nothing
        End If

        Const sqlGrupoActual As String = "SELECT Nombre_Grupo FROM grupos_workflow " &
                                         "WHERE ID_GRUPO = @idGrupo " &
                                         "AND RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                                         "AND ID_ACTIVIDAD = @idActividad LIMIT 1"
        tarea.NombreGrupoActual = Leer(Of String)(contexto, sqlGrupoActual,
            New List(Of IDataParameter) From {
                Parametro("@idGrupo", tarea.IdGrupoActual),
                Parametro("@idRuta", tarea.IdRuta),
                Parametro("@idActividad", tarea.IdActividadActual)},
            Function(reader As IDataReader) As String
                Return If(reader.Read(), Texto(reader, "Nombre_Grupo"), String.Empty)
            End Function)
        If String.IsNullOrWhiteSpace(tarea.NombreGrupoActual) Then Return Nothing

        Dim metadatos As MetadatosRuta = ObtenerMetadatosRuta(contexto, tarea.IdRuta)
        If metadatos Is Nothing OrElse Not EsIdentificadorSeguro(metadatos.NombreRuta) OrElse
           Not EsIdentificadorSeguro(metadatos.CampoRadicado) Then
            Return Nothing
        End If
        Dim tabla As String = "dat_adic_tar" & metadatos.NombreRuta
        If Not EsIdentificadorSeguro(tabla) Then Return Nothing

        Dim sqlDatos As String = "SELECT `" & metadatos.CampoRadicado & "` AS RADICADO, FLUJO_TRABAJO_WF " &
                                 "FROM `" & tabla & "` WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea LIMIT 1"
        Dim encontrado As Boolean = Leer(Of Boolean)(contexto, sqlDatos,
            New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea)},
            Function(reader As IDataReader) As Boolean
                If Not reader.Read() Then Return False
                tarea.Radicado = Texto(reader, "RADICADO")
                Dim flujoDatos As Integer = Entero(reader, "FLUJO_TRABAJO_WF")
                If flujoDatos > 0 Then
                    If tarea.IdFlujoTrabajo <> flujoDatos OrElse tarea.IdActividadFlujoActual <= 0 Then Return False
                    tarea.TipoContexto = TiposContextoDevolverActividad.Flujo
                Else
                    If tarea.IdFlujoTrabajo <> 0 OrElse tarea.IdActividadFlujoActual <> 0 Then Return False
                    tarea.TipoContexto = TiposContextoDevolverActividad.Ruta
                End If
                tarea.TokenVersion = tarea.IdEstado.ToString(CultureInfo.InvariantCulture)
                Return True
            End Function)
        Return If(encontrado, tarea, Nothing)
    End Function

    Public Function Evaluar(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaDevolverActividad) As ResultadoAutorizacionDevolverActividad Implements IDevolverActividadAutorizacionRepository.Evaluar
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
           tarea.IdTarea <= 0 OrElse tarea.IdRuta <> contexto.IdRutaWorkflow Then
            Return BloquearAutorizacion(CodigosBloqueoDevolverActividad.TareaNoDisponible,
                                        "La tarea no está disponible para devolución.")
        End If
        If Not contexto.PuedeDevolverActividad Then
            Return BloquearAutorizacion(CodigosBloqueoDevolverActividad.PermisoDenegado,
                                        "El usuario no tiene permiso para devolver la tarea.")
        End If
        Return New ResultadoAutorizacionDevolverActividad With {.Autorizado = True}
    End Function

    Public Function BuscarDestinos(ByVal contexto As ContextoModuloWorkflow,
                                   ByVal tarea As TareaDevolverActividad,
                                   ByVal solicitud As SolicitudPreviewDevolverActividad) As ResultadoBusquedaDevolverActividad Implements IDevolverActividadPreviewRepository.BuscarDestinos
        If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 OrElse solicitud.TamanoPagina < 1 Then
            Return BloquearBusqueda(CodigosBloqueoDevolverActividad.TareaInvalida, "La consulta de devolución no es válida.")
        End If
        Dim autorizacion As ResultadoAutorizacionDevolverActividad = Evaluar(contexto, tarea)
        If Not autorizacion.Autorizado Then Return BloquearBusqueda(autorizacion.CodigoBloqueo, autorizacion.MensajeFuncional)

        Try
            If String.Equals(tarea.TipoContexto, TiposContextoDevolverActividad.Ruta, StringComparison.OrdinalIgnoreCase) Then
                Return LeerDestinosRuta(contexto, tarea, solicitud, 0)
            End If
            If String.Equals(tarea.TipoContexto, TiposContextoDevolverActividad.Flujo, StringComparison.OrdinalIgnoreCase) Then
                Return LeerDestinosFlujo(contexto, tarea, solicitud, 0)
            End If
        Catch
            Return BloquearBusqueda(CodigosBloqueoDevolverActividad.NoDisponible,
                                    "No fue posible consultar las actividades anteriores.")
        End Try
        Return BloquearBusqueda(CodigosBloqueoDevolverActividad.ContextoInconsistente,
                                "El contexto de la tarea no es válido para devolución.")
    End Function

    Public Function ResolverDestino(ByVal contexto As ContextoModuloWorkflow,
                                    ByVal tarea As TareaDevolverActividad,
                                    ByVal idConector As Integer) As ResultadoResolucionDevolverActividad Implements IDevolverActividadEjecucionRepository.ResolverDestino
        If idConector <= 0 Then
            Return BloquearResolucion(CodigosBloqueoDevolverActividad.ConectorInvalido,
                                      "El conector seleccionado no es válido.")
        End If
        Dim autorizacion As ResultadoAutorizacionDevolverActividad = Evaluar(contexto, tarea)
        If Not autorizacion.Autorizado Then Return BloquearResolucion(autorizacion.CodigoBloqueo, autorizacion.MensajeFuncional)

        Dim solicitud As New SolicitudPreviewDevolverActividad With {
            .IdTarea = tarea.IdTarea,
            .TamanoPagina = 1,
            .Termino = String.Empty
        }
        Try
            Dim resultado As ResultadoBusquedaDevolverActividad = Nothing
            If String.Equals(tarea.TipoContexto, TiposContextoDevolverActividad.Ruta, StringComparison.OrdinalIgnoreCase) Then
                resultado = LeerDestinosRuta(contexto, tarea, solicitud, idConector)
            ElseIf String.Equals(tarea.TipoContexto, TiposContextoDevolverActividad.Flujo, StringComparison.OrdinalIgnoreCase) Then
                resultado = LeerDestinosFlujo(contexto, tarea, solicitud, idConector)
            Else
                Return BloquearResolucion(CodigosBloqueoDevolverActividad.ContextoInconsistente,
                                          "El contexto de la tarea no es válido para devolución.")
            End If
            If resultado Is Nothing OrElse Not String.IsNullOrWhiteSpace(resultado.CodigoBloqueo) OrElse
               resultado.Destinos Is Nothing OrElse resultado.Destinos.Count <> 1 Then
                Return BloquearResolucion(If(resultado Is Nothing OrElse String.IsNullOrWhiteSpace(resultado.CodigoBloqueo),
                                             CodigosBloqueoDevolverActividad.ConectorNoDisponible,
                                             resultado.CodigoBloqueo),
                                          If(resultado Is Nothing OrElse String.IsNullOrWhiteSpace(resultado.MensajeFuncional),
                                             "La actividad anterior ya no está disponible.", resultado.MensajeFuncional))
            End If
            Return New ResultadoResolucionDevolverActividad With {.Destino = resultado.Destinos(0)}
        Catch
            Return BloquearResolucion(CodigosBloqueoDevolverActividad.NoDisponible,
                                      "No fue posible validar la actividad anterior.")
        End Try
    End Function

    Private Function LeerDestinosRuta(ByVal contexto As ContextoModuloWorkflow,
                                      ByVal tarea As TareaDevolverActividad,
                                      ByVal solicitud As SolicitudPreviewDevolverActividad,
                                      ByVal idConector As Integer) As ResultadoBusquedaDevolverActividad
        Dim limite As Integer = Math.Min(51, Math.Max(1, solicitud.TamanoPagina + 1))
        Dim desplazamiento As Integer = If(idConector > 0, 0, Math.Max(0, solicitud.OrdenDespuesDe))
        Dim sql As String = "SELECT disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO AS ID_CONECTOR, " &
                            "origen.ID_ACTIVIDAD AS ID_ACTIVIDAD_ORIGEN, destino.ID_ACTIVIDAD AS ID_ACTIVIDAD_DESTINO, " &
                            "COALESCE(MIN(grupoDestino.ID_GRUPO), 0) AS ID_GRUPO_DESTINO, " &
                            "origen.NOMBRE_ACTIVIDAD AS NOMBRE_ACTIVIDAD, MIN(grupoDestino.NOMBRE_GRUPO) AS NOMBRE_GRUPO, " &
                            "COALESCE(disponible.Estado_evia_correo, 0) AS ESTADO_CORREO " &
                            "FROM actividades_disponibles_envio AS disponible " &
                            "INNER JOIN listado_actividades_workflow AS origen " &
                            "ON origen.ID_ACTIVIDAD = disponible.Listado_Actividades_Workflow_Id_Actividad " &
                            "INNER JOIN listado_actividades_workflow AS destino " &
                            "ON destino.ID_ACTIVIDAD = disponible.ID_ACTIVIDAD_SIGUIENTE " &
                            "LEFT JOIN grupos_workflow AS grupoDestino ON grupoDestino.ID_ACTIVIDAD = origen.ID_ACTIVIDAD " &
                            "AND grupoDestino.RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                            "WHERE disponible.ID_ACTIVIDAD_SIGUIENTE = @idActividadActual " &
                            "AND disponible.id_Ruta = @idRuta " &
                            "AND origen.RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                            "AND destino.RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                            "AND (@termino = '' OR origen.NOMBRE_ACTIVIDAD LIKE CONCAT('%', @termino, '%') " &
                            "OR grupoDestino.NOMBRE_GRUPO LIKE CONCAT('%', @termino, '%')) "
        Dim parametros As New List(Of IDataParameter) From {
            Parametro("@idActividadActual", tarea.IdActividadActual),
            Parametro("@idRuta", tarea.IdRuta),
            Parametro("@termino", If(solicitud.Termino, String.Empty))}
        If idConector > 0 Then
            sql &= "AND disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO = @idConector "
            parametros.Add(Parametro("@idConector", idConector))
        End If
        sql &= "GROUP BY disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO, origen.ID_ACTIVIDAD, destino.ID_ACTIVIDAD, " &
               "origen.NOMBRE_ACTIVIDAD, disponible.Estado_evia_correo " &
               "ORDER BY origen.NOMBRE_ACTIVIDAD, disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO " &
               "LIMIT @limite OFFSET @desplazamiento"
        parametros.Add(Parametro("@limite", limite))
        parametros.Add(Parametro("@desplazamiento", desplazamiento))
        Return MaterializarBusqueda(contexto, sql, parametros, tarea, solicitud, TiposContextoDevolverActividad.Ruta)
    End Function

    Private Function LeerDestinosFlujo(ByVal contexto As ContextoModuloWorkflow,
                                       ByVal tarea As TareaDevolverActividad,
                                       ByVal solicitud As SolicitudPreviewDevolverActividad,
                                       ByVal idConector As Integer) As ResultadoBusquedaDevolverActividad
        If tarea.IdFlujoTrabajo <= 0 OrElse tarea.IdActividadFlujoActual <= 0 Then
            Return BloquearBusqueda(CodigosBloqueoDevolverActividad.ContextoInconsistente,
                                    "El flujo de la tarea no es válido para devolución.")
        End If
        Dim limite As Integer = Math.Min(51, Math.Max(1, solicitud.TamanoPagina + 1))
        Dim desplazamiento As Integer = If(idConector > 0, 0, Math.Max(0, solicitud.OrdenDespuesDe))
        Dim sql As String = "SELECT conector.ID_REGISTRO_ACTIVIDAD_ENVIO AS ID_CONECTOR, " &
                            "origenFlujo.listado_actividades_workflow_id_Actividad AS ID_ACTIVIDAD_ORIGEN, " &
                            "conector.ID_ACTIVIDAD_FUENTE AS ID_ACTIVIDAD_DESTINO, " &
                            "conector.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE AS ID_ACTIVIDAD_FLUJO_ORIGEN, " &
                            "conector.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO AS ID_ACTIVIDAD_FLUJO_DESTINO, " &
                            "COALESCE(conector.ID_USUARIO_WORKFLOW_FUENTE, 0) AS ID_USUARIO_DESTINO, " &
                            "COALESCE(MIN(grupoOrigen.ID_GRUPO), 0) AS ID_GRUPO_DESTINO, " &
                            "origen.NOMBRE_ACTIVIDAD AS NOMBRE_ACTIVIDAD, " &
                            "CONCAT_WS(' - ', usuario.Nombre_Usuario, usuario.Cargo_Usuario) AS NOMBRE_USUARIO, " &
                            "MIN(grupoOrigen.NOMBRE_GRUPO) AS NOMBRE_GRUPO, " &
                            "COALESCE(conector.Estado_evia_correo, 0) AS ESTADO_CORREO " &
                            "FROM wf_registro_conectores_actividades_envio_flujo_trabajo AS conector " &
                            "INNER JOIN wf_registro_actividaes_flujos_trabajo AS origenFlujo " &
                            "ON origenFlujo.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = conector.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE " &
                            "AND origenFlujo.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = conector.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO " &
                            "INNER JOIN listado_actividades_workflow AS origen " &
                            "ON origen.ID_ACTIVIDAD = origenFlujo.listado_actividades_workflow_id_Actividad " &
                            "LEFT JOIN usuario_workflow AS usuario ON usuario.idU_suario = conector.ID_USUARIO_WORKFLOW_FUENTE " &
                            "AND usuario.ESTADO_USUARIO = 1 " &
                            "LEFT JOIN grupos_workflow AS grupoOrigen ON grupoOrigen.ID_ACTIVIDAD = origen.ID_ACTIVIDAD " &
                            "AND grupoOrigen.RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                            "WHERE conector.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = @idFlujo " &
                            "AND conector.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO = @idActividadFlujoActual " &
                            "AND origen.RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                            "AND ((COALESCE(conector.ID_USUARIO_WORKFLOW_FUENTE, 0) > 0 " &
                            "AND EXISTS (SELECT 1 FROM usuario_workflow AS usuarioValido " &
                            "INNER JOIN grupos_workflow AS grupoValido ON grupoValido.ID_GRUPO = usuarioValido.GRUPOS_WORKFLOW_ID_GRUPO " &
                            "WHERE grupoValido.RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                            "AND grupoValido.ID_ACTIVIDAD = origen.ID_ACTIVIDAD " &
                            "AND usuarioValido.idU_suario = conector.ID_USUARIO_WORKFLOW_FUENTE " &
                            "AND usuarioValido.ESTADO_USUARIO = 1) " &
                            ") OR (COALESCE(conector.ID_USUARIO_WORKFLOW_FUENTE, 0) = 0 " &
                            "AND EXISTS (SELECT 1 FROM grupos_workflow AS grupoValido " &
                            "WHERE grupoValido.ID_ACTIVIDAD = origen.ID_ACTIVIDAD " &
                            "AND grupoValido.RUTAS_WORKFLOW_ID_RUTA = @idRuta))) " &
                            "AND (@termino = '' OR origen.NOMBRE_ACTIVIDAD LIKE CONCAT('%', @termino, '%') " &
                            "OR usuario.Nombre_Usuario LIKE CONCAT('%', @termino, '%') " &
                            "OR grupoOrigen.NOMBRE_GRUPO LIKE CONCAT('%', @termino, '%')) "
        Dim parametros As New List(Of IDataParameter) From {
            Parametro("@idRuta", tarea.IdRuta),
            Parametro("@idFlujo", tarea.IdFlujoTrabajo),
            Parametro("@idActividadFlujoActual", tarea.IdActividadFlujoActual),
            Parametro("@termino", If(solicitud.Termino, String.Empty))}
        If idConector > 0 Then
            sql &= "AND conector.ID_REGISTRO_ACTIVIDAD_ENVIO = @idConector "
            parametros.Add(Parametro("@idConector", idConector))
        End If
        sql &= "GROUP BY conector.ID_REGISTRO_ACTIVIDAD_ENVIO, origenFlujo.listado_actividades_workflow_id_Actividad, " &
               "conector.ID_ACTIVIDAD_FUENTE, conector.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE, " &
               "conector.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO, conector.ID_USUARIO_WORKFLOW_FUENTE, " &
               "origen.NOMBRE_ACTIVIDAD, usuario.Nombre_Usuario, usuario.Cargo_Usuario, conector.Estado_evia_correo " &
               "ORDER BY origen.NOMBRE_ACTIVIDAD, conector.ID_REGISTRO_ACTIVIDAD_ENVIO " &
               "LIMIT @limite OFFSET @desplazamiento"
        parametros.Add(Parametro("@limite", limite))
        parametros.Add(Parametro("@desplazamiento", desplazamiento))
        Return MaterializarBusqueda(contexto, sql, parametros, tarea, solicitud, TiposContextoDevolverActividad.Flujo)
    End Function

    Private Function MaterializarBusqueda(ByVal contexto As ContextoModuloWorkflow,
                                          ByVal sql As String,
                                          ByVal parametros As IEnumerable(Of IDataParameter),
                                          ByVal tarea As TareaDevolverActividad,
                                          ByVal solicitud As SolicitudPreviewDevolverActividad,
                                          ByVal tipo As String) As ResultadoBusquedaDevolverActividad
        Dim resultado As New ResultadoBusquedaDevolverActividad With {.TamanoPagina = Math.Min(50, Math.Max(1, solicitud.TamanoPagina))}
        Dim filas As IList(Of DestinoDevolverActividad) = Leer(Of IList(Of DestinoDevolverActividad))(contexto, sql, parametros,
            Function(reader As IDataReader) As IList(Of DestinoDevolverActividad)
                Dim destinos As New List(Of DestinoDevolverActividad)()
                Dim orden As Integer = Math.Max(0, solicitud.OrdenDespuesDe)
                While reader.Read()
                    orden += 1
                    destinos.Add(New DestinoDevolverActividad With {
                        .IdConector = Entero(reader, "ID_CONECTOR"),
                        .TipoContexto = tipo,
                        .IdActividadOrigen = Entero(reader, "ID_ACTIVIDAD_ORIGEN"),
                        .IdActividadDestino = Entero(reader, "ID_ACTIVIDAD_DESTINO"),
                        .IdFlujoTrabajo = If(String.Equals(tipo, TiposContextoDevolverActividad.Flujo, StringComparison.OrdinalIgnoreCase), tarea.IdFlujoTrabajo, 0),
                        .IdActividadFlujoOrigen = EnteroSeguro(reader, "ID_ACTIVIDAD_FLUJO_ORIGEN"),
                        .IdActividadFlujoDestino = EnteroSeguro(reader, "ID_ACTIVIDAD_FLUJO_DESTINO"),
                        .IdUsuarioWorkflowDestino = EnteroSeguro(reader, "ID_USUARIO_DESTINO"),
                        .IdGrupoWorkflowDestino = Entero(reader, "ID_GRUPO_DESTINO"),
                        .NombreActividad = Texto(reader, "NOMBRE_ACTIVIDAD"),
                        .NombreUsuarioDestino = TextoSeguro(reader, "NOMBRE_USUARIO"),
                        .NombreGrupoDestino = Texto(reader, "NOMBRE_GRUPO"),
                        .RequiereNotificacion = Entero(reader, "ESTADO_CORREO") <> 0,
                        .Orden = orden
                    })
                End While
                Return destinos
            End Function)
        If filas Is Nothing Then Return resultado
        resultado.HayMas = filas.Count > resultado.TamanoPagina
        For indice As Integer = 0 To Math.Min(resultado.TamanoPagina, filas.Count) - 1
            resultado.Destinos.Add(filas(indice))
        Next
        If resultado.Destinos.Count = 0 Then
            resultado.CodigoBloqueo = CodigosBloqueoDevolverActividad.SinDestinos
            resultado.MensajeFuncional = "No hay actividades anteriores disponibles para esta tarea."
        End If
        Return resultado
    End Function

    Private Function ObtenerMetadatosRuta(ByVal contexto As ContextoModuloWorkflow, ByVal idRuta As Integer) As MetadatosRuta
        Const sql As String = "SELECT rw.Nombre_Ruta, MAX(CASE WHEN clr.Campo_Radicado = 1 THEN clr.Nombre_Campo END) AS Campo_Radicado " &
                              "FROM rutas_workflow AS rw " &
                              "LEFT JOIN configuracion_listado_ruta AS clr ON clr.Rutas_Workflow_id_Ruta = rw.ID_RUTA " &
                              "WHERE rw.ID_RUTA = @idRuta GROUP BY rw.ID_RUTA, rw.Nombre_Ruta"
        Return Leer(Of MetadatosRuta)(contexto, sql, New List(Of IDataParameter) From {Parametro("@idRuta", idRuta)},
            Function(reader As IDataReader) As MetadatosRuta
                If Not reader.Read() Then Return Nothing
                Return New MetadatosRuta With {.NombreRuta = Texto(reader, "Nombre_Ruta"), .CampoRadicado = Texto(reader, "Campo_Radicado")}
            End Function)
    End Function

    Private Function Leer(Of T)(ByVal contexto As ContextoModuloWorkflow,
                                ByVal sql As String,
                                ByVal parametros As IEnumerable(Of IDataParameter),
                                ByVal projector As Func(Of IDataReader, T)) As T
        Using connection As IDbConnection = _connectionFactory.CreateOpenConnection(contexto)
            Return _dataExecutor.ExecuteReader(connection, Nothing, sql, parametros, projector)
        End Using
    End Function

    Private Shared Function Parametro(ByVal nombre As String, ByVal valor As Object) As IDataParameter
        Return New MySqlParameter(nombre, If(valor, DBNull.Value))
    End Function

    Private Shared Function Texto(ByVal reader As IDataReader, ByVal fieldName As String) As String
        Dim ordinal As Integer = reader.GetOrdinal(fieldName)
        Return If(reader.IsDBNull(ordinal), String.Empty, Convert.ToString(reader.GetValue(ordinal), CultureInfo.InvariantCulture))
    End Function

    Private Shared Function TextoSeguro(ByVal reader As IDataReader, ByVal fieldName As String) As String
        Try
            Return Texto(reader, fieldName)
        Catch
            Return String.Empty
        End Try
    End Function

    Private Shared Function Entero(ByVal reader As IDataReader, ByVal fieldName As String) As Integer
        Dim ordinal As Integer = reader.GetOrdinal(fieldName)
        Return If(reader.IsDBNull(ordinal), 0, Convert.ToInt32(reader.GetValue(ordinal), CultureInfo.InvariantCulture))
    End Function

    Private Shared Function EnteroSeguro(ByVal reader As IDataReader, ByVal fieldName As String) As Integer
        Try
            Return Entero(reader, fieldName)
        Catch
            Return 0
        End Try
    End Function

    Private Shared Function EnteroLargo(ByVal reader As IDataReader, ByVal fieldName As String) As Long
        Dim ordinal As Integer = reader.GetOrdinal(fieldName)
        Return If(reader.IsDBNull(ordinal), 0L, Convert.ToInt64(reader.GetValue(ordinal), CultureInfo.InvariantCulture))
    End Function

    Private Shared Function EsIdentificadorSeguro(ByVal value As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso Regex.IsMatch(value, "^[A-Za-z0-9_]+$")
    End Function

    Private Shared Function BloquearAutorizacion(ByVal codigo As String, ByVal mensaje As String) As ResultadoAutorizacionDevolverActividad
        Return New ResultadoAutorizacionDevolverActividad With {.Autorizado = False, .CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function

    Private Shared Function BloquearBusqueda(ByVal codigo As String, ByVal mensaje As String) As ResultadoBusquedaDevolverActividad
        Return New ResultadoBusquedaDevolverActividad With {.CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function

    Private Shared Function BloquearResolucion(ByVal codigo As String, ByVal mensaje As String) As ResultadoResolucionDevolverActividad
        Return New ResultadoResolucionDevolverActividad With {.CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function

    Private Class MetadatosRuta
        Public Property NombreRuta As String
        Public Property CampoRadicado As String
    End Class
End Class
