Imports System
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text

Public NotInheritable Class CondicionesHttpDocumento
    Private Sub New()
    End Sub

    Public Shared Function CrearETag(ByVal referencia As String,
                                    ByVal longitud As Long,
                                    ByVal ultimaModificacionUtc As DateTime) As String
        Dim material As String = String.Join("|", New String() {
            If(referencia, String.Empty),
            longitud.ToString(CultureInfo.InvariantCulture),
            NormalizarHttpDate(ultimaModificacionUtc).Ticks.ToString(CultureInfo.InvariantCulture)})
        Using sha As SHA256 = SHA256.Create()
            Dim hash As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(material))
            Dim texto As New StringBuilder(hash.Length * 2)
            For Each value As Byte In hash
                texto.Append(value.ToString("x2", CultureInfo.InvariantCulture))
            Next
            Return """" & texto.ToString() & """"
        End Using
    End Function

    Public Shared Function EsNoModificado(ByVal ifNoneMatch As String,
                                          ByVal ifModifiedSince As String,
                                          ByVal etag As String,
                                          ByVal ultimaModificacionUtc As DateTime) As Boolean
        If Not String.IsNullOrWhiteSpace(ifNoneMatch) Then
            For Each candidato As String In ifNoneMatch.Split(","c)
                Dim valor As String = candidato.Trim()
                If valor = "*" OrElse CompararEtagDebil(valor, etag) Then Return True
            Next
            Return False
        End If

        Dim fecha As DateTime
        Return TryParseHttpDate(ifModifiedSince, fecha) AndAlso NormalizarHttpDate(ultimaModificacionUtc) <= fecha
    End Function

    Public Shared Function IfRangePermiteRango(ByVal ifRange As String,
                                               ByVal etag As String,
                                               ByVal ultimaModificacionUtc As DateTime) As Boolean
        If String.IsNullOrWhiteSpace(ifRange) Then Return True
        Dim valor As String = ifRange.Trim()
        If valor.StartsWith("W/", StringComparison.OrdinalIgnoreCase) Then Return False
        If valor.StartsWith("""", StringComparison.Ordinal) Then
            Return String.Equals(valor, etag, StringComparison.Ordinal)
        End If
        Dim fecha As DateTime
        Return TryParseHttpDate(valor, fecha) AndAlso NormalizarHttpDate(ultimaModificacionUtc) <= fecha
    End Function

    Public Shared Function NormalizarHttpDate(ByVal value As DateTime) As DateTime
        Dim utc As DateTime = If(value.Kind = DateTimeKind.Utc, value, value.ToUniversalTime())
        Return New DateTime(utc.Ticks - (utc.Ticks Mod TimeSpan.TicksPerSecond), DateTimeKind.Utc)
    End Function

    Private Shared Function CompararEtagDebil(ByVal left As String, ByVal right As String) As Boolean
        Return String.Equals(QuitarDebil(left), QuitarDebil(right), StringComparison.Ordinal)
    End Function

    Private Shared Function QuitarDebil(ByVal value As String) As String
        Dim resultado As String = If(value, String.Empty).Trim()
        If resultado.StartsWith("W/", StringComparison.OrdinalIgnoreCase) Then resultado = resultado.Substring(2).Trim()
        Return resultado
    End Function

    Private Shared Function TryParseHttpDate(ByVal value As String, ByRef result As DateTime) As Boolean
        result = DateTime.MinValue
        If String.IsNullOrWhiteSpace(value) Then Return False
        Dim parsed As DateTimeOffset
        If Not DateTimeOffset.TryParseExact(value.Trim(), "r", CultureInfo.InvariantCulture,
                                            DateTimeStyles.AllowWhiteSpaces Or DateTimeStyles.AssumeUniversal,
                                            parsed) Then Return False
        result = parsed.UtcDateTime
        Return True
    End Function
End Class
