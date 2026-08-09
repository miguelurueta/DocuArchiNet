Public Class WebFormDaConsultaDocumento
    Inherits System.Web.UI.Page
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        Dim clasjava As New Classscrripjava
        Try
            Session.Item("DA_IMAGEN") = Me.hdnEmailID_VAL.Value
            Dim Result As String = ""
            Dim Refclas As New ClassDaGabinete
            Dim extension As String = ""
            Dim id_tipo_imagen As Integer = 0
            Result = Refclas.SolicitaIdTipoImagen(Session.Item("DA_IMAGEN"), _
                                                    Session.Item("DA_GABINETE_CONSULTA"), _
                                                    id_tipo_imagen)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_botones_consulta)
                Exit Sub
            End If
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen, extension)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_botones_consulta)
                Exit Sub
            End If
            If extension = ".TIF" Or extension = ".BMP" Or extension = ".JPG" Then
                Me.Iframe_visor_externo_.Attributes("SRC") = "../Docuarchi/WebFormDaVisorDocuarchi.aspx"
                Me.UpdatePanel_visor_externo.Update()
                Me.ModalPopupExtender_visor_externo.Show()
            Else
                Me.Iframe_visor_externo_.Attributes("SRC") = "../Docuarchi/WebFormDaVisorExterno.aspx"
                Me.UpdatePanel_visor_externo.Update()
                Me.ModalPopupExtender_visor_externo.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_botones_consulta)
            Exit Sub
        End Try
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", "$(document).ready(function () {$().inicio();});"))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        LabelGabinte.Text = Session.Item("DA_GABINETE_CONSULTA")
        Me.Hidden_gabinete.Value = Session.Item("DA_GABINETE_CONSULTA")
        Dim Refclas As New ClassConsultaGabinete
        Dim Result As String = ""
        If HttpContext.Current.Session("TIPOMODULO") = "PUBLICO" Then
            Result = Refclas.Genera_interface_consulta(Session.Item("DA_GABINETE_CONSULTA"), Me.Page, 2)
            If Result <> "YES" Then
                LabelGabinte.Text = Result
            End If
        Else
            Result = Refclas.Genera_interface_consulta(Session.Item("DA_GABINETE_CONSULTA"), Me.Page, 1)
            If Result <> "YES" Then
                LabelGabinte.Text = Result
            End If
        End If

    End Sub
    Private Sub Button_consulta_general_Click(sender As Object, e As EventArgs) Handles Button_consulta_general.Click
        Dim scri As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassConsultaGabinete
            Dim refgabinete As New ClassDaGabinete
            If HttpContext.Current.Session("TIPOMODULO") = "PUBLICO" Then
                Session.Item("SortExpression_da_consulta") = "ID"
                Session.Item("SortDirection_produccion_da_consulta") = "DESC"
                Result = Refclas.Generando_Consulta_Gabinete(Me.Page, _
                                                             1, _
                                                             Session.Item("DA_GABINETE_CONSULTA"), _
                                                             2, _
                                                             TextBox_buequeda_general.Text, _
                                                             Session.Item("SortExpression_da_consulta"), _
                                                             Session.Item("SortDirection_da_consulta"))
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_consulta)
                End If
            Else

                Session.Item("SortExpression_da_consulta") = "ID"
                Session.Item("SortDirection_produccion_da_consulta") = "DESC"
                Result = Refclas.Generando_Consulta_Gabinete(Me.Page, _
                                                             1, _
                                                             Session.Item("DA_GABINETE_CONSULTA"), _
                                                             2, _
                                                             TextBox_buequeda_general.Text, _
                                                             Session.Item("SortExpression_da_consulta"), _
                                                             Session.Item("SortDirection_da_consulta"))
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_consulta)
                End If
            End If

        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanel_consulta)
        End Try
    End Sub
    Private Sub Button_consulta_Click(sender As Object, e As EventArgs) Handles Button_consulta.Click
        Dim scri As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassConsultaGabinete
            Dim refgabinete As New ClassDaGabinete
            If HttpContext.Current.Session("TIPOMODULO") = "PUBLICO" Then
                Session.Item("SortExpression_da_consulta") = "ID"
                Session.Item("SortDirection_produccion_da_consulta") = "DESC"
                Result = Refclas.Generando_Consulta_Gabinete(Me.Page, _
                                                             1, _
                                                             Session.Item("DA_GABINETE_CONSULTA"), _
                                                             1, _
                                                             "", _
                                                             Session.Item("SortExpression_da_consulta"), _
                                                             Session.Item("SortDirection_da_consulta"))
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_consulta)
                End If
            Else
                
                Session.Item("SortExpression_da_consulta") = "ID"
                Session.Item("SortDirection_produccion_da_consulta") = "DESC"
                Result = Refclas.Generando_Consulta_Gabinete(Me.Page, _
                                                             1, _
                                                             Session.Item("DA_GABINETE_CONSULTA"), _
                                                             1, _
                                                             "", _
                                                             Session.Item("SortExpression_da_consulta"), _
                                                             Session.Item("SortDirection_da_consulta"))
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_consulta)
                End If
            End If
            
        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanel_consulta)
        End Try
    End Sub

    Private Sub Button_limpiar_campos_Click(sender As Object, e As EventArgs) Handles Button_limpiar_campos.Click
        Dim scri As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassConsultaGabinete
            If HttpContext.Current.Session("TIPOMODULO") = "PUBLICO" Then
                Result = Refclas.Limpiar_campos_consulta(Me.Page, _
                                                         Session.Item("DA_GABINETE_CONSULTA"), _
                                                         2)
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_consulta)
                End If
            Else
                Result = Refclas.Limpiar_campos_consulta(Me.Page, _
                                                         Session.Item("DA_GABINETE_CONSULTA"), _
                                                         1)
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_consulta)
                End If
            End If
            
        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanel_consulta)
        End Try
    End Sub

    Private Sub GridView_val_radicacion_DataBound(sender As Object, e As EventArgs) Handles GridView_val_radicacion.DataBound
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

    Private Sub GridView_val_radicacion_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_val_radicacion.PageIndexChanging
        Dim scri As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassConsultaGabinete
            Dim refgabinete As New ClassDaGabinete
            GridView_val_radicacion.PageIndex = e.NewPageIndex
            If HttpContext.Current.Session("TIPOMODULO") = "PUBLICO" Then
               
                Result = Refclas.Generando_Consulta_Gabinete(Me.Page, _
                                                             1, _
                                                             HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"), _
                                                             3, _
                                                             HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_DA_CONSULTA"), _
                                                             HttpContext.Current.Session.Item("SortExpression_da_consulta"), _
                                                             HttpContext.Current.Session.Item("SortDirection_da_consulta"))
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_conenido_grid_val_radicacion)
                End If
            Else
               
                Result = Refclas.Generando_Consulta_Gabinete(Me.Page, _
                                                             1, _
                                                             HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"), _
                                                             3, _
                                                             HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_DA_CONSULTA"), _
                                                             HttpContext.Current.Session.Item("SortExpression_da_consulta"), _
                                                             HttpContext.Current.Session.Item("SortDirection_da_consulta"))
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_conenido_grid_val_radicacion)
                End If
            End If
        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanel_conenido_grid_val_radicacion)
        End Try
    End Sub
    Private Sub GridView_val_radicacion_Sorting(sender As Object, e As GridViewSortEventArgs) Handles GridView_val_radicacion.Sorting
        Dim scri As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassConsultaGabinete
            Dim refgabinete As New ClassDaGabinete
            Session.Item("SortExpression_da_consulta") = e.SortExpression
            If Session.Item("SortDirection_da_consulta") = "DESC" Then
                Session.Item("SortDirection_da_consulta") = "ASC"
            Else
                Session.Item("SortDirection_da_consulta") = "DESC"
            End If
            If HttpContext.Current.Session("TIPOMODULO") = "PUBLICO" Then
               
                Result = Refclas.Generando_Consulta_Gabinete(Me.Page, _
                                                            1, _
                                                            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"), _
                                                            3, _
                                                            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_DA_CONSULTA"), _
                                                            HttpContext.Current.Session.Item("SortExpression_da_consulta"), _
                                                            HttpContext.Current.Session.Item("SortDirection_da_consulta"))
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_conenido_grid_val_radicacion)
                End If
            Else

                Result = Refclas.Generando_Consulta_Gabinete(Me.Page, _
                                                             1, _
                                                             HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"), _
                                                             3, _
                                                             HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_DA_CONSULTA"), _
                                                             HttpContext.Current.Session.Item("SortExpression_da_consulta"), _
                                                             HttpContext.Current.Session.Item("SortDirection_da_consulta"))
                If Result <> "YES" Then
                    scri.Showscripman(Result, UpdatePanel_conenido_grid_val_radicacion)
                End If
            End If
        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanel_conenido_grid_val_radicacion)
        End Try
    End Sub

    Private Sub GridView_val_radicacion_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_val_radicacion.RowCreated
        Dim scri As New Classscrripjava
        Try
            'e.Row.Cells(0).Visible = False
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
            e.Row.Cells(4).Visible = False
            e.Row.Cells(5).Visible = False
            e.Row.Cells(6).Visible = False
            e.Row.Cells(7).Visible = False
            e.Row.Cells(8).Visible = False
        Catch ex As Exception
            'scri.Showscripman(ex.Message, UpdatePanel_conenido_grid_val_radicacion)
        End Try

    End Sub

    Private Sub Button_listar_tipos_Click(sender As Object, e As EventArgs) Handles Button_listar_tipos.Click
        Dim Mens As New Classscrripjava
        Try
            Dim drow_list As DropDownList = sender.page.findcontrol("ComboBoxtipo")
            Dim update_drow As UpdatePanel = sender.page.findcontrol("update_panel_drowlist")
            Dim ref_ModalPopupExtende_trd_popup = sender.page.findcontrol("ModalPopupExtende_tipo_popup")
            ref_ModalPopupExtende_trd_popup.Show()
            Dim refclas As New ClassGaTipoDocumental
            Dim Result As String = ""
            Dim matri() As String = {"DIGITALIZADO", "ELECTRONICO"}
            Result = refclas.Solicita_tipos_documentales_combo_excluyentes(drow_list, matri, Me.Hidden_valor_seleccion.Value, update_drow)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Updatepanel_botones)
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Updatepanel_botones)
            Exit Sub
        End Try
    End Sub

    Private Sub Button_lista_ayuda_tipo_Click(sender As Object, e As EventArgs) Handles Button_lista_ayuda_tipo.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaTipoDocumental
            Result = Refclas.Solicita_ayuda_tipo_documento(Me.Hidden_valor_seleccion.Value, Me.TextBoxinfotipo.Text)
            If Result <> "YES" Then
                Mens.Showscripman(Result, UpdatePanelmensaje)
                Exit Sub
            End If
            If Hidden_valor_seleccion.Value = "" Then Exit Sub
            Result = Refclas.SolicitaIdTipoFormatoDocumento(Hidden_valor_seleccion.Value,
                                                            Hidden_id_tipo.Value)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanelmensaje)
                Exit Sub
            End If
            Me.Updatepanel_botones.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, UpdatePanelmensaje)
            Exit Sub
        End Try
    End Sub

    
    Private Sub Image1_Init(sender As Object, e As EventArgs) Handles Image1.Init

    End Sub

    Private Sub ImageButton_exportar_archivo_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_exportar_archivo.Click
        Dim scrijava As New Classscrripjava
        Try
            Dim Ref_clas_gabinete As New ClassDaGabinete
            Dim Result As String = ""
            If Me.hdnEmailID_VAL.Value = -1 Then
                scrijava.Showscripman("Debe seleccionar el registro ", Me.UpdatePanelButon)
                Exit Sub
            End If
            Result = Ref_clas_gabinete.Inicializa_interface_exporta_archivo_gabinete(Me.hdnEmailID_VAL.Value,
                                                                                     HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"),
                                                                                     HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                                     ifimpre_descarga_anexo_respuesta_,
                                                                                     ModalPopupExtender_edition_descarga_anexo_respuesta,
                                                                                     UpdatePanel_descarga_anexo_respuesta, 1)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
        End Try
           
    End Sub

    Private Sub ImageButtonVisualiza_Documento_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonVisualiza_Documento.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman("Debe seleccionar un registro para visualizar", UpdatePanel_consulta)
                Exit Sub
            End If
            Session.Item("DA_IMAGEN") = Me.hdnEmailID_VAL.Value
            Dim Result As String = ""
            Dim Refclas As New ClassDaGabinete
            Dim extension As String = ""
            Dim id_tipo_imagen As Integer = 0
            Result = Refclas.SolicitaIdTipoImagen(Session.Item("DA_IMAGEN"), Session.Item("DA_GABINETE_CONSULTA"), id_tipo_imagen)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_consulta)
                Exit Sub
            End If
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen, extension)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_consulta)
                Exit Sub
            End If
            If extension = ".TIF" Or extension = ".BMP" Or extension = ".JPG" Then
                Me.Iframe_visor_externo_.Attributes("SRC") = "../Docuarchi/WebFormDaVisorDocuarchi.aspx"
                Me.UpdatePanel_visor_externo.Update()
                Me.ModalPopupExtender_visor_externo.Show()
            Else
                Me.Iframe_visor_externo_.Attributes("SRC") = "../Docuarchi/WebFormDaVisorExterno.aspx"
                Me.UpdatePanel_visor_externo.Update()
                Me.ModalPopupExtender_visor_externo.Show()
            End If
           
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_consulta)

        End Try
    End Sub

    Private Sub Button_actualiza_indice_Click(sender As Object, e As EventArgs) Handles Button_actualiza_indice.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassConsultaGabinete
            Dim campos As String = ""
            Result = Refclas.Lista_campos_edicion_gabinete(Session.Item("DA_GABINETE_CONSULTA"), campos)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_actualiza)
                Exit Sub
            Else
                Hidden_campos_dinamicos_edita.Value = campos
                Result = Refclas.lista_valores_campo_edita(Session.Item("DA_GABINETE_CONSULTA"), _
                                                           campos, _
                                                           Session.Item("DA_IMAGEN"), _
                                                           hidden_valore_campos.Value)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.Updatepanel_actualiza)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_actualiza)
        End Try
    End Sub

    Private Sub ImageButtonindice_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonindice.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman("Debe seleccionar un registro para ver el indice", UpdatePanel_consulta)
                Exit Sub
            End If

            Session.Item("DA_IMAGEN") = Me.hdnEmailID_VAL.Value
            Me.ifrm_indice_.Attributes.Add("src", "../Docuarchi/WebFormDaIndiceDocuarhi.aspx")
            Me.UpdatePanelindice.Update()
            Me.ModalPopupExtenderimpre_indice.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_consulta)
            Exit Sub
        End Try
    End Sub

  

    Protected Sub ImageButton_toponimica_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_toponimica.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            'Session.Item("DA_GABINETE_CONSULTA")
            If Me.hdnEmailID_VAL.Value = "0" Or Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro para la ubicación", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_expediente As Integer = 0
            Dim refclas_consulta As New ClassConsultaGabinete
            Result = refclas_consulta.Retorna_id_expediente_documento(Session.Item("DA_GABINETE_CONSULTA"), Me.hdnEmailID_VAL.Value, id_expediente)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_Ubicacion_expediente_por_codigo_unico(id_expediente, Me.TreeViewArchivo_u_b_t, "")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanelViewArchivo_u_b_t.Update()
                Me.ModalPopupExtende_ubicacion_toponimica_expediente_popup.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    
    Private Sub Button_pogres_show_Click(sender As Object, e As EventArgs) Handles Button_pogres_show.Click
        Me.ModalPopupExtender_edition_pro_gres_bar.Show()
    End Sub

    Private Sub Button_cerrar_pro_gres_bar_Click(sender As Object, e As EventArgs) Handles Button_cerrar_pro_gres_bar.Click
        Me.ModalPopupExtender_edition_pro_gres_bar.Hide()
    End Sub

   
End Class