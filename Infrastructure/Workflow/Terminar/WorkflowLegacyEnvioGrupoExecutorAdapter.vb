Imports System
Imports System.Globalization

'Único límite nuevo para la mutación directa de Enviar a grupo.
Public Class WorkflowLegacyEnvioGrupoExecutorAdapter
    Implements IEnvioGrupoLegacyExecutor

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaWorkflow,
                             ByVal destino As DestinoEnvioGrupoWorkflow) As ResultadoEjecucionWorkflow Implements IEnvioGrupoLegacyExecutor.Ejecutar
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse tarea Is Nothing OrElse Not tarea.EstaActiva OrElse
           destino Is Nothing OrElse destino.IdActividadDestino <= 0 Then
            Return Rechazado(CodigosBloqueoPrevisualizacion.TransicionInconsistente,
                             "No fue posible preparar el envio de la tarea.",
                             False)
        End If

        Try
            Dim pagina As System.Web.UI.Page = Nothing
            Dim resultadoEvento As String = String.Empty
            Dim resultadoCorreo As String = String.Empty
            Dim resultadoLegacy As String = New ClassWorkflow().Terminar_Tarea_Workflow(
                String.Empty,
                destino.IdActividadDestino.ToString(CultureInfo.InvariantCulture),
                tarea.IdTarea,
                destino.NombreActividad,
                pagina,
                resultadoEvento,
                0,
                resultadoCorreo,
                0,
                0,
                0,
                If(destino.RequiereNotificacion, 1, 0),
                0,
                contexto.IdUsuarioWorkflow,
                tarea.IdActividadOrigen,
                0,
                1,
                0,
                0)
            If Not String.Equals(resultadoLegacy, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Rechazado(CodigosBloqueoPrevisualizacion.TransicionRechazada,
                                 "No fue posible enviar la tarea.",
                                 False)
            End If

            Dim resultado As New ResultadoEjecucionWorkflow With {
                .Exito = True,
                .EstadoFinal = "completada",
                .MensajeFuncional = "La tarea fue enviada.",
                .EsReintentable = False
            }
            If EsAdvertencia(resultadoEvento) Then
                resultado.Advertencias.Add("La tarea fue enviada con una advertencia de procesamiento posterior.")
            End If
            If EsAdvertencia(resultadoCorreo) Then
                resultado.Advertencias.Add("La tarea fue enviada con una advertencia de notificacion.")
            End If
            Return resultado
        Catch
            Return Rechazado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                             "No fue posible enviar la tarea.",
                             True)
        End Try
    End Function

    Private Shared Function Rechazado(ByVal codigo As String,
                                      ByVal mensaje As String,
                                      ByVal reintentable As Boolean) As ResultadoEjecucionWorkflow
        Return New ResultadoEjecucionWorkflow With {
            .Exito = False,
            .EstadoFinal = "bloqueado",
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .EsReintentable = reintentable
        }
    End Function

    Private Shared Function EsAdvertencia(ByVal resultado As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(resultado) AndAlso
               Not String.Equals(resultado, "YES", StringComparison.OrdinalIgnoreCase)
    End Function
End Class
