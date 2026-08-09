Imports System.ComponentModel
Imports System.IO
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports GestionDocumental_Docuarchi.net.Class_config_general_service
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class WebServiceMigracion
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_auto_complete_registro_migracion(ByVal parameter As Object, ByVal value As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que solicita la estructura con los registro de auto 
        '          de auto complete del registro de migración
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter            : Representa la estructura contenedora de los parametros
        '                       nombre de tabla name_table_auto y el dbms de conuslta
        '                       name_dbs_auto.
        'value                : Representa el valor de consulta sobre la tabla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_auto_complete_migracion  : Retorna la estructura con los registtros
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-31
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_auto_complete_migracion)()
        Dim item_ilist As class_stru_auto_complete_migracion = New class_stru_auto_complete_migracion
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service_auto_complete))(parameter)
            Dim Result As String = ""
            Dim name_dbs_auto As String = deserialize_parameter(0).name_dbs_auto
            Dim name_table_auto As String = deserialize_parameter(0).name_table_auto
            Dim name_campo_auto As String = deserialize_parameter(0).name_campo_auto
            Dim value_auto As String = value
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            item_ilist.Error_result = Class_ra_mig_registro_migracion.Solicita_datos_auto_complete_registro_migracion(name_dbs_auto,
                                                                                                                      value_auto,
                                                                                                                      item_ilist.country)
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
    Public Function Service_eliminar_documento_migrado(ByVal id_registro_migracion As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Service que expone la funcion de eliminación del documento migrado y 
        '          actualiza el registro de migracion con el usuario que elimina y el usuario
        '          fecha y actualiza en estado cero la versión del documento eliminado
        '          
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_registro_migracion : Representa la identificación del registro migracion
        '                        
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'Class_ra_mig_registro_migracion : Retorna la estructura del registro de migracion
        '                                      
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-08-28
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_list_vew_migra_documento)()
        Dim item_ilist As class_stru_list_vew_migra_documento = New class_stru_list_vew_migra_documento
        Try
            Dim Result As String = ""
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            item_ilist.Error_result = Class_ra_mig_registro_migracion.Elimina_documento_migrado(id_registro_migracion)

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
    Public Function Service_solicita_estructura_registro_migracion_documento(ByVal id_registro_migracion As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Service solicita la estructura registro migración documento
        '          
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_registro_migracion : Representa la identificación del registro migracion
        '                        
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'Class_ra_mig_registro_migracion : Retorna la estructura del registro de migracion
        '                                      
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-08-28
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_registro_migracion)()
        Dim item_ilist As class_stru_registro_migracion = New class_stru_registro_migracion
        Try
            Dim Result As String = ""
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            item_ilist.Error_result = Class_ra_mig_registro_migracion.Solicita_clase_datos_registro_migracion_documento(id_registro_migracion,
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
    Public Function Service_solicita_estructura_registro_migracion_documento_gestion(ByVal id_imagen As Object, ByVal gabinete As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Service solicita la estructura registro migración documento desde la
        '          la imagen y el gabinete
        '          
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_registro_migracion : Representa la identificación del registro migracion
        '                        
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'Class_ra_mig_registro_migracion : Retorna la estructura del registro de migracion
        '                                      
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-08-28
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_registro_migracion)()
        Dim item_ilist As class_stru_registro_migracion = New class_stru_registro_migracion
        Try
            Dim Result As String = ""
            Dim id_registro_migracion As Long = 0
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            item_ilist.Error_result = Class_ra_mig_registro_migracion.Solicita_id_registro_migracion_imagen(id_imagen,
                                                                                                            gabinete,
                                                                                                            id_registro_migracion)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            item_ilist.Error_result = Class_ra_mig_registro_migracion.Solicita_clase_datos_registro_migracion_documento(id_registro_migracion,
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
    Public Function Service_consulta_documentos_migrados(ByVal parameter As Object,
                                                         ByVal tipo_consulta As Object,
                                                         ByVal valor_consulta As String)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la consulta de documentos migrados
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
        'Fecha                 : 2024-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim serializer = New JavaScriptSerializer()
            Dim Class_config_general_service_ = Nothing
            Dim Result As String = ""
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            Dim Class_system1 As New Class_system1
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Class_config_general_service_ = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If Class_config_general_service_ Is Nothing Then
                iList_class_stru_Row_Gabinete_Generic.Error_result = "Imposible deserealizar los parametros de configuracion"
                resultList.Add(iList_class_stru_Row_Gabinete_Generic)
                Return resultList
            End If
            iList_class_stru_Row_Gabinete_Generic.Error_result = Class_ra_mig_registro_migracion.Consulta_registro_migracion(tipo_consulta,
                                                                                                                            valor_consulta,
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
    Public Function Service_estructura_campos_dynamic_registro_migracion(ByVal parameter As Object)
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de registro de migración para la
        '         tabla dinamica boot
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        '                     
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_campos_table_bostra_table  : Retorna la estructura de campos de tabla
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim item_ilist As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            item_ilist.Error_result = Class_ra_mig_registro_migracion.Solicita_campos_lista_consulta_documentos_migracion(item_ilist.Obj_ilist_fileds_generic)
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
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_interface_busqueda_gabinete(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la estructura de los campos de consulta
        'de los registros de migracion de documentos
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'name_espace_form_control : Representa el nombre del esppacio de nombres
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Retorna la estructura de los campos
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_config_general_service)()
        Try

            Dim des_parmeter_interface_show = New List(Of class_config_general_parmeter_interface_show)()
            des_parmeter_interface_show = JsonConvert.DeserializeObject(Of List(Of class_config_general_parmeter_interface_show))(parameter)
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            Dim Result As String = Class_ra_mig_registro_migracion.Solicita_campos_consulta_documentos_migracion(des_parmeter_interface_show(0).apost_name_content,
                                                                                                                 resultList)

            resultList(0).error_gestion = Result
            Return resultList
        Catch ex As Exception
            resultList(0).error_gestion = ex.Message
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_lista_series_relacionadas_gabinete_migracion(ByVal id_imagen As Object, ByVal gabinete As Object, ByVal id_gabinete As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Servicio que espone las estructuras para clasidicación de la tipologia
        '          documental
        '  
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'id_gabinete           : Representa la identificación del gabinete
        'gabinete              : Representa el nombre del gabinete
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_cambio_tipologia_gabinete : Retorna la estructura de gestion documental 
        '                                  para tipologias
        '                   
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-08-18
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_cambio_tipologia_gabinete)()
        Dim item_ilist As class_cambio_tipologia_gabinete = New class_cambio_tipologia_gabinete
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            item_ilist.Error_result = ClassDaGabinete.Solicita_lista_series_relacionadas_gabinete_migracion(id_imagen,
                                                                                                            id_gabinete,
                                                                                                            gabinete,
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
    Public Function Service_Solicita_estructura_lista_documento_migrado(ByVal id_imagen As Object, ByVal gabinete As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Service solicita la estructura de visualización del documento migrado 
        '          de formato
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : Retorna la estructura del documento
        '                                      migrado
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-06-21
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_list_vew_migra_documento)()
        Dim item_ilist As class_stru_list_vew_migra_documento = New class_stru_list_vew_migra_documento
        Try
            Dim Result As String = ""
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            item_ilist.Error_result = Class_ra_mig_registro_migracion.Solicita_estructura_lista_documento_migrado(id_imagen,
                                                                                                                  gabinete,
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
    Public Function Service_migra_formato_documento(ByVal id_imagen As Object, ByVal gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Service migra formato de documento con la identificación del documento y
        'el nombre del gabinete
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : Retorna la estructura del documento
        '                                      migrado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-22
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_list_vew_migra_documento)()
        Dim item_ilist As class_stru_list_vew_migra_documento = New class_stru_list_vew_migra_documento
        Try
            Dim Result As String = ""
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            If Session.Item("UTIL_MIGRA_FORMATO_ARCHIVO") = 0 Then
                item_ilist.Error_result = "El usuario de gestión no tiene permiso para migrar formato de archivo"
                resultList.Add(item_ilist)
                Return resultList
            End If
            item_ilist.Error_result = Class_ra_mig_registro_migracion.Migra_formato_documento(id_imagen,
                                                                                              gabinete,
                                                                                              Session.Item("GA_IDUSUARIOGESTION"),
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
    Public Function Service_guarda_documento_digitalizado(ByVal id_imagen As Object, ByVal gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Service que guarda documento digitalizado para migracion
        '
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : Retorna la estructura del documento
        '                                      migrado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-08
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_list_vew_migra_documento)()
        Dim item_ilist As class_stru_list_vew_migra_documento = New class_stru_list_vew_migra_documento
        Try
            Dim Result As String = ""
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            If Session.Item("UTIL_MIGRA_LOAD_FORMATO_ARCHIVO") = 0 Then
                item_ilist.Error_result = "El usuario de gestión no tiene permiso para guardar documentos digitalizados para migracion"
                resultList.Add(item_ilist)
                Return resultList
            End If
            Dim ClassWorkflowDigitalizacion As New ClassWorkflowDigitalizacion
            Dim MatriDocument() As String = Nothing
            item_ilist.Error_result = ClassWorkflowDigitalizacion.SolicitaMatrizDocumentosDigitalizados(id_imagen,
                                                                                                        HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"),
                                                                                                        MatriDocument)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            Dim class_stru_list_vew_migra_documento As class_stru_list_vew_migra_documento = Nothing
            Result = Class_ra_mig_registro_migracion.Adjunta_documento_migracion(id_imagen,
                                                                                 gabinete,
                                                                                 MatriDocument(0),
                                                                                 Session.Item("GA_IDUSUARIOGESTION"),
                                                                                 Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                 class_stru_list_vew_migra_documento)
            If Result = "YES" Then
                class_stru_list_vew_migra_documento.Error_result = Result
                resultList.Add(class_stru_list_vew_migra_documento)
            Else
                item_ilist.Error_result = Result
                resultList.Add(item_ilist)
            End If
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_inicializa_intefaz_escaner(ByVal id_imagen As Object, ByVal gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Service que expone la inicialización de la interfaz de digitaliza
        '          para el móudulo de migración
        '
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : Retorna la estructura con la url del
        '                                      componente de migración
        '                                     
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_list_vew_migra_documento)()
        Dim item_ilist As class_stru_list_vew_migra_documento = New class_stru_list_vew_migra_documento
        Try
            Dim Result As String = ""
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            If Session.Item("UTIL_MIGRA_LOAD_FORMATO_ARCHIVO") = 0 Then
                item_ilist.Error_result = "El usuario de gestión no tiene permiso para guardar documentos digitalizados para migracion"
                resultList.Add(item_ilist)
                Return resultList
            End If
            Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString)
            'item_ilist.Error_result = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER")
            If Directory.Exists(Ruttempo) = False Then
                Directory.CreateDirectory(Ruttempo)
            End If
            '--------------------------------
            'Crea ruta escaneo
            '--------------------------------
            'Dim ruta_escaner As String = Ruttempo & "\ESCANERWEB"
            'If Directory.Exists(ruta_escaner) = False Then
            'Directory.CreateDirectory(ruta_escaner)
            'End If
            'HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") = ruta_escaner
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = "MIGRACION"
            item_ilist.url_ruta_documento = "../workflow/WebFormEscan.aspx"
            item_ilist.Error_result = "YES"
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_migra_formato_remplaza_documento(ByVal id_imagen As Object, ByVal gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Service migra  formato y remplaza versión del documento
        'con la identificación del documento y
        'el nombre del gabinete
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : Retorna la estructura del documento
        '                                      migrado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-23
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_list_vew_migra_documento)()
        Dim item_ilist As class_stru_list_vew_migra_documento = New class_stru_list_vew_migra_documento
        Try
            Dim Result As String = ""
            Dim Class_ra_mig_registro_migracion As New Class_ra_mig_registro_migracion
            If Session.Item("UTIL_MIGRA_UPDATE_INDICE_BATCH") = 0 Then
                item_ilist.Error_result = "El usuario de gestión no tiene permiso para migrar formato de archivo por lotes"
                resultList.Add(item_ilist)
                Return resultList
            End If
            item_ilist.Error_result = Class_ra_mig_registro_migracion.Migra_formato_documento(id_imagen,
                                                                                              gabinete,
                                                                                              Session.Item("GA_IDUSUARIOGESTION"),
                                                                                              Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                              item_ilist)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            item_ilist.Error_result = Class_ra_ver_version_documento.Remplaza_version_documento(item_ilist.id_registro_migracion,
                                                                                                id_imagen,
                                                                                                gabinete,
                                                                                                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                                Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                                Session.Item("DA_Login_Usuario"),
                                                                                                item_ilist.Extension_doc_migrado)
            resultList.Add(item_ilist)
            Return resultList
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
End Class