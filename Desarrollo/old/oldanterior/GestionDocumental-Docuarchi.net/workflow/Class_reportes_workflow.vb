Public Class Class_reportes_workflow
    Function Consulta_Reporte(ByVal Dato_Sql As String, _
                              ByVal nombre_reporte As String, _
                              ByRef pag As Page) As String
        Try
            Dim MatriSql() As String = Split(Dato_Sql, "//")
            Dim MatriParametros() As String
            Dim Ref As New ClassWorkflowReportes
            Dim Result As String = ""
            Erase MatriParametros
            Dim Tableparametro As Panel = pag.FindControl("Tableparametro")
            If Tableparametro Is Nothing Then
                Consulta_Reporte = "Imposible encontrar el control (Crear_Parametro_consulta )"
                Exit Function
            End If
            Dim UpdatePanel_parametros As UpdatePanel = pag.FindControl("UpdatePanel_parametros")
            If UpdatePanel_parametros Is Nothing Then
                Consulta_Reporte = "Imposible encontrar el control (UpdatePanel_parametros )"
                Exit Function
            End If
            'verifica que tenga parametros la consulta
            If Not MatriSql Is Nothing And UBound(MatriSql) > 0 Then
                MatriParametros = Split(MatriSql(1), "#")
            End If
            'Verifica parametros de consulta
            Dim i As Integer = 0
            Dim Darodoc As String = ""
            Dim CodigoSQL2 As String = ""
            If Not MatriParametros Is Nothing Then
                If MatriParametros(0) <> "" Then
                    Result = Me.Crear_Parametro_consulta(MatriParametros, _
                                                         pag)
                    If Result <> "YES" Then
                        Consulta_Reporte = Result
                        Exit Function
                    End If
                Else
                    Tableparametro.Controls.Clear()
                    UpdatePanel_parametros.Update()
                    Result = Ref.Resultado_consulta(pag, Trim(MatriSql(0)), _
                                                    nombre_reporte)
                    If Result <> "YES" Then
                        Consulta_Reporte = Result
                        Exit Function
                    End If
                End If
                Consulta_Reporte = "YES"
                Exit Function
            Else
                Consulta_Reporte = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Consulta_Reporte = "Función Consulta_Reporte inconsistencia general " & ex.Message
        End Try
    End Function
    Function Crear_Parametro_consulta(ByVal Matri_Parameros() As String, _
                                      ByRef pag As Page) As String
        Try
            Dim i As Integer = 0
            Dim RegisTro = New TableRow
            Dim CellDa = New TableCell
            Dim Lebol = New Label
            Dim Result As String = ""
            Dim refclas As New ClassWorkflowReportes
            Dim refclas_rad As New ClassRadicador
            Dim Tableparametro As Table = pag.FindControl("Tableparametro")
            If Tableparametro Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (Crear_Parametro_consulta )"
                Exit Function
            End If
            Dim Panel_parametros_consulta As Panel = pag.FindControl("Panel_parametros_consulta")
            If Panel_parametros_consulta Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (Panel_parametros_consulta )"
                Exit Function
            End If
            Dim Hidden_parametro_sel As Object = pag.FindControl("Hidden_parametro_sel")
            If Hidden_parametro_sel Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (Hidden_parametro_sel )"
                Exit Function
            End If
            Dim Button_reporte As Button = pag.FindControl("Button_reporte")
            If Button_reporte Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (Button_reporte )"
                Exit Function
            End If
            Dim UpdatePanel_parametros As UpdatePanel = pag.FindControl("UpdatePanel_parametros")
            If UpdatePanel_parametros Is Nothing Then
                Crear_Parametro_consulta = "Imposible encontrar el control (UpdatePanel_parametros )"
                Exit Function
            End If
            For i = 0 To UBound(Matri_Parameros) - 1
                RegisTro = New TableRow
                CellDa = New TableCell
                Dim TexboxC As New TextBox
                Dim Labelc As New Label
                Dim textolabel As String = ""
                Dim nombre_tabla As String = ""
                Dim nombre_campo As String = ""
                Dim estado_campo_fecha As String = ""
                If Matri_Parameros(i) <> "" Then
                    If InStr(Matri_Parameros(i), "$") > 0 Then
                        textolabel = Matri_Parameros(i).Split("$")(0)
                        nombre_campo = Matri_Parameros(i).Split("$")(1)
                        nombre_tabla = Matri_Parameros(i).Split("$")(2)
                        estado_campo_fecha = Matri_Parameros(i).Split("$")(3)
                        Matri_Parameros(i) = textolabel
                        If nombre_campo <> "" And nombre_tabla <> "" Then
                            Result = refclas_rad.agregar_auto_complete_workflow(Matri_Parameros(i), _
                                                                                Panel_parametros_consulta, _
                                                                                "GetPosiblesDatos", _
                                                                                nombre_tabla, _
                                                                                nombre_campo)
                            If Result <> "YES" Then
                                Crear_Parametro_consulta = Result
                                Exit Function
                            End If
                        End If
                    Else
                        textolabel = Matri_Parameros(i)
                    End If
                    Labelc.Style.Add("float", "left")
                    TexboxC.Style.Add("float", "left")
                    TexboxC.ID = Matri_Parameros(i)
                    Labelc.Text = textolabel
                    Labelc.Attributes.Add("Class", "h6 font-weight-light")
                    'CellDa.Font.Bold = True
                    CellDa.Controls.Add(Labelc)
                    RegisTro.Controls.Add(CellDa)
                    CellDa = New TableCell
                    CellDa.Controls.Add(TexboxC)
                    If estado_campo_fecha = "DATE" Then
                        TexboxC.MaxLength = 10
                        TexboxC.Attributes.Add("onkeypress", "GetChar (event);")
                        TexboxC.Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                        TexboxC.Attributes.Add("placeholder", "yyyy mm dd")
                        TexboxC.Width = 95
                        TexboxC.Attributes.Add("class", "ml-0 form-control")
                        Dim bhtml As New HtmlControls.HtmlGenericControl("button")
                        bhtml.ID = "Fecha_ela_" & TexboxC.ID
                        bhtml.Attributes.Add("class", "ml-1 btn btn-success border-0")
                        bhtml.Attributes.Add("font-size", "10px")
                        bhtml.Attributes.Add("title", "formato aaaa mm dd")
                        Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Attributes.Add("class", "fad fa-calendar-alt fa-1x")
                        bhtml.Controls.Add(ihtml)
                        CellDa.Controls.Add(bhtml)
                        
                        Result = refclas_rad.Agregar_Calendar(bhtml.ID, TexboxC.ID, Panel_parametros_consulta)
                        If Result <> "YES" Then
                            'Genera_Interface_Radicacion_Entrante = Result
                            'Exit Function
                        End If

                    Else
                        TexboxC.Style.Add("width", "90%")
                        TexboxC.Attributes.Add("Class", "form-control m-2 w-90")
                    End If
                    RegisTro.Controls.Add(CellDa)
                    Tableparametro.Controls.Add(RegisTro)
                End If
            Next
            RegisTro = New TableRow
            CellDa = New TableCell
            RegisTro.Controls.Add(CellDa)
            CellDa = New TableCell
            Dim Butonc As New Button
            Butonc.Attributes.Add("Class", "btn btn-success mt-5")
            Butonc.Attributes.Add("float", "rigth")
            Butonc.Text = "Consultar"
            AddHandler Butonc.Click, AddressOf Button_Click
            CellDa.Controls.Add(Butonc)
            RegisTro.Controls.Add(CellDa)
            Tableparametro.Controls.Add(RegisTro)
            'Asignar matris de parametros al boton
            Butonc.ID = "consulta_parametro_index"
            'Butonc.CssClass = "boton_blanco"
            Hidden_parametro_sel.Value = "1|"
            For i = 0 To UBound(Matri_Parameros)
                'Butonc.ID = Butonc.ID & Matri_Parameros(i) & "|"
                Hidden_parametro_sel.Value = Hidden_parametro_sel.Value & Matri_Parameros(i) & "|"
            Next
            Button_reporte.Visible = False
            UpdatePanel_parametros.Update()
            Crear_Parametro_consulta = "YES"
            Exit Function
        Catch ex As Exception
            Crear_Parametro_consulta = "Inconsistencia función Crear_Parametro_consulta " & ex.Message
        End Try
    End Function
    Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim clasjava As New Classscrripjava
        Dim UpdatePanel_parametros As UpdatePanel = sender.page.findcontrol("UpdatePanel_parametros")
        Try
            Dim Hidden_parametro_sel As Object = sender.page.findcontrol("Hidden_parametro_sel")
            Dim TreeView1 As TreeView = sender.page.findcontrol("TreeView1")
            Dim update_tre_principal As UpdatePanel = sender.page.findcontrol("update_tre_principal")
            Dim Matri_Parametro() As String
            Dim RefDato_Sql_Consulta As String = Trim(HttpContext.Current.Session.Item("Dato_Sql_Consulta"))
            Dim ref As New ClassWorkflowReportes
            Dim Result As String = ""
            Erase Matri_Parametro
            Matri_Parametro = Split(Hidden_parametro_sel.Value, "|")
            Dim i As Integer = 0
            Dim Darodoc As String
            Dim Matri_Nodo() As String
            Erase Matri_Nodo
            Dim Datos_Nodo As String = ""
            Result = ref.NodoChild_Selecionado(TreeView1, _
                                               Datos_Nodo)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, update_tre_principal)
                Exit Sub
            Else
                Matri_Nodo = Split(Datos_Nodo, "|")
            End If
            'Remplazar parametros en la cosnulta
            If Not Matri_Parametro Is Nothing Then
                For i = 1 To UBound(Matri_Parametro)
                    Darodoc = "#" & Matri_Parametro(i)
                    If InStr(RefDato_Sql_Consulta, Darodoc) Then
                        Dim ValorParametro As String = ""
                        Result = obtener_valor_parametro(Matri_Parametro(i), _
                                                         ValorParametro, _
                                                         sender.page)
                        If Result <> "YES" Then
                            clasjava.Showscripman(Result, UpdatePanel_parametros)
                            Exit Sub
                        End If
                        RefDato_Sql_Consulta = RefDato_Sql_Consulta.Replace(Darodoc, "'" & ValorParametro & "' ")
                    End If
                Next
                Result = ref.Resultado_consulta(sender.page, _
                                                RefDato_Sql_Consulta, _
                                                Matri_Nodo(1))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, UpdatePanel_parametros)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_parametros)
        End Try
    End Sub
    Function obtener_valor_parametro(ByVal Parametro As String, _
                                     ByRef Valor_Dato As String, _
                                     ByRef pag As Page) As String
        Dim clasjava As New Classscrripjava
        Try
            Dim i As Integer = 0
            Dim i2 As Integer = 0
            Dim RefCel As New TableCell
            Dim Reftextbox As New Object
            Dim texto_box As String = ""
            Reftextbox = Nothing
            Reftextbox = pag.FindControl(Parametro)
            If Reftextbox Is Nothing Then
                obtener_valor_parametro = "Imposible encontrar el control " & Parametro
                Exit Function
            Else
                Valor_Dato = Reftextbox.Text
                obtener_valor_parametro = "YES"
                Exit Function
            End If

        Catch ex As Exception
            obtener_valor_parametro = "Inconsistencia general función " & ex.Message
        End Try
    End Function

    Function Datos_Sql_Reporte(ByVal id_Reporte As String, _
                               ByRef Datos_Sql As String) As String
        Try
            Dim Parametro_Consulta = "select SQL_REPORTE FROM REPORTES_WORKFLOW WHERE " & _
            "ID_REPORTE =" & id_Reporte & " AND Estado_Reporte=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Datos_Sql_Reporte = "Imposible listar el reporte" & Result
                Exit Function
            End If
            If Datset Is Nothing Or Datset.Tables.Count = 0 Then
                Datos_Sql_Reporte = " Imposible listal tabla reportes tabla (0) "
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Datos_Sql_Reporte = "YES"
                Exit Function
            Else
                Datos_Sql = Trim(Datset.Tables(0).Rows(0).Item(0).ToString)
                Datos_Sql_Reporte = "YES"
                Exit Function
            End If
            Datos_Sql_Reporte = "YES"
        Catch ex As Exception
            Datos_Sql_Reporte = ex.Message
        End Try

    End Function
End Class
