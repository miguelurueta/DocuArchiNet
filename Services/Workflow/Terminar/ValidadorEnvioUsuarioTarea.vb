Imports System

'Valida y normaliza solicitudes exclusivas de Enviar a usuario antes de consultar o ejecutar.
Public Class ValidadorEnvioUsuarioTarea
    Private Const TamanoPaginaPredeterminado As Integer = 25
    Private Const TamanoPaginaMaximo As Integer = 50
    Private Const LongitudConsultaMaxima As Integer = 100
    Private Const LongitudCursorMaxima As Integer = 1024
    Private Const LongitudTokenMaxima As Integer = 128

    Public Function NormalizarPreview(ByVal solicitud As SolicitudPreviewEnvioUsuario,
                                      ByRef normalizada As SolicitudPreviewEnvioUsuario) As ErrorTransicionDto
        normalizada = New SolicitudPreviewEnvioUsuario()
        If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 Then
            Return CrearError(CodigosBloqueoPrevisualizacion.TareaInvalida,
                         "La tarea solicitada no es valida.")
        End If

        Dim consulta As String = If(solicitud.Consulta, String.Empty).Trim()
        If consulta.Length > LongitudConsultaMaxima OrElse ContieneControl(consulta) Then
            Return CrearError(CodigosBloqueoPrevisualizacion.BusquedaUsuarioTerminoInvalido,
                         "El filtro de destinos no es valido.")
        End If

        Dim cursor As String = If(solicitud.Cursor, String.Empty).Trim()
        If cursor.Length > LongitudCursorMaxima OrElse (cursor.Length > 0 AndAlso
            Not System.Text.RegularExpressions.Regex.IsMatch(cursor, "^[A-Za-z0-9_-]+$")) Then
            Return CrearError(CodigosBloqueoPrevisualizacion.CursorUsuarioInvalido,
                         "El cursor de destinos no es valido.")
        End If

        Dim tamanoPagina As Integer = If(solicitud.TamanoPagina = 0, TamanoPaginaPredeterminado, solicitud.TamanoPagina)
        If tamanoPagina < 1 OrElse tamanoPagina > TamanoPaginaMaximo Then
            Return CrearError(CodigosBloqueoPrevisualizacion.TareaInvalida,
                         "El tamaño de pagina solicitado no es valido.")
        End If

        normalizada.IdTarea = solicitud.IdTarea
        normalizada.Consulta = consulta
        normalizada.Cursor = cursor
        normalizada.TamanoPagina = tamanoPagina
        Return Nothing
    End Function

    Public Function ValidarEjecucion(ByVal solicitud As SolicitudEnvioUsuarioWorkflow) As ErrorTransicionDto
        If solicitud Is Nothing OrElse solicitud.IdTarea <= 0 Then
            Return CrearError(CodigosBloqueoPrevisualizacion.TareaInvalida,
                         "La tarea solicitada no es valida.")
        End If
        If solicitud.IdUsuarioWorkflowDestino <= 0 OrElse solicitud.IdActividadDestino <= 0 Then
            Return CrearError(CodigosBloqueoPrevisualizacion.UsuarioDestinoInvalido,
                         "El destino seleccionado no es valido.")
        End If
        Dim token As String = If(solicitud.TokenVersion, String.Empty).Trim()
        If token.Length = 0 OrElse token.Length > LongitudTokenMaxima OrElse ContieneControl(token) Then
            Return CrearError(CodigosBloqueoPrevisualizacion.VersionInvalida,
                         "La version de la tarea no es valida.")
        End If
        Return Nothing
    End Function

    Private Shared Function ContieneControl(ByVal valor As String) As Boolean
        For Each caracter As Char In If(valor, String.Empty)
            If Char.IsControl(caracter) Then Return True
        Next
        Return False
    End Function

    Private Shared Function CrearError(ByVal codigo As String, ByVal mensaje As String) As ErrorTransicionDto
        Return New ErrorTransicionDto With {
            .Codigo = codigo,
            .MensajeVisible = mensaje,
            .ReferenciaTrazabilidad = String.Empty
        }
    End Function
End Class
