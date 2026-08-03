Public Structure stru_envio
    Dim ID_RESPUESTA_RADICADO As Integer
    Dim ID_REMIT_DEST_INT As Integer
    Dim ID_AREA As Integer
    Dim codigo_dest_externo As Integer
    Dim system_plantilla_radicado_id_plantilla As Integer
    Dim RADICADO As String
    Dim ID_RUTA_WF As Integer
    Dim ID_TAREA_WF As Long
    Dim FECHA_REGISTRO As String
    Dim FECHA_VENCE As String
    Dim FECHA_RESPUETA As String
    Dim HORA_RESPUESTA As String
    Dim TIEMPO_RESPUESTA As Long
    Dim RADICADO_RESPUESTA As String
    Dim MEDIO_ENVIO As String
    Dim EMPRESA_ENVIO As String
    Dim GUIA_ENVIO As String
    Dim FECHA_ENVIO As String
    Dim HORA_ENVIO As String
    Dim ID_USUARIO_RADICADO As Integer
    Dim ID_IMAGEN As Long
    Dim GABINETE As String
    Dim NOTA_RESPUESTA As String
    Dim FECHA_RECIBO_FISICO As String
    Dim HORA_RECIBO_FISICO As String
    Dim DESTINATARIO As String
    Dim DIRECCION_DESTINATARIO As String
    Dim TRAMITE_DOCUMENTO As String
    Dim ESTADO_ENVIO As String
    Dim ESTADO_RESPUESTA As String
    Dim ASUNTO As String
    Dim id_usuario_gestion_propietario As Integer
    Dim USUARIO_RESPONSABLE As String
    Dim ID_IMAGEN_RESPUESTA As Integer
    Dim estado_envio_correo As Integer
    Dim TIPO_RESPUESTA_ELAB_USUARIO As Integer
    Dim ESTADO_APROBACION As Integer
    Dim TIEMPO_RESPUESTA_APROBACION As Long
    Dim NOTA_SOLICITUD As String
    Dim DESCRIPCION_ESTADO_APROBACION As String
    Dim FECHA_REGISTRO_SOLICITUD As String
    Dim FECHA_REGISTRO_APROBACION As String
    Dim ID_SOLICITUDES_APROBACION_RESP As Integer
    Dim ID_TIPO_DOC_RESPUESTA As Integer
    Dim FECHA_REGISTRO_EVIO_CORREO As String
    Dim CORREO_NOTIFICACION As String
    Dim FECHA_CONFIRMACION_CORREO_RECIBIDO As String
    Dim IP_CONFIRMACION_CORREO_RECIBIDO As String
    Dim HUELLA_CORREO_RECIBIDO As String
End Structure
Public Class Class_ra_respuesta_radicado
    Function SolicitaIdRespuestaRadicado(ByVal RadicadoTramite As String,
                                         ByVal IdRespuestaRadicado As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita identificación respuesta radicado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RadicadoTramite     : Representa el radicado del tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdRespuestaRadicado   : Retorna la idnetificación de la respuesta del tramite
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim SQLConsulta As String = "SELECT ID_RESPUESTA_RADICADO" &
             " FROM ra_respuesta_radicado " &
            " where  RADICADO='" & RadicadoTramite & "' order by  ID_RESPUESTA_RADICADO asc"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(SQLConsulta, Datset)
            If Result <> "YES" Then
                SolicitaIdRespuestaRadicado = "Función SolicitaIdRespuestaRadicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                IdRespuestaRadicado = 0
                SolicitaIdRespuestaRadicado = "YES"
                Exit Function
            Else
                IdRespuestaRadicado = Datset.Tables(0).Rows(0).Item(0)
                SolicitaIdRespuestaRadicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdRespuestaRadicado = "Inconsistencia general function SolicitaIdRespuestaRadicado " & ex.Message
        End Try
    End Function
    Function Retorna_id_respuesta_radicado(ByVal radicado As String,
                                           ByVal id_usuario_gestion As Integer,
                                           ByRef id_respuesta_radicado As Integer) As String
        Try
            Dim Parametro_Consulta As String = "SELECT ID_RESPUESTA_RADICADO" &
               " FROM ra_respuesta_radicado " &
              " where ID_REMIT_DEST_INT =" & id_usuario_gestion & " and RADICADO='" & radicado & "' order by  ID_RESPUESTA_RADICADO asc"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_respuesta_radicado = "Función Retorna_id_respuesta_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_respuesta_radicado = 0
                Retorna_id_respuesta_radicado = "YES"
                Exit Function
            Else
                id_respuesta_radicado = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_respuesta_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_respuesta_radicado = "Inconsistencia general función Retorna_id_respuesta_radicado " & ex.Message
        End Try
    End Function
    Function Retorna_id_respuesta_radicado_usuario_no_propietario(ByVal radicado As String,
                                                                  ByRef id_respuesta_radicado As Integer) As String
        Try
            Dim Parametro_Consulta As String = "SELECT ID_RESPUESTA_RADICADO" &
               " FROM ra_respuesta_radicado " &
              " where RADICADO='" & radicado & "' order by  ID_RESPUESTA_RADICADO desc"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_respuesta_radicado_usuario_no_propietario = "Función Retorna_id_respuesta_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_respuesta_radicado = 0
                Retorna_id_respuesta_radicado_usuario_no_propietario = "YES"
                Exit Function
            Else
                id_respuesta_radicado = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_respuesta_radicado_usuario_no_propietario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_respuesta_radicado_usuario_no_propietario = "Inconsistencia general función Retorna_id_respuesta_radicado_usuario_no_propietario " & ex.Message
        End Try
    End Function
    Function Solicita_id_destinatario_externo_plantilla(ByVal id_respuesta As Integer,
                                                        ByRef id_destinatrio_externo As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select codigo_dest_externo from ra_respuesta_radicado where ID_RESPUESTA_RADICADO='" & id_respuesta & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_destinatario_externo_plantilla = "Funcion  Solicita_id_destinatario_externo_plantilla dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_destinatrio_externo = 0
                Solicita_id_destinatario_externo_plantilla = "YES"
                Exit Function
            Else
                id_destinatrio_externo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_destinatario_externo_plantilla = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_id_destinatario_externo_plantilla = "Inconsistencia función Solicita_id_destinatario_externo_plantilla " & ex.Message
        End Try
    End Function
    Function Solicta_nombre_area_responsable_respuesta(ByVal id_respuesta As Integer,
                                                       ByRef nombre_area As String) As String
        Try
            Dim Parametro_Consulta As String = "Select AREA_RESPONSABLE from ra_respuesta_radicado where ID_RESPUESTA_RADICADO='" & id_respuesta & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicta_nombre_area_responsable_respuesta = "Funcion  Retorna_area_responsable_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_area = ""
                Solicta_nombre_area_responsable_respuesta = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_area = ""
                    Solicta_nombre_area_responsable_respuesta = "YES"
                    Exit Function
                Else
                    nombre_area = Datset.Tables(0).Rows(0).Item(0)
                    Solicta_nombre_area_responsable_respuesta = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicta_nombre_area_responsable_respuesta = "Inconsistencia función Solicta_nombre_area_responsable_respuesta " & ex.Message
        End Try
    End Function
    Function Solicta_asunto_solicitud_respuesta(ByVal id_respuesta As Integer,
                                                ByRef respuesta As String) As String
        '********************************************
        'Funcion : Retorna asunto respuesta
        'por el id de la respuesta
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-08-15
        '********************************************
        Try

            Dim Parametro_Consulta As String = "Select ASUNTO from ra_respuesta_radicado where ID_RESPUESTA_RADICADO='" & id_respuesta & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicta_asunto_solicitud_respuesta = "Funcion  Retorna_asunto_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                respuesta = ""
                Solicta_asunto_solicitud_respuesta = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    respuesta = ""
                    Solicta_asunto_solicitud_respuesta = "YES"
                    Exit Function
                Else
                    respuesta = Datset.Tables(0).Rows(0).Item(0)
                    Solicta_asunto_solicitud_respuesta = "YES"
                    Exit Function
                End If
            End If

        Catch ex As Exception
            Solicta_asunto_solicitud_respuesta = "Inconsistencia general función  Solicta_asunto_solicitud_respuesta  " & ex.Message
        End Try
    End Function
    Function Solicita_id_usuario_gestion_respuesta_radicado(ByVal id_respuesta As Integer,
                                                            ByRef id_usuario_gestion As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select ID_REMIT_DEST_INT from ra_respuesta_radicado where ID_RESPUESTA_RADICADO='" & id_respuesta & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_usuario_gestion_respuesta_radicado = "Funcion  Solicita_id_usuario_gestion_respuesta_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_usuario_gestion = 0
                Solicita_id_usuario_gestion_respuesta_radicado = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_usuario_gestion = 0
                    Solicita_id_usuario_gestion_respuesta_radicado = "YES"
                    Exit Function
                Else
                    id_usuario_gestion = Datset.Tables(0).Rows(0).Item(0)
                    Solicita_id_usuario_gestion_respuesta_radicado = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_id_usuario_gestion_respuesta_radicado = "Inconsistencia función Retorna_id_usuario_gestion " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraRespuestaRadicado(ByVal IdRespuestaRadicado As Integer,
                                                 ByRef struc_envio As stru_envio) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de un tramite de gestión de correspodencia
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdRespuestaRadicado : Representa la identificación de la respeusta de un tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'struc_envio         : Retorna la estructura de la respeusta de una tramite
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "Select ID_RESPUESTA_RADICADO,ID_REMIT_DEST_INT,ID_AREA," &
            "system_plantilla_radicado_id_plantilla,RADICADO,ID_RUTA_WF,ID_TAREA_WF,FECHA_REGISTRO," &
            "FECHA_VENCE,FECHA_RESPUETA,HORA_RESPUESTA,TIEMPO_RESPUESTA,RADICADO_RESPUESTA,MEDIO_ENVIO," &
            "EMPRESA_ENVIO,GUIA_ENVIO,FECHA_ENVIO,HORA_ENVIO,ID_USUARIO_RADICADO,ID_IMAGEN,GABINETE," &
            "NOTA_RESPUESTA,FECHA_RECIBO_FISICO,HORA_RECIBO_FISICO,DESTINATARIO,DIRECCION_DESTINATARIO," &
            "TRAMITE_DOCUMENTO,ESTADO_ENVIO,ESTADO_RESPUESTA from ra_respuesta_radicado where ID_RESPUESTA_RADICADO=" & IdRespuestaRadicado
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructuraRespuestaRadicado = " Función SolicitaEstructuraRespuestaRadicado dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    struc_envio.ID_RESPUESTA_RADICADO = 0
                Else
                    struc_envio.ID_RESPUESTA_RADICADO = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    struc_envio.ID_REMIT_DEST_INT = 0
                Else
                    struc_envio.ID_REMIT_DEST_INT = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    struc_envio.ID_AREA = 0
                Else
                    struc_envio.ID_AREA = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    struc_envio.system_plantilla_radicado_id_plantilla = 0
                Else
                    struc_envio.system_plantilla_radicado_id_plantilla = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    struc_envio.RADICADO = ""
                Else
                    struc_envio.RADICADO = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    struc_envio.ID_RUTA_WF = 0
                Else
                    struc_envio.ID_RUTA_WF = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    struc_envio.ID_TAREA_WF = 0
                Else
                    struc_envio.ID_TAREA_WF = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    struc_envio.FECHA_REGISTRO = ""
                Else
                    struc_envio.FECHA_REGISTRO = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    struc_envio.FECHA_VENCE = ""
                Else
                    struc_envio.FECHA_VENCE = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) = True Then
                    struc_envio.FECHA_RESPUETA = ""
                Else
                    struc_envio.FECHA_RESPUETA = Datset.Tables(0).Rows(0).Item(9)
                End If
                If Datset.Tables(0).Rows(0).IsNull(10) = True Then
                    struc_envio.HORA_RESPUESTA = ""
                Else
                    struc_envio.HORA_RESPUESTA = Datset.Tables(0).Rows(0).Item(10)
                End If
                If Datset.Tables(0).Rows(0).IsNull(11) = True Then
                    struc_envio.TIEMPO_RESPUESTA = 0
                Else
                    struc_envio.TIEMPO_RESPUESTA = Datset.Tables(0).Rows(0).Item(11)
                End If
                If Datset.Tables(0).Rows(0).IsNull(12) = True Then
                    struc_envio.RADICADO_RESPUESTA = ""
                Else
                    struc_envio.RADICADO_RESPUESTA = Datset.Tables(0).Rows(0).Item(12)
                End If
                If Datset.Tables(0).Rows(0).IsNull(13) = True Then
                    struc_envio.MEDIO_ENVIO = ""
                Else
                    struc_envio.MEDIO_ENVIO = Datset.Tables(0).Rows(0).Item(13)
                End If
                If Datset.Tables(0).Rows(0).IsNull(14) = True Then
                    struc_envio.EMPRESA_ENVIO = ""
                Else
                    struc_envio.EMPRESA_ENVIO = Datset.Tables(0).Rows(0).Item(14)
                End If
                If Datset.Tables(0).Rows(0).IsNull(15) = True Then
                    struc_envio.GUIA_ENVIO = ""
                Else
                    struc_envio.GUIA_ENVIO = Datset.Tables(0).Rows(0).Item(15)
                End If
                If Datset.Tables(0).Rows(0).IsNull(16) = True Then
                    struc_envio.FECHA_ENVIO = ""
                Else
                    struc_envio.FECHA_ENVIO = Datset.Tables(0).Rows(0).Item(16)
                End If
                If Datset.Tables(0).Rows(0).IsNull(17) = True Then
                    struc_envio.HORA_ENVIO = ""
                Else
                    struc_envio.HORA_ENVIO = Datset.Tables(0).Rows(0).Item(17)
                End If
                If Datset.Tables(0).Rows(0).IsNull(18) = True Then
                    struc_envio.ID_USUARIO_RADICADO = 0
                Else
                    struc_envio.ID_USUARIO_RADICADO = Datset.Tables(0).Rows(0).Item(18)
                End If
                If Datset.Tables(0).Rows(0).IsNull(19) = True Then
                    struc_envio.ID_IMAGEN = 0
                Else
                    struc_envio.ID_IMAGEN = Datset.Tables(0).Rows(0).Item(19)
                End If
                If Datset.Tables(0).Rows(0).IsNull(20) = True Then
                    struc_envio.GABINETE = ""
                Else
                    struc_envio.GABINETE = Datset.Tables(0).Rows(0).Item(20)
                End If
                If Datset.Tables(0).Rows(0).IsNull(21) = True Then
                    struc_envio.NOTA_RESPUESTA = ""
                Else
                    struc_envio.NOTA_RESPUESTA = Datset.Tables(0).Rows(0).Item(21)
                End If
                If Datset.Tables(0).Rows(0).IsNull(22) = True Then
                    struc_envio.FECHA_RECIBO_FISICO = ""
                Else
                    struc_envio.FECHA_RECIBO_FISICO = Datset.Tables(0).Rows(0).Item(22)
                End If
                If Datset.Tables(0).Rows(0).IsNull(23) = True Then
                    struc_envio.HORA_RECIBO_FISICO = ""
                Else
                    struc_envio.HORA_RECIBO_FISICO = Datset.Tables(0).Rows(0).Item(23)
                End If
                If Datset.Tables(0).Rows(0).IsNull(24) = True Then
                    struc_envio.DESTINATARIO = ""
                Else
                    struc_envio.DESTINATARIO = Datset.Tables(0).Rows(0).Item(24)
                End If
                If Datset.Tables(0).Rows(0).IsNull(25) = True Then
                    struc_envio.DIRECCION_DESTINATARIO = ""
                Else
                    struc_envio.DIRECCION_DESTINATARIO = Datset.Tables(0).Rows(0).Item(25)
                End If
                If Datset.Tables(0).Rows(0).IsNull(26) = True Then
                    struc_envio.TRAMITE_DOCUMENTO = ""
                Else
                    struc_envio.TRAMITE_DOCUMENTO = Datset.Tables(0).Rows(0).Item(26)
                End If
                If Datset.Tables(0).Rows(0).IsNull(27) = True Then
                    struc_envio.ESTADO_ENVIO = 0
                Else
                    struc_envio.ESTADO_ENVIO = Datset.Tables(0).Rows(0).Item(27)
                End If
                If Datset.Tables(0).Rows(0).IsNull(28) = True Then
                    struc_envio.ESTADO_RESPUESTA = 0
                Else
                    struc_envio.ESTADO_RESPUESTA = Datset.Tables(0).Rows(0).Item(28)
                End If
                SolicitaEstructuraRespuestaRadicado = "YES"
                Exit Function
            Else
                SolicitaEstructuraRespuestaRadicado = "Imposible encontrar estructura de tramite de respuesta numero " & IdRespuestaRadicado
            End If
        Catch ex As Exception
            SolicitaEstructuraRespuestaRadicado = "Inconsistencia general funcion SolicitaEstructuraRespuestaRadicado " & ex.Message
        End Try
    End Function

    Function Solicita_datos_estructura_envio_por_id_respuesta(ByVal id_respuesta As Integer,
                                                             ByRef struc_envio As stru_envio,
                                                             Optional ByVal op_confirma As Integer = 0) As String
        Dim Parametro_Consulta As String = "Select ID_RESPUESTA_RADICADO,ID_REMIT_DEST_INT,ID_AREA," &
            "system_plantilla_radicado_id_plantilla,RADICADO,ID_RUTA_WF,ID_TAREA_WF,FECHA_REGISTRO," &
            "FECHA_VENCE,FECHA_RESPUETA,HORA_RESPUESTA,TIEMPO_RESPUESTA,RADICADO_RESPUESTA,MEDIO_ENVIO," &
            "EMPRESA_ENVIO,GUIA_ENVIO,FECHA_ENVIO,HORA_ENVIO,ID_USUARIO_RADICADO,ID_IMAGEN,GABINETE," &
            "NOTA_RESPUESTA,FECHA_RECIBO_FISICO,HORA_RECIBO_FISICO,DESTINATARIO,DIRECCION_DESTINATARIO," &
            "TRAMITE_DOCUMENTO,ESTADO_ENVIO,ESTADO_RESPUESTA,id_usuario_gestion_propietario,ASUNTO," &
            "codigo_dest_externo,USUARIO_RESPONSABLE,ID_IMAGEN_RESPUESTA,estado_envio_correo," &
            "TIPO_RESPUESTA_ELAB_USUARIO,ESTADO_APROBACION,TIEMPO_RESPUESTA_APROBACION," &
            "NOTA_SOLICITUD,DESCRIPCION_ESTADO_APROBACION,FECHA_REGISTRO_SOLICITUD,FECHA_REGISTRO_APROBACION," &
            "ID_SOLICITUDES_APROBACION_RESP,ID_TIPO_DOC_RESPUESTA,FECHA_REGISTRO_EVIO_CORREO," &
            "CORREO_NOTIFICACION,FECHA_CONFIRMACION_CORREO_RECIBIDO,IP_CONFIRMACION_CORREO_RECIBIDO," &
            "HUELLA_CORREO_RECIBIDO" &
            " from ra_respuesta_radicado where ID_RESPUESTA_RADICADO='" & id_respuesta & "' limit 1"
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim ref_clas_gestion_fecha As New ClassGestionFechas
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_datos_estructura_envio_por_id_respuesta = "Función Solicita_datos_estructura_envio_por_id_respuesta dice  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then

                If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                    struc_envio.ID_RESPUESTA_RADICADO = 0
                Else
                    struc_envio.ID_RESPUESTA_RADICADO = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = True Then
                    struc_envio.ID_REMIT_DEST_INT = 0
                Else
                    struc_envio.ID_REMIT_DEST_INT = Dat_reader.Tables(0).Rows(0).Item(1)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(2) = True Then
                    struc_envio.ID_AREA = 0
                Else
                    struc_envio.ID_AREA = Dat_reader.Tables(0).Rows(0).Item(2)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(3) = True Then
                    struc_envio.system_plantilla_radicado_id_plantilla = 0
                Else
                    struc_envio.system_plantilla_radicado_id_plantilla = Dat_reader.Tables(0).Rows(0).Item(3)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(4) = True Then
                    struc_envio.RADICADO = ""
                Else
                    struc_envio.RADICADO = Dat_reader.Tables(0).Rows(0).Item(4)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(5) = True Then
                    struc_envio.ID_RUTA_WF = 0
                Else
                    struc_envio.ID_RUTA_WF = Dat_reader.Tables(0).Rows(0).Item(5)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(6) = True Then
                    struc_envio.ID_TAREA_WF = 0
                Else
                    struc_envio.ID_TAREA_WF = Dat_reader.Tables(0).Rows(0).Item(6)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(7) = True Then
                    struc_envio.FECHA_REGISTRO = ""
                Else
                    Dim fech_ref_date_db As Date = Dat_reader.Tables(0).Rows(0).Item(7)
                    Result = ref_clas_gestion_fecha.FormateaFechaTimeDbDefault(fech_ref_date_db,
                                                                                   struc_envio.FECHA_REGISTRO)
                    If Result <> "YES" Then
                        Solicita_datos_estructura_envio_por_id_respuesta = "Funcion Solicita_datos_estructura_envio_por_id_respuesta dice error formatendo fecha (" & fech_ref_date_db & ")" & Result
                        Exit Function
                    End If
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(8) = True Then
                    struc_envio.FECHA_VENCE = ""
                Else
                    Dim fech_ref_date_db As Date = Dat_reader.Tables(0).Rows(0).Item(8)
                    Result = ref_clas_gestion_fecha.FormateaFechaTimeDbDefault(fech_ref_date_db, struc_envio.FECHA_VENCE)
                    If Result <> "YES" Then
                        Solicita_datos_estructura_envio_por_id_respuesta = "Funcion Solicita_datos_estructura_envio_por_id_respuesta dice error formatendo fecha (" & fech_ref_date_db & ")" & Result
                        Exit Function
                    End If
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(9) = True Then
                    struc_envio.FECHA_RESPUETA = ""
                Else
                    Dim fech_ref_date_db As Date = Dat_reader.Tables(0).Rows(0).Item(9)
                    Result = ref_clas_gestion_fecha.FormateaFechaTimeDbDefault(fech_ref_date_db, struc_envio.FECHA_RESPUETA)
                    If Result <> "YES" Then
                        Solicita_datos_estructura_envio_por_id_respuesta = "Funcion Solicita_datos_estructura_envio_por_id_respuesta dice error formatendo fecha (" & fech_ref_date_db & ")" & Result
                        Exit Function
                    End If
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(10) = True Then
                    struc_envio.HORA_RESPUESTA = ""
                Else
                    struc_envio.HORA_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(10)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(11) = True Then
                    struc_envio.TIEMPO_RESPUESTA = 0
                Else
                    struc_envio.TIEMPO_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(11)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(12) = True Then
                    struc_envio.RADICADO_RESPUESTA = ""
                Else
                    struc_envio.RADICADO_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(12)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(13) = True Then
                    struc_envio.MEDIO_ENVIO = ""
                Else
                    struc_envio.MEDIO_ENVIO = Dat_reader.Tables(0).Rows(0).Item(13)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(14) = True Then
                    struc_envio.EMPRESA_ENVIO = ""
                Else
                    struc_envio.EMPRESA_ENVIO = Dat_reader.Tables(0).Rows(0).Item(14)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(15) = True Then
                    struc_envio.GUIA_ENVIO = ""
                Else
                    struc_envio.GUIA_ENVIO = Dat_reader.Tables(0).Rows(0).Item(15)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(16) = True Then
                    struc_envio.FECHA_ENVIO = ""
                Else
                    struc_envio.FECHA_ENVIO = Dat_reader.Tables(0).Rows(0).Item(16)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(17) = True Then
                    struc_envio.HORA_ENVIO = ""
                Else
                    struc_envio.HORA_ENVIO = Dat_reader.Tables(0).Rows(0).Item(17)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(18) = True Then
                    struc_envio.ID_USUARIO_RADICADO = 0
                Else
                    struc_envio.ID_USUARIO_RADICADO = Dat_reader.Tables(0).Rows(0).Item(18)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(19) = True Then
                    struc_envio.ID_IMAGEN = 0
                Else
                    struc_envio.ID_IMAGEN = Dat_reader.Tables(0).Rows(0).Item(19)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(20) = True Then
                    struc_envio.GABINETE = ""
                Else
                    struc_envio.GABINETE = Dat_reader.Tables(0).Rows(0).Item(20)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(21) = True Then
                    struc_envio.NOTA_RESPUESTA = ""
                Else
                    struc_envio.NOTA_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(21)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(22) = True Then
                    struc_envio.FECHA_RECIBO_FISICO = ""
                Else
                    struc_envio.FECHA_RECIBO_FISICO = Dat_reader.Tables(0).Rows(0).Item(22)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(23) = True Then
                    struc_envio.HORA_RECIBO_FISICO = ""
                Else
                    struc_envio.HORA_RECIBO_FISICO = Dat_reader.Tables(0).Rows(0).Item(23)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(24) = True Then
                    struc_envio.DESTINATARIO = ""
                Else
                    struc_envio.DESTINATARIO = Dat_reader.Tables(0).Rows(0).Item(24)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(25) = True Then
                    struc_envio.DIRECCION_DESTINATARIO = ""
                Else
                    struc_envio.DIRECCION_DESTINATARIO = Dat_reader.Tables(0).Rows(0).Item(25)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(26) = True Then
                    struc_envio.TRAMITE_DOCUMENTO = ""
                Else
                    struc_envio.TRAMITE_DOCUMENTO = Dat_reader.Tables(0).Rows(0).Item(26)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(27) = True Then
                    struc_envio.ESTADO_ENVIO = 0
                Else
                    struc_envio.ESTADO_ENVIO = Dat_reader.Tables(0).Rows(0).Item(27)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(28) = True Then
                    struc_envio.ESTADO_RESPUESTA = 0
                Else
                    struc_envio.ESTADO_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(28)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(29) = True Then
                    struc_envio.id_usuario_gestion_propietario = 0
                Else
                    struc_envio.id_usuario_gestion_propietario = Dat_reader.Tables(0).Rows(0).Item(29)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(30) = True Then
                    struc_envio.ASUNTO = 0
                Else
                    struc_envio.ASUNTO = Dat_reader.Tables(0).Rows(0).Item(30)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(31) = True Then
                    struc_envio.codigo_dest_externo = 0
                Else
                    struc_envio.codigo_dest_externo = Dat_reader.Tables(0).Rows(0).Item(31)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(32) = True Then
                    struc_envio.USUARIO_RESPONSABLE = ""
                Else
                    struc_envio.USUARIO_RESPONSABLE = Dat_reader.Tables(0).Rows(0).Item(32)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(33) = True Then
                    struc_envio.ID_IMAGEN_RESPUESTA = 0
                Else
                    struc_envio.ID_IMAGEN_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(33)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(34) = True Then
                    struc_envio.estado_envio_correo = 0
                Else
                    struc_envio.estado_envio_correo = Dat_reader.Tables(0).Rows(0).Item(34)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(35) = True Then
                    struc_envio.TIPO_RESPUESTA_ELAB_USUARIO = 0
                Else
                    struc_envio.TIPO_RESPUESTA_ELAB_USUARIO = Dat_reader.Tables(0).Rows(0).Item(35)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(36) = True Then
                    struc_envio.ESTADO_APROBACION = 0
                Else
                    struc_envio.ESTADO_APROBACION = Dat_reader.Tables(0).Rows(0).Item(36)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(37) = True Then
                    struc_envio.TIEMPO_RESPUESTA_APROBACION = 0
                Else
                    struc_envio.TIEMPO_RESPUESTA_APROBACION = Dat_reader.Tables(0).Rows(0).Item(37)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(38) = True Then
                    struc_envio.NOTA_SOLICITUD = ""
                Else
                    struc_envio.NOTA_SOLICITUD = Dat_reader.Tables(0).Rows(0).Item(38)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(39) = True Then
                    struc_envio.DESCRIPCION_ESTADO_APROBACION = ""
                Else
                    struc_envio.DESCRIPCION_ESTADO_APROBACION = Dat_reader.Tables(0).Rows(0).Item(39)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(40) = True Then
                    struc_envio.FECHA_REGISTRO_SOLICITUD = ""
                Else
                    Dim fech_ref_date_db As Date = Dat_reader.Tables(0).Rows(0).Item(40)
                    Result = ref_clas_gestion_fecha.FormateaFechaTimeDbDefault(fech_ref_date_db,
                                                                                   struc_envio.FECHA_REGISTRO_SOLICITUD)
                    If Result <> "YES" Then
                        Solicita_datos_estructura_envio_por_id_respuesta = "Funcion Solicita_datos_estructura_envio_por_id_respuesta dice error formatendo fecha (" & fech_ref_date_db & ")" & Result
                        Exit Function
                    End If

                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(41) = True Then
                    struc_envio.FECHA_REGISTRO_APROBACION = ""
                Else
                    Dim fech_ref_date_db As Date = Dat_reader.Tables(0).Rows(0).Item(41)
                    Result = ref_clas_gestion_fecha.FormateaFechaTimeDbDefault(fech_ref_date_db,
                                                                                   struc_envio.FECHA_REGISTRO_APROBACION)
                    If Result <> "YES" Then
                        Solicita_datos_estructura_envio_por_id_respuesta = "Funcion Solicita_datos_estructura_envio_por_id_respuesta dice error formatendo fecha (" & fech_ref_date_db & ")" & Result
                        Exit Function
                    End If
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(42) = True Then
                    struc_envio.ID_SOLICITUDES_APROBACION_RESP = 0
                Else
                    struc_envio.ID_SOLICITUDES_APROBACION_RESP = Dat_reader.Tables(0).Rows(0).Item(42)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(43) = True Then
                    struc_envio.ID_TIPO_DOC_RESPUESTA = 0
                Else
                    struc_envio.ID_TIPO_DOC_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(43)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(44) = True Then
                    struc_envio.FECHA_REGISTRO_EVIO_CORREO = ""
                Else
                    struc_envio.FECHA_REGISTRO_EVIO_CORREO = Dat_reader.Tables(0).Rows(0).Item(44)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(45) = True Then
                    struc_envio.CORREO_NOTIFICACION = ""
                Else
                    struc_envio.CORREO_NOTIFICACION = Dat_reader.Tables(0).Rows(0).Item(45)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(46) = True Then
                    struc_envio.FECHA_CONFIRMACION_CORREO_RECIBIDO = ""
                Else
                    struc_envio.FECHA_CONFIRMACION_CORREO_RECIBIDO = Dat_reader.Tables(0).Rows(0).Item(46)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(47) = True Then
                    struc_envio.IP_CONFIRMACION_CORREO_RECIBIDO = ""
                Else
                    struc_envio.IP_CONFIRMACION_CORREO_RECIBIDO = Dat_reader.Tables(0).Rows(0).Item(47)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(48) = True Then
                    struc_envio.HUELLA_CORREO_RECIBIDO = ""
                Else
                    struc_envio.HUELLA_CORREO_RECIBIDO = Dat_reader.Tables(0).Rows(0).Item(48)
                End If
                Solicita_datos_estructura_envio_por_id_respuesta = "YES"
                Exit Function
            Else

                If op_confirma = 0 Then
                    Solicita_datos_estructura_envio_por_id_respuesta = "El radicado  no tiene respuesta relacionada"
                    Exit Function
                Else
                    Solicita_datos_estructura_envio_por_id_respuesta = "YES"
                    Exit Function
                End If

            End If
        Catch ex As Exception
            Solicita_datos_estructura_envio_por_id_respuesta = "Inconsistencia función Solicita_datos_estructura_envio_por_id_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_estados_semaforo_respuesta_electronica(ByVal id_respuesta As Integer,
                                                            ByRef imag As Image) As String
        '*************************************************************************
        'Función : Retorna estados_semaforo respuesta a correo electronico
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT ID_IMAGEN,FECHA_RESPUETA,RADICADO_RESPUESTA,estado_envio_correo,ID_IMAGEN_RESPUESTA,ESTADO_RESPUESTA" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estados_semaforo_respuesta_electronica = "Función Solicita_estados_semaforo_respuesta_electronica dice " & Result
                Exit Function
            End If
            Dim estado_final As Integer = 0
            Dim estado_plantilla As Integer = 0
            Dim estado_radicado As Integer = 0
            Dim estado_respuesta As Integer = 0
            Dim estado_envio_correo As Integer = 0
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estados_semaforo_respuesta_electronica = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If HttpContext.Current.Session.Item("GA_TIPO_MODULO_RESPUESTA") = 0 Then
                    If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                        estado_final = 1
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                        estado_final = 2
                        estado_respuesta = 1
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                        If estado_respuesta = 1 Then
                            estado_final = 3
                            estado_radicado = 1
                        End If

                    End If
                    If Datset.Tables(0).Rows(0).IsNull(3) = False Then

                        If Datset.Tables(0).Rows(0).Item(3) <> 0 And estado_radicado = 1 Then
                            estado_final = 4
                            estado_envio_correo = 1
                        End If

                    End If
                    If Datset.Tables(0).Rows(0).IsNull(4) = False Then
                        If estado_envio_correo = 1 Then
                            estado_final = 5
                        Else
                            estado_final = 6
                        End If

                    End If
                    imag.ImageUrl = "../radicador/imagenes/electronica_resp_estado_" & estado_final & ".png"
                    Solicita_estados_semaforo_respuesta_electronica = "YES"
                    Exit Function
                Else
                    'Radicacion respuesta
                    If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                        estado_final = 1
                    End If
                    'guarda respuesta
                    If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                        estado_final = 2
                    End If
                    'confirma respuesta
                    If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                        estado_final = 3
                    End If
                    'Evio respuesta
                    If Datset.Tables(0).Rows(0).Item(5) <> 0 And Datset.Tables(0).Rows(0).Item(5) <> 4 Then
                        estado_final = 4
                    End If
                    imag.ImageUrl = "../radicador/imagenes/electronica_resp_estado_V" & estado_final & ".png"
                    Solicita_estados_semaforo_respuesta_electronica = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_estados_semaforo_respuesta_electronica = "Inconsistencia general función Solicita_estados_semaforo_respuesta_electronica " & ex.Message
        End Try
    End Function
    Function Solicita_estados_semaforo_respuesta_electronica(ByVal id_respuesta As Integer,
                                                             ByRef imag_url As String) As String
        '*************************************************************************
        'Función : Retorna estados_semaforo respuesta a correo electronico
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT ID_IMAGEN,FECHA_RESPUETA,RADICADO_RESPUESTA,estado_envio_correo,ID_IMAGEN_RESPUESTA,ESTADO_RESPUESTA" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estados_semaforo_respuesta_electronica = "Función Solicita_estados_semaforo_respuesta_electronica dice " & Result
                Exit Function
            End If
            Dim estado_final As Integer = 0
            Dim estado_plantilla As Integer = 0
            Dim estado_radicado As Integer = 0
            Dim estado_respuesta As Integer = 0
            Dim estado_envio_correo As Integer = 0
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estados_semaforo_respuesta_electronica = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If HttpContext.Current.Session.Item("GA_TIPO_MODULO_RESPUESTA") = 0 Then
                    If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                        estado_final = 1
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                        estado_final = 2
                        estado_respuesta = 1
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                        If estado_respuesta = 1 Then
                            estado_final = 3
                            estado_radicado = 1
                        End If

                    End If
                    If Datset.Tables(0).Rows(0).IsNull(3) = False Then

                        If Datset.Tables(0).Rows(0).Item(3) <> 0 And estado_radicado = 1 Then
                            estado_final = 4
                            estado_envio_correo = 1
                        End If

                    End If
                    If Datset.Tables(0).Rows(0).IsNull(4) = False Then
                        If estado_envio_correo = 1 Then
                            estado_final = 5
                        Else
                            estado_final = 6
                        End If

                    End If
                    imag_url = "../radicador/imagenes/electronica_resp_estado_" & estado_final & ".png"
                    Solicita_estados_semaforo_respuesta_electronica = "YES"
                    Exit Function
                Else
                    'Radicacion respuesta
                    If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                        estado_final = 1
                    End If
                    'guarda respuesta
                    If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                        estado_final = 2
                    End If
                    'confirma respuesta
                    If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                        estado_final = 3
                    End If
                    'Evio respuesta
                    If Datset.Tables(0).Rows(0).Item(5) <> 0 And Datset.Tables(0).Rows(0).Item(5) <> 4 Then
                        estado_final = 4
                    End If
                    imag_url = "../radicador/imagenes/electronica_resp_estado_V" & estado_final & ".png"
                    Solicita_estados_semaforo_respuesta_electronica = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_estados_semaforo_respuesta_electronica = "Inconsistencia general función Solicita_estados_semaforo_respuesta_electronica " & ex.Message
        End Try
    End Function
    Function Solicita_estados_semaforo_respuesta(ByVal id_respuesta As Integer,
                                                ByRef imag As Image) As String
        '*************************************************************************
        'Función : Retorna estados_semaforo
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT ID_IMAGEN,FECHA_RESPUETA,RADICADO_RESPUESTA,estado_envio_correo,ID_IMAGEN_RESPUESTA,ESTADO_RESPUESTA" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estados_semaforo_respuesta = "Función Solicita_estados_semaforo_respuesta dice " & Result
                Exit Function
            End If
            Dim estado_final As Integer = 0
            Dim estado_plantilla As Integer = 1
            Dim estado_radicado As Integer = 1
            Dim estado_respuesta As Integer = 1
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estados_semaforo_respuesta = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If HttpContext.Current.Session.Item("GA_TIPO_MODULO_RESPUESTA") = 0 Then
                    If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                        estado_final = 1
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                        estado_final = 2
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                        estado_final = 3
                    End If
                    imag.ImageUrl = "../radicador/imagenes/resp_estado_" & estado_final & ".png"
                    Solicita_estados_semaforo_respuesta = "YES"
                    Exit Function
                Else
                    'Radicacion respuesta
                    If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                        estado_final = 1
                    End If
                    'guarda respuesta
                    If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                        estado_final = 2
                    End If
                    'confirma respuesta
                    If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                        estado_final = 3
                    End If
                    'Evio respuesta
                    If Datset.Tables(0).Rows(0).Item(5) <> 0 And Datset.Tables(0).Rows(0).Item(5) <> 4 Then
                        estado_final = 4
                    End If
                    imag.ImageUrl = "../radicador/imagenes/electronica_resp_estado_V" & estado_final & ".png"
                    Solicita_estados_semaforo_respuesta = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_estados_semaforo_respuesta = "Inconsistencia general función Solicita_estados_semaforo_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_estados_semaforo_respuesta(ByVal id_respuesta As Integer,
                                                 ByRef imag_url As String) As String
        '*************************************************************************
        'Función : Retorna estados_semaforo
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT ID_IMAGEN,FECHA_RESPUETA,RADICADO_RESPUESTA,estado_envio_correo,ID_IMAGEN_RESPUESTA,ESTADO_RESPUESTA" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estados_semaforo_respuesta = "Función Solicita_estados_semaforo_respuesta dice " & Result
                Exit Function
            End If
            Dim estado_final As Integer = 0
            Dim estado_plantilla As Integer = 1
            Dim estado_radicado As Integer = 1
            Dim estado_respuesta As Integer = 1
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estados_semaforo_respuesta = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If HttpContext.Current.Session.Item("GA_TIPO_MODULO_RESPUESTA") = 0 Then
                    If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                        estado_final = 1
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                        estado_final = 2
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                        estado_final = 3
                    End If
                    imag_url = "../radicador/imagenes/resp_estado_" & estado_final & ".png"
                    Solicita_estados_semaforo_respuesta = "YES"
                    Exit Function
                Else
                    'Radicacion respuesta
                    If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                        estado_final = 1
                    End If
                    'guarda respuesta
                    If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                        estado_final = 2
                    End If
                    'confirma respuesta
                    If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                        estado_final = 3
                    End If
                    'Evio respuesta
                    If Datset.Tables(0).Rows(0).Item(5) <> 0 And Datset.Tables(0).Rows(0).Item(5) <> 4 Then
                        estado_final = 4
                    End If
                    imag_url = "../radicador/imagenes/electronica_resp_estado_V" & estado_final & ".png"
                    Solicita_estados_semaforo_respuesta = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_estados_semaforo_respuesta = "Inconsistencia general función Solicita_estados_semaforo_respuesta " & ex.Message
        End Try
    End Function
    Function Retorna_estados_respeuesta_documento(ByVal id_respuesta As Integer,
                                                  ByRef id_imagen_plantilla As Integer,
                                                  ByRef radicado_respuesta As Integer,
                                                  ByRef fecha_respuesta As Integer,
                                                  ByRef id_imagen_respuesta As Integer,
                                                  ByRef estado_envio_respuesta As Integer) As String
        '*************************************************************************
        'Función : Retorna_estados_respeuesta_documento
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT ID_IMAGEN,RADICADO_RESPUESTA,FECHA_RESPUETA,ID_IMAGEN_RESPUESTA,estado_envio_correo" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_respeuesta_documento = "Función Retorna_estados_respeuesta_documento dice " & Result
                Exit Function
            End If
            Dim estado_final As Integer = 0
            Dim estado_plantilla As Integer = 1
            Dim estado_radicado As Integer = 1
            Dim estado_respuesta As Integer = 1
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estados_respeuesta_documento = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    id_imagen_plantilla = 1
                Else
                    id_imagen_plantilla = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                    radicado_respuesta = 1
                Else
                    radicado_respuesta = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                    fecha_respuesta = 1
                Else
                    fecha_respuesta = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = False Then
                    id_imagen_respuesta = 1
                Else
                    id_imagen_respuesta = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = False Then
                    estado_envio_respuesta = Datset.Tables(0).Rows(0).Item(4)
                Else
                    estado_envio_respuesta = 0
                End If
                Retorna_estados_respeuesta_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_respeuesta_documento = "Inconsistencia general función Retorna_estados_respeuesta_documento " & ex.Message
        End Try

    End Function
    Function Actualiza_estado_envio_correo_notificacion(ByVal id_respuesta As Long,
                                                        ByVal correo_notificado As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGestionFechas
            Dim Fecha As String = ""
            Result = Refclas.Formatea_fecha_time_framework(Now, Fecha)
            If Result <> "YES" Then
                Actualiza_estado_envio_correo_notificacion = Result
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim sql As String = "update ra_respuesta_radicado set FECHA_REGISTRO_EVIO_CORREO='" & Fecha & "'," &
                "CORREO_NOTIFICACION='" & correo_notificado & "' where ID_RESPUESTA_RADICADO=" & id_respuesta
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql)
            If Result <> "YES" Then
                Actualiza_estado_envio_correo_notificacion = Result
                Exit Function
            End If
            Actualiza_estado_envio_correo_notificacion = "YES"
        Catch ex As Exception
            Actualiza_estado_envio_correo_notificacion = "Inconsistencia general función Actualiza_estado_envio_correo_notificacion " & ex.Message
        End Try
    End Function
    Function Solicita_estado_recibido_respuesta_usuario(ByVal id_respuesta As Integer,
                                                        ByRef estado_confirmacion As String) As String
        Try
            Dim Parametro_Consulta As String = "SELECT FECHA_CONFIRMACION_CORREO_RECIBIDO " &
              " FROM ra_respuesta_radicado " &
             " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_recibido_respuesta_usuario = "Función Solicita_estado_recibido_respuesta_usuario dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_recibido_respuesta_usuario = "Imposible econtrar el registro del la respuesta " & id_respuesta
                Exit Function
            End If
            If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                estado_confirmacion = "NO"
            Else
                estado_confirmacion = "YES"
            End If
            Solicita_estado_recibido_respuesta_usuario = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_estado_recibido_respuesta_usuario = "Inconsistencia general función Solicita_estado_recibido_respuesta_usuario " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_recibido_respuesta(ByVal id_respuesta As Integer) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGestionFechas
            Dim Fecha As String = ""
            Result = Refclas.Formatea_fecha_time_framework(Now, Fecha)
            If Result <> "YES" Then
                Actualiza_estado_recibido_respuesta = Result
                Exit Function
            End If
            Dim identi_host As String = ""
            identi_host = ModuleGeneral.Identi_Host()
            Dim huella As String = ""
            encriptacion.encript_md5(id_respuesta & "|" & Fecha & "|" & identi_host,
                                          "7894561230!",
                                           huella)
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim sql As String = "update ra_respuesta_radicado set FECHA_CONFIRMACION_CORREO_RECIBIDO='" & Fecha & "'," &
                "IP_CONFIRMACION_CORREO_RECIBIDO='" & identi_host & "',HUELLA_CORREO_RECIBIDO='" & huella & "'" & "  where ID_RESPUESTA_RADICADO=" & id_respuesta
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql)
            If Result <> "YES" Then
                Actualiza_estado_recibido_respuesta = Result
                Exit Function
            End If
            Actualiza_estado_recibido_respuesta = "YES"
        Catch ex As Exception
            Actualiza_estado_recibido_respuesta = "Inconsistencia general función Actualiza_estado_recibido_respuesta " & ex.Message
        End Try
    End Function
End Class
