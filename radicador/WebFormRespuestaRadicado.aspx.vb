Imports System.IO
Imports GemBox.Document
Imports GemBox.Document.Tables
Imports System.Xml

Public Class WebFormRespuestaRadicado
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim clasjava As New Classscrripjava
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            If Me.IsPostBack = False And AjaxFileUpload_dowload.IsInFileUploadPostBack = False Then
                Me.DropDownList_prioridad_solicitud.Items.Add("Normal")
                Me.DropDownList_prioridad_solicitud.Items.Add("Urgente")
                ComponentInfo.SetLicense("DTFX-JTBY-6RJK-Y101")
                Dim Result As String = ""
                Dim Refclas As New ClassWorkflow
                Dim Radicado As String = ""
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Radicado)
                If Result <> "YES" Then
                    Exit Sub
                End If
                If Radicado = "" Then
                    label_result.Text = "La tarea seleccionada no tiene radicado relacionado "
                    Exit Sub
                End If
                Dim refclas_resp As New Classgestionrespuesta
                Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
                Dim id_respuesta As Integer = 0
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                   id_respuesta)
                If Result <> "YES" Then
                    label_result.Text = Result
                    Exit Sub
                End If
                Dim Refclas_gestion_resp As New Classgestionrespuesta
                Result = Refclas_gestion_resp.Actualiza_ruta_workflow_respuesta_radicado(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                         Radicado)
                If Result <> "YES" Then
                    label_result.Text = label_result.Text & Result
                    Exit Sub
                End If
                If id_respuesta = 0 Then
                    label_result.Text = "El radicado actual no tiene una respuesta relacionada"
                    Exit Sub
                End If
                Dim estru As stru_envio = Nothing
                Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                            estru)
                If Result <> "YES" Then
                    label_result.Text = Result
                    Exit Sub
                End If
                Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION") = id_respuesta
                label_title.Text = "Respuesta al radicado (" & Radicado & ") código (" & id_respuesta & ") Peticionario (" & estru.DESTINATARIO & ") Tramite (" & estru.TRAMITE_DOCUMENTO & ")"
                Me.Hidden_radicado.Value = Radicado
                Me.Hidden_id_respuesta.Value = id_respuesta
                Me.Hidden_id_propietario_resp.Value = estru.ID_REMIT_DEST_INT
                Dim nombre_plantilla As String = ""
                Result = refclas_resp.Retorna_nombre_plantilla_por_id_respuesta(id_respuesta,
                                                                                nombre_plantilla,
                                                                                "")
                If Result <> "YES" Then
                    label_result.Text = Result
                    'Exit Sub
                Else
                    Dim estado_obligatorio As Integer = 0
                    Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
                    Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
                    Dim id_tipo_tramite As Integer = 0
                    Dim descripcion_tramite As String = ""
                    Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla,
                                                                                                         Radicado,
                                                                                                         id_tipo_tramite,
                                                                                                         descripcion_tramite)
                    If Result <> "YES" Then
                        label_result.Text = label_result.Text & Result
                        Exit Sub
                    End If
                    Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                                     estado_obligatorio)
                    If Result <> "YES" Then
                        label_result.Text = label_result.Text & Result
                        Exit Sub
                    End If
                    Me.Hidden_obliga_rep.Value = estado_obligatorio
                    'If estado_obligatorio = 1 Then
                    '    label_text_title.Text = "(1) Elaborar una respuesta a la petición o solicitud (Opción recomendada por el sistema, el trámite " & estru.TRAMITE_DOCUMENTO & " es de obligatoria respuesta)"
                    '    label_text_title.ForeColor = System.Drawing.Color.Green
                    '    'label_text_confirma.Font.Size = 20
                    '    'label_text_confirma.Font.Bold = True
                    '    label_text_confirma.Attributes.Add("font-size", "15px")
                    '    Me.Image_formal_visto.Visible = True
                    'Else
                    '    label_text_confirma.Text = "(2) Solo confirmar o darse por enterado (Opción recomendada por el sistema, el trámite " & estru.TRAMITE_DOCUMENTO & " no es de obligatoria respuesta)"
                    '    label_text_confirma.ForeColor = System.Drawing.Color.Green
                    '    'label_text_confirma.Font.Size = 20
                    '    'label_text_confirma.Font.Bold = True
                    '    label_text_confirma.Attributes.Add("font-size", "15px")
                    '    Me.Image_formal_visto_simple.Visible = True

                    'End If
                    Dim estado_envio As Integer = -1
                    Result = refclas_resp.Retorna_estado_envio_respuesta(nombre_plantilla,
                                                                         Radicado,
                                                                         estado_envio)
                    If Result <> "YES" Then
                        label_result.Text = Result
                    Else
                        Dim descripcion_estado_obligatorio As String = ""
                        If estado_obligatorio = 1 Then
                            descripcion_estado_obligatorio = ", el trámite requiere de una respuesta"
                        Else
                            descripcion_estado_obligatorio = ", el trámite requiere solo una confirmación"
                        End If
                        Hidden_tipo_respuesta.Value = estado_envio
                        If estado_envio = 1 Then
                            label_result.Text = "Tipo de contestacíon correo electrónico" & descripcion_estado_obligatorio
                        Else
                            label_result.Text = "Tipo de contestacíon correo físico" & descripcion_estado_obligatorio
                        End If
                    End If
                End If
                If Directory.Exists(Server.MapPath("../Temp_Image/" & "/adjuntos_respuesta/" & Hidden_id_respuesta.Value & "/")) = False Then
                    Directory.CreateDirectory(Server.MapPath("../Temp_Image/" & "/adjuntos_respuesta/" & Hidden_id_respuesta.Value & "/"))
                End If
                Dim Ref_clas_anexos As New Class_ra_anexos_respuesta
                Result = Ref_clas_anexos.listar_anexos_respuesta_solicitud(Hidden_id_respuesta.Value,
                                                                          Me.DropDownList_anexos_respuesta,
                                                                          UpdatePanel_anexos_respuesta,
                                                                          DropDownList_anexos_respuesta_simple,
                                                                          UpdatePanel_anexos_respuesta_simple)
                If Result <> "YES" Then
                    label_result.Text = "Inconsistencia " & Result
                    Exit Sub
                End If
                Dim correo_electronico As String = ""
                Dim refclasradicado As New ClassRadicador
                Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(estru.codigo_dest_externo,
                                                                                           correo_electronico,
                                                                                           estru.system_plantilla_radicado_id_plantilla)
                If Result <> "YES" Then
                    label_result.Text = "Inconsistencia " & Result
                    Exit Sub
                End If
                Me.Hidden_remitente_destinatario.Value = estru.codigo_dest_externo
                Result = Refclas_gestion_resp.Actualiza_estados_general_semaforo(id_respuesta,
                                                                                 Me.Page)
                If Result <> "YES" Then
                    label_result.Text = "Inconsistencia " & Result
                    Exit Sub
                End If
            Else

            End If
        Catch ex As Exception
            label_result.Text = ex.Message
        End Try
    End Sub
    Protected Sub Button_me_active_men_dive_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Me.Hidden_menu_var_event_dive.Value = "" Then
                Exit Sub
            End If
            '-------------------------------------------
            'Reversa respuesta
            '-------------------------------------------
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim Valselect As String = Me.Hidden_menu_var_event_dive.Value
            If Valselect = "R-R-D" Then
                If HttpContext.Current.Session.Item("REVERSA_RESPUESTA") = 0 Then
                    Me.TextBox_login_usuario_val.Text = ""
                    Me.TextBox_pasw_usuario_val.Text = ""
                    Me.UpdatePanel_contenido_radica_documento.Update()
                    Me.ModalPopupExtender_edition_reversa_respuesta.Show()
                Else
                    Me.ModalPopupExtender_edition_confirma_reversa_respuesta.Show()
                End If
            End If
            If Valselect = "R-P-I" Then
                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                Dim Result As String = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, estru)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.TextBox_dext_externo.Text = estru.DESTINATARIO
                    UpdatePanel_dest_externo.Update()
                    ModalPopupExtender_edition_asigna_dest_externo.Show()
                End If
            End If
            If Valselect = "D-D-R-R" Then
                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                Dim Result As String = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, estru)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("PU_TRAZABILIDAD") = estru.RADICADO
                    Me.Iframe_visor_externo_.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
                    Me.UpdatePanel_detalle_respuesta.Update()
                    Me.ModalPopupExtender_detalle_respuesta.Show()
                End If

            End If
            If Valselect = "E-N-R" Then
                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                Dim Result As String = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, estru)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("PU_TRAZABILIDAD") = estru.RADICADO
                    Me.TextBox_NOTA_RESPUESTA.Text = estru.NOTA_RESPUESTA
                    Me.UpdatePanel_nota_respuesta.Update()
                    Me.ModalPopupExtender_nota_respuesta.Show()
                End If
            End If
            If Valselect = "N-R-C" Then
                Dim correo_electronico As String = ""
                Dim refclasradicado As New ClassRadicador
                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                'Dim Result As String = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, _
                '                                                                                          estru)
                'Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(estru.codigo_dest_externo, _
                '                                                                           correo_electronico, _
                '                                                                           estru.system_plantilla_radicado_id_plantilla)
                'If Result <> "YES" Then
                '    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                '    Exit Sub
                'End If
                'TextBox_correo_electronico.Text = correo_electronico
                'Me.UpdatePanel_notifica_correo.Update()
                ModalPopupExtender_edition_notifica_correo_respuesta.Show()
            End If
            If Valselect = "D-V-D-T" Then
                Dim estru As stru_envio = Nothing
                Dim refclas_resp As New Classgestionrespuesta
                Dim Result As String = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, estru)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("PU_TRAZABILIDAD") = Me.Hidden_id_respuesta.Value
                    Me.Iframe_transacciones_.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
                    Me.UpdatePanel_transacciones.Update()
                    Me.ModalPopupExtender_transacciones.Show()
                End If
            End If
            '-------------------------------------------------------
            'Guarda documento respuesta en formato no modificable
            '-------------------------------------------------------
            If Valselect = "G-R-F" Then
                Dim Refclas As New ClassRadicador
                Dim Refclasgestion As New Classgestionrespuesta
                If Hidden_id_respuesta.Value = "-1" Then
                    Refclasjava.Showscripman_menu("El trámite actual no tiene asignada una respuesta ", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Me.Hidden_id_propietario_resp.Value <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                    Refclasjava.Showscripman_menu("El usuario de gestión no es el autorizado para gestionar la respuesta ", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim id_imagen_plantilla As Integer = 0
                Dim radicado_respuesta As Integer = 0
                Dim fecha_respuesta As Integer = 0
                Dim id_imagen_respuesta As Integer = 0
                Dim estado_envio_respuesta As Integer = 0
                Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
                Dim Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Hidden_id_respuesta.Value, _
                                                                                        id_imagen_plantilla, _
                                                                                        radicado_respuesta, _
                                                                                        fecha_respuesta, _
                                                                                        id_imagen_respuesta, _
                                                                                        estado_envio_respuesta)
                If id_imagen_respuesta <> 0 Then
                    Refclasjava.Showscripman_menu("El trámite ya tiene una respuesta públicada, imposible guardar y públicar el documento de respuesta ", _
                                                  Me.UpdatePanel_menu_var_event, _
                                                  "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                'Result = Refclasgestion.Almacena_documento_respuesta_permanente(Hidden_id_respuesta.Value)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclasgestion.Actualiza_estados_general_semaforo(Hidden_id_respuesta.Value, Me.Page)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            '---------------------------------------------------------
            'Comparte documentos para solicitud de colaboración
            '---------------------------------------------------------
            If Valselect = "S-C-R-G-RD" Then
                'If Session.Item("ID_TAREA_SELECCIONDA") = 0 Or Session.Item("ID_TAREA_SELECCIONDA") = -1 Then
                '    Refclasjava.Showscripman_menu("El sistema no detecto tareas seleccionada para compartir documentos", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                '    Exit Sub
                'Else
                '    Dim Refclas As New ClassWorkflow
                '    Dim Result = Refclas.Lista_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), Session.Item("GA_STRU_DOCUMENTO_RADICADO"))
                '    If Result <> "YES" Then
                '        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                '        Exit Sub
                '    End If
                '    Dim refclas_seleccion As New Classselecciotarea
                '    Result = refclas_seleccion.Retorna_Matriz_imagenes_relacionadas_a_tarea(Session.Item("ID_TAREA_SELECCIONDA"))
                '    If Result <> "YES" Then
                '        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                '        Exit Sub
                '    End If
                '    Session.Item("GA_STRU_DOCUMENTO_TIPO_COMPARTIDO") = "COMPARTIR RESPUESTA"
                '    Me.Iframe_compartir_documento_.Attributes("SRC") = "../gestion/WebFormGaCompartirDocumento.aspx"
                '    Me.UpdatePanel_autoriza_compartir_documento.Update()
                '    Me.ModalPopupExtender_edition_autoriza_compartir_documento.Show()
                'End If
                Dim Result As String = ""
                Dim refclas As New ClassRaSolicitudesAprobacion
                Result = refclas.Verfica_viabilidad_solicitud_aprobacion_respuesta(Val(Hidden_id_respuesta.Value), _
                                                                                   Me.TextBox_fecha_limite_solicitud.Text)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                UpdatePanel_registro_solicitud.Update()
                ModalPopupExtender_actualizacion_anualidad.Show()
            End If
            '-------------------------------------------------------------
            'Lista documentos de respuestas de solicitudes de colaboración
            '-------------------------------------------------------------
            If Valselect = "L-R-S-G-RD" Then
                Dim Result As String = ""
                Dim struc_envio As stru_envio = Nothing
                Dim refclasgestion As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, struc_envio)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                HttpContext.Current.Session.Item("GA_STRU_RADICADO_COLABORACION") = struc_envio.RADICADO
                Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "RELACIONADO A RADICADO"
                Me.Iframe_registro_colaboracion_.Attributes.Add("src", "../gestion/WebFormGaListaDocumentosColaboracion.aspx")
                Me.UpdatePanel_registro_colaboracion.Update()
                Me.ModalPopupExtender_edition_registro_colaboracion.Show()
            End If
            '---------------------------------------------------------------
            'Lista solicitudes de aprobación respuesta
            '---------------------------------------------------------------
            If Valselect = "S-A-R-G-RD" Then
                Me.Iframe_solicitud_aprobacion.Attributes.Add("src", "../radicador/WebFormRaGestionSolicitudesAprobacion.aspx")
                Me.UpdatePanel_solicitud_aprobacion.Update()
                Me.ModalPopupExtender_solicitud_aprobacion.Show()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, UpdatePanel_menu_var_event)
        End Try
    End Sub
   
    

    Protected Sub Button_reversar_Click(sender As Object, e As EventArgs) Handles Button_reversar.Click
        'UpdatePanel_contenido_radica_documento
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Refclas As New Classgestionrespuesta
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Me.Hidden_id_respuesta.Value, _
                                                                                id_imagen_plantilla, _
                                                                                radicado_respuesta, _
                                                                                fecha_respuesta, _
                                                                                id_imagen_respuesta, _
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_contenido_radica_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'If fecha_respuesta = 0 And radicado_respuesta = 0 Then
            '    clasjava.Showscripman_menu("La respuesta a reversar aun no tiene respuesta ", Me.UpdatePanel_contenido_radica_documento, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim id_usuario_autoriza As Integer = -1
            Result = Refclas.Valida_usuario_administrador_general(Me.TextBox_login_usuario_val.Text, _
                                                                  Me.TextBox_pasw_usuario_val.Text, _
                                                                  id_usuario_autoriza, _
                                                                  "reversa_respuesta")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_contenido_radica_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, stru, 1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_contenido_radica_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Reversa_respuesta_radicado(stru, _
                                                        Me.TextBox_login_usuario_val.Text, _
                                                        id_usuario_autoriza)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_contenido_radica_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Actualiza_estados_general_semaforo(Hidden_id_respuesta.Value, Me.Page)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_contenido_radica_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_reversa_respuesta.Hide()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_contenido_radica_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_guardar_Click(sender As Object, e As EventArgs) Handles Button_guardar.Click

        Dim Refclas As New Classgestionrespuesta
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Dim clasjava As New Classscrripjava
        Try
            Dim stru As stru_envio = Nothing
            Dim Result As String = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                                                     stru, 1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                clasjava.Showscripman_menu("El usuario no es el responsable de la respuesta, no puede actualizar la nota de la respuesta", Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru.FECHA_RESPUETA <> "" Then
                clasjava.Showscripman_menu("El tramite tiene una confirmación de respuesta, no puede actualizar la nota", Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Actualiza_nota_respuesta(Hidden_id_respuesta.Value, Me.TextBox_NOTA_RESPUESTA.Text)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub
    Protected Sub Button_nota_solo_confirmar_Click(sender As Object, e As EventArgs) Handles Button_nota_solo_confirmar.Click
        Dim clasjava As New Classscrripjava
        Try
            If Hidden_id_respuesta.Value = "-1" Then
                clasjava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.Hidden_id_propietario_resp.Value <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                clasjava.Showscripman_menu("El usuario de gestión no es el autorizado para gestionar la respuesta", Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim Result As String = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, estru)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("PU_TRAZABILIDAD") = estru.RADICADO
                Me.TextBox_NOTA_RESPUESTA.Text = estru.NOTA_RESPUESTA
                Me.UpdatePanel_nota_respuesta.Update()
                Me.ModalPopupExtender_nota_respuesta.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    

    Protected Sub Button_confirmar_solo_confirmar_Click(sender As Object, e As EventArgs) Handles Button_confirmar_solo_confirmar.Click
        Dim clasjava As New Classscrripjava
        Try
            If Hidden_id_respuesta.Value = "-1" Then
                clasjava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            If Me.Hidden_id_propietario_resp.Value <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                clasjava.Showscripman_menu("El usuario de gestión no es el autorizado para gestionar la respuesta", Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            '-----------------------------------------------------------------
            'Retorna datos del tramite para verficar que no se halla iniciado
            'una solicitud de radciación de respuesta
            '-----------------------------------------------------------------
            Dim refclas_resp As New Classgestionrespuesta
            'Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim Result As String = ""
            Result = refclas_resp.Confirma_recibido_de_la_solicitud(Me.Hidden_id_respuesta.Value,
                                                                    Me.Page)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")
                'Exit Sub
            End If

            If Me.Hidden_tipo_respuesta.Value = 0 Then
                Result = refclas_resp.Retorna_estados_semaforo_respuesta_solo_confirmacion(Hidden_id_respuesta.Value, _
                                                                                           Me.Image_estado_resp_solo_confirm)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")

                End If
            Else
                Result = refclas_resp.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(Hidden_id_respuesta.Value, _
                                                                                                       Me.Image_estado_resp_solo_confirm)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If   
            UpdatePanel_image_semaforo_resp.Update()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_solo_confirmar, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_descarga_plantilla_Click(sender As Object, e As EventArgs) Handles Button_descarga_plantilla.Click

        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
                scrijava.Showscripman_menu("Por favor active web service para workflow", Me.UpdatePanel_boton_descarga_formato, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("URL_WEB_SERVICE") = "" Then
                scrijava.Showscripman_menu("Por favor informe la url web service para workflow", Me.UpdatePanel_boton_descarga_formato, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_boton_descarga_formato, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '----------------------------------------------
            '
            Dim refclas_gestion As New ClassGestion
            Dim id_respuesta As Integer = Me.Hidden_id_respuesta.Value
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim radicado As String = ""
            Dim refclas_resp As New Classgestionrespuesta
            Dim id_usuario_firma As Integer = Me.DropDownList_lista_firmas.SelectedValue
            Result = refclas_resp.Descarga_plantilla_radicada_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                        radicado, _
                                                                        Me.Page, _
                                                                        Hidden_ruta_archivo.Value, _
                                                                        id_usuario_firma)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_descarga_formato, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
            updatapanel_iframe.Update()

        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_descarga_formato, "ModalPopupExtender_mensaje_personalizado")
        Finally
            Me.ModalPopupExtender_descarga_formato.Hide()
        End Try
    End Sub
    Protected Sub Button_descarga_plantilla_radicado_Click(sender As Object, e As EventArgs) Handles Button_descarga_plantilla_radicado.Click
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
                scrijava.Showscripman_menu("Por favor active web service para workflow", Me.UpdatePanel_descarga_plantilla_radicada, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("URL_WEB_SERVICE") = "" Then
                scrijava.Showscripman_menu("Por favor informe la url web service para workflow", Me.UpdatePanel_descarga_plantilla_radicada, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_descarga_plantilla_radicada, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim refclas_resp As New Classgestionrespuesta
            Dim radicado As String = ""
            Dim id_usuario_respuesta As Integer = Me.DropDownList_lista_firma_interface.SelectedValue
            Result = refclas_resp.Descarga_plantilla_radicada_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                        radicado, _
                                                                        Me.Page, _
                                                                        Hidden_ruta_archivo.Value, _
                                                                        id_usuario_respuesta)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_descarga_plantilla_radicada, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            If Hidden_descarga_hml_dowload.Value = 1 Then
                ModalPopupExtender_edition_descarga_plantilla_radicada.Hide()
                ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                updatapanel_iframe.Update()
            End If
            If Hidden_descarga_hml_dowload.Value = 0 Then
                ModalPopupExtender_edition_descarga_plantilla_radicada.Hide()
                Me.ifm_html_editor.Attributes.Add("src", "../radicador/WebFormEdicionWordHtml.aspx")
                UpdatePanel_html_editor.Update()
                ModalPopupExtender_edition_html.Show()
            End If
            Result = refclas_resp.Actualiza_estados_general_semaforo(Me.Hidden_id_respuesta.Value, _
                                                                     Me.Page)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_descarga_plantilla_radicada, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_descarga_plantilla_radicada, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_carga_plantilla_Click(sender As Object, e As EventArgs) Handles Button_carga_plantilla.Click
        Dim Refclas As New Classgestionrespuesta
        Dim refclas_solicitud As New ClassRaSolicitudesAprobacion
        Dim scrijava As New Classscrripjava
        Try

            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""   
            Result = Refclas.Generar_interface_responder_solicitud(Hidden_id_respuesta.Value, _
                                                                       Val(Me.Hidden_id_propietario_resp.Value), _
                                                                        Me.Page)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Panel_descarga_formato_adjunto_archivo.Visible = False
                UpdatePanel_descarga_formato_adjunto_archivo.Update()
                Panel_opcion_adjunta.Visible = True
                UpdatePane_opcion_adjunta.Update()
                Me.ModalPopupExtender_edition_sube_documento_respuesta.Show()
            End If

        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
            If Session.Item("WF_ERROR_RESPUESTA") = "adjunto" Then
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/"
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA") = archivo_donwload
                sender.SaveAs(archivo_donwload)
            End If
            If Session.Item("WF_ERROR_RESPUESTA") = "anexo" Then
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/"
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                If Directory.Exists(ruta_fisica) = False Then
                    Directory.CreateDirectory(ruta_fisica)
                End If
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & e.FileName
                HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA") = archivo_donwload
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                sender.SaveAs(archivo_donwload)
            End If
        Catch ex As Exception
            Session.Item("WF_ERROR_RESPUESTA") = ex.Message

        End Try
    End Sub

    Private Sub Button_sube_documento_adjunto_respuesta_Click(sender As Object, e As EventArgs) Handles Button_sube_documento_adjunto_respuesta.Click
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try
            If Session.Item("WF_ERROR_RESPUESTA") <> "anexo" And Session.Item("WF_ERROR_RESPUESTA") <> "adjunto" Then
                scrijava.Showscripman_menu(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_anexos_respuesta_boton, "ModalPopupExtender_mensaje_personalizado")
                ModalPopupExtender_edition_sube_documento_respuesta.Hide()
                Exit Sub
            End If
            'Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & Hidden_id_respuesta.Value & "/"
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/TEMP_TIF/"
            ruta_virtual = Server.MapPath(ruta_virtual)
            If Directory.Exists(ruta_virtual) = False Then
                Directory.CreateDirectory(ruta_virtual)
            End If
            Result = Refclas.Subir_anexo_a_la_respuesta(Hidden_id_respuesta.Value,
                                                        HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA"),
                                                        ruta_virtual,
                                                        Me.DropDownList_anexos_respuesta,
                                                        Me.UpdatePanel_anexos_respuesta,
                                                        Me.DropDownList_anexos_respuesta_simple,
                                                        Me.UpdatePanel_anexos_respuesta_simple)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_anexos_respuesta_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            ModalPopupExtender_edition_sube_documento_respuesta.Hide()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_sube_documento_Click(sender As Object, e As EventArgs) Handles Button_sube_documento.Click
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try
            Dim id_tipo_envio As Integer = 0
            Dim consecutivo_radicado_respuesta As String = ""
            Dim id_imagen_almacena As Integer = 0
            If Session.Item("WF_ERROR_RESPUESTA") <> "anexo" And Session.Item("WF_ERROR_RESPUESTA") <> "adjunto" Then
                scrijava.Showscripman_menu(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_sube_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
                ModalPopupExtender_edition_sube_documento_respuesta.Hide()
                Exit Sub
            End If
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/"
            Dim archivo_donwload As String = HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA")
            If Me.Check_adjunta_formato.Checked = True Then
                Result = Refclas.Subir_formato_respuesta_radicado(Hidden_id_respuesta.Value, _
                                                       archivo_donwload, _
                                                       consecutivo_radicado_respuesta, _
                                                       id_imagen_almacena, _
                                                       id_tipo_envio, _
                                                       Me.Image_estado_resp, _
                                                       UpdatePanel_image_semaforo)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_sube_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
                    ModalPopupExtender_edition_sube_documento_respuesta.Hide()
                    Exit Sub
                End If
                Me.Hidden_tipo_respuesta.Value = id_tipo_envio
                Kill(archivo_donwload)
                UpdatePanel_respuesta_documento.Update()
                Me.ModalPopupExtender_edition_sube_documento_respuesta.Hide()
            Else
                Result = Refclas.Subir_respuesta_radicado(Hidden_id_respuesta.Value, _
                                                        archivo_donwload, _
                                                        consecutivo_radicado_respuesta, _
                                                        id_imagen_almacena, _
                                                        id_tipo_envio, _
                                                        Me.Image_estado_resp, _
                                                        UpdatePanel_image_semaforo)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_sube_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
                    ModalPopupExtender_edition_sube_documento_respuesta.Hide()
                    Exit Sub
                End If
                Me.Hidden_tipo_respuesta.Value = id_tipo_envio
                Kill(archivo_donwload)
                UpdatePanel_respuesta_documento.Update()
                Me.ModalPopupExtender_edition_sube_documento_respuesta.Hide()
            End If

           
            If Session.Item("GA_TIPO_MODULO_RESPUESTA") = 1 Then
                Result = Refclas.Generar_interface_confirmar_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                       Me.Hidden_id_propietario_resp.Value, _
                                                                       Me.Hidden_radicado.Value, _
                                                                       Me.Page, _
                                                                       Me.DropDownList_lista_firmas_confirma_respuesta,
                                                                       Me.Hidden_tipo_respuesta.Value, _
                                                                       1)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_sube_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.ModalPopupExtender_edition_confirma_envio_respuesta.Show()
            Else
                ModalPopupExtender_edition_sube_documento_respuesta.Hide()
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_sube_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_imprimir_Click(sender As Object, e As EventArgs) Handles Button_imprimir.Click
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try

            Dim Result As String = ""
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_imagen As Integer = -1
            Dim gabinete_imagen As String = ""
            Dim refclas_gestion As New Classgestionrespuesta
            Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(Hidden_id_respuesta.Value, gabinete_imagen, id_imagen)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclas_visualiza As New ClassVisualisaDocumento
            Dim id_respuesta As Integer = Me.Hidden_id_respuesta.Value
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim matri_documento() As String = Nothing
            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen, "IMP03GESTIONTMP", matri_documento)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, estru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim fil_ext As String = FileIO.FileSystem.GetFileInfo(matri_documento(1)).Extension
            Dim ruta_local As String = Server.MapPath("../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
            Dim file_copia As String = ruta_local & estru.ID_RESPUESTA_RADICADO & "-" & estru.RADICADO & fil_ext
            If FileIO.FileSystem.FileExists(file_copia) Then
                Kill(file_copia)
            End If
            FileCopy(matri_documento(1), file_copia)
            Session.Item("RA_RUTA_IMPRESION_FINAL") = file_copia
            UpdatePaneliframe.Update()
            ModalPopupExtenderimpre.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_descarga_Click(sender As Object, e As EventArgs) Handles Button_descarga.Click
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", _
                                           Me.UpdatePanel_respuesta_documento, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_imagen As Integer = -1
            Dim gabinete_imagen As String = ""
            Dim refclas_gestion As New Classgestionrespuesta
            Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(Hidden_id_respuesta.Value, _
                                                                              gabinete_imagen, _
                                                                              id_imagen)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_imagen = -1 Then
                scrijava.Showscripman_menu("No hay documento para descargar", _
                                           Me.UpdatePanel_respuesta_documento, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclas_visualiza As New ClassVisualisaDocumento
            Dim id_respuesta As Integer = Me.Hidden_id_respuesta.Value
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim matri_documento() As String = Nothing
            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen, _
                                                                            "IMP03GESTIONTMP", _
                                                                            matri_documento)
            If Result <> "YES" Then
                If Result = "Imposible encontrar datos del documento Gnerando matriz de documentos" Then
                    scrijava.Showscripman_menu("El sistema no pudo encontrar un borrador de respuesta para descargar", _
                                               Me.UpdatePanel_respuesta_documento, _
                                               "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, _
                                               "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

            End If
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                                        estru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, _
                                           Me.UpdatePanel_respuesta_documento, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim fil_ext As String = FileIO.FileSystem.GetFileInfo(matri_documento(1)).Extension
            Dim ruta_local As String = Server.MapPath("../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
            Dim file_copia As String = ruta_local & estru.ID_RESPUESTA_RADICADO & "-" & estru.RADICADO & fil_ext
            If FileIO.FileSystem.FileExists(file_copia) Then
                Kill(file_copia)
            End If
            FileCopy(matri_documento(1), file_copia)
            Hidden_ruta_archivo.Value = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/" & estru.ID_RESPUESTA_RADICADO & "-" & estru.RADICADO & fil_ext
            ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
            updatapanel_iframe.Update()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_radicar_tramite_Click(sender As Object, e As EventArgs) Handles Button_radicar_tramite.Click
        Dim scrijava As New Classscrripjava
        Dim refclasradicado As New ClassRadicador
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Try
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", _
                                           Me.UpdatePanel_respuesta_documento, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.Hidden_id_propietario_resp.Value <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                scrijava.Showscripman_menu("El usuario de gestión no es el autorizado para gestionar la respuesta", Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estado_documento As String = "YES"
            Result = Refclas.Verifica_existencia_documento_de_respuesta(Hidden_id_respuesta.Value, _
                                                                        estado_documento)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_documento = "NO" Then
                scrijava.Showscripman_menu("La respuesta actual no tiene un documento de respuesta asociado, por favor cargue el documento", Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_TIPO_MODULO_RESPUESTA") = 0 Then
                Result = Refclas.Prepara_interface_radica_confirma_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                             Me.Hidden_radicado.Value, _
                                                                             Me.Page, _
                                                                             Me.Hidden_tipo_respuesta.Value, _
                                                                             1)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.ModalPopupExtender_edition_radica_documento_respuesta.Show()
            End If
            If Session.Item("GA_TIPO_MODULO_RESPUESTA") = 1 Then
                Result = Refclas.Generar_interface_confirmar_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                       Me.Hidden_id_propietario_resp.Value, _
                                                                       Me.Hidden_radicado.Value, _
                                                                       Me.Page, _
                                                                       Me.DropDownList_lista_firmas_confirma_respuesta,
                                                                       Me.Hidden_tipo_respuesta.Value, _
                                                                       1)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.ModalPopupExtender_edition_confirma_envio_respuesta.Show()
            End If

        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_examinar_destinatario_Click(sender As Object, e As EventArgs) Handles Button_examinar_destinatario.Click
        Dim Clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassRadicador
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Dim refclasraenv As New ClassRaEnvioCorrespondencia
            Dim Result As String = ""
            Dim id_escrip As Integer = -1
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = -1
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, nombre_plantilla)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_bton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_id_script_validacion(id_plantilla, "DINAMICOEXTERNO", "REMITENTE_COR", id_escrip)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_bton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_escrip = -1 Or id_escrip = 0 Then
                Clasjava.Showscripman_menu("No hay plantilla relacionada para el campo", Me.UpdatePanel_bton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim nombre_plantilla_validacion As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_escrip, nombre_plantilla_validacion)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_bton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Verifica_Permisos_usuario_plantilla_validacion(id_escrip, nombre_plantilla_validacion, 0)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_bton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'If Me.TextBox_NOMBRE_RAZON_SOCIAL.Text <> "" Then
            '    Result = refclasraenv.asigna_remitente_destinatario_id_dest(Me.Page, Me.TextBox_NOMBRE_RAZON_SOCIAL.Text, HttpContext.Current.Session("RA_ID_DEST_EXTERNO"))
            '    If Result <> "YES" Then
            '        Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
            '        Exit Sub
            '    End If
            'End If
            Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = id_escrip
            Me.Iframe_validacion_plantilla_.Attributes.Add("src", "../radicador/WebFormGestionPlantillasvalidacion.aspx")
            Me.UpdatePanel_validacion_plantilla.Update()
            Me.ModalPopupExtender_valiacion_plantilla.Show()
        Catch ex As Exception
            Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_bton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_confirmar_Click(sender As Object, e As EventArgs) Handles Button_confirmar.Click

        'Dim Result As String = ""
        'Dim Refclas As New ClassRadicador
        'Dim Refclasgestion As New Classgestionrespuesta
        'Dim Clasjava As New Classscrripjava
        'Try
        '    Me.Hidden_resultado_ventana.Value = ""
        '    Dim resultado_correo As String = ""
        '    Result = Refclasgestion.Confirma_respuesta_con_radicado(Hidden_id_respuesta.Value, Me.Page, 1, resultado_correo)
        '    If Result <> "YES" Then
        '        Clasjava.Showscripman_menu(Result, Me.UpdatePanel_radic_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    End If
        '    Dim reflclas_resp_radicado As New Class_ra_respuesta_radicado
        '    If Me.Hidden_tipo_respuesta.Value = 0 Then
        '        Result = reflclas_resp_radicado.Solicita_estados_semaforo_respuesta(Hidden_id_respuesta.Value, Me.Image_estado_resp)
        '        If Result <> "YES" Then
        '            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_radic_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
        '            Exit Sub
        '        End If
        '    Else
        '        Dim ref_clas_resp As New Class_ra_respuesta_radicado
        '        Result = ref_clas_resp.Solicita_estados_semaforo_respuesta_electronica(Hidden_id_respuesta.Value, Me.Image_estado_resp)
        '        If Result <> "YES" Then
        '            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_radic_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
        '            Exit Sub
        '        End If
        '    End If
        '    'Me.ModalPopupExtender_edition_radica_documento_respuesta.Hide()
        '    Me.Hidden_resultado_ventana.Value = "YES"
        '    UpdatePanel_image_semaforo.Update()

        '    If resultado_correo <> "" Then
        '        Clasjava.Showscripman_menu(Result, Me.UpdatePanel_radic_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    End If

        'Catch ex As Exception
        '    Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_radic_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
        'End Try
    End Sub

    Private Sub Button_notificar_correo_Click(sender As Object, e As EventArgs) Handles Button_notificar_correo.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim Refclasgestion As New Classgestionrespuesta
        Dim Refclas_ra_respuesta As New Class_ra_respuesta_radicado
        Dim Clasjava As New Classscrripjava
        Try
            Dim stru_envi As stru_envio = Nothing
            If Hidden_id_respuesta.Value = "-1" Then
                Clasjava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_notifica_correo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.Hidden_id_propietario_resp.Value <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                Clasjava.Showscripman_menu("El usuario de gestión no es el autorizado para gestionar la respuesta", Me.UpdatePanel_notifica_correo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Hidden_tipo_respuesta.Value = "-1" Then
                Clasjava.Showscripman_menu("El tramite actual no tiene tipo de respuesta asociada", Me.UpdatePanel_notifica_correo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estado_anexo As Integer = 1
            If Me.CheckBox_anexa_anexos.Checked = True Then
                estado_anexo = 1
            Else
                estado_anexo = 0
            End If
            Result = Refclas_ra_respuesta.Solicita_datos_estructura_envio_por_id_respuesta(Hidden_id_respuesta.Value, _
                                                                                           stru_envi)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_notifica_correo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            If stru_envi.TIPO_RESPUESTA_ELAB_USUARIO = "1" Then
                Result = Refclasgestion.Confirma_respuesta_al_correo_con_radicado(Hidden_id_respuesta.Value, _
                                                                                  Me.Page, _
                                                                                  Me.Hidden_text_user_correo.Value, _
                                                                                  estado_anexo)
                If Result <> "YES" Then
                    Clasjava.Showscripman_menu(Result, Me.UpdatePanel_notifica_correo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = Refclasgestion.Confirma_respuesta_al_correo_con_sin_radicado(Hidden_id_respuesta.Value, _
                                                                                      Me.Page, _
                                                                                      Me.Hidden_text_user_correo.Value, _
                                                                                      estado_anexo)
                If Result <> "YES" Then
                    Clasjava.Showscripman_menu(Result, Me.UpdatePanel_notifica_correo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Result = Refclasgestion.Actualiza_estados_general_semaforo(Hidden_id_respuesta.Value, _
                                                                       Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_notifica_correo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_notifica_correo_respuesta.Hide()
        Catch ex As Exception
            Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_notifica_correo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    'Private Sub Button_publcar_respuesta_Click(sender As Object, e As EventArgs) Handles Button_publcar_respuesta.Click
    '    Dim Result As String = ""
    '    Dim Refclas As New ClassRadicador
    '    Dim Refclasgestion As New Classgestionrespuesta
    '    Dim Clasjava As New Classscrripjava
    '    Try
    '        If Hidden_id_respuesta.Value = "-1" Then
    '            Clasjava.Showscripman_menu("El trámite actual no tiene asignada una respuesta ", Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        If Me.Hidden_id_propietario_resp.Value <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
    '            Clasjava.Showscripman_menu("El usuario de gestión no es el autorizado para gestionar la respuesta ", Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        Dim id_imagen_plantilla As Integer = 0
    '        Dim radicado_respuesta As Integer = 0
    '        Dim fecha_respuesta As Integer = 0
    '        Dim id_imagen_respuesta As Integer = 0
    '        Dim estado_envio_respuesta As Integer = 0
    '        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
    '        Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Hidden_id_respuesta.Value, _
    '                                                                            id_imagen_plantilla, _
    '                                                                            radicado_respuesta, _
    '                                                                            fecha_respuesta, _
    '                                                                            id_imagen_respuesta, _
    '                                                                            estado_envio_respuesta)
    '        If id_imagen_respuesta <> 0 Then
    '            Clasjava.Showscripman_menu("El trámite ya tiene una respuesta públicada, imposible guardar y públicar el documento de respuesta ", Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        'Result = Refclasgestion.Almacena_documento_respuesta_permanente(Hidden_id_respuesta.Value)
    '        If Result <> "YES" Then
    '            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        Result = Refclasgestion.Actualiza_estados_general_semaforo(Hidden_id_respuesta.Value, _
    '                                                                   Me.Page)
    '        If Result <> "YES" Then
    '            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '    Catch ex As Exception
    '        Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
    '    End Try
    'End Sub

    Private Sub Button_reintenta_notificar_correo_Click(sender As Object, e As EventArgs) Handles Button_reintenta_notificar_correo.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim Refclasgestion As New Classgestionrespuesta
        Dim Clasjava As New Classscrripjava
        Try
            Dim estado_anexos As Integer = 1
            If CheckBox_confirma_envio_enexos.Checked = True Then
                estado_anexos = 1
            Else
                estado_anexos = 0
            End If
            Result = Refclasgestion.Confirma_respuesta_al_correo_con_radicado(Hidden_id_respuesta.Value, Me.Page, Me.TextBox_correo_electronico_interf.Text, estado_anexos)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_radic_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclasgestion.Actualiza_estados_general_semaforo(Hidden_id_respuesta.Value, Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_radic_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_radic_documento_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    'Protected Sub Button_gestiona_respuesta_Click(sender As Object, e As EventArgs) Handles Button_gestiona_respuesta.Click
    '    Dim Clasjava As New Classscrripjava
    '    Try
    '        Dim Refclas As New ClassRaSolicitudesAprobacion
    '        Dim Refclasgestion As New Classgestionrespuesta
    '        If Hidden_id_respuesta.Value = "-1" Then
    '            Clasjava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        If Me.Hidden_id_propietario_resp.Value <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
    '            Clasjava.Showscripman_menu("El usuario de gestión no es el autorizado para gestionar la respuesta", Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        '-------------------------------------------------
    '        'Verifica existencia de solicitudes aviertas
    '        'o aprobadas
    '        '-------------------------------------------------
    '        Dim estado As String = ""
    '        Dim Ref_clas_rc_solicitudes As New ClassRaSolicitudesAprobacion
    '        Dim Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(Hidden_id_respuesta.Value, _
    '                                                                                                        0, _
    '                                                                                                        estado, _
    '                                                                                                        HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
    '        If Result <> "YES" Then
    '            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        If estado = "YES" Then
    '            Clasjava.Showscripman_menu("El sistema ha detectado solicitudes de aprobación pendientes por decisión, imposible editar la plantilla ", Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        estado = ""
    '        Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(Hidden_id_respuesta.Value, _
    '                                                                                                    1, _
    '                                                                                                    estado, _
    '                                                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
    '        If Result <> "YES" Then
    '            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        If estado = "YES" Then
    '            Clasjava.Showscripman_menu("El sistema ha detectado una solicitud donde se aprueba el documento de respuesta, imposible editar la plantilla", Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        Dim stru As stru_envio = Nothing
    '        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
    '        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Hidden_id_respuesta.Value, stru)
    '        If Result <> "YES" Then
    '            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        Dim id_imagen_plantilla As Integer = 0
    '        Dim radicado_respuesta As Integer = 0
    '        Dim fecha_respuesta As Integer = 0
    '        Dim id_imagen_respuesta As Integer = 0
    '        Dim estado_envio_respuesta As Integer = 0
    '        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
    '        Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Hidden_id_respuesta.Value, _
    '                                                                            id_imagen_plantilla, _
    '                                                                            radicado_respuesta, _
    '                                                                            fecha_respuesta, _
    '                                                                            id_imagen_respuesta, _
    '                                                                            estado_envio_respuesta)
    '        If Result <> "YES" Then
    '            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        If fecha_respuesta <> 0 Then
    '            Clasjava.Showscripman_menu("El tramite ya tiene una respuesta publicada, no puede editar la respuesta", Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        End If
    '        If Session.Item("GA_TIPO_MODULO_RESPUESTA") = 1 Then
    '            If stru.RADICADO_RESPUESTA = "" Then
    '                Result = Refclasgestion.Prepara_interface_descarga_plantilla_con_radicado(Me.Hidden_id_respuesta.Value, Me.Page)
    '                If Result <> "YES" Then
    '                    Clasjava.Showscripman_menu(Result, Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '                    Exit Sub
    '                Else
    '                    Hidden_descarga_hml_dowload.Value = 0
    '                    UpdatePanel_descarga_hml_dowload.Update()
    '                    ModalPopupExtender_edition_descarga_plantilla_radicada.Show()
    '                    Exit Sub
    '                End If
    '            Else
    '                Me.ifm_html_editor.Attributes.Add("src", "../radicador/WebFormEdicionWordHtml.aspx")
    '                UpdatePanel_html_editor.Update()
    '                ModalPopupExtender_edition_html.Show()
    '            End If
    '        End If
    '        If Session.Item("GA_TIPO_MODULO_RESPUESTA") = 0 Then
    '            Me.ifm_html_editor.Attributes.Add("src", "../radicador/WebFormEdicionWordHtml.aspx")
    '            UpdatePanel_html_editor.Update()
    '            ModalPopupExtender_edition_html.Show()
    '        End If

    '    Catch ex As Exception
    '        Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
    '    End Try
    'End Sub

    Private Sub Button_antualiza_semaforo_chkeditor_Click(sender As Object, e As EventArgs) Handles Button_antualiza_semaforo_chkeditor.Click
        Dim tipo_respuesta_tramite As Integer = 0
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try
            Result = Refclas.Retorna_estado_envio_por_id_respuesta(Hidden_id_respuesta.Value, tipo_respuesta_tramite)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_actualiza_guardar_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'Result = Refclas.Lista_documento_respuesta_drowlis(Hidden_id_respuesta.Value, Me.DropDownList_lista_plantillas)
            'If Result <> "YES" Then
            '    scrijava.Showscripman_menu(Result, Me.UpdatePanel_actualiza_guardar_documento, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim reflclas_resp_radicado As New Class_ra_respuesta_radicado
            If tipo_respuesta_tramite = 0 Then
                Result = reflclas_resp_radicado.Solicita_estados_semaforo_respuesta(Hidden_id_respuesta.Value, Me.Image_estado_resp)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_actualiza_guardar_documento, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                UpdatePanel_image_semaforo.Update()
                UpdatePanel_combo_plantillas.Update()
                'UpdatePanel_respuesta.Update()
            Else
                Dim ref_clas_resp As New Class_ra_respuesta_radicado
                Result = ref_clas_resp.Solicita_estados_semaforo_respuesta_electronica(Hidden_id_respuesta.Value, Me.Image_estado_resp)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_actualiza_guardar_documento, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                UpdatePanel_image_semaforo.Update()
                UpdatePanel_combo_plantillas.Update()
                'UpdatePanel_respuesta.Update()
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_actualiza_guardar_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_eliminar_Click(sender As Object, e As EventArgs) Handles Button_eliminar.Click
        Dim Clasjava As New Classscrripjava
        Try
            Dim Refclas_ As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""  
            Dim Refclas As New Classgestionrespuesta
            If Hidden_resp.Value = "0" Then
                Exit Sub
            End If
            Result = Refclas.Elimina_documento_respuesta(Val(Hidden_id_respuesta.Value), _
                                                                  Me.Image_estado_resp)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            UpdatePanel_image_semaforo.Update()
            UpdatePanel_combo_plantillas.Update()
        Catch ex As Exception
            Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_activa_registro_solicitud_Click(sender As Object, e As EventArgs) Handles Button_activa_registro_solicitud.Click

        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim refclas As New ClassRaSolicitudesAprobacion
            Result = refclas.Verfica_viabilidad_solicitud_aprobacion_respuesta(Val(Hidden_id_respuesta.Value), _
                                                                               Me.TextBox_fecha_limite_solicitud.Text)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            UpdatePanel_registro_solicitud.Update()
            ModalPopupExtender_actualizacion_anualidad.Show()
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub
    Private Sub Button_registrar_solicitud_aprobacion_Click(sender As Object, e As EventArgs) Handles Button_registrar_solicitud_aprobacion.Click
        'Dim Refcriptman As New Classscrripjava
        'Try
        '    Dim Result As String = ""
        '    Dim Refclas As New ClassRaSolicitudesAprobacion
        '    Dim resultado_envio_correo As String = ""
        '    Result = Refclas.Registra_solicitud_aprobacion(Me.DropDownList_prioridad_solicitud.Text, _
        '                                                   Me.TextBox_nota_aprobacion.Text, _
        '                                                   Me.TextBox_fecha_limite_solicitud.Text, _
        '                                                   Me.Hidden_text_user.Value, _
        '                                                   Val(Hidden_id_respuesta.Value), _
        '                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
        '                                                   "", _
        '                                                   resultado_envio_correo)
        '    If Result <> "YES" Then
        '        Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_registro_solicitud, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    Else
        '        If resultado_envio_correo <> "YES" Then
        '            Refcriptman.Showscripman_menu("Se registro la solicitud de aprobación pero fue imposible enviar la notificación al correo electrónico, error  " & resultado_envio_correo, Me.UpdatePanel_registro_solicitud, "ModalPopupExtender_mensaje_personalizado")
        '        End If
        '        Me.ModalPopupExtender_actualizacion_anualidad.Hide()
        '    End If
        'Catch ex As Exception
        '    Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_registro_solicitud, "ModalPopupExtender_mensaje_personalizado")
        'End Try
    End Sub
    Private Sub Button_examinar_dest_externo_Click(sender As Object, e As EventArgs) Handles Button_examinar_dest_externo.Click

        Dim Clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassRadicador
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Dim refclasraenv As New ClassRaEnvioCorrespondencia
            Dim Result As String = ""
            Dim id_escrip As Integer = -1
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = -1
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, _
                                                                           nombre_plantilla)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_id_script_validacion(id_plantilla, _
                                                          "DINAMICOEXTERNO", _
                                                          "REMITENTE_COR", _
                                                          id_escrip)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_escrip = -1 Or id_escrip = 0 Then
                Clasjava.Showscripman_menu("No hay plantilla relacionada para el campo", Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim nombre_plantilla_validacion As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_escrip,
                                                                                    nombre_plantilla_validacion)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Verifica_Permisos_usuario_plantilla_validacion(id_escrip, _
                                                                            nombre_plantilla_validacion, _
                                                                            0)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'If Me.TextBox_NOMBRE_RAZON_SOCIAL.Text <> "" Then
            '    Result = refclasraenv.asigna_remitente_destinatario_id_dest(Me.Page, Me.TextBox_NOMBRE_RAZON_SOCIAL.Text, HttpContext.Current.Session("RA_ID_DEST_EXTERNO"))
            '    If Result <> "YES" Then
            '        Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
            '        Exit Sub
            '    End If
            'End If
            Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = id_escrip
            Me.Iframe_validacion_plantilla_.Attributes.Add("src", "../radicador/WebFormGestionPlantillasvalidacion.aspx")
            Me.UpdatePanel_validacion_plantilla.Update()
            Me.ModalPopupExtender_valiacion_plantilla.Show()
        Catch ex As Exception
            Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_Asigana_datos_validacion_edicion_Click(sender As Object, e As EventArgs) Handles Button_Asigana_datos_validacion_edicion.Click
        Me.ModalPopupExtender_valiacion_plantilla.Hide()
    End Sub

    Private Sub Button_actualizar_peticionario_Click(sender As Object, e As EventArgs) Handles Button_actualizar_peticionario.Click
        Dim refclasconsulta As New ClassRaConsultaRadicados
        Dim scripjava As New Classscrripjava
        Dim refclas As New Classgestionrespuesta
        Try
            Dim Result As String = ""
            If Me.Hidden_id_respuesta.Value = "-1" Then
                scripjava.Showscripman_menu("Imposible encontrar la identificación de la respuesta", Me.UpdatePanel_dest_externo_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBox_dext_externo.Text = "" Then
                scripjava.Showscripman_menu("Debe informar el nombre del peticionario o destinatario externo", Me.UpdatePanel_dest_externo_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBox_login_usuario_val_externo.Text = "" Then
                scripjava.Showscripman_menu("Debe informar el nombre del usuario de autorización", Me.UpdatePanel_dest_externo_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBox_pasw_usuario_val_externo.Text = "" Then
                scripjava.Showscripman_menu("Debe informar la contraseña del usuario de autorización", Me.UpdatePanel_dest_externo_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_user_autorizacion As Integer = 0
            Result = refclas.Valida_usuario_administrador_general(Me.TextBox_login_usuario_val_externo.Text, _
                                                                  Me.TextBox_pasw_usuario_val_externo.Text, _
                                                                  id_user_autorizacion, _
                                                                  "actualiza_peticionario")
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = refclasconsulta.Reasigna_destinatario_externo(Me.Hidden_id_respuesta.Value, _
                                                                   id_user_autorizacion.ToString & _
                                                                   "-" & Me.TextBox_login_usuario_val_externo.Text, _
                                                                   Me.TextBox_dext_externo.Text, _
                                                                   Me.Hidden_remitente_destinatario.Value)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                                        estru)
            If Result <> "YES" Then
                label_result.Text = Result
                Exit Sub
            Else
                label_title.Text = "Respuesta al radicado (" & estru.RADICADO & ") código (" & estru.ID_RESPUESTA_RADICADO & ") Peticionario (" & estru.DESTINATARIO & ") Tramite (" & estru.TRAMITE_DOCUMENTO & ")"
                Me.UpdatePanel_titulo_respuesta.Update()
            End If
            ModalPopupExtender_edition_asigna_dest_externo.Hide()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_dest_externo_boton, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Private Sub Button_anexo_cargar_Click(sender As Object, e As EventArgs) Handles Button_anexo_cargar.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                                       stru, _
                                                                                       1)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, _
                                           Me.UpdatePanel_anexos_respuesta_boton, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                scrijava.Showscripman_menu("El usuario no es el responsable de la respuesta, no puede agregar anexos",
                                           Me.UpdatePanel_anexos_respuesta_boton,
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Label_estado_carga.Text = "Por favor cargue archivos tipo (xls,xlsx,docx,doc,txt,pdf,jpg,ppt,pptx,bmp;tif,tiff,pdfa), maximo número de archivos a adjuntar 1"
            AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload_adjunto"
            AjaxFileUpload_dowload.AllowedFileTypes = "xls,xlsx,docx,doc,txt,pdf,jpg,ppt,pptx,bmp,tif,tiff,pdfa"
            AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
            Session.Item("WF_ERROR_RESPUESTA") = "anexo"
            Panel_descarga_formato_adjunto_archivo.Visible = False
            UpdatePanel_descarga_formato_adjunto_archivo.Update()
            Panel_opcion_adjunta.Visible = False
            UpdatePane_opcion_adjunta.Update()
            UpdatePanel_descarga.Update()
            Me.ModalPopupExtender_edition_sube_documento_respuesta.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_anexo_eliminar_Click(sender As Object, e As EventArgs) Handles Button_anexo_eliminar.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            If Me.Hidden_resp_elimina_anexo.Value = 0 Then
                Exit Sub
            End If
            If Me.DropDownList_anexos_respuesta.Text = "" Then
                scrijava.Showscripman_menu("Por favor seleccione el archivo a eliminar", _
                                           Me.UpdatePanel_anexos_respuesta_boton, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & Hidden_id_respuesta.Value & "/"
            Result = Refclas.Eliminar_anexo_documento_respuesta(Me.DropDownList_anexos_respuesta.SelectedValue, _
                                                                Hidden_id_respuesta.Value, _
                                                                DropDownList_anexos_respuesta, _
                                                                Me.UpdatePanel_anexos_respuesta, _
                                                                Me.DropDownList_anexos_respuesta_simple, _
                                                                Me.UpdatePanel_anexos_respuesta_simple)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_anexos_respuesta_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_descargar_anexo_Click(sender As Object, e As EventArgs) Handles Button_descargar_anexo.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim ref_class_gestion As New Classgestionrespuesta
            Dim Result As String = ""
            If Me.DropDownList_anexos_respuesta.Text = "" Then
                scrijava.Showscripman_menu("Por favor seleccione el archivo a descargar", Me.UpdatePanel_anexos_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = ref_class_gestion.Descargar_enexo_dcoumento_respuesta(Me.DropDownList_anexos_respuesta.SelectedValue,
                                                                         ifimpre_descarga_anexo_respuesta_,
                                                                         ModalPopupExtender_edition_descarga_anexo_respuesta,
                                                                         UpdatePanel_descarga_anexo_respuesta)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_anexos_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_anexo_cargar_simple_Click(sender As Object, e As EventArgs) Handles Button_anexo_cargar_simple.Click
        Dim scrijava As New Classscrripjava
        Try
            
            Dim Result As String = ""
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                                        stru, _
                                                                                        1)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                scrijava.Showscripman_menu("El usuario no es el responsable de la respuesta, no puede agregar anexos", Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Label_estado_carga.Text = "Por favor cargue archivos tipo (xls,xlsx,docx,doc,txt,pdf,jpg,ppt,pptx,bmp,tif,tiff,pdfa), maximo número de archivos a adjuntar 1"
            AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload_adjunto_simple"
            AjaxFileUpload_dowload.AllowedFileTypes = "xls,xlsx,docx,doc,txt,pdf,jpg,ppt,pptx,bmp,tif,tiff,pdfa"
            AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
            Session.Item("WF_ERROR_RESPUESTA") = "anexo"
            Panel_descarga_formato_adjunto_archivo.Visible = False
            UpdatePanel_descarga_formato_adjunto_archivo.Update()
            Panel_opcion_adjunta.Visible = False
            UpdatePane_opcion_adjunta.Update()
            UpdatePanel_descarga.Update()
            ModalPopupExtender_edition_sube_documento_respuesta.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_anexo_eliminar_simple_Click(sender As Object, e As EventArgs) Handles Button_anexo_eliminar_simple.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            If Me.DropDownList_anexos_respuesta_simple.Text = "" Then
                scrijava.Showscripman_menu("Por favor seleccione el archivo a eliminar", _
                                           Me.UpdatePanel_anexos_respuesta_simple, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & Hidden_id_respuesta.Value & "/"
            Result = Refclas.Eliminar_anexo_documento_respuesta(Me.DropDownList_anexos_respuesta_simple.SelectedValue, _
                                                                Hidden_id_respuesta.Value, _
                                                                DropDownList_anexos_respuesta, _
                                                                Me.UpdatePanel_anexos_respuesta, _
                                                                Me.DropDownList_anexos_respuesta_simple, _
                                                                Me.UpdatePanel_anexos_respuesta_simple)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_descargar_anexo_simple_Click(sender As Object, e As EventArgs) Handles Button_descargar_anexo_simple.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim ref_class_gestion As New Classgestionrespuesta
            Dim Result As String = ""
            If Me.DropDownList_anexos_respuesta.Text = "" Then
                scrijava.Showscripman_menu("Por favor seleccione el archivo a descargar", Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = ref_class_gestion.Descargar_enexo_dcoumento_respuesta(Me.DropDownList_anexos_respuesta_simple.SelectedValue,
                                                                           ifimpre_descarga_anexo_respuesta_,
                                                                           ModalPopupExtender_edition_descarga_anexo_respuesta,
                                                                           UpdatePanel_descarga_anexo_respuesta)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_sube_documento_adjunto_respuesta_simple_Click(sender As Object, e As EventArgs) Handles Button_sube_documento_adjunto_respuesta_simple.Click
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try
            If Session.Item("WF_ERROR_RESPUESTA") <> "anexo" And Session.Item("WF_ERROR_RESPUESTA") <> "adjunto" Then
                scrijava.Showscripman_menu(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
                ModalPopupExtender_edition_sube_documento_respuesta.Hide()
                Exit Sub
            End If
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/TEMP_TIF/"
            ruta_virtual = Server.MapPath(ruta_virtual)
            If Directory.Exists(ruta_virtual) = False Then
                Directory.CreateDirectory(ruta_virtual)
            End If
            Result = Refclas.Subir_anexo_a_la_respuesta(Hidden_id_respuesta.Value, _
                                                        HttpContext.Current.Session.Item("EXTENSION_ARCHIVO_ADJUNTA"), _
                                                        ruta_virtual, _
                                                        DropDownList_anexos_respuesta, _
                                                        Me.UpdatePanel_anexos_respuesta, _
                                                        Me.DropDownList_anexos_respuesta_simple, _
                                                        Me.UpdatePanel_anexos_respuesta_simple)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_sube_documento_respuesta.Hide()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta_simple, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_confirmar_envio_respuesta_Click(sender As Object, e As EventArgs) Handles Button_confirmar_envio_respuesta.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim Refclasgestion As New Classgestionrespuesta
        Dim Clasjava As New Classscrripjava
        Try
            Me.Hidden_resultado_envio.Value = ""
            Dim resultado_correo As String = ""
            Result = Refclasgestion.Responder_a_la_solicitud(Hidden_id_respuesta.Value, _
                                                             Me.Page, _
                                                             1, _
                                                             Me.DropDownList_lista_firmas_confirma_respuesta.SelectedValue, _
                                                             resultado_correo)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.update_boton_confirma, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclasgestion.Actualiza_estados_general_semaforo(Hidden_id_respuesta.Value, _
                                                                       Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.update_boton_confirma, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            UpdatePanel_image_semaforo.Update()
            UpdatePanel_image_semaforo_resp.Update()
            If resultado_correo <> "" Then
                Clasjava.Showscripman_menu(resultado_correo, Me.update_boton_confirma, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_resultado_envio.Value = "YES"
            ModalPopupExtender_edition_confirma_envio_respuesta.Hide()
        Catch ex As Exception
            Clasjava.Showscripman_menu(ex.Message, Me.update_boton_confirma, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ImageButton_desplegar_formal_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_desplegar_formal.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim refclas_resp As New Classgestionrespuesta
            Dim Result As String = ""
            Dim t As String = ImageButton_desplegar_formal.ImageUrl
            'Result = refclas_resp.Lista_documento_respuesta_drowlis(Hidden_id_respuesta.Value, Me.DropDownList_lista_plantillas)
            'If Result <> "YES" Then
            '    scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Result = refclas_resp.Actualiza_estados_general_semaforo(Hidden_id_respuesta.Value, Me.Page)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & Hidden_id_respuesta.Value & "/"
            Result = refclas_resp.Lista_nombre_documentos_anexos_respuesta_droplist(Server.MapPath(ruta_virtual), Me.DropDownList_anexos_respuesta)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanel_anexos_respuesta.Update()
            End If

        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ImageButton_confirmar_firma_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_confirmar_firma.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim Result As String = refclas_resp.Prepara_interface_solo_confirma_respuesta(Me.Hidden_id_respuesta.Value, Me.Page)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub

            End If
            Dim tipo_respuesta_tramite As Integer = 0
            Result = refclas_resp.Retorna_estado_envio_por_id_respuesta(Hidden_id_respuesta.Value, tipo_respuesta_tramite)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_tipo_respuesta.Value = tipo_respuesta_tramite
            '------------------------------------------
            'Electronica =1 física 0
            '------------------------------------------
            If tipo_respuesta_tramite = 0 Then
                Result = refclas_resp.Retorna_estados_semaforo_respuesta_solo_confirmacion(Hidden_id_respuesta.Value, Me.Image_estado_resp_solo_confirm)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")

                End If
            Else
                Result = refclas_resp.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(Hidden_id_respuesta.Value, Me.Image_estado_resp_solo_confirm)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")

                End If
            End If
            Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & Hidden_id_respuesta.Value & "/"
            Result = refclas_resp.Lista_nombre_documentos_anexos_respuesta_droplist(Server.MapPath(ruta_virtual), Me.DropDownList_anexos_respuesta_simple)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanel_anexos_respuesta_simple.Update()
            End If
            UpdatePanel_image_semaforo_resp.Update()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ButtonActiva_solicitud_aprobacion_Click(sender As Object, e As EventArgs) Handles ButtonActiva_solicitud_aprobacion.Click
        Me.Iframe_solicitud_aprobacion.Attributes.Add("src", "../radicador/WebFormRaGestionSolicitudesAprobacion.aspx")
        Me.UpdatePanel_solicitud_aprobacion.Update()
        Me.ModalPopupExtender_solicitud_aprobacion.Show()
    End Sub

    Private Sub Button_descarga_respuesta_Click(sender As Object, e As EventArgs) Handles Button_descarga_respuesta.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim struc_envio As stru_envio = Nothing
            Dim refclasgestion As New Classgestionrespuesta
            Dim refclasdescargapublic As New Classdescargapublico
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, struc_envio)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If struc_envio.RADICADO Is Nothing Then
                scrijava.Showscripman_menu("El radicado " & Session.Item("PU_TRAZABILIDAD") & " no tiene una respuesta relacionada", Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If struc_envio.ID_TIPO_DOC_RESPUESTA = 1 Then
                Me.ModalPopupExtender_opcion_descarga_respuesta.Show()
            Else
                Result = refclasgestion.Descarga_documento_respuesta(struc_envio.ID_RESPUESTA_RADICADO,
                                                                     Me.DropDownList_tipo_archivo.Text,
                                                                     0,
                                                                     Me.ifmExcel_,
                                                                     Me.updatapanel_iframe,
                                                                     Me.Hidden_ruta_archivo)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_combo_plantillas, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_descarga_docmento_respuesta_Click(sender As Object, e As EventArgs) Handles Button_descarga_docmento_respuesta.Click
        Dim Result As String = ""
        Dim Refclas_gestion_respuesta As New Classgestionrespuesta
        Dim scripjava As New Classscrripjava
        Try
            If Me.Check_opcion_descarga_respuesta_sin_firma.Checked = True Then
                Result = Refclas_gestion_respuesta.Descarga_documento_respuesta(Me.Hidden_id_respuesta.Value,
                                                                                Me.DropDownList_tipo_archivo.Text,
                                                                                0,
                                                                                Me.ifmExcel_,
                                                                                Me.updatapanel_iframe,
                                                                                Me.Hidden_ruta_archivo)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePane_opcion_descarga_respuesta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                
                End If
            End If
            If Me.CheckBox_opcion_descarga_respuesta_con_firma.Checked = True Then
                Result = Refclas_gestion_respuesta.Descarga_documento_respuesta(Me.Hidden_id_respuesta.Value,
                                                                                Me.DropDownList_tipo_archivo.Text,
                                                                                1,
                                                                                Me.ifmExcel_,
                                                                                Me.updatapanel_iframe,
                                                                                Me.Hidden_ruta_archivo)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePane_opcion_descarga_respuesta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub         
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePane_opcion_descarga_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_activa_solicitudes_colaboracion_Click(sender As Object, e As EventArgs) Handles Button_activa_solicitudes_colaboracion.Click

        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim struc_envio As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, struc_envio)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_gestion_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HttpContext.Current.Session.Item("GA_STRU_RADICADO_COLABORACION") = struc_envio.RADICADO
            Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "RELACIONADO A RADICADO"
            Me.Iframe_registro_colaboracion_.Attributes.Add("src", "../gestion/WebFormGaListaDocumentosColaboracion.aspx")
            Me.UpdatePanel_registro_colaboracion.Update()
            Me.ModalPopupExtender_edition_registro_colaboracion.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_gestion_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_descarga_formato_Click(sender As Object, e As EventArgs) Handles Button_activa_descarga_formato.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim Refclas As New Class_relacion_firmas_autorizadas
            Dim refclas_gestion As New Classgestionrespuesta
            Dim stru_usu_firmas() As stru_usu_firmas_autorizadas = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                                        estru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '-----------------------------------------
            'abre ventana de descarga de la plantilla
            'radicada
            '-----------------------------------------
            If Session.Item("GA_TIPO_MODULO_RESPUESTA") = 1 Then
                If estru.RADICADO_RESPUESTA = "" Then
                    Result = Refclas.Solicita_lista_usuarios_permitidos_firma(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                              stru_usu_firmas)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePanel_respuesta_documento)
                        Exit Sub
                    End If
                    Result = refclas_gestion.Lista_usuarios_firmas_permitidas_iterface(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                       stru_usu_firmas, _
                                                                                       HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                       Me.DropDownList_lista_firma_interface, _
                                                                                       Me.UpdatePanel_descarga_formato_interface)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePanel_respuesta_documento)
                        Exit Sub
                    End If
                    Result = refclas_resp.Prepara_interface_descarga_plantilla_con_radicado(Me.Hidden_id_respuesta.Value, _
                                                                                            Me.Page)
                    If Result <> "YES" Then
                        scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        Hidden_descarga_hml_dowload.Value = 1
                        UpdatePanel_descarga_hml_dowload.Update()
                        ModalPopupExtender_edition_descarga_plantilla_radicada.Show()
                        Exit Sub
                    End If
                Else
                    Result = Refclas.Solicita_lista_usuarios_permitidos_firma(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                              stru_usu_firmas)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePanel_respuesta_documento)
                        Exit Sub
                    End If
                    Dim Refclas_registro_dow As New Class_ra_ra_registro_down_formato
                    Dim id_usuario_respuesta As Integer = 0
                    Result = Refclas_registro_dow.Solicita_utltimo_usuario_firma_formato_descarga(Me.Hidden_id_respuesta.Value, _
                                                                                                  id_usuario_respuesta)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePanel_respuesta_documento)
                        Exit Sub
                    End If
                    Result = refclas_gestion.Lista_usuarios_firmas_permitidas_iterface(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                     stru_usu_firmas, _
                                                                                     id_usuario_respuesta, _
                                                                                     Me.DropDownList_lista_firmas, _
                                                                                     Me.UpdatePanel_descarga_formato)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePanel_respuesta_documento)
                        Exit Sub
                    End If
                    ModalPopupExtender_descarga_formato.Show()
                End If
            End If
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, Me.UpdatePanel_gestion_respuesta)
        End Try
    End Sub

    Protected Sub Check_adjunta_formato_CheckedChanged(sender As Object, e As EventArgs) Handles Check_adjunta_formato.CheckedChanged
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim extension As String = ""
            If HttpContext.Current.Session.Item("TIPO_ADJUNTA_STATE") = 1 Then
                If Check_adjunta_formato.Checked = True Then

                    Result = Refclas.Actualiza_mensaje_tipo_adjunta(1, _
                                                       Label_estado_carga.Text, _
                                                       AjaxFileUpload_dowload, _
                                                       UpdatePanel_Panel_descarga_ajax, _
                                                       UpdatePanel_descarga)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePane_opcion_adjunta)
                    End If
                Else
                    Result = Refclas.Actualiza_mensaje_tipo_adjunta(2, _
                                                       Label_estado_carga.Text, _
                                                       AjaxFileUpload_dowload, _
                                                       UpdatePanel_Panel_descarga_ajax, _
                                                       UpdatePanel_descarga)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePane_opcion_adjunta)
                    End If
                End If
            Else

            End If
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, Me.UpdatePane_opcion_adjunta)
        End Try
    End Sub

    Protected Sub CheckBox_adjunta_documento_libre_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_adjunta_documento_libre.CheckedChanged
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim extension As String = ""
            If HttpContext.Current.Session.Item("TIPO_ADJUNTA_STATE") = 1 Then
                If CheckBox_adjunta_documento_libre.Checked = True Then
                    Result = Refclas.Actualiza_mensaje_tipo_adjunta(2, _
                                                                    Label_estado_carga.Text, _
                                                                    AjaxFileUpload_dowload, _
                                                                    UpdatePanel_Panel_descarga_ajax, _
                                                                    UpdatePanel_descarga)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePane_opcion_adjunta)
                    End If
                Else
                    Result = Refclas.Actualiza_mensaje_tipo_adjunta(1, _
                                                                    Label_estado_carga.Text, _
                                                                    AjaxFileUpload_dowload, _
                                                                    UpdatePanel_Panel_descarga_ajax, _
                                                                    UpdatePanel_descarga)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePane_opcion_adjunta)
                    End If
                End If
            Else

            End If
            
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, Me.UpdatePane_opcion_adjunta)
        End Try
    End Sub

    Protected Sub Button_confirma_reversar_Click(sender As Object, e As EventArgs) Handles Button_confirma_reversar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Refclas As New Classgestionrespuesta
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Me.Hidden_id_respuesta.Value, _
                                                                                id_imagen_plantilla, _
                                                                                radicado_respuesta, _
                                                                                fecha_respuesta, _
                                                                                id_imagen_respuesta, _
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_confirma_reversa)
                Exit Sub
            End If
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value, _
                                                                                        stru, _
                                                                                        1)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_confirma_reversa)
                Exit Sub
            End If
            Result = Refclas.Reversa_respuesta_radicado(stru, _
                                                        Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                        Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_confirma_reversa)
                Exit Sub
            End If
            Result = Refclas.Actualiza_estados_general_semaforo(Hidden_id_respuesta.Value, _
                                                                Me.Page)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_confirma_reversa)
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_confirma_reversa)
        Finally
            Me.ModalPopupExtender_edition_confirma_reversa_respuesta.Hide()
        End Try
    End Sub

    Private Sub Button_cancelar_registro_Click(sender As Object, e As EventArgs) Handles Button_cancelar_registro.Click
        Me.ModalPopupExtender_actualizacion_anualidad.Hide()
    End Sub

    Protected Sub Button_cancel_envio_respuesta_Click(sender As Object, e As EventArgs) Handles Button_cancel_envio_respuesta.Click
        Me.ModalPopupExtender_edition_confirma_envio_respuesta.Hide()
    End Sub

    Protected Sub Button_cancela_descarga_plantilla_Click(sender As Object, e As EventArgs) Handles Button_cancela_descarga_plantilla.Click
        Me.ModalPopupExtender_descarga_formato.Hide()
    End Sub

    Protected Sub Button_cancelar_documento_respuesta_Click(sender As Object, e As EventArgs) Handles Button_cancelar_documento_respuesta.Click
        Me.ModalPopupExtender_opcion_descarga_respuesta.Hide()
    End Sub

    Protected Sub Button__reversar_cancelar_Click(sender As Object, e As EventArgs) Handles Button__reversar_cancelar.Click
        Me.ModalPopupExtender_edition_reversa_respuesta.Hide()
    End Sub
End Class