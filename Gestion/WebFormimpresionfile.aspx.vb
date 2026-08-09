Public Class WebFormimpresionfile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Me.Labelestado.Text = ""
            If Session.Item("RA_RUTA_IMPRESION_FINAL") <> "" And Session.Item("RA_RUTA_IMPRESION_FINAL") <> "OJO" Then
                Dim SES As String = Session.Item("RA_RUTA_IMPRESION_FINAL")
                Me.sid2.Value = SES

            End If
            '***************************
            '0-codigo destinatario
            '1-codigo remitente
            '2-id usuario
            '3-codigo plantilla
            '4-Consecutivo Radicado
            '5-Consecutivo codigo barra
            '***************************

        Catch ex As Exception
            Me.Labelestado.Text = ex.Message
        End Try
    End Sub

End Class