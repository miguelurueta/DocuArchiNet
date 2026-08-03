Public Class WebFormWorkflowEditarRuta
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
        If IsPostBack = False Then
            Dim rutas() As String = Nothing
            Result = Refclas.Solicita_nombres_rutas_workflow(rutas)
            If Result = "YES" Then
                Result = Refclas.Lista_rutas_interface_importacion(rutas, Me.DropDownList_rutas_workflow, Me.UpdatePanel_droplist_rutas)
                If Result <> "YES" Then
                    Label_Estado_ruta.Text = "Actualiza interface dice " & Result
                Else
                    '-----------------------------------------------
                    'Verfica que este seleccionada una ruta
                    '----------------------------------------------
                    If Me.DropDownList_rutas_workflow.Text <> "" Then
                        Dim Refclasworkflow As New ClassWorkflow
                        Dim id_ruta As Integer = 0
                        '--------------------------------------------
                        'Retorna el id de la ruta seleccionada
                        '--------------------------------------------
                        Dim Ref_class_wf_ruta As New Class_worflow_rutas
                        Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(Me.DropDownList_rutas_workflow.Text, id_ruta)
                        If Result <> "YES" Then
                            Label_Estado_ruta.Text = Label_Estado_ruta.Text & " Retorna id ruta dice " & Result
                        Else
                            '------------------------------------------------------------------
                            'Lista los campos disponibles de la ruta directamente en la tabla
                            '-------------------------------------------------------------------
                            Result = Refclas.Lista_campos_disponibles_ruta_tabla(Me.DropDownList_rutas_workflow.Text, Me.data_grid, Me.Hidden_resultado_gred, Me.titulo_label_title, Me.hdnEmailID, Me.UpdateGeneral)
                            If Result <> "YES" Then
                                Label_Estado_ruta.Text = Label_Estado_ruta.Text & " Lista campos disponibles ruta tabla dice " & Result
                            End If
                            '-------------------------------------------------------------------------
                            'Lista los campos que se listan en la ruta en la tabla de configuración
                            '-------------------------------------------------------------------------
                            Result = Refclas.Lista_campos_disponibles_ruta(id_ruta, Me.data_grid_dos, Me.Hidden_resultado_gred_dos, Me.titulo_label_title_dos, Me.hdnEmailID_dos, Me.UpdateGeneral_documentos)
                            If Result <> "YES" Then
                                Label_Estado_ruta.Text = Label_Estado_ruta.Text & " Lista campos disponibles ruta tabla configuración dice " & Result
                            End If
                        End If
                       

                    End If
                    
                End If
             

            End If
        End If

    End Sub

    Protected Sub DropDownList_rutas_workflow_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_rutas_workflow.SelectedIndexChanged
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.DropDownList_rutas_workflow.Text <> "" Then
                Dim Refclasworkflow As New ClassWorkflow
                Dim id_ruta As Integer = 0
                '--------------------------------------------
                'Retorna el id de la ruta seleccionada
                '--------------------------------------------
                Dim Ref_class_wf_ruta As New Class_worflow_rutas
                Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(Me.DropDownList_rutas_workflow.Text, _
                                                                    id_ruta)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_droplist_rutas, "ModalPopupExtender_mensaje_personalizado")
                Else
                    '------------------------------------------------------------------
                    'Lista los campos disponibles de la ruta directamente en la tabla
                    '-------------------------------------------------------------------
                    Result = Refclas.Lista_campos_disponibles_ruta_tabla(Me.DropDownList_rutas_workflow.Text, _
                                                                         Me.data_grid, _
                                                                         Me.Hidden_resultado_gred, _
                                                                         Me.titulo_label_title, _
                                                                         Me.hdnEmailID, _
                                                                         Me.UpdateGeneral)
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdatePanel_droplist_rutas, "ModalPopupExtender_mensaje_personalizado")
                    End If
                    '-------------------------------------------------------------------------
                    'Lista los campos que se listan en la ruta en la tabla de configuración
                    '-------------------------------------------------------------------------
                    Result = Refclas.Lista_campos_disponibles_ruta(id_ruta, _
                                                                   Me.data_grid_dos, _
                                                                   Me.Hidden_resultado_gred, _
                                                                   Me.titulo_label_title_dos, _
                                                                   Me.hdnEmailID_dos, _
                                                                   Me.UpdateGeneral)
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdatePanel_droplist_rutas, "ModalPopupExtender_mensaje_personalizado")
                    End If
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.UpdatePanel_droplist_rutas, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_agregar_campo_disponible_Click(sender As Object, e As EventArgs) Handles Button_agregar_campo_disponible.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.DropDownList_rutas_workflow.Text = "" Then
                clasjava.Showscripman_menu("Por favor seleccione la ruta", Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Matri_Typo_Mysql() As Matri_Typo_campo_Mysql = Nothing
            Result = Refclas.Typo_Campo_Resutl_Mysql(Matri_Typo_Mysql)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Lista_tipos_campo_interface(Matri_Typo_Mysql, Me.DropDownList_tipo_campo, Me.UpdatePanel_tipo_campo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_agregar_campo_ruta_workflow.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_agregar_campo_Click(sender As Object, e As EventArgs) Handles Button_agregar_campo.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            Dim obliga_campo As Integer = 0
            If Me.CheckBox_option_obligatorio.Checked = True Then
                obliga_campo = 1
            End If
           
            Result = Refclas.Adiciona_Nuevo_Campo_Ruta_workflow(Me.DropDownList_rutas_workflow.Text, Me.TextBox_nombre_campo.Text, Me.DropDownList_tipo_campo.Text, _
                                                              Me.TextBox_longitud_campo.Text, obliga_campo, Me.TextBox_longitud_campo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_agregar_campo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If Me.DropDownList_rutas_workflow.Text <> "" Then
                    Dim Refclasworkflow As New ClassWorkflow
                    Dim id_ruta As Integer = 0
                    '--------------------------------------------
                    'Retorna el id de la ruta seleccionada
                    '--------------------------------------------
                    Dim Ref_class_wf_ruta As New Class_worflow_rutas
                    Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(Me.DropDownList_rutas_workflow.Text, _
                                                                        id_ruta)
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_agregar_campo, "ModalPopupExtender_mensaje_personalizado")
                    Else
                        '------------------------------------------------------------------
                        'Lista los campos disponibles de la ruta directamente en la tabla
                        '-------------------------------------------------------------------
                        Result = Refclas.Lista_campos_disponibles_ruta_tabla(Me.DropDownList_rutas_workflow.Text, Me.data_grid, Me.Hidden_resultado_gred, Me.titulo_label_title, Me.hdnEmailID, Me.UpdateGeneral)
                        If Result <> "YES" Then
                            clasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_agregar_campo, "ModalPopupExtender_mensaje_personalizado")
                        End If

                    End If
                End If
                ModalPopupExtender_edition_agregar_campo_ruta_workflow.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.UpdatePanel_buton_agregar_campo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub DropDownList_tipo_campo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_tipo_campo.SelectedIndexChanged
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            Result = Refclas.Selecion_tipo_campo(Me.DropDownList_tipo_campo, Me.TextBox_longitud_campo, Me.UpdatePanel_longitud_campo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_tipo_campo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.UpdatePanel_tipo_campo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_importar_campo_Click(sender As Object, e As EventArgs) Handles Button_importar_campo.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.DropDownList_rutas_workflow.Text = "" Then
                clasjava.Showscripman_menu("Por favor seleccione la ruta", Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID.Value = "" Then
                clasjava.Showscripman_menu("Debe seleccionar el campo a importar de la lista", Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclasworkflow As New ClassWorkflow
            Dim id_ruta As Integer = 0
            '--------------------------------------------
            'Retorna el id de la ruta seleccionada
            '--------------------------------------------
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(Me.DropDownList_rutas_workflow.Text, _
                                                                id_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Adiciona_campo_listado_ruta_workflow(id_ruta, Me.hdnEmailID.Value, Me.HiddenType.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                '------------------------------------------------------------------
                'Lista listado campos ruta workflow
                '-------------------------------------------------------------------
                Result = Refclas.Lista_campos_disponibles_ruta(id_ruta, Me.data_grid_dos, Me.Hidden_resultado_gred, Me.titulo_label_title_dos, Me.hdnEmailID_dos, Me.UpdateGeneral_documentos)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.Update_botones_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_cancelar_confirmacion_Click(sender As Object, e As EventArgs) Handles Button_cancelar_confirmacion.Click
        ModalPopupExtender_edition_confirma_eliminar_campo_lista.Hide()
    End Sub

    Protected Sub Button_eliminar_Click(sender As Object, e As EventArgs) Handles Button_eliminar.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_dos.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el campos de la lista ", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_confirma_eliminar_campo_lista.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub data_grid_dos_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_dos.RowCreated
        e.Row.Cells(0).Visible = False
    End Sub

    Protected Sub Button_aceptar_confirmacion_Click(sender As Object, e As EventArgs) Handles Button_aceptar_confirmacion.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_dos.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el campos de la lista ", Me.UpdatePanel_confirma_eliminar_campo_lista, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Eliminar_campo_listado_ruta(Me.hdnEmailID_dos.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu("Debe seleccionar el campos de la lista ", Me.UpdatePanel_confirma_eliminar_campo_lista, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_estado_eliminar.Value = "YES"
                ModalPopupExtender_edition_confirma_eliminar_campo_lista.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.UpdatePanel_confirma_eliminar_campo_lista, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_editar_campo_lista_Click(sender As Object, e As EventArgs) Handles Button_editar_campo_lista.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_dos.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el campos de la lista ", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Asigna_datos_interface_configuracion_campo(Me.hdnEmailID_dos.Value, Me.CheckBox_Lista_Campo_ruta, _
             Me.CheckBox_Ordena_La_lista, Me.CheckBox_Campo_Prioridad_Lista, Me.UpdatePanel_configura_campo_lista)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_configura_campo_lista.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_actualiza_campo_lista_Click(sender As Object, e As EventArgs) Handles Button_actualiza_campo_lista.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            Result = Refclas.Actualiza_configuracion_campo_listado_ruta(Me.hdnEmailID_dos.Value, Me.CheckBox_Lista_Campo_ruta, Me.CheckBox_Ordena_La_lista,
                                    Me.CheckBox_Campo_Prioridad_Lista, Me.Hidden_estado_configura_campo_lista, Me.UpdatePanel_configura_campo_ruta_boton)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_configura_campo_ruta_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_configura_campo_lista.Hide()
        Catch ex As Exception
            clasjava.Showscripman_menu("Inconsistencia general " & ex.Message, Me.UpdatePanel_configura_campo_ruta_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_configura_orden_Click(sender As Object, e As EventArgs) Handles Button_activa_configura_orden.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.DropDownList_rutas_workflow.Text = "" Then
                clasjava.Showscripman_menu("Por favor seleccione la ruta", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
           
            Dim Refclasworkflow As New ClassWorkflow
            Dim id_ruta As Integer = 0
            '--------------------------------------------
            'Retorna el id de la ruta seleccionada
            '--------------------------------------------
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(Me.DropDownList_rutas_workflow.Text, _
                                                                id_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Lista_orden_ordenacion_listado_ruta(id_ruta, Me.DropDownList_configuracion_listado_ruta, Me.UpdatePanel_configura_listado_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_configura_listado_ruta.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Protected Sub Button_actualiza_configuracion_ruta_Click(sender As Object, e As EventArgs) Handles Button_actualiza_configuracion_ruta.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclasworkflow As New ClassWorkflow
            Dim id_ruta As Integer = 0
            '--------------------------------------------
            'Retorna el id de la ruta seleccionada
            '--------------------------------------------
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(Me.DropDownList_rutas_workflow.Text, id_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_configura_listado_ruta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estado_configuracion As String = "YES"
            Result = Refclas.Verifica_existencia_lista_ordenacion_listado_ruta(id_ruta, estado_configuracion)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_configura_listado_ruta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_configuracion = "NO" Then
                Result = Refclas.Registra_configuracion_listado_ordenacion_ruta(id_ruta, DropDownList_configuracion_listado_ruta.Text)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_configura_listado_ruta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = Refclas.Actualiza_configuracion_listado_ordenacion_ruta(id_ruta, DropDownList_configuracion_listado_ruta.Text)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_configura_listado_ruta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            ModalPopupExtender_edition_configura_listado_ruta.Hide()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_configura_listado_ruta, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Protected Sub ImageButton_baja_item_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_baja_item.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclasworkflow As New ClassWorkflow
            Dim id_ruta As Integer = 0
            '--------------------------------------------
            'Retorna el id de la ruta seleccionada
            '--------------------------------------------
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(Me.DropDownList_rutas_workflow.Text, _
                                                                id_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID_dos.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el campos de la lista ", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Bajar_Indice_campo_lista(Me.hdnEmailID_dos.Value, id_ruta, Me.Hidden_id_idex_config_siguiente, _
                     Me.Hidden_ide_orden_siguiente, Me.Hidden_id_orden_seleccion, Me.Hidden_resultado_aprobacion)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ImageButton_sube_item_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_sube_item.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclasworkflow As New ClassWorkflow
            Dim id_ruta As Integer = 0
            '--------------------------------------------
            'Retorna el id de la ruta seleccionada
            '--------------------------------------------
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(Me.DropDownList_rutas_workflow.Text, _
                                                                id_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID_dos.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el campos de la lista ", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Subir_Indice_campo_lista(Me.hdnEmailID_dos.Value, id_ruta, Me.Hidden_id_idex_config_siguiente, _
                     Me.Hidden_ide_orden_siguiente, Me.Hidden_id_orden_seleccion, Me.Hidden_resultado_aprobacion)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_campo_radicado_Click(sender As Object, e As EventArgs) Handles Button_activa_campo_radicado.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_dos.Value = "-1" Then
                clasjava.Showscripman_menu("Seleccione el registro para activar el campo radicado", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_nombre_campo.Value = "campo_radicado"
            Me.Label_promp_campo.Text = "Desea activar el campo seleccionado como campo de radicado en la ruta?"
            Me.UpdatePanel_digitaliza_actualizar_campo_lista.Update()
            Me.ModalPopupExtender_edition_digitaliza_actualizar_campo_lista.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_cancelar_confirmacion_actualiza_campo_Click(sender As Object, e As EventArgs) Handles Button_cancelar_confirmacion_actualiza_campo.Click
        Me.ModalPopupExtender_edition_digitaliza_actualizar_campo_lista.Hide()
    End Sub

    Protected Sub Button_aceptar_confirmacion_actualiza_campo_Click(sender As Object, e As EventArgs) Handles Button_aceptar_confirmacion_actualiza_campo.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If TextBox_clave_campo.Text <> "7894561230.7894561230." Then
                clasjava.Showscripman_menu("Clave incorrecta", Me.UpdatePanel_digitaliza_actualizar_campo_lista_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_estado_actualizar.Value = "-1"
            Result = Refclas.Actualiza_estado_campo_listado_ruta(Me.hdnEmailID_dos.Value, _
                                                                 Hidden_nombre_campo.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_digitaliza_actualizar_campo_lista_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_estado_actualizar.Value = "1"
                Me.ModalPopupExtender_edition_digitaliza_actualizar_campo_lista.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_digitaliza_actualizar_campo_lista_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_campo_tramite_Click(sender As Object, e As EventArgs) Handles Button_activa_campo_tramite.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_dos.Value = "-1" Then
                clasjava.Showscripman_menu("Seleccione el registro para activar el campo trámite", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_nombre_campo.Value = "campo_tramite"
            Me.Label_promp_campo.Text = "Desea activar el campo seleccionado como campo trámite en la ruta?"
            Me.UpdatePanel_digitaliza_actualizar_campo_lista.Update()
            Me.ModalPopupExtender_edition_digitaliza_actualizar_campo_lista.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_campo_beneficiario_Click(sender As Object, e As EventArgs) Handles Button_activa_campo_beneficiario.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_dos.Value = "-1" Then
                clasjava.Showscripman_menu("Seleccione el registro para activar el campo beneficiario", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_nombre_campo.Value = "Campo_beneficiario"
            Me.Label_promp_campo.Text = "Desea activar el campo seleccionado como campo beneficiario en la ruta?"
            Me.UpdatePanel_digitaliza_actualizar_campo_lista.Update()
            Me.ModalPopupExtender_edition_digitaliza_actualizar_campo_lista.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_campo_fecha_Click(sender As Object, e As EventArgs) Handles Button_activa_campo_fecha.Click
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_dos.Value = "-1" Then
                clasjava.Showscripman_menu("Seleccione el registro para activar el campo fecha vencimiento", Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_nombre_campo.Value = "campo_fecha_vence"
            Me.Label_promp_campo.Text = "Desea activar el campo seleccionado como campo fecha vencimiento en la ruta?"
            Me.UpdatePanel_digitaliza_actualizar_campo_lista.Update()
            Me.ModalPopupExtender_edition_digitaliza_actualizar_campo_lista.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_expediente_seleccionado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class