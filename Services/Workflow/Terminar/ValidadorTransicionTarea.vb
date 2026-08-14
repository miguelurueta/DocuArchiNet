Public Class ValidadorTransicionTarea
    Public Function ValidarContexto(ByVal contexto As ContextoModuloWorkflow) As ErrorTransicionDto
        If contexto Is Nothing OrElse Not contexto.EsValido() Then
            Return CrearError("WORKFLOW_CONTEXT_INVALID", "No fue posible validar el contexto de la tarea.")
        End If

        Return Nothing
    End Function

    Public Function ValidarSolicitud(ByVal solicitud As SolicitudTransicionWorkflow) As ErrorTransicionDto
        If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 Then
            Return CrearError("WORKFLOW_TASK_INVALID", "La tarea seleccionada no es valida.")
        End If

        If solicitud.IdConector < 0 Then
            Return CrearError("WORKFLOW_CONNECTOR_INVALID", "El destino seleccionado no es valido.")
        End If

        Return Nothing
    End Function

    Private Shared Function CrearError(ByVal codigo As String, ByVal mensaje As String) As ErrorTransicionDto
        Return New ErrorTransicionDto With {
            .Codigo = codigo,
            .MensajeVisible = mensaje,
            .ReferenciaTrazabilidad = String.Empty
        }
    End Function
End Class
