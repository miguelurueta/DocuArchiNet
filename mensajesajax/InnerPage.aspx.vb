Public Class InnerPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Text.Text = HttpContext.Current.Session.Item("MESAJEJAVA")
        Me.btnOkay2.Attributes.Add("onClick", "cancel();")
        Me.Cancelar.Attributes.Add("onClick", "cancel();")
        'Me.Updatemensaj.Update()
    End Sub

    Protected Sub btnOkay2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOkay2.Click
        Session.Item("MESAJEJAVA") = "1"
    End Sub

    Protected Sub Cancelar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Cancelar.Click
        Session.Item("MESAJEJAVA") = "0"
    End Sub

    Private Sub InnerPage_LoadComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete

    End Sub
End Class