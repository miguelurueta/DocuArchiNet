Imports System
Imports System.Collections.Generic
Imports System.Data

'Consultas de solo lectura y revalidación del envío directo a una actividad de grupo.
Public Class MySqlEnvioGrupoRepository
    Inherits MySqlWorkflowPreviewRepositoryBase
    Implements IEnvioGrupoDestinosRepository, IEnvioGrupoEjecucionRepository

    Private ReadOnly _docuarchiConnectionFactory As IModuleConnectionFactory

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory,
                   ByVal docuarchiConnectionFactory As IModuleConnectionFactory,
                   ByVal dataExecutor As IDataExecutor)
        MyBase.New(connectionFactory, dataExecutor)
        _docuarchiConnectionFactory = docuarchiConnectionFactory
    End Sub

    Public Function ObtenerDestinos(ByVal contexto As ContextoModuloWorkflow,
                                    ByVal tarea As TareaWorkflow) As ResultadoDestinosEnvioGrupo Implements IEnvioGrupoDestinosRepository.ObtenerDestinos
        Dim bloqueo As ResultadoDestinosEnvioGrupo = ValidarEstado(contexto, tarea)
        If bloqueo IsNot Nothing Then Return bloqueo

        Try
            Return New ResultadoDestinosEnvioGrupo With {.Destinos = LeerDestinos(contexto, tarea, 0)}
        Catch
            Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                            "No fue posible consultar los destinos de la tarea.")
        End Try
    End Function

    Public Function ResolverDestino(ByVal contexto As ContextoModuloWorkflow,
                                    ByVal tarea As TareaWorkflow,
                                    ByVal idActividadDestino As Integer) As ResultadoResolucionEnvioGrupo Implements IEnvioGrupoEjecucionRepository.ResolverDestino
        If idActividadDestino <= 0 Then
            Return BloquearResolucion(CodigosBloqueoPrevisualizacion.ActividadDestinoInvalida,
                                      "El destino seleccionado no es valido.")
        End If

        Dim bloqueo As ResultadoDestinosEnvioGrupo = ValidarEstado(contexto, tarea)
        If bloqueo IsNot Nothing Then
            Return BloquearResolucion(bloqueo.CodigoBloqueo, bloqueo.MensajeFuncional)
        End If

        Try
            Dim destinos As IList(Of DestinoEnvioGrupoWorkflow) = LeerDestinos(contexto, tarea, idActividadDestino)
            If destinos Is Nothing OrElse destinos.Count <> 1 Then
                Return BloquearResolucion(CodigosBloqueoPrevisualizacion.ActividadDestinoNoDisponible,
                                          "El destino seleccionado ya no esta disponible.")
            End If
            Return New ResultadoResolucionEnvioGrupo With {.Destino = destinos(0)}
        Catch
            Return BloquearResolucion(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                      "No fue posible validar el destino seleccionado.")
        End Try
    End Function

    Private Function ValidarEstado(ByVal contexto As ContextoModuloWorkflow,
                                   ByVal tarea As TareaWorkflow) As ResultadoDestinosEnvioGrupo
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
           tarea.IdRuta <= 0 OrElse tarea.IdRuta <> contexto.IdRutaWorkflow Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.TareaNoDisponible,
                            "La tarea no esta disponible para envio.")
        End If

        If tarea.IdFlujoTrabajo > 0 Then
            Const sqlFlujo As String = "SELECT TIPO_RUTA_ABIERTA_CERRADA FROM wf_flujos_trabajo " &
                                       "WHERE ID_WF_FLUJOS_TRABAJO = @idFlujo LIMIT 1"
            Dim flujoCerrado As Boolean = EjecutarLectura(Of Boolean)(contexto, sqlFlujo,
                New List(Of IDataParameter) From {Parametro("@idFlujo", tarea.IdFlujoTrabajo)},
                Function(reader As IDataReader) As Boolean
                    Return Not reader.Read() OrElse Entero(reader, "TIPO_RUTA_ABIERTA_CERRADA") <> 0
                End Function)
            If flujoCerrado Then
                Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                                "La tarea pertenece a un flujo de trabajo cerrado.")
            End If

            If tarea.IdActividadFlujoTrabajo > 0 Then
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
                    Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                                    "La tarea pertenece a una actividad de flujo cerrada.")
                End If
            End If
            Return Nothing
        End If

        Dim metadatos As MetadatosRutaWorkflow = ObtenerMetadatosRuta(contexto, tarea.IdRuta)
        If metadatos Is Nothing OrElse Not EsIdentificadorSeguro(metadatos.NombreRuta) OrElse
           Not EsIdentificadorSeguro(metadatos.CampoTramite) OrElse _docuarchiConnectionFactory Is Nothing Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                            "No fue posible validar la ruta de la tarea.")
        End If

        Dim tabla As String = "dat_adic_tar" & metadatos.NombreRuta
        If Not EsIdentificadorSeguro(tabla) Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
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
            Return Bloquear(CodigosBloqueoPrevisualizacion.RutaCerrada,
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
            Return Bloquear(CodigosBloqueoPrevisualizacion.RutaCerrada,
                            "La ruta de la tarea no esta disponible.")
        End If
        Return Nothing
    End Function

    Private Function LeerDestinos(ByVal contexto As ContextoModuloWorkflow,
                                  ByVal tarea As TareaWorkflow,
                                  ByVal idActividadDestino As Integer) As IList(Of DestinoEnvioGrupoWorkflow)
        Dim sql As String = "SELECT actividad.ID_ACTIVIDAD AS ID_ACTIVIDAD_DESTINO, actividad.NOMBRE_ACTIVIDAD, " &
                            "COALESCE(grupo.ID_GRUPO, 0) AS ID_GRUPO_DESTINO, grupo.NOMBRE_GRUPO, " &
                            "COALESCE(actividad.estado_envio_correo, 0) AS ESTADO_ENVIO_CORREO " &
                            "FROM LISTADO_ACTIVIDADES_WORKFLOW AS actividad " &
                            "LEFT JOIN grupos_workflow AS grupo ON grupo.ID_ACTIVIDAD = actividad.ID_ACTIVIDAD " &
                            "AND grupo.RUTAS_WORKFLOW_ID_RUTA = actividad.RUTAS_WORKFLOW_ID_RUTA " &
                            "WHERE actividad.RUTAS_WORKFLOW_ID_RUTA = @idRuta "
        If idActividadDestino > 0 Then sql &= "AND actividad.ID_ACTIVIDAD = @idActividadDestino "
        sql &= "ORDER BY actividad.NOMBRE_ACTIVIDAD, actividad.ID_ACTIVIDAD"

        Dim parametros As New List(Of IDataParameter) From {Parametro("@idRuta", tarea.IdRuta)}
        If idActividadDestino > 0 Then parametros.Add(Parametro("@idActividadDestino", idActividadDestino))
        Return EjecutarLectura(Of IList(Of DestinoEnvioGrupoWorkflow))(contexto, sql, parametros,
            Function(reader As IDataReader) As IList(Of DestinoEnvioGrupoWorkflow)
                Dim destinos As New List(Of DestinoEnvioGrupoWorkflow)()
                While reader.Read()
                    destinos.Add(New DestinoEnvioGrupoWorkflow With {
                        .IdActividadDestino = Entero(reader, "ID_ACTIVIDAD_DESTINO"),
                        .IdGrupoWorkflowDestino = Entero(reader, "ID_GRUPO_DESTINO"),
                        .NombreActividad = Texto(reader, "NOMBRE_ACTIVIDAD"),
                        .NombreGrupoDestino = Texto(reader, "NOMBRE_GRUPO"),
                        .RequiereNotificacion = Entero(reader, "ESTADO_ENVIO_CORREO") <> 0
                    })
                End While
                Return destinos
            End Function)
    End Function

    Private Shared Function Bloquear(ByVal codigo As String, ByVal mensaje As String) As ResultadoDestinosEnvioGrupo
        Return New ResultadoDestinosEnvioGrupo With {.CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function

    Private Shared Function BloquearResolucion(ByVal codigo As String, ByVal mensaje As String) As ResultadoResolucionEnvioGrupo
        Return New ResultadoResolucionEnvioGrupo With {.CodigoBloqueo = codigo, .MensajeFuncional = mensaje}
    End Function
End Class
