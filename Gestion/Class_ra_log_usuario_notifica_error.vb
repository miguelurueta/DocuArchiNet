Public Class Class_ra_log_usuario_notifica_error
    Function Solicita_correo_notifica_error(ByVal Id_modulo As Integer,
                                            ByRef correo_electronico As String) As String
        '*****************************************************************
        'Funcion : Retorna el correo electronico notificación errores
        'Fecha : 2022-10-15
        'Ing : Miguel Angel Urueta Miranda
        '*****************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "SELECT correo_administrador FROM ra_log_usuario_notifica_error   " &
            "  where id_modulo=" & Id_modulo
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_correo_notifica_error = " Error Solicita_correo_notifica_error   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                correo_electronico = ""
                Solicita_correo_notifica_error = "YES"
                Exit Function
            Else
                correo_electronico = Datset.Tables(0).Rows(0).Item(0).ToString
                Solicita_correo_notifica_error = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_correo_notifica_error = "Inconsistencia general funcion Solicita_correo_notifica_error " & ex.Message
        End Try
    End Function
End Class
