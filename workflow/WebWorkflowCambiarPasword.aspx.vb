Public Class WebWorkflowCambiarPasword
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub ButtonAceptar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonAceptar.Click
        Dim Result As String = ""
        Dim Refclas As New ClassWorkflowUsuario
        Dim RefclasJava As New Classscrripjava
        Try
            Result = Refclas.Cambiar_Contraseña_Wf(Me.TextBoxPaswuno.Text, Me.TextBoxPaswdos.Text)
            If Result <> "YES" Then
                RefclasJava.Showscripman(Result, Updatepanel_Boton)
            Else
                RefclasJava.Showscripman("Su  contraseña se cambio conrrectamente", Updatepanel_Boton)
            End If
        Catch ex As Exception
            RefclasJava.Showscripman(ex.Message, Updatepanel_Boton)
        End Try
    End Sub
End Class