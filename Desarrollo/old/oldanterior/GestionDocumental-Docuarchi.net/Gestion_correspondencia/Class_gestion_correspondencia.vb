Public Class Class_gestion_correspondencia
    Function Pre_reasigna_tarea_gestion_correspondencia(ByVal id_tarea As Long,
                                                        ByVal id_usuario_workflow As Integer,
                                                        ByVal id_flujo_trabajo As Integer,
                                                        ByVal id_actividad_flujo_trabajo As Integer,
                                                        ByVal id_usuario_workflow_flujo_trabajo As Integer,
                                                        ByVal usuario_autoriza As String,
                                                        ByRef resultado_correo As String) As String
        Try
            If id_usuario_workflow = -1 Then
                Pre_reasigna_tarea_gestion_correspondencia = "Debe informar el usuario la cual se le asignara el trámite"
                Exit Function
            End If
            Dim Result As String = ""
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(id_tarea,
                                                                                         Estado_solicitud_aprobacion,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Pre_reasigna_tarea_gestion_correspondencia = Result
                Exit Function
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Pre_reasigna_tarea_gestion_correspondencia = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                Exit Function
            End If
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea,
                                                                                    Radicado)
            If Result <> "YES" Then
                Pre_reasigna_tarea_gestion_correspondencia = Result
                Exit Function
            End If
            If Radicado = "" Then
                Pre_reasigna_tarea_gestion_correspondencia = "La tarea seleccionada no tiene radicado relacionado"
                Exit Function
            End If
            Dim Refclas_gestion_respuesta As New Classgestionrespuesta
            Dim struc_envio As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim id_respuesta As Integer = 0
            Result = ref_ra_resp_radic.Retorna_id_respuesta_radicado(Radicado,
                                                                     HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                     id_respuesta)


            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                       struc_envio)
            If Result <> "YES" Then
                Pre_reasigna_tarea_gestion_correspondencia = Result
                Exit Function
            End If
            If struc_envio.RADICADO Is Nothing Then
                Pre_reasigna_tarea_gestion_correspondencia = "El radicado " & Radicado & " no tiene una respuesta relacionada"
                Exit Function
            End If

            If struc_envio.FECHA_RESPUETA <> "" Then
                Pre_reasigna_tarea_gestion_correspondencia = "El tramite ya se encuentra con una respuesta imposible reasignar "
                Exit Function
            End If
            Dim refclas_gestino_resp As New Classgestionrespuesta
            Dim id_actividad_usuario_workflow As Integer = 0
            Dim id_grupo_usuario_workflow As Integer = 0
            Dim ref_class_list As New Class_Listado_Actividades_workflow
            Dim Refclass_usuario_workflow As New ClassWorkflowUsuario
            Result = Refclass_usuario_workflow.Solicita_id_grupo_usuario_workflow(id_usuario_workflow,
                                                                                  id_grupo_usuario_workflow)
            If Result <> "YES" Then
                Pre_reasigna_tarea_gestion_correspondencia = Result
                Exit Function
            End If
            Result = ref_class_list.Retorna_actividad_grupo_workflow(id_grupo_usuario_workflow,
                                                                    id_actividad_usuario_workflow,
                                                                    "")
            If Result <> "YES" Then
                Pre_reasigna_tarea_gestion_correspondencia = Result
                Exit Function
            End If
            Result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario_batch(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       id_tarea,
                                                                                       id_usuario_workflow,
                                                                                       id_actividad_usuario_workflow,
                                                                                       id_usuario_workflow,
                                                                                       0,
                                                                                       "",
                                                                                       resultado_correo,
                                                                                       usuario_autoriza,
                                                                                       id_flujo_trabajo,
                                                                                       id_actividad_flujo_trabajo,
                                                                                       id_usuario_workflow_flujo_trabajo,
                                                                                       0)
            If Result <> "YES" Then
                Pre_reasigna_tarea_gestion_correspondencia = Result
                Exit Function
            Else
                Pre_reasigna_tarea_gestion_correspondencia = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Pre_reasigna_tarea_gestion_correspondencia = "Inconsistencia general función Pre_reasigna_tarea_gestion_correspondencia " & ex.Message
        End Try
    End Function
    Function Seleccion_menu_tramite(ByVal Value_sel As String, _
                                    ByRef pag As Page, _
                                    ByVal id_tarea_selecion As Object) As String
        Try
            Dim Result As String = ""
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            If Value_sel = "R-R-D" Then
                If _
                    id_tarea_selecion = "0" Or id_tarea_selecion = "-1" Then
                    Seleccion_menu_tramite = "Debe seleccionar el tramite para reversar la respuesta"
                    Exit Function
                End If
                Dim TextBox_login_usuario_val As TextBox = pag.FindControl("TextBox_login_usuario_val")
                Dim TextBox_pasw_usuario_val As TextBox = pag.FindControl("TextBox_pasw_usuario_val")
                Dim UpdatePanel_contenido_radica_documento As UpdatePanel = pag.FindControl("UpdatePanel_contenido_radica_documento")
                Dim ModalPopupExtender_edition_reversa_respuesta As AjaxControlToolkit.ModalPopupExtender = _
                    pag.FindControl("ModalPopupExtender_edition_reversa_respuesta")
                Dim ModalPopupExtender_edition_confirma_reversa_respuesta As AjaxControlToolkit.ModalPopupExtender = _
                    pag.FindControl("ModalPopupExtender_edition_confirma_reversa_respuesta")
                If HttpContext.Current.Session.Item("REVERSA_RESPUESTA") = 0 Then
                    TextBox_login_usuario_val.Text = ""
                    TextBox_pasw_usuario_val.Text = ""
                    If (Not UpdatePanel_contenido_radica_documento Is Nothing) Then
                        UpdatePanel_contenido_radica_documento.Update()
                    End If
                    ModalPopupExtender_edition_reversa_respuesta.Show()
                Else
                    ModalPopupExtender_edition_confirma_reversa_respuesta.Show()
                End If
            End If
            If Value_sel = "R-T-D" Then
                Dim TextBox_login_autoriza_reasigna As TextBox = pag.FindControl("TextBox_login_autoriza_reasigna")
                Dim TextBox_pasw_autoriza_reasigna As TextBox = pag.FindControl("TextBox_pasw_autoriza_reasigna")
                Dim UpdatePanel_contenido_reasigna_responsable_tramite As UpdatePanel = pag.FindControl("UpdatePanel_contenido_reasigna_responsable_tramite")
                Dim ModalPopupExtender_edition_reasigna_responsable_tramite As  _
                     AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_reasigna_responsable_tramite")

                Dim ModalPopupExtender_edition_confirma_reasigna_responsable_tramite As  _
                     AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_confirma_reasigna_responsable_tramite")
                If _
                    id_tarea_selecion = "0" Or id_tarea_selecion = "-1" Then
                    Seleccion_menu_tramite = "Debe seleccionar el tramite para reasignar"
                    Exit Function
                End If
                If HttpContext.Current.Session.Item("REASIGNA_RESPUESTA_TRAMITE") = 0 Then
                    Seleccion_menu_tramite = "El usuario no tiene permiso para reasignar el trámite"
                    Exit Function
                End If
                If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW") = 0 Then
                    TextBox_login_autoriza_reasigna.Text = ""
                    TextBox_pasw_autoriza_reasigna.Text = ""
                    UpdatePanel_contenido_reasigna_responsable_tramite.Update()
                    ModalPopupExtender_edition_reasigna_responsable_tramite.Show()
                    Seleccion_menu_tramite = "YES"
                    Exit Function
                Else
                    ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Show()
                    Seleccion_menu_tramite = "YES"
                    Exit Function
                End If
            End If
            '----------------------------------------------
            'Muestra detalle de respuesta
            '----------------------------------------------    
            Dim Iframe_visor_externo As Object = pag.FindControl("Iframe_visor_externo__")
            Dim UpdatePanel_detalle_respuesta As UpdatePanel = pag.FindControl("UpdatePanel_detalle_respuesta")
            Dim ModalPopupExtender_detalle_respuesta As AjaxControlToolkit.ModalPopupExtender _
                = pag.FindControl("ModalPopupExtender_detalle_respuesta")
            If Value_sel = "D-RT" Then
                If id_tarea_selecion = "0" Or id_tarea_selecion = "-1" Then
                    Seleccion_menu_tramite = "No hay tarea seleccionada imposible mostrar detalle"
                    Exit Function
                End If
                Dim Radicado As String = ""
                Dim Refclas As New ClassWorkflow
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecion, _
                                                                                     Radicado)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                End If
                Dim estru As stru_envio = Nothing
                'Dim refclas_resp As New Classgestionrespuesta

                Dim id_respuesta As Integer = 0
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                   id_respuesta)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                End If
                If id_respuesta = 0 Then
                    Dim Resulta = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                                   id_respuesta)
                    If Resulta <> "YES" Then
                        Seleccion_menu_tramite = Result
                        Exit Function
                    End If
                    If id_respuesta = 0 Then
                        Seleccion_menu_tramite = Result
                        Exit Function
                    End If

                    Resulta = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, estru)
                    If Resulta <> "YES" Then
                        Seleccion_menu_tramite = Result
                        Exit Function
                    Else
                        HttpContext.Current.Session.Item("PU_TRAZABILIDAD") = estru.RADICADO
                        Iframe_visor_externo.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
                        UpdatePanel_detalle_respuesta.Update()
                        ModalPopupExtender_detalle_respuesta.Show()
                    End If
                Else
                    HttpContext.Current.Session.Item("PU_TRAZABILIDAD") = Radicado
                    Iframe_visor_externo.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
                    UpdatePanel_detalle_respuesta.Update()
                    ModalPopupExtender_detalle_respuesta.Show()
                End If

            End If
            '------------------------------------------------------------
            'Muestra log de la respuesta del radicado
            '-------------------------------------------------------------   

            If Value_sel = "T-DT" Then
                Dim Iframe_transacciones As Object = pag.FindControl("Iframe_transacciones_")
                Dim UpdatePanel_transacciones As UpdatePanel = pag.FindControl("UpdatePanel_transacciones")
                Dim ModalPopupExtender_transacciones As AjaxControlToolkit.ModalPopupExtender = _
                    pag.FindControl("ModalPopupExtender_transacciones")
                If id_tarea_selecion = "0" Or id_tarea_selecion = "-1" Then
                    Seleccion_menu_tramite = "No hay tarea seleccionada imposible mostrar detalle"
                    Exit Function
                End If
                Dim Refclas As New ClassWorkflow
                Dim Radicado As String = ""
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecion, _
                                                                                     Radicado)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                End If

                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                Dim id_respuesta As Integer = 0
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                    id_respuesta)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                End If
                If id_respuesta = 0 Then
                    Dim Resulta = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                                   id_respuesta)
                    If Resulta <> "YES" Then
                        Seleccion_menu_tramite = Result
                        Exit Function
                    End If
                    If id_respuesta = 0 Then
                        Seleccion_menu_tramite = "El radicado actual no tiene una respuesta relacionada"
                        Exit Function
                    End If

                    Resulta = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                                 estru)
                    If Resulta <> "YES" Then
                        Seleccion_menu_tramite = Result
                        Exit Function
                    Else
                        HttpContext.Current.Session.Item("PU_TRAZABILIDAD") = id_respuesta
                        Iframe_visor_externo.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
                        UpdatePanel_detalle_respuesta.Update()
                        ModalPopupExtender_detalle_respuesta.Show()
                    End If
                Else
                    HttpContext.Current.Session.Item("PU_TRAZABILIDAD") = id_respuesta
                    Iframe_transacciones.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
                    UpdatePanel_transacciones.Update()
                    ModalPopupExtender_transacciones.Show()
                End If
            End If
            '------------------------------------------------------------
            'Muestra log de la respuesta del radicado ventana historial
            '-------------------------------------------------------------        
            If Value_sel = "T-DT-H" Then
                Dim Iframe_transacciones As Object = pag.FindControl("Iframe_transacciones_historial_")
                Dim UpdatePanel_transacciones As UpdatePanel = pag.FindControl("UpdatePanel_transacciones")
                Dim ModalPopupExtender_transacciones As AjaxControlToolkit.ModalPopupExtender = _
                    pag.FindControl("ModalPopupExtender_transacciones")
                If id_tarea_selecion = "0" Or id_tarea_selecion = "-1" Then
                    Seleccion_menu_tramite = "No hay tarea seleccionada imposible mostrar log"
                    Exit Function
                End If
                Dim Refclas As New ClassWorkflow
                Dim Radicado As String = ""
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecion, _
                                                                                     Radicado)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                End If

                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                Dim id_respuesta As Integer = 0
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                   id_respuesta)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                End If
                If id_respuesta = 0 Then
                    Dim Resulta = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                    id_respuesta)
                    If Resulta <> "YES" Then
                        Seleccion_menu_tramite = Result
                        Exit Function
                    End If
                    If id_respuesta = 0 Then
                        Seleccion_menu_tramite = "El radicado actual no tiene una respuesta relacionada"
                        Exit Function
                    End If

                    Resulta = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                                 estru)
                    If Resulta <> "YES" Then
                        Seleccion_menu_tramite = Result
                        Exit Function
                    Else
                        HttpContext.Current.Session.Item("PU_TRAZABILIDAD") = id_respuesta
                        Iframe_visor_externo.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
                        UpdatePanel_detalle_respuesta.Update()
                        ModalPopupExtender_detalle_respuesta.Show()
                    End If
                Else
                    HttpContext.Current.Session.Item("PU_TRAZABILIDAD") = id_respuesta
                    Iframe_transacciones.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
                    UpdatePanel_transacciones.Update()
                    ModalPopupExtender_transacciones.Show()
                End If
            End If
            '-------------------------------------------------
            'Muestra la trazabilidad del documento workflow
            '-------------------------------------------------       
            If Value_sel = "G-TDW" Then
                Dim Iframe_trazabilidad As Object = pag.FindControl("Iframe_trazabilidad_")
                Dim UpdatePanel_trazabilidad As UpdatePanel = pag.FindControl("UpdatePanel_trazabilidad")
                Dim ModalPopupExtender_trazabilidad As AjaxControlToolkit.ModalPopupExtender = _
                    pag.FindControl("ModalPopupExtender_trazabilidad")
                If id_tarea_selecion = "0" Or id_tarea_selecion = "-1" Then
                    Seleccion_menu_tramite = "No hay tarea seleccionada imposible mostrar detalle "
                    Exit Function
                End If
                Dim Refclas As New ClassWorkflow
                Dim Radicado As String = ""
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecion, _
                                                                                     Radicado)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                End If
                HttpContext.Current.Session.Item("PU_TRAZABILIDAD") = Radicado
                Iframe_trazabilidad.Attributes("SRC") = "../workflow/WebFormTrazabilidadWorkflow.aspx"
                UpdatePanel_trazabilidad.Update()
                ModalPopupExtender_trazabilidad.Show()
            End If
            '---------------------------------------------------
            'Lista solictudes documento de aprobación
            '---------------------------------------------------
            If Value_sel = "S-A-R" Then
                If _
                    id_tarea_selecion = "0" Or id_tarea_selecion = "-1" Then
                    Seleccion_menu_tramite = "Debe seleccionar el tramite para listar las solicitudes de aprobación"
                    Exit Function
                End If
                Dim Iframe_solicitud_aprobacion As Object = pag.FindControl("Iframe_solicitud_aprobacion")
                Dim UpdatePanel_solicitud_aprobacion As Object = pag.FindControl("UpdatePanel_solicitud_aprobacion")
                Dim ModalPopupExtender_solicitud_aprobacion As AjaxControlToolkit.ModalPopupExtender = _
                    pag.FindControl("ModalPopupExtender_solicitud_aprobacion")
                Dim Refclas As New ClassWorkflow
                Dim Radicado As String = ""
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecion,
                                                                                        Radicado)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                End If

                Dim estru As stru_envio = Nothing
                'Dim refclas_resp As New Classgestionrespuesta
                Dim id_respuesta As Integer = 0
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                    id_respuesta)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                End If
                HttpContext.Current.Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION") = id_respuesta
                Iframe_solicitud_aprobacion.Attributes.Add("src", "../radicador/WebFormRaGestionSolicitudesAprobacion.aspx")
                UpdatePanel_solicitud_aprobacion.Update()
                ModalPopupExtender_solicitud_aprobacion.Show()
            End If
            '-----------------------------------------------
            'Consulta historico tramites
            '-----------------------------------------------
            If Value_sel = "B-HT" Then
                Dim Iframe_historico_tramite_ As Object = pag.FindControl("Iframe_historico_tramite_")
                Dim UpdatePanel_historico_tramite As UpdatePanel = pag.FindControl("UpdatePanel_historico_tramite")
                Dim ModalPopupExtender_historico_tramite As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_historico_tramite")
                Iframe_historico_tramite_.Attributes("SRC") = "../gestion_correspondencia/WebForm_interface_consulta_historico.aspx"
                UpdatePanel_historico_tramite.Update()
                ModalPopupExtender_historico_tramite.Show()
            End If
            If Value_sel = "R-P-I" Then
                Dim Hidden_id_respuesta As Object = pag.FindControl("Hidden_id_respuesta")
                Dim TextBox_dext_externo As TextBox = pag.FindControl("TextBox_dext_externo")
                Dim UpdatePanel_dest_externo As UpdatePanel = pag.FindControl("UpdatePanel_dest_externo")
                Dim ModalPopupExtender_edition_asigna_dest_externo As AjaxControlToolkit.ModalPopupExtender =
                    pag.FindControl("ModalPopupExtender_edition_asigna_dest_externo")
                Dim estru As stru_envio = Nothing
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Hidden_id_respuesta.Value,
                                                                                            estru)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                Else
                    TextBox_dext_externo.Text = estru.DESTINATARIO
                    UpdatePanel_dest_externo.Update()
                    ModalPopupExtender_edition_asigna_dest_externo.Show()
                End If
            End If
            If Value_sel = "N-R-C" Then
                Dim correo_electronico As String = ""
                Dim refclasradicado As New ClassRadicador
                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                Dim ModalPopupExtender_edition_notifica_correo_respuesta As AjaxControlToolkit.ModalPopupExtender =
                    pag.FindControl("ModalPopupExtender_edition_notifica_correo_respuesta")

                ModalPopupExtender_edition_notifica_correo_respuesta.Show()
            End If
            '---------------------------------------------------------------
            'Lista solicitudes de aprobación respuesta
            '---------------------------------------------------------------
            If Value_sel = "S-A-R-G-RD" Then
                Dim UpdatePanel_solicitud_aprobacion As UpdatePanel = pag.FindControl("UpdatePanel_solicitud_aprobacion")
                Dim ModalPopupExtender_solicitud_aprobacion As AjaxControlToolkit.ModalPopupExtender =
                    pag.FindControl("ModalPopupExtender_solicitud_aprobacion")
                Dim Iframe_solicitud_aprobacion As Object = pag.FindControl("Iframe_solicitud_aprobacion")
                Iframe_solicitud_aprobacion.Attributes.Add("src", "../radicador/WebFormRaGestionSolicitudesAprobacion.aspx")
                UpdatePanel_solicitud_aprobacion.Update()
                ModalPopupExtender_solicitud_aprobacion.Show()
            End If
            If Value_sel = "D-D-R-R" Then
                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                Dim Iframe_visor_externo_ As Object = pag.FindControl("Iframe_visor_externo__")
                Dim UpdatePanel_detalle_respuesta_ As UpdatePanel = pag.FindControl("UpdatePanel_detalle_respuesta")
                Dim ModalPopupExtender_detalle_respuesta_ As Object = pag.FindControl("ModalPopupExtender_detalle_respuesta")
                Dim Hidden_id_respuesta As Object = pag.FindControl("Hidden_id_respuesta")
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Hidden_id_respuesta.Value, estru)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("PU_TRAZABILIDAD") = estru.RADICADO
                    Iframe_visor_externo_.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
                    UpdatePanel_detalle_respuesta_.Update()
                    ModalPopupExtender_detalle_respuesta_.Show()
                End If

            End If
            If Value_sel = "D-V-D-T" Then
                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                Dim Iframe_transacciones As Object = pag.FindControl("Iframe_transacciones_")
                Dim Hidden_id_respuesta As Object = pag.FindControl("Hidden_id_respuesta")
                Dim UpdatePanel_transacciones As Object = pag.FindControl("UpdatePanel_transacciones")
                Dim ModalPopupExtender_transacciones As Object = pag.FindControl("ModalPopupExtender_transacciones")
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Hidden_id_respuesta.Value, estru)
                If Result <> "YES" Then
                    Seleccion_menu_tramite = Result
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("PU_TRAZABILIDAD") = Hidden_id_respuesta.Value
                    Iframe_transacciones.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
                    UpdatePanel_transacciones.Update()
                    ModalPopupExtender_transacciones.Show()
                End If
            End If
            Seleccion_menu_tramite = "YES"
            Exit Function
        Catch ex As Exception
            Seleccion_menu_tramite = "Incinsistencia general función Seleccion_menu_tramite " & ex.Message
        End Try
    End Function
End Class
