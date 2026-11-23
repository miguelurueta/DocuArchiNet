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
        ValidarOrigen(http)
    End Sub
    Private Shared Sub ValidarOrigen(http As HttpContext)
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
        ValidarToken(http, ctx)
    End Sub
    Private Shared Sub ValidarToken(http As HttpContext, ctx As ContextoImportacionExcel)
        If Not String.Equals(TryCast(http.Session(ClaveContexto), String), Vinculo(http, ctx), StringComparison.Ordinal) OrElse Not PerfilImportacionRequestGuard.CompararTokens(TryCast(http.Session(ClaveToken), String), http.Request.Headers("X-Importacion-Excel-CSRF")) Then Throw Rechazo()
    End Sub
    'Se invoca antes de acceder a Form/Files: ASP.NET puede almacenar el cuerpo en buffer.
    Public Shared Sub ValidarMultipart(http As HttpContext, ctx As ContextoImportacionExcel, limites As LimitesLecturaExcel)
        ServicioPlantillasExcel.Autorizar(ctx)
        ReglasCargaExcel.Limites(limites)
        If http Is Nothing OrElse http.Request.HttpMethod <> "POST" Then Throw Rechazo()
        Dim tipo = http.Request.ContentType
        If Not Text.RegularExpressions.Regex.IsMatch(If(tipo, ""), "\Amultipart/form-data;\s*boundary=(?:[A-Za-z0-9'()+_,./:=?-]{1,70}|""[A-Za-z0-9'()+_,./:=?-]{1,70}"")\z", Text.RegularExpressions.RegexOptions.IgnoreCase Or Text.RegularExpressions.RegexOptions.CultureInvariant) Then Throw Rechazo()
        If http.Request.ContentLength <= 0 OrElse CLng(http.Request.ContentLength) > CLng(limites.MaxArchivoMB) * 1000000L + MaximoBytesSolicitud Then Throw Rechazo()
        ValidarOrigen(http)
        ValidarToken(http, ctx)
    End Sub
    Public Shared Function LeerMultipart(http As HttpContext) As SolicitudRecepcionExcel
        Dim requeridos = New String() {"idCarga", "referenciaRecepcion", "conexionId", "plantillaId"}
        If http.Request.Form.Count <> requeridos.Length OrElse http.Request.Files.Count <> 1 OrElse http.Request.Files.AllKeys(0) <> "archivo" Then Throw Rechazo()
        For Each clave In requeridos
            Dim valores = http.Request.Form.GetValues(clave)
            If valores Is Nothing OrElse valores.Length <> 1 OrElse String.IsNullOrEmpty(valores(0)) OrElse valores(0).Length > 8192 Then Throw Rechazo()
        Next
        Dim conexion, plantilla As UInteger
        If Not UInteger.TryParse(http.Request.Form("conexionId"), Globalization.NumberStyles.None, Globalization.CultureInfo.InvariantCulture, conexion) OrElse conexion = 0 OrElse Not UInteger.TryParse(http.Request.Form("plantillaId"), Globalization.NumberStyles.None, Globalization.CultureInfo.InvariantCulture, plantilla) OrElse plantilla = 0 OrElse Not ReglasCargaExcel.IdValido(http.Request.Form("idCarga")) Then Throw Rechazo()
        Return New SolicitudRecepcionExcel With {.IdCarga = http.Request.Form("idCarga"), .ReferenciaRecepcion = http.Request.Form("referenciaRecepcion"), .ConexionId = conexion, .PlantillaId = plantilla}
    End Function
    Private Shared Function Vinculo(http As HttpContext, ctx As ContextoImportacionExcel) As String
        ServicioPlantillasExcel.Autorizar(ctx)
        If http Is Nothing OrElse http.Session Is Nothing Then Throw Rechazo()
        Return http.Session.SessionID & ":" & ctx.UsuarioId.ToString(Globalization.CultureInfo.InvariantCulture) & ":" & ctx.EmpresaId.ToString(Globalization.CultureInfo.InvariantCulture)
    End Function
    Private Shared Function Rechazo() As ImportacionExcelException
        Return ServicioPlantillasExcel.ErrorControlado("SOLICITUD_NO_VERIFICADA")
    End Function
End Class
