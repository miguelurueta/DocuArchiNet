Public Class WebFormRaNotasSolicitudesAprobacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().cligred();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)

        End If
        If Page.IsPostBack = False Then
            Dim Refclas As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            If Session.Item("GA_INTERCAMBIO_TIPO_NOTA_APROBACION") = "GENERAL" Then
                Result = Refclas.Lista_notas_solicitudes_generales_de_aprobacion(Session.Item("GA_INTERCAMBIO_NOTA_APROBACION"), _
                                                                                 Me.GridViewlista, _
                                                                                 Me.hdnEmailID, _
                                                                                 Me.UpdatePanelanotacion)
                If Result <> "YES" Then
                    Me.Label_Estado.Text = Result
                End If
            End If
            If Session.Item("GA_INTERCAMBIO_TIPO_NOTA_APROBACION") = "ESPECIFICA" Then
                Result = Refclas.Lista_notas_solicitudes_especificas_de_aprobacion(Session.Item("GA_INTERCAMBIO_NOTA_APROBACION"), _
                                                                                   Me.GridViewlista, _
                                                                                   Me.hdnEmailID, _
                                                                                   Me.UpdatePanelanotacion)
                If Result <> "YES" Then
                    Me.Label_Estado.Text = Result
                End If
            End If
        End If
    End Sub
    Private Sub GridViewlista_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowCreated
        e.Row.Cells(1).Visible = False

    End Sub
    
    Private Sub Buttonclidatos_Click(sender As Object, e As EventArgs) Handles Buttonclidatos.Click
        Dim Result As String = ""
        Dim Refclas As New ClassRaSolicitudesAprobacion
        Dim refmensaje As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "0" Then
                Exit Sub
            End If
            Result = Refclas.Retorna_nota_solicitud_de_aprobacion(Me.hdnEmailID.Value, _
                                                                  Me.TextBoxdatos.Text)
            If Result <> "YES" Then
                refmensaje.Showscripman_menu(Result, Me.Updateboton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ButtonGuardar.Visible = False
            Me.ButtonActualizar.Visible = True
            UpdatePanel_guardar_nota.Update()
            Me.UpdatePaneltextbos.Update()
            ModalPopupExtender_edition_nota_respuesta.Show()
        Catch ex As Exception
            refmensaje.Showscripman_menu(ex.Message, Me.Updateboton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class