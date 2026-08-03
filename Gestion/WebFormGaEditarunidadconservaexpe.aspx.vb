Public Class WebFormGaEditarunidadconservaexpe
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

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
            classcrip.Showscripman_menu(ex.Message, update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
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
            classcrip.Showscripman_menu(ex.Message, update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
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
            classcrip.Showscripman_menu(ex.Message, update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
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

    Private Sub WebFormGaEditarunidadconservaexpe_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
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

                Dim refclas_rad As New ClassRadicador
                Dim id_empresa As Integer = 0
                Dim Result As String = ""
                Dim refclascexpediente As New ClassUnidadConservacion
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxCodigoManual.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "CODIGO_UNICO")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_FINAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_FINAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_INICIAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_INICIAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxTEMA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "TEMA_UNIDAD_CONSERVACION")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxASUNTO_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "DESCRIPCION_UNIDAD_CONSERVACION")
                Me.hdnEmailID.Value = Session.Item("GA_ID_UNIDAD_CONTENEDORA")
                Me.Hiddenname_empresagestion.Value = Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA")
                Result = refclascexpediente.Activar_editar_unidad_conservacion(update_panel_controles, Me.hdnEmailID, Me.Hiddenname_empresagestion)
                If Result <> "YES" Then
                    Label_estado.Text = Result
                End If
                Dim refclas As New ClassAdmonEmpresa
                refclas.Retorna_Id_Emprea(Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA"), Me.Hidden_id_empresa.Value)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & "|" & Result
                End If
                Session.Item("GA_ID_UNIDAD_CONTENEDORA") = 0
                Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = ""
            Else
                Dim Result As String = ""
                Dim refclas_rad As New ClassRadicador
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxCodigoManual.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "CODIGO_UNICO")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_FINAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_FINAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxRANGO_EXTREMO_INICIAL.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "RANGO_EXTREMO_INICIAL")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxTEMA_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "TEMA_UNIDAD_CONSERVACION")
                Result = refclas_rad.agregar_auto_complete(Me.TextBoxASUNTO_EXPEDIENTE.ClientID, Me.panel_controles, "GetGuiaRadicaconasp", "unidad_conservacion", "DESCRIPCION_UNIDAD_CONSERVACION")
            End If
        Catch ex As Exception
            Label_estado.Text = Label_estado.Text & "|" & ex.Message
        End Try
    End Sub

    Protected Sub ButtonAceptar_Click(sender As Object, e As EventArgs) Handles ButtonAceptar.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim estado_codigo_unico As Integer = 0
            If Me.CheckBoxActivaCodigomanual.Checked = True Then
                estado_codigo_unico = 1
            Else
                estado_codigo_unico = 0
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassUnidadConservacion
            Dim id_organigrama As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim id_instrumento As Integer = 0
            Dim id_tipo_unidad As Integer = 0
            Dim nombre_organigrama As String = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim nombre_area As String = ""
            Dim nombre_instrumento As String = ""
            Dim nombre_tipo_unidad As String = ""
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
            If Not Me.DropDownList_tipo_unidad_contenedora.SelectedItem Is Nothing Then
                id_tipo_unidad = Me.DropDownList_tipo_unidad_contenedora.SelectedItem.Value
                nombre_tipo_unidad = Me.DropDownList_tipo_unidad_contenedora.SelectedItem.Text
            End If
            Result = Refclas.Actualiza_Unidad_Conservacion(Session.Item("GA_IDUSUARIOGESTION"), _
                                                           Me.hdnEmailID.Value, _
                                                           Me.TextBoxCodigoManual.Text, _
                                                           estado_codigo_unico, _
                                                           Me.TextBoxFECHA_EXTREMA_INICIAL.Text, _
                                                           Me.TextBoxFECHA_EXTREMA_FINAL.Text, _
                                                           Me.TextBoxRANGO_EXTREMO_INICIAL.Text, _
                                                           Me.TextBoxRANGO_EXTREMO_FINAL.Text, _
                                                           Me.TextBoxTEMA_EXPEDIENTE.Text, _
                                                           Me.TextBoxASUNTO_EXPEDIENTE.Text, _
                                                           Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA"), _
                                                           Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                           Session.Item("ip_host_name"), _
                                                           Me.Hidden_id_empresa.Value, _
                                                           1, nombre_organigrama, _
                                                           nombre_area, _
                                                           nombre_serie, _
                                                           nombre_sub_serie, _
                                                           nombre_tipo_unidad, _
                                                           1, _
                                                           Me.DropDownListsub_seccion.Text, _
                                                           id_serie, _
                                                           id_sub_serie, _
                                                           id_area, _
                                                           id_organigrama, _
                                                           id_instrumento, _
                                                           id_tipo_unidad)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Me.Hidden_resultado.Value = ""
            Else
                Me.Hidden_resultado.Value = "YES"
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
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
            Result = Refclas.Seleccion_instrumento_consrvacion(id_area, id_organigrama, id_instrumento, _
                                                               Me.DropDownListSerie, Me.DropDownListSubserie, _
                                                               Me.update_panel_controles)
            If Result <> "YES" Then
                scripjava.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.update_panel_controles)
        End Try
    End Sub

    Protected Sub ButtonRestaurar_Click(sender As Object, e As EventArgs) Handles ButtonRestaurar.Click
        Dim classcrip As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclasunidad As New ClassUnidadConservacion
            Result = Refclasunidad.Limpia_campos_unidad_conservacion(Me.table_controles)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclasunidad.Activar_editar_unidad_conservacion(update_panel_controles, Me.hdnEmailID, Me.Hiddenname_empresagestion)
            If Result <> "YES" Then
                classcrip.Showscripman_menu(Result, Me.update_panel_controles, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            classcrip.Showscripman_menu(ex.Message, Updatepanel_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class