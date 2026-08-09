Public Class Class_ra_ver_control_version_documento
    Function Solicita_id_registro_control_version(ByVal id_gabinete As Integer,
                                                  ByVal id_imagen As Integer,
                                                  ByRef id_control_version_documento As Long) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita identificación de registro con la identificacion del 
        '          gabinete y la identificacion de la imagen
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Respresenta la identificación de la imagen
        'id_gabinete           : Representa la identificacion del gabinete                        
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_control_version_documento : Retorna la identifcación del control de 
        '                               registro de versión
        '                                    
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_control_version_documento " &
            " FROM ra_ver_control_version_documento " &
            " WHERE system1_id_gabinete=" & id_gabinete & " and id_imagen=" & id_imagen
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_ver_control_version_documento")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_registro_control_version = "Error de conexion funcion  Solicita_id_registro_control_version " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_control_version_documento = 0
                Solicita_id_registro_control_version = "YES"
                Exit Function
            Else
                id_control_version_documento = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_registro_control_version = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_registro_control_version = "Inconsistencia general funcion Solicita_id_registro_control_version " & ex.Message
        End Try
    End Function
    Function Registra_control_version_documento(ByVal id_gabinete As Integer,
                                                ByVal id_imagen As Integer,
                                                ByVal fecha_registro As String,
                                                ByRef id_control_version_documento As Long) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra el control de version de un documento 
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Respresenta la identificación de la imagen
        'id_gabinete           : Representa la identificacion del gabinete                        
        'fecha_registro        : Representa la fecha de registro del control
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_control_version_documento : Retorna la identifcación del control de 
        '                               registro de versión
        '                                    
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim SQL_insert As String = "insert into ra_ver_control_version_documento (system1_id_gabinete,id_imagen," &
                "control_version,fecha_registro_actualizacion) values (" & id_gabinete & "," & id_imagen & ",0," &
                "'" & fecha_registro & "')"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_ver_control_version_documento")
            Result = ref.SELECTION_LAST_INSERT_COMMAND(SQL_insert,
                                                       id_control_version_documento)
            If Result <> "YES" Then
                Registra_control_version_documento = "Error imposible insertar el control de registro funcion  Registra_control_version_documento error " & Result
                Exit Function
            Else
                Registra_control_version_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_control_version_documento = "Inconsistencia general función Registra_control_version_documento " & ex.Message
        End Try
    End Function
End Class
