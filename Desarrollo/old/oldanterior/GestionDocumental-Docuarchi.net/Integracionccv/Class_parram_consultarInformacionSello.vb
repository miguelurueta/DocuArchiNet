Public Class Class_parram_consultarInformacionSello
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
    Public inscripciones As InscripcionesInformacionSello()
    Public Property inscripciones_() As InscripcionesInformacionSello()
        Get
            Return inscripciones
        End Get
        Set(ByVal value As InscripcionesInformacionSello())
            inscripciones = value
        End Set
    End Property
End Class
