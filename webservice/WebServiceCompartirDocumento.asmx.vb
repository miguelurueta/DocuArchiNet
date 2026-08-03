Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceCompartirDocumento
    Inherits System.Web.Services.WebService
    <WebMethod()>
    Public Function ServiceRegistraDecisionSolicitudAprobacion(ByVal IdDocumentoCompartidoUsuario As Object,
                                                               ByVal NotaRegistroDecision As Object,
                                                               ByVal DescripcionDecision As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servcio web que expone el registro de decisión de un documento compartido
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdDocumentoCompartidoUsuario : Representa la identificación del registro del documento compartido
        '                               al usuario 
        'NotaRegistroDecision        : Representa la nota del registro de decisión 
        'DescripcionDecision         : Representa la descripción de la decisicón del usuario
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CdCompartirDocunento  : Retorna la estructura del documento compartido
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-15
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim CdCompartirDocunento = New List(Of CdCompartirDocunento)()
        Dim IlstCdCompartirDocunento As CdCompartirDocunento = New CdCompartirDocunento
        Try
            Dim ClassGaCompartirDocumento As New ClassGaCompartirDocumento
            IlstCdCompartirDocunento.IdDcoumento = 0
            IlstCdCompartirDocunento.Gabinete = ""
            IlstCdCompartirDocunento.AppError = ClassGaCompartirDocumento.ConfirmaDesicionDocumentoCompartido(IdDocumentoCompartidoUsuario,
                                                                                                              NotaRegistroDecision,
                                                                                                              DescripcionDecision,
                                                                                                              IlstCdCompartirDocunento.ResultadoEnvioCorreo,
                                                                                                              IlstCdCompartirDocunento.EstadoCambioSolicitudUsuario,
                                                                                                              IlstCdCompartirDocunento.EstadoResultadoAprobacion,
                                                                                                              IlstCdCompartirDocunento.IdDcoumento,
                                                                                                              IlstCdCompartirDocunento.Gabinete)
            CdCompartirDocunento.Add(IlstCdCompartirDocunento)
            Return CdCompartirDocunento
        Catch ex As Exception
            IlstCdCompartirDocunento.AppError = ex.Message
            CdCompartirDocunento.Add(IlstCdCompartirDocunento)
            Return CdCompartirDocunento
        End Try
    End Function

End Class