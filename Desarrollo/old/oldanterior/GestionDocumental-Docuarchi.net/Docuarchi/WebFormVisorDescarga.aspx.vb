Public Class WebFormVisorDescarga
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
    End Sub
    Protected Sub ImageButton_descarga_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_descarga.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.ifmExcel_.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & Session.Item("DA_DESCARGA")
            updatapanel_iframe.Update()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_Descarga)
        End Try

    End Sub
End Class