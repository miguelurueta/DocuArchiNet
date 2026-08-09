Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService_tipodocentrante
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_lista_tramite_auto_vinculacion_gabinete(ByVal id_gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que Solicita lista de tramites de auto vinculación de
        'de documento a expediente relacionada el gabinete seleccionado
        '
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_gabinete                  : Representa la identiifcación del gabinete
        '                               
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-19
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resul_service = New List(Of control_general_drow_lista)()
        Dim item As New control_general_drow_lista
        Dim lista_item_drow As New List(Of control_drow_lista)
        Try
            Dim Result As String = ""
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Class_tipo_doc_entrante.Solicita_lista_tramite_auto_vinculacion_gabinete(id_gabinete,
                                                                                              lista_item_drow)
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
            item.error_sistema = "Función Service_solicita_gabinetes_migracion " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function

End Class