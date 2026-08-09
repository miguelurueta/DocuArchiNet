Imports System.ComponentModel.Design.Serialization
Imports System.Web.Script.Serialization

Public Class Class_Desserializacion
    Function Deserialize(Of T)(ByVal context As String) As T
        Try
            Dim jsonData As String = context
            Dim obj = DirectCast(New JavaScriptSerializer().Deserialize(Of T)(jsonData), T)
            Return obj
        Catch ex As Exception

        End Try
    End Function
    Function DesSerializacion_SolicitaToken(ByVal param As String,
                                            ByRef stru As SolicitaToken) As String

        Try
            Dim parram As New Class_parram_token
            parram = Deserialize(Of Class_parram_token)(param)
            stru.codigoerror = parram.codigoerror
            stru.mensajeerror = parram.mensajeerror
            stru.token = parram.token
            DesSerializacion_SolicitaToken = "YES"
        Catch ex As Exception
            DesSerializacion_SolicitaToken = "Inconsistencia general función Serializacion_java " & ex.Message
        End Try
    End Function
    Function DesSerializacion_consultarRadicado(ByVal param As String,
                                                ByRef stru As ConsultarRadicado_sii) As String

        Try
            Dim parram As New Class_parram_consultarRadicado
            parram = Deserialize(Of Class_parram_consultarRadicado)(param)
            stru.codigoerror = parram.codigoerror
            stru.mensajeerror = parram.mensajeerror
            stru.radicado = parram.radicado
            stru.tipotramite = parram.tipotramite
            stru.operacion = parram.operacion
            stru.recibo = parram.recibo
            stru.fecharadicacion = parram.fecharadicacion
            stru.matricula = parram.matricula
            stru.proponente = parram.proponente
            stru.idclase = parram.idclase
            stru.identificacion = parram.identificacion
            stru.nombre = parram.nombre
            stru.estadofinal = parram.estadofinal
            stru.usuariofinal = parram.usuariofinal
            stru.fechaestadofinal = parram.fechaestadofinal
            stru.horaestadofinal = parram.horaestadofinal
            stru.sucursalfinal = parram.sucursalfinal
            stru.actoreparto = parram.actoreparto
            stru.tipodoc = parram.tipodoc
            stru.tipodocsirep = parram.tipodocsirep
            stru.tipodocdigitalizacion = parram.tipodocdigitalizacion
            stru.tipoingreso = parram.tipoingreso
            stru.numerodoc = parram.numerodoc
            stru.origendoc = parram.origendoc
            stru.fechadoc = parram.fechadoc
            stru.municipiodoc = parram.municipiodoc
            stru.numerointernorue = parram.numerointernorue
            stru.numerounicorue = parram.numerounicorue
            stru.tipogasto = parram.tipogasto
            stru.subtipotramite = parram.subtipotramite
            stru.cumplorequisitosbenley1780 = parram.cumplorequisitosbenley1780
            stru.mantengorequisitosbenley1780 = parram.mantengorequisitosbenley1780
            stru.renunciobeneficiosley1780 = parram.renunciobeneficiosley1780
            stru.multadoponal = parram.multadoponal
            stru.controlactividadaltoimpacto = parram.controlactividadaltoimpacto
            stru.servicios = CType(parram.servicios, servicios())
            stru.imagenes = CType(parram.imagenes, imagenes())
            stru.estados = CType(parram.estados, estados())
            If stru.matricula = "NUEVANAT" Then
                stru.matricula = ""
            End If
            If stru.proponente = "NUEVANAT" Then
                stru.proponente = ""
            End If
            Dim ClassCarateres As New ClassCarateres
            ClassCarateres.RemplazaCaracteresNoValidos(HttpContext.Current.Session.Item("DG_CDCARACTERES"), stru.nombre)
            DesSerializacion_consultarRadicado = "YES"
        Catch ex As Exception
            DesSerializacion_consultarRadicado = "Inconsistencia general función DesSerializacion_consultarRadicado " & ex.Message
        End Try
    End Function
    Function DesSerializacion_consultarRecibo(ByVal param As String, ByRef stru As consultarRecibo) As String

        Try
            Dim parram As New Class_parram_consultarRecibo
            parram = Deserialize(Of Class_parram_consultarRecibo)(param)
            stru.codigoerror = parram.codigoerror
            stru.mensajeerror = parram.mensajeerror
            stru.recibo = parram.recibo
            stru.fecha = parram.fecha
            stru.hora = parram.hora
            stru.operacion = parram.operacion
            stru.factura = parram.factura
            stru.radicado = parram.radicado
            stru.rutasii = parram.rutasii
            stru.usuario = parram.usuario
            stru.tipogasto = parram.tipogasto
            stru.idclase = parram.idclase
            stru.identificacion = parram.identificacion
            stru.nombre = parram.nombre
            stru.direccion = parram.direccion
            stru.municipio = parram.municipio
            stru.telefono = parram.telefono
            stru.email = parram.email
            stru.tipotramite = parram.tipotramite
            stru.valorneto = parram.valorneto
            stru.tipodoc = parram.tipodoc
            stru.numerodoc = parram.numerodoc
            stru.origendoc = parram.origendoc
            stru.fechadoc = parram.fechadoc
            stru.municipiodoc = parram.municipiodoc
            stru.numerointernorue = parram.numerointernorue
            stru.numerounicorue = parram.numerounicorue
            stru.servicios = CType(parram.servicios, servicios())
            stru.imagenes = CType(parram.imagenes, imagenes())
            Dim ClassCarateres As New ClassCarateres
            ClassCarateres.RemplazaCaracteresNoValidos(HttpContext.Current.Session.Item("DG_CDCARACTERES"), stru.nombre)
            DesSerializacion_consultarRecibo = "YES"
        Catch ex As Exception
            DesSerializacion_consultarRecibo = "Inconsistencia general función DesSerializacion_consultarRecibo " & ex.Message
        End Try
    End Function
    Function DesSerializacion_firma_digital(ByVal param As String, ByRef stru As stru_firma_digital_sii) As String
        Try
            Dim parram As New Class_parram_firma_digital
            parram = Deserialize(Of Class_parram_firma_digital)(param)
            stru.codigoerror = parram.codigoerror
            stru.mensajeerror = parram.mensajeerror
            stru.url = parram.url
            stru.codigofirmapdf = parram.codigofirmapdf
            DesSerializacion_firma_digital = "YES"
        Catch ex As Exception
            DesSerializacion_firma_digital = "Inconsistencia geenral función DesSerializacion_firma_digital " & ex.Message
        End Try
    End Function
    Function DesSerializacion_consultarInformacionSello(ByVal param As String,
                                                        ByRef stru As consultarInformacionSello) As String
        Try
            Dim parram As New Class_parram_consultarInformacionSello
            parram = Deserialize(Of Class_parram_consultarInformacionSello)(param)
            stru.codigoerror = parram.codigoerror
            stru.mensajeerror = parram.mensajeerror
            stru.inscripciones = CType(parram.inscripciones, InscripcionesInformacionSello())
            DesSerializacion_consultarInformacionSello = "YES"
        Catch ex As Exception
            DesSerializacion_consultarInformacionSello = "Inconsistencia general función DesSerializacion_consultarRadicado " & ex.Message
        End Try
    End Function
    Function DesSerializacion_ConsultaExpedienteSIIMercantil(ByVal param As String,
                                                             ByRef ClassConsultaExpedienteSIIMercantil As ClassConsultaExpedienteSIIMercantil) As String
        Try
            Dim parram As New ClassConsultaExpedienteSIIMercantil
            parram = Deserialize(Of ClassConsultaExpedienteSIIMercantil)(param)
            ClassConsultaExpedienteSIIMercantil = CType(parram, ClassConsultaExpedienteSIIMercantil)
            Dim ClassCarateres As New ClassCarateres
            ClassCarateres.RemplazaCaracteresNoValidos(HttpContext.Current.Session.Item("DG_CDCARACTERES"), ClassConsultaExpedienteSIIMercantil.nombre)
            DesSerializacion_ConsultaExpedienteSIIMercantil = "YES"
        Catch ex As Exception
            DesSerializacion_ConsultaExpedienteSIIMercantil = "Inconsistencia general función DesSerializacion_ConsultaExpedienteSIIMercantil " & ex.Message
        End Try
    End Function
    Function DesSerializacion_ConsultaExpedienteSIIProponente(ByVal param As String,
                                                              ByRef ConsultarExpedienteProponente As consultarExpedienteProponente) As String
        Try
            Dim parram As New consultarExpedienteProponente
            parram = Deserialize(Of consultarExpedienteProponente)(param)
            ConsultarExpedienteProponente = CType(parram, consultarExpedienteProponente)
            Dim ClassCarateres As New ClassCarateres
            ClassCarateres.RemplazaCaracteresNoValidos(HttpContext.Current.Session.Item("DG_CDCARACTERES"), ConsultarExpedienteProponente.nombre)
            DesSerializacion_ConsultaExpedienteSIIProponente = "YES"
        Catch ex As Exception
            DesSerializacion_ConsultaExpedienteSIIProponente = "Inconsistencia general función DesSerializacion_ConsultaExpedienteSIIProponente " & ex.Message
        End Try
    End Function
    Function DesSerializacion_SolicitaToken_Abo(ByVal param As String, ByRef stru As SolicitaToken_abo) As String

        Try
            Dim parram As New parram_token_abo
            parram = Deserialize(Of parram_token_abo)(param)
            stru.codigoerror = parram.codigoerror
            stru.mensajeerror = parram.mensajeerror
            stru.token = parram.token
            DesSerializacion_SolicitaToken_Abo = "YES"
        Catch ex As Exception
            DesSerializacion_SolicitaToken_Abo = "Inconsistencia general función  DesSerializacion_SolicitaToken_Abo " & ex.Message
        End Try
    End Function
    Function DesSerializacion_recibirCambioEstadoRadicado(ByVal param As String, ByRef stru As recibirCambioEstadoRadicado) As String
        Try
            Dim parram As New parram_recibirCambioEstadoRadicado
            parram = Deserialize(Of parram_recibirCambioEstadoRadicado)(param)
            stru.codigoerror = parram.codigoerror
            stru.mensajeerror = parram.mensajeerror
            DesSerializacion_recibirCambioEstadoRadicado = "YES"
        Catch ex As Exception
            DesSerializacion_recibirCambioEstadoRadicado = "Inconsistencia general función recibirCambioEstadoRadicado: " & ex.Message
        End Try
    End Function
    Function DesSerializacion_recibirConsultarEstadoRadicado(ByVal param As String, ByRef stru As recibirConsultarEstadoRadicado) As String
        Try
            Dim parram As New parram_recibirConsultarEstadoRadicado
            parram = Deserialize(Of parram_recibirConsultarEstadoRadicado)(param)
            stru.codigoerror = parram.codigoerror
            stru.mensajeerror = parram.mensajeerror
            stru.codigoestado = parram.codigoestado
            DesSerializacion_recibirConsultarEstadoRadicado = "YES"
        Catch ex As Exception
            DesSerializacion_recibirConsultarEstadoRadicado = "Inconsistencia general función DesSerializacion_recibirConsultarEstadoRadicado: " & ex.Message
        End Try
    End Function
    Public Structure SolicitaToken_abo
        Dim codigoerror As String
        Dim mensajeerror As String
        Dim token As String
    End Structure
End Class
Public Class parram_token_abo
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
Public Class parram_recibirCambioEstadoRadicado
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
End Class
Public Structure recibirCambioEstadoRadicado
    Dim codigoerror As String
    Dim mensajeerror As String
End Structure
Public Class parram_recibirConsultarEstadoRadicado
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
    Public codigoestado As String
    Public Property codigoestado_() As String
        Get
            Return codigoestado
        End Get
        Set(ByVal value As String)
            codigoestado = value
        End Set
    End Property
End Class
Public Structure recibirConsultarEstadoRadicado
    Dim codigoerror As String
    Dim mensajeerror As String
    Dim codigoestado As String
End Structure