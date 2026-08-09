Public Class ClassRaCamposConsultaRueGabinete
    Function SolicitaNombreCampoConsultaGabinete(ByVal IdGabineteRue As Integer,
                                                 ByRef CampoConsulta As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion :  Solicita el noombre del campo consulta del gabinete según codigo enviado por el rue
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdGabineteRue       : Representa la identificación del gabinete RUE
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CampoConsulta  : Retorna el nombre del campo consulta
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2017-08-17
        'Elabora               : Miguel Angel Urueta Miranda
        'Fecha modifica        : 2025-05-06
        'Modifica              : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_consulta As String = "Select NOMBRE_CAMPO_CONSULTA from ra_campos_consulta_rue_gabinete where Ra_tiporue_gabinete_ID_RUE_GABINETE='" & IdGabineteRue & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_campos_consulta_rue_gabinete")
            Dim Result = ref.SELECTION_SELECT_FIELDA(Parametro_consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreCampoConsultaGabinete = "Función SolicitaNombreCampoConsultaGabinete Error  (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreCampoConsultaGabinete = "Imposible encontrar el campo consulta del gabinete  en la tabla relación ra_campos_consulta_rue_gabinete"
                Exit Function
            Else
                CampoConsulta = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreCampoConsultaGabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreCampoConsultaGabinete = "Inconsistencia general función SolicitaNombreCampoConsultaGabinete " & ex.Message
        End Try

    End Function
End Class
