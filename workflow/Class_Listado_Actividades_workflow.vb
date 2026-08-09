Imports GestionDocumental_Docuarchi.net.Class_config_general_service

Public Structure stru_list_actividades
    Public id_actividad As Integer
    Public nombre_actividad As String
    Public result As String
End Structure
Public Class Class_Listado_Actividades_workflow_service
    Property Error_gestion As String
    Property Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)
End Class
Public Class Class_Listado_Actividades_workflow
    Function SolicitaNombreTipoActividadGeneralWorkflow(ByVal IdActividad As Integer,
                                                        ByRef NombreActividad As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita nombre tipo actividad general workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdActividad         : Representa la identificación de la actividad workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreActividad     : Retorna el nombre de la actividad
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2012-11-16
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT AGW.TIPO_ACTIVIDAD FROM LISTADO_ACTIVIDADES_WORKFLOW AS LAW " &
            " INNER JOIN ACTIVIDADES_GENERALES_WORKFLOW AS AGW " &
            " ON (LAW.ACTIVIDADES_GENERALES_WORKFLOW_ID_ACTIVIDAD_GENERAL = " &
            " AGW.ID_ACTIVIDAD_GENERAL) WHERE LAW.ID_ACTIVIDAD= " & IdActividad
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreTipoActividadGeneralWorkflow = "Error funcion SolicitaNombreTipoActividadGeneralWorkflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreTipoActividadGeneralWorkflow = "mposible encontrar el nombre del tipo de actividad general relacionada con la identificación de la actividad (" & IdActividad & ")."
                Exit Function
            Else
                NombreActividad = Datset.Tables(0).Rows(0).Item(0).ToString()
                SolicitaNombreTipoActividadGeneralWorkflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            SolicitaNombreTipoActividadGeneralWorkflow = "Inconsistencia general funcion SolicitaNombreTipoActividadGeneralWorkflow " & ex.Message
        End Try
    End Function
    Function Solicita_id_ruta_id_actividad_workflow(ByVal id_actividad As Integer,
                                                    ByRef id_ruta As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la identificación de la ruta a la que pertenece la acti
        '          vidad workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_actividad      : Representa la identiifcación de la actividad
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_ruta  : Retorna la identificación de la ruta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-08
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select Rutas_Workflow_id_Ruta from listado_actividades_workflow where Id_Actividad=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_ruta_id_actividad_workflow = "Error listando descripción tabla actividades_generales_workflow  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_ruta_id_actividad_workflow = "Imposible encontrar la ruta del codigo de actividad (" & id_actividad & ")"
                Exit Function
            Else
                id_ruta = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_ruta_id_actividad_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_ruta_id_actividad_workflow = "Inconsistencia general función Solicita_id_ruta_id_actividad_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_tipo_actividad_general_workflow(ByVal id_actividad As Integer,
                                                      ByRef tipo_actividad As String,
                                                      ByRef id_tipo_actividad As Integer,
                                                      ByRef id_agrupacion_actividad As Integer,
                                                      ByRef nombre_tipo_actividad As String) As String
        '-------------------------------------------------------------------------
        'Función : Retorna el tipo de actividad según el parametro enviado
        'es de id actividad
        'Fecha : 2017-09-18
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select agw.Tipo_Actividad,agw.Id_Actividad_General,agw.Agrupacion_actividad,agw.Nombre_tipo_actividad from listado_actividades_workflow as law " &
               " inner join actividades_generales_workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General) where law.Id_Actividad=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipo_actividad_general_workflow = "Error listando descripción tabla actividades_generales_workflow  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipo_actividad_general_workflow = "Imposible encontrar el tipo de actividad del codigo de actividad " & id_actividad
                Exit Function
            Else
                tipo_actividad = Datset.Tables(0).Rows(0).Item(0)
                id_tipo_actividad = Datset.Tables(0).Rows(0).Item(1)
                id_agrupacion_actividad = Datset.Tables(0).Rows(0).Item(2)
                nombre_tipo_actividad = Datset.Tables(0).Rows(0).Item(3)
                Solicita_tipo_actividad_general_workflow = "YES"
            End If
        Catch ex As Exception
            Solicita_tipo_actividad_general_workflow = "Inconsistencia general función Solicita_tipo_actividad_general_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_id_tipo_actividad_workflow(ByVal id_actividad As Integer,
                                                 ByRef id_tipo_actividad As Integer) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select Actividades_Generales_Workflow_Id_Actividad_General from listado_actividades_workflow where  Id_Actividad=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_tipo_actividad_workflow = "Error Función Solicita_id_tipo_actividad_workflow dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_tipo_actividad_workflow = "Imposible encontrar el tipo de actividad de la actividad (" & id_actividad & ")"
                Exit Function
            Else
                id_tipo_actividad = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_tipo_actividad_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_tipo_actividad_workflow = "Inconsistencia general funcion Solicita_id_tipo_actividad_workflow " & ex.Message
        End Try
    End Function

    Function Solicita_actividad_workflow_final(ByRef id_actividad As Integer) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select Id_Actividad from listado_actividades_workflow where  actividad_final=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_actividad_workflow_final = "Error Función Solicita_actividad_workflow_final dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_actividad_workflow_final = "Imposible encontrar actvidad final en la ruta por favor contacte a su administrador"
                Exit Function
            Else
                id_actividad = Datset.Tables(0).Rows(0).Item(0)
                Solicita_actividad_workflow_final = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_actividad_workflow_final = "Inconsistencia general función Solicita_actividad_workflow_final " & ex.Message
        End Try
    End Function
    Function Retorna_actividad_grupo_workflow(ByVal id_grupo As Integer,
                                              ByRef id_actividad As Integer,
                                              ByRef nombre_actividad As String) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT law.id_Actividad,law.Nombre_Actividad FROM grupos_workflow gw" &
            " inner join listado_actividades_workflow as law on " &
            " (gw.id_Actividad=law.Id_Actividad) " &
            " WHERE Id_Grupo='" & id_grupo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                id_actividad = "0"
                Retorna_actividad_grupo_workflow = "Función Retorna_actividad_grupo_workflow dice error de conexion o consultando " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_actividad_grupo_workflow = "Imposible encontrar el grupo para el usuario workflow para el usuario de gestión contacte al administrador para crear la relación"
                Exit Function
            Else
                Dim nombre_usuario As String = ""
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_actividad = 0
                Else
                    id_actividad = Datset.Tables(0).Rows(0).Item(0)
                End If
                nombre_actividad = Datset.Tables(0).Rows(0).Item(1)
                If id_actividad = 0 Then
                    Retorna_actividad_grupo_workflow = "El usuario workflow " & nombre_usuario & " el grupo nos esta relacionado a una actividad "
                    Exit Function
                Else
                    Retorna_actividad_grupo_workflow = "YES"
                    Exit Function
                End If
                Retorna_actividad_grupo_workflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_actividad_grupo_workflow = "Inconsistencia función Retorna_actividad_grupo_workflow " & ex.Message
        End Try
    End Function
    Function Retorna_Nombre_Actividad_id_actividad(ByVal id_actividad As Integer,
                                                   ByRef nombre_actividad As String) As String
        '---------------------------------------------------------------------------
        'Función : Retorna nombre actividad con el parametro id actividad
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-09-19
        '---------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select nombre_actividad from listado_actividades_workflow as law " &
              "  where law.Id_Actividad=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_Nombre_Actividad_id_actividad = "Error de conexión función Retorna_Nombre_Actividad_id_actividad  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Nombre_Actividad_id_actividad = "Imposible encontrar el nombre de actividad del codigo de actividad " & id_actividad
                Exit Function
            Else
                nombre_actividad = Datset.Tables(0).Rows(0).Item(0)
                Retorna_Nombre_Actividad_id_actividad = "YES"
            End If
        Catch ex As Exception
            Retorna_Nombre_Actividad_id_actividad = "Inconsistencia general función Retorna_Nombre_Actividad_id_actividad " & ex.Message
        End Try
    End Function
    Function Solicita_listado_actividades_ruta(ByVal id_ruta As Integer,
                                               ByVal tipo_consulta As Integer,
                                               ByVal valor_consulta As String,
                                               ByRef grediview As GridView,
                                               ByRef reflabel As Label,
                                               ByRef hideselecion As HtmlInputHidden,
                                               ByRef update As UpdatePanel) As String
        Try
            Dim Sql_consulta As String = ""
            If tipo_consulta = 1 Then
                Sql_consulta = "SELECT ID_ACTIVIDAD,NOMBRE_ACTIVIDAD as GRUPO,DESCRIPCION_ACTIVIDAD AS DESCRIPCION FROM LISTADO_ACTIVIDADES_WORKFLOW " &
                " WHERE RUTAS_WORKFLOW_ID_RUTA=" & id_ruta & " order by NOMBRE_ACTIVIDAD"
            Else
                Sql_consulta = "SELECT ID_ACTIVIDAD,NOMBRE_ACTIVIDAD as GRUPO,DESCRIPCION_ACTIVIDAD AS DESCRIPCION FROM LISTADO_ACTIVIDADES_WORKFLOW " &
                " WHERE (NOMBRE_ACTIVIDAD like '%" & valor_consulta & "%'" &
                " or NOMBRE_ACTIVIDAD like '%" & valor_consulta & "%'" &
                " or DESCRIPCION_ACTIVIDAD like '%" & valor_consulta & "%'" &
                ") and  RUTAS_WORKFLOW_ID_RUTA=" & id_ruta & " order by NOMBRE_ACTIVIDAD"
            End If
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_listado_actividades_ruta = "Error listando descripción tabla listado_actividades_workflow  " & Result
                Exit Function
            End If
            Datset.Tables(0).Columns.Add("DESTINO", GetType(String))
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "0 grupo(s) "
                grediview.DataSource = Nothing
                hideselecion.Value = ""
                grediview.DataBind()
                update.Update()
                Solicita_listado_actividades_ruta = "YES"
                Exit Function
            Else
                reflabel.Text = Datset.Tables(0).Rows.Count & " grupo(s) "
                grediview.DataSource = Datset
                hideselecion.Value = ""
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim divhtml_ As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-share-all")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_envio_actividad_tarea(event,this);")
                    ahtml.Attributes.Add("title", "Enviar a (" & grediview.Rows(i).Cells(2).Text.ToString() & ")")
                    ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn bg-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_detalle_actividad(event,this);")
                    ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("id_actividad_destino", grediview.Rows(i).Cells(1).Text.ToString())
                    ihtml.Attributes.Add("class", "fad fa-user-friends")
                    ahtml.Attributes.Add("title", "Actividad de grupo de usuarios")
                    ahtml.Controls.Add(ihtml)
                    divhtml_.Controls.Add(ahtml)
                    divhtml_.Style.Add("display", "inline-flex")
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count).Controls.Add(divhtml)
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml_)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 2
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Solicita_listado_actividades_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_listado_actividades_ruta = "Inconsistencia general función Solicita_listado_actividades_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_class_actividades_workflow_ruta(ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Asigna la estructura de las actividades de ala ruta workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_service_ilist_drowlist  : Retorna la estructura generica de las activdades de ruta
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim sql_consulta As String = "SELECT ID_ACTIVIDAD,NOMBRE_ACTIVIDAD  " &
                " FROM LISTADO_ACTIVIDADES_WORKFLOW " &
                " order by NOMBRE_ACTIVIDAD"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_class_actividades_workflow_ruta = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_class_actividades_workflow_ruta = "YES"
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                Class_service_ilist_drowlist.Add(Item)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.value_campo = Datset.Tables(0).Rows(i).Item(1)
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0)
                    Class_service_ilist_drowlist.Add(Item)
                Next
                Solicita_class_actividades_workflow_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_class_actividades_workflow_ruta = "Inconistencia general función  Solicita_class_actividades_workflow_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_class_actividades_workflow_ruta_default_actividad_usuario(ByVal optio_blank As Integer,
                                                                                ByVal id_actividad_workflow As Integer,
                                                                                ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Retorna la estructura de la actividad a la que perttecene un usuario workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_actividad_workflow         : Indentificador de la actividad workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Class_service_ilist_drowlist  : Retorna la estructura generica de las activdades 
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-24
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim sql_consulta As String = "SELECT ID_ACTIVIDAD,NOMBRE_ACTIVIDAD  " &
                " FROM LISTADO_ACTIVIDADES_WORKFLOW " &
                " where ID_ACTIVIDAD=" & id_actividad_workflow &
                " order by NOMBRE_ACTIVIDAD"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_class_actividades_workflow_ruta_default_actividad_usuario = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_class_actividades_workflow_ruta_default_actividad_usuario = "YES"
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                If optio_blank = 1 Then
                    Class_service_ilist_drowlist.Add(Item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.value_campo = Datset.Tables(0).Rows(i).Item(1)
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0)
                    Class_service_ilist_drowlist.Add(Item)
                Next
                Solicita_class_actividades_workflow_ruta_default_actividad_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_class_actividades_workflow_ruta_default_actividad_usuario = "Inconistencia general función  Solicita_class_actividades_workflow_ruta_default_actividad_usuario " & ex.Message
        End Try
    End Function
    Function Lista_actividades_workflow_ruta(ByVal inserta_null As Integer,
                                             ByRef stru_list_actividades As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista actividades workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null           : Determina si inserta la primera fila vacia
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_list_actividades  : Retorna la estructura de as actividades
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ID_ACTIVIDAD,NOMBRE_ACTIVIDAD FROM LISTADO_ACTIVIDADES_WORKFLOW "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_actividades_workflow_ruta = "Error Consultando en tabla " & "LISTADO_ACTIVIDADES_WORKFLOW" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_actividades_workflow_ruta = "YES"
                Exit Function
            Else
                If inserta_null = 1 Then
                    Dim item As New stru_list_actividades
                    item.id_actividad = -1
                    item.nombre_actividad = ""
                    item.result = "YES"
                    stru_list_actividades.Add(item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim item As New stru_list_actividades
                    item.id_actividad = Datset.Tables(0).Rows(i).Item(0)
                    item.nombre_actividad = Datset.Tables(0).Rows(i).Item(1)
                    item.result = "YES"
                    stru_list_actividades.Add(item)
                Next
                Lista_actividades_workflow_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_actividades_workflow_ruta = "Inconsistencia general funcio Lista_actividades_workflow_ruta " & ex.Message
        End Try
    End Function

    Function Lista_Actividades_Combo_Duplex(ByRef Comb As DropDownList) As String
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ID_ACTIVIDAD,NOMBRE_ACTIVIDAD FROM LISTADO_ACTIVIDADES_WORKFLOW " &
            " WHERE RUTAS_WORKFLOW_ID_RUTA=" & HttpContext.Current.Session("Id_Ruta_Workflow")
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_Actividades_Combo_Duplex = "Error Consultando en tabla " & "LISTADO_ACTIVIDADES_WORKFLOW" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_Actividades_Combo_Duplex = "YES"
                Exit Function
            Else
                Comb.Items.Clear()
                Dim prob As String = ""
                Comb.Items.Add("")
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    Comb.Items.Add(Datset.Tables(0).Rows(i).Item(1).ToString)
                Next
                Lista_Actividades_Combo_Duplex = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_Actividades_Combo_Duplex = "Error general funcion Lista_Actividades_Combo " & ex.Message
        End Try
    End Function
    Function Obtener_Id_Actividad(ByRef ID_ACTIVIDAD As String,
                                  ByVal Nombre_Actividad As String) As String

        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ID_ACTIVIDAD FROM LISTADO_ACTIVIDADES_WORKFLOW " &
            " WHERE NOMBRE_ACTIVIDAD='" & Nombre_Actividad & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                ID_ACTIVIDAD = "0"
                Obtener_Id_Actividad = "Error Consultando en tabla " & "LISTADO_ACTIVIDADES_WORKFLOW" & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then

                ID_ACTIVIDAD = "0"
                Obtener_Id_Actividad = "YES"
                Exit Function
            Else
                ID_ACTIVIDAD = Datset.Tables(0).Rows(0).Item(0).ToString
                Obtener_Id_Actividad = "YES"
            End If
        Catch ex As Exception
            Obtener_Id_Actividad = "Inconsistencia general función Obtener_Id_Actividad " & ex.Message
        End Try
    End Function
    Function Solicita_estado_envio_correo_actividad(ByVal id_actividad As Integer,
                                                    ByRef estado_envio_correo As Integer) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select estado_envio_correo from listado_actividades_workflow where  Id_Actividad=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("listado_actividades_workflow")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_envio_correo_actividad = "Error Función Solicita_estado_envio_correo_actividad dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_envio_correo_actividad = "Imposible encontrar el estado de envío de correo de la actividad con id (" & id_actividad & ")"
                Exit Function
            Else
                estado_envio_correo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_envio_correo_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_envio_correo_actividad = "Inconsistencia general función Solicita_estado_envio_correo_actividad " & ex.Message
        End Try
    End Function
    Function Solicita_actividades_ruta(ByVal id_actividad As Integer,
                                       ByRef drop_list As DropDownList) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta As String = "SELECT ID_ACTIVIDAD,NOMBRE_ACTIVIDAD FROM LISTADO_ACTIVIDADES_WORKFLOW "
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("LISTADO_ACTIVIDADES_WORKFLOW")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_actividades_ruta = "Función Solicita_actividades_ruta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Solicita_actividades_ruta = "YES"
                Exit Function
            Else
                drop_list.Items.Clear()
                Dim ilist As New ListItem
                ilist.Value = 0
                ilist.Text = ""
                drop_list.Items.Add(ilist)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1) & " (GRUPO)"
                    drop_list.Items.Add(ilist)
                Next
                If id_actividad <> -1 Then
                    For i As Integer = 0 To drop_list.Items.Count - 1
                        If drop_list.Items(i).Value = id_actividad Then
                            drop_list.SelectedIndex = i
                            Exit For
                        End If
                    Next
                End If
                Solicita_actividades_ruta = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_actividades_ruta = "Inconsistencia general función Solicita_actividades_ruta " & ex.Message
        End Try
    End Function
End Class
