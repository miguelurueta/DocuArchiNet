Imports System.Runtime.CompilerServices
Imports System
Imports System.Drawing
Imports System.Threading.Tasks
Public Class Class_transparent

    Function TransparentAsync(ByVal image As Bitmap, ByVal color As Color, ByVal tolerance As Integer, ByRef result As String) As Object
        Try
            Dim result_ As String = ""
            Dim bitm As Object
            bitm = Me.Transparent(image, color, tolerance, result_)
            If result_ = "YES" Then
                result = result_
                Return bitm
            Else
                result = result_
                Return Nothing
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Function Transparent(ByVal image As Bitmap, ByVal color As Color, ByVal tolerance As Integer, ByRef result As String) As Bitmap
        Try
            Dim b As Bitmap = New Bitmap(image)
            Me.ForEachPixel(b, Function(i, j, col)
                                   If IsCloseTo(color, col, tolerance) Then
                                       b.SetPixel(i, j, color)
                                   End If
                               End Function)
            b.MakeTransparent(color)
            result = "YES"
            Return b
        Catch ex As Exception
            result = ex.Message
        End Try
    End Function


    Sub ForEachPixel(ByVal image As Bitmap, ByVal onPixel As Action(Of Integer, Integer, Color))
        For i As Integer = image.Size.Width - 1 To 0
            For j As Integer = image.Size.Height - 1 To 0
                onPixel(i, j, image.GetPixel(i, j))
            Next
        Next
    End Sub


    Function IsCloseTo(ByVal color As Color, ByVal anotherColor As Color, ByVal tolerance As Integer) As Boolean
        Return Math.Abs(color.R - anotherColor.R) < tolerance AndAlso Math.Abs(color.G - anotherColor.G) < tolerance AndAlso Math.Abs(color.B - anotherColor.B) < tolerance
    End Function


End Class
