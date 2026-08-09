Imports System.IO

Public Class WebFormGaGestionUnidadConservacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub WebFormGaGestionUnidadConservacion_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Try
            Dim rejava As New Classscrripjava
            If Me.IsPostBack = False Then

                '-------------------------------------------
                '-----Agregar auto completar
                '-------------------------------------------
                Dim refclas As New ClassRadicador
                Dim Result = refclas.agregar_auto_complete(Me.TextBoxCODIGO_UNICO.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "ID_UNIDAD_CONSERVACION")
                Result = refclas.agregar_auto_complete(Me.TextBoxNOMBRE_AREA.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "NOMBRE_AREA")
                Result = refclas.agregar_auto_complete(Me.TextBoxNOMBRE_SERIE.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "NOMBRE_SERIE")
                Result = refclas.agregar_auto_complete(Me.TextBoxNOMBRE_SUBSERIE.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "NOMBRE_SUBSERIE")
                Result = refclas.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_INICIAL.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_INICIAL")
                Result = refclas.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_FINAL.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_FINAL")
                Result = refclas.agregar_auto_complete(Me.TextBoxTEMA_UNIDAD_CONSERVACION.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "TEMA_UNIDAD_CONSERVACION")
                Result = refclas.agregar_auto_complete(Me.TextBoxCONSECUTIVO_UNIDAD_CONSERVACION.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "CODIGO_UNICO")
                Result = refclas.agregar_auto_complete(Me.TextBoxNOMBRE_SUB_AREA.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "NOMBRE_SUB_AREA")
                Dim refclasunidad As New ClassUnidadConservacion
                Result = refclasunidad.lista_tipos_unidades_Combo_inten_vacio(Me.DropDownListtipounidadconservacion)
                If Result <> "YES" Then
                    'rejava.Showscripman_menu(Result, Me.UpdatePaneLconsulta, "ModalPopupExtender_mensaje_personalizado")
                    Label_estado.Text = Label_estado.Text & "|" & Result
                End If
            Else
                Dim refclas As New ClassRadicador
                Dim Result = refclas.agregar_auto_complete(Me.TextBoxCODIGO_UNICO.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "ID_UNIDAD_CONSERVACION")
                Result = refclas.agregar_auto_complete(Me.TextBoxNOMBRE_AREA.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "NOMBRE_AREA")
                Result = refclas.agregar_auto_complete(Me.TextBoxNOMBRE_SERIE.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "NOMBRE_SERIE")
                Result = refclas.agregar_auto_complete(Me.TextBoxNOMBRE_SUBSERIE.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "NOMBRE_SUBSERIE")
                Result = refclas.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_INICIAL.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_INICIAL")
                Result = refclas.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_FINAL.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_FINAL")
                Result = refclas.agregar_auto_complete(Me.TextBoxTEMA_UNIDAD_CONSERVACION.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "TEMA_UNIDAD_CONSERVACION")
                Result = refclas.agregar_auto_complete(Me.TextBoxCONSECUTIVO_UNIDAD_CONSERVACION.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "CODIGO_UNICO")
                Result = refclas.agregar_auto_complete(Me.TextBoxNOMBRE_SUB_AREA.ClientID, Me.Panelcampos, "GetGuiaRadicaconasp", "unidad_conservacion", "NOMBRE_SUB_AREA")
            End If
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim parmaenter As String = "'" + Me.data_grid.ClientID & "','" & Me.hdnEmailID.ClientID & "'"
            Dim scr2 As [String] = "$(document).ready(function () {$().auto_postback(" + parmaenter + ");});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr2, True)
            End If
            Dim script As [String] = "$(document).ready(function(){$('#" + TextBoxFECHA_CREACION_FINAL.ClientID & "').format_date();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", _
                                                                                    TextBoxFECHA_CREACION_FINAL.ClientID))) Then
                ScriptManager.RegisterClientScriptBlock(Me.TextBoxFECHA_CREACION_FINAL, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", _
                                                                                    TextBoxFECHA_CREACION_FINAL.ClientID), script, True)
            End If
            script = "$(document).ready(function(){$('#" + TextBoxFECHA_CREACION_INI.ClientID & "').format_date();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_CREACION_INI.ClientID))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_CREACION_INI.ClientID), script, True)
            End If
            script = "$(document).ready(function(){$('#" + TextBoxFECHA_EXTREMA_FINAL.ClientID & "').format_date();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_FINAL.ClientID))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_FINAL.ClientID), script, True)
            End If
            script = "$(document).ready(function(){$('#" + TextBoxFECHA_EXTREMA_INICIAL.ClientID & "').format_date();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_INICIAL.ClientID))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_INICIAL.ClientID), script, True)
            End If

        Catch ex As Exception
        End Try
    End Sub

    Private Sub WebFormGaGestionUnidadConservacion_PreRenderComplete(sender As Object, e As EventArgs) Handles Me.PreRenderComplete
        Dim clasjava As New Classscrripjava
        Dim result As String = ""
        Dim clasadmonempresa As New ClassAdmonEmpresa
        If Me.IsPostBack = False Then
            If Session.Item("GA_MANAGER_GESTION") = 1 Then
                result = clasadmonempresa.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa, _
                                                                           Me.UpdatePaneLconsulta)
                If result <> "YES" Then
                    clasjava.Show(result)
                    Exit Sub
                End If
                Dim empresa_usuario_gestion As String = ""
                result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion, _
                                                                                 Session.Item("GA_IDUSUARIOGESTION"))
                If result <> "YES" Then
                    clasjava.Showscripman_menu(result, Me.UpdatePaneLconsulta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Me.DropDownListEntidadEmpresa.Items.Count > 0 Then
                    Me.DropDownListEntidadEmpresa.Text = empresa_usuario_gestion
                    Me.UpdatePaneLconsulta.Update()
                End If
            Else
                Dim Class_empresa_gestion_documental As New Class_empresa_gestion_documental
                result = Class_empresa_gestion_documental.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa, _
                                                                                           Me.UpdatePaneLconsulta, _
                                                                                           Session.Item("GA_IDUSUARIOGESTION"))
                If result <> "YES" Then
                    clasjava.Showscripman_menu(result, Me.UpdatePaneLconsulta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim empresa_usuario_gestion As String = ""
                result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion, Session.Item("GA_IDUSUARIOGESTION"))
                If result <> "YES" Then
                    clasjava.Showscripman_menu(result, Me.UpdatePaneLconsulta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Me.DropDownListEntidadEmpresa.Items.Count > 0 Then
                    Me.DropDownListEntidadEmpresa.Text = empresa_usuario_gestion
                End If
            End If
        End If
    End Sub
    Private Sub ButtonConsulta_Click(sender As Object, e As EventArgs) Handles ButtonConsulta.Click
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            If Me.DropDownListEntidadEmpresa.Text = "" Then
                clasjava.Showscripman_menu("Seleccione entidad empresa", Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_Id_Emprea(Me.DropDownListEntidadEmpresa.Text, id_empresa)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim refclaexpediente As New ClassUnidadConservacion
            Result = refclaexpediente.Consulta_unidad_conservacion(Me.TextBoxCODIGO_UNICO.Text, Me.TextBoxFECHA_CREACION_INI.Text, Me.TextBoxFECHA_CREACION_FINAL.Text, _
            Me.TextBoxNOMBRE_AREA.Text, Me.TextBoxNOMBRE_SERIE.Text, Me.TextBoxNOMBRE_SUBSERIE.Text, Me.TextBoxFECHA_EXTREMA_INICIAL.Text, _
             Me.TextBoxFECHA_EXTREMA_FINAL.Text, Me.TextBoxRANGO_EXTREMO_INICIAL.Text, Me.TextBoxRANGO_EXTREMO_FINAL.Text, _
             Me.DropDownListUusuariocreador, 1, Me.data_grid, Me.titulo_label_expedientes, Me.DropDownListtipounidadconservacion.Text, _
              Me.TextBoxDESCRIPCION_UNIDAD_CONSERVACION.Text, id_empresa, Me.UpdateGeneral, Me.hdnEmailID, Me.HiddenEmailconsulta, _
               Me.CheckBoxsolo_expeidente_propio.Checked, Me.CheckBox_Descripcion.Checked, Me.CheckBox_tema.Checked, Me.TextBoxTEMA_UNIDAD_CONSERVACION.Text, _
                Me.TextBoxCONSECUTIVO_UNIDAD_CONSERVACION.Text, Me.TextBoxNOMBRE_SUB_AREA.Text, Me.DropDownListEstado_Expediente.Text)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonRotulo_Click(sender As Object, e As EventArgs) Handles ButtonRotulo.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Try
            Dim Refclas As New ClassUnidadConservacion
            Dim ruta_archivo As String = ""
            If Me.hdnEmailID.Value = "-1" Or Me.hdnEmailID.Value = "" Then
                scripjava.Showscripman_menu("Debe seleccionar el registro", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Genera_rotulo_unidad_conservacion_pdf(Me.hdnEmailID.Value, _
                                                                   Session.Item("GA_IDEMPRESA"), _
                                                                   0, _
                                                                   ruta_archivo)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If ruta_archivo <> "" Then
                    Dim fileinf As New FileInfo(ruta_archivo)
                    If File.Exists(ruta_archivo) Then
                        Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/PUBLIC/DESCARGA/")
                        Dim filecopia As String = ruta_local & fileinf.Name
                        If File.Exists(filecopia) Then
                            Kill(filecopia)
                        End If
                        File.Move(ruta_archivo, filecopia)
                        Me.Hidden_ruta_archivo.Value = "../Temp_Image/PUBLIC/DESCARGA/" & fileinf.Name
                        ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                        updatapanel_iframe.Update()
                    End If
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonImprimirRotulo_Click(sender As Object, e As EventArgs) Handles ButtonImprimirRotulo.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Try
            Dim Refclas As New ClassUnidadConservacion
            Dim ruta_archivo As String = ""
            If Me.hdnEmailID.Value = "-1" Or Me.hdnEmailID.Value = "" Then
                scripjava.Showscripman_menu("Debe seleccionar el registro", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Genera_rotulo_unidad_conservacion_pdf(Me.hdnEmailID.Value, _
                                                                   Session.Item("GA_IDEMPRESA"), _
                                                                   0, _
                                                                   ruta_archivo)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
                Me.ifimpre_post_.Attributes.Add("src", "../Gestion/WebFormimpresionfile.aspx")
                UpdatePaneliframe_post.Update()
                ModalPopupExtenderimpre_post.Show()
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_eliminar_unidad_conservacion_Click(sender As Object, e As EventArgs) Handles Button_eliminar_unidad_conservacion.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Try
            Dim Refclas As New ClassUnidadConservacion
            Dim ruta_archivo As String = ""
            If Me.hdnEmailID.Value = "-1" Or Me.hdnEmailID.Value = "" Then
                scripjava.Showscripman_menu("Debe seleccionar el registro", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.Hidden_promp_mensaje.Value = "0" Then Exit Sub
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 0 Then
                If Session.Item("GA_ELIMINA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para eliminar unidad de conservación", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Verifica_propiedad_usuario_unidad_conservacion(Me.hdnEmailID.Value, Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim stru() As unidad_conservacion = Nothing
            Result = Refclas.Listar_datos_Unidad_Conservacion_estructura(Me.hdnEmailID.Value, stru)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru(0).TIPO_UNIDAD_CONSERVACION = 1 Then
                Result = Refclas.Elimina_unidad_conservacion_tipo_contenedor_expediente_lista(hdnEmailID.Value, _
                   Session.Item("GA_IDUSUARIOGESTION"), Session.Item("GA_LOGINUSUARIOGESTION"), Session.Item("ip_host_name"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Else
                    Hidden_result.Value = "YES"
                End If
            Else
                Result = Refclas.Eliminar_unidad_conservacion(hdnEmailID.Value, _
                   Session.Item("GA_IDUSUARIOGESTION"), Session.Item("GA_LOGINUSUARIOGESTION"), Session.Item("ip_host_name"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Else
                    Hidden_result.Value = "YES"
                End If
            End If
           
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_Editar_unidad_conservacion_Click(sender As Object, e As EventArgs) Handles Button_Editar_unidad_conservacion.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Try
            Dim Refclas As New ClassUnidadConservacion
            If Me.hdnEmailID.Value = "-1" Or Me.hdnEmailID.Value = "" Then
                scripjava.Showscripman_menu("Debe seleccionar el registro", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru() As unidad_conservacion = Nothing
            Result = Refclas.Listar_datos_Unidad_Conservacion_estructura(Me.hdnEmailID.Value, stru)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            If Session.Item("GA_MANAGER_GESTION") = 1 Then
            Else
                If Session.Item("GA_EDITA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para editar unidad de conservación", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Verifica_propiedad_usuario_unidad_conservacion(Me.hdnEmailID.Value, _
                 Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            If stru(0).TIPO_UNIDAD_CONSERVACION = 1 Then
                Session.Item("GA_ID_UNIDAD_CONTENEDORA") = Me.hdnEmailID.Value
                Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = Me.DropDownListEntidadEmpresa.Text
                Me.Hidden_estado_editar_unidad.Value = "YES"
                Me.Iframe_agregar_expdiente_popup_.Attributes.Add("src", "../Gestion/WebFormGaEditarunidadconservaexpe.aspx")
                Me.UpdatePanel_agregar_expdiente_popup.Update()
                Me.ModalPopupExtende_agregar_expdiente_popup.Show()
            End If
           
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
        
    End Sub

    Private Sub Button_actualizar_unidad_Click(sender As Object, e As EventArgs) Handles Button_actualizar_unidad.Click
        Dim Refclas As New ClassUnidadConservacion
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim value_id As Integer = Me.hdnEmailID.Value
            Result = Refclas.Consulta_unidad_conservacion_post(Me.UpdateGeneral, _
                                                               Me.hdnEmailID, _
                                                               Me.HiddenEmailconsulta, _
                                                               Me.data_grid, _
                                                               Me.titulo_label_expedientes)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
            End If
            Me.hdnEmailID.Value = value_id
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_ubicacio_unidad_conservacion_Click(sender As Object, e As EventArgs) Handles Button_ubicacio_unidad_conservacion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            If Me.hdnEmailID.Value = "0" Or Me.hdnEmailID.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para mostrar la ubicación toponimica", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_Ubicacion_unidad_conservacion_por_codigo_unico(Me.hdnEmailID.Value, _
                                                                                    Me.TreeViewArchivo_u_b_t, _
                                                                                    "")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanelViewArchivo_u_b_t.Update()
                Me.ModalPopupExtende_ubicacion_toponimica_expediente_popup.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ButtonRestaurar_Click(sender As Object, e As EventArgs) Handles ButtonRestaurar.Click
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Try
          
            Dim refclaexpediente As New ClassUnidadConservacion
            Result = refclaexpediente.Limpia_campos_consulta_unidad_conservacion(Me.Panelcampos, _
                                                                                 Me.UpdatePaneLconsulta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_cancelar_agregar_unidad_conservacion_Click(sender As Object, e As EventArgs) Handles Button_cancelar_agregar_unidad_conservacion.Click
        Me.ModalPopupExtende_agregar_unidad_conservacion.Hide()
    End Sub

    Private Sub data_grid_PageIndexChanged(sender As Object, e As EventArgs) Handles data_grid.PageIndexChanged
        Try

        Catch ex As Exception

        End Try
    End Sub

    Private Sub data_grid_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid.PageIndexChanging
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Try
            data_grid.PageIndex = e.NewPageIndex
            Dim refclaexpediente As New ClassUnidadConservacion
            Result = refclaexpediente.Consulta_unidad_conservacion_paging(Me.data_grid, _
                                                                              Me.titulo_label_expedientes, _
                                                                              Me.hdnEmailID, _
                                                                              Me.UpdateGeneral)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub

    Private Sub Button_actualiza_expdientes_agregados_Click(sender As Object, e As EventArgs) Handles Button_actualiza_expdientes_agregados.Click
        Dim Result As String = ""
        Dim clasjava As New Classscrripjava
        Try

            Dim refclaexpediente As New ClassUnidadConservacion
            Result = refclaexpediente.Consulta_unidad_conservacion_paging(Me.data_grid, _
                                                                          Me.titulo_label_expedientes, _
                                                                          Me.hdnEmailID, _
                                                                          Me.UpdateGeneral)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class