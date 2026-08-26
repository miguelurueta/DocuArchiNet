Imports System
Imports System.Collections.Generic
Imports System.Diagnostics

'Caso de uso exclusivo. Preview no escribe; ejecución revalida todo dentro del lock y no acepta destino del cliente.
Public Class ServicioDevolverUsuarioAnterior
    Private ReadOnly _tareaRepository As IDevolverUsuarioAnteriorTareaRepository
    Private ReadOnly _autorizacionRepository As IDevolverUsuarioAnteriorAutorizacionRepository
    Private ReadOnly _historialRepository As IDevolverUsuarioAnteriorHistorialRepository
    Private ReadOnly _tokenCodec As IDevolverUsuarioAnteriorTokenCodec
    Private ReadOnly _concurrencyGuard As IDevolverUsuarioAnteriorConcurrencyGuard
    Private ReadOnly _ejecutor As IDevolverUsuarioAnteriorLegacyExecutor
    Private ReadOnly _auditoriaRepository As IDevolverUsuarioAnteriorAuditoriaRepository

    Public Sub New(ByVal tareaRepository As IDevolverUsuarioAnteriorTareaRepository,
                   ByVal autorizacionRepository As IDevolverUsuarioAnteriorAutorizacionRepository,
                   ByVal historialRepository As IDevolverUsuarioAnteriorHistorialRepository,
                   ByVal tokenCodec As IDevolverUsuarioAnteriorTokenCodec)
        Me.New(tareaRepository, autorizacionRepository, historialRepository, tokenCodec, Nothing, Nothing, Nothing)
    End Sub

    Public Sub New(ByVal tareaRepository As IDevolverUsuarioAnteriorTareaRepository,
                   ByVal autorizacionRepository As IDevolverUsuarioAnteriorAutorizacionRepository,
                   ByVal historialRepository As IDevolverUsuarioAnteriorHistorialRepository,
                   ByVal tokenCodec As IDevolverUsuarioAnteriorTokenCodec,
                   ByVal concurrencyGuard As IDevolverUsuarioAnteriorConcurrencyGuard,
                   ByVal ejecutor As IDevolverUsuarioAnteriorLegacyExecutor,
                   ByVal auditoriaRepository As IDevolverUsuarioAnteriorAuditoriaRepository)
        _tareaRepository = tareaRepository
        _autorizacionRepository = autorizacionRepository
        _historialRepository = historialRepository
        _tokenCodec = tokenCodec
        _concurrencyGuard = concurrencyGuard
        _ejecutor = ejecutor
        _auditoriaRepository = auditoriaRepository
    End Sub

    Public Function Previsualizar(ByVal contexto As ContextoModuloWorkflow,
                                  ByVal solicitud As SolicitudPreviewDevolverUsuarioAnterior) As PrevisualizacionDevolverUsuarioAnteriorDto
        Dim respuesta As New PrevisualizacionDevolverUsuarioAnteriorDto With {.IdTarea = If(solicitud Is Nothing, 0L, solicitud.IdTarea)}
        If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 Then
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverUsuarioAnterior.TareaInvalida, "La tarea indicada no es válida.")
            Return respuesta
        End If
        If contexto Is Nothing OrElse Not contexto.EsValido() Then
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverUsuarioAnterior.ContextoInvalido, "No fue posible validar la sesión de la tarea.")
            Return respuesta
        End If
        If Not contexto.PuedeDevolverUsuarioAnterior Then
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverUsuarioAnterior.PermisoDenegado, "El usuario no tiene permiso para devolver la tarea.")
            Return respuesta
        End If
        If _tareaRepository Is Nothing OrElse _autorizacionRepository Is Nothing OrElse _historialRepository Is Nothing OrElse _tokenCodec Is Nothing Then
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, "La devolución a usuario anterior no está disponible.")
            Return respuesta
        End If
        Try
            Dim tarea As TareaDevolverUsuarioAnterior = _tareaRepository.ObtenerTarea(contexto, solicitud.IdTarea)
            Dim errorAutorizacion As ErrorDevolverUsuarioAnteriorDto = ValidarAutorizacion(contexto, tarea)
            If errorAutorizacion IsNot Nothing Then
                respuesta.[Error] = errorAutorizacion
                Return respuesta
            End If
            Dim historial As ResultadoHistorialDevolverUsuarioAnterior = _historialRepository.ObtenerAntecedente(contexto, tarea)
            Dim errorHistorial As ErrorDevolverUsuarioAnteriorDto = ValidarHistorial(contexto, historial)
            If errorHistorial IsNot Nothing Then
                respuesta.[Error] = errorHistorial
                Return respuesta
            End If
            Dim token As String = _tokenCodec.Emitir(contexto, tarea, historial.UsuarioHistorico)
            If String.IsNullOrWhiteSpace(token) Then
                respuesta.[Error] = CrearError(CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, "No fue posible preparar la confirmación de devolución.")
                Return respuesta
            End If
            respuesta.TokenVersion = token
            respuesta.Contexto = New ContextoDevolverUsuarioAnteriorDto With {
                .ActividadActual = tarea.IdActividadActual.ToString(Globalization.CultureInfo.InvariantCulture),
                .ActividadAnterior = historial.UsuarioHistorico.NombreActividad,
                .UsuarioAnterior = historial.UsuarioHistorico.NombreUsuario}
            Return respuesta
        Catch
            respuesta.[Error] = CrearError(CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, "No fue posible consultar el usuario anterior.")
            Return respuesta
        End Try
    End Function

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal solicitud As SolicitudEjecutarDevolverUsuarioAnterior) As ResultadoDevolverUsuarioAnteriorDto
        Dim cronometro As Stopwatch = Stopwatch.StartNew()
        Dim tareaAuditoria As TareaDevolverUsuarioAnterior = CrearTareaAuditoria(contexto, solicitud)
        Try
            If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 OrElse String.IsNullOrWhiteSpace(solicitud.TokenVersion) OrElse solicitud.TokenVersion.Length > 512 Then
                Return RegistrarAuditoria(contexto, tareaAuditoria, Nothing,
                    Bloquear(CodigosBloqueoDevolverUsuarioAnterior.VersionInvalida, "La confirmación de devolución no es válida.", False), cronometro.ElapsedMilliseconds)
            End If
            If contexto Is Nothing OrElse Not contexto.EsValido() Then
                Return RegistrarAuditoria(contexto, tareaAuditoria, Nothing,
                    Bloquear(CodigosBloqueoDevolverUsuarioAnterior.ContextoInvalido, "No fue posible validar la sesión de la tarea.", False), cronometro.ElapsedMilliseconds)
            End If
            If Not contexto.PuedeDevolverUsuarioAnterior Then
                Return RegistrarAuditoria(contexto, tareaAuditoria, Nothing,
                    Bloquear(CodigosBloqueoDevolverUsuarioAnterior.PermisoDenegado, "El usuario no tiene permiso para devolver la tarea.", False), cronometro.ElapsedMilliseconds)
            End If
            If _tareaRepository Is Nothing OrElse _autorizacionRepository Is Nothing OrElse _historialRepository Is Nothing OrElse _tokenCodec Is Nothing OrElse
               _concurrencyGuard Is Nothing OrElse _ejecutor Is Nothing Then
                Return RegistrarAuditoria(contexto, tareaAuditoria, Nothing,
                    Bloquear(CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, "La devolución a usuario anterior no está disponible.", True), cronometro.ElapsedMilliseconds)
            End If

            Dim guard As ResultadoGuardDevolverUsuarioAnterior = _concurrencyGuard.Adquirir(contexto, solicitud.IdTarea)
            If guard Is Nothing OrElse Not guard.Adquirido OrElse guard.Lease Is Nothing Then
                Return RegistrarAuditoria(contexto, tareaAuditoria, Nothing,
                    Bloquear(If(guard Is Nothing OrElse String.IsNullOrWhiteSpace(guard.CodigoBloqueo), CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, guard.CodigoBloqueo),
                              If(guard Is Nothing OrElse String.IsNullOrWhiteSpace(guard.MensajeFuncional), "No fue posible preparar la devolución de la tarea.", guard.MensajeFuncional), True),
                    cronometro.ElapsedMilliseconds)
            End If

            Using guard.Lease
                Dim tarea As TareaDevolverUsuarioAnterior = _tareaRepository.ObtenerTarea(contexto, solicitud.IdTarea)
                Dim errorAutorizacion As ErrorDevolverUsuarioAnteriorDto = ValidarAutorizacion(contexto, tarea)
                If errorAutorizacion IsNot Nothing Then
                    Return RegistrarAuditoria(contexto, If(tarea, tareaAuditoria), Nothing,
                        Bloquear(errorAutorizacion.Codigo, errorAutorizacion.MensajeVisible, False), cronometro.ElapsedMilliseconds)
                End If
                Dim historial As ResultadoHistorialDevolverUsuarioAnterior = _historialRepository.ObtenerAntecedente(contexto, tarea)
                Dim errorHistorial As ErrorDevolverUsuarioAnteriorDto = ValidarHistorial(contexto, historial)
                If errorHistorial IsNot Nothing Then
                    Return RegistrarAuditoria(contexto, tarea, Nothing,
                        Bloquear(errorHistorial.Codigo, errorHistorial.MensajeVisible, False), cronometro.ElapsedMilliseconds)
                End If
                If Not _tokenCodec.Validar(contexto, tarea, historial.UsuarioHistorico, solicitud.TokenVersion) Then
                    Return RegistrarAuditoria(contexto, tarea, historial.UsuarioHistorico,
                        Bloquear(CodigosBloqueoDevolverUsuarioAnterior.VersionConflicto, "La tarea o su historial cambió; actualice la información antes de devolverla.", False),
                        cronometro.ElapsedMilliseconds)
                End If
                Dim resultado As ResultadoEjecucionDevolverUsuarioAnterior = _ejecutor.Ejecutar(contexto, tarea, historial.UsuarioHistorico)
                If resultado Is Nothing Then
                    resultado = Bloquear(CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, "No fue posible devolver la tarea.", True)
                End If
                Return RegistrarAuditoria(contexto, tarea, historial.UsuarioHistorico, resultado, cronometro.ElapsedMilliseconds)
            End Using
        Catch
            Return RegistrarAuditoria(contexto, tareaAuditoria, Nothing,
                Bloquear(CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, "No fue posible devolver la tarea.", True), cronometro.ElapsedMilliseconds)
        End Try
    End Function

    Private Function ValidarAutorizacion(ByVal contexto As ContextoModuloWorkflow,
                                         ByVal tarea As TareaDevolverUsuarioAnterior) As ErrorDevolverUsuarioAnteriorDto
        If tarea Is Nothing OrElse Not tarea.EstaActiva Then
            Return CrearError(CodigosBloqueoDevolverUsuarioAnterior.TareaNoDisponible, "La tarea no está disponible para devolución.")
        End If
        Dim autorizacion As ResultadoAutorizacionDevolverUsuarioAnterior = _autorizacionRepository.Evaluar(contexto, tarea)
        If autorizacion Is Nothing OrElse Not autorizacion.Autorizado Then
            Return CrearError(If(autorizacion Is Nothing OrElse String.IsNullOrWhiteSpace(autorizacion.CodigoBloqueo), CodigosBloqueoDevolverUsuarioAnterior.PermisoDenegado, autorizacion.CodigoBloqueo),
                              If(autorizacion Is Nothing OrElse String.IsNullOrWhiteSpace(autorizacion.MensajeFuncional), "El usuario no tiene permiso para devolver la tarea.", autorizacion.MensajeFuncional))
        End If
        Return Nothing
    End Function

    Private Shared Function ValidarHistorial(ByVal contexto As ContextoModuloWorkflow,
                                             ByVal historial As ResultadoHistorialDevolverUsuarioAnterior) As ErrorDevolverUsuarioAnteriorDto
        If historial Is Nothing OrElse Not historial.EsValido Then
            Return CrearError(If(historial Is Nothing OrElse String.IsNullOrWhiteSpace(historial.CodigoBloqueo), CodigosBloqueoDevolverUsuarioAnterior.HistorialNoDisponible, historial.CodigoBloqueo),
                              If(historial Is Nothing OrElse String.IsNullOrWhiteSpace(historial.MensajeFuncional), "No existe un usuario anterior disponible para la tarea.", historial.MensajeFuncional))
        End If
        If contexto Is Nothing OrElse historial.UsuarioHistorico.IdUsuarioWorkflow = contexto.IdUsuarioWorkflow Then
            Return CrearError(CodigosBloqueoDevolverUsuarioAnterior.AutoDevolucion, "El usuario no puede devolver la tarea a sí mismo.")
        End If
        Return Nothing
    End Function

    Private Function RegistrarAuditoria(ByVal contexto As ContextoModuloWorkflow,
                                        ByVal tarea As TareaDevolverUsuarioAnterior,
                                        ByVal usuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior,
                                        ByVal resultado As ResultadoEjecucionDevolverUsuarioAnterior,
                                        ByVal duracion As Long) As ResultadoDevolverUsuarioAnteriorDto
        Dim respuesta As ResultadoDevolverUsuarioAnteriorDto = MapearResultado(resultado, usuarioHistorico)
        If tarea Is Nothing OrElse tarea.IdTarea <= 0 OrElse resultado Is Nothing Then Return respuesta
        Dim referencia As String = "WF-RUS-" & Guid.NewGuid().ToString("N").Substring(0, 16)
        respuesta.ReferenciaAuditoria = referencia
        If _auditoriaRepository Is Nothing Then
            If respuesta.Exito Then respuesta.Advertencias.Add("La tarea fue devuelta, pero no se pudo registrar la auditoría adicional.")
            Return respuesta
        End If
        Dim auditoria As New AuditoriaDevolverUsuarioAnterior With {
            .IdTarea = tarea.IdTarea,
            .IdUsuarioWorkflow = If(contexto Is Nothing, 0, contexto.IdUsuarioWorkflow),
            .IdRuta = If(tarea.IdRuta > 0, tarea.IdRuta, If(contexto Is Nothing, 0, contexto.IdRutaWorkflow)),
            .IdFlujoTrabajo = tarea.IdFlujoTrabajo,
            .IdActividadOrigen = tarea.IdActividadActual,
            .IdActividadDestino = If(usuarioHistorico Is Nothing, 0, usuarioHistorico.IdActividad),
            .FechaUtc = DateTime.UtcNow,
            .DuracionMilisegundos = Math.Max(0, duracion),
            .Resultado = If(resultado.Exito, "EXITO", If(String.Equals(resultado.EstadoFinal, "bloqueado", StringComparison.OrdinalIgnoreCase), "BLOQUEADO", "ERROR")),
            .CodigoFuncional = If(resultado.Exito, "WORKFLOW_RETURN_USER_SUCCESS", resultado.CodigoBloqueo),
            .Referencia = referencia}
        Try
            If Not _auditoriaRepository.Registrar(auditoria) AndAlso respuesta.Exito Then
                respuesta.Advertencias.Add("La tarea fue devuelta, pero no se pudo registrar la auditoría adicional.")
            End If
        Catch
            If respuesta.Exito Then respuesta.Advertencias.Add("La tarea fue devuelta, pero no se pudo registrar la auditoría adicional.")
        End Try
        Return respuesta
    End Function

    Private Shared Function MapearResultado(ByVal resultado As ResultadoEjecucionDevolverUsuarioAnterior,
                                            ByVal usuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior) As ResultadoDevolverUsuarioAnteriorDto
        If resultado Is Nothing Then
            Return New ResultadoDevolverUsuarioAnteriorDto With {
                .Exito = False, .EstadoFinal = "bloqueado", .CodigoBloqueo = CodigosBloqueoDevolverUsuarioAnterior.NoDisponible,
                .MensajeFuncional = "No fue posible devolver la tarea.", .[Error] = CrearError(CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, "No fue posible devolver la tarea.")}
        End If
        Dim respuesta As New ResultadoDevolverUsuarioAnteriorDto With {
            .Exito = resultado.Exito, .EstadoFinal = resultado.EstadoFinal, .CodigoBloqueo = resultado.CodigoBloqueo,
            .MensajeFuncional = resultado.MensajeFuncional, .EsReintentable = resultado.EsReintentable,
            .ActividadDestino = If(usuarioHistorico Is Nothing, String.Empty, usuarioHistorico.NombreActividad),
            .UsuarioDestino = If(usuarioHistorico Is Nothing, String.Empty, usuarioHistorico.NombreUsuario),
            .Advertencias = If(resultado.Advertencias, New List(Of String)())}
        If Not respuesta.Exito Then respuesta.[Error] = CrearError(respuesta.CodigoBloqueo, respuesta.MensajeFuncional)
        Return respuesta
    End Function

    Private Shared Function Bloquear(ByVal codigo As String, ByVal mensaje As String, ByVal reintentable As Boolean) As ResultadoEjecucionDevolverUsuarioAnterior
        Return New ResultadoEjecucionDevolverUsuarioAnterior With {.Exito = False, .EstadoFinal = "bloqueado", .CodigoBloqueo = codigo, .MensajeFuncional = mensaje, .EsReintentable = reintentable}
    End Function

    Private Shared Function CrearTareaAuditoria(ByVal contexto As ContextoModuloWorkflow,
                                                ByVal solicitud As SolicitudEjecutarDevolverUsuarioAnterior) As TareaDevolverUsuarioAnterior
        Return New TareaDevolverUsuarioAnterior With {.IdTarea = If(solicitud Is Nothing, 0L, solicitud.IdTarea), .IdRuta = If(contexto Is Nothing, 0, contexto.IdRutaWorkflow)}
    End Function

    Private Shared Function CrearError(ByVal codigo As String, ByVal mensaje As String) As ErrorDevolverUsuarioAnteriorDto
        Return New ErrorDevolverUsuarioAnteriorDto With {.Codigo = codigo, .MensajeVisible = mensaje, .ReferenciaTrazabilidad = String.Empty}
    End Function
End Class
