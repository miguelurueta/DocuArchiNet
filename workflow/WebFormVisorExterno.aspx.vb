Imports Neodynamic.WebControls.ImageDraw

Public Class WebFormVisorExterno
    Inherits System.Web.UI.Page
    Public Matri_Doc_Visual() As String
    Public Doc_actual As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().clired_user();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            Dim spli_sel() As String = Session.Item("SESIONITERCAMBIOVISOR").ToString.Split("|")
            Dim Refclas As New Classselecciotarea
            Dim Ref2 As New ClassListandoTareas
            Dim Result As String = ""
            Result = ""
            Dim Ref_Class_grupos_workflow As New Class_grupos_workflow
            Dim Id_Activida As String = ""
            Dim value_default As String = ""
            If Me.IsPostBack = False Then
                ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
                ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
                
                If spli_sel(0) = "VISOR WORKFLOW" Then
                    Result = Ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Activida, _
                                                                                              HttpContext.Current.Session("Id_Grupo_Workflow"))
                    If Result <> "YES" Then
                        Me.Label1.Text = Result
                        Exit Sub
                    End If
                    Dim id_activ = Val(Id_Activida)
                    Dim id_tarea As Integer = Val(spli_sel(1))
                    Dim ref_ClassDaGabinete As New ClassDaGabinete
                    Result = ref_ClassDaGabinete.Lista_documentos_visor_workflow(id_tarea, _
                                                                                 Me.TreeViewseleccion, _
                                                                                 0, _
                                                                                 id_activ, _
                                                                                 0, _
                                                                                 Matri_Doc_Visual, _
                                                                                 -1, _
                                                                                 value_default)
                    If Result <> "YES" Then
                        Me.Label1.Text = Result
                        Exit Sub
                    End If
                    Dim Refclas_visor As New ClassWorflowVisor
                    If Not Matri_Doc_Visual Is Nothing Then
                        Session.Item("TIPO_VISOR_PDF") = ""
                        Result = Refclas_visor.Selecion_treiew_documento_visor_workflow_externo(Matri_Doc_Visual,
                                                                                                Me.LabelConteo.Text,
                                                                                                Doc_actual,
                                                                                                Me.Hidden_tipo_visor_externo,
                                                                                                Me.Page,
                                                                                                Me.ifrm_visor_,
                                                                                                value_default,
                                                                                                Me.UpdatePanel_ifr_visor,
                                                                                                UpdatePanelButon,
                                                                                                DropDownList_zom,
                                                                                                UpdatePanelButon)
                        If Result <> "YES" Then
                            Me.Label1.Text = Result
                            'Exit Sub
                        Else
                            Me.Hidden_estado_visor.Value = "1"
                        End If
                        If Me.TreeViewseleccion.Nodes.Count > 0 Then
                            If Me.TreeViewseleccion.Nodes.Count > 0 Then
                                Me.TreeViewseleccion.Nodes(0).Selected = True
                            End If
                        End If
                    End If
                End If
                If spli_sel(0) = "VISOR RADICADOR" Then
                    Dim Refclasvis As New ClassRaVisualizaRadicado
                    Dim radicado As String = Val(spli_sel(1))
                    Dim id_plantilla As String = spli_sel(2)
                    Result = Refclasvis.visualiza_documentos_radicado(radicado, _
                                                                      id_plantilla, _
                                                                      Me.TreeViewseleccion, _
                                                                      value_default)
                    If Result <> "YES" Then
                        Me.Label1.Text = Result
                        'Exit Sub
                    End If
                    Result = Refclasvis.visualiza_documentos_relacionados_respuesta(radicado, _
                                                                                    0, _
                                                                                    Me.TreeViewseleccion)
                    If Result <> "YES" Then
                        Me.Label1.Text = Result
                        Exit Sub
                    End If
                End If
               
            End If
            
            If Me.IsPostBack = False Then
                If spli_sel(0) = "VISOR WORKFLOW" Or spli_sel(0) = "VISOR RADICADOR" Then
                    Dim Refclas_visor As New ClassWorflowVisor
                    If Not Matri_Doc_Visual Is Nothing Then
                        Session.Item("TIPO_VISOR_PDF") = ""
                        Result = Refclas_visor.Selecion_treiew_documento_visor_workflow_externo(Matri_Doc_Visual,
                                                                                                Me.LabelConteo.Text,
                                                                                                Doc_actual,
                                                                                                Me.Hidden_tipo_visor_externo,
                                                                                                Me.Page,
                                                                                                Me.ifrm_visor_,
                                                                                                value_default,
                                                                                                Me.UpdatePanel_ifr_visor,
                                                                                                UpdatePanelButon,
                                                                                                DropDownList_zom,
                                                                                                UpdatePanelButon)
                        If Result <> "YES" Then
                            Me.Label1.Text = Result
                            Exit Sub
                        Else
                            Me.Hidden_estado_visor.Value = "1"
                        End If
                        If Me.TreeViewseleccion.Nodes.Count > 0 Then
                            If Me.TreeViewseleccion.Nodes(0).ChildNodes.Count > 0 Then
                                Me.TreeViewseleccion.Nodes(0).ChildNodes(0).Selected = True
                            End If
                        End If
                    End If

                End If
            Else
                Me.Hidden_estado_visor.Value = "0"
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE_EMERGENTE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
        Catch ex As Exception
            Me.Label1.Text = ex.Message
        End Try
    End Sub

    Protected Sub TreeViewseleccion_SelectedNodeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TreeViewseleccion.SelectedNodeChanged
        Dim Mens As New Classscrripjava
        Try

            Dim Refclas_visor As New ClassWorflowVisor
            Dim Result As String = ""
            Session.Item("TIPO_VISOR_PDF") = ""
            Result = Refclas_visor.Selecion_treiew_documento_visor_workflow_externo(Matri_Doc_Visual,
                                                                                    Me.LabelConteo.Text, Doc_actual,
                                                                                    Me.Hidden_tipo_visor_externo,
                                                                                    Me.Page,
                                                                                    Me.ifrm_visor_,
                                                                                    sender.selectedvalue(),
                                                                                    Me.UpdatePanel_ifr_visor,
                                                                                    UpdatePanelButon,
                                                                                    DropDownList_zom,
                                                                                    UpdatePanelButon)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanelseleccion)
                Exit Sub
            End If
            Me.UpdatePanel_conte_bot.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanelseleccion)
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
            Me.UpdatePanel_conte_bot.Update()
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
            Me.UpdatePanel_conte_bot.Update()
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
            Me.UpdatePanel_conte_bot.Update()
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
            Me.UpdatePanel_conte_bot.Update()
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
                                                                          HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                          DropDownList_zom, _
                                                                          UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
            Me.UpdatePanel_drows_bot.Update()
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
                                                                          HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                          DropDownList_zom, _
                                                                          UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
            Me.UpdatePanel_drows_bot.Update()
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
    Private Sub ImageRotate180_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate180.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            If Matri_Doc_Visual Is Nothing Then Exit Sub
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 180, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
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
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 270, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
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
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 90, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
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
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                   Me.DropDownList_zom, _
                                                                   UpdatePanelButon)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanelButon)
            End If
            Me.UpdatePanel_conte_bot.Update()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelButon)
        End Try
    End Sub
    Private Sub ImageButtonindice_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonindice.Click
        Dim clasjava As New Classscrripjava
        Try
            '0-ruta documento 1-id workflow 2-id imagen   3-extension documento, 4-estado, 5-nombre gabinete
            Dim seleccion() As String = Session.Item("WF_TAGSELECCION_EMERGENTE").ToString.Split("|")
            If seleccion.Length = 1 Then Exit Sub
            If seleccion(2) = "-1" Then
                clasjava.Showscripman("Debe seleccionar un registro para ver el indice", UpdatePanelButon)
                Exit Sub
            End If
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = seleccion(5)
            Session.Item("DA_IMAGEN") = seleccion(2)
            Me.ifrm_indice_.Attributes.Add("src", "../Docuarchi/WebFormDaIndiceDocuarhi.aspx")
            Me.UpdatePanelindice.Update()
            Me.ModalPopupExtenderimpre_indice.Show()
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
            Dim seleccion() As String = Session.Item("WF_TAGSELECCION_EMERGENTE").ToString.Split("|")
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
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE_EMERGENTE"), "|")
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
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE_EMERGENTE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
            'Dim datos_log As String = ""
            'Result = refgabinete.Retorna_Datos_Auditoria_Gabinete(Session.Item("DA_IMAGEN"), Session.Item("DA_GABINETE_CONSULTA"), datos_log)
            'If Result <> "YES" Then
            '    scri.Showscripman("Imposible encontrar datos log " & Result, Me.UpdatePanelButon)
            '    Exit Sub
            'End If
            'Dim selecion As String = ""
            'Result = refgabinete.Registra_Auditoria_Eventos(HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"), selecion & " Imagen Principal " & Matri_Doc_Visual(0), HttpContext.Current.Session.Item("DA_IMAGEN"), datos_log, "Guardar")
            'If Result <> "YES" Then
            '    scri.Showscripman("Imposible registrar datos log " & Result, Me.UpdatePanelButon)
            '    Exit Sub
            'End If
            Session.Item("DA_GABINETE_IMPRESION") = seleccion(5)
            Session.Item("DA_ID_IMAGEN_IMPRESION") = seleccion(2)
            Iframe_guardar.Attributes.Add("src", "../Docuarchi/WebFormDaExportArchivo.aspx")
            Me.ModalPopupExtender_guardar.Show()
            UpdatePane_guardar.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, UpdatePanelButon)
        End Try
    End Sub
End Class