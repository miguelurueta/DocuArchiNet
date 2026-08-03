Public Class ClassGredview
    Function add_clase_acender_decender(ByVal expresion_colum_name As String, _
                                        ByVal stor_matri_colum() As Object, _
                                        ByVal srt_direccion As String, _
                                        ByRef gred As GridView) As String
        Try

            For i As Integer = 0 To gred.HeaderRow.Cells.Count - 1
                gred.HeaderRow.Cells(i).Attributes.Remove("Class")              
            Next          
            For i As Integer = 0 To stor_matri_colum.Length - 1
                Dim tempo_cell As String = stor_matri_colum(i)
                If tempo_cell <> "" Then
                    If tempo_cell = expresion_colum_name Then
                        If srt_direccion = "ASC" Then
                            gred.HeaderRow.Cells(i).Attributes.Add("Class", "SortedDescendingHeaderStyle")
                        End If
                        If srt_direccion = "DESC" Then
                            gred.HeaderRow.Cells(i).Attributes.Add("Class", "SortedAscendingHeaderStyle")
                        End If
                    End If
                End If

            Next
            add_clase_acender_decender = "YES"
        Catch ex As Exception
            add_clase_acender_decender = "Inconsistencia general función add_clase_acender_decender " & ex.Message
        End Try
    End Function
    
End Class
