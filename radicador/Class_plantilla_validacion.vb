'Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Dynamsoft.DotNet.TWAIN.Barcode
Public Structure CAMPOS_PLANTILLA_VALIDACION
    Dim ID_CAMPO As Integer
    Dim Nombre_Campo As String
    Dim Tipo_Campo As String
    Dim Unico_campo As Integer
    Dim Aloja_null_campo As Integer
    Dim Visible_Campo As Integer
    Dim Obligatorio_Campo As Integer
    Dim Orden_Campos As Integer
    Dim IDENTI_CAMPO As Integer
    Dim TEXTO_CAMPO As String
    Dim TEXTO_CAMPO_MODIFICADO As String
    Dim valida_capital_text As Integer
    Dim Campo_nombre_pqr As Integer
    Dim Campo_correo_electrnico_pqr As Integer
    Dim Aleas_Campo_pqr As String
End Structure
Public Structure validacion_plantilla
    Dim ID_SCRIPT As Integer
    Dim Campo_Plantilla As String
    Dim TIPO_SCRIPT As String
    Dim COMBINACION_TECLA As String
    Dim VALOR_SCRIPT As String
    Dim ESTADO_ESCRIPT As String
    Dim PLATAFORMA_SCRIPT As String
    Dim IDENTI_CAMPO As Integer
    Dim TEXTO_CAMPO As String
    Dim TEXTO_CAMPO_MODIFICADO As String
End Structure
Public Structure asignacion_plantilla_validacion
    Dim Nombre_Campo_Fuente_Pla_Validacion As String
    Dim Tipo_Campo_Fuente_Pla_Validacion As String
    Dim Valor_Campo_Fuente_Pla_Validacion As String
    Dim Nombre_Campo_Destino_Pla_Radicacion As String
    Dim Tipo_Campo_Destino_Pla_Radciacion As String
    Dim Valor_Campo_Destino_Pla_Radicacion As String
End Structure
Public Structure datos_plantilla_validacion
    Dim id_plantilla_validacion As Integer
    Dim datos_campo_plantilla As String
End Structure
Public Class Class_plantilla_validacion

    Function Solicita_id_relacion_usuario_gestion_como_remitente_interno(ByVal id_usuario_gestion As Integer,
                                                                         ByVal nombre_plantilla_validacion As String,
                                                                         ByVal nombre_campo_key As String,
                                                                         ByRef id_relacion_gestion_remitente As Integer) As String
        Try
            Dim Parametro_Consulta = "select " & nombre_campo_key &
            " from " & nombre_plantilla_validacion & " where id_interno_radicado=" & id_usuario_gestion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("plantilla_validacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_relacion_usuario_gestion_como_remitente_interno = "Funcion  Verifica_existencia_campo_identificacion_usuario_gestion_plantilla dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_relacion_gestion_remitente = 0
                Solicita_id_relacion_usuario_gestion_como_remitente_interno = "YES"
                Exit Function
            Else
                id_relacion_gestion_remitente = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_relacion_usuario_gestion_como_remitente_interno = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_relacion_usuario_gestion_como_remitente_interno = "Inconsistencia general función Solicita_id_relacion_usuario_gestion_como_remitente_interno " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_usuario_remitente_plantilla_validacion(ByVal nombre_campo_remitente As String,
                                                                        ByVal nombre_plantilla_validacion As String,
                                                                        ByVal nombre_remitente_usuario_gestion As String,
                                                                        ByVal nombre_campo_key As String,
                                                                        ByRef id_remitente As Integer) As String
        '------------------------------------------------------------
        'Función : Verifica la existencia del usuario gestión
        'registrado como remitente de correspondencia
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-12-01
        '------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select " & nombre_campo_key &
            " from " & nombre_plantilla_validacion & " where " & nombre_campo_remitente & "='" & nombre_remitente_usuario_gestion & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("campos_plantilla_validacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_usuario_remitente_plantilla_validacion = "Funcion  Verifica_existencia_usuario_remitente_plantilla_validacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_remitente = 0
                Verifica_existencia_usuario_remitente_plantilla_validacion = "YES"
                Exit Function
            Else
                id_remitente = Datset.Tables(0).Rows(0).Item(0)
                Verifica_existencia_usuario_remitente_plantilla_validacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_usuario_remitente_plantilla_validacion = "Inconsistencia general función Verifica_existencia_usuario_remitente_plantilla_validacion " & ex.Message
        End Try
    End Function
    Function Verifica_Existencia_Campos_Unico_Validacion(ByVal sqlconsulta As String,
                                                         ByVal nombre_campo As String,
                                                         ByVal valor_campo As String,
                                                         ByVal aleas_campo As String)

        '******************************************************************
        'Funcion : Verifica la existencia de campos unicos de validacion
        'Fecha : 2014-08-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(sqlconsulta, Dat_reader)
            If Result <> "YES" Then
                Verifica_Existencia_Campos_Unico_Validacion = " Error verificando campo unico   " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Verifica_Existencia_Campos_Unico_Validacion = "YES"
                Exit Function
            Else
                Verifica_Existencia_Campos_Unico_Validacion = "El valor (" & valor_campo & ") ya se encuentra registrado para el campo (" & aleas_campo & ")"
                Exit Function
            End If
            Verifica_Existencia_Campos_Unico_Validacion = "YES"
        Catch ex As Exception
            Verifica_Existencia_Campos_Unico_Validacion = "Inconsistencia funcion Verifica_Existencia_Campos_Unico_Validacion " & ex.Message
        End Try
    End Function

    Function Solicita_estructura_plantilla_validacion_externos(ByVal id_scrip_plnatilla As Integer,
                                                               ByVal nombre_plantilla As String,
                                                               ByVal name_space_campo As String,
                                                               ByRef Class_config_general_service As List(Of Class_config_general_service)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita los datos de la estructura de plantilla de terceros
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_scrip_plnatilla    : Representa el codigo de la plantilla de validacion
        '                        de terceros
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service : Retorna la estructura general de los campos
        '                               para la estructura del formulario
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-10-17
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Class_config_general_service_ As New Class_config_general_service
            Dim Parametro_Consulta As String = "select  cpv.Nombre_Campo,cpv.Aleas_Campo_pqr,cpv.Tipo_Campo,cpv.Unico_campo,cpv.Aloja_null_campo " &
             " ,cpv.Obligatorio_Campo,cpv.Campo_Primari_Key,cpv.valida_capital_text,cpv.Campo_correo_electrnico_pqr,cpv.tipo_iteractua_campo" &
             "" &
            " from relacion_script_plantilla as rsp inner join campos_plantilla_validacion as cpv on " &
             " ( cpv.Plantilla_Validacion_Id_Plantilla_Validacion=rsp.Plantilla_Validacion_Id_Plantilla_Validacion)" &
            " where script_actividades_id_script = " & id_scrip_plnatilla & " and Visible_Campo=1   order by cpv.Orden_Campos"
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_estructura_plantilla_validacion_externos = " Error listando campos validacion  web service " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_plantilla_validacion_externos = "Imposible encontrar campos de validación para la plantilla (" & id_scrip_plnatilla &
                    ") de la tabla (campos_plantilla_validacion)"
                Exit Function
            Else
                For Iconta As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
                    'ojo hubo cambio
                    parameter_gestion.name_space_campo = name_space_campo
                    '----nombre campo
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(0) = False Then
                        parameter_gestion.name_campo = Dat_reader.Tables(0).Rows(Iconta).Item(0).ToString
                    Else
                        parameter_gestion.name_campo = ""
                    End If
                    '----aleas campo
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(1) = False Then
                        parameter_gestion.aleas_campo = Dat_reader.Tables(0).Rows(Iconta).Item(1).ToString
                    Else
                        parameter_gestion.aleas_campo = parameter_gestion.name_campo
                    End If
                    Dim tipo_campo As String = ""
                    Dim leng_campo As Integer = 0
                    '-----Tipo campo tamaño campo
                    If InStr(Dat_reader.Tables(0).Rows(Iconta).Item(2), "VARCHAR") > 0 Then
                        Dim temp_campo As String = Dat_reader.Tables(0).Rows(Iconta).Item(2).Replace("VARCHAR", "")
                        temp_campo = temp_campo.Replace("(", "")
                        temp_campo = temp_campo.Replace(")", "")
                        leng_campo = Val(temp_campo)
                        tipo_campo = "VARCHAR"
                    Else
                        Select Case Dat_reader.Tables(0).Rows(Iconta).Item(2)
                            Case "INT"
                                leng_campo = 9
                                tipo_campo = Dat_reader.Tables(0).Rows(Iconta).Item(2)
                            Case "DATE"
                                leng_campo = 10
                                tipo_campo = Dat_reader.Tables(0).Rows(Iconta).Item(2)
                            Case Else
                                leng_campo = 100
                                tipo_campo = Dat_reader.Tables(0).Rows(Iconta).Item(2)
                        End Select
                    End If
                    parameter_gestion.tipo_campo = tipo_campo
                    parameter_gestion.max_leng_campo = leng_campo
                    '----Campo unico
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(3) = False Then
                        parameter_gestion.campo_unico = Dat_reader.Tables(0).Rows(Iconta).Item(3)
                    Else
                        parameter_gestion.campo_unico = 0
                    End If
                    '----Aloja null
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(4) = False Then
                        parameter_gestion.alow_null = Dat_reader.Tables(0).Rows(Iconta).Item(4)
                    Else
                        parameter_gestion.alow_null = 0
                    End If
                    '----Campo obligatorio
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(5) = False Then
                        parameter_gestion.obligatorio_campo = Dat_reader.Tables(0).Rows(Iconta).Item(5)
                    Else
                        parameter_gestion.obligatorio_campo = 0
                    End If
                    '----Asigna nombre campo primary
                    If Dat_reader.Tables(0).Rows(Iconta).Item(6) = 1 Then
                        parameter_gestion.name_campo_id = Dat_reader.Tables(0).Rows(Iconta).Item(0)
                    Else
                        parameter_gestion.name_campo_id = ""
                    End If
                    '----Asigna validacion lera capital
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(7) = False Then
                        parameter_gestion.valida_capital_text = Dat_reader.Tables(0).Rows(Iconta).Item(7)
                        If parameter_gestion.valida_capital_text = 1 Then
                            Result = Class_config_general_service_.add_event_control_form("onfocusout", "ValidateCapitalLeter", parameter_gestion.event_control)
                            If Result <> "YES" Then
                                Solicita_estructura_plantilla_validacion_externos = Result
                                Exit Function
                            End If
                        End If
                    Else
                        parameter_gestion.valida_capital_text = 0
                    End If
                    '----Asigna si el control es tipo correo   
                    If Dat_reader.Tables(0).Rows(Iconta).IsNull(8) = False Then
                        parameter_gestion.control_tip_correo = Dat_reader.Tables(0).Rows(Iconta).Item(8)
                    Else
                        parameter_gestion.control_tip_correo = 0
                    End If
                    Dim homologa_tipo As Integer = 1
                    If (Dat_reader.Tables(0).Rows(Iconta).Item(9) = 2) Then
                        homologa_tipo = 0
                    End If
                    parameter_gestion.drow_name_control_id = ""  ' Inicializa campo value drowp list
                    parameter_gestion.drow_name_controls_destino = ""
                    parameter_gestion.drow_name_padre_control = ""
                    '-----Asigna tipo de control    (0-option    1- imputext)
                    parameter_gestion.campo_tip = homologa_tipo
                    '-----Asigana los valores de los campos drowlist PAIS
                    If homologa_tipo = 0 And parameter_gestion.name_campo = "Pais" Then
                        Dim config_service_drowlis As Class_config_general_service.Class_config_general_service_drowlist = New Class_config_general_service.Class_config_general_service_drowlist()
                        config_service_drowlis.limit_rows = "3000"
                        config_service_drowlis.name_campo_condicion = ""
                        config_service_drowlis.name_campo_orden = "Nombre_Pais"
                        config_service_drowlis.name_campo_value = "Nombre_Pais"
                        config_service_drowlis.name_dbs_auto = "DA"
                        config_service_drowlis.name_table_auto = "pais_radicacion"
                        config_service_drowlis.tipo_orden = "asc"
                        config_service_drowlis.value_condicion = ""
                        config_service_drowlis.value_default = "COLOMBIA"
                        config_service_drowlis.name_campo_primary = "Id_Pais_Radicacion"
                        config_service_drowlis.addd_seleccion = 1
                        config_service_drowlis.value_seleccion = ""
                        parameter_gestion.drow_name_controls_destino = "Departemento"
                        config_service_drowlis.campo_estado_auto_lista = 1
                        parameter_gestion.config_service_drowlis_destino = New List(Of Class_config_general_service.Class_config_general_service_drowlist)
                        parameter_gestion.config_service_drowlis_destino.Add(config_service_drowlis)
                    End If
                    If homologa_tipo = 0 And parameter_gestion.name_campo = "Departemento" Then
                        Dim config_service_drowlis As Class_config_general_service.Class_config_general_service_drowlist = New Class_config_general_service.Class_config_general_service_drowlist()
                        config_service_drowlis.limit_rows = "3000"
                        config_service_drowlis.name_campo_condicion = "Pais_Radicacion_Id_Pais_Radicacion"
                        config_service_drowlis.name_campo_orden = "Nombre_Departamento"
                        config_service_drowlis.name_campo_value = "Nombre_Departamento"
                        config_service_drowlis.name_dbs_auto = "DA"
                        config_service_drowlis.name_table_auto = "depart_radicacion"
                        config_service_drowlis.tipo_orden = "asc"
                        config_service_drowlis.value_condicion = "-1"
                        config_service_drowlis.value_default = ""
                        config_service_drowlis.addd_seleccion = 1
                        config_service_drowlis.value_seleccion = ""
                        config_service_drowlis.campo_estado_auto_lista = 0
                        config_service_drowlis.name_campo_primary = "Id_Depart_Radicacion"
                        parameter_gestion.drow_name_controls_destino = "Municipio"
                        parameter_gestion.drow_name_padre_control = "Pais"
                        parameter_gestion.config_service_drowlis_destino = New List(Of Class_config_general_service.Class_config_general_service_drowlist)
                        parameter_gestion.config_service_drowlis_destino.Add(config_service_drowlis)
                    End If
                    If homologa_tipo = 0 And parameter_gestion.name_campo = "Municipio" Then
                        Dim config_service_drowlis As Class_config_general_service.Class_config_general_service_drowlist = New Class_config_general_service.Class_config_general_service_drowlist()
                        config_service_drowlis.limit_rows = "3000"
                        config_service_drowlis.name_campo_condicion = "Depart_Radicacion_Id_Depart_Radicacion"
                        config_service_drowlis.name_campo_orden = "Nombre_Municipio"
                        config_service_drowlis.name_campo_value = "Nombre_Municipio"
                        config_service_drowlis.name_dbs_auto = "DA"
                        config_service_drowlis.name_table_auto = "municipio_radicacion"
                        config_service_drowlis.tipo_orden = "asc"
                        config_service_drowlis.value_condicion = "-1"
                        config_service_drowlis.value_default = ""
                        config_service_drowlis.addd_seleccion = 0
                        config_service_drowlis.value_seleccion = ""
                        config_service_drowlis.campo_estado_auto_lista = 0
                        parameter_gestion.drow_name_controls_destino = ""
                        parameter_gestion.drow_name_padre_control = "Pais"
                        config_service_drowlis.name_campo_primary = "id_Municipio_Radicacion"
                        parameter_gestion.config_service_drowlis_destino = New List(Of Class_config_general_service.Class_config_general_service_drowlist)
                        parameter_gestion.config_service_drowlis_destino.Add(config_service_drowlis)
                    End If
                    parameter_gestion.tbl_control = nombre_plantilla
                    parameter_gestion.error_gestion = "YES"
                    parameter_gestion.disable_campo = 1
                    parameter_gestion.dbms_control = "DA"
                    parameter_gestion.clas_service_control = "WebService_control_general.asmx"
                    parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
                    Class_config_general_service.Add(parameter_gestion)
                Next
                Solicita_estructura_plantilla_validacion_externos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_plantilla_validacion_externos = "Inconsistencia general funcion Solicita_estructura_plantilla_validacion_externos " & ex.Message
        End Try
    End Function
    Function Asigna_datos_validacion_campos_radicacion(ByVal id_script As Integer,
                                                       ByVal campo_radicacion As String,
                                                       ByVal id_plantilla_radicacion As Integer,
                                                       ByVal id_registro_identificador As Integer,
                                                       ByRef Campos_Plantilla() As Campos_Plantilla) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asgina datos de validación des una plantilla de validación
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_script                 : Representa la identificación del script de validación
        'campo_radicacion          : Representa el nombre del campo de radicación destino
        'id_plantilla_radicacion   : Representa la idnetiifcación de la plantilla de racicación
        'id_registro_identificador : Representa la identificación del registro en la plantilla validacion
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Campos_Plantilla          : Retorna la estrucutura con el campo validación
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-10-29
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try

            Dim campo_validacion As String = ""
            Dim nombre_plantilla_validacion As String = ""
            Dim Result As String = ""
            '-----/////Retorna  plantilla validacion 
            Result = Retorna_Nombre_Plantilla_Validacion(id_script,
                                                         nombre_plantilla_validacion)
            If Result <> "YES" Then
                Asigna_datos_validacion_campos_radicacion = Result
                Exit Function
            End If
            '-----/////Retorna id  plantilla validacion 
            Dim id_plantilla_validacion As Integer = -1
            Result = Retorna_id_plantilla_validacion_nombre(nombre_plantilla_validacion,
                                                            id_plantilla_validacion)
            If Result <> "YES" Then
                Asigna_datos_validacion_campos_radicacion = Result
                Exit Function
            End If
            '----////Retorna campo crieterio busqueda validacion----////
            Result = Lista_Campo_criterio_busqueda_Plantilla_Validacion(id_script,
                                                                        campo_validacion)
            If Result <> "YES" Then
                Asigna_datos_validacion_campos_radicacion = Result
                Exit Function
            End If
            '----/////Lista campos destino y fuente validacion---///
            Dim matri_campo_fuente_destino() As asignacion_plantilla_validacion
            Erase matri_campo_fuente_destino
            Result = Lista_Campos_fuente_destino_validacion_dinamica_externa(id_script,
                                                                             matri_campo_fuente_destino)
            If Result <> "YES" Then
                Asigna_datos_validacion_campos_radicacion = Result
                Exit Function
            End If
            '----////Retorna campo primary key plantilla validacion---/////
            Dim nombre_campo_prinary As String = ""
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Retorna_Campo_Primary_key_plantilla_validacion(id_plantilla_validacion,
                                                                                                      nombre_campo_prinary)
            If Result <> "YES" Then
                Asigna_datos_validacion_campos_radicacion = Result
                Exit Function
            End If
            Dim val_leng As Integer = 0
            If matri_campo_fuente_destino Is Nothing Then
            Else
                val_leng = matri_campo_fuente_destino.Length
            End If
            ReDim Preserve matri_campo_fuente_destino(val_leng)
            Dim Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
            matri_campo_fuente_destino(val_leng).Nombre_Campo_Fuente_Pla_Validacion = campo_validacion
            matri_campo_fuente_destino(val_leng).Nombre_Campo_Destino_Pla_Radicacion = campo_radicacion
            If campo_radicacion = "REMITENTE_COR" Or campo_radicacion = "DESTINATARIO_COR" Then
                matri_campo_fuente_destino(val_leng).Tipo_Campo_Destino_Pla_Radciacion = "VARCHAR"
            Else
                Result = Class_ra_detalle_plantilla_radicado.Retorna_Tipo_Campo_Plantilla_Radicacion(id_plantilla_radicacion,
                                                                                                     matri_campo_fuente_destino(val_leng).Nombre_Campo_Destino_Pla_Radicacion,
                                                                                                     matri_campo_fuente_destino(val_leng).Tipo_Campo_Destino_Pla_Radciacion)
                If Result <> "YES" Then
                    Asigna_datos_validacion_campos_radicacion = Result
                    Exit Function
                End If
            End If
            Result = Asigna_Datos_Fuente_Destino_Plantilla_Validacion(id_script,
                                                                      matri_campo_fuente_destino,
                                                                      nombre_plantilla_validacion,
                                                                      nombre_campo_prinary,
                                                                      id_registro_identificador)
            If Result <> "YES" Then
                Asigna_datos_validacion_campos_radicacion = Result
                Exit Function
            End If
            Dim ClassGestionFechas As New ClassGestionFechas
            '------////Asignar datos interface radicacion-----/////
            For i As Integer = 0 To matri_campo_fuente_destino.Length - 1
                For i2 As Integer = 0 To Campos_Plantilla.Length - 1
                    If Campos_Plantilla(i2).Campo_Plantilla = matri_campo_fuente_destino(i).Nombre_Campo_Destino_Pla_Radicacion Then
                        '----///Valida campo obligatorio
                        If matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion = "" And Campos_Plantilla(i2).Campo_Obligatorio = 1 Then
                            Asigna_datos_validacion_campos_radicacion = "El campo (" & Campos_Plantilla(i2).Campo_Plantilla & ") es obligatorio en la plantilla de radicacion, " &
                            "debe ser informado en la plnatilla de validación (" & nombre_plantilla_validacion & ") el campo (" & matri_campo_fuente_destino(i).Nombre_Campo_Fuente_Pla_Validacion & ") " &
                            " con el identificador (" & id_registro_identificador & ") "
                            Exit Function
                        End If
                        Select Case matri_campo_fuente_destino(i).Tipo_Campo_Destino_Pla_Radciacion
                            Case "VARCHAR"
                                Campos_Plantilla(i2).TEXTO_CAMPO = Left(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, Campos_Plantilla(i2).tam_campo)
                                Campos_Plantilla(i2).VALUE_CAMPO = Left(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, Campos_Plantilla(i2).tam_campo)
                            Case "INT"
                                Campos_Plantilla(i2).TEXTO_CAMPO = Left(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, 9)
                                Campos_Plantilla(i2).VALUE_CAMPO = Left(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, 9)
                            Case "Date"
                                ClassGestionFechas.Formatea_fecha_date_base_mysql(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, Campos_Plantilla(i2).TEXTO_CAMPO)
                                ClassGestionFechas.Formatea_fecha_date_base_mysql(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, Campos_Plantilla(i2).VALUE_CAMPO)
                            Case "DATETIME"
                                ClassGestionFechas.Formatea_fecha_time_base_mysql(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, Campos_Plantilla(i2).TEXTO_CAMPO)
                                ClassGestionFechas.Formatea_fecha_time_base_mysql(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, Campos_Plantilla(i2).VALUE_CAMPO)
                            Case Else
                                Campos_Plantilla(i2).TEXTO_CAMPO = Left(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, 30)
                                Campos_Plantilla(i2).VALUE_CAMPO = Left(matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion, 30)
                        End Select
                    End If
                Next
            Next
            Asigna_datos_validacion_campos_radicacion = "YES"
            Exit Function
        Catch ex As Exception
            Asigna_datos_validacion_campos_radicacion = "Inconsistencia general función Asigna_datos_validacion_campos_radicacion " & ex.Message
        End Try
    End Function
    Function Asigna_Datos_Fuente_Destino_Plantilla_Validacion(ByVal id_script As Integer,
                                                              ByRef matri() As asignacion_plantilla_validacion,
                                                              ByVal nombre_plantilla_validacion As String,
                                                              ByVal nombre_campo_prinary As String,
                                                              ByVal id_registro As Integer) As String

        '**************************************************************************************
        'Funcion : asigna datos validacion a los campos validados desde la tabla validacion
        'Fecha : 2014-08-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************************************************
        Try
            '----------------------------------------------------------
            'Asigna campos de seleccion
            '----------------------------------------------------------
            Dim campos_selccion As String = "Select "
            For i As Integer = 0 To matri.Length - 1
                campos_selccion = "Select " & matri(i).Nombre_Campo_Fuente_Pla_Validacion
                Dim condicion_cosnulta As String = " from " & nombre_plantilla_validacion & " where " & nombre_campo_prinary &
                    "='" & id_registro & "'"
                Dim sqlconsulta As String = campos_selccion & condicion_cosnulta
                Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
                Dim Dat_reader As New DataSet
                Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(sqlconsulta, Dat_reader)
                If result <> "YES" Then
                    Asigna_Datos_Fuente_Destino_Plantilla_Validacion = " Error asignando datos campos validacion   " & result
                    Exit Function
                End If
                If Dat_reader.Tables(0).Rows.Count > 0 Then
                    If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                        matri(i).Valor_Campo_Destino_Pla_Radicacion = ""
                    Else
                        matri(i).Valor_Campo_Destino_Pla_Radicacion = Dat_reader.Tables(0).Rows(0).Item(0)
                    End If
                End If
            Next
            Asigna_Datos_Fuente_Destino_Plantilla_Validacion = "YES"
        Catch ex As Exception
            Asigna_Datos_Fuente_Destino_Plantilla_Validacion = "Funcion Asigna_Datos_Fuente_Destino_Plantilla_Validacion " & ex.Message
        End Try
    End Function
    Function Lista_Campos_fuente_destino_validacion_dinamica_externa(ByVal id_script As Integer,
                                                                     ByRef matri() As asignacion_plantilla_validacion) As String
        '********************************************************************
        'Funcion : Lista campos fuente y destino de validacion
        'Fecha : 2014-08-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '********************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select Campo_Fuente_Plan_Valid,Tipo_Campo_Fuente_Plan_valid,Campo_Destino_Plan_Radic,Tipo_Campo_Destino_Plan_Radic from rel_campos_val_radic where " &
            " Script_actividades_Id_Script=" & id_script
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Lista_Campos_fuente_destino_validacion_dinamica_externa = " Error Listando campos fuente y destino validacion   " & result
                Exit Function
            End If
            Dim contador As Integer = 0
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    ReDim Preserve matri(contador)
                    matri(contador).Nombre_Campo_Fuente_Pla_Validacion = Dat_reader.Tables(0).Rows(i).Item(0)
                    matri(contador).Tipo_Campo_Fuente_Pla_Validacion = Dat_reader.Tables(0).Rows(i).Item(1)
                    matri(contador).Nombre_Campo_Destino_Pla_Radicacion = Dat_reader.Tables(0).Rows(i).Item(2)
                    matri(contador).Tipo_Campo_Destino_Pla_Radciacion = Dat_reader.Tables(0).Rows(i).Item(3)
                    matri(contador).Valor_Campo_Fuente_Pla_Validacion = ""
                    matri(contador).Valor_Campo_Destino_Pla_Radicacion = ""
                    contador = contador + 1
                Next
                Lista_Campos_fuente_destino_validacion_dinamica_externa = "YES"
                Exit Function
            Else
                Lista_Campos_fuente_destino_validacion_dinamica_externa = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_Campos_fuente_destino_validacion_dinamica_externa = "Funcion Lista_Campos_fuente_destino_validacion_dinamica_externa " & ex.Message
        End Try
    End Function
    Function Lista_Campo_criterio_busqueda_Plantilla_Validacion(ByVal ID_SCRIPT As Integer, ByRef campo As String) As String
        '****************************************************
        'Funcion : Lista el campo criterio de busqueda
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2014-08-12
        '****************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "SELECT  rsp.campo_busqueda_plantilla" &
                    " FROM ra_script_actividades as sa " &
                    " inner join relacion_script_plantilla as rsp on  " &
                    " (rsp.script_actividades_id_Script=sa.id_script) " &
                    " where id_script=" & ID_SCRIPT
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Lista_Campo_criterio_busqueda_Plantilla_Validacion = " Error Listando opciones plantilla radicacion   " & result
                Exit Function
            End If

            If Dat_reader.Tables(0).Rows.Count > 0 Then
                campo = Dat_reader.Tables(0).Rows(0).Item(0)
                Lista_Campo_criterio_busqueda_Plantilla_Validacion = "YES"
                Exit Function
            Else
                Lista_Campo_criterio_busqueda_Plantilla_Validacion = "Imposible encontrar campo criterio  "
                Exit Function
            End If
        Catch ex As Exception
            Lista_Campo_criterio_busqueda_Plantilla_Validacion = "Inconsistencia Lista_Campo_criterio_busqueda_Plantilla_Validacion " & ex.Message
        End Try
    End Function
    Function Retorna_Campo_Validacion_nombre_plantilla_validacion(ByVal id_script As Integer,
                                                                  ByRef nombre_plantilla_validacion As String,
                                                                  ByRef campo_plantilla_validacion As String) As String
        '**********************************************************************
        'Función : Lista el nombre del campo de validación y el nombre de la
        'plantilla de validación
        'Fecha : 2014-08-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '**********************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select NOMBRE_CAMPO,NOMBRE_PLANTILLA from ra_script_actividades where " &
            " Id_Script=" & id_script
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Retorna_Campo_Validacion_nombre_plantilla_validacion = " Error Listando campo validacion y nombre plantilla   " & result
                Exit Function
            End If
            Dim contador As Integer = 0
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                campo_plantilla_validacion = Dat_reader.Tables(0).Rows(0).Item(0)
                nombre_plantilla_validacion = Dat_reader.Tables(0).Rows(0).Item(1)
                Retorna_Campo_Validacion_nombre_plantilla_validacion = "YES"
                Exit Function
            Else
                Retorna_Campo_Validacion_nombre_plantilla_validacion = "Imposible encontrar campo validacion y nombre plantilla "
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Campo_Validacion_nombre_plantilla_validacion = "Inconsistencia general funcion Retorna_Campo_Validacion_nombre_plantilla_validacion " & ex.Message
        End Try
    End Function
    Function Retorna_id_Plantilla_Validacion_id_script(ByVal Id_script_validacion As Integer,
                                                       ByRef id_Plantilla As Integer) As String
        '*****************************************************************
        'Funcion : Retorna id plantilla validacion con el id de la
        'del script
        'Fecha : 2014-07-28
        'Ing : Miguel Angel Urueta Miranda
        '*****************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "SELECT pv.Id_Plantilla_Validacion FROM relacion_script_plantilla as rsp INNER JOIN PLANTILLA_VALIDACION as pv on " &
            " (pv.Id_Plantilla_Validacion=rsp.Plantilla_Validacion_Id_Plantilla_Validacion) where rsp.script_actividades_id_script=" & Id_script_validacion
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_Plantilla_Validacion_id_script = " Error funcion Retorna_id_Plantilla_Validacion_id_script  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_Plantilla_Validacion_id_script = "Imposible encontrar el id de la plantilla alidación con el id script (" & Id_script_validacion & ")"
                Exit Function
            Else
                id_Plantilla = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_Plantilla_Validacion_id_script = "YES"
                Exit Function
            End If
        Catch ex As Exception
            id_Plantilla = "Inconsistencia general funcion id_Plantilla " & ex.Message
        End Try
    End Function
    Function Retorna_id_plantilla_validacion_nombre(ByVal nombre_plantilla As String,
                                                    ByRef id_plantilla As Integer) As String
        '**************************************************************************************
        'Funcion : Retorna id plantilla de validacion con el paramentro nombre plantilla que
        'es unico en la base de datos
        'Fecha : 2014-08-04
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Id_Plantilla_Validacion from plantilla_validacion " &
                " where Nombre_Plantilla='" & nombre_plantilla & "'"
            Dim Dat_reader As New DataSet
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_id_plantilla_validacion_nombre = " Error funcion Retorna_id_plantilla_validacion_nombre   " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Retorna_id_plantilla_validacion_nombre = "Imposible encontrar el id de la plantilla (" & nombre_plantilla & ")"
                Exit Function
            Else
                id_plantilla = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_id_plantilla_validacion_nombre = "YES"
            End If
        Catch ex As Exception
            Retorna_id_plantilla_validacion_nombre = "Inconsistencia funcion Retorna_id_plantilla_validacion_nombre " & ex.Message
        End Try
    End Function
    Function Asigna_datos_campos_plantilla_validacion_externos(ByVal id_registro As Integer,
                                                               ByVal nombre_plantilla As String,
                                                               ByVal campo_primary As String,
                                                               ByRef Class_config_general_service As List(Of Class_config_general_service)) As String
        '---------------------------------------------------------------------------
        'Funcion : Asigna datos de la plantilla de validación a la clase 
        '          Class_config_general_service
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro    : Representa el identificador de registro 
        '                        
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service : Retorna la estructura general de los campos
        '                               para la estructura del formulario y los 
        '                               valores.
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-01-02
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim campo_idex As String = ""
            Dim Result As String = ""
            Class_config_general_service.Item(0).name_campo_id = campo_primary
            Dim sql_consulta As String = "Select * from " & nombre_plantilla & " where " & campo_primary & "='" & id_registro & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Asigna_datos_campos_plantilla_validacion_externos = "Error (" & Result & ") listando en la plantilla (" & nombre_plantilla & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    For z As Integer = 0 To Class_config_general_service.Count - 1
                        Class_config_general_service.Item(z).dms_id_registro = id_registro
                        Class_config_general_service.Item(z).tbl_control = nombre_plantilla
                        If Class_config_general_service.Item(z).name_campo = Datset.Tables(0).Columns(i).ColumnName Then
                            If Datset.Tables(0).Rows(0).IsNull(i) = True Then
                                Class_config_general_service.Item(z).texto_campo = ""
                                Class_config_general_service.Item(z).value_campo = ""
                                Class_config_general_service.Item(z).value_campo_old = ""

                            Else
                                Class_config_general_service.Item(z).texto_campo = Datset.Tables(0).Rows(0).Item(i)
                                Class_config_general_service.Item(z).value_campo = Datset.Tables(0).Rows(0).Item(i)
                                Class_config_general_service.Item(z).value_campo_old = Datset.Tables(0).Rows(0).Item(i)
                            End If

                        End If
                    Next
                Next
                Asigna_datos_campos_plantilla_validacion_externos = "YES"
                Exit Function
            Else
                Asigna_datos_campos_plantilla_validacion_externos = "Imposible encontrar datos para listar en la plantilla (" & nombre_plantilla & ")"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_datos_campos_plantilla_validacion_externos = "Inconsistencia general funcion Asigna_datos_campos_plantilla_validacion_externos (" & ex.Message & ")"
        End Try
    End Function
    Function Update_tercero_plantilla_validacion(ByVal id_script_plantilla_validacion As Integer,
                                                 ByRef Ilist_Class_config_general_service As List(Of Class_config_general_service)) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra el tercero o destinatario externo para radicación
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_script_plantilla_validacion : Representa la identificación del script
        'de la plantilla de validación
        'Ilist_Class_config_general_service : Representa la estructura de los campos
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Ilist_Class_config_general_service  : Retorna la estructura de los campos
        'para agregar el registro en la tabla
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-01-03
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Class_config_general_service As New Class_config_general_service
            Dim sql_update As String = ""
            Dim Result As String = ""
            Dim nombre_plantillas As String = ""
            nombre_plantillas = Ilist_Class_config_general_service(0).tbl_control
            '---------------------------------------------------------------------------------
            'Verifica existencia de los campos unicos
            '---------------------------------------------------------------------------------
            Dim Campos_plantilla_validacion() As CAMPOS_PLANTILLA_VALIDACION = Nothing
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Lista_Campos_Plantilla_Validacion(id_script_plantilla_validacion,
                                                                                         Campos_plantilla_validacion)
            If Result <> "YES" Then
                Update_tercero_plantilla_validacion = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Asigna los valores del campo a la estructura
            '---------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                For k As Integer = 0 To Ilist_Class_config_general_service.Count - 1
                    If Campos_plantilla_validacion(i).Nombre_Campo = Ilist_Class_config_general_service.Item(k).name_campo Then
                        If Ilist_Class_config_general_service.Item(k).texto_campo <> Ilist_Class_config_general_service.Item(k).value_campo_old Then
                            Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO = Ilist_Class_config_general_service.Item(k).texto_campo
                        Else
                            Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO = ""
                        End If
                        Campos_plantilla_validacion(i).TEXTO_CAMPO = Ilist_Class_config_general_service.Item(k).texto_campo
                    End If
                Next
            Next
            '---------------------------------------------------------------------------------
            'Valida campos obligatorios vacios
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 And Campos_plantilla_validacion(i).Obligatorio_Campo = 1 Then
                    If Campos_plantilla_validacion(i).TEXTO_CAMPO = "" Then
                        Dim ref_campo As String = ""
                        If Campos_plantilla_validacion(i).Aleas_Campo_pqr = "" Then
                            ref_campo = Campos_plantilla_validacion(i).Nombre_Campo
                        Else
                            ref_campo = Campos_plantilla_validacion(i).Aleas_Campo_pqr
                        End If
                        Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                            Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                            Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                        Update_tercero_plantilla_validacion = "El campo " & ref_campo & " es obligatorio"
                        Exit Function
                    End If
                End If
            Next
            '---------------------------------------------------------------------------------
            'Verifica existencia de los campos unicos
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 And Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                    If Campos_plantilla_validacion(i).Unico_campo = 1 Then
                        Dim sql As String = "Select " & Campos_plantilla_validacion(i).Nombre_Campo & " from " & nombre_plantillas &
                            " where " & Campos_plantilla_validacion(i).Nombre_Campo & "='" & Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO & "'"
                        Result = Me.Verifica_Existencia_Campos_Unico_Validacion(sql,
                                                                                Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO,
                                                                                Campos_plantilla_validacion(i).Aleas_Campo_pqr)
                        If Result <> "YES" Then
                            Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                        Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                                        Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                            Update_tercero_plantilla_validacion = Result
                            Exit Function
                        End If
                    End If
                End If
            Next
            Result = Class_config_general_service.Create_update_form_control_auto(Ilist_Class_config_general_service,
                                                                                  sql_update)
            If Result <> "YES" Then
                Update_tercero_plantilla_validacion = Result
                Exit Function
            End If
            If sql_update = "" Then
                Update_tercero_plantilla_validacion = "No se detectaron cambios en el formulario"
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Update_tercero_plantilla_validacion = Result
                Exit Function
            End If
            Update_tercero_plantilla_validacion = "YES"
        Catch ex As Exception
            Update_tercero_plantilla_validacion = "Inconsistencia funcion Update_tercero_plantilla_validacion Error (" & ex.Message & ")"
        End Try
    End Function
    Function Delete_tercero_pantilla_validacion(ByVal id_dext_ext As Integer,
                                                ByVal id_script_plantilla_validacion As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Elimina el tercero o destinatario externo para radicación
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_script_plantilla_validacion : Representa la identificación del script
        'de la plantilla de validación
        'id_dext_ext
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-01-10
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            '------------------------------------------------------------------------
            'Verifica la existencia de guias relacionadas con destinatarios externos
            '------------------------------------------------------------------------
            Dim refclasenvio As New ClassRaEnvioCorrespondencia
            Dim estado_existencia As String = "YES"
            Result = refclasenvio.Verifica_existencia_destina_ext_guia(id_dext_ext,
                                                                       estado_existencia)
            If Result <> "YES" Then
                Delete_tercero_pantilla_validacion = Result
                Exit Function
            End If
            If estado_existencia = "YES" Then
                Delete_tercero_pantilla_validacion = "El destinatario externo tiene quías de envío registradas imposible eliminar"
                Exit Function
            End If
            '---------------------------------------------
            'Verifica la existencia de registros
            '---------------------------------------------
            Dim classradicado As New ClassRadicador
            Dim plantilla_radic() As String = Nothing
            Result = classradicado.Retorna_plantillas_relacionadas_script(id_script_plantilla_validacion,
                                                                          plantilla_radic)
            If Result <> "YES" Then
                Delete_tercero_pantilla_validacion = Result
                Exit Function
            End If
            Dim Resultado As String = ""
            Result = classradicado.Retorna_registros_relacion_validacion_plantillas(plantilla_radic,
                                                                                    Resultado,
                                                                                    id_dext_ext)
            If Result <> "YES" Then
                Delete_tercero_pantilla_validacion = Result
                Exit Function
            End If
            Dim plantillas_valid As String = ""
            If Not plantilla_radic Is Nothing Then
                For i As Integer = 0 To plantilla_radic.Length - 1
                    plantillas_valid = plantillas_valid & plantilla_radic(i) & ","
                Next
            End If
            If Resultado = "YES" Then
                Delete_tercero_pantilla_validacion = "Imposible eliminar el registro, existen valores relacionados en las plantillas de validación (" & plantillas_valid & ")"
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna nombre plantilla validacion
            '---------------------------------------------------
            Dim nombre_plantilla As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script_plantilla_validacion,
                                                                                    nombre_plantilla)
            If Result <> "YES" Then
                Delete_tercero_pantilla_validacion = Result
                Exit Function
            End If
            Dim id_plantilla_validacion As Integer = -1
            Result = Retorna_id_Plantilla_Validacion_id_script(id_script_plantilla_validacion,
                                                               id_plantilla_validacion)
            If Result <> "YES" Then
                Delete_tercero_pantilla_validacion = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Retorna nombre campo primary key
            '-----------------------------------------------------
            Dim nombre_campo_key As String = ""
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Retorna_Campo_Primary_key_plantilla_validacion(id_plantilla_validacion,
                                                                                                      nombre_campo_key)
            If Result <> "YES" Then
                Delete_tercero_pantilla_validacion = Result
                Exit Function
            End If
            '----------------------------------------------------
            'Verfica relacion plantilla validacion vs radicacion
            '----------------------------------------------------
            Result = classradicado.Verifca_Existencia_Relacion_pla_validacion_pla_radicacion(id_plantilla_validacion,
                                                                                             id_dext_ext)
            If Result <> "YES" Then
                Delete_tercero_pantilla_validacion = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Elimina registro plantilla
            '-----------------------------------------------------
            Dim sql As String = "delete from " & nombre_plantilla & " where " & nombre_campo_key & "='" & id_dext_ext & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql)
            If Result <> "YES" Then
                Delete_tercero_pantilla_validacion = Result
                Exit Function
            End If
            Delete_tercero_pantilla_validacion = "YES"
        Catch ex As Exception
            Delete_tercero_pantilla_validacion = "inconsistencia generaal function Delete_tercero_pantilla_validacion (" & ex.Message & ")"
        End Try
    End Function
    Function Update_tercero_plantilla_validacion_simplificada(ByVal id_script_plantilla_validacion As Integer,
                                                              ByVal Ilist_Class_config_general_service As List(Of Class_config_general_service),
                                                              ByRef class_config_gneral_service_row_option_tom_select As List(Of class_config_gneral_service_row_option_tom_select)) As String
        '---------------------------------------------------------------------------
        'Funcion : Actualiza el tercero o destinatario externo para radicación
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_script_plantilla_validacion : Representa la identificación del script
        'de la plantilla de validación
        'Ilist_Class_config_general_service : Representa la estructura de los campos
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Ilist_Class_config_general_service  : Retorna la estructura de los campos
        'para agregar el registro en la tabla
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-01-03
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Class_config_general_service As New Class_config_general_service
            Dim sql_update As String = ""
            Dim Result As String = ""
            Dim nombre_plantillas As String = ""
            nombre_plantillas = Ilist_Class_config_general_service(0).tbl_control
            '---------------------------------------------------------------------------------
            'Verifica existencia de los campos unicos
            '---------------------------------------------------------------------------------
            Dim Campos_plantilla_validacion() As CAMPOS_PLANTILLA_VALIDACION = Nothing
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Lista_Campos_Plantilla_Validacion(id_script_plantilla_validacion,
                                                                                         Campos_plantilla_validacion)
            If Result <> "YES" Then
                Update_tercero_plantilla_validacion_simplificada = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Asigna los valores del campo a la estructura
            '---------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                For k As Integer = 0 To Ilist_Class_config_general_service.Count - 1
                    If Campos_plantilla_validacion(i).Nombre_Campo = Ilist_Class_config_general_service.Item(k).name_campo Then
                        If Ilist_Class_config_general_service.Item(k).texto_campo <> Ilist_Class_config_general_service.Item(k).value_campo_old Then
                            Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO = Ilist_Class_config_general_service.Item(k).texto_campo
                        Else
                            Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO = ""
                        End If
                        Campos_plantilla_validacion(i).TEXTO_CAMPO = Ilist_Class_config_general_service.Item(k).texto_campo
                    End If
                Next
            Next
            '---------------------------------------------------------------------------------
            'Valida campos obligatorios vacios
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 And Campos_plantilla_validacion(i).Obligatorio_Campo = 1 Then
                    If Campos_plantilla_validacion(i).TEXTO_CAMPO = "" Then
                        Dim ref_campo As String = ""
                        If Campos_plantilla_validacion(i).Aleas_Campo_pqr = "" Then
                            ref_campo = Campos_plantilla_validacion(i).Nombre_Campo
                        Else
                            ref_campo = Campos_plantilla_validacion(i).Aleas_Campo_pqr
                        End If
                        Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                            Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                            Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                        Update_tercero_plantilla_validacion_simplificada = "El campo " & ref_campo & " es obligatorio"
                        Exit Function
                    End If
                End If
            Next
            '---------------------------------------------------------------------------------
            'Verifica existencia de los campos unicos
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 And Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                    If Campos_plantilla_validacion(i).Unico_campo = 1 Then
                        Dim sql As String = "Select " & Campos_plantilla_validacion(i).Nombre_Campo & " from " & nombre_plantillas &
                            " where " & Campos_plantilla_validacion(i).Nombre_Campo & "='" & Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO & "'"
                        Result = Me.Verifica_Existencia_Campos_Unico_Validacion(sql,
                                                                                Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO,
                                                                                Campos_plantilla_validacion(i).Aleas_Campo_pqr)
                        If Result <> "YES" Then
                            Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                        Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                                        Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                            Update_tercero_plantilla_validacion_simplificada = Result
                            Exit Function
                        End If
                    End If
                End If
            Next
            Result = Class_config_general_service.Create_update_form_control_auto(Ilist_Class_config_general_service,
                                                                                  sql_update)
            If Result <> "YES" Then
                Update_tercero_plantilla_validacion_simplificada = Result
                Exit Function
            End If
            If sql_update = "" Then
                Update_tercero_plantilla_validacion_simplificada = "No se detectaron cambios en el formulario"
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Update_tercero_plantilla_validacion_simplificada = Result
                Exit Function
            End If
            '-----------------------------------------------------
            '----//Asigna los valores del TOM SELCT
            '-----------------------------------------------------
            Dim item_Tom_Select As New class_config_gneral_service_row_option_tom_select
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Campo_nombre_pqr = 1 Then
                    If Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                        item_Tom_Select.tex_value = Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO
                    Else
                        item_Tom_Select.tex_value = Campos_plantilla_validacion(i).TEXTO_CAMPO
                    End If
                End If
                If Campos_plantilla_validacion(i).IDENTI_CAMPO = 1 Then
                    item_Tom_Select.id_value = Ilist_Class_config_general_service(0).dms_id_registro
                End If
            Next
            class_config_gneral_service_row_option_tom_select.Add(item_Tom_Select)
            Update_tercero_plantilla_validacion_simplificada = "YES"
            Exit Function
        Catch ex As Exception
            Update_tercero_plantilla_validacion_simplificada = "Inconsistencia funcion Update_tercero_plantilla_validacion_simplificada Error (" & ex.Message & ")"
        End Try
    End Function
    Function Registra_tercero_plantilla_validacion_simpificada(ByVal id_script_plantilla_validacion As Integer,
                                                               ByVal Ilist_Class_config_general_service As List(Of Class_config_general_service),
                                                               ByRef class_config_gneral_service_row_option_tom_select As List(Of class_config_gneral_service_row_option_tom_select)) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra el tercero o destinatario externo para radicación
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_script_plantilla_validacion : Representa la identificación del script
        'de la plantilla de validación
        'Ilist_Class_config_general_service : Representa la plantilla de configuración
        'general con los datos del formulario
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_config_gneral_service_row_option_tom_select  : Retorna la estructura
        'de los campos para agregar el registro en el selctor TOM SELECT
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""

            Dim Campos_plantilla_validacion() As CAMPOS_PLANTILLA_VALIDACION = Nothing
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Lista_Campos_Plantilla_Validacion(id_script_plantilla_validacion,
                                                                                         Campos_plantilla_validacion)
            If Result <> "YES" Then
                Registra_tercero_plantilla_validacion_simpificada = Result
                Exit Function
            End If
            '-------------------------------------------------------
            'Identifica nombre del campo identi en la matriz campos
            '-------------------------------------------------------
            Dim campo_idex As String = ""
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).IDENTI_CAMPO = 1 Then
                    campo_idex = Campos_plantilla_validacion(i).Nombre_Campo
                End If
            Next
            If campo_idex = "" Then
                Registra_tercero_plantilla_validacion_simpificada = "La plantilla carece de campo identiti por favor agregelo en el modulo administración"
                Exit Function
            End If

            '---------------------------------------------------
            'Retorna nombre plantilla validacion
            '---------------------------------------------------
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script_plantilla_validacion,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Registra_tercero_plantilla_validacion_simpificada = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Asigna los valores del campo a la estructura
            '---------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                For k As Integer = 0 To Ilist_Class_config_general_service.Count - 1
                    If Campos_plantilla_validacion(i).Nombre_Campo = Ilist_Class_config_general_service.Item(k).name_campo Then
                        Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO = Ilist_Class_config_general_service.Item(k).texto_campo
                    End If
                Next
            Next

            '---------------------------------------------------------------------------------
            'Valida los formatos fecha
            '---------------------------------------------------------------------------------
            Dim Class_config_general_service As New Class_config_general_service
            Dim ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 Then
                    If Campos_plantilla_validacion(i).Tipo_Campo = "DATE" And Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                        Result = ClassGestionFechas.Verifi_campo_fecha(Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO)
                        If Result <> "YES" Then
                            Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                                Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                            Registra_tercero_plantilla_validacion_simpificada = "El formato fecha no cumple " & Result
                            Exit Function
                        End If
                    End If
                End If
            Next
            '---------------------------------------------------------------------------------
            'Valida campos obligatorios vacios
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 And Campos_plantilla_validacion(i).Obligatorio_Campo = 1 Then
                    If Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO = "" Then
                        Dim ref_campo As String = ""
                        If Campos_plantilla_validacion(i).Aleas_Campo_pqr = "" Then
                            ref_campo = Campos_plantilla_validacion(i).Nombre_Campo
                        Else
                            ref_campo = Campos_plantilla_validacion(i).Aleas_Campo_pqr
                        End If
                        Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                            Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                            Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                        Registra_tercero_plantilla_validacion_simpificada = "El campo " & ref_campo & " es obligatorio"
                        Exit Function
                    End If
                End If
            Next
            '---------------------------------------------------------------------------------
            'Verifica existencia de los campos unicos
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 And Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                    If Campos_plantilla_validacion(i).Unico_campo = 1 Then
                        Dim sql As String = "Select " & Campos_plantilla_validacion(i).Nombre_Campo & " from " & nombre_plantillas &
                            " where " & Campos_plantilla_validacion(i).Nombre_Campo & "='" & Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO & "'"
                        Result = Me.Verifica_Existencia_Campos_Unico_Validacion(sql,
                                                                                Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO,
                                                                                Campos_plantilla_validacion(i).Aleas_Campo_pqr)
                        If Result <> "YES" Then
                            Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                                Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                            Registra_tercero_plantilla_validacion_simpificada = Result
                            Exit Function
                        End If
                    End If

                End If
            Next
            '---------------------------------------------------------------------------- -----
            'Genera comnando de insertcion
            '---------------------------------------------------------------------------------
            Dim sqlactualizacion As String = "INSERT INTO " & nombre_plantillas & " "
            Dim sqlcampos As String = ""
            Dim sqlvalores As String = ""
            '---------------------------------------------------------------------------------
            'Lista campos de insertcion
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 Then

                    If sqlcampos = "" Then
                        sqlcampos = sqlcampos & "(" & Campos_plantilla_validacion(i).Nombre_Campo
                    Else
                        sqlcampos = sqlcampos & " , " & Campos_plantilla_validacion(i).Nombre_Campo
                    End If

                    If sqlvalores = "" Then
                        sqlvalores = sqlvalores & "(" & "'" & Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO & "'"
                    Else
                        sqlvalores = sqlvalores & " , " & "'" & Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO & "'"
                    End If
                End If
            Next
            sqlvalores = sqlvalores & ")"
            sqlcampos = sqlcampos & ")"
            sqlactualizacion = sqlactualizacion & sqlcampos & " values " & sqlvalores
            Dim lastinsert As Object = 0
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_LAST_INSERT_COMMAND(sqlactualizacion,
                                                                 lastinsert)
            If Result <> "YES" Then
                Registra_tercero_plantilla_validacion_simpificada = Result
                Exit Function
            End If
            '-----------------------------------------------------
            '----//Asigna los valores del TOM SELCT
            '-----------------------------------------------------
            Dim item_Tom_Select As New class_config_gneral_service_row_option_tom_select
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Campo_nombre_pqr = 1 Then
                    item_Tom_Select.tex_value = Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO
                End If
                If Campos_plantilla_validacion(i).IDENTI_CAMPO = 1 Then
                    item_Tom_Select.id_value = lastinsert
                End If
            Next
            class_config_gneral_service_row_option_tom_select.Add(item_Tom_Select)
            Registra_tercero_plantilla_validacion_simpificada = "YES"
            Exit Function
        Catch ex As Exception
            Registra_tercero_plantilla_validacion_simpificada = "Inconsistencia general funcion Registra_tercero_plantilla_validacion_simpificada " & ex.Message
        End Try
    End Function
    Function Registra_tercero_plantilla_validacion(ByVal id_script_plantilla_validacion As Integer,
                                                   ByRef Ilist_Class_config_general_service As List(Of Class_config_general_service)) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra el tercero o destinatario externo para radicación
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_script_plantilla_validacion : Representa la identificación del script
        'de la plantilla de validación
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Ilist_Class_config_general_service  : Retorna la estructura de los campos
        'para agregar el registro en la tabla
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-12-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Campos_plantilla_validacion() As CAMPOS_PLANTILLA_VALIDACION = Nothing
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Lista_Campos_Plantilla_Validacion(id_script_plantilla_validacion,
                                                                                         Campos_plantilla_validacion)
            If Result <> "YES" Then
                Registra_tercero_plantilla_validacion = Result
                Exit Function
            End If
            '-------------------------------------------------------
            'Identifica nombre del campo identi en la matriz campos
            '-------------------------------------------------------
            Dim campo_idex As String = ""
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).IDENTI_CAMPO = 1 Then
                    campo_idex = Campos_plantilla_validacion(i).Nombre_Campo
                End If
            Next
            If campo_idex = "" Then
                Registra_tercero_plantilla_validacion = "La plantilla carece de campo identiti por favor agregelo en el modulo administración"
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna nombre plantilla validacion
            '---------------------------------------------------
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script_plantilla_validacion,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Registra_tercero_plantilla_validacion = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Asigna los valores del campo a la estructura
            '---------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                For k As Integer = 0 To Ilist_Class_config_general_service.Count - 1
                    If Campos_plantilla_validacion(i).Nombre_Campo = Ilist_Class_config_general_service.Item(k).name_campo Then
                        Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO = Ilist_Class_config_general_service.Item(k).texto_campo
                    End If
                Next
            Next
            '---------------------------------------------------------------------------------
            'Valida los formatos fecha
            '---------------------------------------------------------------------------------
            Dim Class_config_general_service As New Class_config_general_service
            Dim ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 Then
                    If Campos_plantilla_validacion(i).Tipo_Campo = "DATE" And Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                        Result = ClassGestionFechas.Verifi_campo_fecha(Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO)
                        If Result <> "YES" Then
                            Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                                Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                            Registra_tercero_plantilla_validacion = "El formato fecha no cumple " & Result
                            Exit Function
                        End If
                    End If
                End If
            Next

            '---------------------------------------------------------------------------------
            'Valida campos obligatorios vacios
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 And Campos_plantilla_validacion(i).Obligatorio_Campo = 1 Then
                    If Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO = "" Then
                        Dim ref_campo As String = ""
                        If Campos_plantilla_validacion(i).Aleas_Campo_pqr = "" Then
                            ref_campo = Campos_plantilla_validacion(i).Nombre_Campo
                        Else
                            ref_campo = Campos_plantilla_validacion(i).Aleas_Campo_pqr
                        End If
                        Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                            Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                            Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                        Registra_tercero_plantilla_validacion = "El campo " & ref_campo & " es obligatorio"
                        Exit Function
                    End If
                End If
            Next
            '---------------------------------------------------------------------------------
            'Verifica existencia de los campos unicos
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 And Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                    If Campos_plantilla_validacion(i).Unico_campo = 1 Then
                        Dim sql As String = "Select " & Campos_plantilla_validacion(i).Nombre_Campo & " from " & nombre_plantillas &
                            " where " & Campos_plantilla_validacion(i).Nombre_Campo & "='" & Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO & "'"
                        Result = Me.Verifica_Existencia_Campos_Unico_Validacion(sql,
                                                                                Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO,
                                                                                Campos_plantilla_validacion(i).Aleas_Campo_pqr)
                        If Result <> "YES" Then
                            Class_config_general_service.add_control_error_form(Campos_plantilla_validacion(i).Nombre_Campo,
                                                                                Ilist_Class_config_general_service.Item(0).name_space_campo,
                                                                                Ilist_Class_config_general_service.Item(0).config_service_controls_error)
                            Registra_tercero_plantilla_validacion = Result
                            Exit Function
                        End If
                    End If

                End If
            Next

            '---------------------------------------------------------------------------- -----
            'Genera comnando de insertcion
            '---------------------------------------------------------------------------------
            Dim sqlactualizacion As String = "INSERT INTO " & nombre_plantillas & " "
            Dim sqlcampos As String = ""
            Dim sqlvalores As String = ""
            '---------------------------------------------------------------------------------
            'Lista campos de insertcion
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Campos_plantilla_validacion.Length - 1
                If Campos_plantilla_validacion(i).Visible_Campo = 1 And Campos_plantilla_validacion(i).IDENTI_CAMPO <> 1 Then

                    If sqlcampos = "" Then
                        sqlcampos = sqlcampos & "(" & Campos_plantilla_validacion(i).Nombre_Campo
                    Else
                        sqlcampos = sqlcampos & " , " & Campos_plantilla_validacion(i).Nombre_Campo
                    End If

                    If sqlvalores = "" Then
                        sqlvalores = sqlvalores & "(" & "'" & Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO & "'"
                    Else
                        sqlvalores = sqlvalores & " , " & "'" & Campos_plantilla_validacion(i).TEXTO_CAMPO_MODIFICADO & "'"
                    End If
                End If
            Next
            sqlvalores = sqlvalores & ")"
            sqlcampos = sqlcampos & ")"
            sqlactualizacion = sqlactualizacion & sqlcampos & " values " & sqlvalores
            Dim lastinsert As Object = 0
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_LAST_INSERT_COMMAND(sqlactualizacion,
                                                                 lastinsert)
            If Result <> "YES" Then
                Registra_tercero_plantilla_validacion = Result
                Exit Function
            End If
            '--------------------------------------------------------
            'Asigna el la identificación del registro
            '--------------------------------------------------------
            For k As Integer = 0 To Ilist_Class_config_general_service.Count - 1
                Ilist_Class_config_general_service.Item(k).dms_id_registro = lastinsert
                Ilist_Class_config_general_service.Item(k).error_gestion = "YES"
            Next

            '--------------------------------------------------------
            'Asigna el registro de los botones awasome para las
            'iteraciónes en  las tablas
            '--------------------------------------------------------
            If Ilist_Class_config_general_service.Count > 0 Then
                '--------------------------------------------
                'Registro del boton asigna registro usuario 
                '--------------------------------------------
                'Add Elment I
                Dim Item_Ilist_atrib_general As Class_config_general_service_boton_atributes_awsome = New Class_config_general_service_boton_atributes_awsome()
                Item_Ilist_atrib_general.Element_i_name_atribute_color_awsome = "white"
                Item_Ilist_atrib_general.Element_i_name_atribute_fas_awsome = "fal"
                Item_Ilist_atrib_general.Element_i_name_atribute_boton_awsome = "fa-user-check"
                Item_Ilist_atrib_general.Element_i_name_atribute_dimension_awsome = "fa-lg"
                'Add Element boton
                Item_Ilist_atrib_general.Element_a_name_atribute_color_awsome = "btn-primary"
                Item_Ilist_atrib_general.Element_a_name_atribute_fas_awsome = "btn"
                Item_Ilist_atrib_general.Element_a_name_atribute_dimension_awsome = "btn-sm"
                Item_Ilist_atrib_general.Element_a_name_atribute_onclclick_awsome = "prevent(event,this)"
                Item_Ilist_atrib_general.Element_a_name_atribute_tip_event_awsome = "asigna_registro_usuario"
                Item_Ilist_atrib_general.Element_a_name_atribute_title_awsome = "Asigna registro de usuario remitente"
                'Add atribute Element boton  
                Dim ItList_atribute = New List(Of Class_config_general_atributes_awsome)()
                Dim Item_Ilist_atribute As Class_config_general_atributes_awsome = New Class_config_general_atributes_awsome()
                Item_Ilist_atribute.name_atribute = "id"
                Item_Ilist_atribute.value_atribute = lastinsert
                Item_Ilist_atrib_general.Element_a_atribute_atributes_boton_awsome = New List(Of Class_config_general_atributes_awsome)
                Item_Ilist_atrib_general.Element_a_atribute_atributes_boton_awsome.Add(Item_Ilist_atribute)
                'Add atribute row
                ItList_atribute = New List(Of Class_config_general_atributes_awsome)()
                Item_Ilist_atribute = New Class_config_general_atributes_awsome()
                Item_Ilist_atribute.name_atribute = "id"
                Item_Ilist_atribute.value_atribute = lastinsert
                Item_Ilist_atrib_general.Element_a_atribute_atributes_row_awsome = New List(Of Class_config_general_atributes_awsome)
                Item_Ilist_atrib_general.Element_a_atribute_atributes_row_awsome.Add(Item_Ilist_atribute)
                Ilist_Class_config_general_service.Item(0).config_service_boton_atributes_awsome = New List(Of Class_config_general_service_boton_atributes_awsome)
                Ilist_Class_config_general_service.Item(0).config_service_boton_atributes_awsome.Add(Item_Ilist_atrib_general)
                ''-----------------------------------
                ''Registro del boton edita registro
                ''-----------------------------------
                'Add Element I
                Item_Ilist_atrib_general = New Class_config_general_service_boton_atributes_awsome()
                Item_Ilist_atrib_general.Element_i_name_atribute_color_awsome = "white"
                Item_Ilist_atrib_general.Element_i_name_atribute_fas_awsome = "fal"
                Item_Ilist_atrib_general.Element_i_name_atribute_boton_awsome = "fa-user-edit"
                Item_Ilist_atrib_general.Element_i_name_atribute_dimension_awsome = "fa-lg"
                'Add Element boton
                Item_Ilist_atrib_general.Element_a_name_atribute_color_awsome = "btn-danger"
                Item_Ilist_atrib_general.Element_a_name_atribute_fas_awsome = "btn"
                Item_Ilist_atrib_general.Element_a_name_atribute_dimension_awsome = "btn-sm"
                Item_Ilist_atrib_general.Element_a_name_atribute_onclclick_awsome = "prevent(event,this)"
                Item_Ilist_atrib_general.Element_a_name_atribute_tip_event_awsome = "edita_reg_usuario"
                Item_Ilist_atrib_general.Element_a_name_atribute_title_awsome = "Edita usuario remitente"
                'Add atribute Element boton  
                ItList_atribute = New List(Of Class_config_general_atributes_awsome)()
                Item_Ilist_atribute = New Class_config_general_atributes_awsome()
                Item_Ilist_atribute.name_atribute = "id"
                Item_Ilist_atribute.value_atribute = lastinsert
                Item_Ilist_atrib_general.Element_a_atribute_atributes_boton_awsome = New List(Of Class_config_general_atributes_awsome)
                Item_Ilist_atrib_general.Element_a_atribute_atributes_boton_awsome.Add(Item_Ilist_atribute)
                'Add atribute row
                ItList_atribute = New List(Of Class_config_general_atributes_awsome)()
                Item_Ilist_atribute = New Class_config_general_atributes_awsome()
                Item_Ilist_atribute.name_atribute = "id"
                Item_Ilist_atribute.value_atribute = lastinsert
                Item_Ilist_atrib_general.Element_a_atribute_atributes_row_awsome = New List(Of Class_config_general_atributes_awsome)
                Item_Ilist_atrib_general.Element_a_atribute_atributes_row_awsome.Add(Item_Ilist_atribute)
                Ilist_Class_config_general_service.Item(0).config_service_boton_atributes_awsome.Add(Item_Ilist_atrib_general) 'Agrega el boton
                ''------------------------------------
                ''Registro del boton elimina registro
                ''------------------------------------
                'Add Element I
                Item_Ilist_atrib_general = New Class_config_general_service_boton_atributes_awsome()
                Item_Ilist_atrib_general.Element_i_name_atribute_color_awsome = "white"
                Item_Ilist_atrib_general.Element_i_name_atribute_fas_awsome = "fal"
                Item_Ilist_atrib_general.Element_i_name_atribute_boton_awsome = "fa-user-times"
                Item_Ilist_atrib_general.Element_i_name_atribute_dimension_awsome = "fa-lg"
                'Add Element boton
                Item_Ilist_atrib_general.Element_a_name_atribute_color_awsome = "btn-warning"
                Item_Ilist_atrib_general.Element_a_name_atribute_fas_awsome = "btn"
                Item_Ilist_atrib_general.Element_a_name_atribute_dimension_awsome = "btn-sm"
                Item_Ilist_atrib_general.Element_a_name_atribute_onclclick_awsome = "prevent(event,this)"
                Item_Ilist_atrib_general.Element_a_name_atribute_tip_event_awsome = "elimina_reg_usuario"
                Item_Ilist_atrib_general.Element_a_name_atribute_title_awsome = "Elimina registro de usuario remitente externo"

                ItList_atribute = New List(Of Class_config_general_atributes_awsome)()
                Item_Ilist_atribute = New Class_config_general_atributes_awsome()
                Item_Ilist_atribute.name_atribute = "id"
                Item_Ilist_atribute.value_atribute = lastinsert
                Item_Ilist_atrib_general.Element_a_atribute_atributes_boton_awsome = New List(Of Class_config_general_atributes_awsome)
                Item_Ilist_atrib_general.Element_a_atribute_atributes_boton_awsome.Add(Item_Ilist_atribute)
                'Add atribute row
                ItList_atribute = New List(Of Class_config_general_atributes_awsome)()
                Item_Ilist_atribute = New Class_config_general_atributes_awsome()
                Item_Ilist_atribute.name_atribute = "id"
                Item_Ilist_atribute.value_atribute = lastinsert
                Item_Ilist_atrib_general.Element_a_atribute_atributes_row_awsome = New List(Of Class_config_general_atributes_awsome)
                Item_Ilist_atrib_general.Element_a_atribute_atributes_row_awsome.Add(Item_Ilist_atribute)
                Ilist_Class_config_general_service.Item(0).config_service_boton_atributes_awsome.Add(Item_Ilist_atrib_general) 'Agrega el boton
            End If
            Registra_tercero_plantilla_validacion = "YES"
            Exit Function
        Catch ex As Exception
            Registra_tercero_plantilla_validacion = "Inconsiatencia general funcion Registra_tercero_plantilla_validacion (" & ex.Message & ")"
        End Try
    End Function
    Function Consulta_ini_plantilla_validacion(ByVal id_script As Integer,
                                               ByVal page1 As Page,
                                               ByVal estado_asignacion As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Inicializa la consulta para crear la estructura y poder insertar
        '          nuevos registros
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_script             : Representa el scrip relación de la plantilla
        'page1                 : Representa el objeto page de los controles
        'estado_asignacion     : Reprenta si se muestra en la consulta el campo asignacion
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-01-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim refclasradicado As New ClassRadicador
            HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = ""
            Dim Result As String = ""
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION
            Erase Matri_Datos
            Dim prefijocampo As String = ""
            Dim scripma As GridView = page1.FindControl(prefijocampo & "GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl(prefijocampo & "titulo_label_validacion")
            Dim updatelabel As UpdatePanel = page1.FindControl(prefijocampo & "UpdatePanelabel_validacion")
            If scripma Is Nothing Then
                Consulta_ini_plantilla_validacion = "Imposible encontrar datagrid  " & prefijocampo & "GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Consulta_ini_plantilla_validacion = "Imposible encontrar el control  " & prefijocampo & "titulo_label"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Consulta_ini_plantilla_validacion = "Imposible encontrar el control  " & prefijocampo & "UpdatePanelabel_validacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl(prefijocampo & "UpdatePanel_conenido_grid_val_radicacion")
            If updatelabel Is Nothing Then
                Consulta_ini_plantilla_validacion = "Imposible encontrar el control  " & prefijocampo & "UpdatePanel_conenido_grid_val_radicacion"
                Exit Function
            End If

            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Lista_Campos_Plantilla_Validacion(id_script,
                                                                                         Matri_Datos)
            If Result <> "YES" Then
                Consulta_ini_plantilla_validacion = Result
                Exit Function
            End If
            Dim seleccampos As String = "Select "
            If Matri_Datos Is Nothing Then
                Consulta_ini_plantilla_validacion = "Imposible encontrar campos validacion"
                Exit Function
            End If
            '****************************************************
            'Retorna nombre plantilla validacion
            '****************************************************
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Consulta_ini_plantilla_validacion = Result
                Exit Function
            End If
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    seleccampos = " Select " & UCase(Matri_Datos(i).Nombre_Campo)
                    Exit For

                End If
            Next
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO <> 1 And Matri_Datos(i).Visible_Campo = 1 Then
                    seleccampos = seleccampos & "," & UCase(Matri_Datos(i).Nombre_Campo)
                End If
            Next
            Dim sqlfrom As String = " From " & nombre_plantillas
            Dim condicionsql As String = " where "
            Dim datakey() As String
            Erase datakey
            Dim campo_key As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    ReDim Preserve datakey(0)
                    datakey(0) = Matri_Datos(i).Nombre_Campo
                    campo_key = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If datakey Is Nothing Then
                Consulta_ini_plantilla_validacion = "La plantilla " & nombre_plantillas & " no tiene campo identi o primary key"
                Exit Function
            End If
            If Trim(condicionsql) = "where" Then
                condicionsql = condicionsql & " " & campo_key & "=-1"
            End If
            Dim sql_consulta As String = seleccampos & " " & sqlfrom & " " & condicionsql
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_ini_plantilla_validacion = "Error general " & Result
                labetitle.Text = "Error general  " & "(" & Result & " ) " & nombre_plantillas
                scripma.DataSource = Nothing
                scripma.DataKeyNames = datakey
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Exit Function
            End If
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) en la plantilla " & nombre_plantillas
                scripma.DataSource = Dat_set_zero
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA_DATA_SET_CAHE") = Nothing
                scripma.DataKeyNames = datakey
                scripma.DataBind()
                scripma.Rows(0).Visible = False
                updat.Update()
                updatelabel.Update()
                Consulta_ini_plantilla_validacion = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = sql_consulta
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) en la plantilla " & nombre_plantillas
                scripma.DataKeyNames = datakey
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    If estado_asignacion = "YES" Then
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-user-check fa-lg")
                        ihtml.Style.Add("color", "white")
                        ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Asigna registro de usuario remitente")
                        ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "asigna_registro_usuario")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-user-edit fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Editar registro de usuario remitente externo")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "edita_reg_usuario")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-user-times fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-warning btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Elimina registro de usuario remitente externo")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "elimina_reg_usuario")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                updat.Update()
                updatelabel.Update()
                Consulta_ini_plantilla_validacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_ini_plantilla_validacion = "Inconsistencia general funcion Consulta_ini_plantilla_validacion " & ex.Message
        End Try
    End Function
    'GENERA CONSULTA SQL PARA VALIDACION
    Function Genera_Sql_Consulta_Validacion(ByVal id_script As Integer,
                                            ByVal page1 As Page,
                                            ByRef sql_consulta As String,
                                            ByVal estado_asignacion As String) As String
        '*******************************************************************************
        'Funcion : Genera consulta para plantillas de validacion, con los parametros
        'seleccionados en la interface
        'Fecha : 2014-08-03
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************************************
        Try
            Dim refclasradicado As New ClassRadicador
            HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = ""
            Dim Result As String = ""
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION
            Erase Matri_Datos
            Dim prefijocampo As String = ""
            Dim scripma As GridView = page1.FindControl(prefijocampo & "GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl(prefijocampo & "titulo_label_validacion")
            Dim updatelabel As UpdatePanel = page1.FindControl(prefijocampo & "UpdatePanelabel_validacion")
            If scripma Is Nothing Then
                Genera_Sql_Consulta_Validacion = "Imposible encontrar datagrid  " & prefijocampo & "GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Genera_Sql_Consulta_Validacion = "Imposible encontrar el control  " & prefijocampo & "titulo_label"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Genera_Sql_Consulta_Validacion = "Imposible encontrar el control  " & prefijocampo & "UpdatePanelabel_validacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl(prefijocampo & "UpdatePanel_conenido_grid_val_radicacion")
            If updatelabel Is Nothing Then
                Genera_Sql_Consulta_Validacion = "Imposible encontrar el control  " & prefijocampo & "UpdatePanel_conenido_grid_val_radicacion"
                Exit Function
            End If

            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Lista_Campos_Plantilla_Validacion(id_script,
                                                                                         Matri_Datos)
            If Result <> "YES" Then
                Genera_Sql_Consulta_Validacion = Result
                Exit Function
            End If
            Dim seleccampos As String = "Select "
            If Matri_Datos Is Nothing Then
                Genera_Sql_Consulta_Validacion = "Imposible encontrar campos validacion"
                Exit Function
            End If
            '****************************************************
            'Retorna nombre plantilla validacion
            '****************************************************
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Genera_Sql_Consulta_Validacion = Result
                Exit Function
            End If
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    seleccampos = " Select " & UCase(Matri_Datos(i).Nombre_Campo)
                    Exit For

                End If
            Next
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO <> 1 And Matri_Datos(i).Visible_Campo = 1 Then
                    seleccampos = seleccampos & "," & UCase(Matri_Datos(i).Nombre_Campo)
                End If
            Next
            Dim sqlfrom As String = " From " & nombre_plantillas
            Dim condicionsql As String = " where "
            Dim datakey() As String
            Erase datakey
            Dim campo_key As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    ReDim Preserve datakey(0)
                    datakey(0) = Matri_Datos(i).Nombre_Campo
                    campo_key = Matri_Datos(i).Nombre_Campo
                End If
                If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO = 0 Then
                    If Matri_Datos(i).Nombre_Campo = "Pais" Or Matri_Datos(i).Nombre_Campo = "Municipio" _
                           Or Matri_Datos(i).Nombre_Campo = "Departemento" Then
                        Dim obj As DropDownList = page1.FindControl(prefijocampo & UCase(Matri_Datos(i).Nombre_Campo))
                        If obj Is Nothing Then
                            Genera_Sql_Consulta_Validacion = "Imposible encontrar campo " & prefijocampo & Matri_Datos(i).Nombre_Campo
                            Exit Function
                        End If
                        If obj.SelectedValue <> "" And obj.SelectedValue <> "SELECCIONE" Then
                            If condicionsql = " where " Then
                                condicionsql = condicionsql & Matri_Datos(i).Nombre_Campo & "='" & obj.Text & "'"
                            Else
                                condicionsql = condicionsql & " and " & Matri_Datos(i).Nombre_Campo & "='" & obj.Text & "'"
                            End If
                        End If
                    Else
                        If Matri_Datos(i).Tipo_Campo = "DATE" Or Matri_Datos(i).Tipo_Campo = "INT" Then
                            Dim obj As TextBox = page1.FindControl(prefijocampo & Matri_Datos(i).Nombre_Campo)
                            Dim obj2 As TextBox = page1.FindControl(prefijocampo & Matri_Datos(i).Nombre_Campo & "-2")
                            '-----------------------------------------
                            'Busca campo principal
                            '-----------------------------------------
                            If obj Is Nothing Then
                                Genera_Sql_Consulta_Validacion = "Imposible encontrar campo " & prefijocampo & Matri_Datos(i).Nombre_Campo
                                Exit Function
                            End If
                            '-----------------------------------------
                            'Busca campo segundario
                            '-----------------------------------------
                            If obj2 Is Nothing Then
                                Genera_Sql_Consulta_Validacion = "Imposible encontrar campo " & prefijocampo & Matri_Datos(i).Nombre_Campo & "-2"
                                Exit Function
                            End If
                            If obj.Text <> "" And obj2.Text <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & Matri_Datos(i).Nombre_Campo & " between '" & obj.Text & "' and '" & obj2.Text & "'"
                                Else
                                    condicionsql = condicionsql & " and " & Matri_Datos(i).Nombre_Campo & "  between '" & obj.Text & "' and '" & obj2.Text & "'"
                                End If
                            Else
                                If obj.Text <> "" Then
                                    If condicionsql = " where " Then
                                        condicionsql = condicionsql & Matri_Datos(i).Nombre_Campo & "='" & obj.Text & "'"
                                    Else
                                        condicionsql = condicionsql & " and " & Matri_Datos(i).Nombre_Campo & "='" & obj.Text & "'"
                                    End If
                                End If
                                If obj2.Text <> "" Then
                                    If condicionsql = " where " Then
                                        condicionsql = condicionsql & Matri_Datos(i).Nombre_Campo & "='" & obj2.Text & "'"
                                    Else
                                        condicionsql = condicionsql & " and " & Matri_Datos(i).Nombre_Campo & "='" & obj2.Text & "'"
                                    End If
                                End If
                            End If
                        Else
                            '-----------------------------------------------------------------
                            'Caso campo no between
                            '-----------------------------------------------------------------
                            Dim obj As TextBox = page1.FindControl(prefijocampo & Matri_Datos(i).Nombre_Campo)
                            If obj Is Nothing Then
                                Genera_Sql_Consulta_Validacion = "Imposible encontrar campo " & prefijocampo & Matri_Datos(i).Nombre_Campo
                                Exit Function
                            End If
                            If obj.Text <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & Matri_Datos(i).Nombre_Campo & "='" & obj.Text & "'"
                                Else
                                    condicionsql = condicionsql & " and " & Matri_Datos(i).Nombre_Campo & "='" & obj.Text & "'"
                                End If
                            End If
                        End If
                    End If
                End If
            Next
            If datakey Is Nothing Then
                Genera_Sql_Consulta_Validacion = "La plantilla " & nombre_plantillas & " no tiene campo identi o primary key"
                Exit Function
            End If
            If Trim(condicionsql) = "where" Then
                condicionsql = condicionsql & " " & campo_key & "=-1"
            End If
            sql_consulta = seleccampos & " " & sqlfrom & " " & condicionsql
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Genera_Sql_Consulta_Validacion = "Error general " & Result
                labetitle.Text = "Error general  " & "(" & Result & " ) " & nombre_plantillas
                scripma.DataSource = Nothing
                scripma.DataKeyNames = datakey
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Exit Function
            End If
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) en la plantilla " & nombre_plantillas
                scripma.DataSource = Dat_set_zero
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA_DATA_SET_CAHE") = Nothing
                scripma.DataKeyNames = datakey
                scripma.DataBind()
                scripma.Rows(0).Visible = False
                updat.Update()
                updatelabel.Update()
                Genera_Sql_Consulta_Validacion = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = sql_consulta
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) en la plantilla " & nombre_plantillas
                scripma.DataKeyNames = datakey
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    If estado_asignacion = "YES" Then
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-user-check fa-lg")
                        ihtml.Style.Add("color", "white")
                        ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Asigna registro de usuario remitente")
                        ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "asigna_registro_usuario")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-user-edit fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Editar registro de usuario remitente externo")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "edita_reg_usuario")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-user-times fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-warning btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Elimina registro de usuario remitente externo")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "elimina_reg_usuario")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                updat.Update()
                updatelabel.Update()
                Genera_Sql_Consulta_Validacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Genera_Sql_Consulta_Validacion = "Inconsistencia Funcion Genera_Sql_Consulta_Validacion " & ex.Message
        End Try
    End Function
    Function Registra_usuario_remitente_interno(ByVal nombre_plantilla_validacion As String,
                                                ByVal id_usuario_gestion As Integer,
                                                ByVal nombre_campo_key As String,
                                                ByRef matri_campos_relacion_getion_remitente() As CAMPOS_PLANTILLA_VALIDACION_RAD_INTERNO,
                                                ByRef id_remit_interno As Object) As String
        '------------------------------------------------------
        'Función : Registra el usuario de gestión como usuario
        'remitente y retorna el codido de usuario remitente
        'Fecha : 2017-12-02
        'Ingenienro : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim campos_insert As String = ""
            Dim values_insert As String = ""
            Dim tabla_insert As String = " Insert into " & nombre_plantilla_validacion & "("
            Dim sqlinsert As String = ""
            For i As Integer = 0 To matri_campos_relacion_getion_remitente.Length - 1
                If matri_campos_relacion_getion_remitente(i).IDENTI_CAMPO = 0 Then
                    If campos_insert = "" Then
                        campos_insert = matri_campos_relacion_getion_remitente(i).Nombre_Campo
                    Else
                        campos_insert = campos_insert & "," & matri_campos_relacion_getion_remitente(i).Nombre_Campo
                    End If
                    If values_insert = "" Then
                        Dim refvalor As String = ""
                        If matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO = "" Then
                            refvalor = "null"
                        Else
                            refvalor = "'" & matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO & "'"
                        End If
                        values_insert = refvalor
                    Else
                        Dim refvalor As String = ""
                        If matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO = "" Then
                            refvalor = "null"
                        Else
                            refvalor = "'" & matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO & "'"
                        End If
                        values_insert = values_insert & "," & refvalor
                    End If
                End If
            Next
            sqlinsert = tabla_insert & campos_insert & ") values (" & values_insert & ")"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref2.SELECTION_LAST_INSERT_COMMAND(sqlinsert, id_remit_interno)
            If Result <> "YES" Then
                Registra_usuario_remitente_interno = Result
                Exit Function
            End If
            Registra_usuario_remitente_interno = "YES"
        Catch ex As Exception
            Registra_usuario_remitente_interno = "Inconsistencia general función Registra_usuario_remitente_interno " & ex.Message
        End Try
    End Function

    Function Actualiza_usuario_remitente_interno(ByVal nombre_plantilla_validacion As String,
                                                ByVal nombre_campo_key As String,
                                                ByRef matri_campos_relacion_getion_remitente() As CAMPOS_PLANTILLA_VALIDACION_RAD_INTERNO,
                                                ByRef id_remit_interno As Object) As String
        '----------------------------------------------------
        'Función : Actualiza el usuario remitente con los 
        'usuario de gestión con el parametro de identifica
        'ción remitente
        'Fecha : 2017-12-02
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------
        Try
            Dim campos_insert As String = ""
            Dim tabla_insert As String = " update " & nombre_plantilla_validacion & " set "
            Dim sqlupdate As String = ""
            For i As Integer = 0 To matri_campos_relacion_getion_remitente.Length - 1
                If matri_campos_relacion_getion_remitente(i).IDENTI_CAMPO = 0 Then
                    Dim refvalor As String = ""
                    If matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO = "" Then
                        refvalor = "null"
                    Else
                        refvalor = "'" & matri_campos_relacion_getion_remitente(i).TEXTO_CAMPO & "'"
                    End If
                    If campos_insert = "" Then
                        campos_insert = matri_campos_relacion_getion_remitente(i).Nombre_Campo & "=" & refvalor
                    Else
                        campos_insert = campos_insert & "," & matri_campos_relacion_getion_remitente(i).Nombre_Campo & "=" & refvalor
                    End If
                End If
            Next
            sqlupdate = tabla_insert & campos_insert & " where " & nombre_campo_key & "=" & id_remit_interno
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref2.SELECTION_INSERT_COMMAND(sqlupdate)
            If Result <> "YES" Then
                Actualiza_usuario_remitente_interno = Result
                Exit Function
            End If
            Actualiza_usuario_remitente_interno = "YES"
        Catch ex As Exception
            Actualiza_usuario_remitente_interno = "Inconsistencia general función Actualiza_usuario_remitente_interno " & ex.Message
        End Try
    End Function
    Function Registra_usuario_gestion_remitente_externo(ByVal Nombre_plantilla_radicado As String,
                                                        ByVal id_plantilla_radicado As Integer,
                                                        ByVal id_usuario_gestion As Integer,
                                                        ByRef id_relacion_gestion_remitente As Integer) As String
        Try
            '-------------------------------------------------
            'Lista campos adicionales plantilla 
            '-------------------------------------------------
            Dim Result As String = ""
            Dim Matri_Campos_plantilla() As Campos_Plantilla = Nothing
            Dim Estado_opcion_fecha As Integer = 0
            Dim Estado_opcion_cita_respuesta As Integer = 0
            Dim Estado_opcion_radicado_general As Integer = 0
            Dim Refclas As New ClassRadicador
            Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
            Result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(id_plantilla_radicado,
                                                                                                Matri_Campos_plantilla,
                                                                                                Estado_opcion_fecha,
                                                                                                Estado_opcion_cita_respuesta,
                                                                                                Estado_opcion_radicado_general)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            '---------------------------------------
            '------Asigna datos campos validacion
            '---------------------------------------
            Dim ref_Class_ra_script_actividades As New Class_ra_script_actividades
            Dim matri_validacion() As validacion_plantilla
            Erase matri_validacion
            Result = ref_Class_ra_script_actividades.lista_campos_Validacion_plantilla(id_plantilla_radicado,
                                                                                       matri_validacion)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            If Not matri_validacion Is Nothing Then
                For i2 As Integer = 0 To matri_validacion.Length - 1
                    For i3 As Integer = 0 To Matri_Campos_plantilla.Length - 1
                        If Matri_Campos_plantilla(i3).Campo_Plantilla = matri_validacion(i2).Campo_Plantilla Then
                            Matri_Campos_plantilla(i3).TIPO_SCRIPT = matri_validacion(i2).TIPO_SCRIPT
                            Matri_Campos_plantilla(i3).COMBINACION_TECLA = matri_validacion(i2).COMBINACION_TECLA
                            Matri_Campos_plantilla(i3).VALOR_SCRIPT = matri_validacion(i2).VALOR_SCRIPT
                            Matri_Campos_plantilla(i3).ESTADO_ESCRIPT = matri_validacion(i2).ESTADO_ESCRIPT
                            Matri_Campos_plantilla(i3).PLATAFORMA_SCRIPT = matri_validacion(i2).PLATAFORMA_SCRIPT
                            Matri_Campos_plantilla(i3).ID_SCRIPT = matri_validacion(i2).ID_SCRIPT

                        End If
                    Next
                Next
            End If
            '----------------------------------------------------
            'Retorna el tipo de validación del campo comparación
            '----------------------------------------------------
            Dim nombre_campo As String = ""
            Dim Tipo_script As String = ""
            Dim id_SCRIPT As Integer = -1
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Tipo_Validacion_Campo(Matri_Campos_plantilla,
                                                                              "REMITENTE_COR",
                                                                              Tipo_script,
                                                                              id_SCRIPT)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna nombre plantilla validación
            '---------------------------------------------------
            Dim nombre_plantilla_validacion As String = ""
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_SCRIPT,
                                                                                    nombre_plantilla_validacion)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Solicita identificación plantilla validación
            '---------------------------------------------------
            Dim id_plantilla_validacion As Integer = 0
            Result = Class_plantilla_validacion.Retorna_id_plantilla_validacion_nombre(nombre_plantilla_validacion,
                                                                                       id_plantilla_validacion)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            '--------------------------------------------------
            'Verifica la existencia de campo de relación de 
            'usuario interno con destinatario externo
            '--------------------------------------------------
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Dim estado_existencia_campo_relacion As String = "NO"
            Result = Class_campos_plantilla_validacion.Verifica_existencia_campo_identificacion_usuario_gestion_plantilla(id_plantilla_validacion,
                                                                                                                          estado_existencia_campo_relacion)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            If estado_existencia_campo_relacion = "NO" Then
                Registra_usuario_gestion_remitente_externo = "Imposible encontrar el campo id_interno_radicado en la plantilla de validación " & nombre_plantilla_validacion &
                    " contacte a su administrador para que agregue el campo a la plantilla " & nombre_plantilla_validacion
                Exit Function
            End If
            '-------------------------------------------------
            'Retorna campo primary plantilla validacion
            '-------------------------------------------------
            Dim nombre_campo_primary As String = ""

            Result = Class_campos_plantilla_validacion.Retorna_Campo_Primary_key_plantilla_validacion(id_plantilla_validacion,
                                                                                                      nombre_campo_primary)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            '------------------------------------------------
            'Solicita relación entre usuario de gestion y 
            'remitente interno
            '------------------------------------------------
            Result = Class_plantilla_validacion.Solicita_id_relacion_usuario_gestion_como_remitente_interno(id_usuario_gestion,
                                                                                                            nombre_plantilla_validacion,
                                                                                                            nombre_campo_primary,
                                                                                                            id_relacion_gestion_remitente)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            ''------------------------------------------------------------------------------
            ''Se sale por que el campo relacion usuario de gestion y remitente esta creado
            ''------------------------------------------------------------------------------
            If id_relacion_gestion_remitente <> 0 Then
                Registra_usuario_gestion_remitente_externo = "YES"
                Exit Function
            End If
            '----------------------------------------------------
            'Solicita campos relación remitente usuario gestión
            '----------------------------------------------------
            Dim Class_relacion_script_plantilla As New Class_relacion_script_plantilla
            Dim matri_campos_relacion_getion_remitente() As CAMPOS_PLANTILLA_VALIDACION_RAD_INTERNO = Nothing
            Result = Class_relacion_script_plantilla.Solicta_campos_relacion_remitente_usuario_gestion(id_SCRIPT,
                                                                                                       matri_campos_relacion_getion_remitente)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Solicita el nombre del campo remitente en la 
            'relación de capos entre el usuario de gestión y el 
            'usuario remitente en la plantilla validación
            '---------------------------------------------------
            Dim nombre_campo_nombre_usuario_remitente As String = ""
            Result = Class_campos_plantilla_validacion.Solicita_nombre_campo_remitente_relacion_usuario_gestion_usuario_remitente("Nombre_Remitente",
                                                                                                                                  matri_campos_relacion_getion_remitente,
                                                                                                                                  nombre_campo_nombre_usuario_remitente)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            If nombre_campo_nombre_usuario_remitente = "" Then
                Registra_usuario_gestion_remitente_externo = "Debe relacionar los campos (nombre remitente) en la plantilla de validación " & nombre_plantilla_validacion & " en la columna (Relacion usuario gestion) del administrador de plantillas"
                Exit Function
            End If
            '---------------------------------------------------
            'Solicita datos de caracterización usuario gestión
            '----------------------------------------------------
            Dim nombre_usuario_gestion As String = ""
            Dim correo_electronico As String = ""
            Dim telefono As String = ""
            Dim identificacion As String = ""
            Dim direccion As String = ""
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Result = Class_remit_dest_interno.Solicita_datos_de_caracterizacion_usuario_gestion(id_usuario_gestion,
                                                                                                nombre_usuario_gestion,
                                                                                                correo_electronico,
                                                                                                telefono,
                                                                                                identificacion,
                                                                                                direccion)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            '-------------------------------------------------------
            'Verifica existencia del nombre del usuario de gestión
            'como remitente de correspondencia
            '-------------------------------------------------------
            Dim estado_existencia_usuario_gestion_remitente As String = "NO"
            Result = Class_plantilla_validacion.Verifica_existencia_usuario_gestion_como_remitente_plantilla_validacion(nombre_usuario_gestion,
                                                                                                                        nombre_plantilla_validacion,
                                                                                                                        nombre_campo_nombre_usuario_remitente,
                                                                                                                        estado_existencia_usuario_gestion_remitente)
            If Result <> "YES" Then
                Registra_usuario_gestion_remitente_externo = Result
                Exit Function
            End If
            If estado_existencia_usuario_gestion_remitente = "NO" Then
                '----------------------------------------------------------
                'Asigna los datos del usuario de gestión 
                '----------------------------------------------------------
                Result = Class_campos_plantilla_validacion.Asigna_datos_estructutura_plantilla_validacion_usuario_gestion(id_usuario_gestion,
                                                                                                                          nombre_usuario_gestion,
                                                          correo_electronico,
                                                          telefono,
                                                          identificacion,
                                                          direccion,
                                                          id_usuario_gestion,
                                                          matri_campos_relacion_getion_remitente)
                If Result <> "YES" Then
                    Registra_usuario_gestion_remitente_externo = Result
                    Exit Function
                End If
                '------------------------------------------------------------
                'Registra el nuevo remitente con los datos del usuario
                'de gestión
                '------------------------------------------------------------
                Dim id_remitente As Integer = 0
                Result = Class_plantilla_validacion.Registra_usuario_remitente_interno(nombre_plantilla_validacion,
                                                                                       id_usuario_gestion,
                                                                                       nombre_campo_primary,
                                                                                       matri_campos_relacion_getion_remitente,
                                                                                       id_remitente)
                If Result <> "YES" Then
                    Registra_usuario_gestion_remitente_externo = Result
                    Exit Function
                Else
                    id_relacion_gestion_remitente = id_remitente
                End If
                Registra_usuario_gestion_remitente_externo = "YES"
                Exit Function
            Else

                '---------------------------------------------------
                'Solicita datos de caracterización usuario gestión
                '----------------------------------------------------               
                'Result = Class_remit_dest_interno.Solicita_datos_de_caracterizacion_usuario_gestion(id_usuario_gestion,
                '                                                               nombre_usuario_gestion,
                '                                                               correo_electronico,
                '                                                               telefono,
                '                                                               identificacion,
                '                                                               direccion)
                'If Result <> "YES" Then
                '    Registra_usuario_gestion_remitente_externo = Result
                '    Exit Function
                'End If
                '----------------------------------------------------------
                'Asigna los datos del usuario de gestión 
                '----------------------------------------------------------
                'Result = Class_campos_plantilla_validacion.Asigna_datos_estructutura_plantilla_validacion_usuario_gestion(id_usuario_gestion,
                '                                                                                                              nombre_usuario_gestion,
                '                                                                                                              correo_electronico,
                '                                                                                                              telefono,
                '                                                                                                              identificacion,
                '                                                                                                              direccion,
                '                                                                                                              id_usuario_gestion,
                '                                                                                                              matri_campos_relacion_getion_remitente)
                'If Result <> "YES" Then
                '    Registra_usuario_gestion_remitente_externo = Result
                '    Exit Function
                'End If
                '---------------------------------------------------
                'Verfica la existencia del usuario de gestión en la
                'tabla de validación
                '---------------------------------------------------
                'Dim id_remitente As Integer = 0
                'Result = Class_plantilla_validacion.Verifica_existencia_usuario_remitente_plantilla_validacion(nombre_campo_nombre_usuario_remitente,
                '                                                                                                   nombre_plantilla_validacion,
                '                                                                                                   nombre_usuario_gestion,
                '                                                                                                   nombre_campo_primary,
                '                                                                                                   id_remitente)
                'If Result <> "YES" Then
                '    Registra_usuario_gestion_remitente_externo = Result
                '    Exit Function
                'End If
                '------------------------------------------------------------
                'Actualiza usuario remitente con los datos del usuario de
                'gestión
                '-------------------------------------------------------------
                'Result = Class_plantilla_validacion.Actualiza_usuario_remitente_interno(nombre_plantilla_validacion,
                '                                                                            nombre_campo_primary,
                '                                                                            matri_campos_relacion_getion_remitente,
                '                                                                            id_relacion_gestion_remitente)
                'If Result <> "YES" Then
                '    Registra_usuario_gestion_remitente_externo = Result
                '    Exit Function
                'End If
                Registra_usuario_gestion_remitente_externo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_usuario_gestion_remitente_externo = "Inconsistencia general funcion Registra_usuario_gestion_remitente_externo " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_usuario_gestion_como_remitente_plantilla_validacion(ByVal nombre_usuario_gestion_remitente As String,
                                                                                     ByVal nombre_plantilla_validacion As String,
                                                                                     ByVal nombre_campo_remitente_plantilla_validacion As String,
                                                                                     ByRef estado_existencia_usuario As String) As String
        Try
            Dim Parametro_Consulta = "select " & nombre_campo_remitente_plantilla_validacion &
            " from " & nombre_plantilla_validacion & " where " & nombre_campo_remitente_plantilla_validacion & "='" & nombre_usuario_gestion_remitente & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("campos_plantilla_validacion")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_usuario_gestion_como_remitente_plantilla_validacion = "Función  Verifica_existencia_usuario_gestion_como_remitente_plantilla_validacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia_usuario = "NO"
                Verifica_existencia_usuario_gestion_como_remitente_plantilla_validacion = "YES"
                Exit Function
            Else
                estado_existencia_usuario = "YES"
                Verifica_existencia_usuario_gestion_como_remitente_plantilla_validacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_usuario_gestion_como_remitente_plantilla_validacion = "Inconsistencia general función Verifica_existencia_usuario_gestion_como_remitente_plantilla_validacion " & ex.Message
        End Try
    End Function
    Function Valida_existencia_nombre_usuario_pqr(ByVal nombre_campo_nombre As String,
                                                  ByVal valor_nombre_campo As String,
                                                  ByVal nombre_campo_idest As String,
                                                  ByVal nombre_plantilla_validacion As String,
                                                  ByRef valor_clave_primaria As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select " & nombre_campo_idest & " from " & nombre_plantilla_validacion & " where " & nombre_campo_nombre & "='" &
                valor_nombre_campo & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Valida_existencia_nombre_usuario_pqr = " Error verificando existencia nombre usuario  " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                valor_clave_primaria = -1
                Valida_existencia_nombre_usuario_pqr = "YES"
                Exit Function
            Else
                valor_clave_primaria = Dat_reader.Tables(0).Rows(0).Item(0)
                Valida_existencia_nombre_usuario_pqr = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Valida_existencia_nombre_usuario_pqr = "Inconsistencia general función Valida_existencia_nombre_usuario_pqr " & ex.Message
        End Try
    End Function
    Function Retorna_Tipo_Validacion_Campo(ByVal stru() As Campos_Plantilla,
        ByVal nombre_campo As String, ByRef Tipo_script As String, ByRef id_SCRIPT As Integer) As String
        Try
            Tipo_script = ""
            For i As Integer = 0 To stru.Length - 1
                If stru(i).Campo_Plantilla = nombre_campo Then
                    Tipo_script = stru(i).TIPO_SCRIPT
                    id_SCRIPT = stru(i).ID_SCRIPT
                    Exit For
                End If
            Next

            Retorna_Tipo_Validacion_Campo = "YES"
        Catch ex As Exception
            Retorna_Tipo_Validacion_Campo = "Inconsistencia general funcion : Retorna_Tipo_Validacion_Campo " & ex.Message
        End Try
    End Function
    Function Retorna_Nombre_Plantilla_Validacion(ByVal Id_script_validacion As Integer,
                                                 ByRef Nombre_Plantilla As String) As String
        '*****************************************************************
        'Funcion : Retorna nombre plantilla validacion con el id de la
        'plantilla
        'Fecha : 2014-07-28
        'Ing : Miguel Angel Urueta Miranda
        '*****************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "SELECT pv.Nombre_Plantilla FROM relacion_script_plantilla as rsp INNER JOIN PLANTILLA_VALIDACION as pv on " &
            " (pv.Id_Plantilla_Validacion=rsp.Plantilla_Validacion_Id_Plantilla_Validacion) where rsp.script_actividades_id_script=" & Id_script_validacion
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Nombre_Plantilla_Validacion = " Error Listando nombre plantilla VALIDACION   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Nombre_Plantilla_Validacion = "Imposible encontrar el nombre de la plantilla validacion"
                Exit Function
            Else
                Nombre_Plantilla = Datset.Tables(0).Rows(0).Item(0).ToString
                Retorna_Nombre_Plantilla_Validacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Nombre_Plantilla_Validacion = "Inconsistencia general funcion Retorna_Nombre_Plantilla_Validacion " & ex.Message
        End Try
    End Function

    Function Registra_actualiza_usuario_pqr(ByVal id_script As Integer,
                                            ByRef id_usuario_pqr As Integer,
                                            ByVal array_form_control() As CAMPOS_PLANTILLA_VALIDACION_PQR) As String
        Try
            Dim Result As String = ""
            Dim nombre_plantillas As String = ""
            Dim campo_idex As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Dim array_bd_control() As CAMPOS_PLANTILLA_VALIDACION_PQR = Nothing
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(id_script,
                                                                                                array_bd_control)
            If Result <> "YES" Then
                Registra_actualiza_usuario_pqr = Result
                Exit Function
            End If
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Registra_actualiza_usuario_pqr = Result
                Exit Function
            End If
            For i As Integer = 0 To array_bd_control.Length - 1
                For k As Integer = 0 To array_form_control.Length - 1
                    If LCase(array_bd_control(i).Nombre_Campo) = LCase(array_form_control(k).Nombre_Campo) Then
                        array_bd_control(i).TEXTO_CAMPO_MODIFICADO = array_form_control(k).TEXTO_CAMPO_MODIFICADO
                        Exit For
                    End If
                Next
            Next
            Dim ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To array_bd_control.Length - 1
                If array_bd_control(i).Visible_Campo = 1 And array_bd_control(i).IDENTI_CAMPO <> 1 Then
                    If array_bd_control(i).Tipo_Campo = "DATE" And array_bd_control(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                        Result = ClassGestionFechas.Verifi_campo_fecha(array_bd_control(i).TEXTO_CAMPO_MODIFICADO)
                        If Result <> "YES" Then
                            Registra_actualiza_usuario_pqr = "El formato fecha no cumple " & Result
                            Exit Function
                        End If
                    End If
                End If
            Next
            For i As Integer = 0 To array_bd_control.Length - 1
                If array_bd_control(i).IDENTI_CAMPO = 1 Then
                    campo_idex = array_bd_control(i).Nombre_Campo
                End If
            Next
            If campo_idex = "" Then
                Registra_actualiza_usuario_pqr = "La plantilla carece de campo identi por favor agregelo en el módulo administración"
                Exit Function
            End If
            '---------------------------------------------------------------------------------
            'Valida campos obligatorios vacios
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To array_bd_control.Length - 1
                If array_bd_control(i).Visible_Campo = 1 And array_bd_control(i).IDENTI_CAMPO <> 1 And array_bd_control(i).Obligatorio_Campo = 1 Then
                    If array_bd_control(i).TEXTO_CAMPO_MODIFICADO = "" Then

                        Dim ref_campo As String = ""
                        If array_bd_control(i).Aleas_Campo_pqr = "" Then
                            ref_campo = array_bd_control(i).Nombre_Campo
                        Else
                            ref_campo = array_bd_control(i).Aleas_Campo_pqr
                        End If
                        Registra_actualiza_usuario_pqr = "El campo " & ref_campo & " es obligatorio"
                        Exit Function
                    End If
                End If
            Next
            '-------------------------------------------------------------
            'Actualiza los campos de letra capital, upercase o towupercase
            '-------------------------------------------------------------
            Dim clas_trd_documental As New ClassTrdDocumental
            For i As Integer = 0 To array_bd_control.Length - 1
                If array_bd_control(i).Visible_Campo = 1 And array_bd_control(i).IDENTI_CAMPO <> 1 And array_bd_control(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                    If array_bd_control(i).valida_capital_text = 1 Then
                        clas_trd_documental.Formato_sub_serie(array_bd_control(i).TEXTO_CAMPO_MODIFICADO,
                                                              array_bd_control(i).TEXTO_CAMPO_MODIFICADO)
                    End If
                    If array_bd_control(i).valida_capital_text = 2 Then
                        array_bd_control(i).TEXTO_CAMPO_MODIFICADO = UCase(array_bd_control(i).TEXTO_CAMPO_MODIFICADO)
                    End If
                    If array_bd_control(i).valida_capital_text = 3 Then
                        array_bd_control(i).TEXTO_CAMPO_MODIFICADO = LCase(array_bd_control(i).TEXTO_CAMPO_MODIFICADO)
                    End If
                End If
            Next
            If id_usuario_pqr = 0 Then
                '---------------------------------------------------------------------------- -----
                'Genera comnando de insertcion
                '---------------------------------------------------------------------------------
                Dim sqlactualizacion As String = "INSERT INTO " & nombre_plantillas & " "
                Dim sqlcampos As String = ""
                Dim sqlvalores As String = ""
                '---------------------------------------------------------------------------------
                'Lista campos de insertcion
                '---------------------------------------------------------------------------------
                For i As Integer = 0 To array_bd_control.Length - 1
                    If array_bd_control(i).Visible_Campo = 1 And array_bd_control(i).IDENTI_CAMPO <> 1 Then

                        If sqlcampos = "" Then
                            sqlcampos = sqlcampos & "(" & array_bd_control(i).Nombre_Campo
                        Else
                            sqlcampos = sqlcampos & " , " & array_bd_control(i).Nombre_Campo
                        End If

                        If sqlvalores = "" Then
                            sqlvalores = sqlvalores & "(" & "'" & array_bd_control(i).TEXTO_CAMPO_MODIFICADO & "'"
                        Else
                            sqlvalores = sqlvalores & " , " & "'" & array_bd_control(i).TEXTO_CAMPO_MODIFICADO & "'"
                        End If
                    End If
                Next
                sqlvalores = sqlvalores & ")"
                sqlcampos = sqlcampos & ")"
                sqlactualizacion = sqlactualizacion & sqlcampos & " values " & sqlvalores
                Dim lastinsert As Object = Nothing
                Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
                Result = Ref_Car_Conec.SELECTION_LAST_INSERT_COMMAND(sqlactualizacion, lastinsert)
                If Result <> "YES" Then
                    Registra_actualiza_usuario_pqr = Result
                    Exit Function
                End If
                id_usuario_pqr = lastinsert
                Registra_actualiza_usuario_pqr = "YES"
                Exit Function
            Else
                '---------------------------------------------------------------------------- -----
                'Genera comnando de actualización
                '---------------------------------------------------------------------------------
                Dim sqlactualizacion As String = "update " & nombre_plantillas & " "
                Dim sqlcondicion As String = " where " & campo_idex & "=" & id_usuario_pqr
                Dim sqlvalores As String = ""
                '---------------------------------------------------------------------------------
                'Lista campos de insertcion
                '---------------------------------------------------------------------------------
                For i As Integer = 0 To array_bd_control.Length - 1
                    If array_bd_control(i).Visible_Campo = 1 And array_bd_control(i).IDENTI_CAMPO <> 1 Then
                        Dim valor_campo_actualizacion As String = ""
                        If array_bd_control(i).TEXTO_CAMPO_MODIFICADO = "" Then
                            valor_campo_actualizacion = "null"
                        Else
                            valor_campo_actualizacion = "'" & array_bd_control(i).TEXTO_CAMPO_MODIFICADO & "'"
                        End If
                        If sqlvalores = "" Then
                            sqlvalores = sqlvalores & "set " & array_bd_control(i).Nombre_Campo & "=" & valor_campo_actualizacion
                        Else
                            sqlvalores = sqlvalores & " , " & array_bd_control(i).Nombre_Campo & "=" & valor_campo_actualizacion
                        End If
                    End If
                Next
                sqlvalores = sqlvalores & sqlcondicion
                sqlactualizacion = sqlactualizacion & sqlvalores
                Dim lastinsert As Object = Nothing
                Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
                Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sqlactualizacion)
                If Result <> "YES" Then
                    Registra_actualiza_usuario_pqr = Result
                    Exit Function
                End If
                Registra_actualiza_usuario_pqr = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_actualiza_usuario_pqr = "Inconsistencia general funcion Registra_actualiza_usuario_pqr " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_formulario_registro_validacion_externo(ByVal id_secript As Integer,
                                                                        ByVal id_registro As Integer,
                                                                        ByVal name_space_campo As String,
                                                                        ByRef resultList As List(Of Class_config_general_service)) As String
        '---------------------------------------------------------------------------
        'Funcion : Expone la estructura del los externos o terceros peticionarios
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_secript      : Representa la identificaación del script de la plantilla
        'id_registro     : Representa el identificación del registro
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service : Retorna la estructura de los campos de los
        '                               terceros o solicitantes
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Dim Result As String = ""
            Dim id_script_plantilla As Object = id_secript
            Dim Nombre_plantilla_validacion As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script_plantilla,
                                                                                    Nombre_plantilla_validacion)
            If Result <> "YES" Then
                Solicita_estructura_formulario_registro_validacion_externo = Result
                Exit Function
            End If
            Dim ClassRadicador As New ClassRadicador
            Result = ClassRadicador.Verifica_Permisos_usuario_plantilla_validacion(id_script_plantilla,
                                                                                   Nombre_plantilla_validacion,
                                                                                   1)
            If Result <> "YES" Then
                Solicita_estructura_formulario_registro_validacion_externo = Result
                Exit Function
            End If
            Dim id_plantilla As Integer = 0
            Result = Class_plantilla_validacion.Retorna_id_Plantilla_Validacion_id_script(id_script_plantilla,
                                                                                          id_plantilla)
            If Result <> "YES" Then
                Solicita_estructura_formulario_registro_validacion_externo = Result
                Exit Function
            End If
            Result = Class_plantilla_validacion.Solicita_estructura_plantilla_validacion_externos(id_script_plantilla,
                                                                                                  Nombre_plantilla_validacion,
                                                                                                  name_space_campo,
                                                                                                  resultList)
            If Result <> "YES" Then
                Solicita_estructura_formulario_registro_validacion_externo = Result
                Exit Function
            End If
            Dim campo_primary As String = ""
            Result = Class_campos_plantilla_validacion.Retorna_Campo_Primary_key_plantilla_validacion(id_plantilla,
                                                                                                      campo_primary)
            If Result <> "YES" Then
                Solicita_estructura_formulario_registro_validacion_externo = Result
                Exit Function
            End If
            Dim Class_config_general_service As New Class_config_general_service
            '-------Asigna datos a la interface
            If id_registro <> 0 Then
                Result = Class_plantilla_validacion.Asigna_datos_campos_plantilla_validacion_externos(id_registro,
                                                                                                      Nombre_plantilla_validacion,
                                                                                                      campo_primary,
                                                                                                      resultList)
                If Result <> "YES" Then
                    Solicita_estructura_formulario_registro_validacion_externo = Result
                    Exit Function
                End If
                For i As Integer = 0 To resultList.Count - 1
                    If resultList.Item(i).campo_tip = 0 Then
                        If Not resultList.Item(i).config_service_drowlis_destino Is Nothing Then
                            '-----------------------------asigna valor clave campo destino------------------------------------
                            For z As Integer = 0 To resultList.Count - 1
                                If resultList.Item(i).name_campo = resultList.Item(z).drow_name_controls_destino Then
                                    resultList.Item(i).config_service_drowlis_destino.Item(0).value_condicion = resultList.Item(z).config_service_drowlis_destino.Item(0).value_condicion
                                End If
                            Next
                            Dim Class_service_ilist_drowlist = New List(Of Class_config_general_service.Class_service_ilist_drowlist)()
                            resultList.Item(i).config_service_drowlis_destino(0).value_default = resultList.Item(i).texto_campo
                            Result = Class_config_general_service.Solicita_datos_drowlist_form_control(resultList.Item(i),
                                                                                                       Class_service_ilist_drowlist)
                            If Result <> "YES" Then
                                Solicita_estructura_formulario_registro_validacion_externo = Result
                                Exit Function
                                Exit For
                            Else
                                resultList.Item(i).ilist_row_drowlist = Class_service_ilist_drowlist
                            End If
                        End If
                    End If
                Next
            Else
                '-------Lista los registros de cada campo tipo drowplist  sin asignacion de datos al formulario
                For i As Integer = 0 To resultList.Count - 1
                    If resultList.Item(i).campo_tip = 0 Then
                        If Not resultList.Item(i).config_service_drowlis_destino Is Nothing Then
                            If resultList.Item(i).config_service_drowlis_destino(0).campo_estado_auto_lista <> 0 Then
                                Dim Class_service_ilist_drowlist = New List(Of Class_config_general_service.Class_service_ilist_drowlist)()
                                resultList.Item(i).config_service_drowlis_destino(0).value_default = resultList.Item(i).texto_campo
                                Result = Class_config_general_service.Solicita_datos_drowlist_form_control(resultList.Item(i),
                                                                                                           Class_service_ilist_drowlist)
                                If Result <> "YES" Then
                                    Solicita_estructura_formulario_registro_validacion_externo = Result
                                    Exit Function
                                    Exit For
                                Else
                                    resultList.Item(i).ilist_row_drowlist = Class_service_ilist_drowlist
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            resultList.Item(0).name_campo_id = campo_primary
            resultList.Item(0).tbl_control = Nombre_plantilla_validacion
            Solicita_estructura_formulario_registro_validacion_externo = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_estructura_formulario_registro_validacion_externo = "Inconsistencia general funcion Solicita_estructura_formulario_registro_validacion_externo " & ex.Message
        End Try
    End Function
    Function Solicita_datos_auto_complete_tercero_plantilla(ByVal name_dbs_auto As String,
                                                            ByVal nombre_plantilla As String,
                                                            ByVal name_campo_nombre As String,
                                                            ByVal name_campo_primary As String,
                                                            ByVal value_auto As String,
                                                            ByRef country As List(Of class_config_gneral_service_row_option_tom_select)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la lista de terceros o solicitantes de plantillas de val
        '          validación para auto complete
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'name_dbs_auto      : Representa el conector de base de datos
        'nombre_plantilla   : Representa el nombre de la plantilla de validación
        'name_campo_nombre  : Representa el campo nombre de la plantilla
        'name_campo_primary : Representa el campo primary de la plantilla
        'value_auto         : Representa el parametro de busqueda
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'country            : Retorna la estructura con los datos
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-20
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ref As Object
            Dim Result As String = ""
            Dim Sql_consulta As String = "Select " & name_campo_primary & "," & name_campo_nombre & " from " & nombre_plantilla & " where " & name_campo_nombre & " like '%" & value_auto & "%' LIMIT 50"
            If name_dbs_auto = "WF" Then
                ref = New conect.Dbase_Conction_Mysql
            Else
                ref = New conect.Dbase_Conction_Mysql_RA
            End If
            If value_auto = "" Then
                Dim item_ As New class_config_gneral_service_row_option_tom_select
                item_.id_value = "-1"
                item_.tex_value = "No resultados"
                country.Add(item_)
                Solicita_datos_auto_complete_tercero_plantilla = "no result"
                Exit Function
            End If
            Dim Datset As DataSet = New DataSet("DAT_ADIC")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_auto_complete_tercero_plantilla = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Dim item_ As New class_config_gneral_service_row_option_tom_select
                item_.id_value = "-1"
                item_.tex_value = "No resultados"
                country.Add(item_)
                Solicita_datos_auto_complete_tercero_plantilla = "no result"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim item_ As New class_config_gneral_service_row_option_tom_select
                    item_.id_value = Datset.Tables(0).Rows(i).Item(0)
                    If Datset.Tables(0).Rows(i).IsNull(1) = False Then
                        Dim obsgetipe As Object = Datset.Tables(0).Rows(i).Item(1).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = Datset.Tables(0).Rows(i).Item(1).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            item_.tex_value = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                        Else
                            item_.tex_value = Datset.Tables(0).Rows(i).Item(1).ToString()
                        End If
                    End If
                    country.Add(item_)
                Next
                Solicita_datos_auto_complete_tercero_plantilla = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_auto_complete_tercero_plantilla = "Inconsistencia general funcion Solicita_datos_auto_complete_tercero_plantilla " & ex.Message
        End Try
    End Function
    Function Solicita_campo_property_plantilla_validacion(ByVal id_script As Integer,
                                                          ByRef campo_nombre As String,
                                                          ByRef campo_identificacion As String,
                                                          ByRef campo_anualidad As String,
                                                          ByRef campo_primary_key As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita campos property de la plantilla de validación
        '          de terceros
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_script             : Representa la iodenitifcación del escript del tercero
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'campo_nombre  : Retorna el nombre del campo nombre
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-18
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            campo_nombre = ""
            Dim Result As String = ""
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Dim array_bd_control() As CAMPOS_PLANTILLA_VALIDACION_PQR = Nothing
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(id_script,
                                                                                                array_bd_control)
            If Result <> "YES" Then
                Solicita_campo_property_plantilla_validacion = Result
                Exit Function
            End If
            For i As Integer = 0 To array_bd_control.Length - 1
                If array_bd_control(i).Campo_nombre_pqr = 1 Then
                    campo_nombre = UCase(array_bd_control(i).Nombre_Campo)
                End If
                If array_bd_control(i).Campo_Iidenti_pqr = 1 Then
                    campo_identificacion = UCase(array_bd_control(i).Nombre_Campo)
                End If
                If array_bd_control(i).Campo_anualidad_pqr = 1 Then
                    campo_anualidad = UCase(array_bd_control(i).Nombre_Campo)
                End If
                If array_bd_control(i).IDENTI_CAMPO = 1 Then
                    campo_primary_key = UCase(array_bd_control(i).Nombre_Campo)
                End If
            Next
            If campo_nombre = "" Then
                Solicita_campo_property_plantilla_validacion = "Imposible encontar el campo nombre del a plantilla de validación"
                Exit Function
            Else
                Solicita_campo_property_plantilla_validacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_campo_property_plantilla_validacion = "Inconsistencia general función Solicita_campo_property_plantilla_validacion " & ex.Message
        End Try
    End Function
    Function Valida_exitencia_usuario_peticionario(ByVal id_script As Integer,
                                                   ByVal array_form_control() As CAMPOS_PLANTILLA_VALIDACION_PQR,
                                                   ByRef primary_peticionario As Integer,
                                                   ByRef campo_error As String,
                                                   ByRef valor_campo_error As String) As String
        Try
            Dim Result As String = ""
            Dim nombre_plantillas As String = ""
            Dim campo_idex As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Dim array_bd_control() As CAMPOS_PLANTILLA_VALIDACION_PQR = Nothing
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(id_script,
                                                                                                array_bd_control)
            If Result <> "YES" Then
                Valida_exitencia_usuario_peticionario = Result
                Exit Function
            End If
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Valida_exitencia_usuario_peticionario = Result
                Exit Function
            End If
            Dim campo_consulta_nombre As String = ""
            Dim campo_identificacion As String = ""
            Dim campo_anualidad As String = ""
            Dim campo_primary_key As String = ""
            '----------------------------------------------Asigna campos de validación exitencia usuario--------------------
            For i As Integer = 0 To array_bd_control.Length - 1
                If array_bd_control(i).Campo_nombre_pqr = 1 Then
                    campo_consulta_nombre = UCase(array_bd_control(i).Nombre_Campo)
                End If
                If array_bd_control(i).Campo_Iidenti_pqr = 1 Then
                    campo_identificacion = UCase(array_bd_control(i).Nombre_Campo)
                End If
                If array_bd_control(i).Campo_anualidad_pqr = 1 Then
                    campo_anualidad = UCase(array_bd_control(i).Nombre_Campo)
                End If
                If array_bd_control(i).IDENTI_CAMPO = 1 Then
                    campo_primary_key = UCase(array_bd_control(i).Nombre_Campo)
                End If
            Next
            '-------------------------------------------Valida exitencia campos--------------------------------------
            If campo_consulta_nombre = "" Then
                Valida_exitencia_usuario_peticionario = "El campo (nombre peticionario) no se encuentra configurado en la plantilla de validación (" & nombre_plantillas & ")"
                Exit Function
            End If
            If campo_identificacion = "" Then
                Valida_exitencia_usuario_peticionario = "El campo (identificacion) no se encuentra configurado en la plantilla de validación (" & nombre_plantillas & ")"
                Exit Function
            End If
            If campo_anualidad = "" Then
                Valida_exitencia_usuario_peticionario = "El campo (anualidad) no se encuentra configurado en la plantilla de validación (" & nombre_plantillas & ")"
                Exit Function
            End If
            If campo_primary_key = "" Then
                Valida_exitencia_usuario_peticionario = "El campo (primary) no se encuentra configurado en la plantilla de validación (" & nombre_plantillas & ")"
                Exit Function
            End If
            '------------------------------------Asigna los valores de los campos del formulario---------------------------------------
            Dim valor_campo_nombre_form_control As String = ""
            Dim valor_campo_identificacion_form_control As String = ""
            Dim valor_campo_anualidad_form_control As String = ""
            For i As Integer = 0 To array_form_control.Length - 1
                If UCase(array_form_control(i).Nombre_Campo) = campo_consulta_nombre Then
                    valor_campo_nombre_form_control = array_form_control(i).TEXTO_CAMPO_MODIFICADO
                End If
                If UCase(array_form_control(i).Nombre_Campo) = campo_identificacion Then
                    valor_campo_identificacion_form_control = array_form_control(i).TEXTO_CAMPO_MODIFICADO
                End If
                If UCase(array_form_control(i).Nombre_Campo) = campo_anualidad Then
                    valor_campo_anualidad_form_control = array_form_control(i).TEXTO_CAMPO_MODIFICADO
                End If
            Next
            Dim ret_valor_bsd_nombre As String = ""
            Dim ret_valor_bsd_identifcacion As String = ""
            Dim ret_valor_bsd_anualidad As String = ""
            '--------------------------------Solicita existencia peticionario por rodos los campos-----------------------------
            Result = Me.Solicita_existencia_peticionario_nombre_identifcacion_anualidad(valor_campo_nombre_form_control,
                                                                                        valor_campo_identificacion_form_control,
                                                                                        valor_campo_anualidad_form_control,
                                                                                        campo_consulta_nombre,
                                                                                        campo_identificacion,
                                                                                        campo_anualidad,
                                                                                        campo_primary_key,
                                                                                        nombre_plantillas,
                                                                                        primary_peticionario,
                                                                                        ret_valor_bsd_nombre,
                                                                                        ret_valor_bsd_identifcacion,
                                                                                        ret_valor_bsd_anualidad)
            If Result <> "YES" Then
                Valida_exitencia_usuario_peticionario = Result
                Exit Function
            End If
            '-------Autoriza la actualización del usuario
            If primary_peticionario <> 0 Then
                Valida_exitencia_usuario_peticionario = "YES"
                Exit Function
            End If
            Result = Me.Solicita_exitencia_nombre_peticionario(valor_campo_nombre_form_control,
                                                               campo_consulta_nombre,
                                                               campo_identificacion,
                                                               campo_anualidad,
                                                               campo_primary_key,
                                                               nombre_plantillas,
                                                               primary_peticionario,
                                                               ret_valor_bsd_nombre,
                                                               ret_valor_bsd_identifcacion,
                                                               ret_valor_bsd_anualidad)
            If Result <> "YES" Then
                Valida_exitencia_usuario_peticionario = Result
                Exit Function
            End If
            '-----------------------Valida caso nombre de usuario no identificado
            If primary_peticionario = 0 Then

                Result = Me.Solicita_exitencia_numero_identificacion_peticionario(valor_campo_identificacion_form_control,
                                                                                  campo_consulta_nombre,
                                                                                  campo_identificacion,
                                                                                  campo_anualidad,
                                                                                  campo_primary_key,
                                                                                  nombre_plantillas,
                                                                                  primary_peticionario,
                                                                                  ret_valor_bsd_nombre,
                                                                                  ret_valor_bsd_identifcacion,
                                                                                  ret_valor_bsd_anualidad)
                If Result <> "YES" Then
                    Valida_exitencia_usuario_peticionario = Result
                    Exit Function
                End If
                '-------Autoriza la creación del usuario
                If primary_peticionario = 0 Then
                    Valida_exitencia_usuario_peticionario = "YES"
                    Exit Function
                Else
                    Valida_exitencia_usuario_peticionario = "Esta intentando registrar le nombre (" & valor_campo_nombre_form_control & ") con el numero de identificación (" & ret_valor_bsd_identifcacion & ") para que pueda coincidir su registro debe remplazarlo con el nombre (" & ret_valor_bsd_nombre & "). Desea que el sistema lo actualice por usted?"
                    campo_error = LCase(campo_consulta_nombre)
                    valor_campo_error = ret_valor_bsd_nombre
                    Exit Function
                End If
            End If
            '-----------------------Valida caso nombre de usuario  identificado-----------
            If primary_peticionario <> 0 Then
                If ret_valor_bsd_identifcacion <> valor_campo_identificacion_form_control Then

                    campo_error = LCase(campo_identificacion)
                    valor_campo_error = ret_valor_bsd_identifcacion
                    Valida_exitencia_usuario_peticionario = "Esta intentando registrar la identificación (" & valor_campo_identificacion_form_control & ") para el usuario (" & valor_campo_nombre_form_control & ") para que pueda coincidir su registro debe remplazarlo con el numero de identicacion  (" & ret_valor_bsd_identifcacion & "), esta de acuerdo con replazar el valor ?"
                    Exit Function
                End If
                If ret_valor_bsd_anualidad <> valor_campo_anualidad_form_control And ret_valor_bsd_anualidad <> "" Then
                    campo_error = LCase(campo_anualidad)
                    valor_campo_error = ret_valor_bsd_anualidad
                    Valida_exitencia_usuario_peticionario = "Esta intentando registrar la anualidad " & valor_campo_anualidad_form_control & " para el usuario (" & ret_valor_bsd_nombre & ") para que pueda coincidir su registro debe remplazar con la anualidad  (" & ret_valor_bsd_anualidad & "), desea remplazar el valor?"
                    Exit Function
                End If
            End If
            Valida_exitencia_usuario_peticionario = "YES"
        Catch ex As Exception
            Valida_exitencia_usuario_peticionario = "Inconsistencia general funcion Valida_exitencia_usuario_peticionario " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_peticionario_nombre_identifcacion_anualidad(ByVal valor_campo_nombre As String,
                                                                             ByVal valor_campo_identificacion As String,
                                                                             ByVal valor_campo_anualidad As String,
                                                                             ByVal campo_consulta_nombre As String,
                                                                             ByVal campo_idntificiacion As String,
                                                                             ByVal campo_anualidad As String,
                                                                             ByVal campo_primary_key As String,
                                                                             ByVal nombre_plantilla_validacion As String,
                                                                             ByRef primary_peticionario As Integer,
                                                                             ByRef ret_valor_bsd_nombre As String,
                                                                             ByRef ret_valor_bsd_identifcacion As String,
                                                                             ByRef ret_valor_bsd_anualidad As String) As String
        Try
            Dim Sql_consulta As String = "Select " & campo_primary_key & "," & campo_consulta_nombre & "," & campo_idntificiacion &
                "," & campo_anualidad & " from " & nombre_plantilla_validacion & " where " & campo_consulta_nombre & "='" &
                valor_campo_nombre & "' and " & campo_idntificiacion & "='" & valor_campo_identificacion & "' and " &
                campo_anualidad & "='" & valor_campo_anualidad & "'"
            Dim Result As String = ""
            Dim Dat_reader As New DataSet
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Sql_consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_existencia_peticionario_nombre_identifcacion_anualidad = " Error Solicita_existencia_peticionario_nombre_identifcacion_anualidad  " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                primary_peticionario = 0
                Solicita_existencia_peticionario_nombre_identifcacion_anualidad = "YES"
                Exit Function
            Else
                primary_peticionario = Dat_reader.Tables(0).Rows(0).Item(0)
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = False Then
                    ret_valor_bsd_nombre = Dat_reader.Tables(0).Rows(0).Item(1)
                Else
                    ret_valor_bsd_nombre = ""
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(2) = False Then
                    ret_valor_bsd_identifcacion = Dat_reader.Tables(0).Rows(0).Item(2)
                Else
                    ret_valor_bsd_identifcacion = ""
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(3) = False Then
                    ret_valor_bsd_anualidad = Dat_reader.Tables(0).Rows(0).Item(3)
                Else
                    ret_valor_bsd_anualidad = ""
                End If
                Solicita_existencia_peticionario_nombre_identifcacion_anualidad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_peticionario_nombre_identifcacion_anualidad = "Inconsistencia general funcion Solicita_existencia_peticionario_nombre_identifcacion_anualidad " & ex.Message
        End Try
    End Function
    Function Solicita_exitencia_nombre_peticionario(ByVal valor_campo_nombre As String,
                                                    ByVal campo_consulta_nombre As String,
                                                    ByVal campo_idntificiacion As String,
                                                    ByVal campo_anualidad As String,
                                                    ByVal campo_primary_key As String,
                                                    ByVal nombre_plantilla_validacion As String,
                                                    ByRef primary_peticionario As Integer,
                                                    ByRef ret_valor_bsd_nombre As String,
                                                    ByRef ret_valor_bsd_identifcacion As String,
                                                    ByRef ret_valor_bsd_anualidad As String) As String
        Try
            Dim Sql_consulta As String = "Select " & campo_primary_key & "," & campo_consulta_nombre & "," & campo_idntificiacion &
               "," & campo_anualidad & " from " & nombre_plantilla_validacion & " where " & campo_consulta_nombre & "='" &
               valor_campo_nombre & "'"
            Dim Result As String = ""
            Dim Dat_reader As New DataSet
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Sql_consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_exitencia_nombre_peticionario = " Error Solicita_exitencia_nombre_peticionario  " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                primary_peticionario = 0
                Solicita_exitencia_nombre_peticionario = "YES"
                Exit Function
            Else
                primary_peticionario = Dat_reader.Tables(0).Rows(0).Item(0)
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = False Then
                    ret_valor_bsd_nombre = Dat_reader.Tables(0).Rows(0).Item(1)
                Else
                    ret_valor_bsd_nombre = ""
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(2) = False Then
                    ret_valor_bsd_identifcacion = Dat_reader.Tables(0).Rows(0).Item(2)
                Else
                    ret_valor_bsd_identifcacion = ""
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(3) = False Then
                    ret_valor_bsd_anualidad = Dat_reader.Tables(0).Rows(0).Item(3)
                Else
                    ret_valor_bsd_anualidad = ""
                End If
                Solicita_exitencia_nombre_peticionario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_exitencia_nombre_peticionario = "Inconsistencia general funcion Solicita_exitencia_nombre_peticionario " & ex.Message
        End Try
    End Function
    Function Solicita_exitencia_numero_identificacion_peticionario(ByVal valor_campo_identificacion As String,
                                                                   ByVal campo_consulta_nombre As String,
                                                                   ByVal campo_idntificiacion As String,
                                                                   ByVal campo_anualidad As String,
                                                                   ByVal campo_primary_key As String,
                                                                   ByVal nombre_plantilla_validacion As String,
                                                                   ByRef primary_peticionario As Integer,
                                                                   ByRef ret_valor_bsd_nombre As String,
                                                                   ByRef ret_valor_bsd_identifcacion As String,
                                                                   ByRef ret_valor_bsd_anualidad As String) As String
        Try
            Dim Sql_consulta As String = "Select " & campo_primary_key & "," & campo_consulta_nombre & "," & campo_idntificiacion &
               "," & campo_anualidad & " from " & nombre_plantilla_validacion & " where " & campo_idntificiacion & "='" &
               valor_campo_identificacion & "'"
            Dim Result As String = ""
            Dim Dat_reader As New DataSet
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Sql_consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_exitencia_numero_identificacion_peticionario = " Error Solicita_exitencia_nombre_peticionario  " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                primary_peticionario = 0
                Solicita_exitencia_numero_identificacion_peticionario = "YES"
                Exit Function
            Else
                primary_peticionario = Dat_reader.Tables(0).Rows(0).Item(0)
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = False Then
                    ret_valor_bsd_nombre = Dat_reader.Tables(0).Rows(0).Item(1)
                Else
                    ret_valor_bsd_nombre = ""
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(2) = False Then
                    ret_valor_bsd_identifcacion = Dat_reader.Tables(0).Rows(0).Item(2)
                Else
                    ret_valor_bsd_identifcacion = ""
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(3) = False Then
                    ret_valor_bsd_anualidad = Dat_reader.Tables(0).Rows(0).Item(3)
                Else
                    ret_valor_bsd_anualidad = ""
                End If
                Solicita_exitencia_numero_identificacion_peticionario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_exitencia_numero_identificacion_peticionario = "Inconsistencia general funcion Solicita_exitencia_numero_identificacion_peticionario " & ex.Message
        End Try
    End Function
    Function Lista_anulidad_usuario_peticionario(ByRef dropW_list As DropDownList) As String
        Try
            dropW_list.Items.Clear()
            Dim anual_permitido = System.DateTime.Now.Year - 19
            dropW_list.Items.Add("")
            For i As Integer = 1950 To anual_permitido
                dropW_list.Items.Add(i)
            Next
            Lista_anulidad_usuario_peticionario = "YES"
        Catch ex As Exception
            Lista_anulidad_usuario_peticionario = "Inconistencia general funcion Lista_anulidad_usuario_peticionario " & ex.Message
        End Try
    End Function
    'Function Solicita_valor_primary_plantilla_validacion(ByVal nombre_campo_primary As String,
    '                                                     ByVal nombre_plantilla_validacion As String,
    '                                                     ByRef valor_campo_primary As Object) As String
    '    '---------------------------------------------------------------------------
    '    'Funcion : Solicita el valor del campo de la llave primaria de la plantilla
    '    '          de validacion
    '    '---------------------------------------------------------------------------
    '    '                           PARAMETROS  
    '    '---------------------------------------------------------------------------
    '    'nombre_plantilla_validacion : Representa el nombre de plantilla de validacion
    '    'nombre_campo_primary        : Campo plantilla validacion
    '    '---------------------------------------------------------------------------
    '    '                           RETORNO
    '    '---------------------------------------------------------------------------
    '    'valor_campo_primary  : Retorna la idnetificación del usuario radicador
    '    '---------------------------------------------------------------------------
    '    '                         CARACTERIZACIÓN
    '    '---------------------------------------------------------------------------
    '    'Fecha                 : 2023-10-17
    '    'Elabora               : Miguel Angel Urueta Miranda
    '    '----------------------------------------------------------------------------
    '    Try
    '        Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
    '        Dim Parametro_Consulta As String = "SELECT " & nombre_campo_primary & " FROM where " & nombre_campo_primary & "="

    '        Dim Datset As New DataSet
    '        Dim Result As String = ""
    '        Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
    '        If Result <> "YES" Then
    '            Solicita_valor_primary_plantilla_validacion = " Error Listando nombre plantilla VALIDACION   " & Result
    '            Exit Function
    '        End If
    '        If Datset.Tables(0).Rows.Count = 0 Then
    '            Solicita_valor_primary_plantilla_validacion = "Imposible encontrar el nombre de la plantilla validacion"
    '            Exit Function
    '        Else
    '            Nombre_Plantilla = Datset.Tables(0).Rows(0).Item(0).ToString
    '            Retorna_Nombre_Plantilla_Validacion = "YES"
    '            Exit Function
    '        End If
    '    Catch ex As Exception
    '        Solicita_valor_primary_plantilla_validacion = "Inconsistencia general funcion Solicita_valor_primary_plantilla_validacion " & ex.Message
    '    End Try
    'End Function
End Class
