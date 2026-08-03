Public Class Class_ra_campos_obligatorio_tipo_tramite
    Function Lista_campos_obligatorios_unico_radicacion(ByVal id_tipo_tramite As Integer, _
                                                        ByRef stru() As estru_campos_unicos) As String
        Try
            Dim conext As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim Result As String = ""
            Erase stru
            Dim Parametro_Consulta As String = "Select id_campo_obligatorio_tipo_tramite,Campo_Plantilla,campo_valida_unico,campo_valida_obligatorio from ra_campos_obligatorio_tipo_tramite  " & _
                "where id_top_doc_entrante=" & id_tipo_tramite
            Result = conext.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Lista_campos_obligatorios_unico_radicacion = "Inconsistencia listando campos obligatorios y unicos " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Lista_campos_obligatorios_unico_radicacion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru(i)
                    stru(i).id_campo_obligatorio_tipo_tramite = datset.Tables(0).Rows(i).Item(0)
                    stru(i).Campo_Plantilla = datset.Tables(0).Rows(i).Item(1)
                    stru(i).campo_valida_unico = datset.Tables(0).Rows(i).Item(2)
                    stru(i).campo_valida_obligatorio = datset.Tables(0).Rows(i).Item(3)
                Next
                Lista_campos_obligatorios_unico_radicacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_campos_obligatorios_unico_radicacion = "Inconsistencia función Lista_campos_obligatorios_unico_radicacion " & ex.Message
        End Try
    End Function
End Class
