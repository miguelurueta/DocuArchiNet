Imports AjaxControlToolkit
Imports System.IO

Public Class WebFormConsultaRadicacion
    Inherits System.Web.UI.Page
    Private Sub Button_notificar_envio_Click(sender As Object, e As EventArgs) Handles Button_notificar_envio.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar el regitro del radicado de la lista para notificar", _
                                            Me.UpdatePanel_botones_radicacion, _
                                            "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim correo_remitente As String = ""
            Result = refclas.Retorna_correo_electronico_usuario_radicador(Session.Item("RA_ID_USUARIO"), _
                                                                          correo_remitente)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Me.Hidden_id_plantilla_radicado.Value = split(1)
            Session.Item("GA_RUTA_TEMPORAL_DESCARGA_ARCHIVO_CORREO") = HttpContext.Current.Session.Item("RA_RUTA_TEMPO_DESCARGA") & "\"
            Me.Hidden_cuenta_correo_envio.Value = correo_remitente
            Me.Hidden_correo_envio_default.Value = ""
            Me.Iframe_comparte_coreo.Attributes.Add("src", "../radicador/WebFormNotificar.aspx")
            Me.UpdatePanel_iframenotifica.Update()
            ModalPopupExtender_notifica_gestion.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim refclas As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Try
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim result As String = ""
            Dim Ref_class_workflow_rutas As New Class_worflow_rutas
            Dim Refclas_producion As New ClassGaProducionDocumental
            Dim Result_planti As String = ""
            Dim nombre_plantilla_radicado As String = ""
            Dim Id_Plantilla As Integer = 0
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            If split(0) = "PRODUCCION" Then
                Result_planti = Class_system_plantilla_radicado.Solicita_nombre_id_plantilla_radicación_interna_default(nombre_plantilla_radicado,
                                                                                                          Id_Plantilla,
                                                                                                          1)
                If Result_planti <> "YES" Then
                    Label_estado_transac.Text = Result_planti
                    Exit Sub
                End If
                Session.Item("RA_TIPO_PLANTILLA_CONSULTA") = "RADICACION ENTRANTE"
                Session.Item("RA_PLANTILLA_CONSULTA") = split(4)
                Session.Item("RA_MODULO_SELECCIONADO") = "PRODUCCION|" & Id_Plantilla.ToString & "|" & "RADICACION ENTRANTE" & "|" & "0" & "|" & nombre_plantilla_radicado
                split = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
                Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO") = Id_Plantilla
                Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO") = 1
                HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") = "PRODUCCION"
                result = refclas.Genera_Interface_consulta_radicados(Id_Plantilla, _
                                                                     "RADICACION ENTRANTE", _
                                                                     Me, _
                                                                     nombre_plantilla_radicado)
                If result <> "YES" Then
                    Label_estado_transac.Text = result
                Else
                    Label_estado_transac.Text = ""
                End If
                result = Ref_class_workflow_rutas.Solicita_nombre_ruta_por_id_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")), _
                                                                                   HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"))
                If result <> "YES" Then
                    Label_estado_transac.Text = Label_estado_transac.Text & result

                End If
            End If
            If split(0) = "CONSULTA" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    Session.Item("RA_TIPO_PLANTILLA_CONSULTA") = "RADICACION ENTRANTE"
                    Session.Item("RA_PLANTILLA_CONSULTA") = split(4)
                    Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO") = Val(split(1))
                    Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO") = 1
                    result = refclas.Genera_Interface_consulta_radicados(split(1),
                                                                         split(0),
                                                                         Me,
                                                                         split(4))
                    If result <> "YES" Then
                        Label_estado_transac.Text = result
                    Else
                        Label_estado_transac.Text = ""
                    End If

                    result = Ref_class_workflow_rutas.Solicita_nombre_ruta_por_id_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                                       HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"))
                    If result <> "YES" Then
                        Label_estado_transac.Text = Label_estado_transac.Text & result

                    End If
                End If
            End If
            If split(0) = "GESTIONRADICADOS" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    Session.Item("RA_TIPO_PLANTILLA_CONSULTA") = "RADICACION ENTRANTE"
                    Session.Item("RA_PLANTILLA_CONSULTA") = split(4)
                    Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO") = Val(split(1))
                    Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO") = 1
                    result = refclas.Genera_Interface_consulta_radicados(split(1),
                                                                         split(0),
                                                                         Me,
                                                                         split(4))
                    If result <> "YES" Then
                        Label_estado_transac.Text = result
                    Else
                        Label_estado_transac.Text = ""
                    End If
                End If
            End If
            If split(0) = "CONSULTA" Then
                If split(2) = "RADICACION SALIENTE" Then
                    Session.Item("RA_TIPO_PLANTILLA_CONSULTA") = "RADICACION SALIENTE"
                    Session.Item("RA_PLANTILLA_CONSULTA") = split(4)
                    Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO") = Val(split(1))
                    Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO") = 2
                    result = refclas.Genera_Interface_consulta_radicados(split(1),
                                                                         split(0),
                                                                         Me,
                                                                         split(4))
                    If result <> "YES" Then
                        Label_estado_transac.Text = result
                        HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Lista campos dinamicos edición : " & result & vbCrLf
                        Exit Sub
                    Else
                        Label_estado_transac.Text = ""
                    End If
                    result = Ref_class_workflow_rutas.Solicita_nombre_ruta_por_id_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                                      HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"))
                    If result <> "YES" Then
                        Label_estado_transac.Text = Label_estado_transac.Text & result

                    End If
                End If
            End If

            If Me.IsPostBack = False Then
                Dim Refclas_inicio As New ClassInicioRadicador
                result = Refclas_inicio.Crea_Dir_Temporal_ra()
                If result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & result & vbCrLf
                    Label_estado_transac.Text = result
                Else
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & result & vbCrLf
                End If
                ifmExcel_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                updatapanel_iframe.Update()
                'Dim Ref_class_inicio As New InicioWorkflow
                'result = Ref_class_inicio.Crea_Dir_Temporal_wf()
                'If result <> "YES" Then
                'Label_estado_transac.Text = Label_estado_transac.Text + result
                'End If
            End If
            If Me.panel_edita_campos_dinamicos.Visible = True Then
                result = refclas.Lista_campos_dinamicos_edicion_plantilla(Me.Page, _
                                                                          split(1), _
                                                                          split(4), _
                                                                          split(2))
                If result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Lista campos dinamicos edición : " & result & vbCrLf
                End If
          
            End If
            result = refclas.asgina_auto_complete_edicion(Me.Page)
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
        Catch ex As Exception
        End Try

    End Sub
    Private Sub Button__consulta_val_radicacion_rest_Click(sender As Object, e As EventArgs) Handles Button__consulta_val_radicacion_rest.Click
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            Hidden_resultado_consulta.Value = ""
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            If HttpContext.Current.Session.Item("RA_PERMISO_CONSULTA") = 0 Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para consultar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            result = refclas.Genera_Sql_Consulta_radicados_entrantes(Me, _
                                                                     split(1), _
                                                                     split(4), _
                                                                     "", _
                                                                     1, _
                                                                     1)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resultado_consulta.Value = "YES"
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                UpdatePanelabel_val_radicacion.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
   
    Private Sub Button_consulta_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_consulta_val_radicacion.Click
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            Hidden_resultado_consulta.Value = ""
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            If HttpContext.Current.Session.Item("RA_PERMISO_CONSULTA") = 0 Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para consultar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            result = refclas.Genera_Sql_Consulta_radicados_entrantes(Me, _
                                                                     split(1), _
                                                                     split(4), _
                                                                     "", _
                                                                     1, _
                                                                     1)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resultado_consulta.Value = "YES"
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                UpdatePanelabel_val_radicacion.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub GridView_val_radicacion_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_val_radicacion.PageIndexChanging

        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            GridView_val_radicacion.PageIndex = e.NewPageIndex
            Hidden_resultado_consulta.Value = ""
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            result = refclas.Genera_Sql_Consulta_radicados_entrantes(Me, _
                                                               split(1), _
                                                               split(4), _
                                                               "", _
                                                               3, _
                                                               1)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_conenido_grid_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                'UpdatePanelabel_val_radicacion.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_conenido_grid_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_consulta_val_radicacion_general_Click(sender As Object, e As EventArgs) Handles Button_consulta_val_radicacion_general.Click
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            If TextBox_buequeda_general.Text = "" Then
                Exit Sub
            End If
            Hidden_resultado_consulta.Value = ""
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            If HttpContext.Current.Session.Item("RA_PERMISO_CONSULTA") = 0 Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para consultar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            result = refclas.Genera_Sql_Consulta_radicados_entrantes(Me, _
                                                                     split(1), _
                                                                     split(4), _
                                                                      TextBox_buequeda_general.Text, _
                                                                      2, _
                                                                      1)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resultado_consulta.Value = "YES"
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                UpdatePanelabel_val_radicacion.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    

    Private Sub Button_lipiar_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_lipiar_val_radicacion.Click
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Try
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim result As String = ""
            result = refclas.Limpiar_campos_consulta_radicados_entrantes(split(1), "", Me, split(4))
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_botones_validacion)
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_validacion)
        End Try
    End Sub

    Private Sub Button_Reimpresion_radicado_Click(sender As Object, e As EventArgs) Handles Button_Reimpresion_radicado.Click
        '-----------------------------------------------------------------
        'Asigna los datos de impresion para se leidos por las paginas
        'Impresion por texto o impresion post
        '-----------------------------------------------------------------
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("RA_PERMISO_IMPRIMIR_GUIA") = 0 Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para reimprimir", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro del radicado de la lista para poder imprimir el rótulo de radicado", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclasconsulta As New ClassRaConsultaRadicados
            Result = refclasconsulta.Reimpresion_rotulo_radicacion(Me.Page)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_validacion)
        End Try
    End Sub

    Protected Sub Button_Editar_radicados_Click(sender As Object, e As EventArgs) Handles Button_Editar_radicados.Click

        Dim refclas As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("RA_PERMISO_EDITA_RADICADO") = 0 Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para editar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro del radicado de la lista para poder editar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim result As String = ""
            refclas.asgina_auto_complete_edicion(Me)
            result = refclas.asigna_datos_edicion_plantilla_radicado(Me.Page)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            result = refclas.asigna_datos_plantilla_campos_dinamicos_DB(Me.Page)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_listar_destinatarios_entrantes_Click(sender As Object, e As EventArgs) Handles Button_listar_destinatarios_entrantes.Click

        Dim refclas As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Try
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim selected_text As String = ""
            Dim Droplistarea As DropDownList = Nothing
            Dim Droplist As DropDownList = Nothing
            Dim updatepanel As UpdatePanel = Nothing
            Dim result As String = ""
            Dim id_empresa As Integer = -1
            Dim refclas_rad As New ClassRadicador
            result = refclas_rad.Retorna_ID_Empresa_Usuario_Radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"), _
                                                                      id_empresa)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_organigrama As Integer = -1
            Dim ref_clas_empresa As New Class_registro_organigrama
            result = ref_clas_empresa.Retorna_Id_Organigrama_activo_empresa(id_empresa, _
                                                                            id_organigrama)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If split(2) = "RADICACION ENTRANTE" Then
                    Droplist = Me.DropDownList_destinatario_entrante
                    Droplistarea = Me.DropDownList_area_destinatario_entrate
                    updatepanel = Me.UpdatePnaelcontrolesradicacion_entrante
                    If Droplist.Text <> "" And Droplist.Text <> "SELECCIONE" Then
                        selected_text = Droplist.Text
                    Else
                        selected_text = ""
                    End If

                Else
                    Droplist = Me.DropDownList_remitente_saliente
                    Droplistarea = Me.DropDownList_area_remitente_saliente
                    updatepanel = Me.UpdatePnaelcontrolesradicacion_saliente
                    If Droplist.Text <> "" And Droplist.Text <> "SELECCIONE" Then
                        selected_text = Droplist.Text
                    Else
                        selected_text = ""
                    End If

                End If
                result = refclas_rad.Retorna_Areas_Departamento_Radicacion(id_empresa, _
                                                                           Droplistarea, _
                                                                           Droplistarea.Text)
                If result <> "YES" Then
                    clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Droplistarea.Text = "TODAS LAS AREAS" Then
                    selected_text = "SELECCIONE"
                    Droplist.Text = "SELECCIONE"
                    Hidden_area_remitente_destinatario.Value = "-1"
                Else
                    Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                    If Droplistarea.Text <> "SELECCIONE" Then
                        result = ref_Class_areas_depart_radicacion.Retorna_id_area_usuario_gestion(id_organigrama, _
                                                                                                   Droplistarea.Text, _
                                                                                                   Hidden_area_remitente_destinatario.Value)
                        If result <> "YES" Then
                            clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                            Hidden_remitente_destinario_interno.Value = "-1"
                            Hidden_area_remitente_destinatario.Value = "-1"
                            Droplist.Items.Clear()
                            updatepanel_Asigana_datos_validacion_edicion.Update()
                            Exit Sub
                        End If
                    Else
                        Hidden_remitente_destinario_interno.Value = "-1"
                        Hidden_area_remitente_destinatario.Value = "-1"
                        Droplist.Items.Clear()
                        updatepanel_Asigana_datos_validacion_edicion.Update()
                        Exit Sub
                    End If
                End If
                result = refclas_rad.Lista_usuarios_gestion_internos_por_area(id_organigrama, _
                                                                              Droplistarea.Text, _
                                                                              Droplist, _
                                                                              id_empresa, _
                                                                              updatepanel, _
                                                                              selected_text)
                If result <> "YES" Then
                    clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                    'droparea.Focus()
                    Exit Sub
                End If

                If Droplist.Text <> "SELECCIONE" And Droplist.Text <> "" Then
                    Dim spli_nombre_dest() As String = Droplist.Text.ToString.Split("((")
                    result = refclas_rad.lista_id_usuario_gestion_interno(id_empresa, Trim(spli_nombre_dest(0)), _
                                                                          Hidden_remitente_destinario_interno.Value)
                    If result <> "YES" Then
                        clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                        'droparea.Focus()
                        Exit Sub
                    End If
                Else
                    Hidden_remitente_destinario_interno.Value = "-1"
                End If

            End If
            updatepanel_Asigana_datos_validacion_edicion.Update()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_listar_id_destinatario_Click(sender As Object, e As EventArgs) Handles Button_listar_id_destinatario.Click
        Dim refclas As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Try
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim selected_text As String = ""
            Dim Droplistarea As DropDownList = Nothing
            Dim Droplist As DropDownList = Nothing
            Dim updatepanel As UpdatePanel = Nothing
            Dim result As String = ""
            Dim id_empresa As Integer = -1
            Dim refclas_rad As New ClassRadicador
            Dim Ref_class_remit_dest_int As New Class_remit_dest_interno
            result = refclas_rad.Retorna_ID_Empresa_Usuario_Radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"), id_empresa)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_organigrama As Integer = -1
            Dim ref_clas_empresa As New Class_registro_organigrama
            result = ref_clas_empresa.Retorna_Id_Organigrama_activo_empresa(id_empresa, _
                                                                            id_organigrama)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If split(2) = "RADICACION ENTRANTE" Then
                    Droplist = Me.DropDownList_destinatario_entrante
                    Droplistarea = Me.DropDownList_area_destinatario_entrate
                    updatepanel = Me.UpdatePnaelcontrolesradicacion_entrante
                Else
                    Droplist = Me.DropDownList_remitente_saliente
                    Droplistarea = Me.DropDownList_area_remitente_saliente
                    updatepanel = Me.UpdatePnaelcontrolesradicacion_saliente
                    If Droplist.Text <> "" And Droplist.Text <> "SELECCIONE" Then
                        selected_text = Droplist.Text
                    Else
                        selected_text = ""
                    End If

                End If
                If Droplist.Text <> "SELECCIONE" Then
                    Dim spli_nombre_dest() As String = Droplist.Text.ToString.Split("((")
                    result = refclas_rad.lista_id_usuario_gestion_interno(id_empresa, _
                                                                          Trim(spli_nombre_dest(0)), _
                                                                          Hidden_remitente_destinario_interno.Value)
                    If result <> "YES" Then
                        clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                      
                        Exit Sub
                    End If
                    If Droplistarea.Text = "TODAS LAS AREAS" Then
                        Dim nombre_area As String = ""
                        result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(Hidden_remitente_destinario_interno.Value, _
                                                                                     Hidden_area_remitente_destinatario.Value, _
                                                                                     nombre_area)
                        If result <> "YES" Then
                            clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        End If
                        For i As Integer = 0 To Droplistarea.Items.Count - 1
                            If Droplistarea.Items(i).Text = nombre_area Then
                                Droplistarea.Text = nombre_area
                                Exit For
                            End If
                        Next
                        result = refclas_rad.Lista_usuarios_gestion_internos_por_area(id_organigrama, _
                                                                                      Droplistarea.Text, _
                                                                                      Droplist, _
                                                                                      id_empresa, _
                                                                                      updatepanel, _
                                                                                      Trim(spli_nombre_dest(0)))
                        If result <> "YES" Then
                            clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                            'droparea.Focus()
                            Exit Sub
                        End If
                    Else
                        Dim nombre_area As String = ""
                        result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(Hidden_remitente_destinario_interno.Value, _
                                                                                                    Hidden_area_remitente_destinatario.Value, _
                                                                                                    nombre_area)
                        If result <> "YES" Then
                            clasjava.Showscripman_menu(result, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        Else
                            '********************************************************
                            'Lista las areas del usuario
                            '********************************************************

                            For i As Integer = 0 To Droplistarea.Items.Count - 1
                                If Droplistarea.Items(i).Text = nombre_area Then
                                    Droplistarea.Text = nombre_area
                                    Exit For
                                End If
                            Next

                        End If
                    End If
                Else
                    Hidden_remitente_destinario_interno.Value = "-1"
                End If

            End If
            updatepanel_Asigana_datos_validacion_edicion.Update()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_gestion_remitente_entrante_Click(sender As Object, e As EventArgs) Handles Button_gestion_remitente_entrante.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRaConsultaRadicados
        Dim Clasjava As New Classscrripjava
        Try
            Dim id_escrip As Integer = -1
            Result = Refclas.retorna_id_escript_campo_plantilla_validacion(id_escrip, "REMITENTE_COR")
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePnaelcontrolesradicacion_entrante)
                Exit Sub
            End If
            If id_escrip = -1 Or id_escrip = 0 Then
                Clasjava.Showscripman("No hay plantilla relacionada para el campo", UpdatePnaelcontrolesradicacion_saliente)
                Exit Sub
            End If
            Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = id_escrip
            Me.Iframe_validacion_plantilla_.Attributes.Add("src", "../radicador/WebFormGestionPlantillasvalidacion.aspx")
            Me.UpdatePanel_validacion_plantilla.Update()
            Me.ModalPopupExtender_valiacion_plantilla.Show()
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePnaelcontrolesradicacion_entrante)
        End Try
    End Sub

    Private Sub Button_gestion_remitente_saliente_Click(sender As Object, e As EventArgs) Handles Button_gestion_remitente_saliente.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRaConsultaRadicados
        Dim Clasjava As New Classscrripjava
        Try
            Dim id_escrip As Integer = -1
            Result = Refclas.retorna_id_escript_campo_plantilla_validacion(id_escrip, "REMITENTE_COR")
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePnaelcontrolesradicacion_saliente)
                Exit Sub
            End If
            If id_escrip = -1 Or id_escrip = 0 Then
                Clasjava.Showscripman("No hay plantilla relacionada para el campo", UpdatePnaelcontrolesradicacion_saliente)
                Exit Sub
            End If
            Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = id_escrip
            Me.Iframe_validacion_plantilla_.Attributes.Add("src", "../radicador/WebFormGestionPlantillasvalidacion.aspx")
            Me.UpdatePanel_validacion_plantilla.Update()
            Me.ModalPopupExtender_valiacion_plantilla.Show()
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePnaelcontrolesradicacion_saliente)
        End Try
    End Sub

    Private Sub Button_Asigana_datos_validacion_edicion_Click(sender As Object, e As EventArgs) Handles Button_Asigana_datos_validacion_edicion.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaConsultaRadicados
        Dim Clasjava As New Classscrripjava
        Try
            Result = refclas.asigna_remitente_destinatario_interface_edicion(Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, updatepanel_Asigana_datos_validacion_edicion)
            End If
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, updatepanel_Asigana_datos_validacion_edicion)
        End Try
    End Sub

    Private Sub Button_editar_tipo_tramite_Click(sender As Object, e As EventArgs) Handles Button_editar_tipo_tramite.Click
        Dim Result As String = ""
        Dim refclas As New ClassRadicador
        Dim Clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("RA_PERMISO_EDITA_RADICADO") = 0 Then
                Clasjava.Showscripman_menu("El usuario no tiene permisos para editar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID_VAL.Value = "-1" Then
                Clasjava.Showscripman_menu("Debe seleccionar el registro del radicado de la lista para poder editar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim tipo_tramite_documento As String = ""
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim Ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = Ref_Class_plantillas_radicacion.retorna_tipo_documental_radicado(Me.hdnEmailID_VAL.Value, _
                                                                                    split(4), _
                                                                                    tipo_tramite_documento)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") = "PRODUCCION" Then
                Dim Refclas_gaproducion As New Class_tipo_doc_entrante
                Result = Refclas_gaproducion.Lista_tipos_documentales_de_radicacion_interna_item(Me.DropDownList_edita_tipo_tramite,
                                                                                                 split(1),
                                                                                                 tipo_tramite_documento)
                If Result <> "YES" Then
                    Clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
                Dim id_tramite As Integer = 0
                id_tramite = Me.DropDownList_edita_tipo_tramite.SelectedValue
                Dim id_flujo As Integer = 0
                DropDownList_flujo_tramite.Items.Clear()
                DropDownList_lista_actividades_flujo.Items.Clear()
                DropDownList_lista_usuarios_flujo.Items.Clear()
                UpdatePanel_edita_reasigna_flujo.Update()
                If id_tramite <> 0 Then
                    Result = refclas.Solicita_id_flujo_plantilla_radicado(split(4),
                                                                          Me.hdnEmailID_VAL.Value,
                                                                          id_flujo)
                    If Result <> "YES" Then
                        Clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                        Exit Sub
                    End If
                    Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_relaciones_flujo_trabajo_tramite_default(id_tramite,
                                                                                                                       id_flujo,
                                                                                                                       DropDownList_flujo_tramite)
                    If Result <> "YES" Then
                        Clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                        Exit Sub
                    End If
                    If id_flujo <> 0 Then
                        Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
                        Result = Class_wf_registro_actividaes_flujos_trabajo.Lista_actividades_workflow_flujo_drowlist(1,
                                                                                                                       id_flujo,
                                                                                                                       DropDownList_lista_actividades_flujo,
                                                                                                                       UpdatePanel_edita_reasigna_flujo)
                        If Result <> "YES" Then
                            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                        End If
                    End If
                End If
            Else
                Result = refclas.Listar_Tipos_Documentales_items_default(Me.DropDownList_edita_tipo_tramite,
                                                                         split(1),
                                                                         tipo_tramite_documento)
                If Result <> "YES" Then
                    Clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                Dim id_tramite As Integer = 0
                id_tramite = Me.DropDownList_edita_tipo_tramite.SelectedValue
                Dim id_flujo As Integer = 0
                DropDownList_flujo_tramite.Items.Clear()
                DropDownList_lista_actividades_flujo.Items.Clear()
                DropDownList_lista_usuarios_flujo.Items.Clear()
                UpdatePanel_edita_reasigna_flujo.Update()
                If id_tramite <> 0 Then
                    Result = refclas.Solicita_id_flujo_plantilla_radicado(split(4),
                                                                          Me.hdnEmailID_VAL.Value,
                                                                          id_flujo)
                    If Result <> "YES" Then
                        Clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                        Exit Sub
                    End If
                    Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
                    Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_relaciones_flujo_trabajo_tramite_default(id_tramite,
                                                                                                                       id_flujo,
                                                                                                                       DropDownList_flujo_tramite)
                    If Result <> "YES" Then
                        Clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                        Exit Sub
                    End If
                    If id_flujo <> 0 Then
                        Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
                        Result = Class_wf_registro_actividaes_flujos_trabajo.Lista_actividades_workflow_flujo_drowlist(1,
                                                                                                                       id_flujo,
                                                                                                                       DropDownList_lista_actividades_flujo,
                                                                                                                       UpdatePanel_edita_reasigna_flujo)
                        If Result <> "YES" Then
                            Clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                        End If
                    End If
                End If
            End If
            Result = refclas.Retorna_fecha_vence_radicado(Me.hdnEmailID_VAL.Value, _
                                                          split(4), _
                                                          Me.TextBox_fecha_tramite_vence.Text)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            UpdatePanel_edita_tipo_tramite.Update()
            Me.ModalPopupExtender_Panel_edita_tipo_tramite.Show()
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePanel_botones_radicacion)

        End Try
    End Sub

    Private Sub Button_actualiza_fecha_limte_respuesta_Click(sender As Object, e As EventArgs) Handles Button_actualiza_fecha_limte_respuesta.Click
        Dim ob As Object
        Dim Refclas As New ClassRadicador
        Dim refclasconsulta As New ClassRaConsultaRadicados
        Dim calsjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim numero_dias As Integer = 0
            Dim id_tramite As Integer = Me.DropDownList_edita_tipo_tramite.SelectedItem.Value
            Dim id_flujo As Integer = 0
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            DropDownList_lista_actividades_flujo.Items.Clear()
            DropDownList_lista_usuarios_flujo.Items.Clear()
            UpdatePanel_edita_reasigna_flujo.Update()
            Result = Refclas.Retorna_Dias_Vencimiento_tramite(split(1),
                                                              Me.DropDownList_edita_tipo_tramite.SelectedItem.Text,
                                                              numero_dias)
            If Result <> "YES" Then
                calsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                Me.DropDownList_edita_tipo_tramite.Focus()
                Exit Sub
            End If
            Dim nombre_plantilla As String = ""
            Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
            Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(split(1),
                                                                       nombre_plantilla)
            If Result <> "YES" Then
                calsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                Me.DropDownList_edita_tipo_tramite.Focus()
                Exit Sub
            End If
            Dim fecha_radicado As String = ""
            Result = refclasconsulta.Retorna_fecha_radicado(nombre_plantilla,
                                                            Me.hdnEmailID_VAL.Value,
                                                            fecha_radicado)
            If Result <> "YES" Then
                calsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                Me.DropDownList_edita_tipo_tramite.Focus()
                Exit Sub
            End If
            Dim dat As Date = Now.Date
            dat = Date.Parse(fecha_radicado)
            If numero_dias = 0 Then Exit Sub
            Dim refclasalmacen As New ClassAlmacenamiento
            Result = Refclas.DateAgregarLaborales(numero_dias,
                                                  dat,
                                                  ob)
            If Result <> "YES" Then
                calsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                Me.DropDownList_edita_tipo_tramite.Focus()
                Exit Sub
            Else
                Dim feha As String = ob
                Dim refclas_gestion_fechas As New ClassGestionFechas
                Result = refclas_gestion_fechas.formata_fecha_tipo_date(feha)
                If Result <> "YES" Then
                    calsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                    Me.DropDownList_edita_tipo_tramite.Focus()
                    Exit Sub
                Else
                    feha = Left(feha, 10)
                    Me.TextBox_fecha_tramite_vence.Text = feha.Replace("/", "-")
                End If
                If id_tramite <> 0 Then
                    Result = Refclas.Solicita_id_flujo_plantilla_radicado(nombre_plantilla,
                                                                          Me.hdnEmailID_VAL.Value,
                                                                          id_flujo)
                    If Result <> "YES" Then
                        calsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Dim Class_ra_relacion_tramite_flujo_wokflow As New Class_ra_relacion_tramite_flujo_wokflow
                    Result = Class_ra_relacion_tramite_flujo_wokflow.Solicita_relaciones_flujo_trabajo_tramite_default(id_tramite,
                                                                                                                       id_flujo,
                                                                                                                       DropDownList_flujo_tramite)
                    If Result <> "YES" Then
                        calsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
                    If DropDownList_flujo_tramite.SelectedValue <> "" Then
                        Result = Class_wf_registro_actividaes_flujos_trabajo.Lista_actividades_workflow_flujo_drowlist(1,
                                                                                                                       Val(DropDownList_flujo_tramite.SelectedValue),
                                                                                                                       DropDownList_lista_actividades_flujo,
                                                                                                                       UpdatePanel_edita_reasigna_flujo)
                        If Result <> "YES" Then
                            calsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                        End If
                    End If

                End If
            End If
        Catch ex As Exception
            calsjava.Showscripman(ex.Message, Me.UpdatePanel_edita_tipo_tramite)
        Finally
            Me.UpdatePanel_edita_tipo_tramite.Update()
        End Try
    End Sub
    Private Sub Button_actualiza_lista_actividades_flujo_Click(sender As Object, e As EventArgs) Handles Button_actualiza_lista_actividades_flujo.Click
        Dim calsjava As New Classscrripjava
        Try
            Dim id_flujo As Integer = Val(DropDownList_flujo_tramite.SelectedValue)
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim Result As String = ""
            Result = Class_wf_registro_actividaes_flujos_trabajo.Lista_actividades_workflow_flujo_drowlist(1,
                                                                                                           id_flujo,
                                                                                                           DropDownList_lista_actividades_flujo,
                                                                                                           UpdatePanel_edita_reasigna_flujo)
            If Result <> "YES" Then
                calsjava.Showscripman(Result, Me.UpdatePanel_boton_tipo_tramite)
            End If
        Catch ex As Exception
            calsjava.Showscripman(ex.Message, Me.UpdatePanel_boton_tipo_tramite)
        End Try
    End Sub
    Private Sub Button_actualiza_lista_usuarios_actividades_Click(sender As Object, e As EventArgs) Handles Button_actualiza_lista_usuarios_actividades.Click
        Dim calsjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim id_actividad_flujo As Integer = Val(DropDownList_lista_actividades_flujo.SelectedValue)
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Result = Class_usuario_workflow.Solicita_usuarios_relacionados_actividad_flujo(0, id_actividad_flujo,
                                                                                           DropDownList_lista_usuarios_flujo,
                                                                                           UpdatePanel_edita_reasigna_flujo)
            If Result <> "YES" Then
                calsjava.Showscripman(Result, Me.UpdatePanel_boton_tipo_tramite)
            End If
        Catch ex As Exception
            calsjava.Showscripman(ex.Message, Me.UpdatePanel_boton_tipo_tramite)
        End Try
    End Sub
    Private Sub Button_actualiza_indice_Click(sender As Object, e As EventArgs) Handles Button_actualiza_indice.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassConsultaGabinete
            Dim campos As String = ""
            Result = Refclas.Lista_campos_edicion_gabinete(Session.Item("DA_GABINETE_CONSULTA"), campos)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_actualiza)
                Exit Sub
            Else
                Hidden_campos_dinamicos_edita.Value = campos
                Result = Refclas.lista_valores_campo_edita(Session.Item("DA_GABINETE_CONSULTA"), campos, Session.Item("DA_IMAGEN"), hidden_valore_campos.Value)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.Updatepanel_actualiza)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_actualiza)
        End Try
    End Sub
    Private Sub Button_actualiza_tipo_tramite_Click(sender As Object, e As EventArgs) Handles Button_actualiza_tipo_tramite.Click
        Dim scripjava As New Classscrripjava
        Dim Result As String = ""
        Dim refclas As New ClassRaConsultaRadicados
        Try
            If Hidden_alert_respuesta.Value = "NO" Then Exit Sub
            If Session.Item("RA_PERMISO_EDITA_RADICADO") = 0 Then
                scripjava.Showscripman_menu("El usuario no tiene permisos para editar radicados ", UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If     
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar el registro ", UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Dim planti_val As New PLANTILLA_VALIDACION_CAMPOS_ESTATICOS
            Result = ref_Class_plantillas_radicacion.retorna_datos_radicacion_estructura(split(2), _
                                                                                         Me.hdnEmailID_VAL.Value, _
                                                                                         split(4), _
                                                                                         planti_val)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If planti_val.FECHALIMITERESPUESTA <> "" Then
                Dim fecha_limite_resp As String = planti_val.FECHALIMITERESPUESTA
                Dim plit() As String = Nothing
                If InStr(planti_val.FECHALIMITERESPUESTA, "/") > 0 Then
                    plit = planti_val.FECHALIMITERESPUESTA.Split("/")
                Else
                    plit = planti_val.FECHALIMITERESPUESTA.Split("-")
                End If
                If Not plit Is Nothing Then
                    fecha_limite_resp = plit(0) & "-" & plit(1) & "-" & plit(2)
                End If
            End If
            Dim estado_registro As String = "YES"
            Result = refclas.Retorna_existencia_registro_respuesta_radicado(Me.hdnEmailID_VAL.Value, _
                                                                            estado_registro)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, UpdatePanel_edita_tipo_tramite)
                Exit Sub
            End If
            Dim id_flujo_trabajo As Integer = 0
            If Me.DropDownList_flujo_tramite.Items.Count > 0 Then
                If Me.DropDownList_flujo_tramite.SelectedItem.Selected = True Then
                    id_flujo_trabajo = Me.DropDownList_flujo_tramite.SelectedItem.Value
                End If
            End If     
            Dim matri_campos() As Campos_Plantilla
            Erase matri_campos
            Dim id_actividad_workflow As Integer = 0
            Dim id_usuario_workflow As Integer = 0
            If DropDownList_lista_actividades_flujo.SelectedValue = "" Then
                id_actividad_workflow = -1
            Else
                id_actividad_workflow = Val(DropDownList_lista_actividades_flujo.SelectedValue)
            End If
            If DropDownList_lista_usuarios_flujo.SelectedValue = "" Then
                id_usuario_workflow = -1
            Else
                id_usuario_workflow = Val(DropDownList_lista_usuarios_flujo.SelectedValue)
            End If
            Result = refclas.actualiza_tipo_tramite_documento_fecha_limite(split(4),
                                                                           Me.hdnEmailID_VAL.Value,
                                                                           estado_registro,
                                                                           Me.TextBox_fecha_tramite_vence.Text,
                                                                           Me.DropDownList_edita_tipo_tramite.SelectedItem.Text,
                                                                           planti_val,
                                                                           split(1),
                                                                           split(2),
                                                                           id_actividad_workflow,
                                                                           id_usuario_workflow,
                                                                           matri_campos,
                                                                           id_flujo_trabajo)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, UpdatePanel_boton_tipo_tramite, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Dim campos As String = ""
                Dim campos_aleas As String = ""
                Dim valores As String = ""
                For i As Integer = 0 To matri_campos.Length - 1
                    If i = 0 Then
                        campos = campos & matri_campos(i).ID_CAMPO_ASPNET
                        valores = matri_campos(i).TEXTO_CAMPO_MODIFICADO
                        campos_aleas = matri_campos(i).Alias_Campo.Replace(" ", "_")
                        'campos_aleas = campos_aleas & "RE_" & matri_campos(i).Campo_Plantilla & "-" & matri_campos(i).Alias_Campo & "-" & matri_campos(i).Tipo_Campo & "-Edit"
                    Else
                        valores = valores & "|||||" & matri_campos(i).TEXTO_CAMPO_MODIFICADO
                        campos = campos & "|" & matri_campos(i).ID_CAMPO_ASPNET
                        campos_aleas = campos_aleas & "|" & matri_campos(i).Alias_Campo.Replace(" ", "_")
                        'campos_aleas = campos_aleas & "|RE_" & matri_campos(i).Campo_Plantilla & "-" & matri_campos(i).Alias_Campo & "-" & matri_campos(i).Tipo_Campo & "-Edit"
                    End If

                Next
                hidden_campos_dinamicos_aleas.Value = campos_aleas
                Hidden_campos_dinamicos_edita.Value = campos_aleas
                hidden_valore_campos.Value = valores
                Hidden_campos_dinamicos_edita.Value = "YES"
                Updatepanel_actualiza.Update()
                Me.ModalPopupExtender_Panel_edita_tipo_tramite.Hide()
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, UpdatePanel_boton_tipo_tramite)
        End Try
    End Sub

    Private Sub Button_editar_campos_dinamicos_consulta_Click(sender As Object, e As EventArgs) Handles Button_editar_campos_dinamicos_consulta.Click

        Dim refclas As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("RA_PERMISO_EDITA_RADICADO") = 0 Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para editar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro del radicado de la lista para poder editar", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim result As String = ""
            result = refclas.asigna_datos_plantilla_campos_dinamicos_DB(Me.Page)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edita_campos_dinamicos.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el radicado de la lista para desplegar el visor", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value & "|" & split(1)
            Me.Iframe_visor_externo_wf_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_visor_externo.Update()
            Me.ModalPopupExtender_visor_externo.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Private Sub Button_edita_campos_dinamicos_Click(sender As Object, e As EventArgs) Handles Button_edita_campos_dinamicos.Click
        Dim refclas As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            If Hidden_alert_respuesta.Value = "NO" Then Exit Sub
            If HttpContext.Current.Session.Item("RA_PERMISO_EDITA_RADICADO") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para editar", UpdatePanel_edita_campos_dinamicos_actualiza)
                Exit Sub
            End If
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman("Debe seleccionar el registro a editar", UpdatePanel_edita_campos_dinamicos_actualiza)
                Exit Sub
            End If
            Dim matri_campos() As Campos_Plantilla = Nothing
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            result = refclas.Actualiza_campos_dinamicos_plantilla_db(Me.Page, _
                                                                     split(1), _
                                                                     split(4), _
                                                                     matri_campos, _
                                                                     Me.hdnEmailID_VAL.Value)
            If result <> "YES" Then
                clasjava.Showscripman(result, UpdatePanel_edita_campos_dinamicos_actualiza)
                Exit Sub
            Else
                Dim campos As String = ""
                Dim campos_aleas As String = ""
                Dim valores As String = ""
                For i As Integer = 0 To matri_campos.Length - 1
                    If i = 0 Then
                        campos = campos & matri_campos(i).ID_CAMPO_ASPNET
                        valores = matri_campos(i).TEXTO_CAMPO
                        campos_aleas = matri_campos(i).Alias_Campo.Replace(" ", "_")
                        'campos_aleas = campos_aleas & "RE_" & matri_campos(i).Campo_Plantilla & "-" & matri_campos(i).Alias_Campo & "-" & matri_campos(i).Tipo_Campo & "-Edit"
                    Else
                        valores = valores & "|||||" & matri_campos(i).TEXTO_CAMPO
                        campos = campos & "|" & matri_campos(i).ID_CAMPO_ASPNET
                        campos_aleas = campos_aleas & "|" & matri_campos(i).Alias_Campo.Replace(" ", "_")
                        'campos_aleas = campos_aleas & "|RE_" & matri_campos(i).Campo_Plantilla & "-" & matri_campos(i).Alias_Campo & "-" & matri_campos(i).Tipo_Campo & "-Edit"
                    End If
                   
                Next
                hidden_campos_dinamicos_aleas.value = campos_aleas
                Hidden_campos_dinamicos_edita.Value = campos_aleas
                hidden_valore_campos.Value = valores
                Hidden_buton_seleccion_edita_dinamico.Value = "Button_edita_campos_dinamicos"
                Hidden_campos_dinamicos_edita.Value = "YES"
                Updatepanel_actualiza.Update()
                Me.ModalPopupExtender_edita_campos_dinamicos.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_edita_campos_dinamicos_actualiza)
        End Try
    End Sub

    Private Sub Button_Exportar_Radicados_Click(sender As Object, e As EventArgs) Handles Button_Exportar_Radicados.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            If Me.Hidden_colum_header.Value = "" Then
                scripjava.Showscripman_menu("No se encontraron registros de radicados en la lista para exportar ", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((20 * Rnd()) + 1))
            Dim ruta_archivo As String = HttpContext.Current.Session.Item("RA_RUTA_TEMPO_DESCARGA") + "\" + value.ToString + "test.xls"
            Result = Refclasreposte.genera_xls_paginacion(Me.GridView_val_radicacion, _
                                                          ".xls", _
                                                          ruta_archivo, _
                                                          Me.Hidden_colum_header.Value, _
                                                          "CONSULTA RADICADOS", _
                                                          Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                          HttpContext.Current.Session.Item("RA_DATO_CONSULTA_DATA_SET_CAHE"))
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Me.Hidden_ruta_archivo.Value = "../Temp_Radicacion/" & HttpContext.Current.Session.Item("RA_ID_USUARIO").ToString & "/DESCARGA/" & value.ToString + "test.xls"
                    ifmExcel_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                    updatapanel_iframe.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
        
    End Sub

    Private Sub Button_actualizar_entrantres_Click(sender As Object, e As EventArgs) Handles Button_actualizar_entrantres.Click
        Dim refclas As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try

            If HttpContext.Current.Session.Item("RA_PERMISO_EDITA_RADICADO") = 0 Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para editar", UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro a editar", UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim matri_campos() As Campos_Plantilla = Nothing
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim Resultdado_correo As String = ""
            Dim update_wf As Integer = 1
            If Me.ceked_actualiza_wf.Checked = True Then
                update_wf = 1
            Else
                update_wf = 0
            End If
            result = refclas.Actualiza_datos_interface_radicacion(split(4),
                                                                  Me.hdnEmailID_VAL.Value,
                                                                  Me.Page,
                                                                  split(1),
                                                                  split(2),
                                                                  update_wf,
                                                                  matri_campos,
                                                                  Resultdado_correo)
            If result <> "YES" Then
                Hidden_resultado_campo_estatico.Value = "FALSE"
                clasjava.Showscripman_menu(result, UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Dim campos As String = ""
                Dim campos_aleas As String = ""
                Dim valores As String = ""
                For i As Integer = 0 To matri_campos.Length - 1
                    If i = 0 Then
                        campos = campos & matri_campos(i).ID_CAMPO_ASPNET
                        valores = matri_campos(i).TEXTO_CAMPO_MODIFICADO
                        campos_aleas = matri_campos(i).Alias_Campo.Replace(" ", "_")
                        'campos_aleas = campos_aleas & "RE_" & matri_campos(i).Campo_Plantilla & "-" & matri_campos(i).Alias_Campo & "-" & matri_campos(i).Tipo_Campo & "-Edit"
                    Else
                        valores = valores & "|||||" & matri_campos(i).TEXTO_CAMPO_MODIFICADO
                        campos = campos & "|" & matri_campos(i).ID_CAMPO_ASPNET
                        campos_aleas = campos_aleas & "|" & matri_campos(i).Alias_Campo.Replace(" ", "_")
                        'campos_aleas = campos_aleas & "|RE_" & matri_campos(i).Campo_Plantilla & "-" & matri_campos(i).Alias_Campo & "-" & matri_campos(i).Tipo_Campo & "-Edit"
                    End If

                Next
                If Resultdado_correo <> "" Then
                    clasjava.Showscripman_menu(Resultdado_correo, UpdatePanel_edit_boton, "ModalPopupExtender_mensaje_personalizado")
                End If
                hidden_campos_dinamicos_aleas.Value = campos_aleas
                Hidden_campos_dinamicos_edita.Value = campos_aleas
                Hidden_campos_dinamicos_edita.Value = campos_aleas
                hidden_valore_campos.Value = valores
                Hidden_resultado_campo_estatico.Value = "TRUE"
                Hidden_campos_dinamicos_edita.Value = "YES"
                Updatepanel_actualiza.Update()
                Me.ModalPopupExtender_editar_radicacion_entrante.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_edit_boton)
        End Try
    End Sub

    Private Sub Button_actualiza_salientes_Click(sender As Object, e As EventArgs) Handles Button_actualiza_salientes.Click
        Dim refclas As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            If Hidden_alert_respuesta.Value = "NO" Then Exit Sub
            If HttpContext.Current.Session.Item("RA_PERMISO_EDITA_RADICADO") = 0 Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para editar", Me.UpdatePanel_boton_edita_saliente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro a editar", Me.UpdatePanel_boton_edita_saliente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim matri_campos() As Campos_Plantilla = Nothing
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            result = refclas.Actualiza_datos_interface_radicacion_saliente(split(4), _
                                                                           Me.hdnEmailID_VAL.Value, _
                                                                           Me.Page, _
                                                                           split(1), _
                                                                           split(2), _
                                                                           matri_campos)
            If result <> "YES" Then
                Hidden_resultado_campo_estatico.Value = "FALSE"
                clasjava.Showscripman_menu(result, Me.UpdatePanel_boton_edita_saliente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Dim campos As String = ""
                Dim campos_aleas As String = ""
                Dim valores As String = ""
                For i As Integer = 0 To matri_campos.Length - 1
                    If i = 0 Then
                        campos = campos & matri_campos(i).ID_CAMPO_ASPNET
                        valores = matri_campos(i).TEXTO_CAMPO_MODIFICADO
                        campos_aleas = matri_campos(i).Alias_Campo.Replace(" ", "_")

                    Else
                        valores = valores & "|||||" & matri_campos(i).TEXTO_CAMPO_MODIFICADO
                        campos = campos & "|" & matri_campos(i).ID_CAMPO_ASPNET
                        campos_aleas = campos_aleas & "|" & matri_campos(i).Alias_Campo.Replace(" ", "_")

                    End If

                Next
                hidden_campos_dinamicos_aleas.Value = campos_aleas
                Hidden_campos_dinamicos_edita.Value = campos_aleas
                Hidden_campos_dinamicos_edita.Value = campos_aleas
                hidden_valore_campos.Value = valores
                Hidden_resultado_campo_estatico.Value = "TRUE"
                Hidden_campos_dinamicos_edita.Value = "YES"
                Updatepanel_actualiza.Update()
                Me.ModalPopupExtender_editar_radicacion_saliente.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePnaelcontrolesradicacion_entrante)
        End Try
    End Sub

    Protected Sub Button_Log_respuesta_Click(sender As Object, e As EventArgs) Handles Button_Log_respuesta.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro del radicado para deplegar el log de transacciones ", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_respuesta_radicado As Integer = -1
            Dim Refclas As New Classgestionrespuesta
            Dim Result As String = ""
            Result = Refclas.Reorna_id_respuesta_radicado(Me.hdnEmailID_VAL.Value, id_respuesta_radicado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_respuesta_radicado = -1 Then
                clasjava.Showscripman_menu("El tipo de radicado no requiere de una respuesta, no se realizaron transacciones para mostrar", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value & "|" & id_plantilla
            Session.Item("PU_TRAZABILIDAD") = id_respuesta_radicado
            Me.Iframe_log_transacciones_.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
            Me.UpdatePanel_transacciones.Update()
            Me.ModalPopupExtender_transacciones.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Private Sub Button_detalle_radicado_Click(sender As Object, e As EventArgs) Handles Button_detalle_radicado.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el radicado de la lista para desplegar el detalle", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_respuesta_radicado As Integer = -1
            Dim Refclas As New Classgestionrespuesta
            Dim Result As String = ""
            Result = Refclas.Reorna_id_respuesta_radicado(Me.hdnEmailID_VAL.Value, id_respuesta_radicado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_respuesta_radicado = -1 Then
                clasjava.Showscripman_menu("El tipo de radicado no es radicado con registro de respuesta, no hay detalles para mostrar", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value & "|" & id_plantilla
            Session.Item("PU_TRAZABILIDAD") = Me.hdnEmailID_VAL.Value
            Me.Iframe_log_transacciones_.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
            Me.UpdatePanel_transacciones.Update()
            Me.ModalPopupExtender_transacciones.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_botones_radicacion)
            Exit Sub
        End Try
    End Sub

    Private Sub Button_log_Click(sender As Object, e As EventArgs) Handles Button_log.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el radicado de la lista para desplegar el log de transacciones", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_respuesta_radicado As Integer = -1
            Dim Refclas As New Classgestionrespuesta
            Dim Result As String = ""
            Session.Item("PU_TRAZABILIDAD") = Me.hdnEmailID_VAL.Value
            Me.Iframe_log_transacciones_.Attributes("SRC") = "../Gestion/WebFormLogRadicado.aspx"
            Me.UpdatePanel_transacciones.Update()
            Me.ModalPopupExtender_transacciones.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Protected Sub Button_certificado_radicado_Click(sender As Object, e As EventArgs) Handles Button_certificado_radicado.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassWorkflowReportes
        Dim scripjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar el radicado para descargar el detalle", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ruta_archivo As String = ""
            Dim archivo_server As String = ""
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim Reclas_radicado As New ClassRadicador
            If split(0) = "CONSULTA" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    Result = Reclas_radicado.Genera_archivo_detalle_radicado(Me.hdnEmailID_VAL.Value, split(4), split(2), ruta_archivo, archivo_server)
                    If Result <> "YES" Then
                        scripjava.Showscripman_menu(Result, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        Hidden_ruta_archivo.Value = archivo_server
                        ifmExcel_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                        updatapanel_iframe.Update()
                    End If                 
                End If
            End If
           
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_tool_activa_lista_documentos_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_lista_documentos.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_estados_modulo_rad As New Class_ra_rad_estados_modulo_radicacion
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar el radicado", UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Ref_class_estados_modulo_rad.Interface_documentos_relacionados_radicados(Me.Page,
                                                                                            1,
                                                                                            "GridView_list_documento_relacion",
                                                                                            1,
                                                                                            "IframeDitaliza_",
                                                                                            "../workflow/WebFormEscan.aspx",
                                                                                            1,
                                                                                            1,
                                                                                            Me.hdnEmailID_VAL.Value)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_admon_documentos.Show()
            End If

        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub GridView_list_documento_relacion_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_list_documento_relacion.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
            e.Row.Cells(5).Visible = False
        Catch ex As Exception
        End Try
    End Sub
    Private Sub Button_tool_visualiza_documento_Click(sender As Object, e As EventArgs) Handles Button_tool_visualiza_documento.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_result_boton_tool.Value = ""
            Dim Result As String = ""
            Dim Refclass As New ClassDaGabinete
            Result = Refclass.Prevent_visualiza_documento_seleccion_envio_radicado(hiden_seleccion_documento.Value, 0,
                                                                                 Me.IframeVisor_,
                                                                                 Me.UpdatePanelIframevisor)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Me.Hidden_result_boton_tool.Value = ""
            Else
                Me.Hidden_result_boton_tool.Value = "YES"
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_tool_activa_sube_documento_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Session.Item("RA_RADICADO_CONSULTA") = hdnEmailID_VAL.Value
            If Session.Item("RA_RADICADO_CONSULTA") = "-1" Or Session.Item("RA_RADICADO_CONSULTA") = "" Then
                clasjava.Showscripman_menu("Debe seleccionar el radicado", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
                                                                             Session.Item("DG_TIPO_TRAMITE"), _
                                                                             Session.Item("DG_ID_CONFIG_DIGITALIZACION"), _
                                                                             0)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"), _
                                                                                                                            Session.Item("DG_TIPO_TRAMITE"), _
                                                                                                                            "", _
                                                                                                                            Me.DropDownList_adjunta_documento, _
                                                                                                                            Me.Update_actualiza_adjunta_documento, _
                                                                                                                            estado_resultado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Session.Item("WF_TIPO_ADJUNTA") = "CONSULTA_RADICADO"
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_guardar_desicion_Click(sender As Object, e As EventArgs) Handles Button_guardar_desicion.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAlmacenamiento
            Me.Hidden_result_load.Value = ""
            Dim Ref_class As New Class_estados_modulo_radicacion
            Dim stru_ As stru_registro_estado = Nothing
            If Me.DropDownList_adjunta_documento.SelectedValue = "" Then
                Session.Item("DG_LISTA_CHEQUEO") = "-1"
            Else
                Session.Item("DG_LISTA_CHEQUEO") = Me.DropDownList_adjunta_documento.SelectedValue
            End If
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                CLAS.Showscripman_menu(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim evalua_flujo_ruta As Integer = 0
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") <> 0 Then
                evalua_flujo_ruta = 1
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "ESCANER" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                Dim Nombre_Ruta_Workflow As String = ""
                Dim Refclas_dagabinete As New ClassDaGabinete
                Dim ID_DOCUMENTO As Integer = 0
                Dim TIPO_DOCUMENTO As Integer = 0
                Dim datos_image As stru_datos_image_lista = Nothing
                Result = Refclas_dagabinete.Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado("", _
                                                                                                             ID_DOCUMENTO, _
                                                                                                             TIPO_DOCUMENTO, _
                                                                                                             datos_image, _
                                                                                                             Session.Item("DG_TIPODIGITALIZACION"), _
                                                                                                             HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                                                             HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"), _
                                                                                                             Me.hdnEmailID_VAL.Value, _
                                                                                                             evalua_flujo_ruta, _
                                                                                                             1)
                If Result <> "YES" Then
                    CLAS.Showscripman_menu(Result, UpdatePanel_descarga, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.Hidden_result_load.Value = "YES"
                Me.Hidden_date_row.Value = datos_image.nombre_gabinete & "|" & datos_image.id_imagen & "|" & datos_image.radicado & "|" & datos_image.tipodocumental & "|" & datos_image.notipodocumento & "|" & datos_image.extension
                Me.ModalPopupExtender_sube_documento_adjunto.Hide()
            End If
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePanel_descarga)
        End Try
    End Sub
    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
            Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
            If Session.Item("WF_TIPO_ADJUNTA") = "ESCANER" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
                Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "VISOR" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
                Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            End If
        Catch ex As Exception
            Session.Item("WF_ERROR_RESPUESTA") = ex.Message
        End Try
    End Sub

    Private Sub ButtonAlmacenar_Click(sender As Object, e As EventArgs) Handles ButtonAlmacenar.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAlmacenamiento
            Me.Hidden_result_load_.Value = ""
            Dim Ref_class As New Class_estados_modulo_radicacion
            Dim evalua_flujo_ruta As Integer = 0
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") <> 0 Then
                evalua_flujo_ruta = 1
            End If
            Dim Nombre_Ruta_Workflow As String = ""
            Dim Refclas_dagabinete As New ClassDaGabinete
            Dim ID_DOCUMENTO As Integer = 0
            Dim TIPO_DOCUMENTO As Integer = 0
            Dim datos_image As stru_datos_image_lista = Nothing
            Result = Refclas_dagabinete.Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado("",
                                                                                                         ID_DOCUMENTO,
                                                                                                         TIPO_DOCUMENTO,
                                                                                                         datos_image,
                                                                                                         Session.Item("DG_TIPODIGITALIZACION"),
                                                                                                         HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                                         HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                         Me.hdnEmailID_VAL.Value,
                                                                                                         evalua_flujo_ruta,
                                                                                                         0, 1)
            If Result <> "YES" Then
                CLAS.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_result_load_.Value = "YES"
            Me.Hidden_date_row_.Value = datos_image.nombre_gabinete & "|" & datos_image.id_imagen & "|" & datos_image.radicado & "|" & datos_image.tipodocumental & "|" & datos_image.notipodocumento & "|" & datos_image.extension

        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_elimina_documento_Click(sender As Object, e As EventArgs) Handles Button_tool_elimina_documento.Click
        'Dim clasjava As New Classscrripjava
        'Try
        '    Dim Result As String = ""
        '    Me.Hidden_result_boton_tool.Value = ""
        '    Dim Refclass As New Class_estados_modulo_radicacion
        '    If Me.HiddenPROMP.Value = "1" Then
        '        Exit Sub
        '    End If
        '    Dim estado_actualizacion_imagen_ruta As String = "YES"
        '    'Result = Refclass.Eliminar_documento_relacionado_radicado(Me.hiden_seleccion_documento.Value,
        '    '                                                          HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
        '    '                                                          1,
        '    '                                                          Session.Item("MASTER_ELIMINA_GABINETE_WORKFLOW"),
        '    '                                                          estado_actualizacion_imagen_ruta)
        '    If Result <> "YES" Then
        '        Me.Hidden_result_boton_tool.Value = ""
        '        clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        '    Else
        '        Me.Hidden_result_boton_tool.Value = "YES"
        '        If estado_actualizacion_imagen_ruta <> "YES" Then
        '            clasjava.Showscripman_menu("Se elimino el documento, pero no se pudo actualizar la imagen en la ruta worklfow error : " & _
        '                                       estado_actualizacion_imagen_ruta, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        '        End If
        '    End If
        'Catch ex As Exception
        '    clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        'End Try
    End Sub
    Private Sub Button_tool_activa_cambia_tipologia_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_cambia_tipologia.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
                                                                             Session.Item("DG_TIPO_TRAMITE"), _
                                                                             Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"), _
                                                                                                                   Session.Item("DG_TIPO_TRAMITE"), _
                                                                                                                   "", _
                                                                                                                   Me.DropDownList_tipologia_documental, _
                                                                                                                   Me.Update_actualiza_tipologia_documental, _
                                                                                                                   estado_resultado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_actualiza_tipologia_documental.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_actualiza_tipologia_documental_Click(sender As Object, e As EventArgs) Handles Button_actualiza_tipologia_documental.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_resulta_botno_tipologia_documental.Value = ""
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflowDigitalizacion
            Dim split() As String = Me.hiden_seleccion_documento.Value.ToString.Split("|")
            Dim valor_campo As String = ""
            Result = Refclas.Actualiza_tipo_documento_lista_chequeo(split(1), _
                                                                    Val(Me.DropDownList_tipologia_documental.SelectedValue), _
                                                                    split(0), _
                                                                    Me.DropDownList_tipologia_documental.SelectedItem.Text, _
                                                                    Session.Item("DG_ID_CONFIG_DIGITALIZACION"), _
                                                                    split(2), _
                                                                    valor_campo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipologia_documental, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If valor_campo = "" Then
                    valor_campo = "Documento"
                End If
                Me.Hidden_resulta_botno_tipologia_documental.Value = valor_campo
                Me.ModalPopupExtender_edition_actualiza_tipologia_documental.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tipologia_documental, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_tool_activa_detalle_radicado_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_detalle_radicado.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_estados_modulo_rad As New Class_estados_modulo_radicacion
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar el radicado", UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_class_detalle_plantilla_rad As New Class_ra_detalle_plantilla_radicado
            Result = ref_class_detalle_plantilla_rad.Genera_interface_detalle_radicado(Me.hdnEmailID_VAL.Value, _
                                                                                       Me.Page)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_detalle_radicado.Show()
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub



    Private Sub Button_Trazabilidad_Click(sender As Object, e As EventArgs) Handles Button_Trazabilidad.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro del radicado de la lista para desplegar la trazabilidad", UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value & "|" & id_plantilla
            'Me.Label12.Text = "Trazabilidad radicado"
            Session.Item("PU_TRAZABILIDAD") = Me.hdnEmailID_VAL.Value
            Me.Iframe_trazabilidad_.Attributes("SRC") = "../workflow/WebFormTrazabilidadWorkflow.aspx"
            Me.UpdatePanel_trazabilidad.Update()
            Me.ModalPopupExtender_trazabilidad.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub


End Class