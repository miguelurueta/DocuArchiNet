Imports System.IO

Public Class Class_digitalizacion_guia
    Function Almacena_documentos_digitalizados_con_radicado_guia(ByVal ruta_documentos As String,
                                                                 ByVal id_tarea_workflow As Long,
                                                                 ByVal id_ruta As Integer,
                                                                 ByVal Tipo_Amacen As Integer,
                                                                 ByRef Treview As TreeView,
                                                                 ByVal tipo_digitalizacion As String,
                                                                 ByRef id_imagen_almacenada As Integer,
                                                                 Optional tipo_almacenamiento As Integer = 0) As String
        '---------------------------------------------------------------
        'Funcion : Almacena los documentos digitalizados para radicados
        ' 
        'Fecha : 2019-02-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim RefclasDigitaliza As New ClassWorkflowDigitalizacion
            Dim Matri_Documentos_Final() As String
            Dim matri_datos() As Datos_Almacenamiento
            Erase Matri_Documentos_Final
            If tipo_almacenamiento = 0 Then
                '--------------------------------------------------------
                'Retorna los documentos almacenados en el file system
                '--------------------------------------------------------
                Result = RefclasDigitaliza.SolicitaMatrizDocumentosDigitalizados(id_tarea_workflow,
                                                                                 ruta_documentos,
                                                                                 Matri_Documentos_Final)
                If Result <> "YES" Then
                    Almacena_documentos_digitalizados_con_radicado_guia = "Imposible encontrar documentos " & ruta_documentos & Result
                    Exit Function
                End If
            Else
                '-------------------------------------------------------
                'Retorma matriz de documentos almacenados adjuntos
                '-------------------------------------------------------
                Dim Refclas_almacenamiento As New ClassAlmacenamiento
                Result = Refclas_almacenamiento.Retorna_matriz_documentos_adjuntos_workflow(Matri_Documentos_Final)
                If Result <> "YES" Then
                    Almacena_documentos_digitalizados_con_radicado_guia = Result
                    Exit Function
                End If
            End If
            Dim Gabinete As String = HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE")
            Dim Radicado As String = HttpContext.Current.Session.Item("DG_RADICADO")
            ReDim Preserve matri_datos(0)
            Dim nombre_campo_radicado_gabinete As String = ""
            Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Result = Ref_Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(Gabinete,
                                                                                nombre_campo_radicado_gabinete)
            If Result <> "YES" Then
                Almacena_documentos_digitalizados_con_radicado_guia = Result
                Exit Function
            End If
            matri_datos(0).nombre_campo = nombre_campo_radicado_gabinete
            If Tipo_Amacen = 1 Then
                matri_datos(0).valor_campo = Radicado
            Else
                matri_datos(0).valor_campo = ""
            End If
            ReDim Preserve matri_datos(1)
            matri_datos(1).nombre_campo = "ENLASE"
            matri_datos(1).valor_campo = Radicado
            '-----------------------------------------------
            'Retorna nombre ruta tarea
            '-----------------------------------------------
            Dim Nombre_ruta As String = ""
            Dim Refclas_workflow As New ClassWorkflow
            Dim Ref_class_ruta As New Class_worflow_rutas
            'Result = Ref_class_ruta.Retorna_nombre_ruta_por_id_ruta(id_ruta.ToString, _
            '                                                        Nombre_ruta)
            'If Result <> "YES" Then
            '    Almacena_documentos_digitalizados_con_radicado_guia = Result
            '    Exit Function
            'End If
            Nombre_ruta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
            '------------------------------------------------
            'Retorna si el tipo de tarea workflow es externa
            'Valores 1. Tarea interna    2. Tarea externa
            '------------------------------------------------
            Dim Ref_dat_adic As New Class_DAT_ADIC_TAR
            Dim id_tipo_tarea As Integer = 0
            Result = Ref_dat_adic.SolicitaTipoFujoExternoInterno(id_tarea_workflow,
                                                                         id_tipo_tarea,
                                                                         Nombre_ruta)
            If Result <> "YES" Then
                Almacena_documentos_digitalizados_con_radicado_guia = Result
                Exit Function
            End If
            '------------------------------------------------
            'Retorna el nombre de la tabla de radicación
            'si el flujo se genero internamente desde
            'desde el radicador
            '-----------------------------------------------
            Dim Refclasalmacena As New ClassAlmacenamiento
            Dim Nombre_plantilla_radicado As String = ""
            Dim id_expediente As Integer = 0
            Dim id_tipo_expediente As Integer = 0
            Dim nombre_expediente As String = ""
            Dim nombre_tipo_documento As String = ""
            Dim id_clase_documento As Integer = 0
            Dim fecha_elaboracion As String = ""
            Dim Refclas_radicado As New ClassRadicador
            Dim reflcas_tipo_documento As New ClassGaTipoDocumental
            If id_tipo_tarea = 1 Then
                Dim Ref_Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
                Result = Ref_Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                                  Nombre_plantilla_radicado)
                If Result <> "YES" Then
                    Almacena_documentos_digitalizados_con_radicado_guia = Result
                    Exit Function
                End If
                '---------------------------------------
                'Retorna expediente y id expediente
                '---------------------------------------
                If Nombre_plantilla_radicado <> "" Then
                    Result = Refclas_radicado.Retorna_nombre_expediente_id_expediente_radicado(Radicado,
                                                                                               Nombre_plantilla_radicado,
                                                                                               id_expediente,
                                                                                               nombre_expediente,
                                                                                               id_tipo_expediente)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = Result
                        Exit Function
                    End If
                End If
                If id_expediente <> 0 Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                                   id_clase_documento)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                    Dim ref
                    Dim date1al As String = Date.Today
                    Result = ""
                    Dim ref_ClassGestionFechas As New ClassGestionFechas
                    Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = "Error formateando fecha almacenamiento Funcion: Almacenamiento_Documentos_Digitalizados " & Result
                        Exit Function
                    Else
                        fecha_elaboracion = date1al

                    End If
                End If
            End If
            '----------------------------------------------
            'Configura la radicacion tipo tramite
            '----------------------------------------------
            Dim id_tipo_documento As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim descripcion_tipo_documento As String = ""
            Dim nombre_area As String = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            If HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") <> -1 Then
                Dim stru As stru_tipo_lista_chequeo
                Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
                Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
                                                                                                                 stru)
                If Result <> "YES" Then
                    Almacena_documentos_digitalizados_con_radicado_guia = Result
                    Exit Function
                End If
                If stru.subseries_documentales_Id_SubSeries <> 0 Then
                    id_tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
                Else
                    id_tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
                End If
                '-----------------------------------------------
                'Retorna serie y sub serie tipo documento
                '-----------------------------------------------
                Dim stru_tipo As stru_tipo_documental = Nothing
                Dim ref_clas_trd As New ClassTrdDocumental
                Result = ref_clas_trd.Solicita_datos_estructura_tipo_documento(id_tipo_documento, stru_tipo)
                If Result <> "YES" Then
                    Almacena_documentos_digitalizados_con_radicado_guia = Result
                    Exit Function
                End If
                id_serie = stru_tipo.Series_Documentales_Id_Series
                id_sub_serie = stru_tipo.sub_serie_id_serie
                Dim ref_Class_series_documentales As New Class_series_documentales
                Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(id_serie,
                                                                                        id_area)
                If Result <> "YES" Then
                    Almacena_documentos_digitalizados_con_radicado_guia = Result
                    Exit Function
                End If
                Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
                If id_tipo_documento <> 0 Then
                    Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(id_serie,
                                                                                        id_sub_serie,
                                                                                        id_tipo_documento,
                                                                                        descripcion_tipo_documento)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = Result
                        Exit Function
                    End If
                End If
                If nombre_tipo_documento = "" Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                                   id_clase_documento)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                End If
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                If id_area <> 0 Then
                    Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area,
                                                                                               nombre_area)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = Result
                        Exit Function
                    End If
                End If

                If id_serie <> 0 Then
                    Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(id_serie,
                                                                                         nombre_serie)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = Result
                        Exit Function
                    End If
                End If
                Dim Class_subseries_documentales As New Class_subseries_documentales
                If id_sub_serie <> 0 Then
                    Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
                                                                                    nombre_sub_serie)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = Result
                        Exit Function
                    End If
                End If

                Dim date1al As String = Date.Today
                Result = ""
                If fecha_elaboracion = "" Then
                    Dim ref_ClassGestionFechas As New ClassGestionFechas
                    Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = "Error formateando fecha almacenamiento Funcion: Almacenamiento_Documentos_Digitalizados " & Result
                        Exit Function
                    Else
                        fecha_elaboracion = date1al

                    End If
                End If

            End If
            Dim Ref_producion As New ClassGaProducionDocumental
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(Gabinete,
                                                                                                        inventario_documental,
                                                                                                        aplica_trd,
                                                                                                        asigna_unidad)
            If Result <> "YES" Then
                Almacena_documentos_digitalizados_con_radicado_guia = Result
                Exit Function
            End If
            If aplica_trd = 1 Then
                If nombre_tipo_documento = "" Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                                  id_clase_documento)
                    If Result <> "YES" Then
                        Almacena_documentos_digitalizados_con_radicado_guia = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                End If
            End If

            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            Dim matri_gestion As estructure_gestion
            matri_gestion.CLASE_DOCUMENTO = nombre_tipo_documento
            matri_gestion.EXPEDIENTE = nombre_expediente
            matri_gestion.ID_AREA = id_area
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            matri_gestion.ID_EXPEDIENTE = id_expediente
            matri_gestion.ID_SERIE = id_serie
            matri_gestion.ID_SUB_SERIE = id_sub_serie
            matri_gestion.ID_TIPO_EXPEDIENTE = id_tipo_expediente
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = fecha_elaboracion
            matri_gestion.TIPODOCUMENTO = descripcion_tipo_documento
            matri_gestion.NOMBRE_SERIE = nombre_serie
            matri_gestion.NOMBRE_SUB_SERIE = nombre_sub_serie
            ReDim Preserve matri_datos(2)
            matri_datos(2).nombre_campo = "EXPEDIENTE"
            matri_datos(2).valor_campo = nombre_expediente
            ReDim Preserve matri_datos(3)
            matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
            matri_datos(3).valor_campo = nombre_tipo_documento
            ReDim Preserve matri_datos(4)
            matri_datos(4).nombre_campo = "FECHAELABORACION"
            matri_datos(4).valor_campo = fecha_elaboracion
            ReDim Preserve matri_datos(5)
            matri_datos(5).nombre_campo = "TIPODOCUMENTO"
            matri_datos(5).valor_campo = descripcion_tipo_documento
            ReDim Preserve matri_datos(6)
            matri_datos(6).nombre_campo = "NOMBRESERIE"
            matri_datos(6).valor_campo = nombre_serie
            ReDim Preserve matri_datos(7)
            matri_datos(7).nombre_campo = "NOMBRESUBSERIE"
            matri_datos(7).valor_campo = nombre_sub_serie
            '----------------------------------------------
            'Genera la matriz de datos de almacenamiento
            '----------------------------------------------
            Dim Refclas_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim Matri_Datos_Almacen() As String
            Erase Matri_Datos_Almacen
            Result = Refclas_Class_DETALLE_GABIENETE.SolicitaValoresCamposDocumentoGabinete(Matri_Datos_Almacen,
                                                                                                Gabinete,
                                                                                                matri_datos)
            If Result <> "YES" Then
                Almacena_documentos_digitalizados_con_radicado_guia = "Imposible general mariz datos almacenamiento "
                Exit Function
            End If
            If Matri_Datos_Almacen Is Nothing Then
                Almacena_documentos_digitalizados_con_radicado_guia = "Matriz de datos de almacenamiento es nothing "
                Exit Function
            End If
            '----------------------------------------------
            'Obtiene el tipo documento 
            '----------------------------------------------
            Dim Tipo_Documento As Integer = -1
            Dim Refclasvisor As New Classactualizacionvisor
            Result = ""
            Dim filinf As New FileInfo(Matri_Documentos_Final(0))
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(filinf.Extension,
                                                                          Tipo_Documento)
            If Result <> "YES" Then
                Almacena_documentos_digitalizados_con_radicado_guia = Result
                Exit Function
            End If

            '-----------------------------------------------
            'Almacena documento en la base docuarchi.net
            '-----------------------------------------------
            Dim UserWf As String = "WF-" & HttpContext.Current.Session.Item("Login_Usuario_Workfow")
            Result = ""
            Result = Refclasalmacena.Almacenamiento("", "", Gabinete, 0, Matri_Datos_Almacen, 2,
            Matri_Documentos_Final.Length, Tipo_Documento, Matri_Documentos_Final, 0, id_imagen_almacenada,
            Tipo_Documento, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA,
            matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, Radicado)
            If Result <> "YES" Then
                Almacena_documentos_digitalizados_con_radicado_guia = Result
                Exit Function
            End If
            'Dim ilist As New ListItem
            'ilist.Text = ID_ALMACEN & "|R" & matri_datos(0).valor_campo
            ''ilist.Value = Datset.Tables(0).Rows(i).Item(0).ToString & "|" & _
            ''   Datset.Tables(0).Rows(i).Item(1).ToString & "|" & _
            ''  Datset.Tables(0).Rows(i).Item(2).ToString & "|" & _
            '' Datset.Tables(0).Rows(i).Item(3).ToString & "|" & _
            ''Datset.Tables(0).Rows(i).Item(4).ToString()
            'ilist.Value = Gabinete & "|" & List.Items.Count + 1
            'List.Items.Add(ilist)
            Dim attrNodeGru1 As New TreeNode
            attrNodeGru1.Value = Gabinete & "|" & id_imagen_almacenada & "|" & Radicado & "|" & Tipo_Documento
            attrNodeGru1.PopulateOnDemand = False
            'attrNodeGru1.ImageUrl = "../workflow/imageneswf/page_white.png"
            Dim refclas_seleccion As New Classselecciotarea
            Result = refclas_seleccion.Agrega_icono_image_tre_view(Matri_Documentos_Final(0), attrNodeGru1)
            If descripcion_tipo_documento = "" Then
                attrNodeGru1.Text = "Documento(" & Treview.Nodes.Count & ")"
            Else
                attrNodeGru1.Text = descripcion_tipo_documento
            End If
            Treview.Nodes.Add(attrNodeGru1)
            '-----------------------------------------------
            'Elimina los documentos almacenados
            '-----------------------------------------------
            For k As Integer = 0 To Matri_Documentos_Final.Length - 1
                If File.Exists(Matri_Documentos_Final(k)) Then
                    File.Delete(Matri_Documentos_Final(k))
                End If
            Next
            If tipo_almacenamiento <> 0 Then
                If File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")) = True Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                End If
                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
            End If
            Almacena_documentos_digitalizados_con_radicado_guia = "YES"
            Exit Function
        Catch ex As Exception
            Almacena_documentos_digitalizados_con_radicado_guia = "Inconsistencia general función Almacena_documentos_digitalizados_con_radicado_guia " & ex.Message
        End Try

    End Function
    Function Activa_adjuntar_documento_guia_respuesta(ByVal id_respuesta As Integer, _
                                                      ByRef pag As Page) As String
        Try
            Dim nombre_ruta_default As String = ""
            Dim id_ruta As Integer = 0
            Dim id_tarea As Long = 0
            Dim radicado As String = ""
            Dim estru As stru_envio = Nothing
            Dim Result As String = ""
            Dim Ref_class_workflow_ruta As New Class_worflow_rutas
            Dim ref_class_respuesta_radicado As New Class_ra_respuesta_radicado
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim ref_class_worflow As New ClassWorkflow
            Dim Hidden_0001 As Object = pag.FindControl("Hidden_0001")
            Dim Hidden_0002 As Object = pag.FindControl("Hidden_0002")
            Dim Label_estado_lista_chequeo As Label = pag.FindControl("Label_estado_lista_chequeo")
            Dim UpdateGeneral As UpdatePanel = pag.FindControl("UpdateGeneral")
            Dim ModalPopupExtender_edition_lista_chequeo_tramite As AjaxControlToolkit.ModalPopupExtender _
                = pag.FindControl("ModalPopupExtender_edition_lista_chequeo_tramite")
            Dim ModalPopupExtender_sube_documento_adjunto As AjaxControlToolkit.ModalPopupExtender _
                = pag.FindControl("ModalPopupExtender_sube_documento_adjunto")
            Dim estado_resultado As String = ""
            If HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") <> -1 Then
                '---------------------------------------------------------------------------------------
                'Solicita tipos documentales relacionados al tramite
                '---------------------------------------------------------------------------------------
                Result = Refclas_digitalizacion.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta(HttpContext.Current.Session.Item("DG_ID_TRAMITE"), _
                                                                                                                              HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"), _
                                                                                                                              pag, _
                                                                                                                              estado_resultado)
            End If
            Hidden_0001.Value = "-1"
            If Result <> "YES" Then
                Hidden_0002.Value = "0"
                Label_estado_lista_chequeo.Text = Result
                UpdateGeneral.Update()
            End If
            If estado_resultado = "YES" Then
                Hidden_0002.Value = "1"
                ModalPopupExtender_edition_lista_chequeo_tramite.Show()
                HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
            Else
                HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
                HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                ModalPopupExtender_sube_documento_adjunto.Show()
            End If
            Activa_adjuntar_documento_guia_respuesta = "YES"
        Catch ex As Exception
            Activa_adjuntar_documento_guia_respuesta = "Inconsistencia general función Activa_adjuntar_documento_guia_respuesta " & ex.Message
        End Try
    End Function
    Function Activa_digitalizar_documento_resp_guia(ByRef pag As Page, _
                                                    ByVal id_respuesta As Integer) As String
        Try


            Dim UpdatePanelseleccion_digitalizado As Object = pag.FindControl("UpdatePanelseleccion_digitalizado")
            Dim Ref_HiddenIdFlujo As Object = pag.FindControl("HiddenIdFlujo")
            Dim Ref_HiddenRuta As Object = pag.FindControl("HiddenRuta")
            Dim UpdateDatos As Object = pag.FindControl("UpdateDatos")
            Dim ModalpopoenlaceExt As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalpopoenlaceExt")
            Dim TreeViewseleccion_digitalizado As TreeView = pag.FindControl("TreeViewseleccion_digitalizado")
            Dim Label_relacion_documentos As Label = pag.FindControl("Label_relacion_documentos")
            Dim IframeDitaliza As Object = pag.FindControl("IframeDitaliza_")
            Dim UpdatePanel_iframe_digitaliza As UpdatePanel = pag.FindControl("UpdatePanel_iframe_digitaliza")
            'UpdatePanel_general_variable.Update()
            HttpContext.Current.Session.Item("WF_TAGSELECCION_EMERGENTE") = ""
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = ""
            HttpContext.Current.Session.Item("DG_ID_TRAMITE") = 0
            HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = ""
            HttpContext.Current.Session.Item("DG_ID_GABINETE") = 0
            HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE") = ""
            HttpContext.Current.Session.Item("DG_RADICADO") = ""
            HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") = -1
            HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = ""
            HttpContext.Current.Session.Item("WF_TAGSELECCION_EMERGENTE") = ""
            HttpContext.Current.Session.Item("DG_ID_RUTA") = 0
            HttpContext.Current.Session.Item("DG_ID_TAREA") = 0
            Dim nombre_ruta_default As String = ""
            Dim id_ruta As Integer = 0
            Dim id_tarea As Long = 0
            Dim radicado As String = ""
            Dim estru As stru_envio = Nothing
            Dim Result As String = ""
            Dim Ref_class_workflow_ruta As New Class_worflow_rutas
            Dim ref_class_respuesta_radicado As New Class_ra_respuesta_radicado
            Dim ref_class_worflow As New ClassWorkflow
            Result = ref_class_respuesta_radicado.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, _
                                                                                                   estru, _
                                                                                                   1)
            If Result <> "YES" Then
                Activa_digitalizar_documento_resp_guia = Result
                Exit Function
            End If
            If estru.RADICADO = "" Then
                Activa_digitalizar_documento_resp_guia = "La respuesta no tiene un radicado seleccionado"
                Exit Function
            End If
            radicado = estru.RADICADO
            Dim Ref_class_wf_ruta As New Class_worflow_rutas
            Result = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(nombre_ruta_default)
            If Result <> "YES" Then
                Activa_digitalizar_documento_resp_guia = Result
                Exit Function
            End If
            Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta_default, _
                                                                id_ruta)
            If Result <> "YES" Then
                Activa_digitalizar_documento_resp_guia = Result
                Exit Function
            End If
            Dim refclas As New Classselecciotarea
            Result = refclas.Retorna_id_tarea_seleccionada_radicado(radicado, _
                                                                    id_ruta, _
                                                                    id_tarea)
            If Result <> "YES" Then
                Activa_digitalizar_documento_resp_guia = Result
                Exit Function
            End If

            Dim Ruta_Web_Escaner As String = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString.Replace("\", "/")
            Ref_HiddenRuta.Value = Ruta_Web_Escaner
            Dim Refclas_digitalizacion As New ClassWorkflowDigitalizacion
            '-----Retorna el tipo de flujo interno o externo
            Dim refclas_dat_adit As New Class_DAT_ADIC_TAR
            Dim id_tipo_flujo As Integer = 0
            Result = refclas_dat_adit.SolicitaIdTipoFlujoTareaWorkflow(id_tarea, _
                                                            nombre_ruta_default, _
                                                            id_tipo_flujo)
            If Result <> "YES" Then
                Activa_digitalizar_documento_resp_guia = Result
                Exit Function
            End If
            If id_tipo_flujo = 1 Then
                HttpContext.Current.Session.Item("DG_RADICADO") = radicado
                Result = Refclas_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna(id_ruta, _
                                                                                 id_tarea, _
                                                                                 HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"), _
                                                                                 HttpContext.Current.Session.Item("DG_ID_TRAMITE"), _
                                                                                 HttpContext.Current.Session.Item("DG_ID_GABINETE"), _
                                                                                 HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"), _
                                                                                 HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION"), _
                                                                                 HttpContext.Current.Session.Item("DG_RADICADO"), _
                                                                                 HttpContext.Current.Session("DG_NOMBRE_TRAMITE"))
            Else
                Result = Refclas_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna(id_ruta, _
                                                                                         id_tarea, _
                                                                                         nombre_ruta_default, _
                                                                                         HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"), _
                                                                                         HttpContext.Current.Session.Item("DG_ID_GABINETE"), _
                                                                                         HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"), _
                                                                                         HttpContext.Current.Session.Item("DG_RADICADO"), _
                                                                                         HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"))
                If Result <> "YES" Then
                    Activa_digitalizar_documento_resp_guia = Result
                    Exit Function
                End If
                Dim ref_class_tipo_doc_entrante As New Class_tipo_doc_entrante
                Result = ref_class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                       HttpContext.Current.Session.Item("DG_ID_TRAMITE"))
                If Result <> "YES" Then
                    Activa_digitalizar_documento_resp_guia = Result
                    Exit Function
                End If
            End If
            Dim Refclasdigitaliza As New ClassWorkflowDigitalizacion
            Dim Resultado As String = ""
            Result = Refclasdigitaliza.Lista_Documentos_Almacenados_Escaner_Treview(Resultado, _
                                                                                    id_tarea, _
                                                                                    TreeViewseleccion_digitalizado, _
                                                                                    HttpContext.Current.Session.Item("DG_ID_GABINETE"), _
                                                                                    HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"), _
                                                                                    UpdateDatos, _
                                                                                    Label_relacion_documentos, _
                                                                                    UpdatePanelseleccion_digitalizado, _
                                                                                    HttpContext.Current.Session.Item("DG_RADICADO"))
            If Result <> "YES" Then
                Activa_digitalizar_documento_resp_guia = "Inconsistencia buscando documentos almacenados " & Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE"
            Result = Refclasdigitaliza.EliminaDocumentosDigigitalizados(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"))
            If Result <> "YES" Then
                Activa_digitalizar_documento_resp_guia = "Inconsistencia eliminando documentos temporales " & Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("DG_ID_RUTA") = id_ruta
            HttpContext.Current.Session.Item("DG_ID_TAREA") = id_tarea
            Ref_HiddenIdFlujo.value = id_tarea
            Ref_HiddenRuta.value = id_ruta
            IframeDitaliza.Attributes.Add("src", "../workflow/WebFormEscan.aspx")
            UpdatePanel_iframe_digitaliza.Update()
            UpdateDatos.Update()
            ModalpopoenlaceExt.Show()
            Activa_digitalizar_documento_resp_guia = "YES"
            Exit Function
        Catch ex As Exception
            Activa_digitalizar_documento_resp_guia = "Inconsistencia general función Activa_digitalizar_documento_resp_guia " & ex.Message
        End Try
    End Function
End Class
