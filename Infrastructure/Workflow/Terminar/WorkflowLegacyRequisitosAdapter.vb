Imports System
Imports System.Collections.Generic
Imports System.Web

'Adaptador de consultas legacy previas. El servicio recibe un resultado tipado y nunca Session ni textos internos.
Public Class WorkflowLegacyRequisitosAdapter
    Implements IRequisitosTransicionRepository

    Public Function Evaluar(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaWorkflow,
                            ByVal destino As DestinoEjecucionWorkflow) As ResultadoRequisitosTransicion Implements IRequisitosTransicionRepository.Evaluar
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse tarea Is Nothing OrElse
           Not tarea.EstaActiva OrElse destino Is Nothing Then
            Return Bloqueado(CodigosBloqueoPrevisualizacion.ContextoInvalido,
                             "No fue posible validar los requisitos de la tarea.",
                             "WORKFLOW_CONTEXT_VALID")
        End If

        Dim requestContext As HttpContext = HttpContext.Current
        If requestContext Is Nothing OrElse requestContext.Session Is Nothing Then
            Return Bloqueado(CodigosBloqueoPrevisualizacion.ContextoInvalido,
                             "No fue posible validar los requisitos de la tarea.",
                             "WORKFLOW_CONTEXT_VALID")
        End If

        Dim idUsuarioGestion As Integer = 0
        If Not Integer.TryParse(Convert.ToString(requestContext.Session.Item("GA_IDUSUARIOGESTION")), idUsuarioGestion) OrElse
           idUsuarioGestion <= 0 Then
            Return Bloqueado(CodigosBloqueoPrevisualizacion.ContextoInvalido,
                             "No fue posible validar los requisitos de la tarea.",
                             "WORKFLOW_GESTION_USER")
        End If

        Try
            Dim requisitoRespuesta As New RequisitoTransicion With {
                .Codigo = "WORKFLOW_RESPONSE_REQUIRED",
                .Descripcion = "Respuesta o confirmacion requerida.",
                .Obligatorio = True,
                .Satisfecho = False
            }
            Dim resultadoRespuesta As String = New Classgestionrespuesta().Verifica_respuesta_radicado_sin_respuesta(
                idUsuarioGestion,
                CInt(tarea.IdTarea))
            If Not String.Equals(resultadoRespuesta, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Bloqueado(CodigosBloqueoPrevisualizacion.RequisitoNoCumplido,
                                 "La tarea requiere respuesta o confirmacion antes de enviarla.",
                                 requisitoRespuesta)
            End If
            requisitoRespuesta.Satisfecho = True

            Dim estadoAprobacion As String = String.Empty
            Dim requisitoAprobacion As New RequisitoTransicion With {
                .Codigo = "WORKFLOW_APPROVAL_REQUIRED",
                .Descripcion = "No existen solicitudes de aprobacion pendientes.",
                .Obligatorio = True,
                .Satisfecho = False
            }
            Dim resultadoAprobacion As String = New ClassRaSolicitudesAprobacion().Verifica_solicitudes_de_aprobacion_sin_desicion(
                CInt(tarea.IdTarea),
                estadoAprobacion,
                idUsuarioGestion)
            If Not String.Equals(resultadoAprobacion, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Bloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                                 "No fue posible validar las solicitudes de aprobacion de la tarea.",
                                 requisitoAprobacion)
            End If
            If String.Equals(estadoAprobacion, "YES", StringComparison.OrdinalIgnoreCase) Then
                Return Bloqueado(CodigosBloqueoPrevisualizacion.RequisitoNoCumplido,
                                 "La tarea tiene solicitudes de aprobacion pendientes.",
                                 requisitoAprobacion)
            End If
            requisitoAprobacion.Satisfecho = True

            Return New ResultadoRequisitosTransicion With {
                .Cumple = True,
                .Requisitos = New List(Of RequisitoTransicion) From {requisitoRespuesta, requisitoAprobacion}
            }
        Catch
            Return Bloqueado(CodigosBloqueoPrevisualizacion.TransicionNoDisponible,
                             "No fue posible validar los requisitos de la tarea.",
                             "WORKFLOW_REQUIREMENTS_AVAILABLE")
        End Try
    End Function

    Private Shared Function Bloqueado(ByVal codigo As String,
                                      ByVal mensaje As String,
                                      ByVal codigoRequisito As String) As ResultadoRequisitosTransicion
        Return Bloqueado(codigo, mensaje, New RequisitoTransicion With {
            .Codigo = codigoRequisito,
            .Descripcion = mensaje,
            .Obligatorio = True,
            .Satisfecho = False
        })
    End Function

    Private Shared Function Bloqueado(ByVal codigo As String,
                                      ByVal mensaje As String,
                                      ByVal requisito As RequisitoTransicion) As ResultadoRequisitosTransicion
        Return New ResultadoRequisitosTransicion With {
            .Cumple = False,
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje,
            .Requisitos = New List(Of RequisitoTransicion) From {requisito}
        }
    End Function
End Class
