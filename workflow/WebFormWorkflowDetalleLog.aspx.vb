Public Class WebFormWorkflowDetalleLog
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.TextBoxDetalle.Text = HttpContext.Current.Session.Item("WF_DETALLES_SESION")
    End Sub

End Class