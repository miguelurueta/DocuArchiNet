Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Xml
Public Class Class_ra_m_interface_meta_datos
    Function Crear_interface_registra_meta_datos(ByVal id_imagen As Integer,
                                                 ByVal nombre_gabinete As String,
                                                 ByRef stru_detalle_sis_meta_dato() As Class_ra_m_detalle_sis_meta_datos_) As String
        '------------------------------------------------------------------
        'Function : Retorna el estructura para la creación de la interface
        'para el registro del meta dato, con los parametros id_imagen y 
        'nombre del gabinete
        'Ing . Miguel Angel Urueta Miranda
        'Fecha : 2022-03-03
        '------------------------------------------------------------------
        Try
            Dim Ref_ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim Result As String = ""
            Dim id_registro_produccion As Long = 0
            Result = Ref_ClassGaProducionDocumental.Solicita_id_registro_producion_documental(id_imagen,
                                                                                              nombre_gabinete,
                                                                                              id_registro_produccion)
            If Result <> "YES" Then
                Crear_interface_registra_meta_datos = Result
                Exit Function
            End If
            '-----Valida registro producción documental
            If id_registro_produccion = 0 Then
                Crear_interface_registra_meta_datos = "El documento (" & id_imagen & ") no esta relacionado en el registro de produución documental gabinete (" & nombre_gabinete & ")"
                Exit Function
            End If
            Dim ref_Class_ra_m_registro_meta_dato_archivo As New Class_ra_m_registro_meta_dato_archivo
            Dim id_registro_meta_archivo_meta_dato As Long = 0
            Dim id_sistema_meta_dato As Integer = 0
            Result = ref_Class_ra_m_registro_meta_dato_archivo.Solicita_existencia_registro_sistema_meta_dato_Archivo(id_registro_produccion,
                                                                                                                      id_registro_meta_archivo_meta_dato,
                                                                                                                      id_sistema_meta_dato)
            If Result <> "YES" Then
                Crear_interface_registra_meta_datos = Result
                Exit Function
            End If

            Dim Class_ra_m_detalle_sis_meta_datos As New Class_ra_m_detalle_sis_meta_datos
            If id_sistema_meta_dato <> 0 Then
                Result = Class_ra_m_detalle_sis_meta_datos.Solicita_estructura_meta_dato_sistema(id_sistema_meta_dato,
                                                                                                 stru_detalle_sis_meta_dato)
                If Result <> "YES" Then
                    Crear_interface_registra_meta_datos = Result
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Valida sistema meta dato serie expediente
            '------------------------------------------
            Dim id_expediente As Integer = 0
            Dim id_serie_documental As Integer = 0
            Dim tipo_sistema_meta_dato As Integer = 0
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim Class_series_documentales As New Class_series_documentales
            Dim Class_ra_m_sistema_meta_datos As New Class_ra_m_sistema_meta_datos
            If id_sistema_meta_dato = 0 Then
                Result = Ref_ClassGaProducionDocumental.Solicita_id_expediente_registro_produccion(id_registro_produccion,
                                                                                                   id_expediente,
                                                                                                   1)
                If Result <> "YES" Then
                    Crear_interface_registra_meta_datos = Result
                    Exit Function
                End If
                If id_expediente <> 0 Then
                    Result = ClassGaExpediente.Solicita_id_serie_documental_expediente(id_expediente,
                                                                                       id_serie_documental)
                    If Result <> "YES" Then
                        Crear_interface_registra_meta_datos = Result
                        Exit Function
                    End If
                    If id_serie_documental <> 0 Then
                        Result = Class_series_documentales.Solicita_id_sistema_meta_dato_serie_documental(id_serie_documental,
                                                                                                          id_sistema_meta_dato)
                        If Result <> "YES" Then
                            Crear_interface_registra_meta_datos = Result
                            Exit Function
                        End If
                        If id_sistema_meta_dato <> 0 Then
                            Result = Class_ra_m_sistema_meta_datos.Solicita_tipo_sistema_meta_datos(id_sistema_meta_dato,
                                                                                                    tipo_sistema_meta_dato)
                            If Result <> "YES" Then
                                Crear_interface_registra_meta_datos = Result
                                Exit Function
                            End If
                            If tipo_sistema_meta_dato <> 1 Then
                                Crear_interface_registra_meta_datos = "El tipo de sistema de me datos no corresponde a un meta datos de archivos"
                                Exit Function
                            End If
                        End If
                    End If
                End If
            End If
            '-------------------------------------------
            'Valida sistema meta datos default
            '-------------------------------------------
            If id_sistema_meta_dato = 0 Then
                Result = Class_ra_m_sistema_meta_datos.Solicita_identificacion_sistema_meta_dato_default_archivo(1, id_sistema_meta_dato)
                If Result <> "YES" Then
                    Crear_interface_registra_meta_datos = Result
                    Exit Function
                End If
                If id_sistema_meta_dato = 0 Then
                    Crear_interface_registra_meta_datos = "El sistema no registra sistema meta datos por defecto para crear la interface, imposible continuar"
                    Exit Function
                End If
                Result = Class_ra_m_detalle_sis_meta_datos.Solicita_estructura_meta_dato_sistema(id_sistema_meta_dato,
                                                                                                stru_detalle_sis_meta_dato)
                If Result <> "YES" Then
                    Crear_interface_registra_meta_datos = Result
                    Exit Function
                End If
            End If
            '-------------------------------------------
            'Valida asignacion de datos a la interface
            '-------------------------------------------
            Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
            Dim Ruta_almacenamiento As String = ""
            Dim Nombre_archivo_xml As String = ""
            If id_registro_meta_archivo_meta_dato <> 0 Then
                Result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Ruta_almacenamiento,
                                                                       nombre_gabinete)
                If Result <> "YES" Then
                    Crear_interface_registra_meta_datos = Result
                    Exit Function
                End If
                Result = ref_Class_ra_m_registro_meta_dato_archivo.Solicita_nombre_archivo_xml_meta_dato_archivo(id_imagen,
                                                                                                                 nombre_gabinete,
                                                                                                                 Ruta_almacenamiento,
                                                                                                                 Nombre_archivo_xml)
                If Result <> "YES" Then
                    Crear_interface_registra_meta_datos = Result
                    Exit Function
                End If
                If File.Exists(Nombre_archivo_xml) Then
                    Result = Class_ra_m_detalle_sis_meta_datos.Asigna_contenido_estructura_meta_dato_de_archivo_xml(Nombre_archivo_xml, stru_detalle_sis_meta_dato)
                    Crear_interface_registra_meta_datos = Result
                    Exit Function
                End If
            End If
            Crear_interface_registra_meta_datos = "YES"
        Catch ex As Exception
            Crear_interface_registra_meta_datos = "Inconsistencia genera funcion Crear_interface_registra_meta_datos " & ex.Message
        End Try
    End Function
    Function Crea_interface_meta_datos(ByVal id_sistema_meta_datos As Integer,
                                       ByVal nombre_sistema_meta_datos As String,
                                       ByVal id_imagen_produccion_sistema_meta_datos As Long,
                                       ByVal gabinete_sistema_meta_datos As String,
                                       ByRef Page1 As Page,
                                       ByRef pane As Panel,
                                       ByRef Update As UpdatePanel) As String
        Try
            Dim Class_ra_m_detalle_sis_meta_datos As New Class_ra_m_detalle_sis_meta_datos
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim stru_detalle_item_meta_dato() As stru_detalle_sis_meta_dato = Nothing
            Dim Result As String = ""
            Dim estado_meta_dato As Integer = 0
            Result = ClassGaProducionDocumental.Solicita_estado_meta_dato_archivo_produccion(id_sistema_meta_datos,
                                                                                             estado_meta_dato)
            If Result <> "YES" Then
                Crea_interface_meta_datos = Result
                Exit Function
            End If
            Result = Class_ra_m_detalle_sis_meta_datos.Solicita_estructura_meta_dato_sistema_stru(id_sistema_meta_datos,
                                                                                                  stru_detalle_item_meta_dato)
            If Result <> "YES" Then
                Crea_interface_meta_datos = Result
                Exit Function
            End If
            If estado_meta_dato = 0 Then
                'Result = Me.Asigna_meta_datos_auto_poblado_inicia(id_imagen_produccion_sistema_meta_datos,
                '                                                 id_sistema_meta_datos,
                '                                                 nombre_sistema_meta_datos,
                '                                                 stru_detalle_item_meta_dato)
                'If Result <> "YES" Then
                '    Crea_interface_meta_datos = Result
                '    Exit Function
                'End If
            End If
            Result = Me.Inicializa_interface_meta_datos(stru_detalle_item_meta_dato,
                                                        Page1,
                                                        pane,
                                                        Update)
            If Result <> "YES" Then
                Crea_interface_meta_datos = Result
                Exit Function
            End If

            Crea_interface_meta_datos = "YES"
        Catch ex As Exception
            Crea_interface_meta_datos = "Inconsistencia fucion Crea_interface_meta_datos " & ex.Message
        End Try
    End Function
    Function Inicializa_interface_meta_datos(ByVal stru_detalle_item_meta_dato() As stru_detalle_sis_meta_dato,
                                             ByRef Page1 As Page,
                                             ByRef pane As Panel,
                                             ByRef Update As UpdatePanel) As String
        Try
            Dim Table As New Table
            Table.ID = "raw_some_table"
            Table.Attributes.Add("width", "100%")
            Dim objRow As New TableRow
            Dim objCell As New TableCell
            Dim m_TextBoxes() As TextBox = {}
            Dim LabelBox() As Label = {}
            If stru_detalle_item_meta_dato Is Nothing Then
                Inicializa_interface_meta_datos = "No hay estructura de meta datos para inicializar la interface"
                Exit Function
            End If
            Dim increment As Integer = 0
            ReDim Preserve LabelBox(increment)
            LabelBox(increment) = New Label
            objRow = New TableRow()
            objRow.HorizontalAlign = HorizontalAlign.Center
            objRow.CssClass = "modal_content_no_back_inferior nav_botota_person_gray"
            objCell = New TableCell
            objCell.CssClass = "pt-1"
            objCell.ColumnSpan = 2
            LabelBox(increment).Text = "Meta datos de usuarios"
            objCell.Controls.Add(LabelBox(increment))
            objRow.Cells.Add(objCell)
            Table.Rows.Add(objRow)
            increment = increment + 1
            For i As Integer = 0 To stru_detalle_item_meta_dato.Length - 1
                If stru_detalle_item_meta_dato(i).AUTO_PLOBLADO = 0 Then
                    Dim obliga As String = ""
                    If stru_detalle_item_meta_dato(i).estado_obliga_torio = "O" Then
                        obliga = " *"
                    End If
                    ReDim Preserve m_TextBoxes(increment)
                    ReDim Preserve LabelBox(increment)
                    LabelBox(increment) = New Label
                    LabelBox(increment).Text = UCase(stru_detalle_item_meta_dato(i).nombre_meta_dato) & obliga
                    LabelBox(increment).CssClass = "h6 font-weight-light"
                    LabelBox(increment).ToolTip = stru_detalle_item_meta_dato(i).descripcion_meta_dato & " Estandar : " & stru_detalle_item_meta_dato(i).ESTANDAR
                    m_TextBoxes(increment) = New TextBox
                    m_TextBoxes(increment).ToolTip = stru_detalle_item_meta_dato(i).descripcion_meta_dato & " Estandar : " & stru_detalle_item_meta_dato(i).ESTANDAR
                    m_TextBoxes(increment).ID = stru_detalle_item_meta_dato(i).EQUIVALENCIA_AUTO_POBLADO
                    objRow = New TableRow()
                    objCell = New TableCell
                    objCell.CssClass = "pt-2"
                    objCell.Controls.Add(LabelBox(increment))
                    objRow.Cells.Add(objCell)
                    objCell = New TableCell
                    objCell.Controls.Add(m_TextBoxes(increment))
                    objRow.Cells.Add(objCell)
                    Table.Rows.Add(objRow)
                    increment = increment + 1
                End If
            Next
            increment = increment + 1
            ReDim Preserve LabelBox(increment)
            LabelBox(increment) = New Label
            objRow = New TableRow()
            objCell = New TableCell
            objRow.HorizontalAlign = HorizontalAlign.Center
            objRow.CssClass = "modal_content_no_back_inferior nav_botota_person_gray"
            objCell.CssClass = "pt-1"
            objCell.ColumnSpan = 2
            LabelBox(increment).Text = "Meta datos de auto poblado"
            objCell.Controls.Add(LabelBox(increment))
            objRow.Cells.Add(objCell)
            Table.Rows.Add(objRow)
            increment = increment + 1
            For i As Integer = 0 To stru_detalle_item_meta_dato.Length - 1
                If stru_detalle_item_meta_dato(i).AUTO_PLOBLADO = 1 Then
                    Dim obliga As String = ""
                    If stru_detalle_item_meta_dato(i).estado_obliga_torio = "O" Then
                        obliga = " *"
                    End If
                    ReDim Preserve m_TextBoxes(increment)
                    ReDim Preserve LabelBox(increment)
                    LabelBox(increment) = New Label
                    LabelBox(increment).Text = UCase(stru_detalle_item_meta_dato(i).nombre_meta_dato) & obliga
                    LabelBox(increment).CssClass = "h6 font-weight-light"
                    LabelBox(increment).ToolTip = stru_detalle_item_meta_dato(i).descripcion_meta_dato & " Estandar : " & stru_detalle_item_meta_dato(i).ESTANDAR
                    m_TextBoxes(increment) = New TextBox
                    m_TextBoxes(increment).Enabled = False
                    m_TextBoxes(increment).ToolTip = stru_detalle_item_meta_dato(i).descripcion_meta_dato & " Estandar : " & stru_detalle_item_meta_dato(i).ESTANDAR
                    m_TextBoxes(increment).ID = stru_detalle_item_meta_dato(i).EQUIVALENCIA_AUTO_POBLADO
                    m_TextBoxes(increment).Text = stru_detalle_item_meta_dato(i).VALOR_AUTO_POBLADO
                    objRow = New TableRow()
                    objCell = New TableCell
                    objCell.CssClass = "pt-2"
                    objCell.Controls.Add(LabelBox(increment))
                    objRow.Cells.Add(objCell)
                    objCell = New TableCell
                    objCell.Controls.Add(m_TextBoxes(increment))
                    objRow.Cells.Add(objCell)
                    Table.Rows.Add(objRow)
                    increment = increment + 1
                End If
            Next
            pane.Controls.Add(Table)
            Update.Update()
            Inicializa_interface_meta_datos = "YES"
        Catch ex As Exception
            Inicializa_interface_meta_datos = "Inconsistencia general funcion Inicializa_interface_meta_datos " & ex.Message
        End Try
    End Function

End Class
