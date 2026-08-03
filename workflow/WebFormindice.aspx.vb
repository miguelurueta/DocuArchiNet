Imports AjaxControlToolkit
Public Class WebFormindice
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If

        Dim refclasindice As New ClassWorkflowIndiceDA
        Dim nombrecampo As String = ""
        If Session.Item("WF_ID_GABINETE_SELECCIONADO") <> "" And Session.Item("WF_GABINETE_SELECCIONADO") <> "" Then
            Dim result As String = refclasindice.Genera_interface_indice_documento(Session.Item("WF_ID_GABINETE_SELECCIONADO"), _
                                                                           Session.Item("WF_GABINETE_SELECCIONADO"), _
                                                                           Me.Page, _
                                                                           nombrecampo, _
                                                                           Me.Panel1, _
                                                                           ActualizaindiceImage, _
                                                                           1, _
                                                                           0)
            If result <> "YES" Then
                'mens.showscripman("imposible obtener indice de documentos " & result, Me.updatepanelseleccion)
                Exit Sub
            Else

            End If
            

        Else
            'Me.Panel1.Controls.Clear()
        End If
        
    End Sub


    Private Sub Button_listar_tipos_Click(sender As Object, e As EventArgs) Handles Button_listar_tipos.Click
        Dim Mens As New Classscrripjava
        Try
            Dim drow_list As DropDownList = sender.page.findcontrol("ComboBoxtipo")
            Dim update_drow As UpdatePanel = sender.page.findcontrol("update_panel_drowlist")
            Dim ref_ModalPopupExtende_trd_popup = sender.page.findcontrol("ModalPopupExtende_tipo_popup")
            Me.Label_trd_popup.Text = "Clasificar tipo documento"
            ref_ModalPopupExtende_trd_popup.Show()
            Dim refclas As New ClassGaTipoDocumental
            Dim Result As String = ""
            Dim matri() As String = {"DIGITALIZADO", "ELECTRONICO"}
            Result = refclas.Solicita_tipos_documentales_combo_excluyentes(drow_list, matri, Me.Hidden_valor_seleccion.Value, update_drow)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Updatepanel_botones)
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Updatepanel_botones)
            Exit Sub
        End Try
    End Sub

    Private Sub Button_lista_ayuda_tipo_Click(sender As Object, e As EventArgs) Handles Button_lista_ayuda_tipo.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaTipoDocumental
            Result = Refclas.Solicita_ayuda_tipo_documento(Me.Hidden_valor_seleccion.Value, Me.TextBoxinfotipo.Text)
            If Result <> "YES" Then
                Mens.Showscripman(Result, UpdatePanelmensaje)
                Exit Sub
            End If
            If Hidden_valor_seleccion.Value = "" Then Exit Sub
            Result = Refclas.SolicitaIdTipoFormatoDocumento(Hidden_valor_seleccion.Value,
                                                            Hidden_id_tipo.Value)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanelmensaje)
                Exit Sub
            End If
            Me.Updatepanel_botones.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, UpdatePanelmensaje)
            Exit Sub
        End Try
    End Sub

    
    Private Sub Button_actualiza_hiden_Expediente_Click(sender As Object, e As EventArgs) Handles Button_actualiza_hiden_Expediente.Click
        Me.Hidden_id_expediente.Value = "0"
        Me.Hidden_id_tipo_expediente.Value = "0"
        Updatepanel_actualiza.Update()
        
    End Sub

    Private Sub Button_actualiza_indice_imagen_Click(sender As Object, e As EventArgs) Handles Button_actualiza_indice_imagen.Click
        Dim Matri_Sender() As String
        Dim Result As String = ""
        Erase Matri_Sender
        Dim Mens As New Classscrripjava
        Matri_Sender = Split(Hidden_image_gabinete.Value, "|")
        Dim Refclasindice As New ClassWorkflowIndiceDA
        Try
            Result = Refclasindice.Actualiza_Indice_Imagen(Matri_Sender(0),
                                                           Matri_Sender(1),
                                                           "",
                                                           "",
                                                           HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                           HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                           Me.Page,
                                                           Session.Item("WF_INTER_SELECION_DOCUMENTO"))
            If Result <> "YES" Then
                Mens.Showscripman("Imposible actualizar indice " & Result, Updatepanel_actualiza)
            End If
        Catch ex As Exception
            Mens.Showscripman("Error general actualizando indice " & ex.Message, Updatepanel_actualiza)
        End Try
    End Sub
End Class