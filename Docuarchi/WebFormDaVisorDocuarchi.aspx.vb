Imports Neodynamic.WebControls.ImageDraw

Public Class WebFormDaVisorDocuarchi
    Inherits System.Web.UI.Page
    Public Matri_Doc_Visual() As String
    Public Doc_actual As String = ""

    Private Sub WebFormDaVisorDocuarchi_Init(sender As Object, e As EventArgs) Handles Me.Init
        ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
        ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
    End Sub

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
                HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE") = "1"
                Me.Hidden_gabinete.Value = Session.Item("DA_GABINETE_CONSULTA")
                Me.Hidden_imagen.Value = Session.Item("DA_IMAGEN")
                Result = Refclas.Visualizacion_Documentos_da(Matri_Doc_Visual,
                                                             Session.Item("DA_IMAGEN"),
                                                             Session.Item("DA_GABINETE_CONSULTA"))
                If Result = "YES" Then
                    If Not Matri_Doc_Visual Is Nothing Then
                        For i As Integer = 0 To Matri_Doc_Visual.Length - 1
                            Session.Item("DA_MATRI_IMAGE_EMERGENTE") = Session.Item("DA_MATRI_IMAGE_EMERGENTE") & Matri_Doc_Visual(i) & "|"
                        Next
                        Result = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                 Doc_actual, _
                                                                 "inicio", _
                                                                 0, _
                                                                 Me, _
                                                                 HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                 HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                 HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"), _
                                                                 HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                 HttpContext.Current.Session.Item("DA_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                 DropDownList_zom, _
                                                                 UpdatePanelButon)
                        If Result <> "YES" Then
                            Label_estado.Text = Label_estado.Text & Result
                        End If
                    End If
                End If
                Dim refgabinete As New ClassDaGabinete
                Dim datos_log As String = ""
                Result = refgabinete.Retorna_Datos_Auditoria_Gabinete(Session.Item("DA_IMAGEN"), _
                                                                      Session.Item("DA_GABINETE_CONSULTA"), _
                                                                      datos_log)
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                End If
                Dim selecion As String = ""
                Result = refgabinete.Registra_Auditoria_Eventos(HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"), _
                                                                selecion & " Imagen Principal " & Matri_Doc_Visual(0), _
                                                                HttpContext.Current.Session.Item("DA_IMAGEN"), _
                                                                datos_log, _
                                                                "Visualiza")
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
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
            Label_estado.Text = Label_estado.Text & ex.Message
        End Try
    End Sub
    Private Sub ImageButtonSiguiente_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonSiguiente.Click
        Dim clasjava As New Classscrripjava
        Try

            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "+1", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
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
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "-1", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
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
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "final", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
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
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "inicio", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
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
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual, "-", _
                                                                          Me, HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"), _
                                                                          DropDownList_zom, _
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
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual, _
                                                                          "+", _
                                                                          Me, _
                                                                          HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"), _
                                                                          Me.DropDownList_zom, _
                                                                          UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
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
                                                                              HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                              HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                              HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"), _
                                                                              DropDownList_zom, _
                                                                              UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub

    Private Sub ImageButtonindice_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonindice.Click
        Session.Item("TIPOVISOR_INDICE_EXPEDIENTE") = "DOCUARCHI.VISOR"
        Me.ifrm_indice_visor_docuarchi_.Attributes.Add("src", "../Docuarchi/WebFormDaIndiceDocuarhi.aspx")
        Me.UpdatePanelindice.Update()
    End Sub

    Private Sub ImageButtonimprimir_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonimprimir.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            If HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = "" Then
                scri.Showscripman_menu("Debe seleccionar una gabinete ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = refgabinete.SolicitaPermisosGeneralesGabinete(HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"),
                                                                   HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                   stru_permiso)
            If Result <> "YES" Then
                scri.Showscripman_menu("Imposible imprimir ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru_permiso.IMPRI_IMAGEN = 0 Then
                scri.Showscripman_menu("El usuario no tiene permisos para imprimir en el gabinete ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("DA_MATRI_IMAGE_EMERGENTE"), "|")
            Session.Item("RA_RUTA_IMPRESION_FINAL") = ""
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    'ReDim Preserve Matri_Doc_Visual(i)
                    If i = 1 Then
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Matri_Temp(i)
                    Else
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Session.Item("RA_RUTA_IMPRESION_FINAL") & "," & Matri_Temp(i)
                    End If

                Next
            End If

            If Session.Item("RA_RUTA_IMPRESION_FINAL") = "" Then
                scri.Showscripman_menu("Imposible consultar imprimir la matriz de documentos esta vacia ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("DA_MATRI_IMAGE_EMERGENTE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
            Session.Item("DA_GABINETE_IMPRESION") = Session.Item("DA_GABINETE_CONSULTA")
            Session.Item("DA_ID_IMAGEN_IMPRESION") = Session.Item("DA_IMAGEN")
            Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL") = Matri_Doc_Visual(HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"))
            Me.ModalPopupExtenderimpre_post.Show()
            UpdatePaneliframe_post.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanelButon)
        End Try
    End Sub

    Protected Sub ImageButtonguardardocumento_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonguardardocumento.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            If HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = "" Then
                scri.Showscripman_menu("Debe seleccionar una gabinete ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = refgabinete.SolicitaPermisosGeneralesGabinete(HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"),
                                                                   HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                   stru_permiso)
            If Result <> "YES" Then
                scri.Showscripman_menu("Imposible guardar ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If stru_permiso.GUARDAR_IMAGEN = 0 Then
                scri.Showscripman_menu("El usuario no tiene permisos para guardar desde gabinete ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("DA_MATRI_IMAGE_EMERGENTE"), "|")
            Session.Item("RA_RUTA_IMPRESION_FINAL") = ""
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    'ReDim Preserve Matri_Doc_Visual(i)
                    If i = 1 Then
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Matri_Temp(i)
                    Else
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Session.Item("RA_RUTA_IMPRESION_FINAL") & "," & Matri_Temp(i)
                    End If
                Next
            End If
            If Session.Item("RA_RUTA_IMPRESION_FINAL") = "" Then
                scri.Showscripman_menu("Imposible consultar descargar la matriz de documentos esta vacia ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("DA_MATRI_IMAGE_EMERGENTE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
            Session.Item("DA_GABINETE_IMPRESION") = Session.Item("DA_GABINETE_CONSULTA")
            Session.Item("DA_ID_IMAGEN_IMPRESION") = Session.Item("DA_IMAGEN")
            ifimpre_descarga_anexo_respuesta_.Attributes.Add("src", "../Docuarchi/WebFormDaExportArchivo.aspx")
            Me.ModalPopupExtender_edition_descarga_anexo_respuesta.Show()
            Me.UpdatePanel_descarga_anexo_respuesta.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanelButon)
        End Try

    End Sub

    Private Sub ImageRotate45_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate45.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, _
                                                                          90, _
                                                                          Me, _
                                                                          HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub

    Private Sub ImageRotate180_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate180.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, _
                                                                          180, _
                                                                          Me, _
                                                                          HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub

    Private Sub ImageRotate270_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate270.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, _
                                                                          270, _
                                                                          Me, _
                                                                          HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub

    Private Sub ImageButton_ir_pagina_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_ir_pagina.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.LabelConteo.Text = "" Then Exit Sub
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "seleccion", _
                                                                   Val(Me.LabelConteo.Text), _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("DA_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
   

    Private Sub ImageButtoninfo_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtoninfo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassConsultaGabinete
            Dim Matri() As String
            Erase Matri
            Dim Result As String = Refclas.Retorna_datos_sistema_imagen_info(Session.Item("DA_IMAGEN"), _
                                                                             Session.Item("DA_GABINETE_CONSULTA"), _
                                                                             Matri)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.TextBox_info.Text = ""
            For i As Integer = 0 To Matri.Length - 1
                Me.TextBox_info.Text = Me.TextBox_info.Text & Matri(i).ToString & vbCrLf
            Next
            Me.TextBox_info.Text = Me.TextBox_info.Text & "Ruta Documento " & Matri_Doc_Visual(1)
            UpdatePane_info.Update()
            ModalPopupExtender_info.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
End Class