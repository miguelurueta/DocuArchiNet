Public Class Class_disco_detalle
    Function Numero_Imagenes(ByVal gabinet As String, _
                                    ByVal tandiscvar As Long, _
                                    ByVal disc As Long, _
                                    ByRef EstadoDisco As String) As String
        '*****************************************************************
        'Funcion Numero_Imagenes
        'Fecha 2013-08-09
        'Ingeniero Miguel Angel Urueta Miranda
        '*****************************************************************
        Try
            Dim Sql_consulta As String = "select NUMERO_IMAGENES  from disco_detalle  where disco = '" & disc & "'" & _
            " and gabinete ='" & gabinet & "'"
            Dim Numero_Imagenesl As Integer = 0
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("disco_detalle")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If result <> "YES" Then
                Numero_Imagenes = "Error Consultando en tabla Disco_Detalle " & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Numero_Imagenes = "Imposible encontrar registros en la tabla " & gabinet
                Exit Function
            End If

            Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(0)
            If IsDBNull(Tempvalor) Then
                Numero_Imagenes = "El disco " & disc & " no esta sincronizado para alamcenar contacte a su administrador estado null"
                Exit Function
            End If
            Numero_Imagenesl = Tempvalor
            If Numero_Imagenesl = 0 Then
                Numero_Imagenes = "El disco " & disc & " no esta sincronizado para alamcenar contacte a su administrador"
                Exit Function
            End If
            If tandiscvar > 572523149 And Numero_Imagenesl > 80000 Then
                EstadoDisco = "SL"
                Numero_Imagenes = "YES"
                Exit Function
            End If
            If tandiscvar < 572523149 And Numero_Imagenesl > 7500 Then
                EstadoDisco = "SL"
                Numero_Imagenes = "YES"
                Exit Function
            End If
            Numero_Imagenes = "YES"
        Catch ex As Exception
            Numero_Imagenes = "Inconsistencia general función Función Numero_Imagenes " & ex.Message
        End Try

    End Function
End Class
