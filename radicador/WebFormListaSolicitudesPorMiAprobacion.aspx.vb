Imports System.IO

Public Class WebFormListaSolicitudesPorMiAprobacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then
            Session.Item("SortExpression_aprobacion") = "ID_CD_USUARIOS_SOLICITUDES_APROBACION"
            Session.Item("SortDirection_aprobacion") = "DESC"
            Me.DropDownList_estado_aprobacion.Items.Add("Aprobado")
            Me.DropDownList_estado_aprobacion.Items.Add("Desaprobado")
            Me.DropDownList_estado_aprobacion.Items.Add("Archivado")
            Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Dim Result As String = reflcas_respuesta.Lista_solictudes_de_aprobacion_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  HiddenEmailconsulta, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                  UpdateGeneral, _
                                                                                                  "Solicitudes Pendientes por responder", _
                                                                                                  1, _
                                                                                                  "", _
                                                                                                  Session.Item("SortExpression_aprobacion"), _
                                                                                                  Session.Item("SortDirection_aprobacion"), _
                                                                                                  Me.UpdatePanel_estado_listado_solicitud)
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            End If
            Result = reflcas_respuesta.Retorna_numero_de_solicitudes_aprobacion_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                              HttpContext.Current.Session.Item("GA_NUMERO_SOLICITUDES_PENDIENTES_APROBAR_USUARIO"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            End If
        End If

    End Sub
    Private Sub Button_restore_lista_service_Click(sender As Object, e As EventArgs) Handles Button_restore_lista_service.Click
        Dim clasjava As New Classscrripjava
        Try
            Session.Item("SortExpression_aprobacion") = "ID_CD_USUARIOS_SOLICITUDES_APROBACION"
            Session.Item("SortDirection_aprobacion") = "DESC"
            Me.DropDownList_estado_aprobacion.Items.Add("Aprobado")
            Me.DropDownList_estado_aprobacion.Items.Add("Desaprobado")
            Me.DropDownList_estado_aprobacion.Items.Add("Archivado")
            Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Dim Result As String = reflcas_respuesta.Lista_solictudes_de_aprobacion_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  HiddenEmailconsulta, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                  UpdateGeneral, _
                                                                                                  "Solicitudes Pendientes por responder", _
                                                                                                  1, _
                                                                                                  "", _
                                                                                                  Session.Item("SortExpression_aprobacion"), _
                                                                                                  Session.Item("SortDirection_aprobacion"), _
                                                                                                  Me.UpdatePanel_estado_listado_solicitud)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub data_grid_listado_solicitudes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_solicitudes.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_listado_solicitudes.PageIndex = e.NewPageIndex
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Solicitudes Pendientes por responder"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Solicitudes Archivadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Solicitudes Aprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Solicitudes Desaprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Aprobado"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Desaprobado"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Archivado"
            End If
            Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Dim Result As String = reflcas_respuesta.Lista_solictudes_de_aprobacion_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  HiddenEmailconsulta, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                  UpdateGeneral, _
                                                                                                  selecion_name, _
                                                                                                  Session.Item("GA_TIPO_CONSULTA_SOLICITUD_APROBACION"), _
                                                                                                  Session.Item("GA_DATO_CONSULTA_SOLICITUD_APROBACION"), _
                                                                                                  Session.Item("SortExpression_aprobacion"), _
                                                                                                  Session.Item("SortDirection_aprobacion"), _
                                                                                                  Me.UpdatePanel_estado_listado_solicitud)
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
    Private Sub data_grid_listado_solicitudes_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_listado_solicitudes.Sorting
        Dim clasjava As New Classscrripjava
        Try
            Dim Sortdir As String = ""
            Dim Result As String = ""
            Dim Refclas As New ClassGredview
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Solicitudes Pendientes por responder"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Solicitudes Archivadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Solicitudes Aprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Solicitudes Desaprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Aprobado"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Desaprobado"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Archivado"
            End If
            Session.Item("SortExpression_aprobacion") = e.SortExpression
            If Session.Item("SortDirection_aprobacion") = "DESC" Then
                Session.Item("SortDirection_aprobacion") = "ASC"
            Else
                Session.Item("SortDirection_aprobacion") = "DESC"
            End If
             Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Result = reflcas_respuesta.Lista_solictudes_de_aprobacion_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  HiddenEmailconsulta, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                  UpdateGeneral, _
                                                                                                  selecion_name, _
                                                                                                  Session.Item("GA_TIPO_CONSULTA_SOLICITUD_APROBACION"), _
                                                                                                  Session.Item("GA_DATO_CONSULTA_SOLICITUD_APROBACION"), _
                                                                                                  Session.Item("SortExpression_aprobacion"), _
                                                                                                  Session.Item("SortDirection_aprobacion"), _
                                                                                                  Me.UpdatePanel_estado_listado_solicitud)
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
    Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
        Dim clasjava As New Classscrripjava
        Try
            If Not e.Row.Cells.Item(1) Is Nothing Then
                e.Row.Cells(1).Visible = False
            End If
            If Not e.Row.Cells.Item(2) Is Nothing Then
                e.Row.Cells(2).Visible = False
            End If
            If Not e.Row.Cells.Item(3) Is Nothing Then
                e.Row.Cells(3).Visible = False
            End If

        Catch ex As Exception

        End Try

    End Sub
    Private Sub data_grid_listado_solicitudes_DataBound(sender As Object, e As EventArgs) Handles data_grid_listado_solicitudes.DataBound
        Try
            Select Case sender.SortDirection
                'Case SortDirection.Ascending
                '    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                '    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                'Case SortDirection.Descending
                '    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                '    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                '    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                '    sender.FooterRow.ForeColor = System.Drawing.Color.Black
            End Select
        Catch ex As Exception
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
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro de solicitud", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru_user As STRU_SOLICITUD_APROBACION_USUARIOS = Nothing
            Result = refclas_solicitudes.Retorna_datos_solictud_aprobacion_usuarios(Me.hdnEmailID.Value, _
                                                                                    stru_user)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru As STRU_SOLICITUD_ARPBACION = Nothing
            Result = refclas_solicitudes.Retorna_datos_solicitud_aprobación_documentos(stru_user.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, _
                                                                                       stru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_imagen As Integer = -1
            Dim gabinete_imagen As String = ""
            Dim refclas_gestion As New Classgestionrespuesta
            Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD, _
                                                                              gabinete_imagen, _
                                                                              id_imagen)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclas_visualiza As New ClassVisualisaDocumento
            Dim id_respuesta As Integer = stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim matri_documento() As String = Nothing
            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen, _
                                                                            "IMP03GESTIONTMP", _
                                                                            matri_documento)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(stru.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD, _
                                                                                        estru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
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
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_desicion_aprobacion_Click(sender As Object, e As EventArgs) Handles Button_activa_desicion_aprobacion.Click
        Dim Result As String = ""
        Dim struc_envio As stru_envio = Nothing
        Dim refclasgestion As New Classgestionrespuesta
        Dim refclas_solicitudes As New ClassRaSolicitudesAprobacion
        Dim scrijava As New Classscrripjava
        Dim refclasdescargapublic As New Classdescargapublico
        Try
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro de solicitud", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = refclas_solicitudes.Lista_documentos_de_correccion_aprobacion_drow_list(Me.Hidden_id_solicitud.Value, _
                                                                                             Val(Me.hdnEmailID.Value), _
                                                                                             Me.DropDownList_docuentos_colaboracion, _
                                                                                             Me.UpdatePanel_adjunto_documento_colaboracion)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_desicion_solicitud.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_guadar_registro_desicion_Click(sender As Object, e As EventArgs) Handles Button_guadar_registro_desicion.Click
        Dim Result As String = ""
        Dim struc_envio As stru_envio = Nothing
        Dim refclasgestion As New Classgestionrespuesta
        Dim Refclas As New ClassRaSolicitudesAprobacion
        Dim scrijava As New Classscrripjava
        Dim refclasdescargapublic As New Classdescargapublico
        Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
        Dim correos_relacionados As String = ""
        Dim resultado_evio_correo As String = ""
        Try
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro de solicitud", Me.UpdatePanel_desicion_solicitud, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.DropDownList_estado_aprobacion.Text = "" Then
                scrijava.Showscripman_menu("Por favor seleccione la decisión para la solicitud", Me.UpdatePanel_desicion_solicitud, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru As STRU_SOLICITUD_APROBACION_USUARIOS = Nothing
            Result = Refclas.Retorna_datos_solictud_aprobacion_usuarios(hdnEmailID.Value, _
                                                                        stru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_desicion_solicitud, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(stru.Remit_Dest_Interno_id_remit_dest_Int, _
                                                                               correos_relacionados)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_desicion_solicitud, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru_general As STRU_SOLICITUD_ARPBACION = Nothing
            Result = Refclas.Retorna_datos_solicitud_aprobación_documentos(stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, _
                                                                           stru_general)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, _
                                           Me.UpdatePanel_desicion_solicitud, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estado_aprobacion As Integer = 0
            Dim descripcion_estado_aprobacion As String = ""
            If Me.DropDownList_estado_aprobacion.Text = "Aprobado" Then
                estado_aprobacion = 1
                descripcion_estado_aprobacion = "Aprobado"
            End If
            If Me.DropDownList_estado_aprobacion.Text = "Desaprobado" Then
                estado_aprobacion = 2
                descripcion_estado_aprobacion = "Desaprobado"
            End If
            If Me.DropDownList_estado_aprobacion.Text = "Archivado" Then
                estado_aprobacion = 3
                descripcion_estado_aprobacion = "Archivado"
            End If
            Dim estado_autoriza_firma As Integer = 0
            If Me.CheckBox_autoriza_firma.Checked = True Then
                estado_autoriza_firma = 1
            Else
                estado_autoriza_firma = 0
            End If
            Result = Refclas.Actualiza_estado_solicitud_aprobacion_usuario(hdnEmailID.Value, _
                                                                           estado_aprobacion, _
                                                                           descripcion_estado_aprobacion, _
                                                                           Me.Hidden_actualizacion_general.Value, _
                                                                           Me.Hidden_actualizacion_usuario.Value, _
                                                                           stru_general.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD, _
                                                                           HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                           correos_relacionados, _
                                                                           resultado_evio_correo, _
                                                                           0, _
                                                                           Me.TextBox_nota_solicitud.Text, _
                                                                           estado_autoriza_firma)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_desicion_solicitud, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_resultado_aprobacion.Value = "YES"
                ModalPopupExtender_desicion_solicitud.Hide()
                If resultado_evio_correo <> "YES" Then
                    scrijava.Showscripman_menu("Se confirmo la solicitud pero no se pudo notificar al correo electrónico" & resultado_evio_correo, Me.UpdatePanel_desicion_solicitud, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_desicion_solicitud, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_ver_documento_solicitud_Click(sender As Object, e As EventArgs) Handles Button_ver_documento_solicitud.Click
        Dim clasjava As New Classscrripjava
        Try
            'Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Seleccione el registro", update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim stru As STRU_SOLICITUD_APROBACION_USUARIOS = Nothing
            Dim Result = Refclas.Retorna_datos_solictud_aprobacion_usuarios(hdnEmailID.Value, stru)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim stru_general As STRU_SOLICITUD_ARPBACION = Nothing
            Result = Refclas.Retorna_datos_solicitud_aprobación_documentos(stru.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION, stru_general)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refcals As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim stru_envio As stru_envio = Nothing
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(stru_general.Ra_Respuesta_Radicado_ID_RESPUESTA_RADICAD, stru_envio, 1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclass_seleccion As New Classselecciotarea
            Dim id_tarea_workflow As Integer = 0
            Result = refclass_seleccion.Retorna_id_tarea_seleccionada_radicado(stru_envio.RADICADO, stru_envio.ID_RUTA_WF, id_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("SESIONITERCAMBIOVISOR") = "VISOR WORKFLOW" + "|" & id_tarea_workflow.ToString & "|" & stru_envio.system_plantilla_radicado_id_plantilla
            Me.Iframe_visor_externo_wf_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_visor_externo.Update()
            Me.ModalPopupExtender_visor_externo.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub
    '-----------Captura el evento del unpload del archivo
    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
            If Session.Item("WF_TIPO_ADJUNTA") = "CORECCION" Then
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

    '-------Boton que guarda el archivo
    Private Sub Button_guardar_desicion_Click(sender As Object, e As EventArgs) Handles Button_guardar_desicion.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAlmacenamiento
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                CLAS.Showscripman(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga)
                Exit Sub
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "CORECCION" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                Dim Refclascompartir As New ClassRaSolicitudesAprobacion
                Result = Refclascompartir.Registra_documento_correcion_aprobacion("IMP03GESTIONTMP", Me.Hidden_id_solicitud.Value, _
                        Session.Item("WF_RUTA_TEMPO_ADJUNTA"), DropDownList_docuentos_colaboracion, Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    CLAS.Showscripman(Result, Me.UpdatePanel_descarga)
                    Exit Sub
                Else
                    Me.UpdatePanel_adjunto_documento_colaboracion.Update()
                    Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                End If
            End If
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePanel_descarga)
        End Try
    End Sub
    Protected Sub ImageButton_elimina_archivo_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_elimina_archivo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclascompartir As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            If Me.DropDownList_docuentos_colaboracion.Text = "" Then Exit Sub
            Result = Refclascompartir.Elimina_documento_correcion_documento_colaboracion_dorw_list(Me.DropDownList_docuentos_colaboracion.Text, Me.DropDownList_docuentos_colaboracion, _
                                                                                                  Me.UpdatePanel_adjunto_documento_colaboracion)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_adjunto_documento_colaboracion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_adjunto_documento_colaboracion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ImageButton_adjunta_archivo_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_adjunta_archivo.Click   
        Session.Item("WF_TIPO_ADJUNTA") = "CORECCION"
        AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
        'AjaxFileUpload_dowload.AllowedFileTypes = "tif,jpg,tiff,bmp,pdf"
        Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
        AjaxFileUpload_dowload.MaximumNumberOfFiles = 4
        UpdatePanel_descarga.Update()
        Me.ModalPopupExtender_sube_documento_adjunto.Show()
    End Sub

    
    Private Sub Button_lik_service_boton_Click(sender As Object, e As EventArgs) Handles Button_lik_service_boton.Click
        Dim clasjava As New Classscrripjava
        Try
           
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Solicitudes Pendientes por responder"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Solicitudes Archivadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Solicitudes Aprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Solicitudes Desaprobadas"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Aprobado"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Desaprobado"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Archivado"
            End If
            Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Dim Result As String = reflcas_respuesta.Lista_solictudes_de_aprobacion_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  HiddenEmailconsulta, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                  UpdateGeneral, _
                                                                                                  selecion_name, _
                                                                                                  1, _
                                                                                                  "", _
                                                                                                  Session.Item("SortExpression_aprobacion"), _
                                                                                                  Session.Item("SortDirection_aprobacion"), _
                                                                                                  Me.UpdatePanel_estado_listado_solicitud)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
            End If
            
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ImageButton_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_buscar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Dim Result As String = reflcas_respuesta.Lista_solictudes_de_aprobacion_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  HiddenEmailconsulta, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                  UpdateGeneral, _
                                                                                                  "", _
                                                                                                  2, _
                                                                                                  Me.TextBox_busqueda.Text, _
                                                                                                  Session.Item("SortExpression_aprobacion"), _
                                                                                                  Session.Item("SortDirection_aprobacion"), _
                                                                                                  Me.UpdatePanel_estado_listado_solicitud)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    
End Class