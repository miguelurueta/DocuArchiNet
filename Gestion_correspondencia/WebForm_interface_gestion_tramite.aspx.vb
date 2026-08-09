Imports System.IO
Imports System.Drawing
Imports System.Web.Services
Imports Neodynamic.WebControls.ImageDraw
Public Class WebForm_interface_gestion_tramite
    Inherits System.Web.UI.Page
    Public Matri_Doc_Visual() As String
    Public Doc_actual As String = ""
    Public ruta_documento As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False And AjaxFileUpload_dowload.IsInFileUploadPostBack = False And AjaxFileUpload_sube_plantilla_respuesta.IsInFileUploadPostBack = False Then
            Dim Refclas_inicio As New Class_inicializa_gestion_correspondencia
            Dim Result As String = ""
            Result = Refclas_inicio.Inicializa_gestion_correspondencia(Session.Item("Id_Usuario_Workflow"),
                                                                       Session.Item("Id_Ruta_Workflow"),
                                                                       Session.Item("Id_Grupo_Workflow"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
                Exit Sub
            End If
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = 0
            HttpContext.Current.Session.Item("DG_ID_TRAMITE") = 0
            HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = ""
            HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
            Dim campo_radicado As String = ""
            Dim Ref_class As New Class_configuracion_listado_ruta
            Result = Ref_class.SolicitaNombreCampoRadicadoRuta(Session.Item("Id_Ruta_Workflow"),
                                                               Session.Item("WF_CAMPOS_RADICADO_LISTA_TRAMITE"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
                Exit Sub
            End If
            Result = Ref_class.SolicitaNombreCampoBenificiarioRuta(Session.Item("Id_Ruta_Workflow"),
                                                                      Session.Item("WF_CAMPOS_BENEFICIARIO_LISTA_TRAMITE"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
                Exit Sub
            End If
            Result = Ref_class.SolicitaNombreCampoTramiteRuta(Session.Item("Id_Ruta_Workflow"),
                                                              Session.Item("WF_CAMPOS_TRAMITE_LISTA_TRAMITE"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
                Exit Sub
            End If
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Session.Item("SortExpression_compartido") = "etw.fecha_inicio"
            Session.Item("SortDirection_compartido") = "DESC"
            Result = reflcas_respuesta.Lista_tramites(Session.Item("Id_Usuario_Workflow"),
                                                      Session.Item("Id_Ruta_Workflow"),
                                                      Session.Item("Id_Grupo_Workflow"),
                                                      Session.Item("WF_ID_ACTIVIDAD"),
                                                      HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE"),
                                                      Session.Item("WF_RUTAWORKFLOW"),
                                                      Me.data_grid_listado_solicitudes,
                                                      HiddenEmailconsulta,
                                                      Me.Label_titulo_listado_solicitudes,
                                                      Me.hdnEmailID,
                                                      UpdateGeneral,
                                                      "",
                                                      Me.UpdatePanel_title,
                                                      1,
                                                      "",
                                                       Session.Item("SortExpression_compartido"),
                                                       Session.Item("SortDirection_compartido"),
                                                      Me.Hidden_content)
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            End If
            Dim Refclas As New InicioWorkflow
            Result = Refclas.Inicializa_firma_usuario_workflow()
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            Else
                HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL") = HttpContext.Current.Session("WF_RUTA_FIRMA") & HttpContext.Current.Session("Id_Usuario_Workflow") & ".bmp"
            End If
            Hiddenintercambio2.Value = HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL")
            Dim refclas2 As New ClassNeodynamic
            refclas2.Firma_transparente()
        Else
            Dim Matri_Temp() As String
            Erase Matri_Temp
            If Session.Item("WF_MATRI_IMAGE") <> "" Then
                Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
                If Not Matri_Temp Is Nothing Then
                    For i As Integer = 0 To Matri_Temp.Length - 2
                        ReDim Preserve Matri_Doc_Visual(i)
                        Matri_Doc_Visual(i) = Matri_Temp(i)
                    Next
                End If
            End If
        End If
    End Sub
    Private Sub data_grid_listado_solicitudes_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_listado_solicitudes.Sorting
        Dim clasjava As New Classscrripjava
        Try
            Dim Sortdir As String = ""
            Dim Result As String = ""
            Dim Refclas As New ClassGredview
            Session.Item("SortExpression_compartido") = e.SortExpression
            If Session.Item("SortDirection_compartido") = "DESC" Then
                Session.Item("SortDirection_compartido") = "ASC"
            Else
                Session.Item("SortDirection_compartido") = "DESC"
            End If
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Result = reflcas_respuesta.Cahche_pagin_sorting_lista_tramites(Me.data_grid_listado_solicitudes,
                                                                           UpdateGeneral,
                                                                           1,
                                                                           Session.Item("SortExpression_compartido"),
                                                                           Session.Item("SortDirection_compartido"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.hdnEmailID.Value = "-1"
                Me.UpdateGeneral.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_solicitudes.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_listado_solicitudes.PageIndex = e.NewPageIndex
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Dim Result As String = ""
            Result = reflcas_respuesta.Cahche_pagin_sorting_lista_tramites(Me.data_grid_listado_solicitudes,
                                                                            Me.UpdateGeneral,
                                                                            0,
                                                                            Session.Item("SortExpression_compartido"),
                                                                            Session.Item("SortDirection_compartido"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdateGeneral)
            Else
                Me.hdnEmailID.Value = "-1"
                Me.UpdateGeneral.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImageButton_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_buscar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Dim Result As String = ""
            If Me.auto_complex.Text = "" Then
                Exit Sub
            End If
            Result = reflcas_respuesta.Cahche_Search_lista_tramites(Me.data_grid_listado_solicitudes,
                                                                    UpdateGeneral,
                                                                    0,
                                                                    Session.Item("SortExpression_compartido"),
                                                                    Session.Item("SortDirection_compartido"),
                                                                    Me.auto_complex.Text,
                                                                    Me.Label_titulo_listado_solicitudes,
                                                                    Me.UpdatePanel_title)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_lik_service_boton_Click(sender As Object, e As EventArgs) Handles Button_lik_service_boton.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Por tramitar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "En tramite"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Tramitado"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Solicitud por aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Solicitud aprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Solicitud desaprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "8" Then
                selecion_name = "Solicitud archivada"
            End If
            If Me.Hidden_lik_service_boton.Value = "9" Then
                selecion_name = "Solicitud anulada"
            End If
            If Me.Hidden_lik_service_boton.Value = "10" Then
                selecion_name = "Tramitado archivado"
            End If
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Dim Result As String = ""
            Result = reflcas_respuesta.Cache_filtra_tramites(Me.data_grid_listado_solicitudes,
                                                            UpdateGeneral,
                                                            0,
                                                            Session.Item("SortExpression_compartido"),
                                                            Session.Item("SortDirection_compartido"),
                                                            selecion_name,
                                                            Me.Label_titulo_listado_solicitudes,
                                                            Me.UpdatePanel_title)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
        Catch ex As Exception
        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_DataBound(sender As Object, e As EventArgs) Handles data_grid_listado_solicitudes.DataBound
        Try
            'Select Case sender.SortDirection
            '    Case SortDirection.Ascending
            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black

            '    Case SortDirection.Descending
            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black

            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black
            'End Select
        Catch ex As Exception
        End Try

    End Sub
    '---------------------------------------------------------
    'Activa gestion  respuesta documento de correspondencia
    '---------------------------------------------------------
    Private Sub Button_activa_respuesta_radicado_Click(sender As Object, e As EventArgs) Handles Button_activa_respuesta_radicado.Click
        Dim Mens As New Classscrripjava
        Try
            Me.Hidden_id_respuesta.Value = -1
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Me.hdnEmailID.Value
            If Session.Item("RESPUESTA_TRAMITE") = 0 Then
                Mens.Showscripman_menu("El usuario no tiene permiso para responder el trámite", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If _
             HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                Mens.Showscripman_menu("Debe seleccionar el tramite a responder", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = "0" Then
                Mens.Showscripman_menu("El usuario workflow no tiene usuario de gestión relacionado", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                                 Radicado)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            If Radicado = "" Then
                Mens.Showscripman_menu("La tarea seleccionada no tiene radicado relacionado", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim refclas_resp As New Classgestionrespuesta
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Dim id_respuesta As Integer = 0
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                               HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                               id_respuesta)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            Dim Refclas_gestion_resp As New Classgestionrespuesta
            Result = Refclas_gestion_resp.Actualiza_ruta_workflow_respuesta_radicado(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                         Radicado)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            If id_respuesta = 0 Then
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                          id_respuesta)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                    Exit Sub
                End If
                If id_respuesta = 0 Then
                    Mens.Showscripman_menu("El radicado actual no tiene una respuesta relacionada", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Mens.Showscripman_menu("El usuario no tiene asiganda la tarea para gestionar la respuesta", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Me.Hidden_radicado.Value = Radicado
            Me.Hidden_id_respuesta.Value = id_respuesta
            If id_respuesta = 0 Then
                Mens.Showscripman("El radicado actual no tiene una respuesta relacionada", Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                            estru)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            If estru.ID_REMIT_DEST_INT = 0 Then
                Result = Refclas_gestion_resp.Reasigna_respuesta_sistema_usuario(id_respuesta,
                                                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                 Radicado,
                                                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                    Exit Sub
                End If
                estru.ID_REMIT_DEST_INT = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            End If
            If estru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                Result = Refclas_gestion_resp.Reasigna_respuesta_sistema_usuario(id_respuesta,
                                                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                 Radicado,
                                                                                 estru.ID_REMIT_DEST_INT)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                    Exit Sub
                End If
            End If
            Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION") = id_respuesta
            label_title.Text = "Respuesta al radicado (" & Radicado & ") código (" & id_respuesta & ") Peticionario (" & estru.DESTINATARIO & ") Tramite (" & estru.TRAMITE_DOCUMENTO & ")"
            'UpdatePanel_titulo_respuesta.Update()
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
                    Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                    Exit Sub
                End If
                Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                                     estado_obligatorio)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                    Exit Sub
                End If
                Me.Hidden_obliga_rep.Value = estado_obligatorio
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
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            Dim correo_electronico As String = ""
            Dim refclasradicado As New ClassRadicador
            Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(estru.codigo_dest_externo,
                                                                                           correo_electronico,
                                                                                           estru.system_plantilla_radicado_id_plantilla)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            Me.Hidden_remitente_destinatario.Value = estru.codigo_dest_externo
            Result = Refclas_gestion_resp.Actualiza_estados_general_semaforo(id_respuesta,
                                                                                 Me.Page)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            UpdatePanel_descarga_hml_dowload.Update()
            ModalPopup_respuesta_radicado.Show()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_respuesta_radicado)
            Exit Sub
        End Try
    End Sub
    Private Sub Button_valida_Cerrar_respuesta_radicado_Click(sender As Object, e As EventArgs) Handles Button_valida_Cerrar_respuesta_radicado.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclass As New Class_DAT_ADIC_TAR
            Result = Refclass.Solicita_estado_tramite_tarea_workflow(Session.Item("WF_RUTAWORKFLOW"), _
                                                                     Val(Me.hdnEmailID.Value), _
                                                                     Me.Hidden_estado_tramite.Value)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_valida_cerra_respuesta_radicado)
            End If
            ModalPopup_respuesta_radicado.Hide()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_valida_cerra_respuesta_radicado)
        End Try
    End Sub

    Private Sub ImageButtonterminar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonterminar.Click
        Dim Mens As New Classscrripjava
        Try
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Me.hdnEmailID.Value
            If Me.Hidden_men_result.Value = 0 Then Exit Sub
            If _
             HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                Mens.Showscripman("Debe seleccionar el tramite a terminar", Me.updatemenu)
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New Class_Listado_Actividades_workflow
            Dim id_actividad As Integer = 0
            Result = Refclas.Solicita_actividad_workflow_final(id_actividad)
            If Result <> "YES" Then
                Me.Hidden_men_result.Value = 0
                Mens.Showscripman(Result, Me.updatemenu)
                Exit Sub
            End If
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            Result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                Me.Hidden_men_result.Value = 0
                Mens.Showscripman(Result, Me.updatemenu)
                Exit Sub
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")), _
                                                                                         Estado_solicitud_aprobacion, _
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Me.Hidden_men_result.Value = 0
                Mens.Showscripman(Result, Me.updatemenu)
                Exit Sub
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Me.Hidden_men_result.Value = 0
                Mens.Showscripman("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.updatemenu)
                Exit Sub
            End If
            Dim Refclas_workflow As New ClassWorkflow
            Result = Refclas_workflow.Terminar_Tarea_Workflow_Bacth("", _
                                                                    id_actividad.ToString, _
                                                                    0, _
                                                                    HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                    "", _
                                                                    0, _
                                                                    0, _
                                                                    0, _
                                                                    0, _
                                                                    "", _
                                                                    0)
            If Result <> "YES" Then
                Me.Hidden_men_result.Value = 0
                Mens.Showscripman(Result, Me.updatemenu)
                Exit Sub
            Else
                Me.ModalPopupExtender_visor_externo.Hide()
                Me.Hidden_men_result.Value = 1
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.updatemenu)
        End Try
    End Sub

    Private Sub ImageButtonEnviarUsuario_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonEnviarUsuario.Click
        Dim Mens As New Classscrripjava
        Try
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Me.hdnEmailID.Value
            If _
             HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                Mens.Showscripman("Debe seleccionar el tramite a terminar", Me.updatemenu)
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_reasigna_tramite_usuario.Show()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.updatemenu)
        End Try
    End Sub



    Private Sub Button_me_active_men_dive_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Ref_class As New Class_gestion_correspondencia
            Dim Result As String = ""
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Me.hdnEmailID.Value
            Result = Ref_class.Seleccion_menu_tramite(Me.Hidden_menu_var_event_dive.Value, _
                                                      Page, _
                                                      HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                Refcriptman.Showscripman(Result, Me.UpdatePanel_menu_var_event)
                Exit Sub
            End If
        Catch ex As Exception
            Refcriptman.Showscripman(ex.Message, Me.UpdatePanel_menu_var_event)
        End Try
    End Sub
    Protected Sub Button_autoriza_confirma_reasigna_Click(sender As Object, e As EventArgs) Handles Button_autoriza_confirma_reasigna.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim id_usuario_autoriza As Integer = 0
            If Session.Item("ID_TAREA_SELECCIONDA") = "0" Or Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                Me.ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Hide()
                clasjava.Showscripman("Por favor seleccione una tarea para reasignar la respuesta", Me.UpdatePanel_contenido_confirma_reasigna_responsable_tramite)
                Exit Sub
            End If
            Dim Resultado_correo As String = ""
            Result = Refclas.Reasigna_respuesta_tarea_manual(Session.Item("ID_TAREA_SELECCIONDA"), _
                                                             Resultado_correo, _
                                                             Me.Page, _
                                                             Me.TextBox_login_autoriza_reasigna.Text, _
                                                             id_usuario_autoriza)
            If Result <> "YES" Then
                Me.ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Hide()
                clasjava.Showscripman(Result, Me.UpdatePanel_contenido_confirma_reasigna_responsable_tramite)
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Hide()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_contenido_confirma_reasigna_responsable_tramite)
        End Try
    End Sub
    Protected Sub Button_cancela_confirma_reasigna_Click(sender As Object, e As EventArgs) Handles Button_cancela_confirma_reasigna.Click
        Me.ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Hide()
    End Sub
    Protected Sub Button_autoriza_reasigna_Click(sender As Object, e As EventArgs) Handles Button_autoriza_reasigna.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim id_usuario_autoriza As Integer = 0
            If Session.Item("ID_TAREA_SELECCIONDA") = "0" Or Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                clasjava.Showscripman("Por favor seleccione una tarea para reasignar la respuesta", _
                                      Me.UpdatePanel_contenido_reasigna_responsable_tramite)
                Exit Sub
            End If
            Result = Refclas.Valida_usuario_administrador_general(Me.TextBox_login_autoriza_reasigna.Text, _
                                                                  Me.TextBox_pasw_autoriza_reasigna.Text, _
                                                                  id_usuario_autoriza, _
                                                                  "reasigna_documento")
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_contenido_reasigna_responsable_tramite)
                Exit Sub
            End If

            Dim Resultado_correo As String = ""
            Result = Refclas.Reasigna_respuesta_tarea_manual(Session.Item("ID_TAREA_SELECCIONDA"), _
                                                             Resultado_correo, _
                                                             Me.Page, _
                                                             Me.TextBox_login_autoriza_reasigna.Text, _
                                                             id_usuario_autoriza)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_contenido_reasigna_responsable_tramite)
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_reasigna_responsable_tramite.Hide()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_contenido_reasigna_responsable_tramite)
        End Try
    End Sub

    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        Try
            Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.hdnEmailID.Value
            Me.Iframe_visor_externo_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_visor_externo.Update()
            Me.ModalPopupExtender_visor_externo.Show()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ImageButtonactualizar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonactualizar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim campo_radicado As String = ""
            Dim Result As String = ""
            Dim Ref_class As New Class_configuracion_listado_ruta
            Result = Ref_class.SolicitaNombreCampoRadicadoRuta(Session.Item("Id_Ruta_Workflow"),
                                                               campo_radicado)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.updatemenu)
                Exit Sub
            End If
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Session.Item("SortExpression_compartido") = "etw.fecha_inicio"
            Session.Item("SortDirection_compartido") = "DESC"
            Result = reflcas_respuesta.Lista_tramites(Session.Item("Id_Usuario_Workflow"), _
                                                      Session.Item("Id_Ruta_Workflow"), _
                                                      Session.Item("Id_Grupo_Workflow"), _
                                                      Session.Item("WF_ID_ACTIVIDAD"), _
                                                      HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE"), _
                                                      Session.Item("WF_RUTAWORKFLOW"), _
                                                      Me.data_grid_listado_solicitudes, _
                                                      HiddenEmailconsulta, _
                                                      Me.Label_titulo_listado_solicitudes, _
                                                      Me.hdnEmailID, _
                                                      UpdateGeneral, _
                                                      "Todos", _
                                                      Me.UpdatePanel_title, _
                                                      1, _
                                                      "", _
                                                       Session.Item("SortExpression_compartido"), _
                                                       Session.Item("SortDirection_compartido"), _
                                                      Me.Hidden_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.updatemenu)
                Exit Sub
            Else
                Label_anunciado_filtro.Text = "Todos"
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.updatemenu)
        End Try
    End Sub
    'Protected Sub Button_confirma_reversar_Click(sender As Object, e As EventArgs) Handles Button_confirma_reversar.Click
    '    Dim clasjava As New Classscrripjava
    '    Try
    '        Dim Result As String = ""
    '        Dim id_imagen_plantilla As Integer = 0
    '        Dim radicado_respuesta As Integer = 0
    '        Dim fecha_respuesta As Integer = 0
    '        Dim id_imagen_respuesta As Integer = 0
    '        Dim estado_envio_respuesta As Integer = 0
    '        Dim Radicado As String = ""
    '        Dim Refclas_ As New ClassWorkflow
    '        Me.Hidden_con_ref.Value = ""
    '        Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
    '        Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
    '                                                                             Radicado)
    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.UpdatePanel_confirma_reversa)
    '            Exit Sub
    '        End If
    '        Dim estru As stru_envio = Nothing
    '        Dim refclas_resp As New Classgestionrespuesta
    '        Dim id_respuesta As Integer = 0
    '        Result = refclas_resp.Retorna_id_respuesta_radicado(Radicado, _
    '                                                            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
    '                                                            id_respuesta)
    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.UpdatePanel_confirma_reversa)
    '            Exit Sub
    '        End If
    '        Dim Refclas As New Classgestionrespuesta
    '        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
    '        Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta, _
    '                                                                            id_imagen_plantilla, _
    '                                                                            radicado_respuesta, _
    '                                                                            fecha_respuesta, _
    '                                                                            id_imagen_respuesta, _
    '                                                                            estado_envio_respuesta)
    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.UpdatePanel_confirma_reversa)
    '            Exit Sub
    '        End If
    '        Dim stru As stru_envio = Nothing
    '        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
    '        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, stru, 1)
    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.UpdatePanel_confirma_reversa)
    '            Exit Sub
    '        End If
    '        Result = Refclas.Reversa_respuesta_radicado(stru, _
    '                                                    Session.Item("GA_LOGINUSUARIOGESTION"), _
    '                                                    Session.Item("GA_IDUSUARIOGESTION"))
    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.UpdatePanel_confirma_reversa)
    '            Exit Sub
    '        End If
    '        Me.Hidden_con_ref.Value = "YES"
    '    Catch ex As Exception
    '        clasjava.Showscripman(ex.Message, Me.UpdatePanel_confirma_reversa)
    '    Finally
    '        Me.ModalPopupExtender_edition_confirma_reversa_respuesta.Hide()
    '    End Try
    'End Sub


    Protected Sub ImageButton_guarda_lista_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_guarda_lista.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            Dim dat As Date
            dat = Now
            If Me.Hidden_colum_header.Value = "" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman("No hay resultados para exportar", Me.UpdatePanel_busqueda)
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((100 * Rnd()) + 1))
            Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + "/reportes/" + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + "/")
            If Directory.Exists(ruta_create) = False Then
                Directory.CreateDirectory(ruta_create)
            End If
            Dim Ref As New ClassReportesRadicado
            Dim nombre_reporte As String = "LISTADO DE TRAMITES Y RESPUESTAS"
            Dim ruta_archivo As String = ruta_create + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + value.ToString + "test.xls"
            Result = Refclasreposte.genera_xls_paginacion(Me.data_grid_listado_solicitudes, ".xls", _
                                               ruta_archivo, _
                                               Hidden_colum_header.Value, _
                                               nombre_reporte, _
                                               Session.Item("GA_LOGINUSUARIOGESTION"), _
                                               HttpContext.Current.Session.Item("dat_gred_cahce"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.UpdatePanel_busqueda)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + "/reportes/" + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + "/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & value.ToString + "test.xls"
                    Me.ifmExcel_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                    Me.updatapanel_iframe.Update()
                    Me.update_botonoes_opciones_solicitud_general.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_busqueda)
        End Try
    End Sub

    Private Sub ImageButtonConfirmar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonConfirmar.Click
        Dim Mens As New Classscrripjava
        Try
            Me.Hidden_id_respuesta_.Value = -1
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Me.hdnEmailID.Value
            If Session.Item("RESPUESTA_TRAMITE") = 0 Then
                Mens.Showscripman_menu("El usuario no tiene permiso para responder el trámite", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If _
             HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                Mens.Showscripman_menu("Debe seleccionar el tramite a responder", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = "0" Then
                Mens.Showscripman_menu("El usuario workflow no tiene usuario de gestión relacionado", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                                 Radicado)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            If Radicado = "" Then
                Mens.Showscripman_menu("La tarea seleccionada no tiene radicado relacionado", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If


            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Dim id_respuesta As Integer = 0
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                               HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                               id_respuesta)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            If id_respuesta = 0 Then
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                          id_respuesta)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                    Exit Sub
                End If
                If id_respuesta = 0 Then
                    Mens.Showscripman("El radicado actual no tiene una respuesta relacionada", Me.UpdatePanel_respuesta_radicado)
                    Exit Sub
                Else
                    Mens.Showscripman("El usuario no tiene asiganda la tarea para gestionar la respuesta", Me.UpdatePanel_respuesta_radicado)
                    Exit Sub
                End If
            End If
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                        estru)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            Dim correo_electronico As String = ""
            Dim refclasradicado As New ClassRadicador
            Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(estru.codigo_dest_externo, _
                                                                                       correo_electronico, _
                                                                                       estru.system_plantilla_radicado_id_plantilla)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            Me.Hidden_id_respuesta_.Value = id_respuesta
            Me.ModalPopupExtender_edition_confirma_respuesta.Show()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_respuesta_radicado)
        End Try
    End Sub


    Protected Sub ImageButton_filter_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_filter.Click
        Try
            Me.ModalPopupExtender_edition_filtro_historico.Show()
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub Button_consultar_Click(sender As Object, e As EventArgs) Handles Button_consultar.Click

        Dim clasjava As New Classscrripjava
        Try

            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Por tramitar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "En tramite"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Tramitado"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Solicitud por aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Solicitud aprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Solicitud desaprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "8" Then
                selecion_name = "Solicitud archivada"
            End If
            If Me.Hidden_lik_service_boton.Value = "9" Then
                selecion_name = "Solicitud anulada"
            End If
            If Me.Hidden_lik_service_boton.Value = "10" Then
                selecion_name = "Tramitado archivado"
            End If
            Dim Valor_consulta As String = ""
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Dim Result As String = ""
            Result = reflcas_respuesta.Solicita_parametro_consulta_filtro(Me.TextBox_fecha_ini_asigna.Text, _
                                                                        Me.TextBox_fecha_fin_asigna.Text, _
                                                                        Me.TextBox_fecha_ini_final_tramite.Text, _
                                                                        Me.TextBox_fecha_fin_final_tramite.Text, _
                                                                        Valor_consulta)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_filtro_historico)
                Exit Sub
            End If
            If Valor_consulta = "" Then
                clasjava.Showscripman("Debe seleccionar algún criterio", UpdatePanel_filtro_historico)
                Exit Sub
            End If
            Result = reflcas_respuesta.Lista_tramites(Session.Item("Id_Usuario_Workflow"), _
                                                     Session.Item("Id_Ruta_Workflow"), _
                                                     Session.Item("Id_Grupo_Workflow"), _
                                                     Session.Item("WF_ID_ACTIVIDAD"), _
                                                     HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE"), _
                                                     Session.Item("WF_RUTAWORKFLOW"), _
                                                     Me.data_grid_listado_solicitudes, _
                                                     HiddenEmailconsulta, _
                                                     Me.Label_titulo_listado_solicitudes, _
                                                     Me.hdnEmailID, _
                                                     UpdateGeneral, _
                                                     selecion_name, _
                                                     Me.UpdatePanel_title, _
                                                     3, _
                                                     Valor_consulta, _
                                                     Session.Item("SortExpression_compartido"), _
                                                     Session.Item("SortDirection_compartido"), _
                                                     Me.Hidden_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_filtro_historico)
            Else
                Me.ModalPopupExtender_edition_filtro_historico.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_filtro_historico)
        End Try

    End Sub

    Protected Sub Button_cancela_autoriza_reasignacion_Click(sender As Object, e As EventArgs) Handles Button_cancela_autoriza_reasignacion.Click
        Me.ModalPopupExtender_edition_autoriza_reasignacion_tarea.Hide()
    End Sub

    'Protected Sub Button_cancela_reasignar_Click(sender As Object, e As EventArgs) Handles Button_cancela_reasignar.Click
    '    Me.ModalPopupExtender_edition_reasigna_tramite_usuario.Hide()
    'End Sub

    Protected Sub Button_cancela_solo_confirma_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Me.ModalPopupExtender_edition_confirma_respuesta.Hide()
    End Sub

    Protected Sub Button_cancela_autoriza_reasigna_Click(sender As Object, e As EventArgs) Handles Button_cancela_autoriza_reasigna.Click
        Me.ModalPopupExtender_edition_reasigna_responsable_tramite.Hide()
    End Sub

    'Protected Sub Button_cancel_confirma_reversar_Click(sender As Object, e As EventArgs) Handles Button_cancel_confirma_reversar.Click
    '    Me.ModalPopupExtender_edition_confirma_reversa_respuesta.Hide()
    'End Sub

    Private Sub Button_tool_activa_detalle_radicado_seleccion_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_detalle_radicado_seleccion.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_estados_modulo_rad As New Class_estados_modulo_radicacion
            If hdnEmailID.Value = "-1" Or hdnEmailID.Value = "0" Then
                Mens.Showscripman_menu("Debe seleccionar la tarea para ver la información", UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(hdnEmailID.Value, _
                                                                                    radicado)
            If Result <> "YES" Then
                Mens.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_class_detalle_plantilla_rad As New Class_ra_detalle_plantilla_radicado
            Result = ref_class_detalle_plantilla_rad.Genera_interface_detalle_radicado(radicado, _
                                                                                       Me.Page)
            If Result <> "YES" Then
                Mens.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_detalle_radicado.Show()
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub ImageButtonanotacion_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonanotacion.Click
        Dim refclsjava As New Classscrripjava
        Try
            Dim Result As String = ""
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Me.hdnEmailID.Value
            If Session.Item("ID_TAREA_SELECCIONDA") = "0" Then Exit Sub
            'If HttpContext.Current.Session("Interactuar_Anotaciones") = "0" Then
            '    refclsjava.Showscripman("El usuario no tiene permiso para interactuar con anotaciones ", UpdatePanel_boton_tool)
            '    Exit Sub
            'End If
            Dim refclas As New Class_anotacion_tarea
            Result = refclas.Listar_Anotaciones_tarea_workflow(Me.GridView_lista_notas, _
                                                               HttpContext.Current.Session("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                refclsjava.Showscripman(Result, UpdatePanel_boton_tool)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_content_anotacion.Show()
                Exit Sub
            End If

        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub


    Private Sub GridView_lista_notas_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView_lista_notas.RowCreated
        Try
            e.Row.Cells(2).Visible = False
            e.Row.Cells(1).Visible = False
            e.Row.Cells(5).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_activa_lista_imagenes_gestion_corresponencia_Click(sender As Object, e As EventArgs) Handles Button_activa_lista_imagenes_gestion_corresponencia.Click
        Dim refmensaje As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_seleccion_tarea As New Classselecciotarea
            Result = Refclas_seleccion_tarea.Lista_imagenes_gestion_de_correspondencia(Val(Me.hdnEmailID.Value), Me.Page)
            If Result <> "YES" Then
                refmensaje.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = ""
                HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = 0
                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Val(Me.hdnEmailID.Value)
                ModalPopupExtender_edition_lista_imagenes_gestion.Show()
            End If
        Catch ex As Exception
            refmensaje.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub GridView_list_documento_relacion_wf_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_list_documento_relacion_wf.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
            e.Row.Cells(5).Visible = False
        Catch ex As Exception
        End Try
    End Sub
    Private Sub Button_selecion_treview_documento_Click(sender As Object, e As EventArgs) Handles Button_selecion_treview_documento.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Valor_Tab_Selccion As String = hiden_seleccion_documento_wf.Value
            Dim Result As String = ""
            Dim Refclas As New ClassVisualisaDocumento
            Result = Refclas.Visualiza_documento_workflow_visor(Valor_Tab_Selccion,
                                                                Me.ifrm_visor_,
                                                                Me.Panel_indice,
                                                                Me.UpdatePanelindice,
                                                                Me.UpdatePanelVisor,
                                                                1, HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                1,
                                                                Me.HiddenHeigth,
                                                                Me.Page, Matri_Doc_Visual,
                                                                                   Doc_actual)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_seleccion_treview)
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_seleccion_treview)
        End Try
    End Sub

    Private Sub WebForm_interface_gestion_tramite_Init(sender As Object, e As EventArgs) Handles Me.Init
        ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
        ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
    End Sub
    'ZONA VISOR NEODIAMIC
    Private Sub ImageButtonInicio_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonInicio.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "inicio",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   Me.DropDownList_zom,
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Private Sub ImageButtonFinal_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonFinal.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "final",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   DropDownList_zom,
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageButtonAnterior_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonAnterior.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "-1",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   DropDownList_zom,
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageButtonSiguiente_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonSiguiente.Click
        Try
            Dim clasjava As New Classscrripjava
            Me.ImageButtonSiguiente.Enabled = False
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "+1",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   Me.DropDownList_zom,
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
        Finally
            Me.ImageButtonSiguiente.Enabled = True
        End Try
    End Sub
    Private Sub ImageButton_ir_pagina_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_ir_pagina.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.LabelConteo.Text = "" Then Exit Sub
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, Doc_actual,
                                                                   "seleccion",
                                                                   Val(Me.LabelConteo.Text),
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   Me.DropDownList_zom,
                                                                   Me.Updatepanel_boton_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
            Me.UpdatePanel_conte_bot.Update()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Protected Sub ImageMenos_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMenos.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual, "-",
                                                                          Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                          DropDownList_zom,
                                                                          Updatepanel_boton_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageMas_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMas.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual,
                                                                          "+",
                                                                          Me,
                                                                          HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                          DropDownList_zom,
                                                                          Updatepanel_boton_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)

            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Private Sub DropDownList_zom_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_zom.SelectedIndexChanged
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale_zom(Matri_Doc_Visual, DropDownList_zom.SelectedValue,
                                                                              Me,
                                                                              HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                              HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                              HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                              DropDownList_zom,
                                                                              Me.UpdatePanel_drows_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)

            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Private Sub ImageRotate45_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate45.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 90, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), HttpContext.Current.Session.Item("WF_IMAGE_WITH"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageRotate180_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate180.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 180, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), HttpContext.Current.Session.Item("WF_IMAGE_WITH"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageRotate270_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate270.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 270, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), HttpContext.Current.Session.Item("WF_IMAGE_WITH"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    'Activa para subir documento relacionado
    Private Sub Button_tool_adjunta_documento_relacionado_Click(sender As Object, e As EventArgs) Handles Button_tool_adjunta_documento_relacionado.Click
        Dim refclas_java As New Classscrripjava
        Try
            If Session.Item("ID_TAREA_SELECCIONDA") = -1 Or Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_USUARIO") = 0 Then
                refclas_java.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", UpdatePanel_boton_tool)
                Exit Sub
            End If

            Dim Result As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                                            Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                refclas_java.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                refclas_java.Showscripman_menu("Imposible econtrar la imagen relacionada a la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = structure_gabinete_workflow.NOMBRE_GABINETE
            If HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = 0 Then
                HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = structure_datos_tarea_workflow.ID_IMAGEN
            End If
            Me.Check_anexo_radicado_adj.Visible = False
            Me.Check_anexo_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Visible = True
            Me.CheckBox_relacionado_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Checked = True
            Me.Update_actualiza_adjunta_documento.Update()
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_lista As String = ""
            'Me.Buttonaceptar.Enabled = True
            Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
            AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
            Dim Ref_class As New ClassGaProducionDocumental
            Dim Extension_permitida As String = ""
            Result = Ref_class.Solicita_listado_extension_de_archivos_permitidas(Extension_permitida)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Extension_permitida = "" Then
                refclas_java.Showscripman_menu("El sistema no registra extensiones permitidas", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            AjaxFileUpload_dowload.AllowedFileTypes = Extension_permitida & ",TIFF"
            '------------------------------------------
            'Verfica lista existencia lista de chequeo
            '------------------------------------------
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"),
                                                                               estado_lista)
            If Result <> "YES" Then
                refclas_java.Showscripman(Result, UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            If estado_lista = "YES" Then
                If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                    Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                     Session.Item("DG_TIPO_TRAMITE"),
                                                                                     Session.Item("DG_ID_CONFIG_DIGITALIZACION"), 0)
                    If Result <> "YES" Then
                        refclas_java.Showscripman(Result, UpdatePanel_boton_tool)
                        Exit Sub
                    End If
                End If
                Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
                Dim estado_resultado As String = ""

                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                                Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                                "",
                                                                                                                                Me.DropDownList_adjunta_documento,
                                                                                                                                Me.Update_actualiza_adjunta_documento,
                                                                                                                                estado_resultado)
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = 1
                    Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                    Me.ModalPopupExtender_sube_documento_adjunto.Show()
                End If
            Else
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
                UpdatePanel_descarga.Update()
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            refclas_java.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub ImageButtonguardardocumento_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonguardardocumento.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            If HttpContext.Current.Session.Item("WF_TAGSELECCION") = "" Then
                scri.Showscripman("Debe seleccionar un documento ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Tag_Seleccion() As String = HttpContext.Current.Session.Item("WF_TAGSELECCION").ToString.Split("|")
            Dim gabinete_consulta As String = Tag_Seleccion(5)
            Dim id_imagen As Integer = Tag_Seleccion(2)
            Result = refgabinete.SolicitaPermisosGeneralesGabinete(gabinete_consulta,
                                                                   HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                   stru_permiso)
            If Result <> "YES" Then
                scri.Showscripman("Imposible guardar " & Result, Me.Updatepanel_boton_content)
                Exit Sub
            End If
            If stru_permiso.GUARDAR_IMAGEN = 0 Then
                scri.Showscripman("El usuario no tiene permisos para guardar imagenes desde gabinete ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            Session.Item("RA_RUTA_IMPRESION_FINAL") = ""
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    'ReDim Preserve Matri_Doc_Visual(i)
                    If i = 1 Then
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Matri_Temp(i)
                    Else
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Session.Item("RA_RUTA_IMPRESION_FINAL") & "," & Matri_Temp(i)
                    End If
                Next
            End If
            If Session.Item("RA_RUTA_IMPRESION_FINAL") = "" Then
                scri.Showscripman("Imposible consultar descargar la matriz de documentos esta vacia ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
            Session.Item("DA_GABINETE_IMPRESION") = gabinete_consulta
            Session.Item("DA_ID_IMAGEN_IMPRESION") = id_imagen
            Iframe_guardar.Attributes.Add("src", "../Docuarchi/WebFormDaExportArchivo.aspx")
            Me.ModalPopupExtender_guardar.Show()
            UpdatePane_guardar.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, Updatepanel_boton_content)
        End Try

    End Sub
    Private Sub ImageButtonimprimir_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonimprimir.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            If HttpContext.Current.Session.Item("WF_TAGSELECCION") = "" Then
                scri.Showscripman("Debe seleccionar un documento ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Tag_Seleccion() As String = HttpContext.Current.Session.Item("WF_TAGSELECCION").ToString.Split("|")
            Dim gabinete_consulta As String = Tag_Seleccion(5)
            Dim id_imagen As Integer = Tag_Seleccion(2)
            Result = refgabinete.SolicitaPermisosGeneralesGabinete(gabinete_consulta,
                                                                   HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                   stru_permiso)
            If Result <> "YES" Then
                scri.Showscripman("Imposible imprimir " & Result, Me.Updatepanel_boton_content)
                Exit Sub
            End If
            If stru_permiso.IMPRI_IMAGEN = 0 Then
                scri.Showscripman("El usuario no tiene permisos para imprimir en el gabinete ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            Session.Item("RA_RUTA_IMPRESION_FINAL") = ""
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    'ReDim Preserve Matri_Doc_Visual(i)
                    If i = 1 Then
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Matri_Temp(i)
                    Else
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Session.Item("RA_RUTA_IMPRESION_FINAL") & "," & Matri_Temp(i)
                    End If

                Next
            End If

            If Session.Item("RA_RUTA_IMPRESION_FINAL") = "" Then
                scri.Showscripman("Imposible consultar imprimir la matriz de documentos esta vacia ", Me.Updatepanel_boton_content)
                Exit Sub
            End If

            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
            Session.Item("DA_GABINETE_IMPRESION") = gabinete_consulta
            Session.Item("DA_ID_IMAGEN_IMPRESION") = id_imagen
            Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL") = Matri_Doc_Visual(HttpContext.Current.Session.Item("WF_DOC_ACTUAL"))

            Me.ModalPopupExtenderimpre_post.Show()
            UpdatePaneliframe_post.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, Updatepanel_boton_content)
        End Try
    End Sub
    '--------------------------------------------------------------
    'Activa ventana para subir documentos al visor del documento
    '-------------------------------------------------------------
    Private Sub Button_tool_activa_sube_documento_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Session.Item("ID_TAREA_SELECCIONDA") = Val(Me.hdnEmailID.Value)
            If Session.Item("ID_TAREA_SELECCIONDA") = -1 Or Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                                            Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")",
                                           Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownList_adjunta_documento.Items.Clear()
            'Me.UpdatePane_adjunta_autamatico_documento.Update()
            Dim nombre_gabinete As String = ""
            Dim id_imagen As Integer = 0
            If HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = "" Then
                HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = structure_gabinete_workflow.NOMBRE_GABINETE
                nombre_gabinete = structure_gabinete_workflow.NOMBRE_GABINETE
            Else
                nombre_gabinete = HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO")
            End If
            If HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = 0 Then
                HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = structure_datos_tarea_workflow.ID_IMAGEN
                id_imagen = structure_datos_tarea_workflow.ID_IMAGEN
            Else
                id_imagen = HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO")
            End If
            Dim Ref_class As New ClassGaProducionDocumental
            Dim Extension_permitida As String = ""
            Result = Ref_class.Solicita_listado_extension_de_archivos_permitidas(Extension_permitida)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Extension_permitida = "" Then
                clasjava.Showscripman_menu("El sistema no registra extensiones permitidas", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            AjaxFileUpload_dowload.AllowedFileTypes = Extension_permitida & ",TIFF"
            '----------------------------------------
            'Solicita extensión del archivo
            '----------------------------------------
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(nombre_gabinete,
                                                                                          id_imagen,
                                                                                          stru_paramter_image)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Class_da_extension As New Class_da_extension
            Dim extension As String = ""
            Result = Class_da_extension.RetornaExtensionTipoDocumento(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                      extension)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If extension = ".TIF" Or extension = ".BMP" Or extension = ".JPG" Then
                Me.Check_anexo_radicado_adj.Visible = True
                Me.Check_anexo_radicado_adj.Enabled = True
                Me.CheckBox_relacionado_radicado_adj.Visible = True
                Me.CheckBox_relacionado_radicado_adj.Enabled = True
                Me.h_adjunto_adjunto_doc_visor.Visible = True
                Me.Update_actualiza_adjunta_documento.Update()
            Else
                Me.Check_anexo_radicado_adj.Visible = False
                Me.Check_anexo_radicado_adj.Enabled = False
                Me.CheckBox_relacionado_radicado_adj.Visible = True
                Me.CheckBox_relacionado_radicado_adj.Enabled = False
                Me.CheckBox_relacionado_radicado_adj.Checked = True
                Me.h_adjunto_adjunto_doc_visor.Visible = False
                Me.Update_actualiza_adjunta_documento.Update()
            End If
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_lista As String = ""
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"),
                                                                               estado_lista)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            If estado_lista = "YES" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                 0)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                           Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                           "",
                                                                                                                           Me.DropDownList_adjunta_documento,
                                                                                                                           Me.Update_actualiza_adjunta_documento,
                                                                                                                           estado_resultado)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
                Session.Item("DG_LISTA_CHEQUEO") = 1
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_tool_activa_sube_documento_lista_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento_lista.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Session.Item("ID_TAREA_SELECCIONDA") = Val(Me.hdnEmailID.Value)
            If Session.Item("ID_TAREA_SELECCIONDA") = -1 Or Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                                            Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")",
                                           Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownList_adjunta_documento.Items.Clear()
            Dim nombre_gabinete As String = ""
            Dim id_imagen As Integer = 0
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO_LISTA_RESPUESTA") = structure_gabinete_workflow.NOMBRE_GABINETE
            HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_LISTA_RESPUESTA") = structure_datos_tarea_workflow.ID_IMAGEN
            Dim Ref_class As New ClassGaProducionDocumental

            Me.Check_anexo_radicado_adj.Visible = False
            Me.Check_anexo_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Visible = True
            Me.CheckBox_relacionado_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Checked = True
            Me.h_adjunto_adjunto_doc_visor.Visible = False
            Me.Update_actualiza_adjunta_documento.Update()
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_lista As String = ""
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"),
                                                                               estado_lista)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            If estado_lista = "YES" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                 0)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                           Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                           "",
                                                                                                                           Me.DropDownList_adjunta_documento,
                                                                                                                           Me.Update_actualiza_adjunta_documento,
                                                                                                                           estado_resultado)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Session.Item("WF_TIPO_ADJUNTA") = "LISTA"
                Session.Item("DG_LISTA_CHEQUEO") = 1
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Session.Item("WF_TIPO_ADJUNTA") = "LISTA"
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
            Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
            If Session.Item("WF_TIPO_ADJUNTA") = "ESCANER" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
                Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "VISOR" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
                Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "LISTA" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
                Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "ENLACE" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If

                Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
                Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            End If
        Catch ex As Exception
            Session.Item("WF_ERROR_RESPUESTA") = ex.Message
        End Try
    End Sub
    Private Sub Button_guardar_desicion_Click(sender As Object, e As EventArgs) Handles Button_guardar_desicion.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAlmacenamiento
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            Me.Hidden_result_load.Value = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                CLAS.Showscripman(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga)
                Exit Sub
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "ESCANER" Then
                Me.Hidden_result_load.Value = ""
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                If Me.DropDownList_adjunta_documento.SelectedValue = "" Then
                    Session.Item("DG_LISTA_CHEQUEO") = "-1"
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = Me.DropDownList_adjunta_documento.SelectedValue
                End If
                Dim ID_DOCUMENTO As Integer = 0
                Dim TIPO_DOCUMENTO As Integer = 0
                Dim datos_image As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Dim ref_ClassAlmacenamiento As New ClassAlmacenamiento
                Dim ob As Object = Nothing
                Result = ref_ClassAlmacenamiento.Almacenamiento_Documentos_Digitalizados("",
                                                                                        ID_DOCUMENTO,
                                                                                        TIPO_DOCUMENTO,
                                                                                        ob,
                                                                                        stru_datos_image_lista,
                                                                                        Session.Item("DG_TIPODIGITALIZACION"),
                                                                                        1)
                If Result <> "YES" Then
                    CLAS.Showscripman_menu(Result, Me.UpdatePanel_descarga, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.Hidden_result_load.Value = "YES"
                Me.Hidden_date_row.Value = datos_image
                Me.ModalPopupExtender_sube_documento_adjunto.Hide()
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "LISTA" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                If Me.CheckBox_relacionado_radicado_adj.Checked = True Then
                    Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Session.Item("ID_TAREA_SELECCIONDA"), radicado)
                    Dim id_imagen As Integer = 0
                    If Val(Me.DropDownList_adjunta_documento.SelectedValue) = -1 Or Val(Me.DropDownList_adjunta_documento.SelectedValue) = 0 Then
                        Result = Refclas.Adjunta_donumento_relacionado(Me.Page,
                                                                       id_imagen,
                                                                       HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO_LISTA_RESPUESTA"),
                                                                       HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_LISTA_RESPUESTA"),
                                                                       Val(Me.DropDownList_adjunta_documento.SelectedValue),
                                                                       Session.Item("ID_TAREA_SELECCIONDA"),
                                                                       radicado,
                                                                       stru_datos_image_lista,
                                                                       0)
                        If Result <> "YES" Then
                            CLAS.Showscripman(Result, UpdatePanel_descarga)
                            Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                            Exit Sub
                        Else
                            Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        End If
                        Me.Hidden_result_load.Value = "YES"
                        If stru_datos_image_lista.notipodocumento = "" Then
                            Dim nun_doc As Object = Val(Hidden_numero_doc_rel_wf.Value) + 1
                            stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                        End If
                        Me.Hidden_date_row.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                       "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & Session.Item("ID_TAREA_SELECCIONDA")
                    Else
                        Result = Refclas.Adjunta_donumento_relacionado(Me.Page,
                                                                       id_imagen,
                                                                       HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO_LISTA_RESPUESTA"),
                                                                       HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_LISTA_RESPUESTA"),
                                                                       Val(Me.DropDownList_adjunta_documento.SelectedValue),
                                                                       Session.Item("ID_TAREA_SELECCIONDA"),
                                                                       radicado,
                                                                       stru_datos_image_lista,
                                                                       0)
                        If Result <> "YES" Then
                            CLAS.Showscripman(Result, UpdatePanel_descarga)
                            ModalPopupExtender_sube_documento_adjunto.Hide()
                            Session.Item("DG_LISTA_CHEQUEO") = -1
                            Exit Sub
                        Else
                            Session.Item("DG_LISTA_CHEQUEO") = -1
                            Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        End If
                        Me.Hidden_result_load.Value = "YES"
                        If stru_datos_image_lista.notipodocumento = "" Then

                            stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                        End If
                        Me.Hidden_date_row.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                       "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & Session.Item("ID_TAREA_SELECCIONDA")
                    End If
                End If
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "VISOR" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                If Me.Check_anexo_radicado_adj.Checked = True Then
                    Result = Refclas.Adjunta_documento_parte_documento(Me.Page)
                    If Result <> "YES" Then
                        CLAS.Showscripman(Result, UpdatePanel_descarga)
                        Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        Exit Sub
                    Else
                        Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                    End If
                End If
                If Me.CheckBox_relacionado_radicado_adj.Checked = True Then
                    Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Session.Item("ID_TAREA_SELECCIONDA"), radicado)
                    Dim id_imagen As Integer = 0
                    If Val(Me.DropDownList_adjunta_documento.SelectedValue) = -1 Or Val(Me.DropDownList_adjunta_documento.SelectedValue) = 0 Then
                        Result = Refclas.Adjunta_donumento_relacionado(Me.Page,
                                                                       id_imagen,
                                                                       HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                       HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                       Val(Me.DropDownList_adjunta_documento.SelectedValue),
                                                                       Session.Item("ID_TAREA_SELECCIONDA"),
                                                                       radicado,
                                                                       stru_datos_image_lista,
                                                                       0)
                        If Result <> "YES" Then
                            CLAS.Showscripman(Result, UpdatePanel_descarga)
                            Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                            Exit Sub
                        Else
                            Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        End If
                        Me.Hidden_result_load.Value = "YES"
                        If stru_datos_image_lista.notipodocumento = "" Then
                            Dim nun_doc As Object = Val(Hidden_numero_doc_rel_wf.Value) + 1
                            stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                        End If
                        Me.Hidden_date_row.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                       "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & Session.Item("ID_TAREA_SELECCIONDA")
                    Else
                        Result = Refclas.Adjunta_donumento_relacionado(Me.Page,
                                                                       id_imagen,
                                                                       HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                       HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                       Val(Me.DropDownList_adjunta_documento.SelectedValue),
                                                                       Session.Item("ID_TAREA_SELECCIONDA"),
                                                                       radicado,
                                                                       stru_datos_image_lista,
                                                                       0)
                        If Result <> "YES" Then
                            CLAS.Showscripman(Result, UpdatePanel_descarga)
                            ModalPopupExtender_sube_documento_adjunto.Hide()
                            Session.Item("DG_LISTA_CHEQUEO") = -1
                            Exit Sub
                        Else
                            Session.Item("DG_LISTA_CHEQUEO") = -1
                            Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        End If
                        Me.Hidden_result_load.Value = "YES"
                        If stru_datos_image_lista.notipodocumento = "" Then

                            stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                        End If
                        Me.Hidden_date_row.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                       "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & Session.Item("ID_TAREA_SELECCIONDA")
                    End If
                End If
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "ENLACE" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                If Me.DropDownList_adjunta_documento.SelectedValue = "" Then
                    Session.Item("DG_LISTA_CHEQUEO") = "-1"
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = Me.DropDownList_adjunta_documento.SelectedValue
                End If
                Dim id_imagen_almacenada As Integer = 0

                Result = Refclas.Almacenamiento_documentos_load_enlace("",
                                                                       1,
                                                                       HttpContext.Current.Session("WF_RUTA_TEMPO_ADJUNTA"),
                                                                       HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                       1,
                                                                       id_imagen_almacenada,
                                                                       stru_datos_image_lista)
                If Result <> "YES" Then
                    CLAS.Showscripman(Result, UpdatePanel_descarga)
                    ModalPopupExtender_sube_documento_adjunto.Hide()
                    Session.Item("DG_LISTA_CHEQUEO") = -1
                    Exit Sub
                Else
                    Me.Hidden_result_load.Value = "YES"
                    If stru_datos_image_lista.notipodocumento = "" Then
                        Dim nun_doc As Object = Val(Hidden_numero_doc_rel_wf.Value) + 1
                        stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                    End If
                    Me.Hidden_date_row.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                   "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE")
                    Session.Item("DG_LISTA_CHEQUEO") = -1
                    Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                End If
            End If
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePanel_descarga)
        End Try
    End Sub
    'Elimina documento lista doumentos workflow
    Private Sub Button_eliminar_documento_Click(sender As Object, e As EventArgs) Handles Button_eliminar_documento.Click
        Dim Mens As New Classscrripjava
        Dim Result As String = ""
        Dim RefclasEliminadoc As New ClassEliminarDocListResult
        Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Try
            Hidden_confir_elimina.Value = ""
            If Me.Hidden_selccion_documento_eliminar_wf.Value = "" Or Me.Hidden_selccion_documento_eliminar_wf.Value = "-1" Then
                Mens.Showscripman_menu("Seleccione el documento a eliminar", Me.UpdatePanel_seleccion_treview, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_tarea As Long = Val(Me.hdnEmailID.Value)
            Dim slplitlist() As String = Me.Hidden_selccion_documento_eliminar_split_wf.Value.ToString.Split("|")
            Dim Estado_relacion As String = "YES"
            Result = Class_DAT_ADIC_TAR.Verifica_relacion_imagen_workflow(Val(Hidden_selccion_documento_eliminar_wf.Value),
                                                                       Session.Item("Id_Ruta_Workflow"),
                                                                       id_tarea,
                                                                       Estado_relacion)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_seleccion_treview)
                Exit Sub
            End If
            If Estado_relacion = "YES" Then
                Mens.Showscripman("El registro se encuentra relacionado a un flujo de trabajo como documento principal, imposible eliminar", Me.UpdatePanel_seleccion_treview)
                Exit Sub
            End If

            Result = RefclasEliminadoc.EliminarDocumentosGabinete(slplitlist(1),
                                                                        0,
                                                                        slplitlist(0),
                                                                        0,
                                                                        1,
                                                                        Session.Item("MASTER_ELIMINA_GABINETE_WORKFLOW"),
                                                                        id_tarea,
                                                                        "GESTIONCORRESPONDENCIA")
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_seleccion_treview)
                Exit Sub
            Else
                Hidden_confir_elimina.Value = "YES"
                Dim refcla As New ClassWorflowVisor
                Dim Resutl As String = ""
                If Val(Me.Hidden_selccion_documento_eliminar_wf.Value) = Val(Me.hiden_seleccion_documento_id_wf.Value) Then
                    Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL", 0)
                    If Resutl <> "YES" Then
                        Mens.Showscripman(Resutl, Me.UpdatePanel_seleccion_treview)
                    End If
                End If
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_seleccion_treview)
        End Try
    End Sub
    'Activa clasificar documento seleción workflow
    Private Sub Button_clasficar_documento_Click(sender As Object, e As EventArgs) Handles Button_clasficar_documento.Click
        Dim refclas_java As New Classscrripjava
        Try
            If Me.Hidden_selccion_documento_cambia_tipo_wf.Value = "" Or Me.Hidden_selccion_documento_cambia_tipo_wf.Value = "-1" Then
                refclas_java.Showscripman_menu("Seleccione el registro para cambiar el tipo documento", Me.UpdatePanel_seleccion_treview, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Val(Me.hdnEmailID.Value)
            Dim Refclas As New ClassWorkflowDigitalizacion
            Dim Result As String = ""
            Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE_ADJUNTOWORKFLOW"
            Dim Refclas_digitalizacion As New ClassWorkflowDigitalizacion
            '-----Retorna el tipo de flujo interno o externo
            Dim refclas_workflow_digitalizacion As New ClassWorkflowDigitalizacion
            Dim id_tipo_flujo As Integer = 0
            Dim refclas_dat_adit As New Class_DAT_ADIC_TAR
            Result = refclas_dat_adit.SolicitaIdTipoFlujoTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                            Session.Item("WF_RUTAWORKFLOW"),
                                                            id_tipo_flujo)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_seleccion_treview, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_tipo_flujo = 1 Then
                Result = Refclas_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna(Session.Item("Id_Ruta_Workflow"),
                                                                                 HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                 HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"),
                                                                                 HttpContext.Current.Session.Item("DG_ID_TRAMITE"),
                                                                                 HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                                 HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                 HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                 HttpContext.Current.Session.Item("DG_RADICADO"),
                                                                                 HttpContext.Current.Session("DG_NOMBRE_TRAMITE"))
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_seleccion_treview, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = refclas_workflow_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna(Session.Item("Id_Ruta_Workflow"),
                                                                                                  HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                                  HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                  HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"),
                                                                                                  HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                                                  HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                  HttpContext.Current.Session.Item("DG_RADICADO"),
                                                                                                  HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"))
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_seleccion_treview, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim ref_class_tipo_doc_entrante As New Class_tipo_doc_entrante
                Result = ref_class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                             Session.Item("DG_ID_TRAMITE"))
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_seleccion_treview, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                             Session.Item("DG_TIPO_TRAMITE"),
                                                                             Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
            If Result = "YES" Then
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_seleccion_treview, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

            End If

            Dim estado_resultado As String = ""
            Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                           Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                           "",
                                                                                                                           Me.DropDownList_tipologia_documental_workflow,
                                                                                                                           Me.Update_actualiza_tipologia_documental_workflow,
                                                                                                                           estado_resultado)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_seleccion_treview, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_actualiza_tipologia_documental_workflow.Show()
            End If
        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, Me.UpdatePanel_seleccion_treview)

        End Try
    End Sub
    'Actualiza tipologia documental documemnto seleccionado en workflow
    Private Sub Button_actualiza_tipologia_documental_workflow_Click(sender As Object, e As EventArgs) Handles Button_actualiza_tipologia_documental_workflow.Click
        Dim refclas_java As New Classscrripjava
        Try
            Me.Hidden_resulta_botno_tipologia_documental_workflow.Value = ""
            If Me.Hidden_selccion_documento_cambia_tipo_wf.Value = "" Or Me.Hidden_selccion_documento_cambia_tipo_wf.Value = "-1" Then
                refclas_java.Showscripman_menu("Debe seleccionar un item de documentos", Me.UpdatePanel_boton_tipologia_documental_workflow, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflowDigitalizacion
            Dim split() As String = Me.Hidden_selccion_documento_cambia_tipo_split_wf.Value.ToString.Split("|")
            Dim valor_campo As String = ""
            Dim value_tipologia As Integer = -1
            Dim value_text As String = ""
            If Me.DropDownList_tipologia_documental_workflow.Items.Count > 0 Then
                value_tipologia = Val(Me.DropDownList_tipologia_documental_workflow.SelectedValue)
                value_text = Me.DropDownList_tipologia_documental_workflow.SelectedItem.Text
            End If
            Result = Refclas.Actualiza_tipo_documento_lista_chequeo(split(1),
                                                                    value_tipologia,
                                                                    split(0),
                                                                    value_text,
                                                                    Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                    split(2),
                                                                    valor_campo)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tipologia_documental_workflow, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If valor_campo = "" Then
                    valor_campo = "Documento"
                End If
                Me.Hidden_resulta_botno_tipologia_documental_workflow.Value = valor_campo
                Me.ModalPopupExtender_edition_actualiza_tipologia_documental_workflow.Hide()
            End If

        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, UpdatePanel_boton_tipologia_documental_workflow)
        End Try
    End Sub

    Private Sub Button_activa_lista_solicitudes_aprobacion_Click(sender As Object, e As EventArgs) Handles Button_activa_lista_solicitudes_aprobacion.Click
        Dim refmensaje As New Classscrripjava
        Try
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim id_tarea_selecion As Integer = Val(hdnEmailID.Value)
            Dim Result As String = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecion,
                                                                                                Radicado)
            If Result <> "YES" Then
                refmensaje.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim id_respuesta As Integer = 0
            Result = ref_ra_resp_radic.Retorna_id_respuesta_radicado(Radicado,
                                                                     HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                     id_respuesta)
            If Result <> "YES" Then
                refmensaje.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HttpContext.Current.Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION") = id_respuesta
            Iframe_solicitud_aprobacion.Attributes.Add("src", "../radicador/WebFormRaGestionSolicitudesAprobacion.aspx")
            UpdatePanel_solicitud_aprobacion.Update()
            ModalPopupExtender_solicitud_aprobacion.Show()
        Catch ex As Exception
            refmensaje.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    '--------------------------------------------------------------------------------------------------------
    'ZONA RESPUESTA TRAMITE
    '-----------------------------------------------------
    'Activa descargar el formato o protocolo de respuesta
    '------------------------------------------------------
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
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value,
                                                                                        estru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '-----------------------------------------
            'habre ventana de descarga de la plantilla
            'radicada
            '-----------------------------------------
            If Session.Item("GA_TIPO_MODULO_RESPUESTA") = 1 Then
                If estru.RADICADO_RESPUESTA = "" Then
                    Result = Refclas.Solicita_lista_usuarios_permitidos_firma(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              stru_usu_firmas)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePanel_respuesta_documento)
                        Exit Sub
                    End If
                    Result = refclas_gestion.Lista_usuarios_firmas_permitidas_iterface(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       stru_usu_firmas,
                                                                                       HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       Me.DropDownList_lista_firma_interface,
                                                                                       Me.UpdatePanel_descarga_formato_interface)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePanel_gestion_respuesta)
                        Exit Sub
                    End If
                    Result = refclas_resp.Prepara_interface_descarga_plantilla_con_radicado(Me.Hidden_id_respuesta.Value,
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
                    Result = Refclas.Solicita_lista_usuarios_permitidos_firma(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              stru_usu_firmas)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePanel_respuesta_documento)
                        Exit Sub
                    End If
                    Dim Refclas_registro_dow As New Class_ra_ra_registro_down_formato
                    Dim id_usuario_respuesta As Integer = 0
                    Result = Refclas_registro_dow.Solicita_utltimo_usuario_firma_formato_descarga(Me.Hidden_id_respuesta.Value,
                                                                                                  id_usuario_respuesta)
                    If Result <> "YES" Then
                        scrijava.Showscripman(Result, Me.UpdatePanel_respuesta_documento)
                        Exit Sub
                    End If
                    Result = refclas_gestion.Lista_usuarios_firmas_permitidas_iterface(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                     stru_usu_firmas,
                                                                                     id_usuario_respuesta,
                                                                                     Me.DropDownList_lista_firmas,
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
    '---------------------------------------------------
    'Descarga plantilla 
    '---------------------------------------------------
    Protected Sub Button_descarga_plantilla_Click(sender As Object, e As EventArgs) Handles Button_descarga_plantilla.Click
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try
            Me.Hidden_rest_resp.Value = ""
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
            Dim refclas_gestion As New ClassGestion
            Dim id_respuesta As Integer = Me.Hidden_id_respuesta.Value
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim radicado As String = ""
            Dim refclas_resp As New Classgestionrespuesta
            Dim id_usuario_firma As Integer = Me.DropDownList_lista_firmas.SelectedValue
            Result = refclas_resp.Descarga_plantilla_radicada_respuesta(Me.Hidden_id_respuesta.Value,
                                                                        radicado,
                                                                        Me.Page,
                                                                        Hidden_ruta_archivo.Value,
                                                                        id_usuario_firma)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_descarga_formato, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
            update_botonoes_opciones_solicitud_general.Update()
            updatapanel_iframe.Update()
            Me.Hidden_rest_resp.Value = "YES"
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_descarga_formato, "ModalPopupExtender_mensaje_personalizado")
        Finally
            Me.ModalPopupExtender_descarga_formato.Hide()
        End Try
    End Sub
    Protected Sub Button_cancela_descarga_plantilla_Click(sender As Object, e As EventArgs) Handles Button_cancela_descarga_plantilla.Click
        Me.ModalPopupExtender_descarga_formato.Hide()
    End Sub
    '-----------------------------------------------------
    'Descarga plantilla radicado respuesta radicado inicial
    '-----------------------------------------------------
    Protected Sub Button_descarga_plantilla_radicado_resp_Click(sender As Object, e As EventArgs) Handles Button_descarga_plantilla_radicado_resp.Click
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try
            Me.Hidden_rest_resp.Value = ""
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
                scrijava.Showscripman_menu("Por favor active web service para workflow", Me.UpdatePanel_boton_descarga_plantilla, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("URL_WEB_SERVICE") = "" Then
                scrijava.Showscripman_menu("Por favor informe la url web service para workflow", Me.UpdatePanel_boton_descarga_plantilla, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_boton_descarga_plantilla, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclas_resp As New Classgestionrespuesta
            Dim radicado As String = ""
            Dim id_usuario_respuesta As Integer = Me.DropDownList_lista_firma_interface.SelectedValue
            Result = refclas_resp.Descarga_plantilla_radicada_respuesta(Me.Hidden_id_respuesta.Value,
                                                                        radicado,
                                                                        Me.Page,
                                                                        Hidden_ruta_archivo.Value,
                                                                        id_usuario_respuesta)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_descarga_plantilla, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_descarga_plantilla_radicada.Hide()
            ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
            updatapanel_iframe.Update()
            update_botonoes_opciones_solicitud_general.Update()
            Me.Hidden_rest_resp.Value = "YES"
            Result = refclas_resp.Actualiza_estados_general_semaforo(Me.Hidden_id_respuesta.Value,
                                                                     Me.Page)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_descarga_plantilla, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_descarga_plantilla, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    '---------------------------------------------------
    'Activa popup para cargar plantilla
    '---------------------------------------------------
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
            Result = Refclas.Generar_interface_responder_solicitud_gestion_respuesta(Hidden_id_respuesta.Value,
                                                                                     Val(Me.Hidden_id_propietario_resp.Value),
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
    '------------------------------------------------------
    'Ajax para subir plantilla respuesta radicado
    '------------------------------------------------------
    Private Sub AjaxFileUpload_sube_plantilla_respuesta_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_sube_plantilla_respuesta.UploadComplete
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
    '--------------------------------------------------
    'Guarda el documento plantilla de respuesta
    '--------------------------------------------------
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
                Result = Refclas.Subir_formato_respuesta_radicado(Hidden_id_respuesta.Value,
                                                                  archivo_donwload,
                                                                  consecutivo_radicado_respuesta,
                                                                  id_imagen_almacena,
                                                                  id_tipo_envio,
                                                                  Me.Image_estado_resp,
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
                Result = Refclas.Subir_respuesta_radicado(Hidden_id_respuesta.Value,
                                                         archivo_donwload,
                                                         consecutivo_radicado_respuesta,
                                                         id_imagen_almacena,
                                                         id_tipo_envio,
                                                         Me.Image_estado_resp,
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
                Result = Refclas.Generar_interface_confirmar_respuesta(Me.Hidden_id_respuesta.Value,
                                                                       Me.Hidden_id_propietario_resp.Value,
                                                                       Me.Hidden_radicado.Value,
                                                                       Me.Page,
                                                                       Me.DropDownList_lista_firmas_confirma_respuesta,
                                                                       Me.Hidden_tipo_respuesta.Value,
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
    '-------------------------------------------------------------------------
    'Elimina documento respuesta
    '-------------------------------------------------------------------------
    Protected Sub Button_eliminar_Click(sender As Object, e As EventArgs) Handles Button_eliminar.Click
        Dim Clasjava As New Classscrripjava
        Try
            Dim Refclas_ As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            If Hidden_resp.Value = "0" Then
                Exit Sub
            End If
            Result = Refclas.Elimina_documento_respuesta(Val(Hidden_id_respuesta.Value),
                                                                  Me.Image_estado_resp)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            UpdatePanel_image_semaforo.Update()
            'UpdatePanel_combo_plantillas.Update()
        Catch ex As Exception
            Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    '-------------------------------------------------------------------------
    'Descarga documento de respuesta
    '-------------------------------------------------------------------------
    Private Sub Button_descarga_Click(sender As Object, e As EventArgs) Handles Button_descarga.Click
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta",
                                           Me.UpdatePanel_respuesta_documento,
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_imagen As Integer = -1
            Dim gabinete_imagen As String = ""
            Dim refclas_gestion As New Classgestionrespuesta
            Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(Hidden_id_respuesta.Value,
                                                                              gabinete_imagen,
                                                                              id_imagen)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento,
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_imagen = -1 Then
                scrijava.Showscripman_menu("No hay documento para descargar",
                                           Me.UpdatePanel_respuesta_documento,
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclas_visualiza As New ClassVisualisaDocumento
            Dim id_respuesta As Integer = Me.Hidden_id_respuesta.Value
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim matri_documento() As String = Nothing
            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen,
                                                                            "IMP03GESTIONTMP",
                                                                            matri_documento)
            If Result <> "YES" Then
                If Result = "Imposible encontrar datos del documento Gnerando matriz de documentos" Then
                    scrijava.Showscripman_menu("El sistema no pudo encontrar un borrador de respuesta para descargar",
                                               Me.UpdatePanel_respuesta_documento,
                                               "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento,
                                               "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

            End If
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value,
                                                                                        estru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result,
                                           Me.UpdatePanel_respuesta_documento,
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
            update_botonoes_opciones_solicitud_general.Update()
            updatapanel_iframe.Update()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    '-------------------------------------------
    'Activa cargar anexos
    '-------------------------------------------
    Private Sub Button_anexo_cargar_Click(sender As Object, e As EventArgs) Handles Button_anexo_cargar.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.Hidden_id_respuesta.Value,
                                                                                       stru,
                                                                                       1)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result,
                                           Me.UpdatePanel_anexos_respuesta_boton,
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                scrijava.Showscripman_menu("El usuario no es el responsable de la respuesta, no puede agregar anexos",
                                           Me.UpdatePanel_anexos_respuesta_boton,
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/TEMP_TIF/"
            ruta_virtual = Server.MapPath(ruta_virtual)
            If Directory.Exists(ruta_virtual) = False Then
                Directory.CreateDirectory(ruta_virtual)
            End If
            Me.ModalPopupExtender_edition_sube_anexo_respuesta.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta_boton, "ModalPopupExtender_mensaje_personalizado")
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
    Protected Sub Button_radicar_tramite_Click(sender As Object, e As EventArgs) Handles Button_radicar_tramite.Click
        Dim scrijava As New Classscrripjava
        Dim refclasradicado As New ClassRadicador
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Try
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta",
                                           Me.UpdatePanel_respuesta_documento,
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.Hidden_id_propietario_resp.Value <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                scrijava.Showscripman_menu("El usuario de gestión no es el autorizado para gestionar la respuesta", Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estado_documento As String = "YES"
            Result = Refclas.Verifica_existencia_documento_de_respuesta(Hidden_id_respuesta.Value,
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
                Result = Refclas.Prepara_interface_radica_confirma_respuesta(Me.Hidden_id_respuesta.Value,
                                                                             Me.Hidden_radicado.Value,
                                                                             Me.Page,
                                                                             Me.Hidden_tipo_respuesta.Value,
                                                                             1)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_respuesta_documento, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.ModalPopupExtender_edition_radica_documento_respuesta.Show()
            End If
            If Session.Item("GA_TIPO_MODULO_RESPUESTA") = 1 Then
                Result = Refclas.Generar_interface_confirmar_respuesta(Me.Hidden_id_respuesta.Value,
                                                                       Me.Hidden_id_propietario_resp.Value,
                                                                       Me.Hidden_radicado.Value,
                                                                       Me.Page,
                                                                       Me.DropDownList_lista_firmas_confirma_respuesta,
                                                                       Me.Hidden_tipo_respuesta.Value,
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
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla,
                                                                           nombre_plantilla)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_id_script_validacion(id_plantilla,
                                                          "DINAMICOEXTERNO",
                                                          "REMITENTE_COR",
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
            Result = Refclas.Verifica_Permisos_usuario_plantilla_validacion(id_escrip,
                                                                            nombre_plantilla_validacion,
                                                                            0)
            If Result <> "YES" Then
                Clasjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = id_escrip
            Me.Iframe_validacion_plantilla_.Attributes.Add("src", "../radicador/WebFormGestionPlantillasvalidacion.aspx")
            Me.UpdatePanel_validacion_plantilla.Update()
            Me.ModalPopupExtender_valiacion_plantilla.Show()
        Catch ex As Exception
            Clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
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
    Private Sub Button_tool_activa_lista_gestion_solicitud_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_lista_gestion_solicitud.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Class_ra_log_respuesta_radicado As New Class_ra_log_respuesta_radicado
            Dim Radicado As String = ""
            Dim id_respuesta As Integer = 0
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    Radicado)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_gestion_solicitud, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                               HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                               id_respuesta)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_gestion_solicitud, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("SortExpression_publico") = "id_estado_radicado"
            Session.Item("SortDirection_publico") = "DESC"
            Result = Class_ra_log_respuesta_radicado.Lista_gestion_solicitud(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                             id_respuesta,
                                                                             1,
                                                                             "",
                                                                             HttpContext.Current.Session.Item("SortExpression_publico"),
                                                                             HttpContext.Current.Session.Item("SortDirection_publico"),
                                                                             Me.titulo_label_list_gestion_solicitud,
                                                                             Me.GridView_list_gestion_solicitud,
                                                                             Me.Hidden_list_gestion_solicitud,
                                                                             Me.Update_list_gestion_solicitud)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_gestion_solicitud, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_list_gestion_solicitud.Show()
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_gestion_solicitud, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridView_list_gestion_solicitud_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_list_gestion_solicitud.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub
End Class