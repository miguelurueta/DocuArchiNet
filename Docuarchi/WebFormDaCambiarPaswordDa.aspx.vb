Public Class WebFormDaCambiarPaswordDa
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Protected Sub Button_Cambiar_Click(sender As Object, e As EventArgs) Handles Button_Cambiar.Click
        Dim Result As String = ""
        Dim Refclas As New ClassDaIncioDocuarchi
        Dim refava As New Classscrripjava
        Try
            Result = Refclas.Cambiar_Contraseña_da(Me.TextBox_pasword.Text, Me.TextBox_pasword_2.Text)
            If Result <> "YES" Then
                refava.Showscripman(Result, Me.update_general)
            Else
                refava.Showscripman("Su contraseña se cambio correctamente", Me.update_general)
            End If
        Catch ex As Exception
            refava.Showscripman(ex.Message, Me.update_general)
        End Try
    End Sub
End Class