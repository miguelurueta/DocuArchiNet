Public Class ClassRaRelacionRadicadoExternoExpediente
    Function RegistraValidaRelacionExpedienteRadicadoExterno(ByVal Matricula As String,
                                                             ByVal NombreGabinete As String,
                                                             ByVal RadicadoExterno As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Registra la relación de un radicado externo con un expediente con validación de 
        '          existencia
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        'Matricula           : Representa la identificación de la matricula del matriculado SII
        'NombreGabinete      : Representa el nombre del gabinete
        'RadicadoExterno     : Representa la identificación de un radicado externo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            Dim CStruSiiCahcheExpediente As CStruSiiCahcheExpediente = New CStruSiiCahcheExpediente
            Dim Result As String = ""
            Dim IdExpediente As Integer = 0
            If Matricula <> "" Then
                Matricula = Matricula.Replace("S0", "")
            End If
            '// Solicita exitencia del cache de expediente integración SII //
            Result = ClassRaSIiCacheExpediente.SolicitaCacheCreacionExpedienteSII(Matricula,
                                                                                  NombreGabinete,
                                                                                  CStruSiiCahcheExpediente)
            If Result <> "YES" Then
                RegistraValidaRelacionExpedienteRadicadoExterno = Result
                Exit Function
            End If
            '// Sale de la función si no existe un registro de expediente--////
            If CStruSiiCahcheExpediente.Matricula Is Nothing Then
                RegistraValidaRelacionExpedienteRadicadoExterno = "YES"
                Exit Function
            End If
            IdExpediente = CStruSiiCahcheExpediente.IdExpediente
            '// Solicita exitencia de la relación de un radicado externo y un expediente //
            Dim IdExpedienteExistente As Integer = 0
            Result = SolicitaExpedienteRadicadoExterno(RadicadoExterno,
                                                       IdExpedienteExistente)
            If Result <> "YES" Then
                RegistraValidaRelacionExpedienteRadicadoExterno = Result
                Exit Function
            End If
            '// Sale de la función si no existe un registro de relacion previo--////
            If IdExpedienteExistente <> 0 Then
                RegistraValidaRelacionExpedienteRadicadoExterno = "YES"
                Exit Function
            End If
            Dim FechaRegistroCache As String = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            Result = ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(FechaRegistroCache)
            If Result <> "YES" Then
                RegistraValidaRelacionExpedienteRadicadoExterno = Result
                Exit Function
            End If
            Result = RegistraRelacionExpedienteRadicadoExterno(RadicadoExterno,
                                                               IdExpediente,
                                                               FechaRegistroCache)
            RegistraValidaRelacionExpedienteRadicadoExterno = Result
            Exit Function
        Catch ex As Exception
            RegistraValidaRelacionExpedienteRadicadoExterno = "Inconsistencia general funcion RegistraValidaRelacionExpedienteRadicadoExterno " & ex.Message
        End Try
    End Function
    Function RegistraRelacionExpedienteRadicadoExterno(ByVal RadicadoExterno As String,
                                                       ByVal IdExpediente As Integer,
                                                       ByVal FechaRegistroCache As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Registra la relación de un radicado externo con un expediente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RadicadoExterno     : Representa la identificación de un radicado externo
        'IdExpediente        : Representa la identificación del expediente
        'FechaRegistroCache  : Representa la fecha de registro de la relación
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim SqlInsertRelacion As String = "Insert into ra_relacion_radicado_externo_expediente (expediente_archivo_ID_EXPEDIENTE,RadicadoExterno,FechaRegistro) values " &
              " (" & IdExpediente & ",'" & RadicadoExterno & "','" & FechaRegistroCache & "')"
            Dim ConexionDB As New conect.Dbase_Conction_Mysql_DA
            Dim Result As String = ""
            Result = ConexionDB.SELECTION_INSERT_COMMAND(SqlInsertRelacion)
            RegistraRelacionExpedienteRadicadoExterno = Result
            Exit Function
        Catch ex As Exception
            RegistraRelacionExpedienteRadicadoExterno = "Inconsistencia general funcion RegistraRelacionExpedienteRadicadoExterno " & ex.Message
        End Try
    End Function
    Function EliminaRelacionExpedienteRadicadoExterno(ByVal RadicadoExterno As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Registra la relación de un radicado externo con un expediente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RadicadoExterno     : Representa la identificación de un radicado externo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim SqlDeletetRelacion As String = "Delete from ra_relacion_radicado_externo_expediente  where RadicadoExterno='" &
              RadicadoExterno & "'"
            Dim ConexionDB As New conect.Dbase_Conction_Mysql_DA
            Dim Result As String = ""
            Result = ConexionDB.SELECTION_DELETE_COMMAND(SqlDeletetRelacion)
            EliminaRelacionExpedienteRadicadoExterno = Result
            Exit Function
        Catch ex As Exception
            EliminaRelacionExpedienteRadicadoExterno = "Inconsistencia general funcion EliminaRelacionExpedienteRadicadoExterno " & ex.Message
        End Try
    End Function
    Function SolicitaExpedienteRadicadoExterno(ByVal RadicadoExterno As String,
                                               ByRef IdExpediente As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identificación del expediente relacionado a un radciado externo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RadicadoExterno     : Representa la identificación de un radicado externo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdExpediente  : Retorna el identificador de un expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-27
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_relacion_radicado_externo_expediente")
            Dim SQLconsulta As String = "Select expediente_archivo_ID_EXPEDIENTE " &
            " from  ra_relacion_radicado_externo_expediente " &
            " where RadicadoExterno='" & RadicadoExterno & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaExpedienteRadicadoExterno = "Error funcion SolicitaExpedienteRadicadoExterno " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                IdExpediente = 0
                SolicitaExpedienteRadicadoExterno = "YES"
                Exit Function
            Else
                IdExpediente = Datset.Tables(0).Rows(0).Item("expediente_archivo_ID_EXPEDIENTE")
                SolicitaExpedienteRadicadoExterno = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaExpedienteRadicadoExterno = "Inconsistencia general función SolicitaExpedienteRadicadoExterno " & ex.Message
        End Try
    End Function
End Class
