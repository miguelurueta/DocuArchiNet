Public Class WebFormVisor_Otros_Formatos
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            Me.ifrm_visor_.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & Session.Item("WF_RUTA_DOCUMENTO_SELECCIONADO")
            'Dim file_inf As New IO.FileInfo(Session.Item("WF_RUTA_DOCUMENTO_SELECCIONADO"))
            'If UCase(file_inf.Extension) = ".PDF" Then
            '    Me.ifrm_visor.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & Session.Item("WF_RUTA_DOCUMENTO_SELECCIONADO")
            'Else
            '    Session.Item("DA_DESCARGA_EXTERNO") = Session.Item("WF_RUTA_DOCUMENTO_SELECCIONADO")
            '    Me.ifrm_visor.Attributes("SRC") = "../Docuarchi/WebFormDaVisorExternoDescarga.aspx"
            'End If
        End If
    End Sub

End Class