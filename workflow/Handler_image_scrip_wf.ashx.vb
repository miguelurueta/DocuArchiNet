Imports System.Web
Imports System.Web.Services
Imports System.IO

Public Class Handler_image_scrip_wf
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim Ruta_imagen_huella_md5_escript As String = ""
        Try
            Dim ruta As String = HttpContext.Current.Application.Item("")
            Dim query_strin As String = context.Request.QueryString("rut_image").Replace(" ", "+")
            Dim Result = encriptacion.desc_encript_md5(query_strin, _
                                                       "7894561230!", _
                                                       Ruta_imagen_huella_md5_escript)
            If Result <> "YES" Then
                context.Response.Write(Result & Ruta_imagen_huella_md5_escript & " encript " & query_strin)
                Exit Sub
            End If
            Dim files As New FileInfo(Ruta_imagen_huella_md5_escript)
            Dim file_ext As String = files.Extension.Replace(".", "")
            Dim Refclas As New ClassRaEnvioCorrespondencia
            context.Response.ContentType = "application/" & file_ext
            Dim content As Object = Nothing
            Result = Refclas.ReadFile(Ruta_imagen_huella_md5_escript, _
                                      content)
            If Result <> "YES" Then
                context.Response.Write(Result)
            Else
                context.Response.Clear()
                context.Response.AppendHeader("content-disposition", "attachment; filename=" & files.Name)
                context.Response.WriteFile(files.FullName)
                context.Response.End()
            End If
        Catch ex As Exception
            context.Response.Write(ex.Message & "Ruta " & Ruta_imagen_huella_md5_escript)
        End Try
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class