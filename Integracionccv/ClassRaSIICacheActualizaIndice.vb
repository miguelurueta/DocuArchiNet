Public Class CcacheIndiceSII
    Property id_ra_sii_cache_actualiza_indice As Integer
    Property RadicadoSII As String
    Property CodBarras As String
    Property NombreGabinete As String
    Property Matricula As String
    Property FechaRegistroActualizaIndice As String
End Class

Public Class ClassRaSIICacheActualizaIndice
    Function ActualizaIndiceDocumentosSII(ByVal IdTramite As Integer,
                                          ByVal ReciboSII As String,
                                          ByVal CIncripcionSII As List(Of CIncripcionSII)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza los indices de documentos en la integración con el sistema SII
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTramite           : Representa la identiifcación del tramite
        'ReciboSII           : Representa el consecutivo de recibo de caja del SII
        'CIncripcionSII      : Representa la estructura de las inscripciones
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-25
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim ClassRaSIICacheActualizaIndice As New ClassRaSIICacheActualizaIndice
            Dim CcacheIndiceSII As CcacheIndiceSII = Nothing
            Result = ClassRaSIICacheActualizaIndice.SolicitaCacheIndiceSIIRadicado(ReciboSII,
                                                                                   CcacheIndiceSII)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosSII = Result
                Exit Function
            End If
            If Not CcacheIndiceSII Is Nothing Then
                ActualizaIndiceDocumentosSII = "YES"
                Exit Function
            End If
            Dim ClassConsultaExpedienteSII As New ClassConsultaExpedienteSII
            Dim CTipoDocEntrante As New CTipoDocEntrante
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Class_tipo_doc_entrante.SolicitaEstructuraTramite(IdTramite,
                                                                       CTipoDocEntrante)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosSII = Result
                Exit Function
            End If
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            Dim CStruSiiCahcheExpediente As New List(Of CStruSiiCahcheExpediente)
            Result = ClassRaSIiCacheExpediente.SolicitaCacheCreacionExepdienteSiiRadicado(ReciboSII,
                                                                                          0,
                                                                                          CStruSiiCahcheExpediente)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosSII = Result
                Exit Function
            End If

            If CStruSiiCahcheExpediente.Count > 0 Then
                '//-------Actualiza indice para dcumentos de multiplex expedientes SII condicionado por el identificador del expediente--------///
                Dim IcuntCache As Integer = CStruSiiCahcheExpediente.Count
                If IcuntCache > 2 Then
                    IcuntCache = 2
                End If
                For i As Integer = 0 To IcuntCache - 1
                    Result = ClassDaGabinete.ActualizaIndiceDocumentoCacheExpediente(CTipoDocEntrante.nombre_gabinete_workflow,
                                                                                     CStruSiiCahcheExpediente(i))
                    If Result <> "YES" Then
                        ActualizaIndiceDocumentosSII = Result
                        Exit Function
                    End If
                Next
            Else
                Dim Matricula As String = CIncripcionSII(0).MATRICULA_SII
                Dim Proponente As String = CIncripcionSII(0).PROPONENTE_SII
                Dim StruSiiCahcheInscripcion As StruSiiCahcheInscripcion = Nothing
                '//----------Consulta los datos nombre, nit, matricula del expedientes en el SII----//
                Result = ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(Matricula,
                                                                                    Proponente,
                                                                                    CTipoDocEntrante.nombre_gabinete_workflow,
                                                                                    StruSiiCahcheInscripcion)
                If Result <> "YES" Then
                    ActualizaIndiceDocumentosSII = Result
                    Exit Function
                End If
                StruSiiCahcheInscripcion.RadicadoSII = ReciboSII
                Result = ClassDaGabinete.ActualizaIndiceDocumentoIntegracionSII(CTipoDocEntrante.nombre_gabinete_workflow,
                                                                                StruSiiCahcheInscripcion)
                If Result <> "YES" Then
                    ActualizaIndiceDocumentosSII = Result
                    Exit Function
                End If
            End If
            '//------------Registro cache actualización de indice-----////
            CcacheIndiceSII = New CcacheIndiceSII
            CcacheIndiceSII.CodBarras = CIncripcionSII(0).COD_BARRA_SII
            CcacheIndiceSII.Matricula = CIncripcionSII(0).MATRICULA_SII
            CcacheIndiceSII.RadicadoSII = ReciboSII
            CcacheIndiceSII.NombreGabinete = CTipoDocEntrante.nombre_gabinete_workflow
            Result = ClassRaSIICacheActualizaIndice.InsertarCacheIndiceSII(CcacheIndiceSII)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosSII = Result
                Exit Function
            End If
            ActualizaIndiceDocumentosSII = "YES"
            Exit Function
        Catch ex As Exception
            ActualizaIndiceDocumentosSII = "Inconsistencia general funcion ActualizaIndiceDocumentosSII " & ex.Message
        End Try
    End Function
    Function InsertarCacheIndiceSII(ByVal CcacheIndiceSII As CcacheIndiceSII) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Inserta el cache de actualización de indice SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CcacheIndiceSII           : Representa la estructura del registro de actualizacióon de indice
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-24
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim FechaRegistroCache As String = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(FechaRegistroCache)
            Dim SQLInsertInto As String = "Insert into  ra_sii_cache_actualiza_indice (RadicadoSII,CodBarras,NombreGabinete," &
              "FechaRegistroActualizaIndice,Matricula) values ('" & CcacheIndiceSII.RadicadoSII & "','" & CcacheIndiceSII.CodBarras & "','" &
               CcacheIndiceSII.NombreGabinete & "','" & FechaRegistroCache & "','" & CcacheIndiceSII.Matricula & "')"
            Dim ConexionDB As New conect.Dbase_Conction_Mysql_DA
            Result = ConexionDB.SELECTION_INSERT_COMMAND(SQLInsertInto)
            If Result <> "YES" Then
                InsertarCacheIndiceSII = "Error funcion InsertarCacheIndiceSII " & Result
                Exit Function
            Else
                InsertarCacheIndiceSII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            InsertarCacheIndiceSII = "Inconsistencia general function InsertarCacheIndiceSII " & ex.Message
        End Try
    End Function
    Function SolicitaCacheIndiceSIIRadicado(ByVal ReciboSII As String,
                                            ByRef CcacheIndiceSII As CcacheIndiceSII) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el cache de actualización de indice de documentos con la integracion SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'ReciboSII           : Representa el consecutivo de recibo del SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CcacheIndiceSII  : Retorna la estructura con el registro actualización de indice
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim DataBaseConexion As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_sii_cache_actualiza_indice")
            Dim SQLconsulta As String = "Select id_ra_sii_cache_actualiza_indice," &
            "RadicadoSII,CodBarras,NombreGabinete,Matricula,FechaRegistroActualizaIndice" &
            " from  ra_sii_cache_actualiza_indice " &
            " where RadicadoSII='" & ReciboSII & "'"
            Dim Result As String = ""
            Result = DataBaseConexion.SELECTION_SELECT_FIELDA(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaCacheIndiceSIIRadicado = "Error funcion SolicitaCacheIndiceSIIRadicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaCacheIndiceSIIRadicado = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    CcacheIndiceSII = New CcacheIndiceSII
                    CcacheIndiceSII.id_ra_sii_cache_actualiza_indice = Datset.Tables(0).Rows(i).Item("id_ra_sii_cache_actualiza_indice")
                    CcacheIndiceSII.RadicadoSII = Datset.Tables(0).Rows(i).Item("RadicadoSII")
                    CcacheIndiceSII.CodBarras = Datset.Tables(0).Rows(i).Item("CodBarras")
                    CcacheIndiceSII.NombreGabinete = Datset.Tables(0).Rows(i).Item("NombreGabinete")
                    CcacheIndiceSII.Matricula = Datset.Tables(0).Rows(i).Item("Matricula")
                    CcacheIndiceSII.FechaRegistroActualizaIndice = Datset.Tables(0).Rows(i).Item("FechaRegistroActualizaIndice")
                Next
                SolicitaCacheIndiceSIIRadicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaCacheIndiceSIIRadicado = "Inconsistencia general funcion SolicitaCacheIndiceSIIRadicado " & ex.Message
        End Try
    End Function
End Class
