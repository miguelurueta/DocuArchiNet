Public Class WebFormGestionFlujoTrabajoCamaras
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Dim cs As ClientScriptManager = Page.ClientScript
        'Dim clasjava As New Classscrripjava
        'Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        'If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
        '    ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        'End If
        If IsPostBack = False Then
            'Dim Result As String = ""
            'Dim Refclas As New ClassGestionTareasFlujoTrabajo
            'Dim stru_rutas() As stru_ruta = Nothing
            'Result = Refclas.Solicita_rutas_workflow(stru_rutas)
            'If Result = "YES" Then
            '    'Result = Refclas.Lista_rutas_workflow_interface(stru_rutas, Me.DropDownList_rutas, Me.UpdatePanel_drow_rutas)
            '    Result = Refclas.Lista_rutas_workflow_interface(stru_rutas, Me.DropDownList_drow_rutas_edita, Me.UpdatePanel_drow_rutas_edita)
            'End If
            'Dim Stru_tipos_tramite_sii() As stru_tipos_tramite_sii = Nothing
            'Dim Class_ws_tipotramitesii_determina_gabinete As New Class_ws_tipotramitesii_determina_gabinete
            'Result = Class_ws_tipotramitesii_determina_gabinete.Lista_descripcion_tramites_flujo_trabajo(Stru_tipos_tramite_sii)
            'If Result = "YES" Then
            '    Result = Class_ws_tipotramitesii_determina_gabinete.Lista_tipos_tramite_interface(Stru_tipos_tramite_sii, Me.DropDownList_tramites, Me.UpdatePanel_drow_tramites)
            'End If
        End If
    End Sub
    Private Sub DropDownList_tramites_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_tramites.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim Refclas As New ClassGestionTareasFlujoTrabajo
            Dim stru_rutas() As stru_ruta = Nothing
            Dim id_tipo_doc_entrante As Integer = 0
            Me.DropDownList_rutas.Items.Clear()
            Me.UpdatePanel_drow_rutas.Update()
            Me.DropDownList_actividades.Items.Clear()
            Me.UpdatePanel_drow_actividades.Update()
            Me.DropDownList_usurios.Items.Clear()
            Me.UpdatePanel_drow_usuarios.Update()
            If Me.DropDownList_tramites.Text = "" Then
                Exit Sub
            End If
            Dim split() As String = Me.DropDownList_tramites.SelectedItem.Value.Split("|")
            Dim value As String = split(1)
            Result = Class_tipo_doc_entrante.Solicita_identificacion_tipo_documento_entrante_externo_nombre(value,
                                                                                                            id_tipo_doc_entrante)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim existencia_relacion_flujo As Integer = 0
            Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
            Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_existencia_relacion_flujo_tramite(id_tipo_doc_entrante,
                                                                                                        existencia_relacion_flujo)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If existencia_relacion_flujo = 1 Then
                Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_relaciones_flujo_trabajo_tramite(id_tipo_doc_entrante,
                                                                                                           1,
                                                                                                           Me.DropDownList_rutas)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = Refclas.Solicita_rutas_workflow(stru_rutas)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Lista_rutas_workflow_interface(stru_rutas, Me.DropDownList_rutas, Me.UpdatePanel_drow_rutas)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub DropDownList_rutas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_rutas.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGestionTareasFlujoTrabajo
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim stru_listado() As stru_listado_actividades = Nothing
            Dim id_tipo_doc_entrante As Integer = 0
            If Me.DropDownList_rutas.Text = "" Then
                Me.DropDownList_actividades.Items.Clear()
                Me.UpdatePanel_drow_actividades.Update()
                Me.DropDownList_usurios.Items.Clear()
                Me.UpdatePanel_drow_usuarios.Update()
                Exit Sub
            Else
                Dim split() As String = Me.DropDownList_tramites.SelectedItem.Value.Split("|")
                Dim value As String = split(1)
                Result = Class_tipo_doc_entrante.Solicita_identificacion_tipo_documento_entrante_externo_nombre(value,
                                                                                                                id_tipo_doc_entrante)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_rutas, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim existencia_relacion_flujo As Integer = 0
                Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
                Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_existencia_relacion_flujo_tramite(id_tipo_doc_entrante,
                                                                                                            existencia_relacion_flujo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_rutas, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If existencia_relacion_flujo = 1 Then
                    Result = Class_wf_registro_actividaes_flujos_trabajo.Lista_actividades_workflow_flujo_drowlist_tipo(1,
                                                                                                                       Me.DropDownList_rutas.SelectedItem.Value,
                                                                                                                       3,
                                                                                                                       Me.DropDownList_actividades,
                                                                                                                       Me.UpdatePanel_drow_actividades)
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_rutas, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                Else
                    Result = Refclas.Solicita_listado_actividades(Val(Me.DropDownList_rutas.SelectedValue), stru_listado)
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_rutas, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Result = Refclas.Lista_actividades_Interface(stru_listado, Me.DropDownList_actividades, Me.UpdatePanel_drow_actividades)
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_rutas, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                End If

            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_drow_rutas, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub DropDownList_actividades_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_actividades.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try

            If Me.DropDownList_actividades.Text = "" Then
                Me.DropDownList_usurios.Items.Clear()
                Me.UpdatePanel_drow_usuarios.Update()
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim id_tipo_doc_entrante As Integer = 0
            Dim split() As String = Me.DropDownList_tramites.SelectedItem.Value.Split("|")
            Dim value As String = split(1)
            Result = Class_tipo_doc_entrante.Solicita_identificacion_tipo_documento_entrante_externo_nombre(value,
                                                                                                            id_tipo_doc_entrante)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim existencia_relacion_flujo As Integer = 0
            Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
            Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_existencia_relacion_flujo_tramite(id_tipo_doc_entrante,
                                                                                                        existencia_relacion_flujo)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Class_usuario_workflow As New Class_usuario_workflow
            If existencia_relacion_flujo = 1 Then
                Result = Class_usuario_workflow.Solicita_usuarios_relacionados_actividad_flujo(0,
                                                                                               Me.DropDownList_actividades.SelectedItem.Value,
                                                                                               Me.DropDownList_usurios,
                                                                                               Me.UpdatePanel_drow_usuarios)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Me.DropDownList_usurios.Items.Clear()
                Me.UpdatePanel_drow_usuarios.Update()
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_drow_tramites, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_consulta_recibo_Click(sender As Object, e As EventArgs) Handles Button_consulta_recibo.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGestionTareasFlujoTrabajo
            If Me.TextBox_recibo_caja.Text = "" Then
                Refclasjava.Showscripman_menu("Debe digitar el consecutivo del recibo", Me.UpdatePanel_buton_consulta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.DropDownList_ante_pone.Text = "" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el valor de ante poner ", Me.UpdatePanel_buton_consulta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Consulta_datos_recibo_sii(Me.Page)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_consulta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_consulta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_reistrar_flujo_Click(sender As Object, e As EventArgs) Handles Button_reistrar_flujo.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGestionTareasFlujoTrabajo
            Result = Refclas.Inicia_Registra_flujo_trabajo(Me.Page)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_registro, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_elimina_flujo_Click(sender As Object, e As EventArgs) Handles Button_elimina_flujo.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGestionTareasFlujoTrabajo
            Result = Refclas.Eliminar_Flujo_Workflow(Me.Page)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_elimina, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_elimina, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ButtonEdita_actualiza_Click(sender As Object, e As EventArgs) Handles ButtonEdita_actualiza.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGestionTareasFlujoTrabajo
            Result = Refclas.Main_default_actualiza_datos_imagen(Me.TextBox_recibo_actualiza.Text,
                                                                 Me.DropDownList_drow_rutas_edita.SelectedItem.Text,
                                                                Me.DropDownList_antepone_actuliza.Text)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_actualiza, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_actualiza, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub


End Class