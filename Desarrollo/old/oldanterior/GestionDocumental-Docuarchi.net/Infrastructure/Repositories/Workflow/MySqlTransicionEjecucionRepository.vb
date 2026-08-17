Imports System
Imports System.Collections.Generic
Imports System.Data

'Resuelve el destino efectivo desde el estado actual del servidor. No recibe datos de Session ni DTOs de preview.
Public Class MySqlTransicionEjecucionRepository
    Inherits MySqlWorkflowPreviewRepositoryBase
    Implements ITransicionEjecucionRepository

    Private ReadOnly _rutaRepository As MySqlTransicionRutaRepository

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory,
                   ByVal docuarchiConnectionFactory As IModuleConnectionFactory,
                   ByVal dataExecutor As IDataExecutor)
        MyBase.New(connectionFactory, dataExecutor)
        _rutaRepository = New MySqlTransicionRutaRepository(connectionFactory, docuarchiConnectionFactory, dataExecutor)
    End Sub

    Public Function ResolverDestino(ByVal contexto As ContextoModuloWorkflow,
                                    ByVal tarea As TareaWorkflow,
                                    ByVal idConector As Integer) As ResultadoResolucionDestinoTransicion Implements ITransicionEjecucionRepository.ResolverDestino
        If tarea Is Nothing OrElse Not tarea.EstaActiva OrElse idConector <= 0 Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.ConectorNoDisponible,
                            "El destino seleccionado ya no esta disponible.")
        End If

        If String.Equals(tarea.TipoDecision, "RUTA", StringComparison.OrdinalIgnoreCase) Then
            Return ResolverRuta(contexto, tarea, idConector)
        End If
        If String.Equals(tarea.TipoDecision, "FLUJO", StringComparison.OrdinalIgnoreCase) Then
            Return ResolverFlujo(contexto, tarea, idConector)
        End If

        Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                        "No fue posible resolver el destino actual de la tarea.")
    End Function

    Private Function ResolverRuta(ByVal contexto As ContextoModuloWorkflow,
                                  ByVal tarea As TareaWorkflow,
                                  ByVal idConector As Integer) As ResultadoResolucionDestinoTransicion
        Dim disponibles As ResultadoDestinosTransicion = _rutaRepository.ObtenerDestinos(contexto, tarea)
        If disponibles Is Nothing Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                            "No fue posible validar la ruta de la tarea.")
        End If
        If Not String.IsNullOrWhiteSpace(disponibles.CodigoBloqueo) Then
            Return Bloquear(disponibles.CodigoBloqueo, disponibles.MensajeFuncional)
        End If

        Dim candidato As DestinoTransicion = Nothing
        If disponibles.Destinos IsNot Nothing Then
            For Each destino As DestinoTransicion In disponibles.Destinos
                If destino.IdConector = idConector Then
                    candidato = destino
                    Exit For
                End If
            Next
        End If
        If candidato Is Nothing Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.ConectorNoDisponible,
                            "El destino seleccionado ya no esta disponible.")
        End If

        Const sql As String = "SELECT disponible.Estado_evia_correo AS ESTADO_CORREO " &
                              "FROM grupos_workflow AS grupoOrigen " &
                              "INNER JOIN actividades_disponibles_envio AS disponible " &
                              "ON disponible.Listado_Actividades_Workflow_Id_Actividad = grupoOrigen.ID_ACTIVIDAD " &
                              "INNER JOIN listado_actividades_workflow AS destino " &
                              "ON destino.ID_ACTIVIDAD = disponible.ID_ACTIVIDAD_SIGUIENTE " &
                              "WHERE grupoOrigen.ID_GRUPO = @idGrupo " &
                              "AND grupoOrigen.ID_ACTIVIDAD = @idActividadOrigen " &
                              "AND disponible.id_Ruta = @idRuta " &
                              "AND destino.RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                              "AND disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO = @idConector LIMIT 1"
        Dim destinoEjecutable As DestinoEjecucionWorkflow = EjecutarLectura(Of DestinoEjecucionWorkflow)(
            contexto,
            sql,
            New List(Of IDataParameter) From {
                Parametro("@idGrupo", contexto.IdGrupoWorkflow),
                Parametro("@idActividadOrigen", tarea.IdActividadOrigen),
                Parametro("@idRuta", tarea.IdRuta),
                Parametro("@idConector", idConector)
            },
            Function(reader As IDataReader) As DestinoEjecucionWorkflow
                If Not reader.Read() Then Return Nothing
                Return New DestinoEjecucionWorkflow With {
                    .IdConector = candidato.IdConector,
                    .TipoTransicion = "RUTA",
                    .IdActividadDestino = candidato.IdActividadDestino,
                    .IdUsuarioWorkflowDestino = 0,
                    .IdGrupoWorkflowDestino = candidato.IdGrupoWorkflowDestino,
                    .IdFlujoTrabajo = 0,
                    .IdActividadFlujoTrabajoDestino = 0,
                    .IdUsuarioWorkflowFlujoTrabajoDestino = 0,
                    .IdUsuarioWorkflowFuente = contexto.IdUsuarioWorkflow,
                    .IdActividadFlujoTrabajoFuente = 0,
                    .RequiereNotificacion = Entero(reader, "ESTADO_CORREO") <> 0,
                    .NombreActividadDestino = candidato.Nombre,
                    .NombreDestinatario = candidato.NombreDestinatario,
                    .NombreGrupoDestino = candidato.NombreGrupo
                }
            End Function)

        If destinoEjecutable Is Nothing Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.ConectorNoDisponible,
                            "El destino seleccionado ya no esta disponible.")
        End If
        Return New ResultadoResolucionDestinoTransicion With {.Destino = destinoEjecutable}
    End Function

    Private Function ResolverFlujo(ByVal contexto As ContextoModuloWorkflow,
                                   ByVal tarea As TareaWorkflow,
                                   ByVal idConector As Integer) As ResultadoResolucionDestinoTransicion
        If tarea.IdFlujoTrabajo <= 0 OrElse tarea.IdActividadFlujoTrabajo <= 0 Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                            "No fue posible resolver el flujo de la tarea.")
        End If

        Const sql As String = "SELECT conector.ID_REGISTRO_ACTIVIDAD_ENVIO AS ID_CONECTOR, " &
                              "conector.ID_ACTIVIDAD_DESTINO AS ID_ACTIVIDAD_DESTINO, " &
                              "COALESCE(conector.ID_USUARIO_WORKFLOW_DESTINO, 0) AS ID_USUARIO_DESTINO, " &
                              "COALESCE(grupo.ID_GRUPO, 0) AS ID_GRUPO_DESTINO, " &
                              "conector.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO AS ID_FLUJO, " &
                              "conector.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE AS ID_ACTIVIDAD_FLUJO_FUENTE, " &
                              "conector.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO AS ID_ACTIVIDAD_FLUJO_DESTINO, " &
                              "COALESCE(conector.ID_USUARIO_WORKFLOW_FUENTE, 0) AS ID_USUARIO_FUENTE, " &
                              "COALESCE(conector.Estado_evia_correo, 0) AS ESTADO_CORREO, " &
                              "actividad.Nombre_Actividad AS NOMBRE_ACTIVIDAD, " &
                              "CONCAT_WS(' - ', usuario.Nombre_Usuario, usuario.Cargo_Usuario) AS DESTINATARIO, " &
                              "grupo.Nombre_Grupo AS NOMBRE_GRUPO " &
                              "FROM wf_registro_conectores_actividades_envio_flujo_trabajo AS conector " &
                              "INNER JOIN wf_registro_actividaes_flujos_trabajo AS actividadFlujoDestino " &
                              "ON actividadFlujoDestino.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = conector.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO " &
                              "AND actividadFlujoDestino.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = conector.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO " &
                              "INNER JOIN listado_actividades_workflow AS actividad " &
                              "ON actividad.ID_ACTIVIDAD = conector.ID_ACTIVIDAD_DESTINO " &
                              "LEFT JOIN usuario_workflow AS usuario ON usuario.idU_suario = conector.ID_USUARIO_WORKFLOW_DESTINO " &
                              "LEFT JOIN grupos_workflow AS grupo ON grupo.ID_ACTIVIDAD = actividad.ID_ACTIVIDAD " &
                              "WHERE conector.ID_REGISTRO_ACTIVIDAD_ENVIO = @idConector " &
                              "AND conector.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = @idFlujo " &
                              "AND conector.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE = @idActividadFlujoFuente " &
                              "AND (conector.ID_USUARIO_WORKFLOW_FUENTE IS NULL OR conector.ID_USUARIO_WORKFLOW_FUENTE = 0 " &
                              "OR conector.ID_USUARIO_WORKFLOW_FUENTE = @idUsuario) LIMIT 1"
        Dim destinoEjecutable As DestinoEjecucionWorkflow = EjecutarLectura(Of DestinoEjecucionWorkflow)(
            contexto,
            sql,
            New List(Of IDataParameter) From {
                Parametro("@idConector", idConector),
                Parametro("@idFlujo", tarea.IdFlujoTrabajo),
                Parametro("@idActividadFlujoFuente", tarea.IdActividadFlujoTrabajo),
                Parametro("@idUsuario", contexto.IdUsuarioWorkflow)
            },
            Function(reader As IDataReader) As DestinoEjecucionWorkflow
                If Not reader.Read() Then Return Nothing
                Return New DestinoEjecucionWorkflow With {
                    .IdConector = Entero(reader, "ID_CONECTOR"),
                    .TipoTransicion = "FLUJO",
                    .IdActividadDestino = Entero(reader, "ID_ACTIVIDAD_DESTINO"),
                    .IdUsuarioWorkflowDestino = Entero(reader, "ID_USUARIO_DESTINO"),
                    .IdGrupoWorkflowDestino = Entero(reader, "ID_GRUPO_DESTINO"),
                    .IdFlujoTrabajo = Entero(reader, "ID_FLUJO"),
                    .IdActividadFlujoTrabajoDestino = Entero(reader, "ID_ACTIVIDAD_FLUJO_DESTINO"),
                    .IdUsuarioWorkflowFlujoTrabajoDestino = Entero(reader, "ID_USUARIO_DESTINO"),
                    .IdUsuarioWorkflowFuente = Entero(reader, "ID_USUARIO_FUENTE"),
                    .IdActividadFlujoTrabajoFuente = Entero(reader, "ID_ACTIVIDAD_FLUJO_FUENTE"),
                    .RequiereNotificacion = Entero(reader, "ESTADO_CORREO") <> 0,
                    .NombreActividadDestino = Texto(reader, "NOMBRE_ACTIVIDAD"),
                    .NombreDestinatario = Texto(reader, "DESTINATARIO"),
                    .NombreGrupoDestino = Texto(reader, "NOMBRE_GRUPO")
                }
            End Function)
        If destinoEjecutable Is Nothing Then
            Return Bloquear(CodigosBloqueoPrevisualizacion.ConectorNoDisponible,
                            "El destino seleccionado ya no esta disponible.")
        End If
        Return New ResultadoResolucionDestinoTransicion With {.Destino = destinoEjecutable}
    End Function

    Private Shared Function Bloquear(ByVal codigo As String, ByVal mensaje As String) As ResultadoResolucionDestinoTransicion
        Return New ResultadoResolucionDestinoTransicion With {
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje
        }
    End Function
End Class
