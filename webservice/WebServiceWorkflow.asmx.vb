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
Imports GestionDocumental_Docuarchi.net.Class_config_general_service
Public Class table_boot_parameter
    Public table_name_campo As String
    Public table_value_campo As String
    Public table_tipo_campo As String
End Class
Public Class class_item_element
    Public id_item As Integer
End Class
Public Class class_service_workflow
    Public error_result As String
    Public identificador As Integer
    Public value As String
    Public detailt_note As New class_detail_note
End Class

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceWorkflow
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaEstructuraTramiteEnlaceWorkflowLoadFile(ByVal Parameter As Object) As Object
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estrucutura de un tramite workflow cuando esta asignado
        '          
        '          
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------
        'IdTipoTramite       : Representa la identificación del tipo tramite
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-18
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Dim ListCDFileLoadWorkflow = New List(Of CDFileLoadWorkflow)
        Dim CDFileLoadWorkflow As CDFileLoadWorkflow = New CDFileLoadWorkflow()
        Try
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Dim CDParameterFileLoadworkflow = New CDParameterFileLoadworkflow
            If HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE") = -1 Or HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE") = 0 Then
                CDFileLoadWorkflow.AppError = "Seleccione una tarea para poder adjuntar el documento."
                ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
                Return ListCDFileLoadWorkflow
            End If
            If HttpContext.Current.Session("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                CDFileLoadWorkflow.AppError = "El usuario no dispone de permisos para adjuntar documentos en este módulo workflow."
                ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
                Return ListCDFileLoadWorkflow
            End If
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim EstadoLista As String = ""
            Dim estado_resultado As String = ""
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                               EstadoLista)
            If Result <> "YES" Then
                CDFileLoadWorkflow.AppError = Result
                ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
                Return ListCDFileLoadWorkflow
            End If
            CDFileLoadWorkflow.AppError = Class_ra_dig_config_digitalizacion.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                                                      Session.Item("DG_ID_TRAMITE"),
                                                                                                                      Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                                                      0)
            CDParameterFileLoadworkflow.IdTipoTramite = Session.Item("DG_ID_TRAMITE")
            CDParameterFileLoadworkflow.TipoPlantillaTramite = Session.Item("DG_ID_TRAMITE")
            CDParameterFileLoadworkflow.IconfigDigitalizacion = Session.Item("DG_ID_CONFIG_DIGITALIZACION")
            CDFileLoadWorkflow.CDParameterFileLoadworkflow.Add(CDParameterFileLoadworkflow)
            ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
            Return ListCDFileLoadWorkflow
        Catch ex As Exception
            CDFileLoadWorkflow.AppError = ex.Message
            ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
            Return ListCDFileLoadWorkflow
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaEstructuraTramiteAsignadWorkflowLoadFile(ByVal Parameter As Object) As Object
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estrucutura de un tramite workflow cuando esta asignado
        '          
        '          
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------
        'IdTipoTramite       : Representa la identificación del tipo tramite
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-18
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Dim ListCDFileLoadWorkflow = New List(Of CDFileLoadWorkflow)
        Dim CDFileLoadWorkflow As CDFileLoadWorkflow = New CDFileLoadWorkflow()
        Try
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Dim CDParameterFileLoadworkflow = New CDParameterFileLoadworkflow
            If HttpContext.Current.Session("ID_TAREA_SELECCIONDA") = -1 Or HttpContext.Current.Session("ID_TAREA_SELECCIONDA") = 0 Then
                CDFileLoadWorkflow.AppError = "Seleccione una tarea para poder adjuntar el documento."
                ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
                Return ListCDFileLoadWorkflow
            End If
            If HttpContext.Current.Session("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                CDFileLoadWorkflow.AppError = "El usuario no dispone de permisos para adjuntar documentos en este módulo workflow."
                ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
                Return ListCDFileLoadWorkflow
            End If
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim EstadoLista As String = ""
            Dim estado_resultado As String = ""
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"),
                                                                               EstadoLista)
            If Result <> "YES" Then
                CDFileLoadWorkflow.AppError = Result
                ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
                Return ListCDFileLoadWorkflow
            End If
            CDFileLoadWorkflow.AppError = Class_ra_dig_config_digitalizacion.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                                                      Session.Item("DG_ID_TRAMITE"),
                                                                                                                      Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                                                      0)
            CDParameterFileLoadworkflow.IdTipoTramite = Session.Item("DG_ID_TRAMITE")
            CDParameterFileLoadworkflow.TipoPlantillaTramite = Session.Item("DG_ID_TRAMITE")
            CDParameterFileLoadworkflow.IconfigDigitalizacion = Session.Item("DG_ID_CONFIG_DIGITALIZACION")
            CDFileLoadWorkflow.CDParameterFileLoadworkflow.Add(CDParameterFileLoadworkflow)
            ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
            Return ListCDFileLoadWorkflow
        Catch ex As Exception
            CDFileLoadWorkflow.AppError = ex.Message
            ListCDFileLoadWorkflow.Add(CDFileLoadWorkflow)
            Return ListCDFileLoadWorkflow
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaListaTiposDocumentalesTramiteListaAdjunta(ByVal IdTramite As Object)
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista de tipos documentales relacionados a un tipo 
        '          tramite
        '        
        '
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'IdTipoTramite          : Representa la identificación del tipo tramite
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_general_drow_lista        : Retorna la lista tipos documentales
        '                     value: identificación del tipo documento
        '                      text: Nombre del tipo documento
        '
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2025-04-18
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of control_general_drow_lista)()
        Dim item_ilist As control_general_drow_lista = New control_general_drow_lista
        Try
            Dim Result As String = ""
            item_ilist.item_sistema = New List(Of control_drow_lista)
            Dim ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            item_ilist.error_sistema = ra_dig_tipos_docum_lista_chequeo.SolicitaListaTiposDocumentalesTramiteListaAdjunta(IdTramite,
                                                                                                                          "",
                                                                                                                          item_ilist.item_sistema)

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
    Public Function Service_registra_flujo_tarea_workflow_radicado_simple(ByVal id_registro_estado As Integer)
        '-----------------------------------------------------------------------------------
        'Funcion : Servicio que expone el registro de workflow para una radiado de radica
        '          cion simple
        '          
        '         
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_registro_estado  : Representa la idneitifcación del estado de radicación
        '                        
        '
        '                       
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_service_workflow : Retorna la estructura del servicio de registro
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-11-17
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_workflow)
        Dim class_service_workflow As class_service_workflow = New class_service_workflow
        Try

            Dim Result As String = ""
            Dim ClassWorkflow As New ClassWorkflow
            class_service_workflow.error_result = ClassWorkflow.Registra_flujo_tarea_workflow_radicado_simple(HttpContext.Current.Session.Item("Id_actividad_Workflow"),
                                                                                                              HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                                              id_registro_estado)
            resultList.Add(class_service_workflow)
            Return resultList
        Catch ex As Exception
            class_service_workflow.error_result = ex.Message
            resultList.Add(class_service_workflow)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_enviar_tarea_flujo_trabajo_radicacion_simple(ByVal identi_actividad_flujo_destino As Object, ByVal id_tarea_workflow As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el envio de documentos a una actividad o usuario
        '          dentro de un flujo de trabajo
        '          
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la estructura con los parametros
        '                        
        '
        '                       
        '
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
        Dim resultList = New List(Of class_envio_flujo_trabajo)
        Dim class_envio_flujo_trabajo As class_envio_flujo_trabajo = New class_envio_flujo_trabajo
        Try
            Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            class_envio_flujo_trabajo.Error_result = Class_flujo_trabajo_workflow.Enviar_tarea_flujo_trabajo_radicacion_simple(identi_actividad_flujo_destino,
                                                                                                                               id_tarea_workflow,
                                                                                                                               class_envio_flujo_trabajo.Resultado_send_correo)
            resultList.Add(class_envio_flujo_trabajo)
            Return resultList
        Catch ex As Exception
            class_envio_flujo_trabajo.Error_result = ex.Message
            resultList.Add(class_envio_flujo_trabajo)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_listado_actividades_para_envio_tarea_a_flujo(ByVal radicado As Object, ByVal id_tarea_workflow As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la lista de grupos o usuarios para el envio de
        '          tareas 
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la estructura con los parametros
        '                        
        '
        '                       
        '
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

            Dim Result As String = ""
            Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            iList_class_stru_Row_Gabinete_Generic.Error_result = Class_flujo_trabajo_workflow.Solicita_listado_actividades_para_envio_tarea_a_flujo(radicado,
                                                                                                                                                    id_tarea_workflow,
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
    Public Function Service_actualiza_registro_gestion_al_usuario(ByVal Parameter As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Servcio que expone la actualización del registro de usuario
        '          
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Parameter : Representa la estructura con los datos   
        '            del registro de la gestión
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'class_wf_gestion_tarea_usuario_stru : Retorna la estructura del registro de
        '                                      la gestión
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-09-23
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resul_service = New List(Of class_wf_gestion_tarea_usuario_stru)()
        Dim item As class_wf_gestion_tarea_usuario_stru = New class_wf_gestion_tarea_usuario_stru
        Try
            Dim Result As String = ""
            Dim Class_wf_gestion_tarea_usuario As New Class_wf_gestion_tarea_usuario
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of class_wf_gestion_tarea_usuario_stru))(Parameter)
            Result = Class_wf_gestion_tarea_usuario.Actualza_registro_gestion_al_usuario(deserialize_parameter(0))
            item.error_result = Result
            resul_service.Add(item)
            Return resul_service
        Catch ex As Exception
            item.error_result = "Función Service_actualiza_registro_gestion_al_usuario " & ex.Message
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_crea_interfaz_gestion_al_usuario(ByVal parameter As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la estructura con los datos del registro de gestion
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'parameter             : Representa la identificación del registro de gestión
        '                        
        '
        '                       
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_wf_gestion_tarea_usuario_stru : Retorna la estructura de datos de la interfaz
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_wf_gestion_tarea_usuario_stru)
        Dim iList_class_wf_gestion_tarea_usuario_stru As class_wf_gestion_tarea_usuario_stru = New class_wf_gestion_tarea_usuario_stru
        Try
            Dim Class_config_general_service_ = Nothing
            Dim Class_wf_gestion_tarea_usuario As New Class_wf_gestion_tarea_usuario
            iList_class_wf_gestion_tarea_usuario_stru.error_result = Class_wf_gestion_tarea_usuario.Solicita_estructura_registro_gestion(parameter,
                                                                                                                                        iList_class_wf_gestion_tarea_usuario_stru)

            resultList.Add(iList_class_wf_gestion_tarea_usuario_stru)
            Return resultList
        Catch ex As Exception
            iList_class_wf_gestion_tarea_usuario_stru.error_result = ex.Message
            resultList.Add(iList_class_wf_gestion_tarea_usuario_stru)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_gestion_al_usuario(ByVal parameter As Object)
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
            Dim Class_config_general_service_ = Nothing
            Dim Class_wf_gestion_tarea_usuario As New Class_wf_gestion_tarea_usuario
            iList_class_stru_Row_Gabinete_Generic.Error_result = Class_wf_gestion_tarea_usuario.Lista_gestion_al_usuario(3,
                                                                                                                         "",
                                                                                                                         Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                                                         Session.Item("Id_Usuario_Workflow"),
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
    Public Function Service_registra_gestion_al_usuario(ByVal Parameter As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Servcio que expone el registro  de gestión al usuario
        '          
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'class_wf_gestion_tarea_usuario_stru : Representa la estructura con los datos   
        '                                      del registro de la gestión
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'class_wf_gestion_tarea_usuario_stru : Retorna la estructura del registro de
        '                                      la gestión
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-09-22
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resul_service = New List(Of class_wf_gestion_tarea_usuario_stru)()
        Dim item As class_wf_gestion_tarea_usuario_stru = New class_wf_gestion_tarea_usuario_stru
        Try
            Dim Result As String = ""
            Dim Class_wf_gestion_tarea_usuario As New Class_wf_gestion_tarea_usuario
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of class_wf_gestion_tarea_usuario_stru))(Parameter)
            Result = Class_wf_gestion_tarea_usuario.Registro_gestion_al_usuario(Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                Session.Item("Id_Usuario_Workflow"),
                                                                                deserialize_parameter(0))
            item.error_result = Result
            resul_service.Add(item)
            Return resul_service
        Catch ex As Exception
            item.Error_result = "Función Service_registra_gestion_al_usuario " & ex.Message
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_estado_envio_correo_gestion_usuario(ByVal id As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Servcio que expone el estado de envio de correo del tipo de gestión de
        '          usuario
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Id                         : Representa la identificación del tipo de gestión
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'class_wf_tipo_gestion_stru  : Retorna la estructura con el estado de envio de
        '                              correo electrónico
        '                              
        '                     
        '                      
        '
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-09-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resul_service = New List(Of class_wf_tipo_gestion_stru)()
        Dim item As class_wf_tipo_gestion_stru = New class_wf_tipo_gestion_stru
        Try
            Dim Result As String = ""
            Dim Class_wf_tipo_gestion As New Class_wf_tipo_gestion
            Dim estado_envio As Integer = 0
            Result = Class_wf_tipo_gestion.Solicita_estado_envio_correo_gestion_usuario(id, item.estado_envio_correo)
            item.error_result = Result
            resul_service.Add(item)
            Return resul_service
        Catch ex As Exception
            item.error_result = "Función Service_Solicita_estado_envio_correo_gestion_usuario " & ex.Message
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_crea_interface_registro_gestion(ByVal id As Object) As IEnumerable(Of control_general_drow_lista)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone los datos para la creación de la interface
        '          del registro de gestión al usuario workflow
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id                               : Opcional
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista de gabinetes 
        '                     value: identificación del gabinete
        '                      text: Nombre del servicio
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-09-20
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resul_service = New List(Of control_general_drow_lista)()
        Dim item As New control_general_drow_lista
        Dim lista_item_drow As New List(Of control_drow_lista)
        Try
            Dim Result As String = ""
            Dim Class_wf_tipo_gestion As New Class_wf_tipo_gestion
            Result = Class_wf_tipo_gestion.Solicita_lista_tipo_gestion(lista_item_drow)
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
            item.error_sistema = "Función Service_crea_interface_registro_gestion " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_descripcion_tarea_actividad_flujo(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim resultList_error = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            Dim Result As String = ""
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Result = Class_wf_registro_actividaes_flujos_trabajo.Mape_proced_adm_flujo_update_activity_description(parameter, resultList)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo = Nothing
            Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_estructura_actividad_flujo_trabajo(parameter,
                                                                                                             struregistro_actividaes_flujos_trabajo)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                resultList.Item(0).value_campo = struregistro_actividaes_flujos_trabajo.DESCRIPCION_TAREA_ACTIVIDAD
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
    Public Function Service_Actualiza_descripcion_actividad_flujo_trabajo(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service
        Try
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_parameter = Nothing
            Dim Result As String = ""
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Result = Class_wf_registro_actividaes_flujos_trabajo.Actualiza_descripcion_actividad_flujo_trabajo(Val(deserialize_parameter(0).dms_id_registro),
                                                                                                                 deserialize_parameter(0).value_campo)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
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
    Public Function Service_Inactiva_usuario_workflow_balanceo_grupo(ByVal parameter As Object)
        Dim resultList = New List(Of table_boot_lista_usuario_workflow_balance)
        Dim parameter_gestion As table_boot_lista_usuario_workflow_balance = New table_boot_lista_usuario_workflow_balance
        Try
            Dim Result As String = ""
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Result = Class_usuario_workflow.Inactiva_usuario_workflow_balanceo_grupo(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                     parameter)
            parameter_gestion.result = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_cambia_estado_asignacion_usuario_workflow(ByVal parameter As Object, ByVal estado_asig As Object)
        Dim resultList = New List(Of table_boot_lista_usuario_workflow_balance)
        Dim parameter_gestion As table_boot_lista_usuario_workflow_balance = New table_boot_lista_usuario_workflow_balance
        Try
            Dim Result As String = ""
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Result = Class_usuario_workflow.Cambia_estado_asignacion_usuario_workflow(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                     parameter,
                                                                                     estado_asig)
            parameter_gestion.result = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Activa_usuario_workflow_balanceo_grupo(ByVal parameter As Object)
        Dim resultList = New List(Of table_boot_lista_usuario_workflow_balance)
        Dim parameter_gestion As table_boot_lista_usuario_workflow_balance = New table_boot_lista_usuario_workflow_balance
        Try
            Dim Result As String = ""
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Result = Class_usuario_workflow.Activa_usuario_workflow_balanceo_grupo(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                   parameter)
            parameter_gestion.result = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_lista_usuarios_workflow_balanceo(ByVal parameter As Object)
        Dim resultList = New List(Of table_boot_lista_usuario_workflow_balance)
        Dim parameter_gestion As table_boot_lista_usuario_workflow_balance = New table_boot_lista_usuario_workflow_balance
        Try
            If Session.Item("UTIL_GESTION_REASING_USER") = 0 Then
                parameter_gestion.result = "El usuario no tiene permisos para gestionar balanceo de usuarios"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim Result As String = ""
            Dim Class_usuario_workflow As New Class_usuario_workflow
            parameter_gestion.row_usuario_workflow_blance = New List(Of row_usuario_workflow_blance)
            parameter_gestion.result = Class_usuario_workflow.Solicita_lista_usuarios_workflow_balanceo(parameter,
                                                                                                        parameter_gestion.row_usuario_workflow_blance)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_copia_documento_expediente(ByVal parameter As Object)
        Dim list_return = New List(Of class_detail_copia_wf_production)
        Dim parameter_list_return As class_detail_copia_wf_production = New class_detail_copia_wf_production
        Try
            Dim Result As String = ""
            Dim Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
            Result = Class_ra_rel_copia_wf_produccion.Solicita_service_lista_copia_documento_expediente(parameter,
                                                                                                        list_return)
            If Result <> "YES" Then
                parameter_list_return.result = Result
                list_return.Add(parameter_list_return)
                Return list_return
            Else
                Return list_return
            End If
        Catch ex As Exception
            parameter_list_return.result = ex.Message
            list_return.Add(parameter_list_return)
            Return list_return
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_log_procesing_image_workflow(ByVal parameter As Object)
        Dim list_return = New List(Of class_detalle_log_procesing_workflow)
        Dim parameter_list_return As class_detalle_log_procesing_workflow = New class_detalle_log_procesing_workflow
        Try
            Dim Result As String = ""
            Dim Class_logdocuarchi As New Class_logdocuarchi
            Result = Class_logdocuarchi.Solicita_service_detalle_log_procesos_imagen_workflow(parameter,
                                                                                              HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                              list_return)
            If Result <> "YES" Then
                parameter_list_return.result = Result
                list_return.Add(parameter_list_return)
                Return list_return
            Else
                Return list_return
            End If

        Catch ex As Exception

        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_notas_tarea_workflow(ByVal parameter As Object)
        Dim list_return = New List(Of class_detail_note)
        Dim parameter_list_return As class_detail_note = New class_detail_note
        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim Radicado As String = ""
            Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(parameter,
                                                                                Radicado)
            Dim Class_anotacion_tarea As New Class_anotacion_tarea
            Result = Class_anotacion_tarea.Service_solicita_lista_notas_tarea_workflow(parameter,
                                                                                       list_return)
            If Result <> "YES" Then
                Return list_return
            Else
                If list_return(0).id_anotacion <> -1 And Radicado <> "" Then
                    list_return(0).title_anotacion = "Notas del radicado " & Radicado
                Else
                    list_return(0).title_anotacion = ""
                End If
                Return list_return
            End If
        Catch ex As Exception
            parameter_list_return.result = ex.Message
            list_return.Add(parameter_list_return)
            Return list_return
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_add_nota_tarea_workflow(ByVal value_nota As Object)
        Dim list_return = New List(Of class_service_workflow)
        Dim parameter_list_return As class_service_workflow = New class_service_workflow
        Try
            Dim Result As String = ""
            Dim Class_anotacion_tarea As New Class_anotacion_tarea
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Dim class_detail_note As class_detail_note = New class_detail_note
            Result = Class_usuario_workflow.Solicita_caracterizacion_usuario_workflow(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                     class_detail_note.nombre_usuario,
                                                                                     class_detail_note.cargo_usuario,
                                                                                     class_detail_note.loguin_usuario)
            If Result <> "YES" Then
                parameter_list_return.value = ""
                parameter_list_return.error_result = Result
                list_return.Add(parameter_list_return)
                Return list_return
            End If
            Result = Class_anotacion_tarea.Create_note_workflow(value_nota,
                                                                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                                parameter_list_return.identificador,
                                                                class_detail_note.fecha_anotacion)
            If Result <> "YES" Then
                parameter_list_return.value = ""
                parameter_list_return.error_result = Result
                list_return.Add(parameter_list_return)
                Return list_return
            Else
                class_detail_note.dato_anotacion = value_nota
                class_detail_note.id_anotacion = parameter_list_return.identificador
                parameter_list_return.detailt_note = class_detail_note
                parameter_list_return.value = value_nota
                parameter_list_return.error_result = Result
                list_return.Add(parameter_list_return)
                Return list_return
            End If
        Catch ex As Exception
            parameter_list_return.error_result = ex.Message
            list_return.Add(parameter_list_return)
            Return list_return
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_nota_tarea_workflow(ByVal parameter As Object, ByVal value_nota As Object)
        Dim list_return = New List(Of class_service_workflow)
        Dim parameter_list_return As class_service_workflow = New class_service_workflow
        Try
            Dim Result As String = ""
            Dim Class_anotacion_tarea As New Class_anotacion_tarea
            Result = Class_anotacion_tarea.Actualizar_datos_anotacion(value_nota,
                                                                      parameter,
                                                                      HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                      HttpContext.Current.Session.Item("Id_Usuario_Workflow"))
            parameter_list_return.identificador = parameter
            parameter_list_return.value = value_nota
            parameter_list_return.error_result = Result
            list_return.Add(parameter_list_return)
            Return list_return
        Catch ex As Exception
            parameter_list_return.error_result = ex.Message
            list_return.Add(parameter_list_return)
            Return list_return
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_delete_nota_tarea_workflow(ByVal parameter As Object, ByVal value_nota As Object)
        Dim list_return = New List(Of class_service_workflow)
        Dim parameter_list_return As class_service_workflow = New class_service_workflow
        Try
            Dim Result As String = ""
            Dim Class_anotacion_tarea As New Class_anotacion_tarea
            Result = Class_anotacion_tarea.Eliminar_nota_service_workflow(value_nota,
                                                                          parameter,
                                                                          HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                          HttpContext.Current.Session.Item("Id_Usuario_Workflow"))
            parameter_list_return.identificador = parameter
            parameter_list_return.value = value_nota
            parameter_list_return.error_result = Result
            list_return.Add(parameter_list_return)
            Return list_return
        Catch ex As Exception
            parameter_list_return.error_result = ex.Message
            list_return.Add(parameter_list_return)
            Return list_return
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_contenido_nota_tarea_workflow(ByVal parameter As Object)
        Dim list_return = New List(Of class_service_workflow)
        Dim parameter_list_return As class_service_workflow = New class_service_workflow
        Try
            Dim Result As String = ""
            Dim Class_anotacion_tarea As New Class_anotacion_tarea
            Result = Class_anotacion_tarea.Solicta_nota_tarea(parameter,
                                                             parameter_list_return.value)
            parameter_list_return.identificador = parameter
            parameter_list_return.error_result = Result
            list_return.Add(parameter_list_return)
            Return list_return
        Catch ex As Exception
            parameter_list_return.error_result = ex.Message
            list_return.Add(parameter_list_return)
            Return list_return
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_permisos_usuario_workflow_intgracion_sii(ByVal parameter As Object) As IEnumerable(Of Class_permisos_usuarios_workflow_service)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la estructura de persmisos de usuario para 
        '          integración con el sistema SII
        '          
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                        : 
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2025-01-05
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_permisos_usuarios_workflow_service)()
        Dim parameter_gestion As Class_permisos_usuarios_workflow_service = New Class_permisos_usuarios_workflow_service()
        Try
            Dim Result As String = ""
            parameter_gestion.permisos_int_sii = New class_permisos_integracion_sii
            Dim Class_permisos_usuarios_workflow As New Class_permisos_usuarios_workflow
            parameter_gestion.Error_gestion = Class_permisos_usuarios_workflow.Solicita_permisos_usuario_workflow_intgracion_sii(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                                                                parameter_gestion.permisos_int_sii)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registro_tarea_ruta_sii(ByVal parameter As Object) As IEnumerable(Of class_service_workflow)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el registro de una tarea externa SII
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                        : Representa la estructura de los de los datos
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-12-04
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_workflow)()
        Dim parameter_gestion As class_service_workflow = New class_service_workflow()
        Try
            Dim Result As String = ""
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo
            Dim deserialize_parameter As New List(Of Class_config_general_service)
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_result = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim class_registro_tarea_ccv_SII As New class_registro_tarea_ccv_SII
            For i As Integer = 0 To deserialize_parameter.Count - 1
                Select Case deserialize_parameter.Item(i).name_campo
                    Case "recibo"
                        class_registro_tarea_ccv_SII.recibo = deserialize_parameter.Item(i).value_campo
                    Case "codigo_barras"
                        class_registro_tarea_ccv_SII.codigo_barras = deserialize_parameter.Item(i).value_campo
                    Case "matricula"
                        class_registro_tarea_ccv_SII.matricula = deserialize_parameter.Item(i).value_campo
                    Case "rscocial"
                        class_registro_tarea_ccv_SII.rscocial = deserialize_parameter.Item(i).value_campo
                    Case "id_actividad"
                        class_registro_tarea_ccv_SII.id_actividad = deserialize_parameter.Item(i).value_campo
                    Case "id_tramite"
                        class_registro_tarea_ccv_SII.id_tramite = deserialize_parameter.Item(i).value_campo

                End Select
            Next
            class_registro_tarea_ccv_SII.id_usuario_workflow_transacion = 0
            class_registro_tarea_ccv_SII.codigo_rue = ""
            class_registro_tarea_ccv_SII.option_registra_log = 0
            parameter_gestion.error_result = ClassGestionTareasFlujoTrabajo.Registra_tarea_ruta_SII(class_registro_tarea_ccv_SII)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registro_tarea_flujo_sii(ByVal parameter As Object) As IEnumerable(Of class_service_workflow)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio que expone el registro de una tarea externa SII
        '          
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '----------------------------------------------------------------------------------
        'parameter                        : Representa la estructura de los de los datos
        '----------------------------------------------------------------------------------
        '                           RETORNO
        '----------------------------------------------------------------------------------

        '----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '----------------------------------------------------------------------------------
        'Fecha                 : 2024-12-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_workflow)()
        Dim parameter_gestion As class_service_workflow = New class_service_workflow()
        Try
            Dim Result As String = ""
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo
            Dim deserialize_parameter As New List(Of Class_config_general_service)
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_result = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim class_registro_tarea_ccv_SII As New class_registro_tarea_ccv_SII
            For i As Integer = 0 To deserialize_parameter.Count - 1
                Select Case deserialize_parameter.Item(i).name_campo
                    Case "recibo"
                        class_registro_tarea_ccv_SII.recibo = deserialize_parameter.Item(i).value_campo
                    Case "codigo_barras"
                        class_registro_tarea_ccv_SII.codigo_barras = deserialize_parameter.Item(i).value_campo
                    Case "matricula"
                        class_registro_tarea_ccv_SII.matricula = deserialize_parameter.Item(i).value_campo
                    Case "rscocial"
                        class_registro_tarea_ccv_SII.rscocial = deserialize_parameter.Item(i).value_campo
                    Case "id_actividad"
                        class_registro_tarea_ccv_SII.id_actividad = deserialize_parameter.Item(i).value_campo
                    Case "id_tramite"
                        class_registro_tarea_ccv_SII.id_tramite = deserialize_parameter.Item(i).value_campo
                    Case "id_usuario"
                        class_registro_tarea_ccv_SII.id_usuario = deserialize_parameter.Item(i).value_campo
                    Case "id_flujo"
                        class_registro_tarea_ccv_SII.id_flujo = deserialize_parameter.Item(i).value_campo
                End Select
            Next
            class_registro_tarea_ccv_SII.id_usuario_workflow_transacion = 0
            class_registro_tarea_ccv_SII.codigo_rue = ""
            class_registro_tarea_ccv_SII.option_registra_log = 0
            parameter_gestion.error_result = ClassGestionTareasFlujoTrabajo.Registra_tarea_flujo_SII(class_registro_tarea_ccv_SII)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registro_flujo_trabajo_sii_rue(ByVal parameter As Object) As IEnumerable(Of class_service_workflow)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio que expone el registro de una tarea externa RUE SII
        '          
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '----------------------------------------------------------------------------------
        'parameter      : Representa la estructura de los datos del registro rue
        '                 clase : Class_ConSultaRecibo_Service
        '
        '----------------------------------------------------------------------------------
        '                           RETORNO
        '----------------------------------------------------------------------------------
        'class_service_workflow : Retorna la estructura de servicio RUE
        '----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '----------------------------------------------------------------------------------
        'Fecha                 : 2024-12-28
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_workflow)()
        Dim parameter_gestion As class_service_workflow = New class_service_workflow()
        Try
            Dim Result As String = ""
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo
            Dim deserialize_parameter As New List(Of Class_Integracion_SII_registro_tarea_flujo)
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Class_Integracion_SII_registro_tarea_flujo))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_result = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim class_registro_tarea_ccv_SII As New class_registro_tarea_ccv_SII
            For i As Integer = 0 To deserialize_parameter(0).Class_config_general_service.Count - 1
                Select Case deserialize_parameter(0).Class_config_general_service.Item(i).name_campo
                    Case "recibo"
                        class_registro_tarea_ccv_SII.recibo = deserialize_parameter(0).Class_config_general_service.Item(i).value_campo
                    Case "codigo_barras"
                        class_registro_tarea_ccv_SII.codigo_barras = deserialize_parameter(0).Class_config_general_service.Item(i).value_campo
                    Case "matricula"
                        class_registro_tarea_ccv_SII.matricula = deserialize_parameter(0).Class_config_general_service.Item(i).value_campo
                    Case "rscocial"
                        class_registro_tarea_ccv_SII.rscocial = deserialize_parameter(0).Class_config_general_service.Item(i).value_campo
                    Case "id_actividad"
                        class_registro_tarea_ccv_SII.id_actividad = deserialize_parameter(0).Class_config_general_service.Item(i).value_campo
                    Case "id_actividad_fjujo"
                        class_registro_tarea_ccv_SII.id_actividad_fjujo = deserialize_parameter(0).Class_config_general_service.Item(i).value_campo
                    Case "id_tramite"
                        Dim slplit() As String = deserialize_parameter(0).Class_config_general_service.Item(i).value_campo.Split("|")
                        class_registro_tarea_ccv_SII.id_tramite = slplit(0)
                    Case "id_flujo"
                        class_registro_tarea_ccv_SII.id_flujo = deserialize_parameter(0).Class_config_general_service.Item(i).value_campo
                End Select
            Next
            class_registro_tarea_ccv_SII.id_ruta = deserialize_parameter(0).id_ruta
            class_registro_tarea_ccv_SII.id_usuario = deserialize_parameter(0).id_usuario_workflow
            class_registro_tarea_ccv_SII.id_usuario_workflow_transacion = deserialize_parameter(0).id_usuario_workflow_transacion
            class_registro_tarea_ccv_SII.codigo_rue = deserialize_parameter(0).codigo_rue
            class_registro_tarea_ccv_SII.option_registra_log = deserialize_parameter(0).option_registra_log
            If class_registro_tarea_ccv_SII.id_flujo <> 0 Then
                class_registro_tarea_ccv_SII.id_actividad = class_registro_tarea_ccv_SII.id_actividad_fjujo
                parameter_gestion.error_result = ClassGestionTareasFlujoTrabajo.Registra_tarea_flujo_SII(class_registro_tarea_ccv_SII)
            Else
                parameter_gestion.error_result = ClassGestionTareasFlujoTrabajo.Registra_tarea_ruta_SII(class_registro_tarea_ccv_SII)
            End If
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_datos_imagen_workflow_SII(ByVal parameter As Object) As IEnumerable(Of class_service_workflow)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la funcion de actualización la imagen en  una 
        '          ruta con el consecutivo de recibo SII
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                        : Representa el consecutivo de recibo SII
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-12-09
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_workflow)()
        Dim parameter_gestion As class_service_workflow = New class_service_workflow()
        Try
            Dim Result As String = ""
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo
            parameter_gestion.error_result = ClassGestionTareasFlujoTrabajo.Actualiza_datos_imagen_workflow_SII(parameter)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Read_file_fast_Excell(ByVal parameter As Object) As IEnumerable(Of class_service_workflow)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la funcion de actualización la imagen en  una 
        '          ruta con el consecutivo de recibo SII
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                        : Representa el consecutivo de recibo SII
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-12-09
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_workflow)()
        Dim parameter_gestion As class_service_workflow = New class_service_workflow()
        Try
            Dim Result As String = ""
            Dim Class_FastExcel As New Class_FastExcel
            Dim row As Object = Nothing
            parameter_gestion.error_result = Class_FastExcel.Read_file_fast_Excell(parameter, "", row)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_eliminar_flujo_workflow_SII(ByVal parameter As Object) As IEnumerable(Of class_service_workflow)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la funcion de eliminación de tareas SII
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                        : Representa el consecutivo de recibo SII
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-12-07
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_workflow)()
        Dim parameter_gestion As class_service_workflow = New class_service_workflow()
        Try
            Dim Result As String = ""
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo

            parameter_gestion.error_result = ClassGestionTareasFlujoTrabajo.Eliminar_flujo_workflow_SII(parameter)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_estado_tarea_asignada(ByVal parameter As Object)
        Dim resultList = New List(Of Result_row_estado_tarea)
        Dim parameter_gestion As Result_row_estado_tarea = New Result_row_estado_tarea
        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim id_tarea_workflow As Long = 0
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Result = Class_DAT_ADIC_TAR.Solicita_id_tarea_radicado(parameter, "registropublico", "CODIGO_BARRAS", id_tarea_workflow, 1)
            If Result <> "YES" Then
                parameter_gestion.result = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            parameter_gestion.row_estado_tarea = New List(Of table_boot_row_estado_tarea)
            parameter_gestion.result = Class_estados_tarea_workflow.Lista_estado_tarea_asignada(id_tarea_workflow,
                                                                                                parameter_gestion.row_estado_tarea)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_usuarios_relacionados_actividad_flujo(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura de los usuarios relacionados
        'a una actividad de flujo de trabajo
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la identificación de la actividad del flujo de trabajo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura de los usuarios relacionados
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_list_realcion_usuario_actvida_flujo)
        Dim parameter_gestion As Class_list_realcion_usuario_actvida_flujo = New Class_list_realcion_usuario_actvida_flujo
        Try
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Dim Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            parameter_gestion.Error_gestion = Class_usuario_workflow.Solicita_usuarios_relacionados_actividad_flujo(1,
                                                                                                                    parameter,
                                                                                                                    Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist = Class_service_ilist_drowlist
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_actividades_workflow_flujo_inicio(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura de las actividades relacionadas
        'a un flujo de trabajo
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la identificación del flujo de trabajo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura de las actividades
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-06
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_list_realcion_activida_flujo)
        Dim parameter_gestion As Class_list_realcion_activida_flujo = New Class_list_realcion_activida_flujo
        Try
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            parameter_gestion.Error_gestion = Class_wf_registro_actividaes_flujos_trabajo.Solicita_actividades_workflow_flujo_inicio(1,
                                                                                                                                     parameter,
                                                                                                                                     Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist = Class_service_ilist_drowlist
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_actividades_fjujo_usuario(ByVal parameter As Object, ByVal id_actividad_workflow As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura de las actividades relacionadas
        'a 
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter              : Representa la identificación del flujo de trabajo
        'id_actividad_workflow  : Representa la identiifcación de la actividad workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura de las actividades
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-27
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_list_realcion_activida_flujo)
        Dim parameter_gestion As Class_list_realcion_activida_flujo = New Class_list_realcion_activida_flujo
        Try
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            parameter_gestion.Error_gestion = Class_wf_registro_actividaes_flujos_trabajo.Solicita_actividades_usuario_flujo_trabajo(1,
                                                                                                                                     id_actividad_workflow,
                                                                                                                                     parameter,
                                                                                                                                     Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist = Class_service_ilist_drowlist
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_lista_flujo_defult(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura de flujo relaconado a flujo
        '
        '
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la identificación del flujo de trabajo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_Listado_Actividades_workflow_service  : Retorna el listado de flujos de trabajo
        '                                              
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-27
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of class_envio_flujo_trabajo)
        Dim parameter_gestion As class_envio_flujo_trabajo = New class_envio_flujo_trabajo
        Try
            Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            Dim Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            parameter_gestion.Error_gestion = Class_flujo_trabajo_workflow.Solicita_lista_flujo_trabajo_id_flujo(1,
                                                                                                                 parameter,
                                                                                                                 Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist = Class_service_ilist_drowlist
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_actividades_ruta_usuario(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura de actividades de ruta relacionada
        'a un uusario
        '
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la identificación del flujo de trabajo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_Listado_Actividades_workflow_service  : Retorna el listado de actividades de ruta de 
        '                                              un usuario workflow
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-26
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_Listado_Actividades_workflow_service)
        Dim parameter_gestion As Class_Listado_Actividades_workflow_service = New Class_Listado_Actividades_workflow_service
        Try
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim Class_service_ilist_drowlist = New List(Of Class_service_ilist_drowlist)
            parameter_gestion.Error_gestion = Class_Listado_Actividades_workflow.Solicita_class_actividades_workflow_ruta_default_actividad_usuario(0,
                                                                                                                                                    parameter,
                                                                                                                                                    Class_service_ilist_drowlist)
            parameter_gestion.Class_service_ilist_drowlist = Class_service_ilist_drowlist
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_datos_registro_flujo_virtual_sii(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura de un tramite VIRTUAL SII
        '          para llenar la interface de registro de un tramite SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la estructura del registro  virtual
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura para el registro del tramite virtual
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-03
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_ConSultaRecibo_Service)
        Dim parameter_gestion As Class_ConSultaRecibo_Service = New Class_ConSultaRecibo_Service
        Try
            Dim Result As String = ""
            Dim deserialize_parameter As New List(Of class_row_virtual_sii)
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of class_row_virtual_sii))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.Error_gestion = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo
            parameter_gestion.Error_gestion = ClassGestionTareasFlujoTrabajo.Solicita_datos_registro_flujo_virtual_sii(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                                                       deserialize_parameter,
                                                                                                                       parameter_gestion)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_datos_registro_flujo_rue_sii(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura de un tramite RUE SII
        '          para llenar la interface de registro de una tramite SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la estructura del registro rue
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura para el registro del tramite rue
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-24
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_ConSultaRecibo_Service)
        Dim parameter_gestion As Class_ConSultaRecibo_Service = New Class_ConSultaRecibo_Service
        Try
            Dim Result As String = ""
            Dim deserialize_parameter As New List(Of class_row_rue_sii)
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of class_row_rue_sii))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.Error_gestion = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo
            parameter_gestion.Error_gestion = ClassGestionTareasFlujoTrabajo.Solicita_datos_registro_flujo_rue_sii(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                                                   deserialize_parameter,
                                                                                                                   parameter_gestion)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_id_flujo_relaciondo_a_tramite(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la existencia de un flujo relacionado a tramite
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa nombre del tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '  : Retorna la estructura para el registro del tramite rue
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-26
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_workflow)
        Dim parameter_gestion As class_service_workflow = New class_service_workflow
        Try
            Dim Result As String = ""
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim id_tipo_tramite As Integer = 0
            parameter_gestion.error_result = Class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(parameter,
                                                                                                             id_tipo_tramite)
            If parameter_gestion.error_result <> "YES" Then
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            '---------/// Solicita la identificación del flujo de trabajo en la relación con el tipo tramite   ////-////
            Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
            Dim id_flujo_trabajo As Integer = 0
            parameter_gestion.error_result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_id_flujo_relacion_flujo_tramite(id_tipo_tramite,
                                                                                                                              id_flujo_trabajo)
            If parameter_gestion.error_result <> "YES" Then
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.identificador = id_flujo_trabajo
                resultList.Add(parameter_gestion)
                Return resultList
            End If

        Catch ex As Exception
            parameter_gestion.error_result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura de la consulta del un recibo SII
        '          para el registro de una tarea a un flujo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la estructura del recibo a consultar
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura con los datos del recibo para la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_ConSultaRecibo_Service)
        Dim parameter_gestion As Class_ConSultaRecibo_Service = New Class_ConSultaRecibo_Service
        Try
            Dim Result As String = ""
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo
            parameter_gestion.Error_gestion = ClassGestionTareasFlujoTrabajo.Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII(parameter,
                                                                                                                                         parameter_gestion)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura de la consulta del un recibo SII
        '          para el registro de una tarea a una ruta
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la estructura del recibo a consultar
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura con los datos del recibo para la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_ConSultaRecibo_Service)
        Dim parameter_gestion As Class_ConSultaRecibo_Service = New Class_ConSultaRecibo_Service
        Try
            Dim Result As String = ""
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo
            parameter_gestion.Error_gestion = ClassGestionTareasFlujoTrabajo.Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII(parameter,
                                                                                                                                         parameter_gestion)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_datos_registro_rue_sii(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el consumo de la estructura 
        '          para el registro de una tarea rue SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa la estructura del listado de regitro rue
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_ConSultaRecibo_Service  : Retorna la estructura con los datos del recibo para la interfaz
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_ConSultaRecibo_Service)
        Dim parameter_gestion As Class_ConSultaRecibo_Service = New Class_ConSultaRecibo_Service
        Try
            Dim Result As String = ""
            Dim ClassGestionTareasFlujoTrabajo As New ClassGestionTareasFlujoTrabajo
            parameter_gestion.Error_gestion = ClassGestionTareasFlujoTrabajo.Solicita_datos_estructura_consulta_recibo_flujo_interfaz_SII(parameter,
                                                                                                                                         parameter_gestion)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.Error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_actividades_workflow(ByVal parameter As Object)
        Dim resultList = New List(Of stru_list_actividades)
        Dim parameter_gestion As stru_list_actividades = New stru_list_actividades
        Try
            Dim Result As String = ""
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim stru_estado As stru_estado = Nothing
            Result = Class_estados_tarea_workflow.Solicita_estructura_tarea_asignada(Val(parameter),
                                                                                     stru_estado)
            If Result <> "YES" Then
                parameter_gestion.result = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim id_actividad_tarea_workflow As Long = 0
            Dim id_grupo_workflow As Integer = 0
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            If stru_estado.ID_FLUJO_TRABAJO = 0 Then
                Result = Class_Listado_Actividades_workflow.Lista_actividades_workflow_ruta(1,
                                                                                            resultList)
                If Result <> "YES" Then
                    parameter_gestion.result = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                Else
                    Return resultList
                End If
            Else
                Result = Class_wf_registro_actividaes_flujos_trabajo.Lista_actividades_workflow_flujo(1,
                                                                                                      stru_estado.ID_FLUJO_TRABAJO,
                                                                                                      resultList)
                If Result <> "YES" Then
                    parameter_gestion.result = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                Else
                    Return resultList
                End If
            End If
        Catch ex As Exception
            parameter_gestion.result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_lista_usuario_relacionado_actividad(ByVal parameter As Object, ByVal id_tarea As Object)
        Dim resultList = New List(Of stru_list_usuarios)
        Dim parameter_gestion As stru_list_usuarios = New stru_list_usuarios
        Try
            Dim Result As String = ""
            Dim Class_grupos_workflow As New Class_grupos_workflow
            Dim id_grupo_workflow As Integer = 0
            Dim id_actividad_workflow As Integer = 0
            If parameter = -1 Then
                Dim item As New stru_list_usuarios
                item.id_actividad = -1
                item.nombre_actividad = ""
                item.result = "YES"
                resultList.Add(item)
                Return resultList
            End If
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim stru_estado As stru_estado = Nothing
            Result = Class_estados_tarea_workflow.Solicita_estructura_tarea_asignada(Val(id_tarea),
                                                                                     stru_estado)
            If Result <> "YES" Then
                parameter_gestion.result = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim tipo_actividad As String = ""
            Dim id_tipo_actividad As Integer = 0
            Dim id_agrupacion_actividad As Integer = 0
            Dim nombre_tipo_actividad As String = ""
            If stru_estado.ID_FLUJO_TRABAJO = 0 Then
                Result = Class_Listado_Actividades_workflow.Solicita_tipo_actividad_general_workflow(Val(parameter),
                                                                                                     tipo_actividad,
                                                                                                     id_tipo_actividad,
                                                                                                     id_agrupacion_actividad,
                                                                                                     nombre_tipo_actividad)
                If Result <> "YES" Then
                    parameter_gestion.result = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                End If
                If nombre_tipo_actividad = "USUARIOINDIVIDUAL" Or nombre_tipo_actividad = "USUARIORESPONSABLE" Or nombre_tipo_actividad = "USUARIORESPONSABLERADICADOR" Then
                    Dim item As New stru_list_usuarios
                    item.id_actividad = -1
                    item.nombre_actividad = ""
                    item.result = "YES"
                    resultList.Add(item)
                    Return resultList
                End If
                id_actividad_workflow = Val(parameter)
            Else
                Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
                Dim struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo = Nothing
                Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_estructura_actividad_flujo_trabajo(Val(parameter),
                                                                                                                 struregistro_actividaes_flujos_trabajo)
                If Result <> "YES" Then
                    parameter_gestion.result = Result
                    resultList.Add(parameter_gestion)
                    Return resultList
                End If
                If struregistro_actividaes_flujos_trabajo.ID_USUARIO_WORKFLOW <> 0 Then
                    Dim item As New stru_list_usuarios
                    item.id_actividad = -1
                    item.nombre_actividad = ""
                    item.result = "YES"
                    resultList.Add(item)
                    Return resultList
                End If
                id_actividad_workflow = struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad
            End If
            Result = Class_grupos_workflow.Solicita_id_grupo_actividad_workflow(id_actividad_workflow,
                                                                                id_grupo_workflow)
            If Result <> "YES" Then
                parameter_gestion.result = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Result = Class_usuario_workflow.Solicita_usuarios_activos_relacionado_actividad_grupo(1,
                                                                                                  id_grupo_workflow,
                                                                                                  resultList)
            If Result <> "YES" Then
                parameter_gestion.result = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_reasigna_tarea_usuario_sii_workflow(ByVal id_tarea As Object,
                                                                ByVal id_actividad As Object,
                                                                ByVal id_usuario_worlflow As Object,
                                                                ByVal asigna_tarea_sii As Object)
        Dim resultList = New List(Of stru_list_usuarios)
        Dim parameter_gestion As stru_list_usuarios = New stru_list_usuarios
        Try
            Dim Result As String = ""
            Dim ClassReasignaTareaWorkflowSII As New ClassReasignaTareaWorkflowSII
            parameter_gestion.result = ClassReasignaTareaWorkflowSII.ReasignaTareaUsuarioSIIWorkflow(id_tarea,
                                                                                                     id_actividad,
                                                                                                     id_usuario_worlflow,
                                                                                                     asigna_tarea_sii,
                                                                                                     parameter_gestion)
            'Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            'Dim nombre_usuario As String = ""
            'Dim cargo_usuario As String = ""
            'Dim nombre_actividad As String = ""
            'If Session.Item("UTIL_GESTION_REASING_USER") = 0 Then
            '    parameter_gestion.result = "Usuario sin permisos para reasignar"
            '    resultList.Add(parameter_gestion)
            '    Return resultList
            'End If
            ''--------Valida y retorna los parametros para cambio de estado SII
            'Dim estado_sii As String = ""
            'Dim Radicado_sii As String = ""
            'Dim codigo_corto_sii As String = ""
            'Dim Class_CambiaEstadoSii As New Class_CambiaEstadoSii
            'If asigna_tarea_sii = 1 Then
            '    Result = Class_CambiaEstadoSii.SolicitaDatosCambioEstadoSII(id_usuario_worlflow,
            '                                                            id_actividad,
            '                                                            id_tarea,
            '                                                            estado_sii,
            '                                                            Radicado_sii,
            '                                                            codigo_corto_sii)
            '    If Result <> "YES" Then
            '        parameter_gestion.result = Result
            '        resultList.Add(parameter_gestion)
            '        Return resultList
            '    End If
            'End If
            'Result = Class_estados_tarea_workflow.ReasignaTareaUsuarioSII(id_tarea,
            '                                                                 id_actividad,
            '                                                                 id_usuario_worlflow,
            '                                                                 nombre_usuario,
            '                                                                 cargo_usuario,
            '                                                                 nombre_actividad)
            'parameter_gestion.result = Result
            'parameter_gestion.nombre_actividad = nombre_actividad
            'parameter_gestion.nombre_usuario = nombre_usuario
            'parameter_gestion.cargo_usuario = cargo_usuario
            'parameter_gestion.reault_cambio_estado = "YES"
            'If asigna_tarea_sii = 1 Then
            '    Result = Class_CambiaEstadoSii.Cambia_estado_Radicado(id_usuario_worlflow,
            '                                                          id_actividad,
            '                                                          estado_sii,
            '                                                          Radicado_sii,
            '                                                          codigo_corto_sii)
            '    If Result <> "YES" Then
            '        parameter_gestion.reault_cambio_estado = "Se reasigno la tarea en workflow, pero no se cambio el estado en el sii (" & Result & ")"
            '    Else
            '        parameter_gestion.reault_cambio_estado = Result
            '    End If
            'End If
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_reasigna_tarea_workflow(ByVal id_tarea As Object,
                                                    ByVal id_actividad As Object,
                                                    ByVal id_usuario_worlflow As Object)
        Dim resultList = New List(Of stru_list_usuarios)
        Dim parameter_gestion As stru_list_usuarios = New stru_list_usuarios
        Try
            Dim Result As String = ""
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim nombre_usuario As String = ""
            Dim cargo_usuario As String = ""
            Dim nombre_actividad As String = ""
            Result = Class_estados_tarea_workflow.Reasigna_tarea_workflow(id_tarea,
                                                                          id_actividad,
                                                                          id_usuario_worlflow,
                                                                          nombre_usuario,
                                                                          cargo_usuario,
                                                                          nombre_actividad)
            parameter_gestion.result = Result
            parameter_gestion.nombre_actividad = nombre_actividad
            parameter_gestion.nombre_usuario = nombre_usuario
            parameter_gestion.cargo_usuario = cargo_usuario
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.result = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_exporta_dcoumento_gabinete_workflow(ByVal nombre_gabinete As String,
                                                                ByVal nombre_gabinete_destino As String,
                                                                ByVal id_imagen As Integer) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            If HttpContext.Current.Session.Item("EXPORTA_GABINETE_WORKFLOW") = 0 Then
                parameter_gestion.error_gestion = "YES"
                parameter_gestion.result_service_control = "El usuario no tiene permisos para exportar a gabinete"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            If nombre_gabinete = nombre_gabinete_destino Then
                parameter_gestion.error_gestion = "El sistema no permite exportar a un mismo gabinete"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim id_imagen_copia As Integer = 0
            Result = ClassDaGabinete.Expotar_de_gabinete_a_gabinete(nombre_gabinete,
                                                                    nombre_gabinete_destino,
                                                                    id_imagen,
                                                                    id_imagen_copia)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.result_service_control = ""
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.result_service_control = ""
                resultList.Add(parameter_gestion)
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
    Public Function Service_solicita_lista_gabinetes_permitidos(ByVal id As Object) As IEnumerable(Of control_general_drow_lista)
        Dim resul_service = New List(Of control_general_drow_lista)()
        Dim item As New control_general_drow_lista
        Dim lista_item_drow As New List(Of control_drow_lista)
        Try
            Dim Result As String = ""
            Dim Class_system1 As New Class_system1
            Result = Class_system1.Service_lista_gabinetes_net(lista_item_drow)
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
    Public Function Service_activa_vincula_documento_a_expediente(ByVal parameter As Object)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of class_item_element))(parameter)
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim country_ As New List(Of String)
            Result = ClassGaExpediente.Activa_vincula_documento_expediente(deserialize_parameter)
            country.Add(Result)
            Return country
        Catch ex As Exception
            country.Add(ex.Message)
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_activa_copia_documeento_a_expediente(ByVal parameter As Object)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of class_item_element))(parameter)
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim country_ As New List(Of String)
            Result = ClassGaExpediente.Activa_copia_documento_a_expediente(deserialize_parameter)
            country.Add(Result)
            Return country
        Catch ex As Exception
            country.Add(ex.Message)
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_activa_copia_documento_a_produccion_expediente(ByVal parameter As Object)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of class_item_element))(parameter)
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim country_ As New List(Of String)
            Result = ClassGaExpediente.Activa_copia_documento_a_expediente_produccion(deserialize_parameter)
            country.Add(Result)
            Return country
        Catch ex As Exception
            country.Add(ex.Message)
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_datos_auto_complete_tareas_workflow(ByVal parameter As Object, ByVal value As Object)
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
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.Solicita_datos_auto_complete_tareas_workflow(name_dbs_auto,
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
    Public Function Service_Eval_tarea_default_workflow(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            Dim Result As String = ""
            Dim ClassWorkflow As New ClassWorkflow
            Dim resultado_escript As String = ""
            Result = ClassWorkflow.Eval_tarea_default_workflow(resultado_escript)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                parameter_gestion.result_service_control = resultado_escript
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.result_service_control = resultado_escript
                resultList.Add(parameter_gestion)
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
    Public Function Service_solicita_structucre_consulta_ruta(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.solicita_structucre_consulta_ruta("form_control_consul_campos_dat_adit", resultList)
            If Result <> "YES" Then
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
    Dim Class_configuracion_listado_ruta_valor_campo_ As Class_configuracion_listado_ruta_valor_campo_() = New Class_configuracion_listado_ruta_valor_campo_() {}
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_valor_nombre_campo_radicado_beneficiario(ByVal id_tarea As Object) As IEnumerable(Of Class_configuracion_listado_ruta_valor_campo_)
        Dim stru_result_list = Class_configuracion_listado_ruta_valor_campo_.ToList
        Dim stru_result As New Class_configuracion_listado_ruta_valor_campo_
        Try
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Dim Stru_configuracion_listado_ruta_valor_campo_() As Class_configuracion_listado_ruta_valor_campo_ = Nothing
            Dim Result As String = ""
            Result = Class_configuracion_listado_ruta.Solicita_valor_nombre_campo_radicado_beneficiario(id_tarea,
                                                                                                        Session.Item("WF_RUTAWORKFLOW"),
                                                                                                        Session.Item("WF_CAMPOS_RADICADO_LISTA_TRAMITE"),
                                                                                                        Session.Item("WF_CAMPOS_BENEFICIARIO_LISTA_TRAMITE"),
                                                                                                        Session.Item("WF_CAMPOS_TRAMITE_LISTA_TRAMITE"),
                                                                                                        Stru_configuracion_listado_ruta_valor_campo_)
            If Result <> "YES" Then
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            Else
                For i As Integer = 0 To Stru_configuracion_listado_ruta_valor_campo_.Length - 1
                    stru_result_list.Add(Stru_configuracion_listado_ruta_valor_campo_(i))
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
    Public Function GetPosiblesDatos(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As String()
        Try
            Dim result As New List(Of String)()
            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim response As String = ""
            Dim split() As String = contextKey.Split("|")
            Dim sqlconsult As String = ""
            If prefixText = "*." Then
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & "  LIMIT 30  "
            Else
                sqlconsult = "Select distinct " & split(0) & " from " & split(1) & " where " & split(0) & " like '%" & prefixText & "%' LIMIT 30  "
            End If
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                GetPosiblesDatos = result.ToArray
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If datset.Tables(0).Rows(i).IsNull(0) = False Then
                        Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(0).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = datset.Tables(0).Rows(i).Item(0).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            result.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                        Else
                            result.Add(datset.Tables(0).Rows(i).Item(0).ToString())
                        End If
                    End If
                Next
                GetPosiblesDatos = result.ToArray
            Else
                GetPosiblesDatos = result.ToArray
            End If
        Catch ex As Exception
            GetPosiblesDatos = Nothing
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetPosiblesDatos_Tramites(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim estado_existencia As String = ""
            Dim sql_consulta_texto As String = ""
            Dim sql_consulta As String = ""
            If Len(DName) < 3 Then
                Return country
            End If
            'Dim spli_campos() As String = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE").Split(",")
            'For i As Integer = 0 To spli_campos.Length - 1
            '    If i = 0 Then
            '        sql_consulta_texto = spli_campos(i) & " Like '%" & DName & "%'"
            '    Else
            '        sql_consulta_texto = sql_consulta_texto & " or " & spli_campos(i) & " Like '%" & DName & "%'"
            '    End If
            'Next
            'If sql_consulta_texto = "" Then
            '    sql_consulta_texto = "ESTADO_TRAMITE Like '%" & DName & "%'"
            'Else
            '    sql_consulta_texto = sql_consulta_texto & " or ESTADO_TRAMITE  Like '%" & DName & "%'"
            'End If
            'sql_consulta = "Select distinct " & HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE") & ",ESTADO_TRAMITE AS ESTADO" & "  from " & _
            '                 " estados_tarea_workflow etw " & _
            '                 " inner join dat_adic_tar" & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " as  DAT on " & _
            '                 " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA   ) " & _
            '                 " where (" & sql_consulta_texto & ") " & _
            '                 " and etw.id_actividad=" & HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD") & _
            '                 " and etw.fecha_Seleccion is null and etw.fecha_fin is null and etw.id_usuario=" & _
            '                 HttpContext.Current.Session.Item("Id_Usuario_Workflow") & " and etw.estado_tarea=0 and  estado_modulo_radicado = 1 LIMIT 100"
            'Dim refcconect As New conect.Dbase_Conction_Mysql
            'Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                datset = HttpContext.Current.Session.Item("dat_gred_cahce_restore")
            Else
                datset = HttpContext.Current.Session.Item("dat_gred_cahce")
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If country.Count = 30 Then
                        Exit For
                    End If
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                If InStr(UCase(DName.ToString), UCase(subtrin)) > 0 Then
                                    'country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                                    Me.existencia_item(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0),
                                                   country,
                                                   estado_existencia)
                                    If estado_existencia = "NO" Then
                                        country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                                    End If
                                End If
                            Else
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                If InStr(UCase(subtrin), UCase(DName.ToString)) > 0 Then
                                    'country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                    Me.existencia_item(datset.Tables(0).Rows(i).Item(z).ToString(),
                                                  country,
                                                  estado_existencia)
                                    If estado_existencia = "NO" Then
                                        country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                    End If
                                End If
                            End If
                        End If
                    Next

                Next
                Return country
            Else
                Return country
            End If
            'Dim sqlconsult As String = sql_consulta
            'response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            'If response <> "YES" Then
            '    country.Add(response)
            '    Return country
            '    Exit Function
            'End If
            'If datset.Tables(0).Rows.Count > 0 Then
            '    For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
            '        For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
            '            If datset.Tables(0).Rows(i).IsNull(z) = False Then
            '                Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
            '                If obsgetipe = "System.DateTime" Then
            '                    Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
            '                    Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")

            '                    Me.existencia_item(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0), _
            '                                       country, _
            '                                       estado_existencia)
            '                    If estado_existencia = "NO" Then
            '                        country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
            '                    End If

            '                Else
            '                    Me.existencia_item(datset.Tables(0).Rows(i).Item(z).ToString(), _
            '                                      country, _
            '                                      estado_existencia)
            '                    If estado_existencia = "NO" Then
            '                        country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
            '                    End If

            '                End If
            '            End If
            '        Next

            '    Next
            '    Return country
            'Else
            '    Return country
            'End If
        Catch ex As Exception
            country.Add(ex.Message)
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_lista_actividades(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim estado_existencia As String = ""
            Dim sql_consulta_texto As String = ""
            Dim sql_consulta As String = ""
            If Session.Item("DR_TIPO_ACTIVIDAD_AGREGAR") = "USUARIOINDIVIDUAL" Then
                sql_consulta = "Select idU_suario,Nombre_Usuario as NOMBRE_USUARIO,Cargo_Usuario as CARGO_USUARIO, Area_Usuario AS AREA from usuario_workflow as law " &
                " WHERE (Nombre_Usuario like '%" & DName & "%'" & " or Cargo_Usuario like '%" & DName & "%') and " & " Grupos_Workflow_Rutas_Workflow_id_Ruta=" _
                & Session.Item("DR_ID_RUTA_SELECION_FLUJO") & " ORDER BY Nombre_Usuario"
            Else
                sql_consulta = "Select law.Id_Actividad,law.Nombre_Actividad as ACTIVIDAD,law.Descripcion_Actividad as DESCRIPCION_ACTIVIDAD" &
                 ",agw.Descripcion_Actividad as TIPO_ACTIVIDAD from listado_actividades_workflow as law " &
                 " inner join actividades_generales_workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General) " &
                 " where law.Nombre_Actividad like '%" & DName & "%' or law.Descripcion_Actividad like '%" & DName & "%'"
            End If

            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = sql_consulta
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                country.Add(response)
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")

                                Me.existencia_item(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0),
                                                   country,
                                                   estado_existencia)
                                If estado_existencia = "NO" Then
                                    country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                                End If

                            Else
                                Me.existencia_item(datset.Tables(0).Rows(i).Item(z).ToString(),
                                                  country,
                                                  estado_existencia)
                                If estado_existencia = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next

                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            country.Add(ex.Message)
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_autoriza_tarea_workflow(ByVal DName As String)
        Try
            Dim Result As String = ""
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            If DName = 1 Then
                Result = Class_autoriza_tarea_worklfow.Autoriza_tarea(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                      HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                      HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                      HttpContext.Current.Session.Item("Id_actividad_Workflow"))
                If Result <> "YES" Then
                    Return Result
                End If
            Else
                Result = Class_autoriza_tarea_worklfow.Elimnar_autorizacion_tarea(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                  HttpContext.Current.Session.Item("Id_actividad_Workflow"),
                                                                                  HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
                If Result <> "YES" Then
                    Return Result
                End If

            End If
            Return "YES"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_numero_nota_tarea(ByVal DName As String)
        Dim numero_notas As Integer = 0
        Try
            Dim Result As String = ""
            Dim Ref_Class_anotacion_tarea As New Class_anotacion_tarea
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = -0 Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = -1 Then
                Return numero_notas
            End If
            Result = Ref_Class_anotacion_tarea.Listar_Numero_Anotaciones(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                         numero_notas)
            If Result <> "YES" Then
                Return ""
            Else
                If numero_notas = 0 Then
                    Return ""
                Else
                    Return numero_notas
                End If
            End If
        Catch ex As Exception
            Return numero_notas
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_numero_tareas_pendientes(ByVal DName As String)
        Dim numero_tareas As Integer = 0
        Try
            Dim Result As String = ""
            Dim Ref_Class_tarea_pendiente As New Class_tarea_pendiente
            Result = Ref_Class_tarea_pendiente.Solicita_numero_tareas_pendientes(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                 numero_tareas)
            Return numero_tareas
        Catch ex As Exception
            Return numero_tareas
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_id_tarea_seleccionada(ByVal DName As String)
        Dim numero_tareas As Integer = -2
        Try
            numero_tareas = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            Return numero_tareas
        Catch ex As Exception
            Return numero_tareas
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetPosiblesDatos_lista_tareas_workflow(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            If Len(DName) < 3 Then
                Return country
            End If
            Dim estado_existencia As String = ""
            Dim sql_consulta_texto As String = ""
            Dim sql_consulta As String = ""
            'Dim spli_campos() As String = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_WF").Split(",")
            'For i As Integer = 0 To spli_campos.Length - 1
            '    If i = 0 Then
            '        sql_consulta_texto = spli_campos(i) & " Like '%" & DName & "%'"
            '    Else
            '        sql_consulta_texto = sql_consulta_texto & " or " & spli_campos(i) & " Like '%" & DName & "%'"
            '    End If
            'Next
            'If sql_consulta_texto = "" Then
            '    sql_consulta_texto = "ESTADO_TRAMITE Like '%" & DName & "%'"
            'Else
            '    sql_consulta_texto = sql_consulta_texto & " or ESTADO_TRAMITE  Like '%" & DName & "%'"
            'End If
            If HttpContext.Current.Session("WF_TIPO_LISTA_TRAMITE_HI_WF") = 0 Then
                'sql_consulta = "Select " &
                'HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_WF") &
                '",wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                '" estados_tarea_workflow etw " &
                '" inner join dat_adic_tar" & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " as  DAT on " &
                '" (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA  and estado_modulo_radicado = 0) " &
                '" Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                '" where etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '"  and etw.fecha_fin is null    " &
                '" and etw.id_usuario=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & " or  etw.id_usuario is null"
                '") or ( etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '"  and etw.fecha_fin is null    and etw.id_usuario is null) LIMIT 30"

                'sql_consulta = "Select " &
                'HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_WF") &
                '",wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                '" estados_tarea_workflow etw " &
                '" inner join dat_adic_tar" & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " as  DAT on " &
                '" (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA  and estado_modulo_radicado = 0) " &
                '" Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                '" where (" & sql_consulta_texto & ") " &
                '" and ((etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '"  and etw.fecha_fin is null    " &
                '" and etw.id_usuario=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") &
                '") or ( etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '"  and etw.fecha_fin is null    and etw.id_usuario is null)) LIMIT 30"

                ' sql_consulta = "Select " &
                'HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_WF") &
                '",wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                '" estados_tarea_workflow etw " &
                '" inner join dat_adic_tar" & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " as  DAT on " &
                '" (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA  and estado_modulo_radicado = 0) " &
                '" Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                '" where (" & sql_consulta_texto & ") " &
                '" and ((etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '" and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0  " &
                '" and etw.id_usuario=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") &
                '") or ( etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '" and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0  and etw.id_usuario is null)) LIMIT 30"
            Else
                'sql_consulta = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_SCRIPT_HI_WF") &
                '            " where etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '            " and etw.fecha_fin is null   and etw.id_usuario=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") &
                '            " or ( etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '            "  and etw.fecha_fin is null    and etw.id_usuario is null " &
                '            ") "
                'sql_consulta = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_SCRIPT_HI_WF") &
                '            " where (" & sql_consulta_texto & ") " &
                '            " and ((etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '            "  and etw.fecha_fin is null   and etw.id_usuario=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") &
                '            ") or ( etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '            "  and etw.fecha_fin is null    and etw.id_usuario is null " &
                '            ")) "

                'sql_consulta = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_SCRIPT_HI_WF") &
                '           " where (" & sql_consulta_texto & ") " &
                '           " and ((etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '           " and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0 and etw.id_usuario=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") &
                '           ") or ( etw.id_actividad=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") &
                '           " and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0  and etw.id_usuario is null " &
                '           ")) LIMIT 30 "
            End If
            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim datset As New DataSet
            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                datset = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE")
            Else
                datset = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
            End If
            'Dim sqlconsult As String = sql_consulta
            'response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            'If response <> "YES" Then
            '    country.Add(response)
            '    Return country
            '    Exit Function
            'End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    If country.Count = 30 Then
                        Exit For
                    End If
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                If InStr(UCase(DName.ToString), UCase(subtrin)) > 0 Then
                                    'country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                                    Me.existencia_item(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0),
                                                   country,
                                                   estado_existencia)
                                    If estado_existencia = "NO" Then
                                        country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                                    End If
                                End If
                            Else
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                If InStr(UCase(subtrin), UCase(DName.ToString)) > 0 Then
                                    'country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                    Me.existencia_item(datset.Tables(0).Rows(i).Item(z).ToString(),
                                                  country,
                                                  estado_existencia)
                                    If estado_existencia = "NO" Then
                                        country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                    End If
                                End If
                            End If
                        End If
                    Next

                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            country.Add(ex.Message)
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetPosiblesDatos_lista_tareas_pendientes(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim estado_existencia As String = ""
            Dim sql_consulta_texto As String = ""
            Dim sql_consulta As String = ""
            If DName <> "" Then
                Dim split_campos_lista() As String = HttpContext.Current.Session.Item("WF_MTRI_CAMPOS_LISTA_TAREA_PENDIENTE_HI_WF").Split(",")
                For i As Integer = 0 To split_campos_lista.Length - 1
                    If i = 0 Then
                        sql_consulta_texto = " (" & split_campos_lista(i) & " Like '%" & DName & "%'"
                    Else
                        sql_consulta_texto = sql_consulta_texto & " or " & split_campos_lista(i) & " Like '%" & DName & "%'"
                    End If
                Next
                sql_consulta_texto = sql_consulta_texto & ") "
            End If
            Dim sql_filtro As String = ""
            If DName <> "" Then
                sql_filtro = "(tp.Datos_Pendiente " & " Like '%" & DName & "%' ) and "
            End If
            sql_consulta = "SELECT " & HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TAREA_PENDIENTE_HI_WF") & ", tp.ESTADO_PENDIENTE FROM  dat_adic_tar" & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " as dat  " &
            "inner join TAREA_PENDIENTE as tp on " &
            " (dat.inicio_tareas_workflow_id_tarea=tp.inicio_tareas_workflow_id_tarea and  INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" &
            HttpContext.Current.Session("Id_Ruta_Workflow") & " AND ID_USUARIO=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") &
            " AND ID_ACTIVIDAD=" & HttpContext.Current.Session.Item("Id_actividad_Workflow") & " AND ESTADOS_PENDIENTE=1) " &
            " where " & sql_consulta_texto & " " &
            " order by tp.Id_Pendiente desc"
            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = sql_consulta
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                country.Add(response)
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")

                                Me.existencia_item(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0),
                                                   country,
                                                   estado_existencia)
                                If estado_existencia = "NO" Then
                                    country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                                End If

                            Else
                                Me.existencia_item(datset.Tables(0).Rows(i).Item(z).ToString(),
                                                  country,
                                                  estado_existencia)
                                If estado_existencia = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next

                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            country.Add(ex.Message)
            Return country
        End Try
    End Function
    Function existencia_item(ByVal valor_item As String, _
                             ByVal country As Object, _
                             ByRef estado_existencia As String) As String
        Try
            estado_existencia = "NO"
            For i As Integer = 0 To country.Count - 1
                If Trim(country(i).ToString) = Trim(valor_item) Then
                    estado_existencia = "YES"
                    Exit For
                    Exit Function
                End If
            Next
            existencia_item = "YES"
            Exit Function
        Catch ex As Exception
            existencia_item = "Inconsistencia general función existencia_item " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)> _
<Script.Services.ScriptMethod()> _
    Public Function GetPosiblesDatos_Tramites_historico(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim estado_existencia As String = ""
            Dim sql_consulta_texto As String = ""
            Dim sql_consulta As String = ""
            Dim spli_campos() As String = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE").Split(",")
            For i As Integer = 0 To spli_campos.Length - 1
                If i = 0 Then
                    sql_consulta_texto = spli_campos(i) & " Like '%" & DName & "%'"
                Else
                    sql_consulta_texto = sql_consulta_texto & " or " & spli_campos(i) & " Like '%" & DName & "%'"
                End If
            Next
            If sql_consulta_texto = "" Then
                sql_consulta_texto = "ESTADO_TRAMITE Like '%" & DName & "%'"
            Else
                sql_consulta_texto = sql_consulta_texto & " or ESTADO_TRAMITE  Like '%" & DName & "%'"
            End If
            sql_consulta = "Select distinct " & HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE") & ",ESTADO_TRAMITE AS ESTADO" & "  from " & _
                             " estados_tarea_workflow etw " & _
                             " inner join dat_adic_tar" & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " as  DAT on " & _
                             " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA   ) " & _
                             " where (" & sql_consulta_texto & ") " & _
                             " and etw.id_actividad=" & HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD") & _
                             "  and etw.id_usuario=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & " and etw.estado_tarea=0 "
            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = sql_consulta
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                country.Add(response)
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")

                                Me.existencia_item(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0), _
                                                   country, _
                                                   estado_existencia)
                                If estado_existencia = "NO" Then
                                    country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                                End If

                            Else
                                Me.existencia_item(datset.Tables(0).Rows(i).Item(z).ToString(), _
                                                  country, _
                                                  estado_existencia)
                                If estado_existencia = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next

                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            country.Add(ex.Message)
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_usuarios_workflow(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim split_coma() As String = Nothing
            If InStr(DName, ",") > 0 Then
                split_coma = DName.Split(",")

            Else
                ReDim Preserve split_coma(0)
                split_coma(0) = DName
            End If
            If Trim(split_coma(split_coma.Length - 1)) = "" Then
                Return country
                Exit Function
            End If
            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select Relacion_Gestion,login_Usuario,Nombre_Usuario,Cargo_Usuario from usuario_workflow where (Nombre_Usuario like '%" & Trim(split_coma(split_coma.Length - 1)) & "%' or Cargo_Usuario like'%" & Trim(split_coma(split_coma.Length - 1)) & "%') and ESTADO_USUARIO=1 LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    Dim tempo_record As String = "<" & datset.Tables(0).Rows(i).Item(0).ToString() & "> " & datset.Tables(0).Rows(i).Item(2).ToString() & " (" & datset.Tables(0).Rows(i).Item(3).ToString() & ")"
                    tempo_record = tempo_record.Replace(",", "")
                    country.Add(tempo_record)
                Next

                For i As Integer = 0 To country.Count - 1
                    For z As Integer = 0 To split_coma.Length - 1
                        If Trim(country(i).ToString) = Trim(split_coma(z)) Then
                            country.RemoveAt(i)
                            'Return country
                            'Exit Function
                        End If
                    Next
                Next
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
    Public Function GetLista_lista_actividades_ruta(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim datset As New DataSet
            Dim Result As String = ""
            Dim sqlconsulta As String = "SELECT ID_ACTIVIDAD,NOMBRE_ACTIVIDAD as GRUPO,DESCRIPCION_ACTIVIDAD AS DESCRIPCION FROM LISTADO_ACTIVIDADES_WORKFLOW " &
                " WHERE (NOMBRE_ACTIVIDAD like '%" & DName & "%'" &
                " or NOMBRE_ACTIVIDAD like '%" & DName & "%'" &
                " or DESCRIPCION_ACTIVIDAD like '%" & DName & "%'" &
                ") and  RUTAS_WORKFLOW_ID_RUTA=" & HttpContext.Current.Session.Item("Id_Ruta_Workflow") & " order by NOMBRE_ACTIVIDAD"
            response = SELECTION_SELECT_FIELD(sqlconsulta, datset)
            If response <> "YES" Then
                Return country

            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If
                            End If
                        End If
                    Next
                Next
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
    Public Function GetLista_listado_usuarios_workflow_ruta(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim datset As New DataSet
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select UW.NOMBRE_USUARIO," &
                "UW.CARGO_USUARIO,UW.LOGIN_USUARIO from USUARIO_WORKFLOW as UW " &
                "Inner join GRUPOS_WORKFLOW as GW on " &
                "(GW.ID_GRUPO=UW.GRUPOS_WORKFLOW_ID_GRUPO) " &
                " WHERE (NOMBRE_USUARIO like '%" & DName & "%'" &
                " or CARGO_USUARIO like '%" & DName & "%'" &
                " or LOGIN_USUARIO like '%" & DName & "%'" &
                ") and  UW.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" & HttpContext.Current.Session.Item("Id_Ruta_Workflow") & " and ESTADO_USUARIO=1 and UTIL_ASIGNA_TAREA=1  ORDER BY UW.NOMBRE_USUARIO ASC"
            response = SELECTION_SELECT_FIELD(sqlconsulta, datset)
            If response <> "YES" Then
                Return country

            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If
                            End If
                        End If
                    Next
                Next
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
    Public Function GetLista_listado_usuarios_workflow_ruta_asignacion(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim datset As New DataSet
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select " &
                "UW.LOGIN_USUARIO from USUARIO_WORKFLOW as UW " &
                "Inner join GRUPOS_WORKFLOW as GW on " &
                "(GW.ID_GRUPO=UW.GRUPOS_WORKFLOW_ID_GRUPO) " &
                " WHERE (NOMBRE_USUARIO like '%" & DName & "%'" &
                " or CARGO_USUARIO like '%" & DName & "%'" &
                " or LOGIN_USUARIO like '%" & DName & "%'" &
                ") and  UW.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" & HttpContext.Current.Session.Item("Id_Ruta_Workflow") & " and ESTADO_USUARIO=1 and UTIL_ASIGNA_TAREA=1  ORDER BY UW.NOMBRE_USUARIO ASC"
            response = SELECTION_SELECT_FIELD(sqlconsulta, datset)
            If response <> "YES" Then
                Return country

            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If
                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    Function Veri_existe_regitro(ByVal country As Object, _
                                 ByVal valor As String, _
                                 ByRef estado_exist As String) As String
        Try
            Veri_existe_regitro = "NO"
            For i As Integer = 0 To country.Count - 1
                If Trim(country(i).ToString) = Trim(valor) Then
                    estado_exist = "YES"
                    Veri_existe_regitro = "YES"
                    Exit Function
                End If
            Next
            Veri_existe_regitro = "YES"
        Catch ex As Exception
            Veri_existe_regitro = "Inconcistencia general función Veri_existe_regitro " & ex.Message
        End Try
    End Function
    Public Class ArrayItem
        Public text As String
        Public value As String
    End Class
    Public Structure arraItem_
        Dim text As String
        Dim value As String
    End Structure
    Public Class paramter_compartir_documento_tokenize
        Public asunto_ As String
        Public nota_ As String
        Public nivel_urgencia_solicitud_ As String
        Public tipo_solicitud_ As String
        Public fecha_limite_ As String
        Public radicado_relacionado_ As String
        Public id_usuario_propietario_ As Integer
        Public matri_documentos_ As String
    End Class

    <WebMethod(EnableSession:=True)> _
    <Script.Services.ScriptMethod()> _
    Public Function Set_compartir_documentos(ByVal item_user As Object, _
                                             ByVal parameter As Object)

        Dim response As String = ""
        Try
            Dim parram() As stru_usuario_gestion_compartido
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_user = Nothing
            Dim deserialize_parameter = Nothing
            deserialize_user = serializer.Deserialize(Of List(Of ArrayItem))(item_user)
            If deserialize_user Is Nothing Then
                Return "Imposible deserealizar los parametros de usuario"
                Exit Function
            End If
            deserialize_parameter = serializer.Deserialize(Of List(Of paramter_compartir_documento_tokenize))(parameter)
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion"
                Exit Function
            End If

            For z As Integer = 0 To deserialize_user.Count - 1
                ReDim Preserve parram(z)
                parram(z).id_usuario_gestion = deserialize_user(z).value
                parram(z).cargo_usuario = deserialize_user(z).text
            Next

            Dim Result As String = ""
            Dim Refclas As New ClassGaCompartirDocumento
            If deserialize_parameter(0).asunto_ Is Nothing Then
                deserialize_parameter(0).asunto_ = ""
            End If
            If deserialize_parameter(0).nota_ Is Nothing Then
                deserialize_parameter(0).nota_ = ""
            End If
            If deserialize_parameter(0).nivel_urgencia_solicitud_ Is Nothing Then
                deserialize_parameter(0).nivel_urgencia_solicitud_ = ""
            End If
            If deserialize_parameter(0).tipo_solicitud_ Is Nothing Then
                deserialize_parameter(0).tipo_solicitud_ = ""
            End If
            If deserialize_parameter(0).fecha_limite_ Is Nothing Then
                deserialize_parameter(0).fecha_limite_ = ""
            End If
            Dim split_sel() As String = deserialize_parameter(0).matri_documentos_.Split("|")
            Dim stru_selcion() As stru_documentos_compartidos = Nothing
            For i As Integer = 0 To split_sel.Length - 1
                ReDim Preserve stru_selcion(i)
                Dim spli_separador() As String = split_sel(i).Split("_")
                stru_selcion(i).id_imagen = spli_separador(2)
                stru_selcion(i).nombre_gabinete = spli_separador(3)
            Next
            Dim Resultado_correo As String = ""
            Result = Refclas.Registra_solicitud_general_documento_compartido_usuario(parram, _
                                                                                    deserialize_parameter(0).asunto_, _
                                                                                    deserialize_parameter(0).nota_, _
                                                                                    deserialize_parameter(0).nivel_urgencia_solicitud_, _
                                                                                    deserialize_parameter(0).tipo_solicitud_, _
                                                                                    deserialize_parameter(0).fecha_limite_, _
                                                                                    Session.Item("GA_STRU_DOCUMENTO_RADICADO"), _
                                                                                    Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                    stru_selcion, _
                                                                                    Resultado_correo)
            If Result <> "YES" Then
                Return Result
            Else
                If Resultado_correo <> "" Then
                    Return Resultado_correo
                End If
                Return "YES"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)> _
  <Script.Services.ScriptMethod()> _
    Public Function Set_Registra_solicitud_aprobacion(ByVal item_user As Object, _
                                                      ByVal parameter As Object)

        Dim response As String = ""
        Try
            Dim parram() As stru_usuario_gestion_compartido
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_user = Nothing
            Dim deserialize_parameter = Nothing
            deserialize_user = serializer.Deserialize(Of List(Of ArrayItem))(item_user)
            If deserialize_user Is Nothing Then
                Return "Imposible deserealizar los parametros de usuario|"
                Exit Function
            End If
            deserialize_parameter = serializer.Deserialize(Of List(Of paramter_compartir_documento_tokenize))(parameter)
            If deserialize_parameter Is Nothing Then
                Return "Imposible deserealizar los parametros de configuracion|"
                Exit Function
            End If

            For z As Integer = 0 To deserialize_user.Count - 1
                ReDim Preserve parram(z)
                parram(z).id_usuario_gestion = deserialize_user(z).value
                parram(z).cargo_usuario = deserialize_user(z).text
            Next

            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            If deserialize_parameter(0).asunto_ Is Nothing Then
                deserialize_parameter(0).asunto_ = ""
            End If
            If deserialize_parameter(0).nota_ Is Nothing Then
                deserialize_parameter(0).nota_ = ""
            End If
            If deserialize_parameter(0).nivel_urgencia_solicitud_ Is Nothing Then
                deserialize_parameter(0).nivel_urgencia_solicitud_ = ""
            End If
            If deserialize_parameter(0).tipo_solicitud_ Is Nothing Then
                deserialize_parameter(0).tipo_solicitud_ = ""
            End If
            If deserialize_parameter(0).fecha_limite_ Is Nothing Then
                deserialize_parameter(0).fecha_limite_ = ""
            End If
            Dim Resultado_correo As String = ""
            Dim valor_campos As String = ""
            Dim id_respeusta As Integer = deserialize_parameter(0).id_usuario_propietario_
            Result = Refclas.Registra_solicitud_aprobacion_new(deserialize_parameter(0).nivel_urgencia_solicitud_, _
                                                               deserialize_parameter(0).nota_, _
                                                               deserialize_parameter(0).fecha_limite_, _
                                                               parram, _
                                                               id_respeusta, _
                                                               Session.Item("GA_IDUSUARIOGESTION"), _
                                                               "", _
                                                               Resultado_correo, _
                                                               valor_campos)
            If Result <> "YES" Then
                Return Result & "|"
            Else
                
                Return "YES|" & valor_campos
            End If
        Catch ex As Exception
            Return ex.Message & "|"
        End Try
    End Function

    <WebMethod(EnableSession:=True)> _
 <Script.Services.ScriptMethod()> _
    Public Function Set_Agrega_usuario_a_la_solicitud_aprobacion(ByVal item_user As Object, _
                                                                 ByVal parameter As Object)

        Dim response As String = ""
        Try
            Dim parram() As stru_usuario_gestion_compartido
            Dim serializer = New JavaScriptSerializer()
            Dim deserialize_user = Nothing
            Dim deserialize_parameter = Nothing
            deserialize_user = serializer.Deserialize(Of List(Of ArrayItem))(item_user)
            If deserialize_user Is Nothing Then
                Return "Imposible deserealizar los parametros de usuario|"
                Exit Function
            End If
           
            For z As Integer = 0 To deserialize_user.Count - 1
                ReDim Preserve parram(z)
                parram(z).id_usuario_gestion = deserialize_user(z).value
                parram(z).cargo_usuario = deserialize_user(z).text
            Next
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim Resultado_correo As String = ""
            Dim valor_campos As String = ""
            Result = Refclas.Agrega_usuario_a_la_solicitud_aprobacion(parram, _
                                                                      Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
                                                                      HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                      "", _
                                                                      Resultado_correo, _
                                                                      parameter, _
                                                                      valor_campos)
            If Result <> "YES" Then
                Return Result & "|"
            Else
                'If Resultado_correo <> "YES" Then
                '    Return Resultado_correo & "|"
                'End If
                Return "YES|" & valor_campos
            End If
        Catch ex As Exception
            Return ex.Message & "|"
        End Try
    End Function
    <WebMethod(EnableSession:=True)> _
    <Script.Services.ScriptMethod()> _
    Public Function GetLista_usuarios_workflow_z2(ByVal DName As String)
        Dim response As String = ""
        'Dim country As List(Of String) = New List(Of String)()
        Dim country As New List(Of ArrayItem)
        Try

            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select Relacion_Gestion,login_Usuario,Nombre_Usuario,Cargo_Usuario from usuario_workflow where (Nombre_Usuario like '%" & Trim(DName) & "%' or Cargo_Usuario like'%" & Trim(DName) & "%') and ESTADO_USUARIO=1 LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    Dim items As New ArrayItem
                    items.text = datset.Tables(0).Rows(i).Item(2).ToString() & " (" & datset.Tables(0).Rows(i).Item(3).ToString() & ")"
                    items.value = datset.Tables(0).Rows(i).Item(0).ToString()
                    country.Add(items)
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)> _
      <Script.Services.ScriptMethod()> _
    Public Function GetLista_usuarios_workflow_tokenize(ByVal DName As String)
        Dim response As String = ""
        Dim country As New List(Of ArrayItem)
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select idU_suario,login_Usuario,Nombre_Usuario,Cargo_Usuario from usuario_workflow where (Nombre_Usuario like '%" & Trim(DName) & "%' or Cargo_Usuario like'%" & Trim(DName) & "%') and ESTADO_USUARIO=1 LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    Dim items As New ArrayItem
                    items.text = datset.Tables(0).Rows(i).Item(2).ToString() & " (" & datset.Tables(0).Rows(i).Item(3).ToString() & ")"
                    items.value = datset.Tables(0).Rows(i).Item(0).ToString()
                    country.Add(items)
                Next
                Return country
            Else
                Return country
            End If

        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)> _
      <Script.Services.ScriptMethod()> _
    Public Function GetLista_usuarios_workflow_(ByVal DName As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim split_coma() As String = Nothing
            If InStr(DName, ",") > 0 Then
                split_coma = DName.Split(",")

            Else
                ReDim Preserve split_coma(0)
                split_coma(0) = DName
            End If
            If Trim(split_coma(split_coma.Length - 1)) = "" Then
                Return country
                Exit Function
            End If
            Dim refcconect As New conect.Dbase_Conction_Mysql
            Dim refclas As New ClassRadicador
            Dim datset As New DataSet
            Dim sqlconsult As String = "Select idU_suario,login_Usuario,Nombre_Usuario,Cargo_Usuario from usuario_workflow where (Nombre_Usuario like '%" & Trim(split_coma(split_coma.Length - 1)) & "%' or Cargo_Usuario like'%" & Trim(split_coma(split_coma.Length - 1)) & "%') and ESTADO_USUARIO=1 LIMIT 100"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    Dim tempo_record As String = "<" & datset.Tables(0).Rows(i).Item(0).ToString() & "> " & datset.Tables(0).Rows(i).Item(2).ToString() & " (" & datset.Tables(0).Rows(i).Item(3).ToString() & ")"
                    tempo_record = tempo_record.Replace(",", "")
                    country.Add(tempo_record)
                Next

                For i As Integer = 0 To country.Count - 1
                    For z As Integer = 0 To split_coma.Length - 1
                        If Trim(country(i).ToString) = Trim(split_coma(z)) Then
                            country.RemoveAt(i)
                            'Return country
                            'Exit Function
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    Private MYSQL_SELECT_COMMAND As MySqlCommand
    Private Function Returna_Conexion_Mysql(ByRef CconectionMysql As MySql.Data.MySqlClient.MySqlConnection) As String
        Dim poltrue As String = "False"
        If HttpContext.Current.Session.Item("ACTIVA_POOL_DBMS") = "1" Then
            poltrue = "True"
        Else
            poltrue = "False"
        End If
        Dim Contenido_Config As String = "Persist Security Info=" _
          & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
          & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
         & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
         & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString _
         & ";Pooling=" & poltrue & ";Min Pool Size=0;Max Pool Size=" & _
         HttpContext.Current.Session.Item("NUMERO_DBMS_CONEX")

        Try
            CconectionMysql = New MySql.Data.MySqlClient.MySqlConnection(Contenido_Config)
            If Not CconectionMysql Is Nothing Then
                CconectionMysql.Open()
            Else
                Returna_Conexion_Mysql = "Imposible conectar en la base de datos"
                Exit Function
            End If
            Returna_Conexion_Mysql = "YES"
        Catch ex As MySqlException
            Returna_Conexion_Mysql = ex.Message
        Finally
            'CconectionMysql = Nothing
        End Try
    End Function
    Public Function SELECTION_SELECT_FIELD(ByVal Sql_String As String, ByRef objet As Object) As String
        Dim Result As String = ""
        SELECTION_SELECT_FIELD = "SELECTION_SELECT_FIELD NO RECONOCE EL DBMS"
        If HttpContext.Current.Session("TYPE_DBMS_MODULO").ToString = "mysql" Then
            Result = MYSQL_SELECT_FIELD(Sql_String, objet)
            If Result <> "YES" Then
                SELECTION_SELECT_FIELD = "Inconsistencia en la funcion SELECTION_SELEC_FIELD LLAMANDO A MYSQL FIELD " & Result
                Exit Function
            Else
                SELECTION_SELECT_FIELD = "YES"
                Exit Function
            End If

        End If

    End Function
    Private Function MYSQL_SELECT_FIELD(ByVal Sql_String As String, ByRef Mysqldatacet As System.Data.DataSet) As String
        Dim Result As String = ""
        MYSQL_SELECT_FIELD = "YES"
        Mysqldatacet = New DataSet
        Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
        Result = Returna_Conexion_Mysql(conectmyslq)
        If Result <> "YES" Then
            MYSQL_SELECT_FIELD = "Imposible conectar con la base de datos " & Result
            Exit Function
        End If
        MYSQL_SELECT_COMMAND = New MySqlCommand(Sql_String)
        Dim DatMysqlAdpter As MySql.Data.MySqlClient.MySqlDataAdapter = _
            New MySql.Data.MySqlClient.MySqlDataAdapter(MYSQL_SELECT_COMMAND.CommandText, conectmyslq)
        Try
            DatMysqlAdpter.Fill(Mysqldatacet)
        Catch ex As MySqlException
            MYSQL_SELECT_FIELD = ex.Message
        Finally
            conectmyslq.Close()
        End Try


    End Function
End Class