Public Class Principal
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Classsesiondend.RegisterRedirectOnSessionEndScript(Me.Page)
        'HttpContext.Current.Session.Timeout = 1
        'Dim tiempoExpiracion As String = HttpContext.Current.Session.Timeout + 1
        'Dim scriptExpiraSesion As String = " var iMinute=" & tiempoExpiracion & "; function showTimer() {" & _
        '"lessMinutes();" & _
        '"}" & _
        '"function lessMinutes() {" & _
        '"iMinute-= 1;" & _
        '"if (iMinute==0) {" & _
        '"alert('Su sesión ha expirado,\nserá redireccionado a la página de acceso.');" & _
        '"window.location.href = '" & ResolveClientUrl("~/WebFormindice.aspx") & "'; }" & _
        '"window.setTimeout('lessMinutes();', 60000)}"
        'ScriptManager.RegisterStartupScript(Me.Page, Page.GetType(), "SessionExpira", scriptExpiraSesion, True)
        'If Not Page.IsPostBack Then
        'HttpContext.Current.Session.Abandon()
        'HttpContext.Current.Session.Remove(HttpContext.Current.Session.SessionID)
        'Response.AppendHeader("Refresh", (Session.Timeout * 60) + 5 & "; Url=gestor.aspx")
        'End If
    End Sub

    
   
End Class