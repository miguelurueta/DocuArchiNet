Option Strict On
Imports System
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Net.Mime
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web

Public NotInheritable Class ImportacionCarpetasRequestGuard
    Private Const ClaveToken As String = "DOC111_OPERACION_CSRF_TOKEN"
    Private Const ClaveVinculo As String = "DOC111_OPERACION_CSRF_VINCULO"
    Public Const CabeceraToken As String = "X-Importacion-Carpetas-CSRF"
    Private Sub New()
    End Sub

    Public Shared Function Emitir(http As HttpContext, contexto As ContextoImportacionCarpetas) As String
        ExigirContexto(http, contexto)
        Dim vinculo = VinculoContexto(http, contexto)
        Dim token = TryCast(http.Session(ClaveToken), String)
        If Not String.Equals(TryCast(http.Session(ClaveVinculo), String), vinculo, StringComparison.Ordinal) OrElse Not PerfilImportacionRequestGuard.CompararTokens(token, token) Then
            Dim bytes(31) As Byte
            Using random = RandomNumberGenerator.Create()
                random.GetBytes(bytes)
            End Using
            token = Convert.ToBase64String(bytes)
            http.Session(ClaveToken) = token
            http.Session(ClaveVinculo) = vinculo
        End If
        http.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        http.Response.Cache.SetNoStore()
        Return token
    End Function

    'Identificador de espacio de referencias del navegador. No es un permiso,
    'token CSRF ni identidad autenticada; no contiene credenciales/conexiones.
    Public Shared Function ReferenciaRecuperacion(http As HttpContext, contexto As ContextoImportacionCarpetas) As String
        ExigirContexto(http, contexto)
        Dim documento = PerfilImportacionJson.Escribir(New With {.sesion = VinculoContexto(http, contexto), .login = Convert.ToString(http.Session("GA_LOGINUSUARIOGESTION"), CultureInfo.InvariantCulture),
            .servidor = Convert.ToString(http.Session("RA_IP_SERVER_MODULO"), CultureInfo.InvariantCulture).Trim(), .base = Convert.ToString(http.Session("RA_DB_NAME_MODULO"), CultureInfo.InvariantCulture).Trim(), .tipo = Convert.ToString(http.Session("RA_TYPE_DBMS_MODULO"), CultureInfo.InvariantCulture).Trim()})
        Dim vinculo As String
        Using sha = SHA256.Create()
            vinculo = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(documento))).Replace("-", "").ToLowerInvariant()
        End Using
        Const nombre As String = "DOC111_RECUPERACION"
        If Not String.Equals(TryCast(http.Session(nombre & "_VINCULO"), String), vinculo, StringComparison.Ordinal) OrElse Not Regex.IsMatch(If(TryCast(http.Session(nombre & "_REF"), String), ""), "\A[0-9a-f]{32}\z", RegexOptions.CultureInvariant) Then
            http.Session(nombre & "_VINCULO") = vinculo : http.Session(nombre & "_REF") = Guid.NewGuid().ToString("N")
        End If
        Return CStr(http.Session(nombre & "_REF"))
    End Function

    Public Shared Function LeerJson(Of T As Class)(http As HttpContext, contexto As ContextoImportacionCarpetas, limites As LimitesImportacionCarpetas) As T
        Dim tipo = ValidarBase(http, contexto, limites)
        If Not String.Equals(tipo.MediaType, "application/json", StringComparison.OrdinalIgnoreCase) OrElse (Not String.IsNullOrEmpty(tipo.CharSet) AndAlso Not String.Equals(tipo.CharSet, "utf-8", StringComparison.OrdinalIgnoreCase)) Then Throw Rechazo()
        Dim maximo = Math.Min(CLng(limites.MaximoBytesManifiesto), CLng(PerfilImportacionJson.MaximoCaracteres) * 3L)
        If http.Request.ContentLength > maximo Then Throw Exceso()
        Dim stream = http.Request.InputStream
        If stream Is Nothing OrElse Not stream.CanRead OrElse Not stream.CanSeek Then Throw Invalida()
        Dim posicion = stream.Position
        Try
            stream.Position = 0
            Using memoria As New MemoryStream()
                Dim buffer(4095) As Byte
                While True
                    Dim leidos = stream.Read(buffer, 0, buffer.Length)
                    If leidos = 0 Then Exit While
                    If memoria.Length + leidos > maximo Then Throw Exceso()
                    memoria.Write(buffer, 0, leidos)
                End While
                If memoria.Length <> http.Request.ContentLength Then Throw Invalida()
                Dim texto = New UTF8Encoding(False, True).GetString(memoria.ToArray())
                Dim sobre = PerfilImportacionJson.Leer(Of SobreSolicitudPerfil(Of T))(texto)
                If sobre.solicitud Is Nothing Then Throw Invalida()
                Return sobre.solicitud
            End Using
        Catch ex As DecoderFallbackException
            Throw Invalida()
        Catch ex As IOException
            Throw Invalida()
        Finally
            stream.Position = posicion
        End Try
    End Function

    Public Shared Function LeerMultipart(http As HttpContext, contexto As ContextoImportacionCarpetas, limites As LimitesImportacionCarpetas) As RecibirArchivoImportacionSolicitud
        Dim tipo = ValidarBase(http, contexto, limites)
        If Not String.Equals(tipo.MediaType, "multipart/form-data", StringComparison.OrdinalIgnoreCase) Then Throw Rechazo()
        If tipo.Boundary Is Nothing OrElse Not Regex.IsMatch(tipo.Boundary, "\A[0-9A-Za-z'()+_,./:=? -]{0,69}[0-9A-Za-z'()+_,./:=?-]\z", RegexOptions.CultureInvariant) Then Throw Rechazo()
        If http.Request.ContentType.Split(";"c).Count(Function(p) p.Trim().StartsWith("boundary=", StringComparison.OrdinalIgnoreCase)) <> 1 Then Throw Rechazo()
        Dim metadatos = CULng(limites.MaximoBytesManifiesto)
        Dim maximo = If(ULong.MaxValue - limites.MaximoBytesArchivo < metadatos, ULong.MaxValue, limites.MaximoBytesArchivo + metadatos)
        If CULng(http.Request.ContentLength) > maximo Then Throw Exceso()
        'No se toca Form/Files antes de autoridad, token, origen y cabeceras.
        Try
            Dim formulario = http.Request.Form
            Dim campos = {"loteId", "archivoId", "claveIntento"}
            If formulario.Count <> 3 OrElse formulario.AllKeys.Any(Function(k) k Is Nothing OrElse Not campos.Contains(k, StringComparer.Ordinal)) Then Throw Invalida()
            For Each campo In campos
                Dim valores = formulario.GetValues(campo)
                If valores Is Nothing OrElse valores.Length <> 1 OrElse String.IsNullOrWhiteSpace(valores(0)) Then Throw Invalida()
            Next
            If Not ValidadorPerfilImportacion.ReferenciaValida(formulario("loteId")) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(formulario("archivoId")) OrElse Not Regex.IsMatch(formulario("claveIntento"), "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant) Then Throw Invalida()
            If http.Request.Files.Count <> 1 Then Throw Invalida()
            If http.Request.Files(0).ContentLength < 0 OrElse CULng(http.Request.Files(0).ContentLength) > limites.MaximoBytesArchivo Then Throw Exceso()
            Return New RecibirArchivoImportacionSolicitud With {.loteId = formulario("loteId"), .archivoId = formulario("archivoId"), .claveIntento = formulario("claveIntento")}
        Catch ex As HttpRequestValidationException
            Throw Invalida()
        End Try
    End Function

    Private Shared Function ValidarBase(http As HttpContext, contexto As ContextoImportacionCarpetas, limites As LimitesImportacionCarpetas) As ContentType
        ExigirContexto(http, contexto)
        LimitesImportacionCarpetas.Exigir(limites)
        If http.Request.HttpMethod <> "POST" OrElse http.Request.ContentLength <= 0 Then Throw Rechazo()
        If Not String.Equals(TryCast(http.Session(ClaveVinculo), String), VinculoContexto(http, contexto), StringComparison.Ordinal) OrElse Not PerfilImportacionRequestGuard.CompararTokens(TryCast(http.Session(ClaveToken), String), http.Request.Headers(CabeceraToken)) Then Throw Rechazo()
        Dim origin = http.Request.Headers("Origin")
        If Not String.IsNullOrEmpty(origin) Then
            Dim origen As Uri = Nothing
            If Not Uri.TryCreate(origin, UriKind.Absolute, origen) OrElse origen.AbsolutePath <> "/" OrElse origen.Query <> "" OrElse origen.Fragment <> "" OrElse Not String.Equals(origen.GetLeftPart(UriPartial.Authority), http.Request.Url.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase) Then Throw Rechazo()
        End If
        If String.Equals(http.Request.Headers("Sec-Fetch-Site"), "cross-site", StringComparison.OrdinalIgnoreCase) Then Throw Rechazo()
        Try
            Return New ContentType(http.Request.ContentType)
        Catch ex As FormatException
            Throw Rechazo()
        Catch ex As ArgumentException
            Throw Rechazo()
        End Try
    End Function

    Private Shared Sub ExigirContexto(http As HttpContext, contexto As ContextoImportacionCarpetas)
        If http Is Nothing OrElse http.Session Is Nothing OrElse contexto Is Nothing OrElse Not contexto.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) Then Throw Rechazo()
        If Convert.ToString(http.Session("GA_IDUSUARIOGESTION"), CultureInfo.InvariantCulture) <> contexto.UsuarioRef OrElse Convert.ToString(http.Session("GA_IDEMPRESA"), CultureInfo.InvariantCulture) <> contexto.AmbitoRef Then Throw Rechazo()
        Dim login = Convert.ToString(http.Session("GA_LOGINUSUARIOGESTION"), CultureInfo.InvariantCulture)
        If String.IsNullOrWhiteSpace(login) OrElse String.Equals(login.Trim(), "CONSULTAPUBLICO", StringComparison.OrdinalIgnoreCase) Then Throw Rechazo()
    End Sub

    Private Shared Function VinculoContexto(http As HttpContext, contexto As ContextoImportacionCarpetas) As String
        Return http.Session.SessionID & ":" & contexto.UsuarioRef & ":" & contexto.AmbitoRef & ":DOC111_OPERACION"
    End Function
    Private Shared Function Rechazo() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_NO_VERIFICADA", "Recargue el contexto autenticado antes de continuar.")
    End Function
    Private Shared Function Invalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "La forma de la solicitud no es válida.")
    End Function
    Private Shared Function Exceso() As PerfilImportacionException
        Return New PerfilImportacionException("LIMITE_EXCEDIDO", "La solicitud excede la capacidad permitida.")
    End Function
End Class
