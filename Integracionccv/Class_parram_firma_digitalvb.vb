Public Structure stru_firma_digital_sii
    Dim codigoerror As String
    Dim mensajeerror As String
    Dim codigofirmapdf As String
    Dim url As String
End Structure
Public Class Class_parram_firma_digital
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
    Public url As String
    Public Property url_() As String
        Get
            Return url
        End Get
        Set(ByVal value As String)
            url = value
        End Set
    End Property
    Public codigofirmapdf As String
    Public Property codigofirmapdf_() As String
        Get
            Return codigofirmapdf
        End Get
        Set(ByVal value As String)
            codigofirmapdf = value
        End Set
    End Property
End Class
