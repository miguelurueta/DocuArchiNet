Public Class WebFormContendorPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then
            Me.Hidden_id_tarea_selecionada.Value = "YES"
        Else
            Me.Hidden_estado_actualizacion.Value = "YES"
        End If
    End Sub

    Private Sub WebFormContendorPage_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
       
    End Sub
End Class