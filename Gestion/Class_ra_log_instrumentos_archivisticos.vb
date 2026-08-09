Public Class Class_ra_log_instrumentos_archivisticos
    Function Solicita_existencia_registro_instrumento(ByVal id_instrumento As Integer, _
                                                      ByRef existencia As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_log_instrumentos_archivisticos")
            Dim sql_consulta As String = "Select id_tran from ra_log_instrumentos_archivisticos " & _
                " where Ra_registro_instrumento_archivistico_id_instrumento=" & id_instrumento 
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_instrumento = "Función Solicita_existencia_registro_instrumento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Solicita_existencia_registro_instrumento = "YES"
                Exit Function
            Else
                existencia = "YES"
                Solicita_existencia_registro_instrumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_instrumento = "Inconsistencia general función Solicita_existencia_registro_instrumento " & ex.Message
        End Try
    End Function
End Class
