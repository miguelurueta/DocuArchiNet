Imports System.IO.File
Public Class ClassBase64
    Function DecodeBase64ToString(valor As String) As String
        Try
            Dim myBase64ret As Byte() = Convert.FromBase64String(valor)
            Dim myStr As String = System.Text.Encoding.UTF8.GetString(myBase64ret)
            Return myStr
            DecodeBase64ToString = "YES"
        Catch ex As Exception
            DecodeBase64ToString = "Funcion DecodeBase64ToString " & ex.Message
        End Try

    End Function
    Function EncodeStrToBase64(valor As String) As String
        Dim myByte As Byte() = System.Text.Encoding.UTF8.GetBytes(valor)
        Dim myBase64 As String = Convert.ToBase64String(myByte)
        Return myBase64
    End Function
    Function DecodeBase64ToFile(valor As String, ByVal file_ As String) As String
        Try
            'If System.IO.File.Exists(file_) Then
            '    System.IO.File.Delete(file_)
            'End If
            Dim imageBytes As Byte() = Convert.FromBase64String(valor)
            System.IO.File.WriteAllBytes(file_, imageBytes)
            DecodeBase64ToFile = "YES"
        Catch ex As Exception
            DecodeBase64ToFile = "Inconsistencia general funcion DecodeBase64ToFile " & ex.Message
        End Try
    End Function
End Class
