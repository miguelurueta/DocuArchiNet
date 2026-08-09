Public Class interface_config_digitaliza_
    Public Property id_config_digitalizacion As Integer
    Public Property error_gestion As String
    Public Property zoon_visor As String
    Public Property tumbail_visor As Integer
    Public Property vista_configuracion_escaner As Integer
    Public Property duplex_configuracion As Integer
    Public Property desc_pag_blanco_configuracion As Integer
    Public Property detect_borde_configuracion As Integer
    Public Property desk_configuracion As Integer
    Public Property adf_configuracion As Integer
    Public Property controlador_propio_configuracion As Integer
End Class
Public Class Class_ra_config_digitalizacion_user_gestion
    Function Solicita_existencia_configuracion_interface_digitalizacion(ByVal id_usuario_gestion As Integer,
                                                                        ByRef parameter_gestion As interface_config_digitaliza) As String
        '----------------------------------------------------------------------------------
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la configuración de la interface
        'de digitalización de un usuario de gestión logueado
        'con el parametro usuario de gestión
        '---------------------------------------------------------------------------------------------------------------------------------
        'Restorno 
        '----------------------------------------------------------------------------------------------------------------------------------
        '------------------------------
        'zoon_visor                  : Gauarda la configuración del zoon del visor de digitalizacion 10-100 valores retornados
        '----------------------------
        'tumbail_visor               : Gauarda la configuración si esta visible el tumbail del visor de componente de digitalización 1-0  valores retornados
        '----------------------------
        '-----------------------------
        'vista_configuracion_escaner : Gauarda la configuración si esta visible el menún de configuración del escaner 1-0   valores retornados
        '-----------------------------
        'duplex_configuracion        : Gauarda la configuración si esta activo o no activo el duplex para la digitalización 1-0 valores retornados
        '-----------------------------
        'desc_pag_blanco_configuracion: Gauarda la configuración si esta activo o no activo el descarte de pagina en blanco 1-0 valores retornados
        '-----------------------------
        'detect_borde_configuracion  : Gauarda la configuración si esta activo o no activo la correción de bordes  1-0 valores retornados
        '-----------------------------
        'controlador_propio_configuracion : Gauarda la configuración si esta activo o no activo la controlador propio de escaner
        '-----------------------------
        '-----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        'Fecha     : 2022-08-16
        '-----------
        '--------------------------------------------------------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select id_config_digitalizacion,zoon_visor,tumbail_visor," &
            "vista_configuracion_escaner,duplex_configuracion,desc_pag_blanco_configuracion," &
            "detect_borde_configuracion,controlador_propio_configuracion,adf_configuracion,desk_configuracion" &
            " from ra_config_digitalizacion_user_gestion where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_set As DataSet = New DataSet("ra_config_digitalizacion_user_gestion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_set)
            If Result <> "YES" Then
                Solicita_existencia_configuracion_interface_digitalizacion = " Error funcion Solicita_existencia_configuracion_interface_digitalizacion " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count = 0 Then
                parameter_gestion.id_config_digitalizacion = -1
                Solicita_existencia_configuracion_interface_digitalizacion = "YES"
                Exit Function
            Else
                parameter_gestion.id_config_digitalizacion = Dat_set.Tables(0).Rows(0).Item(0)
                parameter_gestion.zoon_visor = Dat_set.Tables(0).Rows(0).Item(1)
                parameter_gestion.tumbail_visor = Dat_set.Tables(0).Rows(0).Item(2)
                parameter_gestion.vista_configuracion_escaner = Dat_set.Tables(0).Rows(0).Item(3)
                parameter_gestion.duplex_configuracion = Dat_set.Tables(0).Rows(0).Item(4)
                parameter_gestion.desc_pag_blanco_configuracion = Dat_set.Tables(0).Rows(0).Item(5)
                parameter_gestion.detect_borde_configuracion = Dat_set.Tables(0).Rows(0).Item(6)
                parameter_gestion.controlador_propio_configuracion = Dat_set.Tables(0).Rows(0).Item(7)
                parameter_gestion.adf_configuracion = Dat_set.Tables(0).Rows(0).Item(8)
                parameter_gestion.desk_configuracion = Dat_set.Tables(0).Rows(0).Item(9)
                Solicita_existencia_configuracion_interface_digitalizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_configuracion_interface_digitalizacion = "Inconsistencia general funcion Solicita_existencia_configuracion_interface_digitalizacion " & ex.Message
        End Try
    End Function
    Function Registro_configuracion_interface_user_escaner(ByVal id_usuario_gestion As Integer,
                                                           ByRef parameter_gestion As interface_config_digitaliza) As String
        '----------------------------------------------------------------------------------
        '----------------------------------------------------------------------------------
        'Funcion : Registra la confguración default de la interface de digitalización
        '
        '
        '---------------------------------------------------------------------------------------------------------------------------------
        'Parametros de retorno y de actualización
        '----------------------------------------------------------------------------------------------------------------------------------
        '------------------------------
        'zoon_visor                  : Gauarda la configuración del zoon del visor de digitalizacion 10-100 valores retornados
        '----------------------------
        'tumbail_visor               : Gauarda la configuración si esta visible el tumbail del visor de componente de digitalización 1-0  valores retornados
        '----------------------------
        '-----------------------------
        'vista_configuracion_escaner : Gauarda la configuración si esta visible el menún de configuración del escaner 1-0   valores retornados
        '-----------------------------
        'duplex_configuracion        : Gauarda la configuración si esta activo o no activo el duplex para la digitalización 1-0 valores retornados
        '-----------------------------
        'desc_pag_blanco_configuracion: Gauarda la configuración si esta activo o no activo el descarte de pagina en blanco 1-0 valores retornados
        '-----------------------------
        'detect_borde_configuracion  : Gauarda la configuración si esta activo o no activo la correción de bordes  1-0 valores retornados
        '-----------------------------
        'controlador_propio_configuracion : Gauarda la configuración si esta activo o no activo la controlador propio de escaner
        '-----------------------------
        '-----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        'Fecha     : 2022-08-16
        '-----------
        '--------------------------------------------------------------------------------------------------------------------------------------------------
        Try
            parameter_gestion.id_config_digitalizacion = 0
            parameter_gestion.zoon_visor = 40
            parameter_gestion.tumbail_visor = 0
            parameter_gestion.vista_configuracion_escaner = 1
            parameter_gestion.duplex_configuracion = 0
            parameter_gestion.desc_pag_blanco_configuracion = 0
            parameter_gestion.detect_borde_configuracion = 0
            parameter_gestion.controlador_propio_configuracion = 0
            parameter_gestion.desk_configuracion = 0
            parameter_gestion.adf_configuracion = 1
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim sql_insert As String = "insert into ra_config_digitalizacion_user_gestion (" &
              "  zoon_visor,tumbail_visor," &
            "vista_configuracion_escaner,duplex_configuracion,desc_pag_blanco_configuracion," &
            "detect_borde_configuracion,controlador_propio_configuracion,remit_dest_interno_id_Remit_Dest_Int,adf_configuracion,desk_configuracion) values" &
            "(" & parameter_gestion.zoon_visor & ",'" & parameter_gestion.tumbail_visor & "'" &
            "," & parameter_gestion.vista_configuracion_escaner & "," & parameter_gestion.duplex_configuracion &
            "," & parameter_gestion.desc_pag_blanco_configuracion & "," & parameter_gestion.detect_borde_configuracion &
            "," & parameter_gestion.controlador_propio_configuracion & "," & id_usuario_gestion & "," & parameter_gestion.adf_configuracion &
            "," & parameter_gestion.desk_configuracion & ")"
            Result = Ref_Car_Conec.SELECTION_LAST_INSERT_COMMAND(sql_insert, parameter_gestion.id_config_digitalizacion)
            If Result <> "YES" Then
                Registro_configuracion_interface_user_escaner = Result
                Exit Function
            Else
                Registro_configuracion_interface_user_escaner = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registro_configuracion_interface_user_escaner = "Inconsistencia general funcion Registro_configuracion_interface_user_escaner " & ex.Message
        End Try
    End Function
    Function Actualiza_configuracion_interface_user_escaner(ByVal id_usuario_gestion As Integer,
                                                            ByRef parameter_gestion As interface_config_digitaliza) As String
        '----------------------------------------------------------------------------------
        '----------------------------------------------------------------------------------
        'Funcion : Actualiza la confguración default de la interface de digitalización
        '
        '
        '---------------------------------------------------------------------------------------------------------------------------------
        'Parametros de retorno y de actualización
        '----------------------------------------------------------------------------------------------------------------------------------
        '------------------------------
        'zoon_visor                  : Gauarda la configuración del zoon del visor de digitalizacion 10-100 valores retornados
        '----------------------------
        'tumbail_visor               : Gauarda la configuración si esta visible el tumbail del visor de componente de digitalización 1-0  valores retornados
        '----------------------------
        '-----------------------------
        'vista_configuracion_escaner : Gauarda la configuración si esta visible el menún de configuración del escaner 1-0   valores retornados
        '-----------------------------
        'duplex_configuracion        : Gauarda la configuración si esta activo o no activo el duplex para la digitalización 1-0 valores retornados
        '-----------------------------
        'desc_pag_blanco_configuracion: Gauarda la configuración si esta activo o no activo el descarte de pagina en blanco 1-0 valores retornados
        '-----------------------------
        'detect_borde_configuracion  : Gauarda la configuración si esta activo o no activo la correción de bordes  1-0 valores retornados
        '-----------------------------
        'controlador_propio_configuracion : Gauarda la configuración si esta activo o no activo la controlador propio de escaner
        '-----------------------------
        '-----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        'Fecha     : 2022-08-16
        '-----------
        '--------------------------------------------------------------------------------------------------------------------------------------------------
        Try

            parameter_gestion.id_config_digitalizacion = 0
            parameter_gestion.zoon_visor = 40
            parameter_gestion.tumbail_visor = 0
            parameter_gestion.vista_configuracion_escaner = 0
            parameter_gestion.duplex_configuracion = 0
            parameter_gestion.desc_pag_blanco_configuracion = 0
            parameter_gestion.detect_borde_configuracion = 0
            parameter_gestion.controlador_propio_configuracion = 0
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim sql_update As String = "update ra_config_digitalizacion_user_gestion " &
                " set zoon_visor=" & parameter_gestion.zoon_visor &
                " ,tumbail_visor=" & parameter_gestion.tumbail_visor &
                " ,vista_configuracion_escaner=" & parameter_gestion.vista_configuracion_escaner &
                " ,duplex_configuracion=" & parameter_gestion.duplex_configuracion &
                " ,desc_pag_blanco_configuracion=" & parameter_gestion.desc_pag_blanco_configuracion &
                " ,detect_borde_configuracion=" & parameter_gestion.detect_borde_configuracion &
                " ,controlador_propio_configuracion=" & parameter_gestion.controlador_propio_configuracion &
                " ,desk_configuracion=" & parameter_gestion.desk_configuracion &
                " ,adf_configuracion=" & parameter_gestion.adf_configuracion &
                " where remit_dest_interno_id_Remit_Dest_Int =" & id_usuario_gestion
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Actualiza_configuracion_interface_user_escaner = Result
                Exit Function
            Else
                Actualiza_configuracion_interface_user_escaner = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_configuracion_interface_user_escaner = "Inconsistencia general funcion Actualiza_configuracion_interface_user_escaner " & ex.Message
        End Try
    End Function
    Function Update_configuracion_interface_user_escaner(ByVal id_usuario_gestion As Integer,
                                                           ByRef parameter_gestion As interface_config_digitaliza) As String
        '----------------------------------------------------------------------------------
        '----------------------------------------------------------------------------------
        'Funcion : Actualiza la confguración default de la interface de digitalización
        '
        '
        '---------------------------------------------------------------------------------------------------------------------------------
        'Parametros de retorno y de actualización
        '----------------------------------------------------------------------------------------------------------------------------------
        '------------------------------
        'zoon_visor                  : Gauarda la configuración del zoon del visor de digitalizacion 10-100 valores retornados
        '----------------------------
        'tumbail_visor               : Gauarda la configuración si esta visible el tumbail del visor de componente de digitalización 1-0  valores retornados
        '----------------------------
        '-----------------------------
        'vista_configuracion_escaner : Gauarda la configuración si esta visible el menún de configuración del escaner 1-0   valores retornados
        '-----------------------------
        'duplex_configuracion        : Gauarda la configuración si esta activo o no activo el duplex para la digitalización 1-0 valores retornados
        '-----------------------------
        'desc_pag_blanco_configuracion: Gauarda la configuración si esta activo o no activo el descarte de pagina en blanco 1-0 valores retornados
        '-----------------------------
        'detect_borde_configuracion  : Gauarda la configuración si esta activo o no activo la correción de bordes  1-0 valores retornados
        '-----------------------------
        'controlador_propio_configuracion : Gauarda la configuración si esta activo o no activo la controlador propio de escaner
        '-----------------------------
        '-----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        'Fecha     : 2022-08-16
        '-----------
        '--------------------------------------------------------------------------------------------------------------------------------------------------
        Try

            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim sql_update As String = "update ra_config_digitalizacion_user_gestion " &
                " set zoon_visor=" & parameter_gestion.zoon_visor &
                " ,tumbail_visor=" & parameter_gestion.tumbail_visor &
                " ,vista_configuracion_escaner=" & parameter_gestion.vista_configuracion_escaner &
                " ,duplex_configuracion=" & parameter_gestion.duplex_configuracion &
                " ,desc_pag_blanco_configuracion=" & parameter_gestion.desc_pag_blanco_configuracion &
                " ,detect_borde_configuracion=" & parameter_gestion.detect_borde_configuracion &
                " ,controlador_propio_configuracion=" & parameter_gestion.controlador_propio_configuracion &
                " ,desk_configuracion=" & parameter_gestion.desk_configuracion &
                " ,adf_configuracion=" & parameter_gestion.adf_configuracion &
                " ,adf_configuracion=" & parameter_gestion.adf_configuracion &
                " where remit_dest_interno_id_Remit_Dest_Int =" & id_usuario_gestion
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Update_configuracion_interface_user_escaner = Result
                Exit Function
            Else
                Update_configuracion_interface_user_escaner = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Update_configuracion_interface_user_escaner = "Inconsistencia general funcion Update_configuracion_interface_user_escaner " & ex.Message
        End Try
    End Function
End Class
