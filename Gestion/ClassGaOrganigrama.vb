Imports MySql.Data.MySqlClient
Imports System
Imports System.IO
Imports System.Drawing
Imports System.Web.Script.Serialization
Imports MindFusion.Diagramming
Imports MindFusion.Diagramming.WebForms
Imports MindFusion.Diagramming.Import.VisioImporter
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports Graphics = System.Drawing.Graphics
Public Structure stru_organigrama
    Dim id_ruta As Integer
    Dim nombre_ruta As String
End Structure
Public Structure stru_diagrama_organico
    Dim EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA As Integer
    Dim NOMBRE_ORGANIGRAMA As String
    Dim FECHA_ORGANIGRAMA As String
    Dim ESTADO_ORGANIGRAMA As String
    Dim NUMERO_RESOLUCION As String
    Dim DETALLE_RESOLUCION As String
    Dim CONSECUTIVO_AREA_DEPARTAMENTO As Integer
    Dim VERSION_ORGANIGRAMA As String
    Dim CODIGO_ISO As String
    Dim CONSECUTIVO_SERIE As Integer
    Dim CONSECUTIVO_SUB_SERIE As Integer
    Dim FECHA_REGITRO_SISTEMA As String
End Structure
Public Structure stru_area_organigrama
    Dim Codigo_Area As Integer
    Dim Nombre_Area As String
    Dim Descripcion As String
    Dim Estado_Area As Integer
    Dim Codigo_Arbitrario As String
    Dim Consecutivo_Serie As Integer
    Dim CONSECUTIVO_AREA As Integer
    Dim Estado_Area_Pqr As Integer
    Dim Estado_Publico_Area As Integer
    Dim Area_padre As Integer
End Structure
Public Class ClassGaOrganigrama
    Function Seleccion_menu_pricipal(ByVal valor_seleccion As String, ByRef pag As Page) As String
        Try
            Dim Check_activa_area As CheckBox = pag.FindControl("Check_activa_area")
            Dim CheckBox_inactiva_area As CheckBox = pag.FindControl("CheckBox_inactiva_area")
            Dim UpdatePanel_activar_inactivar As UpdatePanel = pag.FindControl("UpdatePanel_activar_inactivar")
            Dim UpdatePanel_diagran_view As UpdatePanel = pag.FindControl("UpdatePanel_diagran_view")
            Dim diagramView As Object = pag.FindControl("diagramView")
            Dim ModalPopupExtender_activar_inactivar As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_activar_inactivar")
            Dim ModalPopupExtender_agregar_organigrama As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_agregar_organigrama")
            Dim UpdatePanel_editar_organigrama As UpdatePanel = pag.FindControl("UpdatePanel_editar_organigrama")
            Dim TextBox_nombre_organigrama_editar As TextBox = pag.FindControl("TextBox_nombre_organigrama_editar")
            Dim TextBox_codigo_resulucion_editar As TextBox = pag.FindControl("TextBox_codigo_resulucion_editar")
            Dim TextBox_fecha_organigrama_editar As TextBox = pag.FindControl("TextBox_fecha_organigrama_editar")
            Dim TextBox_descripcion_resolucion_editar As TextBox = pag.FindControl("TextBox_descripcion_resolucion_editar")
            Dim TextBox_version_organigrama_editar As TextBox = pag.FindControl("TextBox_version_organigrama_editar")
            Dim TextBox_codigo_norma_editar As TextBox = pag.FindControl("TextBox_codigo_norma_editar")
            Dim ModalPopupExtender_editar_organigrama As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_editar_organigrama")
            Dim Label_Confirmado As Label = pag.FindControl("Label_Confirmado")
            Dim HiddenField_tipo_operacion As Object = pag.FindControl("HiddenField_tipo_operacion")
            Dim UpdatePanel_confirmar_eliminar As UpdatePanel = pag.FindControl("UpdatePanel_confirmar_eliminar")
            Dim ModalPopupExtender_confirmar_eliminar As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_confirmar_eliminar")
            Dim DropDownList_organigramas_disponibles As DropDownList = pag.FindControl("DropDownList_organigramas_disponibles")
            Dim updatemenu As UpdatePanel = pag.FindControl("updatemenu")
            Dim Check_activa_organigrama As CheckBox = pag.FindControl("Check_activa_organigrama")
            Dim CheckBox_inactiva_organigrama As CheckBox = pag.FindControl("CheckBox_inactiva_organigrama")
            Dim ModalPopupExtender_cambia_estado_organigrama As AjaxControlToolkit.ModalPopupExtender = pag.FindControl("ModalPopupExtender_cambia_estado_organigrama")
            Dim UpdatePanel_cambia_estado_organigrama As UpdatePanel = pag.FindControl("UpdatePanel_cambia_estado_organigrama")

            Dim Result As String = ""
            Dim refclas_ga_organigrama As New ClassGaOrganigrama
            If valor_seleccion = "ORG-CAMBIA-ESTADO-ORG" Then
                If diagramView.Diagram.Selection.Items.Count = 0 Then
                    Seleccion_menu_pricipal = "Debe seleccionar la actividad para cambiar de estado"
                    Exit Function
                End If
                If diagramView.Diagram.Selection.Items.Count > 1 Then
                    Seleccion_menu_pricipal = "Por favor seleccione una sola actividad para cambiar de estado"
                    Exit Function
                End If
                Dim obshape As Object = diagramView.Diagram.Selection.Items(0)
                Dim ob As Object = obshape.GetType
                If ob.Fullname <> "MindFusion.Diagramming.ShapeNode" Then
                    Seleccion_menu_pricipal = "Debe seleccionar la actividad para cambiar de estado"
                    Exit Function
                End If
                Result = Me.Asigna_datos_estado_interface_area_departamento(obshape.id, _
                                                                            Check_activa_area, _
                                                                            CheckBox_inactiva_area, _
                                                                            UpdatePanel_activar_inactivar)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                Else
                    ModalPopupExtender_activar_inactivar.Show()
                End If
            End If
            If valor_seleccion = "ORG-ADD-ORG" Then
                ModalPopupExtender_agregar_organigrama.Show()
            End If
            If valor_seleccion = "ORG-EDIDA-ORG" Then
                If DropDownList_organigramas_disponibles.Text = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el organigrama "
                    Exit Function
                End If
                Result = refclas_ga_organigrama.Activa_editar_organigrama(HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO"), _
                                                                         TextBox_nombre_organigrama_editar.Text, _
                                                                         TextBox_codigo_resulucion_editar.Text, _
                                                                         TextBox_descripcion_resolucion_editar.Text, _
                                                                         TextBox_version_organigrama_editar.Text, _
                                                                         TextBox_codigo_norma_editar.Text, _
                                                                         TextBox_fecha_organigrama_editar.Text, _
                                                                         UpdatePanel_editar_organigrama, _
                                                                         ModalPopupExtender_editar_organigrama)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                End If
            End If
            If valor_seleccion = "ORG-ELIM-ORG" Then
                If DropDownList_organigramas_disponibles.Text = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el organigrama "
                    Exit Function
                End If
                Label_Confirmado.Text = "Desea eliminar el organigrama seleccionado " & DropDownList_organigramas_disponibles.SelectedItem.Text
                HiddenField_tipo_operacion.Value = "ELIMINADIAGRAMA"
                UpdatePanel_confirmar_eliminar.Update()
                updatemenu.Update()
                ModalPopupExtender_confirmar_eliminar.Show()
            End If
            If valor_seleccion = "ORG-ACTIVA-ORG" Then
                If DropDownList_organigramas_disponibles.Text = "" Then
                    Seleccion_menu_pricipal = "Debe seleccionar el organigrama "
                    Exit Function
                End If
                Result = Me.Activa_cambia_estado_organigrama(DropDownList_organigramas_disponibles.SelectedValue, _
                                                            Check_activa_organigrama, _
                                                             CheckBox_inactiva_organigrama, _
                                                             ModalPopupExtender_cambia_estado_organigrama, _
                                                              UpdatePanel_cambia_estado_organigrama)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                End If
            End If
            If valor_seleccion = "ORG-EXP-ORG" Then
                If HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = 0 Then
                    Seleccion_menu_pricipal = "Debe seleccionar el organigrama para exportar "
                    Exit Function
                End If
                Dim ref_ifmExcel As Object = pag.FindControl("ifmExcel_")
                Dim ref_Hidden_ruta_archivo As Object = pag.FindControl("Hidden_ruta_archivo")
                Dim ref_updatapanel_iframe As Object = pag.FindControl("updatapanel_iframe")
                Result = Me.Exporta_pdf_mindifucion_diagrama_organico(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL"), diagramView, ref_ifmExcel, ref_Hidden_ruta_archivo, ref_updatapanel_iframe)
                If Result <> "YES" Then
                    Seleccion_menu_pricipal = Result
                    Exit Function
                End If
            End If

            Seleccion_menu_pricipal = "YES"
        Catch ex As Exception
            Seleccion_menu_pricipal = "Inconsistencia general función Seleccion_menu_pricipal " & ex.Message
        End Try
    End Function
    Function Solicita_organigramas_workflow(ByVal id_empresa As Integer, _
                                             ByRef rutas() As stru_organigrama) As String
        '----------------------------------------------
        'Función : Solicita_nombres_rutas_workflow
        'Fecha : 2018-05-29
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            Erase rutas
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("registro_organigrama")
            Dim sql_consulta As String = "Select ID_ORGANIGRAMA,NOMBRE_ORGANIGRAMA from registro_organigrama " & _
                " where EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_organigramas_workflow = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_organigramas_workflow = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve rutas(i)
                    rutas(i).id_ruta = Datset.Tables(0).Rows(i).Item(0)
                    rutas(i).nombre_ruta = Datset.Tables(0).Rows(i).Item(1)
                Next
                Solicita_organigramas_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_organigramas_workflow = "Inconsistencia general función Solicita_organigramas_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_id_organigramas_default(ByVal id_empresa As Integer, _
                                              ByRef id_organigrama As Integer) As String
        '----------------------------------------------
        'Función : Solicita_organigramas_default
        'Fecha : 2018-05-29
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try

            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("registro_organigrama")
            Dim sql_consulta As String = "Select ID_ORGANIGRAMA from registro_organigrama " & _
                " where EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa & _
                " and ESTADO_ORGANIGRAMA=1"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_organigramas_default = "Función Solicita_id_organigramas_default dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_organigramas_default = "Imposible encontrar organigrama activo en el sistema"
                Exit Function
            Else
                id_organigrama = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_organigramas_default = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_organigramas_default = "Inconsistencia general función Solicita_id_organigramas_default " & ex.Message
        End Try
    End Function
    Function Lista_organigramas_interface_importacion(ByVal rutas() As stru_organigrama, _
                                                      ByRef ref_droplis As DropDownList, _
                                                      ByRef ref_update As UpdatePanel,
                                                      ByVal estado_limpia As Integer) As String
        '------------------------------------------------------
        'Función : Lista el nombre de organigramas disponibes
        'en la interface
        'Fecha : 2018-05-29
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            If estado_limpia = 0 Then
                ref_droplis.Items.Clear()
            End If
            If Not rutas Is Nothing Then

                For i As Integer = 0 To rutas.Length - 1
                    Dim ilis As New ListItem
                    ilis.Text = rutas(i).nombre_ruta
                    ilis.Value = rutas(i).id_ruta
                    ref_droplis.Items.Add(ilis)
                Next
                ref_update.Update()
                Lista_organigramas_interface_importacion = "YES"
            Else
                ref_update.Update()
                Lista_organigramas_interface_importacion = "YES"
            End If
        Catch ex As Exception
            Lista_organigramas_interface_importacion = "Inconsistencia general función Lista_organigramas_interface_importacion " & ex.Message
        End Try
    End Function
    Function Abrir_organigrama(ByVal id_organigrama As Integer, _
                              ByRef up_date_panel As UpdatePanel, _
                              ByRef diagramView As DiagramView, _
                              ByVal zon As Object, _
                              ByRef ref_CheckBox_Grid_alineamiento As Object, _
                              ByRef updatemenu As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim plantilla_organigrama As String = ""
            If id_organigrama = -1 Then
                HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = id_organigrama
                diagramView.Diagram.ClearAll()
                up_date_panel.Update()
                updatemenu.Update()
                Abrir_organigrama = "YES"
                Exit Function
            End If
            Result = Me.Solicita_diagrama_organigrama(id_organigrama, plantilla_organigrama)
            If Result <> "YES" Then
                Abrir_organigrama = Result
                Exit Function
            End If
            Dim matri_areas() As stru_area_organigrama = Nothing
            If plantilla_organigrama = "" Then
                Result = Me.Solicita_areas_relacionadas_a_organigrama(id_organigrama, _
                                                                      matri_areas)
                If Result <> "YES" Then
                    Abrir_organigrama = Result
                    Exit Function
                End If
                Result = Me.Crear_diagrama_organigrama(diagramView, matri_areas, up_date_panel)
                If Result <> "YES" Then
                    Abrir_organigrama = Result
                    Exit Function
                End If
                Dim string_diagrama As String = diagramView.SaveToString(SaveToStringFormat.Base64, True)
                Result = Me.Actualiza_diagrama_organigrama(string_diagrama, _
                                                           id_organigrama)
                If Result <> "YES" Then
                    Abrir_organigrama = Result
                    Exit Function
                End If
                HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = id_organigrama
                up_date_panel.Update()
                Abrir_organigrama = "YES"
                Exit Function
            Else

                diagramView.Diagram.LoadFromString(plantilla_organigrama)
                diagramView.ZoomFactor = zon
                If diagramView.Diagram.ShowGrid = True Then
                    ref_CheckBox_Grid_alineamiento.Checked = True
                Else
                    ref_CheckBox_Grid_alineamiento.Checked = False
                End If
                HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = id_organigrama
                up_date_panel.Update()
                updatemenu.Update()
                Abrir_organigrama = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Abrir_organigrama = "Inconsistencia general función Abrir_organigrama " & ex.Message
        End Try
    End Function
    Function Solicita_diagrama_organigrama(ByVal id_organigrama As Integer, _
                                           ByRef plantilla_organigrama As String) As String
        '------------------------------------------------
        'Función : Retorna el contenido del organigrama
        'Fecha : 2018-05-30
        'Ing : Miguel Angel Urueta Miranda
        '-------------------------------------------------
        Try

            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("plantilla_diagrama_organico")
            Dim sql_consulta As String = "Select Archivo_Plantilla_Mindifucion from plantilla_diagrama_organico " & _
                " where REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA=" & id_organigrama
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_diagrama_organigrama = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_diagrama_organigrama = "Imposible encontrar la plantilla del organigrama "
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    plantilla_organigrama = ""
                Else
                    plantilla_organigrama = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_diagrama_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_diagrama_organigrama = "Inconsistencia general función Solicita_diagrama_organigrama " & ex.Message
        End Try
    End Function
    Function Solicita_areas_relacionadas_a_organigrama(ByVal id_organigrama As Integer, _
                                                      ByRef matri_areas() As stru_area_organigrama) As String
        Try
            Erase matri_areas
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Dim sql_consulta As String = "Select Codigo_Area,Nombre_Area,Descripcion,Estado_Area," _
            & "Codigo_Arbitrario,Consecutivo_Serie,CONSECUTIVO_AREA,Estado_Area_Pqr," & _
            "Estado_Publico_Area,Area_padre from areas_depart_radicacion " & _
                " where Registro_Organigrama_Id_Organigrama=" & id_organigrama
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_areas_relacionadas_a_organigrama = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_areas_relacionadas_a_organigrama = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_areas(i)
                    matri_areas(i).Codigo_Area = Datset.Tables(0).Rows(i).Item(0)
                    matri_areas(i).Nombre_Area = Datset.Tables(0).Rows(i).Item(1)
                    If Datset.Tables(0).Rows(i).IsNull(2) Then
                        matri_areas(i).Descripcion = ""
                    Else
                        matri_areas(i).Descripcion = Datset.Tables(0).Rows(i).Item(2)
                    End If
                    matri_areas(i).Estado_Area = Datset.Tables(0).Rows(i).Item(3)
                    matri_areas(i).Codigo_Arbitrario = Datset.Tables(0).Rows(i).Item(4)
                    matri_areas(i).Consecutivo_Serie = Datset.Tables(0).Rows(i).Item(5)
                    matri_areas(i).CONSECUTIVO_AREA = Datset.Tables(0).Rows(i).Item(6)
                    matri_areas(i).Estado_Area_Pqr = Datset.Tables(0).Rows(i).Item(7)
                    matri_areas(i).Estado_Publico_Area = Datset.Tables(0).Rows(i).Item(8)
                    If Datset.Tables(0).Rows(i).IsNull(9) Then
                        matri_areas(i).Area_padre = 0
                    Else
                        matri_areas(i).Area_padre = Datset.Tables(0).Rows(i).Item(9)
                    End If
                Next
                Solicita_areas_relacionadas_a_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_areas_relacionadas_a_organigrama = "Inconsistencia general función Solicita_areas_relaciondas_a_organigrama " & ex.Message
        End Try
    End Function
    Function Crear_diagrama_organigrama(ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView, _
                                        ByVal matri_areas() As stru_area_organigrama, _
                                        ByRef UpdatePanel_diagran_view As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../configuracion/dlc_flujo.vdx")
            If File.Exists(ruta_plantilla) = False Then
                Crear_diagrama_organigrama = "Imposible encontrar el archivo " & ruta_plantilla
                Exit Function
            End If
            Dim ob As New MindFusion.Diagramming.Import.VisioImporter
            Dim diagran As New MindFusion.Diagramming.DiagramDocument
            Dim digran_page As New MindFusion.Diagramming.DiagramPage
            diagran = ob.Import(ruta_plantilla)
            If diagran.Pages.Count = 0 Then
                Crear_diagrama_organigrama = "La plantilla base no tiene el formato correcto, no contiene por lo menos un diagrama "
                Exit Function
            End If
            DiagramView.Diagram.Items.Clear()
            DiagramView.Diagram = diagran.Pages(0)
            DiagramView.ZoomFactor = 75
            If Not matri_areas Is Nothing Then
                For i As Integer = 0 To matri_areas.Length - 1
                    Result = Me.Agrega_shape_area_diagrama(DiagramView, UpdatePanel_diagran_view, _
                                                         matri_areas(i).Nombre_Area, matri_areas(i).Codigo_Arbitrario, _
                                                         matri_areas(i).Codigo_Area, i)
                    If Result <> "YES" Then
                        Crear_diagrama_organigrama = Result
                        Exit Function
                    End If
                Next
                Crear_diagrama_organigrama = "YES"
                Exit Function
            Else
                Crear_diagrama_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Crear_diagrama_organigrama = "Inconsistencia general función Crear_diagrama_organigrama " & ex.Message
        End Try
    End Function
    Function Agrega_shape_area_diagrama(ByRef DiagramView As MindFusion.Diagramming.WebForms.DiagramView, _
                                        ByRef UpdatePanel_diagran_view As UpdatePanel, _
                                         ByVal nombre_area_departamento As String, ByVal codigo_arbitrario As String, _
                                         ByVal id_area As Integer, ByVal contador As Integer) As String
        '---------------------------------------------------------
        'Función : Agrega un shape a la interface del diagrama del
        'organigrama
        'Fecha : 2018-05-31
        'Ing : Miguel Angel Urueta Miranda
        '---------------------------------------------------------
        Try
            Dim sap As New MindFusion.Diagramming.ShapeNode
            Dim x As Integer = 0
            Dim y As Integer = 0
            '-------------------------------------------
            'Posiciona el shape en el grid del diagrama
            '-------------------------------------------
            x = (40 * contador) + 10
            y = (DiagramView.Diagram.Bounds.Height / 2) - 100
            Dim Rect = New RectangleF(x, y, 35, 35)
            sap.Bounds = Rect
            sap.Id = id_area
            sap.Locked = False
            '-----------------------------------
            'Aplica estilo al shape
            '-----------------------------------
            Dim Refclas_flujo_ruta As New Class_worflow_rutas
            Dim Result As String = ""
            Result = Me.Aplica_estilo_shape_add_diagrama(sap, id_area, nombre_area_departamento, codigo_arbitrario)
            If Result <> "YES" Then
                Agrega_shape_area_diagrama = "Imposible aplicar el estilo al shape, mensaje " & Result
                Exit Function
            End If
            DiagramView.Diagram.Items.Add(sap)
            'UpdatePanel_diagran_view.Update()
            Agrega_shape_area_diagrama = "YES"
        Catch ex As Exception
            Agrega_shape_area_diagrama = "Inconsistencia general función Agrega_shape_area_diagrama " & ex.Message
        End Try
    End Function
    Function Aplica_estilo_shape_add_diagrama(ByRef shape As MindFusion.Diagramming.ShapeNode, _
                                            ByVal id_area_departamento As Integer, _
                                            ByVal nombre_area As String, _
                                            ByVal Codigo_arbitrario As String) As String
        '--------------------------------------------------
        'Función : Aplica estilo del shape
        'Fecha : 2017-06-30
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------
        Try
            shape.AllowOutgoingLinks = False
            Dim Stroke = New MindFusion.Drawing.Pen(Color.Black, 0)
            Dim Fill = New MindFusion.Drawing.SolidBrush(Color.White)
            shape.PolygonalTextLayout = True
            shape.EnableStyledText = True
            Dim form = New StringAlignment
            shape.TextFormat.Alignment = StringAlignment.Center
            Dim ref_tink As MindFusion.Diagramming.Thickness
            ref_tink.Top = 15
            Dim re_trim = Trim(nombre_area)
            re_trim = Trim(re_trim)
            re_trim = re_trim.Replace(vbCrLf, "")
            shape.TextPadding = ref_tink
            shape.Text = re_trim & vbCrLf & "(" & Codigo_arbitrario & ")"
            shape.ToolTip = re_trim
            Dim sysdra = New System.Drawing.Font("Bold", 11)
            shape.Font = sysdra
            Dim penBrush As MindFusion.Drawing.Brush = New MindFusion.Drawing.LinearGradientBrush( _
             Color.White, Color.White, 0)
            shape.Brush = penBrush
            shape.ImageAlign = MindFusion.Drawing.ImageAlign.TopCenter
            shape.ImageUrl = "../workflow/imageneswf/ActividadSistema.png"
            Aplica_estilo_shape_add_diagrama = "YES"
        Catch ex As Exception
            Aplica_estilo_shape_add_diagrama = "Inconsistencia general función   Aplica_estilo_shape_add_diagrama " & ex.Message
        End Try
    End Function
    Function Actualiza_diagrama_organigrama(ByVal string_archivo As String, _
                                            ByVal id_organigrama As Integer) As String
        Dim myConnection As New MySqlConnection
        Try
            Dim Vss_Bynary() As Byte = Nothing
            Dim Result As String = ""
            Dim sql_atualiza_ruta As String = "update plantilla_diagrama_organico set Archivo_Plantilla_Mindifucion = ?imagen  where " _
            & "REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA =" & id_organigrama
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim sqlresultinsert As Integer = 0
            ref.Returna_Conexion_Mysql(myConnection)
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myCommand.Connection = myConnection
            myCommand.CommandText = sql_atualiza_ruta
            myCommand.Parameters.AddWithValue("?imagen", string_archivo)
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_diagrama_organigrama = "Imposible actualizar plantilla organigrama  "
                myConnection.Close()
                Exit Function
            End If
            myConnection.Close()
            Actualiza_diagrama_organigrama = "YES"
        Catch ex As Exception
            Actualiza_diagrama_organigrama = "Inconsistencia general función Actualiza_diagrama_organigrama " & ex.Message
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
        End Try
    End Function
    Function Verifica_Existencia_areadep_Organigrama(ByVal Id_Organigrama As String, _
    ByVal Nombre_Area As String, ByRef Existencia As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Dim Parametro_Consulta As String = "Select Codigo_Area " & _
            " from areas_depart_radicacion where REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA=" & Id_Organigrama & _
            " and NOMBRE_AREA='" & Nombre_Area & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_areadep_Organigrama = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verifica_Existencia_areadep_Organigrama = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verifica_Existencia_areadep_Organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Existencia_areadep_Organigrama = "Inconsistencia General Funcion Verifica_Existencia_areadep_Organigrama " & ex.Message
        End Try
    End Function
    Function Verfica_existencia_codigo_arbitrario(ByVal Id_Organigrama As String, _
    ByVal Codigo_Arbitrario As String, ByRef Existencia As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Dim Parametro_Consulta As String = "Select Codigo_Area " & _
            " from areas_depart_radicacion where REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA=" & Id_Organigrama & _
            " and Codigo_Arbitrario='" & Codigo_Arbitrario & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verfica_existencia_codigo_arbitrario = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verfica_existencia_codigo_arbitrario = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verfica_existencia_codigo_arbitrario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verfica_existencia_codigo_arbitrario = "Incosistencia general función Verfica_existencia_codigo_arbitrario " & ex.Message
        End Try
    End Function
    Function Agregar_AreaDep_Organigrama(ByVal Id_Organigrama As String, _
                                         ByVal Nombre_AreaDep As String, _
                                         ByVal Descripcion As String, _
                                         ByVal Cod_ARB As String, _
                                         ByVal option_pqr As Integer, _
                                         ByVal option_public As Integer, _
                                         ByRef up_date_panel As UpdatePanel, _
                                         ByRef diagramView As Object) As String
        If Nombre_AreaDep = "" Then
            Agregar_AreaDep_Organigrama = "Debe informar el nombre del area"
            Exit Function
        End If
        If Cod_ARB = "" Then
            Agregar_AreaDep_Organigrama = "Debe informar el codigo arbitrario"
            Exit Function
        End If
        If Descripcion = "" Then
            Agregar_AreaDep_Organigrama = "Debe informar la descripción"
            Exit Function
        End If
        Dim Result As String = ""
        Dim existecia_area_departamento As String = ""
        Result = Verifica_Existencia_areadep_Organigrama(HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO"), _
                                                          UCase(Nombre_AreaDep), _
                                                          existecia_area_departamento)
        If Result <> "YES" Then
            Agregar_AreaDep_Organigrama = Result
            Exit Function
        End If
        If existecia_area_departamento = "YES" Then
            Agregar_AreaDep_Organigrama = "Esta intentando registrar el nombre del area o departamento con un nombre existente"
            Exit Function
        End If
        Result = Verfica_existencia_codigo_arbitrario(HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO"), _
                                                      UCase(Cod_ARB), _
                                                      existecia_area_departamento)
        If Result <> "YES" Then
            Agregar_AreaDep_Organigrama = Result
            Exit Function
        End If
        If existecia_area_departamento = "YES" Then
            Agregar_AreaDep_Organigrama = "Esta intentando registrar el código arbitrario con uno existente"
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Agregar_AreaDep_Organigrama = ""
        Try
            Dim mySqldatReader As MySqlDataReader
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = " SELECT CONSECUTIVO_AREA_DEPARTAMENTO FROM REGISTRO_ORGANIGRAMA where ID_ORGANIGRAMA = " _
            & "'" & Id_Organigrama & "' " & "for update"
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Agregar_AreaDep_Organigrama = "Imposible Encontrar Registro En Tabla REGISTRO_ORGANIGRAMA Error Conexion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Agregar_AreaDep_Organigrama = "Imposible Encontrar Registro En Tabla REGISTRO_ORGANIGRAMA"
                'myTrans.Rollback()
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function

            End If
            mySqldatReader.Read()
            Dim prox_consecutivo_area As Integer = mySqldatReader.Item(0)
            prox_consecutivo_area = prox_consecutivo_area + 1
            mySqldatReader.Close()
            Dim Parametro_Actualiza_System1 As String = "update REGISTRO_ORGANIGRAMA set CONSECUTIVO_AREA_DEPARTAMENTO = " & "'" & prox_consecutivo_area & "' " & _
             " where ID_ORGANIGRAMA =" & Id_Organigrama
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            '*********************************
            'Determina si se actualizo
            'el nuevo id de la base de datos
            '*********************************
            If Switc = 0 Then
                Agregar_AreaDep_Organigrama = "Imposible actualizar la tabla REGISTRO_ORGANIGRAMA  : " & Parametro_Actualiza_System1
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim SqlInsert As String = "Insert Into Areas_Depart_Radicacion (Registro_Organigrama_Id_Organigrama," _
            & "Nombre_Area,Descripcion,Estado_Area,Codigo_Arbitrario,Consecutivo_Serie,Estado_Area_Pqr,Estado_Publico_Area) values ('" & _
            Id_Organigrama & "','" & UCase(Nombre_AreaDep) & "','" & UCase(Descripcion) & "','" & "1" & "','" & Cod_ARB & "',0," & option_pqr & _
            "," & option_public & ")"
            myCommand.CommandText = SqlInsert
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_AreaDep_Organigrama = "Imposible agregar area departamento REGISTRO_ORGANIGRAMA  : " & Parametro_Actualiza_System1
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim lasinsert As Object = myCommand.LastInsertedId
            Result = Me.Agrega_shape_area_diagrama(diagramView, _
                                                   up_date_panel, _
                                                   UCase(Nombre_AreaDep), _
                                                   Cod_ARB, lasinsert, 0)
            If Result <> "YES" Then
                Agregar_AreaDep_Organigrama = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim string_diagrama As String = diagramView.SaveToString(SaveToStringFormat.Base64, True)
            Dim update_diagrama As String = "update plantilla_diagrama_organico set Archivo_Plantilla_Mindifucion = ?imagen  where " _
            & "REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA =" & Id_Organigrama
            myCommand.CommandText = update_diagrama
            myCommand.Parameters.AddWithValue("?imagen", string_diagrama)
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_AreaDep_Organigrama = "Imposible actualizar plantilla REGISTRO_ORGANIGRAMA   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            up_date_panel.Update()
            myTrans.Commit()
            Agregar_AreaDep_Organigrama = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Agregar_AreaDep_Organigrama = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Agregar_AreaDep_Organigrama = Agregar_AreaDep_Organigrama
        End Try
    End Function
    Function Editar_area_departamento( _
    ByVal Nombre_AreaDep As TextBox, _
    ByVal Descripcion As TextBox, _
    ByVal Cod_ARB As TextBox, _
    ByVal option_pqr As CheckBox, _
    ByVal option_public As CheckBox, _
    ByRef up_date_panel As UpdatePanel, _
    ByRef modal_popup_extender As AjaxControlToolkit.ModalPopupExtender, _
    ByVal DiagramView As Object) As String
        Dim update_registro As String = "Update areas_depart_radicacion "
        Dim Result As String = ""
        Dim Cambios As String = ""
        If DiagramView.Diagram.Selection.Items.Count = 0 Then
            Editar_area_departamento = "Debe seleccionar la actividad a editar"
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count > 1 Then
            Editar_area_departamento = "Por favor seleccione una sola actividad para editar"
            Exit Function
        End If
        Dim obshape As Object = DiagramView.Diagram.Selection.Items(0)
        Dim ob As Object = obshape.GetType
        If ob.Fullname <> "MindFusion.Diagramming.ShapeNode" Then
            Editar_area_departamento = "Debe seleccionar la actividad a editar"
            Exit Function
        End If
        Dim activo_pqr As Integer = 0
        Dim activo_publico As Integer = 0
        If option_pqr.Checked = True Then
            activo_pqr = 1
        End If
        If option_public.Checked = True Then
            activo_publico = 1
        End If
        Dim stru_area_organigrama As stru_area_organigrama = Nothing
        Result = Me.Solicita_datos_caracterizacion_area(obshape.id, stru_area_organigrama)
        If Result <> "YES" Then
            Editar_area_departamento = Result
            Exit Function
        End If
        Dim existecia_area_departamento As String = ""
        If UCase(stru_area_organigrama.Nombre_Area) <> UCase(Nombre_AreaDep.Text) Then
            Result = Verifica_Existencia_areadep_Organigrama(HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO"), _
                                                           UCase(Nombre_AreaDep.Text), existecia_area_departamento)
            If Result <> "YES" Then
                Editar_area_departamento = Result
                Exit Function
            End If
            If existecia_area_departamento = "YES" Then
                Editar_area_departamento = "Esta intentando actualizar el nombre del area con uno existente"
                Exit Function
            End If
            If update_registro = "Update areas_depart_radicacion " Then
                update_registro = update_registro & " set Nombre_Area='" & UCase(Nombre_AreaDep.Text) & "'"
            End If
            Cambios = Cambios & " Cambio nombre área departamento valor actual " & UCase(stru_area_organigrama.Nombre_Area) & " Nuevo valor " & UCase(Nombre_AreaDep.Text)
        End If
        If UCase(stru_area_organigrama.Codigo_Arbitrario) <> UCase(Cod_ARB.Text) Then
            Result = Verfica_existencia_codigo_arbitrario(HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO"), _
                                                           UCase(Cod_ARB.Text), existecia_area_departamento)
            If Result <> "YES" Then
                Editar_area_departamento = Result
                Exit Function
            End If
            If existecia_area_departamento = "YES" Then
                Editar_area_departamento = "Esta intentando actualizar el código arbitrario con uno existente"
                Exit Function
            End If
            If update_registro = "Update areas_depart_radicacion " Then
                update_registro = update_registro & " set Codigo_Arbitrario='" & UCase(Cod_ARB.Text) & "'"
            Else
                update_registro = update_registro & ", Codigo_Arbitrario='" & UCase(Cod_ARB.Text) & "'"
            End If
            Cambios = Cambios & " Cambio código arbitrario área departamento valor actual " & UCase(stru_area_organigrama.Codigo_Arbitrario) & " Nuevo valor " & UCase(Cod_ARB.Text)
        End If
        If UCase(stru_area_organigrama.Descripcion) <> UCase(Descripcion.Text) Then
            If update_registro = "Update areas_depart_radicacion " Then
                update_registro = update_registro & " set Descripcion='" & UCase(Descripcion.Text) & "'"
            Else
                update_registro = update_registro & ", Descripcion='" & UCase(Descripcion.Text) & "'"
            End If
            Cambios = Cambios & " Cambio descripción área departamento valor actual " & UCase(stru_area_organigrama.Descripcion) & " Nuevo valor " & UCase(Descripcion.Text)
        End If
        If stru_area_organigrama.Estado_Area_Pqr <> activo_pqr Then
            If update_registro = "Update areas_depart_radicacion " Then
                update_registro = update_registro & " set Estado_Area_Pqr='" & activo_pqr & "'"
            Else
                update_registro = update_registro & ", Estado_Area_Pqr='" & activo_pqr & "'"
            End If
            Cambios = Cambios & " Cambio estado pqr área departamento valor actual " & stru_area_organigrama.Estado_Area_Pqr & " Nuevo valor " & activo_pqr
        End If
        If stru_area_organigrama.Estado_Publico_Area <> activo_publico Then
            If update_registro = "Update areas_depart_radicacion " Then
                update_registro = update_registro & " set Estado_Publico_Area='" & activo_publico & "'"
            Else
                update_registro = update_registro & ", Estado_Publico_Area='" & activo_publico & "'"
            End If
            Cambios = Cambios & " Cambio estado público área departamento valor actual " & stru_area_organigrama.Estado_Publico_Area & " Nuevo valor " & activo_publico
        End If
        If update_registro = "Update areas_depart_radicacion " Then
            Editar_area_departamento = "No se detectaron cambios para el área o departamento a editar"
            Exit Function
        Else
            update_registro = update_registro & " where Codigo_Area='" & obshape.id & "'"
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = Date.Now
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Editar_area_departamento = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "EDITAR AREA"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "EDITA AREA   (" & _
        " ID AREA " & obshape.id & Cambios & ")"
        isert_datos = isert_datos & "(" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date1al & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO Ra_log_organigrama (registro_organigrama_ID_ORGANIGRAMA,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Editar_area_departamento = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Editar_area_departamento = "Imposible actualizar la tabla REGISTRO_ORGANIGRAMA   "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            
            Result = Me.Actualiza_shape_organigrama(obshape, obshape.id, UCase(Nombre_AreaDep.Text), Cod_ARB.Text)
            If Result <> "YES" Then
                Editar_area_departamento = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim string_diagrama As String = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            Dim update_diagrama As String = "update plantilla_diagrama_organico set Archivo_Plantilla_Mindifucion = ?imagen  where " _
            & "REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA =" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO")
            myCommand.CommandText = update_diagrama
            myCommand.Parameters.AddWithValue("?imagen", string_diagrama)
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Editar_area_departamento = "Imposible actualizar plantilla REGISTRO_ORGANIGRAMA   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Editar_area_departamento = "Imposible actualizar log organigrama   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            up_date_panel.Update()
            myTrans.Commit()
            modal_popup_extender.Hide()
            Editar_area_departamento = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Editar_area_departamento = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Editar_area_departamento = Editar_area_departamento
        End Try

    End Function
    Function Actualiza_shape_organigrama(ByRef shape As MindFusion.Diagramming.ShapeNode, _
                                            ByVal id_area_departamento As Integer, _
                                            ByVal nombre_area As String, _
                                            ByVal Codigo_arbitrario As String) As String
        Try
            Dim ref_tink As MindFusion.Diagramming.Thickness
            ref_tink.Top = 15
            Dim re_trim = Trim(nombre_area)
            re_trim = Trim(re_trim)
            re_trim = re_trim.Replace(vbCrLf, "")
            shape.TextPadding = ref_tink
            shape.Text = re_trim & vbCrLf & "(" & Codigo_arbitrario & ")"
            shape.ToolTip = re_trim
            Actualiza_shape_organigrama = "YES"
        Catch ex As Exception
            Actualiza_shape_organigrama = "Inconsistencia general función Actualiza_shape_organigrama " & ex.Message
        End Try
    End Function
    Function Activa_editar_area_departamento( _
    ByVal Nombre_AreaDep As TextBox, ByVal Descripcion As TextBox, _
    ByVal Cod_ARB As TextBox, _
    ByVal option_pqr As CheckBox, _
    ByVal option_public As CheckBox, _
    ByRef up_date_panel As UpdatePanel, _
    ByRef modal_popup_extender As AjaxControlToolkit.ModalPopupExtender, _
    ByVal DiagramView As Object) As String
        Try
            Dim Result As String = ""
            If DiagramView.Diagram.Selection.Items.Count = 0 Then
                Activa_editar_area_departamento = "Debe seleccionar la actividad a eliminar"
                Exit Function
            End If
            If DiagramView.Diagram.Selection.Items.Count > 1 Then
                Activa_editar_area_departamento = "Por favor seleccione una sola actividad para eliminar"
                Exit Function
            End If
            Dim obshape As Object = DiagramView.Diagram.Selection.Items(0)
            Dim ob As Object = obshape.GetType
            If ob.Fullname <> "MindFusion.Diagramming.ShapeNode" Then
                Activa_editar_area_departamento = "Debe seleccionar la actividad a eliminar"
                Exit Function
            End If
            Dim stru_area_organigrama As stru_area_organigrama = Nothing
            Result = Me.Solicita_datos_caracterizacion_area(obshape.id, stru_area_organigrama)
            If Result <> "YES" Then
                Activa_editar_area_departamento = Result
                Exit Function
            End If
            Result = Me.Lista_datos_interface_caraterizacion_organigrama( _
            Nombre_AreaDep, Descripcion, _
            Cod_ARB, _
            option_pqr, _
            option_public, _
            up_date_panel, _
            modal_popup_extender, _
            stru_area_organigrama)
            If Result <> "YES" Then
                Activa_editar_area_departamento = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("ORG_ID_AREA_ACTIVA") = obshape.id
            Activa_editar_area_departamento = "YES"
        Catch ex As Exception
            Activa_editar_area_departamento = "Inconsistencia general función Activa_editar_area_departamento " & ex.Message
        End Try
    End Function
    Function Solicita_datos_caracterizacion_area(ByVal id_area As Integer, _
                                             ByRef stru_area_organigrama As stru_area_organigrama) As String
        '---------------------------------------------- 
        'Función : Solicita datos de caracterización
        'area organigrama
        'Fecha : 2018-06-02
        'Ing : Miguel Angel Urueta Miranda 
        '---------------------------------------------- 
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Dim sql_consulta As String = "Select Codigo_Area,Nombre_Area,Descripcion," & _
                "Estado_Area,Codigo_Arbitrario,Consecutivo_Serie,CONSECUTIVO_AREA," & _
                "Estado_Area_Pqr,Estado_Publico_Area,Area_padre" & _
                " from areas_depart_radicacion " & _
                " where Codigo_Area=" & id_area
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_caracterizacion_area = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_caracterizacion_area = "Imposible enoncontrar datos de caracterización del área " & id_area
                Exit Function
            Else
                stru_area_organigrama.Codigo_Area = Datset.Tables(0).Rows(0).Item(0)
                stru_area_organigrama.Nombre_Area = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).IsNull(2) Then
                    stru_area_organigrama.Descripcion = ""
                Else
                    stru_area_organigrama.Descripcion = Datset.Tables(0).Rows(0).Item(2)
                End If
                stru_area_organigrama.Estado_Area = Datset.Tables(0).Rows(0).Item(3)
                stru_area_organigrama.Codigo_Arbitrario = Datset.Tables(0).Rows(0).Item(4)
                stru_area_organigrama.Consecutivo_Serie = Datset.Tables(0).Rows(0).Item(5)
                stru_area_organigrama.CONSECUTIVO_AREA = Datset.Tables(0).Rows(0).Item(6)
                stru_area_organigrama.Estado_Area_Pqr = Datset.Tables(0).Rows(0).Item(7)
                stru_area_organigrama.Estado_Publico_Area = Datset.Tables(0).Rows(0).Item(8)
                If Datset.Tables(0).Rows(0).IsNull(9) Then
                    stru_area_organigrama.Area_padre = 0
                Else
                    stru_area_organigrama.Area_padre = Datset.Tables(0).Rows(0).Item(9)
                End If
                Solicita_datos_caracterizacion_area = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_caracterizacion_area = "Incosistencia general función Solicita_datos_caracterizacion_area " & ex.Message
        End Try
    End Function
    Function Lista_datos_interface_caraterizacion_organigrama( _
    ByVal Nombre_AreaDep As TextBox, ByVal Descripcion As TextBox, _
    ByVal Cod_ARB As TextBox, _
    ByVal option_pqr As CheckBox, _
    ByVal option_public As CheckBox, _
    ByRef up_date_panel As UpdatePanel, _
    ByRef modal_popup_extender As AjaxControlToolkit.ModalPopupExtender, _
    ByVal stru_area_organigrama As stru_area_organigrama) As String
        Try
            Nombre_AreaDep.Text = stru_area_organigrama.Nombre_Area
            Descripcion.Text = stru_area_organigrama.Descripcion
            Cod_ARB.Text = stru_area_organigrama.Codigo_Arbitrario
            If stru_area_organigrama.Estado_Area_Pqr = 1 Then
                option_pqr.Checked = True
            Else
                option_pqr.Checked = False
            End If
            If stru_area_organigrama.Estado_Publico_Area = 1 Then
                option_public.Checked = True
            Else
                option_public.Checked = False
            End If
            up_date_panel.Update()
            modal_popup_extender.Show()
            Lista_datos_interface_caraterizacion_organigrama = "YES"
            Exit Function
        Catch ex As Exception
            Lista_datos_interface_caraterizacion_organigrama = "Inconsistencia general función Lista_datos_interface_caraterizacion_organigrama " & ex.Message
        End Try
    End Function
    Function Eliminar_area_departamento(ByVal id_area As Integer, _
                                        ByRef up_date_panel As UpdatePanel, _
                                        ByVal DiagramView As Object) As String
        If DiagramView.Diagram.Selection.Items.Count = 0 Then
            Eliminar_area_departamento = "Debe seleccionar la actividad a eliminar"
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count > 1 Then
            Eliminar_area_departamento = "Por favor seleccione una sola actividad para eliminar"
            Exit Function
        End If
        Dim obshape As Object = DiagramView.Diagram.Selection.Items(0)
        Dim ob As Object = obshape.GetType
        If ob.Fullname <> "MindFusion.Diagramming.ShapeNode" Then
            Eliminar_area_departamento = "Debe seleccionar la actividad a eliminar"
            Exit Function
        End If
        Dim Result As String = ""
        Dim existencia As String = ""
        Result = Me.Verfica_existencia_area_relacion_usuario(id_area, existencia)
        If Result <> "YES" Then
            Eliminar_area_departamento = Result
            Exit Function
        End If
        If existencia = "YES" Then
            Eliminar_area_departamento = "Imposible eliminar el área o departamento, usuarios relacionados al área"
            Exit Function
        End If
        Result = Me.Verifica_series_relacionadas_al_area(id_area, existencia)
        If Result <> "YES" Then
            Eliminar_area_departamento = Result
            Exit Function
        End If
        If existencia = "YES" Then
            Eliminar_area_departamento = "Imposible eliminar el área o departamento, series relacionadas al área"
            Exit Function
        End If
        Result = Me.Solicita_registro_como_padre(id_area, existencia)
        If Result <> "YES" Then
            Eliminar_area_departamento = Result
            Exit Function
        End If
        If existencia = "YES" Then
            Eliminar_area_departamento = "Imposible eliminar el área o departamento, área relacionada con padre de una sub área"
            Exit Function
        End If
        Result = Me.Solicita_registro_como_hijo(id_area, existencia)
        If Result <> "YES" Then
            Eliminar_area_departamento = Result
            Exit Function
        End If
        If existencia = "YES" Then
            Eliminar_area_departamento = "Imposible eliminar el área o departamento, area relacionada con padre de una sub área"
            Exit Function
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = Date.Now
        Dim Refclas As New ClassListandoTareas
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Eliminar_area_departamento = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "ELIMINAR AREA"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "ELIMINA AREA   (" & _
        " ID AREA " & obshape.id & " NOMBRE " & obshape.text & ")"
        isert_datos = isert_datos & "(" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date1al & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO Ra_log_organigrama (registro_organigrama_ID_ORGANIGRAMA,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim Delete As String = "Delete " & _
           " from AREAS_DEPART_RADICACION where CODIGO_AREA=" & id_area
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Eliminar_area_departamento = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = Delete
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_area_departamento = "Imposible eliminar area departamento   "
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_area_departamento = "Imposible actualizar log organigrama   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            DiagramView.Diagram.Items.Remove(obshape)
            Dim string_diagrama As String = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            Dim update_diagrama As String = "update plantilla_diagrama_organico set Archivo_Plantilla_Mindifucion = ?imagen  where " _
            & "REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA =" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO")
            myCommand.CommandText = update_diagrama
            myCommand.Parameters.AddWithValue("?imagen", string_diagrama)
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_area_departamento = "Imposible actualizar plantilla REGISTRO_ORGANIGRAMA   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            up_date_panel.Update()
            myTrans.Commit()
            Eliminar_area_departamento = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_area_departamento = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Eliminar_area_departamento = Eliminar_area_departamento
        End Try
    End Function
    Function Verfica_existencia_area_relacion_usuario(ByVal id_area As Integer, _
                                                      ByRef existencia As String) As String
        '-----------------------------------------------
        'Función : Verifica la existencia de usuarios
        'relacionados al area
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2018-06-06
        '-----------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Dim Parametro_Consulta As String = "Select Areas_Dep_Radicacion_id_Areas_Dep " & _
            " from remit_dest_interno where Areas_Dep_Radicacion_id_Areas_Dep=" & id_area
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verfica_existencia_area_relacion_usuario = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Verfica_existencia_area_relacion_usuario = "YES"
                Exit Function
            Else
                existencia = "YES"
                Verfica_existencia_area_relacion_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verfica_existencia_area_relacion_usuario = "Inconsistencia general función Verfica_existencia_area_relacion_usuario " & ex.Message
        End Try
    End Function
    Function Verifica_series_relacionadas_al_area(ByVal id_area As Integer, _
                                                      ByRef existencia As String) As String
        '-------------------------------------------------
        'Función : Verfica existencia series relacionadas
        'al area
        'Fecha : 2018-06-06
        'Ingeniero : Miguel Angel Urueta Miranda
        '-------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Parametro_Consulta As String = "Select * " & _
            " from SERIES_DOCUMENTALES where AREAS_DEPART_RADICACION_CODIGO_AREA=" & id_area
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_series_relacionadas_al_area = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Verifica_series_relacionadas_al_area = "YES"
                Exit Function
            Else
                existencia = "YES"
                Verifica_series_relacionadas_al_area = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_series_relacionadas_al_area = "Inconsistencia general función Verifica_series_relacionadas_al_area " & ex.Message
        End Try
    End Function
    Function Solicita_registro_como_padre(ByVal id_area As Integer, _
                                         ByRef existencia As String) As String
        '--------------------------------------------
        'Function : Verifica existencia de area
        'relacionada como padre
        'Fecha : 2018-06-06
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_org_relacion_areas_depart_radicacion")
            Dim Parametro_Consulta As String = "Select * " & _
            " from ra_org_relacion_areas_depart_radicacion where areas_depart_radicacion_Codigo_Area=" & id_area
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_registro_como_padre = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Solicita_registro_como_padre = "YES"
                Exit Function
            Else
                existencia = "YES"
                Solicita_registro_como_padre = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_registro_como_padre = "Inconsistencia general función Solicita_registro_como_padre " & ex.Message
        End Try
    End Function
    Function Solicita_registro_como_hijo(ByVal id_area As Integer, _
                                         ByRef existencia As String) As String
        '--------------------------------------------
        'Function : Verifica existencia de area
        'relacionada como hijo
        'Fecha : 2018-06-06
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_org_relacion_areas_depart_radicacion")
            Dim Parametro_Consulta As String = "Select * " & _
            " from ra_org_relacion_areas_depart_radicacion where id_area_hijo=" & id_area
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_registro_como_hijo = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                Solicita_registro_como_hijo = "YES"
                Exit Function
            Else
                existencia = "YES"
                Solicita_registro_como_hijo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_registro_como_hijo = "Inconsistencia general función Solicita_registro_como_padre " & ex.Message
        End Try
    End Function
    Function Asigna_datos_estado_interface_area_departamento(ByVal id_area As Integer, _
                                                   ByRef chek_option_activa As CheckBox, _
                                                   ByRef chek_option_inactiva As CheckBox, _
                                                   ByRef ref_update As UpdatePanel) As String
        Try
            Dim stru_area_organigrama As stru_area_organigrama = Nothing
            Dim Result As String = Me.Solicita_datos_caracterizacion_area(id_area, stru_area_organigrama)
            If Result <> "YES" Then
                Asigna_datos_estado_interface_area_departamento = Result
                Exit Function
            Else
                If stru_area_organigrama.Estado_Area = 1 Then
                    chek_option_activa.Checked = True
                    chek_option_inactiva.Checked = False
                Else
                    chek_option_activa.Checked = False
                    chek_option_inactiva.Checked = True
                End If
                ref_update.Update()
                Asigna_datos_estado_interface_area_departamento = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Asigna_datos_estado_interface_area_departamento = "Inconsistencia general función Asigna_datos_estado_area_departamento " & ex.Message
        End Try
    End Function
    Function Cambia_estado_area_departamento(ByRef chek_option_activa As CheckBox, _
                                                   ByRef chek_option_inactiva As CheckBox, _
                                                   ByRef ref_update As UpdatePanel, _
                                                   ByRef DiagramView As Object) As String

        Dim Result As String = ""
        If DiagramView.Diagram.Selection.Items.Count = 0 Then
            Cambia_estado_area_departamento = "Debe seleccionar la actividad para cambiar el estado"
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count > 1 Then
            Cambia_estado_area_departamento = "Por favor seleccione una sola actividad para cambiar el estado"
            Exit Function
        End If
        Dim obshape As Object = DiagramView.Diagram.Selection.Items(0)
        Dim ob As Object = obshape.GetType
        If ob.Fullname <> "MindFusion.Diagramming.ShapeNode" Then
            Cambia_estado_area_departamento = "Debe seleccionar la actividad para cambiar el estado"
            Exit Function
        End If
        Dim estado_area As Integer = 0
        Dim tipo_cambio As String = ""
        If chek_option_activa.Checked = True Then
            estado_area = 1
            tipo_cambio = "ACTIVO"
        Else
            tipo_cambio = "INAACTIVO"
            estado_area = 0
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = Date.Now
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Cambia_estado_area_departamento = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "CAMBIO ESTADO AREA"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "CAMBIO ESTADO AREA   (" & _
        " ID AREA " & obshape.id & " NUEVO ESTADO " & tipo_cambio & ")"
        isert_datos = isert_datos & "(" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date1al & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO Ra_log_organigrama (registro_organigrama_ID_ORGANIGRAMA,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim update_registro As String = "update areas_depart_radicacion set Estado_Area=" & estado_area & " Where Codigo_Area=" & obshape.id
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Cambia_estado_area_departamento = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_area_departamento = "Imposible actualizar el estado del área o departamento   "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim penBrush As MindFusion.Drawing.Brush 
            If estado_area = 1 Then
                penBrush = New MindFusion.Drawing.LinearGradientBrush( _
             Color.White, Color.White, 0)
            Else
                penBrush = New MindFusion.Drawing.LinearGradientBrush( _
            Color.Gray, Color.Gray, 0)
            End If
            obshape.Brush = penBrush
            Dim string_diagrama As String = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            Dim update_diagrama As String = "update plantilla_diagrama_organico set Archivo_Plantilla_Mindifucion = ?imagen  where " _
            & "REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA =" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO")
            myCommand.CommandText = update_diagrama
            myCommand.Parameters.AddWithValue("?imagen", string_diagrama)
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_area_departamento = "Imposible actualizar plantilla REGISTRO_ORGANIGRAMA   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_area_departamento = "Imposible actualizar log organigrama   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            ref_update.Update()
            myTrans.Commit()
            Cambia_estado_area_departamento = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_estado_area_departamento = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Cambia_estado_area_departamento = Cambia_estado_area_departamento
        End Try
    End Function
    Function Agregar_sub_area_departamento(ByRef ref_update As UpdatePanel, _
                                           ByRef DiagramView As Object, _
                                           ByVal tipo_relacion As Integer) As String
        Dim Result As String = ""
        Dim HiddenField_value_selecion As Object = Nothing
        HiddenField_value_selecion = ref_update.FindControl("HiddenField_value_selecion")
        If HiddenField_value_selecion Is Nothing Then
            Agregar_sub_area_departamento = "Imposible encontrar el control selección HiddenField_value_selecion"
            Exit Function
        End If
        If HiddenField_value_selecion.Value = "" Then
            Agregar_sub_area_departamento = "Debe seleccionar dos actividades en el diagrama como mínimo para conectar "
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count = 0 Then
            Agregar_sub_area_departamento = "Debe seleccionar las actividades para agregar la sub área"
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count <> 2 Then
            Agregar_sub_area_departamento = "Por favor seleccione las dos actividades para agregar la sub área"
            Exit Function
        End If
        Dim Matshape() As Object
        Erase Matshape
        'Dim split() As String = HiddenField_value_selecion.Value.Split("|")
        'For i As Integer = 0 To split.Length - 1
        'If Split(0) = Split(1) Then
        'Agregar_sub_area_departamento = "La actividad destino no puede ser la misma actividad de inicio "
        'Exit Function
        'End If
        Dim i As Integer = 0
        For Each sha_ As Object In DiagramView.Diagram.Selection.Items
            Dim ob As Object = sha_.GetType
            If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                ReDim Preserve Matshape(i)
                Matshape(i) = sha_
                i = i + 1
            End If
        Next
        'Next
        If Matshape Is Nothing Then
            Agregar_sub_area_departamento = "Debe seleccionar dos actividades como mínimo para conectar "
            Exit Function
        End If
        If Matshape.Length = 1 Then
            Agregar_sub_area_departamento = "Debe seleccionar dos actividades como mínimo para conectar "
            Exit Function
        End If
        If Matshape.Length > 2 Then
            Agregar_sub_area_departamento = "Solo debe seleccionar dos actividades al mismo tiempo para conectar "
            Exit Function
        End If
        Dim id_actividad_destino As Integer = Val(Matshape(1).id)
        Dim id_actividad_fuente As Integer = Val(Matshape(0).id)
        Dim id_relacion As Integer = 0
        Result = Me.Solicita_relacion_area_con_sub_area(id_actividad_fuente, _
                                                        id_actividad_destino, _
                                                        id_relacion)
        If Result <> "YES" Then
            Agregar_sub_area_departamento = Result
            Exit Function
        End If
        If id_relacion <> 0 Then
            Agregar_sub_area_departamento = "Existe una relación, imposible conectar"
            Exit Function
        End If
        Result = Me.Solicita_relacion_area_con_sub_area(id_actividad_destino, _
                                                        id_actividad_fuente, _
                                                        id_relacion)
        If Result <> "YES" Then
            Agregar_sub_area_departamento = Result
            Exit Function
        End If
        If id_relacion <> 0 Then
            Agregar_sub_area_departamento = "Existe una relación, imposible conectar"
            Exit Function
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = Date.Now
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Agregar_sub_area_departamento = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "AGREGA RELACION SUB AREA"
        If tipo_relacion <> 1 Then
            detalle_trans = "AGREGA RELACION DE JERARQUIA"
        End If
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = detalle_trans & " " & id_actividad_destino & "  (" & _
        " RELACIONA AREA " & id_actividad_fuente & " CON EL AREA " & id_actividad_destino & ")"
        isert_datos = isert_datos & "(" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date1al & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO Ra_log_organigrama (registro_organigrama_ID_ORGANIGRAMA,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim update_registro As String = "update areas_depart_radicacion set Area_padre=" & id_actividad_fuente & " Where Codigo_Area=" & id_actividad_destino
        Dim insert_relacion As String = "Insert into ra_org_relacion_areas_depart_radicacion (areas_depart_radicacion_Codigo_Area,id_area_hijo, tipo_relacion,ID_ORGANIGRAMA_REF) values (" & _
        id_actividad_fuente & "," & id_actividad_destino & "," & tipo_relacion & "," & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") & ")"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Agregar_sub_area_departamento = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_sub_area_departamento = "Imposible actualizar el estado del área o departamento   "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = insert_relacion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_sub_area_departamento = "Imposible registrar la relación de area padre y sub area   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim last_insert As Object = myCommand.LastInsertedId
            Dim link As MindFusion.Diagramming.DiagramLink
            link = DiagramView.Diagram.Factory.CreateDiagramLink(Matshape(0), Matshape(1))
            'link.Text = Matshape(0).Text & "->" & Matshape(1).text
            link.AutoRoute = False
            link.AutoSnapToNode = False
            'EVITA QUE SE MUEVA EL CONECTOR FINAL
            link.AllowMoveEnd = False
            link.DrawCrossings = False
            link.CrossingRadius = 5
            link.DrawCrossings = True
            link.Id = last_insert
            Dim Fill = New MindFusion.Drawing.SolidBrush(Color.Yellow)
            link.HeadBrush = Fill
            link.AutoRoute = True
            Dim penBrush As MindFusion.Drawing.Brush
            penBrush = New MindFusion.Drawing.LinearGradientBrush( _
            Color.LightBlue, Color.LightBlue, 0)
            If tipo_relacion = 1 Then
                Matshape(1).Brush = penBrush
            End If
            Dim string_diagrama As String = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            Dim update_diagrama As String = "update plantilla_diagrama_organico set Archivo_Plantilla_Mindifucion = ?imagen  where " _
            & "REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA =" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO")
            myCommand.CommandText = update_diagrama
            myCommand.Parameters.AddWithValue("?imagen", string_diagrama)
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_sub_area_departamento = "Imposible actualizar plantilla REGISTRO_ORGANIGRAMA   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_sub_area_departamento = "Imposible actualizar log organigrama   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            ref_update.Update()
            myTrans.Commit()
            Agregar_sub_area_departamento = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Agregar_sub_area_departamento = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Agregar_sub_area_departamento = Agregar_sub_area_departamento
        End Try
    End Function
    Function Solicita_relacion_area_con_sub_area(ByVal id_area_padre As Integer, _
                                                 ByVal id_area_hijo As Integer, _
                                                 ByRef id_relacion As Integer) As String
        '--------------------------------------------
        'Function : Verifica existencia de area
        'relacionada 
        'Fecha : 2018-06-10
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_org_relacion_areas_depart_radicacion")
            Dim Parametro_Consulta As String = "Select * " & _
            " from ra_org_relacion_areas_depart_radicacion where id_area_hijo=" & id_area_hijo & _
            " and areas_depart_radicacion_Codigo_Area=" & id_area_padre
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_relacion_area_con_sub_area = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_relacion = 0
                Solicita_relacion_area_con_sub_area = "YES"
                Exit Function
            Else
                id_relacion = Datset.Tables(0).Rows(0).Item(0)
                Solicita_relacion_area_con_sub_area = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_relacion_area_con_sub_area = "Inconsistencia general función Solicita_relacion_area_con_sub_area " & ex.Message
        End Try

    End Function
    Function Elimina_relacion_sub_area_departamento(ByRef ref_update As UpdatePanel, _
                                           ByRef DiagramView As Object) As String
        Dim Result As String = ""
        If DiagramView.Diagram.Selection.Items.Count = 0 Then
            Elimina_relacion_sub_area_departamento = "Debe seleccionar el conector para eliminar la relación de sub area o departamento"
            Exit Function
        End If
        If DiagramView.Diagram.Selection.Items.Count > 1 Then
            Elimina_relacion_sub_area_departamento = "Solo puede eliminar una relación al mismo tiempo "
            Exit Function
        End If
        Dim link As MindFusion.Diagramming.DiagramLink = Nothing
        For Each sha_ As Object In DiagramView.Diagram.Selection.Items
            Dim ob As Object = sha_.GetType
            If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                Elimina_relacion_sub_area_departamento = "Debe seleccionar un elemento de tipo conector para eliminar la relación "
                Exit Function
            Else
                link = sha_
            End If
        Next
        'link.Destination
        Dim id_actividad_destino As Integer = 0
        Dim id_actividad_fuente As Integer = 0
        Dim id_tipo_relacion As Integer = 0
        If link.Id <> "-1" Then
            Result = Me.Solicita_datos_area_relacion_sub_area(link.Id, id_actividad_fuente, id_actividad_destino, id_tipo_relacion)
            If Result <> "YES" Then
                Elimina_relacion_sub_area_departamento = Result
                Exit Function
            End If
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = Date.Now
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Elimina_relacion_sub_area_departamento = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "ELIMINA RELACION SUB AREA"
        If id_tipo_relacion <> 1 Then
            detalle_trans = "ELIMINA RELACION JERARQUIA"
        End If
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = detalle_trans & " " & id_actividad_destino & "  (" & _
        " ELIMINA RELACION AREA " & id_actividad_fuente & " CON EL AREA " & id_actividad_destino & ")"
        isert_datos = isert_datos & "(" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date1al & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO Ra_log_organigrama (registro_organigrama_ID_ORGANIGRAMA,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim update_registro As String = "update areas_depart_radicacion set Area_padre=Null" & " Where Codigo_Area=" & id_actividad_destino
        Dim elimina_relacion As String = "Delete from ra_org_relacion_areas_depart_radicacion where id_relacion=" & link.Id
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Elimina_relacion_sub_area_departamento = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_relacion_sub_area_departamento = "Imposible actualizar el estado del área o departamento   "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = elimina_relacion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_relacion_sub_area_departamento = "Imposible eliminar la relación de área padre y sub área"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If           
            Dim penBrush As MindFusion.Drawing.Brush
            penBrush = New MindFusion.Drawing.LinearGradientBrush( _
            Color.White, Color.White, 0)
            link.Destination.Brush = penBrush
            DiagramView.Diagram.Items.Remove(link)
            Dim string_diagrama As String = DiagramView.SaveToString(SaveToStringFormat.Base64, True)
            Dim update_diagrama As String = "update plantilla_diagrama_organico set Archivo_Plantilla_Mindifucion = ?imagen  where " _
            & "REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA =" & HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO")
            myCommand.CommandText = update_diagrama
            myCommand.Parameters.AddWithValue("?imagen", string_diagrama)
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_relacion_sub_area_departamento = "Imposible actualizar plantilla REGISTRO_ORGANIGRAMA   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_relacion_sub_area_departamento = "Imposible actualizar log organigrama   "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            ref_update.Update()
            myTrans.Commit()
            Elimina_relacion_sub_area_departamento = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Elimina_relacion_sub_area_departamento = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Elimina_relacion_sub_area_departamento = Elimina_relacion_sub_area_departamento
        End Try
    End Function
    Function Solicita_datos_area_relacion_sub_area(ByVal id_relacion As Integer, _
                                                   ByRef id_padre As Integer, _
                                                   ByRef id_hijo As Integer, _
                                                   ByRef tipo_relacion As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_org_relacion_areas_depart_radicacion")
            Dim Parametro_Consulta As String = "Select areas_depart_radicacion_Codigo_Area,id_area_hijo,tipo_relacion " & _
            " from ra_org_relacion_areas_depart_radicacion where id_relacion=" & id_relacion
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_area_relacion_sub_area = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_area_relacion_sub_area = "Imposible encontrar la relación (" & id_relacion & ")"
                Exit Function
            Else
                id_padre = Datset.Tables(0).Rows(0).Item(0)
                id_hijo = Datset.Tables(0).Rows(0).Item(1)
                tipo_relacion = Datset.Tables(0).Rows(0).Item(2)
                Solicita_datos_area_relacion_sub_area = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_area_relacion_sub_area = "Inconsistencia general función Solicita_datos_area_relacion_sub_area " & ex.Message
        End Try
    End Function
    Function Agregar_Organigrama(ByVal nombre_organigrama As String, _
                                 ByVal numero_resolucion As String, _
                                 ByVal detalle_resolucion As String, _
                                 ByVal version_organigrama As String, _
                                 ByVal codigo_iso_organigrama As String, _
                                 ByVal fecha_organigrama As String, _
                                 ByRef drop_list As DropDownList, _
                                 ByRef update As UpdatePanel) As String

        If nombre_organigrama = "" Then
            Agregar_Organigrama = "Debe informar el nombre del nuevo organigrama"
            Exit Function
        End If
        If numero_resolucion = "" Then
            Agregar_Organigrama = "Debe informar el código de la resolución o acto administrativo del nuevo organigrama"
            Exit Function
        End If
        If detalle_resolucion = "" Then
            Agregar_Organigrama = "Debe informar la descripción de la resolución o acto administrativo del nuevo organigrama"
            Exit Function
        End If
        If fecha_organigrama = "" Then
            Agregar_Organigrama = "Debe informar la fecha creación del nuevo organigrama"
            Exit Function
        End If
        Dim id_empresa As Integer = 0
        Dim Result As String = ""
        Result = Me.Solicita_id_empresa_gestion_documental(id_empresa)
        If Result <> "YES" Then
            Agregar_Organigrama = Result
            Exit Function
        End If
        Dim confirm As Boolean = True
        Result = Me.Verifica_existencia_organigrama(nombre_organigrama, id_empresa, confirm)
        If Result <> "YES" Then
            Agregar_Organigrama = Result
            Exit Function
        End If
        If confirm = True Then
            Agregar_Organigrama = "Estar intentando registrar el organigrama " & nombre_organigrama & " que ya se encuentra registrado, imposible continuar"
            Exit Function
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = Date.Now
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Agregar_Organigrama = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim ref_version_organigrama As String = Left(date1al.ToString, 10)
        ref_version_organigrama = ref_version_organigrama.ToString.Replace("/", "")
        ref_version_organigrama = ref_version_organigrama.ToString.Replace("-", "")
        If version_organigrama <> "" Then
            ref_version_organigrama = version_organigrama
        End If
        Dim ref_codigo_iso_organigrama As Object = Nothing
        If codigo_iso_organigrama = "" Then
            ref_codigo_iso_organigrama = "Null"
        Else
            ref_codigo_iso_organigrama = "'" & codigo_iso_organigrama & "'"
        End If
        Dim last_insert As Object = Nothing
        Dim SqlInsert As String = "Insert Into  Registro_Organigrama (NOMBRE_ORGANIGRAMA," _
       & "FECHA_ORGANIGRAMA,ESTADO_ORGANIGRAMA,NUMERO_RESOLUCION,DETALLE_RESOLUCION,EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA,VERSION_ORGANIGRAMA,CODIGO_ISO,FECHA_REGITRO_SISTEMA) values ('" & _
       nombre_organigrama & "','" & fecha_organigrama & "',0,'" & numero_resolucion & "','" & detalle_resolucion & "'," & id_empresa & _
       ",'" & ref_version_organigrama & "'," & ref_codigo_iso_organigrama & ",'" & date1al & "')"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Agregar_Organigrama = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = SqlInsert
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_Organigrama = "Imposible registrar el organigrama"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            last_insert = myCommand.LastInsertedId
            Dim Registro_plantilla As String = "insert into plantilla_diagrama_organico (REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA) values (" & _
            last_insert & ")"
            myCommand.CommandText = Registro_plantilla
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Agregar_Organigrama = "Imposible registrar la plantilla del organigrama"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim ilis As New ListItem
            ilis.Text = nombre_organigrama
            ilis.Value = last_insert
            drop_list.Items.Add(ilis)
            update.Update()
            myTrans.Commit()
            Agregar_Organigrama = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Agregar_Organigrama = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Agregar_Organigrama = Agregar_Organigrama
        End Try
    End Function
    Function Solicita_id_empresa_gestion_documental(ByRef id_empresa As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select  ID_EMPRESA " & _
                 " from EMPRESA_GESTION_DOCUMENTAL where ESTADO_EMPRESA=1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("EMPRESA_GESTION_DOCUMENTAL")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Solicita_id_empresa_gestion_documental = " Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_empresa = Dat_reader.Tables(0).Rows(0).Item(0)         
                Solicita_id_empresa_gestion_documental = "YES"
                Exit Function
            Else
                Solicita_id_empresa_gestion_documental = "Imposible encontrar empresa activa para registrar "
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_empresa_gestion_documental = "Inconsistencia general función Solicita_id_empresa_gestion_documental " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_organigrama(ByVal nombre_organigrama As String, _
                                             ByVal id_empresa As Integer, _
                                             ByRef confirm As Boolean) As String
        '*****************************************************
        'Funcion que verifica la existencia del nombre del 
        'organigrama con el id de la empresa/padre
        'Fecha : 2013-10-30
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************
        Try
            confirm = True
            Dim Parametro_Consulta As String = "select  ID_ORGANIGRAMA " & _
              " from REGISTRO_ORGANIGRAMA where NOMBRE_ORGANIGRAMA='" & nombre_organigrama & "' and " & _
              " EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=" & id_empresa
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("REGISTRO_ORGANIGRAMA")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Verifica_existencia_organigrama = " Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                confirm = True
                Verifica_existencia_organigrama = "YES"
                Exit Function
            Else
                confirm = False
                Verifica_existencia_organigrama = "YES"
                Exit Function
            End If


        Catch ex As Exception
            Verifica_existencia_organigrama = "Inconsistencia General Funcion Verifica_existencia_organigrama " & ex.Message
        End Try
    End Function
    Function Activa_editar_organigrama(ByVal id_organigrama As Integer, _
                                       ByRef nombre_organigrama As String, _
                                       ByRef numero_resolucion As String, _
                                       ByRef detalle_resolucion As String, _
                                       ByRef version_organigrama As String, _
                                       ByRef codigo_iso_organigrama As String, _
                                       ByRef fecha_organigrama As String, _
                                       ByRef update As UpdatePanel, _
                                       ByRef ModalPopupExtender_editar_organigrama As  _
                                       AjaxControlToolkit.ModalPopupExtender) As String
        Try
            If id_organigrama = 0 Then
                Activa_editar_organigrama = "Debe seleccionar el organigrama a editar"
                Exit Function
            End If
            Dim stru_area_organigrama As stru_diagrama_organico = Nothing
            Dim Result As String = ""
            Result = Me.Solicita_datos_organigrama(id_organigrama, stru_area_organigrama)
            If Result <> "YES" Then
                Activa_editar_organigrama = Result
                Exit Function
            Else
                nombre_organigrama = stru_area_organigrama.NOMBRE_ORGANIGRAMA
                numero_resolucion = stru_area_organigrama.NUMERO_RESOLUCION
                detalle_resolucion = stru_area_organigrama.DETALLE_RESOLUCION
                version_organigrama = stru_area_organigrama.VERSION_ORGANIGRAMA
                codigo_iso_organigrama = stru_area_organigrama.CODIGO_ISO
                Dim Tempfecha As String = Left(stru_area_organigrama.FECHA_ORGANIGRAMA, 10)
                Dim spli() As String = Tempfecha.ToString.Split("/")
                fecha_organigrama = spli(2) & "-" & spli(1) & "-" & spli(0)
                update.Update()
                ModalPopupExtender_editar_organigrama.Show()
                Activa_editar_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Activa_editar_organigrama = "Inconsistencia general función Activa_editar_organigrama " & ex.Message
        End Try
    End Function
    Function Solicita_datos_organigrama(ByVal id_organigrama As Integer, _
                                        ByRef stru_area_organigrama As  _
                                        stru_diagrama_organico) As String
        '---------------------------------------------- 
        'Función : Solicita datos de caracterización
        ' organigrama
        'Fecha : 2018-06-13
        'Ing : Miguel Angel Urueta Miranda 
        '---------------------------------------------- 
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("registro_organigrama")
            Dim sql_consulta As String = "Select EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA,NOMBRE_ORGANIGRAMA,FECHA_ORGANIGRAMA," & _
                "ESTADO_ORGANIGRAMA,NUMERO_RESOLUCION,DETALLE_RESOLUCION,CONSECUTIVO_AREA_DEPARTAMENTO," & _
                "VERSION_ORGANIGRAMA,CODIGO_ISO,CONSECUTIVO_SERIE,CONSECUTIVO_SUB_SERIE,FECHA_REGITRO_SISTEMA" & _
                " from registro_organigrama " & _
                " where ID_ORGANIGRAMA=" & id_organigrama
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_organigrama = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_organigrama = "Imposible enoncontrar datos de caracterización del organigrama " & id_organigrama
                Exit Function
            Else
                stru_area_organigrama.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA = Datset.Tables(0).Rows(0).Item(0)
                stru_area_organigrama.NOMBRE_ORGANIGRAMA = Datset.Tables(0).Rows(0).Item(1)
                stru_area_organigrama.FECHA_ORGANIGRAMA = Datset.Tables(0).Rows(0).Item(2)
                stru_area_organigrama.ESTADO_ORGANIGRAMA = Datset.Tables(0).Rows(0).Item(3)
                stru_area_organigrama.NUMERO_RESOLUCION = Datset.Tables(0).Rows(0).Item(4)
                stru_area_organigrama.DETALLE_RESOLUCION = Datset.Tables(0).Rows(0).Item(5)
                stru_area_organigrama.CONSECUTIVO_AREA_DEPARTAMENTO = Datset.Tables(0).Rows(0).Item(6)
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru_area_organigrama.VERSION_ORGANIGRAMA = ""
                Else
                    stru_area_organigrama.VERSION_ORGANIGRAMA = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    stru_area_organigrama.CODIGO_ISO = ""
                Else
                    stru_area_organigrama.CODIGO_ISO = Datset.Tables(0).Rows(0).Item(8)
                End If
                stru_area_organigrama.CONSECUTIVO_SERIE = Datset.Tables(0).Rows(0).Item(9)
                stru_area_organigrama.CONSECUTIVO_SUB_SERIE = Datset.Tables(0).Rows(0).Item(10)
                If Datset.Tables(0).Rows(0).IsNull(11) Then
                    stru_area_organigrama.FECHA_REGITRO_SISTEMA = ""
                Else
                    stru_area_organigrama.FECHA_REGITRO_SISTEMA = Datset.Tables(0).Rows(0).Item(11)
                End If
                Solicita_datos_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_organigrama = "Incosistencia general función Solicita_datos_organigrama " & ex.Message
        End Try
    End Function
    Function Editar_organigrama(ByVal id_organigrama As Integer, _
                                       ByVal nombre_organigrama As String, _
                                       ByVal numero_resolucion As String, _
                                       ByVal detalle_resolucion As String, _
                                       ByVal version_organigrama As String, _
                                       ByVal codigo_iso_organigrama As String, _
                                       ByVal fecha_organigrama As String, _
                                       ByRef drop_list As DropDownList, _
                                       ByRef update As UpdatePanel) As String

        If nombre_organigrama = "" Then
            Editar_organigrama = "Debe informar el nombre el nombre del organigrama"
            Exit Function
        End If
        If numero_resolucion = "" Then
            Editar_organigrama = "Debe informar el código de la resolución o acto administrativo del organigrama"
            Exit Function
        End If
        If detalle_resolucion = "" Then
            Editar_organigrama = "Debe informar la descripción de la resolución o acto administrativo del organigrama"
            Exit Function
        End If
        If fecha_organigrama = "" Then
            Editar_organigrama = "Debe informar la fecha creación del organigrama "
            Exit Function
        End If
        Dim stru_organigrama As stru_diagrama_organico = Nothing
        Dim Result As String = ""
        Result = Me.Solicita_datos_organigrama(id_organigrama, stru_organigrama)
        If Result <> "YES" Then
            Editar_organigrama = Result
            Exit Function
        End If
        Dim Id_empresa As Integer = 0
        Dim confirm As Boolean = True
        Dim Cambios As String = ""
        Dim update_registro As String = "Update registro_organigrama "
        If nombre_organigrama <> stru_organigrama.NOMBRE_ORGANIGRAMA Then
            Result = Me.Solicita_id_empresa_gestion_documental(Id_empresa)
            If Result <> "YES" Then
                Editar_organigrama = Result
                Exit Function
            End If
            Result = Me.Verifica_existencia_organigrama(nombre_organigrama, Id_empresa, confirm)
            If Result <> "YES" Then
                Editar_organigrama = Result
                Exit Function
            End If
            If confirm = True Then
                Editar_organigrama = "Esta intentado editar el nombre del organigrama con uno ya existente, imposible continuar"
                Exit Function
            End If
            Cambios = Cambios & " Cambio nombre organigrama valor actual " & stru_organigrama.NOMBRE_ORGANIGRAMA & " Nuevo valor " & nombre_organigrama
            If update_registro = "Update registro_organigrama " Then
                update_registro = update_registro & " set NOMBRE_ORGANIGRAMA='" & nombre_organigrama & "'"
            Else
                update_registro = update_registro & " , NOMBRE_ORGANIGRAMA='" & nombre_organigrama & "'"
            End If
        End If
        If numero_resolucion <> stru_organigrama.NUMERO_RESOLUCION Then
            If update_registro = "Update registro_organigrama " Then
                update_registro = update_registro & " set NUMERO_RESOLUCION='" & numero_resolucion & "'"
            Else
                update_registro = update_registro & " , NUMERO_RESOLUCION='" & numero_resolucion & "'"
            End If
            Cambios = Cambios & " Cambio de numero de resolución o acto administrativo valor actual " & stru_organigrama.NUMERO_RESOLUCION & " Nuevo valor " & numero_resolucion
        End If
        If detalle_resolucion <> stru_organigrama.DETALLE_RESOLUCION Then
            If update_registro = "Update registro_organigrama " Then
                update_registro = update_registro & " set DETALLE_RESOLUCION='" & detalle_resolucion & "'"
            Else
                update_registro = update_registro & " , DETALLE_RESOLUCION='" & detalle_resolucion & "'"
            End If
            Cambios = Cambios & " Cambio descripcion de resolución o acto administrativo valor actual " & stru_organigrama.DETALLE_RESOLUCION & " Nuevo valor " & detalle_resolucion
        End If
       
        If version_organigrama <> stru_organigrama.VERSION_ORGANIGRAMA Then
            Dim ref_version_organigrama As String = ""
            If version_organigrama = "" Then
                ref_version_organigrama = "Null"
            Else
                ref_version_organigrama = "'" & version_organigrama & "'"
            End If
            If update_registro = "Update registro_organigrama " Then
                update_registro = update_registro & " set VERSION_ORGANIGRAMA=" & ref_version_organigrama
            Else
                update_registro = update_registro & " , VERSION_ORGANIGRAMA=" & ref_version_organigrama
            End If
            Cambios = Cambios & " Cambio versión organigrama valor actual " & stru_organigrama.VERSION_ORGANIGRAMA & " Nuevo valor " & version_organigrama
        End If
        If codigo_iso_organigrama <> stru_organigrama.CODIGO_ISO Then
            Dim ref_codigo_iso_organigrama As String = ""
            If codigo_iso_organigrama = "" Then
                ref_codigo_iso_organigrama = "Null"
            Else
                ref_codigo_iso_organigrama = "'" & codigo_iso_organigrama & "'"
            End If
            If update_registro = "Update registro_organigrama " Then
                update_registro = update_registro & " set CODIGO_ISO=" & ref_codigo_iso_organigrama
            Else
                update_registro = update_registro & " , CODIGO_ISO=" & ref_codigo_iso_organigrama
            End If
            Cambios = Cambios & " Cambio código norma iso organigrama valor actual " & stru_organigrama.CODIGO_ISO & " Nuevo valor " & codigo_iso_organigrama
        End If
        Dim Tempfecha As String = Left(stru_organigrama.FECHA_ORGANIGRAMA, 10)
        Dim spli() As String = Tempfecha.ToString.Split("/")
        Dim fecha_organigrama_ As String = spli(2) & "-" & spli(1) & "-" & spli(0)
        If fecha_organigrama <> fecha_organigrama_ Then
            If update_registro = "Update registro_organigrama " Then
                update_registro = update_registro & " set FECHA_ORGANIGRAMA='" & fecha_organigrama & "'"
            Else
                update_registro = update_registro & " , FECHA_ORGANIGRAMA='" & fecha_organigrama & "'"
            End If
            Cambios = Cambios & " Cambio fecha del organigrama valor actual " & fecha_organigrama_ & " Nuevo valor " & fecha_organigrama
        End If
        If update_registro <> "Update registro_organigrama " Then
            update_registro = update_registro & " where ID_ORGANIGRAMA=" & id_organigrama
        End If
        If update_registro = "Update registro_organigrama " Then
            Editar_organigrama = "No se detectaron cambios para actualizar el organigrama"
            Exit Function
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = Date.Now
        Dim Refclas As New ClassListandoTareas
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Editar_organigrama = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "EDITA ORGANIGRAMA"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "EDITA ORGANIGRAMA " & id_organigrama & "  (" & _
        " EDITA ORGANIGRAMA CON LOS SIGUIENTES CAMBIOS " & Cambios & ")"
        isert_datos = isert_datos & "(" & id_organigrama & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date1al & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO Ra_log_organigrama (registro_organigrama_ID_ORGANIGRAMA,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Editar_organigrama = ""
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_registro
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Editar_organigrama = "Imposible registrar cambios en el organigrama"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Editar_organigrama = "Imposible registrar log del organigrama"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If stru_organigrama.NOMBRE_ORGANIGRAMA <> nombre_organigrama Then
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items.Item(i).Value = id_organigrama.ToString Then
                        drop_list.Items.Item(i).Text = nombre_organigrama
                        update.Update()
                        Exit For
                    End If
                Next
            End If
            myTrans.Commit()
            Editar_organigrama = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Editar_organigrama = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Editar_organigrama = Editar_organigrama
        End Try
    End Function
    Function Eliminar_organigrama(ByVal id_organigrama As Integer, _
                                  ByVal zon_view As Integer, _
                                  ByRef drop_list As DropDownList, _
                                  ByRef re_update As UpdatePanel, _
                                  ByRef ref_diagram As MindFusion.Diagramming.WebForms.DiagramView, _
                                  ByRef up_date As UpdatePanel) As String
        Dim Result As String = ""
        Dim Existencia As String = ""
        Result = Me.Verifica_relacion_areas_dep_Organigrama(id_organigrama, Existencia)
        If Result <> "YES" Then
            Eliminar_organigrama = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_organigrama = "El organigrama esta relacionado con areas o departamentos, imposible continuar"
            Exit Function
        End If
        Result = Me.Verifica_instrumentos_relacionados_al_organigrama(id_organigrama, Existencia)
        If Result <> "YES" Then
            Eliminar_organigrama = Result
            Exit Function
        End If
        If Existencia = "YES" Then
            Eliminar_organigrama = "El organigrama esta relacionado con instrumentos archivisticos, imposible continuar"
            Exit Function
        End If
        Dim ruta_plantilla As String = HttpContext.Current.Server.MapPath("../configuracion/dlc_flujo.vdx")
        If File.Exists(ruta_plantilla) = False Then
            Eliminar_organigrama = "Imposible encontrar el archivo " & ruta_plantilla
            Exit Function
        End If
        Dim ob As New MindFusion.Diagramming.Import.VisioImporter
        Dim diagran As New MindFusion.Diagramming.DiagramDocument
        Dim digran_page As New MindFusion.Diagramming.DiagramPage
        Dim delete_oganigrama As String = "Delete from  registro_organigrama where ID_ORGANIGRAMA=" & id_organigrama
        Dim delete_plantilla As String = "Delete from plantilla_diagrama_organico where REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA=" & id_organigrama
        Dim delete_log_organigrama As String = "Delete from ra_log_organigrama where registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        ref.Returna_Conexion_Mysql(myConnection)
        Eliminar_organigrama = ""
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = delete_log_organigrama
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                'myTrans.Rollback()
                'myConnection.Close()
                'Eliminar_organigrama = "Imposible eliminar la plantilla del organigrama  "
                'Exit Function
            End If
            myCommand.CommandText = delete_plantilla
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_organigrama = "Imposible eliminar la plantilla del organigrama  "
                Exit Function
            End If
            myCommand.CommandText = delete_oganigrama
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                myTrans.Rollback()
                Eliminar_organigrama = "Imposible eliminar el organigrama  "
                myConnection.Close()
                Exit Function
            End If         
          
            For i As Integer = 0 To drop_list.Items.Count - 1
                If drop_list.Items(i).Value = id_organigrama.ToString Then
                    drop_list.Items.Remove(drop_list.Items(i))
                    Exit For
                End If
            Next
            diagran = ob.Import(ruta_plantilla)
            If diagran.Pages.Count = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_organigrama = "La plantilla base no tiene el formato correcto, no contiene por lo menos un diagrama "
                Exit Function
            End If
            ref_diagram.Diagram.Items.Clear()
            ref_diagram.Diagram = diagran.Pages(0)
            ref_diagram.ZoomFactor = zon_view
            up_date.Update()
            re_update.Update()
            up_date.Update()
            HttpContext.Current.Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = 0
            myTrans.Commit()
            myConnection.Close()
            Eliminar_organigrama = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Eliminar_organigrama = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Eliminar_organigrama = Eliminar_organigrama
        End Try
    End Function
   
    Function Verifica_relacion_areas_dep_Organigrama(ByVal Id_Organigrama As String, _
                                                     ByRef Existencia As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("areas_depart_radicacion")
            Dim Parametro_Consulta As String = "Select Codigo_Area " & _
            " from areas_depart_radicacion where REGISTRO_ORGANIGRAMA_ID_ORGANIGRAMA=" & Id_Organigrama
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_relacion_areas_dep_Organigrama = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verifica_relacion_areas_dep_Organigrama = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verifica_relacion_areas_dep_Organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_relacion_areas_dep_Organigrama = "Inconsistencia General Función Verifica_relacion_areas_dep_Organigrama " & ex.Message
        End Try
    End Function
    Function Verifica_instrumentos_relacionados_al_organigrama(ByVal id_organigrama As Integer, _
     ByRef Existencia As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_registro_instrumento_archivistico")
            Dim Parametro_Consulta As String = "Select id_instrumento " & _
            " from ra_registro_instrumento_archivistico where registro_organigrama_ID_ORGANIGRAMA=" & id_organigrama
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_instrumentos_relacionados_al_organigrama = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Existencia = "NO"
                Verifica_instrumentos_relacionados_al_organigrama = "YES"
                Exit Function
            Else
                Existencia = "YES"
                Verifica_instrumentos_relacionados_al_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_instrumentos_relacionados_al_organigrama = "Inconsistencia general función Verifica_instrumentos_relacionados_al_organigrama " & ex.Message
        End Try
    End Function
    Function Activa_cambia_estado_organigrama(ByVal id_organigrama As Integer, _
                                              ByRef Check_activa_organigrama As CheckBox, _
                                              ByRef CheckBox_inactiva_organigrama As CheckBox, _
                                              ByRef ModalPopupExtender_cambia_estado_organigrama As  _
                                              AjaxControlToolkit.ModalPopupExtender, _
                                              ByRef UpdatePanel_cambia_estado_organigrama As UpdatePanel) As String
        Try
            Dim estado_organigrama As Integer = 0
            Dim Result As String = ""
            Result = Me.Solicita_estado_organigrama(id_organigrama, estado_organigrama)
            If Result <> "YES" Then
                Activa_cambia_estado_organigrama = Result
                Exit Function
            Else
                If estado_organigrama = 1 Then
                    Check_activa_organigrama.Checked = True
                    CheckBox_inactiva_organigrama.Checked = False
                Else
                    Check_activa_organigrama.Checked = False
                    CheckBox_inactiva_organigrama.Checked = True
                End If
                ModalPopupExtender_cambia_estado_organigrama.Show()
                UpdatePanel_cambia_estado_organigrama.Update()
                Activa_cambia_estado_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Activa_cambia_estado_organigrama = "Inconsistencia general función Activa_cambia_estado_organigrama " & ex.Message
        End Try
    End Function
    Function Solicita_estado_organigrama(ByVal id_organigrama As Integer, _
                                         ByRef estado_organigrama As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("registro_organigrama")
            Dim Parametro_Consulta As String = "Select ESTADO_ORGANIGRAMA " & _
            " from registro_organigrama where ID_ORGANIGRAMA=" & id_organigrama
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_organigrama = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_organigrama = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_organigrama = "Imposible encontrar el estado del radicado "
                Exit Function
            Else
                estado_organigrama = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_organigrama = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_organigrama = "Inconsistencia general función Solicita_estado_organigrama " & ex.Message
        End Try
    End Function
    Function Retorna_Id_Organigrama_activo_empresa(ByVal id_empresa As Integer, _
                                                   ByRef id_organigrama As Integer, _
                                                   ByVal estado_confirma As Integer) As String
        '************************************************************
        'Funcion : Retorna id organigrama de la empresa activa
        'Fecha : 2014-08-07
        'Ingeniero : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT ID_ORGANIGRAMA  FROM registro_organigrama " & _
                   "where ESTADO_ORGANIGRAMA=1 and " & _
                   "  EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA='" & id_empresa & "'"
            Dim Dat_reader As New DataSet
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Retorna_Id_Organigrama_activo_empresa = " Error función Retorna_Id_Organigrama_activo_empresa   " & result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                If estado_confirma = 0 Then
                    id_organigrama = 0
                    Retorna_Id_Organigrama_activo_empresa = "YES"
                    Exit Function
                Else
                    Retorna_Id_Organigrama_activo_empresa = " Imposible econtrar organigrama activo de la empresa   "
                    Exit Function
                End If
            Else
                id_organigrama = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_Id_Organigrama_activo_empresa = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Id_Organigrama_activo_empresa = "Inconsistencia General Funcion Retorna_Id_Organigrama_activo_empresa  : " & ex.Message
        End Try
    End Function
    Function Cambia_estado_organigrama(ByVal id_organigrama As Integer, _
                                              ByRef Check_activa_organigrama As CheckBox, _
                                              ByRef CheckBox_inactiva_organigrama As CheckBox) As String



        Dim estado_organigrama As Integer = 0
        Dim Result As String = ""
        Result = Me.Solicita_estado_organigrama(id_organigrama, estado_organigrama)
        If Result <> "YES" Then
            Cambia_estado_organigrama = Result
            Exit Function
        End If
        Dim nuevo_estado_organigrama As Integer = 0
        If Check_activa_organigrama.Checked = True Then
            nuevo_estado_organigrama = 1
        Else
            nuevo_estado_organigrama = 0
        End If
        If nuevo_estado_organigrama = estado_organigrama Then
            Cambia_estado_organigrama = "YES"
            Exit Function
        End If
        Dim date1al As String = ""
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date1al)
        If Result <> "YES" Then
            Cambia_estado_organigrama = Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim hor As String = Now
        Dim detalle_trans As String = "CAMBIA ESTADO ORGANIGRAMA"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "CAMBIA ESTADO ORGANIGRAMA " & id_organigrama & "  (" & _
        " CAMBIA DE ESTADO " & estado_organigrama & " A ESTADO " & nuevo_estado_organigrama & ")"
        isert_datos = isert_datos & "(" & id_organigrama & _
            ",'" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date1al & "'," & _
                    "'" & iphost & "','GESTOR-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO Ra_log_organigrama (registro_organigrama_ID_ORGANIGRAMA,desc_op,USER_OPER,ID_USER,DATE_TRANS" & _
                                             ",IP_TRANS,MODULO_REGISTRO,CAMPOS) values " & _
                                             isert_datos
        Dim update_inactiva_todos As String = "Update registro_organigrama set ESTADO_ORGANIGRAMA=0"
        Dim update_cambia_estado As String = "Update registro_organigrama set ESTADO_ORGANIGRAMA=" & nuevo_estado_organigrama & " where ID_ORGANIGRAMA=" & id_organigrama
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Cambia_estado_organigrama = ""
        Try
            Dim Switc As Integer
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            If nuevo_estado_organigrama = 1 Then
                myCommand.CommandText = update_inactiva_todos
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Cambia_estado_organigrama = "Imposible inactivar los organigramas para balance"
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myCommand.CommandText = update_cambia_estado
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_organigrama = "Imposible cambiar el estado del organigrama"
                If nuevo_estado_organigrama = 1 Then
                    myTrans.Rollback()
                End If
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Cambia_estado_organigrama = "Imposible registrar log del organigrama"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            Cambia_estado_organigrama = "YES"
            Exit Function
        Catch ex As MySqlException
            If myConnection.State = ConnectionState.Open Then
                myTrans.Rollback()
                myConnection.Close()
                Cambia_estado_organigrama = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally
            If myConnection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Cambia_estado_organigrama = Cambia_estado_organigrama
        End Try
    End Function
    Function Exporta_pdf_mindifucion_diagrama_organico(ByVal Ruta_Archivo As String, ByRef diagramView As MindFusion.Diagramming.WebForms.DiagramView, _
                                     ByRef ref_iframe As Object, ByRef ref_hiden As Object, ByRef updatapanel_iframe As UpdatePanel) As String
        Try
            Dim pdfExp As New MindFusion.Diagramming.Export.PdfExporter
            If Directory.Exists(Ruta_Archivo) = False Then
                Directory.CreateDirectory(Ruta_Archivo)
                Directory.CreateDirectory(Ruta_Archivo & "\" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            Else
                If Directory.Exists(Ruta_Archivo & "\" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")) = False Then
                    Directory.CreateDirectory(Ruta_Archivo & "\" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                End If
            End If
            ref_hiden.value = Ruta_Archivo & "\" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "\export_" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ".pdf"
            If File.Exists(ref_hiden.value) = True Then
                Kill(ref_hiden.value)
            End If
            Dim obshape As Object = Nothing
            Dim obshape_diagran As MindFusion.Diagramming.ShapeNode = Nothing
            For Each obshape In diagramView.Diagram.Items
                Dim ob As Object = obshape.GetType
                If ob.Fullname = "MindFusion.Diagramming.ShapeNode" Then
                    Dim localPath = HttpContext.Current.Server.MapPath(obshape.ImageUrl)
                    obshape.Image = Image.FromFile(localPath)
                    obshape_diagran = obshape
                    obshape_diagran.ImageAlign = MindFusion.Drawing.ImageAlign.TopCenter
                End If
            Next
            pdfExp.Export(diagramView.Diagram, ref_hiden.value)
            ref_iframe.Attributes.Add("src", "..\Docuarchi\WebFormDaDescarga.aspx")
            updatapanel_iframe.Update()
            Exporta_pdf_mindifucion_diagrama_organico = "YES"
        Catch ex As Exception
            Exporta_pdf_mindifucion_diagrama_organico = "Inconsistencia general función Exporta_pdf_mindifucion_diagrama_organico " & ex.Message
        End Try

    End Function
End Class
