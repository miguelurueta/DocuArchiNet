Imports System.IO

Public Class WebFormRaGestionSolicitudesAprobacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim clasjava As New Classscrripjava
            Dim Result As String = ""
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            If Me.IsPostBack = False Then
                Me.DropDownList_prioridad_solicitud.Items.Add("Normal")
                Me.DropDownList_prioridad_solicitud.Items.Add("Urgente")
                Dim refclas As New ClassWorkflow
                Result = refclas.Lista_Actividades_Usuario_listview(Me.GridViewlista, _
                                                                    Me.Label_totales_usuario)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                End If
                Dim stru_envi As stru_envio = Nothing
                Dim refclassgestion As New Classgestionrespuesta
                Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
                Me.Hidden_id_respuesta.Value = Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION")
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
                                                                                            stru_envi)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                Else
                    Label_solicitudes_relacionadas.Text = Label_solicitudes_relacionadas.Text & " de respuesta para el radicado " & stru_envi.RADICADO
                End If
                Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
                Result = reflcas_respuesta.Lista_solicitudes_generales_de_aprobacion_de_una_respuesta(Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
                                                                                                      Me.data_grid, _
                                                                                                      HiddenEmailconsulta, _
                                                                                                      titulo_label_expedientes, _
                                                                                                      Me.hdnEmailID, UpdateGeneral, _
                                                                                                      data_grid_documentos, _
                                                                                                      titulo_label_expedientes_documentos, _
                                                                                                      UpdateGeneral_documentos)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                End If
            End If
        Catch ex As Exception
            Label_estado.Text = Label_estado.Text & ex.Message
        End Try
    End Sub
    Private Sub GridViewlista_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowCreated
        e.Row.Cells(1).Visible = False

    End Sub
    Private Sub Button_activa_registro_solicitud_Click(sender As Object, e As EventArgs) Handles Button_activa_registro_solicitud.Click

        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim refclas As New ClassRaSolicitudesAprobacion
            Result = refclas.Verfica_viabilidad_solicitud_aprobacion_respuesta(Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
                                                                               Me.TextBox_fecha_limite_solicitud.Text)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            UpdatePanel_registro_solicitud.Update()
            ModalPopupExtender_actualizacion_anualidad.Show()
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub

    Private Sub Button_activa_usuario_relacion_Click(sender As Object, e As EventArgs) Handles Button_activa_usuario_relacion.Click
        Me.ModalPopupExtender_lista_usuarios_solicitud.Show()
    End Sub

    Protected Sub Button_asignar_usuarios_Click(sender As Object, e As EventArgs) Handles Button_asignar_usuarios.Click
        Dim Refcriptman As New Classscrripjava
        Try
            If Me.hdnEmailID_sel.Value = "0" Then
                Refcriptman.Showscripman_menu("Por favor seleccione los usuarios para solicitud de aprobación ", Me.updatepnael_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                ModalPopupExtender_lista_usuarios_solicitud.Hide()
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.updatepnael_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_registrar_solicitud_aprobacion_Click(sender As Object, e As EventArgs) Handles Button_registrar_solicitud_aprobacion.Click
        Dim Refcriptman As New Classscrripjava
        Try
            'Dim Result As String = ""
            'Dim Refclas As New ClassRaSolicitudesAprobacion
            'Dim par = Me.Hidden_correos_electronico.Value
            'Me.HiddenEmailconsulta.Value = ""
            'Dim resultado_envio_correo As String = ""
            'Result = Refclas.Registra_solicitud_aprobacion(Me.DropDownList_prioridad_solicitud.Text, _
            '                                               Me.TextBox_nota_aprobacion.Text, _
            '                                               Me.TextBox_fecha_limite_solicitud.Text, _
            '                                               Me.TextBox_user_seleccionado.Text, _
            '                                               Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
            '                                               HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
            '                                               Me.Hidden_correos_electronico.Value, _
            '                                               resultado_envio_correo)
            'If Result <> "YES" Then
            '    Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_registro_solicitud, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'Else
            '    Session.Item("GA_STRU_ESTADO_NUEVA_SOLICITUD_APROBACION") = "YES"
            '    Me.HiddenEmailconsulta.Value = "YES"
            '    Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            '    Result = reflcas_respuesta.Lista_solicitudes_generales_de_aprobacion_de_una_respuesta(Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
            '                                                                                          Me.data_grid, _
            '                                                                                          HiddenEmailconsulta, _
            '                                                                                          titulo_label_expedientes, _
            '                                                                                          Me.hdnEmailID, _
            '                                                                                          UpdateGeneral, _
            '                                                                                          data_grid_documentos, _
            '                                                                                          titulo_label_expedientes_documentos, _
            '                                                                                          UpdateGeneral_documentos)
            '    If Result <> "YES" Then
            '        Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_registro_solicitud, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If
            '    If resultado_envio_correo <> "YES" Then
            '        Refcriptman.Showscripman_menu("Se registro la solicitud de aprobación pero fue imposible enviar la notificación al correo electrónico, error  " & resultado_envio_correo, Me.UpdatePanel_registro_solicitud, "ModalPopupExtender_mensaje_personalizado")
            '    End If
            '    Me.ModalPopupExtender_actualizacion_anualidad.Hide()
            'End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_registro_solicitud, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub data_grid_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid.PageIndex = e.NewPageIndex
            Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Dim Result As String = reflcas_respuesta.Lista_solicitudes_generales_de_aprobacion_de_una_respuesta(Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
                                                                                                                Me.data_grid, _
                                                                                                                HiddenEmailconsulta, _
                                                                                                                titulo_label_expedientes, _
                                                                                                                Me.hdnEmailID, _
                                                                                                                UpdateGeneral, _
                                                                                                                data_grid_documentos, _
                                                                                                                titulo_label_expedientes_documentos, _
                                                                                                                UpdateGeneral_documentos)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub
    Private Sub data_grid_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid.RowCreated
        Try
            'e.Row.Cells(0).Visible = False
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button_listar_usuarios_relacionados_solicitud_Click(sender As Object, e As EventArgs) Handles Button_listar_usuarios_relacionados_solicitud.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Result = Refclas.Lista_usuarios_relacionados_a_solictud_de_aprobacion(hdnEmailID_documentos.Value, _
                                                                                  Me.data_grid_documentos, _
                                                                                  HiddenEmailconsulta, _
                                                                                  Me.titulo_label_expedientes_documentos, _
                                                                                  Hidden_id_usuarios_sel, _
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

    Private Sub data_grid_documentos_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_documentos.PageIndexChanging
        Dim Refcriptman As New Classscrripjava
        Try
            data_grid_documentos.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Result = Refclas.Lista_usuarios_relacionados_a_solictud_de_aprobacion(hdnEmailID_documentos.Value, _
                                                                                  Me.data_grid_documentos, _
                                                                                  HiddenEmailconsulta, _
                                                                                  Me.titulo_label_expedientes_documentos, _
                                                                                  Hidden_id_usuarios_sel, _
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
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try

    End Sub

    Protected Sub Button_envia_correo_notificacion_Click(sender As Object, e As EventArgs) Handles Button_envia_correo_notificacion.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            If hdnEmailID_documentos.Value = "0" Or hdnEmailID_documentos.Value = "-1" Then
                Refcriptman.Showscripman_menu("Debe seleccionar el registro de la solicitud de aprobación", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If        
            Result = Refclas.Activa_notificacion_correo_solicitud_aprobacion(Val(hdnEmailID_documentos.Value))
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_notifica_solicitud_usuario_correo_Click(sender As Object, e As EventArgs) Handles Button_notifica_solicitud_usuario_correo.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            If Hidden_id_usuarios_sel.Value = "0" Or Hidden_id_usuarios_sel.Value = "-1" Then
                Refcriptman.Showscripman_menu("Debe seleccionar el registro de usuario de solicitud de aprobación", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Activa_notificacion_correo_solicitud_aprobacion_usuario(Val(Hidden_id_usuarios_sel.Value))
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_archiva_solicitud_Click(sender As Object, e As EventArgs) Handles Button_archiva_solicitud.Click
        'Archiva_solicitud_aprobacion_usuario
        Dim Refcriptman As New Classscrripjava
        Try
            Me.Hidden_resultado_aprobacion.Value = ""
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Dim correos_relacionados As String = ""
            Dim resultado_evio_correo As String = ""

            If Hidden_id_usuarios_sel.Value = "0" Or Hidden_id_usuarios_sel.Value = "-1" Then
                Refcriptman.Showscripman_menu("Debe seleccionar el registro de usuario de solicitud de aprobación", _
                                              Me.UpdatePanel_expediente_seleccionado, _
                                              "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru As STRU_SOLICITUD_APROBACION_USUARIOS = Nothing
            Result = Refclas.Retorna_datos_solictud_aprobacion_usuarios(Hidden_id_usuarios_sel.Value, stru)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(stru.Remit_Dest_Interno_id_remit_dest_Int, _
                                                                              correos_relacionados)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Actualiza_estado_archivado_solicitud_aprobacion_usuario(Hidden_id_usuarios_sel.Value, 3, "Archivado", Me.Hidden_actualizacion_general.Value, Me.Hidden_actualizacion_usuario.Value, _
                    Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), correos_relacionados, resultado_evio_correo, 1)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_resultado_aprobacion.Value = "YES"
                If resultado_evio_correo <> "YES" Then
                    Refcriptman.Showscripman_menu("Se archivo la solicitud pero no se pudo notificar al correo electrónico" & resultado_evio_correo, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_nuevo_integrante_Click(sender As Object, e As EventArgs) Handles Button_nuevo_integrante.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim correos_relacionados As String = ""
            If hdnEmailID_documentos.Value = "0" Or hdnEmailID_documentos.Value = "-1" Then
                Refcriptman.Showscripman_menu("Debe seleccionar el registro de la solicitud de aprobación", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_registro_solicitud_usuario.Show()
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub



    Protected Sub Button_registrar_solicitud_aprobacion_usuario_Click(sender As Object, e As EventArgs) Handles Button_registrar_solicitud_aprobacion_usuario.Click
        'Dim Refcriptman As New Classscrripjava
        'Try
        '    Hidden_resultado_actualizar_usuario.Value = ""
        '    Dim resultado_envio_correo As String = ""
        '    Dim Result As String = ""
        '    Dim Refclas As New ClassRaSolicitudesAprobacion
        '    If hdnEmailID_documentos.Value = "0" Or hdnEmailID_documentos.Value = "-1" Then
        '        Refcriptman.Showscripman_menu("Debe seleccionar el registro de la solicitud de aprobación", Me.UpdatePanel_registro_solicitud_usuario, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    End If
        '    'Result = Refclas.Agrega_usuario_a_la_solicitud_aprobacion(Me.TextBox_user_relacion.Text, _
        '    '                                      Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
        '    '                                      HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
        '    '                                      Me.Hidden_correos_electronico.Value, _
        '    '                                      resultado_envio_correo, _
        '    '                                      hdnEmailID_documentos.Value)
        '    If Result <> "YES" Then
        '        Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_registro_solicitud_usuario, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    Else
        '        Result = Refclas.Lista_usuarios_relacionados_a_solictud_de_aprobacion(hdnEmailID_documentos.Value, _
        '                                                                              Me.data_grid_documentos, _
        '                                                                              HiddenEmailconsulta, _
        '                                                                              Me.titulo_label_expedientes_documentos, _
        '                                                                              Hidden_id_usuarios_sel, _
        '                                                                              UpdateGeneral_documentos)
        '        If Result <> "YES" Then
        '            Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_registro_solicitud_usuario, "ModalPopupExtender_mensaje_personalizado")
        '            Exit Sub
        '        End If
        '        Hidden_resultado_actualizar_usuario.Value = "YES"
        '        If resultado_envio_correo <> "YES" Then
        '            Refcriptman.Showscripman_menu("Se relacionaron los usuarios pero no se pudo notificar al correo electrónico", Me.UpdatePanel_registro_solicitud_usuario, "ModalPopupExtender_mensaje_personalizado")
        '            Exit Sub
        '        End If
        '        ModalPopupExtender_registro_solicitud_usuario.Hide()
        '    End If
        'Catch ex As Exception

        'End Try
    End Sub

    Private Sub Button_activa_relacion_usuarios_Click(sender As Object, e As EventArgs) Handles Button_activa_relacion_usuarios.Click
        Me.ModalPopupExtender_lista_usuarios_solicitud.Show()
    End Sub

    Protected Sub Button_activa_ver_nota_general_Click(sender As Object, e As EventArgs) Handles Button_activa_ver_nota_general.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim correos_relacionados As String = ""
            If hdnEmailID_documentos.Value = "0" Or hdnEmailID_documentos.Value = "-1" Then
                Refcriptman.Showscripman_menu("Debe seleccionar el registro de la solicitud de aprobación", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("GA_INTERCAMBIO_NOTA_APROBACION") = hdnEmailID_documentos.Value
            Session.Item("GA_INTERCAMBIO_TIPO_NOTA_APROBACION") = "GENERAL"
            Me.Iframelibre_notas_general_.Attributes("SRC") = "../radicador/WebFormRaNotasSolicitudesAprobacion.aspx"
            Me.UpdatePanelLibre.Update()
            Me.ModalPopupExtenderLibre.Show()
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Protected Sub Button_estado_solicitud_Click(sender As Object, e As EventArgs) Handles Button_estado_solicitud.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim correos_relacionados As String = ""
            If Hidden_id_usuarios_sel.Value = "0" Or Hidden_id_usuarios_sel.Value = "-1" Then
                Refcriptman.Showscripman_menu("Debe seleccionar el registro de la solicitud de aprobación", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("GA_INTERCAMBIO_NOTA_APROBACION") = Hidden_id_usuarios_sel.Value
            Session.Item("GA_INTERCAMBIO_TIPO_NOTA_APROBACION") = "ESPECIFICA"
            Me.Iframelibre_notas_general_.Attributes("SRC") = "../radicador/WebFormRaNotasSolicitudesAprobacion.aspx"
            Me.UpdatePanelLibre.Update()
            Me.ModalPopupExtenderLibre.Show()
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Protected Sub Button_cancelar_registro_Click(sender As Object, e As EventArgs) Handles Button_cancelar_registro.Click
        ModalPopupExtender_actualizacion_anualidad.Hide()
    End Sub
    Protected Sub Button_descarga_documento_Click(sender As Object, e As EventArgs) Handles Button_descarga_documento.Click
        Dim scrijava As New Classscrripjava
        Dim split() As String = Me.Hidden_documento_descarga.Value.Split("|")
        Dim Result As String = ""
        Try
            If split.Length > 1 Then
                Dim refclas_visualiza As New ClassVisualisaDocumento
                Dim matri_documento() As String = Nothing
                Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(split(0), split(1), matri_documento)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_descraga_documento, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim files As New FileInfo(matri_documento(1))
                Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("GA_RUTA_TEMPO") & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Directory.Exists(Ruttempo) = False Then
                    Directory.CreateDirectory(Ruttempo)
                End If
                Ruttempo = Ruttempo & "\tempo_image_externa"
                If Directory.Exists(Ruttempo) = False Then
                    Directory.CreateDirectory(Ruttempo)
                End If
                If File.Exists(Ruttempo & "\" & "documento_descarga" & files.Extension) Then
                    Kill(Ruttempo & "\" & "documento_descarga" & files.Extension)
                    File.Copy(files.FullName, Ruttempo & "\" & "documento_descarga" & files.Extension)
                Else
                    File.Copy(files.FullName, Ruttempo & "\" & "documento_descarga" & files.Extension)
                End If
                Dim url_imagen As String = HttpContext.Current.Session.Item("GA_RUTA_TEMPO") + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString + "/tempo_image_externa/" & "documento_descarga" & files.Extension
                Hidden_ruta_archivo.Value = url_imagen
                ifmExcel_.Attributes.Add("src", url_imagen)
                updatapanel_iframe.Update()
            End If

        Catch ex As Exception
            scrijava.Showscripman_menu(Result, Me.UpdatePanel_descraga_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_documentos_correccion_Click(sender As Object, e As EventArgs) Handles Button_documentos_correccion.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava
        Try
            If Me.Hidden_id_usuarios_sel.Value = "0" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro para ver los documentos relacionados", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim stru() As stru_documentos_colaboracion = Nothing
            Result = Refclas.Lista_documentos_correcion_aprobacion_usuario(Me.Hidden_id_usuarios_sel.Value, _
                                                                           stru, _
                                                                           Me.Label_estado_doc_colaboracion, _
                                                                           Me.UpdatePanel_estado_doc_colaboracion)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Lista_documentos_correcion_aprobacion_interface(Me.Page, _
                                                                             Me.Hidden_id_usuarios_sel.Value)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_lista_documentos_colaboracion.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    
    Protected Sub Button_todos_documentos_correccion_Click(sender As Object, e As EventArgs) Handles Button_todos_documentos_correccion.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava
        Try
            If Me.hdnEmailID_documentos.Value = "0" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro para ver los documentos relacionados", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim stru() As stru_documentos_colaboracion = Nothing
            Result = Refclas.Lista_documentos_correcion_aprobacion_documento_compartido(Me.hdnEmailID_documentos.Value, _
                                                                                        stru, _
                                                                                        Me.Label_estado_doc_colaboracion, _
                                                                                        Me.UpdatePanel_estado_doc_colaboracion)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Lista_documentos_correcion_aprobacion_interface_general(Me.Page, _
                                                                                     Me.hdnEmailID_documentos.Value)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_lista_documentos_colaboracion.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    
    Protected Sub Button_activa_anulacion_solicitud_Click(sender As Object, e As EventArgs) Handles Button_activa_anulacion_solicitud.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava
        Try
            If Me.hdnEmailID_documentos.Value = "0" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro para anular la solicitud", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_anula_solictud_aprobacion.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_nula_solicitud_aprobacion_Click(sender As Object, e As EventArgs) Handles Button_nula_solicitud_aprobacion.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava
        Try
            If Me.hdnEmailID_documentos.Value = "0" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro para anular la solicitud", Me.UpdatePanel_buton_anula, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBox_nota_anulado.Text = "" Then
                scrijava.Showscripman_menu("Por favor digte la nota de anulación", Me.UpdatePanel_buton_anula, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassRaSolicitudesAprobacion    
            Dim Resultdado_correo As String = ""
            Result = Refclas.Actualiza_estado_anulado_general_aprobacion(Me.hdnEmailID_documentos.Value, _
                                                                         Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
                                                                         Me.TextBox_nota_anulado.Text, _
                                                                         Resultdado_correo)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_buton_anula, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("GA_STRU_ESTADO_NUEVA_SOLICITUD_APROBACION") = ""
            Me.Hidden_resultado_actualizar_usuario.Value = "YES"
            UpdatePanel_registro_solicitud_usuario.Update()
            If Resultdado_correo <> "YES" Then
                scrijava.Showscripman_menu("Se anulo la soicitud de parobación de la respuesta pero no se notifico al correo electrónico por el siguiente error : " & Resultdado_correo, Me.UpdatePanel_buton_anula, "ModalPopupExtender_mensaje_personalizado")
            End If
            ModalPopupExtender_edition_anula_solictud_aprobacion.Hide()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_anula, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_actualiza_Click(sender As Object, e As EventArgs) Handles Button_actualiza.Click
        Dim scrijava As New Classscrripjava
        Try
            Me.HiddenEmailconsulta.Value = "YES"
            Dim reflcas_respuesta As New ClassRaSolicitudesAprobacion
            Dim Result As String = reflcas_respuesta.Lista_solicitudes_generales_de_aprobacion_de_una_respuesta(Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION"), _
                                                                                                                Me.data_grid, _
                                                                                                                HiddenEmailconsulta, _
                                                                                                                titulo_label_expedientes, _
                                                                                                                Me.hdnEmailID, _
                                                                                                                UpdateGeneral, _
                                                                                                                data_grid_documentos, _
                                                                                                                titulo_label_expedientes_documentos, _
                                                                                                                UpdateGeneral_documentos)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_registro_solicitud, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_actualizacion_anualidad.Hide()
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, Me.UpdatePanel_registro_solicitud)
        End Try
    End Sub
    Protected Sub Button_cancela_registro_solictud_Click(sender As Object, e As EventArgs) Handles Button_cancela_registro_solictud.Click
        Me.ModalPopupExtender_registro_solicitud_usuario.Hide()
    End Sub

    Private Sub Button_actualiza_registro_solicitud_Click(sender As Object, e As EventArgs) Handles Button_actualiza_registro_solicitud.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Hidden_resultado_actualizar_usuario.Value = ""
            Dim resultado_envio_correo As String = ""
            Dim Result As String = ""
            Dim Refclas As New ClassRaSolicitudesAprobacion
            
                Result = Refclas.Lista_usuarios_relacionados_a_solictud_de_aprobacion(hdnEmailID_documentos.Value, _
                                                                                      Me.data_grid_documentos, _
                                                                                      HiddenEmailconsulta, _
                                                                                      Me.titulo_label_expedientes_documentos, _
                                                                                      Hidden_id_usuarios_sel, _
                                                                                      UpdateGeneral_documentos)
                If Result <> "YES" Then
                    Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_registro_solicitud_usuario, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Hidden_resultado_actualizar_usuario.Value = "YES"
                If resultado_envio_correo <> "YES" Then
                    Refcriptman.Showscripman_menu("Se relacionaron los usuarios pero no se pudo notificar al correo electrónico", Me.UpdatePanel_registro_solicitud_usuario, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                ModalPopupExtender_registro_solicitud_usuario.Hide()

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Button_cancela_nula_solicitud_aprobacion_Click(sender As Object, e As EventArgs) Handles Button_cancela_nula_solicitud_aprobacion.Click
        Me.ModalPopupExtender_edition_anula_solictud_aprobacion.Hide()
    End Sub
End Class