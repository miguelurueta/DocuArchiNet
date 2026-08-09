Imports System.Web
Imports System.Web.Services

Public Class Handler_file_pdf
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim Refclas As New ClassRaEnvioCorrespondencia
        context.Response.ContentType = "application/" & ".PDF"
        Dim content As Object = Nothing
        Dim ruta_imagen As String = context.Request.QueryString("rut_image")
        ruta_imagen = ruta_imagen.Replace("/", "")
        Dim Result = Refclas.ReadFile(ruta_imagen, content)
        If Result <> "YES" Then
            'context.Response.ContentType = "text/plain"
            'context.Response.Write(Result)
        Else
            context.Response.BinaryWrite(content)
            context.Response.End()
            context.Response.TransmitFile(ruta_imagen)
            context.Response.End()
        End If
       
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class