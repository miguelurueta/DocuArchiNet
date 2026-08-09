Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports GestionDocumental_Docuarchi.net.conect
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json
' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService_Meta_Dato
    Inherits System.Web.Services.WebService
    Dim ra_m_detalle_sis_meta_datos_ As Class_ra_m_detalle_sis_meta_datos_() = New Class_ra_m_detalle_sis_meta_datos_() {}
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_parameter_interface_meta_dato(ByVal id_image As Object, ByVal gabinete As Object) As IEnumerable(Of Class_ra_m_detalle_sis_meta_datos_)
        Dim stru_result_list = ra_m_detalle_sis_meta_datos_.ToList
        Dim stru_result As New Class_ra_m_detalle_sis_meta_datos_
        Try

            Dim Class_ra_m_interface_meta_datos As New Class_ra_m_interface_meta_datos
            Dim stru_detalle_sis_meta_dato() As Class_ra_m_detalle_sis_meta_datos_ = Nothing
            Dim Result As String = ""
            Result = Class_ra_m_interface_meta_datos.Crear_interface_registra_meta_datos(id_image,
                                                                                        gabinete,
                                                                                        stru_detalle_sis_meta_dato)
            If Result <> "YES" Then
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            Else
                For i As Integer = 0 To stru_detalle_sis_meta_dato.Length - 1
                    stru_result_list.Add(stru_detalle_sis_meta_dato(i))
                Next
                Return stru_result_list
            End If
        Catch ex As Exception
            stru_result.ERROR_SERVICE = ex.Message
            stru_result_list.Add(stru_result)
            Return stru_result_list
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_listar_meta_datos_Archivo(ByVal id_image As Object, ByVal gabinete As Object)
        Dim country As New List(Of class_ra_m_meta_archivo_)
        Dim country_ERROR As New List(Of class_ra_m_meta_archivo_)
        Try

            Dim Class_ra_m_registro_meta_dato_archivo As New Class_ra_m_registro_meta_dato_archivo
            Dim Result As String = ""
            Result = Class_ra_m_registro_meta_dato_archivo.Solicita_listar_meta_datos_Archivo(id_image,
                                                                                              gabinete,
                                                                                              country)
            If Result <> "YES" Then
                Dim item As New class_ra_m_meta_archivo_
                item.ERROR_SERVICE = Result
                country_ERROR.Add(item)
                Return country_ERROR
            Else
                Return country
            End If
        Catch ex As Exception
            Dim item As New class_ra_m_meta_archivo_
            item.ERROR_SERVICE = ex.Message
            country_ERROR.Add(item)
            Return country_ERROR
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_agrea_meta_dato_documento(ByVal id_image As Object,
                                                      ByVal gabinete As Object,
                                                      ByVal parameter As Object,
                                                      ByVal radicado As Object,
                                                      ByVal modulo_funcion As Integer,
                                                      ByVal valida_firma_digital As Integer,
                                                      ByVal valida_meta_dato_obligatorio As Integer,
                                                      ByVal id_tarea As Object) As IEnumerable(Of Class_ra_m_detalle_sis_meta_datos_)
        Dim stru_result_list = ra_m_detalle_sis_meta_datos_.ToList
        Dim stru_result As New Class_ra_m_detalle_sis_meta_datos_
        Try

            Dim Class_ra_m_interface_meta_datos As New Class_ra_m_interface_meta_datos
            Dim stru_detalle_sis_meta_dato() As Class_ra_m_detalle_sis_meta_datos_ = Nothing
            Dim deserialize_parameter = Nothing
            Dim valida_expediente_obligatorio As Integer = 0
            deserialize_parameter = JsonConvert.DeserializeObject(Of Class_ra_m_detalle_sis_meta_datos_())(parameter)
            Dim Class_ra_m_registro_meta_dato_archivo As New Class_ra_m_registro_meta_dato_archivo
            Dim Result As String = ""
            Dim id_registro_version As Integer = 0
            Result = Class_ra_m_registro_meta_dato_archivo.Agrega_meta_dato_documento(id_image,
                                                                                      gabinete,
                                                                                      radicado,
                                                                                      id_tarea,
                                                                                      id_registro_version,
                                                                                      modulo_funcion,
                                                                                      valida_firma_digital,
                                                                                      valida_meta_dato_obligatorio,
                                                                                      valida_expediente_obligatorio,
                                                                                      deserialize_parameter)
            If Result <> "YES" Then
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            Else
                stru_result.ESTADO_FIRMA_DIGITAL = deserialize_parameter(0).ESTADO_FIRMA_DIGITAL
                stru_result.ERROR_SERVICE = "YES"
                stru_result_list.Add(stru_result)
                Return stru_result_list
            End If

        Catch ex As Exception
            stru_result.ERROR_SERVICE = ex.Message
            stru_result_list.Add(stru_result)
            Return stru_result_list
        End Try
    End Function
End Class