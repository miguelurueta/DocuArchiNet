Public Class WebFormListadoConsultaRue
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If IsPostBack = False Then
            HiddenParramRue.Value = Request.Form("param")
            HiddenCamaraRue.Value = Request.Form("codigoCamara")
            Dim ClientIP, Forwaded, RealIP
            RealIP = ""
            ClientIP = Request.ServerVariables("REMOTE_ADDR")
            ClientIP = ClientIP & "-" & Request.ServerVariables("REMOTE_HOST")
            If ClientIP <> "" Then
                Session.Item("ip_host_name") = ClientIP
            Else
                Forwaded = Request.ServerVariables("HTTP_X-Forwarded-For")
                If Forwaded <> "" Then
                    Session.Item("ip_host_name") = Forwaded
                Else
                    Session.Item("ip_host_name") = "Imposible encontrar ip host"
                End If
            End If
        End If
    End Sub

End Class