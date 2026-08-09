Public Class WebFormListaSolicitudesAprobacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Page.IsPostBack = False Then
            'Me.DropDownList_estados_solicitudes.Items.Add("Todas")
            'Me.DropDownList_estados_solicitudes.Items.Add("Solicitudes Pendientes por aprobar")
            'Me.DropDownList_estados_solicitudes.Items.Add("Solicitudes Archivadas")
            'Me.DropDownList_estados_solicitudes.Items.Add("Solicitudes Aprobadas")
            'Me.DropDownList_estados_solicitudes.Items.Add("Solicitudes Desaprobadas")
            'Me.DropDownList_estados_solicitudes.Text = "Solicitudes Aprobadas"
            Session.Item("SortExpression_solicitudes_apro") = "ID_RESPUESTA_RADICADO"
            Session.Item("SortDirection_solicitudes_apro") = "DESC"
            Dim refclas As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            Result = refclas.Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            Me.GridViewlista, _
                                                                                            Me.Label_titulo_listado_solicitudes, _
                                                                                            Hidden_id, _
                                                                                            Me.UpdatePanelmensaje, _
                                                                                            "Solicitudes Aprobadas", _
                                                                                            1, _
                                                                                             "", _
                                                                                             Session.Item("SortExpression_solicitudes_apro"), _
                                                                                             Session.Item("SortDirection_solicitudes_apro"))
            If Result <> "YES" Then
                Label_estado.Text = Result
            End If
            
        End If
    End Sub

    Private Sub GridViewlista_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridViewlista.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            Dim Sortdir As String = ""
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Solicitudes Aprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Solicitudes Pendientes por aprobar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Solicitudes Archivadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Solicitudes Desaprobadas"
            End If
            GridViewlista.PageIndex = e.NewPageIndex
            Dim refclas As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            Result = refclas.Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            Me.GridViewlista, _
                                                                                            Me.Label_titulo_listado_solicitudes, _
                                                                                            Hidden_id, _
                                                                                            Me.UpdatePanelmensaje, _
                                                                                            selecion_name, _
                                                                                            HttpContext.Current.Session.Item("Tipo_consulta_solicitudes_apro"), _
                                                                                            HttpContext.Current.Session.Item("Tipo_dato_solicitudes_apro"), _
                                                                                            Session.Item("SortExpression_solicitudes_apro"), _
                                                                                            Session.Item("SortDirection_solicitudes_apro"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelmensaje, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.hdnEmailID.Value = "-1"
                Me.UpdatePanelmensaje.Update()
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelmensaje, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridViewlista_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            'e.Row.Cells(3).Visible = False
        Catch ex As Exception

        End Try
        
    End Sub
    Private Sub GridViewlista_Sorting(sender As Object, e As GridViewSortEventArgs) Handles GridViewlista.Sorting
        Dim clasjava As New Classscrripjava
        Try
            Dim Sortdir As String = ""
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Solicitudes Aprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Solicitudes Pendientes por aprobar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Solicitudes Archivadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Solicitudes Desaprobadas"
            End If
            Session.Item("SortExpression_solicitudes_apro") = e.SortExpression
            If Session.Item("SortDirection_solicitudes_apro") = "DESC" Then
                Session.Item("SortDirection_solicitudes_apro") = "ASC"
            Else
                Session.Item("SortDirection_solicitudes_apro") = "DESC"
            End If
            Dim refclas As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            Result = refclas.Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            Me.GridViewlista, _
                                                                                            Me.Label_titulo_listado_solicitudes, _
                                                                                            Hidden_id, _
                                                                                            Me.UpdatePanelmensaje, _
                                                                                            selecion_name, _
                                                                                            HttpContext.Current.Session.Item("Tipo_consulta_solicitudes_apro"), _
                                                                                            HttpContext.Current.Session.Item("Tipo_dato_solicitudes_apro"), _
                                                                                            Session.Item("SortExpression_solicitudes_apro"), _
                                                                                            Session.Item("SortDirection_solicitudes_apro"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelmensaje, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.hdnEmailID.Value = "-1"
                Me.UpdatePanelmensaje.Update()
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelmensaje, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub GridViewlista_DataBound(sender As Object, e As EventArgs) Handles GridViewlista.DataBound
        Try
            Select Case sender.SortDirection
                Case SortDirection.Ascending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                Case SortDirection.Descending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black
            End Select
        Catch ex As Exception
        End Try

    End Sub
    Private Sub Button_lik_service_boton_Click(sender As Object, e As EventArgs) Handles Button_lik_service_boton.Click
        Dim clasjava As New Classscrripjava
        Try

            Dim selecion_name As String = ""
             If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Solicitudes Aprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Solicitudes Pendientes por aprobar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Solicitudes Archivadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Solicitudes Desaprobadas"
            End If
            Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            Result = reflcas_respuesta.Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            Me.GridViewlista, _
                                                                                            Me.Label_titulo_listado_solicitudes, _
                                                                                            Hidden_id, _
                                                                                            Me.UpdatePanelmensaje, _
                                                                                            selecion_name, _
                                                                                            1, _
                                                                                            HttpContext.Current.Session.Item("Tipo_dato_solicitudes_apro"), _
                                                                                            Session.Item("SortExpression_solicitudes_apro"), _
                                                                                            Session.Item("SortDirection_solicitudes_apro"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonSubir_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonSubir.Click
        If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") <> "0" Then
            Me.ModalPopupTexto.Show()
        End If
    End Sub

   

    Private Sub ButtonFiltrar_Click(sender As Object, e As EventArgs) Handles ButtonFiltrar.Click
        Me.ModalPopupExtender_Filtro.Show()
    End Sub

   

    Private Sub ButtonCerrarpendiente_Click(sender As Object, e As EventArgs) Handles ButtonCerrarpendiente.Click
        Me.ModalPopupExtenderactualiza.Hide()
    End Sub

    Private Sub Button_actualiza_pendiente_Click(sender As Object, e As EventArgs) Handles Button_actualiza_pendiente.Click
        Dim refclas As New ClassWorkflow
        Dim Result As String = ""
        Dim Refcriptman As New Classscrripjava
        If Hidden_id.Value = "0" Then Exit Sub
        Try
            If Me.contenidobusqueda_actualiza.Text = "" Then Exit Sub
            Dim spli() As String = Hidden_id.Value.Split("-")

            Result = refclas.Actualizar_Datos_tarea_pendiente(Me.contenidobusqueda_actualiza.Text, spli(0))
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.Updatepanael_Actualiza, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_Resultado_actualiza.Value = "YES"
                ModalPopupExtenderactualiza.Hide()
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.Updatepanael_Actualiza, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub btnCancelpagina_Click(sender As Object, e As EventArgs) Handles btnCancelpagina.Click
        Me.ModalPopupExtendermesjpagina.Hide()
    End Sub

    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        'Session.Item("OPCIONSELECIONPENDIENTE") = "VISOR EMERGENTE"
        Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value
        'Me.Labelpendient.Text = "Tareas pendiente"
        Me.Iframe_visor_externo_wf_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
        Me.UpdatePanel_visor_externo.Update()
        Me.ModalPopupExtender_visor_externo.Show()
    End Sub

    

    Protected Sub Button_sacar_pendiente_Click(sender As Object, e As EventArgs) Handles Button_sacar_pendiente.Click
        Dim Refclas As New ClassWorkflow
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_resultado_pendiente.Value = ""
            If Me.Hidden_id.Value = "0" Or Me.Hidden_id.Value = "-1" Then
                clasjava.Showscripman_menu("Seleccione el registro", Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Hidden_rad.Value = "0" Or Hidden_rad.Value = "-1" Then
                clasjava.Showscripman_menu("Seleccione el registro", Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_solicitud As New ClassRaSolicitudesAprobacion
            Dim Refclas_gestion_resp As New Classgestionrespuesta
            Result = Refclas_gestion_resp.Actualiza_ruta_workflow_respuesta_radicado(HttpContext.Current.Session("Id_Ruta_Workflow"), Hidden_rad.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru_general As STRU_SOLICITUD_ARPBACION = Nothing
            Result = Refclas_solicitud.Retorna_datos_solicitud_aprobación_documentos(Me.Hidden_id.Value, stru_general)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refcals As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim stru_envio As stru_envio = Nothing
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(stru_general.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD, _
                                                                                       stru_envio, _
                                                                                       1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_pendiente As Integer = 0
            Result = Refclas.Retorna_id_pendiente_tarea_pendiente_aprobacion(stru_envio.ID_TAREA_WF, id_pendiente)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result & ", es posible que la tarea de workflow la tenga asignada en este momento", Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_id_tarea.Value = stru_envio.ID_TAREA_WF
                Me.Hidden_id_pendiente.Value = id_pendiente
                Me.Hidden_resultado_pendiente.Value = "YES"
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ModalPopupTexto.Hide()
    End Sub


    Protected Sub Button_ver_documento_solicitud_Click(sender As Object, e As EventArgs) Handles Button_ver_documento_solicitud.Click

        Dim clasjava As New Classscrripjava
        Try
            'Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            If Me.Hidden_id.Value = "0" Or Me.Hidden_id.Value = "-1" Then
                clasjava.Showscripman_menu("Seleccione el registro", Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            Dim stru_general As STRU_SOLICITUD_ARPBACION = Nothing
            Result = Refclas.Retorna_datos_solicitud_aprobación_documentos(Me.Hidden_id.Value, stru_general)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refcals As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim stru_envio As stru_envio = Nothing
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(stru_general.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD, _
                                                                                       stru_envio, 1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclass_seleccion As New Classselecciotarea
            Dim id_tarea_workflow As Integer = 0
            Result = refclass_seleccion.Retorna_id_tarea_seleccionada_radicado(stru_envio.RADICADO, stru_envio.ID_RUTA_WF, id_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("SESIONITERCAMBIOVISOR") = "VISOR WORKFLOW" + "|" & id_tarea_workflow.ToString & "|" & stru_envio.system_plantilla_radicado_id_plantilla
            'Me.Labelpendient.Text = "Tareas pendiente"
            Me.Iframe_visor_externo_wf_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_visor_externo.Update()
            Me.ModalPopupExtender_visor_externo.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Protected Sub Button_ver_documento_respuesta_solicitud_Click(sender As Object, e As EventArgs) Handles Button_ver_documento_respuesta_solicitud.Click
        Dim Result As String = ""
        Dim struc_envio As stru_envio = Nothing
        Dim refclasgestion As New Classgestionrespuesta
        Dim refclas_solicitudes As New ClassRaSolicitudesAprobacion
        Dim scrijava As New Classscrripjava
        Dim refclasdescargapublic As New Classdescargapublico
        Try
            If Me.Hidden_id.Value = "0" Or Me.Hidden_id.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro de solicitud", Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'Dim stru_user As STRU_SOLICITUD_APROBACION_USUARIOS = Nothing
            'Result = refclas_solicitudes.Retorna_datos_solictud_aprobacion_usuarios(Me.hdnEmailID.Value, stru_user)
            'If Result <> "YES" Then
            '    scrijava.Showscripman_menu(Result, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim stru As STRU_SOLICITUD_ARPBACION = Nothing
            Result = refclas_solicitudes.Retorna_datos_solicitud_aprobación_documentos(Me.Hidden_id.Value, stru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_imagen As Integer = -1
            Dim gabinete_imagen As String = ""
            Dim refclas_gestion As New Classgestionrespuesta
            Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD, gabinete_imagen, id_imagen)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclas_visualiza As New ClassVisualisaDocumento
            Dim id_respuesta As Integer = stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim matri_documento() As String = Nothing
            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen, "IMP03GESTIONTMP", matri_documento)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD, estru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
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
            scrijava.Showscripman_menu(ex.Message, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub ButtonActiva_solicitud_aprobacion_Click(sender As Object, e As EventArgs) Handles ButtonActiva_solicitud_aprobacion.Click
        Dim Result As String = ""
        Dim struc_envio As stru_envio = Nothing
        Dim refclasgestion As New Classgestionrespuesta
        Dim refclas_solicitudes As New ClassRaSolicitudesAprobacion
        Dim scrijava As New Classscrripjava
        Try
            If Me.Hidden_id.Value = "0" Or Me.Hidden_id.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro de solicitud", Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            
            Dim stru As STRU_SOLICITUD_ARPBACION = Nothing
            Result = refclas_solicitudes.Retorna_datos_solicitud_aprobación_documentos(Me.Hidden_id.Value, stru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION") = stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD
            Me.Iframe_solicitud_aprobacion.Attributes.Add("src", "../radicador/WebFormRaGestionSolicitudesAprobacion.aspx")
            Me.UpdatePanel_solicitud_aprobacion.Update()
            Me.ModalPopupExtender_solicitud_aprobacion.Show()

        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_ver_nota_general_Click(sender As Object, e As EventArgs) Handles Button_activa_ver_nota_general.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim correos_relacionados As String = ""
            If Hidden_id.Value = "0" Or Hidden_id.Value = "-1" Then
                Refcriptman.Showscripman_menu("Debe seleccionar el registro de la solicitud de aprobación", Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("GA_INTERCAMBIO_NOTA_APROBACION") = Hidden_id.Value
            Session.Item("GA_INTERCAMBIO_TIPO_NOTA_APROBACION") = "GENERAL"
            Me.Iframelibre_notas_general_.Attributes("SRC") = "../radicador/WebFormRaNotasSolicitudesAprobacion.aspx"
            Me.UpdatePanelLibre.Update()
            Me.ModalPopupExtenderLibre.Show()
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

   
    Private Sub ImageButton_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_buscar.Click
        Dim clasjava As New Classscrripjava
        Try

            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Solicitudes Aprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Solicitudes Pendientes por aprobar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Solicitudes Archivadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Solicitudes Desaprobadas"
            End If
            Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            Result = reflcas_respuesta.Lista_usuarios_relacionados_a_solictud_de_aprobacion_pendiente(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            Me.GridViewlista, _
                                                                                            Me.Label_titulo_listado_solicitudes, _
                                                                                            Hidden_id, _
                                                                                            Me.UpdatePanelmensaje, _
                                                                                            selecion_name, _
                                                                                            2, _
                                                                                            Me.contenidobusqueda.Text, _
                                                                                            Session.Item("SortExpression_solicitudes_apro"), _
                                                                                            Session.Item("SortDirection_solicitudes_apro"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_listar_usuarios_relacionados_solicitud_Click(sender As Object, e As EventArgs) Handles Button_listar_usuarios_relacionados_solicitud.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Result = Refclas.Lista_usuarios_relacionados_a_solictud_de_aprobacion(Val(Me.Hidden_id.Value), _
                                                                                  Me.data_grid_documentos, _
                                                                                  Me.titulo_label_expedientes_documentos, _
                                                                                  UpdateGeneral_documentos)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdateGeneral_documentos.Update()
                Me.ModalPopupExtender_edition_usu_rel_solicitud.Show()
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_documentos_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_documentos.RowCreated
        e.Row.Cells(1).Visible = False
    End Sub
End Class