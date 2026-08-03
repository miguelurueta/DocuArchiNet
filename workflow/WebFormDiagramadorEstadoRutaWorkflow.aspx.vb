Imports MindFusion.Diagramming

Public Class WebFormDiagramadorRutaWorkflow
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim Nombre_flujo_trabajo As String = ""
        If IsPostBack = False Then
            Result = Refclas.Lista_ruta_trabajo_tarea_worflow_grafico(Session.Item("Id_Ruta_Workflow"), _
                                                                      Session.Item("RU_ID_TAREA_RUTA_TRABAJO"), _
                                                                      Session.Item("RU_RADICADO_RUTA_TRABAJO"), _
                                                                      Me.diagramView, Me.UpdatePanel_diagran_view, _
                                                                      Me.CheckBox_Grid_alineamiento, Me.Label_nombre_flujo_trabjo)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Result
            End If
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Result = Ref_clas_rutas.lista_zon_interface(Me.DropDownZonFactor, Me.updatemenu)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 3 " & Result
            End If
            Me.DropDownList_vistas_disponibles_workflow.Items.Clear()
            Me.DropDownList_vistas_disponibles_workflow.Items.Add("Vista de estados de la tarea en la ruta de trabajo")
            Me.DropDownList_vistas_disponibles_workflow.Items.Add("Vista de grupos que interactuan con la tarea")
            Me.DropDownList_vistas_disponibles_workflow.Items.Add("Vista de usuarios que interactuan con la tarea")
            'Me.DropDownList_vistas_disponibles_workflow.Text = "Estado de la ruta de trabajo"
        End If
        diagramView.LicenseKey = "AQAAAEQAAAAoAAAAAQAAFx8IvJCRi56MkJmL35yQko+ekYbfjNGe0YyylpGbuYqMlpCR0buWnpiNnpKSlpGY0aianbmQjZKM//8kUb3nKfdw6tdlsocDxSo9XQSlcFbsP0LRQx1Gv9GwV+gLRASirGRQYiL2I50e"
        diagramView.Diagram.LinkCustomDraw = CustomDraw.None
        diagramView.DelKeyAction = WebForms.DelKeyAction.None
        diagramView.ModificationStart = ModificationStart.SelectedOnly
        diagramView.Diagram.AllowSplitLinks = True
        diagramView.Diagram.LinkShape = LinkShape.Polyline
        diagramView.Diagram.AllowUnanchoredLinks = False
        diagramView.Behavior = Behavior.DrawLinks
        diagramView.Diagram.UndoManager.UndoEnabled = True
        If CheckBox_Grid_alineamiento.Checked = True Then
            diagramView.Diagram.ShowGrid = True
        Else
            diagramView.Diagram.ShowGrid = False
        End If
    End Sub
    Protected Sub DropDownZonFactor_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownZonFactor.SelectedIndexChanged
        Dim Ref_clas_rutas As New Class_worflow_rutas
        Dim Result As String = ""
        Result = Ref_clas_rutas.Aplica_zon_factor_diagranview(Me.diagramView, Me.DropDownZonFactor.Text, Me.UpdatePanel_diagran_view)
    End Sub

    Private Sub CheckBox_Grid_alineamiento_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_Grid_alineamiento.CheckedChanged
        Try
            If CheckBox_Grid_alineamiento.Checked = True Then
                diagramView.Diagram.ShowGrid = True
            Else
                diagramView.Diagram.ShowGrid = False
            End If
            UpdatePanel_diagran_view.Update()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ImageButtonGuardar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonGuardar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_worflow_rutas
            Dim Result As String = Refclas.Exporta_pdf_mindifucion(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL"),
                                                                   Me.diagramView,
                                                                   Me.ifmExcel_,
                                                                   Me.Hidden_ruta_archivo,
                                                                   Me.updatapanel_iframe)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub DropDownList_vistas_disponibles_workflow_TextChanged(sender As Object, e As EventArgs) Handles DropDownList_vistas_disponibles_workflow.TextChanged
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Nombre_flujo_trabajo As String = ""
            If Me.DropDownList_vistas_disponibles_workflow.Text = "Vista de estados de la tarea en la ruta de trabajo" Then
                Result = Refclas.Lista_ruta_trabajo_tarea_worflow_grafico(Session.Item("Id_Ruta_Workflow"), Session.Item("RU_ID_TAREA_RUTA_TRABAJO"), Session.Item("RU_RADICADO_RUTA_TRABAJO"), Me.diagramView, Me.UpdatePanel_diagran_view, Me.CheckBox_Grid_alineamiento, Me.Label_nombre_flujo_trabjo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If
            If Me.DropDownList_vistas_disponibles_workflow.Text = "Vista de grupos que interactuan con la tarea" Then
                Result = Refclas.Diagrama_trazabilidad_ruta_trabajo_por_actividad(Session.Item("RU_ID_TAREA_RUTA_TRABAJO"), Me.diagramView, Me.UpdatePanel_diagran_view, Me.CheckBox_Grid_alineamiento)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If
            If Me.DropDownList_vistas_disponibles_workflow.Text = "Vista de usuarios que interactuan con la tarea" Then
                Result = Refclas.Diagrama_trazabilidad_ruta_trabajo_por_usuario(Session.Item("RU_ID_TAREA_RUTA_TRABAJO"), Me.diagramView, Me.UpdatePanel_diagran_view, Me.CheckBox_Grid_alineamiento)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub

    Protected Sub ImageButton_detalle_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_detalle.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_worflow_rutas
            Dim Result As String = Refclas.Mostrar_detalle_elemento_diagrama(Me.diagramView, _
                                                                             Me.Page)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class