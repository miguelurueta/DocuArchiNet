Public Class Class_Ra_gabexp_relacion_index_gabinete_expediente
    Function SolicitaRelacionGabineteDefault(ByVal IdGabinete As Integer,
                                             ByRef IdRelacionExpedienteGabinete As Integer) As String
        '-----------------------------------------------------------------------------------
        'Funcion : Solicita relacion gabinete expediente default
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_gabinete           : Representa la identificacion del gabinete en sistema
        '                      : docuachi contenedor
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'id_relacion_expediente_gabinete  : Retorna la idnetificación del usuario radicador
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2023-06-03
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select id_relacion_gab_expe from ra_gabexp_relacion_index_gabinete_expediente" &
                " where system1_id_gabinete=" & IdGabinete & " and default_relacion=1"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DA_EXTENSION")
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaRelacionGabineteDefault = "Error funcion Solicita_relacion_gabinete_default  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                IdRelacionExpedienteGabinete = -1
                SolicitaRelacionGabineteDefault = "YES"
                Exit Function
            Else
                IdRelacionExpedienteGabinete = Datset.Tables(0).Rows(0).Item(0)
                SolicitaRelacionGabineteDefault = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaRelacionGabineteDefault = "Inconsistencia general función SolicitaRelacionGabineteDefault " & ex.Message
        End Try
    End Function
    Function SolicitaValoresCampoExpedienteParaCampoIndiceGabinete(ByVal IdExpediente As Integer,
                                                                   ByVal IdGabinete As Integer,
                                                                   ByVal EstadoObliga As Integer,
                                                                   ByRef StruRelExpGabinete() As stru_rel_exp_gabinete) As String
        '-----------------------------------------------------------------------------------
        'Funcion : Solicita valor campos gabinetes expedientes 
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_expediente              : Representa la identificación del expediente 
        'id_gabinete                : Representa la identificacion del gabinete
        'estado_obliga              : Representa si obliga la relación expediente gabinete
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'stru_rel_exp_gabinete      : Retorna el valor de de campos y
        '                             gabinetes recuperados desde el expediente
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2023-06-06
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim IdRelacionExpedienteGabinete As Integer = -1
            Result = SolicitaRelacionGabineteDefault(IdGabinete,
                                                     IdRelacionExpedienteGabinete)
            If Result <> "YES" Then
                SolicitaValoresCampoExpedienteParaCampoIndiceGabinete = Result
                Exit Function
            End If
            If IdRelacionExpedienteGabinete = -1 Then
                If EstadoObliga = 1 Then
                    SolicitaValoresCampoExpedienteParaCampoIndiceGabinete = "Debe establecer una relación entre el expediente y el gabinete para construir el indice del documento, por favor contacte a su administrador"
                    Exit Function
                Else
                    SolicitaValoresCampoExpedienteParaCampoIndiceGabinete = "YES"
                    Exit Function
                End If
            End If
            Dim Class_ra_gabexp_relacion_campos_gabinete_expediente As New Class_ra_gabexp_relacion_campos_gabinete_expediente
            Result = Class_ra_gabexp_relacion_campos_gabinete_expediente.SolicitaEstructuraRelacionGabineteExpediente(IdRelacionExpedienteGabinete,
                                                                                                                      StruRelExpGabinete)
            If Result <> "YES" Then
                SolicitaValoresCampoExpedienteParaCampoIndiceGabinete = Result
                Exit Function
            End If
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaValoresCampoGabineteCampoExpediente(IdExpediente,
                                                                                   StruRelExpGabinete)
            If Result <> "YES" Then
                SolicitaValoresCampoExpedienteParaCampoIndiceGabinete = Result
                Exit Function
            Else
                SolicitaValoresCampoExpedienteParaCampoIndiceGabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaValoresCampoExpedienteParaCampoIndiceGabinete = "Inconsistencia general funcion SolicitaValoresCampoExpedienteParaCampoIndiceGabinete " & ex.Message
        End Try
    End Function
End Class
