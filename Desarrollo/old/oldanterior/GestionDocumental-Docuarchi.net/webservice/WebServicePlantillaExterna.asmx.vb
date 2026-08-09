Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServicePlantillaExterna
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_estructura_campos_dynamic_polantilla_externa_rue_SII(ByVal name_plantilla As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estructura de campos de una plantilla externa 
        '          para la carga de formatos de excel para la plantilla de rues ccv
        '        
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'name_plantilla               : Representa el nombre de la plantilla plantilla
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'class_campos_table_bostra_table :Representa la estructura de campos  
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-12-11
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_plantilla_Generic)
        Dim item_ilist As class_stru_Row_plantilla_Generic = New class_stru_Row_plantilla_Generic
        Try
            Dim id_plantilla As Integer = 0
            Dim Class_imp01_plantillaimp As New Class_imp01_plantillaimp
            Dim Class_imp01_campos_plantilla As New Class_imp01_campos_plantilla
            item_ilist.Error_result = Class_imp01_plantillaimp.Solicita_identificacion_plantilla_externa_x_nombe(name_plantilla,
                                                                                                                id_plantilla)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            item_ilist.Error_result = Class_imp01_campos_plantilla.Solicita_estructura_campos_dynamic_polantilla_externa_rue_SII(id_plantilla,
                                                                                                                                 item_ilist.Obj_ilist_fileds_generic)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
End Class