Public Class Class_ra_registro_proceso
    Function Solicita_nombre_proceso(ByVal id_proceso As Integer,
                                     ByRef nombre_proceso As String) As String
        '----------------------------------------------------
        'Funcion : Solicita el nombre del proceso por la 
        'identificacion del proceso
        'Fecha : 2022-02-17
        'Ing . Miguel Angel Urueta Miranda
        '-----------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT Nombre_proceso " &
          " FROM ra_registro_proceso  " &
          " where Id_registro_proceso=" & id_proceso
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_registro_proceso")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_proceso = "Función Solicita_nombre_proceso dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_proceso = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_proceso = "YES"
                Exit Function
            Else
                nombre_proceso = ""
                Solicita_nombre_proceso = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_proceso = "inconsistencia general funcion Solicita_nombre_proceso " & ex.Message
        End Try
    End Function
End Class
