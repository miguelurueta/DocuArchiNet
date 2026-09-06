Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks

Public NotInheritable Class ExternalImportHttpResponseValidator
    Private Const BufferSize As Integer = 81920

    Public Async Function ReadValidatedAsync(ByVal response As HttpResponseMessage,
                                             ByVal allowedMediaTypes As ISet(Of String),
                                             ByVal maximumBytes As Long,
                                             ByVal correlationId As String,
                                             ByVal cancellationToken As CancellationToken) As Task(Of Byte())
        If response Is Nothing Then Throw Invalid(correlationId)
        If Not response.IsSuccessStatusCode Then
            Throw New ExternalImportHttpException(New ExternalImportHttpErrorMapper().FromStatus(response.StatusCode, correlationId))
        End If
        If response.Content Is Nothing OrElse maximumBytes <= 0 Then Throw Invalid(correlationId)

        Dim contentLength = response.Content.Headers.ContentLength
        If contentLength.HasValue AndAlso contentLength.Value > maximumBytes Then Throw Invalid(correlationId)

        Dim mediaType As String = Nothing
        If response.Content.Headers.ContentType IsNot Nothing Then mediaType = response.Content.Headers.ContentType.MediaType
        If String.IsNullOrWhiteSpace(mediaType) OrElse allowedMediaTypes Is Nothing OrElse
           Not allowedMediaTypes.Contains(mediaType) Then Throw Invalid(correlationId)

        Using source As Stream = Await response.Content.ReadAsStreamAsync().ConfigureAwait(False)
            Using destination As New MemoryStream()
                Dim buffer(BufferSize - 1) As Byte
                Dim total As Long = 0
                Do
                    Dim read = Await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(False)
                    If read = 0 Then Exit Do
                    total += read
                    If total > maximumBytes Then Throw Invalid(correlationId)
                    Await destination.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(False)
                Loop
                Return destination.ToArray()
            End Using
        End Using
    End Function

    Private Shared Function Invalid(ByVal correlationId As String) As ExternalImportHttpException
        Return New ExternalImportHttpException(New ExternalImportHttpErrorMapper().InvalidResponse(correlationId))
    End Function
End Class
