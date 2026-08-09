Imports AjaxControlToolkit
Imports System.IO

Public Class WebFormGaGestionExpediente_vb
    Inherits System.Web.UI.Page

    Private Sub WebFormGaGestionExpediente_vb_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
    End Sub
    Private Sub WebFormGaGestionExpediente_vb_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Try
            Dim rejava As New Classscrripjava
            If Me.IsPostBack = False Then

                '-------------------------------------------
                '-----Agregar auto completar
                '-------------------------------------------
                Dim Result As String = ""
                Dim refclas As New ClassRadicador
                Dim Refclas_ra_tipo_expediente As New Class_ra_tipo_expediente
                Result = Refclas_ra_tipo_expediente.lista_tipos_expedientes_Combo(Me.DropDownListtipoexpediente_,
                                                                                      Me.UpdatePaneLconsulta)
                If Result <> "YES" Then
                    'rejava.Showscripman_menu(Result, Me.UpdatePaneLconsulta, "ModalPopupExtender_mensaje_personalizado")
                    Label_estado.Text = Result
                End If
                Dim refclasunidad As New ClassUnidadConservacion
                Result = refclasunidad.lista_tipos_unidades_documentales(Me.DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL)
                If Result <> "YES" Then
                    'rejava.Showscripman_menu(Result, Me.UpdatePaneLconsulta, "ModalPopupExtender_mensaje_personalizado")
                    Label_estado.Text = Label_estado.Text & "|" & Result
                End If
                '--------------------------------------------------
                'LISTA CICLOS ARCHIVO
                '--------------------------------------------------
                Dim refclascexpediente As New ClassGaExpediente
                Result = refclascexpediente.Listar_ciclos_archivo(Me.DropDownListNOMBRE_CICLO_ARCHIVO, "", "Todas")
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result & "|"
                    Exit Sub
                End If
                '--------------------------------------------------
                'LISTA FONDOS DOCUMENTALES
                '--------------------------------------------------
                Result = refclascexpediente.Listar_fodos_documentales(Me.DropDownListNOMBRE_FONDO, "", "Todas")
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result & "|"
                    Exit Sub
                End If
                Hidden0003.Value = ""
                Hidden0007.Value = ""
                '--------------------------------------------------
                'Activa opcion vincula documentos al expediente
                '--------------------------------------------------
                If HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 2 Then
                    If Not HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA") Is Nothing Then
                        If HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA").Length > 0 Then
                            Hidden0005.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                            Hidden0007.Value = HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA")
                            Hidden0006.Value = 2
                            For i As Integer = 0 To HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA").Length - 1
                                If i = 0 Then
                                    Hidden0003.Value = HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA")(i)
                                Else
                                    Hidden0003.Value = Hidden0003.Value & "-" & HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA")(i)
                                End If
                            Next
                        End If
                    End If
                End If
                '--------------------------------------------------
                'Activa opcion copia documentos al expediente
                '--------------------------------------------------
                If HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 1 Then
                    If Not HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA") Is Nothing Then
                        If HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA").Length > 0 Then
                            Hidden0005.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                            Hidden0007.Value = HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA")
                            Hidden0006.Value = 1
                            For i As Integer = 0 To HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA").Length - 1
                                If i = 0 Then
                                    Hidden0003.Value = HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i)
                                Else
                                    Hidden0003.Value = Hidden0003.Value & "-" & HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i)
                                End If
                            Next
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
        End Try


    End Sub
    Private Sub data_grid_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid.PageIndexChanging
        Dim clas As New Classscrripjava
        Try
            data_grid.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim refclas As New ClassGaExpediente
            Me.hdnEmailID.Value = "-1"
            Result = refclas.Consulta_Expedientes_post(Me.UpdateGeneral,
                                                       Me.hdnEmailID,
                                                       Me.HiddenEmailconsulta,
                                                       Me.data_grid,
                                                       Me.titulo_label_expedientes,
                                                       HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion"),
                                                       HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"),
                                                       HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"),
                                                       "",
                                                       HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion"),
                                                       HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE"),
                                                       UpdatePanel_general_titulo)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            Else
                If Me.data_grid_documentos_exp.Rows.Count > 0 Then
                    Me.data_grid_documentos_exp.DataSource = Nothing
                    Me.data_grid_documentos_exp.DataBind()
                    Me.UpdateGeneral_documentos_exp.Update()
                    Me.Label_expediente_seleccionado_exp.Text = ""
                    Me.UpdatePanel_expediente_seleccionado_exp.Update()
                End If
               
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
            Dim refclas As New ClassGaExpediente
            Me.hdnEmailID.Value = "-1"
            Result = refclas.Consulta_Expedientes_post(Me.UpdateGeneral,
                                                       Me.hdnEmailID,
                                                       Me.HiddenEmailconsulta,
                                                       Me.data_grid,
                                                       Me.titulo_label_expedientes,
                                                       HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion"),
                                                       HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"),
                                                       HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"),
                                                       "",
                                                       HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion"),
                                                       HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE"),
                                                       UpdatePanel_general_titulo)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub WebFormGaGestionExpediente_vb_PreRenderComplete(sender As Object, e As EventArgs) Handles Me.PreRenderComplete
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Dim clasadmonempresa As New ClassAdmonEmpresa
        If Me.IsPostBack = False Then
            'If Session.Item("GA_MANAGER_GESTION") = 1 Then
            '    result = clasadmonempresa.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa,
            '                                                               Me.UpdatePaneLconsulta)
            '    If result <> "YES" Then
            '        clasjava.Show(result)
            '        Exit Sub
            '    End If

            '    Dim empresa_usuario_gestion As String = ""
            '    result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion,
            '                                                                     Session.Item("GA_IDUSUARIOGESTION"))
            '    If result <> "YES" Then
            '        clasjava.Showscripman_menu(result, Me.UpdatePaneLconsulta, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If
            '    If Me.DropDownListEntidadEmpresa.Items.Count > 0 Then
            '        Me.DropDownListEntidadEmpresa.Text = empresa_usuario_gestion
            '        Me.UpdatePaneLconsulta.Update()
            '    End If
            'Else
            '    Dim Class_empresa_gestion_documental As New Class_empresa_gestion_documental
            '    result = Class_empresa_gestion_documental.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa,
            '                                                                               Me.UpdatePaneLconsulta,
            '                                                                               Session.Item("GA_IDUSUARIOGESTION"))
            '    If result <> "YES" Then
            '        clasjava.Showscripman_menu(result, Me.UpdatePaneLconsulta, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If
            '    Dim empresa_usuario_gestion As String = ""
            '    result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion,
            '                                                                     Session.Item("GA_IDUSUARIOGESTION"))
            '    If result <> "YES" Then
            '        clasjava.Showscripman_menu(result, Me.UpdatePaneLconsulta, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If
            '    If Me.DropDownListEntidadEmpresa.Items.Count > 0 Then
            '        Me.DropDownListEntidadEmpresa.Text = empresa_usuario_gestion
            '    End If
            'End If
            HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = "CODIGO_UNICO"
            HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = "DESC"
            HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion") = ""
            HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_ADD_PAGINACION") = ""
            Dim refclaexpediente As New ClassGaExpediente
            result = refclaexpediente.Consulta_Expedientes("-11111111111111", Me.TextBoxFECHA_CREACION_INI.Text, Me.TextBoxFECHA_CREACION_FINAL.Text,
         Me.TextBoxTEMA_EXPEDIENTE_.Text, Me.TextBoxNOMBRE_AREA_TRD_.Text, Me.TextBoxNOMBRE_SERIE_TRD_.Text, Me.TextBoxNOMBRE_SUBSERIE_TRD_.Text,
         Me.TextBoxFECHA_EXTREMA_INICIAL_INICIAL_.Text, Me.TextBoxFECHA_EXTREMA_INICIAL_FINAL_.Text, Me.TextBoxFECHA_EXTREMA_FINAL_INICIAL_.Text, Me.TextBoxFECHA_EXTREMA_FINAL_FINAL_.Text,
         Me.TextBoxRANGO_EXTREMO_INICIAL_INICIAL_.Text, Me.TextBoxRANGO_EXTREMO_INICIAL_FINAL_.Text, Me.TextBoxRANGO_EXTREMO_FINAL_INICIAL_.Text, Me.TextBoxRANGO_EXTREMO_FINAL_FINAL_.Text,
         Me.DropDownListUusuariocreador_, Me.DropDownListEstado_Expediente_.Text, Me.data_grid, Me.titulo_label_expedientes, Me.DropDownListtipoexpediente_.Text,
         Me.DropDownListEstadoExpedienteSierre_.Text, Me.TextBoxASUNTO_EXPEDIENTE_.Text, Session.Item("ID_EMPRESA"), Me.UpdateGeneral, Me.hdnEmailID, Me.HiddenEmailconsulta,
         Me.CheckBoxsolo_expeidente_propio.Checked, Me.CheckBox_Asunto_.Checked, Me.CheckBox_observacion.Checked, Me.TextBoxOBSERVACION_EXPEDIENTE_.Text,
         Me.TextBoxID_EXPEDIENTE.Text, Me.TextBoxNOMBRE_SUB_AREA_.Text, DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL_.Text, Me.DropDownListNOMBRE_CICLO_ARCHIVO_.Text,
         Me.DropDownListNOMBRE_FONDO_.Text, Me.TextBoxNOMBRE_PERSONA_EXPEDIENTE_.Text, Me.TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE_.Text, Me.TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE_.Text,
         Me.TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE_.Text, Me.data_grid_documentos_exp,
         Me.titulo_label_expedientes_documentos, Me.UpdateGeneral_documentos_exp,
         1,
         HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"),
         HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"),
         "",
         HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion"),
         HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE"),
         UpdatePanel_general_titulo, Hidden_0001.Value)
        End If
    End Sub

    Private Sub ScriptManager1_PreRender(sender As Object, e As EventArgs) Handles ScriptManager1.PreRender
        'Dim script As [String] = "$(document).ready(){auto_zize();};"
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), Script, True)
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), "mover();", "", True)
    End Sub

    Private Sub UpdateGeneral_Load(sender As Object, e As EventArgs) Handles UpdateGeneral.Load
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), "auto_zize();", "", True)

    End Sub

    Private Sub UpdatePaneLconsulta_Load(sender As Object, e As EventArgs) Handles UpdatePaneLconsulta.Load
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), "mover();", "", True)
    End Sub

    Private Sub ButtonConsulta_Click(sender As Object, e As EventArgs) Handles ButtonConsulta.Click
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Try
            HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = "ID_EXPEDIENTE"
            HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = "DESC"
            HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion") = ""
            Dim refclaexpediente As New ClassGaExpediente
            Result = refclaexpediente.Consulta_Expedientes(Me.TextBoxCODIGO_UNICO.Text, Me.TextBoxFECHA_CREACION_INI.Text, Me.TextBoxFECHA_CREACION_FINAL.Text,
             Me.TextBoxTEMA_EXPEDIENTE_.Text, Me.TextBoxNOMBRE_AREA_TRD_.Text, Me.TextBoxNOMBRE_SERIE_TRD_.Text, Me.TextBoxNOMBRE_SUBSERIE_TRD_.Text,
             Me.TextBoxFECHA_EXTREMA_INICIAL_INICIAL_.Text, Me.TextBoxFECHA_EXTREMA_INICIAL_FINAL_.Text, Me.TextBoxFECHA_EXTREMA_FINAL_INICIAL_.Text, Me.TextBoxFECHA_EXTREMA_FINAL_FINAL_.Text,
             Me.TextBoxRANGO_EXTREMO_INICIAL_INICIAL_.Text, Me.TextBoxRANGO_EXTREMO_INICIAL_FINAL_.Text, Me.TextBoxRANGO_EXTREMO_FINAL_INICIAL_.Text, Me.TextBoxRANGO_EXTREMO_FINAL_FINAL_.Text,
             Me.DropDownListUusuariocreador_, Me.DropDownListEstado_Expediente_.Text, Me.data_grid, Me.titulo_label_expedientes, Me.DropDownListtipoexpediente_.Text,
             Me.DropDownListEstadoExpedienteSierre_.Text, Me.TextBoxASUNTO_EXPEDIENTE_.Text, Session.Item("ID_EMPRESA"), Me.UpdateGeneral, Me.hdnEmailID, Me.HiddenEmailconsulta,
             Me.CheckBoxsolo_expeidente_propio.Checked, Me.CheckBox_Asunto_.Checked, Me.CheckBox_observacion.Checked, Me.TextBoxOBSERVACION_EXPEDIENTE_.Text,
             Me.TextBoxID_EXPEDIENTE.Text, Me.TextBoxNOMBRE_SUB_AREA_.Text, DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL_.Text, Me.DropDownListNOMBRE_CICLO_ARCHIVO_.Text,
             Me.DropDownListNOMBRE_FONDO_.Text, Me.TextBoxNOMBRE_PERSONA_EXPEDIENTE_.Text, Me.TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE_.Text, Me.TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE_.Text,
             Me.TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE_.Text, Me.data_grid_documentos_exp,
             Me.titulo_label_expedientes_documentos, Me.UpdateGeneral_documentos_exp,
             1,
             HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"),
             HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"),
             "",
             HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion"),
             HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE"),
             UpdatePanel_general_titulo, Hidden_0001.Value)
            If Result <> "YES" Then
                Me.Hidden_res_consulta.Value = "NO"
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_res_consulta.Value = "YES"
                Me.Label_expediente_seleccionado_exp.Text = ""
                Me.UpdatePanel_expediente_seleccionado_exp.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ButtonConsultaLike_Click(sender As Object, e As EventArgs) Handles ButtonConsultaLike.Click
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Try
            HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = "ID_EXPEDIENTE"
            HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = "DESC"
            HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion") = ""
            Dim refclaexpediente As New ClassGaExpediente
            Result = refclaexpediente.Consulta_Expedientes(Me.TextBoxCODIGO_UNICO.Text, Me.TextBoxFECHA_CREACION_INI.Text, Me.TextBoxFECHA_CREACION_FINAL.Text,
             Me.TextBoxTEMA_EXPEDIENTE.Text, Me.TextBoxNOMBRE_AREA_TRD_.Text, Me.TextBoxNOMBRE_SERIE_TRD_.Text, Me.TextBoxNOMBRE_SUBSERIE_TRD_.Text,
             Me.TextBoxFECHA_EXTREMA_INICIAL_INICIAL_.Text, Me.TextBoxFECHA_EXTREMA_INICIAL_FINAL_.Text, Me.TextBoxFECHA_EXTREMA_FINAL_INICIAL_.Text, Me.TextBoxFECHA_EXTREMA_FINAL_FINAL_.Text,
             Me.TextBoxRANGO_EXTREMO_INICIAL_INICIAL_.Text, Me.TextBoxRANGO_EXTREMO_INICIAL_FINAL_.Text, Me.TextBoxRANGO_EXTREMO_FINAL_INICIAL_.Text, Me.TextBoxRANGO_EXTREMO_FINAL_FINAL_.Text,
             Me.DropDownListUusuariocreador_, Me.DropDownListEstado_Expediente_.Text, Me.data_grid, Me.titulo_label_expedientes, Me.DropDownListtipoexpediente_.Text,
             Me.DropDownListEstadoExpedienteSierre_.Text, Me.TextBoxASUNTO_EXPEDIENTE.Text, Session.Item("ID_EMPRESA"), Me.UpdateGeneral, Me.hdnEmailID, Me.HiddenEmailconsulta,
             Me.CheckBoxsolo_expeidente_propio.Checked, Me.CheckBox_Asunto_.Checked, Me.CheckBox_observacion.Checked, Me.TextBoxOBSERVACION_EXPEDIENTE.Text,
             Me.TextBoxID_EXPEDIENTE.Text, Me.TextBoxNOMBRE_SUB_AREA_.Text, DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL.Text, Me.DropDownListNOMBRE_CICLO_ARCHIVO.Text,
             Me.DropDownListNOMBRE_FONDO.Text, Me.TextBoxNOMBRE_PERSONA_EXPEDIENTE.Text, Me.TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE.Text, Me.TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE_.Text,
             Me.TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE.Text, Me.data_grid_documentos_exp,
             Me.titulo_label_expedientes_documentos, Me.UpdateGeneral_documentos_exp,
             2,
             HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"),
             HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"),
             Me.TextBox_buequeda_general.Text,
             HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion"),
             HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE"),
             UpdatePanel_general_titulo, Hidden_0001.Value)
            If Result <> "YES" Then
                Me.Hidden_res_consulta.Value = "NO"
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_res_consulta.Value = "YES"
                Me.Label_expediente_seleccionado_exp.Text = ""
                Me.UpdatePanel_expediente_seleccionado_exp.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles data_grid.RowDataBound
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub data_grid_DataBound(sender As Object, e As EventArgs) Handles data_grid.DataBound
        Try
            'Select Case sender.SortDirection
            '    Case SortDirection.Ascending
            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black

            '    Case SortDirection.Descending
            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black

            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black
            'End Select
        Catch ex As Exception
        End Try

    End Sub

    Private Sub Button_asigna_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_asigna_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Dim refclas As New ClassGaExpediente
        If Me.Hiddennameasigna.Value = "" Then
            clasjava.Showscripman_menu("El contenedor no tiene asignada una ocpción de asignación ", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End If

    End Sub
    Protected Sub ButtonRestaurar__Click(sender As Object, e As EventArgs) Handles ButtonRestaurar.Click

        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Dim Refclas As New ClassGaExpediente
        Try
            Result = Refclas.Limpia_campos_consulta_expediente(Me.Panelcampos, Me.UpdatePaneLconsulta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_nuevo_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_nuevo_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("GA_MANAGER_GESTION") <> 0 Then
            Else
                If Session.Item("GA_REGISTRA_EXPEDIENTES") = 0 Then
                    clasjava.Showscripman_menu("El usuario no tiene permiso para registrar expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If

            'If Me.DropDownListEntidadEmpresa.Text = "Seleccione" Or Me.DropDownListEntidadEmpresa.Text = "" Then
            '    clasjava.Showscripman_menu("Debe seleccionar la empresa o entidad del expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim Result As String = ""
            Dim Refclas As New ClassAdmonEmpresa
            Dim Resfclas As New ClassGaTipoDocumental
            Dim refclas_gaexpediente As New ClassGaExpediente
            Dim id_empresa As Integer = 0
            Dim refclas_rad As New ClassRadicador
            HttpContext.Current.Session.Item("SESIONITERCAMBIOEXPEDIENTE") = ""
            Result = Refclas.Retorna_nombre_empresa_usuario_gestion(HttpContext.Current.Session.Item("SESIONITERCAMBIOEXPEDIENTE"),
                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            End If

            Result = refclas_gaexpediente.Activa_registrar_expediente_conservacion(Me.Page)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            update_panel_controles.Update()
            Me.ModalPopupExtender_edition_add_edit_expediente.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_actualiza_expdientes_agregados_Click(sender As Object, e As EventArgs) Handles Button_actualiza_expdientes_agregados.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            If Session.Item("SESIONITERCAMBIOEXPEDIENTE") = "AGREGO_EXPDIENTE_VENTANA_WEB" Then
                Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion") = " Limit 50"
                Result = Refclas.Listar_expedientes_agregados(Session.Item("GA_IDUSUARIOGESTION"), _
                                                              Me.data_grid, _
                                                              Me.titulo_label_expedientes, _
                                                              Me.HiddenEmailconsulta, _
                                                              Me.hdnEmailID, _
                                                              Me.UpdateGeneral, _
                                                              HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion"), _
                                                              HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"), _
                                                              HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"), _
                                                              "", _
                                                              Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion"))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_acciones_exp)
                End If
            End If
            If Session.Item("SESIONITERCAMBIOEXPEDIENTE") = "ACTUALIZA_EXPDIENTE_VENTANA_WEB" Then
                Dim value_id As Integer = Me.hdnEmailID.Value
                Result = Refclas.Consulta_Expedientes_post(Me.UpdateGeneral,
                                                           Me.hdnEmailID,
                                                           Me.HiddenEmailconsulta,
                                                           Me.data_grid,
                                                           Me.titulo_label_expedientes,
                                                           HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion"),
                                                           HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"),
                                                           HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"),
                                                           "",
                                                           HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion"),
                                                           HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE"),
                                                           UpdatePanel_general_titulo)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_acciones_exp)
                End If
                Me.hdnEmailID.Value = value_id
            End If
            '"AGREGO_EXPDIENTE_VOLUMEN_WEB"
            If Session.Item("SESIONITERCAMBIOEXPEDIENTE") = "AGREGO_EXPDIENTE_VOLUMEN_WEB" Then
                Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion") = " Limit 50"
                Result = Refclas.Listar_expedientes_agregados_volumen(Session.Item("GA_IDUSUARIOGESTION"),
                                                                      Me.data_grid,
                                                                      Me.titulo_label_expedientes,
                                                                      Me.HiddenEmailconsulta,
                                                                      Me.hdnEmailID,
                                                                      Me.UpdateGeneral,
                                                                      Me.hdnEmailID.Value,
                                                                      HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion"),
                                                                      HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"),
                                                                      HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"),
                                                                      "",
                                                                      Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion"),
                                                                      HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE"),
                                                                      UpdatePanel_general_titulo)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_acciones_exp)

                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_acciones_exp)
        End Try

    End Sub

    Private Sub Button_Editar_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_Editar_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim refclascexpediente As New ClassGaExpediente
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para editar", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then

            Else
                If HttpContext.Current.Session.Item("GA_EDITA_EXPEDIENTES") = 0 Then
                    clasjava.Showscripman_menu("Usuario sin permisos para editar expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = refclascexpediente.Verifica_propiedad_usuario_expediente(Me.hdnEmailID.Value,
                                                                                  HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim ref_Class_empresa_gestion_documental As New Class_empresa_gestion_documental
            Dim nombre_empresa As String = ""
            Result = ref_Class_empresa_gestion_documental.Solicita_nombre_empresa_por_id(Session.Item("ID_EMPRESA"),
                                                                                nombre_empresa)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("SESIONITERCAMBIOEXPEDIENTE") = nombre_empresa & "|" & Me.hdnEmailID.Value & "|EDITAR"
            Result = refclascexpediente.Asigna_datos_interface_expediente(update_panel_controles,
                                                                         hdnEmailID,
                                                                         Hidden_id_empresa)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_add_edit_expediente.Show()
            'If Me.DropDownListEntidadEmpresa.Text = "Seleccione" Or Me.DropDownListEntidadEmpresa.Text = "" Then
            '    clasjava.Showscripman_menu("Debe seleccionar la empresa o entidad del expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            'Session.Item("SESIONITERCAMBIOEXPEDIENTE") = Me.DropDownListEntidadEmpresa.Text & "|" & Me.hdnEmailID.Value & "|EDITAR"
            'Me.Iframe_agregar_expdiente_popup_.Attributes.Add("src", "../gestion/WebFormGaEditarExpediente.aspx")
            'Me.Hidden_estado_editar.Value = "YES"
            'UpdatePanel_agregar_expdiente_popup.Update()
            'Me.ModalPopupExtende_agregar_expdiente_popup.Show()

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_eliminar_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_eliminar_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""
            Hidden_result.Value = "NO"
            If Me.HiddenPROMP.Value = "0" Then Exit Sub
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para eliminar", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then

            Else
                If HttpContext.Current.Session.Item("GA_ELIMINA_EXPEDIENTES") = 0 Then
                    clasjava.Showscripman_menu("Usuario sin permisos para eliminar expedientes", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Verifica_propiedad_usuario_expediente(Me.hdnEmailID.Value, _
                                                                       HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Result = Refclas.Eliminar_Expediente(Me.hdnEmailID.Value, _
                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                 HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                 HttpContext.Current.Session.Item("ip_host_name"), 1, 0)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_result.Value = "YES"
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_estado_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_estado_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""

            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para cambiar el estado", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '----------------------------------------
            'Retorna el estado del expediente
            '----------------------------------------
            Dim estado_publico As Integer = 0
            Dim estado_expediente As Integer = -1
            Result = Refclas.Retorna_estado_expediente(Me.hdnEmailID.Value, _
                                                       estado_expediente, _
                                                       estado_publico)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Check_ButtonAbierto.Checked = False
            Me.CheckBox_ButtonSerrado.Checked = False
            If estado_expediente = 1 Then
                Me.Check_ButtonAbierto.Checked = True
            End If
            If estado_expediente <> 1 Then
                Me.CheckBox_ButtonSerrado.Checked = True
            End If
            Me.ModalPopupExtender_cambia_estado_expediente_popup.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_actualiza_estado_expediente_popup_Click(sender As Object, e As EventArgs) Handles Button_actualiza_estado_expediente_popup.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""
            If Me.HiddenPROMP.Value = "0" Then Exit Sub
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 1 Then
                Result = Refclas.Verifica_propiedad_usuario_expediente(Me.hdnEmailID.Value, _
                                                                       HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

            End If
            Dim estado_expediente As Integer = -1
            Dim captura_estado As String = ""
            If Me.Check_ButtonAbierto.Checked = True Then
                estado_expediente = 1
                captura_estado = "Abrir"
            End If
            If Me.CheckBox_ButtonSerrado.Checked = True Then
                estado_expediente = 0
                captura_estado = "Cerrar"
            End If

            Dim estado_expediente_db As Integer = -1
            Dim estado_publico As Integer = 0
            Result = Refclas.Retorna_estado_expediente(Me.hdnEmailID.Value, _
                                                       estado_expediente_db, _
                                                       estado_publico)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_expediente_db <> estado_expediente Then
                If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                    clasjava.Showscripman_menu("Debe seleccionar un expediente para cambiar el estado", Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Me.TextBox_cambia_estado_exp_popup.Text = "" Then
                    clasjava.Showscripman_menu("Por favor digite el motivo del cambio de estado", Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.cambia_estado_abierto_serrado_expediente(Me.hdnEmailID.Value, _
                                                                          estado_expediente, _
                                                                          HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                          HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                                          HttpContext.Current.Session.Item("ip_host_name"), _
                                                                          Me.TextBox_cambia_estado_exp_popup.Text)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Dim value_id As Integer = Me.hdnEmailID.Value
                    Result = Refclas.Consulta_Expedientes_post(Me.UpdateGeneral,
                                                               Me.hdnEmailID,
                                                               Me.HiddenEmailconsulta,
                                                               Me.data_grid,
                                                               Me.titulo_label_expedientes,
                                                               HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion"),
                                                               HttpContext.Current.Session.Item("SortExpression_expe_clasificacion"),
                                                               HttpContext.Current.Session.Item("SortDirection_expe_clasificacion"),
                                                               "",
                                                               HttpContext.Current.Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion"),
                                                               HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE"),
                                                               UpdatePanel_general_titulo)
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
                    End If
                    Me.hdnEmailID.Value = value_id
                    Me.ModalPopupExtender_cambia_estado_expediente_popup.Hide()
                End If

            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.updatepanel_botones_cambia_estado_expediente_popup)
        End Try
    End Sub

    Private Sub Button_volumen_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_volumen_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("GA_MANAGER_GESTION") <> 0 Then
            Else
                If Session.Item("GA_REGISTRA_EXPEDIENTES") = 0 Then
                    clasjava.Showscripman_menu("El usuario no tiene permiso para registrar expediente volumen", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para agregar volumen", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim ref_Class_empresa_gestion_documental As New Class_empresa_gestion_documental
            Dim nombre_empresa As String = ""
            Dim Result As String = ref_Class_empresa_gestion_documental.Solicita_nombre_empresa_por_id(Session.Item("ID_EMPRESA"),
                                                                                                       nombre_empresa)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclasexp As New ClassGaExpediente
            Result = Refclasexp.Retorna_estado_expediente_volumen_anexo(Me.hdnEmailID.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("SESIONITERCAMBIOEXPEDIENTE") = nombre_empresa & "|" & Me.hdnEmailID.Value & "|VOLUMEN"
            Dim refclascexpediente As New ClassGaExpediente
            Result = refclascexpediente.Asigna_datos_interface_expediente(update_panel_controles,
                                                                         hdnEmailID,
                                                                         Hidden_id_empresa)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_add_edit_expediente.Show()
            'Me.Iframe_agregar_expdiente_popup_.Attributes.Add("src", "../gestion/WebFormGaEditarExpediente.aspx")
            'Me.Hidden_estado_editar.Value = "NO"
            'Me.UpdatePanel_agregar_expdiente_popup.Update()
            'Me.ModalPopupExtende_agregar_expdiente_popup.Show()

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_rotulo_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_rotulo_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para imprimir el rotulo", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclasexp As New ClassGaExpediente
            Dim Result As String = ""
            '------------------------------------------------------
            'Retorna plantilla impresion usuario gestión
            '------------------------------------------------------
            Dim nombre_plantilla_impresion As String = ""
            Dim id_configuracion_plantilla_rotulo As Integer = 0
            Result = Refclasexp.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                 id_configuracion_plantilla_rotulo,
                                                                                                 nombre_plantilla_impresion)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If nombre_plantilla_impresion = "" Then
                nombre_plantilla_impresion = "DEFAULT"
            End If
            Dim ruta_archivo As String = ""
            Result = Refclasexp.Genera_rotulo_Eexpediente_pdf(Me.hdnEmailID.Value,
                                                              Session.Item("GA_IDEMPRESA"),
                                                              nombre_plantilla_impresion,
                                                              ruta_archivo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu("se registro el expediente, pero no se genero el rotulo " & Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
                Me.ifimpre_post_.Attributes.Add("src", "../Gestion/WebFormimpresionfile.aspx")
                UpdatePaneliframe_post.Update()
                ModalPopupExtenderimpre_post.Show()
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_archivar_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_archivar_expediente_gestion.Click
        Dim Refclas_empresa As New ClassAdmonEmpresa
        Dim scripjava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaExpediente
            Dim id_empresa_gestion As Integer = 0
            Dim Result As String = ""
            If Session.Item("GA_MANAGER_GESTION") <> 1 Then
                If Session.Item("GA_ARCHIVA_EXPEDIENTES") = 0 Then
                    scripjava.Showscripman_menu("Usuario sin permisos para archivar expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Verifica_propiedad_usuario_expediente(Me.hdnEmailID.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar un expediente para archivar", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.DropDownListEntidadEmpresa.Text = "" Then
                scripjava.Showscripman_menu("Debe seleccionar la empresa de gestión", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_empresa.Retorna_Id_Emprea(Me.DropDownListEntidadEmpresa.Text, _
            id_empresa_gestion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_empresa.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa_r_u_e, Me.UpdatePanelEntidad_r_u_e)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownListEntidadEmpresa_r_u_e.Text = Me.DropDownListEntidadEmpresa.Text
            Me.UpdatePanelEntidad_r_u_e.Update()
            Dim refclas_unidad As New ClassGestionArchivo
            If Me.HiddenField_estado_ubicacion.Value <> "YES" Then
                Result = refclas_unidad.Listar_Entidad_Archivo_Edificio(Me.TreeViewArchivo_r_u_e, Me.DropDownListEntidadEmpresa_r_u_e.Text)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.HiddenField_estado_ubicacion.Value = "YES"
                    UpdatePanelViewArchivo_r_u_e.Update()
                End If
            End If
            ModalPopupExtende_reubicar_unidad_expediente_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub TreeViewArchivo_r_u_e_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeViewArchivo_r_u_e.SelectedNodeChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Tagform As String = sender.selectedvalue()
            Dim Result As String = ""
            Dim Refclas As New ClassGestionArchivo
            Result = Refclas.Seleccion_Treview_archivar(Tagform, _
                                                        Me.TreeViewArchivo_r_u_e.SelectedNode, _
                                                        Me.DropDownListEntidadEmpresa_r_u_e.Text, _
                                                        Me.TreeViewArchivo_r_u_e.SelectedNode.Value, _
                                                        Me.TreeViewArchivo_r_u_e.SelectedNode.Text, _
                                                        Me.TreeViewArchivo_r_u_e)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanelViewArchivo_r_u_e.Update()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_agrega_unidad_contenedora_Click(sender As Object, e As EventArgs) Handles Button_agrega_unidad_contenedora.Click
        Dim scripjava As New Classscrripjava
        Try
            If Me.DropDownListEntidadEmpresa.Text = "" Then
                scripjava.Showscripman_menu("Por favor seleccione la empresa de gestión", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim node_tag As String = ""
            If Me.TreeViewArchivo_r_u_e.Nodes.Count > 0 Then
                If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then
                    Exit Sub
                End If
                node_tag = Me.TreeViewArchivo_r_u_e.SelectedNode.Value
            Else
                Exit Sub
            End If
            If InStr(node_tag, "ENTREPAÑO") <= 0 Then
                Exit Sub
            End If
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then
                scripjava.Showscripman_menu("Por favor seleccione el entrepaño", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub

            End If
            If InStr(Me.TreeViewArchivo_r_u_e.SelectedNode.Value, "ENTREPAÑO") <= 0 Then
                scripjava.Showscripman_menu("Por favor seleccione el entrepaño", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario workflow no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 1 Then
            Else
                If Session.Item("GA_REGISTRA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para agregar unidad de conservación", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Me.Hidden_tipo_unidad_seleccion.Value = node_tag
            Me.Iframe_agregar_unidad_conservacion_popup.Attributes.Add("src", "../Gestion/WebFormGagregarunidadconservacionexpediente.aspx")
            Me.UpdatePanel_agregar_unidad_conservacion_popup.Update()
            Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_agrega_unidad_conservacion_interface_Click(sender As Object, e As EventArgs) Handles Button_agrega_unidad_conservacion_interface.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim splinodo() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.Split("|")
            Dim refclasunidad As New ClassUnidadConservacion
            Dim estru_unidad() As unidad_conservacion
            Erase estru_unidad
            Dim id_entrepaño As Integer = splinodo(0)
            Dim Result = refclasunidad.Listar_unidades_conservacion_contenedoras_de_unidades_conservacion(id_entrepaño, _
                                                                                                          1, _
                                                                                                          estru_unidad)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim no_tag_ref As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value

            Result = refclasunidad.Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion(Me.TreeViewArchivo_r_u_e, estru_unidad, _
                                                                                                 no_tag_ref, _
                                                                                                 Me.TreeViewArchivo_r_u_e.SelectedNode.Text, _
                                                                                                 Me.TreeViewArchivo_r_u_e.SelectedNode)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = ""
            Session.Item("GA_ID_UNIDAD_CONTENEDORA") = 0
            Me.UpdatePanelViewArchivo_r_u_e.Update()
            Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Hide()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ButtonButtonEditar_Click(sender As Object, e As EventArgs) Handles ButtonButtonEditar.Click
        Dim scripjava As New Classscrripjava
        Try
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then Exit Sub
            If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip <> "UNIDAD CONTENEDORA EXPEDIENTE" Then
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 1 Then
            Else
                If Session.Item("GA_EDITA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para editar unidad de conservación", Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Verifica_propiedad_usuario_unidad_conservacion(Me.TreeViewArchivo_r_u_e.SelectedNode.Value, _
                                                                                Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.Split("|")
            Session.Item("GA_ID_UNIDAD_CONTENEDORA") = split(0)
            Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = Me.DropDownListEntidadEmpresa.Text
            Me.Iframe_agregar_unidad_conservacion_popup.Attributes.Add("src", "../Gestion/WebFormGaEditarunidadconservaexpe.aspx")
            Me.UpdatePanel_agregar_unidad_conservacion_popup.Update()
            Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Show()
          
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ButtonRotulo_Click(sender As Object, e As EventArgs) Handles ButtonRotulo.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para descargar el rotulo", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'If Me.DropDownListEntidadEmpresa.Text = "Seleccione" Or Me.DropDownListEntidadEmpresa.Text = "" Then
            '    clasjava.Showscripman_menu("Debe seleccionar la empresa o entidad del expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim Result As String = ""
            Dim Refclasexp As New ClassGaExpediente
           
            '------------------------------------------------------
            'Retorna plantilla impresion usuario gestión
            '------------------------------------------------------
            Dim nombre_plantilla_impresion As String = ""
            Dim id_configuracion_plantilla_rotulo As Integer = 0
            Result = Refclasexp.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                    id_configuracion_plantilla_rotulo, nombre_plantilla_impresion)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If nombre_plantilla_impresion = "" Then
                nombre_plantilla_impresion = "DEFAULT"
            End If
            Dim ruta_archivo As String = ""
            Result = Refclasexp.Genera_rotulo_Eexpediente_pdf(Me.hdnEmailID.Value, Session.Item("GA_IDEMPRESA"), nombre_plantilla_impresion, ruta_archivo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else     
                If ruta_archivo <> "" Then
                    Dim fileinf As New FileInfo(ruta_archivo)
                    If File.Exists(ruta_archivo) Then
                        Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/PUBLIC/DESCARGA/")
                        Dim filecopia As String = ruta_local & fileinf.Name
                        If File.Exists(filecopia) Then
                            Kill(filecopia)
                        End If
                        File.Move(ruta_archivo, filecopia)
                        Me.Hidden_ruta_archivo.Value = "../Temp_Image/PUBLIC/DESCARGA/" & fileinf.Name
                        ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                        updatapanel_iframe.Update()
                    End If
                End If
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_archivar_Click(sender As Object, e As EventArgs) Handles Button_archivar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman("Debe seleccionar un expediente para archivar", Me.UpdatePanel_botones_unidad_r_u_e)
                Exit Sub
            End If
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then Exit Sub
            If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.ToString.Split("|")
                Result = Refclas.Archiva_expediente_unidad_contenedora_Archivado(split(0), _
                                                                                 Me.hdnEmailID.Value, _
                                                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                 HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                                                 HttpContext.Current.Session.Item("ip_host_name"))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_botones_unidad_r_u_e)
                    Exit Sub
                Else
                    ModalPopupExtende_reubicar_unidad_expediente_popup.Hide()
                End If
            End If
            If InStr(Me.TreeViewArchivo_r_u_e.SelectedNode.Text, "Entrepaño") > 0 Then
                Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.ToString.Split("|")
                Result = Refclas.Archiva_expediente_en_entrepano_archivado(split(0), _
                                                                           Me.hdnEmailID.Value, _
                                                                           HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                           HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                                           HttpContext.Current.Session.Item("ip_host_name"))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_botones_unidad_r_u_e)
                    Exit Sub
                Else
                    ModalPopupExtende_reubicar_unidad_expediente_popup.Hide()
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e)
        End Try
    End Sub

    Private Sub Button_ubicacio_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_ubicacio_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para mostrar la ubicación toponimica", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_Ubicacion_expediente_por_codigo_unico(Me.hdnEmailID.Value, Me.TreeViewArchivo_u_b_t, "")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanelViewArchivo_u_b_t.Update()
                Me.ModalPopupExtende_ubicacion_toponimica_expediente_popup.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_desachivar_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_desachivar_expediente_gestion.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar un expediente para mostrar la ubicación toponimica", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassGaExpediente
            If Session.Item("GA_MANAGER_GESTION") <> 1 Then
                If Session.Item("GA_ARCHIVA_EXPEDIENTES") = 0 Then
                    scripjava.Showscripman_menu("Usuario sin permisos para desarchivar expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

                Result = Refclas.Verifica_propiedad_usuario_expediente(Me.hdnEmailID.Value, Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

            End If
            If HiddenField_botones_respuesta.Value = "1" Then
                Result = Refclas.Des_Archiva_expediente(Me.hdnEmailID.Value, Session.Item("GA_IDUSUARIOGESTION"), Session.Item("GA_LOGINUSUARIOGESTION"), Session.Item("ip_host_name"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                HiddenField_botones_respuesta.Value = "-1"
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    

    Private Sub Button_actualizar_unidad_Click(sender As Object, e As EventArgs) Handles Button_actualizar_unidad.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.TreeViewArchivo_r_u_e.SelectedNode.Text = Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA")
            Me.UpdatePanelViewArchivo_r_u_e.Update()
            Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Hide()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonEliminar_unidad_contendora_Click(sender As Object, e As EventArgs) Handles ButtonEliminar_unidad_contendora.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim refclas As New ClassUnidadConservacion
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then
                scripjava.Showscripman_menu("Debe seleccionar la unidad contenedora a eliminar", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim spli_unidad_contenedora() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.Split("|")
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 0 Then
                If Session.Item("GA_ELIMINA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para eliminar unidad de conservación", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = refclas.Verifica_propiedad_usuario_unidad_conservacion(Val(spli_unidad_contenedora(0)),
                                                                                Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If


            If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip <> "UNIDAD CONTENEDORA EXPEDIENTE" Then
                scripjava.Showscripman_menu("El tipo de unidad no se puede eliminar", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '--------------------------------------------------------------
            'Elimina el tipo de unidad contenedora de expediente
            '--------------------------------------------------------------
            If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                If Hidden_result_eliminar.Value = "1" Then
                    Result = refclas.Elimina_unidad_conservacion_tipo_contenedor_expediente(Val(spli_unidad_contenedora(0)),
                                                                                            Me.TreeViewArchivo_r_u_e.SelectedNode,
                                                                                            Session.Item("GA_IDUSUARIOGESTION"),
                                                                                            Me.TreeViewArchivo_r_u_e,
                                                                                            Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                                            Session.Item("ip_host_name"),
                                                                                            UpdatePanelViewArchivo_r_u_e)
                    If Result <> "YES" Then
                        scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    End If
                    Hidden_result_eliminar.Value = "0"
                    Me.UpdatePanelViewArchivo_r_u_e.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

   
    Private Sub data_grid_documentos_exp_DataBound(sender As Object, e As EventArgs) Handles data_grid_documentos_exp.DataBound
        Try

        Catch ex As Exception
        End Try
    End Sub
    Private Sub data_grid_documentos_exp_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_documentos_exp.Sorting
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
                                                                               Me.UpdateGeneral_documentos_exp,
                                                                               Me.hdnEmailID_documentos,
                                                                               Me.HiddenEmailconsulta_documentos,
                                                                               Me.data_grid_documentos_exp,
                                                                               Me.titulo_label_expedientes_documentos,
                                                                               Me.Label_expediente_seleccionado_exp,
                                                                               Me.UpdatePanel_expediente_seleccionado_exp,
                                                                               descripcion_unidad,
                                                                               Session.Item("GA_TIPO_CONSULTA_doc_clasificacion"),
                                                                               Session.Item("SortExpression_doc_clasificacion"),
                                                                               Session.Item("SortDirection_doc_clasificacion"),
                                                                                "",
                                                                                UpdatePanel_documentos_exp_title)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral_documentos_exp, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral_documentos_exp, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_documentos_exp_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_documentos_exp.PageIndexChanging
        Dim clas As New Classscrripjava
        Try
            data_grid_documentos_exp.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim refclas As New ClassGaClasificacionDocumental
            Me.hdnEmailID_documentos.Value = "-1"
            Dim id_unidad_documental As Integer = 0
            id_unidad_documental = Val(hdnEmailID.Value)
            Session.Item("GA_TIPO_CONSULTA_doc_clasificacion") = 3
            Dim descripcion_unidad As String = "Unidad documental seleccionada : identificador unico " & id_unidad_documental.ToString & " Nombre  " & id_unidad_documental
            Result = refclas.Lista_documentos_relacionados_a_unidad_documental(id_unidad_documental,
                                                                               Me.UpdateGeneral_documentos_exp,
                                                                               Me.hdnEmailID_documentos,
                                                                               Me.HiddenEmailconsulta_documentos,
                                                                               Me.data_grid_documentos_exp,
                                                                               Me.titulo_label_expedientes_documentos,
                                                                               Me.Label_expediente_seleccionado_exp,
                                                                               Me.UpdatePanel_expediente_seleccionado_exp,
                                                                               descripcion_unidad,
                                                                               Session.Item("GA_TIPO_CONSULTA_doc_clasificacion"),
                                                                               Session.Item("SortExpression_doc_clasificacion"),
                                                                               Session.Item("SortDirection_doc_clasificacion"),
                                                                                "", UpdatePanel_documentos_exp_title)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdateGeneral_documentos_exp, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdateGeneral_documentos_exp, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_documentos_relacionado_Click(sender As Object, e As EventArgs) Handles Button_documentos_relacionado.Click
        Dim clas As New Classscrripjava
        Try
            If Me.hdnEmailID.Value <> "-1" And Me.hdnEmailID.Value <> "0" Then
                Dim id_unidad_documental As Integer = 0
                id_unidad_documental = Me.hdnEmailID.Value
                Dim descripcion_unidad As String = "Unidad documental seleccionada : identificador unico " & id_unidad_documental.ToString & " Nombre  "
                Dim Result As String = ""
                '------------------------------------------------
                'Verifica expediente produción documental 
                'par no exponer los oducmentos
                '------------------------------------------------
                Dim estado_expediente As Integer = 0
                Dim estado_publico As Integer = 0
                Dim Reclas_gestion_expediente As New ClassGaExpediente
                Result = Reclas_gestion_expediente.Retorna_estado_expediente(id_unidad_documental, _
                                                                             estado_expediente, _
                                                                             estado_publico)
                If Result <> "YES" Then
                    clas.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                'If estado_publico = 2 Then
                '    clas.Showscripman_menu("No se pueden listar los documentos relacionados debido a que el expediente o unidad documental pertenecen a la producción documental de otro usuario ", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                '    Exit Sub
                'End If
                Dim Refclas As New ClassGaClasificacionDocumental
                Session.Item("SortExpression_doc_clasificacion") = "ID_DOCUMENTO_DOCUARCHI_ALMACEN"
                Session.Item("SortDirection_doc_clasificacion") = "DESC"
                Session.Item("GA_DATO_CONSULTA_doc_id_unidad_clasificacion") = Val(hdnEmailID.Value)
                Result = Refclas.Lista_documentos_relacionados_a_unidad_documental(id_unidad_documental,
                                                                                   Me.UpdateGeneral_documentos_exp,
                                                                                   Me.hdnEmailID_documentos,
                                                                                   Me.HiddenEmailconsulta_documentos,
                                                                                   Me.data_grid_documentos_exp,
                                                                                   Me.titulo_label_expedientes_documentos,
                                                                                   Me.Label_expediente_seleccionado_exp,
                                                                                   Me.UpdatePanel_expediente_seleccionado_exp,
                                                                                   descripcion_unidad,
                                                                                   1,
                                                                                   Session.Item("SortExpression_doc_clasificacion"),
                                                                                   Session.Item("SortDirection_doc_clasificacion"),
                                                                                   "", UpdatePanel_documentos_exp_title)
                If Result <> "YES" Then
                    clas.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Else
                    Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
                    Result = Reclas_gestion_expediente.SolicitaDatosEstructuraExpediente(id_unidad_documental,
                                                                                                        estru_unidad_conservacion)
                    If Result <> "YES" Then
                        clas.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Else
                        Me.Label_expediente_seleccionado_exp.Text = "Expediente : " & estru_unidad_conservacion(0).CODIGO_UNICO
                        Me.UpdatePanel_expediente_seleccionado_exp.Update()
                    End If

                End If
            End If
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
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
            Session.Item("GA_TIPO_CONSULTA_doc_clasificacion") = 2
            Result = Refclas.Lista_documentos_relacionados_a_unidad_documental(id_unidad_documental,
                                                                               Me.UpdateGeneral_documentos_exp,
                                                                               Me.hdnEmailID_documentos,
                                                                               Me.HiddenEmailconsulta_documentos,
                                                                               Me.data_grid_documentos_exp,
                                                                               Me.titulo_label_expedientes_documentos,
                                                                               Me.Label_expediente_seleccionado_exp,
                                                                               Me.UpdatePanel_expediente_seleccionado_exp,
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
    Private Sub Button_ver_documentos_relacionados_Click(sender As Object, e As EventArgs) Handles Button_ver_documentos_relacionados.Click
        Dim clas As New Classscrripjava
        Try
            Dim id_unidad_documental As Integer = 0
            id_unidad_documental = Val(hdnEmailID.Value)
            Dim descripcion_unidad As String = "Unidad documental seleccionada : identificador unico " & id_unidad_documental.ToString & " Nombre  " & id_unidad_documental
            Dim Result As String = ""
            Dim Refclas As New ClassGaClasificacionDocumental

            Session.Item("SortExpression_doc_clasificacion") = "ID_DOCUMENTO_DOCUARCHI_ALMACEN"
            Session.Item("SortDirection_doc_clasificacion") = "DESC"
            Session.Item("GA_DATO_CONSULTA_doc_id_unidad_clasificacion") = Val(hdnEmailID.Value)
            Result = Refclas.Lista_documentos_relacionados_a_unidad_documental(id_unidad_documental,
                                                                               Me.UpdateGeneral_documentos_exp,
                                                                               Me.hdnEmailID_documentos,
                                                                               Me.HiddenEmailconsulta_documentos,
                                                                               Me.data_grid_documentos_exp,
                                                                               Me.titulo_label_expedientes_documentos,
                                                                               Me.Label_expediente_seleccionado_exp,
                                                                               Me.UpdatePanel_expediente_seleccionado_exp,
                                                                               descripcion_unidad,
                                                                               1,
                                                                                Session.Item("SortExpression_doc_clasificacion"),
                                                                                Session.Item("SortDirection_doc_clasificacion"),
                                                                                "",
                                                                                UpdatePanel_documentos_exp_title)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, Me.UpdatePanel_boton_Event, "ModalPopupExtender_mensaje_personalizado")
            Else
               
            End If

        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_Event, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_ver_documento_Click(sender As Object, e As EventArgs) Handles Button_ver_documento.Click
        Dim clas As New Classscrripjava
        Try
            Dim estado_expediente As Integer = 0
            Dim estado_publico As Integer = 0
            Dim Result As String = ""
            Dim id_expediente As Integer = 0
            id_expediente = Val(hdnEmailID.Value)
            Dim Refclas As New ClassGaExpediente
            Dim stru_expediente() As expediente_conservacion = Nothing
            Result = Refclas.SolicitaDatosEstructuraExpediente(id_expediente,
                                                               stru_expediente)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, _
                                     Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_estado_expediente(id_expediente, _
                                                       estado_expediente, _
                                                       estado_publico)
            If Result <> "YES" Then
                clas.Showscripman_menu(Result, _
                                      Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            'If estado_publico = 2 And stru_expediente(0).ID_USUARIO_GESTION <> Session.Item("GA_IDUSUARIOGESTION") Then
            '    clas.Showscripman_menu("Imposible visualizar el documento del expediente, debido a que pertenece a la producción documental de otro usuario ", _
            '                           Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Session.Item("CC_SESIONITERCAMBIOVISOR") = Me.hdnEmailID_documentos.Value & "|" & Me.Hidden_gabienete.Value
            Iframe_visor_externo_clasficacion_.Attributes.Add("src", "../gestion/WebFormGaVisorClasificacion.aspx")
            Me.UpdatePanel_visor_externo.Update()
            Me.ModalPopupExtender_visor_externo.Show()
        Catch ex As Exception
            clas.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_configura_rotulo_Click(sender As Object, e As EventArgs) Handles Button_configura_rotulo.Click
        Dim Refclas_empresa As New ClassGaExpediente
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim id_configuracion_rotulo As Integer = 0
            Dim nombre_configuracion As String = ""
            Result = Refclas_empresa.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), id_configuracion_rotulo, nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_empresa.Retorna_listado_configuracion_rotulo_expediente(nombre_configuracion, Me.DropDownList_configura_plantilla_rotulo, Me.UpdatePanel_configura_plantilla_rotulo)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_configura_plantilla_rotulo.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_aceptar_configura_plantilla_rotulo_Click(sender As Object, e As EventArgs) Handles Button_aceptar_configura_plantilla_rotulo.Click
        Dim Refclas_empresa As New ClassGaExpediente
        Dim scripjava As New Classscrripjava
        Try
            If Me.DropDownList_configura_plantilla_rotulo.Text = "" Then
                scripjava.Showscripman_menu("Seleccione la plantilla", Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim id_configuracion_rotulo As Integer = 0
            Dim nombre_configuracion As String = ""
            Result = Refclas_empresa.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), id_configuracion_rotulo, nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_configuracion_rotulo_expediente As Integer = 0
            Result = Refclas_empresa.Retorna_id_nombre_configuracion_rotulo_expediente(Me.DropDownList_configura_plantilla_rotulo.Text, id_configuracion_rotulo_expediente)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_configuracion_rotulo = 0 Then
                Result = Refclas_empresa.Registra_configuracion_rotulo_expediente_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), id_configuracion_rotulo_expediente)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = Refclas_empresa.Actualiza_configuracion_rotulo_expediente_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), id_configuracion_rotulo_expediente)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Me.ModalPopupExtender_edition_configura_plantilla_rotulo.Hide()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_cerrar_emergente_Click(sender As Object, e As EventArgs) Handles Button_cerrar_emergente.Click
        Me.ModalPopupExtender_visor_externo.Hide()
    End Sub

    Private Sub Button_listar_volumenes_relacionados_Click(sender As Object, e As EventArgs) Handles Button_listar_volumenes_relacionados.Click
        Dim clas As New Classscrripjava
        Try
            If Me.hdnEmailID.Value <> "-1" And Me.hdnEmailID.Value <> "0" Then
                Dim Refclas As New Class_ra_relacion_expediente
                Dim Result As String = ""
                Dim estado_padre As String = ""
                Result = Refclas.Verfica_existencia_expediente_padre_volumen(Val(Me.hdnEmailID.Value), _
                                                                             estado_padre)
                If Result <> "YES" Then
                    clas.Showscripman(Result, Me.UpdatePanel_botones_opcion)
                    Exit Sub
                End If
                If estado_padre = "NO" Then
                    clas.Showscripman("El expediente no tiene expedientes relacionados como volumen para mostrar :" & Me.hdnEmailID.Value, Me.UpdatePanel_botones_opcion)
                    Exit Sub
                End If
                Result = Refclas.Solicita_listado_expedientes_volumenes_relacionados(Val(Me.hdnEmailID.Value),
                                                                                     Me.titulo_volumenes_relacionados,
                                                                                     Me.data_grid_volumenes_relacionados,
                                                                                     Me.hdn_id_elment,
                                                                                     Me.UpdateGeneral_volumenes_relacionados,
                                                                                     Me.UpdatePanel_volumenes_relacionados_title)


                If Result <> "YES" Then
                    clas.Showscripman(Result, Me.UpdatePanel_botones_opcion)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_volumenes_relacionados.Show()
                End If
            End If
        Catch ex As Exception
            clas.Showscripman(ex.Message, Me.UpdatePanel_botones_opcion)
        End Try
    End Sub

    Private Sub Button_active_eli_rel_Click(sender As Object, e As EventArgs) Handles Button_active_eli_rel.Click
        Dim clas As New Classscrripjava
        Try
            If Me.HiddenPROMP.Value = "0" Then Exit Sub
            If Me.Hidden_eli_rel.Value <> "-1" And Me.Hidden_eli_rel.Value <> "0" Then
                Dim Refclas As New Class_ra_relacion_expediente
                Dim Result As String = ""
                Dim Estado_padre As String = ""
                Result = Refclas.Verfica_existencia_expediente_padre_volumen(Val(Me.Hidden_eli_rel.Value), _
                                                                            Estado_padre)
                If Result <> "YES" Then
                    clas.Showscripman(Result, Me.UpdatePanel_boton_volumenes_relacionados)
                    Exit Sub
                End If
                If Estado_padre = "YES" Then
                    Result = Refclas.Solicita_listado_expedientes_volumenes_relacionados(Val(Me.hdnEmailID.Value),
                                                                                        Me.titulo_volumenes_relacionados,
                                                                                         Me.data_grid_volumenes_relacionados,
                                                                                        Me.hdn_id_elment,
                                                                                        Me.UpdateGeneral_volumenes_relacionados,
                                                                                        Me.UpdatePanel_volumenes_relacionados_title)


                    If Result <> "YES" Then
                        clas.Showscripman(Result, Me.UpdatePanel_boton_volumenes_relacionados)
                        Exit Sub
                    Else
                        Me.ModalPopupExtender_edition_volumenes_relacionados.Show()
                    End If
                Else
                    Dim id_expediente_padre As Integer = 0
                    Result = Refclas.Des_registrar_expediente_volumen(Val(Me.Hidden_eli_rel.Value),
                                                                      Me.Hidden_eli_result.Value,
                                                                      id_expediente_padre)
                    If Result <> "YES" Then
                        clas.Showscripman(Result, Me.UpdatePanel_boton_volumenes_relacionados)
                        Exit Sub
                    End If
                    Dim Existencia As String = ""
                    Result = Refclas.Verfica_existencia_expediente_padre_volumen(Val(Me.hdnEmailID.Value), _
                                                                                 Existencia)
                    If Result <> "YES" Then
                        clas.Showscripman(Result, Me.UpdatePanel_boton_volumenes_relacionados)
                        Exit Sub
                    End If
                    If Existencia = "YES" Then
                        Me.Hidden_eli_result.Value = ""
                    End If
                End If
            Else
                clas.Showscripman("Registro no seleccionado", Me.UpdatePanel_boton_volumenes_relacionados)
            End If
        Catch ex As Exception
            clas.Showscripman(ex.Message, Me.UpdatePanel_boton_volumenes_relacionados)
        End Try
    End Sub

    Protected Sub Button_activa_ventana_rel_volumen_Click(sender As Object, e As EventArgs) Handles Button_activa_ventana_rel_volumen.Click
        Dim clas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_ra_relacion_expediente
            Dim estado_resultado As String = 0
            Result = Refclas.Valida_interface_relacion_volumen(Val(Me.hdnEmailID.Value))
            If Result <> "YES" Then
                clas.Showscripman(Result, Me.UpdatePanel_botones_opcion)
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_padres_relacionados.Show()
        Catch ex As Exception
            clas.Showscripman(ex.Message, Me.UpdatePanel_botones_opcion)
        End Try
    End Sub

    Protected Sub ImageButton_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_buscar.Click
        Dim clas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            Dim estado_solo_palabra As Integer = 1
            If Me.CheckBox_optio_busq.Checked = True Then
                estado_solo_palabra = 1
            Else
                estado_solo_palabra = 0
            End If
            Result = Refclas.Solicita_expedientes_para_relacionar_volumenes(Me.TextBox_busqueda_padres.Text,
                                                                            Me.titulo_padres_relacionados,
                                                                            Me.data_grid_padres_relacionados,
                                                                            Me.Hidden_eli_rel_volumen,
                                                                            Me.UpdateGeneral_padres_relacionados,
                                                                            Me.UpdatePanel_padres_relacionados_title,
                                                                            estado_solo_palabra)
            If Result <> "YES" Then
                clas.Showscripman(Result, Me.UpdatePanel_boton_padres_relacionados)
                Exit Sub
            End If

        Catch ex As Exception
            clas.Showscripman(ex.Message, Me.UpdatePanel_boton_padres_relacionados)
        End Try
    End Sub

    Private Sub Button_active_eli_rel_padres_Click(sender As Object, e As EventArgs) Handles Button_active_eli_rel_padres.Click
        Dim clas As New Classscrripjava
        Try
            If Me.HiddenPROMP.Value = "0" Then Exit Sub
            Dim Result As String = ""
            Dim Refclas As New Class_ra_relacion_expediente
            Result = Refclas.Relacionar_como_expediente_volumen(Val(Me.Hidden_eli_rel_volumen.Value),
                                                                Val(Me.hdnEmailID.Value),
                                                                Me.Hidden_eli_result_padres.Value)
            If Result <> "YES" Then
                clas.Showscripman(Result, Me.UpdatePanel_boton_padres_relacionados)
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_padres_relacionados.Hide()
        Catch ex As Exception
            clas.Showscripman(ex.Message, Me.UpdatePanel_boton_padres_relacionados)
        End Try
    End Sub

    Private Sub Button_general_indice_expediente_Click(sender As Object, e As EventArgs) Handles Button_general_indice_expediente.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim estado_expediente_db As Integer = -1
            Dim estado_publico As Integer = 0
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para crear el indice", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_estado_expediente(Val(Me.hdnEmailID.Value), _
                                                       estado_expediente_db, _
                                                       estado_publico)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_expediente_db = 0 Then
                clasjava.Showscripman_menu("El expediente esta cerrado, imposible crear indice expediente", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
            Result = Class_ra_cert_indice_expediente.Crear_indice_expediente(Val(Me.hdnEmailID.Value), _
                                                                             1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_menu_var_event)
        End Try
    End Sub
    Private Sub Button_indice_expediente_Click(sender As Object, e As EventArgs) Handles Button_indice_expediente.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
            Dim Result As String = ""
            Dim Existencia_indice As String = ""
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para mostrar el indice", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Class_ra_cert_indice_expediente.Solicita_existencia_indice_db(Val(Me.hdnEmailID.Value),
                                                                                   Existencia_indice)
            If Existencia_indice = "NO" Then
                clasjava.Showscripman_menu("El expediente no tiene indice para mostrar", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("CERT_ID_EXPEDIENTE_INDICE") = Val(Me.hdnEmailID.Value)
            Me.Iframe_indice_.Attributes.Add("src", "../gestion/WebForm_indice_expediente.aspx")
            Me.UpdatePanel_indice.Update()
            Me.ModalPopupExtender_indice.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_pogres_show_Click(sender As Object, e As EventArgs) Handles Button_pogres_show.Click
        Me.ModalPopupExtender_edition_pro_gres_bar.Show()
    End Sub

    Private Sub Button_cerrar_pro_gres_bar_Click(sender As Object, e As EventArgs) Handles Button_cerrar_pro_gres_bar.Click
        Me.ModalPopupExtender_edition_pro_gres_bar.Hide()
    End Sub
    Private Sub Button_lista_ayuda_expediente_Click(sender As Object, e As EventArgs) Handles Button_lista_ayuda_expediente.Click
        Dim classcrip As New Classscrripjava
        Dim Result As String = ""
        Dim Resfclas As New Class_ra_tipo_expediente

        Try
            Result = Resfclas.Retorna_ayuda_clase_expediente(Me.DropDownListBoxtipoexpediente.Text,
                                                             Me.TextBoxayuda.Text)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = True
            Me.TextBoxNUMERO_DIGITALIZADO_CONTENIDO.ReadOnly = True
            Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.ReadOnly = True
            If Me.DropDownListBoxtipoexpediente.Text = "FISICO" Then
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.Text = 0
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.BackColor = Drawing.Color.White
            End If
            'EXPEDIENTE HIBRIDO
            If Me.DropDownListBoxtipoexpediente.Text = "HIBRIDO" Then
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.Text = 0
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.BackColor = Drawing.Color.White
            End If
            'EXPEDIENTE MIXTO
            If Me.DropDownListBoxtipoexpediente.Text = "MIXTO" Then
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.ReadOnly = False
                Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.Text = 0
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.Text = 0
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.BackColor = Drawing.Color.White
                Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.BackColor = Drawing.Color.White
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_selecion_organigrama_Click(sender As Object, e As EventArgs) Handles Button_selecion_organigrama.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            Result = Refclas.Seleccion_organigrama(Me.DropDownListorganigrama.Text,
                                                   Me.Hidden_id_empresa.Value,
                                                  Me.DropDownListArea,
                                                  Me.DropDownListSerie,
                                                  Me.DropDownListSubserie,
                                                  Me.DropDownList_instrumento,
                                                  Me.DropDownListNOMBRE_CICLO_ARCHIVO,
                                                  Me.Labelresultado,
                                                  Me.update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman(Result, Me.update_panel_controles)
                Exit Sub
            End If

        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_selecion_area_Click(sender As Object, e As EventArgs) Handles Button_selecion_area.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            Dim id_instrumento As Integer = 0
            If Not Me.DropDownList_instrumento.SelectedItem Is Nothing Then
                id_instrumento = Me.DropDownList_instrumento.SelectedValue
            End If
            Result = Refclas.Seleccion_area_departamento(Me.DropDownListorganigrama.SelectedItem.Text,
                                                        Me.DropDownListArea.SelectedItem.Text,
                                                        id_instrumento,
                                                        Me.DropDownListSerie,
                                                        Me.DropDownListSubserie,
                                                        Me.update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman(Result, Me.update_panel_controles)
                Exit Sub
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_selecion_serie_Click(sender As Object, e As EventArgs) Handles Button_selecion_serie.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            Dim id_istrumento As Integer = 0
            Dim nombre_serie As String = ""
            If Not Me.DropDownList_instrumento.SelectedItem Is Nothing Then
                id_istrumento = Me.DropDownList_instrumento.SelectedValue
            End If
            If Not Me.DropDownListSerie.SelectedItem Is Nothing Then
                nombre_serie = Me.DropDownListSerie.SelectedItem.Text
            End If
            Result = Refclas.Seleccion_serie_documental(Me.DropDownListorganigrama.Text,
                                                      Me.DropDownListArea.Text,
                                                      id_istrumento, nombre_serie,
                                                      Me.DropDownListSubserie,
                                                      Me.update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ButtonRestaurar_Click(sender As Object, e As EventArgs) Handles ButtonRestaurar.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAdmonEmpresa
            Dim Resfclas As New ClassGaTipoDocumental
            Dim id_empresa As Integer = 0
            Dim refe_exp As New ClassGaExpediente
            Result = refe_exp.Limpia_campos_agregar_expediente(Me.table_controles)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = refe_exp.Activa_registrar_expediente_conservacion(Me.Page)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        Finally
            Me.update_panel_controles.Update()
        End Try
    End Sub

    Private Sub Button1_seleccion_expediente_manual_Click(sender As Object, e As EventArgs) Handles Button1_seleccion_expediente_manual.Click
        If Me.CheckBoxActivaCodigomanual.Checked = True Then
            Me.TextBoxCodigoManual.ReadOnly = False
            Me.TextBoxCodigoManual.BackColor = Drawing.Color.White
        Else
            Me.TextBoxCodigoManual.ReadOnly = True
            Me.TextBoxCodigoManual.BackColor = Drawing.Color.Gray
        End If
    End Sub

    Private Sub WebFormGaGestionExpediente_vb_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        'Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 0
    End Sub

    Private Sub Button_show_printer_Click(sender As Object, e As EventArgs) Handles Button_show_printer.Click
        Try
            Me.ifimpre_post_.Attributes.Add("src", "../Gestion/WebFormimpresionfile.aspx")
            UpdatePaneliframe_post.Update()
            ModalPopupExtenderimpre_post.Show()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ImageButton_buscar_volumen_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_buscar_volumen.Click
        Dim clas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            Dim estado_solo_palabra As Integer = 1
            If Me.CheckBox_optio_busq_volumen.Checked = True Then
                estado_solo_palabra = 1
            Else
                estado_solo_palabra = 0
            End If
            Result = Refclas.Solicita_expedientes_para_relacionar_expdiente_padre(Me.TextBox_busqueda_padres_volumen.Text,
                                                                            Me.titulo_relacionar_volumen,
                                                                            Me.data_grid_relacionar_volumen,
                                                                            Me.Hidden_relacion_volumen,
                                                                            Me.UpdateGeneral_relacionar_volumen,
                                                                            Me.UpdatePanel_relacionar_volumen_title__volumen,
                                                                            estado_solo_palabra)
            If Result <> "YES" Then
                clas.Showscripman(Result, Me.UpdatePanel_boton_relacionar_volumen)
                Exit Sub
            End If

        Catch ex As Exception
            clas.Showscripman(ex.Message, Me.UpdatePanel_boton_relacionar_volumen)
        End Try
    End Sub
End Class