Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel.Design.Serialization
Imports System.Web.Script.Serialization
Public Class Handler_file_guardar_chkd
    Implements System.Web.IHttpHandler, IRequiresSessionState
    
    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        Try
            Dim strJson As String = New StreamReader(context.Request.InputStream).ReadToEnd()
            Dim chk_editor_class As New chk_editor
            chk_editor_class = Deserialize(Of chk_editor)(strJson)
            If chk_editor_class IsNot Nothing Then
                Dim texto As String = chk_editor_class.m_ckeditor
                Dim id_respuesta As Integer = chk_editor_class.m_id_respuesta
                Dim refclas As New Classgestionrespuesta
                Dim result As String = ""
                Dim id_imagen_guardada As Integer = 0
                result = refclas.Actualiza_guarda_documento_respuesta_chkeditor(id_respuesta, texto, id_imagen_guardada)
                If result <> "YES" Then
                    context.Response.Write(result)
                Else
                    If id_imagen_guardada = 0 Then
                        context.Response.Write("YES")
                    Else
                        context.Response.Write("Guarda_documento_nuevo_respuesta")
                    End If

                End If
            Else
                context.Response.Write("Imposible encontrar los datos de serializacion")
            End If
        Catch ex As Exception
            context.Response.Write("Error :" + ex.Message)
        End Try

    End Sub
    Public Function Deserialize(Of T)(context As String) As T
        Dim jsonData As String = context

        'cast to specified objectType
        Dim obj = DirectCast(New JavaScriptSerializer().Deserialize(Of T)(jsonData), T)
        Return obj
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
Public Class chk_editor
    Public Property chk_editor() As String
        Get
            Return m_ckeditor
        End Get
        Set(value As String)
            m_ckeditor = value
        End Set
    End Property
    Public m_ckeditor As String
    Public Property m_id_respuesta_vase() As String
        Get
            Return m_id_respuesta
        End Get
        Set(value As String)
            m_id_respuesta = value
        End Set
    End Property
    Public m_id_respuesta As String
End Class