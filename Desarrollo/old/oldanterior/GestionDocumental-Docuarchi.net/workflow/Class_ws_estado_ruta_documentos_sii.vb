Public Class Class_ws_estado_ruta_documentos_sii
    Public Shared Function Solicita_estado_sii_actividad_workflow(ByVal id_Actividad As Integer,
                                                                  ByRef estado_sii As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la relacion del estado de la actividad workflow en SII
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_Actividad          : Representa el nombre de plantilla del radicado
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'estado_sii            : Retorna el estado de la tarea con el SII
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_estado_ruta_documentos_sii")
            Dim Sql_consulta As String = "SELECT codigo_estado from ws_estado_ruta_documentos_sii  WHERE id_actividad_workflow=" & id_Actividad
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_sii_actividad_workflow = "Función Solicita_estado_sii_actividad_workflow dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_sii = "-1"
                Solicita_estado_sii_actividad_workflow = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estado_sii = "-1"
                Else
                    estado_sii = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_estado_sii_actividad_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_sii_actividad_workflow = "Inconsistencia general funcion Solicita_estado_sii_actividad_workflow " & ex.Message
        End Try
    End Function
End Class
