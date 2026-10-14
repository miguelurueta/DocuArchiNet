Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class Class_transparent

    Function Transparent(ByVal image As Bitmap,
                         ByVal color As Color,
                         ByVal tolerance As Integer,
                         ByRef result As String) As Bitmap
        If image Is Nothing Then
            result = "Imagen inválida"
            Return Nothing
        End If
        If tolerance < 0 OrElse tolerance > 255 Then
            result = "Tolerancia inválida"
            Return Nothing
        End If

        Dim transparentBitmap As Bitmap = Nothing
        Dim bitmapData As BitmapData = Nothing
        Try
            transparentBitmap = New Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb)
            Using graphics As Graphics = Graphics.FromImage(transparentBitmap)
                graphics.DrawImageUnscaled(image, 0, 0)
            End Using

            Dim rectangle As New Rectangle(0, 0, transparentBitmap.Width, transparentBitmap.Height)
            bitmapData = transparentBitmap.LockBits(rectangle,
                                                     ImageLockMode.ReadWrite,
                                                     PixelFormat.Format32bppArgb)

            Dim stride As Integer = Math.Abs(bitmapData.Stride)
            Dim pixels(stride * transparentBitmap.Height - 1) As Byte
            Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length)

            For y As Integer = 0 To transparentBitmap.Height - 1
                Dim rowOffset As Integer = If(bitmapData.Stride >= 0,
                                              y * stride,
                                              (transparentBitmap.Height - 1 - y) * stride)
                For x As Integer = 0 To transparentBitmap.Width - 1
                    Dim offset As Integer = rowOffset + (x * 4)
                    Dim blue As Integer = pixels(offset)
                    Dim green As Integer = pixels(offset + 1)
                    Dim red As Integer = pixels(offset + 2)

                    If Math.Abs(color.R - red) < tolerance AndAlso
                       Math.Abs(color.G - green) < tolerance AndAlso
                       Math.Abs(color.B - blue) < tolerance Then
                        pixels(offset + 3) = 0
                    End If
                Next
            Next

            Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length)
            transparentBitmap.UnlockBits(bitmapData)
            bitmapData = Nothing
            result = "YES"
            Return transparentBitmap
        Catch
            result = "No fue posible aplicar transparencia"
            Return Nothing
        Finally
            If bitmapData IsNot Nothing AndAlso transparentBitmap IsNot Nothing Then
                transparentBitmap.UnlockBits(bitmapData)
            End If
            If result <> "YES" AndAlso transparentBitmap IsNot Nothing Then transparentBitmap.Dispose()
        End Try
    End Function

    Function IsCloseTo(ByVal color As Color, ByVal anotherColor As Color, ByVal tolerance As Integer) As Boolean
        Return Math.Abs(color.R - anotherColor.R) < tolerance AndAlso
               Math.Abs(color.G - anotherColor.G) < tolerance AndAlso
               Math.Abs(color.B - anotherColor.B) < tolerance
    End Function

End Class
