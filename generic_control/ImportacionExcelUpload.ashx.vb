Option Strict On
Imports System
Imports System.Text
Imports System.Web
Imports System.Web.SessionState
Imports Newtonsoft.Json

Public Class ImportacionExcelUpload
    Implements IHttpHandler, IRequiresSessionState
    Public Sub ProcessRequest(http As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim salida As New RespuestaEstadoCargaExcel With {.operacionId = Guid.NewGuid().ToString("N")}
        http.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        http.Response.Cache.SetNoStore()
        Try
            Dim c = New ImportacionExcelContextAdapter().Resolver(http)
            Dim adaptador As New ImportacionExcelConexionAdapter
            adaptador.ValidarModulo(http, c)
            Dim limites As New LimitesLecturaExcel With {.MaxFilasDatos = adaptador.Limite("MaxFilasDatos", 10000), .MaxArchivoMB = adaptador.Limite("MaxArchivoMB", 10)}
            ImportacionExcelRequestGuard.ValidarMultipart(http, c, limites)
            Dim solicitud = ImportacionExcelRequestGuard.LeerMultipart(http)
            Dim archivo = http.Request.Files(0)
            If archivo.ContentLength <= 0 OrElse CLng(archivo.ContentLength) > CLng(limites.MaxArchivoMB) * 1000000L Then Throw ReglasCargaExcel.Fallo("LIMITE_ARCHIVO_EXCEDIDO")
            Dim servicios As New ComposicionCargaExcel(http, solicitud.ConexionId)
            salida.datos = servicios.Recepcion.Recibir(solicitud, archivo.FileName, archivo.InputStream, archivo.ContentLength)
            salida.ok = True : salida.codigo = "OK" : salida.mensaje = "Recepción verificada."
        Catch ex As ImportacionExcelException
            salida.codigo = ex.Codigo : salida.mensaje = ex.Message
        Catch
            salida.codigo = "RESULTADO_INCIERTO" : salida.mensaje = "Consulte el mismo ID antes de repetir la recepción."
        End Try
        http.Response.StatusCode = 200
        http.Response.ContentType = "application/json" : http.Response.ContentEncoding = Encoding.UTF8
        http.Response.TrySkipIisCustomErrors = True : http.Response.SuppressFormsAuthenticationRedirect = True
        http.Response.Write(JsonConvert.SerializeObject(salida, New JsonSerializerSettings With {.TypeNameHandling = TypeNameHandling.None, .StringEscapeHandling = StringEscapeHandling.EscapeHtml, .Culture = Globalization.CultureInfo.InvariantCulture}))
    End Sub
    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class
