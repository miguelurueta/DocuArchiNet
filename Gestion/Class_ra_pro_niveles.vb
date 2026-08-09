Public Structure stru_niveles
    Dim id_nivel As Integer
    Dim remit_dest_interno_id_Remit_Dest_Int As Integer
    Dim nivel As Integer
    Dim nombre_nivel As String
    Dim id_nivel_padre As Integer
    Dim estado_padre As Integer
    Dim conta_expediente As Integer
    Dim estado_nivel As Integer
    Dim contador_nivel As Integer
    Dim estado_nivel_compartido As Integer
    Dim value_path As String
End Structure
Public Structure stru_niveles_hijo
    Dim id_nivel As Integer
    Dim estado_revisa As Integer
    Dim estado_repetido As Integer
    Dim numero_expediente As Integer
End Structure
Public Class Class_ra_pro_niveles
    Function Lista_niveles_ocultos(ByVal id_usuario_gestion As Integer, _
                                   ByVal tipo_consulta As Integer, _
                                   ByVal valor_consulta As String, _
                                   ByRef colum_order_name As String, _
                                   ByRef order_colum As String, _
                                   ByRef labetitle As Label, _
                                   ByRef scripma As GridView, _
                                   ByRef hideselecion As HtmlInputHidden, _
                                   ByRef updat As UpdatePanel) As String
        Try
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "SELECT id_nivel,nombre_nivel as NOMBRE_NIVEL " & _
                    " from ra_pro_niveles " & _
                    " where remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion & _
                    " and estado_nivel_oculto_padre = 0 order by  " & colum_order_name & " " & order_colum
            Else
                sql_consulta = "SELECT id_nivel,nombre_nivel as NOMBRE_NIVEL " & _
                    " from ra_pro_niveles " & _
                    " where (" & _
                    "  nombre_nivel like '%" & valor_consulta & "%')" & _
                    " and remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion & _
                    " and estado_nivel_oculto_padre=0 order by  " & colum_order_name & " " & order_colum
            End If
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido") = {"OPCIONES", "id_nivel", _
                                                                               "NOMBRE_NIVEL"}
            HttpContext.Current.Session.Item("SortExpression_publico") = colum_order_name
            HttpContext.Current.Session.Item("SortDirection_publico") = order_colum
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_PUBLICO") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_PUBLICO") = sql_consulta
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_rad_estados_modulo_radicacion")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_niveles_ocultos = "Error fucion Lista_radicados_pendientes  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) "
                scripma.DataSource = Datset
                hideselecion.Value = "-1"
                scripma.DataBind()
                updat.Update()
                Lista_niveles_ocultos = "YES"
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
                    ihtml.Attributes.Add("class", "fad fa-arrow-to-bottom")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_lista_nivel(event,this);")
                    ahtml.Attributes.Add("title", "Agrega nivel oculto a la estructuctura")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "a_s_r_p_333")
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
                Result = Refclas.add_clase_acender_decender(colum_order_name, _
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_compartido"), _
                                                            order_colum, _
                                                            scripma)
                If Result <> "YES" Then
                    Lista_niveles_ocultos = "Error add clase funcion  Lista_radicados_pendientes_interface " & Result
                    Exit Function
                End If
            End If
            Lista_niveles_ocultos = "YES"
        Catch ex As Exception
            Lista_niveles_ocultos = "Inconsistencia general función Lista_niveles_ocultos " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_nodo_principal(ByVal id_usuario_gestion As Integer, _
                                                ByRef estado_existencia_nivel As String) As String
        Try
            Dim Parametro_Consulta = "select id_nivel " & _
            " from ra_pro_niveles WHERE remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_nodo_principal = "Funcion  Solicita_existencia_nodo_principal dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_existencia_nivel = "NO"
                Solicita_existencia_nodo_principal = "YES"
                Exit Function
            Else
                estado_existencia_nivel = "YES"
                Solicita_existencia_nodo_principal = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_nodo_principal = "Inconsistencia general función Solicita_existencia_nodo_principal " & ex.Message
        End Try
    End Function
    Function Solicita_nivel_nodo(ByVal id_nivel As Integer, _
                                 ByRef numero_nivel As Integer) As String
        Try
            Dim Parametro_Consulta = "select nivel " & _
         " from ra_pro_niveles WHERE id_nivel=" & id_nivel
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nivel_nodo = "Funcion Solicita_numero_nivel dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_nivel = 0
                Solicita_nivel_nodo = "Imposible encontrar el numero del nivel (" & id_nivel & ")"
                Exit Function
            Else
                numero_nivel = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nivel_nodo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nivel_nodo = "Inconsistencia general función Solicita_nivel_nodo " & ex.Message
        End Try
    End Function
    Function Solicita_niveles_organizacion(ByVal id_usuario_gestion As Integer, _
                                           ByRef stru_nieves() As stru_niveles) As String
        Try
            Dim Parametro_Consulta = "select id_nivel,remit_dest_interno_id_Remit_Dest_Int,nivel," & _
                "nombre_nivel,id_nivel_padre, estado_padre,conta_expediente,estado_nivel, contador_nivel, estado_nivel_compartido " & _
            " from ra_pro_niveles WHERE remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion & _
            " and estado_nivel=1"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_niveles_organizacion = "Funcion Solicita_niveles_organizacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_nieves = Nothing
                Solicita_niveles_organizacion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_nieves(i)
                    stru_nieves(i).id_nivel = Datset.Tables(0).Rows(i).Item(0)
                    stru_nieves(i).remit_dest_interno_id_Remit_Dest_Int = Datset.Tables(0).Rows(i).Item(1)
                    stru_nieves(i).nivel = Datset.Tables(0).Rows(i).Item(2)
                    stru_nieves(i).nombre_nivel = Datset.Tables(0).Rows(i).Item(3)
                    stru_nieves(i).id_nivel_padre = Datset.Tables(0).Rows(i).Item(4)
                    stru_nieves(i).estado_padre = Datset.Tables(0).Rows(i).Item(5)
                    stru_nieves(i).conta_expediente = Datset.Tables(0).Rows(i).Item(6)
                    stru_nieves(i).estado_nivel = Datset.Tables(0).Rows(i).Item(7)
                    stru_nieves(i).contador_nivel = Datset.Tables(0).Rows(i).Item(8)
                    stru_nieves(i).estado_nivel_compartido = Datset.Tables(0).Rows(i).Item(9)
                Next
                Solicita_niveles_organizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_niveles_organizacion = "Inconsistencia general función Solicita_niveles_organizacion " & ex.Message
        End Try
    End Function
    Function Solicita_niveles_organizacion_compartidos(ByVal id_usuario_gestion As Integer, _
                                                       ByRef stru_nieves() As stru_niveles) As String
        Try
            Dim Parametro_Consulta = "select pro_nivel.id_nivel,pro_nivel.remit_dest_interno_id_Remit_Dest_Int,pro_nivel.nivel," & _
                "pro_nivel.nombre_nivel,pro_nivel.id_nivel_padre, pro_nivel.estado_padre,pro_nivel.conta_expediente,pro_nivel.estado_nivel," & _
                "pro_nivel.contador_nivel, pro_nivel.estado_nivel_compartido from ra_pro_permisos_niveles as ra_pro " & _
                "inner join ra_pro_niveles as pro_nivel on (pro_nivel.id_nivel=ra_pro.ra_pro_niveles_id_nivel ) " & _
                "WHERE ra_pro.remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion & "    order by pro_nivel.id_nivel"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_niveles_organizacion_compartidos = "Funcion Solicita_niveles_organizacion_compartidos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_nieves = Nothing
                Solicita_niveles_organizacion_compartidos = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_nieves(i)
                    stru_nieves(i).id_nivel = Datset.Tables(0).Rows(i).Item(0)
                    stru_nieves(i).remit_dest_interno_id_Remit_Dest_Int = Datset.Tables(0).Rows(i).Item(1)
                    stru_nieves(i).nivel = Datset.Tables(0).Rows(i).Item(2)
                    stru_nieves(i).nombre_nivel = Datset.Tables(0).Rows(i).Item(3)
                    stru_nieves(i).id_nivel_padre = Datset.Tables(0).Rows(i).Item(4)
                    stru_nieves(i).estado_padre = Datset.Tables(0).Rows(i).Item(5)
                    stru_nieves(i).conta_expediente = Datset.Tables(0).Rows(i).Item(6)
                    stru_nieves(i).estado_nivel = Datset.Tables(0).Rows(i).Item(7)
                    stru_nieves(i).contador_nivel = Datset.Tables(0).Rows(i).Item(8)
                    stru_nieves(i).estado_nivel_compartido = Datset.Tables(0).Rows(i).Item(9)
                Next
                Solicita_niveles_organizacion_compartidos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_niveles_organizacion_compartidos = "Inconsistencia general función Solicita_niveles_organizacion " & ex.Message
        End Try
    End Function
    Function Solicita_niveles_hijos(ByVal id_nivel_padre As Integer, _
                                    ByRef stru_nieves() As stru_niveles) As String
        Try
            Dim Parametro_Consulta = "select id_nivel,remit_dest_interno_id_Remit_Dest_Int,nivel," & _
                "nombre_nivel,id_nivel_padre, estado_padre,conta_expediente,estado_nivel, contador_nivel, estado_nivel_compartido " & _
            " from ra_pro_niveles WHERE id_nivel_padre=" & id_nivel_padre
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_niveles_hijos = "Funcion Solicita_niveles_hijos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_nieves = Nothing
                Solicita_niveles_hijos = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_nieves(i)
                    stru_nieves(i).id_nivel = Datset.Tables(0).Rows(i).Item(0)
                    stru_nieves(i).remit_dest_interno_id_Remit_Dest_Int = Datset.Tables(0).Rows(i).Item(1)
                    stru_nieves(i).nivel = Datset.Tables(0).Rows(i).Item(2)
                    stru_nieves(i).nombre_nivel = Datset.Tables(0).Rows(i).Item(3)
                    stru_nieves(i).id_nivel_padre = Datset.Tables(0).Rows(i).Item(4)
                    stru_nieves(i).estado_padre = Datset.Tables(0).Rows(i).Item(5)
                    stru_nieves(i).conta_expediente = Datset.Tables(0).Rows(i).Item(6)
                    stru_nieves(i).estado_nivel = Datset.Tables(0).Rows(i).Item(7)
                    stru_nieves(i).contador_nivel = Datset.Tables(0).Rows(i).Item(8)
                    stru_nieves(i).estado_nivel_compartido = Datset.Tables(0).Rows(i).Item(9)
                Next
                Solicita_niveles_hijos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_niveles_hijos = "Inconsistencia general función Solicita_niveles_hijos " & ex.Message
        End Try
    End Function
    Function Solicita_estado_nivel_propietario(ByVal id_usuario_gestion As Integer, _
                                               ByVal id_nivel As Integer, _
                                               ByRef estado_propietario As String) As String
        Try
            Dim Parametro_Consulta = "select id_nivel " & _
           " from ra_pro_niveles WHERE remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion & _
           " and id_nivel=" & id_nivel
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_nivel_propietario = "Funcion  Solicita_estado_nivel_propietario dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_propietario = "NO"
                Solicita_estado_nivel_propietario = "YES"
                Exit Function
            Else
                estado_propietario = "YES"
                Solicita_estado_nivel_propietario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_nivel_propietario = "Inconsistencia general función Solicita_estado_nivel_propietario " & ex.Message
        End Try
    End Function
    Function Solicita_id_nivel_padre(ByVal id_nivel As Integer, _
                                     ByRef id_nivel_padre As Integer) As String
        Try
            Dim Parametro_Consulta = "select id_nivel_padre " & _
            " from ra_pro_niveles WHERE id_nivel=" & id_nivel
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_nivel_padre = "Funcion  Solicita_id_nivel_padre dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_nivel_padre = 0
                Solicita_id_nivel_padre = "Imposible encontrar el registro del nivel (" & id_nivel & ")"
                Exit Function
            Else
                id_nivel_padre = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_nivel_padre = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_nivel_padre = "Inconsistencia general función Solicita_id_nivel_padre " & ex.Message
        End Try
    End Function
    Function Solicita_id_nivel_padre(ByRef id_nivel As Integer) As String
        Try
            Dim Parametro_Consulta = "select id_nivel_padre " & _
            " from ra_pro_niveles WHERE id_nivel=" & id_nivel
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_nivel_padre = "Funcion  Solicita_id_nivel_padre dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                'id_nivel_padre = 0
                Solicita_id_nivel_padre = "Imposible encontrar el registro del nivel (" & id_nivel & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).Item(0) = 0 Then
                    Solicita_id_nivel_padre = "Imposible encontrar el registro del nivel (" & id_nivel & ")"
                    Exit Function
                End If
                id_nivel = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_nivel_padre = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_nivel_padre = "Inconsistencia general función Solicita_id_nivel_padre " & ex.Message
        End Try
    End Function
    Function Solicita_id_nivel_padre_raiz(ByRef id_nivel As Integer) As Integer
        Try
            Dim Result As String = ""
            Dim ID_NIVEL_PADRE As Integer = 0
            Result = Me.Solicita_id_nivel_padre(id_nivel)
            If Result <> "YES" Then
                Solicita_id_nivel_padre_raiz = id_nivel
                Exit Function
            Else
                Solicita_id_nivel_padre_raiz(id_nivel)
            End If
        Catch ex As Exception

        End Try
    End Function
    Function Solicita_niveles_relacionados_padre_recursive(ByRef id_nivel_padre As Integer, _
                                                           ByRef matri_permanente() As stru_niveles_hijo, _
                                                           ByVal id_usuario_gestion As Integer) As String
        Try
            Dim Result As String = ""
            If matri_permanente Is Nothing Then
                Result = Me.Solicita_nodos_niveles_padre(id_nivel_padre, _
                                                         matri_permanente, _
                                                         id_usuario_gestion)
                If Result <> "YES" Then
                    Return Solicita_niveles_relacionados_padre_recursive = Result
                    Exit Function
                End If
                If matri_permanente Is Nothing Then
                    Solicita_niveles_relacionados_padre_recursive = "YES"
                    Exit Function
                End If
                '---------------------------------------
                'Solicita los niveles relacionados
                '---------------------------------------

                If matri_permanente(0).estado_revisa = 0 Then
                    Dim ref_id_nivel_padre As Integer = matri_permanente(0).id_nivel
                    Result = Me.Solicita_niveles_relacionados_padre_recursive(ref_id_nivel_padre, _
                                                                              matri_permanente, _
                                                                              id_usuario_gestion)
                    If Result <> "YES" Then
                        Solicita_niveles_relacionados_padre_recursive = "YES"
                        Exit Function
                    End If
                    matri_permanente(0).estado_revisa = 1
                End If

            Else
                For i As Integer = 0 To matri_permanente.Length - 1
                    If matri_permanente(i).estado_revisa = 0 Then
                        Dim ref_id_nivel_padre As Integer = matri_permanente(i).id_nivel
                        Result = Me.Solicita_nodos_niveles_padre(ref_id_nivel_padre, _
                                                                 matri_permanente, _
                                                                 id_usuario_gestion)
                        If Result <> "YES" Then
                            Solicita_niveles_relacionados_padre_recursive = "YES"
                            Exit Function
                        End If
                        matri_permanente(i).estado_revisa = 1
                    End If
                Next
                Dim estado_salida As Integer = 0
                For i As Integer = 0 To matri_permanente.Length - 1
                    If matri_permanente(i).estado_revisa = 0 Then
                        estado_salida = 1
                    End If
                Next
                If estado_salida = 0 Then
                    Return Solicita_niveles_relacionados_padre_recursive = "YES"
                    Exit Function
                Else
                    Dim ref_id_nivel_padre As Integer = matri_permanente(0).id_nivel
                    Result = Me.Solicita_niveles_relacionados_padre_recursive(ref_id_nivel_padre, _
                                                                              matri_permanente, _
                                                                              id_usuario_gestion)
                    If Result <> "YES" Then
                        Solicita_niveles_relacionados_padre_recursive = Result
                        Exit Function
                    End If
                End If
            End If
        Catch ex As Exception
            Solicita_niveles_relacionados_padre_recursive = "Inconsistencia general función Solicita_niveles_relacionados_padre_recursive " & ex.Message
        End Try
    End Function
    Function Solicita_nodos_niveles_padre(ByRef id_nivel_padre As Integer, _
                                          ByRef matri_permanente() As stru_niveles_hijo, _
                                          ByVal id_usuario_gestion As Integer) As String
        Try
            Dim Parametro_Consulta = "select id_nivel,rppn.id_permisos_niveles, rapn.conta_expediente " & _
           " from ra_pro_niveles as rapn " & _
           " left outer join ra_pro_permisos_niveles as rppn on (rppn.ra_pro_niveles_id_nivel " & _
           " = rapn.id_nivel and rppn.remit_dest_interno_id_Remit_Dest_Int=" & id_usuario_gestion & ")" & _
           " WHERE id_nivel_padre=" & id_nivel_padre
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nodos_niveles_padre = "Funcion  Solicita_nodos_niveles_padre dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nodos_niveles_padre = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If matri_permanente Is Nothing Then
                        ReDim Preserve matri_permanente(i)
                        matri_permanente(i).id_nivel = Datset.Tables(0).Rows(i).Item(0)
                        matri_permanente(i).estado_revisa = 0
                        If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                            matri_permanente(i).estado_repetido = 0
                        Else
                            matri_permanente(i).estado_repetido = Datset.Tables(0).Rows(i).Item(1)
                        End If
                        If Datset.Tables(0).Rows(i).IsNull(2) = True Then
                            matri_permanente(i).numero_expediente = 0
                        Else
                            matri_permanente(i).numero_expediente = Datset.Tables(0).Rows(i).Item(2)
                        End If
                    Else
                        Dim matri_leng As Integer = UBound(matri_permanente) + 1
                        ReDim Preserve matri_permanente(matri_leng)
                        matri_permanente(matri_leng).id_nivel = Datset.Tables(0).Rows(i).Item(0)
                        matri_permanente(matri_leng).estado_revisa = 0
                        If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                            matri_permanente(matri_leng).estado_repetido = 0
                        Else
                            matri_permanente(matri_leng).estado_repetido = Datset.Tables(0).Rows(i).Item(1)
                        End If
                        If Datset.Tables(0).Rows(i).IsNull(2) = True Then
                            matri_permanente(matri_leng).numero_expediente = 0
                        Else
                            matri_permanente(matri_leng).numero_expediente = Datset.Tables(0).Rows(i).Item(2)
                        End If
                    End If
                Next
                Solicita_nodos_niveles_padre = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nodos_niveles_padre = "Incosistencia general función ita_nodos_niveles_padre " & ex.Message
        End Try
    End Function
    Function Solicita_numero_expediente_nivel(ByVal id_nivel As Integer, _
                                              ByRef numero_expedientes As Integer) As String
        Try
            Dim Parametro_Consulta = "select conta_expediente " & _
           " from ra_pro_niveles WHERE id_nivel=" & id_nivel
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_expediente_nivel = "Funcion  Solicita_numero_expediente_nivel dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_expedientes = 0
                Solicita_numero_expediente_nivel = "Imposible encontrar el numero de expedientes del nivel (" & id_nivel & ")"
                Exit Function
            Else
                numero_expedientes = Datset.Tables(0).Rows(0).Item(0)
                Solicita_numero_expediente_nivel = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_expediente_nivel = "Inconsistencia general función Solicita_numero_expediente_nivel " & ex.Message
        End Try
    End Function
    Function Solicita_numero_niveles_hijos(ByVal id_nivel_padre As Integer, _
                                           ByRef numero_nivel_hijo_padre As Integer) As String
        Try
            Dim Parametro_Consulta = "select id_nivel_padre " & _
            " from ra_pro_niveles WHERE id_nivel_padre=" & id_nivel_padre
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_niveles_hijos = "Funcion  Solicita_numero_niveles_hijos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_nivel_padre = 0
                Solicita_numero_niveles_hijos = "YES"
                Exit Function
            Else
                id_nivel_padre = Datset.Tables(0).Rows.Count
                Solicita_numero_niveles_hijos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_niveles_hijos = "Inconsistencia general función Solicita_numero_niveles_hijos " & ex.Message
        End Try
    End Function
    Function Solicita_usuario_propietario_nivel(ByVal id_nivel As Integer, _
                                                ByRef nombre_propietario As String, _
                                                ByRef cargo_propietario As String, _
                                                ByRef id_Remit_Dest_Int As Integer) As String
        Try
            Dim Parametro_Consulta = "select rdi.Nombre_Remitente, rdi.Cargo_Remite, rdi.id_Remit_Dest_Int " & _
           " from ra_pro_niveles as rpn " & _
           " inner join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=rpn.remit_dest_interno_id_Remit_Dest_Int) " & _
           " WHERE id_nivel=" & id_nivel
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_niveles")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_usuario_propietario_nivel = "Funcion  Solicita_usuario_propietario_nivel dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_usuario_propietario_nivel = "Imposible encontrar el propietario del nivel (" & id_nivel & ")"
                Exit Function
            Else
                nombre_propietario = Datset.Tables(0).Rows(0).Item(0)
                cargo_propietario = Datset.Tables(0).Rows(0).Item(1)
                id_Remit_Dest_Int = Datset.Tables(0).Rows(0).Item(2)
                Solicita_usuario_propietario_nivel = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuario_propietario_nivel = "Inconsistencia general función Solicita_usuario_propietario_nivel " & ex.Message
        End Try
    End Function
End Class
