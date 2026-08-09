Public Class WebFormGaVisorDocumentoExterno
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Page.IsPostBack = False Then
            Dim spli_sel() As String = Session.Item("CC_TAGSELECCION_EMERGENTE").ToString.Split("|")
            Me.ifrm_visor_.Attributes("src") = "../workflow/Handler_image_wf.ashx?rut_image=" & Session.Item("CC_SESIONITERCAMBIOVISOR")
            'Me.UpdatePanelvisor.Update()
        End If
        
    End Sub
   
End Class