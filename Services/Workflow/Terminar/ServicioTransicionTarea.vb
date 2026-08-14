Imports System.Collections.Generic

'Fachada Application. La composicion con repositorios se realizara desde Presentation en fases posteriores.
Public Class ServicioTransicionTarea
    Private ReadOnly _tareaRepository As ITareaWorkflowRepository
    Private ReadOnly _flujoRepository As ITransicionFlujoRepository
    Private ReadOnly _rutaRepository As ITransicionRutaRepository
    Private ReadOnly _configuracionConectorRepository As IConfiguracionConectorRepository
    Private ReadOnly _featureGate As IWorkflowModernFeatureGate
    Private ReadOnly _validador As ValidadorTransicionTarea
    Private ReadOnly _ejecutor As EjecutorTransicionTarea

    Public Sub New(ByVal tareaRepository As ITareaWorkflowRepository,
                   ByVal flujoRepository As ITransicionFlujoRepository,
                   ByVal rutaRepository As ITransicionRutaRepository,
                   ByVal configuracionConectorRepository As IConfiguracionConectorRepository,
                   ByVal featureGate As IWorkflowModernFeatureGate,
                   ByVal validador As ValidadorTransicionTarea,
                   ByVal ejecutor As EjecutorTransicionTarea)
        _tareaRepository = tareaRepository
        _flujoRepository = flujoRepository
        _rutaRepository = rutaRepository
        _configuracionConectorRepository = configuracionConectorRepository
        _featureGate = featureGate
        _validador = validador
        _ejecutor = ejecutor
    End Sub

    Public Function EvaluarHabilitacion(ByVal contexto As ContextoModuloWorkflow) As HabilitacionWorkflowModernDto
        Dim errorContexto As ErrorTransicionDto = _validador.ValidarContexto(contexto)
        If errorContexto IsNot Nothing Then
            Return New HabilitacionWorkflowModernDto With {
                .Estado = "inactivo",
                .Codigo = errorContexto.Codigo,
                .MensajeFuncional = errorContexto.MensajeVisible,
                .Activo = False
            }
        End If

        Return New EvaluadorHabilitacionWorkflowModern(_featureGate).Evaluar(contexto)
    End Function

    Public Function Previsualizar(ByVal contexto As ContextoModuloWorkflow, ByVal idTarea As Long) As PrevisualizacionTransicionDto
        Dim habilitacion As HabilitacionWorkflowModernDto = EvaluarHabilitacion(contexto)
        Dim respuesta As New PrevisualizacionTransicionDto With {.IdTarea = idTarea}

        If Not habilitacion.Activo Then
            respuesta.[Error] = CrearError(habilitacion.Codigo, habilitacion.MensajeFuncional)
            Return respuesta
        End If

        If idTarea <= 0 Then
            respuesta.[Error] = CrearError("WORKFLOW_TASK_INVALID", "La tarea seleccionada no es valida.")
            Return respuesta
        End If

        Dim tarea As TareaWorkflow = _tareaRepository.ObtenerTarea(contexto, idTarea)
        If tarea Is Nothing OrElse Not tarea.EstaActiva Then
            respuesta.[Error] = CrearError("WORKFLOW_TASK_UNAVAILABLE", "La tarea no esta disponible para envio.")
            Return respuesta
        End If

        respuesta.Origen = tarea.IdActividadOrigen.ToString()
        respuesta.TipoDecision = tarea.TipoDecision
        respuesta.TokenVersion = tarea.TokenVersion
        If String.Equals(tarea.TipoDecision, "FLUJO", System.StringComparison.OrdinalIgnoreCase) Then
            respuesta.Destinos = New ProveedorTransicionesFlujo(_flujoRepository).Obtener(contexto, tarea)
        ElseIf String.Equals(tarea.TipoDecision, "RUTA", System.StringComparison.OrdinalIgnoreCase) Then
            respuesta.Destinos = New ProveedorTransicionesRuta(_rutaRepository).Obtener(contexto, tarea)
        Else
            respuesta.[Error] = CrearError("WORKFLOW_TRANSITION_INCONSISTENT", "No fue posible resolver el destino de la tarea.")
        End If

        Return respuesta
    End Function

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal solicitud As SolicitudTransicionWorkflow) As ResultadoTransicionDto
        Dim habilitacion As HabilitacionWorkflowModernDto = EvaluarHabilitacion(contexto)
        If Not habilitacion.Activo Then
            Return CrearResultadoBloqueado(habilitacion.Codigo, habilitacion.MensajeFuncional)
        End If

        Dim errorSolicitud As ErrorTransicionDto = _validador.ValidarSolicitud(solicitud)
        If errorSolicitud IsNot Nothing Then
            Return CrearResultadoBloqueado(errorSolicitud.Codigo, errorSolicitud.MensajeVisible)
        End If

        Dim tarea As TareaWorkflow = _tareaRepository.ObtenerTarea(contexto, solicitud.IdTarea)
        If tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
           Not String.Equals(tarea.TokenVersion, solicitud.TokenVersion, System.StringComparison.Ordinal) Then
            Return CrearResultadoBloqueado("WORKFLOW_VERSION_CONFLICT", "La tarea cambio; actualice la informacion antes de enviarla.")
        End If

        If Not _configuracionConectorRepository.EsConectorDisponible(contexto, tarea, solicitud.IdConector) Then
            Return CrearResultadoBloqueado("WORKFLOW_CONNECTOR_UNAVAILABLE", "El destino seleccionado ya no esta disponible.")
        End If

        Return _ejecutor.Ejecutar(contexto, solicitud)
    End Function

    Private Shared Function CrearError(ByVal codigo As String, ByVal mensaje As String) As ErrorTransicionDto
        Return New ErrorTransicionDto With {.Codigo = codigo, .MensajeVisible = mensaje, .ReferenciaTrazabilidad = String.Empty}
    End Function

    Private Shared Function CrearResultadoBloqueado(ByVal codigo As String, ByVal mensaje As String) As ResultadoTransicionDto
        Return New ResultadoTransicionDto With {
            .Exito = False,
            .EstadoFinal = "bloqueado",
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .[Error] = CrearError(codigo, mensaje),
            .EsReintentable = False
        }
    End Function
End Class

Public Class EvaluadorHabilitacionWorkflowModern
    Private ReadOnly _featureGate As IWorkflowModernFeatureGate

    Public Sub New(ByVal featureGate As IWorkflowModernFeatureGate)
        _featureGate = featureGate
    End Sub

    Public Function Evaluar(ByVal contexto As ContextoModuloWorkflow) As HabilitacionWorkflowModernDto
        Dim habilitacion As HabilitacionWorkflowModern = _featureGate.Evaluar(contexto)
        If habilitacion Is Nothing Then
            Return New HabilitacionWorkflowModernDto With {
                .Estado = "inactivo",
                .Codigo = "WORKFLOW_MODERN_INACTIVE",
                .MensajeFuncional = "La experiencia moderna no esta habilitada.",
                .Activo = False
            }
        End If

        Return New HabilitacionWorkflowModernDto With {
            .Estado = habilitacion.Estado,
            .Codigo = habilitacion.Codigo,
            .MensajeFuncional = habilitacion.MensajeFuncional,
            .Activo = habilitacion.EstaActiva
        }
    End Function
End Class

Public Class ProveedorTransicionesFlujo
    Private ReadOnly _repository As ITransicionFlujoRepository

    Public Sub New(ByVal repository As ITransicionFlujoRepository)
        _repository = repository
    End Sub

    Public Function Obtener(ByVal contexto As ContextoModuloWorkflow, ByVal tarea As TareaWorkflow) As IList(Of DestinoTransicionDto)
        Return Mapear(_repository.ObtenerDestinos(contexto, tarea))
    End Function

    Friend Shared Function Mapear(ByVal destinos As IList(Of DestinoTransicion)) As IList(Of DestinoTransicionDto)
        Dim resultado As New List(Of DestinoTransicionDto)()
        If destinos Is Nothing Then Return resultado

        For Each destino As DestinoTransicion In destinos
            resultado.Add(New DestinoTransicionDto With {
                .Id = destino.IdConector,
                .Nombre = destino.Nombre,
                .Tipo = destino.TipoTransicion,
                .Orden = destino.Orden
            })
        Next
        Return resultado
    End Function
End Class

Public Class ProveedorTransicionesRuta
    Private ReadOnly _repository As ITransicionRutaRepository

    Public Sub New(ByVal repository As ITransicionRutaRepository)
        _repository = repository
    End Sub

    Public Function Obtener(ByVal contexto As ContextoModuloWorkflow, ByVal tarea As TareaWorkflow) As IList(Of DestinoTransicionDto)
        Return ProveedorTransicionesFlujo.Mapear(_repository.ObtenerDestinos(contexto, tarea))
    End Function
End Class

Public Class EjecutorTransicionTarea
    Private ReadOnly _legacyExecutor As IWorkflowLegacyExecutor

    Public Sub New(ByVal legacyExecutor As IWorkflowLegacyExecutor)
        _legacyExecutor = legacyExecutor
    End Sub

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal solicitud As SolicitudTransicionWorkflow) As ResultadoTransicionDto
        Dim resultado As ResultadoEjecucionWorkflow = _legacyExecutor.Ejecutar(contexto, solicitud)
        Return New ResultadoTransicionDto With {
            .Exito = resultado.Exito,
            .EstadoFinal = resultado.EstadoFinal,
            .CodigoBloqueo = resultado.CodigoBloqueo,
            .MensajeFuncional = resultado.MensajeFuncional,
            .ReferenciaAuditoria = resultado.ReferenciaAuditoria,
            .EsReintentable = resultado.EsReintentable
        }
    End Function
End Class
