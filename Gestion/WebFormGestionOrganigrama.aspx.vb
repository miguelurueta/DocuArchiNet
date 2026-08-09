Imports System.Drawing
Imports System.Web.Script.Serialization
Imports MindFusion.Diagramming
Imports MindFusion.Diagramming.WebForms
Imports MindFusion.Diagramming.Import.VisioImporter
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports Graphics = System.Drawing.Graphics
Public Class WebFormGestionOrganigrama
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
            Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = 0
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Result = Ref_clas_rutas.lista_zon_interface(Me.DropDownZonFactor, Me.updatemenu)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 1 " & Result
            End If
            Dim ref_clas_organigrama As New ClassGaOrganigrama
            Dim organigramas() As stru_organigrama = Nothing
            Result = ref_clas_organigrama.Solicita_organigramas_workflow(Session.Item("GA_IDEMPRESA"), _
                                                                         organigramas)
            If Result = "YES" Then
                Me.DropDownList_organigramas_disponibles.Items.Add("")
                Result = ref_clas_organigrama.Lista_organigramas_interface_importacion(organigramas, _
                                                                                       Me.DropDownList_organigramas_disponibles, _
                                                                                       Me.updatemenu, _
                                                                                       1)
                If Result <> "YES" Then
                    Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 2 " & Result
                End If
            Else
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 3 " & Result
            End If
        End If
        diagramView.LicenseKey = "AQAAAEQAAAAoAAAAAQAAFx8IvJCRi56MkJmL35yQko+ekYbfjNGe0YyylpGbuYqMlpCR0buWnpiNnpKSlpGY0aianbmQjZKM//8kUb3nKfdw6tdlsocDxSo9XQSlcFbsP0LRQx1Gv9GwV+gLRASirGRQYiL2I50e"
        diagramView.Diagram.LinkCustomDraw = CustomDraw.None
        diagramView.DelKeyAction = WebForms.DelKeyAction.None
        diagramView.ModificationStart = ModificationStart.AutoHandles
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
        diagramView.LinkModifyingScript = "diagranview_bloqued(sender, args);"
    End Sub

    Private Sub DropDownList_organigramas_disponibles_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_organigramas_disponibles.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaOrganigrama
            Dim value As Integer = -1
            If Me.DropDownList_organigramas_disponibles.SelectedValue = "" Then
                value = -1
            Else
                value = Val(Me.DropDownList_organigramas_disponibles.SelectedValue)
            End If
            Result = Refclas.Abrir_organigrama(value, _
                                               Me.UpdatePanel_diagran_view, _
                                               Me.diagramView, _
                                               DropDownZonFactor.Text, _
                                               CheckBox_Grid_alineamiento, _
                                               Me.updatemenu)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub DropDownZonFactor_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownZonFactor.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Dim Result As String = ""
            Result = Ref_clas_rutas.Aplica_zon_factor_diagranview(Me.diagramView, Me.DropDownZonFactor.Text, Me.UpdatePanel_diagran_view)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub

    Private Sub ImageButtonGuardar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonGuardar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaOrganigrama
            If Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = 0 Then
                Exit Sub
            End If
            Dim string_diagrama As String = diagramView.SaveToString(SaveToStringFormat.Base64, True)
            Result = Refclas.Actualiza_diagrama_organigrama(string_diagrama, Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImageButtonActivaCrearArea_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonActivaCrearArea.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = 0 Then
                Refclasjava.Showscripman_menu("Seleccione el organigrama donde quiere crear el área o departamento", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_crear_area_departamento.Show()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_agregar_area_Click(sender As Object, e As EventArgs) Handles Button_agregar_area.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaOrganigrama
            Dim activo_pqr As Integer = 0
            Dim activo_publico As Integer = 0
            If Me.CheckBox_activa_pqrs.Checked = True Then
                activo_pqr = 1
            End If
            If Me.CheckBox_activa_publica.Checked = True Then
                activo_publico = 1
            End If
            Result = Refclas.Agregar_AreaDep_Organigrama(Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO"), _
                                                       Me.TextBox_nombre_area_dependencia.Text, _
                                                       Me.TextBox_descripcion_area.Text, _
                                                       Me.TextBox_codigo_arbitrario.Text, _
                                                       activo_pqr, _
                                                       activo_publico, _
                                                       Me.UpdatePanel_diagran_view, _
                                                       Me.diagramView)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePane_crear_area_departamento)
                Exit Sub
            Else
                Me.ModalPopupExtender_crear_area_departamento.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_crear_area_departamento)
        End Try
    End Sub

    Protected Sub ImageButtonEditarArea_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonEditarArea.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = 0 Then
                Refclasjava.Showscripman_menu("Seleccione el organigrama donde quiere ediatar el area o departamnento", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub   
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassGaOrganigrama
            Result = Refclas.Activa_editar_area_departamento( _
                                                       Me.TextBox_editar_nombre_area_dependencia, _
                                                       Me.TextBox_editar_descripcion_area, _
                                                       Me.TextBox_editar_codigo_arbitrario, _
                                                       Me.CheckBox_editar_activa_pqrs, _
                                                       Me.CheckBox_editar_activa_publica, _
                                                       Me.UpdatePanel_editar_area_departamento, _
                                                       Me.ModalPopupExtender_editar_area_departamento, _
                                                       Me.diagramView)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.updatemenu)
        End Try
    End Sub

    Private Sub Button_edita_area_Click(sender As Object, e As EventArgs) Handles Button_edita_area.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = 0 Then
                Refclasjava.Showscripman("Seleccione el organigrama donde quiere ediatar el area o departamnento", _
                                         Me.UpdatePane_editar_area_departamento)
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassGaOrganigrama
            Result = Refclas.Editar_area_departamento( _
                                                       Me.TextBox_editar_nombre_area_dependencia, _
                                                       Me.TextBox_editar_descripcion_area, _
                                                       Me.TextBox_editar_codigo_arbitrario, _
                                                       Me.CheckBox_editar_activa_pqrs, _
                                                       Me.CheckBox_editar_activa_publica, _
                                                       Me.UpdatePanel_diagran_view, _
                                                       Me.ModalPopupExtender_editar_area_departamento, _
                                                       Me.diagramView)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePane_editar_area_departamento)
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_editar_area_departamento)
        End Try
    End Sub

    
    Private Sub ImageButtonActivaEliminarElemento_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonActivaEliminarElemento.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If diagramView.Diagram.Selection.Items.Count = 0 Then
                Refclasjava.Showscripman_menu("Debe seleccionar el elemento a eliminar", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If diagramView.Diagram.Selection.Items.Count > 1 Then
                Refclasjava.Showscripman_menu("Por favor seleccione un solo elemento para eliminar", Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim obshape As Object = diagramView.Diagram.Selection.Items(0)
            Dim ob As Object = obshape.GetType
            If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                Me.Label_Confirmado.Text = "Desea eliminar el área o departamento"
                Me.HiddenField_tipo_operacion.Value = "SHAPE"
                Me.UpdatePanel_confirmar_eliminar.Update()
                Me.ModalPopupExtender_confirmar_eliminar.Show()
            Else
                If obshape.id <> "-1" Then
                    Me.Label_Confirmado.Text = "Desea eliminar la relación sub área o departamento"
                    Me.HiddenField_tipo_operacion.Value = "LINK"
                    Me.UpdatePanel_confirmar_eliminar.Update()
                    Me.ModalPopupExtender_confirmar_eliminar.Show()
                End If          
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    
    Private Sub Button_cancelar_eliminar_Click(sender As Object, e As EventArgs) Handles Button_cancelar_eliminar.Click
        Try
            Me.ModalPopupExtender_confirmar_eliminar.Hide()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_confirmar_eliminar_Click(sender As Object, e As EventArgs) Handles Button_confirmar_eliminar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If HiddenField_tipo_operacion.Value <> "ELIMINADIAGRAMA" Then
                If diagramView.Diagram.Selection.Items.Count = 0 Then
                    Refclasjava.Showscripman("Debe seleccionar el elemento a eliminar", Me.UpdatePane_confirmar_eliminar)
                    Exit Sub
                End If
                If diagramView.Diagram.Selection.Items.Count > 1 Then
                    Refclasjava.Showscripman("Por favor seleccione un solo elemento para eliminar", Me.UpdatePane_confirmar_eliminar)
                    Exit Sub
                End If
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassGaOrganigrama
            If Me.HiddenField_tipo_operacion.Value = "SHAPE" Then
                Dim obshape As Object = diagramView.Diagram.Selection.Items(0)
                Result = Refclas.Eliminar_area_departamento(obshape.id, Me.UpdatePanel_diagran_view, diagramView)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_confirmar_eliminar)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_confirmar_eliminar.Hide()
                End If
            End If
            If Me.HiddenField_tipo_operacion.Value = "LINK" Then
                Result = Refclas.Elimina_relacion_sub_area_departamento(Me.UpdatePanel_diagran_view, diagramView)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_confirmar_eliminar)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_confirmar_eliminar.Hide()
                End If
            End If
            If HiddenField_tipo_operacion.Value = "ELIMINADIAGRAMA" Then
                Result = Refclas.Eliminar_organigrama(Me.DropDownList_organigramas_disponibles.SelectedValue, _
                                                      Me.DropDownZonFactor.Text, _
                                                      Me.DropDownList_organigramas_disponibles, _
                                                      updatemenu, _
                                                      diagramView, _
                                                      UpdatePanel_diagran_view)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_confirmar_eliminar)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_confirmar_eliminar.Hide()
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_confirmar_eliminar)
        End Try
    End Sub
    Protected Sub Button_me_active_men_dive_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim ref_clas_workflow_rutas As New ClassGaOrganigrama
            Dim Result As String = ""
            Result = ref_clas_workflow_rutas.Seleccion_menu_pricipal(Me.Hidden_menu_var_event_dive.Value, _
                                                                     Me.Page)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, UpdatePanel_menu_var_event)
        End Try
    End Sub
    Private Sub Button_cancelar_inactivar_activar_Click(sender As Object, e As EventArgs) Handles Button_cancelar_inactivar_activar.Click
        Me.ModalPopupExtender_activar_inactivar.Hide()
    End Sub

    Private Sub Button_activar_inactivar_Click(sender As Object, e As EventArgs) Handles Button_activar_inactivar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim refclas As New ClassGaOrganigrama
            Dim Result As String = ""
            Result = refclas.Cambia_estado_area_departamento(Me.Check_activa_area, _
                                                             Me.CheckBox_inactiva_area, _
                                                             Me.UpdatePanel_diagran_view, _
                                                             Me.diagramView)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePane_activar_inactivar)
                Exit Sub
            Else
                Me.ModalPopupExtender_activar_inactivar.Hide()
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_activar_inactivar)
        End Try
    End Sub

    Private Sub ImageButton_conectar_sub_area_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_conectar_sub_area.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim refclas As New ClassGaOrganigrama
            Dim Result As String = ""
            Result = refclas.Agregar_sub_area_departamento( _
                                                           Me.UpdatePanel_diagran_view, Me.diagramView, 1)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.updatemenu)
                Exit Sub        
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.updatemenu)
        End Try
    End Sub

    Private Sub ImageButton_conectar_actividades_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_conectar_actividades.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim refclas As New ClassGaOrganigrama
            Dim Result As String = ""
            Result = refclas.Agregar_sub_area_departamento( _
                                                           Me.UpdatePanel_diagran_view, _
                                                           Me.diagramView, _
                                                           2)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.updatemenu)
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.updatemenu)
        End Try
    End Sub

    Private Sub Button_agregar_organigrama_Click(sender As Object, e As EventArgs) Handles Button_agregar_organigrama.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim refclas As New ClassGaOrganigrama
            Dim Result As String = ""
            Result = refclas.Agregar_Organigrama(Me.TextBox_nombre_organigrama.Text, _
                                                 Me.TextBox_codigo_resulucion.Text, _
                                                 Me.TextBox_descripcion_resolucion.Text, _
                                                 Me.TextBox_version_organigrama.Text, _
                                                 Me.TextBox_codigo_norma.Text, _
                                                 Me.TextBox_fecha_organigrama.Text, _
                                                 Me.DropDownList_organigramas_disponibles, _
                                                 Me.updatemenu)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePane_agregar_organigrama)
                Exit Sub
            Else
                Me.ModalPopupExtender_agregar_organigrama.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_agregar_organigrama)
        End Try
    End Sub

    Private Sub Button_editar_organigrama_editar_Click(sender As Object, e As EventArgs) Handles Button_editar_organigrama_editar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim refclas As New ClassGaOrganigrama
            Dim Result As String = ""
            If Me.DropDownList_organigramas_disponibles.Text = "" Then
                Refclasjava.Showscripman("Debe seleccionar el organigrama a editar", Me.UpdatePane_editar_organigrama)
                Exit Sub
            End If
            Result = refclas.Editar_organigrama(Me.DropDownList_organigramas_disponibles.SelectedValue, _
                                                Me.TextBox_nombre_organigrama_editar.Text, _
                                                 Me.TextBox_codigo_resulucion_editar.Text, _
                                                 Me.TextBox_descripcion_resolucion_editar.Text, _
                                                 Me.TextBox_version_organigrama_editar.Text, _
                                                 Me.TextBox_codigo_norma_editar.Text, _
                                                 Me.TextBox_fecha_organigrama_editar.Text, _
                                                 Me.DropDownList_organigramas_disponibles, _
                                                 Me.updatemenu)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePane_editar_organigrama)
                Exit Sub
            Else
                Me.ModalPopupExtender_editar_organigrama.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_editar_organigrama)
        End Try
    End Sub

    Private Sub Button_cancelar_cambio_estado_organigrama_Click(sender As Object, e As EventArgs) Handles Button_cancelar_cambio_estado_organigrama.Click
        Me.ModalPopupExtender_cambia_estado_organigrama.Hide()
    End Sub

    Private Sub Button_cambia_estado_organigrama_Click(sender As Object, e As EventArgs) Handles Button_cambia_estado_organigrama.Click
        Dim Refclasjava As New Classscrripjava
        Try
            'UpdatePanel_cambia_estado_organigrama
            Dim refclas As New ClassGaOrganigrama
            Dim Result As String = ""
            If Me.DropDownList_organigramas_disponibles.Text = "" Then
                Refclasjava.Showscripman("Debe seleccionar el organigrama para cambiar estado", Me.UpdatePane_cambia_estado_organigrama)
                Exit Sub
            End If
            Result = refclas.Cambia_estado_organigrama(Me.DropDownList_organigramas_disponibles.SelectedValue, _
                                                 Me.Check_activa_organigrama, _
                                                 Me.CheckBox_inactiva_organigrama)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePane_cambia_estado_organigrama)
                Exit Sub
            Else
                Me.ModalPopupExtender_cambia_estado_organigrama.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman("Inconsistencia general " & ex.Message, Me.UpdatePane_cambia_estado_organigrama)
        End Try
    End Sub
End Class