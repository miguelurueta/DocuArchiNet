Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports System.Web.Script.Serialization
Imports System.Drawing
Imports System.Xml
Imports System.Collections.Generic
Imports MindFusion.Diagramming
Imports MindFusion.Diagramming.WebForms
Imports MindFusion.Drawing

Public Class CustomNode
    Inherits ShapeNode
    Public Sub New()
        MyBase.New()
        m_nodeType = 1
        m_label = ""
    End Sub

    Protected Overrides Sub SaveToXml(ByVal xmlElement As XmlElement, ByVal context As XmlPersistContext)
        MyBase.SaveToXml(xmlElement, context)
        context.WriteInt(m_nodeType, "NodeType", xmlElement)
        context.WriteString(m_label, "Label", xmlElement)
    End Sub

    Protected Overrides Sub LoadFromXml(ByVal xmlElement As XmlElement, ByVal context As XmlPersistContext)
        MyBase.LoadFromXml(xmlElement, context)
        m_nodeType = context.ReadInt("NodeType", xmlElement)
        m_label = context.ReadString("Label", xmlElement)
    End Sub

    Protected Overrides Sub SaveTo(ByVal writer As System.IO.BinaryWriter, ByVal context As PersistContext)
        MyBase.SaveTo(writer, context)
        writer.Write(m_nodeType)
        writer.Write(m_label)
    End Sub

    Protected Overrides Sub LoadFrom(ByVal reader As System.IO.BinaryReader, ByVal context As PersistContext)
        MyBase.LoadFrom(reader, context)
        m_nodeType = reader.ReadInt16()
        m_label = reader.ReadString()
    End Sub


    Public Property NodeType() As Integer
        Get
            Return m_nodeType
        End Get
        Set(ByVal value As Integer)
            m_nodeType = value
        End Set
    End Property

    Public Property Label() As String
        Get
            Return m_label
        End Get
        Set(ByVal value As String)
            m_label = value
        End Set
    End Property


    Private m_nodeType As Integer
    Private m_label As String
End Class


Public Class CustomNodeConverter
    Inherits ShapeNodeConverter
    Public Sub New(ByVal view As DiagramView)
        MyBase.New(view)
    End Sub

    Public Overrides ReadOnly Property SupportedTypes() As IEnumerable(Of Type)
        Get
            Return New List(Of Type)(New Type() {GetType(CustomNode)})
        End Get
    End Property

    ''' <summary>
    ''' Deserialize override.
    ''' </summary>
    Public Overrides Function Deserialize(ByVal dictionary As IDictionary(Of String, Object), ByVal type As Type, ByVal serializer As JavaScriptSerializer) As Object
        Dim node As CustomNode = TryCast(MyBase.Deserialize(dictionary, type, serializer), CustomNode)
        node.NodeType = Integer.Parse(dictionary("nodeType").ToString())
        node.Label = dictionary("label").ToString()
        Return node
    End Function


    Public Overrides Function Serialize(ByVal obj As Object, ByVal serializer As System.Web.Script.Serialization.JavaScriptSerializer) As IDictionary(Of String, Object)
        Dim node As CustomNode = TryCast(obj, CustomNode)
        If node IsNot Nothing Then
            Dim json As IDictionary(Of String, Object) = MyBase.Serialize(obj, serializer)
            If json IsNot Nothing Then
                json.Add("nodeType", node.NodeType)
                json.Add("label", node.Label)
            End If
            Return json
        End If
        Return Nothing

    End Function
End Class