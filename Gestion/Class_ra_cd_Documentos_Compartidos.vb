Imports MySql.Data.MySqlClient

Public Class Class_ra_Cd_Documentos_Compartidos
    Function Restricion_eliminar_documento_compartido(ByVal id_imagen As Integer,
                                                      ByVal nombre_gabinete As String,
                                                      ByRef id_registro_documento_compartido As Integer) As String
        '----------------------------------------------------------
        'Función : Verifica la relación del documento con el
        'registro de documentos compartidos, si el sistema
        'devuelve la variable id_registro_documento_compartido
        'en cero (0) se autoriza la eliminación del documento
        'si devuelve diferente de cero contine el identificador
        'del documento lo que restringe la eliminación.
        'Fecha : 2017-12-15
        'Ing :Miguel Angel Urueta Miranda
        '----------------------------------------------------------
        Try
            '--------------------------------------------------
            'Solicita relación imagen a registro de documento
            'compartido
            '--------------------------------------------------
            Dim Class_ra_cd_documentos_gabinete_compartido As New Class_ra_cd_documentos_gabinete_compartido
            Dim Class_ra_Cd_Documentos_Compartidos As New Class_ra_Cd_Documentos_Compartidos
            id_registro_documento_compartido = 0
            Dim Result As String = ""
            Result = Class_ra_cd_documentos_gabinete_compartido.Solicita_id_registro_documento_compartido_gabinete(id_imagen,
                                                                                                                   nombre_gabinete,
                                                                                                                   id_registro_documento_compartido)
            If Result <> "YES" Then
                Restricion_eliminar_documento_compartido = Result
                Exit Function
            End If
            '-------------------------------------------------
            'Autoriza la eliminación del documento
            '-------------------------------------------------
            If id_registro_documento_compartido = 0 Then
                Restricion_eliminar_documento_compartido = "YES"
                Exit Function
            End If
            '-------------------------------------------------
            'Verifica existencia relación documento compartido
            '-------------------------------------------------
            Dim estado_registro_relacion As Integer = 1
            Result = Class_ra_Cd_Documentos_Compartidos.Verifica_existencia_de_registro_general_documentos_compartidos(id_registro_documento_compartido,
                                                                                                                       estado_registro_relacion)
            If Result <> "YES" Then
                Restricion_eliminar_documento_compartido = Result
                Exit Function
            End If
            If estado_registro_relacion = 0 Then
                'Autoriza eliminación del documento
                id_registro_documento_compartido = 0
                Restricion_eliminar_documento_compartido = "YES"
                Exit Function
            Else
                'Restringe eliminación del documento
                Restricion_eliminar_documento_compartido = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Restricion_eliminar_documento_compartido = "Inconsistencia general función Restricion_eliminar_documento_compartido " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_de_registro_general_documentos_compartidos(ByVal id_registro_documento_compartido As Long,
                                                                            ByRef estado_registro_relacion As Integer) As String
        '-----------------------------------------------------------------
        'Función : Verifica la existencia de relacion del documento
        'en el registro principal, la variable estado_registro_relacion
        'cuando devuelve cero(0) no existe relación y el estado uno (1) 
        'existe una relación entre el registro y el documento
        'Fecha : 2017-12-15
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select ID_RA_CD_DOCUMENTOS_COMPARTIDOS " &
           " from ra_cd_documentos_compartidos where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_registro_documento_compartido
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_documentos_compartidos")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_de_registro_general_documentos_compartidos = "Función  Verifica_existencia_de_registro_general_documentos_compartidos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_registro_relacion = 0
                Verifica_existencia_de_registro_general_documentos_compartidos = "YES"
                Exit Function
            Else
                estado_registro_relacion = 1
                Verifica_existencia_de_registro_general_documentos_compartidos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_de_registro_general_documentos_compartidos = "Inconsistencia general función Verifica_existencia_de_registro_general_documentos_compartidos " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraGeneraldocumentosCompartido(ByVal IdDocumentoCompartidoGeneral As Integer,
                                                           ByRef stru As STRU_DOCUMENTO_COMPARTIDO_GENERAL) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura general de un documento compartido
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdDocumentoCompartidoGeneral   : Representa la identificación de un documento compartido
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'STRU_DOCUMENTO_COMPARTIDO_GENERAL  : Retorna la estructura general de un documento compartido
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-13
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT ID_RA_CD_DOCUMENTOS_COMPARTIDOS," &
            "Remit_Dest_Interno_id_remit_dest_Int,FECHA_REGISTRO_SOLICITUD,FECHA_REGISTRO_APROBACION," &
            "ESTADO_APROBACION,TIEMPO_RESPUESTA_APROBACION,ESTADO_PRIORIDAD,NOTA_SOLICITUD,DESCRIPCION_ESTADO_APROBACION,FECHA_LIMITE_RESPUESTA," &
            "TIPO_REGISTRO_COMPARTIDO,DESCRIPCION_TIPO_COMPARTIDO,ASUNTO_DOCUMENTO,RADICADO_RELACIONADO,ESTADO_CONFIRMACION_COLABORACION," &
            "ESTADO_ELIMINADO from ra_cd_documentos_compartidos where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & IdDocumentoCompartidoGeneral
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructuraGeneraldocumentosCompartido = "Error Función SolicitaEstructuraGeneraldocumentosCompartido " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEstructuraGeneraldocumentosCompartido = "Imposible encontrar datos para la solicitud de aprobación número " & IdDocumentoCompartidoGeneral
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru.ID_RA_CD_DOCUMENTOS_COMPARTIDOS = 0
                Else
                    stru.ID_RA_CD_DOCUMENTOS_COMPARTIDOS = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru.Remit_Dest_Interno_id_remit_dest_Int = 0
                Else
                    stru.Remit_Dest_Interno_id_remit_dest_Int = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru.FECHA_REGISTRO_SOLICITUD = ""
                Else
                    stru.FECHA_REGISTRO_SOLICITUD = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru.FECHA_REGISTRO_APROBACION = ""
                Else
                    stru.FECHA_REGISTRO_APROBACION = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru.ESTADO_APROBACION = 0
                Else
                    stru.ESTADO_APROBACION = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    stru.TIEMPO_RESPUESTA_APROBACION = 0
                Else
                    stru.TIEMPO_RESPUESTA_APROBACION = Datset.Tables(0).Rows(0).Item(5)
                End If

                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    stru.ESTADO_PRIORIDAD = 0
                Else
                    stru.ESTADO_PRIORIDAD = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    stru.NOTA_SOLICITUD = ""
                Else
                    stru.NOTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    stru.DESCRIPCION_ESTADO_APROBACION = ""
                Else
                    stru.DESCRIPCION_ESTADO_APROBACION = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) = True Then
                    stru.FECHA_LIMITE_RESPUESTA = ""
                Else
                    stru.FECHA_LIMITE_RESPUESTA = Datset.Tables(0).Rows(0).Item(9)
                End If
                If Datset.Tables(0).Rows(0).IsNull(10) = True Then
                    stru.TIPO_REGISTRO_COMPARTIDO = 0
                Else
                    stru.TIPO_REGISTRO_COMPARTIDO = Datset.Tables(0).Rows(0).Item(10)
                End If
                If Datset.Tables(0).Rows(0).IsNull(11) = True Then
                    stru.DESCRIPCION_TIPO_COMPARTIDO = ""
                Else
                    stru.DESCRIPCION_TIPO_COMPARTIDO = Datset.Tables(0).Rows(0).Item(11)
                End If
                If Datset.Tables(0).Rows(0).IsNull(12) = True Then
                    stru.ASUNTO_DOCUMENTO = ""
                Else
                    stru.ASUNTO_DOCUMENTO = Datset.Tables(0).Rows(0).Item(12)
                End If
                If Datset.Tables(0).Rows(0).IsNull(13) = True Then
                    stru.RADICADO_RELACIONADO = ""
                Else
                    stru.RADICADO_RELACIONADO = Datset.Tables(0).Rows(0).Item(13)
                End If
                If Datset.Tables(0).Rows(0).IsNull(14) = True Then
                    stru.ESTADO_CONFIRMACION_COLABORACION = 0
                Else
                    stru.ESTADO_CONFIRMACION_COLABORACION = Datset.Tables(0).Rows(0).Item(14)
                End If
                If Datset.Tables(0).Rows(0).IsNull(15) = True Then
                    stru.ESTADO_ELIMINADO = 0
                Else
                    stru.ESTADO_ELIMINADO = Datset.Tables(0).Rows(0).Item(15)
                End If
                SolicitaEstructuraGeneraldocumentosCompartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstructuraGeneraldocumentosCompartido = "Inconsistencia general función SolicitaEstructuraGeneraldocumentosCompartido " & ex.Message
        End Try
    End Function
    Function Archiva_solcitud_aprobacion_documento_compartido(ByVal id_documento_compartido As Integer) As String

        Dim stru As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
        Dim Result As String = ""
        Result = Me.SolicitaEstructuraGeneraldocumentosCompartido(id_documento_compartido,
                                                                  stru)
        If Result <> "YES" Then
            Archiva_solcitud_aprobacion_documento_compartido = Result
            Exit Function
        End If
        If stru.TIPO_REGISTRO_COMPARTIDO <> 2 Then
            Archiva_solcitud_aprobacion_documento_compartido = "La solicitud que desea archivar no es de tipo aprobación, imposible continuar con el proceso de archivado"
            Exit Function
        End If
        If stru.ESTADO_APROBACION <> 0 Then
            Archiva_solcitud_aprobacion_documento_compartido = "El documento cuenta con una desición imposible registrar la desición (" & stru.DESCRIPCION_ESTADO_APROBACION & ")"
            Exit Function
        End If
        Dim estado_decisicion As String = ""
        Dim Refclas_rd_usuarios_solcitud As New Class_ra_cd_usuarios_documentos_compartidos
        Result = Refclas_rd_usuarios_solcitud.Solicita_estado_decision_usuario_documento_aprobacion(id_documento_compartido,
                                                                                                    estado_decisicion)
        If Result <> "YES" Then
            Archiva_solcitud_aprobacion_documento_compartido = Result
            Exit Function
        End If
        If estado_decisicion = "YES" Then
            Archiva_solcitud_aprobacion_documento_compartido = "El documento compartido registra decisión por parte de un usuario, imposible continuar con el proceso de archivado"
            Exit Function
        End If
        Dim date1al As String = ""
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now, date1al)
        If Result <> "YES" Then
            Archiva_solcitud_aprobacion_documento_compartido = Result
            Exit Function
        End If
        Dim stiempo As Object = Nothing
        Dim minuno As Object = Nothing
        Dim hora As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(stru.FECHA_REGISTRO_SOLICITUD,
                                                                          stiempo,
                                                                          hora,
                                                                          minuno,
                                                                          dias_calendario,
                                                                          dias_no_habiles)
        If Result <> "YES" Then
            Archiva_solcitud_aprobacion_documento_compartido = Result
            Exit Function
        End If
        Dim sql_update_user As String = "update  ra_cd_usuarios_documentos_compartidos  set ESTADO_ELIMINADO=0" &
                     ", ASUNTO_DOCUMENTO='Archivado por usuario solicitante (" & stru.ASUNTO_DOCUMENTO & ") '" &
                     " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido
        Dim sql_update As String = "Update ra_cd_documentos_compartidos set ESTADO_APROBACION=" & 3 & ",DESCRIPCION_ESTADO_APROBACION='" & "Archivado" & "'" &
           ",FECHA_REGISTRO_APROBACION='" & date1al & "',TIEMPO_RESPUESTA_APROBACION=" & stiempo &
           ", NOTA_SOLICITUD='Archivado por el solicitante'" &
           ", ASUNTO_DOCUMENTO='Archivado por el solicitante (" & stru.ASUNTO_DOCUMENTO & ")'" &
           " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            '-------------------------------------------------
            'Actualiza solicitud de aprobación usuario
            '-------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_update_user
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Archiva_solcitud_aprobacion_documento_compartido = "Imposible actualizar la solicitud de aprobación del usuario  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '------------------------------------------------
            'Actualiza solicitud general aprobacion
            '------------------------------------------------

            myCommand.CommandText = sql_update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Archiva_solcitud_aprobacion_documento_compartido = "Imposible actualizar la solicitud general de aprobación  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            myConnection.Close()
            Archiva_solcitud_aprobacion_documento_compartido = "YES"
        Catch ex As Exception
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Archiva_solcitud_aprobacion_documento_compartido = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Archiva_solcitud_aprobacion_documento_compartido = "Error General " & ex.Message
            Exit Function
        End Try


    End Function
End Class
