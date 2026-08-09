Public Class class_detalle_plantilla_radicado
    Public System_Plantilla_Radicado_id_Plantilla As Integer
    Public Campo_Plantilla As String
    Public Tipo_Campo As String
    Public Comportamiento_Campo As String
    Public Alias_Campo As String
    Public Orden_Campo As Integer
    Public Estado_Campo As Integer
    Public Descripcion_Campo As String
    Public Campo_Obligatorio As String
    Public campo_sistema As Integer
    Public ID_SCRIPT As Integer
    Public TIPO_SCRIPT As String
    Public COMBINACION_TECLA As String
    Public VALOR_SCRIPT As String
    Public ESTADO_ESCRIPT As String
    Public PLATAFORMA_SCRIPT As String
    Public ID_CAMPO_ASPNET As String
    Public TEXTO_CAMPO As String
    Public TEXTO_CAMPO_MODIFICADO As String
    Public estado_dinamico_estatico As Integer
    Public Campo_rad_interno As Integer
    Public Campo_rad_externo As Integer
    Public long_campo As Integer
End Class
Public Structure Campos_Plantilla
    Dim System_Plantilla_Radicado_id_Plantilla As Integer
    Dim Campo_Plantilla As String
    Dim Tipo_Campo As String
    Dim tam_campo As Integer
    Dim Comportamiento_Campo As String
    Dim Alias_Campo As String
    Dim Orden_Campo As Integer
    Dim Estado_Campo As Integer
    Dim Descripcion_Campo As String
    Dim Campo_Obligatorio As String
    Dim campo_sistema As Integer
    Dim ID_SCRIPT As Integer
    Dim TIPO_SCRIPT As String
    Dim COMBINACION_TECLA As String
    Dim VALOR_SCRIPT As String
    Dim ESTADO_ESCRIPT As String
    Dim PLATAFORMA_SCRIPT As String
    Dim ID_CAMPO_ASPNET As String
    Dim TEXTO_CAMPO As String
    Dim TEXTO_CAMPO_MODIFICADO As String
    Dim VALUE_CAMPO As Object
    Dim estado_dinamico_estatico As Integer
    Dim Campo_rad_interno As Integer
    Dim Campo_rad_externo As Integer
    Dim Campo_rad_simple As Integer
End Structure
Public Class Class_ra_detalle_plantilla_radicado
    Function Asigna_valores_campos_plantilla_radicado_auto_relacion_plantilla_expediente(ByVal radicado As String,
                                                                                         ByVal plantilla As String,
                                                                                         ByRef ra_auto_rel_campos_plantilla_rad_expediente() As ra_auto_rel_campos_plantilla_rad_expediente) As String
        '-----------------------------------------------------------------------------
        'Funcion : Asigna valores campos plantilla radicado a expediente para el auto
        '          registro de expedientes
        '-----------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------
        'radicado       : Representa la identificación del radicado
        'plantilla      : Representa el nombre de la plantilla de radicación
        '-----------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'ra_auto_rel_campos_plantilla_rad_expediente  : Retorna la estructura de relacion
        'con los valores de los campos 
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-11-15
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim SQLconsulta As String = ""
            Dim Sql_campos As String = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To ra_auto_rel_campos_plantilla_rad_expediente.Length - 1
                If i = 0 Then
                    Sql_campos = "Select " & ra_auto_rel_campos_plantilla_rad_expediente(i).Campo_Plantilla
                Else
                    Sql_campos = Sql_campos & "," & ra_auto_rel_campos_plantilla_rad_expediente(i).Campo_Plantilla
                End If
            Next
            SQLconsulta = Sql_campos & " from " & plantilla & " where Consecutivo_Rad='" & radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(plantilla)
            Result = ref.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            If Result <> "YES" Then
                Asigna_valores_campos_plantilla_radicado_auto_relacion_plantilla_expediente = "Error funcion Asigna_valores_campos_plantilla_radicado_auto_relacion_plantilla_expediente " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Asigna_valores_campos_plantilla_radicado_auto_relacion_plantilla_expediente = "Imposible encontrar los datos de registro del radicado (" & radicado & ") en la " &
                    " plantilla (" & plantilla & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    For z As Integer = 0 To ra_auto_rel_campos_plantilla_rad_expediente.Length - 1
                        If UCase(Datset.Tables(0).Columns(i).ColumnName) = UCase(ra_auto_rel_campos_plantilla_rad_expediente(z).Campo_Plantilla) Then
                            If Datset.Tables(0).Rows(0).IsNull(Datset.Tables(0).Columns(i).ColumnName) Then
                                ra_auto_rel_campos_plantilla_rad_expediente(z).value_campo_expediente = ""
                                ra_auto_rel_campos_plantilla_rad_expediente(z).value_campo_plaantilla = ""
                            Else
                                ra_auto_rel_campos_plantilla_rad_expediente(z).value_campo_expediente = Left(Datset.Tables(0).Rows(0).Item(Datset.Tables(0).Columns(i).ColumnName),
                                                                                                        ra_auto_rel_campos_plantilla_rad_expediente(z).longitud_campo)
                                ra_auto_rel_campos_plantilla_rad_expediente(z).value_campo_plaantilla = Datset.Tables(0).Rows(0).Item(Datset.Tables(0).Columns(i).ColumnName)
                                If ra_auto_rel_campos_plantilla_rad_expediente(z).Tipo_Campo_plantilla = "DATE" Then
                                    ClassGestionFechas.Formatea_fecha_time_base_mysql(ra_auto_rel_campos_plantilla_rad_expediente(z).value_campo_expediente,
                                                                                      ra_auto_rel_campos_plantilla_rad_expediente(z).value_campo_expediente)
                                End If

                            End If
                        End If
                    Next
                Next
                Asigna_valores_campos_plantilla_radicado_auto_relacion_plantilla_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_valores_campos_plantilla_radicado_auto_relacion_plantilla_expediente = "Inconsistencia general función Asigna_valores_campos_plantilla_radicado_auto_relacion_plantilla_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_campo_plantilla(ByVal id_plantilla As Integer,
                                                 ByVal nombre_campo_plantilla As String,
                                                 ByRef existencia_campo As String) As String
        Try
            Dim sqlconsulta As String = "Select System_Plantilla_Radicado_id_Plantilla from detalle_plantilla_radicado where System_Plantilla_Radicado_id_Plantilla=" & id_plantilla
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_set As New DataSet
            Dim Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(sqlconsulta, Dat_set)
            If Result <> "YES" Then
                Solicita_existencia_campo_plantilla = " función Solicita_existencia_campo_plantilla Error:   " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count > 0 Then
                existencia_campo = "YES"
                Solicita_existencia_campo_plantilla = "YES"
                Exit Function
            Else
                existencia_campo = "NO"
                Solicita_existencia_campo_plantilla = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_campo_plantilla = "Inconsistencia general función Solicita_existencia_campo_plantilla " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_campos_radicacion_simplificada(ByVal Id_Plantilla As Integer,
                                                                ByVal Nombre_plantilla_radicado As String,
                                                                ByVal name_espace_form_control As String,
                                                                ByVal name_tab_control As String,
                                                                ByVal name_classe As String,
                                                                ByVal Estado_opcion_fecha As Integer,
                                                                ByVal Estado_opcion_cita_respuesta As Integer,
                                                                ByVal Estado_opcion_radicado_general As Integer,
                                                                ByRef Class_config_general_service As List(Of Class_config_general_service)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos de radicacion simple
        '---------------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------------
        'Id_Plantilla                 : Representa la identificación de la plantilla del
        '                              radicado
        'Estado_opcion_fecha          : Representa si el sistema incluye la opcion fecha 
        'Estado_opcion_cita_respuesta : Reprenta si el sistema incluye citación de respuesta
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'Class_config_general_service : Retorna la estructura con los campos
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-10-15
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------------
        Try
            Dim tipo_plantilla As String = ""
            Dim campo_dinamico_cargo As String = ""
            Dim Result As String = ""
            Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
            Dim Campos_Plantilla() As Campos_Plantilla = Nothing
            Result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(Id_Plantilla,
                                                                                                Campos_Plantilla,
                                                                                                Estado_opcion_fecha,
                                                                                                Estado_opcion_cita_respuesta,
                                                                                                Estado_opcion_radicado_general)
            If Result <> "YES" Then
                Solicita_estructura_campos_radicacion_simplificada = Result
                Exit Function
            End If
            '---------------------------------------
            '------Asigna datos campos validacion
            '---------------------------------------
            Dim ref_Class_ra_script_actividades As New Class_ra_script_actividades
            Dim validacion_plantilla() As validacion_plantilla = Nothing
            Result = ref_Class_ra_script_actividades.lista_campos_Validacion_plantilla(Id_Plantilla,
                                                                                       validacion_plantilla)
            If Result <> "YES" Then
                Solicita_estructura_campos_radicacion_simplificada = Result
                Exit Function
            End If
            If Not validacion_plantilla Is Nothing Then
                For i2 As Integer = 0 To validacion_plantilla.Length - 1
                    For i3 As Integer = 0 To Campos_Plantilla.Length - 1
                        If Campos_Plantilla(i3).Campo_Plantilla = validacion_plantilla(i2).Campo_Plantilla Then
                            Campos_Plantilla(i3).TIPO_SCRIPT = validacion_plantilla(i2).TIPO_SCRIPT
                            Campos_Plantilla(i3).COMBINACION_TECLA = validacion_plantilla(i2).COMBINACION_TECLA
                            Campos_Plantilla(i3).VALOR_SCRIPT = validacion_plantilla(i2).VALOR_SCRIPT
                            Campos_Plantilla(i3).ESTADO_ESCRIPT = validacion_plantilla(i2).ESTADO_ESCRIPT
                            Campos_Plantilla(i3).PLATAFORMA_SCRIPT = validacion_plantilla(i2).PLATAFORMA_SCRIPT
                            Campos_Plantilla(i3).ID_SCRIPT = validacion_plantilla(i2).ID_SCRIPT
                        End If
                    Next
                Next
            End If
            '-------///-----Agrega campo tipo tramite
            Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
            parameter_gestion.name_campo = "Descripcion_Documento"
            parameter_gestion.aleas_campo = "TRAMITE DOCUMENTO"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 0
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 120
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.name_tab_control = "div_datos_registro_tramite"
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = "tipo_doc_entrante"
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.error_gestion = "YES"
            parameter_gestion.drow_name_controls_destino = "RE_flujo_trabajo" '//Agrega el campo destino que llenara el control con el evento de slección
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.obligatorio_campo = 1
            parameter_gestion.valida_capital_text = 0
            parameter_gestion.control_input_class = "form-control-drow"
            parameter_gestion.label_input_class_font = ""
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.tooltipAyuda = "Seleccione el tipo de tramite a radicar para su documento"
            Dim Class_service_ilist_drowlist = New List(Of Class_config_general_service.Class_service_ilist_drowlist)()
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Class_tipo_doc_entrante.Solicita_lista_tipo_tramite_simple(Id_Plantilla,
                                                                                Class_service_ilist_drowlist)
            If Result <> "YES" Then
                Solicita_estructura_campos_radicacion_simplificada = Result
                Exit Function
            End If
            Class_config_general_service.Add(parameter_gestion)
            Class_config_general_service.Item(0).ilist_row_drowlist = Class_service_ilist_drowlist
            '-------///-----Agrega campo fjujo--------////
            '-------Agrega la configuración del servicio de consulta para el  campo destino  
            Dim config_service_drowlis As New List(Of Class_config_general_service.Class_config_general_service_drowlist)
            Dim item_config_service_drowlis As Class_config_general_service.Class_config_general_service_drowlist = New Class_config_general_service.Class_config_general_service_drowlist()
            item_config_service_drowlis.limit_rows = 30
            item_config_service_drowlis.name_dbs_auto = "DA"
            item_config_service_drowlis.name_table_auto = "ra_relacion_tramite_flujo_wokflow"
            item_config_service_drowlis.name_campo_primary = "ID_WF_FLUJO_TRABAJO"
            item_config_service_drowlis.tipo_orden = "ASC"
            item_config_service_drowlis.name_campo_value = "NOMBRE_FLUJO_TRABAJO"
            item_config_service_drowlis.name_campo_orden = "NOMBRE_FLUJO_TRABAJO"
            item_config_service_drowlis.name_campo_condicion = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            config_service_drowlis.Add(item_config_service_drowlis)
            parameter_gestion = New Class_config_general_service()
            parameter_gestion.config_service_drowlis_destino = New List(Of Class_config_general_service.Class_config_general_service_drowlist)
            parameter_gestion.config_service_drowlis_destino = config_service_drowlis
            parameter_gestion.name_campo = "RE_flujo_trabajo"
            parameter_gestion.aleas_campo = "Flujo tramite"
            parameter_gestion.alow_null = 1
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 0
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 120
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.name_tab_control = "div_datos_registro_tramite"
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = Nombre_plantilla_radicado
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.error_gestion = "YES"
            parameter_gestion.drow_name_controls_destino = ""
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.obligatorio_campo = 1
            parameter_gestion.valida_capital_text = 0
            parameter_gestion.control_input_class = "form-control-drow"
            parameter_gestion.label_input_class_font = ""
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.tooltipAyuda = "Seleccione el tipo de flujo de trabajo del tramite"
            '///-------Agrega el asunto del tramite----/////
            Class_config_general_service.Add(parameter_gestion)
            parameter_gestion = New Class_config_general_service()
            parameter_gestion.name_campo = "ASUNTO"
            parameter_gestion.aleas_campo = "Asunto"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.disable_campo = 1
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 30
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.name_tab_control = "div_datos_registro_tramite"
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = Nombre_plantilla_radicado
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.obligatorio_campo = 1
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.Place_Holder = ""
            parameter_gestion.tooltipAyuda = "Digite el asunto en una descripción breve no mas de 30 caracteres"
            parameter_gestion.error_gestion = "YES"
            Class_config_general_service.Add(parameter_gestion)
            '///-------Agrega el remitente o tercero del tramite----/////
            parameter_gestion = New Class_config_general_service()
            parameter_gestion.name_campo = "REMITENTE_COR"
            parameter_gestion.aleas_campo = "Solicitante"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 15
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.disable_campo = 1
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 120
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.name_tab_control = "div_datos_registro_tramite"
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = Nombre_plantilla_radicado
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.obligatorio_campo = 1
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.Place_Holder = "Digita y valida nombre del solicitante"
            parameter_gestion.url_class_service_control_plantilla = "../webservice/WebService_radicacion_Simplificada.asmx"
            parameter_gestion.service_control_plantilla = "Service_Solicita_datos_auto_complete_externo"
            parameter_gestion.Tom_option = "solicitante"
            parameter_gestion.Tom_item = "solicitante"
            parameter_gestion.Tom_alow = "1"
            parameter_gestion.tooltipAyuda = "Gestione o agregue el solicitante del tramite"
            parameter_gestion.error_gestion = "YES"
            Dim Tipo_script As String = ""
            Dim id_escript As Integer = -1
            Dim nombre_plantilla_validacion As String = ""
            Dim campo_nombre As String = ""
            Dim campo_identificacion As String = ""
            Dim campo_anualidad As String = ""
            Dim campo_primary_key As String = ""
            Dim ClassRadicador As New ClassRadicador
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Tipo_Validacion_Campo(Campos_Plantilla,
                                                                              "REMITENTE_COR",
                                                                              Tipo_script,
                                                                              id_escript)
            If Result <> "YES" Then
                Solicita_estructura_campos_radicacion_simplificada = Result
                Exit Function
            End If

            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_escript,
                                                                                    nombre_plantilla_validacion)
            If Result <> "YES" Then
                Solicita_estructura_campos_radicacion_simplificada = Result
                Exit Function
            End If
            Result = Class_plantilla_validacion.Solicita_campo_property_plantilla_validacion(id_escript,
                                                                                             campo_nombre,
                                                                                             campo_identificacion,
                                                                                             campo_anualidad,
                                                                                             campo_primary_key)
            If Result <> "YES" Then
                Solicita_estructura_campos_radicacion_simplificada = Result
                Exit Function
            End If
            parameter_gestion.Tipo_script = Tipo_script
            parameter_gestion.id_escript = id_escript
            parameter_gestion.name_plantilla_validacion = nombre_plantilla_validacion
            parameter_gestion.campo_nombre_plantilla_val = campo_nombre
            parameter_gestion.campo_primary_plantilla_val = campo_primary_key
            Class_config_general_service.Add(parameter_gestion)

            '//----------------Agrega el campo destinatario-----------------------
            '///-------Agrega el remitente o tercero del tramite----/////
            parameter_gestion = New Class_config_general_service()
            parameter_gestion.name_campo = "Destinatario_Cor"
            parameter_gestion.aleas_campo = "Responsable del tramite"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 15
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.disable_campo = 1
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 120
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.name_tab_control = "div_datos_registro_tramite"
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = Nombre_plantilla_radicado
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.obligatorio_campo = 1
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.Place_Holder = "Digita y valida nombre del responsable"
            parameter_gestion.tooltipAyuda = "Seleccione el responsable del tramite"
            parameter_gestion.url_class_service_control_plantilla = "../webservice/WebService_radicacion_Simplificada.asmx"
            parameter_gestion.service_control_plantilla = "Service_Solicita_datos_auto_complete_interno"
            parameter_gestion.Tom_option = "destinatario"
            parameter_gestion.Tom_item = "destinatario"
            parameter_gestion.Tom_alow = "1"
            parameter_gestion.Tipo_script = 1
            parameter_gestion.id_escript = 0
            parameter_gestion.name_plantilla_validacion = "remit_dest_interno"
            parameter_gestion.campo_nombre_plantilla_val = "Nombre_Remitente"
            parameter_gestion.campo_primary_plantilla_val = "id_Remit_Dest_Int"
            Class_config_general_service.Add(parameter_gestion)
            parameter_gestion.error_gestion = "YES"
            Solicita_estructura_campos_radicacion_simplificada = "YES"
        Catch ex As Exception
            Solicita_estructura_campos_radicacion_simplificada = "Inconistencia general funcion Solicita_estructura_campos_radicacion_simplificada " & ex.Message
        End Try
    End Function
    'LISTA CAMPOS PLANTILLA RADICACION ENTRANTE CAMPOS FIJOS Y CAMPOS DINAMICOS
    Function Lista_Campos_Adicionales_Plantilla(ByVal Id_Plantilla As Integer,
                                                ByRef Matri_Datos() As Campos_Plantilla,
                                                ByRef Estado_opcion_fecha As Integer,
                                                ByRef Estado_opcion_cita_respuesta As Integer,
                                                ByRef Estado_opcion_radicado_general As Integer) As String
        '-----------------------------------------------------
        'Funcion : Lista los campos y el detalle de los campos
        'con los parametors id de la plantillas y retorna una
        'matriz con la estructura 
        'Fecha : 2014-04-07
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------
        Dim tipo_plantilla As String = ""
        Dim campo_dinamico_cargo As String = ""
        Dim Result As String = ""
        Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
        Result = ref_Class_system_plantilla_radicado.Retorna_Tipo_Plantilla(Id_Plantilla,
                                                                            tipo_plantilla)
        If Result <> "YES" Then
            Lista_Campos_Adicionales_Plantilla = "Fución Lista_Campos_Adicionales_Plantilla dice " & Result
            Exit Function
        End If
        If tipo_plantilla = "RADICACION ENTRANTE" Then
            campo_dinamico_cargo = "CARGO_DESTINATARIO@CARGO_DESTINATARIO@VARCHAR@0@0@DIGITACION@CARGO_DESTINATARIO@100"
        Else
            campo_dinamico_cargo = "CARGO_REMITENTE@CARGO_REMITENTE@VARCHAR@0@0@DIGITACION@CARGO_REMITENTE@100"
        End If
        Dim Campos_estaticos() As String = {"Descripcion_Documento@RE_Descripcion_Documento@VARCHAR@1@1@SELECCION@TRAMITE DOCUMENTO@100", "Anexos_Cor@RE_Anexos_Cor-Anexos_Cor-VARCHAR@VARCHAR@1@1@DIGITACION@ANEXOS@244",
        "Fecha_Documento@RE_Fecha_Documento-RA_ENTRANTE-DATE@DATE@1@1@DIGITACION@FECHA DOCUMENTO@20", "Numero_Folios@RE_Numero_Folios-Numero_Folios-INT@INT@1@1@DIGITACION@NUMERO FOLIOS@9", "REMITENTE_COR@Remitente_Cor-Remitente_Cor-VARCHAR@VARCHAR@1@1@DIGITACION@REMITENTE@240", "Destinatario_Cor@Destinatario_Cor@VARCHAR@1@1@SELECCION@DESTINATARIO@100", "ASUNTO@ASUNTO@VARCHAR@0@1@DIGITACION@ASUNTO@240", campo_dinamico_cargo}
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select System_Plantilla_Radicado_id_Plantilla,Campo_Plantilla,Tipo_Campo,Comportamiento_Campo," &
            "Alias_Campo,Orden_Campo,Estado_Campo,Descripcion_Campo,Campo_Obligatorio,Campo_rad_interno,Campo_rad_externo,Campo_rad_simple,tam_campo" &
            " from detalle_plantilla_radicado where System_Plantilla_Radicado_id_Plantilla =" &
            Id_Plantilla & " order by Orden_Campo"
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_Campos_Adicionales_Plantilla = " Error listando campos adicionales   " & Parametro_Consulta
                Return Lista_Campos_Adicionales_Plantilla
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_Campos_Adicionales_Plantilla = "YES"
                Exit Function
            Else
                '0--Nombre campo
                '1--Campo id asp.net
                '2--tipo campo
                '3--campo obligatorio
                '4-- campo visible
                Dim Iconta As Integer = 0
                For z3 As Integer = 0 To Campos_estaticos.Length - 1
                    Dim Splitplantilla() As String
                    Erase Splitplantilla
                    Splitplantilla = Campos_estaticos(z3).Split("@")
                    ReDim Preserve Matri_Datos(Iconta)
                    Matri_Datos(Iconta).System_Plantilla_Radicado_id_Plantilla = Id_Plantilla
                    Matri_Datos(Iconta).Campo_Plantilla = Splitplantilla(0)
                    Matri_Datos(Iconta).Tipo_Campo = Splitplantilla(2)
                    Matri_Datos(Iconta).Comportamiento_Campo = Splitplantilla(5)
                    Matri_Datos(Iconta).Alias_Campo = Splitplantilla(6)
                    Matri_Datos(Iconta).Orden_Campo = Iconta
                    Matri_Datos(Iconta).Estado_Campo = Splitplantilla(4)
                    Matri_Datos(Iconta).Descripcion_Campo = ""
                    Matri_Datos(Iconta).Campo_Obligatorio = Splitplantilla(3)
                    Matri_Datos(Iconta).ID_CAMPO_ASPNET = Splitplantilla(1)
                    Matri_Datos(Iconta).campo_sistema = 1
                    Matri_Datos(Iconta).estado_dinamico_estatico = 1
                    Matri_Datos(Iconta).Campo_rad_externo = 1
                    Matri_Datos(Iconta).Campo_rad_interno = 1
                    Matri_Datos(Iconta).Campo_rad_simple = 1
                    Matri_Datos(Iconta).tam_campo = Splitplantilla(7)
                    Iconta = Iconta + 1
                Next
                For zi As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(Iconta)
                    If Dat_reader.Tables(0).Rows(zi).IsNull("System_Plantilla_Radicado_id_Plantilla") = False Then
                        Matri_Datos(Iconta).System_Plantilla_Radicado_id_Plantilla = Dat_reader.Tables(0).Rows(zi).Item("System_Plantilla_Radicado_id_Plantilla").ToString
                    Else
                        Matri_Datos(Iconta).System_Plantilla_Radicado_id_Plantilla = 0
                    End If
                    If Dat_reader.Tables(0).Rows(zi).IsNull("Campo_Plantilla") = False Then
                        Matri_Datos(Iconta).Campo_Plantilla = Dat_reader.Tables(0).Rows(zi).Item("Campo_Plantilla").ToString
                    Else
                        Matri_Datos(Iconta).Campo_Plantilla = ""
                    End If
                    If Dat_reader.Tables(0).Rows(zi).IsNull("Tipo_Campo") = False Then
                        Matri_Datos(Iconta).Tipo_Campo = Dat_reader.Tables(0).Rows(zi).Item("Tipo_Campo").ToString
                    Else
                        Matri_Datos(Iconta).Tipo_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(zi).IsNull("Comportamiento_Campo") = False Then
                        Matri_Datos(Iconta).Comportamiento_Campo = Dat_reader.Tables(0).Rows(zi).Item("Comportamiento_Campo").ToString
                    Else
                        Matri_Datos(Iconta).Comportamiento_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(zi).IsNull("Alias_Campo") = False Then
                        Matri_Datos(Iconta).Alias_Campo = Dat_reader.Tables(0).Rows(zi).Item("Alias_Campo").ToString
                    Else
                        Matri_Datos(Iconta).Alias_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(zi).IsNull("Orden_Campo") = False Then
                        Matri_Datos(Iconta).Orden_Campo = Dat_reader.Tables(0).Rows(zi).Item("Orden_Campo")
                    Else
                        Matri_Datos(Iconta).Orden_Campo = 10000
                    End If
                    If Dat_reader.Tables(0).Rows(zi).IsNull("Estado_Campo") = False Then
                        Matri_Datos(Iconta).Estado_Campo = Dat_reader.Tables(0).Rows(zi).Item("Estado_Campo")
                    Else
                        Matri_Datos(Iconta).Estado_Campo = 0
                    End If
                    Select Case Matri_Datos(Iconta).Campo_Plantilla
                        Case "CITARADICADO"
                            Matri_Datos(Iconta).Estado_Campo = Estado_opcion_cita_respuesta
                            Matri_Datos(Iconta).campo_sistema = 1
                        Case "FECHALIMITERESPUESTA"
                            Matri_Datos(Iconta).Estado_Campo = Estado_opcion_fecha
                            Matri_Datos(Iconta).campo_sistema = 1
                    End Select

                    If Dat_reader.Tables(0).Rows(zi).IsNull("Descripcion_Campo") = False Then
                        Matri_Datos(Iconta).Descripcion_Campo = Dat_reader.Tables(0).Rows(zi).Item("Descripcion_Campo").ToString
                    Else
                        Matri_Datos(Iconta).Descripcion_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(zi).IsNull("Campo_Obligatorio") = False Then
                        Matri_Datos(Iconta).Campo_Obligatorio = Dat_reader.Tables(0).Rows(zi).Item("Campo_Obligatorio").ToString
                    Else
                        Matri_Datos(Iconta).Campo_Obligatorio = ""
                    End If
                    Matri_Datos(Iconta).Campo_rad_interno = Dat_reader.Tables(0).Rows(zi).Item("Campo_rad_interno")
                    Matri_Datos(Iconta).Campo_rad_externo = Dat_reader.Tables(0).Rows(zi).Item("Campo_rad_externo")
                    Matri_Datos(Iconta).Campo_rad_simple = Dat_reader.Tables(0).Rows(zi).Item("Campo_rad_simple")
                    Matri_Datos(Iconta).tam_campo = Dat_reader.Tables(0).Rows(zi).Item("tam_campo")
                    Matri_Datos(Iconta).estado_dinamico_estatico = 2
                    Iconta = Iconta + 1
                Next
                Lista_Campos_Adicionales_Plantilla = "YES"
                Exit Function
            End If
            Lista_Campos_Adicionales_Plantilla = "YES"
        Catch ex As Exception
            Lista_Campos_Adicionales_Plantilla = "Funcion Lista_Campos_Adicionales_Plantilla " & ex.Message
        End Try
    End Function
    Function Lista_Campos_Adicionales_Plantilla(ByVal Id_Plantilla As Integer,
                                                ByRef Matri_Datos() As Campos_Plantilla) As String
        '-----------------------------------------------------
        'Funcion : Lista los campos y el detalle de los campos
        'con los parametors id de la plantillas y retorna una
        'matriz con la estructura 
        'Fecha : 2014-04-07
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select * from detalle_plantilla_radicado where System_Plantilla_Radicado_id_Plantilla =" &
            Id_Plantilla & " order by Orden_Campo"
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_Campos_Adicionales_Plantilla = " Error listando campos adicionales   " & Parametro_Consulta
                Return Lista_Campos_Adicionales_Plantilla
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_Campos_Adicionales_Plantilla = "YES"
                Exit Function
            Else
                Dim Iconta As Integer = 0

                For zi As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1

                    'Do While Dat_reader.Read()
                    ReDim Preserve Matri_Datos(Iconta)
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(0) = False Then
                        Matri_Datos(Iconta).System_Plantilla_Radicado_id_Plantilla = Dat_reader.Tables(0).Rows(Iconta).Item(0).ToString
                    Else
                        Matri_Datos(Iconta).System_Plantilla_Radicado_id_Plantilla = 0
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(1) = False Then
                        Matri_Datos(Iconta).Campo_Plantilla = Dat_reader.Tables(0).Rows(Iconta).Item(1).ToString
                    Else
                        Matri_Datos(Iconta).Campo_Plantilla = ""
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(2) = False Then
                        Matri_Datos(Iconta).Tipo_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(2).ToString
                    Else
                        Matri_Datos(Iconta).Tipo_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(3) = False Then
                        Matri_Datos(Iconta).Comportamiento_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(3).ToString
                    Else
                        Matri_Datos(Iconta).Comportamiento_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(4) = False Then
                        Matri_Datos(Iconta).Alias_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(4).ToString
                    Else
                        Matri_Datos(Iconta).Alias_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(5) = False Then
                        Matri_Datos(Iconta).Orden_Campo = Iconta
                    Else
                        Matri_Datos(Iconta).Orden_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(6) = False Then
                        Matri_Datos(Iconta).Estado_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(6).ToString
                    Else
                        Matri_Datos(Iconta).Estado_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(7) = False Then
                        Matri_Datos(Iconta).Descripcion_Campo = Dat_reader.Tables(0).Rows(Iconta).Item(7).ToString
                    Else
                        Matri_Datos(Iconta).Descripcion_Campo = ""
                    End If
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(8) = False Then
                        Matri_Datos(Iconta).Campo_Obligatorio = Dat_reader.Tables(0).Rows(Iconta).Item(8).ToString
                    Else
                        Matri_Datos(Iconta).Campo_Obligatorio = ""
                    End If
                    Iconta = Iconta + 1

                Next
                ReDim Preserve Matri_Datos(Iconta)
                Matri_Datos(Iconta).Campo_Plantilla = "REMITENTE_COR"
                Matri_Datos(Iconta).Campo_Obligatorio = "1"
                Matri_Datos(Iconta).Alias_Campo = "Remitente Correspondencia"
                Matri_Datos(Iconta).campo_sistema = "1"
                Matri_Datos(Iconta).COMBINACION_TECLA = ""
                Matri_Datos(Iconta).Comportamiento_Campo = "DIGITACION"
                Matri_Datos(Iconta).Orden_Campo = "-1"
                Lista_Campos_Adicionales_Plantilla = "YES"
                Exit Function
            End If
            Lista_Campos_Adicionales_Plantilla = "YES"
        Catch ex As Exception
            Lista_Campos_Adicionales_Plantilla = "Funcion Lista_Campos_Adicionales_Plantilla " & ex.Message
        End Try
    End Function
    Function Retorna_Tipo_Campo_Plantilla_Radicacion(ByVal id_plantilla_radicado As Integer, ByVal nombre_campo As String, ByRef tipo_campo As String) As String
        '******************************************************************************
        'Funcion : retorna tipo campo plantilla radicacion
        'Fecha : 2014-08-12
        'Ingeniero :Miguel Angel Urueta Miranda
        '******************************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select Tipo_Campo from detalle_plantilla_radicado where " &
            " System_Plantilla_Radicado_id_Plantilla=" & id_plantilla_radicado & " and Campo_Plantilla='" & nombre_campo & "'"
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Retorna_Tipo_Campo_Plantilla_Radicacion = " Error Listando tipo campo plantilla radicacion  " & result
                Exit Function
            End If
            Dim contador As Integer = 0
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                tipo_campo = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_Tipo_Campo_Plantilla_Radicacion = "YES"
                Exit Function
            Else
                Retorna_Tipo_Campo_Plantilla_Radicacion = "Imposible encontrar tipo campo plantilla radicacion campo " & nombre_campo
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Tipo_Campo_Plantilla_Radicacion = "Inconsistencia general funcion Retorna_Tipo_Campo_Plantilla_Radicacion " & ex.Message
        End Try
    End Function
    Function Genera_interface_detalle_radicado(ByVal radicado As String,
                                               ByRef pag As Page) As String
        Try
            Dim reflcas_consulta_radicacion As New ClassRaConsultaRadicados
            Dim struc_envio As PLANTILLA_VALIDACION_CAMPOS_ESTATICOS = Nothing
            Dim tipo_radicado As String = ""
            Dim nombre_plantilla_radicado As String = ""
            Dim Result As String = ""
            Dim ref_class_ra_registro_general_rad As New Class_ra_registro_general_radicacion
            Result = ref_class_ra_registro_general_rad.SolicitaNombrePlantillaRadicado(radicado,
                                                                                       nombre_plantilla_radicado)
            If Result <> "YES" Then
                Genera_interface_detalle_radicado = Result
                Exit Function
            End If
            Dim id_tipo_tramite As Integer = 0
            Dim nombre_tramite As String = ""
            Dim ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = ref_Class_plantillas_radicacion.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla_radicado,
                                                                                                        radicado,
                                                                                                        id_tipo_tramite,
                                                                                                        nombre_tramite)
            If Result <> "YES" Then
                Genera_interface_detalle_radicado = Result
                Exit Function
            End If
            Dim tipo_tramite As Integer = 0
            Dim ref_class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = ref_class_tipo_doc_entrante.Solicita_tipo_tramite_por_id_tramite(id_tipo_tramite,
                                                                                      tipo_tramite)
            If Result <> "YES" Then
                Genera_interface_detalle_radicado = Result
                Exit Function
            End If
            If tipo_tramite = 1 Then
                tipo_radicado = "RADICACION ENTRANTE"
            Else
                tipo_radicado = "RADICACION SALIENTE"
            End If
            Result = ref_Class_plantillas_radicacion.retorna_datos_radicacion_estructura(tipo_radicado,
                                                                                         radicado,
                                                                                         nombre_plantilla_radicado,
                                                                                         struc_envio)
            If Result <> "YES" Then
                Genera_interface_detalle_radicado = Result
                Exit Function
            End If
            Dim Ref_class_usuario_radicador As New Class_usuario_radicador
            Dim Nombre_usuario_gestion_radicador As String = ""
            Dim Cargo_usuario_gestion_radicador As String = ""
            Dim sede_empresa As String = ""
            Result = Ref_class_usuario_radicador.Solicita_caraterizacion_usuario_radicador_gestion(struc_envio.Usuario_Radicador_id_usuario,
                                                                                                   Nombre_usuario_gestion_radicador,
                                                                                                   Cargo_usuario_gestion_radicador,
                                                                                                   sede_empresa)
            If Result <> "YES" Then
                Genera_interface_detalle_radicado = Result
                Exit Function
            End If
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Dim nombre_usuario_gestion As String = ""
            Dim cargo_usuario As String = ""
            Dim sede As String = ""
            Result = Class_remit_dest_interno.Solicita_detalle_usuario_gestion(struc_envio.Destinatario_Externo_id_Dest_Ext,
                                                                               nombre_usuario_gestion,
                                                                               cargo_usuario,
                                                                               sede)
            If Result <> "YES" Then
                Genera_interface_detalle_radicado = Result
                Exit Function
            End If
            Dim nombre_flujo As String = ""
            Dim Ref_class_flujo_trabajo As New Class_flujo_trabajo_workflow
            If struc_envio.id_tipo_flujo_workflow <> 0 Then
                Result = Ref_class_flujo_trabajo.SolicitaNombreFlujoTrabajoPorIdFlujo(struc_envio.id_tipo_flujo_workflow,
                                                                                           nombre_flujo)
                If Result <> "YES" Then
                    Genera_interface_detalle_radicado = Result
                    Exit Function
                End If
            End If
            Dim Ref_Label_RADICADO_TRAMITE As Label = pag.FindControl("Label_RADICADO_TRAMITE")
            Dim Ref_LabelDESTINATARIO As Label = pag.FindControl("LabelDESTINATARIO")
            Dim Ref_Label_TIPO_TRAMITE As Label = pag.FindControl("Label_TIPO_TRAMITE")
            Dim Ref_LabelFECHA_REGISTRO As Label = pag.FindControl("LabelFECHA_REGISTRO")
            Dim Ref_Label_ASUNTO_RADICADO_ As Label = pag.FindControl("Label_ASUNTO_RADICADO_")
            Dim Ref_Label_FLUJO_RADICADO As Label = pag.FindControl("Label_FLUJO_RADICADO")
            Dim Ref_Label_radicador_usuario As Label = pag.FindControl("Label_radicador_usuario")
            Dim Ref_Label_CARGO_USUARIO_RADICADOR As Label = pag.FindControl("Label_CARGO_USUARIO_RADICADOR")
            Dim ref_Label_SEDE_USUARIO As Label = pag.FindControl("Label_SEDE_USUARIO")
            Dim ref_Update_detalle_radicado As UpdatePanel = pag.FindControl("Update_detalle_radicado")
            Dim Ref_Label_FECHA_VENCE As Label = pag.FindControl("Label_FECHA_VENCE")
            Dim REF_LabelASIGNADO As Label = pag.FindControl("LabelASIGNADO")
            Ref_Label_RADICADO_TRAMITE.Text = radicado
            Ref_LabelDESTINATARIO.Text = struc_envio.Remitente_Cor
            Ref_Label_TIPO_TRAMITE.Text = struc_envio.Descripcion_Documento
            Ref_LabelFECHA_REGISTRO.Text = struc_envio.Fecha_Radicado
            Ref_Label_ASUNTO_RADICADO_.Text = struc_envio.Asunto
            Ref_Label_radicador_usuario.Text = Nombre_usuario_gestion_radicador
            Ref_Label_CARGO_USUARIO_RADICADOR.Text = Cargo_usuario_gestion_radicador
            ref_Label_SEDE_USUARIO.Text = sede_empresa
            Ref_Label_FLUJO_RADICADO.Text = nombre_flujo
            Ref_Label_FECHA_VENCE.Text = struc_envio.FECHALIMITERESPUESTA
            REF_LabelASIGNADO.Text = nombre_usuario_gestion & " (" & cargo_usuario & ")"
            ref_Update_detalle_radicado.Update()
            Genera_interface_detalle_radicado = "YES"
        Catch ex As Exception
            Genera_interface_detalle_radicado = "Inconsistencia general función Genera_interface_detalle_radicado " & ex.Message
        End Try
    End Function
End Class
