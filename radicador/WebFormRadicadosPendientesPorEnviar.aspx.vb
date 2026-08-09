Public Class WebFormRadicadosPendientesPorEnviar
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim result As String = ""
            Dim refclas As New ClassRadicador
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            If Me.IsPostBack = False Then
                Dim split() As String = Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
                Dim Refclas_producion As New ClassGaProducionDocumental
                Dim Result_planti As String = ""
                Dim nombre_plantilla_radicado As String = ""
                Dim Id_Plantilla As Integer = 0
                Result_planti = Class_system_plantilla_radicado.Solicita_nombre_id_plantilla_radicación_interna_default(nombre_plantilla_radicado,
                                                                                                          Id_Plantilla,
                                                                                                          1)
                If Result_planti <> "YES" Then
                    Label_estado_transac.Text = Result_planti
                    Exit Sub
                End If
                Session.Item("RA_MODULO_SELECCIONADO") = "PRODUCCION|" & Id_Plantilla.ToString & "|" & "RADICACION ENTRANTE" & "|" & "0" & "|" & nombre_plantilla_radicado
                result = refclas.Genera_Sql_Consulta_radicados_internos_pendientes_por_enviar(Me.Page, _
                                                                                              split(1), _
                                                                                              split(4))
                If result <> "YES" Then
                    Label_estado_transac.Text = result
                Else
                    Label_estado_transac.Text = ""
                End If

            End If
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
        Catch ex As Exception

        End Try
        
    End Sub

   
    Private Sub Button_enviar_Click(sender As Object, e As EventArgs) Handles Button_enviar.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro de la lista para enviar el radicado al destinatario", Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassGaProducionDocumental
            Dim Resultado_correo As String = ""
            Result = Refclas.Envia_documento_workflow_radicado(Me.hdnEmailID_VAL.Value,
                                                               Hidden_ruta_archivo,
                                                               updatapanel_iframe,
                                                               ifmExcel_,
                                                               Resultado_correo)
            If Result <> "YES" Then
                Me.Hidden00001.Value = ""
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If Resultado_correo <> "" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_botones_radicacion, "ModalPopupExtender_mensaje_personalizado")
                End If
                Me.Hidden00001.Value = "YES"
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelabel_val_radicacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class