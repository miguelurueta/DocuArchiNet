Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceHistoricoCorrespondencia
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceArchivaTramiteHistoricoRadicado(ByVal RadicadoTramite As Object,
                                                           ByVal IdRespuestaRadicado As Object,
                                                           ByVal NotaArchivadoTramite As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion :Servicio que Archiva radicado desde el historico de radicado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RadicadoTramite      : Representa el consecutivo del radicado 
        'NotaArchivadoTramite : Representa la nota del archivo del tramite
        'IdRespuestaRadicado  _ Representa la identifcación de la respuesta
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-17
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim IlisCdGestionrespuesta = New List(Of CdGestionrespuesta)
        Dim ItemCdGestionrespuesta As CdGestionrespuesta = New CdGestionrespuesta()
        Try
            Dim Classgestionrespuesta As New Classgestionrespuesta
            ItemCdGestionrespuesta.AppError = Classgestionrespuesta.ArchivaTramiteHistoricoRadicado(RadicadoTramite,
                                                                                                    IdRespuestaRadicado,
                                                                                                    NotaArchivadoTramite)
            IlisCdGestionrespuesta.Add(ItemCdGestionrespuesta)
            Return IlisCdGestionrespuesta
        Catch ex As Exception
            ItemCdGestionrespuesta.AppError = "Inconsistencia servicio ServiceArchivaTramiteHistoricoRadicado " & ex.Message
            IlisCdGestionrespuesta.Add(ItemCdGestionrespuesta)
            Return IlisCdGestionrespuesta
        End Try

    End Function


End Class