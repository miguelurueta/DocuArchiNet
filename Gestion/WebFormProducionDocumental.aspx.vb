Imports System.IO

Public Class WebFormProducionDocumental
    Inherits System.Web.UI.Page
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If

        If Me.Page.IsPostBack = False Then
            Session.Item("DG_TRAMITE_DIGITAIZACION") = -1
            Session.Item("GA_ESTADO_ARCHIVA_DOCUMENTO") = 0
            Me.TreeViewArchivo.ExpandDepth = 0
            Dim Result As String = ""
            Dim Reflcas As New ClassGaProducionDocumental
            Dim class_ra_pro_consecutivo As New Class_ra_pro_consecutivo_documento_produccion
            'Session.Item("GA_TIPO_CONSECUTVO_DOC_PRODUCCION")
            Dim Ref_class_nivel As New Class_niveles_organizacion
            Dim Ref_class_pro_nivel As New Class_ra_pro_niveles
            Session.Item("DG_ESTADO_VENTA") = 0
            'Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = ""
            Hidden0003.Value = ""
            Hidden0007.Value = ""
            If Not HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA") Is Nothing Then
                If HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA").Length > 0 Then
                    Hidden0005.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Hidden0007.Value = HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA")
                    Hidden0006.Value = 1
                    For i As Integer = 0 To HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA").Length - 1
                        If i = 0 Then
                            Hidden0003.Value = HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i)
                        Else
                            Hidden0003.Value = Hidden0003.Value & "-" & HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i)
                        End If

                    Next
                End If
            Else
                Hidden0003.Value = ""
            End If
            HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA") = Nothing
            Result = class_ra_pro_consecutivo.Solicita_id_consectivo_usuario_producion(Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       Session.Item("GA_CONSECUTVO_DOC_PRODUCCION"))
            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & " " & Result
            Else
                If Session.Item("GA_CONSECUTVO_DOC_PRODUCCION") = 0 Then
                    Result = class_ra_pro_consecutivo.Registra_conecutivo_producio_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                   Session.Item("GA_CONSECUTVO_DOC_PRODUCCION"))
                    If Result <> "YES" Then
                        Label_estado.Text = Label_estado.Text & " " & Result
                    End If
                End If
            End If
            Dim existencia As String = ""
            Result = Ref_class_pro_nivel.Solicita_existencia_nodo_principal(Session.Item("GA_IDUSUARIOGESTION"),
                                                                            existencia)

            If Result <> "YES" Then
                Label_estado.Text = Label_estado.Text & " " & Result
            Else
                If existencia = "NO" Then
                    Dim tre_node As New TreeNode
                    Dim id_nivel As Integer = 0
                    Result = Ref_class_nivel.Agregar_nivel_clasificacion(0,
                                                                        "Expedientes",
                                                                        Session.Item("GA_IDUSUARIOGESTION"),
                                                                        Me.TreeViewArchivo,
                                                                        tre_node,
                                                                        Me.UpdatePanelViewArchivo,
                                                                        id_nivel)
                    If Result <> "YES" Then
                        Label_estado.Text = Label_estado.Text & " " & Result
                    Else
                        Result = Ref_class_nivel.Registra_expedientes_version_old(id_nivel,
                                                                                  Session.Item("GA_IDUSUARIOGESTION"),
                                                                                  Me.TreeViewArchivo,
                                                                                  tre_node,
                                                                                  Me.UpdatePanelViewArchivo)
                        If Result <> "YES" Then
                            Label_estado.Text = Label_estado.Text & " " & Result
                        End If

                    End If
                Else
                    Dim stru_nivel() As stru_niveles = Nothing
                    Dim Refclas As New Class_ra_pro_niveles
                    Result = Refclas.Solicita_niveles_organizacion(Session.Item("GA_IDUSUARIOGESTION"),
                                                                   stru_nivel)
                    If Result <> "YES" Then
                        Label_estado.Text = Label_estado.Text & " " & Result
                        Exit Sub
                    End If
                    If Not stru_nivel Is Nothing Then
                        Result = Ref_class_nivel.Lista_niveles_de_organizacion(Session.Item("GA_IDUSUARIOGESTION"),
                                                                               Me.TreeViewArchivo,
                                                                               Me.UpdatePanelViewArchivo,
                                                                               stru_nivel,
                                                                               0)
                        If Result <> "YES" Then
                            Label_estado.Text = Label_estado.Text & " " & Result
                        End If

                    End If
                    stru_nivel = Nothing
                    Result = Refclas.Solicita_niveles_organizacion_compartidos(Session.Item("GA_IDUSUARIOGESTION"),
                                                                               stru_nivel)
                    If Result <> "YES" Then
                        Label_estado.Text = Label_estado.Text & " " & Result
                        'Exit Sub
                    End If
                    If Not stru_nivel Is Nothing Then
                        Result = Ref_class_nivel.Lista_niveles_de_organizacion(Session.Item("GA_IDUSUARIOGESTION"),
                                                                               Me.TreeViewArchivo,
                                                                               Me.UpdatePanelViewArchivo,
                                                                               stru_nivel,
                                                                               1)
                        If Result <> "YES" Then
                            Label_estado.Text = Label_estado.Text & " " & Result
                        End If
                    End If
                End If
            End If

        End If
    End Sub

    Protected Sub Button_nueva_carpeta_Click(sender As Object, e As EventArgs) Handles Button_nueva_carpeta.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            Dim ref_Class_ra_pro_niveles_has_expediente_archivo As New Class_ra_pro_niveles_has_expediente_archivo

            Dim id_nivel As Integer = 0
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                clasjava.Showscripman_menu("Por favor seleccione el nodo donde desea agregar la carpeta o expediente ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") > 0 Then
                Dim split_sel() As String = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
                Result = ref_Class_ra_pro_niveles_has_expediente_archivo.Solicita_id_nivel_expediente(Val(split_sel(2)), _
                                                                                                      id_nivel)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.Hidden_parent_node_id.Value = "YES"
            Else
                Me.Hidden_parent_node_id.Value = ""
                id_nivel = Val(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"))
            End If

            Dim index_nodo As Integer = 1
            Dim trednode As TreeNode = Nothing
            Result = Refclas.Activa_agregar_expediente_producion(id_nivel, _
                                                                 Session.Item("GA_IDUSUARIOGESTION"), _
                                                                 Me.DropDownList_serie_documental, _
                                                                 Me.DropDownList_sub_serie_asunto, _
                                                                 Me.DropDownList_fondo, _
                                                                 Me.DropDownList_gabinete_producion, _
                                                                 Me.DropDownList_instrumento, _
                                                                 Me.UpdatePanel_agregar_expediente_carpeta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_agregar_expediente_carpeta.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub DropDownList_serie_documental_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_serie_documental.SelectedIndexChanged
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            If Me.DropDownList_serie_documental.SelectedItem Is Nothing Then Exit Sub
            Result = Refclas.Solicita_sub_series_documentales_relacionadas_a_la_serie_documental_item(Me.DropDownList_serie_documental.SelectedItem.Value, _
                                                                                                      Me.DropDownList_sub_serie_asunto, _
                                                                                                      Me.UpdatePanel_agregar_expediente_carpeta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_agregar_expediente_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_agregar_expediente_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_agregar_expediente_Click(sender As Object, e As EventArgs) Handles Button_agregar_expediente.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim ref_Class_ra_pro_niveles_has_expediente_archivo As New Class_ra_pro_niveles_has_expediente_archivo
            Me.Hidden_rest_agrre_exp_0011.Value = ""
            Dim id_nivel As Integer = 0
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                clasjava.Showscripman_menu("Por favor seleccione el nodo donde desea agregar la carpeta o expediente ", Me.UpdatePanel_agregar_expediente_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_expediente_relacion As Integer = 0

            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") > 0 Then
                Dim split_sel() As String = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
                Result = ref_Class_ra_pro_niveles_has_expediente_archivo.Solicita_id_nivel_expediente(Val(split_sel(2)), _
                                                                                                      id_nivel)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_agregar_expediente_carpeta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                id_nivel = Val(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"))
            End If
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim id_instrumento_archivistico As Integer = 0
            If Not Me.DropDownList_serie_documental.SelectedItem Is Nothing Then
                nombre_serie = Me.DropDownList_serie_documental.SelectedItem.Text
                id_serie = Me.DropDownList_serie_documental.SelectedItem.Value
            End If
            If Not Me.DropDownList_sub_serie_asunto.SelectedItem Is Nothing Then
                nombre_sub_serie = Me.DropDownList_sub_serie_asunto.SelectedItem.Text
                id_sub_serie = Me.DropDownList_sub_serie_asunto.SelectedItem.Value
            End If
            If Not Me.DropDownList_instrumento.SelectedItem Is Nothing Then
                id_instrumento_archivistico = Me.DropDownList_instrumento.SelectedItem.Value
            End If
            Dim Refclas As New ClassGaProducionDocumental
            Result = Refclas.Agregar_expediente_produccion_carpeta_a_la_estructura(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                   Me.TextBox_nombre_expediente_carpeta.Text, _
                                                                                   nombre_serie, _
                                                                                   nombre_sub_serie, _
                                                                                   Me.DropDownList_fondo, _
                                                                                   Me.TreeViewArchivo.SelectedNode, _
                                                                                   Me.UpdatePanelViewArchivo, _
                                                                                   id_expediente_relacion, _
                                                                                   id_serie, _
                                                                                   id_sub_serie, _
                                                                                   Me.DropDownList_gabinete_producion.Text, _
                                                                                   id_nivel, _
                                                                                   id_instrumento_archivistico, _
                                                                                   Me.Hidden_rest_expe_tit_0009, _
                                                                                   Me.Hidden_rest_ur_expe_tit_0010, _
                                                                                   Me.TextBox_nombre_persona_expediente.Text, _
                                                                                   Me.TextBox_identificacion_persona_expediente.Text, _
                                                                                   Me.TextBox_asunto_expediente.Text, _
                                                                                   Me.TextBox_tema_expediente.Text, _
                                                                                   Me.TextBox_observacion_expediente.Text)
            If Result <> "YES" Then
                Me.Hidden_rest_agrre_exp_0011.Value = ""
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_add_expediente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If Me.CheckBox_hide_form.Checked = True Then
                    Me.Hidden_rest_agrre_exp_0011.Value = "YES"
                    Me.UpdatePanel_boton_add_expediente.Update()
                Else
                    Me.Hidden_parent_node_id.Value = ""
                    Me.Hidden_rest_agrre_exp_0011.Value = "YES"
                    Me.UpdatePanel_boton_add_expediente.Update()
                    Me.ModalPopupExtender_edition_agregar_expediente_carpeta.Hide()
                End If

            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_add_expediente, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_activa_busqueda_treview_Click(sender As Object, e As EventArgs) Handles Button_activa_busqueda_treview.Click
        Dim reclas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref As New ClassGestorSesion
            Dim Refclas As New ClassGaProducionDocumental
            Dim Refclas_niveles As New Class_niveles_organizacion
            Dim node As New TreeNode
            Dim k = Hidden_texto_buequeda.Value.Replace("__doPostBack('TreeViewArchivo',", "")
            k = k.Replace("'", "")
            k = k.Replace("(", "")
            k = k.Replace(")", "")
            k = k.Replace("s", "")
            k = k.Replace("||", "|")
            k = k.Replace("\\", "/")
            node = Me.TreeViewArchivo.FindNode(k)
            Dim Refclas_ As New ClassGaProducionDocumental
            Dim ref_value As Integer = 0
            If Not node Is Nothing Then
                Dim spli() As String = Nothing
                If InStr(node.Value, "|") = 0 Then
                    Session.Item("PG_SELECCION_ID_EXPEIDENTE") = 0
                Else
                    spli = node.Value.Split("|")
                    Session.Item("PG_SELECCION_ID_EXPEIDENTE") = spli(2)
                End If
                If Session.Item("SortExpression_produccion") = "" Then
                    Session.Item("SortExpression_produccion") = "ID_REGISTRO_PRODUCION_DOCUMENTAL"
                End If
                If Session.Item("SortDirection_produccion") = "" Then
                    Session.Item("SortDirection_produccion") = "DESC"
                End If
                Result = Refclas_.Lista_documentos_relacionados_expediente_producion(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                     Me.Page, _
                                                                                     1, _
                                                                                     "", _
                                                                                     Session.Item("SortExpression_produccion"), _
                                                                                     Session.Item("SortDirection_produccion"))
                If Result <> "YES" Then
                    reclas.Showscripman_menu(Result, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                node.Selected = True
                Result = Refclas_niveles.Auto_expand(node)
                If Result <> "YES" Then
                    reclas.Showscripman_menu(Result, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If        
        Catch ex As Exception
            reclas.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_eliminar_carpeta_Click(sender As Object, e As EventArgs) Handles Button_eliminar_carpeta.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.HiddenField_rest_0004.Value = ""
            If HiddenField_botones_respuesta.Value = "0" Then
                Exit Sub
            End If
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                clasjava.Showscripman_menu("Por favor seleccione el elemento para eliminar ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_exp As New ClassGaExpediente
            Dim Result As String = ""
            Dim index_nodo As Integer = 1
            Dim trednode As TreeNode = Nothing
            Dim Refclas As New ClassGaProducionDocumental
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                Dim Refclas_nivel As New Class_niveles_organizacion
                Result = Refclas_nivel.Eliminar_nivel_organizacion_expediente(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                              Val(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION")), _
                                                                              Me.TreeViewArchivo, _
                                                                              Me.UpdatePanelViewArchivo)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Dim split_sel() As String = Nothing
                split_sel = Me.Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
                Result = Refclas_exp.Eliminar_Expediente(split_sel(2), _
                                                         Session.Item("GA_IDUSUARIOGESTION"), _
                                                         HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                         HttpContext.Current.Session.Item("ip_host_name"), _
                                                         0, _
                                                         split_sel(1))
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                   
                End If
            End If
            Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = ""
            Me.HiddenField_rest_0004.Value = "YES"
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_actualizar_carpeta_Click(sender As Object, e As EventArgs) Handles Button_activa_actualizar_carpeta.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                clasjava.Showscripman_menu("Por favor seleccione el elemento para editar ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim index_nodo As Integer = 1
            Dim trednode As TreeNode = Nothing
            Dim Refclas As New ClassGaProducionDocumental
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                Session.Item("PG_SELECCION_ID_NIVEL") = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION")
                Me.TextBox_nombre_nivel_editar.Text = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION_TEXT")
                Me.UpdatePanel_editar_nivel.Update()
                Me.ModalPopupExtender_editar_nivel.Show()
            Else
                Dim split_sel() As String = Nothing
                Dim result_ref As String = ""
                split_sel = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
                Result = Refclas.Activa_editar_expediente_produccion(split_sel(1), _
                                                                     split_sel(2), _
                                                                     Session.Item("GA_IDUSUARIOGESTION"), _
                                                                     Me.TextBox_nombre_expediente_carpeta_actualizar, _
                                                                     Me.TextBox_nombre_persona_expediente_actualizar, _
                                                                     Me.TextBox_identificacion_persona_expediente_actualizar, _
                                                                     Me.TextBox_asunto_expediente_actualizar, _
                                                                     Me.TextBox_tema_expediente_actualizar, _
                                                                     Me.TextBox_observacion_expediente_actualizar, _
                                                                     Me.DropDownList_serie_documental_actualizar, _
                                                                     Me.DropDownList_sub_serie_asunto_actualizar, _
                                                                     Me.DropDownList_fondo_actualizar, _
                                                                     Me.UpdatePanel_actualizar_expediente_carpeta, _
                                                                     Me.DropDownList_gabinete_producion_edit, _
                                                                     Me.DropDownList_instrumento_edita, _
                                                                     result_ref, _
                                                                     Me.Button_agregar_expediente_actualizar)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_actualizar_expediente_carpeta.Show()
                    If result_ref <> "YES" Then
                        clasjava.Showscripman_menu(result_ref, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                End If
            End If
           
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub DropDownList_instrumento_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_instrumento.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            Result = Refclas.Seleccion_instrumento_producion_documental(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                        Me.DropDownList_serie_documental, _
                                                                        Me.DropDownList_sub_serie_asunto, _
                                                                        Me.DropDownList_instrumento, _
                                                                        Me.UpdatePanel_agregar_expediente_carpeta)

            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_agregar_expediente_carpeta)
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_agregar_expediente_carpeta)
        End Try
    End Sub

    Private Sub DropDownList_instrumento_edita_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_instrumento_edita.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            Result = Refclas.Seleccion_instrumento_producion_documental(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                      Me.DropDownList_serie_documental_actualizar, _
                                                                      Me.DropDownList_sub_serie_asunto_actualizar, _
                                                                      Me.DropDownList_instrumento_edita, _
                                                                      Me.UpdatePanel_actualizar_expediente_carpeta)

            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_actualizar_expediente_carpeta)
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_actualizar_expediente_carpeta)
        End Try
    End Sub
    Private Sub Button_agregar_expediente_actualizar_Click(sender As Object, e As EventArgs) Handles Button_agregar_expediente_actualizar.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_rest_result_agre_expe_tit_011.Value = ""
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                clasjava.Showscripman("Por favor seleccione el expediente para editar ", Me.UpdatePanel_botones_actualiza_expediente)
                Exit Sub
            End If
            Dim Refclas_exp As New ClassGaExpediente
            Dim Result As String = ""
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim id_istrumento As Integer = 0
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            If Not Me.DropDownList_instrumento_edita.SelectedItem Is Nothing Then
                id_istrumento = Me.DropDownList_instrumento_edita.SelectedItem.Value
            End If
            If Not Me.DropDownList_serie_documental_actualizar.SelectedItem Is Nothing Then
                nombre_serie = Me.DropDownList_serie_documental_actualizar.SelectedItem.Text
                id_serie = Me.DropDownList_serie_documental_actualizar.SelectedItem.Value
            End If
            If Not Me.DropDownList_sub_serie_asunto_actualizar.SelectedItem Is Nothing Then
                nombre_sub_serie = Me.DropDownList_sub_serie_asunto_actualizar.SelectedItem.Text
                id_sub_serie = Me.DropDownList_sub_serie_asunto_actualizar.SelectedItem.Value
            End If
            Dim nombre_gabinete As String = ""
            If Not Me.DropDownList_gabinete_producion_edit.SelectedItem Is Nothing Then
                nombre_gabinete = Me.DropDownList_gabinete_producion_edit.SelectedItem.Text
            End If
            If nombre_gabinete = "" Then
                clasjava.Showscripman_menu("Por favor selecione el gabinete de produción ", Me.UpdatePanel_botones_actualiza_expediente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split_sel() As String = Nothing
            split_sel = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            Result = Refclas_exp.Actualiza_expediente_produccion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                 Me.TextBox_nombre_expediente_carpeta_actualizar.Text, _
                                                                 Me.TextBox_nombre_persona_expediente_actualizar.Text, _
                                                                 Me.TextBox_identificacion_persona_expediente_actualizar.Text, _
                                                                 Me.TextBox_asunto_expediente_actualizar.Text, _
                                                                 Me.TextBox_tema_expediente_actualizar.Text, _
                                                                 Me.TextBox_observacion_expediente_actualizar.Text _
                                                                 , nombre_serie _
                                                                 , nombre_sub_serie, _
                                                                 Me.DropDownList_fondo_actualizar, _
                                                                 split_sel(2) _
                                                                  , Me.TreeViewArchivo.SelectedNode, _
                                                                  Me.UpdatePanelViewArchivo, _
                                                                  HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                                  HttpContext.Current.Session.Item("ip_host_name"), _
                                                                  id_serie, _
                                                                  id_sub_serie, _
                                                                  nombre_gabinete, _
                                                                  id_istrumento)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_actualiza_expediente, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION_TEXT") = Me.TextBox_nombre_expediente_carpeta_actualizar.Text
                Me.Hidden_rest_result_agre_expe_tit_011.Value = "YES"
                Me.ModalPopupExtender_edition_actualizar_expediente_carpeta.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_actualiza_expediente, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub DropDownList_serie_documental_actualizar_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_serie_documental_actualizar.SelectedIndexChanged
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            If Me.DropDownList_serie_documental_actualizar.SelectedItem Is Nothing Then Exit Sub
            Result = Refclas.Solicita_sub_series_documentales_relacionadas_a_la_serie_documental_item(Me.DropDownList_serie_documental_actualizar.SelectedItem.Value, _
                                                                                                      Me.DropDownList_sub_serie_asunto_actualizar, _
                                                                                                      Me.UpdatePanel_actualizar_expediente_carpeta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_actualizar_expediente_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_actualizar_expediente_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_Activa_Agregar_archivo_Click(sender As Object, e As EventArgs) Handles Button_Activa_Agregar_archivo.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                clasjava.Showscripman_menu("Por favor seleccione el expediente  para adjuntar el documentos", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_exp As New ClassGaExpediente
            Dim Result As String = ""
            Dim index_nodo As Integer = 1
            Dim trednode As TreeNode = Nothing
            Dim Refclas As New ClassGaProducionDocumental
            Dim estado_expediente As Integer = 1
            Dim estado_publico As Integer = 1   
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                clasjava.Showscripman_menu("Por favor seleccione un expediente para adjuntar archivo ", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split() As String = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            Result = Refclas_exp.Retorna_estado_expediente(Val(split(2)),
                                                           estado_expediente,
                                                           estado_publico)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_expediente <> 1 Then
                clasjava.Showscripman_menu("No se puede adjuntar el documento al expediente por que está cerrado", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'Dim Ref_class As New ClassGaProducionDocumental
            'Dim Extension_permitida As String = ""
            'Result = Ref_class.Solicita_listado_extension_de_archivos_permitidas(Extension_permitida)
            'If Result <> "YES" Then
            '    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            'If Extension_permitida = "" Then
            '    clasjava.Showscripman_menu("El sistema no registra extensiones permitidas", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            'Label_estado_carga.Text = "Solo puede cargar un archivo a la vez "
            'Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
            'AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
            'AjaxFileUpload_dowload.AllowedFileTypes = Extension_permitida & ",TIFF"
            'Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
            'AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
            'UpdatePanel_descarga.Update()
            'Result = Refclas.Solicitar_agregar_documento_a_carpeta_expediente(split(2), _
            '                                                                  Session.Item("GA_IDUSUARIOGESTION"), _
            '                                                                  split(1), _
            '                                                                  Me.DropDownList_tipo_documento, _
            '                                                                  Me.TextBox_ruta_archivo, _
            '                                                                  Me.TextBox_nombre_archivo, _
            '                                                                  Me.UpdatePanel_agrega_documento_carpeta_expediente)
            'If Result <> "YES" Then
            '    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'Else
            '    Me.ModalPopupExtender_edition_agrega_documento_carpeta_expediente.Show()
            'End If
            Result = Refclas.Solicitar_agregar_documento_a_carpeta_expediente(split(2),
                                                                              Session.Item("GA_IDUSUARIOGESTION"),
                                                                              split(1),
                                                                              Me.DropDownList_adjunta_documento,
                                                                              Me.Update_actualiza_adjunta_documento)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "PRODUCCION"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
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
                Dim anti As New AntiVirus.Scanner
                Dim fil_name As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
                Dim rest = anti.ScanAndClean(ruta_fisica & fil_name)
                If rest = 1 Then
                    Session.Item("WF_ERROR_RESPUESTA") = "El archivo tiene virus " & ruta_fisica & fil_name
                    Kill(ruta_fisica & fil_name)
                Else
                    Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
                End If

            End If
        Catch ex As Exception
            Session.Item("WF_ERROR_RESPUESTA") = ex.Message

        End Try
    End Sub
    Private Sub Button_guardar_desicion_Click(sender As Object, e As EventArgs) Handles Button_guardar_desicion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                clasjava.Showscripman(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga)
                Exit Sub
            End If          
            If Session.Item("WF_TIPO_ADJUNTA") = "VISOR" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                Dim file_inf As New FileInfo(Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                Me.TextBox_nombre_archivo.Text = file_inf.Name
                Me.TextBox_ruta_archivo.Text = Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                Me.UpdatePanel_agrega_documento_carpeta_expediente.Update()
                Dim Ref_class As New ClassGaProducionDocumental
                Dim index_nodo As Integer = 1
                Dim trednode As TreeNode = Nothing
                Dim Refclas As New ClassGaProducionDocumental
                Dim id_registro As Integer = 0
                Result = Ref_class.Activa_agregar_documento_carpeta_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                               Session.Item("GA_IDUSUARIOGESTION"), _
                                                                               Me.TextBox_nombre_documento.Text, _
                                                                               Me.TextBox_ruta_archivo.Text, _
                                                                               Me.DropDownList_tipo_documento, _
                                                                               id_registro, _
                                                                               Me.data_grid, _
                                                                               Me.UpdateGeneral_documentos, _
                                                                               Me.HiddenField_rest_des.Value)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_descarga, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    If Me.data_grid.Rows.Count > 0 Then
                        Me.Hidden0001.Value = id_registro
                    Else
                        
                    End If
                    Me.hdnEmailID.Value = "-1"
                    Me.UpdatePanel_agrega_documento_carpeta_expediente.Update()
                    'Me.UpdateGeneral_documentos.Update()
                    Me.ModalPopupExtender_edition_agrega_documento_carpeta_expediente.Hide()
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_descarga)
        Finally
            ModalPopupExtender_sube_documento_adjunto.Hide()
        End Try
    End Sub
    Private Sub Button_inline_trevie_Click(sender As Object, e As EventArgs) Handles Button_inline_trevie.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            Dim ref_value As Integer = 0
            Dim split_sel() As String = Nothing
            Dim Split_title() As String = Nothing
            Dim Tagform As String = ""
            If InStr(HiddenField_0003.Value, "\\") Then
                Split_title = HiddenField_0003.Value.Split("\\")
                Tagform = Split_title(0)
            Else
                Tagform = HiddenField_0003.Value
            End If
            Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION_TEXT") = Me.HiddenField_rest_text_node.Value
            Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = Tagform
            If InStr(Tagform, "|") = 0 Then
                Session.Item("PG_SELECCION_ID_EXPEIDENTE") = 0
            Else
                split_sel = Tagform.Split("|")
                Session.Item("PG_SELECCION_ID_EXPEIDENTE") = split_sel(2)
            End If
            If Session.Item("SortExpression_produccion") = "" Then
                Session.Item("SortExpression_produccion") = "ID_REGISTRO_PRODUCION_DOCUMENTAL"
            End If
            If Session.Item("SortDirection_produccion") = "" Then
                Session.Item("SortDirection_produccion") = "DESC"
            End If
            Result = Refclas.Lista_documentos_relacionados_expediente_producion(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                Me.Page, _
                                                                                1, _
                                                                                "", _
                                                                                Session.Item("SortExpression_produccion"), _
                                                                                Session.Item("SortDirection_produccion"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
            Session.Item("PG_SELECCION_ID_EXPEIDENTE") = 0
        End Try
    End Sub
    Private Sub TreeViewArchivo_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeViewArchivo.SelectedNodeChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Tagform As String = sender.selectedvalue()
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            Dim ref_value As Integer = 0
            Dim split_sel() As String = Nothing
            If Me.TreeViewArchivo.SelectedValue <> "" Then
                ref_value = Val(Me.TreeViewArchivo.SelectedValue)
            End If
            If InStr(Me.TreeViewArchivo.SelectedNode.Value, "|") = 0 Then
                Session.Item("PG_SELECCION_ID_EXPEIDENTE") = 0
            Else
                split_sel = Me.TreeViewArchivo.SelectedNode.Value.Split("|")
                Session.Item("PG_SELECCION_ID_EXPEIDENTE") = split_sel(2)
            End If
            If Session.Item("SortExpression_produccion") = "" Then
                Session.Item("SortExpression_produccion") = "ID_REGISTRO_PRODUCION_DOCUMENTAL"
            End If
            If Session.Item("SortDirection_produccion") = "" Then
                Session.Item("SortDirection_produccion") = "DESC"
            End If

            Result = Refclas.Lista_documentos_relacionados_expediente_producion(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                Me.Page, _
                                                                                1, _
                                                                                "", _
                                                                                 Session.Item("SortExpression_produccion"), _
                                                                                 Session.Item("SortDirection_produccion"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                'Session.Item("PG_SELECCION_ID_EXPEIDENTE") = split_sel(2)
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanelViewArchivo, "ModalPopupExtender_mensaje_personalizado")
            Session.Item("PG_SELECCION_ID_EXPEIDENTE") = 0
        End Try
    End Sub
    Private Sub Button_actualiza_Click(sender As Object, e As EventArgs) Handles Button_actualiza.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = 0 Then Exit Sub
            'If Session.Item("SortExpression_produccion") = "" Then
            '    Session.Item("SortExpression_produccion") = "ID_REGISTRO_PRODUCION_DOCUMENTAL"
            'End If
            'If Session.Item("SortDirection_produccion") = "" Then
            '    Session.Item("SortDirection_produccion") = "DESC"
            'End If
            Session.Item("SortExpression_produccion") = "ID_REGISTRO_PRODUCION_DOCUMENTAL"
            Session.Item("SortDirection_produccion") = "DESC"
            Result = Refclas.Lista_documentos_relacionados_expediente_producion(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                Me.Page, _
                                                                                1, _
                                                                                "", _
                                                                                Session.Item("SortExpression_produccion"), _
                                                                                Session.Item("SortDirection_produccion"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub data_grid_DataBound(sender As Object, e As EventArgs) Handles data_grid.DataBound
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

    Private Sub data_grid_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid.PageIndexChanging
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            If Session.Item("SortExpression_produccion") = "" Then
                Session.Item("SortExpression_produccion") = "ID_REGISTRO_PRODUCION_DOCUMENTAL"
            End If
            If Session.Item("SortDirection_produccion") = "" Then
                Session.Item("SortDirection_produccion") = "DESC"
            End If
            Result = Refclas.Lista_documentos_relacionados_expediente_producion(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                Me.Page, _
                                                                                Session.Item("GA_TIPO_CONSULTA_DOC_PRODUCCION"), _
                                                                                Session.Item("GA_DATO_CONSULTA_DOC_PRODUCCION"), _
                                                                                Session.Item("SortExpression_produccion"), _
                                                                                Session.Item("SortDirection_produccion"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub data_grid_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid.RowCreated
        'e.Row.Cells(0).Visible = False
        e.Row.Cells(1).Visible = False
        e.Row.Cells(2).Visible = False
        e.Row.Cells(3).Visible = False
        e.Row.Cells(4).Visible = False
        e.Row.Cells(10).Visible = False
    End Sub
    Private Sub data_grid_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid.Sorting
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            If Session.Item("SortExpression_produccion") = "" Then
                Session.Item("SortExpression_produccion") = "ID_REGISTRO_PRODUCION_DOCUMENTAL"
            End If
            If Session.Item("SortDirection_produccion") = "" Then
                Session.Item("SortDirection_produccion") = "DESC"
            End If
            Session.Item("SortExpression_produccion") = e.SortExpression
            If Session.Item("SortDirection_produccion") = "DESC" Then
                Session.Item("SortDirection_produccion") = "ASC"
            Else
                Session.Item("SortDirection_produccion") = "DESC"
            End If
            Result = Refclas.Lista_documentos_relacionados_expediente_producion(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                Me.Page, _
                                                                                Session.Item("GA_TIPO_CONSULTA_SOLICITUD_PRODUCCION"), _
                                                                                Session.Item("GA_DATO_CONSULTA_SOLICITUD_PRODUCCION"), _
                                                                                Session.Item("SortExpression_produccion"), _
                                                                                Session.Item("SortDirection_produccion"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral_documentos, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub ButtonButtonEditar_Click(sender As Object, e As EventArgs) Handles ButtonButtonEditar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            If Me.hdnEmailID.Value = "-1" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el documento de la lista para editar los datos", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Asigna_datos_interface_edicion_archivo(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                    Me.hdnEmailID.Value, _
                                                                    Me.TextBox_nombre_documento_edita, _
                                                                    Me.DropDownList_tipo_documento_edita, _
                                                                    UpdatePanel_edita_documento_carpeta_expediente)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_edita_documento_carpeta_expediente.Show()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_editar_archivo_expediente_Click(sender As Object, e As EventArgs) Handles Button_editar_archivo_expediente.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaProducionDocumental
            Dim Result As String = ""
            If Me.hdnEmailID.Value = "-1" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el documento de la lista para editar los datos", Me.UpdatePanel_edita_archivo_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Activa_editar_documento_carpeta_expediente(Me.hdnEmailID.Value, _
                                                                        Me.TextBox_nombre_documento_edita.Text, _
                                                                        Me.DropDownList_tipo_documento_edita)
            If Result <> "YES" Then
                Hidden0001_edita.Value = ""
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_edita_archivo_boton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If Not Me.DropDownList_tipo_documento_edita.SelectedItem Is Nothing Then
                    Hidden0002_edita.Value = Me.DropDownList_tipo_documento_edita.SelectedItem.Text
                Else
                    Hidden0002_edita.Value = ""
                End If
                Hidden0001_edita.Value = "1"
                Me.ModalPopupExtender_edition_edita_documento_carpeta_expediente.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_edita_archivo_boton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ButtonVerDocumento_Click(sender As Object, e As EventArgs) Handles ButtonVerDocumento.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "-1" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el documento de la lista para visualizar", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas_producion As New ClassGaProducionDocumental
            Result = Refclas_producion.Activa_visualizacion_documento_producion(Session.Item("GA_IDUSUARIOGESTION"),
                                                                                Me.hdnEmailID.Value,
                                                                                Me.Iframe_visor_externo_da_,
                                                                                Me.UpdatePanel_visor_externo,
                                                                                Me.ModalPopupExtender_visor_externo)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub

    Private Sub ButtonDescarga_Click(sender As Object, e As EventArgs) Handles ButtonDescarga.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "-1" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el documento de la lista para descargar", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim Refclas As New ClassGaProducionDocumental
            Dim Result As String = ""
            Result = Refclas.Descarga_documento_producion_documental(Session.Item("GA_IDUSUARIOGESTION"),
                                                                     Me.hdnEmailID.Value,
                                                                     Me.ifmExcel_,
                                                                     Me.updatapanel_iframe,
                                                                     Me.Hidden_ruta_archivo)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonRadicar_Click(sender As Object, e As EventArgs) Handles ButtonRadicar.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Refclas_exp As New ClassGaExpediente
            Dim estado_expediente As Integer = 1
            Dim estado_publico As Integer = 1
            Dim Result As String = ""
            Result = Refclas_exp.Retorna_estado_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                           estado_expediente, _
                                                           estado_publico)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_expediente <> 1 Then
                Refclasjava.Showscripman_menu("No se puede adjuntar el documento a la carpeta o expediente por que está cerrado", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            If Session.Item("GA_Radicar_enviar_documento") = 0 Then
                Refclasjava.Showscripman_menu("El usuario no tiene permiso para radicar " _
                                              , Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID.Value = "-1" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el documento de la lista para radicar " _
                                             , Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Erase Session.Item("RA_ADJUNTOS_RADICADO_INTERNO")
            If Me.hdnEmailID_sel.Value <> "0" Then
                Dim split_selecion_dcou_produc() As String = Me.hdnEmailID_sel.Value.Split("|")
                Dim i_conter As Integer = 0
                For i As Integer = 0 To split_selecion_dcou_produc.Length - 1
                    If Val(Me.hdnEmailID.Value) <> Val(split_selecion_dcou_produc(i)) Then
                        ReDim Preserve Session.Item("RA_ADJUNTOS_RADICADO_INTERNO")(i_conter)
                        Session.Item("RA_ADJUNTOS_RADICADO_INTERNO")(i_conter) = split_selecion_dcou_produc(i)
                        i_conter = i_conter + 1
                    End If
                Next
            End If
            Dim Refclas As New ClassGaProducionDocumental
            If Not Session.Item("RA_ADJUNTOS_RADICADO_INTERNO") Is Nothing Then
                For i As Integer = 0 To Session.Item("RA_ADJUNTOS_RADICADO_INTERNO").Length - 1
                    Dim nombre_documento_radicado As String = ""
                    Dim registro_radicado As String = ""
                    Result = Refclas.Solicita_documento_radicado_produccion(Session.Item("RA_ADJUNTOS_RADICADO_INTERNO")(i), _
                                                                            nombre_documento_radicado, _
                                                                            registro_radicado)
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    If nombre_documento_radicado <> "" Then
                        Refclasjava.Showscripman_menu("El documento (" & nombre_documento_radicado & ") se encuentra relacionado radicado(" & registro_radicado & ") , imposible volver a radicar ", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                Next
            End If
            Dim id_plantilla_radicacion As Integer = 0
            Dim nombre_plantilla_radicacion As String = ""
            Dim id_relacion_gestion_remitente As Integer = 0
            Result = Refclas.Solicita_radicar_documento_produccion(Me.hdnEmailID.Value, _
                                                                   Session.Item("GA_IDUSUARIOGESTION"), _
                                                                   id_plantilla_radicacion, _
                                                                   nombre_plantilla_radicacion, _
                                                                   id_relacion_gestion_remitente)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("RA_MODULO_SELECCIONADO") = "RADICACION" & "|" & id_plantilla_radicacion & "|" & "RADICACION ENTRANTE" & "|" & "" & "|" & nombre_plantilla_radicacion & "|" & Me.hdnEmailID.Value & "|" & id_relacion_gestion_remitente
                Me.Iframe_radica_interno_da_.Attributes("SRC") = "../Radicador/WebFormRadicacionEntranteInterna.aspx"
                Me.UpdatePanel_radica_interno.Update()
                Me.ModalPopupExtender_radica_interno.Show()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_activa_compartir_documento_Click(sender As Object, e As EventArgs) Handles Button_activa_compartir_documento.Click
        Dim Refclasjava As New Classscrripjava
        Try

            If Me.hdnEmailID.Value = "-1" And Me.hdnEmailID_sel.Value = "0" Then
                Refclasjava.Showscripman_menu("Debe seleccionar los documentos de la lista para compartir", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassGaProducionDocumental
            Dim Result As String = ""
            Dim matri_id_produc() As Long = Nothing
            Dim i As Integer = 0
            If Me.hdnEmailID.Value <> "-1" Then
                ReDim Preserve matri_id_produc(i)
                matri_id_produc(i) = Val(Me.hdnEmailID.Value)
                i = i + 1
            End If
            If Me.hdnEmailID_sel.Value <> "0" Then
                Dim split_tempo() As String = Me.hdnEmailID_sel.Value.Split("|")
                For z As Integer = 0 To split_tempo.Length - 1
                    If split_tempo(z) <> Me.hdnEmailID.Value Then
                        ReDim Preserve matri_id_produc(i)
                        matri_id_produc(i) = Val(split_tempo(z))
                        i = i + 1
                    End If
                Next
            End If

            Result = Refclas.Compartir_documento_produccion_documental(Session.Item("GA_IDUSUARIOGESTION"),
                                                                       matri_id_produc,
                                                                       Me.Iframe_compartir_documento_,
                                                                       Me.UpdatePanel_autoriza_compartir_documento,
                                                                       Me.ModalPopupExtender_edition_autoriza_compartir_documento)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim result As String = ""
        Dim refclas As New ClassGaProducionDocumental
        Dim campo() As STRU_CAMPOS_GRIDVIEW
        ReDim Preserve campo(0)
        campo(0).valor_campo = "1"
        ReDim Preserve campo(1)
        campo(1).valor_campo = "oJO"
        ReDim Preserve campo(2)
        campo(2).valor_campo = "2017-01-01"
        result = refclas.Agrega_fila_gre(Me.data_grid, campo, Me.UpdateGeneral_documentos)
    End Sub

    Protected Sub Button_Digitaliza_Click(sender As Object, e As EventArgs) Handles Button_Digitaliza.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                clasjava.Showscripman_menu("Por favor seleccione el expediente o carpeta para adjuntar el documento digitalizado ", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_exp As New ClassGaExpediente
            Dim Result As String = ""
            Dim index_nodo As Integer = 1
            Dim trednode As TreeNode = Nothing
            Dim Refclas As New ClassGaProducionDocumental
            Dim Refclas_gabinete As New ClassDaGabinete
            Dim extension As String = ""
            Dim id_tipo_imagen As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Dim id_expediente As Integer = 0
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                clasjava.Showscripman_menu("Por favor seleccione un expediente para adjuntar archivo ", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split() As String = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            Dim estado_expediente As Integer = 0
            Dim estado_expediente_publico As Integer = 0
            Result = Refclas_exp.Retorna_estado_expediente(split(2), _
                                                           estado_expediente, _
                                                           estado_expediente_publico)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If estado_expediente <> 1 Then
                    clasjava.Showscripman_menu("El expediente esta cerrado imposible agregar un documento", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
                Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
                Dim estado_propietario As String = ""
                Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
                Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                              split(1), _
                                                                              estado_propietario)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If estado_propietario = "NO" Then
                    Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(split(1), _
                                                                                                           Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                           stru_permisos_niveles)
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    If stru_permisos_niveles.carga_archivo = 0 Then
                        clasjava.Showscripman_menu("El usuario no tiene persmisos para cargar archivos al expediente, el nivel al que pertenece el expediente es propiedad de otro usuario", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                End If
                If Me.hdnEmailID.Value <> "-1" Then
                    Result = Refclas.Solicita_datos_caracterizacion_archivo_produccion(Me.hdnEmailID.Value, _
                                                                                       "", _
                                                                                       "", _
                                                                                       0, _
                                                                                       id_expediente, _
                                                                                       id_imagen, _
                                                                                       nombre_gabinete, _
                                                                                       fecha_documento, _
                                                                                       numero_folios)
                    If Result <> "YES" Then
                        clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Session.Item("DG_SELECION_TREE") = nombre_gabinete & "|" & id_imagen
                Else
                    Session.Item("DG_SELECION_TREE") = ""
                End If
                HttpContext.Current.Session.Item("SELECCIONTEMPORAL") = Session.Item("DG_ID_EXPEDIENTE") & "|0"
                Session.Item("DG_ID_EXPEDIENTE") = split(2)
                Session.Item("DG_TIPODIGITALIZACION") = "PRODUCCION"
                HiddenIdFlujo.Value = Session.Item("SELECCIONTEMPORAL")
                Dim Ruta_Web_Escaner As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString.Replace("\", "/")
                HiddenRuta.Value = Ruta_Web_Escaner
                Me.Label_digitaliza_documento_adjunto.Text = "Cargando archivo digitalizado al expediente " & "(" & Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION_TEXT") & ")"
                Me.UpdatePanel_titule_digitaliza.Update()
                If Session.Item("DG_ESTADO_VENTA") = 0 Then
                    Me.IframeDitaliza_adjunto_.Attributes.Add("src", "../workflow/WebFormEscan.aspx")
                    UpdatePanel_iframe_digitaliza_adjunto.Update()
                End If
                Me.ModalPopupExtender_edition_digitaliza_documento_adjunto.Show()
                If Session.Item("DG_ESTADO_VENTA") = 0 Then
                    Session.Item("DG_ESTADO_VENTA") = 1
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_notificar_envio_Click(sender As Object, e As EventArgs) Handles Button_notificar_envio.Click

        Dim scripjava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "-1" And Me.Hidden_sele_docu.Value = "" Then
                scripjava.Showscripman_menu("Debe seleccionar el documento para adjuntar al correo electrónico", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclass As New ClassGaProducionDocumental
            Dim document_select() As Long = Nothing
            If Me.Hidden_sele_docu.Value <> "" Then
                If InStr(Me.Hidden_sele_docu.Value, "|") > 0 Then
                    Dim split_() As String = Me.Hidden_sele_docu.Value.Split("|")
                    For i As Integer = 0 To split_.Length - 1
                        ReDim Preserve document_select(i)
                        document_select(i) = Val(split_(i))
                    Next
                Else
                    ReDim Preserve document_select(0)
                    document_select(0) = Val(Me.Hidden_sele_docu.Value)
                End If

            Else
                ReDim Preserve document_select(0)
                document_select(0) = Val(Me.hdnEmailID.Value)
            End If
            Result = Refclass.Activa_envio_correo_electronico(Session.Item("GA_IDUSUARIOGESTION"), _
                                                              document_select, _
                                                              Me.Iframe_comparte_coreo, _
                                                              Me.UpdatePanel_iframenotifica, _
                                                              Me.ModalPopupExtender_notifica_gestion, _
                                                              Me.Hidden_cuenta_correo_envio, _
                                                              Me.Hidden_correo_envio_default)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    '-------almacena los documentos digitalizados
    Private Sub ButtonAlmacenar_Click(sender As Object, e As EventArgs) Handles ButtonAlmacenar.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Refclas As New ClassListandoTareas
            Dim Result As String = ""
            Dim Nombre_Ruta_Workflow As String = ""
            Dim RefclasDigitaliza As New ClassGaProducionDocumental
            Dim id_registro_copia As Long = 0
            Me.Hidden_001_inst_row.Value = ""
            If Session.Item("DG_TIPODIGITALIZACION") = "PRODUCCION" Then
                Result = RefclasDigitaliza.Guarda_documento_digitalizado_producion(Session.Item("DG_ID_EXPEDIENTE"), _
                                                                                   Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                   Session.Item("DG_NOMBRE_DOCUMENTO"), _
                                                                                   Session.Item("DG_SELECCION_TIPODOCUMENTO_EXPEDIENTE"), _
                                                                                   id_registro_copia, _
                                                                                   Me.data_grid, _
                                                                                   Me.UpdateGeneral_documentos, _
                                                                                   Me.Hidden_001_inst_row.Value)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_botones_comandos)
                    Exit Sub
                End If
            End If
           
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_botones_comandos)
        End Try
    End Sub

    Private Sub Button_busca_general_archivo_Click(sender As Object, e As EventArgs) Handles Button_busca_general_archivo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim refclass As New ClassGaProducionDocumental
            If Me.TextBox_buequeda_general.Text = "" Then Exit Sub
            If Session.Item("SortExpression_produccion") = "" Then
                Session.Item("SortExpression_produccion") = "ID_REGISTRO_PRODUCION_DOCUMENTAL"
            End If
            If Session.Item("SortDirection_produccion") = "" Then
                Session.Item("SortDirection_produccion") = "DESC"
            End If
            Result = refclass.Lista_documentos_relacionados_expediente_producion(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                 Me.Page, _
                                                                                 3, _
                                                                                 Me.TextBox_buequeda_general.Text, _
                                                                                 Session.Item("SortExpression_produccion"), _
                                                                                 Session.Item("SortDirection_produccion"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_label_resultado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_label_resultado, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_Restaura_busqueda_Click(sender As Object, e As EventArgs) Handles Button_Restaura_busqueda.Click
        Dim reclas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            Dim ref_value As Integer = 0
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                Exit Sub
            End If
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                Session.Item("PG_SELECCION_ID_EXPEIDENTE") = 0
            Else
                Dim split_sel() = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").ToString.Split("|")
                Session.Item("PG_SELECCION_ID_EXPEIDENTE") = split_sel(2)
            End If
            If Session.Item("SortExpression_produccion") = "" Then
                Session.Item("SortExpression_produccion") = "ID_REGISTRO_PRODUCION_DOCUMENTAL"
            End If
            If Session.Item("SortDirection_produccion") = "" Then
                Session.Item("SortDirection_produccion") = "DESC"
            End If
            Result = Refclas.Lista_documentos_relacionados_expediente_producion(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                Me.Page, _
                                                                                1, _
                                                                                "", _
                                                                                Session.Item("SortExpression_produccion"), _
                                                                                Session.Item("SortDirection_produccion"))
                If Result <> "YES" Then
                reclas.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

        Catch ex As Exception
            reclas.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_activa_descarga_dcoumento_plantila_Click(sender As Object, e As EventArgs) Handles Button_activa_descarga_dcoumento_plantila.Click
        Me.ModalPopupExtender_edition_descarga_documento_plantilla.Show()
    End Sub

    Protected Sub Button_descarga_plantilla_Click(sender As Object, e As EventArgs) Handles Button_descarga_plantilla.Click
        Dim reclas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaGembox
            Result = Refclas.Solicita_descargar_documento_plantilla(Me.DropDownList_lista_tipo_plantilla.SelectedValue,
                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                    Me.Hidden_ruta_archivo, Me.ifmExcel_, Me.updatapanel_iframe,
                                                                    Me.UpdatePanel_botones_unidad)

            If Result <> "YES" Then
                reclas.Showscripman_menu(Result, Me.UpdatePanel_descarga_documento_plantilla, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_descarga_documento_plantilla.Hide()
            End If
        Catch ex As Exception
            reclas.Showscripman_menu(ex.Message, Me.UpdatePanel_descarga_documento_plantilla, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

   
    Protected Sub Button_agregar_nivel_Click(sender As Object, e As EventArgs) Handles Button_agregar_nivel.Click
        Dim reclas As New Classscrripjava
        Try
            Dim Refclas As New Class_niveles_organizacion
            Dim Result As String = ""
            Dim id_node As Integer = 0
            Me.HiddenField_rest_0005.Value = ""
            Result = Refclas.Agregar_nivel_clasificacion_java(Session.Item("PG_SELECCION_ID_NIVEL"), _
                                                              TextBox_nombre_nivel.Text, _
                                                              HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                              Me.Hidden_rest_tit_0006, _
                                                              Me.Hidden_rest_val_0008, _
                                                              Me.Hidden_rest_ur_0007, _
                                                              id_node)
            If Result <> "YES" Then
                reclas.Showscripman(Result, Me.UpdatePanel_agregar_nivel)
            Else
                Me.HiddenField_rest_0005.Value = "YES"
                Me.ModalPopupExtender_agregar_nivel.Hide()
            End If
        Catch ex As Exception
            reclas.Showscripman(ex.Message, Me.UpdatePanel_agregar_nivel)
        End Try
    End Sub

    Private Sub Button_activa_nuevo_nivel_Click(sender As Object, e As EventArgs) Handles Button_activa_nuevo_nivel.Click
        Dim clasjava As New Classscrripjava
        Try
            Session.Item("PG_SELECCION_ID_NIVEL") = 0
            If InStr(HiddenField_0003.Value, "|") > 0 Then
                clasjava.Showscripman_menu("Por favor seleccion un nivel para agregar el nuevo nivel ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Dim split_() As String = Nothing
                If InStr(HiddenField_0003.Value, "\\") > 0 Then
                    split_ = HiddenField_0003.Value.Split("\\")
                    Session.Item("PG_SELECCION_ID_NIVEL") = Val(split_(0))
                Else
                    Session.Item("PG_SELECCION_ID_NIVEL") = 0
                End If
            End If
            Me.ModalPopupExtender_agregar_nivel.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_carpeta)
        End Try
    End Sub

    Private Sub Button_activa_eliminar_nivel_Click(sender As Object, e As EventArgs) Handles Button_activa_eliminar_nivel.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.HiddenField_rest_0004.Value = ""
            If HiddenField_botones_respuesta.Value = "0" Then
                Exit Sub
            End If
            If InStr(HiddenField_0003.Value, "|") > 0 Then
                clasjava.Showscripman_menu("Por favor seleccion un nivel para eliminar ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split_() As String = HiddenField_0003.Value.Split("\\")
            Session.Item("PG_SELECCION_ID_NIVEL") = split_(0)
            Dim Result As String = ""
            Dim Refclas As New Class_niveles_organizacion
            Result = Refclas.Eliminar_nivel_organizacion_expediente(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                    Val(split_(0)), _
                                                                    Me.TreeViewArchivo, _
                                                                    Me.UpdatePanelViewArchivo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.HiddenField_rest_0004.Value = "YES"
                Me.ModalPopupExtender_editar_nivel.Hide()
            End If
           
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_carpeta)
        End Try
    End Sub

    Private Sub Button_editar_nivel_Click(sender As Object, e As EventArgs) Handles Button_editar_nivel.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_res_edita_nivel_0001.Value = ""
            Dim Result As String = ""
            Dim Refclas As New Class_niveles_organizacion
            Result = Refclas.Editar_nivel_de_organizacion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                          Session.Item("PG_SELECCION_ID_NIVEL"), _
                                                          Me.TextBox_nombre_nivel_editar.Text, _
                                                          Me.TreeViewArchivo, _
                                                          Me.UpdatePanelViewArchivo)
            If Result <> "YES" Then
                Me.Hidden_res_edita_nivel_0001.Value = ""
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_editar_nivel_boton, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.UpdatePanel_editar_nivel.Update()
                Me.Hidden_res_edita_nivel_0001.Value = "YES"
                Me.ModalPopupExtender_editar_nivel.Hide()
            End If

        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_editar_nivel_boton)
        End Try
    End Sub


    Private Sub Button_activa_compartir_nivel_Click(sender As Object, e As EventArgs) Handles Button_activa_compartir_nivel.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.HiddenField_0003.Value = "" Then
                clasjava.Showscripman("Por favor seleccione el nivel de organización a compartir ", Me.UpdatePanel_botones_carpeta)
                Exit Sub
            End If
            If InStr(Me.HiddenField_0003.Value, "|") > 0 Then
                clasjava.Showscripman("Los expedientes o carpetas no se pueden compartir, seleccione un nivel de organización para compartir ", Me.UpdatePanel_botones_carpeta)
                Exit Sub
            End If
            Me.ModalPopupExtender_compartir_nivel.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_carpeta)
        End Try
    End Sub

    Private Sub Button_activa_lista_permiso_compartidos_nivel_Click(sender As Object, e As EventArgs) Handles Button_activa_lista_permiso_compartidos_nivel.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.HiddenField_0003.Value = "" Then
                clasjava.Showscripman_menu("Por favor seleccion un nivel de organización para listar los usuarios a quien se le ha compartido ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If InStr(Me.HiddenField_0003.Value, "|") > 0 Then
                clasjava.Showscripman_menu("Por favor seleccion un nivel de organización para listar los usuarios a quien se le ha compartido ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Ref_class As New Class_ra_pro_niveles
            Dim Ref_class_ra_pro_persmisos As New Class_ra_pro_permisos_niveles
            Dim Estado_propietario As String = ""
            Dim Result As String
            Result = Ref_class.Solicita_estado_nivel_propietario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                 Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), _
                                                                 Estado_propietario)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_botones_carpeta)
                Exit Sub
            End If
            If Estado_propietario = "NO" Then
                Result = Ref_class_ra_pro_persmisos.Lista_permisos_usuario_gestion_nivel(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                        Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), _
                                                                                        Me.Page)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_botones_carpeta)
                    Exit Sub
                End If
            Else
                Session.Item("SortExpression_colaboracion") = "id_permisos_niveles"
                Session.Item("SortDirection_colaboracion") = "DESC"
                Dim reflcas_respuesta As New Class_ra_pro_permisos_niveles
                Result = reflcas_respuesta.Solicita_listado_usuario_permisos_nivel(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), _
                                                                                   Me.data_grid_listado_permisos, _
                                                                                   Me.Label_title_permisos, _
                                                                                   Me.hdnEmailID, _
                                                                                   Me.UpdatePanel_listar_permisos_niveles, _
                                                                                   1, _
                                                                                   Me.TextBox_consulta_permisos.Text, _
                                                                                   Session.Item("SortExpression_colaboracion"), _
                                                                                   Session.Item("SortDirection_colaboracion"), _
                                                                                   Me.UpdatePanel_title_permisos)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.ModalPopupExtender_listar_permisos_niveles.Show()
                End If
            End If
           
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_carpeta)
        End Try
    End Sub

    Private Sub data_grid_listado_permisos_DataBound(sender As Object, e As EventArgs) Handles data_grid_listado_permisos.DataBound
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

    Private Sub data_grid_listado_permisos_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_permisos.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_listado_permisos.PageIndex = e.NewPageIndex
            Dim reflcas_respuesta As New Class_ra_pro_permisos_niveles
            Dim Result As String = reflcas_respuesta.Solicita_listado_usuario_permisos_nivel(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), _
                                                                                             Me.data_grid_listado_permisos, _
                                                                                             Me.Label_title_permisos, _
                                                                                             Me.hdnEmailID, _
                                                                                             Me.UpdatePanel_listar_permisos_niveles, _
                                                                                             Session.Item("GA_TIPO_CONSULTA_SOLICITUD_COLABORACION"), _
                                                                                             Session.Item("GA_DATO_CONSULTA_SOLICITUD_COLABORACION"), _
                                                                                             Session.Item("SortExpression_colaboracion"), _
                                                                                             Session.Item("SortDirection_colaboracion"), _
                                                                                             Me.UpdatePanel_title_permisos)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_listar_permisos_niveles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_listar_permisos_niveles)
        End Try
    End Sub

    Private Sub data_grid_listado_permisos_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_permisos.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub

   
    Private Sub data_grid_listado_permisos_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_listado_permisos.Sorting
        Dim clasjava As New Classscrripjava
        Try
            Session.Item("SortExpression_colaboracion") = e.SortExpression
            If Session.Item("SortDirection_colaboracion") = "DESC" Then
                Session.Item("SortDirection_colaboracion") = "ASC"
            Else
                Session.Item("SortDirection_colaboracion") = "DESC"
            End If
            Dim reflcas_respuesta As New Class_ra_pro_permisos_niveles
            Dim Result As String = reflcas_respuesta.Solicita_listado_usuario_permisos_nivel(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), _
                                                                                             Me.data_grid_listado_permisos, _
                                                                                             Me.Label_title_permisos, _
                                                                                             Me.hdnEmailID, _
                                                                                             Me.UpdatePanel_listar_permisos_niveles, _
                                                                                             Session.Item("GA_TIPO_CONSULTA_SOLICITUD_COLABORACION"), _
                                                                                             Session.Item("GA_DATO_CONSULTA_SOLICITUD_COLABORACION"), _
                                                                                             Session.Item("SortExpression_colaboracion"), _
                                                                                             Session.Item("SortDirection_colaboracion"), _
                                                                                             Me.UpdatePanel_title_permisos)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_listar_permisos_niveles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_listar_permisos_niveles)
        End Try
    End Sub

    Private Sub Button_eliminar_regi_permiso_Click(sender As Object, e As EventArgs) Handles Button_eliminar_regi_permiso.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_00_09.Value = "-1"
            If HiddenField_botones_respuesta.Value = "0" Then
                Exit Sub
            End If
            Me.Hidden_rest_ur_permiso_elimina_0007.value = ""
            Dim Result As String = ""
            Dim Refclas As New Class_niveles_organizacion
            Result = Refclas.Dejar_de_compartir_nivel_organizacion_usuario_gestion(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), _
                                                                                   Me.Hidden_sel.Value, _
                                                                                   Me.TreeViewArchivo.SelectedNode, _
                                                                                   Me.UpdatePanelViewArchivo, _
                                                                                   Me.Label_title_permisos, _
                                                                                   Me.UpdatePanel_title_permisos, _
                                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                   Me.Hidden_rest_ur_permiso_elimina_0007)
            If Result <> "YES" Then
                Me.Hidden_00_09.Value = "-1"
                clasjava.Showscripman(Result, Me.UpdatePanel_botones_lista)
            Else
                Me.Hidden_00_09.Value = "1"
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_lista)
        End Try
    End Sub

    Private Sub Button_activa_busqueda_Click(sender As Object, e As EventArgs) Handles Button_activa_busqueda.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim reflcas_respuesta As New Class_ra_pro_permisos_niveles
            Dim Result As String = reflcas_respuesta.Solicita_listado_usuario_permisos_nivel(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), _
                                                                                             Me.data_grid_listado_permisos, _
                                                                                             Me.Label_title_permisos, _
                                                                                             Me.hdnEmailID, _
                                                                                             Me.UpdatePanel_listar_permisos_niveles, _
                                                                                             2, _
                                                                                             Me.TextBox_consulta_permisos.Text, _
                                                                                             Session.Item("SortExpression_colaboracion"), _
                                                                                             Session.Item("SortDirection_colaboracion"), _
                                                                                             Me.UpdatePanel_title_permisos)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_botones_lista)
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_lista)
        End Try
    End Sub

    Private Sub Button_restaura_consulta_Click(sender As Object, e As EventArgs) Handles Button_restaura_consulta.Click
        Dim clasjava As New Classscrripjava
        Try
            Session.Item("SortExpression_colaboracion") = "USUARIO_COMPARTIDO"
            Session.Item("SortDirection_colaboracion") = "DESC"
            Dim reflcas_respuesta As New Class_ra_pro_permisos_niveles
            Dim Result As String = reflcas_respuesta.Solicita_listado_usuario_permisos_nivel(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), _
                                                                                             Me.data_grid_listado_permisos, _
                                                                                             Me.Label_title_permisos, _
                                                                                             Me.hdnEmailID, _
                                                                                             Me.UpdatePanel_listar_permisos_niveles, _
                                                                                             1, _
                                                                                             "", _
                                                                                             Session.Item("SortExpression_colaboracion"), _
                                                                                             Session.Item("SortDirection_colaboracion"), _
                                                                                             Me.UpdatePanel_title_permisos)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_botones_carpeta)
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_carpeta)
        End Try
    End Sub

    Private Sub Button_activa_ubicacion_archivo_Click(sender As Object, e As EventArgs) Handles Button_activa_ubicacion_archivo.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "-1" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el documento de la lista a copiar", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclass As New Class_ra_pro_niveles_has_expediente_archivo
            Result = Refclass.Solicita_ubicacion_expediente_estructura(Val(Me.hdnEmailID.Value), _
                                                                       Me.TextBox_nivel_ubicacion.Text, _
                                                                       Me.TextBox_expediente_ubicacion.Text, _
                                                                       Me.TextBox_propietario_nivel_ubicacion.Text, _
                                                                       Me.TextBox_cargo_propietario_nivel.Text, _
                                                                       Me.UpdatePanel_ubicacion_documento, _
                                                                       Me.ModalPopupExtender_ubicacion_documento)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_unidad)

        End Try
    End Sub

    
    Private Sub Button_pogres_show_Click(sender As Object, e As EventArgs) Handles Button_pogres_show.Click
        Me.ModalPopupExtender_edition_pro_gres_bar.Show()
    End Sub

    Private Sub Button_cerrar_pro_gres_bar_Click(sender As Object, e As EventArgs) Handles Button_cerrar_pro_gres_bar.Click
        Me.ModalPopupExtender_edition_pro_gres_bar.Hide()
    End Sub

    Private Sub WebFormProducionDocumental_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        HttpContext.Current.Session.Item("GA_ROMULARIO_WEB") = Me

    End Sub

    Private Sub Button_copia_update_Click(sender As Object, e As EventArgs) Handles Button_copia_update.Click
        Me.UpdateGeneral_documentos.Update()
        Me.UpdatePanel_label_resultado.Update()
    End Sub

    Protected Sub Button_archivar_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_archivar_expediente_gestion.Click
        Dim Refclas_empresa As New ClassAdmonEmpresa
        Dim scripjava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaExpediente
            Dim id_empresa_gestion As Integer = 0
            Dim Result As String = ""
            'If Session.Item("GA_MANAGER_GESTION") <> 1 Then
            '    If Session.Item("GA_ARCHIVA_EXPEDIENTES") = 0 Then
            '        scripjava.Showscripman_menu("Usuario sin permisos para archivar expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If
            '    Result = Refclas.Verifica_propiedad_usuario_expediente(Me.hdnEmailID.Value, _
            '                                                           HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '    If Result <> "YES" Then
            '        scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If
            'End If
            Dim split() As String = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                scripjava.Showscripman_menu("Por favor seleccione un expediente para archivar ", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                          split(1), _
                                                                          estado_propietario)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(split(1), _
                                                                                                       Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If stru_permisos_niveles.editar_expediente = 0 Then
                    scripjava.Showscripman_menu("El usuario no tiene persmisos para editar o archivar el expediente, el nivel al que pertenece el expediente es propiedad de otro usuario", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "0" Or Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar un expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Result = Refclas_empresa.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa_r_u_e, _
                                                                      Me.UpdatePanelEntidad_r_u_e)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.UpdatePanelEntidad_r_u_e.Update()
            Dim refclas_unidad As New ClassGestionArchivo
            If Session.Item("GA_ESTADO_ARCHIVA_DOCUMENTO") = 0 Then
                Result = refclas_unidad.Listar_Entidad_Archivo_Edificio(Me.TreeViewArchivo_r_u_e, _
                                                                        Me.DropDownListEntidadEmpresa_r_u_e.Text)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("GA_ESTADO_ARCHIVA_DOCUMENTO") = 1
                    UpdatePanelViewArchivo_r_u_e.Update()
                End If
            End If
            ModalPopupExtende_reubicar_unidad_expediente_popup.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub TreeViewArchivo_r_u_e_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeViewArchivo_r_u_e.SelectedNodeChanged
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Tagform As String = sender.selectedvalue()
            Dim Result As String = ""
            Dim Refclas As New ClassGestionArchivo
            Result = Refclas.Seleccion_Treview_archivar(Tagform, _
                                                        Me.TreeViewArchivo_r_u_e.SelectedNode, _
                                                        Me.DropDownListEntidadEmpresa_r_u_e.Text, _
                                                        Me.TreeViewArchivo_r_u_e.SelectedNode.Value, _
                                                        Me.TreeViewArchivo_r_u_e.SelectedNode.Text, _
                                                        Me.TreeViewArchivo_r_u_e)
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
    Private Sub Button_agrega_unidad_contenedora_Click(sender As Object, e As EventArgs) Handles Button_agrega_unidad_contenedora.Click
        Dim scripjava As New Classscrripjava
        Try
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
                scripjava.Showscripman_menu("Por favor seleccione el entrepaño para agregar la unidad", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then
                scripjava.Showscripman_menu("Por favor seleccione el entrepaño", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub

            End If
            If InStr(Me.TreeViewArchivo_r_u_e.SelectedNode.Value, "ENTREPAÑO") <= 0 Then
                scripjava.Showscripman_menu("Por favor seleccione el entrepano", Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
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
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e)
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
            Dim Result = refclasunidad.Listar_unidades_conservacion_contenedoras_de_unidades_conservacion(id_entrepaño, _
                                                                                                          1, _
                                                                                                          estru_unidad)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad_r_u_e, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim no_tag_ref As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value

            Result = refclasunidad.Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion(Me.TreeViewArchivo_r_u_e, estru_unidad, _
                                                                                                 no_tag_ref, _
                                                                                                 Me.TreeViewArchivo_r_u_e.SelectedNode.Text, _
                                                                                                 Me.TreeViewArchivo_r_u_e.SelectedNode)
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
                Result = refclas.Verifica_propiedad_usuario_unidad_conservacion(Val(spli_unidad_contenedora(0)), _
                                                                                Session.Item("GA_IDUSUARIOGESTION"))
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
                                                                                            Me.TreeViewArchivo_r_u_e.SelectedNode, _
                                                                                            Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            Me.TreeViewArchivo_r_u_e, _
                                                                                            Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                                                            Session.Item("ip_host_name"), _
                                                                                            UpdatePanelViewArchivo_r_u_e)
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

    Private Sub Button_editar_unidad_contenedora_Click(sender As Object, e As EventArgs) Handles Button_editar_unidad_contenedora.Click
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
            Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = Me.DropDownListEntidadEmpresa_r_u_e.Text
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
            If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "0" Or Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "-1" Then
                clasjava.Showscripman("Debe seleccionar un expediente para archivar", Me.UpdatePanel_botones_unidad_r_u_e)
                Exit Sub
            End If
            If Me.TreeViewArchivo_r_u_e.SelectedNode Is Nothing Then Exit Sub
            If Me.TreeViewArchivo_r_u_e.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.ToString.Split("|")
                Result = Refclas.Archiva_expediente_unidad_contenedora_Archivado(split(0), _
                                                                                 Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                 HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                                                 HttpContext.Current.Session.Item("ip_host_name"))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_botones_unidad_r_u_e)
                    Exit Sub
                Else
                    ModalPopupExtende_reubicar_unidad_expediente_popup.Hide()
                End If
            End If
            If InStr(Me.TreeViewArchivo_r_u_e.SelectedNode.Text, "Entrepaño") > 0 Then
                Dim split() As String = Me.TreeViewArchivo_r_u_e.SelectedNode.Value.ToString.Split("|")
                Result = Refclas.Archiva_expediente_en_entrepano_archivado(split(0), _
                                                                           Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                           HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                           HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                                           HttpContext.Current.Session.Item("ip_host_name"))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_botones_unidad_r_u_e)
                    Exit Sub
                Else
                    ModalPopupExtende_reubicar_unidad_expediente_popup.Hide()
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_unidad_r_u_e)
        End Try
    End Sub
    Private Sub Button_Editar_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_Editar_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try

            If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "0" Or Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para editar", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_empresa As New ClassAdmonEmpresa
            Dim Result = Refclas_empresa.Listar_Empresa_de_Gestion_Activa(Me.DropDownListEntidadEmpresa_r_u_e, _
                                                                          Me.UpdatePanelEntidad_r_u_e)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split() As String = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                clasjava.Showscripman_menu("Por favor seleccione un expediente para editar ", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                          split(1), _
                                                                          estado_propietario)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim estado_edita As String = "EDITAR"
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(split(1), _
                                                                                                       Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If stru_permisos_niveles.editar_expediente = 1 Then
                    estado_edita = "EDITAR-PER"
                End If
            End If
            Session.Item("SESIONITERCAMBIOEXPEDIENTE") = Me.DropDownListEntidadEmpresa_r_u_e.Text & "|" & Session.Item("PG_SELECCION_ID_EXPEIDENTE") & "|" & estado_edita
            Me.Iframe_agregar_expdiente_popup_.Attributes.Add("src", "../gestion/WebFormGaEditarExpediente.aspx")
            Me.Hidden_estado_editar.Value = "YES"
            UpdatePanel_agregar_expdiente_popup.Update()
            Me.ModalPopupExtende_agregar_expdiente_popup.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_desachivar_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_desachivar_expediente_gestion.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "0" Or Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "-1" Then
                scripjava.Showscripman_menu("Debe seleccionar un expediente para desarchivar", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassGaExpediente
            'If Session.Item("GA_MANAGER_GESTION") <> 1 Then
            '    If Session.Item("GA_ARCHIVA_EXPEDIENTES") = 0 Then
            '        scripjava.Showscripman_menu("Usuario sin permisos para desarchivar expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If

            '    Result = Refclas.Verifica_propiedad_usuario_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
            '                                                           Session.Item("GA_IDUSUARIOGESTION"))
            '    If Result <> "YES" Then
            '        scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If

            'End If
            Dim split() As String = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                scripjava.Showscripman_menu("Por favor seleccione un expediente para desarchivar ", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                          Split(1), _
                                                                          estado_propietario)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(Split(1), _
                                                                                                       Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If stru_permisos_niveles.editar_expediente = 0 Then
                    scripjava.Showscripman_menu("El usuario no tiene persmisos para editar o desarchivar el expediente, el nivel al que pertenece el expediente es propiedad de otro usuario", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            If HiddenField_botones_respuesta.Value = "1" Then
                Result = Refclas.Des_Archiva_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                        Session.Item("GA_IDUSUARIOGESTION"), _
                                                        Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                        Session.Item("ip_host_name"))
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                HiddenField_botones_respuesta.Value = "-1"
            End If
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_ubicacio_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_ubicacio_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "0" Or Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para mostrar la ubicación toponimica", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_Ubicacion_expediente_por_codigo_unico(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), Me.TreeViewArchivo_u_b_t, "")
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
    Private Sub ButtonRotulo_Click(sender As Object, e As EventArgs) Handles ButtonRotulo.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "0" Or Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para descargar el rotulo", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
           
            Dim Result As String = ""
            Dim Refclasexp As New ClassGaExpediente
            '------------------------------------------------------
            'Retorna plantilla impresion usuario gestión
            '------------------------------------------------------
            Dim nombre_plantilla_impresion As String = ""
            Dim id_configuracion_plantilla_rotulo As Integer = 0
            Result = Refclasexp.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                 id_configuracion_plantilla_rotulo, _
                                                                                                 nombre_plantilla_impresion)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If nombre_plantilla_impresion = "" Then
                nombre_plantilla_impresion = "DEFAULT"
            End If
            Dim ruta_archivo As String = ""
            Result = Refclasexp.Genera_rotulo_Eexpediente_pdf(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                              Session.Item("GA_IDEMPRESA"), _
                                                              nombre_plantilla_impresion, _
                                                              ruta_archivo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
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
                        Me.UpdatePanel_botones_unidad.Update()
                        updatapanel_iframe.Update()
                    End If
                End If
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_rotulo_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_rotulo_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "0" Or Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para imprimir el rotulo", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclasexp As New ClassGaExpediente
            Dim Result As String = ""
            '------------------------------------------------------
            'Retorna plantilla impresion usuario gestión
            '------------------------------------------------------
            Dim nombre_plantilla_impresion As String = ""
            Dim id_configuracion_plantilla_rotulo As Integer = 0
            Result = Refclasexp.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                 id_configuracion_plantilla_rotulo, _
                                                                                                 nombre_plantilla_impresion)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If nombre_plantilla_impresion = "" Then
                nombre_plantilla_impresion = "DEFAULT"
            End If
            Dim ruta_archivo As String = ""
            Result = Refclasexp.Genera_rotulo_Eexpediente_pdf(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                              Session.Item("GA_IDEMPRESA"), _
                                                              nombre_plantilla_impresion, _
                                                              ruta_archivo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu("se registro el expediente, pero no se genero el rotulo " & Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
                Me.ifimpre_post_.Attributes.Add("src", "../Gestion/WebFormimpresionfile.aspx")
                UpdatePaneliframe_post.Update()
                ModalPopupExtenderimpre_post.Show()
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_configura_rotulo_Click(sender As Object, e As EventArgs) Handles Button_configura_rotulo.Click
        Dim Refclas_empresa As New ClassGaExpediente
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim id_configuracion_rotulo As Integer = 0
            Dim nombre_configuracion As String = ""
            Result = Refclas_empresa.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                      id_configuracion_rotulo, _
                                                                                                      nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_empresa.Retorna_listado_configuracion_rotulo_expediente(nombre_configuracion, _
                                                                                     Me.DropDownList_configura_plantilla_rotulo, _
                                                                                     Me.UpdatePanel_configura_plantilla_rotulo)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_configura_plantilla_rotulo.Show()
        Catch ex As Exception
            scripjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
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
            Result = Refclas_empresa.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                      id_configuracion_rotulo, _
                                                                                                      nombre_configuracion)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_configuracion_rotulo_expediente As Integer = 0
            Result = Refclas_empresa.Retorna_id_nombre_configuracion_rotulo_expediente(Me.DropDownList_configura_plantilla_rotulo.Text, _
                                                                                       id_configuracion_rotulo_expediente)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_configuracion_rotulo = 0 Then
                Result = Refclas_empresa.Registra_configuracion_rotulo_expediente_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                  id_configuracion_rotulo_expediente)
                If Result <> "YES" Then
                    scripjava.Showscripman_menu(Result, Me.UpdatePanel_configura_plantilla_rotulo, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = Refclas_empresa.Actualiza_configuracion_rotulo_expediente_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                   id_configuracion_rotulo_expediente)
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
    Private Sub Button_estado_expediente_gestion_Click(sender As Object, e As EventArgs) Handles Button_estado_expediente_gestion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""

            If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "0" Or Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un expediente para cambiar el estado", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            
            '----------------------------------------
            'Retorna el estado del expediente
            '----------------------------------------
            Dim estado_publico As Integer = 0
            Dim estado_expediente As Integer = -1
            Result = Refclas.Retorna_estado_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                       estado_expediente, _
                                                       estado_publico)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Check_ButtonAbierto.Checked = False
            Me.CheckBox_ButtonSerrado.Checked = False
            If estado_expediente = 1 Then
                Me.Check_ButtonAbierto.Checked = True
            End If
            If estado_expediente <> 1 Then
                Me.CheckBox_ButtonSerrado.Checked = True
            End If
            Me.ModalPopupExtender_cambia_estado_expediente_popup.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_actualiza_estado_expediente_popup_Click(sender As Object, e As EventArgs) Handles Button_actualiza_estado_expediente_popup.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""
            'If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 1 Then
            '    Result = Refclas.Verifica_propiedad_usuario_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
            '                                                           HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '    If Result <> "YES" Then
            '        clasjava.Showscripman_menu(Result, Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
            '        Exit Sub
            '    End If
            'End If
            Dim split() As String = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                clasjava.Showscripman_menu("Por favor seleccione un expediente para cambiar de estado ", Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Refclas_permisos_nivel As New Class_ra_pro_permisos_niveles
            Dim estado_propietario As String = ""
            Dim stru_permisos_niveles As stru_permiso_nivel = Nothing
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                          split(1), _
                                                                          estado_propietario)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_propietario = "NO" Then
                Result = Refclas_permisos_nivel.Solicita_datos_estrctura_permiso_nivel_usuario_gestion(split(1), _
                                                                                                       Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                       stru_permisos_niveles)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If stru_permisos_niveles.editar_expediente = 0 Then
                    clasjava.Showscripman_menu("El usuario no tiene persmisos para editar o cambiar el estado del expediente, el nivel al que pertenece el expediente es propiedad de otro usuario", Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim estado_expediente As Integer = -1
            Dim captura_estado As String = ""
            If Me.Check_ButtonAbierto.Checked = True Then
                estado_expediente = 1
                captura_estado = "Abrir"
            End If
            If Me.CheckBox_ButtonSerrado.Checked = True Then
                estado_expediente = 0
                captura_estado = "Cerrar"
            End If
            Dim estado_expediente_db As Integer = -1
            Dim estado_publico As Integer = 0
            Result = Refclas.Retorna_estado_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                       estado_expediente_db, _
                                                       estado_publico)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_expediente_db <> estado_expediente Then
                If Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "0" Or Session.Item("PG_SELECCION_ID_EXPEIDENTE") = "-1" Then
                    clasjava.Showscripman_menu("Debe seleccionar un expediente para cambiar el estado", Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Me.TextBox_cambia_estado_exp_popup.Text = "" Then
                    clasjava.Showscripman_menu("Por favor digite el motivo del cambio de estado", Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.cambia_estado_abierto_serrado_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                          estado_expediente, _
                                                                          HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                          HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                                          HttpContext.Current.Session.Item("ip_host_name"), _
                                                                          Me.TextBox_cambia_estado_exp_popup.Text)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.updatepanel_botones_cambia_estado_expediente_popup, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.ModalPopupExtender_cambia_estado_expediente_popup.Hide()
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.updatepanel_botones_cambia_estado_expediente_popup)
        End Try
    End Sub

    Private Sub Button_general_indice_expediente_Click(sender As Object, e As EventArgs) Handles Button_general_indice_expediente.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim estado_expediente_db As Integer = -1
            Dim estado_publico As Integer = 0
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""
            Result = Refclas.Retorna_estado_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                       estado_expediente_db, _
                                                       estado_publico)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_expediente_db = 0 Then
                clasjava.Showscripman_menu("El expediente esta cerrado, imposible crear indice expediente", Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
            Result = Class_ra_cert_indice_expediente.Crear_indice_expediente(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                             1)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_opcion)
        End Try
    End Sub

    Private Sub Button_indice_expediente_Click(sender As Object, e As EventArgs) Handles Button_indice_expediente.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
            Dim Result As String = ""
            Dim Existencia_indice As String = ""
            Result = Class_ra_cert_indice_expediente.Solicita_existencia_indice_db(Session.Item("PG_SELECCION_ID_EXPEIDENTE"), _
                                                                                   Existencia_indice)
            If Existencia_indice = "NO" Then
                clasjava.Showscripman_menu("El expediente no tiene indice para mostrar", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Session.Item("CERT_ID_EXPEDIENTE_INDICE") = Session.Item("PG_SELECCION_ID_EXPEIDENTE")
            Me.Iframe_indice_.Attributes.Add("src", "../gestion/WebForm_indice_expediente.aspx")
            Me.UpdatePanel_indice.Update()
            Me.ModalPopupExtender_indice.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_activa_gestion_meta_dato_Click(sender As Object, e As EventArgs) Handles Button_activa_gestion_meta_dato.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            Dim Class_ra_m_sistema_meta_datos As New Class_ra_m_sistema_meta_datos
            If Me.hdnEmailID.Value = "-1" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el documento de la lista para gestionar el meta dato", Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Class_ra_m_registro_meta_dato_arhivo As New Class_ra_m_registro_meta_dato_archivo
            Dim id_sistema_meta_datos As Integer = 0
            Result = Class_ra_m_registro_meta_dato_arhivo.Solicita_existencia_meta_dato_archivo(Val(Me.hdnEmailID.Value), _
                                                                                                id_sistema_meta_datos)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Class_ra_m_sistema_meta_datos.Solicita_lista_sistema_meta_datos_archivo(id_sistema_meta_datos, _
                                                                                             Me.DropDownList_meta_datos, _
                                                                                             Me.UpdatePanel_gestion_meta_datos_up)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_gestion_meta_datos.Show()
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_unidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_gestion_meta_datos_Click(sender As Object, e As EventArgs) Handles Button_gestion_meta_datos.Click
      
    End Sub

    Private Sub Button_gestion_meta_dato_Click(sender As Object, e As EventArgs) Handles Button_gestion_meta_dato.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Me.DropDownList_meta_datos.Items.Count = 0 Then
                Refclasjava.Showscripman_menu("Por favor seleccionar el sistema de meta datos", Me.UpdatePanel_gestion_meta_datos, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If Me.DropDownList_meta_datos.SelectedItem Is Nothing Then
                    Refclasjava.Showscripman_menu("Por favor seleccionar el sistema de meta datos", Me.UpdatePanel_gestion_meta_datos, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim Result As String = ""
            Session.Item("ID_SISTEMA_META_DATOS") = Me.DropDownList_meta_datos.SelectedItem.Value
            Session.Item("ID_IMAGEN_PRODUCCION_SISTEMA_META_DATOS") = Val(Me.hdnEmailID.Value)
            Session.Item("NOMBRE_SISTEMA_META_DATOS") = Me.DropDownList_meta_datos.SelectedItem.Text
            Result = ClassGaProducionDocumental.Solicita_nombre_gabinete_archivo_produccion(Session.Item("ID_IMAGEN_PRODUCCION_SISTEMA_META_DATOS"), _
                                                                                            Session.Item("GABINETE_SISTEMA_META_DATOS"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_gestion_meta_datos, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Iframe_gestion_meta_data_archivo_.Attributes("SRC") = "../Gestion/WebForm_Gestion_Meta_Datos.aspx"
            Me.UpdatePanel_gestion_meta_data_archivo.Update()
            Me.ModalPopupExtende_gestion_meta_data_archivo.Show()
            Me.ModalPopupExtender_edition_gestion_meta_datos.Hide()
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_gestion_meta_datos, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_ocultar_nivel_Click(sender As Object, e As EventArgs) Handles Button_ocultar_nivel.Click
        Dim clasjava As New Classscrripjava
        Try
            If HiddenField_0003.Value = "" Then
                Exit Sub
            End If
            If InStr(HiddenField_0003.Value, "|") > 0 Then
                clasjava.Showscripman_menu("Por favor seleccione un nivel a ocultar ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim split_() As String = HiddenField_0003.Value.Split("\\")
            Me.HiddenField_rest_0004.Value = ""
            If HiddenField_botones_respuesta.Value = "0" Then
                Exit Sub
            End If
            Session.Item("PG_SELECCION_ID_NIVEL") = split_(0)
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Class_niveles_organizacion As New Class_niveles_organizacion
            Dim Result As String = ""
            Dim estado_propietario = "NO"
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                          Session.Item("PG_SELECCION_ID_NIVEL"), _
                                                                          estado_propietario)
            If estado_propietario = "NO" Then
                clasjava.Showscripman_menu("El usuario no es el propietario, no puede ocultar el nivel ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru_niveles_hijo_() As stru_niveles_hijo = Nothing
            Dim numero_expediente As Integer = 0
            Dim numero_niveles As Integer = 0
            Result = Class_niveles_organizacion.Cambia_estado_nivel_organizacion_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                 Val(Session.Item("PG_SELECCION_ID_NIVEL")), _
                                                                                                 0, _
                                                                                                 stru_niveles_hijo_, _
                                                                                                 numero_expediente, _
                                                                                                 numero_niveles)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.HiddenField_numero_expediente.Value = numero_expediente
            Me.HiddenField_numero_nivel.Value = numero_niveles
            Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = ""
            Me.HiddenField_rest_0004.Value = "YES"
            HiddenField_0003.Value = ""
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_activa_listar_niveles_ocultos_Click(sender As Object, e As EventArgs) Handles Button_activa_listar_niveles_ocultos.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Session.Item("SortExpression_publico") = "id_nivel"
            Session.Item("SortDirection_publico") = "DESC"
            Result = Reflcas_nivel_prop.Lista_niveles_ocultos(Session.Item("GA_IDUSUARIOGESTION"), _
                                                             1, _
                                                             "", _
                                                             HttpContext.Current.Session.Item("SortExpression_publico"), _
                                                             HttpContext.Current.Session.Item("SortDirection_publico"), _
                                                             Me.titulo_label_lista_niveles_ocultos, _
                                                             Me.GridView_lista_niveles_ocultos, _
                                                             Me.Hidden_lista_niveles_ocultos, _
                                                             Me.Update_lista_niveles_ocultos)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_lista_niveles_ocultos.Show()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridView_lista_niveles_ocultos_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_lista_niveles_ocultos.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            GridView_lista_niveles_ocultos.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Result = Reflcas_nivel_prop.Lista_niveles_ocultos(Session.Item("GA_IDUSUARIOGESTION"), _
                                                              HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO"), _
                                                              HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO"), _
                                                              HttpContext.Current.Session.Item("SortExpression_publico"), _
                                                              HttpContext.Current.Session.Item("SortDirection_publico"), _
                                                              Me.titulo_label_lista_niveles_ocultos, _
                                                              Me.GridView_lista_niveles_ocultos, _
                                                              Me.Hidden_lista_niveles_ocultos, _
                                                              Me.Update_lista_niveles_ocultos)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub GridView_lista_niveles_ocultos_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_lista_niveles_ocultos.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    
    Private Sub GridView_lista_niveles_ocultos_Sorting(sender As Object, e As GridViewSortEventArgs) Handles GridView_lista_niveles_ocultos.Sorting
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Session.Item("SortExpression_publico") = e.SortExpression
            If Session.Item("SortDirection_publico") = "DESC" Then
                Session.Item("SortDirection_publico") = "ASC"
            Else
                Session.Item("SortDirection_publico") = "DESC"
            End If
            Result = Reflcas_nivel_prop.Lista_niveles_ocultos(Session.Item("GA_IDUSUARIOGESTION"), _
                                                              HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO"), _
                                                              HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO"), _
                                                              HttpContext.Current.Session.Item("SortExpression_publico"), _
                                                              HttpContext.Current.Session.Item("SortDirection_publico"), _
                                                              Me.titulo_label_lista_niveles_ocultos, _
                                                              Me.GridView_lista_niveles_ocultos, _
                                                              Me.Hidden_lista_niveles_ocultos, _
                                                              Me.Update_lista_niveles_ocultos)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_muestra_nivel_oculto_Click(sender As Object, e As EventArgs) Handles Button_muestra_nivel_oculto.Click
        Dim clasjava As New Classscrripjava
        Try
            HiddenField_res_muestra_nivel.Value = ""
            Dim Reflcas_nivel_prop As New Class_ra_pro_niveles
            Dim Class_niveles_organizacion As New Class_niveles_organizacion
            Dim Result As String = ""
            Dim estado_propietario = "NO"
            Result = Reflcas_nivel_prop.Solicita_estado_nivel_propietario(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                          Val(Me.Hidden_lista_niveles_ocultos.Value), _
                                                                          estado_propietario)
            If estado_propietario = "NO" Then
                clasjava.Showscripman_menu("El usuario no es el propietario del nivel, no puede agregar el nivel a la estructura ", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim stru_niveles_hijo_() As stru_niveles_hijo = Nothing
            Dim numero_expediente As Integer = 0
            Dim numero_niveles As Integer = 0
            Result = Class_niveles_organizacion.Cambia_estado_nivel_organizacion_usuario_gestion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                 Val(Me.Hidden_lista_niveles_ocultos.Value), _
                                                                                                 1, _
                                                                                                 stru_niveles_hijo_, _
                                                                                                 numero_expediente, _
                                                                                                 numero_niveles)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                HiddenField_res_muestra_nivel.Value = ""
                Exit Sub
            Else
                clasjava.Showscripman_menu("Se agrego la estructura correctamente, actualice la pagina para reflejar los cambios", Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
                HiddenField_res_muestra_nivel.Value = "YES"
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_carpeta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    'Private Sub ButtonExportaListaExpediente_Click(sender As Object, e As EventArgs) Handles ButtonExportaListaExpediente.Click
    '    Dim Classscrripjava As New Classscrripjava
    '    Try
    '        Dim Result As String = ""
    '        Dim RutaArchivoReporte As String = ""
    '        Dim UrlArchivoReporte As String = ""
    '        Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
    '        Result = ClassGaProducionDocumental.ExportaListaContenidoExpediente(Me.data_grid,
    '                                                                            Me.Hidden_colum_header_reporte.Value,
    '                                                                            RutaArchivoReporte,
    '                                                                            UrlArchivoReporte)
    '        If Result <> "YES" Then
    '            Classscrripjava.Showscripman_menu(Result, Me.UpdatePanel_botones_opcion, "ModalPopupExtender_mensaje_personalizado")
    '            Exit Sub
    '        Else
    '            If File.Exists(RutaArchivoReporte) = True Then
    '                Hidden_ruta_archivo.Value = UrlArchivoReporte
    '                Me.ifmExcel_reporte_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
    '                UpdatePanel_botones_unidad.Update()
    '            End If
    '        End If
    '    Catch ex As Exception
    '        Classscrripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_opcion)
    '    End Try
    'End Sub
End Class