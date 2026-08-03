Public Structure stru_ra_mig_config_migracion
    Dim id_mig_config_migracion As Integer
    Dim aplica_ocr As Integer
    Dim aplica_comprencion As Integer
    Dim nivel_comprencion As Integer
    Dim ruta_temporal_migracion As String       ' Almacena la ruta temporal de almacenamiento del documento migrado "C:\\...\"
    Dim formato_permitido_migracion As String   ' Almacena el formato permitido para migración .PDF   
    Dim formato_archivo_permitido As String     ' Almacena el formato de archivos permitodos para migracion .TIF,.PDF, .BMP,
    Dim version_formato_migracion_pdf As String ' Almacena la version del formato pdf
End Structure
Public Structure mig_config_upload_gestion
    Dim ID_CONFIG_UPLOAD_GESTION As Integer
    Dim EXTENSION_UPLOAD As String
    Dim LENG_UPLOAD As String
    Dim NAME_PROCESO As String
    Dim ESTADO_PROCESO As String
End Structure
Public Class Class_ra_mig_config_migracion
    Function Solicita_estructura_parametro_migracion(ByRef ra_mig_config_migracion As stru_ra_mig_config_migracion) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de configuracion de migracion
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        '                      
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ra_mig_config_migracion : Retorna la estructura de la configuración de 
        '                          migración
        '                                      
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            Sql_consulta = "SELECT id_mig_config_migracion,aplica_ocr,aplica_comprencion,nivel_comprencion,ruta_temporal_migracion," &
            "formato_permitido_migracion,formato_archivo_permitido,version_formato_migracion_pdf from ra_mig_config_migracion"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_mig_config_migracion")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_parametro_migracion = "Error de conexion funcion Solicita_estructura_parametro_migracion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_parametro_migracion = "Imposible encontrar la configuración de migración "
                Exit Function
            Else
                ra_mig_config_migracion.id_mig_config_migracion = Datset.Tables(0).Rows(0).Item(0)
                ra_mig_config_migracion.aplica_ocr = Datset.Tables(0).Rows(0).Item(1)
                ra_mig_config_migracion.aplica_comprencion = Datset.Tables(0).Rows(0).Item(2)
                ra_mig_config_migracion.nivel_comprencion = Datset.Tables(0).Rows(0).Item(3)
                ra_mig_config_migracion.ruta_temporal_migracion = Datset.Tables(0).Rows(0).Item(4)
                ra_mig_config_migracion.formato_permitido_migracion = Datset.Tables(0).Rows(0).Item(5)
                ra_mig_config_migracion.formato_archivo_permitido = Datset.Tables(0).Rows(0).Item(6)
                ra_mig_config_migracion.version_formato_migracion_pdf = Datset.Tables(0).Rows(0).Item(7)
                Solicita_estructura_parametro_migracion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_parametro_migracion = "Inconsistencia general funcion Solicita_estructura_parametro_migracion " & ex.Message
        End Try
    End Function
    Function Solicita_parameter_upload_migracion(ByRef mig_config_upload_gestion As mig_config_upload_gestion) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de configuracion de migracion para upload
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        '                      
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ra_mig_config_migracion : Retorna la estructura de la configuración de 
        '                          migración
        '                                      
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            Sql_consulta = "SELECT leng_load_file," &
            "formato_permitido_migracion from ra_mig_config_migracion"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_mig_config_migracion")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_parameter_upload_migracion = "Error de conexion funcion Solicita_parameter_upload_migracion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_parameter_upload_migracion = "Imposible encontrar la configuración de migración para upload"
                Exit Function
            Else
                mig_config_upload_gestion.LENG_UPLOAD = Val(Datset.Tables(0).Rows(0).Item(0))
                mig_config_upload_gestion.EXTENSION_UPLOAD = Datset.Tables(0).Rows(0).Item(1)
                Solicita_parameter_upload_migracion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_parameter_upload_migracion = "Inconsistencia general funcion Solicita_parameter_upload_migracion " & ex.Message
        End Try
    End Function
End Class
