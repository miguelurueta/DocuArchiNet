Public Class Class_ra_tipo_documento
    Function Solicita_id_clase_documento(ByVal clase_documento As String, _
                                         ByRef id_clase_documento As Integer) As String
        Try
            Dim Parametro_Consulta As String = "SELECT ID_TIPO_DOCUMENTO" & _
                   " FROM ra_tipo_documento " & _
                    " where  DOCUMENTO='" & clase_documento & "'"
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_tipo_documento")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_clase_documento = "Función Solicita_id_clase_documento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_clase_documento = "Imposible econtrar el id de la clase del documento " & clase_documento
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_clase_documento = 0
                Else
                    id_clase_documento = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_clase_documento = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_id_clase_documento = "Inconsistencia función Solicita_id_clase_documento " & ex.Message
        End Try
    End Function
End Class
