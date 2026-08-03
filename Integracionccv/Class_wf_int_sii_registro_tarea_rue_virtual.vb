Public Class Class_wf_int_sii_registro_tarea_rue_virtual
    Function Solicita_existencia_registro_radicado_sii(ByVal datos_radicado As String,
                                                       ByRef existencia_radicado As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la existencia del registro de un radicado SII VIRTUAL
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'datos_radicado           : Representa el radicado SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'existencia_radicado  : Retorna existencia del radicado SII YES/NO
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-02
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_int_sii_registro_tarea_rue_virtual")
            Dim Sql_consulta As String = ""
            Sql_consulta = "Select CODIGO_BARRAS from wf_int_sii_registro_tarea_rue_virtual  " &
            " WHERE CODIGO_BARRAS='" & datos_radicado & "'"
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_radicado_sii = "Función Solicita_existencia_registro_recibo_sii dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia_radicado = "NO"
                Solicita_existencia_registro_radicado_sii = "YES"
                Exit Function
            Else
                existencia_radicado = "YES"
                Solicita_existencia_registro_radicado_sii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_radicado_sii = "Inconistencia general funcion Solicita_existencia_registro_radicado_sii " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_registro_recibo_sii(ByVal datos_recibo As String,
                                                     ByRef existencia_recibo As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la existencia del registro de un recibo SII RUE
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'datos_recibo           : Representa el recibo SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'existencia_recibo  : Retorna existencia del recibo SII YES/NO
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-30
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_int_sii_registro_tarea_rue_virtual")
            Dim Sql_consulta As String = ""
            Sql_consulta = "Select DATOS_RECIBO from wf_int_sii_registro_tarea_rue_virtual  " &
            " WHERE DATOS_RECIBO='" & datos_recibo & "'"
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_recibo_sii = "Función Solicita_existencia_registro_recibo_sii dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia_recibo = "NO"
                Solicita_existencia_registro_recibo_sii = "YES"
                Exit Function
            Else
                existencia_recibo = "YES"
                Solicita_existencia_registro_recibo_sii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_recibo_sii = "Inconistencia general funcion Solicita_existencia_registro_recibo_sii " & ex.Message
        End Try
    End Function
End Class
