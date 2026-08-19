Public Class ValidadorEnvioGrupoTarea
    Public Function ValidarSolicitud(ByVal solicitud As SolicitudEnvioGrupoWorkflow) As ErrorTransicionDto
        If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 Then
            Return CrearError(CodigosBloqueoPrevisualizacion.TareaInvalida, "La tarea seleccionada no es valida.")
        End If
        If solicitud.IdActividadDestino <= 0 Then
            Return CrearError(CodigosBloqueoPrevisualizacion.ActividadDestinoInvalida, "El destino seleccionado no es valido.")
        End If
        If String.IsNullOrWhiteSpace(solicitud.TokenVersion) Then
            Return CrearError(CodigosBloqueoPrevisualizacion.VersionInvalida,
                              "La informacion de la tarea debe actualizarse antes de enviarla.")
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
