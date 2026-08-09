Imports System.IO

Public Class WebFormGaAplicarTrd
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Refclas_indice As New ClassWorkflowIndiceDA
        Dim Result As String = ""
        If Me.IsPostBack = False Then
            Dim Refclastrd As New ClassTrdDocumental
            Dim Refclas_Instrumento As New ClassGaGestionInstrumento
            Dim codigo_Area As Integer = 0
            Dim nombre_area As String = ""
            Dim rsocial As String = ""
            Dim id_empresa As Integer = 0
            Dim id_organigrama As Integer = 0
            If HttpContext.Current.Session.Item("TRD_APLICA_ID_SERIE") = -1 Then
                Result = Refclastrd.Retorna_Datos_Datos_Area(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                             codigo_Area, _
                                                             nombre_area, _
                                                             rsocial, _
                                                             id_empresa)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                Else
                    Result = Refclastrd.Retorna_id_organigrama_empresa_gestion(id_empresa, _
                                                                               id_organigrama)
                    Hidden_id_organigrama.Value = id_organigrama
                    If Result <> "YES" Then
                        Label_estado.Text = Label_estado.Text & Result
                    End If
                    Dim Ref_class_registro_instrumento As New Class_ra_registro_instrumento_archivistico
                    Result = Ref_class_registro_instrumento.Retorna_id_instrumento_activo(1, _
                                                                                          id_organigrama, _
                                                                                          Hidden_0001.Value)
                    If Result <> "YES" Then
                        Label_estado.Text = Label_estado.Text & Result
                    End If
                    If Hidden_0001.Value = -1 Then
                        Label_estado.Text = Label_estado.Text & "Imposible encontrar instrumento archivístico activo"
                    End If
                    Dim ref_ComboBoxArea As DropDownList = sender.page.findcontrol("ComboBoxArea")
                    Result = Refclastrd.solicita_areas_de_gestion_organigrama(id_organigrama, _
                                                                              ref_ComboBoxArea)
                    If Result <> "YES" Then
                        Label_estado.Text = Label_estado.Text & Result
                    End If
                End If
            Else
                Me.ComboBoxArea.Visible = False
                Me.Button_asignar.Visible = False
                Me.Button_Exportar_Lista.Visible = False
                '------------------------------------------
                'Solicita id insturmento serie documental
                '------------------------------------------
                Result = Refclastrd.Solicita_id_instrumento_serie_documental(HttpContext.Current.Session.Item("TRD_APLICA_ID_SERIE"), _
                                                                             Hidden_0001.Value)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                End If
                Result = Refclastrd.lista_series_sub_series_tipo(Hidden_0001.Value, _
                                                                 -1, _
                                                                 HttpContext.Current.Session.Item("TRD_APLICA_ID_SERIE"), _
                                                                 HttpContext.Current.Session.Item("TRD_APLICA_ID_SUB_SERIE"), _
                                                                 "", _
                                                                 Me.GridViewlista, _
                                                                 Me.UpdatePanelmensaje)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                End If
            End If
        End If
        Dim refclas_radic As New ClassRadicador
        If HttpContext.Current.Session.Item("TRD_APLICA_ID_SERIE") = -1 Then
            'Result = refclas_radic.agregar_auto_complete_tipos(Me.TextBox_buequeda_general.ID, Panel_busq, "GetPosiblesTipos", "tipo_doc_series", Hidden_0001.Value)
            'If Result <> "YES" Then
            '    Label_estado.Text = Label_estado.Text & Result
            'End If
        Else
            Dim value_context As String = HttpContext.Current.Session.Item("TRD_APLICA_ID_SERIE") & "|" & _
                HttpContext.Current.Session.Item("TRD_APLICA_ID_SUB_SERIE") & "|" & Hidden_0001.Value
            'Result = refclas_radic.agregar_auto_complete_tipos_serie_sub_serie(Me.TextBox_buequeda_general.ID, Panel_busq, "GetPosiblesTipos_serie_sub_series", "tipo_doc_series", value_context)
            'If Result <> "YES" Then
            '    Label_estado.Text = Label_estado.Text & Result
            'End If
        End If
      
    End Sub

    Private Sub ComboBoxArea_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxArea.SelectedIndexChanged
        Dim Result As String = ""
        Dim Reflcatrd As New ClassTrdDocumental
        Dim clasjava As New Classscrripjava
        Try
            If Me.ComboBoxArea.Text = "Seleccione Área o departamento" Then
                Result = Reflcatrd.lista_series_sub_series_tipo(-1, _
                                                                -1, _
                                                                -1, _
                                                                -1, _
                                                                "", _
                                                                Me.GridViewlista, _
                                                                Me.UpdatePanelmensaje)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.update_panel_drowlist, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Exit Sub
                End If
            End If

            Dim id_area As Integer = 0
            Hidden_id_area.Value = id_area
            Result = Reflcatrd.Retorna_id_area_por_organigrama_nombrearea(Hidden_id_organigrama.Value, _
                                                                          Me.ComboBoxArea.Text, _
                                                                          id_area)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_panel_drowlist, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_id_area.Value = id_area
            End If
            Result = Reflcatrd.lista_series_sub_series_tipo(-1, _
                                                             id_area, _
                                                             -1, _
                                                             -1, _
                                                             "", _
                                                             Me.GridViewlista, _
                                                             Me.UpdatePanelmensaje)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_panel_drowlist, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.update_panel_drowlist, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridViewlista_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowCreated
        e.Row.Cells(1).Visible = False
        e.Row.Cells(2).Visible = False
        e.Row.Cells(3).Visible = False
    End Sub


    Private Sub Button_Exportar_Lista_Click(sender As Object, e As EventArgs) Handles Button_Exportar_Lista.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassWorkflowReportes
        Dim clasjava As New Classscrripjava
        Try
            Dim dat As Date
            dat = Now
            'Refclasreposte.ExportToExcel(Me.GridView_val_radicacion)
            If Me.Hidden_colum_header.Value = "" Then
                'scripjava.Showscripman("Imposible encontrar las columnas del data gred ", Me.Updatepanel_botones)
                'clasjava.Showscripman_menu("I ", Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((20 * Rnd()) + 1))
            Dim ru_server As String = Server.MapPath(HttpContext.Current.Session.Item("GA_RUTA_TEMPO") & Session.Item("GA_IDUSUARIOGESTION"))
            If Directory.Exists(ru_server) = False Then
                Directory.CreateDirectory(ru_server)
            End If
            Dim ruta_archivo As String = ru_server & "\" & value.ToString & "Descarga_trd" & ".xls"
            'UpdatePanelmensaje.Update()
            Result = Refclasreposte.genera_xls(Me.GridViewlista, ".xls", ruta_archivo, Me.Hidden_colum_header.Value)
            If Result <> "YES" Then
                'scripjava.Showscripman(Result, Me.Updatepanel_botones)
                clasjava.Showscripman_menu(Result, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else

                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("GA_RUTA_TEMPO") & Session.Item("GA_IDUSUARIOGESTION") & "/" & value.ToString & "Descarga_trd" & ".xls"
                    ifmExcel_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                    updatapanel_iframe.Update()
                End If
            End If
        Catch ex As Exception
            'scripjava.Showscripman(ex.Message, Me.Updatepanel_botones)
            clasjava.Showscripman_menu(ex.Message, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_busqueda_tipo_general_Click(sender As Object, e As EventArgs) Handles Button_busqueda_tipo_general.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Reflcatrd As New ClassTrdDocumental
            If Hidden_0001.Value = -1 Then
                clasjava.Showscripman_menu("El sistema no pudo encontrar el instrumento archivístico predeterminado, consulte con su administrador", Me.update_panel_drowlist, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Reflcatrd.lista_series_sub_series_tipo(Hidden_0001.Value, _
                                                             -1, _
                                                             HttpContext.Current.Session.Item("TRD_APLICA_ID_SERIE"), _
                                                             HttpContext.Current.Session.Item("TRD_APLICA_ID_SUB_SERIE"), _
                                                             Me.TextBox_buequeda_general.Text, _
                                                             Me.GridViewlista, _
                                                             Me.UpdatePanelmensaje)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.update_panel_drowlist, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.update_panel_drowlist, "ModalPopupExtender_mensaje_personalizado")

        End Try
    End Sub

End Class