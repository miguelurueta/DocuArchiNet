Public Class WebFormRespuestaFormalRadicado
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Not Me.IsPostBack Then

            'Dim Result As String = ""
            'Dim Refclas As New ClassWorkflow
            'Dim Radicado As String = ""
            'Result = Refclas.Lista_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), Radicado)
            'If Result <> "YES" Then

            '    Exit Sub
            'End If
            'If Radicado = "" Then
            '    Label_result.Text = "La tarea seleccionada no tiene radicado relacionado "
            '    Exit Sub
            'End If
            'Dim refclas_resp As New Classgestionrespuesta
            'Dim id_respuesta As Integer = 0
            'Result = refclas_resp.Retorna_id_respuesta_radicado(Radicado, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), id_respuesta)
            'If Result <> "YES" Then
            '    Label_result.Text = Result
            '    Exit Sub
            'End If
            'If id_respuesta = 0 Then
            '    Label_result.Text = "El radicado actual no tiene una respuesta relacionada"
            '    Exit Sub
            'End If
            ''Label_estado_resultado.Text = "Respuesta tramite radicado " & Radicado & " codigo respuesta " & id_respuesta
            ''Me.Hidden_radicado.Value = Radicado
            ''Me.Hidden_id_respuesta.Value = id_respuesta

            'Result = refclas_resp.Lista_documento_respuesta_drowlis(Hidden_id_respuesta.Value, Me.DropDownList_lista_plantillas)
            'If Result <> "YES" Then
            '    Label_estado_resultado.Text = Result
            'Else

            'End If


            'Me.htmlEditor.config.toolbar = New Object() {
            '    New Object() {"Source", "-", "NewPage", "Preview", "-", "Templates"},
            '    New Object() {"Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Print", "SpellChecker", "Scayt"},
            '    New Object() {"Undo", "Redo", "-", "Find", "Replace", "-", "SelectAll", "RemoveFormat"},
            '    "/",
            '    New Object() {"Bold", "Italic", "Underline", "Strike", "-", "Subscript", "Superscript"},
            '    New Object() {"NumberedList", "BulletedList", "-", "Outdent", "Indent"},
            '    New Object() {"JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock"},
            '    New Object() {"Link", "Unlink"},
            '    New Object() {"Image", "Table", "SpecialChar", "PageBreak"},
            '    "/",
            '    New Object() {"Styles", "Format", "Font", "FontSize"},
            '    New Object() {"TextColor", "BGColor"},
            '    New Object() {"Maximize", "ShowBlocks"}}


        End If

    End Sub

    Protected Sub Button_descarga_plantilla_Click(sender As Object, e As EventArgs) Handles Button_descarga_plantilla.Click
        'ModalPopupExtender_edition_html.Show()
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
                scrijava.Showscripman("Por favor active web service para workflow", UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            If Session.Item("URL_WEB_SERVICE") = "" Then
                scrijava.Showscripman("Por favor informe la url web service para workflow", UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman("El tramite actual no tiene asignada una respuesta", UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Dim refclas_gestion As New ClassGestion
            Dim id_respuesta As Integer = Me.Hidden_id_respuesta.Value
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim conten As Object = Nothing
            Result = refclas_gestion.Descarga_documento_plantilla_respuesta(Me.Page, ruta_fisica, ruta_virtual, id_respuesta, conten, Hidden_radicado.Value)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Result = Refclas.SaveFile(ruta_fisica & Me.Hidden_id_respuesta.Value & ".docx", conten)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Hidden_ruta_archivo.Value = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/" + Me.Hidden_id_respuesta.Value + ".docx"
            ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
            updatapanel_iframe.Update()
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, UpdatePanel_respuesta_documento)
        End Try
    End Sub

    Protected Sub Button_carga_plantilla_Click(sender As Object, e As EventArgs) Handles Button_carga_plantilla.Click
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman("El tramite actual no tiene asignada una respuesta", UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Hidden_id_respuesta.Value, _
                                                                                id_imagen_plantilla, _
                                                                                radicado_respuesta, _
                                                                                fecha_respuesta, _
                                                                                id_imagen_respuesta, _
                                                                                estado_envio_respuesta)
            If fecha_respuesta <> 0 And radicado_respuesta <> 0 Then
                scrijava.Showscripman("El tramite ya tiene una respuesta asociada, imposible cargar plantilla", UpdatePanel_respuesta_documento)
                Exit Sub
            End If

            Label_estado_carga.Text = "Por favor cargue el archivo " & Hidden_id_respuesta.Value & "docx"
            UpdatePanel_descarga.Update()
            ModalPopupExtender_edition_sube_documento_respuesta.Show()
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, UpdatePanel_respuesta_documento)
        End Try
    End Sub

    Private Sub Button_cargar_Click(sender As Object, e As EventArgs) Handles Button_cargar.Click
       
    End Sub

   
    

    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
            Session.Item("WF_ERROR_RESPUESTA") = ""
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim scrijava As New Classscrripjava
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & ".docx"
            If IO.File.Exists(archivo_donwload) Then
                Kill(archivo_donwload)
            End If
            Me.AjaxFileUpload_dowload.SaveAs(ruta_fisica & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & ".docx")
        Catch ex As Exception
            Session.Item("WF_ERROR_RESPUESTA") = ex.Message

        End Try
    End Sub

    Private Sub Button_sube_documento_Click(sender As Object, e As EventArgs) Handles Button_sube_documento.Click
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                scrijava.Showscripman(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga)
                ModalPopupExtender_edition_sube_documento_respuesta.Hide()
                Exit Sub
            End If
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/"
            Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & ".docx"
            Result = Refclas.Descarga_archivo_donwload(archivo_donwload, Hidden_id_respuesta.Value)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, Me.UpdatePanel_descarga)
                ModalPopupExtender_edition_sube_documento_respuesta.Hide()
                Exit Sub
            Else
                Dim id_imagen As Integer = 0
                Result = Refclas.Guardar_Documento_Respuesta(id_imagen, "IMP03GESTIONTMP", Hidden_id_respuesta.Value, archivo_donwload, 1)
                If Result <> "YES" Then
                    scrijava.Showscripman(Result, Me.UpdatePanel_descarga)
                    ModalPopupExtender_edition_sube_documento_respuesta.Hide()
                    Exit Sub
                Else
                    Me.DropDownList_lista_plantillas.Items.Clear()
                    Me.DropDownList_lista_plantillas.Items.Add(Hidden_id_respuesta.Value)
                    UpdatePanel_respuesta_documento.Update()
                End If
            End If
            Dim reflclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = reflclas_resp_radicado.Solicita_estados_semaforo_respuesta(Hidden_id_respuesta.Value, Me.Image_estado_resp)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, Me.UpdatePanel_descarga)

            End If
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, Me.UpdatePanel_descarga)
        End Try
    End Sub

    Private Sub Button_inicio_respuesta_Click(sender As Object, e As EventArgs) Handles Button_inicio_respuesta.Click

        Dim scrijava As New Classscrripjava
        Try
            Dim refclas_resp As New Classgestionrespuesta
            Dim Result As String = ""
            Result = refclas_resp.Lista_documento_respuesta_drowlis(Hidden_id_respuesta.Value, Me.DropDownList_lista_plantillas)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Dim tipo_respuesta_tramite As Integer = 0
            Result = refclas_resp.Retorna_estado_envio_por_id_respuesta(Hidden_id_respuesta.Value, tipo_respuesta_tramite)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Me.Hidden_tipo_respuesta.Value = tipo_respuesta_tramite
            Dim reflclas_resp_radicado As New Class_ra_respuesta_radicado
            If tipo_respuesta_tramite = 0 Then
                Result = reflclas_resp_radicado.Solicita_estados_semaforo_respuesta(Hidden_id_respuesta.Value, Me.Image_estado_resp)
                If Result <> "YES" Then
                    scrijava.Showscripman(Result, UpdatePanel_respuesta_documento)

                End If
            Else
                Dim ref_clas_resp As New Class_ra_respuesta_radicado
                Result = ref_clas_resp.Solicita_estados_semaforo_respuesta_electronica(Hidden_id_respuesta.Value, Me.Image_estado_resp)
                If Result <> "YES" Then
                    scrijava.Showscripman(Result, UpdatePanel_respuesta_documento)

                End If
            End If
            UpdatePanel_image_semaforo.Update()
            UpdatePanel_combo_plantillas.Update()
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, UpdatePanel_respuesta_documento)
        End Try
    End Sub

    Private Sub WebFormRespuestaFormalRadicado_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete

    End Sub

    Private Sub WebFormRespuestaFormalRadicado_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

    End Sub

    Private Sub Button_descarga_Click(sender As Object, e As EventArgs) Handles Button_descarga.Click
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Hidden_id_respuesta.Value = "-1" Then
                scrijava.Showscripman("El tramite actual no tiene asignada una respuesta", UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Dim id_imagen As Integer = -1
            Dim gabinete_imagen As String = ""
            Dim refclas_gestion As New Classgestionrespuesta
            Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(Hidden_id_respuesta.Value, gabinete_imagen, id_imagen)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Dim refclas_visualiza As New ClassVisualisaDocumento
            Dim id_respuesta As Integer = Me.Hidden_id_respuesta.Value
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim matri_documento() As String = Nothing
            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen, "IMP03GESTIONTMP", matri_documento)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, UpdatePanel_respuesta_documento)
                Exit Sub
            End If

            Dim fil_ext As String = FileIO.FileSystem.GetFileInfo(matri_documento(1)).Extension
            Dim ruta_local As String = Server.MapPath("../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
            Dim file_copia As String = ruta_local & "documento_plantilla_respuesta" & fil_ext
            If FileIO.FileSystem.FileExists(file_copia) Then
                Kill(file_copia)
            End If
            FileCopy(matri_documento(1), file_copia)
            Hidden_ruta_archivo.Value = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/" + "documento_plantilla_respuesta" & fil_ext
            ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
            updatapanel_iframe.Update()
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, UpdatePanel_respuesta_documento)
        End Try
    End Sub

    Protected Sub Button_radicar_tramite_Click(sender As Object, e As EventArgs) Handles Button_radicar_tramite.Click
        Dim scrijava As New Classscrripjava
        Dim refclasradicado As New ClassRadicador
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Try
            Dim estado_documento As String = "YES"
            Result = Refclas.Verifica_existencia_documento_de_respuesta(Hidden_id_respuesta.Value, estado_documento)
            If estado_documento <> "YES" Then
                scrijava.Showscripman("La respuesta actual no tiene un documento de respuesta asociado, por favor cargue el documento", UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Result = Refclas.Prepara_interface_radica_confirma_respuesta(Me.Hidden_id_respuesta.Value, Me.Hidden_radicado.Value, Me.Page, Me.Hidden_tipo_respuesta.Value)
            If estado_documento <> "YES" Then
                scrijava.Showscripman(Result, UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_radica_documento_respuesta.Show()
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, UpdatePanel_respuesta_documento)
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
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, _
                                                                          nombre_plantilla)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_bton)
                Exit Sub
            End If
            Result = Refclas.Retorna_id_script_validacion(id_plantilla, "DINAMICOEXTERNO", "REMITENTE_COR", id_escrip)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_bton)
                Exit Sub
            End If
            If id_escrip = -1 Or id_escrip = 0 Then
                Clasjava.Showscripman("No hay plantilla relacionada para el campo", UpdatePanel_bton)
                Exit Sub
            End If
            Dim nombre_plantilla_validacion As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_escrip, nombre_plantilla_validacion)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_bton)
                Exit Sub
            End If
            Result = Refclas.Verifica_Permisos_usuario_plantilla_validacion(id_escrip, nombre_plantilla_validacion, 0)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_bton)
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
            Clasjava.Showscripman(ex.Message, UpdatePanel_bton)
        End Try
    End Sub

    Protected Sub Button_confirmar_Click(sender As Object, e As EventArgs) Handles Button_confirmar.Click

        'Dim Result As String = ""
        'Dim Refclas As New ClassRadicador
        'Dim Refclasgestion As New Classgestionrespuesta
        'Dim Clasjava As New Classscrripjava
        'Try
        '    Dim resultado_correo As String = ""
        '    Result = Refclasgestion.Confirma_respuesta_con_radicado(Hidden_id_respuesta.Value, Me.Page, 0, resultado_correo)
        '    If Result <> "YES" Then
        '        Clasjava.Showscripman(Result, UpdatePanel_contenido_radica_documento)
        '        Exit Sub
        '    End If
        '    Dim reflclas_resp_radicado As New Class_ra_respuesta_radicado
        '    If Me.Hidden_tipo_respuesta.Value = 0 Then
        '        Result = reflclas_resp_radicado.Solicita_estados_semaforo_respuesta(Hidden_id_respuesta.Value, Me.Image_estado_resp)
        '        If Result <> "YES" Then
        '            Clasjava.Showscripman(Result, UpdatePanel_contenido_radica_documento)
        '            Exit Sub
        '        End If
        '    Else
        '        Dim ref_clas_resp As New Class_ra_respuesta_radicado
        '        Result = ref_clas_resp.Solicita_estados_semaforo_respuesta_electronica(Hidden_id_respuesta.Value, Me.Image_estado_resp)
        '        If Result <> "YES" Then
        '            Clasjava.Showscripman(Result, UpdatePanel_contenido_radica_documento)
        '            Exit Sub
        '        End If
        '    End If

        '    UpdatePanel_image_semaforo.Update()
        '    If resultado_correo <> "" Then
        '        Clasjava.Showscripman(resultado_correo, UpdatePanel_contenido_radica_documento)
        '        Exit Sub
        '    End If
        'Catch ex As Exception
        '    Clasjava.Showscripman(ex.Message, UpdatePanel_contenido_radica_documento)
        'End Try
    End Sub

    Protected Sub Button_confirmar_tramite_Click(sender As Object, e As EventArgs) Handles Button_confirmar_tramite.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim Refclasgestion As New Classgestionrespuesta
        Dim Clasjava As New Classscrripjava
        Try
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Hidden_id_respuesta.Value, _
                                                                                id_imagen_plantilla, _
                                                                                radicado_respuesta, _
                                                                                fecha_respuesta, _
                                                                                id_imagen_respuesta, _
                                                                                estado_envio_respuesta)
            If id_imagen_respuesta <> 0 Then
                Clasjava.Showscripman("El tramite ya tiene una respuesta publicada, imposible guardar y públicar el documento de respuesta", UpdatePanel_respuesta_documento)
                Exit Sub
            End If
            'Result = Refclasgestion.Almacena_documento_respuesta_permanente(Hidden_id_respuesta.Value)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_respuesta_documento)
                Exit Sub
            End If
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePanel_respuesta_documento)
        End Try
    End Sub

    Private Sub Button_notificar_correo_Click(sender As Object, e As EventArgs) Handles Button_notificar_correo.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim Refclasgestion As New Classgestionrespuesta
        Dim Clasjava As New Classscrripjava
        Try
            Result = Refclasgestion.Confirma_respuesta_al_correo_con_radicado(Hidden_id_respuesta.Value, Me.Page, Me.TextBox_correo_electronico.Text, 1)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_contenido_radica_documento)
                Exit Sub
            End If
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePanel_contenido_radica_documento)
        End Try
    End Sub

    

    
End Class