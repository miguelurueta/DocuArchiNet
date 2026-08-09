Public Class Class_relacion_sirep_workflow
    Function Solicita_codigo_sii_usuario_workflow(ByVal id_usuario_wf As Integer,
                                                  ByRef codigo_sii As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT CODIGO_CORTO_SIREP FROM relacion_sirep_workflow" &
            " WHERE ID_UUSARIO_WORKFLOW=" & id_usuario_wf
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("relacion_sirep_workflow")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_codigo_sii_usuario_workflow = "Funcion  Solicita_codigo_sii_usuario_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                codigo_sii = ""
                Solicita_codigo_sii_usuario_workflow = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    codigo_sii = ""
                Else
                    codigo_sii = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_codigo_sii_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_codigo_sii_usuario_workflow = "Inconsistencia general función Solicita_codigo_sii_usuario_workflow " & ex.Message
        End Try
    End Function

    Public Shared Function Solicita_codigo_usuario_sii_operador(ByVal id_usuario_workflow As Integer,
                                                                ByRef codigo_usuario_sii As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el codigo usuario oprador SII
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_usuario_workflow         : Representa el id usuario workfolow
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        ' codigo_usuario_sii           : Retorna codigo sii
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("relacion_sirep_workflow")
            Dim Sql_consulta As String = ""
            Sql_consulta = "Select CODIGO_CORTO_SIREP from relacion_sirep_workflow  " &
            " WHERE ID_UUSARIO_WORKFLOW='" & id_usuario_workflow & "'"
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_codigo_usuario_sii_operador = "Función Solicita_codigo_usuario_sii_operador dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_codigo_usuario_sii_operador = "Imposible encontrar el codigo sii del usuario (" & id_usuario_workflow & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    codigo_usuario_sii = ""
                Else
                    codigo_usuario_sii = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_codigo_usuario_sii_operador = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_codigo_usuario_sii_operador = "Inconsistencia general función Solicita_codigo_usuario_sii_operador " & ex.Message
        End Try
    End Function
End Class
