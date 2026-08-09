Public Class WebFormRecuperarTarea
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim refclas As New Class_DAT_ADIC_TAR
        Dim Result As String = ""
        Result = refclas.Genera_interface_recuperar_rarea_workflow(Me.Page)
        If Result <> "YES" Then

        End If 
    End Sub

    Private Sub WebFormRecuperarTarea_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender  
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().cligred();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)

        End If

    End Sub
    
    Private Sub GridViewlista_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(4).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub btnOkay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOkay.Click
        Dim split() As String = Me.hdnConsult.Value.Split("|")
        If split.Length > 1 Then
            Dim Tex As TextBox = Page.FindControl(split(0))
            Tex.Text = Me.Droupdatos.Text
            Me.hdnConsult.Value = "0"
            Dim updatpane As UpdatePanel = Page.FindControl("Recupera")
            updatpane.Update()
            Me.ModalPopupTexto.Hide()

        End If
    End Sub

    Private Sub ModalPopupTexto_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles ModalPopupTexto.Load
        Try
            Dim split() As String = Me.hdnConsult.Value.Split("|")
            If split.Length > 1 Then
                btnOkay.Attributes.Add("onClick", "asignadatos('" + split(0) + "|" + Me.Droupdatos.Text + "')")
                Dim Ref As New ClassWorkflow
                Dim Result = Ref.Listar_Posibles_Datos(split(0), _
                                                       split(1), _
                                                       Me.Droupdatos)
            End If
        Catch ex As Exception

        End Try
       

    End Sub

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.hdnConsult.Value = "0"
    End Sub

    Private Sub Button5_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button5.Click
        Me.ModalPopupTexto.Show()
    End Sub

    Private Sub btnOkay_Command(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles btnOkay.Command

    End Sub

    Private Sub UpdateGeneral_Load(sender As Object, e As EventArgs) Handles UpdateGeneral.Load
        'Dim script As [String] = "$(document).ready(function(){$('#" + GridViewlista.ClientID & "').Scrollable({ScrollHeight: 270,IsInUpdatePanel:false});});"
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", GridViewlista.ClientID), script, True)
    End Sub

   
    Private Sub GridViewlista_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewlista.RowDataBound
        
    End Sub

    Private Sub Button_buscar_Click(sender As Object, e As EventArgs) Handles Button_buscar.Click
        Me.ModalPopupExtenderbusqueda.Show()
    End Sub

    Private Sub Buttond_Filtro_Click(sender As Object, e As EventArgs) Handles Buttond_Filtro.Click

    End Sub
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        Try
            Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value
            Me.Iframe_visor_externo_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_visor_externo.Update()
            Me.ModalPopupExtender_visor_externo.Show()
        Catch ex As Exception

        End Try   
    End Sub
    Protected Sub ButtonRecuperar_Click(sender As Object, e As EventArgs) Handles ButtonRecuperar.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Refclas As New ClassWorkflow
            Dim Result As String = ""
            Me.Hidden_resultado_reasigna.Value = ""
            If HttpContext.Current.Session.Item("TIPOACTIVIDADWF") <> "ENLASE" Then
                If HttpContext.Current.Session.Item("RECUPERAR_TAREA") = 0 Then
                    Mens.Showscripman("El usuario no tiene permiso para recuperar tarea", Me.Updatepanel_botones)
                    Exit Sub
                End If
            End If
            If Me.hdnEmailID.Value = "0" Then
                Mens.Showscripman("Por favor seleccione el registro", Me.Updatepanel_botones)
                Exit Sub
            Else
                Dim spli() As String = Me.hdnEmailID.Value.Split("-")
                Dim id_actividad As Integer = 0
                Result = Refclas.Retorna_id_area_id_tarea_recuperada(Val(spli(0)), _
                                                                     id_actividad)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.Updatepanel_botones)
                    Exit Sub
                End If
                Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
                Dim estado_activida_modulo_rad As Integer = 0
                Result = Class_estados_tarea_workflow.Solicita_estado_gestion_radicado_tarea_workflow(Val(spli(0)),
                                                                                                      estado_activida_modulo_rad)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.Updatepanel_botones)
                    Exit Sub
                End If
                If estado_activida_modulo_rad = 1 Then
                    Mens.Showscripman("La tarea se encuentra en estado de gestión de radicado, imposible recuperar", Me.Updatepanel_botones)
                    Exit Sub
                End If
                Me.hdnEmailID.Value = Val(spli(0)) & "-" & id_actividad
                Me.UpdatePanel_hiden.Update()
                Me.Hidden_resultado_reasigna.Value = "YES"
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.Updatepanel_botones)
        End Try    
    End Sub

    Protected Sub ButtonRecuperarReasignar_Click(sender As Object, e As EventArgs) Handles ButtonRecuperarReasignar.Click
        Dim Mens As New Classscrripjava
        Try
            Me.Hidden_resultado_reasigna.Value = ""
            Dim Refclas As New ClassWorkflow
            Dim Result As String = ""
            If Me.hdnEmailID.Value = "0" Then
                Mens.Showscripman("Por favor seleccione el registro", Me.Updatepanel_botones)
                Exit Sub
            End If
            Dim spli() As String = Me.hdnEmailID.Value.Split("-")
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Val(spli(0)), _
                                                                                 Radicado)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.Updatepanel_botones)
                Exit Sub
            End If
            If Radicado = "" Then
                Mens.Showscripman("Imposible encontrar radicado para la tarea que desea recuperar ", Me.Updatepanel_botones)
                Exit Sub
            End If
            Dim id_actividad As Integer = 0
            Result = Refclas.Retorna_id_area_id_tarea_recuperada(Val(spli(0)), id_actividad)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.Updatepanel_botones)
                Exit Sub
            End If
            Me.hdnEmailID.Value = Val(spli(0)) & "-" & id_actividad
            Me.UpdatePanel_hiden.Update()
            If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW") = 0 Then
                Me.TextBox_login_autoriza_reasignacion_tarea.Text = ""
                Me.TextBox_pasw_autoriza_reasignacion_tarea.Text = ""
                Me.UpdatePanel_autoriza_reasignacion_tarea.Update()
                Me.ModalPopupExtender_edition_autoriza_reasignacion_tarea.Show()
                Exit Sub
            Else
                Me.Hidden_resultado_reasigna.Value = "YES"
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.Updatepanel_botones)
        End Try
    End Sub

    Private Sub Button_autoriza_reasignacion_Click(sender As Object, e As EventArgs) Handles Button_autoriza_reasignacion.Click
        Dim refclas_gestion As New Classgestionrespuesta
        Dim id_usuario_autoriza As Integer = -1
        Dim Mens As New Classscrripjava
        Dim Result As String = ""
        Try
            Me.Hidden_resultado_reasigna_acpetacion.Value = ""
            Result = refclas_gestion.Valida_usuario_administrador_general(Me.TextBox_login_autoriza_reasignacion_tarea.Text, _
                                                                          Me.TextBox_pasw_autoriza_reasignacion_tarea.Text, _
                                                                          id_usuario_autoriza, _
                                                                          "reasigna_documento")
            If Result <> "YES" Then
                Mens.Showscripman(Result, UpdatePanel_autoriza_reasignacion_tarea)
                Exit Sub
            Else
                Me.Hidden_usuario_autoriza.Value = Me.TextBox_login_autoriza_reasignacion_tarea.Text
                Me.Hidden_usuario_autoriza_id.Value = id_usuario_autoriza
                Me.Hidden_resultado_reasigna_acpetacion.Value = "YES"
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_autoriza_reasignacion_tarea)
        End Try
    End Sub

    Protected Sub Button_consulta_envios_workflow_Click(sender As Object, e As EventArgs) Handles Button_consulta_envios_workflow.Click
        Dim classjava As New Classscrripjava
        Try
            Dim Refclas_dat_adic As New Class_DAT_ADIC_TAR
            Dim Result As String = ""
            Result = Refclas_dat_adic.Consulta_Datos_Tareas_estudiadas(Me.Page)
            If Result <> "YES" Then
                classjava.Showscripman(Result, Me.UpdateGenera_botones_consulta)
            End If
        Catch ex As Exception
            classjava.Showscripman(ex.Message, Me.UpdateGenera_botones_consulta)
        End Try
    End Sub
End Class