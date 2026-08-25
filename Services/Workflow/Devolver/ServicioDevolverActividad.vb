Imports System
Imports System.Collections.Generic
Imports System.Diagnostics

'Caso de uso exclusivo de devolución. El preview solo compone lecturas autorizadas; la ejecución se incorpora en esta misma capacidad.
Public Class ServicioDevolverActividad
    Private Const TamanoPaginaPredeterminado As Integer = 25
    Private Const TamanoPaginaMaximo As Integer = 50
    Private Const LongitudMinimaTermino As Integer = 2
    Private Const LongitudMaximaTermino As Integer = 80

    Private ReadOnly _tareaRepository As IDevolverActividadTareaRepository
    Private ReadOnly _autorizacionRepository As IDevolverActividadAutorizacionRepository
    Private ReadOnly _previewRepository As IDevolverActividadPreviewRepository
    Private ReadOnly _ejecucionRepository As IDevolverActividadEjecucionRepository
    Private ReadOnly _cursorCodec As IDevolverActividadCursorCodec
    Private ReadOnly _concurrencyGuard As IDevolverActividadConcurrencyGuard
    Private ReadOnly _ejecutor As IDevolverActividadLegacyExecutor
    Private ReadOnly _auditoriaRepository As IDevolverActividadAuditoriaRepository

    Public Sub New(ByVal tareaRepository As IDevolverActividadTareaRepository,
                   ByVal autorizacionRepository As IDevolverActividadAutorizacionRepository,
                   ByVal previewRepository As IDevolverActividadPreviewRepository,
                   ByVal cursorCodec As IDevolverActividadCursorCodec)
        Me.New(tareaRepository, autorizacionRepository, previewRepository, Nothing, cursorCodec, Nothing, Nothing, Nothing)
    End Sub

    Public Sub New(ByVal tareaRepository As IDevolverActividadTareaRepository,
                   ByVal autorizacionRepository As IDevolverActividadAutorizacionRepository,
                   ByVal previewRepository As IDevolverActividadPreviewRepository,
                   ByVal ejecucionRepository As IDevolverActividadEjecucionRepository,
                   ByVal cursorCodec As IDevolverActividadCursorCodec,
                   ByVal concurrencyGuard As IDevolverActividadConcurrencyGuard,
                   ByVal ejecutor As IDevolverActividadLegacyExecutor,
                   ByVal auditoriaRepository As IDevolverActividadAuditoriaRepository)
        _tareaRepository = tareaRepository
        _autorizacionRepository = autorizacionRepository
        _previewRepository = previewRepository
        _ejecucionRepository = ejecucionRepository
        _cursorCodec = cursorCodec
        _concurrencyGuard = concurrencyGuard
        _ejecutor = ejecutor
        _auditoriaRepository = auditoriaRepository
    End Sub

    Public Function Previsualizar(ByVal contexto As ContextoModuloWorkflow,
                                  ByVal solicitud As SolicitudPreviewDevolverActividad) As PrevisualizacionDevolverActividadDto
        Dim respuesta As New PrevisualizacionDevolverActividadDto With {
            .IdTarea = If(solicitud Is Nothing, 0L, solicitud.IdTarea),
            .TamanoPagina = TamanoPaginaPredeterminado
        }
        Dim terminoNormalizado As String = String.Empty
        Dim errorNormalizacion As ErrorDevolverActividadDto = NormalizarSolicitud(solicitud, terminoNormalizado)
        If errorNormalizacion IsNot Nothing Then
            respuesta.[Error] = errorNormalizacion
            Return respuesta
        End If
        respuesta.IdTarea = solicitud.IdTarea
        respuesta.TamanoPagina = solicitud.TamanoPagina

        If contexto Is Nothing OrElse Not contexto.EsValido() Then
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverActividad.ContextoInvalido,
                                            "No fue posible validar la sesión de la tarea.")
            Return respuesta
        End If
        If Not contexto.PuedeDevolverActividad Then
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverActividad.PermisoDenegado,
                                            "El usuario no tiene permiso para devolver la tarea.")
            Return respuesta
        End If
        If _tareaRepository Is Nothing OrElse _autorizacionRepository Is Nothing OrElse
           _previewRepository Is Nothing OrElse _cursorCodec Is Nothing Then
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverActividad.NoDisponible,
                                            "La devolución de tarea no está disponible.")
            Return respuesta
        End If

        Dim tarea As TareaDevolverActividad = _tareaRepository.ObtenerTarea(contexto, solicitud.IdTarea)
        If tarea Is Nothing OrElse Not tarea.EstaActiva Then
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverActividad.TareaNoDisponible,
                                            "La tarea no está disponible para devolución.")
            Return respuesta
        End If
        Dim autorizacion As ResultadoAutorizacionDevolverActividad = _autorizacionRepository.Evaluar(contexto, tarea)
        If autorizacion Is Nothing OrElse Not autorizacion.Autorizado Then
            respuesta.[Error] = CrearError(If(autorizacion Is Nothing OrElse String.IsNullOrWhiteSpace(autorizacion.CodigoBloqueo),
                                               CodigosBloqueoDevolverActividad.PermisoDenegado, autorizacion.CodigoBloqueo),
                                            If(autorizacion Is Nothing OrElse String.IsNullOrWhiteSpace(autorizacion.MensajeFuncional),
                                               "El usuario no tiene permiso para devolver la tarea.", autorizacion.MensajeFuncional))
            Return respuesta
        End If

        If Not String.IsNullOrWhiteSpace(solicitud.Cursor) AndAlso
           Not _cursorCodec.Validar(contexto, tarea, terminoNormalizado, solicitud.Cursor,
                                    solicitud.IdConectorDespuesDe, solicitud.OrdenDespuesDe) Then
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverActividad.CursorInvalido,
                                            "El cursor de devolución no es válido para esta tarea.")
            Return respuesta
        End If
        solicitud.Termino = terminoNormalizado

        Dim resultados As ResultadoBusquedaDevolverActividad = _previewRepository.BuscarDestinos(contexto, tarea, solicitud)
        If resultados Is Nothing OrElse Not String.IsNullOrWhiteSpace(resultados.CodigoBloqueo) Then
            respuesta.[Error] = CrearError(If(resultados Is Nothing OrElse String.IsNullOrWhiteSpace(resultados.CodigoBloqueo),
                                               CodigosBloqueoDevolverActividad.NoDisponible, resultados.CodigoBloqueo),
                                            If(resultados Is Nothing OrElse String.IsNullOrWhiteSpace(resultados.MensajeFuncional),
                                               "No fue posible consultar las actividades anteriores.", resultados.MensajeFuncional))
            Return respuesta
        End If

        respuesta.TokenVersion = tarea.TokenVersion
        respuesta.Contexto = New ContextoDevolverActividadDto With {
            .Radicado = tarea.Radicado,
            .ActividadActual = tarea.IdActividadActual.ToString(),
            .GrupoActual = tarea.NombreGrupoActual,
            .TipoContexto = tarea.TipoContexto
        }
        respuesta.TamanoPagina = resultados.TamanoPagina
        respuesta.HayMas = resultados.HayMas
        respuesta.Destinos = MapearDestinos(resultados.Destinos)
        If respuesta.HayMas AndAlso respuesta.Destinos.Count > 0 Then
            Dim ultimo As DestinoDevolverActividad = resultados.Destinos(resultados.Destinos.Count - 1)
            respuesta.CursorSiguiente = _cursorCodec.Proteger(contexto, tarea, terminoNormalizado, ultimo)
            If String.IsNullOrWhiteSpace(respuesta.CursorSiguiente) Then
                respuesta.[Error] = CrearError(CodigosBloqueoDevolverActividad.NoDisponible,
                                                "No fue posible preparar la siguiente página de devolución.")
                respuesta.Destinos = New List(Of DestinoDevolverActividadDto)()
                respuesta.HayMas = False
            End If
        End If
        Return respuesta
    End Function

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal solicitud As SolicitudEjecutarDevolverActividad) As ResultadoDevolverActividadDto
        Dim cronometro As Stopwatch = Stopwatch.StartNew()
        Dim resultado As ResultadoEjecucionDevolverActividad = Nothing
        Dim tareaAuditoria As TareaDevolverActividad = CrearTareaAuditoria(contexto, solicitud)
        Try
            Dim errorSolicitud As ResultadoEjecucionDevolverActividad = ValidarSolicitudEjecucion(solicitud)
            If errorSolicitud IsNot Nothing Then
                Return MapearEjecucion(RegistrarAuditoria(contexto, tareaAuditoria, Nothing, errorSolicitud, cronometro.ElapsedMilliseconds))
            End If
            If contexto Is Nothing OrElse Not contexto.EsValido() Then
                resultado = Bloquear(CodigosBloqueoDevolverActividad.ContextoInvalido,
                                     "No fue posible validar la sesión de la tarea.", False)
                Return MapearEjecucion(RegistrarAuditoria(contexto, tareaAuditoria, Nothing, resultado, cronometro.ElapsedMilliseconds))
            End If
            If _tareaRepository Is Nothing OrElse _autorizacionRepository Is Nothing OrElse _ejecucionRepository Is Nothing OrElse
               _concurrencyGuard Is Nothing OrElse _ejecutor Is Nothing Then
                resultado = Bloquear(CodigosBloqueoDevolverActividad.NoDisponible,
                                     "La devolución de tarea no está disponible.", True)
                Return MapearEjecucion(RegistrarAuditoria(contexto, tareaAuditoria, Nothing, resultado, cronometro.ElapsedMilliseconds))
            End If

            Dim guard As ResultadoGuardDevolverActividad = _concurrencyGuard.Adquirir(contexto, solicitud.IdTarea)
            If guard Is Nothing OrElse Not guard.Adquirido OrElse guard.Lease Is Nothing Then
                resultado = Bloquear(If(guard Is Nothing OrElse String.IsNullOrWhiteSpace(guard.CodigoBloqueo),
                                         CodigosBloqueoDevolverActividad.NoDisponible, guard.CodigoBloqueo),
                                     If(guard Is Nothing OrElse String.IsNullOrWhiteSpace(guard.MensajeFuncional),
                                        "No fue posible preparar la devolución de la tarea.", guard.MensajeFuncional), True)
                Return MapearEjecucion(RegistrarAuditoria(contexto, tareaAuditoria, Nothing, resultado, cronometro.ElapsedMilliseconds))
            End If

            Using guard.Lease
                'Dentro del lock se releen el permiso, el snapshot, el token y el conector; el request no aporta destino.
                Dim tarea As TareaDevolverActividad = _tareaRepository.ObtenerTarea(contexto, solicitud.IdTarea)
                If tarea Is Nothing OrElse Not tarea.EstaActiva Then
                    resultado = Bloquear(CodigosBloqueoDevolverActividad.TareaNoDisponible,
                                         "La tarea no está disponible para devolución.", False)
                    Return MapearEjecucion(RegistrarAuditoria(contexto, tareaAuditoria, Nothing, resultado, cronometro.ElapsedMilliseconds))
                End If
                Dim autorizacion As ResultadoAutorizacionDevolverActividad = _autorizacionRepository.Evaluar(contexto, tarea)
                If autorizacion Is Nothing OrElse Not autorizacion.Autorizado Then
                    resultado = Bloquear(If(autorizacion Is Nothing OrElse String.IsNullOrWhiteSpace(autorizacion.CodigoBloqueo),
                                             CodigosBloqueoDevolverActividad.PermisoDenegado, autorizacion.CodigoBloqueo),
                                         If(autorizacion Is Nothing OrElse String.IsNullOrWhiteSpace(autorizacion.MensajeFuncional),
                                            "El usuario no tiene permiso para devolver la tarea.", autorizacion.MensajeFuncional), False)
                    Return MapearEjecucion(RegistrarAuditoria(contexto, tarea, Nothing, resultado, cronometro.ElapsedMilliseconds))
                End If
                If Not String.Equals(tarea.TokenVersion, solicitud.TokenVersion, StringComparison.Ordinal) Then
                    resultado = Bloquear(CodigosBloqueoDevolverActividad.VersionConflicto,
                                         "La tarea cambió; actualice la información antes de devolverla.", False)
                    Return MapearEjecucion(RegistrarAuditoria(contexto, tarea, Nothing, resultado, cronometro.ElapsedMilliseconds))
                End If

                Dim resolucion As ResultadoResolucionDevolverActividad = _ejecucionRepository.ResolverDestino(contexto, tarea, solicitud.IdConector)
                If resolucion Is Nothing OrElse Not resolucion.EsValido Then
                    resultado = Bloquear(If(resolucion Is Nothing OrElse String.IsNullOrWhiteSpace(resolucion.CodigoBloqueo),
                                             CodigosBloqueoDevolverActividad.ConectorNoDisponible, resolucion.CodigoBloqueo),
                                         If(resolucion Is Nothing OrElse String.IsNullOrWhiteSpace(resolucion.MensajeFuncional),
                                            "La actividad anterior ya no está disponible.", resolucion.MensajeFuncional), False)
                    Return MapearEjecucion(RegistrarAuditoria(contexto, tarea, Nothing, resultado, cronometro.ElapsedMilliseconds))
                End If

                resultado = _ejecutor.Ejecutar(contexto, tarea, resolucion.Destino)
                If resultado Is Nothing Then
                    resultado = Bloquear(CodigosBloqueoDevolverActividad.NoDisponible,
                                         "No fue posible devolver la tarea.", True)
                End If
                Return MapearEjecucion(RegistrarAuditoria(contexto, tarea, resolucion.Destino, resultado, cronometro.ElapsedMilliseconds))
            End Using
        Catch
            resultado = Bloquear(CodigosBloqueoDevolverActividad.NoDisponible,
                                 "No fue posible devolver la tarea.", True)
            Return MapearEjecucion(RegistrarAuditoria(contexto, tareaAuditoria, Nothing, resultado, cronometro.ElapsedMilliseconds))
        End Try
    End Function

    Private Function RegistrarAuditoria(ByVal contexto As ContextoModuloWorkflow,
                                        ByVal tarea As TareaDevolverActividad,
                                        ByVal destino As DestinoDevolverActividad,
                                        ByVal resultado As ResultadoEjecucionDevolverActividad,
                                        ByVal duracion As Long) As ResultadoEjecucionDevolverActividad
        If resultado Is Nothing OrElse tarea Is Nothing OrElse tarea.IdTarea <= 0 Then Return resultado
        Dim referencia As String = "WF-RET-" & Guid.NewGuid().ToString("N").Substring(0, 16)
        resultado.ReferenciaAuditoria = referencia
        If _auditoriaRepository Is Nothing Then
            If resultado.Exito Then resultado.Advertencias.Add("La tarea fue devuelta, pero no se pudo registrar la auditoría adicional.")
            Return resultado
        End If
        Dim auditoria As New AuditoriaDevolverActividad With {
            .IdTarea = tarea.IdTarea,
            .IdUsuarioWorkflow = If(contexto Is Nothing, 0, contexto.IdUsuarioWorkflow),
            .IdRuta = If(tarea.IdRuta > 0, tarea.IdRuta, If(contexto Is Nothing, 0, contexto.IdRutaWorkflow)),
            .IdFlujoTrabajo = tarea.IdFlujoTrabajo,
            .IdActividadOrigen = tarea.IdActividadActual,
            .IdActividadDestino = If(destino Is Nothing, 0, destino.IdActividadDestino),
            .IdConector = If(destino Is Nothing, 0, destino.IdConector),
            .FechaUtc = DateTime.UtcNow,
            .DuracionMilisegundos = Math.Max(0, duracion),
            .Resultado = If(resultado.Exito, "EXITO", If(String.Equals(resultado.EstadoFinal, "bloqueado", StringComparison.OrdinalIgnoreCase), "BLOQUEADO", "ERROR")),
            .CodigoFuncional = If(resultado.Exito, "WORKFLOW_RETURN_SUCCESS", resultado.CodigoBloqueo),
            .Referencia = referencia
        }
        If Not _auditoriaRepository.Registrar(auditoria) AndAlso resultado.Exito Then
            resultado.Advertencias.Add("La tarea fue devuelta, pero no se pudo registrar la auditoría adicional.")
        End If
        Return resultado
    End Function

    Private Shared Function ValidarSolicitudEjecucion(ByVal solicitud As SolicitudEjecutarDevolverActividad) As ResultadoEjecucionDevolverActividad
        If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 OrElse solicitud.IdConector <= 0 Then
            Return Bloquear(CodigosBloqueoDevolverActividad.ConectorInvalido,
                            "La solicitud de devolución no es válida.", False)
        End If
        If String.IsNullOrWhiteSpace(solicitud.TokenVersion) OrElse solicitud.TokenVersion.Length > 80 Then
            Return Bloquear(CodigosBloqueoDevolverActividad.VersionInvalida,
                            "La versión de la tarea no es válida.", False)
        End If
        Return Nothing
    End Function

    Private Shared Function CrearTareaAuditoria(ByVal contexto As ContextoModuloWorkflow,
                                                 ByVal solicitud As SolicitudEjecutarDevolverActividad) As TareaDevolverActividad
        Return New TareaDevolverActividad With {
            .IdTarea = If(solicitud Is Nothing, 0L, solicitud.IdTarea),
            .IdRuta = If(contexto Is Nothing, 0, contexto.IdRutaWorkflow),
            .IdGrupoActual = If(contexto Is Nothing, 0, contexto.IdGrupoWorkflow),
            .IdActividadActual = 0,
            .EstaActiva = False
        }
    End Function

    Private Shared Function Bloquear(ByVal codigo As String,
                                     ByVal mensaje As String,
                                     ByVal reintentable As Boolean) As ResultadoEjecucionDevolverActividad
        Return New ResultadoEjecucionDevolverActividad With {
            .Exito = False,
            .EstadoFinal = "bloqueado",
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .EsReintentable = reintentable
        }
    End Function

    Private Shared Function MapearEjecucion(ByVal resultado As ResultadoEjecucionDevolverActividad) As ResultadoDevolverActividadDto
        Dim respuesta As New ResultadoDevolverActividadDto()
        If resultado Is Nothing Then
            respuesta.Exito = False
            respuesta.EstadoFinal = "bloqueado"
            respuesta.CodigoBloqueo = CodigosBloqueoDevolverActividad.NoDisponible
            respuesta.MensajeFuncional = "No fue posible devolver la tarea."
            respuesta.[Error] = CrearError(respuesta.CodigoBloqueo, respuesta.MensajeFuncional)
            Return respuesta
        End If
        respuesta.Exito = resultado.Exito
        respuesta.EstadoFinal = resultado.EstadoFinal
        respuesta.CodigoBloqueo = resultado.CodigoBloqueo
        respuesta.MensajeFuncional = resultado.MensajeFuncional
        respuesta.EsReintentable = resultado.EsReintentable
        respuesta.ReferenciaAuditoria = resultado.ReferenciaAuditoria
        respuesta.Advertencias = If(resultado.Advertencias, New List(Of String)())
        If Not resultado.Exito Then respuesta.[Error] = CrearError(resultado.CodigoBloqueo, resultado.MensajeFuncional)
        Return respuesta
    End Function

    Private Shared Function NormalizarSolicitud(ByVal solicitud As SolicitudPreviewDevolverActividad,
                                                 ByRef terminoNormalizado As String) As ErrorDevolverActividadDto
        terminoNormalizado = String.Empty
        If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 Then
            Return CrearError(CodigosBloqueoDevolverActividad.TareaInvalida, "La tarea indicada no es válida.")
        End If
        If solicitud.TamanoPagina <= 0 Then solicitud.TamanoPagina = TamanoPaginaPredeterminado
        solicitud.TamanoPagina = Math.Min(TamanoPaginaMaximo, solicitud.TamanoPagina)
        terminoNormalizado = If(solicitud.Termino, String.Empty).Trim().ToUpperInvariant()
        If terminoNormalizado.Length > 0 AndAlso terminoNormalizado.Length < LongitudMinimaTermino Then
            Return CrearError(CodigosBloqueoDevolverActividad.TerminoInvalido,
                              "El término de búsqueda debe tener al menos dos caracteres.")
        End If
        If terminoNormalizado.Length > LongitudMaximaTermino Then
            Return CrearError(CodigosBloqueoDevolverActividad.TerminoInvalido,
                              "El término de búsqueda excede el tamaño permitido.")
        End If
        Return Nothing
    End Function

    Private Shared Function MapearDestinos(ByVal destinos As IList(Of DestinoDevolverActividad)) As IList(Of DestinoDevolverActividadDto)
        Dim resultado As New List(Of DestinoDevolverActividadDto)()
        If destinos Is Nothing Then Return resultado
        For Each destino As DestinoDevolverActividad In destinos
            If destino Is Nothing Then Continue For
            resultado.Add(New DestinoDevolverActividadDto With {
                .IdConector = destino.IdConector,
                .NombreActividad = destino.NombreActividad,
                .Destinatario = destino.NombreUsuarioDestino,
                .GrupoDestino = destino.NombreGrupoDestino,
                .TipoContexto = destino.TipoContexto,
                .Orden = destino.Orden
            })
        Next
        Return resultado
    End Function

    Private Shared Function CrearError(ByVal codigo As String, ByVal mensaje As String) As ErrorDevolverActividadDto
        Return New ErrorDevolverActividadDto With {
            .Codigo = codigo,
            .MensajeVisible = mensaje,
            .ReferenciaTrazabilidad = String.Empty
        }
    End Function
End Class
