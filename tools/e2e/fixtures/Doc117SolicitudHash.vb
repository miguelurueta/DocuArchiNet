Option Strict On
Option Compare Binary
Imports System
Imports System.Text
Imports System.Security.Cryptography

' Laboratory-only calculator. Must match CoordinadorImportacionCarpetas.Documento.vb.
' Not referenced from or included in the production project.
Public NotInheritable Class Doc117SolicitudHash
    Private Sub New()
    End Sub
    Public Shared Function Calcular(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, revisionEsperada As UInteger, claveOperacion As String, fase As String) As String
        Using sha = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(New With {.contexto = contexto, .lote = loteId, .archivo = archivoId, .revision = revisionEsperada, .clave = claveOperacion, .fase = fase})))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
End Class
