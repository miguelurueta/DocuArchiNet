Public Structure SolicitaToken
    Dim codigoerror As String
    Dim mensajeerror As String
    Dim token As String
End Structure
Public Class Class_parram_token
    Public Structure SolicitaToken
        Dim codigoerror As String
        Dim mensajeerror As String
        Dim token As String
    End Structure
    Public codigoerror As String
    Public Property codigoerror_() As String
        Get
            Return codigoerror
        End Get
        Set(ByVal value As String)
            codigoerror = value
        End Set
    End Property
    Public mensajeerror As String
    Public Property mensajeerror_() As String
        Get
            Return mensajeerror
        End Get
        Set(ByVal value As String)
            mensajeerror = value
        End Set
    End Property
    Public token As String
    Public Property token_() As String
        Get
            Return token
        End Get
        Set(ByVal value As String)
            token = value
        End Set
    End Property
End Class
