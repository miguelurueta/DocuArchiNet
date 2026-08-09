Public Class WebFormRadicacionEntranteInterna
    Inherits System.Web.UI.Page

    

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
        Dim comillas As String = Char.ConvertFromUtf32(34)
        Dim script As [String] = "$(document).ready(function(){$('#" + data_grid_auxiliar_lista.ClientID & "').Scrollable({ScrollHeight: 310,IsInUpdatePanel:true});});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID))) Then
            'ScriptManager.RegisterClientScriptBlock(UpdatePanel_auxiliar_destinatarios_internos_popup, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID), script, True)
        End If
        ' Dim script As [String] = "$(document).ready(function(){$('#" + data_grid_auxiliar_lista.ClientID & "').gridviewScroll({width:1200,height: 200});});"
        'Dim script As [String] = "$(document).ready(function(){$('#" + data_grid_auxiliar_lista.ClientID & "').fixedHeaderTable({footer: true,cloneHeadToFoot: true, altClass: 'odd', autoShow: true });});"
        'If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID))) Then
        'ScriptManager.RegisterClientScriptBlock(data_grid_auxiliar_lista, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", data_grid_auxiliar_lista.ClientID), script, True)

        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), "Key", "MakeStaticHeader(" + data_grid_auxiliar_lista.ClientID + ", 400, 950 , 40 ,true);", True)
        'End If
        'scr = "$(document).ready(function () {$().gridviewScroll();});"
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.IsPostBack = False Then
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
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            If HttpContext.Current.Session("Radicar_enviar_documento_master_interno") = 0 Then
                Result = Class_remit_dest_interno.Solicita_lista_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), RE_DropDownList_user_radica, UpdatePnaelcontrolesradicacion)
                If Result <> "YES" Then
                    Label_estado_transac.Text = Result
                End If
            Else
                Result = Class_remit_dest_interno.Solicita_lista_usuarios_gestion(Session.Item("GA_IDUSUARIOGESTION"), RE_DropDownList_user_radica, UpdatePnaelcontrolesradicacion)
                If Result <> "YES" Then
                    Label_estado_transac.Text = Result
                End If
            End If

        End If

        If Session.Item("RA_MODULO_SELECCIONADO") <> "" Then
            Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim Refclas As New ClassRadicador
            Dim reflcas_producion As New ClassGaProducionDocumental
            Dim Result As String = ""
            '------------------------------------------------------
            'Detecta la seleccion del modulo de radicacion
            '------------------------------------------------------
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    If Me.IsPostBack = False Then

                        Result = reflcas_producion.Genera_Interface_Radicacion_Entrante(split(1), _
                                                                                        split(0), _
                                                                                        Me, split(4), _
                                                                                         split(5))
                        If Result <> "YES" Then
                            Label_estado_transac.Text = Result
                        Else
                            Label_estado_transac.Text = ""
                        End If
                    Else
                        'If Me.TableControles.Controls.Count = 0 Then
                        '    Result = Refclas.Genera_Interface_Radicacion_Entrante(split(1), split(0), Me, split(4))
                        'End If

                    End If

                End If
                If split(2) = "RADICACION SALIENTE" Then
                    If Me.IsPostBack = False Then

                        Result = Refclas.Genera_Interface_Radicacion_Saliente(split(1), _
                                                                              split(0), _
                                                                              Me, split(4))
                        If Result <> "YES" Then
                            Label_estado_transac.Text = Result
                        Else
                            Label_estado_transac.Text = ""
                        End If
                    Else
                        'If Me.TableControles.Controls.Count = 0 Then
                        '    Result = Refclas.Genera_Interface_Radicacion_Entrante(split(1), split(0), Me, split(4))
                        'End If

                    End If

                End If

            End If


        End If


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
            Dim reflcas_producion As New ClassGaProducionDocumental
            Dim result As String = ""
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    If Me.IsPostBack = True Then
                        result = reflcas_producion.Genera_Interface_Radicacion_Entrante(split(1), _
                                                                                        split(0), _
                                                                                        Me, _
                                                                                        split(4), _
                                                                                        split(5))
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
                        'result = refclas.Genera_Interface_Radicacion_Saliente(split(1), split(0), Me, split(4))
                        'If result <> "YES" Then
                        '    clasjava.Showscripman_menu(result, Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                        '    Exit Sub
                        'End If
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
                'clasjava.Showscripman("Imposible encontrar el control UpdatePnaelcontrolesradicacion", Me.UpdatePnaelcontrolesradicacion)
                clasjava.Showscripman_menu("Imposible encontrar el control UpdatePnaelcontrolesradicacion", Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Not hide Is Nothing And hide.value <> "" Then
                'Dim id_empresa As Integer = -1
                'result = refclas.Retorna_ID_Empresa_Usuario_Radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"), _
                '                                                      id_empresa)
                'If result <> "YES" Then
                '    If Not update Is Nothing Then
                '        'clasjava.Showscripman(result, update)
                '        clasjava.Showscripman_menu(result, Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                '        droparea.Focus()
                '        Exit Sub
                '    End If
            Else
                'Dim id_organigrama As Integer = -1
                'Dim ref_clas_empresa As New Class_registro_organigrama
                'result = ref_clas_empresa.Retorna_Id_Organigrama_activo_empresa(id_empresa, _
                '                                                                id_organigrama)
                'If result <> "YES" Then
                '    'clasjava.Showscripman(result, update)
                '    clasjava.Showscripman_menu(result, Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
                '    droparea.Focus()
                '    Exit Sub
                'Else

                '    If Not Droplist Is Nothing Then
                '        result = refclas.Lista_usuarios_gestion_internos_por_area(id_organigrama, hide.Value, Droplist, id_empresa)
                '        If result <> "YES" Then
                '            If Not update Is Nothing Then
                '                'clasjava.Showscripman(result, update)
                '                'droparea.Focus()
                '                Exit Sub
                '            End If
                '        End If
                '    End If
                'End If
                'droparea.Focus()
                'End If
            End If

        Catch ex As Exception
            'clasjava.Showscripman(ex.Message, update)
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePnaelcontrolesradicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Buttontramitevence_Click(sender As Object, e As EventArgs) Handles Buttontramitevence.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim RefclasRadicador As New ClassRadicador
            Result = RefclasRadicador.Seleccion_tipo_tramite(Page)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelbotonesradicado)
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
            Dim consecutivo_radicado As String = ""
            Dim archivo_server As String = ""
            If split(0) = "RADICACION" Then
                If split(2) = "RADICACION ENTRANTE" Then
                    If Me.IsPostBack = True Then
                        Dim resultado_correo As String = ""
                        result = refclas.Registra_Radicacion_entrante_interna(split(1), _
                                                                              split(0), _
                                                                              Me, _
                                                                              split(4), _
                                                                              Val(split(6)), _
                                                                              Val(split(5)), _
                                                                              consecutivo_radicado, _
                                                                              archivo_server, _
                                                                              resultado_correo, _
                                                                              Session.Item("PG_SELECCION_ID_EXPEIDENTE"))
                        If result <> "YES" Then
                            hiden000001.Value = ""
                            clasjava.Showscripman_menu(result, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        Else
                            Hidden_ruta_archivo.Value = archivo_server
                            ifmExcel_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                            updatapanel_iframe.Update()
                            hiden000001.Value = "YES"
                            If resultado_correo <> "YES" Then
                                clasjava.Showscripman_menu(resultado_correo, Me.UpdatePanelradciacionbotones, "ModalPopupExtender_mensaje_personalizado")
                                Exit Sub
                            End If              
                        End If
                    End If
                End If           
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
    Private Sub Button_asignar_auxiliar_destinatarios_internos_popup_Click(sender As Object, e As EventArgs) Handles Button_asignar_auxiliar_destinatarios_internos_popup.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Dim clasjava As New Classscrripjava
        Try
            If Me.Hidden_auxiliar_id.Value = "-1" Then
                'clasjava.Showscripman("Debe seleccionar un usuario de la lista ", UpdatePanel_botones_popup_interno)
                clasjava.Showscripman_menu("Debe seleccionar un usuario de la lista ", Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.seleciona_usuario_interno_dest_gestion_drowp_list(Val(Me.Hidden_auxiliar_id.Value), Me)
            If Result <> "YES" Then
                'clasjava.Showscripman(Result, UpdatePanel_botones_popup_interno)
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_auxiliar_destinatarios_internos_popup.Hide()
            End If
        Catch ex As Exception
            'clasjava.Showscripman(ex.Message, UpdatePanel_botones_popup_interno)
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_popup_interno, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_llena_wf_flujo_Click(sender As Object, e As EventArgs) Handles Button_llena_wf_flujo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim RefclasRadicador As New ClassRadicador
            Result = RefclasRadicador.Seleccion_actividad_workflow(Page)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelbotonesradicado)
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelbotonesradicado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_llena_actividad_flujo_Click(sender As Object, e As EventArgs) Handles Button_llena_actividad_flujo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim RefclasRadicador As New ClassRadicador
            Result = RefclasRadicador.Seleccion_flujo_workflow(Page)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelbotonesradicado)
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelbotonesradicado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class