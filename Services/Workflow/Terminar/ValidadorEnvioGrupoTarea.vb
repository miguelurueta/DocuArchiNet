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

    Public Function NormalizarBusqueda(ByVal solicitud As SolicitudBusquedaDestinosEnvioGrupo,
                                       ByRef normalizada As SolicitudBusquedaDestinosEnvioGrupo) As ErrorTransicionDto
        normalizada = New SolicitudBusquedaDestinosEnvioGrupo With {
            .IdTarea = If(solicitud Is Nothing, 0, solicitud.IdTarea),
            .Termino = If(solicitud Is Nothing OrElse solicitud.Termino Is Nothing, String.Empty, solicitud.Termino.Trim()),
            .Pagina = If(solicitud Is Nothing OrElse solicitud.Pagina < 1, 1, solicitud.Pagina),
            .TamanoPagina = If(solicitud Is Nothing OrElse solicitud.TamanoPagina < 1, 25, Math.Min(50, solicitud.TamanoPagina))
        }
        If normalizada.IdTarea <= 0 Then
            Return CrearError(CodigosBloqueoPrevisualizacion.TareaInvalida, "La tarea seleccionada no es valida.")
        End If
        If normalizada.Termino.Length = 1 OrElse normalizada.Termino.Length > 80 Then
            Return CrearError(CodigosBloqueoPrevisualizacion.BusquedaTerminoInvalido,
                              "Escriba al menos dos caracteres y un maximo de ochenta para buscar.")
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
