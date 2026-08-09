Imports GestionDocumental_Docuarchi.net.Class_config_general_service

Public Structure stru_actividad_usuario_flujo
    Dim id_actividad_workflow_flujo As Integer
    Dim id_usuario_worlflow_flujo As Integer
End Structure
Public Structure struregistro_actividaes_flujos_trabajo
    Dim ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO As Integer
    Dim wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO As Integer
    Dim listado_actividades_workflow_Id_Actividad As Integer
    Dim IDENTI_GRAFICA_ACTIVIDAD As Integer
    Dim FECHA_REGISTRO As String
    Dim ESTADO_ACTIVIDAD As Integer
    Dim ID_TIPO_ACTIVIDAD As Integer
    Dim ID_USUARIO_WORKFLOW As Integer
    Dim TIPO_ABIERTA_CERRADA_ACTIVIDAD As Integer
    Dim ACTIVIDAD_INICIO As Integer
    Dim ACTIVIDAD_FINAL As Integer
    Dim DESCRIPCION_TAREA_ACTIVIDAD As String
End Structure
Public Class Class_list_realcion_activida_flujo
    Property Error_gestion As String
    Property Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)
End Class
Public Class Class_wf_registro_actividaes_flujos_trabajo

    Function Actualiza_descripcion_actividad_flujo_trabajo(ByVal id_actividad_flujo As Integer,
                                                           ByVal descripcion As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Actualiza la descripción de la actividad del flujo de trabajo
        '          para que se liste en la lista de tareas workflow
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_actividad_flujo   : Representa la idneitcación de la tarea dentro de la
        '                       ruta y el flujo de trabajo
        'descripcion          : Representa la descripcion de la actividad
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        ' 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-10
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ref_descripcion As Object = "Null"
            If descripcion <> "" Then
                ref_descripcion = "'" & descripcion & "'"
            End If
            Dim sql_update As String = "update wf_registro_actividaes_flujos_trabajo set DESCRIPCION_TAREA_ACTIVIDAD=" & ref_descripcion &
                                       " where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad_flujo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ref.SELECTION_INSERT_COMMAND(sql_update)
            Actualiza_descripcion_actividad_flujo_trabajo = Result
            Exit Function
        Catch ex As Exception
            Actualiza_descripcion_actividad_flujo_trabajo = "Inconsistencia general funcion Actualiza_descripcion_actividad_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_actividad_usuario_flujo_trabajo(ByVal id_actividad_workflow As Integer,
                                                      ByVal id_flujo As Integer,
                                                      ByRef id_actividad_flujo As Integer) As String
        '------------------------------------------------------------------------------
        'Funcion : Retora actividad flujo trabajo usuario workflow con la relación
        'de la actividad workflow relacionada a la actividad de flujo de trabajo
        ' 
        '-------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'id_actividad_workflow  : Representa la idneitifcación de la actividad de ruta
        'id_flujo               : Flujo de trabajo al que pertenece la tarea
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------
        'id_actividad_flujo  : Retorna identificación de la actividad de flujo
        '-------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------
        'Fecha                 : 2024-12-24
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO FROM  wf_registro_actividaes_flujos_trabajo as wraft " &
                " where wraft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo & " and wraft.listado_actividades_workflow_Id_Actividad=" & id_actividad_workflow &
                " and wraft.ACTIVIDAD_INICIO=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_actividad_usuario_flujo_trabajo = "Error funcion Solicita_actividades_workflow_flujo_inicio" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_actividad_flujo = 0
                Solicita_actividad_usuario_flujo_trabajo = "Imposible encontar actividad flujo trabajo relacionada a la actividad  (" & id_actividad_workflow & ") del usuario workflow logueado, relacione al usuario logueado a una actividad de inicio del flujo de trabajo"
                Exit Function
            Else
                id_actividad_flujo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_actividad_usuario_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_actividad_usuario_flujo_trabajo = "Inconsistencia general funcio Solicita_actividad_usuario_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_actividades_usuario_flujo_trabajo(ByVal option_blank As Integer,
                                                        ByVal id_actividad_workflow As Integer,
                                                        ByVal id_flujo As Integer,
                                                        ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista actividades workflow relacionadas a la actividad workflow
        'relacionada a la actividad de flujo de trabajo, establecida como actividad
        'de inicio
        '
        ' 
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'opton_blank           : Determina si inserta la primera fila vacia
        'id_actividad_workflow : Indentificación de actividad de inicio
        'id_flujo              : Flujo de trabajo al que pertenece la actividad
        '                        workflow
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'drowp_list  : Retorna la lista de las actividades en formato  DropDownList
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-12-24
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario,wraft.ID_USUARIO_WORKFLOW FROM  wf_registro_actividaes_flujos_trabajo as wraft " &
                " inner join LISTADO_ACTIVIDADES_WORKFLOW as law on (wraft.listado_actividades_workflow_Id_Actividad=law.Id_Actividad)" &
                " left outer join usuario_workflow as uw on (uw.idU_suario=wraft.ID_USUARIO_WORKFLOW)" &
                " where wraft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo & " and wraft.ACTIVIDAD_INICIO=1 and wraft.listado_actividades_workflow_Id_Actividad=" & id_actividad_workflow & " order by " &
                "  wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_actividades_usuario_flujo_trabajo = "Error funcion Solicita_actividades_usuario_flujo_trabajo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_actividades_usuario_flujo_trabajo = "YES"
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                If option_blank = 1 Then
                    Class_service_ilist_drowlist.Add(Item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0)
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        Item.value_campo = Datset.Tables(0).Rows(i).Item(1)
                    Else
                        Item.value_campo = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    End If
                    Class_service_ilist_drowlist.Add(Item)
                Next
                Solicita_actividades_usuario_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_actividades_usuario_flujo_trabajo = "Inconsistencia general funcio Solicita_actividades_usuario_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_actividades_workflow_flujo_inicio(ByVal opton_blank As Integer,
                                                        ByVal id_flujo As Integer,
                                                        ByRef Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista actividades relacionadas a un flujo de trabajo de inicio
        ' de flujo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null           : Determina si inserta la primera fila vacia
        'id_flujo               : Flujo de trabajo al que pertenece la tarea
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'drowp_list  : Retorna la lista de las actividades en formato  DropDownList
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-12-06
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario,wraft.ID_USUARIO_WORKFLOW FROM  wf_registro_actividaes_flujos_trabajo as wraft " &
                " inner join LISTADO_ACTIVIDADES_WORKFLOW as law on (wraft.listado_actividades_workflow_Id_Actividad=law.Id_Actividad)" &
                " left outer join usuario_workflow as uw on (uw.idU_suario=wraft.ID_USUARIO_WORKFLOW)" &
                " where wraft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo & " and wraft.ACTIVIDAD_INICIO=1 order by " &
                "  wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_actividades_workflow_flujo_inicio = "Error funcion Solicita_actividades_workflow_flujo_inicio Consultando en tabla " & "wf_registro_actividaes_flujos_trabajo" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_actividades_workflow_flujo_inicio = "YES"
                Exit Function
            Else
                Dim Item As New Class_service_ilist_drowlist
                Item.value_campo = ""
                Item.id_value = "0"
                If opton_blank = 1 Then
                    Class_service_ilist_drowlist.Add(Item)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Item = New Class_service_ilist_drowlist
                    Item.id_value = Datset.Tables(0).Rows(i).Item(0)
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        Item.value_campo = Datset.Tables(0).Rows(i).Item(1)
                    Else
                        Item.value_campo = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    End If
                    Class_service_ilist_drowlist.Add(Item)
                Next
                Solicita_actividades_workflow_flujo_inicio = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_actividades_workflow_flujo_inicio = "Inconsistencia general funcio Solicita_actividades_workflow_flujo_inicio " & ex.Message
        End Try
    End Function
    Function Lista_actividades_workflow_flujo_drowlist(ByVal inserta_null As Integer,
                                                       ByVal id_flujo As Integer,
                                                       ByRef drowp_list As DropDownList,
                                                       ByRef update As UpdatePanel) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista actividades relacionadas a un flujo de trabajo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null           : Determina si inserta la primera fila vacia
        'id_flujo               : Flujo de trabajo al que pertenece la tarea
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'drowp_list  : Retorna la lista de las actividades en formato  DropDownList
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-23
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario,wraft.ID_USUARIO_WORKFLOW FROM  wf_registro_actividaes_flujos_trabajo as wraft " &
                " inner join LISTADO_ACTIVIDADES_WORKFLOW as law on (wraft.listado_actividades_workflow_Id_Actividad=law.Id_Actividad)" &
                " left outer join usuario_workflow as uw on (uw.idU_suario=wraft.ID_USUARIO_WORKFLOW)" &
                " where wraft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo & " order by " &
                "  wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_actividades_workflow_flujo_drowlist = "Error funcion Lista_actividades_workflow_flujo_drowlist Consultando en tabla " & "wf_registro_actividaes_flujos_trabajo" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drowp_list.Items.Clear()
                update.Update()
                Lista_actividades_workflow_flujo_drowlist = "YES"
                Exit Function
            Else
                update.Update()
                drowp_list.Items.Clear()
                Dim ilist As New ListItem
                If inserta_null = 1 Then
                    ilist.Value = -1
                    ilist.Text = ""
                    drowp_list.Items.Add(ilist)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    Else
                        ilist.Text = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    End If
                    drowp_list.Items.Add(ilist)
                Next
                Lista_actividades_workflow_flujo_drowlist = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_actividades_workflow_flujo_drowlist = "Inconsistencia general funcio Lista_actividades_workflow_flujo_drowlist " & ex.Message
        End Try
    End Function
    Function Lista_actividades_workflow_flujo_drowlist_tipo(ByVal inserta_null As Integer,
                                                            ByVal id_flujo As Integer,
                                                            ByVal id_tipo_actividad_flujo As Integer,
                                                            ByRef drowp_list As DropDownList,
                                                            ByRef update As UpdatePanel) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista actividades relacionadas a un flujo de trabajo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null            : Determina si inserta la primera fila vacia
        'id_flujo                : Flujo de trabajo al que pertenece la tarea
        'id_tipo_actividad_flujo : Determina el tipo de actividad flujo
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'drowp_list  : Retorna la lista de las actividades en formato  DropDownList
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-23
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario,wraft.ID_USUARIO_WORKFLOW FROM  wf_registro_actividaes_flujos_trabajo as wraft " &
                " inner join LISTADO_ACTIVIDADES_WORKFLOW as law on (wraft.listado_actividades_workflow_Id_Actividad=law.Id_Actividad)" &
                " left outer join usuario_workflow as uw on (uw.idU_suario=wraft.ID_USUARIO_WORKFLOW)" &
                " where wraft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo & " and ID_TIPO_ACTIVIDAD=" & id_tipo_actividad_flujo & " order by " &
                "  wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_actividades_workflow_flujo_drowlist_tipo = "Error funcion Lista_actividades_workflow_flujo_drowlist_tipo Consultando en tabla " & "wf_registro_actividaes_flujos_trabajo" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drowp_list.Items.Clear()
                update.Update()
                Lista_actividades_workflow_flujo_drowlist_tipo = "YES"
                Exit Function
            Else
                update.Update()
                drowp_list.Items.Clear()
                Dim ilist As New ListItem
                If inserta_null = 1 Then
                    ilist.Value = -1
                    ilist.Text = ""
                    drowp_list.Items.Add(ilist)
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    Else
                        ilist.Text = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    End If
                    drowp_list.Items.Add(ilist)
                Next
                Lista_actividades_workflow_flujo_drowlist_tipo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_actividades_workflow_flujo_drowlist_tipo = "Inconsistencia general funcio Lista_actividades_workflow_flujo_drowlist_tipo " & ex.Message
        End Try
    End Function
    Function Lista_actividades_workflow_flujo_tipo(ByVal inserta_null As Integer,
                                                   ByVal id_flujo As Integer,
                                                   ByVal id_tipo_actividad_flujo As Integer,
                                                   ByRef stru_list_actividades As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista actividades relacionadas a un flujo de trabajo de un tipo
        '          de actividad especifica
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null            : Determina si inserta la primera fila vacia
        'id_flujo                : Flujo de trabajo al que pertenece la tarea
        'id_tipo_actividad_flujo : Determina el tipo de actividad flujo
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_list_actividades  : Retorna la estructura de las actividades
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario,wraft.ID_USUARIO_WORKFLOW FROM  wf_registro_actividaes_flujos_trabajo as wraft " &
                " inner join LISTADO_ACTIVIDADES_WORKFLOW as law on (wraft.listado_actividades_workflow_Id_Actividad=law.Id_Actividad)" &
                " left outer join usuario_workflow as uw on (uw.idU_suario=wraft.ID_USUARIO_WORKFLOW)" &
                " where wraft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo & " and ID_TIPO_ACTIVIDAD=" & id_tipo_actividad_flujo & " order by " &
                "  wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_actividades_workflow_flujo_tipo = "Error Consultando en tabla " & "wf_registro_actividaes_flujos_trabajo" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_actividades_workflow_flujo_tipo = "YES"
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
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        item.id_actividad = Datset.Tables(0).Rows(i).Item(0)
                        item.nombre_actividad = Datset.Tables(0).Rows(i).Item(1)
                    Else
                        item.id_actividad = Datset.Tables(0).Rows(i).Item(0)
                        item.nombre_actividad = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    End If
                    item.result = "YES"
                    stru_list_actividades.Add(item)
                Next
                Lista_actividades_workflow_flujo_tipo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_actividades_workflow_flujo_tipo = "Inconsistencia general funcio Lista_actividades_workflow_flujo_tipo " & ex.Message
        End Try
    End Function
    Function Lista_actividades_workflow_flujo(ByVal inserta_null As Integer,
                                              ByVal id_flujo As Integer,
                                              ByRef stru_list_actividades As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista actividades relacionadas a un flujo de trabajo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'inserta_null           : Determina si inserta la primera fila vacia
        'id_flujo               : Flujo de trabajo al que pertenece la tarea
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_list_actividades  : Retorna la estructura de las actividades
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario,wraft.ID_USUARIO_WORKFLOW FROM  wf_registro_actividaes_flujos_trabajo as wraft " &
                " inner join LISTADO_ACTIVIDADES_WORKFLOW as law on (wraft.listado_actividades_workflow_Id_Actividad=law.Id_Actividad)" &
                " left outer join usuario_workflow as uw on (uw.idU_suario=wraft.ID_USUARIO_WORKFLOW)" &
                " where wraft.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo & " order by " &
                "  wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,law.Nombre_Actividad,uw.Nombre_Usuario "
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_actividades_workflow_flujo = "Error Consultando en tabla " & "wf_registro_actividaes_flujos_trabajo" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_actividades_workflow_flujo = "YES"
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
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        item.id_actividad = Datset.Tables(0).Rows(i).Item(0)
                        item.nombre_actividad = Datset.Tables(0).Rows(i).Item(1)
                    Else
                        item.id_actividad = Datset.Tables(0).Rows(i).Item(0)
                        item.nombre_actividad = Datset.Tables(0).Rows(i).Item(1) & "(" & Datset.Tables(0).Rows(i).Item(2) & ")"
                    End If
                    item.result = "YES"
                    stru_list_actividades.Add(item)
                Next
                Lista_actividades_workflow_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_actividades_workflow_flujo = "Inconsistencia general funcio Lista_actividades_workflow_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_actividad_flujo_trabajo(ByVal id_actvidad_flujo As Integer,
                                                         ByRef struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de una actividad dentro de un flujo de
        '          trabajo
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_flujo_trabajo      : Respresenta la identificación de la actividad dentro
        '                        del flujo de trabajo
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'struregistro_actividaes_flujos_trabajo  : lista estructura de la actividad
        ' 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-17
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO, " &
                "listado_actividades_workflow_Id_Actividad,IDENTI_GRAFICA_ACTIVIDAD,FECHA_REGISTRO,ESTADO_ACTIVIDAD," &
                "ID_TIPO_ACTIVIDAD,ID_USUARIO_WORKFLOW,TIPO_ABIERTA_CERRADA_ACTIVIDAD,ACTIVIDAD_INICIO,ACTIVIDAD_FINAL,DESCRIPCION_TAREA_ACTIVIDAD " &
                " from wf_registro_actividaes_flujos_trabajo " &
                " where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_actvidad_flujo
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_actividad_flujo_trabajo = "Función Solicita_estructura_actividad_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_actividad_flujo_trabajo = "Imposible encontrar la estructura de la actividad de flujo (" & id_actvidad_flujo & ")"
                Exit Function
            Else
                struregistro_actividaes_flujos_trabajo.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = Datset.Tables(0).Rows(0).Item(0)
                struregistro_actividaes_flujos_trabajo.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = Datset.Tables(0).Rows(0).Item(1)
                struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad = Datset.Tables(0).Rows(0).Item(2)
                If Datset.Tables(0).Rows(0).IsNull(3) = False Then
                    struregistro_actividaes_flujos_trabajo.IDENTI_GRAFICA_ACTIVIDAD = Datset.Tables(0).Rows(0).Item(3)
                Else
                    struregistro_actividaes_flujos_trabajo.IDENTI_GRAFICA_ACTIVIDAD = 0
                End If
                struregistro_actividaes_flujos_trabajo.FECHA_REGISTRO = Datset.Tables(0).Rows(0).Item(4)
                struregistro_actividaes_flujos_trabajo.ESTADO_ACTIVIDAD = Datset.Tables(0).Rows(0).Item(5)
                struregistro_actividaes_flujos_trabajo.ID_TIPO_ACTIVIDAD = Datset.Tables(0).Rows(0).Item(6)
                If Datset.Tables(0).Rows(0).IsNull(7) = False Then
                    struregistro_actividaes_flujos_trabajo.ID_USUARIO_WORKFLOW = Datset.Tables(0).Rows(0).Item(7)
                Else
                    struregistro_actividaes_flujos_trabajo.ID_USUARIO_WORKFLOW = 0
                End If
                struregistro_actividaes_flujos_trabajo.TIPO_ABIERTA_CERRADA_ACTIVIDAD = Datset.Tables(0).Rows(0).Item(8)
                struregistro_actividaes_flujos_trabajo.ACTIVIDAD_INICIO = Datset.Tables(0).Rows(0).Item(9)
                struregistro_actividaes_flujos_trabajo.ACTIVIDAD_FINAL = Datset.Tables(0).Rows(0).Item(10)
                If Datset.Tables(0).Rows(0).IsNull(11) = True Then
                    struregistro_actividaes_flujos_trabajo.DESCRIPCION_TAREA_ACTIVIDAD = ""
                Else
                    struregistro_actividaes_flujos_trabajo.DESCRIPCION_TAREA_ACTIVIDAD = Datset.Tables(0).Rows(0).Item(11)
                End If
                Solicita_estructura_actividad_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_actividad_flujo_trabajo = "Inconsistencia general funcion Solicita_estructura_actividad_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_lista_estructura_actividade_flujo(ByVal id_flujo_trabajo As Integer,
                                                        ByRef struregistro_actividaes_flujos_trabajo() As struregistro_actividaes_flujos_trabajo) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el listado y las estructuras de las actividades relacion
        '          -adas a un flujo de trabajo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_flujo_trabajo      : Respresenta la identificación del flujo de trabajo
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'struregistro_actividaes_flujos_trabajo  : lista estructuras actividades
        ' de un flujo de trabajo especifico
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-23
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO, " &
                "listado_actividades_workflow_Id_Actividad,IDENTI_GRAFICA_ACTIVIDAD,FECHA_REGISTRO,ESTADO_ACTIVIDAD," &
                "ID_TIPO_ACTIVIDAD,ID_USUARIO_WORKFLOW,TIPO_ABIERTA_CERRADA_ACTIVIDAD,ACTIVIDAD_INICIO,ACTIVIDAD_FINAL,DESCRIPCION_TAREA_ACTIVIDAD " &
                " where wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trabajo
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_estructura_actividade_flujo = "Función Solicita_pertenencia_usuario_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_estructura_actividade_flujo = "Imposible encontrar la actividades relacionadas al flujo de trabajo (" & id_flujo_trabajo & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve struregistro_actividaes_flujos_trabajo(i)
                    struregistro_actividaes_flujos_trabajo(i).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = Datset.Tables(0).Rows(i).Item(0)
                    struregistro_actividaes_flujos_trabajo(i).wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = Datset.Tables(0).Rows(i).Item(1)
                    struregistro_actividaes_flujos_trabajo(i).listado_actividades_workflow_Id_Actividad = Datset.Tables(0).Rows(i).Item(2)
                    struregistro_actividaes_flujos_trabajo(i).IDENTI_GRAFICA_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(3)
                    struregistro_actividaes_flujos_trabajo(i).FECHA_REGISTRO = Datset.Tables(0).Rows(i).Item(4)
                    struregistro_actividaes_flujos_trabajo(i).ESTADO_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(5)
                    struregistro_actividaes_flujos_trabajo(i).ID_TIPO_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(6)
                    If Datset.Tables(0).Rows(i).IsNull(7) = False Then
                        struregistro_actividaes_flujos_trabajo(i).ID_USUARIO_WORKFLOW = Datset.Tables(0).Rows(i).Item(7)
                    Else
                        struregistro_actividaes_flujos_trabajo(i).ID_USUARIO_WORKFLOW = 0
                    End If
                    struregistro_actividaes_flujos_trabajo(i).TIPO_ABIERTA_CERRADA_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(8)
                    struregistro_actividaes_flujos_trabajo(i).ACTIVIDAD_INICIO = Datset.Tables(0).Rows(i).Item(9)
                    struregistro_actividaes_flujos_trabajo(i).ACTIVIDAD_FINAL = Datset.Tables(0).Rows(i).Item(10)
                    If Datset.Tables(0).Rows(i).IsNull(11) = True Then
                        struregistro_actividaes_flujos_trabajo(i).DESCRIPCION_TAREA_ACTIVIDAD = ""
                    Else
                        struregistro_actividaes_flujos_trabajo(i).DESCRIPCION_TAREA_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(11)
                    End If
                Next
                Solicita_lista_estructura_actividade_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_estructura_actividade_flujo = "Inconsistencia general funcion Solicita_lista_estructura_actividade_flujo " & ex.Message
        End Try
    End Function
    'Function Solicita_lista_actividades_anterior_flujo_trabajo(ByVal id_registro_actividad_flujo As Integer,
    '                                                           ByRef _struregistro_actividaes_flujos_trabajo() As struregistro_actividaes_flujos_trabajo,
    '                                                           ByRef struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo) As String
    '    '---------------------------------------------------------------------------
    '    'Funcion : Solicita estructura actividad anterior flujo de trabajo
    '    '         
    '    '---------------------------------------------------------------------------
    '    '                           PARAMETROS  
    '    '---------------------------------------------------------------------------
    '    'id_registro_actividad_flujo As Integer  : Representa el registro de la acti
    '    '-vidad referencia del la actividad anterior
    '    '---------------------------------------------------------------------------
    '    '                           RETORNO
    '    '---------------------------------------------------------------------------
    '    'struregistro_actividaes_flujos_trabajo  : lista estructura de la actividad
    '    'anterior
    '    '---------------------------------------------------------------------------
    '    '                         CARACTERIZACIÓN
    '    '---------------------------------------------------------------------------
    '    'Fecha                 : 2023-04-23
    '    'Elabora               : Miguel Angel Urueta Miranda
    '    '----------------------------------------------------------------------------
    '    Try
    '        '----------------------------------------------
    '        'Valida la no exitencia de actividad anterior
    '        '----------------------------------------------
    '        If _struregistro_actividaes_flujos_trabajo(0).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = id_registro_actividad_flujo Then
    '            Solicita_lista_actividades_anterior_flujo_trabajo = "El sistema no registra una actividad anterior en el flujo para devolver"
    '            Exit Function
    '        End If
    '        If _struregistro_actividaes_flujos_trabajo.Length = 1 Then
    '            Solicita_lista_actividades_anterior_flujo_trabajo = "El sistema no registra una actividad anterior en el flujo para devolver"
    '            Exit Function
    '        End If
    '        Dim idex_actividad_anterior As Integer = -1
    '        For i As Integer = 0 To _struregistro_actividaes_flujos_trabajo.Length - 1
    '            If _struregistro_actividaes_flujos_trabajo(i).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = id_registro_actividad_flujo Then
    '                idex_actividad_anterior = i - 1
    '                Exit For
    '            End If
    '        Next
    '        struregistro_actividaes_flujos_trabajo.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = _struregistro_actividaes_flujos_trabajo(idex_actividad_anterior).ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO
    '        struregistro_actividaes_flujos_trabajo.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = _struregistro_actividaes_flujos_trabajo(idex_actividad_anterior).wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO
    '        struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad = _struregistro_actividaes_flujos_trabajo(idex_actividad_anterior).listado_actividades_workflow_Id_Actividad
    '        struregistro_actividaes_flujos_trabajo.IDENTI_GRAFICA_ACTIVIDAD = _struregistro_actividaes_flujos_trabajo
    '        .IDENTI_GRAFICA_ACTIVIDAD
    '        struregistro_actividaes_flujos_trabajo.FECHA_REGISTRO = struregistro_actividaes_flujos_trabajo.FECHA_REGISTRO
    '        struregistro_actividaes_flujos_trabajo(i).ESTADO_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(5)
    '        struregistro_actividaes_flujos_trabajo(i).ID_TIPO_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(6)
    '        If Datset.Tables(0).Rows(i).IsNull(7) = False Then
    '            struregistro_actividaes_flujos_trabajo(i).ID_USUARIO_WORKFLOW = Datset.Tables(0).Rows(i).Item(7)
    '        Else
    '            struregistro_actividaes_flujos_trabajo(i).ID_USUARIO_WORKFLOW = 0
    '        End If
    '        struregistro_actividaes_flujos_trabajo(i).TIPO_ABIERTA_CERRADA_ACTIVIDAD = Datset.Tables(0).Rows(i).Item(8)
    '        struregistro_actividaes_flujos_trabajo(i).ACTIVIDAD_INICIO = Datset.Tables(0).Rows(i).Item(9)
    '        struregistro_actividaes_flujos_trabajo(i).ACTIVIDAD_FINAL = Datset.Tables(0).Rows(i).Item(10)
    '    Catch ex As Exception
    '        Solicita_lista_actividades_anterior_flujo_trabajo = "Inconsistencia general funcion Solicita_lista_actividades_anterior_flujo_trabajo " & ex.Message
    '    End Try
    'End Function
    Function Solicita_id_actividad_workflow_flujo_trabajo(ByVal id_actividad_flujo As Integer,
                                                          ByRef id_actividad_workflow As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select listado_actividades_workflow_Id_Actividad from wf_registro_actividaes_flujos_trabajo " &
                " where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad_flujo
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_actividad_workflow_flujo_trabajo = "Función Solicita_pertenencia_usuario_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_actividad_workflow_flujo_trabajo = "Imposible encontrar la actividad workflow del flujo de trabajo (" & id_actividad_flujo & ")"
                Exit Function
            Else
                id_actividad_workflow = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_actividad_workflow_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_actividad_workflow_flujo_trabajo = "Inconsistencia general funcion Solicita_id_actividad_workflow_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_pertenencia_usuario_flujo_trabajo(ByVal id_actividad As Integer,
                                                        ByVal id_usuario_workflow As Integer,
                                                        ByVal id_flujo_trabajo As Integer,
                                                        ByRef Estado_pertencia As String) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "SELECT * FROM wf_registro_actividaes_flujos_trabajo " &
            "  where wf_flujos_trabajo_id_wf_flujos_trabajo=" & id_flujo_trabajo &
            " AND listado_actividades_workflow_id_actividad=" & id_actividad & " AND ID_USUARIO_WORKFLOW=" & id_usuario_workflow &
            " UNION " &
            "SELECT * FROM wf_registro_actividaes_flujos_trabajo " &
            "  where wf_flujos_trabajo_id_wf_flujos_trabajo=" & id_flujo_trabajo &
            " AND listado_actividades_workflow_id_actividad=" & id_actividad & " AND ID_USUARIO_WORKFLOW IS NULL"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_pertenencia_usuario_flujo_trabajo = "Función Solicita_pertenencia_usuario_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_pertenencia_usuario_flujo_trabajo = "YES"
                Estado_pertencia = "NO"
                Exit Function
            Else
                Estado_pertencia = "YES"
                Solicita_pertenencia_usuario_flujo_trabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_pertenencia_usuario_flujo_trabajo = "Inconsistencia general función Solicita_pertenencia_usuario_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function SolicitaIActividadesUsuarioWorkflowFlujoTrabajo(ByVal IdActividadWorkflow As Integer,
                                                             ByVal IdUsuarioWorkflow As Integer,
                                                             ByVal IdFlujoTrabajo As Integer,
                                                             ByRef stru_actividad_usuario_flujo() As stru_actividad_usuario_flujo) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita las actividades y usuarios worflow relacionados a un flujo de trabajo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdActividadWorkflow : Representa la identificación de la atividad workflow del usuario
        'IdUsuarioWorkflow   : Representa la identiicación del usuario workflow
        'IdFlujoTrabajo      : Representa la identificació del flujo de trabajo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_actividad_usuario_flujo  : Retorna la estructura de las actividades
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "SELECT ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW FROM wf_registro_actividaes_flujos_trabajo " &
            "  where wf_flujos_trabajo_id_wf_flujos_trabajo=" & IdFlujoTrabajo &
            " AND listado_actividades_workflow_id_actividad=" & IdActividadWorkflow & " AND ID_USUARIO_WORKFLOW=" & IdUsuarioWorkflow &
            " UNION " &
            "SELECT ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,ID_USUARIO_WORKFLOW FROM wf_registro_actividaes_flujos_trabajo " &
            "  where wf_flujos_trabajo_id_wf_flujos_trabajo=" & IdFlujoTrabajo &
            " AND listado_actividades_workflow_id_actividad=" & IdActividadWorkflow & " AND ID_USUARIO_WORKFLOW IS NULL"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaIActividadesUsuarioWorkflowFlujoTrabajo = "Función Solicita_id_tarea_usuario_flujo_trabajo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIActividadesUsuarioWorkflowFlujoTrabajo = "Impoaible encontrar la identificación la actividad ( " & IdActividadWorkflow & ") en el flujo (" & IdFlujoTrabajo & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_actividad_usuario_flujo(i)
                    stru_actividad_usuario_flujo(i).id_actividad_workflow_flujo = Datset.Tables(0).Rows(i).Item(0)
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        stru_actividad_usuario_flujo(i).id_usuario_worlflow_flujo = 0
                    Else
                        stru_actividad_usuario_flujo(i).id_usuario_worlflow_flujo = Datset.Tables(0).Rows(i).Item(1)
                    End If
                Next
                SolicitaIActividadesUsuarioWorkflowFlujoTrabajo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIActividadesUsuarioWorkflowFlujoTrabajo = "Inconsistencia general función SolicitaIdTareaUsuarioWorkflowFlujoTrabajo " & ex.Message
        End Try
    End Function
    Function Solicita_trareas_inicio_flujo_trabajo(ByVal id_flujo_trbajo As Integer,
                                                   ByVal id_actividad As Integer,
                                                   ByRef drop_list As DropDownList) As String
        Try
            drop_list.Items.Clear()
            Dim Parametro_Consulta As String = "SELECT ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,Nombre_Actividad, ID_USUARIO_WORKFLOW, uw.Nombre_Usuario FROM wf_registro_actividaes_flujos_trabajo as raf" &
            " inner join listado_actividades_workflow as law on (law.Id_Actividad=raf.listado_actividades_workflow_Id_Actividad)" &
            " left OUTER JOIN usuario_workflow AS uw on (uw.idU_suario=ID_USUARIO_WORKFLOW) " &
           " where wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO=" & id_flujo_trbajo & " and ACTIVIDAD_INICIO=1 order by ID_USUARIO_WORKFLOW"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_trareas_inicio_flujo_trabajo = "Función Solicita_relaciones_flujo_trabajo_tramite dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Solicita_trareas_inicio_flujo_trabajo = "YES"
                Exit Function
            Else
                drop_list.Items.Clear()
                Dim ilist As New ListItem
                ilist.Value = 0
                ilist.Text = ""
                drop_list.Items.Add(ilist)
                Dim Estado_grupo As String = ""
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    If Datset.Tables(0).Rows(i).IsNull(2) = True Then
                        ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    Else
                        If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                            ilist.Text = "No relaciono"
                        Else
                            ilist.Text = Datset.Tables(0).Rows(i).Item(3)
                        End If

                    End If
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
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
                Solicita_trareas_inicio_flujo_trabajo = "YES"
                Exit Function
            End If
            Solicita_trareas_inicio_flujo_trabajo = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_trareas_inicio_flujo_trabajo = "Inconsistencia general función Solicita_trareas_inicio_flujo_trabajo " & ex.Message
        End Try
    End Function
    Function Solicita_id_usuario_id_actividad_workflow_flujo_actividad(ByVal id_actividad_flujo_trabajo As Integer,
                                                                       ByRef id_usuario_wf As Integer,
                                                                       ByRef id_actividad As Integer) As String
        Try
            Dim Parametro_Consulta As String = "SELECT ID_USUARIO_WORKFLOW,listado_actividades_workflow_Id_Actividad FROM wf_registro_actividaes_flujos_trabajo " &
            " where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_actividad_flujo_trabajo
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_actividaes_flujos_trabajo")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_usuario_id_actividad_workflow_flujo_actividad = "Función Solicita_id_usuario_workflow_flujo_actividad dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_usuario_id_actividad_workflow_flujo_actividad = "Imposible econtrar el registro de la actividad en el flujo de trabajo con el id (" & id_actividad_flujo_trabajo & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_usuario_wf = 0
                Else
                    id_usuario_wf = Datset.Tables(0).Rows(0).Item(0)
                End If
                id_actividad = Datset.Tables(0).Rows(0).Item(1)
                Solicita_id_usuario_id_actividad_workflow_flujo_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_usuario_id_actividad_workflow_flujo_actividad = "Inconsistencia general fucion Solicita_id_usuario_id_actividad_workflow_flujo_actividad " & ex.Message
        End Try
    End Function

    '-------------------------name espace proced-------------------------------------------
    Function Mape_proced_adm_flujo_update_activity_description(ByVal id_registro_actividad As Integer,
                                                               ByRef Class_config_general_service As List(Of Class_config_general_service)) As String
        '---------------------------------------------------------------------------
        'Funcion : Retorna la estructura de campos para proceso de operaciones para
        '          la actualización de la descripción de actividad
        '
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_actividad_flujo   : Representa la idneitcación de la tarea dentro de la
        '                       ruta y el flujo de trabajo
        'descripcion          : Representa la descripcion de la actividad
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service : Estructura de campos dinamicos
        ' 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-14
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
            parameter_gestion.name_space_campo = "adm_flujo_update_activity_description"
            '----nombre campo
            parameter_gestion.name_campo = "DESCRIPCION_TAREA_ACTIVIDAD"
            '----aleas campo
            parameter_gestion.aleas_campo = "Decripción actividad"
            Dim tipo_campo As String = "VARCHAR"
            Dim leng_campo As Integer = 40
            '-----Tipo campo tamaño campo
            parameter_gestion.tipo_campo = tipo_campo
            parameter_gestion.max_leng_campo = leng_campo
            '----Campo unico
            parameter_gestion.campo_unico = 0
            '----Aloja null
            parameter_gestion.alow_null = 0
            '----Campo obligatorio
            parameter_gestion.obligatorio_campo = 0

            '----Asigna nombre campo key
            parameter_gestion.name_campo_id = "ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO"
            '----Valor unico registro
            parameter_gestion.dms_id_registro = id_registro_actividad
            '----Asigna validacion lera capital
            parameter_gestion.valida_capital_text = 0
            '----Asigna si el control es tipo correo   
            parameter_gestion.control_tip_correo = 0
            Dim homologa_tipo As Integer = 1
            parameter_gestion.drow_name_controls_destino = ""
            parameter_gestion.drow_name_padre_control = ""
            '-----Asigna tipo de control    (0-option    1- imputext)
            parameter_gestion.title_control = ""
            parameter_gestion.campo_tip = homologa_tipo
            parameter_gestion.tbl_control = "wf_registro_actividaes_flujos_trabajo"
            parameter_gestion.error_gestion = "YES"
            parameter_gestion.disable_campo = 1
            parameter_gestion.dbms_control = "WF"
            parameter_gestion.clas_service_control = "WebService_control_general.asmx"
            parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
            Class_config_general_service.Add(parameter_gestion)
            Mape_proced_adm_flujo_update_activity_description = "YES"
        Catch ex As Exception
            Mape_proced_adm_flujo_update_activity_description = "Inconsistencia general funcion Mape_proced_adm_flujo_update_activity_description " & ex.Message
        End Try
    End Function
End Class
