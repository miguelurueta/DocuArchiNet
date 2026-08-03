Public Class CStruSiiCahcheVinculacion
    Property id_sii_cache_vinculacion As Integer
    Property RadicadoSII As String
    Property CodigoBarras As String
    Property Matricula As String
    Property NombreGabinete As String
    Property IdExpediente As Integer
    Property FechaRegistroCache As String
    Property ErrorService As String
End Class
Public Class CdefaultSiiCahcheVinculacion
    Property CStruSiiCahcheVinculacion As List(Of CStruSiiCahcheVinculacion)
    Property id_sii_cache_vinculacion As Integer
    Property ErrorService As String
End Class
Public Class ClassRaSiiCacheVinculacion
    Function EliminaCahcheVinculacionSII(ByVal RadicadoSII As String) As String
        Try
            Dim Result As String = ""
            Dim ConexioDB As New conect.Dbase_Conction_Mysql_DA
            Dim SQLdelete As String = "Delete from ra_sii_cache_vinculacion where RadicadoSII='" & RadicadoSII & "'"
            Result = ConexioDB.SELECTION_DELETE_COMMAND(SQLdelete)
            EliminaCahcheVinculacionSII = Result
            Exit Function
        Catch ex As Exception
            EliminaCahcheVinculacionSII = "Inconsistencia general funcion EliminaCahcheVinculacionSII " & ex.Message
        End Try
    End Function
    Function RegistraCahcheVinculacionSII(ByVal CStruSiiCahcheVinculacion As CStruSiiCahcheVinculacion,
                                          ByRef IdraSiiCacheVicnculacion As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Inserta el cache registro expediente SII auto creacion
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'StruSiiCahcheInscripcion   : Representa la estructura del registro de inscripción
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-19
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim FechaRegistroCache As String = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(FechaRegistroCache)
            Dim SQLInsertInto As String = "Insert into  ra_sii_cache_vinculacion (RadicadoSII,CodigoBarras,NombreGabinete," &
                  "Matricula,IdExpediente,FechaRegistroCache) values ('" & CStruSiiCahcheVinculacion.RadicadoSII & "','" & CStruSiiCahcheVinculacion.CodigoBarras & "','" &
                   CStruSiiCahcheVinculacion.NombreGabinete & "','" & CStruSiiCahcheVinculacion.Matricula & "'," & CStruSiiCahcheVinculacion.IdExpediente &
                   ",'" & FechaRegistroCache & "')"
            Dim ConexionDB As New conect.Dbase_Conction_Mysql_DA
            Result = ConexionDB.SELECTION_LAST_INSERT_COMMAND(SQLInsertInto, IdraSiiCacheVicnculacion)
            If Result <> "YES" Then
                RegistraCahcheVinculacionSII = "Error funcion RegistraCahcheVinculacionSII " & Result
                Exit Function
            Else
                RegistraCahcheVinculacionSII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            RegistraCahcheVinculacionSII = "Inconsistencia feneral fuction RegistraCahcheVinculacionSII " & ex.Message
        End Try
    End Function
    Function SolicitaCahcheVinculacionSII(ByVal RadicadoSII As String,
                                          ByRef CStruSiiCahcheVinculacion As CStruSiiCahcheVinculacion) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura cache vinculación docuentos a expediente integración SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RadicadoSII          : Representa el recibo de caja de integración SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CStruSiiCahcheVinculacion   : Representa la estructura del registro de vinculación
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_sii_cache_vinculacion")
            Dim SQLconsulta As String = "Select id_sii_cache_vinculacion,RadicadoSII," &
            "CodigoBarras,NombreGabinete,Matricula,IdExpediente" &
            " from  ra_sii_cache_vinculacion " &
            " where RadicadoSII='" & RadicadoSII & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaCahcheVinculacionSII = "Error funcion SolicitaCacheCreacionExpedienteSII " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaCahcheVinculacionSII = "YES"
                Exit Function
            Else
                CStruSiiCahcheVinculacion.id_sii_cache_vinculacion = Datset.Tables(0).Rows(0).Item("id_sii_cache_vinculacion")
                CStruSiiCahcheVinculacion.RadicadoSII = Datset.Tables(0).Rows(0).Item("RadicadoSII")
                CStruSiiCahcheVinculacion.CodigoBarras = Datset.Tables(0).Rows(0).Item("CodigoBarras")
                CStruSiiCahcheVinculacion.NombreGabinete = Datset.Tables(0).Rows(0).Item("NombreGabinete")
                CStruSiiCahcheVinculacion.Matricula = Datset.Tables(0).Rows(0).Item("Matricula")
                CStruSiiCahcheVinculacion.IdExpediente = Datset.Tables(0).Rows(0).Item("IdExpediente")
                SolicitaCahcheVinculacionSII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaCahcheVinculacionSII = "Inconsistencia general función SolicitaCahcheVinculacionSII " & ex.Message
        End Try
    End Function
End Class
