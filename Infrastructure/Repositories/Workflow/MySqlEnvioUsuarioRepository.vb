Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Web.Security

'Consultas de solo lectura y revalidación del envío directo a usuario.
Public Class MySqlEnvioUsuarioRepository
    Inherits MySqlWorkflowPreviewRepositoryBase
    Implements IEnvioUsuarioBusquedaRepository, IEnvioUsuarioEjecucionRepository

    Private Const TamanoPaginaMaximo As Integer = 50
    Private Const LongitudConsultaMaxima As Integer = 100
    Private ReadOnly _docuarchiConnectionFactory As IModuleConnectionFactory

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory,
                   ByVal docuarchiConnectionFactory As IModuleConnectionFactory,
                   ByVal dataExecutor As IDataExecutor)
        MyBase.New(connectionFactory, dataExecutor)
        _docuarchiConnectionFactory = docuarchiConnectionFactory
    End Sub

    Public Function BuscarDestinos(ByVal contexto As ContextoModuloWorkflow,
                                   ByVal tarea As TareaWorkflow,
                                   ByVal solicitud As SolicitudPreviewEnvioUsuario) As ResultadoBusquedaDestinosEnvioUsuario Implements IEnvioUsuarioBusquedaRepository.BuscarDestinos
        If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 OrElse solicitud.TamanoPagina < 1 OrElse
           solicitud.TamanoPagina > TamanoPaginaMaximo OrElse If(solicitud.Consulta, String.Empty).Length > LongitudConsultaMaxima Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.TareaInvalida,
                                    "La consulta de destinos no es valida.")
        End If

        Dim bloqueo As ResultadoBusquedaDestinosEnvioUsuario = ValidarEstado(contexto, tarea)
        If bloqueo IsNot Nothing Then Return bloqueo

        Try
            Return LeerDestinosPaginados(contexto, tarea, solicitud)
        Catch
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                    "No fue posible consultar los destinos de la tarea.")
        End Try
    End Function

    Public Function ResolverDestino(ByVal contexto As ContextoModuloWorkflow,
                                    ByVal tarea As TareaWorkflow,
                                    ByVal idUsuarioWorkflowDestino As Integer,
                                    ByVal idActividadDestino As Integer) As ResultadoResolucionEnvioUsuario Implements IEnvioUsuarioEjecucionRepository.ResolverDestino
        If idUsuarioWorkflowDestino <= 0 OrElse idActividadDestino <= 0 Then
            Return BloquearResolucion(CodigosBloqueoPrevisualizacion.UsuarioDestinoInvalido,
                                      "El destino seleccionado no es valido.")
        End If

        Dim bloqueo As ResultadoBusquedaDestinosEnvioUsuario = ValidarEstado(contexto, tarea)
        If bloqueo IsNot Nothing Then
            Return BloquearResolucion(bloqueo.CodigoBloqueo, bloqueo.MensajeFuncional)
        End If

        Try
            Dim destinos As IList(Of DestinoEnvioUsuarioWorkflow) = LeerDestinos(contexto, tarea,
                                                                                   idUsuarioWorkflowDestino,
                                                                                   idActividadDestino)
            If destinos Is Nothing OrElse destinos.Count <> 1 Then
                Return BloquearResolucion(CodigosBloqueoPrevisualizacion.UsuarioDestinoNoDisponible,
                                          "El destino seleccionado ya no esta disponible.")
            End If
            Return New ResultadoResolucionEnvioUsuario With {.Destino = destinos(0)}
        Catch
            Return BloquearResolucion(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                      "No fue posible validar el destino seleccionado.")
        End Try
    End Function

    Private Function ValidarEstado(ByVal contexto As ContextoModuloWorkflow,
                                   ByVal tarea As TareaWorkflow) As ResultadoBusquedaDestinosEnvioUsuario
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
           tarea.IdRuta <= 0 OrElse tarea.IdRuta <> contexto.IdRutaWorkflow Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.TareaNoDisponible,
                                    "La tarea no esta disponible para envio.")
        End If

        Dim bloqueoRuta As ResultadoBusquedaDestinosEnvioUsuario = ValidarRutaAbierta(contexto, tarea)
        If bloqueoRuta IsNot Nothing Then Return bloqueoRuta

        If tarea.IdFlujoTrabajo <= 0 Then Return Nothing

        Const sqlFlujo As String = "SELECT TIPO_RUTA_ABIERTA_CERRADA FROM wf_flujos_trabajo " &
                                   "WHERE ID_WF_FLUJOS_TRABAJO = @idFlujo LIMIT 1"
        Dim flujoCerrado As Boolean = EjecutarLectura(Of Boolean)(contexto, sqlFlujo,
            New List(Of IDataParameter) From {Parametro("@idFlujo", tarea.IdFlujoTrabajo)},
            Function(reader As IDataReader) As Boolean
                Return Not reader.Read() OrElse Entero(reader, "TIPO_RUTA_ABIERTA_CERRADA") <> 0
            End Function)
        If flujoCerrado Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                                    "La tarea pertenece a un flujo de trabajo cerrado.")
        End If

        If tarea.IdActividadFlujoTrabajo <= 0 Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                                    "La tarea no tiene una actividad de flujo valida.")
        End If

        Const sqlActividadFlujo As String = "SELECT TIPO_ABIERTA_CERRADA_ACTIVIDAD " &
                                            "FROM wf_registro_actividaes_flujos_trabajo " &
                                            "WHERE wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = @idFlujo " &
                                            "AND ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = @idActividadFlujo LIMIT 1"
        Dim actividadCerrada As Boolean = EjecutarLectura(Of Boolean)(contexto, sqlActividadFlujo,
            New List(Of IDataParameter) From {
                Parametro("@idFlujo", tarea.IdFlujoTrabajo),
                Parametro("@idActividadFlujo", tarea.IdActividadFlujoTrabajo)},
            Function(reader As IDataReader) As Boolean
                Return Not reader.Read() OrElse Entero(reader, "TIPO_ABIERTA_CERRADA_ACTIVIDAD") <> 0
            End Function)
        If actividadCerrada Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                                    "La tarea pertenece a una actividad de flujo cerrada.")
        End If
        Return Nothing
    End Function

    Private Function ValidarRutaAbierta(ByVal contexto As ContextoModuloWorkflow,
                                        ByVal tarea As TareaWorkflow) As ResultadoBusquedaDestinosEnvioUsuario
        Dim metadatos As MetadatosRutaWorkflow = ObtenerMetadatosRuta(contexto, tarea.IdRuta)
        If metadatos Is Nothing OrElse Not EsIdentificadorSeguro(metadatos.NombreRuta) OrElse
           Not EsIdentificadorSeguro(metadatos.CampoTramite) OrElse _docuarchiConnectionFactory Is Nothing Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                                    "No fue posible validar la ruta de la tarea.")
        End If

        Dim tabla As String = "dat_adic_tar" & metadatos.NombreRuta
        If Not EsIdentificadorSeguro(tabla) Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                                    "No fue posible validar la ruta de la tarea.")
        End If

        Dim sqlTramite As String = "SELECT `" & metadatos.CampoTramite & "` AS TRAMITE FROM `" & tabla & "` " &
                                   "WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea LIMIT 1"
        Dim tramite As String = EjecutarLectura(Of String)(contexto, sqlTramite,
            New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea)},
            Function(reader As IDataReader) As String
                Return If(reader.Read(), Texto(reader, "TRAMITE"), String.Empty)
            End Function)
        If String.IsNullOrWhiteSpace(tramite) Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.RutaCerrada,
                                    "La ruta de la tarea no esta disponible.")
        End If

        Const sqlRuta As String = "SELECT estado_ruta_open_close FROM tipo_doc_entrante " &
                                  "WHERE Descripcion_Doc = @tramite LIMIT 1"
        Dim rutaCerrada As Boolean = EjecutarLecturaDesde(Of Boolean)(_docuarchiConnectionFactory, contexto, sqlRuta,
            New List(Of IDataParameter) From {Parametro("@tramite", tramite)},
            Function(reader As IDataReader) As Boolean
                Return Not reader.Read() OrElse Entero(reader, "estado_ruta_open_close") <> 0
            End Function)
        If rutaCerrada Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.RutaCerrada,
                                    "La ruta de la tarea no esta disponible.")
        End If
        Return Nothing
    End Function

    Private Function LeerDestinos(ByVal contexto As ContextoModuloWorkflow,
                                  ByVal tarea As TareaWorkflow,
                                  ByVal idUsuarioWorkflowDestino As Integer,
                                  ByVal idActividadDestino As Integer) As IList(Of DestinoEnvioUsuarioWorkflow)
        Const sql As String = "SELECT usuario.IDU_SUARIO AS ID_USUARIO_DESTINO, grupo.ID_ACTIVIDAD AS ID_ACTIVIDAD_DESTINO, " &
                              "usuario.NOMBRE_USUARIO, usuario.CARGO_USUARIO, actividad.NOMBRE_ACTIVIDAD, " &
                              "COALESCE(actividad.estado_envio_correo, 0) AS ESTADO_ENVIO_CORREO " &
                              "FROM usuario_workflow AS usuario " &
                              "INNER JOIN grupos_workflow AS grupo ON grupo.ID_GRUPO = usuario.GRUPOS_WORKFLOW_ID_GRUPO " &
                              "AND grupo.RUTAS_WORKFLOW_ID_RUTA = usuario.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA " &
                              "INNER JOIN listado_actividades_workflow AS actividad ON actividad.ID_ACTIVIDAD = grupo.ID_ACTIVIDAD " &
                              "AND actividad.RUTAS_WORKFLOW_ID_RUTA = grupo.RUTAS_WORKFLOW_ID_RUTA " &
                              "WHERE usuario.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                              "AND usuario.ESTADO_USUARIO = 1 AND usuario.UTIL_ASIGNA_TAREA = 1 " &
                              "AND usuario.IDU_SUARIO = @idUsuarioDestino AND grupo.ID_ACTIVIDAD = @idActividadDestino " &
                              "ORDER BY usuario.NOMBRE_USUARIO, usuario.IDU_SUARIO, grupo.ID_ACTIVIDAD"
        Return EjecutarLectura(Of IList(Of DestinoEnvioUsuarioWorkflow))(contexto, sql,
            New List(Of IDataParameter) From {
                Parametro("@idRuta", tarea.IdRuta),
                Parametro("@idUsuarioDestino", idUsuarioWorkflowDestino),
                Parametro("@idActividadDestino", idActividadDestino)},
            Function(reader As IDataReader) As IList(Of DestinoEnvioUsuarioWorkflow)
                Return MapearDestinos(reader)
            End Function)
    End Function

    Private Function LeerDestinosPaginados(ByVal contexto As ContextoModuloWorkflow,
                                           ByVal tarea As TareaWorkflow,
                                           ByVal solicitud As SolicitudPreviewEnvioUsuario) As ResultadoBusquedaDestinosEnvioUsuario
        Dim cursor As CursorDestinoUsuario = Nothing
        If Not String.IsNullOrWhiteSpace(solicitud.Cursor) AndAlso Not IntentarLeerCursor(solicitud.Cursor, cursor) Then
            Return BloquearBusqueda(CodigosBloqueoPrevisualizacion.CursorUsuarioInvalido,
                                    "El cursor de destinos no es valido.")
        End If

        Dim tamanoPagina As Integer = Math.Max(1, Math.Min(TamanoPaginaMaximo, solicitud.TamanoPagina))
        Dim limite As Integer = tamanoPagina + 1
        Dim consulta As String = If(solicitud.Consulta, String.Empty).Trim()
        Dim tieneCursor As Boolean = cursor IsNot Nothing
        Dim sql As String = "SELECT usuario.IDU_SUARIO AS ID_USUARIO_DESTINO, grupo.ID_ACTIVIDAD AS ID_ACTIVIDAD_DESTINO, " &
                            "usuario.NOMBRE_USUARIO, usuario.CARGO_USUARIO, actividad.NOMBRE_ACTIVIDAD, " &
                            "COALESCE(actividad.estado_envio_correo, 0) AS ESTADO_ENVIO_CORREO " &
                            "FROM usuario_workflow AS usuario " &
                            "INNER JOIN grupos_workflow AS grupo ON grupo.ID_GRUPO = usuario.GRUPOS_WORKFLOW_ID_GRUPO " &
                            "AND grupo.RUTAS_WORKFLOW_ID_RUTA = usuario.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA " &
                            "INNER JOIN listado_actividades_workflow AS actividad ON actividad.ID_ACTIVIDAD = grupo.ID_ACTIVIDAD " &
                            "AND actividad.RUTAS_WORKFLOW_ID_RUTA = grupo.RUTAS_WORKFLOW_ID_RUTA " &
                            "WHERE usuario.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                            "AND usuario.ESTADO_USUARIO = 1 AND usuario.UTIL_ASIGNA_TAREA = 1 " &
                            "AND (@consulta = '' OR usuario.NOMBRE_USUARIO LIKE CONCAT('%', @consulta, '%') " &
                            "OR usuario.CARGO_USUARIO LIKE CONCAT('%', @consulta, '%') " &
                            "OR usuario.LOGIN_USUARIO LIKE CONCAT('%', @consulta, '%')) " &
                            "AND (@tieneCursor = 0 OR usuario.NOMBRE_USUARIO > @cursorNombre " &
                            "OR (usuario.NOMBRE_USUARIO = @cursorNombre AND usuario.IDU_SUARIO > @cursorUsuario) " &
                            "OR (usuario.NOMBRE_USUARIO = @cursorNombre AND usuario.IDU_SUARIO = @cursorUsuario " &
                            "AND grupo.ID_ACTIVIDAD > @cursorActividad)) " &
                            "ORDER BY usuario.NOMBRE_USUARIO, usuario.IDU_SUARIO, grupo.ID_ACTIVIDAD LIMIT @limite"
        Dim parametros As New List(Of IDataParameter) From {
            Parametro("@idRuta", tarea.IdRuta),
            Parametro("@consulta", consulta),
            Parametro("@tieneCursor", If(tieneCursor, 1, 0)),
            Parametro("@cursorNombre", If(tieneCursor, cursor.NombreUsuario, String.Empty)),
            Parametro("@cursorUsuario", If(tieneCursor, cursor.IdUsuarioWorkflow, 0)),
            Parametro("@cursorActividad", If(tieneCursor, cursor.IdActividad, 0)),
            Parametro("@limite", limite)
        }
        Dim filas As IList(Of DestinoEnvioUsuarioWorkflow) = EjecutarLectura(Of IList(Of DestinoEnvioUsuarioWorkflow))(
            contexto, sql, parametros,
            Function(reader As IDataReader) As IList(Of DestinoEnvioUsuarioWorkflow)
                Return MapearDestinos(reader)
            End Function)

        Dim resultado As New ResultadoBusquedaDestinosEnvioUsuario With {.TamanoPagina = tamanoPagina}
        If filas Is Nothing Then Return resultado
        resultado.TieneMas = filas.Count > tamanoPagina
        For indice As Integer = 0 To Math.Min(tamanoPagina, filas.Count) - 1
            resultado.Destinos.Add(filas(indice))
        Next
        If resultado.TieneMas AndAlso resultado.Destinos.Count > 0 Then
            resultado.CursorSiguiente = CrearCursor(resultado.Destinos(resultado.Destinos.Count - 1))
        End If
        Return resultado
    End Function

    Private Shared Function MapearDestinos(ByVal reader As IDataReader) As IList(Of DestinoEnvioUsuarioWorkflow)
        Dim destinos As New List(Of DestinoEnvioUsuarioWorkflow)()
        While reader.Read()
            destinos.Add(New DestinoEnvioUsuarioWorkflow With {
                .IdUsuarioWorkflowDestino = Entero(reader, "ID_USUARIO_DESTINO"),
                .IdActividadDestino = Entero(reader, "ID_ACTIVIDAD_DESTINO"),
                .NombreUsuarioDestino = Texto(reader, "NOMBRE_USUARIO"),
                .CargoUsuarioDestino = Texto(reader, "CARGO_USUARIO"),
                .NombreActividadDestino = Texto(reader, "NOMBRE_ACTIVIDAD"),
                .RequiereNotificacion = Entero(reader, "ESTADO_ENVIO_CORREO") <> 0
            })
        End While
        Return destinos
    End Function

    Private Shared Function CrearCursor(ByVal destino As DestinoEnvioUsuarioWorkflow) As String
        If destino Is Nothing Then Return String.Empty
        Dim contenido As String = String.Join(ChrW(31), New String() {
            If(destino.NombreUsuarioDestino, String.Empty),
            destino.IdUsuarioWorkflowDestino.ToString(Globalization.CultureInfo.InvariantCulture),
            destino.IdActividadDestino.ToString(Globalization.CultureInfo.InvariantCulture)})
        Dim protegido As Byte() = MachineKey.Protect(Encoding.UTF8.GetBytes(contenido), "WorkflowEnviarUsuarioCursor", "v1")
        If protegido Is Nothing OrElse protegido.Length = 0 Then Return String.Empty
        Return Convert.ToBase64String(protegido).TrimEnd("="c).Replace("+", "-").Replace("/", "_")
    End Function

    Private Shared Function IntentarLeerCursor(ByVal valor As String,
                                                ByRef cursor As CursorDestinoUsuario) As Boolean
        Try
            Dim base64 As String = valor.Replace("-", "+").Replace("_", "/")
            base64 = base64.PadRight(base64.Length + ((4 - base64.Length Mod 4) Mod 4), "="c)
            Dim protegido As Byte() = Convert.FromBase64String(base64)
            Dim contenido As Byte() = MachineKey.Unprotect(protegido, "WorkflowEnviarUsuarioCursor", "v1")
            If contenido Is Nothing Then Return False
            Dim partes As String() = Encoding.UTF8.GetString(contenido).Split(New Char() {ChrW(31)})
            If partes.Length <> 3 Then Return False
            Dim idUsuario As Integer = 0
            Dim idActividad As Integer = 0
            If Not Integer.TryParse(partes(1), idUsuario) OrElse Not Integer.TryParse(partes(2), idActividad) OrElse
               idUsuario <= 0 OrElse idActividad <= 0 Then Return False
            cursor = New CursorDestinoUsuario With {
                .NombreUsuario = partes(0),
                .IdUsuarioWorkflow = idUsuario,
                .IdActividad = idActividad
            }
            Return True
        Catch
            cursor = Nothing
            Return False
        End Try
    End Function

    Private Shared Function BloquearBusqueda(ByVal codigo As String,
                                              ByVal mensaje As String) As ResultadoBusquedaDestinosEnvioUsuario
        Return New ResultadoBusquedaDestinosEnvioUsuario With {.CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function

    Private Shared Function BloquearResolucion(ByVal codigo As String,
                                                ByVal mensaje As String) As ResultadoResolucionEnvioUsuario
        Return New ResultadoResolucionEnvioUsuario With {.CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function

    Private Class CursorDestinoUsuario
        Public Property NombreUsuario As String
        Public Property IdUsuarioWorkflow As Integer
        Public Property IdActividad As Integer
    End Class
End Class
