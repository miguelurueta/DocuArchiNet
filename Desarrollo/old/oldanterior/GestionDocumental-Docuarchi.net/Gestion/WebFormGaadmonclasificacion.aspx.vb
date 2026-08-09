Public Class WebFormGaadmonclasificacion
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim script As [String] = "$(document).ready(function(){$('#" + TextBoxFECHA_EXTREMA_FINAL_CUADRO.ClientID & "').format_date();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_FINAL_CUADRO.ClientID))) Then
            ScriptManager.RegisterClientScriptBlock(Me.TextBoxFECHA_EXTREMA_FINAL_CUADRO, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_FINAL_CUADRO.ClientID), script, True)
        End If
        script = "$(document).ready(function(){$('#" + TextBoxFECHA_EXTREMA_INICIAL_CUADRO.ClientID & "').format_date();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_INICIAL_CUADRO.ClientID))) Then
            ScriptManager.RegisterClientScriptBlock(Me.TextBoxFECHA_EXTREMA_INICIAL_CUADRO, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", TextBoxFECHA_EXTREMA_INICIAL_CUADRO.ClientID), script, True)
        End If
        If Me.Page.IsPostBack = False Then
            Dim Result As String = ""
            Dim Refclas As New Class_empresa_gestion_documental
            Dim Refclass_clasificacion As New ClassGaClasificacionDocumental
            Result = Refclas.Solicita_listado_empresa(0, _
                                                      Me.DropDownList_nivel_clasficacion, _
                                                      UpdatePanel_estructura_clasificacion)
            If Result <> "YES" Then
                Me.Label_estado.Text = Me.Label_estado.Text & "|" & Result
            End If
            If Not Me.DropDownList_nivel_clasficacion.SelectedItem Is Nothing Then
                Result = Refclass_clasificacion.Listar_cuadro_clasificacion_documental(Me.DropDownList_nivel_clasficacion.SelectedValue, _
                                                                                       TreeViewEstructura, _
                                                                                       1)
                If Result <> "YES" Then
                    Me.Label_estado.Text = Me.Label_estado.Text & "|" & Result
                End If
            End If
        End If
    End Sub

    Protected Sub Button_activa_agregar_cuadro_clasificacion_Click(sender As Object, e As EventArgs) Handles Button_activa_agregar_cuadro_clasificacion.Click
        Dim Result As String = ""
        Dim Refclas As New ClassAdmonEmpresa
        Dim classcrip As New Classscrripjava
        Dim Class_registro_organigrama As New Class_registro_organigrama
        Try
            Dim Class_empresa_gestion_documental As New Class_empresa_gestion_documental
            Result = Class_empresa_gestion_documental.Solicita_listado_empresa(0, _
                                                                               Me.DropDownList_entidad_empresa_clasificacion, _
                                                                               Me.UpdatePanel_estructura_clasificacion)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Not Me.DropDownList_entidad_empresa_clasificacion.SelectedValue Is Nothing Then
                Result = Class_registro_organigrama.Listar_Organigramas_Empresa_Combo_Default_Items(Me.DropDownList_entidad_empresa_clasificacion.SelectedValue, _
                                                                                                    0, _
                                                                                                    Me.DropDownList_organigrama, _
                                                                                                    Me.UpdatePanel_estructura_clasificacion)
                If Result <> "YES" Then
                    classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim refclas_clasificacion As New Class_Listar_Codigo_pais_3166
            Result = refclas_clasificacion.Listar_Codigo_pais_3166(Me.DropDownList_codigo_estructura)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Hidden_tipo_trasac.Value = "AGREGAR"
            UpdatePanel_contenido_estructura_cuadro_editar_crear.Update()
            ModalPopupExtender_crear_cuadro_clasificacion.Show()
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_lista_cuadro_clasficacion_treview_Click(sender As Object, e As EventArgs) Handles Button_lista_cuadro_clasficacion_treview.Click
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try          
            If Me.DropDownList_nivel_clasficacion.SelectedItem.Text = "" Then
                Me.TreeViewEstructura.Nodes.Clear()
                UpdatePanelViewArchivo.Update()
                Me.Hidden_id_cuadro.Value = 0
                Exit Sub
            End If
            Result = Refclas.Listar_cuadro_clasificacion_documental(Me.DropDownList_nivel_clasficacion.SelectedValue, _
                                                                    Me.TreeViewEstructura, _
                                                                    1)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Result = Refclas.Retorna_id_cuadro_clasificacion_documental(Me.DropDownList_nivel_clasficacion.Text, Me.Hidden_id_cuadro.Value)
                If Result <> "YES" Then
                    classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                UpdatePanelViewArchivo.Update()
                End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_editar_cuadro_clasificacion_Click(sender As Object, e As EventArgs) Handles Button_activa_editar_cuadro_clasificacion.Click

        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try
            If Me.TreeViewEstructura.SelectedNode Is Nothing Then
                classcrip.Showscripman_menu("Seleccione el item a editar", Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim spli() As String = Me.TreeViewEstructura.SelectedNode.Value.Split("|")
            If spli(1) = "CUADRO CLASIFICACION" Then
                Result = Refclas.Asigna_datos_interface_edicion_cuadro_clasificacion(spli(0), _
                                                                                 Me.TextBoxFECHA_EXTREMA_INICIAL_CUADRO, _
                                                                                 Me.TextBoxFECHA_EXTREMA_FINAL_CUADRO, _
                                                                                 Me.DropDownList_codigo_estructura, _
                                                                                 Me.DropDownList_entidad_empresa_clasificacion, _
                                                                                 Me.DropDownList_entidad_empresa_clasificacion, _
                                                                                 Me.DropDownList_organigrama, _
                                                                                 Me.UpdatePanel_contenido_estructura_cuadro_editar_crear)
                If Result <> "YES" Then
                    classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Hidden_tipo_trasac.Value = "EDITAR"
                    Me.Label_title_estructura.Text = "Editar estructura de clasificación documental"
                    UpdatePanel_contenido_estructura_cuadro_editar_crear.Update()
                    ModalPopupExtender_crear_cuadro_clasificacion.Show()
                End If
            Else
                Result = Refclas.Asgina_datos_interface_edita_nivel(spli(0), _
                                                           Me.TextBox_titulo_nivel_clasificacion_editar.Text, _
                                                           Me.TextBox_signatura_nivel_clasificacion_editar.Text)
                If Result <> "YES" Then
                    classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                UpdatePanel_editar_nivel_clasficacion.Update()
                Me.ModalPopupExtender_editar_nivel_clasificacion.Show()
            End If

        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_editar_agregar_cuadro_clasficacion_Click(sender As Object, e As EventArgs) Handles Button_editar_agregar_cuadro_clasficacion.Click
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try
            '----------------------------------------
            'Verificar permisos para crear el cuadro
            '----------------------------------------
            Dim id_empresa As Integer = 0
            Dim id_organigrama As Integer = 0
            Dim nombre_empresa As String = ""
            If Not DropDownList_entidad_empresa_clasificacion.SelectedItem Is Nothing Then
                id_empresa = DropDownList_entidad_empresa_clasificacion.SelectedValue
                nombre_empresa = Me.DropDownList_entidad_empresa_clasificacion.SelectedItem.Text
            Else
                id_empresa = 0
                nombre_empresa = ""
            End If
            If Not DropDownList_organigrama.SelectedItem Is Nothing Then
                id_organigrama = DropDownList_organigrama.SelectedValue
            End If
            If Me.Hidden_tipo_trasac.Value = "EDITAR" Then
                Dim spli() As String = Me.TreeViewEstructura.SelectedNode.Value.Split("|")
                Result = Refclas.Actualiza_cuadro_clasficacion_documental(nombre_empresa, _
                                                                          Me.DropDownList_codigo_estructura.Text, _
                                                                          Me.TextBoxFECHA_EXTREMA_INICIAL_CUADRO.Text, _
                                                                          Me.TextBoxFECHA_EXTREMA_FINAL_CUADRO.Text, _
                                                                          Me.DropDownList_nivel_clasficacion, _
                                                                          Me.TreeViewEstructura, _
                                                                          spli(0), _
                                                                          id_empresa, _
                                                                          id_organigrama)
                If Result <> "YES" Then
                    classcrip.Showscripman_menu(Result, Me.UpdatePanel_boton_editar_agregar_cuadro_clasficacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    UpdatePanel_estructura_clasificacion.Update()
                    UpdatePanelViewArchivo.Update()
                    ModalPopupExtender_crear_cuadro_clasificacion.Hide()
                End If
            End If

            If Me.Hidden_tipo_trasac.Value = "AGREGAR" Then
                Result = Refclas.Crear_Cuadro_de_clasficacion_documental(nombre_empresa, _
                                                                         Me.DropDownList_codigo_estructura.Text, _
                                                                         Me.TextBoxFECHA_EXTREMA_INICIAL_CUADRO.Text, _
                                                                         Me.TextBoxFECHA_EXTREMA_FINAL_CUADRO.Text, _
                                                                         id_empresa, _
                                                                         id_organigrama, _
                                                                         Me.TreeViewEstructura)
                If Result <> "YES" Then
                    classcrip.Showscripman(Result, Me.UpdatePanel_boton_editar_agregar_cuadro_clasficacion)
                    Exit Sub
                Else
                    UpdatePanel_estructura_clasificacion.Update()
                    UpdatePanelViewArchivo.Update()
                    ModalPopupExtender_crear_cuadro_clasificacion.Hide()
                End If
            End If
           
        Catch ex As Exception
            classcrip.Showscripman(ex.Message, Me.UpdatePanel_boton_editar_agregar_cuadro_clasficacion)
        End Try
    End Sub

    Private Sub Button_eliminar_cuadro_clasificacion_Click(sender As Object, e As EventArgs) Handles Button_eliminar_cuadro_clasificacion.Click
        'Hidden_result
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try
            If Hidden_result.Value = "0" Then Exit Sub
            If Me.TreeViewEstructura.SelectedNode Is Nothing Then
                classcrip.Showscripman_menu("Seleccione el item a eliminar", Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim spli() As String = Me.TreeViewEstructura.SelectedNode.Value.Split("|")
            If spli(1) = "CUADRO CLASIFICACION" Then
                Result = Refclas.Eliminar_cuadro_clasificacion_documental(spli(0), _
                                                                     Me.TreeViewEstructura)
                If Result <> "YES" Then
                    classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    UpdatePanelViewArchivo.Update()
                    UpdatePanel_estructura_clasificacion.Update()
                End If
            Else
                Result = Refclas.Elimina_nivel_cuadro_clasificacion_documental(Val(spli(0)), _
                                                                               Me.TreeViewEstructura, _
                                                                               Me.TreeViewEstructura.SelectedNode)
                If Result <> "YES" Then
                    classcrip.Showscripman_menu(Result, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.UpdatePanelViewArchivo.Update()
                End If
            End If
          
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.UpdatePanel_estructura_clasificacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
       
    End Sub

    Protected Sub Button_activa_agregar_nivel_Click(sender As Object, e As EventArgs) Handles Button_activa_agregar_nivel.Click
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try
            '----------------------------------------
            'Verificar permisos para crear el cuadro
            '----------------------------------------
            Dim node_tag As String = ""
            If Me.TreeViewEstructura.Nodes.Count > 0 Then
                If Me.TreeViewEstructura.SelectedNode Is Nothing Then
                    classcrip.Showscripman_menu("Debe seleccionar el item para agregar el nivel", Me.UpdatePanel_opciones_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                node_tag = Me.TreeViewEstructura.SelectedNode.Value
            Else
                classcrip.Showscripman_menu("Debe seleccionar el item para agregar el nivel", Me.UpdatePanel_opciones_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Lista_niveles_de_clasificacion_documental_drowlist(Me.DropDownList_nivel_clasificacion)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.UpdatePanel_opciones_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.TextBox_ayuda_nivel.Text = ""
                UpdatePanel_contenido_estructura_nivel_clasificacion_crear.Update()
                ModalPopupExtender_agregar_nivel_clasificacion.Show()
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.UpdatePanel_opciones_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub DropDownList_nivel_clasificacion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_nivel_clasificacion.SelectedIndexChanged
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try            
            Result = Refclas.Retorna_ayuda_restricciones_niveles_clasificacion(Me.DropDownList_nivel_clasificacion.Text, _
                                                                               Me.TextBox_ayuda_nivel.Text)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.UpdatePanel_contenido_estructura_nivel_clasificacion_crear, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                UpdatePanel_contenido_estructura_nivel_clasificacion_crear.Update()
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.UpdatePanel_contenido_estructura_nivel_clasificacion_crear, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_agregar_nivel_clasificacion_Click(sender As Object, e As EventArgs) Handles Button_agregar_nivel_clasificacion.Click
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try
            If Me.DropDownList_nivel_clasificacion.Text = "" Then
                classcrip.Showscripman_menu("Debe seleccionar el nivel de clasificación ", Me.UpdatePanel_boton_agregar_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim node_tag As String = ""
            If Me.TreeViewEstructura.Nodes.Count > 0 Then
                If Me.TreeViewEstructura.SelectedNode Is Nothing Then
                    classcrip.Showscripman_menu("Debe seleccionar un elemento de la estructura ", Me.UpdatePanel_boton_agregar_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                node_tag = Me.TreeViewEstructura.SelectedNode.Value
            Else
                Exit Sub
            End If
            Dim split() As String = node_tag.Split("|")
            Result = Refclas.Agregar_nivel_cuadro_clasficion_documental(Me.DropDownList_nivel_clasificacion.Text, _
                                                                        Val(split(0)), _
                                                                        Me.TextBox_titulo_nivel_clasificacion.Text, _
                                                                        Me.TextBox_signatura_nivel_clasificacion.Text, _
                                                                        Me.TreeViewEstructura.SelectedNode, _
                                                                        Me.TreeViewEstructura)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.UpdatePanel_boton_agregar_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                UpdatePanelViewArchivo.Update()
                UpdatePanel_contenido_estructura_nivel_clasificacion_crear.Update()
                ModalPopupExtender_agregar_nivel_clasificacion.Hide()
            End If

        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_agregar_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub TreeViewEstructura_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeViewEstructura.SelectedNodeChanged
        Dim Refclasjava As New Classscrripjava
        Try
            'Dim Tagform As String = sender.selectedvalue()
            'Dim Result As String = ""
            'Dim Refclas As New ClassGaClasificacionDocumental
            'Dim spli() As String = sender.selectedvalue().ToString.Split("|")
            'Result = Refclas.Listar_niveles_cuadro_clasficacion_documental_treview(spli(0), _
            '                                                                       Me.TreeViewEstructura.SelectedNode)
            'If Result <> "YES" Then
            '    Refclasjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'Else
            '    Me.TreeViewEstructura.SelectedNode.ExpandAll()
            '    Me.UpdatePanelViewArchivo.Update()
            'End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_elimina_nivel_Click(sender As Object, e As EventArgs) Handles Button_elimina_nivel.Click
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try
            '----------------------------------------
            'Verificar permisos para crear el cuadro
            '----------------------------------------
            If Me.Hidden_result_eliminar.Value = 0 Then Exit Sub
            Dim node_tag As String = ""
            If Me.TreeViewEstructura.Nodes.Count > 0 Then
                If Me.TreeViewEstructura.SelectedNode Is Nothing Then
                    Exit Sub
                End If
                node_tag = Me.TreeViewEstructura.SelectedNode.Value
            Else
                classcrip.Showscripman_menu("Debe seleccionar el nivel de clasificación a eliminar", Me.UpdatePanel_opciones_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim spli() As String = node_tag.Split("|")
            Result = Refclas.Elimina_nivel_cuadro_clasificacion_documental(Val(spli(0)), _
                                                                           Me.TreeViewEstructura, _
                                                                           Me.TreeViewEstructura.SelectedNode)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.UpdatePanel_opciones_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanelViewArchivo.Update()
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.UpdatePanel_opciones_nivel_clasificacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_editar_nivel_clasificacion_Click(sender As Object, e As EventArgs) Handles Button_editar_nivel_clasificacion.Click
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try
            Dim node_tag As String = ""
            If Me.TreeViewEstructura.Nodes.Count > 0 Then
                If Me.TreeViewEstructura.SelectedNode Is Nothing Then
                    classcrip.Showscripman("Debe seleccionar el nivel de clasificación a editar", Me.UpdatePanel_editar_nivel_clasficacion)
                    Exit Sub
                End If
                node_tag = Me.TreeViewEstructura.SelectedNode.Value
            Else
                classcrip.Showscripman("Debe seleccionar el nivel de clasificación a editar", Me.UpdatePanel_editar_nivel_clasficacion)
                Exit Sub
            End If
            Dim spli() As String = node_tag.Split("|")
            Result = Refclas.Actualiza_nivel_cuadro_clasificacion_documental(spli(0), _
                                                                           Me.TextBox_titulo_nivel_clasificacion_editar.Text, _
                                                                           Me.TextBox_signatura_nivel_clasificacion_editar.Text, _
                                                                           Me.TreeViewEstructura.SelectedNode)
            If Result <> "YES" Then
                classcrip.Showscripman(Result, Me.UpdatePanel_editar_nivel_clasficacion)
                Exit Sub
            Else
                Me.UpdatePanelViewArchivo.Update()
                Me.ModalPopupExtender_editar_nivel_clasificacion.Hide()
            End If
        Catch ex As Exception
            classcrip.Showscripman(ex.Message, Me.UpdatePanel_editar_nivel_clasficacion)
        End Try
    End Sub

    Protected Sub Button_activa_edita_descripcion_Click(sender As Object, e As EventArgs) Handles Button_activa_edita_descripcion.Click
        Dim Result As String = ""
        Dim Refclas As New ClassGaClasificacionDocumental
        Dim classcrip As New Classscrripjava
        Try
            Dim node_tag As String = ""
            If Me.TreeViewEstructura.Nodes.Count > 0 Then
                If Me.TreeViewEstructura.SelectedNode Is Nothing Then
                    classcrip.Showscripman("Debe seleccionar el nivel de clasificación a editar", Me.UpdatePanel_opciones_nivel_clasificacion)
                    Exit Sub
                End If
                node_tag = Me.TreeViewEstructura.SelectedNode.Value
            Else
                classcrip.Showscripman("Debe seleccionar el nivel de clasificación a editar", Me.UpdatePanel_opciones_nivel_clasificacion)
                Exit Sub
            End If
            Dim spli() As String = node_tag.Split("|")
            Result = Refclas.Asgina_datos_interface_edita_nivel(spli(0), _
                                                                Me.TextBox_titulo_nivel_clasificacion_editar.Text, _
                                                                Me.TextBox_signatura_nivel_clasificacion_editar.Text)
            If Result <> "YES" Then
                classcrip.Showscripman(Result, Me.UpdatePanel_opciones_nivel_clasificacion)
                Exit Sub
            Else
                Me.UpdatePanel_editar_nivel_clasficacion.Update()
                Me.ModalPopupExtender_editar_nivel_clasificacion.Show()
            End If
        Catch ex As Exception
            classcrip.Showscripman(ex.Message, Me.UpdatePanel_opciones_nivel_clasificacion)
        End Try
    End Sub
End Class