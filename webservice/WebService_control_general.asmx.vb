Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports GestionDocumental_Docuarchi.net.conect
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports Newtonsoft.Json
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json.Linq
Imports System.Xml
Imports GestionDocumental_Docuarchi.net.Class_config_general_service
Imports GestionDocumental_Docuarchi.net.WebServiceRadicacion
Public Class control_general_drow_lista
    Public Property error_sistema As String
    Public Property item_sistema As List(Of control_drow_lista)
End Class
Public Class control_drow_lista
    Public Property value As String
    Public Property text As String
End Class
Public Class control_general_parameter_file
    Dim id_expediente As Integer
    Dim id_tipo_documento As Integer
    Dim nombre_tipo_documento As String
    Dim estado_adjunta_anexo As Integer
    Dim estado_adjunta_relacionado As Integer
    Dim numero_documento_relacionado As Integer
End Class
' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService_control_general
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_datos_auto_complete_campos_form_control(ByVal parameter As Object, ByVal value As Object)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service_auto_complete))(parameter)
            Dim Result As String = ""
            Dim name_dbs_auto As String = deserialize_parameter(0).name_dbs_auto
            Dim name_table_auto As String = deserialize_parameter(0).name_table_auto
            Dim name_campo_auto As String = deserialize_parameter(0).name_campo_auto
            Dim value_auto As String = value
            Dim Class_config_general_service As New Class_config_general_service
            Result = Class_config_general_service.Solicita_datos_auto_complete_campos_form_control(name_dbs_auto,
                                                                                                   name_table_auto,
                                                                                                   name_campo_auto,
                                                                                                   value_auto,
                                                                                                   country)
            If Result <> "YES" Then
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country

        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_source_list_item_control_general(ByVal id As Object, name_fucion As Object) As IEnumerable(Of control_general_drow_lista)
        Dim resul_service = New List(Of control_general_drow_lista)()
        Dim item As New control_general_drow_lista
        Dim lista_item_drow As New List(Of control_drow_lista)
        Try
            Dim Result As String = ""
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
            item.error_sistema = "Función service_source_list_item_control_general " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_source_ilist_drow_control_general(ByVal value As Object, seralice_config_general_service_drowlist As Object) As IEnumerable(Of Class_service_ilist_drowlist)
        Dim resul_service = New List(Of Class_service_ilist_drowlist)()
        Dim item As New Class_service_ilist_drowlist
        Dim lista_item_drow As New List(Of Class_service_ilist_drowlist)
        Dim lista_item_drow_err As New List(Of Class_service_ilist_drowlist)
        Try
            Dim Class_config_general_service As New Class_config_general_service
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service_drowlist))(seralice_config_general_service_drowlist)
            deserialize_parameter(0).value_condicion = value
            Dim Result As String = ""
            Result = Class_config_general_service.Solicita_datos_drowlist_form_control(deserialize_parameter,
                                                                                       lista_item_drow)
            If Result <> "YES" Then
                item.error_sistema = Result
                lista_item_drow_err.Add(item)
                Return lista_item_drow_err
            Else
                If lista_item_drow Is Nothing Then
                    lista_item_drow = New List(Of Class_service_ilist_drowlist)
                    item.error_sistema = "YES"
                    item.id_value = -11
                    item.value_campo = ""
                    lista_item_drow.Add(item)
                    Return lista_item_drow
                Else
                    lista_item_drow.Item(0).error_sistema = Result
                    Return lista_item_drow
                End If

            End If

        Catch ex As Exception
            item.error_sistema = "Función service_source_ilist_drow_control_general " & ex.Message
            lista_item_drow_err.Add(item)
            Return lista_item_drow_err
        End Try
    End Function
End Class