
Imports AjaxControlToolkit
Imports System.IO

Public Class WebFormRadicacionEntrante
    Inherits System.Web.UI.Page
    Private Sub WebFormRadicacion_PreRenderComplete(sender As Object, e As EventArgs) Handles Me.PreRenderComplete
        ' Dim script As [String] = "$(document).ready(function(){$('#" + bu.ClientID & "').autocomplete();});"
        ' ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxEditNombreDestRem.ClientID), script, True)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Me.IsPostBack = False And AjaxFileUpload_dowload.IsInFileUploadPostBack = False Then
                HttpContext.Current.Session.Item("dat_gred_cahce") = vbObject
                HttpContext.Current.Session.Item("dat_gred_cahce_restore") = vbObject
                '**********************************************
                'Crea directori temporal workflow
                '**********************************************
                Dim Refclas As New ClassInicioRadicador
                Dim Result = Refclas.Crea_Dir_Temporal_ra()
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & Result & vbCrLf
                    Label_estado_transac.Text = Result
                Else
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & Result & vbCrLf
                End If

            End If
            If Session.Item("RA_MODULO_SELECCIONADO") <> "" Then
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim Refclas As New ClassRadicador
            Dim Result As String = ""
            '------------------------------------------------------
            'Detecta la seleccion del modulo de radicacion  
            '------------------------------------------------------
            Dim class_ra_consulta_radicado As New ClassRaConsultaRadicados
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO") = Val(split(1))
                    Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO") = 1
                    If Me.IsPostBack = False Then
                        Result = Refclas.Genera_Interface_Radicacion_Entrante(split(1), _
                                                                              split(0), _
                                                                              Me, _
                                                                              split(4))
                        If Result <> "YES" Then
                            Label_estado_transac.Text = Result
                        Else
                            Label_estado_transac.Text = ""
                        End If
                    End If
                    Dim Ref_class_inicioworkflow As New InicioWorkflow
                    If Me.IsPostBack = False Then
                        Result = Ref_class_inicioworkflow.Inicializacion_modulo_workflow_radicado(Me.Page)
                        If Result <> "YES" Then
                            Label_estado_transac.Text = Result
                        End If
                    End If
                End If
                If split(2) = "RADICACION SALIENTE" Then
                    Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO") = Val(split(1))
                    Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO") = 2
                    If Me.IsPostBack = False Then
                            Result = Refclas.Genera_Interface_Radicacion_Saliente(split(1), _
                                                                                  split(0), _
                                                                                  Me, _
                                                                                  split(4))
                        If Result <> "YES" Then
                            Label_estado_transac.Text = Result
                        Else
                            Label_estado_transac.Text = ""
                        End If
                    End If
                    Dim Ref_class_inicioworkflow As New InicioWorkflow
                    If Me.IsPostBack = False Then
                        Result = Ref_class_inicioworkflow.Inicializacion_modulo_workflow_radicado(Me.Page)
                        If Result <> "YES" Then
                            Label_estado_transac.Text = Result
                        End If
                    End If
                End If

            End If
            End If
        Catch ex As Exception
            Label_estado_transac.Text = "Incinsistencia general " & ex.Message
        End Try
    End Sub
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el radicado de la lista para desplegar el visor", UpdatePanel_visor_externo_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value & "|" & split(1)
            Me.Iframe_visor_externo_wf_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_visor_externo.Update()
            Me.ModalPopupExtender_visor_externo.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_visor_externo_boton, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub
    Private Sub UpdatePnaelcontrolesradicacion_Load(sender As Object, e As EventArgs) Handles UpdatePnaelcontrolesradicacion.Load
        Dim clasjava As New Classscrripjava
        Dim update As UpdatePanel = sender.page.findcontrol("UpdatePnaelcontrolesradicacion")
        Try
            '*********************************************************************************
            'Carga interface radicacion entrante por demanda de actualizacion del update panel
            'UpdatePnaelcontrolesradicacion
            '*********************************************************************************
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim refclas As New ClassRadicador
            Dim result As String = ""
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    If Me.IsPostBack = True Then
                        result = refclas.Genera_Interface_Radicacion_Entrante(split(1), _
                                                                              split(0), _
                                                                              Me, _
                                                                              split(4))
                        If result <> "YES" Then
                            'clasjava.Showscripman(result, Me.UpdatePnaelcontrolesradicacion)
                            clasjava.Showscripman_menu(result, Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        End If
                    End If
                End If
            End If
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION SALIENTE" Then
                    If Me.IsPostBack = True Then
                        result = refclas.Genera_Interface_Radicacion_Saliente(split(1), _
                                                                              split(0), _
                                                                              Me, _
                                                                              split(4))
                        If result <> "YES" Then
                            clasjava.Showscripman_menu(result, Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        End If
                    End If
                End If
            End If
            '**********************************************************************************
            'Llena el destinatario interno con base a la seleccion funcion estrategica
            'llenardestinario java escript activa botn llenardestinatario
            '**********************************************************************************
            Dim hide As Object = sender.page.FindControl("Hiddenareagestion")
            If hide Is Nothing Then
                'clasjava.Showscripman("Imposible encontrar el control Hiddenareagestion", Me.UpdatePnaelcontrolesradicacion)
                clasjava.Showscripman_menu("Imposible encontrar el control Hiddenareagestion", Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim droparea As DropDownList = sender.page.FindControl("Area_Destinatario_Cor")
            Dim Droplist As DropDownList = sender.page.FindControl("Destinatario_Cor")
            If update Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control UpdatePnaelcontrolesradicacion", Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Not hide Is Nothing And hide.value <> "" Then
                Dim id_empresa As Integer = -1
                result = refclas.Retorna_ID_Empresa_Usuario_Radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"), id_empresa)
                If result <> "YES" Then
                    If Not update Is Nothing Then
                        'clasjava.Showscripman(result, update)
                        clasjava.Showscripman_menu(result, Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                        droparea.Focus()
                        Exit Sub
                    End If
                Else
                    Dim id_organigrama As Integer = -1
                    Dim ref_clas_empresa As New Class_registro_organigrama
                    result = ref_clas_empresa.Retorna_Id_Organigrama_activo_empresa(id_empresa, _
                                                                                    id_organigrama)
                    If result <> "YES" Then
                        'clasjava.Showscripman(result, update)
                        clasjava.Showscripman_menu(result, Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                        droparea.Focus()
                        Exit Sub
                    Else

                        If Not Droplist Is Nothing Then
                            result = refclas.Lista_usuarios_gestion_internos_por_area(id_organigrama, hide.Value, Droplist, id_empresa)
                            If result <> "YES" Then
                                If Not update Is Nothing Then
                                    'clasjava.Showscripman(result, update)
                                    'droparea.Focus()
                                    Exit Sub
                                End If
                            End If
                        End If
                    End If
                    droparea.Focus()
                End If
            End If

        Catch ex As Exception
            'clasjava.Showscripman(ex.Message, update)
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Buttoncacerrar_Click(sender As Object, e As EventArgs) Handles Buttoncacerrar.Click
        HttpContext.Current.Session("RA_DATO_VALIDACION") = ""
    End Sub
    Private Sub Buttonllenarciudad_Click(sender As Object, e As EventArgs) Handles Buttonllenarciudad.Click
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Try
            Dim Droplist As DropDownList = Me.FindControl("MUNICIPIO")
            If Droplist Is Nothing Then
                'Refclascript.Showscripman("Imposible encontrar droplist dinamico municipo", Me.UpdatePanelContenido)
                clasjava.Showscripman_menu("Imposible encontrar droplist dinamico municipo", Me.UpdatePanelContenido, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.Hiddenseleciondepartamento.Value = "" Or Me.Hiddenseleciondepartamento.Value = "SELECCIONE" Then
                Exit Sub
            End If
            Dim Refclas As New ClassRadicador
            Droplist.Items.Clear()
            Result = Refclas.lista_Municipios_Departamentos(Droplist, Me.Hiddenseleciondepartamento.Value)
            If Result <> "YES" Then
                'Refclascript.Showscripman(Result, Me.UpdatePanelContenido)
                clasjava.Showscripman_menu(Result, Me.UpdatePanelContenido, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Droplist.Focus()
        Catch ex As Exception
            'Refclascript.Showscripman(ex.Message, Me.UpdatePanelContenido)
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelContenido, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Buttonllenardepartamento_Click(sender As Object, e As EventArgs) Handles Buttonllenardepartamento.Click
        Dim clasjava As New Classscrripjava
        Dim Result As String = ""
        Try
            If data_grid.Rows.Count Then
                ScriptManager.RegisterStartupScript(Me.UpdatePanelContenido, MyBase.Page.[GetType](), "xd", "xd();", True)
            End If
            Dim Droplist As DropDownList = Me.FindControl("DEPARTEMENTO")
            If Droplist Is Nothing Then
                'Refclascript.Showscripman("Imposible encontrar droplist dinamico DEPARTEMENTO", Me.UpdatePanelContenido)
                clasjava.Showscripman_menu("Imposible encontrar droplist dinamico DEPARTEMENTO", Me.UpdatePanelContenido, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.Hiddenselecionpais.Value = "" Or Me.Hiddenselecionpais.Value = "SELECCIONE" Then
                Exit Sub
            End If
            Dim Refclas As New ClassRadicador
            Result = Refclas.Lista_Departamentos_Paises(Droplist, Me.Hiddenselecionpais.Value, Me.UpdatePanelContenido)
            If Result <> "YES" Then
                'Refclascript.Showscripman(Result, Me.UpdatePanelContenido)
                clasjava.Showscripman_menu(Result, Me.UpdatePanelContenido, "ModalPopupExtender_mensaje_personalizado")
                'MsgBox(Result, MsgBoxStyle.Critical)
                Exit Sub
            End If
            Droplist.Focus()
        Catch ex As Exception
            'Refclascript.Showscripman(ex.Message, Me.UpdatePanelContenido)
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelContenido, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub WebFormRadicacionEntrante_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        'Dim script As [String] = "$(document).ready(function(){$('#" + data_grid.ClientID & "').Scrollable3();});"
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid.ClientID), script, True)
        'If data_grid.Rows.Count Then
        '    ScriptManager.RegisterStartupScript(Me.UpdateGeneral, MyBase.Page.[GetType](), "xd", "xd();", True)
        'End If
        Dim cs As ClientScriptManager = Page.ClientScript
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), "xd2")) Then
            'ScriptManager.RegisterClientScriptBlock(Me.UpdatePanelContenido, MyBase.Page.[GetType](), "xd2", "xd2();", True)
            'ScriptManager.RegisterClientScriptBlock(Me.data_grid, MyBase.Page.[GetType](), "xd2", "xd2();", True)
        End If
        'actualiza_valor_seleccion_hideemaild()

        Dim scr As [String] = "$(document).ready(function () {$().cligred();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim stru As String = """" & "GridView_val_radicacion" & """" & "," & """" & "hdnEmailID_VAL" & """"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), "xd2")) Then
            'ScriptManager.RegisterStartupScript(Me.GridView_val_radicacion, MyBase.Page.[GetType](), "xdlimpiar", "xdlimpiar(" & stru & ");", True)
        End If
        'Dim comillas As String = Char.ConvertFromUtf32(34)
        'Dim script As [String] = "$(document).ready(function(){$('#" + data_grid_auxiliar_lista.ClientID & "').Scrollable({ScrollHeight: 310,IsInUpdatePanel:true});});"
        'If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID))) Then
        '    'ScriptManager.RegisterClientScriptBlock(UpdatePanel_auxiliar_destinatarios_internos_popup, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID), script, True)
        'End If
        ' Dim script As [String] = "$(document).ready(function(){$('#" + data_grid_auxiliar_lista.ClientID & "').gridviewScroll({width:1200,height: 200});});"
        'Dim script As [String] = "$(document).ready(function(){$('#" + data_grid_auxiliar_lista.ClientID & "').fixedHeaderTable({footer: true,cloneHeadToFoot: true, altClass: 'odd', autoShow: true });});"
        'If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID))) Then
        'ScriptManager.RegisterClientScriptBlock(data_grid_auxiliar_lista, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID), script, True)

        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), "Key", "MakeStaticHeader(" + data_grid_auxiliar_lista.ClientID + ", 400, 950 , 40 ,true);", True)
        'End If
        'scr = "$(document).ready(function () {$().gridviewScroll();});"
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)

    End Sub

    Private Sub data_grid_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim refclas As New ClassRadicador

            Me.hdnEmailID.Value = "-1"
            'ojo
            'ScriptManager.RegisterStartupScript(Me.UpdateGeneral, MyBase.Page.[GetType](), "xd", "xd();", True)
            Result = refclas.Genera_consulta_validacion_post(Me)
            If Result <> "YES" Then
                'clas.Showscripman(Result, Me.UpdateGeneral)
                clasjava.Showscripman_menu("Imposible encontrar droplist dinamico DEPARTEMENTO", Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub asignar_Click(sender As Object, e As EventArgs) Handles asignar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassRadicador
            Dim Result As String = ""
            Me.ModalPopupExtender_valiacion_plantilla.Hide()
            Result = Refclas.Asigna_datos_validacion_interface_radicacion(Me)
            If Result <> "YES" Then
                'clas.Showscripman(Result, Me.UpdatePanelbotones)
                clasjava.Showscripman_menu(Result, Me.UpdatePanelbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
                Me.Editar_.Focus()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelbotones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
   

    Private Sub Editar__Click(sender As Object, e As EventArgs) Handles Editar_.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassRadicador
            Dim Result As String = ""
            If Me.hdnEmailID.Value = "-1" Or Me.hdnEmailID.Value = "0" Then
                'clas.Showscripman("Debe seleccionar el registro para editar", Me.UpdatePanelbotones)
                clasjava.Showscripman_menu("Debe seleccionar el registro para editar", Me.UpdatePanelbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
                Me.Editar_.Focus()
            End If
            Result = Refclas.Asignar_Datos_Plantilla_Para_Edicion(Me)
            If Result <> "YES" Then
                'clas.Showscripman(Result, Me.UpdatePanelbotones)
                clasjava.Showscripman_menu(Result, Me.UpdatePanelbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
                Me.Editar_.Focus()
            End If
            Me.Hiddenestadoedicion.Value = "1"
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelbotones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Eliminar_Click(sender As Object, e As EventArgs) Handles Eliminar.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.HiddenPROMP.Value = "1" Then
                Exit Sub
            End If

            Dim Refclas As New ClassRadicador
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("RA_VALIDACION_ELIMINAR") = "0" Then
                'scriptjava.Showscripman("El usuario no tiene permisos para eliminar", Me.UpdatePanelbotones)
                clasjava.Showscripman_menu("El usuario no tiene permisos para eliminar ", Me.UpdatePanelbotones, "ModalPopupExtender_mensaje_personalizado")
                sender.focus()
                Exit Sub
            End If
            Result = Refclas.Eliminar_Registro_Plantilla_Validacion(Me.Page)
            If Result <> "YES" Then
                'scriptjava.Showscripman(Result, Me.UpdatePanelbotones)
                clasjava.Showscripman_menu(Result, Me.UpdatePanelbotones, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelbotones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Buttonllenardestinatario_Click(sender As Object, e As EventArgs) Handles Buttonllenardestinatario.Click
        'Dim clas As New Classscrripjava
        'Dim Refclas As New ClassRadicador
        'Dim Result As String = ""

        'Result = Refclas.Asigna_datos_validacion_interface_radicac(Me)
        'If Result <> "YES" Then
        '    clas.Showscripman(Result, Me.UpdatePanelbotones)
        '    Exit Sub
        '    Me.Editar_.Focus()
        'End If

    End Sub


    Private Sub Buttontramitevence_Click(sender As Object, e As EventArgs) Handles Buttontramitevence.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim RefclasRadicador As New ClassRadicador
            Result = RefclasRadicador.Seleccion_tipo_tramite(Page)
            If Result <> "YES" Then
                'clasjava.Showscripman(Result, Me.UpdatePanelbotonesradicado)
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelbotonesradicado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Buttonradicar_entrante_Click(sender As Object, e As EventArgs) Handles Buttonradicar_entrante.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim refclas As New ClassRadicador
            Dim result As String = ""
            Dim pag As Page = sender.page
            Dim UpdatePanelradciacionbotones As UpdatePanel = pag.FindControl("UpdatePanelradciacionbotones")
            Dim UpdatePanel_imp_impresion As UpdatePanel = pag.FindControl("UpdatePanel_imp_impresion")
            Dim Hiddendatoradicacion As Object = pag.FindControl("Hiddendatoradicacion")
            Dim hide_ruta As Object = pag.FindControl("Hiddenruta")
            If hide_ruta Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control Hiddenruta", Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Hiddendatoradicacion Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control Hiddendatoradicacion", Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If UpdatePanel_imp_impresion Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control UpdatePanel_imp_impresion", Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim upiframe As UpdatePanel = pag.FindControl("UpdatePaneliframe")
            If upiframe Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control UpdatePaneliframe", Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim upiframe_post As UpdatePanel = pag.FindControl("UpdatePaneliframe_post")
            If upiframe_post Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control UpdatePaneliframe_post", Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Modal As ModalPopupExtender = pag.FindControl("ModalPopupExtenderimpre")
            Dim Modal_post As ModalPopupExtender = pag.FindControl("ModalPopupExtenderimpre_post")
            Dim consecutivo_radicado As String = ""
            Dim estado_error_asignado As String = "YES"
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    If Me.IsPostBack = True Then
                        result = refclas.Registra_Radicacion_entrante(split(1), _
                                                                      split(0), _
                                                                      Me, _
                                                                      split(4), _
                                                                      consecutivo_radicado, _
                                                                      estado_error_asignado)
                        If result <> "YES" Then
                            clasjava.Showscripman_menu(result, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        End If
                        If estado_error_asignado <> "YES" Then
                            clasjava.Showscripman_menu(estado_error_asignado, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                            'Exit Sub
                        End If
                    End If
                End If
                If split(2) = "RADICACION SALIENTE" Then
                    If Me.IsPostBack = True Then
                        result = refclas.Registra_Radicacion_saliente(split(1), _
                                                                      split(0), _
                                                                      Me, _
                                                                      split(4), _
                                                                      consecutivo_radicado, _
                                                                      estado_error_asignado)
                        If result <> "YES" Then
                            clasjava.Showscripman_menu(result, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        End If
                        If estado_error_asignado <> "YES" Then
                            clasjava.Showscripman_menu(estado_error_asignado, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                            'Exit Sub
                        End If
                    End If
                End If
            End If
            '-----------------------------------------------------------------
            'Asigna los datos de impresion para se leidos por las paginas
            'Impresion por texto o impresion post
            '-----------------------------------------------------------------
            HttpContext.Current.Session("RA_DATO_IMPRESION") = consecutivo_radicado
            If HttpContext.Current.Session("RA_TIPO_IMPRESION") = "2" Then
                Dim valor As String = consecutivo_radicado
                Dim spltival() As String = valor.Split("¬")
                Dim Ruta_Sesion As String = ""
                Ruta_Sesion = HttpContext.Current.Session("RA_RUTA_TEMPO_IMPRESION").ToString()
                'Dim gfg = HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO")
                HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO") = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + HttpContext.Current.Session.Item("RA_ID_USUARIO").ToString & "/IMPRESION/" & "RA" & spltival(2) & spltival(3) & spltival(4) & ".pdf"
                Dim rutafinal As String = Ruta_Sesion & "\" & "RA" & spltival(2) & spltival(3) & spltival(4) & ".pdf"
                HttpContext.Current.Session("RA_RUTA_IMPRESION_FINAL") = rutafinal
                hide_ruta.value = rutafinal
                '************************************************************
                'Genera rotulo de impresion radicacionHidden_ruta_archivo
                '************************************************************
                Dim pesta As String = ""
                pesta = HttpContext.Current.Session("RA_RUTA_IMPRESION_FINAL")
                Dim refplan() As Plantilla_Impresion
                Erase refplan
                result = ""
                result = refclas.Genera_Rotulo_Impresion(valor, _
                                                         rutafinal, _
                                                         refplan, _
                                                         HttpContext.Current.Session("RA_TIPO_IMPRESION").ToString())
                If result <> "YES" Then
                    clasjava.Showscripman_menu(result, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                    'Exit Sub
                End If
                upiframe.Update()
                'updat.Update()
                Modal.Show()
            End If
            If HttpContext.Current.Session("RA_TIPO_IMPRESION") = "1" Then
                Dim valor As String = consecutivo_radicado
                Dim spltival() As String = valor.Split("¬")
                Dim Ruta_Sesion As String = ""
                Ruta_Sesion = HttpContext.Current.Session("RA_RUTA_TEMPO_IMPRESION").ToString()
                'HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO") = HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO") & "RA" & spltival(2) & spltival(3) & spltival(4) & ".txt"
                HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO") = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + HttpContext.Current.Session.Item("RA_ID_USUARIO").ToString & "/IMPRESION/" & "RA" & spltival(2) & spltival(3) & spltival(4) & ".txt"
                Dim rutafinal As String = Ruta_Sesion & "\" & "RA" & spltival(2) & spltival(3) & spltival(4) & ".txt"
                HttpContext.Current.Session("RA_RUTA_IMPRESION_FINAL") = rutafinal
                hide_ruta.value = rutafinal
                '************************************************************
                'Genera rotulo de impresion radicacion
                '************************************************************
                Dim pesta As String = ""
                pesta = HttpContext.Current.Session("RA_RUTA_IMPRESION_FINAL")
                Dim refplan() As Plantilla_Impresion
                Erase refplan
                result = ""
                result = refclas.Genera_Rotulo_Impresion(valor, _
                                                         rutafinal, _
                                                         refplan, _
                                                         HttpContext.Current.Session("RA_TIPO_IMPRESION").ToString())
                If result <> "YES" Then
                    clasjava.Showscripman_menu(result, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")

                End If
                upiframe.Update()
                Modal.Show()
            End If
            result = refclas.Limpiar_campos_Plantilla_radicacion_entrante(Me.Page)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub


    Private Sub UpdatePanelContenido_val_radicacion_Load(sender As Object, e As EventArgs) Handles UpdatePanelContenido_val_radicacion.Load
        '*********************************************************************************
        'Carga interface radicacion entrante por demanda de actualizacion del update panel
        'UpdatePnaelcontrolesradicacion
        '*********************************************************************************
        Dim refclas As New ClassRadicador
        Dim refclas_consulta As New ClassRaConsultaRadicados
        Dim clasjava As New Classscrripjava
        Try
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim result As String = ""
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    If Me.IsPostBack = True Then
                        result = refclas_consulta.Genera_Interface_consulta_radicados(split(1), _
                                                                                         split(0), _
                                                                                         Me, _
                                                                                         split(4))
                        If result <> "YES" Then
                            'clasjava.Showscripman(result, Me.UpdatePanelContenido_val_radicacion)
                            clasjava.Showscripman_menu(result, Me.UpdatePanelContenido_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        Else
                            UpdatePanelContenido_val_radicacion.Update()
                        End If
                    End If
                End If
            End If
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION SALIENTE" Then
                    If Me.IsPostBack = True Then
                        result = refclas_consulta.Genera_Interface_consulta_radicados(split(1), _
                                                                                         split(0), _
                                                                                         Me, _
                                                                                         split(4))
                        If result <> "YES" Then
                            clasjava.Showscripman_menu(result, Me.UpdatePanelContenido_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        Else
                            UpdatePanelContenido_val_radicacion.Update()
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelContenido_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Buttonvalidar_radciado_Click(sender As Object, e As EventArgs) Handles Buttonvalidar_radciado.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Try
            Me.GridView_val_radicacion.DataSource = Nothing
            Me.GridView_val_radicacion.DataBind()
            titulo_label_val_radicacion.Text = "Se encontro " & 0 & " registro(s)  "
            Me.hdnEmailID_VAL.Value = "-1"
            Me.UpdatePanelabel_val_radicacion.Update()
            Me.UpdatePanel_conenido_grid_val_radicacion.Update()
            Me.ModalPopupExtender_Val_Radicado.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_modo_radicado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_consulta_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_consulta_val_radicacion.Click

        Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            result = refclas.Genera_Sql_Consulta_radicados_entrantes(Me, _
                                                                     split(1), _
                                                                     split(4), _
                                                                     "", _
                                                                     1, _
                                                                     2)
            If result <> "YES" Then
                Me.Hidden_resultado_consulta_previa.Value = ""
                clasjava.Showscripman_menu(result, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_resultado_consulta_previa.Value = "YES"
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
                UpdatePanelabel_val_radicacion.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")

        End Try
    End Sub

    Private Sub GridView_val_radicacion_PageIndexChanged(sender As Object, e As EventArgs) Handles GridView_val_radicacion.PageIndexChanged
        'Try
        '    Dim refclas As New ClassRadicador
        '    Dim result As String = ""
        '    ClassRadicador.RestoreSelection(GridView_val_radicacion, result)
        'Catch ex As Exception

        'End Try

    End Sub

    Private Sub GridView_val_radicacion_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_val_radicacion.PageIndexChanging
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            GridView_val_radicacion.PageIndex = e.NewPageIndex
            'Hidden_resultado_consulta.Value = ""
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            result = refclas.Genera_Sql_Consulta_radicados_entrantes(Me, _
                                                               split(1), _
                                                               split(4), _
                                                               "", _
                                                               3, _
                                                               2)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanel_conenido_grid_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.hdnEmailID_VAL.Value = "-1"
                HttpContext.Current.Session.Item("ProdSelection") = Nothing
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_conenido_grid_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridView_val_radicacion_PreRender(sender As Object, e As EventArgs) Handles GridView_val_radicacion.PreRender

    End Sub

    Private Sub Button_lipiar_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_lipiar_val_radicacion.Click
        Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Try
            result = refclas.Limpiar_campos_consulta_radicados_entrantes(split(1), "", Me, split(4))
            If result <> "YES" Then
                'clasjava.Showscripman(result, Me.UpdatePanel_botones_val_radicacion)
                clasjava.Showscripman_menu(result, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            'clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_val_radicacion)
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Private Sub Button_Asignar_radicado_relacionado_Click(sender As Object, e As EventArgs) Handles Button_Asignar_radicado_relacionado.Click
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Dim Reflcas As New ClassRadicador
        Try
            Dim remplaza As Integer = 0
            If Me.CheckBox_val_remplaza.Checked = True Then
                remplaza = 0
            Else
                remplaza = 1
            End If
            Result = Reflcas.Asigna_valores_seleccionados_gridview_vieestate(Me.GridView_val_radicacion, _
                                                                             Me.Dropdowlis_sel_val_radciacion, _
                                                                             Me.UpdatePanel_modo_radicado, _
                                                                             remplaza, _
                                                                             Me.CheckBox_relacionado_radicado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelabel_buton_asignacion_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If remplaza = 0 Then
                Hidden_resultado_asignacion_radicado.Value = "YES"
                Me.ModalPopupExtender_Val_Radicado.Hide()
            End If
            Me.Hidden_selecion_radicado.Value = ""
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelabel_buton_asignacion_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End Try
    End Sub

    Private Sub Button_Eliminar_Rel_Radicados_Click(sender As Object, e As EventArgs) Handles Button_Eliminar_Rel_Radicados.Click
        Try
            If Me.Dropdowlis_sel_val_radciacion.Items.Count = 0 Then Exit Sub
            Dim idex As ListItem = Me.Dropdowlis_sel_val_radciacion.SelectedItem
            Me.Dropdowlis_sel_val_radciacion.Items.Remove(idex)

        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button_Asignar_nuevo_radicado_Click(sender As Object, e As EventArgs) Handles Button_Asignar_nuevo_radicado.Click
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Dim Reflcas As New ClassRadicador
        Try
            Me.Hiddenrelacionvalidacion.Value = "-1"
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    Result = Reflcas.Asigna_copia_datos_interface_nuevo_radicado(Me, _
                                                                                 split(1), _
                                                                                 split(4))
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdatePanelabel_buton_asignacion_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        Me.ModalPopupExtender_Val_Radicado.Hide()
                    End If
                Else
                    Result = Reflcas.Asigna_copia_datos_interface_nuevo_radicado_saliente(Me, _
                                                                                          split(1), _
                                                                                          split(4))
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdatePanelabel_buton_asignacion_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        Hidden_resultado_asignacion_radicado.Value = "YES"
                        Me.ModalPopupExtender_Val_Radicado.Hide()
                    End If
                End If
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelabel_buton_asignacion_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Buttonlimpiar_entrante_Click(sender As Object, e As EventArgs) Handles Buttonlimpiar_entrante.Click
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Try

            Dim result As String = ""
            result = refclas.Limpiar_campos_Plantilla_radicacion_entrante(Me.Page)
            If result <> "YES" Then
                clasjava.Showscripman_menu(result, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub



    Private Sub Button_Edit_Expediente_Click(sender As Object, e As EventArgs) Handles Button_Edit_Expediente.Click
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Dim Refclas As New ClassAdmonEmpresa
        Try
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                clasjava.Showscripman_menu("No se ecuetra ningún usuario de gestión relacionado ", Me.UpdatePanel_modo_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 3
            Me.Iframe_expdiente_popup_.Attributes.Add("src", "../gestion/WebFormGaGestionExpediente.aspx")
            Me.UpdatePanel_expdiente_popup.Update()
            Me.ModalPopupExtende_expdiente_popup.Show()
        Catch ex As Exception
            'clasjava.Showscripman(ex.Message, Me.UpdatePanel_modo_radicado)
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_modo_radicado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_Asignar_relacionado_expediente_Click(sender As Object, e As EventArgs) Handles Button_Asignar_relacionado_expediente.Click
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Dim Reflcas As New ClassRadicador
        Try
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Result = Reflcas.Asigna_datos_expediente_radicado(Me, _
                                                              split(1), _
                                                              split(4))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelabel_buton_asignacion_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resultado_asignacion_radicado.Value = "YES"
                Me.ModalPopupExtender_Val_Radicado.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelabel_buton_asignacion_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub



    Private Sub Button_abrir_auxiliar_destinatarios_internos_popup_Click(sender As Object, e As EventArgs) Handles Button_abrir_auxiliar_destinatarios_internos_popup.Click
        Me.ModalPopupExtender_auxiliar_destinatarios_internos_popup.Show()
        'Dim comillas As String = Char.ConvertFromUtf32(34)
        'Dim script As [String] = "$(document).ready(function(){$('#" + data_grid_auxiliar_lista.ClientID & "').MakeStaticHeader({ScrollHeight: 310,IsInUpdatePanel:true});});"
        ''If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID))) Then
        'ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID), script, True)
    End Sub

    Private Sub ModalPopupExtender_auxiliar_destinatarios_internos_popup_PreRender(sender As Object, e As EventArgs) Handles ModalPopupExtender_auxiliar_destinatarios_internos_popup.PreRender

    End Sub

    Private Sub Button_asignar_auxiliar_destinatarios_internos_popup_Click(sender As Object, e As EventArgs) Handles Button_asignar_auxiliar_destinatarios_internos_popup.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Try
            If Me.Hidden_auxiliar_id.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un usuario de la lista ", Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.seleciona_usuario_interno_dest_gestion_drowp_list(Val(Me.Hidden_auxiliar_id.Value), Me)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_auxiliar_destinatarios_internos_popup.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_cierra_popup_expediente_Click(sender As Object, e As EventArgs) Handles Button_cierra_popup_expediente.Click
        Me.Check_anexo_radicado.Checked = True
        Me.check_nuevo_radicado.Checked = False
        Me.CheckBox_relacionado_radicado.Checked = False
        Me.ModalPopupExtende_expdiente_popup.Hide()
    End Sub

    Protected Sub Button_muestra_dias_horas_habil_Click(sender As Object, e As EventArgs) Handles Button_muestra_dias_horas_habil.Click
        Dim refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Result = refclas.Lista_dias_horas_habiles_radicacion(Val(split(1)), Me.TreeViewArchivo_r_u_e, Me.UpdatePanelViewArchivo_r_u_e)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
            Else
                ModalPopupExtende_dias_horas_habiles_popup.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub data_grid_chequeo_actualiza_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_chequeo_actualiza.RowCreated
        e.Row.Cells(1).Visible = False
    End Sub

    Private Sub Button_Eliminar_Expediente_Click(sender As Object, e As EventArgs) Handles Button_Eliminar_Expediente.Click
        Try
            Me.Hiddenid_expediente.Value = 0
            Me.Textbox_expediente_val_radicacion.Text = ""
        Catch ex As Exception

        End Try
    End Sub


    Private Sub Button_consulta_destinatario_interno_restore_Click(sender As Object, e As EventArgs) Handles Button_consulta_destinatario_interno_restore.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim refclasremit As New Class_remit_dest_interno
            Dim Result As String = ""
            Result = refclasremit.Lista_usuarios_gestion_internos_por_area_auxiliar(Val(HttpContext.Current.Session.Item("RA_ID_ORGANIGRAMA")), _
                                                                                           Me.Hiddenareagestion.Value, _
                                                                                           Me.data_grid_auxiliar_lista, _
                                                                                           Val(HttpContext.Current.Session.Item("RA_ID_EMPRESA_CONSULTA")), _
                                                                                           "", _
                                                                                           1, _
                                                                                           Me.TextBoxcontenidobusqueda.Text, _
                                                                                           "ASC")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_consulta_destinatario_interno_Click(sender As Object, e As EventArgs) Handles Button_consulta_destinatario_interno.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim refclasremit As New Class_remit_dest_interno
            Dim Result As String = ""
            Result = refclasremit.Lista_usuarios_gestion_internos_por_area_auxiliar(Val(HttpContext.Current.Session.Item("RA_ID_ORGANIGRAMA")), _
                                                                                    Me.Hiddenareagestion.Value, _
                                                                                    Me.data_grid_auxiliar_lista, _
                                                                                    Val(HttpContext.Current.Session.Item("RA_ID_EMPRESA_CONSULTA")), _
                                                                                    "", _
                                                                                    2, _
                                                                                    Me.TextBoxcontenidobusqueda.Text, _
                                                                                    "ASC")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub data_grid_auxiliar_lista_DataBound(sender As Object, e As EventArgs) Handles data_grid_auxiliar_lista.DataBound
        Try
            Select Case sender.SortDirection
                Case SortDirection.Ascending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                Case SortDirection.Descending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black
            End Select
        Catch ex As Exception
        End Try
    End Sub

    Private Sub data_grid_auxiliar_lista_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_auxiliar_lista.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_auxiliar_lista.PageIndex = e.NewPageIndex
            Dim refclasremit As New Class_remit_dest_interno
            Dim Result As String = ""
            Result = refclasremit.Lista_usuarios_gestion_internos_por_area_auxiliar(Val(HttpContext.Current.Session.Item("RA_ID_ORGANIGRAMA")), _
                                                                                    Me.Hiddenareagestion.Value, _
                                                                                    Me.data_grid_auxiliar_lista, _
                                                                                    Val(HttpContext.Current.Session.Item("RA_ID_EMPRESA_CONSULTA")), _
                                                                                    HttpContext.Current.Session.Item("SortExpression_interno_remit"), _
                                                                                    HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_INTERNO_REMIT"), _
                                                                                    Me.TextBoxcontenidobusqueda.Text, _
                                                                                    HttpContext.Current.Session.Item("SortDirection_interno_remit"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub data_grid_auxiliar_lista_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_auxiliar_lista.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub data_grid_auxiliar_lista_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_auxiliar_lista.Sorting
        Dim clasjava As New Classscrripjava
        Try
            Session.Item("SortExpression_interno_remit") = e.SortExpression
            If Session.Item("SortDirection_interno_remit") = "DESC" Then
                Session.Item("SortDirection_interno_remit") = "ASC"
            Else
                Session.Item("SortDirection_interno_remit") = "DESC"
            End If
            Dim refclasremit As New Class_remit_dest_interno
            Dim Result As String = ""
            Result = refclasremit.Lista_usuarios_gestion_internos_por_area_auxiliar(Val(HttpContext.Current.Session.Item("RA_ID_ORGANIGRAMA")), _
                                                                                    Me.Hiddenareagestion.Value, _
                                                                                    Me.data_grid_auxiliar_lista, _
                                                                                    Val(HttpContext.Current.Session.Item("RA_ID_EMPRESA_CONSULTA")), _
                                                                                    HttpContext.Current.Session.Item("SortExpression_interno_remit"), _
                                                                                    HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_INTERNO_REMIT"), _
                                                                                    Me.TextBoxcontenidobusqueda.Text, _
                                                                                    HttpContext.Current.Session.Item("SortDirection_interno_remit"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Button_nuevo_radicado_Click(sender As Object, e As EventArgs) Handles Button_nuevo_radicado.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_estados As New Class_ra_rad_estados_modulo_radicacion
            Result = Refclas_estados.Solicitar_enviar_radicado_a_estado_pendiente(Me.Page)
            If Result <> "YES" Then
                Me.Hidden_resultado_radic.Value = ""
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_nuevo_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Hidden_resultado_radic.Value = "YES"
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_nuevo_radicado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridView_list_registro_rad_DataBound(sender As Object, e As EventArgs) Handles GridView_list_registro_rad.DataBound
        Try
            Select Case sender.SortDirection
                Case SortDirection.Ascending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                Case SortDirection.Descending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black
            End Select
        Catch ex As Exception
        End Try
    End Sub

    Private Sub GridView_list_registro_rad_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_list_registro_rad.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            GridView_list_registro_rad.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim Refclas As New Class_ra_rad_estados_modulo_radicacion
            Result = Refclas.Lista_radicados_pendientes_interface(HttpContext.Current.Session.Item("RA_ID_USUARIO"),
                                                                  HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"),
                                                                  HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO"),
                                                                  HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO"),
                                                                  HttpContext.Current.Session.Item("SortExpression_publico"),
                                                                  HttpContext.Current.Session.Item("SortDirection_publico"),
                                                                  Me.titulo_label_list_registro_rad,
                                                                  Me.GridView_list_registro_rad,
                                                                  Me.Hidden_list_registro_rad,
                                                                  Me.Update_list_registro_rad)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.Update_list_registro_rad, "ModalPopupExtender_mensaje_personalizado")

            End If
            Me.Hidden_list_registro_rad.Value = -1
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.Update_list_registro_rad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub GridView_list_registro_rad_Sorting(sender As Object, e As GridViewSortEventArgs) Handles GridView_list_registro_rad.Sorting
        Dim clasjava As New Classscrripjava
        Try

            Dim Result As String = ""
            Dim Refclas As New Class_ra_rad_estados_modulo_radicacion
            Session.Item("SortExpression_publico") = e.SortExpression
            If Session.Item("SortDirection_publico") = "DESC" Then
                Session.Item("SortDirection_publico") = "ASC"
            Else
                Session.Item("SortDirection_publico") = "DESC"
            End If
            Result = Refclas.Lista_radicados_pendientes_interface(HttpContext.Current.Session.Item("RA_ID_USUARIO"), _
                                                                  HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"), _
                                                                  HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO"), _
                                                                  HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO"), _
                                                                  HttpContext.Current.Session.Item("SortExpression_publico"), _
                                                                  HttpContext.Current.Session.Item("SortDirection_publico"), _
                                                                  Me.titulo_label_list_registro_rad, _
                                                                  Me.GridView_list_registro_rad, _
                                                                  Me.Hidden_list_registro_rad, _
                                                                  Me.Update_list_registro_rad)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.Update_list_registro_rad, "ModalPopupExtender_mensaje_personalizado")

            End If
            Me.Hidden_list_registro_rad.Value = -1
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.Update_list_registro_rad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub GridView_list_registro_rad_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_list_registro_rad.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Button_tool_lista_pendientes_radicados_Click(sender As Object, e As EventArgs) Handles Button_tool_lista_pendientes_radicados.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_ra_rad_estados_modulo_radicacion
            Session.Item("SortExpression_publico") = "id_estado_radicado"
            Session.Item("SortDirection_publico") = "DESC"
            Result = Refclas.Lista_radicados_pendientes_interface(HttpContext.Current.Session.Item("RA_ID_USUARIO"), _
                                                                  HttpContext.Current.Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO"), _
                                                                  1, _
                                                                  "", _
                                                                  HttpContext.Current.Session.Item("SortExpression_publico"), _
                                                                  HttpContext.Current.Session.Item("SortDirection_publico"), _
                                                                  Me.titulo_label_list_registro_rad, _
                                                                  Me.GridView_list_registro_rad, _
                                                                  Me.Hidden_list_registro_rad, _
                                                                  Me.Update_list_registro_rad)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_list_registro_rad.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_tool_asigna_radicados_pendientes_Click(sender As Object, e As EventArgs) Handles Button_tool_asigna_radicados_pendientes.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Ref_class_estados_modulos_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim Result As String = ""
            Dim srtru_paramenter_image_() As stru_paramter_image = Nothing
            Dim stru_permisos_interface_ As stru_permisos_interface_envio = Nothing
            If Hidden_list_registro_rad.Value = "-1" Then
                clasjava.Showscripman_menu("Imposible encontrar el indentificador del radicado", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Me.Hidden_result_boton_tool.Value = ""
                Exit Sub
            End If
            Result = Ref_class_estados_modulos_radicacion.Asignar_radicado(Val(Hidden_list_registro_rad.Value),
                                                                           "YES",
                                                                            1,
                                                                            Me.Page,
                                                                            1,
                                                                            "GridView_list_documento_relacion",
                                                                            1,
                                                                            "IframeDitaliza_",
                                                                            "../workflow/WebFormEscan.aspx",
                                                                            1,
                                                                            1,
                                                                            srtru_paramenter_image_,
                                                                            stru_permisos_interface_)
            If Result <> "YES" Then
                Me.ModalPopupExtender_edition_list_registro_rad.Hide()
                Me.Hidden_result_boton_tool.Value = ""
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_list_registro_rad.Hide()
                Me.Hidden_result_boton_tool.Value = "YES"
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_tool_imprime_rotulo_Click(sender As Object, e As EventArgs) Handles Button_tool_imprime_rotulo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim upiframe As UpdatePanel = Me.Page.FindControl("UpdatePaneliframe")
            If upiframe Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control UpdatePaneliframe", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim upiframe_post As UpdatePanel = Me.Page.FindControl("UpdatePaneliframe_post")
            If upiframe_post Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control UpdatePaneliframe_post", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Modal As ModalPopupExtender = Me.Page.FindControl("ModalPopupExtenderimpre")
            Dim Result As String = ""
            Dim Ref_classradicador As New ClassRadicador
            Result = Ref_classradicador.Pro_impresion_rotulo(HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO"), _
                                                             Me.Page)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                upiframe.Update()
                Modal.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
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
        '    Result = Refclass.Eliminar_documento_relacionado_radicado_pendiente(Me.hiden_seleccion_documento.Value, _
        '                                                                        HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO"), _
        '                                                                        estado_actualizacion_imagen_ruta)
        '    If Result <> "YES" Then
        '        Me.Hidden_result_boton_tool.Value = ""
        '        clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        '    Else
        '        Me.Hidden_result_boton_tool.Value = "YES"
        '        If estado_actualizacion_imagen_ruta <> "YES" Then
        '            clasjava.Showscripman_menu("Se elimino el documento, pero no se pudo actualzar la imagen en la ruta worklfow error : " & _
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
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim stru_ As stru_registro_estado = Nothing
            Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO"),
                                                                                                      stru_)

            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipologia_documental, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassWorkflowDigitalizacion
            Dim id_configuracion As Integer = 0
            Result = Refclas.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna(Session.Item("Id_Ruta_Workflow"),
                                                                             stru_.id_tarea_workflow,
                                                                             "",
                                                                             0,
                                                                             0,
                                                                             "",
                                                                             id_configuracion,
                                                                             "",
                                                                             "")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipologia_documental, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split() As String = Me.hiden_seleccion_documento.Value.ToString.Split("|")
            Dim valor_campo As String = ""
            Result = Refclas.Actualiza_tipo_documento_lista_chequeo(split(1),
                                                                    Val(Me.DropDownList_tipologia_documental.SelectedValue),
                                                                    split(0),
                                                                    Me.DropDownList_tipologia_documental.SelectedItem.Text,
                                                                    Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                    split(2),
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

    Private Sub Button_tool_activa_sube_documento_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                             Session.Item("DG_TIPO_TRAMITE"),
                                                                             Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                             0)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                            Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                            "",
                                                                                                                            Me.DropDownList_adjunta_documento,
                                                                                                                            Me.Update_actualiza_adjunta_documento,
                                                                                                                            estado_resultado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Session.Item("WF_TIPO_ADJUNTA") = "ENLACE_RADICADO"
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
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO"),
                                                                                                      stru_)

            If Result <> "YES" Then
                CLAS.Showscripman_menu(Result, Me.UpdatePanel_descarga, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                CLAS.Showscripman_menu(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim evalua_flujo_ruta As Integer = 0
            If stru_.id_tarea_workflow <> 0 Then
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
                Result = Refclas_dagabinete.Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado("",
                                                                                                             ID_DOCUMENTO,
                                                                                                             TIPO_DOCUMENTO,
                                                                                                             datos_image,
                                                                                                             Session.Item("DG_TIPODIGITALIZACION"),
                                                                                                             stru_.id_tarea_workflow,
                                                                                                             HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                             stru_.consecutivo_radicado,
                                                                                                             evalua_flujo_ruta,
                                                                                                             1)
                If Result <> "YES" Then
                    CLAS.Showscripman_menu(Result, UpdatePanel_descarga, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.Hidden_result_load.Value = "YES"
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
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim stru_ As stru_registro_estado = Nothing
            Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO"),
                                                                                                      stru_)

            If Result <> "YES" Then
                CLAS.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim evalua_flujo_ruta As Integer = 0
            If stru_.id_tarea_workflow <> 0 Then
                evalua_flujo_ruta = 1
            End If
            Dim Nombre_Ruta_Workflow As String = ""
            Dim Refclas_dagabinete As New ClassDaGabinete
            Dim ID_DOCUMENTO As Integer = 0
            Dim TIPO_DOCUMENTO As Integer = 0
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            Result = Refclas_dagabinete.Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado("",
                                                                                                         ID_DOCUMENTO,
                                                                                                         TIPO_DOCUMENTO,
                                                                                                         stru_datos_image_lista,
                                                                                                         Session.Item("DG_TIPODIGITALIZACION"),
                                                                                                         stru_.id_tarea_workflow,
                                                                                                         HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                         stru_.consecutivo_radicado,
                                                                                                         evalua_flujo_ruta,
                                                                                                         0)
            If Result <> "YES" Then
                CLAS.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_result_load_.Value = "YES"
            Me.Hidden_date_row_.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                    "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & stru_datos_image_lista.id_tarea_workflow &
                    "|" & stru_datos_image_lista.estado_firma_digital & "|" & stru_datos_image_lista.icono_icono_awe_some
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub

    Private Sub Button_tool_auto_terminar_Click(sender As Object, e As EventArgs) Handles Button_tool_auto_terminar.Click
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim id_actividad As Integer = 0
            Dim id_usuario_workflow As Integer = 0
            Dim mensaje As String = ""
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Result = Refclas_f.Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                                           HttpContext.Current.Session.Item("Id_Usuario_Workflow"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Auto_terminar_flujo_documental(id_usuario_workflow, _
                                                            id_actividad, _
                                                            mensaje)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.LabelMensaje_autoterminar.Text = mensaje
            Me.Hidden_id_actividad.Value = id_actividad
            Me.Hidden_id_usuario.Value = id_usuario_workflow
            Me.updatepanel_mensaje_extender_autoterminar.Update()
            Me.updatepanel_mensaje_extender_autoterminar.Update()
            Me.ModalPopupExtendermensaje_autoterminar.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub btnCancel_autoterminar_Click(sender As Object, e As EventArgs) Handles btnCancel_autoterminar.Click
        ModalPopupExtendermensaje_autoterminar.Hide()
    End Sub

    Protected Sub btnOkay_autoterminar_Click(sender As Object, e As EventArgs) Handles btnOkay_autoterminar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim result As String = ""
            Me.Hidden_rest_auto_terinar.value = ""
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            Dim Ref_class_estados_modulos_radicacion As New Class_ra_rad_estados_modulo_radicacion
            result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_boton_auto_terminar)
                Exit Sub
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")), _
                                                                                         Estado_solicitud_aprobacion, _
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_boton_auto_terminar)
                Exit Sub
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                clasjava.Showscripman("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim resultadocorreo As String = "YES"
            Dim refclas As New ClassWorkflow
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim id_registro_radicado As Long = 0
            result = Class_ra_rad_estados_modulo_radicacion.Solicita_id_estado_modulo_id_tarea_workflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), 1, id_registro_radicado)
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_boton_auto_terminar)
                Exit Sub
            End If
            result = refclas.Terminar_Tarea_Workflow_Bacth(Me.Hidden_id_usuario.Value.ToString,
                                                           Me.Hidden_id_actividad.Value.ToString,
                                                           0,
                                                           HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                           "",
                                                           0,
                                                           0,
                                                           0,
                                                           1,
                                                           resultadocorreo,
                                                           0,
                                                           0,
                                                           0,
                                                           1,
                                                           1,
                                                           id_registro_radicado,
                                                           1)
            If result <> "YES" Then
                clasjava.Showscripman(result, UpdatePanel_boton_auto_terminar)
                Exit Sub
            Else
                Me.ModalPopupExtendermensaje_autoterminar.Hide()
                Me.Hidden_id_actividad.Value = "0"
                Me.Hidden_id_usuario.Value = "0"
                Me.updatepanel_mensaje_extender_autoterminar.Update()
                Dim srtru_paramenter_image_() As stru_paramter_image = Nothing
                Dim stru_permisos_interface_ As stru_permisos_interface_envio = Nothing
                Dim Numero_radicados_pendientes As Integer = 0
                result = Ref_class_estados_modulos_radicacion.Asignar_radicado(0,
                                                                              "NO",
                                                                              0,
                                                                              Me.Page,
                                                                              0,
                                                                              "GridView_list_documento_relacion",
                                                                              1,
                                                                              "IframeDitaliza_",
                                                                              "../workflow/WebFormEscan.aspx",
                                                                              1,
                                                                              1,
                                                                              srtru_paramenter_image_,
                                                                              stru_permisos_interface_)
                If result <> "YES" Then
                    clasjava.Showscripman(result, UpdatePanel_boton_auto_terminar)
                    Exit Sub
                Else
                    Me.Hidden_rest_auto_terinar.Value = "YES"
                End If
                If resultadocorreo <> "YES" Then
                    clasjava.Showscripman(resultadocorreo, UpdatePanel_boton_auto_terminar)
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_auto_terminar, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_tool_terminar_Click(sender As Object, e As EventArgs) Handles Button_tool_terminar.Click
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Refclas_w As New ClassWorkflow
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Dim Id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_actividad_flujo_trabajo As Integer = 0
            Result = Refclas_f.Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO"), _
                                                                                                                id_actividad_flujo_trabajo, _
                                                                                                                Id_flujo_trabajo, _
                                                                                                                id_usuario_workflow_actividad_flujo_trabajo, _
                                                                                                                HttpContext.Current.Session.Item("Id_Usuario_Workflow"), _
                                                                                                                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estado_flujo As Integer = 1
            Dim Nombre_flujo_trabajo As String = ""
            If Id_flujo_trabajo <> 0 Then
                Session.Item("WF_ESTADO_FLUJO_RUTA") = "FLUJO"
                Refclas_f.SolicitaEstadoAbiertoCerradoFlujoDocumental(Id_flujo_trabajo, _
                                                                           estado_flujo)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If estado_flujo = 0 Then
                    id_actividad_flujo_trabajo = 0
                End If
                Result = Refclas_f.SolicitaNombreFlujoTrabajoPorIdFlujo(Id_flujo_trabajo, _
                                                                             Nombre_flujo_trabajo)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If id_actividad_flujo_trabajo <> 0 Then
                    Result = Refclas_f.Solicita_listado_actividades_para_envio_tarea_a_flujo(Id_flujo_trabajo,
                                                                                        GridView_envia_flujo,
                                                                                        titulo_label_grid,
                                                                                        Label_nombre_flujo,
                                                                                        Hidden_id_actividad_flujo,
                                                                                        UpdateGeneral_documentos,
                                                                                        Nombre_flujo_trabajo,
                                                                                        id_actividad_flujo_trabajo,
                                                                                        1)
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else

                        ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                        Exit Sub
                    End If
                Else
                    Result = Refclas_f.Solicita_listado_actividades_para_envio_tarea_a_flujo(Id_flujo_trabajo,
                                                                                        GridView_envia_flujo,
                                                                                        titulo_label_grid,
                                                                                        Label_nombre_flujo,
                                                                                        Hidden_id_actividad_flujo,
                                                                                        UpdateGeneral_documentos,
                                                                                        Nombre_flujo_trabajo,
                                                                                        id_actividad_flujo_trabajo,
                                                                                        1)
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
                        'Hidden_lista_ruta_flujo.Value = "F"
                        Exit Sub
                    End If
                End If
            Else
                Session.Item("WF_ESTADO_FLUJO_RUTA") = "RUTA"
                Dim Nombre_ruta As String = ""
                Dim Ref_class_wf_ruta As New Class_worflow_rutas
                Result = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(Nombre_ruta)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim ref_grupos_workflow As New Class_grupos_workflow
                Result = ref_grupos_workflow.Solicita_Listado_actividades_para_envio_de_tareas_a_ruta(Session.Item("Id_Grupo_Workflow"),
                                                                              GridView_envia_flujo,
                                                                              titulo_label_grid,
                                                                              Label_nombre_flujo,
                                                                              Hidden_id_actividad_flujo,
                                                                              Nombre_ruta,
                                                                              UpdateGeneral_documentos,
                                                                              1)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                ModalPopupExtender_edition_lista_actividades_worflow_ruta.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridView_envia_flujo_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_envia_flujo.RowCreated
        Try
            If HttpContext.Current.Session.Item("WF_ESTADO_FLUJO_RUTA") = "FLUJO" Then
                e.Row.Cells(1).Visible = False
                e.Row.Cells(2).Visible = False
                e.Row.Cells(3).Visible = False
                e.Row.Cells(4).Visible = False
                e.Row.Cells(6).Visible = False
            Else
                e.Row.Cells(1).Visible = False
                e.Row.Cells(2).Visible = False
            End If          
        Catch ex As Exception

        End Try
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
                Result = Refclas.Lista_detalle_usuario_workflow(Me.Hidden_id_usuario_workflow.Value, _
                                                                nombre_usuario, _
                                                                cargo_usuario, _
                                                                correo_electronico, _
                                                                grupo)
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
                Result = Refclas.Lista_detalle_actividad_workflow(Me.Hidden_id_actividad_destino.Value, _
                                                                  nombre_actividad, _
                                                                  descripcion_actividad, _
                                                                  tipo_actividad)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Label_nombre_actividad.Text = nombre_actividad
                    Me.Label_descripcion.Text = descripcion_actividad
                    Me.Label_tipo_actividad.Text = tipo_actividad

                End If
                Result = Refclas.Lista_usuarios_relacionados_id_actividad(Me.Hidden_id_actividad_destino.Value, _
                                                                          matri_usuarios_relacionados)
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
    Private Sub Button_activa_enviar_actividad_flujo_trabajo_Click(sender As Object, e As EventArgs) Handles Button_activa_enviar_actividad_flujo_trabajo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim result As String = ""
            Me.Hidden_resul_eviar_actividad.Value = ""
            Dim Ref_class_estados_modulos_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim stru_conector_flujo_ As stru_conector_flujo = Nothing
            Dim ref_Class_wf_registro_conectores As New Class_wf_registro_conectores_actividades_envio_flujo_trabajo
            result = ref_Class_wf_registro_conectores.Solicita_datos_estructura_conector_flujo_trabajo(Me.Hidden_id_actividad_flujo.Value, _
                                                                                                       stru_conector_flujo_)
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_enviar_actividad)
                Exit Sub
            End If
            Dim resultadocorreo As String = "YES"
            Dim refclas As New ClassWorkflow
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim id_registro_radicado As Long = 0
            result = Class_ra_rad_estados_modulo_radicacion.Solicita_id_estado_modulo_id_tarea_workflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                                        1,
                                                                                                        id_registro_radicado)
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_enviar_actividad)
                Exit Sub
            End If
            result = refclas.Terminar_Tarea_Workflow_Bacth(stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO.ToString,
                                                           stru_conector_flujo_.ID_ACTIVIDAD_DESTINO.ToString,
                                                           0,
                                                           Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                           "",
                                                           stru_conector_flujo_.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO,
                                                           stru_conector_flujo_.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO,
                                                           stru_conector_flujo_.ID_USUARIO_WORKFLOW_DESTINO,
                                                           1,
                                                           resultadocorreo,
                                                           0,
                                                           Val(Hidden_id_conector.Value),
                                                           Val(HttpContext.Current.Session("Id_Usuario_Workflow")),
                                                           Val(HttpContext.Current.Session("Id_actividad_Workflow")),
                                                           1,
                                                           id_registro_radicado,
                                                           1)
            If result <> "YES" Then
                clasjava.Showscripman(result, UpdatePanel_enviar_actividad)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                Me.updatepanel_mensaje_extender_autoterminar.Update()
                Dim srtru_paramenter_image_() As stru_paramter_image = Nothing
                Dim stru_permisos_interface_ As stru_permisos_interface_envio = Nothing
                Dim Numero_radicados_pendientes As Integer = 0
                result = Ref_class_estados_modulos_radicacion.Asignar_radicado(0,
                                                                              "NO",
                                                                              0,
                                                                              Me.Page,
                                                                              0,
                                                                              "GridView_list_documento_relacion",
                                                                              1,
                                                                              "IframeDitaliza_",
                                                                              "../workflow/WebFormEscan.aspx",
                                                                              1,
                                                                              1,
                                                                              srtru_paramenter_image_,
                                                                              stru_permisos_interface_)
                If result <> "YES" Then
                    clasjava.Showscripman(result, UpdatePanel_enviar_actividad)
                    Exit Sub
                Else
                    Me.Hidden_resul_eviar_actividad.Value = "YES"
                End If
                If resultadocorreo <> "YES" Then
                    clasjava.Showscripman(resultadocorreo, UpdatePanel_enviar_actividad)
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_enviar_actividad)
        End Try
    End Sub

    Private Sub Button_tool_activa_enviar_actividad_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_enviar_actividad.Click
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Ref_class_listado_actividades As New Class_Listado_Actividades_workflow
            Result = Ref_class_listado_actividades.Solicita_listado_actividades_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")), _
                                                                                     1, _
                                                                                     "", _
                                                                                      Me.GridView_envia_actividades, _
                                                                                      Me.titulo_label_lista_actividad_ruta, _
                                                                                      Me.Hidden_sel_actividad, _
                                                                                      Me.UpdateGeneral_lista_actividades_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_lista_actividades_ruta.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_tool_enviar_actividad_Click(sender As Object, e As EventArgs) Handles Button_tool_enviar_actividad.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim result As String = ""
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                clasjava.Showscripman("Debe seleccionar la tarea", UpdatePanel_boton_tool)
                Exit Sub
            End If
            Me.Hidden_result_boton_tool.Value = ""
            Dim resultadocorreo As String = "YES"
            Dim refclas As New ClassWorkflow
            Dim Ref_class_estados_modulos_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim id_registro_radicado As Long = 0
            result = Class_ra_rad_estados_modulo_radicacion.Solicita_id_estado_modulo_id_tarea_workflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), 1, id_registro_radicado)
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            result = refclas.Terminar_Tarea_Workflow_Bacth("",
                                                           Me.Hidden_id_tarea.Value.ToString,
                                                           0,
                                                           HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                           "",
                                                           0,
                                                           0,
                                                           0,
                                                           1,
                                                           resultadocorreo,
                                                           0,
                                                           0,
                                                           0,
                                                           1,
                                                           1,
                                                           id_registro_radicado,
                                                           1)
            If result <> "YES" Then
                clasjava.Showscripman(result, UpdatePanel_boton_tool)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_lista_actividades_ruta.Hide()
                Dim srtru_paramenter_image_() As stru_paramter_image = Nothing
                Dim stru_permisos_interface_ As stru_permisos_interface_envio = Nothing
                result = Ref_class_estados_modulos_radicacion.Asignar_radicado(0,
                                                                              "NO",
                                                                              0,
                                                                              Me.Page,
                                                                              0,
                                                                              "GridView_list_documento_relacion",
                                                                              1,
                                                                              "IframeDitaliza_",
                                                                              "../workflow/WebFormEscan.aspx",
                                                                              1,
                                                                              1,
                                                                              srtru_paramenter_image_,
                                                                              stru_permisos_interface_)
                If result <> "YES" Then
                    clasjava.Showscripman(result, UpdatePanel_boton_tool)
                    Exit Sub
                Else
                    Me.Hidden_result_boton_tool.Value = "YES"
                End If
                If resultadocorreo <> "YES" Then
                    clasjava.Showscripman(resultadocorreo, UpdatePanel_boton_tool)
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_enviar_usuario_Click(sender As Object, e As EventArgs) Handles Button_tool_enviar_usuario.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim result As String = ""
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                clasjava.Showscripman("Debe seleccionar la tarea", UpdatePanel_boton_tool)
                Exit Sub
            End If
            If Me.Hidden_id_usuario_envio.Value = 0 Then
                clasjava.Showscripman("Debe seleccionar el usuario", UpdatePanel_boton_tool)
                Exit Sub
            End If
            If Me.Hidden_id_actividad_envio.Value = 0 Then
                clasjava.Showscripman("Debe seleccionar al actividad del usuario", UpdatePanel_boton_tool)
                Exit Sub
            End If
            Me.Hidden_result_boton_tool.Value = ""
            Dim resultadocorreo As String = "YES"
            Dim refclas As New ClassWorkflow
            Dim Ref_class_estados_modulos_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim id_registro_radicado As Long = 0
            result = Class_ra_rad_estados_modulo_radicacion.Solicita_id_estado_modulo_id_tarea_workflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), 1, id_registro_radicado)
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            result = refclas.Terminar_Tarea_Workflow_Bacth(Me.Hidden_id_usuario_envio.Value.ToString,
                                                           Me.Hidden_id_actividad_envio.Value.ToString,
                                                           0,
                                                           HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                           "",
                                                           0,
                                                           0,
                                                           0,
                                                           1,
                                                           resultadocorreo,
                                                           0,
                                                           0,
                                                           0,
                                                           1,
                                                           1,
                                                           id_registro_radicado,
                                                           1)
            If result <> "YES" Then
                clasjava.Showscripman(result, UpdatePanel_boton_tool)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_lista_usuarios_ruta.Hide()
                Me.Hidden_id_usuario_envio.Value = 0
                Me.Hidden_id_actividad_envio.Value = 0
                Dim srtru_paramenter_image_() As stru_paramter_image = Nothing
                Dim stru_permisos_interface_ As stru_permisos_interface_envio = Nothing
                result = Ref_class_estados_modulos_radicacion.Asignar_radicado(0,
                                                                              "NO",
                                                                              0,
                                                                              Me.Page,
                                                                              0,
                                                                              "GridView_list_documento_relacion",
                                                                              1,
                                                                              "IframeDitaliza_",
                                                                              "../workflow/WebFormEscan.aspx",
                                                                              1,
                                                                              1,
                                                                              srtru_paramenter_image_,
                                                                              stru_permisos_interface_)
                If result <> "YES" Then
                    clasjava.Showscripman(result, UpdatePanel_boton_tool)
                    Exit Sub
                Else
                    Me.Hidden_result_boton_tool.Value = "YES"
                End If
                If resultadocorreo <> "YES" Then
                    clasjava.Showscripman(resultadocorreo, UpdatePanel_boton_tool)
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub

    Private Sub Button_tool_enviar_ruta_Click(sender As Object, e As EventArgs) Handles Button_tool_enviar_ruta.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim result As String = ""
            Me.Hidden_result_boton_tool.Value = ""
            Dim Ref_class_estados_modulos_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim stru_conector_flujo_ As stru_actividades_envio = Nothing
            Dim ref_Class_actividades_envio As New Class_actividades_disponibles_envio
            result = ref_Class_actividades_envio.Solicita_estructura_actividades_envio(Val(Me.Hidden_id_actividad_disp_envio.Value),
                                                                                      stru_conector_flujo_)
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim resultadocorreo As String = "YES"
            Dim refclas As New ClassWorkflow
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim id_registro_radicado As Long = 0
            result = Class_ra_rad_estados_modulo_radicacion.Solicita_id_estado_modulo_id_tarea_workflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), 1, id_registro_radicado)
            If result <> "YES" Then
                clasjava.Showscripman(result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            result = refclas.Terminar_Tarea_Workflow_Bacth("",
                                                           Me.Hidden_id_actividad_envio.Value.ToString,
                                                           0,
                                                           Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                           "",
                                                           0,
                                                           0,
                                                           0,
                                                           stru_conector_flujo_.Estado_evia_correo,
                                                           resultadocorreo,
                                                           0,
                                                           Val(Hidden_id_conector.Value),
                                                           Val(HttpContext.Current.Session("Id_Usuario_Workflow")),
                                                           Val(HttpContext.Current.Session("Id_actividad_Workflow")),
                                                           1,
                                                           id_registro_radicado,
                                                           1)
            If result <> "YES" Then
                clasjava.Showscripman(result, UpdatePanel_boton_tool)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                Me.updatepanel_mensaje_extender_autoterminar.Update()
                Me.Hidden_id_actividad_envio.Value = 0
                Me.Hidden_id_actividad_disp_envio.Value = 0
                Dim srtru_paramenter_image_() As stru_paramter_image = Nothing
                Dim stru_permisos_interface_ As stru_permisos_interface_envio = Nothing
                Dim Numero_radicados_pendientes As Integer = 0
                result = Ref_class_estados_modulos_radicacion.Asignar_radicado(0,
                                                                               "NO",
                                                                               0,
                                                                               Me.Page,
                                                                               0,
                                                                               "GridView_list_documento_relacion",
                                                                               1,
                                                                               "IframeDitaliza_",
                                                                               "../workflow/WebFormEscan.aspx",
                                                                               1,
                                                                               1,
                                                                               srtru_paramenter_image_,
                                                                               stru_permisos_interface_)
                If result <> "YES" Then
                    clasjava.Showscripman(result, UpdatePanel_boton_tool)
                    Exit Sub
                Else
                    Me.Hidden_result_boton_tool.Value = "YES"
                End If
                If resultadocorreo <> "YES" Then
                    clasjava.Showscripman(resultadocorreo, UpdatePanel_boton_tool)
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_activa_enviar_usuario_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_enviar_usuario.Click
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Ref_class_usuario_workflow As New Class_usuario_workflow
            Result = Ref_class_usuario_workflow.Solicita_listado_usuarios_workflow_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                                        1,
                                                                                        "",
                                                                                        Me.GridView_envia_usuario,
                                                                                        Me.titulo_label_lista_usuario_ruta,
                                                                                        Me.Hidden_sel_actividad,
                                                                                        Me.UpdateGeneral_lista_usuarios_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_lista_usuarios_ruta.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub GridView_envia_actividades_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_envia_actividades.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_tool_busqueda_enviar_actividad_Click(sender As Object, e As EventArgs) Handles Button_tool_busqueda_enviar_actividad.Click

        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Ref_class_listado_actividades As New Class_Listado_Actividades_workflow
            Result = Ref_class_listado_actividades.Solicita_listado_actividades_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")), _
                                                                                     2, _
                                                                                     TextBox_buequeda_general_lista_actividades.Text, _
                                                                                      Me.GridView_envia_actividades, _
                                                                                      Me.titulo_label_lista_actividad_ruta, _
                                                                                      Me.Hidden_sel_actividad, _
                                                                                      Me.UpdateGeneral_lista_actividades_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
        
    End Sub

    Private Sub Button_tool_restore_busqueda_enviar_actividad_Click(sender As Object, e As EventArgs) Handles Button_tool_restore_busqueda_enviar_actividad.Click
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Ref_class_listado_actividades As New Class_Listado_Actividades_workflow
            Result = Ref_class_listado_actividades.Solicita_listado_actividades_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")), _
                                                                                     1, _
                                                                                     "", _
                                                                                      Me.GridView_envia_actividades, _
                                                                                      Me.titulo_label_lista_actividad_ruta, _
                                                                                      Me.Hidden_sel_actividad, _
                                                                                      Me.UpdateGeneral_lista_actividades_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridView_envia_usuario_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_envia_usuario.RowCreated
        Try
            Try
                e.Row.Cells(1).Visible = False
                e.Row.Cells(2).Visible = False
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_tool_busqueda_enviar_usuario_Click(sender As Object, e As EventArgs) Handles Button_tool_busqueda_enviar_usuario.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_usuario_workflow As New Class_usuario_workflow
            Result = Ref_class_usuario_workflow.Solicita_listado_usuarios_workflow_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")), _
                                                                                        2, _
                                                                                        Me.TextBox_buequeda_general_lista_usuarios.Text, _
                                                                                        Me.GridView_envia_usuario, _
                                                                                        Me.titulo_label_lista_usuario_ruta, _
                                                                                        Me.Hidden_sel_actividad, _
                                                                                        Me.UpdateGeneral_lista_usuarios_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_tool_restore_busqueda_enviar_usuario_Click(sender As Object, e As EventArgs) Handles Button_tool_restore_busqueda_enviar_usuario.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_usuario_workflow As New Class_usuario_workflow
            Result = Ref_class_usuario_workflow.Solicita_listado_usuarios_workflow_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")), _
                                                                                        1, _
                                                                                        "", _
                                                                                        Me.GridView_envia_usuario, _
                                                                                        Me.titulo_label_lista_usuario_ruta, _
                                                                                        Me.Hidden_sel_actividad, _
                                                                                        Me.UpdateGeneral_lista_usuarios_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_cerrar_ventana_date_Click(sender As Object, e As EventArgs) Handles Button_cerrar_ventana_date.Click
        Try
            Me.ModalPopupExtender_Val_Radicado.Hide()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_tool_termitar_radicado_Click(sender As Object, e As EventArgs) Handles Button_tool_termitar_radicado.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_result_boton_tool.Value = ""
            If HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO") = 0 Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Ref_class_estados_modulos_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Result = Class_ra_rad_estados_modulo_radicacion.Actualiza_estado_registro_modulo_radicacion(HttpContext.Current.Session.Item("RA_ID_REGISTRO_RADICADO"),
                                                                                                        2)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim srtru_paramenter_image_() As stru_paramter_image = Nothing
            Dim stru_permisos_interface_ As stru_permisos_interface_envio = Nothing
            Result = Ref_class_estados_modulos_radicacion.Asignar_radicado(0,
                                                                          "NO",
                                                                          0,
                                                                          Me.Page,
                                                                          0,
                                                                          "GridView_list_documento_relacion",
                                                                          1,
                                                                          "IframeDitaliza_",
                                                                          "../workflow/WebFormEscan.aspx",
                                                                          1,
                                                                          1,
                                                                          srtru_paramenter_image_,
                                                                          stru_permisos_interface_)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_boton_tool)
                Exit Sub
            Else
                Me.Hidden_result_boton_tool.Value = "YES"
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_activa_detalle_radicado_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_detalle_radicado.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_estados_modulo_rad As New Class_estados_modulo_radicacion
            If HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = "-1" Or HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO") = "" Then
                scripjava.Showscripman_menu("Debe seleccionar el radicado", UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_class_detalle_plantilla_rad As New Class_ra_detalle_plantilla_radicado
            Result = ref_class_detalle_plantilla_rad.Genera_interface_detalle_radicado(HttpContext.Current.Session.Item("RA_RADICADO_REGISTRO"),
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
    '------------ZONA NOTAS-------------------------------
    Private Sub GridView_lista_notas_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView_lista_notas.RowCreated
        Try
            e.Row.Cells(2).Visible = False
            e.Row.Cells(1).Visible = False
            e.Row.Cells(5).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    '-------Anotacion sobre la actividad
    Private Sub ImageButtonanotacion_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonanotacion.Click
        Dim refclsjava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Session.Item("ID_TAREA_SELECCIONDA") = "0" Then Exit Sub
            If HttpContext.Current.Session("Interactuar_Anotaciones") = "0" Then
                refclsjava.Showscripman("El usuario no tiene permiso para interactuar con anotaciones ", UpdatePanel_tool_menu)
                Exit Sub
            End If
            Dim refclas As New Class_anotacion_tarea
            Result = refclas.Listar_Anotaciones_tarea_workflow(Me.GridView_lista_notas,
                                                               HttpContext.Current.Session("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                refclsjava.Showscripman(Result, UpdatePanel_tool_menu)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_content_anotacion.Show()
                Exit Sub
            End If

        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_tool_menu)
        End Try
    End Sub
End Class