Public Structure STRU_CONFIG_PRODUCION
    Dim ACTIVA_OBLIGA_TRD As Integer
    Dim MAX_CARACTERES_CARPETA As Integer
    Dim ACTIVA_CONSECUTIVO_PLANTILLA As Integer
    Dim ID_GABINETE_PRODUCION As Integer
End Structure
Public Class Class_ra_pro_config_producion_documental
    Function Solicita_obligatoriedad_aplica_trd_producion_documental(ByRef stru_config As STRU_CONFIG_PRODUCION) As String
        Try
            Dim Parametro_Consulta As String = "select ACTIVA_OBLIGA_TRD,MAX_CARACTERES_CARPETA,ACTIVA_CONSECUTIVO_PLANTILLA,ID_GABINETE_PRODUCION " &
                                      "from RA_PRO_CONFIG_PRODUCION_DOCUMENTAL"
            Dim Ref_Car_Conec33 As New conect.Dbase_Conction_Mysql_RA
            Dim datset As DataSet = New DataSet("RA_PRO_CONFIG_PRODUCION_DOCUMENTAL")
            Dim Result As String = ""
            Result = Ref_Car_Conec33.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Solicita_obligatoriedad_aplica_trd_producion_documental = "Función Solicita_obligatoriedad_aplica_trd_producion_documental dice " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Solicita_obligatoriedad_aplica_trd_producion_documental = "Imposible encontrar las opciones de configuración de la producción documental, consulte al administrador para agregue la configuración"
                Exit Function
            Else
                stru_config.ACTIVA_OBLIGA_TRD = datset.Tables(0).Rows(0).Item(0)
                stru_config.MAX_CARACTERES_CARPETA = datset.Tables(0).Rows(0).Item(1)
                stru_config.ACTIVA_CONSECUTIVO_PLANTILLA = datset.Tables(0).Rows(0).Item(2)
                If datset.Tables(0).Rows(0).IsNull(3) Then
                    stru_config.ID_GABINETE_PRODUCION = 0
                Else
                    stru_config.ID_GABINETE_PRODUCION = datset.Tables(0).Rows(0).Item(3)
                End If
                Solicita_obligatoriedad_aplica_trd_producion_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_obligatoriedad_aplica_trd_producion_documental = "Inconsistencia general función Solicita_obligatoriedad_aplica_trd_producion_documental " & ex.Message
        End Try
    End Function
End Class
