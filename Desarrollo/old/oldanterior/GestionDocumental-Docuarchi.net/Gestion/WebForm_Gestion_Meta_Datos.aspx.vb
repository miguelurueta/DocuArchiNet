Public Class WebForm_Gestion_Meta_Datos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Class_ra_m_detalle_sis_meta_datos As New Class_ra_m_detalle_sis_meta_datos
        Dim Class_ra_m_interface_meta_datos As New Class_ra_m_interface_meta_datos
        Dim Result As String = ""
        Result = Class_ra_m_interface_meta_datos.Crea_interface_meta_datos(Session.Item("ID_SISTEMA_META_DATOS"), _
                                                                           Session.Item("NOMBRE_SISTEMA_META_DATOS"), _
                                                                           Session.Item("ID_IMAGEN_PRODUCCION_SISTEMA_META_DATOS"), _
                                                                           Session.Item("GABINETE_SISTEMA_META_DATOS"), _
                                                                           Me.Page, _
                                                                           Me.Panel_control_meta_data, _
                                                                           Me.Update_control_meta_data)

        If Result <> "YES" Then
            Label_estado.Text = Result
        End If

    End Sub

End Class