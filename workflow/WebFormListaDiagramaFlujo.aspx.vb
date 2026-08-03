Imports MindFusion.Diagramming
Public Class WebFormListaDiagramaFlujo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Result As String = ""
        Dim Refclas As New Class_flujo_trabajo_workflow
        If IsPostBack = False Then
            Result = Refclas.Abre_flujo_trabajo(Session.Item("DR_ID_FLUJO_TRABAJO_SELECCION"), _
                                                Me.diagramView, _
                                                Me.UpdatePanel_diagran_view, _
                                                Me.CheckBox_Grid_alineamiento)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Result
            End If
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Result = Ref_clas_rutas.lista_zon_interface(Me.DropDownZonFactor, Me.updatemenu)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 3 " & Result
            End If
            Dim nombre_flujo As String = ""
            Result = Refclas.SolicitaNombreFlujoTrabajoPorIdFlujo(Session.Item("DR_ID_FLUJO_TRABAJO_SELECCION"), _
                                                                     nombre_flujo)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 4 " & Result
            End If
            Me.Label_nombre_flujo_trabjo.Text = "Nombre flujo " & nombre_flujo
            'Me.DropDownList_vistas_disponibles_workflow.Items.Clear()
            'Me.DropDownList_vistas_disponibles_workflow.Items.Add("Estado del flujo de trabajo")
            'Me.DropDownList_vistas_disponibles_workflow.Items.Add("Trazabilidad del flujo de trabajo")
            'Me.DropDownList_vistas_disponibles_workflow.Text = "Estado del flujo de trabajo"
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
    Protected Sub ImageButton_detalle_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_detalle.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_worflow_rutas
            Dim Result As String = Refclas.Mostrar_detalle_elemento_diagrama(Me.diagramView, Me.Page)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class