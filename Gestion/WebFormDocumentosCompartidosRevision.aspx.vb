Imports System.IO

Public Class WebFormDocumentosCompartidosRevision
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Session.Item("SortExpression_compartido") = "ID_USUARIOS_DOCUMENTOS_COMPARTIDOS"
            Session.Item("SortDirection_compartido") = "DESC"
            Dim Result As String = reflcas_respuesta.Lista_solictudes_compartidas_de_un_usuario_por_tipos(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                          Me.data_grid_listado_solicitudes, _
                                                                                                          HiddenEmailconsulta, _
                                                                                                          Me.Label_titulo_listado_solicitudes, _
                                                                                                          Me.hdnEmailID, _
                                                                                                          UpdateGeneral, _
                                                                                                          "Todos los compartidos", _
                                                                                                          Me.UpdatePanel_title, _
                                                                                                          1, _
                                                                                                          "", _
                                                                                                          Session.Item("SortExpression_compartido"), _
                                                                                                          Session.Item("SortDirection_compartido"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            End If
            Result = reflcas_respuesta.Retorna_numero_de_documentos_compartidos_de_un_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                              HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            End If
            Me.DropDownList_estado_aprobacion.Items.Clear()
            Me.DropDownList_estado_aprobacion.Items.Add("Aprobado")
            Me.DropDownList_estado_aprobacion.Items.Add("Desaprobado")
            Me.DropDownList_estado_aprobacion.Items.Add("Archivado")
        End If
    End Sub
    Private Sub Button_link_acualiza_lista_Click(sender As Object, e As EventArgs) Handles Button_link_acualiza_lista.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Session.Item("SortExpression_compartido") = "ID_USUARIOS_DOCUMENTOS_COMPARTIDOS"
            Session.Item("SortDirection_compartido") = "DESC"
            Dim Result As String = reflcas_respuesta.Lista_solictudes_compartidas_de_un_usuario_por_tipos(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                          Me.data_grid_listado_solicitudes, _
                                                                                                          HiddenEmailconsulta, _
                                                                                                          Me.Label_titulo_listado_solicitudes, _
                                                                                                          Me.hdnEmailID, _
                                                                                                          UpdateGeneral, _
                                                                                                          "Todos los compartidos", _
                                                                                                          Me.UpdatePanel_title, _
                                                                                                          1, _
                                                                                                          "", _
                                                                                                          Session.Item("SortExpression_compartido"), _
                                                                                                          Session.Item("SortDirection_compartido"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    
    Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
            e.Row.Cells(4).Visible = False
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
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Eliminados"
            End If
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Dim Result As String = reflcas_respuesta.Lista_solictudes_compartidas_de_un_usuario_por_tipos(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                          Me.data_grid_listado_solicitudes, _
                                                                                                          HiddenEmailconsulta, _
                                                                                                          Me.Label_titulo_listado_solicitudes, _
                                                                                                          Me.hdnEmailID, _
                                                                                                          UpdateGeneral, _
                                                                                                          selecion_name, _
                                                                                                          Me.UpdatePanel_title, _
                                                                                                          HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO"), _
                                                                                                          HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO"), _
                                                                                                          Session.Item("SortExpression_compartido"), _
                                                                                                          Session.Item("SortDirection_compartido"))
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
                selecion_name = "Para colaboración"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Para aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Eliminados"
            End If
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Dim Result As String = reflcas_respuesta.Lista_solictudes_compartidas_de_un_usuario_por_tipos(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                          Me.data_grid_listado_solicitudes, _
                                                                                                          HiddenEmailconsulta, _
                                                                                                          Me.Label_titulo_listado_solicitudes, _
                                                                                                          Me.hdnEmailID, _
                                                                                                          UpdateGeneral, _
                                                                                                          selecion_name, _
                                                                                                          Me.UpdatePanel_title, _
                                                                                                          1, _
                                                                                                          "", _
                                                                                                          Session.Item("SortExpression_compartido"), _
                                                                                                          Session.Item("SortDirection_compartido"))
            
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
            Dim Result As String = reflcas_respuesta.Lista_solictudes_compartidas_de_un_usuario_por_tipos(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                          Me.data_grid_listado_solicitudes, _
                                                                                                          HiddenEmailconsulta, _
                                                                                                          Me.Label_titulo_listado_solicitudes, _
                                                                                                          Me.hdnEmailID, _
                                                                                                          UpdateGeneral, _
                                                                                                          selecion_name, _
                                                                                                          Me.UpdatePanel_title, _
                                                                                                          2, _
                                                                                                          Me.TextBox_busqueda.Text, _
                                                                                                          Session.Item("SortExpression_compartido"), _
                                                                                                          Session.Item("SortDirection_compartido"))

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
            If Me.Hidden_solicitud_compartido.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Dim Result As String
            Dim stru_compartido_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
            Dim refclas_cd_compartidos As New Class_ra_Cd_Documentos_Compartidos
            Result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(Me.Hidden_solicitud_compartido.Value,
                                                                                          stru_compartido_general)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Result = reflcas_respuesta.Retorna_documentos_relacionados_a_registro_compartido(Me.Hidden_solicitud_compartido.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.Hidden_resultado_ver_documento.Value = "YES"
                Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO") = Me.Hidden_solicitud_compartido.Value
                Session.Item("GA_STRU_DOCUMENTO_TIPO_COMPARTIDO") = stru_compartido_general.DESCRIPCION_TIPO_COMPARTIDO
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
            If Hiddenxxxxxx.Value = 0 Then
                Exit Sub
            End If
            If Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro", _
                                           Me.update_botonoes_opciones_solicitud_general, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim reflcas_respuesta As New Class_ra_cd_usuarios_documentos_compartidos
            Result = reflcas_respuesta.Elimina_registro_usuario_documento_compartido(Me.hdnEmailID.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else

                Label_titulo_listado_solicitudes.Text = "Se encontraron " & Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION") & " registro(s) de documentos compartidos "
                Hidden_result_eliminar.Value = "YES"
                UpdatePanel_title.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
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
            Session.Item("SortExpression_compartido") = e.SortExpression
            If Session.Item("SortDirection_compartido") = "DESC" Then
                Session.Item("SortDirection_compartido") = "ASC"
            Else
                Session.Item("SortDirection_compartido") = "DESC"
            End If
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            Result = reflcas_respuesta.Lista_solictudes_compartidas_de_un_usuario_por_tipos(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                          Me.data_grid_listado_solicitudes, _
                                                                                                          HiddenEmailconsulta, _
                                                                                                          Me.Label_titulo_listado_solicitudes, _
                                                                                                          Me.hdnEmailID, _
                                                                                                          UpdateGeneral, _
                                                                                                          selecion_name, _
                                                                                                          Me.UpdatePanel_title, _
                                                                                                          HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO"), _
                                                                                                          HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO"), _
                                                                                                          Session.Item("SortExpression_compartido"), _
                                                                                                          Session.Item("SortDirection_compartido"))
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
    Protected Sub Button_activa_desicion_aprobacion_Click(sender As Object, e As EventArgs) Handles Button_activa_desicion_aprobacion.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava
        Dim refclas As New Class_ra_cd_usuarios_documentos_compartidos
        Try
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro de solicitud", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_tipo_compartido As Integer = 0
            Result = refclas.Solicita_tipo_documento_compartido(Val(Me.hdnEmailID.Value), _
                                                                id_tipo_compartido)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_tipo_compartido <> 2 Then
                scrijava.Showscripman_menu("El documento compartido no es de aprobación, no se despliega la venta de decisión ", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_desicion_solicitud.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
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
            Dim stru_esp As STRU_DOCUMENTO_COMPARTIDO_USUARIOS = Nothing
            Dim ref_clas_compartido As New ClassGaCompartirDocumento
            Dim ref_clas As New Class_ra_cd_usuarios_documentos_compartidos
            Result = ref_clas.SolicitaeEstructuraDocumentoCompartido(Me.hdnEmailID.Value,
                                                                                        stru_esp)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, _
                                           Me.update_botonoes_opciones_solicitud_general, _
                                           "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclas_cd_compartidos As New Class_ra_Cd_Documentos_Compartidos
            Result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(stru_esp.ID_RA_CD_DOCUMENTOS_COMPARTIDOS,
                                                                                          stru)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru.TIPO_REGISTRO_COMPARTIDO = 2 Then
                Result = ref_clas_compartido.Lista_usuarios_relacionados_a_solictud_de_aprobacion_doc_compartido(stru_esp.ID_RA_CD_DOCUMENTOS_COMPARTIDOS, _
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
                scrijava.Showscripman_menu("El documento no es una solicitud de aprobación, no tiene resultados para mostrar", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
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

   
End Class