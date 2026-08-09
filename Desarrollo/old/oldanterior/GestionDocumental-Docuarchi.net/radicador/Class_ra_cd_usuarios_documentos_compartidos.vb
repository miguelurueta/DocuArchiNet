Public Structure Detalle_registro_compartido_usuario
    Dim ID_USUARIOS_DOCUMENTOS_COMPARTIDOS As Long
    Dim nombre_usuario As String
    Dim cargo_usuario As String
    Dim FECHA_REGISTRO_SOLICITUD As String
    Dim FECHA_RESPUESTA_SOLICITUD As String
    Dim DESCRIPCION_ESTADO_RESPUESTA As String
    Dim Remit_Dest_Interno_id_Remit_Dest_Int As Integer
End Structure
Public Class Class_ra_cd_usuarios_documentos_compartidos

    Function SolicitaRegistroDetalleUsuariosRelacionadosADocumentoCompartido(ByVal IdDocumentoCompartido As Long,
                                                                             ByRef DetalleRegistroUsuarioCompartido() As Detalle_registro_compartido_usuario) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los detalles del los usuarios que hacen parte del documento compartido
        '
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdDocumentoCompartido  : Representa la identificación del registro del documento compartido
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'DetalleRegistroUsuarioCompartido  : Retorna la estructura detalle del documento compartido
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2019-03-28
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT rdi.Nombre_Remitente, rdi.Cargo_Remite, " &
                "rcud.FECHA_REGISTRO_SOLICITUD,rcud.FECHA_RESPUESTA_SOLICITUD,rcud.DESCRIPCION_ESTADO_RESPUESTA,rcud.ID_USUARIOS_DOCUMENTOS_COMPARTIDOS,rcud.Remit_Dest_Interno_id_Remit_Dest_Int " &
                "from ra_cd_usuarios_documentos_compartidos as rcud " &
                 " inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rcud.Remit_Dest_Interno_id_Remit_Dest_Int) " &
                 "  where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & IdDocumentoCompartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_solicitudes_aprobacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaRegistroDetalleUsuariosRelacionadosADocumentoCompartido = "Error función SolicitaRegistroDetalleUsuariosRelacionadosADocumentoCompartido " & Result
                Exit Function
            End If
            Erase DetalleRegistroUsuarioCompartido
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve DetalleRegistroUsuarioCompartido(i)
                    DetalleRegistroUsuarioCompartido(i).nombre_usuario = Datset.Tables(0).Rows(i).Item(0)
                    DetalleRegistroUsuarioCompartido(i).cargo_usuario = Datset.Tables(0).Rows(i).Item(1)
                    DetalleRegistroUsuarioCompartido(i).FECHA_REGISTRO_SOLICITUD = Datset.Tables(0).Rows(i).Item(2)
                    If Datset.Tables(0).Rows(i).IsNull(3) Then
                        DetalleRegistroUsuarioCompartido(i).FECHA_RESPUESTA_SOLICITUD = ""
                    Else
                        DetalleRegistroUsuarioCompartido(i).FECHA_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(i).Item(3)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(4) Then
                        DetalleRegistroUsuarioCompartido(i).DESCRIPCION_ESTADO_RESPUESTA = ""
                    Else
                        DetalleRegistroUsuarioCompartido(i).DESCRIPCION_ESTADO_RESPUESTA = Datset.Tables(0).Rows(i).Item(4)
                    End If
                    DetalleRegistroUsuarioCompartido(i).ID_USUARIOS_DOCUMENTOS_COMPARTIDOS = Datset.Tables(0).Rows(i).Item(5)
                    DetalleRegistroUsuarioCompartido(i).Remit_Dest_Interno_id_Remit_Dest_Int = Datset.Tables(0).Rows(i).Item(6)
                Next
                SolicitaRegistroDetalleUsuariosRelacionadosADocumentoCompartido = "YES"
                Exit Function
            Else
                SolicitaRegistroDetalleUsuariosRelacionadosADocumentoCompartido = "Imposible encontrar el detalle de usuarios de registro compartido con el id (" & IdDocumentoCompartido & ")"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaRegistroDetalleUsuariosRelacionadosADocumentoCompartido = "Inconsistencia general función SolicitaRegistroDetalleUsuariosRelacionadosADocumentoCompartido " & ex.Message
        End Try
    End Function
    Function Solicita_tipo_documento_compartido(ByVal id_compartido As Integer, _
                                                ByRef id_tipo_compartido As Integer) As String
        '---------------------------------------------------------------
        'Función : Solicita el tipo de registro de documento compartido
        'Fecha : 2019-02-18
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT TIPO_REGISTRO_COMPARTIDO  from ra_cd_usuarios_documentos_compartidos " & _
                      " where ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" & id_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipo_documento_compartido = "Error función Solicita_tipo_documento_compartido " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_tipo_compartido = Datset.Tables(0).Rows(0).Item(0)
                Solicita_tipo_documento_compartido = "YES"
                Exit Function
            Else
                Solicita_tipo_documento_compartido = "Imposible encontrar el tipo de registro compartido con el id (" & id_compartido & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipo_documento_compartido = "Inconsistencia general función Solicita_tipo_documento_compartido " & ex.Message
        End Try
    End Function
    Function SolicitaeEstructuraDocumentoCompartido(ByVal IdDocumentoCompartido As Integer,
                                                    ByRef stru As STRU_DOCUMENTO_COMPARTIDO_USUARIOS) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de un documento compartido con la identificación de registro
        ' del documento compartido
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdDocumentoCompartido  : Representa la identificación del registro del documento compartido
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'STRU_DOCUMENTO_COMPARTIDO_USUARIOS  : Retorna la estructuara del reistro del documento compartido
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-13
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim sql_consulta As String = "SELECT Remit_Dest_Interno_id_remit_dest_Int," &
                     "ID_RA_CD_DOCUMENTOS_COMPARTIDOS,FECHA_REGISTRO_SOLICITUD,FECHA_RESPUESTA_SOLICITUD," _
                    & "ESTADO_RESPUESTA_SOLICITUD,TIEMPO_RESPUESTA_SOLICITANTE,ESTADO_VISTO_SOLICITANTE,DESCRIPCION_ESTADO_RESPUESTA,FECHA_LIMITE_RESPUESTA," &
                     "ASUNTO_DOCUMENTO,RADICADO_RELACIONADO,ESTADO_ELIMINADO,TIPO_REGISTRO_COMPARTIDO,DESCRIPCION_TIPO_COMPARTIDO,ESTADO_CONFIRMACION_COLABORACION" &
                    " from ra_cd_usuarios_documentos_compartidos where ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" & IdDocumentoCompartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaeEstructuraDocumentoCompartido = "Error función SolicitaeEstructuraDocumentoCompartido " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaeEstructuraDocumentoCompartido = "Imposible encontrar la estructura del documento compartido cuya identificación es  (" & IdDocumentoCompartido & ")."
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru.Remit_Dest_Interno_id_remit_dest_Int = 0
                Else
                    stru.Remit_Dest_Interno_id_remit_dest_Int = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru.ID_RA_CD_DOCUMENTOS_COMPARTIDOS = 0
                Else
                    stru.ID_RA_CD_DOCUMENTOS_COMPARTIDOS = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru.FECHA_REGISTRO_SOLICITUD = ""
                Else
                    stru.FECHA_REGISTRO_SOLICITUD = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru.FECHA_RESPUESTA_SOLICITUD = ""
                Else
                    stru.FECHA_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru.ESTADO_RESPUESTA_SOLICITUD = 0
                Else
                    stru.ESTADO_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    stru.TIEMPO_RESPUESTA_SOLICITUD = 0
                Else
                    stru.TIEMPO_RESPUESTA_SOLICITUD = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    stru.ESTADO_VISTO_SOLICITANTE = 0
                Else
                    stru.ESTADO_VISTO_SOLICITANTE = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    stru.DESCRIPCION_ESTADO_RESPUESTA = ""
                Else
                    stru.DESCRIPCION_ESTADO_RESPUESTA = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    stru.FECHA_LIMITE_RESPUESTA = ""
                Else
                    stru.FECHA_LIMITE_RESPUESTA = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) = True Then
                    stru.ASUNTO_DOCUMENTO = ""
                Else
                    stru.ASUNTO_DOCUMENTO = Datset.Tables(0).Rows(0).Item(9)
                End If
                If Datset.Tables(0).Rows(0).IsNull(10) = True Then
                    stru.RADICADO_RELACIONADO = ""
                Else
                    stru.RADICADO_RELACIONADO = Datset.Tables(0).Rows(0).Item(10)
                End If
                If Datset.Tables(0).Rows(0).IsNull(11) = True Then
                    stru.ESTADO_ELIMINADO = 0
                Else
                    stru.ESTADO_ELIMINADO = Datset.Tables(0).Rows(0).Item(11)
                End If
                If Datset.Tables(0).Rows(0).IsNull(12) = True Then
                    stru.TIPO_REGISTRO_COMPARTIDO = 0
                Else
                    stru.TIPO_REGISTRO_COMPARTIDO = Datset.Tables(0).Rows(0).Item(12)
                End If
                If Datset.Tables(0).Rows(0).IsNull(13) = True Then
                    stru.DESCRIPCION_TIPO_COMPARTIDO = ""
                Else
                    stru.DESCRIPCION_TIPO_COMPARTIDO = Datset.Tables(0).Rows(0).Item(13)
                End If
                If Datset.Tables(0).Rows(0).IsNull(14) = True Then
                    stru.ESTADO_CONFIRMACION_COLABORACION = 0
                Else
                    stru.ESTADO_CONFIRMACION_COLABORACION = Datset.Tables(0).Rows(0).Item(14)
                End If
                SolicitaeEstructuraDocumentoCompartido = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaeEstructuraDocumentoCompartido = "Inconsistencia general función SolicitaeEstructuraDocumentoCompartido " & ex.Message
        End Try
    End Function
    Function Elimina_registro_usuario_documento_compartido(ByVal id_documento_compartido As Integer) As String
        Try
            Dim stru_documento As STRU_DOCUMENTO_COMPARTIDO_USUARIOS = Nothing
            Dim result As String = ""
            Dim Refclas As New ClassGaCompartirDocumento
            Dim refclas_cd_compartidos As New Class_ra_Cd_Documentos_Compartidos
            result = Me.SolicitaeEstructuraDocumentoCompartido(id_documento_compartido,
                                                               stru_documento)
            If result <> "YES" Then
                Elimina_registro_usuario_documento_compartido = result
                Exit Function
            End If
            If stru_documento.TIPO_REGISTRO_COMPARTIDO = 3 Then
                Dim stru_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
                result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(stru_documento.ID_RA_CD_DOCUMENTOS_COMPARTIDOS,
                                                                                             stru_general)
                If result <> "YES" Then
                    Elimina_registro_usuario_documento_compartido = result
                    Exit Function
                Else
                    If stru_general.ESTADO_CONFIRMACION_COLABORACION = 0 Then
                        Elimina_registro_usuario_documento_compartido = "No se puede eliminar el registro debe aportar una nota de colaboración o un documento"
                        Exit Function
                    End If
                End If
            End If
            If stru_documento.TIPO_REGISTRO_COMPARTIDO = 2 Then
                Dim stru_general As STRU_DOCUMENTO_COMPARTIDO_GENERAL = Nothing
                result = refclas_cd_compartidos.SolicitaEstructuraGeneraldocumentosCompartido(stru_documento.ID_RA_CD_DOCUMENTOS_COMPARTIDOS,
                                                                                             stru_general)
                If result <> "YES" Then
                    Elimina_registro_usuario_documento_compartido = result
                    Exit Function
                Else
                    If stru_general.ESTADO_APROBACION = 0 Then
                        Elimina_registro_usuario_documento_compartido = "No se puede eliminar el registro por que no tiene una respuesta final"
                        Exit Function
                    End If
                End If
            End If
            If stru_documento.ESTADO_ELIMINADO = 0 Then
                Elimina_registro_usuario_documento_compartido = "El registro ya se encuentra en la badeja de eliminados, imposible eliminar"
                Exit Function
            End If
            Dim sql_consulta As String = "update  ra_cd_usuarios_documentos_compartidos  set ESTADO_ELIMINADO=0" & _
                      " where ID_USUARIOS_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            result = ref.SELECTION_INSERT_COMMAND(sql_consulta)
            If result <> "YES" Then
                Elimina_registro_usuario_documento_compartido = "Error Elimina_registro_usuario_documento_compartido " & result
                Exit Function
            Else
                If HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION") > 0 Then
                    HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION") = HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION") - 1
                End If
            End If

            Elimina_registro_usuario_documento_compartido = "YES"
        Catch ex As Exception
            Elimina_registro_usuario_documento_compartido = "Inconsistencia general función Elimina_registro_usuario_documento_compartido " & ex.Message
        End Try
    End Function
    Function Solicita_estado_decision_usuario_documento_aprobacion(ByVal id_documento_compartido As Integer, _
                                                                   ByRef estado_decision As String) As String
        Try
            Dim sql_consulta As String = "Select * from  ra_cd_usuarios_documentos_compartidos  " & _
                     " where ID_RA_CD_DOCUMENTOS_COMPARTIDOS=" & id_documento_compartido & _
                     " and ESTADO_RESPUESTA_SOLICITUD <> 3 and ESTADO_RESPUESTA_SOLICITUD <> 0"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_estado_decision_usuario_documento_aprobacion = "Error función Solicita_estado_decision_usuario_documento_aprobacion " & result
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count = 0 Then
                    estado_decision = "NO"
                    Solicita_estado_decision_usuario_documento_aprobacion = "YES"
                    Exit Function
                Else
                    estado_decision = "YES"
                    Solicita_estado_decision_usuario_documento_aprobacion = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_estado_decision_usuario_documento_aprobacion = "Iconsistencia general función Solicita_estado_decision_usuario_documento_aprobacion " & ex.Message
        End Try
    End Function
End Class
