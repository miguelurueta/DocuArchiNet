Imports Microsoft.VisualBasic
Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports System.Drawing.Imaging
'Imports FreeImageAPI
Imports Neodynamic
Imports System.IO
Imports System.Net
Imports System.Data.OleDb
Imports Neodynamic.WebControls.ImageDraw
Imports GestionDocumental_Docuarchi.net.ClassConsultaGabinete

Public Class ClassWorflowVisor
    Function Lista_documento_visor_clasificacion(ByRef Matri_Doc_Visual() As String, _
                                                 ByRef LabelConteo As TextBox, _
                                                 ByRef Doc_actual As String, _
                                                 ByRef Hidden_tipo_visor_externo As Object, _
                                                 ByRef pag As Page, _
                                                 ByRef ifrm_visor As Object, _
                                                 ByRef DropDownList_zom As DropDownList, _
                                                 ByRef UpdatePanelButon As UpdatePanel) As String

        Try
            Dim spli_sel() As String = HttpContext.Current.Session.Item("CC_SESIONITERCAMBIOVISOR").ToString.Split("|")
            Dim Result As String = ""
            '---------------------------------------------
            'Retorna datos del documento
            '---------------------------------------------
            Dim Refclas_workflow_visor As New ClassWorflowVisor
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim spli_datos() As Datos_Registro = Nothing
            Result = ClassDaGabinete.Solicita_Datos_Documentos(Val(spli_sel(0)),
                                                               spli_sel(1),
                                                               spli_datos)
            If Result <> "YES" Then
                Lista_documento_visor_clasificacion = Result
                Exit Function
            Else
                If spli_datos Is Nothing Then
                    Lista_documento_visor_clasificacion = "Imposible encontrar datos de la imagen en el gabinete "
                    Exit Function
                End If
            End If
            Dim Class_logdocuarchi As New Class_logdocuarchi
            Dim id_imagen As Long = Val(spli_sel(0))
            Dim nombre_gabinete As String = spli_sel(1)
            Dim visor As String = ""
            Dim Extesnsion As String = ""
            Dim Estado_documento As String = ""
            Dim ref_Class_da_extension As New Class_da_extension
            Result = ref_Class_da_extension.Determina_tipo_documento_list(Val(spli_datos(0).dbt), _
                                                                          visor, _
                                                                          Extesnsion, _
                                                                          Estado_documento)
            If Result <> "YES" Then
                Lista_documento_visor_clasificacion = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("CC_MATRI_IMAGE_EMERGENTE") = ""
            HttpContext.Current.Session.Item("CC_TAGSELECCION_EMERGENTE") = "-|-|" & spli_sel(0) & "|" & Extesnsion & "|NORMAL|" & spli_sel(1)
            If Extesnsion = ".TIF" Or Extesnsion = ".JPG" Or Extesnsion = ".BMP" Then
                HttpContext.Current.Session.Item("CC_ID_DOCUMENTO_SELECCIONADO_EMERGENTE") = spli_sel(0)
                Dim Refclas As New ClassWorflowVisor
                HttpContext.Current.Session.Item("CC_IMAGE_TEMPORAL_EMERGENTE") = ""
                HttpContext.Current.Session.Item("CC_MATRI_IMAGE_EMERGENTE") = ""
                HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE") = "1"
                Result = Refclas.Visualizacion_Documentos(Matri_Doc_Visual, HttpContext.Current.Session("CC_TAGSELECCION_EMERGENTE"), HttpContext.Current.Session("CC_PAGINASELECCION_EMERGENTE"), HttpContext.Current.Session.Item("CC_ID_DOCUMENTO_SELECCIONADO_EMERGENTE"))
                If Result = "YES" Then
                    If Not Matri_Doc_Visual Is Nothing Then
                        For i As Integer = 0 To Matri_Doc_Visual.Length - 1
                            HttpContext.Current.Session.Item("CC_MATRI_IMAGE_EMERGENTE") = HttpContext.Current.Session.Item("CC_MATRI_IMAGE_EMERGENTE") & Matri_Doc_Visual(i) & "|"
                        Next
                        LabelConteo.Text = "1/" & Matri_Doc_Visual.Length
                        Result = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, _
                                                                 Doc_actual, _
                                                                 "inicio", _
                                                                 0, _
                                                                 pag, _
                                                                 HttpContext.Current.Session.Item("CC_DOC_ACTUAL_EMERGENTE"), _
                                                                 HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_EMERGENTE"), _
                                                                 HttpContext.Current.Session.Item("CC_IMAGE_WITH_EMERGENTE"), _
                                                                 HttpContext.Current.Session.Item("CC_IMAGE_HEIHG_SIZE_EMERGENTE"), _
                                                                 HttpContext.Current.Session.Item("CC_IMAGE_WITH_SIZE_EMERGENTE"), _
                                                                 DropDownList_zom, _
                                                                 UpdatePanelButon)
                        If Result <> "YES" Then
                            Lista_documento_visor_clasificacion = Result
                            Exit Function
                        End If
                        Hidden_tipo_visor_externo.Value = "0"
                        Class_logdocuarchi.Registra_log_procesing_image(id_imagen, nombre_gabinete, "VISORAPROBACION", "Visualiza", 0, "", "")
                    End If
                End If
            Else
                Dim refclas As New ClassVisualisaDocumento
                Dim matri_doc() As String = Nothing
                Dim Resutl = refclas.Genera_Matris_Documentos_Almacenados(Val(spli_sel(0)), spli_sel(1), matri_doc)
                If Resutl = "YES" Then
                    Dim fielinf As New FileInfo(matri_doc(1))
                    If UCase(fielinf.Extension) = ".PDF" Then
                        Select Case HttpContext.Current.Session.Item("TIPO_VISOR_PDF")
                            Case "PDF-JS"
                                Dim printer As Integer = 1
                                Dim save As Integer = 1
                                Dim stamp As Integer = 1
                                Dim firma As Integer = 1
                                ifrm_visor.Attributes("SRC") = "../pdfjs/pdf_anotate_view/index.html?rut_image=" & matri_doc(1) & "&urimage_format=" & matri_doc(1).Replace("\", "|") & "&url_firma=" &
                                "../" & HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL") & "&" & "ash=../../workflow/Handler_image_wf.ashx" &
                                "&url_id_imagen=" & id_imagen & "&url_cabinete_imagen=" & nombre_gabinete &
                                "&url_radicado=" & "" & "&url_id_workflow=" & 0 & "&url_desc_transacion=" & "VISORAPROBACION" &
                                "&url_printer=" & printer & "&url_save=" & save &
                                "&url_add_firma=" & firma & "&url_add_stamp=" & stamp

                                Hidden_tipo_visor_externo.Value = "1"
                            Case "LIBRE_PDF"
                                ifrm_visor.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & matri_doc(1)
                                Hidden_tipo_visor_externo.Value = "1"
                            Case Else

                                ifrm_visor.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & matri_doc(1)
                                Hidden_tipo_visor_externo.Value = "1"
                        End Select
                        Class_logdocuarchi.Registra_log_procesing_image(id_imagen, nombre_gabinete, "VISORAPROBACION", "Visualiza", 0, "", "")
                        Lista_documento_visor_clasificacion = "YES"
                        Exit Function
                    Else
                        ifrm_visor.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & matri_doc(1)
                        Hidden_tipo_visor_externo.Value = "1"
                        Class_logdocuarchi.Registra_log_procesing_image(id_imagen, nombre_gabinete, "VISORAPROBACION", "Visualiza", 0, "", "")
                        Lista_documento_visor_clasificacion = "YES"
                        Exit Function
                    End If
                Else
                    Dim Resutla = Me.Limpia_Visor_emergente_Workflow(pag, "noaming")
                    If Resutla <> "YES" Then
                        Lista_documento_visor_clasificacion = Resutla
                        Exit Function
                    End If
                    Lista_documento_visor_clasificacion = Resutl
                    Exit Function
                End If
            End If

        Catch ex As Exception
            Lista_documento_visor_clasificacion = "Inconsistencia general función Lista_documento_visor_clasificacion " & ex.Message
        End Try
    End Function
    Function Selecion_treiew_documento_visor_workflow_externo(ByRef Matri_Doc_Visual() As String,
                                                              ByRef LabelConteo As String,
                                                              ByRef Doc_actual As String,
                                                              ByRef Hidden_tipo_visor_externo As Object,
                                                              ByRef pag As Page,
                                                              ByRef ifrm_visor As Object,
                                                              ByVal selecion_intercambio As String,
                                                              ByRef update_iframe_visor As UpdatePanel,
                                                              ByRef up_date_buton As UpdatePanel,
                                                              ByRef DropDownList_zom As DropDownList,
                                                              ByRef UpdatePanelButon As UpdatePanel) As String

        Try
            Dim spli_sel() As String = selecion_intercambio.ToString.Split("|")
            Dim Result As String = ""
            '---------------------------------------------
            'Retorna datos del documento
            '---------------------------------------------
            Dim Refclas_workflow_visor As New ClassWorflowVisor
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim spli_datos() As Datos_Registro = Nothing
            Result = ClassDaGabinete.Solicita_Datos_Documentos(Val(spli_sel(2)),
                                                                      spli_sel(5),
                                                                      spli_datos)
            If Result <> "YES" Then
                Selecion_treiew_documento_visor_workflow_externo = Result
                Exit Function
            Else
                If spli_datos Is Nothing Then
                    Selecion_treiew_documento_visor_workflow_externo = "Imposible encontrar datos de la imagen en el gabinete "
                    Exit Function
                End If
            End If
            Dim Class_logdocuarchi As New Class_logdocuarchi
            Dim id_imagen As Long = Val(spli_sel(0))
            Dim nombre_gabinete As String = spli_sel(1)
            Dim visor As String = ""
            Dim Extesnsion As String = ""
            Dim Estado_documento As String = ""
            Dim ref_Class_da_extension As New Class_da_extension
            Result = ref_Class_da_extension.Determina_tipo_documento_list(Val(spli_datos(0).dbt),
                                                                          visor,
                                                                          Extesnsion,
                                                                          Estado_documento)
            If Result <> "YES" Then
                Selecion_treiew_documento_visor_workflow_externo = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("WF_MATRI_IMAGE_EMERGENTE") = ""
            HttpContext.Current.Session.Item("WF_TAGSELECCION_EMERGENTE") = selecion_intercambio
            If Extesnsion = ".TIF" Or Extesnsion = ".JPG" Or Extesnsion = ".BMP" Then
                HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_EMERGENTE") = spli_sel(2)
                Dim Refclas As New ClassWorflowVisor
                HttpContext.Current.Session.Item("WF_IMAGE_TEMPORAL_EMERGENTE") = ""
                HttpContext.Current.Session.Item("WF_MATRI_IMAGE_EMERGENTE") = ""
                HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE") = "1"
                Result = Refclas.Visualizacion_Documentos(Matri_Doc_Visual,
                                                          HttpContext.Current.Session("WF_TAGSELECCION_EMERGENTE"),
                                                          HttpContext.Current.Session("WF_PAGINASELECCION_EMERGENTE"),
                                                          HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_EMERGENTE"))
                If Result = "YES" Then
                    If Not Matri_Doc_Visual Is Nothing Then
                        For i As Integer = 0 To Matri_Doc_Visual.Length - 1
                            HttpContext.Current.Session.Item("WF_MATRI_IMAGE_EMERGENTE") = HttpContext.Current.Session.Item("WF_MATRI_IMAGE_EMERGENTE") & Matri_Doc_Visual(i) & "|"
                        Next
                        LabelConteo = "1/" & Matri_Doc_Visual.Length - 1
                        up_date_buton.Update()
                        '            UpdatePanelButon.Update()
                        Result = Me.Muestra_Documento_Visor(Matri_Doc_Visual, Doc_actual,
                                                            "inicio",
                                                            0,
                                                            pag,
                                                            HttpContext.Current.Session.Item("WF_DOC_ACTUAL_EMERGENTE"),
                                                            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_EMERGENTE"),
                                                            HttpContext.Current.Session.Item("WF_IMAGE_WITH_EMERGENTE"),
                                                            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE_EMERGENTE"),
                                                            HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE_EMERGENTE"),
                                                            DropDownList_zom,
                                                            UpdatePanelButon)

                        If Result <> "YES" Then
                            Selecion_treiew_documento_visor_workflow_externo = Result
                            Exit Function
                        End If
                        ifrm_visor.Attributes("SRC") = ""
                        Hidden_tipo_visor_externo.Value = "0"
                        update_iframe_visor.Update()
                    End If
                End If
                Class_logdocuarchi.Registra_log_procesing_image(id_imagen, nombre_gabinete, "VISOREXTERNO", "Visualiza", 0, "", "")
                Selecion_treiew_documento_visor_workflow_externo = "YES"
                Exit Function
            Else
                Dim refclas As New ClassVisualisaDocumento
                Dim matri_doc() As String = Nothing
                Dim Resutl = refclas.Genera_Matris_Documentos_Almacenados(Val(spli_sel(2)),
                                                                          spli_sel(5),
                                                                          matri_doc)
                If Resutl = "YES" Then
                    Dim fielinf As New FileInfo(matri_doc(1))
                    If UCase(fielinf.Extension) = ".PDF" Then
                        Select Case HttpContext.Current.Session.Item("TIPO_VISOR_PDF")
                            Case "PDF-JS"
                                Dim ruta_server As String = "../pdfjs/web/web_file/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "/visual"
                                Dim ruta_server_info As String = "../web/web_file/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "/visual"
                                Dim ruta_file As String = HttpContext.Current.Server.MapPath(ruta_server)
                                If IO.Directory.Exists(ruta_file) = False Then
                                    IO.Directory.CreateDirectory(ruta_file)
                                End If
                                If IO.File.Exists(ruta_file & "\visual_file" & fielinf.Extension) = True Then
                                    Kill(ruta_file & "\visual_file" & fielinf.Extension)
                                End If
                                FileCopy(matri_doc(1), ruta_file & "\visual_file" & fielinf.Extension)
                                ifrm_visor.Attributes("src") = "../pdfjs/web/viewer.html?file=" & ruta_server_info & "/visual_file" & fielinf.Extension
                                Hidden_tipo_visor_externo.Value = "1"
                                update_iframe_visor.Update()
                            Case "LIBRE_PDF"
                                ifrm_visor.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & matri_doc(1)
                                Hidden_tipo_visor_externo.Value = "1"
                                update_iframe_visor.Update()
                            Case Else
                                ifrm_visor.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & matri_doc(1)
                                Hidden_tipo_visor_externo.Value = "1"
                                update_iframe_visor.Update()
                        End Select
                        Class_logdocuarchi.Registra_log_procesing_image(id_imagen, nombre_gabinete, "VISOREXTERNO", "Visualiza", 0, "", "")
                        Selecion_treiew_documento_visor_workflow_externo = "YES"
                        Exit Function
                    Else
                        HttpContext.Current.Session.Item("DA_DESCARGA") = matri_doc(1)
                        ifrm_visor.Attributes("SRC") = "../Docuarchi/WebFormVisorDescarga.aspx"
                        Class_logdocuarchi.Registra_log_procesing_image(id_imagen, nombre_gabinete, "VISOREXTERNO", "Visualiza", 0, "", "")
                        Hidden_tipo_visor_externo.Value = "1"
                        update_iframe_visor.Update()
                        Selecion_treiew_documento_visor_workflow_externo = "YES"
                        Exit Function
                    End If
                Else
                    Dim Resutla = Me.Limpia_Visor_emergente_Workflow(pag, "noaming")
                    If Resutla <> "YES" Then
                        Selecion_treiew_documento_visor_workflow_externo = Resutla
                        Exit Function
                    End If
                    Selecion_treiew_documento_visor_workflow_externo = Resutl
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Selecion_treiew_documento_visor_workflow_externo = "Inconsistencia general función Selecion_treiew_documento_visor_workflow_externo " & ex.Message
        End Try
    End Function
    Function Limpia_Visor_Workflow(ByRef Pag As Page,
                                   ByVal Nombre_Visor As String, Optional limpia_iterface As Integer = 1) As String
        Try
            Dim ref_Panel_tolbar_pdf As Panel = Pag.FindControl("Panel_tolbar_pdf")
            Dim ref_UpdatePanel_panel_toll As UpdatePanel = Pag.FindControl("UpdatePanel_panel_toll")
            Dim UpdatePanel_content_iframe As UpdatePanel = Pag.FindControl("UpdatePanel_content_iframe")
            Dim panel_content_iframe As Panel = Pag.FindControl("panel_content_iframe")
            Dim UpdatePanel_content_image_draw As UpdatePanel = Pag.FindControl("UpdatePanel_content_image_draw")
            Dim panel_content_image_draw As Panel = Pag.FindControl("panel_content_image_draw")
            Dim Result As String = ""
            ref_Panel_tolbar_pdf.Visible = False
            ref_UpdatePanel_panel_toll.Update()
            If Not panel_content_iframe Is Nothing Then
                panel_content_iframe.Visible = False
                UpdatePanel_content_iframe.Update()
            End If
            If Not panel_content_image_draw Is Nothing Then
                panel_content_image_draw.Visible = False
                UpdatePanel_content_image_draw.Update()
            End If
            HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = 0
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = ""
            HttpContext.Current.Session.Item("WF_ID_GABINETE_SELECCIONADO") = ""
            Dim ref_class_selecion As New Classselecciotarea
            Dim frm As Object = Pag.FindControl("ifrm_visor_")
            frm.Attributes("SRC") = "../workflow/WebFormiframe.aspx"
            Dim up As UpdatePanel = Pag.FindControl("UpdatePanelvisor")
            If Not up Is Nothing Then
                up.Update()
            End If
            Dim upindce As UpdatePanel = Pag.FindControl("UpdatePanelindice")
            Dim panel_div_buton As Panel = Pag.FindControl("div_buton")
            If Not panel_div_buton Is Nothing Then
                panel_div_buton.Visible = False
            End If
            If Not upindce Is Nothing Then
                upindce.Update()
            End If
            If limpia_iterface = 1 Then
                Result = ref_class_selecion.Actualiza_interface_estado_flujo_ruta(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                  HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                  HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                  HttpContext.Current.Session("Id_actividad_Workflow"),
                                                                                  Pag)
                If Result <> "YES" Then
                    Limpia_Visor_Workflow = Result
                Else
                    Limpia_Visor_Workflow = "YES"
                End If
            Else
                Limpia_Visor_Workflow = "YES"
            End If

        Catch ex As Exception
            Limpia_Visor_Workflow = "Inconsistencia general funcion Limpia_Visor_Workflow " & ex.Message
        End Try
    End Function
    Public Function Limpia_Visor_emergente_Workflow(ByRef Pag As Page, ByVal Nombre_Visor As String) As String
        Try
            Dim noami As ImageDraw = Pag.FindControl("noaming")
            noami.Elements.Clear()
            HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_EMERGENTE") = ""
            'HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = ""
            HttpContext.Current.Session.Item("WF_TAGSELECCION_EMERGENTE") = ""
            Dim up As UpdatePanel = Pag.FindControl("UpdatePanelvisor")
            up.Update()
            Limpia_Visor_emergente_Workflow = "YES"
        Catch ex As Exception
            Limpia_Visor_emergente_Workflow = "Inconsistencia general funcion Limpia_Visor_Workflow " & ex.Message
        End Try
    End Function
    
    Function Visualizacion_Documentos(ByRef MatriDocRef() As String, _
                                      ByRef WF_TAGSELECCION As String, _
                                      ByRef WF_PAGINASELECCION As String, _
                                      ByRef WF_ID_DOCUMENTO_SELECCIONADO As Object) As String
        Dim Resultado As String = ""
        Dim ite As New Object
        Dim sel As String = WF_TAGSELECCION
        Dim Result As String = ""
        Dim Tag_Seleccion() As String
        Erase Tag_Seleccion
        Dim Matri_Img_Temp() As String
        Erase Matri_Img_Temp
        Try
            Tag_Seleccion = sel.Split("|")
            If Tag_Seleccion Is Nothing Then
                Visualizacion_Documentos = "Imagen sin datos adjuntos"
                Exit Function
            End If
            WF_PAGINASELECCION = "-1"
            '*********************************
            'monta tolbar dot image anotacion
            '*********************************
            Dim Matri_dat_gabi() As String
            Erase Matri_dat_gabi
            If Tag_Seleccion(3) = ".TIF" Or Tag_Seleccion(3) = ".JPG" Or Tag_Seleccion(3) = ".BMP" Then
                Dim ClassDaGabinete As New ClassDaGabinete
                Resultado = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(Tag_Seleccion(2),
                                                                                            Tag_Seleccion(5),
                                                                                            Matri_Img_Temp)
                If Resultado <> "YES" Then

                    Visualizacion_Documentos = Resultado
                    Exit Function
                End If
                If Matri_Img_Temp Is Nothing Then

                    Visualizacion_Documentos = "Imposible encontrar matris documentos"
                    Exit Function
                End If
                Erase MatriDocRef
                For i2 As Integer = 0 To UBound(Matri_Img_Temp)
                    ReDim Preserve MatriDocRef(i2)
                    MatriDocRef(i2) = Matri_Img_Temp(i2)
                Next
            End If
            WF_ID_DOCUMENTO_SELECCIONADO = Val(Tag_Seleccion(2))
            Visualizacion_Documentos = "YES"
            Exit Function
        Catch ex As Exception
            Visualizacion_Documentos = "Inconsistencia general funcion Visualizacion_Documentos " & ex.Message
        End Try
    End Function
    Function Visualizacion_Documentos_da(ByRef MatriDocRef() As String, _
                                         ByVal id_documento As Integer, _
                                         ByRef nombre_gabinete As String) As String
        Dim Resultado As String = ""
        Dim Matri_Img_Temp() As String
        Erase Matri_Img_Temp
        Try

            Dim Matri_dat_gabi() As String
            Erase Matri_dat_gabi
            Dim ClassDaGabinete As New ClassDaGabinete
            Resultado = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_documento,
                                                                                        nombre_gabinete,
                                                                                        Matri_Img_Temp)
            If Resultado <> "YES" Then
                Visualizacion_Documentos_da = Resultado
                Exit Function
            End If
            If Matri_Img_Temp Is Nothing Then
                Visualizacion_Documentos_da = "Imposible encontrar matris documentos"
                Exit Function
            End If
            Erase MatriDocRef
            For i2 As Integer = 0 To UBound(Matri_Img_Temp)
                ReDim Preserve MatriDocRef(i2)
                MatriDocRef(i2) = Matri_Img_Temp(i2)
            Next
            Visualizacion_Documentos_da = "YES"
            Exit Function
        Catch ex As Exception
            Visualizacion_Documentos_da = "Inconsistencia general funcion Visualizacion_Documentos_da " & ex.Message
        End Try
    End Function

    Function Muestra_Documento_Visor_Escale(ByRef Matri_Doc_Visual() As String, _
                                            ByVal Escala As String, _
                                            ByRef pag As Page, _
                                            ByRef WF_DOC_ACTUAL As Object, _
                                            ByRef WF_IMAGE_HEIHG As Object, _
                                            ByRef WF_IMAGE_WITH As Object, _
                                            ByRef drop_list As DropDownList, _
                                            ByRef up_date As UpdatePanel) As String
        Try
            Dim EscaleHeigt As Integer = 0
            Dim EscaleWidth As Integer = 0
            If Escala = "+" Then
                If WF_IMAGE_HEIHG < 20 Or WF_IMAGE_HEIHG >= 100 Then
                    Muestra_Documento_Visor_Escale = "YES"
                    Exit Function
                End If
            End If
            If Escala = "-" Then
                If WF_IMAGE_HEIHG <= 20 Or WF_IMAGE_HEIHG > 100 Then
                    Muestra_Documento_Visor_Escale = "YES"
                    Exit Function
                End If
            End If
            Dim Doc_Actual As Integer = WF_DOC_ACTUAL
            If HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF") = 0 Then
                HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF") = WF_IMAGE_HEIHG
            Else
                WF_IMAGE_HEIHG = HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF")
                WF_IMAGE_WITH = HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF")
            End If
            If Escala = "+" Then
                
                EscaleHeigt = WF_IMAGE_HEIHG + 10
                EscaleWidth = WF_IMAGE_WITH + 10
            End If
            If Escala = "-" Then
                EscaleHeigt = WF_IMAGE_HEIHG - 10
                EscaleWidth = WF_IMAGE_WITH - 10
            End If
            If Escala = "x" Then
                EscaleHeigt = 30
                EscaleWidth = 30
            End If
            HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF") = EscaleHeigt
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            Dim noami As ImageDraw = pag.FindControl("noaming")
            Dim actio As Object = noami.Elements(0).Actions(0)
            actio.HeightPercentage = EscaleHeigt
            actio.WidthPercentage = EscaleWidth
            WF_IMAGE_HEIHG = EscaleHeigt
            WF_IMAGE_WITH = EscaleWidth
            Dim up As UpdatePanel = pag.FindControl("UpdatePanelvisor")
            noami.Attributes.Add("zon_heig", actio.HeightPercentage)
            noami.Attributes.Add("zon_with", actio.WidthPercentage)
            For i As Integer = 0 To drop_list.Items.Count - 1
                If Val(drop_list.Items(i).Value) = WF_IMAGE_WITH Then
                    drop_list.Items(i).Selected = True
                    drop_list.Text = WF_IMAGE_WITH
                    Exit For
                End If
            Next
            Dim UpdatePanel_drows_bot As UpdatePanel = pag.FindControl("UpdatePanel_drows_bot")
            If Not UpdatePanel_drows_bot Is Nothing Then
                UpdatePanel_drows_bot.Update()
            End If
            Dim UpdatePanel_noaming As UpdatePanel = pag.FindControl("UpdatePanel_noaming")
            If Not UpdatePanel_noaming Is Nothing Then
                UpdatePanel_noaming.Update()
            End If
            up_date.Update()
            If Not up Is Nothing Then
                up.Update()
            End If
            Muestra_Documento_Visor_Escale = "YES"
        Catch ex As Exception
            Muestra_Documento_Visor_Escale = "Funcion Muestra_Documento_Visor_Escale " & ex.Message
        End Try
    End Function
    Function Muestra_Documento_Visor_Escale_zom(ByRef Matri_Doc_Visual() As String, _
                                                ByVal Escala As Integer, _
                                                ByRef pag As Page, _
                                                ByRef WF_DOC_ACTUAL As Object, _
                                                ByRef WF_IMAGE_HEIHG As Object, _
                                                ByRef WF_IMAGE_WITH As Object, _
                                                ByRef drop_list As DropDownList, _
                                                ByRef up_date As UpdatePanel) As String
        Try
            HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF") = Escala
            Dim EscaleHeigt As Integer = Escala
            Dim EscaleWidth As Integer = Escala
            ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
            ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
            Dim noami As ImageDraw = pag.FindControl("noaming")
            Dim actio As Object = noami.Elements(0).Actions(0)
            actio.HeightPercentage = EscaleHeigt
            actio.WidthPercentage = EscaleWidth
            WF_IMAGE_HEIHG = EscaleHeigt
            WF_IMAGE_WITH = EscaleWidth
            Dim lab As Label = pag.FindControl("Labeldatos")
            Dim up As UpdatePanel = pag.FindControl("UpdatePanelvisor")
            noami.Attributes.Add("zon_heig", actio.HeightPercentage)
            noami.Attributes.Add("zon_with", actio.WidthPercentage)
            For i As Integer = 0 To drop_list.Items.Count - 1
                If drop_list.Items(i).Value = actio.HeightPercentage Then
                    drop_list.Items(i).Selected = True
                    drop_list.Text = actio.HeightPercentage.ToString
                    Exit For
                End If
            Next
            up_date.Update()
            up.Update()
            Dim UpdatePanel_noaming As UpdatePanel = pag.FindControl("UpdatePanel_noaming")
            If Not UpdatePanel_noaming Is Nothing Then
                UpdatePanel_noaming.Update()
            End If
            Muestra_Documento_Visor_Escale_zom = "YES"
        Catch ex As Exception
            Muestra_Documento_Visor_Escale_zom = "Funcion Muestra_Documento_Visor_Escale_zom " & ex.Message
        End Try
    End Function
    Function Muestra_Documento_Visor_Rotate(ByRef Matri_Doc_Visual() As String, _
                                            ByVal Rotacion As Integer, _
                                            ByRef pag As Page, _
                                            ByRef WF_DOC_ACTUAL As Object, _
                                            ByRef WF_IMAGE_HEIHG As Object, _
                                            ByRef WF_IMAGE_WITH As Object) As String
        Try

            Dim Doc_Actual As Integer = WF_DOC_ACTUAL
            Dim noami As ImageDraw = pag.FindControl("noaming")
            Dim actio As Object = noami.Elements(0).Actions(0)
            actio.HeightPercentage = WF_IMAGE_HEIHG
            actio.WidthPercentage = WF_IMAGE_WITH
            Dim rotate As New Neodynamic.WebControls.ImageDraw.Rotate()
            rotate.Angle = Rotacion
            noami.Elements(0).Actions.Add(rotate)
            Dim up As UpdatePanel = pag.FindControl("UpdatePanelvisor")
            up.Update()
            Dim UpdatePanel_noaming As UpdatePanel = pag.FindControl("UpdatePanel_noaming")
            If Not UpdatePanel_noaming Is Nothing Then
                UpdatePanel_noaming.Update()
            End If
            Muestra_Documento_Visor_Rotate = "YES"
        Catch ex As Exception
            Muestra_Documento_Visor_Rotate = "Función Muestra_Documento_Visor_Rotate " & ex.Message
        End Try
    End Function

    Function Muestra_Documento_Visor(ByRef Matri_Doc_Visual() As String,
                                     ByRef Doc_actual1 As String,
                                     ByVal Naveg As String,
                                     ByVal Pag_Selec As Integer,
                                     ByRef pag As Page,
                                     ByRef WF_DOC_ACTUAL As Object,
                                     ByRef WF_IMAGE_HEIHG As Object,
                                     ByRef WF_IMAGE_WITH As Object,
                                     ByRef WF_IMAGE_HEIHG_SIZE As Object,
                                     ByRef WF_IMAGE_WITH_SIZE As Object,
                                     ByRef drop_list As DropDownList,
                                     ByRef up_date As UpdatePanel) As String
        '***************************************************
        'Funcion : Muestra_Documento_Visor
        'Fecha : 25-02-2011
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Utiliza la matriz documentos
        'seleccionados y el numero de imagen a visualizar
        '***************************************************
        Try
            Dim Doc_Actual As Integer = Val(WF_DOC_ACTUAL)
            Select Case Naveg
                Case "+1"
                    If Matri_Doc_Visual.Length - 1 = Doc_Actual Then
                        Muestra_Documento_Visor = "YES"
                        Exit Function
                    Else
                        Doc_Actual = Doc_Actual + 1
                    End If
                Case "-1"
                    If Doc_Actual = 1 Then
                        Muestra_Documento_Visor = "YES"
                        Exit Function
                    Else
                        Doc_Actual = Doc_Actual - 1
                    End If
                Case "inicio"
                    Doc_Actual = 1
                Case "final"
                    Doc_Actual = Matri_Doc_Visual.Length - 1
                Case "seleccion"
                    If Pag_Selec > Matri_Doc_Visual.Length - 1 Then
                        Muestra_Documento_Visor = "YES"
                        Exit Function
                    End If
                    If Pag_Selec < 1 Then
                        Muestra_Documento_Visor = "YES"
                        Exit Function
                    End If
                    Doc_Actual = Pag_Selec
            End Select
            WF_DOC_ACTUAL = Doc_Actual
            Dim noami As Object = pag.FindControl("noaming")
            If noami Is Nothing Then
                Muestra_Documento_Visor = "Imposible encontrar noami"
                Exit Function
            End If
            Dim imgElem As New Neodynamic.WebControls.ImageDraw.ImageElement
            If noami.Elements.Count > 0 Then
                Dim elemen As ImageElement = noami.Elements(0)
                elemen.SourceFile = Matri_Doc_Visual(Doc_Actual)
                Dim actio As Object = noami.Elements(0).Actions(0)
                If HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF") <> 0 Then
                    actio.HeightPercentage = HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF")
                    actio.WidthPercentage = HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF")
                Else
                    HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF") = actio.HeightPercentage
                End If
                noami.Attributes.Add("zon_heig", actio.HeightPercentage)
                noami.Attributes.Add("zon_with", actio.WidthPercentage)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = actio.HeightPercentage Then
                        drop_list.Items(i).Selected = True
                        drop_list.Text = actio.HeightPercentage.ToString
                        Exit For
                    End If
                Next
                HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF") = actio.HeightPercentage.ToString
                up_date.Update()
            Else
                imgElem.SourceFile = Matri_Doc_Visual(Doc_Actual)
                Dim rotate As New Neodynamic.WebControls.ImageDraw.Scale
                If HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF") <> 0 Then
                    WF_IMAGE_HEIHG = HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF")
                    WF_IMAGE_WITH = HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF")
                Else
                    WF_IMAGE_HEIHG = 50
                    WF_IMAGE_WITH = 50
                    HttpContext.Current.Session.Item("ZOON_VISOR_WEB_TIF") = WF_IMAGE_HEIHG
                End If
                rotate.HeightPercentage = WF_IMAGE_HEIHG
                rotate.WidthPercentage = WF_IMAGE_WITH
                imgElem.Actions.Add(rotate)
                noami.Elements.Add(imgElem)
                Dim actio As Object = noami.Elements(0).Actions(0)
                noami.Attributes.Add("zon_heig", actio.HeightPercentage)
                noami.Attributes.Add("zon_with", actio.WidthPercentage)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = actio.HeightPercentage Then
                        drop_list.Items(i).Selected = True
                        drop_list.Text = actio.HeightPercentage.ToString
                        Exit For
                    End If
                Next
                WF_IMAGE_HEIHG_SIZE = actio.HeightPercentage
                WF_IMAGE_WITH_SIZE = actio.WidthPercentage
                up_date.Update()
            End If
            Dim lab As TextBox = pag.FindControl("LabelConteo")
            If Not lab Is Nothing Then
                lab.Text = Doc_Actual & "/" & Matri_Doc_Visual.Length - 1
            End If
            Dim up As UpdatePanel = pag.FindControl("UpdatePanelvisor")
            If Not up Is Nothing Then
                up.Update()
            End If
            Dim UpdatePanel_conte_bot As Object = pag.FindControl("UpdatePanel_conte_bot")
            If Not UpdatePanel_conte_bot Is Nothing Then
                UpdatePanel_conte_bot.Update()
            End If
            Dim UpdatePanel_noaming As UpdatePanel = pag.FindControl("UpdatePanel_noaming")
            If Not UpdatePanel_noaming Is Nothing Then
                UpdatePanel_noaming.Update()
            End If
            Muestra_Documento_Visor = "YES"
        Catch ex As Exception
            Muestra_Documento_Visor = "Error General Funcion : Muestra_Documento_Visor Descrip Error :" & ex.Message
        End Try
    End Function

    Function Consulta_Documentos_Añadidos(ByVal Id_Documento As Integer,
                                          ByVal Nombre_Gabinete As String,
                                          ByRef Matri_Datos() As Datos_Registro,
                                          ByVal User_A As String) As String
        '****************************************************************
        'Funcion : consulta los documentos añadidos de un documento
        'con los datos nombre gabiente, id_documento genera una matriz
        'de documentos
        'Fecha 2013-05-20
        'Ingeniero : Miguel Angel Urueta Miranda
        '****************************************************************
        Try
            Dim Parametro_Consulta As String = "select ID,DISC,PAG,IDEX,DBT  from " & Nombre_Gabinete &
                       " where dbt='" & Id_Documento & "' And USER='" & User_A & "' order by id"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_TAREA")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Consulta_Documentos_Añadidos = "Funcion  Consulta_Documentos_Añadidos WF-01 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Consulta_Documentos_Añadidos = "YES"
                Exit Function
            Else
                For z As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(z)
                    Matri_Datos(z).Id = Datset.Tables(0).Rows(z).Item(0).ToString
                    Matri_Datos(z).Disc = Datset.Tables(0).Rows(z).Item(1).ToString
                    Matri_Datos(z).Pag = Datset.Tables(0).Rows(z).Item(2).ToString
                    Matri_Datos(z).idex = Datset.Tables(0).Rows(z).Item(3).ToString
                    Matri_Datos(z).dbt = Datset.Tables(0).Rows(z).Item(4).ToString
                Next
                Consulta_Documentos_Añadidos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_Documentos_Añadidos = "funcion Consulta_Documentos_Añadidos (b) dice " & ex.Message
        End Try
    End Function
    Function inicializa_visor_documento_workflow_neodinamyc(ByRef Matri_Doc_Visual() As String,
                                                            ByRef Doc_actual As String,
                                                            ByRef page As Page) As String
        Try
            Dim DropDownList_zom As DropDownList = page.FindControl("DropDownList_zom")
            Dim UpdatePanel_drows_bot As UpdatePanel = page.FindControl("UpdatePanel_drows_bot")
            If DropDownList_zom Is Nothing Then
                inicializa_visor_documento_workflow_neodinamyc = "Imposible encontrar el control (DropDownList_zom)"
                Exit Function
            End If
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = ""
            HttpContext.Current.Session.Item("WF_IMAGE_TEMPORAL") = ""
            HttpContext.Current.Session.Item("WF_DOC_ACTUAL") = "1"
            HttpContext.Current.Session.Item("WF_MATRI_IMAGE") = ""
            Result = Refclas.Visualizacion_Documentos(Matri_Doc_Visual,
                                                      HttpContext.Current.Session("WF_TAGSELECCION"),
                                                      HttpContext.Current.Session("WF_PAGINASELECCION"),
                                                      HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"))
            If Result = "YES" Then
                If Not Matri_Doc_Visual Is Nothing Then
                    For i As Integer = 0 To Matri_Doc_Visual.Length - 1
                        HttpContext.Current.Session.Item("WF_MATRI_IMAGE") = HttpContext.Current.Session.Item("WF_MATRI_IMAGE") & Matri_Doc_Visual(i) & "|"
                    Next
                    Result = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual,
                                                             Doc_actual,
                                                             "inicio",
                                                             0,
                                                             page,
                                                             HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                             HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                             HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                             HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                             HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                             DropDownList_zom,
                                                             UpdatePanel_drows_bot)
                    If Result <> "YES" Then
                        inicializa_visor_documento_workflow_neodinamyc = Result
                        Exit Function
                    End If
                End If
                inicializa_visor_documento_workflow_neodinamyc = "YES"
                Exit Function
            Else
                inicializa_visor_documento_workflow_neodinamyc = Result
                Exit Function
            End If
        Catch ex As Exception
            inicializa_visor_documento_workflow_neodinamyc = "Inconsistencia general función"
        End Try
    End Function
End Class
