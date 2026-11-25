Option Strict On
Imports System
Imports System.Linq
Imports System.Web
Imports System.Web.SessionState
Imports Newtonsoft.Json

Public Class ImportacionExcelAnalisis
    Implements IHttpHandler, IRequiresSessionState
    Public Sub ProcessRequest(http As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim salida As New RespuestaAnalisisPlantillaExcel With {.operacionId = Guid.NewGuid().ToString("N")}
        http.Response.Cache.SetCacheability(HttpCacheability.NoCache) : http.Response.Cache.SetNoStore()
        Try
            Dim ctx = New ImportacionExcelContextAdapter().Resolver(http), adaptador As New ImportacionExcelConexionAdapter
            adaptador.ValidarModulo(http, ctx)
            Dim limites As New LimitesLecturaExcel With {.MaxFilasDatos = adaptador.Limite("MaxFilasDatos", 10000), .MaxArchivoMB = adaptador.Limite("MaxArchivoMB", 10)}
            ImportacionExcelRequestGuard.ValidarMultipart(http, ctx, limites)
            Dim claves = New String() {"conexionId", "hoja", "tabla", "modo"}
            If http.Request.Form.Count <> claves.Length OrElse http.Request.Files.Count <> 1 OrElse http.Request.Files.AllKeys(0) <> "archivo" Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
            For Each clave In claves
                Dim valores = http.Request.Form.GetValues(clave)
                If valores Is Nothing OrElse valores.Length <> 1 OrElse valores(0).Length > 120 Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
            Next
            If http.Request.Form.AllKeys.Any(Function(k) Not claves.Contains(k, StringComparer.Ordinal)) Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
            Dim conexion As UInteger
            If Not UInteger.TryParse(http.Request.Form("conexionId"), Globalization.NumberStyles.None, Globalization.CultureInfo.InvariantCulture, conexion) OrElse conexion = 0 Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
            Dim modo = http.Request.Form("modo"), tabla = http.Request.Form("tabla"), destino As DestinoPlantillaExcel = Nothing
            If Not {"CREAR", "AGREGAR", "REEMPLAZAR"}.Contains(modo, StringComparer.Ordinal) Then Throw ReglasCargaExcel.Fallo("SOLICITUD_INVALIDA")
            Dim c = adaptador.ContextoCarga(http, ctx, conexion)
            If modo <> "CREAR" Then
                If Not ServicioPlantillasExcel.TablaValida(tabla) Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
                destino = adaptador.Resolver(http, ctx, conexion).Destino(tabla)
                If destino Is Nothing OrElse Not destino.Existe OrElse Not String.Equals(destino.Motor, "InnoDB", StringComparison.OrdinalIgnoreCase) Then Throw ReglasCargaExcel.Fallo("DESTINO_INVALIDO")
            End If
            Dim archivo = http.Request.Files(0)
            If archivo.ContentLength <= 0 OrElse CLng(archivo.ContentLength) > CLng(limites.MaxArchivoMB) * 1000000L Then Throw ReglasCargaExcel.Fallo("LIMITE_ARCHIVO_EXCEDIDO")
            salida.datos = New ServicioAnalisisPlantillaExcel().Analizar(archivo.InputStream, archivo.FileName, http.Request.Form("hoja"), limites, destino)
            ReglasCargaExcel.MismaAutoridad(c, adaptador.ContextoCarga(http, New ImportacionExcelContextAdapter().Resolver(http), conexion))
            ImportacionExcelRequestGuard.ValidarMultipart(http, New ImportacionExcelContextAdapter().Resolver(http), limites)
            salida.ok = True : salida.codigo = "OK" : salida.mensaje = "Propuesta lista para revisión. No se ha importado el archivo."
        Catch ex As ImportacionExcelException
            salida.datos = Nothing : salida.codigo = ex.Codigo : salida.mensaje = "Revise el archivo y la configuración seleccionada."
        Catch
            salida.datos = Nothing : salida.codigo = "ANALISIS_NO_DISPONIBLE" : salida.mensaje = "No se pudo analizar el libro. No se ha guardado una plantilla ni iniciado una carga."
        End Try
        http.Response.StatusCode = 200 : http.Response.ContentType = "application/json" : http.Response.ContentEncoding = Text.Encoding.UTF8
        http.Response.TrySkipIisCustomErrors = True : http.Response.SuppressFormsAuthenticationRedirect = True
        http.Response.Write(JsonConvert.SerializeObject(salida, New JsonSerializerSettings With {.TypeNameHandling = TypeNameHandling.None, .StringEscapeHandling = StringEscapeHandling.EscapeHtml, .Culture = Globalization.CultureInfo.InvariantCulture}))
    End Sub
    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class
