Imports System.IO

Public Class WebFormDocumentoCompartidoOtrosUsuarios
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then
            Session.Item("SortExpression_colaboracion") = "NUMERO"
            Session.Item("SortDirection_colaboracion") = "DESC"
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Dim Result As String = reflcas_respuesta.Lista_documentos_compartidos_general_por_tipo(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                   Me.data_grid_listado_solicitudes, _
                                                                                                   HiddenEmailconsulta, _
                                                                                                   Me.Label_titulo_listado_solicitudes, _
                                                                                                   Me.hdnEmailID, _
                                                                                                   UpdateGeneral, _
                                                                                                   "Todos los compartidos", _
                                                                                                   Me.UpdatePanel_title, _
                                                                                                   1, _
                                                                                                   "", _
                                                                                                   Session.Item("SortExpression_colaboracion"), _
                                                                                                   Session.Item("SortDirection_colaboracion"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            End If
            Result = reflcas_respuesta.Retorna_numero_de_documentos_compartidos_de_un_usuario_para_otros_usuarios(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                                  HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            End If
        End If
    End Sub
    Private Sub Button_link_service_actualiza_Click(sender As Object, e As EventArgs) Handles Button_link_service_actualiza.Click
        Dim clasjava As New Classscrripjava
        Try
            Session.Item("SortExpression_colaboracion") = "NUMERO"
            Session.Item("SortDirection_colaboracion") = "DESC"
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Dim Result As String = reflcas_respuesta.Lista_documentos_compartidos_general_por_tipo(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                   Me.data_grid_listado_solicitudes, _
                                                                                                   HiddenEmailconsulta, _
                                                                                                   Me.Label_titulo_listado_solicitudes, _
                                                                                                   Me.hdnEmailID, _
                                                                                                   UpdateGeneral, _
                                                                                                   "Todos los compartidos", _
                                                                                                   Me.UpdatePanel_title, _
                                                                                                   1, _
                                                                                                   "", _
                                                                                                   Session.Item("SortExpression_colaboracion"), _
                                                                                                   Session.Item("SortDirection_colaboracion"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_DataBound(sender As Object, e As EventArgs) Handles data_grid_listado_solicitudes.DataBound
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
    Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
        Try
            'e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(8).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_solicitudes.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_listado_solicitudes.PageIndex = e.NewPageIndex
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos los compartidos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Informativo"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Para aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Para colaboración"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Eliminados"
            End If
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Dim Result As String = reflcas_respuesta.Lista_documentos_compartidos_general_por_tipo(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                   Me.data_grid_listado_solicitudes, _
                                                                                                   HiddenEmailconsulta, _
                                                                                                   Me.Label_titulo_listado_solicitudes, _
                                                                                                   Me.hdnEmailID, _
                                                                                                   UpdateGeneral, _
                                                                                                   selecion_name, _
                                                                                                   Me.UpdatePanel_title, _
                                                                                                   Session.Item("GA_TIPO_CONSULTA_SOLICITUD_COLABORACION"), _
                                                                                                   Session.Item("GA_DATO_CONSULTA_SOLICITUD_COLABORACION"), _
                                                                                                   Session.Item("SortExpression_colaboracion"), _
                                                                                                   Session.Item("SortDirection_colaboracion"))
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
                selecion_name = "Todos los compartidos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Informativo"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Para aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Para colaboración"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Eliminados"
            End If
            Session.Item("SortExpression_colaboracion") = e.SortExpression
            If Session.Item("SortDirection_colaboracion") = "DESC" Then
                Session.Item("SortDirection_colaboracion") = "ASC"
            Else
                Session.Item("SortDirection_colaboracion") = "DESC"
            End If
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Result = reflcas_respuesta.Lista_documentos_compartidos_general_por_tipo(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                     Me.data_grid_listado_solicitudes, _
                                                                                     HiddenEmailconsulta, _
                                                                                     Me.Label_titulo_listado_solicitudes, _
                                                                                     Me.hdnEmailID, _
                                                                                     UpdateGeneral, _
                                                                                     selecion_name, _
                                                                                     Me.UpdatePanel_title, _
                                                                                     Session.Item("GA_TIPO_CONSULTA_SOLICITUD_COLABORACION"), _
                                                                                     Session.Item("GA_DATO_CONSULTA_SOLICITUD_COLABORACION"), _
                                                                                     Session.Item("SortExpression_colaboracion"), _
                                                                                     Session.Item("SortDirection_colaboracion"))
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
    Private Sub Button_lik_service_boton_Click(sender As Object, e As EventArgs) Handles Button_lik_service_boton.Click
        Dim clasjava As New Classscrripjava
        Try

            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos los compartidos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Informativo"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Para aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Para colaboración"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Eliminados"
            End If
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Dim Result As String = reflcas_respuesta.Lista_documentos_compartidos_general_por_tipo(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                   Me.data_grid_listado_solicitudes, _
                                                                                                   HiddenEmailconsulta, _
                                                                                                   Me.Label_titulo_listado_solicitudes, _
                                                                                                   Me.hdnEmailID, _
                                                                                                   UpdateGeneral, _
                                                                                                   selecion_name, _
                                                                                                   Me.UpdatePanel_title, _
                                                                                                   1, _
                                                                                                   "", _
                                                                                                   Session.Item("SortExpression_colaboracion"), _
                                                                                                   Session.Item("SortDirection_colaboracion"))
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
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos los compartidos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Informativo"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "Para aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Para colaboración"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Eliminados"
            End If
            Dim Result As String = reflcas_respuesta.Lista_documentos_compartidos_general_por_tipo(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  HiddenEmailconsulta, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                   UpdateGeneral, _
                                                                                                   selecion_name, _
                                                                                                   Me.UpdatePanel_title, _
                                                                                                   2, _
                                                                                                  Me.TextBox_busqueda.Text, _
                                                                                                   Session.Item("SortExpression_colaboracion"), _
                                                                                                   Session.Item("SortDirection_colaboracion"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_ver_documentos_relacionados_Click(sender As Object, e As EventArgs) Handles Button_ver_documentos_relacionados.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_resultado_ver_documento.Value = ""
            If Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Dim Result As String = reflcas_respuesta.Retorna_documentos_relacionados_a_registro_compartido(Me.hdnEmailID.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.Hidden_resultado_ver_documento.Value = "YES"
                Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO") = Me.hdnEmailID.Value
                Session.Item("GA_STRU_DOCUMENTO_TIPO_COMPARTIDO") = "VER DOCUMENTOS"
                Me.Iframe_compartir_documento_.Attributes("SRC") = "../gestion/WebFormGaCompartirDocumento.aspx"
                Me.UpdatePanel_autoriza_compartir_documento.Update()
                Me.ModalPopupExtender_edition_autoriza_compartir_documento.Show()

            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_eliminar_registro_Click(sender As Object, e As EventArgs) Handles Button_eliminar_registro.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Hidden_result_eliminar.Value = ""
            If Me.Hidden_xxx_res.Value = 0 Then
                Exit Sub
            End If
            If Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Result = reflcas_respuesta.Elimina_registro_general_documento_compartido(Me.hdnEmailID.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Label_titulo_listado_solicitudes.Text = "Se encontraron " & Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") & " registro(s) de documentos compartidos "
                Hidden_result_eliminar.Value = "YES"
                UpdatePanel_title.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_ver_registro_colaboracion_Click(sender As Object, e As EventArgs) Handles Button_ver_registro_colaboracion.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava

        Try
            If Me.hdnEmailID.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro para ver los registros de colaboración ", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
            Dim ref_clas_compartido As New ClassGaCompartirDocumento
            Dim refclas_cd_compartidos As New Class_ra_Cd_Documentos_Compartidos
            Result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(Val(Me.hdnEmailID.Value),
                                                                                         stru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru.TIPO_REGISTRO_COMPARTIDO = 2 Then
                Result = ref_clas_compartido.Lista_usuarios_relacionados_a_solictud_de_aprobacion_doc_compartido(Val(Me.hdnEmailID.Value), _
                                                                                                                 Me.data_grid_documentos, _
                                                                                                                 HiddenEmailconsulta, _
                                                                                                                 Me.titulo_label_expedientes_documentos, _
                                                                                                                 Hidden_id_usuarios_sel, _
                                                                                                                 UpdateGeneral_documentos)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.UpdateGeneral_documentos.Update()
                Me.ModalPopupExtender_edition_usu_rel_solicitud.Show()
            Else
                HttpContext.Current.Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO_COLABORACION") = Me.hdnEmailID.Value
                Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "RELACIONADO ID DOCUMENTO"
                Me.Iframe_registro_colaboracion_.Attributes.Add("src", "../gestion/WebFormGaListaDocumentosColaboracion.aspx")
                Me.UpdatePanel_registro_colaboracion.Update()
                Me.ModalPopupExtender_edition_registro_colaboracion.Show()
            End If

        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_documentos_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_documentos.RowCreated
        e.Row.Cells(0).Visible = False
    End Sub


    Private Sub Button_export_lista_Click(sender As Object, e As EventArgs) Handles Button_export_lista.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            Dim dat As Date
            dat = Now
            'Refclasreposte.ExportToExcel(Me.GridView_val_radicacion)
            If Me.Hidden_colum_header.Value = "" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman("No hay resultados para exportar", Me.UpdatePanel_expediente_seleccionado)
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((100 * Rnd()) + 1))
            Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("GA_RUTA_TEMPO") + _
                                                       "/reportes/" + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString + "/")
            If Directory.Exists(ruta_create) = False Then
                Directory.CreateDirectory(ruta_create)
            End If
            Dim Ref As New ClassReportesRadicado
            Dim nombre_reporte As String = "LISTA RESPUESTAS A SOLICITUD DE APROBACION NUMERO  " & Me.hdnEmailID.Value
            Dim ruta_archivo As String = ruta_create + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString + value.ToString + "test.xls"
            Result = Refclasreposte.genera_xls(data_grid_documentos, ".xls", _
                                               ruta_archivo, _
                                               Hidden_colum_header.Value, _
                                               nombre_reporte, _
                                               Session.Item("GA_LOGINUSUARIOGESTION"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.UpdatePanel_expediente_seleccionado)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("GA_RUTA_TEMPO") + "/reportes/" + _
                        HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString + _
                        "/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & value.ToString + "test.xls"
                    Me.ifmExcel_reporte_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                    UpdatePanel_iframe_reporte.Update()
                    update_botonoes_opciones_solicitud_general.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_expediente_seleccionado)
        End Try
    End Sub

    Private Sub Button_descarga_certificado_Click(sender As Object, e As EventArgs) Handles Button_descarga_certificado.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro para descargar el detalle ", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim file_pdf As String = ""
            Dim Refclass As New Class_ra_cd_detalle_documento_compartido
            Result = Refclass.Genera_detalle_documento_compartido(Val(Me.hdnEmailID.Value), _
                                                                  file_pdf)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                
                Dim fileinf As New FileInfo(file_pdf)
                If File.Exists(file_pdf) Then
                    Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/PUBLIC/DESCARGA/")
                    Dim filecopia As String = ruta_local & fileinf.Name
                    If File.Exists(filecopia) Then
                        Kill(filecopia)
                    End If
                    File.Move(file_pdf, filecopia)
                    Me.Hidden_ruta_archivo.Value = "../Temp_Image/PUBLIC/DESCARGA/" & fileinf.Name
                    ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                    updatapanel_iframe.Update()
                End If
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_archiva_documento_Click(sender As Object, e As EventArgs) Handles Button_archiva_documento.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava
        Try
            If Me.Hidden_xxx_res.Value = 0 Then
                Exit Sub
            End If
            If Me.hdnEmailID.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro para archivar ", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Ref_class As New Class_ra_Cd_Documentos_Compartidos
            Result = Ref_class.Archiva_solcitud_aprobacion_documento_compartido(Me.hdnEmailID.Value)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_xxx_res.Value = 1
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    
End Class