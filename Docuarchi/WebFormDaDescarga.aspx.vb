Imports System.IO
Imports Ionic.Zip
Imports System.Collections.Generic
Public Class WebFormDaDescarga
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
    End Sub
    Private Sub Button_descarga_Click(sender As Object, e As EventArgs) Handles Button_descarga.Click
        Try
            Dim ref_ruta As String = Me.Hidden_ruta_archivo.Value
            Dim split_document() As String = ref_ruta.Split(",")
            If split_document.Length = 1 Then
                Dim filinf As New IO.FileInfo(split_document(0))
                Response.AppendHeader("content-disposition", "attachment; filename=" + filinf.Name)
                Response.Clear()
                Response.WriteFile(split_document(0))
                Response.End()
            Else
                Using zip As New ZipFile()
                    For i As Integer = 0 To split_document.Length - 1
                        If i = 0 Then
                            zip.AddFile(split_document(i), "FilesDocuarchi")
                        Else
                            zip.AddFile(split_document(i), "FilesDocuarchi")
                        End If
                    Next
                    Response.Clear()
                    Response.BufferOutput = False
                    Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                    Response.ContentType = "application/zip"
                    Response.AddHeader("content-disposition", "attachment; filename=" + zipName)
                    zip.Save(Response.OutputStream)
                    Response.[End]()
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class