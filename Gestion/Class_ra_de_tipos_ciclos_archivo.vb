Public Class Class_ra_de_tipos_ciclos_archivo
    Function Retorna_id_ciclo_archivo_nombre(ByVal nombre_ciclo_archivo As String, _
                                            ByRef id_ciclo_archivo As Integer) As String
        '**************************************************
        'Funcion : Retorna id ciclo archivo
        'Fecha : 2017-02-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '*************************************************
        Try
            Dim sqlconsulta As String = "Select Id_tipos_ciclo_archivo from ra_de_tipos_ciclos_archivo  " & _
                          " where Nombre_Tipo_ciclo_archivo='" & nombre_ciclo_archivo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_de_tipos_ciclos_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_ciclo_archivo_nombre = "Función Retorna_id_ciclo_archivo_nombre Error de  conexión " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_ciclo_archivo = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_ciclo_archivo_nombre = "YES"
                Exit Function
            Else
                Retorna_id_ciclo_archivo_nombre = "Imposible encontrar la identificación del nombre del ciclo " & nombre_ciclo_archivo
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_ciclo_archivo_nombre = "Inconsistencia función Retorna_id_ciclo_archivo_nombre " & ex.Message
        End Try
    End Function
End Class
