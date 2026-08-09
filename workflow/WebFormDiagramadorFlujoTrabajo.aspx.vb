Imports System.Drawing
Imports System.Web.Script.Serialization
Imports MindFusion.Diagramming
Imports MindFusion.Diagramming.WebForms
Imports MindFusion.Diagramming.Import.VisioImporter
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports Graphics = System.Drawing.Graphics
Imports System.IO

Public Class WebFormDiagramadorFlujoTrabajo
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Result As String = ""
        If IsPostBack = False Then
            Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString)
            If Directory.Exists(Ruttempo) = False Then
                Directory.CreateDirectory(Ruttempo)
            End If
            HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") = Ruttempo
            HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO") = 0
            Dim Ref_clas_flujos As New Class_flujo_trabajo_workflow
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Dim Ref_class_workflow As New ClassWorkflow
            Dim nombre_ruta As String = ""
            Result = Ref_clas_rutas.Retorna_nombre_ruta_workflow(nombre_ruta)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 1 " & Result
            End If
            Session.Item("DR_RUTASELECCION_FLUJO") = nombre_ruta
            Dim id_ruta As Integer = 0
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta,
                                                                id_ruta)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 2 " & Result
            End If
            Session.Item("DR_ID_RUTA_SELECION_FLUJO") = id_ruta
            Result = Ref_clas_rutas.lista_zon_interface(Me.DropDownZonFactor, Me.updatemenu)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 3 " & Result
            End If
            Dim nombres_rutas() As String = Nothing
            Result = Ref_clas_flujos.Solicita_nombres_flujos_trabajo_workflow(nombres_rutas)
            If Result = "YES" Then
                Me.DropDownList_flujos_disponibles_workflow.Items.Add("")
                Result = Ref_clas_flujos.Lista_nombre_flujos_trabajo_interface(nombres_rutas,
                                                                               Me.DropDownList_flujos_disponibles_workflow,
                                                                               Me.updatemenu, 1)
                If Result <> "YES" Then
                    Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 4 " & Result
                End If
            End If
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
        diagramView.Diagram.LinkEndsMovable = False
        If CheckBox_Grid_alineamiento.Checked = True Then
            diagramView.Diagram.ShowGrid = True
        Else
            diagramView.Diagram.ShowGrid = False
        End If
        diagramView.LinkModifyingScript = "diagranview_bloqued(sender, args);"
    End Sub

    Protected Sub Button_agregar_flujo_trabjo_Click(sender As Object, e As EventArgs) Handles Button_agregar_flujo_trabjo.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Result = Refclas.Agregar_flujo_trabajo_ruta_workflow(Me.DropDownList_combo_rutas.Text,
                                                                 Me.TextBox_flujo_trabajo.Text,
                                                                 Me.TextBox_descripcion_flujo_trabajo.Text,
                                                                 Me.DropDownList_tipo_flujo.Text,
                                                                 Me.diagramView,
                                                                 Me.UpdatePanel_diagran_view)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_nuevo_flujo_trabajo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.DropDownList_flujos_disponibles_workflow.Items.Add(Me.TextBox_flujo_trabajo.Text)
                Me.DropDownList_flujos_disponibles_workflow.Text = Me.TextBox_flujo_trabajo.Text
                Label_Estado_documento.Text = "Flujo " & Me.TextBox_flujo_trabajo.Text
                Me.UpdatePanel_label_estado.Update()
                Me.updatemenu.Update()
                Me.ModalPopupExtender_edition_nuevo_flujo_trabajo.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_nuevo_flujo_trabajo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub


    Protected Sub Button_me_active_men_dive_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Me.Hidden_menu_var_event_dive.Value = "" Then
                Exit Sub
            End If
            Dim ref_clas_workflow_rutas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Result = ref_clas_workflow_rutas.Seleccion_menu_pricipal_flujo_trabajo(Me.Hidden_menu_var_event_dive.Value,
                                                                                   Me.Page)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, UpdatePanel_menu_var_event)
        End Try
    End Sub
    Private Sub DropDownList_flujos_disponibles_workflow_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_flujos_disponibles_workflow.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Result = Refclas.Abre_flujo_trabajo_ruta_workflow(Me.DropDownList_flujos_disponibles_workflow.Text,
                                                              Session.Item("DR_ID_RUTA_SELECION_FLUJO"),
                                                              Me.diagramView,
                                                              Me.UpdatePanel_diagran_view,
                                                              Me.CheckBox_Grid_alineamiento)
            If Result <> "YES" Then
                Label_Estado_documento.Text = "Flujo " & Me.DropDownList_flujos_disponibles_workflow.Text
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Label_Estado_documento.Text = "Flujo " & Me.DropDownList_flujos_disponibles_workflow.Text
                Me.UpdatePanel_label_estado.Update()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImageButtonCrearGrupoActividadUsuario_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonCrearGrupoActividadUsuario.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            If Session.Item("DR_ID_RUTA_SELECION_FLUJO") = 0 Then
                Refclasjava.Showscripman_menu("Debe seleccionar la ruta del flujo documental ", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("DR_FLUJO_SELECCIONADO") = "" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el flujo documental ", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("DR_TIPO_ACTIVIDAD_AGREGAR") = ""
            Result = Refclas.Solicita_listado_actividades_flujo_trabajo(1,
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        Me.data_grid,
                                                                        Me.titulo_label_grid,
                                                                        Me.hdnEmailID,
                                                                        Me.UpdateGeneral_documentos)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_lista_actividades_worflow.Show()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub data_grid_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception
        End Try
    End Sub
    Private Sub Button_buscar_lista_Click(sender As Object, e As EventArgs) Handles Button_buscar_lista.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            If Session.Item("DR_TIPO_ACTIVIDAD_AGREGAR") = "USUARIOINDIVIDUAL" Then
                Result = Refclas.Solicita_listado_actividades_flujo_trabajo_usuario(Session.Item("DR_ID_RUTA_SELECION_FLUJO"),
                                                                               2,
                                                                               Me.TextBox_busqueda.Text,
                                                                               "",
                                                                               "",
                                                                               Me.data_grid,
                                                                               Me.titulo_label_grid,
                                                                               Me.hdnEmailID,
                                                                               Me.UpdateGeneral_documentos)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = Refclas.Solicita_listado_actividades_flujo_trabajo(2,
                                                                            Me.TextBox_busqueda.Text,
                                                                            "",
                                                                            "",
                                                                            Me.data_grid,
                                                                            Me.titulo_label_grid,
                                                                            Me.hdnEmailID,
                                                                            Me.UpdateGeneral_documentos)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If


        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_restore_lista_actividad_Click(sender As Object, e As EventArgs) Handles Button_restore_lista_actividad.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            If Session.Item("DR_TIPO_ACTIVIDAD_AGREGAR") = "USUARIOINDIVIDUAL" Then
                Result = Refclas.Solicita_listado_actividades_flujo_trabajo_usuario(Session.Item("DR_ID_RUTA_SELECION_FLUJO"),
                                                                               1,
                                                                               Me.TextBox_busqueda.Text,
                                                                               "",
                                                                               "",
                                                                               Me.data_grid,
                                                                               Me.titulo_label_grid,
                                                                               Me.hdnEmailID,
                                                                               Me.UpdateGeneral_documentos)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = Refclas.Solicita_listado_actividades_flujo_trabajo(1,
                                                                            Me.TextBox_busqueda.Text,
                                                                            "",
                                                                            "",
                                                                            Me.data_grid,
                                                                            Me.titulo_label_grid,
                                                                            Me.hdnEmailID,
                                                                            Me.UpdateGeneral_documentos)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If

        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_agrega_actividad_flujo_trabajo_Click(sender As Object, e As EventArgs) Handles Button_agrega_actividad_flujo_trabajo.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            If Me.hdnEmailID.Value = "0" Then
                Refclasjava.Showscripman_menu("Por favor seleccione el registro de la actividad que quiere asignar", Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("DR_TIPO_ACTIVIDAD_AGREGAR") = "USUARIOINDIVIDUAL" Then
                Result = Refclas.Agregar_actividad_usuario_flujo_de_trabajo(Me.diagramView,
                                                                            Me.UpdatePanel_diagran_view,
                                                                            Me.hdnEmailID.Value,
                                                                            Val(Session.Item("DR_ID_FLUJO_SELECCIONADO")))
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_lista_actividades_worflow.Hide()
                End If
            Else
                Result = Refclas.Agregar_actividad_flujo_de_trabajo(Me.diagramView,
                                                                    Me.UpdatePanel_diagran_view,
                                                                    Me.hdnEmailID.Value,
                                                                    Val(Session.Item("DR_ID_FLUJO_SELECCIONADO")))
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_lista_actividades_worflow.Hide()
                End If
            End If

        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImageButtonGuardar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonGuardar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            If Session.Item("DR_ID_RUTA_SELECION_FLUJO") = 0 Then
                Refclasjava.Showscripman_menu("Debe seleccionar la ruta del flujo documental ", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("DR_FLUJO_SELECCIONADO") = "" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el flujo documental ", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Guardar_flujo_trabajo_workflow(Me.diagramView, Val(Session.Item("DR_ID_FLUJO_SELECCIONADO")))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImageButtonEliminarActividades_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonEliminarActividades.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            If Session.Item("DR_FLUJO_SELECCIONADO") = "" Then Exit Sub
            ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama.Show()

        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_aceptar_confirmacion_eliminar_elmento_diagrama_Click(sender As Object, e As EventArgs) Handles Button_aceptar_confirmacion_eliminar_elmento_diagrama.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            Result = Refclas.Eliminar_elemento_diagrama_web(Me.diagramView, Me.UpdatePanel_diagran_view, Session.Item("DR_RUTASELECCION_DIAGRAMA"), Val(Session.Item("DR_ID_FLUJO_SELECCIONADO")))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_confirma_eliminar_elmento_diagrama, "ModalPopupExtender_mensaje_personalizado")
                Me.ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama.Hide()
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_confirma_eliminar_elmento_diagrama, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub

    Private Sub Button_cancelar_confirmacion_eliminar_elmento_diagrama_Click(sender As Object, e As EventArgs) Handles Button_cancelar_confirmacion_eliminar_elmento_diagrama.Click
        ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama.Hide()
    End Sub

    Private Sub ImageButton_conectar_actividades_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_conectar_actividades.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            If Session.Item("DR_FLUJO_SELECCIONADO") = "" Then Exit Sub
            Result = Refclas.Crear_conexion_actividades_flujo_trabajo_workflow(Val(Session.Item("DR_ID_FLUJO_SELECCIONADO")),
                                                                               Me.diagramView,
                                                                               Me.UpdatePanel_diagran_view)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try


    End Sub

    Private Sub ImageButton_Crear_Actividad_usuario_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_Crear_Actividad_usuario.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            If Session.Item("DR_ID_RUTA_SELECION_FLUJO") = 0 Then
                Refclasjava.Showscripman_menu("Debe seleccionar la ruta del flujo documental ", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("DR_FLUJO_SELECCIONADO") = "" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el flujo documental ", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("DR_TIPO_ACTIVIDAD_AGREGAR") = "USUARIOINDIVIDUAL"
            Result = Refclas.Solicita_listado_actividades_flujo_trabajo_usuario(Session.Item("DR_ID_RUTA_SELECION_FLUJO"),
                                                                                1,
                                                                                "",
                                                                                "",
                                                                                "",
                                                                                Me.data_grid,
                                                                                Me.titulo_label_grid,
                                                                                Me.hdnEmailID,
                                                                                Me.UpdateGeneral_documentos)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_lista_actividades_worflow.Show()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
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


    Protected Sub Button_Cancela_estado_cerrado_abierto_actividad_Click(sender As Object, e As EventArgs) Handles Button_Cancela_estado_cerrado_abierto_actividad.Click
        ModalPopupExtender_edition_configura_tipo_actividad_flujo.Hide()
    End Sub

    Protected Sub Button_cambia_estado_cerrado_abierto_actividad_Click(sender As Object, e As EventArgs) Handles Button_cambia_estado_cerrado_abierto_actividad.Click
        Dim Refclasjava As New Classscrripjava
        Dim refclas As New Class_flujo_trabajo_workflow
        Try
            Dim Result As String = ""
            Dim id_actividad As Integer = Me.diagramView.Diagram.Selection.Items(0).Id
            Dim estado_actividad_cerrado_abierto As Integer = 0
            If Me.CheckBox_flujo_cerrado_actividad.Checked = True Then
                estado_actividad_cerrado_abierto = 1
            Else
                estado_actividad_cerrado_abierto = 0
            End If
            Result = refclas.Actualiza_estado_abierto_cerrado_actividad_flujo_trabajo(id_actividad,
                                                                                      estado_actividad_cerrado_abierto)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_cambia_estado_actividad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_configura_tipo_actividad_flujo.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_cambia_estado_actividad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_cambia_estado_cerrado_abierto_flujo_Click(sender As Object, e As EventArgs) Handles Button_cambia_estado_cerrado_abierto_flujo.Click
        Dim Refclasjava As New Classscrripjava
        Dim refclas As New Class_flujo_trabajo_workflow
        Try
            Dim Result As String = ""
            Dim estado_flujo_cerrado_abierto As Integer = 0
            If Me.CheckBox_flujo_cerrado.Checked = True Then
                estado_flujo_cerrado_abierto = 1
            Else
                estado_flujo_cerrado_abierto = 0
            End If
            Result = refclas.Actualiza_estado_abierto_cerrado_flujo_trabajo(HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO"),
                                                                            estado_flujo_cerrado_abierto)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_estado_cerrado_abierto, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_configura_tipo_flujo_trabajo.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_estado_cerrado_abierto, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_Cancela_estado_cerrado_abierto_flujo_Click(sender As Object, e As EventArgs) Handles Button_Cancela_estado_cerrado_abierto_flujo.Click
        ModalPopupExtender_edition_configura_tipo_flujo_trabajo.Hide()
    End Sub

    Protected Sub Button_cancelar_confirmacion_eliminar_flujo_trabajo_Click(sender As Object, e As EventArgs) Handles Button_cancelar_confirmacion_eliminar_flujo_trabajo.Click
        ModalPopupExtender_edition_confirma_eliminar_flujo_trabajo.Hide()
    End Sub

    Protected Sub Button_aceptar_confirmacion_eliminar_flujo_trabajo_Click(sender As Object, e As EventArgs) Handles Button_aceptar_confirmacion_eliminar_flujo_trabajo.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_flujo_trabajo_workflow
            Result = Refclas.Elimina_flujo_de_trabajo(Me.diagramView,
                                                      Me.UpdatePanel_diagran_view,
                                                      Me.DropDownList_flujos_disponibles_workflow.Text,
                                                      Me.DropDownZonFactor.Text,
                                                      Me.DropDownList_flujos_disponibles_workflow,
                                                      Me.updatemenu)
            If Result <> "YES" Then
                ModalPopupExtender_edition_confirma_eliminar_flujo_trabajo.Hide()
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_confirma_eliminar_flujo_trabajo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Label_Estado_documento.Text = "Estado "
                Me.UpdatePanel_label_estado.Update()
                ModalPopupExtender_edition_confirma_eliminar_flujo_trabajo.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_confirma_eliminar_flujo_trabajo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImageButton_eliminar_flujo_trabajo_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_eliminar_flujo_trabajo.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Me.DropDownList_flujos_disponibles_workflow.Text = "" Then
                Refclasjava.Showscripman_menu("Por favor seleccione un flujo de trabajo para eliminar", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                ModalPopupExtender_edition_confirma_eliminar_flujo.Show()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_agregar_flujo_trabajo_copia_Click(sender As Object, e As EventArgs) Handles Button_agregar_flujo_trabajo_copia.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Result = Refclas.Duplicar_flujo_de_trabajo(Me.TextBox_flujo_trabajo_copia.Text,
                                                       Me.DropDownList_flujos_disponibles_workflow.Text,
                                                       Me.TextBox_descripcion_flujo_trabajo_copia.Text,
                                                       Me.DropDownList_tipo_flujo_copia.Text,
                                                       Me.DropDownList_flujos_disponibles_workflow,
                                                       Me.updatemenu)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botton_duplicar, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                ModalPopupExtender_edition_copia_flujo_trabajo.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botton_duplicar, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_cancelar_confirmacion_Click(sender As Object, e As EventArgs) Handles Button_cancelar_confirmacion.Click
        Me.ModalPopupExtender_edition_confirma_eliminar_flujo.Hide()
    End Sub

    Protected Sub Button_aceptar_confirmacion_Click(sender As Object, e As EventArgs) Handles Button_aceptar_confirmacion.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_flujo_trabajo_workflow
            Result = Refclas.Eliminar_flujo_trabajo(Me.DropDownList_flujos_disponibles_workflow.Text,
                                                    HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO"),
                                                    Me.DropDownList_flujos_disponibles_workflow,
                                                    Me.updatemenu, Me.diagramView, Me.UpdatePanel_diagran_view,
                                                    Me.DropDownZonFactor.Text)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_confirma_eliminar_flujo, "ModalPopupExtender_mensaje_personalizado")
                Me.ModalPopupExtender_edition_confirma_eliminar_flujo.Hide()
                Exit Sub
            Else
                Me.Label_Estado_documento.Text = "Estado "
                Me.UpdatePanel_label_estado.Update()
                Me.ModalPopupExtender_edition_confirma_eliminar_flujo.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_confirma_eliminar_flujo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_Cancela_estado_actividad_inicio_Click(sender As Object, e As EventArgs) Handles Button_Cancela_estado_actividad_inicio.Click
        Me.ModalPopupExtender_edition_activa_actividad_inicio.Hide()
    End Sub

    Protected Sub Button_cambia_estado_actividad_inicio_Click(sender As Object, e As EventArgs) Handles Button_cambia_estado_actividad_inicio.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_flujo_trabajo_workflow
            Result = Refclas.Activa_desactiva_actividad_inicio(Me.ModalPopupExtender_edition_activa_actividad_inicio,
                                                               Me.Check_actividad_inicio,
                                                               Me.UpdatePanel_diagran_view,
                                                               Me.diagramView,
                                                               HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_canbia_estado_actividad_ini, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_activa_actividad_inicio.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_canbia_estado_actividad_ini, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_editar_flujo_trabjo_Click(sender As Object, e As EventArgs) Handles Button_editar_flujo_trabjo.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_flujo_trabajo_workflow
            Result = Refclas.Actualiza_datos_caracterizacion_flujo_trabajo(DropDownList_flujos_disponibles_workflow,
                                                                          Me.updatemenu,
                                                                          HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO"),
                                                                          Session.Item("DR_ID_RUTA_SELECION_FLUJO"),
                                                                          TextBox_Edita_nombre_flujo_trabajo,
                                                                          TextBox_Edita_descripcion_flujo_trabajo)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_edita_flujo, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_edita_flujo_trabajo.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_edita_flujo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub diagramView_LinkModified(sender As Object, e As LinkEventArgs) Handles diagramView.LinkModified
        sender.cancel = True
    End Sub

    Private Sub Button_regi_resp_Click(sender As Object, e As EventArgs) Handles Button_regi_resp.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_agrega_usuario_responsable_flujo
            Result = Refclas.Agrega_usuario_responsable_flujo(HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO"),
                                                            Me.DropDownList_grupo_respon_flujo,
                                                            Me.UpdatePanel_registra_respon_flujo,
                                                            Me.DropDownList_user_resp)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_usuario_respon_flujo)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_registra_respon_flujo.Show()
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_usuario_respon_flujo)
        End Try
    End Sub

    Protected Sub DropDownList_grupo_respon_flujo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_grupo_respon_flujo.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclass As New Class_remit_dest_interno
            Result = Refclass.Solicita_usuario_areas_departamento(DropDownList_grupo_respon_flujo.SelectedValue,
                                                                  Me.DropDownList_user_resp,
                                                                  Me.UpdatePanel_registra_respon_flujo)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_registra_respon_flujo)
                Exit Sub

            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_registra_respon_flujo)
        End Try
    End Sub

    Protected Sub Button_activa_registra_Click(sender As Object, e As EventArgs) Handles Button_activa_registra.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref As New Class_ra_usuario_gestion_responsable_flujo
            Result = Ref.Registra_usuario_responsable_flujo(Me.DropDownList_user_resp.SelectedValue, _
                                                          Me.DropDownList_grupo_respon_flujo.SelectedValue, _
                                                          HttpContext.Current.Session.Item("DR_ID_FLUJO_SELECCIONADO"), _
                                                          Me.DropDownList_user_resp.SelectedItem.Text, _
                                                          Me.DropDownList_user_respon_flujo, _
                                                          Me.UpdatePanel_usuario_respon_flujo)

            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_activa_registra)
                Exit Sub
            Else
                ModalPopupExtender_edition_registra_respon_flujo.Hide()
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_activa_registra)
        End Try
    End Sub

    Private Sub Button_elimi_resp_Click(sender As Object, e As EventArgs) Handles Button_elimi_resp.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Me.Hidden_res.Value = 0 Then Exit Sub
            If DropDownList_user_respon_flujo.Items.Count = 0 Then Exit Sub
            Dim Result As String = ""
            Dim Refclas As New Class_ra_usuario_gestion_responsable_flujo
            Result = Refclas.Eliminar_usuario_responsable_flujo(Val(DropDownList_user_respon_flujo.SelectedValue), _
                                                                DropDownList_user_respon_flujo, _
                                                                UpdatePanel_usuario_respon_flujo)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_usuario_respon_flujo)
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_usuario_respon_flujo)
        End Try
    End Sub

    Protected Sub Button_config_correo_conector_Click(sender As Object, e As EventArgs) Handles Button_config_correo_conector.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim estado_envio_correo As Integer = 0
            Dim Refclas As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            Dim stru_config_conector_flujo As stru_config_conector_flujo = Nothing
            If Me.CheckBox_estado_correo_conector.Checked = True Then
                stru_config_conector_flujo.Estado_evia_correo = 1
            Else
                stru_config_conector_flujo.Estado_evia_correo = 0
            End If
            If Me.CheckBox_autoriza_tarea.Checked = True Then
                stru_config_conector_flujo.Estado_soicita_autorizacion = 1
            Else
                stru_config_conector_flujo.Estado_soicita_autorizacion = 0
            End If
            If Me.CheckBox_autoriza_tarea_firma_digital.Checked = True Then
                stru_config_conector_flujo.Estado_soicita_autorizacion_firma_digital = 1
            Else
                stru_config_conector_flujo.Estado_soicita_autorizacion_firma_digital = 0
            End If
            If Me.CheckBox_estado_copia_estructura.Checked = True Then
                stru_config_conector_flujo.Estado_copia_documento_estructura = 1
            Else
                stru_config_conector_flujo.Estado_copia_documento_estructura = 0
            End If
            If Me.CheckBox_Estado_asigna_expediente.Checked = True Then
                stru_config_conector_flujo.Estado_asigna_expediente = 1
            Else
                stru_config_conector_flujo.Estado_asigna_expediente = 0
            End If
            If Me.CheckBox_estado_firma_digital.Checked = True Then
                stru_config_conector_flujo.Estado_firma_digital = 1
            Else
                stru_config_conector_flujo.Estado_firma_digital = 0
            End If
            If Me.CheckBox_estado_valida_balanceo.Checked = True Then
                stru_config_conector_flujo.estado_valida_balanceo = 1
            Else
                stru_config_conector_flujo.estado_valida_balanceo = 0
            End If
            If Me.CheckBox_estado_copia_estructura_total.Checked = True Then
                stru_config_conector_flujo.Estado_copia_estructura_total = 1
            Else
                stru_config_conector_flujo.Estado_copia_estructura_total = 0
            End If
            Result = Refclas.Actualiza_configuracion_conector(HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR"), _
                                                             stru_config_conector_flujo)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_buton_configura_envi_correo_conector)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_configura_envi_correo_conector.Hide()
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_buton_configura_envi_correo_conector)
        End Try
    End Sub
End Class