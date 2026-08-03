Public Class FormGaAgregarExpediente
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub FormGaAgregarExpediente_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        
        
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
                Dim refclas_gaexpediente As New ClassGaExpediente
                Dim id_empresa As Integer = 0
                Dim refclas_rad As New ClassRadicador
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxCodigoManual.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "CODIGO_UNICO")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_FINAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "RANGO_EXTREMO_FINAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_INICIAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "RANGO_EXTREMO_INICIAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxTEMA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "TEMA_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxASUNTO_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "ASUNTO_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxOBSERVACION_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "OBSERVACION_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxNOMBRE_PERSONA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "NOMBRE_PERSONA_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "IDENTIFICACION_PERSONA_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "NOMBRE_RESPONSABLE_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "IDENFICACION_RESPONSABLE_EXPEDIENTE")
                Result = refclas_gaexpediente.Activa_registrar_expediente_conservacion(Me.Page)
                If Result <> "YES" Then
                    Labelresultado.Text = Labelresultado.Text & Result
                End If
                
            Else
                Dim refclas_rad As New ClassRadicador
                Dim Result = refclas_rad.agregar_auto_complete(Me.TextBoxCodigoManual.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "CODIGO_UNICO")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_FINAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "RANGO_EXTREMO_FINAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_INICIAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "RANGO_EXTREMO_INICIAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxTEMA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "TEMA_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxASUNTO_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "ASUNTO_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxOBSERVACION_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "OBSERVACION_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxNOMBRE_PERSONA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "NOMBRE_PERSONA_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "IDENTIFICACION_PERSONA_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "NOMBRE_RESPONSABLE_EXPEDIENTE")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "expediente_archivo", "IDENFICACION_RESPONSABLE_EXPEDIENTE")
            End If
        Catch ex As Exception
            Labelresultado.Text = Labelresultado.Text & ex.Message
        End Try
    End Sub

    Private Sub FormGaAgregarExpediente_Unload(sender As Object, e As EventArgs) Handles Me.Unload
       
    End Sub

    Private Sub Button_lista_ayuda_expediente_Click(sender As Object, e As EventArgs) Handles Button_lista_ayuda_expediente.Click
        Dim classcrip As New Classscrripjava
        Dim Result As String = ""
        Dim Resfclas As New Class_ra_tipo_expediente

        Try
            Result = Resfclas.Retorna_ayuda_clase_expediente(Me.DropDownListBoxtipoexpediente.Text, _
                                                             Me.TextBoxayuda.Text)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = True
            Me.TextBoxNUMERO_DIGITALIZADO_CONTENIDO.ReadOnly = True
            Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.ReadOnly = True
            If Me.DropDownListBoxtipoexpediente.Text = "FISICO" Then
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.Text = 0
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.BackColor = Drawing.Color.White
            End If
            'EXPEDIENTE HIBRIDO
            If Me.DropDownListBoxtipoexpediente.Text = "HIBRIDO" Then
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.Text = 0
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.BackColor = Drawing.Color.White
            End If
            'EXPEDIENTE MIXTO
            If Me.DropDownListBoxtipoexpediente.Text = "MIXTO" Then
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.ReadOnly = False
                Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.Text = 0
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.Text = 0
                Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.BackColor = Drawing.Color.White
                Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.BackColor = Drawing.Color.White
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_selecion_organigrama_Click(sender As Object, e As EventArgs) Handles Button_selecion_organigrama.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            Result = Refclas.Seleccion_organigrama(Me.DropDownListorganigrama.Text, _
                                                   Me.Hidden_id_empresa.Value, _
                                                  Me.DropDownListArea, _
                                                  Me.DropDownListSerie, _
                                                  Me.DropDownListSubserie, _
                                                  Me.DropDownList_instrumento, _
                                                  Me.DropDownListNOMBRE_CICLO_ARCHIVO, _
                                                  Me.Labelresultado, _
                                                  Me.update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman(Result, Me.update_panel_controles)
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
            Dim Refclas As New ClassGaExpediente
            Dim id_instrumento As Integer = 0
            If Not Me.DropDownList_instrumento.SelectedItem Is Nothing Then
                id_instrumento = Me.DropDownList_instrumento.SelectedValue
            End If
            Result = Refclas.Seleccion_area_departamento(Me.DropDownListorganigrama.SelectedItem.Text, _
                                                        Me.DropDownListArea.SelectedItem.Text, _
                                                        id_instrumento, _
                                                        Me.DropDownListSerie, _
                                                        Me.DropDownListSubserie, _
                                                        Me.update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman(Result, Me.update_panel_controles)
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
            Dim Refclas As New ClassGaExpediente
            Dim id_istrumento As Integer = 0
            Dim nombre_serie As String = ""
            If Not Me.DropDownList_instrumento.SelectedItem Is Nothing Then
                id_istrumento = Me.DropDownList_instrumento.SelectedValue
            End If
            If Not Me.DropDownListSerie.SelectedItem Is Nothing Then
                nombre_serie = Me.DropDownListSerie.SelectedItem.Text
            End If
            Result = Refclas.Seleccion_serie_documental(Me.DropDownListorganigrama.Text, _
                                                      Me.DropDownListArea.Text, _
                                                      id_istrumento, nombre_serie, _
                                                      Me.DropDownListSubserie, _
                                                      Me.update_panel_controles)
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
            Dim Refclas As New ClassAdmonEmpresa
            Dim Resfclas As New ClassGaTipoDocumental
            Dim id_empresa As Integer = 0
            Dim refe_exp As New ClassGaExpediente
            Result = refe_exp.Limpia_campos_agregar_expediente(Me.table_controles)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = refe_exp.Activa_registrar_expediente_conservacion(Me.Page)
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

    Private Sub ButtonAceptar_Click(sender As Object, e As EventArgs) Handles ButtonAceptar.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            If Session.Item("GA_MANAGER_GESTION") <> 0 Then
            Else
                If Session.Item("GA_REGISTRA_EXPEDIENTES") = 0 Then
                    scripjava.Showscripman_menu("El usuario no tiene permiso para registrar expediente", Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            '----------------------------------------------------------------------------
            'Asigna 0.id entrepaño - 1. id tipo unidad- 2. tipo unidad - 3. descripcion
            '----------------------------------------------------------------------------
            Dim estado_codigo_unico As Integer = 0
            If Me.CheckBoxActivaCodigomanual.Checked = True Then
                estado_codigo_unico = 1
            Else
                estado_codigo_unico = 0
            End If
            If Me.DropDownListBoxtipoexpediente.Text = "" Then
                scripjava.Showscripman_menu("Por favor seleccione el tipo de expediente", Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.Text = "" Then
                scripjava.Showscripman_menu("Por favor digite el número de documentos electronicos", Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.Text = "" Then
                scripjava.Showscripman_menu("Por favor digite el número de folios físicos", Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TextBoxNUMERO_DIGITALIZADO_CONTENIDO.Text = "" Then
                scripjava.Showscripman_menu("Por favor digite el número de documentos digitalizados", Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            
            '------Retorna id tipo expediente (electronico,fisico, hibrido)
            Dim id_tipo_unidad_conservacion As Integer = 0
            Dim id_tipo_expediente As Integer = 0
            Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
            Result = ref_Class_ra_tipo_expediente.Retorna_tipo_id_expediente(id_tipo_expediente, _
                                                                             Me.DropDownListBoxtipoexpediente.Text)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim requiere_unida_conservacion_fisica As Integer = 0
            Result = ref_Class_ra_tipo_expediente.Retorna_tipo_expediente_requiere_unidad_conservacion(id_tipo_expediente, _
                                                                                                       requiere_unida_conservacion_fisica)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
           
            Dim id_expediente As Integer = 0
            Dim option_obliga_archivo_unidad As Integer = 0
            Dim split_unidad As String() = Nothing
            Dim id_unidad_contenedora As Integer = 0
            If Me.TextBox_id_archivo.Text <> "" Then
                split_unidad = Me.TextBox_id_archivo.Text.Split("|")
                id_unidad_contenedora = Val(split_unidad(0))
            End If
            If Me.CheckBox_unidad_contenedora.Checked = True Then
                option_obliga_archivo_unidad = 1
            End If
            Dim aleas_expediente As String = ""
            Dim expediente_relacion As Integer = 0
            Dim id_expediente_registrado As Integer = 0
            Dim serie_documental As String = ""
            Dim sub_serie_documental As String = ""
            Dim id_istrumento As Integer = 0
            Dim ciclo_archivo As String = ""
            If Not Me.DropDownListSerie.SelectedItem Is Nothing Then
                serie_documental = Me.DropDownListSerie.SelectedItem.Text
            End If
            If Not Me.DropDownListSubserie.SelectedItem Is Nothing Then
                sub_serie_documental = Me.DropDownListSubserie.SelectedItem.Text
            End If
            If Not Me.DropDownList_instrumento.SelectedItem Is Nothing Then
                id_istrumento = Me.DropDownList_instrumento.SelectedValue
            End If
            If Not Me.DropDownListNOMBRE_CICLO_ARCHIVO.SelectedItem Is Nothing Then
                ciclo_archivo = Me.DropDownListNOMBRE_CICLO_ARCHIVO.SelectedItem.Text
            End If
            Result = Refclas.Registrar_Expediente_Conservacion(Session.Item("GA_IDUSUARIOGESTION"),
                                                               Me.TextBoxCodigoManual.Text,
             estado_codigo_unico,
             Me.Hidden_id_empresa.Value,
             Me.TextBoxFECHA_EXTREMA_INICIAL.Text,
             Me.TextBoxFECHA_EXTREMA_FINAL.Text,
             Me.TextBoxRANGO_EXTREMO_INICIAL.Text,
             Me.TextBoxRANGO_EXTREMO_FINAL.Text,
             Me.TextBoxTEMA_EXPEDIENTE.Text,
             Me.DropDownListorganigrama.Text,
             Me.DropDownListArea.Text,
             serie_documental,
             sub_serie_documental,
             id_tipo_expediente,
             Me.TextBoxNUMERO_DIGITALIZADO_CONTENIDO.Text.Trim,
             Me.TextBoxNUMERO_FOLIOS_CONTENIDOS.Text.Trim,
             Me.TextBoxNUMERO_ELECTRONICO_CONTENIDO.Text.Trim,
             Me.TextBoxASUNTO_EXPEDIENTE.Text.Trim, 1,
             Me.DropDownList_tipo_unidad_conservacion.Text,
             id_expediente,
             Me.TextBoxOBSERVACION_EXPEDIENTE.Text,
             Me.DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL.Text,
             Me.DropDownListsub_seccion.Text,
             option_obliga_archivo_unidad,
             Hidden_tipo_unidad.Value,
             id_unidad_contenedora,
             requiere_unida_conservacion_fisica,
             ciclo_archivo,
             Me.DropDownListNOMBRE_FONDO.Text,
             Me.TextBoxNOMBRE_PERSONA_EXPEDIENTE.Text,
             Me.TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE.Text,
             TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE.Text,
             Me.TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE.Text,
             aleas_expediente,
             expediente_relacion,
             1, id_istrumento,
             "PRODUCIONDOC",
             0,
             0, 0)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.Updatepanel_botones)
                Exit Sub
            End If
            Result = Refclas.Limpia_campos_agregar_expediente(Me.table_controles, _
                                                              Me.Updatepanel_botones)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.Updatepanel_botones)
                Exit Sub
            End If
            '------------------------------------------------------
            'Retorna plantilla impresion usuario gestión
            '------------------------------------------------------
            Dim Refclasexp As New ClassGaExpediente
            Dim nombre_plantilla_impresion As String = ""
            Dim id_configuracion_plantilla_rotulo As Integer = 0
            Result = Refclasexp.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                 id_configuracion_plantilla_rotulo, _
                                                                                                 nombre_plantilla_impresion)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.Updatepanel_botones)
                Exit Sub
            End If
            If nombre_plantilla_impresion = "" Then
                nombre_plantilla_impresion = "DEFAULT"
            End If
            Dim ruta_archivo As String = ""
            Result = Refclas.Genera_rotulo_Eexpediente_pdf(id_expediente, _
                                                           Session.Item("GA_IDEMPRESA"), _
                                                           nombre_plantilla_impresion, _
                                                           ruta_archivo)
            If Result <> "YES" Then
                scripjava.Showscripman("Se registro el expediente, pero no se generó el rotulo " & Result, Me.Updatepanel_botones)
                Exit Sub
            Else
                Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
                Me.ifimpre_post_.Attributes.Add("src", "../Gestion/WebFormimpresionfile.aspx")
                UpdatePaneliframe_post.Update()
                ModalPopupExtenderimpre_post.Show()
            End If
            Session.Item("SESIONITERCAMBIOEXPEDIENTE") = "AGREGO_EXPDIENTE_VENTANA_WEB"
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.Updatepanel_botones)
        End Try
    End Sub

    Private Sub Button_agrega_unidad_conservacion_interface_Click(sender As Object, e As EventArgs) Handles Button_agrega_unidad_conservacion_interface.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim splinodo() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.Split("|")
            Dim refclasunidad As New ClassUnidadConservacion
            Dim estru_unidad() As unidad_conservacion
            Erase estru_unidad
            Dim id_entrepaño As Integer = splinodo(0)
            Dim Result = refclasunidad.Listar_unidades_conservacion_contenedoras_de_unidades_conservacion(id_entrepaño, 1, estru_unidad)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim no_tag_ref As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value
            Result = refclasunidad.Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion(Me.TreeViewArchivo_r_u_e, estru_unidad, _
            no_tag_ref, Me.TreeViewArchivo_r_u_e.SelectedNode.Text, Me.TreeViewArchivo_r_u_e.SelectedNode)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = ""
            Session.Item("GA_ID_UNIDAD_CONTENEDORA") = 0
            Me.UpdatePanelViewArchivo_r_u_e.Update()
            Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Hide()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_agrega_unidad_contenedora_Click(sender As Object, e As EventArgs) Handles Button_agrega_unidad_contenedora.Click
        Dim scripjava As New Classscrripjava
        Try
            'If Me.DropDownListEntidadEmpresa.Text = "" Then
            '    scripjava.Showscripman_menu("Por favor seleccione la empresa de gestión", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim node_tag As String = ""
            If Me.TreeViewArchivo_r_u_e.Nodes.Count > 0 Then
                If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then
                    Exit Sub
                End If
                node_tag = Me.TreeViewArchivo_r_u_e.SelectedNode.Value
            Else
                Exit Sub
            End If
            If InStr(node_tag, "ENTREPAÑO") <= 0 Then
                scripjava.Showscripman_menu("Sólo en los entrepaños se pueden anidar unidades contenedoras", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then
                scripjava.Showscripman_menu("Por favor seleccione el entrepaño", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub

            End If
            If InStr(Me.TreeViewArchivo_r_u_e.SelectedNode.Value, "ENTREPAÑO") <= 0 Then
                scripjava.Showscripman_menu("Por favor seleccione el entrepaño", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario workflow no tiene asociado un usuario de gestión", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 1 Then
            Else
                If Session.Item("GA_REGISTRA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para agregar unidad de conservación", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Me.Hidden_tipo_unidad_seleccion.Value = node_tag
            Me.Iframe_agregar_unidad_conservacion_popup.Attributes.Add("src", "../Gestion/WebFormGagregarunidadconservacionexpediente.aspx")
            Me.UpdatePanel_agregar_unidad_conservacion_popup.Update()
            Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
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
    Private Sub Button_actualizar_unidad_Click(sender As Object, e As EventArgs) Handles Button_actualizar_unidad.Click
        Dim scripjava As New Classscrripjava
        Try
            Me.TreeViewArchivo_r_u_e.SelectedNode.Text = Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA")
            Me.UpdatePanelViewArchivo_r_u_e.Update()
            Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Hide()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
        
    End Sub
    Private Sub ButtonButtonEditar_Click(sender As Object, e As EventArgs) Handles ButtonButtonEditar.Click
        Dim scripjava As New Classscrripjava
        Try
            If Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                scripjava.Showscripman_menu("El usuario docuarchi.net no tiene asociado un usuario de gestión", Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then Exit Sub
            If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip <> "UNIDAD CONTENEDORA EXPEDIENTE" Then
                scripjava.Showscripman_menu("Sólo se pueden editar unidades contenedoras (cajas de carpetas, cajas de tomos)", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("GA_MANAGER_GESTION") = 1 Then
            Else
                If Session.Item("GA_EDITA_UNIDAD_CONSERVACION") = 0 Then
                    scripjava.Showscripman_menu("El usuario " & Session.Item("GA_LOGINUSUARIOGESTION") & " no tiene permiso para editar unidad de conservación", Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Verifica_propiedad_usuario_unidad_conservacion(Me.TreeViewArchivo_r_u_e.SelectedNode.Value, _
                 Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.Split("|")
            Session.Item("GA_ID_UNIDAD_CONTENEDORA") = split(0)
            Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = Session.Item("EMPRESA_GESTION")
            Me.Iframe_agregar_unidad_conservacion_popup.Attributes.Add("src", "../Gestion/WebFormGaEditarunidadconservaexpe.aspx")
            Me.UpdatePanel_agregar_unidad_conservacion_popup.Update()
            Me.ModalPopupExtende_agregar_unidad_conservacion_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo_r_u_e, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_archivar_Click(sender As Object, e As EventArgs) Handles Button_archivar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            Dim refclas_unidad_conservacion As New ClassUnidadConservacion
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then Exit Sub
            If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.ToString.Split("|")
                Dim codigo_unidad_contenedora As String = ""
                Result = refclas_unidad_conservacion.Retorna_codigo_unidad_conservacion_por_id_unidad(Val(split(0)), codigo_unidad_contenedora)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Hidden_tipo_unidad.Value = "UNIDAD CONTENEDORA EXPEDIENTE"
                TextBox_id_archivo.Text = split(0) & "|" & split(1) & "|" & codigo_unidad_contenedora
                update_panel_controles.Update()
                ModalPopupExtende_reubicar_unidad_expediente_popup.Hide()
                
            End If
            If InStr(Me.TreeViewArchivo_r_u_e.SelectedNode.Text, "Entrepaño") > 0 Then
                Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.ToString.Split("|")
                Hidden_tipo_unidad.Value = "Entrepaño"
                TextBox_id_archivo.Text = Me.TreeViewArchivo_r_u_e.SelectedNode.Value
                update_panel_controles.Update()
                ModalPopupExtende_reubicar_unidad_expediente_popup.Hide()
               
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
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
                
            End If
            
            Result = Refclas_empresa.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa_r_u_e, Me.UpdatePanelEntidad_r_u_e)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'Me.DropDownListEntidadEmpresa_r_u_e.Text = DropDownListEntidadEmpresa_r_u_e.Text
            Me.UpdatePanelEntidad_r_u_e.Update()
            Dim refclas_unidad As New ClassGestionArchivo
            If Me.HiddenField_estado_ubicacion.Value <> "YES" Then
                Result = refclas_unidad.Listar_Entidad_Archivo_Edificio(Me.TreeViewArchivo_r_u_e, Me.DropDownListEntidadEmpresa_r_u_e.Text)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.HiddenField_estado_ubicacion.Value = "YES"
                    UpdatePanelViewArchivo_r_u_e.Update()
                End If
            End If
            ModalPopupExtende_reubicar_unidad_expediente_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_des_archivar_Click(sender As Object, e As EventArgs) Handles Button_des_archivar.Click
        Hidden_tipo_unidad.Value = ""
        TextBox_id_archivo.Text = ""
        update_panel_controles.Update()
    End Sub

    Protected Sub Button_configurar_rotulo_Click(sender As Object, e As EventArgs) Handles Button_configurar_rotulo.Click
        Dim Refclas_empresa As New ClassGaExpediente
        Dim scripjava As New Classscrripjava
        Try
           
            Dim Result As String = ""
            Dim id_configuracion_rotulo As Integer = 0
            Dim nombre_configuracion As String = ""
            Result = Refclas_empresa.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), id_configuracion_rotulo, nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_empresa.Retorna_listado_configuracion_rotulo_expediente(nombre_configuracion, Me.DropDownList_configura_plantilla_rotulo, Me.UpdatePanel_configura_plantilla_rotulo)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If       
            Me.ModalPopupExtender_edition_configura_plantilla_rotulo.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_aceptar_configura_plantilla_rotulo_Click(sender As Object, e As EventArgs) Handles Button_aceptar_configura_plantilla_rotulo.Click
        Dim Refclas_empresa As New ClassGaExpediente
        Dim scripjava As New Classscrripjava
        Try
            If Me.DropDownList_configura_plantilla_rotulo.Text = "" Then
                scripjava.Showscripman_menu("Seleccione la plantilla", Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim id_configuracion_rotulo As Integer = 0
            Dim nombre_configuracion As String = ""
            Result = Refclas_empresa.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), id_configuracion_rotulo, nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_configuracion_rotulo_expediente As Integer = 0
            Result = Refclas_empresa.Retorna_id_nombre_configuracion_rotulo_expediente(Me.DropDownList_configura_plantilla_rotulo.Text, id_configuracion_rotulo_expediente)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_configuracion_rotulo = 0 Then
                Result = Refclas_empresa.Registra_configuracion_rotulo_expediente_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), id_configuracion_rotulo_expediente)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = Refclas_empresa.Actualiza_configuracion_rotulo_expediente_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), id_configuracion_rotulo_expediente)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Me.ModalPopupExtender_edition_configura_plantilla_rotulo.Hide()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub DropDownList_instrumento_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_instrumento.SelectedIndexChanged
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            Result = Refclas.Seleccion_instrumento(Me.DropDownListorganigrama.SelectedItem.Text, _
                                                   Me.DropDownListArea.SelectedItem.Text, _
                                                   Me.DropDownList_instrumento.SelectedValue, _
                                                   Me.DropDownListSerie, _
                                                   Me.DropDownListSubserie, _
                                                   Me.DropDownListNOMBRE_CICLO_ARCHIVO, _
                                                   Me.update_panel_controles)
            If Result <> "YES" Then
                classcrip.Showscripman(Result, Me.update_panel_controles)
                Exit Sub
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class