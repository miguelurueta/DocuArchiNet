Public Class Class_ra_registro_instrumento_archivistico
    Function Retorna_id_instrumento_activo(ByVal id_tipo_instrumento As Integer, _
                                           ByVal id_organigrama As Integer, _
                                           ByRef id_instrumento_activo As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_instrumento from ra_registro_instrumento_archivistico " & _
                " where Estado_instrumento=1 and id_tipo_instrumento=" & id_tipo_instrumento & _
                " and registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_instrumento_activo = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_instrumento_activo = 0
                Retorna_id_instrumento_activo = "YES"
                Exit Function
            Else
                id_instrumento_activo = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_instrumento_activo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_instrumento_activo = "Inconsistencia general función Retorna_id_instrumento_activo " & ex.Message
        End Try
    End Function
    Function Retorna_id_tipo_instrumento(ByVal id_instrumento As Integer, _
                                         ByRef id_tipo_instrumento As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select id_tipo_instrumento from ra_registro_instrumento_archivistico " & _
                " where id_instrumento=" & id_instrumento
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_tipo_instrumento = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_tipo_instrumento = 0
                Retorna_id_tipo_instrumento = "YES"
                Exit Function
            Else
                id_tipo_instrumento = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_tipo_instrumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_tipo_instrumento = "Inconsistencia general función Retorna_id_tipo_instrumento " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_instrumento(ByVal id_instrumento As Integer, _
                                        ByRef nombre_instrumento As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim sql_consulta As String = "Select Nombre_instrumento from ra_registro_instrumento_archivistico " & _
                " where id_instrumento=" & id_instrumento
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_instrumento = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_instrumento = ""
                Retorna_nombre_instrumento = "YES"
                Exit Function
            Else
                nombre_instrumento = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_instrumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_instrumento = "Inconsistencia general función Retorna_nombre_instrumento " & ex.Message
        End Try
    End Function
End Class
