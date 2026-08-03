Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports GestionDocumental_Docuarchi.net.conect
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports Newtonsoft.Json
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Web.Script.Serialization
Imports System.Threading
Imports GestionDocumental_Docuarchi.net.Class_config_general_service

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceConsultaPublica
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_documentos_relacionados_matriculado(ByVal parameter As Object,
                                                                      ByVal tipo_consulta As Object,
                                                                      ByVal valor_consulta As Object,
                                                                      ByVal id_registro_publico As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la consulta de documentos relacionados
        '          a  un matriculado
        '          
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        'tipo_consulta         : Tipo de consulta de gabinete migracion 1 - consulta
        '                        campos  2- Tipo de consulta general todos los campos
        'valor_consulta        : Valor de consulta para tipo de consulta 
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_date_Gabinete_Generic : Retorna la estructura de datos de la consulta
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-10
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim Class_config_general_service_ As List(Of Class_config_general_service) = Nothing
            Dim class_parameter_consulta_documentos_acto As List(Of class_parameter_consulta_documentos_acto) = Nothing
            Dim Result As String = ""
            Dim Class_ra_con_registros_publicos As New Class_ra_con_registros_publicos
            iList_class_stru_Row_Gabinete_Generic.Error_result = Class_ra_con_registros_publicos.Consulta_lista_documentos_relacionados_matriculado(tipo_consulta,
                                                                                                                                                    valor_consulta,
                                                                                                                                                    id_registro_publico,
                                                                                                                                                    Class_config_general_service_,
                                                                                                                                                    iList_class_stru_Row_Gabinete_Generic)
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        Catch ex As Exception
            iList_class_stru_Row_Gabinete_Generic.Error_result = ex.Message
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_documentos_relacionados_actos(ByVal parameter As Object,
                                                                ByVal tipo_consulta As Object,
                                                                ByVal valor_consulta As Object,
                                                                ByVal id_registro_publico As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la consulta de documentos relacionados
        '          a un acto de un matriculado
        '          
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        'tipo_consulta         : Tipo de consulta de gabinete migracion 1 - consulta
        '                        campos  2- Tipo de consulta general todos los campos
        'valor_consulta        : Valor de consulta para tipo de consulta 
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_date_Gabinete_Generic : Retorna la estructura de datos de la consulta
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim Class_config_general_service_ As List(Of Class_config_general_service) = Nothing
            Dim class_parameter_consulta_documentos_acto As List(Of class_parameter_consulta_documentos_acto) = Nothing
            Dim Result As String = ""
            Dim Class_ra_con_registros_publicos As New Class_ra_con_registros_publicos
            If Not parameter Is Nothing Then
                class_parameter_consulta_documentos_acto = JsonConvert.DeserializeObject(Of List(Of class_parameter_consulta_documentos_acto))(parameter)
                If class_parameter_consulta_documentos_acto Is Nothing Then
                    iList_class_stru_Row_Gabinete_Generic.Error_result = "Imposible deserealizar los parametros de configuracion"
                    resultList.Add(iList_class_stru_Row_Gabinete_Generic)
                    Return resultList
                End If
            End If
            iList_class_stru_Row_Gabinete_Generic.Error_result = Class_ra_con_registros_publicos.Consulta_lista_documentos_relacionados_actos(tipo_consulta,
                                                                                                                                              valor_consulta,
                                                                                                                                              class_parameter_consulta_documentos_acto.Item(0).libro,
                                                                                                                                              class_parameter_consulta_documentos_acto.Item(0).inscripcion,
                                                                                                                                              class_parameter_consulta_documentos_acto.Item(0).enlace,
                                                                                                                                              id_registro_publico,
                                                                                                                                              Class_config_general_service_,
                                                                                                                                              iList_class_stru_Row_Gabinete_Generic)
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        Catch ex As Exception
            iList_class_stru_Row_Gabinete_Generic.Error_result = ex.Message
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_lista_actos_expediente(ByVal parameter As Object,
                                                            ByVal tipo_consulta As Object,
                                                            ByVal valor_consulta As Object,
                                                            ByVal id_registro_publico As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la consulta de actos de matricualdos consulta
        '          publica expediente
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        'tipo_consulta         : Tipo de consulta de gabinete migracion 1 - consulta
        '                        campos  2- Tipo de consulta general todos los campos
        'valor_consulta        : Valor de consulta para tipo de consulta 
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_date_Gabinete_Generic : Retorna la estructura de datos de la consulta
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-08
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim Class_config_general_service_ As List(Of Class_config_general_service) = Nothing
            Dim Result As String = ""
            Dim Class_ra_con_registros_publicos As New Class_ra_con_registros_publicos
            If Not parameter Is Nothing Then
                Class_config_general_service_ = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
                If Class_config_general_service_ Is Nothing Then
                    iList_class_stru_Row_Gabinete_Generic.Error_result = "Imposible deserealizar los parametros de configuracion"
                    resultList.Add(iList_class_stru_Row_Gabinete_Generic)
                    Return resultList
                End If
            End If
            iList_class_stru_Row_Gabinete_Generic.Error_result = Class_ra_con_registros_publicos.Consulta_lista_actos_expediente(tipo_consulta,
                                                                                                                                 valor_consulta,
                                                                                                                                 id_registro_publico,
                                                                                                                                 Class_config_general_service_,
                                                                                                                                 iList_class_stru_Row_Gabinete_Generic)
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        Catch ex As Exception
            iList_class_stru_Row_Gabinete_Generic.Error_result = ex.Message
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_consulta_publica_matriculado_gabinete(ByVal parameter As Object,
                                                                  ByVal tipo_consulta As Object,
                                                                  ByVal valor_consulta As Object,
                                                                  ByVal id_registro_publico As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la consulta de documentos matricualdos consulta
        '          publica expediente
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        'tipo_consulta         : Tipo de consulta de gabinete migracion 1 - consulta
        '                        campos  2- Tipo de consulta general todos los campos
        'valor_consulta        : Valor de consulta para tipo de consulta 
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_date_Gabinete_Generic : Retorna la estructura de datos de la consulta
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-07
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim Class_config_general_service_ As List(Of Class_config_general_service) = Nothing
            Dim Result As String = ""
            Dim Class_ra_con_registros_publicos As New Class_ra_con_registros_publicos
            If Not parameter Is Nothing Then
                Class_config_general_service_ = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
                If Class_config_general_service_ Is Nothing Then
                    iList_class_stru_Row_Gabinete_Generic.Error_result = "Imposible deserealizar los parametros de configuracion"
                    resultList.Add(iList_class_stru_Row_Gabinete_Generic)
                    Return resultList
                End If
            End If
            iList_class_stru_Row_Gabinete_Generic.Error_result = Class_ra_con_registros_publicos.Consulta_publica_matriculado_gabinete(tipo_consulta,
                                                                                                                                       valor_consulta,
                                                                                                                                       id_registro_publico,
                                                                                                                                       Class_config_general_service_,
                                                                                                                                       iList_class_stru_Row_Gabinete_Generic)
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        Catch ex As Exception
            iList_class_stru_Row_Gabinete_Generic.Error_result = ex.Message
            resultList.Add(iList_class_stru_Row_Gabinete_Generic)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_lista_tipo_consulta_publica(ByVal id As Object) As IEnumerable(Of control_general_drow_lista)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone los tipos de consulta publica para expedientes
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '                             : 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista tipos 
        '                     value: identificación del registro
        '                      text: Nombre del gabinete  
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-09-06
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resul_service = New List(Of control_general_drow_lista)()
        Dim item As New control_general_drow_lista
        Dim lista_item_drow As New List(Of control_drow_lista)
        Try
            Dim Result As String = ""
            Dim Class_ra_con_registros_publicos As New Class_ra_con_registros_publicos
            Result = Class_ra_con_registros_publicos.Solicita_lista_tipo_consulta_publica(lista_item_drow)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            Else
                item.error_sistema = Result
                item.item_sistema = lista_item_drow
                resul_service.Add(item)
                Return resul_service
            End If
        Catch ex As Exception
            item.error_sistema = "Función Service_solicita_lista_tipo_consulta_publica " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_datos_interface_registro_consulta_publica(ByVal parameter As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Expone el servicio que solicita datos de contrución
        '          del formulario para el registro del usuario de consulta
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter : Representa la estructura del formulario y  los controles
        '                          
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try

            Dim Result As String = ""
            Dim Class_ra_con_usuario_consulta_publica As New Class_ra_con_usuario_consulta_publica
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim des_parmeter_interface_show = New List(Of class_config_general_parmeter_interface_show)()
            des_parmeter_interface_show = JsonConvert.DeserializeObject(Of List(Of class_config_general_parmeter_interface_show))(parameter)
            Result = Class_ra_con_usuario_consulta_publica.Solicita_datos_campos_registro_consulta_publica(des_parmeter_interface_show(0).apost_name_content,
                                                                                                           resultList)
            If Result <> "YES" Then
                resultList.Clear()
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registra_usuario_consulta_publica(ByVal parameter As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Registra el usuario de consulta publica de expediente para ccv
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter             : Representa la estructura del indice extraidos
        '                        de la interface
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_registro_usuario_consulta_publica  : Retorna la idnetificación del
        'usuario radicador
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_ra_con_usuario_consulta_publica)()
        Dim parameter_gestion As class_stru_ra_con_usuario_consulta_publica = New class_stru_ra_con_usuario_consulta_publica()
        Try
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_parameter = Nothing
            Dim Result As String = ""
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_gestion = "Imposible deserealizar los parametros de configuracion"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim Class_ra_con_usuario_consulta_publica As New Class_ra_con_usuario_consulta_publica
            Dim id_registro_usuario As Integer = 0
            Result = Class_ra_con_usuario_consulta_publica.Registro_usuario_consulta_publica(deserialize_parameter,
                                                                                             id_registro_usuario)
            parameter_gestion.error_gestion = Result
            parameter_gestion.id_registro_usuario = id_registro_usuario
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
End Class