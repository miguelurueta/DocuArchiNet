Imports System.Collections.Generic
Imports System.Drawing
Imports System.Xml
Imports System.Web.Script.Serialization
Imports MindFusion.Diagramming
Imports MindFusion.Diagramming.WebForms
Imports MindFusion.Drawing

Imports Bitmap = System.Drawing.Bitmap
Imports Graphics = System.Drawing.Graphics
Imports Image = System.Drawing.Image

Public Class IconNode
    Inherits ShapeNode
    Shared Sub New()
        defaultIcon = New Bitmap(48, 48)

        Dim graphics__1 As Graphics = Graphics.FromImage(defaultIcon)
        Dim font As New Font("Arial", 48, FontStyle.Bold, GraphicsUnit.Pixel)

        graphics__1.FillRectangle(Brushes.White, 0, 0, 48, 48)
        graphics__1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
        graphics__1.DrawString("?", font, Brushes.ForestGreen, 0, 0)

        graphics__1.Dispose()
    End Sub

    Public Sub New(ByVal prototype As IconNode)
        MyBase.New(prototype)
        m_icon = prototype.Icon
        m_label = prototype.Label

        format = New StringFormat()
        format.Alignment = StringAlignment.Center
        format.LineAlignment = StringAlignment.Center

        Me.Font = New Font("Arial", 3, FontStyle.Bold, GraphicsUnit.Pixel)
        Bounds = prototype.Bounds
    End Sub

    Public Sub New(ByVal diagram As Diagram)
        MyBase.New(diagram)
        m_icon = defaultIcon
        m_label = "label"

        format = New StringFormat()
        format.Alignment = StringAlignment.Center
        format.LineAlignment = StringAlignment.Center

        Me.Font = New Font("Arial", 3, FontStyle.Bold, GraphicsUnit.Pixel)
        Bounds = New RectangleF(Bounds.Location, CalculateSize())
    End Sub

    Public Overrides Sub DrawShadow(ByVal graphics As IGraphics, ByVal options As RenderOptions)
    End Sub

    Public Overrides Sub Draw(ByVal graphics As IGraphics, ByVal options As RenderOptions)
        Dim iconSizePixels As New Rectangle(0, 0, m_icon.Width, m_icon.Height)
        Dim imageSize As RectangleF = MindFusion.Utilities.DeviceToDoc(graphics, iconSizePixels)

        ' Draw the icon center at the top
        TryCast(m_icon, Bitmap).MakeTransparent(TryCast(m_icon, Bitmap).GetPixel(0, 0))
        graphics.DrawImage(m_icon, Bounds.X + Bounds.Width / 2 - imageSize.Width / 2, Bounds.Y)

        ' Draw the label at the bottom
        Dim labelBounds As RectangleF = RectangleF.FromLTRB(Bounds.X, Bounds.Y + imageSize.Height, Bounds.Right, Bounds.Bottom)

        graphics.DrawString(m_label, Font, Brushes.Black, labelBounds, format)
    End Sub

    Private Function CalculateSize() As SizeF
        Dim tempImage As New Bitmap(1, 1)
        Dim graphics__1 As Graphics = Graphics.FromImage(tempImage)
        Dim measureGraphics As IGraphics = New GdiGraphics(graphics__1)

        Parent.MeasureUnit.ApplyTo(measureGraphics)
        Dim iconSizePixels As New Rectangle(0, 0, m_icon.Width, m_icon.Height)
        Dim imageSize As RectangleF = MindFusion.Utilities.DeviceToDoc(measureGraphics, iconSizePixels)

        measureGraphics.Dispose()
        tempImage.Dispose()

        Dim textSize As SizeF = Parent.MeasureString(m_label, Me.Font, Integer.MaxValue, format)

        Return New SizeF(Math.Max(imageSize.Width, textSize.Width), imageSize.Height + textSize.Height)
    End Function


    Protected Overrides Sub UpdateCreate(ByVal current As PointF)
        MyBase.UpdateCreate(current)
        Bounds = New RectangleF(current, CalculateSize())
    End Sub

    Protected Overrides Sub SaveToXml(ByVal xmlElement As XmlElement, ByVal context As XmlPersistContext)
        MyBase.SaveToXml(xmlElement, context)
        context.WriteString(m_label, "Label", xmlElement)
        context.WriteImage(m_icon, "Image", xmlElement)
    End Sub

    Protected Overrides Sub LoadFromXml(ByVal xmlElement As XmlElement, ByVal context As XmlPersistContext)
        MyBase.LoadFromXml(xmlElement, context)
        m_label = context.ReadString("Label", xmlElement)
        m_icon = context.ReadImage("Image", xmlElement)
    End Sub

    Protected Overrides Sub SaveTo(ByVal writer As System.IO.BinaryWriter, ByVal context As PersistContext)
        MyBase.SaveTo(writer, context)
        writer.Write(m_label)
        context.SaveImage(m_icon)
    End Sub

    Protected Overrides Sub LoadFrom(ByVal reader As System.IO.BinaryReader, ByVal context As PersistContext)
        MyBase.LoadFrom(reader, context)
        m_label = reader.ReadString()
        m_icon = context.LoadImage()
    End Sub


    Public Property Icon() As Image
        Get
            Return m_icon
        End Get
        Set(ByVal value As Image)
            m_icon = value
            Bounds = New RectangleF(Bounds.Location, CalculateSize())
        End Set
    End Property

    Public Property Label() As String
        Get
            Return m_label
        End Get
        Set(ByVal value As String)
            m_label = value
            Bounds = New RectangleF(Bounds.Location, CalculateSize())
        End Set
    End Property


    Private m_icon As Image
    Private m_label As String
    Private format As StringFormat
    Private Shared defaultIcon As Image
End Class

Public Class IconNodeConverter
    Inherits ShapeNodeConverter
    Public Sub New(ByVal view As DiagramView)
        MyBase.New(view, True)
    End Sub

    Public Overrides ReadOnly Property SupportedTypes() As IEnumerable(Of Type)
        Get
            Return New List(Of Type)(New Type() {GetType(IconNode)})
        End Get
    End Property

    ''' <summary>
    ''' Deserialize override.
    ''' </summary>
    Public Overrides Function Deserialize(ByVal dictionary As IDictionary(Of String, Object), ByVal type As Type, ByVal serializer As JavaScriptSerializer) As Object
        Dim node As IconNode = TryCast(MyBase.Deserialize(dictionary, type, serializer), IconNode)
        node.Label = dictionary("label").ToString()
        Return node
    End Function


    Public Overrides Function Serialize(ByVal obj As Object, ByVal serializer As System.Web.Script.Serialization.JavaScriptSerializer) As IDictionary(Of String, Object)
        Dim bApp As IconNode = TryCast(obj, IconNode)
        If bApp IsNot Nothing Then
            Dim json As IDictionary(Of String, Object) = MyBase.Serialize(obj, serializer)
            If json IsNot Nothing Then
                json.Add("label", bApp.Label)
            End If
            Return json
        End If
        Return Nothing

    End Function
End Class