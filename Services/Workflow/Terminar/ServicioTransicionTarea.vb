Imports System
Imports System.Collections.Generic
Imports System.Diagnostics

'Fachada Application. El navegador aporta intención; esta capa revalida y compone el resultado público.
Public Class ServicioTransicionTarea
    Private ReadOnly _tareaRepository As ITareaWorkflowRepository
    Private ReadOnly _flujoRepository As ITransicionFlujoRepository
    Private ReadOnly _rutaRepository As ITransicionRutaRepository
    Private ReadOnly _ejecucionRepository As ITransicionEjecucionRepository
    Private ReadOnly _requisitosRepository As IRequisitosTransicionRepository
    Private ReadOnly _auditoriaRepository As IAuditoriaTransicionRepository
    Private ReadOnly _concurrencyGuard As ITransicionConcurrencyGuard
    Private ReadOnly _featureGate As IWorkflowModernFeatureGate
    Private ReadOnly _validador As ValidadorTransicionTarea
    Private ReadOnly _ejecutor As EjecutorTransicionTarea

    Public Sub New(ByVal tareaRepository As ITareaWorkflowRepository,
                   ByVal flujoRepository As ITransicionFlujoRepository,
                   ByVal rutaRepository As ITransicionRutaRepository,
                   ByVal ejecucionRepository As ITransicionEjecucionRepository,
                   ByVal requisitosRepository As IRequisitosTransicionRepository,
                   ByVal auditoriaRepository As IAuditoriaTransicionRepository,
                   ByVal concurrencyGuard As ITransicionConcurrencyGuard,
                   ByVal featureGate As IWorkflowModernFeatureGate,
                   ByVal validador As ValidadorTransicionTarea,
                   ByVal ejecutor As EjecutorTransicionTarea)
        _tareaRepository = tareaRepository
        _flujoRepository = flujoRepository
        _rutaRepository = rutaRepository
        _ejecucionRepository = ejecucionRepository
        _requisitosRepository = requisitosRepository
        _auditoriaRepository = auditoriaRepository
        _concurrencyGuard = concurrencyGuard
        _featureGate = featureGate
        _validador = validador
        _ejecutor = ejecutor
    End Sub

    'La previsualizacion permanece libre de escritura, guard y adaptadores legacy.
    Public Sub New(ByVal tareaRepository As ITareaWorkflowRepository,
                   ByVal flujoRepository As ITransicionFlujoRepository,
                   ByVal rutaRepository As ITransicionRutaRepository,
                   ByVal featureGate As IWorkflowModernFeatureGate,
                   ByVal validador As ValidadorTransicionTarea)
        Me.New(tareaRepository,
               flujoRepository,
               rutaRepository,
               Nothing,
               Nothing,
               Nothing,
               Nothing,
               featureGate,
               validador,
               Nothing)
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
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.TareaInvalida, "La tarea seleccionada no es valida.")
            Return respuesta
        End If

        Dim tarea As TareaWorkflow = _tareaRepository.ObtenerTarea(contexto, idTarea)
        If tarea Is Nothing OrElse Not tarea.EstaActiva Then
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.TareaNoDisponible, "La tarea no esta disponible para envio.")
            Return respuesta
        End If

        respuesta.Origen = tarea.IdActividadOrigen.ToString()
        respuesta.TipoDecision = tarea.TipoDecision
        respuesta.Contexto.Radicado = tarea.Radicado
        respuesta.Contexto.ActividadOrigen = tarea.IdActividadOrigen.ToString()
        respuesta.Contexto.GrupoActual = tarea.GrupoActual
        respuesta.RequiereNotificacion = tarea.RequiereNotificacion
        respuesta.TokenVersion = tarea.TokenVersion

        Dim resultadoDestinos As ResultadoPrevisualizacionDestinosDto = Nothing
        If String.Equals(tarea.TipoDecision, "FLUJO", StringComparison.OrdinalIgnoreCase) Then
            resultadoDestinos = New ProveedorTransicionesFlujo(_flujoRepository).Obtener(contexto, tarea)
        ElseIf String.Equals(tarea.TipoDecision, "RUTA", StringComparison.OrdinalIgnoreCase) Then
            resultadoDestinos = New ProveedorTransicionesRuta(_rutaRepository).Obtener(contexto, tarea)
        Else
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.TransicionInconsistente, "No fue posible resolver el destino de la tarea.")
        End If

        If respuesta.[Error] Is Nothing AndAlso resultadoDestinos IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(resultadoDestinos.CodigoBloqueo) Then
            respuesta.[Error] = CrearError(resultadoDestinos.CodigoBloqueo, resultadoDestinos.MensajeFuncional)
        ElseIf respuesta.[Error] Is Nothing AndAlso resultadoDestinos IsNot Nothing Then
            respuesta.Destinos = resultadoDestinos.Destinos
        End If
        If respuesta.[Error] Is Nothing AndAlso (respuesta.Destinos Is Nothing OrElse respuesta.Destinos.Count = 0) Then
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.SinDestinos, "No hay destinos disponibles para la tarea.")
        End If
        Return respuesta
    End Function

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal solicitud As SolicitudTransicionWorkflow) As ResultadoTransicionDto
        Dim cronometro As Stopwatch = Stopwatch.StartNew()
        Try
            Dim habilitacion As HabilitacionWorkflowModernDto = EvaluarHabilitacion(contexto)
            If Not habilitacion.Activo Then
                Return RegistrarAuditoria(contexto,
                                          CrearTareaAuditoria(contexto, solicitud),
                                          Nothing,
                                          CrearResultadoBloqueado(habilitacion.Codigo, habilitacion.MensajeFuncional, False, Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If

            Dim errorSolicitud As ErrorTransicionDto = _validador.ValidarSolicitud(solicitud)
            If errorSolicitud IsNot Nothing Then
                Return RegistrarAuditoria(contexto,
                                          CrearTareaAuditoria(contexto, solicitud),
                                          Nothing,
                                          CrearResultadoBloqueado(errorSolicitud.Codigo, errorSolicitud.MensajeVisible, False, Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If
            If _tareaRepository Is Nothing OrElse _ejecucionRepository Is Nothing OrElse _requisitosRepository Is Nothing OrElse
               _concurrencyGuard Is Nothing OrElse _ejecutor Is Nothing Then
                Return CrearResultadoBloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                               "La operacion de envio no esta disponible en este servicio.",
                                               True,
                                               Nothing)
            End If

            Dim guard As ResultadoGuardTransicion = _concurrencyGuard.Adquirir(contexto, solicitud.IdTarea, solicitud.TokenVersion)
            If guard Is Nothing OrElse Not guard.Adquirido OrElse guard.Lease Is Nothing Then
                Return RegistrarAuditoria(contexto,
                                          CrearTareaAuditoria(contexto, solicitud),
                                          Nothing,
                                          CrearResultadoBloqueado(If(guard Is Nothing OrElse String.IsNullOrWhiteSpace(guard.CodigoBloqueo),
                                                                      CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                                                      guard.CodigoBloqueo),
                                                                   If(guard Is Nothing OrElse String.IsNullOrWhiteSpace(guard.MensajeFuncional),
                                                                      "No fue posible preparar el envio de la tarea.",
                                                                      guard.MensajeFuncional),
                                                                   True,
                                                                   Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If

            Using guard.Lease
                Dim tarea As TareaWorkflow = _tareaRepository.ObtenerTarea(contexto, solicitud.IdTarea)
                If tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
                   Not String.Equals(tarea.TokenVersion, solicitud.TokenVersion, StringComparison.Ordinal) Then
                    Return RegistrarAuditoria(contexto,
                                              CrearTareaAuditoria(contexto, solicitud),
                                              Nothing,
                                              CrearResultadoBloqueado(CodigosBloqueoPrevisualizacion.VersionConflicto,
                                                                     "La tarea cambio; actualice la informacion antes de enviarla.",
                                                                     False,
                                                                     Nothing),
                                              cronometro.ElapsedMilliseconds)
                End If

                Dim resolucion As ResultadoResolucionDestinoTransicion = _ejecucionRepository.ResolverDestino(contexto, tarea, solicitud.IdConector)
                If resolucion Is Nothing OrElse Not resolucion.EsValido Then
                    Dim codigo As String = If(resolucion Is Nothing OrElse String.IsNullOrWhiteSpace(resolucion.CodigoBloqueo),
                                              CodigosBloqueoPrevisualizacion.ConectorNoDisponible,
                                              resolucion.CodigoBloqueo)
                    Dim mensaje As String = If(resolucion Is Nothing OrElse String.IsNullOrWhiteSpace(resolucion.MensajeFuncional),
                                               "El destino seleccionado ya no esta disponible.",
                                               resolucion.MensajeFuncional)
                    Return RegistrarAuditoria(contexto,
                                              tarea,
                                              Nothing,
                                              CrearResultadoBloqueado(codigo, mensaje, False, Nothing),
                                              cronometro.ElapsedMilliseconds)
                End If

                Dim requisitos As ResultadoRequisitosTransicion = _requisitosRepository.Evaluar(contexto, tarea, resolucion.Destino)
                If requisitos Is Nothing OrElse Not requisitos.Cumple Then
                    Dim codigo As String = If(requisitos Is Nothing OrElse String.IsNullOrWhiteSpace(requisitos.CodigoBloqueo),
                                              CodigosBloqueoPrevisualizacion.RequisitoNoCumplido,
                                              requisitos.CodigoBloqueo)
                    Dim mensaje As String = If(requisitos Is Nothing OrElse String.IsNullOrWhiteSpace(requisitos.MensajeFuncional),
                                               "La tarea no cumple los requisitos para enviarse.",
                                               requisitos.MensajeFuncional)
                    Return RegistrarAuditoria(contexto,
                                              tarea,
                                              resolucion.Destino,
                                              CrearResultadoBloqueado(codigo, mensaje, False, If(requisitos Is Nothing, Nothing, requisitos.Requisitos)),
                                              cronometro.ElapsedMilliseconds)
                End If

                Dim ejecucion As ResultadoEjecucionWorkflow = _ejecutor.Ejecutar(contexto, tarea, resolucion.Destino)
                Dim respuesta As ResultadoTransicionDto = MapearEjecucion(ejecucion, tarea, resolucion.Destino, requisitos.Requisitos)
                Return RegistrarAuditoria(contexto, tarea, resolucion.Destino, respuesta, cronometro.ElapsedMilliseconds)
            End Using
        Catch
            Return RegistrarAuditoria(contexto,
                                      CrearTareaAuditoria(contexto, solicitud),
                                      Nothing,
                                      CrearResultadoBloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                                             "No fue posible enviar la tarea.",
                                                             True,
                                                             Nothing),
                                      cronometro.ElapsedMilliseconds)
        End Try
    End Function

    Private Function RegistrarAuditoria(ByVal contexto As ContextoModuloWorkflow,
                                        ByVal tarea As TareaWorkflow,
                                        ByVal destino As DestinoEjecucionWorkflow,
                                        ByVal respuesta As ResultadoTransicionDto,
                                        ByVal duracionMilisegundos As Long) As ResultadoTransicionDto
        If respuesta Is Nothing OrElse tarea Is Nothing OrElse _auditoriaRepository Is Nothing Then Return respuesta

        Dim referencia As String = "WF-MOD-" & Guid.NewGuid().ToString("N").Substring(0, 16)
        Dim auditoria As New AuditoriaTransicion With {
            .IdTarea = tarea.IdTarea,
            .IdUsuarioWorkflow = If(contexto Is Nothing, 0, contexto.IdUsuarioWorkflow),
            .IdRutaWorkflow = If(tarea.IdRuta > 0, tarea.IdRuta, If(contexto Is Nothing, 0, contexto.IdRutaWorkflow)),
            .IdFlujoTrabajo = tarea.IdFlujoTrabajo,
            .IdActividadOrigen = tarea.IdActividadOrigen,
            .IdActividadDestino = If(destino Is Nothing, 0, destino.IdActividadDestino),
            .IdConector = If(destino Is Nothing, 0, destino.IdConector),
            .Canal = "MODERNO",
            .Mecanismo = "ASMX_MODERNO",
            .FechaUtc = DateTime.UtcNow,
            .DuracionMilisegundos = Math.Max(0, duracionMilisegundos),
            .Resultado = ResolverResultadoAuditoria(respuesta),
            .CodigoFuncional = ResolverCodigoAuditoria(respuesta),
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
                                                ByVal solicitud As SolicitudTransicionWorkflow) As TareaWorkflow
        Return New TareaWorkflow With {
            .IdTarea = If(solicitud Is Nothing, 0, solicitud.IdTarea),
            .IdRuta = If(contexto Is Nothing, 0, contexto.IdRutaWorkflow)
        }
    End Function

    Private Shared Function ResolverResultadoAuditoria(ByVal respuesta As ResultadoTransicionDto) As String
        If respuesta IsNot Nothing AndAlso respuesta.Exito Then Return "EXITO"
        If respuesta IsNot Nothing AndAlso String.Equals(respuesta.EstadoFinal, "bloqueado", StringComparison.OrdinalIgnoreCase) Then
            Return "BLOQUEADO"
        End If
        Return "ERROR"
    End Function

    Private Shared Function ResolverCodigoAuditoria(ByVal respuesta As ResultadoTransicionDto) As String
        If respuesta IsNot Nothing AndAlso respuesta.Exito Then Return "WORKFLOW_MODERN_SUCCESS"
        If respuesta IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(respuesta.CodigoBloqueo) Then
            Return respuesta.CodigoBloqueo
        End If
        Return CodigosBloqueoPrevisualizacion.TransicionNoDisponible
    End Function

    Private Shared Sub AgregarAdvertenciaAuditoria(ByVal respuesta As ResultadoTransicionDto)
        If respuesta Is Nothing Then Return
        If respuesta.Advertencias Is Nothing Then respuesta.Advertencias = New List(Of String)()
        respuesta.Advertencias.Add("No fue posible registrar la trazabilidad adicional de la solicitud.")
    End Sub

    Private Shared Function MapearEjecucion(ByVal ejecucion As ResultadoEjecucionWorkflow,
                                            ByVal tarea As TareaWorkflow,
                                            ByVal destino As DestinoEjecucionWorkflow,
                                            ByVal requisitos As IList(Of RequisitoTransicion)) As ResultadoTransicionDto
        If ejecucion Is Nothing Then
            Return CrearResultadoBloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                           "No fue posible enviar la tarea.",
                                           True,
                                           requisitos)
        End If

        Dim respuesta As New ResultadoTransicionDto With {
            .Exito = ejecucion.Exito,
            .EstadoFinal = ejecucion.EstadoFinal,
            .CodigoBloqueo = ejecucion.CodigoBloqueo,
            .MensajeFuncional = ejecucion.MensajeFuncional,
            .EsReintentable = ejecucion.EsReintentable,
            .TokenVersion = tarea.TokenVersion,
            .ActividadDestino = If(destino Is Nothing, String.Empty, destino.NombreActividadDestino),
            .Destino = CrearDestinoDto(destino),
            .Requisitos = MapearRequisitos(requisitos)
        }
        If ejecucion.Advertencias IsNot Nothing Then
            For Each advertencia As String In ejecucion.Advertencias
                respuesta.Advertencias.Add(advertencia)
            Next
        End If
        If Not respuesta.Exito Then respuesta.[Error] = CrearError(respuesta.CodigoBloqueo, respuesta.MensajeFuncional)
        Return respuesta
    End Function

    Private Shared Function CrearResultadoBloqueado(ByVal codigo As String,
                                                    ByVal mensaje As String,
                                                    ByVal reintentable As Boolean,
                                                    ByVal requisitos As IList(Of RequisitoTransicion)) As ResultadoTransicionDto
        Return New ResultadoTransicionDto With {
            .Exito = False,
            .EstadoFinal = "bloqueado",
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .[Error] = CrearError(codigo, mensaje),
            .EsReintentable = reintentable,
            .Requisitos = MapearRequisitos(requisitos)
        }
    End Function

    Private Shared Function CrearDestinoDto(ByVal destino As DestinoEjecucionWorkflow) As DestinoTransicionDto
        If destino Is Nothing Then Return Nothing
        Return New DestinoTransicionDto With {
            .Id = destino.IdConector,
            .IdActividadDestino = destino.IdActividadDestino,
            .Nombre = destino.NombreActividadDestino,
            .Destinatario = destino.NombreDestinatario,
            .Grupo = destino.NombreGrupoDestino,
            .Tipo = destino.TipoTransicion
        }
    End Function

    Private Shared Function MapearRequisitos(ByVal requisitos As IList(Of RequisitoTransicion)) As IList(Of RequisitoTransicionDto)
        Dim resultado As New List(Of RequisitoTransicionDto)()
        If requisitos Is Nothing Then Return resultado
        For Each requisito As RequisitoTransicion In requisitos
            If requisito Is Nothing Then Continue For
            resultado.Add(New RequisitoTransicionDto With {
                .Codigo = requisito.Codigo,
                .Descripcion = requisito.Descripcion,
                .Obligatorio = requisito.Obligatorio,
                .Satisfecho = requisito.Satisfecho
            })
        Next
        Return resultado
    End Function

    Private Shared Function CrearError(ByVal codigo As String, ByVal mensaje As String) As ErrorTransicionDto
        Return New ErrorTransicionDto With {.Codigo = codigo, .MensajeVisible = mensaje, .ReferenciaTrazabilidad = String.Empty}
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
                .Codigo = CodigosBloqueoPrevisualizacion.PoliticaModernaNoDisponible,
                .MensajeFuncional = "No fue posible validar la política de la experiencia moderna.",
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

    Public Function Obtener(ByVal contexto As ContextoModuloWorkflow, ByVal tarea As TareaWorkflow) As ResultadoPrevisualizacionDestinosDto
        Return Mapear(_repository.ObtenerDestinos(contexto, tarea))
    End Function

    Friend Shared Function Mapear(ByVal resultadoRepositorio As ResultadoDestinosTransicion) As ResultadoPrevisualizacionDestinosDto
        Dim resultado As New ResultadoPrevisualizacionDestinosDto()
        If resultadoRepositorio Is Nothing Then
            resultado.CodigoBloqueo = CodigosBloqueoPrevisualizacion.TransicionInconsistente
            resultado.MensajeFuncional = "No fue posible resolver el destino de la tarea."
            Return resultado
        End If
        resultado.CodigoBloqueo = resultadoRepositorio.CodigoBloqueo
        resultado.MensajeFuncional = resultadoRepositorio.MensajeFuncional
        If resultadoRepositorio.Destinos Is Nothing Then Return resultado

        For Each destino As DestinoTransicion In resultadoRepositorio.Destinos
            resultado.Destinos.Add(New DestinoTransicionDto With {
                .Id = destino.IdConector,
                .IdActividadDestino = destino.IdActividadDestino,
                .Nombre = destino.Nombre,
                .Destinatario = destino.NombreDestinatario,
                .Grupo = destino.NombreGrupo,
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

    Public Function Obtener(ByVal contexto As ContextoModuloWorkflow, ByVal tarea As TareaWorkflow) As ResultadoPrevisualizacionDestinosDto
        Return ProveedorTransicionesFlujo.Mapear(_repository.ObtenerDestinos(contexto, tarea))
    End Function
End Class

Public Class EjecutorTransicionTarea
    Private ReadOnly _legacyExecutor As IWorkflowLegacyExecutor

    Public Sub New(ByVal legacyExecutor As IWorkflowLegacyExecutor)
        _legacyExecutor = legacyExecutor
    End Sub

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaWorkflow,
                             ByVal destino As DestinoEjecucionWorkflow) As ResultadoEjecucionWorkflow
        If _legacyExecutor Is Nothing Then
            Return New ResultadoEjecucionWorkflow With {
                .Exito = False,
                .EstadoFinal = "bloqueado",
                .CodigoBloqueo = CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                .MensajeFuncional = "La operacion de envio no esta disponible.",
                .EsReintentable = True
            }
        End If
        Return _legacyExecutor.Ejecutar(contexto, tarea, destino)
    End Function
End Class
