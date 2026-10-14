Imports System
Imports System.Globalization
Imports System.IO
Imports System.Web
Imports System.Web.SessionState

Public Class Handler_image_wf
    Implements IHttpHandler, IReadOnlySessionState

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        PrepararRespuesta(context)
        Dim metodo As String = context.Request.HttpMethod.ToUpperInvariant()
        If metodo <> "GET" AndAlso metodo <> "HEAD" Then
            context.Response.StatusCode = 405
            context.Response.AppendHeader("Allow", "GET, HEAD")
            ResponderError(context, "Solicitud no permitida.")
            Return
        End If

        Try
            Dim referencia As String = context.Request.QueryString("doc_ref")
            If Not String.IsNullOrWhiteSpace(referencia) Then
                ProcesarReferenciaAutorizada(context, referencia, metodo = "HEAD")
                Return
            End If

            Dim rutaLegacy As String = context.Request.QueryString("rut_image")
            If Not String.IsNullOrWhiteSpace(rutaLegacy) Then
                ProcesarLegacy(context, rutaLegacy, metodo = "HEAD")
                Return
            End If

            context.Response.StatusCode = 400
            ResponderError(context, "Referencia de documento requerida.")
        Catch ex As HttpException
            ResponderFalloTransferencia(context)
        Catch ex As IOException
            ResponderFalloTransferencia(context)
        Catch ex As UnauthorizedAccessException
            ResponderFalloTransferencia(context, 404, "Documento no disponible.")
        Catch
            ResponderFalloTransferencia(context, 500, "No fue posible entregar el documento.")
        End Try
    End Sub

    Private Shared Sub ProcesarReferenciaAutorizada(ByVal context As HttpContext,
                                                     ByVal referencia As String,
                                                     ByVal esHead As Boolean)
        Dim sessionId As String = If(context.Session Is Nothing, String.Empty, context.Session.SessionID)
        Dim documento As DocumentoStreamingAutorizado = Nothing
        Dim codec As New DocumentoStreamingReferencia()
        If Not codec.TryResolver(referencia, sessionId, documento) Then
            context.Response.StatusCode = 404
            ResponderError(context, "Documento no disponible.")
            Return
        End If

        Dim archivo As New FileInfo(documento.Ruta)
        If Not archivo.Exists Then
            context.Response.StatusCode = 404
            ResponderError(context, "Documento no disponible.")
            Return
        End If

        Dim ultimaModificacion As DateTime = CondicionesHttpDocumento.NormalizarHttpDate(archivo.LastWriteTimeUtc)
        Dim etag As String = CondicionesHttpDocumento.CrearETag(referencia, archivo.Length, ultimaModificacion)
        ConfigurarRepresentacionAutorizada(context, documento, archivo.Length, ultimaModificacion, etag)

        Dim headerRange As String = If(esHead, Nothing, context.Request.Headers("Range"))
        If String.IsNullOrWhiteSpace(headerRange) AndAlso
           CondicionesHttpDocumento.EsNoModificado(context.Request.Headers("If-None-Match"),
                                                    context.Request.Headers("If-Modified-Since"),
                                                    etag,
                                                    ultimaModificacion) Then
            context.Response.StatusCode = 304
            context.Response.SuppressContent = True
            Completar(context)
            Return
        End If

        Dim intervalo As IntervaloBytesDocumento = Nothing
        Dim resultadoRango As ResultadoRangoHttp = RangoHttpDocumento.Resolver(headerRange, archivo.Length, intervalo)
        If resultadoRango = ResultadoRangoHttp.Valido AndAlso
           Not CondicionesHttpDocumento.IfRangePermiteRango(context.Request.Headers("If-Range"), etag, ultimaModificacion) Then
            resultadoRango = ResultadoRangoHttp.SinRango
            intervalo = Nothing
        End If

        If resultadoRango = ResultadoRangoHttp.Invalido Then
            context.Response.StatusCode = 416
            context.Response.AppendHeader("Content-Range", "bytes */" & archivo.Length.ToString(CultureInfo.InvariantCulture))
            context.Response.AppendHeader("Content-Length", "0")
            context.Response.SuppressContent = True
            Completar(context)
            Return
        End If

        Dim inicio As Long = 0L
        Dim longitud As Long = archivo.Length
        If resultadoRango = ResultadoRangoHttp.Valido Then
            inicio = intervalo.Inicio
            longitud = intervalo.Longitud
            context.Response.StatusCode = 206
            context.Response.AppendHeader("Content-Range", String.Format(CultureInfo.InvariantCulture,
                                                                           "bytes {0}-{1}/{2}",
                                                                           intervalo.Inicio,
                                                                           intervalo.Fin,
                                                                           archivo.Length))
        Else
            context.Response.StatusCode = 200
        End If
        context.Response.AppendHeader("Content-Length", longitud.ToString(CultureInfo.InvariantCulture))

        If esHead Then
            context.Response.SuppressContent = True
            Completar(context)
            Return
        End If
        Transmitir(context, archivo.FullName, inicio, longitud)
    End Sub

    Private Shared Sub ProcesarLegacy(ByVal context As HttpContext,
                                      ByVal rutaRecibida As String,
                                      ByVal esHead As Boolean)
        Dim ruta As String = Path.GetFullPath(rutaRecibida.Replace("|", "/"))
        Dim archivo As New FileInfo(ruta)
        If Not archivo.Exists Then
            context.Response.StatusCode = 404
            ResponderError(context, "Documento no disponible.")
            Return
        End If

        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        context.Response.Cache.SetNoStore()
        context.Response.ContentType = ObtenerMime(archivo.Extension)
        context.Response.AppendHeader("X-Content-Type-Options", "nosniff")
        context.Response.AppendHeader("Content-Disposition", Disposicion(archivo.Name, EsPdf(archivo.Extension)))
        context.Response.StatusCode = 200
        context.Response.AppendHeader("Content-Length", archivo.Length.ToString(CultureInfo.InvariantCulture))
        If esHead Then
            context.Response.SuppressContent = True
            Completar(context)
            Return
        End If
        Transmitir(context, archivo.FullName, 0L, archivo.Length)
    End Sub

    Private Shared Sub ConfigurarRepresentacionAutorizada(ByVal context As HttpContext,
                                                           ByVal documento As DocumentoStreamingAutorizado,
                                                           ByVal longitud As Long,
                                                           ByVal ultimaModificacion As DateTime,
                                                           ByVal etag As String)
        context.Response.ContentType = documento.TipoContenido
        context.Response.AppendHeader("Accept-Ranges", "bytes")
        context.Response.AppendHeader("Vary", "Cookie")
        context.Response.AppendHeader("X-Content-Type-Options", "nosniff")
        context.Response.AppendHeader("Content-Disposition", Disposicion(documento.Nombre, EsPdf(Path.GetExtension(documento.Nombre))))
        context.Response.Cache.SetCacheability(HttpCacheability.Private)
        context.Response.Cache.AppendCacheExtension("no-cache")
        context.Response.Cache.SetMaxAge(TimeSpan.Zero)
        context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches)
        context.Response.AppendHeader("ETag", etag)
        context.Response.Cache.SetLastModified(ultimaModificacion)
    End Sub

    Private Shared Sub Transmitir(ByVal context As HttpContext,
                                  ByVal ruta As String,
                                  ByVal inicio As Long,
                                  ByVal longitud As Long)
        Using origen As New FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
            EscritorStreamingDocumento.Copiar(origen,
                                               context.Response.OutputStream,
                                               inicio,
                                               longitud,
                                               Function() context.Response.IsClientConnected)
        End Using
        Completar(context)
    End Sub

    Private Shared Sub PrepararRespuesta(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.BufferOutput = False
        context.Response.TrySkipIisCustomErrors = True
    End Sub

    Private Shared Sub ResponderFalloTransferencia(ByVal context As HttpContext,
                                                   Optional ByVal status As Integer = 500,
                                                   Optional ByVal mensaje As String = "No fue posible completar la transferencia.")
        If context.Response.HeadersWritten OrElse Not context.Response.IsClientConnected Then
            Completar(context)
            Return
        End If
        context.Response.Clear()
        context.Response.StatusCode = status
        ResponderError(context, mensaje)
    End Sub

    Private Shared Sub ResponderError(ByVal context As HttpContext, ByVal mensaje As String)
        context.Response.ContentType = "text/plain; charset=utf-8"
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        context.Response.Cache.SetNoStore()
        If String.Equals(context.Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase) Then
            context.Response.SuppressContent = True
        Else
            context.Response.Write(mensaje)
        End If
        Completar(context)
    End Sub

    Private Shared Sub Completar(ByVal context As HttpContext)
        context.ApplicationInstance.CompleteRequest()
    End Sub

    Private Shared Function Disposicion(ByVal nombre As String, ByVal inline As Boolean) As String
        Dim seguro As String = Path.GetFileName(If(nombre, "documento")).Replace("""", String.Empty).Replace(vbCr, String.Empty).Replace(vbLf, String.Empty)
        Return If(inline, "inline", "attachment") & "; filename=""" & seguro & """"
    End Function

    Private Shared Function EsPdf(ByVal extension As String) As Boolean
        Return String.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function ObtenerMime(ByVal extension As String) As String
        Select Case If(extension, String.Empty).ToLowerInvariant()
            Case ".pdf" : Return "application/pdf"
            Case ".jpg", ".jpeg" : Return "image/jpeg"
            Case ".png" : Return "image/png"
            Case ".gif" : Return "image/gif"
            Case ".tif", ".tiff" : Return "image/tiff"
            Case ".txt" : Return "text/plain"
            Case ".xml" : Return "application/xml"
            Case ".doc" : Return "application/msword"
            Case ".docx" : Return "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            Case ".xls" : Return "application/vnd.ms-excel"
            Case ".xlsx" : Return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Case Else : Return "application/octet-stream"
        End Select
    End Function

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class
