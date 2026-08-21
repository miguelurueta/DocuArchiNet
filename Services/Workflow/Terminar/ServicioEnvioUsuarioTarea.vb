Imports System
Imports System.Collections.Generic
Imports System.Diagnostics

'Aplicación exclusiva de Enviar a usuario: preview de solo lectura y ejecución aislada.
Public Class ServicioEnvioUsuarioTarea
    Private ReadOnly _tareaRepository As ITareaWorkflowRepository
    Private ReadOnly _busquedaRepository As IEnvioUsuarioBusquedaRepository
    Private ReadOnly _ejecucionRepository As IEnvioUsuarioEjecucionRepository
    Private ReadOnly _requisitosRepository As IEnvioUsuarioRequisitosRepository
    Private ReadOnly _autorizacionRepository As IEnvioUsuarioAutorizacionRepository
    Private ReadOnly _auditoriaRepository As IAuditoriaTransicionRepository
    Private ReadOnly _concurrencyGuard As ITransicionConcurrencyGuard
    Private ReadOnly _ejecutor As IEnvioUsuarioLegacyExecutor
    Private ReadOnly _validadorSolicitud As ValidadorEnvioUsuarioTarea
    Private ReadOnly _validadorContexto As ValidadorTransicionTarea

    Public Sub New(ByVal tareaRepository As ITareaWorkflowRepository,
                   ByVal busquedaRepository As IEnvioUsuarioBusquedaRepository,
                   ByVal validadorSolicitud As ValidadorEnvioUsuarioTarea)
        Me.New(tareaRepository, busquedaRepository, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, validadorSolicitud)
    End Sub

    Public Sub New(ByVal tareaRepository As ITareaWorkflowRepository,
                   ByVal busquedaRepository As IEnvioUsuarioBusquedaRepository,
                   ByVal ejecucionRepository As IEnvioUsuarioEjecucionRepository,
                   ByVal requisitosRepository As IEnvioUsuarioRequisitosRepository,
                   ByVal autorizacionRepository As IEnvioUsuarioAutorizacionRepository,
                   ByVal auditoriaRepository As IAuditoriaTransicionRepository,
                   ByVal concurrencyGuard As ITransicionConcurrencyGuard,
                   ByVal ejecutor As IEnvioUsuarioLegacyExecutor,
                   ByVal validadorSolicitud As ValidadorEnvioUsuarioTarea)
        _tareaRepository = tareaRepository
        _busquedaRepository = busquedaRepository
        _ejecucionRepository = ejecucionRepository
        _requisitosRepository = requisitosRepository
        _autorizacionRepository = autorizacionRepository
        _auditoriaRepository = auditoriaRepository
        _concurrencyGuard = concurrencyGuard
        _ejecutor = ejecutor
        _validadorSolicitud = If(validadorSolicitud, New ValidadorEnvioUsuarioTarea())
        _validadorContexto = New ValidadorTransicionTarea()
    End Sub

    Public Function Previsualizar(ByVal contexto As ContextoModuloWorkflow,
                                  ByVal solicitud As SolicitudPreviewEnvioUsuario) As PrevisualizacionEnvioUsuarioDto
        Dim normalizada As SolicitudPreviewEnvioUsuario = Nothing
        Dim respuesta As New PrevisualizacionEnvioUsuarioDto With {
            .IdTarea = If(solicitud Is Nothing, 0, solicitud.IdTarea)
        }

        Dim errorContexto As ErrorTransicionDto = _validadorContexto.ValidarContexto(contexto)
        If errorContexto IsNot Nothing Then
            respuesta.[Error] = errorContexto
            Return respuesta
        End If
        If Not TieneCambioUsuario(contexto) Then
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.PermisoCambioUsuarioDenegado,
                                           "El usuario no tiene permiso para enviar la tarea a otro usuario.")
            Return respuesta
        End If

        Dim errorSolicitud As ErrorTransicionDto = _validadorSolicitud.NormalizarPreview(solicitud, normalizada)
        If errorSolicitud IsNot Nothing Then
            respuesta.[Error] = errorSolicitud
            Return respuesta
        End If
        respuesta.IdTarea = normalizada.IdTarea
        respuesta.TamanoPagina = normalizada.TamanoPagina

        If _tareaRepository Is Nothing OrElse _busquedaRepository Is Nothing Then
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                           "La consulta de destinos no esta disponible.")
            Return respuesta
        End If

        Try
            Dim tarea As TareaWorkflow = _tareaRepository.ObtenerTarea(contexto, normalizada.IdTarea)
            If tarea Is Nothing OrElse Not tarea.EstaActiva Then
                respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.TareaNoDisponible,
                                               "La tarea no esta disponible para envio.")
                Return respuesta
            End If

            Dim destinos As ResultadoBusquedaDestinosEnvioUsuario = _busquedaRepository.BuscarDestinos(contexto, tarea, normalizada)
            If destinos Is Nothing OrElse Not String.IsNullOrWhiteSpace(destinos.CodigoBloqueo) Then
                respuesta.[Error] = CrearError(
                    If(destinos Is Nothing, CodigosBloqueoPrevisualizacion.TransicionNoDisponible, destinos.CodigoBloqueo),
                    If(destinos Is Nothing, "No fue posible consultar los destinos de la tarea.", destinos.MensajeFuncional))
                Return respuesta
            End If

            respuesta.Contexto.Radicado = tarea.Radicado
            respuesta.Contexto.ActividadOrigen = tarea.IdActividadOrigen.ToString()
            respuesta.Contexto.GrupoActual = tarea.GrupoActual
            respuesta.TokenVersion = tarea.TokenVersion
            respuesta.TamanoPagina = destinos.TamanoPagina
            respuesta.TieneMas = destinos.TieneMas
            respuesta.CursorSiguiente = destinos.CursorSiguiente
            respuesta.Destinos = MapearDestinos(destinos.Destinos)
            If respuesta.Destinos.Count = 0 Then
                respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.SinDestinos,
                                               "No hay usuarios disponibles para la tarea.")
            End If
            Return respuesta
        Catch
            respuesta.[Error] = CrearError(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                           "No fue posible consultar los destinos de la tarea.")
            Return respuesta
        End Try
    End Function

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal solicitud As SolicitudEnvioUsuarioWorkflow) As ResultadoEnvioUsuarioDto
        Dim cronometro As Stopwatch = Stopwatch.StartNew()
        Try
            Dim errorContexto As ErrorTransicionDto = _validadorContexto.ValidarContexto(contexto)
            If errorContexto IsNot Nothing Then
                Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                          CrearBloqueado(errorContexto.Codigo, errorContexto.MensajeVisible, False, Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If
            If Not TieneCambioUsuario(contexto) Then
                Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                          CrearBloqueado(CodigosBloqueoPrevisualizacion.PermisoCambioUsuarioDenegado,
                                                         "El usuario no tiene permiso para enviar la tarea a otro usuario.", False, Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If

            Dim errorSolicitud As ErrorTransicionDto = _validadorSolicitud.ValidarEjecucion(solicitud)
            If errorSolicitud IsNot Nothing Then
                Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                          CrearBloqueado(errorSolicitud.Codigo, errorSolicitud.MensajeVisible, False, Nothing),
                                          cronometro.ElapsedMilliseconds)
            End If
            If _tareaRepository Is Nothing OrElse _ejecucionRepository Is Nothing OrElse _requisitosRepository Is Nothing OrElse
               _autorizacionRepository Is Nothing OrElse _concurrencyGuard Is Nothing OrElse _ejecutor Is Nothing Then
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
                If Not _autorizacionRepository.TieneCambioUsuario(contexto) Then
                    Return RegistrarAuditoria(contexto, CrearTareaAuditoria(contexto, solicitud), Nothing,
                                              CrearBloqueado(CodigosBloqueoPrevisualizacion.PermisoCambioUsuarioDenegado,
                                                             "El usuario no tiene permiso para enviar la tarea a otro usuario.", False, Nothing),
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

                Dim resolucion As ResultadoResolucionEnvioUsuario = _ejecucionRepository.ResolverDestino(
                    contexto, tarea, solicitud.IdUsuarioWorkflowDestino, solicitud.IdActividadDestino)
                If resolucion Is Nothing OrElse Not resolucion.EsValido Then
                    Return RegistrarAuditoria(contexto, tarea, Nothing,
                                              CrearBloqueado(If(resolucion Is Nothing, CodigosBloqueoPrevisualizacion.UsuarioDestinoNoDisponible, resolucion.CodigoBloqueo),
                                                             If(resolucion Is Nothing, "El destino seleccionado ya no esta disponible.", resolucion.MensajeFuncional),
                                                             False, Nothing),
                                              cronometro.ElapsedMilliseconds)
                End If

                Dim requisitos As ResultadoRequisitosEnvioUsuario = _requisitosRepository.Evaluar(contexto, tarea, resolucion.Destino)
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

    Private Shared Function TieneCambioUsuario(ByVal contexto As ContextoModuloWorkflow) As Boolean
        Return contexto IsNot Nothing AndAlso contexto.PuedeCambioUsuario
    End Function

    Private Function RegistrarAuditoria(ByVal contexto As ContextoModuloWorkflow,
                                        ByVal tarea As TareaWorkflow,
                                        ByVal destino As DestinoEnvioUsuarioWorkflow,
                                        ByVal respuesta As ResultadoEnvioUsuarioDto,
                                        ByVal duracion As Long) As ResultadoEnvioUsuarioDto
        If respuesta Is Nothing OrElse tarea Is Nothing OrElse _auditoriaRepository Is Nothing Then Return respuesta
        Dim referencia As String = "WF-USR-" & Guid.NewGuid().ToString("N").Substring(0, 16)
        Dim auditoria As New AuditoriaTransicion With {
            .IdTarea = tarea.IdTarea,
            .IdUsuarioWorkflow = If(contexto Is Nothing, 0, contexto.IdUsuarioWorkflow),
            .IdRutaWorkflow = If(tarea.IdRuta > 0, tarea.IdRuta, If(contexto Is Nothing, 0, contexto.IdRutaWorkflow)),
            .IdFlujoTrabajo = tarea.IdFlujoTrabajo,
            .IdActividadOrigen = tarea.IdActividadOrigen,
            .IdActividadDestino = If(destino Is Nothing, 0, destino.IdActividadDestino),
            .IdConector = 0,
            .Canal = "MODERNO",
            .Mecanismo = "ASMX_ENVIO_USUARIO",
            .FechaUtc = DateTime.UtcNow,
            .DuracionMilisegundos = Math.Max(0, duracion),
            .Resultado = If(respuesta.Exito, "EXITO", If(String.Equals(respuesta.EstadoFinal, "bloqueado", StringComparison.OrdinalIgnoreCase), "BLOQUEADO", "ERROR")),
            .CodigoFuncional = If(respuesta.Exito, "WORKFLOW_USER_SEND_SUCCESS", If(String.IsNullOrWhiteSpace(respuesta.CodigoBloqueo), CodigosBloqueoPrevisualizacion.TransicionNoDisponible, respuesta.CodigoBloqueo)),
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
                                                ByVal solicitud As SolicitudEnvioUsuarioWorkflow) As TareaWorkflow
        Return New TareaWorkflow With {
            .IdTarea = If(solicitud Is Nothing, 0, solicitud.IdTarea),
            .IdRuta = If(contexto Is Nothing, 0, contexto.IdRutaWorkflow)
        }
    End Function

    Private Shared Function CrearBloqueado(ByVal codigo As String,
                                           ByVal mensaje As String,
                                           ByVal reintentable As Boolean,
                                           ByVal requisitos As IList(Of RequisitoTransicion)) As ResultadoEnvioUsuarioDto
        Return New ResultadoEnvioUsuarioDto With {
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
                                            ByVal destino As DestinoEnvioUsuarioWorkflow,
                                            ByVal requisitos As IList(Of RequisitoTransicion)) As ResultadoEnvioUsuarioDto
        If ejecucion Is Nothing OrElse Not ejecucion.Exito Then
            Return CrearBloqueado(If(ejecucion Is Nothing OrElse String.IsNullOrWhiteSpace(ejecucion.CodigoBloqueo),
                                     CodigosBloqueoPrevisualizacion.TransicionNoDisponible, ejecucion.CodigoBloqueo),
                                  If(ejecucion Is Nothing OrElse String.IsNullOrWhiteSpace(ejecucion.MensajeFuncional),
                                     "No fue posible enviar la tarea.", ejecucion.MensajeFuncional),
                                  ejecucion IsNot Nothing AndAlso ejecucion.EsReintentable, requisitos)
        End If
        Return New ResultadoEnvioUsuarioDto With {
            .Exito = True,
            .EstadoFinal = ejecucion.EstadoFinal,
            .MensajeFuncional = ejecucion.MensajeFuncional,
            .Destino = MapearDestino(destino),
            .TokenVersion = tarea.TokenVersion,
            .EsReintentable = False,
            .Advertencias = If(ejecucion.Advertencias, New List(Of String)()),
            .Requisitos = MapearRequisitos(requisitos)
        }
    End Function

    Private Shared Function MapearDestinos(ByVal destinos As IList(Of DestinoEnvioUsuarioWorkflow)) As IList(Of DestinoEnvioUsuarioDto)
        Dim resultado As New List(Of DestinoEnvioUsuarioDto)()
        For Each destino As DestinoEnvioUsuarioWorkflow In If(destinos, New List(Of DestinoEnvioUsuarioWorkflow)())
            If destino Is Nothing Then Continue For
            resultado.Add(New DestinoEnvioUsuarioDto With {
                .IdUsuarioWorkflowDestino = destino.IdUsuarioWorkflowDestino,
                .IdActividadDestino = destino.IdActividadDestino,
                .NombreUsuarioDestino = destino.NombreUsuarioDestino,
                .CargoUsuarioDestino = destino.CargoUsuarioDestino,
                .NombreActividadDestino = destino.NombreActividadDestino
            })
        Next
        Return resultado
    End Function

    Private Shared Function MapearDestino(ByVal destino As DestinoEnvioUsuarioWorkflow) As DestinoEnvioUsuarioDto
        If destino Is Nothing Then Return Nothing
        Return New DestinoEnvioUsuarioDto With {
            .IdUsuarioWorkflowDestino = destino.IdUsuarioWorkflowDestino,
            .IdActividadDestino = destino.IdActividadDestino,
            .NombreUsuarioDestino = destino.NombreUsuarioDestino,
            .CargoUsuarioDestino = destino.CargoUsuarioDestino,
            .NombreActividadDestino = destino.NombreActividadDestino
        }
    End Function

    Private Shared Function MapearRequisitos(ByVal requisitos As IList(Of RequisitoTransicion)) As IList(Of RequisitoTransicionDto)
        Dim resultado As New List(Of RequisitoTransicionDto)()
        If requisitos IsNot Nothing Then
            For Each requisito As RequisitoTransicion In requisitos
                If requisito Is Nothing Then Continue For
                resultado.Add(New RequisitoTransicionDto With {
                    .Codigo = requisito.Codigo,
                    .Descripcion = requisito.Descripcion,
                    .Obligatorio = requisito.Obligatorio,
                    .Satisfecho = requisito.Satisfecho
                })
            Next
        End If
        Return resultado
    End Function

    Private Shared Function CrearError(ByVal codigo As String, ByVal mensaje As String) As ErrorTransicionDto
        Return New ErrorTransicionDto With {
            .Codigo = codigo,
            .MensajeVisible = mensaje,
            .ReferenciaTrazabilidad = String.Empty
        }
    End Function

    Private Shared Sub AgregarAdvertenciaAuditoria(ByVal respuesta As ResultadoEnvioUsuarioDto)
        If respuesta Is Nothing Then Return
        If respuesta.Advertencias Is Nothing Then respuesta.Advertencias = New List(Of String)()
        respuesta.Advertencias.Add("No fue posible registrar la trazabilidad adicional de la solicitud.")
    End Sub
End Class
