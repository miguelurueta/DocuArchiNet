Imports System
Imports System.Collections.Generic

'Consulta el estado de respuesta para Enviar a usuario; nunca reasigna ni cambia la respuesta.
Public Class WorkflowLegacyEnvioUsuarioRequisitosAdapter
    Implements IEnvioUsuarioRequisitosRepository

    Public Function Evaluar(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaWorkflow,
                            ByVal destino As DestinoEnvioUsuarioWorkflow) As ResultadoRequisitosEnvioUsuario Implements IEnvioUsuarioRequisitosRepository.Evaluar
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse contexto.IdUsuarioGestion <= 0 OrElse
           tarea Is Nothing OrElse Not tarea.EstaActiva OrElse destino Is Nothing OrElse
           destino.IdUsuarioWorkflowDestino <= 0 OrElse destino.IdActividadDestino <= 0 Then
            Return Bloqueado(CodigosBloqueoPrevisualizacion.ContextoInvalido,
                             "No fue posible validar los requisitos de la tarea.",
                             "WORKFLOW_CONTEXT_VALID")
        End If

        Try
            Dim requisitoRespuesta As New RequisitoTransicion With {
                .Codigo = "WORKFLOW_RESPONSE_ALLOWED",
                .Descripcion = "La tarea no requiere confirmacion ni radicado de respuesta.",
                .Obligatorio = True,
                .Satisfecho = False
            }
            Dim resultadoRespuesta As String = New Classgestionrespuesta().Verifica_respuesta_radicado_sin_respuesta(
                contexto.IdUsuarioGestion,
                CInt(tarea.IdTarea))
            If Not String.Equals(resultadoRespuesta, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Bloqueado(CodigosBloqueoPrevisualizacion.RequisitoNoCumplido,
                                 "La tarea requiere respuesta o confirmacion antes de enviarla.",
                                 requisitoRespuesta)
            End If

            requisitoRespuesta.Satisfecho = True
            Return New ResultadoRequisitosEnvioUsuario With {
                .Cumple = True,
                .Requisitos = New List(Of RequisitoTransicion) From {requisitoRespuesta}
            }
        Catch
            Return Bloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                             "No fue posible validar los requisitos de la tarea.",
                             "WORKFLOW_REQUIREMENTS_AVAILABLE")
        End Try
    End Function

    Private Shared Function Bloqueado(ByVal codigo As String,
                                      ByVal mensaje As String,
                                      ByVal codigoRequisito As String) As ResultadoRequisitosEnvioUsuario
        Return Bloqueado(codigo, mensaje, New RequisitoTransicion With {
            .Codigo = codigoRequisito,
            .Descripcion = mensaje,
            .Obligatorio = True,
            .Satisfecho = False
        })
    End Function

    Private Shared Function Bloqueado(ByVal codigo As String,
                                      ByVal mensaje As String,
                                      ByVal requisito As RequisitoTransicion) As ResultadoRequisitosEnvioUsuario
        Return New ResultadoRequisitosEnvioUsuario With {
            .Cumple = False,
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .Requisitos = New List(Of RequisitoTransicion) From {requisito}
        }
    End Function
End Class
