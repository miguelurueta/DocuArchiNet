Imports System
Imports System.Globalization

Public Enum ResultadoRangoHttp
    SinRango = 0
    Valido = 1
    Invalido = 2
End Enum

Public NotInheritable Class IntervaloBytesDocumento
    Public Property Inicio As Long
    Public Property Fin As Long

    Public ReadOnly Property Longitud As Long
        Get
            Return Fin - Inicio + 1L
        End Get
    End Property
End Class

Public NotInheritable Class RangoHttpDocumento
    Private Sub New()
    End Sub

    Public Shared Function Resolver(ByVal headerRange As String,
                                    ByVal longitudTotal As Long,
                                    ByRef intervalo As IntervaloBytesDocumento) As ResultadoRangoHttp
        intervalo = Nothing
        If String.IsNullOrWhiteSpace(headerRange) Then Return ResultadoRangoHttp.SinRango
        If longitudTotal <= 0L Then Return ResultadoRangoHttp.Invalido

        Dim partesUnidad As String() = headerRange.Trim().Split(New Char() {"="c}, 2)
        If partesUnidad.Length <> 2 OrElse Not String.Equals(partesUnidad(0).Trim(), "bytes", StringComparison.OrdinalIgnoreCase) Then
            Return ResultadoRangoHttp.Invalido
        End If
        Dim expresion As String = partesUnidad(1).Trim()
        If expresion.Length = 0 OrElse expresion.Contains(",") Then Return ResultadoRangoHttp.Invalido
        Dim limites As String() = expresion.Split(New Char() {"-"c}, 2)
        If limites.Length <> 2 Then Return ResultadoRangoHttp.Invalido

        Dim inicio As Long
        Dim fin As Long
        If limites(0).Length = 0 Then
            Dim sufijo As Long
            If Not Long.TryParse(limites(1), NumberStyles.None, CultureInfo.InvariantCulture, sufijo) OrElse sufijo <= 0L Then
                Return ResultadoRangoHttp.Invalido
            End If
            inicio = Math.Max(0L, longitudTotal - sufijo)
            fin = longitudTotal - 1L
        Else
            If Not Long.TryParse(limites(0), NumberStyles.None, CultureInfo.InvariantCulture, inicio) OrElse
               inicio < 0L OrElse inicio >= longitudTotal Then Return ResultadoRangoHttp.Invalido
            If limites(1).Length = 0 Then
                fin = longitudTotal - 1L
            ElseIf Not Long.TryParse(limites(1), NumberStyles.None, CultureInfo.InvariantCulture, fin) OrElse fin < inicio Then
                Return ResultadoRangoHttp.Invalido
            Else
                fin = Math.Min(fin, longitudTotal - 1L)
            End If
        End If

        intervalo = New IntervaloBytesDocumento With {.Inicio = inicio, .Fin = fin}
        Return ResultadoRangoHttp.Valido
    End Function
End Class
