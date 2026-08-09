Public Class Class_ra_val_externo_clasic_sexo
    Function Solicita_lista_clasific_sexo(ByRef ref_drowlis As DropDownList) As String
        '-----------------------------------------------------------------
        'Funcion : Solicita datos de clasificación del tipo de sexo
        'Fecha : 2022-09-30
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select nombre_tipo_sexo from ra_val_externo_clasic_sexo "
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta,
                                                          Datset)
            If Result <> "YES" Then
                Solicita_lista_clasific_sexo = " Error listando tipologia de clasifición de sexo   " & Result
                Exit Function
            End If
            ref_drowlis.Items.Clear()
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_clasific_sexo = "Imposible encontrar tipos de claficación en la tabla (ra_val_externo_clasic_sexo)"
                Exit Function
            Else
                ref_drowlis.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ref_drowlis.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Solicita_lista_clasific_sexo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_clasific_sexo = "Inconsistencia general funcion Solicita_lista_clasific_sexo " & ex.Message
        End Try
    End Function
End Class
