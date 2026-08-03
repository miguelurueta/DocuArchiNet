Imports MySql.Data.MySqlClient
Public Class class_rad_parameter_oper_document
    Property DG_NOMBRE_GABINETE As String
    Property DG_RADICADO As String
    Property DG_LISTA_CHEQUEO As Integer
    Property DG_ID_CONFIG_DIGITALIZACION As Integer
    Property ID_IMAGEN As Integer
    Property DG_TIPODIGITALIZACION As String
    Property VALUE_ITEM As Integer
    Property TEXT_ITEM As String
    Property ID_TAREA_SELECCIONDA As Long
End Class
Public Class class_rad_return_registro_radicado
    Property codigo_radicado As String
    Property asignar_radicado As String
    Property id_registro_estado As Object
    Property error_gestion As String
End Class
Public Class Class_ra_radicacion_simplificada
    Function Solicita_estructura_radicacion_simplificada(ByRef Class_config_general_service As List(Of Class_config_general_service)) As String
        '------------------------------------------------------------------------------------
        'Funcion : Solicita estructura de radicación simplificada
        '------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------------
        '
        '
        '
        '
        '
        '------------------------------------------------------------------------------------
        '                           RETORNO
        '------------------------------------------------------------------------------------
        'Class_config_general_service  : Retorna la estructura de radicación
        '------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '------------------------------------------------------------------------------------
        'Fecha                 : 2024-10-25
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim Class_system_plantilla_defaul_simplificada As New class_system_plantilla_defaul_simplificada
            Result = Class_system_plantilla_radicado.Solicita_estructura_plantilla_radicacion_default_simplificada(Class_system_plantilla_defaul_simplificada)
            If Result <> "YES" Then
                Solicita_estructura_radicacion_simplificada = Result
                Exit Function
            End If
            Dim Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
            Result = Class_ra_detalle_plantilla_radicado.Solicita_estructura_campos_radicacion_simplificada(Class_system_plantilla_defaul_simplificada.id_Plantilla,
                                                                                                            Class_system_plantilla_defaul_simplificada.Nombre_Plantilla_Radicado,
                                                                                                           "content_radicacion_simplificada",
                                                                                                            "",
                                                                                                            "content_radicacion_simplificada",
                                                                                                            1,
                                                                                                            0,
                                                                                                            1,
                                                                                                            Class_config_general_service)
            If Result <> "YES" Then
                Solicita_estructura_radicacion_simplificada = Result
                Exit Function
            End If
            Solicita_estructura_radicacion_simplificada = "YES"
        Catch ex As Exception
            Solicita_estructura_radicacion_simplificada = "Inconsistencia general funcion Solicita_estructura_radicacion_simplificada " & ex.Message
        End Try
    End Function
    Function Registro_radicacion_simplificada(ByVal nombre_plantilla As String,
                                              ByVal id_usuario_radicacion As Integer,
                                              ByVal id_actividad_usuario_workflow As Integer,
                                              ByVal id_usuario_workflow As Integer,
                                              ByVal id_usuario_gestion As Integer,
                                              ByVal loguin_usuario_gestion As String,
                                              ByVal Class_config_general_service As List(Of Class_config_general_service),
                                              ByRef codigo_radicado_consecutivo As String,
                                              ByRef Asignar_radicado As String,
                                              ByRef id_registro_estado As Object) As String

        '------------------------------------------------------------------------------------
        'Funcion : Registra radicación simplificada 
        '------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------------
        'id_plantilla           : Representa la identificación de la planilla de radicación
        'nombre_plantilla       : Representa el nombre de la plantilla de radicación
        'id_tipo_plantilla      : Representa la idneitfcación del tipo de plantilla
        'id_usuario_radicacion  : Representa el usuario que radica la correspondencia
        'Class_config_general_service : Representa la estructura con los campos y valores
        '------------------------------------------------------------------------------------
        '                           RETORNO
        '------------------------------------------------------------------------------------
        'codigo_radicado_consecutivo  : Retorna el consecutivo del radicado
        '------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '------------------------------------------------------------------------------------
        'Fecha                 : 2024-10-25
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------
        Dim Result As String = ""
        Dim ClassRadicador As New ClassRadicador
        Dim estado_autorizacion As String = ""
        Dim id_plantilla As Integer = 0
        '-----///Solicita datos plantilla radicacion///------------
        Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
        Result = Class_system_plantilla_radicado.SolicitaIdPlantillaRadicado(id_plantilla,
                                                                               nombre_plantilla)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        Dim tipo_plantilla As String = "RADICACION ENTRANTE"
        Result = Class_system_plantilla_radicado.Retorna_Tipo_Plantilla(id_plantilla,
                                                                        tipo_plantilla)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        Dim id_tipo_plantilla As Integer = 0
        If tipo_plantilla = "RADICACION ENTRANTE" Then
            id_tipo_plantilla = 1
        Else
            id_tipo_plantilla = 2
        End If
        '------///Solicita autorización del tiempo permitido de radicación///----------
        Result = ClassRadicador.Solicita_autorizacion_dias_hora_de_radicacion(1,
                                                                              id_plantilla,
                                                                              estado_autorizacion)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        If estado_autorizacion <> "YES" Then
            Registro_radicacion_simplificada = estado_autorizacion
            Exit Function
        End If
        '------///Solicita la identifiicacion del flujo de trabajo///----------
        Dim id_flujo_trabajo As Object = "0"
        For i As Integer = 0 To Class_config_general_service.Count - 1
            If Class_config_general_service.Item(i).name_campo = "RE_flujo_trabajo" Then
                If Class_config_general_service.Item(i).value_campo <> "" And Class_config_general_service.Item(i).value_campo <> "0" _
                    And Class_config_general_service.Item(i).value_campo <> "-1" Then
                    id_flujo_trabajo = Class_config_general_service.Item(i).value_campo
                Else
                    id_flujo_trabajo = "null"
                End If
            End If
        Next
        '------///Valida existencia actividad de inicio de flujo de trabajo ///----------
        Dim Ref_class_flujo_workflow As New Class_flujo_trabajo_workflow
        Dim Existencia As String = ""
        If id_flujo_trabajo <> "null" Then
            Result = Ref_class_flujo_workflow.Solicita_existencia_actvidad_inicio_flujo_trabajo(id_flujo_trabajo,
                                                                                                Existencia)
            If Result <> "YES" Then
                Registro_radicacion_simplificada = Result
                Exit Function
            End If
        End If
        If Existencia = "NO" Then
            Registro_radicacion_simplificada = "Está intentando radicar un trámite relacionado a un flujo de trabajo el cual no tiene una actividad de inicio relacionada"
            Exit Function
        End If
        '----///////Solicita la actvidad de flujo documental del usuario workflow seleccionado
        Dim id_actividad_flujo_trabajo As Integer = 0
        Dim id_usuario_workflow_flujo_trabajo As Integer = 0
        Dim id_registro_actvidad_flujo_trabajo As Integer = 0
        Dim nombre_flujo_trabajo As String = ""
        Dim Refclas_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
        If id_flujo_trabajo <> 0 Then
            Result = Ref_class_flujo_workflow.Solicita_datos_actividad_inicio_flujo(id_flujo_trabajo,
                                                                                    id_registro_actvidad_flujo_trabajo,
                                                                                    id_actividad_flujo_trabajo,
                                                                                    id_usuario_workflow_flujo_trabajo)
            If Result <> "YES" Then
                Registro_radicacion_simplificada = Result
                Exit Function
            End If
            If id_registro_actvidad_flujo_trabajo <> 0 Then
                'id_usuario_workflow = id_usuario_workflow_flujo_trabajo
                ' = id_actividad_flujo_trabajo
            Else
                Result = Refclas_flujo_trabajo_workflow.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo,
                                                                                                  nombre_flujo_trabajo)
                If Result <> "YES" Then
                    Registro_radicacion_simplificada = Result
                    Exit Function
                End If
                If Result <> "YES" Then
                    Registro_radicacion_simplificada = "El flujo (" & nombre_flujo_trabajo & ") no tiene una actividad de inicio contacte al administrador"
                    Exit Function
                End If
            End If
        End If
        Dim ConseCRadicado As Integer = 0
        Dim ConseCCodbarra As Integer = 0
        Dim Resultado_General As String = "YES"
        Dim _Plantilla_Impre() As String
        Dim _Campo_NIT As String = ""
        Dim _Campo_NIT_REAL As String = ""
        Erase _Plantilla_Impre
        Dim Estado_opcion_fecha As Integer = 1
        Dim Estado_opcion_cita_respuesta As Integer = 1
        Dim Estado_opcion_radicado_general As Integer = 1
        Dim Estado_opcion_valida_externo As Integer = 1
        Dim Estado_opcion_radicado_codigo_corto As Integer = 0
        Dim Util_activo_plantilla_codigo_simple As Integer = 0
        '------///Lista opciones plantilla////------
        Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
        Result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(id_plantilla,
                                                                                       Estado_opcion_fecha,
                                                                                       Estado_opcion_cita_respuesta,
                                                                                       Estado_opcion_radicado_general,
                                                                                       Estado_opcion_valida_externo,
                                                                                       Estado_opcion_radicado_codigo_corto,
                                                                                       Util_activo_plantilla_codigo_simple)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        '--/////Solicita la lista de campos de la plantilla de radicación--///////
        Dim Campos_Plantilla() As Campos_Plantilla
        Erase Campos_Plantilla
        Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
        Result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(id_plantilla,
                                                                                            Campos_Plantilla,
                                                                                            Estado_opcion_fecha,
                                                                                            Estado_opcion_cita_respuesta,
                                                                                            Estado_opcion_radicado_general)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        '--/////Solicita la lista de parametros de script de validación de la plantilla de radicación--///////
        Dim ref_Class_ra_script_actividades As New Class_ra_script_actividades
        Dim validacion_plantilla() As validacion_plantilla
        Erase validacion_plantilla
        Result = ref_Class_ra_script_actividades.lista_campos_Validacion_plantilla(id_plantilla,
                                                                                   validacion_plantilla)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        '--/////Asigna los parametros de validación a los campos de la  de plantillas de radicación--///////
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
        '--/////Asigna datos a los campos extraidos del formulario--/////// name_campo  Descripcion_Documento
        Dim id_tipo_tramite_documento As Integer = 0
        Dim Descripcion_documento As String = ""
        For i3 As Integer = 0 To Campos_Plantilla.Length - 1
            If Campos_Plantilla(i3).Estado_Campo = 1 And Campos_Plantilla(i3).Campo_rad_externo = 1 Then
                For i As Integer = 0 To Class_config_general_service.Count - 1
                    If UCase(Campos_Plantilla(i3).Campo_Plantilla) = UCase(Class_config_general_service.Item(i).name_campo) Then
                        Campos_Plantilla(i3).TEXTO_CAMPO = Class_config_general_service.Item(i).texto_campo
                        Campos_Plantilla(i3).ID_CAMPO_ASPNET = Class_config_general_service.Item(i).name_campo
                        Campos_Plantilla(i3).VALUE_CAMPO = Class_config_general_service.Item(i).value_campo
                        If Campos_Plantilla(i3).Campo_Plantilla = "Descripcion_Documento" Then
                            Descripcion_documento = Class_config_general_service.Item(i).texto_campo
                            id_tipo_tramite_documento = Class_config_general_service.Item(i).value_campo
                        End If
                    End If
                Next
            End If
        Next
        '--///Calcula tiempo respuesta trammite fecha límite y asigna al campo----/////
        Dim fecha_vence As String = ""
        Dim ClassGestionFechas As New ClassGestionFechas
        Result = ClassGestionFechas.Solicita_fecha_limite_tramite(id_plantilla,
                                                                  Descripcion_documento,
                                                                  fecha_vence)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        '--///Asigna datos fecha de vencimiento tramite---//////
        For i3 As Integer = 0 To Campos_Plantilla.Length - 1
            If Campos_Plantilla(i3).Campo_Plantilla = "FECHALIMITERESPUESTA" Then
                Campos_Plantilla(i3).TEXTO_CAMPO = fecha_vence
                Exit For
            End If
        Next
        '--///Asigna codigo destinatario externo------//////
        Dim codigo_dest_externo As Integer = -1
        For i As Integer = 0 To Class_config_general_service.Count - 1
            If Class_config_general_service.Item(i).name_campo = "REMITENTE_COR" Then
                codigo_dest_externo = Class_config_general_service.Item(i).value_campo
                Exit For
            End If
        Next
        '////-----Retorna id usuario destino radicacion---////
        Dim codigo_destinatario As Integer = -1
        Dim ref_Destinatario_Cor As String = ""
        Dim ref_cargo_festinatario As String = ""
        Dim value_tempo As String = ""
        For i As Integer = 0 To Class_config_general_service.Count - 1
            If Class_config_general_service.Item(i).name_campo = "Destinatario_Cor" Then
                codigo_destinatario = Class_config_general_service.Item(i).value_campo
                value_tempo = Class_config_general_service.Item(i).texto_campo
                Exit For
            End If
        Next
        If value_tempo <> "" Then
            Dim split_dest() As String = value_tempo.ToString.Split("<")
            ref_Destinatario_Cor = Trim(split_dest(0))
            value_tempo = value_tempo.Replace(ref_Destinatario_Cor, "")
            value_tempo = value_tempo.Replace("<", "")
            value_tempo = value_tempo.Replace(">", "")
            value_tempo = Trim(value_tempo)
            ref_cargo_festinatario = value_tempo
        End If
        '--/////Asigna datos validación por script---//////////
        Dim Class_plantilla_validacion As New Class_plantilla_validacion
        For i3 As Integer = 0 To Campos_Plantilla.Length - 1
            If Campos_Plantilla(i3).Estado_Campo = 1 Then
                If Campos_Plantilla(i3).ID_SCRIPT <> 0 And Campos_Plantilla(i3).VALUE_CAMPO <> "" Then
                    Result = Class_plantilla_validacion.Asigna_datos_validacion_campos_radicacion(Campos_Plantilla(i3).ID_SCRIPT,
                                                                                                  Campos_Plantilla(i3).Campo_Plantilla,
                                                                                                  id_plantilla,
                                                                                                  Val(Campos_Plantilla(i3).VALUE_CAMPO),
                                                                                                  Campos_Plantilla)
                    If Result <> "YES" Then
                        Registro_radicacion_simplificada = Result
                        Exit Function
                    End If
                End If
            End If
        Next

        Dim fecha_documento_ As String = ""
        Result = ClassGestionFechas.FormateaFechaAlmacenamiento(fecha_documento_)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If

        '/////----------Verifica campos obligatorios de radicación simple o de sistema-------/////
        For i4 As Integer = 0 To Campos_Plantilla.Length - 1
            If Campos_Plantilla(i4).Campo_Obligatorio = 1 And Campos_Plantilla(i4).Estado_Campo = 1 And (Campos_Plantilla(i4).campo_sistema = 1 Or Campos_Plantilla(i4).Campo_rad_simple = 1) Then
                If Campos_Plantilla(i4).Comportamiento_Campo = "SELECCION" Then
                    If Campos_Plantilla(i4).TEXTO_CAMPO = "" Or Campos_Plantilla(i4).TEXTO_CAMPO = "SELECCIONE" Then
                        Select Case UCase(Campos_Plantilla(i4).Alias_Campo)
                            Case "MEDIO RECEPCION"
                            Case Else
                                Registro_radicacion_simplificada = "Debe seleccionar el campo " & UCase(Campos_Plantilla(i4).Alias_Campo)
                                Exit Function
                        End Select

                    End If
                Else
                    Select Case UCase(Campos_Plantilla(i4).Alias_Campo)
                        Case "ANEXOS"
                            Campos_Plantilla(i4).TEXTO_CAMPO = "NA"
                        Case "FECHA DOCUMENTO"
                            Campos_Plantilla(i4).TEXTO_CAMPO = fecha_documento_
                        Case "NUMERO FOLIOS"
                            Campos_Plantilla(i4).TEXTO_CAMPO = "0"
                        Case "IDENTIFICACION_REMITENTE"

                        Case "MEDIO RECEPCION"
                            Campos_Plantilla(i4).TEXTO_CAMPO = "NO INFORMADO"
                        Case Else
                            If Campos_Plantilla(i4).TEXTO_CAMPO = "" Then
                                Registro_radicacion_simplificada = "Debe informar el campo " & UCase(Campos_Plantilla(i4).Alias_Campo)
                                Exit Function
                            End If
                    End Select
                End If
            End If
        Next

        '////-----Verifica formatos campos  fechas---/////
        For i4 As Integer = 0 To Campos_Plantilla.Length - 1
            If Campos_Plantilla(i4).Estado_Campo = 1 And Campos_Plantilla(i4).Campo_rad_externo = 1 And Campos_Plantilla(i4).Tipo_Campo = "Date" And Campos_Plantilla(i4).TEXTO_CAMPO <> "" Then
                Result = ClassGestionFechas.Verifi_campo_fecha(Campos_Plantilla(i4).TEXTO_CAMPO)
                If Result <> "YES" Then
                    Registro_radicacion_simplificada = Result & "El formato del campo  " & Campos_Plantilla(i4).Campo_Plantilla
                    Exit Function
                End If
            End If
        Next
        Dim expediente_documento As String = "Null"
        '----////Retorna nombre del area y la identificación del area del destinatario----/////
        Dim nombre_area As String = ""
        Dim id_area As Integer = -1
        Dim Ref_class_remit_dest_int As New Class_remit_dest_interno
        Result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(codigo_destinatario,
                                                                                    id_area,
                                                                                    nombre_area)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        Dim estado_modulo_respuesta As Integer = 0
        Dim Refclas_tipo_doc_entrante As New Class_tipo_doc_entrante
        Result = Refclas_tipo_doc_entrante.Determina_gestion_modulo_pqr_Tipo_Tramite(id_plantilla,
                                                                                     Descripcion_documento,
                                                                                     estado_modulo_respuesta)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
        '-----//////Retorna sede empresa usuario radicador----/////  
        Dim nombre_sede As String = ""
        Dim id_sede As Integer = -1
        Dim Ref_Class_usuario_radicador As New Class_usuario_radicador
        Result = Ref_Class_usuario_radicador.Retorna_Id_Nombre_sede_Empresa(id_usuario_radicacion,
                                                                            id_sede,
                                                                            nombre_sede)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        Dim pre_consecutivo As String = año_radic & id_area.ToString.PadLeft((4 - id_area.ToString.Length) + id_area.ToString.Length, "0") &
         id_plantilla.ToString.PadLeft((2 - id_plantilla.ToString.Length) + id_plantilla.ToString.Length, "0")
        Dim SqlCampos As String = ""
        SqlCampos = "INSERT INTO " & nombre_plantilla & " ( Destinatario_Externo_id_Dest_Ext," &
            "Remit_Dest_Interno_id_Remit_Dest_Int,Usuario_Radicador_id_usuario,System_Plantilla_Radicado_id_Plantilla," &
            "Consecutivo_Rad,Consecutivo_CodBarra,Fecha_Radicado,Codigo_Sede,Id_area_remit_dest_interno,Area_remit_dest_interno,Tipo_radicado_plantilla,CARGO_DESTINATARIO"

        '----////Agrega campos dinamicos----/////////
        For i4 As Integer = 0 To Campos_Plantilla.Length - 1
            If Campos_Plantilla(i4).Estado_Campo = 1 Then
                SqlCampos = SqlCampos & "," & Campos_Plantilla(i4).Campo_Plantilla
            End If
        Next
        Dim sqlrespuesta As String = ""
        Dim estado_respuesta As Integer = 0
        Dim existencia_tabla_respuesta As String = "NO"
        Dim class_tipo_doc_entrante As New Class_tipo_doc_entrante
        Result = class_tipo_doc_entrante.Retorna_tipo_respuesta_tramite_radicado(id_plantilla,
                                                                                 Descripcion_documento,
                                                                                 estado_respuesta)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        '----///Registra estado respuesta radicado---/////
        Dim estado_resp_obligatoria As Integer = 0
        If estado_respuesta <> 0 Then
            sqlrespuesta = "Insert into ra_respuesta_radicado (ID_REMIT_DEST_INT,ID_AREA,system_plantilla_radicado_id_plantilla," &
             "RADICADO,FECHA_REGISTRO,FECHA_VENCE,DESTINATARIO,TRAMITE_DOCUMENTO,codigo_dest_externo,ASUNTO,AREA_RESPONSABLE,CARGO_RESPONSABLE,USUARIO_RESPONSABLE,TIPO_RESPUESTA_ELAB_USUARIO) VALUES "
            Result = class_tipo_doc_entrante.Retorna_estado_respuesta_obligatoria(id_plantilla,
                                                                                  Descripcion_documento,
                                                                                  estado_resp_obligatoria)
            If Result <> "YES" Then
                Registro_radicacion_simplificada = Result
                Exit Function
            End If
        End If
        '----////Asignar datos radicacion---///////
        Dim date1al As String = Date.Now
        Result = ClassGestionFechas.Formatea_fecha_time_framework(Date.Now,
                                                                  date1al)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = "Imposible formatear fecha " & Result
            Exit Function
        End If
        '-----/////Solicita tipo modulo de envio si se envia  1- Modulo workflow 2-Modulo simplificado y modulo radicación 3 Solo para soporte documental sin gestión------///
        Dim ref_class_tipo_doc_emtrante As New Class_tipo_doc_entrante
        Dim tipo_envio_tramite As Integer = 0
        Result = ref_class_tipo_doc_emtrante.Solicita_tipo_modulo_soporte_documental_envio(id_tipo_tramite_documento,
                                                                                           tipo_envio_tramite)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        '---///////Determina si el radicado genera flujo----///////
        Dim estado_flow_sube_radicado As Integer = 2
        Result = class_tipo_doc_entrante.Determina_Suberadicado_Tipo_Tramite(id_plantilla,
                                                                             Descripcion_documento,
                                                                             estado_flow_sube_radicado)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        '----/////Asigna estado de sube radicado del registro del radicado
        If tipo_envio_tramite = 2 Or tipo_envio_tramite = 3 Then
            estado_flow_sube_radicado = 2
        End If
        '---////Implementa consecutivo radicado general---/////
        Dim Ref_Class_system_plantilla_consecutivo_radicado As New Class_system_plantilla_consecutivo_radicado
        Dim Estado_consecutivo_radicado_general As String = ""
        Result = Ref_Class_system_plantilla_consecutivo_radicado.Solicita_estado_registro_consecutivo_radicado(Estado_consecutivo_radicado_general)
        If Result <> "YES" Then
            Registro_radicacion_simplificada = Result
            Exit Function
        End If
        '----////Asigna sql consecutivo radicado---/////
        Dim Parametro_Select_System1 As String = ""
        If Estado_consecutivo_radicado_general = "YES" Then
            Parametro_Select_System1 = " Select Consecutivo_Rad,Consecutivo_CodBarra FROM system_plantilla_consecutivo_radicado  For update"
        Else
            Parametro_Select_System1 = " Select Consecutivo_Rad,Consecutivo_CodBarra FROM system_plantilla_Radicado " &
            " where id_plantilla=" & id_plantilla & " For update"
        End If
        '----////Implemetar función Inicializa consecutivo radicado anual---//////
        If Estado_consecutivo_radicado_general = "YES" Then
            Result = Ref_Class_system_plantilla_consecutivo_radicado.Inicializa_consecutivo_radicado_anual()
            If Result <> "YES" Then
                Registro_radicacion_simplificada = Result
                Exit Function
            End If
        End If
        Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
        If tipo_envio_tramite = 2 Or tipo_envio_tramite = 3 Then
            Dim radicado_asignado As String = ""
            Dim estado_asignado As String = ""
            Result = Class_ra_rad_estados_modulo_radicacion.Solicita_radicado_existencia_radicado_asignado(id_usuario_radicacion,
                                                                                                           id_plantilla,
                                                                                                           id_tipo_plantilla,
                                                                                                           estado_asignado,
                                                                                                           radicado_asignado,
                                                                                                           0,
                                                                                                           0)
            If Result <> "YES" Then
                Registro_radicacion_simplificada = Result
                Exit Function
            End If
            If estado_asignado = "YES" Then
                Registro_radicacion_simplificada = "El usuario tiene pendiente por terminar el radicado : " & radicado_asignado & " imposible radicar"
                Exit Function
            End If

        End If
        '-----//Formatea fecha de almacenamiento para radicado-----////
        Dim DateCreate As Date = Now
        Dim fecha_selecion As Object = Nothing
        Dim Refclas_gestion_fecha As New ClassGestionFechas
        Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                    fecha_selecion)
        If Result <> "YES" Then
            Asignar_radicado = Result
            Exit Function
        End If
        '---/////Deteramina tipo radicado en la plantilla nuevo---////
        Dim tipo_radicado_plnatilla As Integer = 1
        Dim SqlDatos As String = "( '" & codigo_destinatario & "','" & codigo_dest_externo & "','" & id_usuario_radicacion & "','" &
         id_plantilla & "'"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim sqlresultinsert As Integer = 0
        Dim Class_zero_fill As New Class_zero_fill
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Registro_radicacion_simplificada = "Imposible Encontrar Registro En Tabla Systema Error Conexion"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Registro_radicacion_simplificada = "Imposible Encontrar Registro En Tabla Systema"
                myConnection.Close()
                Exit Function
            End If
            '////-----Valores recuperados de la consulta de la tabla system1---////
            mySqldatReader.Read()
            ConseCRadicado = mySqldatReader.Item(0)
            ConseCCodbarra = mySqldatReader.Item(1)
            ConseCRadicado = ConseCRadicado + 1
            ConseCCodbarra = ConseCCodbarra + 1
            Dim ConsecutivoRadicadoString As String = ConseCRadicado.ToString
            Dim conseCCodbarrastrin As String = ConseCCodbarra.ToString
            If Estado_opcion_radicado_codigo_corto = 0 Then
                Result = Class_zero_fill.zero_fill(ConsecutivoRadicadoString, 5, "0")
                If Result <> "YES" Then
                    Registro_radicacion_simplificada = "Imposible agregar zerofill " & Result
                    myConnection.Close()
                    Exit Function
                End If
                Result = Class_zero_fill.zero_fill(conseCCodbarrastrin, 5, "0")
                If Result <> "YES" Then
                    Registro_radicacion_simplificada = "Imposible agregar zerofill " & Result
                    myConnection.Close()
                    Exit Function
                End If
            End If
            If Estado_opcion_radicado_codigo_corto = 0 Then
                ConsecutivoRadicadoString = pre_consecutivo & ConsecutivoRadicadoString
                conseCCodbarrastrin = pre_consecutivo & conseCCodbarrastrin
            Else
                ConsecutivoRadicadoString = año_radic & id_plantilla & ConsecutivoRadicadoString
                conseCCodbarrastrin = año_radic & id_plantilla & conseCCodbarrastrin
            End If
            '----/////Hábilita código simple---/////////
            If Util_activo_plantilla_codigo_simple = 1 Then
                ConsecutivoRadicadoString = ConseCRadicado.ToString
            End If
            SqlDatos = SqlDatos & ",'" & ConsecutivoRadicadoString & "'"
            SqlDatos = SqlDatos & ",'" & conseCCodbarrastrin & "'"
            SqlDatos = SqlDatos & ",'" & date1al & "'"
            SqlDatos = SqlDatos & ",'" & id_sede & "'"
            SqlDatos = SqlDatos & ",'" & id_area & "'"
            SqlDatos = SqlDatos & ",'" & nombre_area & "'"
            SqlDatos = SqlDatos & ",'" & tipo_radicado_plnatilla & "'"
            SqlDatos = SqlDatos & ",'" & ref_cargo_festinatario & "'"
            mySqldatReader.Close()
            Dim remitente_externo As String = ""
            For i4 As Integer = 0 To Campos_Plantilla.Length - 1
                If Campos_Plantilla(i4).Estado_Campo = 1 Then
                    If Campos_Plantilla(i4).TEXTO_CAMPO = "" Then
                        SqlDatos = SqlDatos & "," & "null"
                    Else
                        If Campos_Plantilla(i4).Campo_Plantilla = "REMITENTE_COR" Then
                            remitente_externo = Campos_Plantilla(i4).TEXTO_CAMPO
                        End If
                        If Campos_Plantilla(i4).Campo_Plantilla = "FECHALIMITERESPUESTA" Then
                            fecha_vence = Campos_Plantilla(i4).TEXTO_CAMPO
                        End If
                        If Campos_Plantilla(i4).Campo_Plantilla = "Destinatario_Cor" Then
                            SqlDatos = SqlDatos & ",'" & Trim(ref_Destinatario_Cor) & "'"
                        Else
                            SqlDatos = SqlDatos & ",'" & Trim(Campos_Plantilla(i4).TEXTO_CAMPO) & "'"
                        End If
                    End If
                End If
            Next
            '---////Insertar expediente---////
            'If tipo_radicado_plnatilla = 2 Then
            '    SqlCampos = SqlCampos & "," & "Expediente,id_Expediente"
            '    SqlDatos = SqlDatos & ",'" & Textbox_expediente_val_radicacion.Text & "','" & Hiddenid_expediente.value & "'"
            'End If
            SqlCampos = SqlCampos & "," & "Flag_Flow"
            SqlCampos = SqlCampos & "," & "Estado_Radicado,tipo_doc_entrante_id_tipo_doc_entrante,id_tipo_flujo_workflow"
            SqlCampos = SqlCampos & " ) values "
            SqlDatos = SqlDatos & ",'" & estado_flow_sube_radicado & "'"
            SqlDatos = SqlDatos & ",'" & "1" & "'," & id_tipo_tramite_documento & "," & id_flujo_trabajo
            SqlDatos = SqlDatos & ")"
            Dim Parametro_Insercio As String = SqlCampos & SqlDatos
            Dim Parametro_Actualiza_System1 As String = ""
            If Estado_consecutivo_radicado_general = "YES" Then
                Parametro_Actualiza_System1 = "update system_plantilla_consecutivo_radicado set Consecutivo_Rad = " & "'" & ConseCRadicado & "' ," &
                " Consecutivo_CodBarra = " & "'" & ConseCRadicado & "'"
            Else
                Parametro_Actualiza_System1 = "update system_plantilla_radicado set Consecutivo_Rad = " & "'" & ConseCRadicado & "' ," &
                " Consecutivo_CodBarra = " & "'" & ConseCRadicado & "'" & " where id_Plantilla =" &
                id_plantilla
            End If
            myCommand.CommandText = Parametro_Actualiza_System1
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registro_radicacion_simplificada = "Imposible actualizar el consecutivo del radicacion en la plantilla  "
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = Parametro_Insercio
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registro_radicacion_simplificada = "Imposible registrar el radicado en la plantilla  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim id_registro_radicado = myCommand.LastInsertedId
            '---------------------------------------------------------------------
            'Registro general de radicado
            '---------------------------------------------------------------------
            Dim fecha_documento As String = "null"
            Dim asunto_documento As String = "null"
            Dim numero_folios As String = "null"
            For i4 As Integer = 0 To Campos_Plantilla.Length - 1
                Select Case Campos_Plantilla(i4).Campo_Plantilla
                    Case "Fecha_Documento"
                        fecha_documento = "'" & Campos_Plantilla(i4).TEXTO_CAMPO & "'"
                    Case "Numero_Folios"
                        numero_folios = Campos_Plantilla(i4).TEXTO_CAMPO
                    Case "ASUNTO"
                        asunto_documento = "'" & Campos_Plantilla(i4).TEXTO_CAMPO & "'"
                End Select
            Next
            Dim parametro_produccion_documental As String = ""
            parametro_produccion_documental = "INSERT INTO ra_registro_general_radicacion " &
            " (System_Plantilla_Radicado_id_Plantilla,id_Radicado_plantilla,Nombre_plantilla_radicado,Consecutivo_Rad,Consecutivo_CodBarra,Flag_Flow) values (" &
            id_plantilla & "," & id_registro_radicado & ",'" & nombre_plantilla & "','" & ConsecutivoRadicadoString & "','" & conseCCodbarrastrin & "',0 )"
            myCommand.CommandText = parametro_produccion_documental
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registro_radicacion_simplificada = "Imposible realizar el registro general del radicado  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '----//////inserta registro respuesta al tramite----//////
            If sqlrespuesta <> "" Then
                Dim sql_campos_insert As String = " ( " & codigo_destinatario & "," & id_area & "," & id_plantilla & ",'" &
                ConsecutivoRadicadoString & "','" & date1al & "','" & fecha_vence & "','" & remitente_externo & "','" & Descripcion_documento & "'," & codigo_dest_externo & "," & asunto_documento &
                     ",'" & nombre_area & "','" & ref_cargo_festinatario & "','" & ref_Destinatario_Cor & "','" & estado_resp_obligatoria & "')"
                Parametro_Insercio = sqlrespuesta & sql_campos_insert
                myCommand.CommandText = Parametro_Insercio
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Registro_radicacion_simplificada = "Imposible registrar respuesta radicado  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                Dim campos_trans_envio As String = "RADICA SOLICITUD DE RESPUESTA DEL RADICADO (" & ConsecutivoRadicadoString &
                ") el día " & date1al & " desde la plantilla de radicación (" & nombre_plantilla & ")"
                Dim insert_datos_envio As String = "('" & "RADICA SOLICITUD DE RESPUESTA" & "','" & loguin_usuario_gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
                    myCommand.LastInsertedId & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & date1al & "','RADICACION','" & campos_trans_envio & "')"
                Dim update_envio As String = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                    ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                                    insert_datos_envio
                myCommand.CommandText = update_envio
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Registro_radicacion_simplificada = "Imposible registrar log respuesta radicado   "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If

            If tipo_envio_tramite = 3 Or tipo_envio_tramite = 2 Then
                Dim sql_registro_radicado As String = "Insert into ra_rad_estados_modulo_radicacion  " &
                    "(system_plantilla_radicado_id_Plantilla,id_radicado,consecutivo_radicado,fecha_registro," &
                    "estado,remitente,id_usuario_radicado,id_tarea_workflow,tipo_doc_entrante_id_Tipo_Doc_Entrante) values (" &
                    id_plantilla & "," & id_registro_radicado & ",'" & ConsecutivoRadicadoString & "','" &
                    date1al & "'," & "0,'" & remitente_externo & "'," & id_usuario_radicacion & ",0," & id_tipo_tramite_documento & ")"
                myCommand.CommandText = sql_registro_radicado
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Registro_radicacion_simplificada = "Imposible registrar estados del radicado   "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                id_registro_estado = myCommand.LastInsertedId
            End If
            myTrans.Commit()
            myConnection.Close()
            Registro_radicacion_simplificada = "YES"
            Dim Ref_class_workflow As New ClassWorkflow
            Dim Ref_class_estados As New Class_ra_rad_estados_modulo_radicacion
            Dim id_tarea_workflow As Long = 0
            Asignar_radicado = "YES"
            Dim Rest As String = ""
            If tipo_envio_tramite = 2 Or tipo_envio_tramite = 3 Then
                '------/////Registra el flujo documental del radicado-----/////////
                If tipo_envio_tramite = 2 Then
                    Result = Ref_class_workflow.Registra_flujo_documento(id_actividad_usuario_workflow,
                                                                         id_usuario_workflow,
                                                                         0,
                                                                         ConsecutivoRadicadoString,
                                                                         id_plantilla,
                                                                         id_flujo_trabajo,
                                                                         id_registro_actvidad_flujo_trabajo,
                                                                         id_usuario_workflow_flujo_trabajo,
                                                                         0,
                                                                         estado_modulo_respuesta,
                                                                         id_tarea_workflow,
                                                                         fecha_selecion,
                                                                         1)
                    If Result <> "YES" Then
                        Asignar_radicado = "Error asignación workflow (" & Result & ") "
                        Asignar_radicado = Asignar_radicado.Replace("'", "")
                        Asignar_radicado = Asignar_radicado.Replace("\", "")
                        Asignar_radicado = Asignar_radicado.Replace("/", "")
                        Rest = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion_error(id_registro_estado,
                                                                                                                        1,
                                                                                                                        Asignar_radicado)
                        If Rest <> "YES" Then
                            Asignar_radicado = Asignar_radicado & " Error actualiza estado  (" & Result & ")"
                            Exit Function
                        End If
                        Exit Function
                    End If
                End If
                '------/////Registra el flujo documental del radicado activo para soporte documental-----/////////
                Rest = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion(id_registro_estado,
                                                                                                          0)
                If Rest <> "YES" Then
                    Asignar_radicado = Asignar_radicado & " Error actualiza estado  (" & Result & ")"
                    Exit Function
                End If
                '-----////Registra la relación del flujo de trabajo en la tabla estados radicado-----////
                If id_tarea_workflow <> 0 Then
                    Rest = Ref_class_estados.Relaciona_id_tarea_wf_estado_radicado(id_registro_estado,
                                                                                   id_tarea_workflow)
                    If Rest <> "YES" Then
                        Asignar_radicado = Asignar_radicado & " Error actualiza estado id tarea  (" & Result & ")"
                        Exit Function
                    End If
                End If
                '----/////----Actualiza estado flow radciado----//////
                Rest = ClassRadicador.Actualiza_estado_flow_radicado(ConsecutivoRadicadoString,
                                                                     nombre_plantilla,
                                                                     7)
                If Rest <> "YES" Then
                    Asignar_radicado = Rest
                    Exit Function
                End If
                Registro_radicacion_simplificada = "YES"
                Exit Function
            Else
                Registro_radicacion_simplificada = "YES"
                Exit Function
            End If
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Registro_radicacion_simplificada = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registro_radicacion_simplificada = "Error General " & e.Message
            Exit Function
        End Try
    End Function


    Function Inicializa_cliente_workflow_radicacion_simple() As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Iniciliza cliente workflow para radicación simplifiacada
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Dim Result As String = ""
            Result = Class_worflow_rutas.Retorna_nombre_ruta_por_id_ruta(HttpContext.Current.Session.Item("Id_Ruta_Workflow").ToString,
                                                                         HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"))
            If Result <> "YES" Then
                Inicializa_cliente_workflow_radicacion_simple = Result
                Exit Function
            End If
            Dim Actividad_Seleccion As Integer = 0
            Dim id_actividad As Integer = 0
            Dim Refclas_ As New Classselecciotarea
            Result = ""
            Dim TipoActividad As String = ""
            Result = Refclas_.Determina_Tipo_Actividad_Usuario(id_actividad,
                                                               TipoActividad)

            If Result <> "YES" Then
                Inicializa_cliente_workflow_radicacion_simple = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("Id_actividad_Workflow") = id_actividad
            HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD") = id_actividad
            HttpContext.Current.Session.Item("TIPOACTIVIDADWF") = TipoActividad
            Inicializa_cliente_workflow_radicacion_simple = "YES"
            Exit Function
        Catch ex As Exception
            Inicializa_cliente_workflow_radicacion_simple = "inconsistencia general función Inicializa_cliente_workflow_radicacion_simple " & ex.Message
        End Try
    End Function

End Class
