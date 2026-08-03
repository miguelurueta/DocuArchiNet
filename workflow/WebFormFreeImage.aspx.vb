Imports System.IO
Imports Neodynamic.WebControls.ImageDraw

Public Class WebFormFreeImage
    Inherits RefreshArticle.BasePage
    Public Matri_Doc_Visual() As String
    Public Doc_actual As String = ""
    Public ruta_documento As String = ""

    Private Sub WebFormFreeImage_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
       
        ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
        ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
    End Sub

    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr2 As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr2, True)
        End If
        Page.MaintainScrollPositionOnPostBack = True
        '------------------------------------------------------------------------------------------------------------
        'Control boton subir documento, se inactiva apenas carga el nuevo documento asignado manual por el usuario
        '------------------------------------------------------------------------------------------------------------
        'ButtonAceptar.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(ButtonAceptar, Nothing) + ";")
        'ImageButtonSiguiente.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(ImageButtonSiguiente, Nothing) + ";")
        '--------------------------------------------------------------------------
        'Asigna la ruta de la firma mecanica del usuario
        '---------------------------------------------------------------------------
        Hiddenintercambio2.Value = HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL")
        If Page.IsPostBack = False Then
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = ""
            HttpContext.Current.Session.Item("WF_IMAGE_TEMPORAL") = ""
            HttpContext.Current.Session.Item("WF_DOC_ACTUAL") = "1"
            Session.Item("WF_MATRI_IMAGE") = ""
            Result = Refclas.Visualizacion_Documentos(Matri_Doc_Visual, _
                                                      HttpContext.Current.Session("WF_TAGSELECCION"), _
                                                      HttpContext.Current.Session("WF_PAGINASELECCION"), _
                                                      HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"))
            If Result = "YES" Then
                If Not Matri_Doc_Visual Is Nothing Then
                    For i As Integer = 0 To Matri_Doc_Visual.Length - 1
                        Session.Item("WF_MATRI_IMAGE") = Session.Item("WF_MATRI_IMAGE") & Matri_Doc_Visual(i) & "|"
                    Next
                    Result = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                             Doc_actual, _
                                                             "inicio", _
                                                             0, _
                                                             Me, _
                                                             HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
                                                             HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), _
                                                             HttpContext.Current.Session.Item("WF_IMAGE_WITH"), _
                                                             HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"), _
                                                             HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"), _
                                                             DropDownList_zom, _
                                                             Me.UpdatePanel_drows_bot)
                    If Result <> "YES" Then
                        'MsgBox(Result)
                    End If
                End If
            End If
            'Aplicar transparencia sobre la firma
            Dim refclas2 As New ClassNeodynamic
            Dim resolt As String = ""
            resolt = refclas2.Firma_transparente()
            If Session.Item("IMPRIMIR_IMAGENES") = 0 Then
                'Me.im.Visible = False
            Else
                'Bton.Visible = True
            End If
            If Session.Item("AGREGAR_FIRMA") = 0 Then
                Me.ImageFirma.Visible = False
            Else
                Me.ImageFirma.Visible = True
            End If
            If Session.Item("ADJUNTAR_IMAGENES_USUARIO") = 0 Then
                Me.ImageButtonadjunta.Visible = False
            Else
                Me.ImageButtonadjunta.Visible = True
            End If

        Else

            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If

        End If
    End Sub

    Private Sub ImageButtonInicio_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonInicio.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "inicio", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"), _
                                                                   Me.DropDownList_zom, _
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageButtonFinal_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonFinal.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "final", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"), _
                                                                   DropDownList_zom, _
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageButtonAnterior_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonAnterior.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "-1", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"), _
                                                                   DropDownList_zom, _
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageButtonSiguiente_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonSiguiente.Click
        Try
            Dim clasjava As New Classscrripjava
            Me.ImageButtonSiguiente.Enabled = False
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                   Doc_actual, _
                                                                   "+1", _
                                                                   0, _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"), _
                                                                   Me.DropDownList_zom, _
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
        Finally
            Me.ImageButtonSiguiente.Enabled = True
        End Try
    End Sub
    Private Sub ImageButton_ir_pagina_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_ir_pagina.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.LabelConteo.Text = "" Then Exit Sub
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, Doc_actual, _
                                                                   "seleccion", _
                                                                   Val(Me.LabelConteo.Text), _
                                                                   Me, _
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"), _
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"), _
                                                                   Me.DropDownList_zom, _
                                                                   Me.Updatepanel_boton_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
            Me.UpdatePanel_conte_bot.Update()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Protected Sub ImageMenos_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMenos.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual, "-", _
                                                                          Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH"), _
                                                                          DropDownList_zom, _
                                                                          Updatepanel_boton_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageMas_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMas.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual, _
                                                                          "+", _
                                                                          Me, _
                                                                          HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), _
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH"), _
                                                                          DropDownList_zom, _
                                                                          Updatepanel_boton_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Private Sub DropDownList_zom_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_zom.SelectedIndexChanged
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale_zom(Matri_Doc_Visual, DropDownList_zom.SelectedValue, _
                                                                              Me, _
                                                                              HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
                                                                              HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), _
                                                                              HttpContext.Current.Session.Item("WF_IMAGE_WITH"), _
                                                                              DropDownList_zom, _
                                                                              Me.UpdatePanel_drows_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Private Sub ImageRotate45_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate45.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 90, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), HttpContext.Current.Session.Item("WF_IMAGE_WITH"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageRotate180_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate180.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 180, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), HttpContext.Current.Session.Item("WF_IMAGE_WITH"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageRotate270_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate270.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 270, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"), _
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), HttpContext.Current.Session.Item("WF_IMAGE_WITH"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
   

    Private Sub ImageButtonguardar_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonguardar.Click
        Dim Mens As New Classscrripjava
        Try
            Dim refclas As New ClassNeodynamic
            Dim Result As String = ""
            Dim fx, fy, fwith, fhigth, topcontenido, scrootop, tamimag, heigimageor, witimageor As String
            Dim matrival() As String = Split(Me.Hiddenintercambio.Value, "-")
            fy = matrival(0)
            fx = matrival(1)
            fhigth = matrival(2)
            fwith = matrival(3)
            topcontenido = matrival(4)
            scrootop = matrival(5)
            tamimag = matrival(6)
            heigimageor = matrival(7)
            witimageor = matrival(8)
            Dim ruta As String = Matri_Doc_Visual(HttpContext.Current.Session.Item("WF_DOC_ACTUAL"))
            Result = refclas.Shape_Firma(ruta, _
                                         fx, _
                                         fy, _
                                         fwith, _
                                         fhigth, _
                                         Matri_Doc_Visual, _
                                         Me, _
                                         topcontenido, _
                                         scrootop, _
                                         tamimag, _
                                         heigimageor, _
                                         witimageor)
            If Result <> "YES" Then
                Mens.Showscripman("Grabar error " & Result & " " & Me.Hiddenintercambio.Value, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            Mens.Showscripman("Grabar error " & ex.ToString & " " & Me.Hiddenintercambio.Value, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageButtonadjunta_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonadjunta.Click   
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("ADJUNTAR_IMAGENES_USUARIO") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.Updatepanel_boton_content)
                Exit Sub
            End If 
            ModalPopupExtender_seleccion_tipo_adjunto.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    
    Protected Sub Button_guardar_automatico_Click(sender As Object, e As EventArgs) Handles Button_guardar_automatico.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim estado_lista As String = ""
            Dim Ref_classAlmacenamiento As New ClassAlmacenamiento
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim option_sel As Integer = 1
            Me.HiddenField_estado_guarda.Value = ""
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            If Me.Check_anexo_radicado_automatico.Checked = True Then
                Result = Ref_classAlmacenamiento.Adjunta_imagen_default(option_sel,
                                                                        Me.Page,
                                                                        0,
                                                                        Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue),
                                                                        0,
                                                                        HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                        stru_datos_image_lista)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()
                End If
            End If
            If Me.CheckBox_relacionado_radicado_automatico.Checked = True Then
                Dim estado_resultado As String = ""
                Dim Refclas_config As New Class_ra_dig_config_digitalizacion
                If Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue) <> -1 And Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue) <> 0 Then
                    Session.Item("DG_LISTA_CHEQUEO") = Me.Hidden_0001.Value
                    Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
                    Dim Refclas_workflow As New ClassWorkflow
                    Result = Ref_classAlmacenamiento.Adjunta_imagen_default(2,
                                                                            Me.Page,
                                                                            0,
                                                                            Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue),
                                                                            0,
                                                                            HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"), stru_datos_image_lista)
                    If Result <> "YES" Then
                        clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
                        Exit Sub
                    Else
                        Me.Hidden_list_cheo_acepta.Value = "YES"
                        Me.HiddenField_estado_guarda.Value = "YES"
                        Me.UpdatePane_seleccion_tipo_adjunto.Update()
                        Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()
                        'Me.ModalPopupExtender_edition_lista_chequeo_tramite.Hide()
                    End If
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = Me.Hidden_0001.Value
                    Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
                    Result = Ref_classAlmacenamiento.Adjunta_imagen_default(2,
                                                                            Me.Page,
                                                                            0,
                                                                            Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue),
                                                                            0,
                                                                            HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"), stru_datos_image_lista)
                    If Result <> "YES" Then
                        clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
                        Exit Sub
                    Else
                        Me.Hidden_list_cheo_acepta.Value = "YES"
                        Me.HiddenField_estado_guarda.Value = "YES"
                        Me.UpdatePane_seleccion_tipo_adjunto.Update()
                        Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()
                    End If
                End If
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePane_adjunta_autamatico_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_guardar_desicion_Click(sender As Object, e As EventArgs) Handles Button_guardar_desicion.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAlmacenamiento
            Dim Refclas_digitalizacion As New ClassWorkflowDigitalizacion
            Dim id_imagen As Integer = 0
            Dim estado_lista As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                Exit Sub
            End If
            Me.Hidden_tip_adjunt_.Value = "wf"
            If Me.Check_anexo_radicado_adj.Checked = True Then
                Result = Refclas.Adjunta_documento_parte_documento(Me.Page)
                If Result <> "YES" Then
                    CLAS.Showscripman(Result, UpdatePane_seleccion_tipo_adjunto)
                    ModalPopupExtender_sube_documento_adjunto.Hide()
                    Exit Sub
                Else
                    ModalPopupExtender_sube_documento_adjunto.Hide()
                End If

            End If
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            If Me.CheckBox_relacionado_radicado_adj.Checked = True Then
                Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Session.Item("ID_TAREA_SELECCIONDA"), radicado)
                If Val(Me.DropDownList_adjunta_documento.SelectedValue) = -1 Or Val(Me.DropDownList_adjunta_documento.SelectedValue) = 0 Then
                    Result = Refclas.Adjunta_donumento_relacionado(Me.Page,
                                                                   id_imagen,
                                                                   HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                   HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                   Val(Me.DropDownList_adjunta_documento.SelectedValue),
                                                                   Session.Item("ID_TAREA_SELECCIONDA"),
                                                                   radicado,
                                                                   stru_datos_image_lista,
                                                                   0)
                    If Result <> "YES" Then
                        CLAS.Showscripman(Result, UpdatePane_seleccion_tipo_adjunto)
                        Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        Exit Sub
                    Else
                        Me.Hidden_date_row_.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                       "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento
                        Me.HiddenField_estado_guarda.Value = "YES"
                        Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                    End If
                Else
                    Result = Refclas.Adjunta_donumento_relacionado(Me.Page,
                                                                  id_imagen,
                                                                  HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                  HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                  Val(Me.DropDownList_adjunta_documento.SelectedValue),
                                                                  Session.Item("ID_TAREA_SELECCIONDA"),
                                                                  radicado,
                                                                  stru_datos_image_lista,
                                                                  0)
                    If Result <> "YES" Then
                        CLAS.Showscripman(Result, UpdatePane_seleccion_tipo_adjunto)
                        Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        Session.Item("DG_LISTA_CHEQUEO") = -1
                        Exit Sub
                    Else
                        Me.Hidden_date_row_.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                       "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento
                        Session.Item("DG_LISTA_CHEQUEO") = -1
                        Me.HiddenField_estado_guarda.Value = "YES"
                        Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                    End If
                End If
            End If
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePane_seleccion_tipo_adjunto)    
        End Try
    End Sub
    Private Sub Button_tool_activa_sube_documento_automatico_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento_automatico.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("ID_TAREA_SELECCIONDA") = -1 Or Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim Result As String = ""
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"), _
                                                                                            Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar la imagen relacionada a la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE, _
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownList_adjunta_documento_automatico.Items.Clear()
            Me.UpdatePane_adjunta_autamatico_documento.Update()
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = structure_gabinete_workflow.NOMBRE_GABINETE
            'HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = structure_datos_tarea_workflow.ID_IMAGEN
            '------------------------------------------
            'Verfica lista existencia lista de chequeo
            '------------------------------------------
            Dim estado_lista As String = ""
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                               estado_lista)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            If estado_lista = "YES" Then
                If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                    Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
                                                                                     Session.Item("DG_TIPO_TRAMITE"), _
                                                                                     Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                End If
                Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
                Dim estado_resultado As String = ""
                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"), _
                                                                                                                                Session.Item("DG_TIPO_TRAMITE"), _
                                                                                                                                "", _
                                                                                                                                Me.DropDownList_adjunta_documento_automatico, _
                                                                                                                                Me.UpdatePanel_actualiza_adjunta_documento_automatico, _
                                                                                                                                estado_resultado)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.ModalPopupExtender_adjunta_autamatico_documento.Show()
                End If
            Else
                Me.ModalPopupExtender_adjunta_autamatico_documento.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_tool_activa_sube_documento_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("ID_TAREA_SELECCIONDA") = -1 Or Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_USUARIO") = 0 Then
                clasjava.Showscripman_menu("El usuario no tiene permisos para adjuntar imagenes ", UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Ref_class As New ClassGaProducionDocumental
            Dim Extension_permitida As String = ""
            Result = Ref_class.Solicita_listado_extension_de_archivos_permitidas(Extension_permitida)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Extension_permitida = "" Then
                clasjava.Showscripman_menu("El sistema no registra extensiones permitidas", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            AjaxFileUpload_dowload.AllowedFileTypes = Extension_permitida & ",TIFF,tif,jpg,bmp,pdf"
            AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
            UpdatePanel_descarga.Update()
            Me.DropDownList_adjunta_documento.Items.Clear()
            Me.Update_actualiza_adjunta_documento.Update()
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"), _
                                                                                            Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar la imagen relacionada a la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE, _
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = structure_gabinete_workflow.NOMBRE_GABINETE
            'HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = structure_datos_tarea_workflow.ID_IMAGEN
            '------------------------------------------
            'Verfica lista existencia lista de chequeo
            '------------------------------------------
            Dim estado_lista As String = ""
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"), _
                                                                               estado_lista)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            If estado_lista = "YES" Then
                If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                    Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
                                                                                     Session.Item("DG_TIPO_TRAMITE"), _
                                                                                     Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                End If
                Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
                Dim estado_resultado As String = ""
                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"), _
                                                                                                                                Session.Item("DG_TIPO_TRAMITE"), _
                                                                                                                                "", _
                                                                                                                                Me.DropDownList_adjunta_documento, _
                                                                                                                                Me.Update_actualiza_adjunta_documento, _
                                                                                                                                estado_resultado)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = 1
                    Me.ModalPopupExtender_sube_documento_adjunto.Show()
                End If
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
                UpdatePanel_descarga.Update()
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    
    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
            Session.Item("WF_ERROR_RESPUESTA") = ""
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim scrijava As New Classscrripjava
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/"
            'Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            'Dim exte = Path.GetTempPath() & "\" & e.FileName
            Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "_doc_adjunto_" & e.FileName
            If IO.File.Exists(archivo_donwload) Then
                Kill(archivo_donwload)
            End If
            Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
            Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
        Catch ex As Exception
            Session.Item("WF_ERROR_RESPUESTA") = ex.Message

        End Try
    End Sub
    Private Sub ImageButtonimprimir_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonimprimir.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            If HttpContext.Current.Session.Item("WF_TAGSELECCION") = "" Then
                scri.Showscripman("Debe seleccionar un documento ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Tag_Seleccion() As String = HttpContext.Current.Session.Item("WF_TAGSELECCION").ToString.Split("|")
            Dim gabinete_consulta As String = Tag_Seleccion(5)
            Dim id_imagen As Integer = Tag_Seleccion(2)
            Result = refgabinete.SolicitaPermisosGeneralesGabinete(gabinete_consulta,
                                                                   HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                   stru_permiso)
            If Result <> "YES" Then
                scri.Showscripman("Imposible imprimir " & Result, Me.Updatepanel_boton_content)
                Exit Sub
            End If
            If stru_permiso.IMPRI_IMAGEN = 0 Then
                scri.Showscripman("El usuario no tiene permisos para imprimir en el gabinete ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
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
                scri.Showscripman("Imposible consultar imprimir la matriz de documentos esta vacia ", Me.Updatepanel_boton_content)
                Exit Sub
            End If

            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
            Session.Item("DA_GABINETE_IMPRESION") = gabinete_consulta
            Session.Item("DA_ID_IMAGEN_IMPRESION") = id_imagen
            Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL") = Matri_Doc_Visual(HttpContext.Current.Session.Item("WF_DOC_ACTUAL"))
            Me.ModalPopupExtenderimpre_post.Show()
            UpdatePaneliframe_post.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, Updatepanel_boton_content)
        End Try
    End Sub
    Protected Sub ImageButtonguardardocumento_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonguardardocumento.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            If HttpContext.Current.Session.Item("WF_TAGSELECCION") = "" Then
                scri.Showscripman("Debe seleccionar un documento ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Tag_Seleccion() As String = HttpContext.Current.Session.Item("WF_TAGSELECCION").ToString.Split("|")
            Dim gabinete_consulta As String = Tag_Seleccion(5)
            Dim id_imagen As Integer = Tag_Seleccion(2)
            Result = refgabinete.SolicitaPermisosGeneralesGabinete(gabinete_consulta,
                                                                   HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                   stru_permiso)
            If Result <> "YES" Then
                scri.Showscripman("Imposible guardar " & Result, Me.Updatepanel_boton_content)
                Exit Sub
            End If
            If stru_permiso.GUARDAR_IMAGEN = 0 Then
                scri.Showscripman("El usuario no tiene permisos para guardar imagenes desde gabinete ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
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
                scri.Showscripman("Imposible consultar descargar la matriz de documentos esta vacia ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If

            Session.Item("DA_GABINETE_IMPRESION") = gabinete_consulta
            Session.Item("DA_ID_IMAGEN_IMPRESION") = id_imagen
            Iframe_guardar.Attributes.Add("src", "../Docuarchi/WebFormDaExportArchivo.aspx")
            Me.ModalPopupExtender_guardar.Show()
            UpdatePane_guardar.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, Updatepanel_boton_content)
        End Try

    End Sub
    Private Sub ImageButtoninfo_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtoninfo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassConsultaGabinete
            Dim Matri() As String
            Erase Matri
            If HttpContext.Current.Session.Item("WF_TAGSELECCION") = "" Then
                clasjava.Showscripman("Debe seleccionar un documento ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Tag_Seleccion() As String = HttpContext.Current.Session.Item("WF_TAGSELECCION").ToString.Split("|")
            Dim gabinete_consulta As String = Tag_Seleccion(5)
            Dim id_imagen As Integer = Tag_Seleccion(2)
            Dim Result As String = Refclas.Retorna_datos_sistema_imagen_info(id_imagen, _
                                                                             gabinete_consulta, _
                                                                             Matri)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
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
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    
    Private Sub data_grid_chequeo_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_chequeo.RowCreated
        e.Row.Cells(0).Visible = False
    End Sub

    Protected Sub Button_Actualizar_Lista_chequeo_Click(sender As Object, e As EventArgs) Handles Button_Actualizar_Lista_chequeo.Click
        Dim refclas_java As New Classscrripjava
        Try
            Dim Refclas As New ra_dig_tipos_docum_lista_chequeo
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Or Session.Item("DG_TIPODIGITALIZACION") = "PREDETERMINADO" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
                                                                                 Session.Item("DG_TIPO_TRAMITE"), _
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
            End If
            Dim estado_resultado As String = ""
            If Result = "YES" Then
                Result = Refclas.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta(Session.Item("DG_ID_TRAMITE"), _
                                                                                                               Session.Item("DG_TIPO_TRAMITE"), Me.Page, _
                                                                                                               estado_resultado)
            End If
            Hidden_0001.Value = "-1"
            If Result <> "YES" Then
                Me.data_grid_chequeo.DataSource = Nothing
                Me.data_grid_chequeo.DataBind()
                Hidden_0002.Value = "0"
                Label_estado_lista_chequeo.Text = Result
                UpdateGeneral.Update()
            Else
                Hidden_0002.Value = "1"
            End If
        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, UpdatePanel_lista_chequeo)
        Finally
            Me.UpdatePanel_lista_chequeo_estado.Update()
        End Try
    End Sub

    
End Class