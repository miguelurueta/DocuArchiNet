Imports Dynamsoft.DotNet.TWAIN.Barcode
Imports GestionDocumental_Docuarchi.net.Class_config_general_service
Imports iTextSharp.text
Imports iTextSharp.text.pdf.fonts

Public Class Class_ra_log_respuesta_radicado
    Function Registra_gestion_respuesta(ByVal Class_config_general_service_ As List(Of Class_config_general_service)) As String
        Try
            Dim sql_insert As String = ""
            Dim Result As String = ""
            Dim estado_obligatorio_respuesta As Integer = 0
            Dim id_tipo_tramite As Integer = 0
            Dim descripcion_tramite As String = ""
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    Radicado)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Dim id_respuesta As Integer = 0
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                               HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                               id_respuesta)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim date1al As String = ""
            Result = ""
            Result = refclas_gestion_fechas.Formatea_fecha_time_db(Date.Now,
                                                               date1al)
            If Result <> "YES" Then
                Registra_gestion_respuesta = "Imposible formatear fecha " & Result
                Exit Function
            End If
            Dim Class_config_general_service As New Class_config_general_service
            Result = Class_config_general_service.add_campo_form_control("ID_RESPUESTA_RADICADO", "ID_RESPUESTA_RADICADO", "", id_respuesta, id_respuesta, 0, 1, "0", 0, 1, Class_config_general_service_)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If

            Result = Class_config_general_service.add_campo_form_control("desc_op", "desc_op", "", "GESTION TRAMITE", "GESTION TRAMITE", 0, 1, "0", 0, 1, Class_config_general_service_)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Result = Class_config_general_service.add_campo_form_control("USER_OPER", "USER_OPER", "", HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), 0, 1, "0", 0, 1, Class_config_general_service_)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Result = Class_config_general_service.add_campo_form_control("ID_USER", "ID_USER", "", HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), 0, 1, "0", 0, 1, Class_config_general_service_)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Result = Class_config_general_service.add_campo_form_control("DATE_TRANS", "DATE_TRANS", "", date1al, date1al, 0, 1, "0", 0, 1, Class_config_general_service_)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Result = Class_config_general_service.add_campo_form_control("IP_TRANS", "IP_TRANS", "", HttpContext.Current.Session.Item("ip_host_name"), HttpContext.Current.Session.Item("ip_host_name"), 0, 1, "0", 0, 1, Class_config_general_service_)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Dim datehora As String = Date.Now.Hour
            Result = Class_config_general_service.add_campo_form_control("HORA_REGISTRO", "HORA_REGISTRO", "", date1al, date1al, 0, 1, "0", 0, 1, Class_config_general_service_)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Result = Class_config_general_service.add_campo_form_control("MODULO_REGISTRO", "MODULO_REGISTRO", "", "GESTION CORRESPONDENCIA", "GESTION CORRESPONDENCIA", 0, 1, "0", 0, 1, Class_config_general_service_)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Result = Class_config_general_service.Create_insert_form_control("ra_log_respuesta_radicado",
                                                                             Class_config_general_service_,
                                                                             sql_insert)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Registra_gestion_respuesta = Result
                Exit Function
            End If
            Registra_gestion_respuesta = "YES"
            Exit Function
        Catch ex As Exception
            Registra_gestion_respuesta = "Inconsistencia general funcion Registra_gestion_respuesta " & ex.Message
        End Try
    End Function
    Function Lista_gestion_solicitud(ByVal id_usuario_gestion As Integer,
                                     ByVal id_respuesta As Integer,
                                     ByVal tipo_consulta As Integer,
                                     ByVal valor_consulta As String,
                                     ByRef colum_order_name As String,
                                     ByRef order_colum As String,
                                     ByRef labetitle As Label,
                                     ByRef scripma As GridView,
                                     ByRef hideselecion As HtmlInputHidden,
                                     ByRef updat As UpdatePanel) As String
        Try
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT id_tran,CAMPOS AS GESTION,HORA_REGISTRO AS FECHA" &
                    " from ra_log_respuesta_radicado " &
                    " where ID_USER=" & id_usuario_gestion & " and ID_RESPUESTA_RADICADO=" & id_respuesta

            Else
                sql_consulta = "SELECT id_tran,CAMPOS AS GESTION,HORA_REGISTRO AS FECHA" &
                    " from ra_log_respuesta_radicado " &
                    " where (" &
                    "  CAMPOS like '%" & valor_consulta & "%')" &
                    " and ID_USER=" & id_usuario_gestion & " and ID_RESPUESTA_RADICADO=" & id_respuesta
            End If
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido") = {"OPCIONES", "id_tran,CAMPOS",
                                                                               "GESTION", "FECHA"}
            HttpContext.Current.Session.Item("SortExpression_publico") = colum_order_name
            HttpContext.Current.Session.Item("SortDirection_publico") = order_colum
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_PUBLICO") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO") = sql_consulta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_log_respuesta_radicado")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_gestion_solicitud = "Error fucion Lista_gestion_solicitud  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) "
                scripma.DataSource = Datset
                hideselecion.Value = "-1"
                scripma.DataBind()
                updat.Update()
                Lista_gestion_solicitud = "YES"
                Exit Function
            Else
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s)  "
                scripma.DataSource = Datset
                hideselecion.Value = "-1"
                scripma.DataBind()
                updat.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-edit")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_lista_gestion(event,this);")
                    ahtml.Attributes.Add("title", "Editar gestión")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "e_g_r_s")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml)
                    'Agrega eliminar registro
                    divhtml = New HtmlControls.HtmlGenericControl("div")
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-trash-alt")
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_lista_gestion(event,this);")
                    ahtml.Attributes.Add("title", "Eliminar gestión")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "d_g_r_s")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_compartido"),
                                                            order_colum,
                                                            scripma)
                If Result <> "YES" Then
                    Lista_gestion_solicitud = "Error add clase funcion  Lista_gestion_solicitud " & Result
                    Exit Function
                End If
            End If
            Lista_gestion_solicitud = "YES"
        Catch ex As Exception
            Lista_gestion_solicitud = "Inconsistencia general función Lista_gestion_solicitud " & ex.Message
        End Try
    End Function
    Function Elimina_registro_gestion_solicitud(ByVal Class_config_general_service_delete As List(Of Class_config_general_service_delete)) As String
        Try
            Dim sql_delete As String = ""
            Dim table As String = "ra_log_respuesta_radicado"
            Dim Class_config_general_service As New Class_config_general_service
            Dim Result As String = ""
            Result = Class_config_general_service.Create_delete_form_control(table, Class_config_general_service_delete, sql_delete)
            If Result <> "YES" Then
                Elimina_registro_gestion_solicitud = Result
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(sql_delete)
            If Result <> "YES" Then
                Elimina_registro_gestion_solicitud = Result
                Exit Function
            End If
            Elimina_registro_gestion_solicitud = "YES"
            Exit Function
        Catch ex As Exception
            Elimina_registro_gestion_solicitud = "Inconsistencia general funcion Elimina_registro_gestion_solicitud " & ex.Message
        End Try
    End Function
    Function Solicita_datos_gestion_solicitud(ByVal id_gestion As Integer,
                                              ByRef Class_config_general_service As List(Of Class_config_general_service)) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_log_respuesta_radicado")
            Dim sql_consulta As String = "select CAMPOS from ra_log_respuesta_radicado where id_tran=" & id_gestion
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_gestion_solicitud = "Error fucion Lista_gestion_solicitud  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                Else
                    parameter_gestion.value_campo = Datset.Tables(0).Rows(0).Item(0)
                End If
                parameter_gestion.name_campo = "CAMPOS"
                parameter_gestion.error_gestion = "YES"
                Class_config_general_service.Add(parameter_gestion)
                Solicita_datos_gestion_solicitud = "YES"
            Else
                Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
                parameter_gestion.error_gestion = Result
                Class_config_general_service.Add(parameter_gestion)
                Solicita_datos_gestion_solicitud = Result
            End If
        Catch ex As Exception
            Solicita_datos_gestion_solicitud = "Inconsistencia general función Solicita_datos_gestion_solicitud " & ex.Message
        End Try
    End Function
    Function Actualiza_datos_gestion_solicitud(ByVal id_gestion As Integer,
                                               ByVal Class_config_general_service As List(Of Class_config_general_service)) As String
        Try
            Dim Result As String = ""
            Dim Class_config_general_service_ As New Class_config_general_service
            Dim sql_update As String = ""
            Result = Class_config_general_service_.Create_update_form_control("ra_log_respuesta_radicado", Class_config_general_service, "id_tran", id_gestion, sql_update)
            If Result <> "YES" Then
                Actualiza_datos_gestion_solicitud = Result
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Actualiza_datos_gestion_solicitud = Result
                Exit Function
            End If
            Actualiza_datos_gestion_solicitud = "YES"
            Exit Function
        Catch ex As Exception
            Actualiza_datos_gestion_solicitud = "Inconsistencia general función Actualiza_datos_gestion_solicitud " & ex.Message
        End Try
    End Function
    Function Retorna_log_respuesta_radicado(ByVal page1 As Page,
                                           ByVal id_respuesta_radicado As Integer) As String
        Try
            Dim Result As String = ""
            Dim Nombre_ruta As String = ""
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID_VAL")
            If scripma Is Nothing Then
                Retorna_log_respuesta_radicado = "Imposible encontrar datagrid  " & "GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Retorna_log_respuesta_radicado = "Imposible encontrar el control  " & "titulo_label_val_radicacion"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Retorna_log_respuesta_radicado = "Imposible encontrar el control  " & "UpdatePanelabel_val_radicacion"
                Exit Function
            End If

            Dim Sql_consulta As String = "Select desc_op as DESCRIPCION_OPERACION,USER_OPER AS USUARIO_TRANSACCION," &
                "HORA_REGISTRO AS FECHA_REGISTRO,CAMPOS AS DETALLE_TRANSACCION,IP_TRANS AS DIRECCION_TRANSACCION,MODULO_REGISTRO,SEND_CORREO_ELECTRONICO FROM ra_log_respuesta_radicado " &
                " where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado & " ORDER BY id_tran"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("radicado")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_log_respuesta_radicado = "Error listando datos funcion Retorna_log_respuesta_radicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontraron (" & Datset.Tables(0).Rows.Count & ") registro(s)  "
                scripma.DataSource = Datset
                hideselecion.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Retorna_log_respuesta_radicado = "YES"
                Exit Function
            Else
                labetitle.Text = "Se encontraron (" & Datset.Tables(0).Rows.Count & ") registro(s)  "
                scripma.DataSource = Datset
                hideselecion.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", i)
                Next
                Retorna_log_respuesta_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_log_respuesta_radicado = "Inconsistencia general función Retorna_log_respuesta_radicado " & ex.Message
        End Try
    End Function
End Class
