Public Class WebFormDescargaRadicado
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
       
    End Sub

    Private Sub Button_descarga_Click(sender As Object, e As EventArgs) Handles Button_descarga.Click
        Try
            Dim ref_ruta As String = Me.Hidden_ruta_archivo.Value
            If ref_ruta <> "" Then
                Dim filinf As New IO.FileInfo(Server.MapPath(ref_ruta))
                Response.AppendHeader("content-disposition", "attachment; filename=" & filinf.Name)
                Response.Clear()
                Response.WriteFile(ref_ruta)
                Response.End()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub WebFormDescargaRadicado_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
    End Sub
End Class