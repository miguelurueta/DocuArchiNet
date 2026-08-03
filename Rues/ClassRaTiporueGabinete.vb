Public Class ClassRaTiporueGabinete
    Function SolicitaNombreGabineteConCodigoGabineteTipoRue(ByVal CodigoGabineteRue As String,
                                                            ByRef IdGabineteRue As Integer,
                                                            ByRef NombreGabinete As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el identificador del gabinete en la tabla de relación
        'de rues, y el nombre del gabinete relacionado al codigo rue
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CodigoGabineteRue   : Representa el codigo gabinete rue
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdGabineteRue  : Retorna la identificación del gabinete rue 
        'NombreGabinete : Retorna el nombre del gabinete RUE
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2017-08-17
        'Elabora               : Miguel Angel Urueta Miranda
        'Modificado            : 2025-05-06
        'Modifica              : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Parametro_consulta As String = "Select ID_RUE_GABINETE,NOMBRE_GABINETE from ra_tiporue_gabinete where CODIGO_GABINETE_RUE='" & CodigoGabineteRue & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_tiporue_gabinete")
            Dim Result = ref.SELECTION_SELECT_FIELDA(Parametro_consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreGabineteConCodigoGabineteTipoRue = "Función SolicitaNombreGabineteConCodigoGabineteTipoRue Error (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreGabineteConCodigoGabineteTipoRue = "Imposible encontrar la relación del código gabinete " & CodigoGabineteRue & " en la tabla relación ra_tiporue_gabinete"
                Exit Function
            Else
                IdGabineteRue = Datset.Tables(0).Rows(0).Item(0)
                NombreGabinete = Datset.Tables(0).Rows(0).Item(1)
                SolicitaNombreGabineteConCodigoGabineteTipoRue = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreGabineteConCodigoGabineteTipoRue = "Inconsistencia general función SolicitaNombreGabineteConCodigoGabineteTipoRue " & ex.Message
        End Try
    End Function
End Class
