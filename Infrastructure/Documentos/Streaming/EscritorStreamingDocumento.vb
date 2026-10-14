Imports System
Imports System.IO

Public NotInheritable Class EscritorStreamingDocumento
    Public Const TamanoBuffer As Integer = 65536

    Private Sub New()
    End Sub

    Public Shared Function Copiar(ByVal origen As Stream,
                                  ByVal destino As Stream,
                                  ByVal inicio As Long,
                                  ByVal longitud As Long,
                                  Optional ByVal clienteConectado As Func(Of Boolean) = Nothing) As Long
        If origen Is Nothing Then Throw New ArgumentNullException("origen")
        If destino Is Nothing Then Throw New ArgumentNullException("destino")
        If Not origen.CanRead OrElse Not origen.CanSeek Then Throw New ArgumentException("El origen debe permitir lectura y posicionamiento.", "origen")
        If Not destino.CanWrite Then Throw New ArgumentException("El destino debe permitir escritura.", "destino")
        If inicio < 0L OrElse longitud < 0L OrElse inicio > origen.Length OrElse longitud > origen.Length - inicio Then
            Throw New ArgumentOutOfRangeException("longitud")
        End If

        origen.Position = inicio
        Dim restante As Long = longitud
        Dim totalEscrito As Long = 0L
        Dim buffer(TamanoBuffer - 1) As Byte
        While restante > 0L
            If clienteConectado IsNot Nothing AndAlso Not clienteConectado() Then Exit While
            Dim solicitado As Integer = CInt(Math.Min(CLng(buffer.Length), restante))
            Dim leidos As Integer = origen.Read(buffer, 0, solicitado)
            If leidos <= 0 Then Throw New EndOfStreamException("El archivo terminó antes del intervalo declarado.")
            destino.Write(buffer, 0, leidos)
            restante -= leidos
            totalEscrito += leidos
        End While
        Return totalEscrito
    End Function
End Class
