Public Structure Datos_Registro
    Dim Id As String
    Dim Disc As String
    Dim Pag As String
    Dim idex As String
    Dim dbt As String
End Structure
Public Class ClassVisualisaDocumento
    Function Visualiza_documento_workflow_visor(ByVal id_seleccion As String,
                                                ByRef ifrm_visor As Object,
                                                ByRef Panel_indice As Panel,
                                                ByRef UpdatePanelindice As Object,
                                                ByRef UpdatePanelVisor As Object,
                                                ByVal estado_actualiza_indice As Integer,
                                                ByVal id_tarea_workflow As Long,
                                                ByVal anula_visor_espress As Integer,
                                                ByRef HiddenHeigth As Object,
                                                ByRef pag As Page,
                                                ByRef Matri_Doc_Visual() As String,
                                                ByRef Doc_actual As String) As String
        '-------------------------------------------------------------------
        'Función :Visualiza documento en el visor de workflow y el indice
        'Fecha : 2017-01-27
        'Ing Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------

        Try
            Dim Class_logdocuarchi As New Class_logdocuarchi
            Dim ref_Panel_tolbar_pdf As Panel = pag.FindControl("Panel_tolbar_pdf")
            Dim ref_UpdatePanel_panel_toll As UpdatePanel = pag.FindControl("UpdatePanel_panel_toll")
            Dim panel_div_buton As Panel = pag.FindControl("div_buton")
            Dim UpdatePanel_content_iframe As UpdatePanel = pag.FindControl("UpdatePanel_content_iframe")
            Dim panel_content_iframe As Panel = pag.FindControl("panel_content_iframe")
            Dim UpdatePanel_content_image_draw As UpdatePanel = pag.FindControl("UpdatePanel_content_image_draw")
            Dim panel_content_image_draw As Panel = pag.FindControl("panel_content_image_draw")
            If id_seleccion = "" Then
                Visualiza_documento_workflow_visor = "Por favor seleccione el documento para visualizar"
                Exit Function
            End If
            Dim ValueItem As String = id_seleccion
            Dim spligabi() As String = ValueItem.Split("|")
            Dim id_tipo_imagen As Integer = 0
            Dim extension_imagen As String = ""
            Dim Refclas As New ClassDaGabinete
            Dim Resutl As String = ""
            Resutl = Refclas.SolicitaIdTipoImagen(spligabi(1),
                                                    spligabi(0),
                                                    id_tipo_imagen)
            If Resutl <> "YES" Then
                Visualiza_documento_workflow_visor = Resutl
                Exit Function
            End If
            Dim ClassDaExtension As New Class_da_extension
            Resutl = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen,
                                                                                 extension_imagen)
            If Resutl <> "YES" Then
                Visualiza_documento_workflow_visor = Resutl
                Exit Function
            End If

            Dim refcla As New ClassWorflowVisor
            Dim refclasindice As New ClassWorkflowIndiceDA
            HttpContext.Current.Session.Item("WF_MATRI_IMAGE") = ""
            HttpContext.Current.Session.Item("WF_TAGSELECCION") = "|0|" & spligabi(1) & "|" & extension_imagen & "|" & "" & "|" & spligabi(0)
            HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = Val(spligabi(1))
            If extension_imagen = ".TIF" Or extension_imagen = ".JPG" Or extension_imagen = ".BMP" Then
                HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = spligabi(0)
                HttpContext.Current.Session.Item("WF_ID_GABINETE_SELECCIONADO") = spligabi(1)
                If estado_actualiza_indice = 1 Then
                    If Not ref_Panel_tolbar_pdf Is Nothing Then
                        ref_Panel_tolbar_pdf.Visible = False
                        ref_UpdatePanel_panel_toll.Update()
                    End If
                    If Not panel_content_iframe Is Nothing Then
                        panel_content_iframe.Visible = False
                        UpdatePanel_content_iframe.Update()
                    End If
                    If Not panel_content_image_draw Is Nothing Then
                        panel_content_image_draw.Visible = True
                        UpdatePanel_content_image_draw.Update()
                    End If
                    ifrm_visor.Attributes("SRC") = ""
                    Resutl = refclasindice.Genera_interface_indice_documento(HttpContext.Current.Session.Item("WF_ID_GABINETE_SELECCIONADO"),
                                                                             HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                             pag,
                                                                             "",
                                                                             Panel_indice,
                                                                             UpdatePanelindice,
                                                                             0,
                                                                             0)
                    If Resutl <> "YES" Then
                        Visualiza_documento_workflow_visor = Resutl
                        Exit Function
                    End If
                    Resutl = refcla.inicializa_visor_documento_workflow_neodinamyc(Matri_Doc_Visual,
                                                                                   Doc_actual,
                                                                                   pag)
                    If Resutl <> "YES" Then
                        Visualiza_documento_workflow_visor = Resutl
                        Exit Function
                    End If
                    panel_div_buton.Visible = True
                    UpdatePanelindice.Update()
                    UpdatePanelVisor.Update()
                Else
                    If Not ref_Panel_tolbar_pdf Is Nothing Then
                        ref_Panel_tolbar_pdf.Visible = False
                        ref_UpdatePanel_panel_toll.Update()
                    End If
                    If Not panel_content_iframe Is Nothing Then
                        panel_content_iframe.Visible = False
                        UpdatePanel_content_iframe.Update()
                    End If
                    If Not panel_content_image_draw Is Nothing Then
                        panel_content_image_draw.Visible = True
                        UpdatePanel_content_image_draw.Update()
                    End If
                    Resutl = refcla.inicializa_visor_documento_workflow_neodinamyc(Matri_Doc_Visual,
                                                                                   Doc_actual,
                                                                                   pag)
                    If Resutl <> "YES" Then
                        Visualiza_documento_workflow_visor = Resutl
                        Exit Function
                    End If
                    ifrm_visor.Attributes("SRC") = ""
                    UpdatePanelVisor.Update()
                End If
                Class_logdocuarchi.Registra_log_procesing_image(Val(spligabi(1)), spligabi(0), "WORKFLOW", "Visualiza", id_tarea_workflow, "", "")
                Visualiza_documento_workflow_visor = "YES"
                Exit Function
            Else
                Dim refclas_vis As New ClassVisualisaDocumento
                Dim matri_doc() As String = Nothing
                Resutl = refclas_vis.Genera_Matris_Documentos_Almacenados(Val(spligabi(1)),
                                                                          spligabi(0),
                                                                          matri_doc)

                If Resutl = "YES" Then
                    HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = spligabi(0)
                    HttpContext.Current.Session.Item("WF_ID_GABINETE_SELECCIONADO") = spligabi(1)
                    HttpContext.Current.Session.Item("WF_RUTA_DOCUMENTO_SELECCIONADO") = matri_doc(1)
                    If estado_actualiza_indice = 1 Then
                        Resutl = refclasindice.Genera_interface_indice_documento(HttpContext.Current.Session.Item("WF_ID_GABINETE_SELECCIONADO"),
                                                                                 HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                                 pag,
                                                                                 "",
                                                                                 Panel_indice,
                                                                                 UpdatePanelindice,
                                                                                 0,
                                                                                 0)
                        If Resutl <> "YES" Then
                            Visualiza_documento_workflow_visor = Resutl
                            Exit Function
                        End If
                        panel_div_buton.Visible = True
                        UpdatePanelindice.Update()
                        'Visualiza_documento_workflow_visor = "YESg" & Resutl
                        'Exit Function
                    End If
                    If Not ref_Panel_tolbar_pdf Is Nothing Then
                        ref_Panel_tolbar_pdf.Visible = True
                        ref_UpdatePanel_panel_toll.Update()
                    End If
                    If Not panel_content_iframe Is Nothing Then
                        panel_content_iframe.Visible = True
                        UpdatePanel_content_iframe.Update()
                    End If
                    If Not panel_content_image_draw Is Nothing Then
                        panel_content_image_draw.Visible = False
                        UpdatePanel_content_image_draw.Update()
                    End If
                    HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = Val(spligabi(1))
                    Dim file_inf As New IO.FileInfo(matri_doc(1))
                    If UCase(file_inf.Extension) = ".PDF" Then
                        If HttpContext.Current.Session("UTIL_VISOR_EXPRESS") = 1 And HttpContext.Current.Session("VALIDA_VISOR_EXPRES") = 1 And anula_visor_espress <> 1 Then
                            ifrm_visor.Attributes("SRC") = "../pdfjs/pdf_anotate_view/index.html?rut_image=" & matri_doc(1) & "&urimage_format=" & matri_doc(1).Replace("\", "|") & "&url_firma=" &
                                "../" & HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL") & "&" & "ash=../../workflow/Handler_image_wf.ashx" &
                                "&url_id_imagen=" & HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") & "&url_cabinete_imagen=" & HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") &
                                "&url_radicado=" & "" & "&url_id_workflow=" & id_tarea_workflow & "&url_desc_transacion=" & "WORKFLOW" &
                                "&url_printer=" & HttpContext.Current.Session.Item("IMPRIMIR_IMAGENES") & "&url_save=" & HttpContext.Current.Session.Item("UTIL_SAVE_DOCUMENT") &
                                "&url_add_firma=" & HttpContext.Current.Session.Item("AGREGAR_FIRMA") & "&url_add_stamp=" & HttpContext.Current.Session.Item("AGREGAR_STAMP")
                        Else
                            ifrm_visor.Attributes("SRC") = "../workflow/Handler_image_wf.ashx?rut_image=" & matri_doc(1)
                        End If
                    Else
                        HttpContext.Current.Session.Item("DA_DESCARGA_EXTERNO") = matri_doc(1)
                        ifrm_visor.Attributes("SRC") = "../Docuarchi/WebFormDaVisorExternoDescarga.aspx"
                    End If
                    If Not ref_Panel_tolbar_pdf Is Nothing Then
                        Dim valu = HiddenHeigth.Value - 10
                        Dim valu_he = ref_Panel_tolbar_pdf.Height.Value
                    End If
                    UpdatePanelVisor.Update()
                    Visualiza_documento_workflow_visor = "YES"
                    Class_logdocuarchi.Registra_log_procesing_image(Val(spligabi(1)), spligabi(0), "WORKFLOW", "Visualiza", id_tarea_workflow, "", "")
                    Exit Function
                Else
                    If Not ref_Panel_tolbar_pdf Is Nothing Then
                        ref_Panel_tolbar_pdf.Visible = False
                        ref_UpdatePanel_panel_toll.Update()
                    End If
                    If Not panel_content_iframe Is Nothing Then
                        panel_content_iframe.Visible = True
                        UpdatePanel_content_iframe.Update()
                    End If
                    If Not panel_content_image_draw Is Nothing Then
                        panel_content_image_draw.Visible = False
                        UpdatePanel_content_image_draw.Update()
                    End If
                    Class_logdocuarchi.Registra_log_procesing_image(Val(spligabi(1)), spligabi(0), "WORKFLOW", "Visualiza", id_tarea_workflow, "", "")
                    Dim Resutla = refcla.Limpia_Visor_Workflow(pag, "PRINCIPAL")
                    If Resutla <> "YES" Then
                        Visualiza_documento_workflow_visor = Resutla
                        Exit Function
                    Else

                        Visualiza_documento_workflow_visor = Resutla
                        Exit Function
                    End If
                End If
                Class_logdocuarchi.Registra_log_procesing_image(Val(spligabi(1)), spligabi(0), "WORKFLOW", "Visualiza", id_tarea_workflow, "", "")
                Visualiza_documento_workflow_visor = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Visualiza_documento_workflow_visor = "Inconsistencia general fucíon Visualiza_documento_workflow_visor " & ex.Message

        End Try
    End Function
    Function Consulta_Documentos_Añadidos(ByVal Id_Documento As Integer,
                                          ByVal Nombre_Gabinete As String,
                                          ByRef Matri_Datos() As Datos_Registro,
                                          ByVal User_A As String) As String
        '---------------------------------------------
        'Funcion lista listado de documentos añadidos
        'por un usuario especifico
        'Fecha : 2017-01-27
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID,DISC,PAG,IDEX,DBT  from " & Nombre_Gabinete &
                       " where dbt='" & Id_Documento & "' And USER='" & User_A & "' order by id"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DA_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Datset Is Nothing Then
                Consulta_Documentos_Añadidos = " La funcion Consulta_Documentos_Añadidos :  " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(i)
                    Matri_Datos(i).Id = (Trim(Datset.Tables(0).Rows(i).Item(0).ToString))
                    Matri_Datos(i).Disc = (Trim(Datset.Tables(0).Rows(i).Item(1).ToString))
                    Matri_Datos(i).Pag = (Trim(Datset.Tables(0).Rows(i).Item(2).ToString))
                    Matri_Datos(i).idex = (Trim(Datset.Tables(0).Rows(i).Item(3).ToString))
                    Matri_Datos(i).dbt = (Trim(Datset.Tables(0).Rows(i).Item(4).ToString))

                Next
                Consulta_Documentos_Añadidos = "YES"
                Exit Function
            Else
                Consulta_Documentos_Añadidos = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Consulta_Documentos_Añadidos = "Inconsistencia general función Consulta_Documentos_Añadidos " & ex.Message
        End Try
    End Function

    Function Genera_Matris_Documentos_Almacenados(ByVal Id_Documento As Integer,
                                                  ByVal Nombre_Gabinete As String,
                                                  ByRef Matri_Doc_a_Visualizar() As String) As String
        '***********************************************
        'Funcion : Genera_Matris_Documentos_Almacenados
        'Fecha : 2014-03-05
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Genera matriz de documentos 
        'almacenados incluyendo los documentos
        'añanidos, modificado para aplicacion web
        '************************************************
        Dim Result As String = ""
        Dim Matri_Dat_Principal() As Datos_Registro
        Erase Matri_Dat_Principal
        '*******************************
        'Consulta datos de sistema
        'del documento seleccionado
        '*******************************
        Try
            Result = Solicita_Datos_Documentos(Id_Documento,
                                               Nombre_Gabinete,
                                               Matri_Dat_Principal)
            If Result <> "YES" Then
                Genera_Matris_Documentos_Almacenados = "Imposible encontrar datos del documento Gnerando matriz de documentos"
                Exit Function
            End If
            If Matri_Dat_Principal Is Nothing Then
                Genera_Matris_Documentos_Almacenados = "Matris datos de sistema de documento principal es nula"
                Exit Function
            End If
            '***************************************
            'Consulta numero paginas documentos 
            'añadidos
            '***************************************
            Dim Ruta_Busqueda As String = ""
            Dim Numero_Doc_Añadidos As Integer = 0
            Dim Numero_Doc_Principal As Integer = 0
            Dim Matri_Documentos() As String
            Dim Estado_Documento As String = ""
            Dim Visor As String = ""
            Dim Exet As String = ""
            Erase Matri_Documentos
            Dim ref_Class_da_extension As New Class_da_extension
            Result = ref_Class_da_extension.Determina_tipo_documento_list(Matri_Dat_Principal(0).dbt,
                                                                          Visor,
                                                                          Exet,
                                                                          Estado_Documento)
            If Result <> "YES" Then
                Genera_Matris_Documentos_Almacenados = "Funcion Genera_Matris_Documentos_Almacenados " & Result
                Exit Function
            End If
            If Estado_Documento = "ADJUNTO" _
            Then
                Result = Suma_Numero_Documentos_Añadidos(Id_Documento,
                                                         Nombre_Gabinete,
                                                         Numero_Doc_Añadidos)
                If Result <> "YES" Then
                    Genera_Matris_Documentos_Almacenados = Result
                    Exit Function
                End If
                Numero_Doc_Principal = Val(Matri_Dat_Principal(0).Pag)
            Else
                Numero_Doc_Principal = Val(Matri_Dat_Principal(0).Pag)
            End If
            '****************************************
            'Consulta ruta busqueda de documentos
            '****************************************
            'Dim Refclasalamacen As New ClassAlmacenamiento
            Dim ref_Class_SYSTEM1RUT As New Class_SYSTEM1RUT
            Result = ""
            Result = ref_Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Ruta_Busqueda,
                                                                       Nombre_Gabinete)
            If Result <> "YES" Then
                Genera_Matris_Documentos_Almacenados = Result
                Exit Function
            End If
            '******************************************
            'Genera Matriz documentos del documento
            'principal
            '******************************************
            Dim Refclasvisor As New ClassWorflowVisor
            Dim ClassDaGabinete As New ClassDaGabinete
            Erase Matri_Doc_a_Visualizar
            Result = ""
            Result = ClassDaGabinete.Genera_Matriz_Documentos(Matri_Doc_a_Visualizar,
                                                              Id_Documento,
                                                              Matri_Dat_Principal(0).dbt,
                                                              Ruta_Busqueda,
                                                              Matri_Dat_Principal,
                                                              Numero_Doc_Principal,
                                                              Nombre_Gabinete,
                                                              0)
            If Result <> "YES" Then
                Genera_Matris_Documentos_Almacenados = "Error generando matris documento principal " & Result
                Exit Function
            End If
            '*******************************************
            'Determina si tiene documento añadidos
            '*******************************************
            If Numero_Doc_Añadidos = 0 Then
                Genera_Matris_Documentos_Almacenados = "YES"
                Exit Function
            End If
            '*******************************************
            'Genera matriz datos doc añadidos
            '*******************************************
            Dim Matri_Dat_Añadidos() As Datos_Registro
            Erase Matri_Dat_Añadidos
            If Estado_Documento = "ADJUNTO" _
            Then
                Result = ""
                Result = ClassDaGabinete.Consulta_Documentos_Añadidos(Id_Documento,
                                                                      Nombre_Gabinete,
                                                                      Matri_Dat_Añadidos)
                If Result <> "YES" Then
                    Genera_Matris_Documentos_Almacenados = "Error buscando documentos añadidos " & Result
                    Exit Function
                End If
                If Matri_Dat_Añadidos Is Nothing Then
                    Genera_Matris_Documentos_Almacenados = "YES"
                    Exit Function
                End If
                '*************************************************
                'Genera matriz documentos añadidos
                '*************************************************
                For z As Integer = 0 To UBound(Matri_Dat_Añadidos)
                    Result = ""
                    Result = ClassDaGabinete.Genera_Matriz_Documentos(Matri_Doc_a_Visualizar,
                                                                      Matri_Dat_Añadidos(z).Id,
                                                                      Matri_Dat_Principal(0).dbt,
                                                                      Ruta_Busqueda,
                                                                      Matri_Dat_Añadidos,
                                                                      Matri_Dat_Añadidos(z).Pag,
                                                                      Nombre_Gabinete, z)
                    If Result <> "YES" Then
                        Genera_Matris_Documentos_Almacenados = "Error Generando matris doc añadidos " & Result
                        Exit Function
                    End If
                Next
            End If
            Genera_Matris_Documentos_Almacenados = "YES"
        Catch ex As Exception
            Genera_Matris_Documentos_Almacenados = "Error General Genera_Matris_Documentos_Almacenados " & ex.Message
        End Try
    End Function
    Function Solicita_Datos_Documentos(ByVal Id_Documento As Integer,
                                       ByVal Nombre_Gabinete As String,
                                       ByRef Matri_Datos() As Datos_Registro) As String
        '--------------------------------------------------------------
        'Funcion : Solicita los datos del sistema del registro de las
        'imagenes de docuarchi.net
        'Modificacion : Se agraga le nuevo modelo de conexión de la 
        'aplicación web
        'Fecha : 2014-02-05
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID,DISC,PAG,IDEX,DBT  from " & Nombre_Gabinete &
                   " where id='" & Id_Documento & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DA_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Datset Is Nothing Then
                Solicita_Datos_Documentos = " La funcion Solicita_Datos_Documentos : Datadet nothing  Cod " & Result
                Exit Function
            End If
            If Result <> "YES" Then
                Solicita_Datos_Documentos = " La funcion Solicita_Datos_Documentos :  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_Datos_Documentos = "Imposible Encontrar datos para el id " & Id_Documento
                Exit Function
            Else
                ReDim Preserve Matri_Datos(0)
                Matri_Datos(0).Id = Datset.Tables(0).Rows(0).Item(0)
                Matri_Datos(0).Disc = Datset.Tables(0).Rows(0).Item(1)
                Matri_Datos(0).Pag = Datset.Tables(0).Rows(0).Item(2)
                Matri_Datos(0).idex = Datset.Tables(0).Rows(0).Item(3)
                Matri_Datos(0).dbt = Datset.Tables(0).Rows(0).Item(4)

            End If
            Solicita_Datos_Documentos = "YES"
        Catch ex As Exception
            Solicita_Datos_Documentos = ex.Message
        End Try
    End Function
    Function Suma_Numero_Documentos_Añadidos(ByVal Id_Documento As String,
                                             ByVal Nombre_Gabinete As String,
                                             ByRef Numero_Doc As Integer) As String
        '*****************************************
        'Funcion : Suma_Numero_Documentos_Añadidos
        'Fecha : 2011-02-16
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Consulta numero documentos
        'añadidos para la imagen
        'Modificada: 2014-03-05 para la conexión
        'de la aplicacion web
        '*****************************************
        Try
            Dim Parametro_Consulta As String = "select sum(pag) as numero_Paginas  from " & Nombre_Gabinete &
                       " where dbt='" & Id_Documento & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DA_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Datset Is Nothing Then
                Suma_Numero_Documentos_Añadidos = " La funcion Suma_Numero_Documentos_Añadidos : Dataset nothing  Cod " & Result
                Exit Function
            End If
            If Result <> "YES" Then
                Suma_Numero_Documentos_Añadidos = " La funcion Suma_Numero_Documentos_Añadidos :  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Suma_Numero_Documentos_Añadidos = "Imposible Encontrar numero paginas añadidas para el id: " & Id_Documento
                Exit Function
            Else

                Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(0)
                If IsDBNull(Tempvalor) = True Then
                    Numero_Doc = 0
                Else
                    Numero_Doc = Datset.Tables(0).Rows(0).Item(0)
                End If

            End If

            Suma_Numero_Documentos_Añadidos = "YES"
        Catch ex As Exception
            Suma_Numero_Documentos_Añadidos = "Error general Funcion " & vbCrLf &
            "Suma_Numero_Documentos_Añadidos Decri Error : " & ex.Message
        End Try
    End Function
End Class
