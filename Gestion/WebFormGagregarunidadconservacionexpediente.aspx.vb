Public Class WebFormGagregarunidadconservacionexpediente
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Private Sub FormGaAgregarExpediente_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim parmaenter As String = "'" + "" & "','" & "" & "'"
            Dim scr2 As [String] = "$(document).ready(function () {$().auto_postback(" + parmaenter + ");});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr2, True)
            End If
            Dim script As [String] = "$(document).ready(function(){$('#" + TextBoxFECHA_EXTREMA_FINAL.ClientID & "').format_date();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_FINAL.ClientID))) Then
                ScriptManager.RegisterClientScriptBlock(Me.TextBoxFECHA_EXTREMA_FINAL, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_FINAL.ClientID), script, True)
            End If
            script = "$(document).ready(function(){$('#" + TextBoxFECHA_EXTREMA_INICIAL.ClientID & "').format_date();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_INICIAL.ClientID))) Then
                ScriptManager.RegisterClientScriptBlock(Me.TextBoxFECHA_EXTREMA_INICIAL, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_INICIAL.ClientID), script, True)
            End If
            If Page.IsPostBack = False Then
                Dim Result As String = ""
                Dim Refclas As New ClassAdmonEmpresa
                Dim Resfclas As New ClassGaTipoDocumental
                Dim Refclas_unidadConservacion As New ClassUnidadConservacion
                Dim id_empresa As Integer = 0
                Dim refclas_rad As New ClassRadicador
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxCodigoManual.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "CODIGO_UNICO")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_FINAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_FINAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_INICIAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_INICIAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxTEMA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "TEMA_UNIDAD_CONSERVACION")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxASUNTO_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "DESCRIPCION_UNIDAD_CONSERVACION")
                id_empresa = Session.Item("GA_IDEMPRESA")
                Hidden_id_empresa.Value = id_empresa
                Result = Refclas_unidadConservacion.Activa_registrar_unidad_conservacion(DropDownListorganigrama,
                                                                                        DropDownList_tipo_unidad_contenedora,
                                                                                        DropDownListArea,
                                                                                        DropDownList_instrumento,
                                                                                        TextBox_ayuda_conetedora,
                                                                                        update_panel_controles)
                If Result <> "YES" Then
                    Labelresultado.Text = Result
                End If

            Else
                Dim refclas_rad As New ClassRadicador
                Dim Result As String = refclas_rad.agregar_auto_complete(Me.TextBoxCodigoManual.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "CODIGO_UNICO")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_FINAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_FINAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_INICIAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_INICIAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxTEMA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "TEMA_UNIDAD_CONSERVACION")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxASUNTO_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "DESCRIPCION_UNIDAD_CONSERVACION")
            End If
        Catch ex As Exception
            Labelresultado.Text = Labelresultado.Text & ex.Message
        End Try
    End Sub
    Private Sub Button_selecion_organigrama_Click(sender As Object, e As EventArgs) Handles Button_selecion_organigrama.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            If Me.DropDownListorganigrama.SelectedItem Is Nothing Then
                Exit Sub
            End If
            Dim id_organigrama As Integer = Me.DropDownListorganigrama.SelectedItem.Value
            Result = Refclas.Seleccion_organigrama_unidad_conservacion(id_organigrama, Me.DropDownListArea, _
                                                                     Me.DropDownListSerie, Me.DropDownListSubserie, _
                                                                     Me.DropDownList_instrumento, Me.update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
           
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_selecion_area_Click(sender As Object, e As EventArgs) Handles Button_selecion_area.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            If Me.DropDownListArea.SelectedItem Is Nothing Then Exit Sub
            Dim id_area As Integer = Me.DropDownListArea.SelectedItem.Value
            Dim id_instrumento As Integer = 0
            If Not Me.DropDownList_instrumento.SelectedItem Is Nothing Then
                id_instrumento = Me.DropDownList_instrumento.SelectedItem.Value
            End If
            Result = Refclas.Seleccion_area_departamento_unidad_conservacion(id_area, id_instrumento, _
                                                                           Me.DropDownListSerie, Me.DropDownListSubserie, _
                                                                           Me.update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
          
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_selecion_serie_Click(sender As Object, e As EventArgs) Handles Button_selecion_serie.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            If Me.DropDownListSerie.SelectedItem Is Nothing Then Exit Sub
            Dim id_serie As Integer = Me.DropDownListSerie.SelectedItem.Value
            Result = Refclas.Seleccion_serie_documental_unidad_conservacion(id_serie, Me.DropDownListSubserie, Me.update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ButtonRestaurar_Click(sender As Object, e As EventArgs) Handles ButtonRestaurar.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclasunidad As New ClassUnidadConservacion
            Result = Refclasunidad.Limpia_campos_unidad_conservacion(Me.table_controles)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclasunidad.Activa_registrar_unidad_conservacion(DropDownListorganigrama, _
                                                                                        DropDownList_tipo_unidad_contenedora, _
                                                                                        DropDownListArea, _
                                                                                        DropDownList_instrumento, _
                                                                                        TextBox_ayuda_conetedora, _
                                                                                        update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        Finally
            Me.update_panel_controles.Update()
        End Try
    End Sub
    Private Sub Button1_seleccion_expediente_manual_Click(sender As Object, e As EventArgs) Handles Button1_seleccion_expediente_manual.Click
        If Me.CheckBoxActivaCodigomanual.Checked = True Then
            Me.TextBoxCodigoManual.ReadOnly = False
            Me.TextBoxCodigoManual.BackColor = Drawing.Color.White
        Else
            Me.TextBoxCodigoManual.ReadOnly = True
            Me.TextBoxCodigoManual.BackColor = Drawing.Color.Gray
        End If
    End Sub

    Private Sub Button_lista_ayuda_tipo_unidad_Click(sender As Object, e As EventArgs) Handles Button_lista_ayuda_tipo_unidad.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Refclas_u As New ClassUnidadConservacion
            Dim result As String = ""
            result = Refclas_u.Retorna_descripcion_tipo_unidad_conservacion(Me.DropDownList_tipo_unidad_contenedora.Text, Me.TextBox_ayuda_conetedora.Text)
            If result <> "YES" Then
                classcrip.Showscripman_menu(result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonAceptar_Click(sender As Object, e As EventArgs) Handles ButtonAceptar.Click
        Dim classcrip As New Classscrripjava
        Try
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                classcrip.Showscripman_menu("El usuario  no tiene asociado un usuario de gestión ", Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 0 Then
                If Session.Item("GA_REGISTRA_UNIDAD_CONSERVACION") = 0 Then
                    classcrip.Showscripman_menu("El usuario  no tiene permiso para registrar unidad de conservación", Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            Dim refclasempresa As New ClassAdmonEmpresa
            '---------------------------------------------------------------------------------------
            'Asigna 0.id entrepaño - 1. id tipo unidad- 2. tipo unidad - 3. descripcion -4 Estado archivo
            '---------------------------------------------------------------------------------------
            Dim split_datos() As String = Hidden_tipo_unidad_seleccion.Value.ToString.Split("|")
            Dim estado_codigo_unico As Integer = 0
            If Me.CheckBoxActivaCodigomanual.Checked = True Then
                estado_codigo_unico = 1
            Else
                estado_codigo_unico = 0
            End If
            If Me.DropDownList_tipo_unidad_contenedora.Text = "" Then
                classcrip.Showscripman_menu("Debe informar el tipo de unidad contenedora", Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim identificacion_tipo As Integer = 0
            '-----------------------------------------------------------
            'Retorna tipo unidad de conservacion
            '-----------------------------------------------------------
            Result = Refclas.Retorna_id_tipo_unidad_almacenamiento_expediente(Me.DropDownList_tipo_unidad_contenedora.Text, _
                                                                              1, _
                                                                              identificacion_tipo)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_empresa As Integer = Hidden_id_empresa.Value
            Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = ""
            Session.Item("GA_ID_UNIDAD_CONTENEDORA") = 0
            Dim id_organigrama As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim id_instrumento As Integer = 0
            Dim nombre_organigrama As String = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim nombre_area As String = ""
            Dim nombre_instrumento As String = ""
            If Not Me.DropDownListorganigrama.SelectedItem Is Nothing Then
                id_organigrama = Me.DropDownListorganigrama.SelectedItem.Value
                nombre_organigrama = Me.DropDownListorganigrama.SelectedItem.Text
            End If
            If Not Me.DropDownListArea.SelectedItem Is Nothing Then
                id_area = Me.DropDownListArea.SelectedItem.Value
                nombre_area = Me.DropDownListArea.SelectedItem.Text
            End If
            If Not Me.DropDownListSerie.SelectedItem Is Nothing Then
                id_serie = Me.DropDownListSerie.SelectedItem.Value
                nombre_serie = Me.DropDownListSerie.SelectedItem.Text
            End If
            If Not Me.DropDownListSubserie.SelectedItem Is Nothing Then
                id_sub_serie = Me.DropDownListSubserie.SelectedItem.Value
                nombre_sub_serie = Me.DropDownListSubserie.SelectedItem.Text
            End If
            If Not Me.DropDownList_instrumento.SelectedItem Is Nothing Then
                id_instrumento = Me.DropDownList_instrumento.SelectedItem.Value
                nombre_instrumento = Me.DropDownList_instrumento.SelectedItem.Text
            End If
            Result = Refclas.Registrar_Unidad_Conservacion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                           Val(split_datos(0)), _
                                                           Me.TextBoxCodigoManual.Text, _
                                                           estado_codigo_unico, _
                                                           1, _
                                                           identificacion_tipo, _
                                                           Me.TextBoxFECHA_EXTREMA_INICIAL.Text, _
                                                           Me.TextBoxFECHA_EXTREMA_FINAL.Text, _
                                                           Me.TextBoxRANGO_EXTREMO_INICIAL.Text, _
                                                           Me.TextBoxRANGO_EXTREMO_FINAL.Text, _
                                                           Me.TextBoxTEMA_EXPEDIENTE.Text, _
                                                           Me.TextBoxASUNTO_EXPEDIENTE.Text, _
                                                           1, _
                                                           id_empresa, _
                                                           nombre_organigrama, _
                                                           nombre_area, _
                                                           nombre_serie, _
                                                           nombre_sub_serie, _
                                                           Session.Item("GA_ID_UNIDAD_CONTENEDORA"), _
                                                           Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA"), _
                                                           Me.DropDownListsub_seccion.Text, _
                                                           id_organigrama, id_area, _
                                                           id_serie, _
                                                           id_sub_serie, _
                                                           id_instrumento)
            If Result <> "YES" Then
                Hidden_resultado.Value = ""
                classcrip.Showscripman_menu(Result, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resultado.Value = "YES"
            End If

        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub TreeViewArchivo_r_u_e_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeViewArchivo_r_u_e.SelectedNodeChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Tagform As String = sender.selectedvalue()
            Dim Result As String = ""
            Dim Refclas As New ClassGestionArchivo
            Result = Refclas.Seleccion_Treview_archivar(Tagform, Me.TreeViewArchivo_r_u_e.SelectedNode, Me.DropDownListEntidadEmpresa_r_u_e.Text, Me.TreeViewArchivo_r_u_e.SelectedNode.Value, Me.TreeViewArchivo_r_u_e.SelectedNode.Text, Me.TreeViewArchivo_r_u_e)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanelViewArchivo_r_u_e.Update()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ButtonButtonEditar_Click(sender As Object, e As EventArgs) Handles ButtonButtonEditar.Click
        'Dim scripjava As New Classscrripjava
        'Try
        '    If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
        '        scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    End If
        '    Dim Result As String = ""
        '    Dim Refclas As New ClassUnidadConservacion
        '    If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then Exit Sub
        '    If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip <> "UNIDAD CONTENEDORA EXPEDIENTE" Then
        '        Exit Sub
        '    End If
        '    If Session.Item("GA_MANAGER_GESTION") = 1 Then
        '    Else
        '        If Session.Item("GA_EDITA_UNIDAD_CONSERVACION") = 0 Then
        '            scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para editar unidad de conservación", Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        '            Exit Sub
        '        End If
        '        Result = Refclas.Verifica_propiedad_usuario_unidad_conservacion(Me.TreeViewArchivo_r_u_e.SelectedNode.Value, _
        '         Session.Item("GA_IDUSUARIOGESTION"))
        '        If Result <> "YES" Then
        '            scripjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        '            Exit Sub
        '        End If
        '    End If
        '    Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.Split("|")
        '    Session.Item("GA_ID_UNIDAD_CONTENEDORA") = split(0)
        '    Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = Me.DropDownListEntidadEmpresa.Text
        '    Me.Iframe_agregar_unidad_conservacion_popup.Attributes.Add("src", "../Gestion/WebFormGaEditarunidadconservaexpe.aspx")
        '    Me.UpdatePanel_agregar_unidad_conservacion_popup.Update()
        '    Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Show()
        'Catch ex As Exception
        '    scripjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        'End Try
    End Sub
    Private Sub Button_archivar_Click(sender As Object, e As EventArgs) Handles Button_archivar.Click
        Dim clasjava As New Classscrripjava
        'Try
        '    Dim Result As String = ""
        '    Dim Refclas As New ClassGaExpediente
        '    If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
        '        clasjava.Showscripman_menu("Debe seleccionar un expediente para archivar", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    End If
        '    If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then Exit Sub
        '    If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
        '        Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.ToString.Split("|")
        '        Result = Refclas.Archiva_expediente_unidad_contenedora_Archivado(split(0), Me.hdnEmailID.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
        '                HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), HttpContext.Current.Session.Item("ip_host_name"))
        '        If Result <> "YES" Then
        '            clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        '            Exit Sub
        '        Else
        '            ModalPopupExtende_reubicar_unidad_expediente_popup.Hide()
        '        End If
        '    End If
        '    If InStr(Me.TreeViewArchivo_r_u_e.SelectedNode.Text, "Entrepaño") > 0 Then
        '        Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.ToString.Split("|")
        '        Result = Refclas.Archiva_expediente_en_entrepano_archivado(split(0), Me.hdnEmailID.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
        '               HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), HttpContext.Current.Session.Item("ip_host_name"))
        '        If Result <> "YES" Then
        '            clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        '            Exit Sub
        '        Else
        '            ModalPopupExtende_reubicar_unidad_expediente_popup.Hide()
        '        End If
        '    End If
        'Catch ex As Exception
        '    clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        'End Try

    End Sub
    Private Sub Button_actualizar_unidad_Click(sender As Object, e As EventArgs) Handles Button_actualizar_unidad.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.TreeViewArchivo_r_u_e.SelectedNode.Text = Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA")
            Me.UpdatePanelViewArchivo_r_u_e.Update()
            Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Hide()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonEliminar_unidad_contendora_Click(sender As Object, e As EventArgs) Handles ButtonEliminar_unidad_contendora.Click
        Dim scripjava As New Classscrripjava
        Try

            Dim Result As String = ""
            Dim refclas As New ClassUnidadConservacion
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then
                scripjava.Showscripman_menu("Debe seleccionar la unidad contenedora a eliminar", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim spli_unidad_contenedora() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.Split("|")
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 0 Then
                If Session.Item("GA_ELIMINA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para eliminar unidad de conservación", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = refclas.Verifica_propiedad_usuario_unidad_conservacion(Val(spli_unidad_contenedora(0)), Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If


            If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip <> "UNIDAD CONTENEDORA EXPEDIENTE" Then
                scripjava.Showscripman_menu("El tipo de unidad no se puede eliminar", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '--------------------------------------------------------------
            'Elimina el tipo de unidad contenedora de expediente
            '--------------------------------------------------------------
            If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                If Hidden_result_eliminar.Value = "1" Then
                    Result = refclas.Elimina_unidad_conservacion_tipo_contenedor_expediente(Val(spli_unidad_contenedora(0)), _
                    Me.TreeViewArchivo_r_u_e.SelectedNode, Session.Item("GA_IDUSUARIOGESTION"), Me.TreeViewArchivo_r_u_e, Session.Item("GA_LOGINUSUARIOGESTION"), Session.Item("ip_host_name"), UpdatePanelViewArchivo_r_u_e)
                    If Result <> "YES" Then
                        scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    End If
                    Hidden_result_eliminar.Value = "0"
                    Me.UpdatePanelViewArchivo_r_u_e.Update()
                End If
            End If


        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_activa_archivar_unidad_Click(sender As Object, e As EventArgs) Handles Button_activa_archivar_unidad.Click
        Dim Refclas_empresa As New ClassAdmonEmpresa
        Dim scripjava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaExpediente
            Dim id_empresa_gestion As Integer = 0
            Dim Result As String = ""
            If Session.Item("GA_MANAGER_GESTION") <> 1 Then
                If Session.Item("GA_ARCHIVA_EXPEDIENTES") = 0 Then
                    scripjava.Showscripman_menu("Usuario sin permisos para archivar expediente", Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                'Result = Refclas.Verifica_propiedad_usuario_expediente(Me.hdnEmailID.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                'If Result <> "YES" Then
                '    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                '    Exit Sub
                'End If
            End If
            'If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
            '    scripjava.Showscripman_menu("Debe seleccionar un expediente para archivar", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            'If Me.DropDownListEntidadEmpresa.Text = "" Then
            '    scripjava.Showscripman_menu("Debe seleccionar la empresa de gestión", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            'Result = Refclas_empresa.Retorna_Id_Emprea(Me.DropDownListEntidadEmpresa.Text, _
            'id_empresa_gestion)
            'If Result <> "YES" Then
            '    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Result = Refclas_empresa.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa_r_u_e, Me.UpdatePanelEntidad_r_u_e)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'Me.DropDownListEntidadEmpresa_r_u_e.Text = Me.DropDownListEntidadEmpresa.Text
            Me.UpdatePanelEntidad_r_u_e.Update()
            Dim refclas_unidad As New ClassGestionArchivo
            'If Me.HiddenField_estado_ubicacion.Value <> "YES" Then
            Result = refclas_unidad.Listar_Entidad_Archivo_Edificio(Me.TreeViewArchivo_r_u_e, Me.DropDownListEntidadEmpresa_r_u_e.Text)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                'Me.HiddenField_estado_ubicacion.Value = "YES"
                UpdatePanelViewArchivo_r_u_e.Update()
            End If
            'End If
            ModalPopupExtende_reubicar_unidad_expediente_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub DropDownList_instrumento_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_instrumento.SelectedIndexChanged
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            Dim id_area As Integer = 0
            Dim id_instrumento As Integer = 0
            Dim id_organigrama As Integer = 0
            If Not Me.DropDownListArea.SelectedItem Is Nothing Then
                id_area = Me.DropDownListArea.SelectedItem.Value
            End If
            If Not Me.DropDownList_instrumento.SelectedItem Is Nothing Then
                id_instrumento = Me.DropDownList_instrumento.SelectedItem.Value
            End If
            If Not Me.DropDownListorganigrama.SelectedItem Is Nothing Then
                id_organigrama = Me.DropDownListorganigrama.SelectedItem.Value
            End If
            Result = Refclas.Seleccion_instrumento_consrvacion(id_area, _
                                                               id_organigrama, _
                                                               id_instrumento, _
                                                               Me.DropDownListSerie, _
                                                               Me.DropDownListSubserie, _
                                                               Me.update_panel_controles)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.update_panel_controles)
        End Try
    End Sub
End Class