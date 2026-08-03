Public Class WebFormRadicadoExterno
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.IsPostBack = False Then
            'Me.ifrm_ds_.Attributes.Add("SRC", "../workflow/WebFormReportesWorkflow.aspx")
        End If
    End Sub

    Private Sub WebFormWorkflowExterno_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
    End Sub

End Class