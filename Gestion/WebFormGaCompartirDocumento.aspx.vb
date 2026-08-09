Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports GestionDocumental_Docuarchi.net.conect
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.Web.Http
Imports System.IO

Public Class WebFormGaCompartirDocumento
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim clasjava As New Classscrripjava
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            Dim Result As String = ""
            If IsPostBack = False Then
                Me.DropDownList_prioridad_solicitud.Items.Add("Normal")
                Me.DropDownList_prioridad_solicitud.Items.Add("Urgente")
                Me.DropDownList_tipo_documento_compartir.Items.Add("Informativo")
                Me.DropDownList_tipo_documento_compartir.Items.Add("Para aprobación")
                Me.DropDownList_tipo_documento_compartir.Items.Add("Para colaboración")
                Dim refclas As New ClassGaCompartirDocumento
                Result = refclas.Interface_dinamica_documentos_a_compartir(Me.Page)
                If Result <> "YES" Then
                    Me.Label_resultado.Text = Result
                End If
                Result = refclas.Perfila_interface_compartir_documento(Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), _
                                                                       Session.Item("GA_STRU_DOCUMENTO_TIPO_COMPARTIDO"), _
                                                                       Me.Page)
                If Result <> "YES" Then
                    Me.Label_resultado.Text = Me.Label_resultado.Text & "|" & Result
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button_activa_visor_documento_Click(sender As Object, e As EventArgs) Handles Button_activa_visor_documento.Click
        Dim scrijava As New Classscrripjava
        Dim split() As String = Me.Hidden_value_documento.Value.Split(",")
        Dim Result As String = ""
        Try
            If split.Length > 3 Then
                Dim extension As String = split(3).Replace(".", "")
                If UCase(extension) = "PDF" Or UCase(extension) = "JPG" Or UCase(extension) = "BMP" Or UCase(extension) = "TIF" Or UCase(extension) = "TIFF" Then
                    Session.Item("CC_SESIONITERCAMBIOVISOR") = split(1) & "|" & split(2)
                    If HttpContext.Current.Session("VALIDA_VISOR_EXPRES") = 1 Then
                        HttpContext.Current.Session.Item("TIPO_VISOR_PDF") = "PDF-JS"
                    Else
                        HttpContext.Current.Session.Item("TIPO_VISOR_PDF") = ""
                    End If
                    Iframe_visor_externo_clasficacion_.Attributes.Add("src", "../gestion/WebFormGaVisorClasificacion.aspx")
                    UpdatePanel_visor_externo.Update()
                    ModalPopupExtender_visor_externo.Show()
                Else
                    Dim refclas_visualiza As New ClassVisualisaDocumento
                    Dim matri_documento() As String = Nothing
                    Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(split(1), split(2), matri_documento)
                    If Result <> "YES" Then
                        scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
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
                    ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                    updatapanel_iframe.Update()
                End If
            Else
                scrijava.Showscripman_menu("La estrucutra no esta completa debe tener por lo menos cuatro niveles " & Me.Hidden_value_documento.Value, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub UpdatePanel_seleccion_documento_Load(sender As Object, e As EventArgs) Handles UpdatePanel_seleccion_documento.Load
        
    End Sub

    

    Protected Sub Button_compartir_documento_Click(sender As Object, e As EventArgs) Handles Button_compartir_documento.Click
        Dim scrijava As New Classscrripjava
        Try
            'Exit Sub
            'Dim Result As String = ""
            'Dim Refclas As New ClassGaCompartirDocumento
            'Dim cheval As String = Hidden_iten_ckek.Value
            'Hidden_resultado_compartir.Value = ""
            'If cheval = "" Then
            '    scrijava.Showscripman_menu("Por favor seleccione los documentos a compartir", Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            'Dim split_sel() As String = cheval.Split("|")
            'Dim stru_selcion() As stru_documentos_compartidos = Nothing
            'For i As Integer = 0 To split_sel.Length - 1
            '    ReDim Preserve stru_selcion(i)
            '    Dim spli_separador() As String = split_sel(i).Split("_")
            '    stru_selcion(i).id_imagen = spli_separador(2)
            '    stru_selcion(i).nombre_gabinete = spli_separador(3)
            'Next
            'Dim Resultado_correo As String = ""
            'Dim ref_value As String = Me.Hidden_text_user.Value
            'ref_value = ref_value.Replace("||", ">")
            'ref_value = ref_value.Replace("|", "<")
            'Result = Refclas.Registra_solicitud_general_documento_compartido_usuario(ref_value, _
            '                                                                         Me.TextBox_asunto_documento.Text, _
            '                                                                         Me.TextBox_nota_documento.Text, _
            '                                                                         Me.DropDownList_prioridad_solicitud.Text, _
            '                                                                         Me.DropDownList_tipo_documento_compartir.Text, _
            '                                                                         Me.TextBox_fecha_limite_solicitud.Text, _
            '                                                                         Session.Item("GA_STRU_DOCUMENTO_RADICADO"), _
            '                                                                         Session.Item("GA_IDUSUARIOGESTION"), _
            '                                                                         stru_selcion, _
            '                                                                         Resultado_correo)
            'If Result <> "YES" Then
            '    Hidden_resultado_compartir.Value = ""
            '    scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'Else
            '    Hidden_resultado_compartir.Value = "YES"
            '    If Resultado_correo <> "" Then
            '        scrijava.Showscripman_menu(Resultado_correo, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If
            'End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    
    Protected Sub ImageButton__adjuntar_archivo_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton__adjuntar_archivo.Click
        'Me.Buttonaceptar.Enabled = True
        Session.Item("WF_TIPO_ADJUNTA") = "COLABORACION"
        AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
        'AjaxFileUpload_dowload.AllowedFileTypes = "tif,jpg,tiff,bmp,pdf"
        Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
        AjaxFileUpload_dowload.MaximumNumberOfFiles = 4
        UpdatePanel_descarga.Update()
        Me.ModalPopupExtender_sube_documento_adjunto.Show()
    End Sub
    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try

            If Session.Item("WF_TIPO_ADJUNTA") = "COLABORACION" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & e.FileName
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
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                CLAS.Showscripman(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga)
                Exit Sub
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "COLABORACION" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                Dim Refclascompartir As New ClassGaCompartirDocumento
                Result = Refclascompartir.Registra_documento_colaboracion_documento_compartido("IMP03GESTIONTMP", Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), _
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
    Private Sub Button_anexar_aportes_colaboracion_Click(sender As Object, e As EventArgs) Handles Button_anexar_aportes_colaboracion.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaCompartirDocumento
            Dim Result As String = ""
            Dim id_usuario_compartido As Integer = 0
            Result = Refclas.Retorna_id_usuario_documento_compartido(Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), id_usuario_compartido, Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Lista_documentos_de_colaboracion_usuario_colaboracion_drow_list(Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), id_usuario_compartido, _
                                                                                             Me.DropDownList_docuentos_colaboracion, Me.UpdatePanel_adjunto_documento_colaboracion)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_contenido_nota_id_documento_compartido(Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), id_usuario_compartido, Me.TextBox_nota_colaboracion.Text, Hidden_id_nota_document.Value)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                UpdatePanel_guarda_actualiza_nota.Update()
                UpdatePanel_colaboracion_documento_compartido.Update()
            End If
            ModalPopupExtender_edition_colaboracion_documento_compartido.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub ImageButton_eliminar_archivo_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_eliminar_archivo.Click
        Dim scrijava As New Classscrripjava
        Try
            If Me.DropDownList_docuentos_colaboracion.Text = "" Then Exit Sub
            Dim Refclas As New ClassGaCompartirDocumento
            Dim Result As String = ""
            Result = Refclas.Elimina_documento_colaboracion_documento_compartido_dorw_list(Me.DropDownList_docuentos_colaboracion.Text, Me.DropDownList_docuentos_colaboracion, _
                                                                                         Me.UpdatePanel_adjunto_documento_colaboracion)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_adjunto_documento_colaboracion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_adjunto_documento_colaboracion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_guarda_actualiza_nota_Click(sender As Object, e As EventArgs) Handles Button_guarda_actualiza_nota.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaCompartirDocumento
            Dim Result As String = ""
            Dim id_usuario_compartido As Integer = 0
            Result = Refclas.Retorna_id_usuario_documento_compartido(Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), id_usuario_compartido, Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_guarda_actualiza_nota, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Hidden_id_nota_document.Value = 0 Then
                If Me.TextBox_nota_colaboracion.Text = "" Then
                    scrijava.Showscripman_menu("Por favor digite la nota", Me.UpdatePanel_guarda_actualiza_nota, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Registrar_nota_documento_compartido(Me.TextBox_nota_colaboracion.Text, Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), id_usuario_compartido, Hidden_id_nota_document.Value)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_guarda_actualiza_nota, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                If Me.TextBox_nota_colaboracion.Text = "" Then
                    scrijava.Showscripman_menu("Por favor digite la nota", Me.UpdatePanel_guarda_actualiza_nota, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Actualiza_nota_documento_compartido(Hidden_id_nota_document.Value, Me.TextBox_nota_colaboracion.Text)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_guarda_actualiza_nota, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Button_cerrar_colaboracion_validacion_Click(sender As Object, e As EventArgs) Handles Button_cerrar_colaboracion_validacion.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaCompartirDocumento
            Dim Result As String = ""
            Dim id_usuario_compartido As Integer = 0
            Dim id_estado As Integer = 0
            Result = Refclas.Retorna_id_usuario_documento_compartido(Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), id_usuario_compartido, Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_confirmar_colaboracion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Hidden_id_nota_document.Value <> 0 Or Me.DropDownList_docuentos_colaboracion.Text <> "" Then
                Result = Refclas.Retorna_estado_confirmacion_documento_colaboracion_usuario(Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), id_usuario_compartido, id_estado)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_confirmar_colaboracion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    If id_estado = 0 Then
                        scrijava.Showscripman_menu("Antes de cerrar la ventana, debe confirmar la colaboración", Me.UpdatePanel_confirmar_colaboracion, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                End If
            End If
            ModalPopupExtender_edition_colaboracion_documento_compartido.Hide()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Button_confirmar_colaboracion_Click(sender As Object, e As EventArgs) Handles Button_confirmar_colaboracion.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaCompartirDocumento
            Dim Resultado_correo As String = ""
            Result = Refclas.Confirma_colaboracion_solicitud_colaboracion(Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO"), Hidden_id_nota_document.Value, _
                                                                          Me.DropDownList_docuentos_colaboracion, Me.TextBox_nota_colaboracion.Text, Resultado_correo)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_confirmar_colaboracion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Resultado_correo <> "" Then
                scrijava.Showscripman_menu(Resultado_correo, Me.UpdatePanel_confirmar_colaboracion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_colaboracion_documento_compartido.Hide()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_confirmar_colaboracion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    
   
    
   
End Class