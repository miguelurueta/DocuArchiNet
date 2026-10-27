Option Strict On
Imports System
Imports System.Security.Cryptography
Imports System.Web

Public NotInheritable Class PerfilImportacionRequestGuard
    Private Const ClaveToken As String = "DOC110_CSRF_TOKEN"
    Private Const ClaveContexto As String = "DOC110_CSRF_CONTEXTO"
    Private Sub New()
    End Sub

    Public Shared Function Emitir(http As HttpContext, contexto As ContextoPerfilImportacion) As String
        ExigirContexto(http, contexto)
        Dim vinculo = VinculoContexto(http, contexto)
        Dim token = TryCast(http.Session(ClaveToken), String)
        If Not String.Equals(TryCast(http.Session(ClaveContexto), String), vinculo, StringComparison.Ordinal) OrElse Not TokenValido(token) Then
            Dim bytes(31) As Byte
            Using random = RandomNumberGenerator.Create()
                random.GetBytes(bytes)
            End Using
            token = Convert.ToBase64String(bytes)
            http.Session(ClaveToken) = token
            http.Session(ClaveContexto) = vinculo
        End If
        http.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        http.Response.Cache.SetNoStore()
        Return token
    End Function

    Public Shared Sub Validar(http As HttpContext, contexto As ContextoPerfilImportacion)
        ExigirContexto(http, contexto)
        If http.Request.HttpMethod <> "POST" OrElse Not String.Equals(http.Request.ContentType.Split(";"c)(0).Trim(), "application/json", StringComparison.OrdinalIgnoreCase) Then Throw Rechazo()
        If http.Request.ContentLength < 0 OrElse http.Request.ContentLength > PerfilImportacionJson.MaximoCaracteres * 2 Then Throw Rechazo()
        Dim origin = http.Request.Headers("Origin")
        If Not String.IsNullOrEmpty(origin) Then
            Dim origen As Uri = Nothing
            If Not Uri.TryCreate(origin, UriKind.Absolute, origen) OrElse origen.AbsolutePath <> "/" OrElse origen.Query <> "" OrElse origen.Fragment <> "" OrElse Not String.Equals(origen.GetLeftPart(UriPartial.Authority), http.Request.Url.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase) Then Throw Rechazo()
        End If
        If Not String.Equals(TryCast(http.Session(ClaveContexto), String), VinculoContexto(http, contexto), StringComparison.Ordinal) Then Throw Rechazo()
        If Not CompararTokens(TryCast(http.Session(ClaveToken), String), http.Request.Headers("X-Importacion-Perfil-CSRF")) Then Throw Rechazo()
    End Sub

    Public Shared Function CompararTokens(esperado As String, recibido As String) As Boolean
        If Not TokenValido(esperado) OrElse Not TokenValido(recibido) Then Return False
        Dim a = Convert.FromBase64String(esperado)
        Dim b = Convert.FromBase64String(recibido)
        Dim diferencia As Integer = 0
        For i = 0 To 31
            diferencia = diferencia Or (CInt(a(i)) Xor CInt(b(i)))
        Next
        Return diferencia = 0
    End Function

    Private Shared Function TokenValido(token As String) As Boolean
        If token Is Nothing OrElse token.Length <> 44 Then Return False
        Try
            Dim bytes = Convert.FromBase64String(token)
            Return bytes.Length = 32 AndAlso Convert.ToBase64String(bytes) = token
        Catch ex As FormatException
            Return False
        End Try
    End Function
    Private Shared Function VinculoContexto(http As HttpContext, contexto As ContextoPerfilImportacion) As String
        Return http.Session.SessionID & ":" & contexto.UsuarioRef & ":" & contexto.AmbitoRef
    End Function
    Private Shared Sub ExigirContexto(http As HttpContext, contexto As ContextoPerfilImportacion)
        If http Is Nothing OrElse http.Session Is Nothing OrElse contexto Is Nothing OrElse Not contexto.PuedeAdministrar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) Then Throw Rechazo()
    End Sub
    Private Shared Function Rechazo() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_NO_VERIFICADA", "Recargue el contexto autenticado antes de continuar.")
    End Function
End Class
