Public Class WebFormGaconsultaclasificacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
      
        If Me.IsPostBack = False Then
            'Result = Refclas.Lista_cuadro_clasificacion_documental_drowlist(Me.DropDownList_nivel_clasficacion)
            'If Result <> "YES" Then
            '    Me.LabelEstado.Text = Me.LabelEstado.Text & "|" & Result
            'End If
            Dim Result As String = ""
            Dim Refclas As New Class_empresa_gestion_documental
            Dim Refclass_clasificacion As New ClassGaClasificacionDocumental
            Result = Refclas.Solicita_listado_empresa(0, _
                                                      Me.DropDownList_nivel_clasficacion, _
                                                      UpdatePanel_estructura_clasificacion)
            If Result <> "YES" Then
                Me.Label_estado.Text = Me.Label_estado.Text & "|" & Result
            End If
            If Not Me.DropDownList_nivel_clasficacion.SelectedItem Is Nothing Then
                Result = Refclass_clasificacion.Listar_cuadro_clasificacion_documental(Me.DropDownList_nivel_clasficacion.SelectedValue, _
                                                                                       TreeViewEstructura, _
                                                                                       0)
                If Result <> "YES" Then
                    Me.Label_estado.Text = Me.Label_estado.Text & "|" & Result
                End If
            End If
        End If
    End Sub

    
    Private Sub data_grid_documentos_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles data_grid_documentos.RowCommand
        Dim clas As New Classscrripjava
        Try
            If e.CommandName = "visor" Then
                Dim index = Convert.ToInt32(e.CommandArgument)
                Dim id_unidad_documental As Integer = 0
                Dim row As GridViewRow = data_grid_documentos.Rows(index)
                data_grid_documentos.SelectedIndex = index
                id_unidad_documental = Val(row.Cells(1).Text)
                'Dim descripcion_unidad As String = "Unidad documental seleccionada : identificador unico " & id_unidad_documental.ToString & " Nombre  " & row.Cells(2).Text
                Session.Item("CC_SESIONITERCAMBIOVISOR") = row.Cells(1).Text & "|" & row.Cells(2).Text
                Iframe_visor_externo__.Attributes.Add("src", "../gestion/WebFormGaVisorClasificacion.aspx")
                UpdatePanel_visor_externo.Update()
                ModalPopupExtender_visor_externo.Show()
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles data_grid.RowCommand
        Dim clas As New Classscrripjava
        Try
            If e.CommandName = "Increase" Then
                Dim index = Convert.ToInt32(e.CommandArgument)
                Dim id_unidad_documental As Integer = 0
                Dim row As GridViewRow = data_grid.Rows(index)
                data_grid.SelectedIndex = index
                id_unidad_documental = Val(row.Cells(1).Text)
                Dim descripcion_unidad As String = "Unidad documental seleccionada : identificador unico " & id_unidad_documental.ToString & " Nombre  " & row.Cells(2).Text
                Dim Result As String = ""
                'Dim Refclas As New ClassGaClasificacionDocumental
                'Result = Refclas.Lista_documentos_relacionados_a_unidad_documental(id_unidad_documental, _
                'Me.UpdateGeneral_documentos, Me.hdnEmailID_documentos, Me.HiddenEmailconsulta_documentos, Me.data_grid_documentos, _
                'Me.titulo_label_expedientes_documentos, Me.Label_expediente_seleccionado, Me.UpdatePanel_expediente_seleccionado, descripcion_unidad)
                'If Result <> "YES" Then
                '    clas.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
                'End If
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid.PageIndexChanging
        Dim clas As New Classscrripjava
        Try
            data_grid.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim refclas As New ClassGaClasificacionDocumental
            Result = refclas.Lista_jerarquia_expedientes_unidades_simples_por_serie_area(HttpContext.Current.Session.Item("serie_expe_clasificacion"), _
                                                                                    HttpContext.Current.Session.Item("nivel_expe_clasificacion"), _
                                                                                    Me.data_grid, _
                                                                                    HiddenEmailconsulta, _
                                                                                    Me.titulo_label_expedientes, _
                                                                                    Me.hdnEmailID, _
                                                                                    UpdateGeneral, _
                                                                                    Me.data_grid_documentos, _
                                                                                    Me.titulo_label_expedientes_documentos, _
                                                                                    Me.UpdateGeneral_documentos, _
                                                                                    HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion"), _
                                                                                    HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"), _
                                                                                    HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"), _
                                                                                    "", _
                                                                                    UpdatePanel_title_expediente)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid.Sorting
        Dim clas As New Classscrripjava
        Try
            Session.Item("SortExpression_expe_clasificacion") = e.SortExpression
            If Session.Item("SortDirection_expe_clasificacion") = "DESC" Then
                Session.Item("SortDirection_expe_clasificacion") = "ASC"
            Else
                Session.Item("SortDirection_expe_clasificacion") = "DESC"
            End If
            Dim Result As String = ""
            Dim refclas As New ClassGaClasificacionDocumental
            Result = refclas.Lista_jerarquia_expedientes_unidades_simples_por_serie_area(HttpContext.Current.Session.Item("serie_expe_clasificacion"), _
                                                                                         HttpContext.Current.Session.Item("nivel_expe_clasificacion"), _
                                                                                         Me.data_grid, _
                                                                                         HiddenEmailconsulta, _
                                                                                         Me.titulo_label_expedientes, _
                                                                                         Me.hdnEmailID, _
                                                                                         UpdateGeneral, _
                                                                                         Me.data_grid_documentos, _
                                                                                         Me.titulo_label_expedientes_documentos, _
                                                                                         Me.UpdateGeneral_documentos, _
                                                                                         HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion"), _
                                                                                         HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"), _
                                                                                         HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"), _
                                                                                         "", _
                                                                                         UpdatePanel_title_expediente)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
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
    Private Sub data_grid_documentos_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_documentos.PageIndexChanging
        Dim clas As New Classscrripjava
        Try
            data_grid_documentos.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim refclas As New ClassGaClasificacionDocumental   
            Me.hdnEmailID_documentos.Value = "-1"
            Dim id_unidad_documental As Integer = 0
            id_unidad_documental = Val(hdnEmailID.Value)
            Dim descripcion_unidad As String = "Unidad documental seleccionada : identificador unico " & id_unidad_documental.ToString & " Nombre  " & id_unidad_documental
            Result = refclas.Lista_documentos_relacionados_a_unidad_documental(id_unidad_documental,
                                                                               Me.UpdateGeneral_documentos,
                                                                               Me.hdnEmailID_documentos,
                                                                               Me.HiddenEmailconsulta_documentos,
                                                                               Me.data_grid_documentos,
                                                                               Me.titulo_label_expedientes_documentos,
                                                                               Me.Label_expediente_seleccionado,
                                                                               Me.UpdatePanel_expediente_seleccionado,
                                                                               descripcion_unidad,
                                                                               Session.Item("GA_TIPO_CONSULTA_doc_clasificacion"),
                                                                               Session.Item("SortExpression_doc_clasificacion"),
                                                                               Session.Item("SortDirection_doc_clasificacion"),
                                                                                "", UpdatePanel_documentos_exp_title)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_documentos_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_documentos.Sorting
        Dim clas As New Classscrripjava
        Try
            Session.Item("SortExpression_doc_clasificacion") = e.SortExpression
            If Session.Item("SortDirection_doc_clasificacion") = "DESC" Then
                Session.Item("SortDirection_doc_clasificacion") = "ASC"
            Else
                Session.Item("SortDirection_doc_clasificacion") = "DESC"
            End If
            Dim Result As String = ""
            Dim refclas As New ClassGaClasificacionDocumental
            Me.hdnEmailID_documentos.Value = "-1"
            Dim id_unidad_documental As Integer = 0
            id_unidad_documental = Val(hdnEmailID.Value)
            Dim descripcion_unidad As String = "Unidad documental seleccionada : identificador unico " & id_unidad_documental.ToString & " Nombre  " & id_unidad_documental
            Result = refclas.Lista_documentos_relacionados_a_unidad_documental(id_unidad_documental,
                                                                               Me.UpdateGeneral_documentos,
                                                                               Me.hdnEmailID_documentos,
                                                                               Me.HiddenEmailconsulta_documentos,
                                                                               Me.data_grid_documentos,
                                                                               Me.titulo_label_expedientes_documentos,
                                                                               Me.Label_expediente_seleccionado,
                                                                               Me.UpdatePanel_expediente_seleccionado,
                                                                               descripcion_unidad,
                                                                               Session.Item("GA_TIPO_CONSULTA_doc_clasificacion"),
                                                                               Session.Item("SortExpression_doc_clasificacion"),
                                                                               Session.Item("SortDirection_doc_clasificacion"),
                                                                                "",
                                                                                UpdatePanel_documentos_exp_title)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_documentos_DataBound(sender As Object, e As EventArgs) Handles data_grid_documentos.DataBound
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
    Protected Sub DropDownList_nivel_clasficacion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_nivel_clasficacion.SelectedIndexChanged
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try
            If Me.DropDownList_nivel_clasficacion.Text = "" Then
                Me.TreeViewEstructura.Nodes.Clear()
                update_tre_principal.Update()
                Me.Hidden_id_cuadro.Value = 0
                Exit Sub
            End If
            Result = Refclas.Listar_cuadro_clasificacion_documental(Me.DropDownList_nivel_clasficacion.SelectedValue, _
                                                                    Me.TreeViewEstructura, _
                                                                    0)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Result = Refclas.Retorna_id_cuadro_clasificacion_documental(Me.DropDownList_nivel_clasficacion.Text, Me.Hidden_id_cuadro.Value)
                If Result <> "YES" Then
                    classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                update_tre_principal.Update()
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub TreeViewEstructura_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeViewEstructura.SelectedNodeChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Tagform As String = sender.selectedvalue()
            Dim Result As String = ""
            Dim Refclas As New ClassGaClasificacionDocumental
            Me.Hidden_result_selecion.Value = ""
            Result = Refclas.Listar_niveles_cuadro_clasficacion_documental_treview_Consulta(Tagform, _
                                                                                            Me.TreeViewEstructura.SelectedNode, _
                                                                                            Me.HiddenEmailconsulta, _
                                                                                            Me.titulo_label_expedientes, _
                                                                                            Me.hdnEmailID, UpdateGeneral, _
                                                                                            Me.data_grid, _
                                                                                            Me.data_grid_documentos, _
                                                                                            Me.titulo_label_expedientes_documentos, _
                                                                                            Me.UpdateGeneral_documentos, _
                                                                                            Me.UpdatePanel_title_expediente)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.update_tre_principal, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_result_selecion.Value = "YES"
                Me.TreeViewEstructura.SelectedNode.ExpandAll()
                Me.update_tre_principal.Update()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.update_tre_principal, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_ver_documentos_relacionados_Click(sender As Object, e As EventArgs) Handles Button_ver_documentos_relacionados.Click
        Dim clas As New Classscrripjava
        Try
            Dim id_unidad_documental As Integer = 0
            id_unidad_documental = Val(hdnEmailID.Value)
            Me.Hidden_estado_relacion.Value = ""
            Dim descripcion_unidad As String = "Unidad documental seleccionada : identificador unico " & id_unidad_documental.ToString & " Nombre  " & id_unidad_documental
            Dim Result As String = ""
            Dim Refclas As New ClassGaClasificacionDocumental
            Dim RefclassGaExpediente As New ClassGaExpediente
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Session.Item("SortExpression_doc_clasificacion") = "ID_DOCUMENTO_DOCUARCHI_ALMACEN"
            Session.Item("SortDirection_doc_clasificacion") = "DESC"
            Session.Item("GA_DATO_CONSULTA_doc_id_unidad_clasificacion") = Val(hdnEmailID.Value)
            Result = Refclas.Lista_documentos_relacionados_a_unidad_documental(id_unidad_documental,
                                                                               Me.UpdateGeneral_documentos,
                                                                               Me.hdnEmailID_documentos,
                                                                               Me.HiddenEmailconsulta_documentos,
                                                                               Me.data_grid_documentos,
                                                                               Me.titulo_label_expedientes_documentos,
                                                                               Me.Label_expediente_seleccionado,
                                                                               Me.UpdatePanel_expediente_seleccionado,
                                                                               descripcion_unidad,
                                                                               1,
                                                                                Session.Item("SortExpression_doc_clasificacion"),
                                                                                Session.Item("SortDirection_doc_clasificacion"),
                                                                                "",
                                                                                UpdatePanel_documentos_exp_title)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdatePanel_boton_Event, "ModalPopupExtender_mensaje_personalizado")
                Me.Hidden_estado_relacion.Value = ""
            Else
                Me.Hidden_estado_relacion.Value = "YES"
            End If
            Result = RefclassGaExpediente.SolicitaDatosEstructuraExpediente(id_unidad_documental,
                                                                                          estru_unidad_conservacion)
            If Result <> "YES" Then
                Me.Label_expediente_seleccionado.Text = "Expediente : " & ""
                Me.UpdatePanel_expediente_seleccionado.Update()
                clas.Showscripman_menu(Result, Me.UpdatePanel_boton_Event, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.Label_expediente_seleccionado.Text = "Expediente : " & estru_unidad_conservacion(0).CODIGO_UNICO
                Me.UpdatePanel_expediente_seleccionado.Update()
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_Event, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_restore_busqueda_expediente_Click(sender As Object, e As EventArgs) Handles Button_restore_busqueda_expediente.Click
        Dim clas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim refclas As New ClassGaClasificacionDocumental
            If HttpContext.Current.Session.Item("serie_expe_clasificacion") = 0 Then
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("nivel_expe_clasificacion") = "" Then
                Exit Sub
            End If
            Result = refclas.Lista_jerarquia_expedientes_unidades_simples_por_serie_area(HttpContext.Current.Session.Item("serie_expe_clasificacion"), _
                                                                                         HttpContext.Current.Session.Item("nivel_expe_clasificacion"), _
                                                                                         Me.data_grid, _
                                                                                         HiddenEmailconsulta, _
                                                                                         Me.titulo_label_expedientes, _
                                                                                         Me.hdnEmailID, _
                                                                                         UpdateGeneral, _
                                                                                         Me.data_grid_documentos, _
                                                                                         Me.titulo_label_expedientes_documentos, _
                                                                                         Me.UpdateGeneral_documentos, _
                                                                                         1, _
                                                                                         HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"), _
                                                                                         HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"), _
                                                                                         Me.TextBox_busqueda.Text, _
                                                                                         Me.UpdatePanel_title_expediente)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdatePanel_boton_Event, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_Event, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_busqueda_expediente_Click(sender As Object, e As EventArgs) Handles Button_busqueda_expediente.Click
        Dim clas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim refclas As New ClassGaClasificacionDocumental
            If HttpContext.Current.Session.Item("serie_expe_clasificacion") = 0 Then
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("nivel_expe_clasificacion") = "" Then
                Exit Sub
            End If
            Result = refclas.Lista_jerarquia_expedientes_unidades_simples_por_serie_area(HttpContext.Current.Session.Item("serie_expe_clasificacion"), _
                                                                                         HttpContext.Current.Session.Item("nivel_expe_clasificacion"), _
                                                                                         Me.data_grid, _
                                                                                         HiddenEmailconsulta, _
                                                                                         Me.titulo_label_expedientes, _
                                                                                         Me.hdnEmailID, _
                                                                                         UpdateGeneral, _
                                                                                         Me.data_grid_documentos, _
                                                                                         Me.titulo_label_expedientes_documentos, _
                                                                                         Me.UpdateGeneral_documentos, _
                                                                                         2, _
                                                                                         HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"), _
                                                                                         HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"), _
                                                                                         Me.TextBox_busqueda.Text, _
                                                                                         Me.UpdatePanel_title_expediente)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdatePanel_boton_Event, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_Event, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    
    Private Sub Button_ver_documento_Click(sender As Object, e As EventArgs) Handles Button_ver_documento.Click
        Dim clas As New Classscrripjava
        Try
            Session.Item("CC_SESIONITERCAMBIOVISOR") = Me.hdnEmailID_documentos.Value & "|" & Me.Hidden_gabienete.Value
            Iframe_visor_externo__.Attributes.Add("src", "../gestion/WebFormGaVisorClasificacion.aspx")
            UpdatePanel_visor_externo.Update()
            ModalPopupExtender_visor_externo.Show()
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_busqueda_documento_Click(sender As Object, e As EventArgs) Handles Button_busqueda_documento.Click
        Dim clas As New Classscrripjava
        Try
            Dim id_unidad_documental As Integer = 0
            id_unidad_documental = Val(hdnEmailID.Value)
            Dim descripcion_unidad As String = "Unidad documental seleccionada : identificador unico " & id_unidad_documental.ToString & " Nombre  " & id_unidad_documental
            Dim Result As String = ""
            Dim Refclas As New ClassGaClasificacionDocumental
            Result = Refclas.Lista_documentos_relacionados_a_unidad_documental(id_unidad_documental,
                                                                               Me.UpdateGeneral_documentos,
                                                                               Me.hdnEmailID_documentos,
                                                                               Me.HiddenEmailconsulta_documentos,
                                                                               Me.data_grid_documentos,
                                                                               Me.titulo_label_expedientes_documentos,
                                                                               Me.Label_expediente_seleccionado,
                                                                               Me.UpdatePanel_expediente_seleccionado,
                                                                               descripcion_unidad,
                                                                               2,
                                                                                Session.Item("SortExpression_doc_clasificacion"),
                                                                                Session.Item("SortDirection_doc_clasificacion"),
                                                                                Me.TextBox_busqueda_documento.Text,
                                                                                UpdatePanel_documentos_exp_title)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
            End If

        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

   
End Class