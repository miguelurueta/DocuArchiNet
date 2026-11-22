Option Strict On
Imports System
Imports System.Security.Cryptography
Imports System.Web

Public NotInheritable Class ImportacionExcelRequestGuard
    Private Const ClaveToken As String = "DOC122_EXCEL_CSRF"
    Private Const ClaveContexto As String = "DOC122_EXCEL_CONTEXTO"
    Public Const MaximoBytesSolicitud As Integer = 262144
    Private Sub New()
    End Sub
    Public Shared Sub ValidarTransporte(http As HttpContext)
        If http Is Nothing OrElse http.Request.HttpMethod <> "POST" OrElse Not String.Equals(http.Request.ContentType.Split(";"c)(0).Trim(), "application/json", StringComparison.OrdinalIgnoreCase) OrElse http.Request.ContentLength < 0 OrElse http.Request.ContentLength > MaximoBytesSolicitud Then Throw Rechazo()
        Dim origin = http.Request.Headers("Origin")
        If Not String.IsNullOrEmpty(origin) Then
            Dim u As Uri = Nothing
            If Not Uri.TryCreate(origin, UriKind.Absolute, u) OrElse u.AbsolutePath <> "/" OrElse u.Query <> "" OrElse u.Fragment <> "" OrElse u.UserInfo <> "" OrElse Not String.Equals(u.GetLeftPart(UriPartial.Authority), http.Request.Url.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase) Then Throw Rechazo()
        End If
        http.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        http.Response.Cache.SetNoStore()
    End Sub
    Public Shared Function Emitir(http As HttpContext, ctx As ContextoImportacionExcel) As String
        ValidarTransporte(http)
        Dim identidad = Vinculo(http, ctx)
        Dim token = TryCast(http.Session(ClaveToken), String)
        If Not String.Equals(TryCast(http.Session(ClaveContexto), String), identidad, StringComparison.Ordinal) OrElse Not PerfilImportacionRequestGuard.CompararTokens(token, token) Then
            Dim bytes(31) As Byte
            Using random = RandomNumberGenerator.Create()
                random.GetBytes(bytes)
            End Using
            token = Convert.ToBase64String(bytes)
            http.Session(ClaveToken) = token
            http.Session(ClaveContexto) = identidad
        End If
        Return token
    End Function
    Public Shared Sub Validar(http As HttpContext, ctx As ContextoImportacionExcel)
        ValidarTransporte(http)
        If Not String.Equals(TryCast(http.Session(ClaveContexto), String), Vinculo(http, ctx), StringComparison.Ordinal) OrElse Not PerfilImportacionRequestGuard.CompararTokens(TryCast(http.Session(ClaveToken), String), http.Request.Headers("X-Importacion-Excel-CSRF")) Then Throw Rechazo()
    End Sub
    Private Shared Function Vinculo(http As HttpContext, ctx As ContextoImportacionExcel) As String
        ServicioPlantillasExcel.Autorizar(ctx)
        If http Is Nothing OrElse http.Session Is Nothing Then Throw Rechazo()
        Return http.Session.SessionID & ":" & ctx.UsuarioId.ToString(Globalization.CultureInfo.InvariantCulture) & ":" & ctx.EmpresaId.ToString(Globalization.CultureInfo.InvariantCulture)
    End Function
    Private Shared Function Rechazo() As ImportacionExcelException
        Return ServicioPlantillasExcel.ErrorControlado("SOLICITUD_NO_VERIFICADA")
    End Function
End Class
