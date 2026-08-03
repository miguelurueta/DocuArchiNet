Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Newtonsoft.Json


' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class WebServiceRue
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaCatractrizacionExpedienteRue(ByVal Matricula As Object, ByVal Gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que solicita datos de caracterización expediente SII
        '
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Matricula                    : Representa la matricula rue
        '                               
        'Gabinete                     : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-05-06
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim CdefRues = New List(Of CdefRues)()
        Dim IlstCdefRues As CdefRues = New CdefRues
        Try
            Dim Result As String = ""
            Dim ClassRues As New ClassRues
            IlstCdefRues.CdRues = New CdRues
            IlstCdefRues.ErrorAppp = ClassRues.SolicitaCatractrizacionExpedienteRue(Matricula,
                                                                                    Gabinete,
                                                                                    IlstCdefRues.CdRuesCaracterizacion)
            CdefRues.Add(IlstCdefRues)
            Return CdefRues
        Catch ex As Exception
            IlstCdefRues.ErrorAppp = ex.Message
            CdefRues.Add(IlstCdefRues)
            Return CdefRues
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceIniciaConsultaRue(ByVal ParramRue As Object, ByVal CodigoCamara As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que incicializa la consulta rue de documentos
        '
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'ParramRue                    : Representa la data encriptada del rue
        '                               
        'CodigoCamara                 : Representa el código de la camara
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-05-06
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim CdefRues = New List(Of CdefRues)()
        Dim IlstCdefRues As CdefRues = New CdefRues
        Try
            Dim Result As String = ""
            Dim ClassRues As New ClassRues
            IlstCdefRues.CdRues = New CdRues
            IlstCdefRues.ErrorAppp = ClassRues.IniciaConsultaRue(ParramRue,
                                                                 CodigoCamara,
                                                                 IlstCdefRues.CdRues,
                                                                 IlstCdefRues.NombreEmpresa)
            CdefRues.Add(IlstCdefRues)
            Return CdefRues
        Catch ex As Exception
            IlstCdefRues.ErrorAppp = ex.Message
            CdefRues.Add(IlstCdefRues)
            Return CdefRues
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceConsultaGabineteRue(ByVal _CdefRues As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la consulta de documentos rues
        '          
        '          
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la estructura del parran rue 
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_date_Gabinete_Generic : Retorna la estructura de datos de la consulta
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2025-05-07
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim Class_config_general_service_ As List(Of Class_config_general_service) = Nothing
            Dim CdefRues As List(Of CdefRues) = Nothing
            Dim Result As String = ""
            Dim ClassRues As New ClassRues
            CdefRues = JsonConvert.DeserializeObject(Of List(Of CdefRues))(_CdefRues)
            iList_class_stru_Row_Gabinete_Generic.Error_result = ClassRues.ConsultaGabineteRue(CdefRues.Item(0).CdRues,
                                                                                               CdefRues.Item(0).NombreEmpresa,
                                                                                               iList_class_stru_Row_Gabinete_Generic,
                                                                                               iList_class_stru_Row_Gabinete_Generic.NameTabla)
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        Catch ex As Exception
            iList_class_stru_Row_Gabinete_Generic.Error_result = ex.Message
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        End Try
    End Function
End Class