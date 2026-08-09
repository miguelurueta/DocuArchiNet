Imports System.Web
Imports System.Web.Services
Imports System.IO

Public Class Handler_image_wf

    Implements System.Web.IHttpHandler, IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            Dim ruta As String = context.Request.QueryString("rut_image")
            ruta = ruta.Replace("|", "/")
            Dim files As New FileInfo(ruta)
            Dim file_ext As String = files.Extension.Replace(".", "")
            Dim Refclas As New ClassRaEnvioCorrespondencia
            context.Response.ContentType = "application/" & file_ext
            Dim content As Object = Nothing
            Dim Result = Refclas.ReadFile(ruta, content)
            If Result <> "YES" Then
                context.Response.ContentType = "text/plain"
                context.Response.Write(Result)
            Else
                If UCase(file_ext) = "PDF" Then
                    context.Response.BinaryWrite(content)
                    'context.Response.Clear()
                    'context.Response.AppendHeader("content-disposition", "attachment; filename=" & files.FullName)
                    'context.Response.WriteFile(files.FullName)
                    'context.Response.End()
                Else
                    context.Response.Clear()
                    context.Response.AppendHeader("content-disposition", "attachment; filename=" & files.FullName)
                    context.Response.WriteFile(files.FullName)
                    context.Response.End()
                End If
            End If
        Catch ex As Exception
            context.Response.Clear()
            context.Response.Write(ex.Message)
        End Try
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class