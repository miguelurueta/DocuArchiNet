Imports System.Math
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.CodeDom.Compiler
Imports System.Reflection
Imports System.IO
Imports System.Collections.Specialized
Imports AjaxControlToolkit
Public Class CDParameterFileLoadworkflow
    Property IdTipoTramite As Integer
    Property TipoPlantillaTramite As String
    Property IconfigDigitalizacion As Integer
End Class
Public Class CDFileLoadWorkflow
    Property AppError As String
    Property CDParameterFileLoadworkflow As New List(Of CDParameterFileLoadworkflow)
End Class
Public Class ClassWorkflow

    Function Registra_flujo_tarea_workflow_radicado_simple(ByVal id_actividad_usuario_workflow As Integer,
                                                           ByVal id_usuario_workflow As Integer,
                                                           ByVal id_registro_estado As Integer) As String
        '------------------------------------------------------------------------------------
        'Funcion : Registra el flujo de una radicado en modulo de radicaciónsimple 
        '------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------------
        'id_plantilla           : Representa la identificación de la planilla de radicación
        '
        'id_tipo_plantilla      : Representa la idneitfcación del tipo de plantilla
        'id_usuario_radicacion  : Representa el usuario que radica la correspondencia
        '
        '------------------------------------------------------------------------------------
        '                           RETORNO
        '------------------------------------------------------------------------------------
        'codigo_radicado_consecutivo  : Retorna el consecutivo del radicado
        '------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-17
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim stru_registro_estado As stru_registro_estado = Nothing
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(id_registro_estado,
                                                                                                      stru_registro_estado)
            If Result <> "YES" Then
                Registra_flujo_tarea_workflow_radicado_simple = Result
                Exit Function
            End If
            '----///////Solicita la relación del flujo de trabajo----////
            Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
            Dim id_flujo_trabajo As Integer = 0
            Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_id_flujo_relacion_flujo_tramite(stru_registro_estado.tipo_doc_entrante_id_Tipo_Doc_Entrante,
                                                                                                      id_flujo_trabajo)
            If Result <> "YES" Then
                Registra_flujo_tarea_workflow_radicado_simple = Result
                Exit Function
            End If
            '----///////Solicita la actvidad de flujo documental del usuario workflow seleccionado---//
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_flujo_trabajo As Integer = 0
            Dim id_registro_actvidad_flujo_trabajo As Integer = 0
            Dim nombre_flujo_trabajo As String = ""
            Dim Refclas_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            Dim class_flujo_workflow As New Class_flujo_trabajo_workflow
            If id_flujo_trabajo <> 0 Then
                Result = class_flujo_workflow.Solicita_datos_actividad_inicio_flujo(id_flujo_trabajo,
                                                                                    id_registro_actvidad_flujo_trabajo,
                                                                                    id_actividad_flujo_trabajo,
                                                                                    id_usuario_workflow_flujo_trabajo)
                If Result <> "YES" Then
                    Registra_flujo_tarea_workflow_radicado_simple = Result
                    Exit Function
                End If
                If id_registro_actvidad_flujo_trabajo <> 0 Then

                Else
                    Result = Refclas_flujo_trabajo_workflow.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo,
                                                                                                  nombre_flujo_trabajo)
                    If Result <> "YES" Then
                        Registra_flujo_tarea_workflow_radicado_simple = Result
                        Exit Function
                    End If
                    If Result <> "YES" Then
                        Registra_flujo_tarea_workflow_radicado_simple = "El flujo (" & nombre_flujo_trabajo & ") no tiene una actividad de inicio contacte al administrador"
                        Exit Function
                    End If
                End If
            End If
            '-----/////Solicita tipo modulo de envio si se envia  1- Modulo workflow 2-Modulo simplificado y modulo radicación 3 Solo para soporte documental sin gestión------///
            Dim ref_class_tipo_doc_emtrante As New Class_tipo_doc_entrante
            Dim tipo_envio_tramite As Integer = 0
            Result = ref_class_tipo_doc_emtrante.Solicita_tipo_modulo_soporte_documental_envio(stru_registro_estado.tipo_doc_entrante_id_Tipo_Doc_Entrante,
                                                                                               tipo_envio_tramite)
            If Result <> "YES" Then
                Registra_flujo_tarea_workflow_radicado_simple = Result
                Exit Function
            End If
            '-----//Formatea fecha de seleccion workflow -----////
            Dim DateCreate As Date = Now
            Dim fecha_selecion As Object = Nothing
            Dim Refclas_gestion_fecha As New ClassGestionFechas
            Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                        fecha_selecion)
            If Result <> "YES" Then
                Registra_flujo_tarea_workflow_radicado_simple = Result
                Exit Function
            End If
            Dim estado_modulo_respuesta As Integer = 0
            Dim Refclas_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Refclas_tipo_doc_entrante.Determina_gestion_modulo_pqr_id_Tipo_Tramite(stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                                                            stru_registro_estado.tipo_doc_entrante_id_Tipo_Doc_Entrante,
                                                                                            estado_modulo_respuesta)
            If Result <> "YES" Then
                Registra_flujo_tarea_workflow_radicado_simple = Result
                Exit Function
            End If
            '------/////Registra el flujo documental del radicado activo para soporte documental-----/////////
            Dim ClassWorkflow As New ClassWorkflow
            Dim id_tarea_workflow As Long = 0
            Result = ClassWorkflow.Registra_flujo_documento(id_actividad_usuario_workflow,
                                                            id_usuario_workflow,
                                                            0,
                                                            stru_registro_estado.consecutivo_radicado,
                                                            stru_registro_estado.system_plantilla_radicado_id_Plantilla,
                                                            id_flujo_trabajo,
                                                            id_registro_actvidad_flujo_trabajo,
                                                            id_usuario_workflow_flujo_trabajo,
                                                            0,
                                                            estado_modulo_respuesta,
                                                            id_tarea_workflow,
                                                            fecha_selecion,
                                                            1)
            If Result <> "YES" Then
                Registra_flujo_tarea_workflow_radicado_simple = Result
                Exit Function
            End If
            '-----////Registra la relación del flujo de trabajo en la tabla estados radicado-----////
            Dim class_estados As New Class_ra_rad_estados_modulo_radicacion
            Result = class_estados.Relaciona_id_tarea_wf_estado_radicado(id_registro_estado,
                                                                         id_tarea_workflow)
            If Result <> "YES" Then
                Registra_flujo_tarea_workflow_radicado_simple = " Error actualiza estado id tarea  (" & Result & ")"
                Exit Function
            End If
            '------/////Registra el flujo documental del radicado activo para soporte documental-----/////////
            Result = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion(id_registro_estado,
                                                                                                        0)
            If Result <> "YES" Then
                Registra_flujo_tarea_workflow_radicado_simple = Result
                Exit Function
            End If
            Registra_flujo_tarea_workflow_radicado_simple = "YES"
            Exit Function
        Catch ex As Exception
            Registra_flujo_tarea_workflow_radicado_simple = "Inconsistencia general funcion Registra_flujo_tarea_workflow_radicado_simple " & ex.Message
        End Try
    End Function
    Function Activa_adjuntar_documento_digitalizado_tarea_seleccionada(ByVal id_tarea As Long,
                                                                       ByRef pag As Page) As String
        Try
            Dim Hidden_id_tarea_sel As Object = pag.FindControl("Hidden_id_tarea_sel")
            Dim UpdatePanel_general_variable As UpdatePanel = pag.FindControl("UpdatePanel_general_variable")
            Dim ref_IframeDitaliza As Object = pag.FindControl("IframeDitaliza_adjunto_")
            Dim ref_UpdatePanel_iframe_digitaliza As Object = pag.FindControl("UpdatePanel_iframe_digitaliza_adjunto")
            Dim Ref_HiddenIdFlujo As Object = pag.FindControl("HiddenIdFlujo")
            Dim Ref_HiddenRuta As Object = pag.FindControl("HiddenRuta")
            Dim Ref_UpdateDatos As Object = pag.FindControl("UpdateDatos")
            Dim ModalPopupExtender_edition_digitaliza_documento_adjunto As ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_digitaliza_documento_adjunto")
            Dim UpdatePanel_boton_tool As UpdatePanel = pag.FindControl("UpdatePanel_boton_tool")
            Hidden_id_tarea_sel.value = id_tarea
            UpdatePanel_general_variable.Update()
            HttpContext.Current.Session("SELECCIONTEMPORAL") = id_tarea.ToString & "|" & 0 & "|" & 0 & "|" & 0
            HttpContext.Current.Session.Item("WF_TAGSELECCION_EMERGENTE") = ""
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = ""
            HttpContext.Current.Session.Item("DG_ID_TRAMITE") = 0
            HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = ""
            HttpContext.Current.Session.Item("DG_ID_GABINETE") = 0
            HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE") = ""
            HttpContext.Current.Session.Item("DG_RADICADO") = ""
            HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
            HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = ""
            HttpContext.Current.Session.Item("WF_TAGSELECCION_EMERGENTE") = ""
            Ref_HiddenIdFlujo.Value = HttpContext.Current.Session.Item("SELECCIONTEMPORAL")
            Dim Result As String = ""
            Dim Ruta_Web_Escaner As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString.Replace("\", "/")
            Ref_HiddenRuta.Value = Ruta_Web_Escaner
            Dim Refclas_digitalizacion As New ClassWorkflowDigitalizacion
            '-----Retorna el tipo de flujo interno o externo
            Dim refclas_dat_adit As New Class_DAT_ADIC_TAR
            Dim id_tipo_flujo As Integer = 0
            Result = refclas_dat_adit.SolicitaIdTipoFlujoTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                            HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                            id_tipo_flujo)
            If Result <> "YES" Then
                Activa_adjuntar_documento_digitalizado_tarea_seleccionada = Result
                Exit Function
            End If
            If id_tipo_flujo = 1 Then
                Result = Refclas_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                 id_tarea,
                                                                                 HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"),
                                                                                 HttpContext.Current.Session.Item("DG_ID_TRAMITE"),
                                                                                 HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                                 HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                 HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                 HttpContext.Current.Session.Item("DG_RADICADO"),
                                                                                 HttpContext.Current.Session("DG_NOMBRE_TRAMITE"))
            Else
                Result = Refclas_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                         HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                         HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"), HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                                         HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                         HttpContext.Current.Session.Item("DG_RADICADO"),
                                                                                         HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"))
                If Result <> "YES" Then
                    Activa_adjuntar_documento_digitalizado_tarea_seleccionada = Result
                    Exit Function
                End If
                Dim ref_class_tipo_doc_entrante As New Class_tipo_doc_entrante
                Result = ref_class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                       HttpContext.Current.Session.Item("DG_ID_TRAMITE"))
                If Result <> "YES" Then
                    Activa_adjuntar_documento_digitalizado_tarea_seleccionada = Result
                    Exit Function
                End If
            End If
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE_ADJUNTOWORKFLOW"
            Dim Refclasdigitaliza As New ClassWorkflowDigitalizacion
            ref_IframeDitaliza.Attributes.Add("src", "../workflow/WebFormEscan.aspx")
            ref_UpdatePanel_iframe_digitaliza.Update()
            Ref_UpdateDatos.Update()
            If Not UpdatePanel_boton_tool Is Nothing Then
                UpdatePanel_boton_tool.Update()
            End If
            ModalPopupExtender_edition_digitaliza_documento_adjunto.Show()
            Activa_adjuntar_documento_digitalizado_tarea_seleccionada = "YES"
            Exit Function
        Catch ex As Exception
            Activa_adjuntar_documento_digitalizado_tarea_seleccionada = "Inconsistencia general función Activa_adjuntar_documento_digitalizado_tarea_seleccionada " & ex.Message
        End Try
    End Function

    Function Verifica_tareas_en_estado_pendiente(ByVal id_usuario_workflow As Integer,
                                                 ByRef estado As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT Inicio_Tareas_Workflow_id_Tarea FROM tarea_pendiente" &
            " WHERE id_usuario=" & id_usuario_workflow &
            " AND  Estados_Pendiente=" & 1 & " limit 1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Verifica_tareas_en_estado_pendiente = "Error Consultando en tabla " & "ESTADOS_TAREA_WORKFLOW" & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado = "NO"
                Verifica_tareas_en_estado_pendiente = "YES"
                Exit Function
            Else
                estado = "YES"
                Verifica_tareas_en_estado_pendiente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_tareas_en_estado_pendiente = "Inconsistencia función Verifica_tareas_en_estado_pendiente " & ex.Message
        End Try
    End Function
    Function verifica_tarea_seleccionada_workflow(ByVal id_usuario_workflow As Integer,
                                                  ByVal id_tarea As Integer,
                                                  ByRef estado As String,
                                                  ByRef nombre_actividad As String,
                                                  ByRef nombre_usuario As String,
                                                  ByRef loguin_usuario As String,
                                                  ByRef cargo As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT law.Nombre_Actividad,uw.Nombre_Usuario,uw.login_Usuario,uw.Cargo_Usuario FROM ESTADOS_TAREA_WORKFLOW as etw " &
            "Left outer join  listado_actividades_workflow  as law on (law.id_actividad=etw.id_actividad) " &
            "Left outer join usuario_workflow  as uw on (uw.idU_suario=etw.Id_Usuario) " &
            "  where  inicio_tareas_workflow_id_tarea=" & id_tarea & "  AND " &
            "  Fecha_Seleccion IS NOT NULL and fecha_fin is null "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                verifica_tarea_seleccionada_workflow = "Error Consultando en tabla " & "ESTADOS_TAREA_WORKFLOW" & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado = "NO"
                verifica_tarea_seleccionada_workflow = "YES"
                Exit Function
            Else
                nombre_actividad = Datset.Tables(0).Rows(0).Item(0)
                nombre_usuario = Datset.Tables(0).Rows(0).Item(1)
                loguin_usuario = Datset.Tables(0).Rows(0).Item(2)
                cargo = Datset.Tables(0).Rows(0).Item(3)
                estado = "YES"
                verifica_tarea_seleccionada_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            verifica_tarea_seleccionada_workflow = "Inconsistencia general funcion verifica_tarea_seleccionada_workflow " & ex.Message
        End Try
    End Function

    Function Retorna_id_imagen_seleccionada(ByVal id_tarea_seleccionada As Integer,
                                            ByVal id_ruta_workflow As Integer,
                                            ByRef id_imagen As Integer) As String
        Try
            Dim ref_list As New ClassListandoTareas
            Dim Result As String = ""
            Dim I2 As Integer = 0
            id_imagen = 0
            Dim NombreRutaF As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(id_ruta_workflow.ToString,
                                                                     NombreRutaF)
            If Result <> "YES" Then
                Retorna_id_imagen_seleccionada = "Erro listando Ruta " + Result
                Exit Function
            End If
            If NombreRutaF = "" Then
                Retorna_id_imagen_seleccionada = "Imposible Econtrar Nombre de la ruta " + Result
                Exit Function
            End If
            Dim Sql_consulta As String = "Select ID_IMAGEN From DAT_ADIC_TAR" & NombreRutaF &
              " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea_seleccionada
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DAT_ADIC_TAR")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_imagen_seleccionada = "Función Datset.Tables(0).Rows(0) dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_imagen_seleccionada = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_imagen = 0
                Else
                    id_imagen = Datset.Tables(0).Rows(0).Item(0)
                End If
                Retorna_id_imagen_seleccionada = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_id_imagen_seleccionada = "Inconsistencia usuario de gestión Retorna_id_imagen_seleccionada " & ex.Message
        End Try
    End Function

    Private Sub comman1_clik(ByVal sender As _
            System.Object, ByVal e As System.EventArgs)
        Try
            Dim Ref As New Classscrripjava
            Dim Pag As Page = sender.Page
            Dim Matri_Sender() As String
            Dim Result As String = ""
            Erase Matri_Sender
            Dim hiden As Object = Pag.FindControl("HiddenFiltro")
            hiden.value = ""
            Dim updat As UpdatePanel = Pag.FindControl("Recupera")
            Matri_Sender = Split(sender.id, "|")
            Result = Consulta_Datos_Actividad(Pag)
            If Result <> "YES" Then
                Ref.Showscripman(Result, updat)

            End If

        Catch ex As Exception

        End Try
    End Sub



    Function Cambiar_Estado_Prioridad_Tarea(ByVal ID_ACTIVIDAD As String,
                                            ByVal Id_Usuario As String,
                                            ByVal Tarea_Seleccion As String,
                                            ByVal Prioridad As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "UPDATE ESTADOS_TAREA_WORKFLOW SET  ESTADO_PRIORIDAD=" & Prioridad &
            " WHERE ID_ACTIVIDAD=" & ID_ACTIVIDAD &
            " AND ID_USUARIO=" & Id_Usuario & " AND " &
            " ESTADO_TAREA = 0 AND Inicio_Tareas_Workflow_id_Tarea =" & Tarea_Seleccion &
            " AND FECHA_FIN IS NULL"
            Dim ref As New conect.Dbase_Conction_Mysql

            Dim Result As String = ref.SELECTION_INSERT_COMMAND(Sql_consulta)
            If Result <> "YES" Then
                Cambiar_Estado_Prioridad_Tarea = "El usuario no puede actualizar anotaciones de otros usuarios"
                Exit Function
            End If
            Cambiar_Estado_Prioridad_Tarea = "YES"
        Catch ex As Exception
            Cambiar_Estado_Prioridad_Tarea = "Error General " & ex.ToString
        End Try
    End Function

    Function Retorna_trazabilidad_radicado(ByVal page1 As Page,
                                           ByVal radicado As String) As String
        Try
            Dim Result As String = ""
            Dim Nombre_ruta As String = ""
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID_VAL")
            If scripma Is Nothing Then
                Retorna_trazabilidad_radicado = "Imposible encontrar datagrid  " & "GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Retorna_trazabilidad_radicado = "Imposible encontrar el control  " & "titulo_label_val_radicacion"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Retorna_trazabilidad_radicado = "Imposible encontrar el control  " & "UpdatePanelabel_val_radicacion"
                Exit Function
            End If
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(Nombre_ruta)
            If Result <> "YES" Then
                Retorna_trazabilidad_radicado = Result
                Exit Function
            End If
            Dim id_ruta As Integer = 0
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(Nombre_ruta,
                                                                id_ruta)
            If Result <> "YES" Then
                Retorna_trazabilidad_radicado = Result
                Exit Function
            End If
            Dim nombre_campo_radicado As String = ""
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                              nombre_campo_radicado)
            If Result <> "YES" Then
                Retorna_trazabilidad_radicado = Result
                Exit Function
            End If
            Dim Sql_consulta As String = "select etw.id_Estado, law.nombre_actividad as ESTADO,uw.Nombre_Usuario as USUARIO_ASIGNADO, etw.fecha_inicio  as FECHA_ASIGNACION" &
                "  from dat_adic_tar" & Nombre_ruta & " as datr " &
                " inner join estados_tarea_workflow etw on " &
                " (etw.inicio_tareas_workflow_id_tarea=datr.inicio_tareas_workflow_id_tarea) " &
                " inner join listado_actividades_workflow law on " &
                " (law.id_actividad = etw.id_actividad) " &
                " left outer join usuario_workflow as uw on(uw.idu_suario = etw.Id_Usuario) " &
                " where datr." & nombre_campo_radicado & "='" & radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("radicado")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_trazabilidad_radicado = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontraron (" & Datset.Tables(0).Rows.Count & ") registro(s)  de trazabilidad para el radicado (" & radicado & ")"
                scripma.DataSource = Datset
                hideselecion.value = "-1"
                scripma.DataBind()
                HttpContext.Current.Session.Item("DATA_SET_SESION_TRAZA_RAD") = scripma
                updat.Update()
                updatelabel.Update()
                Retorna_trazabilidad_radicado = "YES"
                Exit Function
            Else
                'HttpContext.Current.Session.Item("RA_DATO_CONSULTA_RADICADO") = Sql_consulta
                labetitle.Text = "Se encontraron (" & Datset.Tables(0).Rows.Count & ") registro(s)  de trazabilidad para el radicado (" & radicado & ")"
                scripma.DataSource = Datset
                hideselecion.value = "-1"
                scripma.DataBind()
                HttpContext.Current.Session.Item("DATA_SET_SESION_TRAZA_RAD") = scripma
                updat.Update()
                updatelabel.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    Dim mdiv As New HtmlControls.HtmlGenericControl("div")
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString)
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-file-alt fa-sm")
                    Dim itm_a As New HtmlControls.HtmlGenericControl("a")
                    itm_a.Attributes.Add("class", "btn btn-success btn-sm")
                    itm_a.Attributes.Add("onclick", "prevent(event,this);")
                    itm_a.Attributes.Add("title", "Ver detalle del registro")
                    itm_a.ID = scripma.Rows(i).Cells(1).Text.ToString()
                    itm_a.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    itm_a.Controls.Add(ihtml)
                    mdiv.Controls.Add(itm_a)
                    scripma.Rows(i).Cells(0).Controls.Add(mdiv)
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            'scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            'scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Retorna_trazabilidad_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_trazabilidad_radicado = "Inconsistencia general función Retorna_trazabilidad_radicado " & ex.Message
        End Try
    End Function

    Function Lista_Estado_Prioridad_workflow(ByRef Estado_Tarea As String) As String
        Try
            Dim Sql_consulta As String = "SELECT ESTADO_PRIORIDAD" &
            " FROM ESTADOS_TAREA_WORKFLOW " &
            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & HttpContext.Current.Session("ID_TAREA_SELECCIONDA") &
            " AND FECHA_FIN IS NULL AND ESTADO_TAREA=0"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_Estado_Prioridad_workflow = " #12 Inconsistencia en la consulta   ESTADOS_TAREA_WORKFLOW para la tarea " & HttpContext.Current.Session("ID_TAREA_SELECCIONDA")
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_Estado_Prioridad_workflow = " #13 no se registra estado en ESTADOS_TAREA_WORKFLOW para la tarea " & HttpContext.Current.Session("ID_TAREA_SELECCIONDA")
                Exit Function
            Else
                Estado_Tarea = Datset.Tables(0).Rows(0).Item(0).ToString
                Lista_Estado_Prioridad_workflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_Estado_Prioridad_workflow = "Error General " & ex.ToString
        End Try
    End Function

    Function Actualizar_Datos_tarea_pendiente(ByVal DAT_TAREA As String,
                                              ByVal ID_TAREA_PENDIENTE As String) As String
        Try
            Dim Parametro_Insert As String = "UPDATE TAREA_PENDIENTE" &
            " SET DATOS_PENDIENTE='" & DAT_TAREA & "'" &
            " WHERE ID_PENDIENTE=" & ID_TAREA_PENDIENTE & " AND " &
            " ID_USUARIO=" & HttpContext.Current.Session("Id_Usuario_Workflow")
            Dim ref As New conect.Dbase_Conction_Mysql

            Dim Result As String = ref.SELECTION_INSERT_COMMAND(Parametro_Insert)
            If Result <> "YES" Then
                Actualizar_Datos_tarea_pendiente = "El usuario no puede actualizar anotaciones de otros usuarios"
                Exit Function
            End If

            Actualizar_Datos_tarea_pendiente = "YES"
        Catch EX As Exception
            Actualizar_Datos_tarea_pendiente = "Error Gerneral funcion Actualizar_Datos_tarea_pendiente " + EX.Message
        End Try
    End Function

    Function Consulta_Datos_Actividad(ByRef Refpage As Page) As String
        Try
            Dim Matri_Campos_Lista() As String
            Erase Matri_Campos_Lista
            Dim SqlConsula As String = ""
            Dim SqlConsultaG As String = ""
            Dim Sql_consulta As String = "Select NOMBRE_CAMPO,TIPO_CAMPO from " &
                " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & HttpContext.Current.Session("Id_Ruta_Workflow") &
                " AND LISTA_TAREA=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then

                Consulta_Datos_Actividad = " Error # 01 Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then

                Consulta_Datos_Actividad = " # 01 La tabla CONFIGURACION_LISTADO_RUTA no tiene campos de consulta"
                Exit Function
            Else
                Erase Matri_Campos_Lista
                Dim i As Integer = 0
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Campos_Lista(i)
                    Matri_Campos_Lista(i) = Datset.Tables(0).Rows(i).Item(0).ToString

                Next
            End If
            SqlConsula = "SELECT ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA,ETW.ID_ACTIVIDAD,LAW.NOMBRE_ACTIVIDAD,ETW.ID_USUARIO"
            If Not Matri_Campos_Lista Is Nothing Then
                Dim i As Integer = 0
                For i = 0 To UBound(Matri_Campos_Lista)
                    SqlConsula = SqlConsula & "," & Matri_Campos_Lista(i)
                Next
            End If
            Dim Nombre_Ruta As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Consulta_Datos_Actividad = Result
                Exit Function
            End If
            Dim Nombre_actividad As String = ""
            Dim drownousca As DropDownList = Refpage.FindControl("DropDownListActividad")
            If drownousca Is Nothing Then
                Consulta_Datos_Actividad = "Imposible encontrar control drownobusca en la pagina"
                Exit Function
            Else
                Nombre_actividad = drownousca.Text
            End If
            Dim id_tar As Integer = -1
            Dim Ref_class_list_actividades As New Class_Listado_Actividades_workflow
            Result = Ref_class_list_actividades.Obtener_Id_Actividad(id_tar,
                                                                     Nombre_actividad)
            If Result <> "YES" Then
                Consulta_Datos_Actividad = Result
                Exit Function
            End If
            Dim SqlConsulTabla As String = " FROM DAT_ADIC_TAR" & Nombre_Ruta & " DATW "
            Dim SqlConsultaCuerpo As String = "INNER JOIN ESTADOS_TAREA_WORKFLOW ETW ON " &
              "(ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA= " &
              "DATW.INICIO_TAREAS_WORKFLOW_ID_TAREA AND ETW.FECHA_FIN IS NULL AND " &
              "ETW.ESTADO_TAREA=0) " &
              "INNER JOIN LISTADO_ACTIVIDADES_WORKFLOW LAW ON " &
              "(LAW.ID_ACTIVIDAD=ETW.ID_ACTIVIDAD AND LAW.ID_ACTIVIDAD=" & id_tar & ")  ORDER BY ETW.FECHA_INICIO DESC"

            SqlConsultaG = SqlConsula & SqlConsulTabla & SqlConsultaCuerpo
            SqlConsultaG = SqlConsultaG & " LIMIT 2000"
            Dim refgrid As GridView = Refpage.FindControl("GridViewlista")
            Dim updat As UpdatePanel = Refpage.FindControl("UpdateGeneral")
            Dim updatelabel As UpdatePanel = Refpage.FindControl("UpdatePanel_labelresultado")
            Dim label_resultado As Label = Refpage.FindControl("Label_resultado")
            If refgrid Is Nothing Then
                Consulta_Datos_Actividad = "Imposible instanciar gredview"
                Exit Function
            End If
            ref = New conect.Dbase_Conction_Mysql
            Datset = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELD(SqlConsultaG, Datset)
            If Result <> "YES" Then
                Consulta_Datos_Actividad = " Error # Imposible encontrar datos de la tarea" & SqlConsultaG
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                refgrid.DataSource = Datset
                refgrid.DataBind()
                updat.Update()
                label_resultado.Text = "Se encontraron (0) registros"
                updatelabel.Update()

            Else
                refgrid.DataSource = Datset
                refgrid.DataBind()
                updat.Update()
                For i As Integer = 0 To refgrid.Rows.Count - 1
                    Dim tex As String = refgrid.Rows(i).Cells(1).Text & "-" & refgrid.Rows(i).Cells(2).Text
                    'key = GridViewlista.DataKeys(E.Row.RowIndex).Value.ToString()
                    refgrid.Rows(i).Attributes.Add("id", tex)
                    'E.Row.Attributes.Add("id", key)
                Next
                label_resultado.Text = "Se encontraron (" & refgrid.Rows.Count & ") registros limite 2000 concordancias"
                updatelabel.Update()

            End If
            Consulta_Datos_Actividad = "YES"
        Catch ex As Exception
            Consulta_Datos_Actividad = "Excepcion General Consulta_Datos_Actividad " & ex.Message
        End Try
    End Function
    Function Consulta_tareas_workflow(ByRef Refpage As Page,
                                      ByVal conector As String,
                                      ByVal limite_registro As Integer,
                                      ByVal tipo_consulta As Integer,
                                      ByVal valor_general As String) As String
        Try
            Dim Matri_Campos_Lista() As String
            Erase Matri_Campos_Lista
            Dim SqlConsula As String = ""
            Dim SqlConsultaG As String = ""
            Dim hdnEmailID As Object = Refpage.FindControl("hdnEmailID")
            If hdnEmailID Is Nothing Then
                Consulta_tareas_workflow = "Imposible encontrar el control (hdnEmailID)"
                Exit Function
            End If
            Dim Hidden_id_tarea_sel As Object = Refpage.FindControl("Hidden_id_tarea_sel")
            If Hidden_id_tarea_sel Is Nothing Then
                Consulta_tareas_workflow = "Imposible encontrar el control (Hidden_id_tarea_sel)"
                Exit Function
            End If
            Dim Sql_consulta As String = "Select NOMBRE_CAMPO,TIPO_CAMPO from " &
                " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & HttpContext.Current.Session("Id_Ruta_Workflow") &
                " AND LISTA_TAREA=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_tareas_workflow = " Error # 01 Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then

            Else
                Erase Matri_Campos_Lista
                Dim i As Integer = 0
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Campos_Lista(i)
                    Matri_Campos_Lista(i) = Datset.Tables(0).Rows(i).Item(0).ToString
                Next
            End If
            SqlConsula = "SELECT ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA,ETW.ID_ACTIVIDAD,LAW.NOMBRE_ACTIVIDAD AS GRUPO,uw.Nombre_Usuario AS USUARIO,uw.Cargo_Usuario AS CARGO, wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO "
            If Not Matri_Campos_Lista Is Nothing Then
                Dim i As Integer = 0
                For i = 0 To UBound(Matri_Campos_Lista)
                    SqlConsula = SqlConsula & "," & Matri_Campos_Lista(i)
                Next
            End If
            Dim Nombre_Ruta As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Consulta_tareas_workflow = Result
                Exit Function
            End If
            Dim SqlConsulTabla As String = " FROM DAT_ADIC_TAR" & Nombre_Ruta & " DATW "
            Dim SqlConsultaCuerpo As String = "INNER JOIN ESTADOS_TAREA_WORKFLOW ETW ON " &
              "(ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA= " &
              "DATW.INICIO_TAREAS_WORKFLOW_ID_TAREA AND ETW.FECHA_FIN IS NULL AND " &
              "ETW.ESTADO_TAREA=0) " &
              " LEFT OUTER JOIN usuario_workflow AS uw on (ETW.ID_USUARIO=uw.idU_suario) " &
              " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=ETW.ID_FLUJO_TRABAJO) " &
              " INNER JOIN LISTADO_ACTIVIDADES_WORKFLOW LAW ON " &
              "(LAW.ID_ACTIVIDAD=ETW.ID_ACTIVIDAD)  "
            Dim Objet As New Object
            Dim MatriTipo() As String
            Dim Tipo_Campos As String = ""
            Erase MatriTipo
            Dim Reftable As Table = Refpage.FindControl("TableControles")
            If Reftable Is Nothing Then
                Consulta_tareas_workflow = "Imposible encontrar control table en la pagina"
                Exit Function
            End If
            If tipo_consulta = 1 Then
                For i As Integer = 0 To Reftable.Controls.Count - 1
                    Dim tabler As TableRow = Reftable.Controls(i)
                    For Each Objet In tabler.Controls
                        For Each Objet1 In Objet.Controls
                            If Objet1.GetType().Name = "TextBox" Then
                                If Objet1.Text <> "" Then
                                    SqlConsultaCuerpo = "INNER JOIN ESTADOS_TAREA_WORKFLOW ETW ON " &
                                    "(ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA= " &
                                    "DATW.INICIO_TAREAS_WORKFLOW_ID_TAREA AND ETW.FECHA_FIN IS NULL ) " &
                                    " LEFT OUTER JOIN usuario_workflow AS uw on (ETW.ID_USUARIO=uw.idU_suario)" &
                                    " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=ETW.ID_FLUJO_TRABAJO) " &
                                    "INNER JOIN LISTADO_ACTIVIDADES_WORKFLOW LAW ON " &
                                    "(LAW.ID_ACTIVIDAD=ETW.ID_ACTIVIDAD) WHERE "
                                    Exit For
                                End If
                            End If
                        Next
                    Next
                Next
            End If
            SqlConsultaG = SqlConsula & SqlConsulTabla & SqlConsultaCuerpo
            Dim valorCuerpo As Integer = SqlConsultaG.Length
            For i As Integer = 0 To Reftable.Controls.Count - 1
                Dim tabler As TableRow = Reftable.Controls(i)
                For Each Objet In tabler.Controls
                    For Each Objet1 In Objet.Controls
                        If Objet1.GetType().Name = "TextBox" Then
                            Dim ref_texbox As TextBox = Objet1
                            Dim valor_atribute As String = ref_texbox.Attributes.Item("TIPO")
                            If tipo_consulta = 1 Then
                                If Objet1.Text <> "" And valor_atribute <> "DATE_" Then
                                    If valor_atribute = "DATE" Then
                                        Dim ob_ject As TextBox = tabler.FindControl(Objet1.ID & "_")
                                        Dim campo_plantilla As String = " CAST(" & Objet1.ID & " AS DATE) "
                                        If ob_ject.Text <> "" Then
                                            If valorCuerpo = SqlConsultaG.Length Then
                                                SqlConsultaG = SqlConsultaG & campo_plantilla & " BETWEEN '" & Objet1.text & "' and '" & ob_ject.Text & "' "
                                            Else
                                                SqlConsultaG = SqlConsultaG & " " & conector & " " & campo_plantilla & " BETWEEN '" & Objet1.text & "' and '" & ob_ject.Text & "' "
                                            End If
                                        Else
                                            If valorCuerpo = SqlConsultaG.Length Then
                                                SqlConsultaG = SqlConsultaG & campo_plantilla & "='" & Objet1.text & "' "
                                            Else
                                                SqlConsultaG = SqlConsultaG & " " & conector & " " & campo_plantilla & "='" & Objet1.text & "' "
                                            End If
                                        End If

                                    Else
                                        If valorCuerpo = SqlConsultaG.Length Then
                                            SqlConsultaG = SqlConsultaG & Objet1.ID & "='" & Objet1.text & "' "
                                        Else
                                            SqlConsultaG = SqlConsultaG & " " & conector & " " & Objet1.ID & "='" & Objet1.text & "' "
                                        End If
                                    End If
                                End If
                            Else
                                If valorCuerpo = SqlConsultaG.Length Then
                                    SqlConsultaG = SqlConsultaG & " where " & Objet1.ID & " like '%" & valor_general & "%' "
                                Else
                                    SqlConsultaG = SqlConsultaG & " or " & Objet1.ID & " like '%" & valor_general & "%' "
                                End If
                            End If

                        End If
                    Next
                Next
            Next

            SqlConsultaG = SqlConsultaG & " ORDER BY ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA DESC " & " LIMIT " & limite_registro
            Dim refgrid As GridView = Refpage.FindControl("GridViewlista")
            Dim updat As UpdatePanel = Refpage.FindControl("UpdateGeneral")
            Dim updatelabel As UpdatePanel = Refpage.FindControl("UpdatePanel_labelresultado")
            Dim label_resultado As Label = Refpage.FindControl("Label_resultado")
            If refgrid Is Nothing Then
                Consulta_tareas_workflow = "Imposible encontrar campo GridViewlista"
                Exit Function
            End If
            hdnEmailID.value = "-1"
            Hidden_id_tarea_sel.value = "-1"
            ref = New conect.Dbase_Conction_Mysql
            Datset = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELD(SqlConsultaG, Datset)
            If Result <> "YES" Then
                Consulta_tareas_workflow = " Error # Imposible encontrar datos de la tarea" & SqlConsultaG
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                refgrid.DataSource = Datset
                refgrid.DataBind()
                updat.Update()
                label_resultado.Text = "Se encontraron (0) registro (s)"
                updatelabel.Update()
            Else
                refgrid.DataSource = Datset
                refgrid.DataBind()
                updat.Update()
                For i As Integer = 0 To refgrid.Rows.Count - 1
                    refgrid.Rows(i).Attributes.Add("id", refgrid.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-list-ol fa-lg")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("title", "Lista trazabilidad")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Lista trazabilidad")
                    ahtml.Attributes.Add("idd", refgrid.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "e_c_b_004")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-project-diagram fa-lg")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("title", "Lista trazabilidad en modo gráfico")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Lista trazabilidad en modo gráfico")
                    ahtml.Attributes.Add("idd", refgrid.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "e_c_d_005")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-ballot-check fa-lg")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("title", "Lista autorizaciones")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Lista autorizaciones")
                    ahtml.Attributes.Add("idd", refgrid.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "e_c_d_006")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-info-square fa-lg")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("title", "Detalle del radicado")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-warning btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Detalle del radicado")
                    ahtml.Attributes.Add("idd", refgrid.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "e_c_d_008")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fas fa-folder-open fa-lg")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("title", "Ver documentos")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos")
                    ahtml.Attributes.Add("idd", refgrid.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "e_c_d_007")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    divhtml.Style.Add("display", "inline-flex")
                    refgrid.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To refgrid.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            refgrid.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            refgrid.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                label_resultado.Text = "Se encontraron (" & refgrid.Rows.Count & ") registro (s)  "
                updatelabel.Update()

            End If
            Consulta_tareas_workflow = "YES"
        Catch ex As Exception
            Consulta_tareas_workflow = "Error General  funcion Consulta datos tarea" & ex.Message
        End Try
    End Function
    Function Retorna_id_area_id_tarea_recuperada(ByVal id_tarea As Integer,
                                                 ByRef id_actvidad As Integer) As String
        '---------------------------------------------------
        'Funcón : Retorna el id actividad de la tarea
        'con el parametro id actividad
        'Ing Miguel Angel Urueta Miranda
        'Fecha 2017-05-17
        '---------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select Id_Actividad from " &
                   " estados_tarea_workflow WHERE Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta =" & HttpContext.Current.Session("Id_Ruta_Workflow") &
                   " AND Inicio_Tareas_Workflow_id_Tarea=" & id_tarea & " and FECHA_FIN IS NULL "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_area_id_tarea_recuperada = " Error Función Retorna_id_area_id_tarea_recuperada " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_area_id_tarea_recuperada = " Imposible encontrar el id " & id_tarea & " en la tabla estados tarea"
                Exit Function
            Else
                id_actvidad = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_area_id_tarea_recuperada = "YES"
            End If
        Catch ex As Exception
            Retorna_id_area_id_tarea_recuperada = "Inconsistencia general función Retorna_id_area_id_tarea_recuperada " & ex.Message
        End Try
    End Function

    Function Listar_Posibles_Datos(ByVal Name_Control As String,
                                          ByVal Valor_Text As String,
                                          ByRef COMBO2 As DropDownList) As String
        Try
            Dim Nombre_Ruta As String = ""
            Dim Result As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Listar_Posibles_Datos = Result
                Exit Function
            End If
            Dim Sql_consulta As String = "SELECT DISTINCT " & Name_Control & " FROM DAT_ADIC_TAR" & Nombre_Ruta &
            " WHERE " & Name_Control & " LIKE " & "'%" & Valor_Text & "%'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DAT_ADIC_TAR")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Listar_Posibles_Datos = "Error Consultando en tabla " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_Posibles_Datos = "YES"
                Exit Function
            Else
                COMBO2.Items.Clear()
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    COMBO2.Items.Add(Datset.Tables(0).Rows(i).Item(0).ToString)
                Next
                Listar_Posibles_Datos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_Posibles_Datos = "Inconsistencia general funcion Listar_Posibles_Datos " & ex.Message
        End Try
    End Function

    Function Interface_consulta_tarea_wrokflow(ByRef Page1 As Page) As String
        Try
            Dim Update As New UpdatePanel
            Update.ID = "Recupera"
            Update.UpdateMode = UpdatePanelUpdateMode.Conditional
            Dim Matri_Campos_Lista() As String = Nothing
            Dim Result As String = ""
            Dim Nombre_Ruta As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Dim refclas_radicado As New ClassRadicador
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Interface_consulta_tarea_wrokflow = Result
                Exit Function
            End If
            Dim Sql_consulta As String = "Select NOMBRE_CAMPO,TIPO_CAMPO from " &
                " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & HttpContext.Current.Session("Id_Ruta_Workflow") &
                " AND LISTA_TAREA=1 ORDER BY ID_CAMPO"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Interface_consulta_tarea_wrokflow = "Error Consultando Configuracion listado ruta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Interface_consulta_tarea_wrokflow = " Error # 01 Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA" & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Erase Matri_Campos_Lista
                Dim i As Integer = 0
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Campos_Lista(i)
                    Matri_Campos_Lista(i) = Datset.Tables(0).Rows(i).Item(0).ToString & "|" & Datset.Tables(0).Rows(i).Item(1).ToString
                Next
                Dim Table As Table = Page1.FindControl("TableControles")
                Table.Attributes.Add("width", "100%")
                Dim objRow As TableRow
                Dim objCell As TableCell
                Dim Icontr As Integer = 0
                Dim Z As Integer = 0
                Dim z_ As Integer = 0
                Dim m_TextBoxes() As TextBox = {}
                Dim m_TextBoxes_() As TextBox = {}
                Dim LabelBox() As Label = {}
                Dim Matri_CampoES() As String
                Erase Matri_CampoES
                Dim pane As Panel = Page1.FindControl("Panel1")
                For Z = 0 To Matri_Campos_Lista.Length - 1
                    Erase Matri_CampoES
                    Matri_CampoES = Matri_Campos_Lista(Z).Split("|")
                    If Matri_CampoES(1) = "datetime" Or Matri_CampoES(1) = "date" Then
                        objRow = New TableRow()
                        objRow.Attributes.Add("width", "100%")
                        objCell = New TableCell
                        ReDim Preserve LabelBox(Z)
                        LabelBox(Z) = New Label
                        LabelBox(Z).Attributes.Add("class", "h6 font-weight-light mt-2")
                        LabelBox(Z).Text = Matri_CampoES(0)
                        LabelBox(Z).ID = Matri_CampoES(0).ToString & "__"
                        objCell.Controls.Add(LabelBox(Z))
                        objRow.Cells.Add(objCell)
                        Table.Rows.Add(objRow)
                        ReDim Preserve m_TextBoxes(Z)
                        objRow = New TableRow()
                        objRow.Attributes.Add("width", "100%")
                        objCell = New TableCell
                        m_TextBoxes(Z) = New TextBox
                        m_TextBoxes(Z).Text = ""
                        m_TextBoxes(Z).ID = Matri_CampoES(0).ToString
                        m_TextBoxes(Z).Attributes.Add("onDblClick", "presionBoton('" + Matri_CampoES(0).ToString + "-" _
                       + Matri_CampoES(1).ToString + "')")
                        objCell.Controls.Add(m_TextBoxes(Z))
                        Dim bhtml As New HtmlControls.HtmlGenericControl("button")
                        bhtml = New HtmlControls.HtmlGenericControl("button")
                        bhtml.Attributes.Add("class", "ml-1 mr-1 btn btn-success border-0")
                        bhtml.Attributes.Add("font-size", "10px")
                        bhtml.Attributes.Add("title", "formato aaaa mm dd")
                        bhtml.ID = "Fecha_ela_" & Matri_CampoES(0)
                        Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Attributes.Add("class", "fad fa-calendar-alt fa-1x")
                        bhtml.Controls.Add(ihtml)
                        objCell.Controls.Add(bhtml)
                        Result = refclas_radicado.Agregar_Calendar(bhtml.ID, m_TextBoxes(Z).ID, pane)
                        If Result <> "YES" Then
                            'Genera_Interface_Radicacion_Entrante = Result
                            'Exit Function
                        End If
                        m_TextBoxes(Z).MaxLength = 10
                        m_TextBoxes(Z).Attributes.Add("onkeypress", "GetChar (event);")
                        m_TextBoxes(Z).Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                        m_TextBoxes(Z).Width = 95
                        m_TextBoxes(Z).Attributes.Add("class", "ml-0")
                        m_TextBoxes(Z).Attributes.Add("TIPO", "DATE")
                        m_TextBoxes(Z).Attributes.Add("placeholder", "yyyy mm dd")
                        m_TextBoxes(Z).CssClass = "DATE"

                        '-----------------------------------------------------
                        'Segundo campo date
                        '-----------------------------------------------------
                        ReDim Preserve m_TextBoxes_(z_)
                        m_TextBoxes_(z_) = New TextBox
                        m_TextBoxes_(z_).Text = ""
                        m_TextBoxes_(z_).ID = Matri_CampoES(0).ToString & "_"
                        m_TextBoxes_(z_).Attributes.Add("onDblClick", "presionBoton('" + Matri_CampoES(0).ToString + "-" _
                       + Matri_CampoES(1).ToString + "')")
                        objCell.Controls.Add(m_TextBoxes_(z_))
                        m_TextBoxes_(z_).MaxLength = 10
                        m_TextBoxes_(z_).Attributes.Add("onkeypress", "GetChar (event);")
                        m_TextBoxes_(z_).Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                        m_TextBoxes_(z_).Attributes.Add("placeholder", "yyyy mm dd")
                        m_TextBoxes_(z_).Width = 95
                        m_TextBoxes_(z_).Attributes.Add("class", "ml-0")
                        m_TextBoxes_(z_).CssClass = "DATE"
                        m_TextBoxes_(z_).Attributes.Add("TIPO", "DATE_")

                        bhtml = New HtmlControls.HtmlGenericControl("button")
                        bhtml.Attributes.Add("class", "ml-1 btn btn-success border-0")
                        bhtml.Attributes.Add("font-size", "10px")
                        bhtml.Attributes.Add("title", "formato aaaa mm dd")
                        bhtml.ID = "Fecha_ela__" & Matri_CampoES(0)

                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.ID = "Fecha_ela__m" & Matri_CampoES(0)
                        ihtml.Attributes.Add("class", "fad fa-calendar-alt fa-1x")
                        bhtml.Controls.Add(ihtml)
                        objCell.Controls.Add(bhtml)
                        Result = refclas_radicado.Agregar_Calendar(bhtml.ID, m_TextBoxes_(z_).ID, pane)
                        If Result <> "YES" Then
                            'Genera_Interface_Radicacion_Entrante = Result
                            'Exit Function
                        End If
                        objRow.Cells.Add(objCell)
                        Table.Rows.Add(objRow)
                        Result = agregar_auto_complete_workflow(m_TextBoxes(Z).ID, pane, "GetPosiblesDatos", "DAT_ADIC_TAR" & Nombre_Ruta, m_TextBoxes(Z).ID)
                        If Result <> "YES" Then
                            Interface_consulta_tarea_wrokflow = Result
                            Exit Function
                        End If
                        Result = agregar_auto_complete_workflow(m_TextBoxes_(z_).ID, pane, "GetPosiblesDatos", "DAT_ADIC_TAR" & Nombre_Ruta, m_TextBoxes_(z_).ID)
                        If Result <> "YES" Then
                            Interface_consulta_tarea_wrokflow = Result
                            Exit Function
                        End If
                        z_ = z_ + 1
                    Else
                        objRow = New TableRow()
                        objRow.Attributes.Add("width", "100%")
                        objCell = New TableCell
                        ReDim Preserve m_TextBoxes(Z)
                        ReDim Preserve LabelBox(Z)
                        LabelBox(Z) = New Label
                        LabelBox(Z).Attributes.Add("class", "h6 font-weight-light mt-2")
                        m_TextBoxes(Z) = New TextBox
                        LabelBox(Z).Text = Matri_CampoES(0)
                        LabelBox(Z).ID = Matri_CampoES(0).ToString & "__"
                        m_TextBoxes(Z).Text = ""
                        m_TextBoxes(Z).ID = Matri_CampoES(0).ToString
                        m_TextBoxes(Z).Attributes.Add("onDblClick", "presionBoton('" + Matri_CampoES(0).ToString + "-" _
                       + Matri_CampoES(1).ToString + "')")
                        objCell.Controls.Add(LabelBox(Z))
                        objCell.Controls.Add(m_TextBoxes(Z))
                        m_TextBoxes(Z).Attributes.Add("TIPO", "")
                        m_TextBoxes(Z).Style.Add("width", "98%")
                        objRow.Cells.Add(objCell)
                        Table.Rows.Add(objRow)
                        Result = agregar_auto_complete_workflow(m_TextBoxes(Z).ID, pane, "GetPosiblesDatos", "DAT_ADIC_TAR" & Nombre_Ruta, m_TextBoxes(Z).ID)
                        If Result <> "YES" Then
                            Interface_consulta_tarea_wrokflow = Result
                            Exit Function
                        End If
                    End If

                    'Dim trigText As New AsyncPostBackTrigger()
                    'trigText.ControlID = m_TextBoxes(Z).ID
                    'Update.Triggers.Add(trigText)
                Next
                pane.Controls.Add(Table)
                Interface_consulta_tarea_wrokflow = "YES"
                Exit Function
            Else
                Interface_consulta_tarea_wrokflow = "Imposible encontrar campos ruta para listar"
                Exit Function
            End If
        Catch ex As Exception
            Interface_consulta_tarea_wrokflow = "Inconsistencia general función Interface_consulta_tarea_wrokflow " & ex.Message
        End Try
    End Function
    Function agregar_auto_complete_workflow(ByVal id_tex As String,
                                            ByRef pnae As Panel,
                                            ByVal ruta_webservice As String,
                                            ByVal tabla As String,
                                            ByVal campo As String) As String
        '***************************************************************
        'Funcion : Agrega control autocomplete, debe agregar funcion
        'java onDataShown para navegador chrome
        'Fecha : 2014-08-21
        'Ingeniero : Miguel Angel Urueta Miranda
        '***************************************************************
        Try
            Dim Auto As New AutoCompleteExtender
            Auto.TargetControlID = id_tex
            Auto.MinimumPrefixLength = 2
            Auto.EnableCaching = True
            Auto.CompletionSetCount = 10
            Auto.CompletionInterval = 50
            Auto.ServiceMethod = ruta_webservice
            Auto.ServicePath = "../webservice/WebServiceWorkflow.asmx"
            Auto.ContextKey = campo & "|" & tabla
            Auto.UseContextKey = True
            Auto.CompletionSetCount = 20
            'Auto.OnClientShown = "onDataShown"
            Auto.CompletionListCssClass = "completionList"
            Auto.CompletionListHighlightedItemCssClass = "itemHighlighted"
            Auto.CompletionListItemCssClass = "listItem"
            pnae.Controls.Add(Auto)
            agregar_auto_complete_workflow = "YES"
        Catch ex As Exception
            agregar_auto_complete_workflow = "Inconsistencia fucnion agregar_auto_complete " & ex.Message
        End Try
    End Function
    Private Sub DropDownListActividad_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim Refclas As New ClassGestorSesion
        Dim Mesaje As New Classscrripjava
        Dim Result As String = ""
        Dim Pag As Page = sender.Page
        Consulta_Datos_Actividad(Pag)
        If Result <> "YES" Then
            Mesaje.Show(Result)
        End If

    End Sub
    Function Retorna_id_pendiente_tarea_pendiente_aprobacion(ByVal id_tarea As Integer,
                                                             ByRef id_pendiente As Integer) As String
        Try
            Dim Ref2 As New ClassListandoTareas
            Dim Id_Activida_User As String = ""
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Dim Result As String = ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida_User,
                                                                                                     HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Result <> "YES" Or Id_Activida_User = "" Then
                Retorna_id_pendiente_tarea_pendiente_aprobacion = "Imposible Obtener Id actividad "
                Exit Function
            End If
            Dim Sql_consulta As String = "SELECT Id_Pendiente FROM TAREA_PENDIENTE as tp " &
             " WHERE INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" &
              HttpContext.Current.Session("Id_Ruta_Workflow") & " AND ID_USUARIO=" & HttpContext.Current.Session("Id_Usuario_Workflow") &
              " and inicio_tareas_workflow_id_tarea=" & id_tarea &
              " AND ID_ACTIVIDAD=" & Id_Activida_User & " AND ESTADOS_PENDIENTE=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_pendiente_tarea_pendiente_aprobacion = "Error Función Retorna_id_pendiente_tarea_pendiente_aprobacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_pendiente_tarea_pendiente_aprobacion = "La tarea no se encuentra en estado pendiente"
                Exit Function
            Else
                id_pendiente = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_pendiente_tarea_pendiente_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_pendiente_tarea_pendiente_aprobacion = "Inconsistencia general función Retorna_id_pendiente_tarea_pendiente_aprobacion  Error : " & ex.Message
        End Try
    End Function
    Function Terminar_Tarea_Workflow(ByVal Id_Usuario_Destino As String,
                                     ByVal Id_Actividad_Destino As String,
                                     ByVal id_Tarea As Long,
                                     ByVal Nombre_Activ As String,
                                     ByRef pag As Page,
                                     ByRef Resultado_evalua_terminar As String,
                                     Optional ByVal notifica As Integer = 0,
                                     Optional ByRef resultado_correo As String = "",
                                     Optional ByVal id_flujo_trabajo As Integer = 0,
                                     Optional ByVal id_actividad_flujo_trabajo As Integer = 0,
                                     Optional ByVal id_usuario_workflow_flujo_trabajo As Integer = 0,
                                     Optional ByVal notifica_envio_correo As Integer = 0,
                                     Optional ByVal id_conector As Integer = 0,
                                     Optional ByVal id_usuario_wf_envia As Integer = 0,
                                     Optional ByVal id_actividad_wf_envia As Integer = 0,
                                     Optional ByVal activa_actualizacion_paramtros_interface As Integer = 1,
                                     Optional ByVal activa_evento_dinamicos As Integer = 1,
                                     Optional ByVal activa_reasigna_sii As Integer = 0,
                                     Optional ByVal activa_reasigna_tarea_workflow As Integer = 0) As String
        Dim Ref As New ClassWorkflow
        Dim Ref2 As New ClassListandoTareas
        Dim Result As String = ""
        Dim Id_Actividad As String = "0"
        Dim Id_Activida_User As String = ""
        Dim Estado_Prioridad As String = ""
        Dim id_usuario_gestion As Integer = 0
        Dim ref_usuario_destino As Integer = 0
        Resultado_evalua_terminar = "YES"
        Try
            If id_Tarea = "0" Then
                Terminar_Tarea_Workflow = "YES"
                Exit Function
            End If
            '-----------------------------------------
            'Consulta el id de la actividad destino
            '-----------------------------------------
            Dim Ref_class_list_actividades As New Class_Listado_Actividades_workflow
            If Id_Actividad_Destino = "" Then
                Result = Ref_class_list_actividades.Obtener_Id_Actividad(Id_Actividad_Destino,
                                                                         Nombre_Activ)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = "Error Consultando id actividad destino " + Result
                    Exit Function
                End If
                If Id_Actividad_Destino = "0" Then
                    Terminar_Tarea_Workflow = "Debe Seleccionar Actividad destino "
                    Exit Function
                End If
            End If
            Dim stru_config_conector_ruta As stru_config_conector_ruta = Nothing
            Dim stru_config_conector_flujo As stru_config_conector_flujo = Nothing
            Dim Class_actividades_disponibles_envio As New Class_actividades_disponibles_envio
            Dim ref_conect As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Dim Refclass_usuario_workflow As New ClassWorkflowUsuario
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim imagenes_sin_expediente As Integer = 0
            Dim imagenes_sin_firma As Integer = 0
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Dim existencia As String = ""
            Dim nombre_campo_radicado As String = ""
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Dim Radicado As String = ""
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                              nombre_campo_radicado)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow = Result
                Exit Function
            End If
            '------------------------------------
            'Retorna radicado tarea seleccionada
            '------------------------------------
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Val(id_Tarea),
                                                                                    nombre_campo_radicado,
                                                                                    HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                    Radicado)
            If Result <> "YES" Then
                resultado_correo = Result
                Terminar_Tarea_Workflow = "YES"
                Exit Function
            End If
            '----/////////////-------------////////----------------//////-------------
            'ZONA DE DIRECIONAMIENTO POR TIPOS DE ACTIVIDADES
            '----/////////////-------------////////----------------//////-------------
            '-------------------------------------------------------
            'Solicita el tipo de actividad de la actividad destino
            'y la estructura de la actividad destino
            '-------------------------------------------------------
            Dim id_tipo_actividad As Integer = 0
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim Class_actividades_generales_workflow As New Class_actividades_generales_workflow
            Dim actividades_generales_workflow_ As actividades_generales_workflow = Nothing
            If Id_Actividad_Destino <> "" Then
                Result = Class_Listado_Actividades_workflow.Solicita_id_tipo_actividad_workflow(Val(Id_Actividad_Destino),
                                                                                                id_tipo_actividad)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                Result = Class_actividades_generales_workflow.Solicita_estructura_tipo_actividad_workflow(id_tipo_actividad,
                                                                                                          actividades_generales_workflow_)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
            End If
            '--------------------------------------------------------
            'ZONA DIRECIONA TAREA A USUARIO RESPONSABLE DEL RADICADO
            'ACTIVIDAD VIRTUAL QUE REPRESENTA EL USUARIO A QUIEN SE
            'LE RADICA LA TAREA, ES DECIR EL DESTINATAIO.
            'NO ES COMPATIBLE CON FLUJOS EXTERNOS
            '--------------------------------------------------------
            Dim Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
            Dim Class_plantillas_radicacion As New Class_plantillas_radicacion
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Dim Class_grupos_workflow As New Class_grupos_workflow
            Dim nombre_plantilla_radicado As String = ""
            Dim id_grupo_usuario_workflow As Integer = 0
            Dim id_usuario_worflow_destino As Integer = 0
            Dim id_actividad_destino_asig As Integer = 0
            Dim id_usuario_workflow_flujo_trabajo_send_correo As Integer = 0
            If actividades_generales_workflow_.Nombre_tipo_actividad = "USUARIORESPONSABLE" Then
                Result = Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                              nombre_plantilla_radicado)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                Dim id_usuario_gestion_radicado As Integer = 0
                Result = Class_plantillas_radicacion.Solicita_remitente_destinatario_fecha_radicado(nombre_plantilla_radicado,
                                                                                                    Radicado,
                                                                                                    id_usuario_gestion_radicado,
                                                                                                    0,
                                                                                                    "")
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                '----Solicita usuario workflow relacionado
                Result = Class_remit_dest_interno.Solicita_id_usuario_workflow_relacionado(id_usuario_gestion_radicado,
                                                                                           id_usuario_worflow_destino)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                '----Solicita id grupo usuario workflow
                Result = Class_usuario_workflow.Solicita_id_grupo_usuario_workflow(id_usuario_worflow_destino,
                                                                                   id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                '----Solicita id actividad relacionado al grupo workflow
                Result = Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(id_actividad_destino_asig,
                                                                                      id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                Id_Actividad_Destino = id_actividad_destino_asig
                Id_Usuario_Destino = id_usuario_worflow_destino
                If id_flujo_trabajo <> 0 Then
                    id_usuario_workflow_flujo_trabajo_send_correo = id_usuario_worflow_destino
                End If
            End If
            '--------------------------------------------------------
            'ZONA DIRECIONA TAREA A USUARIO RADICADOR
            'ACTIVIDAD VIRTUAL QUE REPRESENTA EL USUARIO QUE RADICA
            'LA TAREA, NOS ES COMPATIBLE CON FLUJOS EXTERNOS
            '--------------------------------------------------------
            Dim Class_ra_usuario_radicador As New Class_ra_usuario_radicador
            If actividades_generales_workflow_.Nombre_tipo_actividad = "USUARIORESPONSABLERADICADOR" Then
                Result = Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                              nombre_plantilla_radicado)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                Dim id_usuario_radicador As Integer = 0
                Result = Class_plantillas_radicacion.Solicita_id_usuario_radicacion_plantilla_radicado(nombre_plantilla_radicado,
                                                                                                       Radicado,
                                                                                                       id_usuario_radicador)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                Dim id_usuario_gestion_rel_radicacion As Integer = 0
                Result = Class_ra_usuario_radicador.Solicita_id_usuario_gestion_relacion_usuario_radicador(id_usuario_radicador,
                                                                                                           id_usuario_gestion_rel_radicacion)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                If id_usuario_gestion_rel_radicacion = 0 Then
                    Terminar_Tarea_Workflow = "El codigo de usuario radicador ( " & Val(id_usuario_radicador) & " ) no registra relación de usuario de gestión"
                    Exit Function
                End If
                '----Solicita usuario workflow relacionado
                Result = Class_remit_dest_interno.Solicita_id_usuario_workflow_relacionado(id_usuario_gestion_rel_radicacion,
                                                                                           id_usuario_worflow_destino)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                '----Solicita id grupo usuario workflow
                Result = Class_usuario_workflow.Solicita_id_grupo_usuario_workflow(id_usuario_worflow_destino,
                                                                                   id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                '----Solicita id actividad relacionado al grupo workflow
                Result = Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(id_actividad_destino_asig,
                                                                                      id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
                Id_Actividad_Destino = id_actividad_destino_asig
                Id_Usuario_Destino = id_usuario_worflow_destino
                If id_flujo_trabajo <> 0 Then
                    id_usuario_workflow_flujo_trabajo_send_correo = id_usuario_worflow_destino
                End If
            End If
            If Id_Usuario_Destino = "" Then
                ref_usuario_destino = 0
            Else
                ref_usuario_destino = Val(Id_Usuario_Destino)
            End If
            '----/////////////-------------////////----------------//////-------------
            'TERMINA ZONA DE DIRECIONAMIENTO POR TIPOS DE ACTIVIDADES
            '----/////////////-------------////////----------------//////-------------


            '-------------------------------------------------------
            'ZONA VALIDACION  DE LAS RESTRICIONES DE ENVIO POR 
            'CONECTOR - 1 
            '1 EXISTENCIA AUTORIZACION TAREA
            '2 EXISTENCIA DE FIRMA DIGITAL
            '3 EXISTENCIA DE COPIA A ESTRUCTURA
            '4 EXISTENCIA DE ASIGNACION EXPEDIENTE
            '5 EXISTENCIA DE ASIGACION TOTAL AL EXPEDIENTE
            '6 EXITENCIA DE TIPOLOGIAS OBLIGATORIAS
            '------------------------------------------------------
            If id_conector <> 0 Then
                If id_flujo_trabajo <> 0 Then
                    Result = ref_conect.Solicita_configuracion_conector_flujo(id_conector,
                                                                              stru_config_conector_flujo)
                    If Result <> "YES" Then
                        Terminar_Tarea_Workflow = Result
                        Exit Function
                    End If
                    '----------- Valida solicitud de autorización de la tarea ---------------------------------
                    If stru_config_conector_flujo.Estado_soicita_autorizacion = 1 Then
                        Result = Class_autoriza_tarea_worklfow.SolicitaExistenciaAutorizacion(id_Tarea,
                                                                                                id_actividad_wf_envia,
                                                                                                id_usuario_wf_envia,
                                                                                                existencia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If existencia = "NO" Then
                            Terminar_Tarea_Workflow = "Debe autorizar la tarea antes de terminarla"
                            Exit Function
                        End If
                    End If
                    '-------------------------Valida aturización firma digital ---------------------------
                    If stru_config_conector_flujo.Estado_soicita_autorizacion_firma_digital = 1 Then
                        Result = Class_autoriza_tarea_worklfow.Solicita_existencia_firma_autorizacion(id_Tarea,
                                                                                                      id_actividad_wf_envia,
                                                                                                      id_usuario_wf_envia,
                                                                                                      existencia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If existencia = "NO" Then
                            Terminar_Tarea_Workflow = "Debe firmar digitalmente la autorización antes de terminarla"
                            Exit Function
                        End If
                    End If
                    '-------------------------Valida la copia de todos los documentos a expediente ---------
                    If stru_config_conector_flujo.Estado_copia_estructura_total = 1 Then
                        Dim stru_paramter_image_final As stru_paramter_image() = Nothing
                        Dim refclas As New Classselecciotarea
                        Result = refclas.Solicita_lista_id_producion_relacionados_tarea_workflow(id_Tarea,
                                                                                                 stru_paramter_image_final)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If stru_paramter_image_final Is Nothing Then
                            Terminar_Tarea_Workflow = "Imposible encontrar imagenes para de la tarea  para validar la copia al expediente, imposible terminar la tarea"
                            Exit Function
                        End If
                        Dim Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
                        Dim _ra_rel_copia_wf_produccion() As ra_rel_copia_wf_produccion = Nothing
                        Result = Class_ra_rel_copia_wf_produccion.Solicita_estrucutura_copia_documento_expediente(id_Tarea,
                                                                                                                  HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                                                  _ra_rel_copia_wf_produccion)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If _ra_rel_copia_wf_produccion Is Nothing Then
                            Terminar_Tarea_Workflow = "Debe copiar los documentos de la tarea a un expediente  "
                            Exit Function
                        End If
                        '-------------Compara los archivos que no se han copiado a expdiente
                        Dim Documentos_no_copy As String = ""
                        Dim shutwch As Integer = -1
                        Dim contador_document As Integer = 0
                        For i As Integer = 0 To stru_paramter_image_final.Length - 1
                            shutwch = -1
                            For k As Integer = 0 To _ra_rel_copia_wf_produccion.Length - 1
                                If stru_paramter_image_final(i).ID = _ra_rel_copia_wf_produccion(k).id_imagen_da Then
                                    shutwch = 1
                                    'Exit For
                                End If
                            Next
                            If shutwch = -1 Then
                                contador_document += 1
                                If stru_paramter_image_final(i).TIPODOCUMENTO = "" Then
                                    Documentos_no_copy = Documentos_no_copy & stru_paramter_image_final(i).ID & " @ "
                                Else
                                    Documentos_no_copy = Documentos_no_copy & stru_paramter_image_final(i).TIPODOCUMENTO & " @ "
                                End If
                            End If
                        Next
                        If Documentos_no_copy <> "" Then
                            Dim tex_normalice As String = Documentos_no_copy.Normalize(NormalizationForm.FormD)
                            Dim reg As Regex = New Regex("[^a-zA-Z0-9 ]")
                            Dim textoSinAcentos As String = reg.Replace(tex_normalice, "")
                            Terminar_Tarea_Workflow = "Debe copiar (" & contador_document & ") documento(s) a un expediente (" & textoSinAcentos & ")"
                            Exit Function
                        End If
                    End If
                    '-------------------------Valida la copia de algunos documentos a expediente ----------
                    If stru_config_conector_flujo.Estado_copia_documento_estructura = 1 Then
                        Dim Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
                        Dim numero_documentos_copia As Integer = 0
                        Result = Class_ra_rel_copia_wf_produccion.Solicita_existencia_numero_imagenes_copiadas(id_Tarea,
                                                                                                               HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                                               numero_documentos_copia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If numero_documentos_copia = 0 Then
                            Terminar_Tarea_Workflow = "Debe copiar al menos un documento de la tarea a un expediente  "
                            Exit Function
                        End If
                    End If
                    '-------------------------------------------------------
                    'Valida estado firma digital y asinación de expediente 
                    '-------------------------------------------------------
                    If stru_config_conector_flujo.Estado_firma_digital = 1 Or stru_config_conector_flujo.Estado_asigna_expediente = 1 Then
                        Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                         id_Tarea,
                                                                                                        structure_datos_tarea_workflow)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        Result = Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                          structure_gabinete_workflow)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        Result = ClassDaGabinete.Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                                                                  Radicado,
                                                                                                                                  imagenes_sin_expediente,
                                                                                                                                  imagenes_sin_firma)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If stru_config_conector_flujo.Estado_firma_digital = 1 And imagenes_sin_firma > 0 Then
                            Terminar_Tarea_Workflow = "Faltan " & imagenes_sin_firma & " documento (s) por firma digital, imposible terminar la tarea "
                            Exit Function
                        End If
                        If stru_config_conector_flujo.Estado_asigna_expediente = 1 And imagenes_sin_expediente > 0 Then
                            Terminar_Tarea_Workflow = "Faltan " & imagenes_sin_expediente & " documento (s) por vincular al  expediente, imposible terminar la tarea "
                            Exit Function
                        End If
                    End If
                Else
                    Result = Class_actividades_disponibles_envio.Solicita_configuracion_conector_ruta(id_conector,
                                                                                                      stru_config_conector_ruta)
                    If Result <> "YES" Then
                        Terminar_Tarea_Workflow = Result
                        Exit Function
                    End If
                    If stru_config_conector_ruta.Estado_soicita_autorizacion = 1 Then
                        Result = Class_autoriza_tarea_worklfow.SolicitaExistenciaAutorizacion(id_Tarea,
                                                                                               id_actividad_wf_envia,
                                                                                               id_usuario_wf_envia,
                                                                                               existencia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If existencia = "NO" Then
                            Terminar_Tarea_Workflow = "Debe autorizar la tarea antes de terminarla"
                            Exit Function
                        End If
                    End If
                    If stru_config_conector_ruta.Estado_soicita_autorizacion_firma_digital = 1 Then
                        Result = Class_autoriza_tarea_worklfow.Solicita_existencia_firma_autorizacion(id_Tarea,
                                                                                                         id_actividad_wf_envia,
                                                                                                         id_usuario_wf_envia,
                                                                                                         existencia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If existencia = "NO" Then
                            Terminar_Tarea_Workflow = "Debe firmar digitalmente la autorización antes de terminarla"
                            Exit Function
                        End If
                    End If
                    If stru_config_conector_ruta.Estado_copia_documento_estructura = 1 Then
                        Dim stru_paramter_image_final As stru_paramter_image() = Nothing
                        Dim refclas As New Classselecciotarea
                        Result = refclas.Solicita_lista_id_producion_relacionados_tarea_workflow(id_Tarea,
                                                                                                 stru_paramter_image_final)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        Dim Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
                        Dim numero_documentos_copia As Integer = 0
                        Result = Class_ra_rel_copia_wf_produccion.Solicita_existencia_numero_imagenes_copiadas(id_Tarea,
                                                                                                               HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                                               numero_documentos_copia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If numero_documentos_copia < stru_paramter_image_final.Length Then
                            Terminar_Tarea_Workflow = "Debe copiar los documentos de la tarea a un expediente  "
                            Exit Function
                        End If
                    End If
                    '-------------------------------------------------------
                    'Valida estado firma digital y asinación de expediente 
                    '-------------------------------------------------------
                    If stru_config_conector_ruta.Estado_firma_digital = 1 Or stru_config_conector_ruta.Estado_asigna_expediente = 1 Then
                        Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                         id_Tarea,
                                                                                                        structure_datos_tarea_workflow)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        Result = Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                          structure_gabinete_workflow)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        Result = ClassDaGabinete.Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                                                                  Radicado,
                                                                                                                                  imagenes_sin_expediente,
                                                                                                                                  imagenes_sin_firma)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        If stru_config_conector_ruta.Estado_firma_digital = 1 And imagenes_sin_firma > 0 Then
                            Terminar_Tarea_Workflow = "Faltan " & imagenes_sin_firma & " documento (s) por firma digital, imposible terminar la tarea "
                            Exit Function
                        End If
                        If stru_config_conector_ruta.Estado_asigna_expediente = 1 And imagenes_sin_expediente > 0 Then
                            Terminar_Tarea_Workflow = "Faltan " & imagenes_sin_expediente & " documento (s) por vincular al  expediente, imposible terminar la tarea "
                            Exit Function
                        End If
                    End If

                End If
            End If
            Id_Actividad = Id_Actividad_Destino
            Dim ResultadoComp As String = ""
            Dim Conection_conectro_C = "Persist Security Info=" _
                    & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                    & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                   & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                   & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString
            Dim Id_User As String = HttpContext.Current.Session("Id_Usuario_Workflow").ToString
            Dim mParam() As Object = {Conection_conectro_C,
                                      HttpContext.Current.Session("Id_Usuario_Workflow").ToString,
                                      id_Tarea,
                                      HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                      ref_usuario_destino,
                                      Id_Actividad,
                                      id_flujo_trabajo,
                                      id_actividad_flujo_trabajo,
                                      id_usuario_workflow_flujo_trabajo}
            Dim Resultado1 As String = ""
            Dim refcla As New ClassEdtiScript
            '-------------------------------------------------
            'ZONA EVENTO DINAMICO PRETERMINAR ACTIVIDAD
            '-------------------------------------------------
            If activa_evento_dinamicos = 1 Then
                If HttpContext.Current.Session("PRETERMINARACTIVIAD") <> "" Then
                    Resultado1 = refcla.Compila_Evalua(ResultadoComp, HttpContext.Current.Session("PRETERMINARACTIVIAD"), "PRETERMINARACTIVIAD", mParam)
                    If Resultado1 <> "YES" Then
                        Terminar_Tarea_Workflow = "Error #13 Error Compilando Function PRETERMINARACTIVIAD" & Resultado1
                        Exit Function
                    End If
                    If ResultadoComp <> "YES" Then
                        Terminar_Tarea_Workflow = "Imposible eviar la tarea  " & ResultadoComp
                        Exit Function
                    End If
                End If
            End If
            '-------------------------------------------------------
            'Solicita estructura de la tarea asignada
            '-------------------------------------------------------
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim stru_estado As stru_estado = Nothing
            Result = Class_estados_tarea_workflow.Solicita_estructura_tarea_asignada(id_Tarea,
                                                                                     stru_estado)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow = Result
                Exit Function
            End If
            '------------------------------------------------------
            'ZONA VALIDACION TAREA TOMADA O ASIGNADA POR OTRO 
            'USUARIO NO APLICA PARA CASOS DE REASIGNACION
            '------------------------------------------------------
            Dim nombre_usuario_asignado_tarea As String = ""
            Dim cargo_usuario_asignado_tarea As String = ""
            If stru_estado.Fecha_Seleccion <> "" And stru_estado.Id_Usuario <> 0 And activa_reasigna_sii = 0 And activa_reasigna_tarea_workflow = 0 Then
                If stru_estado.Id_Usuario <> HttpContext.Current.Session.Item("Id_Usuario_Workflow") Then
                    Class_usuario_workflow.Solicita_nombre_cargo_usuario_workflow(stru_estado.Id_Usuario,
                                                                                  nombre_usuario_asignado_tarea,
                                                                                  cargo_usuario_asignado_tarea)
                    If Result <> "YES" Then
                        Terminar_Tarea_Workflow = "La tarea esta asignada a otro usuario, sin embargo no se pudo identificar por lo siguiente : " & Result
                        Exit Function
                    Else
                        Terminar_Tarea_Workflow = "El usuario (" & nombre_usuario_asignado_tarea & ") del cargo (" & cargo_usuario_asignado_tarea & ") tiene la tarea asignada, es posible que esta tarea sea grupal y el usuario ya selecciono la tarea. Por favor actualice su lista de tareas."
                        Exit Function
                    End If
                End If
            End If
            Id_Activida_User = stru_estado.Id_Actividad.ToString
            Dim estado_prioridad_tarea As Integer = stru_estado.Estado_Prioridad
            Dim id_ruta_tarea As Integer = stru_estado.id_Ruta
            Dim Minuto_Dur As Long = 0
            Dim Fecha_Fromat As String = ""
            Dim DateCreate As Date = Now
            Dim fecha_seleccion_bd As String = ""
            Dim fecha_seleccion_fm As String = ""
            '---------------------------------------------------------------
            'Formatea framework actual para registro de base de datos mysql
            '---------------------------------------------------------------
            Dim Refclas_gestion_fecha As New ClassGestionFechas
            Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                         Fecha_Fromat)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow = Result
                Exit Function
            End If
            '---------------------------------------------------------------------------
            'Caso fecha de asignación sin datos caso en que no esta asignada la tarea
            'cuando se reasigna la tarea
            '--------------------------------------------------------------------------
            If stru_estado.Fecha_Seleccion = "" Then
                fecha_seleccion_fm = Fecha_Fromat
            Else
                Refclas_gestion_fecha.Formatea_fecha_time_base_mysql(stru_estado.Fecha_Seleccion,
                                                                     fecha_seleccion_fm)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
            End If
            '-----------------------------------
            'Solicita los minutos de diferencia
            '-----------------------------------
            Result = Refclas_gestion_fecha.Resta_fechas_db(fecha_seleccion_fm,
                                                           Fecha_Fromat,
                                                           Minuto_Dur)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow = Result
                Exit Function
            End If
            If Minuto_Dur <= -1 Then
                Minuto_Dur = CInt(Abs(Minuto_Dur))
            End If
            '------------------------------------------
            'Solicita tiempo total tarea
            '------------------------------------------
            Dim Minuto_Dur_final As Long = 0
            Dim fecha_asignacion_inicial As String = ""
            Refclas_gestion_fecha.Formatea_fecha_time_base_mysql(stru_estado.Fecha_Inicio,
                                                                 fecha_asignacion_inicial)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow = Result
                Exit Function
            End If
            Result = Refclas_gestion_fecha.Resta_fechas_db(fecha_asignacion_inicial,
                                                           Fecha_Fromat,
                                                           Minuto_Dur_final)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow = Result
                Exit Function
            End If
            If Minuto_Dur_final <= -1 Then
                Minuto_Dur_final = CInt(Abs(Minuto_Dur_final))
            End If
            '---------------------------------------------------
            'ZONA DE BALANCEO POR FLUJO DE TRABAJO O RUTA
            '---------------------------------------------------
            Dim Class_wf_registro_asignacion_ruta As New Class_wf_registro_asignacion_ruta
            Dim Class_wf_registro_asignacion_flujo As New Class_wf_registro_asignacion_flujo
            Dim estado_registro_balanceo As Integer = 0
            If id_conector <> 0 Then
                If id_flujo_trabajo <> 0 Then
                    'If id_usuario_workflow_flujo_trabajo = 0 And stru_config_conector_flujo.estado_valida_balanceo = 1 And
                    '                             id_actividad_flujo_trabajo <> 0 Then
                    '    Result = Class_wf_registro_asignacion_flujo.balanceo_asignacion_por_conector_flujo(Val(Id_Actividad),
                    '                                                                                       id_actividad_flujo_trabajo,
                    '                                                                                       Fecha_Fromat,
                    '                                                                                       ref_usuario_destino)
                    '    If Result <> "YES" Then
                    '        Terminar_Tarea_Workflow = Result
                    '        Exit Function
                    '    End If
                    '    If ref_usuario_destino <> 0 Then
                    '        estado_registro_balanceo = 1
                    '    Else
                    '        estado_registro_balanceo = 0
                    '    End If
                    '    id_usuario_workflow_flujo_trabajo = ref_usuario_destino
                    'End If
                    If ref_usuario_destino = 0 And stru_config_conector_flujo.estado_valida_balanceo = 1 Then
                        Result = Class_wf_registro_asignacion_ruta.Balanceo_asignacion_por_conector_ruta(Val(Id_Actividad),
                                                                                                         Fecha_Fromat,
                                                                                                         ref_usuario_destino)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                        id_usuario_workflow_flujo_trabajo = ref_usuario_destino
                        If ref_usuario_destino <> 0 Then
                            estado_registro_balanceo = 1
                        Else
                            estado_registro_balanceo = 0
                        End If
                    End If
                Else
                    If ref_usuario_destino = 0 And stru_config_conector_ruta.estado_valida_balanceo = 1 Then
                        Result = Class_wf_registro_asignacion_ruta.Balanceo_asignacion_por_conector_ruta(Val(Id_Actividad),
                                                                                                         Fecha_Fromat,
                                                                                                         ref_usuario_destino)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If

                        If ref_usuario_destino <> 0 Then
                            estado_registro_balanceo = 1
                        Else
                            estado_registro_balanceo = 0
                        End If
                    End If
                End If
            End If
            '-------------------------------------------------------------
            'ZONA BALANCEO POR REASIGNACION SII O WORKFLOW BALANCEO
            '-------------------------------------------------------------
            If (activa_reasigna_sii = 1 Or activa_reasigna_tarea_workflow = 1) And ref_usuario_destino <> 0 And id_conector = 0 Then
                Dim estado_registro_sii As Integer = 0
                '--------------------------------------------------
                'Caso reasignación ruta
                '---------------------------------------------------
                If id_flujo_trabajo = 0 Then
                    '---Registra balanceo usuario destino incrementa actividades asignadas en 1-+1
                    Result = Class_wf_registro_asignacion_ruta.Registro_balanceo_reasignacion_ruta(Val(Id_Actividad),
                                                                                                       ref_usuario_destino,
                                                                                                       id_Tarea, 0,
                                                                                                       1,
                                                                                                       Fecha_Fromat,
                                                                                                       estado_registro_sii)
                    If Result <> "YES" Then
                        Terminar_Tarea_Workflow = Result
                        Exit Function
                    End If
                    estado_registro_balanceo = estado_registro_sii
                    '---Registra balanceo al usuario anterior decrementa actividades asignadas -1
                    If stru_estado.Id_Usuario <> 0 Then
                        Result = Class_wf_registro_asignacion_ruta.Registro_balanceo_reasignacion_ruta(stru_estado.Id_Actividad,
                                                                                                       stru_estado.Id_Usuario,
                                                                                                       id_Tarea,
                                                                                                       stru_estado.id_Estado,
                                                                                                       0,
                                                                                                       Fecha_Fromat,
                                                                                                       estado_registro_sii)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                    End If
                End If

                If id_flujo_trabajo <> 0 Then
                    ''---Registra balanceo usuario destino incrementa actividades asignadas  
                    'Result = Class_wf_registro_asignacion_flujo.Registro_balanceo_asignacion_flujo(id_actividad_flujo_trabajo,
                    '                                                                               ref_usuario_destino,
                    '                                                                               id_Tarea,
                    '                                                                               0,
                    '                                                                               1,
                    '                                                                               Fecha_Fromat,
                    '                                                                               estado_registro_balanceo)
                    'If Result <> "YES" Then
                    '    Terminar_Tarea_Workflow = Result
                    '    Exit Function
                    'End If
                    ''---Registra balanceo al usuario anterior decrementa actividades asignadas -1
                    'If stru_estado.Id_Usuario <> 0 Then
                    '    Result = Class_wf_registro_asignacion_flujo.Registro_balanceo_asignacion_flujo(stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO,
                    '                                                                                   stru_estado.Id_Usuario,
                    '                                                                                   id_Tarea,
                    '                                                                                   stru_estado.id_Estado,
                    '                                                                                   0,
                    '                                                                                   Fecha_Fromat,
                    '                                                                                   estado_registro_balanceo)
                    '    If Result <> "YES" Then
                    '        Terminar_Tarea_Workflow = Result
                    '        Exit Function
                    '    End If
                    'End If
                    '---Registra balanceo usuario destino incrementa actividades asignadas en 1-+1
                    Result = Class_wf_registro_asignacion_ruta.Registro_balanceo_reasignacion_ruta(Val(Id_Actividad),
                                                                                                   ref_usuario_destino,
                                                                                                   id_Tarea, 0,
                                                                                                   1,
                                                                                                   Fecha_Fromat,
                                                                                                   estado_registro_sii)
                    If Result <> "YES" Then
                        Terminar_Tarea_Workflow = Result
                        Exit Function
                    End If
                    estado_registro_balanceo = estado_registro_sii
                    '---Registra balanceo al usuario anterior decrementa actividades asignadas -1
                    If stru_estado.Id_Usuario <> 0 Then
                        Result = Class_wf_registro_asignacion_ruta.Registro_balanceo_reasignacion_ruta(stru_estado.Id_Actividad,
                                                                                                       stru_estado.Id_Usuario,
                                                                                                       id_Tarea,
                                                                                                       stru_estado.id_Estado,
                                                                                                       0,
                                                                                                       Fecha_Fromat,
                                                                                                       estado_registro_sii)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow = Result
                            Exit Function
                        End If
                    End If
                End If
                'Terminar_Tarea_Workflow = "OJO BALANCE SII"
                'Exit Function
            End If
            'Terminar_Tarea_Workflow = "No asig"
            'Exit Function
            '--------------------------------------------------------------------------
            'Caso tarea sin usuario asignado, por reasignación se llena con el id 
            'del usuario que esta reasignando y es quien termina la tarea
            '--------------------------------------------------------------------------
            Dim opdate_usuario As String = ""
            If stru_estado.Id_Usuario = 0 Then
                opdate_usuario = ",Id_Usuario=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow")
            End If
            Dim update_fecha_seleccion As String = ""
            If stru_estado.Fecha_Seleccion = "" Then
                update_fecha_seleccion = ",fecha_seleccion='" & Fecha_Fromat & "',Duracion_Inicio_Seleccion=" & Minuto_Dur
            End If
            Dim Parametro_Update As String = "UPDATE ESTADOS_TAREA_WORKFLOW " &
                "SET FECHA_FIN='" & Fecha_Fromat & "'" &
                ",Duracion_Seleccion_Fin=" & Minuto_Dur &
                 ",TOTAL_DURACION_ACTIVIDAD=" & Minuto_Dur_final & opdate_usuario & update_fecha_seleccion &
                " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_Tarea &
                " AND ID_ACTIVIDAD=" & Id_Activida_User & " and FECHA_FIN IS NULL"
            Dim Parametro_Insert As String = ""
            If ref_usuario_destino <> 0 Then
                Parametro_Insert = "INSERT INTO ESTADOS_TAREA_WORKFLOW " &
                "(INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA," &
                "INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_ACTIVIDAD,ID_USUARIO," &
                "FECHA_INICIO,ESTADO_PRIORIDAD,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO) VALUES (" &
                id_ruta_tarea & "," &
                id_Tarea & "," &
                Id_Actividad & "," &
                ref_usuario_destino & ",'" &
                Fecha_Fromat & "'," &
                estado_prioridad_tarea & "," & id_flujo_trabajo & "," & id_actividad_flujo_trabajo & "," & id_usuario_workflow_flujo_trabajo & ")"
            Else
                Parametro_Insert = "INSERT INTO ESTADOS_TAREA_WORKFLOW " &
                            "(INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA," &
                            "INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_ACTIVIDAD," &
                            "FECHA_INICIO,ESTADO_PRIORIDAD,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO) VALUES (" &
                            id_ruta_tarea & "," &
                            id_Tarea & "," &
                            Id_Actividad & ",'" &
                            Fecha_Fromat & "'," &
                            estado_prioridad_tarea & "," & id_flujo_trabajo & "," & id_actividad_flujo_trabajo & "," & id_usuario_workflow_flujo_trabajo & ")"
            End If
            '-----------------------------------
            'ZONA SII INTEGRACION
            '-----------------------------------
            'Solicita codigo corto sii para ccv
            '----------------------------------
            Dim codigo_sii As String = ""
            Dim Class_relacion_sirep_workflow As New Class_relacion_sirep_workflow
            If ref_usuario_destino <> 0 Then
                Result = Class_relacion_sirep_workflow.Solicita_codigo_sii_usuario_workflow(ref_usuario_destino,
                                                                                            codigo_sii)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
            End If
            '----------------------------------------
            'Actualiza codigo corto SII
            '----------------------------------------
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            If codigo_sii <> "" Then
                Result = Class_DAT_ADIC_TAR.Actualiza_codigo_corto_sii_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                      id_Tarea,
                                                                                      codigo_sii)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow = Result
                    Exit Function
                End If
            End If
            '---------------------------------------------------------------
            'ZONA ACTUALIZA ESTADO DE LA TAREA EN WORKFLOW
            '---------------------------------------------------------------
            Dim last_insert As Integer = 0
            Result = Cambia_Estado(Parametro_Update,
                                   Parametro_Insert,
                                   last_insert)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow = Result
                Exit Function
            End If
            '------------------------------------------
            'ZONA REGISTRO LOG BALANCEO
            '------------------------------------------
            Dim Class_wf_log_asignacion_balanceo As New Class_wf_log_asignacion_balanceo
            Dim stru_registro_balanceo As stru_registro_balanceo = Nothing
            stru_registro_balanceo.estados_tarea_workflow_id_Estado = last_insert
            stru_registro_balanceo.id_tarea_workflow = id_Tarea
            stru_registro_balanceo.id_actividad = Id_Actividad
            stru_registro_balanceo.usuario_workflow_idU_suario = ref_usuario_destino
            stru_registro_balanceo.id_usuario_workflow_flujo_trabajo = id_usuario_workflow_flujo_trabajo
            stru_registro_balanceo.id_actividad_flujo_trabajo = id_actividad_flujo_trabajo
            stru_registro_balanceo.fecha_registro = Fecha_Fromat
            stru_registro_balanceo.id_flujo_trabajo = id_flujo_trabajo
            stru_registro_balanceo.id_usuario_reasigna = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
            Dim estado_registro As String = "YES"
            If estado_registro_balanceo = 1 And ref_usuario_destino <> 0 Then
                Result = Class_wf_log_asignacion_balanceo.Registro_log_balanceo(stru_registro_balanceo)
                If Result <> "YES" Then
                    estado_registro = "Se envio la tarea, pero no se pudo registrar el log de balanceo " & Result
                End If
            End If
            '------------------------------------------
            'ZONA REGISTRA LOG REASINACION  
            '------------------------------------------
            Dim Class_wf_log_estados_workflow As New Class_wf_log_estados_workflow
            Dim Stru_wf_log_estados_workflow As Stru_wf_log_estados_workflow = Nothing
            If activa_reasigna_tarea_workflow = 1 Then
                Stru_wf_log_estados_workflow.usuario_workflow_idU_suario = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
                Stru_wf_log_estados_workflow.estados_tarea_workflow_id_Estado = stru_estado.id_Estado
                Stru_wf_log_estados_workflow.id_tarea_workflow = id_Tarea
                Stru_wf_log_estados_workflow.fecha_registro = Fecha_Fromat
                Stru_wf_log_estados_workflow.tipo_transacion = "RASIGNA WORKFLOW"
                Stru_wf_log_estados_workflow.Direccion_ip_Nombre = HttpContext.Current.Session.Item("ip_host_name")
                Stru_wf_log_estados_workflow.id_usuario_anterior = stru_estado.Id_Usuario
                Stru_wf_log_estados_workflow.id_actividad_anterior = stru_estado.Id_Actividad
                Stru_wf_log_estados_workflow.id_actividad_siguiente = Id_Actividad
                Stru_wf_log_estados_workflow.id_usuario_siguiente = ref_usuario_destino
                Stru_wf_log_estados_workflow.estados_tarea_siguiente_workflow_id_Estado = last_insert
                Result = Class_wf_log_estados_workflow.Registra_log_estado_tarea_worlkflow(Stru_wf_log_estados_workflow)
                If Result <> "YES" Then
                    estado_registro = "Se envio la tarea, pero no se pudo registrar el log de reasignacion " & Result
                End If
            End If
            If activa_reasigna_sii = 1 Then
                Stru_wf_log_estados_workflow.usuario_workflow_idU_suario = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
                Stru_wf_log_estados_workflow.estados_tarea_workflow_id_Estado = stru_estado.id_Estado
                Stru_wf_log_estados_workflow.id_tarea_workflow = id_Tarea
                Stru_wf_log_estados_workflow.fecha_registro = Fecha_Fromat
                Stru_wf_log_estados_workflow.tipo_transacion = "RASIGNA INTEGRACION SII"
                Stru_wf_log_estados_workflow.Direccion_ip_Nombre = HttpContext.Current.Session.Item("ip_host_name")
                Stru_wf_log_estados_workflow.id_usuario_anterior = stru_estado.Id_Usuario
                Stru_wf_log_estados_workflow.id_actividad_anterior = stru_estado.Id_Actividad
                Stru_wf_log_estados_workflow.id_actividad_siguiente = Id_Actividad
                Stru_wf_log_estados_workflow.id_usuario_siguiente = ref_usuario_destino
                Stru_wf_log_estados_workflow.estados_tarea_siguiente_workflow_id_Estado = last_insert
                Result = Class_wf_log_estados_workflow.Registra_log_estado_tarea_worlkflow(Stru_wf_log_estados_workflow)
                If Result <> "YES" Then
                    estado_registro = "Se envio la tarea, pero no se pudo registrar el log de estado de tarea " & Result
                End If
            End If
            '----------------------------------------------------------------------
            'ZONA REGISTRA LOG REASIGNACION MANUAL DESDE USUARIO EXTERNO AL FLUJO
            '----------------------------------------------------------------------
            Dim Stru_wf_log_rasignacion_tarea As Stru_wf_log_rasignacion_tarea = Nothing
            Dim Class_wf_log_rasignacion_tarea As New Class_wf_log_rasignacion_tarea
            Stru_wf_log_rasignacion_tarea.estados_tarea_workflow_id_Estado = last_insert
            Stru_wf_log_rasignacion_tarea.id_usuario_workflow_flujo_trabajo = id_usuario_workflow_flujo_trabajo
            Stru_wf_log_rasignacion_tarea.usuario_workflow_idU_suario = ref_usuario_destino
            Stru_wf_log_rasignacion_tarea.usuario_workflow_rasigna = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
            Stru_wf_log_rasignacion_tarea.id_tarea_workflow = id_Tarea
            Stru_wf_log_rasignacion_tarea.id_actividad = Val(Id_Actividad)
            Stru_wf_log_rasignacion_tarea.fecha_registro = Fecha_Fromat
            Stru_wf_log_rasignacion_tarea.Direccion_ip_Nombre = HttpContext.Current.Session.Item("ip_host_name")
            Stru_wf_log_rasignacion_tarea.id_actividad_flujo_trabajo = id_actividad_flujo_trabajo
            Stru_wf_log_rasignacion_tarea.id_flujo_trabajo = id_flujo_trabajo
            If (activa_reasigna_sii = 1 Or activa_reasigna_tarea_workflow = 1) And ref_usuario_destino <> 0 Then
                Result = Class_wf_log_rasignacion_tarea.Registra_log_reasignacion_tarea_workflow(Stru_wf_log_rasignacion_tarea)
                If Result <> "YES" Then
                    estado_registro = "Se envio la tarea, pero no se pudo registrar el log de reasignacion " & Result
                End If
            End If
            '----------------------------------------------------------------
            'ZONA ACTUALIZA CAMPOS Y PARAMENTROS INTERFACE
            '----------------------------------------------------------------
            If activa_actualizacion_paramtros_interface = 1 Then
                If HttpContext.Current.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W") > 0 Then
                    HttpContext.Current.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W") = HttpContext.Current.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W") - 1
                End If
                'Dim id_tarea_seleccion As Integer = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0"
                HttpContext.Current.Session.Item("DG_ID_TRAMITE") = 0
                HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = ""
                HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
                Dim Label_estado_selecion As Label = pag.FindControl("Label_estado_selecion")
                Dim updatemenu As UpdatePanel = pag.FindControl("updatemenu")
                Label_estado_selecion.Text = ""
                updatemenu.Update()
                '-------------------------------------------------
                'Actualiza el numero de actividades en espera
                '------------------------------------------------

                Dim NumActi As Integer = HttpContext.Current.Session.Item("NUMEROACTIVIDADES")
                If NumActi > 0 Then
                    NumActi -= 1
                End If
                Dim LabelEspera As Label = pag.FindControl("LabelEspera")
                Dim Hidden_00005_2222 As Object = pag.FindControl("Hidden_00005_2222")
                Dim UpdatePanelnumeroespera As UpdatePanel = pag.FindControl("UpdatePanelnumeroespera")
                HttpContext.Current.Session("NUMEROACTIVIDADES") = NumActi
                If Not LabelEspera Is Nothing Then
                    LabelEspera.Text = "(" & HttpContext.Current.Session("NUMEROACTIVIDADES") & ")"
                    Hidden_00005_2222.value = id_Tarea
                    UpdatePanelnumeroespera.Update()
                End If
                If Not HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") Is Nothing Then
                    For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Count - 1
                        If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).Item(0) = id_Tarea Then
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).delete()
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").AcceptChanges()
                            Exit For
                        End If
                    Next
                End If
                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                    For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Count - 1
                        If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).Item(0) = id_Tarea Then
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).delete()
                            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").AcceptChanges()
                            Exit For
                        End If
                    Next
                End If
            End If
            '---------------------------------------------------------------
            'ZONA NOTIFICA CORREO ELECTRONICO
            '--------------------------------------------------------------
            Dim Refclas_gestion_respuesta As New Classgestionrespuesta
            Dim correo_electronico As String = ""
            If notifica = 1 Then
                '----------------------------------------------
                'Solicita relacion usuario gestión workflow
                '----------------------------------------------
                If ref_usuario_destino <> 0 Then
                    Result = Class_usuario_workflow.Retorna_relacion_id_usuario_gestion_con_usuario_wrokflow(ref_usuario_destino,
                                                                                                             id_usuario_gestion)
                    If Result <> "YES" Then
                        resultado_correo = "La tarea se envió pero no se notificó al correo electrónico  : " & Result
                        'Terminar_Tarea_Workflow = "YES"
                    End If
                End If
                Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
                Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(id_usuario_gestion,
                                                                                   correo_electronico)
                If Result <> "YES" Then
                    resultado_correo = "La tarea se envió pero no se notificó al correo electrónico " & Result
                    'Terminar_Tarea_Workflow = "YES"
                    'Exit Function
                End If
                Dim tramite As String = ""
                Dim Fecha_vence As String = ""
                Dim fecha_registro As String = ""
                Dim destinatario As String = ""
                Dim asunto As String = ""
                Result = Refclas_gestion_respuesta.Retorna_detalle_respuesta_radicado(Radicado,
                                                                                      tramite,
                                                                                      Fecha_vence,
                                                                                      fecha_registro,
                                                                                      destinatario,
                                                                                      asunto)
                If Result <> "YES" Then
                    resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                    'Terminar_Tarea_Workflow = "YES"
                    'Exit Function
                End If
                Dim split_notificacion() As String = {"Trámite asignado : " & Radicado & " Tipo tramite " & tramite, "Fecha de radicación : " & fecha_registro _
                        , "Fecha límite de respuesta : " & Fecha_vence, "Remite : " & destinatario, "Asunto : " & asunto, "Radicado : " & Radicado,
                        "Para tramitar este radicado por favor ingrese al módulo flujo de trabajo del gestor web DocuArchi.net"}
                asunto = "Asignación de tramite " & Radicado & " tipo tramite " & tramite & " Fecha vencimiento  " & Fecha_vence
                Dim refclascorreo As New ClassCorreo
                Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion,
                                                                            correo_electronico,
                                                                            asunto)
                If Result <> "YES" Then
                    resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                    'Terminar_Tarea_Workflow = "YES"
                    'Exit Function
                End If
            Else
                Dim Refclas_grupos_workflow As New Class_grupos_workflow
                Dim Ref_class_usuario As New Class_usuario_workflow
                Dim ref_class_flujo_trabajo As New Class_flujo_trabajo_workflow
                If notifica_envio_correo = 1 Then
                    Dim title_comentario_send As String = ""
                    '----------------------------------
                    'Solicita campo tramite
                    '----------------------------------
                    Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
                    Dim nombre_campo_tramite As String = ""
                    Dim tramite_workflow As String = ""
                    Result = Class_configuracion_listado_ruta.SolicitaNombreCampoTramiteRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                                nombre_campo_tramite)
                    If Result <> "YES" Then
                        resultado_correo = "La tarea se envio, pero fue imposible  ubicar el campo tramite  error (" & Result & ")"
                        'Terminar_Tarea_Workflow = "YES"
                        'Exit Function
                    End If
                    Result = ref_Class_DAT_ADIC_TAR.SolicitaTramiteFlujoWorkflow(id_Tarea,
                                                                                 HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                 nombre_campo_tramite,
                                                                                 HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                 tramite_workflow,
                                                                                 0)
                    If Result <> "YES" Then
                        resultado_correo = "La tarea se envio, pero fue imposible  ubicar el nombre del tramite  error (" & Result & ")"
                        'Terminar_Tarea_Workflow = "YES"
                        'Exit Function
                    End If
                    Dim nombre_campo_beneficiario As String = ""
                    Dim nombre_beneficiario As String = ""
                    Result = Class_configuracion_listado_ruta.SolicitaNombreCampoBenificiarioRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                                     nombre_campo_beneficiario)
                    If Result <> "YES" Then
                        resultado_correo = "La tarea se envio, pero fue imposible  ubicar el campo beneficiario error (" & Result & ")"
                        'Terminar_Tarea_Workflow = "YES"
                        'Exit Function
                    End If
                    Result = ref_Class_DAT_ADIC_TAR.SolicitaBeneficiarioTareaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                nombre_campo_beneficiario,
                                                                                id_Tarea,
                                                                                nombre_beneficiario)
                    If Result <> "YES" Then
                        resultado_correo = Result
                        'Terminar_Tarea_Workflow = "YES"
                        'Exit Function
                    End If
                    Dim id_grupo As Integer = 0
                    Result = Refclas_grupos_workflow.Solicita_id_grupo_actividad_workflow(Val(Id_Actividad),
                                                                                          id_grupo)
                    If Result <> "YES" Then
                        resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                        'Terminar_Tarea_Workflow = "YES"
                        'Exit Function
                    End If
                    Dim nombre_grupo As String = ""
                    Result = Refclas_grupos_workflow.Solicita_nombre_grupo_workflow(id_grupo,
                                                                                    nombre_grupo)
                    If Result <> "YES" Then
                        resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                        'Terminar_Tarea_Workflow = "YES"
                        'Exit Function
                    End If
                    If id_flujo_trabajo <> 0 Then
                        Dim comentario As String = ""
                        Dim title_comentario As String = ""
                        Dim nota_cometario As String = ""
                        Dim correos_electronicos As String = ""
                        Dim nombre_flujo As String = ""
                        Result = ref_class_flujo_trabajo.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo,
                                                                                                    nombre_flujo)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            'Terminar_Tarea_Workflow = "YES"
                            'Exit Function
                        End If
                        If id_usuario_workflow_flujo_trabajo <> 0 Or id_usuario_workflow_flujo_trabajo_send_correo <> 0 Then
                            Dim id_user_send As Integer = 0
                            If id_usuario_workflow_flujo_trabajo <> 0 Then
                                id_user_send = id_usuario_workflow_flujo_trabajo
                            End If
                            If id_usuario_workflow_flujo_trabajo_send_correo <> 0 Then
                                id_user_send = id_usuario_workflow_flujo_trabajo_send_correo
                            End If
                            Result = Refclass_usuario_workflow.Solicita_correo_usuario_workflow(id_user_send,
                                                                                                correos_electronicos)
                            If Result <> "YES" Then
                                resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                                'Terminar_Tarea_Workflow = "YES"
                                'Exit Function
                            End If
                            comentario = "Para revisar esta tarea por favor ingrese a la opción flujos y tareas del gestor web DocuArchi.net"
                            title_comentario = "Nueva tarea de usuario asignada bajo el radicado : " & Radicado & " Tramite : " & tramite_workflow & " , flujo de trabajo (" & nombre_flujo & ")"
                            title_comentario_send = "Asignación de tarea : " & Radicado & " Tramite :  " & tramite_workflow &
                               " Remitente : " & nombre_beneficiario
                        Else
                            Result = Refclas_grupos_workflow.Solicita_correo_usuarios_grupo_workflow(id_grupo,
                                                                                                     correos_electronicos)
                            If Result <> "YES" Then
                                resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                                'Terminar_Tarea_Workflow = "YES"
                                'Exit Function
                            End If
                            comentario = "Para revisar esta tarea por favor ingrese a la opción flujos y tareas del gestor web DocuArchi.net"
                            title_comentario = "Nueva tarea de tipo grupal. Se asigna al grupo (" & nombre_grupo & "), bajo el radicado : " & Radicado & " Tramite : " & tramite_workflow & " , flujo de trabajo (" & nombre_flujo & ")"
                            title_comentario_send = "Asignación de tarea : " & Radicado & " Tramite :  " & tramite_workflow &
                              " Remitente : " & nombre_beneficiario
                            nota_cometario = "Nota : Si esta tarea no aparece en su registro de tareas por trabajar es posible que algún integrante de su grupo la esté trabajando"
                        End If
                        Dim split_notificacion() As String = {title_comentario,
                                                              comentario,
                                                              nota_cometario}
                        Dim refclascorreo As New ClassCorreo
                        Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion,
                                                                                    correos_electronicos,
                                                                                     title_comentario_send)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            'Terminar_Tarea_Workflow = "YES"
                            'Exit Function
                        End If
                    Else
                        Dim comentario As String = ""
                        Dim title_comentario As String = ""
                        Dim nota_cometario As String = ""
                        Dim correos_electronicos As String = ""
                        If Val(Id_Usuario_Destino) <> 0 Then
                            Result = Refclass_usuario_workflow.Solicita_correo_usuario_workflow(Val(Id_Usuario_Destino),
                                                                                        correos_electronicos)
                            If Result <> "YES" Then
                                resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                                'Terminar_Tarea_Workflow = "YES"
                                'Exit Function
                            End If
                            comentario = "Para revisar esta tarea por favor ingrese al módulo flujo de trabajo del gestor web DocuArchi.net"
                            title_comentario = "Nueva tarea de usuario asignada bajo el radicado : (" & Radicado & ") Tramite :  (" & tramite_workflow &
                                ") Remitente : (" & nombre_beneficiario & ")"
                            title_comentario_send = "Asignación de tarea : " & Radicado & " Tramite :  " & tramite_workflow &
                              " Remitente : " & nombre_beneficiario
                        Else

                            Result = Refclas_grupos_workflow.Solicita_correo_usuarios_grupo_workflow(id_grupo,
                                                                                                     correos_electronicos)
                            If Result <> "YES" Then
                                resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                                Terminar_Tarea_Workflow = "YES"
                                Exit Function
                            End If

                            comentario = "Para revisar esta tarea por favor ingrese al módulo flujo de trabajo del gestor web DocuArchi.net"
                            title_comentario = "Nueva tarea de tipo grupal. Se asigna al grupo (" & nombre_grupo & "), bajo el radicado : (" & Radicado & ") Tramite :  (" & tramite_workflow &
                                ")  Remitente : (" & nombre_beneficiario & ")"
                            title_comentario_send = "Asignación de tarea : " & Radicado & " Tramite :  " & tramite_workflow &
                              " Remitente : " & nombre_beneficiario
                            nota_cometario = "Nota : Si esta tarea no aparece en su registro de tareas por trabajar es posible que algún integrante de su grupo la esté trabajando"
                        End If
                        Dim split_notificacion() As String = {title_comentario,
                                                              comentario,
                                                              nota_cometario}
                        Dim refclascorreo As New ClassCorreo
                        Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion,
                                                                                    correos_electronicos,
                                                                                    title_comentario_send)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            'Terminar_Tarea_Workflow = "YES"
                            'Exit Function
                        End If
                    End If
                End If
            End If
            '------------------------------------
            'ZONA EVALUA EVENTO TERMINAR TAREA
            '------------------------------------
            If activa_evento_dinamicos = 1 Then
                Dim mParam_terminar() As Object = {Conection_conectro_C,
                                      HttpContext.Current.Session("Id_Usuario_Workflow").ToString,
                                      id_Tarea,
                                      HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                      ref_usuario_destino,
                                      Id_Actividad,
                                      id_flujo_trabajo,
                                      id_actividad_flujo_trabajo,
                                      id_usuario_workflow_flujo_trabajo}
                If HttpContext.Current.Session("TERMINARACTIVIDAD") <> "" Then
                    Resultado1 = refcla.Compila_Evalua(ResultadoComp,
                                                   HttpContext.Current.Session("TERMINARACTIVIDAD"),
                                                   "TERMINARACTIVIDAD", mParam_terminar)
                    If Resultado1 <> "YES" Then
                        Resultado_evalua_terminar = "La tarea se envió, pero fue imposible evaluar el servicio  TERMINARACTIVIDAD  error (" & Resultado1 & ")"
                        'Terminar_Tarea_Workflow = "YES"
                        'Exit Function
                    End If
                    If ResultadoComp <> "YES" Then
                        Resultado_evalua_terminar = "La tarea se envió, pero fue imposible evaluar el servicio  TERMINARACTIVIDAD  error (" & ResultadoComp & ")"
                        'Terminar_Tarea_Workflow = "YES"
                        'Exit Function
                    End If
                End If
            End If
            If estado_registro <> "YES" Then
                Resultado_evalua_terminar = estado_registro
                'Exit Function
            End If
            Terminar_Tarea_Workflow = "YES"
        Catch ex As Exception
            Terminar_Tarea_Workflow = "Inconsistencia general función Terminar_Tarea_Workflow " & ex.Message
        End Try
    End Function

    Function Terminar_Tarea_Workflow_Bacth(ByVal Id_Usuario_Destino As String,
                                           ByVal Id_Actividad_Destino As String,
                                           ByVal Id_Pendiente As Integer,
                                           ByVal id_tarea As Long,
                                           ByVal Nombre_Activ As String,
                                           Optional ByVal id_flujo_trabajo As Integer = 0,
                                           Optional ByVal id_actividad_flujo_trabajo As Integer = 0,
                                           Optional ByVal id_usuario_workflow_flujo_trabajo As Integer = 0,
                                           Optional ByRef notifica_envio_correo As Integer = 0,
                                           Optional ByRef resultado_correo As String = "",
                                           Optional ByVal estado_actividad As Integer = 1,
                                           Optional ByVal id_conector As Integer = 0,
                                           Optional ByVal id_usuario_wf_envia As Integer = 0,
                                           Optional ByVal id_actividad_wf_envia As Integer = 0,
                                           Optional ByVal actualiza_estado_registro As Integer = 0,
                                           Optional ByVal id_registro_radicado As Long = 0,
                                           Optional ByVal verifica_tarea_documento As Integer = 0,
                                           Optional ByVal estado_activida_modulo_rad As Integer = 0) As String
        Try
            Dim Ref As New ClassWorkflow
            Dim Ref2 As New ClassListandoTareas
            Dim Result As String = ""
            Dim Id_Actividad As String = "0"
            Dim Id_Activida_User As String = ""
            Dim Estado_Prioridad As String = ""
            Dim Id_User As String = HttpContext.Current.Session("Id_Usuario_Workflow")
            Dim ref_usuario_destino As Integer = 0
            Dim Radicado As String = ""
            '-----------------------------------------
            'Consulta el id de la actividad destino
            '-----------------------------------------
            Dim Ref_class_list_actividades As New Class_Listado_Actividades_workflow
            If Id_Actividad_Destino = "" Then
                Result = Ref_class_list_actividades.Obtener_Id_Actividad(Id_Actividad,
                                                                         Nombre_Activ)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = "Error Consultando id actividad destino " + Result
                    Exit Function
                End If
                If Id_Actividad = "0" Then
                    Terminar_Tarea_Workflow_Bacth = "Debe Seleccionar Actividad destino "
                    Exit Function
                End If

            Else
                Id_Actividad = Id_Actividad_Destino
            End If
            '-------------------------------------------
            'Solicita tipo actividad workflow
            '-------------------------------------------
            '-------------------------------------------------------
            'Solicita el tipo de actividad de la actividad destino
            '-------------------------------------------------------
            Dim id_tipo_actividad As Integer = 0
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim Class_actividades_generales_workflow As New Class_actividades_generales_workflow
            Dim actividades_generales_workflow_ As actividades_generales_workflow = Nothing
            If Id_Actividad <> 0 Then
                Result = Class_Listado_Actividades_workflow.Solicita_id_tipo_actividad_workflow(Val(Id_Actividad),
                                                                                                id_tipo_actividad)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                Result = Class_actividades_generales_workflow.Solicita_estructura_tipo_actividad_workflow(id_tipo_actividad,
                                                                                                          actividades_generales_workflow_)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
            End If
            Dim Ref_class_adic As New Class_DAT_ADIC_TAR
            Dim id_imagen As Long = 0
            If verifica_tarea_documento = 1 Then
                Result = Ref_class_adic.SolicitaidImagenTareaworkflow(id_tarea,
                                                                 HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                 id_imagen)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                If id_imagen = 0 Then
                    Terminar_Tarea_Workflow_Bacth = "Imposible terminar la tarea sin documento relacionado"
                    Exit Function
                End If
            End If
            'consulta el tipo de tramite
            Dim tipo_tramite As String = ""
            Dim nombre_campo_tramite_ruta As String = ""
            Dim estado As Integer = 0
            Dim ref_config_ruta As New Class_configuracion_listado_ruta
            Result = ref_config_ruta.SolicitaNombreCampoTramiteRuta(Val(HttpContext.Current.Session("Id_Ruta_Workflow")),
                                                                       nombre_campo_tramite_ruta)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow_Bacth = Result
                Exit Function
            End If
            Ref_class_adic.SolicitaTramiteFlujoWorkflow(id_tarea,
                                                        Val(HttpContext.Current.Session("Id_Ruta_Workflow")),
                                                        nombre_campo_tramite_ruta,
                                                        HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                        tipo_tramite,
                                                        estado)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow_Bacth = Result
                Exit Function
            End If
            Dim nombre_campo_beneficiario As String = ""
            Dim nombre_beneficiario As String = ""
            Result = ref_config_ruta.SolicitaNombreCampoBenificiarioRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                            nombre_campo_beneficiario)
            If Result <> "YES" Then
                resultado_correo = Result
                Terminar_Tarea_Workflow_Bacth = "YES"
                Exit Function
            End If
            Result = Ref_class_adic.SolicitaBeneficiarioTareaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                      nombre_campo_beneficiario,
                                                                      Val(id_tarea),
                                                                      nombre_beneficiario)
            If Result <> "YES" Then
                resultado_correo = Result
                Terminar_Tarea_Workflow_Bacth = "YES"
                Exit Function
            End If
            Dim nombre_campo_radicado As String = ""
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                             nombre_campo_radicado)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow_Bacth = Result
                Exit Function
            End If
            '************************************
            'Retorna radicado tarea seleccionada
            '************************************
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Val(id_tarea),
                                                                                    nombre_campo_radicado,
                                                                                    HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                    Radicado)
            If Result <> "YES" Then
                resultado_correo = Result
                Terminar_Tarea_Workflow_Bacth = "YES"
                Exit Function
            End If
            '------------------------------------------------
            'Caso usuario responsable de flujo
            '------------------------------------------------
            Dim Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
            Dim Class_plantillas_radicacion As New Class_plantillas_radicacion
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Dim Class_grupos_workflow As New Class_grupos_workflow
            Dim nombre_plantilla_radicado As String = ""
            Dim id_grupo_usuario_workflow As Integer = 0
            Dim id_usuario_worflow_destino As Integer = 0
            Dim id_actividad_destino_asig As Integer = 0
            Dim id_usuario_workflow_flujo_trabajo_send_correo As Integer = 0
            If actividades_generales_workflow_.Nombre_tipo_actividad = "USUARIORESPONSABLE" Then
                Result = Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                              nombre_plantilla_radicado)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                Dim id_usuario_gestion_radicado As Integer = 0
                Result = Class_plantillas_radicacion.Solicita_remitente_destinatario_fecha_radicado(nombre_plantilla_radicado,
                                                                                                    Radicado,
                                                                                                    id_usuario_gestion_radicado,
                                                                                                    0,
                                                                                                    "")
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                '----Solicita usuario workflow relacionado
                Result = Class_remit_dest_interno.Solicita_id_usuario_workflow_relacionado(id_usuario_gestion_radicado,
                                                                                           id_usuario_worflow_destino)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                '----Solicita id grupo usuario workflow
                Result = Class_usuario_workflow.Solicita_id_grupo_usuario_workflow(id_usuario_worflow_destino,
                                                                                   id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                '----Solicita id actividad relacionado al grupo workflow
                Result = Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(id_actividad_destino_asig,
                                                                                      id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                Id_Actividad = id_actividad_destino_asig
                Id_Usuario_Destino = id_usuario_worflow_destino
                If id_flujo_trabajo <> 0 Then
                    id_usuario_workflow_flujo_trabajo_send_correo = id_usuario_worflow_destino
                End If
            End If

            '--------------------------------------------------------
            'Caso responsable de actividad o flujo usuario radicador
            '--------------------------------------------------------
            Dim Class_ra_usuario_radicador As New Class_ra_usuario_radicador
            If actividades_generales_workflow_.Nombre_tipo_actividad = "USUARIORESPONSABLERADICADOR" Then
                Result = Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                              nombre_plantilla_radicado)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                Dim id_usuario_radicador As Integer = 0
                Result = Class_plantillas_radicacion.Solicita_id_usuario_radicacion_plantilla_radicado(nombre_plantilla_radicado,
                                                                                                       Radicado,
                                                                                                       id_usuario_radicador)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                Dim id_usuario_gestion_rel_radicacion As Integer = 0
                Result = Class_ra_usuario_radicador.Solicita_id_usuario_gestion_relacion_usuario_radicador(id_usuario_radicador,
                                                                                                           id_usuario_gestion_rel_radicacion)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                If id_usuario_gestion_rel_radicacion = 0 Then
                    Terminar_Tarea_Workflow_Bacth = "El codigo de usuario radicador ( " & Val(id_usuario_radicador) & " ) no registra relación de usuario de gestión"
                    Exit Function
                End If
                '----Solicita usuario workflow relacionado
                Result = Class_remit_dest_interno.Solicita_id_usuario_workflow_relacionado(id_usuario_gestion_rel_radicacion,
                                                                                           id_usuario_worflow_destino)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                '----Solicita id grupo usuario workflow
                Result = Class_usuario_workflow.Solicita_id_grupo_usuario_workflow(id_usuario_worflow_destino,
                                                                                   id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                '----Solicita id actividad relacionado al grupo workflow
                Result = Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(id_actividad_destino_asig,
                                                                                      id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                Id_Actividad_Destino = id_actividad_destino_asig
                Id_Usuario_Destino = id_usuario_worflow_destino
                If id_flujo_trabajo <> 0 Then
                    id_usuario_workflow_flujo_trabajo_send_correo = id_usuario_worflow_destino
                End If
            End If
            If Id_Usuario_Destino = "" Then
                ref_usuario_destino = 0
            Else
                ref_usuario_destino = Val(Id_Usuario_Destino)
            End If
            If Id_Usuario_Destino = "0" Then
                Id_Usuario_Destino = ""
            End If
            If Id_Usuario_Destino = "" Then
                ref_usuario_destino = 0
            Else
                ref_usuario_destino = Val(Id_Usuario_Destino)
            End If
            Dim stru_config_conector_ruta As stru_config_conector_ruta = Nothing
            Dim stru_config_conector_flujo As stru_config_conector_flujo = Nothing
            Dim Class_actividades_disponibles_envio As New Class_actividades_disponibles_envio
            Dim ref_conect As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim imagenes_sin_expediente As Integer = 0
            Dim imagenes_sin_firma As Integer = 0
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Dim existencia As String = ""
            If id_conector <> 0 Then
                If id_flujo_trabajo <> 0 Then
                    Result = ref_conect.Solicita_configuracion_conector_flujo(id_conector,
                                                                              stru_config_conector_flujo)
                    If Result <> "YES" Then
                        Terminar_Tarea_Workflow_Bacth = Result
                        Exit Function
                    End If
                    notifica_envio_correo = stru_config_conector_flujo.Estado_evia_correo
                    '--------------------------------------------------------
                    'Valida estado de autorización de la tarea
                    '--------------------------------------------------------
                    If stru_config_conector_flujo.Estado_soicita_autorizacion = 1 Then
                        Result = Class_autoriza_tarea_worklfow.SolicitaExistenciaAutorizacion(id_tarea,
                                                                                                id_actividad_wf_envia,
                                                                                                id_usuario_wf_envia,
                                                                                                existencia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If existencia = "NO" Then
                            Terminar_Tarea_Workflow_Bacth = "Debe autorizar la tarea antes de terminarla"
                            Exit Function
                        End If
                    End If
                    '--------------------------------------------------
                    'Valida autorización con firma digital con flujo
                    '--------------------------------------------------
                    If stru_config_conector_flujo.Estado_soicita_autorizacion_firma_digital = 1 Then
                        Result = Class_autoriza_tarea_worklfow.Solicita_existencia_firma_autorizacion(id_tarea,
                                                                                                      id_actividad_wf_envia,
                                                                                                      id_usuario_wf_envia,
                                                                                                      existencia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If existencia = "NO" Then
                            Terminar_Tarea_Workflow_Bacth = "Debe firmar digitalmente la autorización antes de terminarla"
                            Exit Function
                        End If
                    End If
                    '-------------------------Valida la copia de todos los documentos a expediente ---------
                    If stru_config_conector_flujo.Estado_copia_estructura_total = 1 Then
                        Dim stru_paramter_image_final As stru_paramter_image() = Nothing
                        Dim refclas As New Classselecciotarea
                        Result = refclas.Solicita_lista_id_producion_relacionados_tarea_workflow(id_tarea,
                                                                                                 stru_paramter_image_final)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If stru_paramter_image_final Is Nothing Then
                            Terminar_Tarea_Workflow_Bacth = "Imposible encontrar imagenes para de la tarea  para validar la copia al expediente, imposible terminar la tarea"
                            Exit Function
                        End If
                        Dim Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
                        Dim _ra_rel_copia_wf_produccion() As ra_rel_copia_wf_produccion = Nothing
                        Result = Class_ra_rel_copia_wf_produccion.Solicita_estrucutura_copia_documento_expediente(id_tarea,
                                                                                                                  HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                                                  _ra_rel_copia_wf_produccion)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If _ra_rel_copia_wf_produccion Is Nothing Then
                            Terminar_Tarea_Workflow_Bacth = "Debe copiar los documentos de la tarea a un expediente  "
                            Exit Function
                        End If
                        '-------------Compara los archivos que no se han copiado a expdiente
                        Dim Documentos_no_copy As String = ""
                        Dim shutwch As Integer = -1
                        Dim contador_document As Integer = 0
                        For i As Integer = 0 To stru_paramter_image_final.Length - 1
                            shutwch = -1
                            For k As Integer = 0 To _ra_rel_copia_wf_produccion.Length - 1
                                If stru_paramter_image_final(i).ID = _ra_rel_copia_wf_produccion(k).id_imagen_da Then
                                    shutwch = 1
                                    'Exit For
                                End If
                            Next
                            If shutwch = -1 Then
                                contador_document += 1
                                If stru_paramter_image_final(i).TIPODOCUMENTO = "" Then
                                    Documentos_no_copy = Documentos_no_copy & stru_paramter_image_final(i).ID & " @ "
                                Else
                                    Documentos_no_copy = Documentos_no_copy & stru_paramter_image_final(i).TIPODOCUMENTO & " @ "
                                End If
                            End If
                        Next
                        If Documentos_no_copy <> "" Then
                            Dim tex_normalice As String = Documentos_no_copy.Normalize(NormalizationForm.FormD)
                            Dim reg As Regex = New Regex("[^a-zA-Z0-9 ]")
                            Dim textoSinAcentos As String = reg.Replace(tex_normalice, "")
                            Terminar_Tarea_Workflow_Bacth = "Debe copiar (" & contador_document & ") documento(s) a un expediente (" & textoSinAcentos & ")"
                            Exit Function
                        End If
                    End If
                    '-------------------------------------------------------------
                    'Valida numero de imagenes copiadas  al expediente por flujo
                    '-------------------------------------------------------------
                    '-------------------------Valida la copia de algunos documentos a expediente ----------
                    If stru_config_conector_flujo.Estado_copia_documento_estructura = 1 Then
                        Dim Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
                        Dim numero_documentos_copia As Integer = 0
                        Result = Class_ra_rel_copia_wf_produccion.Solicita_existencia_numero_imagenes_copiadas(id_tarea,
                                                                                                               HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                                               numero_documentos_copia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If numero_documentos_copia = 0 Then
                            Terminar_Tarea_Workflow_Bacth = "Debe copiar al menos un documento de la tarea a un expediente  "
                            Exit Function
                        End If
                    End If
                    '-------------------------------------------------------
                    'Valida estado firma digital y asinación de expediente 
                    '-------------------------------------------------------
                    If stru_config_conector_flujo.Estado_firma_digital = 1 Or stru_config_conector_flujo.Estado_asigna_expediente = 1 Then
                        Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                         id_tarea,
                                                                                                        structure_datos_tarea_workflow)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        Result = Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                          structure_gabinete_workflow)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        Result = ClassDaGabinete.Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                                                                  Radicado,
                                                                                                                                  imagenes_sin_expediente,
                                                                                                                                  imagenes_sin_firma)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If stru_config_conector_flujo.Estado_firma_digital = 1 And imagenes_sin_firma > 0 Then
                            Terminar_Tarea_Workflow_Bacth = "Faltan " & imagenes_sin_firma & " documento (s) por firma digital, imposible terminar la tarea "
                            Exit Function
                        End If
                        If stru_config_conector_flujo.Estado_asigna_expediente = 1 And imagenes_sin_expediente > 0 Then
                            Terminar_Tarea_Workflow_Bacth = "Faltan " & imagenes_sin_expediente & " documento (s) por vincular al  expediente, imposible terminar la tarea "
                            Exit Function
                        End If
                    End If
                Else
                    Result = Class_actividades_disponibles_envio.Solicita_configuracion_conector_ruta(id_conector,
                                                                                                      stru_config_conector_ruta)
                    If Result <> "YES" Then
                        Terminar_Tarea_Workflow_Bacth = Result
                        Exit Function
                    End If
                    notifica_envio_correo = stru_config_conector_ruta.Estado_evia_correo
                    '--------------------------------------------------------
                    'Valida esta de autorización de la tarea
                    '--------------------------------------------------------
                    If stru_config_conector_ruta.Estado_soicita_autorizacion = 1 Then
                        Result = Class_autoriza_tarea_worklfow.SolicitaExistenciaAutorizacion(id_tarea,
                                                                                               id_actividad_wf_envia,
                                                                                               id_usuario_wf_envia,
                                                                                               existencia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If existencia = "NO" Then
                            Terminar_Tarea_Workflow_Bacth = "Debe autorizar la tarea antes de terminarla"
                            Exit Function
                        End If
                    End If
                    '--------------------------------------------------
                    'Valida autorización con firma digital
                    '-------------------------------------------------
                    If stru_config_conector_ruta.Estado_soicita_autorizacion_firma_digital = 1 Then
                        Result = Class_autoriza_tarea_worklfow.Solicita_existencia_firma_autorizacion(id_tarea,
                                                                                                         id_actividad_wf_envia,
                                                                                                         id_usuario_wf_envia,
                                                                                                         existencia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If existencia = "NO" Then
                            Terminar_Tarea_Workflow_Bacth = "Debe firmar digitalmente la autorización antes de terminarla"
                            Exit Function
                        End If
                    End If
                    '----------------------------------------------------
                    'Valida numero de imagenes copiadas  al expediente
                    '----------------------------------------------------
                    If stru_config_conector_ruta.Estado_copia_documento_estructura = 1 Then
                        Dim stru_paramter_image_final As stru_paramter_image() = Nothing
                        Dim refclas As New Classselecciotarea
                        Result = refclas.Solicita_lista_id_producion_relacionados_tarea_workflow(id_tarea,
                                                                                                 stru_paramter_image_final)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        Dim Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
                        Dim numero_documentos_copia As Integer = 0
                        Result = Class_ra_rel_copia_wf_produccion.Solicita_existencia_numero_imagenes_copiadas(id_tarea,
                                                                                                               HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                                               numero_documentos_copia)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If numero_documentos_copia < stru_paramter_image_final.Length Then
                            Terminar_Tarea_Workflow_Bacth = "Debe copiar los documentos de la tarea a un expediente  "
                            Exit Function
                        End If
                    End If
                    '-------------------------------------------------------
                    'Valida estado firma digital y asinación de expediente 
                    '-------------------------------------------------------
                    If stru_config_conector_ruta.Estado_firma_digital = 1 Or stru_config_conector_ruta.Estado_asigna_expediente = 1 Then
                        Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                         id_tarea,
                                                                                                        structure_datos_tarea_workflow)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        Result = Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                          structure_gabinete_workflow)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        Result = ClassDaGabinete.Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                                                                  Radicado,
                                                                                                                                  imagenes_sin_expediente,
                                                                                                                                  imagenes_sin_firma)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        If stru_config_conector_ruta.Estado_firma_digital = 1 And imagenes_sin_firma > 0 Then
                            Terminar_Tarea_Workflow_Bacth = "Faltan " & imagenes_sin_firma & " documento (s) por firma digital, imposible terminar la tarea "
                            Exit Function
                        End If
                        If stru_config_conector_ruta.Estado_asigna_expediente = 1 And imagenes_sin_expediente > 0 Then
                            Terminar_Tarea_Workflow_Bacth = "Faltan " & imagenes_sin_expediente & " documento (s) por vincular al  expediente, imposible terminar la tarea "
                            Exit Function
                        End If
                    End If
                End If
            End If
            Dim Conection_conectro_C = "Persist Security Info=" _
                    & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                    & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                   & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                   & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString
            Dim mParam() As Object = {Conection_conectro_C, HttpContext.Current.Session("Id_Usuario_Workflow").ToString, id_tarea.ToString,
                    HttpContext.Current.Session("Id_Ruta_Workflow").ToString, ref_usuario_destino, Id_Actividad,
                                      id_flujo_trabajo, id_actividad_flujo_trabajo, id_usuario_workflow_flujo_trabajo}
            Dim ResultadoComp As String = ""
            Dim Resultado1 As String = ""
            Dim refcla As New ClassEdtiScript
            If HttpContext.Current.Session("PRETERMINARACTIVIAD") <> "" Then
                Resultado1 = refcla.Compila_Evalua(ResultadoComp, HttpContext.Current.Session("PRETERMINARACTIVIAD"), "PRETERMINARACTIVIAD", mParam)
                If Resultado1 <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = "Error #13 Error Compilando Function PRETERMINARACTIVIAD" & Resultado1
                    Exit Function
                End If
                If ResultadoComp <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = "Imposible eviar la tarea " & ResultadoComp
                    Exit Function
                End If
            End If
            '////////////Esta evaluación sec debe hacer el final enviar la tarea despues del envio
            If HttpContext.Current.Session("TERMINARACTIVIDAD") <> "" Then
                Resultado1 = refcla.Compila_Evalua(ResultadoComp, HttpContext.Current.Session("TERMINARACTIVIDAD"), "TERMINARACTIVIDAD", mParam)
                'Resultado1 = mEval.("ENLASE", mParam)
                If Resultado1 <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = "Error #13 Error Compilando Function TERMINARACTIVIDAD" & Resultado1
                    Exit Function
                End If
                If ResultadoComp <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = "Imposible eviar la tarea " & ResultadoComp
                    Exit Function
                End If
            End If
            Result = ""
            '-----------------------------------------
            '------consulta id actividad usuario
            '-----------------------------------------
            Result = ""
            Dim Refclass_usuario_workflow As New ClassWorkflowUsuario
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Result = ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida_User,
                                                                                      HttpContext.Current.Session.Item("Id_Grupo_Workflow"))
            If Result <> "YES" Or Id_Activida_User = "" Then
                Terminar_Tarea_Workflow_Bacth = "Imposible Obtener Id actividad "
                Exit Function
            End If
            '--------------------------------------------
            'Consulta el etado prioridad tarea workflow
            'ruta tarea
            '-------------------------------------------
            Dim Ref_class_estados As New Class_estados_tarea_workflow
            Result = ""
            Result = Ref_class_estados.Solicita_estado_prioridad(Id_Activida_User,
                                                                 HttpContext.Current.Session("Id_Usuario_Workflow").ToString,
                                                                 id_tarea,
                                                                 Estado_Prioridad,
                                                                 estado_actividad)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow_Bacth = "Error Consultando prioridad de tarea " + Result
                Exit Function
            End If
            If Estado_Prioridad = "" Then
                Terminar_Tarea_Workflow_Bacth = "Imposible Terminar tarea no tiene prioridad asignada o otra seccion la cambio el estado de la tarea"
                Exit Function
            End If
            Dim Matri_Datos() As String
            'Matri_Datos(0) prioridad de tarea
            'Matri_Datos(1) ruta al que pertenece la tarea
            'Matri_Datos(2) Fecha seleccion usuario workflow
            Erase Matri_Datos
            Matri_Datos = Split(Estado_Prioridad, "|")
            If Matri_Datos Is Nothing Then
                Terminar_Tarea_Workflow_Bacth = "Imposible Terminar tarea matriz nula de datos "
                Exit Function
            End If
            Dim fecha_seleccion_bd As String = ""
            Dim fecha_seleccion_fm As String = ""
            Dim fecha_asignacion_inicial As String = ""
            If estado_actividad = 1 Then
                Result = Me.Solicita_fecha_seleccion_tarea(Val(id_tarea),
                                                          Val(Id_Activida_User),
                                                          fecha_seleccion_bd)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                Result = Me.Solicita_fecha_asginacion_tarea(Val(id_tarea),
                                                           Val(Id_Activida_User),
                                                           fecha_asignacion_inicial)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
            Else
                Result = Me.Solicita_fecha_asginacion_tarea(Val(id_tarea),
                                                            Val(Id_Activida_User),
                                                            fecha_seleccion_bd)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
                fecha_asignacion_inicial = fecha_seleccion_bd
            End If
            ''----------------------------------
            ''Formatea fecha servidor time
            ''----------------------------------
            fecha_seleccion_fm = fecha_seleccion_bd
            Dim Refclas_gestion_fecha As New ClassGestionFechas
            '-----------------------------
            Dim Minuto_Dur As Long = 0
            Dim Minuto_Dur_final As Long = 0
            Dim Fecha_Fromat As String = ""
            Dim DateCreate As Date = Now
            '-----------------------------
            'Formatea framework actual
            '-----------------------------
            Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                         Fecha_Fromat)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow_Bacth = Result
                Exit Function
            End If
            '-----------------------------------
            'Solicita los minutos de diferencia
            '-----------------------------------
            Result = Refclas_gestion_fecha.Resta_fechas_db(fecha_seleccion_fm,
                                                           Fecha_Fromat,
                                                           Minuto_Dur)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow_Bacth = Result
                Exit Function
            End If
            If Minuto_Dur <= -1 Then
                Minuto_Dur = CInt(Abs(Minuto_Dur))
            End If
            '------------------------------------------------------------------
            'Solicita los minutos totales de la tarea que termina
            '------------------------------------------------------------------
            Result = Refclas_gestion_fecha.Resta_fechas_db(fecha_asignacion_inicial,
                                                           Fecha_Fromat,
                                                           Minuto_Dur_final)
            If Result <> "YES" Then
                Terminar_Tarea_Workflow_Bacth = Result
                Exit Function
            End If
            If Minuto_Dur_final <= -1 Then
                Minuto_Dur_final = CInt(Abs(Minuto_Dur_final))
            End If
            Dim Parametro_Update As String = ""
            If estado_actividad = 1 Then
                Parametro_Update = "UPDATE ESTADOS_TAREA_WORKFLOW " &
                             "SET FECHA_FIN='" & Fecha_Fromat & "'" &
                             ",Duracion_Seleccion_Fin=" & Minuto_Dur &
                             ",TOTAL_DURACION_ACTIVIDAD=" & Minuto_Dur_final &
                             ",ESTADO_ACTIVIDA_MODULO_RAD=" & 0 &
                             ",Estado_Tarea='" & "0" & "'" &
                             " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea &
                             " AND ID_ACTIVIDAD=" & Id_Activida_User & " and FECHA_FIN IS NULL"
            Else
                Parametro_Update = "UPDATE ESTADOS_TAREA_WORKFLOW " &
                            "SET FECHA_FIN='" & Fecha_Fromat & "'" &
                            ",fecha_Seleccion='" & fecha_asignacion_inicial & "'" &
                            ",Duracion_Seleccion_Fin=" & Minuto_Dur_final &
                            ",TOTAL_DURACION_ACTIVIDAD=" & Minuto_Dur_final &
                            ",Duracion_inicio_seleccion=" & 0 &
                            ",ESTADO_ACTIVIDA_MODULO_RAD=" & 0 &
                            ",Estado_Tarea='" & "0" & "'" &
                            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea &
                            " AND ID_ACTIVIDAD=" & Id_Activida_User & " and FECHA_FIN IS NULL"
            End If
            '---------------------------------------------------
            'Determina balanceo de carga de trabajo del grupo
            'para ruta y flujo de trabajo
            '---------------------------------------------------
            Dim Class_wf_registro_asignacion_ruta As New Class_wf_registro_asignacion_ruta
            Dim Class_wf_registro_asignacion_flujo As New Class_wf_registro_asignacion_flujo
            If id_conector <> 0 Then
                If id_flujo_trabajo <> 0 Then
                    If id_usuario_workflow_flujo_trabajo = 0 And stru_config_conector_flujo.estado_valida_balanceo = 1 And
                                                 id_actividad_flujo_trabajo <> 0 Then
                        Result = Class_wf_registro_asignacion_flujo.balanceo_asignacion_por_conector_flujo(Val(Id_Actividad),
                                                                                                           id_actividad_flujo_trabajo,
                                                                                                           Fecha_Fromat,
                                                                                                           ref_usuario_destino)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If
                        id_usuario_workflow_flujo_trabajo = ref_usuario_destino
                    End If
                Else
                    If ref_usuario_destino = 0 And stru_config_conector_ruta.estado_valida_balanceo = 1 Then
                        Result = Class_wf_registro_asignacion_ruta.Balanceo_asignacion_por_conector_ruta(Val(Id_Actividad),
                                                                                                         Fecha_Fromat,
                                                                                                         ref_usuario_destino)
                        If Result <> "YES" Then
                            Terminar_Tarea_Workflow_Bacth = Result
                            Exit Function
                        End If

                    End If
                End If
            End If

            Dim Parametro_Insert As String = ""
            If ref_usuario_destino <> 0 Then
                Parametro_Insert = "INSERT INTO ESTADOS_TAREA_WORKFLOW " &
                "(INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA," &
                "INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_ACTIVIDAD,ID_USUARIO," &
                "FECHA_INICIO,ESTADO_PRIORIDAD,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO) VALUES (" &
                Matri_Datos(1) & "," &
                id_tarea & "," &
                Id_Actividad & "," &
                ref_usuario_destino & ",'" &
                Fecha_Fromat & "'," &
                Matri_Datos(0) & "," & id_flujo_trabajo & "," & id_actividad_flujo_trabajo & "," & id_usuario_workflow_flujo_trabajo & ")"
            Else
                Parametro_Insert = "INSERT INTO ESTADOS_TAREA_WORKFLOW " &
                            "(INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA," &
                            "INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_ACTIVIDAD," &
                            "FECHA_INICIO,ESTADO_PRIORIDAD,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO) VALUES (" &
                            Matri_Datos(1) & "," &
                            id_tarea & "," &
                            Id_Actividad & ",'" &
                            Fecha_Fromat & "'," &
                            Matri_Datos(0) & "," & id_flujo_trabajo & "," & id_actividad_flujo_trabajo & "," & id_usuario_workflow_flujo_trabajo & ")"
            End If
            Dim Parametro_Estado_Pendiente As String = "UPDATE TAREA_PENDIENTE " &
                "SET ESTADOS_PENDIENTE=0 WHERE ID_PENDIENTE=" & Id_Pendiente
            '-----------------------------------
            'solicita codigo corto sii para ccv
            '----------------------------------
            Dim codigo_sii As String = ""
            Dim Class_relacion_sirep_workflow As New Class_relacion_sirep_workflow
            If ref_usuario_destino <> 0 Then
                Result = Class_relacion_sirep_workflow.Solicita_codigo_sii_usuario_workflow(ref_usuario_destino,
                                                                                            codigo_sii)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
            End If
            '----------------------------------------
            'Actuliza codigo corto sii
            '----------------------------------------
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            If codigo_sii <> "" Then
                Result = Class_DAT_ADIC_TAR.Actualiza_codigo_corto_sii_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"), id_tarea, codigo_sii)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
            End If
            '------------------------------------------------------------------------------------------------
            'Actualiza y cambia de actividad la tarea y de pendiente Cambia el estado del tramite workflow
            '------------------------------------------------------------------------------------------------
            Result = ""
            Dim last_insert As Long = 0
            If estado_actividad = 1 Then
                Result = Cambia_Estado(Parametro_Update,
                                       Parametro_Insert,
                                       Parametro_Estado_Pendiente,
                                       last_insert)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
            Else
                Result = Cambia_Estado_tramite_workflow(Parametro_Update,
                                                        Parametro_Insert,
                                                        last_insert)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
            End If
            '------------------------------------------------------------------
            'Actualiza codigo corto SII
            '-----------------------------------------------------------------

            '------------------------------------------------------------------
            'Actualiza estado registro radicado
            '------------------------------------------------------------------
            'Dim ref_class_estado As New Class_estados_modulo_radicacion
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            If actualiza_estado_registro = 1 Then
                Result = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion(id_registro_radicado,
                                                                                                            2)
                If Result <> "YES" Then
                    Terminar_Tarea_Workflow_Bacth = Result
                    Exit Function
                End If
            End If
            '------------------------------------------------
            'Actualiza numero tareas asignadas modulo gestion
            'de correspondencia
            '------------------------------------------------
            If HttpContext.Current.Session("WF_NUMERO_TRAMITE_ASIGNADO") > 0 Then
                HttpContext.Current.Session("WF_NUMERO_TRAMITE_ASIGNADO") = HttpContext.Current.Session("WF_NUMERO_TRAMITE_ASIGNADO") - 1
            End If
            If Not HttpContext.Current.Session.Item("dat_gred_cahce") Is Nothing Then
                If HttpContext.Current.Session.Item("dat_gred_cahce").GetType.ToString = "System.Data.DataSet" Then
                    For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows.Count - 1
                        If HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item(0) = id_tarea Then
                            HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).delete()
                            HttpContext.Current.Session.Item("dat_gred_cahce").AcceptChanges()
                            Exit For
                        End If
                    Next
                End If
            End If
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item(0) = id_tarea Then
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).delete()
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            Dim Refclas_grupos_workflow As New Class_grupos_workflow
            Dim Ref_class_usuario As New Class_usuario_workflow
            Dim ref_class_flujo_trabajo As New Class_flujo_trabajo_workflow
            If notifica_envio_correo = 1 Then
                Dim title_comentario_send As String = ""
                Dim title_comentario As String = ""
                Dim nota_cometario As String = ""
                Dim comentario As String = ""
                If id_flujo_trabajo <> 0 Then
                    Dim nombre_flujo As String = ""
                    Result = ref_class_flujo_trabajo.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo,
                                                                                               nombre_flujo)
                    If Result <> "YES" Then
                        resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                        Terminar_Tarea_Workflow_Bacth = "YES"
                        Exit Function
                    End If

                    Dim correos_electronicos As String = ""
                    If id_usuario_workflow_flujo_trabajo <> 0 Or id_usuario_workflow_flujo_trabajo_send_correo <> 0 Then
                        Dim id_user_send As Integer = 0
                        If id_usuario_workflow_flujo_trabajo <> 0 Then
                            id_user_send = id_usuario_workflow_flujo_trabajo
                        End If
                        If id_usuario_workflow_flujo_trabajo_send_correo <> 0 Then
                            id_user_send = id_usuario_workflow_flujo_trabajo_send_correo
                        End If
                        Result = Refclass_usuario_workflow.Solicita_correo_usuario_workflow(id_user_send,
                                                                                            correos_electronicos)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            Terminar_Tarea_Workflow_Bacth = "YES"
                            Exit Function
                        End If
                        comentario = "Para revisar esta tarea por favor ingrese al módulo flujo de trabajo del gestor web DocuArchi.net"
                        title_comentario = "Nueva tarea de usuario asignada bajo el radicado : " & Radicado & " Tramite : " & tipo_tramite & " , flujo de trabajo (" & nombre_flujo & ")"
                        title_comentario_send = "Asignación de tarea : " & Radicado & " Tramite :  " & tipo_tramite &
                                " Remitente : " & nombre_beneficiario
                    Else
                        Dim id_grupo As Integer = 0
                        Result = Refclas_grupos_workflow.Solicita_id_grupo_actividad_workflow(Val(Id_Actividad),
                                                                                              id_grupo)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            Terminar_Tarea_Workflow_Bacth = "YES"
                            Exit Function
                        End If
                        Result = Refclas_grupos_workflow.Solicita_correo_usuarios_grupo_workflow(id_grupo,
                                                                                                 correos_electronicos)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            Terminar_Tarea_Workflow_Bacth = "YES"
                            Exit Function
                        End If
                        Dim nombre_grupo As String = ""
                        Result = Refclas_grupos_workflow.Solicita_nombre_grupo_workflow(id_grupo,
                                                                                        nombre_grupo)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            Terminar_Tarea_Workflow_Bacth = "YES"
                            Exit Function
                        End If
                        comentario = "Para revisar esta tarea por favor ingrese al módulo flujo de trabajo del gestor web DocuArchi.net"
                        title_comentario = "Nueva tarea de tipo grupal. Se asigna al grupo (" & nombre_grupo & "), bajo el radicado : " & Radicado & " Tramite : " & tipo_tramite & " , flujo de trabajo (" & nombre_flujo & ")"
                        nota_cometario = "Nota : Si esta tarea no aparece en su registro de tareas por trabajar es posible que algún integrante de su grupo la esté trabajando"
                        title_comentario_send = "Asignación de tarea : " & Radicado & " Tramite :  " & tipo_tramite &
                                " Remitente : " & nombre_beneficiario
                    End If
                    Dim split_notificacion() As String = {title_comentario,
                         comentario, nota_cometario}

                    Dim refclascorreo As New ClassCorreo
                    Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion,
                                                                                correos_electronicos,
                                                                                title_comentario_send)
                    If Result <> "YES" Then
                        resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                        Terminar_Tarea_Workflow_Bacth = "YES"
                        Exit Function
                    End If
                Else
                    Dim correos_electronicos As String = ""
                    If Val(Id_Usuario_Destino) <> 0 Then
                        Result = Refclass_usuario_workflow.Solicita_correo_usuario_workflow(Val(Id_Usuario_Destino),
                                                                                            correos_electronicos)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            Terminar_Tarea_Workflow_Bacth = "YES"
                            Exit Function
                        End If
                        comentario = "Para revisar esta tarea por favor ingrese al módulo flujo de trabajo del gestor web DocuArchi.net"
                        title_comentario = "Nueva tarea de usuario asignada bajo el radicado : " & Radicado & " Tramite :  " & tipo_tramite
                    Else
                        Dim id_grupo As Integer = 0
                        Result = Refclas_grupos_workflow.Solicita_id_grupo_actividad_workflow(Val(Id_Actividad),
                                                                                              id_grupo)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            Terminar_Tarea_Workflow_Bacth = "YES"
                            Exit Function
                        End If
                        Result = Refclas_grupos_workflow.Solicita_correo_usuarios_grupo_workflow(id_grupo,
                                                                                                 correos_electronicos)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            Terminar_Tarea_Workflow_Bacth = "YES"
                            Exit Function
                        End If
                        Dim nombre_grupo As String = ""
                        Result = Refclas_grupos_workflow.Solicita_nombre_grupo_workflow(id_grupo,
                                                                                        nombre_grupo)
                        If Result <> "YES" Then
                            resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                            Terminar_Tarea_Workflow_Bacth = "YES"
                            Exit Function
                        End If
                        comentario = "Para revisar esta tarea por favor ingrese al módulo flujo de trabajo del gestor web DocuArchi.net"
                        title_comentario = "Nueva tarea de tipo grupal. Se asigna al grupo (" & nombre_grupo & "), bajo el radicado : " & Radicado & " Tramite :  " & tipo_tramite
                        nota_cometario = "Nota : Si esta tarea no aparece en su registro de tareas por trabajar es posible que algún integrante de su grupo la esté trabajando"
                        title_comentario_send = "Asignación de tarea : " & Radicado & " Tramite :  " & tipo_tramite &
                               " Remitente : " & nombre_beneficiario
                    End If
                    Dim split_notificacion() As String = {title_comentario,
                                                          comentario,
                                                          nota_cometario}
                    Dim refclascorreo As New ClassCorreo
                    Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion,
                                                                                correos_electronicos,
                                                                                title_comentario_send)
                    If Result <> "YES" Then
                        resultado_correo = "La tarea se envió pero no se notificó al correo electrónico por la siguiente inconsistencia " & Result
                        Terminar_Tarea_Workflow_Bacth = "YES"
                        Exit Function
                    End If
                End If
            End If
            Terminar_Tarea_Workflow_Bacth = "YES"
        Catch ex As Exception
            Terminar_Tarea_Workflow_Bacth = "Inconsistencia funcion Terminar_Tarea_Workflow_Bacth " & ex.Message
        End Try
    End Function

    Function EnviaTareaFInalWorflowArchivaRespuesta(ByVal IdTareaWorkflow As Long,
                                                    ByVal IdRutaWorkflow As Integer,
                                                    ByVal IdActividadDestino As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Funcion que envia la tarea worflow a final para las tareas archivadas
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow      : Representa la identiifcación de la tarea workflow
        'IdRutaWorkflow       : Representa la identificación de la ruta
        'IdActividadDestino   : Representa la identifcación de la actividad destino
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-17
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim StruEstadoTarea As stru_estado = Nothing
            Result = Class_estados_tarea_workflow.SolicitaDatosEstructuraTareasAsinada(IdTareaWorkflow,
                                                                                       StruEstadoTarea)
            If Result <> "YES" Then
                EnviaTareaFInalWorflowArchivaRespuesta = Result
                Exit Function
            End If
            Dim FechaAsignacionWorklow As String = StruEstadoTarea.Fecha_Inicio
            Dim FechaSelecionWorkflow As String = StruEstadoTarea.Fecha_Seleccion
            Dim FechaFinWorkflow As String = ""
            Dim DateCreate As Date = Now
            Dim MinutoDuracioFinal As Long = 0
            Dim ClassGestionFechas As New ClassGestionFechas
            '//-----Solicita la fecha final de la tarea
            Result = ClassGestionFechas.Formatea_fecha_time_framework(DateCreate,
                                                                      FechaFinWorkflow)
            If Result <> "YES" Then
                EnviaTareaFInalWorflowArchivaRespuesta = Result
                Exit Function
            End If
            '//-----Formatea la fecha de asignación de la tarea
            Result = ClassGestionFechas.csfc_Formatea_Fecha_Almacenamiento_Time_bsd(FechaAsignacionWorklow)
            If Result <> "YES" Then
                EnviaTareaFInalWorflowArchivaRespuesta = Result
                Exit Function
            End If
            '//--------Formatea la fecha de selección de la tarea
            If FechaSelecionWorkflow <> "" Then
                Result = ClassGestionFechas.csfc_Formatea_Fecha_Almacenamiento_Time_bsd(FechaSelecionWorkflow)
                If Result <> "YES" Then
                    EnviaTareaFInalWorflowArchivaRespuesta = Result
                    Exit Function
                End If
            End If
            If FechaSelecionWorkflow = "" Then
                FechaSelecionWorkflow = FechaFinWorkflow
            End If

            '//------Solicita los minutos de duración de trabajo con la tarea
            Result = ClassGestionFechas.Resta_fechas_db(FechaAsignacionWorklow,
                                                        FechaFinWorkflow,
                                                        MinutoDuracioFinal)
            If Result <> "YES" Then
                EnviaTareaFInalWorflowArchivaRespuesta = Result
                Exit Function
            End If
            If MinutoDuracioFinal <= -1 Then
                MinutoDuracioFinal = CInt(Abs(MinutoDuracioFinal))
            End If
            Dim ParametroUpdate = "UPDATE ESTADOS_TAREA_WORKFLOW " &
                            "SET FECHA_FIN='" & FechaFinWorkflow & "'" &
                            ",fecha_Seleccion='" & FechaSelecionWorkflow & "'" &
                            ",Duracion_Seleccion_Fin=" & MinutoDuracioFinal &
                            ",TOTAL_DURACION_ACTIVIDAD=" & MinutoDuracioFinal &
                            ",Duracion_inicio_seleccion=" & 0 &
                            ",ESTADO_ACTIVIDA_MODULO_RAD=" & 0 &
                            ",Estado_Tarea='" & "0" & "'" &
                            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow &
                            " AND ID_ACTIVIDAD=" & StruEstadoTarea.Id_Actividad & " and FECHA_FIN IS NULL"
            Dim ParametroInsert = "INSERT INTO ESTADOS_TAREA_WORKFLOW " &
                            "(INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA," &
                            "INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_ACTIVIDAD," &
                            "FECHA_INICIO,ESTADO_PRIORIDAD,ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO) VALUES (" &
                            IdRutaWorkflow & "," &
                            IdTareaWorkflow & "," &
                            IdActividadDestino & ",'" &
                            FechaFinWorkflow & "'," &
                            0 & "," & 0 & "," & 0 & "," & 0 & ")"
            Dim last_insert As Object = 0
            Result = Cambia_Estado_tramite_workflow(ParametroUpdate,
                                                    ParametroInsert,
                                                    last_insert)
            EnviaTareaFInalWorflowArchivaRespuesta = Result
            Exit Function
        Catch ex As Exception
            EnviaTareaFInalWorflowArchivaRespuesta = "Inconsitencia general funcion EnviaTareaFInalWorflowArchivaRespuesta " & ex.Message
        End Try
    End Function
    Function Solicita_fecha_seleccion_tarea(ByVal id_tarea As Long,
                                            ByVal id_actividad As Integer,
                                            ByRef fecha_seleccion As String) As String
        Try
            Dim Sql_consulta As String = "Select Fecha_Seleccion  " &
                " From estados_tarea_workflow " &
                " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea &
                " AND ID_ACTIVIDAD=" & id_actividad & " and FECHA_FIN IS NULL"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_fecha_seleccion_tarea = "Función Solicita_fecha_seleccion_tarea dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_fecha_seleccion_tarea = "Imposible encontrar la fecha de selecciona de la tarea (" & id_tarea & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Solicita_fecha_seleccion_tarea = "La fecha de la tarea  (" & id_tarea & ") se encuentra vacia"
                    Exit Function
                Else
                    Dim j As Date = Datset.Tables(0).Rows(0).Item(0)
                    fecha_seleccion = Trim(CStr(j.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")))
                End If
                Solicita_fecha_seleccion_tarea = "YES"
            End If
        Catch ex As Exception
            Solicita_fecha_seleccion_tarea = "Inconsistencia general función Solicita_fecha_seleccion_tarea " & ex.Message
        End Try
    End Function
    Function Solicita_fecha_asginacion_tarea(ByVal id_tarea As Long,
                                             ByVal id_actividad As Integer,
                                             ByRef fecha_seleccion As String) As String
        Try
            Dim Sql_consulta As String = "Select Fecha_inicio  " &
                " From estados_tarea_workflow " &
                " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea &
                " AND ID_ACTIVIDAD=" & id_actividad & " and FECHA_FIN IS NULL"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_fecha_asginacion_tarea = "Función Solicita_fecha_asginacion_tarea dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_fecha_asginacion_tarea = "Imposible encontrar la fecha de asignación de la tarea (" & id_tarea & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Solicita_fecha_asginacion_tarea = "La fecha de la tarea  (" & id_tarea & ") se encuentra vacia"
                    Exit Function
                Else
                    Dim j As Date = Datset.Tables(0).Rows(0).Item(0)
                    fecha_seleccion = Trim(CStr(j.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")))
                End If
                Solicita_fecha_asginacion_tarea = "YES"
            End If
        Catch ex As Exception
            Solicita_fecha_asginacion_tarea = "Inconsistencia general función Solicita_fecha_asginacion_tarea " & ex.Message
        End Try
    End Function
    Function Lista_Actividades(ByRef scripma As GridView) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ID_ACTIVIDAD,NOMBRE_ACTIVIDAD as NOMBRE_GRUPO,DESCRIPCION_ACTIVIDAD AS DESCRIPCION_GRUPO FROM LISTADO_ACTIVIDADES_WORKFLOW " &
            " WHERE RUTAS_WORKFLOW_ID_RUTA=" & HttpContext.Current.Session.Item("Id_Ruta_Workflow")
            'Sql_consulta = "Select idu_suario,nombre_usuario from usuario_workflow"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_Actividades = "Error Consultando en tabla " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset
                scripma.DataBind()
                Lista_Actividades = "YES"
                Exit Function
            Else
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count
                    Dim tex As String = scripma.Rows(i).Cells(1).Text & "-" & scripma.Rows(i).Cells(2).Text
                    scripma.Rows(i).Attributes.Add("id", tex)
                Next
                Lista_Actividades = "YES"
            End If

        Catch es As Exception
            Lista_Actividades = "Error General " + es.ToString
        End Try
    End Function
    Function Lista_Actividades_Usuario_listview(ByRef scripma As GridView,
                                                ByRef label_resultado As Label) As String
        Try

            Dim Sql_consulta As String = ""
            Sql_consulta = "Select Relacion_Gestion,UW.NOMBRE_USUARIO," &
            "UW.CARGO_USUARIO,GW.NOMBRE_GRUPO,UW.LOGIN_USUARIO,UW.CORREO_USUARIO AS CORREO_ELECTRONICO from USUARIO_WORKFLOW as UW " &
            "Inner join GRUPOS_WORKFLOW as GW on " &
            "(GW.ID_GRUPO=UW.GRUPOS_WORKFLOW_ID_GRUPO) " &
            "WHERE UW.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" & HttpContext.Current.Session("Id_Ruta_Workflow") &
            " and ESTADO_USUARIO=1 ORDER BY UW.NOMBRE_USUARIO ASC"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_Actividades_Usuario_listview = "Error Consultando lista usuarios " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset
                scripma.DataBind()
                label_resultado.Text = Datset.Tables(0).Rows.Count & " usuarios disponibles"
                Lista_Actividades_Usuario_listview = "YES"
                Exit Function
            Else

                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    Dim tex As String = scripma.Rows(i).Cells(1).Text
                    'key = GridViewlista.DataKeys(e.Row.RowIndex).Value.ToString()
                    scripma.Rows(i).Attributes.Add("id", tex)
                    'E.Row.Attributes.Add("id", key)
                Next
                'scripma.Columns.Item(0).Visible = False
                'scripma.Columns.Item(4).Visible = False
                label_resultado.Text = Datset.Tables(0).Rows.Count & " usuarios disponibles"
            End If

            Lista_Actividades_Usuario_listview = "YES"

        Catch es As Exception
            Lista_Actividades_Usuario_listview = "Error General " + es.Message
        End Try
    End Function
    Function Lista_Actividades_Usuario_listview(ByRef scripma As GridView) As String
        Try

            Dim Sql_consulta As String = ""
            Sql_consulta = "Select UW.NOMBRE_USUARIO," &
            "UW.CARGO_USUARIO,GW.NOMBRE_GRUPO,UW.LOGIN_USUARIO,UW.IDU_SUARIO,GW.ID_ACTIVIDAD from USUARIO_WORKFLOW as UW " &
            "Inner join GRUPOS_WORKFLOW as GW on " &
            "(GW.ID_GRUPO=UW.GRUPOS_WORKFLOW_ID_GRUPO) " &
            "WHERE UW.GRUPOS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" & HttpContext.Current.Session("Id_Ruta_Workflow") &
            " and ESTADO_USUARIO=1 ORDER BY UW.NOMBRE_USUARIO ASC"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_Actividades_Usuario_listview = "Error Consultando lista usuarios " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset
                scripma.DataBind()

                Lista_Actividades_Usuario_listview = "YES"
                Exit Function
            Else

                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count
                    Dim tex As String = scripma.Rows(i).Cells(5).Text & "-" & scripma.Rows(i).Cells(6).Text
                    'key = GridViewlista.DataKeys(e.Row.RowIndex).Value.ToString()
                    scripma.Rows(i).Attributes.Add("id", tex)
                    'E.Row.Attributes.Add("id", key)
                Next
                'scripma.Columns.Item(3).Visible = False
                'scripma.Columns.Item(4).Visible = False

            End If

            Lista_Actividades_Usuario_listview = "YES"

        Catch es As Exception
            Lista_Actividades_Usuario_listview = "Error General " + es.Message
        End Try
    End Function
    Function Cambia_Estado(ByVal Parametro_Update As String,
                           ByVal Parametro_Insert As String,
                           ByRef last_insert As Long) As String
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        'myConnection.Open()
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        Try
            myCommand.CommandText = Parametro_Update
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Cambia_Estado = "Imposible actualizar estado tarea función Cambia_Estado " & Parametro_Update
                Exit Function
            End If
            myCommand.CommandText = Parametro_Insert
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_Estado = "Imposible actualizar estado tarea función Cambia_Estado " & Parametro_Update
                Exit Function
            End If
            last_insert = myCommand.LastInsertedId
            myTrans.Commit()
            myConnection.Close()
            Cambia_Estado = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Cambia_Estado = "Error Actualizando  " & Parametro_Update &
                " Insertando " & Parametro_Insert

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Cambia_Estado = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
        End Try
    End Function
    Function Cambia_Estado(ByVal Parametro_Update As String,
                           ByVal Parametro_Insert As String,
                           ByVal Parametro_Estado_Pendiente As String,
                           ByRef last_insert As Long) As String
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        'myConnection.Open()
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        Try
            myCommand.CommandText = Parametro_Update
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Cambia_Estado = "Imposible actualizar estado tarea función Cambia_Estado " & Parametro_Update
                Exit Function
            End If
            myCommand.CommandText = Parametro_Insert
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_Estado = "Imposible actualizar estado tarea función Cambia_Estado " & paramter_resp
                Exit Function
            End If
            last_insert = myCommand.LastInsertedId
            myCommand.CommandText = Parametro_Estado_Pendiente
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_Estado = "Imposible actualizar estado tarea función Cambia_Estado cambia estado pendiente " & Parametro_Estado_Pendiente
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Cambia_Estado = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Cambia_Estado = "Error actualizando  " & Parametro_Update &
                " Insertando " & Parametro_Insert

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Cambia_Estado = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
        End Try
    End Function
    Function Cambia_Estado_tramite_workflow(ByVal Parametro_Update As String,
                                            ByVal Parametro_Insert As String,
                                            ByRef last_insert As Long) As String
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        'myConnection.Open()
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        myTrans = myConnection.BeginTransaction()
        myCommand.Connection = myConnection
        myCommand.Transaction = myTrans
        Dim paramter_resp As Integer = 0
        Try
            myCommand.CommandText = Parametro_Update
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myConnection.Close()
                Cambia_Estado_tramite_workflow = "Imposible actualizar estado tarea función Cambia_Estado " & Parametro_Update
                Exit Function
            End If
            myCommand.CommandText = Parametro_Insert
            paramter_resp = myCommand.ExecuteNonQuery()
            If paramter_resp = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_Estado_tramite_workflow = "Imposible actualizar estado tarea función Cambia_Estado " & paramter_resp
                Exit Function
            End If
            last_insert = myCommand.LastInsertedId
            myTrans.Commit()
            myConnection.Close()
            Cambia_Estado_tramite_workflow = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                myConnection.Close()
                Cambia_Estado_tramite_workflow = "Error Actualizando  " & Parametro_Update &
                " Insertando " & Parametro_Insert

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Cambia_Estado_tramite_workflow = "An exception of type " & ex.GetType().ToString() &
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
        End Try
    End Function
    Function Auto_terminar_flujo_documental(ByRef id_usuario As Integer,
                                            ByRef id_actividad As Integer,
                                            ByRef mensaje As String) As String
        Try
            Dim Result As String = ""
            Dim Radicado As String = ""
            Dim Refclas As New ClassWorkflow
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Auto_terminar_flujo_documental = "El usuario no tiene una tarea seleccionada para auto terminar"
                Exit Function
            End If
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") <> "0" Then
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Radicado)
                If Result <> "YES" Then
                    Auto_terminar_flujo_documental = Result
                    Exit Function
                End If
                If Radicado = "" Then
                    Auto_terminar_flujo_documental = "La tarea seleccionada no tiene radicado seleccionado "
                    Exit Function
                End If
                Dim Refclas_resp As New Classgestionrespuesta
                Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
                '-----Verifica si el usuario es propietario de la respuesta
                Dim id_respuesta As Integer = 0
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                   id_respuesta)
                If Result <> "YES" Then
                    Auto_terminar_flujo_documental = Result
                    Exit Function
                End If
                '-----------------------------------------------
                'Esto se ejecuta si el asignado a la respuesta
                '-----------------------------------------------
                If id_respuesta <> 0 Then
                    '------------------------------------------
                    'Verifica si hay respuestas por confirmar
                    '------------------------------------------
                    Dim estado_respuesta As String = "NO"
                    Result = Refclas_resp.Verfica_respuesta_con_fecha_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                id_respuesta,
                                                                                estado_respuesta)
                    If Result <> "YES" Then
                        Auto_terminar_flujo_documental = Result
                        Exit Function
                    End If
                    If estado_respuesta = "NO" Then
                        Auto_terminar_flujo_documental = "Por favor confirme la respuesta del radicado"
                        Exit Function
                    End If
                    '-----------------------------------------------
                    'Verifica estado solicitudes de aprobación sin
                    'desición
                    '-----------------------------------------------
                    Dim Estado_solicitud_aprobacion As String = ""
                    Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
                    Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")), Estado_solicitud_aprobacion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                    If Result <> "YES" Then
                        Auto_terminar_flujo_documental = Result
                        Exit Function
                    End If
                    If Estado_solicitud_aprobacion = "YES" Then
                        Auto_terminar_flujo_documental = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                        Exit Function
                    End If
                End If


            Else
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Radicado)
                If Result <> "YES" Then
                    Auto_terminar_flujo_documental = Result
                    Exit Function
                End If
            End If
            '******************************************
            'Retorna datos general radicado
            '******************************************
            Dim Refclasgestion As New Classgestionrespuesta
            Dim nombre_plantilla As String = ""
            Dim id_radicado As Integer = 0
            Dim id_usuario_gestion_salida As Integer = 0
            Dim id_uusario_workflow_salida As Integer = 0
            Dim id_grupo_workflow As Integer = 0
            Dim id_activdad_salida As Integer = 0
            Dim nombre_actividad_salida As String = ""
            Dim nombre_usuario_workflow As String = ""
            Result = Refclasgestion.Retorna_datos_general_radicado(Radicado,
                                                                   nombre_plantilla,
                                                                   id_radicado)
            If Result <> "YES" Then
                Auto_terminar_flujo_documental = Result
                Exit Function
            End If
            '***************************************************
            'Solicita el usuario de gestion desde la plantilla
            'del radicado
            '***************************************************
            Result = Refclasgestion.Retorna_id_usuario_gestion_plantilla_radicado(nombre_plantilla,
                                                                                  id_radicado,
                                                                                  id_usuario_gestion_salida)
            If Result <> "YES" Then
                Auto_terminar_flujo_documental = Result
                Exit Function
            End If
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Result = Class_usuario_workflow.Retorna_id_usuario_workflow_usuario_gestion(id_usuario_gestion_salida,
                                                                                        id_uusario_workflow_salida,
                                                                                        id_grupo_workflow,
                                                                                        nombre_usuario_workflow)
            If Result <> "YES" Then
                Auto_terminar_flujo_documental = Result
                Exit Function
            End If
            Dim Ref_class_listado As New Class_Listado_Actividades_workflow
            Result = Ref_class_listado.Retorna_actividad_grupo_workflow(id_grupo_workflow,
                                                                        id_activdad_salida,
                                                                        nombre_actividad_salida)
            If Result <> "YES" Then
                Auto_terminar_flujo_documental = Result
                Exit Function
            End If
            id_usuario = id_uusario_workflow_salida
            id_actividad = id_activdad_salida
            mensaje = "El sistema detecto al usuario (" +
                    nombre_usuario_workflow & ") del área o división (" & nombre_actividad_salida & ") " & vbCrLf & "como el destinatario del documento o tarea " & vbCrLf & " y se dispone a envíar el tramite al usuario. Desea continuar?"
            Auto_terminar_flujo_documental = "YES"
        Catch ex As Exception
            Auto_terminar_flujo_documental = "Inconsistencia función Auto_terminar_flujo_documental " & ex.Message
        End Try
    End Function




    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Function Seleccion_reasigna_envia_tarea_pendiente(ByVal pag As Page, ByVal hdnEmailID_sel As String,
                                                      ByVal id_flujo_trabajo As Integer, ByVal id_actividad_flujo_trabajo As Integer,
                                                      ByVal id_usuario_workflow_flujo_trabajo As Integer) As String
        Try
            Dim ref_ModalPopupExtendermesjpagina As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtendermesjpagina")
            Dim ref_HiddenFiltro As Object = pag.FindControl("HiddenFiltro")
            Dim ref_Hidden_id As Object = pag.FindControl("Hidden_id")
            Dim ref_Hidden_res_envi As Object = pag.FindControl("Hidden_res_envi")
            Dim ref_Hidden_resp_envio As Object = pag.FindControl("Hidden_resp_envio")
            Dim ref_hdnEmailID As Object = pag.FindControl("hdnEmailID")
            If HttpContext.Current.Session.Item("OPCIONSELECIONPENDIENTE") = "ENVIARUSUARIO-S" Then
                If hdnEmailID_sel = "-1" Then
                    Seleccion_reasigna_envia_tarea_pendiente = "Imposible reasignar tareas sin seleccionar"
                    Exit Function
                End If
            End If
            Dim refclas As New ClassWorkflow
            Dim Result As String = ""
            '-------------------------------------------------------------
            'Detecta actividades
            '-------------------------------------------------------------
            Dim split_id_tarea_pendiente() As String = hdnEmailID_sel.ToString.Split("-")
            If split_id_tarea_pendiente Is Nothing Then
                Seleccion_reasigna_envia_tarea_pendiente = "El sistema no detecto las actividades seleccionadas en la ventana pendiente"
                Exit Function
            End If
            If split_id_tarea_pendiente.Length < 2 Then
                Seleccion_reasigna_envia_tarea_pendiente = "El sistema no detecto la celdas en la ventana pendientes"
                Exit Function
            End If
            '-------------------------------------------------------------
            'Detecta usuario Destino
            '-------------------------------------------------------------
            If ref_hdnEmailID.Value = "0" Then
                HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
            Else
                HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ref_hdnEmailID.Value
            End If
            If HttpContext.Current.Session.Item("SESIONITERCAMBIO") = "" Then
                Seleccion_reasigna_envia_tarea_pendiente = "Imposible enviar tarea, usuario no seleccionado"
                Exit Function
            End If
            Dim Split_id_usuario() As String = HttpContext.Current.Session.Item("SESIONITERCAMBIO").ToString.Split("-")
            If Split_id_usuario Is Nothing Then
                Seleccion_reasigna_envia_tarea_pendiente = "El sistema no detecto el usuario seleccionado en la actividad pendiente"
                Exit Function
            End If
            If Split_id_usuario.Length < 2 Then
                Seleccion_reasigna_envia_tarea_pendiente = "El sistema no detecto la celdas en la ventana usuarios"
                Exit Function
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(split_id_tarea_pendiente(1)), Estado_solicitud_aprobacion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Seleccion_reasigna_envia_tarea_pendiente = Result
                Exit Function
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Seleccion_reasigna_envia_tarea_pendiente = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                Exit Function
            End If
            Dim resultado_correo As String = ""
            Dim resultado_correo_general As String = ""
            Dim refclas_gestino_resp As New Classgestionrespuesta
            Result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario_batch(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       split_id_tarea_pendiente(1),
                                                                                       Split_id_usuario(0), Split_id_usuario(1),
                                                                                       Split_id_usuario(0), split_id_tarea_pendiente(0), "",
                                                                                       resultado_correo,
                                                                                       " sin autorizacion usuario permitido",
                                                                                       id_flujo_trabajo,
                                                                                       id_actividad_flujo_trabajo,
                                                                                       id_usuario_workflow_flujo_trabajo,
                                                                                       1)
            If Result <> "YES" Then
                ref_Hidden_res_envi.Value = ""
                ref_Hidden_resp_envio.value = ""
                Seleccion_reasigna_envia_tarea_pendiente = Result
                Exit Function
            Else
                HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                HttpContext.Current.Session.Item("OPCIONSELECIONPENDIENTE") = ""
                ref_Hidden_res_envi.Value = "YES"
                ref_Hidden_resp_envio.value = "YES"
                ref_HiddenFiltro.Value = ""
                ref_ModalPopupExtendermesjpagina.Hide()
                Seleccion_reasigna_envia_tarea_pendiente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Seleccion_reasigna_envia_tarea_pendiente = "Inconsistencia general función Seleccion_reasigna_envia_tarea_pendiente " & ex.Message
        End Try
    End Function
    Function Pre_envio_tarea_a_qctividad_desde_pendiente(ByVal pag As Page,
                                                         ByVal hdnEmailID_sel As String,
                                                         ByRef resul_correo As String) As String
        '-------------------------------------------------------
        'Función : Selecciona el tipo de envío de actividad
        'desde pendiente
        'Fecha : 2017-10-11
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim ref_ModalPopupExtendermesjpagina As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtendermesjpagina")
            Dim ref_HiddenFiltro As Object = pag.FindControl("HiddenFiltro")
            Dim ref_Hidden_id As Object = pag.FindControl("Hidden_id")
            Dim ref_Hidden_res_envi As Object = pag.FindControl("Hidden_res_envi")
            Dim ref_hdnEmailID As Object = pag.FindControl("hdnEmailID")
            Dim ref_CheckBox_noti_envio As CheckBox = pag.FindControl("CheckBox_noti_envio")
            Dim sa = HttpContext.Current.Session.Item("OPCIONSELECIONPENDIENTE")
            If HttpContext.Current.Session.Item("OPCIONSELECIONPENDIENTE") = "ENVIARACTIVIDAD-S*" Then
                If hdnEmailID_sel = "-1" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "Imposible enviar tareas sin seleccionar"
                    Exit Function
                End If
                '-------------------------------------------------------------
                'Detecta tareas seleccionadas
                '-------------------------------------------------------------
                Dim split_id_tarea_pendiente() As String = hdnEmailID_sel.ToString.Split("-")
                If split_id_tarea_pendiente Is Nothing Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "El sistema no detecto las actividades seleccionadas en la ventana pendiente"
                    Exit Function
                End If
                If split_id_tarea_pendiente.Length < 2 Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "El sistema no detecto la celdas en la ventana pendientes"
                    Exit Function
                End If
                Dim refclas As New ClassWorkflow
                Dim Result As String = ""
                If ref_hdnEmailID.Value = "0" Then
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                Else
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ref_hdnEmailID.Value
                End If

                If HttpContext.Current.Session.Item("SESIONITERCAMBIO") = "" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "Imposible enviar la tarea a la actividad seleccionada"
                    Exit Function
                End If
                '-------------------------------------------------------------
                'Detecta actividad Destino
                '-------------------------------------------------------------
                Dim Split_id_usuario() As String = HttpContext.Current.Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                If Split_id_usuario Is Nothing Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "El sistema no detecto la actividad seleccionada en la lista de actividades"
                    Exit Function
                End If
                If Split_id_usuario.Length < 2 Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "El sistema no detecto la celdas en la ventana de actividades"
                    Exit Function
                End If

                '---------------------------------
                'Verifica respuesta radicado
                '---------------------------------
                Dim refclasgestion As New Classgestionrespuesta
                Result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                  split_id_tarea_pendiente(1))
                If Result <> "YES" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = Result
                    Exit Function
                End If
                '-----------------------------------------------
                'Verifica estado solicitudes de aprobación sin
                'desición
                '-----------------------------------------------
                Dim Estado_solicitud_aprobacion As String = ""
                Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
                Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(split_id_tarea_pendiente(1)),
                                                                                             Estado_solicitud_aprobacion,
                                                                                             HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = Result
                    Exit Function
                End If
                If Estado_solicitud_aprobacion = "YES" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                    Exit Function
                End If
                '------------------------------------
                'Eestado envió correo electrónico
                '------------------------------------
                Dim estado_envio_correo As Integer = 0
                If ref_CheckBox_noti_envio.Checked = True Then
                    estado_envio_correo = 1
                Else
                    estado_envio_correo = 0
                End If
                Result = refclas.Terminar_Tarea_Workflow_Bacth("",
                                                               Split_id_usuario(0),
                                                               split_id_tarea_pendiente(0),
                                                               split_id_tarea_pendiente(1),
                                                               Split_id_usuario(1),
                                                               0, 0, 0, estado_envio_correo, resul_correo, 1)

                If Result <> "YES" Then
                    ref_Hidden_res_envi.Value = ""
                    Pre_envio_tarea_a_qctividad_desde_pendiente = Result
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                    HttpContext.Current.Session.Item("OPCIONSELECIONPENDIENTE") = ""
                    ref_Hidden_res_envi.Value = "YES"
                    ref_HiddenFiltro.Value = ""
                    ref_ModalPopupExtendermesjpagina.Hide()
                End If
            End If
            'ENVIARUSUARIO-S
            If HttpContext.Current.Session.Item("OPCIONSELECIONPENDIENTE") = "ENVIARUSUARIO-S*" Then
                If hdnEmailID_sel = "-1" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "Imposible enviar tareas sin seleccionar"
                    Exit Function
                End If
                '-------------------------------------------------------------
                'Detecta tareas seleccionadas
                '-------------------------------------------------------------
                Dim split_id_tarea_pendiente() As String = hdnEmailID_sel.ToString.Split("-")
                If split_id_tarea_pendiente Is Nothing Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "El sistema no detecto las actividades seleccionadas en la ventana pendiente"
                    Exit Function
                End If
                If split_id_tarea_pendiente.Length < 2 Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "El sistema no detecto la celdas en la ventana pendientes"
                    Exit Function
                End If
                Dim refclas As New ClassWorkflow
                Dim Result As String = ""
                If ref_hdnEmailID.Value = "0" Then
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                Else
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ref_hdnEmailID.Value
                End If

                If HttpContext.Current.Session.Item("SESIONITERCAMBIO") = "" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "Imposible enviar la tarea a la actividad seleccionada"
                    Exit Function
                End If
                '-------------------------------------------------------------
                'Detecta actividad Destino
                '-------------------------------------------------------------
                Dim Split_id_usuario() As String = HttpContext.Current.Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                If Split_id_usuario Is Nothing Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "El sistema no detecto la actividad seleccionada en la lista de actividades"
                    Exit Function
                End If
                If Split_id_usuario.Length < 2 Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "El sistema no detecto la celdas en la ventana de actividades"
                    Exit Function
                End If

                '---------------------------------
                'Verifica respuesta radicado
                '---------------------------------
                Dim refclasgestion As New Classgestionrespuesta
                Result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                  split_id_tarea_pendiente(1))
                If Result <> "YES" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = Result
                    Exit Function
                End If
                '-----------------------------------------------
                'Verifica estado solicitudes de aprobación sin
                'desición
                '-----------------------------------------------
                Dim Estado_solicitud_aprobacion As String = ""
                Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
                Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(split_id_tarea_pendiente(1)),
                                                                                             Estado_solicitud_aprobacion,
                                                                                             HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = Result
                    Exit Function
                End If
                If Estado_solicitud_aprobacion = "YES" Then
                    Pre_envio_tarea_a_qctividad_desde_pendiente = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                    Exit Function
                End If
                '------------------------------------
                'Eestado envió correo electrónico
                '------------------------------------
                Dim estado_envio_correo As Integer = 0
                If ref_CheckBox_noti_envio.Checked = True Then
                    estado_envio_correo = 1
                Else
                    estado_envio_correo = 0
                End If
                Result = refclas.Terminar_Tarea_Workflow_Bacth(Split_id_usuario(0),
                                                               Split_id_usuario(1),
                                                              split_id_tarea_pendiente(0),
                                                              split_id_tarea_pendiente(1),
                                                              "",
                                                              0, 0, 0, estado_envio_correo, resul_correo, 1)
                If Result <> "YES" Then
                    ref_Hidden_res_envi.Value = ""
                    Pre_envio_tarea_a_qctividad_desde_pendiente = Result
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                    HttpContext.Current.Session.Item("OPCIONSELECIONPENDIENTE") = ""
                    ref_Hidden_res_envi.Value = "YES"
                    ref_HiddenFiltro.Value = ""
                    ref_ModalPopupExtendermesjpagina.Hide()
                End If
            End If
            Pre_envio_tarea_a_qctividad_desde_pendiente = "YES"
        Catch ex As Exception
            Pre_envio_tarea_a_qctividad_desde_pendiente = "Inconsistencia general función Pre_envio_tarea_a_qctividad_desde_pendiente " & ex.Message
        End Try

    End Function
    Function Pre_reasigna_tarea_usuario_desde_pendiente(ByVal pag As Page,
                                                        ByVal hdnEmailID_sel As String,
                                                        ByVal id_flujo_trabajo As Integer,
                                                        ByVal id_actividad_flujo_trabajo As Integer,
                                                        ByVal id_usuario_workflow_flujo_trabajo As Integer,
                                                        ByVal usuario_autoriza As String) As String
        '-------------------------------------------------------
        'Función : Reasigna y envia la tarea a un usuario
        'desde pendiente
        'Fecha : 2017-10-11
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim ref_ModalPopupExtendermesjpagina As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtendermesjpagina")
            Dim ref_HiddenFiltro As Object = pag.FindControl("HiddenFiltro")
            Dim ref_Hidden_id As Object = pag.FindControl("Hidden_id")
            Dim ref_Hidden_res_envi As Object = pag.FindControl("Hidden_res_envi")
            Dim ref_hdnEmailID As Object = pag.FindControl("hdnEmailID")
            If hdnEmailID_sel = "-1" Then
                Pre_reasigna_tarea_usuario_desde_pendiente = "Imposible enviar tareas sin seleccionar"
                Exit Function
            End If
            Dim refclas As New ClassWorkflow
            Dim Result As String = ""
            '-------------------------------------------------------------
            'Detecta actividades
            '-------------------------------------------------------------
            Dim split_id_tarea_pendiente() As String = hdnEmailID_sel.ToString.Split("-")
            If split_id_tarea_pendiente Is Nothing Then
                Pre_reasigna_tarea_usuario_desde_pendiente = "El sistema no detecto las actividades seleccionadas en la ventana pendiente"
                Exit Function
            End If
            If split_id_tarea_pendiente.Length < 2 Then
                Pre_reasigna_tarea_usuario_desde_pendiente = "El sistema no detecto la celdas en la ventana pendientes"
                Exit Function
            End If
            '-------------------------------------------------------------
            'Detecta usuario Destino
            '-------------------------------------------------------------
            If ref_hdnEmailID.Value = "0" Then
                HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
            Else
                HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ref_hdnEmailID.Value
            End If
            If HttpContext.Current.Session.Item("SESIONITERCAMBIO") = "" Then
                Pre_reasigna_tarea_usuario_desde_pendiente = "Imposible enviar tarea, usuario no seleccionado"
                Exit Function
            End If
            Dim Split_id_usuario_actividad() As String = HttpContext.Current.Session.Item("SESIONITERCAMBIO").ToString.Split("-")
            If Split_id_usuario_actividad Is Nothing Then
                Pre_reasigna_tarea_usuario_desde_pendiente = "El sistema no detecto el usuario seleccionado en la lista"
                Exit Function
            End If
            If Split_id_usuario_actividad.Length < 2 Then
                Pre_reasigna_tarea_usuario_desde_pendiente = "El sistema no detecto la celdas en la ventana usuarios"
                Exit Function
            End If
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            'Dim refclasgestion As New Classgestionrespuesta
            'Result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), split_id_tarea_pendiente(1))
            'If Result <> "YES" Then
            '    Pre_reasigna_tarea_usuario_desde_pendiente = Result
            '    Exit Function
            'End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(split_id_tarea_pendiente(1)), Estado_solicitud_aprobacion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Pre_reasigna_tarea_usuario_desde_pendiente = Result
                Exit Function
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Pre_reasigna_tarea_usuario_desde_pendiente = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                Exit Function
            End If
            Dim refclas_gestino_resp As New Classgestionrespuesta
            Result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario_batch(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                      split_id_tarea_pendiente(1),
                                                                                      Split_id_usuario_actividad(0),
                                                                                      Split_id_usuario_actividad(1),
                                                                                      Split_id_usuario_actividad(0),
                                                                                      split_id_tarea_pendiente(0),
                                                                                      "",
                                                                                      "",
                                                                                      usuario_autoriza,
                                                                                      id_flujo_trabajo,
                                                                                      id_actividad_flujo_trabajo,
                                                                                      id_usuario_workflow_flujo_trabajo,
                                                                                      1)
            If Result <> "YES" Then
                ref_Hidden_res_envi.Value = ""
                Pre_reasigna_tarea_usuario_desde_pendiente = Result
                Exit Function
            Else
                HttpContext.Current.Session.Item("SESIONITERCAMBIO") = ""
                HttpContext.Current.Session.Item("OPCIONSELECIONPENDIENTE") = ""
                ref_Hidden_res_envi.Value = "YES"
                ref_HiddenFiltro.Value = ""
                'ref_ModalPopupExtendermesjpagina.Hide()
                Pre_reasigna_tarea_usuario_desde_pendiente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Pre_reasigna_tarea_usuario_desde_pendiente = "Inconsistencia general función Pre_rasigna_tarea_pendiente " & ex.Message
        End Try

    End Function

    Function Reasigna_actividad_con_autorizacion_flujo_trabajo(ByVal pag As Page,
                                                               ByVal usuario_autoriza As String,
                                                               ByVal evalua_actividades_pendientes_tarea As Integer,
                                                               ByVal id_tarea_seleccionada As Integer,
                                                               ByRef tre As TreeView,
                                                               ByRef Resultado_evalua_terminar As String) As String
        Try

            Dim ref_Hidden_id_usuario_workflow As Object = pag.FindControl("Hidden_id_usuario_workflow")
            Dim ref_Hidden_id_actividad_ruta As Object = pag.FindControl("Hidden_id_actividad_ruta")
            Dim ref_Hidden_id_flujo_trabjo As Object = pag.FindControl("Hidden_id_flujo_trabjo")
            Dim ref_Hidden_id_actividad_flujo As Object = pag.FindControl("Hidden_id_actividad_flujo")
            Dim ref_Hidden_result_envi_flujo As Object = pag.FindControl("Hidden_result_envi_flujo")
            Dim ref_Hidden_id_actividad_destino As Object = pag.FindControl("Hidden_id_actividad_destino")
            Dim ref_TextBox_pasw_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_pasw_lista_actividades_ruta_flujo")
            Dim ref_TextBox_login_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_login_lista_actividades_ruta_flujo")
            Dim ref_UpdatePanel_autoriza_reasignacion_tarea_flujo As UpdatePanel = pag.FindControl("UpdatePanel_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_ruta_flujo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_worflow_ruta")
            Dim result As String = ""
            Dim refclasgestion As New Classgestionrespuesta
            If evalua_actividades_pendientes_tarea = 1 Then
                '---------------------------------
                'Verifica respuesta radicado
                '---------------------------------
                Dim result_ As String = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), id_tarea_seleccionada)
                If result_ <> "YES" Then
                    Reasigna_actividad_con_autorizacion_flujo_trabajo = result
                    Exit Function
                End If
                '-----------------------------------------------
                'Verifica estado solicitudes de aprobación sin
                'desición
                '-----------------------------------------------
                Dim Estado_solicitud_aprobacion As String = ""
                Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
                result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(id_tarea_seleccionada,
                                                                                             Estado_solicitud_aprobacion,
                                                                                             HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If result <> "YES" Then
                    Reasigna_actividad_con_autorizacion_flujo_trabajo = result
                    Exit Function
                End If
                If Estado_solicitud_aprobacion = "YES" Then
                    Reasigna_actividad_con_autorizacion_flujo_trabajo = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                    Exit Function
                End If
            End If

            '------------------------------------------------
            'Reasigna y envia la tarea por medio de flujo
            'de trabajo
            '------------------------------------------------
            Dim refclas_gestino_resp As New Classgestionrespuesta
            result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                id_tarea_seleccionada,
                                                                                ref_Hidden_id_usuario_workflow.value,
                                                                                ref_Hidden_id_actividad_destino.value,
                                                                                ref_Hidden_id_usuario_workflow.value,
                                                                                tre,
                                                                                "",
                                                                                usuario_autoriza,
                                                                                0,
                                                                                pag,
                                                                                ref_Hidden_id_flujo_trabjo.value,
                                                                                ref_Hidden_id_actividad_flujo.Value,
                                                                                ref_Hidden_id_usuario_workflow.value,
                                                                                Resultado_evalua_terminar)
            If result <> "YES" Then
                Reasigna_actividad_con_autorizacion_flujo_trabajo = result
                Exit Function
            Else
                ref_Hidden_id_flujo_trabjo.value = "0"
                ref_Hidden_id_actividad_flujo.Value = "0"
                ref_Hidden_id_usuario_workflow.value = "0"
                ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                Reasigna_actividad_con_autorizacion_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Reasigna_actividad_con_autorizacion_flujo_trabajo = "Inconsistencia general función Reasigna_actividad_con_autorizacion_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente(ByVal pag As Page,
                                                                           ByVal id_actividad_wf_envia As Integer,
                                                                           ByVal id_usuario_wf_envia As Integer) As String
        Try
            Dim ref_Hidden_id As Object = pag.FindControl("Hidden_id")
            Dim ref_Hidden_id_usuario_workflow As Object = pag.FindControl("Hidden_id_usuario_workflow")
            Dim ref_Hidden_id_actividad_ruta As Object = pag.FindControl("Hidden_id_actividad_ruta")
            Dim ref_Hidden_id_flujo_trabjo As Object = pag.FindControl("Hidden_id_flujo_trabjo")
            Dim ref_Hidden_id_actividad_flujo As Object = pag.FindControl("Hidden_id_actividad_flujo")
            Dim ref_Hidden_result_envi_flujo As Object = pag.FindControl("Hidden_result_envi_flujo")
            Dim ref_Hidden_id_actividad_destino As Object = pag.FindControl("Hidden_id_actividad_destino")
            Dim ref_Hidden_id_conector As Object = pag.FindControl("Hidden_id_conector")
            Dim ref_TextBox_pasw_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_pasw_lista_actividades_ruta_flujo")
            Dim ref_TextBox_login_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_login_lista_actividades_ruta_flujo")
            Dim ref_UpdatePanel_autoriza_reasignacion_tarea_flujo As UpdatePanel = pag.FindControl("UpdatePanel_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_ruta_flujo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_worflow_ruta")
            Dim result As String = ""
            If ref_Hidden_id.Value = "-1" Then
                Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = "Imposible enviar tarea sin seleccionar"
                Exit Function
            End If
            Dim split_id_tarea_pendiente() As String = ref_Hidden_id.Value.ToString.Split("-")
            If split_id_tarea_pendiente Is Nothing Then
                Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = "El sistema no detecto las actividades seleccionadas en la ventana pendiente"
                Exit Function
            End If
            If split_id_tarea_pendiente.Length < 2 Then
                Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = "El sistema no detecto la celdas en la ventana pendientes"
                Exit Function
            End If
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            Dim result_ As String = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), split_id_tarea_pendiente(1))
            If result_ <> "YES" And result_ <> "El trámite requiere de un radicado de respuesta" And result_ <> "El trámite requiere de una confirmación de respuesta" Then
                Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = result_
                Exit Function
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(split_id_tarea_pendiente(1)),
                                                                                         Estado_solicitud_aprobacion,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If result <> "YES" Then
                Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = result
                Exit Function
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                Exit Function
            End If
            '------------------------------------------------
            'Reasigna y envia la tarea por medio de flujo
            'de trabajo
            '------------------------------------------------
            If result_ = "El trámite requiere de un radicado de respuesta" _
                Or result_ = "El trámite requiere de una confirmación de respuesta" Then
                If ref_Hidden_id_usuario_workflow.Value = "0" Then
                    Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = "El trámite requiere de un radicado de respuesta, no se permite reasignar a una actividad de grupos de usuarios"
                    Exit Function
                End If
                If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW") = 0 Then
                    '-------------------------------------------------------------
                    'Reasigna tarea con autorización
                    '-------------------------------------------------------------
                    ref_TextBox_login_autoriza_reasignacion_tarea_flujo.Text = ""
                    ref_TextBox_pasw_autoriza_reasignacion_tarea_flujo.Text = ""
                    ref_UpdatePanel_autoriza_reasignacion_tarea_flujo.Update()
                    ref_ModalPopupExtender_edition_lista_actividades_ruta_flujo.Show()
                    Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = "YES"
                    Exit Function
                Else
                    '---------------------------------------------------------
                    'Reasigna y envia tarea a usuario
                    '---------------------------------------------------------
                    Dim refclas_gestino_resp As New Classgestionrespuesta
                    result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario_batch(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                               split_id_tarea_pendiente(1),
                                                                                               ref_Hidden_id_usuario_workflow.value,
                                                                                               ref_Hidden_id_actividad_destino.value,
                                                                                               ref_Hidden_id_usuario_workflow.value,
                                                                                               split_id_tarea_pendiente(0),
                                                                                               "",
                                                                                               "",
                                                                                               "sin autorizacion usuario permitido",
                                                                                               ref_Hidden_id_flujo_trabjo.value,
                                                                                               ref_Hidden_id_actividad_flujo.Value,
                                                                                               ref_Hidden_id_usuario_workflow.value,
                                                                                               1)
                    If result <> "YES" Then
                        ref_Hidden_result_envi_flujo.value = ""
                        Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = result
                        Exit Function
                    Else
                        ref_Hidden_id_flujo_trabjo.value = "0"
                        ref_Hidden_id_actividad_flujo.Value = "0"
                        ref_Hidden_id_usuario_workflow.value = "0"
                        ref_Hidden_result_envi_flujo.value = "YES"
                        ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                        Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = "YES"
                        Exit Function
                    End If

                End If
            End If

            Dim id_usuario_destino As Object
            If ref_Hidden_id_usuario_workflow.Value = "0" Or ref_Hidden_id_usuario_workflow.Value = "&nbsp;" Then
                id_usuario_destino = ""
                ref_Hidden_id_usuario_workflow.Value = "0"
            Else
                id_usuario_destino = ref_Hidden_id_usuario_workflow.Value
            End If
            '----------------------------------------------------------
            'Solicita estado envió notificación al correo electrónico
            '----------------------------------------------------------
            Dim ref_clas_ As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            Dim estado_envio_correo As Integer = 0
            Dim Result_corrreo As String = ""
            Dim id_conector As Object = Val(ref_Hidden_id_conector.Value)
            result = ref_clas_.Solicita_estado_notifica_envio_conector(id_conector,
                                                                       estado_envio_correo)
            If result <> "YES" Then
                Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = result
                Exit Function
            End If
            Dim refclas As New ClassWorkflow
            result = refclas.Terminar_Tarea_Workflow_Bacth(id_usuario_destino,
                                                           ref_Hidden_id_actividad_destino.Value,
                                                           split_id_tarea_pendiente(0),
                                                           split_id_tarea_pendiente(1),
                                                           "",
                                                           ref_Hidden_id_flujo_trabjo.Value,
                                                           ref_Hidden_id_actividad_flujo.Value,
                                                           ref_Hidden_id_usuario_workflow.Value,
                                                           estado_envio_correo,
                                                           Result_corrreo, 1,
                                                           id_conector,
                                                           id_usuario_wf_envia,
                                                           id_actividad_wf_envia)
            If result <> "YES" Then
                ref_Hidden_result_envi_flujo.value = ""
                Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = result
                Exit Function
            Else
                ref_Hidden_id_flujo_trabjo.value = "0"
                ref_Hidden_id_actividad_flujo.Value = "0"
                ref_Hidden_id_usuario_workflow.value = "0"
                ref_Hidden_result_envi_flujo.value = "YES"
                ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                If Result_corrreo <> "" Then
                    Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = Result_corrreo
                Else
                    Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = "YES"
                End If
                Exit Function
            End If
        Catch ex As Exception
            Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente = "Inconsistencia general función Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente " & ex.Message
        End Try
    End Function

    Function Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente(ByVal pag As Page,
                                                                         ByVal usuario_autoriza As String,
                                                                         ByVal evalua_actividades_pendientes_tarea As Integer) As String
        Try
            Dim ref_Hidden_id As Object = pag.FindControl("Hidden_id")
            Dim ref_Hidden_id_usuario_workflow As Object = pag.FindControl("Hidden_id_usuario_workflow")
            Dim ref_Hidden_id_actividad_ruta As Object = pag.FindControl("Hidden_id_actividad_ruta")
            Dim ref_Hidden_id_flujo_trabjo As Object = pag.FindControl("Hidden_id_flujo_trabjo")
            Dim ref_Hidden_id_actividad_flujo As Object = pag.FindControl("Hidden_id_actividad_flujo")
            Dim ref_Hidden_result_envi_flujo As Object = pag.FindControl("Hidden_result_envi_flujo")
            Dim ref_Hidden_id_actividad_destino As Object = pag.FindControl("Hidden_id_actividad_destino")
            Dim ref_TextBox_pasw_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_pasw_lista_actividades_ruta_flujo")
            Dim ref_TextBox_login_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_login_lista_actividades_ruta_flujo")
            Dim ref_UpdatePanel_autoriza_reasignacion_tarea_flujo As UpdatePanel = pag.FindControl("UpdatePanel_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_ruta_flujo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_worflow_ruta")
            Dim result As String = ""
            If ref_Hidden_id.Value = "-1" Then
                Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente = "Imposible enviar tarea sin seleccionar"
                Exit Function
            End If
            Dim split_id_tarea_pendiente() As String = ref_Hidden_id.Value.ToString.Split("-")
            If split_id_tarea_pendiente Is Nothing Then
                Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente = "El sistema no detecto las actividades seleccionadas en la ventana pendiente"
                Exit Function
            End If
            If split_id_tarea_pendiente.Length < 2 Then
                Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente = "El sistema no detecto la celdas en la ventana pendientes"
                Exit Function
            End If

            Dim refclasgestion As New Classgestionrespuesta
            If evalua_actividades_pendientes_tarea = 1 Then
                '---------------------------------
                'Verifica respuesta radicado
                '---------------------------------
                Dim result_ As String = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), split_id_tarea_pendiente(1))
                If result_ <> "YES" Then
                    Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente = result
                    Exit Function
                End If
                '-----------------------------------------------
                'Verifica estado solicitudes de aprobación sin
                'desición
                '-----------------------------------------------
                Dim Estado_solicitud_aprobacion As String = ""
                Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
                result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(split_id_tarea_pendiente(1)), Estado_solicitud_aprobacion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If result <> "YES" Then
                    Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente = result
                    Exit Function
                End If
                If Estado_solicitud_aprobacion = "YES" Then
                    Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                    Exit Function
                End If
            End If

            '------------------------------------------------
            'Reasigna y envia la tarea por medio de flujo
            'de trabajo
            '------------------------------------------------
            Dim refclas_gestino_resp As New Classgestionrespuesta
            result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario_batch(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       split_id_tarea_pendiente(1),
                                                                                       ref_Hidden_id_usuario_workflow.value,
                                                                                       ref_Hidden_id_actividad_destino.value,
                                                                                       ref_Hidden_id_usuario_workflow.value,
                                                                                       split_id_tarea_pendiente(0),
                                                                                       "",
                                                                                       "",
                                                                                       "sin autorizacion usuario permitido",
                                                                                       ref_Hidden_id_flujo_trabjo.value,
                                                                                       ref_Hidden_id_actividad_flujo.Value,
                                                                                       ref_Hidden_id_usuario_workflow.value,
                                                                                       1)
            If result <> "YES" Then
                ref_Hidden_result_envi_flujo.value = ""
                Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente = result
                Exit Function
            Else
                ref_Hidden_id_flujo_trabjo.value = "0"
                ref_Hidden_id_actividad_flujo.Value = "0"
                ref_Hidden_id_usuario_workflow.value = "0"
                ref_Hidden_result_envi_flujo.value = "YES"
                ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente = "Inconsistencia general función Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente " & ex.Message
        End Try
    End Function
    Function Registra_flujo_documento(ByVal id_actividad As Integer,
                                      ByVal id_usuario_workflow As Integer,
                                      ByVal id_imagen As Integer,
                                      ByVal radicado As String,
                                      ByVal id_plantilla_radicado As Integer,
                                      ByVal ID_FLUJO_TRABAJO As Integer,
                                      ByVal ID_ACTIVIDAD_FLUJO_TRABAJO As Integer,
                                      ByVal ID_USUARIO_WORKFLOW_FLUJO_TRABAJO As Integer,
                                      ByVal ESTADO_RECUPERACION_FLUJO_TRABAJO As Integer,
                                      ByVal estado_modulo_radicado As Integer,
                                      ByRef id_tarea_workflow As Long,
                                      ByVal fecha_seleccion As Object,
                                      ByVal estado_activida_modulo_rad As Integer) As String
        '----------------------------------------------------------------
        'Función : Registra flujo documental atomico se utiliza
        'para la radicación interna de documentos
        'Fecha : 2017-12-14
        'Ingeniero : Migue Angel Urueta Miranda
        '----------------------------------------------------------------
        Dim Result As String = ""
        Dim Refclas_workflow As New Class_worflow_rutas
        '----------------------------------------------------------------
        'Solicita el nombre de la ruta Retorna_id_ruta_por_id_actividad
        '----------------------------------------------------------------
        Dim nombre_ruta As String = ""
        Dim id_ruta As Integer = 0
        Result = Refclas_workflow.Retorna_id_ruta_por_id_actividad(id_actividad,
                                                                   id_ruta)
        If Result <> "YES" Then
            Registra_flujo_documento = Result
            Exit Function
        End If
        Dim Ref_class_ruta As New Class_worflow_rutas
        If HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") = "" Then
            Result = Ref_class_ruta.Retorna_nombre_ruta_workflow(nombre_ruta)
            If Result <> "YES" Then
                Registra_flujo_documento = Result
                Exit Function
            Else
                HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") = nombre_ruta
            End If
        Else
            nombre_ruta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
        End If
        Dim ref_id_usuario_workflow As Object = "Null"
        If id_usuario_workflow <> 0 Then
            ref_id_usuario_workflow = id_usuario_workflow
        End If
        '----------------------------------------------------
        'Solicita los campos relación de la plantilla de 
        'radicación y la ruta workflow
        '-----------------------------------------------------
        Dim Class_ra_relacion_ruta_plantilla As New Class_ra_relacion_ruta_plantilla
        Dim stru_campos_plantilla_ruta() As csfc_structure_relacion_campos_plantilla_ruta = Nothing
        Result = Class_ra_relacion_ruta_plantilla.solicita_campos_relacion_ruta_plantilla(id_plantilla_radicado,
                                                                                          id_ruta,
                                                                                          stru_campos_plantilla_ruta)
        If Result <> "YES" Then
            Registra_flujo_documento = Result
            Exit Function
        End If
        '---------------------------------------------------
        'Solicita nombre plantilla radicado
        '---------------------------------------------------
        Dim nombre_plantilla_radicado As String = ""
        Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
        Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(id_plantilla_radicado,
                                                                             nombre_plantilla_radicado)
        If Result <> "YES" Then
            Registra_flujo_documento = Result
            Exit Function
        End If
        '------------------------------------------------------------
        'Verifica la existencia del radicado en el flujo documental
        '------------------------------------------------------------
        Dim estado_existencia_radicado As String = "YES"
        Result = csfc_verifica_existencia_radicado(radicado,
                                                   nombre_ruta,
                                                   estado_existencia_radicado,
                                                   id_tarea_workflow)
        If Result <> "YES" Then
            Registra_flujo_documento = Result
            Exit Function
        End If
        If estado_existencia_radicado = "YES" Then
            Registra_flujo_documento = "El sistema detecto la existencia el radicado (" & radicado & ") en el flujo documental "
            Exit Function
        End If
        '---------------------------------------------------
        'Asigna datos estructura plantilla radicacion ruta
        '----------------------------------------------------
        Result = Me.Asigna_datos_estructura_plantilla_radicacion_ruta(nombre_plantilla_radicado,
                                                                      id_plantilla_radicado,
                                                                      radicado,
                                                                      stru_campos_plantilla_ruta)
        If Result <> "YES" Then
            Registra_flujo_documento = Result
            Exit Function
        End If
        '--------------------------------------------------------
        'Formatea campos tipo date  y date time
        '--------------------------------------------------------
        Dim refclas_ClassGestionFechas As New ClassGestionFechas
        For i As Integer = 0 To stru_campos_plantilla_ruta.Length - 1
            If stru_campos_plantilla_ruta(i).tipo_campo_plantilla = "DATE" Then
                If Not stru_campos_plantilla_ruta(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_ruta(i).dato_campo_plantilla <> "" Then
                    Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio(stru_campos_plantilla_ruta(i).dato_campo_plantilla)
                    If Result <> "YES" Then
                        Registra_flujo_documento = Result
                        Exit Function
                    End If
                End If
            End If
            If stru_campos_plantilla_ruta(i).tipo_campo_plantilla = "DATETIME" Then
                If Not stru_campos_plantilla_ruta(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_ruta(i).dato_campo_plantilla <> "" Then
                    Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Almacenamiento_Time_bsd(stru_campos_plantilla_ruta(i).dato_campo_plantilla)
                    If Result <> "YES" Then
                        Registra_flujo_documento = Result
                        Exit Function
                    End If
                End If

            End If
        Next
        '------------------------------------------------------
        'Solicita tipo tramite radicado
        '------------------------------------------------------
        Dim tipo_tramite_radicado As String = ""
        Result = Me.Solicita_tipo_tramite_radicado(nombre_plantilla_radicado,
                                                   radicado,
                                                   tipo_tramite_radicado)
        If Result <> "YES" Then
            Registra_flujo_documento = Result
            Exit Function
        End If
        '-------------------------------------------------------
        'Solicita nombre gabinete workflow
        '-------------------------------------------------------
        Dim nombre_gabinete_wf As String = ""
        Result = Me.Solicita_nombre_gabinete_workflow(tipo_tramite_radicado,
                                                      id_plantilla_radicado,
                                                      nombre_gabinete_wf)
        If Result <> "YES" Then
            Registra_flujo_documento = Result
            Exit Function
        End If
        '-------------------------------------------------------
        'Solicita id gabinete en la tabla de configuración
        '-------------------------------------------------------
        Dim id_gabinete As Integer = 0
        Result = Me.Solicita_id_gabinete_workflow(nombre_gabinete_wf,
                                                  id_gabinete)
        If Result <> "YES" Then
            Registra_flujo_documento = Result
            Exit Function
        End If
        Dim fecha_ini As String = ""
        Dim DateCreate As Date = Now
        Result = ""
        Dim Refclas_gestion_fecha As New ClassGestionFechas
        '-----------------------------
        'Formatea framework actual
        '-----------------------------
        Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                     fecha_ini)
        If Result <> "YES" Then
            Registra_flujo_documento = Result
            Exit Function
        End If
        Dim sqlresultinsert As Object
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Sql_Insercion As String = "insert INICIO_TAREAS_WORKFLOW ( Rutas_Workflow_id_Ruta," &
              "Fecha_Ini_Workflow,Flag_sistema,id_dat_ext)" &
              " VALUES (" & id_ruta & ",'" &
              fecha_ini & "',1," & "0" & " )"
            myCommand.CommandText = Sql_Insercion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_flujo_documento = "Imposible registrar el inicio tarea en workflow  " & Sql_Insercion
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim last_insert = myCommand.LastInsertedId

            '--------------------------------------------------------
            'Constuye sql inserción para tabla workflow
            '--------------------------------------------------------
            Dim sqlinsert As String = ""
            Dim campos_insert As String = ""
            Dim valores_insert As String = ""
            For i As Integer = 0 To stru_campos_plantilla_ruta.Length - 1
                If i = 0 Then
                    campos_insert = "(" & stru_campos_plantilla_ruta(i).nombre_campo_ruta
                    valores_insert = "('" & stru_campos_plantilla_ruta(i).dato_campo_plantilla & "'"
                Else
                    campos_insert = campos_insert & "," & stru_campos_plantilla_ruta(i).nombre_campo_ruta
                    valores_insert = valores_insert & ",'" & stru_campos_plantilla_ruta(i).dato_campo_plantilla & "'"
                End If
            Next
            '--------------------------------------
            'Inserta los datos del flujo documental
            '--------------------------------------
            Dim Ref_id_imagen As Object
            If id_imagen <> 0 Then
                Ref_id_imagen = id_imagen
            Else
                Ref_id_imagen = "Null"
            End If
            campos_insert = campos_insert & ",INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_GABINETE,ID_IMAGEN,FLUJO_TRABAJO_WF,estado_modulo_radicado)"
            valores_insert = valores_insert & ",'" & last_insert & "','" & id_gabinete & "'," & Ref_id_imagen & "," & ID_FLUJO_TRABAJO & "," &
                estado_modulo_radicado & ")"
            sqlinsert = "insert into DAT_ADIC_TAR" & nombre_ruta & " " & campos_insert & " values " & valores_insert
            myCommand.CommandText = sqlinsert
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_flujo_documento = "Error registra_flujo_workflow " & sqlinsert
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------------------
            'Registra los datos de estados tarea workflow
            '----------------------------------------------
            Dim sqlinsert_dat As String = "INSERT INTO ESTADOS_TAREA_WORKFLOW (" &
                "Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta," &
                "Inicio_Tareas_Workflow_id_Tarea,Id_Actividad,FECHA_INICIO," &
                "ESTADO_PRIORIDAD,ESTADO_TAREA,Id_Usuario,ID_FLUJO_TRABAJO," &
                "ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO," &
                "ESTADO_RECUPERACION_FLUJO_TRABAJO,ESTADO_ACTIVIDA_MODULO_RAD) VALUES (" &
                id_ruta & "," &
                last_insert & "," &
                id_actividad & ",'" &
                fecha_ini & "'," &
                "0,0," & ref_id_usuario_workflow & "," & ID_FLUJO_TRABAJO & "," &
                ID_ACTIVIDAD_FLUJO_TRABAJO & "," & ID_USUARIO_WORKFLOW_FLUJO_TRABAJO &
                "," & ESTADO_RECUPERACION_FLUJO_TRABAJO & "," & estado_activida_modulo_rad & ")"
            myCommand.CommandText = sqlinsert_dat
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_flujo_documento = "csfc_r_f_w 03 Función csfc_registra_flujo_workflow Error de Conexión " & sqlinsert_dat
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            id_tarea_workflow = last_insert
            myTrans.Commit()
            myConnection.Close()
            Registra_flujo_documento = "YES"
            Exit Function
        Catch e As Exception
            Try

            Catch ex As MySqlException
                myTrans.Rollback()
                If Not myTrans.Connection Is Nothing Then
                    Registra_flujo_documento = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registra_flujo_documento = "Error General " & e.Message
            Exit Function
        End Try
    End Function

    Function Asigna_datos_estructura_plantilla_radicacion_ruta(ByVal nombre_plantilla As String,
                                                               ByVal id_plantilla_radicado As Integer,
                                                               ByVal consecutivo_radicado As String,
                                                               ByRef stru_campos_plantilla_ruta() As csfc_structure_relacion_campos_plantilla_ruta) As String
        '------------------------------------------------------
        'Solicita los radicados pendientes por flujo documental
        'y asigna los datos de los campos a insertar en la tabla
        'de listado de flujos workflow
        '------------------------------------------------------
        Try
            Dim select_plantilla As String = "Select "
            For z As Integer = 0 To stru_campos_plantilla_ruta.Length - 1
                If z = 0 Then
                    select_plantilla = select_plantilla & stru_campos_plantilla_ruta(z).nombre_campo_plantilla
                Else
                    select_plantilla = select_plantilla & "," & stru_campos_plantilla_ruta(z).nombre_campo_plantilla
                End If
            Next
            Dim sql_consulta_plantilla As String = select_plantilla & " from " & nombre_plantilla & " as pa  where Consecutivo_Rad='" & consecutivo_radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sql_consulta_plantilla, Datset)
            If result <> "YES" Then
                Asigna_datos_estructura_plantilla_radicacion_ruta = "Función  Asigna_datos_estructura_plantilla_radicacion_ruta dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Asigna_datos_estructura_plantilla_radicacion_ruta = "Imposible encontrar la relación de campos entre la plantilla de radicacion " & nombre_plantilla & " y la ruta general "
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    If Datset.Tables(0).Rows(0).IsNull(i) = False Then
                        stru_campos_plantilla_ruta(i).dato_campo_plantilla = Left(Datset.Tables(0).Rows(0).Item(i), stru_campos_plantilla_ruta(i).dimension_campo_ruta)
                        stru_campos_plantilla_ruta(i).dato_campo_plantilla = stru_campos_plantilla_ruta(i).dato_campo_plantilla.ToString.Replace("'", "")
                    Else
                        stru_campos_plantilla_ruta(i).dato_campo_plantilla = ""
                    End If
                Next
                Asigna_datos_estructura_plantilla_radicacion_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_datos_estructura_plantilla_radicacion_ruta = "Inconsistencia general función Asigna_datos_estructura_plantilla_radicacion_ruta " & ex.Message
        End Try

    End Function

    Function Solicita_tipo_tramite_radicado(ByVal nombre_plantilla_radicado As String,
                                            ByVal radicado As String,
                                            ByRef tipo_tramite As String) As String
        '---------------------------------------------------------------
        'Función : Solicita el tipo de tramite de un radicado espefico
        'en una plantilla especifica de radicación
        'Fecha : 2017-12-04
        'Ing. Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select Descripcion_Documento from " & nombre_plantilla_radicado & " where Consecutivo_Rad='" & radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_plantilla_radicado)
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_tipo_tramite_radicado = "Función  Solicita_tipo_tramite_radicado dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipo_tramite_radicado = "Imposible encontrar el tipo de tramite del radicado  " & radicado & " en la plantilla " & nombre_plantilla_radicado
                Exit Function
            Else
                tipo_tramite = Datset.Tables(0).Rows(0).Item(0)
                Solicita_tipo_tramite_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipo_tramite_radicado = "Inconsistencia general función Solicita_tipo_tramite_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_gabinete_workflow(ByVal tipo_tramite As String,
                                               ByVal id_plantilla As Integer,
                                               ByRef nombre_gabinete_workflow As String) As String
        '---------------------------------------------------------------
        'Función: Solicita el nombre del gabinete workflow del tipo tramite 
        'Fehca : 2017-12-04
        'Ing: Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select nombre_gabinete_workflow from tipo_doc_entrante where Descripcion_Doc='" & tipo_tramite & "' and " &
                " system_plantilla_radicado_id_plantilla=" & id_plantilla
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_nombre_gabinete_workflow = "Función  Solicita_id_gabinete_tramite dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_gabinete_workflow = "Imposible encontrar la descripcion del tramite  " & tipo_tramite & " y sus parametros de configuración "
                Exit Function
            Else
                nombre_gabinete_workflow = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_gabinete_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_gabinete_workflow = "Inconsistencia general función Solicita_id_gabinete_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_id_gabinete_configuracion_gabinete(ByVal nombre_gabinete As String, ByRef id_gabinete As Integer) As String
        '--------------------------------------------------------
        'Fucción : Solicita la identificación del gabinete
        'Fecha : 2017-12-04
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select id_gabinete from system1 where NOMBRE='" & nombre_gabinete & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_id_gabinete_configuracion_gabinete = "Función  Solicita_id_gabinete_configuracion_gabinete dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_gabinete_configuracion_gabinete = "Imposible encontrar la identificacion del gabinete  " & nombre_gabinete & " en la tabla de configuración "
                Exit Function
            Else
                id_gabinete = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_gabinete_configuracion_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_gabinete_configuracion_gabinete = "Inconsistencia general función Solicita_id_gabinete_configuracion_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_id_gabinete_workflow(ByVal nombre_gabinete As String, ByRef id_gabinete As Integer) As String
        '--------------------------------------------------------
        'Fucción : Solicita la identificación del gabinete
        'Fecha : 2017-12-04
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select id_Gabinete from configuracion_gabinete where Nombre_Gabinete='" & nombre_gabinete & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_id_gabinete_workflow = "Función  Solicita_id_gabinete_workflow dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_gabinete_workflow = "Imposible encontrar la identificacion del gabinete  " & nombre_gabinete & " en la tabla de configuración "
                Exit Function
            Else
                id_gabinete = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_gabinete_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_gabinete_workflow = "Inconsistencia general función Solicita_id_gabinete_workflow " & ex.Message
        End Try
    End Function
    Public Shared Function csfc_verifica_existencia_radicado(ByVal Consecutivo_radicado As String,
                                                             ByVal nombre_ruta As String,
                                                             ByRef Estado_existencia_radicado As String,
                                                             ByRef id_tarea_workflow As Long) As String
        '--------------------------------------------------------------------
        'Funcion : Verifica existencia del radicado en la tabla de workflow
        'con el número de radicado 
        'Fecha : 2017-08-24
        'Ingeniero : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select RADICADO,INICIO_TAREAS_WORKFLOW_ID_TAREA FROM dat_adic_tar" & nombre_ruta & " Where RADICADO='" & Consecutivo_radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If result <> "YES" Then
                csfc_verifica_existencia_radicado = "Función  csfc_verifica_existencia_radicado dice " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Estado_existencia_radicado = "NO"
                id_tarea_workflow = 0
                csfc_verifica_existencia_radicado = "YES"
                Exit Function
            Else
                id_tarea_workflow = Datset.Tables(0).Rows(0).Item(1)
                Estado_existencia_radicado = "YES"
                csfc_verifica_existencia_radicado = "YES"
                Exit Function
            End If

        Catch ex As Exception
            csfc_verifica_existencia_radicado = "Inconsistencia general función csfc_verifica_existencia_radicado " & ex.Message
        End Try
    End Function
    Function After_envio_usuario_workflow(ByVal tipo_envio_usuario As Integer,
                                          ByVal estado_envio_correo As Integer,
                                          ByVal id_usuario_wokflow_destino As Integer,
                                          ByVal id_actividad_destino As Integer,
                                          ByVal id_tarea_seleccion As Long,
                                          ByRef page As Page,
                                          ByRef resul_correo As String,
                                          ByRef Resultado_evalua_terminar As String) As String
        Try
            Dim Result As String = ""
            Dim TreeViewseleccion As TreeView = page.FindControl("TreeViewseleccion")
            If tipo_envio_usuario = 2 Then
                Dim refclas As New ClassWorkflow
                Result = refclas.Terminar_Tarea_Workflow(id_usuario_wokflow_destino.ToString,
                                                         id_actividad_destino.ToString,
                                                         id_tarea_seleccion,
                                                         "",
                                                         page,
                                                         Resultado_evalua_terminar,
                                                         0,
                                                         resul_correo,
                                                         0,
                                                         0,
                                                         0,
                                                         estado_envio_correo)
                If Result <> "YES" Then
                    After_envio_usuario_workflow = Result
                    Exit Function
                Else
                    After_envio_usuario_workflow = "YES"
                    Exit Function
                End If
            End If
            If tipo_envio_usuario = 1 Then
                '-----------------------------------------------
                'Verifica estado solicitudes de aprobación sin
                'desición
                '-----------------------------------------------
                Dim Estado_solicitud_aprobacion As String = ""
                Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
                Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(id_tarea_seleccion,
                                                                                             Estado_solicitud_aprobacion,
                                                                                             HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    After_envio_usuario_workflow = Result
                    Exit Function
                End If
                If Estado_solicitud_aprobacion = "YES" Then
                    After_envio_usuario_workflow = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                    Exit Function
                End If
                Dim refclas_gestino_resp As New Classgestionrespuesta
                Result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                     id_tarea_seleccion,
                                                                                     id_usuario_wokflow_destino,
                                                                                     id_actividad_destino,
                                                                                     id_usuario_wokflow_destino,
                                                                                     TreeViewseleccion,
                                                                                     resul_correo,
                                                                                     "sin autorizacion usuario permitido",
                                                                                     0,
                                                                                     page,
                                                                                     0,
                                                                                     0,
                                                                                     0,
                                                                                     Resultado_evalua_terminar)
                If Result <> "YES" Then
                    After_envio_usuario_workflow = Result
                    Exit Function
                Else
                    After_envio_usuario_workflow = "YES"
                    Exit Function
                End If
            End If
            After_envio_usuario_workflow = "YES"
        Catch ex As Exception
            After_envio_usuario_workflow = "inconsitencia general función After_envio_usuario_workflow " & ex.Message
        End Try
    End Function

    Function Validar_enviar_actividad_por_conector_flujo_o_ruta(ByVal page As Page) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Activa enviar una tarea por flujo o por ruta de trabajo
        'tarea
        '-----------
        'Parametros:
        '-----------
        'page : referencia a los formularios. 
        '
        '-----------
        'Retorno   
        '----------
        'Lista actividades para envio de tareas por ruta o por flujo de trabajo
        '----------
        'Fecha     : 2023-04-26
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "Debe seleccionar la tarea para listar las lineas de envio"
                Exit Function
            End If
            Dim Hidden_lista_ruta_flujo As HtmlInputHidden = page.FindControl("Hidden_lista_ruta_flujo")
            If Hidden_lista_ruta_flujo Is Nothing Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "Imposible encontrar el control (Hidden_lista_ruta_flujo) "
                Exit Function
            End If
            Dim Hidden_id_actividad_flujo As HtmlInputHidden = page.FindControl("Hidden_id_actividad_flujo")
            If Hidden_id_actividad_flujo Is Nothing Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "Imposible encontrar el control (Hidden_id_actividad_flujo) "
                Exit Function
            End If
            Dim Hidden_activa_popup As HtmlInputHidden = page.FindControl("Hidden_activa_popup")
            If Hidden_activa_popup Is Nothing Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "Imposible encontrar el control (Hidden_activa_popup) "
                Exit Function
            End If
            Dim GridView_envia_flujo As GridView = page.FindControl("GridView_envia_flujo")
            If GridView_envia_flujo Is Nothing Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "Imposible encontrar el control (GridView_envia_flujo) "
                Exit Function
            End If
            Dim titulo_label_grid As Label = page.FindControl("titulo_label_grid")
            If titulo_label_grid Is Nothing Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "Imposible encontrar el control (titulo_label_grid) "
                Exit Function
            End If
            Dim Label_nombre_flujo As Label = page.FindControl("Label_nombre_flujo")
            If Label_nombre_flujo Is Nothing Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "Imposible encontrar el control (Label_nombre_flujo) "
                Exit Function
            End If
            Dim UpdateGeneral_documentos As UpdatePanel = page.FindControl("UpdateGeneral_documentos")
            If UpdateGeneral_documentos Is Nothing Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "Imposible encontrar el control (UpdateGeneral_documentos) "
                Exit Function
            End If
            Dim ModalPopupExtender_edition_lista_actividades_worflow_ruta As AjaxControlToolkit.ModalPopupExtender =
                page.FindControl("ModalPopupExtender_edition_lista_actividades_worflow_ruta")
            If ModalPopupExtender_edition_lista_actividades_worflow_ruta Is Nothing Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "Imposible encontrar el control (ModalPopupExtender_edition_lista_actividades_worflow_ruta) "
                Exit Function
            End If
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Dim Radicado As String = ""
            Dim Id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_actividad_flujo_trabajo As Integer = 0
            Hidden_lista_ruta_flujo.Value = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    Radicado)
            If Result <> "YES" Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = Result
                Exit Function
            End If
            Result = Refclas_f.Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(Radicado,
                                                                                                                id_actividad_flujo_trabajo,
                                                                                                                Id_flujo_trabajo,
                                                                                                                id_usuario_workflow_actividad_flujo_trabajo,
                                                                                                                HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                                                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = Result
                Exit Function
            End If
            Dim Nombre_flujo_trabajo As String = ""
            Dim estado_flujo As Integer = 1
            If Id_flujo_trabajo <> 0 Then
                HttpContext.Current.Session.Item("WF_ESTADO_FLUJO_RUTA") = "FLUJO"
                Refclas_f.SolicitaEstadoAbiertoCerradoFlujoDocumental(Id_flujo_trabajo,
                                                                           estado_flujo)
                If Result <> "YES" Then
                    Validar_enviar_actividad_por_conector_flujo_o_ruta = Result
                    Exit Function
                End If
                If estado_flujo = 0 Then
                    id_actividad_flujo_trabajo = 0
                End If
                Result = Refclas_f.SolicitaNombreFlujoTrabajoPorIdFlujo(Id_flujo_trabajo,
                                                                             Nombre_flujo_trabajo)
                If Result <> "YES" Then
                    Validar_enviar_actividad_por_conector_flujo_o_ruta = Result
                    Exit Function
                End If
                If id_actividad_flujo_trabajo <> 0 Then
                    Result = Refclas_f.Solicita_listado_actividades_para_envio_tarea_a_flujo(Id_flujo_trabajo,
                                                                                             GridView_envia_flujo,
                                                                                             titulo_label_grid,
                                                                                             Label_nombre_flujo,
                                                                                             Hidden_id_actividad_flujo,
                                                                                             UpdateGeneral_documentos,
                                                                                             Nombre_flujo_trabajo,
                                                                                             id_actividad_flujo_trabajo,
                                                                                             1)
                    If Result <> "YES" Then
                        Validar_enviar_actividad_por_conector_flujo_o_ruta = Result
                        Exit Function
                    Else
                        ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                        Hidden_activa_popup.Value = "YES"
                        Hidden_lista_ruta_flujo.Value = "F"
                        Validar_enviar_actividad_por_conector_flujo_o_ruta = "YES"
                        Exit Function
                    End If
                Else
                    Result = Refclas_f.Solicita_listado_actividades_para_envio_tarea_a_flujo(Id_flujo_trabajo,
                                                                                             GridView_envia_flujo,
                                                                                             titulo_label_grid,
                                                                                             Label_nombre_flujo,
                                                                                             Hidden_id_actividad_flujo,
                                                                                             UpdateGeneral_documentos,
                                                                                             Nombre_flujo_trabajo,
                                                                                             id_actividad_flujo_trabajo,
                                                                                             1)
                    If Result <> "YES" Then
                        Validar_enviar_actividad_por_conector_flujo_o_ruta = Result
                        Exit Function
                    Else
                        ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                        Hidden_lista_ruta_flujo.Value = "F"
                        Validar_enviar_actividad_por_conector_flujo_o_ruta = "YES"
                        Exit Function
                    End If
                End If
            End If
            HttpContext.Current.Session.Item("WF_ESTADO_FLUJO_RUTA") = "RUTA"
            Dim ref_grupos_workflow As New Class_grupos_workflow
            Result = ref_grupos_workflow.Solicita_Listado_actividades_para_envio_de_tareas_a_ruta(HttpContext.Current.Session.Item("Id_Grupo_Workflow"),
                                                                                                  GridView_envia_flujo,
                                                                                                  titulo_label_grid,
                                                                                                  Label_nombre_flujo,
                                                                                                  Hidden_id_actividad_flujo,
                                                                                                  HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                  UpdateGeneral_documentos,
                                                                                                  1)
            If Result <> "YES" Then
                Validar_enviar_actividad_por_conector_flujo_o_ruta = Result
                Exit Function
            Else
                ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                Hidden_lista_ruta_flujo.Value = "R"
                Validar_enviar_actividad_por_conector_flujo_o_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Validar_enviar_actividad_por_conector_flujo_o_ruta = "Inconsistencia general función Validar_enviar_actividad_por_conector_flujo_o_ruta " & ex.Message
        End Try
    End Function
    Function Activa_devolver_actividades_anteriores(ByVal id_tarea_workflow As Long,
                                                    ByRef page As Page) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Activa la opcion de listas las atvidades anteriores de un flujo de trabajo o una ruta para la devolución de una
        'tarea
        '-----------
        'Parametros:
        '-----------
        'id_tarea_workflow : identificación de la tarea workflow. 
        '
        '-----------
        'Retorno   
        '----------
        '
        '
        'stru_estado - Retorna los datos de la estructura de registro de tarea asignada
        '----------
        'Fecha     : 2023-04-26
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Hidden_lista_ruta_flujo As HtmlInputHidden = page.FindControl("Hidden_lista_ruta_flujo")
            If Hidden_lista_ruta_flujo Is Nothing Then
                Activa_devolver_actividades_anteriores = "Imposible encontrar el control (Hidden_lista_ruta_flujo) "
                Exit Function
            End If
            Dim Hidden_id_actividad_flujo As HtmlInputHidden = page.FindControl("Hidden_id_actividad_flujo")
            If Hidden_id_actividad_flujo Is Nothing Then
                Activa_devolver_actividades_anteriores = "Imposible encontrar el control (Hidden_id_actividad_flujo) "
                Exit Function
            End If
            Dim Hidden_activa_popup As HtmlInputHidden = page.FindControl("Hidden_activa_popup")
            If Hidden_activa_popup Is Nothing Then
                Activa_devolver_actividades_anteriores = "Imposible encontrar el control (Hidden_activa_popup) "
                Exit Function
            End If
            Dim GridView_envia_flujo As GridView = page.FindControl("GridView_envia_flujo")
            If GridView_envia_flujo Is Nothing Then
                Activa_devolver_actividades_anteriores = "Imposible encontrar el control (GridView_envia_flujo) "
                Exit Function
            End If
            Dim titulo_label_grid As Label = page.FindControl("titulo_label_grid")
            If titulo_label_grid Is Nothing Then
                Activa_devolver_actividades_anteriores = "Imposible encontrar el control (titulo_label_grid) "
                Exit Function
            End If
            Dim Label_nombre_flujo As Label = page.FindControl("Label_nombre_flujo")
            If Label_nombre_flujo Is Nothing Then
                Activa_devolver_actividades_anteriores = "Imposible encontrar el control (Label_nombre_flujo) "
                Exit Function
            End If
            Dim UpdateGeneral_documentos As UpdatePanel = page.FindControl("UpdateGeneral_documentos")
            If UpdateGeneral_documentos Is Nothing Then
                Activa_devolver_actividades_anteriores = "Imposible encontrar el control (UpdateGeneral_documentos) "
                Exit Function
            End If
            Dim ModalPopupExtender_edition_lista_actividades_worflow_ruta As AjaxControlToolkit.ModalPopupExtender =
                page.FindControl("ModalPopupExtender_edition_lista_actividades_worflow_ruta")
            If ModalPopupExtender_edition_lista_actividades_worflow_ruta Is Nothing Then
                Activa_devolver_actividades_anteriores = "Imposible encontrar el control (ModalPopupExtender_edition_lista_actividades_worflow_ruta) "
                Exit Function
            End If
            Dim UpdatePanel_tool_menu As UpdatePanel = page.FindControl("UpdatePanel_tool_menu")
            If UpdatePanel_tool_menu Is Nothing Then
                Activa_devolver_actividades_anteriores = "Imposible encontrar el control (UpdatePanel_tool_menu) "
                Exit Function
            End If
            Dim Result As String = ""
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim stru_estado As stru_estado = Nothing
            If HttpContext.Current.Session.Item("DEVOLVER_TAREA_WORKFLOW") = 0 Then
                Activa_devolver_actividades_anteriores = "El usuario no tiene permisos para devolver la tarea"
                Exit Function
            End If
            Result = Class_estados_tarea_workflow.Solicita_estructura_tarea_asignada(id_tarea_workflow,
                                                                                     stru_estado)
            If Result <> "YES" Then
                Activa_devolver_actividades_anteriores = Result
                Exit Function
            End If
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim Class_flujo_trabajo As New Class_flujo_trabajo_workflow
            Dim nombre_flujo_trabajo As String = ""
            Dim struregistro_actividaes_flujos_trabajo() As struregistro_actividaes_flujos_trabajo = Nothing
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Dim Class_actividades_disponibles_envio As New Class_actividades_disponibles_envio
            Dim nombre_ruta_workflow As String = ""
            If stru_estado.ID_FLUJO_TRABAJO <> 0 Then
                Result = Class_flujo_trabajo.SolicitaNombreFlujoTrabajoPorIdFlujo(stru_estado.ID_FLUJO_TRABAJO,
                                                                                           nombre_flujo_trabajo)
                If Result <> "YES" Then
                    Activa_devolver_actividades_anteriores = Result
                    Exit Function
                End If
                HttpContext.Current.Session.Item("WF_ESTADO_FLUJO_RUTA") = "FLUJO"
                Result = Class_flujo_trabajo.Solicita_lista_actividade_anteriores_flujo_trabajo(stru_estado.ID_FLUJO_TRABAJO,
                                                                                                    GridView_envia_flujo,
                                                                                                    titulo_label_grid,
                                                                                                    Label_nombre_flujo,
                                                                                                    Hidden_id_actividad_flujo,
                                                                                                    UpdateGeneral_documentos,
                                                                                                    nombre_flujo_trabajo,
                                                                                                    stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO,
                                                                                                    1)
                If Result <> "YES" Then
                    Activa_devolver_actividades_anteriores = Result
                    Exit Function
                Else
                    ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                    UpdatePanel_tool_menu.Update()
                    Hidden_activa_popup.Value = "YES"
                    Hidden_lista_ruta_flujo.Value = "F"
                    Activa_devolver_actividades_anteriores = "YES"
                    Exit Function
                End If
            End If
            If stru_estado.ID_FLUJO_TRABAJO = 0 Then
                Result = Class_worflow_rutas.Retorna_nombre_ruta_por_id_ruta(stru_estado.id_Ruta,
                                                                                 nombre_ruta_workflow)
                If Result <> "YES" Then
                    Activa_devolver_actividades_anteriores = Result
                    Exit Function
                End If
                HttpContext.Current.Session.Item("WF_ESTADO_FLUJO_RUTA") = "RUTA"
                Result = Class_actividades_disponibles_envio.Solicita_Listado_actividades_anteriores_ruta(stru_estado.Id_Actividad,
                                                                                                              GridView_envia_flujo,
                                                                                                              titulo_label_grid,
                                                                                                              Label_nombre_flujo,
                                                                                                              Hidden_id_actividad_flujo,
                                                                                                              nombre_ruta_workflow,
                                                                                                              UpdateGeneral_documentos,
                                                                                                              1)
                If Result <> "YES" Then
                    Activa_devolver_actividades_anteriores = Result
                    Exit Function
                Else
                    ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                    UpdatePanel_tool_menu.Update()
                    Hidden_activa_popup.Value = "YES"
                    Hidden_lista_ruta_flujo.Value = "R"
                    Activa_devolver_actividades_anteriores = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Activa_devolver_actividades_anteriores = "Inconsistencia general funcion Activa_devolver_actividades_anteriores " & ex.Message
        End Try
    End Function
    Function Devolver_tarea_workflow_usuario_anterior(ByVal id_tarea_workflow As Long,
                                                      ByVal id_actividad_workflow As Integer,
                                                      ByVal id_usuario_workflow As Integer,
                                                      ByRef page As Page,
                                                      ByRef Resultado_evalua_terminar As String,
                                                      ByRef lista_actividades As Integer) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Devuelve la tarea al ante penultimo usuario que entervino en la tarea
        '
        '-----------
        'Parametros:
        '-----------
        'id_tarea_workflow - identificación de la tarea workflow, id_actividad_workflow - identifica la tarea workflow del
        'usuario que devuelve la tarea
        '-----------
        'Retorno   ;
        '----------
        'estado_registro_asignacion, valores (NO) si tiene advertencia y no devuelve la tarea
        '(YES) si devuelve la tarea
        'stru_estado - Retorna los datos de la estructura de registro anterior de la tarea asignada
        '----------
        'Fecha     : 2022-09-10
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Hidden_lista_ruta_flujo As HtmlInputHidden = page.FindControl("Hidden_lista_ruta_flujo")
            If Hidden_lista_ruta_flujo Is Nothing Then
                Devolver_tarea_workflow_usuario_anterior = "Imposible encontrar el control (Hidden_lista_ruta_flujo) "
                Exit Function
            End If
            Dim Hidden_id_actividad_flujo As HtmlInputHidden = page.FindControl("Hidden_id_actividad_flujo")
            If Hidden_id_actividad_flujo Is Nothing Then
                Devolver_tarea_workflow_usuario_anterior = "Imposible encontrar el control (Hidden_id_actividad_flujo) "
                Exit Function
            End If
            Dim Hidden_activa_popup As HtmlInputHidden = page.FindControl("Hidden_activa_popup")
            If Hidden_activa_popup Is Nothing Then
                Devolver_tarea_workflow_usuario_anterior = "Imposible encontrar el control (Hidden_activa_popup) "
                Exit Function
            End If
            Dim GridView_envia_flujo As GridView = page.FindControl("GridView_envia_flujo")
            If GridView_envia_flujo Is Nothing Then
                Devolver_tarea_workflow_usuario_anterior = "Imposible encontrar el control (GridView_envia_flujo) "
                Exit Function
            End If
            Dim titulo_label_grid As Label = page.FindControl("titulo_label_grid")
            If titulo_label_grid Is Nothing Then
                Devolver_tarea_workflow_usuario_anterior = "Imposible encontrar el control (titulo_label_grid) "
                Exit Function
            End If
            Dim Label_nombre_flujo As Label = page.FindControl("Label_nombre_flujo")
            If Label_nombre_flujo Is Nothing Then
                Devolver_tarea_workflow_usuario_anterior = "Imposible encontrar el control (Label_nombre_flujo) "
                Exit Function
            End If
            Dim UpdateGeneral_documentos As UpdatePanel = page.FindControl("UpdateGeneral_documentos")
            If UpdateGeneral_documentos Is Nothing Then
                Devolver_tarea_workflow_usuario_anterior = "Imposible encontrar el control (UpdateGeneral_documentos) "
                Exit Function
            End If
            Dim ModalPopupExtender_edition_lista_actividades_worflow_ruta As AjaxControlToolkit.ModalPopupExtender =
                page.FindControl("ModalPopupExtender_edition_lista_actividades_worflow_ruta")
            If ModalPopupExtender_edition_lista_actividades_worflow_ruta Is Nothing Then
                Devolver_tarea_workflow_usuario_anterior = "Imposible encontrar el control (ModalPopupExtender_edition_lista_actividades_worflow_ruta) "
                Exit Function
            End If
            Dim UpdatePanel_tool_menu As UpdatePanel = page.FindControl("UpdatePanel_tool_menu")
            If UpdatePanel_tool_menu Is Nothing Then
                Devolver_tarea_workflow_usuario_anterior = "Imposible encontrar el control (UpdatePanel_tool_menu) "
                Exit Function
            End If
            Dim Result As String = ""
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim stru_estado As stru_estado = Nothing
            If HttpContext.Current.Session.Item("DEVOLVER_TAREA_WORKFLOW") = 0 Then
                Devolver_tarea_workflow_usuario_anterior = "El usuario no tiene permisos para devolver la tarea"
                Exit Function
            End If
            Result = Class_estados_tarea_workflow.Solicita_datos_tarea_usuario_anterior_a_devolver(id_tarea_workflow,
                                                                                                   1,
                                                                                                   stru_estado)
            If Result <> "YES" And Result <> "NA" Then
                Devolver_tarea_workflow_usuario_anterior = Result
                Exit Function
            End If
            '-------------------------------------------------------
            'Caso sin actividad anterior
            '-------------------------------------------------------
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim Class_flujo_trabajo As New Class_flujo_trabajo_workflow
            Dim nombre_flujo_trabajo As String = ""
            Dim struregistro_actividaes_flujos_trabajo() As struregistro_actividaes_flujos_trabajo = Nothing
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Dim Class_actividades_disponibles_envio As New Class_actividades_disponibles_envio
            Dim nombre_ruta_workflow As String = ""
            If Result = "NA" Then
                If stru_estado.ID_FLUJO_TRABAJO <> 0 Then
                    Result = Class_flujo_trabajo.SolicitaNombreFlujoTrabajoPorIdFlujo(stru_estado.ID_FLUJO_TRABAJO,
                                                                                           nombre_flujo_trabajo)
                    If Result <> "YES" Then
                        Devolver_tarea_workflow_usuario_anterior = Result
                        Exit Function
                    End If
                    HttpContext.Current.Session.Item("WF_ESTADO_FLUJO_RUTA") = "FLUJO"
                    Result = Class_flujo_trabajo.Solicita_lista_actividade_anteriores_flujo_trabajo(stru_estado.ID_FLUJO_TRABAJO,
                                                                                                    GridView_envia_flujo,
                                                                                                    titulo_label_grid,
                                                                                                    Label_nombre_flujo,
                                                                                                    Hidden_id_actividad_flujo,
                                                                                                    UpdateGeneral_documentos,
                                                                                                    nombre_flujo_trabajo,
                                                                                                    stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO,
                                                                                                    1)
                    If Result <> "YES" Then
                        Devolver_tarea_workflow_usuario_anterior = Result
                        Exit Function
                    Else
                        ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                        UpdatePanel_tool_menu.Update()
                        lista_actividades = 1
                        Hidden_activa_popup.Value = "YES"
                        Hidden_lista_ruta_flujo.Value = "F"
                        Devolver_tarea_workflow_usuario_anterior = "YES"
                        Exit Function
                    End If
                End If
                If stru_estado.ID_FLUJO_TRABAJO = 0 Then
                    Result = Class_worflow_rutas.Retorna_nombre_ruta_por_id_ruta(stru_estado.id_Ruta,
                                                                                 nombre_ruta_workflow)
                    If Result <> "YES" Then
                        Devolver_tarea_workflow_usuario_anterior = Result
                        Exit Function
                    End If
                    HttpContext.Current.Session.Item("WF_ESTADO_FLUJO_RUTA") = "RUTA"
                    Result = Class_actividades_disponibles_envio.Solicita_Listado_actividades_anteriores_ruta(stru_estado.Id_Actividad,
                                                                                                              GridView_envia_flujo,
                                                                                                              titulo_label_grid,
                                                                                                              Label_nombre_flujo,
                                                                                                              Hidden_id_actividad_flujo,
                                                                                                              nombre_ruta_workflow,
                                                                                                              UpdateGeneral_documentos,
                                                                                                              1)
                    If Result <> "YES" Then
                        Devolver_tarea_workflow_usuario_anterior = Result
                        Exit Function
                    Else
                        ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                        UpdatePanel_tool_menu.Update()
                        lista_actividades = 1
                        Hidden_activa_popup.Value = "YES"
                        Hidden_lista_ruta_flujo.Value = "R"
                        Devolver_tarea_workflow_usuario_anterior = "YES"
                        Exit Function
                    End If
                End If
            End If
            If stru_estado.Id_Usuario = id_usuario_workflow And stru_estado.Id_Actividad = id_actividad_workflow Then
                Devolver_tarea_workflow_usuario_anterior = "El usuario no puede devolver la tarea así mismo"
                Exit Function
            End If
            If stru_estado.ID_FLUJO_TRABAJO <> 0 And stru_estado.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = 0 Then
                stru_estado.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = stru_estado.Id_Usuario
            End If
            Result = Me.Terminar_Tarea_Workflow(stru_estado.Id_Usuario,
                                                stru_estado.Id_Actividad,
                                                id_tarea_workflow,
                                                "",
                                                page,
                                                Resultado_evalua_terminar,
                                                1,
                                                "",
                                                stru_estado.ID_FLUJO_TRABAJO,
                                                stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO,
                                                stru_estado.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO,
                                                1,
                                                0,
                                                0,
                                                0)
            If Result <> "YES" Then
                Devolver_tarea_workflow_usuario_anterior = Result
                Exit Function
            End If
            lista_actividades = 0
            Devolver_tarea_workflow_usuario_anterior = "YES"
        Catch ex As Exception
            Devolver_tarea_workflow_usuario_anterior = "inconsitencia general funcion Devolver_tarea_workflow_usuario_anterior " & ex.Message
        End Try

    End Function
    Function Eval_tarea_default_workflow(ByRef resultado_esctipt As String) As String
        Try
            Dim Conection_conectro_C = "Persist Security Info=" _
                  & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                  & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                  & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                  & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString


            If HttpContext.Current.Session("DEFAULTSCRIPT") = "" Then
                Eval_tarea_default_workflow = "El evento DEFAULTSCRIPT-WEB  no esta disponible para su grupo"
                Exit Function
            End If
            Dim p = HttpContext.Current.Session("DEFAULTSCRIPT")
            Dim mParam() As Object = {Conection_conectro_C,
                                      HttpContext.Current.Session("Id_Usuario_Workflow").ToString,
                                      HttpContext.Current.Session("Id_Grupo_Workflow").ToString,
                                      HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD").ToString,
                                      HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA").ToString,
                                      HttpContext.Current.Session("Id_Ruta_Workflow").ToString}
            Dim Resultado1 As String = ""
            Dim refcla As New ClassEdtiScript
            Resultado1 = refcla.Compila_Evalua(resultado_esctipt,
                                               HttpContext.Current.Session("DEFAULTSCRIPT"),
                                               "DEFAULTSCRIPT",
                                               mParam)
            If Resultado1 <> "YES" Then
                Eval_tarea_default_workflow = "Error  Error Compilando Funcion DEFAULTSCRIPT-WEB " & Resultado1
                Exit Function
            Else
                Eval_tarea_default_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Eval_tarea_default_workflow = "Inconsistencia general funcion Eval_tarea_default_workflow : " & ex.Message
        End Try
    End Function
End Class
