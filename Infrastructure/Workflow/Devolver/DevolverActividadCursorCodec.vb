Imports System
Imports System.Globalization
Imports System.Text
Imports System.Web.Security

'Protege la continuación del preview contra manipulación y contra reutilización en otra tarea, sesión o filtro.
Public Class DevolverActividadCursorCodec
    Implements IDevolverActividadCursorCodec

    Public Function Proteger(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaDevolverActividad,
                             ByVal terminoNormalizado As String,
                             ByVal destino As DestinoDevolverActividad) As String Implements IDevolverActividadCursorCodec.Proteger
        If Not EsContextoValido(contexto, tarea) OrElse destino Is Nothing OrElse destino.IdConector <= 0 OrElse destino.Orden <= 0 OrElse
           Not String.Equals(destino.TipoContexto, tarea.TipoContexto, StringComparison.OrdinalIgnoreCase) Then
            Return String.Empty
        End If

        Dim contenido As String = String.Join(ChrW(31), New String() {
            tarea.IdTarea.ToString(CultureInfo.InvariantCulture),
            tarea.TokenVersion,
            contexto.IdUsuarioWorkflow.ToString(CultureInfo.InvariantCulture),
            contexto.IdGrupoWorkflow.ToString(CultureInfo.InvariantCulture),
            contexto.IdRutaWorkflow.ToString(CultureInfo.InvariantCulture),
            tarea.TipoContexto.ToUpperInvariant(),
            If(terminoNormalizado, String.Empty),
            destino.Orden.ToString(CultureInfo.InvariantCulture),
            destino.IdConector.ToString(CultureInfo.InvariantCulture)})
        Try
            Dim protegido As Byte() = MachineKey.Protect(Encoding.UTF8.GetBytes(contenido), "WorkflowDevolverActividadCursor", "v1")
            If protegido Is Nothing OrElse protegido.Length = 0 Then Return String.Empty
            Return Convert.ToBase64String(protegido).TrimEnd("="c).Replace("+", "-").Replace("/", "_")
        Catch
            Return String.Empty
        End Try
    End Function

    Public Function Validar(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaDevolverActividad,
                            ByVal terminoNormalizado As String,
                            ByVal cursor As String,
                            ByRef idConector As Integer,
                            ByRef orden As Integer) As Boolean Implements IDevolverActividadCursorCodec.Validar
        idConector = 0
        orden = 0
        If Not EsContextoValido(contexto, tarea) OrElse String.IsNullOrWhiteSpace(cursor) Then Return False

        Try
            Dim base64 As String = cursor.Replace("-", "+").Replace("_", "/")
            base64 = base64.PadRight(base64.Length + ((4 - base64.Length Mod 4) Mod 4), "="c)
            Dim contenidoProtegido As Byte() = Convert.FromBase64String(base64)
            Dim contenido As Byte() = MachineKey.Unprotect(contenidoProtegido, "WorkflowDevolverActividadCursor", "v1")
            If contenido Is Nothing Then Return False

            Dim partes As String() = Encoding.UTF8.GetString(contenido).Split(New Char() {ChrW(31)})
            If partes.Length <> 9 Then Return False
            Dim idTarea As Long = 0
            Dim idUsuario As Integer = 0
            Dim idGrupo As Integer = 0
            Dim idRuta As Integer = 0
            If Not Long.TryParse(partes(0), NumberStyles.None, CultureInfo.InvariantCulture, idTarea) OrElse
               Not Integer.TryParse(partes(2), NumberStyles.None, CultureInfo.InvariantCulture, idUsuario) OrElse
               Not Integer.TryParse(partes(3), NumberStyles.None, CultureInfo.InvariantCulture, idGrupo) OrElse
               Not Integer.TryParse(partes(4), NumberStyles.None, CultureInfo.InvariantCulture, idRuta) OrElse
               Not Integer.TryParse(partes(7), NumberStyles.None, CultureInfo.InvariantCulture, orden) OrElse
               Not Integer.TryParse(partes(8), NumberStyles.None, CultureInfo.InvariantCulture, idConector) OrElse
               idTarea <> tarea.IdTarea OrElse idUsuario <> contexto.IdUsuarioWorkflow OrElse
               idGrupo <> contexto.IdGrupoWorkflow OrElse idRuta <> contexto.IdRutaWorkflow OrElse
               orden <= 0 OrElse idConector <= 0 Then
                idConector = 0
                orden = 0
                Return False
            End If
            Return String.Equals(partes(1), tarea.TokenVersion, StringComparison.Ordinal) AndAlso
                   String.Equals(partes(5), tarea.TipoContexto, StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(partes(6), If(terminoNormalizado, String.Empty), StringComparison.Ordinal)
        Catch
            idConector = 0
            orden = 0
            Return False
        End Try
    End Function

    Private Shared Function EsContextoValido(ByVal contexto As ContextoModuloWorkflow,
                                              ByVal tarea As TareaDevolverActividad) As Boolean
        Return contexto IsNot Nothing AndAlso contexto.EsValido() AndAlso tarea IsNot Nothing AndAlso tarea.EstaActiva AndAlso
               tarea.IdTarea > 0 AndAlso Not String.IsNullOrWhiteSpace(tarea.TokenVersion) AndAlso
               Not String.IsNullOrWhiteSpace(tarea.TipoContexto)
    End Function
End Class
