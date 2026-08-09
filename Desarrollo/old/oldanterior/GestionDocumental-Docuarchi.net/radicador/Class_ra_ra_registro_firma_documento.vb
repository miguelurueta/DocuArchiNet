Public Class Class_ra_ra_registro_firma_documento

    Function Solicita_ultimo_usuario_firma_documento(ByVal id_respuesta As Integer, _
                                                      ByRef id_usuario_firma As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select id_usuario_firma from ra_ra_registro_firma_documento " & _
                                               " Where ra_respuesta_radicado_ID_RESPUESTA_RADICADO=" & id_respuesta & _
                                               "  order by id_registro_firma desc  LIMIT 1 "
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_ultimo_usuario_firma_documento = "Función Solicita_ultimo_usuario_firma_documento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_usuario_firma = 0
                Solicita_ultimo_usuario_firma_documento = "YES"
                Exit Function
            Else
                id_usuario_firma = Datset.Tables(0).Rows(0).Item(0)
                Solicita_ultimo_usuario_firma_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_ultimo_usuario_firma_documento = "Inconsistencia general función Solicita_ultimo_usuario_firma_documento " & ex.Message
        End Try
    End Function
End Class
