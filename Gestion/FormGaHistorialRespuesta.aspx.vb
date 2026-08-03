Imports System.IO

Public Class FormGaHistorialRespuesta
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then
            DropDownListtiporespuesta.Items.Clear()
            DropDownListtiporespuesta.Items.Add("TODOS")
            DropDownListtiporespuesta.Items.Add("RESPUESTA CON RADICADO")
            DropDownListtiporespuesta.Items.Add("SOLO CONFIRMACION")
            DropDownListestadorespuesta.Items.Add("TODOS")
            DropDownListestadorespuesta.Items.Add("TRAMITES CON RESPUESTA")
            DropDownListestadorespuesta.Items.Add("PENDIENTES POR RESPONDER")
            DropDownListestadorespuesta.Items.Add("ENVIADOS AL SOLICITANTE O PETICIONARIO")
            DropDownListestadorespuesta.Items.Add("PENDIENTES POR ENVIAR AL SOLICITANTE O PETICIONARIO")
            Me.DropDownList_record.Items.Clear()
            For i As Integer = 1 To 5000
                Me.DropDownList_record.Items.Add(i)
            Next
            If Session.Item("GA_Manager_Produccion") = 0 Then
                CheckBoxtodosusuarios.Enabled = False
                CheckBoxtodosusuarios.Checked = True
                Session.Item("RA_RADICADO_CONSULTA_RESPUESTA_TODAS") = 0
            Else
                CheckBoxtodosusuarios.Checked = False
                CheckBoxtodosusuarios.Enabled = True
                Session.Item("RA_RADICADO_CONSULTA_RESPUESTA_TODAS") = 1
            End If
            Me.DropDownList_record.Text = "500"
            Dim Result As String = ""
            Dim id_organigrama As Integer = 0
            Dim Refclas_gestion_documental As New ClassGestionDocumental
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            Result = Reclas_registro_organigrama.Retorna_id_organigrama_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                        id_organigrama)
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            Else
                Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_consulta_areas(id_organigrama, _
                                                                                                        Me.DropDownListAREA_RESPONSABLE)
                If Result <> "YES" Then
                    Label_estado_transac.Text = Label_estado_transac.Text & Result
                End If
            End If
            Dim refclas_radicado As New ClassRadicador
            Result = refclas_radicado.Listar_Tipos_Documentales_historial(Me.DropDownListTRAMITE_DOCUMENTO)
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
            Dim refclas_radic As New ClassRadicador
            Result = refclas_radic.agregar_auto_complete(Me.TextBoxDESTINATARIO.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "DESTINATARIO")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
            Result = refclas_radic.agregar_auto_complete(Me.TextBoxRADICADO.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "RADICADO")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
            Result = refclas_radic.agregar_auto_complete(Me.TextBoxUSUARIO_RESPONSABLE.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "USUARIO_RESPONSABLE")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
            Result = refclas_radic.agregar_auto_complete(Me.TextBoxRADICADO_RESPUESTA.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "RADICADO_RESPUESTA")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
            Result = refclas_radic.agregar_auto_complete(Me.TextBoxASUNTO.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "ASUNTO")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
        End If
    End Sub
    Private Sub Button_consulta_like_Click(sender As Object, e As EventArgs) Handles Button_consulta_like.Click
        Dim refclas As New Classgestionrespuesta
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            Dim option_usuarios As Integer = 1
            If Me.CheckBoxtodosusuarios.Checked = True Then
                option_usuarios = 0
            Else
                option_usuarios = 1
            End If
            Dim top_limite As Integer = Val(Hidden_max_registro.Value)
            Hidden_resultado_consulta.Value = ""
            result = refclas.Consulta_historial_respuestas(Me.TextBoxID_RESPUESTA_RADICADO_INI.Text, _
                                                           Me.TextBoxID_RESPUESTA_RADICADO_FIN.Text, _
                                                           Me.TextBoxFECHA_REGISTRO_INI.Text, _
                                                           Me.TextBoxFECHA_REGISTRO_FIN.Text, _
                                                           Me.TextBoxFECHA_VENCE_INI.Text, _
                                                           Me.TextBoxFECHA_VENCE_FIN.Text, _
                                                           Me.TextBoxFECHA_RESPUETA_INI.Text, _
                                                           Me.TextBoxFECHA_RESPUETA_FIN.Text, _
                                                           Me.TextBoxFECHA_ENVIO_INI.Text, _
                                                           Me.TextBoxFECHA_ENVIO_FIN.Text, _
                                                           Me.TextBoxDESTINATARIO.Text, _
                                                           Me.TextBoxASUNTO.Text, _
                                                           Me.TextBoxRADICADO.Text, _
                                                           Me.DropDownListestadorespuesta.Text, _
                                                           Me.DropDownListtiporespuesta.Text, _
                                                           option_usuarios, _
                                                           Session.Item("GA_IDUSUARIOGESTION"), _
                                                           Me, _
                                                           Me.TextBoxRADICADO_RESPUESTA.Text, _
                                                           Me.TextBoxUSUARIO_RESPONSABLE.Text, _
                                                           Me.DropDownListTRAMITE_DOCUMENTO.Text, _
                                                           Me.DropDownListAREA_RESPONSABLE.Text, _
                                                           top_limite, _
                                                           2, _
                                                           Me.TextBox_buequeda_general.Text)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resultado_consulta.Value = "YES"
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                UpdatePanelabel_val_radicacion.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_validacion)
        End Try
    End Sub
    Private Sub Button_consulta_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_consulta_val_radicacion.Click
        Dim refclas As New Classgestionrespuesta
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            Dim option_usuarios As Integer = 1
            If Me.CheckBoxtodosusuarios.Checked = True Then
                option_usuarios = 0
            Else
                option_usuarios = 1
            End If
            Dim top_limite As Integer = Val(Hidden_max_registro.Value)
            Hidden_resultado_consulta.Value = ""
            result = refclas.Consulta_historial_respuestas(Me.TextBoxID_RESPUESTA_RADICADO_INI.Text, _
                                                           Me.TextBoxID_RESPUESTA_RADICADO_FIN.Text, _
                                                           Me.TextBoxFECHA_REGISTRO_INI.Text, _
                                                           Me.TextBoxFECHA_REGISTRO_FIN.Text, _
                                                           Me.TextBoxFECHA_VENCE_INI.Text, _
                                                           Me.TextBoxFECHA_VENCE_FIN.Text, _
                                                           Me.TextBoxFECHA_RESPUETA_INI.Text, _
                                                           Me.TextBoxFECHA_RESPUETA_FIN.Text, _
                                                           Me.TextBoxFECHA_ENVIO_INI.Text, _
                                                           Me.TextBoxFECHA_ENVIO_FIN.Text, _
                                                           Me.TextBoxDESTINATARIO.Text, _
                                                           Me.TextBoxASUNTO.Text, _
                                                           Me.TextBoxRADICADO.Text, _
                                                           Me.DropDownListestadorespuesta.Text, _
                                                           Me.DropDownListtiporespuesta.Text, _
                                                           option_usuarios, _
                                                           Session.Item("GA_IDUSUARIOGESTION"), _
                                                           Me, _
                                                           Me.TextBoxRADICADO_RESPUESTA.Text, _
                                                           Me.TextBoxUSUARIO_RESPONSABLE.Text, _
                                                           Me.DropDownListTRAMITE_DOCUMENTO.Text, _
                                                           Me.DropDownListAREA_RESPONSABLE.Text, _
                                                           top_limite, _
                                                           1, _
                                                           "")
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resultado_consulta.Value = "YES"
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                UpdatePanelabel_val_radicacion.Update()
            End If

        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_validacion)
        End Try
    End Sub

    Private Sub Button_lipiar_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_lipiar_val_radicacion.Click
        Dim refclas As New Classgestionrespuesta
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            Dim option_usuarios As Integer = 1
            Hidden_resultado_consulta.Value = ""
            result = refclas.Limpiar_campos_consulta_respuestas(Me.TextBoxFECHA_REGISTRO_INI, _
                                                                Me.TextBoxFECHA_REGISTRO_FIN, _
                                                                Me.TextBoxFECHA_VENCE_INI, _
                                                                Me.TextBoxFECHA_VENCE_FIN, _
                                                                Me.TextBoxFECHA_RESPUETA_INI, _
                                                                Me.TextBoxFECHA_RESPUETA_FIN, _
                                                                Me.TextBoxFECHA_ENVIO_INI, _
                                                                Me.TextBoxFECHA_ENVIO_FIN, _
                                                                Me.TextBoxDESTINATARIO, _
                                                                Me.TextBoxASUNTO, _
                                                                Me.TextBoxRADICADO, _
                                                                Me.DropDownListestadorespuesta, _
                                                                Me.DropDownListtiporespuesta, _
                                                                Me.Page, _
                                                                Me.TextBoxRADICADO_RESPUESTA, _
                                                                Me.TextBoxUSUARIO_RESPONSABLE, _
                                                                Me.DropDownListTRAMITE_DOCUMENTO, _
                                                                Me.DropDownListAREA_RESPONSABLE)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub

            End If

        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_validacion)
        End Try
    End Sub

    Private Sub UpdatePanelContenido_val_radicacion_Load(sender As Object, e As EventArgs) Handles UpdatePanelContenido_val_radicacion.Load
        Try
            Dim refclas_radic As New ClassRadicador
            Dim Result = refclas_radic.agregar_auto_complete(Me.TextBoxDESTINATARIO.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "DESTINATARIO")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
            Result = refclas_radic.agregar_auto_complete(Me.TextBoxRADICADO.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "RADICADO")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
            Result = refclas_radic.agregar_auto_complete(Me.TextBoxUSUARIO_RESPONSABLE.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "USUARIO_RESPONSABLE")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
            Result = refclas_radic.agregar_auto_complete(Me.TextBoxRADICADO_RESPUESTA.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "RADICADO_RESPUESTA")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
            Result = refclas_radic.agregar_auto_complete(Me.TextBoxASUNTO.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_respuesta_radicado", "ASUNTO")
            If Result <> "YES" Then
                Label_estado_transac.Text = Label_estado_transac.Text & Result
            End If
        Catch ex As Exception

        End Try
       
    End Sub
    Private Sub Button_detalle_radicado_Click(sender As Object, e As EventArgs) Handles Button_detalle_radicado.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refcals As New Classgestionrespuesta
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, stru, 1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("PU_TRAZABILIDAD") = stru.RADICADO
            Me.Iframe_detalle_respuesta_.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
            Me.UpdatePanel_detalle_respuesta.Update()
            Me.ModalPopupExtender_detalle_respuesta.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub
    Private Sub Button_Trazabilidad_Click(sender As Object, e As EventArgs) Handles Button_Trazabilidad.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refcals As New Classgestionrespuesta
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, stru, 1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Label12.Text = "Trazabilidad radicado"
            Session.Item("PU_TRAZABILIDAD") = stru.RADICADO
            Me.Iframe_trazabilidad_.Attributes("SRC") = "../workflow/WebFormTrazabilidadWorkflow.aspx"
            Me.UpdatePanel_trazabilidad.Update()
            Me.ModalPopupExtender_trazabilidad.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub
    Protected Sub Button_Log_respuesta_Click(sender As Object, e As EventArgs) Handles Button_Log_respuesta.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("PU_TRAZABILIDAD") = Me.hdnEmailID_VAL.Value
            Me.Iframe_transacciones_.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
            Me.UpdatePanel_transacciones.Update()
            Me.ModalPopupExtender_transacciones.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub
    Private Sub Button_descarga_Click(sender As Object, e As EventArgs) Handles Button_descarga.Click
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try

            Dim Result As String = ""
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, estru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_Manager_Produccion") = 0 Then
                If estru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                    scrijava.Showscripman_menu("El usuario no es el responsable de la respuesta, no puede descargar la plantilla", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            If estru.ID_IMAGEN = 0 Then
                scrijava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_imagen As Integer = -1
            Dim gabinete_imagen As String = ""
            Dim refclas_gestion As New Classgestionrespuesta
            Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(Me.hdnEmailID_VAL.Value, gabinete_imagen, id_imagen)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclas_visualiza As New ClassVisualisaDocumento
            Dim id_respuesta As Integer = Me.hdnEmailID_VAL.Value
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim matri_documento() As String = Nothing
            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen, "IMP03GESTIONTMP", matri_documento)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
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
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        Dim clasjava As New Classscrripjava
        Try
            'Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            If Me.hdnEmailID_VAL.Value = "" Or Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Seleccione el registro", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refcals As New Classgestionrespuesta
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, stru, 1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value & "|" & stru.system_plantilla_radicado_id_plantilla
            'Me.Labelpendient.Text = "Tareas pendiente"
            Me.Iframe_imagen_respuesta_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_imagen_respuesta.Update()
            Me.ModalPopupExtender_imagen_respuesta.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Private Sub DropDownList_record_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_record.SelectedIndexChanged
        Me.Hidden_max_registro.Value = Me.DropDownList_record.Text
    End Sub
    Private Sub Button_notificar_correo_Click(sender As Object, e As EventArgs) Handles Button_notificar_correo.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim Refclasgestion As New Classgestionrespuesta
        Dim Clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                Clasjava.Showscripman("Debe seleccionar el registro", Me.UpdatePanel_boton_notifica_correo)
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, _
                                                                                        estru)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, Me.UpdatePanel_boton_notifica_correo)
                Exit Sub
            End If
            If estru.FECHA_RESPUETA = "" Then
                Clasjava.Showscripman("El tramite actual no tiene  una respuesta", Me.UpdatePanel_boton_notifica_correo)
                Exit Sub
            End If
            If estru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                Clasjava.Showscripman("El usuario de gestión no es el autorizado para gestionar la respuesta", Me.UpdatePanel_boton_notifica_correo)
                Exit Sub
            End If
            If estru.TIPO_RESPUESTA_ELAB_USUARIO = -1 Then
                Clasjava.Showscripman("El tramite actual no tiene tipo de respuesta asociada", Me.UpdatePanel_boton_notifica_correo)
                Exit Sub
            End If
            Dim estado_anexo As Integer = 1
            If Me.CheckBox_anexa_anexos.Checked = True Then
                estado_anexo = 1
            Else
                estado_anexo = 0
            End If
            If estru.TIPO_RESPUESTA_ELAB_USUARIO = "1" Then
                Result = Refclasgestion.Confirma_respuesta_al_correo_con_radicado(Me.hdnEmailID_VAL.Value, _
                                                                                  Me.Page, _
                                                                                  Me.TextBox_correo_electronico.Text, _
                                                                                  estado_anexo)
                If Result <> "YES" Then
                    Clasjava.Showscripman(Result, Me.UpdatePanel_notifica_correo)
                    Exit Sub
                End If
            Else
                Result = Refclasgestion.Confirma_respuesta_al_correo_con_sin_radicado(Me.hdnEmailID_VAL.Value, _
                                                                                      Me.Page, _
                                                                                      Me.TextBox_correo_electronico.Text, _
                                                                                      estado_anexo)
                If Result <> "YES" Then
                    Clasjava.Showscripman(Result, Me.UpdatePanel_boton_notifica_correo)
                    Exit Sub
                End If
            End If

        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_notifica_correo)
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
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro", Me.UpdatePanel_boton_Panel_reversa_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Me.hdnEmailID_VAL.Value, _
                                                                                id_imagen_plantilla, _
                                                                                radicado_respuesta, _
                                                                                fecha_respuesta, _
                                                                                id_imagen_respuesta, _
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_Panel_reversa_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If fecha_respuesta = 0 And radicado_respuesta = 0 Then
                clasjava.Showscripman_menu("La respuesta a reversar aun no tiene respuesta ", Me.UpdatePanel_boton_Panel_reversa_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_usuario_autoriza As Integer = -1
            Result = Refclas.Valida_usuario_administrador_general(Me.TextBox_login_usuario_val.Text, _
                                                                  Me.TextBox_pasw_usuario_val.Text, _
                                                                  id_usuario_autoriza, _
                                                                  "reversa_respuesta")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_Panel_reversa_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, _
                                                                                        stru, _
                                                                                        1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_Panel_reversa_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Reversa_respuesta_radicado(stru, _
                                                        Me.TextBox_login_usuario_val.Text, _
                                                        id_usuario_autoriza)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_Panel_reversa_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_reversa_respuesta.Hide()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_Panel_reversa_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_actualizar_peticionario_Click(sender As Object, e As EventArgs) Handles Button_actualizar_peticionario.Click
        Dim refclasconsulta As New ClassRaConsultaRadicados
        Dim scripjava As New Classscrripjava
        Dim refclas As New Classgestionrespuesta
        Try
            Dim Result As String = ""
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar el registro", Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBox_dext_externo.Text = "" Then
                scripjava.Showscripman_menu("Debe informar el nombre del peticionario o destinatario externo", Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBox_login_usuario_val_externo.Text = "" Then
                scripjava.Showscripman_menu("Debe informar el nombre del usuario de autorización", Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBox_pasw_usuario_val_externo.Text = "" Then
                scripjava.Showscripman_menu("Debe informar la contraseña del usuario de autorización", Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_user_autorizacion As Integer = 0
            Result = refclas.Valida_usuario_administrador_general(Me.TextBox_login_usuario_val_externo.Text, Me.TextBox_pasw_usuario_val_externo.Text, id_user_autorizacion, "actualiza_peticionario")
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = refclasconsulta.Reasigna_destinatario_externo(Me.hdnEmailID_VAL.Value, id_user_autorizacion.ToString & "-" & Me.TextBox_login_usuario_val_externo.Text, Me.TextBox_dext_externo.Text, Me.Hidden_remitente_destinatario.Value)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            ModalPopupExtender_edition_asigna_dest_externo.Hide()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_dest_externo, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Protected Sub Button_guardar_Click(sender As Object, e As EventArgs) Handles Button_guardar.Click
        'UpdatePanel_botones_registro
        Dim Refclas As New Classgestionrespuesta
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro", Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim Result As String = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, stru, 1)
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
            Result = Refclas.Actualiza_nota_respuesta(Me.hdnEmailID_VAL.Value, Me.TextBox_NOTA_RESPUESTA.Text)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub
    Private Sub Button_imprimir_Click(sender As Object, e As EventArgs) Handles Button_imprimir.Click
        Dim Refclas As New ClassRaEnvioCorrespondencia
        Dim scrijava As New Classscrripjava
        Try

            Dim Result As String = ""
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, estru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_Manager_Produccion") = 0 Then
                If estru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                    scrijava.Showscripman_menu("El usuario no es el responsable de la respuesta, no puede imprimir", Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim id_imagen As Integer = -1
            Dim gabinete_imagen As String = ""
            Dim refclas_gestion As New Classgestionrespuesta
            Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(Me.hdnEmailID_VAL.Value, gabinete_imagen, id_imagen)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclas_visualiza As New ClassVisualisaDocumento
            Dim id_respuesta As Integer = Me.hdnEmailID_VAL.Value
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            Dim matri_documento() As String = Nothing
            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen, "IMP03GESTIONTMP", matri_documento)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
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
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_me_active_men_dive_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.Hidden_menu_var_event_dive.Value = "" Then
                Exit Sub
            End If
            Dim Valselect As String = Me.Hidden_menu_var_event_dive.Value
            Dim Result As String = ""
            If Valselect <> "E-R-H-R" Then
                If Me.hdnEmailID_VAL.Value = "-1" Then
                    clasjava.Showscripman_menu("Debe seleccionar el registro", _
                                               Me.UpdatePanel_menu_var_event, _
                                               "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
           
            Dim estru As stru_envio = Nothing
            Dim refclas_resp As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            If Valselect <> "E-R-H-R" Then
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, _
                                                                                       estru)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, _
                                               Me.UpdatePanel_menu_var_event, _
                                               "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
           

            If Valselect = "R-R-D" Then
                Me.TextBox_login_usuario_val.Text = ""
                Me.TextBox_pasw_usuario_val.Text = ""
                Me.UpdatePanel_contenido_radica_documento.Update()
                Me.ModalPopupExtender_edition_reversa_respuesta.Show()
            End If
            If Valselect = "R-P-I" Then

                Me.TextBox_dext_externo.Text = estru.DESTINATARIO
                UpdatePanel_dest_externo.Update()
                ModalPopupExtender_edition_asigna_dest_externo.Show()

            End If
            If Valselect = "D-D-R-R" Then
                Session.Item("PU_TRAZABILIDAD") = estru.RADICADO
                Me.Iframe_detalle_respuesta_.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
                Me.UpdatePanel_detalle_respuesta.Update()
                Me.ModalPopupExtender_detalle_respuesta.Show()

            End If
            If Valselect = "E-N-R" Then
                Session.Item("PU_TRAZABILIDAD") = estru.RADICADO
                Me.TextBox_NOTA_RESPUESTA.Text = estru.NOTA_RESPUESTA
                Me.UpdatePanel_nota_respuesta.Update()
                Me.ModalPopupExtender_nota_respuesta.Show()
            End If
            If Valselect = "N-R-C" Then
                Dim correo_electronico As String = ""
                Dim refclasradicado As New ClassRadicador
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, _
                                                                                            estru)
                Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(estru.codigo_dest_externo, _
                                                                                           correo_electronico, _
                                                                                           estru.system_plantilla_radicado_id_plantilla)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                TextBox_correo_electronico.Text = correo_electronico
                Me.UpdatePanel_notifica_correo.Update()
                ModalPopupExtender_edition_notifica_correo_respuesta.Show()
            End If
            If Valselect = "D-V-D-T" Then
                Session.Item("PU_TRAZABILIDAD") = Me.hdnEmailID_VAL.Value
                Me.Iframe_transacciones_.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
                Me.UpdatePanel_transacciones.Update()
                Me.ModalPopupExtender_transacciones.Show()
            End If
            If Valselect = "D-E-D-R" Then
                Dim refcals As New Classgestionrespuesta
                Dim stru As stru_envio = Nothing
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, _
                                                                                            stru, _
                                                                                            1)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.Label12.Text = "Trazabilidad radicado"
                Session.Item("PU_TRAZABILIDAD") = stru.RADICADO
                Me.Iframe_trazabilidad_.Attributes("SRC") = "../workflow/WebFormTrazabilidadWorkflow.aspx"
                Me.UpdatePanel_trazabilidad.Update()
                Me.ModalPopupExtender_trazabilidad.Show()
            End If
            If Valselect = "V-I-H-R" Then
                Dim refcals As New Classgestionrespuesta
                Dim stru As stru_envio = Nothing
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, _
                                                                                            stru, _
                                                                                            1)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value & "|" & stru.system_plantilla_radicado_id_plantilla
                Me.Iframe_imagen_respuesta_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
                Me.UpdatePanel_imagen_respuesta.Update()
                Me.ModalPopupExtender_imagen_respuesta.Show()
            End If
            If Valselect = "D-I-H-R" Then
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.hdnEmailID_VAL.Value, estru)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Session.Item("GA_Manager_Produccion") = 0 Then
                    If estru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                        clasjava.Showscripman_menu("El usuario no es el responsable de la respuesta, no puede descargar la plantilla", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                End If
                If estru.ID_IMAGEN = 0 Then
                    clasjava.Showscripman_menu("El tramite actual no tiene asignada una respuesta", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim id_imagen As Integer = -1
                Dim gabinete_imagen As String = ""
                Dim refclas_gestion As New Classgestionrespuesta
                Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(Me.hdnEmailID_VAL.Value, gabinete_imagen, id_imagen)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim refclas_visualiza As New ClassVisualisaDocumento
                Dim id_respuesta As Integer = Me.hdnEmailID_VAL.Value
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim matri_documento() As String = Nothing
                Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen, "IMP03GESTIONTMP", matri_documento)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
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
            End If
            If Valselect = "E-R-H-R" Then
                If Me.Hidden_colum_header.Value = "" Then
                    Hidden_ruta_archivo.Value = ""
                    clasjava.Showscripman("No hay resultados para exportar", Me.UpdatePanel_menu_var_event)
                    Exit Sub
                End If
                Dim value As Integer = CInt(Int((100000 * Rnd()) + 1))
                Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + "/reportes/" + "PUBLICO" + "/")
                If Directory.Exists(ruta_create) = False Then
                    Directory.CreateDirectory(ruta_create)
                End If
                Dim Refclasreposte As New ClassReportesRadicado
                Dim ruta_archivo As String = ruta_create + HttpContext.Current.User.Identity.Name + value.ToString + "test.xls"
                Result = Refclasreposte.genera_xls(Me.GridView_val_radicacion, ".xls", _
                                                   ruta_archivo, Hidden_colum_header.Value, _
                                                   "HISTORIAL DE RESPUESTAS", Session.Item("GA_LOGINUSUARIOGESTION"))
                If Result <> "YES" Then
                    Hidden_ruta_archivo.Value = ""
                    clasjava.Showscripman(Result, Me.UpdatePanel_menu_var_event)
                    Exit Sub
                Else
                    If File.Exists(ruta_archivo) = True Then
                        Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + "/reportes/" + "PUBLICO" + "/" & HttpContext.Current.User.Identity.Name & value.ToString + "test.xls"
                        ifmExcel_.Attributes.Add("src", "../Gestion/WebFormDescargaRadicadogd.aspx")
                        updatapanel_iframe.Update()
                    End If
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_menu_var_event)
        End Try
    End Sub
    
    Protected Sub Button_Exportar_Radicados_Click(sender As Object, e As EventArgs) Handles Button_Exportar_Radicados.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            If Me.Hidden_colum_header.Value = "" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman("No hay resultados para exportar", Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((100000 * Rnd()) + 1))
            Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + "/reportes/" + "PUBLICO" + "/")
            If Directory.Exists(ruta_create) = False Then
                Directory.CreateDirectory(ruta_create)
            End If
            Dim ruta_archivo As String = ruta_create + HttpContext.Current.User.Identity.Name + value.ToString + "test.xls"
            Result = Refclasreposte.genera_xls(Me.GridView_val_radicacion,
                                               ".xls",
                                               ruta_archivo,
                                               Hidden_colum_header.Value,
                                               "HISTORIAL DE RESPUESTAS",
                                               Session.Item("GA_LOGINUSUARIOGESTION"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + "/reportes/" + "PUBLICO" + "/" & HttpContext.Current.User.Identity.Name & value.ToString + "test.xls"
                    ifmExcel_.Attributes.Add("src", "../Gestion/WebFormDescargaRadicadogd.aspx")
                    updatapanel_iframe.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub

    Private Sub GridView_val_radicacion_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_val_radicacion.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub

   
End Class