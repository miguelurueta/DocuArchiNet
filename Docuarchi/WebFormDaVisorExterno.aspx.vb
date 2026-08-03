Public Class WebFormDaVisorExterno
    Inherits System.Web.UI.Page
    Public Matri_Doc_Visual() As String
    Public Doc_actual As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            If Page.IsPostBack = False Then
                Dim Refclas As New ClassWorflowVisor
                Dim Result As String = ""
                HttpContext.Current.Session.Item("DA_IMAGE_TEMPORAL_EMERGENTE") = ""
                Session.Item("DA_MATRI_IMAGE_EMERGENTE") = ""
                Result = Refclas.Visualizacion_Documentos_da(Matri_Doc_Visual,
                                                             Session.Item("DA_IMAGEN"),
                                                             Session.Item("DA_GABINETE_CONSULTA"))
                If Result = "YES" Then
                    If Not Matri_Doc_Visual Is Nothing Then
                        Dim file_inf As New IO.FileInfo(Matri_Doc_Visual(1))
                        If UCase(file_inf.Extension) = ".PDF" Then
                            Me.ifrm_visor_.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & Matri_Doc_Visual(1)
                        Else
                            Session.Item("DA_DESCARGA_EXTERNO") = Matri_Doc_Visual(1)
                            Me.ifrm_visor_.Attributes("SRC") = "../Docuarchi/WebFormDaVisorExternoDescarga.aspx"
                        End If
                    End If
                End If
                Me.Hidden_gabinete_.Value = Session.Item("DA_GABINETE_CONSULTA")
                Me.Hidden_imagen_.Value = Session.Item("DA_IMAGEN")
                Dim refgabinete As New ClassDaGabinete
                Dim datos_log As String = ""
                Result = refgabinete.Retorna_Datos_Auditoria_Gabinete(Session.Item("DA_IMAGEN"), _
                                                                      Session.Item("DA_GABINETE_CONSULTA"), _
                                                                      datos_log)
                If Result <> "YES" Then
                    'scri.Showscripman("Imposible encontrar datos log " & Result, Me.UpdatePanelButon)
                    'Exit Sub
                End If
                Dim selecion As String = ""
                Result = refgabinete.Registra_Auditoria_Eventos(HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"), _
                                                                selecion & " Imagen Principal " & Matri_Doc_Visual(0), _
                                                                HttpContext.Current.Session.Item("DA_IMAGEN"), _
                                                                datos_log, _
                                                                "Visualiza")
                If Result <> "YES" Then
                    'scri.Showscripman("Imposible registrar datos log " & Result, Me.UpdatePanelButon)
                    'Exit Sub
                End If
            Else
                Dim Matri_Temp() As String
                Erase Matri_Temp
                Matri_Temp = Split(Session.Item("DA_MATRI_IMAGE_EMERGENTE"), "|")
                If Not Matri_Temp Is Nothing Then
                    For i As Integer = 0 To Matri_Temp.Length - 2
                        ReDim Preserve Matri_Doc_Visual(i)
                        Matri_Doc_Visual(i) = Matri_Temp(i)
                    Next
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ImageButtonindice_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonindice.Click
        Session.Item("TIPOVISOR_INDICE_EXPEDIENTE") = "DOCUARCHI.VISOR"
        Me.ifrm_indice_visor_externo_.Attributes.Add("src", "../Docuarchi/WebFormDaIndiceDocuarhi.aspx")
        Me.UpdatePanelindice.Update()
    End Sub
    Private Sub ImageButtoninfo_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtoninfo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassConsultaGabinete
            Dim Matri() As String
            Erase Matri
            Dim Result As String = Refclas.Retorna_datos_sistema_imagen_info(Session.Item("DA_IMAGEN"), Session.Item("DA_GABINETE_CONSULTA"), Matri)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
                Exit Sub
            End If
            Me.TextBox_info.Text = ""
            For i As Integer = 0 To Matri.Length - 1
                Me.TextBox_info.Text = Me.TextBox_info.Text & Matri(i).ToString & vbCrLf
            Next
            'Me.TextBox_info.Text = Me.TextBox_info.Text & "Ruta Documento " & Matri_Doc_Visual(1)
            UpdatePane_info.Update()
            ModalPopupExtender_info.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
End Class