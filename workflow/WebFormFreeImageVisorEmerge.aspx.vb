Imports Neodynamic.WebControls.ImageDraw

Public Class WebFormFreeImageVisorEmerge
    Inherits System.Web.UI.Page
    Public Matri_Doc_Visual() As String
    Public Doc_actual As String = ""

    Private Sub WebFormFreeImageVisorEmerge_Init(sender As Object, e As EventArgs) Handles Me.Init
        ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
        ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Page.IsPostBack = False Then
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = ""
            HttpContext.Current.Session.Item("WF_IMAGE_TEMPORAL_EMERGENTE") = ""
            HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE") = "1"
            Session.Item("WF_MATRI_IMAGE_EMERGENTE") = ""
            Result = Refclas.Visualizacion_Documentos(Matri_Doc_Visual, _
                                                      HttpContext.Current.Session("WF_TAGSELECCION_EMERGENTE"), _
                                                      HttpContext.Current.Session("WF_PAGINASELECCION_EMERGENTE"), _
                                                      HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_EMERGENTE"))
            If Result = "YES" Then

                If Not Matri_Doc_Visual Is Nothing Then
                    For i As Integer = 0 To Matri_Doc_Visual.Length - 1
                        Session.Item("WF_MATRI_IMAGE_EMERGENTE") = Session.Item("WF_MATRI_IMAGE_EMERGENTE") & Matri_Doc_Visual(i) & "|"
                    Next
                    Result = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                             Doc_actual, _
                                                             "inicio", _
                                                             0, _
                                                             Me, _
                                                             HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                             HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                             HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                             HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                             HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                             DropDownList_zom, _
                                                             UpdatePanelButon)

                End If

            End If


        Else

            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE_EMERGENTE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If

        End If
    End Sub
    Private Sub ImageButtonSiguiente_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonSiguiente.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.ImageButtonSiguiente.Enabled = False
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "+1", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        Finally
            Me.ImageButtonSiguiente.Enabled = True
        End Try
    End Sub
    Private Sub ImageButtonAnterior_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonAnterior.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "-1", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try

    End Sub
    Private Sub ImageButtonFinal_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonFinal.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "final", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
      
    End Sub
    Private Sub ImageButtonInicio_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonInicio.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "inicio", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try

    End Sub
    Protected Sub ImageMenos_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMenos.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual, _
                                                                          "-", _
                                                                          Me, _
                                                                          HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                          DropDownList_zom, _
                                                                          UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try  
    End Sub

    Private Sub ImageMas_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMas.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual, _
                                                                          "+", _
                                                                          Me, _
                                                                          HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                          DropDownList_zom, _
                                                                          UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try     
    End Sub
    Private Sub DropDownList_zom_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_zom.SelectedIndexChanged
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale_zom(Matri_Doc_Visual, _
                                                                              DropDownList_zom.SelectedValue, _
                                                                              Me, _
                                                                               HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                               HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                               HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                              DropDownList_zom, _
                                                                              UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
End Class