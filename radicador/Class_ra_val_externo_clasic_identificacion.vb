Public Class Class_ra_val_externo_clasic_identificacion
    Function Solicita_lista_clasific_identificacion(ByRef ref_drowlis As DropDownList) As String
        '--------------------------------------------------------------
        'Funcion : Solicita datos de clasificación de identificacion
        'Fecha : 2022-09-30
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select nombre_tipo_identificacion from ra_val_externo_clasic_identificacion "
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta,
                                                          Datset)
            If Result <> "YES" Then
                Solicita_lista_clasific_identificacion = " Error listando tipologia de clasifición de identificación   " & Result
                Exit Function
            End If
            ref_drowlis.Items.Clear()
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_clasific_identificacion = "Imposible encontrar tipos de claficación en la tabla (ra_val_externo_clasic_identificacion)"
                Exit Function
            Else
                ref_drowlis.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ref_drowlis.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Solicita_lista_clasific_identificacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_clasific_identificacion = "Inconsistencia general funcion Solicita_lista_clasific_identificacion " & ex.Message
        End Try
    End Function
End Class
