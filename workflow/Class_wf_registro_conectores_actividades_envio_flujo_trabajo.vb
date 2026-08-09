Public Structure stru_config_conector_flujo
    Dim Estado_evia_correo As Integer
    Dim Estado_soicita_autorizacion As Integer
    Dim Estado_soicita_autorizacion_firma_digital As Integer
    Dim Estado_copia_documento_estructura As Integer
    Dim Estado_asigna_expediente As Integer
    Dim Estado_firma_digital As Integer
    Dim estado_valida_balanceo As Integer
    Dim Estado_copia_estructura_total As Integer
End Structure
Public Structure stru_conector_flujo
    Dim wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO As Integer
    Dim ID_ACTIVIDAD_FUENTE As Integer
    Dim ID_ACTIVIDAD_DESTINO As Integer
    Dim IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE As Integer
    Dim IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO As Integer
    Dim ID_USUARIO_WORKFLOW_FUENTE As Integer
    Dim ID_USUARIO_WORKFLOW_DESTINO As Integer
    Dim Estado_evia_correo As Integer
    Dim Estado_soicita_autorizacion As Integer
    Dim Estado_soicita_autorizacion_firma_digital As Integer
    Dim Estado_copia_documento_estructura As Integer
    Dim Estado_asigna_expediente As Integer
End Structure
Public Class Class_wf_registro_conectores_actividades_envio_flujo_trabajo

    Function Solicita_datos_estructura_conector_flujo_trabajo(ByVal id_conector As Integer,
                                                              ByRef stru_conector_flujo_ As stru_conector_flujo) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "SELECT wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,ID_ACTIVIDAD_FUENTE,ID_ACTIVIDAD_DESTINO,IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE," &
                "IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,ID_USUARIO_WORKFLOW_FUENTE,ID_USUARIO_WORKFLOW_DESTINO,Estado_evia_correo,Estado_soicita_autorizacion,Estado_soicita_autorizacion_firma_digital" &
                ",Estado_copia_estructura,Estado_asigna_expediente " &
                " from wf_registro_conectores_actividades_envio_flujo_trabajo  " &
                " where ID_REGISTRO_ACTIVIDAD_ENVIO= " & id_conector
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_conector_flujo_trabajo = "Función Solicita_datos_estructura_conector_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_estructura_conector_flujo_trabajo = "Imposible encontrar el detalle  del conector de flujo  (" & id_conector & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru_conector_flujo_.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = 0
                Else
                    stru_conector_flujo_.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru_conector_flujo_.ID_ACTIVIDAD_FUENTE = 0
                Else
                    stru_conector_flujo_.ID_ACTIVIDAD_FUENTE = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru_conector_flujo_.ID_ACTIVIDAD_DESTINO = 0
                Else
                    stru_conector_flujo_.ID_ACTIVIDAD_DESTINO = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE = 0
                Else
                    stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO = 0
                Else
                    stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    stru_conector_flujo_.ID_USUARIO_WORKFLOW_FUENTE = 0
                Else
                    stru_conector_flujo_.ID_USUARIO_WORKFLOW_FUENTE = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO = 0
                Else
                    stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    stru_conector_flujo_.Estado_evia_correo = 0
                Else
                    stru_conector_flujo_.Estado_evia_correo = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    stru_conector_flujo_.Estado_soicita_autorizacion = 0
                Else
                    stru_conector_flujo_.Estado_soicita_autorizacion = Datset.Tables(0).Rows(0).Item(8)
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) = True Then
                    stru_conector_flujo_.Estado_soicita_autorizacion_firma_digital = 0
                Else
                    stru_conector_flujo_.Estado_soicita_autorizacion_firma_digital = Datset.Tables(0).Rows(0).Item(9)
                End If
                Solicita_datos_estructura_conector_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_conector_flujo_trabajo = "Incosistencia general función Solicita_datos_estructura_conector_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_estado_notifica_envio_conector(ByVal id_conector As Integer,
                                                     ByRef estado_envio_correo As Integer) As String
        Try
            If id_conector = 0 Then
                Solicita_estado_notifica_envio_conector = "Por favor borre el cache de su navegador, para poder utilizar la opción de envío"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Estado_evia_correo " &
              " from wf_registro_conectores_actividades_envio_flujo_trabajo  " &
              " where ID_REGISTRO_ACTIVIDAD_ENVIO= " & id_conector
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_notifica_envio_conector = "Función Solicita_estado_notifica_envio_conector dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_notifica_envio_conector = "Imposible encontrar el estado de envió del conector de flujo  (" & id_conector & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estado_envio_correo = 0
                Else
                    estado_envio_correo = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_estado_notifica_envio_conector = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_notifica_envio_conector = "Inconsistencia general función Solicita_estado_notifica_envio_conector " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_envio_correo_conector_flujo_trabajo(ByVal id_conector As Integer,
                                                                  ByVal estado_envio_correo As Integer) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "UPDATE wf_registro_conectores_actividades_envio_flujo_trabajo set Estado_evia_correo=" & estado_envio_correo &
              "  where ID_REGISTRO_ACTIVIDAD_ENVIO=" & id_conector
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(Parametro_Consulta)
            If Result <> "YES" Then
                Actualiza_estado_envio_correo_conector_flujo_trabajo = "Función Actualiza_estado_envio_correo_conector_flujo_trabajo dice " & Result
                Exit Function
            Else
                Actualiza_estado_envio_correo_conector_flujo_trabajo = Result
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_estado_envio_correo_conector_flujo_trabajo = "Inconsistencia general función Actualiza_estado_envio_correo_conector_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_configuracion_conector_flujo(ByVal id_registro_actividad_envio As Integer,
                                                   ByRef stru_config_conector_flujo As stru_config_conector_flujo) As String
        Try
            If id_registro_actividad_envio = 0 Then
                Solicita_configuracion_conector_flujo = "Por favor borre el cache de su navegador, para poder utilizar la opción de envío"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Estado_evia_correo, " &
                "Estado_soicita_autorizacion,Estado_soicita_autorizacion_firma_digital" &
                ",Estado_copia_estructura,Estado_asigna_expediente,Estado_firma_digital,estado_valida_balanceo,Estado_copia_estructura_total " &
                " from wf_registro_conectores_actividades_envio_flujo_trabajo  " &
                " where ID_REGISTRO_ACTIVIDAD_ENVIO= " & id_registro_actividad_envio
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_configuracion_conector_flujo = "Función Solicita_configuracion_conector_flujo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_configuracion_conector_flujo = "Imposible encontrar la configuración del conector de flujo  (" & id_registro_actividad_envio & ")"
                Exit Function
            Else
                stru_config_conector_flujo.Estado_evia_correo = Datset.Tables(0).Rows(0).Item(0)
                stru_config_conector_flujo.Estado_soicita_autorizacion = Datset.Tables(0).Rows(0).Item(1)
                stru_config_conector_flujo.Estado_soicita_autorizacion_firma_digital = Datset.Tables(0).Rows(0).Item(2)
                stru_config_conector_flujo.Estado_copia_documento_estructura = Datset.Tables(0).Rows(0).Item(3)
                stru_config_conector_flujo.Estado_asigna_expediente = Datset.Tables(0).Rows(0).Item(4)
                stru_config_conector_flujo.Estado_firma_digital = Datset.Tables(0).Rows(0).Item(5)
                stru_config_conector_flujo.estado_valida_balanceo = Datset.Tables(0).Rows(0).Item(6)
                stru_config_conector_flujo.Estado_copia_estructura_total = Datset.Tables(0).Rows(0).Item(7)
                Solicita_configuracion_conector_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_configuracion_conector_flujo = "Inconsistencia general función Solicita_configuracion_conector_flujo " & ex.Message
        End Try
    End Function
    Function Actualiza_configuracion_conector(ByVal id_registro_actividad_envio As Integer,
                                              ByVal stru_config_conector_flujo As stru_config_conector_flujo) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "UPDATE wf_registro_conectores_actividades_envio_flujo_trabajo " &
                "set Estado_evia_correo=" & stru_config_conector_flujo.Estado_evia_correo &
                ", Estado_soicita_autorizacion=" & stru_config_conector_flujo.Estado_soicita_autorizacion &
                ", Estado_soicita_autorizacion_firma_digital=" & stru_config_conector_flujo.Estado_soicita_autorizacion_firma_digital &
                ", Estado_copia_estructura=" & stru_config_conector_flujo.Estado_copia_documento_estructura &
                ", Estado_asigna_expediente=" & stru_config_conector_flujo.Estado_asigna_expediente &
                ", estado_firma_digital=" & stru_config_conector_flujo.Estado_firma_digital &
                ", estado_valida_balanceo=" & stru_config_conector_flujo.estado_valida_balanceo &
                ", Estado_copia_estructura_total=" & stru_config_conector_flujo.Estado_copia_estructura_total &
              "  where ID_REGISTRO_ACTIVIDAD_ENVIO=" & id_registro_actividad_envio
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(Parametro_Consulta)
            If Result <> "YES" Then
                Actualiza_configuracion_conector = "Función Actualiza_configuracion_conector dice " & Result
                Exit Function
            Else
                Actualiza_configuracion_conector = Result
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_configuracion_conector = "Inconsistencia general función Actualiza_configuracion_conector " & ex.Message
        End Try
    End Function
End Class
