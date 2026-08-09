Public Class class_stru_ra_con_usuario_consulta_publica
    Property error_gestion As String
    Property id_registro_usuario As Integer
    Property ra_con_tipos_identificacion_id_tipo_identificacion As Integer
    Property fecha_registro_usuario As String
    Property primer_nombre As String
    Property segundo_nombre As String
    Property primer_apellido As String
    Property segundo_apellido As String
    Property nombre_completo As String
    Property numero_identificacion As String
End Class
Public Structure stru_ra_con_usuario_consulta_publica
    Dim error_gestion As String
    Dim id_registro_usuario As Integer
    Dim ra_con_tipos_identificacion_id_tipo_identificacion As Integer
    Dim fecha_registro_usuario As String
    Dim primer_nombre As String
    Dim segundo_nombre As String
    Dim primer_apellido As String
    Dim segundo_apellido As String
    Dim nombre_completo As String
    Dim numero_identificacion As String
End Structure
Public Class Class_ra_con_usuario_consulta_publica
    Function Registro_usuario_consulta_publica(ByVal Class_config_general_service As List(Of Class_config_general_service),
                                               ByRef id_usuario_registro As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Realiza el registro del usuario publico de consulta expedientes
        'para camaras de comercio
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service : Representa la estructura con los datos del 
        'del registro
        '                           
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_usuario_registro  : Retorna la idneiticcaición del registro
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim sql_campos As String = ""
            Dim sql_insert_campos As String = ""
            Dim sql_table_insert As String = "insert into " & "ra_con_usuario_consulta_publica" & "  "
            Dim valor_campo As String = ""
            Dim valor_campo_nombre As String = ""
            For i As Integer = 0 To Class_config_general_service.Count - 1
                Select Case Class_config_general_service(i).name_campo
                    Case "primer_nombre"
                        If Class_config_general_service(i).value_campo <> "" Then
                            If valor_campo_nombre = "" Then
                                valor_campo_nombre = Class_config_general_service(i).value_campo
                            Else
                                valor_campo_nombre = valor_campo_nombre & " " & Class_config_general_service(i).value_campo
                            End If
                        End If
                    Case "segundo_nombre"
                        If Class_config_general_service(i).value_campo <> "" Then
                            If valor_campo_nombre = "" Then
                                valor_campo_nombre = Class_config_general_service(i).value_campo
                            Else
                                valor_campo_nombre = valor_campo_nombre & " " & Class_config_general_service(i).value_campo
                            End If
                        End If
                    Case "primer_apellido"
                        If Class_config_general_service(i).value_campo <> "" Then
                            If valor_campo_nombre = "" Then
                                valor_campo_nombre = Class_config_general_service(i).value_campo
                            Else
                                valor_campo_nombre = valor_campo_nombre & " " & Class_config_general_service(i).value_campo
                            End If
                        End If
                    Case "segundo_apellido"
                        If Class_config_general_service(i).value_campo <> "" Then
                            If valor_campo_nombre = "" Then
                                valor_campo_nombre = Class_config_general_service(i).value_campo
                            Else
                                valor_campo_nombre = valor_campo_nombre & " " & Class_config_general_service(i).value_campo
                            End If
                        End If
                End Select
            Next
            For i As Integer = 0 To Class_config_general_service.Count - 1
                If sql_campos = "" Then
                    sql_campos = "(" & Class_config_general_service(i).name_campo
                Else
                    sql_campos = sql_campos & "," & Class_config_general_service(i).name_campo
                End If
                If Class_config_general_service(i).alow_tipo_value = 0 Then
                    valor_campo = Class_config_general_service(i).texto_campo
                Else
                    valor_campo = Class_config_general_service(i).value_campo
                End If
                If Class_config_general_service(i).alow_null = 1 And valor_campo = "" Then
                    valor_campo = "Null"
                Else
                    valor_campo = "'" & valor_campo & "'"
                End If
                If sql_insert_campos = "" Then
                    sql_insert_campos = "(" & valor_campo
                Else
                    sql_insert_campos = sql_insert_campos & "," & valor_campo
                End If
            Next
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim time1al As String = Date.Now.ToString
            ref_ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
            sql_insert_campos = sql_insert_campos & ",'" & valor_campo_nombre & "','" & time1al & "'"
            sql_insert_campos = sql_insert_campos & ")"
            sql_campos = sql_campos & ",nombre_completo,fecha_registro_usuario) "
            Dim sql_insert As String = sql_table_insert & sql_campos & " values " & sql_insert_campos
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.SELECTION_LAST_INSERT_COMMAND(sql_insert, id_usuario_registro)
            Registro_usuario_consulta_publica = Result
            Exit Function
        Catch ex As Exception
            Registro_usuario_consulta_publica = "Inconsistencia general funcion Registro_usuario_consulta_publica " & ex.Message
        End Try
    End Function
    Function Solicita_datos_campos_registro_consulta_publica(ByVal name_espace_form_control As String,
                                                             ByRef Class_config_general_service As List(Of Class_config_general_service)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita datos de contrución del formulario para el registro
        '          del usuario de consulta
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'name_espace_form_control : Representa el espacio de nombre donde se agrupan
        '                           los controles
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Retorna la estructura del formulario
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Class_config_general_services As New Class_config_general_service
            Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
            Dim config_service_drowlis As New List(Of Class_config_general_service.Class_config_general_service_drowlist)
            Dim item_config_service_drowlis As Class_config_general_service.Class_config_general_service_drowlist = New Class_config_general_service.Class_config_general_service_drowlist()
            item_config_service_drowlis.limit_rows = 10
            item_config_service_drowlis.name_dbs_auto = "DA"
            item_config_service_drowlis.name_table_auto = "ra_con_tipos_identificacion"
            item_config_service_drowlis.name_campo_primary = "id_tipo_identificacion"
            item_config_service_drowlis.tipo_orden = "ASC"
            item_config_service_drowlis.name_campo_value = "tipo_identificacion"
            item_config_service_drowlis.name_campo_orden = "tipo_identificacion"
            config_service_drowlis.Add(item_config_service_drowlis)
            parameter_gestion.config_service_drowlis = New List(Of Class_config_general_service.Class_config_general_service_drowlist)
            parameter_gestion.config_service_drowlis = config_service_drowlis
            parameter_gestion.name_campo = "ra_con_tipos_identificacion_id_tipo_identificacion"
            parameter_gestion.aleas_campo = "Tipo identificación"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 0
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.tipo_campo = "INT"
            parameter_gestion.max_leng_campo = 9
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = "ra_con_usuario_consulta_publica"
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.error_gestion = "YES"
            parameter_gestion.drow_name_controls_destino = ""
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.obligatorio_campo = 1
            parameter_gestion.valida_capital_text = 0
            parameter_gestion.control_input_class = "form-control-drow"
            parameter_gestion.label_input_class_font = ""
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.tooltipAyuda = "Seleccione su tipo de identificación entre las opciones mostradas a continuación, este registro de campos es obligatorio"
            Dim Class_service_ilist_drowlist = New List(Of Class_config_general_service.Class_service_ilist_drowlist)()
            Result = Class_config_general_services.Solicita_datos_drowlist_form_control(parameter_gestion.config_service_drowlis,
                                                                                        Class_service_ilist_drowlist)
            If Result <> "YES" Then
                Solicita_datos_campos_registro_consulta_publica = Result
                Exit Function
            End If
            Class_config_general_service.Add(parameter_gestion)
            Class_config_general_service.Item(0).ilist_row_drowlist = Class_service_ilist_drowlist

            parameter_gestion = New Class_config_general_service()
            parameter_gestion.name_campo = "numero_identificacion"
            parameter_gestion.aleas_campo = "Indentificación"
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
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = "ra_con_usuario_consulta_publica"
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.obligatorio_campo = 1
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.Place_Holder = "Digite su número de identificación"
            parameter_gestion.tooltipAyuda = "Digite su número de identificación, para el caso de las personas jurídicas por favor digite la identificación con el digito de verificación DIAN"
            parameter_gestion.error_gestion = "YES"
            Class_config_general_service.Add(parameter_gestion)

            parameter_gestion = New Class_config_general_service()
            parameter_gestion.name_campo = "primer_nombre"
            parameter_gestion.aleas_campo = "Primer nombre"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.disable_campo = 1
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 80
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = "ra_con_usuario_consulta_publica"
            parameter_gestion.error_gestion = "YES"
            parameter_gestion.obligatorio_campo = 1
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.Place_Holder = "Digite su primer nombre"
            parameter_gestion.tooltipAyuda = "Digite su primer nombre para completar el registro, este dato es de registro obligatorio"
            Class_config_general_service.Add(parameter_gestion)

            parameter_gestion = New Class_config_general_service()
            parameter_gestion.name_campo = "segundo_nombre"
            parameter_gestion.aleas_campo = "Segundo nombre"
            parameter_gestion.alow_null = 1
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 0
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.disable_campo = 1
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 80
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = "ra_con_usuario_consulta_publica"
            parameter_gestion.obligatorio_campo = 0
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.Place_Holder = "Digite su segundo nombre"
            parameter_gestion.tooltipAyuda = "Digite su segundo nombre para completar el registro, este dato de registro es opcional"
            parameter_gestion.error_gestion = "YES"
            Class_config_general_service.Add(parameter_gestion)

            parameter_gestion = New Class_config_general_service()
            parameter_gestion.name_campo = "primer_apellido"
            parameter_gestion.aleas_campo = "Primer apellido"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.disable_campo = 1
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 80
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = "ra_con_usuario_consulta_publica"
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.obligatorio_campo = 1
            parameter_gestion.error_gestion = "YES"
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.Place_Holder = "Digite su primer apellido"
            parameter_gestion.tooltipAyuda = "Digite su primer apellido para completar el registro, este dato de registro es obligatorio"
            Class_config_general_service.Add(parameter_gestion)

            parameter_gestion = New Class_config_general_service()
            parameter_gestion.name_campo = "segundo_apellido"
            parameter_gestion.aleas_campo = "Segundo apellido"
            parameter_gestion.alow_null = 1
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 0
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.disable_campo = 1
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 80
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = 0
            parameter_gestion.tbl_control = "ra_con_usuario_consulta_publica"
            parameter_gestion.type_cells_alow_control = "group"
            parameter_gestion.obligatorio_campo = 0
            parameter_gestion.clas_service_control = "NA"
            parameter_gestion.Tupcae_label = ""
            parameter_gestion.Place_Holder = "Digite su segundo apellido"
            parameter_gestion.tooltipAyuda = "Digite su segundo apellido para completar el registro, este dato es opcional"
            parameter_gestion.error_gestion = "YES"
            Class_config_general_service.Add(parameter_gestion)
            Solicita_datos_campos_registro_consulta_publica = "YES"
        Catch ex As Exception
            Solicita_datos_campos_registro_consulta_publica = "Inconsistencia general funcion Solicita_datos_campos_registro_consulta_publica " & ex.Message
        End Try
    End Function
End Class
