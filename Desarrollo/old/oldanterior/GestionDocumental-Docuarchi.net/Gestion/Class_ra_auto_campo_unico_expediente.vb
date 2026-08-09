Public Structure stru_campos_expediente
    Dim campo_expediente As String
    Dim valor_campo_expediente As String
    Dim estado_obligatorio As Integer
    Dim estado_unico As Integer
End Structure
Public Class Class_ra_auto_campo_unico_expediente
    Function SolicitaCamposUnicosAutoRegistroExpediente(ByVal IdAutoRegistro As Integer,
                                                        ByRef stru_campos_expediente() As stru_campos_expediente) As String
        '-----------------------------------------------------------------
        'Funcion : Solicita los campos del expediente de auto registro 
        'para solicitar a servicios externos los datos para la creación
        'del expediente automatico.
        'Fecha : 2022-06-14
        'Ing. Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------
        Try
            Erase stru_campos_expediente
            Dim Parametro_Consulta As String = " SELECT  campo_expediente,estado_obligatorio,estado_unico" &
            " from ra_auto_campo_unico_expediente where ra_auto_registro_expediente_id_auto_registro='" &
            IdAutoRegistro & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_auto_campo_unico_expediente")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaCamposUnicosAutoRegistroExpediente = "Función SolicitaCamposUnicosAutoRegistroExpediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaCamposUnicosAutoRegistroExpediente = "Imposible encontrar  registro de campos unicos de auto gestión del codigo (" & IdAutoRegistro & ") , por favor revice el codigo del auto registro"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_campos_expediente(i)
                    stru_campos_expediente(i).campo_expediente = Datset.Tables(0).Rows(i).Item(0)
                    stru_campos_expediente(i).estado_obligatorio = Datset.Tables(0).Rows(i).Item(1)
                    stru_campos_expediente(i).estado_unico = Datset.Tables(0).Rows(i).Item(2)
                    stru_campos_expediente(i).valor_campo_expediente = ""
                Next
                SolicitaCamposUnicosAutoRegistroExpediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaCamposUnicosAutoRegistroExpediente = "Inconsistencia general funcion SolicitaCamposUnicosAutoRegistroExpediente " & ex.Message
        End Try
    End Function
End Class
