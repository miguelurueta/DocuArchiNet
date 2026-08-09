Public Class CustomTreeNode
    Inherits TreeNode

    Public Sub New()
        MyBase.New()
    End Sub

    Private _Attributes As NameValueCollection = New NameValueCollection()
    Private _cssClass As String

    Public Property cssClass As String
        Get
            Return _cssClass
        End Get
        Set(ByVal value As String)
            _cssClass = value
        End Set
    End Property

    Public Property Attributes As NameValueCollection
        Get
            Return Me._Attributes
        End Get
        Set(ByVal value As NameValueCollection)
            Me._Attributes = value
        End Set
    End Property

    Protected Overrides Sub RenderPreText(ByVal writer As HtmlTextWriter)
        writer.AddAttribute(HtmlTextWriterAttribute.[Class], cssClass)
        writer.RenderBeginTag(HtmlTextWriterTag.Div)
        MyBase.RenderPreText(writer)
    End Sub

    Protected Overrides Sub RenderPostText(ByVal writer As HtmlTextWriter)
        writer.RenderEndTag()
        MyBase.RenderPostText(writer)
    End Sub
End Class
