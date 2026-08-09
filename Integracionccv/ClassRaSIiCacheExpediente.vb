Imports MySql.Data.MySqlClient

Public Class CStruSiiCahcheExpediente
    Property id_ra_sii_cache_exepediente As Integer
    Property RadicadoSII As String
    Property CodBarras As String
    Property NitIdentificacion As String
    Property Rsocial As String
    Property NombreGabinete As String
    Property Matricula As String
    Property EstadoVinculaDocumento As Integer
    Property IdExpediente As Integer
    Property EstadoPadre As Integer
    Property FechaRegistroCache As String
    Property ErrorService As String
End Class
Public Class CdefaultSiiCacheSII
    Property CStruSiiCahcheExpediente As List(Of CStruSiiCahcheExpediente)
    Property id_ra_sii_cache_exepediente As Integer
    Property ErrorService As String
End Class
Public Class ClassRaSIiCacheExpediente
    Function ActualizaEstadoVinculacionDocumentoSII(ByVal IdExpediente As Integer) As String
        Try
            '-----------------------------------------------------------------------------------------------
            'Funcion : Actualiza estado documentos vinculados al expedediente en el cache sii
            '-----------------------------------------------------------------------------------------------
            '                           PARAMETROS  
            '-----------------------------------------------------------------------------------------------
            'IdExpediente   : Representa la identificacion del expediente
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
            Dim Result As String = ""
            Dim SQLupdate As String = "update ra_sii_cache_exepediente set EstadoVinculaDocumento=1 where IdExpediente=" & IdExpediente
            Dim ConexionDB As New conect.Dbase_Conction_Mysql_DA
            Result = ConexionDB.SELECTION_INSERT_COMMAND(SQLupdate)
            ActualizaEstadoVinculacionDocumentoSII = Result
            Exit Function
        Catch ex As Exception
            ActualizaEstadoVinculacionDocumentoSII = "Inconsistencia general funcion ActualizaEstadoVinculacionDocumentoSII " & ex.Message
        End Try
    End Function
    Function SolicitaRegistroExpedienteMatricula(ByVal IdTramite As Integer,
                                                 ByVal SCIncripcionSII As List(Of CIncripcionSII),
                                                 ByRef CStruSiiCahcheExpediente As CStruSiiCahcheExpediente) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura del registro de la creción del expediente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CIncripcionSII        : Representa la estructura con la inscripción
        'IdTramite             : Representa la identificación del tramite
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CStruSiiCahcheExpediente   : Representa la estructura del registro del expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim CTipoDocEntrante As New CTipoDocEntrante
            Result = Class_tipo_doc_entrante.SolicitaEstructuraTramite(IdTramite,
                                                                       CTipoDocEntrante)

            If Result <> "YES" Then
                SolicitaRegistroExpedienteMatricula = Result
                Exit Function
            End If
            Dim ClassConsultaExpedienteSII As New ClassConsultaExpedienteSII
            Dim StruSiiCahcheInscripcion As StruSiiCahcheInscripcion = Nothing
            Result = ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(SCIncripcionSII.Item(0).MATRICULA_SII,
                                                                                SCIncripcionSII.Item(0).PROPONENTE_SII,
                                                                                CTipoDocEntrante.nombre_gabinete_workflow,
                                                                                StruSiiCahcheInscripcion)
            If Result <> "YES" Then
                SolicitaRegistroExpedienteMatricula = Result
                Exit Function
            End If
            Dim MatricuaSII As String = ""
            '//-------------Valida  la matricula padre en de la inscripción SII -------////
            If StruSiiCahcheInscripcion.MatriculaPropietario = "" Then
                MatricuaSII = StruSiiCahcheInscripcion.Matricula
            Else
                MatricuaSII = StruSiiCahcheInscripcion.MatriculaPropietario
            End If
            MatricuaSII = MatricuaSII.Replace("S0", "")
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            Result = ClassRaSIiCacheExpediente.SolicitaCacheCreacionExpedienteSII(MatricuaSII,
                                                                                 CTipoDocEntrante.nombre_gabinete_workflow,
                                                                                 CStruSiiCahcheExpediente)
            SolicitaRegistroExpedienteMatricula = Result
            Exit Function
        Catch ex As Exception
            SolicitaRegistroExpedienteMatricula = "Inconsistencia general funcion SolicitaRegistroExpedienteMatricula " & ex.Message
        End Try
    End Function
    Function RegistraCacheCreacionExpedienteSII(ByVal CStruSiiCahcheExpediente As CStruSiiCahcheExpediente,
                                                ByRef IdraSiiCacheExepediente As Integer) As String

        '-----------------------------------------------------------------------------------------------
        'Funcion : Inserta el cache registro expediente SII auto creacion y la relación del expediente
        'con el radicado
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
        'Try
        Dim NitIdentificacion As String = ""
        If CStruSiiCahcheExpediente.NitIdentificacion <> "" Then
            NitIdentificacion = CStruSiiCahcheExpediente.NitIdentificacion
        End If
        Dim Rsocial As String = ""
        If CStruSiiCahcheExpediente.Rsocial <> "" Then
            Rsocial = Left(CStruSiiCahcheExpediente.Rsocial, 40)
            Rsocial = Rsocial.Replace("'", "")
            Rsocial = Rsocial.Replace("´", "")
        End If
        Dim Matricula As String = CStruSiiCahcheExpediente.Matricula
        If Matricula <> "" Then
            Matricula = Matricula.Replace("S0", "")
        End If
        Dim FechaRegistroCache As String = ""
        Dim ClassGestionFechas As New ClassGestionFechas
        ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(FechaRegistroCache)
        Dim SQLInsertInto As String = "Insert into  ra_sii_cache_exepediente (RadicadoSII,CodigoBarras,NitIdentificacion,Rsocial,NombreGabinete," &
                  "EstadoVinculaDocumento,Matricula,IdExpediente,FechaRegistroCache,EstadoPadre) values ('" & CStruSiiCahcheExpediente.RadicadoSII & "','" &
                  CStruSiiCahcheExpediente.CodBarras & "','" & NitIdentificacion & "','" &
                  Rsocial & "','" & CStruSiiCahcheExpediente.NombreGabinete & "',0,'" & Matricula &
                  "'," & CStruSiiCahcheExpediente.IdExpediente & ",'" & FechaRegistroCache & "'," & CStruSiiCahcheExpediente.EstadoPadre & ")"
        Dim SqlInsertRelacion As String = "Insert into ra_relacion_radicado_externo_expediente (expediente_archivo_ID_EXPEDIENTE,RadicadoExterno,FechaRegistro) values " &
            " (" & CStruSiiCahcheExpediente.IdExpediente & ",'" & CStruSiiCahcheExpediente.RadicadoSII & "','" & FechaRegistroCache & "')"
        Dim Result As String = ""
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Try
            myCommand.CommandText = SQLInsertInto
            Dim ResultadoSqlinsert = myCommand.ExecuteNonQuery()
            If ResultadoSqlinsert = 0 Then
                RegistraCacheCreacionExpedienteSII = "Imposible registrar cache de inscripcion SII  "
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = SqlInsertRelacion
            ResultadoSqlinsert = myCommand.ExecuteNonQuery()
            If ResultadoSqlinsert = 0 Then
                RegistraCacheCreacionExpedienteSII = "Imposible registrar relación expediente radicado  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            RegistraCacheCreacionExpedienteSII = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                RegistraCacheCreacionExpedienteSII = "Error registrando cache de inscripción de SII  " & e.Message

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    RegistraCacheCreacionExpedienteSII = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
        End Try
    End Function
    Function SolicitaCacheCreacionExepdienteSiiRadicado(ByVal ReciboSII As String,
                                                        ByVal ValidaExitenciaRegistro As Integer,
                                                        ByRef LCStruSiiCahcheExpediente As List(Of CStruSiiCahcheExpediente)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura cache expediente de integración SII por recibo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'ReciboSII          : Representa el consecutivo del recibo de caja del sistema SII
        '         
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'LCStruSiiCahcheExpediente   : Representa la estructura del registro del expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-22
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim DataBaseConexion As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_sii_cache_exepediente")
            Dim SQLconsulta As String = "Select id_ra_sii_cache_exepediente,RadicadoSII," &
            "CodigoBarras,NitIdentificacion,Rsocial,NombreGabinete,Matricula,EstadoVinculaDocumento,IdExpediente,EstadoPadre" &
            " from  ra_sii_cache_exepediente " &
            " where RadicadoSII='" & ReciboSII & "'"
            Dim Result As String = ""
            Result = DataBaseConexion.SELECTION_SELECT_FIELDA(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaCacheCreacionExepdienteSiiRadicado = "Error funcion SolicitaCacheCreacionExepdienteSiiRadicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If ValidaExitenciaRegistro = 1 Then
                    SolicitaCacheCreacionExepdienteSiiRadicado = "No fue posible encontrar registros de creación de expedientes relacionados con el recibo.(" & ReciboSII & ")"
                    Exit Function
                Else
                    SolicitaCacheCreacionExepdienteSiiRadicado = "YES"
                    Exit Function
                End If

            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim CStruSiiCahcheExpediente As New CStruSiiCahcheExpediente
                    CStruSiiCahcheExpediente.id_ra_sii_cache_exepediente = Datset.Tables(0).Rows(i).Item("id_ra_sii_cache_exepediente")
                    CStruSiiCahcheExpediente.RadicadoSII = Datset.Tables(0).Rows(i).Item("RadicadoSII")
                    CStruSiiCahcheExpediente.CodBarras = Datset.Tables(0).Rows(i).Item("CodigoBarras")
                    If Datset.Tables(0).Rows(i).IsNull("NitIdentificacion") = True Then
                        CStruSiiCahcheExpediente.NitIdentificacion = ""
                    Else
                        CStruSiiCahcheExpediente.NitIdentificacion = Datset.Tables(0).Rows(i).Item("NitIdentificacion")
                    End If
                    CStruSiiCahcheExpediente.Rsocial = Datset.Tables(0).Rows(i).Item("Rsocial")
                    CStruSiiCahcheExpediente.NombreGabinete = Datset.Tables(0).Rows(i).Item("NombreGabinete")
                    CStruSiiCahcheExpediente.Matricula = Datset.Tables(0).Rows(i).Item("Matricula")
                    CStruSiiCahcheExpediente.EstadoVinculaDocumento = Datset.Tables(0).Rows(i).Item("EstadoVinculaDocumento")
                    CStruSiiCahcheExpediente.IdExpediente = Datset.Tables(0).Rows(i).Item("IdExpediente")
                    CStruSiiCahcheExpediente.EstadoPadre = Datset.Tables(0).Rows(i).Item("EstadoPadre")
                    LCStruSiiCahcheExpediente.Add(CStruSiiCahcheExpediente)
                Next
                SolicitaCacheCreacionExepdienteSiiRadicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaCacheCreacionExepdienteSiiRadicado = "Inconsistencia general función SolicitaCacheCreacionExepdienteSiiRadicado"
        End Try
    End Function
    Function SolicitaCacheCreacionExpedienteSII(ByVal MatriculaSII As String,
                                                ByVal Gabinete As String,
                                                ByRef CStruSiiCahcheExpediente As CStruSiiCahcheExpediente) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura cache expediente de integración SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'MatriculaSII          : Representa la matricula del matricualado de integración SII
        'Gabinete              : Representa el gabinete de o registro
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruSiiCahcheExpediente   : Representa la estructura del registro del expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_sii_cache_exepediente")
            Dim SQLconsulta As String = "Select id_ra_sii_cache_exepediente,RadicadoSII," &
            "CodigoBarras,NitIdentificacion,Rsocial,NombreGabinete,Matricula,EstadoVinculaDocumento,IdExpediente,EstadoPadre" &
            " from  ra_sii_cache_exepediente " &
            " where Matricula='" & MatriculaSII & "' and NombreGabinete='" & Gabinete & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaCacheCreacionExpedienteSII = "Error funcion SolicitaCacheCreacionExpedienteSII " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaCacheCreacionExpedienteSII = "YES"
                Exit Function
            Else
                CStruSiiCahcheExpediente.id_ra_sii_cache_exepediente = Datset.Tables(0).Rows(0).Item("id_ra_sii_cache_exepediente")
                CStruSiiCahcheExpediente.RadicadoSII = Datset.Tables(0).Rows(0).Item("RadicadoSII")
                CStruSiiCahcheExpediente.CodBarras = Datset.Tables(0).Rows(0).Item("CodigoBarras")
                If Datset.Tables(0).Rows(0).IsNull("NitIdentificacion") = True Then
                    CStruSiiCahcheExpediente.NitIdentificacion = ""
                Else
                    CStruSiiCahcheExpediente.NitIdentificacion = Datset.Tables(0).Rows(0).Item("NitIdentificacion")
                End If
                CStruSiiCahcheExpediente.Rsocial = Datset.Tables(0).Rows(0).Item("Rsocial")
                CStruSiiCahcheExpediente.NombreGabinete = Datset.Tables(0).Rows(0).Item("NombreGabinete")
                CStruSiiCahcheExpediente.Matricula = Datset.Tables(0).Rows(0).Item("Matricula")
                CStruSiiCahcheExpediente.EstadoVinculaDocumento = Datset.Tables(0).Rows(0).Item("EstadoVinculaDocumento")
                CStruSiiCahcheExpediente.IdExpediente = Datset.Tables(0).Rows(0).Item("IdExpediente")
                CStruSiiCahcheExpediente.EstadoPadre = Datset.Tables(0).Rows(0).Item("EstadoPadre")
                SolicitaCacheCreacionExpedienteSII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaCacheCreacionExpedienteSII = "Inconsistencia general función SolicitaCacheCreacionExpedienteSII " & ex.Message
        End Try
    End Function
End Class
