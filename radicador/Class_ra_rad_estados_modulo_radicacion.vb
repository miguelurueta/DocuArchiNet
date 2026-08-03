Imports System.IO
Imports Newtonsoft.Json

Public Structure stru_registro_estado
    Dim system_plantilla_radicado_id_Plantilla As Integer
    Dim id_radicado As Integer
    Dim consecutivo_radicado As String
    Dim fecha_registro As String
    Dim estado As Integer
    Dim remitente As String
    Dim id_usuario_radicado As Integer
    Dim id_tarea_workflow As Long
    Dim tipo_doc_entrante_id_Tipo_Doc_Entrante As Integer
End Structure
Public Structure stru_permisos_interface_envio
    Dim enviar_usuario As Integer
    Dim enviar_actividad As Integer
    Dim auto_terminar As Integer
    Dim terminar_radicado As Integer
    Dim nombre_flujo_ruta As String
    Dim estado_cerrado As Integer
    Dim estado_modulo As Integer
End Structure
Public Class class_estado_modulo_radicado
    Property estado_asignado As String
    Property radicado As String
    Property id_tarea_workflow As Long
    Property id_registro_estado As Long
    Property total_pendiente As Integer
    Property error_gestion As String
End Class
Public Class class_estados_modulo_radicacion_config
    Property numero_pndiente As Integer
    Property enviar_usuario As Integer
    Property enviar_actividad As Integer
    Property auto_terminar As Integer
    Property terminar_radicado As Integer
    Property nombre_flujo_ruta As String
    Property id_flujo_trabajo As Integer
    Property estado_cerrado As String
    Property estado_modulo As Integer
    Property estado_resp_obligatoria As Integer
    Property nombre_campo_radicado_gabinete As String
    Property inventario_documental As Integer
    Property aplica_trd As Integer
    Property asigna_unidad As Integer
    Property Ruta_Web_Escaner As String
    Property url_escaner As String
    Property util_estado_pendiete_rad As Integer
    Property util_opcion_auto_vincula As Integer
    Property DG_TIPODIGITALIZACION As String
    Property DG_ID_TRAMITE As Integer
    Property DG_TIPO_TRAMITE As String
    Property DG_ID_GABINETE As Integer
    Property DG_NOMBRE_GABINETE As String
    Property DG_RADICADO As String
    Property DG_LISTA_CHEQUEO As Integer
    Property DG_ID_CONFIG_DIGITALIZACION As Integer
    Property DG_SELECION_TREE As String
    Property DG_NOMBRE_TRAMITE As String
    Property RA_TIPO_MODULO_GESTION_ENVIO_RADICADO As Integer
    Property RA_ID_REGISTRO_RADICADO As Integer
    Property RA_RADICADO_REGISTRO As String
    Property ID_TAREA_SELECCIONDA As Long
    Property SELECCIONTEMPORAL As String
    Property ROW_GABINETE_GENERIC As List(Of class_stru_Row_Gabinete_Generic)
    Property stru_permisos_interface_envio As stru_permisos_interface_envio
    Property stru_registro_estado As stru_registro_estado
    Property error_gestion As String
End Class
Public Class Class_ra_rad_estados_modulo_radicacion
    Function Actualiza_estado_registro_radicado_pendiente(ByVal id_registro_estado As Long,
                                                          ByVal estado As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza la tabla estado del radicado pendiente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
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
        'Fecha                 : 2023-11-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Update As String = "update ra_rad_estados_modulo_radicacion set estado= " & estado &
               "  where id_estado_radicado =" & id_registro_estado
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref2.SELECTION_INSERT_COMMAND(Update)
            If Result <> "YES" Then
                Actualiza_estado_registro_radicado_pendiente = "Funcion  Actualiza_estado_registro_radicado_pendiente dice " & Result
                Exit Function
            Else
                Actualiza_estado_registro_radicado_pendiente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_estado_registro_radicado_pendiente = "Inconsistencia general funcion Actualiza_estado_registro_radicado_pendiente " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_estado_radicado_radicacion_simple_vacia(ByRef class_estados_modulo_radicacion_config As class_estados_modulo_radicacion_config) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura vacia del estado de una radicado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------

        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_estados_modulo_radicacion_config  : Retorna la estructura del estado de radicado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-06
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim stru_permisos_interface_envio As stru_permisos_interface_envio = Nothing
            Dim stru_registro_estado As stru_registro_estado = Nothing
            '//-------Agrega la estructura de documentos relacionados vacia
            class_estados_modulo_radicacion_config.ROW_GABINETE_GENERIC = New List(Of class_stru_Row_Gabinete_Generic)
            Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
            HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") = ""
            class_estados_modulo_radicacion_config.Ruta_Web_Escaner = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString.Replace("\", "/")
            class_estados_modulo_radicacion_config.RA_RADICADO_REGISTRO = stru_registro_estado.consecutivo_radicado
            class_estados_modulo_radicacion_config.RA_ID_REGISTRO_RADICADO = 0
            class_estados_modulo_radicacion_config.util_opcion_auto_vincula = 0
            class_estados_modulo_radicacion_config.DG_TIPODIGITALIZACION = "TRAMITE SIMPLE"
            If stru_registro_estado.id_tarea_workflow = 0 Then
                class_estados_modulo_radicacion_config.SELECCIONTEMPORAL = 0 & "|" & "0"
            Else
                class_estados_modulo_radicacion_config.SELECCIONTEMPORAL = stru_registro_estado.id_tarea_workflow & "|" & "0"
            End If
            class_estados_modulo_radicacion_config.url_escaner = "../workflow/WebFormEscan.aspx"
            '"../workflow/WebFormEscan.aspx"
            class_estados_modulo_radicacion_config.DG_SELECION_TREE = ""
            class_estados_modulo_radicacion_config.ID_TAREA_SELECCIONDA = stru_registro_estado.id_tarea_workflow
            stru_permisos_interface_envio.estado_modulo = 2
            class_estados_modulo_radicacion_config.stru_permisos_interface_envio = stru_permisos_interface_envio
            class_estados_modulo_radicacion_config.stru_registro_estado = stru_registro_estado
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = class_estados_modulo_radicacion_config.DG_TIPODIGITALIZACION
            HttpContext.Current.Session.Item("DG_ID_TRAMITE") = class_estados_modulo_radicacion_config.DG_ID_TRAMITE
            HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = class_estados_modulo_radicacion_config.DG_TIPO_TRAMITE
            HttpContext.Current.Session.Item("DG_ID_GABINETE") = class_estados_modulo_radicacion_config.DG_ID_GABINETE
            HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE") = class_estados_modulo_radicacion_config.DG_NOMBRE_GABINETE
            HttpContext.Current.Session.Item("DG_RADICADO") = class_estados_modulo_radicacion_config.DG_RADICADO
            HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = class_estados_modulo_radicacion_config.DG_LISTA_CHEQUEO
            HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = class_estados_modulo_radicacion_config.DG_ID_CONFIG_DIGITALIZACION
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = class_estados_modulo_radicacion_config.DG_SELECION_TREE
            HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE") = class_estados_modulo_radicacion_config.DG_NOMBRE_TRAMITE
            HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = class_estados_modulo_radicacion_config.RA_TIPO_MODULO_GESTION_ENVIO_RADICADO
            HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO") = class_estados_modulo_radicacion_config.RA_ID_REGISTRO_RADICADO
            HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = class_estados_modulo_radicacion_config.RA_RADICADO_REGISTRO
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = class_estados_modulo_radicacion_config.ID_TAREA_SELECCIONDA
            HttpContext.Current.Session.Item("SELECCIONTEMPORAL") = class_estados_modulo_radicacion_config.SELECCIONTEMPORAL
            Solicita_estructura_estado_radicado_radicacion_simple_vacia = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_estructura_estado_radicado_radicacion_simple_vacia = "Inconsistencia general funcion Solicita_estructura_estado_radicado_radicacion_simple_vacia " & ex.Message
        End Try
    End Function

    Function Solicita_estructura_estado_radicado_radicacion_simple(ByVal id_registro_estado As Integer,
                                                                   ByVal id_usuario_radicacion As Integer,
                                                                   ByVal id_plantilla_radicacion As Integer,
                                                                   ByVal id_tipo_plantilla_radicacion As Integer,
                                                                   ByRef class_estados_modulo_radicacion_config As class_estados_modulo_radicacion_config) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura del estado de una radicado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_registro_estado           : Representa la identificación del registro de estado del radicado
        'id_usuario_radicacion        : Representa la identificación del usuario de radicación
        'id_plantilla_radicacion      : Representa la identificación de la plantilla de radicación
        'id_tipo_plantilla_radicacion : Representa el tipo de identificación
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_estados_modulo_radicacion_config  : Retorna la estructura del estado de radicado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-06
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            '------------------------------------------------------------------------------------
            'Solicita el numero de radicados asignados pendientes
            '------------------------------------------------------------------------------------
            Dim Result As String = ""
            Dim Class_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim stru_permisos_interface_envio As stru_permisos_interface_envio = Nothing
            Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            Dim class_workflow_ruta As New Class_worflow_rutas
            Dim Numero_pendietes As Integer = 0
            Result = Class_estados_modulo_radicacion.Solicita_numero_radicados_pendientes(id_usuario_radicacion,
                                                                                          id_plantilla_radicacion,
                                                                                          id_tipo_plantilla_radicacion,
                                                                                          class_estados_modulo_radicacion_config.numero_pndiente)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If

            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim stru_registro_estado As stru_registro_estado = Nothing
            Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(id_registro_estado,
                                                                                                      stru_registro_estado)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim Nombre_plantilla As String = ""
            Result = Class_system_plantilla_radicado.Solicita_nombre_plantilla_radicado(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                        Nombre_plantilla)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            '----/////Solicita estado pendiente plantilla---/////
            Dim litss As New Class_system_plantilla_radicado_opciones
            Result = Class_system_plantilla_radicado.Solicita_Opcion_Plantilla_Radicacion(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                          litss)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            class_estados_modulo_radicacion_config.util_estado_pendiete_rad = litss.util_estado_pendiente_rad
            Result = Class_system_plantilla_radicado.Retorna_Tipo_Plantilla(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                            class_estados_modulo_radicacion_config.DG_TIPO_TRAMITE)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            Dim Class_plantillas_radicacion As New Class_plantillas_radicacion
            Dim Tipo_tramite As String = ""
            Result = Class_plantillas_radicacion.retorna_tipo_documental_radicado(stru_registro_estado.consecutivo_radicado,
                                                                                  Nombre_plantilla,
                                                                                  class_estados_modulo_radicacion_config.DG_NOMBRE_TRAMITE)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Class_tipo_doc_entrante.retorna_id_tipo_tramite_radicado(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                              class_estados_modulo_radicacion_config.DG_NOMBRE_TRAMITE,
                                                                              class_estados_modulo_radicacion_config.DG_ID_TRAMITE)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            Class_tipo_doc_entrante.Solicita_opcion_auto_vinculacion(class_estados_modulo_radicacion_config.DG_ID_TRAMITE,
                                                                     class_estados_modulo_radicacion_config.util_opcion_auto_vincula)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            Result = Class_tipo_doc_entrante.Retorna_id_nombre_gabinete_tipo_tramite(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                     class_estados_modulo_radicacion_config.DG_NOMBRE_TRAMITE,
                                                                                     class_estados_modulo_radicacion_config.DG_ID_GABINETE,
                                                                                     class_estados_modulo_radicacion_config.DG_NOMBRE_GABINETE)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            Dim id_config_digitalizacion As Integer = 0
            Dim class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Result = class_ra_dig_config_digitalizacion.Solicita_id_configuracion_digitalizacion(class_estados_modulo_radicacion_config.DG_ID_TRAMITE,
                                                                                                 class_estados_modulo_radicacion_config.DG_TIPO_TRAMITE,
                                                                                                 class_estados_modulo_radicacion_config.DG_ID_CONFIG_DIGITALIZACION,
                                                                                                 0)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            Dim Ref_class_detalle_gabinete As New Class_DETALLE_GABIENETE
            Dim nombre_campo_radicado_gabinete As String = ""
            Result = Ref_class_detalle_gabinete.SolicitaNombreCampoRadicadoGabinete(class_estados_modulo_radicacion_config.DG_NOMBRE_GABINETE,
                                                                                    class_estados_modulo_radicacion_config.nombre_campo_radicado_gabinete)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            Dim Ref_class_system1 As New Class_system1
            Result = Ref_class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(class_estados_modulo_radicacion_config.DG_NOMBRE_GABINETE,
                                                                                                         class_estados_modulo_radicacion_config.inventario_documental,
                                                                                                         class_estados_modulo_radicacion_config.aplica_trd,
                                                                                                         class_estados_modulo_radicacion_config.asigna_unidad)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            Result = Class_tipo_doc_entrante.Solicita_tipo_modulo_soporte_documental_envio(class_estados_modulo_radicacion_config.DG_ID_TRAMITE,
                                                                                           class_estados_modulo_radicacion_config.RA_TIPO_MODULO_GESTION_ENVIO_RADICADO)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If

            Result = Class_plantillas_radicacion.Retorna_id_flujo_trabajo_radicado(Nombre_plantilla,
                                                                                   stru_registro_estado.consecutivo_radicado,
                                                                                   class_estados_modulo_radicacion_config.id_flujo_trabajo)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            '//-------Agrega la estructura de documentos relacionados
            class_estados_modulo_radicacion_config.ROW_GABINETE_GENERIC = New List(Of class_stru_Row_Gabinete_Generic)
            Dim iList_class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic = New class_stru_Row_Gabinete_Generic
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.SolicitaDocumentosRelacionadosRadicadoEnlace(class_estados_modulo_radicacion_config.nombre_campo_radicado_gabinete,
                                                                                        class_estados_modulo_radicacion_config.DG_NOMBRE_GABINETE,
                                                                                        stru_registro_estado.consecutivo_radicado,
                                                                                        class_estados_modulo_radicacion_config.aplica_trd,
                                                                                        iList_class_stru_Row_Gabinete_Generic)
            If Result <> "YES" Then
                Solicita_estructura_estado_radicado_radicacion_simple = Result
                Exit Function
            End If
            class_estados_modulo_radicacion_config.ROW_GABINETE_GENERIC.Add(iList_class_stru_Row_Gabinete_Generic)
            '//-------Configura los parametros de interface para el tipo de asignación 2 
            If class_estados_modulo_radicacion_config.RA_TIPO_MODULO_GESTION_ENVIO_RADICADO = 2 Then
                stru_permisos_interface_envio.terminar_radicado = 0
                class_estados_modulo_radicacion_config.estado_resp_obligatoria = 0
                Result = Class_tipo_doc_entrante.Retorna_tipo_respuesta_tramite_radicado(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                         class_estados_modulo_radicacion_config.DG_NOMBRE_TRAMITE,
                                                                                         class_estados_modulo_radicacion_config.estado_resp_obligatoria)
                If Result <> "YES" Then
                    Solicita_estructura_estado_radicado_radicacion_simple = Result
                    Exit Function
                End If

                Result = Class_tipo_doc_entrante.Retorna_tipo_respuesta_tramite_radicado(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                         class_estados_modulo_radicacion_config.DG_NOMBRE_TRAMITE,
                                                                                         class_estados_modulo_radicacion_config.estado_resp_obligatoria)
                If Result <> "YES" Then
                    Solicita_estructura_estado_radicado_radicacion_simple = Result
                    Exit Function
                End If
                '----------------------------------------------
                'Configura interface para flujo de trabajo
                '---------------------------------------------
                If class_estados_modulo_radicacion_config.id_flujo_trabajo <> 0 Then
                    Result = Class_flujo_trabajo_workflow.SolicitaEstadoAbiertoCerradoFlujoDocumental(class_estados_modulo_radicacion_config.id_flujo_trabajo,
                                                                                                           stru_permisos_interface_envio.estado_cerrado)
                    If Result <> "YES" Then
                        Solicita_estructura_estado_radicado_radicacion_simple = Result
                        Exit Function
                    End If
                    Result = Class_flujo_trabajo_workflow.SolicitaNombreFlujoTrabajoPorIdFlujo(class_estados_modulo_radicacion_config.id_flujo_trabajo,
                                                                                                    stru_permisos_interface_envio.nombre_flujo_ruta)
                    If Result <> "YES" Then
                        Solicita_estructura_estado_radicado_radicacion_simple = Result
                        Exit Function
                    End If
                    'stru_permisos_interface_envio.estado_cerrado = 0
                    If stru_permisos_interface_envio.estado_cerrado = 1 Then
                        class_estados_modulo_radicacion_config.estado_cerrado = "Abierto"
                    Else
                        class_estados_modulo_radicacion_config.estado_cerrado = "Cerrado"
                    End If
                    If class_estados_modulo_radicacion_config.estado_resp_obligatoria = 1 Then
                        stru_permisos_interface_envio.terminar_radicado = 0
                        stru_permisos_interface_envio.enviar_usuario = 0
                        stru_permisos_interface_envio.enviar_actividad = 0
                        stru_permisos_interface_envio.auto_terminar = 1
                        stru_permisos_interface_envio.nombre_flujo_ruta = stru_permisos_interface_envio.nombre_flujo_ruta

                        'If opcion_evalua_actualiza_panel = 1 Then
                        '    Panel_EnviarUsuario.Visible = False
                        '    Panel_EnviaActividad.Visible = False
                        '    Panel_autoterminar.Visible = True
                        '    panel_terminar_rad.Visible = False
                        '    Panel_enviar_flujo.Visible = False
                        '    If stru_permisos_interface_envio.estado_cerrado = 1 Then
                        '        estado_cerrado = "Abierto"
                        '    Else
                        '        estado_cerrado = "Cerrado"
                        '    End If
                        '    'Label_estado_selecion.Text = "Flujo : " & stru_permisos_interface_envio.nombre_flujo_ruta & " Tipo flujo : " & estado_cerrado
                        '    'updatemenu.Update()
                        'End If
                    Else
                        If stru_permisos_interface_envio.estado_cerrado = 1 Then
                            stru_permisos_interface_envio.enviar_usuario = 0
                            stru_permisos_interface_envio.enviar_actividad = 0
                            stru_permisos_interface_envio.auto_terminar = 0
                            stru_permisos_interface_envio.estado_cerrado = 0
                            'If opcion_evalua_actualiza_panel = 1 Then
                            '    Panel_EnviarUsuario.Visible = False
                            '    Panel_EnviaActividad.Visible = False
                            '    Panel_autoterminar.Visible = False
                            '    panel_terminar_rad.Visible = False
                            '    Panel_enviar_flujo.Visible = True
                            '    Label_estado_selecion.Text = "Flujo : " & stru_permisos_interface_envio.nombre_flujo_ruta & " Tipo flujo : " & "Cerrado"
                            '    updatemenu.Update()
                            'End If
                        Else
                            stru_permisos_interface_envio.enviar_usuario = 1
                            stru_permisos_interface_envio.enviar_actividad = 1
                            stru_permisos_interface_envio.auto_terminar = 0
                            stru_permisos_interface_envio.estado_cerrado = 1
                            'If opcion_evalua_actualiza_panel = 1 Then
                            '    Panel_EnviarUsuario.Visible = False
                            '    Panel_EnviaActividad.Visible = False
                            '    Panel_autoterminar.Visible = False
                            '    panel_terminar_rad.Visible = False
                            '    Panel_enviar_flujo.Visible = True
                            '    Label_estado_selecion.Text = "Flujo : " & stru_permisos_interface_envio.nombre_flujo_ruta & " Tipo flujo : " & "Abierto"
                            '    updatemenu.Update()
                            'End If
                        End If
                    End If
                Else
                    '----------------------------------------------
                    'Configura interface para ruta de trabajo
                    '---------------------------------------------
                    Dim estado_ruta As Integer = 0
                    If stru_registro_estado.id_tarea_workflow <> 0 Then
                        Result = class_workflow_ruta.Solicita_etado_abierto_cerrado_ruta_tarea(stru_registro_estado.id_tarea_workflow,
                                                                                               HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                               estado_ruta,
                                                                                               "")
                        If Result <> "YES" Then
                            Solicita_estructura_estado_radicado_radicacion_simple = Result
                            Exit Function
                        End If
                    End If
                    '----/////Configura interface para respuesta obligatoria---////
                    If class_estados_modulo_radicacion_config.estado_resp_obligatoria = 1 Then
                        stru_permisos_interface_envio.terminar_radicado = 0
                        stru_permisos_interface_envio.enviar_usuario = 0
                        stru_permisos_interface_envio.enviar_actividad = 0
                        stru_permisos_interface_envio.auto_terminar = 1
                        stru_permisos_interface_envio.nombre_flujo_ruta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
                        If estado_ruta = 1 Then
                            stru_permisos_interface_envio.estado_cerrado = 0
                        Else
                            stru_permisos_interface_envio.estado_cerrado = 1
                        End If
                        class_estados_modulo_radicacion_config.estado_cerrado = ""
                        If stru_permisos_interface_envio.estado_cerrado = 1 Then
                            class_estados_modulo_radicacion_config.estado_cerrado = "Abierto"
                        Else
                            class_estados_modulo_radicacion_config.estado_cerrado = "Cerrado"
                        End If
                    Else
                        If estado_ruta = 1 Then
                            stru_permisos_interface_envio.terminar_radicado = 0
                            stru_permisos_interface_envio.enviar_usuario = 0
                            stru_permisos_interface_envio.enviar_actividad = 1
                            stru_permisos_interface_envio.auto_terminar = 0
                            stru_permisos_interface_envio.nombre_flujo_ruta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
                            stru_permisos_interface_envio.estado_cerrado = 0
                            class_estados_modulo_radicacion_config.estado_cerrado = ""
                            If stru_permisos_interface_envio.estado_cerrado = 1 Then
                                class_estados_modulo_radicacion_config.estado_cerrado = "Abierto"
                            Else
                                class_estados_modulo_radicacion_config.estado_cerrado = "Cerrado"
                            End If
                        Else
                            stru_permisos_interface_envio.terminar_radicado = 0
                            stru_permisos_interface_envio.enviar_usuario = 1
                            stru_permisos_interface_envio.enviar_actividad = 1
                            stru_permisos_interface_envio.auto_terminar = 0
                            stru_permisos_interface_envio.nombre_flujo_ruta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
                            stru_permisos_interface_envio.estado_cerrado = 1
                            class_estados_modulo_radicacion_config.estado_cerrado = "Cerrado"

                        End If
                    End If
                End If
            End If
            If class_estados_modulo_radicacion_config.RA_TIPO_MODULO_GESTION_ENVIO_RADICADO = 3 Then
                stru_permisos_interface_envio.terminar_radicado = 1
                stru_permisos_interface_envio.enviar_usuario = 0
                stru_permisos_interface_envio.enviar_actividad = 0
                stru_permisos_interface_envio.auto_terminar = 0
                stru_permisos_interface_envio.nombre_flujo_ruta = ""
                stru_permisos_interface_envio.estado_cerrado = 0
            End If
            Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString)
            If Directory.Exists(Ruttempo) = False Then
                Directory.CreateDirectory(Ruttempo)
            End If
            '--------------------------------
            'Crea ruta escaneo
            '--------------------------------
            Dim ruta_escaner As String = Ruttempo & "\ESCANERWEB"
            If Directory.Exists(ruta_escaner) = False Then
                Directory.CreateDirectory(ruta_escaner)
            End If
            HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") = ruta_escaner
            class_estados_modulo_radicacion_config.Ruta_Web_Escaner = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString.Replace("\", "/")
            class_estados_modulo_radicacion_config.RA_RADICADO_REGISTRO = stru_registro_estado.consecutivo_radicado
            class_estados_modulo_radicacion_config.RA_ID_REGISTRO_RADICADO = id_registro_estado
            class_estados_modulo_radicacion_config.DG_TIPODIGITALIZACION = "TRAMITE SIMPLE"
            If stru_registro_estado.id_tarea_workflow = 0 Then
                class_estados_modulo_radicacion_config.SELECCIONTEMPORAL = id_registro_estado & "|" & "0"
            Else
                class_estados_modulo_radicacion_config.SELECCIONTEMPORAL = stru_registro_estado.id_tarea_workflow & "|" & "0"
            End If
            class_estados_modulo_radicacion_config.url_escaner = "../workflow/WebFormEscan.aspx"
            class_estados_modulo_radicacion_config.DG_SELECION_TREE = ""
            class_estados_modulo_radicacion_config.ID_TAREA_SELECCIONDA = stru_registro_estado.id_tarea_workflow
            stru_permisos_interface_envio.estado_modulo = 2
            class_estados_modulo_radicacion_config.stru_permisos_interface_envio = stru_permisos_interface_envio
            class_estados_modulo_radicacion_config.stru_registro_estado = stru_registro_estado
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = class_estados_modulo_radicacion_config.DG_TIPODIGITALIZACION
            HttpContext.Current.Session.Item("DG_ID_TRAMITE") = class_estados_modulo_radicacion_config.DG_ID_TRAMITE
            HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = class_estados_modulo_radicacion_config.DG_TIPO_TRAMITE
            HttpContext.Current.Session.Item("DG_ID_GABINETE") = class_estados_modulo_radicacion_config.DG_ID_GABINETE
            HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE") = class_estados_modulo_radicacion_config.DG_NOMBRE_GABINETE
            HttpContext.Current.Session.Item("DG_RADICADO") = class_estados_modulo_radicacion_config.DG_RADICADO
            HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = class_estados_modulo_radicacion_config.DG_LISTA_CHEQUEO
            HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = class_estados_modulo_radicacion_config.DG_ID_CONFIG_DIGITALIZACION
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = class_estados_modulo_radicacion_config.DG_SELECION_TREE
            HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE") = class_estados_modulo_radicacion_config.DG_NOMBRE_TRAMITE
            HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = class_estados_modulo_radicacion_config.RA_TIPO_MODULO_GESTION_ENVIO_RADICADO
            HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO") = class_estados_modulo_radicacion_config.RA_ID_REGISTRO_RADICADO
            HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = class_estados_modulo_radicacion_config.RA_RADICADO_REGISTRO
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = class_estados_modulo_radicacion_config.ID_TAREA_SELECCIONDA
            HttpContext.Current.Session.Item("SELECCIONTEMPORAL") = class_estados_modulo_radicacion_config.SELECCIONTEMPORAL
            Solicita_estructura_estado_radicado_radicacion_simple = "YES"
        Catch ex As Exception
            Solicita_estructura_estado_radicado_radicacion_simple = "Inconsitencia general función Solicita_estructura_estado_radicado_radicacion_simple " & ex.Message
        End Try
    End Function
    Function Solicita_id_estado_modulo_radicado(ByVal radicado As String,
                                                ByVal info_esistencia As Integer,
                                                ByRef id_estado_radicado As Integer,
                                                ByRef estado As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select id_estado_radicado,estado from ra_rad_estados_modulo_radicacion " &
              " where  consecutivo_radicado='" & radicado & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_estado_modulo_radicado = "Función  Solicita_id_estado_modulo_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_estado_radicado = 0
                estado = 0
                If info_esistencia = 1 Then
                    Solicita_id_estado_modulo_radicado = "Imposible encontrar el estado de radicación inicail del radicado (" & radicado & ")"
                    Exit Function
                Else
                    Solicita_id_estado_modulo_radicado = "YES"
                    Exit Function
                End If
            Else
                id_estado_radicado = Datset.Tables(0).Rows(0).Item(0)
                estado = Datset.Tables(0).Rows(0).Item(1)
                Solicita_id_estado_modulo_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_estado_modulo_radicado = "Inconistencia general funcion Solicita_id_estado_modulo_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_id_estado_modulo_id_tarea_workflow(ByVal id_tarea_workflow As Long,
                                                        ByVal info_esistencia As Integer,
                                                        ByRef id_estado_radicado As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select id_estado_radicado from ra_rad_estados_modulo_radicacion " &
              " where  id_tarea_workflow=" & id_tarea_workflow
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_estado_modulo_id_tarea_workflow = "Función  Solicita_id_estado_modulo_id_tarea_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_estado_radicado = 0
                If info_esistencia = 1 Then
                    Solicita_id_estado_modulo_id_tarea_workflow = "Imposible encontrar el estado de radicación con el id tarea (" & id_tarea_workflow & ")"
                    Exit Function
                Else
                    Solicita_id_estado_modulo_id_tarea_workflow = "YES"
                    Exit Function
                End If
            Else
                id_estado_radicado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_estado_modulo_id_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_estado_modulo_id_tarea_workflow = "Inconistencia general funcion Solicita_id_estado_modulo_id_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_radicado_existencia_radicado_asignado(ByVal id_usuario_radicado As Integer,
                                                            ByVal id_plantilla As Integer,
                                                            ByVal tipo_plantilla As Integer,
                                                            ByRef class_estado_modulo_radicado As class_estado_modulo_radicado) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita exitencia de radicado asignado en el modulo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_usuario_radicado : Representa la identificación del usuario radicador
        'id_plantilla        : Representa la identificación de la plantilla de radicación
        'tipo_plantilla      : Representa el tipo de plantilla de radicacion 1-Entrante 2- Saliente
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_estado_modulo_radicado  : Retorna la estructura
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-10-29
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "select consecutivo_radicado,id_tarea_workflow,id_estado_radicado from ra_rad_estados_modulo_radicacion " &
                " where estado=0 and id_usuario_radicado=" & id_usuario_radicado & " and tipo_plantilla_radicado=" & tipo_plantilla &
                " and system_plantilla_radicado_id_Plantilla=" & id_plantilla
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_radicado_existencia_radicado_asignado = "Funcion  Solicita_radicado_existencia_radicado_asignado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                class_estado_modulo_radicado.estado_asignado = "NO"
                Solicita_radicado_existencia_radicado_asignado = "YES"
                Exit Function
            Else
                class_estado_modulo_radicado.estado_asignado = "YES"
                class_estado_modulo_radicado.radicado = Datset.Tables(0).Rows(0).Item(0)
                class_estado_modulo_radicado.id_tarea_workflow = Datset.Tables(0).Rows(0).Item(1)
                class_estado_modulo_radicado.id_registro_estado = Datset.Tables(0).Rows(0).Item(2)
                Solicita_radicado_existencia_radicado_asignado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_radicado_existencia_radicado_asignado = "Inconistencia general funcion Solicita_radicado_existencia_radicado_asignado " & ex.Message
        End Try
    End Function
    Function Solicita_radicado_existencia_radicado_asignado(ByVal id_usuario_radicado As Integer,
                                                            ByVal id_plantilla As Integer,
                                                            ByVal tipo_plantilla As Integer,
                                                            ByRef estado_asignado As String,
                                                            ByRef radicado As String,
                                                            ByRef id_tarea_workflow As Long,
                                                            ByRef id_registro_estado As Long) As String
        Try
            Dim Parametro_Consulta As String = "select consecutivo_radicado,id_tarea_workflow,id_estado_radicado from ra_rad_estados_modulo_radicacion " &
                " where estado=0 and id_usuario_radicado=" & id_usuario_radicado & " and tipo_plantilla_radicado=" & tipo_plantilla &
                " and system_plantilla_radicado_id_Plantilla=" & id_plantilla
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_radicado_existencia_radicado_asignado = "Funcion  Solicita_radicado_existencia_radicado_asignado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_asignado = "NO"
                Solicita_radicado_existencia_radicado_asignado = "YES"
                Exit Function
            Else
                estado_asignado = "YES"
                radicado = Datset.Tables(0).Rows(0).Item(0)
                id_tarea_workflow = Datset.Tables(0).Rows(0).Item(1)
                id_registro_estado = Datset.Tables(0).Rows(0).Item(2)
                Solicita_radicado_existencia_radicado_asignado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_radicado_existencia_radicado_asignado = "Inconistencia general funcion Solicita_radicado_existencia_radicado_asignado " & ex.Message
        End Try
    End Function
    Function SolicitaDatosEstructuraEstadoRadicado(ByVal IdRegistroEstado As Long,
                                                   ByRef StruRegistroEstado As stru_registro_estado) As String
        Try
            Dim SqlConsulta As String = "select system_plantilla_radicado_id_Plantilla,id_radicado,consecutivo_radicado," &
                                               "fecha_registro,estado,remitente,id_usuario_radicado,id_tarea_workflow,tipo_doc_entrante_id_Tipo_Doc_Entrante" &
                                               " from ra_rad_estados_modulo_radicacion " &
                                               " where id_estado_radicado=" & IdRegistroEstado
            Dim ConectDabase As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result = ConectDabase.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Return Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Return "Imposible encontrar los datos de estado del reistro de radicado  (" & IdRegistroEstado & ")"
            Else
                StruRegistroEstado.system_plantilla_radicado_id_Plantilla = Datset.Tables(0).Rows(0).Item(0)
                StruRegistroEstado.id_radicado = Datset.Tables(0).Rows(0).Item(1)
                StruRegistroEstado.consecutivo_radicado = Datset.Tables(0).Rows(0).Item(2)
                StruRegistroEstado.fecha_registro = Datset.Tables(0).Rows(0).Item(3)
                StruRegistroEstado.estado = Datset.Tables(0).Rows(0).Item(4)
                StruRegistroEstado.remitente = Datset.Tables(0).Rows(0).Item(5)
                StruRegistroEstado.id_usuario_radicado = Datset.Tables(0).Rows(0).Item(6)
                StruRegistroEstado.id_tarea_workflow = Datset.Tables(0).Rows(0).Item(7)
                StruRegistroEstado.tipo_doc_entrante_id_Tipo_Doc_Entrante = Datset.Tables(0).Rows(0).Item(8)
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia general funcion SolicitaDatosEstructuraEstadoRadicado " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_registro_modulo_radicacion(ByVal id_registro_estado As Long,
                                                         ByVal estado As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza la tabla estado del radicado y registra el error de asignación
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_registro_estado  : Representa la identificación del registro de estado del radicado
        'estado              : Representa el nombre del campo de radicación destino
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-11-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Update As String = "update ra_rad_estados_modulo_radicacion set estado= " & estado &
               " where id_estado_radicado =" & id_registro_estado
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref2.SELECTION_INSERT_COMMAND(Update)
            If Result <> "YES" Then
                Actualiza_estado_registro_modulo_radicacion = "Funcion  Sube_radicado_a_estado_pendiente dice " & Result
                Exit Function
            Else
                Actualiza_estado_registro_modulo_radicacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_estado_registro_modulo_radicacion = "Inconsistencia general funcion Actualiza_estado_registro_modulo_radicacion " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_registro_modulo_radicacion_error(ByVal id_registro_estado As Long,
                                                               ByVal estado As Integer,
                                                               ByVal error_asing As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza la tabla estado del radicado y registra el error de asignación
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_registro_estado  : Representa la identificación del registro de estado del radicado
        'estado              : Representa el nombre del campo de radicación destino
        'error_asing         : Representa el detalle de error de asignación
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-11-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Update As String = "update ra_rad_estados_modulo_radicacion set estado= " & estado &
               " , log_error_wf_asing='" & error_asing & "' where id_estado_radicado =" & id_registro_estado
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref2.SELECTION_INSERT_COMMAND(Update)
            If Result <> "YES" Then
                Actualiza_estado_registro_modulo_radicacion_error = "Funcion  Actualiza_estado_registro_modulo_radicacion_error dice " & Result
                Exit Function
            Else
                Actualiza_estado_registro_modulo_radicacion_error = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_estado_registro_modulo_radicacion_error = "Inconsistencia general funcion Actualiza_estado_registro_modulo_radicacion " & ex.Message
        End Try
    End Function
    Function Interface_documentos_relacionados_radicados(ByVal pag As Page,
                                                         ByVal opcion_agrega_inten_listview As Integer,
                                                         ByVal nombre_listview As String,
                                                         ByVal opcion_evalua_actualiza_iframe As Integer,
                                                         ByVal nombre_iframe As String,
                                                         ByVal url_iframe As String,
                                                         ByVal opcion_evalua_actualiza_panel As Integer,
                                                         ByVal opcion_evalua_titulo_radicado As Integer,
                                                         ByVal radicado As String) As String
        Try
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = ""
            HttpContext.Current.Session.Item("DG_ID_TRAMITE") = 0
            HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = ""
            HttpContext.Current.Session.Item("DG_ID_GABINETE") = 0
            HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE") = ""
            HttpContext.Current.Session.Item("DG_RADICADO") = ""
            HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
            HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = ""
            HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE") = ""
            HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = 0
            HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO") = 0
            HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = ""
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0"
            HttpContext.Current.Session.Item("SELECCIONTEMPORAL") = ""
            Dim treview As GridView = pag.FindControl(nombre_listview)
            Dim hiden_seleccion_documento As HtmlInputHidden = pag.FindControl("hiden_seleccion_documento")
            Dim Hidden_numero_doc_rel As HtmlInputHidden = pag.FindControl("Hidden_numero_doc_rel")
            Dim UpdatePanelseleccion_digitalizado As UpdatePanel = pag.FindControl("UpdatePanelseleccion_digitalizado")
            Dim UpdatePanelseleccion_label_documentos As UpdatePanel = pag.FindControl("UpdatePanelseleccion_label_documentos")
            Dim Label_documentos As Label = pag.FindControl("Label_documentos")
            Dim h_radicado_title As Label = pag.FindControl("h_radicado_title")
            Dim HiddenIdFlujo As HtmlInputHidden = pag.FindControl("HiddenIdFlujo")
            Dim UpdatePanel_title_radicado As UpdatePanel = pag.FindControl("UpdatePanel_title_radicado")
            If opcion_evalua_titulo_radicado = 1 Then
                If h_radicado_title Is Nothing Then
                    Interface_documentos_relacionados_radicados = "Imposible encontrar el control (h_radicado_title)"
                    Exit Function
                End If
                If UpdatePanel_title_radicado Is Nothing Then
                    Interface_documentos_relacionados_radicados = "Imposible encontrar el control (UpdatePanel_title_radicado)"
                    Exit Function
                End If
            End If
            Dim Datset As DataSet = New DataSet("ini")
            If opcion_agrega_inten_listview = 1 Then
                If Hidden_numero_doc_rel Is Nothing Then
                    Interface_documentos_relacionados_radicados = "Imposible encontrar el control (Hidden_numero_doc_rel)"
                    Exit Function
                End If
                If treview Is Nothing Then
                    Interface_documentos_relacionados_radicados = "Imposible encontrar el control (" & nombre_listview & ")"
                    Exit Function
                End If
                If UpdatePanelseleccion_digitalizado Is Nothing Then
                    Interface_documentos_relacionados_radicados = "Imposible encontrar el control (" & "UpdatePanelseleccion_digitalizado" & ")"
                    Exit Function
                End If
                If UpdatePanelseleccion_label_documentos Is Nothing Then
                    Interface_documentos_relacionados_radicados = "Imposible encontrar el control (" & "UpdatePanelseleccion_label_documentos" & ")"
                    Exit Function
                End If
                If Label_documentos Is Nothing Then
                    Interface_documentos_relacionados_radicados = "Imposible encontrar el control (" & "Label_documentos" & ")"
                    Exit Function
                End If
                If hiden_seleccion_documento Is Nothing Then
                    Interface_documentos_relacionados_radicados = "Imposible encontrar el control (" & "hiden_seleccion_documento" & ")"
                    Exit Function
                End If
            End If
            Dim iframe As Object = pag.FindControl(nombre_iframe)
            Dim UpdatePanel_iframe_digitaliza As UpdatePanel = pag.FindControl("UpdatePanel_iframe_digitaliza")
            If opcion_evalua_actualiza_iframe = 1 Then
                If iframe Is Nothing Then
                    Interface_documentos_relacionados_radicados = "Imposible encontrar el control (" & nombre_iframe & ")"
                    Exit Function
                End If
            End If
            Dim ref_UpdatePanel_boton_tool As UpdatePanel = pag.FindControl("UpdatePanel_boton_tool")
            Dim Ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim Nombre_plantilla As String = ""
            Dim Result As String = ""
            Result = Ref_Class_system_plantilla_radicado.Solicita_nombre_plantilla_radicado(HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"),
                                                                                            Nombre_plantilla)
            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            Result = Ref_Class_system_plantilla_radicado.Retorna_Tipo_Plantilla(HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"),
                                                                                HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"))
            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            Dim ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Dim Tipo_tramite As String = ""
            Result = ref_Class_plantillas_radicacion.retorna_tipo_documental_radicado(radicado,
                                                                                      Nombre_plantilla,
                                                                                      HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"))
            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            Dim Ref_Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Ref_Class_tipo_doc_entrante.retorna_id_tipo_tramite_radicado(HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"),
                                                                                  HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                  HttpContext.Current.Session.Item("DG_ID_TRAMITE"))
            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            Result = Ref_Class_tipo_doc_entrante.Retorna_id_nombre_gabinete_tipo_tramite(HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"),
                                                                                                     HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                                     HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                                                     HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"))
            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            Dim id_config_digitalizacion As Integer = 0
            Dim ref_class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Result = ref_class_ra_dig_config_digitalizacion.Solicita_id_configuracion_digitalizacion(HttpContext.Current.Session.Item("DG_ID_TRAMITE"),
                                                                                                     HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"),
                                                                                                     HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION"), 0)
            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            Dim Ref_class_detalle_gabinete As New Class_DETALLE_GABIENETE
            Dim nombre_campo_radicado_gabinete As String = ""
            Result = Ref_class_detalle_gabinete.SolicitaNombreCampoRadicadoGabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                nombre_campo_radicado_gabinete)
            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            Dim Ref_class_system1 As New Class_system1
            Dim inventario_documental As Integer
            Dim aplica_trd As Integer
            Dim asigna_unidad As Integer
            Result = Ref_class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            If opcion_agrega_inten_listview = 1 Then
                treview.Visible = True
                Dim ref_class_da_gabinete As New ClassDaGabinete
                Result = ref_class_da_gabinete.Lista_documentos_relacionados_a_radicado_enlace(nombre_campo_radicado_gabinete,
                                                                                              HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                              radicado,
                                                                                              aplica_trd, HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                              1,
                                                                                              treview,
                                                                                              Label_documentos,
                                                                                              hiden_seleccion_documento,
                                                                                              UpdatePanelseleccion_digitalizado,
                                                                                              UpdatePanelseleccion_label_documentos,
                                                                                              Val(Hidden_numero_doc_rel.Value))

                If Result <> "YES" Then
                    Interface_documentos_relacionados_radicados = Result
                    Exit Function
                End If
            End If
            If opcion_evalua_actualiza_iframe = 1 Then
                If iframe.Attributes.Item("src") = "#_" Then
                    iframe.Attributes.Add("src", url_iframe)
                    iframe.visible = True
                    UpdatePanel_iframe_digitaliza.Update()
                End If
            End If
            Dim nombre_campo_radicado As String = ""
            Dim Ref_class_config As New Class_configuracion_listado_ruta
            Result = Ref_class_config.SolicitaNombreCampoRadicadoRuta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                      nombre_campo_radicado)

            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            Dim Ref_class_adic_tar As New Class_DAT_ADIC_TAR
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = 0
            Result = Ref_class_adic_tar.Solicita_id_tarea_radicado(radicado,
                                                                  HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                  nombre_campo_radicado,
                                                                  HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                  1)
            If Result <> "YES" Then
                Interface_documentos_relacionados_radicados = Result
                Exit Function
            End If
            ref_UpdatePanel_boton_tool.Update()
            Dim Ruta_Web_Escaner As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString.Replace("\", "/")
            'HiddenRuta.Value = Ruta_Web_Escaner
            HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = radicado
            HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO") = 0
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE"
            HttpContext.Current.Session.Item("SELECCIONTEMPORAL") = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") & "|" & HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = ""
            If opcion_evalua_titulo_radicado = 1 Then
                h_radicado_title.Text = "Radicado : " & radicado
                UpdatePanel_title_radicado.Update()
            End If
            HiddenIdFlujo.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            Interface_documentos_relacionados_radicados = "YES"
        Catch ex As Exception
            Interface_documentos_relacionados_radicados = "Inconsistencia general función Interface_documentos_radicados " & ex.Message
        End Try
    End Function
    Function Asignar_radicado(ByVal id_registro_estado As Long,
                              ByVal evalua_radicado_asignado As String,
                              ByVal evalua_registro_flujo As String,
                              ByVal pag As Page,
                              ByVal opcion_agrega_inten_listview As Integer,
                              ByVal nombre_listview As String,
                              ByVal opcion_evalua_actualiza_iframe As Integer,
                              ByVal nombre_iframe As String,
                              ByVal url_iframe As String,
                              ByVal opcion_evalua_actualiza_panel As Integer,
                              ByVal opcion_evalua_titulo_radicado As Integer,
                              ByRef stru_parameter_image() As stru_paramter_image,
                              ByRef stru_permisos_interface_envio As stru_permisos_interface_envio) As String
        Try

            Dim Result As String = ""
            Dim panel_terminar_rad As Panel = pag.FindControl("Panel_terminar_radicado")
            Dim Panel_EnviarUsuario As Panel = pag.FindControl("Panel_EnviarUsuario")
            Dim Panel_EnviaActividad As Panel = pag.FindControl("Panel_EnviaActividad")
            Dim Panel_autoterminar As Panel = pag.FindControl("Panel_auto_terminar")
            Dim Panel_enviar_flujo As Panel = pag.FindControl("Panel_enviar_flujo")
            Dim Label_estado_selecion As Label = pag.FindControl("Label_estado_selecion")
            Dim h_radicado_title As Label = pag.FindControl("h_radicado_title")
            Dim UpdatePanel_title_radicado As UpdatePanel = pag.FindControl("UpdatePanel_title_radicado")
            Dim UpdatePanel_boton_nuevo_radicado As UpdatePanel = pag.FindControl("UpdatePanel_boton_nuevo_radicado")
            Dim Button_nuevo_radicado As Button = pag.FindControl("Button_nuevo_radicado")
            Dim updatemenu As UpdatePanel = pag.FindControl("updatemenu")
            Dim Hidden_radicado_seleccion As HtmlInputHidden = pag.FindControl("Hidden_radicado_seleccion")
            Dim Label_numero_item As Label = pag.FindControl("Label_numero_item")
            Dim Hidden_numero_rad_pend As HtmlInputHidden = pag.FindControl("Hidden_numero_rad_pend")
            Dim HiddenIdFlujo As HtmlInputHidden = pag.FindControl("HiddenIdFlujo")
            Dim HiddenRuta As HtmlInputHidden = pag.FindControl("HiddenRuta")
            Dim Panel_imprime_rotulo As Panel = pag.FindControl("Panel_imprime_rotulo")
            Dim Panel_cargar_archivo As Panel = pag.FindControl("Panel_cargar_archivo")
            If opcion_evalua_titulo_radicado = 1 Then
                If h_radicado_title Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (h_radicado_title)"
                    Exit Function
                End If
                If UpdatePanel_title_radicado Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (UpdatePanel_title_radicado)"
                    Exit Function
                End If
                If UpdatePanel_boton_nuevo_radicado Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (UpdatePanel_boton_nuevo_radicado)"
                    Exit Function
                End If
                If Button_nuevo_radicado Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Button_nuevo_radicado)"
                    Exit Function
                End If
            End If
            If opcion_evalua_actualiza_panel = 1 Then
                If Panel_cargar_archivo Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Panel_cargar_archivo)"
                    Exit Function
                End If
                If Panel_imprime_rotulo Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Panel_imprime_rotulo )"
                    Exit Function
                End If
                If Hidden_numero_rad_pend Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Hidden_numero_rad_pend)"
                    Exit Function
                End If
                If Label_numero_item Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Label_numero_item)"
                    Exit Function
                End If
                If Panel_enviar_flujo Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Panel_enviar_flujo)"
                    Exit Function
                End If
                If Panel_autoterminar Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Panel_auto_terminar)"
                    Exit Function
                End If
                If panel_terminar_rad Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Panel_terminar_radicado)"
                    Exit Function
                End If
                If updatemenu Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (updatemenu)"
                    Exit Function
                End If
                If Panel_EnviarUsuario Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Panel_EnviarUsuario)"
                    Exit Function
                End If
                If Panel_EnviaActividad Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Panel_EnviaActividad)"
                    Exit Function
                End If
                If Label_estado_selecion Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Label_estado_selecion)"
                    Exit Function
                End If
            End If

            Dim treview As GridView = pag.FindControl(nombre_listview)
            Dim hiden_seleccion_documento As HtmlInputHidden = pag.FindControl("hiden_seleccion_documento")
            Dim Hidden_numero_doc_rel As HtmlInputHidden = pag.FindControl("Hidden_numero_doc_rel")
            Dim UpdatePanelseleccion_digitalizado As UpdatePanel = pag.FindControl("UpdatePanelseleccion_digitalizado")
            Dim UpdatePanelseleccion_label_documentos As UpdatePanel = pag.FindControl("UpdatePanelseleccion_label_documentos")
            Dim Label_documentos As Label = pag.FindControl("Label_documentos")
            Dim Datset As DataSet = New DataSet("ini")
            If opcion_agrega_inten_listview = 1 Then
                If Hidden_numero_doc_rel Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (Hidden_numero_doc_rel)"
                    Exit Function
                End If
                If treview Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (" & nombre_listview & ")"
                    Exit Function
                End If
                If UpdatePanelseleccion_digitalizado Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (" & "UpdatePanelseleccion_digitalizado" & ")"
                    Exit Function
                End If
                If UpdatePanelseleccion_label_documentos Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (" & "UpdatePanelseleccion_label_documentos" & ")"
                    Exit Function
                End If
                If Label_documentos Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (" & "Label_documentos" & ")"
                    Exit Function
                End If
                If hiden_seleccion_documento Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (" & "hiden_seleccion_documento" & ")"
                    Exit Function
                End If
            End If
            Dim iframe As Object = pag.FindControl(nombre_iframe)
            Dim UpdatePanel_iframe_digitaliza As UpdatePanel = pag.FindControl("UpdatePanel_iframe_digitaliza")
            If opcion_evalua_actualiza_iframe = 1 Then
                If iframe Is Nothing Then
                    Asignar_radicado = "Imposible encontrar el control (" & nombre_iframe & ")"
                    Exit Function
                End If
            End If
            Dim ref_UpdatePanel_boton_tool As UpdatePanel = pag.FindControl("UpdatePanel_boton_tool")
            '------------------------------------------------------------------------------------
            'Solicita el numero de radicados asignados pendientes
            '------------------------------------------------------------------------------------
            Dim Numero_pendietes As Integer = 0
            Result = Me.Solicita_numero_radicados_pendientes(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                             HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"),
                                                             HttpContext.Current.Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO"),
                                                             Numero_pendietes)
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            If opcion_evalua_actualiza_panel = 1 Then
                Label_numero_item.Text = Numero_pendietes
                Hidden_numero_rad_pend.Value = Numero_pendietes
            End If
            '------------------------------------------------------------------------------------
            'Evalua radicado asignado, esta opción se registra cuando el usuario no ha terminado
            'con un radicado y quiere enviarlo a pendiente
            '------------------------------------------------------------------------------------
            Dim Estado_asignado As String = ""
            Dim Radicado As String = ""
            Dim id_tarea_workflow As Long
            Dim id_registro_estado_existente As Long
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            If evalua_radicado_asignado = "YES" Then
                Result = Class_ra_rad_estados_modulo_radicacion.Solicita_radicado_existencia_radicado_asignado(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                                                              HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"),
                                                                                                              HttpContext.Current.Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO"),
                                                                                                              Estado_asignado,
                                                                                                              Radicado,
                                                                                                              id_tarea_workflow,
                                                                                                              id_registro_estado_existente)
                If Result <> "YES" Then
                    Asignar_radicado = Result
                    Exit Function
                End If
                '-----------------------------------------------
                'Envia el radicado asignado a estado pendiente
                '-----------------------------------------------
                If Estado_asignado = "YES" Then
                    Result = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion(id_registro_estado_existente,
                                                                                                                1)
                    If Result <> "YES" Then
                        Asignar_radicado = "Impsoble enviar el radicado actual a estado pediente, error : " & Result
                        Exit Function
                    Else
                        HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = ""
                        HttpContext.Current.Session.Item("DG_ID_TRAMITE") = 0
                        HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = ""
                        HttpContext.Current.Session.Item("DG_ID_GABINETE") = 0
                        HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE") = ""
                        HttpContext.Current.Session.Item("DG_RADICADO") = ""
                        HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
                        HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
                        HttpContext.Current.Session.Item("DG_SELECION_TREE") = ""
                        HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE") = ""
                        HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = 0
                        HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO") = 0
                        HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = ""
                        HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0"
                        HttpContext.Current.Session.Item("SELECCIONTEMPORAL") = ""
                        '---------------------------------------------------------------
                        'Oculta interface workflow
                        '---------------------------------------------------------------
                        stru_permisos_interface_envio.estado_modulo = 0
                        stru_permisos_interface_envio.terminar_radicado = 0
                        stru_permisos_interface_envio.enviar_usuario = 0
                        stru_permisos_interface_envio.enviar_actividad = 0
                        stru_permisos_interface_envio.auto_terminar = 0
                        stru_permisos_interface_envio.nombre_flujo_ruta = ""
                        stru_permisos_interface_envio.estado_cerrado = 0
                        stru_permisos_interface_envio.estado_modulo = 1
                        If opcion_evalua_actualiza_panel = 1 Then
                            Panel_EnviarUsuario.Visible = False
                            Panel_EnviaActividad.Visible = False
                            Panel_autoterminar.Visible = False
                            panel_terminar_rad.Visible = False
                            Panel_enviar_flujo.Visible = False
                            Panel_cargar_archivo.Visible = False
                            Panel_imprime_rotulo.Visible = False
                            Label_estado_selecion.Text = ""
                        End If
                        '---------------------------------------------------------------
                        'Limpia documentos relacionados
                        '---------------------------------------------------------------
                        If opcion_agrega_inten_listview = 1 Then
                            treview.Visible = False
                            Label_documentos.Text = "Documentos 0"
                            Hidden_numero_doc_rel.Value = 0
                            hiden_seleccion_documento.Value = -1
                            UpdatePanelseleccion_digitalizado.Update()
                            UpdatePanelseleccion_label_documentos.Update()
                        End If
                        If opcion_evalua_titulo_radicado = 1 Then
                            h_radicado_title.Text = "Radicado : " & ""
                            HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = ""
                            Button_nuevo_radicado.Visible = True
                            UpdatePanel_title_radicado.Update()
                            UpdatePanel_boton_nuevo_radicado.Update()
                        End If
                        Hidden_radicado_seleccion.Value = ""
                        If opcion_evalua_actualiza_panel = 1 Then
                            Numero_pendietes = Numero_pendietes + 1
                            Label_numero_item.Text = Numero_pendietes
                            Hidden_numero_rad_pend.Value = Numero_pendietes
                            updatemenu.Update()
                        End If
                        If opcion_evalua_actualiza_iframe = 1 Then
                            'iframe.visible = False
                            'UpdatePanel_iframe_digitaliza.Update()
                        End If
                        If id_registro_estado = 0 Then
                            Asignar_radicado = "YES"
                            Exit Function
                        End If
                    End If
                End If
            End If
            '---------------------------------------------------------------
            'Oculta interface workflow
            '---------------------------------------------------------------
            stru_permisos_interface_envio.estado_modulo = 0
            stru_permisos_interface_envio.terminar_radicado = 0
            stru_permisos_interface_envio.enviar_usuario = 0
            stru_permisos_interface_envio.enviar_actividad = 0
            stru_permisos_interface_envio.auto_terminar = 0
            stru_permisos_interface_envio.nombre_flujo_ruta = ""
            stru_permisos_interface_envio.estado_cerrado = 0
            stru_permisos_interface_envio.estado_modulo = 1
            If opcion_evalua_actualiza_panel = 1 Then
                Panel_EnviarUsuario.Visible = False
                Panel_EnviaActividad.Visible = False
                Panel_autoterminar.Visible = False
                panel_terminar_rad.Visible = False
                Panel_enviar_flujo.Visible = False
                Panel_imprime_rotulo.Visible = False
                Panel_cargar_archivo.Visible = False
                Label_estado_selecion.Text = ""
            End If
            '---------------------------------------------------------------
            'Limpia documentos relacionados
            '---------------------------------------------------------------
            If opcion_agrega_inten_listview = 1 Then
                treview.Visible = False
                Label_documentos.Text = "Documentos 0"
                Hidden_numero_doc_rel.Value = 0
                hiden_seleccion_documento.Value = -1
                UpdatePanelseleccion_digitalizado.Update()
                UpdatePanelseleccion_label_documentos.Update()
            End If
            If opcion_evalua_actualiza_iframe = 1 Then
                'iframe.visible = False
                'UpdatePanel_iframe_digitaliza.Update()
            End If
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = ""
            HttpContext.Current.Session.Item("DG_ID_TRAMITE") = 0
            HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = ""
            HttpContext.Current.Session.Item("DG_ID_GABINETE") = 0
            HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE") = ""
            HttpContext.Current.Session.Item("DG_RADICADO") = ""
            HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
            HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = ""
            HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE") = ""
            HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = 0
            HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO") = 0
            HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = ""
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0"
            HttpContext.Current.Session.Item("SELECCIONTEMPORAL") = ""
            If id_registro_estado = 0 Then
                If opcion_evalua_actualiza_panel = 1 Then
                    updatemenu.Update()
                End If
                If opcion_evalua_titulo_radicado = 1 Then
                    h_radicado_title.Text = "Radicado : " & ""
                    HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = ""
                    Button_nuevo_radicado.Visible = True
                    UpdatePanel_title_radicado.Update()
                    UpdatePanel_boton_nuevo_radicado.Update()
                End If
                Asignar_radicado = "YES"
                Exit Function
            End If
            Dim stru_registro_estado As stru_registro_estado = Nothing
            Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(id_registro_estado,
                                                                                                      stru_registro_estado)
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            Dim Ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim Nombre_plantilla As String = ""
            Result = Ref_Class_system_plantilla_radicado.Solicita_nombre_plantilla_radicado(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                            Nombre_plantilla)
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            Result = Ref_Class_system_plantilla_radicado.Retorna_Tipo_Plantilla(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"))
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            Dim ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Dim Tipo_tramite As String = ""
            Result = ref_Class_plantillas_radicacion.retorna_tipo_documental_radicado(stru_registro_estado.consecutivo_radicado,
                                                                                      Nombre_plantilla,
                                                                                      HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"))
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If

            Dim Ref_Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Ref_Class_tipo_doc_entrante.retorna_id_tipo_tramite_radicado(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                  HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                  HttpContext.Current.Session.Item("DG_ID_TRAMITE"))
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            Result = Ref_Class_tipo_doc_entrante.Retorna_id_nombre_gabinete_tipo_tramite(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                         HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                         HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                                         HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"))
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            Dim id_config_digitalizacion As Integer = 0
            Dim ref_class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Result = ref_class_ra_dig_config_digitalizacion.Solicita_id_configuracion_digitalizacion(HttpContext.Current.Session.Item("DG_ID_TRAMITE"),
                                                                                                     HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"),
                                                                                                     HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                                     0)
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            Dim Ref_class_detalle_gabinete As New Class_DETALLE_GABIENETE
            Dim nombre_campo_radicado_gabinete As String = ""
            Result = Ref_class_detalle_gabinete.SolicitaNombreCampoRadicadoGabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                nombre_campo_radicado_gabinete)
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            Dim Ref_class_system1 As New Class_system1
            Dim inventario_documental As Integer
            Dim aplica_trd As Integer
            Dim asigna_unidad As Integer
            Result = Ref_class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            Result = Ref_Class_tipo_doc_entrante.Solicita_tipo_modulo_soporte_documental_envio(HttpContext.Current.Session.Item("DG_ID_TRAMITE"),
                                                                                               HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO"))
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            Dim id_flujo_trabajo As Integer = 0
            Result = ref_Class_plantillas_radicacion.Retorna_id_flujo_trabajo_radicado(Nombre_plantilla,
                                                                                       stru_registro_estado.consecutivo_radicado,
                                                                                       id_flujo_trabajo)
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            '----------------------------------------------
            'Verifica asignación de flujo de documental
            '----------------------------------------------
            Dim Ref_class_workflow As New ClassWorkflow
            Dim Estado_modulo_respuesta As Integer = 0
            Dim Ref_class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            Dim Ref_class_radicador As New ClassRadicador
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_flujo_trabajo As Integer = 0
            Dim id_registro_actvidad_flujo_trabajo As Integer = 0
            '----------------------------------------------
            'Registra flujo de trabajo
            '----------------------------------------------
            If stru_registro_estado.id_tarea_workflow = 0 And HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = 2 _
                                                        And evalua_registro_flujo = 1 Then
                Result = Ref_Class_tipo_doc_entrante.Determina_gestion_modulo_pqr_Tipo_Tramite(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                               HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                               Estado_modulo_respuesta)
                If Result <> "YES" Then
                    Asignar_radicado = Result
                    Exit Function
                End If
                If id_flujo_trabajo <> 0 Then
                    Result = Ref_class_flujo_trabajo_workflow.Solicita_datos_actividad_inicio_flujo(id_flujo_trabajo,
                                                                                                    id_registro_actvidad_flujo_trabajo,
                                                                                                    id_actividad_flujo_trabajo,
                                                                                                    id_usuario_workflow_flujo_trabajo)
                    If Result <> "YES" Then
                        Asignar_radicado = Result
                        Exit Function
                    End If
                End If
                Dim id_tarea_workflow_ As Long = 0
                Dim Refclas_gestion_fecha As New ClassGestionFechas
                '-----------------------------
                'Formatea framework actual
                '-----------------------------
                Dim DateCreate As Date = Now
                Dim fecha_selecion As Object = Nothing
                Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                             fecha_selecion)
                If Result <> "YES" Then
                    Asignar_radicado = Result
                    Exit Function
                End If
                Result = Ref_class_workflow.Registra_flujo_documento(HttpContext.Current.Session.Item("Id_actividad_Workflow"),
                                                                     HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                     0,
                                                                     stru_registro_estado.consecutivo_radicado,
                                                                     stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                     id_flujo_trabajo,
                                                                     id_registro_actvidad_flujo_trabajo,
                                                                     id_usuario_workflow_flujo_trabajo,
                                                                     0,
                                                                     Estado_modulo_respuesta,
                                                                     id_tarea_workflow_,
                                                                     fecha_selecion, 1)
                If Result <> "YES" And id_tarea_workflow_ = 0 Then
                    Dim Rest As String = ""
                    Rest = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion(id_registro_estado,
                                                                         1)
                    If Rest <> "YES" Then
                        Asignar_radicado = Rest
                        Exit Function
                    End If
                    Asignar_radicado = "Imposible asignar el radicado error de asignacion error " & Result
                    Exit Function
                Else
                    Dim Rest As String = ""
                    Rest = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion(id_registro_estado,
                                                                         0)
                    If Rest <> "YES" Then
                        Asignar_radicado = Rest
                        Exit Function
                    End If
                    Rest = Me.Relaciona_id_tarea_wf_estado_radicado(id_registro_estado,
                                                                    id_tarea_workflow_)
                    If Rest <> "YES" Then
                        Asignar_radicado = Rest
                        Exit Function
                    End If
                    Result = Ref_class_radicador.Actualiza_estado_flow_radicado(stru_registro_estado.consecutivo_radicado,
                                                                              Nombre_plantilla,
                                                                              7)
                    If Rest <> "YES" Then
                        Asignar_radicado = Rest
                        Exit Function
                    End If
                    stru_registro_estado.id_tarea_workflow = id_tarea_workflow_
                End If
            End If
            '----------------------------------------------
            'Lista documentos cuando el tramite o radicado
            'pertenece a un flujo documental
            '----------------------------------------------
            Dim classdagabinete As New ClassDaGabinete
            Dim classselecion As New Classselecciotarea
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim id_imagen_seleccion As Integer = 0
            If stru_registro_estado.id_tarea_workflow <> 0 And HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = 2 Then
                Result = Class_DAT_ADIC_TAR.SolicitaIdImagenRelacionadaTareaworkflowIdRuta(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                           stru_registro_estado.id_tarea_workflow,
                                                                                           id_imagen_seleccion)
                If Result <> "YES" Then
                    Asignar_radicado = Result
                    Exit Function
                End If
            End If
            stru_parameter_image = Nothing
            Result = classdagabinete.Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado(nombre_campo_radicado_gabinete,
                                                                                                          HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                          stru_registro_estado.consecutivo_radicado,
                                                                                                          aplica_trd,
                                                                                                          stru_parameter_image)
            If Result <> "YES" Then
                Asignar_radicado = Result
                Exit Function
            End If
            If opcion_agrega_inten_listview = 1 Then
                treview.Visible = True
                Dim ref_class_da_gabinete As New ClassDaGabinete
                Result = ref_class_da_gabinete.Lista_documentos_relacionados_a_radicado_enlace(nombre_campo_radicado_gabinete,
                                                                                             HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                             stru_registro_estado.consecutivo_radicado,
                                                                                             aplica_trd, stru_registro_estado.id_tarea_workflow,
                                                                                             1,
                                                                                             treview,
                                                                                             Label_documentos,
                                                                                             hiden_seleccion_documento,
                                                                                             UpdatePanelseleccion_digitalizado,
                                                                                             UpdatePanelseleccion_label_documentos,
                                                                                             Val(Hidden_numero_doc_rel.Value))

                If Result <> "YES" Then
                    Asignar_radicado = Result
                    Exit Function
                End If
            End If
            If opcion_evalua_actualiza_iframe = 1 Then
                If iframe.Attributes.Item("src") = "#_" Then
                    iframe.Attributes.Add("src", url_iframe)
                    iframe.visible = True
                    UpdatePanel_iframe_digitaliza.Update()
                End If
            End If
            If evalua_radicado_asignado = "YES" Then
                Result = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion(id_registro_estado,
                                                                                                            0)
                If Result <> "YES" Then
                    Asignar_radicado = Result
                    Exit Function
                End If
                If opcion_evalua_actualiza_panel = 1 Then
                    Numero_pendietes = Numero_pendietes - 1
                    Label_numero_item.Text = Numero_pendietes
                    Hidden_numero_rad_pend.Value = Numero_pendietes
                    updatemenu.Update()
                End If
            End If
            Dim Ref_class_workflow_ruta As New Class_worflow_rutas
            If HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = 2 Then
                If opcion_evalua_actualiza_panel = 1 Then
                    panel_terminar_rad.Visible = False
                    Panel_imprime_rotulo.Visible = True
                    Panel_cargar_archivo.Visible = True
                End If
                stru_permisos_interface_envio.terminar_radicado = 0
                Dim estado_resp_obligatoria As Integer = 0
                Result = Ref_Class_tipo_doc_entrante.Retorna_tipo_respuesta_tramite_radicado(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                             HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                             estado_resp_obligatoria)
                If Result <> "YES" Then
                    Asignar_radicado = Result
                    Exit Function
                End If
                '----------------------------------------------
                'Configura interface para flujo de trabajo
                '---------------------------------------------
                If id_flujo_trabajo <> 0 Then
                    Result = Ref_class_flujo_trabajo_workflow.SolicitaEstadoAbiertoCerradoFlujoDocumental(id_flujo_trabajo,
                                                                                                               stru_permisos_interface_envio.estado_cerrado)
                    If Result <> "YES" Then
                        Asignar_radicado = Result
                        Exit Function
                    End If
                    Result = Ref_class_flujo_trabajo_workflow.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo,
                                                                                                        stru_permisos_interface_envio.nombre_flujo_ruta)
                    If Result <> "YES" Then
                        Asignar_radicado = Result
                        Exit Function
                    End If
                    Dim estado_cerrado As String = ""
                    If estado_resp_obligatoria = 1 Then
                        stru_permisos_interface_envio.terminar_radicado = 0
                        stru_permisos_interface_envio.enviar_usuario = 0
                        stru_permisos_interface_envio.enviar_actividad = 0
                        stru_permisos_interface_envio.auto_terminar = 1
                        stru_permisos_interface_envio.nombre_flujo_ruta = stru_permisos_interface_envio.nombre_flujo_ruta
                        If opcion_evalua_actualiza_panel = 1 Then
                            Panel_EnviarUsuario.Visible = False
                            Panel_EnviaActividad.Visible = False
                            Panel_autoterminar.Visible = True
                            panel_terminar_rad.Visible = False
                            Panel_enviar_flujo.Visible = False
                            If stru_permisos_interface_envio.estado_cerrado = 1 Then
                                estado_cerrado = "Abierto"
                            Else
                                estado_cerrado = "Cerrado"
                            End If
                            Label_estado_selecion.Text = "Flujo : " & stru_permisos_interface_envio.nombre_flujo_ruta & " Tipo flujo : " & estado_cerrado
                            updatemenu.Update()
                        End If
                    Else
                        If stru_permisos_interface_envio.estado_cerrado = 1 Then
                            stru_permisos_interface_envio.enviar_usuario = 0
                            stru_permisos_interface_envio.enviar_actividad = 0
                            stru_permisos_interface_envio.auto_terminar = 0
                            stru_permisos_interface_envio.estado_cerrado = 0
                            If opcion_evalua_actualiza_panel = 1 Then
                                Panel_EnviarUsuario.Visible = False
                                Panel_EnviaActividad.Visible = False
                                Panel_autoterminar.Visible = False
                                panel_terminar_rad.Visible = False
                                Panel_enviar_flujo.Visible = True
                                Label_estado_selecion.Text = "Flujo : " & stru_permisos_interface_envio.nombre_flujo_ruta & " Tipo flujo : " & "Cerrado"
                                updatemenu.Update()
                            End If
                        Else
                            stru_permisos_interface_envio.enviar_usuario = 1
                            stru_permisos_interface_envio.enviar_actividad = 1
                            stru_permisos_interface_envio.auto_terminar = 0
                            stru_permisos_interface_envio.estado_cerrado = 1
                            If opcion_evalua_actualiza_panel = 1 Then
                                Panel_EnviarUsuario.Visible = False
                                Panel_EnviaActividad.Visible = False
                                Panel_autoterminar.Visible = False
                                panel_terminar_rad.Visible = False
                                Panel_enviar_flujo.Visible = True
                                Label_estado_selecion.Text = "Flujo : " & stru_permisos_interface_envio.nombre_flujo_ruta & " Tipo flujo : " & "Abierto"
                                updatemenu.Update()
                            End If
                        End If
                    End If

                Else
                    '----------------------------------------------
                    'Configura interface para ruta de trabajo
                    '---------------------------------------------
                    Dim estado_ruta As Integer = 0
                    If stru_registro_estado.id_tarea_workflow <> 0 Then
                        Result = Ref_class_workflow_ruta.Solicita_etado_abierto_cerrado_ruta_tarea(stru_registro_estado.id_tarea_workflow,
                                                                                               HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                               estado_ruta,
                                                                                               "")
                        If Result <> "YES" Then
                            Asignar_radicado = Result
                            Exit Function
                        End If
                    End If
                    '-----------------------------------------------
                    'Configura interface para respuesta obligatoria
                    '-----------------------------------------------
                    If estado_resp_obligatoria = 1 Then
                        stru_permisos_interface_envio.terminar_radicado = 0
                        stru_permisos_interface_envio.enviar_usuario = 0
                        stru_permisos_interface_envio.enviar_actividad = 0
                        stru_permisos_interface_envio.auto_terminar = 1
                        stru_permisos_interface_envio.nombre_flujo_ruta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
                        If estado_ruta = 1 Then
                            stru_permisos_interface_envio.estado_cerrado = 0
                        Else
                            stru_permisos_interface_envio.estado_cerrado = 1
                        End If
                        Dim estado_cerrado As String = ""
                        If opcion_evalua_actualiza_panel = 1 Then
                            Panel_EnviarUsuario.Visible = False
                            Panel_EnviaActividad.Visible = False
                            Panel_autoterminar.Visible = True
                            panel_terminar_rad.Visible = False
                            Panel_enviar_flujo.Visible = False
                            If stru_permisos_interface_envio.estado_cerrado = 1 Then
                                estado_cerrado = "Abierto"
                            Else
                                estado_cerrado = "Cerrado"
                            End If
                            Label_estado_selecion.Text = "Ruta : " & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " Tipo ruta : " & estado_cerrado
                            updatemenu.Update()
                        End If
                    Else
                        If estado_ruta = 1 Then
                            stru_permisos_interface_envio.terminar_radicado = 0
                            stru_permisos_interface_envio.enviar_usuario = 0
                            stru_permisos_interface_envio.enviar_actividad = 1
                            stru_permisos_interface_envio.auto_terminar = 0
                            stru_permisos_interface_envio.nombre_flujo_ruta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
                            stru_permisos_interface_envio.estado_cerrado = 0
                            If opcion_evalua_actualiza_panel = 1 Then
                                Panel_EnviarUsuario.Visible = False
                                Panel_EnviaActividad.Visible = False
                                Panel_autoterminar.Visible = False
                                panel_terminar_rad.Visible = False
                                Panel_enviar_flujo.Visible = True
                                Dim estado_cerrado As String = ""
                                If stru_permisos_interface_envio.estado_cerrado = 1 Then
                                    estado_cerrado = "Abierto"
                                Else
                                    estado_cerrado = "Cerrado"
                                End If
                                Label_estado_selecion.Text = "Ruta : " & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " Tipo ruta : " & estado_cerrado
                                updatemenu.Update()
                            End If
                        Else
                            stru_permisos_interface_envio.terminar_radicado = 0
                            stru_permisos_interface_envio.enviar_usuario = 1
                            stru_permisos_interface_envio.enviar_actividad = 1
                            stru_permisos_interface_envio.auto_terminar = 0
                            stru_permisos_interface_envio.nombre_flujo_ruta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
                            stru_permisos_interface_envio.estado_cerrado = 1
                            If opcion_evalua_actualiza_panel = 1 Then
                                Panel_EnviarUsuario.Visible = True
                                Panel_EnviaActividad.Visible = True
                                Panel_autoterminar.Visible = False
                                panel_terminar_rad.Visible = False
                                Panel_enviar_flujo.Visible = True
                                Dim estado_cerrado As String = ""
                                If stru_permisos_interface_envio.estado_cerrado = 1 Then
                                    estado_cerrado = "Abierto"
                                Else
                                    estado_cerrado = "Cerrado"
                                End If
                                Label_estado_selecion.Text = "Ruta : " & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " Tipo ruta : " & estado_cerrado
                                updatemenu.Update()
                            End If
                        End If
                    End If
                End If
            End If
            If HttpContext.Current.Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = 3 Then
                stru_permisos_interface_envio.terminar_radicado = 1
                stru_permisos_interface_envio.enviar_usuario = 0
                stru_permisos_interface_envio.enviar_actividad = 0
                stru_permisos_interface_envio.auto_terminar = 0
                stru_permisos_interface_envio.nombre_flujo_ruta = ""
                stru_permisos_interface_envio.estado_cerrado = 0
                If opcion_evalua_actualiza_panel = 1 Then
                    Panel_EnviarUsuario.Visible = False
                    Panel_EnviaActividad.Visible = False
                    Panel_autoterminar.Visible = False
                    panel_terminar_rad.Visible = True
                    Panel_enviar_flujo.Visible = False
                    Panel_imprime_rotulo.Visible = True
                    Panel_cargar_archivo.Visible = True
                    Label_estado_selecion.Text = ""
                    updatemenu.Update()
                End If
            End If
            If opcion_evalua_titulo_radicado = 1 Then
                h_radicado_title.Text = "Radicado : " & stru_registro_estado.consecutivo_radicado & "   Beneficiario : " & stru_registro_estado.remitente
                If stru_registro_estado.id_tarea_workflow = 0 Then
                    h_radicado_title.Text = h_radicado_title.Text & "  Sin flujo documental seleccione nuevamente la plantilla del menú"
                End If
                Button_nuevo_radicado.Visible = True
                UpdatePanel_title_radicado.Update()
                UpdatePanel_boton_nuevo_radicado.Update()
            End If
            HiddenIdFlujo.Value = stru_registro_estado.id_tarea_workflow
            ref_UpdatePanel_boton_tool.Update()
            Dim Ruta_Web_Escaner As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString.Replace("\", "/")
            HiddenRuta.Value = Ruta_Web_Escaner
            HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = stru_registro_estado.consecutivo_radicado
            HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO") = id_registro_estado
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE"
            If stru_registro_estado.id_tarea_workflow = 0 Then
                HttpContext.Current.Session.Item("SELECCIONTEMPORAL") = id_registro_estado & "|" & "0"
            Else
                HttpContext.Current.Session.Item("SELECCIONTEMPORAL") = stru_registro_estado.id_tarea_workflow & "|" & "0"
            End If
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = ""
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = stru_registro_estado.id_tarea_workflow
            Hidden_radicado_seleccion.Value = stru_registro_estado.consecutivo_radicado
            stru_permisos_interface_envio.estado_modulo = 2
            Asignar_radicado = "YES"
        Catch ex As Exception
            Asignar_radicado = "Inconistencia general funcion Asignar_radicado " & ex.Message
        End Try

    End Function

    Function Relaciona_id_tarea_wf_estado_radicado(ByVal id_registro_estado As Long,
                                                   ByVal id_tarea_workflow As Long) As String

        Try
            Dim Update As String = "update ra_rad_estados_modulo_radicacion set id_tarea_workflow = " & id_tarea_workflow &
               " where id_estado_radicado=" & id_registro_estado
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref2.SELECTION_INSERT_COMMAND(Update)
            If Result <> "YES" Then
                Relaciona_id_tarea_wf_estado_radicado = "Funcion  Relaciona_id_tarea_wf_estado_radicado dice " & Result
                Exit Function
            Else
                Relaciona_id_tarea_wf_estado_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Relaciona_id_tarea_wf_estado_radicado = "Inconsistencia general funcion Relaciona_id_tarea_wf_estado_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_numero_radicados_pendientes(ByVal id_usuario_radicado As Integer,
                                                  ByVal id_plantilla As Integer,
                                                  ByRef numero_radicado As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el numeero de radicados pendientes
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_usuario_radicado : Representa la identificación del usuario radicador
        'id_plantilla        : Representa la identificación de la plantilla de radicación
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'numero_radicado       : Retorna el numero de radicados pendientes
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-11-16
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "select id_estado_radicado from ra_rad_estados_modulo_radicacion " &
                " where estado=1 and id_usuario_radicado=" & id_usuario_radicado &
                " and system_plantilla_radicado_id_Plantilla=" & id_plantilla
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_radicados_pendientes = "Funcion  Solicita_numero_radicados_pendientes dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_radicado = 0
                Solicita_numero_radicados_pendientes = "YES"
                Exit Function
            Else
                numero_radicado = Datset.Tables(0).Rows.Count
                Solicita_numero_radicados_pendientes = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_radicados_pendientes = "Inconistencia general funcion Solicita_numero_radicados_pendientes " & ex.Message
        End Try
    End Function
    Function Solicita_estado_radicado_asignado_usuario_gestion_documentos(ByVal id_usuario_radicado As Integer,
                                                                          ByVal id_plantilla As Integer,
                                                                          ByRef estado_asignado As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita si el usuario tiene un radicado asignado para gestion documental
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_usuario_radicado : Representa la identificación del usuario radicador
        'id_plantilla        : Representa la identificación de la plantilla de radicación
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'estado_asignado       : Retorna el estado de asignación "YES"  positivo "NO" negativo
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-21
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select id_estado_radicado from ra_rad_estados_modulo_radicacion " &
                " where estado=0 and id_usuario_radicado=" & id_usuario_radicado &
                " and system_plantilla_radicado_id_Plantilla=" & id_plantilla
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_radicado_asignado_usuario_gestion_documentos = "Funcion  Solicita_estado_radicado_asignado_usuario_gestion_documentos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_asignado = "NO"
                Solicita_estado_radicado_asignado_usuario_gestion_documentos = "YES"
                Exit Function
            Else
                estado_asignado = "YES"
                Solicita_estado_radicado_asignado_usuario_gestion_documentos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_radicado_asignado_usuario_gestion_documentos = "Inconistencia general funcion Solicita_estado_radicado_asignado_usuario_gestion_documentos " & ex.Message
        End Try
    End Function
    Function Solicita_numero_radicados_pendientes(ByVal id_usuario_radicado As Integer,
                                                  ByVal id_plantilla As Integer,
                                                  ByVal tipo_plantilla As Integer,
                                                  ByRef numero_radicado As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select id_estado_radicado from ra_rad_estados_modulo_radicacion " &
                " where estado=1 and id_usuario_radicado=" & id_usuario_radicado & " and tipo_plantilla_radicado=" & tipo_plantilla &
                " and system_plantilla_radicado_id_Plantilla=" & id_plantilla
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_radicados_pendientes = "Funcion  Solicita_numero_radicados_pendientes dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_radicado = 0
                Solicita_numero_radicados_pendientes = "YES"
                Exit Function
            Else
                numero_radicado = Datset.Tables(0).Rows.Count
                Solicita_numero_radicados_pendientes = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_radicados_pendientes = "Inconistencia general funcion Solicita_numero_radicados_pendientes " & ex.Message
        End Try
    End Function
    Function Solicitar_enviar_radicado_a_estado_pendiente(ByRef pag As Page) As String
        Try
            Dim estado_asignado As String = ""
            Dim radicado As String = ""
            Dim id_tarea_workflow As Long = 0
            Dim id_registro_estado As Long = 0
            Dim Ref_class_estados_modulos_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim Result As String = ""
            Result = Class_ra_rad_estados_modulo_radicacion.Solicita_radicado_existencia_radicado_asignado(Val(HttpContext.Current.Session("RA_ID_USUARIO")),
                                                                                                           HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"),
                                                                                                           HttpContext.Current.Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO"),
                                                                                                           estado_asignado,
                                                                                                           radicado,
                                                                                                           id_tarea_workflow,
                                                                                                           id_registro_estado)
            If Result <> "YES" Then
                Solicitar_enviar_radicado_a_estado_pendiente = Result
                Exit Function
            End If
            Dim srtru_paramenter_image_() As stru_paramter_image = Nothing
            Dim stru_permisos_interface_ As stru_permisos_interface_envio = Nothing
            If estado_asignado = "YES" Then
                Result = Ref_class_estados_modulos_radicacion.Asignar_radicado(0,
                                                                               "YES",
                                                                                1,
                                                                                pag,
                                                                                1,
                                                                                "GridView_list_documento_relacion",
                                                                                1,
                                                                                "IframeDitaliza_",
                                                                                "../workflow/WebFormEscan.aspx",
                                                                                1,
                                                                                1,
                                                                                srtru_paramenter_image_,
                                                                                stru_permisos_interface_)
                If Result <> "YES" Then
                    Solicitar_enviar_radicado_a_estado_pendiente = Result
                    Exit Function

                End If
            End If
            Solicitar_enviar_radicado_a_estado_pendiente = "YES"
            Exit Function
        Catch ex As Exception
            Solicitar_enviar_radicado_a_estado_pendiente = "Inconsistencia general función Solicitar_enviar_radicado_a_estado_pendiente " & ex.Message
        End Try
    End Function
    Function Lista_campos_radicados_pendientes(ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo BOOTSTRAF para la lista de
        '          de radicados pendientes
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        '
        '                             
        '
        ' 
        '
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Retorna la estructura de campos
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-11-16
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            item = New class_campos_table_bostra_table
            item.field = "Opciones"
            item.title = "OPCIONES"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 0
            item.clickToSelect = False
            item.visible_like_sql = 0
            item.align = "center"
            item.events = "window.operateEventsEstado"
            item.formatter = "operateFormattertablebootEstado"
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "id_estado_radicado"
            item.title = "id_estado_radicado"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "RADICADO"
            item.title = "RADICADO"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "REMITENTE"
            item.title = "REMITENTE"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "TRAMITE"
            item.title = "TRAMITE"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "FECHA"
            item.title = "FECHA"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "id_tarea_wf"
            item.title = "id_tarea_wf"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            Lista_campos_radicados_pendientes = "YES"
        Catch ex As Exception
            Lista_campos_radicados_pendientes = "Inconsistencia general funcion Lista_campos_radicados_pendientes " & ex.Message
        End Try
    End Function
    Function Solicita_row_radicados_pendientes_radicacion_table_boot(ByVal consulta As String,
                                                                     ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '          radicados pendientes
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'consulta               : Representa la consulta en comando SQL
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_row_gabinete_generic  : Retorna la estructura de datos de la consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-11-16
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("gabinete")
            Result = ref.SELECTION_SELECT_FIELDA(consulta, Datset)
            If Result <> "YES" Then
                Solicita_row_radicados_pendientes_radicacion_table_boot = "Funcion  Solicita_row_radicados_pendientes_radicacion_table_boot " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            Solicita_row_radicados_pendientes_radicacion_table_boot = "YES"
        Catch ex As Exception
            Solicita_row_radicados_pendientes_radicacion_table_boot = "Inconsistencia general fucnion Solicita_row_radicados_pendientes_radicacion_table_boot " & ex.Message
        End Try
    End Function
    Function Solicita_radicados_pendientes_radicacion(ByVal id_usuario_radicado As Integer,
                                                      ByVal id_plantilla As Integer,
                                                      ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita lista de radicados pedientes por radicación
        '          
        '         
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_usuario_radicado   : Representa la identificación del usuario de radicación
        'id_plantilla          : Representa la identificación de la plantilla de radicación
        '
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_stru_Row_Gabinete_Generic  : Retorna la estructura con los datos para 
        'llenar la tabla en la interface
        ' 
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-10-29
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Result = Lista_campos_radicados_pendientes(class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Solicita_radicados_pendientes_radicacion = Result
                Exit Function
            End If
            Dim sql_consulta As String = "SELECT id_estado_radicado,rre.consecutivo_radicado as RADICADO,rre.remitente as REMITENTE," &
                     "tde.Descripcion_Doc AS TRAMITE,rre.fecha_registro as FECHA, rre.id_tarea_workflow as id_tarea_wf " &
                     " from ra_rad_estados_modulo_radicacion AS rre " &
                     " left OUTER JOIN tipo_doc_entrante AS tde on (tde.id_Tipo_Doc_Entrante=rre.tipo_doc_entrante_id_Tipo_Doc_Entrante)" &
                     " where rre.id_usuario_radicado=" & id_usuario_radicado &
                     " and rre.system_plantilla_radicado_id_Plantilla=" & id_plantilla &
                     " and rre.estado=1 order by  id_estado_radicado desc"
            ' --------- /// Ejecuta la consulta  y retorna los row
            Result = Solicita_row_radicados_pendientes_radicacion_table_boot(sql_consulta,
                                                                             class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                Solicita_radicados_pendientes_radicacion = Result
                Exit Function
            End If
            Solicita_radicados_pendientes_radicacion = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_radicados_pendientes_radicacion = "Inconsistencia general función Solicita_radicados_pendientes_radicacion " & ex.Message
        End Try
    End Function
    Function Lista_radicados_pendientes_interface(ByVal id_usuario_radicado As Integer,
                                                  ByVal id_plantilla As Integer,
                                                  ByVal tipo_consulta As Integer,
                                                  ByVal valor_consulta As String,
                                                  ByRef colum_order_name As String,
                                                  ByRef order_colum As String,
                                                  ByRef labetitle As Label,
                                                  ByRef scripma As GridView,
                                                  ByRef hideselecion As HtmlInputHidden,
                                                  ByRef updat As UpdatePanel) As String
        Try
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT id_estado_radicado,rre.consecutivo_radicado as RADICADO,rre.remitente as REMITENTE," &
                    "tde.Descripcion_Doc AS TRAMITE,rre.fecha_registro as FECHA " &
                    " from ra_rad_estados_modulo_radicacion AS rre " &
                    " left OUTER JOIN tipo_doc_entrante AS tde on (tde.id_Tipo_Doc_Entrante=rre.tipo_doc_entrante_id_Tipo_Doc_Entrante)" &
                    " where rre.id_usuario_radicado=" & id_usuario_radicado &
                    " and rre.system_plantilla_radicado_id_Plantilla=" & id_plantilla &
                    " and rre.estado=1 order by  " & colum_order_name & " " & order_colum
            Else
                sql_consulta = "SELECT id_estado_radicado,rre.consecutivo_radicado as RADICADO,rre.remitente as REMITENTE," &
                    "tde.Descripcion_Doc AS TRAMITE,rre.fecha_registro as FECHA " &
                    " from ra_rad_estados_modulo_radicacion AS rre " &
                    " left OUTER JOIN tipo_doc_entrante AS tde on (tde.id_Tipo_Doc_Entrante=rre.tipo_doc_entrante_id_Tipo_Doc_Entrante)" &
                    " where (" &
                    "  rre.consecutivo_radicado like '%" & valor_consulta & "%'" &
                    " or rre.remitente like '%" & valor_consulta & "%'" &
                    " or rre.fecha_registro like '%" & valor_consulta & "%' )" &
                    " and rre.id_usuario_radicado=" & id_usuario_radicado &
                    " and rre.system_plantilla_radicado_id_Plantilla=" & id_plantilla &
                    " and rre.estado=1 order by  " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido") = {"OPCIONES", "id_estado_radicado",
                                                                               "RADICADO", "REMITENTE",
                                                                               "TRAMITE", "FECHA"}
            HttpContext.Current.Session.Item("SortExpression_publico") = colum_order_name
            HttpContext.Current.Session.Item("SortDirection_publico") = order_colum
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_PUBLICO") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO") = sql_consulta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_radicados_pendientes_interface = "Error fucion Lista_radicados_pendientes  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) "
                scripma.DataSource = Datset
                hideselecion.Value = "-1"
                scripma.DataBind()
                updat.Update()
                Lista_radicados_pendientes_interface = "YES"
                Exit Function
            Else
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s)  "
                scripma.DataSource = Datset
                hideselecion.Value = "-1"
                scripma.DataBind()
                updat.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-arrow-to-bottom")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Asigna radicado")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "a_s_r_p_333")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_compartido"),
                                                            order_colum,
                                                            scripma)
                If Result <> "YES" Then
                    Lista_radicados_pendientes_interface = "Error add clase funcion  Lista_radicados_pendientes_interface " & Result
                    Exit Function
                End If
            End If
            Lista_radicados_pendientes_interface = "YES"
        Catch ex As Exception
            Lista_radicados_pendientes_interface = "Inconsistencia general función Lista_radicados_pendientes_interface " & ex.Message
        End Try
    End Function

End Class
