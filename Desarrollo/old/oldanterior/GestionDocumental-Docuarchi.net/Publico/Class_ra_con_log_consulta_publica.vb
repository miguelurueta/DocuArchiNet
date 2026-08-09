Public Structure ra_con_log_consulta_publica
    Dim Id_registro_log_consulta As Integer
    Dim ra_con_usuario_consulta_publica_id_registro_usuario As Integer
    Dim matricula As String
    Dim gabinete As String
    Dim id_imagen As Integer
    Dim operacion As String
    Dim ip_host As String
End Structure
Public Class Class_ra_con_log_consulta_publica

    Function Registro_log_consulta_publica_expediente(ByVal Ra_con_log_consulta_publica As ra_con_log_consulta_publica,
                                                      ByRef Id_registro_log_consulta As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra log consulta publica 
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Ra_con_log_consulta_publica  : Representa la estructura del registro log
        '                             : de la consulta publica
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Id_registro_log_consulta : Retorna la idnetificación del log 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim time1al As String = Date.Now.ToString
            ref_ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
            Dim SQL_insert As String = "Insert into ra_con_log_consulta_publica (ra_con_usuario_consulta_publica_id_registro_usuario," &
                "matricula,gabinete,id_imagen,operacion,ip_host,fecha_registro_log) values (" &
                Ra_con_log_consulta_publica.ra_con_usuario_consulta_publica_id_registro_usuario & ",'" &
                Ra_con_log_consulta_publica.matricula & "','" & Ra_con_log_consulta_publica.gabinete & "'," &
                Ra_con_log_consulta_publica.id_imagen & ",'" & Ra_con_log_consulta_publica.operacion & "','" &
                Ra_con_log_consulta_publica.ip_host & "','" & time1al & "')"
            Dim Result As String = ref.SELECTION_LAST_INSERT_COMMAND(SQL_insert,
                                                                     Id_registro_log_consulta)
            Registro_log_consulta_publica_expediente = Result
            Exit Function
        Catch ex As Exception
            Registro_log_consulta_publica_expediente = "Inconsistencia general funcion Registro_log_consulta_publica_expediente " & ex.Message
        End Try
    End Function
End Class
