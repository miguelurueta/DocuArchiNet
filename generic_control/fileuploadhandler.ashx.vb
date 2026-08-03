Imports System.Web
Imports System.Web.Services
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json


Public Class FileUploadHandler
    'Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext)
        If context.Request.Files.Count > 0 Then
            Dim file As HttpPostedFile = context.Request.Files(0)
            Dim resultList = New List(Of UploadFilesResult)()
            Dim path As String = context.Server.MapPath("../generic_control/uploads/")
            file.SaveAs(path & file.FileName)
            Dim uploadFiles As UploadFilesResult = New UploadFilesResult()
            uploadFiles.name = file.FileName
            uploadFiles.size = file.ContentLength
            uploadFiles.type = "image/jpeg"
            uploadFiles.url = "/Content/uploads/" & file.FileName
            uploadFiles.deleteUrl = "/FileUploadHandler.ashx?file=" & file.FileName
            uploadFiles.thumbnailUrl = "/Content/uploads/" & file.FileName
            uploadFiles.deleteType = "GET"
            resultList.Add(uploadFiles)
            Dim jFiles As JsonFiles = New JsonFiles(resultList)
            Dim jFilesJson As String = JsonConvert.SerializeObject(jFiles)
            context.Response.ContentType = "text/plain"
            context.Response.Write(jFilesJson)
        End If


    End Sub

    Public ReadOnly Property IsReusable As Boolean
        Get
            Return False
        End Get
    End Property
End Class