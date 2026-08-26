Imports System
Imports System.Globalization

'Único punto mutante de Usuario anterior. No usa controles Web Forms, notificación, eventos ni componentes de respuestas.
Public Class WorkflowLegacyDevolverUsuarioAnteriorExecutorAdapter
    Implements IDevolverUsuarioAnteriorLegacyExecutor

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaDevolverUsuarioAnterior,
                             ByVal usuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior) As ResultadoEjecucionDevolverUsuarioAnterior Implements IDevolverUsuarioAnteriorLegacyExecutor.Ejecutar
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
           usuarioHistorico Is Nothing OrElse usuarioHistorico.IdUsuarioWorkflow <= 0 OrElse usuarioHistorico.IdActividad <= 0 OrElse
           usuarioHistorico.IdRuta <> tarea.IdRuta OrElse usuarioHistorico.IdFlujoTrabajo <> tarea.IdFlujoTrabajo Then
            Return Rechazar(CodigosBloqueoDevolverUsuarioAnterior.DestinoNoDisponible, "No fue posible preparar la devolución de la tarea.", False)
        End If
        Try
            Dim pagina As System.Web.UI.Page = Nothing
            Dim resultadoEvento As String = String.Empty
            Dim resultadoCorreo As String = String.Empty
            Dim resultadoLegacy As String = TerminarTareaWorkflow(
                usuarioHistorico.IdUsuarioWorkflow.ToString(CultureInfo.InvariantCulture),
                usuarioHistorico.IdActividad.ToString(CultureInfo.InvariantCulture),
                tarea.IdTarea,
                String.Empty,
                pagina,
                resultadoEvento,
                0,
                resultadoCorreo,
                usuarioHistorico.IdFlujoTrabajo,
                usuarioHistorico.IdActividadFlujoTrabajo,
                usuarioHistorico.IdUsuarioWorkflowFlujoTrabajo,
                0,
                0,
                contexto.IdUsuarioWorkflow,
                tarea.IdActividadActual,
                0,
                0,
                0,
                0)
            If Not String.Equals(resultadoLegacy, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Rechazar(CodigosBloqueoDevolverUsuarioAnterior.Rechazada, "No fue posible devolver la tarea.", False)
            End If
            Return New ResultadoEjecucionDevolverUsuarioAnterior With {
                .Exito = True,
                .EstadoFinal = "completada",
                .MensajeFuncional = "La tarea fue devuelta al usuario anterior.",
                .EsReintentable = False}
        Catch
            Return Rechazar(CodigosBloqueoDevolverUsuarioAnterior.NoDisponible, "No fue posible devolver la tarea.", True)
        End Try
    End Function

    'Se mantiene dentro del adaptador para que el único invocador productivo del motor sea esta capacidad.
    'La sustitución protegida permite verificar la matriz inhibidora sin Page ni base de datos.
    Protected Overridable Function TerminarTareaWorkflow(ByVal idUsuarioDestino As String,
                                                          ByVal idActividadDestino As String,
                                                          ByVal idTarea As Long,
                                                          ByVal nombreActividad As String,
                                                          ByRef pagina As System.Web.UI.Page,
                                                          ByRef resultadoEvento As String,
                                                          ByVal notifica As Integer,
                                                          ByRef resultadoCorreo As String,
                                                          ByVal idFlujoTrabajo As Integer,
                                                          ByVal idActividadFlujoTrabajo As Integer,
                                                          ByVal idUsuarioWorkflowFlujoTrabajo As Integer,
                                                          ByVal notificaEnvioCorreo As Integer,
                                                          ByVal idConector As Integer,
                                                          ByVal idUsuarioWorkflowEnvia As Integer,
                                                          ByVal idActividadWorkflowEnvia As Integer,
                                                          ByVal actualizaInterfazLegacy As Integer,
                                                          ByVal activaEventosDinamicos As Integer,
                                                          ByVal activaReasignaSii As Integer,
                                                          ByVal activaReasignaTareaWorkflow As Integer) As String
        Return New ClassWorkflow().Terminar_Tarea_Workflow(
            idUsuarioDestino,
            idActividadDestino,
            idTarea,
            nombreActividad,
            pagina,
            resultadoEvento,
            notifica,
            resultadoCorreo,
            idFlujoTrabajo,
            idActividadFlujoTrabajo,
            idUsuarioWorkflowFlujoTrabajo,
            notificaEnvioCorreo,
            idConector,
            idUsuarioWorkflowEnvia,
            idActividadWorkflowEnvia,
            actualizaInterfazLegacy,
            activaEventosDinamicos,
            activaReasignaSii,
            activaReasignaTareaWorkflow)
    End Function

    Private Shared Function Rechazar(ByVal codigo As String, ByVal mensaje As String, ByVal reintentable As Boolean) As ResultadoEjecucionDevolverUsuarioAnterior
        Return New ResultadoEjecucionDevolverUsuarioAnterior With {.Exito = False, .EstadoFinal = "bloqueado", .CodigoBloqueo = codigo, .MensajeFuncional = mensaje, .EsReintentable = reintentable}
    End Function
End Class
