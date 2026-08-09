Public Class CustomTreeNodeTable
    Inherits TreeNode

    Public Sub New()
        MyBase.New()

    End Sub

    Public _cssClass As String
    Public _valores_table() As String
    Public _valores_table_tr() As String
    Public Property valores_table As String()
        Get
            Return _valores_table
        End Get
        Set(ByVal value As String())
            _valores_table = value
        End Set
    End Property
    Public Property valores_table_tr As String()
        Get
            Return _valores_table_tr
        End Get
        Set(ByVal value As String())
            _valores_table_tr = value
        End Set
    End Property
    Public Property cssClass As String
        Get
            Return _cssClass
        End Get
        Set(ByVal value As String)
            _cssClass = value
        End Set
    End Property

   

    Protected Overrides Sub RenderPreText(ByVal writer As HtmlTextWriter)
        writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%")
        writer.RenderBeginTag(HtmlTextWriterTag.Table)
        writer.AddAttribute(HtmlTextWriterAttribute.[Class], cssClass)
        writer.RenderBeginTag(HtmlTextWriterTag.Tr)
        For i As Integer = 0 To _valores_table.Length - 1
            writer.RenderBeginTag(HtmlTextWriterTag.Td)
            writer.Write(_valores_table(i))
            writer.RenderEndTag()
        Next
        writer.RenderEndTag()
        writer.AddAttribute(HtmlTextWriterAttribute.[Class], "table_tre_row")
        writer.AddAttribute("id", _valores_table_tr(0))
        writer.RenderBeginTag(HtmlTextWriterTag.Tr)
        For i As Integer = 1 To _valores_table_tr.Length - 1
            writer.RenderBeginTag(HtmlTextWriterTag.Td)
            writer.Write(_valores_table_tr(i))
            writer.RenderEndTag()
        Next
        writer.RenderEndTag()
        writer.RenderEndTag()
        'writer.RenderEndTag()
        MyBase.RenderPreText(writer)
    End Sub

    Protected Overrides Sub RenderPostText(ByVal writer As HtmlTextWriter)
        'writer.RenderEndTag()
        MyBase.RenderPostText(writer)
    End Sub

    'Protected Overrides Sub LoadViewState(ByVal state As Object)
    '    If state IsNot Nothing Then
    '        Dim arStates() As Object = state
    '        _cssClass = arStates(1)
    '        _valores_table = arStates(2)
    '        _valores_table_tr = arStates(3)
    '        MyBase.LoadViewState(arStates(0))
    '    End If
    'End Sub

    'Protected Overrides Function SaveViewState() As Object
    '    Dim arStates(4) As Object
    '    arStates(0) = MyBase.SaveViewState()
    '    arStates(1) = _cssClass
    '    arStates(2) = _valores_table
    '    arStates(3) = _valores_table_tr
    '    Return arStates
    'End Function
    'Protected Overrides Sub LoadViewState(ByVal savedState As Object)
    '    If Not savedState Is Nothing Then
    '        Dim myState() As Object = savedState
    '        MyBase.LoadViewState(myState(0))
    '    End If
    'End Sub

    'Protected Overrides Function SaveViewState() As Object
    '    Dim baseState As Object = MyBase.SaveViewState()
    '    Dim allStates(0) As Object
    '    allStates(0) = baseState
    '    Return allStates
    'End Function

End Class
