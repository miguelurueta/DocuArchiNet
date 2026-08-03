Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Xml
Imports System.Web.Script.Serialization
Imports MindFusion.Diagramming
Imports MindFusion.Diagramming.WebForms
Imports MindFusion.Drawing


Public Class IconLink
    Inherits DiagramLink
    Public Sub New(ByVal diagram As Diagram)
        MyBase.New(diagram)
        m_midPointShape = PointShape.Circle
    End Sub

    Public Sub New(ByVal prototype As IconLink, ByVal src As DiagramNode, ByVal dest As DiagramNode)
        MyBase.New(prototype, src, dest)
        m_midPointShape = prototype.midPointShape
    End Sub

    Public Overrides Function Clone(ByVal clipboard As Boolean) As DiagramItem
        Return New IconLink(Me, GetDummyNode(), GetDummyNode())
    End Function

    Friend Function GetDummyNode() As DummyNode
        If Parent IsNot Nothing Then
            Return Parent.Dummy
        End If

        Return New DummyNode(Nothing)
    End Function

    Public Overrides Sub DrawShadow(ByVal graphics As IGraphics, ByVal options As RenderOptions)
    End Sub

    Public Overrides Sub Draw(ByVal graphics As IGraphics, ByVal options As RenderOptions)

        Dim pen As New System.Drawing.Pen(Color.Black, 0.1F)
        Dim brush As System.Drawing.Brush = New System.Drawing.SolidBrush(Color.Black)

        Dim cp As PointCollection = Me.ControlPoints
        Dim path As New GraphicsPath()

        path.AddLine(cp(0), cp(1))

        Dim p0 As New PointF((cp(0).X + cp(1).X) / 2, (cp(0).Y + cp(1).Y) / 2)
        Dim p1 As New PointF((cp(0).X + p0.X) / 2, (cp(0).Y + p0.Y) / 2)
        Dim p2 As New PointF((p0.X + cp(1).X) / 2, (p0.Y + cp(1).Y) / 2)

        Select Case m_midPointShape
            Case PointShape.Circle
                If True Then
                    brush = New System.Drawing.SolidBrush(Color.CornflowerBlue)
                    path.AddEllipse(p0.X - 1, p0.Y - 1, 2, 2)
                    path.AddEllipse(p1.X - 1, p1.Y - 1, 2, 2)
                    path.AddEllipse(p2.X - 1, p2.Y - 1, 2, 2)
                    Exit Select
                End If
            Case PointShape.Rectangle
                If True Then
                    brush = New System.Drawing.SolidBrush(Color.PaleGoldenrod)
                    path.AddRectangle(New RectangleF(p0.X - 1, p0.Y - 1, 2, 2))
                    path.AddRectangle(New RectangleF(p1.X - 1, p1.Y - 1, 2, 2))
                    path.AddRectangle(New RectangleF(p2.X - 1, p2.Y - 1, 2, 2))
                    Exit Select
                End If
        End Select

        graphics.DrawPath(pen, path)
        graphics.FillPath(brush, path)
        path.Dispose()
    End Sub

    Protected Overrides Sub SaveToXml(ByVal xmlElement As XmlElement, ByVal context As XmlPersistContext)
        MyBase.SaveToXml(xmlElement, context)
        context.WriteEnum(m_midPointShape, "MidPointShape", xmlElement)
    End Sub

    Protected Overrides Sub LoadFromXml(ByVal xmlElement As XmlElement, ByVal context As XmlPersistContext)
        MyBase.LoadFromXml(xmlElement, context)
        m_midPointShape = CType(context.ReadEnum("MidPointShape", xmlElement), PointShape)
    End Sub

    Protected Overrides Sub SaveTo(ByVal writer As System.IO.BinaryWriter, ByVal context As PersistContext)
        MyBase.SaveTo(writer, context)
        writer.Write(CInt(m_midPointShape))
    End Sub

    Protected Overrides Sub LoadFrom(ByVal reader As System.IO.BinaryReader, ByVal context As PersistContext)
        MyBase.LoadFrom(reader, context)
        m_midPointShape = CType(reader.ReadInt32(), PointShape)
    End Sub

    Public Property MidPointShape() As PointShape
        Get
            Return m_midPointShape
        End Get
        Set(ByVal value As PointShape)
            m_midPointShape = value
        End Set
    End Property

    Private m_midPointShape As PointShape
    Public Enum PointShape
        Circle
        Rectangle
    End Enum
End Class

Public Class IconLinkConverter
    Inherits DiagramLinkConverter
    Public Sub New(ByVal view As DiagramView)
        MyBase.New(view, True)
    End Sub


    ''' <summary>
    ''' SupportedTypes override.
    ''' </summary>
    Public Overrides ReadOnly Property SupportedTypes() As IEnumerable(Of Type)
        Get
            Return New List(Of Type)(New Type() {GetType(IconLink)})
        End Get
    End Property

    ''' <summary>
    ''' Deserialize override.
    ''' </summary>
    Public Overrides Function Deserialize(ByVal dictionary As IDictionary(Of String, Object), ByVal type As Type, ByVal serializer As JavaScriptSerializer) As Object
        Dim link As IconLink = TryCast(MyBase.Deserialize(dictionary, type, serializer), IconLink)
        link.MidPointShape = CType(dictionary("midPointShape"), IconLink.PointShape)
        Return link
    End Function


    Public Overrides Function Serialize(ByVal obj As Object, ByVal serializer As System.Web.Script.Serialization.JavaScriptSerializer) As IDictionary(Of String, Object)
        Dim link As IconLink = TryCast(obj, IconLink)
        If link IsNot Nothing Then
            Dim json As IDictionary(Of String, Object) = MyBase.Serialize(obj, serializer)
            If json IsNot Nothing Then
                json.Add("midPointShape", link.MidPointShape)
            End If
            Return json
        End If
        Return Nothing

    End Function
End Class