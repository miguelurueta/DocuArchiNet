Public Class Class_ws_usuarioworkflowsii
    Function solicita_usuario_validacion_sii(ByRef codigo_empresa As String, _
                                             ByRef usuariows As String, _
                                             ByRef clavews As String) As String
        Try
            Dim Sql_consulta As String = "Select usuariows,clavews,codigoempresa from ws_usuarioworkflowsii"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_usuarioworkflowsii")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                solicita_usuario_validacion_sii = "Error funcion solicita_usuario_validacion_sii " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                solicita_usuario_validacion_sii = "Imposible encontrar usuario de validación SII"
                Exit Function
            Else
                usuariows = CStr(Datset.Tables(0).Rows(0).Item(0).ToString)
                clavews = CStr(Datset.Tables(0).Rows(0).Item(1).ToString)
                codigo_empresa = CStr(Datset.Tables(0).Rows(0).Item(2).ToString)
                solicita_usuario_validacion_sii = "YES"
            End If
        Catch ex As Exception
            solicita_usuario_validacion_sii = "Inconsistencia general función  solicita_usuario_validacion_sii " & ex.Message
        End Try
    End Function
    Function Solicita_url_nombrefuncion_restfull(ByRef url As String, _
                                                ByVal nombre_funcion As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_usuarioworkflowsii")
            Dim Sql_consulta As String = "Select url,nombre_funcion_url from  ws_url_restfull where nombre_funcion_url='" _
                                         & nombre_funcion & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_url_nombrefuncion_restfull = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_url_nombrefuncion_restfull = "Imposible encontrar la url de la función  " & nombre_funcion
                Exit Function
            Else
                url = Datset.Tables(0).Rows(0).Item(0)
                Solicita_url_nombrefuncion_restfull = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_url_nombrefuncion_restfull = "Inconsistencia general función  Solicita_url_nombrefuncion_restfull " & ex.Message
        End Try
    End Function
End Class
