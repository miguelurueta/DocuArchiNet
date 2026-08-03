Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceVersionDocumento
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_remplaza_version(ByVal parameter As Object)
        '---------------------------------------------------------------------------------------
        'Funcion : Service que remplaza la versión del documento desde culaquier modulo
        '          
        '----------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '----------------------------------------------------------------------------------------
        'parameter.id_imagen   : Representa la identificación de la imagen dentro del
        '                        gabinete.
        'parameter.gabinete    : Representa el nombre del gabinete.
        'parameter.NameModulo  : Representa el nombre del modulo de migración
        '                        
        '----------------------------------------------------------------------------------------
        '                           RETORNO
        '----------------------------------------------------------------------------------------
        'class_list_detalle_version_document : Retorna la estructura del doumento de remplazo
        '                                      
        '----------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '----------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-28
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------
        Dim resultList = New List(Of class_list_detalle_version_document)()
        Dim item_ilist As class_list_detalle_version_document = New class_list_detalle_version_document
        Try
            Dim Result As String = ""
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            Dim deserialize_parameter As class_version_paramerter_replace
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of class_version_paramerter_replace)(parameter)
            Dim RutaDocumento As String = ""
            Dim ExensionDocumento As String = ""
            If deserialize_parameter.NameModulo = "REMPLAZAVERSION" Then
                RutaDocumento = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER_FILE")
            End If
            item_ilist.error_sistema = Class_ra_ver_version_documento.AdjuntaVersionDocumento(deserialize_parameter.NameModulo,
                                                                                                1,
                                                                                                deserialize_parameter.Gabinete,
                                                                                                deserialize_parameter.IdImagen,
                                                                                                RutaDocumento,
                                                                                                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                                HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                                HttpContext.Current.Session.Item("DA_Login_Usuario"),
                                                                                                ExensionDocumento,
                                                                                                item_ilist)
            If item_ilist.error_sistema <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.error_sistema = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_detalle_version_documento(ByVal id_registro_version As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que solicita la url de descarga de documentos
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_version          : Representa la identiifcación del registro de
        '                               version del documento
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of ra_ver_version_documento)()
        Dim item_ilist As ra_ver_version_documento = New ra_ver_version_documento
        Try
            Dim Result As String = ""
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            item_ilist.Error_result = Class_ra_ver_version_documento.Solicita_class_version_documento(id_registro_version,
                                                                                                      item_ilist)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_descarga_version_documento(ByVal id_registro_version As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que solicita la url de descarga de documentos
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_version          : Representa la identiifcación del registro de
        '                               version del documento
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_visor_migracion)()
        Dim item_ilist As class_stru_visor_migracion = New class_stru_visor_migracion
        Try
            Dim Result As String = ""
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            item_ilist.Error_result = Class_ra_ver_version_documento.Descarga_version_documento(id_registro_version,
                                                                                                item_ilist)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_documentos_version(ByVal id_registro_version As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que solicita la estructura de visuzalizacion de documentos
        '          de version para visualziacion
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_version          : Representa la identiifcación del registro de
        '                               version del documento
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_visor_migracion)()
        Dim item_ilist As class_stru_visor_migracion = New class_stru_visor_migracion
        Try
            Dim Result As String = ""
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            item_ilist.Error_result = Class_ra_ver_version_documento.Solicita_documentos_visor_version(id_registro_version,
                                                                                                       item_ilist)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_remplaza_version_documento(ByVal id_imagen As Object, ByVal gabinete As Object, ByVal id_registro_migracion As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Service que remplaza la versión del documento migrado
        '          
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete.
        'gabinete              : Representa el nombre del gabinete.
        'id_registro_migracion : Representa la identificación del registro de migración
        '                        de documentos.
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : 
        '                                      
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-07-01
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_list_vew_migra_documento)()
        Dim item_ilist As class_stru_list_vew_migra_documento = New class_stru_list_vew_migra_documento
        Try
            Dim Result As String = ""
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            Dim Stru_registro_migracion As stru_registro_migracion = Nothing
            item_ilist.Error_result = Class_ra_ver_version_documento.Remplaza_version_documento(id_registro_migracion,
                                                                                                id_imagen,
                                                                                                gabinete,
                                                                                                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                                Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                                Session.Item("DA_Login_Usuario"),
                                                                                                item_ilist.Extension_doc_migrado)


            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_restaura_version_documento_gabinete(ByVal id_registro_version As Object, ByVal tipo_modulo As Integer) As Object
        '--------------------------------------------------------------------------------
        'Funcion : Service restaura versión del documento en el gabinete con la 
        '          identiifcación del registro de version y el tipo de modulo que despliega
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_registro_version   : Representa la identificación del registro de versión
        '                        
        '
        '
        '                        
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : 
        '                                      
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-07-13
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_result_list_detalle_version_document)()
        Dim item_ilist As class_result_list_detalle_version_document = New class_result_list_detalle_version_document
        Try
            Dim Result As String = ""
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            Dim Stru_registro_migracion As stru_registro_migracion = Nothing
            item_ilist.ILIST_lista_detalle_version_document = New List(Of class_list_detalle_version_document)
            item_ilist.Error_result = Class_ra_ver_version_documento.Restaura_version_documento_gabinete(id_registro_version,
                                                                                                         tipo_modulo,
                                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                         Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                                         item_ilist)


            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_elimina_version_documento(ByVal id_registro_version As Object,
                                                      ByVal tipo_modulo As Integer,
                                                      ByVal elimina_permante As Integer,
                                                      ByVal valida_firma_digital As Object) As Object
        '-----------------------------------------------------------------------------------
        'Funcion : Service elimina version del documento  con la 
        '          identiifcación del registro de version y el tipo de modulo que despliega
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_registro_version   : Representa la identificación del registro de versión
        '                        -
        '
        '
        '                        
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : 
        '                                      
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-07-13
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_result_list_detalle_version_document)()
        Dim item_ilist As class_result_list_detalle_version_document = New class_result_list_detalle_version_document
        Try
            Dim Result As String = ""
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            Dim Stru_registro_migracion As stru_registro_migracion = Nothing
            item_ilist.Error_result = Class_ra_ver_version_documento.Elimina_version_documento(id_registro_version,
                                                                                               tipo_modulo,
                                                                                               HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                               Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                               elimina_permante,
                                                                                               0,
                                                                                               valida_firma_digital)


            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_versiones_de_documentos(ByVal id_imagen As Object, ByVal id_gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que retorna la lista de versiones de documentos
        '
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        '                               
        'id_gabinete                  : Representa la identiiccación del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura del detalle de la lista
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_result_list_detalle_version_document)()
        Dim item_ilist As class_result_list_detalle_version_document = New class_result_list_detalle_version_document
        Try
            Dim Result As String = ""
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            item_ilist.Error_result = Class_ra_ver_version_documento.Solicita_lista_versiones_de_documentos(id_imagen,
                                                                                                            id_gabinete,
                                                                                                            item_ilist.ILIST_lista_detalle_version_document)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            Dim Class_system1 As New Class_system1
            item_ilist.Error_result = Class_system1.SolicitaNombreGabinetePorId(id_gabinete,
                                                                             item_ilist.Gabinete)
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
End Class