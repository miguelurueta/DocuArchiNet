Imports System.Drawing
Imports AjaxControlToolkit

Public Class ClassConsultaGabinete
    Function Limpiar_campos_consulta(ByRef page1 As Page,
                                     ByVal NombreGabi As String,
                                     ByVal tipo_consulta As Integer) As String
        Try
            Dim updatetable As UpdatePanel = page1.FindControl("UpdatePanel_consulta")
            Dim ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim stru_campo_detalle() As stru_campo_detalle = Nothing
            Dim Result As String = ""
            If tipo_consulta = 1 Then
                Result = ref_Class_DETALLE_GABIENETE.SolicitaDetalleCamposGabinete(NombreGabi,
                                                                                      stru_campo_detalle)
                If Result <> "YES" Then
                    Limpiar_campos_consulta = "Funcion  Generando_Consulta_Gabinete WF-03 Mensaje DBMS" & Result
                    Exit Function
                End If
                If stru_campo_detalle Is Nothing Then
                    Limpiar_campos_consulta = "Imposible encontrar los campos para gabinete : " & NombreGabi
                    Exit Function
                End If
            Else
                Result = ref_Class_DETALLE_GABIENETE.Solicita_detalle_campos_gabinete_publico(NombreGabi,
                                                                                              stru_campo_detalle)
                If Result <> "YES" Then
                    Limpiar_campos_consulta = "Funcion  Generando_Consulta_Gabinete WF-03 Mensaje DBMS" & Result
                    Exit Function
                End If
                If stru_campo_detalle Is Nothing Then
                    Limpiar_campos_consulta = "Imposible encontrar los campos para gabinete : " & NombreGabi
                    Exit Function
                End If
            End If

            For i As Integer = 0 To stru_campo_detalle.Length - 1
                If stru_campo_detalle(i).nombre_campo = "DATE" Or stru_campo_detalle(i).nombre_campo = "INT" Then
                    Dim obj As TextBox = page1.FindControl(stru_campo_detalle(i).nombre_campo)
                    Dim obj2 As TextBox = page1.FindControl(stru_campo_detalle(i).nombre_campo & "_2")
                    '-----------------------------------------
                    'Busca campo principal
                    '-----------------------------------------
                    If Not obj Is Nothing Then
                        obj.Text = ""
                    End If
                    If Not obj2 Is Nothing Then
                        obj2.Text = ""
                    End If
                Else
                    Dim obj As TextBox = page1.FindControl(stru_campo_detalle(i).nombre_campo)
                    If Not obj Is Nothing Then
                        obj.Text = ""
                    End If
                End If
            Next
            updatetable.Update()
            Limpiar_campos_consulta = "YES"
        Catch ex As Exception
            Limpiar_campos_consulta = "Inconsistencia general función Limpiar_campos_consulta " & ex.Message
        End Try
    End Function

    Function Generando_Consulta_Gabinete(ByVal page1 As Page,
                                         ByVal codigo_plantilla As Integer,
                                         ByVal NombreGabi As String,
                                         ByVal tipo_consulta As Integer,
                                         ByVal valor_consulta As String,
                                         ByRef colum_order_name As String,
                                         ByRef order_colum As String) As String
        Try
            Dim Refclas_ As New ClassGredview
            Dim Result As String = ""
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim Hidden_nureg As HtmlInputHidden = page1.FindControl("Hidden_nureg")
            Dim Hidden_nugab_sele As HtmlInputHidden = page1.FindControl("Hidden_nugab_sele")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID_VAL")
            If scripma Is Nothing Then
                Generando_Consulta_Gabinete = "Imposible encontrar datagrid  " & "GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Generando_Consulta_Gabinete = "Imposible encontrar el control  " & "titulo_label_val_radicacion"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Generando_Consulta_Gabinete = "Imposible encontrar el control  " & "UpdatePanelabel_val_radicacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            If updatelabel Is Nothing Then
                Generando_Consulta_Gabinete = "Imposible encontrar el control  " & "UpdatePanel_conenido_grid_val_radicacion"
                Exit Function
            End If
            If hideselecion Is Nothing Then
                Generando_Consulta_Gabinete = "Imposible encontrar el control  " & "hdnEmailID_VAL"
                Exit Function
            End If
            Hidden_nugab_sele.Value = NombreGabi
            Dim ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim stru_campo_detalle() As stru_campo_detalle = Nothing
            Dim seleccampos As String = "Select "
            If tipo_consulta <> 3 Then
                Result = ref_Class_DETALLE_GABIENETE.SolicitaDetalleCamposGabinete(NombreGabi,
                                                                                      stru_campo_detalle)
                If Result <> "YES" Then
                    Generando_Consulta_Gabinete = "Funcion  Generando_Consulta_Gabinete WF-01 Mensaje DBMS" & Result
                    Exit Function
                End If
                If stru_campo_detalle Is Nothing Then
                    Generando_Consulta_Gabinete = "Imposible encontrar los campos para gabinete : " & NombreGabi
                    Exit Function
                End If
                ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(0)
                HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(0) = "OPCIONES"
                ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(1)
                HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(1) = "ID"
                ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(2)
                HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(2) = "DISC"
                ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(3)
                HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(3) = "PAG"
                ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(4)
                HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(4) = "DBT"
                ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(5)
                HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(5) = "IDEX"
                ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(6)
                HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(6) = "USER"
                ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(7)
                HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(7) = "DATE1"
                ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(8)
                HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(8) = "TIME1"
                Dim iconta As Integer = 8
                Dim campo_clase_documento As String = ""
                For k As Integer = 0 To stru_campo_detalle.Length - 1
                    If stru_campo_detalle(k).nombre_campo = "TIPODOCUMENTO" Then
                        campo_clase_documento = "TIPODOCUMENTO"
                    End If
                Next
                For i As Integer = 0 To stru_campo_detalle.Length - 1
                    Dim refcampo As String = stru_campo_detalle(i).nombre_campo
                    If stru_campo_detalle(i).nombre_campo = "DATE" Then
                        refcampo = "CAST(" & stru_campo_detalle(i).nombre_campo & " AS DATE) AS " & stru_campo_detalle(i).nombre_campo
                    End If
                    If seleccampos = "Select " Then
                        If campo_clase_documento <> "" Then
                            seleccampos = seleccampos & "ID,DISC,PAG,DBT,IDEX,USER,DATE1,TIME1," & campo_clase_documento & "," & refcampo
                            iconta = iconta + 1
                            ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(iconta)
                            HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(iconta) = campo_clase_documento
                            iconta = iconta + 1
                            ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(iconta)
                            HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(iconta) = refcampo
                        Else
                            seleccampos = seleccampos & "ID,DISC,PAG,DBT,IDEX,USER,DATE1,TIME1," & refcampo
                            iconta = iconta + 1
                            ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(iconta)
                            HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(iconta) = refcampo
                        End If
                    Else
                        If stru_campo_detalle(i).nombre_campo <> campo_clase_documento Then
                            seleccampos = seleccampos & "," & refcampo
                            iconta = iconta + 1
                            ReDim Preserve HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(iconta)
                            HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta")(iconta) = refcampo
                        End If
                    End If
                Next
            End If
            Dim sqlfrom As String = " From " & NombreGabi
            Dim condicionsql As String = " where "
            Dim datakey() As String = {"id"}
            Dim prefijocampo As String = ""
            If tipo_consulta = 2 Then
                For i As Integer = 0 To stru_campo_detalle.Length - 1
                    Dim likeigual As String = " Like"
                    If condicionsql = " where " Then
                        condicionsql = condicionsql & stru_campo_detalle(i).nombre_campo & likeigual & "'%" & valor_consulta & "%'"
                    Else
                        condicionsql = condicionsql & " or " & stru_campo_detalle(i).nombre_campo & likeigual & "'%" & valor_consulta & "%'"
                    End If
                Next
            End If
            If tipo_consulta = 1 Then
                For i As Integer = 0 To stru_campo_detalle.Length - 1
                    If stru_campo_detalle(i).nombre_campo = "DATE" Or stru_campo_detalle(i).nombre_campo = "INT" Then
                        Dim obj As TextBox = page1.FindControl(stru_campo_detalle(i).nombre_campo)
                        Dim obj2 As TextBox = page1.FindControl(stru_campo_detalle(i).nombre_campo & "_2")
                        Dim objcheck As CheckBox = page1.FindControl(stru_campo_detalle(i).nombre_campo & "_check")
                        '-----------------------------------------
                        'Busca campo segundario
                        '-----------------------------------------
                        Dim campo_plantilla As String = stru_campo_detalle(i).nombre_campo
                        If Not obj Is Nothing And Not obj2 Is Nothing Then
                            If obj.Text <> "" And obj2.Text <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & " between '" & obj.Text & "' and '" & obj2.Text & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "  between '" & obj.Text & "' and '" & obj2.Text & "'"
                                End If
                            Else
                                If obj.Text <> "" Then
                                    Dim ref_text As String = obj.Text
                                    Dim likeigual As String = "="
                                    If Not objcheck Is Nothing Then
                                        If objcheck.Checked = True Then
                                            likeigual = " like"
                                            If InStr(ref_text, "%") <= 0 Then
                                                obj.Text = "%" & ref_text & "%"
                                            End If
                                        End If
                                    End If
                                    If condicionsql = " where " Then
                                        condicionsql = condicionsql & campo_plantilla & likeigual & "'" & ref_text & "'"
                                    Else
                                        condicionsql = condicionsql & " and " & campo_plantilla & "='" & ref_text & "'"
                                    End If
                                End If
                                If obj2.Text <> "" Then
                                    Dim ref_text As String = obj2.Text
                                    Dim likeigual As String = "="
                                    If Not objcheck Is Nothing Then
                                        If objcheck.Checked = True Then
                                            likeigual = " like"
                                            If InStr(ref_text, "%") <= 0 Then
                                                ref_text = "%" & ref_text & "%"
                                            End If
                                        End If
                                    End If
                                    If condicionsql = " where " Then
                                        condicionsql = condicionsql & campo_plantilla & likeigual & "'" & ref_text & "'"
                                    Else
                                        condicionsql = condicionsql & " and " & campo_plantilla & likeigual & "'" & ref_text & "'"
                                    End If
                                End If
                            End If
                        End If

                    Else
                        '-----------------------------------------------------------------
                        'Caso campo no between
                        '-----------------------------------------------------------------
                        Dim obj As TextBox = page1.FindControl(stru_campo_detalle(i).nombre_campo)
                        Dim objcheck As CheckBox = page1.FindControl(stru_campo_detalle(i).nombre_campo & "_check")
                        If Not obj Is Nothing Then
                            If obj.Text <> "" Then
                                Dim ref_text As String = obj.Text
                                Dim likeigual As String = "="
                                If Not objcheck Is Nothing Then
                                    If objcheck.Checked = True Then
                                        likeigual = " like"
                                        If InStr(ref_text, "%") <= 0 Then
                                            ref_text = "%" & ref_text & "%"
                                        End If
                                    End If
                                End If
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & stru_campo_detalle(i).nombre_campo & likeigual & "'" & ref_text & "'"
                                Else
                                    condicionsql = condicionsql & " and " & stru_campo_detalle(i).nombre_campo & likeigual & "'" & ref_text & "'"
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            Dim Sql_consulta As String = ""
            If tipo_consulta = 3 Then
                Sql_consulta = HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_DA_CONSULTA") & " AND DBT <=1 Order by " & colum_order_name & " " & order_colum & " LIMIT 2000"
            Else
                Sql_consulta = ""
                Sql_consulta = seleccampos & " " & sqlfrom & " " & condicionsql & " AND DBT <=1 Order by " & colum_order_name & " " & order_colum & " LIMIT 2000"
                If condicionsql = " where " Then
                    Generando_Consulta_Gabinete = "Debe seleccionar criterios de busqueda "
                    Exit Function
                End If
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_SOLICITUD_DA_CONSULTA") = tipo_consulta
            If tipo_consulta <> 3 Then
                HttpContext.Current.Session.Item("GA_DATO_CONSULTA_SOLICITUD_DA_CONSULTA") = seleccampos & " " & sqlfrom & " " & condicionsql
            End If
            Dim Datset_consulta As DataSet = New DataSet(NombreGabi)
            Dim Dat_reader As MySql.Data.MySqlClient.MySqlDataReader = Nothing
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta,
                                                 Datset_consulta,
                                                 NombreGabi)
            If Result <> "YES" Then
                Generando_Consulta_Gabinete = "Error listando datos " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                Hidden_nureg.Value = Datset_consulta.Tables(0).Rows.Count
                labetitle.Text = "Se encontro " & Datset_consulta.Tables(0).Rows.Count & " registro(s) en el gabinete " & NombreGabi
                scripma.DataSource = Datset_consulta
                scripma.DataKeyNames = datakey
                hideselecion.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Generando_Consulta_Gabinete = "YES"
                Exit Function
            Else
                Hidden_nureg.Value = Datset_consulta.Tables(0).Rows.Count
                labetitle.Text = "Se encontro " & Datset_consulta.Tables(0).Rows.Count & " registro(s) en el gabinete " & NombreGabi
                scripma.DataKeyNames = datakey
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("onmousedown", "RowClick(this,false);")
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fal fa-file-image fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documento asociado al registro")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "visualiza_documento")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fal fa-info fa-lg imag_crusor_da")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn   btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver indice registro documento")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "indice_documento")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-file-download fa-lg ")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Descargar documento relacionado al registro")
                    ahtml.Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "descarga_documento")
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
                        If scripma.Rows(i).Cells(z).Text <> "" Then
                            Dim subtrin As String = scripma.Rows(i).Cells(z).Text
                            If InStr(subtrin, "/") > 0 And subtrin.Length <= 27 Then
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                If splitsubtrin.Length > 2 Then
                                    Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                    scripma.Rows(i).Cells(z).Text = tempo_fecha
                                End If
                            End If
                        End If
                    Next
                Next
                Result = Refclas_.add_clase_acender_decender(colum_order_name,
                                                      HttpContext.Current.Session.Item("Sort_matri_colum_da_consulta"),
                                                      order_colum,
                                                      scripma)
                If Result <> "YES" Then
                    Generando_Consulta_Gabinete = "Error add clase función  add_clase_acender_decender " & Result
                    Exit Function
                End If
                Generando_Consulta_Gabinete = "YES"
                Exit Function
            End If
            Generando_Consulta_Gabinete = "YES"
        Catch ex As Exception
            Generando_Consulta_Gabinete = "Inconsistencia Funcion Generando_Consulta_Gabinete " & ex.Message
        End Try
    End Function
    Function Generando_Consulta_Gabinete_publica(ByVal page1 As Page,
                                                 ByVal codigo_plantilla As Integer,
                                                 ByVal NombreGabi As String, ByVal p As Integer) As String
        Try
            Dim Result As String = ""
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID_VAL")
            If scripma Is Nothing Then
                Generando_Consulta_Gabinete_publica = "Imposible encontrar datagrid  " & "GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Generando_Consulta_Gabinete_publica = "Imposible encontrar el control  " & "titulo_label_val_radicacion"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Generando_Consulta_Gabinete_publica = "Imposible encontrar el control  " & "UpdatePanelabel_val_radicacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            If updatelabel Is Nothing Then
                Generando_Consulta_Gabinete_publica = "Imposible encontrar el control  " & "UpdatePanel_conenido_grid_val_radicacion"
                Exit Function
            End If
            If hideselecion Is Nothing Then
                Generando_Consulta_Gabinete_publica = "Imposible encontrar el control  " & "hdnEmailID_VAL"
                Exit Function
            End If
            Dim I2 As Integer = 0
            Dim Sql_consulta = "SELECT CAMPO,TIPO,SISTEMA,VISIBLE,ESTADO,CAMPO_PUBLICO FROM " &
                "DETALLE_GABIENETE " &
                "WHERE GABINETE='" & NombreGabi & "' AND VISIBLE=1  ORDER BY IDENTI"

            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(NombreGabi)
            Result = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Generando_Consulta_Gabinete_publica = "Funcion  Generando_Consulta_Gabinete_publica WF-01 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Generando_Consulta_Gabinete_publica = "Imposible encontrar los campos para gabinete : " & NombreGabi
                Exit Function
            End If
            Dim Matri_campo_nombre As String = ""
            Dim Matri_Campos_Gabinete() As String
            Erase Matri_Campos_Gabinete
            Dim icont As Integer = 0
            For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                'ReDim Preserve Datos_Imagen(I)
                'If Datset.Tables(0).Rows(y).Item(5) = 1 Then
                ReDim Preserve Matri_Campos_Gabinete(icont)
                Matri_Campos_Gabinete(icont) = Datset.Tables(0).Rows(y).Item(0).ToString
                If Datset.Tables(0).Rows(y).IsNull(1) = False Then
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & Datset.Tables(0).Rows(y).Item(1).ToString
                Else
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & ""
                End If
                If Datset.Tables(0).Rows(y).IsNull(2) = False Then
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & Datset.Tables(0).Rows(y).Item(2).ToString
                Else
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & ""
                End If
                If Datset.Tables(0).Rows(y).IsNull(3) = False Then
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & Datset.Tables(0).Rows(y).Item(2).ToString
                Else
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & ""
                End If
                If Datset.Tables(0).Rows(y).IsNull(4) = False Then
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & Datset.Tables(0).Rows(y).Item(4).ToString
                Else
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & ""
                End If
                If Datset.Tables(0).Rows(y).IsNull(5) = False Then
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & Datset.Tables(0).Rows(y).Item(5).ToString
                Else
                    Matri_Campos_Gabinete(icont) = Matri_Campos_Gabinete(icont) & "|" & ""
                End If
                If y = 0 Then
                    Matri_campo_nombre = Datset.Tables(0).Rows(y).Item(0).ToString
                Else
                    Matri_campo_nombre = Matri_campo_nombre & "," & Datset.Tables(0).Rows(y).Item(0).ToString
                End If
                icont = icont + 1
                'End If
            Next
            Dim seleccampos As String = "Select "
            For i As Integer = 0 To Matri_Campos_Gabinete.Length - 1
                Dim spli_matri() As String = Matri_Campos_Gabinete(i).ToString.Split("|")

                If seleccampos = "Select " Then
                    seleccampos = seleccampos & "ID,DISC,PAG,DBT,IDEX,USER,DATE1,TIME1," & spli_matri(0)
                Else
                    seleccampos = seleccampos & "," & spli_matri(0)
                End If

            Next
            Dim sqlfrom As String = " From " & NombreGabi
            Dim condicionsql As String = " where "
            Dim datakey() As String = {"id"}
            Dim indeslen As Integer = Matri_Campos_Gabinete.Length
            'ReDim Preserve Matri_Campos_Gabinete(indeslen)
            'Matri_Campos_Gabinete(indeslen) = "DATE1|DATE|0|0|0"
            Dim prefijocampo As String = ""
            For i As Integer = 0 To Matri_Campos_Gabinete.Length - 1
                Dim Matri_Datos() As String = Matri_Campos_Gabinete(i).ToString.Split("|")
                If Matri_Datos(5) = 1 Then
                    If Matri_Datos(1) = "DATE" Or Matri_Datos(1) = "INT" Then
                        Dim obj As TextBox = page1.FindControl(Matri_Datos(0))
                        Dim obj2 As TextBox = page1.FindControl(Matri_Datos(0) & "_2")
                        '-----------------------------------------
                        'Busca campo principal
                        '-----------------------------------------
                        If obj Is Nothing Then
                            Generando_Consulta_Gabinete_publica = "Imposible encontrar campo " & prefijocampo & Matri_Datos(0)
                            Exit Function
                        End If
                        '-----------------------------------------
                        'Busca campo segundario
                        '-----------------------------------------
                        Dim campo_plantilla As String = Matri_Datos(0)
                        'If LCase(Matri_Datos(i).Campo_Plantilla) = "fecha_radicado" Then
                        '    campo_plantilla = " CAST(" & Matri_Datos(i).Campo_Plantilla & " AS DATE) "
                        'Else
                        '    campo_plantilla = Matri_Datos(i).Campo_Plantilla
                        'End If
                        If obj2 Is Nothing Then
                            Generando_Consulta_Gabinete_publica = "Imposible encontrar campo " & prefijocampo & Matri_Datos(0)
                            Exit Function
                        End If
                        If obj.Text <> "" And obj2.Text <> "" Then
                            If condicionsql = " where " Then
                                condicionsql = condicionsql & campo_plantilla & " between '" & obj.Text & "' and '" & obj2.Text & "'"
                            Else
                                condicionsql = condicionsql & " and " & campo_plantilla & "  between '" & obj.Text & "' and '" & obj2.Text & "'"
                            End If
                        Else
                            If obj.Text <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & "='" & obj.Text & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "='" & obj.Text & "'"
                                End If
                            End If
                            If obj2.Text <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & "='" & obj2.Text & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "='" & obj2.Text & "'"
                                End If
                            End If
                        End If
                    Else
                        '-----------------------------------------------------------------
                        'Caso campo no between
                        '-----------------------------------------------------------------
                        Dim obj As TextBox = page1.FindControl(Matri_Datos(0))
                        If obj Is Nothing Then
                            Generando_Consulta_Gabinete_publica = "Imposible encontrar campo " & prefijocampo & Matri_Datos(0)
                            Exit Function
                        End If
                        If obj.Text <> "" Then
                            If condicionsql = " where " Then
                                condicionsql = condicionsql & Matri_Datos(0) & "='" & obj.Text & "'"
                            Else
                                condicionsql = condicionsql & " and " & Matri_Datos(0) & "='" & obj.Text & "'"
                            End If
                        End If
                    End If
                End If
            Next
            Sql_consulta = ""
            Sql_consulta = seleccampos & " " & sqlfrom & " " & condicionsql & " AND DBT <=1 Order by  ID ASC  LIMIT 500"
            If condicionsql = " where " Then
                Generando_Consulta_Gabinete_publica = "Debe seleccionar criterios de busqueda "
                Exit Function
            End If
            Dim Datset_consulta As DataSet = New DataSet(NombreGabi)
            Dim Dat_reader As MySql.Data.MySqlClient.MySqlDataReader = Nothing
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset_consulta, NombreGabi)
            If Result <> "YES" Then
                Generando_Consulta_Gabinete_publica = "Error listando datos " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                'If Dat_reader.HasRows = True Then
                labetitle.Text = "Se encontro " & Datset_consulta.Tables(0).Rows.Count & " registro(s) en el gabinete " & NombreGabi
                scripma.DataSource = Datset_consulta
                scripma.DataKeyNames = datakey
                hideselecion.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Generando_Consulta_Gabinete_publica = "YES"
                Exit Function
            Else
                'HttpContext.Current.Session.Item("RA_DATO_CONSULTA_RADICADO") = sql_consulta
                labetitle.Text = "Se encontro " & Datset_consulta.Tables(0).Rows.Count & " registro(s) en el gabinete " & NombreGabi
                scripma.DataKeyNames = datakey
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                Next
                Generando_Consulta_Gabinete_publica = "YES"
                Exit Function
            End If
            Generando_Consulta_Gabinete_publica = "YES"
        Catch ex As Exception
            Generando_Consulta_Gabinete_publica = "Inconsistencia Funcion Generando_Consulta_Gabinete_publica " & ex.Message
        End Try
    End Function
    Function asigna_imagen_gabinete(ByVal Nombre_Gabinete As String, ByRef imga As Object) As String
        Try
            Dim color_gabinete As String = "Negro"
            Dim Result As String = ""
            Result = Me.Retorna_color_gabinete(Nombre_Gabinete, color_gabinete)
            If Result <> "YES" Then
                asigna_imagen_gabinete = Result
                Exit Function
            End If
            Dim ruta_gabinete As String = ""
            Select Case color_gabinete
                Case "Azul"
                    ruta_gabinete = "../Docuarchi/imagenes/GabineteAzul.png"
                Case "Verde"
                    ruta_gabinete = "../Docuarchi/imagenes/GabineteVerde.png"
                Case "Amarillo"
                    ruta_gabinete = "../Docuarchi/imagenes/Gabineteamarillo.png"
                Case "Rojo"
                    ruta_gabinete = "../Docuarchi/imagenes/GabineteRojo.png"
                Case "Negro"
                    ruta_gabinete = "../Docuarchi/imagenes/negro.png"
            End Select
            imga.ImageUrl = ruta_gabinete
            asigna_imagen_gabinete = "YES"
        Catch ex As Exception
            asigna_imagen_gabinete = "Inconsistencia funcion asigna_imagen_gabinete " & ex.Message
        End Try
    End Function
    Function Retorna_color_gabinete(ByVal Nombre_Gabinete As String,
                                    ByRef color_gabinete As String) As String
        Try
            Dim Sql_consulta As String = "Select COLOR_GABINETE  from config_gabinete_detalle where NOMBRE_GABINETE='" & Nombre_Gabinete & "'"
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet(Nombre_Gabinete)
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_color_gabinete = "Funcion Retorna_color_gabinete dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_color_gabinete = "Imposible en contrar el color del gabinete " & Nombre_Gabinete
                Exit Function
            Else
                color_gabinete = Datset.Tables(0).Rows(0).Item(0)
                Retorna_color_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_color_gabinete = "Inconsistencia general función Retorna_color_gabinete " & ex.Message
        End Try
    End Function
    Function lista_valores_campo_edita(ByVal NombreGabi As String,
                                       ByVal campos As String,
                                       ByVal id_imagen As Integer,
                                       ByRef datos As String) As String
        Try
            Dim matri_campos As String() = campos.Split("|")
            Dim campos_consulta As String = ""
            For i As Integer = 0 To matri_campos.Length - 1
                If i = 0 Then
                    campos_consulta = matri_campos(i)
                Else
                    campos_consulta = campos_consulta & "," & matri_campos(i)
                End If
            Next
            Dim Sql_consulta = "SELECT " & campos_consulta & " FROM " & NombreGabi & " where id=" & id_imagen
            Dim Result As String = ""
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
            Result = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                lista_valores_campo_edita = "Funcion  lista_valores_campo_edita WF-01 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                lista_valores_campo_edita = "Imposible encontrar id para el gabinete : " & NombreGabi
                Exit Function
            Else
                For y As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    If y = 0 Then
                        If Datset.Tables(0).Rows(0).IsNull(y) = True Then
                            datos = ""
                        Else
                            Dim obsgetipe As Object = Datset.Tables(0).Rows(0).Item(y).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = Datset.Tables(0).Rows(0).Item(y).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                datos = tempo_fecha
                            Else
                                datos = Datset.Tables(0).Rows(0).Item(y)
                            End If

                        End If
                    Else
                        If Datset.Tables(0).Rows(0).IsNull(y) = True Then
                            datos = datos & "|||||" & ""
                        Else
                            Dim obsgetipe As Object = Datset.Tables(0).Rows(0).Item(y).GetType.ToString
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = Datset.Tables(0).Rows(0).Item(y).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                datos = datos & "|||||" & tempo_fecha
                            Else
                                datos = datos & "|||||" & Datset.Tables(0).Rows(0).Item(y)
                            End If

                        End If
                    End If
                Next
                lista_valores_campo_edita = "YES"
                Exit Function
            End If

        Catch ex As Exception
            lista_valores_campo_edita = "Inconsistencia general función lista_valores_campo_edita " & ex.Message
        End Try

    End Function
    Function Lista_campos_edicion_gabinete(ByVal NombreGabi As String,
                                           ByRef campos As String) As String
        Try
            Dim Sql_consulta = "SELECT CAMPO FROM " &
                   "DETALLE_GABIENETE " &
                   "WHERE GABINETE='" & NombreGabi & "' AND VISIBLE=1 ORDER BY IDENTI"
            Dim Result As String = ""
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
            Result = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_campos_edicion_gabinete = "Funcion  Lista_campos_edicion_gabinete WF-01 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_campos_edicion_gabinete = "Imposible encontrar los campos para gabinete : " & NombreGabi
                Exit Function
            Else
                For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If y = 0 Then
                        campos = Datset.Tables(0).Rows(y).Item(0)
                    Else
                        campos = campos & "|" & Datset.Tables(0).Rows(y).Item(0)
                    End If
                Next
                Lista_campos_edicion_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_campos_edicion_gabinete = "Inconsistencia general función Lista_campos_edicion_gabinete " & ex.Message
        End Try
    End Function
    Function Genera_interface_consulta(
                                       ByVal NombreGabi As String,
                                       ByRef Page1 As Page,
                                       ByVal tipo_consulta As Integer) As String
        Try
            Dim Refclas_indice As New ClassWorkflowIndiceDA
            Dim ref As New ClassListandoTareas
            Dim pane As Panel = Page1.FindControl("Panel_campos_consuta")
            Dim Update As UpdatePanel = Page1.FindControl("UpdatePanel_consulta")
            Update.UpdateMode = UpdatePanelUpdateMode.Conditional
            Dim Result As String = ""
            Dim I2 As Integer = 0
            Dim ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim stru_campo_detalle() As stru_campo_detalle = Nothing
            If tipo_consulta = 1 Then
                Result = ref_Class_DETALLE_GABIENETE.SolicitaDetalleCamposGabinete(NombreGabi,
                                                                                      stru_campo_detalle)
                If Result <> "YES" Then
                    Genera_interface_consulta = "Funcion  Generando_Consulta_Gabinete WF-01 Mensaje DBMS" & Result
                    Exit Function
                End If
                If stru_campo_detalle Is Nothing Then
                    Genera_interface_consulta = "Imposible encontrar los campos para gabinete : " & NombreGabi
                    Exit Function
                End If
            Else
                Result = ref_Class_DETALLE_GABIENETE.Solicita_detalle_campos_gabinete_publico(NombreGabi,
                                                                                             stru_campo_detalle)
                If Result <> "YES" Then
                    Genera_interface_consulta = "Funcion  Generando_Consulta_Gabinete WF-01 Mensaje DBMS" & Result
                    Exit Function
                End If
                If stru_campo_detalle Is Nothing Then
                    Genera_interface_consulta = "Imposible encontrar los campos para gabinete : " & NombreGabi
                    Exit Function
                End If
            End If

            'Dim Sql_consulta = "SELECT CAMPO,TIPO,SISTEMA,VISIBLE,ESTADO FROM " & _
            '    "DETALLE_GABIENETE " & _
            '    "WHERE GABINETE='" & NombreGabi & "' AND VISIBLE=1 ORDER BY IDENTI"
            'Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            'Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
            'Result = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            'If Result <> "YES" Then
            '    Genera_interface_consulta = "Funcion  Genera_interface_consulta WF-01 Mensaje DBMS " & Result
            '    Exit Function
            'End If
            'If Datset.Tables(0).Rows.Count = 0 Then
            '    Genera_interface_consulta = "Imposible encontrar los campos para gabinete : " & NombreGabi
            '    Exit Function
            'End If
            'Dim Matri_campo_nombre As String = ""
            'Dim Matri_Campos_Gabinete() As String
            'Erase Matri_Campos_Gabinete
            'For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
            '    ReDim Preserve Matri_Campos_Gabinete(y)
            '    Matri_Campos_Gabinete(y) = Datset.Tables(0).Rows(y).Item(0).ToString
            '    If Datset.Tables(0).Rows(y).IsNull(1) = False Then
            '        Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & Datset.Tables(0).Rows(y).Item(1).ToString
            '    Else
            '        Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & ""
            '    End If
            '    If Datset.Tables(0).Rows(y).IsNull(2) = False Then
            '        Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & Datset.Tables(0).Rows(y).Item(2).ToString
            '    Else
            '        Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & ""
            '    End If
            '    If Datset.Tables(0).Rows(y).IsNull(3) = False Then
            '        Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & Datset.Tables(0).Rows(y).Item(2).ToString
            '    Else
            '        Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & ""
            '    End If
            '    If Datset.Tables(0).Rows(y).IsNull(4) = False Then
            '        Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & Datset.Tables(0).Rows(y).Item(4).ToString
            '    Else
            '        Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & ""
            '    End If
            '    If y = 0 Then
            '        Matri_campo_nombre = Datset.Tables(0).Rows(y).Item(0).ToString
            '    Else
            '        Matri_campo_nombre = Matri_campo_nombre & "," & Datset.Tables(0).Rows(y).Item(0).ToString
            '    End If
            'Next
            '********************************************
            'Consulta opcion aplica trd
            '*******************************************
            Dim estru_gestion As estructure_gestion
            estru_gestion = Nothing
            Dim refclastrd As New ClassTrdDocumental
            Dim opt_tabla_retencion As Integer = 0
            Dim option_inventario As Integer = 0
            Dim id_inventario As Integer = 0
            Dim opt_seleccion_unidad As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") <> 0 Then
                Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(opt_tabla_retencion,
                                                                                   NombreGabi)
                If Result <> "YES" Then
                    Genera_interface_consulta = Result
                    Exit Function
                End If

                '************************************************
                'Consulta opcion selecciona unidad documental
                '************************************************

                Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(opt_seleccion_unidad,
                                                                                   NombreGabi)
                If Result <> "YES" Then
                    Genera_interface_consulta = Result
                    Exit Function
                End If
                '----------------------------------------------------
                'Verfica si esta activado invnetario documental
                '-----------------------------------------------------
                Dim refclasalmacen As New ClassTrdDocumental
                Result = ref_Class_system1.VerificaOpcionAplicarInventarioDocumental(option_inventario,
                                                                                        NombreGabi)
                If Result <> "YES" Then
                    Genera_interface_consulta = Result
                    Exit Function
                End If
                If option_inventario = 1 Then
                    If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                        'Genera_interface_consulta = "El usuario docuarchi debe estar asociado a un usuario de gestión  "
                        'Exit Function
                    End If

                End If
            End If
            Dim Table As Table = Page1.FindControl("Table_campos_consulta")
            Dim objRow As TableRow
            Dim objCell As TableCell
            Dim m_TextBoxes() As TextBox = {}
            Dim m_TextBoxes2() As TextBox = {}
            Dim LabelBox() As CheckBox = {}
            Dim LabelBox_title() As Label = {}
            '**********************************
            'Creacion del boton guardar
            '**********************************
            objRow = New TableRow
            objCell = New TableCell
            Dim Icontr As Integer = 0
            Dim z As Integer = 0
            'Dim Matri_CampoES() As String
            'Erase Matri_CampoES
            Dim refclas_radicado As New ClassRadicador
            For z = 0 To stru_campo_detalle.Length - 1
                'Erase Matri_CampoES
                'Matri_CampoES = Matri_Campos_Gabinete(z).Split("|")
                ReDim Preserve m_TextBoxes(z)
                ReDim Preserve m_TextBoxes2(z)
                ReDim Preserve LabelBox(z + 1)
                ReDim Preserve LabelBox_title(z + 1)
                '--inicializa label-textbox
                LabelBox(z + 1) = New CheckBox
                LabelBox_title(z + 1) = New Label
                m_TextBoxes(z) = New TextBox
                m_TextBoxes2(z) = New TextBox
                If stru_campo_detalle(z).nombre_campo = "" Then
                    LabelBox_title(z + 1).Text = "SIN CAMPO"
                    m_TextBoxes(z).Text = "SIN CAMPO"
                    m_TextBoxes(z).ID = "SIN CAMPO-Z"
                Else
                    If stru_campo_detalle(z).nombre_campo = "ENLASE" Then
                        LabelBox_title(z + 1).Text = "ENLACE"
                    Else
                        LabelBox_title(z + 1).Text = stru_campo_detalle(z).nombre_campo
                    End If
                    If stru_campo_detalle(z).nombre_campo = "DATE1" Then
                        LabelBox_title(z + 1).Text = "FECHA ALMACENAMIENTO"

                    End If
                    LabelBox(z + 1).ID = stru_campo_detalle(z).nombre_campo & "_check"
                    LabelBox_title(z + 1).Font.Name = "Segoe UI"
                    LabelBox_title(z + 1).Attributes.Add("class", "h6 font-weight-light  ml-2 ")
                    m_TextBoxes(z).ID = stru_campo_detalle(z).nombre_campo
                    m_TextBoxes2(z).ID = stru_campo_detalle(z).nombre_campo & "_2"
                End If
                If stru_campo_detalle(z).nombre_campo = "INT" Or stru_campo_detalle(z).nombre_campo = "DATE" Then
                    Result = Refclas_indice.agregar_auto_complete_docuarchi(m_TextBoxes(z).ID, pane, "GetPosiblesDatosGabinete", NombreGabi, stru_campo_detalle(z).nombre_campo)
                    Result = Refclas_indice.agregar_auto_complete_docuarchi(m_TextBoxes2(z).ID, pane, "GetPosiblesDatosGabinete", NombreGabi, stru_campo_detalle(z).nombre_campo)
                Else
                    Result = Refclas_indice.agregar_auto_complete_docuarchi(m_TextBoxes(z).ID, pane, "GetPosiblesDatosGabinete", NombreGabi, stru_campo_detalle(z).nombre_campo)
                End If
                '------------------------------------------------------------------------
                objRow = New TableRow()
                objCell = New TableCell
                objCell.Attributes.Add("class", "pt-2")
                objCell.Controls.Add(LabelBox(z + 1))
                objCell.Controls.Add(LabelBox_title(z + 1))
                objRow.Cells.Add(objCell)
                Table.Rows.Add(objRow)
                objRow = New TableRow()
                objCell = New TableCell
                If stru_campo_detalle(z).nombre_campo = "INT" Or stru_campo_detalle(z).nombre_campo = "DATE" Then
                    If stru_campo_detalle(z).nombre_campo = "DATE" Then
                        m_TextBoxes(z).Width = 95
                        m_TextBoxes(z).MaxLength = 10
                        m_TextBoxes(z).Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                        m_TextBoxes(z).Attributes.Add("placeholder", "0000 00 00")
                        objCell.Controls.Add(m_TextBoxes(z))
                        Dim bhtml As New HtmlControls.HtmlGenericControl("button")
                        bhtml.Attributes.Add("class", "ml-1 btn border-0")
                        bhtml.Attributes.Add("title", "formato aaaa mm dd")
                        bhtml.ID = UCase("Fecha_ela_" & stru_campo_detalle(z).nombre_campo) & "33" & z
                        Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                        ihtml.Attributes.Add("class", "fad fa-calendar-alt fa-1x")
                        bhtml.Controls.Add(ihtml)
                        objCell.Controls.Add(bhtml)
                        Result = refclas_radicado.Agregar_Calendar(bhtml.ID, m_TextBoxes(z).ID, pane)
                        If Result <> "YES" Then
                            'Genera_Interface_Radicacion_Entrante = Result
                            'Exit Function
                        End If
                        m_TextBoxes2(z).MaxLength = 10
                        m_TextBoxes2(z).Attributes.Add("onkeypress", "GetChar (event);")
                        m_TextBoxes2(z).Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                        m_TextBoxes2(z).Attributes.Add("placeholder", "0000 00 00")
                        m_TextBoxes2(z).Width = 95
                        m_TextBoxes2(z).Attributes.Add("class", "ml-2")
                        objCell.Controls.Add(m_TextBoxes2(z))
                        bhtml = New HtmlControls.HtmlGenericControl("button")
                        bhtml.Attributes.Add("class", "ml-1 btn border-0")
                        bhtml.Attributes.Add("title", "formato aaaa mm dd")
                        bhtml.ID = UCase("Fecha_ela__" & stru_campo_detalle(z).nombre_campo) & "_34" & z
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Attributes.Add("class", "fad fa-calendar-alt fa-1x")
                        bhtml.Controls.Add(ihtml)
                        objCell.Controls.Add(bhtml)
                        Result = refclas_radicado.Agregar_Calendar(bhtml.ID, m_TextBoxes2(z).ID, pane)
                        If Result <> "YES" Then
                            'Genera_Interface_Radicacion_Entrante = Result
                            'Exit Function
                        End If
                        m_TextBoxes2(z).CssClass = "date_indice"
                    Else
                        m_TextBoxes(z).Width = 90
                        m_TextBoxes2(z).Width = 90
                        objCell.Controls.Add(m_TextBoxes(z))
                        objCell.Controls.Add(m_TextBoxes2(z))
                    End If
                Else
                    m_TextBoxes(z).CssClass = "date_indice_text_box"
                    m_TextBoxes(z).Width = pane.Width
                    objCell.Controls.Add(m_TextBoxes(z))
                End If
                objRow.Cells.Add(objCell)
                'objRow.Width = 5
                'objRow.Height = 5
                '-------------------------------------------------------------------------
                'Dim boton_trd As New Button
                'boton_trd.ID = "boton_trd"
                'boton_trd.Text = "T"
                'boton_trd.ToolTip = "Selecciona tabla retención documental "
                'Dim boton_trd_restore As New Button
                'boton_trd_restore.ID = "boton_trd_restore"
                'boton_trd_restore.Text = "R"
                'boton_trd_restore.ToolTip = "Restaura tabla retención documental "
                If opt_tabla_retencion = 1 Then
                    If m_TextBoxes(z).ID = "NOMBRESERIE" Then
                        m_TextBoxes(z).BackColor = Color.PaleGoldenrod
                        'boton_trd.BackColor = Color.PaleGoldenrod
                        'boton_trd_restore.BackColor = Color.PaleGoldenrod
                        'objCell.Controls.Add(boton_trd)
                        'objCell.Controls.Add(boton_trd_restore)
                        'AddHandler boton_trd.Click, AddressOf _
                        'comman_trd_clik
                        'AddHandler boton_trd_restore.Click, AddressOf _
                        'comman_trd_restore_clik
                    End If
                End If
                If m_TextBoxes(z).ID = "NOMBRESUBSERIE" Then
                    m_TextBoxes(z).BackColor = Color.PaleGoldenrod
                End If
                If m_TextBoxes(z).ID = "TIPODOCUMENTO" Then
                    m_TextBoxes(z).BackColor = Color.PaleGoldenrod
                End If
                'Dim boton_clase_documento As New Button
                'boton_clase_documento.ID = "boton_clase_documento"
                'boton_clase_documento.ToolTip = "Selecciona tipo documento, unidad documental simple"
                'boton_clase_documento.Text = "C"
                'Dim boton_clase_documento_restore As New Button
                'boton_clase_documento_restore.ID = "boton_clase_documento_restore"
                'boton_clase_documento_restore.ToolTip = "Restaura tipo documento, unidad documental simple"
                'boton_clase_documento_restore.Text = "R"
                '-----expdiente
                'Dim boton_expediente As New Button
                'boton_expediente.ID = "boton_expediente"
                'boton_expediente.Text = "E"
                'boton_expediente.ToolTip = "Selecciona unidad compleja, compuesta (Expediente)"
                'Dim boton_expediente_restore As New Button
                'boton_expediente_restore.ID = "boton_expediente_restore"
                'boton_expediente_restore.Text = "R"
                'boton_expediente_restore.ToolTip = "Restaura unidad compleja, compuesta (Expediente)"
                '----unidad conservacion
                'Dim boton_unidad_conserva As New Button
                'boton_unidad_conserva.ID = "boton_unidad_conserva"
                'boton_unidad_conserva.Text = "U"
                'boton_unidad_conserva.ToolTip = "Selecciona unidad conservación (Carpeta, legajo, tómo, etc...)"
                'Dim boton_fecha_elaboracion As New Button
                'boton_fecha_elaboracion.ID = "boton_fecha_elaboracion"
                'boton_fecha_elaboracion.Text = "F"
                'boton_fecha_elaboracion.ToolTip = "Selecciona fecha elaboración del documento "
                'Dim boton_fecha_elaboracion_restore As New Button
                'boton_fecha_elaboracion_restore.ID = "boton_fecha_elaboracion_restore"
                'boton_fecha_elaboracion_restore.Text = "R"
                'boton_fecha_elaboracion_restore.ToolTip = "Restaura fecha elaboración del documento "
                If opt_seleccion_unidad = 1 Then
                    If m_TextBoxes(z).ID = "EXPEDIENTE" Then
                        m_TextBoxes(z).BackColor = Color.Pink
                        ' boton_expediente.BackColor = Color.Pink
                        ' boton_expediente_restore.BackColor = Color.Pink
                        ' objCell.Controls.Add(boton_expediente)
                        ' objCell.Controls.Add(boton_expediente_restore)
                        ' AddHandler boton_expediente.Click, AddressOf _
                        '  comman_expediente_clik
                        ' AddHandler boton_expediente_restore.Click, AddressOf _
                        'comman_expediente_restore_clik
                    End If
                    If m_TextBoxes(z).ID = "CLASEDOCUMENTO" Then
                        m_TextBoxes(z).BackColor = Color.GreenYellow
                        ' boton_clase_documento.BackColor = Color.GreenYellow
                        ' boton_clase_documento_restore.BackColor = Color.GreenYellow
                        ' objCell.Controls.Add(boton_clase_documento)
                        ' objCell.Controls.Add(boton_clase_documento_restore)
                        ' AddHandler boton_clase_documento.Click, AddressOf _
                        '  comman_tipo_documemnto_clik
                        ' AddHandler boton_clase_documento_restore.Click, AddressOf _
                        'comman_tipo_documemnto_restore_clik
                    End If
                    If m_TextBoxes(z).ID = "UNIDADCONSERVA" Then
                        m_TextBoxes(z).BackColor = Color.PaleGreen
                        'objCell.Controls.Add(boton_unidad_conserva)
                        'boton_unidad_conserva.SetBounds(m_TextBoxes(z).Bounds.X + m_TextBoxes(z).Width, _
                        'm_TextBoxes(z).Bounds.Y, 25, m_TextBoxes(z).Bounds.Height)
                        'refpanel.Controls.Add(boton_unidad_conserva)
                        'm_TextBoxes(z).ContextMenuStrip = ContextMenuStripunidadconservacion
                        'AddHandler boton_unidad_conserva.Click, AddressOf _
                        'comman_unidad_simple_clik
                    End If
                    If m_TextBoxes(z).ID = "FECHAELABORACION" Then
                        'm_TextBoxes(z).BackColor = Color.PaleGoldenrod
                        'boton_fecha_elaboracion.BackColor = Color.PaleGoldenrod
                        'boton_fecha_elaboracion_restore.BackColor = Color.PaleGoldenrod
                        m_TextBoxes(z).Enabled = True
                        'objCell.Controls.Add(boton_fecha_elaboracion)
                        'objCell.Controls.Add(boton_fecha_elaboracion_restore)
                        If HttpContext.Current.Session.Item("GA_APLICATRD_DOCUMENTOS") <> 0 Then
                            'Result = refclas_radicado.Agregar_Calendar(boton_fecha_elaboracion.ID, m_TextBoxes(z).ID, pane)
                            'If Result <> "YES" Then
                            '    'Genera_Interface_Radicacion_Entrante = Result
                            '    'Exit Function
                            'End If
                        End If
                        'AddHandler boton_fecha_elaboracion_restore.Click, AddressOf _
                        'comman_trd_fecha_restore_clik
                        'AddHandler boton_fecha_elaboracion.Click, AddressOf _
                        'comman_trd_fecha_clik  & "_2"
                    End If
                End If
                Table.Rows.Add(objRow)
            Next
            'nombre = m_TextBoxes(0).ID
            'pane.Controls.Add(Table)
            'Dim tribOTON1 As New AsyncPostBackTrigger()
            'tribOTON1.ControlID = "Button_actualiza_hiden_Expediente"
            'Update.Triggers.Add(tribOTON1)
            'Update.ContentTemplateContainer.Controls.Add(pane)
            'Dim hiden As HtmlInputHidden = Page1.FindControl("Hiddenheih")
            'pane.Height = Val(hiden.Value)
            'Page1.Form.Controls.Add(Update)
            Update.Update()
            Genera_interface_consulta = "YES"
        Catch ex As Exception
            Genera_interface_consulta = "Inconsistencia general funcion Genera_interface_consulta " & ex.Message
        End Try
    End Function
   
    Private Sub comman_trd_clik(ByVal sender As  _
       System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim update As UpdatePanel = sender.page.findcontrol("UpdatePanel_consulta")
        Try
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                MsgBox("El usuario docuarchi no tiene usuario de gestión relacionado", MsgBoxStyle.Information)
                'sender.focus()
                Exit Sub
            End If
            'If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
            '    If HttpContext.Current.Session.Item("GA_APLICATRD_DOCUMENTOS") = 0 Then
            '        Mens.Showscripman("El usuario no tiene permisos para aplicar trd al documento", update)
            '        'sender.focus()
            '        Exit Sub
            '    End If
            '    Dim resulta As String = ""
            '    Dim refclas_inventario As New ClassGaGestionInventario
            '    If Hidden_id_inventario.Value <> 0 Then
            '        resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '        If resulta <> "YES" Then
            '            Mens.Showscripman(resulta, update)
            '            'sender.focus()
            '            Exit Sub
            '        End If
            '    End If
            'End If

            Dim ref_Iframe_trd_popup = sender.page.findcontrol("Iframe_trd_popup_")
            ref_Iframe_trd_popup.Attributes.Add("src", "../gestion/WebFormGaAplicarTrd.aspx")
            Dim ref_UpdatePanel_trd_popup = sender.page.findcontrol("UpdatePanel_trd_popup")
            ref_UpdatePanel_trd_popup.Update()
            Dim ref_ModalPopupExtende_trd_popup = sender.page.findcontrol("ModalPopupExtende_trd_popup")
            ref_ModalPopupExtende_trd_popup.Show()
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally

        End Try
    End Sub
    Private Sub comman_trd_restore_clik(ByVal sender As  _
          System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim update As UpdatePanel = sender.page.findcontrol("UpdatePanel_consulta")
        Try
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                MsgBox("El usuario docuarchi no tiene usuario de gestión relacionado", MsgBoxStyle.Information)
                'sender.focus()
                Exit Sub
            End If
            'If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
            '    If HttpContext.Current.Session.Item("GA_APLICATRD_DOCUMENTOS") = 0 Then
            '        Mens.Showscripman("El usuario no tiene permisos para retaurar trd al documento", update)
            '        'sender.focus()
            '        Exit Sub
            '    End If
            '    Dim resulta As String = ""
            '    Dim refclas_inventario As New ClassGaGestionInventario
            '    If Hidden_id_inventario.Value <> 0 Then
            '        resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '        If resulta <> "YES" Then
            '            Mens.Showscripman(resulta, update)
            '            'sender.focus()
            '            Exit Sub
            '        End If
            '    End If
            'End If
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally
            'toltip.SetToolTip(sender, "Selecciona tabla retención documental")
        End Try
    End Sub
    Private Sub comman_expediente_restore_clik(ByVal sender As  _
       System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim update As UpdatePanel = sender.page.findcontrol("UpdatePanel_consulta")
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Try
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Mens.Showscripman("El usuario docuarchi no tiene usuario de gestión relacionado", update)
                'sender.focus()
                Exit Sub
            End If
            'If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
            '    If HttpContext.Current.Session.Item("GA_ASIGNA_EXPEDIENTE_DOCUMENTOS") = 0 Then
            '        Mens.Showscripman("El usuario no tiene permisos para restaurar expediente al documento", update)
            '        'sender.focus()
            '        Exit Sub
            '    End If
            '    Dim resulta As String = ""
            '    Dim refclas_inventario As New ClassGaGestionInventario
            '    If Hidden_id_inventario.Value <> "0" Then
            '        resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '        If resulta <> "YES" Then
            '            Mens.Showscripman(resulta, update)
            '            'sender.focus()
            '            Exit Sub
            '        End If
            '    End If
            'End If
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally
            'toltip.SetToolTip(sender, "Selecciona unidad compleja, compuesta (Expediente)")
        End Try
    End Sub
    Private Sub comman_expediente_clik(ByVal sender As  _
       System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim update As UpdatePanel = sender.page.findcontrol("UpdatePanel_consulta")
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Try
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Mens.Showscripman("El usuario docuarchi no tiene usuario de gestión relacionado", update)
                Exit Sub
            End If
            'If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
            '    If HttpContext.Current.Session.Item("GA_ASIGNA_EXPEDIENTE_DOCUMENTOS") = 0 Then
            '        Mens.Showscripman("El usuario no tiene permisos para asignar expediente al documento", update)
            '        'sender.focus()
            '        Exit Sub
            '    End If
            '    Dim resulta As String = ""
            '    Dim refclas_inventario As New ClassGaGestionInventario
            '    If Hidden_id_inventario.Value <> "0" Then
            '        resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '        If resulta <> "YES" Then
            '            Mens.Showscripman(resulta, update)
            '            'sender.focus()
            '            Exit Sub
            '        End If
            '    End If
            'End If
            'Dim Refclas As New ClassAdmonEmpresa
            'Dim Result As String = ""
            'If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 1 Then
            '    'Result = Refclas.Listar_Empresa_de_Gestion_Activa(FormGaGestionExpediente.ComboBoxEntidadEmpresa, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '    'If Result <> "YES" Then
            '    '    Mens.Showscripman(Result, update)
            '    '    Exit Sub
            '    'End If

            '    'Dim clasadmonempresa As New ClassAdmonEmpresa
            '    'Dim empresa_usuario_gestion As String = ""
            '    'Result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '    'If Result <> "YES" Then
            '    '    Mens.Showscripman(Result, update)
            '    '    sender.focus()
            '    '    Exit Sub
            '    'End If
            '    'If FormGaGestionExpediente.ComboBoxEntidadEmpresa.Items.Count > 0 Then
            '    '    FormGaGestionExpediente.ComboBoxEntidadEmpresa.Text = empresa_usuario_gestion
            '    'End If
            'Else
            '    'Result = Refclas.Listar_Empresa_de_Gestion_Activa(FormGaGestionExpediente.ComboBoxEntidadEmpresa, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '    'If Result <> "YES" Then
            '    '    MsgBox(Result, MsgBoxStyle.Information)
            '    '    Exit Sub
            '    'End If
            '    'Dim clasadmonempresa As New ClassAdmonEmpresa
            '    'Dim empresa_usuario_gestion As String = ""
            '    'Result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '    'If Result <> "YES" Then
            '    '    Mens.Showscripman(Result, update)
            '    '    sender.focus()
            '    '    Exit Sub
            '    'End If
            '    'If FormGaGestionExpediente.ComboBoxEntidadEmpresa.Items.Count > 0 Then
            '    '    FormGaGestionExpediente.ComboBoxEntidadEmpresa.Text = empresa_usuario_gestion
            '    'End If
            'End If
            Dim ref_Iframe_expdiente_popup = sender.page.findcontrol("Iframe_expdiente_popup_")
            ref_Iframe_expdiente_popup.Attributes.Add("src", "../gestion/WebFormGaGestionExpediente.aspx")
            Dim ref_UpdatePanel_expdiente_popup = sender.page.findcontrol("UpdatePanel_expdiente_popup")
            ref_UpdatePanel_expdiente_popup.Update()
            Dim ref_ModalPopupExtende_expdiente_popup = sender.page.findcontrol("ModalPopupExtende_expdiente_popup")
            ref_ModalPopupExtende_expdiente_popup.Show()
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally
            'toltip.SetToolTip(sender, "Selecciona unidad compleja, compuesta (Expediente)")
        End Try
    End Sub
    Private Sub comman_tipo_documemnto_restore_clik(ByVal sender As  _
      System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim update As UpdatePanel = sender.page.findcontrol("UpdatePanel_consulta")
        Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
        Try
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Mens.Showscripman("El usuario docuarchi no tiene usuario de gestión relacionado", update)
                ref_Hidden_resultado.value = ""
                ref_Updatepanel_actualiza.Update()
                'sender.focus()
                Exit Sub
            End If
            'If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
            '    If HttpContext.Current.Session.Item("GA_SELECCIONA_CLASE_DOCUMENTOS") = 0 Then
            '        Mens.Showscripman("El usuario no tiene permisos para restaurar la clase de documento", update)
            '        ref_Hidden_resultado.value = ""
            '        ref_Updatepanel_actualiza.Update()
            '        'sender.focus()
            '        Exit Sub
            '    End If
            '    Dim resulta As String = ""
            '    Dim refclas_inventario As New ClassGaGestionInventario
            '    If Hidden_id_inventario.Value <> 0 Then
            '        resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '        If resulta <> "YES" Then
            '            ref_Hidden_resultado.value = ""
            '            ref_Updatepanel_actualiza.Update()
            '            Mens.Showscripman(resulta, update)
            '            'sender.focus()
            '            Exit Sub
            '        End If
            '    End If
            'End If
            ref_Hidden_resultado.value = "YES"
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally

        End Try
    End Sub
    Private Sub comman_tipo_documemnto_clik(ByVal sender As  _
      System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim ref_Hidden_valor_seleccion As HtmlInputHidden = sender.page.findcontrol("Hidden_valor_seleccion")
        Dim update As UpdatePanel = sender.page.findcontrol("UpdatePanel_consulta")
        Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
        Dim ref_ModalPopupExtende_tipo_popup As ModalPopupExtender = sender.page.findcontrol("ModalPopupExtende_tipo_popup")
        Try
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Mens.Showscripman("El usuario docuarchi no tiene usuario de gestión relacionado", update)
                ref_Hidden_resultado.value = ""
                ref_Updatepanel_actualiza.Update()
                'sender.focus()
                Exit Sub
            End If
            'If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
            '    If HttpContext.Current.Session.Item("GA_SELECCIONA_CLASE_DOCUMENTOS") = 0 Then
            '        Mens.Showscripman("El usuario no tiene permisos para seleccionar la clase de documento", update)
            '        ref_Hidden_resultado.value = ""
            '        ref_Updatepanel_actualiza.Update()
            '        'sender.focus()
            '        Exit Sub
            '    End If
            '    Dim resulta As String = ""
            '    Dim refclas_inventario As New ClassGaGestionInventario
            '    If Hidden_id_inventario.Value <> 0 Then
            '        resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            '        If resulta <> "YES" Then
            '            ref_Hidden_resultado.value = ""
            '            ref_Updatepanel_actualiza.Update()
            '            Mens.Showscripman(resulta, update)
            '            'sender.focus()
            '            Exit Sub
            '        End If
            '    End If
            'End If
            Dim drow_list As DropDownList = sender.page.findcontrol("ComboBoxtipo")
            Dim update_drow As UpdatePanel = sender.page.findcontrol("update_panel_drowlist")
            Dim ref_ModalPopupExtende_trd_popup = sender.page.findcontrol("ModalPopupExtende_tipo_popup")
            ref_ModalPopupExtende_trd_popup.Show()
            Dim refclas As New ClassGaTipoDocumental
            Dim Result As String = ""
            Dim matri() As String = {"DIGITALIZADO", "ELECTRONICO"}
            Result = refclas.Solicita_tipos_documentales_combo_excluyentes(drow_list, matri, ref_Hidden_valor_seleccion.Value, update_drow)
            If Result <> "YES" Then
                Mens.Showscripman(Result, update)
                Exit Sub
            End If
            ref_ModalPopupExtende_tipo_popup.Show()
            ref_Hidden_resultado.value = "YES"
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally

        End Try
    End Sub
    Function Retorna_id_expediente_documento(ByVal gabinete As String, _
                                             ByVal id_imagen As Integer, _
                                             ByRef id_expdiente As Integer) As String
        '------------------------------------------------------
        'Funcion : Retorna id expediente documento
        'Fecha : 2016-01-27
        'Ing Miguel Angel Urueta Miranda
        '-----------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassTrdDocumental
            Dim option_tabla As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_tabla, _
                                                                               gabinete)
            If Result <> "YES" Then
                Retorna_id_expediente_documento = Result
                Exit Function
            End If

            Dim Parametro_Consulta As String = "Select ID_EXPEDIENTE from " & gabinete & " where ID='" & id_imagen & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
            Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_expediente_documento = "Funcion  Retorna_id_expediente_documento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_expediente_documento = "Imposible encontrar registro para el documento "
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    Retorna_id_expediente_documento = "El documento no está archivado, imposible mostrar ubicación toponímica "
                    Exit Function
                Else
                    id_expdiente = Datset.Tables(0).Rows(0).Item(0)
                    Retorna_id_expediente_documento = "YES"
                    Exit Function
                End If

            End If
        Catch ex As Exception
            Retorna_id_expediente_documento = "Inconsistencia general función Retorna_id_expediente_documento " & ex.Message
        End Try
    End Function

    Function Retorna_datos_sistema_imagen_info(ByVal Id_Imagen As Long, _
                                               ByVal Nombre_Gabinete As String, _
                                               ByRef Matri_Datos_Sistema() As String) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "Select ID,DISC,PAG,DBT,IDEX,USER,DATE1,TIME1 from " & Nombre_Gabinete & " where id=" & Id_Imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_IMAGEN")
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_datos_sistema_imagen_info = "Funcion  Retorna_datos_sistema_imagen_info Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                ReDim Preserve Matri_Datos_Sistema(0)
                Matri_Datos_Sistema(0) = "Indice Imagen " & Datset.Tables(0).Rows(0).Item(0)
                ReDim Preserve Matri_Datos_Sistema(1)
                Matri_Datos_Sistema(1) = "Disco Imagen " & Datset.Tables(0).Rows(0).Item(1)
                ReDim Preserve Matri_Datos_Sistema(2)
                Matri_Datos_Sistema(2) = "Numero Paginas " & Datset.Tables(0).Rows(0).Item(2)
                ReDim Preserve Matri_Datos_Sistema(3)
                Matri_Datos_Sistema(3) = "Estado Documento " & Datset.Tables(0).Rows(0).Item(3)
                ReDim Preserve Matri_Datos_Sistema(4)
                Matri_Datos_Sistema(4) = "Carpeta Imagen " & Datset.Tables(0).Rows(0).Item(4)
                ReDim Preserve Matri_Datos_Sistema(5)
                Matri_Datos_Sistema(5) = "Usuario Almacenamiento " & Datset.Tables(0).Rows(0).Item(5)
                ReDim Preserve Matri_Datos_Sistema(6)
                Matri_Datos_Sistema(6) = "Fecha Almacenamiento " & Datset.Tables(0).Rows(0).Item(6)
                ReDim Preserve Matri_Datos_Sistema(7)
                Matri_Datos_Sistema(7) = "Hora Almacenamiento " & Datset.Tables(0).Rows(0).Item(7)
                Retorna_datos_sistema_imagen_info = "YES"
                Exit Function
            Else
                Retorna_datos_sistema_imagen_info = "Imposible encontrar datos de la imagen " & Id_Imagen & " en el gabinete " & Nombre_Gabinete
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_sistema_imagen_info = "Inconsistencia función Retorna_datos_sistema_imagen_info " & ex.Message
        End Try
    End Function
    Function Datos_Sitema_de_Imagen(ByVal Id_Imagen As Long, _
                                    ByVal Nombre_Gabinete As String, _
                                    ByRef Matri_Datos_Sistema() As String) As String
        '************************************************
        'Funcion : Datos_Sitema_de_Imagen
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2010-11-02
        'Descripcion : Retorna los datos del 
        'sistema de el id de la imagen solicitada
        'Modificacion : El la funcion esta forma
        'tenado los campos fecha  DIA MOD 2010-11-09
        'Modificacion DIA 2011-11-10 se agrega funcnion
        'campos obligatorios para ordenar la matriz de 
        'de datos que devuelbe la funcion datos sistema
        'Modificada : 2014-05-03, se modifica el modo de
        'conexion a la base de datos para adaptarlo al modulo
        'web
        'Ing : Miguel Angel Urueta Miranda
        '*****************************************************
        Try
            '*******************************************************************
            'Consulta orden campos de cosnulta desde la tabla detalle_gabinete
            '*******************************************************************
            Dim Matri_Campos_Obli() As String
            Erase Matri_Campos_Obli
            Dim Refalmacena As New ClassAlmacenamiento
            Dim ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim Result As String = ref_Class_DETALLE_GABIENETE.Consulta_Campos_Obligatorio(Nombre_Gabinete, _
                                                                                           Matri_Campos_Obli)
            If Result <> "YES" Then
                Datos_Sitema_de_Imagen = "Error Funcion Mother Datos_Sitema_de_Imagen " & vbCrLf & _
                " Consultando orden campos en la tabla detalle_gabinete Error de la Funcion Consulta_Campos_Obligatorio :" & Result
                Exit Function
            End If
            Dim Campos_Insert As String = "ID,DISC,PAG,DBT,IDEX,USER,DATE1,TIME1"
            For z2 As Integer = 0 To UBound(Matri_Campos_Obli)
                Dim Matri_Tempo() As String
                Erase Matri_Tempo
                Matri_Tempo = Matri_Campos_Obli(z2).Split("|")
                Campos_Insert = Campos_Insert & "," & Matri_Tempo(1).ToString
            Next
            Dim Parametro_Consulta As String = "select " & Campos_Insert & " from  " & Nombre_Gabinete & _
                       "  where id = '" & Id_Imagen & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_IMAGEN")
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Datos_Sitema_de_Imagen = "Funcion  Datos_Sitema_de_Imagen Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Datos_Sitema_de_Imagen = " Imposible encontrar datos del sistema de la imagen "
                Exit Function
            Else
                Dim typde As String = ""
                Erase Matri_Datos_Sistema
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    ReDim Preserve Matri_Datos_Sistema(i)
                    typde = Datset.Tables(0).Columns(i).GetType.ToString
                    Dim obtype As Object = Datset.Tables(0).Rows(0).Item(i)
                    If IsDBNull(obtype) = True Then
                        Matri_Datos_Sistema(i) = ""
                    Else
                        If typde = "System.DateTime" Then
                            Dim SplitWf() As String = Left(Datset.Tables(0).Rows(0).Item(i).ToString, 10).Split("/")
                            If Not SplitWf Is Nothing Then
                                Matri_Datos_Sistema(i) = SplitWf(2) & "/" & SplitWf(1) & "/" & SplitWf(0)
                            Else
                                Matri_Datos_Sistema(i) = "#Error"
                            End If
                        Else
                            Matri_Datos_Sistema(i) = Datset.Tables(0).Rows(0).Item(i)
                        End If
                    End If
                Next
                Datos_Sitema_de_Imagen = "YES"
            End If
        Catch ex As Exception
            Datos_Sitema_de_Imagen = ex.Message
        End Try
    End Function
    Function verifica_exitencia_valor_invnetario_gabinete(ByVal nombre_gabinete As String,
                                                          ByVal id_imagen As Integer,
                                                          ByRef id_invnetario As Long) As String
        '*******************************************************************
        'Retorna valor inventario en el gabinete
        'fecha : 2015-02-15
        'Ing : Miguel Angel Urueta Miranda
        'Modificado para la versión web por Ing Miguel Angel Urueta Miranda
        '*******************************************************************
        Try
            Dim Parametro_Consulta As String = "Select ID_INVENTARIO_DOCUMENTAL from " & nombre_gabinete & " where id='" & id_imagen & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_IMAGEN")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                verifica_exitencia_valor_invnetario_gabinete = " Error consultando existencia inventario   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                verifica_exitencia_valor_invnetario_gabinete = "Imposible encontrar id de la imagen en el inventario"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    id_invnetario = Datset.Tables(0).Rows(0).Item(0)
                Else
                    id_invnetario = 0
                End If
                verifica_exitencia_valor_invnetario_gabinete = "YES"
                Exit Function
            End If

        Catch ex As Exception
            verifica_exitencia_valor_invnetario_gabinete = "Inconsistencia función  verifica_exitencia_valor_invnetario_gabinete " & ex.Message
        End Try
    End Function
End Class
