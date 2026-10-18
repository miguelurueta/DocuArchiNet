Imports System

' Convierte el rectángulo CSS de la imagen a una matriz PDF, sin E/S.
Public NotInheritable Class GeometriaGrafoPdf
    Public Shared Function Matriz(left As Double, bottom As Double, right As Double, top As Double,
                                  rotation As Integer, canvasWidth As Double, canvasHeight As Double,
                                  x As Double, yFromBottom As Double, width As Double, height As Double) As Double()
        For Each value In New Double() {left, bottom, right, top, canvasWidth, canvasHeight, x, yFromBottom, width, height}
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then Throw New ArgumentException("Geometría inválida")
        Next
        If right <= left OrElse top <= bottom OrElse canvasWidth <= 0 OrElse canvasHeight <= 0 OrElse
           width <= 0 OrElse height <= 0 Then Throw New ArgumentException("Dimensiones inválidas")
        If rotation <> 0 AndAlso rotation <> 90 AndAlso rotation <> 180 AndAlso rotation <> 270 Then
            Throw New ArgumentException("Rotación inválida")
        End If
        Dim viewWidth = If(rotation Mod 180 = 0, right - left, top - bottom)
        Dim viewHeight = If(rotation Mod 180 = 0, top - bottom, right - left)
        Dim u = x * viewWidth / canvasWidth
        Dim v = (yFromBottom - height) * viewHeight / canvasHeight
        Dim w = width * viewWidth / canvasWidth
        Dim h = height * viewHeight / canvasHeight
        Select Case rotation
            Case 90
                Return New Double() {0, w, -h, 0, right - v, bottom + u}
            Case 180
                Return New Double() {-w, 0, 0, -h, right - u, top - v}
            Case 270
                Return New Double() {0, -w, h, 0, left + v, top - u}
            Case Else
                Return New Double() {w, 0, 0, h, left + u, bottom + v}
        End Select
    End Function
End Class
