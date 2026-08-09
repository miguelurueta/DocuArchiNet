Imports AjaxControlToolkit
Imports MySql.Data.MySqlClient
Imports Neodynamic.SDK.Web
Imports System.IO
Imports System.Drawing
Imports System.Globalization
Imports GemBox
'Imports GemBox.Document.Tables
Imports System.Xml
Imports Org.BouncyCastle.Asn1
Imports iTextSharp.text
Imports Newtonsoft.Json

Public Class ClassPqrs
    Function Guardar_documento_texto_pqr(ByVal texto_documento As String,
                                         ByVal ruta_documento As String) As String
        Try
            GemBox.Document.ComponentInfo.SetLicense("DTFX-JTBY-6RJK-Y101")
            If File.Exists(ruta_documento) = True Then
                Kill(ruta_documento)
            End If
            Dim document As New GemBox.Document.DocumentModel

            document.Content.LoadText(texto_documento)
            document.Save(ruta_documento, GemBox.Document.PdfSaveOptions.PdfDefault)
            Guardar_documento_texto_pqr = "YES"
        Catch ex As Exception
            Guardar_documento_texto_pqr = "Inconsistencia general función Guardar_documento_texto_pqr " & ex.Message
        End Try
    End Function
    Function Listar_Tipos_Documentales_pqrs(ByRef RefCombo As DropDownList,
                                            ByVal id_plantilla As Integer) As String
        Try

            RefCombo.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Descripcion_Doc from tipo_doc_entrante where system_plantilla_radicado_id_plantilla=" &
                id_plantilla & " and tipo_activo_pqr=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Listar_Tipos_Documentales_pqrs = " Error Listando tipos documentales   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_Tipos_Documentales_pqrs = "YES"
                Exit Function
            Else
                RefCombo.Items.Add("SELECCIONE")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    RefCombo.Items.Add(Datset.Tables(0).Rows(i).Item(0).ToString)
                Next
                Listar_Tipos_Documentales_pqrs = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_Tipos_Documentales_pqrs = ex.Message
        End Try
    End Function
    Function Retorna_area_usuario_pqr_asignado(ByVal id_organigrama As Integer,
                                               ByVal nombre_area As String,
                                               ByRef codigo_area As Integer,
                                               ByRef id_usuario_gestión As Integer,
                                               ByRef nombre_usuario_gestion As String,
                                               ByRef cargo_usuario_gestion As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Codigo_Area,rdi.id_Remit_Dest_Int,rdi.Nombre_Remitente,rdi.Cargo_Remite from areas_depart_radicacion as adr " &
                "inner join remit_dest_interno as rdi on (rdi.Areas_Dep_Radicacion_id_Areas_Dep=adr.Codigo_Area and rdi.estado_usuario_para_gestion_pqr=1)" &
                "where Registro_Organigrama_Id_Organigrama=" & id_organigrama & " and Nombre_Area='" & nombre_area & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_area_usuario_pqr_asignado = "Error listando usuarios destinatarios internos " & Result
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count = 0 Then
                    Retorna_area_usuario_pqr_asignado = "Impisible encontrar el usuario de gestión para asignar el pqr "
                    Exit Function
                Else
                    codigo_area = Datset.Tables(0).Rows(0).Item(0)
                    id_usuario_gestión = Datset.Tables(0).Rows(0).Item(1)
                    nombre_usuario_gestion = Datset.Tables(0).Rows(0).Item(2)
                    cargo_usuario_gestion = Datset.Tables(0).Rows(0).Item(3)
                    Retorna_area_usuario_pqr_asignado = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Retorna_area_usuario_pqr_asignado = "Inconsistencia general función Retorna_area_usuario_pqr_asignado " & ex.Message
        End Try
    End Function

    Function Consulta_existencia_usuario_pqrs_registrado(ByVal nombre_plantilla_validacion As String,
                                                         ByVal nombre_campo_consulta As String,
                                                         ByVal valor_consulta As String,
                                                         ByRef retorna_valor_campo As String,
                                                         ByRef existencia_registro As String,
                                                         ByRef campo_null_registro As String,
                                                         ByVal nombre_campo_anualidad As String,
                                                         ByRef resultado_campo_anualidad As String,
                                                         ByVal nombre_campo_dext As String,
                                                         ByRef valor_campo_dext As String) As String
        '-----------------------------------------------------------------------------------------------------------
        'Función : Rertona existencia del campo registrado de valida la existencia del numero de cedula
        'ción de la plantilla valores devueltos existencia_registro YES existe registro, NO no existe registro
        'campo_null_registro YES campo nulo , NO 
        'Ing Miguel Angel Urueta Miranda
        'Fecha : 2016-11-25
        '------------------------------------------------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select " & nombre_campo_consulta & "," & nombre_campo_anualidad & "," & nombre_campo_dext & " from " & nombre_plantilla_validacion & " where " & nombre_campo_consulta & "='" &
            valor_consulta & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Consulta_existencia_usuario_pqrs_registrado = " Error función Consulta_existencia_usuario_pqrs_registrado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia_registro = "NO"
                Consulta_existencia_usuario_pqrs_registrado = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    retorna_valor_campo = ""
                    campo_null_registro = "YES"
                Else
                    retorna_valor_campo = Datset.Tables(0).Rows(0).Item(0)
                    campo_null_registro = "NO"
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    resultado_campo_anualidad = ""
                Else
                    resultado_campo_anualidad = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    valor_campo_dext = ""
                Else
                    valor_campo_dext = Datset.Tables(0).Rows(0).Item(2)
                End If
                existencia_registro = "YES"
                Consulta_existencia_usuario_pqrs_registrado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_existencia_usuario_pqrs_registrado = "Error General Función  Consulta_existencia_usuario_pqrs_registrado " & ex.Message
        End Try
    End Function

    Function Retorna_nombre_campo_nit_pqr_validacion(ByVal id_plantilla_validacion As Integer,
                                                     ByRef nombre_campo_nit As String) As String
        '************************************************************
        'Función : Rertona el nombre del campo nit de la plantilla
        'de validación
        'Ing Miguel Angel Urueta Miranda
        'Fecha : 2016-11-24
        '************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Nombre_Campo from campos_plantilla_validacion where Plantilla_Validacion_Id_Plantilla_Validacion='" &
            id_plantilla_validacion & "' AND Campo_Iidenti_pqr=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_campo_nit_pqr_validacion = " Error Listando campo nit pqr   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_campo_nit_pqr_validacion = "No hay campo nit activo para PQRS "
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count > 1 Then
                    Retorna_nombre_campo_nit_pqr_validacion = "El sistema detecto varios campos nit  activos para PQRS imposible continuar "
                    Exit Function
                End If
                nombre_campo_nit = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_campo_nit_pqr_validacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_campo_nit_pqr_validacion = "Error General Función  Retorna_nombre_campo_nit_pqr_validacion " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_campo_dest_pqr_validacion(ByVal id_plantilla_validacion As Integer,
                                                      ByRef nombre_campo_idext As String) As String
        '************************************************************
        'Función : Rertona el nombre del campo nit de la plantilla
        'de validación
        'Ing Miguel Angel Urueta Miranda
        'Fecha : 2016-11-24
        '************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Nombre_Campo from campos_plantilla_validacion where Plantilla_Validacion_Id_Plantilla_Validacion='" &
            id_plantilla_validacion & "' AND Campo_Primari_key=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_campo_dest_pqr_validacion = " Error Listando campo nit pqr   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_campo_dest_pqr_validacion = "No hay campo identi activo para PQRS "
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count > 1 Then
                    Retorna_nombre_campo_dest_pqr_validacion = "El sistema detecto varios campos identi  activos para PQRS imposible continuar "
                    Exit Function
                End If
                nombre_campo_idext = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_campo_dest_pqr_validacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_campo_dest_pqr_validacion = "Error General Función  Retorna_nombre_campo_dest_pqr_validacion " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_campo_anualidad_pqr_validacion(ByVal id_plantilla_validacion As Integer,
                                                           ByRef nombre_campo_nit As String) As String
        '************************************************************
        'Función : Rertona el nombre del campo nit de la plantilla
        'de validación
        'Ing Miguel Angel Urueta Miranda
        'Fecha : 2016-11-24
        '************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Nombre_Campo from campos_plantilla_validacion where Plantilla_Validacion_Id_Plantilla_Validacion='" &
            id_plantilla_validacion & "' AND Campo_anualidad_pqr=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_campo_anualidad_pqr_validacion = " Error Listando campo anualidad pqr   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_campo_anualidad_pqr_validacion = "No hay campo anualidad activo para PQRS "
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count > 1 Then
                    Retorna_nombre_campo_anualidad_pqr_validacion = "El sistema detecto varios campos anualidad activos para PQRS imposible continuar "
                    Exit Function
                End If
                nombre_campo_nit = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_campo_anualidad_pqr_validacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_campo_anualidad_pqr_validacion = "Error General Función  Retorna_nombre_campo_anualidad_pqr_validacion " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_codigo_plantilla(ByRef codigo_plantilla As Integer,
                                             ByRef nombre_plantilla As String) As String
        '******************************************************
        'Funcion : Verifica_Existencia_Destinatario_Ext_Guia
        'Ing Miguel Angel Urueta Miranda
        'Descripcion : Verifica la existencia de plantilla
        'de radicación ublicada para pqrs
        'Fecha : 2016-11-24
        '******************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select id_Plantilla,Nombre_Plantilla_Radicado from system_plantilla_radicado where Util_activo_plantilla_pqr='" &
            "1" & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_codigo_plantilla = " Error Listando plantilla predeterminada para pqrs   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_codigo_plantilla = "No hay plantilla activa para el registro de PQRS "
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count > 1 Then
                    Retorna_nombre_codigo_plantilla = "El sistema detecto varias plantillas activas para PQRS imposible continuar "
                    Exit Function
                End If
                codigo_plantilla = Datset.Tables(0).Rows(0).Item(0)
                nombre_plantilla = Datset.Tables(0).Rows(0).Item(1)
                Retorna_nombre_codigo_plantilla = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_nombre_codigo_plantilla = "Error General Función  Retorna_nombre_codigo_plantilla " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_codigo_plantilla_validacion(ByVal Codigo_Plantilla As Integer,
                                                        ByRef campo_comparacion As String,
                                                        ByRef id_plantilla_validacion As Integer,
                                                        ByRef nombre_plantilla_validacion As String,
                                                        ByRef codigo_script As Integer) As String
        '-----------------------------------------------------------
        'Función : Rertorna el nombre de la plantilla de validación
        'relacionada con la plantilla de radicación
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-11-24
        '-----------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRadicador
            Dim Estado_opcion_fecha As Integer = 0
            Dim Estado_opcion_cita_respuesta As Integer = 0
            Dim Estado_opcion_radicado_general As Integer = 0
            Dim Estado_opcion_valida_externo As Integer = 0
            Dim Estado_opcion_radicado_codigo_corto As Integer = 0
            Dim Util_activo_plantilla_codigo_simple As Integer = 0
            '---------------------------------------
            '------Lista opciones plantilla
            '---------------------------------------
            Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(Codigo_Plantilla,
                                                                                           Estado_opcion_fecha,
                                                                                           Estado_opcion_cita_respuesta,
                                                                                           Estado_opcion_radicado_general,
                                                                                           Estado_opcion_valida_externo,
                                                                                           Estado_opcion_radicado_codigo_corto,
                                                                                           Util_activo_plantilla_codigo_simple)
            If Result <> "YES" Then
                Retorna_nombre_codigo_plantilla_validacion = Result
                Exit Function
            End If
            Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
            Dim Matri_Datos() As Campos_Plantilla
            Erase Matri_Datos
            Result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(Codigo_Plantilla,
                                                                                                Matri_Datos,
                                                                                                Estado_opcion_fecha,
                                                                                                Estado_opcion_cita_respuesta,
                                                                                                Estado_opcion_radicado_general)
            If Result <> "YES" Then
                Retorna_nombre_codigo_plantilla_validacion = Result
                Exit Function
            End If
            Dim ref_Class_ra_script_actividades As New Class_ra_script_actividades
            Dim matri() As validacion_plantilla
            Erase matri
            Result = ref_Class_ra_script_actividades.lista_campos_Validacion_plantilla(Codigo_Plantilla,
                                                                                       matri)
            If Result <> "YES" Then
                Retorna_nombre_codigo_plantilla_validacion = Result
                Exit Function
            End If

            If Not matri Is Nothing Then
                For i2 As Integer = 0 To matri.Length - 1
                    For i3 As Integer = 0 To Matri_Datos.Length - 1
                        If Matri_Datos(i3).Campo_Plantilla = matri(i2).Campo_Plantilla Then
                            Matri_Datos(i3).TIPO_SCRIPT = matri(i2).TIPO_SCRIPT
                            Matri_Datos(i3).COMBINACION_TECLA = matri(i2).COMBINACION_TECLA
                            Matri_Datos(i3).VALOR_SCRIPT = matri(i2).VALOR_SCRIPT
                            Matri_Datos(i3).ESTADO_ESCRIPT = matri(i2).ESTADO_ESCRIPT
                            Matri_Datos(i3).PLATAFORMA_SCRIPT = matri(i2).PLATAFORMA_SCRIPT
                            Matri_Datos(i3).ID_SCRIPT = matri(i2).ID_SCRIPT

                        End If
                    Next
                Next
            End If
            '***************************************************************************
            'Verifcia existencia destinatario externo
            '***************************************************************************
            'Dim codigo_script As Integer = 0
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_Plantilla = "REMITENTE_COR" Then
                    If Matri_Datos(i).ID_SCRIPT = 0 Then
                        Retorna_nombre_codigo_plantilla_validacion = "El sistema ha detectado que el campo remitente no tiene una plantilla de validación relacionada por favor seleccione una plantilla"
                        Exit Function
                    Else
                        codigo_script = Matri_Datos(i).ID_SCRIPT
                    End If

                End If
            Next
            Dim Ref_Class_relacion_script_plantilla As New Class_relacion_script_plantilla
            If codigo_script <> 0 Then
                Result = Ref_Class_relacion_script_plantilla.retorna_campo_compracion_plantilla(codigo_script,
                                                                                                campo_comparacion,
                                                                                                nombre_plantilla_validacion,
                                                                                                id_plantilla_validacion)
                If Result <> "YES" Then
                    Retorna_nombre_codigo_plantilla_validacion = Result
                    Exit Function
                End If
                Retorna_nombre_codigo_plantilla_validacion = "YES"
                Exit Function
            Else
                Retorna_nombre_codigo_plantilla_validacion = "Imposible encontrar el código de plantilla de validación del  plantilla radicado id " & Codigo_Plantilla
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_codigo_plantilla_validacion = "Inconsistencia general función Retorna_nombre_codigo_plantilla_validacion " & ex.Message
        End Try
    End Function
    Function Retorna_datos_atension_pqrs(ByVal nombre_configuracion As String,
                                         ByRef resultado_configuracion As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select texto_configuracion_pqr " &
             "  from ra_config_pqrs  " &
            " where nombre_configuracion ='" & nombre_configuracion & "'"
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_datos_atension_pqrs = " Error listando datos configuración pqrs  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Retorna_datos_atension_pqrs = "No se encontraron datos para la configuración : " & nombre_configuracion
                Exit Function
            Else
                resultado_configuracion = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_datos_atension_pqrs = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_datos_atension_pqrs = "inconsistencia general función Retorna_datos_atension_pqrs " & ex.Message
        End Try
    End Function

    Function Asigna_datos_nit_anualidad_plantilla_validacion_pqr(ByRef Page1 As Page,
                                                                 ByVal id_script As Integer) As String
        Try
            Dim Result As String = ""
            Dim refclas As New ClassRadicador
            Dim ref_consulta As New ClassRaConsultaRadicados
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR
            Erase Matri_Datos
            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(id_script,
                                                                                                Matri_Datos)
            If Result <> "YES" Then
                Asigna_datos_nit_anualidad_plantilla_validacion_pqr = Result
                Exit Function
            End If
            Dim ref_TextBox_nit_identificacion As Object = Page1.FindControl("TextBox_nit_identificacion")
            If ref_TextBox_nit_identificacion Is Nothing Then
                Asigna_datos_nit_anualidad_plantilla_validacion_pqr = "Imposible encontrar el control ref_TextBox_nit_identificacion"
                Exit Function
            End If
            Dim ref_DropDownList_anualidad As Object = Page1.FindControl("DropDownList_anualidad")
            If ref_TextBox_nit_identificacion Is Nothing Then
                Asigna_datos_nit_anualidad_plantilla_validacion_pqr = "Imposible encontrar el control DropDownList_anualidad"
                Exit Function
            End If
            Dim ref_UpdatePanelContenido As UpdatePanel = Page1.FindControl("UpdatePanelContenido")
            If ref_UpdatePanelContenido Is Nothing Then
                Asigna_datos_nit_anualidad_plantilla_validacion_pqr = "Imposible encontrar el control ref_UpdatePanelContenido"
                Exit Function
            End If
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_Iidenti_pqr = 1 Then
                    Dim text_campo_nit As TextBox = Page1.FindControl(UCase(Matri_Datos(i).Nombre_Campo))
                    If Not text_campo_nit Is Nothing Then
                        text_campo_nit.Text = ref_TextBox_nit_identificacion.text
                    End If

                End If
                If Matri_Datos(i).Campo_anualidad_pqr = 1 Then
                    Dim text_campo_anualidad As TextBox = Page1.FindControl(UCase(Matri_Datos(i).Nombre_Campo))
                    If Not text_campo_anualidad Is Nothing Then
                        text_campo_anualidad.Text = ref_DropDownList_anualidad.text
                    End If
                End If
            Next
            ref_UpdatePanelContenido.Update()
            Asigna_datos_nit_anualidad_plantilla_validacion_pqr = "YES"
        Catch ex As Exception
            Asigna_datos_nit_anualidad_plantilla_validacion_pqr = "Incosistencia función Asigna_datos_nit_anualidad_plantilla_validacion_pqr " & ex.Message
        End Try
    End Function
    Function Actualiza_anualidad_usuario_pqr(ByVal id_script As Integer,
                                             ByVal nombre_usuario As String,
                                             ByVal nit_usuario As String,
                                             ByVal anualidad As String,
                                             ByRef id_remit_dest_int As Integer) As String
        Try
            Dim Result As String = ""
            Dim refclas As New ClassRadicador
            Dim ref_consulta As New ClassRaConsultaRadicados
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Erase Matri_Datos
            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(id_script,
                                                                                             Matri_Datos)
            If Result <> "YES" Then
                Actualiza_anualidad_usuario_pqr = Result
                Exit Function
            End If
            '****************************************************
            'Retorna nombre plantilla
            '****************************************************
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Actualiza_anualidad_usuario_pqr = Result
                Exit Function
            End If
            Dim campo_consulta_nombre As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_nombre_pqr = 1 Then
                    campo_consulta_nombre = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_consulta_nombre = "" Then
                Actualiza_anualidad_usuario_pqr = "El sistema no registra un campo nombre contacte a su administrador"
                Exit Function
            End If
            Dim campo_nit As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_Iidenti_pqr = 1 Then
                    campo_nit = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_nit = "" Then
                Actualiza_anualidad_usuario_pqr = "El sistema no registra un campo nit contacte a su administrador "
                Exit Function
            End If
            Dim campo_anualidad As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_anualidad_pqr = 1 Then
                    campo_anualidad = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_anualidad = "" Then
                Actualiza_anualidad_usuario_pqr = "El sistema no registra un campo campo_anualidad contacte a su administrador "
                Exit Function
            End If
            Dim campo_idex As String = ""
            '*****************************************************
            'Busca campo identi en la mtriz campos
            '*****************************************************
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    campo_idex = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_idex = "" Then
                Actualiza_anualidad_usuario_pqr = "La plantilla carece de campo identi por favor agregelo en el módulo administración"
                Exit Function
            End If
            '-----------------------------------------------------
            'Verifica existencia campo nombre
            '-----------------------------------------------------

            Dim valor_clave_usuario As Integer = -1
            Result = Class_plantilla_validacion.Valida_existencia_nombre_usuario_pqr(campo_consulta_nombre,
                                                                                     nombre_usuario,
                                                                                     campo_idex,
                                                                                     nombre_plantillas,
                                                                                     valor_clave_usuario)
            If Result <> "YES" Then
                Actualiza_anualidad_usuario_pqr = Result
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Parametro_Consulta As String = "UPDATE " & nombre_plantillas & " SET " & campo_anualidad & "='" & anualidad & "' where " & campo_nit & "='" & nit_usuario & "' AND " & campo_consulta_nombre & "='" & nombre_usuario & "'"
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(Parametro_Consulta)
            If Result <> "YES" Then
                Actualiza_anualidad_usuario_pqr = " Error actualizando  " & Result
                Exit Function
            End If
            id_remit_dest_int = valor_clave_usuario
            Actualiza_anualidad_usuario_pqr = "YES"
            Exit Function
        Catch ex As Exception
            Actualiza_anualidad_usuario_pqr = "Inconsistencia general función Actualiza_anualidad_usuario_pqr " & ex.Message
        End Try
    End Function
    Function Recupera_anualidad_usuario_pqrs(ByVal id_script As Integer,
                                             ByVal nit As String,
                                             ByVal correo_electronico As String) As String
        Try
            Dim Result As String = ""
            Dim refclas As New ClassRadicador
            Dim ref_consulta As New ClassRaConsultaRadicados
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Erase Matri_Datos
            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(id_script,
                                                                                             Matri_Datos)
            If Result <> "YES" Then
                Recupera_anualidad_usuario_pqrs = Result
                Exit Function
            End If
            '****************************************************
            'Retorna nombre plantilla
            '****************************************************
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Recupera_anualidad_usuario_pqrs = Result
                Exit Function
            End If
            Dim campo_consulta_nombre As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_nombre_pqr = 1 Then
                    campo_consulta_nombre = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_consulta_nombre = "" Then
                Recupera_anualidad_usuario_pqrs = "El sistema no registra un campo nombre contacte a su administrador"
                Exit Function
            End If
            Dim campo_nit As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_Iidenti_pqr = 1 Then
                    campo_nit = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_nit = "" Then
                Recupera_anualidad_usuario_pqrs = "El sistema no registra un campo nit contacte a su administrador "
                Exit Function
            End If
            Dim campo_anualidad As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_anualidad_pqr = 1 Then
                    campo_anualidad = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_anualidad = "" Then
                Recupera_anualidad_usuario_pqrs = "El sistema no registra un campo anualidad contacte a su administrador "
                Exit Function
            End If
            Dim campo_correo_electronico As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_correo_electrnico_pqr = 1 Then
                    campo_correo_electronico = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_correo_electronico = "" Then
                Recupera_anualidad_usuario_pqrs = "El sistema no registra un campo correo electrónico contacte a su administrador "
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Parametro_Consulta As String = "Select " & campo_consulta_nombre & "," & campo_anualidad & " from " & nombre_plantillas & " where " & campo_nit & "='" & nit & "' and " &
                campo_correo_electronico & "='" & correo_electronico & "'"
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Recupera_anualidad_usuario_pqrs = " Error consultando anualidad usuario pqrs  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                Dim refclas_correo As New ClassCorreo
                Dim anualidad As String = ""
                Dim _nombre As String = ""
                If Dat_reader.Tables(0).Rows(0).IsNull(1) Then
                    anualidad = ""
                Else
                    anualidad = Dat_reader.Tables(0).Rows(0).Item(1)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(0) Then
                    _nombre = ""
                Else
                    _nombre = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                Dim asunto As String = "PQRS Recuperación año de nacimiento " & anualidad
                Dim matri_mensaje() As String = {"En nuestro sistema se registro que su año de nacimento es (" & anualidad & ") a nombre de " & _nombre & ".", " El año (" & anualidad & ") que recibe en este correo electrónico le permitirá  ingresar al sistema junto con su número de identificación " & nit}
                Dim matri_documentos() As String = Nothing
                Result = refclas_correo.Envio_Correo_confirmacion_solicitud_aprobacion_respuesta(matri_mensaje,
                                                                                                 correo_electronico,
                                                                                                 asunto,
                                                                                                 matri_documentos)
                If Result <> "YES" Then
                    Recupera_anualidad_usuario_pqrs = Result
                Else
                    Recupera_anualidad_usuario_pqrs = "YES"
                    Exit Function
                End If
            Else
                Recupera_anualidad_usuario_pqrs = "El correo electrónico no pertenece al numero de identificación, por favor contacte al area de sistemas o tecnología de la entidad"
                Exit Function
            End If
        Catch ex As Exception
            Recupera_anualidad_usuario_pqrs = "Inconsistencia general función Recupera_anualidad_usuario_pqrs " & ex.Message
        End Try
    End Function
    Function Lista_nombre_usuarios_pqr(ByVal id_script As Integer,
                                       ByRef drolist As DropDownList,
                                       ByRef update_panel As UpdatePanel,
                                       ByVal nit As String) As String
        Try
            Dim Result As String = ""
            Dim refclas As New ClassRadicador
            Dim ref_consulta As New ClassRaConsultaRadicados
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Erase Matri_Datos
            drolist.Items.Clear()
            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(id_script,
                                                                                             Matri_Datos)
            If Result <> "YES" Then
                Lista_nombre_usuarios_pqr = Result
                Exit Function
            End If
            '****************************************************
            'Retorna nombre plantilla
            '****************************************************
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Lista_nombre_usuarios_pqr = Result
                Exit Function
            End If
            Dim campo_consulta_nombre As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_nombre_pqr = 1 Then
                    campo_consulta_nombre = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_consulta_nombre = "" Then
                Lista_nombre_usuarios_pqr = "El sistema no registra un campo nombre contacte a su administrador"
                Exit Function
            End If
            Dim campo_nit As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_Iidenti_pqr = 1 Then
                    campo_nit = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_nit = "" Then
                Lista_nombre_usuarios_pqr = "El sistema no registra un campo identificación contacte a su administrador "
                Exit Function
            End If
            Dim campo_anualidad As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_anualidad_pqr = 1 Then
                    campo_anualidad = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_anualidad = "" Then
                Lista_nombre_usuarios_pqr = "El sistema no registra un campo campo de anualidad contacte a su administrador "
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Parametro_Consulta As String = "Select " & campo_consulta_nombre & " from " & nombre_plantillas & " where " & campo_nit & "='" & nit & "'"
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_nombre_usuarios_pqr = " Error listando  nombres de usuario  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    drolist.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0))
                Next
                update_panel.Update()
                Lista_nombre_usuarios_pqr = "YES"
                Exit Function
            Else
                update_panel.Update()
                Lista_nombre_usuarios_pqr = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_nombre_usuarios_pqr = "Inconsistencia general funcion Lista_nombre_usuarios_pqr " & ex.Message
        End Try
    End Function
    Function Genera_Interface_Gestion_Plantilla_Validacion_pqr(ByRef Page1 As Page,
                                                               ByVal id_script As Integer) As String
        Try
            Dim Result As String = ""
            Dim refclas As New ClassRadicador
            Dim ref_consulta As New ClassRaConsultaRadicados
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Erase Matri_Datos
            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(id_script,
                                                                                               Matri_Datos)
            If Result <> "YES" Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = Result
                Exit Function
            End If
            '****************************************************
            'Retorna nombre plantilla
            '****************************************************
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = Result
                Exit Function
            End If
            Dim _LabelboxIco As Label() = {}
            Dim m_TextBoxes() As TextBox = {}
            Dim _image() As ImageButton = {}
            Dim LabelBox() As Label = {}
            Dim _ComboBox() As DropDownList = {}
            Dim _CommamdBoton() As Button = {}
            Dim Contador_Control As Integer = 0
            Dim Contador_Combo As Integer = 0
            Dim Contador_Text As Integer = 0
            Dim objRowlibre As TableRow
            objRowlibre = New TableRow
            Dim z2 As Integer = 0
            Dim Update As UpdatePanel = Page1.FindControl("UpdatePanelContenido")
            If Update Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = "Imposible encontrar el control UpdatePanelContenido"
                Exit Function
            End If
            Update.UpdateMode = UpdatePanelUpdateMode.Conditional
            Dim Table As Table = Page1.FindControl("_ValidacionConsulta")
            If Update Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = "Imposible encontrar el control _ValidacionConsulta"
                Exit Function
            End If
            Table.Controls.Clear()
            Dim Panelref As Panel = Page1.FindControl("_Panelvalidacion")
            If Update Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = "Imposible encontrar el control _Panelvalidacion"
                Exit Function
            End If
            Dim hiden_edit_Hiddenestadoedicion As Object = Page1.FindControl("Hiddenestadoedicion")
            If hiden_edit_Hiddenestadoedicion Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = "Imposible encontrar el control Hiddenestadoedicion"
                Exit Function
            End If
            Dim ref_TextBox_nit_identificacion As Object = Page1.FindControl("TextBox_nit_identificacion")
            If ref_TextBox_nit_identificacion Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = "Imposible encontrar el control ref_TextBox_nit_identificacion"
                Exit Function
            End If
            Dim ref_DropDownList_anualidad As Object = Page1.FindControl("DropDownList_anualidad")
            If ref_TextBox_nit_identificacion Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = "Imposible encontrar el control DropDownList_anualidad"
                Exit Function
            End If

            Dim Panel_ex3_tabs_1 As Panel = Page1.FindControl("Panel_ex3_tabs_1")
            If Panel_ex3_tabs_1 Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = "Imposible encontrar el control Panel_ex3_tabs_1"
                Exit Function
            End If
            Dim Panel_ex3_tabs_2 As Panel = Page1.FindControl("Panel_ex3_tabs_2")
            If Panel_ex3_tabs_2 Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion_pqr = "Imposible encontrar el control Panel_ex3_tabs_2"
                Exit Function
            End If
            Dim objRow As TableRow
            Dim objCell As TableCell
            Dim largo As Integer = 200
            Dim largocombo As Integer = 150
            Dim largocombinado As Integer = 80
            Dim contador_columna As Integer = 0
            Dim divhtml_content As New HtmlControls.HtmlGenericControl("div")
            divhtml_content.Attributes.Add("class", "w-100")
            Dim divhtml_objRow As HtmlControls.HtmlGenericControl = Nothing
            Dim divhtml_objCell As HtmlControls.HtmlGenericControl = Nothing
            Dim aleas_campo As String = ""
            Dim obligatorio As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                aleas_campo = Matri_Datos(i).Aleas_Campo_pqr
                If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO <> 1 Then
                    aleas_campo = Matri_Datos(i).Aleas_Campo_pqr
                    If Matri_Datos(i).Tipo_Campo = "DATE" Or Matri_Datos(i).Tipo_Campo = "INT" Then

                        If Matri_Datos(i).Obligatorio_Campo = 1 Then
                            obligatorio = "*"
                        Else
                            obligatorio = ""
                        End If
                        objRow = New TableRow
                        objCell = New TableCell
                        divhtml_objRow = New HtmlControls.HtmlGenericControl("div")
                        divhtml_objRow.Attributes.Add("class", "row pt-2 pb-2")
                        divhtml_objCell = New HtmlControls.HtmlGenericControl("div")
                        divhtml_objCell.Attributes.Add("class", "col-6")
                        ReDim Preserve _LabelboxIco(Contador_Control)
                        _LabelboxIco(Contador_Control) = New Label
                        If Matri_Datos(i).Aleas_Campo_pqr = "" Then
                            _LabelboxIco(Contador_Control).Text = UCase(Matri_Datos(i).Nombre_Campo) & obligatorio
                        Else
                            _LabelboxIco(Contador_Control).Text = UCase((Matri_Datos(i).Aleas_Campo_pqr)) & obligatorio
                        End If
                        If Matri_Datos(i).Obligatorio_Campo = 1 Then
                            _LabelboxIco(Contador_Control).ForeColor = Drawing.Color.Red
                        Else
                            _LabelboxIco(Contador_Control).ForeColor = Drawing.Color.Black
                        End If
                        _LabelboxIco(Contador_Control).Font.Name = "Arial"
                        _LabelboxIco(Contador_Control).ID = Matri_Datos(i).Nombre_Campo & i
                        '-----Agrega control label
                        divhtml_objCell.Controls.Add(_LabelboxIco(Contador_Control))
                        divhtml_objRow.Controls.Add(divhtml_objCell)
                        divhtml_objCell = New HtmlControls.HtmlGenericControl("div")
                        divhtml_objCell.Attributes.Add("class", "col-6")
                        '--------------------------------------------------------
                        '-------Agrega controles tipo text box
                        '--------------------------------------------------------
                        If Matri_Datos(i).tipo_iteractua_campo = 1 Then
                            ReDim Preserve m_TextBoxes(Contador_Control)
                            m_TextBoxes(Contador_Control) = New TextBox
                            m_TextBoxes(Contador_Control).ID = UCase(Matri_Datos(i).Nombre_Campo)
                            m_TextBoxes(Contador_Control).Attributes.Add("class", "w-100")
                            m_TextBoxes(Contador_Control).Attributes.Add("class", "form-control")

                            '------------------------------------------------
                            'Asigna letra capital al nombre
                            '------------------------------------------------
                            If Matri_Datos(i).Campo_nombre_pqr = 1 Then
                                m_TextBoxes(Contador_Control).Attributes.Add("Class", "tranforn_capital_text")
                            End If
                            '------------------------------------------------
                            'Asigna el nit 
                            '------------------------------------------------
                            If Matri_Datos(i).Campo_Iidenti_pqr = 1 Then
                                m_TextBoxes(Contador_Control).Text = ref_TextBox_nit_identificacion.text
                                m_TextBoxes(Contador_Control).Enabled = False
                            End If
                            If Matri_Datos(i).Campo_anualidad_pqr = 1 Then
                                m_TextBoxes(Contador_Control).Text = ref_DropDownList_anualidad.text
                                m_TextBoxes(Contador_Control).Enabled = False
                            End If

                            divhtml_objCell.Controls.Add(m_TextBoxes(Contador_Control))
                            divhtml_objRow.Controls.Add(divhtml_objCell)

                            '-------------------------------------------------
                            'Agrega imagen al campo date
                            '-------------------------------------------------
                            If Matri_Datos(i).Tipo_Campo = "DATE" Then
                                ReDim Preserve _image(Contador_Control)
                                _image(Contador_Control) = New ImageButton
                                _image(Contador_Control).ID = UCase(Matri_Datos(i).Nombre_Campo) & "_Image"
                                _image(Contador_Control).AlternateText = "#"
                                _image(Contador_Control).Height = 20
                                _image(Contador_Control).Width = 20
                                _image(Contador_Control).ImageAlign = ImageAlign.Bottom
                                _image(Contador_Control).BackColor = Drawing.Color.Blue
                                _image(Contador_Control).ImageUrl = "../imagera/Calendar.png"
                                divhtml_objCell.Controls.Add(_image(Contador_Control))
                                'objCell.Controls.Add(_image(Contador_Control))
                                '-----Label espacio 
                                _LabelboxIco(Contador_Control) = New Label
                                _LabelboxIco(Contador_Control).Text = "F"
                                'objCell.Controls.Add(_LabelboxIco(Contador_Control))
                                divhtml_objCell.Controls.Add(_LabelboxIco(Contador_Control))
                                '-------------------------------------------
                                '-----Agregar calendar al boton imagen
                                '-------------------------------------------
                                Result = refclas.Agregar_Calendar(_image(Contador_Control).ID.ToString, m_TextBoxes(Contador_Control).ID.ToString, Panelref)
                                If Result <> "YES" Then
                                    Genera_Interface_Gestion_Plantilla_Validacion_pqr = Result
                                    Exit Function
                                End If
                            End If
                            divhtml_objRow.Controls.Add(divhtml_objCell)
                            'objRow.Cells.Add(objCell)
                        Else
                            '----Asigna campo drowplis
                            ReDim Preserve _ComboBox(Contador_Control)
                            _ComboBox(Contador_Control) = New DropDownList
                            _ComboBox(Contador_Control).Width = largocombinado
                            _ComboBox(Contador_Control).ID = Matri_Datos(i).Nombre_Campo
                            _ComboBox(Contador_Control).Attributes.Add("class", "w-100")
                            _ComboBox(Contador_Control).Attributes.Add("class", "form-control")
                            divhtml_objCell.Controls.Add(_ComboBox(Contador_Control))
                            divhtml_objRow.Controls.Add(divhtml_objCell)
                        End If
                    Else
                        '---------------------------------Campos tipo texto -----------------------------------------
                        If Matri_Datos(i).Obligatorio_Campo = 1 Then
                            obligatorio = "*"
                        Else
                            obligatorio = ""
                        End If
                        '------------AGREGA ROW 
                        divhtml_objRow = New HtmlControls.HtmlGenericControl("div")
                        divhtml_objRow.Attributes.Add("class", "row pt-2 pb-2")
                        '------------AGREGA CELL
                        divhtml_objCell = New HtmlControls.HtmlGenericControl("div")
                        divhtml_objCell.Attributes.Add("class", "col-6")
                        ReDim Preserve _LabelboxIco(Contador_Control)
                        _LabelboxIco(Contador_Control) = New Label
                        _LabelboxIco(Contador_Control).Attributes.Add("class", "h6 tranforn_capital_text")
                        If Matri_Datos(i).Aleas_Campo_pqr = "" Then
                            _LabelboxIco(Contador_Control).Text = Matri_Datos(i).Nombre_Campo & obligatorio
                        Else
                            _LabelboxIco(Contador_Control).Text = Matri_Datos(i).Aleas_Campo_pqr & obligatorio
                        End If
                        _LabelboxIco(Contador_Control).ID = Matri_Datos(i).Nombre_Campo & i
                        '----------AGREGA LABEL
                        divhtml_objCell.Controls.Add(_LabelboxIco(Contador_Control))
                        divhtml_objRow.Controls.Add(divhtml_objCell)
                        If Matri_Datos(i).tipo_iteractua_campo = 1 Then
                            '--//Agrega atributos a controles tipo INPUT
                            ReDim Preserve m_TextBoxes(Contador_Control)
                            m_TextBoxes(Contador_Control) = New TextBox
                            m_TextBoxes(Contador_Control).ID = LCase(Matri_Datos(i).Nombre_Campo)
                            divhtml_objCell = New HtmlControls.HtmlGenericControl("div")
                            divhtml_objCell.Attributes.Add("class", "col-6")
                            divhtml_objCell.Controls.Add(m_TextBoxes(Contador_Control))
                            divhtml_objRow.Controls.Add(divhtml_objCell)
                            m_TextBoxes(Contador_Control).Attributes.Add("ref_aleas_campo", UCase(aleas_campo))
                            m_TextBoxes(Contador_Control).Attributes.Add("ref_campo_obligatorio", obligatorio)
                            m_TextBoxes(Contador_Control).Attributes.Add("ref_tipo_campo", "text")

                            '------------------Agrega campos al grupo datos del solicitante
                            If Matri_Datos(i).tipo_agrupacion_campo = 1 Then
                                m_TextBoxes(Contador_Control).Attributes.Add("class", "w-100 form-control rel-campo-solicitante")
                                Panel_ex3_tabs_1.Controls.Add(divhtml_objRow)
                            End If
                            '------------------Agrega campos al grupo datos de contacto
                            If Matri_Datos(i).tipo_agrupacion_campo = 2 Then
                                m_TextBoxes(Contador_Control).Attributes.Add("class", "w-100 form-control rel-campo-contacto")
                                Panel_ex3_tabs_2.Controls.Add(divhtml_objRow)
                            End If
                            '------------------Asigna el atributo de correo electronico
                            If Matri_Datos(i).Campo_correo_electrnico_pqr = 1 Then
                                m_TextBoxes(Contador_Control).Attributes.Add("ref_tipo_campo_correo", "1")
                            End If
                            Contador_Control = Contador_Control + 1
                        Else
                            '--//Agrega atributos a controles tipo SELECT
                            ReDim Preserve _ComboBox(Contador_Control)
                            _ComboBox(Contador_Control) = New DropDownList
                            _ComboBox(Contador_Control).ID = LCase(Matri_Datos(i).Nombre_Campo)
                            divhtml_objCell = New HtmlControls.HtmlGenericControl("div")
                            divhtml_objCell.Attributes.Add("class", "col-6")
                            divhtml_objCell.Controls.Add(_ComboBox(Contador_Control))
                            divhtml_objRow.Controls.Add(divhtml_objCell)
                            _ComboBox(Contador_Control).Attributes.Add("ref_aleas_campo", UCase(aleas_campo))
                            _ComboBox(Contador_Control).Attributes.Add("ref_campo_obligatorio", obligatorio)
                            _ComboBox(Contador_Control).Attributes.Add("ref_tipo_campo", "combo")
                            '------------------Agrega campos al grupo datos del solicitante
                            If Matri_Datos(i).tipo_agrupacion_campo = 1 Then
                                _ComboBox(Contador_Control).Attributes.Add("class", "w-100 form-control rel-campo-solicitante")
                                Panel_ex3_tabs_1.Controls.Add(divhtml_objRow)
                            End If
                            '------------------Agrega campos al grupo datos de contacto
                            If Matri_Datos(i).tipo_agrupacion_campo = 2 Then
                                _ComboBox(Contador_Control).Attributes.Add("class", "w-100 form-control rel-campo-contacto")
                                Panel_ex3_tabs_2.Controls.Add(divhtml_objRow)
                            End If
                            '------------------Agrega datos de clasificación de la identificación
                            If Matri_Datos(i).Campo_clasifcacion_identificacion_pqr = 1 Then
                                Dim Class_ra_val_externo_clasic_identificacion As New Class_ra_val_externo_clasic_identificacion
                                Result = Class_ra_val_externo_clasic_identificacion.Solicita_lista_clasific_identificacion(_ComboBox(Contador_Control))
                                If Result <> "" Then
                                    If Result <> "YES" Then
                                        Genera_Interface_Gestion_Plantilla_Validacion_pqr = Result
                                        Exit Function
                                    End If
                                End If
                            End If
                            '------------------Agrega datos de clasificación de tipo de población
                            If Matri_Datos(i).Campo_clasificacion_poblacion_pqr = 1 Then
                                Dim Class_ra_val_externo_clasic_poblacion As New Class_ra_val_externo_clasic_poblacion
                                Result = Class_ra_val_externo_clasic_poblacion.Solicita_lista_clasific_poblacion(_ComboBox(Contador_Control))
                                If Result <> "" Then
                                    If Result <> "YES" Then
                                        Genera_Interface_Gestion_Plantilla_Validacion_pqr = Result
                                        Exit Function
                                    End If
                                End If
                            End If
                            '------------------Agrega datos de clasificación de sexo
                            If Matri_Datos(i).Campo_clasificacion_sexo_pqr = 1 Then
                                Dim Class_ra_val_externo_clasic_sexo As New Class_ra_val_externo_clasic_sexo
                                Result = Class_ra_val_externo_clasic_sexo.Solicita_lista_clasific_sexo(_ComboBox(Contador_Control))
                                If Result <> "" Then
                                    If Result <> "YES" Then
                                        Genera_Interface_Gestion_Plantilla_Validacion_pqr = Result
                                        Exit Function
                                    End If
                                End If
                            End If
                            '-----Agrega campo pais
                            If Matri_Datos(i).Nombre_Campo = "pais" Then
                                Dim Class_pais_radicacion As New Class_pais_radicacion
                                Result = Class_pais_radicacion.Lista_Paises(0,
                                                                            _ComboBox(Contador_Control),
                                                                            Update)
                                If Result <> "YES" Then
                                    Genera_Interface_Gestion_Plantilla_Validacion_pqr = Result
                                    Exit Function
                                End If
                                Dim nombre_campo As String = LCase(Matri_Datos(i).Nombre_Campo)
                                _ComboBox(Contador_Control).Attributes.Add("onchange", "event_add_departamento('" & nombre_campo & "');")
                            End If
                            '-----Agrega municipio a departamnento
                            If Matri_Datos(i).Nombre_Campo = "departemento" Then
                                Dim nombre_campo As String = LCase(Matri_Datos(i).Nombre_Campo)
                                _ComboBox(Contador_Control).Attributes.Add("onchange", "event_add_municipio('" & nombre_campo & "');")
                            End If
                            '------------------Agrega datos de anualidad
                            If Matri_Datos(i).Campo_anualidad_pqr = 1 Then
                                Class_plantilla_validacion.Lista_anulidad_usuario_peticionario(_ComboBox(Contador_Control))
                            End If
                            If Matri_Datos(i).Campo_correo_electrnico_pqr = 1 Then
                                _ComboBox(Contador_Control).Attributes.Add("ref_tipo_campo_correo", "1")
                            End If
                            Contador_Control = Contador_Control + 1
                        End If
                    End If
                End If
            Next
            Genera_Interface_Gestion_Plantilla_Validacion_pqr = "YES"
            Exit Function
        Catch ex As Exception
            Genera_Interface_Gestion_Plantilla_Validacion_pqr = "Inconsistencia función Genera_Interface_Gestion_Plantilla_Validacion_pqr " & ex.Message
        End Try
    End Function
    Function generar_campos_ubicacion(ByRef _ComboBox() As DropDownList,
                                      ByRef objcell As TableCell,
                                      ByRef objRow As TableRow,
                                      ByRef Table As Object,
                                      ByRef Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR,
                                      ByRef contador As Integer,
                                      ByRef _LabelboxIco() As Label) As String
        Try
            Dim largocombo As Integer = 150
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Nombre_Campo = "Pais" Or Matri_Datos(i).Nombre_Campo = "Municipio" _
                       Or Matri_Datos(i).Nombre_Campo = "Departemento" Then
                    objRow = New TableRow
                    objcell = New TableCell
                    ReDim Preserve _LabelboxIco(contador)
                    _LabelboxIco(contador) = New Label
                    _LabelboxIco(contador).Text = UCase(Matri_Datos(i).Nombre_Campo)
                    _LabelboxIco(contador).ID = Matri_Datos(i).Nombre_Campo & i
                    _LabelboxIco(contador).ForeColor = Drawing.Color.Black
                    _LabelboxIco(contador).Font.Size = 8
                    _LabelboxIco(contador).Font.Name = "Arial"
                    objcell.Controls.Add(_LabelboxIco(contador))
                    objRow.Cells.Add(objcell)
                    Table.Rows.Add(objRow)
                    ReDim Preserve _ComboBox(contador)
                    objRow = New TableRow
                    objcell = New TableCell
                    _ComboBox(contador) = New DropDownList
                    _ComboBox(contador).ID = UCase(Matri_Datos(i).Nombre_Campo)
                    _ComboBox(contador).Width = largocombo
                    Select Case Matri_Datos(i).Nombre_Campo
                        Case "Pais"
                            _ComboBox(contador).Attributes.Add("onchange", "llenardepartamento();")
                        Case "Municipio"
                            _ComboBox(contador).Attributes.Add("onchange", "seleccionmuicipio();")
                        Case "Departemento"
                            _ComboBox(contador).Attributes.Add("onchange", "llenarciudad();")
                    End Select

                    objcell.Controls.Add(_ComboBox(contador))
                    objRow.Cells.Add(objcell)
                    Table.Rows.Add(objRow)

                End If

            Next

            generar_campos_ubicacion = "YES"
        Catch ex As Exception
            generar_campos_ubicacion = "Inconsistencia general funcion " & ex.Message
        End Try
    End Function
    Function Lista_campos_caracterizacion_usuario_pqr_matriz(ByVal idscipt As Integer, ByVal codigo_usuario As Integer,
        ByRef datos_documentos() As String) As String
        Try
            Dim refclas_radicado As New ClassRadicador
            Dim Result As String = ""
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Erase Matri_Datos
            Dim campo_idex As String = ""
            '****************************************************
            'Lista campos plantilla validación
            '****************************************************
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(idscipt,
                                                                                               Matri_Datos)
            If Result <> "YES" Then
                Lista_campos_caracterizacion_usuario_pqr_matriz = Result
                Exit Function
            End If

            '*****************************************************
            'Busca campo identi en la mtriz campos
            '*****************************************************
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    campo_idex = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_idex = "" Then
                Lista_campos_caracterizacion_usuario_pqr_matriz = "La plantilla carece de campo identi por favor agregelo en el módulo administración"
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna nombre plantilla validación
            '---------------------------------------------------
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(idscipt,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Lista_campos_caracterizacion_usuario_pqr_matriz = Result
                Exit Function
            End If
            Dim seleccion As String = "Select "
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 0 And Matri_Datos(i).Campo_anualidad_pqr = 0 And Matri_Datos(i).Visible_Campo = 1 Then
                    If seleccion = "Select " Then
                        seleccion = seleccion & Matri_Datos(i).Nombre_Campo
                    Else
                        seleccion = seleccion & "," & Matri_Datos(i).Nombre_Campo
                    End If
                End If
            Next
            Dim Parametro_Consulta As String = seleccion & " from " & nombre_plantillas & " where " & campo_idex & "='" &
                       codigo_usuario & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_campos_caracterizacion_usuario_pqr_matriz = " Error verificando existencia de usuario pqr  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_campos_caracterizacion_usuario_pqr_matriz = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Dat_reader.Tables(0).Columns.Count - 1
                    If Dat_reader.Tables(0).Rows(0).IsNull(i) = True Then
                        ReDim Preserve datos_documentos(i)
                        datos_documentos(i) = UCase(Dat_reader.Tables(0).Columns(i).ColumnName) & "|" & ""
                    Else
                        ReDim Preserve datos_documentos(i)
                        datos_documentos(i) = UCase(Dat_reader.Tables(0).Columns(i).ColumnName) & "|" & UCase(Dat_reader.Tables(0).Rows(0).Item(i))
                    End If
                Next
                Lista_campos_caracterizacion_usuario_pqr_matriz = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_campos_caracterizacion_usuario_pqr_matriz = "Inconsistencia general función Lista_campos_caracterizacion_usuario_pqr " & ex.Message
        End Try
    End Function
    Function Lista_campos_caracterizacion_usuario_pqr(ByVal idscipt As Integer, ByVal codigo_usuario As Integer,
        ByRef datos_documentos As String) As String
        Try
            Dim refclas_radicado As New ClassRadicador
            Dim Result As String = ""
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Erase Matri_Datos
            Dim campo_idex As String = ""
            '****************************************************
            'Lista campos plantilla validación
            '****************************************************
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(idscipt,
                                                                                             Matri_Datos)
            If Result <> "YES" Then
                Lista_campos_caracterizacion_usuario_pqr = Result
                Exit Function
            End If

            '*****************************************************
            'Busca campo identi en la mtriz campos
            '*****************************************************
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    campo_idex = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_idex = "" Then
                Lista_campos_caracterizacion_usuario_pqr = "La plantilla carece de campo identi por favor agregelo en el módulo administración"
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna nombre plantilla validación
            '---------------------------------------------------
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(idscipt,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Lista_campos_caracterizacion_usuario_pqr = Result
                Exit Function
            End If
            Dim seleccion As String = "Select "
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 0 And Matri_Datos(i).Campo_anualidad_pqr = 0 And Matri_Datos(i).Visible_Campo = 1 Then
                    If seleccion = "Select " Then
                        seleccion = seleccion & Matri_Datos(i).Nombre_Campo
                    Else
                        seleccion = seleccion & "," & Matri_Datos(i).Nombre_Campo
                    End If
                End If
            Next
            Dim Parametro_Consulta As String = seleccion & " from " & nombre_plantillas & " where " & campo_idex & "='" &
                       codigo_usuario & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_campos_caracterizacion_usuario_pqr = " Error verificando existencia de usuario pqr  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_campos_caracterizacion_usuario_pqr = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Dat_reader.Tables(0).Columns.Count - 1

                    If Dat_reader.Tables(0).Rows(0).IsNull(i) = True Then

                        datos_documentos = datos_documentos & UCase(Dat_reader.Tables(0).Columns(i).ColumnName) & "=" & "" & vbCrLf
                    Else
                        datos_documentos = datos_documentos & UCase(Dat_reader.Tables(0).Columns(i).ColumnName) & "=" & Dat_reader.Tables(0).Rows(0).Item(i) & vbCrLf
                    End If
                Next
                Lista_campos_caracterizacion_usuario_pqr = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_campos_caracterizacion_usuario_pqr = "Inconsistencia general función Lista_campos_caracterizacion_usuario_pqr " & ex.Message
        End Try
    End Function
    Function Lista_campos_nit_nombre_usuario_pqr(ByVal idscipt As Integer, ByVal codigo_usuario As Integer,
        ByRef nombre As String, ByRef nit_identificacion As String, ByRef anualidad As String) As String
        Try
            Dim refclas_radicado As New ClassRadicador
            Dim Result As String = ""
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Erase Matri_Datos
            Dim campo_idex As String = ""
            '****************************************************
            'Lista campos plantilla validación
            '****************************************************
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(idscipt,
                                                                                             Matri_Datos)
            If Result <> "YES" Then
                Lista_campos_nit_nombre_usuario_pqr = Result
                Exit Function
            End If

            '*****************************************************
            'Busca campo identi en la mtriz campos
            '*****************************************************
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    campo_idex = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_idex = "" Then
                Lista_campos_nit_nombre_usuario_pqr = "La plantilla carece de campo identi por favor agregelo en el módulo administración"
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna nombre plantilla validación
            '---------------------------------------------------
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(idscipt,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Lista_campos_nit_nombre_usuario_pqr = Result
                Exit Function
            End If
            Dim campo_consulta_nombre As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_nombre_pqr = 1 Then
                    campo_consulta_nombre = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_consulta_nombre = "" Then
                Lista_campos_nit_nombre_usuario_pqr = "El sistema no registra un campo nombre contacte a su administrador"
                Exit Function
            End If
            Dim campo_nit As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_Iidenti_pqr = 1 Then
                    campo_nit = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_nit = "" Then
                Lista_campos_nit_nombre_usuario_pqr = "El sistema no registra un campo nit contacte a su administrador "
                Exit Function
            End If
            Dim campo_anualidad As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_anualidad_pqr = 1 Then
                    campo_anualidad = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_anualidad = "" Then
                Lista_campos_nit_nombre_usuario_pqr = "El sistema no registra un campo campo_anualidad contacte a su administrador "
                Exit Function
            End If
            Dim Parametro_Consulta As String = "Select " & campo_nit & "," & campo_consulta_nombre & "," & campo_anualidad & " from " & nombre_plantillas & " where " & campo_idex & "='" &
                    codigo_usuario & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_campos_nit_nombre_usuario_pqr = " Error verificando existencia nombre usuario  " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_campos_nit_nombre_usuario_pqr = "YES"
                Exit Function
            Else
                If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                    nit_identificacion = ""
                Else
                    nit_identificacion = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = True Then
                    nombre = ""
                Else
                    nombre = Dat_reader.Tables(0).Rows(0).Item(1)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(2) = True Then
                    anualidad = ""
                Else
                    anualidad = Dat_reader.Tables(0).Rows(0).Item(2)
                End If
                Lista_campos_nit_nombre_usuario_pqr = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_campos_nit_nombre_usuario_pqr = "Inconsistencia general función Lista_campos_nit_nombre_usuario_pqr " & ex.Message
        End Try
    End Function
    Function Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr(ByVal pag1 As Page,
                                                                     ByVal idscipt As Integer,
                                                                     ByRef id_remit_dest As Integer) As String
        '********************************************************************************
        'Funcion : Recupera los campos de la pagina web direccionado por la matriz
        'de de tipo CAMPOS_PLANTILLA_VALIDACION el sistema determina los cambios
        'realizados sobre los campos en el formulario, verifica los campos unicos,
        'campos obligatorios, determina la estrucutura de los campos de ubicacion
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2014-08-02
        '*********************************************************************************
        Try
            '--------------------------------------------------------------------------------
            'ubicacion de campos en la pagina web y comprobacion de existencia
            '--------------------------------------------------------------------------------
            'Dim update As UpdatePanel = pag1.FindControl("UpdatePanelContenido")
            Dim hide As Object = pag1.FindControl("hdnEmailID_VAL")

            If hide Is Nothing Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "Imposible el control hdnEmailID en la funcion Agregar_Nuevo_Registro_plantilla_validacion"
                Exit Function
            End If
            Dim hidepais As Object = pag1.FindControl("Hiddenselecionpais")
            If hidepais Is Nothing Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "Imposible el control Hiddenselecionpais en la funcion Agregar_Nuevo_Registro_plantilla_validacion"
                Exit Function
            End If
            Dim hidepartamento As Object = pag1.FindControl("Hiddenseleciondepartamento")
            If hidepartamento Is Nothing Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "Imposible el control Hiddenselecionpais en la funcion Agregar_Nuevo_Registro_plantilla_validacion"
                Exit Function
            End If
            Dim hidemunicipio As Object = pag1.FindControl("Hiddenmunicipio")
            If hidemunicipio Is Nothing Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "Imposible el control hidemunicipio en la funcion Agregar_Nuevo_Registro_plantilla_validacion"
                Exit Function
            End If

            Dim refclas_radicado As New ClassRadicador
            Dim Result As String = ""
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION_PQR
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Erase Matri_Datos
            Dim campo_idex As String = ""
            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Result = Class_campos_plantilla_validacion.Solicita_Campos_Plantilla_Validacion_pqr(idscipt,
                                                                                             Matri_Datos)
            If Result <> "YES" Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = Result
                Exit Function
            End If
            '*****************************************************
            'Busca campo identi en la mtriz campos
            '*****************************************************
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    campo_idex = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_idex = "" Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "La plantilla carece de campo identi por favor agregelo en el módulo administración"
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna nombre plantilla validación
            '---------------------------------------------------
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(idscipt,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = Result
                Exit Function
            End If
            '--------------------------------------------------------
            'Asignacion a los campos de formulario
            '--------------------------------------------------------
            Dim estado_ubicacion As Integer = -1
            For i As Integer = 0 To Matri_Datos.Length - 1
                '----------------------------------------------------------
                'Asignacion de de datos de la intrface a la matriz
                '---------------------------------------------------------
                If Matri_Datos(i).Nombre_Campo = "Pais" Or Matri_Datos(i).Nombre_Campo = "Municipio" _
                               Or Matri_Datos(i).Nombre_Campo = "Departemento" Then
                    Select Case Matri_Datos(i).Nombre_Campo
                        Case "Pais"
                            Dim ob As Object = pag1.FindControl(UCase(Matri_Datos(i).Nombre_Campo))
                            If Not ob Is Nothing Then
                                Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = hidepais.value
                                'hidepais.value = ob.text
                                estado_ubicacion = 1
                            End If
                        Case "Departemento"
                            Dim ob As Object = pag1.FindControl(UCase(Matri_Datos(i).Nombre_Campo))
                            If Not ob Is Nothing Then
                                Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = hidepartamento.value
                                'hidepais.value = ob.text
                                estado_ubicacion = 1
                            End If

                        Case "Municipio"
                            Dim ob As Object = pag1.FindControl(UCase(Matri_Datos(i).Nombre_Campo))
                            If Not ob Is Nothing Then
                                Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = hidemunicipio.value
                                'hidepais.value = ob.text
                                estado_ubicacion = 1
                            End If

                    End Select

                Else
                    Dim ob As Object = pag1.FindControl(UCase(Matri_Datos(i).Nombre_Campo))
                    If Not ob Is Nothing Then
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = ob.text
                    End If
                End If
            Next
            '---------------------------------------------------------------------------------
            'Valida los formatos fecha
            '---------------------------------------------------------------------------------
            Dim ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO <> 1 Then
                    If Matri_Datos(i).Tipo_Campo = "DATE" And Matri_Datos(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                        Result = ClassGestionFechas.Verifi_campo_fecha(Matri_Datos(i).TEXTO_CAMPO_MODIFICADO)
                        If Result <> "YES" Then
                            Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "El formato fecha no cumple " & Result
                            Exit Function
                        End If
                    End If
                End If
            Next
            '---------------------------------------------------------------------------------
            'Valida campos obligatorios vacios
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO <> 1 And Matri_Datos(i).Obligatorio_Campo = 1 Then
                    If Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = "" Then
                        Dim ob As Object = pag1.FindControl(UCase(Matri_Datos(i).Nombre_Campo))
                        If Not ob Is Nothing Then
                            ob.focus()
                        End If
                        Dim ref_campo As String = ""
                        If Matri_Datos(i).Aleas_Campo_pqr = "" Then
                            ref_campo = Matri_Datos(i).Nombre_Campo
                        Else
                            ref_campo = Matri_Datos(i).Aleas_Campo_pqr
                        End If
                        Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "El campo " & ref_campo & " es obligatorio"
                        Exit Function

                    End If
                End If
            Next
            Dim campo_consulta_nombre As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Campo_nombre_pqr = 1 Then
                    campo_consulta_nombre = Matri_Datos(i).Nombre_Campo
                End If
            Next
            If campo_consulta_nombre = "" Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "El sistema no registra un campo (nombre) contacte a su administrador"
                Exit Function
            End If
            Dim text_nombre As TextBox = pag1.FindControl(campo_consulta_nombre)
            If text_nombre Is Nothing Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "Imposible encontrar el campo nombre (" & campo_consulta_nombre & ")"
                Exit Function
            End If
            '-----------------------------------------------------
            'Verifica existencia campo nombre
            '-----------------------------------------------------

            Dim valor_clave_usuario As Integer = -1
            Result = Class_plantilla_validacion.Valida_existencia_nombre_usuario_pqr(campo_consulta_nombre,
                                                                                     text_nombre.Text,
                                                                                     campo_idex,
                                                                                     nombre_plantillas,
                                                                                     valor_clave_usuario)
            If Result <> "YES" Then
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = Result
                Exit Function
            End If
            '---------------------------------------------------------------------------------
            'Valida campos ubicacion 
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO <> 1 And Matri_Datos(i).Obligatorio_Campo = 1 Then
                    If Matri_Datos(i).Nombre_Campo = "Pais" Or Matri_Datos(i).Nombre_Campo = "Municipio" _
                       Or Matri_Datos(i).Nombre_Campo = "Departemento" Then
                        If Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = "" Or Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = "SELECCIONE" Then
                            Dim ob As Object = pag1.FindControl(UCase(Matri_Datos(i).Nombre_Campo))
                            If Not ob Is Nothing Then
                                ob.focus()
                            End If
                            Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "El campo " & Matri_Datos(i).Nombre_Campo & " es obligatorio"
                            Exit Function

                        End If
                    End If
                End If
            Next
            '---------------------------------------------------------------------------------
            'Verifica existencia de los campos unicos
            '---------------------------------------------------------------------------------
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO <> 1 Then
                    If Matri_Datos(i).Unico_campo = 1 And Matri_Datos(i).Campo_nombre_pqr = 0 Then
                        Dim sql As String = "Select " & Matri_Datos(i).Nombre_Campo & " from " & nombre_plantillas &
                            " where " & Matri_Datos(i).Nombre_Campo & "='" & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & "'"
                        Result = Class_plantilla_validacion.Verifica_Existencia_Campos_Unico_Validacion(sql, Matri_Datos(i).Nombre_Campo,
                                                                                                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO,
                                                                                                        Matri_Datos(i).Aleas_Campo_pqr)
                        If Result <> "YES" Then
                            Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = Result
                            Exit Function
                        End If
                    End If

                End If
            Next
            '-------------------------------------------------------------
            'Actualiza los campos de letra capital, upercase o towupercase
            '-------------------------------------------------------------
            Dim clas_trd_documental As New ClassTrdDocumental
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO <> 1 And Matri_Datos(i).TEXTO_CAMPO_MODIFICADO <> "" Then
                    If Matri_Datos(i).valida_capital_text = 1 Then
                        clas_trd_documental.Formato_sub_serie(Matri_Datos(i).TEXTO_CAMPO_MODIFICADO,
                                                              Matri_Datos(i).TEXTO_CAMPO_MODIFICADO)
                    End If
                    If Matri_Datos(i).valida_capital_text = 2 Then
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = UCase(Matri_Datos(i).TEXTO_CAMPO_MODIFICADO)
                    End If
                    If Matri_Datos(i).valida_capital_text = 3 Then
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = LCase(Matri_Datos(i).TEXTO_CAMPO_MODIFICADO)
                    End If
                End If
            Next
            If valor_clave_usuario = -1 Then
                '---------------------------------------------------------------------------- -----
                'Genera comnando de insertcion
                '---------------------------------------------------------------------------------
                Dim sqlactualizacion As String = "INSERT INTO " & nombre_plantillas & " "
                Dim sqlcampos As String = ""
                Dim sqlvalores As String = ""
                '---------------------------------------------------------------------------------
                'Lista campos de insertcion
                '---------------------------------------------------------------------------------
                For i As Integer = 0 To Matri_Datos.Length - 1
                    If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO <> 1 Then

                        If sqlcampos = "" Then
                            sqlcampos = sqlcampos & "(" & Matri_Datos(i).Nombre_Campo
                        Else
                            sqlcampos = sqlcampos & " , " & Matri_Datos(i).Nombre_Campo
                        End If

                        If sqlvalores = "" Then
                            sqlvalores = sqlvalores & "(" & "'" & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & "'"
                        Else
                            sqlvalores = sqlvalores & " , " & "'" & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & "'"
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
                    Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = Result
                    Exit Function
                End If
                hide.value = lastinsert
                id_remit_dest = lastinsert
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "YES"
                Exit Function
            Else
                '---------------------------------------------------------------------------- -----
                'Genera comnando de actualización
                '---------------------------------------------------------------------------------
                Dim sqlactualizacion As String = "update " & nombre_plantillas & " "
                Dim sqlcondicion As String = " where " & campo_idex & "=" & valor_clave_usuario
                Dim sqlvalores As String = ""
                '---------------------------------------------------------------------------------
                'Lista campos de insertcion
                '---------------------------------------------------------------------------------
                For i As Integer = 0 To Matri_Datos.Length - 1
                    If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO <> 1 Then
                        Dim valor_campo_actualizacion As String = ""
                        If Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = "" Then
                            valor_campo_actualizacion = "null"
                        Else
                            valor_campo_actualizacion = "'" & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & "'"
                        End If
                        If sqlvalores = "" Then
                            sqlvalores = sqlvalores & "set " & Matri_Datos(i).Nombre_Campo & "=" & valor_campo_actualizacion
                        Else
                            sqlvalores = sqlvalores & " , " & Matri_Datos(i).Nombre_Campo & "=" & valor_campo_actualizacion
                        End If
                    End If
                Next
                sqlvalores = sqlvalores & sqlcondicion
                sqlactualizacion = sqlactualizacion & sqlvalores
                Dim lastinsert As Object = Nothing
                Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
                Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sqlactualizacion)
                If Result <> "YES" Then
                    Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = Result
                    Exit Function
                End If
                hide.value = valor_clave_usuario
                id_remit_dest = valor_clave_usuario
                Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr = "Inconsistencia funcion Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr " & ex.Message
        End Try
    End Function

    Function Guardar_Documento_pqr(ByRef Id_imagen As Integer,
                                   ByVal Nombre_Gabinete As String,
                                   ByVal id_registro_radicado As Integer,
                                   ByVal ruta_document As String,
                                   ByVal radicado_pqr As String,
                                   ByVal enlace_pqr As String,
                                   ByVal id_usario_gestion As Integer,
                                   ByVal id_tipo_tramite As Integer) As String
        Try
            Dim Refeclasaladir As New ClassAñadirDocumento
            Dim MatriDatosAlmacen() As String
            Erase MatriDatosAlmacen
            Dim Result As String = ""
            Dim Refalmacena As New ClassAlmacenamiento
            Dim option_unidad_conservacion As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(option_unidad_conservacion,
                                                                       Nombre_Gabinete)
            If Result <> "YES" Then
                Guardar_Documento_pqr = "Inconsistencia verficando opcón asignación unidad y expediente codigo:  " & Result
                Exit Function
            End If
            Dim ref_dig_tipos_docum_listachequeo As New ra_dig_tipos_docum_lista_chequeo
            Dim id_tipo_documental_lista As Integer = 0
            Result = ref_dig_tipos_docum_listachequeo.Solicita_id_lista_chequeo_default_radicado(id_tipo_tramite,
                                                                                                 id_tipo_documental_lista)
            If Result <> "YES" Then
                Guardar_Documento_pqr = Result
                Exit Function
            End If
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            Dim matri_gestion As estructure_gestion = Nothing
            If id_tipo_documental_lista <> 0 Then
                Result = Refalmacena.Solicita_datos_estructura_tipo_documento_lista_chequeo(id_tipo_documental_lista,
                                                                                            "DOCUMENTO ELECTRONICO",
                                                                                             matri_gestion)
                If Result <> "YES" Then
                    Guardar_Documento_pqr = Result
                    Exit Function
                End If
            Else
                matri_gestion.CLASE_DOCUMENTO = ""
                matri_gestion.EXPEDIENTE = ""
                matri_gestion.ID_AREA = 0
                matri_gestion.ID_CLASE_DOCUMENTO = 0
                matri_gestion.ID_EXPEDIENTE = 0
                matri_gestion.ID_SERIE = 0
                matri_gestion.ID_SUB_SERIE = 0
                matri_gestion.ID_TIPO_EXPEDIENTE = 0
                matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
                matri_gestion.ID_TIPODOCUMENTO = 0
                matri_gestion.ID_UNIDAD_CONSERVACION = 0
                matri_gestion.ID_USUARIO_GESTION = 0
                matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
                matri_gestion.UNIDAD_CONSERVACION = ""
                matri_gestion.FECHA_ELABORACION = ""
            End If
            Dim matri_datos() As Datos_Almacenamiento
            ReDim Preserve matri_datos(0)
            matri_datos(0).nombre_campo = "NUMERORADICA"
            matri_datos(0).valor_campo = radicado_pqr
            ReDim Preserve matri_datos(1)
            matri_datos(1).nombre_campo = "ENLASE"
            matri_datos(1).valor_campo = enlace_pqr
            If option_unidad_conservacion = 1 Then
                ReDim Preserve matri_datos(3)
                matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
                matri_datos(3).valor_campo = "DOCUMENTO ELECTRONICO"
                Dim date1al As String = Date.Today
                Result = ""
                Dim ref_ClassGestionFechas As New ClassGestionFechas
                Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
                If Result <> "YES" Then
                    Guardar_Documento_pqr = "Error formatenado fecha alamcenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                    Exit Function
                End If
                ReDim Preserve matri_datos(4)
                matri_datos(4).nombre_campo = "FECHAELABORACION"
                matri_datos(4).valor_campo = date1al
            End If
            If id_tipo_documental_lista <> 0 Then
                ReDim Preserve matri_datos(5)
                matri_datos(5).nombre_campo = "TIPODOCUMENTO"
                matri_datos(5).valor_campo = matri_gestion.TIPODOCUMENTO
                ReDim Preserve matri_datos(6)
                matri_datos(6).nombre_campo = "NOMBRESERIE"
                matri_datos(6).valor_campo = matri_gestion.NOMBRE_SERIE
                ReDim Preserve matri_datos(7)
                matri_datos(7).nombre_campo = "NOMBRESUBSERIE"
                matri_datos(7).valor_campo = matri_gestion.NOMBRE_SUB_SERIE
                ReDim Preserve matri_datos(8)
                matri_datos(8).nombre_campo = "NOMBRESUBSERIE"
                matri_datos(8).valor_campo = matri_gestion.NOMBRE_SUB_SERIE
            End If
            Dim RefclasAñadir As New ClassAñadirDocumento
            Dim Refclaswfdigtializado As New ClassWorkflowDigitalizacion
            Dim Matri_Datos_Almacen() As String
            Erase Matri_Datos_Almacen
            Result = Refclaswfdigtializado.Obtiene_Valores_Campos_Documento_Enlazados(Matri_Datos_Almacen,
                                                                                      Nombre_Gabinete,
                                                                                      matri_datos)
            If Result <> "YES" Then
                Guardar_Documento_pqr = "Imposible general mariz datos almacenamiento "
                Exit Function
            End If
            If Matri_Datos_Almacen Is Nothing Then
                Guardar_Documento_pqr = "Matriz de datos de almacenamiento es nothing "
                Exit Function
            End If
            Dim Filein As New System.IO.FileInfo(ruta_document)
            Result = ""
            Dim Tipo_Doc_int As Integer = -1
            Dim Refclasvisor As New Classactualizacionvisor
            Result = ""
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension), _
                                                                          Tipo_Doc_int)
            If Result <> "YES" Then
                Guardar_Documento_pqr = Result
                Exit Function
            End If

            Id_imagen = Tipo_Doc_int
            Dim radicado As String = ""
            Dim matri_documento() As String = {ruta_document}
            Result = Refalmacena.Almacenamiento("", "", Nombre_Gabinete, 0, Matri_Datos_Almacen, _
            2, 1, Tipo_Doc_int, matri_documento, 0, Id_imagen, Tipo_Doc_int, HttpContext.Current.Session.Item("GA_IDEMPRESA"), _
            id_usario_gestion, matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE, _
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE, _
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION, _
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE, _
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION, _
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado)
            If Result <> "YES" Then
                Guardar_Documento_pqr = "Almacenando  dice " & Result
                Exit Function
            End If
            '----------------------------------------------------------
            'Actualiza el estado del codigo del documento docuarchi
            '----------------------------------------------------------
            If id_registro_radicado <> 0 Then
                Dim SQL As String = "Update ra_registro_pqr set id_imagen=" & Id_imagen & _
                    ",nombre_gabinete='" & Nombre_Gabinete & "' " & _
                    " where id_registro_pqr=" & id_registro_radicado
                Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
                Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(SQL)
                If Result <> "YES" Then
                    Guardar_Documento_pqr = "Inconsistencia actualizando id documento" & Result
                    Exit Function
                End If
            End If
            Guardar_Documento_pqr = "YES"
            Exit Function
        Catch ex As Exception
            Guardar_Documento_pqr = "Inconsistencia función Guardar_Documento_Respuesta " & ex.Message
        End Try
    End Function
    Function Consulta_pqrs_registrados_usuario_post(ByRef update As UpdatePanel, ByRef hideselecion As Object, _
                                       ByRef HiddenEmailconsulta As Object, ByRef grediview As GridView, ByRef reflabel As Object) As String
        Try
            If HiddenEmailconsulta.value = "" Then
                Consulta_pqrs_registrados_usuario_post = "YES"
                Exit Function
            End If
            Dim sql_consulta As String = HiddenEmailconsulta.value
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("radicado")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_pqrs_registrados_usuario_post = "Error listando datos " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                'Dim label_act As Label = grediview.Page.FindControl("Label_scrool")
                'If Not update Is Nothing Then
                '    label_act.Text = "Paginación 1 de " & Datset.Tables(0).Rows.Count
                'End If
                HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) de expediente " &
                grediview.DataSource = Datset
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Consulta_pqrs_registrados_usuario_post = "YES"
                Exit Function
            Else
                HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) de expediente "
                'grediview.DataKeyNames = DataKey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                Next
                Consulta_pqrs_registrados_usuario_post = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_pqrs_registrados_usuario_post = "Inconsistencia funcion Consulta_pqrs_registrados_usuario_post " & ex.Message
        End Try
    End Function
    Function Consulta_pqrs_registrados_usuario(ByVal id_usuario_pqrs As Integer, _
                                               ByRef grediview As GridView, _
                                               ByRef HiddenEmailconsulta As Object, _
                                               ByRef hideselecion As Object, _
                                               ByRef reflabel As Label, _
                                               ByRef update As UpdatePanel, _
                                               ByVal tipo_consulta As Integer, _
                                               ByVal valor_consulta As String, _
                                               ByVal consulta_cache As String, _
                                               ByRef colum_order_name As String, _
                                               ByRef order_colum As String) As String
        Try

            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT radicado_pqr as RADICADO_PQRSD,fecha_registro AS FECHA_REGISTRO," _
                                       & "asunto_solicitud as ASUNTO_PQRSD from ra_registro_pqr where id_dext_externo=" & _
                                       id_usuario_pqrs & " order by " & colum_order_name & " " & order_colum
            Else
                sql_consulta = "SELECT radicado_pqr as RADICADO_PQRSD,fecha_registro AS FECHA_REGISTRO," _
                                      & "asunto_solicitud as ASUNTO_PQRS from ra_registro_pqr where " & _
                                       "( radicado_pqr like '%" & valor_consulta & "%'" & _
                                       " or fecha_registro like '%" & valor_consulta & "%'" & _
                                       " or asunto_solicitud like '%" & valor_consulta & "%'" & _
                                      " ) and id_dext_externo=" & id_usuario_pqrs & _
                                      " order by " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("Sort_matri_colum_publico") = {"OPCIONES", "RADICADO_PQRSD", _
                                                                             "FECHA_REGISTRO", "ASUNTO_PQRSD"}
            HttpContext.Current.Session.Item("SortExpression_publico") = colum_order_name
            HttpContext.Current.Session.Item("SortDirection_publico") = order_colum
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_PUBLICO") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO") = sql_consulta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_registro_pqr")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_pqrs_registrados_usuario = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Consulta_pqrs_registrados_usuario = "YES"
                Exit Function
            Else
                HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "doc_rel_solic")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-chart-network fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Estados de la solicitud")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "est_solic")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-th-list fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Detalles de la solicitud")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "detale_solic")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-bars fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-warning btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Transaciones de la solicitud")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "trans_sac_solic")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                   
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name, _
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_publico"), _
                                                            order_colum, _
                                                            grediview)
                If Result <> "YES" Then
                    Consulta_pqrs_registrados_usuario = "Error add clase funcion  Consulta_pqrs_registrados_usuario " & Result
                    Exit Function
                End If
                Consulta_pqrs_registrados_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_pqrs_registrados_usuario = "Inconsistencia general función Consulta_pqrs_registrados_usuario " & ex.Message
        End Try
    End Function
End Class
