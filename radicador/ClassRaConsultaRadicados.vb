Imports AjaxControlToolkit
Imports MySql.Data.MySqlClient
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
Imports Dynamsoft.DotNet.TWAIN.Barcode

Public Class ClassRaConsultaRadicados
    Function Retorna_plantilla_radicacion_activas_drowlist(ByRef ref_drowlist As DropDownList) As String
        Try
            Dim Sql_consulta = "SELECT Nombre_Plantilla_Radicado FROM  system_plantilla_radicado where Tipo_Plantilla='" & "RADICACION ENTRANTE" & "'" &
                " AND Estado_Plantilla=1"
            Dim Result As String = ""
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system_plantilla_radicado")
            Result = ref2.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_plantilla_radicacion_activas_drowlist = "Funcion  Retorna_plantilla_radicacion_activas_drowlist WF-01 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_plantilla_radicacion_activas_drowlist = "YES"
                Exit Function
            Else
                ref_drowlist.Items.Clear()
                For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ref_drowlist.Items.Add(Datset.Tables(0).Rows(y).Item(0))
                Next
                If ref_drowlist.Items.Count > 0 Then
                    ref_drowlist.SelectedIndex = 0
                End If
                Retorna_plantilla_radicacion_activas_drowlist = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_plantilla_radicacion_activas_drowlist = "inconsistencia general funcion Retorna_plantilla_radicacion_activas_drowlist " & ex.Message
        End Try
    End Function
    Function lista_valores_campo_edita_plantilla(ByVal NombreGabi As String, ByVal campos As String, ByVal radicado As String, ByRef datos As String) As String
        Try
            Dim matri_campos As String() = campos.Split("º")
            Dim campos_consulta As String = ""
            For i As Integer = 0 To matri_campos.Length - 1
                If i = 0 Then
                    Dim matri() As String = matri_campos(i).Split("|")
                    campos_consulta = matri(0)
                Else
                    Dim matri() As String = matri_campos(i).Split("|")
                    campos_consulta = campos_consulta & "," & matri(0)
                End If
            Next
            Dim Sql_consulta = "SELECT " & campos_consulta & " FROM " & NombreGabi & " where Consecutivo_Rad='" & radicado & "'"
            Dim Result As String = ""
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
            Result = ref2.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                lista_valores_campo_edita_plantilla = "Funcion  lista_valores_campo_edita WF-01 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                lista_valores_campo_edita_plantilla = "Imposible encontrar id para el gabinete : " & NombreGabi
                Exit Function
            Else
                For y As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    If y = 0 Then
                        If Datset.Tables(0).Rows(0).IsNull(y) = True Then
                            datos = ""
                        Else
                            datos = Datset.Tables(0).Rows(0).Item(y)
                        End If
                    Else
                        If Datset.Tables(0).Rows(0).IsNull(y) = True Then
                            datos = datos & "|||||" & ""
                        Else
                            datos = datos & "|||||" & Datset.Tables(0).Rows(0).Item(y)
                        End If
                    End If
                Next
                lista_valores_campo_edita_plantilla = "YES"
                Exit Function
            End If

        Catch ex As Exception
            lista_valores_campo_edita_plantilla = "Inconsistencia general función lista_valores_campo_edita_plantilla " & ex.Message
        End Try

    End Function
    Function Lista_campos_edicion_plantilla(ByVal codigo_plantilla As Integer,
          ByVal nombre_plantilla_radicacion As String, ByRef campos As String) As String
        '-------------------------------------------------------------------
        'Función : Lista los campos de edicion de la plantilla de radicacion
        'en una matriz seoarados por comas, y º
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2015-10-07
        '--------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Refradicado As New ClassRadicador
            Dim Estado_opcion_fecha As Integer = 0
            Dim Estado_opcion_cita_respuesta As Integer = 0
            Dim Estado_opcion_radicado_general As Integer = 0
            Dim Estado_opcion_radicado_codigo_corto As Integer = 0
            Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(codigo_plantilla,
                                                                                           Estado_opcion_fecha,
                                                                                           Estado_opcion_cita_respuesta,
                                                                                           Estado_opcion_radicado_general,
                                                                                           Estado_opcion_radicado_codigo_corto)
            If Result <> "YES" Then
                Lista_campos_edicion_plantilla = Result
                Exit Function
            End If
            Dim Matri_Datos() As Campos_Plantilla
            Erase Matri_Datos
            Result = Refradicado.Lista_Campos_Adicionales_pre_consulta_radicacion(codigo_plantilla, Matri_Datos, Estado_opcion_fecha, Estado_opcion_cita_respuesta, Estado_opcion_radicado_general)
            If Result <> "YES" Then
                Lista_campos_edicion_plantilla = Result
                Exit Function
            End If
            For i As Integer = 0 To Matri_Datos.Length - 1
                Dim aleas As String = Trim(Matri_Datos(i).Alias_Campo)
                aleas = aleas.Replace(" ", "_")
                If Matri_Datos(i).Estado_Campo = 1 Then
                    If campos = "" Then
                        campos = Matri_Datos(i).Campo_Plantilla & "|" & aleas
                    Else
                        campos = campos & "º" & Matri_Datos(i).Campo_Plantilla & "|" & aleas
                    End If
                End If
            Next
            Lista_campos_edicion_plantilla = "YES"

        Catch ex As Exception
            Lista_campos_edicion_plantilla = "Inconsistencia general función Lista_campos_edicion_plantilla " & ex.Message
        End Try
    End Function
    Function Limpiar_Campos_Interface_Plantilla_Validacion(ByRef pag1 As Page) As String
        '*********************************************************************************
        'Funcion : Limpia los campos interface plantilla validacion
        'Fecha : 2014-08-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '**********************************************************************************
        Try
            Dim update As UpdatePanel = pag1.FindControl("UpdatePanelContenido")
            'Dim hide As Object = pag1.FindControl("hdnEmailID")
            Dim dateplantilla As String = HttpContext.Current.Session("SESIONITERCAMBIOPLANTILLAVALIDACION")
            If update Is Nothing Then
                Limpiar_Campos_Interface_Plantilla_Validacion = "Imposible el control UpdatePanelContenido en la funcion Limpiar_Campos_Interface_Plantilla_Validacion"
                Exit Function
            End If
            'If hide Is Nothing Then
            'Limpiar_Campos_Interface_Plantilla_Validacion = "Imposible el control hdnEmailID en la funcion Limpiar_Campos_Interface_Plantilla_Validacion"
            'Exit Function
            'End If


            Dim hidepais As Object = pag1.FindControl("Hiddenselecionpais")
            If hidepais Is Nothing Then
                Limpiar_Campos_Interface_Plantilla_Validacion = "Imposible el control Hiddenselecionpais en la funcion Limpiar_Campos_Interface_Plantilla_Validacion"
                Exit Function
            End If
            Dim hidepartamento As Object = pag1.FindControl("Hiddenseleciondepartamento")
            If hidepartamento Is Nothing Then
                Limpiar_Campos_Interface_Plantilla_Validacion = "Imposible el control Hiddenselecionpais en la funcion Limpiar_Campos_Interface_Plantilla_Validacion"
                Exit Function
            End If
            Dim hidemunicipio As Object = pag1.FindControl("Hiddenmunicipio")
            If hidemunicipio Is Nothing Then
                Limpiar_Campos_Interface_Plantilla_Validacion = "Imposible el control hidemunicipio en la funcion Limpiar_Campos_Interface_Plantilla_Validacion"
                Exit Function
            End If
            Dim Refclas_radicado As New ClassRadicador
            Dim Result As String = ""
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION
            Erase Matri_Datos
            Dim campo_idex As String = ""
            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Lista_Campos_Plantilla_Validacion(dateplantilla, Matri_Datos)
            If Result <> "YES" Then
                Limpiar_Campos_Interface_Plantilla_Validacion = Result
                Exit Function
            End If
            For i As Integer = 0 To Matri_Datos.Length - 1
                '----------------------------------------------------------
                'Asignacion de de datos de la intrface a la matriz
                '---------------------------------------------------------
                If Matri_Datos(i).Nombre_Campo = "Pais" Or Matri_Datos(i).Nombre_Campo = "Municipio" _
                               Or Matri_Datos(i).Nombre_Campo = "Departemento" Then
                    Select Case Matri_Datos(i).Nombre_Campo
                        Case "Pais"
                            Dim ob As DropDownList = pag1.FindControl(Matri_Datos(i).Nombre_Campo)
                            If Not ob Is Nothing Then
                                ob.SelectedIndex = 0
                                'Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = hidepais.value
                                'hidepais.value = ob.text
                                'estado_ubicacion = 1
                            End If
                        Case "Departemento"
                            Dim ob As DropDownList = pag1.FindControl(Matri_Datos(i).Nombre_Campo)
                            If Not ob Is Nothing Then
                                ob.Items.Clear()
                                hidepais.value = ""

                            End If

                        Case "Municipio"
                            Dim ob As DropDownList = pag1.FindControl(Matri_Datos(i).Nombre_Campo)
                            If Not ob Is Nothing Then
                                ob.Items.Clear()
                                hidemunicipio.value = ""

                            End If

                    End Select

                Else
                    Dim ob As Object = pag1.FindControl(Matri_Datos(i).Nombre_Campo)
                    If Not ob Is Nothing Then
                        ob.text = ""

                    End If
                End If
            Next
            update.Update()
            Limpiar_Campos_Interface_Plantilla_Validacion = "YES"
        Catch ex As Exception
            Limpiar_Campos_Interface_Plantilla_Validacion = "Inconsistencia Funcion Limpiar_Campos_Interface_Plantilla_Validacion " & ex.Message
        End Try
    End Function
    Function Genera_Interface_consulta_radicados(ByVal Codigo_Plantilla As String,
                                                 ByVal Tipo_Plantilla As String,
                                                 ByRef Page1 As Page,
                                                 ByVal nombre_plantilla As String) As String
        Try
            Dim refclasradicacion As New ClassRadicador
            Dim Resultado_General As String = "YES"
            Dim Result As String = ""
            '***************************************************************
            'Lista las opciones plantilla
            '***************************************************************
            Dim Estado_opcion_fecha As Integer = 0
            Dim Estado_opcion_cita_respuesta As Integer = 0
            Dim Estado_opcion_radicado_general As Integer = 0
            Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(Codigo_Plantilla,
                                                                                            Estado_opcion_fecha,
                                                                                            Estado_opcion_cita_respuesta,
                                                                                            Estado_opcion_radicado_general)
            If Result <> "YES" Then
                Genera_Interface_consulta_radicados = Result
                Exit Function
            End If
            Dim Matri_Datos() As Campos_Plantilla
            Erase Matri_Datos
            Result = refclasradicacion.Lista_Campos_Adicionales_pre_consulta_radicacion(Codigo_Plantilla,
                                                                                        Matri_Datos,
                                                                                        Estado_opcion_fecha,
                                                                                        Estado_opcion_cita_respuesta,
                                                                                        Estado_opcion_radicado_general)
            If Result <> "YES" Then
                Genera_Interface_consulta_radicados = Result
                Exit Function
            End If
            Dim _LabelboxIco As Label() = {}
            Dim m_TextBoxes() As TextBox = {}
            Dim LabelBox() As Label = {}
            'Dim Picture() As PictureBox = {}
            Dim _ComboBox() As DropDownList = {}
            Dim _CommamdBoton() As Button = {}
            Dim _image() As ImageButton = {}
            Dim Contador_Control As Integer = 0
            Dim Contador_Combo As Integer = 0
            Dim Contador_Text As Integer = 0
            Dim z2 As Integer = 0
            Dim pane As Panel = Page1.FindControl("_Panelvalidacion_val_radicacion")
            If pane Is Nothing Then
                Genera_Interface_consulta_radicados = "Imposible encontar el control _Panelvalidacion_val_radicacion"
                Exit Function
            End If
            'pane.Controls.Clear()
            Dim tablecontrolesdinamicos As Table = Page1.FindControl("_ValidacionConsulta_val_radicacion")
            If tablecontrolesdinamicos Is Nothing Then
                Genera_Interface_consulta_radicados = "Imposible encontar el control _ValidacionConsulta_val_radicacion"
                Exit Function
            End If
            Dim Update As UpdatePanel = Page1.FindControl("UpdatePanelContenido_val_radicacion")
            If Update Is Nothing Then
                Genera_Interface_consulta_radicados = "Imposible encontar el control UpdatePanelContenido_val_radicacion"
                Exit Function
            End If
            tablecontrolesdinamicos.Controls.Clear()
            Dim TableTitle As New Table
            Dim objRow As TableRow
            Dim objCell As TableCell
            '******************************************************************
            'Agrega controles dinamicos
            '******************************************************************
            objRow = New TableRow
            objRow.Width = 120
            Dim Contador_row As Integer = 1
            objCell = New TableCell
            objCell.Width = 100
            objCell.HorizontalAlign = HorizontalAlign.Left
            objCell.Wrap = True
            Contador_Control = Contador_Control + 1
            ReDim Preserve LabelBox(Contador_Control)
            LabelBox(Contador_Control) = New Label
            LabelBox(Contador_Control).Text = "Plantilla " & nombre_plantilla
            LabelBox(Contador_Control).Font.Size = 9
            LabelBox(Contador_Control).ForeColor = Drawing.Color.Red
            LabelBox(Contador_Control).Width = 85
            LabelBox(Contador_Control).Font.Name = "Segoe UI"
            LabelBox(Contador_Control).Attributes.Add("class", "h6 font-weight-light")
            objCell.Controls.Add(LabelBox(Contador_Control))
            objCell.Attributes.Add("background-color", "#E7EDF5")
            objRow.Cells.Add(objCell)
            tablecontrolesdinamicos.Controls.Add(objRow)
            If Matri_Datos.Length > 1 Then
                For k As Integer = 0 To Matri_Datos.Length - 1
                    If Matri_Datos(k).Estado_Campo = 1 Then
                        '--------------------------------------------------------------
                        'Agrega el label
                        '--------------------------------------------------------------
                        objRow = New TableRow
                        objCell = New TableCell
                        objCell.Width = 100
                        objCell.HorizontalAlign = HorizontalAlign.Left
                        objCell.Wrap = True
                        Contador_Control = Contador_Control + 1
                        ReDim Preserve LabelBox(Contador_Control)
                        LabelBox(Contador_Control) = New Label
                        If Matri_Datos(k).Campo_Obligatorio = 1 Then
                            LabelBox(Contador_Control).Text = Trim(Matri_Datos(k).Alias_Campo).Replace("_", " ")
                        Else
                            LabelBox(Contador_Control).Text = Trim(Matri_Datos(k).Alias_Campo).Replace("_", " ")
                        End If
                        LabelBox(Contador_Control).Font.Size = 9
                        LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
                        LabelBox(Contador_Control).Width = 85
                        LabelBox(Contador_Control).Font.Name = "Segoe UI"
                        LabelBox(Contador_Control).Attributes.Add("class", "h6 font-weight-light pt-3")
                        objCell.Controls.Add(LabelBox(Contador_Control))
                        objRow.Cells.Add(objCell)
                        tablecontrolesdinamicos.Controls.Add(objRow)
                        '---------------------------------------------------------------
                        'Agrega campo texbox
                        '---------------------------------------------------------------
                        objRow = New TableRow
                        Dim id_campo_aspnet As String = "REE_" & Matri_Datos(k).Campo_Plantilla & "-" & Matri_Datos(k).Campo_Plantilla & "-" & Matri_Datos(k).Tipo_Campo
                        Matri_Datos(k).ID_CAMPO_ASPNET = id_campo_aspnet
                        objCell = New TableCell
                        objCell.Wrap = True
                        ReDim Preserve m_TextBoxes(Contador_Control)
                        m_TextBoxes(Contador_Control) = New TextBox
                        m_TextBoxes(Contador_Control).ID = id_campo_aspnet
                        m_TextBoxes(Contador_Control).Attributes.Add("onkeypress", "GetChar (event);")
                        objCell.Controls.Add(m_TextBoxes(Contador_Control))
                        If Matri_Datos(k).Tipo_Campo = "DATE" Then
                            m_TextBoxes(Contador_Control).Width = 95
                            m_TextBoxes(Contador_Control).MaxLength = 10
                            m_TextBoxes(Contador_Control).Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                            m_TextBoxes(Contador_Control).Attributes.Add("placeholder", "0000 00 00")
                            Dim bhtml As New HtmlControls.HtmlGenericControl("button")
                            bhtml.Attributes.Add("class", "ml-1 btn border-0")
                            bhtml.Attributes.Add("title", "formato aaaa mm dd")
                            bhtml.ID = UCase(Matri_Datos(k).Campo_Plantilla) & "_-1Image"
                            Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                            ihtml.Attributes.Add("class", "fad fa-calendar-alt fa-1x")
                            bhtml.Controls.Add(ihtml)
                            objCell.Controls.Add(bhtml)
                            '-------------------------------------------------------
                            'Agrega boton calendario
                            '-------------------------------------------------------
                            Result = refclasradicacion.Agregar_Calendar(bhtml.ID.ToString,
                                                                        m_TextBoxes(Contador_Control).ID.ToString, pane)
                            If Result <> "YES" Then
                                'Genera_Interface_consulta_radicados = Result
                                'Exit Function
                            End If
                            ReDim Preserve m_TextBoxes(Contador_Control)
                            m_TextBoxes(Contador_Control) = New TextBox
                            m_TextBoxes(Contador_Control).MaxLength = 10
                            m_TextBoxes(Contador_Control).ID = id_campo_aspnet & "_2"
                            m_TextBoxes(Contador_Control).Attributes.Add("onkeypress", "GetChar (event);")
                            m_TextBoxes(Contador_Control).Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                            m_TextBoxes(Contador_Control).Attributes.Add("placeholder", "0000 00 00")
                            m_TextBoxes(Contador_Control).Width = 95
                            m_TextBoxes(Contador_Control).Attributes.Add("class", "ml-2")
                            objCell.Controls.Add(m_TextBoxes(Contador_Control))
                            bhtml = New HtmlControls.HtmlGenericControl("button")
                            bhtml.Attributes.Add("class", "ml-1 btn border-0")
                            bhtml.Attributes.Add("title", "formato aaaa mm dd")
                            bhtml.ID = UCase(Matri_Datos(k).Campo_Plantilla) & "_-2Image"
                            ihtml = New HtmlControls.HtmlGenericControl("i")
                            ihtml.Attributes.Add("class", "fad fa-calendar-alt fa-1x")
                            bhtml.Controls.Add(ihtml)
                            objCell.Controls.Add(bhtml)
                            '--------------------------------------
                            '-----Agregar claendar al botonimage
                            '--------------------------------------
                            Result = refclasradicacion.Agregar_Calendar(bhtml.ID.ToString, m_TextBoxes(Contador_Control).ID.ToString, pane)
                            If Result <> "YES" Then
                                'Genera_Interface_consulta_radicados = Result
                                'Exit Function
                            End If
                            objRow.Cells.Add(objCell)
                            tablecontrolesdinamicos.Controls.Add(objRow)
                        End If
                        If Matri_Datos(k).Tipo_Campo = "INT" Then
                            m_TextBoxes(Contador_Control).Width = 65
                            m_TextBoxes(Contador_Control).MaxLength = 9
                            m_TextBoxes(Contador_Control).Attributes.Add("onkeypress", "validate_numero(event,this)")
                            Contador_Control = Contador_Control + 1
                            ReDim Preserve LabelBox(Contador_Control)
                            LabelBox(Contador_Control) = New Label
                            LabelBox(Contador_Control).Font.Size = 10
                            LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
                            LabelBox(Contador_Control).Font.Name = "Segoe UI"
                            LabelBox(Contador_Control).Attributes.Add("class", "h6 font-weight-light pt-3")
                            LabelBox(Contador_Control).Text = " a "
                            objCell.Controls.Add(LabelBox(Contador_Control))
                            ReDim Preserve m_TextBoxes(Contador_Control)
                            m_TextBoxes(Contador_Control) = New TextBox
                            m_TextBoxes(Contador_Control).ID = id_campo_aspnet & "_2"
                            m_TextBoxes(Contador_Control).Attributes.Add("class", "form-control ")
                            m_TextBoxes(Contador_Control).Width = 85
                            m_TextBoxes(Contador_Control).Attributes.Add("onkeypress", "GetChar (event);")
                            objCell.Controls.Add(m_TextBoxes(Contador_Control))
                            objRow.Cells.Add(objCell)
                            tablecontrolesdinamicos.Controls.Add(objRow)
                            '******************************************************************
                            'Agregar auto completar
                            '******************************************************************
                            Dim metodo As String = "GetGuiaRadicaconasp"
                            If HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") = "PRODUCCION" Then
                                metodo = "GetGuiaRadicacon_interna"
                            End If
                            Result = refclasradicacion.agregar_auto_complete(m_TextBoxes(Contador_Control).ID,
                                                                             pane,
                                                                             metodo,
                                                                             nombre_plantilla,
                                                                             Matri_Datos(k).Campo_Plantilla)
                            If Result <> "YES" Then
                                'Genera_Interface_consulta_radicados = Result
                                'Exit Function
                            End If
                        End If

                        If Matri_Datos(k).Tipo_Campo = "VARCHAR" Or Matri_Datos(k).Tipo_Campo = "" Then
                            '******************************************************************
                            'Agregar auto completar
                            '******************************************************************
                            Dim campo_numero As String = ""
                            Dim longitud_campo As Integer = 0
                            If Matri_Datos(k).Tipo_Campo <> "TEXT" Then
                                campo_numero = Matri_Datos(k).Tipo_Campo.Replace("VARCHAR", "")
                                campo_numero = campo_numero.Replace("(", "")
                                campo_numero = campo_numero.Replace(")", "")
                                longitud_campo = Val(campo_numero)
                            End If
                            If longitud_campo <> 0 Then
                                m_TextBoxes(Contador_Control).MaxLength = longitud_campo
                            End If
                            m_TextBoxes(Contador_Control).Attributes.Add("class", "form-control")
                            Dim metodo As String = "GetGuiaRadicaconasp"
                            If HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") = "PRODUCCION" Then
                                metodo = "GetGuiaRadicacon_interna"
                            End If
                            Result = refclasradicacion.agregar_auto_complete(m_TextBoxes(Contador_Control).ID,
                                                                             pane,
                                                                             metodo,
                                                                             nombre_plantilla,
                                                                             Matri_Datos(k).Campo_Plantilla)
                            If Result <> "YES" Then
                                'Genera_Interface_consulta_radicados = Result
                                'Exit Function
                            End If
                            objRow.Cells.Add(objCell)
                            tablecontrolesdinamicos.Controls.Add(objRow)
                        End If
                        Contador_row = Contador_row + 1
                    End If
                Next
            End If
            pane.Controls.Add(tablecontrolesdinamicos)
            Genera_Interface_consulta_radicados = "YES"
        Catch ex As Exception
            Genera_Interface_consulta_radicados = "Inconsistencia funcion Genera_Interface_pre_consulta_radicado_entrante " & ex.Message
        End Try

    End Function



    Function retorna_datos_radicacion_impresion(ByVal id_radicacion As String,
                                                ByVal nombre_plantilla As String,
                                                ByRef codigo_destinatario As Integer,
                                                ByRef codigo_remitente As Integer,
                                                ByRef id_usuario As Integer,
                                                ByRef codigo_plantilla As String,
                                                ByRef Consecutivo_Radicado As String,
                                                ByRef Consecutivo_codigo_barra As String) As String
        '***********************************************************************
        'Función : Retorna datos de radicación con el consecutivo interno del
        'codigo de barras
        'Fecha : 2015-05-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '************************************************************************
        Try
            Dim Parametro_Consulta As String = "Select Destinatario_Externo_id_Dest_Ext,Remit_Dest_Interno_id_Remit_Dest_Int, " &
                "Usuario_Radicador_id_usuario,System_Plantilla_Radicado_id_Plantilla,Consecutivo_Rad,Consecutivo_CodBarra " &
                " from " & nombre_plantilla & " where Consecutivo_Rad='" & id_radicacion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                retorna_datos_radicacion_impresion = " Función retorna_datos_radicacion_impresion dice   " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                retorna_datos_radicacion_impresion = "Función retorna_datos_radicacion_impresion Imposible encontrar el id " & id_radicacion & "  del radicado"
                Exit Function
            Else
                codigo_destinatario = Dat_reader.Tables(0).Rows(0).Item(0)
                codigo_remitente = Dat_reader.Tables(0).Rows(0).Item(1)
                id_usuario = Dat_reader.Tables(0).Rows(0).Item(2)
                codigo_plantilla = Dat_reader.Tables(0).Rows(0).Item(3)
                Consecutivo_Radicado = Dat_reader.Tables(0).Rows(0).Item(4)
                Consecutivo_codigo_barra = Dat_reader.Tables(0).Rows(0).Item(5)
                retorna_datos_radicacion_impresion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            retorna_datos_radicacion_impresion = "Inconsistencia general función retorna_datos_radicacion_impresion " & ex.Message
        End Try
    End Function
    Function Solicita_rotulo_radicado(ByVal radicado As String,
                                      ByRef ruta_rotulo As String) As String
        Try
            Dim codigo_destinatario As Integer = 0
            Dim codigo_remitente As Integer = 0
            Dim id_usuario As Integer = 0
            Dim codigo_plantilla As Integer = 0
            Dim Consecutivo_Radicado_val As String = ""
            Dim Consecutivo_codigo_barra As String = ""
            Dim refclasconsulta As New ClassRaConsultaRadicados
            Dim split() As String = HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim Result As String = ""
            Dim refclas As New ClassRadicador
            Result = refclasconsulta.retorna_datos_radicacion_impresion(radicado,
                                                                        split(4),
                                                                        codigo_destinatario,
                                                                        codigo_remitente,
                                                                        id_usuario,
                                                                        codigo_plantilla,
                                                                        Consecutivo_Radicado_val,
                                                                        Consecutivo_codigo_barra)
            If Result <> "YES" Then
                Solicita_rotulo_radicado = Result
                Exit Function
            End If
            If HttpContext.Current.Session("RA_TIPO_IMPRESION") = "2" Then
                Dim valor As String = codigo_destinatario & "¬" & codigo_remitente & "¬" & id_usuario & "¬" & codigo_plantilla & "¬" & Consecutivo_Radicado_val & "¬" & Consecutivo_codigo_barra
                Dim Ruta_Sesion As String = ""
                Ruta_Sesion = HttpContext.Current.Session("RA_RUTA_TEMPO_IMPRESION").ToString()
                If Ruta_Sesion = "" Then
                    Solicita_rotulo_radicado = "El sistema no registra la ruta de radicación"
                    Exit Function
                End If
                Dim rutafinal As String = Ruta_Sesion & "\" & "RA" & radicado & ".pdf"
                HttpContext.Current.Session("RA_RUTA_IMPRESION_FINAL") = rutafinal
                ruta_rotulo = rutafinal
                Dim refplan() As Plantilla_Impresion
                Erase refplan
                Result = ""
                Result = refclas.Genera_Rotulo_Impresion(valor,
                                                         rutafinal,
                                                         refplan,
                                                         HttpContext.Current.Session("RA_TIPO_IMPRESION").ToString())
                If Result <> "YES" Then
                    Solicita_rotulo_radicado = Result
                    Exit Function
                End If
                Solicita_rotulo_radicado = "YES"
                Exit Function
            End If
            If HttpContext.Current.Session("RA_TIPO_IMPRESION") = "1" Then
                Dim valor As String = codigo_destinatario & "¬" & codigo_remitente & "¬" & id_usuario & "¬" & codigo_plantilla & "¬" & Consecutivo_Radicado_val & "¬" & Consecutivo_codigo_barra
                Dim spltival() As String = valor.Split("¬")
                Dim Ruta_Sesion As String = ""
                Ruta_Sesion = HttpContext.Current.Session("RA_RUTA_TEMPO_IMPRESION").ToString()
                If Ruta_Sesion = "" Then
                    Solicita_rotulo_radicado = "El sistema no registra la ruta de radicación"
                    Exit Function
                End If
                Dim rutafinal As String = Ruta_Sesion & "\" & "RA" & radicado & ".txt"
                ruta_rotulo = rutafinal
                HttpContext.Current.Session("RA_RUTA_IMPRESION_FINAL") = rutafinal
                Dim refplan() As Plantilla_Impresion
                Erase refplan
                Result = ""
                Result = refclas.Genera_Rotulo_Impresion(valor,
                                                         rutafinal,
                                                         refplan,
                                                         HttpContext.Current.Session("RA_TIPO_IMPRESION").ToString())
                If Result <> "YES" Then
                    Solicita_rotulo_radicado = Result
                    Exit Function
                End If
                Solicita_rotulo_radicado = "YES"
                Exit Function
            End If
            Solicita_rotulo_radicado = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_rotulo_radicado = "Inconsistencia general funcion Solicita_rotulo_radicado " & ex.Message
        End Try
    End Function
    Function Reimpresion_rotulo_radicacion(ByRef pag As Page) As String
        '-----------------------------------------------------------------
        'Asigna los datos de impresion para se leidos por las paginas
        'Impresion por texto o impresion post
        '-----------------------------------------------------------------
        Try
            Dim refclas As New ClassRadicador
            Dim result As String
            Dim UpdatePanelradciacionbotones As UpdatePanel = pag.FindControl("UpdatePanel_botones_radicacion")
            Dim UpdatePanel_imp_impresion As UpdatePanel = pag.FindControl("UpdatePanel_imp_impresion")
            Dim Hiddendatoradicacion As Object = pag.FindControl("Hiddendatoradicacion")
            Dim hide_ruta As Object = pag.FindControl("Hiddenruta")
            If hide_ruta Is Nothing Then
                Reimpresion_rotulo_radicacion = "Imposible encontrar el control Hiddenruta"
                Exit Function
            End If
            If Hiddendatoradicacion Is Nothing Then
                Reimpresion_rotulo_radicacion = "Imposible encontrar el control Hiddendatoradicacion"
                Exit Function
            End If
            If UpdatePanel_imp_impresion Is Nothing Then
                Reimpresion_rotulo_radicacion = "Imposible encontrar el control UpdatePanel_imp_impresion"
                Exit Function
            End If
            Dim upiframe As UpdatePanel = pag.FindControl("UpdatePaneliframe")
            If upiframe Is Nothing Then
                Reimpresion_rotulo_radicacion = "Imposible encontrar el control UpdatePaneliframe"
                Exit Function
            End If
            Dim upiframe_post As UpdatePanel = pag.FindControl("UpdatePaneliframe_post")
            If upiframe_post Is Nothing Then
                Reimpresion_rotulo_radicacion = "Imposible encontrar el control UpdatePaneliframe_post"
                Exit Function
            End If
            Dim Modal As ModalPopupExtender = pag.FindControl("ModalPopupExtenderimpre")
            If Modal Is Nothing Then
                Reimpresion_rotulo_radicacion = "Imposible encontrar el control ModalPopupExtenderimpre"
                Exit Function
            End If
            Dim Modal_post As ModalPopupExtender = pag.FindControl("ModalPopupExtenderimpre_post")
            If Modal_post Is Nothing Then
                Reimpresion_rotulo_radicacion = "Imposible encontrar el control ModalPopupExtenderimpre_post"
                Exit Function
            End If
            Dim Hidden_consecutivo_radicado As Object = pag.FindControl("Hidden_consecutivo_radicado")
            If Hidden_consecutivo_radicado Is Nothing Then
                Reimpresion_rotulo_radicacion = "Imposible encontrar el control Hidden_consecutivo_radicado"
                Exit Function
            End If
            Dim consecutivo_radicado As String = Hidden_consecutivo_radicado.Value
            Dim hdnEmailID_VAL As Object = pag.FindControl("hdnEmailID_VAL")
            If hdnEmailID_VAL Is Nothing Then
                Reimpresion_rotulo_radicacion = "Imposible encontrar el control hdnEmailID_VAL"
                Exit Function
            End If
            If hdnEmailID_VAL.Value = "0" Or hdnEmailID_VAL.Value = "-1" Then
                Reimpresion_rotulo_radicacion = "Debe seleccionar el consecutivo a reimprimir"
                Exit Function
            End If
            consecutivo_radicado = hdnEmailID_VAL.Value
            '---------------------------------------------------------------
            'Consulta datos radicación
            '---------------------------------------------------------------
            Dim codigo_destinatario As Integer = 0
            Dim codigo_remitente As Integer = 0
            Dim id_usuario As Integer = 0
            Dim codigo_plantilla As Integer = 0
            Dim Consecutivo_Radicado_val As String = ""
            Dim Consecutivo_codigo_barra As String = ""
            Dim refclasconsulta As New ClassRaConsultaRadicados
            Dim split() As String = HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            result = refclasconsulta.retorna_datos_radicacion_impresion(hdnEmailID_VAL.Value,
                                                                        split(4),
                                                                        codigo_destinatario,
                                                                        codigo_remitente,
                                                                        id_usuario,
                                                                        codigo_plantilla,
                                                                        Consecutivo_Radicado_val,
                                                                        Consecutivo_codigo_barra)
            If result <> "YES" Then
                Reimpresion_rotulo_radicacion = result
                Exit Function
            End If
            HttpContext.Current.Session("RA_DATO_IMPRESION") = codigo_destinatario & "¬" & codigo_remitente & "¬" & id_usuario & "¬" & codigo_plantilla & "¬" & Consecutivo_Radicado_val & "¬" & Consecutivo_codigo_barra
            If HttpContext.Current.Session("RA_TIPO_IMPRESION") = "2" Then
                Dim valor As String = codigo_destinatario & "¬" & codigo_remitente & "¬" & id_usuario & "¬" & codigo_plantilla & "¬" & Consecutivo_Radicado_val & "¬" & Consecutivo_codigo_barra
                Dim Ruta_Sesion As String = ""
                Ruta_Sesion = HttpContext.Current.Session("RA_RUTA_TEMPO_IMPRESION").ToString()
                If Ruta_Sesion = "" Then
                    Reimpresion_rotulo_radicacion = "El sistema no registra la ruta de radicación"
                    Exit Function
                End If
                Dim rutafinal As String = Ruta_Sesion & "\" & "RA" & consecutivo_radicado & ".pdf"
                HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO") = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + HttpContext.Current.Session.Item("RA_ID_USUARIO").ToString & "/IMPRESION/" & "RA" & consecutivo_radicado & ".pdf"
                HttpContext.Current.Session("RA_RUTA_IMPRESION_FINAL") = rutafinal
                hide_ruta.value = rutafinal
                'Exit Sub
                '************************************************************
                'Genera rotulo de impresion radicacion
                '************************************************************

                Dim refplan() As Plantilla_Impresion
                Erase refplan
                result = ""
                result = refclas.Genera_Rotulo_Impresion(valor,
                                                         rutafinal,
                                                         refplan,
                                                         HttpContext.Current.Session("RA_TIPO_IMPRESION").ToString())
                If result <> "YES" Then
                    Reimpresion_rotulo_radicacion = result
                    'Exit Sub
                End If
                upiframe.Update()
                Modal.Show()
                Reimpresion_rotulo_radicacion = "YES"
            End If
            If HttpContext.Current.Session("RA_TIPO_IMPRESION") = "1" Then
                Dim valor As String = codigo_destinatario & "¬" & codigo_remitente & "¬" & id_usuario & "¬" & codigo_plantilla & "¬" & Consecutivo_Radicado_val & "¬" & Consecutivo_codigo_barra
                Dim spltival() As String = valor.Split("¬")
                Dim Ruta_Sesion As String = ""
                Ruta_Sesion = HttpContext.Current.Session("RA_RUTA_TEMPO_IMPRESION").ToString()
                If Ruta_Sesion = "" Then
                    Reimpresion_rotulo_radicacion = "El sistema no registra la ruta de radicación"
                    Exit Function
                End If
                Dim rutafinal As String = Ruta_Sesion & "\" & "RA" & consecutivo_radicado & ".txt"
                HttpContext.Current.Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO") = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + HttpContext.Current.Session.Item("RA_ID_USUARIO").ToString & "/IMPRESION/" & "RA" & consecutivo_radicado & ".txt"
                HttpContext.Current.Session("RA_RUTA_IMPRESION_FINAL") = rutafinal
                hide_ruta.value = rutafinal
                'Exit Sub
                '************************************************************
                'Genera rotulo de impresion radicacion
                '************************************************************
                Dim pesta As String = ""
                pesta = HttpContext.Current.Session("RA_RUTA_IMPRESION_FINAL")
                Dim refplan() As Plantilla_Impresion
                Erase refplan
                result = ""
                result = refclas.Genera_Rotulo_Impresion(valor,
                                                         rutafinal,
                                                         refplan,
                                                         HttpContext.Current.Session("RA_TIPO_IMPRESION").ToString())
                If result <> "YES" Then
                    Reimpresion_rotulo_radicacion = result
                End If
                upiframe.Update()
                Modal.Show()
                Reimpresion_rotulo_radicacion = "YES"

            End If
            Reimpresion_rotulo_radicacion = "YES"
        Catch ex As Exception
            Reimpresion_rotulo_radicacion = "Inconsistencia genral función Reimpresion_rotulo_radicacion " & ex.Message
        End Try
    End Function

    Function asgina_auto_complete_edicion(ByRef page As Page) As String
        Try
            Dim split() As String = HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim refclas_radicacion As New ClassRadicador
            Dim result As String = ""
            If split(2) = "RADICACION ENTRANTE" Then
                Dim panel_controles As Panel = page.FindControl("conenedor_controles_entrante")

                Dim TextBox_remitente_entrante As TextBox = page.FindControl("TextBox_remitente_entrante")
                If Not TextBox_remitente_entrante Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete("TextBox_remitente_entrante", panel_controles, "GetGuiaRadicaconasp", split(4), "Remitente_Cor")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If
                Dim TextBoxIdentificacion_remitente As TextBox = page.FindControl("TextBoxIdentificacion_remitente")
                If Not TextBoxIdentificacion_remitente Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete("TextBoxIdentificacion_remitente", panel_controles, "GetGuiaRadicaconasp", split(4), "IDENTIFICACION_REMITENTE")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If

                Dim TextBox_asunto_entrante As TextBox = page.FindControl("TextBox_asunto_entrante")
                If Not TextBox_asunto_entrante Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete("TextBox_asunto_entrante", panel_controles, "GetGuiaRadicaconasp", split(4), "Asunto")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If

                Dim TextBox_cita_radicado_entrante As TextBox = page.FindControl("TextBox_cita_radicado_entrante")
                If Not TextBox_cita_radicado_entrante Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete("TextBox_cita_radicado_entrante", panel_controles, "GetGuiaRadicaconasp", split(4), "CITARADICADO")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If


                Dim TextBox_anexos_entrante As TextBox = page.FindControl("TextBox_anexos_entrante")
                If Not TextBox_anexos_entrante Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete("TextBox_anexos_entrante", panel_controles, "GetGuiaRadicaconasp", split(4), "Anexos_Cor")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If

                Dim TextBox_fecha_documento_entrante As TextBox = page.FindControl("TextBox_fecha_documento_entrante")
                result = refclas_radicacion.Agregar_Calendar("ImageButton_saliente", "TextBox_fecha_documento_entrante", panel_controles)
                If result <> "YES" Then
                    asgina_auto_complete_edicion = result
                    Exit Function
                End If

            Else
                Dim panel_controles As Panel = page.FindControl("conenedor_controles_saliente")
                Dim Tipo_script As String = ""
                Dim id_escript As Integer = -1
                Dim TextBox_remitente_saliente As TextBox = page.FindControl("TextBox_remitente_saliente")
                If Not TextBox_remitente_saliente Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete(TextBox_remitente_saliente.ID, panel_controles, "GetGuiaRadicaconasp", split(4), "Destinatario_Cor")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If
                Dim TextBox_identificacion_destinatario As TextBox = page.FindControl("TextBox_identificacion_destinatario")
                If Not TextBox_identificacion_destinatario Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete(TextBox_identificacion_destinatario.ID, panel_controles, "GetGuiaRadicaconasp", split(4), "IDENTIFICACION_DESTINATARIO")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If

                Dim TextBox_asunto_saliente As TextBox = page.FindControl("TextBox_asunto_saliente")
                If Not TextBox_asunto_saliente Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete(TextBox_asunto_saliente.ID, panel_controles, "GetGuiaRadicaconasp", split(4), "Asunto")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If

                Dim TextBox_cita_radicado_saliente As TextBox = page.FindControl("TextBox_cita_radicado_saliente")
                If Not TextBox_cita_radicado_saliente Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete(TextBox_cita_radicado_saliente.ID, panel_controles, "GetGuiaRadicaconasp", split(4), "CITARADICADO")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If

                Dim TextBox_anexos_saliente As TextBox = page.FindControl("TextBox_anexos_saliente")
                If Not TextBox_anexos_saliente Is Nothing Then
                    result = refclas_radicacion.agregar_auto_complete(TextBox_anexos_saliente.ID, panel_controles, "GetGuiaRadicaconasp", split(4), "Anexos_Cor")
                    If result <> "YES" Then
                        asgina_auto_complete_edicion = result
                        Exit Function
                    End If
                End If


            End If
            asgina_auto_complete_edicion = "YES"
        Catch ex As Exception
            asgina_auto_complete_edicion = "Inconsistencias finción asgina_auto_complete_edicion " & ex.Message
        End Try
    End Function

    Function asigna_datos_edicion_plantilla_radicado(ByRef page As Page) As String
        Try
            Dim refclas As New ClassRaConsultaRadicados
            Dim refclas_radicacion As New ClassRadicador
            Dim split() As String = HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim selected_text As String = ""
            Dim Droplistarea As DropDownList = Nothing
            Dim Droplist As DropDownList = Nothing
            Dim modal_popup As ModalPopupExtender = Nothing
            Dim update_controles As UpdatePanel = Nothing
            Dim hdnEmailID_VAL As Object = page.FindControl("hdnEmailID_VAL")
            If HttpContext.Current.Session.Item("RA_PERMISO_EDITA_RADICADO") = 0 Then
                asigna_datos_edicion_plantilla_radicado = "El usuario no tiene permisos para editar"
                Exit Function
            End If
            If hdnEmailID_VAL.Value = "-1" Then
                asigna_datos_edicion_plantilla_radicado = "Debe seleccionar el registro a editar"
                Exit Function
            End If
            Dim result As String = ""
            Dim stru_plantilla As PLANTILLA_VALIDACION_CAMPOS_ESTATICOS = Nothing
            Dim ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            result = ref_Class_plantillas_radicacion.retorna_datos_radicacion_estructura(split(2),
                                                                                         hdnEmailID_VAL.Value,
                                                                                         split(4),
                                                                                         stru_plantilla)
            If result <> "YES" Then
                asigna_datos_edicion_plantilla_radicado = result
                Exit Function
            End If
            '***************************************************************
            'Asigna valores plantillas al html intercambio
            '***************************************************************
            Dim Hidden_area_remitente_destinatario As Object = page.FindControl("Hidden_area_remitente_destinatario")
            Dim Hidden_remitente_destinario_interno As Object = page.FindControl("Hidden_remitente_destinario_interno")
            Dim Hidden_tipo_plantilla As Object = page.FindControl("Hidden_tipo_plantilla")
            Dim Hidden_nombre_plantilla_radicado As Object = page.FindControl("Hidden_nombre_plantilla_radicado")
            Dim Hidden_remitente_destinatario As Object = page.FindControl("Hidden_remitente_destinatario")
            Hidden_area_remitente_destinatario.value = stru_plantilla.Id_area_remit_dest_interno
            Hidden_remitente_destinario_interno.value = stru_plantilla.Destinatario_Externo_id_Dest_Ext
            Hidden_remitente_destinatario.value = stru_plantilla.Remit_Dest_Interno_id_Remit_Dest_Int
            Hidden_nombre_plantilla_radicado.value = split(4)
            Hidden_tipo_plantilla.value = split(2)
            Dim updatepanel_Asigana_datos_validacion_edicion As UpdatePanel = page.FindControl("updatepanel_Asigana_datos_validacion_edicion")
            '***************************************************************
            'Lista las opciones plantilla
            '***************************************************************
            Dim Estado_opcion_fecha As Integer = 0
            Dim Estado_opcion_cita_respuesta As Integer = 0
            Dim Estado_opcion_radicado_general As Integer = 0
            Dim Estado_opcion_valida_externo As Integer = 0
            '---------------------------------------
            '------Lista opciones plantilla
            '---------------------------------------
            Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(split(1),
                                                                                           Estado_opcion_fecha,
                                                                                           Estado_opcion_cita_respuesta,
                                                                                           Estado_opcion_radicado_general,
                                                                                           Estado_opcion_valida_externo)
            If result <> "YES" Then
                asigna_datos_edicion_plantilla_radicado = result
                Exit Function
            End If
            Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
            Dim Matri_Datos() As Campos_Plantilla
            Erase Matri_Datos
            result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(split(1),
                                                                                                Matri_Datos,
                                                                                                Estado_opcion_fecha,
                                                                                                Estado_opcion_cita_respuesta,
                                                                                                Estado_opcion_radicado_general)
            If result <> "YES" Then
                asigna_datos_edicion_plantilla_radicado = result
                Exit Function
            End If

            Dim id_empresa As Integer = -1
            Dim refclas_rad As New ClassRadicador
            result = refclas_rad.Retorna_ID_Empresa_Usuario_Radicador(HttpContext.Current.Session.Item("RA_ID_USUARIO"), id_empresa)
            If result <> "YES" Then
                asigna_datos_edicion_plantilla_radicado = result
                Exit Function
            End If
            Dim id_organigrama As Integer = -1
            Dim ref_clas_empresa As New Class_registro_organigrama
            result = ref_clas_empresa.Retorna_Id_Organigrama_activo_empresa(id_empresa,
                                                                            id_organigrama)
            If result <> "YES" Then
                asigna_datos_edicion_plantilla_radicado = result
                Exit Function
            Else
                If split(2) = "RADICACION ENTRANTE" Then
                    Dim panel_controles As Panel = page.FindControl("conenedor_controles_entrante")
                    Dim Tipo_script As String = ""
                    Dim id_escript As Integer = -1
                    Dim Class_plantilla_validacion As New Class_plantilla_validacion
                    result = Class_plantilla_validacion.Retorna_Tipo_Validacion_Campo(Matri_Datos,
                                                                                      "REMITENTE_COR",
                                                                                      Tipo_script,
                                                                                      id_escript)
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_remitente_entrante As TextBox = page.FindControl("TextBox_remitente_entrante")
                    result = refclas_radicacion.agregar_auto_complete("TextBox_remitente_entrante", panel_controles, "GetGuiaRadicaconasp", split(4), "Remitente_Cor")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    If id_escript <> -1 Then
                        TextBox_remitente_entrante.BackColor = Drawing.Color.Yellow
                        TextBox_remitente_entrante.Attributes.Add("disabled", "true")
                    End If
                    Dim TextBoxIdentificacion_remitente As TextBox = page.FindControl("TextBoxIdentificacion_remitente")
                    If id_escript <> -1 Then
                        TextBoxIdentificacion_remitente.BackColor = Drawing.Color.Yellow
                        TextBoxIdentificacion_remitente.Attributes.Add("disabled", "true")
                    End If
                    result = refclas_radicacion.agregar_auto_complete("TextBoxIdentificacion_remitente", panel_controles, "GetGuiaRadicaconasp", split(4), "IDENTIFICACION_REMITENTE")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_asunto_entrante As TextBox = page.FindControl("TextBox_asunto_entrante")
                    result = refclas_radicacion.agregar_auto_complete("TextBox_asunto_entrante", panel_controles, "GetGuiaRadicaconasp", split(4), "Asunto")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_cita_radicado_entrante As TextBox = page.FindControl("TextBox_cita_radicado_entrante")
                    result = refclas_radicacion.agregar_auto_complete("TextBox_cita_radicado_entrante", panel_controles, "GetGuiaRadicaconasp", split(4), "CITARADICADO")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_Numero_Folios_entrante As TextBox = page.FindControl("TextBox_Numero_Folios_entrante")
                    Dim TextBox_anexos_entrante As TextBox = page.FindControl("TextBox_anexos_entrante")
                    result = refclas_radicacion.agregar_auto_complete("TextBox_anexos_entrante", panel_controles, "GetGuiaRadicaconasp", split(4), "Anexos_Cor")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_fecha_documento_entrante As TextBox = page.FindControl("TextBox_fecha_documento_entrante")
                    result = refclas_radicacion.Agregar_Calendar("ImageButton_saliente", "TextBox_fecha_documento_entrante", panel_controles)
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If

                    TextBox_remitente_entrante.Text = stru_plantilla.Remitente_Cor
                    TextBoxIdentificacion_remitente.Text = stru_plantilla.IDENTIFICACION_REMITENTE
                    TextBox_asunto_entrante.Text = stru_plantilla.Asunto
                    TextBox_cita_radicado_entrante.Text = stru_plantilla.CITARADICADO
                    TextBox_Numero_Folios_entrante.Text = stru_plantilla.Numero_Folios
                    TextBox_anexos_entrante.Text = stru_plantilla.Anexos_Cor
                    TextBox_fecha_documento_entrante.Text = stru_plantilla.Fecha_Documento
                    Droplist = page.FindControl("DropDownList_destinatario_entrante")
                    Droplistarea = page.FindControl("DropDownList_area_destinatario_entrate")
                    modal_popup = page.FindControl("ModalPopupExtender_editar_radicacion_entrante")
                    update_controles = page.FindControl("UpdatePnaelcontrolesradicacion_entrante")
                    selected_text = stru_plantilla.Destinatario_Cor
                Else
                    Dim panel_controles As Panel = page.FindControl("conenedor_controles_saliente")
                    Dim Tipo_script As String = ""
                    Dim id_escript As Integer = -1
                    Dim Class_plantilla_validacion As New Class_plantilla_validacion
                    result = Class_plantilla_validacion.Retorna_Tipo_Validacion_Campo(Matri_Datos,
                                                                                      "REMITENTE_COR",
                                                                                      Tipo_script, id_escript)
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_remitente_saliente As TextBox = page.FindControl("TextBox_remitente_saliente")
                    result = refclas_radicacion.agregar_auto_complete("TextBox_remitente_saliente", panel_controles, "GetGuiaRadicaconasp", split(4), "Destinatario_Cor")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    If id_escript <> -1 Then
                        TextBox_remitente_saliente.BackColor = Drawing.Color.Yellow
                        TextBox_remitente_saliente.Attributes.Add("disabled", "true")
                    End If
                    Dim TextBox_identificacion_destinatario As TextBox = page.FindControl("TextBox_identificacion_destinatario")
                    If id_escript <> -1 Then
                        TextBox_identificacion_destinatario.BackColor = Drawing.Color.Yellow
                        TextBox_identificacion_destinatario.Attributes.Add("disabled", "true")
                    End If
                    result = refclas_radicacion.agregar_auto_complete("TextBox_identificacion_destinatario", panel_controles, "GetGuiaRadicaconasp", split(4), "IDENTIFICACION_DESTINATARIO")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_asunto_saliente As TextBox = page.FindControl("TextBox_asunto_saliente")
                    result = refclas_radicacion.agregar_auto_complete("TextBox_asunto_saliente", panel_controles, "GetGuiaRadicaconasp", split(4), "Asunto")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_cita_radicado_saliente As TextBox = page.FindControl("TextBox_cita_radicado_saliente")
                    result = refclas_radicacion.agregar_auto_complete("TextBox_cita_radicado_saliente", panel_controles, "GetGuiaRadicaconasp", split(4), "CITARADICADO")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_Numero_Folios_saliente As TextBox = page.FindControl("TextBox_Numero_Folios_saliente")
                    Dim TextBox_anexos_saliente As TextBox = page.FindControl("TextBox_anexos_saliente")
                    result = refclas_radicacion.agregar_auto_complete("TextBox_anexos_saliente", panel_controles, "GetGuiaRadicaconasp", split(4), "Anexos_Cor")
                    If result <> "YES" Then
                        asigna_datos_edicion_plantilla_radicado = result
                        Exit Function
                    End If
                    Dim TextBox_fecha_documento_saliente As TextBox = page.FindControl("TextBox_fecha_documento_saliente")
                    TextBox_remitente_saliente.Text = stru_plantilla.Destinatario_Cor
                    TextBox_identificacion_destinatario.Text = stru_plantilla.IDENTIFICACION_DESTINATARIO
                    TextBox_asunto_saliente.Text = stru_plantilla.Asunto
                    TextBox_cita_radicado_saliente.Text = stru_plantilla.CITARADICADO
                    TextBox_Numero_Folios_saliente.Text = stru_plantilla.Numero_Folios
                    TextBox_anexos_saliente.Text = stru_plantilla.Anexos_Cor
                    TextBox_fecha_documento_saliente.Text = stru_plantilla.Fecha_Documento
                    Droplist = page.FindControl("DropDownList_remitente_saliente")
                    Droplistarea = page.FindControl("DropDownList_area_remitente_saliente")
                    modal_popup = page.FindControl("ModalPopupExtender_editar_radicacion_saliente")
                    update_controles = page.FindControl("UpdatePnaelcontrolesradicacion_saliente")
                    selected_text = stru_plantilla.Remitente_Cor
                End If
                result = refclas_rad.Retorna_Areas_Departamento_Radicacion(id_empresa,
                                                                           Droplistarea,
                                                                           stru_plantilla.Area_remit_dest_interno)
                If result <> "YES" Then
                    asigna_datos_edicion_plantilla_radicado = result
                    Exit Function
                End If
                result = refclas_rad.Lista_usuarios_gestion_internos_por_area(id_organigrama,
                                                                              stru_plantilla.Area_remit_dest_interno,
                                                                              Droplist,
                                                                              id_empresa,
                                                                              update_controles,
                                                                              selected_text)
                If result <> "YES" Then
                    asigna_datos_edicion_plantilla_radicado = result
                    Exit Function
                End If
            End If
            updatepanel_Asigana_datos_validacion_edicion.Update()
            modal_popup.Show()
            asigna_datos_edicion_plantilla_radicado = "YES"
        Catch ex As Exception
            asigna_datos_edicion_plantilla_radicado = "Inconsistencia función asigna_datos_edicion_plantilla_radicado " & ex.Message
        End Try
    End Function
    Function retorna_id_escript_campo_plantilla_validacion(ByRef id_escript As Integer, ByVal nombre_campo As String) As String
        Try
            Dim refclas_radicacion As New ClassRadicador
            Dim split() As String = HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim result As String = ""
            '***************************************************************
            'Lista las opciones plantilla
            '***************************************************************
            Dim Estado_opcion_fecha As Integer = 0
            Dim Estado_opcion_cita_respuesta As Integer = 0
            Dim Estado_opcion_radicado_general As Integer = 0
            Dim Estado_opcion_valida_externo As Integer = 0
            '---------------------------------------
            '------Lista opciones plantilla
            '---------------------------------------
            Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(split(1),
                                                                                           Estado_opcion_fecha,
                                                                                           Estado_opcion_cita_respuesta,
                                                                                           Estado_opcion_radicado_general,
                                                                                           Estado_opcion_valida_externo)
            If result <> "YES" Then
                retorna_id_escript_campo_plantilla_validacion = result
                Exit Function
            End If
            Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
            Dim Matri_Datos() As Campos_Plantilla
            Erase Matri_Datos
            result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(split(1),
                                                                                                Matri_Datos,
                                                                                                Estado_opcion_fecha,
                                                                                                Estado_opcion_cita_respuesta,
                                                                                                Estado_opcion_radicado_general)
            If result <> "YES" Then
                retorna_id_escript_campo_plantilla_validacion = result
                Exit Function
            End If
            '---------------------------------------
            '------Asigna datos campos validacion
            '---------------------------------------
            Dim ref_Class_ra_script_actividades As New Class_ra_script_actividades
            Dim matri() As validacion_plantilla
            Erase matri
            result = ref_Class_ra_script_actividades.lista_campos_Validacion_plantilla(Val(split(1)),
                                                                                       matri)
            If result <> "YES" Then
                retorna_id_escript_campo_plantilla_validacion = result
                Exit Function
            End If

            If Not matri Is Nothing Then
                For i2 As Integer = 0 To matri.Length - 1
                    For i3 As Integer = 0 To Matri_Datos.Length - 1
                        If Matri_Datos(i3).Campo_Plantilla = matri(i2).Campo_Plantilla Then
                            Matri_Datos(i3).TIPO_SCRIPT = matri(i2).TIPO_SCRIPT
                            Matri_Datos(i3).COMBINACION_TECLA = matri(i2).COMBINACION_TECLA
                            Matri_Datos(i3).VALOR_SCRIPT = matri(i2).VALOR_SCRIPT
                            Matri_Datos(i3).ESTADO_ESCRIPT = matri(i2).ESTADO_ESCRIPT
                            Matri_Datos(i3).PLATAFORMA_SCRIPT = matri(i2).PLATAFORMA_SCRIPT
                            Matri_Datos(i3).ID_SCRIPT = matri(i2).ID_SCRIPT

                        End If
                    Next
                Next
            End If
            Dim Tipo_script As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            result = Class_plantilla_validacion.Retorna_Tipo_Validacion_Campo(Matri_Datos,
                                                                              nombre_campo,
                                                                              Tipo_script,
                                                                              id_escript)
            If result <> "YES" Then
                retorna_id_escript_campo_plantilla_validacion = result
                Exit Function
            End If
            retorna_id_escript_campo_plantilla_validacion = "YES"
        Catch ex As Exception
            retorna_id_escript_campo_plantilla_validacion = "Inconsistencia función retorna_id_escript_campo_plantilla_validacion " & ex.Message
        End Try
    End Function

    Function Actualiza_campos_dinamicos_plantilla_db(ByRef pag As Page,
                                                     ByVal Codigo_Plantilla As Integer,
                                                     ByVal nombre_plantilla As String,
                                                     ByRef Matri_Datos() As Campos_Plantilla,
                                                     ByVal consecutivo_radicado As String) As String
        Erase Matri_Datos
        Dim Result As String = Lista_Campos_Adicionales_Plantilla(Codigo_Plantilla,
                                                                  Matri_Datos)
        If Result <> "YES" Then
            Actualiza_campos_dinamicos_plantilla_db = Result
            Exit Function
        End If
        If Matri_Datos Is Nothing Then
            Actualiza_campos_dinamicos_plantilla_db = "La plantilla no contiene campos dinamicos"
            Exit Function
        End If
        For i As Integer = 0 To Matri_Datos.Length - 1
            Dim control As Object = pag.FindControl("RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit")
            If Not control Is Nothing Then
                If control.GetType.ToString = "System.Web.UI.WebControls.DropDownList" Then
                    Dim droplist As DropDownList = control
                    Matri_Datos(i).ID_CAMPO_ASPNET = "RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit"
                    Matri_Datos(i).TEXTO_CAMPO = droplist.SelectedValue
                Else
                    Dim la = control.GetType.ToString
                    Matri_Datos(i).ID_CAMPO_ASPNET = "RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit"
                    Matri_Datos(i).TEXTO_CAMPO = control.text
                End If

            End If
        Next
        Dim cambios_campos As String = ""
        Dim campos_sql As String = "update " & nombre_plantilla
        For i As Integer = 0 To Matri_Datos.Length - 1
            If Matri_Datos(i).Campo_Obligatorio = 1 And Matri_Datos(i).TEXTO_CAMPO = "" Then
                Actualiza_campos_dinamicos_plantilla_db = "El campo " & Matri_Datos(i).Alias_Campo & " es obligatorio"
                Exit Function
            End If
            If i = 0 Then
                cambios_campos = cambios_campos & Matri_Datos(i).Campo_Plantilla & "=" & Matri_Datos(i).TEXTO_CAMPO
                If Matri_Datos(i).TEXTO_CAMPO = "" Then
                    campos_sql = campos_sql & " set " & Matri_Datos(i).Campo_Plantilla & "=null"
                Else
                    campos_sql = campos_sql & " set " & Matri_Datos(i).Campo_Plantilla & "='" & Matri_Datos(i).TEXTO_CAMPO & "'"
                End If

            Else
                cambios_campos = cambios_campos & "  " & Matri_Datos(i).Campo_Plantilla & "=" & Matri_Datos(i).TEXTO_CAMPO
                If Matri_Datos(i).TEXTO_CAMPO = "" Then
                    campos_sql = campos_sql & "," & Matri_Datos(i).Campo_Plantilla & "=null"
                Else
                    campos_sql = campos_sql & "," & Matri_Datos(i).Campo_Plantilla & "='" & Matri_Datos(i).TEXTO_CAMPO & "'"
                End If

            End If
        Next
        campos_sql = campos_sql & " where Consecutivo_Rad='" & consecutivo_radicado & "'"
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = Date.Now
        Result = refclas_gestion_fechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Actualiza_campos_dinamicos_plantilla_db = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("RA_ID_USUARIO")
        Dim logi_user As String = HttpContext.Current.Session.Item("RA_LOGIN_USER")
        Dim hour As String = Date.Now.Hour
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try

            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = campos_sql
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_campos_dinamicos_plantilla_db = "Imposible actualizar tipo documento tipo tramite  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            campos_sql = "INSERT INTO ra_log_radicados (desc_op,USER_OPER,ID_USER,DATE_TRANS,CONSECUTIVO_RADICADO,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES (" &
                "'" & "ACTUALIZA TIPO TRAMITE DOCUMENTO" & "','" & logi_user & "','" & id_user & "','" & date1al & "','" & consecutivo_radicado & "','" & cambios_campos &
                "','" & iphost & "','" & hour.ToString & "','" & "RADICACION" & "')"
            myCommand.CommandText = campos_sql
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_campos_dinamicos_plantilla_db = "Imposible actualizar fecha limite de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_campos_dinamicos_plantilla_db = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_campos_dinamicos_plantilla_db = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_campos_dinamicos_plantilla_db = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Lista_campos_dinamicos_edicion_plantilla(ByRef pag As Page,
                                                      ByVal Codigo_Plantilla As Integer,
                                                      ByVal nombre_plantilla As String,
                                                      ByVal tipo_plantilla As String) As String
        Try
            Dim Matri_Datos() As Campos_Plantilla
            Erase Matri_Datos
            Dim Result As String = Lista_Campos_Adicionales_Plantilla(Codigo_Plantilla,
                                                                      Matri_Datos)
            If Result <> "YES" Then
                Lista_campos_dinamicos_edicion_plantilla = Result
                Exit Function
            End If
            '******************************************************************
            'Agrega controles dinamicos
            '******************************************************************
            Dim tablecontrolesdinamicos As Table
            Dim pane As Panel
            Dim refclas As New ClassRadicador
            If tipo_plantilla = "RADICACION ENTRANTE" Then
                tablecontrolesdinamicos = pag.FindControl("Table_edita_campos_dinamicos")
                pane = pag.FindControl("Panel_dinamico_edita_campos_dinamicos")
            Else
                tablecontrolesdinamicos = pag.FindControl("Table_edita_campos_dinamicos_saliente")
                pane = pag.FindControl("Panel_dinamico_edita_campos_dinamicos_saliente")
            End If

            pane.Controls.Clear()
            Dim objRow As TableRow
            Dim objCell As TableCell
            Dim LabelBox() As Label = {}
            Dim m_TextBoxes() As TextBox = {}
            Dim _ComboBox() As DropDownList = {}
            Dim _image() As ImageButton = {}
            Dim Contador_Control As Integer = 0
            Dim Contador_row As Integer = 1
            If Matri_Datos Is Nothing Then
                Lista_campos_dinamicos_edicion_plantilla = "YES"
                Exit Function
            End If
            If Matri_Datos.Length > 1 Then
                For k As Integer = 0 To Matri_Datos.Length - 1
                    If Matri_Datos(k).Estado_Campo = 1 And Matri_Datos(k).campo_sistema <> 1 Then

                        '--------------------------------------------------------------
                        'Agrega el label
                        '--------------------------------------------------------------
                        objCell = New TableCell
                        objRow = New TableRow
                        'objCell.Width = 100
                        objCell.HorizontalAlign = HorizontalAlign.Left
                        objCell.VerticalAlign = VerticalAlign.Top
                        objCell.Wrap = True
                        Contador_Control = Contador_Control + 1
                        ReDim Preserve LabelBox(Contador_Control)
                        LabelBox(Contador_Control) = New Label
                        LabelBox(Contador_Control).Attributes.Add("class", "h6 font-weight-light mt-3")
                        If Matri_Datos(k).Campo_Obligatorio = 1 Then
                            LabelBox(Contador_Control).Text = Trim(Matri_Datos(k).Alias_Campo & "*").Replace("_", " ")
                        Else
                            LabelBox(Contador_Control).Text = Trim(Matri_Datos(k).Alias_Campo).Replace("_", " ")
                        End If
                        LabelBox(Contador_Control).ID = Trim(Matri_Datos(k).Alias_Campo & "-EDIT")
                        LabelBox(Contador_Control).Font.Size = 14
                        LabelBox(Contador_Control).ForeColor = Drawing.Color.Black
                        LabelBox(Contador_Control).Font.Name = "'Segoe UI'"
                        Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                        divhtml.Attributes.Add("class", "mt-3")
                        divhtml.Controls.Add(LabelBox(Contador_Control))
                        objCell.Controls.Add(divhtml)
                        objRow.Cells.Add(objCell)
                        tablecontrolesdinamicos.Rows.Add(objRow)
                        '---------------------------------------------------------------
                        'Agrega campo texbox
                        '---------------------------------------------------------------
                        objRow = New TableRow
                        objCell = New TableCell
                        Dim id_campo_aspnet As String = "RE_" & Matri_Datos(k).Campo_Plantilla & "-" & Matri_Datos(k).Campo_Plantilla & "-" & Matri_Datos(k).Tipo_Campo & "-Edit"
                        Matri_Datos(k).ID_CAMPO_ASPNET = id_campo_aspnet
                        objCell.Wrap = False
                        Contador_Control = Contador_Control + 1
                        If Matri_Datos(k).Comportamiento_Campo = "SELECCION" Then
                            ReDim Preserve _ComboBox(Contador_Control)
                            _ComboBox(Contador_Control) = New DropDownList
                            _ComboBox(Contador_Control).ID = id_campo_aspnet
                            _ComboBox(Contador_Control).Attributes.Add("class", "form-control")
                            objCell.Controls.Add(_ComboBox(Contador_Control))
                            refclas.Lista_Valores_campo_seleccion_plantilla_radicacion(Codigo_Plantilla, Matri_Datos(k).Campo_Plantilla,
                                                                                       _ComboBox(Contador_Control))
                        Else
                            ReDim Preserve m_TextBoxes(Contador_Control)
                            m_TextBoxes(Contador_Control) = New TextBox
                            m_TextBoxes(Contador_Control).Attributes.Add("innerText", Matri_Datos(k).Campo_Plantilla)
                            m_TextBoxes(Contador_Control).ID = id_campo_aspnet
                            m_TextBoxes(Contador_Control).Columns = 50
                            m_TextBoxes(Contador_Control).Attributes.Add("class", "form-control")
                            objCell.Controls.Add(m_TextBoxes(Contador_Control))
                            If Matri_Datos(k).Tipo_Campo = "DATE" Then
                                '-------------------------------------------------------
                                'Agrega boton calendario
                                '-------------------------------------------------------
                                Result = refclas.Agregar_Control_Calendario_dinamico(_image, Contador_Control, objCell, objRow, id_campo_aspnet, m_TextBoxes(Contador_Control).ID.ToString, pane)
                                If Result <> "YES" Then
                                End If
                            Else
                                '******************************************************************
                                'Agregar auto completar
                                '******************************************************************
                                Result = refclas.agregar_auto_complete(m_TextBoxes(Contador_Control).ID, pane, "GetGuiaRadicaconasp", nombre_plantilla, Matri_Datos(k).Campo_Plantilla)
                                If Result <> "YES" Then
                                    'Resultado_General = Result

                                End If

                            End If
                        End If
                        objRow.Cells.Add(objCell)
                        tablecontrolesdinamicos.Rows.Add(objRow)
                    End If
                Next
                pane.Controls.Add(tablecontrolesdinamicos)
            End If
            Lista_campos_dinamicos_edicion_plantilla = "YES"
        Catch ex As Exception
            Lista_campos_dinamicos_edicion_plantilla = "Función Lista_campos_dinamicos_edicion_plantilla " & ex.Message
        End Try
    End Function
    Function asigna_datos_plantilla_campos_dinamicos_DB(ByRef page As Page) As String
        Try
            Dim refclas As New ClassRaConsultaRadicados
            Dim refclas_radicacion As New ClassRadicador
            Dim split() As String = HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim Matri_Datos() As Campos_Plantilla
            Erase Matri_Datos
            Dim Result As String = Lista_Campos_Adicionales_Plantilla(split(1),
                                                                      Matri_Datos)
            If Matri_Datos Is Nothing Then
                asigna_datos_plantilla_campos_dinamicos_DB = "La plantilla no contiene campos dinamicos"
                Exit Function
            End If
            If Result <> "YES" Then
                asigna_datos_plantilla_campos_dinamicos_DB = Result
                Exit Function
            End If
            Dim update As UpdatePanel
            If split(2) = "RADICACION ENTRANTE" Then
                update = page.FindControl("UpdatePanel_edita_campos_dinamicos")
            Else
                update = page.FindControl("UpdatePanel_edita_campos_dinamicos_saliente")
            End If

            Dim hdnEmailID_VAL As Object = page.FindControl("hdnEmailID_VAL")
            If hdnEmailID_VAL.Value = "-1" Then
                asigna_datos_plantilla_campos_dinamicos_DB = "Debe seleccionar el registro a editar"
                Exit Function
            End If

            Dim campos_sql As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If i = 0 Then
                    campos_sql = campos_sql & Matri_Datos(i).Campo_Plantilla
                Else
                    campos_sql = campos_sql & "," & Matri_Datos(i).Campo_Plantilla
                End If
            Next
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select " & campos_sql & " from " & split(4) & " where Consecutivo_Rad='" &
            hdnEmailID_VAL.Value & "'"
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                asigna_datos_plantilla_campos_dinamicos_DB = " función asigna_datos_plantilla_campos_dinamicos_DB dice  " & Result
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i2 As Integer = 0 To Matri_Datos.Length - 1
                    If Dat_reader.Tables(0).Rows(0).IsNull(Matri_Datos(i2).Campo_Plantilla) = False Then
                        If Matri_Datos(i2).Tipo_Campo = "DATE" Then
                            Dim splidatos() As String = Nothing
                            If Dat_reader.Tables(0).Rows(0).Item(Matri_Datos(i2).Campo_Plantilla).ToString = "" Then
                                Matri_Datos(i2).TEXTO_CAMPO = ""
                            Else
                                If InStr(Dat_reader.Tables(0).Rows(0).Item(Matri_Datos(i2).Campo_Plantilla).ToString, "\") > 0 Then
                                    splidatos = Left(Dat_reader.Tables(0).Rows(0).Item(Matri_Datos(i2).Campo_Plantilla).ToString, 10).Split("\")
                                End If
                                If InStr(Dat_reader.Tables(0).Rows(0).Item(Matri_Datos(i2).Campo_Plantilla).ToString, "/") > 0 Then
                                    splidatos = Left(Dat_reader.Tables(0).Rows(0).Item(Matri_Datos(i2).Campo_Plantilla).ToString, 10).Split("/")
                                End If
                                If InStr(Dat_reader.Tables(0).Rows(0).Item(Matri_Datos(i2).Campo_Plantilla).ToString, "-") > 0 Then
                                    splidatos = Left(Dat_reader.Tables(0).Rows(0).Item(Matri_Datos(i2).Campo_Plantilla).ToString, 10).Split("-")
                                End If
                                If splidatos.Length >= 2 Then
                                    Matri_Datos(i2).TEXTO_CAMPO = splidatos(0) & "-" & splidatos(1) & "-" & splidatos(2)
                                Else
                                    Matri_Datos(i2).TEXTO_CAMPO = Left(Dat_reader.Tables(0).Rows(0).Item(Matri_Datos(i2).Campo_Plantilla).ToString, 10)
                                End If

                            End If

                        Else
                            Matri_Datos(i2).TEXTO_CAMPO = Dat_reader.Tables(0).Rows(0).Item(Matri_Datos(i2).Campo_Plantilla)
                        End If

                    Else
                        Matri_Datos(i2).TEXTO_CAMPO = ""
                    End If
                Next
            End If
            For i As Integer = 0 To Matri_Datos.Length - 1
                Dim control As Object = page.FindControl("RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit")
                If Not control Is Nothing Then
                    If control.GetType.ToString = "System.Web.UI.WebControls.DropDownList" Then
                        Dim droplist As DropDownList = control
                        droplist.SelectedValue = Matri_Datos(i).TEXTO_CAMPO
                    Else
                        control.text = Matri_Datos(i).TEXTO_CAMPO
                    End If

                End If
            Next
            update.Update()
            asigna_datos_plantilla_campos_dinamicos_DB = "YES"
        Catch ex As Exception
            asigna_datos_plantilla_campos_dinamicos_DB = "Inconsistencia función asigna_datos_plantilla_campos_dinamicos_DB " & ex.Message
        End Try
    End Function
    Function Retorna_fecha_radicado(ByVal nombre_plantilla As String,
                                    ByVal consecutivo_radicado As String,
                                    ByRef fecha_radicado As String) As String
        Try
            Dim Parametro_Consulta As String = "Select Fecha_Radicado from " & nombre_plantilla & "  where Consecutivo_Rad='" & consecutivo_radicado & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_fecha_radicado = " Error listando fecha radicado   " & consecutivo_radicado
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Retorna_fecha_radicado = "Imposible encontrar fecha radicado " & consecutivo_radicado
                Exit Function
            Else
                fecha_radicado = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_fecha_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_fecha_radicado = "Inconsistencia general función Retorna_fecha_radicado " & ex.Message
        End Try
    End Function
    Function Lista_Campos_Adicionales_Plantilla_del_sistema(ByVal Id_Plantilla As Integer, ByRef Matri_Datos() As Campos_Plantilla) As String
        '-----------------------------------------------------
        'Funcion : Lista los campos y el detalle de los campos
        'con los parametors id de la plantillas y retorna una
        'matriz con la estructura del sistema
        'Fecha : 2016-03-23
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select * from detalle_plantilla_radicado where System_Plantilla_Radicado_id_Plantilla =" &
            Id_Plantilla & " order by Orden_Campo"
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_Campos_Adicionales_Plantilla_del_sistema = " Error listando campos adicionales   " & Parametro_Consulta
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_Campos_Adicionales_Plantilla_del_sistema = "YES"
                Exit Function
            Else
                Dim Iconta As Integer = 0

                For zi As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Dim nombre_campo As String = Dat_reader.Tables(0).Rows(zi).Item(1).ToString
                    If nombre_campo = "CITARADICADO" Or nombre_campo = "FECHALIMITERESPUESTA" Or nombre_campo = "IDENTIFICACION_REMITENTE" Or nombre_campo = "IDENTIFICACION_DESTINATARIO" Then
                        ReDim Preserve Matri_Datos(Iconta)
                        If Dat_reader.Tables(0).Rows(zi).IsNull(0) = False Then
                            Matri_Datos(Iconta).System_Plantilla_Radicado_id_Plantilla = Dat_reader.Tables(0).Rows(zi).Item(0).ToString
                        Else
                            Matri_Datos(Iconta).System_Plantilla_Radicado_id_Plantilla = 0
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(1) = False Then
                            Matri_Datos(Iconta).Campo_Plantilla = Dat_reader.Tables(0).Rows(zi).Item(1).ToString
                        Else
                            Matri_Datos(Iconta).Campo_Plantilla = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(2) = False Then
                            Matri_Datos(Iconta).Tipo_Campo = Dat_reader.Tables(0).Rows(zi).Item(2).ToString
                        Else
                            Matri_Datos(Iconta).Tipo_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(3) = False Then
                            Matri_Datos(Iconta).Comportamiento_Campo = Dat_reader.Tables(0).Rows(zi).Item(3).ToString
                        Else
                            Matri_Datos(Iconta).Comportamiento_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(4) = False Then
                            Matri_Datos(Iconta).Alias_Campo = Dat_reader.Tables(0).Rows(zi).Item(4).ToString
                        Else
                            Matri_Datos(Iconta).Alias_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(5) = False Then
                            Matri_Datos(Iconta).Orden_Campo = Iconta
                        Else
                            Matri_Datos(Iconta).Orden_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(6) = False Then
                            Matri_Datos(Iconta).Estado_Campo = Dat_reader.Tables(0).Rows(zi).Item(6).ToString
                        Else
                            Matri_Datos(Iconta).Estado_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(7) = False Then
                            Matri_Datos(Iconta).Descripcion_Campo = Dat_reader.Tables(0).Rows(zi).Item(7).ToString
                        Else
                            Matri_Datos(Iconta).Descripcion_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(8) = False Then
                            Matri_Datos(Iconta).Campo_Obligatorio = Dat_reader.Tables(0).Rows(zi).Item(8).ToString
                        Else
                            Matri_Datos(Iconta).Campo_Obligatorio = "0"
                        End If

                        Iconta = Iconta + 1
                    End If
                Next
                Lista_Campos_Adicionales_Plantilla_del_sistema = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_Campos_Adicionales_Plantilla_del_sistema = "Funcion Lista_Campos_Adicionales_Plantilla_del_sistema " & ex.Message
        End Try
    End Function
    Function Lista_Campos_Adicionales_Plantilla(ByVal Id_Plantilla As Integer,
                                                ByRef Matri_Datos() As Campos_Plantilla) As String
        '-----------------------------------------------------
        'Funcion : Lista los campos y el detalle de los campos
        'con los parametors id de la plantillas y retorna una
        'matriz con la estructura 
        'Fecha : 2014-04-07
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select * from detalle_plantilla_radicado where System_Plantilla_Radicado_id_Plantilla =" &
            Id_Plantilla & " order by Orden_Campo"
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_Campos_Adicionales_Plantilla = " Error listando campos adicionales   " & Parametro_Consulta
                Return Lista_Campos_Adicionales_Plantilla
                Exit Function
            End If
            Dim Iconta2 As Integer = 0
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_Campos_Adicionales_Plantilla = "YES"
                Exit Function
            Else
                Dim Iconta As Integer = 0

                For zi As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Dim nombre_campo As String = Dat_reader.Tables(0).Rows(zi).Item(1).ToString
                    If nombre_campo <> "CITARADICADO" And nombre_campo <> "FECHALIMITERESPUESTA" And nombre_campo <> "IDENTIFICACION_REMITENTE" And nombre_campo <> "IDENTIFICACION_DESTINATARIO" Then
                        ReDim Preserve Matri_Datos(Iconta)
                        If Dat_reader.Tables(0).Rows(zi).IsNull(0) = False Then
                            Matri_Datos(Iconta).System_Plantilla_Radicado_id_Plantilla = Dat_reader.Tables(0).Rows(zi).Item(0).ToString
                        Else
                            Matri_Datos(Iconta).System_Plantilla_Radicado_id_Plantilla = 0
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(1) = False Then
                            Matri_Datos(Iconta).Campo_Plantilla = Dat_reader.Tables(0).Rows(zi).Item(1).ToString
                        Else
                            Matri_Datos(Iconta).Campo_Plantilla = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(2) = False Then
                            Matri_Datos(Iconta).Tipo_Campo = Dat_reader.Tables(0).Rows(zi).Item(2).ToString
                        Else
                            Matri_Datos(Iconta).Tipo_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(3) = False Then
                            Matri_Datos(Iconta).Comportamiento_Campo = Dat_reader.Tables(0).Rows(zi).Item(3).ToString
                        Else
                            Matri_Datos(Iconta).Comportamiento_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(4) = False Then
                            Matri_Datos(Iconta).Alias_Campo = Dat_reader.Tables(0).Rows(zi).Item(4).ToString
                        Else
                            Matri_Datos(Iconta).Alias_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(5) = False Then
                            Matri_Datos(Iconta).Orden_Campo = Iconta
                        Else
                            Matri_Datos(Iconta).Orden_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(6) = False Then
                            Matri_Datos(Iconta).Estado_Campo = Dat_reader.Tables(0).Rows(zi).Item(6).ToString
                        Else
                            Matri_Datos(Iconta).Estado_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(7) = False Then
                            Matri_Datos(Iconta).Descripcion_Campo = Dat_reader.Tables(0).Rows(zi).Item(7).ToString
                        Else
                            Matri_Datos(Iconta).Descripcion_Campo = ""
                        End If
                        If Dat_reader.Tables(0).Rows(zi).IsNull(8) = False Then
                            Matri_Datos(Iconta).Campo_Obligatorio = Dat_reader.Tables(0).Rows(zi).Item(8).ToString
                        Else
                            Matri_Datos(Iconta).Campo_Obligatorio = "0"
                        End If

                        Iconta = Iconta + 1
                    End If
                Next
                Lista_Campos_Adicionales_Plantilla = "YES"
                Exit Function
            End If
            Lista_Campos_Adicionales_Plantilla = "YES"
        Catch ex As Exception
            Lista_Campos_Adicionales_Plantilla = "Funcion Lista_Campos_Adicionales_Plantilla " & ex.Message
        End Try
    End Function

    Function Genera_Interface_Gestion_Plantilla_Validacion(ByRef Page1 As Page,
                                                           ByVal id_script As Integer
                                                           ) As String
        Try
            Dim Result As String = ""
            Dim refclas As New ClassRadicador
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION

            Erase Matri_Datos
            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Lista_Campos_Plantilla_Validacion(id_script, Matri_Datos)
            If Result <> "YES" Then
                Genera_Interface_Gestion_Plantilla_Validacion = Result
                Exit Function
            End If
            '****************************************************
            'Retorna nombre plantilla
            '****************************************************
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script, nombre_plantillas)
            If Result <> "YES" Then
                Genera_Interface_Gestion_Plantilla_Validacion = Result
                Exit Function
            End If
            Dim _LabelboxIco As Label() = {}
            Dim m_TextBoxes() As TextBox = {}
            Dim _image() As ImageButton = {}
            Dim LabelBox() As Label = {}
            Dim _ComboBox() As DropDownList = {}
            Dim _CommamdBoton() As Button = {}
            Dim Contador_Control As Integer = 0
            Dim Contador_Combo As Integer = 0
            Dim Contador_Text As Integer = 0
            Dim objRowlibre As TableRow
            objRowlibre = New TableRow
            Dim z2 As Integer = 0
            Dim Update As UpdatePanel = Page1.FindControl("UpdatePanelContenido")
            If Update Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion = "Imposible encontrar el control UpdatePanelContenido"
                Exit Function
            End If
            Update.UpdateMode = UpdatePanelUpdateMode.Conditional
            Dim Table As Table = Page1.FindControl("_ValidacionConsulta")
            If Update Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion = "Imposible encontrar el control _ValidacionConsulta"
                Exit Function
            End If
            'Table.Controls.Clear()
            Dim Panelref As Panel = Page1.FindControl("_Panelvalidacion")
            If Update Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion = "Imposible encontrar el control _Panelvalidacion"
                Exit Function
            End If

            Dim hiden_edit_Hiddenestadoedicion As Object = Page1.FindControl("Hiddenestadoedicion")
            If hiden_edit_Hiddenestadoedicion Is Nothing Then
                Genera_Interface_Gestion_Plantilla_Validacion = "Imposible encontrar el control Hiddenestadoedicion"
                Exit Function
            End If
            Dim objRow As TableRow
            Dim objCell As TableCell
            Dim largo As Integer = 200
            Dim largocombo As Integer = 150
            Dim largocombinado As Integer = 80
            Dim contador_columna As Integer = 0
            For i As Integer = 0 To Matri_Datos.Length - 1
                If i = 0 Then
                    '********************************************************
                    'Lista los campos de ubicacion
                    '********************************************************
                    Dim estado_ubicacion As Integer = 0
                    Dim refclas_rad As New ClassRadicador
                    Result = refclas_rad.Retorna_Estado_Ubicacion_Plantilla_Validacion(id_script, estado_ubicacion)
                    If Result <> "YES" Then
                        Genera_Interface_Gestion_Plantilla_Validacion = Result
                        Exit Function
                    End If
                    If estado_ubicacion = 1 Then
                        Result = generar_campos_ubicacion(_ComboBox,
                                                          objCell,
                                                          objRow,
                                                          Table,
                                                          Matri_Datos,
                                                          Contador_Control,
                                                          _LabelboxIco)
                        If Result <> "YES" Then
                            Genera_Interface_Gestion_Plantilla_Validacion = Result
                            Exit Function
                        End If
                    End If
                End If
                If Matri_Datos(i).Visible_Campo = 1 And Matri_Datos(i).IDENTI_CAMPO <> 1 Then

                    '-------------------------------------------------------------------------------------------
                    '-------------Determina si el campo para agregar es de ubicacion para general la interface
                    '-------------------------------------------------------------------------------------------
                    If Matri_Datos(i).Nombre_Campo = "Pais" Or Matri_Datos(i).Nombre_Campo = "Municipio" _
                        Or Matri_Datos(i).Nombre_Campo = "Departemento" Then

                    Else
                        If Matri_Datos(i).Tipo_Campo = "DATE" Or Matri_Datos(i).Tipo_Campo = "INT" Then
                            objRow = New TableRow
                            objCell = New TableCell
                            ReDim Preserve _LabelboxIco(Contador_Control)
                            _LabelboxIco(Contador_Control) = New Label
                            _LabelboxIco(Contador_Control).Attributes.Add("class", "font-weight-light h6")
                            _LabelboxIco(Contador_Control).Text = UCase(Matri_Datos(i).Nombre_Campo)
                            _LabelboxIco(Contador_Control).ID = Matri_Datos(i).Nombre_Campo & i
                            _LabelboxIco(Contador_Control).ForeColor = Drawing.Color.Black
                            _LabelboxIco(Contador_Control).Font.Name = "'Segoe UI'"
                            objCell.Controls.Add(_LabelboxIco(Contador_Control))
                            objRow.Cells.Add(objCell)
                            Table.Rows.Add(objRowlibre)
                            objCell = New TableCell
                            objRow = New TableRow
                            ReDim Preserve m_TextBoxes(Contador_Control)
                            m_TextBoxes(Contador_Control) = New TextBox
                            m_TextBoxes(Contador_Control).Width = largocombinado
                            m_TextBoxes(Contador_Control).ID = UCase(Matri_Datos(i).Nombre_Campo)
                            m_TextBoxes(Contador_Control).Attributes.Add("class", "form-control")
                            Result = refclas.agregar_auto_complete(m_TextBoxes(Contador_Control).ID, Panelref, "GetGuiaRadicaconasp", nombre_plantillas, Matri_Datos(i).Nombre_Campo)
                            If Result <> "YES" Then
                                Genera_Interface_Gestion_Plantilla_Validacion = Result
                                Exit Function
                            End If
                            objCell.Controls.Add(m_TextBoxes(Contador_Control))
                            objRow.Cells.Add(objCell)
                            '******************************************************************
                            'Agrega imagen al campo date
                            '******************************************************************
                            If Matri_Datos(i).Tipo_Campo = "DATE" Then
                                Dim bhtml As New HtmlControls.HtmlGenericControl("button")
                                bhtml.ID = UCase(Matri_Datos(i).Nombre_Campo) & "_Image"
                                Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                                ihtml.Attributes.Add("class", "fad fa-calendar-alt")
                                bhtml.Controls.Add(ihtml)
                                objCell.Controls.Add(bhtml)
                                '-----Label espacio 
                                _LabelboxIco(Contador_Control) = New Label
                                _LabelboxIco(Contador_Control).Text = "F"
                                _LabelboxIco(Contador_Control).Attributes.Add("class", "font-weight-light h6")
                                _LabelboxIco(Contador_Control).Font.Name = "'Segoe UI'"
                                objCell.Controls.Add(_LabelboxIco(Contador_Control))
                                '-------------------------------------------
                                '-----Agregar calendar al boton imagen
                                '-------------------------------------------
                                Result = refclas.Agregar_Calendar(bhtml.ID.ToString, m_TextBoxes(Contador_Control).ID.ToString, Panelref)
                                If Result <> "YES" Then
                                    Genera_Interface_Gestion_Plantilla_Validacion = Result
                                    Exit Function
                                End If
                            End If
                            m_TextBoxes(Contador_Control) = New TextBox
                            m_TextBoxes(Contador_Control).Width = largocombinado
                            m_TextBoxes(Contador_Control).ID = UCase(Matri_Datos(i).Nombre_Campo) & "-2"
                            m_TextBoxes(Contador_Control).Attributes.Add("class", "form-control")
                            objCell.Controls.Add(m_TextBoxes(Contador_Control))
                            objRow.Cells.Add(objCell)
                            '******************************************************************
                            'Agrega imagen al campo date
                            '******************************************************************
                            If Matri_Datos(i).Tipo_Campo = "DATE" Then
                                Dim bhtml As New HtmlControls.HtmlGenericControl("button")
                                bhtml.ID = UCase(Matri_Datos(i).Nombre_Campo) & "_-2Image"
                                Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                                ihtml.Attributes.Add("class", "fad fa-calendar-alt")
                                bhtml.Controls.Add(ihtml)
                                objCell.Controls.Add(bhtml)
                                '--------------------------------------
                                '-----Agregar claendar al botonimage
                                '--------------------------------------
                                Result = refclas.Agregar_Calendar(bhtml.ID.ToString, m_TextBoxes(Contador_Control).ID.ToString, Panelref)
                                If Result <> "YES" Then
                                    Genera_Interface_Gestion_Plantilla_Validacion = Result
                                    Exit Function
                                End If
                            End If
                            Table.Rows.Add(objRow)
                        Else

                            objCell = New TableCell
                            objRowlibre = New TableRow
                            ReDim Preserve _LabelboxIco(Contador_Control)
                            _LabelboxIco(Contador_Control) = New Label
                            _LabelboxIco(Contador_Control).Text = UCase(Matri_Datos(i).Nombre_Campo)
                            _LabelboxIco(Contador_Control).ID = Matri_Datos(i).Nombre_Campo & i
                            _LabelboxIco(Contador_Control).ForeColor = Drawing.Color.Black
                            _LabelboxIco(Contador_Control).Font.Name = "'Segoe UI'"
                            _LabelboxIco(Contador_Control).Attributes.Add("class", "font-weight-light h6")
                            objCell.Controls.Add(_LabelboxIco(Contador_Control))
                            objRowlibre.Cells.Add(objCell)
                            Table.Rows.Add(objRowlibre)
                            objCell = New TableCell
                            objRowlibre = New TableRow
                            ReDim Preserve m_TextBoxes(Contador_Control)
                            m_TextBoxes(Contador_Control) = New TextBox
                            m_TextBoxes(Contador_Control).Width = largo
                            m_TextBoxes(Contador_Control).ID = UCase(Matri_Datos(i).Nombre_Campo)
                            If Matri_Datos(i).Campo_nombre_pqr = 1 Then
                                m_TextBoxes(Contador_Control).Attributes.Add("onkeypress", "return caracter_especial_nombre(event,this);")
                            End If
                            m_TextBoxes(Contador_Control).Attributes.Add("class", "form-control")

                            objCell.Controls.Add(m_TextBoxes(Contador_Control))
                            Result = refclas.agregar_auto_complete(m_TextBoxes(Contador_Control).ID, Panelref, "GetGuiaRadicaconasp", nombre_plantillas, Matri_Datos(i).Nombre_Campo)
                            If Result <> "YES" Then
                                Genera_Interface_Gestion_Plantilla_Validacion = Result
                                Exit Function
                            End If
                            objRowlibre.Cells.Add(objCell)
                            Table.Rows.Add(objRowlibre)
                        End If
                    End If
                End If
            Next
            Panelref.Controls.Add(Table)
            Genera_Interface_Gestion_Plantilla_Validacion = "YES"
        Catch ex As Exception
            Genera_Interface_Gestion_Plantilla_Validacion = "Inconsistencia función Genera_Interface_Gestion_Plantilla_Validacion " & ex.Message
        End Try

    End Function
    Function generar_campos_ubicacion(ByRef _ComboBox() As DropDownList,
                                      ByRef objcell As TableCell,
                                      ByRef objRow As TableRow,
                                      ByRef Table As Object,
                                      ByRef Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION,
                                      ByRef contador As Integer,
                                      ByRef _LabelboxIco() As Label) As String
        Try
            Dim largocombo As Integer = 150
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).Nombre_Campo = "Pais" Or Matri_Datos(i).Nombre_Campo = "Municipio" _
                       Or Matri_Datos(i).Nombre_Campo = "Departemento" Then
                    objRow = New TableRow
                    objcell = New TableCell
                    ReDim Preserve _LabelboxIco(contador)
                    _LabelboxIco(contador) = New Label
                    _LabelboxIco(contador).Text = UCase(Matri_Datos(i).Nombre_Campo)
                    _LabelboxIco(contador).ID = Matri_Datos(i).Nombre_Campo & i
                    _LabelboxIco(contador).ForeColor = Drawing.Color.Black
                    _LabelboxIco(contador).Attributes.Add("class", "font-weight-light h6")
                    _LabelboxIco(contador).Font.Name = "'Segoe UI'"
                    objcell.Controls.Add(_LabelboxIco(contador))
                    objRow.Cells.Add(objcell)
                    Table.Rows.Add(objRow)
                    ReDim Preserve _ComboBox(contador)
                    objRow = New TableRow
                    objcell = New TableCell
                    _ComboBox(contador) = New DropDownList
                    _ComboBox(contador).ID = UCase(Matri_Datos(i).Nombre_Campo)
                    _ComboBox(contador).Width = largocombo
                    _ComboBox(contador).Attributes.Add("classs", "form-control")
                    Select Case Matri_Datos(i).Nombre_Campo
                        Case "Pais"
                            _ComboBox(contador).Attributes.Add("onchange", "llenardepartamento();")
                        Case "Municipio"
                            _ComboBox(contador).Attributes.Add("onchange", "seleccionmuicipio();")
                        Case "Departemento"
                            _ComboBox(contador).Attributes.Add("onchange", "llenarciudad();")
                    End Select

                    objcell.Controls.Add(_ComboBox(contador))
                    objRow.Cells.Add(objcell)
                    Table.Rows.Add(objRow)

                End If

            Next

            generar_campos_ubicacion = "YES"
        Catch ex As Exception
            generar_campos_ubicacion = "Inconsistencia general funcion " & ex.Message
        End Try
    End Function
    Function Lista_Departamentos_Paises(ByRef RefCombo As DropDownList,
   ByVal Nombre_Pais As String, ByRef update As UpdatePanel) As String
        '-------------------------------------------------------------
        'Funcion : Lista los departamentos, con el parametro nombre
        'del pais, el sistema carga en la interface los datos
        'Fecha : 2014-04-07
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim refclas As New ClassRadicador
            Dim pag1 As Page = update.Page
            'Dim hidepartamento As Object = pag1.FindControl("Hiddenseleciondepartamento")
            'If hidepartamento Is Nothing Then
            '    Lista_Departamentos_Paises = "Imposible el control Hiddenseleciondepartamento en la funcion Lista_Departamentos_Paises"
            '    Exit Function
            'End If
            Dim Result As String = ""
            Dim Id_Pais As String = ""
            Result = refclas.Consulta_Id_Pais(Nombre_Pais, Id_Pais)
            If Result <> "YES" Then
                Lista_Departamentos_Paises = Result
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Nombre_Departamento from depart_radicacion " &
            " where pais_radicacion_id_pais_radicacion = " & Id_Pais
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_Departamentos_Paises = " Error Listando departamentos   " & Result
                Return Lista_Departamentos_Paises
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_Departamentos_Paises = "YES"
                Exit Function
            Else
                RefCombo.Items.Clear()
                RefCombo.Items.Add("SELECCIONE")
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    RefCombo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0).ToString)
                Next
                RefCombo.SelectedIndex = 0
                'hidepartamento.value = RefCombo.Items(0).Text
                'update.Update()
            End If
            Lista_Departamentos_Paises = "YES"
        Catch ex As Exception
            Lista_Departamentos_Paises = ex.Message
        End Try
    End Function
    Function Lista_Departamentos_Paises(ByRef RefCombo As DropDownList,
    ByVal Nombre_Pais As String, ByRef update As UpdatePanel, ByVal nombre_departamento As String) As String
        '-------------------------------------------------------------
        'Funcion : Lista los departamentos, con el parametro nombre
        'del pais, el sistema carga en la interface los datos
        'Fecha : 2014-04-07
        'Ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim refclas As New ClassRadicador
            Dim pag1 As Page = update.Page
            Dim hidepartamento As Object = pag1.FindControl("Hiddenseleciondepartamento")
            If hidepartamento Is Nothing Then
                Lista_Departamentos_Paises = "Imposible el control Hiddenseleciondepartamento en la funcion Lista_Departamentos_Paises"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Id_Pais As String = ""
            Result = refclas.Consulta_Id_Pais(Nombre_Pais, Id_Pais)
            If Result <> "YES" Then
                Lista_Departamentos_Paises = Result
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Nombre_Departamento from depart_radicacion " &
            " where pais_radicacion_id_pais_radicacion = " & Id_Pais
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Lista_Departamentos_Paises = " Error Listando departamentos   " & Result
                Return Lista_Departamentos_Paises
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Lista_Departamentos_Paises = "YES"
                Exit Function
            Else
                RefCombo.Items.Clear()
                RefCombo.Items.Add("SELECCIONE")
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    RefCombo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0).ToString)
                Next
                For i As Integer = 0 To RefCombo.Items.Count - 1
                    If RefCombo.Items(i).Text = nombre_departamento Then
                        RefCombo.SelectedIndex = i
                        hidepartamento.value = RefCombo.Items(i).Text
                        Exit For
                    End If
                Next

                update.Update()
            End If
            Lista_Departamentos_Paises = "YES"
        Catch ex As Exception
            Lista_Departamentos_Paises = ex.Message
        End Try
    End Function
    Public Function lista_Municipios_Departamentos_carga_inicio(ByRef RefCombo As DropDownList,
     ByVal Nombre_Depart As String, ByVal update As UpdatePanel) As String
        Try
            Dim refclas As New ClassRadicador
            Dim pag1 As Page = RefCombo.Page
            Dim hidemunicipio As Object = pag1.FindControl("Hiddenmunicipio")
            If hidemunicipio Is Nothing Then
                lista_Municipios_Departamentos_carga_inicio = "Imposible el control hidemunicipio en la funcion lista_Municipios_Departamentos_carga_inicio"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Id_Dep As String = ""
            Result = refclas.Consulta_Id_Departamento(Nombre_Depart, Id_Dep)
            If Result <> "YES" Then
                lista_Municipios_Departamentos_carga_inicio = Result
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select * from municipio_radicacion " &
            " where Depart_Radicacion_Id_Depart_Radicacion = " & Id_Dep
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                lista_Municipios_Departamentos_carga_inicio = " Error Listando municipios o ciudades   " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                lista_Municipios_Departamentos_carga_inicio = "YES"
                Exit Function
            Else
                RefCombo.Items.Clear()
                RefCombo.Items.Add("SELECCIONE")
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    RefCombo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(2))
                Next
                RefCombo.SelectedIndex = 0
                'hidemunicipio.value = RefCombo.Items(0).Text
                'update.Update()
            End If
            lista_Municipios_Departamentos_carga_inicio = "YES"
        Catch ex As Exception
            lista_Municipios_Departamentos_carga_inicio = ex.Message
        End Try
    End Function
    Public Function lista_Municipios_Departamentos(ByRef RefCombo As DropDownList,
    ByVal Nombre_Depart As String) As String
        Try
            Dim refclas As New ClassRadicador
            Dim pag1 As Page = RefCombo.Page
            Dim hidemunicipio As Object = pag1.FindControl("Hiddenmunicipio")
            If hidemunicipio Is Nothing Then
                lista_Municipios_Departamentos = "Imposible el control hidemunicipio en la funcion lista_Municipios_Departamentos"
                Exit Function
            End If
            Dim Result As String = ""
            Dim Id_Dep As String = ""
            Result = refclas.Consulta_Id_Departamento(Nombre_Depart, Id_Dep)
            If Result <> "YES" Then
                lista_Municipios_Departamentos = Result
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select * from municipio_radicacion " &
            " where Depart_Radicacion_Id_Depart_Radicacion = " & Id_Dep
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                lista_Municipios_Departamentos = " Error Listando municipios o ciudades   " & Result
                Return lista_Municipios_Departamentos
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                lista_Municipios_Departamentos = "YES"
                Exit Function
            Else
                RefCombo.Items.Clear()
                RefCombo.Items.Add("SELECCIONE")
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    RefCombo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(2))
                Next
                'hidemunicipio.value = RefCombo.Items(0).Text
                RefCombo.SelectedIndex = 0
            End If
            lista_Municipios_Departamentos = "YES"
        Catch ex As Exception
            lista_Municipios_Departamentos = ex.Message
        End Try
    End Function
    'GENERA CONSULTA SQL PARA VALIDACION
    Function Genera_Sql_Consulta_Validacion_gestion(ByVal id_script As Integer,
                                                    ByVal page1 As Page,
                                                    ByRef sql_consulta As String,
                                                    ByVal id_des_est As Integer) As String
        '*******************************************************************************
        'Funcion : Genera consulta para plantillas de validacion, con los parametros
        'seleccionados en la interface
        'Fecha : 2014-08-03
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************************************
        Try
            Dim refclasradicado As New ClassRadicador
            HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = ""
            Dim Result As String = ""
            Dim Matri_Datos() As CAMPOS_PLANTILLA_VALIDACION
            Erase Matri_Datos
            Dim prefijocampo As String = ""
            Dim scripma As GridView = page1.FindControl(prefijocampo & "GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl(prefijocampo & "titulo_label_validacion")
            Dim updatelabel As UpdatePanel = page1.FindControl(prefijocampo & "UpdatePanelabel_validacion")
            If scripma Is Nothing Then
                Genera_Sql_Consulta_Validacion_gestion = "Imposible encontrar datagrid  " & prefijocampo & "GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Genera_Sql_Consulta_Validacion_gestion = "Imposible encontrar el control  " & prefijocampo & "titulo_label"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Genera_Sql_Consulta_Validacion_gestion = "Imposible encontrar el control  " & prefijocampo & "UpdatePanelabel_validacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl(prefijocampo & "UpdatePanel_conenido_grid_val_radicacion")
            If updatelabel Is Nothing Then
                Genera_Sql_Consulta_Validacion_gestion = "Imposible encontrar el control  " & prefijocampo & "UpdatePanel_conenido_grid_val_radicacion"
                Exit Function
            End If

            '****************************************************
            'Lista campos plantilla validacion
            '****************************************************
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Result = Class_campos_plantilla_validacion.Lista_Campos_Plantilla_Validacion(id_script, Matri_Datos)
            If Result <> "YES" Then
                Genera_Sql_Consulta_Validacion_gestion = Result
                Exit Function
            End If
            Dim seleccampos As String = "Select "
            If Matri_Datos Is Nothing Then
                Genera_Sql_Consulta_Validacion_gestion = "Imposible encontrar campos validacion"
                Exit Function
            End If
            '****************************************************
            'Retorna nombre plantilla validacion
            '****************************************************
            Dim nombre_plantillas As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                    nombre_plantillas)
            If Result <> "YES" Then
                Genera_Sql_Consulta_Validacion_gestion = Result
                Exit Function
            End If
            Dim campo_comparacion As String = ""
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    seleccampos = " Select " & Matri_Datos(i).Nombre_Campo
                    campo_comparacion = Matri_Datos(i).Nombre_Campo
                    Exit For

                End If
            Next
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO <> 1 Then
                    seleccampos = seleccampos & "," & Matri_Datos(i).Nombre_Campo
                End If
            Next
            Dim sqlfrom As String = " From " & nombre_plantillas
            Dim condicionsql As String = " where " & campo_comparacion & "='" & id_des_est & "' "
            Dim datakey() As String
            Erase datakey
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).IDENTI_CAMPO = 1 Then
                    ReDim Preserve datakey(0)
                    datakey(0) = Matri_Datos(i).Nombre_Campo
                End If

            Next
            If datakey Is Nothing Then
                Genera_Sql_Consulta_Validacion_gestion = "La plantilla " & nombre_plantillas & " no tiene campo identi o primary key"
                Exit Function
            End If
            sql_consulta = seleccampos & " " & sqlfrom & " " & condicionsql
            'If Trim(condicionsql) = "where" Then
            '    labetitle.Text = "Se encontro " & "0" & " registro(s) en la plantilla " & nombre_plantillas
            '    scripma.DataSource = Nothing
            '    scripma.DataBind()
            '    updat.Update()
            '    updatelabel.Update()
            '    Genera_Sql_Consulta_Validacion_gestion = "YES"
            '    Exit Function
            'End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Result <> "YES" Then
                Genera_Sql_Consulta_Validacion_gestion = "Imposible Encontrar datos para listar " & Result
                labetitle.Text = "Imposible Encontrar datos para listar Error (" & Result & ") plantillA (" & nombre_plantillas & ")"
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                scripma.DataSource = Dat_set_zero
                scripma.DataKeyNames = datakey
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                scripma.Rows(0).Visible = False
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) en la plantilla (" & nombre_plantillas & ")"
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                scripma.DataSource = Dat_set_zero
                scripma.DataKeyNames = datakey
                scripma.DataBind()
                scripma.Rows(0).Visible = False
                updat.Update()
                updatelabel.Update()
                Genera_Sql_Consulta_Validacion_gestion = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = sql_consulta
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) en la plantilla " & nombre_plantillas
                scripma.DataKeyNames = datakey
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                Next
                updat.Update()
                updatelabel.Update()
                Genera_Sql_Consulta_Validacion_gestion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Genera_Sql_Consulta_Validacion_gestion = "Inconsistencia Funcion Genera_Sql_Consulta_Validacion " & ex.Message
        End Try
    End Function



    Function asigna_remitente_destinatario_interface_edicion(ByRef pag1 As Page) As String
        Try
            Dim spli() As String = HttpContext.Current.Session.Item("RA_MODULO_SELECCIONADO").ToString.Split("|")
            Dim text_remitente_destintario As TextBox = Nothing
            Dim text_identificacion_remitente_destinatario As TextBox = Nothing
            Dim updat As UpdatePanel = Nothing
            Dim modal_popup As ModalPopupExtender = Nothing
            Dim hidevalorselccion As Object = pag1.FindControl("Hidden_remitente_destinatario")
            Dim refclas_radicado As New ClassRadicador
            If spli.Length > 1 Then
                If spli(2) = "RADICACION ENTRANTE" Then
                    text_remitente_destintario = pag1.FindControl("TextBox_remitente_entrante")
                    text_identificacion_remitente_destinatario = pag1.FindControl("TextBoxIdentificacion_remitente")
                    updat = pag1.FindControl("UpdatePnaelcontrolesradicacion_entrante")
                    modal_popup = pag1.FindControl("ModalPopupExtender_valiacion_plantilla")
                Else
                    text_remitente_destintario = pag1.FindControl("TextBox_remitente_saliente")
                    text_identificacion_remitente_destinatario = pag1.FindControl("TextBox_identificacion_destinatario")
                    updat = pag1.FindControl("UpdatePnaelcontrolesradicacion_saliente")
                    modal_popup = pag1.FindControl("ModalPopupExtender_valiacion_plantilla")
                End If

                If hidevalorselccion.Value = "-1" Then
                    asigna_remitente_destinatario_interface_edicion = "Debe seleccionar el registro a asignar"
                    Exit Function
                End If

                '**********************************************************
                'Solicita campo padre validacion
                '**********************************************************
                Dim campo_validacion As String = ""
                Dim campo_radicacion As String = spli(1)
                Dim nombre_plantilla_validacion As String = ""
                Dim nombre_plantilla_radicacion As String = ""
                Dim Result As String = ""
                Dim Class_plantilla_validacion As New Class_plantilla_validacion
                Result = Class_plantilla_validacion.Retorna_Campo_Validacion_nombre_plantilla_validacion(HttpContext.Current.Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"), nombre_plantilla_radicacion, campo_radicacion)
                If Result <> "YES" Then
                    asigna_remitente_destinatario_interface_edicion = Result
                    Exit Function
                End If

                '**********************************************************
                'Lista campos destino y fuente validacion
                '**********************************************************
                Dim matri_campo_fuente_destino() As asignacion_plantilla_validacion
                Erase matri_campo_fuente_destino
                Result = Class_plantilla_validacion.Lista_Campos_fuente_destino_validacion_dinamica_externa(HttpContext.Current.Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"), matri_campo_fuente_destino)
                If Result <> "YES" Then
                    asigna_remitente_destinatario_interface_edicion = Result
                    Exit Function
                End If
                '*********************************************************
                'Retorna  plantilla validacion 
                '*********************************************************

                Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(HttpContext.Current.Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"),
                                                                                        nombre_plantilla_validacion)
                If Result <> "YES" Then
                    asigna_remitente_destinatario_interface_edicion = Result
                    Exit Function
                End If
                Dim id_plantilla_validacion As Integer = -1
                Result = Class_plantilla_validacion.Retorna_id_plantilla_validacion_nombre(nombre_plantilla_validacion,
                                                                                           id_plantilla_validacion)
                If Result <> "YES" Then
                    asigna_remitente_destinatario_interface_edicion = Result
                    Exit Function
                End If
                '*********************************************************
                'Retorna id plantilla radciacion
                '*********************************************************
                Dim id_plantilla_radicacion As Integer = -1
                Dim Ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
                Result = Ref_Class_system_plantilla_radicado.SolicitaIdPlantillaRadicado(id_plantilla_radicacion,
                                                                                           nombre_plantilla_radicacion)
                If Result <> "YES" Then
                    asigna_remitente_destinatario_interface_edicion = Result
                    Exit Function
                End If
                '*********************************************************
                'Retorna campo crieterio busqueda validacion
                '********************************************************
                Result = Class_plantilla_validacion.Lista_Campo_criterio_busqueda_Plantilla_Validacion(HttpContext.Current.Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"), campo_validacion)
                If Result <> "YES" Then
                    asigna_remitente_destinatario_interface_edicion = Result
                    Exit Function
                End If

                '*********************************************************
                'Retorna campo primary key plantilla validacion
                '*********************************************************
                Dim nombre_campo_prinary As String = ""
                Dim Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
                Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
                Result = Class_campos_plantilla_validacion.Retorna_Campo_Primary_key_plantilla_validacion(id_plantilla_validacion,
                                                                                                          nombre_campo_prinary)
                If Result <> "YES" Then
                    asigna_remitente_destinatario_interface_edicion = Result
                    Exit Function
                End If
                Dim val_leng As Integer = 0
                If matri_campo_fuente_destino Is Nothing Then
                Else
                    val_leng = matri_campo_fuente_destino.Length
                End If
                ReDim Preserve matri_campo_fuente_destino(val_leng)
                matri_campo_fuente_destino(val_leng).Nombre_Campo_Fuente_Pla_Validacion = campo_validacion
                matri_campo_fuente_destino(val_leng).Nombre_Campo_Destino_Pla_Radicacion = campo_radicacion
                If campo_radicacion = "REMITENTE_COR" Or campo_radicacion = "DESTINATARIO_COR" Then
                    matri_campo_fuente_destino(val_leng).Tipo_Campo_Destino_Pla_Radciacion = "VARCHAR"
                Else
                    Result = Class_ra_detalle_plantilla_radicado.Retorna_Tipo_Campo_Plantilla_Radicacion(id_plantilla_radicacion,
                    matri_campo_fuente_destino(val_leng).Nombre_Campo_Destino_Pla_Radicacion, matri_campo_fuente_destino(val_leng).Tipo_Campo_Destino_Pla_Radciacion)
                    If Result <> "YES" Then
                        asigna_remitente_destinatario_interface_edicion = Result
                        Exit Function
                    End If
                End If
                Result = Class_plantilla_validacion.Asigna_Datos_Fuente_Destino_Plantilla_Validacion(HttpContext.Current.Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"), matri_campo_fuente_destino, nombre_plantilla_validacion, nombre_campo_prinary, hidevalorselccion.Value)
                If Result <> "YES" Then
                    asigna_remitente_destinatario_interface_edicion = Result
                    Exit Function
                End If
                For i As Integer = 0 To matri_campo_fuente_destino.Length - 1
                    If matri_campo_fuente_destino(i).Nombre_Campo_Destino_Pla_Radicacion = "IDENTIFICACION_REMITENTE" Or matri_campo_fuente_destino(i).Nombre_Campo_Destino_Pla_Radicacion = "IDENTIFICACION_DESTINATARIO" Then
                        text_identificacion_remitente_destinatario.Text = matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion.ToString
                    End If
                    If matri_campo_fuente_destino(i).Nombre_Campo_Destino_Pla_Radicacion = "REMITENTE_COR" Or matri_campo_fuente_destino(i).Nombre_Campo_Destino_Pla_Radicacion = "DESTINATARIO_COR" Then
                        text_remitente_destintario.Text = matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion.ToString
                    End If
                Next
                updat.Update()
                modal_popup.Hide()
            Else
                asigna_remitente_destinatario_interface_edicion = "Función asigna_remitente_destinatario_interface imposible econtrar el tipo de plantilla"
                Exit Function
            End If
            asigna_remitente_destinatario_interface_edicion = "YES"
        Catch ex As Exception
            asigna_remitente_destinatario_interface_edicion = "Inconsistencia función asigna_remitente_destinatario_interface " & ex.Message
        End Try
    End Function

    Function Retorna_existencia_registro_respuesta_radicado(ByVal radicado As String, ByRef estado_registro_respuesta As String) As String
        Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
        Dim Parametro_Consulta As String = "select RADICADO from ra_respuesta_radicado " &
        " where RADICADO ='" & radicado & "' and ESTADO_RESPUESTA=0"
        Try
            Dim Dat_reader As New DataSet
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_existencia_registro_respuesta_radicado = " Función Retorna_existencia_registro_respuesta_radicado dice   " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                estado_registro_respuesta = "NO"
                Retorna_existencia_registro_respuesta_radicado = "YES"
                Exit Function
            Else
                estado_registro_respuesta = "YES"
                Retorna_existencia_registro_respuesta_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_existencia_registro_respuesta_radicado = "Función Retorna_existencia_registro_respuesta_radicado " & ex.Message
        End Try
    End Function
    Function Reasigna_destinatario_externo(ByVal id_respuesta_radicado As Integer, ByVal usuario_admin As String, ByVal nombre_destintario As String, ByVal id_dext As Integer)
        If nombre_destintario = "" Then
            Reasigna_destinatario_externo = "Debe informar el nombre del destinatario o peticionario"
            Exit Function
        End If
        If usuario_admin = "" Then
            Reasigna_destinatario_externo = "Debe informar el nombre del usuario que autoriza"
            Exit Function
        End If
        Dim stru As stru_envio = Nothing
        Dim Result As String = ""
        Dim Refclasgestion As New Classgestionrespuesta
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        '----------------------------------------------
        'Retorna datos de la estructura
        '----------------------------------------------
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado, stru)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If
        If stru.FECHA_RESPUETA <> "" Then
            Reasigna_destinatario_externo = "Hay una respuesta para este tramite imposible cambiar peticionario"
            Exit Function
        End If
        If stru.ID_IMAGEN <> 0 Then
            Reasigna_destinatario_externo = "Hay una plantilla para este tramite imposible cambiar peticionario"
            Exit Function
        End If
        '----------------------------------------------------------
        'Retorna id script plantilla validación
        '----------------------------------------------------------
        Dim Refclasradicador As New ClassRadicador
        Dim id_script As Integer = 0
        Result = Refclasradicador.Retorna_id_script_validacion(stru.system_plantilla_radicado_id_plantilla, "DINAMICOEXTERNO", "REMITENTE_COR", id_script)
        '**********************************************************
        'Solicita campo padre validacion
        '**********************************************************

        Dim campo_validacion As String = ""
        Dim campo_radicacion As String = ""
        Dim nombre_plantilla_validacion As String = ""
        Dim nombre_plantilla_radicacion As String = ""
        Dim Class_plantilla_validacion As New Class_plantilla_validacion
        Result = Class_plantilla_validacion.Retorna_Campo_Validacion_nombre_plantilla_validacion(id_script, nombre_plantilla_radicacion, campo_radicacion)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If

        '**********************************************************
        'Lista campos destino y fuente validacion
        '**********************************************************
        Dim matri_campo_fuente_destino() As asignacion_plantilla_validacion
        Erase matri_campo_fuente_destino
        Result = Class_plantilla_validacion.Lista_Campos_fuente_destino_validacion_dinamica_externa(id_script, matri_campo_fuente_destino)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If
        '*********************************************************
        'Retorna  plantilla validacion 
        '*********************************************************

        Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_script,
                                                                                nombre_plantilla_validacion)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If
        Dim id_plantilla_validacion As Integer = -1
        Result = Class_plantilla_validacion.Retorna_id_plantilla_validacion_nombre(nombre_plantilla_validacion,
                                                                                   id_plantilla_validacion)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If
        '--------------------------------------------
        'Retorna el nombre de la plantilla
        '--------------------------------------------
        Dim nombre_plantilla As String = ""
        Dim radicado As String = ""
        Result = Refclasgestion.Retorna_nombre_plantilla_por_id_respuesta(id_respuesta_radicado, nombre_plantilla, radicado)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If

        '*********************************************************
        'Retorna campo crieterio busqueda validacion
        '********************************************************
        Result = Class_plantilla_validacion.Lista_Campo_criterio_busqueda_Plantilla_Validacion(id_script, campo_validacion)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If

        '*********************************************************
        'Retorna campo primary key plantilla validacion
        '*********************************************************
        Dim nombre_campo_prinary As String = ""
        Dim Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
        Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
        Result = Class_campos_plantilla_validacion.Retorna_Campo_Primary_key_plantilla_validacion(id_plantilla_validacion,
                                                                                                  nombre_campo_prinary)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If
        Dim val_leng As Integer = 0
        If matri_campo_fuente_destino Is Nothing Then
        Else
            val_leng = matri_campo_fuente_destino.Length
        End If
        ReDim Preserve matri_campo_fuente_destino(val_leng)
        matri_campo_fuente_destino(val_leng).Nombre_Campo_Fuente_Pla_Validacion = campo_validacion
        matri_campo_fuente_destino(val_leng).Nombre_Campo_Destino_Pla_Radicacion = campo_radicacion
        If campo_radicacion = "REMITENTE_COR" Or campo_radicacion = "DESTINATARIO_COR" Then
            matri_campo_fuente_destino(val_leng).Tipo_Campo_Destino_Pla_Radciacion = "VARCHAR"
        Else
            Result = Class_ra_detalle_plantilla_radicado.Retorna_Tipo_Campo_Plantilla_Radicacion(stru.system_plantilla_radicado_id_plantilla,
            matri_campo_fuente_destino(val_leng).Nombre_Campo_Destino_Pla_Radicacion, matri_campo_fuente_destino(val_leng).Tipo_Campo_Destino_Pla_Radciacion)
            If Result <> "YES" Then
                Reasigna_destinatario_externo = Result
                Exit Function
            End If
        End If
        Dim nit_identificacion As String = ""
        Result = Class_plantilla_validacion.Asigna_Datos_Fuente_Destino_Plantilla_Validacion(id_script, matri_campo_fuente_destino, nombre_plantilla_validacion, nombre_campo_prinary, id_dext)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If
        Dim update_plantilla_radicacion As String = "Update " & nombre_plantilla_radicacion
        Dim cambios_campos As String = ""
        For i As Integer = 0 To matri_campo_fuente_destino.Length - 1
            If matri_campo_fuente_destino(i).Nombre_Campo_Destino_Pla_Radicacion = "IDENTIFICACION_REMITENTE" Then
                nit_identificacion = matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion
            End If
            cambios_campos = cambios_campos & "(" & matri_campo_fuente_destino(i).Nombre_Campo_Destino_Pla_Radicacion & "=" & matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion & ")"
            If i = 0 Then
                update_plantilla_radicacion = update_plantilla_radicacion & " set " & matri_campo_fuente_destino(i).Nombre_Campo_Destino_Pla_Radicacion & "='" &
                matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion & "'"
            Else
                update_plantilla_radicacion = update_plantilla_radicacion & "," & matri_campo_fuente_destino(i).Nombre_Campo_Destino_Pla_Radicacion & "='" &
               matri_campo_fuente_destino(i).Valor_Campo_Destino_Pla_Radicacion & "'"
            End If
        Next
        update_plantilla_radicacion = update_plantilla_radicacion & ",Remit_Dest_Interno_id_Remit_Dest_Int=" & id_dext
        update_plantilla_radicacion = update_plantilla_radicacion & " where Consecutivo_rad='" & radicado & "'"
        Dim update_respuesta As String = "update ra_respuesta_radicado set codigo_dest_externo=" & id_dext & ",DESTINATARIO='" & nombre_destintario & "' " &
            " where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
        Dim logi_user As String = HttpContext.Current.Session.Item("Login_Usuario_Workfow")
        Dim id_user_wf As Integer = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
        Dim id_ruta_wf As Integer = HttpContext.Current.Session.Item("Id_Ruta_Workflow")
        Dim nombre_ruta_wf As String = ""
        Dim sql_tabla_wf As String = ""
        Dim sql_actualiza_tramite_wf As String = ""
        Dim date1al As String = Date.Now
        Dim Refclas As New ClassListandoTareas
        Dim refclasalmacen As New ClassRadicador
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = "Imposible formatear fecha " & Result
            Exit Function
        End If
        '---------------------------------------------------------------------------
        'Retorna relacion usuario gestion workflow
        '---------------------------------------------------------------------------
        Result = refclasalmacen.Retorna_relacion_usuario_gestion_workflow(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), id_user_wf)
        If Result <> "YES" Then
            Reasigna_destinatario_externo = Result
            Exit Function
        End If
        If id_user_wf <> 0 Then
            '-----------------------------------------------------------------------
            'Retorna id ruta y nombre usuario
            '-----------------------------------------------------------------------
            Result = Refclas.Retorna_id_ruta_usuario_workflow(id_user_wf, id_ruta_wf, nombre_ruta_wf)
            If Result <> "YES" Then
                Reasigna_destinatario_externo = Result
                Exit Function
            End If
        End If
        '---------------------------------------------------------------------------
        'Verfica  existencia_campos_workflow
        '---------------------------------------------------------------------------
        Dim existencia_campos_workflow As String = "NO"
        Dim matri_campos() As String
        Erase matri_campos
        If id_ruta_wf <> 0 Then
            Result = Refclas.Verifica_existencia_campos_listado_ruta(id_ruta_wf, existencia_campos_workflow, matri_campos)
            If Result <> "YES" Then
                Reasigna_destinatario_externo = Result
                Exit Function
            End If
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(id_ruta_wf.ToString,
                                                                     nombre_ruta_wf)
            If Result <> "YES" Then
                Reasigna_destinatario_externo = Result
                Exit Function
            End If
        End If
        '****************************************************
        'Actualiza campos workflow sql_tabla_wf
        '****************************************************
        Dim id_tarea_worklow As Integer = 0
        If existencia_campos_workflow = "YES" Then
            sql_tabla_wf = "Update dat_adic_tar" & nombre_ruta_wf & " set "
            For i As Integer = 0 To matri_campos.Length - 1
                Select Case UCase(matri_campos(i))
                    Case "BENEFICIARIO"
                        If sql_actualiza_tramite_wf = "" Then
                            sql_actualiza_tramite_wf = sql_tabla_wf & " BENEFICIARIO='" & Trim(nombre_destintario) & "'"
                        Else
                            sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " ,BENEFICIARIO='" & Trim(nombre_destintario) & "'"
                        End If
                    Case "NITIDENTIFICACION"
                        If sql_actualiza_tramite_wf = "" Then
                            sql_actualiza_tramite_wf = sql_tabla_wf & " NITIDENTIFICACION='" & Trim(nit_identificacion) & "'"
                        Else
                            sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " ,NITIDENTIFICACION='" & Trim(nit_identificacion) & "'"
                        End If
                End Select
            Next

            If sql_actualiza_tramite_wf <> "" Then
                Dim existencia As String = "NO"
                Dim sqlconsulta_exitencia As String = "Select RADICADO,INICIO_TAREAS_WORKFLOW_ID_TAREA from dat_adic_tar" & nombre_ruta_wf & " where RADICADO='" & radicado & "'"
                Result = Refclas.Retorna_Existencia_flujo_workflow(sqlconsulta_exitencia, existencia, id_tarea_worklow)
                If Result <> "YES" Then
                    Reasigna_destinatario_externo = Result
                    Exit Function
                End If
                If existencia = "YES" Then
                    sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " where RADICADO='" & radicado & "'"
                Else
                    sql_actualiza_tramite_wf = ""
                End If

            End If
        End If
        Dim hor As String = Now
        Dim detalle_trans As String = "REASIGNACION"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "REASIGNA PETICIONARIO A LA RESPUESTA ( ID RESPUESTA : " & id_respuesta_radicado &
        " RADICADO : " & radicado & " SE CAMBIA AL PETICIONARIO  :  " & nombre_destintario &
         " ID USUARIO PETICIONARIO  : " & id_dext & " PERTENECIENTE LA PLANTILLA : " & nombre_plantilla_validacion & " USUARIO QUE AUTORIZO : " & usuario_admin & ")"
        isert_datos = isert_datos & "('" & detalle_trans & "','" & UCase(logi_user) & "','" & id_user & "','" & date1al & "'," &
                     id_respuesta_radicado & ",'" & iphost & "','" & hor & "','WORKFLOW-WEB','" & campos_trans & "')"

        Dim update_ra_log = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                             isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '-------------------------------------------------
            'Actualiza plantilla radicacion
            '-------------------------------------------------
            If update_plantilla_radicacion <> "" Then
                myCommand.CommandText = update_plantilla_radicacion
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Reasigna_destinatario_externo = "Imposible actualizar tabla de radicacion  "
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '------------------------------------------------
            'Actualiza RESPUESTA RADICADO
            '------------------------------------------------
            If update_respuesta <> "" Then
                myCommand.CommandText = update_respuesta
                sqlresultinsert = 0
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Reasigna_destinatario_externo = "Imposible actualizar tabla respuesta  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '--------------------------------------------
            'Actualiza tabla log de respuesta
            '--------------------------------------------
            myCommand.CommandText = update_ra_log
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Reasigna_destinatario_externo = "Imposible actualizar log respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            '-------------------------------------------------------------
            'Actualiza log radicado
            '-------------------------------------------------------------
            Dim update As String = "INSERT INTO ra_log_radicados (desc_op,USER_OPER,ID_USER,DATE_TRANS,CONSECUTIVO_RADICADO,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES (" &
                "'" & "ACTUALIZA PLANTILLA" & "','" & logi_user & "','" & id_user & "','" & date1al & "','" & radicado & "','" & cambios_campos &
                "','" & iphost & "','" & hor.ToString & "','" & "RADICACION-WEB" & "')"
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Reasigna_destinatario_externo = "Imposible actualizar log  " & update
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            If sql_actualiza_tramite_wf <> "" Then
                Result = Refclas.actualiza_campos_workflow(sql_actualiza_tramite_wf)
                If Result <> "YES" Then
                    Reasigna_destinatario_externo = "Se actualizaron los datos de la plantilla pero no se actualizo el indice workflow " & vbCrLf & Result
                    Exit Function
                End If
            End If
            Reasigna_destinatario_externo = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Reasigna_destinatario_externo = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Reasigna_destinatario_externo = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Actualiza_datos_interface_radicacion(ByVal nombre_plantilla As String,
                                                  ByVal numero_radicado As String,
                                                  ByVal PAGE As Page,
                                                  ByVal codigo_plantilla As Integer,
                                                  ByVal tipo_plantilla As String,
                                                  ByVal update_workflow As Integer,
                                                  ByRef Matri_Datos() As Campos_Plantilla,
                                                  ByRef resultado_correo As String) As String
        Dim DropDownList_area_destinatario_entrate As DropDownList = PAGE.FindControl("DropDownList_area_destinatario_entrate")
        Dim DropDownList_destinatario_entrante As DropDownList = PAGE.FindControl("DropDownList_destinatario_entrante")
        Dim TextBox_remitente_entrante As TextBox = PAGE.FindControl("TextBox_remitente_entrante")
        Dim TextBoxIdentificacion_remitente As TextBox = PAGE.FindControl("TextBoxIdentificacion_remitente")
        Dim TextBox_asunto_entrante As TextBox = PAGE.FindControl("TextBox_asunto_entrante")
        Dim TextBox_cita_radicado_entrante As TextBox = PAGE.FindControl("TextBox_cita_radicado_entrante")
        Dim TextBox_Numero_Folios_entrante As TextBox = PAGE.FindControl("TextBox_Numero_Folios_entrante")
        Dim TextBox_anexos_entrante As TextBox = PAGE.FindControl("TextBox_anexos_entrante")
        Dim TextBox_fecha_documento_entrante As TextBox = PAGE.FindControl("TextBox_fecha_documento_entrante")
        Dim cargo_destinatario As String = ""
        Dim Hidden_tipo_plantilla As Object = PAGE.FindControl("Hidden_tipo_plantilla")
        Dim Destinatario_Externo_id_Dest_Ext As Object = PAGE.FindControl("Hidden_remitente_destinario_interno")
        Dim Id_area_remit_dest_interno As Object = PAGE.FindControl("Hidden_area_remitente_destinatario")
        Dim Remit_Dest_Interno_id_Remit_Dest_Int As Object = PAGE.FindControl("Hidden_remitente_destinatario")
        Dim hdnEmailID_VAL_ref As Object = PAGE.FindControl("hdnEmailID_VAL")
        If hdnEmailID_VAL_ref.value = "-1" Then
            Actualiza_datos_interface_radicacion = "Debe seleccionar un registro para editar"
            Exit Function
        End If
        Dim refUpdatePnaelcontrolesradicacion_entrante = PAGE.FindControl("UpdatePnaelcontrolesradicacion_entrante")

        If DropDownList_area_destinatario_entrate.Text = "" Then
            Actualiza_datos_interface_radicacion = "Por favor seleccione el área del remitente interno"
            Exit Function
        End If
        If (DropDownList_area_destinatario_entrate.Text = "TODAS LAS AREAS") Then
            Actualiza_datos_interface_radicacion = "Por favor seleccione el área del  destintario interno, o asignela"
            Exit Function
        End If
        If (DropDownList_area_destinatario_entrate.Text = "SELECCIONE") Then
            Actualiza_datos_interface_radicacion = "Por favor seleccione el área del  destintario interno, o asignela"
            Exit Function
        End If

        If (DropDownList_destinatario_entrante.Text = "SELECCIONE") Then
            Actualiza_datos_interface_radicacion = "Por favor seleccione el  destintario interno"
            Exit Function
        End If
        If (TextBox_remitente_entrante.Text = "") Then
            Actualiza_datos_interface_radicacion = "Por favor seleccione el  remitente"
            Exit Function
        End If
        If (Destinatario_Externo_id_Dest_Ext.value = "-1") Then
            Actualiza_datos_interface_radicacion = "El destinatario interno esta en en estado (-1) no se puede actualizar"
            Exit Function
        End If
        If (Id_area_remit_dest_interno.value = "-1") Then
            Actualiza_datos_interface_radicacion = "El área del destinatario interno esta en en estado (-1) no se puede actualizar"
            Exit Function
        End If
        If (TextBox_Numero_Folios_entrante.Text = "") Then
            Actualiza_datos_interface_radicacion = "El numero de folios no puede ser nul"
            Exit Function
        End If
        If TextBox_anexos_entrante.Text = "" Then
            Actualiza_datos_interface_radicacion = "Debe digitar información de los anexos "
            Exit Function
        End If
        '--------------------------------------------------------
        'Determina los campos obligatorios del sistema
        '--------------------------------------------------------
        Dim Result As String = ""
        Dim Matri_campo() As Campos_Plantilla = Nothing
        Result = Me.Lista_Campos_Adicionales_Plantilla_del_sistema(codigo_plantilla,
                                                                   Matri_campo)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion = Result
            Exit Function
        End If
        Dim estado_cita_radicado As Integer = 0
        Dim estado_identificacion_remitente As Integer = 0
        If Not Matri_campo Is Nothing Then
            For z As Integer = 0 To Matri_campo.Length - 1
                If Matri_campo(z).Campo_Plantilla = "IDENTIFICACION_REMITENTE" Then
                    estado_identificacion_remitente = Val(Matri_campo(z).Campo_Obligatorio)
                End If
                If Matri_campo(z).Campo_Plantilla = "CITARADICADO" Then
                    estado_cita_radicado = Val(Matri_campo(z).Campo_Obligatorio)
                End If
            Next
        End If
        If HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") <> "PRODUCCION" Then
            If TextBoxIdentificacion_remitente.Text = "" And estado_identificacion_remitente = 1 Then
                Actualiza_datos_interface_radicacion = "Debe digitar la identificación del remitente "
                Exit Function
            End If
        End If

        If TextBox_cita_radicado_entrante.Text = "" And estado_cita_radicado = 1 Then
            Actualiza_datos_interface_radicacion = "Debe digitar el texto de cita radicado "
            Exit Function
        End If
        Dim update_string As String = ""
        Dim spliter() As String = DropDownList_destinatario_entrante.Text.Split("((")
        Dim spliteruno() As String = spliter(2).Split("))")
        cargo_destinatario = spliteruno(0)
        update_string = "update " + nombre_plantilla + " set Destinatario_Externo_id_Dest_Ext=" + "'" + Destinatario_Externo_id_Dest_Ext.value + "'"
        update_string = update_string + ",Id_area_remit_dest_interno=" + "'" + Id_area_remit_dest_interno.value + "'"
        update_string = update_string + ",Remit_Dest_Interno_id_Remit_Dest_Int=" + "'" + Remit_Dest_Interno_id_Remit_Dest_Int.value + "'"
        update_string = update_string + ",Area_remit_dest_interno=" + "'" + DropDownList_area_destinatario_entrate.Text & "'"
        update_string = update_string + ",Destinatario_Cor=" + "'" + Trim(spliter(0)) + "'"
        Dim iconsec As Integer = 0
        Erase Matri_Datos
        If (cargo_destinatario = "") Then
            update_string = update_string + ",cargo_destinatario=" + "null"
        Else
            update_string = update_string + ",cargo_destinatario=" + "'" + Trim(cargo_destinatario) & "'"
        End If
        If (TextBox_remitente_entrante.Text = "") Then
            update_string = update_string + ",Remitente_Cor=" + "null"
        Else
            update_string = update_string + ",Remitente_Cor=" + "'" + Trim(TextBox_remitente_entrante.Text) + "'"
        End If
        If (TextBoxIdentificacion_remitente.Text = "") Then
            update_string = update_string + ",IDENTIFICACION_REMITENTE=" + "null"
        Else
            update_string = update_string + ",IDENTIFICACION_REMITENTE=" + "'" + Trim(TextBoxIdentificacion_remitente.Text) + "'"
        End If

        If (TextBox_asunto_entrante.Text = "") Then
            update_string = update_string + ",Asunto=" + "null"
        Else
            update_string = update_string + ",Asunto=" + "'" + Trim(TextBox_asunto_entrante.Text) + "'"
        End If
        If (TextBox_cita_radicado_entrante.Text = "") Then
            update_string = update_string + ",CITARADICADO=" + "null"
        Else
            update_string = update_string + ",CITARADICADO=" + "'" + Trim(TextBox_cita_radicado_entrante.Text) + "'"
        End If
        If (TextBox_Numero_Folios_entrante.Text = "") Then
            Actualiza_datos_interface_radicacion = "Numero de folios no puede ser null"
            Exit Function
        Else
            update_string = update_string + ",Numero_Folios=" + "'" + Trim(TextBox_Numero_Folios_entrante.Text) + "'"
        End If
        If (TextBox_anexos_entrante.Text = "") Then
            update_string = update_string + ",Anexos_Cor=" + "null"
        Else
            update_string = update_string + ",Anexos_Cor=" + "'" + Trim(TextBox_anexos_entrante.Text) + "'"
        End If
        If (TextBox_fecha_documento_entrante.Text = "") Then
            update_string = update_string + ",Fecha_Documento=" + "null"
        Else
            update_string = update_string + ",Fecha_Documento=" + "'" + Trim(TextBox_fecha_documento_entrante.Text) + "'"
        End If
        '------------------------------------------------------
        'Asignar datos radicacion
        '------------------------------------------------------
        Dim refclasalmacen As New ClassRadicador
        Dim date1al As String = Date.Now
        Dim Refclas As New ClassListandoTareas
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion = "Imposible formatear fecha " & Result
            Exit Function
        End If
        '***************************************************************
        'Lista las opciones plantilla
        '***************************************************************
        Dim Estado_opcion_fecha As Integer = 0
        Dim Estado_opcion_cita_respuesta As Integer = 0
        Dim Estado_opcion_radicado_general As Integer = 0
        Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
        Result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(codigo_plantilla,
                                                                                        Estado_opcion_fecha,
                                                                                        Estado_opcion_cita_respuesta,
                                                                                        Estado_opcion_radicado_general)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion = Result
            Exit Function
        End If
        Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
        Result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(codigo_plantilla,
                                                                                            Matri_Datos,
                                                                                            Estado_opcion_fecha,
                                                                                            Estado_opcion_cita_respuesta,
                                                                                            Estado_opcion_radicado_general)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion = Result
            Exit Function
        End If
        '***************************************************************
        'Retorna datos validacion estructura
        '***************************************************************
        Result = refclasalmacen.retorna_datos_radicacion_estructura(tipo_plantilla,
                                                                    numero_radicado,
                                                                    nombre_plantilla,
                                                                    Matri_Datos)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion = Result
            Exit Function
        End If

        If Not Matri_Datos Is Nothing Then
            For i As Integer = 0 To Matri_Datos.Length - 1
                Select Case Matri_Datos(i).Campo_Plantilla
                    Case "NOMBREPETICIONA"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_remitente_entrante.Text)
                    Case "CARGO_DESTINATARIO"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(cargo_destinatario)
                    Case "Destinatario_Cor"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(spliter(0))
                    Case "REMITENTE_COR"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_remitente_entrante.Text)
                    Case "IDENTIFICACION_REMITENTE"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBoxIdentificacion_remitente.Text)
                    Case "ASUNTO"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_asunto_entrante.Text)
                    Case "CITARADICADO"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_cita_radicado_entrante.Text)
                    Case "Numero_Folios"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = TextBox_Numero_Folios_entrante.Text
                    Case "Anexos_Cor"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_anexos_entrante.Text)
                    Case "Fecha_Documento"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = TextBox_fecha_documento_entrante.Text
                    Case Else
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(Matri_Datos(i).TEXTO_CAMPO)
                End Select
            Next
        End If
        iconsec = Matri_Datos.Length
        ReDim Preserve Matri_Datos(iconsec)
        Matri_Datos(iconsec).TEXTO_CAMPO = DropDownList_area_destinatario_entrate.Text
        Matri_Datos(iconsec).TEXTO_CAMPO_MODIFICADO = DropDownList_area_destinatario_entrate.Text
        Matri_Datos(iconsec).Alias_Campo = "AREA_DESTINATARIO"
        '----------------------------------------------------------------------------
        'Asigna datos dinamicos
        '----------------------------------------------------------------------------
        For i As Integer = 0 To Matri_Datos.Length - 1
            Dim control As Object = PAGE.FindControl("RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit")
            If Not control Is Nothing Then
                If control.GetType.ToString = "System.Web.UI.WebControls.DropDownList" Then
                    Dim droplist As DropDownList = control
                    Matri_Datos(i).ID_CAMPO_ASPNET = "RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit"
                    Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = droplist.SelectedValue
                Else
                    Matri_Datos(i).ID_CAMPO_ASPNET = "RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit"
                    Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = control.text
                End If
            End If
        Next
        For i As Integer = 0 To Matri_Datos.Length - 1
            If Matri_Datos(i).Campo_Obligatorio = 1 And Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = "" Then
                Actualiza_datos_interface_radicacion = "El campo " & Matri_Datos(i).Alias_Campo & " es obligatorio"
                Exit Function
            End If
        Next
        '----------------------------------------------------------------------------
        'Verifica si se realizaron cambios en el indice
        '----------------------------------------------------------------------------
        Dim cambios_campos As String = ""
        Dim cambio_destinatario As String = ""
        Dim cambio_remitente As String = ""
        If Not Matri_Datos Is Nothing Then
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).TEXTO_CAMPO_MODIFICADO <> Matri_Datos(i).TEXTO_CAMPO Then
                    If Matri_Datos(i).Campo_Plantilla = "Destinatario_Cor" Then
                        cambio_destinatario = "YES"
                    End If
                    If Matri_Datos(i).Campo_Plantilla = "REMITENTE_COR" Then
                        cambio_remitente = "YES"
                    End If
                    If Matri_Datos(i).estado_dinamico_estatico = 2 Then
                        If Matri_Datos(i).TEXTO_CAMPO <> Matri_Datos(i).TEXTO_CAMPO_MODIFICADO Then
                            update_string = update_string + "," + Matri_Datos(i).Campo_Plantilla + "=" + "'" + Trim(Matri_Datos(i).TEXTO_CAMPO_MODIFICADO) + "'"
                        End If
                    End If
                    If update_string = "" Then
                        If Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = "" Then
                            cambios_campos = cambios_campos & " " & Matri_Datos(i).Campo_Plantilla & " Valor anterior " & Matri_Datos(i).TEXTO_CAMPO & " Nuevo valor " & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & " "
                        Else
                            cambios_campos = cambios_campos & " " & Matri_Datos(i).Campo_Plantilla & " Valor anterior " & Matri_Datos(i).TEXTO_CAMPO & " Nuevo valor " & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & " "
                        End If
                    Else
                        If Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = "" Then
                            cambios_campos = cambios_campos & " " & Matri_Datos(i).Campo_Plantilla & " Valor anterior " & Matri_Datos(i).TEXTO_CAMPO & " Nuevo valor " & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & " "
                        Else
                            cambios_campos = cambios_campos & " " & Matri_Datos(i).Campo_Plantilla & " Valor anterior " & Matri_Datos(i).TEXTO_CAMPO & " Nuevo valor " & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & " "
                        End If
                    End If
                End If
            Next
        End If
        If cambios_campos = "" Then
            Actualiza_datos_interface_radicacion = "No se detectaron cambios en los campos para actualizar"
            Exit Function
        End If
        If cambio_remitente <> "" Then
            If HttpContext.Current.Session.Item("RA_TIPO_CONSULTA_RADICADO") = "PRODUCCION" Then
                Actualiza_datos_interface_radicacion = "No se puede cambiar el remitente del tramite en modo consulta de producción documental"
                Exit Function
            End If
        End If
        update_string = update_string + " where Consecutivo_Rad=" + "'" + numero_radicado + "'"
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim id_user_wf As Integer = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
        Dim id_ruta_wf As Integer = HttpContext.Current.Session.Item("Id_Ruta_Workflow")
        Dim nombre_ruta_wf As String = ""
        Dim sql_tabla_wf As String = ""
        Dim sql_actualiza_tramite_wf As String = ""

        '---------------------------------------------------------------------------
        'Retorna relacion usuario gestion workflow
        '---------------------------------------------------------------------------
        Result = refclasalmacen.Retorna_relacion_usuario_gestion_workflow(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                          id_user_wf)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion = Result
            Exit Function
        End If

        If id_user_wf <> 0 Then
            '-----------------------------------------------------------------------
            'Retorna id ruta y nombre usuario
            '-----------------------------------------------------------------------
            Result = Refclas.Retorna_id_ruta_usuario_workflow(id_user_wf,
                                                              id_ruta_wf,
                                                              nombre_ruta_wf)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
        End If
        Dim nombre_campo_radicado_ruta As String = ""
        Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
        Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(id_ruta_wf,
                                                                          nombre_campo_radicado_ruta)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion = Result
            Exit Function
        End If
        '---------------------------------------------------------------------------
        'Verfica  existencia_campos_workflow
        '---------------------------------------------------------------------------
        Dim existencia_campos_workflow As String = "NO"
        Dim matri_campos() As String
        Dim stru_campos_plantilla_ruta() As csfc_structure_relacion_campos_plantilla_ruta = Nothing
        Dim Class_ra_relacion_ruta_plantilla As New Class_ra_relacion_ruta_plantilla
        Dim Class_ra_campos_relacionados_ruta_plantilla As New Class_ra_campos_relacionados_ruta_plantilla
        Erase matri_campos
        If id_ruta_wf <> 0 Then
            Result = Refclas.Verifica_existencia_campos_listado_ruta(id_ruta_wf,
                                                                     existencia_campos_workflow,
                                                                     matri_campos)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(id_ruta_wf.ToString,
                                                                     nombre_ruta_wf)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            Dim id_relacion_ruta_plantilla As Integer = 0
            Result = Class_ra_relacion_ruta_plantilla.Solicita_relacion_ruta_plantilla(codigo_plantilla,
                                                                                       id_ruta_wf,
                                                                                       id_relacion_ruta_plantilla)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            Result = Class_ra_campos_relacionados_ruta_plantilla.solicita_relacion_campos_ruta_plantilla(id_relacion_ruta_plantilla,
                                                                                                         stru_campos_plantilla_ruta)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
        End If
        '****************************************************
        'Actualiza campos workflow tabla dat_adic
        '****************************************************
        Dim id_tarea_worklow As Integer = 0
        If existencia_campos_workflow = "YES" Then
            sql_tabla_wf = "Update dat_adic_tar" & nombre_ruta_wf & " set "
            For i As Integer = 0 To stru_campos_plantilla_ruta.Length - 1
                For z As Integer = 0 To Matri_Datos.Length - 1
                    If UCase(Matri_Datos(z).Campo_Plantilla) = UCase(stru_campos_plantilla_ruta(i).nombre_campo_plantilla) Then
                        If sql_actualiza_tramite_wf = "" Then
                            sql_actualiza_tramite_wf = sql_tabla_wf & stru_campos_plantilla_ruta(i).nombre_campo_ruta & " ='" & Trim(Matri_Datos(z).TEXTO_CAMPO_MODIFICADO) & "'"
                        Else
                            sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " ," & stru_campos_plantilla_ruta(i).nombre_campo_ruta & " ='" & Trim(Matri_Datos(z).TEXTO_CAMPO_MODIFICADO) & "'"
                        End If
                    End If
                Next
            Next

            If sql_actualiza_tramite_wf <> "" Then
                Dim existencia As String = "NO"
                Dim sqlconsulta_exitencia As String = "Select " & nombre_campo_radicado_ruta & ",INICIO_TAREAS_WORKFLOW_ID_TAREA from dat_adic_tar" & nombre_ruta_wf & " where " & nombre_campo_radicado_ruta & "='" & numero_radicado & "'"
                Result = Refclas.Retorna_Existencia_flujo_workflow(sqlconsulta_exitencia,
                                                                   existencia,
                                                                   id_tarea_worklow)
                If Result <> "YES" Then
                    Actualiza_datos_interface_radicacion = Result
                    Exit Function
                End If
                If existencia = "YES" Then
                    sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " where " & nombre_campo_radicado_ruta & "='" & numero_radicado & "'"
                Else
                    sql_actualiza_tramite_wf = ""
                End If

            End If
        End If
        '*****************************************************
        'Verifica si hay una respuesta sobre el radicado
        '*****************************************************
        Dim sql_actualiza_radicado_asignado_destintario As String = ""
        Dim id_respuesta As Integer = 0
        Dim exitencia As String = "NO"
        Dim estado_radicado As Integer = 0
        Result = Verifica_existencia_radicado_asignado(numero_radicado,
                                                       exitencia,
                                                       id_respuesta,
                                                       estado_radicado)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion = Result
            Exit Function
        End If
        If estado_radicado <> 0 And cambio_destinatario = "YES" Then
            Actualiza_datos_interface_radicacion = "El sistema registra una respuesta para el radicado, imposible actualizar datos"
            Exit Function
        End If
        If exitencia = "YES" Then
            sql_actualiza_radicado_asignado_destintario = "update ra_respuesta_radicado"
            If cambio_destinatario = "YES" Then
                If sql_actualiza_radicado_asignado_destintario = "update ra_respuesta_radicado" Then
                    sql_actualiza_radicado_asignado_destintario = sql_actualiza_radicado_asignado_destintario &
                    " set ID_REMIT_DEST_INT=" & Destinatario_Externo_id_Dest_Ext.value & ",ID_AREA=" & Id_area_remit_dest_interno.value &
                    ",AREA_RESPONSABLE='" & DropDownList_area_destinatario_entrate.Text & "',CARGO_RESPONSABLE='" & Trim(cargo_destinatario) & "'," &
                    "USUARIO_RESPONSABLE='" & Trim(spliter(0)) & "'"
                Else
                    sql_actualiza_radicado_asignado_destintario = sql_actualiza_radicado_asignado_destintario &
                   ",ID_REMIT_DEST_INT=" & Destinatario_Externo_id_Dest_Ext.value & ",ID_AREA=" & Id_area_remit_dest_interno.value &
                   ",AREA_RESPONSABLE='" & DropDownList_area_destinatario_entrate.Text & "',CARGO_RESPONSABLE='" & Trim(cargo_destinatario) & "'," &
                   "USUARIO_RESPONSABLE='" & Trim(spliter(0)) & "'"
                End If

            End If
            If cambio_remitente = "YES" Then
                If sql_actualiza_radicado_asignado_destintario = "update ra_respuesta_radicado" Then
                    sql_actualiza_radicado_asignado_destintario = sql_actualiza_radicado_asignado_destintario &
                    " set DESTINATARIO='" & Trim(TextBox_remitente_entrante.Text) & "',codigo_dest_externo=" & Remit_Dest_Interno_id_Remit_Dest_Int.value

                Else
                    sql_actualiza_radicado_asignado_destintario = sql_actualiza_radicado_asignado_destintario &
                   ",DESTINATARIO='" & Trim(TextBox_remitente_entrante.Text) & "',codigo_dest_externo=" & Remit_Dest_Interno_id_Remit_Dest_Int.value
                End If
            End If
            If sql_actualiza_radicado_asignado_destintario <> "update ra_respuesta_radicado" Then
                sql_actualiza_radicado_asignado_destintario = sql_actualiza_radicado_asignado_destintario & " Where RADICADO='" & numero_radicado & "'"
            Else
                sql_actualiza_radicado_asignado_destintario = ""
            End If
        End If
        '------------------------------------------------------------------
        'Verifica existencia estado modulo radicado
        '------------------------------------------------------------------
        'Dim Ref_class_estado_modulo_radicado As New Class_estados_modulo_radicacion
        Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
        Dim id_estado_modulo_radicado As Integer = 0
        Dim estado_modulo_radicado As Integer = 0
        Dim sql_actualiza_estado_radicado As String = ""
        Result = Class_ra_rad_estados_modulo_radicacion.Solicita_id_estado_modulo_radicado(numero_radicado,
                                                                                           0,
                                                                                           id_estado_modulo_radicado,
                                                                                           estado_modulo_radicado)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion = Result
            Exit Function
        End If
        If id_estado_modulo_radicado <> 0 Then
            sql_actualiza_estado_radicado = "update ra_rad_estados_modulo_radicacion set remitente=" &
                "'" & Trim(TextBox_remitente_entrante.Text) & "' where id_estado_radicado=" & id_estado_modulo_radicado
        End If
        '*****************************************************************
        'Verifica la existencia de la asignacion de la tarea en workflow
        '*****************************************************************
        Dim Refclasgestion As New Classgestionrespuesta
        Dim nombre_plantilla_ As String = ""
        Dim id_radicado As Integer = 0
        Dim id_usuario_gestion_salida As Integer = 0
        Dim id_uusario_workflow_salida As Integer = 0
        Dim id_grupo_workflow As Integer = 0
        Dim id_activdad_salida As Integer = 0
        Dim nombre_actividad_salida As String = ""
        Dim nombre_usuario_workflow As String = ""
        Dim Refclas_wf As New ClassWorkflow
        Dim sql_reasigna_document_workflow As String = ""
        Dim id_flujo_trabajo As Integer = 0
        Dim id_registro_actvidad_flujo As Integer = 0
        Dim id_usuario_flujo As Integer = 0
        Dim estado_recuperacion_flujo_trabajo As Integer = 0
        '------------------------------------------------------
        'Variables de flujo de treabajo del nuevo destinatario
        '------------------------------------------------------
        Dim id_actividad_flujo_trabajo As Integer = 0
        Dim id_usuario_flujo_trabajo As Integer = 0
        Dim id_usuario_workflow As Integer = 0
        Dim id_registro_actvidad_flujo_trabajo As Integer = 0
        Dim id_actividad As Integer = 0
        Dim correo_electronico As String = ""
        Dim id_area_transaccion As Integer = 0
        Dim nombre_area_transaccion As String = ""
        Dim nombre_usuario_gestion_transaccion As String = ""
        Dim cargo_usuario_gestion_transaccion As String = ""
        Dim tramite As String = ""
        Dim Fecha_vence As String = ""
        Dim fecha_registro As String = ""
        Dim destinatario As String = ""
        Dim asunto As String = ""
        Dim split_notificacion() As String
        Dim asunto_final As String = ""
        Dim id_imagen As Object = 0
        If cambio_destinatario = "YES" And update_workflow = 1 Then
            Result = Refclasgestion.Retorna_datos_general_radicado(numero_radicado,
                                                                   nombre_plantilla,
                                                                   id_radicado)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            '***************************************************
            'Solicita el usuario de gestion desde la plantilla
            'del radicado
            '***************************************************
            Result = Refclasgestion.Retorna_id_usuario_gestion_plantilla_radicado(nombre_plantilla,
                                                                                  id_radicado,
                                                                                  id_usuario_gestion_salida)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Result = Class_usuario_workflow.Retorna_id_usuario_workflow_usuario_gestion(id_usuario_gestion_salida,
                                                                                        id_uusario_workflow_salida,
                                                                                        id_grupo_workflow,
                                                                                        nombre_usuario_workflow)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            Dim Ref_class_listado As New Class_Listado_Actividades_workflow
            Result = Ref_class_listado.Retorna_actividad_grupo_workflow(id_grupo_workflow,
                                                                        id_activdad_salida,
                                                                        nombre_actividad_salida)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            Dim estado_asignacion As String = "YES"
            Dim loguin_usuario As String = ""
            Dim nombre_actividad_salida_ As String = ""
            Dim nombre_usuario_workflow_ As String = ""
            Dim loguin_usuario_ As String = ""
            Dim cargo As String = ""
            Result = Refclas_wf.verifica_tarea_seleccionada_workflow(id_uusario_workflow_salida,
                                                                     id_tarea_worklow,
                                                                     estado_asignacion,
                                                                     nombre_actividad_salida_,
                                                                     nombre_usuario_workflow_,
                                                                     loguin_usuario_,
                                                                     cargo)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            If estado_asignacion = "YES" Then
                Actualiza_datos_interface_radicacion = "El sistema detecto asignación en workflow al usuario " &
                    nombre_usuario_workflow_ & " de la actividad : " & nombre_actividad_salida_ & " Cargo : " & cargo &
                    " Loguin : " & loguin_usuario_ & ". Imposible actualizar el destinatario mientras este asignado"
                Exit Function
            End If
            '----------------------------------------------
            'Verifica que la tarea tenga imagen asignada
            '----------------------------------------------
            Dim existencia_flujo As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = Class_DAT_ADIC_TAR.Solicita_imagen_id_tarea_relacionada_flujo_workflow(existencia_flujo,
                                                                                            nombre_ruta_wf,
                                                                                            nombre_campo_radicado_ruta,
                                                                                            numero_radicado,
                                                                                            id_imagen,
                                                                                            id_tarea_worklow)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Solicita correo usuario de gestión que se releva
            'de la respuesta
            '-------------------------------------------------
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(id_usuario_gestion_salida,
                                                                              correo_electronico)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            '------------------------------------------------
            'Solicita datos de caraterización envío correo 
            'electronico relevo
            '------------------------------------------------
            Dim reflcas_respuesta As New Classgestionrespuesta
            Result = reflcas_respuesta.Retorna_datos_asignacion_respuesta_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                          id_area_transaccion,
                                                                                          nombre_area_transaccion,
                                                                                          nombre_usuario_gestion_transaccion,
                                                                                          cargo_usuario_gestion_transaccion)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If

            Dim stru As PLANTILLA_VALIDACION_CAMPOS_ESTATICOS
            Dim ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = ref_Class_plantillas_radicacion.retorna_datos_radicacion_estructura(tipo_plantilla,
                                                                                         numero_radicado,
                                                                                         nombre_plantilla,
                                                                                         stru)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            asunto = tramite & " Fecha vencimiento  " & stru.FECHALIMITERESPUESTA
            asunto_final = "Relevo respuesta tramite, " & stru.Asunto
            split_notificacion = {"Usted fue relevado para dar respuesta al radicado : " & numero_radicado & " Tipo tramite " & stru.Descripcion_Documento, "Fecha de radicación : " & stru.Fecha_Radicado _
                    , "Fecha límite de respuesta : " & stru.FECHALIMITERESPUESTA, "Remite : " & stru.Remitente_Cor, "Asunto : " & stru.Asunto, "Radicado : " & numero_radicado,
                    "Por el usuario " & nombre_usuario_gestion_transaccion & " del cargo " & cargo_usuario_gestion_transaccion & " en la fecha " & date1al}

            '-----------------------------------------------
            'Reasigna en el flujo de trabajo el documento
            '-----------------------------------------------
            Dim Refclas_flujo_trabajo_workflow As New Class_flujo_trabajo_workflow
            '----------------------------------------------------
            'Solicita datos de flujo documental de la tarea
            '----------------------------------------------------
            Result = Refclas_flujo_trabajo_workflow.solicita_datos_tarea_flujo_trabajo_workflow(id_uusario_workflow_salida,
                                                                                                id_tarea_worklow,
                                                                                                id_flujo_trabajo,
                                                                                                id_registro_actvidad_flujo,
                                                                                                id_usuario_flujo,
                                                                                                estado_recuperacion_flujo_trabajo)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna id actividad de id usuario workflow
            '---------------------------------------------------
            Dim split_dest() As String = DropDownList_destinatario_entrante.Text.ToString.Split("((")
            Dim split_cargo() As String = Nothing
            If split_dest.Length > 0 Then
                split_cargo = split_dest(2).ToString.Split("))")
            End If
            Dim ref_Destinatario_Cor As String = split_dest(0)
            Dim ref_cargo_festinatario As String = split_cargo(0)
            Dim ref_class_radicado As New ClassRadicador
            Dim nombre_flujo_trabajo As String = ""
            Result = ref_class_radicado.Retorna_id_actividad_workflow_id_usuario_workflow_por_destinatario(ref_Destinatario_Cor, id_actividad, id_usuario_workflow)
            If Result <> "YES" Then
                Actualiza_datos_interface_radicacion = Result
                Exit Function
            End If
            cambios_campos = cambios_campos & " ACTUALIZA ESTADO DE LA TAREA WORKFLOW : " & id_tarea_worklow & " Cargo usuario asignado : " &
               ref_cargo_festinatario & " Nombre usuario asignado : " & ref_Destinatario_Cor & " Código usuario asignado : " & id_usuario_workflow &
               " Código actividad asignada : " & id_actividad
            '------------------------------------------------------------------
            'Verifica los datos de flujo documental del del nuevo destinatario
            '------------------------------------------------------------------
            If id_flujo_trabajo <> 0 Then
                '---------------------------------------------------------
                'Solicita la actvidad de flujo documental del usuario 
                'workflow seleccionado
                '---------------------------------------------------------
                Result = Refclas_flujo_trabajo_workflow.Solicita_id_registro_actividad_flujo_trabjo_por_usuario_workflow(id_usuario_workflow,
                                                                                                                        id_flujo_trabajo,
                                                                                                                        id_registro_actvidad_flujo_trabajo,
                                                                                                                        id_actividad_flujo_trabajo,
                                                                                                                        id_usuario_flujo_trabajo)
                If Result <> "YES" Then
                    Actualiza_datos_interface_radicacion = Result
                    Exit Function
                End If
                '---------------------------------------------------------
                'Solicita la actvidad de flujo documental de la
                'actividad relacionada al usuario workflow
                '---------------------------------------------------------
                If id_registro_actvidad_flujo_trabajo = 0 Then
                    Result = Refclas_flujo_trabajo_workflow.Solicita_id_registro_actividad_flujo_trabajo_por_actividad_del_usuario_workflow(id_usuario_workflow,
                                                                                                                                            id_flujo_trabajo,
                                                                                                                                            id_registro_actvidad_flujo_trabajo,
                                                                                                                                            id_actividad_flujo_trabajo)
                    If Result <> "YES" Then
                        Actualiza_datos_interface_radicacion = Result
                        Exit Function
                    End If
                End If
                Result = Refclas_flujo_trabajo_workflow.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo, nombre_flujo_trabajo)
                If Result <> "YES" Then
                    Actualiza_datos_interface_radicacion = Result
                    Exit Function
                End If
                If id_registro_actvidad_flujo_trabajo = 0 Then
                    Actualiza_datos_interface_radicacion = "El destinatario del trámite (" & ref_Destinatario_Cor & ") no pertenece al flujo de trabajo " & nombre_flujo_trabajo &
                    " relacionado al trámite (" & tramite & "), imposible cambiar destinatario. "
                    Exit Function
                End If
            End If

        End If

        Dim hor As String = Now
        Dim detalle_trans As String = "REASIGNACION"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "REASIGNA RESPUESTA NUMERO (" &
        " RADICADO " & numero_radicado & " AL USUARIO DE GESTION  :  " & spliter(0) &
        " cargo " & cargo_destinatario & " ID USUARIO DE GESTION : " & Destinatario_Externo_id_Dest_Ext.value & ")"
        isert_datos = isert_datos & "('" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date1al & "'," &
                     id_respuesta & ",'" & iphost & "','" & hor & "','RADICACION-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                             isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            If update_string <> "" Then
                myCommand.CommandText = update_string
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_datos_interface_radicacion = "Imposible actualizar campos del sistema  " & update_string
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '------------------------------------------------
            'Actualiza tabla respuesta  obligatorio
            '------------------------------------------------
            If sql_actualiza_radicado_asignado_destintario <> "" Then
                myCommand.CommandText = sql_actualiza_radicado_asignado_destintario
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_datos_interface_radicacion = "Imposible actualizar tabla respuesta  " & sql_actualiza_radicado_asignado_destintario
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                '--------------------------------------------
                'Actualiza tabla log de respuesta
                '--------------------------------------------
                If sqlresultinsert <> 0 Then
                    myCommand.CommandText = update_gestion
                    sqlresultinsert = myCommand.ExecuteNonQuery()
                    If sqlresultinsert = 0 Then
                        Actualiza_datos_interface_radicacion = "Imposible actualizar log respuesta  " & update_gestion
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
            End If
            '-------------------------------------------------------------
            'Actualiza log radicado
            '-------------------------------------------------------------
            Dim update As String = "INSERT INTO ra_log_radicados (desc_op,USER_OPER,ID_USER,DATE_TRANS,CONSECUTIVO_RADICADO,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES (" &
                "'" & "ACTUALIZA PLANTILLA" & "','" & logi_user & "','" & id_user & "','" & date1al & "','" & numero_radicado & "','" & cambios_campos &
                "','" & iphost & "','" & hor.ToString & "','" & "RADICACION-WEB" & "')"
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_datos_interface_radicacion = "Imposible actualizar log  " & update
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------------------------------------
            'Actuliza estado modulo radicado
            '----------------------------------------------------------------
            If id_estado_modulo_radicado <> 0 Then
                myCommand.CommandText = sql_actualiza_estado_radicado
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_datos_interface_radicacion = "Imposible actualizar estado modulo radicado  " & update
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            If cambio_destinatario <> "" And id_imagen <> 0 And estado_modulo_radicado <> 1 Then
                Dim ref_class_seleccion As New Classselecciotarea
                Result = ref_class_seleccion.Actualizando_Estado_Tarea_Acrualiza_usuario_tarea_workflow(id_usuario_workflow,
                                                                                                        id_tarea_worklow,
                                                                                                        id_actividad,
                                                                                                        id_activdad_salida,
                                                                                                        id_flujo_trabajo,
                                                                                                        id_registro_actvidad_flujo_trabajo,
                                                                                                        id_usuario_flujo_trabajo,
                                                                                                        estado_recuperacion_flujo_trabajo)
                If Result <> "YES" Then
                    Actualiza_datos_interface_radicacion = "Se actualizaron los datos de la plantilla pero no se reasigno el usuario workflow " & Result
                    Exit Function
                End If
                Dim Refclas_gaproducion As New ClassGaProducionDocumental
                Result = Refclas_gaproducion.Envia_corre_notificacion_radicado_envio_documento(numero_radicado,
                                                                                               "RADICACION ENTRANTE",
                                                                                               nombre_plantilla, "")
                If Result <> "YES" Then
                    resultado_correo = "Se actualizo el trámite, pero no se pudo notificar al destinatario por correo electrónico debido a este error " & Result
                End If
                Dim refclascorreo As New ClassCorreo
                If resultado_correo = "" Then
                    Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion,
                                                                                correo_electronico,
                                                                                asunto_final)
                    If Result <> "YES" Then
                        resultado_correo = resultado_correo & Result
                    End If
                End If

            End If
            If sql_actualiza_tramite_wf <> "" Then
                Result = Refclas.actualiza_campos_workflow(sql_actualiza_tramite_wf)
                If Result <> "YES" Then
                    Actualiza_datos_interface_radicacion = "Se actualizaron los datos de la plantilla pero no se actualizo el indice workflow " & Result
                    Exit Function
                End If

            End If
            Actualiza_datos_interface_radicacion = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_datos_interface_radicacion = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_datos_interface_radicacion = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Actualiza_datos_interface_radicacion_saliente(ByVal nombre_plantilla As String,
                                                           ByVal numero_radicado As String,
                                                           ByVal PAGE As Page,
                                                           ByVal codigo_plantilla As Integer,
                                                           ByVal tipo_plantilla As String,
                                                           ByRef Matri_Datos() As Campos_Plantilla) As String

        Dim DropDownList_area_remitente_saliente As DropDownList = PAGE.FindControl("DropDownList_area_remitente_saliente")
        Dim DropDownList_remitente_saliente As DropDownList = PAGE.FindControl("DropDownList_remitente_saliente")
        Dim TextBox_remitente_saliente As TextBox = PAGE.FindControl("TextBox_remitente_saliente")
        Dim TextBox_identificacion_destinatario As TextBox = PAGE.FindControl("TextBox_identificacion_destinatario")
        Dim TextBox_asunto_saliente As TextBox = PAGE.FindControl("TextBox_asunto_saliente")
        Dim TextBox_cita_radicado_saliente As TextBox = PAGE.FindControl("TextBox_cita_radicado_saliente")
        Dim TextBox_Numero_Folios_saliente As TextBox = PAGE.FindControl("TextBox_Numero_Folios_saliente")
        Dim TextBox_anexos_saliente As TextBox = PAGE.FindControl("TextBox_anexos_saliente")
        Dim TextBox_fecha_documento_saliente As TextBox = PAGE.FindControl("TextBox_fecha_documento_saliente")
        Dim cargo_remitente As String = ""
        Dim Hidden_tipo_plantilla As Object = PAGE.FindControl("Hidden_tipo_plantilla")
        Dim Destinatario_Externo_id_Dest_Ext As Object = PAGE.FindControl("Hidden_remitente_destinario_interno")
        Dim Id_area_remit_dest_interno As Object = PAGE.FindControl("Hidden_area_remitente_destinatario")
        Dim Remit_Dest_Interno_id_Remit_Dest_Int As Object = PAGE.FindControl("Hidden_remitente_destinatario")
        Dim hdnEmailID_VAL_ref As Object = PAGE.FindControl("hdnEmailID_VAL")

        If hdnEmailID_VAL_ref.value = "-1" Then
            Actualiza_datos_interface_radicacion_saliente = "Debe seleccionar un registro para editar"
            Exit Function
        End If
        Dim refUpdatePnaelcontrolesradicacion_entrante = PAGE.FindControl("UpdatePnaelcontrolesradicacion_entrante")
        Dim rejava As New Classscrripjava
        If DropDownList_area_remitente_saliente.Text = "" Then
            Actualiza_datos_interface_radicacion_saliente = "Por favor seleccione el área del remitente interno"
            Exit Function
        End If
        If (DropDownList_area_remitente_saliente.Text = "TODAS LAS AREAS") Then
            Actualiza_datos_interface_radicacion_saliente = "Por favor seleccione el área del  remitente interno, o asignela"
            Exit Function
        End If
        If (DropDownList_area_remitente_saliente.Text = "SELECCIONE") Then
            Actualiza_datos_interface_radicacion_saliente = "Por favor seleccione el área del  remitente interno, o asignela"
            Exit Function
        End If

        If (DropDownList_remitente_saliente.Text = "SELECCIONE") Then
            Actualiza_datos_interface_radicacion_saliente = "Por favor seleccione el  remitente interno"
            Exit Function
        End If
        If (TextBox_remitente_saliente.Text = "") Then
            Actualiza_datos_interface_radicacion_saliente = "Por favor seleccione el  remitente interno"
            Exit Function
        End If
        If (Destinatario_Externo_id_Dest_Ext.value = "-1") Then
            Actualiza_datos_interface_radicacion_saliente = "El destinatario interno esta en en estado (-1) no se puede actualizar"
            Exit Function
        End If
        If (Id_area_remit_dest_interno.value = "-1") Then
            Actualiza_datos_interface_radicacion_saliente = "El area del destinatario interno esta en en estado (-1) no se puede actualizar"
            Exit Function
        End If
        If (TextBox_Numero_Folios_saliente.Text = "") Then
            Actualiza_datos_interface_radicacion_saliente = "El numero de folios no puede ser nul"
            Exit Function
        End If
        If TextBox_anexos_saliente.Text = "" Then
            Actualiza_datos_interface_radicacion_saliente = "Debe digitar información de los anexos "
            Exit Function
        End If
        Dim update_string As String = ""
        Dim spliter() As String = DropDownList_remitente_saliente.Text.Split("((")
        Dim spliteruno() As String = spliter(2).Split("))")
        cargo_remitente = spliteruno(0)
        update_string = "update " + nombre_plantilla + " set Destinatario_Externo_id_Dest_Ext=" + "'" + Destinatario_Externo_id_Dest_Ext.value + "'"
        update_string = update_string + ",Id_area_remit_dest_interno=" + "'" + Id_area_remit_dest_interno.value + "'"
        update_string = update_string + ",Remit_Dest_Interno_id_Remit_Dest_Int=" + "'" + Remit_Dest_Interno_id_Remit_Dest_Int.value + "'"
        update_string = update_string + ",Area_remit_dest_interno=" + "'" + DropDownList_area_remitente_saliente.Text & "'"
        update_string = update_string + ",Remitente_Cor=" + "'" + Trim(spliter(0)) + "'"
        If (cargo_remitente = "") Then
            update_string = update_string + ",cargo_remitente=" + "null"
        Else
            update_string = update_string + ",cargo_remitente=" + "'" + Trim(cargo_remitente) & "'"
        End If

        If (TextBox_remitente_saliente.Text = "") Then
            update_string = update_string + ",Destinatario_Cor=" + "null"
        Else
            update_string = update_string + ",Destinatario_Cor=" + "'" + Trim(TextBox_remitente_saliente.Text) + "'"
        End If


        If (TextBox_identificacion_destinatario.Text = "") Then
            update_string = update_string + ",IDENTIFICACION_DESTINATARIO=" + "null"
        Else
            update_string = update_string + ",IDENTIFICACION_DESTINATARIO=" + "'" + Trim(TextBox_identificacion_destinatario.Text) + "'"
        End If

        If (TextBox_asunto_saliente.Text = "") Then
            update_string = update_string + ",Asunto=" + "null"
        Else
            update_string = update_string + ",Asunto=" + "'" + Trim(TextBox_asunto_saliente.Text) + "'"
        End If

        If (TextBox_cita_radicado_saliente.Text = "") Then
            update_string = update_string + ",CITARADICADO=" + "null"
        Else
            update_string = update_string + ",CITARADICADO=" + "'" + Trim(TextBox_cita_radicado_saliente.Text) + "'"
        End If

        If (TextBox_Numero_Folios_saliente.Text = "") Then
            'update_string = update_string + ",Numero_Folios=" + "null"
            Actualiza_datos_interface_radicacion_saliente = "Numero de folios no pude se null"
            Exit Function
        Else
            update_string = update_string + ",Numero_Folios=" + "'" + Trim(TextBox_Numero_Folios_saliente.Text) + "'"
        End If

        If (TextBox_anexos_saliente.Text = "") Then
            update_string = update_string + ",Anexos_Cor=" + "null"
        Else
            update_string = update_string + ",Anexos_Cor=" + "'" + Trim(TextBox_anexos_saliente.Text) + "'"
        End If

        If (TextBox_fecha_documento_saliente.Text = "") Then
            update_string = update_string + ",Fecha_Documento=" + "null"
        Else
            update_string = update_string + ",Fecha_Documento=" + "'" + Trim(TextBox_fecha_documento_saliente.Text) + "'"
        End If
        '------------------------------------------------------
        'Asignar datos radicacion
        '------------------------------------------------------
        Dim refclasalmacen As New ClassRadicador
        Dim date1al As String = Date.Now
        Dim Result As String = ""
        Dim Refclas As New ClassListandoTareas
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion_saliente = "Imposible formatear fecha " & Result
            Exit Function
        End If
        '***************************************************************
        'Lista las opciones plantilla
        '***************************************************************
        Dim Estado_opcion_fecha As Integer = 0
        Dim Estado_opcion_cita_respuesta As Integer = 0
        Dim Estado_opcion_radicado_general As Integer = 0
        Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
        Result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(codigo_plantilla,
                                                                                       Estado_opcion_fecha,
                                                                                       Estado_opcion_cita_respuesta,
                                                                                       Estado_opcion_radicado_general)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion_saliente = Result
            Exit Function
        End If
        Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
        Erase Matri_Datos
        Result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(codigo_plantilla,
                                                                                            Matri_Datos,
                                                                                            Estado_opcion_fecha,
                                                                                            Estado_opcion_cita_respuesta,
                                                                                            Estado_opcion_radicado_general)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion_saliente = Result
            Exit Function
        End If
        '***************************************************************
        'Retorna datos validacion estructura
        '***************************************************************
        Result = refclasalmacen.retorna_datos_radicacion_estructura(tipo_plantilla,
                                                                    numero_radicado,
                                                                    nombre_plantilla,
                                                                    Matri_Datos)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion_saliente = Result
            Exit Function
        End If
        'update_string = ""
        If Not Matri_Datos Is Nothing Then
            For i As Integer = 0 To Matri_Datos.Length - 1
                Select Case Matri_Datos(i).Campo_Plantilla
                    Case "CARGO_REMITENTE"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(cargo_remitente)
                    Case "REMITENTE_COR"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(spliter(0))
                    Case "Destinatario_Cor"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_remitente_saliente.Text)
                    Case "IDENTIFICACION_REMITENTE"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_identificacion_destinatario.Text)
                    Case "ASUNTO"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_asunto_saliente.Text)
                    Case "CITARADICADO"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_cita_radicado_saliente.Text)
                    Case "Numero_Folios"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = TextBox_Numero_Folios_saliente.Text
                    Case "Anexos_Cor"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(TextBox_anexos_saliente.Text)
                    Case "Fecha_Documento"
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = TextBox_fecha_documento_saliente.Text
                    Case Else
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Trim(Matri_Datos(i).TEXTO_CAMPO)
                End Select
            Next
        End If
        Dim iconsec As Integer = 0
        iconsec = Matri_Datos.Length
        ReDim Preserve Matri_Datos(iconsec)
        Matri_Datos(iconsec).TEXTO_CAMPO = DropDownList_area_remitente_saliente.Text
        Matri_Datos(iconsec).TEXTO_CAMPO_MODIFICADO = DropDownList_area_remitente_saliente.Text
        Matri_Datos(iconsec).Alias_Campo = "AREA_REMITENTE"

        '----------------------------------------------------------------------------
        'Asigna datos dinamicos
        '----------------------------------------------------------------------------
        For i As Integer = 0 To Matri_Datos.Length - 1
            Dim control As Object = PAGE.FindControl("RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit")
            If Not control Is Nothing Then
                If control.GetType.ToString = "System.Web.UI.WebControls.DropDownList" Then
                    Dim droplist As DropDownList = control
                    Matri_Datos(i).ID_CAMPO_ASPNET = "RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit"
                    Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = droplist.SelectedValue
                Else
                    Matri_Datos(i).ID_CAMPO_ASPNET = "RE_" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Campo_Plantilla & "-" & Matri_Datos(i).Tipo_Campo & "-Edit"
                    Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = control.text
                End If

            End If
        Next

        For i As Integer = 0 To Matri_Datos.Length - 1
            If Matri_Datos(i).Campo_Obligatorio = 1 And Matri_Datos(i).TEXTO_CAMPO = "" Then
                Actualiza_datos_interface_radicacion_saliente = "El campo " & Matri_Datos(i).Alias_Campo & " es obligatorio"
                Exit Function
            End If
        Next
        '----------------------------------------------------------------------------
        'Verifica si se realizaron cambios en el indice
        '----------------------------------------------------------------------------
        Dim cambios_campos As String
        If Not Matri_Datos Is Nothing Then
            For i As Integer = 0 To Matri_Datos.Length - 1
                If Matri_Datos(i).TEXTO_CAMPO_MODIFICADO <> Matri_Datos(i).TEXTO_CAMPO Then
                    If Matri_Datos(i).estado_dinamico_estatico = 2 Then
                        If Matri_Datos(i).TEXTO_CAMPO <> Matri_Datos(i).TEXTO_CAMPO_MODIFICADO Then
                            update_string = update_string + "," + Matri_Datos(i).Campo_Plantilla + "=" + "'" + Trim(Matri_Datos(i).TEXTO_CAMPO_MODIFICADO) + "'"
                        End If
                    End If
                    If update_string = "" Then
                        update_string = "update " + nombre_plantilla + " set " + Matri_Datos(i).Campo_Plantilla
                        If Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = "" Then
                            cambios_campos = cambios_campos & " " & Matri_Datos(i).Campo_Plantilla & " Valor anterior " & Matri_Datos(i).TEXTO_CAMPO & " Nuevo valor " & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & " "
                        Else
                            cambios_campos = cambios_campos & " " & Matri_Datos(i).Campo_Plantilla & " Valor anterior " & Matri_Datos(i).TEXTO_CAMPO & " Nuevo valor " & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & " "
                        End If
                    Else
                        If Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = "" Then
                            cambios_campos = cambios_campos & " " & Matri_Datos(i).Campo_Plantilla & " Valor anterior " & Matri_Datos(i).TEXTO_CAMPO & " Nuevo valor " & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & " "
                        Else
                            cambios_campos = cambios_campos & " " & Matri_Datos(i).Campo_Plantilla & " Valor anterior " & Matri_Datos(i).TEXTO_CAMPO & " Nuevo valor " & Matri_Datos(i).TEXTO_CAMPO_MODIFICADO & " "
                        End If

                    End If
                End If
            Next
        End If
        '------------------------------------------------------------------
        'Verifica existencia estado modulo radicado
        '------------------------------------------------------------------
        'Dim Ref_class_estado_modulo_radicado As New Class_estados_modulo_radicacion
        Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
        Dim id_estado_modulo_radicado As Integer = 0
        Dim estado_modulo_radicado As Integer = 0
        Dim sql_actualiza_estado_radicado As String = ""
        Result = Class_ra_rad_estados_modulo_radicacion.Solicita_id_estado_modulo_radicado(numero_radicado,
                                                                                           0,
                                                                                           id_estado_modulo_radicado,
                                                                                           estado_modulo_radicado)
        If Result <> "YES" Then
            Actualiza_datos_interface_radicacion_saliente = Result
            Exit Function
        End If
        If id_estado_modulo_radicado <> 0 Then
            sql_actualiza_estado_radicado = "update ra_rad_estados_modulo_radicacion set remitente=" &
                "'" & Trim(TextBox_remitente_saliente.Text) & "' where id_estado_radicado=" & id_estado_modulo_radicado
        End If
        If cambios_campos = "" Then
            Actualiza_datos_interface_radicacion_saliente = "No se detectaron cambios en los campos para actualizar"
            Exit Function
        End If
        update_string = update_string + " where Consecutivo_Rad=" + "'" + numero_radicado + "'"
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("RA_ID_USUARIO")
        Dim logi_user As String = HttpContext.Current.Session.Item("RA_LOGIN_USER")
        Dim id_user_wf As Integer = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
        Dim id_ruta_wf As Integer = HttpContext.Current.Session.Item("Id_Ruta_Workflow")
        Dim nombre_ruta_wf As String = ""
        Dim sql_tabla_wf As String = ""
        Dim sql_actualiza_tramite_wf As String = ""
        Dim hor As String = Now
        Dim detalle_trans As String = "REASIGNACION"
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans

            If update_string <> "" Then
                myCommand.CommandText = update_string
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_datos_interface_radicacion_saliente = "Imposible actualizar campos del sistema  " & update_string
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If

            '-------------------------------------------------------------
            'Actualiza log radicado
            '-------------------------------------------------------------
            Dim update As String = "INSERT INTO ra_log_radicados (desc_op,USER_OPER,ID_USER,DATE_TRANS,CONSECUTIVO_RADICADO,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES (" &
                "'" & "ACTUALIZA PLANTILLA" & "','" & logi_user & "','" & id_user & "','" & date1al & "','" & numero_radicado & "','" & cambios_campos &
                "','" & iphost & "','" & hor.ToString & "','" & "RADICACION-WEB" & "')"
            myCommand.CommandText = update
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_datos_interface_radicacion_saliente = "Imposible actualizar log  " & update
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------------------------------------
            'Actuliza estado modulo radicado
            '----------------------------------------------------------------
            If id_estado_modulo_radicado <> 0 Then
                myCommand.CommandText = sql_actualiza_estado_radicado
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    Actualiza_datos_interface_radicacion_saliente = "Imposible actualizar estado modulo radicado  " & update
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_datos_interface_radicacion_saliente = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_datos_interface_radicacion_saliente = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_datos_interface_radicacion_saliente = "Error General función Actualiza_datos_interface_radicacion_saliente " & e.Message
            Exit Function
        End Try
    End Function
    Function Verifica_existencia_radicado_asignado(ByVal consecutivo_radicado As String,
                                                   ByRef existencia As String,
                                                   ByRef id_respuesta As Integer,
                                                   ByRef estado_radicado As Integer) As String
        Try
            Dim sql_consulta As String = "Select ID_RESPUESTA_RADICADO,ESTADO_RESPUESTA from ra_respuesta_radicado where RADICADO='" & consecutivo_radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_radicado_asignado = "Función Verifica_existencia_radicado_asignado dice Error Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia = "NO"
                id_respuesta = 0
                estado_radicado = 0
                Verifica_existencia_radicado_asignado = "YES"
                Exit Function
            Else
                id_respuesta = Datset.Tables(0).Rows(0).Item(0)
                estado_radicado = Datset.Tables(0).Rows(0).Item(1)
                existencia = "YES"
                Verifica_existencia_radicado_asignado = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Verifica_existencia_radicado_asignado = "Inconsistencia general función Verifica_existencia_radicado_asignado " & ex.Message
        End Try
    End Function
    Function Retorna_Detalle_tipo_tramite(ByVal id_plantilla As Integer, ByVal nombre_documento As String,
      Optional ByRef numero_dias As Integer = 0, Optional ByRef id_gabinete As Integer = 0, Optional ByRef estado_flujo_tramite As Integer = 0,
    Optional ByRef estado_respuesta As Integer = 0, Optional ByRef nmobre_gabinete As String = "") As String
        '*******************************************************************************
        'Funcion : retorna detalle documento tramite
        'Fecha : 2016-01-29
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************************************
        Try
            numero_dias = 0
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select numero_dias_vence,codigo_gabinete_workflow,flow_tipo,requiere_respuesta,nombre_gabinete_workflow " &
              " from tipo_doc_entrante where system_plantilla_radicado_id_plantilla=" & id_plantilla &
              " and Descripcion_Doc='" & nombre_documento & "'"
            Dim Dat_reader As New DataSet
            Dim Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_Detalle_tipo_tramite = " Error funcion Retorna_Detalle_tipo_tramite   " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                If Dat_reader.Tables(0).Rows(0).IsNull(0) = False Then
                    numero_dias = Dat_reader.Tables(0).Rows(0).Item(0)
                Else
                    numero_dias = 0
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = False Then
                    id_gabinete = Dat_reader.Tables(0).Rows(0).Item(1)
                Else
                    id_gabinete = 0
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(2) = False Then
                    estado_flujo_tramite = Dat_reader.Tables(0).Rows(0).Item(2)
                Else
                    estado_flujo_tramite = 0
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(3) = False Then
                    estado_respuesta = Dat_reader.Tables(0).Rows(0).Item(3)
                Else
                    estado_respuesta = 0
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(4) = False Then
                    nmobre_gabinete = Dat_reader.Tables(0).Rows(0).Item(4)
                Else
                    nmobre_gabinete = ""
                End If

                Retorna_Detalle_tipo_tramite = "YES"
                Exit Function
            Else
                Retorna_Detalle_tipo_tramite = "Imposible encontrar detalle tramite documento"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Detalle_tipo_tramite = "Inconsistencia general funcion Retorna_Detalle_tipo_tramite " & ex.Message
        End Try
    End Function
    Function actualiza_tipo_tramite_documento_fecha_limite(ByVal nombre_plantilla As String,
                                                           ByVal numero_radicado As String,
                                                           ByVal estado_radicado As String,
                                                           ByVal fecha_limite As String,
                                                           ByVal clase_documento As String,
                                                           ByVal estru As PLANTILLA_VALIDACION_CAMPOS_ESTATICOS,
                                                           ByVal codigo_plantilla As Integer,
                                                           ByVal tipo_plantilla As String,
                                                           ByVal id_actividad_flujo As Integer,
                                                           ByVal id_usuario_workflow As Integer,
                                                           ByRef Matri_Datos() As Campos_Plantilla,
                                                           ByVal id_flujo_trabajo As Integer) As String

        '------------------------------------------------------
        'Asignar datos radicacion
        '------------------------------------------------------
        Dim refclasalmacen As New ClassRadicador
        Dim date1al As String = Date.Now
        Dim Result As String = ""
        Dim Refclas As New ClassListandoTareas
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_Fecha_Almacenamiento_Time(date1al)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim iphost As String = HttpContext.Current.Session.Item("ip_host_name")
        Dim id_user As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim logi_user As String = HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")
        Dim id_user_wf As Integer = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
        Dim id_ruta_wf As Integer = HttpContext.Current.Session.Item("Id_Ruta_Workflow")
        Dim nombre_ruta_wf As String = ""
        Dim sql_tabla_wf As String = ""
        Dim sql_actualiza_tramite_wf As String = ""
        Dim id_respuesta As Integer = 0
        Dim nombre_campo_radicado_ruta As String = ""
        Dim nombre_campo_tramite As String = ""
        '--------------------------------------------------------------
        'VALIDA EXITENCIA TRAMITE RADICADO
        '--------------------------------------------------------------
        Dim exitencia_radicado As String = "NO"
        Dim estado_radicado_resp As Integer = 0
        Result = Verifica_existencia_radicado_asignado(numero_radicado,
                                                       exitencia_radicado,
                                                       id_respuesta,
                                                       estado_radicado_resp)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        If estado_radicado_resp <> 0 Then
            actualiza_tipo_tramite_documento_fecha_limite = "El sistema registra una respuesta para el radicado, imposible actualizar el tipo de tramite"
            Exit Function
        End If
        Dim ref_gestion_respuesta As New Class_ra_respuesta_radicado
        Dim estru_resp As stru_envio = Nothing
        If id_respuesta <> 0 Then
            Result = ref_gestion_respuesta.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                            estru_resp,
                                                                                            1)

            If estru_resp.RADICADO_RESPUESTA <> "" Then
                actualiza_tipo_tramite_documento_fecha_limite = "Existe un radicado de respuesta " & " ( " & estru_resp.RADICADO_RESPUESTA & " ) para el documento imposible cambiar el tipo de trámite"
                Exit Function
            End If
        End If
        '--------------------------------------------------------------------------
        'Retorna el gabinete del tramite y la existencia del estado de respuesta
        '--------------------------------------------------------------------------
        Dim id_gabinete_nuevo_tramite As Integer = 0
        Dim nombre_nueno_gabinete As String = ""
        Dim estado_respuesta_nuevo_tramite As Integer = 0
        Result = Retorna_Detalle_tipo_tramite(codigo_plantilla,
                                              clase_documento,
                                              , id_gabinete_nuevo_tramite,
                                              , estado_respuesta_nuevo_tramite,
                                              nombre_nueno_gabinete)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        '--------------------------------------------------------------------------
        'Retorna el nombre del gabinete al que pertenece el tramite anterior
        '--------------------------------------------------------------------------
        Dim id_imagen As Long = 0
        Dim id_gabinete_tramite_anterior As Integer = 0
        Dim antiguo_nombre_gabinete As String = ""
        Result = Retorna_Detalle_tipo_tramite(codigo_plantilla,
                                              estru.Descripcion_Documento,
                                              , id_gabinete_tramite_anterior,
                                              ,
                                              ,
                                              antiguo_nombre_gabinete)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
        Dim estado_modulo_correspo As Integer = 0
        Class_tipo_doc_entrante.Determina_gestion_modulo_pqr_Tipo_Tramite(codigo_plantilla,
                                                                          clase_documento,
                                                                          estado_modulo_correspo)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        '---------------------------------------------------------------------------
        'Retorna relacion usuario gestion workflow
        '---------------------------------------------------------------------------
        Result = refclasalmacen.Retorna_relacion_usuario_gestion_workflow(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                          id_user_wf)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        If id_user_wf <> 0 Then
            '-----------------------------------------------------------------------
            'Retorna id ruta y nombre usuario
            '-----------------------------------------------------------------------
            Result = Refclas.Retorna_id_ruta_usuario_workflow(id_user_wf,
                                                              id_ruta_wf,
                                                              nombre_ruta_wf)
            If Result <> "YES" Then
                actualiza_tipo_tramite_documento_fecha_limite = Result
                Exit Function
            End If
        End If
        '--------------------------------------------------------------
        'Retorna campo radicado ruta
        '--------------------------------------------------------------
        Dim Ref_class_seleccion As New Classselecciotarea
        Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
        Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(id_ruta_wf,
                                                                          nombre_campo_radicado_ruta)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If

        '-------------------------------------------------------------
        'Retorna campo tramite documento
        '-------------------------------------------------------------
        Dim Ref_class_cinfig_listado_ruta As New Class_configuracion_listado_ruta
        Result = Ref_class_cinfig_listado_ruta.SolicitaNombreCampoTramiteRuta(id_ruta_wf,
                                                                                 nombre_campo_tramite)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        '---------------------------------------------------------------------------
        'Verfica existencia FECHALIMITERESPUESTA, TRAMITE existencia_campos_workflow
        '---------------------------------------------------------------------------
        Dim existencia_campos_workflow As String = "NO"
        Dim matri_campos() As String = Nothing
        Erase matri_campos
        If id_ruta_wf <> 0 Then
            Result = Refclas.Verifica_existencia_campos_listado_ruta(id_ruta_wf,
                                                                     existencia_campos_workflow,
                                                                     matri_campos)
            If Result <> "YES" Then
                actualiza_tipo_tramite_documento_fecha_limite = Result
                Exit Function
            End If
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(id_ruta_wf.ToString,
                                                                     nombre_ruta_wf)
            If Result <> "YES" Then
                actualiza_tipo_tramite_documento_fecha_limite = Result
                Exit Function
            End If
        End If
        '-------------------------------------------------------
        'Consulta los datos de flujo del radicado 
        '-------------------------------------------------------
        Dim existencia_flujo As String = "NO"
        Dim id_tarea_worklow As Long = 0
        Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Result = Class_DAT_ADIC_TAR.Solicita_imagen_id_tarea_relacionada_flujo_workflow(existencia_flujo,
                                                                                        nombre_ruta_wf,
                                                                                        nombre_campo_radicado_ruta,
                                                                                        numero_radicado,
                                                                                        id_imagen,
                                                                                        id_tarea_worklow)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        '--------////////////------------------------------------------------
        'ZONA GENERA SQL ACTUALIZA DATOS  TAREA WORKFLOW TABLA DAT_ADIC_TAR
        '--------////////////------------------------------------------------
        '-------------------------------------------------------
        'Solicita estructura de la tarea asignada
        '-------------------------------------------------------
        Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
        Dim stru_estado As stru_estado = Nothing
        If id_tarea_worklow <> 0 Then
            Result = Class_estados_tarea_workflow.Solicita_estructura_tarea_asignada(id_tarea_worklow,
                                                                                     stru_estado)
            If Result <> "YES" Then
                actualiza_tipo_tramite_documento_fecha_limite = Result
                Exit Function
            End If
        End If
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        'Construye comando SQL para la actualización de la tabla  DAT_ADIC_TAR
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        If id_tarea_worklow <> 0 Then
            sql_tabla_wf = "Update dat_adic_tar" & nombre_ruta_wf & " set "
            For i As Integer = 0 To matri_campos.Length - 1
                Select Case UCase(matri_campos(i))
                    Case "FECHAVENCIMIENTO"
                        If sql_actualiza_tramite_wf = "" Then
                            sql_actualiza_tramite_wf = sql_tabla_wf & " FECHAVENCIMIENTO='" & fecha_limite & "'"
                        Else
                            sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " ,FECHAVENCIMIENTO='" & fecha_limite & "'"
                        End If
                    Case nombre_campo_tramite
                        If sql_actualiza_tramite_wf = "" Then
                            sql_actualiza_tramite_wf = sql_tabla_wf & " " & nombre_campo_tramite & "='" & clase_documento & "'"
                        Else
                            sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " ," & nombre_campo_tramite & "='" & clase_documento & "'"
                        End If
                End Select
            Next
            Dim id_imagen_nuevo_gabinete As Long = 0
            '-----------------------------------------
            'Actualiza tipo gabinete
            '-----------------------------------------
            If id_gabinete_nuevo_tramite <> 0 Then
                If sql_actualiza_tramite_wf = "" Then
                    sql_actualiza_tramite_wf = sql_tabla_wf & " ID_GABINETE='" & id_gabinete_nuevo_tramite & "'"
                Else
                    sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " ,ID_GABINETE='" & id_gabinete_nuevo_tramite & "'"
                End If
            End If
            '----------------------------------------
            'Actualiza modulo radicado workflow
            '----------------------------------------
            If sql_actualiza_tramite_wf = "" Then
                sql_actualiza_tramite_wf = sql_tabla_wf & " estado_modulo_radicado='" & estado_modulo_correspo & "'"
            Else
                sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " ,estado_modulo_radicado='" & estado_modulo_correspo & "'"
            End If
            '-----------------------------------------
            'Actualiza flujo trabajo en la tabla dat
            '-----------------------------------------
            If sql_actualiza_tramite_wf = "" Then
                sql_actualiza_tramite_wf = sql_tabla_wf & " FLUJO_TRABAJO_WF='" & id_flujo_trabajo & "'"
            Else
                sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " ,FLUJO_TRABAJO_WF='" & id_flujo_trabajo & "'"
            End If

            '----------------------------------------------------------------
            'Verifica la existencia de cambio de gabinete del radicado
            '----------------------------------------------------------------
            If id_imagen <> 0 Then
                If id_gabinete_nuevo_tramite <> id_gabinete_tramite_anterior Then
                    '---------------------------------------------------------
                    'Verifica la exitencia del radicado en el nuevo gabinete
                    '---------------------------------------------------------
                    Dim nombre_campo_radicado_gabinete As String = ""
                    Dim Refclas_worflow As New ClassWorkflowDigitalizacion
                    Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
                    Result = Ref_Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(nombre_nueno_gabinete,
                                                                                             nombre_campo_radicado_gabinete)
                    If Result <> "YES" Then
                        actualiza_tipo_tramite_documento_fecha_limite = Result
                        Exit Function
                    End If
                    Result = Refclas_worflow.Solicita_id_imagen_en_gabinete_por_radicado(nombre_nueno_gabinete,
                                                                                             nombre_campo_radicado_gabinete,
                                                                                             numero_radicado,
                                                                                             id_imagen_nuevo_gabinete)
                    If Result <> "YES" Then
                        actualiza_tipo_tramite_documento_fecha_limite = Result
                        Exit Function
                    End If
                    If id_imagen_nuevo_gabinete = 0 Then
                        actualiza_tipo_tramite_documento_fecha_limite = "Intenta cambiar de tramite " & estru.Descripcion_Documento & " a " & clase_documento & ", para realizar este cambio debe almacenar  " &
                            " el documento radicado " & numero_radicado & " en el gabinete  " & nombre_nueno_gabinete & "´, el documento que debe almacenar se encuentra en este Gabinete : " & antiguo_nombre_gabinete & ")"
                        Exit Function
                    Else
                        '---------------------------------------------
                        'Actualiza id de imagen en el flujo
                        '---------------------------------------------
                        If sql_actualiza_tramite_wf = "" Then
                            sql_actualiza_tramite_wf = sql_tabla_wf & " ID_IMAGEN='" & id_imagen_nuevo_gabinete & "'"
                        Else
                            sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " ,ID_IMAGEN='" & id_imagen_nuevo_gabinete & "'"
                        End If
                    End If

                End If
            End If
            If sql_actualiza_tramite_wf <> "" Then
                sql_actualiza_tramite_wf = sql_actualiza_tramite_wf & " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea_worklow
            End If
        End If
        '---------------------------------------------------------
        'Construye sql para la actualización del tipo modulo del
        'en la tabla estados tarea  
        '---------------------------------------------------------
        Dim sql_actualiza_tarea As String = ""
        If id_tarea_worklow <> 0 Then
            sql_actualiza_tarea = "update estados_tarea_workflow set ESTADO_ACTIVIDA_MODULO_RAD=" & estado_modulo_correspo &
                " where Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_worklow
        End If
        Dim id_flujo_trabajo_actual As Integer = 0
        Dim Refclas_flujo_trabajo As New Class_flujo_trabajo_workflow
        Dim sqlactualiza_flujo_tarea As String = ""
        Dim sql_registro_reasignacion As String = ""
        '-----------------------------------------------------------------------
        'ZONA DIRECIONA USUARIO Y ACTIVIDAD PARA ACTIVIDADES EXPECIALES
        '-----------------------------------------------------------------------
        Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
        Dim id_actividad_workflow_asignar As Integer = 0
        Dim id_usuario_workflow_asignar As Integer = 0
        If id_usuario_workflow = -1 Or id_usuario_workflow = 0 Then
            id_usuario_workflow_asignar = 0
        Else
            id_usuario_workflow_asignar = id_usuario_workflow
        End If
        If id_actividad_flujo <> 0 And id_actividad_flujo <> -1 And id_tarea_worklow <> 0 Then
            Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_id_actividad_workflow_flujo_trabajo(id_actividad_flujo,
                                                                                                              id_actividad_workflow_asignar)
            If Result <> "YES" Then
                actualiza_tipo_tramite_documento_fecha_limite = Result
                Exit Function
            End If
            '-------------------------------------------------------
            'Solicita el tipo de actividad de la actividad destino
            'y la estructura de la actividad destino
            '-------------------------------------------------------
            Dim id_tipo_actividad As Integer = 0
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim Class_actividades_generales_workflow As New Class_actividades_generales_workflow
            Dim actividades_generales_workflow_ As actividades_generales_workflow = Nothing
            Result = Class_Listado_Actividades_workflow.Solicita_id_tipo_actividad_workflow(id_actividad_workflow_asignar,
                                                                                            id_tipo_actividad)
            If Result <> "YES" Then
                actualiza_tipo_tramite_documento_fecha_limite = Result
                Exit Function
            End If
            Result = Class_actividades_generales_workflow.Solicita_estructura_tipo_actividad_workflow(id_tipo_actividad,
                                                                                                      actividades_generales_workflow_)
            If Result <> "YES" Then
                actualiza_tipo_tramite_documento_fecha_limite = Result
                Exit Function
            End If
            '--------------------------------------------------------
            'ZONA DIRECIONA TAREA A USUARIO RESPONSABLE DEL RADICADO
            'ACTIVIDAD VIRTUAL QUE REPRESENTA EL USUARIO A QUIEN SE
            'LE RADICA LA TAREA, ES DECIR EL DESTINATAIO.
            'NO ES COMPATIBLE CON FLUJOS EXTERNOS
            '--------------------------------------------------------
            Dim Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
            Dim Class_plantillas_radicacion As New Class_plantillas_radicacion
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Dim Class_grupos_workflow As New Class_grupos_workflow
            Dim nombre_plantilla_radicado As String = ""
            Dim id_grupo_usuario_workflow As Integer = 0
            Dim id_usuario_worflow_destino As Integer = 0
            If actividades_generales_workflow_.Nombre_tipo_actividad = "USUARIORESPONSABLE" Then
                Dim id_usuario_gestion_radicado As Integer = 0
                Result = Class_plantillas_radicacion.Solicita_remitente_destinatario_fecha_radicado(nombre_plantilla,
                                                                                                    numero_radicado,
                                                                                                    id_usuario_gestion_radicado,
                                                                                                    0,
                                                                                                    "")
                If Result <> "YES" Then
                    actualiza_tipo_tramite_documento_fecha_limite = Result
                    Exit Function
                End If
                '----Solicita usuario workflow relacionado
                Result = Class_remit_dest_interno.Solicita_id_usuario_workflow_relacionado(id_usuario_gestion_radicado,
                                                                                           id_usuario_worflow_destino)
                If Result <> "YES" Then
                    actualiza_tipo_tramite_documento_fecha_limite = Result
                    Exit Function
                End If
                '----Solicita id grupo usuario workflow
                Result = Class_usuario_workflow.Solicita_id_grupo_usuario_workflow(id_usuario_worflow_destino,
                                                                                   id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    actualiza_tipo_tramite_documento_fecha_limite = Result
                    Exit Function
                End If
                '----Solicita id actividad relacionado al grupo workflow
                Result = Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(id_actividad_workflow_asignar,
                                                                                      id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    actualiza_tipo_tramite_documento_fecha_limite = Result
                    Exit Function
                End If
                id_usuario_workflow_asignar = id_usuario_worflow_destino
            End If
            '--------------------------------------------------------
            'ZONA DIRECIONA TAREA A USUARIO RADICADOR
            'ACTIVIDAD VIRTUAL QUE REPRESENTA EL USUARIO QUE RADICA
            'LA TAREA, NOS ES COMPATIBLE CON FLUJOS EXTERNOS
            '--------------------------------------------------------
            Dim Class_ra_usuario_radicador As New Class_ra_usuario_radicador
            If actividades_generales_workflow_.Nombre_tipo_actividad = "USUARIORESPONSABLERADICADOR" Then

                Dim id_usuario_radicador As Integer = 0
                Result = Class_plantillas_radicacion.Solicita_id_usuario_radicacion_plantilla_radicado(nombre_plantilla,
                                                                                                       numero_radicado,
                                                                                                       id_usuario_radicador)
                If Result <> "YES" Then
                    actualiza_tipo_tramite_documento_fecha_limite = Result
                    Exit Function
                End If
                Dim id_usuario_gestion_rel_radicacion As Integer = 0
                Result = Class_ra_usuario_radicador.Solicita_id_usuario_gestion_relacion_usuario_radicador(id_usuario_radicador,
                                                                                                           id_usuario_gestion_rel_radicacion)
                If Result <> "YES" Then
                    actualiza_tipo_tramite_documento_fecha_limite = Result
                    Exit Function
                End If
                If id_usuario_gestion_rel_radicacion = 0 Then
                    actualiza_tipo_tramite_documento_fecha_limite = "El codigo de usuario radicador ( " & Val(id_usuario_radicador) & " ) no registra relación de usuario de gestión"
                    Exit Function
                End If
                '----Solicita usuario workflow relacionado
                Result = Class_remit_dest_interno.Solicita_id_usuario_workflow_relacionado(id_usuario_gestion_rel_radicacion,
                                                                                           id_usuario_worflow_destino)
                If Result <> "YES" Then
                    actualiza_tipo_tramite_documento_fecha_limite = Result
                    Exit Function
                End If
                '----Solicita id grupo usuario workflow
                Result = Class_usuario_workflow.Solicita_id_grupo_usuario_workflow(id_usuario_worflow_destino,
                                                                                   id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    actualiza_tipo_tramite_documento_fecha_limite = Result
                    Exit Function
                End If
                '----Solicita id actividad relacionado al grupo workflow
                Result = Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(id_actividad_workflow_asignar,
                                                                                      id_grupo_usuario_workflow)
                If Result <> "YES" Then
                    actualiza_tipo_tramite_documento_fecha_limite = Result
                    Exit Function
                End If
                id_usuario_workflow_asignar = id_usuario_worflow_destino
            End If
        End If
        '----------------------------------------------------------------------
        'ZONA CONSTRUYE SQL PARA ACTUALIZAR EL ESTADO DE LA TAREA CON EL NUEVO
        'USUARIO, FLUJO Y TAREA WORKFLOW
        '----------------------------------------------------------------------
        If id_actividad_flujo <> 0 And id_actividad_flujo <> -1 And id_tarea_worklow <> 0 Then
            If id_flujo_trabajo <> stru_estado.ID_FLUJO_TRABAJO Then
                Dim ref_user As Object = "Null"
                If id_usuario_workflow_asignar <> 0 Then
                    ref_user = id_usuario_workflow_asignar
                End If
                sqlactualiza_flujo_tarea = "Update estados_tarea_workflow  set ID_FLUJO_TRABAJO=" & id_flujo_trabajo &
                    ",Id_Usuario=" & ref_user & ",Id_Actividad=" & id_actividad_workflow_asignar &
                    ",ID_USUARIO_WORKFLOW_FLUJO_TRABAJO=" & id_usuario_workflow_asignar & ",ID_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad_flujo &
                    " where Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_worklow & " and Fecha_Fin is null"
            End If
        Else
            sqlactualiza_flujo_tarea = "Update estados_tarea_workflow  set ID_FLUJO_TRABAJO=" & id_flujo_trabajo &
                ",ID_USUARIO_WORKFLOW_FLUJO_TRABAJO=" & 0 & ",ID_ACTIVIDAD_FLUJO_TRABAJO=" & 0 &
                    " where Inicio_Tareas_Workflow_id_Tarea=" & id_tarea_worklow & " and Fecha_Fin is null"
        End If
        If (id_actividad_flujo <> -1 And id_actividad_flujo <> 0) And id_usuario_workflow_asignar = 0 Then
            actualiza_tipo_tramite_documento_fecha_limite = "Debe selecionar el usuario de aisgnación del flujo"
            Exit Function
        End If
        '------------------------------------------
        'ZONA REGISTRA LOG REASINACION  
        '------------------------------------------
        Dim Class_wf_log_estados_workflow As New Class_wf_log_estados_workflow
        Dim Stru_wf_log_estados_workflow As Stru_wf_log_estados_workflow = Nothing
        Stru_wf_log_estados_workflow.estados_tarea_workflow_id_Estado = -1
        If id_actividad_flujo <> 0 And id_actividad_flujo <> -1 And id_tarea_worklow <> 0 Then
            If id_flujo_trabajo <> stru_estado.ID_FLUJO_TRABAJO Then
                Stru_wf_log_estados_workflow.usuario_workflow_idU_suario = HttpContext.Current.Session.Item("Id_Usuario_Workflow")
                Stru_wf_log_estados_workflow.estados_tarea_workflow_id_Estado = stru_estado.id_Estado
                Stru_wf_log_estados_workflow.id_tarea_workflow = id_tarea_worklow
                Stru_wf_log_estados_workflow.fecha_registro = date1al
                Stru_wf_log_estados_workflow.tipo_transacion = "RASIGNA ACTUALIZA TIPO TRAMITE"
                Stru_wf_log_estados_workflow.Direccion_ip_Nombre = HttpContext.Current.Session.Item("ip_host_name")
                Stru_wf_log_estados_workflow.id_usuario_anterior = stru_estado.Id_Usuario
                Stru_wf_log_estados_workflow.id_actividad_anterior = stru_estado.Id_Actividad
                Stru_wf_log_estados_workflow.id_actividad_siguiente = id_actividad_workflow_asignar
                Stru_wf_log_estados_workflow.id_usuario_siguiente = id_usuario_workflow_asignar
                Stru_wf_log_estados_workflow.estados_tarea_siguiente_workflow_id_Estado = stru_estado.id_Estado

            End If
        End If
        '-----------------------------------------------------------------------
        'ZONA PARA LA DESCRIPCIÓN DEL FLUO DE TRABAJO ANTERIOR Y EL ACTUAL
        '-----------------------------------------------------------------------
        Dim nombre_flujo_actual As String = ""
        Dim nombre_nuevo_flujo As String = ""
        If id_flujo_trabajo <> 0 Then
            Result = Refclas_flujo_trabajo.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo,
                                                                                     nombre_nuevo_flujo)
            If Result <> "YES" Then
                actualiza_tipo_tramite_documento_fecha_limite = Result
                Exit Function
            End If
        End If
        If id_flujo_trabajo_actual <> 0 Then
            Result = Refclas_flujo_trabajo.SolicitaNombreFlujoTrabajoPorIdFlujo(id_flujo_trabajo_actual,
                                                                                     nombre_flujo_actual)
            If Result <> "YES" Then
                actualiza_tipo_tramite_documento_fecha_limite = Result
                Exit Function
            End If
        End If
        '-----------------------------------------------------------------------
        'Lista las opciones plantilla
        '------------------------------------------------------------------------
        Dim Estado_opcion_fecha As Integer = 0
        Dim Estado_opcion_cita_respuesta As Integer = 0
        Dim Estado_opcion_radicado_general As Integer = 0
        Dim ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
        Result = ref_Class_system_plantilla_radicado.Lista_Opcion_Plantilla_Radicacion(codigo_plantilla,
                                                                                      Estado_opcion_fecha,
                                                                                      Estado_opcion_cita_respuesta,
                                                                                      Estado_opcion_radicado_general)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        Dim ref_Class_ra_detalle_plantilla_radicado As New Class_ra_detalle_plantilla_radicado
        Erase Matri_Datos
        Result = ref_Class_ra_detalle_plantilla_radicado.Lista_Campos_Adicionales_Plantilla(codigo_plantilla,
                                                                                            Matri_Datos,
                                                                                            Estado_opcion_fecha,
                                                                                            Estado_opcion_cita_respuesta,
                                                                                            Estado_opcion_radicado_general)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        '***************************************************************
        'Retorna datos validacion estructura
        '***************************************************************
        Dim id_tipo_tramite_documento As Integer = 0
        Result = refclasalmacen.retorna_datos_radicacion_estructura(tipo_plantilla,
                                                                    numero_radicado,
                                                                    nombre_plantilla,
                                                                    Matri_Datos)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If
        '---------------------------------------
        'Setea campos modificado
        '--------------------------------------
        If Not Matri_Datos Is Nothing Then
            For i As Integer = 0 To Matri_Datos.Length - 1
                Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = Matri_Datos(i).TEXTO_CAMPO
            Next
        End If
        Dim tramite_anterior As String = ""
        Dim fecha_vence_anterior As String = ""
        If Not Matri_Datos Is Nothing Then
            For i As Integer = 0 To Matri_Datos.Length - 1
                Select Case Matri_Datos(i).Campo_Plantilla
                    Case "Descripcion_Documento"
                        tramite_anterior = Matri_Datos(i).TEXTO_CAMPO
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = clase_documento
                        Matri_Datos(i).Alias_Campo = "DESCRIPCION TRAMITE"
                        Dim refClass_tipo_doc_entrante As New Class_tipo_doc_entrante
                        Result = refClass_tipo_doc_entrante.retorna_id_tipo_tramite_radicado(codigo_plantilla,
                                                                                             clase_documento,
                                                                                             id_tipo_tramite_documento)
                        If Result <> "YES" Then
                            actualiza_tipo_tramite_documento_fecha_limite = Result
                            Exit Function
                        End If
                    Case "FECHALIMITERESPUESTA"
                        fecha_vence_anterior = Matri_Datos(i).TEXTO_CAMPO
                        Matri_Datos(i).TEXTO_CAMPO_MODIFICADO = fecha_limite

                End Select
            Next
        End If
        '-----------------------------------------------------------
        'Crea el registro de respuesta si el nuevo tipo tramite
        'requiere de una respuesta y no existe una respuesta
        '-----------------------------------------------------------
        Dim update_respuesta As String = ""
        Dim sqlrespuesta As String = ""
        Dim tipo_resp_correo As Integer = 0
        Dim Ref_Class_tipo_doc_entrante As New Class_tipo_doc_entrante
        Result = Ref_Class_tipo_doc_entrante.Retorna_tipo_respuesta_tramite_radicado(codigo_plantilla,
                                                                                     clase_documento,
                                                                                     tipo_resp_correo)
        If Result <> "YES" Then
            actualiza_tipo_tramite_documento_fecha_limite = Result
            Exit Function
        End If

        If estado_respuesta_nuevo_tramite <> 0 Then
            '----------------------------------
            'Caso obligatoria respuesta
            '----------------------------------
            If id_respuesta = 0 Then
                '------------------------------------------------------------------------------
                'Registra respuesta  radicacion si el tramite lo requiere y no registra
                '------------------------------------------------------------------------------
                sqlrespuesta = "Insert into ra_respuesta_radicado (ID_REMIT_DEST_INT,ID_AREA,system_plantilla_radicado_id_plantilla," &
               "RADICADO,FECHA_REGISTRO,FECHA_VENCE,DESTINATARIO,TRAMITE_DOCUMENTO,codigo_dest_externo,ASUNTO,AREA_RESPONSABLE,CARGO_RESPONSABLE,USUARIO_RESPONSABLE,TIPO_RESPUESTA_ELAB_USUARIO) VALUES (" &
                estru.Destinatario_Externo_id_Dest_Ext & "," & estru.Id_area_remit_dest_interno & "," & codigo_plantilla & ",'" & numero_radicado & "','" & date1al & "','" &
                fecha_limite & "','" & estru.Remitente_Cor & "','" & clase_documento & "','" & estru.Remit_Dest_Interno_id_Remit_Dest_Int & "','" &
                estru.Asunto & "','" & estru.Area_remit_dest_interno & "','" & estru.CARGO_DESTINATARIO & "','" & estru.Destinatario_Cor & "','" & tipo_resp_correo & "')"
            Else

                '-----------------------------------------------------------------------------
                'Caso actualiza respuesta
                '-----------------------------------------------------------------------------
                update_respuesta = "Update ra_respuesta_radicado set FECHA_VENCE='" & fecha_limite & "',TRAMITE_DOCUMENTO='" & clase_documento & "',TIPO_RESPUESTA_ELAB_USUARIO=" & tipo_resp_correo & " where ESTADO_RESPUESTA=0 and RADICADO='" & numero_radicado & "'"
            End If
        Else
            '------------------------------------------------------------------------------------
            'Caso no requiere respuesta y existe una respuesta y elimina respuesta
            '------------------------------------------------------------------------------------
            If id_respuesta <> 0 Then
                sqlrespuesta = "delete from ra_respuesta_radicado where ID_RESPUESTA_RADICADO=" & id_respuesta
            End If
        End If
        '-----------------------------------------------------------
        'Sql actualiza campos plantilla de radicacion
        '----------------------------------------------------------
        Dim hour As String = Date.Now.Hour
        Dim update_actualiza_plantilla As String = "Update " & nombre_plantilla & " set FECHALIMITERESPUESTA='" & fecha_limite &
            "' , Descripcion_Documento='" & clase_documento & "' ,tipo_doc_entrante_id_tipo_doc_entrante=" & id_tipo_tramite_documento &
            " ,id_tipo_flujo_workflow=" & id_flujo_trabajo &
            " where Consecutivo_Rad='" & numero_radicado & "'"
        '-----------------------------------------------------------
        'Registra log campos tabla respuesta
        '-----------------------------------------------------------
        Dim hor As String = Now
        Dim detalle_trans As String = "CAMBIA TIPO TRAMITE"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "CAMBIA TIPO TRAMITE (" &
        " RADICADO " & numero_radicado & " TIPO TRAMITE ANTERIOR : " & tramite_anterior & " -- NUEVO TIPO TRAMITE  :  " & clase_documento &
        " -- FECHA VENCE ANTERIOR : " & fecha_vence_anterior & " -- NUEVA FECHA VENCE : " & fecha_limite & " ) IDENTIFICACION RESPEUSTA " & id_respuesta
        isert_datos = isert_datos & "('" & detalle_trans & "','" & logi_user & "','" & id_user & "','" & date1al & "'," &
                     id_respuesta & ",'" & iphost & "','" & hor & "','RADICACION-WEB','" & campos_trans & "')"

        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                             isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0

        Try
            '--------------------------------------------------------
            'Acualiza los datos de clase tramite y fecha vence en la
            'plantilla de radicacion
            '--------------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_actualiza_plantilla
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                actualiza_tipo_tramite_documento_fecha_limite = "Imposible actualizar tipo documento tipo tramite  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------------------------------
            'Actualiza la fecha de respuesta y el tipo tramite en la 
            'tabla de respuesta, si hay una respuesta
            '----------------------------------------------------------
            'If sqlrespuesta = "" Then
            If update_respuesta <> "" Then
                myCommand.CommandText = update_respuesta
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    actualiza_tipo_tramite_documento_fecha_limite = "Imposible actualizar fecha limite de respuesta  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '------------------------------------------------------
            'Inserta log de respuesta
            '------------------------------------------------------
            If update_gestion <> "" Then
                myCommand.CommandText = update_gestion
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    actualiza_tipo_tramite_documento_fecha_limite = "Imposible actualizar log de respuesta  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            'Else
            '------------------------------------------------------
            'Registra o elimina la respuesta del radicaado
            '------------------------------------------------------
            If sqlrespuesta <> "" Then
                myCommand.CommandText = sqlrespuesta
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    actualiza_tipo_tramite_documento_fecha_limite = "Imposible registrar o eliminar respuesta  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If

            '-------------------------------------
            'Inserta log radicados
            '-------------------------------------
            Dim cambios_campos As String = "(FECHALIMITERESPUESTA anterior valor=" & estru.FECHALIMITERESPUESTA & " Nuevo valor=" & fecha_limite &
                ") , (Descripcion_Documento anterior valor " & clase_documento & " Nuevo Valor=" & estru.Descripcion_Documento & " -- Cambia flujo relacionado " & nombre_flujo_actual & " por " & nombre_nuevo_flujo & ")"
            Dim update_log = "INSERT INTO ra_log_radicados (desc_op,USER_OPER,ID_USER,DATE_TRANS,CONSECUTIVO_RADICADO,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES (" &
                "'" & "ACTUALIZA TIPO TRAMITE DOCUMENTO" & "','" & logi_user & "','" & id_user & "','" & date1al & "','" & numero_radicado & "','" & cambios_campos &
                "','" & iphost & "','" & hour.ToString & "','" & "RADICACION" & "')"
            myCommand.CommandText = update_log
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                actualiza_tipo_tramite_documento_fecha_limite = "Imposible registrar log respuetas  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If sql_actualiza_tarea <> "" Then
                Result = Class_estados_tarea_workflow.Actualiza_etado_modulo_radicado(sql_actualiza_tarea)
                If Result <> "YES" Then
                    myTrans.Rollback()
                    myConnection.Close()
                    actualiza_tipo_tramite_documento_fecha_limite = "Imposible actualizar el estado del radicado " & vbCrLf & Result
                    Exit Function
                End If
            End If
            If sql_actualiza_tramite_wf <> "" Then
                Result = Refclas.actualiza_campos_workflow(sql_actualiza_tramite_wf)
                If Result <> "YES" Then
                    myTrans.Rollback()
                    myConnection.Close()
                    actualiza_tipo_tramite_documento_fecha_limite = "Imposible actualizar el indice workflow " & vbCrLf & Result
                    Exit Function
                End If
            End If
            If sqlactualiza_flujo_tarea <> "" Then
                Result = Refclas.actualiza_campos_workflow(sqlactualiza_flujo_tarea)
                If Result <> "YES" Then
                    myTrans.Rollback()
                    myConnection.Close()
                    actualiza_tipo_tramite_documento_fecha_limite = "Imposible actualizar el flujo " & vbCrLf & Result
                    Exit Function
                End If
            End If
            If Stru_wf_log_estados_workflow.estados_tarea_workflow_id_Estado <> -1 Then
                Result = Class_wf_log_estados_workflow.Registra_log_estado_tarea_worlkflow(Stru_wf_log_estados_workflow)
                If Result <> "YES" Then
                    myTrans.Rollback()
                    myConnection.Close()
                    actualiza_tipo_tramite_documento_fecha_limite = "No se pudo registrar el log de reasignacion " & Result
                End If
            End If

            myTrans.Commit()
            myConnection.Close()
            actualiza_tipo_tramite_documento_fecha_limite = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    actualiza_tipo_tramite_documento_fecha_limite = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            actualiza_tipo_tramite_documento_fecha_limite = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Lista_plantilla_radicado_combo(ByVal id_empresa As Integer, ByRef refdrowlist As DropDownList) As String
        Try
            refdrowlist.Items.Clear()
            Dim sql_consulta As String = "Select Nombre_Plantilla_Radicado from system_plantilla_radicado where EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA='" & id_empresa & "' and Estado_Plantilla=1 "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_plantilla_radicado_combo = "Función Lista_plantilla_radicado_combo dice Error de Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_plantilla_radicado_combo = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refdrowlist.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Lista_plantilla_radicado_combo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_plantilla_radicado_combo = "Inconsistencia general función Lista_plantilla_radicado_combo " & ex.Message
        End Try
    End Function
    Function Retorna_detalle_usuario_gestion(ByVal id_usuario_gestion As Integer,
    ByRef nombre_usuario_gestion As String, ByRef cargo_usuario_gestion As String) As String
        '*********************************************************
        'Funcion : Asigna detalle usuario gestion
        'Fecha 2015-07-27
        'Ingeniero : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim Parametro_Consulta As String = "select Nombre_Remitente,Cargo_Remite from remit_dest_interno where id_Remit_Dest_Int =" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_detalle_usuario_gestion = "Función Retorna_detalle_usuario_gestion dice Error de Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_detalle_usuario_gestion = "Función Retorna_detalle_usuario_gestion dice : Imposible encontrar detalle de usuario de gestión  "
                Exit Function
            Else
                nombre_usuario_gestion = Datset.Tables(0).Rows(0).Item("Nombre_Remitente")
                cargo_usuario_gestion = Datset.Tables(0).Rows(0).Item("Cargo_Remite")
                Retorna_detalle_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_detalle_usuario_gestion = "Inconsistencia general funcion Retorna_detalle_usuario_gestion " & ex.Message
        End Try
    End Function

    Function Retorna_detalle_radicados_usuario(ByVal nombre_plantilla As String, ByVal id_usuario_gestion As Integer, ByRef matri_radicado() As estructura_radicado, ByVal fecha_ini As String,
        ByVal fecha_fin As String, ByVal hora_ini As String, ByVal hora_fin As String) As String
        Try
            Dim Parametro_Consulta As String = "Select Consecutivo_Rad,Fecha_Radicado,Numero_Folios,Descripcion_Documento,Anexos_Cor,Remitente_Cor from " & nombre_plantilla
            If fecha_fin <> "" Then
                If hora_ini <> "00" And hora_fin <> "00" Then
                    Parametro_Consulta = Parametro_Consulta & " where Fecha_Radicado between '" & fecha_ini & " " & hora_ini & ":00:00" & "' and '" & fecha_fin & " " & hora_fin & ":00:00" & "' and Destinatario_Externo_id_Dest_Ext=" & id_usuario_gestion
                Else
                    Parametro_Consulta = Parametro_Consulta & " where CONVERT(Fecha_Radicado,DATE) between '" & fecha_ini & "' and '" & fecha_fin & "' and Destinatario_Externo_id_Dest_Ext=" & id_usuario_gestion
                End If

            Else
                Parametro_Consulta = Parametro_Consulta & " where CONVERT(Fecha_Radicado,DATE) ='" & fecha_ini & "' and Destinatario_Externo_id_Dest_Ext=" & id_usuario_gestion
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_detalle_radicados_usuario = "Función Retorna_detalle_radicados_usuario dice Error de Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_detalle_radicados_usuario = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_radicado(i)
                    matri_radicado(i).radicado = Datset.Tables(0).Rows(i).Item(0)
                    matri_radicado(i).fecha_radicado = Datset.Tables(0).Rows(i).Item(1)
                    matri_radicado(i).numero_folio = Datset.Tables(0).Rows(i).Item(2)
                    matri_radicado(i).tramite_radicado = Datset.Tables(0).Rows(i).Item(3)
                    matri_radicado(i).anexo_cor = Datset.Tables(0).Rows(i).Item(4)
                    matri_radicado(i).remitente = Datset.Tables(0).Rows(i).Item(5)
                Next
                Retorna_detalle_radicados_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_detalle_radicados_usuario = "Inconsistencia función Retorna_detalle_radicados_usuario " & ex.Message
        End Try
    End Function
    Function Retorna_id_Are_por_nombre(ByVal nombre_area As String,
                                       ByRef id_area As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select Codigo_Area from areas_depart_radicacion where Nombre_Area='" & nombre_area & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_Are_por_nombre = "Función Retorna_id_Are_por_nombre dice Error de Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_Are_por_nombre = "YES"
                Exit Function
            Else
                id_area = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_Are_por_nombre = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_Are_por_nombre = "Inconsistencia general funcion Retorna_id_Are_por_nombre " & ex.Message
        End Try
    End Function
    Public Function Retorna_Id_Usuario_gestion(ByVal Logi_User As String,
                                               ByRef Id_usuarioWf As Integer) As String
        '********************************************
        'Funcion: Esta funcion devuelve el codigo
        'de usuario workflow enviando como parametro
        'el login de usuario
        'Fecha : 2012-06-07
        'Ingenieniero: Miguel Angel Urueta Miranda
        '*******************************************

        Try
            Dim Parametro_Consulta As String = "SELECT  id_Remit_Dest_Int " _
                         & "FROM remit_dest_interno " _
                         & "WHERE Nombre_Remitente='" & Logi_User & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_Usuario_gestion = "Función Retorna_Id_Usuario_gestion dice Error de Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Id_Usuario_gestion = "Imposible encotrar datos el id del usuario"
                Exit Function
            Else
                Id_usuarioWf = Datset.Tables(0).Rows(0).Item(0)
                Retorna_Id_Usuario_gestion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Id_Usuario_gestion = "YES" = "Error General funcion Retorna_Id_Usuario_gestion  " & ex.Message
        End Try
    End Function
    Function Retorna_usuarios_con_radicados_por_area(ByVal nombre_plantilla As String,
                                                     ByVal id_area As String,
                                                     ByVal fecha_ini As String,
                                                     ByVal fecha_fin As String,
                                                     ByRef matri() As Integer,
                                                     ByVal id_usuario_gestion As Integer,
                                                     ByVal hora_ini As String,
                                                     ByVal hora_fin As String) As String
        Try
            Dim Parametro_Consulta As String = "Select distinct (Destinatario_Externo_id_Dest_Ext) from " & nombre_plantilla
            If fecha_fin <> "" Then
                '    Parametro_Consulta = Parametro_Consulta & " where CONVERT(Fecha_Radicado,DATE) between '" & fecha_ini & "' and '" & fecha_fin & "' and Destinatario_Externo_id_Dest_Ext=" & id_usuario_gestion
                'Else
                '    Parametro_Consulta = Parametro_Consulta & " where CONVERT(Fecha_Radicado,DATE) ='" & fecha_ini & "' and Destinatario_Externo_id_Dest_Ext=" & id_usuario_gestion
                If hora_ini <> "00" And hora_fin <> "00" Then
                    Parametro_Consulta = Parametro_Consulta & " where Fecha_Radicado between '" & fecha_ini & " " & hora_ini & ":00:00" & "' and '" & fecha_fin & " " & hora_fin & ":00:00" & "' and Destinatario_Externo_id_Dest_Ext=" & id_usuario_gestion
                Else
                    Parametro_Consulta = Parametro_Consulta & " where CONVERT(Fecha_Radicado,DATE) between '" & fecha_ini & "' and '" & fecha_fin & "' and Destinatario_Externo_id_Dest_Ext=" & id_usuario_gestion
                End If
            End If
            Erase matri
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_usuarios_con_radicados_por_area = "Función Retorna_detalle_radicados_usuario dice Error de Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_usuarios_con_radicados_por_area = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri(i)
                    matri(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_usuarios_con_radicados_por_area = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_usuarios_con_radicados_por_area = "Inconsistencia general función Retorna_usuarios_con_radicados_por_area " & ex.Message
        End Try
    End Function
    Function Retorna_usuarios_con_radicados_por_area(ByVal nombre_plantilla As String, ByVal id_area As String, ByVal fecha_ini As String,
        ByVal fecha_fin As String, ByRef matri() As Integer, ByVal hora_ini As String, ByVal hora_fin As String) As String
        Try
            Dim Parametro_Consulta As String = "Select distinct (Destinatario_Externo_id_Dest_Ext) from " & nombre_plantilla
            If fecha_fin <> "" Then
                '    Parametro_Consulta = Parametro_Consulta & " where CONVERT(Fecha_Radicado,DATE) between '" & fecha_ini & "' and '" & fecha_fin & "' and id_Area_remit_dest_interno=" & id_area
                'Else
                '    Parametro_Consulta = Parametro_Consulta & " where CONVERT(Fecha_Radicado,DATE) ='" & fecha_ini & "' and id_Area_remit_dest_interno=" & id_area
                If hora_ini <> "00" And hora_fin <> "00" Then
                    Parametro_Consulta = Parametro_Consulta & " where Fecha_Radicado between '" & fecha_ini & " " & hora_ini & ":00:00" & "' and '" & fecha_fin & " " & hora_fin & ":00:00" & "' and id_Area_remit_dest_interno=" & id_area
                Else
                    Parametro_Consulta = Parametro_Consulta & " where CONVERT(Fecha_Radicado,DATE) ='" & fecha_ini & "' and id_Area_remit_dest_interno=" & id_area
                End If
            End If
            Erase matri
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_usuarios_con_radicados_por_area = "Función Retorna_detalle_radicados_usuario dice Error de Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_usuarios_con_radicados_por_area = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri(i)
                    matri(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_usuarios_con_radicados_por_area = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_usuarios_con_radicados_por_area = "Inconsistencia general función Retorna_usuarios_con_radicados_por_area " & ex.Message
        End Try
    End Function
    Public Function Lista_Nombre_Entidad(ByRef _Plantilla_Impresion() As Plantilla_Impresion) As String
        Try

            Dim Parametro_Consulta As String = "select * from Empresa_Radicacion_correspondencia"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_Nombre_Entidad = "Función Lista_Nombre_Entidad dice Error de Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_Nombre_Entidad = "Imposible encontrar entidad radicadora"
                Exit Function
            Else
                _Plantilla_Impresion(0).Valor_Campo = Datset.Tables(0).Rows(0).Item(0).ToString
                Lista_Nombre_Entidad = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_Nombre_Entidad = ex.ToString
        End Try
    End Function
    Public Function Lista_Nombre_Entidad(ByRef _Plantilla_Impresion() As String) As String
        Try
            Dim Parametro_Consulta As String = "select * from Empresa_Radicacion_correspondencia"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_Nombre_Entidad = "Función Lista_Nombre_Entidad dice Error de Conexión Base Datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_Nombre_Entidad = "Imposible encontrar entidad radicadora"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    ReDim Preserve _Plantilla_Impresion(i)
                    _Plantilla_Impresion(i) = Datset.Tables(0).Rows(0).Item(i).ToString
                Next
                Lista_Nombre_Entidad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_Nombre_Entidad = ex.Message
        End Try
    End Function
    Function Lista_remisiones_tramites_radicados(ByVal nombre_plantilla As String,
                                                 ByVal nombre_area As String,
                                                 ByVal nombre_usuario As String,
                                                 ByVal fecha_ini As String,
                                                 ByVal fechafin As String,
                                                 ByRef archivo As String,
                                                 ByVal hora_ini As String,
                                                 ByVal hora_fin As String) As String
        Try
            Dim Result As String = ""
            Dim id_area As Integer = 0
            Result = Me.Retorna_id_Are_por_nombre(nombre_area,
                                                  id_area)
            If Result <> "YES" Then
                Lista_remisiones_tramites_radicados = Result
                Exit Function
            End If
            Dim matri_user() As Integer
            Erase matri_user
            If nombre_usuario <> "SELECCIONE" Then
                Dim split() As String = nombre_usuario.Split("((")
                Dim id_user As Integer = 0
                Result = Me.Retorna_Id_Usuario_gestion(Trim(split(0)), id_user)
                If Result <> "YES" Then
                    Lista_remisiones_tramites_radicados = Result
                    Exit Function
                End If
                Result = Me.Retorna_usuarios_con_radicados_por_area(nombre_plantilla,
                                                                    id_area,
                                                                    fecha_ini,
                                                                    fechafin,
                                                                    matri_user,
                                                                    id_user,
                                                                    hora_ini,
                                                                    hora_fin)
                If Result <> "YES" Then
                    Lista_remisiones_tramites_radicados = Result
                    Exit Function
                End If
                If matri_user Is Nothing Then
                    Lista_remisiones_tramites_radicados = "No se encontraron resultados para remisión"
                    Exit Function
                End If
            Else
                Result = Me.Retorna_usuarios_con_radicados_por_area(nombre_plantilla,
                                                                    id_area,
                                                                    fecha_ini,
                                                                    fechafin,
                                                                    matri_user,
                                                                    hora_ini,
                                                                    hora_fin)
                If Result <> "YES" Then
                    Lista_remisiones_tramites_radicados = Result
                    Exit Function
                End If
                If matri_user Is Nothing Then
                    Lista_remisiones_tramites_radicados = "No se encontraron resultados para remisión"
                    Exit Function
                End If
            End If
            '---------------------------------------------
            'Recorre los usuarios que tiene radicados
            '---------------------------------------------
            'Dim archivo As String = ""
            Dim nombre_usuario_ref As String = ""
            Dim cargo_usario As String = ""
            Dim _Plantilla_Impre() As String
            Dim _Campo_NIT As String = ""
            Dim _Campo_NIT_REAL As String = ""
            Erase _Plantilla_Impre
            Result = Lista_Nombre_Entidad(_Plantilla_Impre)
            If Result <> "YES" Then
                Lista_remisiones_tramites_radicados = Result
                Exit Function
            End If
            Randomize()
            Dim value As Integer = CInt(Int((1000 * Rnd()) + 1))
            Result = genera_documento_remision(nombre_area,
                                               _Plantilla_Impre(0),
                                               _Plantilla_Impre(1),
                                               archivo,
                                               matri_user,
                                               nombre_plantilla,
                                               fecha_ini,
                                               fechafin,
                                               value,
                                               hora_ini,
                                               hora_fin)
            If Result <> "YES" Then
                Lista_remisiones_tramites_radicados = Result
                Exit Function
            End If
            Lista_remisiones_tramites_radicados = "YES"
        Catch ex As Exception
            Lista_remisiones_tramites_radicados = "Inconsistencia general función Lista_remisiones_tramites_radicados " & ex.Message
        End Try
    End Function
    Function genera_documento_remision(ByVal area_remitente As String,
                                        ByVal nombre_empresa As String,
                                        ByVal nit_empresa As String,
                                        ByRef archivo As String,
                                        ByVal matri_user() As Integer,
                                        ByVal nombre_plantilla As String,
                                        ByVal fecha_ini As String,
                                        ByVal fechafin As String,
                                        ByVal value As Integer,
                                        ByVal hora_ini As String,
                                        ByVal hora_fin As String) As String
        Dim doc As Document
        Dim writer As PdfWriter
        Try
            Dim ref_clas_rad As New ClassRadicador
            Dim Ruta_Sesion = HttpContext.Current.Session("RA_RUTA_TEMPO_IMPRESION").ToString()
            Dim rutafinal As String = Ruta_Sesion & "\"
            Dim Rutatemp As String = ""
            Rutatemp = rutafinal & "TEMPREMISION" & "\"
            If Directory.Exists(Rutatemp) = False Then
                Directory.CreateDirectory(Rutatemp)
            End If
            Dim archivo_pdf As String = Rutatemp & "temp_" & "RA" & value & ".pdf"
            doc = New Document(PageSize.LETTER)
            doc.SetPageSize(PageSize.LETTER.Rotate())
            If archivo = "" Then
                writer = PdfWriter.GetInstance(doc,
                               New FileStream(archivo_pdf, FileMode.Create))
                writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE)
            Else
                writer = PdfWriter.GetInstance(doc,
                              New FileStream(archivo_pdf, FileMode.Append))
                writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE)
            End If
            Dim nombre_usuario As String = ""
            Dim cargo_usario As String = ""
            doc.Open()
            Dim Result As String = ""
            For i As Integer = 0 To matri_user.Length - 1
                Result = Me.Retorna_detalle_usuario_gestion(matri_user(i),
                                                            nombre_usuario,
                                                            cargo_usario)
                If Result <> "YES" Then
                    genera_documento_remision = Result
                    Exit Function
                End If
                Dim estru_radicado() As estructura_radicado
                Erase estru_radicado
                Result = Me.Retorna_detalle_radicados_usuario(nombre_plantilla,
                                                              matri_user(i),
                                                              estru_radicado,
                                                              fecha_ini,
                                                              fechafin,
                                                              hora_ini,
                                                              hora_fin)
                If Result <> "YES" Then
                    genera_documento_remision = Result
                    Exit Function
                End If
                If estru_radicado Is Nothing Then
                    genera_documento_remision = "YES"
                    Exit Function
                End If
                If i > 0 Then
                    doc.NewPage()
                End If
                Dim ruta_image As String = HttpContext.Current.Server.MapPath("../imagera/logo_trd.png")
                Dim imagen As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(ruta_image)
                imagen.BorderWidth = 0
                imagen.Alignment = Element.ALIGN_LEFT
                Dim percentage As Object = 0.0F
                percentage = 100 / imagen.Width
                imagen.ScalePercent(percentage * 80)
                'Insertamos la imagen en el documento
                doc.Add(imagen)
                Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
                   12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
                Dim _standardFont_datos_unidad_conservacion As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
                12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
                Dim Dat As Date = Now
                Dim paragraf As New Paragraph
                paragraf = New Paragraph("FORMATO UNICO DE ENTREGA DE DOCUMENTOS RADICADOS", _standardFont)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
                paragraf = New Paragraph(nombre_empresa, _standardFont)
                paragraf.Alignment = Element.ALIGN_RIGHT
                doc.Add(paragraf)
                paragraf = New Paragraph(nit_empresa, _standardFont)
                paragraf.Alignment = Element.ALIGN_RIGHT
                doc.Add(paragraf)
                _standardFont.Size = 10
                paragraf = New Paragraph("Documentos relacionados (" & estru_radicado.Length & ") Fecha reporte " & Dat, _standardFont)
                paragraf.Alignment = Element.ALIGN_LEFT
                doc.Add(paragraf)
                paragraf = New Paragraph("Entregados a : " & nombre_usuario & " (" & cargo_usario & ")", _standardFont)
                paragraf.Alignment = Element.ALIGN_LEFT
                doc.Add(paragraf)
                paragraf = New Paragraph("Area : " & area_remitente, _standardFont)
                paragraf.Alignment = Element.ALIGN_LEFT
                doc.Add(paragraf)
                doc.Add(Chunk.NEWLINE)
                Dim tblrdatos As PdfPTable = New PdfPTable(6)
                tblrdatos.WidthPercentage = 100
                Dim cltipounidad As PdfPCell = New PdfPCell(New Phrase("RADICADO", _standardFont_datos_unidad_conservacion))
                cltipounidad.BorderWidth = 1
                Dim cltipounidad_valor As PdfPCell = New PdfPCell(New Phrase("FECHA RADICADO", _standardFont_datos_unidad_conservacion))
                cltipounidad_valor.BorderWidth = 1
                Dim cltipounidad_folio As PdfPCell = New PdfPCell(New Phrase("NUMERO FOLIOS", _standardFont_datos_unidad_conservacion))
                cltipounidad_folio.BorderWidth = 1
                Dim cltipounidad_descripcion As PdfPCell = New PdfPCell(New Phrase("DESCRIPCION", _standardFont_datos_unidad_conservacion))
                cltipounidad_descripcion.BorderWidth = 1
                Dim cltipounidad_anexo As PdfPCell = New PdfPCell(New Phrase("ANEXO", _standardFont_datos_unidad_conservacion))
                cltipounidad_anexo.BorderWidth = 1
                Dim cltipounidad_remit As PdfPCell = New PdfPCell(New Phrase("REMITE", _standardFont_datos_unidad_conservacion))
                cltipounidad_remit.BorderWidth = 1
                tblrdatos.AddCell(cltipounidad)
                tblrdatos.AddCell(cltipounidad_valor)
                tblrdatos.AddCell(cltipounidad_folio)
                tblrdatos.AddCell(cltipounidad_descripcion)
                tblrdatos.AddCell(cltipounidad_anexo)
                tblrdatos.AddCell(cltipounidad_remit)
                Dim _standardFont_datos_unidad_conservacion_table As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
               9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
                For i2 As Integer = 0 To estru_radicado.Length - 1
                    Dim cltipounidad_ As PdfPCell = New PdfPCell(New Phrase(estru_radicado(i2).radicado, _standardFont_datos_unidad_conservacion_table))
                    'cltipounidad_.BorderWidth = 1
                    Dim cltipounidad_valor_ As PdfPCell = New PdfPCell(New Phrase(estru_radicado(i2).fecha_radicado, _standardFont_datos_unidad_conservacion_table))
                    'cltipounidad_valor_.BorderWidth = 1
                    Dim cltipounidad_folio_ As PdfPCell = New PdfPCell(New Phrase(estru_radicado(i2).numero_folio, _standardFont_datos_unidad_conservacion_table))
                    'cltipounidad_folio_.BorderWidth = 1
                    Dim cltipounidad_descripcion_ As PdfPCell = New PdfPCell(New Phrase(estru_radicado(i2).tramite_radicado, _standardFont_datos_unidad_conservacion_table))
                    'cltipounidad_descripcion_.BorderWidth = 1
                    Dim cltipounidad_anexo_ As PdfPCell = New PdfPCell(New Phrase(estru_radicado(i2).anexo_cor, _standardFont_datos_unidad_conservacion_table))
                    Dim cltipounidad_remit_ As PdfPCell = New PdfPCell(New Phrase(estru_radicado(i2).remitente, _standardFont_datos_unidad_conservacion_table))
                    'cltipounidad_descripcion.BorderWidth = 1
                    tblrdatos.AddCell(cltipounidad_)
                    tblrdatos.AddCell(cltipounidad_valor_)
                    tblrdatos.AddCell(cltipounidad_folio_)
                    tblrdatos.AddCell(cltipounidad_descripcion_)
                    tblrdatos.AddCell(cltipounidad_anexo_)
                    tblrdatos.AddCell(cltipounidad_remit_)
                Next
                doc.Add(tblrdatos)
                If tblrdatos.TotalHeight >= 448.0 And doc.PageNumber = 1 Then
                    doc.NewPage()
                End If
                doc.Add(Chunk.NEWLINE)
                doc.Add(Chunk.NEWLINE)
                doc.Add(Chunk.NEWLINE)
                paragraf = New Paragraph("________________________________________", _standardFont)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
                paragraf = New Paragraph(nombre_usuario, _standardFont)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
                paragraf = New Paragraph(cargo_usario, _standardFont)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
                paragraf = New Paragraph(area_remitente, _standardFont)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
            Next
            doc.Close()
            writer.Close()
            archivo = archivo_pdf
            genera_documento_remision = "YES"
        Catch ex As Exception
            genera_documento_remision = "Inconsistencia general función genera_documento_remision " & ex.Message
        Finally

        End Try
    End Function
End Class
