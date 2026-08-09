Public Class WebFormGaGestionInstrumentos
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Result As String = ""
        If Page.IsPostBack = False Then

            Dim ref_clas_organigrama As New ClassGaOrganigrama
            Dim organigramas() As stru_organigrama = Nothing
            Result = ref_clas_organigrama.Solicita_organigramas_workflow(Session.Item("GA_IDEMPRESA"), organigramas)
            If Result = "YES" Then
                Me.DropDownList_organigrama.Items.Add("")
                Result = ref_clas_organigrama.Lista_organigramas_interface_importacion(organigramas, _
                                                                                       Me.DropDownList_organigrama, _
                                                                                       Me.UpdatePanel_instrumentos, _
                                                                                       1)
            End If
       
        End If
    End Sub
      
    Protected Sub Button_me_active_men_dive_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim ref_clas_workflow_rutas As New ClassGaGestionInstrumento
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

    Protected Sub Button_agregar_instrumento_Click(sender As Object, e As EventArgs) Handles Button_agregar_instrumento.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim ref_clas_gestion_instrumento As New ClassGaGestionInstrumento
            Dim Result As String = ""
            Dim id_organigrama As Integer = 0
            Dim id_tipo_instrumento As Integer = 0
            If Me.DropDownList_organigrama.SelectedValue Is Nothing Then
                Refclasjava.Showscripman("Debe seleccionar el organigrama al que pertenecerá el instrumento archivístico", Me.UpdatePanel_agregar_instrumento)
                Exit Sub
            End If
            If Me.DropDownList_tipo_instrumento.SelectedValue Is Nothing Then
                Refclasjava.Showscripman("Debe seleccionar el tipo de instrumento archivístico", Me.UpdatePanel_agregar_instrumento)
                Exit Sub
            End If
            id_organigrama = Val(Me.DropDownList_organigrama.SelectedValue)
            id_tipo_instrumento = Val(Me.DropDownList_tipo_instrumento.SelectedValue)
            Result = ref_clas_gestion_instrumento.Agregar_instrumento_archivistico(id_organigrama, _
                                                                                   Me.TextBox_nombre_instrumento.Text, _
                                                                                   id_tipo_instrumento, _
                                                                                   Me.TextBox_fecha_instrumento.Text, _
                                                                                   Me.TextBox_descripcion_instrumento.Text, _
                                                                                   Me.TextBox_version_instrumento.Text, _
                                                                                   Me.TextBox_Justificacion_instrumento.Text, _
                                                                                   Me.DropDownList_instrumento, _
                                                                                   Me.UpdatePanel_instrumentos)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_agregar_instrumento)
                Exit Sub
            Else
                Me.DropDownList_areas_departamento.Items.Clear()
                Me.TreeViewInstrumento.Nodes.Clear()
                Result = ref_clas_gestion_instrumento.Lista_areas_organigrama_instrumento(Me.DropDownList_organigrama.SelectedValue, _
                                                                                          Me.DropDownList_areas_departamento, _
                                                                                          Me.UpdatePanel_instrumentos)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_agregar_instrumento)
                Else
                    Me.UpdatePanel_treview_instrumento.Update()
                    Me.UpdatePanel_instrumentos.Update()
                End If
                Me.ModalPopupExtender_agregar_instrumento.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_agregar_instrumento)
        End Try
    End Sub

    Protected Sub Button_editar_instrumento_Click(sender As Object, e As EventArgs) Handles Button_editar_instrumento.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim ref_clas_gestion_instrumento As New ClassGaGestionInstrumento
            Dim Result As String = ""
            Dim id_organigrama As Integer = 0
            Dim id_tipo_instrumento As Integer = 0
            If Me.DropDownList_instrumento.SelectedValue Is Nothing Then
                Refclasjava.Showscripman("Debe seleccionar el tipo de instrumento archivístico a editar", Me.UpdatePane_editar_instrumento)
                Exit Sub
            End If
            If Me.DropDownList_tipo_instrumento_editar.SelectedValue Is Nothing Then
                Refclasjava.Showscripman("Debe seleccionar el tipo de instrumento archivístico", Me.UpdatePane_editar_instrumento)
                Exit Sub
            End If
            id_tipo_instrumento = Val(Me.DropDownList_tipo_instrumento_editar.SelectedValue)
            Dim id_instrumento As Integer = Val(Me.DropDownList_instrumento.SelectedValue)
            Result = ref_clas_gestion_instrumento.Edita_instrumento_archivistico(id_instrumento, TextBox_nombre_instrumento_editar.Text, _
                                                                                    id_tipo_instrumento, Me.TextBox_fecha_instrumento_editar.Text, _
                                                                                    Me.TextBox_descripcion_instrumento_editar.Text, _
                                                                                    Me.TextBox_version_instrumento_editar.Text, _
                                                                                    Me.TextBox_Justificacion_instrumento_editar.Text, _
                                                                                    Me.DropDownList_instrumento, _
                                                                                    Me.UpdatePanel_instrumentos)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePane_editar_instrumento)
                Exit Sub
            Else
                Me.ModalPopupExtender_editar_instrumento.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_editar_instrumento)
        End Try
    End Sub

   

    Protected Sub Button_activar_inactivar_Click(sender As Object, e As EventArgs) Handles Button_activar_inactivar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaGestionInstrumento
            If Me.DropDownList_instrumento.SelectedValue Is Nothing Then
                Refclasjava.Showscripman("Debe seleccionar el tipo de instrumento archivístico a cambiar estado", Me.UpdatePane_activar_inactivar)
                Exit Sub
            End If

            Result = Refclas.cambiar_estado_instrumento_archivistico(Me.DropDownList_organigrama.SelectedValue, _
                                                                     Val(Me.DropDownList_instrumento.SelectedValue), _
                                                                     Me.Check_activa_instrumento, _
                                                                     Me.CheckBox_inactiva_instrumento, _
                                                                     Me.DropDownList_instrumento.SelectedItem.Text, _
                                                                     Me.DropDownList_instrumento, _
                                                                     Me.UpdatePanel_instrumentos)
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

    Private Sub DropDownList_organigrama_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_organigrama.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Me.DropDownList_instrumento.Items.Clear()
            Me.DropDownList_areas_departamento.Items.Clear()
            Me.TreeViewInstrumento.Nodes.Clear()
            If Me.DropDownList_organigrama.SelectedValue Is Nothing Then
                Exit Sub
            End If
            If Me.DropDownList_organigrama.SelectedItem.Text = "" Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassGaGestionInstrumento
            Result = Refclas.Lista_instrumentos_archivisticos(Val(Me.DropDownList_organigrama.SelectedValue), _
                                                              Me.DropDownList_instrumento, Me.UpdatePanel_instrumentos)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_instrumentos)
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_instrumentos)
        Finally
            Me.UpdatePanel_treview_instrumento.Update()
            Me.UpdatePanel_instrumentos.Update()
        End Try
    End Sub

  

    Protected Sub Button_confirmar_eliminar_Click(sender As Object, e As EventArgs) Handles Button_confirmar_eliminar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaGestionInstrumento
            Dim Refclas_trd As New ClassTrdDocumental
            If HiddenField_estado_operacion.Value = "IAC-ELIM-INSTRUMENTO" Then
                Result = Refclas.Eliminar_instrumento_archivistico(Me.DropDownList_instrumento.SelectedValue, _
                                                                   Me.DropDownList_instrumento, _
                                                                   Me.UpdatePanel_instrumentos, _
                                                                   Me.DropDownList_areas_departamento, _
                                                                   Me.TreeViewInstrumento, _
                                                                   Me.UpdatePanel_instrumentos)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_confirmar_eliminar.Hide()
                End If
            End If
            If HiddenField_estado_operacion.Value = "IAC-ELIM-INSTRUMENTO1" Then
                If DropDownList_instrumento.Text = "" Then
                    Refclasjava.Showscripman("Debe seleccionar el instrumento para eliminar la serie documental", Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                End If
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Refclasjava.Showscripman("Debe seleccionar el nodo para eliminar la serie documental", Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                End If

                Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_trd.Eliminar_serie_documental(spli_t(1), Me.DropDownList_instrumento.SelectedValue, _
                                                                   Me.TreeViewInstrumento, _
                                                                   Me.UpdatePanel_treview_instrumento)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_confirmar_eliminar.Hide()
                End If
            End If

            If HiddenField_estado_operacion.Value = "IAC-ELIM-INSTRUMENTO2" Then
                If DropDownList_instrumento.Text = "" Then
                    Refclasjava.Showscripman("Debe seleccionar el instrumento para eliminar la serie documental", Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                End If
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Refclasjava.Showscripman("Debe seleccionar el nodo para eliminar la sub serie documental", Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                End If

                Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_trd.Eliminar_sub_serie_documental(spli_t(1), Me.DropDownList_instrumento.SelectedValue, _
                                                                   Me.TreeViewInstrumento, _
                                                                   Me.UpdatePanel_treview_instrumento)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_confirmar_eliminar.Hide()
                End If
            End If
            If HiddenField_estado_operacion.Value = "IAC-ELIM-INSTRUMENTO3" Then
                If DropDownList_instrumento.Text = "" Then
                    Refclasjava.Showscripman("Debe seleccionar el instrumento para eliminar la serie documental", Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                End If
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Refclasjava.Showscripman("Debe seleccionar el nodo para eliminar la sub serie documental", Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                End If

                Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_trd.Eliminar_tipo_documental_serie_sub_serie(spli_t(1), Me.DropDownList_instrumento.SelectedValue, _
                                                                   Me.TreeViewInstrumento, _
                                                                   Me.UpdatePanel_treview_instrumento)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_confirmar_eliminar)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_confirmar_eliminar.Hide()
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_confirmar_eliminar)
        End Try
    End Sub

    Private Sub DropDownList_instrumento_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_instrumento.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaGestionInstrumento
            Me.DropDownList_areas_departamento.Items.Clear()
            Me.TreeViewInstrumento.Nodes.Clear()
            Result = Refclas.Lista_areas_organigrama_instrumento(Me.DropDownList_organigrama.SelectedValue, _
                                                               Me.DropDownList_areas_departamento, _
                                                               Me.UpdatePanel_instrumentos)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_instrumentos)
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_instrumentos)
        Finally
            Me.UpdatePanel_treview_instrumento.Update()
            Me.UpdatePanel_instrumentos.Update()
        End Try
    End Sub

    Protected Sub DropDownList_areas_departamento_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_areas_departamento.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassTrdDocumental
            Me.TreeViewInstrumento.Nodes.Clear()
            Dim stru_serie() As Serie_documental = Nothing
            Result = Refclas.Lista_instrumentos_por_area(Me.DropDownList_areas_departamento.SelectedValue, _
                                                         Me.DropDownList_instrumento.SelectedValue, _
                                                         stru_serie)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_instrumentos)
                Exit Sub
            End If
            Result = Refclas.Lista_instrumento_interface_por_area(stru_serie, _
                                                                  Me.DropDownList_areas_departamento.SelectedItem.Text, _
                                                                  Me.TreeViewInstrumento, _
                                                                  Me.UpdatePanel_treview_instrumento, _
                                                                  1)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_instrumentos)
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_instrumentos)
        Finally
            Me.UpdatePanel_treview_instrumento.Update()
        End Try
    End Sub

    Private Sub CheckBoxConservTotal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBoxConservTotal.CheckedChanged
        Try
            If Me.CheckBoxConservTotal.Checked = True Then
                Me.CheckBoxSerieEliminacion.Checked = False
                Me.CheckBoxSerieSeleccion.Checked = False
            End If
        Catch ex As Exception
        Finally
            UpdatePanel_agregar_serie.Update()
        End Try

    End Sub

    Private Sub CheckBoxSerieEliminacion_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBoxSerieEliminacion.CheckedChanged
        Try
            If Me.CheckBoxSerieEliminacion.Checked = True Then
                Me.CheckBoxConservTotal.Checked = False
                Me.CheckBoxSerieSeleccion.Checked = False
            End If
        Catch ex As Exception
        Finally
            UpdatePanel_agregar_serie.Update()
        End Try

    End Sub

    Private Sub CheckBoxSerieSeleccion_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBoxSerieSeleccion.CheckedChanged
        Try
            If Me.CheckBoxSerieSeleccion.Checked = True Then
                'Me.CheckBoxSerieEliminacion.Checked = False
                Me.CheckBoxConservTotal.Checked = False
            End If
        Catch ex As Exception
        Finally
            UpdatePanel_agregar_serie.Update()
        End Try

    End Sub
    Private Sub CheckBoxDiposicion_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBoxDiposicion.CheckedChanged
        Try
            If Me.CheckBoxDiposicion.Checked = True Then
                Me.CheckBoxSerieSeleccion.Enabled = True
                Me.CheckBoxSerieEliminacion.Enabled = True
                Me.CheckBoxConservTotal.Enabled = True
                Me.CheckBoxSerieDigitalizacion.Enabled = True
                Me.DropDownList_tiempo_retencion_gestion.Enabled = True
                Me.DropDownList_tiempo_retencion_central.Enabled = True
                Me.DropDownListMedio.Enabled = True
            Else
                Me.CheckBoxSerieSeleccion.Enabled = False
                Me.CheckBoxSerieEliminacion.Enabled = False
                Me.CheckBoxConservTotal.Enabled = False
                Me.CheckBoxSerieDigitalizacion.Enabled = False
                Me.DropDownList_tiempo_retencion_gestion.Enabled = False
                Me.DropDownList_tiempo_retencion_central.Enabled = False
                Me.DropDownListMedio.Enabled = False
                Me.CheckBoxSerieSeleccion.Checked = False
                Me.CheckBoxSerieEliminacion.Checked = False
                Me.CheckBoxConservTotal.Checked = False
                Me.CheckBoxSerieDigitalizacion.Checked = False
                Me.DropDownList_tiempo_retencion_gestion.SelectedValue = 0
                Me.DropDownList_tiempo_retencion_central.SelectedValue = 0
                Me.DropDownListMedio.SelectedValue = ""
            End If
        Catch ex As Exception
        Finally
            UpdatePanel_agregar_serie.Update()
        End Try

    End Sub

    Protected Sub Button_agregar_serie_Click(sender As Object, e As EventArgs) Handles Button_agregar_serie.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassTrdDocumental
            Dim public_serie As Integer = 0
            Dim estado_desicion As Integer = 0
            Dim conservacion As Integer = 0
            Dim eliminacion As Integer = 0
            Dim digitalizacion As Integer = 0
            Dim seleccion As Integer = 0
            If Me.CheckBox_public_serie.Checked = True Then
                public_serie = 1
            End If
            If Me.CheckBoxDiposicion.Checked = True Then
                estado_desicion = 1
            End If
            If Me.CheckBoxConservTotal.Checked = True Then
                conservacion = 1
            End If
            If Me.CheckBoxSerieEliminacion.Checked = True Then
                eliminacion = 1
            End If
            If Me.CheckBoxSerieDigitalizacion.Checked = True Then
                digitalizacion = 1
            End If
            If Me.CheckBoxSerieSeleccion.Checked = True Then
                seleccion = 1
            End If
            If DropDownList_instrumento.Text = "" Then
                Refclasjava.Showscripman("Debe seleccionar el instrumento para agregar la serie documental", Me.UpdatePane_agregar_serie)
                Exit Sub
            End If
            If DropDownList_areas_departamento.SelectedValue Is Nothing Then
                Refclasjava.Showscripman("Debe seleccionar el área o departamento para agregar la serie documental", Me.UpdatePane_agregar_serie)
                Exit Sub
            End If
            If DropDownList_areas_departamento.SelectedValue = "" Then
                Refclasjava.Showscripman("Debe seleccionar el área para o departamento agregar la serie documental", Me.UpdatePane_agregar_serie)
                Exit Sub
            End If
            If Label_title_agregar_serie.Text = "Editar serie documental" Then
                Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                Result = Refclas.Actualiza_serie_documental(DropDownList_instrumento.SelectedValue, _
                                                             spli_t(1), _
                                                            DropDownList_areas_departamento.SelectedValue, _
                                                            Me.TextBox_nombre_serie.Text, _
                                                            Me.TextBox_observaciones_serie.Text, _
                                                            Me.TextBoxProceso.Text, Me.TextBoxProcedimiento.Text, _
                                                            Me.TextBoxCodigoSerie.Text, Me.DropDownListMedio.Text, _
                                                            estado_desicion, Me.DropDownList_tiempo_retencion_gestion.Text, _
                                                            Me.DropDownList_tiempo_retencion_central.Text, _
                                                            conservacion, eliminacion, digitalizacion, public_serie, seleccion, _
                                                            Me.TreeViewInstrumento, Me.UpdatePanel_treview_instrumento)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_agregar_serie)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_agregar_serie.Hide()
                End If
            Else
                Result = Refclas.Agregar_serie_documental(DropDownList_instrumento.SelectedValue, _
                                                         DropDownList_areas_departamento.SelectedValue, _
                                                         Me.TextBox_nombre_serie.Text, _
                                                         Me.TextBox_observaciones_serie.Text, _
                                                         Me.TextBoxProceso.Text, Me.TextBoxProcedimiento.Text, _
                                                         Me.TextBoxCodigoSerie.Text, Me.DropDownListMedio.Text, _
                                                         estado_desicion, Me.DropDownList_tiempo_retencion_gestion.Text, _
                                                         Me.DropDownList_tiempo_retencion_central.Text, _
                                                         conservacion, eliminacion, digitalizacion, public_serie, seleccion, _
                                                         Me.TreeViewInstrumento.SelectedNode, Me.UpdatePanel_treview_instrumento)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_agregar_serie)
                    Exit Sub
                Else
                    If Me.CheckBox_ventana_visible.Checked = True Then
                        Refclasjava.Showscripman("Se agrego una nueva serie al instumento", Me.UpdatePane_agregar_serie)
                        Exit Sub
                    Else
                        Me.ModalPopupExtender_agregar_serie.Hide()
                    End If

                End If
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_agregar_serie)
        End Try
    End Sub
   

    Protected Sub Button_activar_inactivar_elemento_Click(sender As Object, e As EventArgs) Handles Button_activar_inactivar_elemento.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_trd As New ClassTrdDocumental
            Dim estado As Integer = 0
            If Me.Check_activa_elemento.Checked = True Then
                estado = 1
            End If
            If HiddenField_oper.Value = "IAC-ACTIVA-TABLA1" Then
                If DropDownList_instrumento.Text = "" Then
                    Refclasjava.Showscripman("Debe seleccionar el instrumento para cambiar el estado de la serie", Me.UpdatePanel_boton_activar_inactivar_elemento)
                    Exit Sub
                End If
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Refclasjava.Showscripman("Debe seleccionar el nodo para cambiar el estado de la serie", Me.UpdatePanel_boton_activar_inactivar_elemento)
                    Exit Sub
                End If
                Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_trd.Cambia_estado_serie_documental(spli_t(1), _
                                                                    estado, _
                                                                    TreeViewInstrumento.SelectedNode.Text, _
                                                                    Me.DropDownList_instrumento.SelectedValue)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_activar_inactivar_elemento)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_activar_inactivar_elemento.Hide()
                End If
            End If
            If HiddenField_oper.Value = "IAC-ACTIVA-TABLA2" Then
                If DropDownList_instrumento.Text = "" Then
                    Refclasjava.Showscripman("Debe seleccionar el instrumento para cambiar el estado de la serie", Me.UpdatePanel_boton_activar_inactivar_elemento)
                    Exit Sub
                End If
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Refclasjava.Showscripman("Debe seleccionar el nodo para cambiar el estado de la sub serie", Me.UpdatePanel_boton_activar_inactivar_elemento)
                    Exit Sub
                End If
                Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_trd.Cambia_estado_sub_serie_documental(spli_t(1), estado, TreeViewInstrumento.SelectedNode.Text, _
                                                                    Me.DropDownList_instrumento.SelectedValue)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_activar_inactivar_elemento)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_activar_inactivar_elemento.Hide()
                End If
            End If
            If HiddenField_oper.Value = "IAC-ACTIVA-TABLA3" Then
                If DropDownList_instrumento.Text = "" Then
                    Refclasjava.Showscripman("Debe seleccionar el instrumento para cambiar el estado de la serie", Me.UpdatePanel_boton_activar_inactivar_elemento)
                    Exit Sub
                End If
                If TreeViewInstrumento.SelectedNode Is Nothing Then
                    Refclasjava.Showscripman("Debe seleccionar el nodo para cambiar el estado de la sub serie", Me.UpdatePanel_boton_activar_inactivar_elemento)
                    Exit Sub
                End If
                Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_trd.Cambia_estado_tipo_documento(spli_t(1), estado, TreeViewInstrumento.SelectedNode.Text, _
                                                                    Me.DropDownList_instrumento.SelectedValue)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_activar_inactivar_elemento)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_activar_inactivar_elemento.Hide()
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_activar_inactivar_elemento)
        End Try
    End Sub

    Private Sub Button_seleccion_agregar_Click(sender As Object, e As EventArgs) Handles Button_seleccion_agregar.Click
        Dim Refclasjava As New Classscrripjava
        Try

            If Me.CheckBox_agrega_tipo.Checked = True Then
                Me.Label_title_agregar_tipo_documento.Text = "Editar serie documental"
                Me.UpdatePanel_title_agregar_serie.Update()
                Me.ModalPopupExtender_agregar_tipo_documento.Show()
                Me.ModalPopupExtender_seleccion_agregar.Hide()
            Else
                DropDownList_tiempo_retencion_gestion_sub_serie.Items.Clear()
                DropDownList_tiempo_retencion_central_sub_serie.Items.Clear()
                For i As Integer = 0 To 100
                    DropDownList_tiempo_retencion_gestion_sub_serie.Items.Add(i)
                    DropDownList_tiempo_retencion_central_sub_serie.Items.Add(i)
                Next
                Me.ModalPopupExtender_agregar_sub_serie.Show()
                Me.Label_title_agregar_sub_serie.Text = "Agregar sub serie documental"
                Me.UpdatePanel_title_agregar_sub_serie.Update()
                Me.UpdatePanel_agregar_sub_serie.Update()
                Me.ModalPopupExtender_seleccion_agregar.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_seleccion_agregar)
        End Try
    End Sub

    Protected Sub Button_agregar_tipo_documento_Click(sender As Object, e As EventArgs) Handles Button_agregar_tipo_documento.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_trd As New ClassTrdDocumental
            If DropDownList_instrumento.Text = "" Then
                Refclasjava.Showscripman("Debe seleccionar el instrumento para agregar un elemento", Me.UpdatePane_agregar_tipo_documento)
                Exit Sub
            End If
            If TreeViewInstrumento.SelectedNode Is Nothing Then
                Refclasjava.Showscripman("Debe seleccionar el nodo para agregar el tipo documental", Me.UpdatePane_agregar_tipo_documento)
                Exit Sub
            End If

            Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
            If Label_title_agregar_tipo_documento.Text <> "Edita tipo documento serie" Then
                Result = Refclas_trd.Agregar_tipo_documental_a_serie(spli_t(1), _
                                                              Me.DropDownList_instrumento.SelectedValue, _
                                                              Me.TextBox_nombre_tipo_documento.Text, _
                                                              Me.TextBox_ruta_documento.Text, _
                                                              Me.TextBoxCodigoDocumento.Text, _
                                                              Me.TreeViewInstrumento.SelectedNode, _
                                                              Me.UpdatePanel_treview_instrumento, _
                                                              Me.CheckBox_trasv_serie)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_agregar_tipo_documento)
                    Exit Sub
                Else
                    If CheckBox_visible_tipo_documento.Checked = True Then
                        Refclasjava.Showscripman("Se agrega nuevo tipo documental a la serie", Me.UpdatePane_agregar_tipo_documento)
                    Else
                        Me.ModalPopupExtender_agregar_tipo_documento.Hide()
                    End If

                End If
            Else
                Result = Refclas_trd.Edita_tipo_documental(spli_t(1), _
                                                            Me.DropDownList_instrumento.SelectedValue, _
                                                            Me.TextBox_nombre_tipo_documento.Text, _
                                                            Me.TextBox_ruta_documento.Text, _
                                                            Me.TextBoxCodigoDocumento.Text, _
                                                            Me.TreeViewInstrumento.SelectedNode, _
                                                            Me.UpdatePanel_treview_instrumento, _
                                                            Me.CheckBox_trasv_serie)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_agregar_tipo_documento)
                    Exit Sub
                Else

                    Me.ModalPopupExtender_agregar_tipo_documento.Hide()
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_agregar_tipo_documento)
        End Try
    End Sub

    Private Sub Button_agregar_sub_serie_Click(sender As Object, e As EventArgs) Handles Button_agregar_sub_serie.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassTrdDocumental
            Dim public_sub_serie As Integer = 0
            Dim estado_desicion As Integer = 0
            Dim conservacion As Integer = 0
            Dim eliminacion As Integer = 0
            Dim digitalizacion As Integer = 0
            Dim seleccion As Integer = 0
            If Me.CheckBox_public_sub_serie.Checked = True Then
                public_sub_serie = 1
            End If
            If Me.CheckBoxDiposicion_sub_serie.Checked = True Then
                estado_desicion = 1
            End If
            If Me.CheckBoxConservTotal_sub_serie.Checked = True Then
                conservacion = 1
            End If
            If Me.CheckBoxSerieEliminacion_sub_serie.Checked = True Then
                eliminacion = 1
            End If
            If Me.CheckBoxSerieDigitalizacion_sub_serie.Checked = True Then
                digitalizacion = 1
            End If
            If Me.CheckBoxSerieSeleccion_sub_serie.Checked = True Then
                seleccion = 1
            End If
            If DropDownList_instrumento.Text = "" Then
                Refclasjava.Showscripman("Debe seleccionar el instrumento para agregar la serie documental", Me.UpdatePane_agregar_sub_serie)
                Exit Sub
            End If
            If DropDownList_areas_departamento.SelectedValue Is Nothing Then
                Refclasjava.Showscripman("Debe seleccionar el área o departamento para agregar la serie documental", Me.UpdatePane_agregar_sub_serie)
                Exit Sub
            End If
            If DropDownList_areas_departamento.SelectedValue = "" Then
                Refclasjava.Showscripman("Debe seleccionar el área para o departamento agregar la serie documental", Me.UpdatePane_agregar_sub_serie)
                Exit Sub
            End If
            If Label_title_agregar_sub_serie.Text = "Editar sub serie documental" Then
                Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                Result = Refclas.Actualiza_sub_serie_documental(DropDownList_instrumento.SelectedValue, _
                                                               spli_t(1), _
                                                               Me.TextBox_nombre_sub_serie.Text, _
                                                               Me.TextBox_observaciones_sub_serie.Text, _
                                                               Me.TextBoxProceso_sub.Text, _
                                                               Me.TextBoxProcedimiento_sub.Text, _
                                                               Me.TextBoxCodigo_sub_Serie.Text, _
                                                               Me.DropDownListMedio_sub_serie.Text, _
                                                               estado_desicion, _
                                                               Me.DropDownList_tiempo_retencion_gestion_sub_serie.Text, _
                                                               Me.DropDownList_tiempo_retencion_central_sub_serie.Text, _
                                                               conservacion, _
                                                               eliminacion, _
                                                               digitalizacion, _
                                                               public_sub_serie, _
                                                               seleccion, _
                                                               Me.TreeViewInstrumento, _
                                                               Me.UpdatePanel_treview_instrumento)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_agregar_sub_serie)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_agregar_sub_serie.Hide()
                End If
            Else
                Dim spli_t() As String = Me.TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
                Result = Refclas.Agregar_sub_serie_documental(DropDownList_instrumento.SelectedValue, _
                                                             spli_t(1), _
                                                             Me.TextBox_nombre_sub_serie.Text, _
                                                             Me.TextBox_observaciones_sub_serie.Text, _
                                                             Me.TextBoxProceso_sub.Text, _
                                                             Me.TextBoxProcedimiento_sub.Text, _
                                                             Me.TextBoxCodigo_sub_Serie.Text, _
                                                             Me.DropDownListMedio_sub_serie.Text, _
                                                             estado_desicion, _
                                                             Me.DropDownList_tiempo_retencion_gestion_sub_serie.Text, _
                                                             Me.DropDownList_tiempo_retencion_central_sub_serie.Text, _
                                                             conservacion, _
                                                             eliminacion, _
                                                             digitalizacion, _
                                                             public_sub_serie, _
                                                             seleccion, _
                                                             Me.TreeViewInstrumento.SelectedNode, _
                                                             Me.UpdatePanel_treview_instrumento)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_agregar_sub_serie)
                    Exit Sub
                Else
                    If Me.CheckBox_visible_ventana_sub_Serie.Checked = True Then
                        Refclasjava.Showscripman("Se agrega la sub serie al instrumento", Me.UpdatePane_agregar_sub_serie)
                    Else
                        Me.ModalPopupExtender_agregar_sub_serie.Hide()
                    End If

                End If
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_agregar_sub_serie)
        End Try
    End Sub

    Protected Sub Button_agregar_tipo_documento_sub_serie_Click(sender As Object, e As EventArgs) Handles Button_agregar_tipo_documento_sub_serie.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_trd As New ClassTrdDocumental
            If DropDownList_instrumento.Text = "" Then
                Refclasjava.Showscripman("Debe seleccionar el instrumento para agregar un elemento", Me.UpdatePane_agregar_tipo_documento_sub_serie)
                Exit Sub
            End If
            If TreeViewInstrumento.SelectedNode Is Nothing Then
                Refclasjava.Showscripman("Debe seleccionar el nodo para agregar el tipo documental", Me.UpdatePane_agregar_tipo_documento_sub_serie)
                Exit Sub
            End If

            Dim spli_t() As String = TreeViewInstrumento.SelectedNode.Value.ToString.Split("|")
            If Label_title_agregar_tipo_documento_sub_serie.Text = "Agregar tipo documento a sub serie documental" Then
                Result = Refclas_trd.Agregar_tipo_documental_a_sub_serie(spli_t(1), _
                                                              Me.DropDownList_instrumento.SelectedValue, _
                                                              Me.TextBox_nombre_tipo_documento_sub_serie.Text, _
                                                              Me.TextBox_ruta_documento_sub_serie.Text, _
                                                              Me.TextBoxCodigoDocumento_sub_serie.Text, _
                                                              Me.TreeViewInstrumento.SelectedNode, _
                                                              Me.UpdatePanel_treview_instrumento, _
                                                              Me.CheckBox_trasv_sub_serie)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_agregar_tipo_documento_sub_serie)
                    Exit Sub
                Else
                    If CheckBox_ventana_tipo_documental_sub_serie.Checked = True Then
                        Refclasjava.Showscripman("Se agrega nuevo tipo documental a la sub serie", Me.UpdatePane_agregar_tipo_documento_sub_serie)
                    Else
                        Me.ModalPopupExtender_agregar_tipo_documento_sub_serie.Hide()
                    End If

                End If
            Else
                Result = Refclas_trd.Edita_tipo_documental(spli_t(1), _
                                                              Me.DropDownList_instrumento.SelectedValue, _
                                                              Me.TextBox_nombre_tipo_documento_sub_serie.Text, _
                                                              Me.TextBox_ruta_documento_sub_serie.Text, _
                                                              Me.TextBoxCodigoDocumento_sub_serie.Text, _
                                                              Me.TreeViewInstrumento.SelectedNode, _
                                                              Me.UpdatePanel_treview_instrumento, _
                                                              Me.CheckBox_trasv_sub_serie)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePane_agregar_tipo_documento_sub_serie)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_agregar_tipo_documento_sub_serie.Hide()
                End If
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePane_agregar_tipo_documento_sub_serie)
        End Try
    End Sub
    Private Sub Button_activa_busqueda_treview_Click(sender As Object, e As EventArgs) Handles Button_activa_busqueda_treview.Click
        Dim reclas As New Classscrripjava
        Try         
            Dim Result As String = ""
            Dim Refclas_niveles As New Class_niveles_organizacion
            Dim node As New TreeNode
            Dim k = Hidden_texto_buequeda.Value.Replace("__doPostBack('TreeViewInstrumento','s", "")
            k = k.Replace("'", "")
            k = k.Replace("(", "")
            k = k.Replace(")", "")
            k = k.Replace("||", "|")
            k = k.Replace("\\", "/")
            node = Me.TreeViewInstrumento.FindNode(k)
            If Not node Is Nothing Then
                node.Selected = True
                Result = Refclas_niveles.Auto_expand(node)
                If Result <> "YES" Then
                    reclas.Showscripman_menu(Result, Me.UpdatePanel_treview_instrumento, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.UpdatePanel_treview_instrumento.Update()
            End If
        Catch ex As Exception
            reclas.Showscripman(ex.Message, Me.UpdatePanel_treview_instrumento)
        End Try
    End Sub
End Class