Public Class Class_zero_fill
    Function zero_fill(ByRef texto_fill As String, _
                       ByVal numero_fill As Integer, _
                       ByVal textofill_add As String) As String
        '-----------------------------------------------------
        'Función : El usuario envia el valor en el campo
        'texto_fill, el sistema agrega el numero el valor
        'enviado en el parametro textofill_add el numero
        'de veces que se indique en el parametro numero_fill
        'a la izquierda.
        '------------------------------------------------------
        Try
            texto_fill = texto_fill.ToString.PadLeft((numero_fill - texto_fill.ToString.Length) + texto_fill.ToString.Length, textofill_add)
            zero_fill = "YES"
        Catch ex As Exception
            zero_fill = "Inconsistencia general funcion " & "zero_fill" & ex.Message
        End Try
    End Function
End Class
