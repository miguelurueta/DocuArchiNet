Public Class Class_ra_config_notifica_correo
    Function Solicita_estado_ruta_notificacion(ByRef ruta_web_server As String, _
                                               ByRef estado_tipo_notificacion As Integer, _
                                               ByRef correo_copia As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " Select url_web_servidor,tipo_notificacion,dir_corroe_notifiacion_copia " &
               " FROM  ra_config_notifica_correo  "
            Dim Datset As DataSet = New DataSet("ra_config_notifica_correo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_ruta_notificacion = "Función Solicita_estado_ruta_notificacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_ruta_notificacion = "Imposible encontrar el registro de configuración para el tipo de notificación de correo electrónico con el modelo de archivos en link"
                Exit Function
            Else
                ruta_web_server = Datset.Tables(0).Rows(0).Item(0)
                estado_tipo_notificacion = Datset.Tables(0).Rows(0).Item(1)
                correo_copia = Datset.Tables(0).Rows(0).Item(2)
                Solicita_estado_ruta_notificacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_ruta_notificacion = "Inconsistencia general función Solicita_estado_ruta_notificacion " & ex.Message
        End Try
    End Function
End Class
