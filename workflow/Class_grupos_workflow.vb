Public Class Class_grupos_workflow
    Function Solicita_id_grupo_actividad_workflow(ByVal id_actividad As Integer, _
                                                  ByRef id_grupo_workflow As Integer) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Id_Grupo " & _
              " from grupos_workflow where id_Actividad= " & id_actividad
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_grupo_actividad_workflow = "Función Solicita_id_grupo_actividad_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_grupo_actividad_workflow = "Imposible encontrar grupo workflow relacionado a al actvidad (" & id_actividad & ")"
                Exit Function
            Else
                id_grupo_workflow = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_grupo_actividad_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_grupo_actividad_workflow = "Inconsistencia general función Solicita_id_grupo_actividad_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_correo_usuarios_grupo_workflow(ByVal id_grupo As Integer, _
                                                     ByRef correos As String) As String
        Try
            Dim Result As String = ""
            correos = ""
            Dim Parametro_Consulta As String = " SELECT  uw.Correo_Usuario " & _
              " from grupos_workflow  as gw" & _
              " inner join usuario_workflow as uw on (uw.Grupos_Workflow_Id_Grupo=gw.Id_Grupo)" & _
              " where gw.Id_Grupo= " & id_grupo
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_correo_usuarios_grupo_workflow = "Función Solicita_correo_usuarios_grupo_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_correo_usuarios_grupo_workflow = "Imposible encontrar correos de usuario workflow relacioandos a grupo (" & id_grupo & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If correos = "" Then
                        If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                            correos = ""
                        Else
                            correos = Datset.Tables(0).Rows(i).Item(0)
                        End If
                    Else
                        If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                            correos = correos & "," & Datset.Tables(0).Rows(i).Item(0)
                        End If
                    End If
                Next
                Solicita_correo_usuarios_grupo_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_correo_usuarios_grupo_workflow = "Inconsistencia general función Solicita_correo_usuarios_grupo_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_grupo_workflow(ByVal id_grupo As Integer, _
                                            ByRef nombre_grupo As String) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  Nombre_Grupo " & _
              " from grupos_workflow where Id_Grupo= " & id_grupo
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_grupo_workflow = "Función Solicita_nombre_grupo_workflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_grupo_workflow = "Imposible encontrar el nombre del grupo workflow con la siguiente identificación (" & id_grupo & ")"
                Exit Function
            Else
                nombre_grupo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_grupo_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_grupo_workflow = "Inconsistencia general función Solicita_nombre_grupo_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_Actividades_Disponibles_Envio(ByVal Id_Grupo As Integer, _
                                                    ByRef matri() As String) As String
        '****************************************************************
        'Function :  Solicita_Actividades_Disponibles_Envio
        'Fecha    : 2009-07-02
        'Ing      : Miguel Angel Urueta Miranda
        'Proced   : Procedimientos debuelve las actividades
        'disponibles de envio del usuario logueado
        'Parameter: id grupo del usuario y matris actividades
        '****************************************************************

        Try
            Dim Parametro_Consulta As String = "select law.nombre_actividad from grupos_workflow gw " & _
            "inner join actividades_disponibles_envio ade on" & _
            "(ade.Listado_Actividades_Workflow_Id_Actividad=gw.id_actividad)" & _
            "inner join listado_actividades_workflow law on" & _
            "(law.id_actividad=ade.id_actividad_siguiente)" & _
            "where gw.id_grupo=" & Id_Grupo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("actividades_disponibles_envio")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_Actividades_Disponibles_Envio = "Error Solicitando Actividades disponibles de envio  " & Result

                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_Actividades_Disponibles_Envio = "YES"
                Exit Function
            Else
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri(i)
                    matri(i) = Datset.Tables(0).Rows(i).Item(0).ToString

                Next
            End If
            Solicita_Actividades_Disponibles_Envio = "YES"
            Return Solicita_Actividades_Disponibles_Envio
        Catch ex As Exception
            Solicita_Actividades_Disponibles_Envio = ex.Message
        End Try
    End Function
    Function Solicita_Listado_actividades_para_envio_de_tareas_a_ruta(ByVal Id_Grupo As Integer,
                                                                      ByRef grediview As GridView,
                                                                      ByRef reflabel As Label,
                                                                      ByRef label_leyend As Label,
                                                                      ByRef hideselecion As Object,
                                                                      ByVal nombre_ruta As String,
                                                                      ByRef update As UpdatePanel,
                                                                      ByVal consulta_boot As Integer) As String
        Try
            label_leyend.Text = "Ruta de trabajo de la tarea (" & nombre_ruta & ")"
            Dim Sql_consulta As String = "select law.id_actividad,ade.id_actividades_disponibles_envio,law.nombre_actividad AS NOMBRE,law.Descripcion_Actividad as DESCRIPCION  from grupos_workflow gw " &
            " inner join actividades_disponibles_envio ade on" &
            " (ade.Listado_Actividades_Workflow_Id_Actividad=gw.id_actividad)" &
            " inner join listado_actividades_workflow law on" &
            " (law.id_actividad=ade.id_actividad_siguiente)" &
            " inner join actividades_generales_workflow as agw on (agw.Id_Actividad_General=law.Actividades_Generales_Workflow_Id_Actividad_General) " &
            " where gw.id_grupo=" & Id_Grupo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("grupos_workflow")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_Listado_actividades_para_envio_de_tareas_a_ruta = "Error Solicita_Listado_actividades_para_envio_de_tareas_a_ruta  " & Result
                Exit Function
            End If
            Datset.Tables(0).Columns.Add("DESTINO", GetType(String))
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = Datset.Tables(0).Rows.Count & " Grupo (s) "
                grediview.DataSource = Nothing
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                Solicita_Listado_actividades_para_envio_de_tareas_a_ruta = "YES"
                Exit Function
            Else
                reflabel.Text = Datset.Tables(0).Rows.Count & " Grupo (s) "
                grediview.DataSource = Datset
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                If consulta_boot = 0 Then
                    For i As Integer = 0 To grediview.Rows.Count - 1
                        grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        Dim imaga_buton As New HtmlInputImage
                        imaga_buton.Attributes.Add("CssClass", "image_buton_clik_image")
                        imaga_buton.Attributes.Add("onclick", "prevent_ruta(event,this)")
                        imaga_buton.Src = "../workflow/imageneswf/share-all-solid.png"
                        imaga_buton.Attributes.Add("title", "Terminar la trea y enviar a la actividad " & grediview.Rows(i).Cells(2).Text)
                        imaga_buton.Attributes.Add("id_tar_sel", grediview.Rows(i).Cells(3).Text.ToString())
                        imaga_buton.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        imaga_buton.Attributes.Add("nombre_actividad", grediview.Rows(i).Cells(2).Text.ToString())
                        Dim imaga_buton_imagen As New HtmlInputImage
                        imaga_buton_imagen.Attributes.Add("Class", "image_buton_clik_image_no_alow_cursor")
                        grediview.Rows(i).Cells(2).Attributes.Add("CssClass", "image_buton_clik_image_no_alow_cursor")
                        imaga_buton_imagen.Attributes.Add("onclick", "prevent_blank(event,this);")
                        imaga_buton_imagen.Attributes.Add("height", "20px")
                        imaga_buton_imagen.Src = "../workflow/imageneswf/user-solid.png"
                        Dim imaga_buton_detalle As New HtmlInputImage
                        imaga_buton_detalle.Attributes.Add("CssClass", "image_buton_clik_image")
                        imaga_buton_detalle.Attributes.Add("onclick", "prevent_detalle_actividad_ruta(event,this)")
                        imaga_buton_detalle.Src = "../workflow/imageneswf/detalle.png"
                        imaga_buton_detalle.Attributes.Add("title", "Detalle de la actividad")
                        imaga_buton_detalle.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        grediview.Rows(i).Cells(0).Controls.Add(imaga_buton_imagen)
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count - 1).Controls.Add(imaga_buton_detalle)
                        grediview.Rows(i).Cells(Datset.Tables(0).Columns.Count).Controls.Add(imaga_buton)
                    Next
                    Solicita_Listado_actividades_para_envio_de_tareas_a_ruta = "YES"
                    Exit Function
                Else
                    For i As Integer = 0 To grediview.Rows.Count - 1
                        grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                        Dim divhtml_ As New HtmlControls.HtmlGenericControl("div")
                        Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fad fa-share-all")
                        Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_envio_ruta_actividad(event,this);")
                        ahtml.Attributes.Add("title", "Enviar a (" & grediview.Rows(i).Cells(3).Text.ToString() & ")")
                        ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(2).Text.ToString())
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn bg-info btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent_detalle_actividad(event,this);")
                        ahtml.Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                        ahtml.Style.Add("margin-left", "3px")
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
                    Solicita_Listado_actividades_para_envio_de_tareas_a_ruta = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_Listado_actividades_para_envio_de_tareas_a_ruta = "Incosistencia general función Solicita_Listado_actividades_para_envio_de_tareas_a_ruta " & ex.Message
        End Try
    End Function
    Function SolicitaEstadoEjecucionEventoInicio(ByRef EvalUsuario As Integer,
                                                 ByVal IdGrupoWorkflow As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el estado de ejecución del evento de inicio compilada en tiempo de ejecución
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdGrupoWorkflow     : Representa la identificación del grupo workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EvalUsuario  : Retorna el estado de evaluación de la tarea de inicio
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim SqlConsulta = ""
            SqlConsulta = "Select LAW.EVALUA_USUARIO from  GRUPOS_WORKFLOW GW" &
            " inner join  LISTADO_ACTIVIDADES_WORKFLOW AS LAW ON" &
            " (LAW.ID_ACTIVIDAD=GW.ID_ACTIVIDAD)" &
            " WHERE  GW.ID_GRUPO =" & IdGrupoWorkflow
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("GRUPOS_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                SolicitaEstadoEjecucionEventoInicio = "Error consultando en la tabla " & " GRUPOS_WORKFLOW " & SqlConsulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEstadoEjecucionEventoInicio = "Imposible encontrar la relación del grupo (" & IdGrupoWorkflow & ") con la actividad workflow del usuario"
                Exit Function
            Else
                EvalUsuario = Datset.Tables(0).Rows(0).Item(0)
                SolicitaEstadoEjecucionEventoInicio = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstadoEjecucionEventoInicio = "Inconsistencia general funcion SolicitaEstadoEjecucionEventoInicio " & ex.Message
        End Try
    End Function
    Function Solicita_id_actividad_usuario_workflow(ByRef id_actividad As String,
                                                    ByVal id_Grupo As Integer) As String

        Try
            Dim Sql_consulta = ""
            Sql_consulta = "Select ID_ACTIVIDAD from " &
            " GRUPOS_WORKFLOW WHERE ID_GRUPO =" & id_Grupo
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("USUARIO_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_actividad_usuario_workflow = "Error funcion Solicita_id_actividad_usuario_workflow  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_actividad_usuario_workflow = "Imposible Encontrar actividad relacionada al grupo (" & id_Grupo & ")"
                Exit Function
            Else
                id_actividad = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_actividad_usuario_workflow = "YES"
            End If
        Catch ex As Exception
            Solicita_id_actividad_usuario_workflow = "Error #03 Consultando id actividad usuario Solicita_id_actividad_usuario_workflow " & ex.Message
        End Try
    End Function

    Function Solicita_id_actividad_grupo_workflow(ByVal id_Grupo As Integer,
                                                  ByRef id_actividad_workflow As Integer) As String

        '---------------------------------------------------------------------------
        'Funcion : Solicita actividad relacionada al grupo workflow
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_Grupo              : Representa la identificación del grupo workflow
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_actividad_workflow : Retorna la idnetificación de la actividad workflow
        '                        relacionada al grupo
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-07-03
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim Sql_consulta = ""
            Sql_consulta = "Select ID_ACTIVIDAD from " &
            " GRUPOS_WORKFLOW WHERE ID_GRUPO =" & id_Grupo
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("GRUPOS_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_actividad_grupo_workflow = "Error funcion Solicita_id_actividad_grupo_workflow  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_actividad_grupo_workflow = "Imposible encontrar la actividad relacionada al grupo (" & id_Grupo & ")"
                Exit Function
            Else
                id_actividad_workflow = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_actividad_grupo_workflow = "YES"
            End If
        Catch ex As Exception
            Solicita_id_actividad_grupo_workflow = "Error general funcion Solicita_id_actividad_grupo_workflow " & ex.Message
        End Try
    End Function

End Class
