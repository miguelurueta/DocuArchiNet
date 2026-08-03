Imports System.IO

Public Class WebFormGatoponimica
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If

        Dim result As String = ""
        Dim clasadmonempresa As New ClassAdmonEmpresa
        Try
            If Me.IsPostBack = False Then
                If Session.Item("GA_MANAGER_GESTION") = 1 Then
                    result = clasadmonempresa.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa, _
                                                                               Me.UpdatePanelEntidadEmpresa)
                    If result <> "YES" Then
                        Me.Label_estado.Text = result
                        Exit Sub
                    End If

                    Dim empresa_usuario_gestion As String = ""
                    result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion, _
                                                                                     Session.Item("GA_IDUSUARIOGESTION"))
                    If result <> "YES" Then
                        Me.Label_estado.Text = result
                        Exit Sub
                    End If
                    If Me.DropDownListEntidadEmpresa.Items.Count > 0 Then
                        Me.DropDownListEntidadEmpresa.Text = empresa_usuario_gestion
                        Me.UpdatePanelEntidadEmpresa.Update()
                    End If
                Else
                    Dim Class_empresa_gestion_documental As New Class_empresa_gestion_documental
                    result = Class_empresa_gestion_documental.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa, _
                                                                                               Me.UpdatePanelEntidadEmpresa, _
                                                                                               Session.Item("GA_IDUSUARIOGESTION"))
                    If result <> "YES" Then
                        Me.Label_estado.Text = result
                        Exit Sub
                    End If
                    Dim empresa_usuario_gestion As String = ""
                    result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion, _
                                                                                     Session.Item("GA_IDUSUARIOGESTION"))
                    If result <> "YES" Then
                        Me.Label_estado.Text = result
                        Exit Sub
                    End If
                    If Me.DropDownListEntidadEmpresa.Items.Count > 0 Then
                        Me.DropDownListEntidadEmpresa.Text = empresa_usuario_gestion
                    End If
                End If
                Dim refclas_unidad As New ClassGestionArchivo
                result = refclas_unidad.Listar_Entidad_Archivo_Edificio(Me.TreeViewArchivo, _
                                                                        Me.DropDownListEntidadEmpresa.Text)
                If result <> "YES" Then
                    Me.Label_estado.Text = result
                    Exit Sub
                Else
                    UpdatePanelViewArchivo.Update()
                End If
                result = refclas_unidad.Lista_nodes_piso_archivo_trenode(Me.TreeViewArchivo, _
                                                                         Me.DropDownListEntidadEmpresa.Text)
                If result <> "YES" Then
                    Me.Label_estado.Text = result
                    Exit Sub
                Else
                    UpdatePanelViewArchivo.Update()
                End If
            End If
        Catch ex As Exception
            Me.Label_estado.Text = ex.Message
        End Try
    End Sub


    Private Sub Button_listar_edificio_Click(sender As Object, e As EventArgs) Handles Button_listar_edificio.Click
        Dim refclas_unidad As New ClassGestionArchivo
        Dim Refclasjava As New Classscrripjava
        Try
            Dim result = refclas_unidad.Listar_Entidad_Archivo_Edificio(Me.TreeViewArchivo, Me.DropDownListEntidadEmpresa.Text)
            If result <> "YES" Then
                Refclasjava.Showscripman_menu(result, Me.UpdatePanel_botones_comandos, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                UpdatePanelViewArchivo.Update()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_comandos, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    

    Private Sub TreeViewunidad_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeViewunidad.SelectedNodeChanged
        Dim scripjava As New Classscrripjava
        Try
            Dim refclas As New ClassGestionArchivo
            Dim result As String = ""
            result = refclas.Seleccion_treview_principal_entrepaño(Me.TreeViewunidad, _
                                                                   Me.UpdatePanel_unidad_treview_unidad)
            If result <> "YES" Then
                scripjava.Showscripman_menu(result, Me.UpdatePanel_unidad_treview_unidad, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_unidad_treview_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonAgregar_Click(sender As Object, e As EventArgs) Handles ButtonAgregar.Click
        Dim scripjava As New Classscrripjava
        Try
            If Me.DropDownListEntidadEmpresa.Text = "" Then
                scripjava.Showscripman_menu("Por favor seleccione la empresa de gestión", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim node_tag As String = ""
            If Me.TreeViewArchivo.Nodes.Count > 0 Then
                If Me.TreeViewArchivo.SelectedNode Is Nothing Then
                    Exit Sub
                End If
                node_tag = Me.TreeViewArchivo.SelectedNode.Value
            Else
                Exit Sub
            End If
            If InStr(node_tag, "ENTREPAÑO") <= 0 Then
                Exit Sub
            End If
            If Me.TreeViewunidad.SelectedNode Is Nothing Then
                scripjava.Showscripman_menu("Por favor seleccione el entrepaño", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub

            End If
            If InStr(Me.TreeViewunidad.SelectedNode.Value, "ENTREPAÑO") <= 0 Then
                scripjava.Showscripman_menu("Por favor seleccione el entrepaño", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario workflow no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 1 Then
            Else
                If Session.Item("GA_REGISTRA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permisos para agregar unidad de conservación", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Me.Hidden_tipo_unidad_seleccion.Value = node_tag
            Me.Iframe_agregar_expdiente_popup_.Attributes.Add("src", "../Gestion/WebFormGagregarunidadconservacionexpediente.aspx")
            Me.UpdatePanel_agregar_expdiente_popup.Update()
            Me.ModalPopupExtende_agregar_expdiente_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_agrega_unidad_conservacion_interface_Click(sender As Object, e As EventArgs) Handles Button_agrega_unidad_conservacion_interface.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim trenode As New TreeNode
            trenode.Text = Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA")
            trenode.Value = Session.Item("GA_ID_UNIDAD_CONTENEDORA")
            trenode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE"
            trenode.ImageUrl = "../Gestion/imagenes/caja_exp.png"
            Me.TreeViewunidad.SelectedNode.ChildNodes.Add(trenode)
            Me.TreeViewunidad.SelectedNode.ExpandAll()
            Me.UpdatePanel_unidad_treview_unidad.Update()
            Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = ""
            Session.Item("GA_ID_UNIDAD_CONTENEDORA") = 0
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonEliminar_Click(sender As Object, e As EventArgs) Handles ButtonEliminar.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim refclas As New ClassUnidadConservacion
            If Me.TreeViewunidad.SelectedNode Is Nothing Then
                scripjava.Showscripman_menu("Debe seleccionar la unidad contenedora a eliminar", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 0 Then
                If Session.Item("GA_ELIMINA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para eliminar unidad de conservación", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = refclas.Verifica_propiedad_usuario_unidad_conservacion(Me.TreeViewunidad.SelectedNode.Value, Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If


            If Me.TreeViewunidad.SelectedNode.ToolTip <> "UNIDAD CONTENEDORA EXPEDIENTE" Then
                scripjava.Showscripman_menu("El tipo de unidad no se puede eliminar", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '--------------------------------------------------------------
            'Elimina el tipo de unidad contenedora de expediente
            '--------------------------------------------------------------
            If Me.TreeViewunidad.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                If HiddenField_botones_respuesta.Value = "1" Then
                    Result = refclas.Elimina_unidad_conservacion_tipo_contenedor_expediente(Me.TreeViewunidad.SelectedNode.Value, _
                    Me.TreeViewunidad.SelectedNode, Session.Item("GA_IDUSUARIOGESTION"), Me.TreeViewunidad, Session.Item("GA_LOGINUSUARIOGESTION"), Session.Item("ip_host_name"), UpdatePanel_unidad_treview_unidad)
                    If Result <> "YES" Then
                        scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    End If
                    HiddenField_botones_respuesta.Value = "-1"
                    Me.UpdatePanel_unidad_treview_unidad.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonButtonEditar_Click(sender As Object, e As EventArgs) Handles ButtonButtonEditar.Click
        Dim scripjava As New Classscrripjava
        Try
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            If Me.TreeViewunidad.SelectedNode Is Nothing Then Exit Sub
            If Me.TreeViewunidad.SelectedNode.ToolTip <> "UNIDAD CONTENEDORA EXPEDIENTE" Then
                scripjava.Showscripman_menu("Debe seleccionar unidad contenedora para editar", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 1 Then
            Else
                If Session.Item("GA_EDITA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para editar unidad de conservación", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Verifica_propiedad_usuario_unidad_conservacion(Me.TreeViewunidad.SelectedNode.Value, _
                                                                                Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Session.Item("GA_ID_UNIDAD_CONTENEDORA") = Me.TreeViewunidad.SelectedNode.Value
            Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = Me.DropDownListEntidadEmpresa.Text
            Me.Iframe_agregar_expdiente_popup_.Attributes.Add("src", "../Gestion/WebFormGaEditarunidadconservaexpe.aspx")
            Me.UpdatePanel_agregar_expdiente_popup.Update()
            Me.ModalPopupExtende_agregar_expdiente_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_actualizar_unidad_Click(sender As Object, e As EventArgs) Handles Button_actualizar_unidad.Click
        Me.TreeViewunidad.SelectedNode.Text = Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA")
        Me.UpdatePanel_unidad_treview_unidad.Update()
        ModalPopupExtende_agregar_expdiente_popup.Hide()
    End Sub

    Protected Sub ButtonReubicar_Click(sender As Object, e As EventArgs) Handles ButtonReubicar.Click
        Dim scripjava As New Classscrripjava
        Try
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 0 Then
                If Session.Item("GA_TRANSLADO_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para reubicar o trasladar unidad de conservación", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            If Me.TreeViewunidad.SelectedNode Is Nothing Then Exit Sub
            If Me.TreeViewunidad.SelectedNode.ToolTip <> "Expediente" And Me.TreeViewunidad.SelectedNode.ToolTip <> "UNIDAD CONTENEDORA EXPEDIENTE" Then
                Exit Sub
            End If
            Dim Refclas_empresa As New ClassAdmonEmpresa
            Dim id_empresa_gestion As Integer = 0
            Dim Result As String = Refclas_empresa.Retorna_Id_Emprea(Me.DropDownListEntidadEmpresa.Text, _
                                                                     id_empresa_gestion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_empresa.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa_r_u_e, _
                                                                      Me.UpdatePanelEntidad_r_u_e)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownListEntidadEmpresa_r_u_e.Text = Me.DropDownListEntidadEmpresa.Text
            Me.UpdatePanelEntidad_r_u_e.Update()
            Dim refclas_unidad As New ClassGestionArchivo
            If Me.HiddenField_estado_ubicacion.Value <> "YES" Then
                Result = refclas_unidad.Listar_Entidad_Archivo_Edificio(Me.TreeViewArchivo_r_u_e, _
                                                                        Me.DropDownListEntidadEmpresa_r_u_e.Text)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.HiddenField_estado_ubicacion.Value = "YES"
                    UpdatePanelViewArchivo_r_u_e.Update()
                End If
            End If
            ModalPopupExtende_reubicar_unidad_expediente_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    

    Private Sub Button_reubicar_Click(sender As Object, e As EventArgs) Handles Button_reubicar.Click
        Dim scripjava As New Classscrripjava
        Dim Refclas_unidad_conservacion As New ClassUnidadConservacion
        Dim Result As String = ""
        If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
            scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End If
        If Session.Item("GA_MANAGER_GESTION") = 0 Then
            If Session.Item("GA_ARCHIVA_EXPEDIENTES") = 0 Then
                scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para archivar expedientes", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        End If
        If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then Exit Sub
        Dim Tipo_unidad As String = ""
        If Me.TreeViewunidad.SelectedNode.ToolTip = "Expediente" Or Me.TreeViewunidad.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
            Result = Refclas_unidad_conservacion.Reubica_expediente_unidad_conservacion(Me.TreeViewunidad.SelectedNode.ToolTip, _
                                                                                        Me.TreeViewunidad, Me.TreeViewArchivo_r_u_e, _
                                                                                        UpdatePanel_unidad_treview_unidad, _
                                                                                        Me.TreeViewArchivo_r_u_e, _
                                                                                        Me.UpdatePanelViewArchivo_r_u_e)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanelViewArchivo_r_u_e.Update()
                Me.UpdatePanel_unidad_treview_unidad.Update()
                ModalPopupExtende_reubicar_unidad_expediente_popup.Hide()
            End If
        End If
    End Sub

    Protected Sub ButtonRotulo_Click(sender As Object, e As EventArgs) Handles ButtonRotulo.Click
        Dim Refclas As New ClassUnidadConservacion
        Dim scripjava As New Classscrripjava
        Try
            Dim ruta_archivo As String = ""
            Dim Result As String = ""
            Dim id_expediente As Integer = 0
            Dim id_unidad As Integer = 0
            If Me.TreeViewunidad.SelectedNode Is Nothing Then Exit Sub
            '----------------------------------------------------------
            'Imprime rotulo unidad contenedora expediente
            '---------------------------------------------------------
            If Me.TreeViewunidad.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                id_unidad = Me.TreeViewunidad.SelectedNode.Value
                Result = Refclas.Genera_rotulo_unidad_conservacion_pdf(id_unidad, Session.Item("GA_IDEMPRESA"), 0, ruta_archivo)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
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
            End If
            '----------------------------------------------------------
            'Imprime rotulo expediente  TreeViewunidad Expediente
            '----------------------------------------------------------
            If Me.TreeViewunidad.SelectedNode.ToolTip = "TreeViewunidad" Then
                Dim ref_class_exp As New ClassGaExpediente
                ruta_archivo = ""
                id_expediente = Me.TreeViewunidad.SelectedNode.Value
                '------------------------------------------------------
                'Retorna plantilla impresion usuario gestión
                '------------------------------------------------------
                Dim nombre_plantilla_impresion As String = ""
                Dim id_configuracion_plantilla_rotulo As Integer = 0
                Result = ref_class_exp.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                        id_configuracion_plantilla_rotulo, nombre_plantilla_impresion)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If nombre_plantilla_impresion = "" Then
                    nombre_plantilla_impresion = "DEFAULT"
                End If
                Result = ref_class_exp.Genera_rotulo_Eexpediente_pdf(id_expediente, Session.Item("GA_IDEMPRESA"), nombre_plantilla_impresion, ruta_archivo)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
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
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ButtonImprimirRotulo_Click(sender As Object, e As EventArgs) Handles ButtonImprimirRotulo.Click
        Dim Refclas As New ClassUnidadConservacion
        Dim scripjava As New Classscrripjava
        Try
            Dim ruta_archivo As String = ""
            Dim Result As String = ""
            Dim id_expediente As Integer = 0
            Dim id_unidad As Integer = 0
            If Me.TreeViewunidad.SelectedNode Is Nothing Then Exit Sub
            '----------------------------------------------------------
            'Imprime rotulo unidad contenedora expediente
            '---------------------------------------------------------
            If Me.TreeViewunidad.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                id_unidad = Me.TreeViewunidad.SelectedNode.Value
                Result = Refclas.Genera_rotulo_unidad_conservacion_pdf(id_unidad, Session.Item("GA_IDEMPRESA"), 0, ruta_archivo)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
                    Me.ifimpre_post_.Attributes.Add("src", "../Gestion/WebFormimpresionfile.aspx")
                    UpdatePaneliframe_post.Update()
                    ModalPopupExtenderimpre_post.Show()
                End If
            End If
            '----------------------------------------------------------
            'Imprime rotulo expediente  TreeViewunidad Expediente
            '----------------------------------------------------------
            If Me.TreeViewunidad.SelectedNode.ToolTip = "TreeViewunidad" Then
                Dim ref_class_exp As New ClassGaExpediente
                ruta_archivo = ""
                id_expediente = Me.TreeViewunidad.SelectedNode.Value
                '------------------------------------------------------
                'Retorna plantilla impresion usuario gestión
                '------------------------------------------------------
                Dim nombre_plantilla_impresion As String = ""
                Dim id_configuracion_plantilla_rotulo As Integer = 0
                Result = ref_class_exp.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                        id_configuracion_plantilla_rotulo, nombre_plantilla_impresion)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If nombre_plantilla_impresion = "" Then
                    nombre_plantilla_impresion = "DEFAULT"
                End If
                Result = ref_class_exp.Genera_rotulo_Eexpediente_pdf(id_expediente, Session.Item("GA_IDEMPRESA"), nombre_plantilla_impresion, ruta_archivo)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
                    Me.ifimpre_post_.Attributes.Add("src", "../Gestion/WebFormimpresionfile.aspx")
                    UpdatePaneliframe_post.Update()
                    ModalPopupExtenderimpre_post.Show()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Buttondesarchivar_Click(sender As Object, e As EventArgs) Handles Buttondesarchivar.Click

        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Me.TreeViewunidad.SelectedNode Is Nothing Then Exit Sub
            If Me.TreeViewunidad.SelectedNode.ToolTip = "Expediente" Then
                Dim Refclas As New ClassGaExpediente
                If Session.Item("GA_MANAGER_GESTION") <> 1 Then
                    If Session.Item("GA_ARCHIVA_EXPEDIENTES") = 0 Then
                        scripjava.Showscripman_menu("Usuario sin permisos para desarchivar expediente", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Dim opreserv As Integer = Me.TreeViewunidad.SelectedNode.Value
                    Result = Refclas.Verifica_propiedad_usuario_expediente(opreserv, Session.Item("GA_IDUSUARIOGESTION"))
                    If Result <> "YES" Then
                        scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                   
                End If
                If HiddenField_botones_respuesta.Value = "1" Then
                    Result = Refclas.Des_Archiva_expediente(Me.TreeViewunidad.SelectedNode.Value, Session.Item("GA_IDUSUARIOGESTION"), Session.Item("GA_LOGINUSUARIOGESTION"), Session.Item("ip_host_name"), Me.TreeViewunidad.SelectedNode, Me.TreeViewunidad)
                    If Result <> "YES" Then
                        scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    HiddenField_botones_respuesta.Value = "-1"
                    Me.UpdatePanel_unidad_treview_unidad.Update()
                End If

            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
        
    End Sub
    Protected Sub Button_configura_rotulo_Click(sender As Object, e As EventArgs) Handles Button_configura_rotulo.Click
        Dim Refclas_empresa As New ClassGaExpediente
        Dim scripjava As New Classscrripjava
        Try

            Dim Result As String = ""
            Dim id_configuracion_rotulo As Integer = 0
            Dim nombre_configuracion As String = ""
            Result = Refclas_empresa.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), id_configuracion_rotulo, nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_empresa.Retorna_listado_configuracion_rotulo_expediente(nombre_configuracion, Me.DropDownList_configura_plantilla_rotulo, Me.UpdatePanel_configura_plantilla_rotulo)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_configura_plantilla_rotulo.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_aceptar_configura_plantilla_rotulo_Click(sender As Object, e As EventArgs) Handles Button_aceptar_configura_plantilla_rotulo.Click
        Dim Refclas_empresa As New ClassGaExpediente
        Dim scripjava As New Classscrripjava
        Try
            If Me.DropDownList_configura_plantilla_rotulo.Text = "" Then
                scripjava.Showscripman_menu("Seleccione la plantilla", Me.UpdatePanel_boton_config_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim id_configuracion_rotulo As Integer = 0
            Dim nombre_configuracion As String = ""
            Result = Refclas_empresa.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                      id_configuracion_rotulo, _
                                                                                                      nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_boton_config_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_configuracion_rotulo_expediente As Integer = 0
            Result = Refclas_empresa.Retorna_id_nombre_configuracion_rotulo_expediente(Me.DropDownList_configura_plantilla_rotulo.Text, _
                                                                                       id_configuracion_rotulo_expediente)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_boton_config_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_configuracion_rotulo = 0 Then
                Result = Refclas_empresa.Registra_configuracion_rotulo_expediente_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                  id_configuracion_rotulo_expediente)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_boton_config_rotulo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = Refclas_empresa.Actualiza_configuracion_rotulo_expediente_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                   id_configuracion_rotulo_expediente)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_boton_config_rotulo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Me.ModalPopupExtender_edition_configura_plantilla_rotulo.Hide()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_config_rotulo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub TreeViewArchivo_TreeNodeExpanded(sender As Object, e As TreeNodeEventArgs) Handles TreeViewArchivo.TreeNodeExpanded
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Tagform As Object = e.Node.ValuePath
            Dim Result As String = ""
            Dim Refclas As New ClassGestionArchivo
            'Result = Refclas.Seleccion_Treview(Tagform, Me.TreeViewArchivo.SelectedNode, Me.DropDownListEntidadEmpresa.Text, _
            '                                   Me.TreeViewArchivo.SelectedNode.Value, Me.TreeViewArchivo.SelectedNode.Text, Me.TreeViewunidad)
            Result = Refclas.Recursive_archivo_tre_view_clik(e.Node, Me.DropDownListEntidadEmpresa.Text, 0, Me.TreeViewunidad, "", "")
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanel_unidad_treview_unidad.Update()
                Me.UpdatePanelViewArchivo.Update()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub TreeViewArchivo_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeViewArchivo.SelectedNodeChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Tagform As String = sender.selectedvalue()
            Dim Result As String = ""
            Dim Refclas As New ClassGestionArchivo
            Result = Refclas.Seleccion_Treview(Tagform, _
                                               Me.TreeViewArchivo.SelectedNode, _
                                               Me.DropDownListEntidadEmpresa.Text, _
                                               Me.TreeViewArchivo.SelectedNode.Value, _
                                               Me.TreeViewArchivo.SelectedNode.Text, _
                                               Me.TreeViewunidad)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanel_unidad_treview_unidad.Update()
                Me.UpdatePanelViewArchivo.Update()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub TreeViewArchivo_r_u_e_TreeNodeExpanded(sender As Object, e As TreeNodeEventArgs) Handles TreeViewArchivo_r_u_e.TreeNodeExpanded
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Tagform As Object = e.Node.ValuePath
            Dim Result As String = ""
            Dim Refclas As New ClassGestionArchivo
            'Result = Refclas.Seleccion_Treview(Tagform, Me.TreeViewArchivo.SelectedNode, Me.DropDownListEntidadEmpresa.Text, _
            '                                   Me.TreeViewArchivo.SelectedNode.Value, Me.TreeViewArchivo.SelectedNode.Text, Me.TreeViewunidad)
            Result = Refclas.Recursive_archivo_tre_view_clik(e.Node, _
                                                             Me.DropDownListEntidadEmpresa.Text, _
                                                             1, _
                                                             Me.TreeViewArchivo_r_u_e, _
                                                             e.Node.ValuePath, _
                                                             e.Node.Text)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanel_unidad_treview_unidad.Update()
                Me.UpdatePanelViewArchivo_r_u_e.Update()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    
    Protected Sub Button_me_active_men_dive_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Session.Item("GA_MANAGER_GESTION") = 0 Then
                If Session.Item("GA_ADMINISTRACION_ESTRUCTURA_ARCHIVO") = 0 Then
                    Refclasjava.Showscripman_menu("El usuario no tiene permisos para interactuar con la adimistración de la estructura", Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
          
            Dim Refclas_gestion_archivo As New ClassGestionArchivo
            Dim Result As String = ""
            If Me.Hidden_menu_var_event_dive.Value <> "" Then
                Result = Refclas_gestion_archivo.Seleccion_gestion_archivo_menu(Me.Hidden_menu_var_event_dive.Value, _
                                                                                Me.Hidden_edita_red_event.Value, _
                                                                                Me.Page)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_menu_var_event, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, UpdatePanel_menu_var_event)
        End Try
    End Sub

    Private Sub DropDownList_reg_edit_departamento_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_reg_edit_departamento.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Class_municipio_radicacion As New Class_municipio_radicacion
            If Me.DropDownList_reg_edit_departamento.SelectedItem.Value = 0 Then
                Me.DropDownList_reg_edit_munici_depart.Items.Clear()
                Me.UpdatePanel_reg_edit_edificio_archivo.Update()
                Exit Sub
            Else
                Result = Class_municipio_radicacion.Lista_municipios_departamento(Me.DropDownList_reg_edit_departamento.SelectedItem.Value,
                                                                         0,
                                                                         Me.DropDownList_reg_edit_munici_depart,
                                                                         Me.UpdatePanel_reg_edit_edificio_archivo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_reg_edit_edificio_archivo)
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_reg_edit_edificio_archivo)
        End Try
    End Sub

    Private Sub DropDownList_reg_edit_pais_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_reg_edit_pais.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Class_depart_radicacion As New Class_depart_radicacion
            If Me.DropDownList_reg_edit_pais.Items.Count = 0 Then Exit Sub
            If Me.DropDownList_reg_edit_pais.SelectedItem.Value = 0 Then
                Me.DropDownList_reg_edit_departamento.Items.Clear()
                Me.DropDownList_reg_edit_munici_depart.Items.Clear()
                Me.UpdatePanel_reg_edit_edificio_archivo.Update()
                Exit Sub
            Else
                Result = Class_depart_radicacion.Lista_departamento_Paises(Me.DropDownList_reg_edit_pais.SelectedItem.Value,
                                                                          0,
                                                                          Me.DropDownList_reg_edit_departamento,
                                                                          Me.UpdatePanel_reg_edit_edificio_archivo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_reg_edit_edificio_archivo)
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_reg_edit_edificio_archivo)
        End Try
    End Sub

    Protected Sub Button_reg_edit_aceptar_Click(sender As Object, e As EventArgs) Handles Button_reg_edit_aceptar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_gestion_archivo As New ClassGestionArchivo
            If Me.Hidden_edita_red_event.Value = "ADD" Then
                Result = Refclas_gestion_archivo.Registra_Edificio_Archivo(Me.DropDownListEntidadEmpresa.Text, _
                                                                    Me.DropDownList_reg_edit_pais.SelectedItem.Text, _
                                                                    Me.DropDownList_reg_edit_departamento.SelectedItem.Text, _
                                                                    Me.DropDownList_reg_edit_munici_depart.SelectedItem.Text, _
                                                                    Me.TextBox_reg_edit_telefono.Text, _
                                                                    Me.TextBox_reg_edit_responsable.Text, _
                                                                    Me.TextBox_reg_edit_direcion.Text, _
                                                                    Me.TextBox_reg_edit_edificio_nombre.Text, _
                                                                    Me.TreeViewArchivo, _
                                                                    Me.UpdatePanelViewArchivo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_edit_add)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_reg_edit_edificio_archivo.Hide()
                End If
            End If
            If Me.Hidden_edita_red_event.Value = "EDIT" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_gestion_archivo.Actualiza_Edificio_Archivo(Me.DropDownListEntidadEmpresa.Text, _
                                                                            Val(split(0)), _
                                                                            Me.DropDownList_reg_edit_pais.SelectedItem.Text, _
                                                                            Me.DropDownList_reg_edit_departamento.SelectedItem.Text, _
                                                                            Me.DropDownList_reg_edit_munici_depart.SelectedItem.Text, _
                                                                            Me.TextBox_reg_edit_telefono.Text, _
                                                                            Me.TextBox_reg_edit_responsable.Text, _
                                                                            Me.TextBox_reg_edit_direcion.Text, _
                                                                            Me.TextBox_reg_edit_edificio_nombre.Text, _
                                                                            Me.TreeViewArchivo.SelectedNode, _
                                                                            Me.UpdatePanelViewArchivo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_edit_add)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_reg_edit_edificio_archivo.Hide()
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_edit_add)
        End Try
    End Sub

    Protected Sub Button_reg_edit_piso_aceptar_Click(sender As Object, e As EventArgs) Handles Button_reg_edit_piso_aceptar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_gestion_archivo As New ClassGestionArchivo
            If Me.Hidden_edita_red_event.Value = "ADD" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_gestion_archivo.Registrar_piso_archivo(split(0), _
                                                                        Me.DropDownListEntidadEmpresa.Text, _
                                                                        Me.TextBox_reg_edit_piso_nombre.Text, _
                                                                        Me.TextBox_reg_edit_piso_telefono.Text, _
                                                                        Me.TextBox_reg_edit_piso_responsable.Text, _
                                                                        Me.TreeViewArchivo.SelectedNode, _
                                                                        Me.UpdatePanelViewArchivo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_reg_edit_piso)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_reg_edit_piso_archivo.Hide()
                    Exit Sub
                End If
            End If
            If Me.Hidden_edita_red_event.Value = "EDIT" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_gestion_archivo.Actualiza_piso_archivo(split(0), _
                                                                       Me.DropDownListEntidadEmpresa.Text, _
                                                                       Me.TextBox_reg_edit_piso_nombre.Text, _
                                                                       Me.TextBox_reg_edit_piso_telefono.Text, _
                                                                       Me.TextBox_reg_edit_piso_responsable.Text, _
                                                                       Me.TreeViewArchivo.SelectedNode, _
                                                                       Me.UpdatePanelViewArchivo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_reg_edit_piso)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_reg_edit_piso_archivo.Hide()
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_reg_edit_piso)
        End Try
    End Sub
    Protected Sub Button_registrar_editar_area_piso_Click(sender As Object, e As EventArgs) Handles Button_registrar_editar_area_piso.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_gestion_archivo As New ClassGestionArchivo
            If Me.Hidden_edita_red_event.Value = "ADD" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_gestion_archivo.Registrar_area_piso_archivo(Val(split(0)), _
                                                                              Me.DropDownListEntidadEmpresa.Text, _
                                                                              Me.TextBox_telefono_area_piso.Text, _
                                                                              Me.TextBox_responsable_area_piso.Text, _
                                                                              Me.TextBox_nombre_area_piso.Text, _
                                                                              Me.TreeViewArchivo.SelectedNode, _
                                                                              Me.UpdatePanelViewArchivo, _
                                                                              Me.DropDownList_tipo_archivo_area_piso.Text)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_reg_edit_piso_tool)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_reg_edit_area_piso.Hide()
                    Exit Sub
                End If
            End If
            If Me.Hidden_edita_red_event.Value = "EDIT" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_gestion_archivo.Actualiza_datos_area_archivo(Val(split(0)), _
                                                                              Me.TextBox_telefono_area_piso.Text, _
                                                                              Me.TextBox_responsable_area_piso.Text, _
                                                                              Me.TextBox_nombre_area_piso.Text, _
                                                                              Me.TreeViewArchivo.SelectedNode, _
                                                                              Me.UpdatePanelViewArchivo, _
                                                                              Me.DropDownList_tipo_archivo_area_piso.Text)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_reg_edit_piso_tool)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_reg_edit_area_piso.Hide()
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_reg_edit_piso_tool)
        End Try
    End Sub

    Protected Sub Button_reg_edit_modulo_aceptar_Click(sender As Object, e As EventArgs) Handles Button_reg_edit_modulo_aceptar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_gestion_archivo As New ClassGestionArchivo
            If Me.Hidden_edita_red_event.Value = "ADD" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_gestion_archivo.Registrar_modulo_archivo(Me.TextBox_reg_edit_modulo_area_nombre.Text, _
                                                                          Me.TextBox_reg_edit_modulo_area_descripcion.Text, _
                                                                          Me.TextBox_reg_edit_modulo_area_seccion.Text, _
                                                                          Me.DropDownListEntidadEmpresa.Text, _
                                                                          Val(split(0)), _
                                                                          Me.TreeViewArchivo.SelectedNode, _
                                                                          Me.UpdatePanelViewArchivo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_reg_edit_modulo_area)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_reg_edit_modulo_area.Hide()
                    Exit Sub
                End If
            End If
            If Me.Hidden_edita_red_event.Value = "EDIT" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_gestion_archivo.Actualiza_datos_modulo_archivo(Val(split(0)), _
                                                                                Me.TextBox_reg_edit_modulo_area_nombre.Text, _
                                                                                Me.TextBox_reg_edit_modulo_area_descripcion.Text, _
                                                                                Me.TextBox_reg_edit_modulo_area_seccion.Text, _
                                                                                Me.DropDownListEntidadEmpresa.Text, _
                                                                                Me.TreeViewArchivo.SelectedNode, _
                                                                                Me.UpdatePanelViewArchivo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_reg_edit_modulo_area)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_reg_edit_modulo_area.Hide()
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_reg_edit_modulo_area)
        End Try
    End Sub

    Protected Sub Button_reg_edit_estante_aceptar_Click(sender As Object, e As EventArgs) Handles Button_reg_edit_estante_aceptar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_gestion_archivo As New ClassGestionArchivo
            If Me.Hidden_edita_red_event.Value = "ADD" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Dim val_for As Integer = Val(Me.DropDownList_reg_edit_estante_numero.Text)
                For i As Integer = 0 To val_for - 1
                    Result = Refclas_gestion_archivo.Registra_estante_archivo(Val(split(0)), _
                                                                          Me.DropDownListEntidadEmpresa.Text, _
                                                                          Me.TreeViewArchivo.SelectedNode, _
                                                                          Me.UpdatePanelViewArchivo)
                    If Result <> "YES" Then
                        Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_edit_reg_estante)
                        Exit Sub
                    End If
                Next 
            End If  
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_edit_reg_estante)
        Finally
            Me.ModalPopupExtender_edition_reg_edit_estante.Hide()

        End Try
    End Sub

    Protected Sub Button_reg_edit_entrepano_aceptar_Click(sender As Object, e As EventArgs) Handles Button_reg_edit_entrepano_aceptar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_gestion_archivo As New ClassGestionArchivo
            If Me.Hidden_edita_red_event.Value = "ADD" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Dim val_for As Integer = Val(Me.DropDownList_reg_edit_entrepano_numero.Text)
                For i As Integer = 0 To val_for - 1
                    Result = Refclas_gestion_archivo.Registra_entrepano_archivo(Val(split(0)), _
                                                                                Me.DropDownListEntidadEmpresa.Text, _
                                                                                Val(Me.DropDownList_reg_edit_entrepano_numero_unidades.Text), _
                                                                                Me.TreeViewArchivo.SelectedNode, _
                                                                                Me.UpdatePanelViewArchivo)
                    If Result <> "YES" Then
                        Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_reg_edit_entrepano)
                        Exit Sub
                    End If
                Next
            End If
            If Me.Hidden_edita_red_event.Value = "EDIT" Then
                Dim split() As String = Me.TreeViewArchivo.SelectedNode.Value.ToString.Split("|")
                Result = Refclas_gestion_archivo.Actualiza_numero_unidades_permitidas(Val(split(0)), _
                                                                                      Val(Me.DropDownList_reg_edit_entrepano_numero_unidades.Text))
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_reg_edit_entrepano)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_reg_edit_entrepano)
        Finally
            Me.ModalPopupExtender_edition_reg_edit_entrepano.Hide()
        End Try
    End Sub

End Class