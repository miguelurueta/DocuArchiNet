Imports System.IO
Public Class Webform_save_digital_image
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Session.Item("WF_RUTA_ERROR_ESCANER_FILE") = ""
            If HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") <> "" Then
                If Directory.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER")) = False Then
                    Directory.CreateDirectory(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"))
                End If
                Dim ruta_stron As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER")
                For Each Archivo In My.Computer.FileSystem.GetFiles(
                    ruta_stron & "\",
                    FileIO.SearchOption.SearchTopLevelOnly, "*.*")
                    Kill(Archivo)
                Next
                Dim strImageName As String = ""
                Dim files As HttpFileCollection = HttpContext.Current.Request.Files
                Dim uploadfile As HttpPostedFile = files("RemoteFile")
                strImageName = uploadfile.FileName
                If File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") + "\" + strImageName) = True Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") + "\" + strImageName)
                End If
                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER_FILE") = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") + "\" + strImageName
                Dim RutaFileSave As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") + "\" + strImageName
                uploadfile.SaveAs(RutaFileSave)
                HttpContext.Current.Session.Item("WF_RUTA_ERROR_ESCANER_FILE") = ""
            Else
                HttpContext.Current.Session.Item("WF_RUTA_ERROR_ESCANER_FILE") = "Imposible encontar la ruta WF_RUTA_TEMPO_ESCANER (" & HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER") & ")"
            End If
        Catch ex As Exception
            HttpContext.Current.Session.Item("WF_RUTA_ERROR_ESCANER_FILE") = "Error general guardando archivo en el servidor (" & ex.Message & ")"
        End Try
    End Sub

End Class