Imports System.IO
Imports MySql.Data
Imports System.Xml
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.IO.IsolatedStorage
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Public Structure unidad_conservacion
    Dim ID_UNIDAD_CONSERVACION As Integer
    Dim ENTRE_PAÑO_ID_ENTREPAÑO As Integer
    Dim CONSECUTIVO_UNIDAD_CONSERVACION As Integer
    Dim CONSECUTIVO_EXPEDIENTE As Integer
    Dim CONSECUTIVO_DOCUMENTO As Integer
    Dim CODIGO_CORTO As String
    Dim CODIGO_UNICO As String
    Dim TIPO_UNIDAD_CONSERVACION As Integer
    Dim NUMERO_FOLIO_UNIDAD_CONSERVACION As Integer
    Dim ID_USUARIO_GESTION As Integer
    Dim FECHA_CREACION As String
    Dim CODIGO_AREA_TRD As String
    Dim NOMBRE_AREA As String
    Dim CODIGO_SERIE As String
    Dim NOMBRE_SERIE As String
    Dim CODIGO_SUBSERIE As String
    Dim NOMBRE_SUBSERIE As String
    Dim ESTADO_UNIDAD_CONSERVACION As Integer
    Dim ESTADO_ARCHIVO_INIDAD As Integer
    Dim ID_TIPO_UNIDAD_CONSERVACION As Integer
    Dim FECHA_EXTREMA_INICIAL As String
    Dim FECHA_EXTREMA_FINAL As String
    Dim RANGO_EXTREMO_INICIAL As String
    Dim RANGO_EXTREMO_FINAL As String
    Dim TEMA_UNIDAD_CONSERVACION As String
    Dim DESCRIPCION_UNIDAD_CONSERVACION As String
    Dim CODIGO_BARRAS_UNIDAD As String
    Dim ID_EMPRESA As Integer
    Dim NOMBRE_TIPO_UNIDAD As String
    Dim IDEX As Integer
    Dim TAM_LETRA_TITULO As Integer
    Dim TAM_LETRA_DATOS_UNIDAD As Integer
    Dim TAM_LETRA_DATOS_TRD As Integer
    Dim TAM_LETRA_UBICACION As Integer
    Dim ID_UNIDAD_CONSERVACION_TOPLOGICA As Integer
    Dim UNIDAD_PADRE As Integer
    Dim VOLUMEN_UNIDAD_CONSERVACION As Integer
    Dim ID_SUB_AREA As Integer
    Dim NOMBRE_SUB_AREA As String
    Dim id_instrumento As Integer

End Structure
Public Class ClassUnidadConservacion
    Function Consulta_unidad_conservacion_post(ByRef update As UpdatePanel, ByRef hideselecion As Object, _
                                       ByRef HiddenEmailconsulta As Object, ByRef grediview As GridView, ByRef reflabel As Object) As String
        Try
            If HiddenEmailconsulta.value = "" Then
                Consulta_unidad_conservacion_post = "YES"
                Exit Function
            End If
            Dim sql_consulta As String = HiddenEmailconsulta.value
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("radicado")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_unidad_conservacion_post = "Error listando datos " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                'Dim label_act As Label = grediview.Page.FindControl("Label_scrool")
                'If Not update Is Nothing Then
                '    label_act.Text = "Paginación 1 de " & Datset.Tables(0).Rows.Count
                'End If
                HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) de expediente " &
                grediview.DataSource = Datset
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Consulta_unidad_conservacion_post = "YES"
                Exit Function
            Else
                'Dim label_act As Label = grediview.Page.FindControl("Label_scrool")
                'If Not update Is Nothing Then
                '    label_act.Text = "Paginación 1 de " & Datset.Tables(0).Rows.Count
                'End If
                HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) de expediente "
                'grediview.DataKeyNames = DataKey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                Next

                Consulta_unidad_conservacion_post = "YES"
                Exit Function
            End If
            Consulta_unidad_conservacion_post = "YES"
        Catch ex As Exception
            Consulta_unidad_conservacion_post = "Inconsistencia funcion Consulta_Expedientes_post " & ex.Message
        End Try
    End Function
    Function Limpia_campos_consulta_unidad_conservacion(ByRef panel_ref As Panel, ByRef update As UpdatePanel) As String
        Try
            Dim ref_DropDownListEstado_Expediente As DropDownList = panel_ref.FindControl("DropDownListEstado_Expediente")
            If Not ref_DropDownListEstado_Expediente Is Nothing Then
                ref_DropDownListEstado_Expediente.Text = "Todos"
            End If
            Dim ref_DropDownListUusuariocreador As DropDownList = panel_ref.FindControl("DropDownListUusuariocreador")
            If Not ref_DropDownListUusuariocreador Is Nothing Then
                ref_DropDownListUusuariocreador.Text = "Todos"
            End If
            Dim tipo As String = ""
            For Each ob As Object In panel_ref.Controls
                Dim g = ob.GetType().ToString
                tipo = tipo & ob.GetType().ToString & vbCrLf
                If ob.GetType().ToString = "System.Web.UI.WebControls.TextBox" Then
                    ob.text = ""
                End If
            Next
            update.Update()
            Limpia_campos_consulta_unidad_conservacion = "YES"
        Catch ex As Exception
            Limpia_campos_consulta_unidad_conservacion = "Inconsistencia función " & ex.Message
        End Try
    End Function
    Function Consulta_unidad_conservacion( _
      ByVal codigo_unico As String, ByVal fecha_creacion_ini As String, ByVal fecha_creacion_fin As String, _
      ByVal nombre_area As String, _
      ByVal nombre_serie As String, ByVal nombre_sub_serie As String, _
      ByVal fecha_extrema_incial As String, ByVal fecha_extrema_final As String, _
      ByVal rango_extremo_inicial As String, ByVal rango_extremo_final As String, _
      ByVal usuario_gestion As DropDownList, ByVal estado_unidad_documental As String, _
      ByRef grediview As GridView, _
      ByRef reflabel As Label, ByVal nombre_tipo_unidad_conservacion As String, _
       ByVal descripcion_unidad_conservacion As String, _
      ByVal id_empresa As Integer, ByRef update As UpdatePanel, ByRef hideselecion As Object, ByRef HiddenEmailconsulta As Object, ByVal option_expeidente_propio As Boolean, _
      ByVal option_descripcion As Boolean, ByVal option_tema As Boolean, ByVal tema_unidad_conservacion As String, _
      ByVal consecutivo_conservacion As String, ByVal sub_seccion As String, ByVal estado_archivado_unidad As String) As String
        Try
            Dim activaand As Integer = -1
            Dim sql_condicion As String = ""
            If codigo_unico <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where ID_UNIDAD_CONSERVACION='" & codigo_unico & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND ID_UNIDAD_CONSERVACION='" & codigo_unico & "'"
                End If

            End If
            If estado_archivado_unidad <> "Todos" Then
                If estado_archivado_unidad = "Archivados" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ENTRE_PAÑO_ID_ENTREPAÑO IS NOT  NULL "
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND ENTRE_PAÑO_ID_ENTREPAÑO IS NOT NULL "
                    End If
                End If
                If estado_archivado_unidad = "Sin archivar" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ENTRE_PAÑO_ID_ENTREPAÑO IS NULL "
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND ENTRE_PAÑO_ID_ENTREPAÑO IS NULL "
                    End If
                End If

            End If
            If consecutivo_conservacion <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where CODIGO_UNICO='" & consecutivo_conservacion & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND CODIGO_UNICO='" & consecutivo_conservacion & "'"
                End If

            End If
            If descripcion_unidad_conservacion <> "" Then
                Dim likeigual As String = "="
                If option_descripcion = True Then
                    If InStr(descripcion_unidad_conservacion, "%") <= 0 Then
                        descripcion_unidad_conservacion = "%" & descripcion_unidad_conservacion & "%"
                    End If
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where DESCRIPCION_UNIDAD_CONSERVACION like '" & descripcion_unidad_conservacion & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND DESCRIPCION_UNIDAD_CONSERVACION like '" & descripcion_unidad_conservacion & "'"
                    End If
                Else
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where DESCRIPCION_UNIDAD_CONSERVACION='" & descripcion_unidad_conservacion & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND DESCRIPCION_UNIDAD_CONSERVACION='" & descripcion_unidad_conservacion & "'"
                    End If
                End If


            End If
            If tema_unidad_conservacion <> "" Then
                Dim likeigual As String = "="
                If option_tema = True Then
                    If InStr(option_tema, "%") <= 0 Then
                        tema_unidad_conservacion = "%" & tema_unidad_conservacion & "%"
                    End If
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where TEMA_UNIDAD_CONSERVACION like '" & tema_unidad_conservacion & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND TEMA_UNIDAD_CONSERVACION like '" & tema_unidad_conservacion & "'"
                    End If
                Else
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where TEMA_UNIDAD_CONSERVACION='" & tema_unidad_conservacion & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND TEMA_UNIDAD_CONSERVACION='" & tema_unidad_conservacion & "'"
                    End If
                End If


            End If
            If fecha_creacion_ini <> "" And fecha_creacion_fin <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where FECHA_CREACION BETWEEN '" & fecha_creacion_ini & "' AND '" & _
                    fecha_creacion_fin & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND FECHA_CREACION BETWEEN '" & fecha_creacion_ini & "' AND '" & _
                   fecha_creacion_fin & "'"
                End If
            Else
                If fecha_creacion_ini <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where FECHA_CREACION='" & fecha_creacion_ini & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND FECHA_CREACION='" & fecha_creacion_ini & "'"
                    End If
                End If
                If fecha_creacion_fin <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where FECHA_CREACION='" & fecha_creacion_fin & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND FECHA_CREACION='" & fecha_creacion_fin & "'"
                    End If
                End If
            End If

            'nombre_area
            If nombre_area <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where NOMBRE_AREA_TRD='" & nombre_area & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND NOMBRE_AREA_TRD='" & nombre_area & "'"
                End If

            End If
            'nombre_serie
            If nombre_serie <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where NOMBRE_SERIE_TRD='" & nombre_serie & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND NOMBRE_SERIE_TRD='" & nombre_serie & "'"
                End If

            End If
            'nombre_sub_serie
            If nombre_sub_serie <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where NOMBRE_SUBSERIE_TRD='" & nombre_sub_serie & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND NOMBRE_SUBSERIE_TRD='" & nombre_sub_serie & "'"
                End If

            End If
            'fecha_extrema_incial
            If fecha_extrema_incial <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where FECHA_EXTREMA_INICIAL='" & fecha_extrema_incial & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND FECHA_EXTREMA_INICIAL='" & fecha_extrema_incial & "'"
                End If

            End If
            'fecha_extrema_final
            If fecha_extrema_final <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where FECHA_EXTREMA_FINAL='" & fecha_extrema_final & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND FECHA_EXTREMA_FINAL='" & fecha_extrema_final & "'"
                End If

            End If
            'rango_extremo_inicial
            If rango_extremo_inicial <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where RANGO_EXTREMO_INICIAL='" & rango_extremo_inicial & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND RANGO_EXTREMO_INICIAL='" & rango_extremo_inicial & "'"
                End If

            End If
            'rango_extremo_final
            If rango_extremo_final <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where RANGO_EXTREMO_FINAL='" & rango_extremo_final & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND RANGO_EXTREMO_FINAL='" & rango_extremo_final & "'"
                End If

            End If

            If usuario_gestion.Items.Count > 0 Then
                For z As Integer = 0 To usuario_gestion.Items.Count - 1
                    Dim id_usuario_gestion As Integer = -1
                    'Dim Result As String = Retorna_id_usuario_gestion_login(usuario_gestion.Items(z), id_usuario_gestion)
                    'If Result <> "YES" Then
                    '    Consulta_Expedientes = Result
                    '    Exit Function
                    'End If
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ID_USUARIO_GESTION='" & id_usuario_gestion & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND ID_USUARIO_GESTION='" & id_usuario_gestion & "'"
                    End If
                Next
            End If
            If option_expeidente_propio = True Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where ID_USUARIO_GESTION='" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND ID_USUARIO_GESTION='" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "'"
                End If
            End If
            Dim ref_Class_tipo_unidad_documental As New Class_tipo_unidad_documental
            If nombre_tipo_unidad_conservacion <> "Todos" And nombre_tipo_unidad_conservacion <> "" Then
                Dim id_tipo_unidad_conservacion As Integer = 0
                Dim Resulta = ref_Class_tipo_unidad_documental.Retorna_id_tipo_unidad_documental_por_nombre(nombre_tipo_unidad_conservacion, _
                                                                                                            id_tipo_unidad_conservacion)
                If Resulta <> "YES" Then
                    Consulta_unidad_conservacion = Resulta
                    Exit Function
                End If
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where TIPO_UNIDAD_CONSERVACION='" & id_tipo_unidad_conservacion & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND TIPO_UNIDAD_CONSERVACION='" & id_tipo_unidad_conservacion & "'"
                End If
            End If

            If activaand = -1 Then
                sql_condicion = sql_condicion & " where ID_EMPRESA_UNIDAD='" & id_empresa & "'"
                activaand = 1
            Else
                sql_condicion = sql_condicion & " AND ID_EMPRESA_UNIDAD='" & id_empresa & "'"
            End If
            If sub_seccion <> "" Then
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where NOMBRE_SUB_AREA='" & sub_seccion & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND NOMBRE_SUB_AREA='" & sub_seccion & "'"
                End If
            End If

            Dim sql_consulta As String = "SELECT ID_UNIDAD_CONSERVACION AS CODIGO_UNICO," & _
                "CODIGO_UNICO AS CONSECUTIVO,NOMBRE_SERIE,NOMBRE_SUBSERIE,NOMBRE_AREA,TEMA_UNIDAD_CONSERVACION AS TEMA,tun.NOMBRE_TIPO_UNIDAD,DESCRIPCION_UNIDAD_CONSERVACION,FECHA_CREACION,CODIGO_AREA_TRD,ID_SUB_AREA,NOMBRE_SUB_AREA," & _
                "CODIGO_SERIE,CODIGO_SUBSERIE," & _
                "FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL,RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL from unidad_conservacion " & _
                "INNER JOIN tipo_unidad_conservacion AS tun on (tun.ID_TIPO_UNIDAD=ID_TIPO_UNIDAD_CONSERVACION) " & _
                 sql_condicion & " order by ID_UNIDAD_CONSERVACION desc"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            HttpContext.Current.Session.Item("GA_SQL_CACHE_CONSULTA_UNIDAD") = sql_consulta
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_unidad_conservacion = "Error listando datos " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then

                'HiddenEmailconsulta.value = ""
                reflabel.Text = "Se encontraron 0 registro (s) "
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Consulta_unidad_conservacion = "YES"
                Exit Function
            Else
                'HiddenEmailconsulta.value = sql_consulta
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                'grediview.DataKeyNames = DataKey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                Next

                Consulta_unidad_conservacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Consulta_unidad_conservacion = "Inconsistencia general funcion Consulta_Expedientes " & ex.Message
        End Try
    End Function
    Function Consulta_unidad_conservacion_paging(ByRef grediview As GridView, _
                                                 ByRef reflabel As Label,
                                                 ByRef hideselecion As Object, _
                                                 ByRef update As UpdatePanel) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(HttpContext.Current.Session.Item("GA_SQL_CACHE_CONSULTA_UNIDAD"), Datset)
            If Result <> "YES" Then
                Consulta_unidad_conservacion_paging = "Error listando datos " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Consulta_unidad_conservacion_paging = "YES"
                Exit Function
            Else
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                grediview.DataSource = Datset
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(0).Text.ToString())
                Next
                Consulta_unidad_conservacion_paging = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_unidad_conservacion_paging = "Inconsistencia general función Consulta_unidad_conservacion_paging " & ex.Message
        End Try
    End Function
    Function Retorna_Ubicacion_unidad_conservacion_por_codigo_unico(ByVal id_unidad_conservacion As Integer, _
   ByVal tree As TreeView, ByVal codigo_unico As String) As String
        '**************************************************************************
        'Funcion Retorna la ubicacion del expediente dentro de la ubicacion fisica
        'Fecha 2014-10-09
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************************************
        Try
            Dim refclas As New ClassGestionArchivo
            Dim Result As String = ""
            Dim refclasunidad As New ClassUnidadConservacion
            '---------------------------------------------------
            'Retorna datos unidad de conservacion
            '---------------------------------------------------
            Dim estru_expediente() As unidad_conservacion
            Erase estru_expediente
            Result = refclasunidad.Listar_datos_Unidad_Conservacion_estructura(id_unidad_conservacion, estru_expediente)
            If Result <> "YES" Then
                Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = Result
                Exit Function
            End If
            If estru_expediente(0).ENTRE_PAÑO_ID_ENTREPAÑO = 0 Then
                Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = "La unidad de conservación no esta archivada"
                Exit Function
            End If
            Dim id_estante As Integer = 0
            Dim id_entrepaño As Integer = 0
            Dim id_entrapaño_idex As Integer = 0
            Dim Unidad_Conservacion_contenedora As String = ""
            Dim nombre_tipo_unidad As String = ""
            Dim struentrepaño() As ClassGestionArchivo.Entrapño_archivo
            Erase struentrepaño
            'If estru_expediente(0).ID_UNIDAD_CONSERVACION <> 0 Then
            '----------------------------------------------------
            'Solicita los datos de la unidad de conservación
            '----------------------------------------------------
            Dim estru_unidad_conservacion() As unidad_conservacion
            Erase estru_unidad_conservacion
            Result = refclasunidad.Listar_datos_Unidad_Conservacion_estructura( _
            estru_expediente(0).ID_UNIDAD_CONSERVACION, estru_unidad_conservacion)
            If Result <> "YES" Then
                Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Retorna el nombre del tipo de unidad de conservación
            '-----------------------------------------------------
            Dim nombre_tipo_unidad_conservacion As String = ""
            Result = refclasunidad.Retorna_nombre_tipo_unidad_conservacion_por_id_unidad_conservacion( _
            estru_expediente(0).ID_UNIDAD_CONSERVACION, nombre_tipo_unidad_conservacion)
            If Result <> "YES" Then
                Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = Result
                Exit Function
            End If
            Unidad_Conservacion_contenedora = "(" & nombre_tipo_unidad_conservacion & ") " & estru_unidad_conservacion(0).CODIGO_UNICO
            Result = refclas.Retorna_id_Entrepaño_id_unidad_conservacion( _
            estru_unidad_conservacion(0).ID_UNIDAD_CONSERVACION, id_entrepaño)
            If Result <> "YES" Then
                Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = Result
                Exit Function
            End If
            Result = refclas.Retorna_Id_Estante_por_entrepaño(id_entrepaño, id_estante)
            If Result <> "YES" Then
                Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = "Imposible el id del estante " & Result
                Exit Function
            End If
            Result = refclas.Listar_Entrepaño_Archivo(estru_expediente(0).ID_EMPRESA, struentrepaño, id_estante)
            If Result <> "YES" Then
                Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = "Imposible listar entrepaños " & Result
                Exit Function
            End If
            If struentrepaño Is Nothing Then
                Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = "Imposible listar entrepaños, no hay estructura tipo 0"
                Exit Function
            End If
            For i As Integer = 0 To struentrepaño.Length - 1
                If estru_expediente(0).ENTRE_PAÑO_ID_ENTREPAÑO = struentrepaño(i).id_entreapaño Then
                    id_entrapaño_idex = i
                    Exit For
                End If
            Next
            tree.Nodes.Clear()
            Dim trenod As New TreeNode
            trenod.Text = "Edificio : " & struentrepaño(id_entrapaño_idex).edificio_contenedor
            Dim trenode_piso As New TreeNode
            trenode_piso.Text = "Piso : " & struentrepaño(id_entrapaño_idex).piso_contenedor
            trenod.ChildNodes.Add(trenode_piso)
            Dim trenode_area As New TreeNode
            trenode_area.Text = "Area : " & struentrepaño(id_entrapaño_idex).area_contenedor
            trenode_piso.ChildNodes.Add(trenode_area)
            Dim trenode_modulo As New TreeNode
            trenode_modulo.Text = "Modulo : " & struentrepaño(id_entrapaño_idex).modulo_contendor
            trenode_area.ChildNodes.Add(trenode_modulo)
            Dim trenode_estante As New TreeNode
            trenode_estante.Text = "Estante : " & struentrepaño(id_entrapaño_idex).estante
            trenode_modulo.ChildNodes.Add(trenode_estante)
            Dim trenode_entrepaño As New TreeNode
            trenode_entrepaño.Text = "Entrepaño : " & struentrepaño(id_entrapaño_idex).entre_paño
            trenode_estante.ChildNodes.Add(trenode_entrepaño)
            Dim trenode_caja As New TreeNode
            trenode_caja.Text = "Unidad de conservación Contenedora : " & Unidad_Conservacion_contenedora
            If Unidad_Conservacion_contenedora <> "" Then
                trenode_entrepaño.ChildNodes.Add(trenode_caja)
            End If
            tree.Nodes.Add(trenod)
            tree.ExpandAll()
            Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = "YES"
            
            'Else
            '----------------------------------------------------
            'solicita el id del entrapaño por el expediente 
            '
            '----------------------------------------------------
            '    Result = refclas.Retorna_id_Entrepaño_id_expediente( _
            '    estru_expediente(0).ID_EXPEDIENTE, id_entrepaño)
            '    If Result <> "YES" Then
            '        Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = Result
            '        Exit Function
            '    End If
            '    Result = refclas.Retorna_Id_Estante_por_entrepaño(id_entrepaño, id_estante)
            '    If Result <> "YES" Then
            '        Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = "Imposible listar la identificación del estante " & Result
            '        Exit Function
            '    End If
            '    Result = refclas.Listar_Entrepaño_Archivo(estru_expediente(0).ID_EMPRESA_GESTION, struentrepaño, id_estante)
            '    If Result <> "YES" Then
            '        Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = "Imposible listar entrepaños " & Result
            '        Exit Function
            '    End If

            '    For i As Integer = 0 To struentrepaño.Length - 1
            '        If estru_expediente(0).ENTRE_PAÑO_ID_ENTREPAÑO = struentrepaño(i).id_entreapaño Then
            '            id_entrapaño_idex = i
            '            Exit For
            '        End If
            '    Next
            'End If
            'If struentrepaño Is Nothing Then
            '    Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = "Imposible listar entrepaños, no hay estructura "
            '    Exit Function
            'End If
            'Result = Me.Retorna_nombre_tipo_expediente_por_id_expediente( _
            '    id_expediente, nombre_tipo_unidad)
            'If Result <> "YES" Then
            '    Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = Result
            '    Exit Function
            'End If
            'tree.Nodes.Clear()
            'Dim trenod As New TreeNode
            'trenod.Text = "Edificio : " & struentrepaño(id_entrapaño_idex).edificio_contenedor
            'Dim trenode_piso As New TreeNode
            'trenode_piso.Text = "Piso : " & struentrepaño(id_entrapaño_idex).piso_contenedor
            'trenod.ChildNodes.Add(trenode_piso)
            'Dim trenode_area As New TreeNode
            'trenode_area.Text = "Area : " & struentrepaño(id_entrapaño_idex).area_contenedor
            'trenode_piso.ChildNodes.Add(trenode_area)
            'Dim trenode_modulo As New TreeNode
            'trenode_modulo.Text = "Modulo : " & struentrepaño(id_entrapaño_idex).modulo_contendor
            'trenode_area.ChildNodes.Add(trenode_modulo)
            'Dim trenode_estante As New TreeNode
            'trenode_estante.Text = "Estante : " & struentrepaño(id_entrapaño_idex).estante
            'trenode_modulo.ChildNodes.Add(trenode_estante)
            'Dim trenode_entrepaño As New TreeNode
            'trenode_entrepaño.Text = "Entrepaño : " & struentrepaño(id_entrapaño_idex).entre_paño
            'trenode_estante.ChildNodes.Add(trenode_entrepaño)
            'Dim trenode_caja As New TreeNode
            'trenode_caja.Text = "Unidad de conservación Contenedora : " & Unidad_Conservacion_contenedora
            'If Unidad_Conservacion_contenedora <> "" Then
            '    trenode_entrepaño.ChildNodes.Add(trenode_caja)
            'End If
            'Dim treenode_expediente As New TreeNode
            'treenode_expediente.Text = " Tipo unidad : " & estru_expediente(0).NOMBRE_TIPO_UNIDAD_DOCUMENTAL & " Clase unidad documental(Contenido) : (" & nombre_tipo_unidad & ") " & codigo_unico & " ID " & id_expediente
            'If Unidad_Conservacion_contenedora = "" Then
            '    trenode_entrepaño.ChildNodes.Add(treenode_expediente)
            'Else
            '    trenode_caja.ChildNodes.Add(treenode_expediente)
            'End If


        Catch ex As Exception
            Retorna_Ubicacion_unidad_conservacion_por_codigo_unico = "Inconsistencia funcion Retorna_Ubicacion_expediente_por_codigo_unico " & ex.Message
        End Try
    End Function
    Function Reubica_expediente_unidad_conservacion(ByVal Tipo_unidad As String, ByRef ref_treview_fuente As TreeView, _
                                                    ByRef ref_treview_destino As TreeView, _
                                                    ByRef ref_update_fuente As UpdatePanel, _
                                                    ByRef tre_view_destino As TreeView, _
                                                    ByRef up_date_destino As UpdatePanel) As String
        Try
            '-----------------------------------------------------
            'Archiva expediente 
            '------------------------------------------------------
            If Tipo_unidad = "Expediente" Then
                Dim estru_expediente() As expediente_conservacion
                Erase estru_expediente
                Dim refclsexpe As New ClassGaExpediente
                Dim Result As String = ""
                '************************************************************
                'Archiva expediente en entrepaño
                '************************************************************
                Dim lig = InStr(ref_treview_destino.SelectedNode.Text, "Entrepaño")
                If lig > 0 Then
                    Result = refclsexpe.Listar_datos_Expediente_estructura_por_tipo_expediente(ref_treview_fuente.SelectedNode.Value, _
                        estru_expediente)
                    If Result <> "YES" Then
                        Reubica_expediente_unidad_conservacion = Result
                        Exit Function
                    End If
                    If estru_expediente Is Nothing Then
                        Reubica_expediente_unidad_conservacion = "Imposible encontrar estructura expediente"
                        Exit Function
                    End If
                    Dim splinode() As String = ref_treview_destino.SelectedNode.Value.ToString.Split("|")
                    Dim refclas_expediente As New ClassGaExpediente
                    Result = refclas_expediente.Archiva_expediente_en_entrepano(splinode(0), _
                    0, ref_treview_fuente.SelectedNode.Value.ToString, _
                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                    HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), HttpContext.Current.Session.Item("ip_host_name"), _
                    estru_expediente(0), ref_treview_destino, ref_treview_fuente, ref_update_fuente)
                    If Result <> "YES" Then
                        Reubica_expediente_unidad_conservacion = Result
                        Exit Function
                    End If
                End If
                '------------------------------------------------
                'Archiva expediente unidad conservación
                '------------------------------------------------
                If ref_treview_destino.SelectedNode.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                    Result = refclsexpe.Listar_datos_Expediente_estructura_por_tipo_expediente(ref_treview_fuente.SelectedNode.Value, _
                       estru_expediente)
                    If Result <> "YES" Then
                        Reubica_expediente_unidad_conservacion = Result
                        Exit Function
                    End If
                    If estru_expediente Is Nothing Then
                        Reubica_expediente_unidad_conservacion = "Imposible encontrar estrucutura expediente"
                        Exit Function
                    End If
                    Dim splinode() As String = ref_treview_destino.SelectedNode.Value.ToString.Split("|")
                    Dim refclas_expediente As New ClassGaExpediente
                    Result = refclas_expediente.Archiva_expediente_unidad_contenedora(splinode(0), _
                    0, ref_treview_fuente.SelectedNode.Value, _
                     HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                    HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), HttpContext.Current.Session.Item("ip_host_name"), ref_treview_destino.SelectedNode, _
                    estru_expediente(0), ref_treview_destino, ref_treview_fuente, ref_update_fuente)
                    If Result <> "YES" Then
                        Reubica_expediente_unidad_conservacion = Result
                        Exit Function
                    End If
                End If
            End If
            If Tipo_unidad = "UNIDAD CONTENEDORA EXPEDIENTE" Then
                Dim Result As String = ""
                Dim lig = InStr(ref_treview_destino.SelectedNode.Text, "Entrepaño")
                If lig = 0 Then
                    Reubica_expediente_unidad_conservacion = "La unidad contenedora solo se puede anidar en entrepaño"
                    Exit Function
                End If
                Dim splinode() As String = ref_treview_destino.SelectedNode.Value.ToString.Split("|")
                Result = Reubicar_unidad_conservacion(ref_treview_fuente.SelectedNode.Value, _
                         ref_treview_fuente, splinode(0), HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"), _
                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), HttpContext.Current.Session.Item("ip_host_name"), _
                         tre_view_destino, up_date_destino)
                If Result <> "YES" Then
                    Reubica_expediente_unidad_conservacion = Result
                    Exit Function
                End If
            End If
            Reubica_expediente_unidad_conservacion = "YES"
        Catch ex As Exception
            Reubica_expediente_unidad_conservacion = "Inconsistencia general función  Reubica_expediente_unidad_conservacion " & ex.Message
        End Try

    End Function
    Function Reubicar_unidad_conservacion(ByVal id_unidad_conservacion As Integer, _
                                          ByRef ref_treview_fuente As TreeView, _
                                          ByVal id_entrepaño As Integer, _
                                          ByVal user_Gestion As String, _
                                          ByVal id_usuario_gestion As Integer, _
                                          ByVal iptrans As String, _
                                          ByRef tre_view_destino As TreeView, _
                                          ByRef up_date_destino As UpdatePanel) As String


        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Dim Result As String = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Reubicar_unidad_conservacion = Result
            Exit Function
        End If
        '--------------------------------------------------------
        'Solicita el número de unidades permitidas para anidar
        '--------------------------------------------------------
        Dim numero_unidades_anidadas As Integer = 0
        Dim numero_unidades_permitidas As Integer = 0
        Result = Solicita_numero_unidades_anidadas_entre_pano(id_entrepaño, numero_unidades_anidadas)
        If Result <> "YES" Then
            Reubicar_unidad_conservacion = Result
            Exit Function
        End If
        If numero_unidades_anidadas > 0 Then
            Result = Me.Solicita_numero_unidades_permitidos_entrepano(id_entrepaño, numero_unidades_permitidas)
            If Result <> "YES" Then
                Reubicar_unidad_conservacion = Result
                Exit Function
            End If
            If numero_unidades_anidadas >= numero_unidades_permitidas Then
                Reubicar_unidad_conservacion = "El entrapaño (" & id_entrepaño & ") no permite anidar más unidades, supero el límite unidades de conservación permitidas. " & _
                                                " Las unidades almacenadas en el entrepaño son (" & numero_unidades_anidadas & ") de  (" & numero_unidades_permitidas & " ) unidad(es) permitida(s)"
                Exit Function
            End If
        End If

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "update unidad_conservacion set ENTRE_PAÑO_ID_ENTREPAÑO=" & id_entrepaño & _
               ", ID_UNIDAD_CONSERVACION_TOPLOGICA=null where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reubicar_unidad_conservacion = "Imposible reubicar unidad de conservación  : " & sqlinsertcion
                errorM = "Imposible reubicar unidad de conservación  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If

            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_unidad_conservacion (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" & _
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" & _
           "'ELIMINA UNIDAD','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," & _
           id_unidad_conservacion & ",'" & iptrans & "','" & hor & "','DOCUARCHI','" & "Se reubica en e entreapño id" & id_entrepaño & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualizar los unidad de conservación  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim texnode_text As String = ref_treview_fuente.SelectedNode.Value
            Result = Rescursive_node_tree_elimina(tre_view_destino, texnode_text)
            If Result <> "YES" Then
                errorM = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim treenode As New TreeNode
            TreeNode = ref_treview_fuente.SelectedNode
            tre_view_destino.SelectedNode.ChildNodes.Add(treenode)
            up_date_destino.Update()
            If Not ref_treview_fuente.SelectedNode Is Nothing Then
                ref_treview_fuente.Nodes.Remove(ref_treview_fuente.SelectedNode)
                Dim sNodo As TreeNode = ref_treview_fuente.SelectedNode
                Dim pNodo As TreeNode = sNodo.Parent
                pNodo.ChildNodes.Remove(sNodo)
            End If
            myTrans.Commit()
            myConnection.Close()
            Reubicar_unidad_conservacion = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Reubicar_unidad_conservacion = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Reubicar_unidad_conservacion = errorM

        End Try
    End Function
    Function Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion(ByRef Tree As TreeView, ByVal estru_unidad_conservacion() As unidad_conservacion, _
   ByVal node_tag As String, ByVal node_text As String, ByRef tred As TreeNode) As String
        Try
            tred.Value = node_tag
            tred.Text = node_text
            tred.ChildNodes.Clear()
            If estru_unidad_conservacion Is Nothing Then
                Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion = "YES"
                Exit Function
            End If
            For i As Integer = 0 To estru_unidad_conservacion.Length - 1
                Dim NodeTree As New TreeNode
                Dim ref_fecha_extrema_ini As String = ""
                Dim ref_fecha_extrema_fin As String = ""
                If estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL <> "" Then
                    ref_fecha_extrema_ini = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL, 10)
                Else
                    ref_fecha_extrema_ini = estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL
                End If
                If estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL <> "" Then
                    ref_fecha_extrema_fin = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL, 10)
                Else
                    ref_fecha_extrema_fin = estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL
                End If
                Dim tipo_unidad As String = ""
                If estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD Is Nothing Then
                Else
                    tipo_unidad = "TIPO UNIDAD : (" & estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD & ")  "
                End If
                NodeTree.Text = tipo_unidad & "CODIGO UNICO: " & estru_unidad_conservacion(i).CODIGO_UNICO & " TEMA: " & _
                estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION & _
                " FECHAS EXTREMAS: " & ref_fecha_extrema_ini & " HASTA " & _
                ref_fecha_extrema_fin & " RANGO EXTREMOS:" & estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL & " HASTA " & _
                estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL & "-" & estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION
                Dim tag_node As String = estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION & "|" & estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD & "|" & estru_unidad_conservacion(i).CODIGO_CORTO
                NodeTree.Value = tag_node
                NodeTree.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE"
                If estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION > 1 Then
                    'NodeTree.ForeColor = Color.Blue
                End If
                NodeTree.ImageUrl = "../Gestion/imagenes/caja_exp.png"
                tred.ChildNodes.Add(NodeTree)
            Next
            'Tree.SelectedNode = Tree.Nodes.Item(0)
            Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion = "YES"
        Catch ex As Exception
            Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion = "Inconsistencia funcion Listar_Unidad_Conservacion_treview_nodo_entre_pano_ubicacion " & ex.Message
        End Try
    End Function
    Function Actualiza_Unidad_Conservacion(ByVal id_usuario_gestion As Integer, ByVal id_unidad_conservacion As Integer, _
    ByVal codigo_unico As String, ByVal estado_codigo_unico As Integer, _
    ByVal fecha_extrema_incial As String, ByVal fecha_extrema_final As String, _
    ByVal rango_extremo_inicial As String, ByVal rango_extremo_final As String, ByVal tema_unidad_conservacion As String, _
    ByVal descripcion_unidad_conservacion As String, ByRef codigo_unidad As String, _
    ByVal user_gestion As String, ByVal ip_transaccion As String, _
    ByVal id_empresa_gestion As Integer, ByVal volumen_unidad_conservacion As Integer, _
    ByVal nombre_organigrama As String, ByVal nombre_area As String, _
    ByVal nombre_serie As String, ByVal nombre_sub_serie As String, ByVal nombre_unidad_conservacion As String, ByVal tipo_unidad_conservacion As Integer, _
     ByVal nombre_sub_seccion As String, ByVal id_serie As Object, _
     ByVal id_sub_serie As Object, ByVal codigo_area As Object, _
     ByVal id_organigrama As Object, ByVal id_instrumento As Object, _
     ByVal id_tipo_unidad As Integer) As String
        Dim Result As String = "YES"
        If estado_codigo_unico = 1 Then
            If codigo_unico = "" Then
                Actualiza_Unidad_Conservacion = "Debe informar el código"
                Exit Function
            End If
            Result = Verfica_Existencia_Codigo_Unico_unidad_conservacion(codigo_unico, _
                                                                         id_empresa_gestion)
            If Result <> "YES" Then
                Actualiza_Unidad_Conservacion = Result
                Exit Function
            End If
        End If
        If nombre_area = "" Then
            Actualiza_Unidad_Conservacion = "Por favor seleccione el área"
            Exit Function
        End If
        '------------------------------------------------
        'Retorna el nombre de la unidad de conservación
        '------------------------------------------------
        Dim ref_nombre_unidad_conservacion As String = "TIPO UNIDAD : (" & nombre_unidad_conservacion & ") "
        Dim id_sub_seccion As Object = 0
        Dim re_nombre_sub_seccion As String = ""
        If nombre_sub_seccion = "" Then
            re_nombre_sub_seccion = "null"
        Else
            re_nombre_sub_seccion = "'" & nombre_sub_seccion & "'"
        End If
        If id_sub_seccion = 0 Then
            id_sub_seccion = "null"
        End If
        Dim ref_nombre_serie As String = "null"
        If nombre_serie = "" Then
            ref_nombre_serie = "null"
        Else
            ref_nombre_serie = "'" & nombre_serie & "'"
        End If
        Dim ref_nombre_sub_serie As String = "null"
        If nombre_sub_serie = "" Then
            ref_nombre_sub_serie = "null"
        Else
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        Dim ref_id_serie As Object
        If id_serie = 0 Then
            ref_id_serie = "null"
        Else
            ref_id_serie = id_serie
        End If
        Dim ref_id_sub_serie As Object
        If id_sub_serie = 0 Then
            ref_id_sub_serie = "null"
        Else
            ref_id_sub_serie = id_sub_serie
        End If
        Dim ref_codigo_area As Object
        If codigo_area = 0 Then
            ref_codigo_area = "null"
        Else
            ref_codigo_area = codigo_area
        End If
        Dim ref_id_instrumento As Object
        If id_instrumento = 0 Then
            ref_id_instrumento = "null"
        Else
            ref_id_instrumento = id_instrumento
        End If
        '---------------------------------------
        'Vefica formato fechas extremas
        '---------------------------------------
        Dim re_fecha_extrema_incial As String = ""
        Dim re_fecha_extrema_final As String = ""
        If fecha_extrema_incial <> "" Then
            Dim splifecha() As String = fecha_extrema_incial.Split("/")
            re_fecha_extrema_incial = "'" & fecha_extrema_incial & "'"
        Else
            re_fecha_extrema_incial = "null"
        End If
        If fecha_extrema_final <> "" Then
            Dim splifecha() As String = fecha_extrema_final.Split("/")
            re_fecha_extrema_final = "'" & fecha_extrema_final & "'"
        Else
            re_fecha_extrema_final = "null"
        End If

        Dim ref_rango_extremo_inicial As String = ""
        If rango_extremo_inicial = "" Then
            ref_rango_extremo_inicial = "null"
        Else
            ref_rango_extremo_inicial = "'" & rango_extremo_inicial & "'"
        End If
        Dim ref_rango_extremo_final As String = ""
        If rango_extremo_final = "" Then
            ref_rango_extremo_final = "null"
        Else
            ref_rango_extremo_final = "'" & rango_extremo_final & "'"
        End If
        Dim ref_tema_unidad_conservacion As String = ""
        If tema_unidad_conservacion = "" Then
            ref_tema_unidad_conservacion = "null"
        Else
            ref_tema_unidad_conservacion = "'" & tema_unidad_conservacion & "'"
        End If

        If descripcion_unidad_conservacion = "" Then
            descripcion_unidad_conservacion = "null"
        Else
            descripcion_unidad_conservacion = "'" & descripcion_unidad_conservacion & "'"
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Actualiza_Unidad_Conservacion = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad As String = 0
        Dim errorM As String = "YES"
        Try
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO Ra_Log_Unidad_conservacion (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" & _
            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) values (" & _
            "'EDITA_UNIDAD_CONSERVACION','" & user_gestion & "','" & id_usuario_gestion & "','" & date1al & "'," & _
            id_unidad_conservacion & ",'" & ip_transaccion & "','" & hor & "','GESTOR-WEB')"
            'myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            Dim siwt As Integer = myCommand.ExecuteNonQuery()
            If siwt = 0 Then
                myConnection.Close()
                errorM = "Imposible registrar log  : " & sqlforupdate
                Exit Function
            End If
            '--------------------------------------------
            'Agregar valores insertcion
            '--------------------------------------------
            Dim update As String = "Update unidad_conservacion set FECHA_EXTREMA_INICIAL=" & re_fecha_extrema_incial & _
            ",FECHA_EXTREMA_FINAL=" & re_fecha_extrema_final & ",RANGO_EXTREMO_INICIAL=" & ref_rango_extremo_inicial & _
            ",RANGO_EXTREMO_FINAL=" & ref_rango_extremo_final & ",TEMA_UNIDAD_CONSERVACION=" & ref_tema_unidad_conservacion & _
            ",DESCRIPCION_UNIDAD_CONSERVACION=" & descripcion_unidad_conservacion & ",CODIGO_UNICO='" & codigo_unico & "'" & _
            ",NOMBRE_AREA='" & nombre_area & "'" & _
            ",CODIGO_AREA_TRD=" & ref_codigo_area & ",NOMBRE_SERIE=" & ref_nombre_serie & ",CODIGO_SERIE=" & _
            id_serie & ",NOMBRE_SUBSERIE=" & ref_nombre_sub_serie & ",CODIGO_SUBSERIE=" & ref_id_sub_serie & _
            ",DESCRIPCION_UNIDAD_CONSERVACION=" & descripcion_unidad_conservacion & _
            ",TIPO_UNIDAD_CONSERVACION=" & tipo_unidad_conservacion & _
            ",ID_TIPO_UNIDAD_CONSERVACION=" & id_tipo_unidad & _
            ",ID_SUB_AREA=" & id_sub_seccion & _
            ",NOMBRE_SUB_AREA=" & re_nombre_sub_seccion & _
            ",id_instrumento=" & ref_id_instrumento & _
            " where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            myCommand.CommandText = update
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_Unidad_Conservacion = "Imposible actualizar unidad de conservacion  : " & update
                myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible actualizar unidad de conservacion  : " & update
                Exit Function
            End If
            codigo_unidad = ref_nombre_unidad_conservacion & " CODIGO UNICO: " & codigo_unico & " TEMA: " & _
            tema_unidad_conservacion & " FECHAS EXTREMAS: " & fecha_extrema_incial & " HASTA " & _
            fecha_extrema_final & " RANGO EXTREMOS:" & rango_extremo_inicial & " HASTA " & _
            rango_extremo_final & "-1"
            myTrans.Commit()
            myConnection.Close()
            Actualiza_Unidad_Conservacion = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Actualiza_Unidad_Conservacion = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_Unidad_Conservacion = errorM

        End Try
    End Function
    Function Activar_editar_unidad_conservacion(ByRef update As UpdatePanel, _
                                                ByRef ref_hdnEmailID As Object, _
                                                ByRef ref_Hidden_id_empresa As Object) As String
        Try
            Dim Result As String = ""
            Dim ref_DropDownListorganigrama As DropDownList = Nothing
            Dim ref_TextBoxASUNTO_EXPEDIENTE As TextBox = Nothing
            Dim ref_TextBoxCodigoManual As TextBox = Nothing
            Dim ref_TextBoxRANGO_EXTREMO_INICIAL As TextBox = Nothing
            Dim ref_TextBoxRANGO_EXTREMO_FINAL As TextBox = Nothing
            Dim ref_TextBoxFECHA_EXTREMA_INICIAL As TextBox = Nothing
            Dim ref_TextBoxFECHA_EXTREMA_FINAL As TextBox = Nothing
            Dim ref_TextBox_ayuda_conetedora As TextBox = Nothing
            Dim ref_TextBoxTEMA_EXPEDIENTE As TextBox = Nothing
            Dim ref_DropDownListArea As DropDownList = Nothing
            Dim ref_DropDownListSerie As DropDownList = Nothing
            Dim ref_DropDownListSubserie As DropDownList = Nothing
            Dim ref_DropDownListBoxtipoexpediente As DropDownList = Nothing
            Dim ref_DropDownList_tipo_unidad_contenedora As DropDownList = Nothing
            Dim ref_DropDownListsub_seccion As DropDownList = Nothing
            Dim ref_DropDownList_instrumento As DropDownList = Nothing
            Dim ref_update_panel_controles As UpdatePanel = Nothing
            Dim matri_nombre_controles() As String = {"DropDownListorganigrama", _
                "TextBoxASUNTO_EXPEDIENTE", _
               "TextBoxCodigoManual", "TextBoxRANGO_EXTREMO_INICIAL", "TextBoxRANGO_EXTREMO_FINAL", "TextBoxFECHA_EXTREMA_INICIAL", _
               "TextBoxFECHA_EXTREMA_FINAL", "TextBox_ayuda_conetedora", "TextBoxTEMA_EXPEDIENTE", "DropDownListArea", "DropDownListSerie", _
               "DropDownListSubserie", "DropDownList_tipo_unidad_contenedora", "DropDownListsub_seccion", "DropDownList_instrumento", "update_panel_controles"}
            Dim total_controles As Integer = matri_nombre_controles.Length
            For i As Integer = 0 To total_controles - 1
                Dim control_ob As Object = Nothing
                control_ob = update.FindControl(matri_nombre_controles(i))
                If control_ob Is Nothing Then
                    Activar_editar_unidad_conservacion = "Imposible encontrar el control " & matri_nombre_controles(i)
                    Exit Function
                End If
                Select Case matri_nombre_controles(i)
                    Case "DropDownListorganigrama"
                        ref_DropDownListorganigrama = control_ob
                    Case "TextBoxASUNTO_EXPEDIENTE"
                        ref_TextBoxASUNTO_EXPEDIENTE = control_ob
                    Case "TextBoxCodigoManual"
                        ref_TextBoxCodigoManual = control_ob
                    Case "TextBoxRANGO_EXTREMO_INICIAL"
                        ref_TextBoxRANGO_EXTREMO_INICIAL = control_ob
                    Case "TextBoxRANGO_EXTREMO_FINAL"
                        ref_TextBoxRANGO_EXTREMO_FINAL = control_ob
                    Case "TextBoxFECHA_EXTREMA_INICIAL"
                        ref_TextBoxFECHA_EXTREMA_INICIAL = control_ob
                    Case "TextBoxFECHA_EXTREMA_FINAL"
                        ref_TextBoxFECHA_EXTREMA_FINAL = control_ob
                    Case "TextBox_ayuda_conetedora"
                        ref_TextBox_ayuda_conetedora = control_ob
                    Case "TextBoxTEMA_EXPEDIENTE"
                        ref_TextBoxTEMA_EXPEDIENTE = control_ob
                    Case "DropDownListArea"
                        ref_DropDownListArea = control_ob
                    Case "DropDownListSerie"
                        ref_DropDownListSerie = control_ob
                    Case "DropDownListSubserie"
                        ref_DropDownListSubserie = control_ob
                    Case "DropDownListBoxtipoexpediente"
                        ref_DropDownListBoxtipoexpediente = control_ob
                    Case "DropDownList_tipo_unidad_contenedora"
                        ref_DropDownList_tipo_unidad_contenedora = control_ob
                    Case "DropDownListsub_seccion"
                        ref_DropDownListsub_seccion = control_ob
                    Case "DropDownList_instrumento"
                        ref_DropDownList_instrumento = control_ob
                    Case "update_panel_controles"
                        ref_update_panel_controles = control_ob
                End Select
            Next
            Dim estru_unidad_conservacion() As unidad_conservacion
            Erase estru_unidad_conservacion
            '-----------------------------------------------
            'Lista estructura unidad conservacion
            '-----------------------------------------------
            Result = Listar_datos_Unidad_Conservacion_estructura(ref_hdnEmailID.value, estru_unidad_conservacion)
            If Result <> "YES" Then
                Activar_editar_unidad_conservacion = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Lista tipos de unidad conservacion selección
            '-----------------------------------------------
            Result = Me.lista_tipos_unidades_Combo_seleccion_items(ref_DropDownList_tipo_unidad_contenedora, 1, _
                                                                 estru_unidad_conservacion(0).ID_TIPO_UNIDAD_CONSERVACION)
            If Result <> "YES" Then
                Activar_editar_unidad_conservacion = Result
                Exit Function
            End If
            '----------------------------------------------
            'Retorna ayuda tipo unidad conservacion
            '---------------------------------------------
            If estru_unidad_conservacion(0).ID_TIPO_UNIDAD_CONSERVACION <> 0 Then
                Result = Me.Retorna_descripcion_tipo_unidad_conservacion_por_id(estru_unidad_conservacion(0).ID_TIPO_UNIDAD_CONSERVACION, ref_TextBox_ayuda_conetedora.Text)
                If Result <> "YES" Then
                    Activar_editar_unidad_conservacion = Result
                    Exit Function
                End If
            End If
            Dim id_organigrama As Integer = 0
            Dim Ref_gagestioninstrumento As New ClassGaGestionInstrumento
            Result = Ref_gagestioninstrumento.Solicita_id_organigrama_area_departamento(estru_unidad_conservacion(0).CODIGO_AREA_TRD, _
                                                                                        id_organigrama)
            If Result <> "YES" Then
                Activar_editar_unidad_conservacion = Result
                Exit Function
            End If
            Dim Refclas_organigrama As New Class_registro_organigrama
            Result = Refclas_organigrama.Listar_Organigramas_Empresa_Combo_Default_Items(HttpContext.Current.Session.Item("GA_IDEMPRESA"), _
                                                                                         id_organigrama, _
                                                                                         ref_DropDownListorganigrama, _
                                                                                         ref_update_panel_controles)
            If Result <> "YES" Then
                Activar_editar_unidad_conservacion = Result
                Exit Function
            End If

            Result = Ref_gagestioninstrumento.Lista_instrumentos_archivisticos(id_organigrama, _
                                                                                 estru_unidad_conservacion(0).id_instrumento, _
                                                                                 ref_DropDownList_instrumento, ref_update_panel_controles)
            If Result <> "YES" Then
                Activar_editar_unidad_conservacion = Result
                Exit Function
            End If
            '---------------------------------------------------------------------------------
            'lista y asigna series documentales relacionadas al  instrumento del expediente
            '---------------------------------------------------------------------------------
            Dim Ref_class_series As New Class_series_documentales
            If estru_unidad_conservacion(0).id_instrumento <> 0 Then
                Result = Ref_class_series.Lista_series_relacionadas_instrumento_id_area_default(estru_unidad_conservacion(0).CODIGO_AREA_TRD, _
                                                                                                estru_unidad_conservacion(0).id_instrumento, _
                                                                                                estru_unidad_conservacion(0).CODIGO_SERIE, _
                                                                                                ref_DropDownListSerie, ref_update_panel_controles)
                If Result <> "YES" Then
                    Activar_editar_unidad_conservacion = "Función Activar_editar_unidad_conservacion dice " & Result
                    Exit Function
                End If
           
            End If

            '---------------------------
            'Retorna areas  organigrama
            '---------------------------
            Dim Refclas_gestionDocumental As New ClassGestionDocumental
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            Dim Class_ra_area_departamento_permitida_usuario_gestion As New Class_ra_area_departamento_permitida_usuario_gestion
            If id_organigrama <> 0 Then
                '---------------------------------------
                'Seleccion tipo lista areas organigrama
                '---------------------------------------
                If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
                    Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default_Items(id_organigrama, _
                                                                                                           estru_unidad_conservacion(0).CODIGO_AREA_TRD, _
                                                                                                           ref_DropDownListArea)
                    If Result <> "YES" Then
                        Activar_editar_unidad_conservacion = Result
                        Exit Function
                    End If
                Else
                    Result = Class_ra_area_departamento_permitida_usuario_gestion.lista_areas_permitidas_usuario_gestion_organigrama_default_items( _
                                                                                                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                                                                   id_organigrama, _
                                                                                                                                                   estru_unidad_conservacion(0).CODIGO_AREA_TRD, _
                                                                                                                                                   ref_DropDownListArea)
                    If Result <> "YES" Then
                        Activar_editar_unidad_conservacion = Result
                        Exit Function
                    End If
                End If

            End If
            Result = Refclas_gestionDocumental.Listar_SubSeries_Documentales_default_item(estru_unidad_conservacion(0).CODIGO_SERIE, _
                                                                                         estru_unidad_conservacion(0).CODIGO_SUBSERIE, _
                                                                                         ref_DropDownListSubserie)
            If Result <> "YES" Then
                Activar_editar_unidad_conservacion = Result
                Exit Function
            End If
            If estru_unidad_conservacion(0).FECHA_EXTREMA_INICIAL <> "" Then
                Dim splifecha() As String = Left(estru_unidad_conservacion(0).FECHA_EXTREMA_INICIAL, 10).Split("/")
                ref_TextBoxFECHA_EXTREMA_INICIAL.Text = splifecha(2) & "/" & splifecha(1) & "/" & splifecha(0)
            End If
            If estru_unidad_conservacion(0).FECHA_EXTREMA_FINAL <> "" Then
                Dim splifecha() As String = Left(estru_unidad_conservacion(0).FECHA_EXTREMA_FINAL, 10).Split("/")
                ref_TextBoxFECHA_EXTREMA_FINAL.Text = splifecha(2) & "/" & splifecha(1) & "/" & splifecha(0)
            End If
            ref_TextBoxRANGO_EXTREMO_INICIAL.Text = estru_unidad_conservacion(0).RANGO_EXTREMO_INICIAL
            ref_TextBoxRANGO_EXTREMO_FINAL.Text = estru_unidad_conservacion(0).RANGO_EXTREMO_FINAL
            ref_TextBoxTEMA_EXPEDIENTE.Text = estru_unidad_conservacion(0).TEMA_UNIDAD_CONSERVACION
            ref_TextBoxASUNTO_EXPEDIENTE.Text = estru_unidad_conservacion(0).DESCRIPCION_UNIDAD_CONSERVACION
            ref_TextBoxCodigoManual.Text = estru_unidad_conservacion(0).CODIGO_UNICO
            Activar_editar_unidad_conservacion = "YES"
        Catch ex As Exception
            Activar_editar_unidad_conservacion = "Inconsistencia general función Activar_editar_unidad_conservacion " & ex.Message
        End Try
    End Function
    Function Asignar_Datos_edicion_unidad_conservacion( _
  ByRef update As UpdatePanel, ByRef ref_hdnEmailID As Object, ByRef ref_Hidden_id_empresa As Object) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAdmonEmpresa
            Dim Resfclas As New ClassGaTipoDocumental
            Dim id_empresa As Integer = 0
            Dim refclas_rad As New ClassRadicador
            Dim refclascexpediente As New ClassGaExpediente
            Dim ref_DropDownListorganigrama As DropDownList = Nothing
            Dim ref_TextBoxASUNTO_EXPEDIENTE As TextBox = Nothing
            Dim ref_TextBoxCodigoManual As TextBox = Nothing
            Dim ref_TextBoxRANGO_EXTREMO_INICIAL As TextBox = Nothing
            Dim ref_TextBoxRANGO_EXTREMO_FINAL As TextBox = Nothing
            Dim ref_TextBoxFECHA_EXTREMA_INICIAL As TextBox = Nothing
            Dim ref_TextBoxFECHA_EXTREMA_FINAL As TextBox = Nothing
            Dim ref_TextBox_ayuda_conetedora As TextBox = Nothing
            Dim ref_TextBoxTEMA_EXPEDIENTE As TextBox = Nothing
            Dim ref_DropDownListArea As DropDownList = Nothing
            Dim ref_DropDownListSerie As DropDownList = Nothing
            Dim ref_DropDownListSubserie As DropDownList = Nothing
            Dim ref_DropDownListBoxtipoexpediente As DropDownList = Nothing
            Dim ref_DropDownList_tipo_unidad_contenedora As DropDownList = Nothing
            Dim ref_DropDownListsub_seccion As DropDownList = Nothing
            Dim matri_nombre_controles() As String = {"DropDownListorganigrama", _
                "TextBoxASUNTO_EXPEDIENTE", _
               "TextBoxCodigoManual", "TextBoxRANGO_EXTREMO_INICIAL", "TextBoxRANGO_EXTREMO_FINAL", "TextBoxFECHA_EXTREMA_INICIAL", _
               "TextBoxFECHA_EXTREMA_FINAL", "TextBox_ayuda_conetedora", "TextBoxTEMA_EXPEDIENTE", "DropDownListArea", "DropDownListSerie", _
               "DropDownListSubserie", "DropDownList_tipo_unidad_contenedora", "DropDownListsub_seccion"}
            Dim total_controles As Integer = matri_nombre_controles.Length
            For i As Integer = 0 To total_controles - 1
                Dim control_ob As Object = Nothing
                control_ob = update.FindControl(matri_nombre_controles(i))
                If control_ob Is Nothing Then
                    Asignar_Datos_edicion_unidad_conservacion = "Imposible encontrar el control " & matri_nombre_controles(i)
                    Exit Function
                End If
                Select Case matri_nombre_controles(i)
                    Case "DropDownListorganigrama"
                        ref_DropDownListorganigrama = control_ob
                    Case "TextBoxASUNTO_EXPEDIENTE"
                        ref_TextBoxASUNTO_EXPEDIENTE = control_ob
                    Case "TextBoxCodigoManual"
                        ref_TextBoxCodigoManual = control_ob
                    Case "TextBoxRANGO_EXTREMO_INICIAL"
                        ref_TextBoxRANGO_EXTREMO_INICIAL = control_ob
                    Case "TextBoxRANGO_EXTREMO_FINAL"
                        ref_TextBoxRANGO_EXTREMO_FINAL = control_ob
                    Case "TextBoxFECHA_EXTREMA_INICIAL"
                        ref_TextBoxFECHA_EXTREMA_INICIAL = control_ob
                    Case "TextBoxFECHA_EXTREMA_FINAL"
                        ref_TextBoxFECHA_EXTREMA_FINAL = control_ob
                    Case "TextBox_ayuda_conetedora"
                        ref_TextBox_ayuda_conetedora = control_ob
                    Case "TextBoxTEMA_EXPEDIENTE"
                        ref_TextBoxTEMA_EXPEDIENTE = control_ob
                    Case "DropDownListArea"
                        ref_DropDownListArea = control_ob
                    Case "DropDownListSerie"
                        ref_DropDownListSerie = control_ob
                    Case "DropDownListSubserie"
                        ref_DropDownListSubserie = control_ob
                    Case "DropDownListBoxtipoexpediente"
                        ref_DropDownListBoxtipoexpediente = control_ob
                    Case "DropDownList_tipo_unidad_contenedora"
                        ref_DropDownList_tipo_unidad_contenedora = control_ob
                    Case "DropDownListsub_seccion"
                        ref_DropDownListsub_seccion = control_ob
                End Select
            Next
            Dim estru_unidad_conservacion() As unidad_conservacion
            Erase estru_unidad_conservacion
            '-----------------------------------------------
            'Lista estructura unidad conservacion
            '-----------------------------------------------
            Result = Listar_datos_Unidad_Conservacion_estructura(ref_hdnEmailID.value, estru_unidad_conservacion)
            If Result <> "YES" Then
                Asignar_Datos_edicion_unidad_conservacion = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Lista tipos unidades de conservación
            '-----------------------------------------------
            Result = lista_tipos_unidades_Combo(ref_DropDownList_tipo_unidad_contenedora, 1)
            If Result <> "YES" Then
                Asignar_Datos_edicion_unidad_conservacion = Result
                Exit Function
            End If
            Dim Nombre_tipo_unidad As String = ""
            Result = Me.Retorna_nombre_tipo_unidad_conservacion(estru_unidad_conservacion(0).ID_TIPO_UNIDAD_CONSERVACION, Nombre_tipo_unidad)
            If Result <> "YES" Then
                Asignar_Datos_edicion_unidad_conservacion = Result
                Exit Function
            End If
            Result = Me.Retorna_descripcion_tipo_unidad_conservacion(ref_DropDownList_tipo_unidad_contenedora.Text, ref_TextBox_ayuda_conetedora.Text)
            If Result <> "YES" Then
                Asignar_Datos_edicion_unidad_conservacion = Result
                Exit Function
            End If
            If estru_unidad_conservacion(0).FECHA_EXTREMA_INICIAL <> "" Then
                Dim splifecha() As String = Left(estru_unidad_conservacion(0).FECHA_EXTREMA_INICIAL, 10).Split("/")
                ref_TextBoxFECHA_EXTREMA_INICIAL.Text = splifecha(2) & "/" & splifecha(1) & "/" & splifecha(0)
            End If
            If estru_unidad_conservacion(0).FECHA_EXTREMA_FINAL <> "" Then
                Dim splifecha() As String = Left(estru_unidad_conservacion(0).FECHA_EXTREMA_FINAL, 10).Split("/")
                ref_TextBoxFECHA_EXTREMA_FINAL.Text = splifecha(2) & "/" & splifecha(1) & "/" & splifecha(0)
            End If
            ref_TextBoxRANGO_EXTREMO_INICIAL.Text = estru_unidad_conservacion(0).RANGO_EXTREMO_INICIAL
            ref_TextBoxRANGO_EXTREMO_FINAL.Text = estru_unidad_conservacion(0).RANGO_EXTREMO_FINAL
            ref_TextBoxTEMA_EXPEDIENTE.Text = estru_unidad_conservacion(0).TEMA_UNIDAD_CONSERVACION
            ref_TextBoxASUNTO_EXPEDIENTE.Text = estru_unidad_conservacion(0).DESCRIPCION_UNIDAD_CONSERVACION
            ref_TextBoxCodigoManual.Text = estru_unidad_conservacion(0).CODIGO_UNICO
            Dim codigo_area As Integer = 0
            Dim NOMBRE_AREA_TRD As String = ""
            Dim CODIGO_SERIE_TRD As Integer = 0
            Dim NOMBRE_SERIE_TRD As String = ""
            Dim CODIGO_SUB_SERIE_TRD As Integer = 0
            Dim NOMBRE_SUBSERIE_TRD As String = ""
            Dim ra_tipo_unidad_conservacion As Integer = 0
            ra_tipo_unidad_conservacion = estru_unidad_conservacion(0).ID_TIPO_UNIDAD_CONSERVACION
            NOMBRE_AREA_TRD = estru_unidad_conservacion(0).NOMBRE_AREA
            codigo_area = estru_unidad_conservacion(0).CODIGO_AREA_TRD
            CODIGO_SERIE_TRD = estru_unidad_conservacion(0).CODIGO_SERIE
            NOMBRE_SERIE_TRD = estru_unidad_conservacion(0).NOMBRE_SERIE
            CODIGO_SUB_SERIE_TRD = estru_unidad_conservacion(0).CODIGO_SUBSERIE
            NOMBRE_SUBSERIE_TRD = estru_unidad_conservacion(0).NOMBRE_SUBSERIE
            id_empresa = estru_unidad_conservacion(0).ID_EMPRESA
            Dim id_organigrama As Integer = 0
            Dim nombre_organigrama As String = ""
            Dim Refclasunidad As New ClassUnidadConservacion
            Dim Refclas_organigrama As New Class_registro_organigrama
            Result = Refclas_organigrama.Listar_Organigramas_Empresa_Combo(id_empresa, _
                                                                           ref_DropDownListorganigrama, _
                                                                           update)
            If Result <> "YES" Then
                Asignar_Datos_edicion_unidad_conservacion = Result
                Exit Function
            End If
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            Result = Class_areas_depart_radicacion.Lista_datos_organigrama_por_codigo_area(codigo_area, _
                                                                                           id_organigrama, _
                                                                                           nombre_organigrama)
            If Result <> "YES" Then
                Asignar_Datos_edicion_unidad_conservacion = Result
                Exit Function
            End If
            ref_DropDownListorganigrama.Text = nombre_organigrama
            Dim Refclas_ As New ClassGestionDocumental
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
                Result = Refclas_.Retorna_Areas_Departamento_Radicacion(id_empresa, nombre_organigrama, _
                ref_DropDownListArea)
                If Result <> "YES" Then
                    Asignar_Datos_edicion_unidad_conservacion = Result
                    Exit Function
                End If
            Else
                Result = Refclas_.lista_areas_permitidas_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), ref_DropDownListArea)
                If Result <> "YES" Then
                    Asignar_Datos_edicion_unidad_conservacion = Result
                    Exit Function
                End If
            End If
            Dim Refclas_dos As New ClassGestionDocumental
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If estru_unidad_conservacion(0).CODIGO_AREA_TRD <> 0 Then
                Result = Refclas_dos.Retorna_Sub_Areas_Departamento_Radicacion(estru_unidad_conservacion(0).CODIGO_AREA_TRD, ref_DropDownListsub_seccion, estru_unidad_conservacion(0).NOMBRE_SUB_AREA)
                If Result <> "YES" Then
                    Asignar_Datos_edicion_unidad_conservacion = "Función Asignar_Datos_edicion_unidad_conservacion dice " & Result
                    Exit Function
                End If
            Else
                If ref_DropDownListArea.Text <> "" Then
                    Dim id_area As Integer = 0
                    Result = ref_Class_areas_depart_radicacion.Retorna_cod_Area_Departamento(id_organigrama, _
                                                                                             id_area, _
                                                                                             ref_DropDownListArea.Text)
                    If Result <> "YES" Then
                        Asignar_Datos_edicion_unidad_conservacion = "Función Asignar_Datos_edicion_unidad_conservacion dice " & Result
                        Exit Function
                    End If
                    Result = Refclas_dos.Retorna_Sub_Areas_Departamento_Radicacion(id_area, ref_DropDownListsub_seccion)
                    If Result <> "YES" Then
                        Asignar_Datos_edicion_unidad_conservacion = "Función Asignar_Datos_edicion_unidad_conservacion dice " & Result
                        Exit Function
                    End If
                End If
            End If
            If NOMBRE_AREA_TRD = "" Then
                Asignar_Datos_edicion_unidad_conservacion = "YES"
                Exit Function
            End If
            ref_DropDownListArea.Text = NOMBRE_AREA_TRD
            Result = Refclas_.Listar_Entidad_Series_Documentales(id_empresa, nombre_organigrama, ref_DropDownListSerie, NOMBRE_AREA_TRD)
            If Result <> "YES" Then
                Asignar_Datos_edicion_unidad_conservacion = Result
                Exit Function
            End If
            If CODIGO_SERIE_TRD <> 0 Then
                ref_DropDownListSerie.Text = NOMBRE_SERIE_TRD
                Result = Refclas_.Listar_Entidad_Sub_Series_Documentales(id_empresa, nombre_organigrama, ref_DropDownListSubserie, NOMBRE_AREA_TRD, NOMBRE_SERIE_TRD)
                If Result <> "YES" Then
                    Asignar_Datos_edicion_unidad_conservacion = Result
                    Exit Function
                End If
            End If
            If CODIGO_SUB_SERIE_TRD <> 0 Then
                ref_DropDownListSubserie.Text = NOMBRE_SUBSERIE_TRD
            End If
            Asignar_Datos_edicion_unidad_conservacion = "YES"
        Catch ex As Exception
            Asignar_Datos_edicion_unidad_conservacion = "inconsistencia funcion Asignar_Datos_edicion_unidad_conservacion " & ex.Message
        End Try
    End Function
    Function Verifica_propiedad_usuario_unidad_conservacion(ByVal id_unidad_conservacion As Integer, _
    ByVal id_usuario_gstion As Integer) As String
        '************************************************************
        'Funcion : Verfica si el usuario es propiestario de
        'la unidad de conservacion
        'Fecha 2014-09-27 Función modificada para la web 2016-09-10
        'Ing Migeuel Angel Urueta Miranda
        '************************************************************
        Try
            Dim SqlConsulta As String = "select * from  unidad_conservacion " & _
            " where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion & " and ID_USUARIO_GESTION=" & id_usuario_gstion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Verifica_propiedad_usuario_unidad_conservacion = " Error de conexión función  Verifica_propiedad_usuario_unidad_conservacion " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_propiedad_usuario_unidad_conservacion = "Usted no es propietario de la unidad de conservacion " & vbCrLf & _
                " no puede ejecutar ninguna accion "
            Else
                Verifica_propiedad_usuario_unidad_conservacion = "YES"
            End If
        Catch ex As Exception
            Verifica_propiedad_usuario_unidad_conservacion = "Inconsistencia funcion Verifica_propiedad_usuario_unidad_conservacion " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_tipo_unidad_conservacion_por_id_unidad_conservacion(ByVal id_unidad_conservacion As Integer, _
    ByRef nombre_tipo_unidad_conservacion As String) As String
        '*********************************************************************
        'Funcion : Retorna nombre tipo unidad de conservación con el iden
        'tificador de la unidad de conservbacion registrada en la tabla
        'unidad_conservacion, haciendo cartesianidad con la tabla
        'tipo_unidad_conservacion con el filtro de id unidad de conservacion
        'Fecha 2015-01-26 Modificado para web 2016-09-15
        'Ingeniero : Miguel Angel Urueta Miranda
        '*********************************************************************
        Try
            Dim sqlconsulta As String = "Select tuc.NOMBRE_TIPO_UNIDAD from unidad_conservacion as uc " & _
            " inner join tipo_unidad_conservacion as tuc on (tuc.ID_TIPO_UNIDAD=uc.ID_TIPO_UNIDAD_CONSERVACION)" & _
            " where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            'Dim Ref_Car_Conec As New Conect.vb.Dbase_Conction_Mysql
            'Dim Dat_reader As MySqlDataReader = Ref_Car_Conec.C_Dareader_Mysql(CONEXIONGESTOR, sqlconsulta)
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                Retorna_nombre_tipo_unidad_conservacion_por_id_unidad_conservacion = " Error Conexión Base Datos función " & _
                " Retorna_nombre_tipo_unidad_conservacion_por_id_unidad_conservacion " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_tipo_unidad_conservacion_por_id_unidad_conservacion = "Imposible el id del tipo de unidad de conservación "
                Exit Function
            Else
                nombre_tipo_unidad_conservacion = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_tipo_unidad_conservacion_por_id_unidad_conservacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_tipo_unidad_conservacion_por_id_unidad_conservacion = "Inconsistencia Función " & vbCrLf & _
            "  Retorna_nombre_tipo_unidad_conservacion_por_id_unidad_conservacion " & ex.Message
        End Try
    End Function
    Function Listar_datos_Unidad_Conservacion_estructura(ByVal id_unidad_conservacion As Integer, _
     ByRef estru_unidad_conservacion() As unidad_conservacion) As String
        '************************************************************
        'Funcion Listar estrucutura unidad de conservacion con el
        'parametro id entrepaño
        'Fecha 2014-09-25
        'Ing : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            Erase estru_unidad_conservacion
            Dim campos_seleccion As String = "ID_UNIDAD_CONSERVACION,ENTRE_PAÑO_ID_ENTREPAÑO,CONSECUTIVO_UNIDAD_CONSERVACION," & _
            "CONSECUTIVO_EXPEDIENTE,CONSECUTIVO_DOCUMENTO,CODIGO_CORTO,CODIGO_UNICO,TIPO_UNIDAD_CONSERVACION,NUMERO_FOLIO_UNIDAD_CONSERVACION," & _
            "ID_USUARIO_GESTION,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA,CODIGO_SERIE,NOMBRE_SERIE,CODIGO_SUBSERIE,NOMBRE_SUBSERIE," & _
            "ESTADO_UNIDAD_CONSERVACION,ESTADO_ARCHIVO_INIDAD,ID_TIPO_UNIDAD_CONSERVACION,FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL," & _
            "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_UNIDAD_CONSERVACION,DESCRIPCION_UNIDAD_CONSERVACION,CODIGO_BARRAS_UNIDAD," & _
            "ID_EMPRESA_UNIDAD,ID_UNIDAD_CONSERVACION_TOPLOGICA,UNIDAD_PADRE,VOLUMEN_UNIDAD_CONSERVACION,ID_SUB_AREA,NOMBRE_SUB_AREA,id_instrumento"
            Dim SqlConsulta As String = "select " & campos_seleccion & " from  unidad_conservacion " & _
                                             " where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Listar_datos_Unidad_Conservacion_estructura = " Error solicitando estrucutura unidad conservacion " & SqlConsulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_unidad_conservacion(i)
                    estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_UNIDAD_CONSERVACION")
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = 0
                    Else
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = Datset.Tables(0).Rows(i).Item(1)
                    End If
                    estru_unidad_conservacion(i).CONSECUTIVO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).CONSECUTIVO_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_EXPEDIENTE")
                    estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_DOCUMENTO")
                    estru_unidad_conservacion(i).CODIGO_CORTO = Datset.Tables(0).Rows(i).Item("CODIGO_CORTO")
                    estru_unidad_conservacion(i).CODIGO_UNICO = Datset.Tables(0).Rows(i).Item("CODIGO_UNICO")
                    estru_unidad_conservacion(i).TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TIPO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).NUMERO_FOLIO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("NUMERO_FOLIO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).ID_USUARIO_GESTION = Datset.Tables(0).Rows(i).Item("ID_USUARIO_GESTION")
                    If Datset.Tables(0).Rows(0).IsNull(10) = True Then
                        estru_unidad_conservacion(i).FECHA_CREACION = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_CREACION = Datset.Tables(0).Rows(i).Item("FECHA_CREACION").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(11) = True Then
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = Datset.Tables(0).Rows(i).Item("CODIGO_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(12) = True Then
                        estru_unidad_conservacion(i).NOMBRE_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_AREA = Datset.Tables(0).Rows(i).Item("NOMBRE_AREA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(13) = True Then
                        estru_unidad_conservacion(i).CODIGO_SERIE = 0
                    Else
                        estru_unidad_conservacion(i).CODIGO_SERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(14) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(15) = True Then
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = 0
                    Else
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SUBSERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(16) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SUBSERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(17) = True Then
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ESTADO_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(18) = True Then
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = 0
                    Else
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = Datset.Tables(0).Rows(i).Item("ESTADO_ARCHIVO_INIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(19) = True Then
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_TIPO_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(20) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_INICIAL").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(21) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_FINAL").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(22) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(23) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(24) = True Then
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TEMA_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(25) = True Then
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("DESCRIPCION_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(26) = True Then
                        estru_unidad_conservacion(i).CODIGO_BARRAS_UNIDAD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_BARRAS_UNIDAD = Datset.Tables(0).Rows(i).Item("CODIGO_BARRAS_UNIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(27) = True Then
                        estru_unidad_conservacion(i).ID_EMPRESA = 0
                    Else
                        estru_unidad_conservacion(i).ID_EMPRESA = Datset.Tables(0).Rows(i).Item("ID_EMPRESA_UNIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(28) = True Then
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION_TOPLOGICA = 0
                    Else
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION_TOPLOGICA = Datset.Tables(0).Rows(i).Item("ID_UNIDAD_CONSERVACION_TOPLOGICA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(29) = True Then
                        estru_unidad_conservacion(i).UNIDAD_PADRE = 0
                    Else
                        estru_unidad_conservacion(i).UNIDAD_PADRE = Datset.Tables(0).Rows(i).Item("UNIDAD_PADRE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(30) = True Then
                        estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("VOLUMEN_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(31) = True Then
                        estru_unidad_conservacion(i).ID_SUB_AREA = 1
                    Else
                        estru_unidad_conservacion(i).ID_SUB_AREA = Datset.Tables(0).Rows(i).Item("ID_SUB_AREA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(32) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUB_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUB_AREA = Datset.Tables(0).Rows(i).Item("NOMBRE_SUB_AREA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(33) = True Then
                        estru_unidad_conservacion(i).id_instrumento = 0
                    Else
                        estru_unidad_conservacion(i).id_instrumento = Datset.Tables(0).Rows(i).Item("id_instrumento")
                    End If

                Next

                Listar_datos_Unidad_Conservacion_estructura = "YES"
                Exit Function
            Else
                Listar_datos_Unidad_Conservacion_estructura = "Imposible encontrar estructura unidad de conservacion"
                Exit Function
            End If

        Catch ex As Exception
            Listar_datos_Unidad_Conservacion_estructura = "Inconsistencia general funcion Listar_datos_Unidad_Conservacion_estructura " & ex.Message
        End Try
    End Function
    Function Genera_Codigo_largo_Unidad_Conservacion(ByRef codigo_unida_conservacion As String, _
    ByVal tipo_unidad_conservacion As Integer, _
    ByVal id_tipo_unidad_conservacion As Integer, _
    ByVal consecutivo_unidad As Integer, ByVal año As String, ByVal disper As Integer) As String
        Try
            Dim ref_id_tipo_unidad_conservacion As String = id_tipo_unidad_conservacion
            Dim Result As String = ""
            'Result = zero_fill(ref_id_tipo_unidad_conservacion, 4, "0")
            'If Result <> "YES" Then
            '    Genera_Codigo_largo_Unidad_Conservacion = Result
            '    Exit Function
            'End If
            Dim ref_consecutivo_unidad As String = consecutivo_unidad
            Result = zero_fill(ref_consecutivo_unidad, 8, "0")
            If Result <> "YES" Then
                Genera_Codigo_largo_Unidad_Conservacion = Result
                Exit Function
            End If
            codigo_unida_conservacion = año & tipo_unidad_conservacion & ref_id_tipo_unidad_conservacion & ref_consecutivo_unidad
            Genera_Codigo_largo_Unidad_Conservacion = "YES"
        Catch ex As Exception
            Genera_Codigo_largo_Unidad_Conservacion = "Iconsistencia general funcion Genera_Codigo_largo_Unidad_Conservacion " & ex.Message
        End Try
    End Function
    Function Genera_Codigo_corto_Unidad_Conservacion(ByRef codigo_unida_conservacion As String, _
       ByVal tipo_unidad_conservacion As Integer, _
       ByVal id_tipo_unidad_conservacion As Integer, _
       ByVal consecutivo_unidad As Integer, ByVal año As String) As String
        Try
            Dim ref_id_tipo_unidad_conservacion As String = id_tipo_unidad_conservacion
            Dim Result As String = ""
            Result = zero_fill(ref_id_tipo_unidad_conservacion, 4, "0")
            If Result <> "YES" Then
                Genera_Codigo_corto_Unidad_Conservacion = Result
                Exit Function
            End If
            codigo_unida_conservacion = año & "-" & tipo_unidad_conservacion & "-" & ref_id_tipo_unidad_conservacion & "-" & consecutivo_unidad
            Genera_Codigo_corto_Unidad_Conservacion = "YES"
        Catch ex As Exception
            Genera_Codigo_corto_Unidad_Conservacion = "Iconsistencia general funcion " & ex.Message
        End Try
    End Function

    Function Retorna_nombre_tipo_unidad_conservacion(ByVal id_tipo_documento As Integer, _
    ByRef nombre_tipo_unidad As String) As String
        '******************************************************
        'Funcion : Retorna elnombre del tipo unidad enviando
        'como parametro el id unidad documental
        'Fecha : 2015-01-14
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Dim sqlconsulta As String = "Select NOMBRE_TIPO_UNIDAD from tipo_unidad_conservacion where " & _
               "  ID_TIPO_UNIDAD='" & id_tipo_documento & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                Retorna_nombre_tipo_unidad_conservacion = " Error Conexión Base Datos función Retorna_tipo_id_unidad" & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_tipo_unidad_conservacion = "Imposible encontrar el nombre del tipo de unidad de conservación "
                Exit Function
            Else
                nombre_tipo_unidad = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_tipo_unidad_conservacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_tipo_unidad_conservacion = "Inconsistencia función Retorna_nombre_tipo_unidad_conservacion " & ex.Message
        End Try
    End Function
    Function Verfica_Existencia_Codigo_Unico_unidad_conservacion(ByVal tipo_unidad_conservacion As Integer, _
                                                                 ByVal id_tipo_unidad_conservacion As Integer, _
                                                                 ByVal codigo_unidad_conservacion As String, _
                                                                 ByVal id_empresa As Integer, _
                                                                 ByVal volumen_unidad_conservacion As Integer) As String
        '*********************************************************
        'Funcion Verfica existencia codigo unidad de conservacion
        'Fecha 2014-09-25
        'Ing Miguel Angel Urueta Miranda
        '**********************************************************
        Try
            Dim SqlConsulta As String = "SELECT * " & _
                    " FROM  unidad_conservacion where ID_TIPO_UNIDAD_CONSERVACION=" & id_tipo_unidad_conservacion & _
                    " AND TIPO_UNIDAD_CONSERVACION=" & tipo_unidad_conservacion & " AND CODIGO_UNICO='" & _
                    codigo_unidad_conservacion & "' and ID_EMPRESA_UNIDAD=" & id_empresa & " AND VOLUMEN_UNIDAD_CONSERVACION=" & _
                    volumen_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Verfica_Existencia_Codigo_Unico_unidad_conservacion = " Error listando tipo_unidad_conservacion" & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verfica_Existencia_Codigo_Unico_unidad_conservacion = "El codigo unico se encuentra regsitrado "
                Exit Function
            Else
                Verfica_Existencia_Codigo_Unico_unidad_conservacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verfica_Existencia_Codigo_Unico_unidad_conservacion = "Inconsistencia Funcion Verfica_Existencia_Codigo_Unico_unidad_conservacion " & ex.Message
        End Try

    End Function
    Function Verfica_Existencia_Codigo_Unico_unidad_conservacion( _
                                                                 ByVal codigo_unidad_conservacion As String, _
                                                                 ByVal id_empresa As Integer) As String
        '*********************************************************
        'Funcion Verfica existencia codigo unidad de conservacion
        'Fecha 2014-09-25
        'Ing Miguel Angel Urueta Miranda
        '**********************************************************
        Try
            Dim SqlConsulta As String = "SELECT * " & _
                    " FROM  unidad_conservacion where " & _
                    " CODIGO_UNICO='" & _
                    codigo_unidad_conservacion & "' and ID_EMPRESA_UNIDAD=" & id_empresa
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Verfica_Existencia_Codigo_Unico_unidad_conservacion = " Error listando tipo_unidad_conservacion" & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verfica_Existencia_Codigo_Unico_unidad_conservacion = "El codigo unico se encuentra regsitrado "
                Exit Function
            Else
                Verfica_Existencia_Codigo_Unico_unidad_conservacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verfica_Existencia_Codigo_Unico_unidad_conservacion = "Inconsistencia Funcion Verfica_Existencia_Codigo_Unico_unidad_conservacion " & ex.Message
        End Try

    End Function
    Function Solicita_numero_unidades_anidadas_entre_pano(ByVal id_entrepano As Integer, _
                                                           ByRef numero_unidades_anidadas As Integer) As String
        '----------------------------------------------------
        'Función : Solicita el número de unidades anidadas
        'en un entrepaño por el id del entrepaño
        'Fecha : 2018-02-16
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------
        Try
            Dim SqlConsulta As String = "SELECT ID_UNIDAD_CONSERVACION " & _
                   " FROM  unidad_conservacion where ENTRE_PAÑO_ID_ENTREPAÑO=" & id_entrepano
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño where")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Solicita_numero_unidades_anidadas_entre_pano = " Error función  Solicita_numero_unidades_permitidos_entrepano " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                numero_unidades_anidadas = Datset.Tables(0).Rows.Count
                Solicita_numero_unidades_anidadas_entre_pano = "YES"
                Exit Function
            Else
                numero_unidades_anidadas = 0
                Solicita_numero_unidades_anidadas_entre_pano = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_unidades_anidadas_entre_pano = "Inconsistencia general función Solicita_numero_unidades_anidadas_entre_pano " & ex.Message
        End Try
    End Function
    Function Solicita_numero_unidades_permitidos_entrepano(ByVal id_entrepano As Integer, _
                                                           ByRef numero_unidades_permitidas As Integer) As String
        '----------------------------------------------------------------------
        'Función : Retorna el número de unidades permitidas de un entrepaño
        'Fecha : 2018-02-16
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------
        Try
            Dim SqlConsulta As String = "SELECT NUMERO_UNIDADES_PERMITIDAS " & _
                    " FROM  entre_paño where ID_ENTREPAÑO=" & id_entrepano
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño where")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Solicita_numero_unidades_permitidos_entrepano = " Error función  Solicita_numero_unidades_permitidos_entrepano " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_numero_unidades_permitidos_entrepano = "Imposible encontrar el número  de unidades permitidas del entre paño (" & id_entrepano & ")"
                Exit Function
            Else
                numero_unidades_permitidas = Datset.Tables(0).Rows(0).Item(0)
                Solicita_numero_unidades_permitidos_entrepano = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_unidades_permitidos_entrepano = "Inconsistencia general función Solicita_numero_unidades_permitidos_entrepano " & ex.Message
        End Try
    End Function
    Function Registrar_Unidad_Conservacion(ByVal id_usuario_gestion As Integer, ByVal id_entrepaño As Integer, _
    ByVal codigo_unico As String, ByVal estado_codigo_unico As Integer, ByVal tipo_unidad_conservacion As Integer, _
    ByVal id_tipo_unidad_conservacion As Integer, ByVal fecha_extrema_incial As String, ByVal fecha_extrema_final As String, _
    ByVal rango_extremo_inicial As String, ByVal rango_extremo_final As String, ByVal tema_unidad_conservacion As String, _
    ByVal descripcion_unidad_conservacion As String, _
    ByVal volumen_unidad As Integer, ByVal id_empresa_gestion As Integer, ByVal nombre_organigrama As String, ByVal nombre_area As String, _
    ByVal nombre_serie As String, ByVal nombre_sub_serie As String, ByRef id_unidad As Integer, ByRef codigo_unidad As String, _
    ByVal nombre_sub_seccion As String, ByVal id_organigrama As Object, _
    ByVal id_area As Object, ByVal id_serie As Object, ByVal id_sub_serie As Object, ByVal id_instrumento As Object, _
    Optional ByVal tre As TreeNode = Nothing, _
    Optional ByVal estado_archivo As Integer = 0) As String
        Dim Result As String = "YES"
        '-------------------------------------------------
        'Verfica el codigo es manual
        '-------------------------------------------------
        'If estado_codigo_unico = 1 Then
        '    If codigo_unico = "" Then
        '        Registrar_Unidad_Conservacion = "Debe informar el código único"
        '        Exit Function
        '    End If
        '    Result = Verfica_Existencia_Codigo_Unico_unidad_conservacion(tipo_unidad_conservacion, _
        '    id_tipo_unidad_conservacion, codigo_unico, id_empresa_gestion, volumen_unidad)
        '    If Result <> "YES" Then
        '        Registrar_Unidad_Conservacion = Result
        '        Exit Function
        '    End If
        'End If
        If nombre_organigrama = "" Then
            Registrar_Unidad_Conservacion = "Por favor seleccione el organigrama"
            Exit Function
        End If

        If nombre_area = "" Then
            Registrar_Unidad_Conservacion = "Por favor seleccione el area"
            Exit Function
        End If
        '--------------------------------------------------------
        'Solicita el número de unidades permitidas para anidar
        '--------------------------------------------------------
        Dim numero_unidades_anidadas As Integer = 0
        Dim numero_unidades_permitidas As Integer = 0
        If id_entrepaño <> 0 Then
            Result = Me.Solicita_numero_unidades_permitidos_entrepano(id_entrepaño, numero_unidades_permitidas)
            If Result <> "YES" Then
                Registrar_Unidad_Conservacion = Result
                Exit Function
            End If
            Result = Solicita_numero_unidades_anidadas_entre_pano(id_entrepaño, numero_unidades_anidadas)
            If Result <> "YES" Then
                Registrar_Unidad_Conservacion = Result
                Exit Function
            End If
            '----------------------------------
            'Compara numero unidades permitidas
            '----------------------------------
            If numero_unidades_anidadas >= numero_unidades_permitidas Then
                Registrar_Unidad_Conservacion = "El entrapaño (" & id_entrepaño & ") no permite anidar más unidades, supero el límite unidades de conservación permitidas. " & _
                                                " Las unidades almacenadas en el entrepaño son (" & numero_unidades_anidadas & ") de  (" & numero_unidades_permitidas & " ) unidad(es) permitida(s)"
                Exit Function
            End If
        End If
        Result = "YES"
        
        '---------------------------------------------
        'Retorna codigo subseccion
        '---------------------------------------------
        Dim id_sub_seccion As Object = 0
        
        '-------------------------------------------
        'Retorna nombre tipo unidad de conservación
        '-------------------------------------------
        Dim nombre_unidad_conservacion As String = ""
        Result = Me.Retorna_nombre_tipo_unidad_conservacion(id_tipo_unidad_conservacion, nombre_unidad_conservacion)
        If Result <> "YES" Then
            Registrar_Unidad_Conservacion = Result
            Exit Function
        End If
        Dim ref_nombre_serie As String = "null"
        If nombre_serie = "" Then
            ref_nombre_serie = "null"
        Else
            ref_nombre_serie = "'" & nombre_serie & "'"
        End If
        Dim ref_nombre_sub_serie As String = "null"
        If nombre_sub_serie = "" Then
            ref_nombre_sub_serie = "null"
        Else
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        If id_serie = 0 Then
            id_serie = "null"
        End If
        If id_sub_serie = 0 Then
            id_sub_serie = "null"
        End If
        '---------------------------------------
        'Vefica formato fechas extremas
        '---------------------------------------
        Dim re_fecha_extrema_incial As String = ""
        Dim re_fecha_extrema_final As String = ""
        If fecha_extrema_incial <> "" Then
            Dim splifecha() As String = fecha_extrema_incial.Split("/")
            re_fecha_extrema_incial = "'" & fecha_extrema_incial & "'"
        Else
            re_fecha_extrema_incial = "null"
        End If
        If fecha_extrema_final <> "" Then
            Dim splifecha() As String = fecha_extrema_final.Split("/")
            re_fecha_extrema_final = "'" & fecha_extrema_final & "'"
        Else
            re_fecha_extrema_final = "null"
        End If

        Dim ref_rango_extremo_inicial As String = ""
        If rango_extremo_inicial = "" Then
            ref_rango_extremo_inicial = "null"
        Else
            ref_rango_extremo_inicial = "'" & rango_extremo_inicial & "'"
        End If
        Dim ref_rango_extremo_final As String = ""
        If rango_extremo_final = "" Then
            ref_rango_extremo_final = "null"
        Else
            ref_rango_extremo_final = "'" & rango_extremo_final & "'"
        End If
        Dim ref_tema_unidad_conservacion As String = ""
        If tema_unidad_conservacion = "" Then
            ref_tema_unidad_conservacion = "null"
        Else
            ref_tema_unidad_conservacion = "'" & tema_unidad_conservacion & "'"
        End If
        Dim ref_descripcion_unidad_conservacion As String = ""
        If descripcion_unidad_conservacion = "" Then
            ref_descripcion_unidad_conservacion = "null"
        Else
            ref_descripcion_unidad_conservacion = "'" & descripcion_unidad_conservacion & "'"
        End If
        Dim re_nombre_sub_seccion As String = ""
        If nombre_sub_seccion = "" Then
            re_nombre_sub_seccion = "null"
        Else
            re_nombre_sub_seccion = "'" & nombre_sub_seccion & "'"
        End If
        If id_sub_seccion = 0 Then
            id_sub_seccion = "null"
        End If
        If id_instrumento = 0 Then
            id_instrumento = "Null"
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Registrar_Unidad_Conservacion = Result
            Exit Function
        End If
        Dim date_registro As String = Date.Today
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_registro)
        If Result <> "YES" Then
            Registrar_Unidad_Conservacion = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad As String = 0
        Dim errorM As String = "YES"
        Try
            Dim sqlforupdate As String = "Select CONSECUTIVO_UNIDAD  from tipo_unidad_conservacion where ID_TIPO_UNIDAD=" & id_tipo_unidad_conservacion & _
                " for update"
            Dim dat_reader As MySqlDataReader
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            dat_reader = myCommand.ExecuteReader()
            If dat_reader Is Nothing Then
                Registrar_Unidad_Conservacion = "Imposible Encontrar consecutivo MODULO error de conexion"
                errorM = "Imposible Encontrar consecutivo MODULO error de conexion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = False Then
                Registrar_Unidad_Conservacion = "Imposible Encontrar consecutivo de la unidad de conservacion"
                errorM = "Imposible Encontrar consecutivo de la unidad de conservacion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = True Then
                dat_reader.Read()
                consecutivo_unidad = dat_reader.Item(0)
                dat_reader.Close()
            End If
            consecutivo_unidad = consecutivo_unidad + 1
            Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
            '-------------------------------------------
            'Asigna codigo corto unidad
            '-------------------------------------------
            Dim codigo_corto_unidad As String = ""
            Result = Genera_Codigo_corto_Unidad_Conservacion(codigo_corto_unidad, tipo_unidad_conservacion, _
            id_tipo_unidad_conservacion, consecutivo_unidad, año_radic)
            If Result <> "YES" Then
                Registrar_Unidad_Conservacion = Result
                errorM = Result
                Exit Function
            End If
            '-------------------------------------------
            'Asigna codigo unico
            '-------------------------------------------
            If estado_codigo_unico = 0 Then
                Result = Genera_Codigo_largo_Unidad_Conservacion(codigo_unico, tipo_unidad_conservacion, _
            id_tipo_unidad_conservacion, consecutivo_unidad, año_radic, 1)
                If Result <> "YES" Then
                    Registrar_Unidad_Conservacion = Result
                    errorM = Result
                    Exit Function
                End If
            End If
            '--------------------------------------------
            'Agregar valores insercion
            '--------------------------------------------
            Dim sqlcampos_insert As String = "Insert into unidad_conservacion (ENTRE_PAÑO_ID_ENTREPAÑO,CONSECUTIVO_UNIDAD_CONSERVACION," & _
            "CODIGO_CORTO,CODIGO_UNICO,TIPO_UNIDAD_CONSERVACION,ID_USUARIO_GESTION,FECHA_CREACION,ID_TIPO_UNIDAD_CONSERVACION," & _
            "FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL,RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_UNIDAD_CONSERVACION," & _
            "DESCRIPCION_UNIDAD_CONSERVACION,CODIGO_BARRAS_UNIDAD,ID_EMPRESA_UNIDAD,CODIGO_AREA_TRD,NOMBRE_AREA," & _
            "CODIGO_SERIE,NOMBRE_SERIE,CODIGO_SUBSERIE,NOMBRE_SUBSERIE,ESTADO_ARCHIVO_INIDAD,ID_SUB_AREA,NOMBRE_SUB_AREA,id_instrumento,fecha_registro) values "
            Dim sqlinsert_campos As String = "(" & id_entrepaño & "," & "0" & ",'" & codigo_corto_unidad & "','" & codigo_unico & "'," & _
            tipo_unidad_conservacion & "," & id_usuario_gestion & ",'" & date1al & "'," & id_tipo_unidad_conservacion & "," _
            & re_fecha_extrema_incial & "," & re_fecha_extrema_final & "," & ref_rango_extremo_inicial & "," & ref_rango_extremo_final & "," & _
            ref_tema_unidad_conservacion & "," & ref_descripcion_unidad_conservacion & ",'" & codigo_corto_unidad & "'," & id_empresa_gestion & _
            "," & id_area & ",'" & nombre_area & "'," & id_serie & "," & ref_nombre_serie & "," & id_sub_serie & _
             "," & ref_nombre_sub_serie & "," & estado_archivo & "," & id_sub_seccion & "," & re_nombre_sub_seccion & "," & id_instrumento & _
             ",'" & date_registro & "')"
            Dim sqlinsertcion As String = sqlcampos_insert & sqlinsert_campos
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registrar_Unidad_Conservacion = "Imposible registrar unidad de conservación  : " & sqlinsertcion
                'myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar unidad de conservacion  : " & sqlinsertcion
                Exit Function
            End If
            Dim objet As Object = myCommand.LastInsertedId
            Dim updatconsecutivo As String = "UPDATE tipo_unidad_conservacion SET CONSECUTIVO_UNIDAD=" & _
            consecutivo_unidad & "  where ID_TIPO_UNIDAD=" & id_tipo_unidad_conservacion
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim tipo_unidad As String = ""
            tipo_unidad = "TIPO UNIDAD : (" & nombre_unidad_conservacion & ")  "
            codigo_unidad = tipo_unidad & "CODIGO UNICO: " & codigo_unico & " TEMA: " & _
            tema_unidad_conservacion & _
            " FECHAS EXTREMAS: " & fecha_extrema_incial & " HASTA " & _
            fecha_extrema_final & " RANGO EXTREMOS:" & rango_extremo_inicial & " HASTA " & _
            rango_extremo_final & "-1"
            'End If
            id_unidad = objet
            myTrans.Commit()
            myConnection.Close()
            Registrar_Unidad_Conservacion = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Registrar_Unidad_Conservacion = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registrar_Unidad_Conservacion = errorM

        End Try
    End Function
    Function Eliminar_unidad_conservacion_contenedora_unidad_treview(ByVal id_unidad_conservacion As Integer, _
    ByRef node As TreeNode, ByVal id_usuario_gestion As Integer, ByRef trevi As TreeView, _
    ByVal user_Gestion As String, ByVal iptrans As String, ByRef update As UpdatePanel) As String
        '*********************************************************
        'Función : Elimina unidad de conservacion, verificando
        'el registro de contenido
        'Fecha : 2015-01-22
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Dim Result As String = ""
        Dim existencia_regi_unidad As String = "YES"
        Result = Me.Verifica_existencia_unidad_conservacion_contenedora_unidad_archivado(id_unidad_conservacion, existencia_regi_unidad)
        If Result <> "YES" Then
            Eliminar_unidad_conservacion_contenedora_unidad_treview = Result
            Exit Function
        End If
        If existencia_regi_unidad = "YES" Then
            Eliminar_unidad_conservacion_contenedora_unidad_treview = "La unidad de conservación no se puede eliminar por que tiene unidades relacionadas"
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Eliminar_unidad_conservacion_contenedora_unidad_treview = Result
            Exit Function
        End If

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try


            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "Delete from unidad_conservacion where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_unidad_conservacion_contenedora_unidad_treview = "Imposible eliminar unidad de conservación  : " & sqlinsertcion
                errorM = "Imposible eliminar unidad de conservación  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_unidad_conservacion (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" & _
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) values (" & _
           "'ELIMINA UNIDAD','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," & _
           id_unidad_conservacion & ",'" & iptrans & "','" & hor & "','DOCUARCHI')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualizar los unidad de conservación  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            trevi.Nodes.Remove(trevi.SelectedNode)
            Dim sNodo As TreeNode = trevi.SelectedNode
            Dim pNodo As TreeNode = sNodo.Parent
            pNodo.ChildNodes.Remove(sNodo)
            update.Update()
            myTrans.Commit()
            myConnection.Close()
            Eliminar_unidad_conservacion_contenedora_unidad_treview = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Eliminar_unidad_conservacion_contenedora_unidad_treview = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Eliminar_unidad_conservacion_contenedora_unidad_treview = errorM

        End Try
    End Function
    Function Retorna_id_configuracion_rotulo_unidad_conservacion_por_id_unidad_conservacion(ByVal id_unidad_conservacion As Integer, _
    ByRef id_configuracion_rotulo As Integer, ByVal id_usuario_gestion As Integer) As String
        '******************************************************************
        'Función : Retorna el id configuracion del rotulo, con el id de la
        'unidad de conservación con el id de la unidad de conservación y 
        'el id de usaurio de gestión
        'Fecha 2015-01-25
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************************
        Try
            Dim sqlconsulta As String = "Select rtuc.ID_ROTULO_UNIDAD_CONSERVACION  from unidad_conservacion as uc " & _
            " inner join ra_configuracion_rotulo_unidad_conservacion as rtuc on " & _
            " (uc.ID_TIPO_UNIDAD_CONSERVACION=rtuc.UNIDAD_CONSERVACION_ID_TIPO_UNIDAD " & ") " & _
            " where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                Retorna_id_configuracion_rotulo_unidad_conservacion_por_id_unidad_conservacion = " Error listando tipo_unidad_conservacion" & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_configuracion_rotulo_unidad_conservacion_por_id_unidad_conservacion = "Imposible encontrar configuración rotulo"
                Exit Function
            Else
                id_configuracion_rotulo = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_configuracion_rotulo_unidad_conservacion_por_id_unidad_conservacion = "YES"
            End If
        Catch ex As Exception
            Retorna_id_configuracion_rotulo_unidad_conservacion_por_id_unidad_conservacion = "Incosistencia  Función " & _
            " Retorna_id_configuracion_rotulo_unidad_conservacion_por_id_unidad_conservacion " & ex.Message
        End Try

    End Function
    Function Asigna_Datos_DB_Configuracion_rotulo_unidad_conservacion_estructura(ByRef estru As rotulo_unidad_conservacion, _
                                                                                 ByVal id_configiuracion_rotulo As Integer) As String
        '***********************************************************************
        'Función : Asigna datos configuración de rotulo unidad de conservación
        ' a la estructura
        'Fecha : 2015-01-24 Modificado para web
        'Img :Miguel Angel Urueta Miranda
        '***********************************************************************
        Try

            Dim sqlconsulta As String = "select EJE_X,EJE_Y,chekmarco,numero_columnas_datos,image_empresa" & _
            ",nit_empresa,nombre_empresa,DATOS_UNIDAD_CONSERVACION,Codigo_unico,Tema_unidad,Fechas_Extremas,Rangos_Extremos" & _
            ",Descripcion_unidad,TRD_UNIDAD_CONSERVACION,Nombre_Area,Codigo_Area,Nombre_Serie,Codigo_Serie,Nombre_sub_Serie," & _
            "Codigo_sub_Serie,Edificio,Piso,Area,Estante,Modulo,Estrepano,UBICACION_UNIDAD_CONSERVACION," & _
            "TAM_LETRA_TITULO,TAM_LETRA_DATOS_UNIDAD,TAM_LETRA_DATOS_TRD,TAM_LETRA_UBICACION,TAM_LETRA_UNI_ANIDADO," & _
            "UNIDADES_ANIDADAS,TAM_LETRA_UNIDADDES_CONTENIDA,campo_orden_expediente " & _
            " from ra_configuracion_rotulo_unidad_conservacion where ID_ROTULO_UNIDAD_CONSERVACION=" & id_configiuracion_rotulo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                Asigna_Datos_DB_Configuracion_rotulo_unidad_conservacion_estructura = " Error Asigna_Datos_DB_Configuracion_rotulo_unidad_conservacion_estructura " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Asigna_Datos_DB_Configuracion_rotulo_unidad_conservacion_estructura = "Imposible encontrar configuración rotulo unidad "
                Exit Function
            Else

                estru.x = Datset.Tables(0).Rows(0).Item(0)
                estru.y = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).Item(2) = 0 Then
                    estru.chekmarco = False
                Else
                    estru.chekmarco = True
                End If
                estru.numero_columnas_datos = Datset.Tables(0).Rows(0).Item(3)
                If Datset.Tables(0).Rows(0).Item(4) = 0 Then
                    estru.image_empresa = False
                Else
                    estru.image_empresa = True
                End If
                If Datset.Tables(0).Rows(0).Item(5) = 0 Then
                    estru.nit_empresa = False
                Else
                    estru.nit_empresa = True
                End If
                If Datset.Tables(0).Rows(0).Item(6) = 0 Then
                    estru.nombre_empresa = False
                Else
                    estru.nombre_empresa = True
                End If
                If Datset.Tables(0).Rows(0).Item(7) = 0 Then
                    estru.DATOS_UNIDAD_CONSERVACION = False
                Else
                    estru.DATOS_UNIDAD_CONSERVACION = True
                End If
                If Datset.Tables(0).Rows(0).Item(8) = 0 Then
                    estru.Codigo_unico = False
                Else
                    estru.Codigo_unico = True
                End If
                If Datset.Tables(0).Rows(0).Item(9) = 0 Then
                    estru.Tema_unidad = False
                Else
                    estru.Tema_unidad = True
                End If
                If Datset.Tables(0).Rows(0).Item(10) = 0 Then
                    estru.Fechas_Extremas = False
                Else
                    estru.Fechas_Extremas = True
                End If
                If Datset.Tables(0).Rows(0).Item(11) = 0 Then
                    estru.Rangos_Extremos = False
                Else
                    estru.Rangos_Extremos = True
                End If
                If Datset.Tables(0).Rows(0).Item(12) = 0 Then
                    estru.Descripcion_unidad = False
                Else
                    estru.Descripcion_unidad = True
                End If
                If Datset.Tables(0).Rows(0).Item(13) = 0 Then
                    estru.TRD_UNIDAD_CONSERVACION = False
                Else
                    estru.TRD_UNIDAD_CONSERVACION = True
                End If
                If Datset.Tables(0).Rows(0).Item(14) = 0 Then
                    estru.Nombre_Area = False
                Else
                    estru.Nombre_Area = True
                End If
                If Datset.Tables(0).Rows(0).Item(15) = 0 Then
                    estru.Codigo_Area = False
                Else
                    estru.Codigo_Area = True
                End If
                If Datset.Tables(0).Rows(0).Item(16) = 0 Then
                    estru.Nombre_Serie = False
                Else
                    estru.Nombre_Serie = True
                End If
                If Datset.Tables(0).Rows(0).Item(17) = 0 Then
                    estru.Codigo_Serie = False
                Else
                    estru.Codigo_Serie = True
                End If
                If Datset.Tables(0).Rows(0).Item(18) = 0 Then
                    estru.Nombre_sub_Serie = False
                Else
                    estru.Nombre_sub_Serie = True
                End If
                If Datset.Tables(0).Rows(0).Item(19) = 0 Then
                    estru.Codigo_sub_Serie = False
                Else
                    estru.Codigo_sub_Serie = True
                End If
                If Datset.Tables(0).Rows(0).Item(20) = 0 Then
                    estru.Edificio = False
                Else
                    estru.Edificio = True
                End If
                If Datset.Tables(0).Rows(0).Item(21) = 0 Then
                    estru.Piso = False
                Else
                    estru.Piso = True
                End If
                If Datset.Tables(0).Rows(0).Item(22) = 0 Then
                    estru.Area = False
                Else
                    estru.Area = True
                End If
                If Datset.Tables(0).Rows(0).Item(23) = 0 Then
                    estru.Estante = False
                Else
                    estru.Estante = True
                End If
                If Datset.Tables(0).Rows(0).Item(24) = 0 Then
                    estru.Modulo = False
                Else
                    estru.Modulo = True
                End If
                If Datset.Tables(0).Rows(0).Item(25) = 0 Then
                    estru.Estrepaño = False
                Else
                    estru.Estrepaño = True
                End If
                If Datset.Tables(0).Rows(0).Item(26) = 0 Then
                    estru.UBICACION_UNIDAD_CONSERVACION = False
                Else
                    estru.UBICACION_UNIDAD_CONSERVACION = True
                End If
                estru.TAM_LETRA_TITULO = Datset.Tables(0).Rows(0).Item(27)
                estru.TAM_LETRA_DATOS_UNIDAD = Datset.Tables(0).Rows(0).Item(28)
                estru.TAM_LETRA_DATOS_TRD = Datset.Tables(0).Rows(0).Item(29)
                estru.TAM_LETRA_UBICACION = Datset.Tables(0).Rows(0).Item(30)
                estru.TAM_LETRA_UNI_ANIDADO = Datset.Tables(0).Rows(0).Item(31)
                If Datset.Tables(0).Rows(0).Item(32) = 0 Then
                    estru.UNIDADES_ANIDADAS = False
                Else
                    estru.UNIDADES_ANIDADAS = True
                End If
                estru.TAM_LETRA_UNIDADDES_CONTENIDA = Datset.Tables(0).Rows(0).Item(33)
                If Datset.Tables(0).Rows(0).IsNull(34) = True Then
                    estru.campo_orden_expediente = "CODIGO_UNICO"
                Else
                    estru.campo_orden_expediente = Datset.Tables(0).Rows(0).Item(34)
                End If
                Asigna_Datos_DB_Configuracion_rotulo_unidad_conservacion_estructura = "YES"
            End If
        Catch ex As Exception
            Asigna_Datos_DB_Configuracion_rotulo_unidad_conservacion_estructura = "Inconsistencia función " & vbCrLf & _
            " Asigna_Datos_DB_Configuracion_rotulo_unidad_conservacion_estructura  " & vbCrLf & ex.Message
        End Try

    End Function
    Function Genera_rotulo_unidad_conservacion_pdf(ByVal id_unidad_conservacion As Integer, _
                                                   ByVal id_empresa As Integer, _
                                                   ByVal id_usuario_gestion As Integer, _
                                                   ByRef archivo_pdf As String) As String
        '***************************************************************
        'Función : Genera rotulo unidad de conservación
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2015-01-25
        '***************************************************************
        Dim doc As New Document
        Dim writer As PdfWriter = Nothing
        Try
            Dim Result As String = ""
            '-------------------------------------------------
            'Retorna id configuracion unidad de conservacion
            '-------------------------------------------------
            Dim id_configuracion_rotulo As Integer = 0
            Result = Me.Retorna_id_configuracion_rotulo_unidad_conservacion_por_id_unidad_conservacion(id_unidad_conservacion, _
                                                                                                       id_configuracion_rotulo, _
                                                                                                       id_usuario_gestion)
            If Result <> "YES" Then
                Genera_rotulo_unidad_conservacion_pdf = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna datos unidad de conservacion
            '---------------------------------------------------
            Dim estru_unidad_conservacion() As unidad_conservacion
            Erase estru_unidad_conservacion
            Result = Me.Listar_datos_Unidad_Conservacion_estructura(id_unidad_conservacion, _
                                                                    estru_unidad_conservacion)
            If Result <> "YES" Then
                Genera_rotulo_unidad_conservacion_pdf = Result
                Exit Function
            End If
            '----------------------------------------------------
            'Retorna datos instrumento archivístico
            '----------------------------------------------------
            Dim nombre_instrumento As String = ""
            Dim version_instrumento As String = ""
            Dim id_tipo_instrumento As Integer = 0
            Dim Refclas_GestionInstrumento As New ClassGaGestionInstrumento
            If estru_unidad_conservacion(0).id_instrumento <> 0 Then
                Result = Refclas_GestionInstrumento.Solicita_datos_instrumento_rotulo(estru_unidad_conservacion(0).id_instrumento, _
                                                                                     id_tipo_instrumento, _
                                                                                     nombre_instrumento, _
                                                                                      version_instrumento)
                If Result <> "YES" Then
                    Genera_rotulo_unidad_conservacion_pdf = Result
                    Exit Function
                End If
            End If
            '-----------------------------------------------------
            'Retorna estructura rotulo unidad de conservación
            '-----------------------------------------------------
            Dim estru As rotulo_unidad_conservacion = Nothing
            Result = Me.Asigna_Datos_DB_Configuracion_rotulo_unidad_conservacion_estructura(estru, _
                                                                                            id_configuracion_rotulo)
            If Result <> "YES" Then
                Genera_rotulo_unidad_conservacion_pdf = Result
                Exit Function
            End If
            Dim nombre_empresa As String = ""
            Dim nit_empresa As String = ""
            Result = Retorna_Datos_Empresa(id_empresa, nombre_empresa, nit_empresa)
            If Result <> "YES" Then
                Genera_rotulo_unidad_conservacion_pdf = Result
                Exit Function
            End If
            '------------------------------------------------------
            'Retorna unidades anidadas en unidad contenedora
            '------------------------------------------------------
            Dim refclasexpediente As New ClassGaExpediente
            Dim estru_expediente() As expediente_conservacion = Nothing
            If estru.UNIDADES_ANIDADAS = True Then
                If estru_unidad_conservacion(0).TIPO_UNIDAD_CONSERVACION = 1 Then
                    '------------------------------------------------------------------------
                    'Retorna datos expedientes anidados en unidad contendora de expedientes
                    '------------------------------------------------------------------------
                    Result = refclasexpediente.Listar_datos_Expediente_estructura_unidad_conservacion(id_unidad_conservacion, _
                                                                                                      estru_expediente, _
                                                                                                      estru.campo_orden_expediente)
                    If Result <> "YES" Then
                        Genera_rotulo_unidad_conservacion_pdf = Result
                        Exit Function
                    End If
                End If
            End If
            Dim struentrepaño() As ClassGestionArchivo.Entrapño_archivo
            Erase struentrepaño
            Dim id_entrapaño_idex As Integer = 0
            If estru.UBICACION_UNIDAD_CONSERVACION = True Then
                Dim id_estante As Integer = 0
                Dim refclas As New ClassGestionArchivo
                If estru_unidad_conservacion(0).ENTRE_PAÑO_ID_ENTREPAÑO <> 0 Then
                    Result = refclas.Retorna_Id_Estante_por_entrepaño(estru_unidad_conservacion(0).ENTRE_PAÑO_ID_ENTREPAÑO, _
                                                                      id_estante)
                    If Result <> "YES" Then
                        Genera_rotulo_unidad_conservacion_pdf = "Imposible listar entrepaños " & Result
                        Exit Function
                    End If
                Else
                    '----------------------------------------------------
                    'solicita el id del entrapaño por la unidad de 
                    'conservacion
                    '----------------------------------------------------
                    Dim id_entrepaño As Integer = 0
                    Result = refclas.Retorna_id_Entrepaño_id_unidad_conservacion( _
                                                                                 estru_unidad_conservacion(0).ID_UNIDAD_CONSERVACION_TOPLOGICA, _
                                                                                 id_entrepaño)
                    If Result <> "YES" Then
                        Genera_rotulo_unidad_conservacion_pdf = Result
                        Exit Function
                    End If
                    Result = refclas.Retorna_Id_Estante_por_entrepaño(id_entrepaño, _
                                                                      id_estante)
                    If Result <> "YES" Then
                        Genera_rotulo_unidad_conservacion_pdf = "Imposible listar entrepaños " & Result
                        Exit Function
                    End If
                End If
                Result = refclas.Listar_Entrepaño_Archivo(id_empresa, struentrepaño, id_estante)
                If Result <> "YES" Then
                    Genera_rotulo_unidad_conservacion_pdf = "Imposible listar entrepaños " & Result
                    Exit Function
                End If

                For i As Integer = 0 To struentrepaño.Length - 1
                    If estru_unidad_conservacion(0).ENTRE_PAÑO_ID_ENTREPAÑO = struentrepaño(i).id_entreapaño Then
                        id_entrapaño_idex = i
                        Exit For
                    End If
                Next
            End If
            Dim Rutatemp As String = HttpContext.Current.Session.Item("GA_RUTA_TEMPO_IMPRESION") & "\"
            Rutatemp = HttpContext.Current.Session.Item("GA_RUTA_TEMPO_IMPRESION") & "\"
            If Directory.Exists(Rutatemp) = False Then
                Directory.CreateDirectory(Rutatemp)
            End If
            archivo_pdf = Rutatemp & "temp_" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ".pdf"
            If File.Exists(archivo_pdf) = True Then
                Kill(archivo_pdf)
            End If
            doc = New Document(New Rectangle(estru.x, estru.y))
            writer = PdfWriter.GetInstance(doc, _
                                New FileStream(archivo_pdf, FileMode.Create))
            writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE)
            doc.Open()
            Dim ruta_image As String = HttpContext.Current.Server.MapPath("../imagera/" & "logo_trd.png")
            If estru.image_empresa = True Then
                If File.Exists(ruta_image) = False And estru.image_empresa = True Then
                    Genera_rotulo_unidad_conservacion_pdf = "El sistema no tiene registrado el icono para rotulo en la ruta  " & _
                    ruta_image
                    MsgBox(Genera_rotulo_unidad_conservacion_pdf, MsgBoxStyle.Information)
                    'Exit Function
                Else
                    Dim imagen As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(ruta_image)
                    imagen.BorderWidth = 0
                    imagen.Alignment = Element.ALIGN_CENTER
                    Dim percentage As Object = 0.0F
                    percentage = 100 / imagen.Width
                    imagen.ScalePercent(percentage * 50)
                    'Insertamos la imagen en el documento
                    doc.Add(imagen)
                End If
            End If
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
            estru.TAM_LETRA_TITULO, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim _standardFont_datos_unidad_conservacion As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
            estru.TAM_LETRA_DATOS_UNIDAD, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim _standardFont_datos_unidad_conservacion_anex As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
            estru.TAM_LETRA_UNIDADDES_CONTENIDA, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim paragraf As New Paragraph
            paragraf = New Paragraph(nombre_empresa, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            If estru.nombre_empresa = True Then
                doc.Add(paragraf)
            End If
            paragraf = New Paragraph(nit_empresa, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            If estru.nit_empresa = True Then
                doc.Add(paragraf)
            End If
            doc.Add(Chunk.NEWLINE)
            paragraf = New Paragraph("DATOS UNIDAD DE CONSERVACION", _standardFont_datos_unidad_conservacion)
            paragraf.Alignment = Element.ALIGN_CENTER
            If estru.DATOS_UNIDAD_CONSERVACION = True Then
                Dim datos_instrumento As String = ""
                If id_tipo_instrumento <> 0 Then
                    If id_tipo_instrumento = 1 Then
                        datos_instrumento = " Instrumento TRD - " & nombre_instrumento & " Vesrión " & version_instrumento
                    End If
                    If id_tipo_instrumento = 2 Then
                        datos_instrumento = " Instrumento TVD - " & nombre_instrumento & " Vesrión " & version_instrumento
                    End If
                End If
                doc.Add(paragraf)
                paragraf = New Paragraph("Código único " & estru_unidad_conservacion(0).ID_UNIDAD_CONSERVACION & " Fecha impresión " & Now.ToString & datos_instrumento, _standardFont_datos_unidad_conservacion)
                paragraf.Alignment = Element.ALIGN_LEFT
                doc.Add(paragraf)
                paragraf = New Paragraph(".", _standardFont_datos_unidad_conservacion)
                paragraf.Alignment = Element.ALIGN_LEFT
                doc.Add(paragraf)
            End If
            doc.Add(New Paragraph(3, vbCrLf))
            Dim tblrdatos As PdfPTable = New PdfPTable(estru.numero_columnas_datos)
            tblrdatos.WidthPercentage = 100
            Dim Descripcion_Tipo As String = ""
            '-------------------------------------------------------------
            'Asigna id entrepaño-id tipo unidad-id unidad -descripcion
            '-------------------------------------------------------------
            If estru_unidad_conservacion(0).TIPO_UNIDAD_CONSERVACION = 1 Then
                Descripcion_Tipo = "Contenedora de unidad documental"
            End If
            If estru_unidad_conservacion(0).TIPO_UNIDAD_CONSERVACION = 2 Then
                Descripcion_Tipo = "Contenedora de Documentos"
            End If
            If estru_unidad_conservacion(0).TIPO_UNIDAD_CONSERVACION = 3 Then
                Descripcion_Tipo = "Contenedora de unidad de conservación"
            End If
            Result = Retorna_nombre_tipo_unidad_conservacion(estru_unidad_conservacion(0).ID_TIPO_UNIDAD_CONSERVACION, Descripcion_Tipo)
            If Result <> "YES" Then
                Genera_rotulo_unidad_conservacion_pdf = Result
                Exit Function
            End If
            Dim cltipounidad As PdfPCell = New PdfPCell(New Phrase("Tipo unidad :", _standardFont_datos_unidad_conservacion))
            cltipounidad.BorderWidth = 1
            Dim cltipounidad_valor As PdfPCell = New PdfPCell(New Phrase(Descripcion_Tipo, _standardFont_datos_unidad_conservacion))
            cltipounidad_valor.BorderWidth = 1
            tblrdatos.AddCell(cltipounidad)
            tblrdatos.AddCell(cltipounidad_valor)
            Dim clCodigounico As PdfPCell = New PdfPCell(New Phrase("Código descriptivo :", _standardFont_datos_unidad_conservacion))
            clCodigounico.BorderWidth = 1
            Dim clCodigounico_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).CODIGO_UNICO, _standardFont_datos_unidad_conservacion))
            clCodigounico_valor.BorderWidth = 1
            If estru.Codigo_unico = True Then
                tblrdatos.AddCell(clCodigounico)
                tblrdatos.AddCell(clCodigounico_valor)
            End If
            Dim clTema As PdfPCell = New PdfPCell(New Phrase("Tema unidad :", _standardFont_datos_unidad_conservacion))
            clTema.BorderWidth = 1
            Dim clTema_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).TEMA_UNIDAD_CONSERVACION, _standardFont_datos_unidad_conservacion))
            clTema_valor.BorderWidth = 1
            If estru.Tema_unidad = True Then
                tblrdatos.AddCell(clTema)
                tblrdatos.AddCell(clTema_valor)
            End If
            Dim clrangosfechas As PdfPCell = New PdfPCell(New Phrase("Fechas Extremas :", _standardFont_datos_unidad_conservacion))
            clrangosfechas.BorderWidth = 1
            Dim clrangosfechas_valor As PdfPCell = New PdfPCell(New Phrase _
            (estru_unidad_conservacion(0).FECHA_EXTREMA_INICIAL & " Hasta " & estru_unidad_conservacion(0).FECHA_EXTREMA_FINAL, _standardFont_datos_unidad_conservacion))
            clrangosfechas_valor.BorderWidth = 1
            If estru.Fechas_Extremas = True Then
                tblrdatos.AddCell(clrangosfechas)
                tblrdatos.AddCell(clrangosfechas_valor)
            End If
            Dim clrangosextremos As PdfPCell = New PdfPCell(New Phrase("Rangos Extremos :", _standardFont_datos_unidad_conservacion))
            clrangosextremos.BorderWidth = 1
            Dim clrangosextremos_valor As PdfPCell = New PdfPCell(New Phrase _
            (estru_unidad_conservacion(0).RANGO_EXTREMO_INICIAL & " Hasta " & estru_unidad_conservacion(0).RANGO_EXTREMO_FINAL, _standardFont_datos_unidad_conservacion))
            clrangosextremos_valor.BorderWidth = 1
            If estru.Rangos_Extremos = True Then
                tblrdatos.AddCell(clrangosextremos)
                tblrdatos.AddCell(clrangosextremos_valor)
            End If

            Dim cldescripcion As PdfPCell = New PdfPCell(New Phrase("Descripción unidad :", _standardFont_datos_unidad_conservacion))
            cldescripcion.BorderWidth = 1
            Dim cldescripcion_valor As PdfPCell = New PdfPCell(New Phrase _
            (estru_unidad_conservacion(0).DESCRIPCION_UNIDAD_CONSERVACION, _standardFont_datos_unidad_conservacion))
            cldescripcion_valor.BorderWidth = 1
            If estru.Descripcion_unidad = True Then
                tblrdatos.AddCell(cldescripcion)
                tblrdatos.AddCell(cldescripcion_valor)
            End If
            doc.Add(tblrdatos)
            Dim _standardFont_trd_unidad_conservacion As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
           estru.TAM_LETRA_DATOS_TRD, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            If estru.TRD_UNIDAD_CONSERVACION = True Then
                paragraf = New Paragraph("DATOS DE INSTRUMENTOS Y CLASIFICACION", _standardFont_trd_unidad_conservacion)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
                paragraf = New Paragraph(".", _standardFont_datos_unidad_conservacion)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
            End If
            '****************************************************
            'Tabla trd
            '****************************************************

            Dim tblPrueba As PdfPTable = New PdfPTable(estru.numero_columnas_datos)
            tblPrueba.WidthPercentage = 100
            Dim clNombre As PdfPCell = New PdfPCell(New Phrase("Nombre Area (Sección):", _standardFont_trd_unidad_conservacion))
            clNombre.BorderWidth = 1
            Dim clNombre_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).NOMBRE_AREA, _standardFont_trd_unidad_conservacion))
            clNombre_valor.BorderWidth = 1
            If estru.Nombre_Area = True Then
                tblPrueba.AddCell(clNombre)
                tblPrueba.AddCell(clNombre_valor)
            End If
            Dim clCodigoArea As PdfPCell = New PdfPCell(New Phrase("Código Area :", _standardFont_trd_unidad_conservacion))
            clCodigoArea.BorderWidth = 1
            Dim clCodigoArea_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).CODIGO_AREA_TRD, _standardFont_trd_unidad_conservacion))
            clCodigoArea_valor.BorderWidth = 1
            If estru.Codigo_Area = True Then
                tblPrueba.AddCell(clCodigoArea)
                tblPrueba.AddCell(clCodigoArea_valor)
            End If

            Dim clNombre_sub As PdfPCell = New PdfPCell(New Phrase("Nombre sub Area (Sub Sección):", _standardFont_trd_unidad_conservacion))
            clNombre_sub.BorderWidth = 1
            Dim clNombre_valor_sub As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).NOMBRE_SUB_AREA, _standardFont_trd_unidad_conservacion))
            clNombre_valor_sub.BorderWidth = 1
            If estru.Nombre_Area = True Then
                tblPrueba.AddCell(clNombre_sub)
                tblPrueba.AddCell(clNombre_valor_sub)
            End If
            Dim clSerie As PdfPCell = New PdfPCell(New Phrase("Nombre Serie :", _standardFont_trd_unidad_conservacion))
            clSerie.BorderWidth = 1
            Dim clSerie_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).NOMBRE_SERIE, _standardFont_trd_unidad_conservacion))
            clSerie_valor.BorderWidth = 1
            If estru.Nombre_Serie = True Then
                tblPrueba.AddCell(clSerie)
                tblPrueba.AddCell(clSerie_valor)
            End If
            Dim clCodigoSerie As PdfPCell = New PdfPCell(New Phrase("Código Serie :", _standardFont_trd_unidad_conservacion))
            clCodigoSerie.BorderWidth = 1
            Dim clCodigoSerie_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).CODIGO_SERIE, _standardFont_trd_unidad_conservacion))
            clCodigoSerie_valor.BorderWidth = 1
            If estru.Codigo_Serie = True Then
                tblPrueba.AddCell(clCodigoSerie)
                tblPrueba.AddCell(clCodigoSerie_valor)
            End If
            Dim clnombresubSerie As PdfPCell = New PdfPCell(New Phrase("Nombre sub Serie :", _standardFont_trd_unidad_conservacion))
            clnombresubSerie.BorderWidth = 1
            Dim clnombresubSerie_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).NOMBRE_SUBSERIE, _standardFont_trd_unidad_conservacion))
            clnombresubSerie_valor.BorderWidth = 1
            If estru.Nombre_sub_Serie = True Then
                tblPrueba.AddCell(clnombresubSerie)
                tblPrueba.AddCell(clnombresubSerie_valor)
            End If
            Dim clcodigosubSerie As PdfPCell = New PdfPCell(New Phrase("Código sub Serie :", _standardFont_trd_unidad_conservacion))
            clcodigosubSerie.BorderWidth = 1
            Dim clcodigosubSerie_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).CODIGO_SUBSERIE, _standardFont_trd_unidad_conservacion))
            clcodigosubSerie_valor.BorderWidth = 1
            If estru.Codigo_sub_Serie = True Then
                tblPrueba.AddCell(clcodigosubSerie)
                tblPrueba.AddCell(clcodigosubSerie_valor)
            End If
            doc.Add(tblPrueba)
            '***********************************************
            'Tabla ubicacion
            '***********************************************
            Dim _standardFont_ubicacion_unidad As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
            estru.TAM_LETRA_UBICACION, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            If estru.UBICACION_UNIDAD_CONSERVACION = True Then
                Dim tblrubicacion As PdfPTable = New PdfPTable(estru.numero_columnas_datos)
                tblrubicacion.WidthPercentage = 100
                Dim cledificio As PdfPCell = New PdfPCell(New Phrase("Edificio Archivo :", _standardFont_ubicacion_unidad))
                cledificio.BorderWidth = 1
                Dim cledificio_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).edificio_contenedor, _standardFont_ubicacion_unidad))
                cledificio_valor.BorderWidth = 1
                If estru.Edificio = True Then
                    tblrubicacion.AddCell(cledificio)
                    tblrubicacion.AddCell(cledificio_valor)
                End If
                Dim clpiso As PdfPCell = New PdfPCell(New Phrase("Piso Archivo :", _standardFont_ubicacion_unidad))
                clpiso.BorderWidth = 1
                Dim clpiso_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).piso_contenedor, _standardFont_ubicacion_unidad))
                clpiso_valor.BorderWidth = 1
                If estru.Piso = True Then
                    tblrubicacion.AddCell(clpiso)
                    tblrubicacion.AddCell(clpiso_valor)
                End If
                Dim clarea As PdfPCell = New PdfPCell(New Phrase("Area Archivo :", _standardFont_ubicacion_unidad))
                clarea.BorderWidth = 1
                Dim clarea_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).area_contenedor, _standardFont_ubicacion_unidad))
                clarea_valor.BorderWidth = 1
                If estru.Area = True Then
                    tblrubicacion.AddCell(clarea)
                    tblrubicacion.AddCell(clarea_valor)
                End If
                Dim clmodulo As PdfPCell = New PdfPCell(New Phrase("Módulo Archivo :", _standardFont_ubicacion_unidad))
                clmodulo.BorderWidth = 1
                Dim clmodulo_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).modulo_contendor, _standardFont_ubicacion_unidad))
                clmodulo_valor.BorderWidth = 1
                If estru.Modulo = True Then
                    tblrubicacion.AddCell(clmodulo)
                    tblrubicacion.AddCell(clmodulo_valor)
                End If
                Dim clestante As PdfPCell = New PdfPCell(New Phrase("Estante Archivo :", _standardFont_ubicacion_unidad))
                clestante.BorderWidth = 1
                Dim clestante_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).estante_contenedor, _standardFont_ubicacion_unidad))
                clestante_valor.BorderWidth = 1
                If estru.Estante = True Then
                    tblrubicacion.AddCell(clestante)
                    tblrubicacion.AddCell(clestante_valor)
                End If
                Dim clentrepaño As PdfPCell = New PdfPCell(New Phrase("Entrepaño Archivo :", _standardFont_ubicacion_unidad))
                clentrepaño.BorderWidth = 1
                Dim clentrepaño_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).codigo_corto, _standardFont_ubicacion_unidad))
                clentrepaño_valor.BorderWidth = 1
                If estru.Estrepaño = True Then
                    tblrubicacion.AddCell(clentrepaño)
                    tblrubicacion.AddCell(clentrepaño_valor)
                End If

                paragraf = New Paragraph("UBICACION TOPONIMICA", _standardFont_ubicacion_unidad)
                paragraf.Alignment = Element.ALIGN_CENTER
                If estru.UBICACION_UNIDAD_CONSERVACION = True Then
                    doc.Add(paragraf)
                    paragraf = New Paragraph(".", _standardFont_datos_unidad_conservacion)
                    paragraf.Alignment = Element.ALIGN_CENTER
                    doc.Add(paragraf)
                End If
                doc.Add(tblrubicacion)

            End If
            '****************************************************
            'Unidades anidadas  estru_expediente
            '****************************************************
            Dim _standardFont_anidad_unidad As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, _
            estru.TAM_LETRA_UNI_ANIDADO, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            If estru.UNIDADES_ANIDADAS = True Then
                If estru_unidad_conservacion(0).TIPO_UNIDAD_CONSERVACION = 1 Then
                    paragraf = New Paragraph("UNIDADES CONTENIDAS", _standardFont_datos_unidad_conservacion)
                    paragraf.Alignment = Element.ALIGN_CENTER
                    doc.Add(paragraf)
                    paragraf = New Paragraph(".", _standardFont_datos_unidad_conservacion_anex)
                    paragraf.Alignment = Element.ALIGN_CENTER
                    'doc.Add(paragraf)
                    Dim tblranidadas As PdfPTable = New PdfPTable(1)
                    tblranidadas.WidthPercentage = 100
                    Dim unidades_contenidas As String = ""
                    If Not estru_expediente Is Nothing Then
                        For i As Integer = 0 To estru_expediente.Length - 1
                            Dim asunto As String = ""
                            If estru_expediente(i).ASUNTO_EXPEDIENTE <> "" Then
                                If estru_expediente(i).ASUNTO_EXPEDIENTE.ToString.Length > 20 Then
                                    asunto = Left(estru_expediente(i).ASUNTO_EXPEDIENTE, 20)
                                Else
                                    asunto = estru_expediente(i).ASUNTO_EXPEDIENTE
                                End If
                            End If

                            unidades_contenidas = unidades_contenidas & "CONSECU : " & estru_expediente(i).CODIGO_UNICO & " ID EXP : " & estru_expediente(i).ID_EXPEDIENTE & " VOLUMEN : " & estru_expediente(i).VOLUMEN_EXPEIDENTE & " RANGOS : " & _
                            estru_expediente(i).RANGO_EXTREMO_INICIAL & " - " & estru_expediente(i).RANGO_EXTREMO_FINAL & " FECHAS EXTREMAS : " & _
                            estru_expediente(i).FECHA_EXTREMA_INICIAL & " - " & estru_expediente(i).FECHA_EXTREMA_FINAL & " ASUNTO : " & asunto & _
                            " TEMA : " & estru_expediente(i).TEMA_EXPEDIENTE & vbCrLf
                        Next
                    End If
                    Dim cledunidad As PdfPCell = New PdfPCell(New Phrase(unidades_contenidas, _standardFont_datos_unidad_conservacion_anex))
                    cledunidad.BorderWidth = 0
                    Dim celunidades_anidadas As PdfPCell = New PdfPCell(New Phrase(unidades_contenidas, _standardFont_anidad_unidad))
                    celunidades_anidadas.BorderWidth = 1
                    tblranidadas.AddCell(cledunidad)
                    'tblranidadas.AddCell(celunidades_anidadas)
                    doc.Add(tblranidadas)
                End If
            End If

            If estru.chekmarco = True Then
                Dim pageRect As Rectangle = doc.PageSize
                Dim content = writer.DirectContent
                Dim pageBorderRect = New Rectangle(doc.PageSize)
                pageBorderRect.Left += doc.LeftMargin
                pageBorderRect.Right -= doc.RightMargin
                pageBorderRect.Top -= doc.TopMargin
                pageBorderRect.Bottom += doc.BottomMargin
                content.SetColorStroke(BaseColor.BLACK)
                content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height)
                content.Stroke()
            End If


            Genera_rotulo_unidad_conservacion_pdf = "YES"
        Catch ex As Exception
            Genera_rotulo_unidad_conservacion_pdf = "Inconsistencia función : Genera_rotulo_unidad_conservacion_pdf " & ex.Message
        Finally
            doc.Close()
            If Not writer Is Nothing Then
                writer.Close()
            End If

        End Try
    End Function
    Function Elimina_unidad_conservacion_tipo_contenedor_expediente(ByVal id_unidad_conservacion As Integer, _
                                                                    ByRef treeview As TreeNode, _
                                                                    ByVal id_usuario_gestion As Integer, _
                                                                    ByRef trevi As TreeView, _
                                                                    ByVal user_Gestion As String, _
                                                                    ByVal iptrans As String, _
                                                                    ByRef update As UpdatePanel) As String
        'Try
        Dim Result As String = ""
        Result = Verfica_Existencia_Expedientes_en_unidad_conservacion(id_unidad_conservacion)
        If Result <> "YES" Then
            Elimina_unidad_conservacion_tipo_contenedor_expediente = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Elimina_unidad_conservacion_tipo_contenedor_expediente = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "delete from  unidad_conservacion where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_unidad_conservacion_tipo_contenedor_expediente = "Imposible eliminar unidad de conservación  : " & sqlinsertcion
                errorM = "Imposible eliminar unidad de conservación  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_unidad_conservacion (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" & _
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) values (" & _
           "'ELIMINA UNIDAD','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," & _
           id_unidad_conservacion & ",'" & iptrans & "','" & hor & "','DOCUARCHI')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualizar los unidad de conservación  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            trevi.Nodes.Remove(trevi.SelectedNode)
            Dim sNodo As TreeNode = trevi.SelectedNode
            Dim pNodo As TreeNode = sNodo.Parent
            pNodo.ChildNodes.Remove(sNodo)
            update.Update()
            myTrans.Commit()
            myConnection.Close()
            Elimina_unidad_conservacion_tipo_contenedor_expediente = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Elimina_unidad_conservacion_tipo_contenedor_expediente = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Elimina_unidad_conservacion_tipo_contenedor_expediente = errorM

        End Try
    End Function
    Function Elimina_unidad_conservacion_tipo_contenedor_expediente_lista(ByVal id_unidad_conservacion As Integer, _
    ByVal id_usuario_gestion As Integer, _
    ByVal user_Gestion As String, ByVal iptrans As String) As String
        'Try
        Dim Result As String = ""
        Result = Verfica_Existencia_Expedientes_en_unidad_conservacion(id_unidad_conservacion)
        If Result <> "YES" Then
            Elimina_unidad_conservacion_tipo_contenedor_expediente_lista = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Elimina_unidad_conservacion_tipo_contenedor_expediente_lista = Result
            Exit Function
        End If

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try


            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "delete from  unidad_conservacion where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Elimina_unidad_conservacion_tipo_contenedor_expediente_lista = "Imposible eliminar unidad de conservación  : " & sqlinsertcion
                errorM = "Imposible eliminar unidad de conservación  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_unidad_conservacion (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" & _
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) values (" & _
           "'ELIMINA UNIDAD','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," & _
           id_unidad_conservacion & ",'" & iptrans & "','" & hor & "','DOCUARCHI')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualizar los unidad de conservación  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            myConnection.Close()
            Elimina_unidad_conservacion_tipo_contenedor_expediente_lista = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Elimina_unidad_conservacion_tipo_contenedor_expediente_lista = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Elimina_unidad_conservacion_tipo_contenedor_expediente_lista = errorM

        End Try
    End Function
    Function Eliminar_unidad_conservacion(ByVal id_unidad_conservacion As Integer, _
    ByVal id_usuario_gestion As Integer, _
    ByVal user_Gestion As String, ByVal iptrans As String) As String
        '*********************************************************
        'Función : Elimina unidad de conservacion, verificando
        'el registro en la unidad de producción
        'Fecha : 2015-01-19
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Dim Result As String = ""
        Dim existencia_regi_unidad As String = "YES"
        Result = Verifica_existencia_unidad_conservacion_control_produccion(id_unidad_conservacion, existencia_regi_unidad)
        If Result <> "YES" Then
            Eliminar_unidad_conservacion = Result
            Exit Function
        End If
        If existencia_regi_unidad = "YES" Then
            Eliminar_unidad_conservacion = "La unidad de conservación no se puede eliminar por que tiene documentos relacionados"
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Eliminar_unidad_conservacion = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try

            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "Delete from unidad_conservacion where ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_unidad_conservacion = "Imposible eliminar unidad de conservación  : " & sqlinsertcion
                errorM = "Imposible eliminar unidad de conservación  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_unidad_conservacion (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" & _
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) values (" & _
           "'ELIMINA UNIDAD','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," & _
           id_unidad_conservacion & ",'" & iptrans & "','" & hor & "','DOCUARCHI-WEB')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualizar los unidad de conservación  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Eliminar_unidad_conservacion = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Eliminar_unidad_conservacion = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() + _
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Eliminar_unidad_conservacion = errorM

        End Try
    End Function
    Function Verifica_existencia_unidad_conservacion_control_produccion(ByVal id_unidad_conservacion As Integer, _
    ByRef existencia_documento_unidad As String) As String
        '*****************************************************************
        'Funcion : Verfica si la unidad de conservación tiene documentos
        'relacionado en produción
        'Fecha 2015-01-19
        'Ing Migeuel Angel Urueta Miranda
        'Modificado para entorno web 2016-10-26
        '*****************************************************************
        Try
            Dim SqlConsulta As String = "select ID_UNIDAD_CONSERVACION from registro_producion_documental " & _
            " where  ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion & " limit 1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Verifica_existencia_unidad_conservacion_control_produccion = " Error conexión de base de datos función Verifica_existencia_unidad_conservacion_control_produccion  " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                existencia_documento_unidad = "YES"
                Verifica_existencia_unidad_conservacion_control_produccion = "YES"
                Exit Function
            Else
                existencia_documento_unidad = "NO"
                Verifica_existencia_unidad_conservacion_control_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_unidad_conservacion_control_produccion = "Inconsistencia función Verifica_existencia_unidad_conservacion_control_produccion " & ex.Message
        End Try
    End Function
    Function Verfica_Existencia_Expedientes_en_unidad_conservacion(ByVal id_unidad_conservacion As Integer) As String
        '*******************************************************************
        'Funcion Verfica existencia expediente en unidad de conservacion
        'Fecha 2014-09-25  
        'Ing Miguel Angel Urueta Miranda
        '*******************************************************************
        Try
            Dim SqlConsulta As String = "SELECT * " & _
                    " FROM  expediente_archivo where UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Verfica_Existencia_Expedientes_en_unidad_conservacion = " Error verificando existencia unidad de conservacion  " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verfica_Existencia_Expedientes_en_unidad_conservacion = "La unidad de conservacion tiene expedientes registrados"
                Exit Function
            Else
                Verfica_Existencia_Expedientes_en_unidad_conservacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verfica_Existencia_Expedientes_en_unidad_conservacion = "Inconsistencia funcion Verfica_Existencia_Expedientes_en_unidad_conservacion " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_unidad_conservacion_contenedora_unidad_archivado(ByVal id_unidad_conservacion As Integer, _
                                                                                  ByRef existencia_documento_unidad As String) As String
        '*****************************************************************
        'Funcion : Verfica si la unidad de conservación tiene otras unidades
        'tipo 2 relacionadas
        'Fecha 2015-01-19
        'Ing Migeuel Angel Urueta Miranda
        '*****************************************************************
        Try
            Dim SqlConsulta As String = "select ID_UNIDAD_CONSERVACION from unidad_conservacion " & _
                " where  ID_UNIDAD_CONSERVACION_TOPLOGICA=" & id_unidad_conservacion & " limit 1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Verifica_existencia_unidad_conservacion_contenedora_unidad_archivado = " Error Conexión Base Datos función Verifica_existencia_unidad_conservacion_contenedora_unidad_archivado " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia_documento_unidad = "NO"
                Verifica_existencia_unidad_conservacion_contenedora_unidad_archivado = "YES"
            Else

                existencia_documento_unidad = "YES"
                Verifica_existencia_unidad_conservacion_contenedora_unidad_archivado = "YES"
            End If
        Catch ex As Exception
            Verifica_existencia_unidad_conservacion_contenedora_unidad_archivado = "Inconsistencia función " & vbCrLf & _
            "  Verifica_existencia_unidad_conservacion_contenedora_unidad_archivado " & vbCrLf & ex.Message
        End Try

    End Function
    Function Retorna_id_tipo_unidad_almacenamiento_expediente(ByVal nombre_tipo As String, ByVal id_tipo_unidad As Integer, ByRef identificacion_tipo As Integer) As String
        '-------------------------------------------------------------------
        'Funcio : Retorna la identifacion del tipo documental contenedor
        'de expedientes con el nombre del expediente y el tipo contendor
        '3
        'Fecha 2016-09-08
        'Ing : Miguel Angel Urueta
        '--------------------------------------------------------------------
        Try
            Dim refclas As New ClassTrdDocumental
            Dim result As String = ""
            Dim sqlconsulta As String = "Select ID_TIPO_UNIDAD from tipo_unidad_conservacion  " & _
            " where TIPO_UNIDAD=" & id_tipo_unidad & " AND NOMBRE_TIPO_UNIDAD='" & nombre_tipo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                Retorna_id_tipo_unidad_almacenamiento_expediente = " Error Conexión Base Datos función Retorna_id_tipo_unidad_almacenamiento_expediente " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_tipo_unidad_almacenamiento_expediente = "Imposible encontrar la identificación del tipo " & nombre_tipo
                Exit Function
            Else
                identificacion_tipo = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_tipo_unidad_almacenamiento_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_tipo_unidad_almacenamiento_expediente = "Inconsistencia general función Retorna_id_tipo_unidad_almacenamiento_expediente " & ex.Message
        End Try
    End Function
    Function Ratorna_codigo_corto_entrepaño_id_entrepaño(ByVal id_entrepaño As Integer, ByRef codigo_corto_entrpaño As String) As String
        '**************************************************************
        'Funcion : Retorna codigo corto entrepaño por id entreapaño
        'Fecha : 2017-01-23
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************************
        Try
            Dim sqlconsulta As String = "Select CODIGO_CORTO  from entre_paño " & _
               " where ID_ENTREPAÑO='" & id_entrepaño & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                Ratorna_codigo_corto_entrepaño_id_entrepaño = " Error Conexión Base Datos función Ratorna_codigo_corto_entrepaño_id_entrepaño " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Ratorna_codigo_corto_entrepaño_id_entrepaño = "Imposible encontrar datos codigo corto entrepaño "
                Exit Function
            Else
                codigo_corto_entrpaño = Datset.Tables(0).Rows(0).Item(0)
                Ratorna_codigo_corto_entrepaño_id_entrepaño = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Ratorna_codigo_corto_entrepaño_id_entrepaño = "Inconsistencia función Ratorna_codigo_corto_entrepaño_id_entrepaño " & ex.Message
        End Try
    End Function
    Function Retorna_datos_unidad_contenedora_por_id(ByVal id_unidad_contenedora As Integer, _
                                                     ByRef nombre_tipo_unidad As String, _
                                                     ByRef codigo_unico_unidad As String) As String
        '**************************************************************
        'Funcion : Retorna nombre del tipo de unidad contenedora
        'y el codigo unico de la unidad contenedora
        'Fecha : 2017-01-23
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************************
        Try
            Dim sqlconsulta As String = "Select tpuc.NOMBRE_TIPO_UNIDAD,uc.CODIGO_UNICO  from unidad_conservacion as uc " & _
                " inner join tipo_unidad_conservacion as tpuc on (tpuc.ID_TIPO_UNIDAD=uc.ID_TIPO_UNIDAD_CONSERVACION)" & _
               " where ID_UNIDAD_CONSERVACION='" & id_unidad_contenedora & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                Retorna_datos_unidad_contenedora_por_id = " Error Conexión Base Datos función Retorna_datos_unidad_contenedora_por_id " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_datos_unidad_contenedora_por_id = "Imposible encontrar datos de tipo unidad contenedora y codigo unico "
                Exit Function
            Else
                nombre_tipo_unidad = Datset.Tables(0).Rows(0).Item(0)
                codigo_unico_unidad = Datset.Tables(0).Rows(0).Item(1)
                Retorna_datos_unidad_contenedora_por_id = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_datos_unidad_contenedora_por_id = "Inconsistencia función Retorna_datos_unidad_contenedora_por_id " & ex.Message
        End Try
    End Function
    Function Retorna_descripcion_tipo_unidad_conservacion(ByVal nombre_tipo_unidad As String, _
    ByRef descripcion_unidad As String) As String
        '**************************************************************
        'Funcion : Retorna descripción del tipo unidad enviando
        'como parametro el nombre de la unidad
        'Fecha : 2015-01-19 Modificado para la version web 2016-09-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************************
        Try
            Dim sqlconsulta As String = "Select DESCRIPCION_UNIDAD from tipo_unidad_conservacion where " & _
               "  NOMBRE_TIPO_UNIDAD='" & nombre_tipo_unidad & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                Retorna_descripcion_tipo_unidad_conservacion = " Error Conexión Base Datos función Retorna_descripcion_tipo_unidad_conservacion" & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_descripcion_tipo_unidad_conservacion = "Imposible encontrar la descripción del tipo de unidad de conservación "
                Exit Function
            Else
                descripcion_unidad = Datset.Tables(0).Rows(0).Item(0)
                Retorna_descripcion_tipo_unidad_conservacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_descripcion_tipo_unidad_conservacion = "Inconsistencia función Retorna_descripcion_tipo_unidad_conservacion " & ex.Message
        End Try

    End Function
    Function Retorna_descripcion_tipo_unidad_conservacion_por_id(ByVal id_tipo_unidad As Integer, _
                                                                 ByRef descripcion_unidad As String) As String
        '**************************************************************
        'Función : Retorna descripción del tipo unidad enviando
        'como parametro la identificación del tipo
        'Fecha : 2018-08-28 
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************************
        Try
            Dim sqlconsulta As String = "Select DESCRIPCION_UNIDAD from tipo_unidad_conservacion where " & _
               "  ID_TIPO_UNIDAD=" & id_tipo_unidad
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Dim result As String = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                Retorna_descripcion_tipo_unidad_conservacion_por_id = " Error Conexión Base Datos función Retorna_descripcion_tipo_unidad_conservacion_por_id " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                descripcion_unidad = ""
                Retorna_descripcion_tipo_unidad_conservacion_por_id = "YES"
                Exit Function
            Else
                descripcion_unidad = Datset.Tables(0).Rows(0).Item(0)
                Retorna_descripcion_tipo_unidad_conservacion_por_id = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_descripcion_tipo_unidad_conservacion_por_id = "Inconsistencia función Retorna_descripcion_tipo_unidad_conservacion_por_id " & ex.Message
        End Try

    End Function
    Function lista_tipos_unidades_Combo_seleccion_items(ByRef refcombo As DropDownList, _
                                                        ByVal tipo_unidad As Integer, _
                                                        ByVal id_tipo As Integer) As String
        '**************************************************
        'Función : Lista los tipos de unidades 
        'disponibles, y asigna el tipo pasado por parametro
        'Fecha : 2018-08-28 
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************
        Try
            Dim refclas As New ClassTrdDocumental
            Dim result As String = ""
            refcombo.Items.Clear()
            Dim sqlconsulta As String = "Select ID_TIPO_UNIDAD,NOMBRE_TIPO_UNIDAD from tipo_unidad_conservacion  " & _
            " where TIPO_UNIDAD=" & tipo_unidad
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                lista_tipos_unidades_Combo_seleccion_items = " Error Conexión Base Datos función lista_tipos_unidades_Combo_seleccion_items " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                lista_tipos_unidades_Combo_seleccion_items = "Imposible encontrar tipos de unidades documentales para asignar "
                Exit Function
            Else
                Dim ilist As System.Web.UI.WebControls.ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New System.Web.UI.WebControls.ListItem
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    refcombo.Items.Add(ilist)
                Next
                For i As Integer = 0 To refcombo.Items.Count - 1
                    If refcombo.Items(i).Value = id_tipo Then
                        refcombo.Items(i).Selected = True
                        Exit For
                    End If
                Next
                lista_tipos_unidades_Combo_seleccion_items = "YES"
                Exit Function
            End If
        Catch ex As Exception
            lista_tipos_unidades_Combo_seleccion_items = "Inconsistencia función lista_tipos_unidades_Combo_seleccion_items " & ex.Message
        End Try
    End Function
    Function lista_tipos_unidades_Combo(ByRef refcombo As DropDownList, ByVal tipo_unidad As Integer) As String
        '**************************************************
        'Función : Lista los tipos de unidades 
        'disponibles
        'Fecha : 2015-01-05 Modificado para web 2016-09-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************
        Try
            Dim refclas As New ClassTrdDocumental
            Dim result As String = ""
            refcombo.Items.Clear()
            Dim sqlconsulta As String = "Select NOMBRE_TIPO_UNIDAD from tipo_unidad_conservacion  " & _
            " where TIPO_UNIDAD=" & tipo_unidad
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                lista_tipos_unidades_Combo = " Error Conexión Base Datos función lista_tipos_unidades_Combo " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                lista_tipos_unidades_Combo = "Imposible encontrar tipos de unidades documentales para asignar"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                lista_tipos_unidades_Combo = "YES"
                Exit Function
            End If

        Catch ex As Exception
            lista_tipos_unidades_Combo = "Inconsistencia funcion lista_tipos_unidades_Combo " & ex.Message
        End Try
    End Function
    Function lista_tipos_unidades_Combo_inten_vacio(ByRef refcombo As DropDownList) As String
        '**************************************************
        'Función : Lista los tipos de unidades 
        'disponibles
        'Fecha : 2015-01-05 Modificado para web 2016-09-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************
        Try
            Dim refclas As New ClassTrdDocumental
            Dim result As String = ""
            refcombo.Items.Clear()
            Dim sqlconsulta As String = "Select NOMBRE_TIPO_UNIDAD from tipo_unidad_conservacion  "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If result <> "YES" Then
                lista_tipos_unidades_Combo_inten_vacio = " Error Conexión Base Datos función lista_tipos_unidades_Combo_inten_vacio " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                lista_tipos_unidades_Combo_inten_vacio = "Imposible encontrar tipos de unidades documentales para asignar"
                Exit Function
            Else
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                lista_tipos_unidades_Combo_inten_vacio = "YES"
                Exit Function
            End If

        Catch ex As Exception
            lista_tipos_unidades_Combo_inten_vacio = "Inconsistencia funcion lista_tipos_unidades_Combo_inten_vacio " & ex.Message
        End Try
    End Function
    Function Agregar_unidad_conservacion_anidad_en_nodo_treview(ByRef Tree As TreeNode, _
    ByVal estru_unidad_conservacion As unidad_conservacion, ByRef reftre As TreeView, _
    Optional ByVal opcion_eliminar_previo As Integer = 0) As String
        '*******************************************************
        'Función : Agrega un nodo a un nodo seleccionado con los
        'datos de la unidad de conservación
        'Fecha : 2015-01-21 
        'Ingeniero : Miguel Angel Urueta Miranda
        '********************************************************
        Try
            Dim NodeTree As New TreeNode
            Dim ref_fecha_extrema_ini As String = ""
            Dim ref_fecha_extrema_fin As String = ""
            If estru_unidad_conservacion.FECHA_EXTREMA_INICIAL <> "" Then
                ref_fecha_extrema_ini = Left(estru_unidad_conservacion.FECHA_EXTREMA_INICIAL, 10)
            Else
                ref_fecha_extrema_ini = estru_unidad_conservacion.FECHA_EXTREMA_INICIAL
            End If
            If estru_unidad_conservacion.FECHA_EXTREMA_FINAL <> "" Then
                ref_fecha_extrema_fin = Left(estru_unidad_conservacion.FECHA_EXTREMA_FINAL, 10)
            Else
                ref_fecha_extrema_fin = estru_unidad_conservacion.FECHA_EXTREMA_FINAL
            End If
            Dim tipo_unidad As String = ""
            If estru_unidad_conservacion.NOMBRE_TIPO_UNIDAD Is Nothing Then
            Else
                tipo_unidad = "TIPO UNIDAD : (" & estru_unidad_conservacion.NOMBRE_TIPO_UNIDAD & ")  "
            End If
            NodeTree.Text = tipo_unidad & "CODIGO UNICO: " & estru_unidad_conservacion.CODIGO_UNICO & " TEMA: " & _
            estru_unidad_conservacion.TEMA_UNIDAD_CONSERVACION & _
            " FECHAS EXTREMAS: " & ref_fecha_extrema_ini & " HASTA " & _
            ref_fecha_extrema_fin & " RANGO EXTREMOS:" & estru_unidad_conservacion.RANGO_EXTREMO_INICIAL & " HASTA " & _
            estru_unidad_conservacion.RANGO_EXTREMO_FINAL & "-" & estru_unidad_conservacion.VOLUMEN_UNIDAD_CONSERVACION
            NodeTree.Value = estru_unidad_conservacion.ID_UNIDAD_CONSERVACION
            If opcion_eliminar_previo = 0 Then
                Dim result As String = Rescursive_node_tree_elimina(reftre, NodeTree.Text)
                If result <> "YES" Then
                    Agregar_unidad_conservacion_anidad_en_nodo_treview = result
                    Exit Function
                End If
            End If
            If estru_unidad_conservacion.VOLUMEN_UNIDAD_CONSERVACION > 1 Then
                'NodeTree.Selected = True
            End If
            NodeTree.ToolTip = "Expediente"
            Tree.ChildNodes.Add(NodeTree)
            Agregar_unidad_conservacion_anidad_en_nodo_treview = "YES"
        Catch ex As Exception
            Agregar_unidad_conservacion_anidad_en_nodo_treview = "Inconsistencia función Agregar_unidad_conservacion_anidad_en_nodo_treview  " & _
            ex.Message
        End Try
    End Function
    Function Rescursive_node_tree_elimina(ByRef aTreeView As TreeView, ByVal text As String) As String
        Try
            Dim n As TreeNode
            For Each n In aTreeView.Nodes
                Dim result As String = PrintRecursive(n, text, aTreeView)
                If result <> "YES" Then
                    Rescursive_node_tree_elimina = result
                    Exit Function
                End If
            Next

            Rescursive_node_tree_elimina = "YES"
        Catch ex As Exception
            Rescursive_node_tree_elimina = "Inconsistencia función Rescursive_node_tree_elimina " & ex.Message
        End Try
    End Function
    Function PrintRecursive(ByRef n As TreeNode, ByVal text As String, ByRef tre As TreeView)

        Try
            '*** Es aqui donde añado lo que necesito guardar de cada nodo ***  
            Dim aNode As TreeNode
            'Por cada nodo de la raiz
            For Each aNode In n.ChildNodes
                If aNode.Value = text Then
                    'Dim pNodo As TreeNode = aNode.Parent
                    n.ChildNodes.Remove(aNode)
                    Exit For
                Else
                    If aNode.ChildNodes.Count > 0 Then
                        PrintRecursive(aNode, text, tre)
                    End If
                End If

            Next

            PrintRecursive = "YES"
        Catch ex As Exception
            PrintRecursive = "Inconsistencia función PrintRecursive " & ex.Message
        End Try
    End Function
    Function Listar_unidades_conservacion_anidades_en_unidades_de_conservacion(ByVal id_unidad_conservacion_contendora As Integer, _
    ByVal tipo_unidad_conservacion As Integer, ByRef estru_unidad_conservacion() As unidad_conservacion) As String
        '************************************************************
        'Funcion Listar estrucutura unidad de conservación contenedora
        ' de unidades de conservación con el
        'parametro id entrepaño, tipo unidad conservación en tre 
        '1- Unidad contendora de expediente
        '2- UNIDAD CONTENEDORA EXPEDIENTE de documentos
        '3- Unidad contendora de unidades de conservación
        'Fecha 2015-01-22
        'Ing : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            Erase estru_unidad_conservacion
            Dim campos_seleccion As String = "ID_UNIDAD_CONSERVACION,ENTRE_PAÑO_ID_ENTREPAÑO,CONSECUTIVO_UNIDAD_CONSERVACION," & _
            "CONSECUTIVO_EXPEDIENTE,CONSECUTIVO_DOCUMENTO,CODIGO_CORTO,CODIGO_UNICO,TIPO_UNIDAD_CONSERVACION,NUMERO_FOLIO_UNIDAD_CONSERVACION," & _
            "ID_USUARIO_GESTION,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA,CODIGO_SERIE,NOMBRE_SERIE,CODIGO_SUBSERIE,NOMBRE_SUBSERIE," & _
            "ESTADO_UNIDAD_CONSERVACION,ESTADO_ARCHIVO_INIDAD,ID_TIPO_UNIDAD_CONSERVACION,FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL," & _
            "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_UNIDAD_CONSERVACION,DESCRIPCION_UNIDAD_CONSERVACION,CODIGO_BARRAS_UNIDAD,TUC.NOMBRE_TIPO_UNIDAD,VOLUMEN_UNIDAD_CONSERVACION"
            Dim SqlConsulta As String = "select " & campos_seleccion & " from  unidad_conservacion " & _
            "LEFT OUTER JOIN tipo_unidad_conservacion as tuc on (tuc.ID_TIPO_UNIDAD=ID_TIPO_UNIDAD_CONSERVACION) " & _
                                              " where ID_UNIDAD_CONSERVACION_TOPLOGICA=" & id_unidad_conservacion_contendora & " and TIPO_UNIDAD_CONSERVACION=" & tipo_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("unidad_conservacion")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Listar_unidades_conservacion_anidades_en_unidades_de_conservacion = " Error solicitando estrucutura unidad conservacion " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_unidad_conservacion(i)
                    estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_UNIDAD_CONSERVACION")
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = 0
                    Else
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = Datset.Tables(0).Rows(i).Item(1)
                    End If
                    estru_unidad_conservacion(i).CONSECUTIVO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).CONSECUTIVO_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_EXPEDIENTE")
                    estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_DOCUMENTO")
                    estru_unidad_conservacion(i).CODIGO_CORTO = Datset.Tables(0).Rows(i).Item("CODIGO_CORTO")
                    estru_unidad_conservacion(i).CODIGO_UNICO = Datset.Tables(0).Rows(i).Item("CODIGO_UNICO")
                    estru_unidad_conservacion(i).TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TIPO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).NUMERO_FOLIO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("NUMERO_FOLIO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).ID_USUARIO_GESTION = Datset.Tables(0).Rows(i).Item("ID_USUARIO_GESTION")
                    If Datset.Tables(0).Rows(i).IsNull(10) = True Then
                        estru_unidad_conservacion(i).FECHA_CREACION = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_CREACION = Datset.Tables(0).Rows(i).Item("FECHA_CREACION").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(11) = True Then
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = Datset.Tables(0).Rows(i).Item("CODIGO_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(12) = True Then
                        estru_unidad_conservacion(i).NOMBRE_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_AREA = Datset.Tables(0).Rows(i).Item("NOMBRE_AREA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(13) = True Then
                        estru_unidad_conservacion(i).CODIGO_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_SERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(14) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(15) = True Then
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = 0
                    Else
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SUBSERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(16) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SUBSERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(17) = True Then
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ESTADO_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(18) = True Then
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = 0
                    Else
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = Datset.Tables(0).Rows(i).Item("ESTADO_ARCHIVO_INIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(19) = True Then
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_TIPO_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(20) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_INICIAL").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(21) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_FINAL").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(22) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(23) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(24) = True Then
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TEMA_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(25) = True Then
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("DESCRIPCION_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(26) = True Then
                        estru_unidad_conservacion(i).CODIGO_BARRAS_UNIDAD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_BARRAS_UNIDAD = Datset.Tables(0).Rows(i).Item("CODIGO_BARRAS_UNIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(27) = True Then
                        estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD = Datset.Tables(0).Rows(i).Item("NOMBRE_TIPO_UNIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(28) = True Then
                        estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("VOLUMEN_UNIDAD_CONSERVACION")
                    End If

                Next
                Listar_unidades_conservacion_anidades_en_unidades_de_conservacion = "YES"
                Exit Function
            Else

                Listar_unidades_conservacion_anidades_en_unidades_de_conservacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_unidades_conservacion_anidades_en_unidades_de_conservacion = "Inconsistencia general función Listar_unidades_conservacion_anidades_en_unidades_de_conservacion " & ex.Message
        End Try
    End Function
    Function Listar_Unidad_Conservacion_treview(ByRef Tree As TreeView, ByVal estru_unidad_conservacion() As unidad_conservacion _
    , Optional ByVal limpia_nodo As Integer = 0) As String
        Try
            If limpia_nodo = 0 Then
                Tree.Nodes.Clear()
            End If
            If estru_unidad_conservacion Is Nothing Then
                Listar_Unidad_Conservacion_treview = "YES"
                Exit Function
            End If
            For i As Integer = 0 To estru_unidad_conservacion.Length - 1
                Dim NodeTree As New TreeNode
                Dim ref_fecha_extrema_ini As String = ""
                Dim ref_fecha_extrema_fin As String = ""
                If estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL <> "" Then
                    ref_fecha_extrema_ini = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL, 10)
                Else
                    ref_fecha_extrema_ini = estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL
                End If
                If estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL <> "" Then
                    ref_fecha_extrema_fin = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL, 10)
                Else
                    ref_fecha_extrema_fin = estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL
                End If
                Dim tipo_unidad As String = ""
                If estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD Is Nothing Then
                Else
                    tipo_unidad = "TIPO UNIDAD : (" & estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD & ")  "
                End If
                NodeTree.Text = tipo_unidad & "CODIGO UNICO: " & estru_unidad_conservacion(i).CODIGO_UNICO & " TEMA: " & _
                estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION & _
                " FECHAS EXTREMAS: " & ref_fecha_extrema_ini & " HASTA " & _
                ref_fecha_extrema_fin & " RANGO EXTREMOS:" & estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL & " HASTA " & _
                estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL & "-" & estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION
                NodeTree.Value = estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION
                If estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION > 1 Then
                    'NodeTree.ForeColor = Color.Blue
                End If
                Tree.Nodes.Add(NodeTree)
            Next
            Listar_Unidad_Conservacion_treview = "YES"
        Catch ex As Exception
            Listar_Unidad_Conservacion_treview = "Inconsistencia funcion Listar_Unidad_Conservacion_treview " & ex.Message
        End Try
    End Function
    Function Listar_unidades_conservacion_anidades_en_unidades_en_entrepaño(ByVal id_entre_paño_contenedor As Integer, _
    ByVal tipo_unidad_conservacion As Integer, ByRef estru_unidad_conservacion() As unidad_conservacion) As String
        '*****************************************************************
        'Funcion Listar entreapño contenedor de unidades de conservación
        '
        'parametro id entrepaño, tipo unidad conservación en tre 
        '1- Unidad contendora de expediente
        '2- UNIDAD CONTENEDORA EXPEDIENTE de documentos
        '3- Unidad contendora de unidades de conservación
        'Fecha 2015-01-23
        'Ing : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            Erase estru_unidad_conservacion
            Dim campos_seleccion As String = "ID_UNIDAD_CONSERVACION,ENTRE_PAÑO_ID_ENTREPAÑO,CONSECUTIVO_UNIDAD_CONSERVACION," & _
            "CONSECUTIVO_EXPEDIENTE,CONSECUTIVO_DOCUMENTO,CODIGO_CORTO,CODIGO_UNICO,TIPO_UNIDAD_CONSERVACION,NUMERO_FOLIO_UNIDAD_CONSERVACION," & _
            "ID_USUARIO_GESTION,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA,CODIGO_SERIE,NOMBRE_SERIE,CODIGO_SUBSERIE,NOMBRE_SUBSERIE," & _
            "ESTADO_UNIDAD_CONSERVACION,ESTADO_ARCHIVO_INIDAD,ID_TIPO_UNIDAD_CONSERVACION,FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL," & _
            "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_UNIDAD_CONSERVACION,DESCRIPCION_UNIDAD_CONSERVACION,CODIGO_BARRAS_UNIDAD,TUC.NOMBRE_TIPO_UNIDAD,VOLUMEN_UNIDAD_CONSERVACION"
            Dim SqlConsulta As String = "select " & campos_seleccion & " from  unidad_conservacion " & _
            "LEFT OUTER JOIN tipo_unidad_conservacion as tuc on (tuc.ID_TIPO_UNIDAD=ID_TIPO_UNIDAD_CONSERVACION) " & _
                                              " where ENTRE_PAÑO_ID_ENTREPAÑO=" & id_entre_paño_contenedor & " and TIPO_UNIDAD_CONSERVACION=" & tipo_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Listar_unidades_conservacion_anidades_en_unidades_en_entrepaño = " Error solicitando estrucutura unidad conservacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then

                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_unidad_conservacion(i)
                    estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_UNIDAD_CONSERVACION")
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = 0
                    Else
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = Datset.Tables(0).Rows(i).Item(1)
                    End If
                    estru_unidad_conservacion(i).CONSECUTIVO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).CONSECUTIVO_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_EXPEDIENTE")
                    estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_DOCUMENTO")
                    estru_unidad_conservacion(i).CODIGO_CORTO = Datset.Tables(0).Rows(i).Item("CODIGO_CORTO")
                    estru_unidad_conservacion(i).CODIGO_UNICO = Datset.Tables(0).Rows(i).Item("CODIGO_UNICO")
                    estru_unidad_conservacion(i).TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TIPO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).NUMERO_FOLIO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("NUMERO_FOLIO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).ID_USUARIO_GESTION = Datset.Tables(0).Rows(i).Item("ID_USUARIO_GESTION")
                    If Datset.Tables(0).Rows(i).IsNull(10) = True Then
                        estru_unidad_conservacion(i).FECHA_CREACION = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_CREACION = Datset.Tables(0).Rows(i).Item("FECHA_CREACION").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(11) = True Then
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = Datset.Tables(0).Rows(i).Item("CODIGO_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(12) = True Then
                        estru_unidad_conservacion(i).NOMBRE_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_AREA = Datset.Tables(0).Rows(i).Item("NOMBRE_AREA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(13) = True Then
                        estru_unidad_conservacion(i).CODIGO_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_SERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(14) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(15) = True Then
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = 0
                    Else
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SUBSERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(16) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SUBSERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(17) = True Then
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ESTADO_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(18) = True Then
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = 0
                    Else
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = Datset.Tables(0).Rows(i).Item("ESTADO_ARCHIVO_INIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(19) = True Then
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_TIPO_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(20) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_INICIAL").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(21) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_FINAL").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(22) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(23) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(24) = True Then
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TEMA_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(25) = True Then
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("DESCRIPCION_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(26) = True Then
                        estru_unidad_conservacion(i).CODIGO_BARRAS_UNIDAD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_BARRAS_UNIDAD = Datset.Tables(0).Rows(i).Item("CODIGO_BARRAS_UNIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(27) = True Then
                        estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD = Datset.Tables(0).Rows(i).Item("NOMBRE_TIPO_UNIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(28) = True Then
                        estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("VOLUMEN_UNIDAD_CONSERVACION")
                    End If

                Next
                Listar_unidades_conservacion_anidades_en_unidades_en_entrepaño = "YES"
                Exit Function
            Else
                Listar_unidades_conservacion_anidades_en_unidades_en_entrepaño = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_unidades_conservacion_anidades_en_unidades_en_entrepaño = "Inconsistencia general función Listar_unidades_conservacion_anidades_en_unidades_en_entrepaño " & ex.Message
        End Try
    End Function
    Function Listar_Unidad_Conservacion_treview_nodo_entre_pano(ByRef Tree As TreeView, _
                                                                ByVal estru_unidad_conservacion() As unidad_conservacion, _
                                                                ByVal node_tag As String, _
                                                                ByVal node_text As String, _
                                                                ByRef tred As TreeNode) As String
        Try
            Tree.Nodes.Clear()
            'Dim tred As New TreeNode
            tred.Value = node_tag
            tred.Text = node_text
            tred.ImageUrl = "../workflow/imageneswf/rectangle-wide-light-pano.png"
            Tree.Nodes.Add(tred)
            If estru_unidad_conservacion Is Nothing Then
                Listar_Unidad_Conservacion_treview_nodo_entre_pano = "YES"
                Exit Function
            End If
            For i As Integer = 0 To estru_unidad_conservacion.Length - 1
                Dim NodeTree As New TreeNode
                Dim ref_fecha_extrema_ini As String = ""
                Dim ref_fecha_extrema_fin As String = ""
                If estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL <> "" Then
                    ref_fecha_extrema_ini = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL, 10)
                Else
                    ref_fecha_extrema_ini = estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL
                End If
                If estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL <> "" Then
                    ref_fecha_extrema_fin = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL, 10)
                Else
                    ref_fecha_extrema_fin = estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL
                End If
                Dim tipo_unidad As String = ""
                If estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD Is Nothing Then
                Else
                    tipo_unidad = "TIPO UNIDAD : (" & estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD & ")  "
                End If
                NodeTree.Text = tipo_unidad & "CODIGO UNICO: " & estru_unidad_conservacion(i).CODIGO_UNICO & " TEMA: " & _
                estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION & _
                " FECHAS EXTREMAS: " & ref_fecha_extrema_ini & " HASTA " & _
                ref_fecha_extrema_fin & " RANGO EXTREMOS:" & estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL & " HASTA " & _
                estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL & "-" & estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION
                NodeTree.Value = estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION
                If estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION > 1 Then
                    'NodeTree. = Color.Blue
                End If
                NodeTree.ImageUrl = "../Gestion/imagenes/caja_exp.png"
                NodeTree.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE"
                tred.ChildNodes.Add(NodeTree)
            Next
            'Tree.SelectedNod = Tree.Nodes.Item(0)
            Listar_Unidad_Conservacion_treview_nodo_entre_pano = "YES"
        Catch ex As Exception
            Listar_Unidad_Conservacion_treview_nodo_entre_pano = "Inconsistencia funcion Listar_Unidad_Conservacion_treview_nodo_entre_pano " & ex.Message
        End Try
    End Function
    Function Listar_Unidad_Conservacion_nono_nodo_entre_pano(ByVal estru_unidad_conservacion() As unidad_conservacion, _
                                                             ByVal node_tag As String, _
                                                             ByVal node_text As String, _
                                                             ByRef tred As TreeNode) As String
        Try

            'Dim tred As New TreeNode
            'tred.Value = node_tag
            'tred.Text = node_text

            If estru_unidad_conservacion Is Nothing Then
                Listar_Unidad_Conservacion_nono_nodo_entre_pano = "YES"
                Exit Function
            End If
            For i As Integer = 0 To estru_unidad_conservacion.Length - 1
                Dim NodeTree As New TreeNode
                Dim ref_fecha_extrema_ini As String = ""
                Dim ref_fecha_extrema_fin As String = ""
                If estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL <> "" Then
                    ref_fecha_extrema_ini = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL, 10)
                Else
                    ref_fecha_extrema_ini = estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL
                End If
                If estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL <> "" Then
                    ref_fecha_extrema_fin = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL, 10)
                Else
                    ref_fecha_extrema_fin = estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL
                End If
                Dim tipo_unidad As String = ""
                If estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD Is Nothing Then
                Else
                    tipo_unidad = "TIPO UNIDAD : (" & estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD & ")  "
                End If
                NodeTree.Text = tipo_unidad & "CODIGO UNICO: " & estru_unidad_conservacion(i).CODIGO_UNICO & " TEMA: " & _
                estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION & _
                " FECHAS EXTREMAS: " & ref_fecha_extrema_ini & " HASTA " & _
                ref_fecha_extrema_fin & " RANGO EXTREMOS:" & estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL & " HASTA " & _
                estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL & "-" & estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION
                NodeTree.Value = estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION
                If estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION > 1 Then
                    'NodeTree. = Color.Blue
                End If
                NodeTree.ImageUrl = "../Gestion/imagenes/caja_exp.png"
                NodeTree.ToolTip = "UNIDAD CONTENEDORA EXPEDIENTE"
                tred.ChildNodes.Add(NodeTree)
            Next
            'Tree.SelectedNod = Tree.Nodes.Item(0)
            Listar_Unidad_Conservacion_nono_nodo_entre_pano = "YES"
        Catch ex As Exception
            Listar_Unidad_Conservacion_nono_nodo_entre_pano = "Inconsistencia funcion Listar_Unidad_Conservacion_nono_nodo_entre_pano " & ex.Message
        End Try
    End Function
    Function Listar_unidades_conservacion_contenedoras_de_unidades_conservacion(ByVal id_entre_paño As Integer, _
                                                                                ByVal tipo_unidad_conservacion As Integer, _
                                                                                ByRef estru_unidad_conservacion() As unidad_conservacion) As String
        '***************************************************************
        'Funcion Listar estrucutura unidad de conservación contenedora
        ' de unidades de conservación con el
        'parametro id entrepaño, tipo unidad conservación en tre 
        '1- Unidad contendora de expediente
        '2- Unidad contenedora de documentos
        '3- Unidad contendora de unidades de conservación
        'Fecha 2015-01-19
        'Ing : Miguel Angel Urueta Miranda
        '****************************************************************
        Try
            Erase estru_unidad_conservacion
            Dim campos_seleccion As String = "ID_UNIDAD_CONSERVACION,ENTRE_PAÑO_ID_ENTREPAÑO,CONSECUTIVO_UNIDAD_CONSERVACION," & _
            "CONSECUTIVO_EXPEDIENTE,CONSECUTIVO_DOCUMENTO,CODIGO_CORTO,CODIGO_UNICO,TIPO_UNIDAD_CONSERVACION,NUMERO_FOLIO_UNIDAD_CONSERVACION," & _
            "ID_USUARIO_GESTION,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA,CODIGO_SERIE,NOMBRE_SERIE,CODIGO_SUBSERIE,NOMBRE_SUBSERIE," & _
            "ESTADO_UNIDAD_CONSERVACION,ESTADO_ARCHIVO_INIDAD,ID_TIPO_UNIDAD_CONSERVACION,FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL," & _
            "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_UNIDAD_CONSERVACION,DESCRIPCION_UNIDAD_CONSERVACION,CODIGO_BARRAS_UNIDAD,TUC.NOMBRE_TIPO_UNIDAD,VOLUMEN_UNIDAD_CONSERVACION"
            Dim SqlConsulta As String = "select " & campos_seleccion & " from  unidad_conservacion " & _
            "LEFT OUTER JOIN tipo_unidad_conservacion as tuc on (tuc.ID_TIPO_UNIDAD=ID_TIPO_UNIDAD_CONSERVACION) " & _
                                              " where ENTRE_PAÑO_ID_ENTREPAÑO=" & id_entre_paño & " and TIPO_UNIDAD_CONSERVACION=" & tipo_unidad_conservacion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("entre_paño")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Listar_unidades_conservacion_contenedoras_de_unidades_conservacion = "Error solicitando estructura unidad de conservación " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then


                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_unidad_conservacion(i)
                    estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_UNIDAD_CONSERVACION")
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = 0
                    Else
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = Datset.Tables(0).Rows(i).Item(1)
                    End If
                    estru_unidad_conservacion(i).CONSECUTIVO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).CONSECUTIVO_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_EXPEDIENTE")
                    estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_DOCUMENTO")
                    estru_unidad_conservacion(i).CODIGO_CORTO = Datset.Tables(0).Rows(i).Item("CODIGO_CORTO")
                    estru_unidad_conservacion(i).CODIGO_UNICO = Datset.Tables(0).Rows(i).Item("CODIGO_UNICO")
                    estru_unidad_conservacion(i).TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TIPO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).NUMERO_FOLIO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("NUMERO_FOLIO_UNIDAD_CONSERVACION")
                    estru_unidad_conservacion(i).ID_USUARIO_GESTION = Datset.Tables(0).Rows(i).Item("ID_USUARIO_GESTION")
                    If Datset.Tables(0).Rows(i).IsNull(10) = True Then
                        estru_unidad_conservacion(i).FECHA_CREACION = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_CREACION = Datset.Tables(0).Rows(i).Item("FECHA_CREACION").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(11) = True Then
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = Datset.Tables(0).Rows(i).Item("CODIGO_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(12) = True Then
                        estru_unidad_conservacion(i).NOMBRE_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_AREA = Datset.Tables(0).Rows(i).Item("NOMBRE_AREA")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(13) = True Then
                        estru_unidad_conservacion(i).CODIGO_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_SERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(14) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(15) = True Then
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = 0
                    Else
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SUBSERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(16) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SUBSERIE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(17) = True Then
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ESTADO_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(18) = True Then
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = 0
                    Else
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = Datset.Tables(0).Rows(i).Item("ESTADO_ARCHIVO_INIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(19) = True Then
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ID_TIPO_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(20) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_INICIAL").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(21) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_FINAL").ToString
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(22) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(23) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(24) = True Then
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TEMA_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(25) = True Then
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("DESCRIPCION_UNIDAD_CONSERVACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(26) = True Then
                        estru_unidad_conservacion(i).CODIGO_BARRAS_UNIDAD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_BARRAS_UNIDAD = Datset.Tables(0).Rows(i).Item("CODIGO_BARRAS_UNIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(27) = True Then
                        estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD = Datset.Tables(0).Rows(i).Item("NOMBRE_TIPO_UNIDAD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(28) = True Then
                        estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).VOLUMEN_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("VOLUMEN_UNIDAD_CONSERVACION")
                    End If
                Next

                Listar_unidades_conservacion_contenedoras_de_unidades_conservacion = "YES"
                Exit Function
            Else

                Listar_unidades_conservacion_contenedoras_de_unidades_conservacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_unidades_conservacion_contenedoras_de_unidades_conservacion = "Inconsistencia general función Listar_unidades_conservacion_contenedoras_de_unidades_conservacion " & ex.Message
        End Try

    End Function
    Function Retorna_Datos_Empresa(ByVal id_empresa As Integer, _
                                   ByRef nombre_empresa As String, _
                                   ByRef nit_empresa As String) As String
        '******************************************************
        'Funcion retorna datos basicos empresa
        'Ing :Miguel Angel Urueta Miranda
        'Fecha 2014-09-29
        '******************************************************
        Try
            Dim SqlConsulta As String = "SELECT RAZON_SOCIAL_EMPRESA,NIT_EMPRESA " & _
                       " FROM  empresa_gestion_documental where ID_EMPRESA =" & id_empresa
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("empresa_gestion_documental")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_Datos_Empresa = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_empresa = Datset.Tables(0).Rows(0).Item(0)
                nit_empresa = Datset.Tables(0).Rows(0).Item(1)
                Retorna_Datos_Empresa = "YES"
                Exit Function
            Else
                Retorna_Datos_Empresa = "Imposible encontrar datos de la empresa"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Datos_Empresa = "Inconsistencia funcion Retorna_Datos_Empresa " & ex.Message
        End Try
    End Function
    
    Function zero_fill(ByRef texto_fill As String, ByVal numero_fill As Integer, ByVal textofill_add As String) As String

        Try

            texto_fill = texto_fill.ToString.PadLeft((numero_fill - texto_fill.ToString.Length) + texto_fill.ToString.Length, textofill_add)

            zero_fill = "YES"

        Catch ex As Exception

            zero_fill = "Inconsistencia general funcion " & "zero_fill" & ex.Message

        End Try

    End Function
    Function Asigna_datos_unidad_conservacion_estructura(ByVal stru_campos_docuarchi() As stru_campos_docuarchi, _
                                                         ByRef matri_gestion As estructure_gestion, _
                                                         ByVal nombre_gabinete As String) As String
        '*********************************************************
        'Funcion : Asigna datos unidad de conservación de la 
        'interface de almacenamiento a la estructura
        'Fecha : 2015-16-16
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim ref_Class_system1 As New Class_system1
            Dim Result As String = ""
            Dim opt_expediente As Integer = 0
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(opt_expediente, _
                                                                       nombre_gabinete)
            If Result <> "YES" Then
                Asigna_datos_unidad_conservacion_estructura = Result
                Exit Function
            End If
            If opt_expediente = 0 Then
                Asigna_datos_unidad_conservacion_estructura = "YES"
                Exit Function
            End If
            Dim ref_ClassWorkflowIndiceDA As New ClassWorkflowIndiceDA
            Dim valor_campo As String = ""
            Result = ref_ClassWorkflowIndiceDA.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                                        "UNIDADCONSERVA", _
                                                                                         valor_campo)
            If Result <> "YES" Then
                Asigna_datos_unidad_conservacion_estructura = Result
                Exit Function
            End If
            matri_gestion.UNIDAD_CONSERVACION = valor_campo
            Result = ref_ClassWorkflowIndiceDA.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                                         "Hidden_id_unidad_conservacion", _
                                                                                          valor_campo)
            If Result <> "YES" Then
                Asigna_datos_unidad_conservacion_estructura = Result
                Exit Function
            End If
            matri_gestion.ID_UNIDAD_CONSERVACION = valor_campo
            Result = ref_ClassWorkflowIndiceDA.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                                        "Hidden_id_tipo_unidad_conservacion", _
                                                                                         valor_campo)
            If Result <> "YES" Then
                Asigna_datos_unidad_conservacion_estructura = Result
                Exit Function
            End If
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = valor_campo
            Asigna_datos_unidad_conservacion_estructura = "YES"
        Catch ex As Exception
            Asigna_datos_unidad_conservacion_estructura = "Inconsistencia general función Asigna_datos_unidad_conservacion_estructura " & ex.Message
        End Try
    End Function
    Function Asigna_datos_unidad_conservacion_estructura(ByVal PAGE1 As Page, _
                                                         ByRef matri_gestion As estructure_gestion, _
                                                         ByVal nombre_gabinete As String) As String
        '*********************************************************
        'Funcion : Asigna datos unidad de conservación de la 
        'interface de almacenamiento a la estructura
        'Fecha : 2015-16-16
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim ref_Class_system1 As New Class_system1
            Dim Result As String = ""
            Dim opt_expediente As Integer = 0
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(opt_expediente, _
                                                                       nombre_gabinete)
            If Result <> "YES" Then
                Asigna_datos_unidad_conservacion_estructura = Result
                Exit Function
            End If
            If opt_expediente = 0 Then
                Asigna_datos_unidad_conservacion_estructura = "YES"
                Exit Function
            End If
            Dim UNIDADCONSERVA As Object = Nothing
            Dim Hidden_id_unidad_conservacion As Object = Nothing
            Dim Hidden_id_tipo_unidad_conservacion As Object = Nothing
            UNIDADCONSERVA = PAGE1.FindControl("UNIDADCONSERVA")
            If UNIDADCONSERVA Is Nothing Then
                Asigna_datos_unidad_conservacion_estructura = "Función Asigna_datos_unidad_conservacion_estructura dice : Imposible encontrar el control UNIDADCONSERVA"
                Exit Function
            End If
            Hidden_id_unidad_conservacion = PAGE1.FindControl("Hidden_id_unidad_conservacion")
            If Hidden_id_unidad_conservacion Is Nothing Then
                Asigna_datos_unidad_conservacion_estructura = "Función Asigna_datos_unidad_conservacion_estructura dice : Imposible encontrar el control Hidden_id_unidad_conservacion"
                Exit Function
            End If
            Hidden_id_tipo_unidad_conservacion = PAGE1.FindControl("Hidden_id_tipo_unidad_conservacion")
            If Hidden_id_tipo_unidad_conservacion Is Nothing Then
                Asigna_datos_unidad_conservacion_estructura = "Función Asigna_datos_unidad_conservacion_estructura dice : Imposible encontrar el control Hidden_id_tipo_unidad_conservacion"
                Exit Function
            End If
            If opt_expediente <> 0 Then
                matri_gestion.ID_UNIDAD_CONSERVACION = Hidden_id_unidad_conservacion.value
                matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = Hidden_id_tipo_unidad_conservacion.value
                matri_gestion.UNIDAD_CONSERVACION = UNIDADCONSERVA.text
                Asigna_datos_unidad_conservacion_estructura = "YES"
                Exit Function
            End If
            Asigna_datos_unidad_conservacion_estructura = "YES"
        Catch ex As Exception
            Asigna_datos_unidad_conservacion_estructura = "Inconsistencia general función Asigna_datos_unidad_conservacion_estructura " & ex.Message
        End Try
    End Function
    Function Retorna_codigo_unidad_conservacion_por_id_unidad(ByVal id_unidad_contenedora As Integer, _
                                                              ByRef codigo_unidad_contendora As String) As String
        '-----------------------------------------------------------------
        'Fucion : Retorna codigo unidad contenedora por id unidad
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-01-22
        '------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select CODIGO_UNICO  " & _
                " from unidad_conservacion where " & _
                " ID_UNIDAD_CONSERVACION='" & id_unidad_contenedora & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_codigo_unidad_conservacion_por_id_unidad = "Funcion Retorna_codigo_unidad_conservacion_por_id_unidad dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                codigo_unidad_contendora = Datset.Tables(0).Rows(0).Item(0)
                Retorna_codigo_unidad_conservacion_por_id_unidad = "YES"
                Exit Function
            Else
                Retorna_codigo_unidad_conservacion_por_id_unidad = "El sistema no pudo encontrar el codigo de unidad contenedora "
                Exit Function
            End If
        Catch ex As Exception
            Retorna_codigo_unidad_conservacion_por_id_unidad = "Inconsistencia general función Retorna_codigo_unidad_conservacion_por_id_unidad " & ex.Message
        End Try
    End Function



    Function Retorna_nombre_tipo_unidad_documental(ByVal id_tipo_unidad_documental As Integer,
                                                   ByRef nombre_tipo_unidad As String) As String
        '-----------------------------------------------------------------
        'Funcion : Reotora el nombre del tipo de unidad documental
        'con el parametro id tipo unidad documental
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-10-22
        '------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select NOMBRE_TIPO_UNIDAD_DOCUMENTAL " &
                " from TIPOS_UNIDAD_DOCUMENTAL  WHERE ID_TIPO_UNIDAD_DOCUMENTAL =" & id_tipo_unidad_documental
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_tipo_unidad_documental = "Funcion  Retorna_nombre_tipo_unidad_documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_tipo_unidad = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_tipo_unidad_documental = "YES"
                Exit Function
            Else
                Retorna_nombre_tipo_unidad_documental = "Imposible encontrar el nombre tipo unidad documental con el id tipo unidad documental (" & id_tipo_unidad_documental & ")"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_tipo_unidad_documental = "Inconsistencia general función Retorna_nombre_tipo_unidad_documental " & ex.Message
        End Try
    End Function
    Function lista_tipos_unidades_documentales(ByRef refcombo As DropDownList) As String
        '-----------------------------------------------------------------
        'Funcion : Lista tipo unidades documentales
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-10-20
        '------------------------------------------------------------------
        Try
            refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select NOMBRE_TIPO_UNIDAD_DOCUMENTAL  " & _
                " from TIPOS_UNIDAD_DOCUMENTAL  "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("TIPOS_UNIDAD_DOCUMENTAL")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                lista_tipos_unidades_documentales = "Funcion  lista_tipos_unidades_documentales " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                'refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                lista_tipos_unidades_documentales = "YES"
                Exit Function
            Else
                lista_tipos_unidades_documentales = "YES"
                Exit Function
            End If
        Catch ex As Exception
            lista_tipos_unidades_documentales = "Inconsistencia función lista_tipos_unidades_documentales " & ex.Message
        End Try
    End Function
    Function Lista_asigna_tipos_unidad_documental(ByRef refcombo As DropDownList, ByVal id_tipo_unidad_documental As Integer) As String
        '-------------------------------------------------------------
        'Función : Lista y asigna los tipos de unidades documentales
        'Fecha : 2016-10-22
        'Ing Miguel Angel Urueta Miranda
        '-------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim nombre_tipo_unidad_documental As String = ""
            Result = Me.Retorna_nombre_tipo_unidad_documental(id_tipo_unidad_documental, nombre_tipo_unidad_documental)
            If Result <> "YES" Then
                Lista_asigna_tipos_unidad_documental = "Funcion Lista_asigna_tipos_unidad_documental  " & Result
                Exit Function
            End If
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "select NOMBRE_TIPO_UNIDAD_DOCUMENTAL  " & _
                    " from TIPOS_UNIDAD_DOCUMENTAL  "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_asigna_tipos_unidad_documental = "Funcion  lista_tipos_unidades_documentales " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                refcombo.Text = nombre_tipo_unidad_documental
                Lista_asigna_tipos_unidad_documental = "YES"
                Exit Function
            Else
                Lista_asigna_tipos_unidad_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_asigna_tipos_unidad_documental = "Inconsistencia general función Lista_asigna_tipos_unidad_documental " & ex.Message
        End Try
    End Function
    Function Lista_asigna_tipos_unidad_expedientes(ByRef refcombo As DropDownList,
                                                   ByVal id_tipo_unidad_expediente As Integer) As String
        '-----------------------------------------------------------------
        'Fucion : Lista tipo unidades de conservacion para expedientes
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-08-29
        '------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim nombre_tipo_unidad_conservacion As String = ""
            Dim ref_Class_tipo_unidad_conservacion As New Class_tipo_unidad_conservacion
            Result = ref_Class_tipo_unidad_conservacion.Retorna_nombre_tipo_unidad_conservacion_expediente(id_tipo_unidad_expediente,
                                                                                                           nombre_tipo_unidad_conservacion)
            If Result <> "YES" Then
                Lista_asigna_tipos_unidad_expedientes = "Funcion tipo_unidad_conservacion  " & Result
                Exit Function
            End If
            refcombo.Items.Clear()
            Dim Parametro_Consulta As String = "select NOMBRE_TIPO_UNIDAD  " &
                " from tipo_unidad_conservacion where " &
                " TIPO_UNIDAD=2"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_asigna_tipos_unidad_expedientes = "Funcion tipo_unidad_conservacion  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                refcombo.Text = nombre_tipo_unidad_conservacion
                Lista_asigna_tipos_unidad_expedientes = "YES"
                Exit Function
            Else
                Lista_asigna_tipos_unidad_expedientes = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_asigna_tipos_unidad_expedientes = "Inconsistencia general función Lista_asigna_tipos_unidad_expedientes " & ex.Message
        End Try
    End Function
    Function Lista_tipos_unidades_conservacion_expedientes(ByRef refcombo As DropDownList) As String
        '-----------------------------------------------------------------
        'Fucion : Lista tipo unidades de conservacion para expedientes
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2016-08-29
        '------------------------------------------------------------------
        Try
            refcombo.Items.Clear()
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "select NOMBRE_TIPO_UNIDAD  " & _
                " from tipo_unidad_conservacion where " & _
                " TIPO_UNIDAD=2"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_unidad_conservacion")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_tipos_unidades_conservacion_expedientes = "Funcion tipo_unidad_conservacion  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                'refcombo.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                Lista_tipos_unidades_conservacion_expedientes = "YES"
                Exit Function
            Else
                Lista_tipos_unidades_conservacion_expedientes = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_tipos_unidades_conservacion_expedientes = "Inconsistencia general función Lista_tipos_unidades_conservacion_expedientes " & ex.Message
        End Try
    End Function

    Function Activa_registrar_unidad_conservacion(ByRef DropDownListorganigrama As DropDownList, _
                                                  ByRef DropDownList_tipo_unidad_contenedora As DropDownList, _
                                                  ByRef DropDownListArea As DropDownList, _
                                                  ByRef DropDownList_instrumento As DropDownList, _
                                                  ByRef TextBox_ayuda_conetedora As TextBox, _
                                                  ByRef update As UpdatePanel) As String
        Try
            Dim Result As String = ""
            '----------------------------------------------
            'Lista organigramas empresa
            '----------------------------------------------
            Dim Refclas_organigrama As New Class_registro_organigrama
            Result = Refclas_organigrama.Listar_Organigramas_Empresa_Combo_Items(HttpContext.Current.Session.Item("ID_EMPRESA"), _
                                                                                 DropDownListorganigrama, _
                                                                                 update)
            If Result <> "YES" Then
                Activa_registrar_unidad_conservacion = Result
                Exit Function
            End If
            '---------------------------------------------
            'Lista tipos unidades
            '---------------------------------------------
            Result = Me.lista_tipos_unidades_Combo(DropDownList_tipo_unidad_contenedora, 1)
            If Result <> "YES" Then
                Activa_registrar_unidad_conservacion = Result
                Exit Function
            End If
            '-----------------------------------------
            'Descripcion tipo unidad contenedora
            '-----------------------------------------
            If DropDownList_tipo_unidad_contenedora.Text <> "" Then
                Result = Me.Retorna_descripcion_tipo_unidad_conservacion(DropDownList_tipo_unidad_contenedora.Text, _
                                                                         TextBox_ayuda_conetedora.Text)
                If Result <> "YES" Then
                    Activa_registrar_unidad_conservacion = Result
                    Exit Function
                End If
            End If
            '-----------------------------------------
            'Retorna area organigrama
            '-----------------------------------------
            Dim Refclas_GestionDocumental As New ClassGestionDocumental
            Dim id_organigrama As Integer = 0
            If Not DropDownListorganigrama.SelectedItem Is Nothing Then
                id_organigrama = DropDownListorganigrama.SelectedItem.Value
            End If
            If id_organigrama <> 0 Then
                '-----------------------------------------------
                'Seleccion tipo lista areas organigrama
                '-----------------------------------------------
                If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
                    Result = Refclas_GestionDocumental.Lista_AreasDep_Organigrama_Series_Items(id_organigrama, DropDownListArea)
                    If Result <> "YES" Then
                        Activa_registrar_unidad_conservacion = Result
                        Exit Function
                    End If
                Else
                    Result = Refclas_GestionDocumental.lista_areas_permitidas_usuario_gestion_organigrama_items(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                                          id_organigrama, DropDownListArea)
                    If Result <> "YES" Then
                        Activa_registrar_unidad_conservacion = Result
                        Exit Function
                    End If
                End If
            End If
            '----------------------------------------------
            'Lista instrumentos activos 
            '----------------------------------------------
            Dim Refclas_instrumentos As New ClassGaGestionInstrumento
            If id_organigrama <> 0 Then
                Result = Refclas_instrumentos.Lista_instrumentos_archivisticos_activos(id_organigrama, _
                                                                                DropDownList_instrumento, _
                                                                                update)
                If Result <> "YES" Then
                    Activa_registrar_unidad_conservacion = Result
                    Exit Function
                End If
            End If
            Activa_registrar_unidad_conservacion = "YES"
        Catch ex As Exception
            Activa_registrar_unidad_conservacion = "Inconsistencia general función Activa_registrar_unidad_conservacion " & ex.Message
        End Try
    End Function

    Function Seleccion_organigrama_unidad_conservacion( _
                                   ByVal id_organigrama As Integer, _
                                   ByRef DropDownListArea As DropDownList, _
                                   ByRef DropDownListSerie As DropDownList, _
                                   ByRef DropDownListSubserie As DropDownList, _
                                   ByRef DropDownList_instrumento As DropDownList, _
                                   ByRef update_panel_controles As UpdatePanel) As String
        Try
            
            Dim Refclas_gestion_Documental As New ClassGestionDocumental
            Dim Refclas_gestion_instrumento As New ClassGaGestionInstrumento
            Dim Result As String = ""
            DropDownListArea.Items.Clear()
            DropDownListSerie.Items.Clear()
            DropDownListSubserie.Items.Clear()
            DropDownList_instrumento.Items.Clear()
            If id_organigrama <> 0 Then
                If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
                    DropDownListArea.Items.Clear()
                    Result = Refclas_gestion_Documental.Lista_AreasDep_Organigrama_Series_Items(id_organigrama, _
                                                                                 DropDownListArea)
                    If Result <> "YES" Then
                        Seleccion_organigrama_unidad_conservacion = Result
                        Exit Function
                    End If
                Else
                    Result = Refclas_gestion_Documental.lista_areas_permitidas_usuario_gestion_organigrama_items(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                                                                                            id_organigrama, _
                                                                                            DropDownListArea)
                    If Result <> "YES" Then
                        Seleccion_organigrama_unidad_conservacion = Result
                        Exit Function
                    End If
                End If
                Result = Refclas_gestion_instrumento.Lista_instrumentos_archivisticos_activos(id_organigrama, DropDownList_instrumento, _
                                                                                            update_panel_controles)
                If Result <> "YES" Then
                    Seleccion_organigrama_unidad_conservacion = Result
                    Exit Function
                End If

            End If
            Seleccion_organigrama_unidad_conservacion = "YES"
            Exit Function
        Catch ex As Exception
            Seleccion_organigrama_unidad_conservacion = "Inconsistencia general fución Seleccion_organigrama " & ex.Message
        Finally
            update_panel_controles.Update()
        End Try
    End Function
    Function Seleccion_instrumento_consrvacion(ByVal id_area_departamento As Integer, _
                                   ByVal id_organigrama As Integer, _
                                   ByVal id_instrumento As Integer, _
                                   ByRef DropDownListSerie As DropDownList, _
                                   ByRef DropDownListSubserie As DropDownList, _
                                   ByRef update_panel_controles As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim id_empresa As Integer = 0
            Dim Refclas As New ClassAdmonEmpresa
            Dim Refclas_dos As New ClassGestionDocumental
            Dim Ref_class_series As New Class_series_documentales
            DropDownListSerie.Items.Clear()
            DropDownListSubserie.Items.Clear()
            If id_area_departamento <> 0 And id_organigrama <> 0 And id_instrumento <> 0 Then

                Result = Ref_class_series.Lista_series_relacionadas_instrumento_id_area(id_area_departamento, _
                                                                                              id_instrumento, _
                                                                                              DropDownListSerie, _
                                                                                              update_panel_controles)
                If Result <> "YES" Then
                    Seleccion_instrumento_consrvacion = Result
                    Exit Function
                End If
            End If
            Seleccion_instrumento_consrvacion = "YES"
        Catch ex As Exception
            Seleccion_instrumento_consrvacion = "Inconsistencia general función Seleccion_instrumento_consrvacion " & ex.Message
        Finally
            update_panel_controles.Update()
        End Try
    End Function

    Function Seleccion_area_departamento_unidad_conservacion(ByVal id_area_departamento As String, _
                                         ByVal id_instrumento As Integer, _
                                         ByRef DropDownListSerie As DropDownList, _
                                         ByRef DropDownListSubserie As DropDownList, _
                                         ByRef update_panel_controles As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim id_empresa As Integer = 0
            Dim id_organigrama As Integer = 0
            Dim Refclas As New ClassAdmonEmpresa
            Dim Refclas_dos As New ClassGestionDocumental
            Dim Ref_class_series As New Class_series_documentales
            DropDownListSerie.Items.Clear()
            DropDownListSubserie.Items.Clear()
            If id_area_departamento <> 0 And id_instrumento <> 0 Then
                Result = Ref_class_series.Lista_series_relacionadas_instrumento_id_area(id_area_departamento, _
                                                                                        id_instrumento, _
                                                                                        DropDownListSerie, _
                                                                                        update_panel_controles)
                If Result <> "YES" Then
                    Seleccion_area_departamento_unidad_conservacion = Result
                    Exit Function
                End If
            End If
            Seleccion_area_departamento_unidad_conservacion = "YES"
        Catch ex As Exception
            Seleccion_area_departamento_unidad_conservacion = "Inconsistencia general función Seleccion_area_departamento_unidad_conservacion " & ex.Message
        Finally
            update_panel_controles.Update()
        End Try
    End Function

    Function Seleccion_serie_documental_unidad_conservacion(ByVal id_serie As Integer, _
                                                            ByRef DropDownListSubserie As DropDownList, _
                                                            ByRef update_panel_controles As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Refclas_dos As New ClassGestionDocumental
            Dim Refclas_Trd_documental As New ClassTrdDocumental
            DropDownListSubserie.Items.Clear()
            If id_serie <> 0 Then
                Result = Refclas_dos.Listar_SubSeries_Documentales_items(id_serie, DropDownListSubserie)
                If Result <> "YES" Then
                    Seleccion_serie_documental_unidad_conservacion = Result
                    Exit Function
                End If
            End If
            Seleccion_serie_documental_unidad_conservacion = "YES"
        Catch ex As Exception
            Seleccion_serie_documental_unidad_conservacion = "Inconsistencia general función Seleccion_serie_documental_unidad_conservacion " & ex.Message
        Finally
            update_panel_controles.Update()
        End Try
    End Function

    Function Limpia_campos_unidad_conservacion(ByRef table_ref As Table) As String
        Try
            Dim tipo As String = ""
            For Each ob_TableRow As TableRow In table_ref.Rows
                For Each ob_TableCell As TableCell In ob_TableRow.Cells
                    For Each obcontrol As Object In ob_TableCell.Controls
                        Dim obcel As String = obcontrol.GetType().ToString
                        If obcel = "System.Web.UI.WebControls.TextBox" Then
                            If obcontrol.ReadOnly = False Then
                                obcontrol.text = ""
                            End If
                        End If

                        If obcel = "System.Web.UI.WebControls.DropDownList" Then
                            Dim ob_drow As DropDownList = obcontrol
                            ob_drow.Items.Clear()
                        End If
                    Next
                Next

            Next
            Limpia_campos_unidad_conservacion = "YES"
        Catch ex As Exception
            Limpia_campos_unidad_conservacion = "Inconsistencia función Limpia_campos_unidad_conservacion " & ex.Message
        End Try
    End Function
End Class
