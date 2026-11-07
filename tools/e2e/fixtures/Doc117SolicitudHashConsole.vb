Option Strict On
Imports System
Imports Newtonsoft.Json

' Offline stdin -> hashes only; no context/payload is written to disk or stdout.
Public Class Doc117HashEntrada
    Public Property Contexto As ContextoImportacionCarpetas
    Public Property Solicitudes As Doc117HashSolicitud()
End Class
Public Class Doc117HashSolicitud
    Public Property LoteId As String
    Public Property ArchivoId As String
    Public Property RevisionEsperada As UInteger
    Public Property ClaveOperacion As String
    Public Property Fase As String
End Class
Public Module Doc117SolicitudHashConsole
    Public Function Main() As Integer
        Try
            Dim texto = Console.In.ReadToEnd()
            If texto.Length > 32768 Then Throw New InvalidOperationException()
            Dim entrada = JsonConvert.DeserializeObject(Of Doc117HashEntrada)(texto, New JsonSerializerSettings With {.MissingMemberHandling = MissingMemberHandling.Error})
            If entrada Is Nothing OrElse entrada.Contexto Is Nothing OrElse entrada.Solicitudes Is Nothing OrElse entrada.Solicitudes.Length <> 2 Then Throw New InvalidOperationException()
            Dim hashes(1) As String
            For n As Integer = 0 To 1
                Dim s = entrada.Solicitudes(n)
                If s Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(s.LoteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(s.ArchivoId) OrElse s.RevisionEsperada = 0 OrElse String.IsNullOrEmpty(s.ClaveOperacion) OrElse s.ClaveOperacion.Length > 64 OrElse s.Fase <> "ALMACENAR" Then Throw New InvalidOperationException()
                hashes(n) = Doc117SolicitudHash.Calcular(entrada.Contexto, s.LoteId, s.ArchivoId, s.RevisionEsperada, s.ClaveOperacion, s.Fase)
            Next
            Console.WriteLine(JsonConvert.SerializeObject(hashes))
            Return 0
        Catch
            Console.Error.WriteLine("DOC117_HASH_INPUT_REJECTED")
            Return 2
        End Try
    End Function
End Module
