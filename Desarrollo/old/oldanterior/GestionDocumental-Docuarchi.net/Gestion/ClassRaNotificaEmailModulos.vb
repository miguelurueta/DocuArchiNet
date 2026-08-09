Public Class ClassRaNotificaEmailModulos
    Function SolicitaEmailNofiticacionModulo(ByVal NameModuloAPP As String,
                                             ByRef EmailNotificacion As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita datos de notificación de correo de modulos
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NameModuloAPP           : Representa el nombre del modulo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EmailNotificacion  : Retorna la lista de email separados por coma
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim SQLconsulta As String = "select EmailNotificacion from ra_notifica_email_modulos where NameModuloAPP='" & NameModuloAPP & "'"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_notifica_email_modulos")
            Result = ref.SELECTION_SELECT_FIELDA(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaEmailNofiticacionModulo = " Función SolicitaEmailNofiticacionModulo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEmailNofiticacionModulo = "Imposible encontrar los correos de notificación para el módulo (" & NameModuloAPP & ")"
                Exit Function
            Else
                EmailNotificacion = Datset.Tables(0).Rows(0).Item(0).ToString
                SolicitaEmailNofiticacionModulo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEmailNofiticacionModulo = "Inconsistencia general funcion SolicitaEmailNofiticacionModulo " & ex.Message
        End Try
    End Function
End Class
