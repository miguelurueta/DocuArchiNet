Public Structure StruSiiCahcheInscripcion
    Dim id_sii_cahche_inscripcion As Integer
    Dim RadicadoSII As String
    Dim CodBarras As String
    Dim NitIdentificacion As String
    Dim Rsocial As String
    Dim NombreGabinete As String
    Dim EstadoActualizacionIndice As Integer
    Dim Matricula As String
    Dim NombrePropietario As String
    Dim Identificacionpro As String
    Dim MatriculaPropietario As String
End Structure
Public Class CacheInscripcion
    Property id_sii_cahche_inscripcion As Integer
    Property RadicadoSII As String
    Property CodBarras As String
    Property NitIdentificacion As String
    Property Rsocial As String
    Property NombreGabinete As String
    Property EstadoActualizacionIndice As Integer
    Property Matricula As String
    Property NombrePropietario As String
    Property Identificacionpro As String
    Property MatriculaPropietario As String
End Class
Public Class CcacheIncripcionSIIResult
    Property AppError As String
    Property CahcheInscripcion As List(Of CacheInscripcion)
End Class
Public Class ClassRaSiiCahcheInscripcion

    Function RegistraCacheInscripcionRadicadoSII(ByVal StruSiiCahcheInscripcion As StruSiiCahcheInscripcion) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Inserta el cache de inscripción SII  para la actualización de los indices
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
        'Fecha                 : 2025-03-31
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim NitIdentificacion As String = ""
            If StruSiiCahcheInscripcion.NitIdentificacion <> "" Then
                NitIdentificacion = StruSiiCahcheInscripcion.NitIdentificacion
            End If
            Dim Rsocial As String = ""
            If StruSiiCahcheInscripcion.Rsocial <> "" Then
                Rsocial = Left(StruSiiCahcheInscripcion.Rsocial, 40)
                Rsocial = Rsocial.Replace("'", "")
                Rsocial = Rsocial.Replace("´", "")
            End If
            Dim SQLInsertInto As String = "Insert into  ra_sii_cahche_inscripcion (RadicadoSII,CodBarras,NitIdentificacion,Rsocial,NombreGabinete," &
              "EstadoActualizacionIndice,Matricula) values ('" & StruSiiCahcheInscripcion.RadicadoSII & "','" & StruSiiCahcheInscripcion.CodBarras & "','" & NitIdentificacion & "','" &
              Rsocial & "','" & StruSiiCahcheInscripcion.NombreGabinete & "',0,'" & StruSiiCahcheInscripcion.Matricula & "')"
            Dim Result As String = ""
            Dim ConexionDB As New conect.Dbase_Conction_Mysql_DA
            Result = ConexionDB.SELECTION_INSERT_COMMAND(SQLInsertInto)
            If Result <> "YES" Then
                RegistraCacheInscripcionRadicadoSII = "Error funcion InsertarCacheInscripcionRadicado " & Result
                Exit Function
            Else
                RegistraCacheInscripcionRadicadoSII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            RegistraCacheInscripcionRadicadoSII = "Inconsistencia general function RegistraCacheInscripcionRadicadoSII " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraCacheInscripcionRadicado(ByVal RadicadoSII As String,
                                                        ByRef CacheInscripcion As CacheInscripcion) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura cache de la inscripcion de un radicado SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RadicadoSII          : Representa el consecutivo radicado de integración SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruSiiCahcheInscripcion   : Representa la estructura del registro de inscripción
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-03-31
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_sii_cahche_inscripcion")
            Dim SQLconsulta As String = "Select id_sii_cahche_inscripcion,RadicadoSII," &
            "CodBarras,NitIdentificacion,Rsocial,NombreGabinete,EstadoActualizacionIndice,Matricula" &
            " from  ra_sii_cahche_inscripcion " &
            " where RadicadoSII='" & RadicadoSII & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructuraCacheInscripcionRadicado = "Error funcion SolicitaEstructuraCacheInscripcionRadicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                CacheInscripcion = Nothing
                SolicitaEstructuraCacheInscripcionRadicado = "YES"
                Exit Function
            Else
                CacheInscripcion.id_sii_cahche_inscripcion = Datset.Tables(0).Rows(0).Item("id_sii_cahche_inscripcion")
                CacheInscripcion.RadicadoSII = Datset.Tables(0).Rows(0).Item("RadicadoSII")
                CacheInscripcion.RadicadoSII = Datset.Tables(0).Rows(0).Item("RadicadoSII")
                CacheInscripcion.CodBarras = Datset.Tables(0).Rows(0).Item("CodBarras")
                If Datset.Tables(0).Rows(0).IsNull("NitIdentificacion") = True Then
                    CacheInscripcion.NitIdentificacion = ""
                Else
                    CacheInscripcion.NitIdentificacion = Datset.Tables(0).Rows(0).Item("NitIdentificacion")
                End If
                CacheInscripcion.Rsocial = Datset.Tables(0).Rows(0).Item("Rsocial")
                CacheInscripcion.NombreGabinete = Datset.Tables(0).Rows(0).Item("NombreGabinete")
                CacheInscripcion.EstadoActualizacionIndice = Datset.Tables(0).Rows(0).Item("EstadoActualizacionIndice")
                CacheInscripcion.Matricula = Datset.Tables(0).Rows(0).Item("Matricula")
                SolicitaEstructuraCacheInscripcionRadicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstructuraCacheInscripcionRadicado = "Inconsistencia general función SolicitaEstructuraCacheInscripcionRadicado " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraCacheInscripcionRadicado(ByVal RadicadoSII As String,
                                                        ByRef StruSiiCahcheInscripcion As StruSiiCahcheInscripcion) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura cache de la inscripcion de un radicado SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RadicadoSII          : Representa el consecutivo radicado de integración SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruSiiCahcheInscripcion   : Representa la estructura del registro de inscripción
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-03-31
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_sii_cahche_inscripcion")
            Dim SQLconsulta As String = "Select id_sii_cahche_inscripcion,RadicadoSII," &
            "CodBarras,NitIdentificacion,Rsocial,NombreGabinete,EstadoActualizacionIndice,Matricula" &
            " from  ra_sii_cahche_inscripcion " &
            " where RadicadoSII='" & RadicadoSII & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructuraCacheInscripcionRadicado = "Error funcion SolicitaEstructuraCacheInscripcionRadicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                StruSiiCahcheInscripcion = Nothing
                SolicitaEstructuraCacheInscripcionRadicado = "YES"
                Exit Function
            Else
                StruSiiCahcheInscripcion.id_sii_cahche_inscripcion = Datset.Tables(0).Rows(0).Item("id_sii_cahche_inscripcion")
                StruSiiCahcheInscripcion.RadicadoSII = Datset.Tables(0).Rows(0).Item("RadicadoSII")
                StruSiiCahcheInscripcion.RadicadoSII = Datset.Tables(0).Rows(0).Item("RadicadoSII")
                StruSiiCahcheInscripcion.CodBarras = Datset.Tables(0).Rows(0).Item("CodBarras")
                If Datset.Tables(0).Rows(0).IsNull("NitIdentificacion") = True Then
                    StruSiiCahcheInscripcion.NitIdentificacion = ""
                Else
                    StruSiiCahcheInscripcion.NitIdentificacion = Datset.Tables(0).Rows(0).Item("NitIdentificacion")
                End If
                StruSiiCahcheInscripcion.Rsocial = Datset.Tables(0).Rows(0).Item("Rsocial")
                StruSiiCahcheInscripcion.NombreGabinete = Datset.Tables(0).Rows(0).Item("NombreGabinete")
                StruSiiCahcheInscripcion.EstadoActualizacionIndice = Datset.Tables(0).Rows(0).Item("EstadoActualizacionIndice")
                StruSiiCahcheInscripcion.Matricula = Datset.Tables(0).Rows(0).Item("Matricula")
                SolicitaEstructuraCacheInscripcionRadicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstructuraCacheInscripcionRadicado = "Inconsistencia general función SolicitaEstructuraCacheInscripcionRadicado " & ex.Message
        End Try
    End Function
End Class
