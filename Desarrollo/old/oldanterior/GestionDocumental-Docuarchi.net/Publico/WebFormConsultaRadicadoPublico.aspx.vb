Public Class WebFormConsultaRadicadoPublico
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If

        Dim Refclas As New ClassRaConsultaRadicados
        Dim Result As String = ""
        If Me.IsPostBack = False Then
            Result = Refclas.Retorna_plantilla_radicacion_activas_drowlist(Me.DropDownList_Pantilla)
            If Result <> "YES" Then
                Label_estado_transac.Text = Result
            End If
            ''-----------------------------------
            ''Retorna login usuario docuarchi
            ''-----------------------------------
            'HttpContext.Current.Session.Item("DA_Login_Usuario") = "CONSULTAPUBLICO"
            'Dim id_usuario_da As Integer = 0
            'Dim Refclasda As New ClassDaIncioDocuarchi
            'Dim refclasgestiondocumental As New ClassGestionDocumental
            'Result = Refclasda.Retorna_id_usuario_docuarchi(id_usuario_da, _
            '                                                HttpContext.Current.Session.Item("DA_Login_Usuario"))
            'If Result <> "YES" Then
            '    Label_estado_transac.Text = Result
            '    Exit Sub
            'End If
            ''-----------------------------------
            ''Retorna id usuario gestion
            ''-----------------------------------
            'If id_usuario_da = 0 Then
            '    Label_estado_transac.Text = "El usuario CONSULTAPUBLICO no esta creado contacte al administrador"
            '    Exit Sub
            'Else
            '    HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI") = id_usuario_da
            'End If
        End If
        'Dim refclasrad As New ClassRadicador
        'Result = refclasrad.agregar_auto_complete(Me.TextBox_radicado.ID, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", Me.DropDownList_Pantilla.Text, "Consecutivo_Rad")
        'If Result <> "YES" Then
        '    Label_estado_transac.Text = Result
        'End If
    End Sub
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim id_plantilla As Integer = 0
            Dim Refclas As New ClassRadicador
            Dim Ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim Result = Ref_Class_system_plantilla_radicado.SolicitaIdPlantillaRadicado(id_plantilla, _
                                                                                            Me.DropDownList_Pantilla.Text)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value & "|" & id_plantilla
            'Label10.Text = ""
            Me.Iframe_imagen_respuesta_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_imagen_respuesta.Update()
            Me.ModalPopupExtender_imagen_respuesta.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_botones_radicacion)
            Exit Sub
        End Try
    End Sub
    Protected Sub Button_consulta_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_consulta_val_radicacion.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim id_plantilla As Integer = 0
        Dim classcripjava As New Classscrripjava
        Try
            Hidden_resultado_consulta.Value = ""
            If Me.DropDownList_Pantilla.Text = "" Then
                classcripjava.Showscripman("Debe seleccionar la plantilla de radicación", Me.UpdatePanel_botones_validacion)
                Exit Sub
            End If
            Dim Ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result = Ref_Class_system_plantilla_radicado.SolicitaIdPlantillaRadicado(id_plantilla, _
                                                                                       Me.DropDownList_Pantilla.Text)
            If Result <> "YES" Then
                classcripjava.Showscripman(Result, Me.UpdatePanel_botones_validacion)
                Exit Sub
            End If
            HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO") = id_plantilla & "|" & "|" & "RADICACION ENTRANTE"
            Session.Item("SortExpression_publico") = "CONSECUTIVO_RADICADO"
            Session.Item("SortDirection_publico") = "DESC"
            Result = Refclas.Genera_Sql_Consulta_radicados_publicos(Me.Page, id_plantilla, _
                                                                    Me.DropDownList_Pantilla.Text, _
                                                                    Me.TextBox_radicado.Text, _
                                                                    1, _
                                                                    "", _
                                                                     Session.Item("SortExpression_publico"), _
                                                                     Session.Item("SortDirection_publico"))
            If Result <> "YES" Then
                classcripjava.Showscripman(Result, Me.UpdatePanel_botones_validacion)
                Exit Sub
            Else
                Hidden_resultado_consulta.Value = "YES"
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                UpdatePanelabel_val_radicacion.Update()
            End If
            HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO") = ""
        Catch ex As Exception
            classcripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_validacion)
        End Try
    End Sub
    Private Sub Button_Trazabilidad_Click(sender As Object, e As EventArgs) Handles Button_Trazabilidad.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman("Debe seleccionar el radicado", UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Session.Item("PU_TRAZABILIDAD") = Me.hdnEmailID_VAL.Value
            Me.Iframe_trazabilidad_.Attributes("SRC") = "../workflow/WebFormTrazabilidadWorkflow.aspx"
            Me.UpdatePanel_trazabilidad.Update()
            Me.ModalPopupExtender_trazabilidad.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_botones_radicacion)
            Exit Sub
        End Try
    End Sub
    Protected Sub Button_Log_respuesta_Click(sender As Object, e As EventArgs) Handles Button_Log_respuesta.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman("Debe seleccionar el radicado", UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Dim id_respuesta_radicado As Integer = -1
            Dim Refclas As New Classgestionrespuesta
            Dim Result As String = ""
            Result = Refclas.Reorna_id_respuesta_radicado(Me.hdnEmailID_VAL.Value, id_respuesta_radicado)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            If id_respuesta_radicado = -1 Then
                clasjava.Showscripman("El tipo de radicado no requiere de una respuesta, no se realizaron transacciones para mostrar", UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Session.Item("PU_TRAZABILIDAD") = id_respuesta_radicado
            Me.Iframe_transacciones_.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
            Me.UpdatePanel_transacciones.Update()
            Me.ModalPopupExtender_transacciones.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_botones_radicacion)
            Exit Sub
        End Try
    End Sub
    Private Sub Button_detalle_radicado_Click(sender As Object, e As EventArgs) Handles Button_detalle_radicado.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman("Debe seleccionar el radicado", UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Dim id_respuesta_radicado As Integer = -1
            Dim Refclas As New Classgestionrespuesta
            Dim Result As String = ""
            Result = Refclas.Reorna_id_respuesta_radicado(Me.hdnEmailID_VAL.Value, id_respuesta_radicado)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            If id_respuesta_radicado = -1 Then
                clasjava.Showscripman("El tipo de radicado no requiere de una respuesta, no hay detalles para mostrar", UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            'Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value & "|" & id_plantilla
            Session.Item("PU_TRAZABILIDAD") = Me.hdnEmailID_VAL.Value
            Me.Iframe_transacciones_.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
            Me.UpdatePanel_transacciones.Update()
            Me.ModalPopupExtender_transacciones.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_botones_radicacion)
            Exit Sub
        End Try
    End Sub
    Private Sub GridView_val_radicacion_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_val_radicacion.PageIndexChanging
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim classcripjava As New Classscrripjava
        Try
            Result = Refclas.Genera_Sql_Consulta_radicados_publicos(Me.Page, 0, _
                                                                    Me.DropDownList_Pantilla.Text, _
                                                                    Me.TextBox_radicado.Text, _
                                                                    1, _
                                                                    Session.Item("GA_DATO_CONSULTA_PUBLICO"), _
                                                                     Session.Item("SortExpression_publico"), _
                                                                     Session.Item("SortDirection_publico"))
            If Result <> "YES" Then
                classcripjava.Showscripman(Result, Me.UpdatePanel_conenido_grid_val_radicacion)
                Exit Sub
            Else
                Hidden_resultado_consulta.Value = "YES"
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                UpdatePanelabel_val_radicacion.Update()
            End If
        Catch ex As Exception
            classcripjava.Showscripman(ex.Message, Me.UpdatePanel_conenido_grid_val_radicacion)
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
    Private Sub GridView_val_radicacion_Sorting(sender As Object, e As GridViewSortEventArgs) Handles GridView_val_radicacion.Sorting
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim classcripjava As New Classscrripjava
        Try
            Session.Item("SortExpression_publico") = e.SortExpression
            If Session.Item("SortDirection_publico") = "DESC" Then
                Session.Item("SortDirection_publico") = "ASC"
            Else
                Session.Item("SortDirection_publico") = "DESC"
            End If
            Result = Refclas.Genera_Sql_Consulta_radicados_publicos(Me.Page, 0, _
                                                                   Me.DropDownList_Pantilla.Text, _
                                                                   Me.TextBox_radicado.Text, _
                                                                   1, _
                                                                   Session.Item("GA_DATO_CONSULTA_PUBLICO"), _
                                                                    Session.Item("SortExpression_publico"), _
                                                                    Session.Item("SortDirection_publico"))
            If Result <> "YES" Then
                classcripjava.Showscripman(Result, Me.UpdatePanel_conenido_grid_val_radicacion)
                Exit Sub
            Else
                Hidden_resultado_consulta.Value = "YES"
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                UpdatePanelabel_val_radicacion.Update()
            End If
        Catch ex As Exception
            classcripjava.Showscripman(ex.Message, Me.UpdatePanel_conenido_grid_val_radicacion)
        End Try
    End Sub
End Class