Imports System.IO

Public Class WebFormPqrsGestion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Result As String = ""
        Dim Refclas As New ClassPqrs
        If Me.Page.IsPostBack = False Then
            'HttpContext.Current.Session.Item("EMPRESA_GESTION")
            Dim ref_class_admon As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Dim id_organigrama As Integer = 0
            Label_tipo_pqrsd.Text = Session.Item("PQRS_TIPO_PQRS")
            Result = ref_class_admon.Retorna_Id_Emprea(HttpContext.Current.Session.Item("EMPRESA_GESTION"), _
                                                       id_empresa)
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            Else
                Dim Reclas_registro_organigrama As New Class_registro_organigrama
                Result = Reclas_registro_organigrama.Retorna_Id_Organigrama_activo_empresa(id_empresa, _
                                                                                           id_organigrama)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                    Exit Sub
                End If
                HttpContext.Current.Session.Item("GA_IDEMPRESA") = id_empresa
                Result = Refclas.Listar_Tipos_Documentales_pqrs(Me.DropDownList_tipo_tramite, _
                                                                Session.Item("PQRS_CODIGO_PLANTILLA_RADICADO"))
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                    Exit Sub
                End If
                Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                Result = Class_areas_depart_radicacion.Lista_areas_usuario_gestion_permitido_para_gestionar_pqr(id_organigrama,
                                                                                                                DropDownList_area_dependencia)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                    Exit Sub
                End If
                Dim nombre As String = ""
                Dim nit As String = ""
                Dim anualidad As String = ""
                Result = Refclas.Lista_campos_nit_nombre_usuario_pqr(Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"),
                                                                     Session.Item("PQRS_ID_USUARIO_PQRS"),
                                                                     nombre,
                                                                     nit,
                                                                     anualidad)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                    Exit Sub
                Else
                    Label_login_uusario.Text = Trim(UCase(nombre))
                    Label_identifcacion.Text = Trim(nit)
                    Label_anualidad.Text = Trim(anualidad)
                End If
            End If

        End If
    End Sub
    Private Sub Button_sube_documento_adjunto_respuesta_Click(sender As Object, e As EventArgs) Handles Button_sube_documento_adjunto_respuesta.Click
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try
            If Session.Item("WF_ERROR_RESPUESTA") <> "anexo" And Session.Item("WF_ERROR_RESPUESTA") <> "adjunto" Then
                scrijava.Showscripman_menu(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_anexos_respuesta, "ModalPopupExtender_mensaje_personalizado")
                ModalPopupExtender_edition_sube_documento_respuesta.Hide()
                Exit Sub
            End If
            Dim ruta_virtual As String = "../Publico/Temp_Image/" & "/adjuntos_pqr/" & Session.Item("PQRS_ID_USUARIO_PQRS") & "/"
            Dim matri_documentos() As String = Nothing
            Result = Refclas.Lista_nombre_documentos_anexos_respuesta_droplist(Server.MapPath(ruta_virtual), _
                                                                               DropDownList_anexos_respuesta)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanel_anexos_respuesta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.UpdatePanel_anexos_respuesta.Update()
            Me.ModalPopupExtender_edition_sube_documento_respuesta.Hide()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
            
            If Session.Item("WF_ERROR_RESPUESTA") = "anexo" Then
                'Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_fisica_uno As String = Server.MapPath("../Publico/Temp_Image/" & "/adjuntos_pqr/")
                If Directory.Exists(ruta_fisica_uno) = False Then
                    Directory.CreateDirectory(ruta_fisica_uno)
                End If
                Dim ruta_virtual As String = "../Publico/Temp_Image/" & "/adjuntos_pqr/" & Session.Item("PQRS_ID_USUARIO_PQRS") & "/"
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                If Directory.Exists(ruta_fisica) = False Then
                    Directory.CreateDirectory(ruta_fisica)
                End If
                Dim fil_name As String = Session.Item("PQRS_ID_USUARIO_PQRS") & ".pdf"
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & fil_name
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                sender.SaveAs(ruta_fisica & fil_name)
                Dim anti As New AntiVirus.Scanner
                Dim rest = anti.ScanAndClean(ruta_fisica & fil_name)
                If rest = 1 Then
                    Session.Item("WF_ERROR_RESPUESTA") = "El archivo tiene virus " & ruta_fisica & fil_name
                    Kill(ruta_fisica & fil_name)
                End If  
            End If
        Catch ex As Exception
            Session.Item("WF_ERROR_RESPUESTA") = ex.Message

        End Try
    End Sub
    Private Sub Button_anexo_cargar_Click(sender As Object, e As EventArgs) Handles Button_anexo_cargar.Click
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try

            Me.ModalPopupExtender_edition_sube_anexo_respuesta.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub


    Protected Sub Button_anexo_eliminar_Click(sender As Object, e As EventArgs) Handles Button_anexo_eliminar.Click
        Dim Result As String = ""
        Dim Refclas As New ClassPqrs
        Dim scrijava As New Classscrripjava
        Try
            Dim ruta_virtual_anexo As String = "../Temp_Image/" & "/adjuntos_pqr/" & Session.Item("PQRS_ID_USUARIO_PQRS") & "/"
            Dim ruta_fisica_anexo As String = Server.MapPath(ruta_virtual_anexo)
            Dim fil_name As String = Session.Item("PQRS_ID_USUARIO_PQRS") & ".pdf"
            Dim ruta_documento_anexo As String = ruta_fisica_anexo & fil_name
            If DropDownList_anexos_respuesta.Text <> "" Then
                If File.Exists(ruta_documento_anexo) = True Then
                    Kill(ruta_documento_anexo)
                    DropDownList_anexos_respuesta.Items.Remove(DropDownList_anexos_respuesta.Text)
                End If
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_anexos_respuesta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_consulta_pqrs_registrados_Click(sender As Object, e As EventArgs) Handles Button_consulta_pqrs_registrados.Click
        Dim Result As String = ""
        Dim Refclas As New ClassPqrs
        Dim scrijava As New Classscrripjava
        Try
            Session.Item("SortExpression_publico") = "FECHA_REGISTRO"
            Session.Item("SortDirection_publico") = "DESC"
            Session.Item("GA_TIPO_CONSULTA_PUBLICO") = "1"
            Session.Item("GA_DATO_CONSULTA_PUBLICO") = ""
            Result = Refclas.Consulta_pqrs_registrados_usuario(Session.Item("PQRS_ID_USUARIO_PQRS"), _
                                                               Me.data_grid, _
                                                               Me.HiddenEmailconsulta, _
                                                               Me.hdnEmailID, _
                                                               Me.titulo_label_expedientes, _
                                                               Me.UpdateGeneral, _
                                                               Session.Item("GA_TIPO_CONSULTA_PUBLICO"), _
                                                               Me.TextBox_busqueda.Text,
                                                               Session.Item("GA_DATO_CONSULTA_PUBLICO"), _
                                                               Session.Item("SortExpression_publico"), _
                                                               Session.Item("SortDirection_publico"))
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ImageButton_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_buscar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassPqrs
            Dim Result As String = ""
            If Me.TextBox_busqueda.Text <> "" Then
                Result = Refclas.Consulta_pqrs_registrados_usuario(Session.Item("PQRS_ID_USUARIO_PQRS"), _
                                                                         Me.data_grid, _
                                                                         Me.HiddenEmailconsulta, _
                                                                         Me.hdnEmailID, _
                                                                         Me.titulo_label_expedientes, _
                                                                         Me.UpdateGeneral, _
                                                                         2, _
                                                                         Me.TextBox_busqueda.Text, _
                                                                         Session.Item("GA_DATO_CONSULTA_PUBLICO"), _
                                                                         Session.Item("SortExpression_publico"), _
                                                                         Session.Item("SortDirection_publico"))
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
                End If
            Else
                Result = Refclas.Consulta_pqrs_registrados_usuario(Session.Item("PQRS_ID_USUARIO_PQRS"), _
                                                                         Me.data_grid, _
                                                                         Me.HiddenEmailconsulta, _
                                                                         Me.hdnEmailID, _
                                                                         Me.titulo_label_expedientes, _
                                                                         Me.UpdateGeneral, _
                                                                         1, _
                                                                         Me.TextBox_busqueda.Text, _
                                                                         Session.Item("GA_DATO_CONSULTA_PUBLICO"), _
                                                                         Session.Item("SortExpression_publico"), _
                                                                         Session.Item("SortDirection_publico"))
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If
           
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_DataBound(sender As Object, e As EventArgs) Handles data_grid.DataBound
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
    Private Sub data_grid_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid.PageIndexChanging
        Dim clas As New Classscrripjava
        Try
            data_grid.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim refclas As New ClassPqrs
            Me.hdnEmailID.Value = "-1"
            Result = refclas.Consulta_pqrs_registrados_usuario(Session.Item("PQRS_ID_USUARIO_PQRS"), _
                                                               Me.data_grid, _
                                                               Me.HiddenEmailconsulta, _
                                                               Me.hdnEmailID, _
                                                               Me.titulo_label_expedientes, _
                                                               Me.UpdateGeneral, _
                                                               Session.Item("GA_TIPO_CONSULTA_PUBLICO"), _
                                                               Me.TextBox_busqueda.Text,
                                                               Session.Item("GA_DATO_CONSULTA_PUBLICO"), _
                                                               Session.Item("SortExpression_publico"), _
                                                               Session.Item("SortDirection_publico"))
            'Result = refclas.Consulta_pqrs_registrados_usuario_post(Me.UpdateGeneral, Me.hdnEmailID, Me.HiddenEmailconsulta, Me.data_grid, Me.titulo_label_expedientes)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.hdnEmailID.Value = "-1"
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid.Sorting
        Dim clas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim refclas As New ClassPqrs
            Me.hdnEmailID.Value = "-1"
            Session.Item("SortExpression_publico") = e.SortExpression
            If Session.Item("SortDirection_publico") = "DESC" Then
                Session.Item("SortDirection_publico") = "ASC"
            Else
                Session.Item("SortDirection_publico") = "DESC"
            End If
            Result = refclas.Consulta_pqrs_registrados_usuario(Session.Item("PQRS_ID_USUARIO_PQRS"), _
                                                               Me.data_grid, _
                                                               Me.HiddenEmailconsulta, _
                                                               Me.hdnEmailID, _
                                                               Me.titulo_label_expedientes, _
                                                               Me.UpdateGeneral, _
                                                               Session.Item("GA_TIPO_CONSULTA_PUBLICO"), _
                                                               Me.TextBox_busqueda.Text,
                                                               Session.Item("GA_DATO_CONSULTA_PUBLICO"), _
                                                               Session.Item("SortExpression_publico"), _
                                                               Session.Item("SortDirection_publico"))
            'Result = refclas.Consulta_pqrs_registrados_usuario_post(Me.UpdateGeneral, Me.hdnEmailID, Me.HiddenEmailconsulta, Me.data_grid, Me.titulo_label_expedientes)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.hdnEmailID.Value = "-1"
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    
    
    Private Sub Button_Trazabilidad_Click(sender As Object, e As EventArgs) Handles Button_Trazabilidad.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el radicado", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("PU_TRAZABILIDAD") = Me.hdnEmailID.Value
            Me.Iframe_trazabilidad_.Attributes("SRC") = "../workflow/WebFormTrazabilidadWorkflow.aspx"
            Me.UpdatePanel_trazabilidad.Update()
            Me.ModalPopupExtender_trazabilidad.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")

        End Try
    End Sub

    Protected Sub Button_Log_respuesta_Click(sender As Object, e As EventArgs) Handles Button_Log_respuesta.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el radicado", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_respuesta_radicado As Integer = -1
            Dim Refclas As New Classgestionrespuesta
            Dim Result As String = ""
            Result = Refclas.Reorna_id_respuesta_radicado(Me.hdnEmailID.Value, id_respuesta_radicado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_respuesta_radicado = -1 Then
                clasjava.Showscripman_menu("El tipo de radicado no requiere de una respuesta, no se realizaron transacciones para mostrar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("PU_TRAZABILIDAD") = id_respuesta_radicado
            Me.Iframe_log_transacciones_.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
            Me.UpdatePanel_log_transacciones.Update()
            Me.ModalPopupExtender_log_transacciones.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_detalle_radicado_Click(sender As Object, e As EventArgs) Handles Button_detalle_radicado.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el radicado", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_respuesta_radicado As Integer = -1
            Dim Refclas As New Classgestionrespuesta
            Dim Result As String = ""
            Result = Refclas.Reorna_id_respuesta_radicado(Me.hdnEmailID.Value, id_respuesta_radicado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_respuesta_radicado = -1 Then
                clasjava.Showscripman_menu("El tipo de radicado no requiere de una respuesta, no hay detalles para mostrar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("PU_TRAZABILIDAD") = Me.hdnEmailID.Value
            Me.Iframe_transacciones_.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
            Me.UpdatePanel_transacciones.Update()
            Me.ModalPopupExtender_transacciones.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click

        Dim scripjava As New Classscrripjava
        Try
            Session.Item("SESIONITERCAMBIOVISOR") = "VISOR RADICADOR|" & Me.hdnEmailID.Value + "|" & Session.Item("PQRS_CODIGO_PLANTILLA_RADICADO")
            Me.Iframe_imagen_respuesta_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_imagen_respuesta.Update()
            Me.ModalPopupExtender_imagen_respuesta.Show()
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.Updatepanel_botones_visor_emergente)
        End Try

    End Sub
End Class