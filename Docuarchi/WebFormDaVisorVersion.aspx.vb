Imports Neodynamic.WebControls.ImageDraw
Public Class WebFormDaVisorVersion
    Inherits System.Web.UI.Page
    Public Matri_Doc_Visual() As String
    Public Doc_actual As Integer = 0
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If

            If Page.IsPostBack = False Then
                Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
                Dim Result As String = ""
                Result = Class_ra_ver_version_documento.Load_visor_tiff_version(Session.Item("VER_ID_REGISTRO_VERSION"),
                                                                                Page,
                                                                                DropDownList_zom,
                                                                                UpdatePanelButon,
                                                                                Doc_actual,
                                                                                Matri_Doc_Visual)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                End If
            Else
                Dim Matri_Temp() As String
                Erase Matri_Temp
                Matri_Temp = Split(Session.Item("VER_MATRI_IMAGE_EMERGENTE"), "|")
                If Not Matri_Temp Is Nothing Then
                    For i As Integer = 0 To Matri_Temp.Length - 1
                        ReDim Preserve Matri_Doc_Visual(i)
                        Matri_Doc_Visual(i) = Matri_Temp(i)
                    Next
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub WebFormDaVisorVersion_Init(sender As Object, e As EventArgs) Handles Me.Init
        ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
        ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
    End Sub
    Private Sub ImageButtonSiguiente_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonSiguiente.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Class_ra_ver_version_documento.Show_visor_tif_version_documento(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "+1",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_WITH_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_SIZE_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_WITH_SIZE_EMERGENTE"),
                                                                   DropDownList_zom,
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
    Private Sub ImageButtonAnterior_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonAnterior.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Class_ra_ver_version_documento.Show_visor_tif_version_documento(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "-1",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_WITH_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_SIZE_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_WITH_SIZE_EMERGENTE"),
                                                                   DropDownList_zom,
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
    Private Sub ImageButtonFinal_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonFinal.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Class_ra_ver_version_documento.Show_visor_tif_version_documento(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "final",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_WITH_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_SIZE_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_WITH_SIZE_EMERGENTE"),
                                                                   DropDownList_zom,
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
    Private Sub ImageButtonInicio_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonInicio.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Class_ra_ver_version_documento.Show_visor_tif_version_documento(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "inicio",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_WITH_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_SIZE_EMERGENTE"),
                                                                   HttpContext.Current.Session.Item("VER_IMAGE_WITH_SIZE_EMERGENTE"),
                                                                   DropDownList_zom,
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
    Protected Sub ImageMenos_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMenos.Click
        Dim scri As New Classscrripjava
        Try
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Class_ra_ver_version_documento.Escale_visor_tiff_version(Matri_Doc_Visual, "-",
                                                                          Me, HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE"),
                                                                          HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_EMERGENTE"),
                                                                          HttpContext.Current.Session.Item("VER_IMAGE_WITH_EMERGENTE"),
                                                                          DropDownList_zom,
                                                                          UpdatePanelButon)

            If Result <> "YES" Then
                scri.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            scri.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub

    Private Sub ImageMas_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMas.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Class_ra_ver_version_documento.Escale_visor_tiff_version(Matri_Doc_Visual, "+",
                                                                                            Me,
                                                                                            HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE"),
                                                                                            HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_EMERGENTE"),
                                                                                            HttpContext.Current.Session.Item("VER_IMAGE_WITH_EMERGENTE"),
                                                                                            DropDownList_zom,
                                                                                            UpdatePanelButon)

            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
    Private Sub DropDownList_zom_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_zom.SelectedIndexChanged
        Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Class_ra_ver_version_documento.Escale_visor_tiff_version_drow_list(Matri_Doc_Visual,
                                                                              DropDownList_zom.SelectedValue,
                                                                              Me,
                                                                              HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE"),
                                                                              HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_EMERGENTE"),
                                                                              HttpContext.Current.Session.Item("VER_IMAGE_WITH_EMERGENTE"),
                                                                              DropDownList_zom,
                                                                              UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
End Class