Public Class Class_Lista_tramites_por_responder
    Function Lista_numero_tramites(ByVal Id_Usuario_Workflow As Integer, _
                                   ByVal Id_Ruta_Workflow As Integer, _
                                   ByVal Id_Grupo_Workflow As Integer, _
                                   ByVal Id_actividad As Integer, _
                                   ByVal nombre_ruta As String, _
                                   ByRef numero_documentos As Integer) As String
        Try
            Dim sql_consulta As String = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea from " & _
                                         " estados_tarea_workflow etw " & _
                                         " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " & _
                                         " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA  )" & _
                                         " where etw.id_actividad=" & Id_actividad & _
                                         " and etw.fecha_Seleccion is null and etw.fecha_fin is null and etw.id_usuario=" & Id_Usuario_Workflow & " and etw.estado_tarea=0 and estado_modulo_radicado = 1 "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_numero_tramites = "Error listando solicitudes relacionadas a un usuario " & Result
                Exit Function
            Else
                numero_documentos = Datset.Tables(0).Rows.Count
                Lista_numero_tramites = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_numero_tramites = "Inconsistencia general funcion Lista_numero_tramites " & ex.Message
        End Try
    End Function
    Function Lista_tramites(ByVal Id_Usuario_Workflow As Integer,
                            ByVal Id_Ruta_Workflow As Integer,
                            ByVal Id_Grupo_Workflow As Integer,
                            ByVal Id_actividad As Integer,
                            ByVal campo_lista_tramite As String,
                            ByVal nombre_ruta As String,
                            ByRef grediview As GridView,
                            ByRef HiddenEmailconsulta As Object,
                            ByRef reflabel As Label,
                            ByRef hideselecion As Object,
                            ByRef update As UpdatePanel,
                            ByVal estado_tramite As String,
                            ByRef update_title As UpdatePanel,
                            ByVal tipo_consulta As Integer,
                            ByVal valor_consulta As String,
                            ByRef colum_order_name As String,
                            ByRef order_colum As String,
                            ByRef Hidden_content As Object) As String
        Try
            HttpContext.Current.Session.Item("dat_gred_cahce_restore") = vbObject
            Dim filtro As String = ""
            If estado_tramite <> "" And estado_tramite <> "Todos" Then
                filtro = " and DAT.estado_tramite='" & estado_tramite & "'"
            End If
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_inicio,etw.estado_prioridad as prioridad," & campo_lista_tramite & ",ESTADO_TRAMITE AS ESTADO,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                               " estados_tarea_workflow etw " &
                               " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " &
                               " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & filtro & " )" &
                               " where etw.id_actividad=" & Id_actividad &
                               " and etw.fecha_Seleccion is null and etw.fecha_fin is null and etw.id_usuario=" & Id_Usuario_Workflow & " and etw.estado_tarea=0 and estado_modulo_radicado = 1 and ESTADO_ACTIVIDA_MODULO_RAD=0 " _
                                 & " order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 2 Then
                Dim sql_consulta_texto As String = ""
                Dim spli_campos() As String = campo_lista_tramite.Split(",")
                For i As Integer = 0 To spli_campos.Length - 1
                    If i = 0 Then
                        sql_consulta_texto = spli_campos(i) & " Like '%" & valor_consulta & "%'"
                    Else
                        sql_consulta_texto = sql_consulta_texto & " or " & spli_campos(i) & " Like '%" & valor_consulta & "%'"
                    End If
                Next
                sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_inicio,etw.estado_prioridad as prioridad," & campo_lista_tramite & ",ESTADO_TRAMITE AS ESTADO,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                              " estados_tarea_workflow etw " &
                              " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " &
                              " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & filtro & " ) " &
                             " where (" & sql_consulta_texto & ") " &
                              " and etw.id_actividad=" & Id_actividad &
                              " and etw.fecha_Seleccion is null and etw.fecha_fin is null and etw.id_usuario=" & Id_Usuario_Workflow & " and etw.estado_tarea=0 and estado_modulo_radicado = 1 and ESTADO_ACTIVIDA_MODULO_RAD=0 " _
                                & " order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 3 Then
                sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_inicio,etw.estado_prioridad as prioridad," & campo_lista_tramite & ",ESTADO_TRAMITE AS ESTADO,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                             " estados_tarea_workflow etw " &
                             " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " &
                             " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & filtro & " ) " &
                            " where (" & valor_consulta & ") " &
                             " and etw.id_actividad=" & Id_actividad &
                             " and etw.fecha_Seleccion is null and etw.fecha_fin is null and etw.id_usuario=" & Id_Usuario_Workflow & " and etw.estado_tarea=0 and estado_modulo_radicado = 1 and ESTADO_ACTIVIDA_MODULO_RAD=0 " _
                               & " order by " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO") = valor_consulta
            HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE") = campo_lista_tramite
            Dim spli_colum() As String = {"OPCIONES", "id_tarea", "fecha_inicio", "prioridad"}
            Dim spli_campos_() As String = campo_lista_tramite.Split(",")
            Dim leng_split As Integer = spli_campos_.Length
            leng_split = leng_split + 4
            Dim estado_solicitud_aprobacion As Integer = 0
            For i As Integer = 0 To spli_campos_.Length - 1
                Dim nuevo_indice As Integer = spli_colum.Length
                ReDim Preserve spli_colum(nuevo_indice)
                spli_colum(nuevo_indice) = spli_campos_(i)
            Next
            Dim nuevo_indice_ As Integer = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "ESTADO"
            nuevo_indice_ = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FECHAINICIOTRAMITE"
            nuevo_indice_ = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FECHAFINALTRAMITE"
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido_hi") = spli_colum
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_tramites = "Error listando solicitudes relacionadas a un usuario " & Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("dat_gred_cahce") = Datset
            HttpContext.Current.Session.Item("WF_NUMERO_TRAMITE_ASIGNADO") = Datset.Tables(0).Rows.Count
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                Hidden_content.value = Datset.Tables(0).Rows.Count
                If tipo_consulta = 1 Then
                    reflabel.Text = "0 registro(s) "
                Else
                    reflabel.Text = "0 registro(s) "
                End If
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                hideselecion.value = "-1"
                update.Update()
                update_title.Update()
                Lista_tramites = "YES"
                Exit Function
            Else
                Hidden_content.value = Datset.Tables(0).Rows.Count
                If tipo_consulta = 1 Then
                    reflabel.Text = Datset.Tables(0).Rows.Count & " registro(s) "
                Else
                    reflabel.Text = Datset.Tables(0).Rows.Count & "  registro(s) "
                End If
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Result = pluguin_lista_tramites(grediview,
                                                colum_order_name,
                                                order_colum,
                                                estado_solicitud_aprobacion,
                                                leng_split)
                If Result <> "YES" Then
                    Lista_tramites = Result
                    Exit Function
                Else
                    Lista_tramites = "YES"
                    Exit Function
                End If

            End If
        Catch ex As Exception
            Lista_tramites = "Inconsistencia general función Lista_tramites " & ex.Message
        End Try
    End Function
    Function Cahche_pagin_sorting_lista_tramites(ByRef grediview As GridView,
                                                 ByRef update_gred As UpdatePanel,
                                                 ByVal valida_sort As Integer,
                                                 ByVal colum_order_name As String,
                                                 ByVal order_colum As String) As String
        Try
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            Datset = HttpContext.Current.Session.Item("dat_gred_cahce")
            Dim spli_campos_() As String = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE").Split(",")
            Dim leng_split As Integer = spli_campos_.Length
            leng_split = leng_split + 4
            Dim estado_solicitud_aprobacion As Integer = 0
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName,
                                                   Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                update_gred.Update()
                Cahche_pagin_sorting_lista_tramites = "YES"
                Exit Function
            Else
                If valida_sort = 1 Then
                    Dim dtTable As DataTable = New DataTable()
                    dtTable = Datset.Tables(0)
                    Dim dv As DataView = dtTable.DefaultView
                    dv.Sort = colum_order_name + " " + order_colum
                    Dim dtSorted As DataTable = New DataTable()
                    dtSorted = dv.ToTable()
                    grediview.DataSource = dtSorted
                    grediview.DataBind()
                    update_gred.Update()
                    Datset.Tables.Clear()
                    Datset.Tables.Add(dtSorted)
                    HttpContext.Current.Session.Item("dat_gred_cahce") = Datset
                Else
                    grediview.DataSource = Datset
                    grediview.DataBind()
                    update_gred.Update()
                End If
                Result = pluguin_lista_tramites(grediview,
                                                colum_order_name,
                                                order_colum,
                                                estado_solicitud_aprobacion,
                                                leng_split)
                If Result <> "YES" Then
                    Cahche_pagin_sorting_lista_tramites = Result
                    Exit Function
                Else

                    Cahche_pagin_sorting_lista_tramites = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Cahche_pagin_sorting_lista_tramites = "Inconsistencia general funcion Cahche_pagin_sorting_lista_tramites " & ex.Message
        End Try
    End Function
    Function Cache_filtra_tramites(ByRef grediview As GridView,
                                   ByRef update_gred As UpdatePanel,
                                   ByVal valida_sort As Integer,
                                   ByVal colum_order_name As String,
                                   ByVal order_colum As String,
                                   ByVal valor As String,
                                   ByRef label As Label,
                                   ByRef update_title As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Datset_filtro As DataSet = New DataSet("estados_tarea_workflow_")
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                HttpContext.Current.Session.Item("dat_gred_cahce") = HttpContext.Current.Session.Item("dat_gred_cahce_restore")
                Datset = HttpContext.Current.Session.Item("dat_gred_cahce")
            Else
                HttpContext.Current.Session.Item("dat_gred_cahce_restore") = HttpContext.Current.Session.Item("dat_gred_cahce")
                Datset = HttpContext.Current.Session.Item("dat_gred_cahce")
            End If
            Dim spli_campos_() As String = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE").Split(",")
            Dim leng_split As Integer = spli_campos_.Length
            leng_split = leng_split + 4
            Dim estado_solicitud_aprobacion As Integer = 0
            Datset_filtro.Tables.Add("cahce_estados_tarea_workflow")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Datset_filtro.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                If valor <> "Todos" Then
                    If Datset.Tables(0).Rows(i).Item("ESTADO") = valor Then
                        Datset_filtro.Tables(0).ImportRow(Datset.Tables(0).Rows(i))
                    End If
                Else
                    Datset_filtro.Tables(0).ImportRow(Datset.Tables(0).Rows(i))
                End If
            Next
            HttpContext.Current.Session.Item("dat_gred_cahce") = Datset_filtro
            If Datset_filtro.Tables(0).Rows.Count = 0 Then
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                label.Text = "0 registro(s) "
                update_title.Update()
                update_gred.Update()
                Cache_filtra_tramites = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset_filtro
                grediview.DataBind()
                update_gred.Update()
                label.Text = Datset_filtro.Tables(0).Rows.Count & " registro(s) "
                update_title.Update()
                Result = pluguin_lista_tramites(grediview,
                                                colum_order_name,
                                                order_colum,
                                                estado_solicitud_aprobacion,
                                                leng_split)
                If Result <> "YES" Then
                    Cache_filtra_tramites = Result
                    Exit Function
                Else
                    Cache_filtra_tramites = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Cache_filtra_tramites = "Inconsistencia general funcion Cache_filtra_tramites " & ex.Message
        End Try
    End Function
    Function Cache_lista_tramites(ByRef grediview As GridView,
                                  ByRef update_gred As UpdatePanel,
                                  ByVal valida_sort As Integer,
                                  ByVal colum_order_name As String,
                                  ByVal order_colum As String,
                                  ByRef label As Label,
                                  ByRef update_title As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Datset_ As DataSet = New DataSet("estados_tarea_workflow_")
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                HttpContext.Current.Session.Item("dat_gred_cahce") = HttpContext.Current.Session.Item("dat_gred_cahce_restore")
                HttpContext.Current.Session.Item("dat_gred_cahce_restore") = vbObject
            End If
            Datset = HttpContext.Current.Session.Item("dat_gred_cahce")
            Dim spli_campos_() As String = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE").Split(",")
            Dim leng_split As Integer = spli_campos_.Length
            leng_split = leng_split + 4
            Dim estado_solicitud_aprobacion As Integer = 0
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                label.Text = "0 registro(s) "
                update_title.Update()
                Cache_lista_tramites = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset
                grediview.DataBind()
                update_gred.Update()
                label.Text = Datset.Tables(0).Rows.Count & " registro(s) "
                update_title.Update()
                HttpContext.Current.Session.Item("dat_gred_cahce") = Datset
                Result = pluguin_lista_tramites(grediview,
                                                colum_order_name,
                                                order_colum,
                                                estado_solicitud_aprobacion,
                                                leng_split)
                If Result <> "YES" Then
                    Cache_lista_tramites = Result
                    Exit Function
                Else
                    Cache_lista_tramites = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Cache_lista_tramites = "Inconsistencia general funcion Cache_lista_tramites " & ex.Message
        End Try
    End Function
    Function Cahche_Search_lista_tramites(ByRef grediview As GridView,
                                          ByRef update_gred As UpdatePanel,
                                          ByVal valida_sort As Integer,
                                          ByVal colum_order_name As String,
                                          ByVal order_colum As String,
                                          ByVal valor As String,
                                          ByRef label As Label,
                                          ByRef update_title As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Datset_filtro As DataSet = New DataSet("estados_tarea_workflow_")
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                HttpContext.Current.Session.Item("dat_gred_cahce") = HttpContext.Current.Session.Item("dat_gred_cahce_restore")
                HttpContext.Current.Session.Item("dat_gred_cahce_restore") = vbObject
            End If
            Datset = HttpContext.Current.Session.Item("dat_gred_cahce")
            Dim spli_campos_() As String = HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE").Split(",")
            Dim leng_split As Integer = spli_campos_.Length
            leng_split = leng_split + 4
            Dim estado_solicitud_aprobacion As Integer = 0
            Datset_filtro.Tables.Add("cahce_estados_tarea_workflow")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Datset_filtro.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    If UCase(valor.ToString) = UCase(Datset.Tables(0).Rows(i).Item(z).ToString) Then
                        Datset_filtro.Tables(0).ImportRow(Datset.Tables(0).Rows(i))
                    End If
                Next
            Next
            If Datset_filtro.Tables(0).Rows.Count = 0 Then
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                update_gred.Update()
                label.Text = "(0)"
                update_title.Update()
                HttpContext.Current.Session.Item("dat_gred_cahce_restore") = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = Datset_filtro
                Cahche_Search_lista_tramites = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset_filtro
                grediview.DataBind()
                update_gred.Update()
                label.Text = "(" & Datset_filtro.Tables(0).Rows.Count & ")"
                update_title.Update()
                HttpContext.Current.Session.Item("dat_gred_cahce_restore") = HttpContext.Current.Session.Item("dat_gred_cahce")
                HttpContext.Current.Session.Item("dat_gred_cahce") = Datset_filtro
                Result = pluguin_lista_tramites(grediview,
                                                colum_order_name,
                                                order_colum,
                                                estado_solicitud_aprobacion,
                                                leng_split)
                Cahche_Search_lista_tramites = Result
                Exit Function
            End If
        Catch ex As Exception
            Cahche_Search_lista_tramites = "Inconsistencia general funcion Cahche_Search_lista_tramites " & ex.Message
        End Try
    End Function
    Function pluguin_lista_tramites(ByRef grediview As GridView,
                                    ByVal colum_order_name As String,
                                    ByVal orden As String,
                                    ByVal estado_solicitud_aprobacion As String,
                                    ByVal leng_split As Integer) As String
        Try
            Dim Result As String = ""
            For i As Integer = 0 To grediview.Rows.Count - 1
                grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                ihtml.Style.Add("color", "white")
                ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg")
                Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                ahtml.Attributes.Add("onclick", "prevent(Event,this);")
                ahtml.Attributes.Add("title", "Ver documentos")
                ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                ahtml.Attributes.Add("tip_event", "documento_solic_tramite")
                ahtml.Style.Add("margin-left", "3px")
                ahtml.Controls.Add(ihtml)
                divhtml.Controls.Add(ahtml)

                ihtml = New HtmlControls.HtmlGenericControl("i")
                ihtml.Style.Add("color", "white")
                ihtml.Attributes.Add("class", "fal fal fa-reply fa-lg")
                ahtml = New HtmlControls.HtmlGenericControl("a")
                ahtml.Attributes.Add("Class", "btn  btn-primary  btn-sm")
                ahtml.Attributes.Add("onclick", "prevent(Event,this);")
                ahtml.Attributes.Add("title", "Responder")
                ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                ahtml.Attributes.Add("tip_event", "responder_solic_tramite")
                ahtml.Style.Add("margin-left", "3px")
                ahtml.Controls.Add(ihtml)
                divhtml.Controls.Add(ahtml)
                ihtml = New HtmlControls.HtmlGenericControl("i")
                ihtml.Style.Add("color", "white")
                ihtml.Attributes.Add("class", "fal fa-user fa-lg ")
                ahtml = New HtmlControls.HtmlGenericControl("a")
                ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                ahtml.Attributes.Add("onclick", "prevent(Event,this);")
                ahtml.Attributes.Add("title", "Reasignar tramite")
                ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                ahtml.Attributes.Add("tip_event", "reasignar_solic_tramite")
                ahtml.Style.Add("margin-left", "3px")
                ahtml.Controls.Add(ihtml)
                divhtml.Controls.Add(ahtml)
                ihtml = New HtmlControls.HtmlGenericControl("i")
                ihtml.Style.Add("color", "white")
                ihtml.Attributes.Add("class", "fal fa-archive fa-lg ")
                ahtml = New HtmlControls.HtmlGenericControl("a")
                ahtml.Attributes.Add("Class", "btn btn-warning btn-sm")
                ahtml.Attributes.Add("onclick", "prevent(Event,this);")
                ahtml.Attributes.Add("title", "Archivar tramite")
                ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                ahtml.Attributes.Add("tip_event", "archiva_solic_tramite")
                ahtml.Style.Add("margin-left", "3px")
                ahtml.Controls.Add(ihtml)
                divhtml.Controls.Add(ahtml)

                ihtml = New HtmlControls.HtmlGenericControl("i")
                ihtml.Style.Add("color", "white")
                ihtml.Attributes.Add("class", "fal fa-external-link fa-lg")
                ahtml = New HtmlControls.HtmlGenericControl("a")
                ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                ahtml.Attributes.Add("onclick", "prevent(Event,this);")
                ahtml.Attributes.Add("title", "Finalizar tramite")
                ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                ahtml.Attributes.Add("tip_event", "finaliza_solic_tramite")
                ahtml.Style.Add("margin-left", "3px")
                ahtml.Controls.Add(ihtml)
                divhtml.Controls.Add(ahtml)

                ihtml = New HtmlControls.HtmlGenericControl("i")
                ihtml.Style.Add("color", "white")
                ihtml.Attributes.Add("class", "fal fa-sticky-note fa-lg")
                ahtml = New HtmlControls.HtmlGenericControl("a")
                ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                ahtml.Attributes.Add("onclick", "prevent(Event,this);")
                ahtml.Attributes.Add("title", "Notas")
                ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                ahtml.Attributes.Add("tip_event", "nota_solic_tramite")
                ahtml.Style.Add("margin-left", "3px")
                ahtml.Controls.Add(ihtml)
                divhtml.Controls.Add(ahtml)
                divhtml.Style.Add("display", "inline-flex")
                grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                If grediview.Rows(i).Cells(leng_split).Text = "Por tramitar" Then
                    grediview.Rows(i).Attributes.Add("Class", "font-weight-bold")
                Else
                    'grediview.Rows(i).Attributes.Add("Class", "font-weight-normal")
                End If
                If grediview.Rows(i).Cells(leng_split).Text <> "Por tramitar" And grediview.Rows(i).Cells(leng_split).Text <> "En tramite" _
                And grediview.Rows(i).Cells(leng_split).Text <> "Tramitado" And grediview.Rows(i).Cells(leng_split).Text <> "Tramitado archivado" Then
                    'estado_solicitud_aprobacion = 1
                    Dim ahtml_ As New HtmlControls.HtmlGenericControl("a")
                    If grediview.Rows(i).Cells(leng_split).Text = "Solicitud por aprobaci&#243;n" Then
                        ahtml_.Style.Add("color", "white")
                        ahtml_.Attributes.Add("Class", "btn_wrrap btn-primary btn-sm")
                    End If
                    If grediview.Rows(i).Cells(leng_split).Text = "Aprobada" Then
                        ahtml_.Style.Add("color", "white")
                        ahtml_.Attributes.Add("Class", "btn_wrrap btn-success btn-sm")
                    End If
                    If grediview.Rows(i).Cells(leng_split).Text = "Solicitud desaprobada" Then
                        ahtml_.Style.Add("color", "white")
                        ahtml_.Attributes.Add("Class", "btn_wrrap btn-danger btn-sm")
                    End If
                    If grediview.Rows(i).Cells(leng_split).Text = "Solicitud archivada" Then
                        ahtml_.Style.Add("color", "black")
                        ahtml_.Attributes.Add("Class", "btn_wrrap btn-warning btn-sm")
                    End If
                    If grediview.Rows(i).Cells(leng_split).Text = "Solicitud anulada" Then
                        ahtml_.Style.Add("color", "white")
                        ahtml_.Attributes.Add("Class", "btn_wrrap btn-warning btn-sm")
                    End If
                    ahtml_.Attributes.Add("onclick", "prevent(Event,this);")
                    ahtml_.Attributes.Add("title", "Lista solictudes de aprobación de respuesta")
                    ahtml_.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml_.Attributes.Add("tip_event", "lista_solic_tramite")
                    ahtml_.Style.Add("margin-left", "3px")
                    ahtml_.InnerHtml = grediview.Rows(i).Cells(leng_split).Text
                    grediview.Rows(i).Cells(leng_split).Controls.Add(ahtml_)
                End If
                For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                    If z > 0 Then
                        grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                        If z = leng_split And estado_solicitud_aprobacion = 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(Event,this);")
                        End If
                    End If
                Next
            Next
            Dim Refclas As New ClassGredview
            Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                        HttpContext.Current.Session.Item("Sort_matri_colum_compartido_hi"),
                                                        orden,
                                                        grediview)
            If Result <> "YES" Then
                pluguin_lista_tramites = "Error add clase funcion  pluguin_lista_tramite " & Result
                Exit Function
            Else
                pluguin_lista_tramites = "YES"
                Exit Function
            End If
        Catch ex As Exception
            pluguin_lista_tramites = "Inconsistencia general funcion pluguin_lista_tramites " & ex.Message
        End Try
    End Function
    Function Consulta_tramites_historico(ByVal Id_Usuario_Workflow As Integer, _
                                         ByVal Id_Ruta_Workflow As Integer, _
                                         ByVal Id_Grupo_Workflow As Integer, _
                                         ByVal Id_actividad As Integer, _
                                         ByVal campo_lista_tramite As String, _
                                         ByVal nombre_ruta As String, _
                                         ByRef grediview As GridView, _
                                         ByRef HiddenEmailconsulta As Object, _
                                         ByRef reflabel As Label, _
                                         ByRef hideselecion As Object, _
                                         ByRef update As UpdatePanel, _
                                         ByVal estado_tramite As String, _
                                         ByRef update_title As UpdatePanel, _
                                         ByVal tipo_consulta As Integer, _
                                         ByVal valor_consulta As String, _
                                         ByRef colum_order_name As String, _
                                         ByRef order_colum As String, _
                                         ByRef Hidden_content As Object) As String
        Try
            Dim filtro As String = ""
            If estado_tramite <> "" And estado_tramite <> "Todos" Then
                filtro = " And DAT.estado_tramite='" & estado_tramite & "'"
            End If
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_inicio,etw.estado_prioridad as prioridad," & campo_lista_tramite & ",ESTADO_TRAMITE AS ESTADO,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " & _
                               " estados_tarea_workflow etw " & _
                               " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " & _
                               " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & filtro & " )" & _
                               " where etw.id_actividad=" & Id_actividad & _
                               "  and etw.id_usuario=" & Id_Usuario_Workflow & "  " _
                                 & " order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 2 Then
                Dim sql_consulta_texto As String = ""
                Dim spli_campos() As String = campo_lista_tramite.Split(",")
                For i As Integer = 0 To spli_campos.Length - 1
                    If i = 0 Then
                        sql_consulta_texto = spli_campos(i) & " Like '%" & valor_consulta & "%'"
                    Else
                        sql_consulta_texto = sql_consulta_texto & " or " & spli_campos(i) & " Like '%" & valor_consulta & "%'"
                    End If
                Next
                sql_consulta = "Select distinct etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_inicio,etw.estado_prioridad as prioridad," & campo_lista_tramite & ",ESTADO_TRAMITE AS ESTADO,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " & _
                              " estados_tarea_workflow etw " & _
                              " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " & _
                              " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & filtro & " ) " & _
                             " where (" & sql_consulta_texto & ") " & _
                              " and etw.id_actividad=" & Id_actividad & _
                              "  and etw.id_usuario=" & Id_Usuario_Workflow & "  " _
                                & " order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 3 Then
                sql_consulta = "Select distinct etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_inicio,etw.estado_prioridad as prioridad," & campo_lista_tramite & ",ESTADO_TRAMITE AS ESTADO,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " & _
                              " estados_tarea_workflow etw " & _
                              " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " & _
                              " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & filtro & " ) " & _
                             " where (" & valor_consulta & ") " & _
                              " and etw.id_actividad=" & Id_actividad & _
                              "  and etw.id_usuario=" & Id_Usuario_Workflow & "  " _
                                & " order by " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_HI") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_HI") = valor_consulta
            HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI") = campo_lista_tramite
            Dim spli_colum() As String = {"OPCIONES", "id_tarea", "fecha_inicio", "prioridad"}
            'Dim spli_colum() As String = campo_lista_tramite.Split(",")
            Dim spli_campos_() As String = campo_lista_tramite.Split(",")
            For i As Integer = 0 To spli_campos_.Length - 1
                Dim nuevo_indice As Integer = spli_colum.Length
                ReDim Preserve spli_colum(nuevo_indice)
                spli_colum(nuevo_indice) = spli_campos_(i)
            Next
            Dim nuevo_indice_ As Integer = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "ESTADO"
            nuevo_indice_ = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FECHAINICIOTRAMITE"
            nuevo_indice_ = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FECHAFINALTRAMITE"
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido_hi") = spli_colum
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_tramites_historico = "Error listando solicitudes historico " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HiddenEmailconsulta.value = ""
                Hidden_content.value = Datset.Tables(0).Rows.Count
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron 0 registro(s) "
                Else
                    reflabel.Text = "Se encontraron 0 registro(s) "
                End If
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Consulta_tramites_historico = "YES"
                Exit Function
            Else
                Hidden_content.value = Datset.Tables(0).Rows.Count
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                Else
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & "  registro(s) "
                End If
                'reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de documentos compartidos"
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "documento_solic_tramite")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-chart-network fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Muestra trazabilidad del tramite")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "trasa_solic_tramite")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-th-list fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Detalle del tramite")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "detalle_solic_tramite")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-bars fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-warning btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Transacciones del tramite")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "transac_solic_tramite")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")

                        End If

                    Next
                Next
                HttpContext.Current.Session.Item("dat_gred_cahce_hi") = Datset
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name, _
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_compartido_hi"), _
                                                            order_colum, _
                                                            grediview)
                If Result <> "YES" Then
                    Consulta_tramites_historico = "Error add clase funcion  Consulta_tramites_historico " & Result
                    Exit Function
                End If
                Consulta_tramites_historico = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_tramites_historico = "Inconsistencia general función Consulta_tramites_historico " & ex.Message
        End Try
    End Function

    Function Solicita_parametro_consulta_filtro(ByVal fecha_ini_asignacion As String, _
                                                ByVal fecha_fin_asignacion As String, _
                                                ByVal fecha_ini_terminacion As String, _
                                                ByVal fecha_fin_terminacion As String, _
                                                ByRef value_consulta As String) As String
        Try
            value_consulta = ""
            If fecha_ini_asignacion <> "" And fecha_fin_asignacion <> "" Then
                value_consulta = "CAST(Fecha_Inicio AS DATE) BETWEEN '" & fecha_ini_asignacion & "' AND '" & _
                    fecha_fin_asignacion & "'"
            Else
                If fecha_ini_asignacion <> "" Then
                    value_consulta = "CAST(Fecha_Inicio AS DATE) = '" & fecha_ini_asignacion & "'"
                End If
                If fecha_fin_asignacion <> "" Then
                    value_consulta = "CAST(Fecha_Inicio AS DATE) = '" & fecha_fin_asignacion & "'"
                End If
            End If
            If fecha_ini_terminacion <> "" And fecha_fin_terminacion <> "" Then
                If value_consulta <> "" Then
                    value_consulta = " and " & value_consulta
                End If
                value_consulta = "CAST(Fecha_Fin AS DATE) BETWEEN '" & fecha_ini_terminacion & "' AND '" & _
                    fecha_fin_terminacion & "'"
            Else
                If fecha_ini_terminacion <> "" Then
                    If value_consulta <> "" Then
                        value_consulta = " and " & value_consulta
                    End If
                    value_consulta = "CAST(Fecha_Fin AS DATE) = '" & fecha_ini_terminacion & "'"
                End If
                If fecha_fin_terminacion <> "" Then
                    If value_consulta <> "" Then
                        value_consulta = " and " & value_consulta
                    End If
                    value_consulta = "CAST(Fecha_Fin AS DATE) = '" & fecha_fin_terminacion & "'"
                End If
            End If
            Solicita_parametro_consulta_filtro = "YES"
        Catch ex As Exception
            Solicita_parametro_consulta_filtro = "Incosnistencia general función Solicita_parametro_consulta_filtro " & ex.Message
        End Try
    End Function
End Class
