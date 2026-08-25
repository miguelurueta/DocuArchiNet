Imports System
Imports System.Globalization

'Único punto mutante de DOC-32: traduce un destino reconstruido al motor legacy sin activar interfaz ni reasignaciones.
Public Class WorkflowLegacyDevolverActividadExecutorAdapter
    Implements IDevolverActividadLegacyExecutor

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaDevolverActividad,
                             ByVal destino As DestinoDevolverActividad) As ResultadoEjecucionDevolverActividad Implements IDevolverActividadLegacyExecutor.Ejecutar
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
           destino Is Nothing OrElse destino.IdConector <= 0 OrElse destino.IdActividadDestino <= 0 OrElse
           Not String.Equals(destino.TipoContexto, tarea.TipoContexto, StringComparison.OrdinalIgnoreCase) Then
            Return Rechazar(CodigosBloqueoDevolverActividad.ContextoInconsistente,
                            "No fue posible preparar la devolución de la tarea.", False)
        End If

        Try
            Dim pagina As System.Web.UI.Page = Nothing
            Dim resultadoEvento As String = String.Empty
            Dim resultadoCorreo As String = String.Empty
            Dim usuarioDestino As String = If(destino.IdUsuarioWorkflowDestino > 0,
                                               destino.IdUsuarioWorkflowDestino.ToString(CultureInfo.InvariantCulture), String.Empty)
            Dim resultadoLegacy As String = New ClassWorkflow().Terminar_Tarea_Workflow(
                usuarioDestino,
                destino.IdActividadDestino.ToString(CultureInfo.InvariantCulture),
                tarea.IdTarea,
                String.Empty,
                pagina,
                resultadoEvento,
                0,
                resultadoCorreo,
                destino.IdFlujoTrabajo,
                destino.IdActividadFlujoOrigen,
                destino.IdUsuarioWorkflowDestino,
                If(destino.RequiereNotificacion, 1, 0),
                destino.IdConector,
                contexto.IdUsuarioWorkflow,
                tarea.IdActividadActual,
                0,
                1,
                0,
                0)
            If Not String.Equals(resultadoLegacy, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Rechazar(CodigosBloqueoDevolverActividad.Rechazada,
                                "No fue posible devolver la tarea.", False)
            End If

            Dim resultado As New ResultadoEjecucionDevolverActividad With {
                .Exito = True,
                .EstadoFinal = "completada",
                .MensajeFuncional = "La tarea fue devuelta.",
                .EsReintentable = False
            }
            If EsAdvertencia(resultadoEvento) Then resultado.Advertencias.Add("La devolución terminó con una advertencia de eventos.")
            If EsAdvertencia(resultadoCorreo) Then resultado.Advertencias.Add("La devolución terminó con una advertencia de notificación.")
            Return resultado
        Catch
            Return Rechazar(CodigosBloqueoDevolverActividad.NoDisponible,
                            "No fue posible devolver la tarea.", True)
        End Try
    End Function

    Private Shared Function Rechazar(ByVal codigo As String,
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

    Private Shared Function EsAdvertencia(ByVal resultado As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(resultado) AndAlso Not String.Equals(resultado, "YES", StringComparison.OrdinalIgnoreCase)
    End Function
End Class
