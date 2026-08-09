Public Class WebFormRaRegistrarguia
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            Dim Result As String = ""
            Dim scripjava As New Classscrripjava
            Dim refclas As New ClassRaEnvioCorrespondencia
            Dim refclasradicado As New ClassRadicador
            'If Me.IsPostBack = False Then
            '    Result = refclas.Lista_tramites_por_enviar(Me.Page)
            '    If Result <> "YES" Then
            '        scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
            '    End If
            'End If

            If Me.IsPostBack = False Then
                '**********************************************
                'Crea directori temporal workflow
                '**********************************************
                Dim Refclas_rad As New ClassInicioRadicador
                Result = Refclas_rad.Crea_Dir_Temporal_ra()
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & Result & vbCrLf
                    Label_result.Text = Result & vbCrLf
                Else
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & Result & vbCrLf
                End If
                Result = refclas.Retorna_operarios_mensajeria_gestion(Me.DropDownList_mensajero_interno, Me.DropDownList_mensajero_interno.Text)
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result
                    Exit Sub
                End If
                Result = refclasradicado.Lista_Destinatario_Interno(DropDownList_remit_dest_interno)
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas.Lista_empresa_envio(Me.DropDownList_procesa_tramite_envio, Me.DropDownList_procesa_tramite_envio.Text)
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result
                    Exit Sub
                Else
                    Dim id_tipo_empresa As Integer = 0
                    If Me.DropDownList_procesa_tramite_envio.Text <> "" Then
                        Result = refclas.retorna_tipo_empresa_registro_guia(Me.DropDownList_procesa_tramite_envio.Text, id_tipo_empresa)
                        If Result <> "YES" Then
                            Label_result.Text = Label_result.Text & Result
                            Exit Sub
                        End If
                        If id_tipo_empresa = 1 Then
                            Me.TextBox_codigo_guia_envio.Enabled = False
                            Me.DropDownList_mensajero_interno.Enabled = True
                            Me.Label_tramite.Text = "Operador interno (El sistema genera el consecutivo de guía, seleccione el operador interno de mensajería)"
                        Else
                            Me.TextBox_codigo_guia_envio.Enabled = True
                            Me.DropDownList_mensajero_interno.Enabled = False
                            Me.Label_tramite.Text = "Operador externo (debe digitar el número de guía que suministra el operador externo)"
                        End If

                    End If
                End If

            End If
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = 0
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, nombre_plantilla)
            If Result <> "YES" Then
                Label_result.Text = Label_result.Text & Result
            Else
                If nombre_plantilla <> "" Then
                    Result = refclasradicado.agregar_auto_complete(Me.TextBox_RADICADO.ID.ToString, _Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", nombre_plantilla, "Consecutivo_Rad")
                    If Result <> "YES" Then
                        Label_result.Text = Label_result.Text & Result
                        Exit Sub
                    End If
                End If
            End If
            Result = refclas.Auto_completar_registrar_guia(Me.Page)
            If Result <> "YES" Then
                Label_result.Text = Label_result.Text & Result
                Exit Sub
            End If
        Catch ex As Exception
            Label_result.Text = Label_result.Text & ex.Message
        End Try
    End Sub
    Private Sub Button_tipo_empresa_guia_Click(sender As Object, e As EventArgs) Handles Button_tipo_empresa_guia.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            Dim id_tipo_empresa As Integer = 0
            If Me.DropDownList_procesa_tramite_envio.Text <> "" Then
                Result = refclas.retorna_tipo_empresa_registro_guia(Me.DropDownList_procesa_tramite_envio.Text, id_tipo_empresa)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_bonones_ocultos)
                    Exit Sub
                End If
                If id_tipo_empresa = 1 Then
                    Me.TextBox_codigo_guia_envio.Enabled = False
                    Me.DropDownList_mensajero_interno.Enabled = True
                    Me.Label_tramite.Text = "Operador interno (El sistema genera el consecutivo de guía, seleccione el operador interno de mensajería)"
                Else
                    Me.TextBox_codigo_guia_envio.Enabled = True
                    Me.DropDownList_mensajero_interno.Enabled = False
                    Me.Label_tramite.Text = "Operador externo (debe digitar el número de guía que suministra el operador externo)"
                End If
                Me.UpdatePanel_procesa_tramite_envio.Update()
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_bonones_ocultos)
        End Try
    End Sub
    Private Sub Button_gestion_remitente_entrante_Click(sender As Object, e As EventArgs) Handles Button_gestion_remitente_entrante.Click

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
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            Result = Refclas.Retorna_id_script_validacion(id_plantilla, "DINAMICOEXTERNO", "REMITENTE_COR", id_escrip)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            If id_escrip = -1 Or id_escrip = 0 Then
                Clasjava.Showscripman("No hay plantilla relacionada para el campo", UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            Dim nombre_plantilla_validacion As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_escrip, nombre_plantilla_validacion)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            Result = Refclas.Verifica_Permisos_usuario_plantilla_validacion(id_escrip, nombre_plantilla_validacion)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            If Me.TextBox_NOMBRE_RAZON_SOCIAL.Text <> "" Then
                Result = refclasraenv.asigna_remitente_destinatario_id_dest(Me.Page, Me.TextBox_NOMBRE_RAZON_SOCIAL.Text, HttpContext.Current.Session("RA_ID_DEST_EXTERNO"))
                If Result <> "YES" Then
                    Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                    Exit Sub
                End If
            End If
            Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = id_escrip
            Me.Iframe_validacion_plantilla_.Attributes.Add("src", "../radicador/WebFormGestionPlantillasvalidacion.aspx")
            Me.UpdatePanel_validacion_plantilla.Update()
            Me.ModalPopupExtender_valiacion_plantilla.Show()
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePanel_procesa_tramite_envio)
        End Try
    End Sub
    Private Sub Button_Asigana_datos_validacion_edicion_Click(sender As Object, e As EventArgs) Handles Button_Asigana_datos_validacion_edicion.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        Try
            Result = refclas.asigna_remitente_destinatario_interface_guia(Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, updatepanel_Asigana_datos_validacion_edicion)
            End If
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, updatepanel_Asigana_datos_validacion_edicion)
        End Try
    End Sub

    Private Sub Button_Asigana_datos_validacion_edicion_manual_Click(sender As Object, e As EventArgs) Handles Button_Asigana_datos_validacion_edicion_manual.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        Try
            Result = refclas.asigna_remitente_destinatario_interface_guia_manual(Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, updatepanel_Asigana_datos_validacion_edicion)
            End If
            Me.UpdatePanel_procesa_tramite_envio.Update()
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, updatepanel_Asigana_datos_validacion_edicion)
        End Try
    End Sub

    Private Sub Button_Asigana_datos_validacion_edicion_manual__Click(sender As Object, e As EventArgs) Handles Button_Asigana_datos_validacion_edicion_manual_.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        Try
            Result = refclas.asigna_remitente_destinatario_interface_guia_manual(Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
            End If
            'Me.UpdatePanel_procesa_tramite_envio.Update()
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePanel_procesa_tramite_envio)
        End Try
    End Sub

    Private Sub Button_registrar_Click(sender As Object, e As EventArgs) Handles Button_registrar.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        If HttpContext.Current.Session.Item("RA_PERMISO_GENERAR_GUIA") = 0 Then
            Clasjava.Showscripman("El usuario no tiene permiso para registrar la guía", UpdatePanel_botones)
            Exit Sub
        End If
        If Hidden_remitente_destinatario.Value = "-1" Then
            Clasjava.Showscripman("Debe seleccionar el destinatario de la guía", UpdatePanel_botones)
            Exit Sub
        End If   
        Dim id_guia As Integer = 0
        Result = refclas.Registra_guia_envio(Me.DropDownList_procesa_tramite_envio.Text, Me.TextBox_codigo_guia_envio.Text, Me.Page, _
        Me.TextBox_NOMBRE_RAZON_SOCIAL.Text, Me.DropDownList_mensajero_interno.Text, Me.TextBox_NIT_IDENTIFICACION.Text, _
         Me.TextBox_DIRECCION.Text, Me.TextBox_TELEFONO.Text, Me.TextBox_CORREO_ELECTRONICO.Text, _
         Me.TextBox_ANEXO.Text, id_guia, Me.DropDownList_remit_dest_interno.Text, Hidden_remitente_destinatario.Value, _
         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), 2, Me.TextBox_RADICADO.Text, 2)
        If Result <> "YES" Then
            Clasjava.Showscripman(Result, UpdatePanel_botones)
            Exit Sub
        Else
            Result = refclas.Limpiar_Campos_registro_guia(Me.Page, 1)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_botones)
                Exit Sub
            End If
            Dim ruta_archivo As String = ""
            Result = refclas.genera_documento_guia(id_guia, ruta_archivo)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_botones)
                Exit Sub
            Else
                Me.Hiddenruta.Value = ruta_archivo
                Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
                UpdatePaneliframe.Update()
                ModalPopupExtenderimpre.Show()
            End If

        End If
    End Sub

    Protected Sub Button_restaurar_Click(sender As Object, e As EventArgs) Handles Button_restaurar.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        Result = refclas.Limpiar_Campos_registro_guia(Me.Page, 1)
        If Result <> "YES" Then
            Clasjava.Showscripman(Result, UpdatePanel_botones)
            Exit Sub
        End If
    End Sub
End Class