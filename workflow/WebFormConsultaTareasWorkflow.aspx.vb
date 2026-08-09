Imports System.IO

Public Class WebFormConsultaTareasWorkflow
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim refclas As New ClassWorkflow
            Dim Result As String = ""
            Result = refclas.Interface_consulta_tarea_wrokflow(Me.Page)
            If Result <> "YES" Then
                Label_resultado.Text = Result
            End If
            If Me.IsPostBack = False Then
                Me.DropDownList_limite_rows.Items.Clear()
                For i As Integer = 500 To 20000 Step 500
                    Me.DropDownList_limite_rows.Items.Add(i)
                Next
                Me.DropDownList_limite_rows.Text = 1000
            End If
        Catch ex As Exception
            Label_resultado.Text = ex.Message
        End Try
        
    End Sub
    Private Sub WebFormConsultaTareasWorkflow_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().cligred();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)

        End If
    End Sub

    Protected Sub Button_consulta_Click(sender As Object, e As EventArgs) Handles Button_consulta.Click
        Dim Ref As New Classscrripjava
        Try
            Dim Result As String = ""
            HiddenFiltro.Value = ""
            Dim Refclas_workflow As New ClassWorkflow
            Dim conector As String = "AND"
            If Me.CheckBox_and.Checked = True Then
                conector = "AND"
            Else
                conector = "OR"
            End If
            Result = Refclas_workflow.Consulta_tareas_workflow(Me.Page, _
                                                               conector, _
                                                               Val(Me.DropDownList_limite_rows.Text), _
                                                               1, _
                                                               "")
            If Result <> "YES" Then
                hdnEmailID.Value = "-1"
                Hidden_id_tarea_sel.Value = "-1"
                Ref.Showscripman(Result, Me.Updatepanel_botones_consulta)
                Exit Sub
            Else
                hdnEmailID.Value = "-1"
                Hidden_id_tarea_sel.Value = "-1"
            End If
        Catch ex As Exception
            Ref.Showscripman(ex.Message, Me.Updatepanel_botones_consulta)
        End Try
    End Sub
    Private Sub GridViewlista_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowCreated
        Try
            'e.Row.Cells(0).Visible = False
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
        Catch ex As Exception

        End Try        
    End Sub

    Private Sub Button_activa_busqueda_general_Click(sender As Object, e As EventArgs) Handles Button_activa_busqueda_general.Click
        Dim Ref As New Classscrripjava
        Try
            Dim Result As String = ""
            HiddenFiltro.Value = ""
            Dim Refclas_workflow As New ClassWorkflow
            Dim conector As String = "AND"
            If Me.CheckBox_and.Checked = True Then
                conector = "AND"
            Else
                conector = "OR"
            End If
            Exit Sub
            Result = Refclas_workflow.Consulta_tareas_workflow(Me.Page, _
                                                               conector, _
                                                               Val(Me.DropDownList_limite_rows.Text), _
                                                               2, _
                                                               TextBox_buequeda_general.Text)
            If Result <> "YES" Then
                Ref.Showscripman(Result, Me.Updatepanel_botones_consulta)
                Exit Sub
            End If
        Catch ex As Exception
            Ref.Showscripman(ex.Message, Me.Updatepanel_botones_consulta)
        End Try
    End Sub

    Private Sub Button_trazabilidad_grafica_Click(sender As Object, e As EventArgs) Handles Button_trazabilidad_grafica.Click
        Dim Refclasjava As New Classscrripjava
        Try
            
            If Me.Hidden_id_tarea_sel.Value = "-1" Or Me.Hidden_id_tarea_sel.Value = "0" Then
                Exit Sub
            End If
            Dim Refclas_w As New ClassWorkflow
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Dim Radicado As String = ""
            Dim Id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_actividad_flujo_trabajo As Integer = 0
            Dim Result As String = ""
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Result = Ref_clas_rutas.Retorna_nombre_ruta_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"))
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.Updatepanel_botones_consulta)
                Exit Sub
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Val(Me.Hidden_id_tarea_sel.Value), _
                                                                                 Radicado)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.Updatepanel_botones_consulta)
                Exit Sub
            End If
            Result = Refclas_f.Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(Radicado, _
                                                                                                                id_actividad_flujo_trabajo, _
                                                                                                               Id_flujo_trabajo, _
                                id_usuario_workflow_actividad_flujo_trabajo, _
                                HttpContext.Current.Session.Item("Id_Usuario_Workflow"), _
                                Val(Me.Hidden_id_tarea_sel.Value))
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.Updatepanel_botones_consulta)
                Exit Sub
            End If
            If Id_flujo_trabajo <> 0 Then
                Session.Item("DR_ID_FLUJO_TRABAJO") = Id_flujo_trabajo
                Session.Item("DR_ID_ACTIVIDAD_FLUJO_TRABAJO") = id_actividad_flujo_trabajo
                Session.Item("DR_ID_USUARIO_WORKFLOW_FLUJO_TRABAJO") = id_usuario_workflow_actividad_flujo_trabajo
                Session.Item("DR_ID_TAREA_FLUJO_TRABAJO") = Val(Me.Hidden_id_tarea_sel.Value)
                Session.Item("DR_RADICADO_FLUJO_TRABAJO") = Radicado
                Me.Iframelibre_.Attributes.Add("SRC", "../workflow/WebFormDiagramaEstadoFlujoTrabajo.aspx")
                Me.UpdatePanelLibre.Update()
                Me.ModalPopupExtenderLibre.Show()
            Else
                Session.Item("RU_ID_TAREA_RUTA_TRABAJO") = Val(Me.Hidden_id_tarea_sel.Value)
                Session.Item("RU_RADICADO_RUTA_TRABAJO") = Radicado
                Me.Iframelibre_.Attributes.Add("SRC", "../workflow/WebFormDiagramadorEstadoRutaWorkflow.aspx")
                Me.UpdatePanelLibre.Update()
                Me.ModalPopupExtenderLibre.Show()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.Updatepanel_botones_consulta)
        End Try
    End Sub
    Private Sub Button_Trazabilidad_Click(sender As Object, e As EventArgs) Handles Button_Trazabilidad.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_w As New ClassWorkflow
            Dim refcals As New Classgestionrespuesta
            Dim Result As String = ""
            Dim Radicado As String = ""
            If Me.Hidden_id_tarea_sel.Value = "-1" Or Me.Hidden_id_tarea_sel.Value = "0" Then
                Exit Sub
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Val(Me.Hidden_id_tarea_sel.Value), _
                                                                                    Radicado)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_botones)
                Exit Sub
            End If
            Session.Item("PU_TRAZABILIDAD") = Radicado
            Me.Iframe_trazabilidad_.Attributes("SRC") = "../workflow/WebFormTrazabilidadWorkflow.aspx"
            Me.UpdatePanel_trazabilidad.Update()
            Me.ModalPopupExtender_trazabilidad.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Updatepanel_botones)
            Exit Sub
        End Try
    End Sub
    Protected Sub Button_Exportar_Radicados_Click(sender As Object, e As EventArgs) Handles Button_Exportar_Radicados.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try

            If Me.Hidden_colum_header.Value = "" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman("No hay resultados para exportar", Me.Updatepanel_botones)
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((100000 * Rnd()) + 1))
            Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + "/reportes/" + "PUBLICO" + "/")
            If Directory.Exists(ruta_create) = False Then
                Directory.CreateDirectory(ruta_create)
            End If
            Dim ruta_archivo As String = ruta_create + HttpContext.Current.User.Identity.Name + value.ToString + "test.xls"
            Result = Refclasreposte.genera_xls(Me.GridViewlista, ".xls", _
                                               ruta_archivo, Me.Hidden_colum_header.Value, _
                                               "CONSULTA ESTADOS TAREAS", Session.Item("GA_LOGINUSUARIOGESTION"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.Updatepanel_botones)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + "/reportes/" + "PUBLICO" + "/" & HttpContext.Current.User.Identity.Name & value.ToString + "test.xls"
                    ifmExcel_.Attributes.Add("src", "../Gestion/WebFormDescargaRadicadogd.aspx")
                    updatapanel_iframe.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.Updatepanel_botones)
        End Try
    End Sub
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click    
        Dim scripjava As New Classscrripjava
        Try
            Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value
            Me.Iframe_imagen_respuesta_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_imagen_respuesta.Update()
            Me.ModalPopupExtender_imagen_respuesta.Show()
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.Updatepanel_botones)
        End Try        
    End Sub
    '#### ZONA LISTA APROBACIONES
    Private Sub Button_descarga_consolidado_aprobacion_Click(sender As Object, e As EventArgs) Handles Button_descarga_consolidado_aprobacion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Result = Class_autoriza_tarea_worklfow.Solicita_consolidado_autorizacion(Me.Hidden_id_tarea_sel.Value, _
                                                                                     Me.ifmExcel_xml_autoriza, _
                                                                                     Me.Hidden_ruta_archivo, _
                                                                                     Me.updatapanel_iframe_xml_autoriza)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.updatemenu_lista_autorizacion)
                Exit Sub
            Else
                Me.updatapanel_iframe.Update()
                Me.Updatepanel_botones.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.updatemenu_lista_autorizacion)
        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_solicitudes.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_listado_solicitudes.PageIndex = e.NewPageIndex
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Dim Result As String = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(Me.Hidden_id_tarea_sel.Value, _
                                                                                            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_LISTA_AUTORIZA"), _
                                                                                            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"), _
                                                                                            HttpContext.Current.Session.Item("SortExpression_lista_autoriza"), _
                                                                                            HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"), _
                                                                                            Me.Label_title_listado_autorizaciones, _
                                                                                            Me.data_grid_listado_solicitudes, _
                                                                                            Me.UpdatePanel_contenido_lista_autorizacion)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_contenido_lista_autorizacion)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_contenido_lista_autorizacion)
        End Try

    End Sub
    Private Sub data_grid_listado_solicitudes_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_listado_solicitudes.Sorting
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Session.Item("SortExpression_lista_autoriza") = e.SortExpression
            If Session.Item("SortDirection_produccion_lista_autoriza") = "DESC" Then
                Session.Item("SortDirection_produccion_lista_autoriza") = "ASC"
            Else
                Session.Item("SortDirection_produccion_lista_autoriza") = "DESC"
            End If
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Result = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(Me.Hidden_id_tarea_sel.Value, _
                                                                              HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_LISTA_AUTORIZA"), _
                                                                              HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"), _
                                                                              HttpContext.Current.Session.Item("SortExpression_lista_autoriza"), _
                                                                              HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"), _
                                                                              Me.Label_title_listado_autorizaciones, _
                                                                              Me.data_grid_listado_solicitudes, _
                                                                              Me.UpdatePanel_contenido_lista_autorizacion)

            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_contenido_lista_autorizacion)

            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try


    End Sub
    Private Sub data_grid_listado_solicitudes_DataBound(sender As Object, e As EventArgs) Handles data_grid_listado_solicitudes.DataBound
        Try
            Select Case sender.SortDirection
                Case SortDirection.Ascending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                Case SortDirection.Descending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black
            End Select
        Catch ex As Exception
        End Try

    End Sub

    Private Sub ImageButton_ista_autorizacio_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_ista_autorizacio.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Session.Item("SortExpression_lista_autoriza") = "Id_autorizacion"
            Session.Item("SortDirection_produccion_lista_autoriza") = "DESC"
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Result = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(Me.Hidden_id_tarea_sel.Value, _
                                                                              1, _
                                                                              HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"), _
                                                                              HttpContext.Current.Session.Item("SortExpression_lista_autoriza"), _
                                                                              HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"), _
                                                                              Me.Label_title_listado_autorizaciones, _
                                                                              Me.data_grid_listado_solicitudes, _
                                                                              Me.UpdatePanel_contenido_lista_autorizacion)

            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_tool_menu)
            Else
                Me.ModalPopupExtender_edition_lista_autorizacion.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub

    Private Sub Button_dowload_xml_Click(sender As Object, e As EventArgs) Handles Button_dowload_xml.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Dim Result As String = ""
            If Me.Hidden_selec_list.Value = -1 Then
                clasjava.Showscripman("Debe seleccionar el item ", Me.UpdatePanel_boton_lista)
                Exit Sub
            End If
            Result = Class_autoriza_tarea_worklfow.Descarga_archivo_xml(Val(Me.Hidden_selec_list.Value), _
                                                                        Me.ifmExcel_xml_autoriza, _
                                                                        Me.Hidden_ruta_archivo, _
                                                                        Me.updatapanel_iframe_xml_autoriza)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_boton_lista)
                Exit Sub
            Else
                Me.updatapanel_iframe_xml_autoriza.Update()
                Me.Updatepanel_botones.Update()
            End If

        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_lista)
        End Try
    End Sub

    '## ZONA DETALLLE RADICADO
    Private Sub Button_tool_activa_detalle_radicado_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_detalle_radicado.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_estados_modulo_rad As New Class_estados_modulo_radicacion
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Val(Me.Hidden_id_tarea_sel.Value), _
                                                                                radicado)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If radicado = "" Then
                scripjava.Showscripman_menu("Debe seleccionar el radicado", UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_class_detalle_plantilla_rad As New Class_ra_detalle_plantilla_radicado
            Result = ref_class_detalle_plantilla_rad.Genera_interface_detalle_radicado(radicado, _
                                                                                       Me.Page)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_detalle_radicado.Show()
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    'Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
    '    Try
    '        e.Row.Cells(1).Visible = False
    '    Catch ex As Exception

    '    End Try
    'End Sub
    'Private Sub data_grid_listado_solicitudes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_solicitudes.PageIndexChanging
    '    Dim clasjava As New Classscrripjava
    '    Try
    '        data_grid_listado_solicitudes.PageIndex = e.NewPageIndex
    '        Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
    '        Dim Result As String = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
    '                                                                                        HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_LISTA_AUTORIZA"), _
    '                                                                                        HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"), _
    '                                                                                        HttpContext.Current.Session.Item("SortExpression_lista_autoriza"), _
    '                                                                                        HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"), _
    '                                                                                        Me.Label_title_listado_autorizaciones, _
    '                                                                                        Me.data_grid_listado_solicitudes, _
    '                                                                                        Me.UpdatePanel_contenido_lista_autorizacion)

    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.UpdatePanel_contenido_lista_autorizacion)

    '        End If
    '    Catch ex As Exception
    '        clasjava.Showscripman(ex.Message, Me.UpdatePanel_contenido_lista_autorizacion)
    '    End Try

    'End Sub
    'Private Sub data_grid_listado_solicitudes_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_listado_solicitudes.Sorting
    '    Dim clasjava As New Classscrripjava
    '    Try

    '        Dim Result As String = ""
    '        Session.Item("SortExpression_lista_autoriza") = e.SortExpression
    '        If Session.Item("SortDirection_produccion_lista_autoriza") = "DESC" Then
    '            Session.Item("SortDirection_produccion_lista_autoriza") = "ASC"
    '        Else
    '            Session.Item("SortDirection_produccion_lista_autoriza") = "DESC"
    '        End If
    '        Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
    '        Result = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
    '                                                                          HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_LISTA_AUTORIZA"), _
    '                                                                          HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"), _
    '                                                                          HttpContext.Current.Session.Item("SortExpression_lista_autoriza"), _
    '                                                                          HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"), _
    '                                                                          Me.Label_title_listado_autorizaciones, _
    '                                                                          Me.data_grid_listado_solicitudes, _
    '                                                                          Me.UpdatePanel_contenido_lista_autorizacion)

    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.UpdatePanel_contenido_lista_autorizacion)

    '        End If

    '    Catch ex As Exception
    '        clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
    '    End Try


    'End Sub
    'Private Sub data_grid_listado_solicitudes_DataBound(sender As Object, e As EventArgs) Handles data_grid_listado_solicitudes.DataBound
    '    Try
    '        Select Case sender.SortDirection
    '            Case SortDirection.Ascending
    '                sender.HeaderRow.ForeColor = System.Drawing.Color.Black
    '                sender.FooterRow.ForeColor = System.Drawing.Color.Black

    '            Case SortDirection.Descending
    '                sender.HeaderRow.ForeColor = System.Drawing.Color.Black
    '                sender.FooterRow.ForeColor = System.Drawing.Color.Black

    '                sender.HeaderRow.ForeColor = System.Drawing.Color.Black
    '                sender.FooterRow.ForeColor = System.Drawing.Color.Black
    '        End Select
    '    Catch ex As Exception
    '    End Try

    'End Sub

    'Private Sub Button_dowload_xml_Click(sender As Object, e As EventArgs) Handles Button_dowload_xml.Click
    '    Dim clasjava As New Classscrripjava
    '    Try
    '        Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
    '        Dim Result As String = ""
    '        If Me.Hidden_selec_list.Value = -1 Then
    '            clasjava.Showscripman("Debe seleccionar el item ", Me.UpdatePanel_boton_lista)
    '            Exit Sub
    '        End If
    '        Result = Class_autoriza_tarea_worklfow.Descarga_archivo_xml(Val(Me.Hidden_selec_list.Value), _
    '                                                                    Me.ifmExcel_xml_autoriza, _
    '                                                                    Me.Hidden_ruta_archivo, _
    '                                                                    Me.updatapanel_iframe_xml_autoriza)
    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.UpdatePanel_boton_lista)
    '            Exit Sub
    '        Else
    '            Me.Updatepanel_botones.Update()
    '        End If

    '    Catch ex As Exception
    '        clasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_lista)
    '    End Try
    'End Sub

    'Private Sub ImageButton_descarga_consolidado_aprobacion_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_descarga_consolidado_aprobacion.Click
    '    Dim clasjava As New Classscrripjava
    '    Try
    '        If hdnEmailID.Value = "-1" Then
    '            Exit Sub
    '        End If
    '        Dim spli() As String = hdnEmailID.Value.Split("-")
    '        If spli.Length = 0 Then
    '            Exit Sub
    '        End If
    '        If spli(0) = "0" Then
    '            Exit Sub
    '        End If
    '        Dim Result As String = ""
    '        Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
    '        Result = Class_autoriza_tarea_worklfow.Solicita_consolidado_autorizacion(Val(spli(0)), _
    '                                                                                 Me.ifmExcel_xml_autoriza, _
    '                                                                                 Me.Hidden_ruta_archivo, _
    '                                                                                 Me.updatapanel_iframe_xml_autoriza)
    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.updatemenu_lista_autorizacion)
    '            Exit Sub
    '        Else
    '            Me.Updatepanel_botones.Update()
    '        End If
    '    Catch ex As Exception
    '        clasjava.Showscripman(ex.Message, Me.updatemenu_lista_autorizacion)
    '    End Try
    'End Sub

    'Private Sub Button_lista_autoriza_Click(sender As Object, e As EventArgs) Handles Button_lista_autoriza.Click
    '    Dim clasjava As New Classscrripjava
    '    Try
    '        If hdnEmailID.Value = "-1" Then
    '            Exit Sub
    '        End If
    '        Dim spli() As String = hdnEmailID.Value.Split("-")
    '        If spli.Length = 0 Then
    '            Exit Sub
    '        End If
    '        If spli(0) = "0" Then
    '            Exit Sub
    '        End If
    '        Dim Result As String = ""
    '        Session.Item("SortExpression_lista_autoriza") = "Id_autorizacion"
    '        Session.Item("SortDirection_produccion_lista_autoriza") = "DESC"
    '        Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
    '        Result = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(Val(spli(0)), _
    '                                                                          1, _
    '                                                                          HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"), _
    '                                                                          HttpContext.Current.Session.Item("SortExpression_lista_autoriza"), _
    '                                                                          HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"), _
    '                                                                          Me.Label_title_listado_autorizaciones, _
    '                                                                          Me.data_grid_listado_solicitudes, _
    '                                                                          Me.UpdatePanel_contenido_lista_autorizacion)

    '        If Result <> "YES" Then
    '            clasjava.Showscripman(Result, Me.Updatepanel_botones_consulta)
    '        Else
    '            Me.ModalPopupExtender_edition_lista_autorizacion.Show()
    '        End If
    '    Catch ex As Exception
    '        clasjava.Showscripman(ex.Message, Me.Updatepanel_botones_consulta)
    '    End Try
    'End Sub
End Class