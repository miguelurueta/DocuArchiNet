Imports System.ComponentModel
Imports System.Web.Services
Imports GestionDocumental_Docuarchi.net.Class_config_general_service
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService_radicacion_Simplificada
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaEstructuraRelacionTipoRestriccion(ByVal IdTipoTramite As Object) As Object
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estructura de una restricción relacionda a un tipo tramite
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
        Dim ListCDrestriccion = New List(Of CDrestriccion)
        Dim CDrestriccion As CDrestriccion = New CDrestriccion()
        Try
            Dim ClassraRestriRelacionTramite As New ClassraRestriRelacionTramite
            CDrestriccion.AppError = ClassraRestriRelacionTramite.SolicitaEstructuraRelacionTipoRestriccion(IdTipoTramite,
                                                                                                            CDrestriccion.CDeRelacionEstadoRetriccion)
            ListCDrestriccion.Add(CDrestriccion)
            Return ListCDrestriccion
        Catch ex As Exception
            CDrestriccion.AppError = ex.Message
            ListCDrestriccion.Add(CDrestriccion)
            Return ListCDrestriccion
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaAutoCompleteDestinatarioRestriccion(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la consulta con restriciones del destinatario del un tramite
        '          
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------
        'parameter       : Representa los parametros de la consulta
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-19
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Dim resultList = New List(Of class_config_gneral_service_row_tom)()
        Dim country As New List(Of class_config_gneral_service_row_option_tom_select)()
        Try
            Dim deserialize_parameter As New List(Of Class_config_general_service_auto_complete)
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Class_config_general_service_auto_complete))(parameter)
            Dim Result As String = ""
            Dim name_dbs_auto As String = deserialize_parameter(0).name_dbs_auto
            Dim name_table_auto As String = deserialize_parameter(0).name_plantilla_validacion
            Dim name_campo_auto As String = deserialize_parameter(0).campo_nombre_plantilla_val
            Dim name_campo_primary As String = deserialize_parameter(0).campo_primary_plantilla_val
            Dim value_auto As String = deserialize_parameter(0).value_auto
            Dim IdTipoRestriccion = deserialize_parameter(0).TomParameter.Find(Function(u) u.NombreCampo = "IdTipoRestriccion").ValorCampo
            Dim IdRestriccion = deserialize_parameter(0).TomParameter.Find(Function(u) u.NombreCampo = "IdRestriccion").ValorCampo
            Dim IdUsuarioGestionRadicado = Session.Item("GA_IDUSUARIOGESTION")
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Result = Class_remit_dest_interno.SolicitaAutoCompleteDestinatarioRestriccion(name_dbs_auto,
                                                                                          name_table_auto,
                                                                                          name_campo_auto,
                                                                                          name_campo_primary,
                                                                                          value_auto,
                                                                                          IdTipoRestriccion,
                                                                                          IdRestriccion,
                                                                                          IdUsuarioGestionRadicado,
                                                                                          country)
            If Result <> "YES" Then
                Dim item_ As New class_config_gneral_service_row_tom
                item_.error_gestion = Result
                resultList.Add(item_)
                Return country
            Else
                Dim item_ As New class_config_gneral_service_row_tom
                item_.error_gestion = Result
                item_.row_tom = country
                resultList.Add(item_)
                Return resultList
            End If
        Catch ex As Exception
            Dim item_ As New class_config_gneral_service_row_tom
            item_.error_gestion = ex.Message
            resultList.Add(item_)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_estado_registro_radicado_pendiente(ByVal id_registro_estado As Object, ByVal estado_radicado As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone el cambio de estado de un radicado en estado
        '          pendiente
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------
        'id_registro_estado  : Representa la identificación del registro de estado del radicado
        'estado              : Representa el nombre del campo de radicación destino 0-para gestor de 
        '                    : documentos  1- Radicado pendiente 2- Radicado pendiente
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-17
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Dim resultList = New List(Of class_estado_modulo_radicado)
        Dim class_estado_modulo_radicado As class_estado_modulo_radicado = New class_estado_modulo_radicado()
        Try
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            class_estado_modulo_radicado.error_gestion = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion(id_registro_estado,
                                                                                                                                            estado_radicado)
            resultList.Add(class_estado_modulo_radicado)
            Return resultList
        Catch ex As Exception
            class_estado_modulo_radicado.error_gestion = ex.Message
            resultList.Add(class_estado_modulo_radicado)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_almacenamiento_documentos_digitalizados_rad_simplificada(ByVal parameter As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el modulo de almaenamiento de documentos 
        '          digitalizados
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter   : Representa la estructura con los datos
        'para el almacenamiento
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-11-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_datos_image_lista)
        Dim parameter_gestion As class_stru_datos_image_lista = New class_stru_datos_image_lista()
        Try
            Dim Result As String = ""
            Dim class_rad_parameter_oper_document As New List(Of class_rad_parameter_oper_document)
            class_rad_parameter_oper_document = JsonConvert.DeserializeObject(Of List(Of class_rad_parameter_oper_document))(parameter)
            If class_rad_parameter_oper_document Is Nothing Then
                parameter_gestion.error_sistema = "Imposible deserializar los datos del formulario "
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            parameter_gestion.error_sistema = ClassAlmacenamiento.Almacenamiento_documentos_adjuntos_digitalizados_rad_simplificada(class_rad_parameter_oper_document(0).DG_TIPODIGITALIZACION,
                                                                                                                                    class_rad_parameter_oper_document(0).ID_TAREA_SELECCIONDA,
                                                                                                                                    class_rad_parameter_oper_document(0).DG_NOMBRE_GABINETE,
                                                                                                                                    class_rad_parameter_oper_document(0).DG_RADICADO,
                                                                                                                                    parameter_gestion)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_sistema = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_tipologia_rad_simplificada(ByVal parameter As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el modulo de actualización tipo documental
        '          desde gsbinete o modulo de radicación simplificada
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_ra_tipo_documental_serie      : Representa la estructura con los datos
        'de gestión del del tipo documental
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_service_ilist_drowlist)
        Dim parameter_gestion As Class_service_ilist_drowlist = New Class_service_ilist_drowlist()
        Try
            Dim Result As String = ""
            Dim class_rad_parameter_oper_document As New List(Of class_rad_parameter_oper_document)
            class_rad_parameter_oper_document = JsonConvert.DeserializeObject(Of List(Of class_rad_parameter_oper_document))(parameter)
            If class_rad_parameter_oper_document Is Nothing Then
                parameter_gestion.error_sistema = "Imposible deserializar los datos del formulario "
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim ClassWorkflowDigitalizacion As New ClassWorkflowDigitalizacion
            Dim Valor_campo As String = ""
            parameter_gestion.error_sistema = ClassWorkflowDigitalizacion.Actualiza_tipo_documento_lista_chequeo(class_rad_parameter_oper_document(0).ID_IMAGEN,
                                                                                                                 class_rad_parameter_oper_document(0).VALUE_ITEM,
                                                                                                                 class_rad_parameter_oper_document(0).DG_NOMBRE_GABINETE,
                                                                                                                 class_rad_parameter_oper_document(0).TEXT_ITEM,
                                                                                                                 class_rad_parameter_oper_document(0).DG_ID_CONFIG_DIGITALIZACION,
                                                                                                                 class_rad_parameter_oper_document(0).DG_RADICADO,
                                                                                                                 parameter_gestion.value_campo)
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_sistema = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_estructura_estado_radicado_radicacion_simple(ByVal parameter As Object, ByVal id_registro_estado As Object) As IEnumerable(Of class_estados_modulo_radicacion_config)
        '-------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estrucutra de estado de radicación  
        '          
        '-------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                       : Representa la estructura de los de los datos
        'id_plantilla_radicado           : Representa la idneitifcación de la plantilla
        'id_registro_estado              : Represnta la idneitifcación del registro de 
        '                                  de estado
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------

        '-------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------
        'Fecha                 : 2024-10-28
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------
        Dim resultList = New List(Of class_estados_modulo_radicacion_config)()
        Dim parameter_gestion As class_estados_modulo_radicacion_config = New class_estados_modulo_radicacion_config()
        Try
            Dim Result As String = ""
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim deserialize_parameter As New List(Of class_system_plantilla_defaul_simplificada)
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of class_system_plantilla_defaul_simplificada))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_gestion = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Result = Class_ra_rad_estados_modulo_radicacion.Solicita_estructura_estado_radicado_radicacion_simple(id_registro_estado,
                                                                                                                  HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                                  deserialize_parameter.Item(0).id_Plantilla,
                                                                                                                  deserialize_parameter.Item(0).id_tipo_plantilla,
                                                                                                                  parameter_gestion)

            parameter_gestion.error_gestion = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_estructura_estado_radicado_radicacion_simple_vacia(ByVal parameter As Object) As IEnumerable(Of class_estados_modulo_radicacion_config)
        '-------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estrucutra de estado de radicación  vacia
        '          
        '-------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                       : Representa la estructura de los de los datos
        '
        '
        '                                  
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------

        '-------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------
        'Fecha                 : 2024-11-07
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------
        Dim resultList = New List(Of class_estados_modulo_radicacion_config)()
        Dim parameter_gestion As class_estados_modulo_radicacion_config = New class_estados_modulo_radicacion_config()
        Try
            Dim Result As String = ""
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Result = Class_ra_rad_estados_modulo_radicacion.Solicita_estructura_estado_radicado_radicacion_simple_vacia(parameter_gestion)

            parameter_gestion.error_gestion = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_radicado_existencia_radicado_asignado(ByVal id_plantilla As Object, ByVal tipo_plantilla As Object) As IEnumerable(Of class_estado_modulo_radicado)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el registro de radicación  
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
        'Fecha                 : 2024-10-28
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_estado_modulo_radicado)()
        Dim parameter_gestion As class_estado_modulo_radicado = New class_estado_modulo_radicado()
        Try
            Dim Result As String = ""
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Result = Class_ra_rad_estados_modulo_radicacion.Solicita_radicado_existencia_radicado_asignado(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                           id_plantilla,
                                                                                                           tipo_plantilla,
                                                                                                           parameter_gestion)
            parameter_gestion.error_gestion = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Inicializa_cliente_workflow_radicacion_simple(ByVal parameter As Object) As Object
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que espone la inicialización del cliente workflow para radicación 
        '          simplificada
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'parameter           : Representa el paramtro general  opcional 
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Dim resultList = New List(Of class_estado_modulo_radicado)()
        Dim parameter_gestion As class_estado_modulo_radicado = New class_estado_modulo_radicado()
        Try
            Dim Result As String = ""
            Dim Class_ra_radicacion_simplificada As New Class_ra_radicacion_simplificada
            Result = Class_ra_radicacion_simplificada.Inicializa_cliente_workflow_radicacion_simple()
            parameter_gestion.error_gestion = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registro_radicacion_simplificada(ByVal parameter As Object, ByVal name_plantilla As Object) As IEnumerable(Of class_rad_return_registro_radicado)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el registro de radicación  
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
        'Fecha                 : 2024-10-28
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_rad_return_registro_radicado)()
        Dim parameter_gestion As class_rad_return_registro_radicado = New class_rad_return_registro_radicado()
        Try
            Dim Result As String = ""
            Dim Class_ra_radicacion_simplificada As New Class_ra_radicacion_simplificada
            Dim deserialize_parameter As New List(Of Class_config_general_service)
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_gestion = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
            End If

            Dim codigo_radicado As String = ""
            Dim asignar_radicado As String = ""
            Dim id_registro_estado As Object = 0
            Result = Class_ra_radicacion_simplificada.Registro_radicacion_simplificada(name_plantilla,
                                                                                       HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                       HttpContext.Current.Session.Item("Id_actividad_Workflow"),
                                                                                       HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                       HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                       deserialize_parameter,
                                                                                       codigo_radicado,
                                                                                       asignar_radicado,
                                                                                       id_registro_estado)
            parameter_gestion.error_gestion = Result
            parameter_gestion.asignar_radicado = asignar_radicado
            parameter_gestion.codigo_radicado = codigo_radicado
            parameter_gestion.id_registro_estado = id_registro_estado
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_estructura_radicacion_simplificada(ByVal parameter As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Expone el servicio que solicita datos de contrución
        '          del formulario para el registro de radicación simplificada
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
            Dim Class_ra_radicacion_simplificada As New Class_ra_radicacion_simplificada
            Result = Class_ra_radicacion_simplificada.Solicita_estructura_radicacion_simplificada(resultList)
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
    Public Function Service_solicita_nombre_plantilla_radicacion_simplificada(ByVal parameter As Object) As Object
        '---------------------------------------------------------------------------
        'Funcion : Expone el servicio que solicita datos de la plantilla de radicacion
        '           simplificada
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter : 
        '                          
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-28
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_system_plantilla_defaul_simplificada)()
        Dim parameter_gestion As class_system_plantilla_defaul_simplificada = New class_system_plantilla_defaul_simplificada()
        Try
            Dim Result As String = ""
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim Class_system_plantilla_defaul_simplificada As New class_system_plantilla_defaul_simplificada
            Result = Class_system_plantilla_radicado.Solicita_estructura_plantilla_radicacion_default_simplificada(Class_system_plantilla_defaul_simplificada)
            parameter_gestion.error_gestion = Result
            parameter_gestion.id_Plantilla = Class_system_plantilla_defaul_simplificada.id_Plantilla
            parameter_gestion.Nombre_Plantilla_Radicado = Class_system_plantilla_defaul_simplificada.Nombre_Plantilla_Radicado
            parameter_gestion.Tipo_Plantilla = Class_system_plantilla_defaul_simplificada.Tipo_Plantilla
            parameter_gestion.id_tipo_plantilla = Class_system_plantilla_defaul_simplificada.id_tipo_plantilla
            parameter_gestion.util_estado_pendiente_rad = Class_system_plantilla_defaul_simplificada.util_estado_pendiente_rad
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_datos_auto_complete_externo(ByVal parameter As Object)
        Dim resultList = New List(Of class_config_gneral_service_row_tom)()
        Dim country As New List(Of class_config_gneral_service_row_option_tom_select)()
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Class_config_general_service_auto_complete))(parameter)
            Dim Result As String = ""
            Dim name_dbs_auto As String = deserialize_parameter(0).name_dbs_auto
            Dim name_table_auto As String = deserialize_parameter(0).name_plantilla_validacion
            Dim name_campo_auto As String = deserialize_parameter(0).campo_nombre_plantilla_val
            Dim name_campo_primary As String = deserialize_parameter(0).campo_primary_plantilla_val
            Dim value_auto As String = deserialize_parameter(0).value_auto
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Solicita_datos_auto_complete_tercero_plantilla(name_dbs_auto,
                                                                                               name_table_auto,
                                                                                               name_campo_auto,
                                                                                               name_campo_primary,
                                                                                               value_auto,
                                                                                               country)
            If Result <> "YES" Then
                Dim item_ As New class_config_gneral_service_row_tom
                item_.error_gestion = Result
                resultList.Add(item_)
                Return country
            Else
                Dim item_ As New class_config_gneral_service_row_tom
                item_.error_gestion = Result
                item_.row_tom = country
                resultList.Add(item_)
                Return resultList
            End If
        Catch ex As Exception
            Dim item_ As New class_config_gneral_service_row_tom
            item_.error_gestion = ex.Message
            resultList.Add(item_)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_datos_auto_complete_interno(ByVal parameter As Object)
        Dim resultList = New List(Of class_config_gneral_service_row_tom)()
        Dim country As New List(Of class_config_gneral_service_row_option_tom_select)()
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of List(Of Class_config_general_service_auto_complete))(parameter)
            Dim Result As String = ""
            Dim name_dbs_auto As String = deserialize_parameter(0).name_dbs_auto
            Dim name_table_auto As String = deserialize_parameter(0).name_plantilla_validacion
            Dim name_campo_auto As String = deserialize_parameter(0).campo_nombre_plantilla_val
            Dim name_campo_primary As String = deserialize_parameter(0).campo_primary_plantilla_val
            Dim value_auto As String = deserialize_parameter(0).value_auto
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Result = Class_remit_dest_interno.Solicita_datos_auto_complete_remitente_interno(name_dbs_auto,
                                                                                             name_table_auto,
                                                                                             name_campo_auto,
                                                                                             name_campo_primary,
                                                                                             value_auto,
                                                                                             country)
            If Result <> "YES" Then
                Dim item_ As New class_config_gneral_service_row_tom
                item_.error_gestion = Result
                resultList.Add(item_)
                Return country
            Else
                Dim item_ As New class_config_gneral_service_row_tom
                item_.error_gestion = Result
                item_.row_tom = country
                resultList.Add(item_)
                Return resultList
            End If
        Catch ex As Exception
            Dim item_ As New class_config_gneral_service_row_tom
            item_.error_gestion = ex.Message
            resultList.Add(item_)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_radicados_pendientes_radicacion(ByVal id_plantilla_radicado As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la lista de tareas de radicación en pendiente
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_plantilla_radicado : Representa la identificacion de la plnatilla de radicación
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
        'Fecha                 : 2024-11-16
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_stru_Row_Gabinete_Generic)
        Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
        Try
            Dim Result As String = ""
            Dim Class_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            iList_class_stru_Row_Gabinete_Generic.Error_result = Class_estados_modulo_radicacion.Solicita_radicados_pendientes_radicacion(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                                                          id_plantilla_radicado,
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
    Public Function Service_solicita_opciones_plantilla_radicacion(ByVal id_plantilla_radicado As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la lista de opciones plantilla de radicación
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_plantilla_radicado : Representa la identificacion de la plnatilla de radicación
        '                        
        '
        '                       
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'Class_system_plantilla_radicado_opciones : Retorna la estructura con las opciones
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-11-16
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of Class_system_plantilla_radicado_opciones)
        Dim Class_system_plantilla_radicado_opciones As Class_system_plantilla_radicado_opciones = New Class_system_plantilla_radicado_opciones
        Try
            Dim Result As String = ""
            '----/////Solicita estado pendiente plantilla---/////
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            'Dim litss As New Class_system_plantilla_radicado_opciones
            Class_system_plantilla_radicado_opciones.Error_result = Class_system_plantilla_radicado.Solicita_Opcion_Plantilla_Radicacion(id_plantilla_radicado,
                                                                                                                                         Class_system_plantilla_radicado_opciones)

            resultList.Add(Class_system_plantilla_radicado_opciones)
            Return resultList
        Catch ex As Exception
            Class_system_plantilla_radicado_opciones.Error_result = ex.Message
            resultList.Add(Class_system_plantilla_radicado_opciones)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_numero_radicados_pendientes(ByVal id_plantilla_radicado As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el numero de radicados pendientes
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_plantilla_radicado : Representa la identificacion de la plnatilla de radicación
        '                        
        '
        '                       
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_estado_modulo_radicado : Retorna la estructura con el numero de tareas pendientes
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-11-16
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_estado_modulo_radicado)
        Dim class_estado_modulo_radicado As class_estado_modulo_radicado = New class_estado_modulo_radicado
        Try
            Dim Result As String = ""
            Dim Class_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            '----/////Solicita estado pendiente plantilla---/////
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim nunmero_radicado_pendiente As Integer = 0
            class_estado_modulo_radicado.error_gestion = Class_estados_modulo_radicacion.Solicita_numero_radicados_pendientes(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                                              id_plantilla_radicado,
                                                                                                                              class_estado_modulo_radicado.total_pendiente)

            resultList.Add(class_estado_modulo_radicado)
            Return resultList
        Catch ex As Exception
            class_estado_modulo_radicado.error_gestion = ex.Message
            resultList.Add(class_estado_modulo_radicado)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_estado_radicado_asignado_usuario_gestion_documentos(ByVal id_plantilla_radicado As Object)
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el estado de radicado asignado a usuario para
        '          gestión de documentos
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_plantilla_radicado : Representa la identificacion de la plnatilla de radicación
        '                        
        '
        '                       
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_estado_modulo_radicado : Retorna la estructura con el numero de tareas pendientes
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-11-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Dim resultList = New List(Of class_estado_modulo_radicado)
        Dim class_estado_modulo_radicado As class_estado_modulo_radicado = New class_estado_modulo_radicado
        Try
            Dim Result As String = ""
            Dim Class_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            '----/////Solicita estado radicado asignado---/////
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim nunmero_radicado_pendiente As Integer = 0
            class_estado_modulo_radicado.error_gestion = Class_estados_modulo_radicacion.Solicita_estado_radicado_asignado_usuario_gestion_documentos(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                                                                      id_plantilla_radicado,
                                                                                                                                                      class_estado_modulo_radicado.estado_asignado)

            resultList.Add(class_estado_modulo_radicado)
            Return resultList
        Catch ex As Exception
            class_estado_modulo_radicado.error_gestion = ex.Message
            resultList.Add(class_estado_modulo_radicado)
            Return resultList
        End Try
    End Function
End Class