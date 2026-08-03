Public Class Class_imp01_plantillaimp
    Function Solicita_identificacion_plantilla_externa_x_nombe(ByVal nombre_plantilla As String,
                                                               ByRef id_plantilla As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identificación de una plantilla por nombre 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'nombre_plantilla    : Representa el nombre de la plantilla
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_plantilla  : Retorna la idnetificación de la plantilla
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Sql_consulta = "SELECT ID_PLANTILLAIMP FROM " &
         "imp01_plantillaimp " &
         "WHERE NOMBRE_PLANTILLA='" & nombre_plantilla & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("imp01_plantillaimp ")
            Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Resulta <> "YES" Then
                Solicita_identificacion_plantilla_externa_x_nombe = "Funcion  Solicita_identificacion_plantilla_externa_x_nombe : (" & Resulta & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_identificacion_plantilla_externa_x_nombe = "Imposible encontrar la identifcación de la plantilla : (" & nombre_plantilla & ")"
                Exit Function
            Else
                id_plantilla = Datset.Tables(0).Rows(0).Item(0)
                Solicita_identificacion_plantilla_externa_x_nombe = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_plantilla_externa_x_nombe = "Inconsistencia general función Solicita_identificacion_plantilla_externa " & ex.Message
        End Try
    End Function
End Class
