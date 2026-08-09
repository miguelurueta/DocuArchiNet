Public Class WebFormWorkflowRelacionFlujoTramite
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Page.IsPostBack = False Then
            Dim Result As String = ""
            Dim Refclas_flujo_trabajo As New Class_flujo_trabajo_workflow
            Result = Refclas_flujo_trabajo.Lista_ruta_flujos_tramites_relacionados(Me.TreeView_lista_flujos, _
                                                                                   Me.update_tre_principal)
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
            End If
        End If

    End Sub

    Private Sub Button_activa_relacion_Click(sender As Object, e As EventArgs) Handles Button_activa_relacion.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            If Me.TreeView_lista_flujos.SelectedNode Is Nothing Then
                Exit Sub
            End If
            Dim tre_node As TreeNode = Me.TreeView_lista_flujos.SelectedNode
            Dim value_node As String = tre_node.Value
            Dim split_value_node() As String = value_node.ToString.Split("|")
            If split_value_node(0) <> "FU" Then
                Refclasjava.Showscripman_menu("Debe seleccionar un flujo de trabajo para relacionar con el trámite ", Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_ruta As Integer = 0
            'Result = Refclas.Solicita_id_ruta_flujo_trabajo(Val(split_value_node(1)), _
            '                                                id_ruta)
            'If Result <> "YES" Then
            '    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim Refclas_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Refclas_tipo_doc_entrante.Solicita_lista_tramite(id_ruta, _
                                                                      1, _
                                                                      "", _
                                                                      "", _
                                                                      "", _
                                                                      Me.data_grid, _
                                                                      Me.titulo_label_grid, _
                                                                      Me.hdnEmailID, _
                                                                      Me.UpdateGeneral_documentos)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.hdnEmailID.Value = "0"
            ModalPopupExtender_edition_lista_tramites.Show()
            UpdateGeneral_documentos.Update()
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub
    Private Sub Button_busqueda_Click(sender As Object, e As EventArgs) Handles Button_busqueda.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            Dim Refclas_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Refclas_tipo_doc_entrante.Solicita_lista_tramite(0, _
                                                                      2, _
                                                                      Me.auto_complex.Text, _
                                                                      "", _
                                                                      "", _
                                                                      Me.data_grid, _
                                                                      Me.titulo_label_grid, _
                                                                      Me.hdnEmailID, _
                                                                      Me.UpdateGeneral_documentos)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.hdnEmailID.Value = "0"
            UpdateGeneral_documentos.Update()
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    
    Private Sub data_grid_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button_relaciona_tramite_flujo_Click(sender As Object, e As EventArgs) Handles Button_relaciona_tramite_flujo.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            If Me.TreeView_lista_flujos.SelectedNode Is Nothing Then
                Exit Sub
            End If
            Dim tre_node As TreeNode = Me.TreeView_lista_flujos.SelectedNode
            Dim value_node As String = tre_node.Value
            Dim split_value_node() As String = value_node.ToString.Split("|")
            If split_value_node(0) <> "FU" Then
                Refclasjava.Showscripman_menu("Debe seleccionar un flujo de trabajo para relacionar con el trámite ", Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID.Value = "0" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el tramite de la lista ", Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Relacionar_tramites_documentales_a_flujos_de_trabajo(Val(split_value_node(1)), _
                                                                                  Me.hdnEmailID.Value, _
                                                                                  tre_node, _
                                                                                  Me.update_tre_principal)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Refclasjava.Showscripman_menu("Trámite relacionado correctamente ", Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_contendor_botones_desicion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_activa_eliminar_Click(sender As Object, e As EventArgs) Handles Button_activa_eliminar.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            If Me.TreeView_lista_flujos.SelectedNode Is Nothing Then
                Exit Sub
            End If
            Dim tre_node As TreeNode = Me.TreeView_lista_flujos.SelectedNode
            Dim value_node As String = tre_node.Value
            Dim split_value_node() As String = value_node.ToString.Split("|")
            If split_value_node(0) <> "TRA" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el trámite para eliminar la relación ", Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Label_title_comfirma_eliminar.Text = "Desea eliminar la relación del trámite (" & tre_node.Text & ") con el flujo de trabajo"
            Me.UpdatePanel_confirma_eliminar.Update()
            Me.ModalPopupExtender_edition_confirma_eliminar.Show()
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_cancelar_confirmacion_Click(sender As Object, e As EventArgs) Handles Button_cancelar_confirmacion.Click
        Me.ModalPopupExtender_edition_confirma_eliminar.Hide()
    End Sub

    Private Sub Button_aceptar_confirmacion_Click(sender As Object, e As EventArgs) Handles Button_aceptar_confirmacion.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            If Me.TreeView_lista_flujos.SelectedNode Is Nothing Then
                Exit Sub
            End If
            Dim tre_node As TreeNode = Me.TreeView_lista_flujos.SelectedNode
            Dim value_node As String = tre_node.Value
            Dim split_value_node() As String = value_node.ToString.Split("|")
            Result = Refclas.Elimina_relacion_tramite_flujo_trabajo(Val(split_value_node(1)), Me.TreeView_lista_flujos, tre_node, Me.update_tre_principal)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_confirma_eliminar, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_confirma_eliminar.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_confirma_eliminar, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_activa_busqueda_treview_Click(sender As Object, e As EventArgs) Handles Button_activa_busqueda_treview.Click
        Dim reclas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref As New ClassGestorSesion

            Result = Ref.selecciona_treview_general(Me.Page, _
                                                    Me.TextBox_busqueda_tre.Text, _
                                                    "TreeView_lista_flujos")
            If Result <> "YES" Then
                reclas.Showscripman_menu(Result, Me.update_tre_principal, "ModalPopupExtender_mensaje_personalizado")

            Else
                Me.update_tre_principal.Update()
            End If
        Catch ex As Exception
            reclas.Showscripman_menu(ex.Message, Me.update_tre_principal, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub
End Class