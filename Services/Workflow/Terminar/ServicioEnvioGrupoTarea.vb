Imports System
Imports System.Collections.Generic
Imports System.Diagnostics

'Aplicación para el reenvío directo a una actividad. No modifica el servicio de transiciones por conector.
Public Class ServicioEnvioGrupoTarea
    Private ReadOnly _tareaRepository As ITareaWorkflowRepository
    Private ReadOnly _destinosRepository As IEnvioGrupoDestinosRepository
    Private ReadOnly _ejecucionRepository As IEnvioGrupoEjecucionRepository
    Private ReadOnly _requisitosRepository As IEnvioGrupoRequisitosRepository
    Private ReadOnly _auditoriaRepository As IAuditoriaTransicionRepository
    Private ReadOnly _concurrencyGuard As ITransicionConcurrencyGuard
    Private ReadOnly _featureGate As IWorkflowModernFeatureGate
    Private ReadOnly _ejecutor As IEnvioGrupoLegacyExecutor
    Private ReadOnly _validadorSolicitud As ValidadorEnvioGrupoTarea
    Private ReadOnly _validadorContexto As ValidadorTransicionTarea

    Public Sub New(ByVal tareaRepository As ITareaWorkflowRepository,
                   ByVal destinosRepository As IEnvioGrupoDestinosRepository,
                   ByVal featureGate As IWorkflowModernFeatureGate,
                   ByVal validadorSolicitud As ValidadorEnvioGrupoTarea)
        Me.New(tareaRepository, destinosRepository, Nothing, Nothing, Nothing, Nothing, featureGate, Nothing, validadorSolicitud)
    End Sub

    Public Sub New(ByVal tareaRepository As ITareaWorkflowRepository,
                   ByVal destinosRepository As IEnvioGrupoDestinosRepository,
                   ByVal ejecucionRepository As IEnvioGrupoEjecucionRepository,
                   ByVal requisitosRepository As IEnvioGrupoRequisitosRepository,
                   ByVal auditoriaRepository As IAuditoriaTransicionRepository,
                   ByVal concurrencyGuard As ITransicionConcurrencyGuard,
                   ByVal featureGate As IWorkflowModernFeatureGate,
                   ByVal ejecutor As IEnvioGrupoLegacyExecutor,
                   ByVal validadorSolicitud As ValidadorEnvioGrupoTarea)
        _tareaRepository = tareaRepository
        _destinosRepository = destinosRepository
        _ejecucionRepository = ejecucionRepository
        _requisitosRepository = requisitosRepository
        _auditoriaRepository = auditoriaRepository
        _concurrencyGuard = concurrencyGuard
        _featureGate = featureGate
        _ejecutor = ejecutor
        _validadorSolicitud = If(validadorSolicitud, New ValidadorEnvioGrupoTarea())
        _validadorContexto = New ValidadorTransicionTarea()
    End Sub

    Public Function Previsualizar(ByVal contexto As ContextoModuloWorkflow,
                                  ByVal idTarea As Long) As PrevisualizacionEnvioGrupoDto
        Dim respuesta As New PrevisualizacionEnvioGrupoDto With {.IdTarea = idTarea}
        Dim habilitacion As HabilitacionWorkflowModernDto = EvaluarHabilitacion(contexto)
        If Not habilitacion.Activo Then
            respuesta.[Error] = CrearError(habilitacion.Codigo, habilitacion.MensajeFuncional)
            Return respuesta
        End If
        If Not TieneCambioRuta(contexto) Then
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.PermisoCambioRutaDenegado,
                                           "El usuario no tiene permiso para enviar la tarea a otra actividad.")
            Return respuesta
        End If
        If idTarea <= 0 OrElse _tareaRepository Is Nothing OrElse _destinosRepository Is Nothing Then
            respuesta.[Error] = CrearError(If(idTarea <= 0, CodigosBloqueoPrevisualizacion.TareaInvalida,
                                             CodigosBloqueoPrevisualizacion.TransicionNoDisponible),
                                           "La tarea no esta disponible para envio.")
            Return respuesta
        End If

        Dim tarea As TareaWorkflow = _tareaRepository.ObtenerTarea(contexto, idTarea)
        If tarea Is Nothing OrElse Not tarea.EstaActiva Then
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.TareaNoDisponible,
                                           "La tarea no esta disponible para envio.")
            Return respuesta
        End If
        Dim destinos As ResultadoDestinosEnvioGrupo = _destinosRepository.ObtenerDestinos(contexto, tarea)
        If destinos Is Nothing OrElse Not String.IsNullOrWhiteSpace(destinos.CodigoBloqueo) Then
            respuesta.[Error] = CrearError(If(destinos Is Nothing, CodigosBloqueoPrevisualizacion.TransicionNoDisponible, destinos.CodigoBloqueo),
                                           If(destinos Is Nothing, "No fue posible consultar los destinos de la tarea.", destinos.MensajeFuncional))
            Return respuesta
        End If

        respuesta.Contexto.Radicado = tarea.Radicado
        respuesta.Contexto.ActividadOrigen = tarea.IdActividadOrigen.ToString()
        respuesta.Contexto.GrupoActual = tarea.GrupoActual
        respuesta.TokenVersion = tarea.TokenVersion
        respuesta.Destinos = MapearDestinos(destinos.Destinos)
        If respuesta.Destinos Is Nothing OrElse respuesta.Destinos.Count = 0 Then
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.SinDestinos,
                                           "No hay destinos disponibles para la tarea.")
        End If
        Return respuesta
    End Function

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal solicitud As SolicitudEnvioGrupoWorkflow) As ResultadoEnvioGrupoDto
        Dim cronometro As Stopwatch = Stopwatch.StartNew()
        Try
            Dim habilitacion As HabilitacionWorkflowModernDto = EvaluarHabilitacion(contexto)
            If Not habilitacion.Activo Then
                Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                          CrearBloqueado(habilitacion.Codigo, habilitacion.MensajeFuncional, False, Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If
            If Not TieneCambioRuta(contexto) Then
                Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                          CrearBloqueado(CodigosBloqueoPrevisualizacion.PermisoCambioRutaDenegado,
                                                         "El usuario no tiene permiso para enviar la tarea a otra actividad.", False, Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If

            Dim errorSolicitud As ErrorTransicionDto = _validadorSolicitud.ValidarSolicitud(solicitud)
            If errorSolicitud IsNot Nothing Then
                Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                          CrearBloqueado(errorSolicitud.Codigo, errorSolicitud.MensajeVisible, False, Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If
            If _tareaRepository Is Nothing OrElse _ejecucionRepository Is Nothing OrElse _requisitosRepository Is Nothing OrElse
               _concurrencyGuard Is Nothing OrElse _ejecutor Is Nothing Then
                Return CrearBloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                      "La operacion de envio no esta disponible en este servicio.", True, Nothing)
            End If

            Dim guard As ResultadoGuardTransicion = _concurrencyGuard.Adquirir(contexto, solicitud.IdTarea, solicitud.TokenVersion)
            If guard Is Nothing OrElse Not guard.Adquirido OrElse guard.Lease Is Nothing Then
                Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                          CrearBloqueado(If(guard Is Nothing OrElse String.IsNullOrWhiteSpace(guard.CodigoBloqueo),
                                                             CodigosBloqueoPrevisualizacion.TransicionNoDisponible, guard.CodigoBloqueo),
                                                         If(guard Is Nothing OrElse String.IsNullOrWhiteSpace(guard.MensajeFuncional),
                                                            "No fue posible preparar el envio de la tarea.", guard.MensajeFuncional),
                                                         True, Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If

            Using guard.Lease
                'La autorización se verifica nuevamente dentro del lock junto con el estado actual de la tarea.
                If Not TieneCambioRuta(contexto) Then
                    Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                              CrearBloqueado(CodigosBloqueoPrevisualizacion.PermisoCambioRutaDenegado,
                                                             "El usuario no tiene permiso para enviar la tarea a otra actividad.", False, Nothing),
                                              cronometro.ElapsedMilliseconds)
                End If
                Dim tarea As TareaWorkflow = _tareaRepository.ObtenerTarea(contexto, solicitud.IdTarea)
                If tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
                   Not String.Equals(tarea.TokenVersion, solicitud.TokenVersion, StringComparison.Ordinal) Then
                    Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                              CrearBloqueado(CodigosBloqueoPrevisualizacion.VersionConflicto,
                                                             "La tarea cambio; actualice la informacion antes de enviarla.", False, Nothing),
                                              cronometro.ElapsedMilliseconds)
                End If

                Dim resolucion As ResultadoResolucionEnvioGrupo = _ejecucionRepository.ResolverDestino(contexto, tarea, solicitud.IdActividadDestino)
                If resolucion Is Nothing OrElse Not resolucion.EsValido Then
                    Return RegistrarAuditoria(contexto, tarea, Nothing,
                                              CrearBloqueado(If(resolucion Is Nothing, CodigosBloqueoPrevisualizacion.ActividadDestinoNoDisponible, resolucion.CodigoBloqueo),
                                                             If(resolucion Is Nothing, "El destino seleccionado ya no esta disponible.", resolucion.MensajeFuncional),
                                                             False, Nothing),
                                              cronometro.ElapsedMilliseconds)
                End If

                Dim requisitos As ResultadoRequisitosEnvioGrupo = _requisitosRepository.Evaluar(contexto, tarea, resolucion.Destino)
                If requisitos Is Nothing OrElse Not requisitos.Cumple Then
                    Return RegistrarAuditoria(contexto, tarea, resolucion.Destino,
                                              CrearBloqueado(If(requisitos Is Nothing OrElse String.IsNullOrWhiteSpace(requisitos.CodigoBloqueo),
                                                                 CodigosBloqueoPrevisualizacion.RequisitoNoCumplido, requisitos.CodigoBloqueo),
                                                             If(requisitos Is Nothing OrElse String.IsNullOrWhiteSpace(requisitos.MensajeFuncional),
                                                                "La tarea no cumple los requisitos para enviarse.", requisitos.MensajeFuncional),
                                                             False, If(requisitos Is Nothing, Nothing, requisitos.Requisitos)),
                                              cronometro.ElapsedMilliseconds)
                End If

                Dim ejecucion As ResultadoEjecucionWorkflow = _ejecutor.Ejecutar(contexto, tarea, resolucion.Destino)
                Return RegistrarAuditoria(contexto, tarea, resolucion.Destino,
                                          MapearEjecucion(ejecucion, tarea, resolucion.Destino, requisitos.Requisitos),
                                          cronometro.ElapsedMilliseconds)
            End Using
        Catch
            Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                      CrearBloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                                     "No fue posible enviar la tarea.", True, Nothing),
                                      cronometro.ElapsedMilliseconds)
        End Try
    End Function

    Private Function EvaluarHabilitacion(ByVal contexto As ContextoModuloWorkflow) As HabilitacionWorkflowModernDto
        Dim errorContexto As ErrorTransicionDto = _validadorContexto.ValidarContexto(contexto)
        If errorContexto IsNot Nothing Then
            Return New HabilitacionWorkflowModernDto With {.Estado = "inactivo", .Codigo = errorContexto.Codigo,
                                                            .MensajeFuncional = errorContexto.MensajeVisible, .Activo = False}
        End If
        Return New EvaluadorHabilitacionWorkflowModern(_featureGate).Evaluar(contexto)
    End Function

    Private Shared Function TieneCambioRuta(ByVal contexto As ContextoModuloWorkflow) As Boolean
        Return contexto IsNot Nothing AndAlso contexto.PuedeCambioRuta
    End Function

    Private Function RegistrarAuditoria(ByVal contexto As ContextoModuloWorkflow,
                                        ByVal tarea As TareaWorkflow,
                                        ByVal destino As DestinoEnvioGrupoWorkflow,
                                        ByVal respuesta As ResultadoEnvioGrupoDto,
                                        ByVal duracion As Long) As ResultadoEnvioGrupoDto
        If respuesta Is Nothing OrElse tarea Is Nothing OrElse _auditoriaRepository Is Nothing Then Return respuesta
        Dim referencia As String = "WF-GRP-" & Guid.NewGuid().ToString("N").Substring(0, 16)
        Dim auditoria As New AuditoriaTransicion With {
            .IdTarea = tarea.IdTarea,
            .IdUsuarioWorkflow = If(contexto Is Nothing, 0, contexto.IdUsuarioWorkflow),
            .IdRutaWorkflow = If(tarea.IdRuta > 0, tarea.IdRuta, If(contexto Is Nothing, 0, contexto.IdRutaWorkflow)),
            .IdFlujoTrabajo = 0,
            .IdActividadOrigen = tarea.IdActividadOrigen,
            .IdActividadDestino = If(destino Is Nothing, 0, destino.IdActividadDestino),
            .IdConector = 0,
            .Canal = "MODERNO",
            .Mecanismo = "ASMX_ENVIO_GRUPO",
            .FechaUtc = DateTime.UtcNow,
            .DuracionMilisegundos = Math.Max(0, duracion),
            .Resultado = If(respuesta.Exito, "EXITO", If(String.Equals(respuesta.EstadoFinal, "bloqueado", StringComparison.OrdinalIgnoreCase), "BLOQUEADO", "ERROR")),
            .CodigoFuncional = If(respuesta.Exito, "WORKFLOW_GROUP_SEND_SUCCESS", If(String.IsNullOrWhiteSpace(respuesta.CodigoBloqueo), CodigosBloqueoPrevisualizacion.TransicionNoDisponible, respuesta.CodigoBloqueo)),
            .Referencia = referencia
        }
        Try
            If _auditoriaRepository.Registrar(auditoria) Then
                respuesta.ReferenciaAuditoria = referencia
            Else
                AgregarAdvertenciaAuditoria(respuesta)
            End If
        Catch
            AgregarAdvertenciaAuditoria(respuesta)
        End Try
        Return respuesta
    End Function

    Private Shared Function CrearTareaAuditoria(ByVal contexto As ContextoModuloWorkflow,
                                                ByVal solicitud As SolicitudEnvioGrupoWorkflow) As TareaWorkflow
        Return New TareaWorkflow With {.IdTarea = If(solicitud Is Nothing, 0, solicitud.IdTarea),
                                       .IdRuta = If(contexto Is Nothing, 0, contexto.IdRutaWorkflow)}
    End Function

    Private Shared Function CrearBloqueado(ByVal codigo As String,
                                           ByVal mensaje As String,
                                           ByVal reintentable As Boolean,
                                           ByVal requisitos As IList(Of RequisitoTransicion)) As ResultadoEnvioGrupoDto
        Return New ResultadoEnvioGrupoDto With {
            .Exito = False,
            .EstadoFinal = "bloqueado",
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .EsReintentable = reintentable,
            .Requisitos = MapearRequisitos(requisitos),
            .[Error] = CrearError(codigo, mensaje)
        }
    End Function

    Private Shared Function MapearEjecucion(ByVal ejecucion As ResultadoEjecucionWorkflow,
                                            ByVal tarea As TareaWorkflow,
                                            ByVal destino As DestinoEnvioGrupoWorkflow,
                                            ByVal requisitos As IList(Of RequisitoTransicion)) As ResultadoEnvioGrupoDto
        If ejecucion Is Nothing OrElse Not ejecucion.Exito Then
            Return CrearBloqueado(If(ejecucion Is Nothing OrElse String.IsNullOrWhiteSpace(ejecucion.CodigoBloqueo),
                                     CodigosBloqueoPrevisualizacion.TransicionNoDisponible, ejecucion.CodigoBloqueo),
                                  If(ejecucion Is Nothing OrElse String.IsNullOrWhiteSpace(ejecucion.MensajeFuncional),
                                     "No fue posible enviar la tarea.", ejecucion.MensajeFuncional),
                                  ejecucion IsNot Nothing AndAlso ejecucion.EsReintentable, requisitos)
        End If
        Return New ResultadoEnvioGrupoDto With {
            .Exito = True,
            .EstadoFinal = ejecucion.EstadoFinal,
            .MensajeFuncional = ejecucion.MensajeFuncional,
            .ActividadDestino = destino.NombreActividad,
            .Destino = MapearDestino(destino),
            .TokenVersion = tarea.TokenVersion,
            .EsReintentable = False,
            .Advertencias = If(ejecucion.Advertencias, New List(Of String)()),
            .Requisitos = MapearRequisitos(requisitos)
        }
    End Function

    Private Shared Function MapearDestinos(ByVal destinos As IList(Of DestinoEnvioGrupoWorkflow)) As IList(Of DestinoEnvioGrupoDto)
        Dim resultado As New List(Of DestinoEnvioGrupoDto)()
        If destinos IsNot Nothing Then
            For Each destino As DestinoEnvioGrupoWorkflow In destinos
                resultado.Add(MapearDestino(destino))
            Next
        End If
        Return resultado
    End Function

    Private Shared Function MapearDestino(ByVal destino As DestinoEnvioGrupoWorkflow) As DestinoEnvioGrupoDto
        If destino Is Nothing Then Return Nothing
        Return New DestinoEnvioGrupoDto With {.IdActividadDestino = destino.IdActividadDestino,
                                              .NombreActividad = destino.NombreActividad,
                                              .GrupoDestino = destino.NombreGrupoDestino}
    End Function

    Private Shared Function MapearRequisitos(ByVal requisitos As IList(Of RequisitoTransicion)) As IList(Of RequisitoTransicionDto)
        Dim resultado As New List(Of RequisitoTransicionDto)()
        If requisitos IsNot Nothing Then
            For Each requisito As RequisitoTransicion In requisitos
                resultado.Add(New RequisitoTransicionDto With {.Codigo = requisito.Codigo, .Descripcion = requisito.Descripcion,
                                                               .Obligatorio = requisito.Obligatorio, .Satisfecho = requisito.Satisfecho})
            Next
        End If
        Return resultado
    End Function

    Private Shared Function CrearError(ByVal codigo As String, ByVal mensaje As String) As ErrorTransicionDto
        Return New ErrorTransicionDto With {.Codigo = codigo, .MensajeVisible = mensaje, .ReferenciaTrazabilidad = String.Empty}
    End Function

    Private Shared Sub AgregarAdvertenciaAuditoria(ByVal respuesta As ResultadoEnvioGrupoDto)
        If respuesta Is Nothing Then Return
        If respuesta.Advertencias Is Nothing Then respuesta.Advertencias = New List(Of String)()
        respuesta.Advertencias.Add("No fue posible registrar la trazabilidad adicional de la solicitud.")
    End Sub
End Class
