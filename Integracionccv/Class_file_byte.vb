Imports System.Net
Imports System.IO
Imports System.Net.Http
Imports System.Threading.Tasks

Public Class Class_file_byte
    Function ReadFile(filePath As String, Optional shareOption As System.IO.FileShare = System.IO.FileShare.None) As Byte()
        Try
            Dim fileBytes() As Byte
            Using fs = New System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, shareOption)
                ReDim fileBytes(CInt(fs.Length - 1))
                fs.Read(fileBytes, 0, fileBytes.Length)
            End Using
            Using fs = New System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, shareOption)
                fs.Write(fileBytes, 0, fileBytes.Length)
            End Using
            Return fileBytes
        Catch ex As Exception

        End Try
    End Function
    Function ReadFile_result(filePath As String,
                             ByRef fylebyte_file_send_firma() As Byte,
                             Optional shareOption As System.IO.FileShare = System.IO.FileShare.None) As String
        Try
            Dim fileBytes() As Byte
            Using fs = New System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, shareOption)
                ReDim fileBytes(CInt(fs.Length - 1))
                fs.Read(fileBytes, 0, fileBytes.Length)
            End Using
            Using fs = New System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, shareOption)
                fs.Write(fileBytes, 0, fileBytes.Length)
            End Using
            fylebyte_file_send_firma = fileBytes
            ReadFile_result = "YES"
        Catch ex As Exception
            ReadFile_result = "Inconsistencia general funcion ReadFile_result " & ex.Message
        End Try
    End Function
    Function DownloadFileViaRestAPI(ByVal webUrl As String,
                                    ByVal credentials As ICredentials,
                                    ByVal DocumentLibName As String,
                                    ByVal fileName As String,
                                    ByVal path As String) As String
        Try

            Using client As WebClient = New WebClient()
                client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f")
                'webUrl = webUrl.Replace("https", "http")
                'client.Credentials = credentials
                'Dim endpointUri As Uri = New Uri(webUrl & "/_api/web/GetFileByServerRelativeUrl('" & webRelativeUrl & "/" & documentLibName & "/" & fileName & "')/$value")
                'Dim p = "http://repositoriosii.s3.amazonaws.com/40/mreg/20240308/8281598.pdf?X-Amz-Content-Sha256=UNSIGNED-PAYLOAD&amp;X-Amz-Algorithm=AWS4-HMAC-SHA256&amp;X-Amz-Credential=AKIAIIZPAUVWMH32PSKA%2F20240523%2Fus-east-1%2Fs3%2Faws4_request&amp;X-Amz-Date=20240523T204331Z&amp;X-Amz-SignedHeaders=host&amp;X-Amz-Expires=10800&amp;X-Amz-Signature=947fcbf007eb10588786072803b7c65331d935ca80ffb745b5412e71c15f71cf"
                Dim endpointUri As Uri = New Uri(webUrl)
                Dim data As Byte() = client.DownloadData(endpointUri)
                Dim outputStream As FileStream = New FileStream(path & fileName, FileMode.OpenOrCreate Or FileMode.Append, FileAccess.Write, FileShare.None)
                outputStream.Write(data, 0, data.Length)
                outputStream.Flush(True)
                outputStream.Close()
            End Using
            DownloadFileViaRestAPI = "YES"
        Catch ex As Exception
            DownloadFileViaRestAPI = "Funcion DownloadFileViaRestAPI Mensaje de servidor " & ex.Message & " archivo fuente " & webUrl & " Destino " & path & "  " & fileName
        End Try

    End Function
    Private Async Function DownloadFileViaRestAPI_Async(webUrl As String,
                                                       credentials As ICredentials,
                                                       DocumentLibName As String,
                                                       fileName As String,
                                                       path As String) As Task(Of String)
        Try
            Using client As New HttpClient()
                ' Si necesitas credenciales, se pueden aplicar así:
                ' Dim handler As New HttpClientHandler() With {.Credentials = credentials}
                ' client = New HttpClient(handler)

                ' Cabeceras necesarias
                client.DefaultRequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f")
                Dim response As HttpResponseMessage = Await client.GetAsync(webUrl)

                If Not response.IsSuccessStatusCode Then
                    Return $"Error HTTP {CInt(response.StatusCode)}: {response.ReasonPhrase}"
                End If
                Dim data As Byte() = Await response.Content.ReadAsByteArrayAsync()
                ' Guardar archivo
                Dim fullPath As String = IO.Path.Combine(path, fileName)
                Using outputStream As New FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None)
                    Await outputStream.WriteAsync(data, 0, data.Length)
                End Using
                Return "YES"
            End Using
        Catch ex As Exception
            Return $"Funcion DownloadFileViaRestAPI Mensaje de servidor {ex.Message} archivo fuente {webUrl} Destino {path} {fileName}"
        End Try
    End Function
End Class
