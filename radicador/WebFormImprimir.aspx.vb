Imports Neodynamic.SDK.Web.ClientPrinter
Public Class WebFormImprimir
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Me.Session.Item("RA_RUTA_IMPRESION_FINAL") <> "OJO" Then
                Dim SES As String = Me.Session.Item("RA_RUTA_IMPRESION_FINAL")
                Me.sid2.Value = SES
                Me.Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO")
            End If
            '***************************
            '0-codigo destinatario
            '1-codigo remitente
            '2-id usuario
            '3-codigo plantilla
            '4-Consecutivo Radicado
            '5-Consecutivo codigo barra
            '***************************
            Dim splitdatos() As String
            Erase splitdatos
            splitdatos = HttpContext.Current.Session("RA_DATO_IMPRESION").ToString.Split("¬")
            If Not splitdatos Is Nothing Then
                Me.usuario_datos.InnerText = splitdatos(2)
                Me.radicad_datos.InnerText = splitdatos(4)
                Me.barr_datos.InnerText = splitdatos(5)
                Me.fech_datos.InnerText = splitdatos(6)
            End If
            
        Catch ex As Exception

        End Try
    End Sub

End Class