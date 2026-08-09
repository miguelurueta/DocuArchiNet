Public Class WebFormAjuntar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub ButtonAceptar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAceptar.Click
        If (FileUpload1.HasFile) Then
            Dim refclas As New ClassNeodynamic
            Dim Matri_Doc() As String
            Erase Matri_Doc
            Dim CLAS As New Classscrripjava
            Dim clasñade As New ClassAñadirDocumento
            If HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") = "" Then
                CLAS.Show("Usuario sin ruta temporal ")
                Exit Sub
            End If
            Dim Result As String = ""
            Result = refclas.Extraer_Documento_de_Multitif(Me, FileUpload1.FileBytes, Matri_Doc, HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL"))
            If Result <> "YES" Then
                CLAS.Show("Imposible extraer documento Multif " & Result)
                Exit Sub
            End If
            If Matri_Doc Is Nothing Then
                CLAS.Show("La matriz de multi tif esta nothing ")
                Exit Sub
            End If
            Dim document As String = Matri_Doc(0)
            If HttpContext.Current.Session.Item("ESTADOFILESERVER") = 1 Then
                Result = clasñade.Añadir_Documentos_tif(HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"), _
                                           HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"), document, Matri_Doc)
                If Result <> "YES" Then
                    CLAS.Show("Imposible añadir el documento " & Result)
                    Exit Sub
                End If
            Else

            End If
        Else
            ' Notify the user that a file was not uploaded.
            'UploadStatusLabel.Text = "You did not specify a file to upload."
        End If
        Session.Item("ESTADOINTERCAMBIO") = 1
    End Sub
End Class