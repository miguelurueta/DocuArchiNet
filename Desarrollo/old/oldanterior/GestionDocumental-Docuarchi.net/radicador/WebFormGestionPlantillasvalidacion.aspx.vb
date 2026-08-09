Imports AjaxControlToolkit
Public Class WebFormGestionPlantillasvalidacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim result As String = ""
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            If Page.IsPostBack = False Then
                Dim script As String = "<script type=text/javascript>ejecuta_ecript_consulta();</script>"
                ScriptManager.RegisterStartupScript(Me, GetType(Page), "ejecuta_ecript_consulta", script, False)
                Dim Refclas As New ClassRadicador

                Dim inter As String = Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION")
                Dim nombre_plantilla As String = ""
                Dim Class_plantilla_validacion As New Class_plantilla_validacion
                result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(inter,
                                                                                        nombre_plantilla)
                If result <> "YES" Then
                    Me.Label_resultado.Text = Me.Label_resultado.Text & result
                Else
                    result = Refclas.Verifica_Permisos_usuario_plantilla_validacion(inter,
                                                                                    nombre_plantilla)
                    If result <> "YES" Then
                        Me.Label_resultado.Text = Me.Label_resultado.Text & result
                    End If
                End If
                Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
                Dim sqlconsulta As String = ""
                Dim ClassRaConsultaRadicados As New Class_plantilla_validacion
                result = ClassRaConsultaRadicados.Consulta_ini_plantilla_validacion(id_escript,
                                                                                    Page,
                                                                                    Me.Hidden_asig_.Value)
                If result <> "YES" Then
                    Me.Label_resultado.Text = Me.Label_resultado.Text & result
                End If
            End If
        Catch ex As Exception
            Me.Label_resultado.Text = result
        End Try
    End Sub
    Private Sub UpdatePanelContenido_Load(sender As Object, e As EventArgs) Handles UpdatePanelContenido.Load
        Try
            Dim result As String = ""
            Dim refclas As New ClassRadicador
            Dim refclas_consulta As New ClassRaConsultaRadicados
            Dim rescrip As New Classscrripjava
            Dim inter As String = Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION")
            result = refclas_consulta.Genera_Interface_Gestion_Plantilla_Validacion(Me, _
                                                                                    Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            If result <> "YES" Then
                rescrip.Showscripman(result, UpdatePanelContenido)
            Else
                Dim estado_ubicacion As Integer = 0
                result = refclas.Retorna_Estado_Ubicacion_Plantilla_Validacion(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"), estado_ubicacion)
                If result <> "YES" Then
                    rescrip.Showscripman(result, UpdatePanelContenido)
                    Exit Sub
                End If
                If estado_ubicacion = 1 Then
                    Dim controles As String = ""
                    Dim droplista As DropDownList = sender.page.findcontrol("PAIS")
                    If droplista Is Nothing Then
                        rescrip.Showscripman("Imposible encontrar el control PAIS", UpdatePanelContenido)
                        Exit Sub
                    End If
                    'droplista.Items.Clear()
                    Dim ilist As New ListItem
                    ilist.Text = "SELECCIONE"
                    ilist.Value = "SELECCIONE"
                    droplista.Items.Add(ilist)
                    ilist = New ListItem
                    ilist.Text = "COLOMBIA"
                    ilist.Value = "COLOMBIA"
                    droplista.Items.Add(ilist)
                    If Me.Hiddenselecionpais.Value <> "SELECCIONE" And Me.Hiddenselecionpais.Value <> "" Then
                        'Me.Hiddenselecionpais.Value = "COLOMBIA"
                        'droplista.SelectedIndex = 1

                    End If
                    '------------------------------------------------
                    'Lista los departamentos en el evento postback
                    'y conserva el elemento seleccionado
                    '-----------------------------------------------
                    Dim droplistadepartamento As DropDownList = sender.page.findcontrol("DEPARTEMENTO")
                    If droplistadepartamento Is Nothing Then
                        rescrip.Showscripman("Imposible encontrar el control DEPARTEMENTO", UpdatePanelContenido)
                        Exit Sub

                    End If
                    If Me.Hiddenselecionpais.Value <> "SELECCIONE" And Me.Hiddenselecionpais.Value <> "" Then
                        Dim Refclasra As New ClassRaConsultaRadicados
                        Dim Resulta As String = ""
                        Resulta = Refclasra.Lista_Departamentos_Paises(droplistadepartamento, _
                                                                       Me.Hiddenselecionpais.Value, _
                                                                       Me.UpdatePanelContenido)
                        If Resulta = "YES" Then
                            droplistadepartamento.Text = Me.Hiddenseleciondepartamento.Value
                            For i As Integer = 0 To droplistadepartamento.Items.Count - 1
                                If droplistadepartamento.Items(i).Text = Me.Hiddenseleciondepartamento.Value Then
                                    droplistadepartamento.SelectedIndex = i
                                    Exit For
                                End If
                            Next

                        End If

                    End If
                    '------------------------------------------------
                    'Lista los municipio en el evento postback
                    'y conserva el elemento seleccionado
                    '-----------------------------------------------
                    Dim droplistmunicipio As DropDownList = sender.page.findcontrol("MUNICIPIO")
                    If droplistmunicipio Is Nothing Then
                        rescrip.Showscripman("Imposible encontrar el control MUNICIPIO", UpdatePanelContenido)
                        Exit Sub
                    End If
                    If Me.Hiddenseleciondepartamento.Value <> "SELECCIONE" And Me.Hiddenseleciondepartamento.Value <> "" Then
                        'Dim Refclasra As New ClassRaConsultaRadicados
                        'Dim Resulta As String = ""
                        result = refclas_consulta.lista_Municipios_Departamentos_carga_inicio(droplistmunicipio, _
                                                                                              Me.Hiddenseleciondepartamento.Value, _
                                                                                              Me.UpdatePanelContenido)
                        If result = "YES" Then
                            droplistmunicipio.Text = Me.Hiddenmunicipio.Value
                            For i As Integer = 0 To droplistmunicipio.Items.Count - 1
                                If droplistmunicipio.Items(i).Text = Me.Hiddenmunicipio.Value Then
                                    droplistadepartamento.SelectedIndex = i
                                    Exit For
                                End If
                            Next

                        End If

                    End If

                End If
            End If
        Catch ex As Exception
            Me.Label_resultado.Text = ex.Message
        End Try
    End Sub

    Private Sub Buttonllenardepartamento_Click(sender As Object, e As EventArgs) Handles Buttonllenardepartamento.Click
       
    End Sub

    Private Sub Buttonllenarciudad_Click(sender As Object, e As EventArgs) Handles Buttonllenarciudad.Click
      
    End Sub

    Private Sub UpdatePanelContenido_PreRender(sender As Object, e As EventArgs) Handles UpdatePanelContenido.PreRender

    End Sub

    Private Sub WebFormGestionPlantillasvalidacion_PreRenderComplete(sender As Object, e As EventArgs) Handles Me.PreRenderComplete

    End Sub
    Private Sub Eliminar_Click(sender As Object, e As EventArgs) Handles Eliminar.Click
       
        Dim scriptjava As New Classscrripjava
        Dim Refclas As New ClassRadicador
        Dim Result As String = ""
        Try
            
            If Me.HiddenPROMP.Value = "1" Then Exit Sub
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scriptjava.Showscripman("Debe seleccionar el registro a eliminar ", Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("RA_VALIDACION_ELIMINAR") = "0" Then
                scriptjava.Showscripman("El usuario no tiene permisos para eliminar", Me.UpdatePanel_botones_radicacion)
                sender.focus()
                Exit Sub
            End If
            Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            Result = Refclas.Eliminar_Registro_Plantilla_Validacion_edicion(Me.Page, id_escript)
            If Result <> "YES" Then
                scriptjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
            End If
        Catch ex As Exception
            scriptjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub
    Private Sub Button_consulta_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_consulta_val_radicacion.Click
        Dim ClassRaConsultaRadicados As New Class_plantilla_validacion
        Dim result As String = ""
        Dim refclascrip As New Classscrripjava
        Dim sqlconsulta As String = ""
        Try
            If Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = 0 Or Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = -1 Then
                refclascrip.Showscripman_menu("El sistema do registra escript de validacion", UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            result = ClassRaConsultaRadicados.Genera_Sql_Consulta_Validacion(id_escript,
                                                                             Me.Page,
                                                                             sqlconsulta,
                                                                             Me.Hidden_asig_.Value)
            If result <> "YES" Then
                refclascrip.Showscripman_menu(result, UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            refclascrip.Showscripman(ex.Message, UpdatePanel_botones_validacion)
        End Try
    End Sub


    Private Sub Button_lipiar_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_lipiar_val_radicacion.Click
        Dim refclas As New ClassRaConsultaRadicados
        Dim result As String = ""
        Dim refclascrip As New Classscrripjava
        Dim sqlconsulta As String = ""
        Try
            If Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = 0 Or Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = -1 Then
                refclascrip.Showscripman("El sistema do registra escript de validacion", UpdatePanel_botones_validacion)
                Exit Sub
            End If
            Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            result = refclas.Limpiar_Campos_Interface_Plantilla_Validacion(Me.Page)
            If result <> "YES" Then
                refclascrip.Showscripman(result, UpdatePanel_botones_validacion)
                Exit Sub
            End If
        Catch ex As Exception
            refclascrip.Showscripman(ex.Message, UpdatePanel_botones_validacion)
        End Try
    End Sub

    Private Sub UpdatePanel_edita_campos_dinamicos_Load(sender As Object, e As EventArgs) Handles UpdatePanel_edita_campos_dinamicos.Load
        Try
            Dim refclas As New ClassRadicador
            Dim result As String = ""
            Dim rescrip As New Classscrripjava
            result = refclas.Inicializa_interface_gestion_plantilla_validacion(Me.Page)
            If result <> "YES" Then
                rescrip.Showscripman(result, Me.UpdatePanel_edita_campos_dinamicos)
                Exit Sub
            Else

            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Editar_pre_Click(sender As Object, e As EventArgs) Handles Editar_pre.Click
        Dim refclas As New ClassRadicador
        Dim result As String = ""
        Dim refclascrip As New Classscrripjava
        Dim sqlconsulta As String = ""
        Try
            Me.Button_registrar.Visible = False
            Me.Button_edita_campos_dinamicos.Visible = True
            UpdatePanel_edita_campos_dinamicos_actualiza.Update()
            If Me.hdnEmailID_VAL.Value = "-1" Then
                refclascrip.Showscripman_menu("Debe seleccionar el registro a editar ", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = 0 Or Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = -1 Then
                refclascrip.Showscripman_menu("El sistema no registra escript de validacion", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.hdnEmailID_VAL.Value = "-1" Then
                refclascrip.Showscripman_menu("Debe seleccionar el registro a editar ", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            UpdatePanel_edita_campos_dinamicos.Update()
            Me.ModalPopupExtender_edita_campos_dinamicos.Show()
            If Me.hdnEmailID_VAL.Value <> "-1" Then
                Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
                result = refclas.Asignar_Datos_Plantilla_Para_Edicion_validacion(Me.Page, _
                                                                                 id_escript)
                If result <> "YES" Then
                    refclascrip.Showscripman_menu(result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            refclascrip.Showscripman(ex.Message, UpdatePanel_botones_radicacion)
        End Try
    End Sub

    Private Sub Button_edita_campos_dinamicos_Click(sender As Object, e As EventArgs) Handles Button_edita_campos_dinamicos.Click
        Dim clasjava As New Classscrripjava
        Dim updat As UpdatePanel = sender.page.FindControl("UpdatePanel_edita_campos_dinamicos_actualiza")
        Dim ref_clas As New ClassRadicador
        Try
            'Me.Button_registrar.Visible = False
            Dim Result As String = ""
            Dim sqlconsulta As String = ""
            Dim hiden_edit_Hiddenestadoedicion As Object = sender.page.FindControl("Hiddenestadoedicion_EDIT")
            If hiden_edit_Hiddenestadoedicion Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control Hiddenestadoedicion", updat, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim hide As Object = sender.page.FindControl("hdnEmailID_VAL")
            If hide Is Nothing Then
                clasjava.Showscripman_menu("Imposible encontrar el control id", updat, "ModalPopupExtender_mensaje_personalizado")
                sender.focus()
                Exit Sub
            End If

            Dim value_hide = hide.value
            If HttpContext.Current.Session.Item("RA_VALIDACION_EDITAR") = "0" Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para editar ", updat, "ModalPopupExtender_mensaje_personalizado")
                sender.focus()
                Exit Sub
            End If
            Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            Result = ref_clas.Editar_Indice_Plantilla_Validacion_eidcion_plantilla_validacion(sender.page, _
                                                                                              id_escript)
            If Result <> "YES" Then
                hide.value = value_hide
                clasjava.Showscripman_menu(Result, updat, "ModalPopupExtender_mensaje_personalizado")
                sender.focus()

            Else
                hide.value = value_hide
                ModalPopupExtender_edita_campos_dinamicos.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, updat)
        End Try
    End Sub

    Private Sub Button_registrar_Click(sender As Object, e As EventArgs) Handles Button_registrar.Click
        Dim clasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New ClassRadicador
        Try
            If HttpContext.Current.Session.Item("RA_VALIDACION_AGREGAR") = "0" Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para agregar", UpdatePanel_edita_campos_dinamicos_actualiza, "ModalPopupExtender_mensaje_personalizado")
                sender.focus()
                Exit Sub
            End If
            Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            Result = Refclas.Agregar_Nuevo_Registro_plantilla_validacion_edicion(sender.page, _
                                                                                 id_escript, _
                                                                                 Me.Hidden_asig_.Value)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_edita_campos_dinamicos_actualiza, "ModalPopupExtender_mensaje_personalizado")
                sender.focus()
            Else
                Hidden_resultado_registrar.Value = "-1"
                ModalPopupExtender_edita_campos_dinamicos.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_edita_campos_dinamicos_actualiza)
        End Try
    End Sub

    Private Sub Button_pre_agregar_Click(sender As Object, e As EventArgs) Handles Button_pre_agregar.Click
        Dim refclas As New ClassRadicador
        Dim result As String = ""
        Dim refclascrip As New Classscrripjava
        Dim sqlconsulta As String = ""
        Try
            Me.Button_registrar.Visible = True
            Me.Button_edita_campos_dinamicos.Visible = False
            UpdatePanel_edita_campos_dinamicos_actualiza.Update()
            If Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = 0 Or Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = -1 Then
                refclascrip.Showscripman("El sistema no registra escript de validación", UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            UpdatePanel_edita_campos_dinamicos.Update()
            ModalPopupExtender_edita_campos_dinamicos.Show()
        Catch ex As Exception
            refclascrip.Showscripman(ex.Message, UpdatePanel_botones_radicacion)
        End Try
    End Sub

    Private Sub Button_ejecutar_consulta_Click(sender As Object, e As EventArgs) Handles Button_ejecutar_consulta.Click
        Try
            If HttpContext.Current.Session("RA_ID_DEST_EXTERNO") <> "-1" Then
                Dim Refclas As New ClassRaConsultaRadicados
                Dim inter As String = Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION")
                Dim dest_int As String = HttpContext.Current.Session("RA_ID_DEST_EXTERNO")
                Dim result = Refclas.Genera_Sql_Consulta_Validacion_gestion(inter, Me.Page, "", HttpContext.Current.Session("RA_ID_DEST_EXTERNO"))
                If result <> "YES" Then
                    HttpContext.Current.Session("RA_ID_DEST_EXTERNO") = "-1"
                    Me.Label_resultado.Text = result
                End If
                HttpContext.Current.Session("RA_ID_DEST_EXTERNO") = "-1"
            End If
        Catch ex As Exception
            Me.Label_resultado.Text = ex.Message
        End Try
    End Sub

    Private Sub Button_limpiar_campos_edicion_Click(sender As Object, e As EventArgs) Handles Button_limpiar_campos_edicion.Click
        Dim refclas As New ClassRadicador
        Dim result As String = ""
        Dim refclascrip As New Classscrripjava
        Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
        'UpdatePanel_edita_campos_dinamicos.Update()
            result = refclas.Limpiar_Campos_Interface_Plantilla_Validacion_edicion(Me.Page, id_escript)
            If result <> "YES" Then
                refclascrip.Showscripman(result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If

        
    End Sub

    Protected Sub Button_Asigna_Click(sender As Object, e As EventArgs) Handles Button_Asigna.Click
      
    End Sub

    Private Sub GridView_val_radicacion_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_val_radicacion.RowCreated
        Try
            'e.Row.Cells(2).Visible = False
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub
End Class