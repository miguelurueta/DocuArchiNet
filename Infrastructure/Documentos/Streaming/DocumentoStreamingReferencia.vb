Imports System
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Web.Security

Public NotInheritable Class DocumentoStreamingAutorizado
    Public Property Ruta As String
    Public Property Nombre As String
    Public Property TipoContenido As String
    Public Property ExpiraUtc As DateTime
End Class

' Referencia opaca para documentos ya autorizados por el flujo WebForms que emite la URL.
Public Class DocumentoStreamingReferencia
    Private Const VersionActual As String = "v1"
    Private Const DuracionMinutos As Integer = 5
    Private Const LongitudMaximaToken As Integer = 8192

    Public Function Emitir(ByVal ruta As String,
                           ByVal nombre As String,
                           ByVal tipoContenido As String,
                           ByVal sessionId As String) As String
        Return Emitir(ruta, nombre, tipoContenido, sessionId, DateTime.UtcNow.AddMinutes(DuracionMinutos))
    End Function

    Public Function Emitir(ByVal ruta As String,
                           ByVal nombre As String,
                           ByVal tipoContenido As String,
                           ByVal sessionId As String,
                           ByVal expiraUtc As DateTime) As String
        If String.IsNullOrWhiteSpace(ruta) OrElse String.IsNullOrWhiteSpace(sessionId) OrElse
           ContieneTraversal(ruta) Then Return String.Empty

        Try
            Dim rutaCanonica As String = Path.GetFullPath(ruta)
            Dim nombreSeguro As String = Path.GetFileName(If(String.IsNullOrWhiteSpace(nombre), rutaCanonica, nombre))
            If String.IsNullOrWhiteSpace(nombreSeguro) Then Return String.Empty
            Dim mimeSeguro As String = If(String.IsNullOrWhiteSpace(tipoContenido), "application/octet-stream", tipoContenido.Trim())
            Dim vencimiento As DateTime = NormalizarUtc(expiraUtc)
            If vencimiento <= DateTime.UtcNow Then Return String.Empty

            Dim contenido As String = String.Join(ChrW(31), New String() {
                VersionActual,
                vencimiento.Ticks.ToString(CultureInfo.InvariantCulture),
                CodificarCampo(sessionId),
                CodificarCampo(rutaCanonica),
                CodificarCampo(nombreSeguro),
                CodificarCampo(mimeSeguro)})
            Dim protegido As Byte() = MachineKey.Protect(Encoding.UTF8.GetBytes(contenido), "DocumentoStreamingReferencia", VersionActual)
            If protegido Is Nothing OrElse protegido.Length = 0 Then Return String.Empty
            Return CodificarBase64Url(protegido)
        Catch
            Return String.Empty
        End Try
    End Function

    Public Function TryResolver(ByVal token As String,
                                ByVal sessionId As String,
                                ByRef documento As DocumentoStreamingAutorizado) As Boolean
        Return TryResolver(token, sessionId, DateTime.UtcNow, documento)
    End Function

    Public Function TryResolver(ByVal token As String,
                                ByVal sessionId As String,
                                ByVal ahoraUtc As DateTime,
                                ByRef documento As DocumentoStreamingAutorizado) As Boolean
        documento = Nothing
        If String.IsNullOrWhiteSpace(token) OrElse token.Length > LongitudMaximaToken OrElse
           String.IsNullOrWhiteSpace(sessionId) Then Return False

        Try
            Dim protegido As Byte() = DecodificarBase64Url(token)
            Dim contenido As Byte() = MachineKey.Unprotect(protegido, "DocumentoStreamingReferencia", VersionActual)
            If contenido Is Nothing OrElse contenido.Length = 0 Then Return False
            Dim partes As String() = Encoding.UTF8.GetString(contenido).Split(New Char() {ChrW(31)})
            If partes.Length <> 6 OrElse Not String.Equals(partes(0), VersionActual, StringComparison.Ordinal) Then Return False

            Dim ticks As Long
            If Not Long.TryParse(partes(1), NumberStyles.None, CultureInfo.InvariantCulture, ticks) Then Return False
            Dim expira As New DateTime(ticks, DateTimeKind.Utc)
            If expira <= NormalizarUtc(ahoraUtc) Then Return False

            Dim sesionToken As String = DecodificarCampo(partes(2))
            If Not ComparacionConstante(sesionToken, sessionId) Then Return False

            Dim ruta As String = DecodificarCampo(partes(3))
            If String.IsNullOrWhiteSpace(ruta) OrElse ContieneTraversal(ruta) Then Return False
            Dim rutaCanonica As String = Path.GetFullPath(ruta)
            If Not String.Equals(ruta, rutaCanonica, StringComparison.OrdinalIgnoreCase) OrElse Not File.Exists(rutaCanonica) Then Return False

            documento = New DocumentoStreamingAutorizado With {
                .Ruta = rutaCanonica,
                .Nombre = Path.GetFileName(DecodificarCampo(partes(4))),
                .TipoContenido = DecodificarCampo(partes(5)),
                .ExpiraUtc = expira
            }
            Return Not String.IsNullOrWhiteSpace(documento.Nombre) AndAlso Not String.IsNullOrWhiteSpace(documento.TipoContenido)
        Catch
            documento = Nothing
            Return False
        End Try
    End Function

    Private Shared Function CodificarCampo(ByVal value As String) As String
        Return CodificarBase64Url(Encoding.UTF8.GetBytes(If(value, String.Empty)))
    End Function

    Private Shared Function DecodificarCampo(ByVal value As String) As String
        Return Encoding.UTF8.GetString(DecodificarBase64Url(value))
    End Function

    Private Shared Function CodificarBase64Url(ByVal value As Byte()) As String
        Return Convert.ToBase64String(value).TrimEnd("="c).Replace("+", "-").Replace("/", "_")
    End Function

    Private Shared Function DecodificarBase64Url(ByVal value As String) As Byte()
        Dim base64 As String = value.Replace("-", "+").Replace("_", "/")
        base64 = base64.PadRight(base64.Length + ((4 - base64.Length Mod 4) Mod 4), "="c)
        Return Convert.FromBase64String(base64)
    End Function

    Private Shared Function NormalizarUtc(ByVal value As DateTime) As DateTime
        If value.Kind = DateTimeKind.Utc Then Return value
        Return value.ToUniversalTime()
    End Function

    Private Shared Function ContieneTraversal(ByVal ruta As String) As Boolean
        Dim segmentos As String() = ruta.Replace("/", "\").Split("\"c)
        Return Array.Exists(segmentos, Function(segmento) String.Equals(segmento, "..", StringComparison.Ordinal))
    End Function

    Private Shared Function ComparacionConstante(ByVal left As String, ByVal right As String) As Boolean
        If left Is Nothing OrElse right Is Nothing OrElse left.Length <> right.Length Then Return False
        Dim diferencia As Integer = 0
        For indice As Integer = 0 To left.Length - 1
            diferencia = diferencia Or (AscW(left(indice)) Xor AscW(right(indice)))
        Next
        Return diferencia = 0
    End Function
End Class
