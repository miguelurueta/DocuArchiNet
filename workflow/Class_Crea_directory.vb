Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Xml
Public Class Class_Crea_directory
    Function Crea_directory(ByVal ruta_archivo As String) As String
        Try
            If Directory.Exists(ruta_archivo) = False Then
                Directory.CreateDirectory(ruta_archivo)
            End If
            Crea_directory = "YES"
            Exit Function
        Catch ex As Exception
            Crea_directory = "Inconsistencia creando el directorio (" & ruta_archivo & ") Error relacionado " & ex.Message
        End Try
    End Function
End Class
