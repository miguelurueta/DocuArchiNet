Imports Neodynamic.WebControls.ImageDraw
Public Class WebFormGaVisorClasificacion
    Inherits System.Web.UI.Page
    Public Matri_Doc_Visual() As String
    Public Doc_actual As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim clasjava As New Classscrripjava
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            Dim spli_sel() As String = Session.Item("CC_SESIONITERCAMBIOVISOR").ToString.Split("|")
            Dim Refclas As New Classselecciotarea
            Dim Ref2 As New ClassListandoTareas
            Dim Result As String = ""
            Result = ""
            Dim Id_Activida As String = ""
            If Me.IsPostBack = False Then
                Me.Hidden_id_imagen.Value = spli_sel(0)
                Me.Hidden_gabinete.Value = spli_sel(1)             
                ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
                ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
                Dim Refclas_wf_visor As New ClassWorflowVisor
                Result = Refclas_wf_visor.Lista_documento_visor_clasificacion(Matri_Doc_Visual,
                                                                              Me.LabelConteo,
                                                                              Doc_actual,
                                                                              Me.Hidden_tipo_visor_externo,
                                                                              Me,
                                                                              Me.ifrm_visor_,
                                                                              DropDownList_zom,
                                                                             UpdatePanelButon)
                If Result <> "YES" Then
                    'Me.Label1.Text = Result
                End If
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("CC_MATRI_IMAGE_EMERGENTE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
        Catch ex As Exception
            'Me.Label1.Text = ex.Message
        End Try
    End Sub
    Protected Sub ImageButton_toponimica_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_toponimica.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassGaExpediente
            'Session.Item("DA_GABINETE_CONSULTA")
            If Me.Hidden_id_imagen.Value = "0" Or Me.Hidden_id_imagen.Value = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar el registro para la ubicación", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim id_expediente As Integer = 0
            Dim refclas_consulta As New ClassConsultaGabinete
            Result = refclas_consulta.Retorna_id_expediente_documento(Me.Hidden_gabinete.Value, Me.Hidden_id_imagen.Value, id_expediente)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Retorna_Ubicacion_expediente_por_codigo_unico(id_expediente, _
                                                                           Me.TreeViewArchivo_u_b_t, _
                                                                           "")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.UpdatePanelViewArchivo_u_b_t.Update()
                Me.ModalPopupExtende_ubicacion_toponimica_expediente_popup.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub ImageButtonSiguiente_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonSiguiente.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.ImageButtonSiguiente.Enabled = False
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "+1", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   DropDownList_zom, _
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
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
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "-1", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_SIZE_EMERGENTE"), _
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
                                                                   HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_SIZE_EMERGENTE"), _
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
                                                                   HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_SIZE_EMERGENTE"), _
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
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual, _
                                                                          "-", _
                                                                          Me, _
                                                                          HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"), _
                                                                          DropDownList_zom, _
                                                                          UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
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
                                                                          HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"), _
                                                                          DropDownList_zom, _
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
                                                                              HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
                                                                              HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), _
                                                                              HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"), _
                                                                              DropDownList_zom, _
                                                                              UpdatePanelButon)
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
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 180, Me, HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
            HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"))
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
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 270, Me, HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
            HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"))
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub

    Private Sub ImageRotate45_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate45.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 90, Me, HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
            HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"))
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
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "seleccion", _
                                                                   Val(Me.LabelConteo.Text), _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("CC_IMAGE_WITH_SIZE_EMERGENTE"), _
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
        Dim clasjava As New Classscrripjava
        Try
            '0-ruta documento 1-id workflow 2-id imagen   3-extension documento, 4-estado, 5-nombre gabinete
            Dim seleccion() As String = Session.Item("CC_TAGSELECCION_EMERGENTE").ToString.Split("|")
            If seleccion.Length = 1 Then Exit Sub
            If seleccion(2) = "-1" Then
                clasjava.Showscripman_menu("Debe seleccionar un registro para ver el indice ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = seleccion(5)
            Session.Item("DA_IMAGEN") = seleccion(2)
            Me.ifrm_indice_visor_docuarchi_.Attributes.Add("src", "../Docuarchi/WebFormDaIndiceDocuarhi.aspx")
            Me.UpdatePanelindice_visor.Update()
            Session.Item("TIPOVISOR_INDICE_EXPEDIENTE") = "DOCUARCHI.VISOR"
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanelButon)
            Exit Sub
        End Try
    End Sub
    Private Sub ImageButtonguardardocumento_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonguardardocumento.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            '0-ruta documento 1-id workflow 2-id imagen   3-extension documento, 4-estado, 5-nombre gabinete
            Dim seleccion() As String = Session.Item("CC_TAGSELECCION_EMERGENTE").ToString.Split("|")
            If seleccion.Length = 1 Then Exit Sub
            If seleccion(5) = "" Then
                scri.Showscripman("Debe seleccionar una gabinete ", Me.UpdatePanelButon)
                Exit Sub
            End If
            Result = refgabinete.SolicitaPermisosGeneralesGabinete(seleccion(5),
                                                                   HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                   stru_permiso)
            If Result <> "YES" Then
                scri.Showscripman("Imposible guardar " & Result, Me.UpdatePanelButon)
                Exit Sub
            End If
            If stru_permiso.GUARDAR_IMAGEN = 0 Then
                scri.Showscripman("El usuario no tiene permisos para guardar desde gabinete ", Me.UpdatePanelButon)
                Exit Sub
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("CC_MATRI_IMAGE_EMERGENTE"), "|")
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
                scri.Showscripman("Imposible consultar descargar la matriz de documentos esta vacia ", Me.UpdatePanelButon)
                Exit Sub
            End If
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("CC_MATRI_IMAGE_EMERGENTE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
           
            Session.Item("DA_GABINETE_IMPRESION") = seleccion(5)
            Session.Item("DA_ID_IMAGEN_IMPRESION") = seleccion(2)
            ifimpre_descarga_anexo_respuesta_.Attributes.Add("src", "../Docuarchi/WebFormDaExportArchivo.aspx")
            Me.ModalPopupExtender_edition_descarga_anexo_respuesta.Show()
            Me.UpdatePanel_descarga_anexo_respuesta.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanelButon)
        End Try
    End Sub
    
    Private Sub ImageButtonimprimir_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonimprimir.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            'If HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = "" Then
            '    scri.Showscripman_menu("Debe seleccionar una gabinete ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim seleccion() As String = Session.Item("CC_TAGSELECCION_EMERGENTE").ToString.Split("|")
            If seleccion.Length = 1 Then Exit Sub
            If seleccion(5) = "" Then
                scri.Showscripman_menu("Debe seleccionar una gabinete ", Me.UpdatePanelButon, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Result = refgabinete.SolicitaPermisosGeneralesGabinete(seleccion(5),
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
            Matri_Temp = Split(Session.Item("CC_MATRI_IMAGE_EMERGENTE"), "|")
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
            Session.Item("DA_GABINETE_IMPRESION") = seleccion(5)
            Session.Item("DA_ID_IMAGEN_IMPRESION") = seleccion(2)
            Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL") = Matri_Doc_Visual(HttpContext.Current.Session.Item("DA_DOC_ACTUAL_EMERGENTE"))
            Me.ModalPopupExtenderimpre_post.Show()
            UpdatePaneliframe_post.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanelButon)
        End Try
    End Sub
End Class