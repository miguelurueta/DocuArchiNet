Imports System.IO

Public Class WebFormArchivaenviocorrespo
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
            If Me.IsPostBack = False Then
                'Result = refclas.Lista_tramites_envios_por_archivar(Me.Page)
                'If Result <> "YES" Then
                '    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                'End If
            End If
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
                Dim refclas_radic As New ClassRadicador
                Result = refclas_radic.agregar_auto_complete(Me.TextBoxRadicado_respuesta.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "RADICADO_RESPUESTA")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas_radic.agregar_auto_complete(Me.TextBoxRadicado.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "RADICADO")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas_radic.agregar_auto_complete(Me.TextBoxUSUARIO_RESPONSABLE.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "USUARIO_RESPONSABLE")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas_radic.agregar_auto_complete(Me.TextBoxDESTINATARIO.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "DESTINATARIO")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas_radic.agregar_auto_complete(Me.TextBox_GUIA_ENVIO.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "GUIA_ENVIO")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Dim refclasenvio As New ClassRaEnvioCorrespondencia
                Result = refclasenvio.Retorna_areas_permitidas_para_envio_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), Me.DropDownList_areas_depart, UpdatePanelContenido_val_radicacion)
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclasenvio.Lista_empresa_envio(Me.DropDownList_empresa_envio, "")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                'Dim Refclas_ As New InicioWorkflow
                'Result = Refclas_.Crea_Dir_Temporal_wf(HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString)
                'If Result <> "YES" Then
                'Label_result.Text = Label_result.Text & Result & vbCrLf
                'End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button_consulta_pendientes_procesar_Click(sender As Object, e As EventArgs) Handles Button_consulta_pendientes_procesar.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            Result = refclas.Lista_tramites_envios_por_archivar(Me.Page)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_validacion)
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_validacion)
        End Try
    End Sub

    Private Sub Button_procesar_envio_Click(sender As Object, e As EventArgs) Handles Button_procesar_envio.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman("Debe seleccionar el regitro a procesar", Me.UpdatePanel_botones_procesa_archivo_tramite)
                Exit Sub
            End If
            ModalPopupExtender_procesa_archivo_tramite.Show()
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_procesa_archivo_tramite)
        End Try
    End Sub

    Private Sub Button_procesa_tramite_envio_Click(sender As Object, e As EventArgs) Handles Button_procesa_tramite_envio.Click
       
    End Sub

    Private Sub Button_notificar_envio_Click(sender As Object, e As EventArgs) Handles Button_notificar_envio.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman("Debe seleccionar el regitro a procesar", Me.UpdatePanel_botones_radicacion)
                Exit Sub

            End If
            Dim Correo_resp As String = ""
            Dim id_usuario_gestion As Integer = 0
            Result = refclas.Retorna_id_usuario_gestion_respuesta(Me.hdnEmailID_VAL.Value, id_usuario_gestion)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            If id_usuario_gestion <> 0 Then
                Result = refclas.Retorna_correo_electronico_usuario_gestion(id_usuario_gestion, Correo_resp)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
            End If
            Dim correo_remitente As String = ""
            Result = refclas.Retorna_correo_electronico_usuario_radicador(Session.Item("RA_ID_USUARIO"), correo_remitente)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Me.Hidden_cuenta_correo_envio.Value = correo_remitente
            Me.Hidden_correo_envio_default.Value = Correo_resp
            Me.ifimpre.Attributes.Add("src", "../radicador/WebFormNotificar.aspx")
            Me.UpdatePaneliframe.Update()
            ModalPopupExtender_notifica_gestion.Show()
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub

    

    Private Sub Button_ver_documento_Click(sender As Object, e As EventArgs) Handles Button_ver_documento.Click
        
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim Refclas_gestion_respuesta As New Classgestionrespuesta
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar el regitro ", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, estru)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estru.ID_TIPO_DOC_RESPUESTA = 1 Then
                Me.ModalPopupExtender_opcion_descarga_respuesta.Show()
            Else
                Result = Refclas_gestion_respuesta.Descarga_documento_respuesta(Me.hdnEmailID_VAL.Value,
                                                                               Me.DropDownList_tipo_archivo.Text,
                                                                               1,
                                                                               Me.ifmExcel_,
                                                                               Me.updatapanel_iframe,
                                                                               Me.Hidden_ruta_archivo)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePane_opcion_descarga_respuesta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.UpdatePanel_botones_radicacion.Update()
                End If
            End If

        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_descarga_docmento_respuesta_Click(sender As Object, e As EventArgs) Handles Button_descarga_docmento_respuesta.Click
        Dim Result As String = ""
        Dim Refclas_gestion_respuesta As New Classgestionrespuesta
        Dim scripjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar el regitro ", Me.UpdatePane_opcion_descarga_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            If Me.Check_opcion_descarga_respuesta_sin_firma.Checked = True Then
                Result = Refclas_gestion_respuesta.Descarga_documento_respuesta(Me.hdnEmailID_VAL.Value,
                                                                                Me.DropDownList_tipo_archivo.Text,
                                                                                0,
                                                                                Me.ifmExcel_,
                                                                                Me.updatapanel_iframe,
                                                                                Me.Hidden_ruta_archivo)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePane_opcion_descarga_respuesta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.UpdatePanel_botones_radicacion.Update()
                End If
            End If
            If Me.CheckBox_opcion_descarga_respuesta_con_firma.Checked = True Then
                Result = Refclas_gestion_respuesta.Descarga_documento_respuesta(Me.hdnEmailID_VAL.Value,
                                                                                Me.DropDownList_tipo_archivo.Text,
                                                                                1,
                                                                                Me.ifmExcel_,
                                                                                Me.updatapanel_iframe,
                                                                                Me.Hidden_ruta_archivo)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePane_opcion_descarga_respuesta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.UpdatePanel_botones_radicacion.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePane_opcion_descarga_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_procesar_archivo_Click(sender As Object, e As EventArgs) Handles Button_procesar_archivo.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            If Me.Hidden_alert_respuesta.Value = "NO" Then Exit Sub
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman("Debe seleccionar el regitro a procesar", Me.UpdatePanel_botones_procesa_archivo_tramite)
                Exit Sub
            End If

            If Me.TextBox_fecha_envio.Text = "" Then
                scripjava.Showscripman("Debe digitar la fecha de recibido", Me.UpdatePanel_botones_procesa_archivo_tramite)
                Me.TextBox_fecha_envio.Focus()
                Exit Sub
            End If

            If Me.TextBox_hora_envio.Text = "" Then
                scripjava.Showscripman("Debe digitar la hora de recibido", Me.UpdatePanel_botones_procesa_archivo_tramite)
                Me.TextBox_fecha_envio.Focus()
                Exit Sub
            End If
            Dim nombre_empresa As String = ""
            Dim numero_guia As String = ""
            Dim radicado_respuesta As String = ""
            Dim id_guia As Integer = 0
            Result = refclas.Retorna_datos_envio_respuesta(Me.hdnEmailID_VAL.Value, nombre_empresa, numero_guia, radicado_respuesta, id_guia)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Result = refclas.Archiva_respuesta(Me.hdnEmailID_VAL.Value, Me.TextBox_fecha_envio.Text, Me.TextBox_hora_envio.Text, id_guia, Me.TextBox_NOTA_CLIENTE.Text, Me.DropDownListESTADO_CONFIRMACION_GUIA.Text)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_procesa_archivo_tramite)
                Exit Sub
            Else
                Hidden_procesa_tramite_envio.Value = "YES"
                Me.ModalPopupExtender_procesa_archivo_tramite.Hide()
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_procesa_archivo_tramite)
        End Try
    End Sub

    Private Sub Button_devolver_envio_Click(sender As Object, e As EventArgs) Handles Button_devolver_envio.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            If Me.Hidden_alert_respuesta.Value = "NO" Then Exit Sub
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman("Debe seleccionar el regitro a procesar", Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Dim nombre_empresa As String = ""
            Dim numero_guia As String = ""
            Dim radicado_respuesta As String = ""
            Dim id_guia As Integer = 0
            Result = refclas.Retorna_datos_envio_respuesta(Me.hdnEmailID_VAL.Value, nombre_empresa, numero_guia, radicado_respuesta, id_guia)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Result = refclas.Devolucion_envio_respuesta(Me.hdnEmailID_VAL.Value, id_guia)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            Else
                UpdatePanel_botones_procesa_archivo_tramite.Update()
                Hidden_procesa_tramite_envio.Value = "YES"

            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub

    Protected Sub Button_lipiar_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_lipiar_val_radicacion.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            Result = refclas.Limpiar_campos_consulta(Me)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_digitaliza_documento_Click(sender As Object, e As EventArgs) Handles Button_digitaliza_documento.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New Class_digitalizacion_guia
        Try
            If Me.Hidden_alert_respuesta.Value = "NO" Then Exit Sub
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman("Debe seleccionar el regitro", Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Result = refclas.Activa_digitalizar_documento_resp_guia(Me.Page, _
                                                                    Val(Me.hdnEmailID_VAL.Value))
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub
    Protected Sub TreeViewseleccion_digitalizado_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeViewseleccion_digitalizado.SelectedNodeChanged
        HttpContext.Current.Session.Item("DG_SELECION_TREE") = TreeViewseleccion_digitalizado.SelectedNode.Value

    End Sub
    'Visualiza el documento digitalizado
    Protected Sub ButtonVisua_Click(sender As Object, e As ImageClickEventArgs) Handles ButtonVisua.Click
        Dim Refclasjavamensaje As New Classscrripjava
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            If Session.Item("WF_TAGSELECCION_EMERGENTE") = "" Then
                If Me.TreeViewseleccion_digitalizado.SelectedNode Is Nothing Then
                    Me.IframeVisor_.Attributes("src") = "../workflow/WebFormFreeImageVisorEmerge.aspx"
                    Me.IframeVisor_.Attributes("Width") = "100%"
                    Me.IframeVisor_.Attributes("Heith") = "100%"
                    'Me.IframeDitaliza.Visible = False
                    Me.Hidden_estado_visor.Value = "Interno"
                    Me.UpdatePanelIframevisor.Update()
                    Exit Sub
                End If
            End If
            If Not Me.TreeViewseleccion_digitalizado.SelectedNode Is Nothing Then
                'Dim Texitem As String = Me.DropDownListLista.SelectedItem.Text
                'Dim slplitlist() As String = Texitem.Split("|")
                Dim ValueItem As String = Me.TreeViewseleccion_digitalizado.SelectedNode.Value.ToString
                Dim spligabi() As String = ValueItem.Split("|")
                Dim id_tipo_imagen As Integer = 0
                Dim extension_imagen As String = ""
                Dim Refclas As New ClassDaGabinete
                Result = Refclas.SolicitaIdTipoImagen(spligabi(1), spligabi(0), id_tipo_imagen)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanelBotones)
                    Exit Sub
                End If
                Dim ClassDaExtension As New Class_da_extension
                Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen, extension_imagen)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanelBotones)
                    Exit Sub
                End If
                Session.Item("WF_TAGSELECCION_EMERGENTE") = "|1|" & spligabi(1) & "|" & extension_imagen & "|ADJUNTO" & "|" & spligabi(0)
            Else

                Exit Sub
            End If


            Dim Valor_Tab_Selccion As String = Session.Item("WF_TAGSELECCION_EMERGENTE")
            'Session.Item("WF_TAGSELECCION_EMERGENTE") = ""
            Dim Valor_Tab_documeto() As String
            Erase Valor_Tab_documeto
            ' Refclasjavamensaje.Showscripman(slplitlist(0), Me.UpdatePanelBotones)
            If Valor_Tab_Selccion = "Documentos workflow" Then
                Exit Sub
            End If
            Valor_Tab_documeto = Split(Valor_Tab_Selccion, "|")
            If Valor_Tab_documeto Is Nothing Then
                Mens.Showscripman("Documento sin datos adjuntos", Me.UpdatePanelBotones)
                Exit Sub
            End If
            Dim refcla As New ClassWorflowVisor
            Dim Resutl As String = ""
            Session.Item("WF_MATRI_IMAGE_EMERGENTE") = ""
            If Valor_Tab_documeto(3) = ".TIF" Or Valor_Tab_documeto(3) = ".JPG" Or Valor_Tab_documeto(3) = ".BMP" Then
                Session.Item("CC_SESIONITERCAMBIOVISOR") = Valor_Tab_documeto(2) & "|" & Valor_Tab_documeto(5)
                Me.IframeVisor_.Attributes("src") = "../gestion/WebFormGaVisorClasificacion.aspx"
                Me.IframeVisor_.Attributes("Width") = "100%"
                Me.IframeVisor_.Attributes("Heith") = "100%"
                Me.Hidden_estado_visor.Value = "Interno"
                Me.UpdatePanelIframevisor.Update()

            Else
                Dim refclas As New ClassVisualisaDocumento
                Dim matri_doc() As String = Nothing
                Resutl = refclas.Genera_Matris_Documentos_Almacenados(Val(Valor_Tab_documeto(2)), Valor_Tab_documeto(5), matri_doc)
                If Resutl = "YES" Then
                    Session.Item("CC_SESIONITERCAMBIOVISOR") = Valor_Tab_documeto(2) & "|" & Valor_Tab_documeto(5)
                    Me.IframeVisor_.Attributes("src") = "../gestion/WebFormGaVisorDocumentoExterno.aspx"
                    Me.IframeVisor_.Attributes("src") = "../gestion/WebFormGaVisorClasificacion.aspx"
                    Me.IframeVisor_.Attributes("Width") = "100%"
                    Me.IframeVisor_.Attributes("Heith") = "100%"
                    Me.Hidden_estado_visor.Value = "Externo"
                    Me.UpdatePanelIframevisor.Update()
                Else
                    Dim Resutla = refcla.Limpia_Visor_emergente_Workflow(Me, "noaming")
                    If Resutla <> "YES" Then
                        Mens.Showscripman(Resutla, Me.UpdatePanelBotones)
                    End If
                    Me.Hidden_estado_visor.Value = "Interno"
                    Mens.Showscripman(Resutl, Me.UpdatePanelBotones)
                    Exit Sub
                End If

            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanelBotones)
            Exit Sub
        End Try
    End Sub
    Protected Sub ImageButton_adjunt_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_adjunt.Click

        Dim refclas_java As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_digitalizacion_guia As New Class_digitalizacion_guia
            If Me.hdnEmailID_VAL.Value = "-1" Then
                refclas_java.Showscripman("Debe seleccionar el regitro", Me.UpdatePanelBotones)
                Exit Sub
            End If
            Result = Ref_class_digitalizacion_guia.Activa_adjuntar_documento_guia_respuesta(Val(Me.hdnEmailID_VAL.Value), _
                                                                                          Me.Page)
            If Result <> "YES" Then
                refclas_java.Showscripman(Result, Me.UpdatePanelBotones)
                Exit Sub
            End If
            'Dim Refclas As New ClassWorkflowDigitalizacion
            'Dim Result As String = ""
            'If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
            '    '---------------------------------------------------------------------------
            '    'Solicita el id configuración digitalización del radicado con el parametro 
            '    ' id tipo de tramite
            '    '---------------------------------------------------------------------------
            '    Result = Refclas.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
            '                                                              Session.Item("DG_TIPO_TRAMITE"), _
            '                                                              Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
            'End If
            'Dim estado_resultado As String = ""
            'If Result = "YES" Then
            '    '---------------------------------------------------------------------------------------
            '    'Solicita tipos documentales relacionados al tramite
            '    '---------------------------------------------------------------------------------------
            '    Result = Refclas.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta(Session.Item("DG_ID_TRAMITE"), _
            '                                                                                                   Session.Item("DG_TIPO_TRAMITE"), _
            '                                                                                                   Me.Page, _
            '                                                                                                   estado_resultado)
            'End If
            'Hidden_0001.Value = "-1"
            'If Result <> "YES" Then
            '    Hidden_0002.Value = "0"
            '    Label_estado_lista_chequeo.Text = Result
            '    UpdateGeneral.Update()
            'End If
            'If estado_resultado = "YES" Then
            '    Hidden_0002.Value = "1"
            '    Me.ModalPopupExtender_edition_lista_chequeo_tramite.Show()
            '    Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
            'Else
            '    Session.Item("DG_LISTA_CHEQUEO") = -1
            '    Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
            '    'AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
            '    'AjaxFileUpload_dowload.AllowedFileTypes = "tif,jpg,tiff,bmp,pdf"
            '    Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
            '    'AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
            '    'UpdatePanel_descarga.Update()
            '    Me.ModalPopupExtender_sube_documento_adjunto.Show()
            'End If
        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, UpdatePanelBotones)
        Finally
            Me.UpdatePanel_lista_chequeo_estado.Update()
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
            
        Catch ex As Exception
            Session.Item("WF_ERROR_RESPUESTA") = ex.Message

        End Try
    End Sub
    Private Sub Button_guardar_desicion_Click(sender As Object, e As EventArgs) Handles Button_guardar_desicion.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_digitalizacion_guia
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                CLAS.Showscripman(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga)
                Exit Sub
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "ESCANER" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                Dim Nombre_Ruta_Workflow As String = ""
                Dim RefclasDigitaliza As New ClassWorkflowDigitalizacion
                Dim datos_enlace As String = Trim(Me.TextBoxDatos.Text)
                Dim ID_DOCUMENTO As Integer = 0
                Dim TIPO_DOCUMENTO As Integer = 0
                Dim id_imagen As Integer = 0
                If Me.TreeViewseleccion_digitalizado.Nodes.Count = 0 Then
                    TIPO_DOCUMENTO = 1
                Else
                    TIPO_DOCUMENTO = 0
                End If
               
                Result = Refclas.Almacena_documentos_digitalizados_con_radicado_guia(Session.Item("WF_RUTA_TEMPO_ADJUNTA"), _
                                                                                     HttpContext.Current.Session.Item("DG_ID_TAREA"), _
                                                                                     HttpContext.Current.Session.Item("DG_ID_RUTA"), _
                                                                                     TIPO_DOCUMENTO, _
                                                                                     Me.TreeViewseleccion_digitalizado, _
                                                                                     HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION"), _
                                                                                     id_imagen, _
                                                                                     1)
                If Result <> "YES" Then
                    CLAS.Showscripman(Result, UpdatePanel_descarga)
                    Exit Sub
                Else
                    Me.Label_relacion_documentos.Text = "Documentos relacionados (" & Me.TreeViewseleccion_digitalizado.Nodes.Count & ")"
                    Me.UpdateDatos.Update()
                End If
                Me.Label_relacion_documentos.Text = "Documentos relacionados (" & Me.TreeViewseleccion_digitalizado.Nodes.Count & ")"
                Me.UpdateDatos.Update()
                Me.UpdatePanelseleccion_digitalizado.Update()
                Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                Me.ModalPopupExtender_edition_lista_chequeo_tramite.Hide()
            End If
            
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePanel_descarga)
        End Try
    End Sub
    Private Sub data_grid_chequeo_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_chequeo.RowCreated
        e.Row.Cells(0).Visible = False
    End Sub
    Private Sub Button_examinar_archivo_lista_chequeo_Click(sender As Object, e As EventArgs) Handles Button_examinar_archivo_lista_chequeo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflowDigitalizacion
            Dim Refclas_workflow As New ClassWorkflow
            Me.Hidden_list_cheo_acepta.Value = ""
            Dim paraban_lista As String = Session.Item("DG_TIPODIGITALIZACION")
            Dim paraban_lista_ As String = Session.Item("DG_ID_CONFIG_DIGITALIZACION")
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                If Session.Item("DG_ID_CONFIG_DIGITALIZACION") <> "-1" Then
                    Result = Refclas.Activa_guardar_documento_digitalizado_relacionado_a_tramite(Session.Item("DG_ID_CONFIG_DIGITALIZACION"), _
                                                                                                 Session.Item("DG_NOMBRE_GABINETE"), _
                                                                                                   Me.Hidden_0001.Value, _
                                                                                                   Session.Item("DG_RADICADO"))
                    If Result <> "YES" Then
                        clasjava.Showscripman(Result, Me.UpdatePanel_lista_chequeo)
                        Exit Sub
                    Else
                        Session.Item("DG_LISTA_CHEQUEO") = Me.Hidden_0001.Value
                        'Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
                        AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
                        AjaxFileUpload_dowload.AllowedFileTypes = "tif,jpg,tiff,bmp,pdf"
                        Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                        AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
                        UpdatePanel_descarga.Update()
                        Me.ModalPopupExtender_edition_lista_chequeo_tramite.Hide()
                        Me.ModalPopupExtender_sube_documento_adjunto.Show()
                    End If
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = -1
                    'Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
                    AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
                    AjaxFileUpload_dowload.AllowedFileTypes = "tif,jpg,tiff,bmp,pdf"
                    Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                    AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
                    UpdatePanel_descarga.Update()
                    Me.ModalPopupExtender_edition_lista_chequeo_tramite.Hide()
                    Me.ModalPopupExtender_sube_documento_adjunto.Show()
                End If

            End If
            'If Session.Item("DG_TIPODIGITALIZACION") = "PREDETERMINADO" Then
            '    If Session.Item("DG_ID_CONFIG_DIGITALIZACION") <> "-1" Then
            '        Result = Refclas.Activa_guardar_documento_digitalizado_relacionado_a_tramite(Session.Item("DG_ID_CONFIG_DIGITALIZACION"), _
            '                                                                                     Session.Item("DG_NOMBRE_GABINETE"), _
            '                                                                                      Me.Hidden_0001.Value, _
            '                                                                                      Session.Item("DG_RADICADO"))
            '        If Result <> "YES" Then
            '            clasjava.Showscripman(Result, Me.UpdatePanel_lista_chequeo)
            '            Exit Sub
            '        Else
            '            Session.Item("DG_LISTA_CHEQUEO") = Me.Hidden_0001.Value
            '            'Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
            '            Result = Refclas_workflow.Adjunta_imagen_default_tipificado(2, _
            '                                                                        Me.Page, _
            '                                                                        0, _
            '                                                                        Session.Item("DG_LISTA_CHEQUEO"))
            '            If Result <> "YES" Then
            '                clasjava.Showscripman(Result, Me.UpdatePanel_lista_chequeo)
            '                Exit Sub
            '            Else
            '                Me.Hidden_list_cheo_acepta.value = "YES"
            '                Me.HiddenField_estado_guarda.Value = "YES"
            '                Me.UpdatePane_seleccion_tipo_adjunto.Update()
            '                Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()
            '                Me.ModalPopupExtender_edition_lista_chequeo_tramite.Hide()
            '                Dim Refclas_select As New Classselecciotarea
            '                If Session.Item("ID_TAREA_SELECCIONDA") = 0 Then Exit Sub
            '                Dim Actividad_Seleccion As Integer = 0
            '                Result = ""
            '                Dim id_actividad As Integer = 0
            '                Result = Refclas_select.Obtener_Numero_Actividades_Selecionadas(id_actividad, HttpContext.Current.Session("Id_Usuario_Workflow"), Actividad_Seleccion)
            '                If Result <> "YES" Then
            '                    clasjava.Showscripman(Result, Me.UpdatePanel_lista_chequeo)
            '                    Exit Sub
            '                End If
            '                Result = Refclas_select.Seleccion_Documentos_wf(Session.Item("ID_TAREA_SELECCIONDA"), Me.TreeViewseleccion, -1, id_actividad, -1, Me.Page)
            '                If Result <> "YES" Then
            '                    clasjava.Showscripman(Result, Me.UpdatePanel_lista_chequeo)
            '                    Exit Sub
            '                Else
            '                    UpdatePanelseleccion.Update()
            '                End If
            '            End If
            '        End If
            '    Else
            '        Session.Item("DG_LISTA_CHEQUEO") = Me.Hidden_0001.Value
            '        'Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
            '        Result = Refclas_workflow.Adjunta_imagen_default(2, Me.Page, 0)
            '        If Result <> "YES" Then
            '            clasjava.Showscripman(Result, Me.UpdatePanel_lista_chequeo)
            '            Exit Sub
            '        Else
            '            Me.Hidden_list_cheo_acepta.Value = "YES"
            '            Me.HiddenField_estado_guarda.Value = "YES"
            '            Me.UpdatePane_seleccion_tipo_adjunto.Update()
            '            Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()
            '            Me.ModalPopupExtender_edition_lista_chequeo_tramite.Hide()
            '            Dim Refclas_select As New Classselecciotarea
            '            If Session.Item("ID_TAREA_SELECCIONDA") = 0 Then Exit Sub
            '            Dim Actividad_Seleccion As Integer = 0
            '            Result = ""
            '            Dim id_actividad As Integer = 0
            '            Result = Refclas_select.Obtener_Numero_Actividades_Selecionadas(id_actividad, HttpContext.Current.Session("Id_Usuario_Workflow"), Actividad_Seleccion)
            '            If Result <> "YES" Then
            '                clasjava.Showscripman(Result, Me.UpdatePanel_lista_chequeo)
            '                Exit Sub
            '            End If
            '            Result = Refclas_select.Seleccion_Documentos_wf(Session.Item("ID_TAREA_SELECCIONDA"), Me.TreeViewseleccion, -1, id_actividad, -1, Me.Page)
            '            If Result <> "YES" Then
            '                clasjava.Showscripman(Result, Me.UpdatePanel_lista_chequeo)
            '                Exit Sub
            '            Else
            '                UpdatePanelseleccion.Update()
            '            End If
            '        End If
            '    End If
            'End If

        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_lista_chequeo)
        End Try
    End Sub
    '-------almacena los documentos digitalizados
    Private Sub ButtonAlmacenar_Click(sender As Object, e As EventArgs) Handles ButtonAlmacenar.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Refclas As New Class_digitalizacion_guia
            Dim Result As String = ""
            Dim ID_DOCUMENTO As Integer = 0
            Dim TIPO_DOCUMENTO As Integer = 0
            Dim id_imagen As Integer = 0
            If Me.TreeViewseleccion_digitalizado.Nodes.Count = 0 Then
                TIPO_DOCUMENTO = 1
            Else
                TIPO_DOCUMENTO = 0
            End If
            Dim Ruta_Web_Escaner As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString.Replace("\", "/")
            Result = Refclas.Almacena_documentos_digitalizados_con_radicado_guia(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"), _
                                                                                  HttpContext.Current.Session.Item("DG_ID_TAREA"), _
                                                                                  HttpContext.Current.Session.Item("DG_ID_RUTA"), _
                                                                                  TIPO_DOCUMENTO, _
                                                                                  Me.TreeViewseleccion_digitalizado, _
                                                                                  HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION"), _
                                                                                  id_imagen, _
                                                                                  0)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdateDatos)
                Exit Sub
            End If
            'Dim Nombre_Ruta_Workflow As String = ""
            'Dim RefclasDigitaliza As New ClassWorkflowDigitalizacion
            'If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
            '    Result = RefclasDigitaliza.Activa_almacenamiento_digitalizado(Me.TextBoxDatos, _
            '                                                                  Me.TreeViewseleccion_digitalizado, _
            '                                                                  Me.Label_relacion_documentos, _
            '                                                                  Me.UpdateDatos, _
            '                                                                  Me.UpdatePanelseleccion_digitalizado)
            '    If Result <> "YES" Then
            '        Mens.Showscripman(Result, Me.UpdateDatos)
            '        Exit Sub
            '    End If
            'End If
            'If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE_ADJUNTOWORKFLOW" Then
            '    Dim id_imagen As Long = 0
            '    Result = RefclasDigitaliza.Almacenamiento_digitalizados_a_flujo_trabajo(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
            '                                                                            id_imagen, _
            '                                                                            Me.TreeViewseleccion, _
            '                                                                            Me.UpdatePanelseleccion, _
            '                                                                            Me.UpdatePanel_label_seleccion, _
            '                                                                            Me.Label_docu_relacionado_wf)
            '    If Result <> "YES" Then
            '        Mens.Showscripman(Result, Me.UpdateDatos)
            '        Exit Sub
            '    Else
            '        Me.ModalPopupExtender_edition_digitaliza_documento_adjunto.Hide()
            '        Exit Sub
            '    End If
            'End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdateDatos)
        End Try
    End Sub
    Protected Sub ImageButtonActivaClasifica_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonActivaClasifica.Click
        Dim refclas_java As New Classscrripjava
        Try
            If Me.TreeViewseleccion_digitalizado.SelectedNode Is Nothing Then
                Exit Sub
            End If
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
                                                                          Session.Item("DG_TIPO_TRAMITE"), _
                                                                          Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
            End If
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            If Result = "YES" Then
                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta(Session.Item("DG_ID_TRAMITE"), _
                                                                                                                       Session.Item("DG_TIPO_TRAMITE"), _
                                                                                                                       Me.Page)
            End If
            Hidden_0003.Value = "-1"
            If Result <> "YES" Then
                Me.data_grid_chequeo.DataSource = Nothing
                Me.data_grid_chequeo.DataBind()
                Hidden_0004.Value = "0"
                Label_estado_lista_chequeo.Text = Result
                UpdateGeneral.Update()
            Else
                Hidden_0004.Value = "1"
                Me.ModalPopupExtender_edition_lista_chequeo_actualiza.Show()
            End If

        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, UpdatePanelBotones)
        Finally
            Me.UpdatePanel_lista_chequeo_estado_actualiza.Update()
        End Try
    End Sub
    Private Sub data_grid_chequeo_actualiza_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_chequeo_actualiza.RowCreated
        e.Row.Cells(0).Visible = False
    End Sub
    Private Sub Button_Actualizar_item_lista_Click(sender As Object, e As EventArgs) Handles Button_Actualizar_item_lista.Click
        Dim refclas_java As New Classscrripjava
        Try
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                If Me.TreeViewseleccion_digitalizado.SelectedNode Is Nothing Then
                    Exit Sub
                End If
                Dim id_tarea As Long = HttpContext.Current.Session.Item("DG_ID_TAREA")
                Dim Refclas As New ClassWorkflowDigitalizacion
                Dim Result As String = ""
                Dim id_configuracion As Integer = 0
                Dim Refclas_digitalizacion As New ClassWorkflowDigitalizacion
                Result = Refclas_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna(HttpContext.Current.Session.Item("DG_ID_RUTA"), _
                                                                                 id_tarea, _
                                                                                 "", _
                                                                                 0, _
                                                                                 0, _
                                                                                 "", _
                                                                                 id_configuracion, _
                                                                                 "", _
                                                                                 "")
                Dim split() As String = Me.TreeViewseleccion_digitalizado.SelectedNode.Value.ToString.Split("|")
                Result = Refclas.Actualiza_tipo_documento_lista_chequeo(split(1), Hidden_0003.Value, _
                                                                        split(0), Me.TreeViewseleccion_digitalizado.SelectedNode, _
                                                                        Session.Item("DG_ID_CONFIG_DIGITALIZACION"), split(2), _
                                                                         Me.UpdatePanelseleccion_digitalizado, "")
                If Result <> "YES" Then
                    refclas_java.Showscripman(Result, UpdatePanel_lista_chequeo_actualiza)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_lista_chequeo_actualiza.Hide()
                End If
            End If
           
        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, UpdatePanel_lista_chequeo_actualiza)
        End Try
    End Sub
    'Elimina imagenes almacenadas
    Protected Sub ButtonElimina_Click(sender As Object, e As ImageClickEventArgs) Handles ButtonElimina.Click
        Dim Result As String = ""
        Dim Refclasjavamensaje As New Classscrripjava
        Dim RefclasEliminadoc As New ClassEliminarDocListResult
        Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Try
            If Me.TreeViewseleccion_digitalizado.Nodes.Count = 0 Then
                Exit Sub
            End If
            Dim id_tarea As Long = HttpContext.Current.Session.Item("DG_ID_TAREA")
            If Not Me.TreeViewseleccion_digitalizado.SelectedNode Is Nothing Then
                If Me.HiddenPROMP.Value = "1" Then
                    Exit Sub
                End If
                Dim Texitem As String = Me.TreeViewseleccion_digitalizado.SelectedNode.Value
                Dim slplitlist() As String = Texitem.Split("|")
                Dim Estado_relacion As String = "YES"
                Result = Class_DAT_ADIC_TAR.Verifica_relacion_imagen_workflow(slplitlist(1),
                                                                              HttpContext.Current.Session.Item("DG_ID_RUTA"),
                                                                              id_tarea, Estado_relacion)
                If Result <> "YES" Then
                    Refclasjavamensaje.Showscripman(Result, Me.UpdatePanelBotones)
                    Exit Sub
                End If
                If Estado_relacion = "YES" Then
                    Refclasjavamensaje.Showscripman("El registro se encuentra relacionado a un flujo de trabajo como documento principal, imposible eliminar", Me.UpdatePanelBotones)
                    Exit Sub
                End If
                Dim id_documento As Integer = Val(slplitlist(1))
                Dim idex As Integer = Val(slplitlist(1))
                Dim gabinete As String = slplitlist(0)
                Result = RefclasEliminadoc.EliminarDocumentosGabinete(id_documento,
                                                                            idex,
                                                                            gabinete,
                                                                            0,
                                                                            0,
                                                                            0,
                                                                            -1,
                                                                            "ARCHIVOCORRESPONDENCIA")
                If Result <> "YES" Then
                    Refclasjavamensaje.Showscripman(Result, Me.UpdatePanelBotones)
                    Exit Sub
                Else
                    HttpContext.Current.Session.Item("DG_SELECION_TREE") = ""
                    Me.TreeViewseleccion_digitalizado.Nodes.Remove(Me.TreeViewseleccion_digitalizado.SelectedNode)
                    Session.Item("WF_TAGSELECCION_EMERGENTE") = ""
                    Me.IframeVisor_.Attributes("src") = "../workflow/WebFormFreeImageVisorEmerge.aspx"
                    Me.IframeVisor_.Attributes("Width") = "100%"
                    Me.IframeVisor_.Attributes("Heith") = "100%"
                    Me.Label_relacion_documentos.Text = "Documentos relacionados (" & Me.TreeViewseleccion_digitalizado.Nodes.Count & ")"
                    Me.UpdatePanelIframevisor.Update()
                    Me.UpdateDatos.Update()
                    Me.UpdatePanelseleccion_digitalizado.Update()
                End If
            End If
        Catch ex As Exception
            Refclasjavamensaje.Showscripman(ex.Message, Me.UpdatePanelBotones)
        End Try
    End Sub
End Class