Imports System.IO

Public Class WebForm_interface_consulta_historico
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Result As String = ""
        If Me.IsPostBack = False Then
            Dim campo_radicado As String = ""
            Dim Ref_class As New Class_configuracion_listado_ruta
            Result = Ref_class.SolicitaNombreCampoRadicadoRuta(Session.Item("Id_Ruta_Workflow"),
                                                               campo_radicado)
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
                Exit Sub
            End If
            Dim Refclas_config_list_ruta As New Class_configuracion_listado_ruta
            Result = Refclas_config_list_ruta.Solicita_campos_lista_tramite(Session.Item("Id_Ruta_Workflow"), _
                                                                            HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & Result
                Exit Sub
            End If
            Session.Item("SortExpression_compartido_hi") = campo_radicado
        End If
    End Sub
    Private Sub data_grid_listado_solicitudes_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_listado_solicitudes.Sorting
        Dim clasjava As New Classscrripjava
        Try
            Dim Sortdir As String = ""
            Dim Result As String = ""
            Dim Refclas As New ClassGredview
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Por tramitar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "En tramite"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Tramitado"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Solicitud por aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Solicitud aprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Solicitud desaprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "8" Then
                selecion_name = "Solicitud archivada"
            End If
            If Me.Hidden_lik_service_boton.Value = "9" Then
                selecion_name = "Solicitud anulada"
            End If
            If Me.Hidden_lik_service_boton.Value = "10" Then
                selecion_name = "Tramitado archivado"
            End If
            Session.Item("SortExpression_compartido_hi") = e.SortExpression
            If Session.Item("SortDirection_compartido_hi") = "DESC" Then
                Session.Item("SortDirection_compartido_hi") = "ASC"
            Else
                Session.Item("SortDirection_compartido_hi") = "DESC"
            End If
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Result = reflcas_respuesta.Consulta_tramites_historico(Session.Item("Id_Usuario_Workflow"), _
                                                     Session.Item("Id_Ruta_Workflow"), _
                                                     Session.Item("Id_Grupo_Workflow"), _
                                                     Session.Item("WF_ID_ACTIVIDAD"), _
                                                     HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI"), _
                                                     Session.Item("WF_RUTAWORKFLOW"), _
                                                     Me.data_grid_listado_solicitudes, _
                                                     HiddenEmailconsulta, _
                                                     Me.Label_titulo_listado_solicitudes, _
                                                     Me.hdnEmailID, _
                                                     UpdateGeneral, _
                                                     selecion_name, _
                                                     Me.UpdatePanel_title, _
                                                     HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_HI"), _
                                                     HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_HI"), _
                                                      Session.Item("SortExpression_compartido_hi"), _
                                                      Session.Item("SortDirection_compartido_hi"), _
                                                      Me.Hidden_content)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.hdnEmailID.Value = "-1"
                Me.UpdateGeneral.Update()
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try


    End Sub
    Private Sub data_grid_listado_solicitudes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_solicitudes.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_listado_solicitudes.PageIndex = e.NewPageIndex
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Por tramitar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "En tramite"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Tramitado"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Solicitud por aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Solicitud aprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Solicitud desaprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "8" Then
                selecion_name = "Solicitud archivada"
            End If
            If Me.Hidden_lik_service_boton.Value = "9" Then
                selecion_name = "Solicitud anulada"
            End If
            If Me.Hidden_lik_service_boton.Value = "10" Then
                selecion_name = "Tramitado archivado"
            End If
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Dim Result As String = ""
            Result = reflcas_respuesta.Consulta_tramites_historico(Session.Item("Id_Usuario_Workflow"), _
                                                     Session.Item("Id_Ruta_Workflow"), _
                                                     Session.Item("Id_Grupo_Workflow"), _
                                                     Session.Item("WF_ID_ACTIVIDAD"), _
                                                     HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI"), _
                                                     Session.Item("WF_RUTAWORKFLOW"), _
                                                     Me.data_grid_listado_solicitudes, _
                                                     HiddenEmailconsulta, _
                                                     Me.Label_titulo_listado_solicitudes, _
                                                     Me.hdnEmailID, _
                                                     UpdateGeneral, _
                                                     selecion_name, _
                                                     Me.UpdatePanel_title, _
                                                     HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_HI"), _
                                                     HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_HI"), _
                                                      Session.Item("SortExpression_compartido_hi"), _
                                                      Session.Item("SortDirection_compartido_hi"), _
                                                      Me.Hidden_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdateGeneral)
            Else
                Me.hdnEmailID.Value = "-1"
                Me.UpdateGeneral.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub

    Private Sub ImageButton_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_buscar.Click
        Dim clasjava As New Classscrripjava
        Try

            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Por tramitar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "En tramite"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Tramitado"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Solicitud por aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Solicitud aprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Solicitud desaprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "8" Then
                selecion_name = "Solicitud archivada"
            End If
            If Me.Hidden_lik_service_boton.Value = "9" Then
                selecion_name = "Solicitud anulada"
            End If
            If Me.Hidden_lik_service_boton.Value = "10" Then
                selecion_name = "Tramitado archivado"
            End If
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Dim Result As String = ""
            Result = reflcas_respuesta.Consulta_tramites_historico(Session.Item("Id_Usuario_Workflow"), _
                                                     Session.Item("Id_Ruta_Workflow"), _
                                                     Session.Item("Id_Grupo_Workflow"), _
                                                     Session.Item("WF_ID_ACTIVIDAD"), _
                                                     HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI"), _
                                                     Session.Item("WF_RUTAWORKFLOW"), _
                                                     Me.data_grid_listado_solicitudes, _
                                                     HiddenEmailconsulta, _
                                                     Me.Label_titulo_listado_solicitudes, _
                                                     Me.hdnEmailID, _
                                                     UpdateGeneral, _
                                                     selecion_name, _
                                                     Me.UpdatePanel_title, _
                                                     2, _
                                                     Me.auto_complex.Text, _
                                                     Session.Item("SortExpression_compartido_hi"), _
                                                     Session.Item("SortDirection_compartido_hi"), _
                                                     Me.Hidden_content)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_lik_service_boton_Click(sender As Object, e As EventArgs) Handles Button_lik_service_boton.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Por tramitar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "En tramite"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Tramitado"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Solicitud por aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Solicitud aprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Solicitud desaprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "8" Then
                selecion_name = "Solicitud archivada"
            End If
            If Me.Hidden_lik_service_boton.Value = "9" Then
                selecion_name = "Solicitud anulada"
            End If
            If Me.Hidden_lik_service_boton.Value = "10" Then
                selecion_name = "Tramitado archivado"
            End If
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Dim Result As String = ""
            Result = reflcas_respuesta.Consulta_tramites_historico(Session.Item("Id_Usuario_Workflow"), _
                                                      Session.Item("Id_Ruta_Workflow"), _
                                                      Session.Item("Id_Grupo_Workflow"), _
                                                      Session.Item("WF_ID_ACTIVIDAD"), _
                                                      HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI"), _
                                                      Session.Item("WF_RUTAWORKFLOW"), _
                                                      Me.data_grid_listado_solicitudes, _
                                                      HiddenEmailconsulta, _
                                                      Me.Label_titulo_listado_solicitudes, _
                                                      Me.hdnEmailID, _
                                                      UpdateGeneral, _
                                                      selecion_name, _
                                                      Me.UpdatePanel_title, _
                                                      HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_HI"), _
                                                      HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_HI"), _
                                                      Session.Item("SortExpression_compartido_hi"), _
                                                      Session.Item("SortDirection_compartido_hi"), _
                                                      Me.Hidden_content)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_menu_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
        Try
            'e.Row.Cells(0).Visible = False
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
        Catch ex As Exception
        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_DataBound(sender As Object, e As EventArgs) Handles data_grid_listado_solicitudes.DataBound
        Try
            'Select Case sender.SortDirection
            '    Case SortDirection.Ascending
            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black

            '    Case SortDirection.Descending
            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black

            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black
            'End Select
        Catch ex As Exception
        End Try

    End Sub
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        'Session.Item("OPCIONSELECION") = "VISOR EMERGENTE"
        Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.hdnEmailID.Value
        'Me.Labelpendient.Text = "Tareas pendiente"
        Me.Iframe_visor_externo_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
        Me.UpdatePanel_visor_externo.Update()
        Me.ModalPopupExtender_visor_externo.Show()
    End Sub
    Private Sub Button_me_active_men_dive_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Ref_class As New Class_gestion_correspondencia
            Dim Result As String = ""
            'HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Me.hdnEmailID.Value
            Result = Ref_class.Seleccion_menu_tramite(Me.Hidden_menu_var_event_dive.Value, _
                                                      Page, _
                                                      Me.hdnEmailID.Value)
            If Result <> "YES" Then
                Refcriptman.Showscripman(Result, Me.UpdatePanel_menu_var_event)
                Exit Sub
            End If
        Catch ex As Exception
            Refcriptman.Showscripman(ex.Message, Me.UpdatePanel_menu_var_event)
        End Try
    End Sub

    Private Sub Button_traza_solic_Click(sender As Object, e As EventArgs) Handles Button_traza_solic.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Ref_class As New Class_gestion_correspondencia
            Dim Result As String = ""
            Result = Ref_class.Seleccion_menu_tramite("G-TDW", _
                                                      Page, _
                                                      Me.hdnEmailID.Value)
            If Result <> "YES" Then
                Refcriptman.Showscripman(Result, Me.update_botonoes_opciones_solicitud_general)
                Exit Sub
            End If
        Catch ex As Exception
            Refcriptman.Showscripman(ex.Message, Me.update_botonoes_opciones_solicitud_general)
        End Try
    End Sub

    Private Sub Button_deta_solic_Click(sender As Object, e As EventArgs) Handles Button_deta_solic.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Ref_class As New Class_gestion_correspondencia
            Dim Result As String = ""
            Result = Ref_class.Seleccion_menu_tramite("D-RT", _
                                                      Page, _
                                                      Me.hdnEmailID.Value)
            If Result <> "YES" Then
                Refcriptman.Showscripman(Result, Me.update_botonoes_opciones_solicitud_general)
                Exit Sub
            End If
        Catch ex As Exception
            Refcriptman.Showscripman(ex.Message, Me.update_botonoes_opciones_solicitud_general)
        End Try
    End Sub

    Private Sub Button_lo_solic_Click(sender As Object, e As EventArgs) Handles Button_lo_solic.Click
        Dim Refcriptman As New Classscrripjava
        Try
            Dim Ref_class As New Class_gestion_correspondencia
            Dim Result As String = ""
            Result = Ref_class.Seleccion_menu_tramite("T-DT-H", _
                                                      Page, _
                                                      Me.hdnEmailID.Value)
            If Result <> "YES" Then
                Refcriptman.Showscripman(Result, Me.update_botonoes_opciones_solicitud_general)
                Exit Sub
            End If
        Catch ex As Exception
            Refcriptman.Showscripman(ex.Message, Me.update_botonoes_opciones_solicitud_general)
        End Try
    End Sub

    Protected Sub ImageButton_filter_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_filter.Click
        Try
            Me.ModalPopupExtender_edition_filtro_historico.Show()
        Catch ex As Exception

        End Try
    End Sub

   

    Protected Sub Button_consultar_Click(sender As Object, e As EventArgs) Handles Button_consultar.Click

        Dim clasjava As New Classscrripjava
        Try

            Dim selecion_name As String = ""
            If Me.Hidden_lik_service_boton.Value = "1" Then
                selecion_name = "Todos"
            End If
            If Me.Hidden_lik_service_boton.Value = "2" Then
                selecion_name = "Por tramitar"
            End If
            If Me.Hidden_lik_service_boton.Value = "3" Then
                selecion_name = "En tramite"
            End If
            If Me.Hidden_lik_service_boton.Value = "4" Then
                selecion_name = "Tramitado"
            End If
            If Me.Hidden_lik_service_boton.Value = "5" Then
                selecion_name = "Solicitud por aprobación"
            End If
            If Me.Hidden_lik_service_boton.Value = "6" Then
                selecion_name = "Solicitud aprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "7" Then
                selecion_name = "Solicitud desaprobada"
            End If
            If Me.Hidden_lik_service_boton.Value = "8" Then
                selecion_name = "Solicitud archivada"
            End If
            If Me.Hidden_lik_service_boton.Value = "9" Then
                selecion_name = "Solicitud anulada"
            End If
            If Me.Hidden_lik_service_boton.Value = "10" Then
                selecion_name = "Tramitado archivado"
            End If
            Dim Valor_consulta As String = ""
            Dim reflcas_respuesta As New Class_Lista_tramites_por_responder
            Dim Result As String = ""
            Result = reflcas_respuesta.Solicita_parametro_consulta_filtro(Me.TextBox_fecha_ini_asigna.Text, _
                                                                          Me.TextBox_fecha_fin_asigna.Text, _
                                                                          Me.TextBox_fecha_ini_final_tramite.Text, _
                                                                          Me.TextBox_fecha_fin_final_tramite.Text, _
                                                                          Valor_consulta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_filtro_historico, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Valor_consulta = "" Then
                clasjava.Showscripman_menu("Debe seleccionar algún criterio de fechas de asignacion o finalización", UpdatePanel_filtro_historico, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = reflcas_respuesta.Consulta_tramites_historico(Session.Item("Id_Usuario_Workflow"), _
                                                     Session.Item("Id_Ruta_Workflow"), _
                                                     Session.Item("Id_Grupo_Workflow"), _
                                                     Session.Item("WF_ID_ACTIVIDAD"), _
                                                     HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI"), _
                                                     Session.Item("WF_RUTAWORKFLOW"), _
                                                     Me.data_grid_listado_solicitudes, _
                                                     HiddenEmailconsulta, _
                                                     Me.Label_titulo_listado_solicitudes, _
                                                     Me.hdnEmailID, _
                                                     UpdateGeneral, _
                                                     selecion_name, _
                                                     Me.UpdatePanel_title, _
                                                     3, _
                                                     Valor_consulta, _
                                                     Session.Item("SortExpression_compartido_hi"), _
                                                     Session.Item("SortDirection_compartido_hi"), _
                                                     Me.Hidden_content)
            If Result <> "YES" Then
                clasjava.ShowScripmanRespuesta(Result, UpdatePanel_filtro_historico, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_filtro_historico.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_filtro_historico)
        End Try

    End Sub

    Protected Sub ImageButton_guarda_lista_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_guarda_lista.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            Dim dat As Date
            dat = Now
            'Refclasreposte.ExportToExcel(Me.GridView_val_radicacion)
            If Me.Hidden_colum_header.Value = "" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman("No hay resultados para exportar", Me.UpdatePanel_busqueda)
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((100 * Rnd()) + 1))
            Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + "/reportes/" + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + "/")
            If Directory.Exists(ruta_create) = False Then
                Directory.CreateDirectory(ruta_create)
            End If
            Dim Ref As New ClassReportesRadicado
            Dim Ref_greview As GridView
            Dim nombre_reporte As String = "CONSULTA HISTORICO TRAMITE"
            Dim ruta_archivo As String = ruta_create + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + value.ToString + "test.xls"
            Result = Refclasreposte.genera_xls_paginacion(Me.data_grid_listado_solicitudes, _
                                                          ".xls", _
                                                          ruta_archivo, _
                                                          Hidden_colum_header.Value, _
                                                          nombre_reporte, _
                                                          Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                          HttpContext.Current.Session.Item("dat_gred_cahce_hi"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.UpdatePanel_busqueda)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + "/reportes/" + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + "/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & value.ToString + "test.xls"
                    Me.ifmExcel_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                    Me.updatapanel_iframe.Update()
                    Me.update_botonoes_opciones_solicitud_general.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_busqueda)
        End Try
    End Sub

    
End Class
