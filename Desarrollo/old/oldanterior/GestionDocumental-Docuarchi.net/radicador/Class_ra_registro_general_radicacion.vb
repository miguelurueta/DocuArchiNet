Public Class Class_ra_registro_general_radicacion
    Function SolicitaNombrePlantillaRadicado(ByVal Radicado As String,
                                             ByRef NombrePlantillaRadicado As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita nombre plantilla radicación radicado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Radicado            : Representa el consecutivo radicado
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombrePlantillaRadicado  : Retorna el nombre de la platilla de radicación
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-29
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select Nombre_plantilla_radicado from ra_registro_general_radicacion " &
                " where Consecutivo_Rad='" & Radicado & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_registro_general_radicacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombrePlantillaRadicado = "Función  Solicita_nombre_plantilla_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombrePlantillaRadicado = "Imposible encontrar el radicado (" & Radicado & ") en registro general, imposible determinar la plantilla de radicación"
                Exit Function
            Else
                NombrePlantillaRadicado = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombrePlantillaRadicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombrePlantillaRadicado = "Inconsistencia general función SolicitaNombrePlantillaRadicado " & ex.Message
        End Try

    End Function
End Class
