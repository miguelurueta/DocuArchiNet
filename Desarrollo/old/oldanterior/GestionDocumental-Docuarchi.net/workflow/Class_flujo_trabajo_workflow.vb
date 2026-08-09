Imports MySql.Data.MySqlClient
Imports System
Imports System.IO
Imports System.Drawing
Imports System.Web.Script.Serialization
Imports MindFusion.Diagramming
Imports MindFusion.Diagramming.WebForms
Imports MindFusion.Diagramming.Import.VisioImporter
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports Graphics = System.Drawing.Graphics
Imports GestionDocumental_Docuarchi.net.Class_config_general_service

Public Structure stru_flujo_trabajo
    Dim id_flujo_trabajo As Integer
    Dim nombre_flujo_trabajo As String

End Structure
Public Structure stru_ruta_workflow
    Dim id_ruta As Integer
    Dim nombre_ruta As String
End Structure
Public Structure stru_tramites
    Dim id_relacion As Integer
    Dim id_tramite As Integer
    Dim id_plantilla As Integer
    Dim nombre_tramite As String
End Structure
Public Structure stru_relacion
    Dim ID_RELACION_TRAMITE As Integer
    Dim system_plantilla_radicado_id_Plantilla As Integer
    Dim tipo_doc_entrante_id_Tipo_Doc_Entrante As Integer
    Dim ID_WF_FLUJO_TRABAJO As Integer
    Dim ESTADO_RELACION As Integer
End Structure
Public Structure stru_shape_actividad
    Dim id_estado As Long
    Dim nombre_actividad As String
    Dim nombre_tipo_actividad As String
    Dim cargo_usuario As String
    Dim estado_recuperacion As Integer
End Structure
Public Structure STRU_ACTIVIDADES_FLUJO_TRABAJO
    Dim ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO As Integer
    Dim wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO As Integer
    Dim listado_actividades_workflow_Id_Actividad As Integer
    Dim IDENTI_GRAFICA_ACTIVIDAD As Integer
    Dim FECHA_REGISTRO As String
    Dim ESTADO_ACTIVIDAD As Integer
    Dim ID_TIPO_ACTIVIDAD As Integer
    Dim ID_USUARIO_WORKFLOW As Object
    Dim TIPO_ABIERTA_CERRADA_ACTIVIDAD As Integer
    Dim ACTIVIDAD_INICIO As Integer
    Dim ACTIVIDAD_FINAL As Integer
    Dim ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO_ANTERIOR As Integer
    Dim STRU_CONECTOR() As STRU_REGISTRO_CONECTORES_FLUJOS_TRABAJO
End Structure
Public Structure STRU_REGISTRO_CONECTORES_FLUJOS_TRABAJO
    Dim ID_REGISTRO_ACTIVIDAD_ENVIO As Integer
    Dim ID_REGISTRO_ACTIVIDAD_ENVIO_ANTERIOR As Integer
    Dim wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO As Integer
    Dim ID_ACTIVIDAD_FUENTE As Integer
    Dim ID_ACTIVIDAD_DESTINO As Integer
    Dim IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE As Integer
    Dim IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO As Integer
    Dim ID_USUARIO_WORKFLOW_FUENTE As Object
    Dim ID_USUARIO_WORKFLOW_DESTINO As Object
End Structure
Public Class class_envio_flujo_trabajo
    Property Error_result As String
    Property Error_gestion As String
    Property Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)
    Property Resultado_send_correo As String
End Class
Public Class Class_flujo_trabajo_workflow
    Function SolicitaExistenciaUsuarioFlujoTarea(ByVal NombreRutaWorkflow As String,
                                                 ByVal IdTareaWorkflow As Long,
                                                 ByVal IdUsuarioWorkflow As Integer,
                                                 ByVal IdActividadUsuarioWorkflow As Integer,
                                                 ByRef IdActividadflujoTrabajo As Integer,
                                                 ByRef IdFlujoTrabajo As Integer,
                                                 ByRef IdUsuarioWorflowFlujoTarabajo As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asgina datos de validación des una plantilla de validación
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_script           : Representa la identificación del script de validación
        'campo_radicacion    : Representa el nombre del campo de radicación destino
        'id_plantilla_radicacion : 
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaFlujoTareaWorkflow(NombreRutaWorkflow,
                                                                   IdTareaWorkflow,
                                                                   IdFlujoTrabajo)
            If Result <> "YES" Then
                SolicitaExistenciaUsuarioFlujoTarea = Result
                Exit Function
            End If
            Dim Refclas_registro_flujo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim StruActividadUsuarioFlujo() As stru_actividad_usuario_flujo = Nothing
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim StruEstadosFlujoTarea() As stru_estados_flujo_tarea = Nothing
            Dim IStru As Integer = 0
            If IdFlujoTrabajo <> 0 Then
                Result = Refclas_registro_flujo.SolicitaIActividadesUsuarioWorkflowFlujoTrabajo(IdActividadUsuarioWorkflow,
                                                                                                IdUsuarioWorkflow,
                                                                                                IdFlujoTrabajo,
                                                                                                StruActividadUsuarioFlujo)
                If Result <> "YES" Then
                    SolicitaExistenciaUsuarioFlujoTarea = Result
                    Exit Function
                End If
                If StruActividadUsuarioFlujo Is Nothing Then
                    SolicitaExistenciaUsuarioFlujoTarea = "El usuario no está asignado a ninguna de las actividades del flujo de trabajo, por lo que no es posible recuperar la tarea correspondiente."
                End If
                For i As Integer = 0 To StruActividadUsuarioFlujo.Length - 1
                    Result = Class_estados_tarea_workflow.SolicitaEstadosFlujoDocumentalIdTareaUsuarioFlujo(IdTareaWorkflow,
                                                                                                            StruActividadUsuarioFlujo(i).id_actividad_workflow_flujo,
                                                                                                            IdFlujoTrabajo,
                                                                                                            StruEstadosFlujoTarea,
                                                                                                            IStru)
                    If Result <> "YES" Then
                        SolicitaExistenciaUsuarioFlujoTarea = Result
                        Exit Function
                    End If
                Next
                If Not StruEstadosFlujoTarea Is Nothing Then
                    IdActividadflujoTrabajo = StruEstadosFlujoTarea(UBound(StruEstadosFlujoTarea)).ID_ACTIVIDAD_FLUJO_TRABAJO
                    IdUsuarioWorflowFlujoTarabajo = StruEstadosFlujoTarea(UBound(StruEstadosFlujoTarea)).ID_USUARIO_WORKFLOW_FLUJO_TRABAJO
                Else
                    IdActividadflujoTrabajo = StruActividadUsuarioFlujo(LBound(StruActividadUsuarioFlujo)).id_actividad_workflow_flujo
                    IdUsuarioWorflowFlujoTarabajo = StruActividadUsuarioFlujo(LBound(StruActividadUsuarioFlujo)).id_usuario_worlflow_flujo
                End If
            End If
            SolicitaExistenciaUsuarioFlujoTarea = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaExistenciaUsuarioFlujoTarea = "Inconsistencia general funcion SolicitaExistenciaUsuarioFlujoTarea " & ex.Message
        End Try
    End Function
    Function Solicita_lista_flujo_trabajo_id_flujo(ByVal optio_blank As Integer,
                                                   ByVal id_flujo_trabajo As Integer,
                                                   ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asigna la estructura del flujo enviado como parametro
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        'nombre_tramite:Representa el nombre del tramite
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_service_ilist_drowlist  : Retorna la estructura generica de la lista de tramites
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-04-24
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim sql_consulta As String = "Select ID_WF_FLUJOS_TRABAJO,NOMBRE_FLUJO_TRABAJO " &
                " from wf_flujos_trabajo " &
                " where ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo &
                " order by NOMBRE_FLUJO_TRABAJO"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_flujo_trabajo_id_flujo = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_flujo_trabajo_id_flujo = "YES"
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                If optio_blank = 1 Then
                    Class_service_ilist_drowlist.Add(Item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.value_campo = Datset.Tables(0).Rows(i).Item(1)
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0)
                    Class_service_ilist_drowlist.Add(Item)
                Next
                Solicita_lista_flujo_trabajo_id_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_flujo_trabajo_id_flujo = "Inconistencia general función  Solicita_lista_flujo_trabajo_id_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_datos_rutas_workflow(ByRef stru_ruta() As stru_ruta_workflow) As String
        '---------------------------------------------------------
        'Función : Retorna la lista de rutas con los respectivos
        'nombre 
        'Fecha : 2017-09-21
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select id_Ruta,Nombre_Ruta AS AREA from rutas_workflow  " &
                " ORDER BY Nombre_Ruta"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("rutas_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_rutas_workflow = "Error Solicita_datos_rutas_workflow  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_rutas_workflow = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_ruta(i)
                    stru_ruta(i).id_ruta = Datset.Tables(0).Rows(i).Item(0)
                    stru_ruta(i).nombre_ruta = Datset.Tables(0).Rows(i).Item(1)
                Next
                Solicita_datos_rutas_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_rutas_workflow = "Inconsistencia general función Solicita_datos_rutas_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_flujos_trabajo_ruta(ByVal id_ruta As Integer,
                                          ByRef stru_flujo_trab() As stru_flujo_trabajo) As String
        '--------------------------------------------------------------
        'Función : Retorna la lista de flujos de trabajo relacionados
        'a la ruta informada
        'Fecha : 2017-09-21
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select ID_WF_FLUJOS_TRABAJO,NOMBRE_FLUJO_TRABAJO AS AREA from wf_flujos_trabajo  " &
            " WHERE rutas_workflow_id_Ruta=" & id_ruta & " and ESTADO_FLUJO=1 ORDER BY NOMBRE_FLUJO_TRABAJO"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_flujos_trabajo_ruta = "Error Solicita_flujo_trabajo_ruta  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_flujos_trabajo_ruta = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_flujo_trab(i)
                    stru_flujo_trab(i).id_flujo_trabajo = Datset.Tables(0).Rows(i).Item(0)
                    stru_flujo_trab(i).nombre_flujo_trabajo = Datset.Tables(0).Rows(i).Item(1)
                Next
                Solicita_flujos_trabajo_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_flujos_trabajo_ruta = "Inconsistencia general función " & ex.Message
        End Try
    End Function
    Function Solicita_tramites_relacionados_flujo_trabajo(ByVal id_flujo_trabajo As Integer,
                                                          ByRef stru_tramite() As stru_tramites) As String
        '--------------------------------------------------------------
        'Función : Retorna la lista los tramites relacionados al 
        'flujo de trabajo
        'Fecha : 2017-09-21- Modificado 04-09-2018
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select rft.ID_RELACION_TRAMITE,tde.Descripcion_Doc from ra_relacion_tramite_flujo_wokflow as rft  " &
            " inner join tipo_doc_entrante  as tde on (tde.id_Tipo_Doc_Entrante= rft.tipo_doc_entrante_id_Tipo_Doc_Entrante) " &
            " WHERE  ID_WF_FLUJO_TRABAJO=" & id_flujo_trabajo & " ORDER BY Descripcion_Doc "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_tramite_flujo_wokflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_tramites_relacionados_flujo_trabajo = "Error Solicita_tramites_relacionados_flujo_trabajo  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tramites_relacionados_flujo_trabajo = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_tramite(i)
                    stru_tramite(i).id_relacion = Datset.Tables(0).Rows(i).Item(0)
                    stru_tramite(i).nombre_tramite = Datset.Tables(0).Rows(i).Item(1)
                Next
                Solicita_tramites_relacionados_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tramites_relacionados_flujo_trabajo = "Inconsistencia general función Solicita_tramites_relacionados_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Lista_ruta_flujos_tramites_relacionados(ByRef ref_treview As TreeView, ByRef ref_update_panel As UpdatePanel) As String
        Try
            ref_treview.Nodes.Clear()
            Dim Result As String = ""
            Dim stru_ruta() As stru_ruta_workflow = Nothing
            Result = Me.Solicita_datos_rutas_workflow(stru_ruta)
            If Result <> "YES" Then
                Lista_ruta_flujos_tramites_relacionados = Result
                Exit Function
            End If
            If stru_ruta Is Nothing Then
                Lista_ruta_flujos_tramites_relacionados = "YES"
                Exit Function
            End If
            Dim tre_node_ruta_title As New TreeNode
            tre_node_ruta_title.Value = "RUTA_TITLE|0"
            tre_node_ruta_title.Text = "RUTAS FLUJOS DE FLUJO TRABAJO"
            Dim tre_node_flujo_title As New TreeNode
            tre_node_flujo_title.Value = "FU_TITLE|0"
            tre_node_flujo_title.Text = "FLUJOS DE TRABJOS RELACIONADOS A LA RUTA"
            Dim tre_node_ruta As TreeNode
            Dim tre_node_flujo As TreeNode
            Dim tre_node_tramite As TreeNode
            For i As Integer = 0 To stru_ruta.Length - 1
                tre_node_ruta = New TreeNode
                tre_node_ruta.Text = stru_ruta(i).nombre_ruta & " (Ruta de trabajo)"
                tre_node_ruta.Value = "RU|" & stru_ruta(i).id_ruta
                tre_node_ruta.ImageUrl = "../workflow/imageneswf/exchange-alt-light.png"
                Dim stru_flujo_trab() As stru_flujo_trabajo = Nothing
                Result = Me.Solicita_flujos_trabajo_ruta(stru_ruta(i).id_ruta,
                                                         stru_flujo_trab)
                If Result <> "YES" Then
                    Lista_ruta_flujos_tramites_relacionados = Result
                    Exit Function
                End If
                If Not stru_flujo_trab Is Nothing Then
                    For z As Integer = 0 To stru_flujo_trab.Length - 1
                        tre_node_flujo = New TreeNode
                        tre_node_flujo.Text = stru_flujo_trab(z).nombre_flujo_trabajo & " (Flujo de trabajo)"
                        tre_node_flujo.Value = "FU|" & stru_flujo_trab(z).id_flujo_trabajo
                        tre_node_flujo.ImageUrl = "../workflow/imageneswf/project-diagram-light.png"
                        Dim stru_tramite() As stru_tramites = Nothing
                        Result = Me.Solicita_tramites_relacionados_flujo_trabajo(stru_flujo_trab(z).id_flujo_trabajo, stru_tramite)
                        If Result <> "YES" Then
                            Lista_ruta_flujos_tramites_relacionados = Result
                            Exit Function
                        End If
                        If Not stru_tramite Is Nothing Then
                            For k As Integer = 0 To stru_tramite.Length - 1
                                tre_node_tramite = New TreeNode
                                tre_node_tramite.Value = "TRA|" & stru_tramite(k).id_relacion
                                tre_node_tramite.Text = stru_tramite(k).nombre_tramite & " (Trámite relacionado)"
                                tre_node_tramite.ImageUrl = "../workflow/imageneswf/id-card-light.png"
                                tre_node_flujo.ChildNodes.Add(tre_node_tramite)
                            Next
                        End If
                        tre_node_ruta.ChildNodes.Add(tre_node_flujo)

                    Next
                End If
                ref_treview.Nodes.Add(tre_node_ruta)
            Next
            Lista_ruta_flujos_tramites_relacionados = "YES"
        Catch ex As Exception
            Lista_ruta_flujos_tramites_relacionados = "Inconsistencia general función Lista_ruta_flujos_tramites_relacionados " & ex.Message
        Finally
            ref_update_panel.Update()
        End Try
    End Function
    Function Verifica_existencia_tramite_relacionado(ByVal id_tipo_tramite As Integer, ByRef existencia_relacion As String, ByRef id_flujo_trabajo As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select ID_WF_FLUJOS_TRABAJO from tipo_doc_entrante where id_Tipo_Doc_Entrante ='" & id_tipo_tramite & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_tramite_relacionado = "Error función Verifica_existencia_tramite_relacionado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    existencia_relacion = "NO"
                    id_flujo_trabajo = 0
                    Verifica_existencia_tramite_relacionado = "YES"
                    Exit Function
                Else
                    existencia_relacion = "YES"
                    id_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                    Verifica_existencia_tramite_relacionado = "YES"
                    Exit Function
                End If
            Else
                Verifica_existencia_tramite_relacionado = "Imposible encontrar el id del tipo trámite (" & id_tipo_tramite & ")"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_tramite_relacionado = "Inconsistencia general función Verifica_existencia_tramite_relacionado " & ex.Message
        End Try
    End Function
    Function Solicita_listado_actividades_flujo_trabajo(ByVal tipo_consulta As Integer,
                                                        ByVal valor_consulta As String,
                                                        ByRef colum_order_name As String,
                                                        ByRef order_colum As String,
                                                        ByRef grediview As GridView,
                                                        ByRef reflabel As Label,
                                                        ByRef hideselecion As Object,
                                                        ByRef update As UpdatePanel) As String
        Try
            Dim Sql_consulta As String = ""
            If tipo_consulta = 1 Then
                Sql_consulta = "Select law.Id_Actividad,law.Nombre_Actividad as ACTIVIDAD,law.Descripcion_Actividad as DESCRIPCION_ACTIVIDAD" &
                ",agw.Descripcion_Actividad as TIPO_ACTIVIDAD from listado_actividades_workflow as law " &
                " inner join actividades_generales_workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General)"
            Else
                Sql_consulta = "Select law.Id_Actividad,law.Nombre_Actividad as ACTIVIDAD,law.Descripcion_Actividad as DESCRIPCION_ACTIVIDAD" &
                ",agw.Descripcion_Actividad as TIPO_ACTIVIDAD from listado_actividades_workflow as law " &
                " inner join actividades_generales_workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General) " &
                " where law.Nombre_Actividad like '%" & valor_consulta & "%' or law.Descripcion_Actividad like '%" & valor_consulta & "%'"
            End If
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_actividades_flujo_trabajo = "Error listando descripción tabla listado_actividades_workflow  " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "0 registro (s) de actividades"
                grediview.DataSource = Nothing
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                Solicita_listado_actividades_flujo_trabajo = "YES"
                Exit Function
            Else
                reflabel.Text = Datset.Tables(0).Rows.Count & " registro (s) de actividades"
                grediview.DataSource = Datset
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    'grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                    'Dim imaga_buton As New HtmlInputImage
                    'imaga_buton.Attributes.Add("CssClass", "image_buton_clik_image")
                    'imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    'imaga_buton.Src = "../imagewf/ACEPTAR.png"
                    'imaga_buton.Attributes.Add("idd", grediview.Rows(i).Cells(0).Text.ToString())
                    'grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count - 1).Controls.Add(imaga_buton)
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-arrow-to-bottom")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Agrega actividad")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "a_s_r_p_333")
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
                Solicita_listado_actividades_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_actividades_flujo_trabajo = "Inconsistencia general función Solicita_listado_actividades_flujo_trabajo " & ex.Message
        End Try

    End Function
    Function Solicita_lista_actividade_anteriores_flujo_trabajo(ByVal id_flujo_trabajo As Integer,
                                                                ByRef grediview As GridView,
                                                                ByRef reflabel As Label,
                                                                ByRef label_nombre_flujo As Label,
                                                                ByRef hideselecion As Object,
                                                                ByRef update As UpdatePanel,
                                                                ByVal nombre_flujo_trabajo As String,
                                                                ByVal id_actividad_flujo_trabajo As Integer,
                                                                ByVal consulta_boot As Integer) As String
        Try
            Dim Sql_consulta As String = "select wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE, wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO," &
            "wraft.listado_actividades_workflow_id_Actividad, wrcaeft.ID_USUARIO_WORKFLOW_FUENTE," &
            "law.Nombre_Actividad  As NOMBRE, wrcaeft.ID_REGISTRO_ACTIVIDAD_ENVIO " &
            "From wf_registro_conectores_actividades_envio_flujo_trabajo As wrcaeft " &
            "INNER Join wf_registro_actividaes_flujos_trabajo  As wraft On (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE) " &
            "INNER Join listado_actividades_workflow As law On (law.Id_Actividad=wraft.listado_actividades_workflow_id_Actividad) " &
            "where wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO =" & id_actividad_flujo_trabajo & " And wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_actividade_anteriores_flujo_trabajo = "Error Solicita_lista_actividade_anteriores_flujo_trabajo " & Result
                Exit Function
            End If
            Datset.Tables(0).Columns.Add("DESCRIPCION", GetType(String))
            Datset.Tables(0).Columns.Add("DESTINO", GetType(String))
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "0 registro(s) "
                grediview.DataSource = Nothing
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                Solicita_lista_actividade_anteriores_flujo_trabajo = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset
                reflabel.Text = Datset.Tables(0).Rows.Count & " registro(s) "
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                If consulta_boot = 0 Then
                    For i As Integer = 0 To grediview.Rows.Count - 1
                        grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        Dim imaga_buton As New HtmlInputImage
                        imaga_buton.Attributes.Add("Class", "image_buton_clik_image")
                        imaga_buton.Attributes.Add("onclick", "prevent(Event,this)")
                        imaga_buton.Attributes.Add("title", "Terminar la trea y enviar a grupo o usuario " & grediview.Rows(i).Cells(5).Text.ToString())
                        imaga_buton.Src = "../workflow/imageneswf/share-all-solid.png"
                        imaga_buton.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        imaga_buton.Attributes.Add("id_flujo_trabjo", grediview.Rows(i).Cells(2).Text.ToString())
                        imaga_buton.Attributes.Add("id_actividad_destino", grediview.Rows(i).Cells(3).Text.ToString())
                        imaga_buton.Attributes.Add("id_usuario_workflow", grediview.Rows(i).Cells(4).Text.ToString())
                        imaga_buton.Attributes.Add("id_conector", grediview.Rows(i).Cells(6).Text.ToString())
                        Dim imaga_buton_imagen As New HtmlInputImage
                        imaga_buton_imagen.Attributes.Add("Class", "image_buton_clik_image_no_alow_cursor")
                        imaga_buton_imagen.Attributes.Add("onclick", "prevent_blank(Event,this);")
                        imaga_buton_imagen.Attributes.Add("height", "20px")
                        If grediview.Rows(i).Cells(4).Text = "&nbsp;" Then
                            imaga_buton_imagen.Src = "../workflow/imageneswf/user-solid.png"
                            imaga_buton_imagen.Attributes.Add("title", "Actividad de grupo de usuarios")
                        Else
                            imaga_buton_imagen.Src = "../workflow/imageneswf/user-solid_uno.png"
                            imaga_buton_imagen.Attributes.Add("title", "Actividad de usuario individual")
                        End If
                        Dim imaga_buton_detalle As New HtmlInputImage
                        imaga_buton_detalle.Attributes.Add("CssClass", "image_buton_clik_image")
                        imaga_buton_detalle.Attributes.Add("onclick", "prevent_detalle(Event,this)")
                        imaga_buton_detalle.Src = "../workflow/imageneswf/detalle.png"
                        imaga_buton_detalle.Attributes.Add("title", "Detalle de la actividad")
                        imaga_buton_detalle.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        imaga_buton_detalle.Attributes.Add("id_flujo_trabjo", grediview.Rows(i).Cells(2).Text.ToString())
                        imaga_buton_detalle.Attributes.Add("id_actividad_destino", grediview.Rows(i).Cells(3).Text.ToString())
                        Dim id_usuario_tempo As String = grediview.Rows(i).Cells(4).Text.ToString().Replace("&nbsp;", "0")
                        imaga_buton_detalle.Attributes.Add("id_usuario_workflow", id_usuario_tempo)
                        grediview.Rows(i).Cells(0).Controls.Add(imaga_buton_imagen)
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count - 1).Controls.Add(imaga_buton_detalle)
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count).Controls.Add(imaga_buton)
                    Next
                    Solicita_lista_actividade_anteriores_flujo_trabajo = "YES"
                    Exit Function
                Else
                    For i As Integer = 0 To grediview.Rows.Count - 1
                        grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(6).Text.ToString())
                        Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                        Dim divhtml_ As New HtmlControls.HtmlGenericControl("div")
                        Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fad fa-share-all")
                        Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_envio_actividad_flujo_anterior(Event,this);")
                        ahtml.Attributes.Add("title", "Enviar a (" & grediview.Rows(i).Cells(5).Text.ToString() & ")")
                        ahtml.Attributes.Add("id", grediview.Rows(i).Cells(6).Text.ToString())
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn bg-info btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_detalle(Event,this);")
                        ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("id_flujo_trabjo", grediview.Rows(i).Cells(2).Text.ToString())
                        ahtml.Attributes.Add("id_actividad_destino", grediview.Rows(i).Cells(3).Text.ToString())
                        Dim id_usuario_tempo As String = grediview.Rows(i).Cells(4).Text.ToString().Replace("&nbsp;", "0")
                        ahtml.Attributes.Add("id_usuario_workflow", id_usuario_tempo)
                        ahtml.Style.Add("margin-left", "3px")
                        If grediview.Rows(i).Cells(4).Text = "&nbsp;" Then
                            ihtml.Attributes.Add("class", "fad fa-user-friends")
                            ahtml.Attributes.Add("title", "Actividad de grupo de usuarios")
                        Else
                            ihtml.Attributes.Add("class", "fad fa-user")
                            ahtml.Attributes.Add("title", "Actividad de usuario individual")
                        End If
                        ahtml.Controls.Add(ihtml)
                        divhtml_.Controls.Add(ahtml)
                        divhtml_.Style.Add("display", "inline-flex")
                        divhtml.Style.Add("display", "inline-flex")
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count).Controls.Add(divhtml)
                        grediview.Rows(i).Cells(0).Controls.Add(divhtml_)
                        For z As Integer = 0 To grediview.Rows(i).Cells.Count - 2
                            If z > 0 Then
                                grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                                grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(Event,this);")
                            End If
                        Next
                    Next
                    Solicita_lista_actividade_anteriores_flujo_trabajo = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_lista_actividade_anteriores_flujo_trabajo = "Inconsistencia general funcion Solicita_lista_actividade_anteriores_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Enviar_tarea_flujo_trabajo_radicacion_simple(ByVal identi_actividad_flujo_destino As Integer,
                                                          ByVal id_tarea_workflow As Integer,
                                                          ByRef resultadocorreo As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Envia la actividad a una actividad de flujo de trabajo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'identi_actividad_flujo_destino  : Representa la identificación del flujo de trabajo
        'id_tarea_workflow               : Representa la identificación de la tarea
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'resultadocorreo  : Retorna el eeatado de  resultado del envio de correo electrónico
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-06
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim result As String = ""
            Dim Ref_class_estados_modulos_radicacion As New Class_estados_modulo_radicacion
            Dim stru_conector_flujo_ As stru_conector_flujo = Nothing
            Dim ref_Class_wf_registro_conectores As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            result = ref_Class_wf_registro_conectores.Solicita_datos_estructura_conector_flujo_trabajo(identi_actividad_flujo_destino,
                                                                                                       stru_conector_flujo_)
            If result <> "YES" Then
                Enviar_tarea_flujo_trabajo_radicacion_simple = result
                Exit Function
            End If
            resultadocorreo = "YES"
            Dim refclas As New ClassWorkflow
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim id_registro_radicado As Long = 0
            result = Class_ra_rad_estados_modulo_radicacion.Solicita_id_estado_modulo_id_tarea_workflow(id_tarea_workflow,
                                                                                                        1,
                                                                                                        id_registro_radicado)
            If result <> "YES" Then
                Enviar_tarea_flujo_trabajo_radicacion_simple = result
                Exit Function
            End If
            result = refclas.Terminar_Tarea_Workflow_Bacth(stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO.ToString,
                                                           stru_conector_flujo_.ID_ACTIVIDAD_DESTINO.ToString,
                                                           0,
                                                           id_tarea_workflow,
                                                           "",
                                                           stru_conector_flujo_.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,
                                                           stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,
                                                           stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO,
                                                           1,
                                                           resultadocorreo,
                                                           0,
                                                           0,
                                                           Val(HttpContext.Current.Session("Id_Usuario_Workflow")),
                                                           Val(HttpContext.Current.Session("Id_actividad_Workflow")),
                                                           1,
                                                           id_registro_radicado,
                                                           1)
            Enviar_tarea_flujo_trabajo_radicacion_simple = result
            Exit Function
        Catch ex As Exception
            Enviar_tarea_flujo_trabajo_radicacion_simple = "Inconsistencia general funcion Enviar_tarea_flujo_trabajo_radicacion_simple " & ex.Message
        End Try
    End Function
    Function Lista_campos_envio_tarea_workflow(ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo BOOTSTRAF para la lista de
        '          de usuarios o activividades de envio
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        '
        '                             
        '
        ' 
        '
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Retorna la estructura de campos
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-11-06
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            item = New class_campos_table_bostra_table
            item.field = "wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO"
            item.title = "IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO"
            item.title = "wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "wraft.listado_actividades_workflow_id_Actividad"
            item.title = "listado_actividades_workflow_id_Actividad"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "ID_USUARIO_WORKFLOW_DESTINO"
            item.title = "ID_USUARIO_WORKFLOW_DESTINO"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "DESTINO"
            item.title = "NOMBRE"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "DESCRIPCION"
            item.title = "DESCRIPCION"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "wrcaeft.ID_REGISTRO_ACTIVIDAD_ENVIO"
            item.title = "ID_REGISTRO_ACTIVIDAD_ENVIO"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "Opciones"
            item.title = "ENVIO"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 0
            item.clickToSelect = False
            item.visible_like_sql = 0
            item.align = "center"
            item.events = "window.operateEventsSEND"
            item.formatter = "operateFormattertablebootSEND"
            class_campos_table_bostra_table.Add(item)
            Lista_campos_envio_tarea_workflow = "YES"
        Catch ex As Exception
            Lista_campos_envio_tarea_workflow = "Inconsistencia general funcion Lista_campos_envio_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_row_campos_envio_tarea_workflow_table_boot(ByVal consulta As String,
                                                                 ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '          para envio de tareas
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'consulta               : Representa la consulta en comando SQL
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_row_gabinete_generic  : Retorna la estructura de datos de la consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-11-06
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("lista")
            Result = ref.SELECTION_SELECT_FIELD(consulta, Datset)
            If Result <> "YES" Then
                Solicita_row_campos_envio_tarea_workflow_table_boot = "Funcion  Solicita_row_campos_envio_tarea_workflow_table_boot " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = Newtonsoft.Json.JsonConvert.SerializeObject(Datset.Tables(0))
            Solicita_row_campos_envio_tarea_workflow_table_boot = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_row_campos_envio_tarea_workflow_table_boot = "Inconsistencia general fucnion Solicita_row_campos_envio_tarea_workflow_table_boot " & ex.Message
        End Try
    End Function
    Function Solicita_listado_actividades_para_envio_tarea_a_flujo(ByVal radicado As String,
                                                                   ByVal id_tarea_workflow As Long,
                                                                   ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista de usuarios de envio de tareas workflow por flujo
        '          de trabajo
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_flujo_trabajo              : Representa identificiación de flujo de 
        '                                trabajo
        'nombre_flujo_trabajo          : Representa nombre flujo de trabajo
        'id_tarea_workflow             : Representa identificación de tarea workflow
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_Row_Gabinete_Generic  : Retorna la estructura con los campos 
        ' y los registros de la consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-11-06
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try

            Dim Class_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            ' ---------/// Solicita datos del flujo de trabajo
            Dim id_usuario_workflow_actividad_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_flujo_trabajo As Integer = 0
            Result = Class_flujo_trabajo_workflow.Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(radicado,
                                                                                                                                   id_actividad_flujo_trabajo,
                                                                                                                                   id_flujo_trabajo,
                                                                                                                                   id_usuario_workflow_actividad_flujo_trabajo,
                                                                                                                                   HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                                                                   id_tarea_workflow)

            ' ---------/// Solicita la estructura de los campos
            Dim sql_variant As String = ""
            If id_actividad_flujo_trabajo <> 0 Then
                sql_variant = " And wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE=" & id_actividad_flujo_trabajo
            End If
            Result = Lista_campos_envio_tarea_workflow(class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Solicita_listado_actividades_para_envio_tarea_a_flujo = Result
                Exit Function
            End If
            Dim Sql_consulta As String = "(Select  wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO, wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,wraft.listado_actividades_workflow_id_Actividad," _
                                       & "wrcaeft.ID_USUARIO_WORKFLOW_DESTINO,law.Nombre_Actividad AS DESTINO,law.Descripcion_Actividad AS DESCRIPCION, wrcaeft.ID_REGISTRO_ACTIVIDAD_ENVIO FROM wf_registro_conectores_actividades_envio_flujo_trabajo As wrcaeft" &
                                       " INNER JOIN wf_registro_actividaes_flujos_trabajo  As wraft On (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO And wraft.ID_USUARIO_WORKFLOW  Is NULL )" &
                                       " INNER JOIN listado_actividades_workflow As law On (law.Id_Actividad=wraft.listado_actividades_workflow_id_Actividad)" &
                                       " where wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo & sql_variant &
                                       ") UNION " &
                                       " (Select  wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,wraft.listado_actividades_workflow_id_Actividad," &
                                       " wrcaeft.ID_USUARIO_WORKFLOW_DESTINO, CONCAT (uw.Nombre_Usuario, '-' ,uw.Cargo_Usuario) AS DESTINO,law.Descripcion_Actividad AS DESCRIPCION, wrcaeft.ID_REGISTRO_ACTIVIDAD_ENVIO " &
                                       " FROM wf_registro_conectores_actividades_envio_flujo_trabajo AS wrcaeft " &
                                       " INNER JOIN wf_registro_actividaes_flujos_trabajo  AS wraft on (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO  )" &
                                       " INNER JOIN usuario_workflow  as uw on (uw.idU_suario=wraft.ID_USUARIO_WORKFLOW)" &
                                       " INNER JOIN listado_actividades_workflow AS law on (law.Id_Actividad=wraft.listado_actividades_workflow_id_Actividad)" &
                                       " where wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo & sql_variant & ") "

            ' --------- /// Ejecuta la consulta  y retorna los row
            Result = Solicita_row_campos_envio_tarea_workflow_table_boot(Sql_consulta,
                                                                         class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                Solicita_listado_actividades_para_envio_tarea_a_flujo = Result
                Exit Function
            End If
            Solicita_listado_actividades_para_envio_tarea_a_flujo = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_listado_actividades_para_envio_tarea_a_flujo = "Inconsistencia general funcion Solicita_listado_actividades_para_envio_tarea_a_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_listado_actividades_para_envio_tarea_a_flujo(ByVal id_flujo_trabajo As Integer,
                                                                   ByRef grediview As GridView,
                                                                   ByRef reflabel As Label,
                                                                   ByRef label_nombre_flujo As Label,
                                                                   ByRef hideselecion As Object,
                                                                   ByRef update As UpdatePanel,
                                                                   ByVal nombre_flujo_trabajo As String,
                                                                   ByVal id_actividad_flujo_trabajo As Integer,
                                                                   ByVal consulta_boot As Integer) As String

        Try
            Dim sql_variant As String = ""
            label_nombre_flujo.Text = "Flujo de trabajo de la tarea (" & nombre_flujo_trabajo & ")"
            If id_actividad_flujo_trabajo <> 0 Then
                sql_variant = " And wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE=" & id_actividad_flujo_trabajo
            End If
            Dim Sql_consulta As String = "(Select  wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO, wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,wraft.listado_actividades_workflow_id_Actividad," _
                                        & "wrcaeft.ID_USUARIO_WORKFLOW_DESTINO,law.Nombre_Actividad  As NOMBRE, wrcaeft.ID_REGISTRO_ACTIVIDAD_ENVIO FROM wf_registro_conectores_actividades_envio_flujo_trabajo As wrcaeft" &
                                        " INNER JOIN wf_registro_actividaes_flujos_trabajo  As wraft On (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO And wraft.ID_USUARIO_WORKFLOW  Is NULL )" &
                                        " INNER JOIN listado_actividades_workflow As law On (law.Id_Actividad=wraft.listado_actividades_workflow_id_Actividad)" &
                                        " where wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo & sql_variant &
                                        ") UNION " &
                                        " (Select  wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,wraft.listado_actividades_workflow_id_Actividad," &
                                        " wrcaeft.ID_USUARIO_WORKFLOW_DESTINO, CONCAT (uw.Nombre_Usuario, '-' ,uw.Cargo_Usuario) as NOMBRE, wrcaeft.ID_REGISTRO_ACTIVIDAD_ENVIO " &
                                        " FROM wf_registro_conectores_actividades_envio_flujo_trabajo AS wrcaeft " &
                                        " INNER JOIN wf_registro_actividaes_flujos_trabajo  AS wraft on (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=wrcaeft.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO  )" &
                                        " INNER JOIN usuario_workflow  as uw on (uw.idU_suario=wraft.ID_USUARIO_WORKFLOW)" &
                                        " INNER JOIN listado_actividades_workflow AS law on (law.Id_Actividad=wraft.listado_actividades_workflow_id_Actividad)" &
                                        " where wrcaeft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo & sql_variant & ") "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_actividades_para_envio_tarea_a_flujo = "Error Solicita_listado_actividades_para_envio_tarea_a_flujo " & Result
                Exit Function
            End If
            Datset.Tables(0).Columns.Add("DESCRIPCION", GetType(String))
            Datset.Tables(0).Columns.Add("DESTINO", GetType(String))
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "0 registro(s) "
                grediview.DataSource = Nothing
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                Solicita_listado_actividades_para_envio_tarea_a_flujo = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset
                reflabel.Text = Datset.Tables(0).Rows.Count & " registro(s) "
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                If consulta_boot = 0 Then
                    For i As Integer = 0 To grediview.Rows.Count - 1
                        grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        Dim imaga_buton As New HtmlInputImage
                        imaga_buton.Attributes.Add("Class", "image_buton_clik_image")
                        imaga_buton.Attributes.Add("onclick", "prevent(event,this)")
                        imaga_buton.Attributes.Add("title", "Terminar la trea y enviar a grupo o usuario " & grediview.Rows(i).Cells(5).Text.ToString())
                        imaga_buton.Src = "../workflow/imageneswf/share-all-solid.png"
                        imaga_buton.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        imaga_buton.Attributes.Add("id_flujo_trabjo", grediview.Rows(i).Cells(2).Text.ToString())
                        imaga_buton.Attributes.Add("id_actividad_destino", grediview.Rows(i).Cells(3).Text.ToString())
                        imaga_buton.Attributes.Add("id_usuario_workflow", grediview.Rows(i).Cells(4).Text.ToString())
                        imaga_buton.Attributes.Add("id_conector", grediview.Rows(i).Cells(6).Text.ToString())
                        Dim imaga_buton_imagen As New HtmlInputImage
                        imaga_buton_imagen.Attributes.Add("Class", "image_buton_clik_image_no_alow_cursor")
                        imaga_buton_imagen.Attributes.Add("onclick", "prevent_blank(event,this);")
                        imaga_buton_imagen.Attributes.Add("height", "20px")
                        If grediview.Rows(i).Cells(4).Text = "&nbsp;" Then
                            imaga_buton_imagen.Src = "../workflow/imageneswf/user-solid.png"
                            imaga_buton_imagen.Attributes.Add("title", "Actividad de grupo de usuarios")
                        Else
                            imaga_buton_imagen.Src = "../workflow/imageneswf/user-solid_uno.png"
                            imaga_buton_imagen.Attributes.Add("title", "Actividad de usuario individual")
                        End If
                        Dim imaga_buton_detalle As New HtmlInputImage
                        imaga_buton_detalle.Attributes.Add("CssClass", "image_buton_clik_image")
                        imaga_buton_detalle.Attributes.Add("onclick", "prevent_detalle(event,this)")
                        imaga_buton_detalle.Src = "../workflow/imageneswf/detalle.png"
                        imaga_buton_detalle.Attributes.Add("title", "Detalle de la actividad")
                        imaga_buton_detalle.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        imaga_buton_detalle.Attributes.Add("id_flujo_trabjo", grediview.Rows(i).Cells(2).Text.ToString())
                        imaga_buton_detalle.Attributes.Add("id_actividad_destino", grediview.Rows(i).Cells(3).Text.ToString())
                        Dim id_usuario_tempo As String = grediview.Rows(i).Cells(4).Text.ToString().Replace("&nbsp;", "0")
                        imaga_buton_detalle.Attributes.Add("id_usuario_workflow", id_usuario_tempo)
                        grediview.Rows(i).Cells(0).Controls.Add(imaga_buton_imagen)
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count - 1).Controls.Add(imaga_buton_detalle)
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count).Controls.Add(imaga_buton)
                    Next
                    Solicita_listado_actividades_para_envio_tarea_a_flujo = "YES"
                    Exit Function
                Else
                    For i As Integer = 0 To grediview.Rows.Count - 1
                        grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(6).Text.ToString())
                        Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                        Dim divhtml_ As New HtmlControls.HtmlGenericControl("div")
                        Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fad fa-share-all")
                        Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_envio_actividad_flujo(event,this);")
                        ahtml.Attributes.Add("title", "Enviar a (" & grediview.Rows(i).Cells(5).Text.ToString() & ")")
                        ahtml.Attributes.Add("id", grediview.Rows(i).Cells(6).Text.ToString())
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn bg-info btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_detalle(event,this);")
                        ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("id_flujo_trabjo", grediview.Rows(i).Cells(2).Text.ToString())
                        ahtml.Attributes.Add("id_actividad_destino", grediview.Rows(i).Cells(3).Text.ToString())
                        Dim id_usuario_tempo As String = grediview.Rows(i).Cells(4).Text.ToString().Replace("&nbsp;", "0")
                        ahtml.Attributes.Add("id_usuario_workflow", id_usuario_tempo)
                        ahtml.Style.Add("margin-left", "3px")
                        If grediview.Rows(i).Cells(4).Text = "&nbsp;" Then
                            ihtml.Attributes.Add("class", "fad fa-user-friends")
                            ahtml.Attributes.Add("title", "Actividad de grupo de usuarios")
                        Else
                            ihtml.Attributes.Add("class", "fad fa-user")
                            ahtml.Attributes.Add("title", "Actividad de usuario individual")
                        End If
                        ahtml.Controls.Add(ihtml)
                        divhtml_.Controls.Add(ahtml)
                        divhtml_.Style.Add("display", "inline-flex")
                        divhtml.Style.Add("display", "inline-flex")
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count).Controls.Add(divhtml)
                        grediview.Rows(i).Cells(0).Controls.Add(divhtml_)
                        For z As Integer = 0 To grediview.Rows(i).Cells.Count - 2
                            If z > 0 Then
                                grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                                grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                            End If
                        Next
                    Next
                    Solicita_listado_actividades_para_envio_tarea_a_flujo = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_listado_actividades_para_envio_tarea_a_flujo = "Inconsistencia general función Solicita_listado_actividades_para_envio_tarea_a_flujo " & ex.Message
        End Try
    End Function
    Function Lista_usuarios_relacionados_id_actividad(ByVal id_actividad_flujo_documental As Integer, ByRef matriz_usuario_relacionado() As String) As String
        Try
            Dim Parametro_Consulta As String = "select uw.Nombre_Usuario from grupos_workflow as gw " &
                "inner join usuario_workflow as uw on (uw.Grupos_Workflow_Id_Grupo=gw.Id_Grupo)" &
                " where id_Actividad=" & id_actividad_flujo_documental & " and ESTADO_USUARIO=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_usuarios_relacionados_id_actividad = "Error función Lista_usuarios_relacionados_id_actividad " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matriz_usuario_relacionado(i)
                    matriz_usuario_relacionado(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Lista_usuarios_relacionados_id_actividad = "YES"
                Exit Function
            Else
                Lista_usuarios_relacionados_id_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_usuarios_relacionados_id_actividad = "Inconsistencia general función Lista_usuarios_relacionados_id_actividad " & ex.Message
        End Try
    End Function
    Function Lista_detalle_actividad_workflow(ByVal id_actividad_flujo_documental As Integer,
                                              ByRef nombre_actividad As String,
                                              ByRef descripcion_actividad As String,
                                              ByRef tipo_actividad As String) As String
        Try
            Dim Parametro_Consulta As String = "select law.Nombre_Actividad,law.Descripcion_Actividad,agw.Descripcion_Actividad from listado_actividades_workflow as law " &
                "left outer join actividades_generales_workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General)" &
                " where Id_Actividad=" & id_actividad_flujo_documental
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_detalle_actividad_workflow = "Error función Lista_detalle_actividad_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_actividad = ""
                Else
                    nombre_actividad = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    descripcion_actividad = ""
                Else
                    descripcion_actividad = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    tipo_actividad = ""
                Else
                    tipo_actividad = Datset.Tables(0).Rows(0).Item(2)
                End If
                Lista_detalle_actividad_workflow = "YES"
                Exit Function
            Else
                Lista_detalle_actividad_workflow = "Imposible encontrar datos de la actividad  (" & id_actividad_flujo_documental & ")"
                Exit Function
            End If
        Catch ex As Exception
            Lista_detalle_actividad_workflow = "Inconsistencia general función Lista_detalle_actividad_workflow " & ex.Message
        End Try
    End Function
    Function Lista_detalle_usuario_workflow(ByVal id_usuario_workflow_destino As Integer,
                                            ByRef nombre_usuario As String,
                                            ByRef cargo_usuario As String,
                                            ByRef correo_electronico As String,
                                            ByRef grupo As String) As String
        Try
            Dim Parametro_Consulta As String = "select uw.Nombre_Usuario,uw.Cargo_Usuario,uw.Correo_Usuario,gw.Nombre_Grupo from usuario_workflow as uw " _
                                               & "left OUTER JOIN grupos_workflow as gw on (gw.Id_Grupo=uw.Grupos_Workflow_Id_Grupo)" _
                                               & " where idU_suario ='" & id_usuario_workflow_destino & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_detalle_usuario_workflow = "Error función Lista_detalle_usuario_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_usuario = ""
                Else
                    nombre_usuario = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    cargo_usuario = ""
                Else
                    cargo_usuario = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    correo_electronico = ""
                Else
                    correo_electronico = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    grupo = ""
                Else
                    grupo = Datset.Tables(0).Rows(0).Item(3)
                End If
                Lista_detalle_usuario_workflow = "YES"
                Exit Function
            Else
                Lista_detalle_usuario_workflow = "Imposible encontrar datos del usuario  (" & id_usuario_workflow_destino & ")"
                Exit Function
            End If
        Catch ex As Exception
            Lista_detalle_usuario_workflow = "Inconsistencia general función Lista_detalle_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_id_ruta_flujo_trabajo(ByVal id_flujo_trabajo As Integer,
                                            ByRef id_ruta As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select rutas_workflow_id_Ruta from wf_flujos_trabajo where ID_WF_FLUJOS_TRABAJO ='" & id_flujo_trabajo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_ruta_flujo_trabajo = "Error función Solicita_id_ruta_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_ruta = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_ruta_flujo_trabajo = "YES"
                Exit Function
            Else
                Solicita_id_ruta_flujo_trabajo = "Imposible encontrar el flujo de trabajo (" & id_flujo_trabajo & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_ruta_flujo_trabajo = "Inconsistencia general función Solicita_id_ruta_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Seleccion_menu_pricipal_flujo_trabajo(ByVal valor_seleccion As String,
                                                   ByRef pag As Page) As String
        Try

            Dim ref_DropDownList_combo_rutas As DropDownList = pag.FindControl("DropDownList_combo_rutas")
            Dim ref_DropDownList_tipo_flujo As DropDownList = pag.FindControl("DropDownList_tipo_flujo")
            Dim ref_DropDownList_flujos_disponibles_workflow As DropDownList = pag.FindControl("DropDownList_flujos_disponibles_workflow")
            Dim ref_ModalPopupExtender_edition_nuevo_flujo_trabajo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_nuevo_flujo_trabajo")
            Dim ref_UpdatePanel_combo_rutas_disponibles As UpdatePanel = pag.FindControl("UpdatePanel_combo_rutas_disponibles")
            Dim ref_UpdatePanel_nuevo_flujo_trabajo As UpdatePanel = pag.FindControl("UpdatePanel_nuevo_flujo_trabajo")
            Dim ref_Iframe_paginas_externas_popup As Object = pag.FindControl("Iframe_paginas_externas_popup_")
            Dim ref_UpdatePanel_paginas_externas_popou As UpdatePanel = pag.FindControl("UpdatePanel_paginas_externas_popou")
            Dim ref_ModalPopupExtender_edition_paginas_externas_popou As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_paginas_externas_popou")
            Dim ref_DropDownList_combo_rutas_copia As DropDownList = pag.FindControl("DropDownList_combo_rutas_copia")
            Dim ref_UpdatePanel_combo_rutas_disponibles_copia As UpdatePanel = pag.FindControl("UpdatePanel_combo_rutas_disponibles_copia")
            Dim ref_DropDownList_tipo_flujo_copia As DropDownList = pag.FindControl("DropDownList_tipo_flujo_copia")
            Dim ref_UpdatePanel_copia_flujo_trabajo As UpdatePanel = pag.FindControl("UpdatePanel_copia_flujo_trabajo")
            Dim ref_ModalPopupExtender_edition_copia_flujo_trabajo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_copia_flujo_trabajo")
            Dim ModalPopupExtender_edition_activa_actividad_inicio As AjaxControlToolkit.ModalPopupExtender =
                pag.FindControl("ModalPopupExtender_edition_activa_actividad_inicio")
            Dim Check_actividad_inicio As CheckBox = pag.FindControl("Check_actividad_inicio")
            Dim UpdatePanel_activa_actividad_inicio As UpdatePanel = pag.FindControl("UpdatePanel_activa_actividad_inicio")
            Dim ref_diagramView As DiagramView = pag.FindControl("diagramView")
            Dim TextBox_Edita_nombre_flujo_trabajo As TextBox = pag.FindControl("TextBox_Edita_nombre_flujo_trabajo")
            Dim TextBox_Edita_descripcion_flujo_trabajo As TextBox = pag.FindControl("TextBox_Edita_descripcion_flujo_trabajo")
            Dim UpdatePanel_edita_flujo_trabajo As UpdatePanel = pag.FindControl("UpdatePanel_edita_flujo_trabajo")
            Dim updatemenu As UpdatePanel = pag.FindControl("updatemenu")
            Dim ModalPopupExtender_edition_edita_flujo_trabajo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_edita_flujo_trabajo")
            Dim ModalPopupExtender_edition_usuario_respon_flujo As AjaxControlToolkit.ModalPopupExtender =
                pag.FindControl("ModalPopupExtender_edition_usuario_respon_flujo")
            Dim UpdatePanel_usuario_respon_flujo As UpdatePanel = pag.FindControl("UpdatePanel_usuario_respon_flujo")
            Dim DropDownList_user_respon_flujo As DropDownList = pag.FindControl("DropDownList_user_respon_flujo")
            Dim UpdatePanel_list_tipo_flujo As UpdatePanel = pag.FindControl("UpdatePanel_list_tipo_flujo")
            Dim updatapanel_iframe As UpdatePanel = pag.FindControl("updatapanel_iframe")
            Dim Hidden_ruta_archivo As Object = pag.FindControl("Hidden_ruta_archivo")
            Dim ifmExcel_ As Object = pag.FindControl("ifmExcel_")
            Dim Result As String = ""
            '---------------------------------------------
            'Exporta a pdf el diagrama 
            '---------------------------------------------
            Dim Class_worflow_rutas As New Class_worflow_rutas
            If valor_seleccion = "F-EXP-FW" Then
                Result = Class_worflow_rutas.Exporta_pdf_mindifucion(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL"),
                                                                     ref_diagramView,
                                                                     ifmExcel_,
                                                                     Hidden_ruta_archivo,
                                                                     updatapanel_iframe)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                End If
            End If
            '---------------------------------------------
            'Agregar flujo de trabajo a ruta
            '---------------------------------------------
            If valor_seleccion = "N-F-TF" Then
                Dim ref_clas_workflow_ruta As New Class_worflow_rutas
                Dim nombres_ruta() As String = Nothing
                Result = ref_clas_workflow_ruta.Solicita_nombres_rutas_workflow(nombres_ruta)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                End If
                ref_DropDownList_combo_rutas.Items.Clear()
                ref_DropDownList_combo_rutas.Items.Add("")
                Result = ref_clas_workflow_ruta.Lista_rutas_interface_importacion(nombres_ruta,
                                                                                  ref_DropDownList_combo_rutas,
                                                                                  ref_UpdatePanel_combo_rutas_disponibles,
                                                                                  1)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                End If
                UpdatePanel_list_tipo_flujo.Update()
                ref_DropDownList_tipo_flujo.Items.Clear()
                ref_DropDownList_tipo_flujo.Items.Add("Abierto")
                ref_DropDownList_tipo_flujo.Items.Add("Cerrado")
                ref_UpdatePanel_nuevo_flujo_trabajo.Update()
                ref_ModalPopupExtender_edition_nuevo_flujo_trabajo.Show()
                Seleccion_menu_pricipal_flujo_trabajo = "YES"
                Exit Function
            End If
            '------------------------------------------------
            'Gestión de tramites y flujos de trabajo
            '------------------------------------------------
            If valor_seleccion = "G-T-FW" Then
                ref_Iframe_paginas_externas_popup.Attributes.Add("src", "../workflow/WebFormWorkflowRelacionFlujoTramite.aspx")
                ref_UpdatePanel_paginas_externas_popou.Update()
                ref_ModalPopupExtender_edition_paginas_externas_popou.Show()
            End If
            '------------------------------------------------
            'Configura actividad seleccionada
            '------------------------------------------------
            If valor_seleccion = "C-A-SF" Then
                Result = Inicializa_interface_configuracion_actividad(pag)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                End If
            End If
            '-----------------------------------------------
            'Configura flujo seleccionado   
            '----------------------------------------------
            If valor_seleccion = "C-F-TS" Then
                Result = Inicializa_interface_configuracion_flujo_trabajo(pag)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                End If
            End If
            If valor_seleccion = "C-FTA-FW" Then
                If ref_DropDownList_flujos_disponibles_workflow.Text = "" Then
                    Seleccion_menu_pricipal_flujo_trabajo = "Por favor seleccione el flujo de trabajo a copiar "
                    Exit Function
                End If
                Dim ref_clas_workflow_ruta As New Class_worflow_rutas
                Dim nombres_ruta() As String = Nothing
                Result = ref_clas_workflow_ruta.Solicita_nombres_rutas_workflow(nombres_ruta)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                End If
                ref_DropDownList_combo_rutas_copia.Items.Add("")
                Result = ref_clas_workflow_ruta.Lista_rutas_interface_importacion(nombres_ruta,
                                                                                  ref_DropDownList_combo_rutas_copia,
                                                                                  ref_UpdatePanel_combo_rutas_disponibles_copia,
                                                                                  1)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                End If
                ref_DropDownList_tipo_flujo_copia.Items.Clear()
                ref_DropDownList_tipo_flujo_copia.Items.Add("Abierto")
                ref_DropDownList_tipo_flujo_copia.Items.Add("Cerrado")
                ref_UpdatePanel_copia_flujo_trabajo.Update()
                ref_ModalPopupExtender_edition_copia_flujo_trabajo.Show()
                Seleccion_menu_pricipal_flujo_trabajo = "YES"
                Exit Function
            End If
            '-----------------------------------------------
            'Activa actividad de inicio
            '-----------------------------------------------
            If valor_seleccion = "C-A-IF" Then
                Result = Me.Solicita_estado_actividad_inicio(ModalPopupExtender_edition_activa_actividad_inicio,
                                                              Check_actividad_inicio,
                                                              UpdatePanel_activa_actividad_inicio,
                                                              ref_diagramView)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                Else
                    ModalPopupExtender_edition_activa_actividad_inicio.Show()
                    Seleccion_menu_pricipal_flujo_trabajo = "YES"
                    Exit Function
                End If
            End If
            '------------------------------------------------
            'Edita datos de caracterización flujo de trabajo
            '------------------------------------------------
            If valor_seleccion = "E-DC-FT" Then
                Result = Me.Solicita_datos_caracterizacion_flujo_trabajo(HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO"),
                                                                         TextBox_Edita_nombre_flujo_trabajo,
                                                                         TextBox_Edita_descripcion_flujo_trabajo,
                                                                         UpdatePanel_edita_flujo_trabajo)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                Else
                    ModalPopupExtender_edition_edita_flujo_trabajo.Show()
                    Seleccion_menu_pricipal_flujo_trabajo = "YES"
                    Exit Function
                End If
            End If
            '----------------------------------------------
            'Usuario responsable flujo trebajo
            '----------------------------------------------
            If valor_seleccion = "E-RS-FT" Then
                If HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO") = 0 Then
                    Seleccion_menu_pricipal_flujo_trabajo = "Debe selecionar el flujo"
                    Exit Function
                End If
                Dim ref_clas_usuar_ges As New Class_ra_usuario_gestion_responsable_flujo
                Result = ref_clas_usuar_ges.Solicita_lista_usuario_responsable_flujo(HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO"),
                                                                                     DropDownList_user_respon_flujo,
                                                                                     UpdatePanel_usuario_respon_flujo)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal_flujo_trabajo = Result
                    Exit Function
                Else
                    ModalPopupExtender_edition_usuario_respon_flujo.Show()
                    Seleccion_menu_pricipal_flujo_trabajo = "YES"
                    Exit Function
                End If
            End If
            '---------------------------------------------------------------------------
            'Configuración del estado de noticación al correo electrónico del conector
            '---------------------------------------------------------------------------
            If valor_seleccion = "C-CONECTOR-WB" Then
                Dim estado_envio_correo_electronico As Integer = 0
                HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR") = 0
                Dim ref_wf_registro_conectores_actividades_envio_flujo_trabajo As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
                If HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO") = 0 Then
                    Seleccion_menu_pricipal_flujo_trabajo = "Debe selecionar el flujo"
                    Exit Function
                End If

                For Each sha In ref_diagramView.Diagram.Selection.Items
                    If ref_diagramView.Diagram.Selection.Items(0).GetType.FullName = "MindFusion.Diagramming.DiagramLink" Then
                        Dim split_id() As String = sha.Id.ToString.Split("_")
                        HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR") = split_id(1)
                        Exit For
                    End If
                Next
                Dim stru_config_conector_flujo As stru_config_conector_flujo = Nothing
                If HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR") <> 0 Then
                    Result = ref_wf_registro_conectores_actividades_envio_flujo_trabajo.Solicita_configuracion_conector_flujo(HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR"),
                                                                                                                              stru_config_conector_flujo)
                    If Result <> "YES" Then
                        Seleccion_menu_pricipal_flujo_trabajo = Result
                        Exit Function
                    End If
                    Dim CheckBox_estado_copia_estructura_total As CheckBox = pag.FindControl("CheckBox_estado_copia_estructura_total")
                    Dim CheckBox_estado_correo_conector As CheckBox = pag.FindControl("CheckBox_estado_correo_conector")
                    Dim CheckBox_autoriza_tarea As CheckBox = pag.FindControl("CheckBox_autoriza_tarea")
                    Dim CheckBox_estado_copia_estructura As CheckBox = pag.FindControl("CheckBox_estado_copia_estructura")
                    Dim CheckBox_Estado_asigna_expediente As CheckBox = pag.FindControl("CheckBox_Estado_asigna_expediente")
                    Dim CheckBox_autoriza_tarea_firma_digital As CheckBox = pag.FindControl("CheckBox_autoriza_tarea_firma_digital")
                    Dim CheckBox_estado_firma_digital As CheckBox = pag.FindControl("CheckBox_estado_firma_digital")
                    Dim CheckBox_estado_valida_balanceo As CheckBox = pag.FindControl("CheckBox_estado_valida_balanceo")
                    Dim UpdatePanel_configura_envi_correo_conector As UpdatePanel = pag.FindControl("UpdatePanel_configura_envi_correo_conector")
                    Dim ModalPopupExtender_edition_configura_envi_correo_conector As AjaxControlToolkit.ModalPopupExtender =
                        pag.FindControl("ModalPopupExtender_edition_configura_envi_correo_conector")
                    If stru_config_conector_flujo.Estado_evia_correo = 1 Then
                        CheckBox_estado_correo_conector.Checked = True
                    Else
                        CheckBox_estado_correo_conector.Checked = False
                    End If
                    If stru_config_conector_flujo.Estado_soicita_autorizacion = 1 Then
                        CheckBox_autoriza_tarea.Checked = True
                    Else
                        CheckBox_autoriza_tarea.Checked = False
                    End If
                    If stru_config_conector_flujo.Estado_soicita_autorizacion_firma_digital = 1 Then
                        CheckBox_autoriza_tarea_firma_digital.Checked = True
                    Else
                        CheckBox_autoriza_tarea_firma_digital.Checked = False
                    End If
                    If stru_config_conector_flujo.Estado_copia_documento_estructura = 1 Then
                        CheckBox_estado_copia_estructura.Checked = True
                    Else
                        CheckBox_estado_copia_estructura.Checked = False
                    End If
                    If stru_config_conector_flujo.Estado_asigna_expediente = 1 Then
                        CheckBox_Estado_asigna_expediente.Checked = True
                    Else
                        CheckBox_Estado_asigna_expediente.Checked = False
                    End If
                    If stru_config_conector_flujo.Estado_firma_digital = 1 Then
                        CheckBox_estado_firma_digital.Checked = True
                    Else
                        CheckBox_estado_firma_digital.Checked = False
                    End If
                    If stru_config_conector_flujo.estado_valida_balanceo = 1 Then
                        CheckBox_estado_valida_balanceo.Checked = True
                    Else
                        CheckBox_estado_valida_balanceo.Checked = False
                    End If
                    If stru_config_conector_flujo.Estado_copia_estructura_total = 1 Then
                        CheckBox_estado_copia_estructura_total.Checked = True
                    Else
                        CheckBox_estado_copia_estructura_total.Checked = False
                    End If
                    UpdatePanel_configura_envi_correo_conector.Update()
                    ModalPopupExtender_edition_configura_envi_correo_conector.Show()
                End If
            End If
            Seleccion_menu_pricipal_flujo_trabajo = "YES"
        Catch ex As Exception
            Seleccion_menu_pricipal_flujo_trabajo = "Inconsistencia general función Seleccion_menu_pricipal_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Inicializa_interface_configuracion_flujo_trabajo(ByRef pag As Page) As String
        Try
            If HttpContext.Current.Session.Item("DR_FLUJO_SELECCIONADO") = "" Then
                Inicializa_interface_configuracion_flujo_trabajo = "Por favor seleccionar el flujo de trabajo a configurar"
                Exit Function
            End If
            Dim ref_ModalPopupExtender_edition_configura_tipo_flujo_trabajo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_configura_tipo_flujo_trabajo")
            Dim ref_Check_flujo_abierto As CheckBox = pag.FindControl("Check_flujo_abierto")
            Dim ref_CheckBox_flujo_cerrado As CheckBox = pag.FindControl("CheckBox_flujo_cerrado")
            Dim ref_UpdatePanel_configura_tipo_flujo_trabajo As UpdatePanel = pag.FindControl("UpdatePanel_configura_tipo_flujo_trabajo")
            Dim estado_actividad_cerrado_abierto As Integer = 0
            Dim Result As String = Me.Retorna_estado_Abierto_cerrado_flujo_documental(HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO"),
                                                                                      estado_actividad_cerrado_abierto)
            If Result <> "YES" Then
                Inicializa_interface_configuracion_flujo_trabajo = Result
                Exit Function
            End If
            If estado_actividad_cerrado_abierto = 1 Then
                ref_CheckBox_flujo_cerrado.Checked = True
                ref_Check_flujo_abierto.Checked = False
            Else
                ref_CheckBox_flujo_cerrado.Checked = False
                ref_Check_flujo_abierto.Checked = True
            End If
            ref_UpdatePanel_configura_tipo_flujo_trabajo.Update()
            ref_ModalPopupExtender_edition_configura_tipo_flujo_trabajo.Show()
            Inicializa_interface_configuracion_flujo_trabajo = "YES"
        Catch ex As Exception
            Inicializa_interface_configuracion_flujo_trabajo = "Inconsistencia general función Inicializa_interface_configuracion_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Inicializa_interface_configuracion_actividad(ByRef pag As Page) As String
        Try
            Dim ref_ModalPopupExtender_edition_configura_tipo_actividad_flujo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_configura_tipo_actividad_flujo")
            Dim ref_Check_flujo_abierto_actividad As CheckBox = pag.FindControl("Check_flujo_abierto_actividad")
            Dim ref_CheckBox_flujo_cerrado_actividad As CheckBox = pag.FindControl("CheckBox_flujo_cerrado_actividad")
            Dim ref_diagramView As DiagramView = pag.FindControl("diagramView")
            Dim ref_UpdatePanel_configura_tipo_actividad_flujo As UpdatePanel = pag.FindControl("UpdatePanel_configura_tipo_actividad_flujo")
            Dim Result As String = ""
            If ref_diagramView.Diagram.Selection.Items.Count = 0 Then
                Inicializa_interface_configuracion_actividad = "Por favor seleccione el elemento del diagrama "
                Exit Function
            End If
            If ref_diagramView.Diagram.Selection.Items.Count > 1 Then
                Inicializa_interface_configuracion_actividad = "Solo se puede configurar un elemento del diagrama"
                Exit Function
            End If
            '-----------------------------------------------------------
            'Actualiza los chek de los estados de las actividades
            '-----------------------------------------------------------
            If ref_diagramView.Diagram.Selection.Items(0).GetType.FullName <> "MindFusion.Diagramming.ShapeNode" Then
                Inicializa_interface_configuracion_actividad = "Solo se puede configurar actividades en el diagrama"
                Exit Function
            End If
            Dim id_actividad As Integer = ref_diagramView.Diagram.Selection.Items(0).Id
            Dim estado_actividad_cerrado_abierto As Integer = 0
            Result = Me.Retorna_estado_Abierto_cerrado_actividad_flujo_documental(id_actividad, estado_actividad_cerrado_abierto)
            If Result <> "YES" Then
                Inicializa_interface_configuracion_actividad = Result
                Exit Function
            End If
            If estado_actividad_cerrado_abierto = 1 Then
                ref_CheckBox_flujo_cerrado_actividad.Checked = True
                ref_Check_flujo_abierto_actividad.Checked = False
            Else
                ref_CheckBox_flujo_cerrado_actividad.Checked = False
                ref_Check_flujo_abierto_actividad.Checked = True
            End If
            ref_UpdatePanel_configura_tipo_actividad_flujo.Update()
            ref_ModalPopupExtender_edition_configura_tipo_actividad_flujo.Show()
            Inicializa_interface_configuracion_actividad = "YES"
        Catch ex As Exception
            Inicializa_interface_configuracion_actividad = "Inconsistencia general función Inicializa_interface_configuracion_actividad " & ex.Message
        End Try
    End Function

    Function Retorna_estado_Abierto_cerrado_actividad_flujo_documental(ByVal id_actividad As Integer,
                                                                       ByRef estado_actividad_cerrado_abierto As Integer) As String
        '----------------------------------------------------------------------
        'Funcion : Retorna el estado de la actividad dentro del flujo de 
        'trabajo
        'Fecha : 2017-09-25
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  TIPO_ABIERTA_CERRADA_ACTIVIDAD from wf_registro_actividaes_flujos_trabajo where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO ='" & id_actividad & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estado_Abierto_cerrado_actividad_flujo_documental = "Error función Retorna_estado_Abierto_cerrado_actividad_flujo_documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_actividad_cerrado_abierto = Datset.Tables(0).Rows(0).Item(0)
                Retorna_estado_Abierto_cerrado_actividad_flujo_documental = "YES"
                Exit Function
            Else
                Retorna_estado_Abierto_cerrado_actividad_flujo_documental = "Imposible encontrar el id actividad en el flujo de trabajo " & id_actividad
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estado_Abierto_cerrado_actividad_flujo_documental = "Inconsistencia general función Retorna_estado_Abierto_cerrado_actividad_flujo_documental " & ex.Message
        End Try
    End Function
    Function Retorna_estado_Abierto_cerrado_flujo_documental(ByVal id_flujo_trabajo As Integer,
                                                             ByRef estado_flujo_cerrado_abierto As Integer) As String
        '----------------------------------------------------------------------
        'Funcion : Retorna el estado  del flujo de 
        'trabajo
        'Fecha : 2017-09-25
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  TIPO_RUTA_ABIERTA_CERRADA from wf_flujos_trabajo where ID_WF_FLUJOS_TRABAJO ='" & id_flujo_trabajo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estado_Abierto_cerrado_flujo_documental = "Error función Retorna_estado_Abierto_cerrado_flujo_documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_flujo_cerrado_abierto = Datset.Tables(0).Rows(0).Item(0)
                Retorna_estado_Abierto_cerrado_flujo_documental = "YES"
                Exit Function
            Else
                Retorna_estado_Abierto_cerrado_flujo_documental = "Imposible encontrar el id del flujo de trabajo " & id_flujo_trabajo
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estado_Abierto_cerrado_flujo_documental = "Inconsistencia general función Retorna_estado_Abierto_cerrado_flujo_documental " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_abierto_cerrado_actividad_flujo_trabajo(ByVal id_actividad As Integer,
                                                                      ByVal estado_actividad_cerrado_abierto As Integer) As String
        '-----------------------------------------------------
        'Funcion : Actualiza el estado abierto o cerrado
        'de una actividad dentro del flujo de trabajo
        'Fecha : 2017-09-25
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Parametro_Actualiza As String = "Update wf_registro_actividaes_flujos_trabajo set TIPO_ABIERTA_CERRADA_ACTIVIDAD=" & estado_actividad_cerrado_abierto &
            " where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO ='" & id_actividad & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ""
            Result = ref.SELECTION_INSERT_COMMAND(Parametro_Actualiza)
            If Result <> "YES" Then
                Actualiza_estado_abierto_cerrado_actividad_flujo_trabajo = "Error función Actualiza_estado_abierto_cerrado_actividad_flujo_trabajo " & Result
                Exit Function
            Else
                Actualiza_estado_abierto_cerrado_actividad_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_estado_abierto_cerrado_actividad_flujo_trabajo = "Inconsistencia general función Actualiza_estado_abierto_cerrado_actividad_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_abierto_cerrado_flujo_trabajo(ByVal id_flujo_trabajo As Integer,
                                                            ByVal estado_flujo_trabajo_cerrado_abierto As Integer) As String
        '-----------------------------------------------------
        'Funcion : Actualiza el estado abierto o cerrado
        'del flujo de trabajo
        'Fecha : 2017-09-25
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Parametro_Actualiza As String = "Update wf_flujos_trabajo set TIPO_RUTA_ABIERTA_CERRADA=" & estado_flujo_trabajo_cerrado_abierto &
            " where ID_WF_FLUJOS_TRABAJO ='" & id_flujo_trabajo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ""
            Result = ref.SELECTION_INSERT_COMMAND(Parametro_Actualiza)
            If Result <> "YES" Then
                Actualiza_estado_abierto_cerrado_flujo_trabajo = "Error función Actualiza_estado_abierto_cerrado_flujo_trabajo " & Result
                Exit Function
            Else
                Actualiza_estado_abierto_cerrado_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_estado_abierto_cerrado_flujo_trabajo = "Inconsistencia general función Actualiza_estado_abierto_cerrado_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Salva_archivo_file_sistem(ByRef ref_diagram As MindFusion.Diagramming.DiagramDocument,
                                       ByRef ruta_archivo As String,
                                       ByVal nombre_flujo_trabajo As String) As String
        Try
            Dim ruta_temp As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA") & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & "\")
            If Directory.Exists(ruta_temp) = False Then
                Directory.CreateDirectory(ruta_temp)
            End If
            ruta_archivo = ruta_temp & "dlc_flujo.txt"
            If File.Exists(ruta_archivo) Then
                Kill(ruta_archivo)
            End If
            'ref_diagram.SaveToString(ruta_archi, True)
            Salva_archivo_file_sistem = "YES"
        Catch ex As Exception
            Salva_archivo_file_sistem = "Inconsistencia general función Salva_archivo_file_sistem " & ex.Message
        End Try
    End Function
    Function VSS_Bytes(ByVal Path As String) As Byte()
        'Funcion Convierte archivos as formato binario
        Dim sPath As String
        Try
            sPath = Path
            Dim Ruta As New FileStream(sPath, FileMode.Open, FileAccess.Read)
            Dim Binario(CInt(Ruta.Length)) As Byte
            Ruta.Read(Binario, 0, CInt(Ruta.Length))
            Ruta.Close()
            Return Binario
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Function Agregar_flujo_trabajo_ruta_workflow(ByVal nombre_ruta_workflow As String,
                                                 ByVal nombre_flujo_trabajo As String,
                                                 ByVal descripcion_flujo_trabajo As String,
                                                 ByVal tipo_flujo_trabajo As String,
                                                 ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                                 ByRef up_date As UpdatePanel) As String
        Try
            If nombre_ruta_workflow = "" Then
                Agregar_flujo_trabajo_ruta_workflow = "Debe seleccionar el nombre de la ruta workflow al que pertenecera el flujo de trabajo"
                Exit Function
            End If
            If nombre_flujo_trabajo = "" Then
                Agregar_flujo_trabajo_ruta_workflow = "Debe informar el nombre del flujo de trabajo"
                Exit Function
            End If
            If descripcion_flujo_trabajo = "" Then
                Agregar_flujo_trabajo_ruta_workflow = "Debe informar la descripción del flujo de trabajo"
                Exit Function
            End If
            If tipo_flujo_trabajo = "" Then
                Agregar_flujo_trabajo_ruta_workflow = "Debe seleccionar el tipo de flujo de trabajo"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim id_ruta As Integer = 0
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta_workflow,
                                                                id_ruta)
            If Result <> "YES" Then
                Agregar_flujo_trabajo_ruta_workflow = Result
                Exit Function
            End If
            Dim estado_existencia As String = "YES"
            Result = Verifica_existencia_flujo_trabajo(nombre_flujo_trabajo, id_ruta, estado_existencia)
            If Result <> "YES" Then
                Agregar_flujo_trabajo_ruta_workflow = Result
                Exit Function
            End If
            If estado_existencia = "YES" Then
                Agregar_flujo_trabajo_ruta_workflow = "El flujo de trabajo " & nombre_flujo_trabajo & " se encuentra registrado en la ruta "
                Exit Function
            End If
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ""
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Agregar_flujo_trabajo_ruta_workflow = "Error formateando fecha almacenamiento Función: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            Dim tipo_flujo_trabajo_numeral As Integer = 0
            If tipo_flujo_trabajo = "Cerrado" Then
                tipo_flujo_trabajo_numeral = 1
            Else
                tipo_flujo_trabajo_numeral = 0
            End If
            Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../configuracion/dlc_flujo.vdx")
            If File.Exists(ruta_plantilla) = False Then
                Agregar_flujo_trabajo_ruta_workflow = "Imposible encontrar el archivo " & ruta_plantilla
                Exit Function
            End If
            Dim ob As New MindFusion.Diagramming.Import.VisioImporter
            Dim diagran As New MindFusion.Diagramming.DiagramDocument
            Dim digran_page As New MindFusion.Diagramming.DiagramPage
            diagran = ob.Import(ruta_plantilla)
            If diagran.Pages.Count = 0 Then
                Agregar_flujo_trabajo_ruta_workflow = "La plantilla base no tiene el formato correcto, no contiene por lo menos un diagrama "
                Exit Function
            End If
            ref_diagram.Diagram.Items.Clear()
            ref_diagram.Diagram = diagran.Pages(0)
            ref_diagram.ZoomFactor = 75
            Dim Binario As Byte() = Nothing
            Dim ruta_archivo As String = ""
            Dim string_plantilla As String = ""
            string_plantilla = ref_diagram.SaveToString(SaveToStringFormat.Base64, True)
            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim myTrans As MySqlTransaction
            Dim sqlresultinsert As Integer = 0
            ref.Returna_Conexion_Mysql(myConnection)
            Dim sql_insert As String = "insert into wf_flujos_trabajo (rutas_workflow_id_Ruta,NOMBRE_FLUJO_TRABAJO,FECHA_FLUJO,TIPO_RUTA_ABIERTA_CERRADA,DESCRIPCION_FLUJO,Archivo_Plantilla_Mindifucion)  values " &
            "(?id_ruta,?nombre_flujo_trabajo,?fecha_flujo,?tipo_ruta,?descripcion_flujo,?archivo)"
            Try
                Dim myCommand As MySqlCommand = myConnection.CreateCommand()
                myTrans = myConnection.BeginTransaction()
                myCommand.Connection = myConnection
                myCommand.Transaction = myTrans
                myCommand.CommandText = sql_insert
                myCommand.Parameters.AddWithValue("?id_ruta", id_ruta)
                myCommand.Parameters.AddWithValue("?nombre_flujo_trabajo", UCase(nombre_flujo_trabajo))
                myCommand.Parameters.AddWithValue("?fecha_flujo", date1al)
                myCommand.Parameters.AddWithValue("?tipo_ruta", tipo_flujo_trabajo_numeral)
                myCommand.Parameters.AddWithValue("?descripcion_flujo", descripcion_flujo_trabajo)
                myCommand.Parameters.AddWithValue("?archivo", string_plantilla)
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Agregar_flujo_trabajo_ruta_workflow = "Imposible crear el nuevo flujo de trabajo  "
                    myConnection.Close()
                    Exit Function
                End If
                Dim id_flujo_trabajo = myCommand.LastInsertedId
                Dim hor As String = Now
                Dim campos As String = "Crea el flujo de trabajo  con el nombre " & UCase(nombre_flujo_trabajo)
                Dim sqlforupdate As String = "INSERT INTO wf_registro_log_flujos_trabajo (DESC_OP,USER_OPER,ID_USER,DATE_TRANS,wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO" &
               ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
               "'CREA FLUJO DE TRABAJO','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") &
               "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                id_flujo_trabajo & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR DOCUMENTAL','" & campos & "')"
                myCommand.CommandText = sqlforupdate
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Agregar_flujo_trabajo_ruta_workflow = "Imposible registrar log duplicar flujo trabajo "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                HttpContext.Current.Session.Item("DR_FLUJO_SELECCIONADO") = nombre_flujo_trabajo
                HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO") = id_flujo_trabajo
                myTrans.Commit()
                myConnection.Close()
                Agregar_flujo_trabajo_ruta_workflow = "YES"
                up_date.Update()
            Catch e As Exception
                Try
                    myTrans.Rollback()
                Catch ex As MySqlException
                    If Not myTrans.Connection Is Nothing Then
                        Agregar_flujo_trabajo_ruta_workflow = "An exception of type " + ex.GetType().ToString() +
                                          " was encountered while attempting to roll back the transaction."
                        myConnection.Close()
                        Exit Function
                    End If
                End Try
                myConnection.Close()
                Agregar_flujo_trabajo_ruta_workflow = "Error General " & e.Message
                Exit Function
            End Try
        Catch ex As Exception
            Agregar_flujo_trabajo_ruta_workflow = "Inconsistencia general funcion Agregar_flujo_trabajo_ruta_workflow  " & ex.Message
        End Try
    End Function
    Function Solicita_nombres_flujos_trabajo_workflow(ByRef rutas() As String) As String
        '----------------------------------------------
        'Función : Retorna nombre de rutas workflow 
        'Fecha : 2017-07-04
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            Erase rutas
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim sql_consulta As String = "Select NOMBRE_FLUJO_TRABAJO from wf_flujos_trabajo " &
                " where ESTADO_FLUJO=1 order by NOMBRE_FLUJO_TRABAJO asc"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombres_flujos_trabajo_workflow = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombres_flujos_trabajo_workflow = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve rutas(i)
                    rutas(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Solicita_nombres_flujos_trabajo_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombres_flujos_trabajo_workflow = "Inconsistencia general función Solicita_nombres_rutas_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_flujos_trabajo_workflow_id_flujo(ByVal id_flujo As Integer,
                                                              ByVal estado_advertencia As Integer,
                                                              ByRef nombre_flujo As String) As String
        '----------------------------------------------
        'Función : Retorna nombre flujo trabajo por
        'id flujo
        'Fecha : 2020-12-10
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim sql_consulta As String = "Select NOMBRE_FLUJO_TRABAJO from wf_flujos_trabajo " &
                " where ID_WF_FLUJOS_TRABAJO=" & id_flujo
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_flujos_trabajo_workflow_id_flujo = "Error funcion Solicita_nombre_flujos_trabajo_workflow_id_flujo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If estado_advertencia = 1 Then
                    Solicita_nombre_flujos_trabajo_workflow_id_flujo = "Imposible encontrar el nombre del flujo del codigo (" & id_flujo & ")"
                    Exit Function
                Else
                    Solicita_nombre_flujos_trabajo_workflow_id_flujo = "YES"
                    Exit Function
                End If
            Else
                nombre_flujo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_flujos_trabajo_workflow_id_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_flujos_trabajo_workflow_id_flujo = "Inconsistencia general función Solicita_nombre_flujos_trabajo_workflow_id_flujo " & ex.Message
        End Try
    End Function
    Function Retorna_id_flujo_trabajo_por_id_ruta(ByVal nombre_flujo_trabajo As String,
                                                  ByVal id_ruta As Integer,
                                                  ByRef id_flujo_trabajo As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select ID_WF_FLUJOS_TRABAJO from wf_flujos_trabajo where NOMBRE_FLUJO_TRABAJO ='" & nombre_flujo_trabajo & "' and rutas_workflow_id_Ruta=" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_flujo_trabajo_por_id_ruta = "Error función Retorna_id_flujo_trabajo_por_id_ruta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_flujo_trabajo_por_id_ruta = "YES"
                Exit Function
            Else
                Retorna_id_flujo_trabajo_por_id_ruta = "Imposible encontrar el id del flujo de trabajo " & nombre_flujo_trabajo
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_flujo_trabajo_por_id_ruta = "Inconsistencia general función Retorna_id_flujo_trabajo_por_id_ruta " & ex.Message
        End Try
    End Function
    Function SolicitaNombreFlujoTrabajoPorIdFlujo(ByVal IdFlujoTrabajo As Integer,
                                                  ByRef NombreFlujoTrabajo As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el nombre del flujo de travajo con la identifiacion de flujo de trabajo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdFlujoTrabajo      : Representa la identificación del flujo de trabajo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreFlujoTrabajo  : Retorna el nombre del flujo de trabajo
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "select  NOMBRE_FLUJO_TRABAJO from wf_flujos_trabajo where ID_WF_FLUJOS_TRABAJO ='" & IdFlujoTrabajo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("nombre_flujo_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreFlujoTrabajoPorIdFlujo = "Error función SolicitaNombreFlujoTrabajoPorIdFlujo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                NombreFlujoTrabajo = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreFlujoTrabajoPorIdFlujo = "YES"
                Exit Function
            Else
                SolicitaNombreFlujoTrabajoPorIdFlujo = "No fue posible encontrar el ID del flujo de trabajo con el nombre proporcionado. " & NombreFlujoTrabajo
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreFlujoTrabajoPorIdFlujo = "Inconsistencia general función SolicitaNombreFlujoTrabajoPorIdFlujo " & ex.Message
        End Try
    End Function
    Function Abre_flujo_trabajo_ruta_workflow(ByVal nombre_flujo_trabajo As String,
                                              ByVal id_ruta As Integer,
                                              ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                              ByRef ref_update As UpdatePanel,
                                              ByRef ref_CheckBox_Grid_alineamiento As CheckBox) As String
        Try

            If nombre_flujo_trabajo = "" Then
                Abre_flujo_trabajo_ruta_workflow = "Debe seleccionar un flujo de trabajo "
                Exit Function
            End If
            Dim Result As String = ""
            '-----------------------------------------------------
            'Solicita id flujo de trabajo
            '-----------------------------------------------------
            Dim id_flujo_trabajo As Integer = 0
            Result = Me.Retorna_id_flujo_trabajo_por_id_ruta(nombre_flujo_trabajo, id_ruta, id_flujo_trabajo)
            If Result <> "YES" Then
                Abre_flujo_trabajo_ruta_workflow = Result
                Exit Function
            End If
            Dim Parametro_Consulta As String = "select Archivo_Plantilla_Mindifucion from wf_flujos_trabajo where NOMBRE_FLUJO_TRABAJO ='" &
                nombre_flujo_trabajo & "' and rutas_workflow_id_Ruta =" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Abre_flujo_trabajo_ruta_workflow = Result
                Exit Function
            End If
            Dim bDatos() As Byte = Nothing
            Dim strin_datos As String = ""
            If Datset.Tables(0).Rows.Count > 0 Then
                strin_datos = Datset.Tables(0).Rows(0).Item(0).ToString
                Dim path_temporal As String = ""
                If strin_datos = "" Then
                    Abre_flujo_trabajo_ruta_workflow = Result
                    Exit Function
                Else
                    HttpContext.Current.Session.Item("DR_FLUJO_SELECCIONADO") = nombre_flujo_trabajo
                    HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO") = id_flujo_trabajo
                    ref_diagram.Diagram.LoadFromString(strin_datos)
                    ref_diagram.ZoomFactor = 75
                    If ref_diagram.Diagram.ShowGrid = True Then
                        ref_CheckBox_Grid_alineamiento.Checked = True
                    Else
                        ref_CheckBox_Grid_alineamiento.Checked = False
                    End If
                    ref_diagram.Diagram.LinkEndsMovable = False
                    ref_update.Update()
                    Abre_flujo_trabajo_ruta_workflow = "YES"
                End If
            Else
                Abre_flujo_trabajo_ruta_workflow = "Imposible encontrar el flujo de trabajo "
                Exit Function
            End If
        Catch ex As Exception
            Abre_flujo_trabajo_ruta_workflow = "Inconsistencia general función Abre_flujo_trabajo_ruta_workflow " & ex.Message
        End Try

    End Function
    Function Abre_flujo_trabajo(ByVal id_flujo_trabajo As Integer,
                                ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                ByRef ref_update As UpdatePanel,
                                ByRef ref_CheckBox_Grid_alineamiento As CheckBox) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select Archivo_Plantilla_Mindifucion from wf_flujos_trabajo where ID_WF_FLUJOS_TRABAJO =" &
                id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Abre_flujo_trabajo = Result
                Exit Function
            End If
            Dim bDatos() As Byte = Nothing
            Dim strin_datos As String = ""
            If Datset.Tables(0).Rows.Count > 0 Then
                strin_datos = Datset.Tables(0).Rows(0).Item(0).ToString
                Dim path_temporal As String = ""
                If strin_datos = "" Then
                    Abre_flujo_trabajo = Result
                    Exit Function
                Else
                    'HttpContext.Current.Session.Item("DR_FLUJO_SELECCIONADO") = nombre_flujo_trabajo
                    '.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO") = id_flujo_trabajo
                    ref_diagram.Diagram.LoadFromString(strin_datos)
                    ref_diagram.ZoomFactor = 75
                    If ref_diagram.Diagram.ShowGrid = True Then
                        ref_CheckBox_Grid_alineamiento.Checked = True
                    Else
                        ref_CheckBox_Grid_alineamiento.Checked = False
                    End If
                    ref_diagram.Diagram.LinkEndsMovable = False
                    ref_update.Update()
                    Abre_flujo_trabajo = "YES"
                End If
            Else
                Abre_flujo_trabajo = "Imposible encontrar el flujo de trabajo "
                Exit Function
            End If
        Catch ex As Exception
            Abre_flujo_trabajo = "Inconsistencia general función Abre_flujo_trabajo " & ex.Message
        End Try

    End Function
    Function Retorna_actividades_recorridas_unicas_flujo_trabajo(ByVal id_tarea As Integer,
                                                                 id_flujo_trabajo As Integer,
                                                                 ByRef matri_id_actividades() As Integer) As String
        Try
            matri_id_actividades = Nothing
            Dim Parametro_Consulta As String = "SELECT  id_actividad_flujo_trabajo from estados_tarea_workflow where inicio_tareas_workflow_id_tarea ='" & id_tarea & "' and ID_FLUJO_TRABAJO=" & id_flujo_trabajo &
                " AND id_actividad_flujo_trabajo <> 0 order by id_Estado"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_actividades_recorridas_unicas_flujo_trabajo = "Error función Retorna_actividades_recorridas_unicas_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_id_actividades(i)
                    matri_id_actividades(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_actividades_recorridas_unicas_flujo_trabajo = "YES"
                Exit Function
            Else
                Retorna_actividades_recorridas_unicas_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_actividades_recorridas_unicas_flujo_trabajo = "Inconsistencia general función Retorna_actividades_recorridas_unicas_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Retorna_actividades_recorridas_flujo_trabajo(ByVal id_tarea As Integer,
                                                          id_flujo_trabajo As Integer,
                                                          ByRef matri_id_actividades() As stru_conector_estado) As String
        Try
            matri_id_actividades = Nothing
            Dim Parametro_Consulta As String = "SELECT id_Estado,id_actividad_flujo_trabajo from estados_tarea_workflow where inicio_tareas_workflow_id_tarea ='" _
                                               & id_tarea & "' and ID_FLUJO_TRABAJO=" & id_flujo_trabajo &
                                               " AND id_actividad_flujo_trabajo <> 0  order by id_Estado "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_actividades_recorridas_flujo_trabajo = "Error función Retorna_actividades_recorridas_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_id_actividades(i)
                    matri_id_actividades(i).id_Estado = Datset.Tables(0).Rows(i).Item(0)
                    matri_id_actividades(i).Id_Actividad = Datset.Tables(0).Rows(i).Item(1)
                Next
                Retorna_actividades_recorridas_flujo_trabajo = "YES"
                Exit Function
            Else
                Retorna_actividades_recorridas_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_actividades_recorridas_flujo_trabajo = "Inconsistencia general función Retorna_actividades_recorridas_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Marca_Actividad_flujo_trabajo_recorridas(ByVal matri_id_actividades() As Integer, ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                               ByRef ref_update As UpdatePanel)
        Try
            For Each sha_ As Object In ref_diagram.Diagram.Items
                Dim ob As Object = sha_.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    For i As Integer = 0 To matri_id_actividades.Length - 1
                        If sha_.id = matri_id_actividades(i).ToString Then
                            Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush(
                            Color.Yellow, Color.Yellow, 0)
                            sha_.Brush = penBrush
                            Exit For
                        End If
                    Next
                End If
            Next
            Marca_Actividad_flujo_trabajo_recorridas = "YES"
            ref_update.Update()
        Catch ex As Exception
            Marca_Actividad_flujo_trabajo_recorridas = "Inconsistencia general función Marca_Actividad_flujo_trabajo_recorridas " & ex.Message
        End Try
    End Function
    Function Marca_Actividad_flujo_trabajo(ByVal id_actividad_flujo_trabajo As Integer,
                                           ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                           ByRef ref_update As UpdatePanel)
        Try
            For Each sha_ As Object In ref_diagram.Diagram.Items
                Dim ob As Object = sha_.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    If sha_.id = id_actividad_flujo_trabajo.ToString Then
                        Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush(
            Color.LightSteelBlue, Color.LightSteelBlue, 0)
                        sha_.Brush = penBrush
                    End If
                End If

            Next
            Marca_Actividad_flujo_trabajo = "YES"
            ref_update.Update()
        Catch ex As Exception
            Marca_Actividad_flujo_trabajo = "Inconsistencia general función Marca_Actividad_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Bytes_VSS(ByVal Bin As Byte(),
                       ByVal nombre_flujo_trabajo As String,
                       ByRef pathTemporal As String) As String
        Try
            Dim oFileStream As FileStream
            pathTemporal = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA") & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & "\TEMPO_PLANTILLA\")
            If Directory.Exists(pathTemporal) = False Then
                Directory.CreateDirectory(pathTemporal)
            End If
            pathTemporal = pathTemporal & nombre_flujo_trabajo
            If File.Exists(pathTemporal) Then
                File.Delete(pathTemporal)
            End If
            oFileStream = New FileStream(pathTemporal, FileMode.CreateNew)
            oFileStream.Write(Bin, 0, Bin.Length)
            oFileStream.Close()
            oFileStream = Nothing
            Bytes_VSS = "YES"
        Catch ex As Exception
            Bytes_VSS = ex.Message
        End Try
    End Function
    Function Lista_nombre_flujos_trabajo_interface(ByVal flujos_trabajo_workflow() As String,
                                                   ByRef ref_droplis As DropDownList,
                                                   ByRef ref_update As UpdatePanel,
                                                   Optional ByVal estado_limpia As Integer = 0) As String
        '----------------------------------------------
        'Función : Lista el nombre de rutas disponibles
        'en la interface
        'Fecha : 2017-06-29
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            If estado_limpia = 0 Then
                ref_droplis.Items.Clear()
            End If
            If Not flujos_trabajo_workflow Is Nothing Then
                For i As Integer = 0 To flujos_trabajo_workflow.Length - 1
                    ref_droplis.Items.Add(flujos_trabajo_workflow(i))
                Next
                ref_update.Update()
                Lista_nombre_flujos_trabajo_interface = "YES"
            Else
                ref_update.Update()
                Lista_nombre_flujos_trabajo_interface = "YES"
            End If
        Catch ex As Exception
            Lista_nombre_flujos_trabajo_interface = "Inconsistencia general función Lista_nombre_flujos_trabajo_interface " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_flujo_trabajo(ByVal nombre_flujo_trabajo As String,
                                               ByVal id_ruta_workflow As Integer,
                                               ByRef estado_existencia As String) As String
        '-----------------------------------------------------------
        'Funcion : Verifica existencia del flujo de trabajo en la
        'ruta de trabajo
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2017-09-07
        '-----------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Sql_consulta As String = "Select rutas_workflow_id_Ruta from wf_flujos_trabajo WHERE rutas_workflow_id_Ruta = '" & id_ruta_workflow & "'" _
            & " AND NOMBRE_FLUJO_TRABAJO = '" & nombre_flujo_trabajo & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_flujo_trabajo = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia = "NO"
                Verifica_existencia_flujo_trabajo = "YES"
                Exit Function
            Else
                estado_existencia = "YES"
                Verifica_existencia_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_flujo_trabajo = "Inconsistencia general función Verifica_existencia_flujo_trabajo " & ex.Message
        End Try

    End Function
    Function Solicita_listado_actividades_flujo_trabajo_usuario(ByVal id_ruta As Integer,
                                                                ByVal tipo_consulta As Integer,
                                                                ByVal valor_consulta As String,
                                                                ByRef colum_order_name As String,
                                                                ByRef order_colum As String,
                                                                ByRef grediview As GridView,
                                                                ByRef reflabel As Label,
                                                                ByRef hideselecion As Object,
                                                                ByRef update As UpdatePanel) As String
        Try
            Dim Sql_consulta As String = ""
            If tipo_consulta = 1 Then
                Sql_consulta = "Select idU_suario,Nombre_Usuario as NOMBRE_USUARIO,Cargo_Usuario as CARGO_USUARIO, Area_Usuario AS AREA from usuario_workflow as law " &
               " WHERE Grupos_Workflow_Rutas_Workflow_id_Ruta=" & id_ruta & " ORDER BY Nombre_Usuario"
            Else
                Sql_consulta = "Select idU_suario,Nombre_Usuario as NOMBRE_USUARIO,Cargo_Usuario as CARGO_USUARIO, Area_Usuario AS AREA from usuario_workflow as law " &
               " WHERE (Nombre_Usuario like '%" & valor_consulta & "%'" & " or Cargo_Usuario like '%" & valor_consulta & "%') and " & " Grupos_Workflow_Rutas_Workflow_id_Ruta=" _
               & id_ruta & " ORDER BY Nombre_Usuario"
            End If

            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_actividades_flujo_trabajo_usuario = "Error listando descripción tabla listado_actividades_workflow  " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "Se encontraron 0 registro (s) de usuario (s)"
                grediview.DataSource = Nothing
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                Solicita_listado_actividades_flujo_trabajo_usuario = "YES"
                Exit Function
            Else
                reflabel.Text = Datset.Tables(0).Rows.Count & " registro (s) de usuario (s)"
                grediview.DataSource = Datset
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    'grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                    'Dim imaga_buton As New HtmlInputImage
                    'imaga_buton.Attributes.Add("CssClass", "image_buton_clik_image")
                    'imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    'imaga_buton.Src = "../imagewf/ACEPTAR.png"
                    ''imaga_buton.ID = grediview.Rows(i).Cells(0).Text.ToString()
                    'imaga_buton.Attributes.Add("idd", grediview.Rows(i).Cells(0).Text.ToString())
                    'grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count - 1).Controls.Add(imaga_buton)
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-arrow-to-bottom")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Asigna radicado")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "a_s_r_p_333")
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
                Solicita_listado_actividades_flujo_trabajo_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_actividades_flujo_trabajo_usuario = "Inconsistencia general función Solicita_listado_actividades_flujo_trabajo_usuario " & ex.Message
        End Try

    End Function
    Function Solicita_id_actividad_ruta_id_flujo_trabajo(ByVal id_actividad_flujo_trabajo As Integer,
                                                         ByRef id_actividad As Integer) As String
        '-------------------------------------------------------------------------
        'Función : Retorna el id actividad de la actividad del flujo documental
        'Fecha : 2017-09-28
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select listado_actividades_workflow_Id_Actividad from wf_registro_actividaes_flujos_trabajo  " &
              " where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_actividad_ruta_id_flujo_trabajo = "Error listando descripción tabla Solicita_id_actividad_ruta_id_flujo_trabajo  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_actividad_ruta_id_flujo_trabajo = "Imposible encontrar el tipo de actividad del flujo de trabajo " & id_actividad_flujo_trabajo
                Exit Function
            Else
                id_actividad = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_actividad_ruta_id_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_actividad_ruta_id_flujo_trabajo = "Inconsistencia Solicita_id_actividad_ruta_id_flujo_trabajo " & ex.Message
        End Try
    End Function


    Function Solicita_tipo_actividad_general_workflow_usuario(ByVal nombre_tipo_actividad As String, ByRef id_tipo_actividad As Integer) As String
        '-------------------------------------------------------------------------
        'Función : Retorna el tipo de actividad de usuario
        'Fecha : 2017-09-18
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select Id_Actividad_General from actividades_generales_workflow " &
               "  where Tipo_Actividad='" & nombre_tipo_actividad & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_generales_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipo_actividad_general_workflow_usuario = "Error listando descripción tabla actividades_generales_workflow  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipo_actividad_general_workflow_usuario = "Imposible encontrar el id tipo de actividad  " & nombre_tipo_actividad
                Exit Function
            Else
                id_tipo_actividad = Datset.Tables(0).Rows(0).Item(0)
                Solicita_tipo_actividad_general_workflow_usuario = "YES"
            End If
        Catch ex As Exception
            Solicita_tipo_actividad_general_workflow_usuario = "Inconsistencia general función Solicita_tipo_actividad_general_workflow " & ex.Message
        End Try
    End Function

    Function Agregar_actividad_flujo_de_trabajo(ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView,
                                                ByRef UpdatePanel_diagran_view As UpdatePanel,
                                                ByVal id_actividad_ruta As Integer,
                                                ByVal id_flujo_trabajo As Integer) As String

        Dim Result As String = ""
        Dim Tipo_actividad As String = ""
        Dim id_tipo_actividad As Integer = 0
        Dim id_agrupacion_actividad As Integer = 0
        Dim nombre_tipo_actividad As String = ""
        Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
        Result = Class_Listado_Actividades_workflow.Solicita_tipo_actividad_general_workflow(id_actividad_ruta,
                                                                                             Tipo_actividad,
                                                                                             id_tipo_actividad,
                                                                                             id_agrupacion_actividad,
                                                                                             nombre_tipo_actividad)
        If Result <> "YES" Then
            Agregar_actividad_flujo_de_trabajo = Result
            Exit Function
        End If
        Dim Class_actividades_generales_workflow As New Class_actividades_generales_workflow
        Dim actividades_generales_workflow_ As actividades_generales_workflow = Nothing
        Result = Class_actividades_generales_workflow.Solicita_estructura_tipo_actividad_workflow(id_tipo_actividad,
                                                                                                  actividades_generales_workflow_)
        If Result <> "YES" Then
            Agregar_actividad_flujo_de_trabajo = Result
            Exit Function
        End If
        If actividades_generales_workflow_.Nombre_tipo_actividad = "ENLACE" Then
            Tipo_actividad = "ENLASE"
        Else
            Tipo_actividad = actividades_generales_workflow_.Nombre_tipo_actividad
        End If
        Dim nombre_actividad As String = ""
        Result = Class_Listado_Actividades_workflow.Retorna_Nombre_Actividad_id_actividad(id_actividad_ruta,
                                                                                          nombre_actividad)
        If Result <> "YES" Then
            Agregar_actividad_flujo_de_trabajo = Result
            Exit Function
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim Fecha_Format As String = Now.ToString
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(Fecha_Format)
        If Result <> "YES" Then
            Agregar_actividad_flujo_de_trabajo = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Dim Sql_Insercion_actividad As String = "Insert into wf_registro_actividaes_flujos_trabajo (wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO," &
            "listado_actividades_workflow_Id_Actividad,FECHA_REGISTRO,ID_TIPO_ACTIVIDAD) values " &
            "(" & id_flujo_trabajo & "," & id_actividad_ruta & ",'" & Fecha_Format & "'" & "," & id_tipo_actividad & ")"
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_Insercion_actividad
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Agregar_actividad_flujo_de_trabajo = "Imposible agregar nueva actividad  "
                myConnection.Close()
                Exit Function
            End If
            Dim identificador As Object = myCommand.LastInsertedId
            '----------------------------------
            'Agrega el shape al diagrama
            '----------------------------------
            Dim string_digram As String = ""
            Result = Me.Agrega_shape_flujo_trabajo(DiagramView,
                                                   UpdatePanel_diagran_view,
                                                   Tipo_actividad,
                                                   identificador,
                                                   nombre_actividad,
                                                   id_flujo_trabajo,
                                                   0,
                                                   string_digram)
            If Result <> "YES" Then
                Agregar_actividad_flujo_de_trabajo = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------
            'Guarda el archivo en la base de datos
            '--------------------------------------
            Dim sql_atualiza_ruta As String = "update wf_flujos_trabajo set Archivo_Plantilla_Mindifucion = ?imagen  where " _
           & "ID_WF_FLUJOS_TRABAJO =" & id_flujo_trabajo
            myCommand.CommandText = sql_atualiza_ruta
            myCommand.Parameters.AddWithValue("?imagen", string_digram)
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Agregar_actividad_flujo_de_trabajo = "Imposible actualiza flujo de trabajo  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Agregar_actividad_flujo_de_trabajo = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Agregar_actividad_flujo_de_trabajo = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Agregar_actividad_flujo_de_trabajo = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Agregar_actividad_usuario_flujo_de_trabajo(ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView,
                                                        ByRef UpdatePanel_diagran_view As UpdatePanel,
                                                        ByVal id_usuario_workflow As Integer,
                                                        ByVal id_flujo_trabajo As Integer) As String

        Dim Result As String = ""
        Dim id_grupo As Integer = 0
        Dim ClassGestionFechas As New ClassGestionFechas
        Result = Me.Retorna_id_grupo_usuario_workflow(id_usuario_workflow, id_grupo)
        If Result <> "YES" Then
            Agregar_actividad_usuario_flujo_de_trabajo = Result & " Imposible agregar la actividad de usuario al flujo documental"
            Exit Function
        End If
        Dim id_actividad As Integer = 0
        Dim nombre_actividad As String = ""
        Dim Ref_class_listado As New Class_Listado_Actividades_workflow
        Result = Ref_class_listado.Retorna_actividad_grupo_workflow(id_grupo,
                                                                    id_actividad,
                                                                    nombre_actividad)
        If Result <> "YES" Then
            Agregar_actividad_usuario_flujo_de_trabajo = Result & " Imposible agregar la actividad de usuario al flujo documental"
            Exit Function
        End If
        '---------------------------------------------
        'Retorna id tipo actividad
        '---------------------------------------------
        Dim Nombre_Tipo_actividad As String = "USUARIOINDIVIDUAL"
        Dim id_tipo_actividad As Integer = 0
        Result = Me.Solicita_tipo_actividad_general_workflow_usuario(Nombre_Tipo_actividad, id_tipo_actividad)
        If Result <> "YES" Then
            Agregar_actividad_usuario_flujo_de_trabajo = Result & " Imposible agregar la actividad de usuario al flujo documental"
            Exit Function
        End If
        Dim nombre_usuario As String = ""
        Dim cargo_usuario As String = ""
        Result = Retorna_Nombre_Cargo_Usuario_Workflow(id_usuario_workflow, nombre_usuario, cargo_usuario)
        If Result <> "YES" Then
            Agregar_actividad_usuario_flujo_de_trabajo = Result & " Imposible agregar la actividad de usuario al flujo documental"
            Exit Function
        End If
        nombre_actividad = nombre_usuario & "(" & cargo_usuario & ")"
        Dim fecha_actual As Object = Date.Now
        Result = ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(fecha_actual)
        If Result <> "YES" Then
            Agregar_actividad_usuario_flujo_de_trabajo = Result
            Exit Function
        End If

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Dim Sql_Insercion_actividad As String = "Insert into wf_registro_actividaes_flujos_trabajo (wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO," &
            "listado_actividades_workflow_Id_Actividad,FECHA_REGISTRO,ID_TIPO_ACTIVIDAD,ID_USUARIO_WORKFLOW) values " &
            "(" & id_flujo_trabajo & "," & id_actividad & ",'" & fecha_actual & "'" & "," & id_tipo_actividad & "," & id_usuario_workflow & ")"
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_Insercion_actividad
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Agregar_actividad_usuario_flujo_de_trabajo = "Imposible agregar nueva actividad  "
                myConnection.Close()
                Exit Function
            End If
            Dim identificador As Object = myCommand.LastInsertedId
            '----------------------------------
            'Agrega el shape al diagrama
            '----------------------------------
            Dim string_digram As String = ""
            Result = Me.Agrega_shape_flujo_trabajo(DiagramView,
                                                   UpdatePanel_diagran_view,
                                                   Nombre_Tipo_actividad,
                                                   identificador,
                                                   nombre_actividad,
                                                   id_flujo_trabajo,
                                                   0,
                                                   string_digram)
            If Result <> "YES" Then
                Agregar_actividad_usuario_flujo_de_trabajo = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------
            'Guarda el archivo en la base de datos
            '--------------------------------------
            Dim sql_atualiza_ruta As String = "update wf_flujos_trabajo set Archivo_Plantilla_Mindifucion = ?imagen  where " _
           & "ID_WF_FLUJOS_TRABAJO =" & id_flujo_trabajo
            myCommand.CommandText = sql_atualiza_ruta
            myCommand.Parameters.AddWithValue("?imagen", string_digram)
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Agregar_actividad_usuario_flujo_de_trabajo = "Imposible actualiza flujo de trabajo  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Agregar_actividad_usuario_flujo_de_trabajo = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Agregar_actividad_usuario_flujo_de_trabajo = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Agregar_actividad_usuario_flujo_de_trabajo = "Error General " & e.Message
            Exit Function
        End Try
    End Function

    Function Agrega_shape_flujo_trabajo(ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView,
                                        ByRef UpdatePanel_diagran_view As UpdatePanel,
                                        ByVal nombre_tipo_actividad As String,
                                        ByVal identificador As Integer,
                                        ByVal nombre_actividad As String,
                                        ByVal id_flujo_trabajo As Integer,
                                        ByVal option_guarda_tabla As Integer,
                                        ByRef string_digram As String) As String
        '---------------------------------------------------------
        'Función : Agrega un shape a la interface del diagrama      
        'Fecha : 2017-07-27
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------
        Try
            Dim sap As New MindFusion.Diagramming.ShapeNode
            'sap.AllowOutgoingLinks = False
            'Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            'Dim Fill = New MindFusion.Drawing.SolidBrush(Color.White)
            'sap.PolygonalTextLayout = True
            'sap.EnableStyledText = True
            Dim x As Integer = 0
            Dim y As Integer = 0
            '-------------------------------------------
            'Posiciona el shape en el grid del diagrama
            '-------------------------------------------
            x = DiagramView.Diagram.Bounds.Width / 2
            y = (DiagramView.Diagram.Bounds.Height / 2) - 100
            Dim Rect = New RectangleF(x, y, 25, 25)
            sap.Bounds = Rect
            'sap.Text = UCase(nombre_actividad)
            sap.Id = identificador
            '-----------------------------------
            'Aplica estilo al shape
            '-----------------------------------
            Dim Refclas_flujo_ruta As New Class_worflow_rutas
            Dim Result As String = ""
            Result = Refclas_flujo_ruta.Aplica_estilo_shape_add(sap,
                                                                nombre_tipo_actividad,
                                                                nombre_actividad)
            If Result <> "YES" Then
                Agrega_shape_flujo_trabajo = "Imposible aplicar el estilo al shape, mensaje " & Result
                Exit Function
            End If
            DiagramView.Diagram.Items.Add(sap)
            Dim Ruta_archivo_guardado As String = ""
            '--------------------------------------------------
            'Guarda el archivo en el sistema de archivo
            '--------------------------------------------------
            Dim string_diagrama As String = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            string_digram = string_diagrama
            '-------------------------------------------------
            'Guarda el archivo en la base de datos
            '-------------------------------------------------
            If option_guarda_tabla = 1 Then
                Result = Me.Guarda_archivo_base_datos_flujo_trabajo_string(string_diagrama, id_flujo_trabajo)
                If Result <> "YES" Then
                    DiagramView.Diagram.Items.Remove(sap)
                    Agrega_shape_flujo_trabajo = Result
                    Exit Function
                End If
            End If

            UpdatePanel_diagran_view.Update()
            Agrega_shape_flujo_trabajo = "YES"
        Catch ex As Exception
            Agrega_shape_flujo_trabajo = "Inconsistencia general función Agrega_shape_ruta_worokflow " & ex.Message
        End Try

    End Function
    Function Agrega_shape_flujo_trabajo_trazabilidad(ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView,
                                                     ByRef UpdatePanel_diagran_view As UpdatePanel,
                                                     ByVal nombre_tipo_actividad As String,
                                                     ByVal identificador As Integer,
                                                     ByVal nombre_actividad As String,
                                                     ByVal id_flujo_trabajo As Integer,
                                                     ByVal contador As Integer,
                                                     ByVal cargo As String) As String
        '---------------------------------------------------------
        'Función : Agrega un shape a la interface del diagrama      
        'Fecha : 2017-07-27
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------
        Try
            Dim sap As New MindFusion.Diagramming.ShapeNode
            'sap.AllowOutgoingLinks = False
            'Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            'Dim Fill = New MindFusion.Drawing.SolidBrush(Color.White)
            'sap.PolygonalTextLayout = True
            'sap.EnableStyledText = True
            Dim x As Integer = 0
            Dim y As Integer = 0
            '-------------------------------------------
            'Posiciona el shape en el grid del diagrama
            '-------------------------------------------
            x = (40 * contador) + 10
            y = (DiagramView.Diagram.Bounds.Height / 2) - 100
            Dim Rect = New RectangleF(x, y, 25, 35)
            sap.Bounds = Rect
            'sap.Text = UCase(nombre_actividad)
            sap.Id = identificador
            sap.Locked = True
            '-----------------------------------
            'Aplica estilo al shape
            '-----------------------------------
            Dim Refclas_flujo_ruta As New Class_worflow_rutas
            Dim Result As String = ""
            Result = Refclas_flujo_ruta.Aplica_estilo_shape_add_trazabilidad(sap,
                                                                             nombre_tipo_actividad,
                                                                             nombre_actividad,
                                                                             cargo)
            If Result <> "YES" Then
                Agrega_shape_flujo_trabajo_trazabilidad = "Imposible aplicar el estilo al shape, mensaje " & Result
                Exit Function
            End If
            DiagramView.Diagram.Items.Add(sap)
            UpdatePanel_diagran_view.Update()
            Agrega_shape_flujo_trabajo_trazabilidad = "YES"
        Catch ex As Exception
            Agrega_shape_flujo_trabajo_trazabilidad = "Inconsistencia general función Agrega_shape_ruta_worokflow " & ex.Message
        End Try
    End Function

    Function Retorna_id_grupo_usuario_workflow(ByVal id_usuario_workflow As Integer, ByRef id_grupo_usuario_workflow As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select Grupos_Workflow_Id_Grupo from usuario_workflow where idU_suario=" & id_usuario_workflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_grupo_usuario_workflow = "Función Retorna_id_grupo_usuario_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    Retorna_id_grupo_usuario_workflow = "El usuario no tiene grupo asignado"
                    Exit Function
                Else
                    id_grupo_usuario_workflow = Datset.Tables(0).Rows(0).Item(0)
                    Retorna_id_grupo_usuario_workflow = "YES"
                    Exit Function
                End If
            Else
                Retorna_id_grupo_usuario_workflow = "Imposible encontrar el usuario en la base de datos, id usuario (" & id_usuario_workflow & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_grupo_usuario_workflow = "Inconsistencia general función Retorna_id_grupo_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Retorna_id_actividad_relacionada_actividad_flujo_trabajo(ByVal id_actividad_flujo_trabajo As Integer, ByVal id_flujo_trabajo As Integer, ByRef id_actividad As Integer) As String
        '--------------------------------------------------------------------------
        'Función : Retorna la id actvidad relacionada con la actividad registrada
        'en el flujo de trabajo
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-09-20
        '---------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select listado_actividades_workflow_Id_Actividad from wf_registro_actividaes_flujos_trabajo where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad_flujo_trabajo &
                " and wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_actividad_relacionada_actividad_flujo_trabajo = "Función Retorna_id_actividad_relacionada_actividad_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_actividad_relacionada_actividad_flujo_trabajo = "Imposible encontrar el id_actvidad de la actividad del flujo de trabajo numero " & id_actividad_flujo_trabajo
                Exit Function
            Else
                id_actividad = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_actividad_relacionada_actividad_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_actividad_relacionada_actividad_flujo_trabajo = "Inconsistencia general función Retorna_id_actividad_relacionada_actividad_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Retorna_id_usuario_workflow_actividad_flujo_trabajo(ByVal id_actividad_flujo_trabajo As Integer, ByVal id_flujo_trabajo As Integer, ByRef id_usuario_workflow As Integer) As String
        '--------------------------------------------------------------------------
        'Función : Retorna el id ussuario workflow relacionado a la actividad
        'en el flujo de trabajo
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-09-20
        '---------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_USUARIO_WORKFLOW from wf_registro_actividaes_flujos_trabajo where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad_flujo_trabajo &
                " and wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_usuario_workflow_actividad_flujo_trabajo = "Función Retorna_id_usuario_workflow_actividad_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_usuario_workflow_actividad_flujo_trabajo = "Imposible encontrar el id_actividad de la actividad del flujo de trabajo numero " & id_actividad_flujo_trabajo
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_usuario_workflow = 0
                    Retorna_id_usuario_workflow_actividad_flujo_trabajo = "YES"
                    Exit Function
                Else
                    id_usuario_workflow = Datset.Tables(0).Rows(0).Item(0)
                    Retorna_id_usuario_workflow_actividad_flujo_trabajo = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Retorna_id_usuario_workflow_actividad_flujo_trabajo = "Inconsistencia general función Retorna_id_usuario_workflow_actividad_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Retorna_Nombre_Cargo_Usuario_Workflow(ByVal id_usuario_workflow As Integer, ByRef nombre_usuario As String, ByRef cargo_usuario As String) As String
        Try
            Dim Parametro_Consulta As String = "select Nombre_Usuario,Cargo_Usuario from usuario_workflow where idU_suario=" & id_usuario_workflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Nombre_Cargo_Usuario_Workflow = "Función Retorna_Nombre_Cargo_Usuario_Workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_usuario = Datset.Tables(0).Rows(0).Item(0)
                cargo_usuario = Datset.Tables(0).Rows(0).Item(1)
                Retorna_Nombre_Cargo_Usuario_Workflow = "YES"
                Exit Function
            Else
                Retorna_Nombre_Cargo_Usuario_Workflow = "Imposible encontrar el usuario en la base de datos, id usuario (" & id_usuario_workflow & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Nombre_Cargo_Usuario_Workflow = "Inconsistencia general función Retorna_Nombre_Cargo_Usuario_Workflow "
        End Try
    End Function
    Function Guarda_archivo_base_datos_flujo_trabajo_string(ByVal string_archivo As String, ByVal id_flujo_trabajo As Integer) As String
        Try

            Dim Result As String = ""
            Dim sql_atualiza_ruta As String = "update wf_flujos_trabajo set Archivo_Plantilla_Mindifucion = ?imagen  where " _
            & "ID_WF_FLUJOS_TRABAJO =" & id_flujo_trabajo
            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim sqlresultinsert As Integer = 0
            ref.Returna_Conexion_Mysql(myConnection)
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myCommand.Connection = myConnection
            myCommand.CommandText = sql_atualiza_ruta
            myCommand.Parameters.AddWithValue("?imagen", string_archivo)
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Guarda_archivo_base_datos_flujo_trabajo_string = "Imposible actualizar archivo flujo trabajo en la base de datos  "
                myConnection.Close()
                Exit Function
            End If
            myConnection.Close()
            Guarda_archivo_base_datos_flujo_trabajo_string = "YES"
        Catch ex As Exception
            Guarda_archivo_base_datos_flujo_trabajo_string = "Inconsistencia general función Guarda_archivo_base_datos " & ex.Message
        End Try
    End Function
    Function Guardar_flujo_trabajo_workflow(ByVal DiagramView As MindFusion.Diagramming.WebForms.DiagramView, ByVal id_flujo_trabajo As Integer) As String
        '---------------------------------------------------------
        'Función : Guarda el archivo y actualiza el flujo
        'en la base de datos
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-09-19
        '---------------------------------------------------------
        Try
            Dim Ruta_archivo_guardado As String = ""
            Dim id_ruta As Integer = 0
            Dim Result As String = ""

            '--------------------------------------------------
            'Guarda el archivo en el sistema de archivo
            '--------------------------------------------------
            Dim string_diagrama As String = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            '-------------------------------------------------
            'Guarda el archivo en la base de datos
            '-------------------------------------------------
            Result = Me.Guarda_archivo_base_datos_flujo_trabajo_string(string_diagrama, id_flujo_trabajo)
            If Result <> "YES" Then
                Guardar_flujo_trabajo_workflow = Result
                Exit Function
            End If
            Guardar_flujo_trabajo_workflow = "YES"
        Catch ex As Exception
            Guardar_flujo_trabajo_workflow = "Inconsistencia general función Guardar_ruta_workflow " & ex.Message
        End Try

    End Function
    Function Crear_conexion_actividades_flujo_trabajo_workflow(
                                                                 ByVal id_flujo_trabajo As Integer,
                                                                 ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView,
                                                                 ByRef UpdatePanel_diagran_view As UpdatePanel) As String
        Dim HiddenField_value_selecion As Object = Nothing
        HiddenField_value_selecion = UpdatePanel_diagran_view.FindControl("HiddenField_value_selecion")
        If HiddenField_value_selecion Is Nothing Then
            Crear_conexion_actividades_flujo_trabajo_workflow = "Imposible encontrar el control selección HiddenField_value_selecion"
            Exit Function
        End If
        Dim id_actividad_fuente As Integer = 0
        Dim id_actividad_destino As Integer = 0
        Dim Result As String = ""
        Dim sha As Object = Nothing
        Dim Matshape() As Object
        Erase Matshape
        If HiddenField_value_selecion.Value = "" Then
            Crear_conexion_actividades_flujo_trabajo_workflow = "Debe seleccionar dos actividades en el diagrama como mínimo para conectar "
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count = 0 Then
            Crear_conexion_actividades_flujo_trabajo_workflow = "Debe seleccionar dos actividades en el diagrama como mínimo para conectar "
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count = 1 Then
            Crear_conexion_actividades_flujo_trabajo_workflow = "Debe seleccionar dos actividades en el diagrama como mínimo para conectar "
            Exit Function
        End If
        Dim split() As String = HiddenField_value_selecion.Value.Split("|")
        For i As Integer = 0 To split.Length - 1
            If split(0) = split(1) Then
                Crear_conexion_actividades_flujo_trabajo_workflow = "La actividad destino no puede ser la misma actividad de inicio "
                Exit Function
            End If
            For Each sha_ As Object In DiagramView.Diagram.Selection.Items
                Dim ob As Object = sha_.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    If sha_.id = split(i) Then
                        ReDim Preserve Matshape(i)
                        Matshape(i) = sha_
                    End If
                End If

            Next
        Next
        If Matshape Is Nothing Then
            Crear_conexion_actividades_flujo_trabajo_workflow = "Debe seleccionar dos actividades como mínimo para conectar "
            Exit Function
        End If
        If Matshape.Length = 1 Then
            Crear_conexion_actividades_flujo_trabajo_workflow = "Debe seleccionar dos actividades como mínimo para conectar "
            Exit Function
        End If
        If Matshape.Length > 2 Then
            Crear_conexion_actividades_flujo_trabajo_workflow = "Solo  debe seleccionar dos actividades al mismo tiempo para conectar "
            Exit Function
        End If
        Dim id_actividad_flujo_destino As Integer = 0
        Dim id_actividad_flujo_fuente As Integer = 0
        id_actividad_flujo_destino = Val(Matshape(1).id)
        id_actividad_flujo_fuente = Val(Matshape(0).id)
        '-------------------------------------
        'Retrona id actividad destino
        '--------------------------------------
        Result = Me.Retorna_id_actividad_relacionada_actividad_flujo_trabajo(id_actividad_flujo_destino, id_flujo_trabajo, id_actividad_destino)
        If Result <> "YES" Then
            Crear_conexion_actividades_flujo_trabajo_workflow = Result
            Exit Function
        End If
        '--------------------------------------
        'Retorna id actividad fuente
        '--------------------------------------
        Result = Me.Retorna_id_actividad_relacionada_actividad_flujo_trabajo(id_actividad_flujo_fuente, id_flujo_trabajo, id_actividad_fuente)
        If Result <> "YES" Then
            Crear_conexion_actividades_flujo_trabajo_workflow = Result
            Exit Function
        End If
        '---------------------------------------------
        'Retorna id usuario workflow actividad fuente
        '---------------------------------------------
        Dim id_usuario_workflow_fuente As Integer = 0
        Result = Retorna_id_usuario_workflow_actividad_flujo_trabajo(id_actividad_flujo_fuente, id_flujo_trabajo, id_usuario_workflow_fuente)
        If Result <> "YES" Then
            Crear_conexion_actividades_flujo_trabajo_workflow = Result
            Exit Function
        End If
        Dim Ref_id_usuario_workflow_fuente As Object
        If id_usuario_workflow_fuente = 0 Then
            Ref_id_usuario_workflow_fuente = "Null"
        Else
            Ref_id_usuario_workflow_fuente = id_usuario_workflow_fuente
        End If
        '---------------------------------------------
        'Retorna id usuario workflow destino
        '---------------------------------------------
        Dim id_usuario_workflow_destino As Integer = 0
        Result = Retorna_id_usuario_workflow_actividad_flujo_trabajo(id_actividad_flujo_destino, id_flujo_trabajo, id_usuario_workflow_destino)
        If Result <> "YES" Then
            Crear_conexion_actividades_flujo_trabajo_workflow = Result
            Exit Function
        End If
        Dim Ref_id_usuario_workflow_destino As Object
        If id_usuario_workflow_destino = 0 Then
            Ref_id_usuario_workflow_destino = "Null"
        Else
            Ref_id_usuario_workflow_destino = id_usuario_workflow_destino
        End If
        '-------------------------------------
        'Verfica la existencia conector
        '-------------------------------------
        Result = Me.Verifica_existencia_conector_actividades_flujo_trabajo(id_actividad_flujo_fuente, id_actividad_flujo_destino, id_flujo_trabajo)
        If Result <> "YES" Then
            Crear_conexion_actividades_flujo_trabajo_workflow = Result
            Exit Function
        End If
        Dim Sql_Insercion As String = "insert into wf_registro_conectores_actividades_envio_flujo_trabajo (ID_ACTIVIDAD_DESTINO," _
           & " ID_ACTIVIDAD_FUENTE, wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE,IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,ID_USUARIO_WORKFLOW_FUENTE,ID_USUARIO_WORKFLOW_DESTINO) values " _
           & "(" & id_actividad_destino & "," & id_actividad_fuente & "," _
            & id_flujo_trabajo & "," & id_actividad_flujo_fuente & "," & id_actividad_flujo_destino & "," & Ref_id_usuario_workflow_fuente & "," & Ref_id_usuario_workflow_destino & " )"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_Insercion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Crear_conexion_actividades_flujo_trabajo_workflow = "Imposible crear conector de actividad  "
                myConnection.Close()
                Exit Function
            End If
            Dim ident = myCommand.LastInsertedId
            Dim link As MindFusion.Diagramming.DiagramLink
            link = DiagramView.Diagram.Factory.CreateDiagramLink(Matshape(0), Matshape(1))
            'link.Text = Matshape(0).Text & "->" & Matshape(1).text
            link.AutoSnapToNode = False
            'EVITA QUE SE MUEVA EL CONECTOR FINAL
            link.AllowMoveEnd = False
            link.AllowMoveStart = False
            'link.DrawCrossings = False
            link.CrossingRadius = 1
            link.AutoRoute = True
            'link.DrawCrossings = True
            link.Id = id_flujo_trabajo.ToString & "_" & ident
            link.Text = ident
            Dim Fill = New MindFusion.Drawing.SolidBrush(Color.Yellow)
            link.HeadBrush = Fill
            Dim Ruta_archivo_guardado As String = ""
            Dim string_plantilla As String = ""
            string_plantilla = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            '-------------------------------------------------
            'Guarda el archivo en la base de datos
            '-------------------------------------------------
            Dim sql_atualiza_ruta As String = "update wf_flujos_trabajo set Archivo_Plantilla_Mindifucion = ?imagen  where " _
          & "ID_WF_FLUJOS_TRABAJO =" & id_flujo_trabajo
            myCommand.CommandText = sql_atualiza_ruta
            myCommand.Parameters.AddWithValue("?imagen", string_plantilla)
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                HiddenField_value_selecion.Value = ""
                DiagramView.Diagram.Selection.Clear()
                If DiagramView.Diagram.UndoManager.History.Commands.Count > 0 Then
                    DiagramView.Diagram.UndoManager.Undo()
                End If
                UpdatePanel_diagran_view.Update()
                Crear_conexion_actividades_flujo_trabajo_workflow = "Imposible actualiza flujo de trabajo  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            DiagramView.Diagram.Selection.Clear()
            HiddenField_value_selecion.Value = ""
            UpdatePanel_diagran_view.Update()
            myTrans.Commit()
            myConnection.Close()
            Crear_conexion_actividades_flujo_trabajo_workflow = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Crear_conexion_actividades_flujo_trabajo_workflow = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Crear_conexion_actividades_flujo_trabajo_workflow = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Elimina_actividad_Flujo_trabajo_workflow(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
            ByRef ref_update As UpdatePanel, ByVal nombre_ruta As String, ByRef shape As MindFusion.Diagramming.ShapeNode,
            ByVal id_flujo_trabajo As Integer) As String
        If shape.Id Is Nothing Then
            Elimina_actividad_Flujo_trabajo_workflow = "El id del shape se ecuentra en estado nothing "
            Exit Function
        End If

        Dim id_actividad As Integer = shape.Id
        '----------------------------------------
        'Función retorna id ruta
        '----------------------------------------
        Dim Result As String = ""
        Dim estado_existencia_conexion_actividad As String = "YES"
        Result = Me.Verifica_existencia_conexion_como_destino_flujo_trabajo(id_actividad, id_flujo_trabajo, estado_existencia_conexion_actividad)
        If Result <> "YES" Then
            Elimina_actividad_Flujo_trabajo_workflow = Result
            Exit Function
        End If
        If estado_existencia_conexion_actividad = "YES" Then
            Elimina_actividad_Flujo_trabajo_workflow = "La actividad esta conectada a otra actividad como actividad fuente, imposible eliminar "
            Exit Function
        End If
        Result = Me.Verifica_existencia_conexion_como_fuente_flujo_trabajo(id_actividad, id_flujo_trabajo, estado_existencia_conexion_actividad)
        If Result <> "YES" Then
            Elimina_actividad_Flujo_trabajo_workflow = Result
            Exit Function
        End If
        If estado_existencia_conexion_actividad = "YES" Then
            Elimina_actividad_Flujo_trabajo_workflow = "La actividad esta conectada a otra actividad como actividad destino, imposible eliminar "
            Exit Function
        End If
        '--------------------------------------------------------------------
        'Verifica que la actividad no tenga tareas relacionadas sin terminar
        '---------------------------------------------------------------------
        Dim estado_actividad_relacionado As String = "YES"
        Result = Verifica_existencia_actividad_relacionada_flujo_trabajo_sin_terminar(id_actividad, estado_actividad_relacionado)
        If Result <> "YES" Then
            Elimina_actividad_Flujo_trabajo_workflow = Result
            Exit Function
        End If
        If estado_actividad_relacionado = "YES" Then
            Elimina_actividad_Flujo_trabajo_workflow = "La actividad tiene tareas relacionadas sin terminar en el flujo de trabajo, imposible eliminar la actividad"
            Exit Function
        End If
        Dim Sql_Insercion_actividad As String = "delete from  wf_registro_actividaes_flujos_trabajo where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" _
            & id_actividad

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_Insercion_actividad
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_actividad_Flujo_trabajo_workflow = "Imposible eliminar actividad  "
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------
            'Elimina el shape al diagrama
            '----------------------------------
            ref_diagram.Diagram.Items.Remove(shape)
            '--------------------------------------
            'Guarda el archivo en la base de datos
            '--------------------------------------
            Dim string_diagrama As String = ref_diagram.SaveToString(SaveToStringFormat.Base64, True)
            Dim sql_atualiza_ruta As String = "update wf_flujos_trabajo set Archivo_Plantilla_Mindifucion = ?imagen  where " _
           & "ID_WF_FLUJOS_TRABAJO =" & id_flujo_trabajo
            myCommand.CommandText = sql_atualiza_ruta
            myCommand.Parameters.AddWithValue("?imagen", string_diagrama)
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_actividad_Flujo_trabajo_workflow = "Imposible actualiza flujo de trabajo  "
                If ref_diagram.Diagram.UndoManager.History.Commands.Count > 0 Then
                    ref_diagram.Diagram.UndoManager.Undo()
                End If
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            ref_update.Update()
            myTrans.Commit()
            myConnection.Close()
            Elimina_actividad_Flujo_trabajo_workflow = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Elimina_actividad_Flujo_trabajo_workflow = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Elimina_actividad_Flujo_trabajo_workflow = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Eliminar_elemento_diagrama_web(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                            ByRef ref_update As UpdatePanel,
                                            ByVal nombre_ruta As String,
                                            ByVal id_flujo_trabajo As Integer) As String
        Try
            Dim Result As String = ""
            If ref_diagram.Diagram.Selection.Items.Count = 0 Then
                Eliminar_elemento_diagrama_web = "Por favor seleccione el elemento del diagrama "
                Exit Function
            End If
            If ref_diagram.Diagram.Selection.Items.Count > 1 Then
                Eliminar_elemento_diagrama_web = "Solo se puede eliminar un elemento del diagrama "
                Exit Function
            End If
            '-----------------------------------------------------------
            'Elimina elementos del digrama que pertenecen a actividades
            '-----------------------------------------------------------
            Dim sha As Object = Nothing
            For Each sha In ref_diagram.Diagram.Selection.Items
                If ref_diagram.Diagram.Selection.Items(0).GetType.FullName = "MindFusion.Diagramming.ShapeNode" Then
                    Result = Me.Elimina_actividad_Flujo_trabajo_workflow(ref_diagram, ref_update, "", sha, id_flujo_trabajo)
                    If Result <> "YES" Then
                        Eliminar_elemento_diagrama_web = Result
                        Exit Function
                    Else
                        Exit For
                    End If
                End If
            Next
            '----------------------------------------------------------
            'Elimina elementos conectores del diagrama 
            '----------------------------------------------------------
            sha = Nothing
            For Each sha In ref_diagram.Diagram.Selection.Items
                If ref_diagram.Diagram.Selection.Items(0).GetType.FullName = "MindFusion.Diagramming.DiagramLink" Then
                    Result = Me.Elimina_conector_actividades_flujo_trabajo_workflow(ref_diagram, ref_update, id_flujo_trabajo, sha)
                    If Result <> "YES" Then
                        Eliminar_elemento_diagrama_web = Result
                        Exit Function
                    Else
                        Exit For
                    End If
                End If
            Next
            Eliminar_elemento_diagrama_web = "YES"
        Catch ex As Exception
            Eliminar_elemento_diagrama_web = "Inconsistencia general función Eliminar_elemento_diagrama_web " & ex.Message
        End Try

    End Function
    Function Verifica_existencia_actividad_relacionada_flujo_trabajo_sin_terminar(ByVal id_actividad_flujo_trabajo As Integer, ByRef estado_actividad_relacionado As String) As String
        '-------------------------------------------------------------------
        'Función : Verifica existencia actividades sin terminar dentro
        'del flujo de trabajo sin terminar
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-10-14
        '-------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_ACTIVIDAD_FLUJO_TRABAJO from estados_tarea_workflow where ID_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad_flujo_trabajo & " and Fecha_Fin is null"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_actividad_relacionada_flujo_trabajo_sin_terminar = "Función Verifica_existencia_actividad_relacionada_flujo_trabajo_sin_terminar dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_actividad_relacionado = "YES"
                Verifica_existencia_actividad_relacionada_flujo_trabajo_sin_terminar = "YES"
                Exit Function
            Else
                estado_actividad_relacionado = "NO"
                Verifica_existencia_actividad_relacionada_flujo_trabajo_sin_terminar = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_actividad_relacionada_flujo_trabajo_sin_terminar = "Inconsistencia función Verifica_existencia_actividad_relacionada_flujo_trabajo_sin_terminar " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_conexion_como_destino_flujo_trabajo(ByVal id_actividad_flujo As Integer, ByVal id_flujo_trabajo As Integer, ByRef estado_existencia_conexion_actividad As String) As String
        '----------------------------------------------------------------------------------------
        'Funcion : Retorna existencia conexión como actividad destino en el flujo de trabajo
        'Fecha : 2017-09-19
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_REGISTRO_ACTIVIDAD_ENVIO from wf_registro_conectores_actividades_envio_flujo_trabajo where IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE =" & id_actividad_flujo & " and wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_conexion_como_destino_flujo_trabajo = "Función Verifica_existencia_conexion_como_destino_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_existencia_conexion_actividad = "YES"
                Verifica_existencia_conexion_como_destino_flujo_trabajo = "YES"
                Exit Function
            Else
                estado_existencia_conexion_actividad = "NO"
                Verifica_existencia_conexion_como_destino_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_conexion_como_destino_flujo_trabajo = "Inconsistencia general función Verifica_existencia_conexion_como_destino_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_conexion_como_fuente_flujo_trabajo(ByVal id_actividad_flujo As Integer, ByVal id_flujo_trabajo As Integer, ByRef estado_existencia_conexion_actividad As String) As String
        '----------------------------------------------------------------------------------------
        'Funcion : Retorna existencia conexión como actividad destino en el flujo de trabajo
        'Fecha : 2017-09-19
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_REGISTRO_ACTIVIDAD_ENVIO from wf_registro_conectores_actividades_envio_flujo_trabajo where  IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO =" & id_actividad_flujo & " and wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_conexion_como_fuente_flujo_trabajo = "Función Verifica_existencia_conexion_como_destino_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_existencia_conexion_actividad = "YES"
                Verifica_existencia_conexion_como_fuente_flujo_trabajo = "YES"
                Exit Function
            Else
                estado_existencia_conexion_actividad = "NO"
                Verifica_existencia_conexion_como_fuente_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_conexion_como_fuente_flujo_trabajo = "Inconsistencia general función Verifica_existencia_conexion_como_fuente_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_conector_actividades_flujo_trabajo(ByVal Id_Actividad_flujo_fuente As Integer, ByVal Id_Actividad_flujo_destino As Integer, ByVal id_flujo As Integer) As String
        '-------------------------------------------------------
        'Función : Verifica la existencia de conectores entre
        'actividades
        'Fecha : 2017-08-03
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Dim Sql_consulta As String = "Select * from wf_registro_conectores_actividades_envio_flujo_trabajo " _
            & " where IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO= " & Id_Actividad_flujo_destino & " and IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE = " & Id_Actividad_flujo_fuente &
            " and wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_conector_actividades_flujo_trabajo = "Función Verifica_existencia_conector_actividades_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_existencia_conector_actividades_flujo_trabajo = "YES"
                Exit Function
            Else
                Verifica_existencia_conector_actividades_flujo_trabajo = "El conector de actividades ya se encuentra registrado, imposible conectar las actividades"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_conector_actividades_flujo_trabajo = "Inconsistencia general función Verifica_existencia_conector_actividades_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Elimina_conector_actividades_flujo_trabajo_workflow(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
            ByRef ref_update As UpdatePanel, ByVal id_flujo_trabajo As Integer, ByRef shape As MindFusion.Diagramming.DiagramLink) As String
        Dim Result As String = ""
        If shape.Id Is Nothing Then
            Elimina_conector_actividades_flujo_trabajo_workflow = "El id del DiagramLink se ecuentra en estado nothing "
            Exit Function
        End If
        '----------------------------------------
        'Función retorna id ruta
        '----------------------------------------
        Dim Refclas_workflow As New ClassWorkflow
        'Dim id_ruta As Integer = 0
        'Result = Refclas_workflow.Retorna_id_ruta_workflow(id_flujo_trabajo, id_ruta)
        'If Result <> "YES" Then
        '    Elimina_conector_actividades_flujo_trabajo_workflow = Result
        '    Exit Function
        'End If
        Dim matri_id_link() As String = shape.Id.ToString.Split("_")
        Dim id_link As Integer = Val(matri_id_link(1))
        Dim Sql_eliminar As String = "delete from wf_registro_conectores_actividades_envio_flujo_trabajo where ID_REGISTRO_ACTIVIDAD_ENVIO=" & id_link &
            " and wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_eliminar
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_conector_actividades_flujo_trabajo_workflow = "Imposible eliminar el conector  "
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------
            'Elimina el link del diagrama
            '----------------------------------
            ref_diagram.Diagram.Items.Remove(shape)
            Dim string_plantilla As String = ""
            string_plantilla = ref_diagram.SaveToString(SaveToStringFormat.Base64, True)
            '-------------------------------------------------
            'Guarda el archivo en la base de datos
            '-------------------------------------------------
            Dim sql_atualiza_ruta As String = "update wf_flujos_trabajo set Archivo_Plantilla_Mindifucion = ?imagen  where " _
          & "ID_WF_FLUJOS_TRABAJO =" & id_flujo_trabajo
            myCommand.CommandText = sql_atualiza_ruta
            myCommand.Parameters.AddWithValue("?imagen", string_plantilla)
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                ref_diagram.Diagram.Selection.Clear()
                If ref_diagram.Diagram.UndoManager.History.Commands.Count > 0 Then
                    ref_diagram.Diagram.UndoManager.Undo()
                End If
                ref_update.Update()
                Elimina_conector_actividades_flujo_trabajo_workflow = "Imposible actualiza flujo de trabajo  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            ref_update.Update()
            myTrans.Commit()
            myConnection.Close()
            Elimina_conector_actividades_flujo_trabajo_workflow = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Elimina_conector_actividades_flujo_trabajo_workflow = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Elimina_conector_actividades_flujo_trabajo_workflow = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Elimina_relacion_tramite_flujo_trabajo(ByVal id_relacion_tramite_flujo As Integer,
                                                    ByRef ref_treview As TreeView,
                                                    ByRef ref_trenode As TreeNode,
                                                    ByRef ref_update As UpdatePanel) As String
        Dim existencia_relacion As String = "YES"
        Dim Result As String = ""
        Dim stru_relacion_tramite As stru_relacion = Nothing
        Result = Me.Solicita_datos_estructura_relacion_tramite_flujo(id_relacion_tramite_flujo,
                                                                     stru_relacion_tramite)
        If Result <> "YES" Then
            Elimina_relacion_tramite_flujo_trabajo = Result
            Exit Function
        End If
        If stru_relacion_tramite.ID_RELACION_TRAMITE = 0 Then
            Elimina_relacion_tramite_flujo_trabajo = "Imposible encontrar la estrucutura de la relacion ( " & id_relacion_tramite_flujo & " )"
            Exit Function
        End If
        Dim nombre_tipo_tramite As String = ""
        Dim ref_class_tipo_doc As New Class_tipo_doc_entrante
        Result = ref_class_tipo_doc.Solicita_nombre_tipo_tramite_por_id_tramite(stru_relacion_tramite.tipo_doc_entrante_id_Tipo_Doc_Entrante, nombre_tipo_tramite)
        If Result <> "YES" Then
            Elimina_relacion_tramite_flujo_trabajo = Result
            Exit Function
        End If
        Dim nombre_flujo_trabajo As String = ""
        If existencia_relacion = "YES" Then
            Result = Me.SolicitaNombreFlujoTrabajoPorIdFlujo(stru_relacion_tramite.ID_WF_FLUJO_TRABAJO, nombre_flujo_trabajo)
            If Result <> "YES" Then
                Elimina_relacion_tramite_flujo_trabajo = Result
                Exit Function
            End If
        End If
        Dim RefclasGestionInstrumento As New ClassGaGestionInstrumento
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date1al)
        If Result <> "YES" Then
            Elimina_relacion_tramite_flujo_trabajo = "Error formateando fecha  log para el control de auditoría "
            Exit Function
        End If
        Dim hor As New System.DateTime
        hor = Date.Now
        Dim Datos_transacion As String = "ELIMINA RELACION ID TRAMITE " & stru_relacion_tramite.tipo_doc_entrante_id_Tipo_Doc_Entrante & " NOMBRE " & nombre_tipo_tramite &
            " CON EL FLUJO DOCUMENTAL ID " & stru_relacion_tramite.ID_WF_FLUJO_TRABAJO & " NOMBRE " & nombre_flujo_trabajo
        Dim hora As String = hor.Hour.ToString & ":" & hor.Minute.ToString & ":" & hor.Second.ToString
        Dim sql_insert As String = "Insert into wf_registro_log_flujos_trabajo (wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,ID_USER,DATE_TRANS,DESC_OP,CAMPOS,IP_TRANS," _
                                      & "HORA_REGISTRO,MODULO_REGISTRO,USER_OPER) values (" &
                                      "'" & stru_relacion_tramite.ID_WF_FLUJO_TRABAJO & "','" & HttpContext.Current.Session.Item("Id_Log_Usuario_Workfow") & "','" & date1al & "','" &
                                      "ELIMINA RELACION FLUJO TRAMITE" & "','" & Datos_transacion & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" &
                                      hora & "','WORKFLOW WEB','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "')"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Dim Sql_Insercion As String = "Delete from ra_relacion_tramite_flujo_wokflow where ID_RELACION_TRAMITE=" & id_relacion_tramite_flujo
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_Insercion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_relacion_tramite_flujo_trabajo = "Imposible relacionar tipo tramite a flujo documental  "
                myConnection.Close()
                Exit Function
            End If
            Dim tre_nod_parent As TreeNode = ref_trenode.Parent
            tre_nod_parent.ChildNodes.Remove(ref_trenode)
            ref_update.Update()
            myTrans.Commit()
            myConnection.Close()
            Elimina_relacion_tramite_flujo_trabajo = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Elimina_relacion_tramite_flujo_trabajo = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Elimina_relacion_tramite_flujo_trabajo = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Elimina_flujo_de_trabajo(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                      ByRef up_date As UpdatePanel,
                                      ByVal nombre_flujo As String,
                                      ByVal zon_view As Integer,
                                      ByRef drop_list As DropDownList,
                                      ByRef re_update As UpdatePanel) As String
        Dim Result As String = ""
        Dim Existencia_relacion As String = ""
        Dim descripcion_documento As String = ""
        Dim id_flujo_trabajo = HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO")
        If id_flujo_trabajo = 0 Then
            Elimina_flujo_de_trabajo = "Debe seleccionar una flujo de trabajo"
            Exit Function
        End If
        Result = Me.Verifica_existencia_tramite_relacionado_flujo_trabajo(id_flujo_trabajo, Existencia_relacion, descripcion_documento)
        If Result <> "YES" Then
            Elimina_flujo_de_trabajo = Result
            Exit Function
        End If
        If Existencia_relacion = "YES" Then
            Elimina_flujo_de_trabajo = "Existe una relación con el tramite " & descripcion_documento
            Exit Function
        End If
        Dim existencia_actividades As String = ""
        Result = Me.Verifica_actividades_relacionadas_al_flujo_de_trabajo(id_flujo_trabajo, existencia_actividades)
        If Result <> "YES" Then
            Elimina_flujo_de_trabajo = Result
            Exit Function
        End If
        If existencia_actividades = "YES" Then
            Elimina_flujo_de_trabajo = "Existen actividades relacionadas al flujo documental "
            Exit Function
        End If
        Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../configuracion/dlc_flujo.vdx")
        If File.Exists(ruta_plantilla) = False Then
            Elimina_flujo_de_trabajo = "Imposible encontrar el archivo " & ruta_plantilla
            Exit Function
        End If
        Dim ob As New MindFusion.Diagramming.Import.VisioImporter
        Dim diagran As New MindFusion.Diagramming.DiagramDocument
        Dim digran_page As New MindFusion.Diagramming.DiagramPage
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Dim sql_insert As String = "Delete from wf_flujos_trabajo where ID_WF_FLUJOS_TRABAJO= " & id_flujo_trabajo
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_insert
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_flujo_de_trabajo = "Imposible eliminar flujo de trabajo  "
                myConnection.Close()
                Exit Function
            End If
            drop_list.Items.Remove(nombre_flujo)
            diagran = ob.Import(ruta_plantilla)
            If diagran.Pages.Count = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Elimina_flujo_de_trabajo = "La plantilla base no tiene el formato correcto, no contiene por lo menos un diagrama "
                Exit Function
            End If
            ref_diagram.Diagram.Items.Clear()
            ref_diagram.Diagram = diagran.Pages(0)
            ref_diagram.ZoomFactor = zon_view
            up_date.Update()
            re_update.Update()
            HttpContext.Current.Session.Item("DR_FLUJO_SELECCIONADO") = ""
            HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO") = 0
            myTrans.Commit()
            myConnection.Close()
            Elimina_flujo_de_trabajo = "YES"
            up_date.Update()
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Elimina_flujo_de_trabajo = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Elimina_flujo_de_trabajo = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Verifica_existencia_tramite_relacionado_flujo_trabajo(ByVal id_flujo_trabajo As Integer, ByRef existencia_relacion As String,
                                                                   ByRef descripcion_documento As String) As String
        Try
            Dim Parametro_Consulta As String = "select Descripcion_Doc from tipo_doc_entrante where ID_WF_FLUJOS_TRABAJO ='" & id_flujo_trabajo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_tramite_relacionado_flujo_trabajo = "Error función Verifica_existencia_tramite_relacionado_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                existencia_relacion = "YES"
                descripcion_documento = Datset.Tables(0).Rows(0).Item(0)
                Verifica_existencia_tramite_relacionado_flujo_trabajo = "YES"
                Exit Function
            Else
                existencia_relacion = "NO"
                descripcion_documento = ""
                Verifica_existencia_tramite_relacionado_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_tramite_relacionado_flujo_trabajo = "Inconsistencia general función Verifica_existencia_tramite_relacionado_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Verifica_actividades_relacionadas_al_flujo_de_trabajo(ByVal id_flujo_trabajo As Integer, ByRef existencia_actividades As String) As String
        '-------------------------------------------------------
        'Función : Verifica la existencia de actividades en el
        'flujo de trabajo
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-09-25
        '-------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO from wf_registro_actividaes_flujos_trabajo where wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO ='" & id_flujo_trabajo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_actividades_relacionadas_al_flujo_de_trabajo = "Error función Verifica_actividades_relacionadas_al_flujo_de_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                existencia_actividades = "YES"
                Verifica_actividades_relacionadas_al_flujo_de_trabajo = "YES"
                Exit Function
            Else
                existencia_actividades = "NO"
                Verifica_actividades_relacionadas_al_flujo_de_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_actividades_relacionadas_al_flujo_de_trabajo = "Inconsistencia general función Verifica_actividades_relacionadas_al_flujo_de_trabajo " & ex.Message
        End Try
    End Function
    Function Lista_flujo_trabajo_tarea_worflow_grafico(ByRef Label_nombre_flujo_trabjo As Label,
                                                       ByRef diagramView As Object,
                                                       ByRef UpdatePanel_diagran_view As Object,
                                                       ByRef CheckBox_Grid_alineamiento As Object) As String
        Try
            Dim Nombre_flujo_trabajo As String = ""
            Dim Result As String = Me.SolicitaNombreFlujoTrabajoPorIdFlujo(HttpContext.Current.Session.Item("DR_ID_FLUJO_TRABAJO"),
                                                                                Nombre_flujo_trabajo)
            If Result <> "YES" Then
                Lista_flujo_trabajo_tarea_worflow_grafico = Result
                Exit Function
            End If
            Result = Me.Abre_flujo_trabajo_ruta_workflow(Nombre_flujo_trabajo,
                                                         HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                         diagramView,
                                                         UpdatePanel_diagran_view,
                                                         CheckBox_Grid_alineamiento)
            If Result <> "YES" Then
                Lista_flujo_trabajo_tarea_worflow_grafico = Result
            Else
                Dim Matri_actividades_recorri() As Integer = Nothing
                Result = Me.Retorna_actividades_recorridas_unicas_flujo_trabajo(HttpContext.Current.Session.Item("DR_ID_TAREA_FLUJO_TRABAJO"),
                                                                                HttpContext.Current.Session.Item("DR_ID_FLUJO_TRABAJO"),
                                                                                Matri_actividades_recorri)
                If Result <> "YES" Then
                    Lista_flujo_trabajo_tarea_worflow_grafico = Result
                    Exit Function
                End If
                If Not Matri_actividades_recorri Is Nothing Then
                    Result = Me.Marca_Actividad_flujo_trabajo_recorridas(Matri_actividades_recorri,
                                                                         diagramView,
                                                                         UpdatePanel_diagran_view)
                    If Result <> "YES" Then
                        Lista_flujo_trabajo_tarea_worflow_grafico = Result
                        Exit Function
                    End If
                    Result = Me.Marca_Actividad_flujo_trabajo(Matri_actividades_recorri(Matri_actividades_recorri.Length - 1),
                                                              diagramView,
                                                              UpdatePanel_diagran_view)
                    If Result <> "YES" Then
                        Lista_flujo_trabajo_tarea_worflow_grafico = Result
                    End If
                End If

                Dim matri_conector() As stru_conector_estado = Nothing
                Dim refclas_workflow_rutas As New Class_worflow_rutas
                Result = Me.Retorna_actividades_recorridas_flujo_trabajo(HttpContext.Current.Session.Item("DR_ID_TAREA_FLUJO_TRABAJO"),
                                                                         HttpContext.Current.Session.Item("DR_ID_FLUJO_TRABAJO"),
                                                                         matri_conector)
                If Result <> "YES" Then
                    Lista_flujo_trabajo_tarea_worflow_grafico = Result
                    Exit Function
                End If
                If Not matri_conector Is Nothing Then
                    If matri_conector.Length > 1 Then
                        For i As Integer = 0 To matri_conector.Length - 1
                            If i < matri_conector.Length - 1 Then
                                Result = refclas_workflow_rutas.Conecta_actividades_recorrido_ruta(matri_conector(i).Id_Actividad,
                                                                                                   matri_conector(i + 1).Id_Actividad,
                                                                                                   (i + 1), diagramView, UpdatePanel_diagran_view,
                                                                                                   matri_conector(i).id_Estado)
                                If Result <> "YES" Then
                                    Lista_flujo_trabajo_tarea_worflow_grafico = Result
                                    Exit Function
                                End If
                            End If
                        Next
                    End If
                End If
                Create_label_flujo_trabajo_diagrama(diagramView, UpdatePanel_diagran_view, Nombre_flujo_trabajo,
                                                    HttpContext.Current.Session.Item("DR_RADICADO_FLUJO_TRABAJO"),
                                                    HttpContext.Current.Session.Item("DR_ID_TAREA_FLUJO_TRABAJO"), 1)
                Dim Refclas_R As New ClassRadicador
                Dim nombre_plantilla_radicado As String = ""
                Dim Ref_Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
                Result = Ref_Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(HttpContext.Current.Session.Item("DR_RADICADO_FLUJO_TRABAJO"),
                                                                                                  nombre_plantilla_radicado)
                If Result <> "YES" Then
                    Lista_flujo_trabajo_tarea_worflow_grafico = Result
                    Exit Function
                End If
                '------------------------------------
                'Solicita nombre tipo documento
                '------------------------------------
                Dim nombre_tipo_documento As String = ""
                Result = Refclas_R.Solicita_nombre_tipo_documento(HttpContext.Current.Session.Item("DR_RADICADO_FLUJO_TRABAJO"), nombre_plantilla_radicado, nombre_tipo_documento)
                If Result <> "YES" Then
                    Lista_flujo_trabajo_tarea_worflow_grafico = Result
                    Exit Function
                End If
                Label_nombre_flujo_trabjo.Text = "Flujo de trabajo (" & Nombre_flujo_trabajo & ")    Tipo tramite relacionado (" & nombre_tipo_documento & ")"
                Lista_flujo_trabajo_tarea_worflow_grafico = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_flujo_trabajo_tarea_worflow_grafico = "Inconsistencia general función Lista_flujo_trabajo_tarea_worflow_grafico " & ex.Message
        End Try
    End Function
    Function Create_label_flujo_trabajo_diagrama(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                    ByRef ref_update As UpdatePanel,
                                    ByVal nombre_flujo As String,
                                    ByVal radicado As String,
                                    ByVal id_tarea As Integer,
                                    ByVal marca_descripcion As Integer) As String
        Try

            Dim sap As New MindFusion.Diagramming.ShapeNode
            Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            Dim x As Integer = 0
            Dim y As Integer = 0
            '-------------------------------------------
            'Posiciona el shape en el grid del diagrama
            '-------------------------------------------
            x = 2
            y = (ref_diagram.Diagram.Bounds.Height) - 1
            Dim Rect = New RectangleF(x, y, 400, 5)
            sap.Bounds = Rect
            Dim sysdra = New System.Drawing.Font("Bold", 12)
            sap.Font = sysdra
            Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush(
             Color.White, Color.White, 0)
            sap.Brush = penBrush
            sap.Transparent = True
            sap.Locked = True
            sap.Text = "Flujo de trabajo (" & nombre_flujo & ")  Radicado relacionado (" & radicado & ") Indentificación de la tarea (" & id_tarea & ") fecha diagrama " & Now.ToString
            ref_diagram.Diagram.Items.Add(sap)
            If marca_descripcion = 1 Then
                sap = New MindFusion.Diagramming.ShapeNode
                Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
                sap.Transparent = True
                sap.Locked = True
                x = 1
                y = (ref_diagram.Diagram.Bounds.Height) - 10
                Rect = New RectangleF(x, y, 215, 30)
                sap.Bounds = Rect
                sap.ImageUrl = "../workflow/imageneswf/contenido_recto.png"
                ref_diagram.Diagram.Items.Add(sap)
            End If
            ref_update.Update()
            Create_label_flujo_trabajo_diagrama = "YES"
        Catch ex As Exception
            Create_label_flujo_trabajo_diagrama = "Inconsistencia genera función Create_label_flujo_trabajo_diagrama " & ex.Message
        End Try
    End Function
    Function Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(ByVal Radicado As String,
                                                                                              ByRef id_actividad_flujo_trabjo As Integer,
                                                                                              ByRef id_flujo_trabajo As Integer,
                                                                                              ByRef id_usuarion_workflow_flujo_trabajo As Integer,
                                                                                              ByVal id_usuario_workflow As Integer,
                                                                                              ByVal id_tarea_selecion As Long) As String
        '--------------------------------------------------------------------
        'Función : Solicita id flujo de trabjo id actividad flujo trabajo y 
        'id usuario workflow relacionado al flujo de trabajo
        'Fecha : 2017-09-30
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.SolicitaIdFlujoTrabajoIdTareaRutaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                         id_tarea_selecion,
                                                                                         id_flujo_trabajo)
            If Result <> "YES" Then
                Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo = Result
                Exit Function
            End If
            If id_flujo_trabajo = 0 Then
                Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo = "YES"
                Exit Function
            End If
            '------------------------------------------------------------
            'Solicita id actividad flujo de trabajo id usuario workflow
            '------------------------------------------------------------
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Result = Class_estados_tarea_workflow.Solicita_id_actividad_flujo_trabajo_id_usuario_flujo_workflow(id_flujo_trabajo,
                                                                                                                id_usuario_workflow,
                                                                                                                id_tarea_selecion,
                                                                                                                id_actividad_flujo_trabjo,
                                                                                                                id_usuarion_workflow_flujo_trabajo)
            If Result <> "YES" Then
                Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo = Result
                Exit Function
            End If
            Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo = "Inconsistencia general funcion Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo " & ex.Message
        End Try
    End Function

    Function solicita_datos_flujo_trabajo_anterior(ByVal id_tarea_seleccionada As Integer,
                                                   ByRef id_flujo_trabajo As Integer,
                                                   ByRef id_actividad_flujo_trabajo As Integer,
                                                   ByRef id_usuario_flujo_trabajo As Integer) As String
        '--------------------------------------------------------------------
        'Función : Retorna anterior estado del ultimo flujo de trabajo los
        'datos de flujo de trabajo 
        'Fecha : 2017-10-17
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO from estados_tarea_workflow " &
                           " where Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_seleccionada
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                solicita_datos_flujo_trabajo_anterior = "Error función solicita_datos_flujo_trabajo_anterior " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_flujo_trabajo = 0
                solicita_datos_flujo_trabajo_anterior = "Imposible encontrar el registro de estado de la tarea " & id_tarea_seleccionada
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count = 1 Then
                    id_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                    id_actividad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(1)
                    id_usuario_flujo_trabajo = Datset.Tables(0).Rows(0).Item(2)
                    solicita_datos_flujo_trabajo_anterior = "YES"
                    Exit Function
                Else
                    id_flujo_trabajo = Datset.Tables(0).Rows(Datset.Tables(0).Rows.Count - 2).Item(0)
                    id_actividad_flujo_trabajo = Datset.Tables(0).Rows(Datset.Tables(0).Rows.Count - 2).Item(1)
                    id_usuario_flujo_trabajo = Datset.Tables(0).Rows(Datset.Tables(0).Rows.Count - 2).Item(2)
                    solicita_datos_flujo_trabajo_anterior = "YES"
                    Exit Function
                End If


            End If
        Catch ex As Exception
            solicita_datos_flujo_trabajo_anterior = "Inconsistencia general función solicita_datos_flujo_trabajo_anterior " & ex.Message
        End Try
    End Function
    Function Solicita_id_flujo_trabajo_po_id_registro_actividad(ByVal id_registro_estado_tarea As Int64,
                                                                ByRef id_flujo_trabajo As Integer) As String
        '--------------------------------------------------------------------
        'Función : Retorna la identificación del flujo de trabajo con el para
        'metro identificacion del registro de la tarea
        'Fecha : 2017-10-17
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select ID_FLUJO_TRABAJO from estados_tarea_workflow " &
                           " where id_Estado=" & id_registro_estado_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_id_flujo_trabajo_po_id_registro_actividad = "Error función Solicita_id_flujo_trabajo_po_id_registro_actividad " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_flujo_trabajo = 0
                Solicita_id_flujo_trabajo_po_id_registro_actividad = "Imposible encontrar el registro de estado del identificador " & id_registro_estado_tarea
                Exit Function
            Else
                id_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_flujo_trabajo_po_id_registro_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_flujo_trabajo_po_id_registro_actividad = "Inconsistencia general función Solicita_id_flujo_trabajo_po_id_registro_actividad " & ex.Message
        End Try
    End Function


    Function SolicitaEstadoAbiertoCerradoFlujoDocumental(ByVal IdFlujoTarea As Integer,
                                                         ByRef EstadoAbiertoCerrado As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el estado cerrado y abierto del flujo informado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdFlujoTarea        : Representa la identificación del flujo de trabajo
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstadoAbiertoCerrado  : Retorna el estado abierto cerrado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2017-10-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select TIPO_RUTA_ABIERTA_CERRADA from wf_flujos_trabajo " &
                " where ID_WF_FLUJOS_TRABAJO=" & IdFlujoTarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaEstadoAbiertoCerradoFlujoDocumental = "Se presentó una inconsistencia en la función SolicitaEstadoAbiertoCerradoFlujoDocumental : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEstadoAbiertoCerradoFlujoDocumental = "No fue posible encontrar el flujo documental con la identificación proporcionada. (" & IdFlujoTarea & ")"
                Exit Function
            Else
                EstadoAbiertoCerrado = Datset.Tables(0).Rows(0).Item(0)
                SolicitaEstadoAbiertoCerradoFlujoDocumental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstadoAbiertoCerradoFlujoDocumental = "Inconsistencia general función SolicitaEstadoAbiertoCerradoFlujoDocumental " & ex.Message
        End Try
    End Function

    Function Solicita_estado_abierto_cerrado_actividad_flujo_docuental(ByVal id_flujo_documental As Integer,
                                                                       ByVal id_actividad As Integer,
                                                                       ByRef estado_abierto_cerrado As Integer) As String
        '--------------------------------------------------------
        'Function Retorna el estado cerrado y abierto de la actividad
        'de flujo de trabajo
        'Fecha : 2017-10-05
        'Ing Miguel Angel Urueta Miranda
        '--------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select TIPO_ABIERTA_CERRADA_ACTIVIDAD from wf_registro_actividaes_flujos_trabajo " &
                " where wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_documental & " and ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_abierto_cerrado_actividad_flujo_docuental = "Error función Solicita_estado_abierto_cerrado_actividad_flujo_docuental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_abierto_cerrado_actividad_flujo_docuental = "Imposible encontrar la actividad de flujo de documental de id (" & id_actividad & ")"
                Exit Function
            Else
                estado_abierto_cerrado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_abierto_cerrado_actividad_flujo_docuental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_abierto_cerrado_actividad_flujo_docuental = "Inconsistencia general función Solicita_estado_abierto_cerrado_actividad_flujo_docuental " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado(ByVal id_tarea_seleccionada As Integer,
                                                                         ByVal id_usuario_workflow As Integer) As String
        Try
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Dim Refclas_w As New ClassWorkflow
            Dim Result As String = ""
            Dim Radicado As String = ""
            Dim Id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_actividad_flujo_trabajo As Integer = 0
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_seleccionada,
                                                                                 Radicado)
            If Result <> "YES" Then
                Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = Result
                Exit Function
            End If
            Result = Refclas_f.Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(Radicado,
                               id_actividad_flujo_trabajo, Id_flujo_trabajo, id_usuario_workflow_actividad_flujo_trabajo,
                               id_usuario_workflow, id_tarea_seleccionada)
            If Result <> "YES" Then
                Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = Result
                Exit Function
            End If
            Dim Nombre_flujo_trabajo As String = ""
            Dim estado_abierto_cerrad_flujo_trabajo As Integer = 0
            Dim estado_abierto_cerrad_actividad_flujo_trabajo As Integer = 0
            If Id_flujo_trabajo <> 0 Then
                Result = Refclas_f.SolicitaNombreFlujoTrabajoPorIdFlujo(Id_flujo_trabajo, Nombre_flujo_trabajo)
                If Result <> "YES" Then
                    Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = Result
                    Exit Function
                End If
                Result = Refclas_f.SolicitaEstadoAbiertoCerradoFlujoDocumental(Id_flujo_trabajo, estado_abierto_cerrad_flujo_trabajo)
                If Result <> "YES" Then
                    Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = Result
                    Exit Function
                End If
                If estado_abierto_cerrad_flujo_trabajo <> 0 Then
                    Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = "La tarea pertenece a un de flujo de trabajo cerrado, imposible enviar la tarea "
                    Exit Function
                End If
                If id_actividad_flujo_trabajo <> 0 Then
                    Result = Refclas_f.Solicita_estado_abierto_cerrado_actividad_flujo_docuental(Id_flujo_trabajo, id_actividad_flujo_trabajo, estado_abierto_cerrad_actividad_flujo_trabajo)
                    If Result <> "YES" Then
                        Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = Result
                        Exit Function
                    Else
                        If estado_abierto_cerrad_actividad_flujo_trabajo = 1 Then
                            Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = "La tarea pertenece a una actividad de flujo de trabajo cerrado, imposible enviar la tarea "
                            Exit Function
                        End If

                    End If
                End If
                Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = "YES"
                Exit Function
            Else
                Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado = "Inconsistencia general función Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente(ByVal id_tarea_seleccionada As Integer, ByVal id_usuario_workflow As Integer,
                                                                                    ByRef Radicado_flujo_trabajo As String) As String
        Try
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Dim Refclas_w As New ClassWorkflow
            Dim Result As String = ""
            Dim Radicado As String = ""
            Dim Id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_actividad_flujo_trabajo As Integer = 0
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_seleccionada,
                                                                                 Radicado)
            If Result <> "YES" Then
                Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = Result
                Exit Function
            End If
            Result = Refclas_f.Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(Radicado,
                               id_actividad_flujo_trabajo, Id_flujo_trabajo, id_usuario_workflow_actividad_flujo_trabajo,
                               id_usuario_workflow, id_tarea_seleccionada)
            If Result <> "YES" Then
                Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = Result
                Exit Function
            End If
            Dim Nombre_flujo_trabajo As String = ""
            Dim estado_abierto_cerrad_flujo_trabajo As Integer = 0
            Dim estado_abierto_cerrad_actividad_flujo_trabajo As Integer = 0
            If Id_flujo_trabajo <> 0 Then
                Result = Refclas_f.SolicitaNombreFlujoTrabajoPorIdFlujo(Id_flujo_trabajo, Nombre_flujo_trabajo)
                If Result <> "YES" Then
                    Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = Result
                    Exit Function
                End If
                Result = Refclas_f.SolicitaEstadoAbiertoCerradoFlujoDocumental(Id_flujo_trabajo, estado_abierto_cerrad_flujo_trabajo)
                If Result <> "YES" Then
                    Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = Result
                    Exit Function
                End If
                If estado_abierto_cerrad_flujo_trabajo <> 0 Then
                    Radicado_flujo_trabajo = Radicado
                    Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = "YES"
                    Exit Function
                End If
                If id_actividad_flujo_trabajo <> 0 Then
                    Result = Refclas_f.Solicita_estado_abierto_cerrado_actividad_flujo_docuental(Id_flujo_trabajo, id_actividad_flujo_trabajo, estado_abierto_cerrad_actividad_flujo_trabajo)
                    If Result <> "YES" Then
                        Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = Result
                        Exit Function
                    Else
                        If estado_abierto_cerrad_actividad_flujo_trabajo = 1 Then
                            Radicado_flujo_trabajo = Radicado
                            Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = "YES"
                            Exit Function
                        End If

                    End If
                End If
                Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = "YES"
                Exit Function
            Else
                Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente = "Inconsistencia general función Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_actividades_selecionadas_en_pendiente_flujo_de_trabajo(ByVal id_tarea As Integer) As String
        '278-444-1700466703098.274-453-1700466703107.273-443-1700466703097.272-449-1700466703103
        'idpendiente -idtarea - radicado
        Try

            Dim Refclas_wf As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Dim Radicados As String = ""
            Dim radi_ref As String = ""
            Result = Refclas_wf.Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado_pendiente(id_tarea, HttpContext.Current.Session.Item("Id_Usuario_Workflow"), radi_ref)
            If Result <> "YES" Then
                Verifica_existencia_actividades_selecionadas_en_pendiente_flujo_de_trabajo = Result
                Exit Function
            End If
            If radi_ref <> "" Then
                If Radicados = "" Then
                    Radicados = radi_ref
                Else
                    Radicados = Radicados & "," & radi_ref
                End If
            End If

            If Radicados <> "" Then
                Verifica_existencia_actividades_selecionadas_en_pendiente_flujo_de_trabajo = "La tarea seleccionada hace parte de un flujo de trabajo cerrado, imposible reenviar la tarea de forma libre utilice la opción (enviar a)"
                Exit Function
            Else
                Verifica_existencia_actividades_selecionadas_en_pendiente_flujo_de_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_actividades_selecionadas_en_pendiente_flujo_de_trabajo = "Inconsistencia función Verifica_existencia_actividades_selecionadas_en_pendiente_flujo_de_trabajo " & ex.Message
        End Try

    End Function
    Function Diagrama_trazabilidad_flujo_trabajo(ByVal id_tarea As Integer,
                                                 ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                                 ByRef ref_update As UpdatePanel,
                                                 ByRef ref_CheckBox_Grid_alineamiento As CheckBox,
                                                 ByVal id_flujo_trabajo As Integer) As String
        Try
            Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../configuracion/dlc_flujo.vdx")
            If File.Exists(ruta_plantilla) = False Then
                Diagrama_trazabilidad_flujo_trabajo = "Imposible encontrar el archivo " & ruta_plantilla
                Exit Function
            End If
            Dim ob As New MindFusion.Diagramming.Import.VisioImporter
            Dim diagran As New MindFusion.Diagramming.DiagramDocument
            Dim digran_page As New MindFusion.Diagramming.DiagramPage
            diagran = ob.Import(ruta_plantilla)
            If diagran.Pages.Count = 0 Then
                Diagrama_trazabilidad_flujo_trabajo = "La plantilla base no tiene el formato correcto, no contiene por lo menos un diagrama "
                Exit Function
            End If
            ref_diagram.Diagram.Items.Clear()
            ref_diagram.Diagram = diagran.Pages(0)
            ref_diagram.ZoomFactor = 75
            Dim stru_shape_actividad() As stru_shape_actividad = Nothing
            Dim Result As String = ""
            Result = Me.Solicita_listado_estructura_actividades_recorridas(id_tarea,
                                                                           HttpContext.Current.Session.Item("DR_ID_FLUJO_TRABAJO"),
                                                                           stru_shape_actividad)
            If Result <> "YES" Then
                Diagrama_trazabilidad_flujo_trabajo = Result
                Exit Function
            End If
            If Not stru_shape_actividad Is Nothing Then
                For i As Integer = 0 To stru_shape_actividad.Length - 1
                    Result = Me.Agrega_shape_flujo_trabajo_trazabilidad(ref_diagram,
                                                                        ref_update,
                                                                        stru_shape_actividad(i).nombre_tipo_actividad,
                                                                        stru_shape_actividad(i).id_estado,
                                                                        stru_shape_actividad(i).nombre_actividad,
                                                                        0,
                                                                        i + 1,
                                                                        stru_shape_actividad(i).cargo_usuario)
                    If Result <> "YES" Then
                        Diagrama_trazabilidad_flujo_trabajo = Result
                        Exit Function
                    End If
                Next
                Result = Conecta_shape_trazabilidad_flujo_trabajo(ref_diagram,
                                                                  ref_update)
                If Result <> "YES" Then
                    Diagrama_trazabilidad_flujo_trabajo = Result
                    Exit Function
                End If
            End If
            Diagrama_trazabilidad_flujo_trabajo = "YES"
        Catch ex As Exception
            Diagrama_trazabilidad_flujo_trabajo = "Inconsistencia general función Diagrama_trazabilidad_flujo_trabajo " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Conecta_shape_trazabilidad_flujo_trabajo(ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                                      ByRef ref_update As UpdatePanel) As String

        Try
            Dim sha As Object = Nothing
            Dim Matshape() As Object
            Erase Matshape
            Dim i As Integer = 0
            For Each sha_ As Object In ref_diagram.Diagram.Items
                Dim ob As Object = sha_.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    ReDim Preserve Matshape(i)
                    Matshape(i) = sha_
                    i = i + 1
                End If
            Next
            If Matshape Is Nothing Then
                Conecta_shape_trazabilidad_flujo_trabajo = "YES"
                Exit Function
            End If
            If Matshape.Length = 1 Or Matshape.Length = 0 Then
                Conecta_shape_trazabilidad_flujo_trabajo = "YES"
                Exit Function
            End If
            For z As Integer = 0 To Matshape.Length - 1
                If z = 0 Then
                    Dim link As MindFusion.Diagramming.DiagramLink
                    link = ref_diagram.Diagram.Factory.CreateDiagramLink(Matshape(0), Matshape(1))
                    Dim Stroke = New MindFusion.Drawing.Pen(Color.Red, 0)
                    link.Pen = Stroke
                    link.AutoRoute = True
                    link.AutoSnapToNode = False
                    link.Text = z + 1
                    'EVITA QUE SE MUEVA EL CONECTOR FINAL
                    link.AllowMoveEnd = False
                    link.AllowMoveStart = False
                    link.DrawCrossings = False
                    link.CrossingRadius = 1
                    link.DrawCrossings = True
                    link.Id = Matshape(0).id
                    link.Tag = "Traza"
                    Dim Fill = New MindFusion.Drawing.SolidBrush(Color.Yellow)
                    link.HeadBrush = Fill
                Else
                    If z < Matshape.Length - 1 Then
                        Dim link As MindFusion.Diagramming.DiagramLink
                        link = ref_diagram.Diagram.Factory.CreateDiagramLink(Matshape(z), Matshape(z + 1))
                        Dim Stroke = New MindFusion.Drawing.Pen(Color.Red, 0)
                        link.Pen = Stroke
                        link.Text = z + 1
                        link.AutoRoute = False
                        link.AutoSnapToNode = False
                        'EVITA QUE SE MUEVA EL CONECTOR FINAL
                        link.AllowMoveEnd = False
                        link.DrawCrossings = False
                        link.AllowMoveStart = False
                        link.CrossingRadius = 1
                        link.DrawCrossings = True
                        link.Id = Matshape(z).id
                        link.Tag = "Traza"
                        Dim Fill = New MindFusion.Drawing.SolidBrush(Color.Yellow)
                        link.HeadBrush = Fill
                    End If

                End If
            Next
            Conecta_shape_trazabilidad_flujo_trabajo = "YES"
        Catch ex As Exception
            Conecta_shape_trazabilidad_flujo_trabajo = "Inconsistencia general función Conecta_shape_trazabilidad_flujo_trabajo " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Solicita_listado_estructura_actividades_recorridas(ByVal id_tarea As Integer,
                                                                ByVal id_flujo_trabajo As Integer,
                                                                ByRef stru_shape_actividad() As stru_shape_actividad) As String
        Try
            stru_shape_actividad = Nothing
            Dim sqlconsulta As String = "(SELECT  etw.id_Estado,law.Nombre_Actividad,agw.Tipo_Actividad, '' as colum, etw.ESTADO_RECUPERACION_FLUJO_TRABAJO  from estados_tarea_workflow as etw " &
            "inner join wf_registro_actividaes_flujos_trabajo as wrafr on (etw.ID_ACTIVIDAD_FLUJO_TRABAJO=wrafr.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO) " &
            "inner join listado_actividades_workflow as law on (wrafr.listado_actividades_workflow_Id_Actividad=law.Id_Actividad) " &
             "inner join Actividades_Generales_Workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General) " &
             " where inicio_tareas_workflow_id_tarea =" & id_tarea & " and ID_FLUJO_TRABAJO=" & id_flujo_trabajo & " AND id_actividad_flujo_trabajo <> 0   " &
            " and ID_USUARIO_WORKFLOW_FLUJO_TRABAJO = 0) union " &
            "(SELECT  etw.id_Estado,uw.Nombre_Usuario,'USUARIOINDIVIDUAL' as colum_virtual, uw.Cargo_Usuario, etw.ESTADO_RECUPERACION_FLUJO_TRABAJO  from estados_tarea_workflow as etw " &
            "inner join wf_registro_actividaes_flujos_trabajo as wrafr on (etw.ID_ACTIVIDAD_FLUJO_TRABAJO=wrafr.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO) " &
            "inner join listado_actividades_workflow as law on (wrafr.listado_actividades_workflow_Id_Actividad=law.Id_Actividad) " &
            "inner join usuario_workflow as uw on (etw.ID_USUARIO_WORKFLOW_FLUJO_TRABAJO=uw.Idu_suario) " &
            "inner join Actividades_Generales_Workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General) " &
            "where inicio_tareas_workflow_id_tarea=" & id_tarea & " and ID_FLUJO_TRABAJO=" & id_flujo_trabajo & " AND id_actividad_flujo_trabajo <> 0 and " &
            "ID_USUARIO_WORKFLOW_FLUJO_TRABAJO <> 0 ) order by  id_Estado "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_estructura_actividades_recorridas = "Error función Solicita_listado_estructura_actividades_recorridas " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_listado_estructura_actividades_recorridas = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_shape_actividad(i)
                    stru_shape_actividad(i).id_estado = Datset.Tables(0).Rows(i).Item(0)
                    stru_shape_actividad(i).nombre_actividad = Datset.Tables(0).Rows(i).Item(1)
                    stru_shape_actividad(i).nombre_tipo_actividad = Datset.Tables(0).Rows(i).Item(2)
                    stru_shape_actividad(i).cargo_usuario = Datset.Tables(0).Rows(i).Item(3)
                    stru_shape_actividad(i).estado_recuperacion = Datset.Tables(0).Rows(i).Item(4)
                Next
                Solicita_listado_estructura_actividades_recorridas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_estructura_actividades_recorridas = "Iconsistencia general función Solicita_listado_estructura_actividades_recorridas " & ex.Message
        End Try
    End Function
    Function solicita_datos_tarea_flujo_trabajo_workflow(ByVal id_usuario_workflow As Integer,
                                                         ByVal id_tarea As Integer,
                                                         ByRef id_flujo_trabajo As Integer,
                                                         ByRef id_actvidad_flujo_trabajo As Integer,
                                                         ByRef id_usuario_flujo_trabajo As Integer,
                                                         ByRef estado_recuperacion_flujo_trabajo As Integer) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ID_FLUJO_TRABAJO,ID_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW_FLUJO_TRABAJO,ESTADO_RECUPERACION_FLUJO_TRABAJO FROM ESTADOS_TAREA_WORKFLOW" &
            " WHERE ID_USUARIO=" & id_usuario_workflow &
            " AND  inicio_tareas_workflow_id_tarea=" & id_tarea & " AND " &
            " FECHA_INICIO IS NOT NULL and fecha_fin is null "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                solicita_datos_tarea_flujo_trabajo_workflow = "Error Consultando en tabla " & "ESTADOS_TAREA_WORKFLOW" & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_flujo_trabajo = 0
                id_actvidad_flujo_trabajo = 0
                id_usuario_flujo_trabajo = 0
                estado_recuperacion_flujo_trabajo = 0
                solicita_datos_tarea_flujo_trabajo_workflow = "YES"
                Exit Function
            Else
                id_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                id_actvidad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(1)
                id_usuario_flujo_trabajo = Datset.Tables(0).Rows(0).Item(2)
                estado_recuperacion_flujo_trabajo = Datset.Tables(0).Rows(0).Item(3)
                solicita_datos_tarea_flujo_trabajo_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            solicita_datos_tarea_flujo_trabajo_workflow = "Inconsistencia general funcion solicita_datos_tarea_flujo_trabajo_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow(ByVal id_usuario_workflow As Integer,
                                                                 ByVal id_flujo_trabajo As Integer,
                                                                 ByRef id_registro_actividad_flujo_trabajo As Integer,
                                                                 ByRef id_actividad_flujo_trabajo As Integer,
                                                                 ByRef id_usuario_workflow_flujo_trabajo As Integer) As String
        '----------------------------------------------------------
        'Function : Solicita el registro de actvividad de flujo de
        'trabajo a la que pertenece el usuario workflow
        'Fecha : 2017-12-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO," &
            "listado_actividades_workflow_Id_Actividad,ID_USUARIO_WORKFLOW FROM wf_registro_actividaes_flujos_trabajo" &
            " WHERE ID_USUARIO_WORKFLOW=" & id_usuario_workflow &
            " and wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo &
            " AND  ESTADO_ACTIVIDAD=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow = "Función Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow dice : " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_actividad_flujo_trabajo = 0
                id_actividad_flujo_trabajo = 0
                id_usuario_workflow_flujo_trabajo = 0
                Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow = "YES"
                Exit Function
            Else
                id_registro_actividad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                id_actividad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    id_usuario_workflow_flujo_trabajo = 0
                Else
                    id_usuario_workflow_flujo_trabajo = Datset.Tables(0).Rows(0).Item(2)
                End If

                Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow = "Inconsistencia general función Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow " & ex.Message
        End Try

    End Function

    Function Solicita_id_registro_actividad_flujo_trabajo_por_actividad_del_usuario_workflow(ByVal id_actividad_usuario_workflow As Integer,
                                                                 ByVal id_flujo_trabajo As Integer,
                                                                 ByRef id_registro_actividad_flujo_trabajo As Integer,
                                                                  ByRef id_actividad_flujo_trabajo As Integer) As String
        '-------------------------------------------------------------------
        'Función : Solicita el registro de la actividad de flujo de trabajo
        'relacionada a la actvidad de usuario workflow
        'Fecha : 2017-12-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO," _
              & "listado_actividades_workflow_Id_Actividad FROM wf_registro_actividaes_flujos_trabajo" &
            " WHERE listado_actividades_workflow_Id_Actividad=" & id_actividad_usuario_workflow &
            " and wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo &
            " AND  ESTADO_ACTIVIDAD=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_id_registro_actividad_flujo_trabajo_por_actividad_del_usuario_workflow = "Función Solicita_id_registro_actividad_flujo_trabajo_por_actividad_del_usuario_workflow dice : " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_actividad_flujo_trabajo = 0
                id_actividad_flujo_trabajo = 0
                Solicita_id_registro_actividad_flujo_trabajo_por_actividad_del_usuario_workflow = "YES"
                Exit Function
            Else
                id_registro_actividad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                id_actividad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(1)
                Solicita_id_registro_actividad_flujo_trabajo_por_actividad_del_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_registro_actividad_flujo_trabajo_por_actividad_del_usuario_workflow = "Inconsistencia general función Solicita_id_registro_actividad_flujo_trabajo_por_actividad_del_usuario_workflow " & ex.Message
        End Try
    End Function

    Function Duplicar_flujo_de_trabajo(ByVal nombre_nuevo_flujo_trabajo As String,
                                       ByVal nombre_flujo_trabajo_copia As String,
                                       ByVal descripcion_flujo_trabajo As String,
                                       ByVal tipo_flujo_trabajo As String,
                                       ByRef drop_lis As DropDownList,
                                       ByRef upda_date As UpdatePanel) As String
        Try
            If nombre_nuevo_flujo_trabajo = "" Then
                Duplicar_flujo_de_trabajo = "Debe informar el nombre del flujo de trabajo"
                Exit Function
            End If
            Dim existencia_flujo_trabajo As String = ""
            Dim Result As String = ""
            Result = Me.Verifica_existencia_flujo_trabajo(nombre_nuevo_flujo_trabajo,
                                                        HttpContext.Current.Session.Item("DR_ID_RUTA_SELECION_FLUJO"),
                                                        existencia_flujo_trabajo)
            If Result <> "YES" Then
                Duplicar_flujo_de_trabajo = Result
                Exit Function
            End If
            If existencia_flujo_trabajo = "YES" Then
                Duplicar_flujo_de_trabajo = "El sistema a detectado un flujo de trabajo con el nombre " & nombre_nuevo_flujo_trabajo & ", Imposible continuar"
                Exit Function
            End If
            Dim id_flujo_trabajo_copia As Integer = 0
            Result = Me.Retorna_id_flujo_trabajo_por_id_ruta(nombre_flujo_trabajo_copia,
                                                           HttpContext.Current.Session.Item("DR_ID_RUTA_SELECION_FLUJO"),
                                                           id_flujo_trabajo_copia)
            If Result <> "YES" Then
                Duplicar_flujo_de_trabajo = Result
                Exit Function
            End If
            Dim ref_diagram_copia As New MindFusion.Diagramming.WebForms.DiagramView
            Result = Solicita_archivo_flujo_trabajo(nombre_flujo_trabajo_copia,
                                                  HttpContext.Current.Session.Item("DR_ID_RUTA_SELECION_FLUJO"),
                                                  id_flujo_trabajo_copia, ref_diagram_copia)

            If Result <> "YES" Then
                Duplicar_flujo_de_trabajo = Result
                Exit Function
            End If
            Dim stru_listado() As STRU_ACTIVIDADES_FLUJO_TRABAJO = Nothing
            Result = Me.Solicita_estructura_listado_actividades_flujo_trabajo(id_flujo_trabajo_copia, stru_listado)
            If Result <> "YES" Then
                Duplicar_flujo_de_trabajo = Result
                Exit Function
            End If
            Dim id_nuevo_flujo_trabajo As Integer = 0
            Dim ref_diagram As New MindFusion.Diagramming.WebForms.DiagramView
            Result = Me.Registra_flujo_trabajo_para_copia(nombre_nuevo_flujo_trabajo, descripcion_flujo_trabajo,
                                                          tipo_flujo_trabajo, ref_diagram, id_nuevo_flujo_trabajo,
                                                          HttpContext.Current.Session.Item("DR_ID_RUTA_SELECION_FLUJO"),
                                                          nombre_flujo_trabajo_copia)
            If Result <> "YES" Then
                Duplicar_flujo_de_trabajo = Result
                Exit Function
            End If
            Result = Me.Registra_actividades_flujo_trabajo_asinga_id_nueva_actividad(id_nuevo_flujo_trabajo,
                                                                                   stru_listado)
            If Result <> "YES" Then
                Duplicar_flujo_de_trabajo = Result
                Exit Function
            End If
            For i As Integer = 0 To stru_listado.Length - 1
                Result = Me.Registra_conectores_actividades_asigna_id_nuevo_conector(id_nuevo_flujo_trabajo,
                                                                                     stru_listado(i), stru_listado)
                If Result <> "YES" Then
                    Duplicar_flujo_de_trabajo = Result
                    Exit Function
                End If
            Next

            '--------------------------------------------------------------------
            'Actualiza los elementos del diagrama con los nuevos identificadores
            '-------------------------------------------------------------------
            Result = Me.Actualiza_elementos_digrama_nuevos_identificadores(stru_listado, ref_diagram_copia,
                                                                           id_flujo_trabajo_copia,
                                                                           id_nuevo_flujo_trabajo)
            If Result <> "YES" Then
                Duplicar_flujo_de_trabajo = Result
                Exit Function
            End If
            '--------------------------------------------------
            'Guarda el archivo en el string
            '--------------------------------------------------
            Dim string_diagrama As String = ref_diagram_copia.SaveToString(SaveToStringFormat.Base64, True)
            '-------------------------------------------------
            'Guarda el archivo en la base de datos
            '-------------------------------------------------
            Result = Me.Guarda_archivo_base_datos_flujo_trabajo_string(string_diagrama, id_nuevo_flujo_trabajo)
            If Result <> "YES" Then
                Duplicar_flujo_de_trabajo = Result
                Exit Function
            End If
            drop_lis.Items.Add(nombre_nuevo_flujo_trabajo)
            upda_date.Update()
            Duplicar_flujo_de_trabajo = "YES"
        Catch ex As Exception
            Duplicar_flujo_de_trabajo = "Inconsistencia general función Duplicar_flujo_de_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_archivo_flujo_trabajo(ByVal nombre_flujo_trabajo As String,
                                            ByVal id_ruta As Integer,
                                            ByVal id_flujo_trabajo As Integer,
                                            ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView
                                           ) As String
        '-----------------------------------------------------
        'Función : Solicita nombre el archivo con el diagrama
        'Fecha : 2018-02-22
        'Ingeniero : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try

            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select Archivo_Plantilla_Mindifucion from wf_flujos_trabajo where NOMBRE_FLUJO_TRABAJO ='" & nombre_flujo_trabajo & "' and rutas_workflow_id_Ruta =" & id_ruta
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_archivo_flujo_trabajo = Result
                Exit Function
            End If
            Dim bDatos() As Byte = Nothing
            Dim strin_datos As String = ""
            If Datset.Tables(0).Rows.Count > 0 Then
                strin_datos = Datset.Tables(0).Rows(0).Item(0).ToString
                Dim path_temporal As String = ""
                If strin_datos = "" Then
                    Solicita_archivo_flujo_trabajo = "Impsoble encontrar el contenido del diagrama " & nombre_flujo_trabajo
                    Exit Function
                Else
                    ref_diagram.Diagram.LoadFromString(strin_datos)
                    ref_diagram.ZoomFactor = 75
                    Solicita_archivo_flujo_trabajo = "YES"
                End If
            Else
                Solicita_archivo_flujo_trabajo = "Imposible encontrar el contenido del diagrama del flujo de trabajo  " & nombre_flujo_trabajo
                Exit Function
            End If
            Solicita_archivo_flujo_trabajo = "YES"
        Catch ex As Exception
            Solicita_archivo_flujo_trabajo = "Inconsistencia general función Solicita_archivo_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_listado_actividades_flujo_trabajo(ByVal id_flujo_trabajo As Integer,
                                                                   ByRef stru_listado() As STRU_ACTIVIDADES_FLUJO_TRABAJO) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO," &
                "listado_actividades_workflow_Id_Actividad,IDENTI_GRAFICA_ACTIVIDAD," &
                "FECHA_REGISTRO,ESTADO_ACTIVIDAD,ID_TIPO_ACTIVIDAD,ID_USUARIO_WORKFLOW," &
                "TIPO_ABIERTA_CERRADA_ACTIVIDAD,ACTIVIDAD_INICIO,ACTIVIDAD_FINAL " &
                "from wf_registro_actividaes_flujos_trabajo where wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_listado_actividades_flujo_trabajo = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_listado = Nothing
                Solicita_estructura_listado_actividades_flujo_trabajo = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_listado(i)
                    stru_listado(i).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO_ANTERIOR = Datset.Tables(0).Rows(i).Item(0)
                    stru_listado(i).listado_actividades_workflow_Id_Actividad = Datset.Tables(0).Rows(i).Item(1)
                    If Datset.Tables(0).Rows(i).IsNull(2) Then
                        stru_listado(i).IDENTI_GRAFICA_ACTIVIDAD = 0
                    Else
                        stru_listado(i).IDENTI_GRAFICA_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(2)
                    End If
                    stru_listado(i).FECHA_REGISTRO = Datset.Tables(0).Rows(i).Item(3)
                    Dim refclas_ClassGestionFechas As New ClassGestionFechas
                    Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Almacenamiento_Time_bsd(stru_listado(i).FECHA_REGISTRO)
                    If Result <> "YES" Then
                        Solicita_estructura_listado_actividades_flujo_trabajo = Result
                        Exit Function
                    End If
                    stru_listado(i).ESTADO_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(4)
                    stru_listado(i).ID_TIPO_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(5)
                    If Datset.Tables(0).Rows(i).IsNull(6) Then
                        stru_listado(i).ID_USUARIO_WORKFLOW = "NULL"
                    Else
                        stru_listado(i).ID_USUARIO_WORKFLOW = Datset.Tables(0).Rows(i).Item(6)
                    End If
                    stru_listado(i).TIPO_ABIERTA_CERRADA_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(7)
                    stru_listado(i).ACTIVIDAD_INICIO = Datset.Tables(0).Rows(i).Item(8)
                    stru_listado(i).ACTIVIDAD_FINAL = Datset.Tables(0).Rows(i).Item(9)
                    Dim stru_conector() As STRU_REGISTRO_CONECTORES_FLUJOS_TRABAJO = Nothing
                    Result = Solicita_conectores_relacionados_a_la_actividad(stru_listado(i).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO_ANTERIOR,
                                                                     id_flujo_trabajo, stru_listado(i).STRU_CONECTOR)
                    If Result <> "YES" Then
                        Solicita_estructura_listado_actividades_flujo_trabajo = Result
                        Exit Function
                    End If
                Next
                Solicita_estructura_listado_actividades_flujo_trabajo = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_estructura_listado_actividades_flujo_trabajo = "Inconsistencia general función Solicita_estructura_listado_actividades_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_conectores_relacionados_a_la_actividad(ByVal id_registro_flujo_actividad_fuente As Integer,
                                                             ByVal id_flujo_trabajo As Integer,
                                                             ByRef stru_conector() As STRU_REGISTRO_CONECTORES_FLUJOS_TRABAJO) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select ID_REGISTRO_ACTIVIDAD_ENVIO,  " &
                "wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,ID_ACTIVIDAD_FUENTE,ID_ACTIVIDAD_DESTINO," &
                "IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE,IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO," &
                "ID_USUARIO_WORKFLOW_FUENTE,ID_USUARIO_WORKFLOW_DESTINO " &
                "from wf_registro_conectores_actividades_envio_flujo_trabajo where IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE=" & id_registro_flujo_actividad_fuente &
                " and wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_conectores_actividades_envio_flujo_trabajo")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_conectores_relacionados_a_la_actividad = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_conector = Nothing
                Solicita_conectores_relacionados_a_la_actividad = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_conector(i)
                    stru_conector(i).ID_REGISTRO_ACTIVIDAD_ENVIO_ANTERIOR = Datset.Tables(0).Rows(i).Item(0)
                    stru_conector(i).wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = Datset.Tables(0).Rows(i).Item(1)
                    stru_conector(i).ID_ACTIVIDAD_FUENTE = Datset.Tables(0).Rows(i).Item(2)
                    stru_conector(i).ID_ACTIVIDAD_DESTINO = Datset.Tables(0).Rows(i).Item(3)
                    stru_conector(i).IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE = Datset.Tables(0).Rows(i).Item(4)
                    stru_conector(i).IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO = Datset.Tables(0).Rows(i).Item(5)
                    If Datset.Tables(0).Rows(i).IsNull(6) Then
                        stru_conector(i).ID_USUARIO_WORKFLOW_FUENTE = "Null"
                    Else
                        stru_conector(i).ID_USUARIO_WORKFLOW_FUENTE = Datset.Tables(0).Rows(i).Item(6)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(7) Then
                        stru_conector(i).ID_USUARIO_WORKFLOW_DESTINO = "Null"
                    Else
                        stru_conector(i).ID_USUARIO_WORKFLOW_DESTINO = Datset.Tables(0).Rows(i).Item(7)
                    End If
                Next
                Solicita_conectores_relacionados_a_la_actividad = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_conectores_relacionados_a_la_actividad = "Inconsistencia general función Solicita_conectores_relacionados_a_la_actividad " & ex.Message
        End Try
    End Function
    Function Registra_flujo_trabajo_para_copia(ByVal nombre_flujo_trabajo As String,
                                                 ByVal descripcion_flujo_trabajo As String, ByVal tipo_flujo_trabajo As String _
                                                 , ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                                 ByRef id_flujo_trabajo As Integer, ByVal id_ruta As Integer,
                                                 ByVal nombre_flujo_copia As String) As String
        Try
            If nombre_flujo_trabajo = "" Then
                Registra_flujo_trabajo_para_copia = "Debe informar el nombre del flujo de trabajo"
                Exit Function
            End If
            If descripcion_flujo_trabajo = "" Then
                Registra_flujo_trabajo_para_copia = "Debe informar la descripción del flujo de trabajo"
                Exit Function
            End If
            If tipo_flujo_trabajo = "" Then
                Registra_flujo_trabajo_para_copia = "Debe seleccionar el tipo de flujo de trabajo"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ""
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Registra_flujo_trabajo_para_copia = "Error formateando fecha almacenamiento Función: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            Dim tipo_flujo_trabajo_numeral As Integer = 0
            If tipo_flujo_trabajo = "Cerrado" Then
                tipo_flujo_trabajo_numeral = 1
            Else
                tipo_flujo_trabajo_numeral = 0
            End If
            Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../configuracion/dlc_flujo.vdx")
            If File.Exists(ruta_plantilla) = False Then
                Registra_flujo_trabajo_para_copia = "Imposible encontrar el archivo " & ruta_plantilla
                Exit Function
            End If
            Dim ob As New MindFusion.Diagramming.Import.VisioImporter
            Dim diagran As New MindFusion.Diagramming.DiagramDocument
            Dim digran_page As New MindFusion.Diagramming.DiagramPage
            diagran = ob.Import(ruta_plantilla)
            If diagran.Pages.Count = 0 Then
                Registra_flujo_trabajo_para_copia = "La plantilla base no tiene el formato correcto, no contiene por lo menos un diagrama "
                Exit Function
            End If
            ref_diagram.Diagram.Items.Clear()
            ref_diagram.Diagram = diagran.Pages(0)
            ref_diagram.ZoomFactor = 75
            Dim Binario As Byte() = Nothing
            Dim ruta_archivo As String = ""
            Dim string_plantilla As String = ""
            string_plantilla = ref_diagram.SaveToString(SaveToStringFormat.Base64, True)
            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim myTrans As MySqlTransaction
            Dim sqlresultinsert As Integer = 0
            ref.Returna_Conexion_Mysql(myConnection)
            Dim sql_insert As String = "insert into wf_flujos_trabajo (rutas_workflow_id_Ruta,NOMBRE_FLUJO_TRABAJO,FECHA_FLUJO,TIPO_RUTA_ABIERTA_CERRADA,DESCRIPCION_FLUJO,Archivo_Plantilla_Mindifucion)  values " &
            "(?id_ruta,?nombre_flujo_trabajo,?fecha_flujo,?tipo_ruta,?descripcion_flujo,?archivo)"
            Try
                Dim myCommand As MySqlCommand = myConnection.CreateCommand()
                myTrans = myConnection.BeginTransaction()
                myCommand.Connection = myConnection
                myCommand.Transaction = myTrans
                myCommand.CommandText = sql_insert
                myCommand.Parameters.AddWithValue("?id_ruta", id_ruta)
                myCommand.Parameters.AddWithValue("?nombre_flujo_trabajo", nombre_flujo_trabajo)
                myCommand.Parameters.AddWithValue("?fecha_flujo", date1al)
                myCommand.Parameters.AddWithValue("?tipo_ruta", tipo_flujo_trabajo_numeral)
                myCommand.Parameters.AddWithValue("?descripcion_flujo", descripcion_flujo_trabajo)
                myCommand.Parameters.AddWithValue("?archivo", string_plantilla)
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Registra_flujo_trabajo_para_copia = "Imposible crear el nuevo flujo de trabajo como copia  "
                    myConnection.Close()
                    Exit Function
                End If
                id_flujo_trabajo = myCommand.LastInsertedId
                Dim hor As String = Now
                Dim campos As String = "Duplica el flujo de trabajo " & nombre_flujo_copia & " con el nombre " & nombre_flujo_trabajo
                Dim sqlforupdate As String = "INSERT INTO wf_registro_log_flujos_trabajo (DESC_OP,USER_OPER,ID_USER,DATE_TRANS,wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO" &
               ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
               "'DUPLICA FLUJO DE TRABAJO','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") &
               "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                id_flujo_trabajo & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR DOCUMENTAL','" & campos & "')"
                myCommand.CommandText = sqlforupdate
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Registra_flujo_trabajo_para_copia = "Imposible registrar log duplicar flujo trabajo "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                myTrans.Commit()
                myConnection.Close()
                Registra_flujo_trabajo_para_copia = "YES"
            Catch e As Exception
                Try
                    myTrans.Rollback()
                Catch ex As MySqlException
                    If Not myTrans.Connection Is Nothing Then
                        Registra_flujo_trabajo_para_copia = "An exception of type " + ex.GetType().ToString() +
                                          " was encountered while attempting to roll back the transaction."
                        myConnection.Close()
                        Exit Function
                    End If
                End Try
                myConnection.Close()
                Registra_flujo_trabajo_para_copia = "Error General " & e.Message
                Exit Function
            End Try
        Catch ex As Exception
            Registra_flujo_trabajo_para_copia = "Inconsistencia general función Registra_flujo_trabajo_para_copia  " & ex.Message
        End Try
    End Function
    Function Registra_actividades_flujo_trabajo_asinga_id_nueva_actividad(ByVal id_nuevo_flujo_trabajo As Integer,
                                                                          ByRef stru_listado() As STRU_ACTIVIDADES_FLUJO_TRABAJO) As String
        Try

            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim myTrans As MySqlTransaction
            Dim sqlresultinsert As Integer = 0
            ref.Returna_Conexion_Mysql(myConnection)
            Dim sql_insert As String = ""
            Dim Result As String = ""
            Try
                Dim myCommand As MySqlCommand = myConnection.CreateCommand()
                myTrans = myConnection.BeginTransaction()
                myCommand.Connection = myConnection
                myCommand.Transaction = myTrans
                For i As Integer = 0 To stru_listado.Length - 1
                    sql_insert = "INSERT INTO wf_registro_actividaes_flujos_trabajo (wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO," &
                        "listado_actividades_workflow_Id_Actividad,FECHA_REGISTRO,ESTADO_ACTIVIDAD,ID_TIPO_ACTIVIDAD," &
                        "ID_USUARIO_WORKFLOW,TIPO_ABIERTA_CERRADA_ACTIVIDAD,ACTIVIDAD_INICIO,ACTIVIDAD_FINAL) VALUES (" &
                        id_nuevo_flujo_trabajo & "," & stru_listado(i).listado_actividades_workflow_Id_Actividad &
                        ",'" & stru_listado(i).FECHA_REGISTRO & "'," & stru_listado(i).ESTADO_ACTIVIDAD & "," & stru_listado(i).ID_TIPO_ACTIVIDAD &
                        "," & stru_listado(i).ID_USUARIO_WORKFLOW & "," & stru_listado(i).TIPO_ABIERTA_CERRADA_ACTIVIDAD & "," &
                        stru_listado(i).ACTIVIDAD_INICIO & "," & stru_listado(i).ACTIVIDAD_FINAL & ")"
                    myCommand.CommandText = sql_insert
                    sqlresultinsert = myCommand.ExecuteNonQuery()
                    If sqlresultinsert = 0 Then
                        If i > 0 Then
                            myTrans.Rollback()
                        End If
                        Registra_actividades_flujo_trabajo_asinga_id_nueva_actividad = "Imposible crear la actividad del flujo de trabajo "
                        myConnection.Close()
                        Exit Function
                    End If
                    stru_listado(i).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = myCommand.LastInsertedId

                Next
                myTrans.Commit()
                myConnection.Close()
                Registra_actividades_flujo_trabajo_asinga_id_nueva_actividad = "YES"
            Catch e As Exception
                Try
                    myTrans.Rollback()
                Catch ex As MySqlException
                    If Not myTrans.Connection Is Nothing Then
                        Registra_actividades_flujo_trabajo_asinga_id_nueva_actividad = "An exception of type " + ex.GetType().ToString() +
                                          " was encountered while attempting to roll back the transaction."
                        myConnection.Close()
                        Exit Function
                    End If
                End Try
                myConnection.Close()
                Registra_actividades_flujo_trabajo_asinga_id_nueva_actividad = "Error General " & e.Message
                Exit Function
            End Try
        Catch ex As Exception
            Registra_actividades_flujo_trabajo_asinga_id_nueva_actividad = "Inconsistencia general función Registra_actividades_flujo_trabajo_asinga_id_nueva_actividad " & ex.Message
        End Try
    End Function
    Function Registra_conectores_actividades_asigna_id_nuevo_conector(ByVal id_nuevo_flujo As Integer,
                                                                          ByRef stru_listado As STRU_ACTIVIDADES_FLUJO_TRABAJO,
                                                                          ByVal stru_listado_() As STRU_ACTIVIDADES_FLUJO_TRABAJO) As String
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Dim sql_insert As String = ""
        Dim Result As String = ""
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            If Not stru_listado.STRU_CONECTOR Is Nothing Then
                For z As Integer = 0 To stru_listado.STRU_CONECTOR.Length - 1
                    Dim nueva_id_actividad_flujo_destino As Integer = 0
                    Result = Me.Retorna_id_actvidad_flujo_destino_nuevo_registro(stru_listado_,
                                                                                 stru_listado.STRU_CONECTOR(z).IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,
                                                                                 nueva_id_actividad_flujo_destino)
                    sql_insert = "INSERT INTO wf_registro_conectores_actividades_envio_flujo_trabajo (wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO," &
               "ID_ACTIVIDAD_FUENTE,ID_ACTIVIDAD_DESTINO,IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE,IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO," &
               "ID_USUARIO_WORKFLOW_FUENTE,ID_USUARIO_WORKFLOW_DESTINO) VALUES (" &
               id_nuevo_flujo & "," & stru_listado.STRU_CONECTOR(z).ID_ACTIVIDAD_FUENTE &
               ",'" & stru_listado.STRU_CONECTOR(z).ID_ACTIVIDAD_DESTINO & "'," & stru_listado.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO & "," _
               & nueva_id_actividad_flujo_destino &
               "," & stru_listado.STRU_CONECTOR(z).ID_USUARIO_WORKFLOW_FUENTE & "," & stru_listado.STRU_CONECTOR(z).ID_USUARIO_WORKFLOW_DESTINO & ")"
                    myCommand.CommandText = sql_insert
                    sqlresultinsert = myCommand.ExecuteNonQuery()
                    If sqlresultinsert = 0 Then
                        If z > 0 Then
                            myTrans.Rollback()
                        End If
                        Registra_conectores_actividades_asigna_id_nuevo_conector = "Imposible crear la actividad del flujo de trabajo "
                        myConnection.Close()
                        Exit Function
                    End If
                    stru_listado.STRU_CONECTOR(z).ID_REGISTRO_ACTIVIDAD_ENVIO = myCommand.LastInsertedId
                Next

            End If
            myTrans.Commit()
            myConnection.Close()
            Registra_conectores_actividades_asigna_id_nuevo_conector = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Registra_conectores_actividades_asigna_id_nuevo_conector = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Registra_conectores_actividades_asigna_id_nuevo_conector = "Error General " & e.Message
            Exit Function
        End Try

    End Function
    Function Retorna_id_actvidad_flujo_destino_nuevo_registro(ByVal stru_listado() As STRU_ACTIVIDADES_FLUJO_TRABAJO,
                                                              ByVal id_actvidad_flujo_trabajo_destino As Integer,
                                                              ByRef nueva_id_actividad_flujo_trabajo As Integer) As String
        Try
            For i As Integer = 0 To stru_listado.Length - 1
                If stru_listado(i).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO_ANTERIOR = id_actvidad_flujo_trabajo_destino Then
                    nueva_id_actividad_flujo_trabajo = stru_listado(i).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO
                    Retorna_id_actvidad_flujo_destino_nuevo_registro = "YES"
                    Exit Function
                End If
            Next
            Retorna_id_actvidad_flujo_destino_nuevo_registro = "YES"
        Catch ex As Exception
            Retorna_id_actvidad_flujo_destino_nuevo_registro = "Inconsistencia general función Retorna_id_actvidad_flujo_destino_nuevo_registro " & ex.Message
        End Try
    End Function
    Function Actualiza_elementos_digrama_nuevos_identificadores(ByVal stru_listado() As STRU_ACTIVIDADES_FLUJO_TRABAJO,
                                                               ByRef ref_diagram_copia As MindFusion.Diagramming.WebForms.DiagramView,
                                                               ByVal id_flujo_trabajo As Integer, ByVal id_nuevo_flujo_trabajo As Integer) As String
        Try
            For i As Integer = 0 To stru_listado.Length - 1
                Dim sap As New MindFusion.Diagramming.ShapeNode
                Dim sha As Object = Nothing
                For Each sha_ As Object In ref_diagram_copia.Diagram.Items
                    Dim ob As Object = sha_.GetType
                    If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                        If sha_.id = stru_listado(i).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO_ANTERIOR Then
                            sha_.id = stru_listado(i).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO
                        End If
                    End If
                    If Not stru_listado(i).STRU_CONECTOR Is Nothing Then
                        For z As Integer = 0 To stru_listado(i).STRU_CONECTOR.Length - 1
                            For Each link_ As Object In ref_diagram_copia.Diagram.Items
                                Dim ob_ink As Object = link_.GetType
                                If ob_ink.Fullname = "MindFusion.Diagramming.DiagramLink" Then
                                    Dim temp_id As String = id_flujo_trabajo.ToString & "_" & stru_listado(i).STRU_CONECTOR(z).ID_REGISTRO_ACTIVIDAD_ENVIO_ANTERIOR
                                    If link_.id = temp_id Then
                                        link_.id = id_nuevo_flujo_trabajo.ToString & "_" & stru_listado(i).STRU_CONECTOR(z).ID_REGISTRO_ACTIVIDAD_ENVIO
                                    End If
                                End If
                            Next
                        Next
                    End If
                Next
            Next
            Actualiza_elementos_digrama_nuevos_identificadores = "YES"
        Catch ex As Exception
            Actualiza_elementos_digrama_nuevos_identificadores = "Inconsistencia general función Actualiza_elementos_digrama_nuevos_identificadores " & ex.Message
        End Try
    End Function
    Function Eliminar_flujo_trabajo(ByVal nombre_flujo_trabajo As String,
                                    ByVal id_flujo_trabajo As Integer,
                                    ByRef drop_list As DropDownList,
                                    ByRef up_date As UpdatePanel,
                                    ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView,
                                    ByRef ref_update As UpdatePanel,
                                    ByVal zon_view As Object) As String

        If nombre_flujo_trabajo = "" Then
            Eliminar_flujo_trabajo = "Debe seleccionar el flujo de trabajo a eliminar"
            Exit Function
        End If
        Dim existencia_relacion As String = ""
        Dim Result As String = ""
        Result = Me.Verfica_existencia_flujo_trabajo_relacionado_a_tramite(id_flujo_trabajo, existencia_relacion)
        If Result <> "YES" Then
            Eliminar_flujo_trabajo = Result
            Exit Function
        End If
        If existencia_relacion = "YES" Then
            Eliminar_flujo_trabajo = "Se han detectado relaciones del flujos de trabajo con trámites, imposible eliminar el flujo de trabajo "
            Exit Function
        End If
        Result = Me.Verifica_registro_flujo_trabajo_ruta(id_flujo_trabajo, existencia_relacion)
        If Result <> "YES" Then
            Eliminar_flujo_trabajo = Result
            Exit Function
        End If
        If existencia_relacion = "YES" Then
            Eliminar_flujo_trabajo = "Se han detectado flujos de trabajo en el sistema, imposible eliminar flujo de trabajo "
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Eliminar_flujo_trabajo = Result
            Exit Function
        End If
        Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../configuracion/dlc_flujo.vdx")
        If File.Exists(ruta_plantilla) = False Then
            Eliminar_flujo_trabajo = "Imposible encontrar el archivo " & ruta_plantilla
            Exit Function
        End If
        Dim ob As New MindFusion.Diagramming.Import.VisioImporter
        Dim diagran As New MindFusion.Diagramming.DiagramDocument
        diagran = ob.Import(ruta_plantilla)
        If diagran.Pages.Count = 0 Then
            Eliminar_flujo_trabajo = "La plantilla base no tiene el formato correcto, no contiene por lo menos un diagrama "
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Dim sql_insert As String = "Delete FROM wf_registro_conectores_actividades_envio_flujo_trabajo " &
            " WHERE   wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_insert
            sqlresultinsert = myCommand.ExecuteNonQuery()
            'If sqlresultinsert = 0 Then
            '    Eliminar_flujo_trabajo = "Imposible elimnar flujo de trabajo  "
            '    myConnection.Close()
            '    Exit Function
            'End If
            sql_insert = "delete FROM wf_registro_actividaes_flujos_trabajo where wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            myCommand.CommandText = sql_insert
            sqlresultinsert = myCommand.ExecuteNonQuery()
            'If sqlresultinsert = 0 Then
            '    Eliminar_flujo_trabajo = "Imposible elimnar actvidades del flujo de trabajo"
            '    myTrans.Rollback()
            '    myConnection.Close()
            '    Exit Function
            'End If
            sql_insert = "delete from  wf_flujos_trabajo where ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            myCommand.CommandText = sql_insert
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Eliminar_flujo_trabajo = "Imposible elimnar el flujo de trabajo"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim hor As String = Now
            Dim campos As String = "Elimina flujo de trabajo " & nombre_flujo_trabajo
            Dim sqlforupdate As String = "INSERT INTO wf_registro_log_flujos_trabajo (DESC_OP,USER_OPER,ID_USER,DATE_TRANS,wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'ELIMINA FLUJO TRABAJO','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") &
           "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
            id_flujo_trabajo & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR DOCUMENTAL','" & campos & "')"
            myCommand.CommandText = sqlforupdate
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Eliminar_flujo_trabajo = "Imposible registrar log flujo trabajo "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            ref_diagram.Diagram.Items.Clear()
            ref_diagram.Diagram = diagran.Pages(0)
            ref_diagram.ZoomFactor = zon_view
            drop_list.Items.Remove(nombre_flujo_trabajo)
            up_date.Update()
            ref_update.Update()
            HttpContext.Current.Session.Item("DR_FLUJO_SELECCIONADO") = ""
            HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO") = 0
            myTrans.Commit()
            myConnection.Close()
            Eliminar_flujo_trabajo = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Eliminar_flujo_trabajo = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Eliminar_flujo_trabajo = "Error General " & e.Message
            Exit Function
        End Try
    End Function

    Function Verfica_existencia_flujo_trabajo_relacionado_a_tramite(ByVal id_flujo_trabajo As Integer,
                                                                    ByRef existencia_relacion As String) As String
        '---------------------------------------------
        'Función : Verifica relación del flujo de
        'trabajo con tipos de tramites
        'Fecha : 2018-02-28
        'Ingeniero : Miguel Angel Urueta Miranda
        '---------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_WF_FLUJO_TRABAJO from ra_relacion_tramite_flujo_wokflow where ID_WF_FLUJO_TRABAJO =" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_tramite_flujo_wokflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verfica_existencia_flujo_trabajo_relacionado_a_tramite = "Error función Verfica_existencia_flujo_trabajo_relacionado_a_tramite " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                existencia_relacion = "YES"
                Verfica_existencia_flujo_trabajo_relacionado_a_tramite = "YES"
                Exit Function
            Else
                existencia_relacion = "NO"
                Verfica_existencia_flujo_trabajo_relacionado_a_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verfica_existencia_flujo_trabajo_relacionado_a_tramite = "Inconsistencia general función Verfica_existencia_flujo_trabajo_relacionado_a_tramite " & ex.Message
        End Try

    End Function
    Function Verifica_registro_flujo_trabajo_ruta(ByVal id_flujo_trabajo As Integer,
                                                ByRef existencia_relacion As String) As String
        '---------------------------------------------
        'Función : Verifica relación del flujo de
        'trabajo con registros en la ruta
        'Fecha : 2018-02-28
        'Ingeniero : Miguel Angel Urueta Miranda
        '---------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select id_Estado from estados_tarea_workflow where ID_FLUJO_TRABAJO=" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_registro_flujo_trabajo_ruta = "Error función Verifica_registro_flujo_trabajo_ruta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                existencia_relacion = "YES"
                Verifica_registro_flujo_trabajo_ruta = "YES"
                Exit Function
            Else
                existencia_relacion = "NO"
                Verifica_registro_flujo_trabajo_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_registro_flujo_trabajo_ruta = "Inconsistencia general función Verifica_registro_flujo_trabajo_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_estado_actividad_inicio(ByVal ModalPopupExtender_edition_activa_actividad_inicio As AjaxControlToolkit.ModalPopupExtender,
                                             ByRef Check_actividad_inicio As CheckBox,
                                             ByRef UpdatePanel_activa_actividad_inicio As UpdatePanel,
                                             ByVal ref_diagramView As DiagramView) As String
        '------------------------------------------------------------------
        'Función : Solicita el estado de la actividad si esta configurada
        'como actividad de inicio, el parametro de activacion (1) se 
        'encuentra activa como inicio, (0) se encuentra inactiva
        'Fecha : 2018-03-01
        'Ingeniero : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------
        Try
            Dim Result As String = ""
            If ref_diagramView.Diagram.Selection.Items.Count = 0 Then
                Solicita_estado_actividad_inicio = "Por favor seleccione el elemento del diagrama "
                Exit Function
            End If
            If ref_diagramView.Diagram.Selection.Items.Count > 1 Then
                Solicita_estado_actividad_inicio = "Solo se puede configurar un elemento del diagrama"
                Exit Function
            End If
            If ref_diagramView.Diagram.Selection.Items(0).GetType.FullName <> "MindFusion.Diagramming.ShapeNode" Then
                Solicita_estado_actividad_inicio = "El elemento seleccionado no se puede activar como actividad de inicio"
                Exit Function
            End If
            Dim id_actividad As Integer = ref_diagramView.Diagram.Selection.Items(0).Id
            Dim estado_inicio As Integer = 0
            Dim Parametro_Consulta As String = "select  ACTIVIDAD_INICIO from wf_registro_actividaes_flujos_trabajo where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO ='" & id_actividad & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_actividad_inicio = "Error función Solicita_estado_actividad_inicio " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_inicio = Datset.Tables(0).Rows(0).Item(0)
                If estado_inicio = 1 Then
                    Check_actividad_inicio.Checked = True
                    UpdatePanel_activa_actividad_inicio.Update()
                Else
                    Check_actividad_inicio.Checked = False
                    UpdatePanel_activa_actividad_inicio.Update()
                End If
                Solicita_estado_actividad_inicio = "YES"
                Exit Function
            Else
                Solicita_estado_actividad_inicio = "Imposible encontrar el estado de inicio flujo de trabajo de la actividad (" & id_actividad & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_actividad_inicio = "Inconsistencia general función Solicita_estado_actividad_inicio " & ex.Message
        End Try
    End Function
    Function Activa_desactiva_actividad_inicio(ByVal ModalPopupExtender_edition_activa_actividad_inicio As AjaxControlToolkit.ModalPopupExtender,
                                               ByRef Check_actividad_inicio As CheckBox,
                                               ByRef UpdatePanel_diagran_view As UpdatePanel,
                                               ByVal ref_diagramView As DiagramView,
                                               ByVal id_flujo_trabajo As Integer) As String
        '-------------------------------------------------------
        'Función : Activa o desactiva el estado de inicio de una
        'actividad
        'Fecha : 2018-03-01
        'Ingemiero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------
        Try
            Dim Result As String = ""
            If ref_diagramView.Diagram.Selection.Items.Count = 0 Then
                Activa_desactiva_actividad_inicio = "Por favor seleccione el elemento del diagrama "
                Exit Function
            End If
            If ref_diagramView.Diagram.Selection.Items.Count > 1 Then
                Activa_desactiva_actividad_inicio = "Solo se puede configurar un elemento del diagrama"
                Exit Function
            End If
            If ref_diagramView.Diagram.Selection.Items(0).GetType.FullName <> "MindFusion.Diagramming.ShapeNode" Then
                Activa_desactiva_actividad_inicio = "El elemento seleccionado no se puede activar como actividad de inicio"
                Exit Function
            End If
            Dim id_actividad As Integer = ref_diagramView.Diagram.Selection.Items(0).Id
            Dim estado_inicio As Integer = 0
            If Check_actividad_inicio.Checked = True Then
                estado_inicio = 1
            End If
            Dim Sql_update As String = "Update wf_registro_actividaes_flujos_trabajo set ACTIVIDAD_INICIO=" & estado_inicio &
                " where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(Sql_update)
            If Result <> "YES" Then
                Activa_desactiva_actividad_inicio = Result
                Exit Function
            Else
                If estado_inicio = 0 Then
                    Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush(
                    Color.White, Color.White, 0)
                    ref_diagramView.Diagram.Selection.Items(0).Brush = penBrush
                Else
                    Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush(
                    Color.Yellow, Color.Yellow, 0)
                    ref_diagramView.Diagram.Selection.Items(0).Brush = penBrush
                End If
                '--------------------------------------------------
                'Guarda el archivo en el sistema de archivo
                '--------------------------------------------------
                Dim string_diagrama As String = ref_diagramView.SaveToString(SaveToStringFormat.Base64, True)
                '-------------------------------------------------
                'Guarda el archivo en la base de datos
                '-------------------------------------------------
                Result = Me.Guarda_archivo_base_datos_flujo_trabajo_string(string_diagrama, id_flujo_trabajo)
                If Result <> "YES" Then
                    Activa_desactiva_actividad_inicio = Result
                    Exit Function
                Else
                    UpdatePanel_diagran_view.Update()
                    Activa_desactiva_actividad_inicio = "YES"
                    Exit Function
                End If

            End If
        Catch ex As Exception
            Activa_desactiva_actividad_inicio = "Inconsistencia general función Activa_desactiva_actividad_inicio " & ex.Message
        End Try
    End Function

    Function Solicita_datos_caracterizacion_flujo_trabajo(ByVal id_flujo_trabajo As Integer,
                                                          ByRef TextBox_Edita_nombre_flujo_trabajo As TextBox,
                                                          ByRef TextBox_Edita_descripcion_flujo_trabajo As TextBox,
                                                          ByRef UpdatePanel_edita_flujo_trabajo As UpdatePanel) As String
        Try
            Dim Parametro_Consulta As String = "select  NOMBRE_FLUJO_TRABAJO,DESCRIPCION_FLUJO from wf_flujos_trabajo where ID_WF_FLUJOS_TRABAJO =" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_caracterizacion_flujo_trabajo = "Error función Solicita_datos_caracterizacion_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                TextBox_Edita_nombre_flujo_trabajo.Text = Datset.Tables(0).Rows(0).Item(0)
                TextBox_Edita_descripcion_flujo_trabajo.Text = Datset.Tables(0).Rows(0).Item(1)
                UpdatePanel_edita_flujo_trabajo.Update()
                Solicita_datos_caracterizacion_flujo_trabajo = "YES"
                Exit Function
            Else
                TextBox_Edita_nombre_flujo_trabajo.Text = ""
                TextBox_Edita_descripcion_flujo_trabajo.Text = ""
                UpdatePanel_edita_flujo_trabajo.Update()
                Solicita_datos_caracterizacion_flujo_trabajo = "Imposible encontrar datos de caracterización del flujo de trabajo (" & id_flujo_trabajo & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_caracterizacion_flujo_trabajo = "Inconsistencia general función Solicita_datos_caracterizacion_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_datos_caracterizacion_flujo_trabajo_anterior(ByVal id_flujo_trabajo As Integer,
                                                          ByRef nombre_flujo_trabajo As String,
                                                          ByRef descripcion_flujo_trabajo As String
                                                          ) As String
        Try
            Dim Parametro_Consulta As String = "select  NOMBRE_FLUJO_TRABAJO,DESCRIPCION_FLUJO from wf_flujos_trabajo where ID_WF_FLUJOS_TRABAJO =" & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_caracterizacion_flujo_trabajo_anterior = "Error función Solicita_datos_caracterizacion_flujo_trabajo_anterior " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                descripcion_flujo_trabajo = Datset.Tables(0).Rows(0).Item(1)
                Solicita_datos_caracterizacion_flujo_trabajo_anterior = "YES"
                Exit Function
            Else
                nombre_flujo_trabajo = ""
                descripcion_flujo_trabajo = ""
                Solicita_datos_caracterizacion_flujo_trabajo_anterior = "Imposible encontrar datos de caracterización del flujo de trabajo (" & id_flujo_trabajo & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_caracterizacion_flujo_trabajo_anterior = "Inconsistencia general función Solicita_datos_caracterizacion_flujo_trabajo_anterior " & ex.Message
        End Try
    End Function
    Function Actualiza_datos_caracterizacion_flujo_trabajo(ByRef DropDownList_flujos_disponibles_workflow As DropDownList,
                                                           ByRef updatemenu As UpdatePanel, ByVal id_flujo_trabajo As Integer,
                                                           ByVal id_tuta As Integer,
                                                          ByRef TextBox_Edita_nombre_flujo_trabajo As TextBox,
                                                          ByRef TextBox_Edita_descripcion_flujo_trabajo As TextBox) As String


        Dim Result As String = ""
        Dim existencia_flujo As String = ""
        Dim nombre_flujo_trabajo As String = ""
        Dim descripcion_flujo_trabajo As String = ""
        Result = Me.Solicita_datos_caracterizacion_flujo_trabajo_anterior(id_flujo_trabajo, nombre_flujo_trabajo,
                                                                          descripcion_flujo_trabajo)
        If Result <> "YES" Then
            Actualiza_datos_caracterizacion_flujo_trabajo = Result
            Exit Function
        End If
        If UCase(nombre_flujo_trabajo) = UCase(TextBox_Edita_nombre_flujo_trabajo.Text) And
           TextBox_Edita_descripcion_flujo_trabajo.Text = descripcion_flujo_trabajo Then
            Actualiza_datos_caracterizacion_flujo_trabajo = "No se detectarón cambios de caracterización para actualizar"
            Exit Function
        End If
        If UCase(nombre_flujo_trabajo) <> UCase(TextBox_Edita_nombre_flujo_trabajo.Text) Then
            Result = Verifica_existencia_flujo_trabajo(UCase(TextBox_Edita_nombre_flujo_trabajo.Text),
                                                 id_tuta, existencia_flujo)
            If Result <> "YES" Then
                Actualiza_datos_caracterizacion_flujo_trabajo = Result
                Exit Function
            End If
            If existencia_flujo = "YES" Then
                Actualiza_datos_caracterizacion_flujo_trabajo = "El flujo de trabajo (" & DropDownList_flujos_disponibles_workflow.Text &
                    ") se encuentra registrado, imposible actualizar este nombre "
                Exit Function
            End If
        End If
        Dim existencia As String = ""
        Result = Me.Verfica_existencia_flujo_trabajo_relacionado_a_tramite(id_flujo_trabajo, existencia)
        If Result <> "YES" Then
            Actualiza_datos_caracterizacion_flujo_trabajo = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Actualiza_datos_caracterizacion_flujo_trabajo = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Dim sql_insert As String = "Update wf_flujos_trabajo set NOMBRE_FLUJO_TRABAJO='" & Trim(TextBox_Edita_nombre_flujo_trabajo.Text) & "'," &
            "DESCRIPCION_FLUJO='" & Trim(TextBox_Edita_descripcion_flujo_trabajo.Text) & "'" &
            " WHERE ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sql_insert
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_datos_caracterizacion_flujo_trabajo = "Imposible elimnar actualizar datos de caracterización flujo de trabajo  "
                myConnection.Close()
                Exit Function
            End If

            Dim hor As String = Now
            Dim campos As String = "Actualiza caracterización flujo de trabajo "
            If UCase(nombre_flujo_trabajo) <> UCase(TextBox_Edita_nombre_flujo_trabajo.Text) Then
                campos = campos & "Se cambió el nombre del flujo de trabajo (" & nombre_flujo_trabajo & ")  con el nuevo nombre  " & UCase(TextBox_Edita_nombre_flujo_trabajo.Text) & " "
            End If
            If TextBox_Edita_descripcion_flujo_trabajo.Text <> descripcion_flujo_trabajo Then
                campos = campos & "Se cambió la descripción del flujo de trabajo (" & descripcion_flujo_trabajo & ")  por la nueva descripción  " & TextBox_Edita_descripcion_flujo_trabajo.Text
            End If
            Dim sqlforupdate As String = "INSERT INTO wf_registro_log_flujos_trabajo (DESC_OP,USER_OPER,ID_USER,DATE_TRANS,wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'ACTUALIZA CARACTERIZACIÓN FLUJO DE TRABAJO','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") &
           "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
            id_flujo_trabajo & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTOR DOCUMENTAL','" & campos & "')"
            myCommand.CommandText = sqlforupdate
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_datos_caracterizacion_flujo_trabajo = "Imposible registrar log flujo trabajo "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If UCase(nombre_flujo_trabajo) <> UCase(TextBox_Edita_nombre_flujo_trabajo.Text) Then
                For i As Integer = 0 To DropDownList_flujos_disponibles_workflow.Items.Count
                    If UCase(DropDownList_flujos_disponibles_workflow.Items(i).Text) = UCase(nombre_flujo_trabajo) Then
                        DropDownList_flujos_disponibles_workflow.Items(i).Text = UCase(TextBox_Edita_nombre_flujo_trabajo.Text)
                        DropDownList_flujos_disponibles_workflow.Items(i).Value = UCase(TextBox_Edita_nombre_flujo_trabajo.Text)
                        Exit For
                    End If
                Next
            End If
            updatemenu.Update()
            myTrans.Commit()
            myConnection.Close()
            If existencia = "YES" Then
                Result = Me.Actualiza_nombre_flujo_relacion_tramite(id_flujo_trabajo, TextBox_Edita_nombre_flujo_trabajo.Text)
                If Result <> "YES" Then
                    Actualiza_datos_caracterizacion_flujo_trabajo = "Se actualizo la caracterización del flujo de trabajo, pero no se actualizo el nombre en las relaciones de con trámites (" & Result & ")"
                    Exit Function
                Else
                    Actualiza_datos_caracterizacion_flujo_trabajo = "YES"
                    Exit Function
                End If
            Else
                Actualiza_datos_caracterizacion_flujo_trabajo = "YES"
                Exit Function
            End If


        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_datos_caracterizacion_flujo_trabajo = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Actualiza_datos_caracterizacion_flujo_trabajo = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Actualiza_nombre_flujo_relacion_tramite(ByVal id_flujo As Integer,
                                                     ByVal nombre_flujo As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_relacion_tramite_flujo_wokflow")
            Dim Parametro_Consulta As String = "update ra_relacion_tramite_flujo_wokflow set NOMBRE_FLUJO_TRABAJO='" & nombre_flujo _
                                               & "' where ID_WF_FLUJO_TRABAJO=" & id_flujo
            Dim Result As String = ref.SELECTION_INSERT_COMMAND(Parametro_Consulta)
            If Result <> "YES" Then
                Actualiza_nombre_flujo_relacion_tramite = Result
                Exit Function
            Else
                Actualiza_nombre_flujo_relacion_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_nombre_flujo_relacion_tramite = "Inconsistencia general función Actualiza_nombre_flujo_relacion_tramite "
        End Try
    End Function
    Function Relacionar_tramites_documentales_a_flujos_de_trabajo(ByVal id_flujo_trabajo As Integer,
                                                                  ByVal id_tipo_tramite As Integer,
                                                                  ByRef nodes_ As TreeNode,
                                                                  ByRef ref_update As UpdatePanel) As String

        Dim id_plantilla_radicado As Integer = 0
        Dim Result As String = ""
        Result = Me.Solicita_id_plantilla_tipo_tramite(id_tipo_tramite,
                                                       id_plantilla_radicado)
        If Result <> "YES" Then
            Relacionar_tramites_documentales_a_flujos_de_trabajo = Result
            Exit Function
        End If
        If id_plantilla_radicado = 0 Then
            Relacionar_tramites_documentales_a_flujos_de_trabajo = "Plantilla radicado no relacionada al trámite "
            Exit Function
        End If
        Dim existencia As String = ""
        Result = Me.Solicita_existencia_relacion_tramite_flujo_trabajo(id_tipo_tramite,
                                                                       id_plantilla_radicado,
                                                                       id_flujo_trabajo,
                                                                       existencia)
        If Result <> "YES" Then
            Relacionar_tramites_documentales_a_flujos_de_trabajo = Result
            Exit Function
        End If
        If existencia = "YES" Then
            Relacionar_tramites_documentales_a_flujos_de_trabajo = "Existe una relación del flujo de trabajo, imposible agregar la relación "
            Exit Function
        End If
        Dim nombre_tipo_tramite As String = ""
        Dim ref_class_tipo_doc As New Class_tipo_doc_entrante
        Result = ref_class_tipo_doc.Solicita_nombre_tipo_tramite_por_id_tramite(id_tipo_tramite,
                                                                                nombre_tipo_tramite)
        If Result <> "YES" Then
            Relacionar_tramites_documentales_a_flujos_de_trabajo = Result
            Exit Function
        End If
        Dim nombre_flujo_trabajo As String = ""
        Result = Me.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo,
                                                              nombre_flujo_trabajo)
        If Result <> "YES" Then
            Relacionar_tramites_documentales_a_flujos_de_trabajo = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Dim Sql_Insercion As String = "Insert into ra_relacion_tramite_flujo_wokflow (system_plantilla_radicado_id_Plantilla," &
            "tipo_doc_entrante_id_Tipo_Doc_Entrante,ID_WF_FLUJO_TRABAJO,ESTADO_RELACION,NOMBRE_FLUJO_TRABAJO) values (" &
           id_plantilla_radicado & "," & id_tipo_tramite & "," & id_flujo_trabajo & ",1,'" & nombre_flujo_trabajo & "')"
        ref.Returna_Conexion_Mysql(myConnection)
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Sql_Insercion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Relacionar_tramites_documentales_a_flujos_de_trabajo = "Imposible relacionar tipo tramite a flujo documental  "
                myConnection.Close()
                Exit Function
            End If
            Dim ident = myCommand.LastInsertedId
            Dim tre_node As New TreeNode
            tre_node.Value = "TRA|" & ident
            tre_node.Text = nombre_tipo_tramite & " (Trámite relacionado)"
            tre_node.ImageUrl = "../workflow/imageneswf/id-card-light.png"
            nodes_.ChildNodes.Add(tre_node)
            nodes_.ExpandAll()
            ref_update.Update()
            myTrans.Commit()
            myConnection.Close()
            Relacionar_tramites_documentales_a_flujos_de_trabajo = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Relacionar_tramites_documentales_a_flujos_de_trabajo = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    myConnection.Close()
                    Exit Function
                End If
            End Try
            myConnection.Close()
            Relacionar_tramites_documentales_a_flujos_de_trabajo = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Solicita_existencia_relacion_tramite_flujo_trabajo(ByVal id_tipo_tramite As Integer,
                                                                ByVal id_plantilla As Integer,
                                                                ByVal id_flujo As Integer,
                                                                ByRef existencia As String) As String
        '--------------------------------------------------------------
        'Función : Solicita_existencia_relacion_tramite_flujo_trabajo
        'verifica existencia relación flujo trabjo tipo tramite
        'Fecha : 2018-09-04
        'Ing Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = " Select ID_RELACION_TRAMITE from  ra_relacion_tramite_flujo_wokflow " &
                                               " where tipo_doc_entrante_id_Tipo_Doc_Entrante = " & id_tipo_tramite &
                                               " and system_plantilla_radicado_id_Plantilla=" & id_plantilla &
                                               " and ID_WF_FLUJO_TRABAJO=" & id_flujo
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_relacion_tramite_flujo_wokflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_relacion_tramite_flujo_trabajo = "Error función Solicita_existencia_relacion_tramite_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                existencia = "YES"
                Solicita_existencia_relacion_tramite_flujo_trabajo = "YES"
                Exit Function
            Else
                existencia = "NO"
                Solicita_existencia_relacion_tramite_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_relacion_tramite_flujo_trabajo = "Inconsistencia general función Solicita_existencia_relacion_tramite_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_id_plantilla_tipo_tramite(ByVal id_tipo_tramite As Integer,
                                                ByRef id_plantilla As Integer) As String
        '--------------------------------------------------------
        'Función : Solicita la indentificaición de la plantilla
        'a la que pertenece el tipo tramite
        'Fecha : 2018-09-04
        'Ing Miguel Angel Urueta Miranda
        '--------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select system_plantilla_radicado_id_plantilla from  tipo_doc_entrante " &
                                               " where id_Tipo_Doc_Entrante = " & id_tipo_tramite
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_plantilla_tipo_tramite = "Error función Solicita_id_plantilla_tipo_tramite " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_plantilla = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_plantilla_tipo_tramite = "YES"
                Exit Function
            Else
                id_plantilla = 0
                Solicita_id_plantilla_tipo_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_plantilla_tipo_tramite = "Inconsistencia general función Solicita_id_plantilla_tipo_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_relacion_tramite_flujo(ByVal id_relacion_tramite_flujo As Integer,
                                                              ByRef srtru_rel_tra As stru_relacion) As String
        Try
            Dim Parametro_Consulta As String = "select ID_RELACION_TRAMITE,system_plantilla_radicado_id_Plantilla," &
                "tipo_doc_entrante_id_Tipo_Doc_Entrante,ID_WF_FLUJO_TRABAJO,ESTADO_RELACION" &
                " from ra_relacion_tramite_flujo_wokflow " &
                                               " where ID_RELACION_TRAMITE = " & id_relacion_tramite_flujo
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_relacion_tramite_flujo_wokflow")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_estructura_relacion_tramite_flujo = "Error función Solicita_datos_estructura_relacion_tramite_flujo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                srtru_rel_tra.ID_RELACION_TRAMITE = Datset.Tables(0).Rows(0).Item(0)
                srtru_rel_tra.system_plantilla_radicado_id_Plantilla = Datset.Tables(0).Rows(0).Item(1)
                srtru_rel_tra.tipo_doc_entrante_id_Tipo_Doc_Entrante = Datset.Tables(0).Rows(0).Item(2)
                srtru_rel_tra.ID_WF_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(3)
                srtru_rel_tra.ESTADO_RELACION = Datset.Tables(0).Rows(0).Item(4)
                Solicita_datos_estructura_relacion_tramite_flujo = "YES"
                Exit Function
            Else
                srtru_rel_tra.ID_RELACION_TRAMITE = 0
                Solicita_datos_estructura_relacion_tramite_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_relacion_tramite_flujo = "Inconsistencia genera función Solicita_datos_estructura_relacion_tramite_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_actvidad_inicio_flujo_trabajo(ByVal id_flujo_trabajo As Integer,
                                                               ByRef existencia_actividad As String) As String
        Try
            Dim Parametro_Consulta As String = "select ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO from  wf_registro_actividaes_flujos_trabajo " &
                                              " where wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = " & id_flujo_trabajo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_actvidad_inicio_flujo_trabajo = "Error función Solicita_existencia_actvidad_inicio_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                existencia_actividad = "YES"
                Solicita_existencia_actvidad_inicio_flujo_trabajo = "YES"
                Exit Function
            Else
                existencia_actividad = "YES"
                Solicita_existencia_actvidad_inicio_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_actvidad_inicio_flujo_trabajo = "Inconsistencia general función Solicita_existencia_actvidad_inicio_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_datos_actividad_inicio_flujo(ByVal id_flujo_trabajo As Integer,
                                                   ByRef id_registro_actividad_flujo_trabajo As Integer,
                                                   ByRef id_actividad_flujo_trabajo As Integer,
                                                   ByRef id_usuario_workflow_flujo_trabajo As Integer) As String
        '----------------------------------------------------------
        'Function : Solicita el registro de actvividad de flujo de
        'trabajo a la que pertenece el usuario workflow
        'Fecha : 2017-12-12
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO," &
            "listado_actividades_workflow_Id_Actividad,ID_USUARIO_WORKFLOW FROM wf_registro_actividaes_flujos_trabajo" &
            " WHERE " &
            " wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo &
            " AND  ACTIVIDAD_INICIO=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                Solicita_datos_actividad_inicio_flujo = "Función Solicita_datos_actividad_inicio_flujo dice : " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_actividad_flujo_trabajo = 0
                id_actividad_flujo_trabajo = 0
                id_usuario_workflow_flujo_trabajo = 0
                Solicita_datos_actividad_inicio_flujo = "YES"
                Exit Function
            Else
                id_registro_actividad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(0)
                id_actividad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    id_usuario_workflow_flujo_trabajo = 0
                Else
                    id_usuario_workflow_flujo_trabajo = Datset.Tables(0).Rows(0).Item(2)
                End If

                Solicita_datos_actividad_inicio_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_actividad_inicio_flujo = "Inconsistencia general función Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow " & ex.Message
        End Try
    End Function
    Function Enviar_actividad_por_conector_flujo_de_trabajo(ByVal pag As Page,
                                                            ByVal id_tarea_seleccionada As Integer,
                                                            ByRef tre As TreeView,
                                                            ByRef estado_resultado_correo As String,
                                                            ByVal id_usuario_wf As Integer,
                                                            ByVal id_actividad_wf As Integer,
                                                            ByVal id_actividad_flujo_conector As Integer,
                                                            ByRef Resultado_evalua_terminar As String) As String
        Try
            Dim ref_TextBox_pasw_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_pasw_lista_actividades_ruta_flujo")
            Dim ref_TextBox_login_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_login_lista_actividades_ruta_flujo")
            Dim ref_UpdatePanel_autoriza_reasignacion_tarea_flujo As UpdatePanel = pag.FindControl("UpdatePanel_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_ruta_flujo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_worflow_ruta")
            Dim result As String = ""
            Dim stru_conector_flujo_ As stru_conector_flujo = Nothing
            Dim ref_Class_wf_registro_conectores As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            result = ref_Class_wf_registro_conectores.Solicita_datos_estructura_conector_flujo_trabajo(id_actividad_flujo_conector,
                                                                                                       stru_conector_flujo_)
            If result <> "YES" Then
                Enviar_actividad_por_conector_flujo_de_trabajo = result
                Exit Function
            End If
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            Dim result_ As String = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), id_tarea_seleccionada)
            If result_ <> "YES" And result_ <> "El trámite requiere de un radicado de respuesta" And result_ <> "El trámite requiere de una confirmación de respuesta" Then
                Enviar_actividad_por_conector_flujo_de_trabajo = result_
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
                Enviar_actividad_por_conector_flujo_de_trabajo = result
                Exit Function
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Enviar_actividad_por_conector_flujo_de_trabajo = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                Exit Function
            End If
            '------------------------------------------------
            'Reasigna y envia la tarea por medio de flujo
            'de trabajo
            '------------------------------------------------
            If result_ = "El trámite requiere de un radicado de respuesta" Or result_ = "El trámite requiere de una confirmación de respuesta" Then
                If stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO = 0 Then
                    Enviar_actividad_por_conector_flujo_de_trabajo = "El trámite requiere de un radicado de respuesta, no se permite reasignar a una actividad a grupos de usuarios"
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
                    Enviar_actividad_por_conector_flujo_de_trabajo = "YES"
                    Exit Function
                Else
                    '---------------------------------------------------------
                    'Reasigna y envia tarea a usuario
                    '---------------------------------------------------------
                    Dim refclas_gestino_resp As New Classgestionrespuesta
                    result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                         id_tarea_seleccionada,
                                                                                         stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO,
                                                                                         stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,
                                                                                         stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO,
                                                                                         tre,
                                                                                         "",
                                                                                         "sin autorizacion usuario permitido",
                                                                                         0,
                                                                                         pag,
                                                                                         stru_conector_flujo_.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,
                                                                                         stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,
                                                                                         stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO,
                                                                                         Resultado_evalua_terminar)
                    If result <> "YES" Then
                        Enviar_actividad_por_conector_flujo_de_trabajo = result
                        Exit Function
                    Else
                        ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                        Enviar_actividad_por_conector_flujo_de_trabajo = "YES"
                        Exit Function
                    End If

                End If
            End If

            '----------------------------------------------------------
            'Solicita estado envió notificación al correo electrónico
            '----------------------------------------------------------
            Dim ref_clas_ As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            Dim estado_envio_correo As Integer = 0
            Dim Result_corrreo As String = ""
            Dim refclas As New ClassWorkflow
            Dim ref_id_usuario_workflow_destino As String = ""
            If stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO = 0 Then
                ref_id_usuario_workflow_destino = ""
            Else
                ref_id_usuario_workflow_destino = stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO.ToString
            End If
            result = refclas.Terminar_Tarea_Workflow(ref_id_usuario_workflow_destino,
                                                     stru_conector_flujo_.ID_ACTIVIDAD_DESTINO.ToString,
                                                     id_tarea_seleccionada,
                                                     "",
                                                     pag,
                                                     Resultado_evalua_terminar,
                                                     0,
                                                     estado_resultado_correo,
                                                     stru_conector_flujo_.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,
                                                     stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,
                                                     stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO,
                                                     stru_conector_flujo_.Estado_evia_correo,
                                                     id_actividad_flujo_conector,
                                                     id_usuario_wf,
                                                     id_actividad_wf)
            If result <> "YES" Then
                Enviar_actividad_por_conector_flujo_de_trabajo = result
                Exit Function
            Else
                ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                Enviar_actividad_por_conector_flujo_de_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Enviar_actividad_por_conector_flujo_de_trabajo = "Inconsistencia general función Enviar_actividad_por_conector_flujo_de_trabajo " & ex.Message
        End Try
    End Function
    Function Enviar_actividad_por_conector_flujo_de_trabajo_anterior(ByVal pag As Page,
                                                                     ByVal id_tarea_seleccionada As Integer,
                                                                     ByRef tre As TreeView,
                                                                     ByRef estado_resultado_correo As String,
                                                                     ByVal id_usuario_wf As Integer,
                                                                     ByVal id_actividad_wf As Integer,
                                                                     ByVal id_actividad_flujo_conector As Integer,
                                                                     ByRef Resultado_evalua_terminar As String) As String
        Try
            Dim ref_TextBox_pasw_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_pasw_lista_actividades_ruta_flujo")
            Dim ref_TextBox_login_autoriza_reasignacion_tarea_flujo As TextBox = pag.FindControl("TextBox_login_lista_actividades_ruta_flujo")
            Dim ref_UpdatePanel_autoriza_reasignacion_tarea_flujo As UpdatePanel = pag.FindControl("UpdatePanel_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_ruta_flujo As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_ruta_flujo")
            Dim ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_edition_lista_actividades_worflow_ruta")
            Dim result As String = ""
            Dim stru_conector_flujo_ As stru_conector_flujo = Nothing
            Dim ref_Class_wf_registro_conectores As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            result = ref_Class_wf_registro_conectores.Solicita_datos_estructura_conector_flujo_trabajo(id_actividad_flujo_conector,
                                                                                                       stru_conector_flujo_)
            If result <> "YES" Then
                Enviar_actividad_por_conector_flujo_de_trabajo_anterior = result
                Exit Function
            End If
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            Dim result_ As String = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), id_tarea_seleccionada)
            If result_ <> "YES" And result_ <> "El trámite requiere de un radicado de respuesta" And result_ <> "El trámite requiere de una confirmación de respuesta" Then
                Enviar_actividad_por_conector_flujo_de_trabajo_anterior = result_
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
                Enviar_actividad_por_conector_flujo_de_trabajo_anterior = result
                Exit Function
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Enviar_actividad_por_conector_flujo_de_trabajo_anterior = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                Exit Function
            End If
            '------------------------------------------------
            'Reasigna y envia la tarea por medio de flujo
            'de trabajo
            '------------------------------------------------
            If result_ = "El trámite requiere de un radicado de respuesta" Or result_ = "El trámite requiere de una confirmación de respuesta" Then
                If stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO = 0 Then
                    Enviar_actividad_por_conector_flujo_de_trabajo_anterior = "El trámite requiere de un radicado de respuesta, no se permite reasignar a una actividad a grupos de usuarios"
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
                    Enviar_actividad_por_conector_flujo_de_trabajo_anterior = "YES"
                    Exit Function
                Else
                    '---------------------------------------------------------
                    'Reasigna y envia tarea a usuario
                    '---------------------------------------------------------
                    Dim refclas_gestino_resp As New Classgestionrespuesta
                    result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                         id_tarea_seleccionada,
                                                                                         stru_conector_flujo_.ID_USUARIO_WORKFLOW_FUENTE,
                                                                                         stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE,
                                                                                         stru_conector_flujo_.ID_USUARIO_WORKFLOW_FUENTE,
                                                                                         tre,
                                                                                         "",
                                                                                         "sin autorizacion usuario permitido",
                                                                                         0,
                                                                                         pag,
                                                                                         stru_conector_flujo_.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,
                                                                                         stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE,
                                                                                         stru_conector_flujo_.ID_USUARIO_WORKFLOW_FUENTE,
                                                                                         Resultado_evalua_terminar)
                    If result <> "YES" Then
                        Enviar_actividad_por_conector_flujo_de_trabajo_anterior = result
                        Exit Function
                    Else
                        ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                        Enviar_actividad_por_conector_flujo_de_trabajo_anterior = "YES"
                        Exit Function
                    End If

                End If
            End If

            '----------------------------------------------------------
            'Solicita estado envió notificación al correo electrónico
            '----------------------------------------------------------
            Dim ref_clas_ As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            Dim estado_envio_correo As Integer = 0
            Dim Result_corrreo As String = ""
            Dim refclas As New ClassWorkflow
            Dim ref_id_usuario_workflow_destino As String = ""
            If stru_conector_flujo_.ID_USUARIO_WORKFLOW_FUENTE = 0 Then
                ref_id_usuario_workflow_destino = ""
            Else
                ref_id_usuario_workflow_destino = stru_conector_flujo_.ID_USUARIO_WORKFLOW_FUENTE.ToString
            End If
            result = refclas.Terminar_Tarea_Workflow(ref_id_usuario_workflow_destino,
                                                     stru_conector_flujo_.ID_ACTIVIDAD_FUENTE.ToString,
                                                     id_tarea_seleccionada,
                                                     "",
                                                     pag,
                                                     Resultado_evalua_terminar,
                                                     0,
                                                     estado_resultado_correo,
                                                     stru_conector_flujo_.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,
                                                     stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE,
                                                     stru_conector_flujo_.ID_USUARIO_WORKFLOW_FUENTE,
                                                     stru_conector_flujo_.Estado_evia_correo,
                                                     id_actividad_flujo_conector,
                                                     id_usuario_wf,
                                                     id_actividad_wf)
            If result <> "YES" Then
                Enviar_actividad_por_conector_flujo_de_trabajo_anterior = result
                Exit Function
            Else
                ref_ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                Enviar_actividad_por_conector_flujo_de_trabajo_anterior = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Enviar_actividad_por_conector_flujo_de_trabajo_anterior = "Inconsistencia general función Enviar_actividad_por_conector_flujo_de_trabajo_anterior " & ex.Message
        End Try
    End Function
End Class