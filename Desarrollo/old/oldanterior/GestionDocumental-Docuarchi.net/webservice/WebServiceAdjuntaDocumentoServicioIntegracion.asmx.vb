Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceAdjuntaDocumentoServicioIntegracion
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceAdjuntaDocumentoServicioIntegracion(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio activa el servicio de integración de documentos
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : 
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CservicioIntegracionAdjuntaDocumento  : Retorna la estructura del servicio
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-10
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim IListCservicioIntegracionAdjuntaDocumento = New List(Of CservicioIntegracionAdjuntaDocumento)()
        Dim CservicioIntegracionAdjuntaDocumento As CservicioIntegracionAdjuntaDocumento = New CservicioIntegracionAdjuntaDocumento()
        Try
            Dim Result As String = ""
            Dim ClassAdjuntaDocumentoServicioIntegracion As New ClassAdjuntaDocumentoServicioIntegracion
            Dim CTipoDocEntrante As CTipoDocEntrante = New CTipoDocEntrante
            CservicioIntegracionAdjuntaDocumento.ErrorService = ClassAdjuntaDocumentoServicioIntegracion.ActivaAdjuntaDocumentoServicioIntegracion(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                                                                                   HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                                                                                   HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                                                                   CTipoDocEntrante)

            CservicioIntegracionAdjuntaDocumento.CTipoDocEntrante = New List(Of CTipoDocEntrante)
            CservicioIntegracionAdjuntaDocumento.CTipoDocEntrante.Add(CTipoDocEntrante)
            CservicioIntegracionAdjuntaDocumento.NameService = CTipoDocEntrante.RaSerServicioInteracion.NombreServicio
            CservicioIntegracionAdjuntaDocumento.IdServicioIntegracion = CTipoDocEntrante.RaSerServicioInteracion.Id_ser_servicioIntegracion
            IListCservicioIntegracionAdjuntaDocumento.Add(CservicioIntegracionAdjuntaDocumento)
            Return IListCservicioIntegracionAdjuntaDocumento
        Catch ex As Exception
            CservicioIntegracionAdjuntaDocumento.ErrorService = "Inconsistencia general funcion  ServiceAdjuntaDocumentoServicioIntegracion " & ex.Message
            IListCservicioIntegracionAdjuntaDocumento.Add(CservicioIntegracionAdjuntaDocumento)
            Return IListCservicioIntegracionAdjuntaDocumento
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceAdjuntaDocumentoServicioIntegracionEnlace(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio activa el servicio de integración de documentos
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : 
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CservicioIntegracionAdjuntaDocumento  : Retorna la estructura del servicio
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-10
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim IListCservicioIntegracionAdjuntaDocumento = New List(Of CservicioIntegracionAdjuntaDocumento)()
        Dim CservicioIntegracionAdjuntaDocumento As CservicioIntegracionAdjuntaDocumento = New CservicioIntegracionAdjuntaDocumento()
        Try
            Dim Result As String = ""
            Dim ClassAdjuntaDocumentoServicioIntegracion As New ClassAdjuntaDocumentoServicioIntegracion
            Dim CTipoDocEntrante As CTipoDocEntrante = New CTipoDocEntrante
            CservicioIntegracionAdjuntaDocumento.ErrorService = ClassAdjuntaDocumentoServicioIntegracion.ActivaAdjuntaDocumentoServicioIntegracion(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                                                                                                   HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                                                                                   HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                                                                   CTipoDocEntrante)

            CservicioIntegracionAdjuntaDocumento.CTipoDocEntrante = New List(Of CTipoDocEntrante)
            CservicioIntegracionAdjuntaDocumento.CTipoDocEntrante.Add(CTipoDocEntrante)
            CservicioIntegracionAdjuntaDocumento.NameService = CTipoDocEntrante.RaSerServicioInteracion.NombreServicio
            CservicioIntegracionAdjuntaDocumento.IdServicioIntegracion = CTipoDocEntrante.RaSerServicioInteracion.Id_ser_servicioIntegracion
            IListCservicioIntegracionAdjuntaDocumento.Add(CservicioIntegracionAdjuntaDocumento)
            Return IListCservicioIntegracionAdjuntaDocumento
        Catch ex As Exception
            CservicioIntegracionAdjuntaDocumento.ErrorService = "Inconsistencia general funcion  rviceAdjuntaDocumentoServicioIntegracionEnlace " & ex.Message
            IListCservicioIntegracionAdjuntaDocumento.Add(CservicioIntegracionAdjuntaDocumento)
            Return IListCservicioIntegracionAdjuntaDocumento
        End Try
    End Function
End Class