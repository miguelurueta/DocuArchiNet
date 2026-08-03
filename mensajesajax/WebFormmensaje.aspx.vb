Public Class WebFormmensaje
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub ServerButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ServerButton.Click
        ClientScript.RegisterStartupScript(Me.GetType(), "key", "launchModal();", True)
    End Sub

    Protected Sub ClientButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ClientButton.Click

    End Sub
End Class