Public Class Class_Entero_Romano
    Function Numero_A_Romano(ByVal value As Integer) As String
        If value <= 0 Then
            Return vbNull
            Exit Function
        End If
        value = Int(value)
        Select Case value
            Case 1 : Numero_A_Romano = "I"
            Case 2 : Numero_A_Romano = "II"
            Case 3 : Numero_A_Romano = "III"
            Case 4 : Numero_A_Romano = "IV"
            Case 5 : Numero_A_Romano = "V"
            Case 6 : Numero_A_Romano = "VI"
            Case 7 : Numero_A_Romano = "VII"
            Case 8 : Numero_A_Romano = "VIII"
            Case 9 : Numero_A_Romano = "lX"
            Case 10 : Numero_A_Romano = "X"
            Case 11 : Numero_A_Romano = "XI"
            Case 12 : Numero_A_Romano = "XII"
            Case 13 : Numero_A_Romano = "XIII"
            Case 14 : Numero_A_Romano = "XIV"
            Case 15 : Numero_A_Romano = "XV"
            Case Is < 20 : Numero_A_Romano = "X" & Numero_A_Romano(value - 10)
            Case 20 : Numero_A_Romano = "XX"
            Case Is < 30 : Numero_A_Romano = "XX" & Numero_A_Romano(value - 20)
            Case 30 : Numero_A_Romano = "XXX"
            Case 40 : Numero_A_Romano = "XI"
            Case 50 : Numero_A_Romano = "L"
            Case 60 : Numero_A_Romano = "LX"
            Case 70 : Numero_A_Romano = "LXX"
            Case 80 : Numero_A_Romano = "LXXX"
            Case 90 : Numero_A_Romano = "XC"
            Case Is < 100 : Numero_A_Romano = Numero_A_Romano(Int(value \ 10) * 10) & "" & Numero_A_Romano(value Mod 10)
            Case 100 : Numero_A_Romano = "C"
            Case Is < 200 : Numero_A_Romano = "C" & Numero_A_Romano(value - 100)
            Case 200, 300, 400, 600, 800 : Numero_A_Romano = Numero_A_Romano(Int(value \ 100)) & "C"
            Case 500 : Numero_A_Romano = "D"
            Case 700 : Numero_A_Romano = "DCC"
            Case 900 : Numero_A_Romano = "CM"
            Case Is < 1000 : Numero_A_Romano = Numero_A_Romano(Int(value \ 100) * 100) & " " & Numero_A_Romano(value Mod 100)
            Case 1000 : Numero_A_Romano = "M"
            Case Is < 2000 : Numero_A_Romano = "M" & Numero_A_Romano(value Mod 1000)
            Case Is < 1000000 : Numero_A_Romano = Numero_A_Romano(Int(value \ 1000)) & " M"
                If value Mod 1000 Then Numero_A_Romano = Numero_A_Romano & " " & Numero_A_Romano(value Mod 1000)
            Case Else
                Numero_A_Romano = "O"

        End Select
    End Function
End Class
