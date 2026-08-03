
Public Class Class_ra_config_upload_gestion
    Public Structure ra_config_upload_gestion
        Dim ID_CONFIG_UPLOAD_GESTION As Integer
        Dim EXTENSION_UPLOAD As String
        Dim LENG_UPLOAD As Long
        Dim NAME_PROCESO As String
        Dim ESTADO_PROCESO As String
    End Structure
    Function Solicita_parameter_upload_gestion(ByVal name_proceso As String,
                                               ByRef ra_config_upload_gestion As ra_config_upload_gestion) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita los parametros para la carga de archivos
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'name_proceso          : Representa el nombre del proceso
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ra_config_upload_gestion : Retorna los parametros
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-01-12
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select ID_CONFIG_UPLOAD_GESTION,EXTENSION_UPLOAD,LENG_UPLOAD,NAME_PROCESO,ESTADO_PROCESO " &
           " from RA_CONFIG_UPLOAD_GESTION where NAME_PROCESO='" & name_proceso & "' and ESTADO_PROCESO=1"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("da_extension")
            Dim Result As String = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_parameter_upload_gestion = "La funcion Solicita_parameter_upload_gestion dice (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_parameter_upload_gestion = "El sistema no pudo encontrar la configuración de upload de archivos para el proceso (" & name_proceso & ") en la tabla ( ra_config_upload_gestion )"
                Exit Function
            Else
                ra_config_upload_gestion.ID_CONFIG_UPLOAD_GESTION = Datset.Tables(0).Rows(0).Item("ID_CONFIG_UPLOAD_GESTION")
                ra_config_upload_gestion.EXTENSION_UPLOAD = Datset.Tables(0).Rows(0).Item("EXTENSION_UPLOAD")
                ra_config_upload_gestion.LENG_UPLOAD = Datset.Tables(0).Rows(0).Item("LENG_UPLOAD")
                ra_config_upload_gestion.NAME_PROCESO = Datset.Tables(0).Rows(0).Item("NAME_PROCESO")
                ra_config_upload_gestion.ESTADO_PROCESO = Datset.Tables(0).Rows(0).Item("ESTADO_PROCESO")
                Solicita_parameter_upload_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_parameter_upload_gestion = "Inconsistencia general funcion Solicita_parameter_upload_gestion error (" & ex.Message & ")"
        End Try
    End Function
End Class
