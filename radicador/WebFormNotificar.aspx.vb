Public Class WebFormNotificar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Refclas_workflow_ini As New InicioWorkflow
        'Dim Result = Refclas_workflow_ini.Crea_Dir_Temporal_wf()
    End Sub

    Private Sub Button_Agregar_Click(sender As Object, e As EventArgs) Handles Button_Agregar.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scripjava As New Classscrripjava
        Try
            'If Me.TextBox_busca_correo.Text = "" Then Exit Sub
            'Result = Refclas.Asigna_nueva_cuenta_correo(Me.TextBox_lista_correos.Text, Me.TextBox_busca_correo.Text)
            'If Result <> "YES" Then
            '    scripjava.Showscripman(Result, Me.Updatepanel_contenedor_controles)
            '    Exit Sub
            'Else
            '    Me.TextBox_busca_correo.Text = ""
            'End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.Updatepanel_contenedor_controles)
        End Try
    End Sub

    Private Sub Button_confgura_cuenta_Click(sender As Object, e As EventArgs) Handles Button_confgura_cuenta.Click
        'Hidden_cuenta_correo_envio
        Dim Result As String = ""
        Dim Refclas As New ClassCorreo
        Dim scripjava As New Classscrripjava
        Try
            Dim pasw As String = ""
            Dim id_cuenta As Integer = -1
            Dim nombre_configuracion As String = ""
            Result = Refclas.Retorna_datos_correo_usuario_remitente_radicacion(Hidden_cuenta_correo_envio.Value, _
                                                                               pasw, _
                                                                               id_cuenta, _
                                                                               nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.Updatepanel_contenedor_controles)
                Exit Sub
            End If
            Result = Refclas.Lista_tipos_configuracion_correos(Me.DropDownList_tipo_cuentas, nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.Updatepanel_contenedor_controles)
                Exit Sub
            End If
            Me.Hidden_tipo_cuenta_correo.Value = nombre_configuracion
            Me.Hidden_id_tipo_cuenta.value = id_cuenta
            Me.TextBox_cuenta_correo.Text = Hidden_cuenta_correo_envio.Value
            UpdatePane_configura_cuenta.Update()
            Me.ModalPopupExtender_configura_cuenta.Show()
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.Updatepanel_contenedor_controles)
        End Try
    End Sub

    Private Sub Button_actualiza_cuenta_Click(sender As Object, e As EventArgs) Handles Button_actualiza_cuenta.Click
        Dim Result As String = ""
        Dim Refclas As New ClassCorreo
        Dim scripjava As New Classscrripjava
        Try
            If Me.TextBox_pasword.Text = "" Then
                scripjava.Showscripman("Debe digitar el pasword", Updatepanel_configura_cuenta_botones)
                Me.TextBox_pasword.Focus()
                Exit Sub
            End If

            If Me.Hidden_id_tipo_cuenta.Value = "-1" Then
                Dim id_tipo_cuenta As Integer = 0
                Result = Refclas.Retorna_tipo_configuracion_correos(Me.DropDownList_tipo_cuentas.Text, _
                                                                    id_tipo_cuenta)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Updatepanel_configura_cuenta_botones)
                    Exit Sub
                End If
                Result = Refclas.Registra_configuracion_correo_remitente(id_tipo_cuenta, _
                                                                         Hidden_cuenta_correo_envio.Value, _
                                                                         Me.TextBox_pasword.Text)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Updatepanel_configura_cuenta_botones)
                    Exit Sub
                End If
            Else
                Dim id_tipo_cuenta As Integer = 0
                Result = Refclas.Retorna_tipo_configuracion_correos(Me.DropDownList_tipo_cuentas.Text, _
                                                                    id_tipo_cuenta)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Updatepanel_configura_cuenta_botones)
                    Exit Sub
                End If
                Result = Refclas.Actualiza_configuracion_correo_remitente(id_tipo_cuenta, _
                                                                          Hidden_cuenta_correo_envio.Value, _
                                                                          Me.TextBox_pasword.Text)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Updatepanel_configura_cuenta_botones)
                    Exit Sub
                End If
            End If
            ModalPopupExtender_configura_cuenta.Hide()
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.Updatepanel_configura_cuenta_botones)
        End Try
    End Sub

    Private Sub Button_notificar_Click(sender As Object, e As EventArgs) Handles Button_notificar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassCorreo
            Dim Result As String = ""
            Dim id_plantilla As Object
            If Me.Hidden_id_plantilla_radicado.Value = "" Then
                id_plantilla = 0
            Else
                id_plantilla = Me.Hidden_id_plantilla_radicado.Value
            End If
            Dim ruta_tempo As String = HttpContext.Current.Session.Item("GA_RUTA_TEMPORAL_DESCARGA_ARCHIVO_CORREO")
            Me.Hidden00001.Value = ""
            Dim tipo_envio As Integer = 0
            If Me.CheckBox_tipo_envio_correo.Checked = True Then
                tipo_envio = 1
                ruta_tempo = Session.Item("WF_RUTA_LINK")
            Else
                tipo_envio = 0
            End If
            If Hidden_tipo_notificacion.Value = "ENVIO CORESPONDENCIA" Then
                Result = Refclas.inicia_notificacion_correo_envio_correspondencia(Me.hdnEmailID_VAL, _
                                                                                  Me.Hidden_text_user_correo.Value, _
                                                                                  Me.TextBox_nota_noti_ficacion, _
                                                                                  Me.Hidden_cuenta_correo_envio, _
                                                                                  Hidden_tipo_notificacion.Value, _
                                                                                  CheckBox_imagen_adjunta, _
                                                                                  Me.CheckBox_pdf, _
                                                                                  Me.CheckBox_lectura, _
                                                                                  Me.CheckBox_pasw, _
                                                                                  id_plantilla, _
                                                                                  ruta_tempo, _
                                                                                  Me.TextBox_asunto_notificacion, _
                                                                                  tipo_envio)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.Updatepanel_contenido_botones, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Hidden00001.Value = "YES"
                End If
            End If
            If Hidden_tipo_notificacion.Value = "NOTIFICA CORESPONDENCIA" Then
                Result = Refclas.inicia_notificacion_correo_envio_correspondencia(Me.hdnEmailID_VAL, _
                                                                                  Me.Hidden_text_user_correo.Value, _
                                                                                  Me.TextBox_nota_noti_ficacion, _
                                                                                  Me.Hidden_cuenta_correo_envio, _
                                                                                  Me.Hidden_tipo_notificacion.Value, _
                                                                                  Me.CheckBox_imagen_adjunta, _
                                                                                  Me.CheckBox_pdf, _
                                                                                  Me.CheckBox_lectura, _
                                                                                  Me.CheckBox_pasw, _
                                                                                  id_plantilla, _
                                                                                  ruta_tempo, _
                                                                                  Me.TextBox_asunto_notificacion, _
                                                                                  tipo_envio)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.Updatepanel_contenido_botones, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Hidden00001.Value = "YES"
                    Exit Sub
                End If
            End If
            'ENVIO CORREO WORKFLOW
            If Hidden_tipo_notificacion.Value = "ENVIO CORREO PRODUCCION" Then
                Dim ruta As String = HttpContext.Current.Session.Item("GA_RUTA_TEMPORAL_DESCARGA_ARCHIVO_CORREO") = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\DESCARGA\"
                Result = Refclas.inicia_notificacion_correo_envio_correspondencia(Me.hdnEmailID_VAL, _
                                                                                  Me.Hidden_text_user_correo.Value, _
                                                                                  Me.TextBox_nota_noti_ficacion, _
                                                                                  Me.Hidden_cuenta_correo_envio, _
                                                                                  Me.Hidden_tipo_notificacion.Value, _
                                                                                  Me.CheckBox_imagen_adjunta, _
                                                                                  Me.CheckBox_pdf, _
                                                                                  Me.CheckBox_lectura, _
                                                                                  Me.CheckBox_pasw, _
                                                                                  0, _
                                                                                  ruta_tempo, _
                                                                                  Me.TextBox_asunto_notificacion, _
                                                                                  tipo_envio)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.Updatepanel_contenido_botones, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Hidden00001.Value = "YES"
                    Exit Sub
                End If
            End If

            If Hidden_tipo_notificacion.Value = "ENVIO CORREO WORKFLOW" Then
                Result = Refclas.inicia_notificacion_correo_envio_correspondencia(Me.hdnEmailID_VAL, _
                                                                                  Me.Hidden_text_user_correo.Value, _
                                                                                  Me.TextBox_nota_noti_ficacion, _
                                                                                  Me.Hidden_cuenta_correo_envio, _
                                                                                  Me.Hidden_tipo_notificacion.Value, _
                                                                                  Me.CheckBox_imagen_adjunta, _
                                                                                  Me.CheckBox_pdf, _
                                                                                  Me.CheckBox_lectura, _
                                                                                  Me.CheckBox_pasw, _
                                                                                  0, _
                                                                                  ruta_tempo, _
                                                                                  Me.TextBox_asunto_notificacion, _
                                                                                  tipo_envio)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.Updatepanel_contenido_botones, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Hidden00001.Value = "YES"
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_contenido_botones)
        End Try

    End Sub
End Class