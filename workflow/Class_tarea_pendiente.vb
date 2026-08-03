Public Class Class_tarea_pendiente
    Public Function Lista_Tareas_Pendiente(ByRef scripma As GridView) As String
        Try
            Dim ref_Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Dim Id_Activida_User As String = ""
            Dim Result As String = ""
            Dim matri_campos_lista() As String
            Erase matri_campos_lista
            Result = ref_Class_configuracion_listado_ruta.Lista_campos_ruta_documento(matri_campos_lista)
            If Result <> "YES" Then
                Lista_Tareas_Pendiente = "Error Consultando lista campos " & Result
                Exit Function
            End If
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Result = ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida_User, _
                                                                                      HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Result <> "YES" Or Id_Activida_User = "" Then
                Lista_Tareas_Pendiente = "Imposible Obtener Id actividad "
                Exit Function
            End If
            Dim nombre_ruta As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString, _
                                                            nombre_ruta)
            If Result <> "YES" Or nombre_ruta = "" Then
                Lista_Tareas_Pendiente = "Imposible Obtener ruta actividad "
                Exit Function
            End If
            Dim nombre_campo_radicado As String = ""
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                              nombre_campo_radicado)
            If Result <> "YES" Then
                Lista_Tareas_Pendiente = Result
                Exit Function
            End If
            Dim Sql_consulta = ""
            Dim campos_lista As String = " tp.Id_Pendiente,tp.Inicio_Tareas_Workflow_id_Tarea,tp.Datos_Pendiente AS DETALLEPENDIENTE,tp.Fecha_Pendiente AS FECHAPENDIENTE,dat." & nombre_campo_radicado
            If Not matri_campos_lista Is Nothing Then
                For i As Integer = 0 To matri_campos_lista.Length - 1
                    If matri_campos_lista(i) <> "FECHAVENCIMIENTO" And matri_campos_lista(i) <> nombre_campo_radicado Then
                        campos_lista = campos_lista & ",dat." & matri_campos_lista(i)
                    End If
                Next
            End If
            Sql_consulta = "SELECT " & campos_lista & ", tp.ESTADO_PENDIENTE FROM TAREA_PENDIENTE as tp " & _
            "inner join  dat_adic_tar" & nombre_ruta & " as dat on " & _
            " (dat.inicio_tareas_workflow_id_tarea=tp.inicio_tareas_workflow_id_tarea)" & _
           " WHERE INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" & _
            HttpContext.Current.Session("Id_Ruta_Workflow") & " AND ID_USUARIO=" & HttpContext.Current.Session("Id_Usuario_Workflow") & _
            " AND ID_ACTIVIDAD=" & Id_Activida_User & " AND ESTADOS_PENDIENTE=1 order by tp.Id_Pendiente desc"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_Tareas_Pendiente = "Error Consultando lista tareas " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset
                scripma.DataBind()
                Lista_Tareas_Pendiente = "YES"
                Exit Function
            Else
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    Dim tex As String = scripma.Rows(i).Cells(1).Text & "-" & scripma.Rows(i).Cells(2).Text
                    scripma.Rows(i).Attributes.Add("id", tex)
                Next
                Lista_Tareas_Pendiente = "YES"
                Exit Function
            End If
        Catch es As Exception
            Lista_Tareas_Pendiente = "Error General " + es.Message
        End Try
    End Function
    Function Lista_tareas_pendientes_workflow(ByVal id_ruta_workflow As Integer, _
                                              ByVal Id_Grupo_Workflow As Integer, _
                                              ByVal Id_Usuario_Workflow As Integer, _
                                              ByRef grediview As GridView, _
                                              ByVal nombre_ruta As String, _
                                              ByRef reflabel As Label, _
                                              ByRef hideselecion As Object, _
                                              ByRef update_gred As UpdatePanel, _
                                              ByRef update_title As UpdatePanel, _
                                              ByVal tipo_consulta As Integer, _
                                              ByVal valor_consulta As String, _
                                              ByRef colum_order_name As String, _
                                              ByRef order_colum As String, _
                                              ByRef Hidden_cantidad_registros As Object) As String
        Try
            Dim ref_Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Dim Id_Activida_User As String = ""
            Dim Result As String = ""
            Dim matri_campos_lista() As String
            Erase matri_campos_lista
            Result = ref_Class_configuracion_listado_ruta.Lista_campos_ruta_documento(matri_campos_lista)
            If Result <> "YES" Then
                Lista_tareas_pendientes_workflow = Result
                Exit Function
            End If
            Dim nombre_campo_radicado As String = ""
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(id_ruta_workflow,
                                                                              nombre_campo_radicado)
            If Result <> "YES" Then
                Lista_tareas_pendientes_workflow = Result
                Exit Function
            End If
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Result = ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida_User, _
                                                                                      Id_Grupo_Workflow)
            If Result <> "YES" Or Id_Activida_User = "" Then
                Lista_tareas_pendientes_workflow = "Imposible Obtener Id actividad "
                Exit Function
            End If
            HttpContext.Current.Session.Item("Id_actividad_Workflow") = Id_Activida_User
            Dim Sql_consulta = ""
            Dim campos_lista As String = " tp.Id_Pendiente,tp.Inicio_Tareas_Workflow_id_Tarea,tp.Datos_Pendiente AS DETALLEPENDIENTE,tp.Fecha_Pendiente AS FECHAPENDIENTE,dat." & nombre_campo_radicado
            Dim campo_lista_dat As String = ""
            If Not matri_campos_lista Is Nothing Then
                For i As Integer = 0 To matri_campos_lista.Length - 1
                    If matri_campos_lista(i) <> "FECHAVENCIMIENTO" And matri_campos_lista(i) <> nombre_campo_radicado Then
                        campos_lista = campos_lista & ",dat." & matri_campos_lista(i)
                    End If
                    If i = 0 Then
                        campo_lista_dat = campo_lista_dat & "dat." & matri_campos_lista(i)
                    Else
                        campo_lista_dat = campo_lista_dat & ",dat." & matri_campos_lista(i)
                    End If
                Next
                campo_lista_dat = campo_lista_dat & ",tp.Datos_Pendiente"
                HttpContext.Current.Session.Item("WF_MTRI_CAMPOS_LISTA_TAREA_PENDIENTE_HI_WF") = campo_lista_dat
            End If
            If tipo_consulta = 1 Then
                Sql_consulta = "SELECT " & campos_lista & ", tp.ESTADO_PENDIENTE FROM TAREA_PENDIENTE as tp " & _
                "inner join  dat_adic_tar" & nombre_ruta & " as dat on " & _
                " (dat.inicio_tareas_workflow_id_tarea=tp.inicio_tareas_workflow_id_tarea)" & _
                " WHERE INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" & _
                id_ruta_workflow & " AND ID_USUARIO=" & Id_Usuario_Workflow & _
                " AND ID_ACTIVIDAD=" & Id_Activida_User & " AND ESTADOS_PENDIENTE=1 order by tp.Id_Pendiente desc"
            End If
            If tipo_consulta = 2 Then
                Dim sql_consulta_texto As String = ""
                If valor_consulta <> "" Then
                    Dim split_campos_lista() As String = campo_lista_dat.Split(",")
                    For i As Integer = 0 To split_campos_lista.Length - 1
                        If i = 0 Then
                            sql_consulta_texto = " (" & split_campos_lista(i) & " Like '%" & valor_consulta & "%'"
                        Else
                            sql_consulta_texto = sql_consulta_texto & " or " & split_campos_lista(i) & " Like '%" & valor_consulta & "%'"
                        End If
                    Next
                    sql_consulta_texto = sql_consulta_texto & ") "
                End If     
                Dim sql_filtro As String = ""
                If valor_consulta <> "" Then
                    sql_filtro = "(tp.Datos_Pendiente " & " Like '%" & valor_consulta & "%' ) and "
                End If
                Sql_consulta = "SELECT " & campos_lista & ", tp.ESTADO_PENDIENTE FROM  dat_adic_tar" & nombre_ruta & " as dat  " & _
                "inner join TAREA_PENDIENTE as tp on " & _
                " (dat.inicio_tareas_workflow_id_tarea=tp.inicio_tareas_workflow_id_tarea and  INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA=" & _
                id_ruta_workflow & " AND ID_USUARIO=" & Id_Usuario_Workflow & _
                " AND ID_ACTIVIDAD=" & Id_Activida_User & " AND ESTADOS_PENDIENTE=1) " & _
                " where " & sql_consulta_texto & " " & _
                " order by tp.Id_Pendiente desc"
            End If
            HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TAREA_PENDIENTE_HI_WF") = campos_lista
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("TAREA_PENDIENTE")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, _
                                                Datset)
            If Result <> "YES" Then
                Lista_tareas_pendientes_workflow = "Funcion Lista_tareas_pendientes_workflow dice (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                grediview.DataSource = Datset
                grediview.DataBind()
                update_gred.Update()
                update_title.Update()
                hideselecion.value = -1
                Hidden_cantidad_registros.value = Datset.Tables(0).Rows.Count
                reflabel.Text = "Tarea(s) pendiente(s) " & Datset.Tables(0).Rows.Count
                Lista_tareas_pendientes_workflow = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset
                grediview.DataBind()
                update_gred.Update()
                update_title.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    grediview.Rows(i).Attributes.Add("id_tarea", grediview.Rows(i).Cells(2).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fas fa-folder-open")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_tarea_pendiente(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_tarea", grediview.Rows(i).Cells(2).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "documentos_tarea_list")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-arrow-to-bottom")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_tarea_pendiente(event,this);")
                    ahtml.Attributes.Add("title", "Asignar tarea desde pediente")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_tarea", grediview.Rows(i).Cells(2).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "asig_tarea_pendiente")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
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
                Hidden_cantidad_registros.value = Datset.Tables(0).Rows.Count
                hideselecion.value = -1
                reflabel.Text = "Tarea(s) pendiente(s) " & Datset.Tables(0).Rows.Count
                Lista_tareas_pendientes_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_tareas_pendientes_workflow = "Inconsistencia general funcion Lista_tareas_pendientes_workflow " & ex.Message
        End Try
    End Function
    Function Sacar_Tarea_Pendiente(ByVal Id_Pendiente As String,
                                   ByVal Id_Tarea As String,
                                   ByRef Refetre As Object,
                                   ByRef page As Page) As String
        Try
            Dim Id_Activida_User As String = ""
            Dim Ref2 As New ClassListandoTareas
            Dim Estado_Prioridad As String = ""
            Dim LabelEspera As Label = page.FindControl("LabelEspera")
            Dim UpdatePanelnumeroespera As UpdatePanel = page.FindControl("UpdatePanelnumeroespera")
            '-----------------------------------------
            'consulta id actividad usuario
            '-----------------------------------------
            Dim Result As String = ""
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Result = ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida_User,
                                                                                      HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Result <> "YES" Or Id_Activida_User = "" Then
                Sacar_Tarea_Pendiente = "Imposible Obtener Id actividad "
                Exit Function
            End If
            '----------------------------------------
            'Consulta los datos de la tarea pendiente
            '----------------------------------------
            Dim Sql_consulta As String = "SELECT INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA," &
               "INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_USUARIO,ID_ACTIVIDAD FROM TAREA_PENDIENTE " &
               " WHERE ID_PENDIENTE=" & Id_Pendiente & " AND ESTADOS_PENDIENTE=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("TAREA_PENDIENTE")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Sacar_Tarea_Pendiente = "Error Consultando TAREAS_PENDIENTE " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Sacar_Tarea_Pendiente = "Imposible encotrar datos del estado pendiente (" & Id_Pendiente & ")"
                Exit Function
            End If
            Dim ID_RUTA As String = ""
            Dim ID_TAREAT As String = ""
            Dim ID_ACTIVIDAD As String = ""
            Dim ID_USUARIO As String = ""
            ID_RUTA = Datset.Tables(0).Rows(0).Item(0).ToString
            ID_TAREAT = Datset.Tables(0).Rows(0).Item(1).ToString
            ID_USUARIO = Datset.Tables(0).Rows(0).Item(2).ToString
            ID_ACTIVIDAD = Datset.Tables(0).Rows(0).Item(3).ToString
            '------------------------------------------------------------
            'Asigna la tarea al usuario
            '------------------------------------------------------------
            Result = ""
            Dim Ref1 As New Classselecciotarea
            Result = Ref1.Asigna_tarea(ID_TAREAT,
                                       0,
                                       ID_ACTIVIDAD,
                                       1,
                                       page,
                                       Id_Pendiente)
            If Result <> "YES" Then
                Sacar_Tarea_Pendiente = Result
                Exit Function
            Else
                If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 1 Then
                    Dim NumActi As Integer = HttpContext.Current.Session.Item("NUMEROACTIVIDADES")
                    NumActi += 1
                    HttpContext.Current.Session("NUMEROACTIVIDADES") = NumActi
                    LabelEspera.Text = "(" & HttpContext.Current.Session("NUMEROACTIVIDADES") & ")"
                    UpdatePanelnumeroespera.Update()
                End If
                Sacar_Tarea_Pendiente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Sacar_Tarea_Pendiente = "Inconsistencia general función Sacar_Tarea_Pendiente " & ex.Message
        End Try
    End Function
    Function Subir_Tarea_Pendiente(ByVal Datos_Ident As String,
                                   ByRef page As Page) As String
        Try
            Dim Id_Activida_User As String = ""
            Dim Ref2 As New ClassListandoTareas
            Dim Estado_Prioridad As String = ""
            Dim LabelEspera As Label = page.FindControl("LabelEspera")
            Dim UpdatePanelnumeroespera As UpdatePanel = page.FindControl("UpdatePanelnumeroespera")
            '-----------------------------------------
            'Consulta id actividad usuario
            '-----------------------------------------
            Dim Result As String = ""
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Dim ClassWorkflow As New ClassWorkflow
            Result = ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida_User,
                                                                                      HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Result <> "YES" Or Id_Activida_User = "" Then
                Subir_Tarea_Pendiente = "Imposible Obtener Id actividad "
                Exit Function
            End If
            '--------------------------------------------
            'Consulta el etado prioridad tarea workflow
            'ruta tarea
            '-------------------------------------------
            Result = ""
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Result = Class_estados_tarea_workflow.Obtener_Estado_Prioridad(HttpContext.Current.Session("ID_TAREA_SELECCIONDA"),
                                                                           Estado_Prioridad)
            If Result <> "YES" Then
                Subir_Tarea_Pendiente = "Error Consultando prioridad de tarea " + Result
                Exit Function
            End If
            If Estado_Prioridad = "" Then
                Subir_Tarea_Pendiente = "Imposible Terminar tarea no tiene prioridad asignada o otra seccion la cambio el estado de la tarea"
                Exit Function
            End If
            Dim Matri_Datos() As String
            'Matri_Datos(0) prioridad de tarea
            'Matri_Datos(1) ruta al que pertenece la tarea
            'Matri_Datos(2) Fecha seleccion usuario workflow
            Erase Matri_Datos
            Matri_Datos = Split(Estado_Prioridad, "|")
            If Matri_Datos Is Nothing Then
                Subir_Tarea_Pendiente = "Imposible Terminar tarea matri nula de datos "
                Exit Function
            End If
            Dim Refclas_gestion_fecha As New ClassGestionFechas
            Dim Fecha_Fromat As String = ""
            Dim DateCreate As Date = Now
            '-----------------------------
            'Formatea framework actual
            '-----------------------------
            Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(DateCreate,
                                                                         Fecha_Fromat)
            If Result <> "YES" Then
                Subir_Tarea_Pendiente = Result
                Exit Function
            End If
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    Radicado)
            If Result <> "YES" Then
                Subir_Tarea_Pendiente = Result
                Exit Function
            End If
            If Datos_Ident = "" Then
                Datos_Ident = Radicado
            End If
            Dim ref_class_ra_solicitudes As New ClassRaSolicitudesAprobacion
            Dim numero_aprobacion As Integer = 0
            If Radicado <> "" Then
                ref_class_ra_solicitudes.Retorna_numero_solicitudes_pendientes_por_aprobacion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                              Radicado,
                                                                                              numero_aprobacion)
            End If
            Dim estado_pediente As String = ""
            If numero_aprobacion <> 0 Then
                estado_pediente = "Solicitud de aprobación"
            End If
            If estado_pediente = "" Then
                estado_pediente = "Null"
            Else
                estado_pediente = "'" & estado_pediente & "'"
            End If
            Dim Parametro_Update As String = "UPDATE ESTADOS_TAREA_WORKFLOW " &
                     "SET ESTADO_TAREA=1" &
                     " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & HttpContext.Current.Session("ID_TAREA_SELECCIONDA") &
                     " AND ID_ACTIVIDAD=" & Id_Activida_User & " and FECHA_FIN IS NULL"
            Dim Parametro_Insert As String = ""
            Parametro_Insert = "INSERT INTO TAREA_PENDIENTE " &
            "(INICIO_TAREAS_WORKFLOW_RUTAS_WORKFLOW_ID_RUTA," &
            "INICIO_TAREAS_WORKFLOW_ID_TAREA,ID_USUARIO,FECHA_PENDIENTE" &
            ",DATOS_PENDIENTE,ESTADOS_PENDIENTE,ID_ACTIVIDAD,ESTADO_PENDIENTE) VALUES (" &
            Matri_Datos(1) & "," &
            HttpContext.Current.Session("ID_TAREA_SELECCIONDA") & "," &
            HttpContext.Current.Session("Id_Usuario_Workflow") & ",'" &
            Fecha_Fromat & "','" &
            Datos_Ident & "',1," &
            Id_Activida_User & "," & estado_pediente & ")"
            '---------------------------------------------------------------
            'Actualiza y cambia de actividad la tarea
            '---------------------------------------------------------------
            Result = ""
            Dim last_insert As Object = 0
            Result = ClassWorkflow.Cambia_Estado(Parametro_Insert,
                                                 Parametro_Update,
                                                 last_insert)
            If Result <> "YES" Then
                Subir_Tarea_Pendiente = Result
                Exit Function
            Else
                Dim kt = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
                Dim kx = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE")
                If Not HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") Is Nothing Then
                    If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 0 Then
                        For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows.Count - 1
                            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).Item(0) = HttpContext.Current.Session("ID_TAREA_SELECCIONDA") Then
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").Tables(0).Rows(i).Item("ESTADO") = 1
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF").AcceptChanges()
                                Exit For
                            End If
                        Next
                    End If
                End If
                If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                    If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 0 Then
                        For i As Integer = 0 To HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows.Count - 1
                            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).Item(0) = HttpContext.Current.Session("ID_TAREA_SELECCIONDA") Then
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").Tables(0).Rows(i).Item("ESTADO") = 1
                                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").AcceptChanges()
                                Exit For
                            End If
                        Next
                    End If
                End If
                If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 1 Then
                    Dim NumActi As Integer = HttpContext.Current.Session.Item("NUMEROACTIVIDADES")
                    NumActi -= 1
                    HttpContext.Current.Session("NUMEROACTIVIDADES") = NumActi
                    LabelEspera.Text = "(" & HttpContext.Current.Session("NUMEROACTIVIDADES") & ")"
                    UpdatePanelnumeroespera.Update()
                End If
                HttpContext.Current.Session("ID_TAREA_SELECCIONDA") = "0"
                Subir_Tarea_Pendiente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Subir_Tarea_Pendiente = "Inconsistencia general funcion Subir_Tarea_Pendiente " & ex.Message
        End Try
    End Function
    Function Solicita_numero_tareas_pendientes(ByVal id_usuario_workflow As Integer,
                                               ByRef numero_pendientes As Integer) As String
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = "SELECT ID_PENDIENTE  FROM TAREA_PENDIENTE" &
              " WHERE id_usuario=" & id_usuario_workflow & " AND ESTADOS_PENDIENTE=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("TAREA_PENDIENTE")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_tareas_pendientes = "Error funcion Solicita_numero_tareas_pendientes  " & Result
                Exit Function
            End If
            numero_pendientes = Datset.Tables(0).Rows.Count
            Solicita_numero_tareas_pendientes = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_numero_tareas_pendientes = "Inconsistencia general funcion Solicita_numero_tareas_pendientes " & ex.Message
        End Try
    End Function
    Function Solicita_estado_pendiente_tarea_workflow(ByVal Id_tareas As Long,
                                                      ByRef id_pendiente As Integer) As String
        Try
            Dim Sql_consulta As String = "SELECT  Id_Pendiente " &
            " FROM tarea_pendiente  " &
            "  where Inicio_Tareas_Workflow_id_Tarea=" & Id_tareas &
            " and ESTADOS_PENDIENTE=1 "
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("tarea_pendiente")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_pendiente_tarea_workflow = "Error funcion Solicita_estado_pendiente_tarea_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_pendiente = -1
                Solicita_estado_pendiente_tarea_workflow = "YES"
                Exit Function
            Else
                id_pendiente = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_pendiente_tarea_workflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_estado_pendiente_tarea_workflow = "Error general funcion Solicita_estado_pendiente_tarea_workflow " & ex.Message
        End Try
    End Function
End Class
