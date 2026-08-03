Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports System.Net
Imports System.IO

Public Class Class_ClassResfull

    Function Solicitar_token_general(ByVal codigo_empresa As String,
                                     ByVal usuariows As String,
                                     ByVal clavews As String,
                                     ByVal url_base As String,
                                     ByRef stru_token As SolicitaToken) As String
        Try

            Dim Parametros As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros.Add("codigoempresa", codigo_empresa)
            Parametros.Add("usuariows", usuariows)
            Parametros.Add("clavews", clavews)
            Dim Result As String = ""
            Dim Class_ClassResfull As New Class_ClassResfull
            Dim RefclasDeserializacion As New Class_Desserializacion
            Dim respuestaServidor As String = ""
            Result = Class_ClassResfull.GetResponse(url_base,
                                                    Parametros,
                                                    "POST",
                                                    respuestaServidor)
            If Result <> "YES" Then
                Solicitar_token_general = Result
                Exit Function
            End If
            stru_token = Nothing
            Result = RefclasDeserializacion.DesSerializacion_SolicitaToken(respuestaServidor,
                                                                           stru_token)
            If Result <> "YES" Then
                Solicitar_token_general = Result
                Exit Function
            End If
            Solicitar_token_general = "YES"
        Catch ex As Exception
            Solicitar_token_general = "Inconsistencia general función Solicitar_token_general " & ex.Message
        End Try
    End Function
    Public Class CDResFull
        Property ErrorApp As String
        Property responseFromServer As String
    End Class
    ' Versión interna asíncrona
    'Private Async Function GetResponseAsync(ByVal urlBase As String,
    '                                        ByVal parameters As Dictionary(Of String, String),
    '                                        ByVal method As String) As Task(Of CDResFull)
    '    Dim CDResFull As New CDResFull
    '    Try
    '        Dim Result As String = ""

    '        If method = "GET" Then
    '            CDResFull = Await Me.GetResponse_GET_Async(urlBase, parameters)
    '            Return CDResFull
    '        End If
    '        If method = "POST" Then
    '            CDResFull = Await Me.GetResponse_POST_Async(urlBase, parameters)
    '            Return CDResFull
    '        End If
    '    Catch ex As Exception
    '        CDResFull.ErrorApp = "Inconsistencia general función GetResponse " & ex.Message
    '        Return CDResFull
    '    End Try
    'End Function

    ' Versión pública que puedes llamar desde cualquier parte, incluso funciones no async
    'Public Function GetResponse(ByVal urlBase As String,
    '                           ByVal parameters As Dictionary(Of String, String),
    '                           ByVal method As String,
    '                           ByRef responseFromServer As String) As String
    '    Try
    '        Dim CDResFull As New CDResFull
    '        ' Si estamos en un contexto ASP.NET y hay un HttpContext activo
    '        If HttpContext.Current IsNot Nothing Then
    '            ' Ejecutar como tarea asíncrona para no bloquear el thread pool
    '            CDResFull = Task.Run(Function() GetResponseAsync(urlBase, parameters, method)).Result
    '            If CDResFull.ErrorApp <> "YES" Then
    '                GetResponse = CDResFull.ErrorApp
    '                Exit Function
    '            Else
    '                responseFromServer = CDResFull.responseFromServer
    '                GetResponse = CDResFull.ErrorApp
    '                Exit Function
    '            End If
    '        Else
    '            ' Si no estamos en ASP.NET, podemos usar ejecución directa
    '            CDResFull = GetResponseAsync(urlBase, parameters, method) _
    '                   .ConfigureAwait(False) _
    '                   .GetAwaiter() _
    '                   .GetResult()
    '            If CDResFull.ErrorApp <> "YES" Then
    '                GetResponse = CDResFull.ErrorApp
    '                Exit Function
    '            Else
    '                responseFromServer = CDResFull.responseFromServer
    '                GetResponse = CDResFull.ErrorApp
    '                Exit Function
    '            End If
    '        End If

    '    Catch ex As Exception
    '        GetResponse = "Inconsitencia general funcion GetResponse " & ex.Message
    '    End Try

    'End Function

    Function GetResponse(ByVal urlBase As String,
                         ByVal parameters As Dictionary(Of String, String),
                         ByVal method As String,
                         ByRef responseFromServer As String) As String
        Try
            Dim Result As String = ""
            If method = "GET" Then
                Result = GetResponse_GET(urlBase,
                                         parameters,
                                         responseFromServer)
                If Result <> "YES" Then
                    GetResponse = Result
                    Exit Function
                End If
            End If
            If method = "POST" Then
                'Result = Await Me.GetResponse_POST_Async(urlBase, parameters)
                Result = GetResponse_POST(urlBase,
                                          parameters,
                                          responseFromServer)
                If Result <> "YES" Then
                    GetResponse = Result
                    Exit Function
                End If
            End If
            GetResponse = "YES"
            Exit Function
        Catch ex As Exception
            GetResponse = "Inconsistencia general función GetResponse " & ex.Message
        End Try

    End Function
    Function ConcatParams(ByVal parameters As Dictionary(Of String, String)) As String
        Dim FirstParam As Boolean = True
        Dim Parametros As StringBuilder = Nothing
        If parameters IsNot Nothing Then
            Parametros = New StringBuilder()
            For Each param As KeyValuePair(Of String, String) In parameters
                Parametros.Append(If(FirstParam, "", " , "))
                'Parametros.Append(param.Key & " : " + System.Net.WebUtility.HtmlEncode(param.Value))
                Parametros.Append(param.Key & " : " & param.Value)
                FirstParam = False
            Next
        End If
        Return If(Parametros Is Nothing, String.Empty, Parametros.ToString())
    End Function

    Function GetResponse_GET(ByVal url As String,
                             ByVal parameters As Dictionary(Of String, String),
                             ByRef responseFromServer As String) As String
        Try
            Dim parametrosConcatenados As String = ConcatParams(parameters)
            Dim urlConParametros As String = url & "?" & parametrosConcatenados
            Dim wr As System.Net.WebRequest = CType(System.Net.WebRequest.Create(urlConParametros), System.Net.HttpWebRequest)
            wr.Method = "GET"
            wr.ContentType = "application/x-www-form-urlencoded"
            Dim newStream As System.IO.Stream
            Dim response As System.Net.WebResponse = wr.GetResponse()
            newStream = response.GetResponseStream()
            Dim reader As System.IO.StreamReader = New System.IO.StreamReader(newStream)
            responseFromServer = reader.ReadToEnd()
            reader.Close()
            newStream.Close()
            response.Close()
            GetResponse_GET = "YES"
        Catch ex As Exception
            GetResponse_GET = "Inconsistencia general función GetResponse_GET " & ex.Message
        End Try
    End Function
    Public Function GetResponse_POST(ByVal url As String,
                                     ByVal parameters As Dictionary(Of String, String),
                                     ByRef responseFromServer As String) As String
        Try
            System.Net.ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf validarCertificado)
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls
            System.Net.ServicePointManager.Expect100Continue = True
            System.Net.ServicePointManager.SecurityProtocol = CType(3072, System.Net.SecurityProtocolType)
            System.Net.ServicePointManager.DefaultConnectionLimit = 9999
            'Dim parametrosConcatenados As String = ConcatParams(parameters)
            Dim wr As System.Net.WebRequest = CType(System.Net.WebRequest.Create(url), System.Net.HttpWebRequest)
            wr.Method = "POST"
            wr.ContentType = "application/json"
            wr.UseDefaultCredentials = True
            Dim newStream As System.IO.Stream
            Dim encoding As System.Text.ASCIIEncoding = New System.Text.ASCIIEncoding()
            Dim serializer As New JavaScriptSerializer()
            Dim serijson As String = serializer.Serialize(parameters)
            Dim byte1 As Byte() = encoding.GetBytes(serijson)
            wr.ContentLength = byte1.Length
            newStream = wr.GetRequestStream()
            newStream.Write(byte1, 0, byte1.Length)
            Dim response As System.Net.WebResponse = wr.GetResponse()
            newStream = response.GetResponseStream()
            Dim reader As System.IO.StreamReader = New System.IO.StreamReader(newStream)
            responseFromServer = reader.ReadToEnd()
            reader.Close()
            newStream.Close()
            response.Close()
            GetResponse_POST = "YES"
        Catch ex As Exception
            GetResponse_POST = "Inconsistencia general función GetResponse_POST " & ex.Message
        End Try
    End Function
    'Dim resultado As String = Await GetResponse_GET_Async(url, parametros, respuesta)
    'Public Async Function GetResponse_GET_Async(ByVal url As String,
    '                                            ByVal parameters As Dictionary(Of String, String)) As Task(Of CDResFull)
    '    Dim CDResFull As New CDResFull
    '    Try
    '        Dim queryString = String.Join("&", parameters.Select(Function(kvp) $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))
    '        Dim fullUrl = If(String.IsNullOrEmpty(queryString), url, $"{url}?{queryString}")
    '        Dim client = HttpClientSingleton.Instance
    '        client.Timeout = TimeSpan.FromSeconds(30)
    '        Dim httpResponse = Await client.GetAsync(fullUrl).ConfigureAwait(False)
    '        Dim responseFromServer = Await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(False)
    '        If httpResponse.IsSuccessStatusCode Then
    '            CDResFull.ErrorApp = "YES"
    '            CDResFull.responseFromServer = responseFromServer
    '            Return CDResFull
    '        Else
    '            Dim errorMessage As String = Await httpResponse.Content.ReadAsStringAsync()
    '            CDResFull.ErrorApp = httpResponse.StatusCode.ToString & errorMessage
    '            Return CDResFull
    '        End If
    '        '' Construir la URL con parámetros
    '        'Dim parametrosConcatenados As String = _ConcatParams(parameters)
    '        'Dim urlConParametros As String = $"{url}?{parametrosConcatenados}"
    '        'Using client As New HttpClient()
    '        '    ' Establecer cabecera de contenido
    '        '    client.DefaultRequestHeaders.ExpectContinue = False
    '        '    ' Hacer la petición GET
    '        '    Dim response As HttpResponseMessage = Await client.GetAsync(urlConParametros)

    '        '    ' Asegurar respuesta exitosa
    '        '    response.EnsureSuccessStatusCode()

    '        '    ' Leer el contenido de la respuesta
    '        '    Dim responseFromServer = Await response.Content.ReadAsStringAsync()
    '        '    CDResFull.ErrorApp = "YES"
    '        '    CDResFull.responseFromServer = responseFromServer
    '        '    Return CDResFull
    '        'End Using
    '    Catch ex As Exception
    '        CDResFull.ErrorApp = "Inconsistencia general función GetResponse_GET " & ex.Message
    '        Return CDResFull
    '    End Try
    'End Function
    Private Async Function GetResponseAsync(urlBase As String, parameters As Dictionary(Of String, String), method As String) As Task(Of CDResFull)
        Dim result As New CDResFull
        Try
            Dim request As HttpWebRequest = CType(WebRequest.Create(urlBase), HttpWebRequest)
            request.Method = method
            request.ContentType = "application/x-www-form-urlencoded"

            ' Construir el cuerpo
            If parameters IsNot Nothing AndAlso parameters.Count > 0 Then
                Dim postData As String = String.Join("&", parameters.Select(Function(kvp) kvp.Key & "=" & HttpUtility.UrlEncode(kvp.Value)))
                Dim data As Byte() = Encoding.UTF8.GetBytes(postData)

                Using stream = Await request.GetRequestStreamAsync()
                    Await stream.WriteAsync(data, 0, data.Length)
                End Using
            End If

            Using response As HttpWebResponse = CType(Await request.GetResponseAsync(), HttpWebResponse)
                Using reader As New StreamReader(response.GetResponseStream())
                    result.responseFromServer = Await reader.ReadToEndAsync()
                End Using
            End Using

            result.ErrorApp = "YES"
        Catch ex As Exception
            result.ErrorApp = "Inconsistencia general en GetResponseAsync: " & ex.Message
        End Try
        Return result
    End Function

    Public Async Function GetResponse_POST_Async(ByVal url As String,
                                                 ByVal parameters As Dictionary(Of String, String)) As Task(Of CDResFull)
        Dim CDResFull As New CDResFull
        Try
            Dim content = New FormUrlEncodedContent(parameters)
            Dim client = HttpClientSingleton.Instance
            client.Timeout = TimeSpan.FromSeconds(30)
            Dim httpResponse = Await client.PostAsync(url, content).ConfigureAwait(False)
            Dim responseFromServer = Await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(False)
            If httpResponse.IsSuccessStatusCode Then
                CDResFull.responseFromServer = responseFromServer
                CDResFull.ErrorApp = "YES"
                Return CDResFull
            Else
                Dim errorMessage As String = Await httpResponse.Content.ReadAsStringAsync()
                CDResFull.ErrorApp = httpResponse.StatusCode.ToString & errorMessage
                Return CDResFull
            End If
            '' Validación de certificado (si aplica)
            'ServicePointManager.ServerCertificateValidationCallback =
            'New System.Net.Security.RemoteCertificateValidationCallback(AddressOf validarCertificado)

            '' Protocolos y límites de conexión
            'ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType) ' Tls12
            'ServicePointManager.Expect100Continue = True
            'ServicePointManager.DefaultConnectionLimit = 9999

            '' Serializar los parámetros como JSON
            'Dim serializer As New JavaScriptSerializer()
            'Dim jsonBody As String = serializer.Serialize(parameters)
            'Dim content As New StringContent(jsonBody, Encoding.UTF8, "application/json")
            'Using client As New HttpClient()
            '    client.DefaultRequestHeaders.ExpectContinue = False
            '    Dim response As HttpResponseMessage = Await client.PostAsync(url, content)
            '    response.EnsureSuccessStatusCode()
            '    Dim responseFromServer = Await response.Content.ReadAsStringAsync()
            '    CDResFull.responseFromServer = responseFromServer
            '    CDResFull.ErrorApp = "YES"
            'End Using
            Return CDResFull
        Catch ex As Exception
            CDResFull.ErrorApp = "Inconsistencia general función GetResponse_POST " & ex.Message
            Return CDResFull
        End Try
    End Function
    Private Function _ConcatParams(ByVal parameters As Dictionary(Of String, String)) As String
        Dim paramList As New List(Of String)

        For Each pair In parameters
            Dim keyEncoded As String = Uri.EscapeDataString(pair.Key)
            Dim valueEncoded As String = Uri.EscapeDataString(pair.Value)
            paramList.Add($"{keyEncoded}={valueEncoded}")
        Next

        Return String.Join("&", paramList)
    End Function


    Public Shared Function ValidateServerCertificate(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function
    Public Shared Function validarCertificado(ByVal sender As Object, ByVal certificado As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal cadena As System.Security.Cryptography.X509Certificates.X509Chain, ByVal sslErrores As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function
End Class
Public NotInheritable Class HttpClientSingleton
    Private Sub New()
    End Sub

    ' HttpClient compartido para toda la aplicación
    Private Shared ReadOnly _client As New HttpClient()

    Public Shared ReadOnly Property Instance As HttpClient
        Get
            Return _client
        End Get
    End Property
End Class
