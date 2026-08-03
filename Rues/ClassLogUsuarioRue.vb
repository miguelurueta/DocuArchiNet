Public Class ClassLogUsuarioRue
    Function RegistraLogUsuarioRue(ByVal rue_camara As CdRues,
                                   ByVal TranSac As String,
                                   ByVal id_imagen As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Registra log de consulta expediente rue
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'rue_camara          : Representa la estructura 
        'TranSac             : Representa el tipo de transación
        'id_imagen           : Representa la imagen
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim RefclasAlama As New ClassAlmacenamiento
            Dim time1al As String = Date.Now.ToString
            Dim date1al As String = Date.Today
            Dim TempoFecha As String = Left(time1al, 10)
            time1al = Trim(time1al.Replace(TempoFecha, ""))
            Dim Result As String = ""
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                RegistraLogUsuarioRue = "Error formateando fecha  RegistraLogUsuarioRue " & Result
                Exit Function
            End If
            Dim hor As New System.DateTime
            hor = Date.Now
            Dim hora As String = hor.Hour.ToString & ":" & hor.Minute.ToString & ":" & hor.Second.ToString
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim sql_insert As String = "Insert into log_usuario_rue (NOMBRE_USUARIO,IDENTIFICACION_USUARIO,EMAIL_USUARIO,DESCRIPCION_TRANSACCION,FECHA_TRANSACCION," _
                                       & "IP_TRANSACCION,HORA_REGISTRO,MODULO_REGISTRO,EXPEDIENTE_AFECTADO,NIT_ENTIDAD,NOMBRE_ENTIDAD,MUNICIPIO_ENTIDAD,ID_IMAGEN) values (" &
                                       "'" & rue_camara.nombreUsuario & "','" & rue_camara.identificacionusuario & "','" & rue_camara.emailUsuario & "','" &
                                       rue_camara.tipoRegistro & "','" & date1al & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" &
                                       hora & "','" & TranSac & "','" & Val(rue_camara.expediente) & "','" & rue_camara.nitEntidad & "','" & rue_camara.nombreEntidad &
                                       "','" & rue_camara.municipioEntidad & "','" & id_imagen & "')"
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                RegistraLogUsuarioRue = "Error registrando  log para el control de auditoría "
                Exit Function
            End If
            RegistraLogUsuarioRue = "YES"
        Catch ex As Exception
            RegistraLogUsuarioRue = "Inconsistencia general fución RegistraLogUsuarioRue " & ex.Message
        End Try

    End Function
End Class
