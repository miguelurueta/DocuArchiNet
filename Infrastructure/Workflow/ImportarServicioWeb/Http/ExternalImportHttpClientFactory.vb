Imports System
Imports System.Net.Http

Public Interface IExternalImportHttpClientFactory
    Function GetClient() As HttpClient
End Interface

' Conserva una configuración estable; cada solicitud lleva sus propias políticas.
Public NotInheritable Class ExternalImportHttpClientFactory
    Implements IExternalImportHttpClientFactory
    Implements IDisposable

    Private ReadOnly _client As HttpClient
    Private ReadOnly _ownsClient As Boolean

    Public Sub New()
        Me.New(False)
    End Sub

    Public Sub New(ByVal useDefaultCredentials As Boolean)
        Me.New(New HttpClientHandler() With {.UseDefaultCredentials = useDefaultCredentials})
    End Sub

    Public Sub New(ByVal handler As HttpMessageHandler)
        If handler Is Nothing Then Throw New ArgumentNullException("handler")
        _client = New HttpClient(handler, True)
        _client.Timeout = System.Threading.Timeout.InfiniteTimeSpan
        _ownsClient = True
    End Sub

    Public Sub New(ByVal client As HttpClient)
        If client Is Nothing Then Throw New ArgumentNullException("client")
        _client = client
        _ownsClient = False
    End Sub

    Public Function GetClient() As HttpClient Implements IExternalImportHttpClientFactory.GetClient
        Return _client
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If _ownsClient Then _client.Dispose()
    End Sub
End Class
