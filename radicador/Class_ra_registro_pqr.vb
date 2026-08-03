Public Class Class_ra_registro_pqr
    Function Solicita_correo_electronico_usuario_pqr(ByVal radicado As String,
                                                     ByRef correo_eloectronico As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_registro_pqr")
            Dim Sql_consulta As String = "Select correo_electronico  from " & " ra_registro_pqr " &
              " WHERE radicado_pqr ='" & radicado & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_correo_electronico_usuario_pqr = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                correo_eloectronico = ""
                Solicita_correo_electronico_usuario_pqr = "YES"
                Exit Function
            Else

                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    correo_eloectronico = ""
                Else
                    correo_eloectronico = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_correo_electronico_usuario_pqr = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_correo_electronico_usuario_pqr = "Inconsistencia general funcion Solicita_correo_electronico_usuario_pqr " & ex.Message
        End Try
    End Function
End Class
