Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports System.Xml.Serialization
Imports System.ComponentModel.Design.Serialization
Imports System.Web.Script.Serialization

Public Class Handler_visualiza_imagen
    Implements System.Web.IHttpHandler, IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        'context.Response.ContentType = "text/plain"
        'context.Response.Write("¡Hola a todos!")
        Dim strJson As Object = context.Request
        Dim controles As New controles
        controles = Deserialize(Of controles)(strJson)
        If controles IsNot Nothing Then
            controles.m_modal_popup.Show()
        End If

    End Sub
    Public Function Deserialize(Of T)(context As Object) As T
        Dim jsonData As Object = context

        'cast to specified objectType
        Dim obj = DirectCast(New JavaScriptSerializer().Deserialize(Of T)(jsonData), T)
        Return obj
    End Function
    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
    Public Class controles
        Public m_update_panel As UpdatePanel
        Public Property update_panel() As UpdatePanel
            Get
                Return m_update_panel
            End Get
            Set(value As UpdatePanel)
                'm_update_panel = value
            End Set
        End Property

        Public m_modal_popup As AjaxControlToolkit.ModalPopupExtender
        Public Property modal_popup() As AjaxControlToolkit.ModalPopupExtender
            Get
                Return m_modal_popup
            End Get
            Set(value As AjaxControlToolkit.ModalPopupExtender)
                'm_update_panel = value
            End Set
        End Property

    End Class
End Class