Public Class WebFormPendientes
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Page.IsPostBack = False Then
            Dim ref_Class_tarea_pendiente As New Class_tarea_pendiente
            ref_Class_tarea_pendiente.Lista_Tareas_Pendiente(Me.GridViewlista)
            Dim Reflcas As New InicioWorkflow
            Dim Result As String = ""
            Result = ""
            Hidden_seleccion.Value = "YES"
            Dim Matri_dat() As String
            Erase Matri_dat
            If Session.Item("CAMBIO_USUARIO") = 0 Then
                Me.ImageButtonEnviarUsuario.Visible = False
            Else
                Me.ImageButtonEnviarUsuario.Visible = True
            End If
            If Session.Item("Cambio_Ruta") = 0 Then
                Me.ImageButtonEnviaActividad.Visible = False
            Else
                Me.ImageButtonEnviaActividad.Visible = True
            End If
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Result = ref_Class_grupos_workflow.Solicita_Actividades_Disponibles_Envio(Session.Item("Id_Grupo_Workflow"), _
                                                                                      Matri_dat)
            If Result = "YES" And Not Matri_dat Is Nothing Then
                Me.DropDownActividades.Items.Clear()
                For z As Integer = 0 To Matri_dat.Length - 1
                    Me.DropDownActividades.Items.Add(Matri_dat(z))
                Next
            Else
                HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Solicita Actividades de Envío:" & Result & vbCrLf
            End If

        End If

    End Sub

    Private Sub GridViewlista_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowCreated
        e.Row.Cells(1).Visible = False
        e.Row.Cells(2).Visible = False
    End Sub
    Private Sub ButtonSubir_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonSubir.Click
        If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") <> "0" Then
            Me.ModalPopupTexto.Show()
        End If
    End Sub

    Private Sub btnOkay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOkay.Click
        Dim Refcriptman As New Classscrripjava
        Dim Refclas As New Class_tarea_pendiente
        Try
            Dim Result As String = ""
            If Me.TextBoxdatos.Text = "" Then
                Refcriptman.Showscripman_menu("Digite la identificación del pendiente ", Me.UpdatePnael_botones_subir_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Subir_Tarea_Pendiente(Me.TextBoxdatos.Text,
                                                   Page)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.UpdatePnael_botones_subir_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Dim ref_Class_tarea_pendiente As New Class_tarea_pendiente
                Result = ref_Class_tarea_pendiente.Lista_Tareas_Pendiente(Me.GridViewlista)
                If Result <> "YES" Then
                    Refcriptman.Showscripman_menu(Result, Me.UpdatePnael_botones_subir_pendiente, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.Hidden_resultado.Value = "YES"
                UpdatePanelmensaje.Update()
                Me.ModalPopupTexto.Hide()
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePnael_botones_subir_pendiente, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    
    Private Sub ButtonFiltrar_Click(sender As Object, e As EventArgs) Handles ButtonFiltrar.Click
        Me.ModalPopupExtender_Filtro.Show()
    End Sub

    Private Sub ImageButtonEnviarUsuario_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonEnviarUsuario.Click
        Dim mens As New Classscrripjava
        Try
            If Me.Hidden_id.Value = "-1" Then
                mens.Showscripman_menu("Debe seleccionar tareas para enviar", Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("CAMBIO_USUARIO") = "0" Then
                mens.Showscripman_menu("El usuario no tiene permiso enviar tarea a otro usuario", Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split_seleccion() As String = Me.Hidden_id.Value.Split("-")
            Dim Result As String = ""
            Dim Id_tarea_seleccionada As Object = split_seleccion(1)
            Dim refclas As New Class_flujo_trabajo_workflow
            Result = refclas.Verifica_existencia_actividades_selecionadas_en_pendiente_flujo_de_trabajo(Id_tarea_seleccionada)
            If Result <> "YES" Then
                mens.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '-----------------------------------------------
            'Verifica la existencia de ruta abierta cerrada
            '-----------------------------------------------
            Dim estado_tramite_ruta As Integer = 0
            Dim tramite As String = ""
            Dim Refclas_workflow_rutas As New Class_worflow_rutas
            Result = Refclas_workflow_rutas.Solicita_etado_abierto_cerrado_ruta_tarea(Id_tarea_seleccionada, _
                                                                                      Session.Item("Id_Ruta_Workflow"), _
                                                                                      estado_tramite_ruta, _
                                                                                      tramite)
            If Result <> "YES" Then
                mens.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_tramite_ruta = 1 Then
                mens.Showscripman_menu("La tarea pertenece al tipo trámite (" & tramite & ") de ruta cerrada. Imposible enviar tarea a usuarios", Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '------------------------------------------------------------------------------
            'Verifica respuesta radicado para inhabilitar el boton de envío y reasignación
            '------------------------------------------------------------------------------
            btnOkpagina.Visible = True
            ButtonReasignarTerminar.Visible = True
            Dim refclasgestion As New Classgestionrespuesta
            Result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), Id_tarea_seleccionada)
            If Result <> "YES" And Result <> "El trámite requiere de una confirmación de respuesta" And Result <> "El trámite requiere de un radicado de respuesta" Then
                mens.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If Result = "El trámite requiere de una confirmación de respuesta" Or Result = "El trámite requiere de un radicado de respuesta" Then
                    Hidden_vi_reasigna.Value = "1"
                    Me.btnOkpagina.Visible = False
                    Me.ButtonReasignarTerminar.Visible = True
                Else
                    Hidden_vi_reasigna.Value = "2"
                    Me.btnOkpagina.Visible = True
                    Me.ButtonReasignarTerminar.Visible = False
                End If
            End If
            Hidden_res_envi.Value = ""
            Session.Item("OPCIONSELECIONPENDIENTE") = "ENVIARUSUARIO-S"
            Session.Item("SESIONITERCAMBIO") = ""
            Me.Labeletiqueta.Text = "Enviar tarea a usuario especifico"
            Me.frameeditexpanse_.Attributes("SRC") = "../workflow/WebFormEnviaUsuario.aspx"
            Me.ButtonReasignarTerminar.Visible = True
            Me.Updatecondiciona.Update()
            Me.UpdatePanelpagina.Update()
            Me.ModalPopupExtendermesjpagina.Show()
        Catch ex As Exception
            mens.Showscripman_menu(ex.Message, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ImageButtonEnviaActividad_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonEnviaActividad.Click
        Dim mens As New Classscrripjava
        Try
            If Me.Hidden_id.Value = "-1" Then
                mens.Showscripman_menu("Debe seleccionar tareas para enviar", Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("CAMBIO_USUARIO") = "0" Then
                mens.Showscripman_menu("El usuario no tiene permiso enviar tarea a otro usuario", Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split_seleccion() As String = Me.Hidden_id.Value.Split("-")
            Dim Result As String = ""
            Dim Id_tarea_seleccionada As Object = split_seleccion(1)
            Dim refclas As New Class_flujo_trabajo_workflow
            Result = refclas.Verifica_existencia_actividades_selecionadas_en_pendiente_flujo_de_trabajo(Id_tarea_seleccionada)
            If Result <> "YES" Then
                mens.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '-----------------------------------------------
            'Verifica la existencia de ruta abierta cerrada
            '-----------------------------------------------
            Dim estado_tramite_ruta As Integer = 0
            Dim tramite As String = ""
            Dim Refclas_workflow_rutas As New Class_worflow_rutas
            Result = Refclas_workflow_rutas.Solicita_etado_abierto_cerrado_ruta_tarea(Id_tarea_seleccionada, _
                                                                                      Session.Item("Id_Ruta_Workflow"), _
                                                                                      estado_tramite_ruta, _
                                                                                      tramite)
            If Result <> "YES" Then
                mens.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_tramite_ruta = 1 Then
                mens.Showscripman_menu("La tarea pertenece al tipo trámite (" & tramite & ") de ruta cerrada. Imposible enviar tarea a grupos", Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.btnOkpagina.Visible = True
            Hidden_res_envi.Value = ""
            Session.Item("OPCIONSELECIONPENDIENTE") = "ENVIARACTIVIDAD-S"
            Session.Item("SESIONITERCAMBIO") = ""
            Me.Labeletiqueta.Text = "Enviar tarea a Actividad"
            Me.frameeditexpanse_.Attributes("SRC") = "../workflow/WebFormEnviaActividad.aspx"
            Me.ButtonReasignarTerminar.Visible = False
            Me.Updatecondiciona.Update()
            Me.UpdatePanelpagina.Update()
            Me.ModalPopupExtendermesjpagina.Show()
        Catch ex As Exception
            mens.Showscripman_menu(ex.Message, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub btnOkpagina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOkpagina.Click
        Dim Refcriptman As New Classscrripjava
        '***********************************************
        'Ejecuta accion enviar actividad a usuario
        '***********************************************
        Try
            If Me.HiddenPROMP.Value = "0" Then Exit Sub
            Dim Result As String = ""
            Dim Refclas_wf As New ClassWorkflow
            Dim Result_correo As String = ""
            Result = Refclas_wf.Pre_envio_tarea_a_qctividad_desde_pendiente(Me.Page, _
                                                                            Hidden_id.Value, _
                                                                            Result_correo)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Result_correo <> "" Then
                Refcriptman.Showscripman_menu(Result_correo, Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImageButtonterminar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonterminar.Click
        Dim Refcriptman As New Classscrripjava
        Try

            If Me.Hidden_id.Value = "-1" Then
                Refcriptman.Showscripman_menu("Po favor seleccione el registro de la lista, para enviar la actividad", Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split_seleccion() As String = Me.Hidden_id.Value.Split("-")
            Dim Result As String = ""
            Dim Id_tarea_seleccionada As Object = split_seleccion(1)
            Dim Refclas_w As New ClassWorkflow
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Dim Radicado As String = ""
            Dim Id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_actividad_flujo_trabajo As Integer = 0
            Hidden_lista_ruta_flujo.Value = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Id_tarea_seleccionada, _
                                                                                 Radicado)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_f.Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(Radicado, _
                                                                                                                id_actividad_flujo_trabajo, _
                                                                                                                Id_flujo_trabajo, _
                                                                                                                id_usuario_workflow_actividad_flujo_trabajo, _
                                                                                                                HttpContext.Current.Session.Item("Id_Usuario_Workflow"), _
                                                                                                                Id_tarea_seleccionada)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Nombre_flujo_trabajo As String = ""
            If Id_flujo_trabajo <> 0 Then
                Result = Refclas_f.SolicitaNombreFlujoTrabajoPorIdFlujo(Id_flujo_trabajo, _
                                                                             Nombre_flujo_trabajo)
                If Result <> "YES" Then
                    Refcriptman.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If id_actividad_flujo_trabajo <> 0 Then
                    Result = Refclas_f.Solicita_listado_actividades_para_envio_tarea_a_flujo(Id_flujo_trabajo,
                                                                                             data_grid,
                                                                                             titulo_label_grid,
                                                                                             Label_nombre_flujo,
                                                                                              Hidden_id_actividad_flujo,
                                                                                             UpdateGeneral_documentos,
                                                                                             Nombre_flujo_trabajo,
                                                                                             id_actividad_flujo_trabajo,
                                                                                             0)
                    If Result <> "YES" Then
                        Refcriptman.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                        Hidden_lista_ruta_flujo.Value = "F"
                        Exit Sub
                    End If
                Else

                    Result = Refclas_f.Solicita_listado_actividades_para_envio_tarea_a_flujo(Id_flujo_trabajo,
                                                                                             data_grid,
                                                                                             titulo_label_grid,
                                                                                             Label_nombre_flujo,
                                                                                             Hidden_id_actividad_flujo,
                                                                                             UpdateGeneral_documentos,
                                                                                             Nombre_flujo_trabajo,
                                                                                             id_actividad_flujo_trabajo,
                                                                                             0)
                    If Result <> "YES" Then
                        Refcriptman.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        Me.ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                        Hidden_lista_ruta_flujo.Value = "F"
                        Exit Sub
                    End If
                End If


            End If
            'Dim Nombre_ruta As String = ""
            'Dim Ref_class_wf_ruta As New Class_worflow_rutas
            'Result = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(Nombre_ruta)
            'If Result <> "YES" Then
            '    Refcriptman.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim ref_grupos_workflow As New Class_grupos_workflow
            Result = ref_grupos_workflow.Solicita_Listado_actividades_para_envio_de_tareas_a_ruta(Session.Item("Id_Grupo_Workflow"),
                                                                                                  data_grid_actividad,
                                                                                                  titulo_label_grid_actividad,
                                                                                                  Label_title_nombre_ruta,
                                                                                                  Hidden_id_actividad_flujo,
                                                                                                  Session.Item("WF_RUTAWORKFLOW"),
                                                                                                  UpdateGeneral_documentos_actividad,
                                                                                                  0)
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Hidden_lista_ruta_flujo.Value = "R"
            Me.ModalPopupExtender_edition_lista_actividades_ruta.Show()

        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.updata_panel_pendiente, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_actualiza_datos_Click(sender As Object, e As EventArgs) Handles Button_actualiza_datos.Click
        Dim Refcriptman As New Classscrripjava
        Try
            If Me.Hidden_id.Value = "0" Or Me.Hidden_id.Value = "-1" Then
                Refcriptman.Showscripman_menu("Debe seleccionar un registro para actualizar ", Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtenderactualiza.Show()
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.Upadatepanel_botnoes, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonCerrarpendiente_Click(sender As Object, e As EventArgs) Handles ButtonCerrarpendiente.Click
        Me.ModalPopupExtenderactualiza.Hide()
    End Sub

    Private Sub Button_actualiza_pendiente_Click(sender As Object, e As EventArgs) Handles Button_actualiza_pendiente.Click
        Dim refclas As New ClassWorkflow
        Dim Result As String = ""
        Dim Refcriptman As New Classscrripjava
        If Hidden_id.Value = "0" Then Exit Sub
        Try
            If Me.contenidobusqueda_actualiza.Text = "" Then Exit Sub
            Dim spli() As String = Hidden_id.Value.Split("-")

            Result = refclas.Actualizar_Datos_tarea_pendiente(Me.contenidobusqueda_actualiza.Text, spli(0))
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.Updatepanael_Actualiza, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_Resultado_actualiza.Value = "YES"
                ModalPopupExtenderactualiza.Hide()
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.Updatepanael_Actualiza, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub btnCancelpagina_Click(sender As Object, e As EventArgs) Handles btnCancelpagina.Click
        Me.ModalPopupExtendermesjpagina.Hide()
    End Sub

    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        'Session.Item("OPCIONSELECIONPENDIENTE") = "VISOR EMERGENTE"
        Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value
        'Me.Labelpendient.Text = "Tareas pendiente"
        Me.Iframe_visor_externo_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
        Me.UpdatePanel_visor_externo.Update()
        Me.ModalPopupExtender_visor_externo.Show()
    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ModalPopupTexto.Hide()
    End Sub
    Private Sub ButtonReasignarTerminar_Click(sender As Object, e As EventArgs) Handles ButtonReasignarTerminar.Click
        Dim Refcriptman As New Classscrripjava
        '***********************************************
        'Ejecuta accion enviar actividad a usuario
        '***********************************************
        Try
            If Me.HiddenPROMP.Value = "0" Then Exit Sub
            If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW") = 0 Then
                'Mens.Showscripman("Usuario sin permisos para reasinar", Me.Updatecondiciona)
                Me.TextBox_login_autoriza_reasignacion_tarea.Text = ""
                Me.TextBox_pasw_autoriza_reasignacion_tarea.Text = ""
                Me.UpdatePanel_autoriza_reasignacion_tarea.Update()
                ModalPopupExtender_edition_autoriza_reasignacion_tarea.Show()
                Exit Sub
            Else
                Dim Result As String = ""
                Dim Refclas_wf As New ClassWorkflow
                Result = Refclas_wf.Pre_reasigna_tarea_usuario_desde_pendiente(Me.Page, Hidden_id.Value, 0, 0, 0, "sin autorizacion usuario permitido")
                If Result <> "YES" Then
                    Refcriptman.Showscripman(Result, Me.Updatecondiciona)
                    Exit Sub
                Else
                    Me.ModalPopupExtendermesjpagina.Hide()
                End If
            End If
        Catch ex As Exception
            Refcriptman.Showscripman(ex.Message, Me.Updatecondiciona)
        End Try
    End Sub
    Private Sub Button_autoriza_reasignacion_Click(sender As Object, e As EventArgs) Handles Button_autoriza_reasignacion.Click
        Dim refclas_gestion As New Classgestionrespuesta
        Dim id_usuario_autoriza As Integer = -1
        Dim Refcriptman As New Classscrripjava
        Dim Result As String = ""
        Try
            Result = refclas_gestion.Valida_usuario_administrador_general(Me.TextBox_login_autoriza_reasignacion_tarea.Text, Me.TextBox_pasw_autoriza_reasignacion_tarea.Text, id_usuario_autoriza, "reasigna_documento")
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_autoriza_reasignacion_tarea, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim Refclas_wf As New ClassWorkflow
            Result = Refclas_wf.Pre_reasigna_tarea_usuario_desde_pendiente(Me.Page, Hidden_id.Value, 0, 0, 0, Me.TextBox_login_autoriza_reasignacion_tarea.Text)
            If Result <> "YES" Then
                Hidden_resp_envio.Value = ""
                Updatecondiciona.Update()
                Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_autoriza_reasignacion_tarea, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resp_envio.Value = "YES"
                Updatecondiciona.Update()
                ModalPopupExtender_edition_autoriza_reasignacion_tarea.Hide()
                Me.ModalPopupExtendermesjpagina.Hide()
            End If

        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_autoriza_reasignacion_tarea, "ModalPopupExtender_mensaje_personalizado")

        End Try
    End Sub
    
    Private Sub data_grid_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid.RowCreated
        'e.Row.Cells(0).Visible = True
        e.Row.Cells(1).Visible = False
        e.Row.Cells(2).Visible = False
        e.Row.Cells(3).Visible = False
        e.Row.Cells(4).Visible = False
        e.Row.Cells(6).Visible = False
    End Sub

    Private Sub Button_activa_enviar_actividad_flujo_trabajo_Click(sender As Object, e As EventArgs) Handles Button_activa_enviar_actividad_flujo_trabajo.Click
        Dim refclas As New ClassWorkflow
        Dim mens As New Classscrripjava
        Try
            Dim result As String = ""
            Me.Hidden_result_envi_flujo.Value = ""
            result = refclas.Envia_actividad_flujo_trabajo_reasignacion_terminar_pendiente(Me.Page, _
                                                                                           HttpContext.Current.Session("Id_actividad_Workflow"), _
                                                                                           HttpContext.Current.Session("Id_Usuario_Workflow"))
            If result <> "YES" Then
                mens.Showscripman(result, UpdatePanel_enviar_actividad)
                Exit Sub
            End If
        Catch ex As Exception
            mens.Showscripman(ex.Message, UpdatePanel_enviar_actividad)
        End Try

    End Sub

    Protected Sub Button_cancelar_confirmacion_Click(sender As Object, e As EventArgs) Handles Button_cancelar_confirmacion.Click
        ModalPopupExtender_edition_envia_actividad_flujo_trabjo.Hide()
    End Sub

    Private Sub Button_detalle_enviar_actividad_flujo_trabajo_Click(sender As Object, e As EventArgs) Handles Button_detalle_enviar_actividad_flujo_trabajo.Click
        Dim clasjava As New Classscrripjava
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            Dim Result As String = ""
            Dim nombre_usuario As String = ""
            Dim cargo_usuario As String = ""
            Dim correo_electronico As String = ""
            Dim grupo As String = ""
            If Me.Hidden_id_usuario_workflow.Value <> "&nbsp;" And Me.Hidden_id_usuario_workflow.Value <> "0" Then
                Result = Refclas.Lista_detalle_usuario_workflow(Me.Hidden_id_usuario_workflow.Value, nombre_usuario, cargo_usuario, correo_electronico, grupo)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Label_nombre_usuario.Text = nombre_usuario
                    Me.Label_cargo.Text = cargo_usuario
                    Me.Label_correo.Text = correo_electronico
                    Me.Label_nombre_grupo.Text = grupo
                    Me.UpdatePanel_detalle_actividad_flujo_user.Update()
                    Me.ModalPopupExtender_edition_detalle_actividad_flujo_user.Show()
                    Exit Sub
                End If
            End If
            Dim nombre_actividad As String = ""
            Dim descripcion_actividad As String = ""
            Dim tipo_actividad As String = ""
            Dim datos_usuario_relacion_usuario As String = ""
            Dim matri_usuarios_relacionados() As String = Nothing
            If Me.Hidden_id_actividad_destino.Value <> "0" And Me.Hidden_id_actividad_destino.Value <> "&nbsp;" Then
                Result = Refclas.Lista_detalle_actividad_workflow(Me.Hidden_id_actividad_destino.Value, nombre_actividad, descripcion_actividad, tipo_actividad)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Label_nombre_actividad.Text = nombre_actividad
                    Me.Label_descripcion.Text = descripcion_actividad
                    'Me.Label_usuario_relacionados.Text = correo_electronico
                    Me.Label_tipo_actividad.Text = tipo_actividad

                End If
                Result = Refclas.Lista_usuarios_relacionados_id_actividad(Me.Hidden_id_actividad_destino.Value, matri_usuarios_relacionados)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    If matri_usuarios_relacionados Is Nothing Then
                        Me.UpdatePanel_detalle_actividad_flujo.Update()
                        Me.ModalPopupExtender_edition_detalle_actividad_flujo.Show()
                        Exit Sub
                    Else
                        For i As Integer = 0 To matri_usuarios_relacionados.Length - 1
                            If i = 0 Then
                                datos_usuario_relacion_usuario = matri_usuarios_relacionados(i)
                            Else
                                datos_usuario_relacion_usuario = datos_usuario_relacion_usuario & "-" & matri_usuarios_relacionados(i)
                            End If
                        Next
                        Me.Label_usuario_relacionados.Text = datos_usuario_relacion_usuario
                        Me.UpdatePanel_detalle_actividad_flujo.Update()
                        Me.ModalPopupExtender_edition_detalle_actividad_flujo.Show()
                        Exit Sub
                    End If
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub data_grid_actividad_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_actividad.RowCreated
        e.Row.Cells(1).Visible = False
        e.Row.Cells(3).Visible = False
    End Sub

    Private Sub Button_detalle_enviar_actividad_ruta_Click(sender As Object, e As EventArgs) Handles Button_detalle_enviar_actividad_ruta.Click
        Dim clasjava As New Classscrripjava
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            Dim Result As String
            Dim nombre_actividad As String = ""
            Dim descripcion_actividad As String = ""
            Dim tipo_actividad As String = ""
            Dim datos_usuario_relacion_usuario As String = ""
            Dim matri_usuarios_relacionados() As String = Nothing
            If Me.Hidden_id_actividad_ruta.Value <> "0" And Me.Hidden_id_actividad_ruta.Value <> "&nbsp;" Then
                Result = Refclas.Lista_detalle_actividad_workflow(Me.Hidden_id_actividad_ruta.Value, nombre_actividad, descripcion_actividad, tipo_actividad)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_lista_actividades_ruta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Label_nombre_actividad.Text = nombre_actividad
                    Me.Label_descripcion.Text = descripcion_actividad
                    'Me.Label_usuario_relacionados.Text = correo_electronico
                    Me.Label_tipo_actividad.Text = tipo_actividad

                End If
                Result = Refclas.Lista_usuarios_relacionados_id_actividad(Me.Hidden_id_actividad_ruta.Value, matri_usuarios_relacionados)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_lista_actividades_ruta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    If matri_usuarios_relacionados Is Nothing Then
                        Me.UpdatePanel_detalle_actividad_flujo.Update()
                        Me.ModalPopupExtender_edition_detalle_actividad_flujo.Show()
                        Exit Sub
                    Else
                        For i As Integer = 0 To matri_usuarios_relacionados.Length - 1
                            If i = 0 Then
                                datos_usuario_relacion_usuario = matri_usuarios_relacionados(i)
                            Else
                                datos_usuario_relacion_usuario = datos_usuario_relacion_usuario & "-" & matri_usuarios_relacionados(i)
                            End If
                        Next
                        Me.Label_usuario_relacionados.Text = datos_usuario_relacion_usuario
                        Me.UpdatePanel_detalle_actividad_flujo.Update()
                        Me.ModalPopupExtender_edition_detalle_actividad_flujo.Show()
                        Exit Sub
                    End If
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_lista_actividades_ruta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_activa_enviar_actividad_ruta_Click(sender As Object, e As EventArgs) Handles Button_activa_enviar_actividad_ruta.Click
        Dim refclas As New ClassWorkflow
        Dim mens As New Classscrripjava
        Try
            Dim result As String = ""
            '---------------------------------
            'Verifica selección tarea envío
            '---------------------------------
            If Hidden_id_actividad_ruta.Value = "" Then
                mens.Showscripman("Por favor seleccione la actividad  " & Hidden_id.Value, Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            '------------------------------------------------------------
            'Verifica respuesta radicado slección lista de actividades
            '------------------------------------------------------------
            Dim spli_id() As String = Hidden_id.Value.Split("-")
            If spli_id.Length < 1 Then
                mens.Showscripman("Imposible determinar la tarea  " & Hidden_id.Value, Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            Dim refclasgestion As New Classgestionrespuesta
            result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), spli_id(1))
            If result <> "YES" Then
                mens.Showscripman(result, Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(spli_id(1)), _
                                                                                         Estado_solicitud_aprobacion, _
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If result <> "YES" Then
                mens.Showscripman(result, Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                mens.Showscripman("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.UpdatePanel_enviar_actividad)
                Exit Sub
            End If
            Dim Refclas_actividades_disp_envio As New Class_actividades_disponibles_envio
            Dim estado_envio_correo As Integer = 0
            Dim resultado_correo As String = ""
            result = Refclas_actividades_disp_envio.Solicita_estado_envio_correo_conector_ruta(Val(Me.Hidden_id_tar_sel.Value), _
                                                                                               estado_envio_correo)
            If result <> "YES" Then
                mens.Showscripman(result, Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            result = refclas.Terminar_Tarea_Workflow_Bacth("", _
                                                           Hidden_id_actividad_ruta.Value, _
                                                           spli_id(0), _
                                                           spli_id(1), _
                                                           "", 0, 0, 0, _
                                                           estado_envio_correo, _
                                                           resultado_correo, 1, _
                                                           Val(Me.Hidden_id_tar_sel.Value), _
                                                           HttpContext.Current.Session("Id_Usuario_Workflow"), _
                                                           HttpContext.Current.Session("Id_actividad_Workflow"))
            If result <> "YES" Then
                Me.Hidden_res_evi.Value = ""
                Updatecondiciona.Update()
                mens.Showscripman(result, UpdatePanel_lista_actividades_ruta)
                Exit Sub
            Else
                Me.Hidden_res_evi.Value = "YES"
                Updatecondiciona.Update()
                UpdateGeneral_documentos_actividad.Update()
                Me.ModalPopupExtender_edition_lista_actividades_ruta.Hide()
                If resultado_correo <> "" Then
                    mens.Showscripman(resultado_correo, Me.UpdatePanel_lista_actividades_ruta)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            mens.Showscripman(ex.Message, UpdatePanel_lista_actividades_ruta)
        End Try
    End Sub

    Protected Sub Button_autoriza_reasignacion_flujo_Click(sender As Object, e As EventArgs) Handles Button_autoriza_reasignacion_flujo.Click
        '--------------------------------------------------------
        'Autoriza reasignación mediante flujo de trabajo
        '--------------------------------------------------------
        Dim refclas_gestion As New Classgestionrespuesta
        Dim id_usuario_autoriza As Integer = -1
        Dim Refcriptman As New Classscrripjava
        Dim Result As String = ""
        Try
            Result = refclas_gestion.Valida_usuario_administrador_general(Me.TextBox_login_lista_actividades_ruta_flujo.Text, Me.TextBox_pasw_lista_actividades_ruta_flujo.Text, id_usuario_autoriza, "reasigna_documento")
            If Result <> "YES" Then
                Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_lista_actividades_ruta_flujo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_wf As New ClassWorkflow
            Hidden_resp_envio_flujo.Value = ""
            Result = Refclas_wf.Reasigna_actividad_con_autorizacion_flujo_trabajo_pendiente(Me.Page, Me.TextBox_login_lista_actividades_ruta_flujo.Text, 0)
            If Result <> "YES" Then
                Hidden_resp_envio_flujo.Value = ""
                Refcriptman.Showscripman_menu(Result, Me.UpdatePanel_lista_actividades_ruta_flujo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resp_envio_flujo.Value = "YES"
                ModalPopupExtender_edition_lista_actividades_ruta_flujo.Hide()
            End If

        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_lista_actividades_ruta_flujo, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub
End Class