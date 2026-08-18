Imports System.Drawing

Public Class ClassListandoTareas
    Function Inicializa_lista_tarea_workflow_simple(ByRef numero_tareas As Integer) As String
        Try
            Dim Resultado2 As String = ""
            Dim Evalua_Uusario As Integer = 0
            Dim enventgrupo As Integer = 1
            Dim Result As String = ""
            ''------------------------------------
            ''Consulta si se ejecuta script
            ''------------------------------------
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Resultado2 = ref_Class_grupos_workflow.SolicitaEstadoEjecucionEventoInicio(Evalua_Uusario, _
                                                                                            HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Resultado2 <> "YES" Then
                Inicializa_lista_tarea_workflow_simple = Resultado2
                Exit Function
            End If
            ''-----------------------------
            ''Lista las treas por script
            ''-----------------------------
            Dim Conection_conectro_C As String = "Persist Security Info=" _
                  & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                  & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                 & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                 & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString
            Dim mParamT() As Object = {Conection_conectro_C, _
                                       HttpContext.Current.Session("Id_Usuario_Workflow").ToString, _
                                       HttpContext.Current.Session("Id_Grupo_Workflow").ToString, HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"), _
                                       "", HttpContext.Current.Session("Id_Ruta_Workflow")}
            Dim Resultado0 As String = ""
            If HttpContext.Current.Session("INICIO").ToString <> "" And Evalua_Uusario = 1 And enventgrupo = 1 Then
                Dim refcla As New ClassEdtiScript
                Dim ResultadoComp As String = ""
                Dim Resultado4 As String = refcla.Compila_Evalua(ResultadoComp,
                                                                 HttpContext.Current.Session("INICIO"),
                                                                 "INICIO",
                                                                 mParamT)
                If Not ResultadoComp Is Nothing Then
                    If ResultadoComp <> "YES" Then
                        Dim Expresion = Left(ResultadoComp, 6)
                        'HttpContext.Current.Session("WF_CAMPOS_LISTA_TRAMITE_SCRIPT_HI_WF") = ResultadoComp
                        If Expresion = "Select" Then
                            Result = Me.Lista_numero_tareas_workflow(1,
                                                                     HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"),
                                                                     HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                     ResultadoComp,
                                                                     HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                     numero_tareas)
                            If Result <> "YES" Then
                                Inicializa_lista_tarea_workflow_simple = Result
                                Exit Function
                            Else
                                Inicializa_lista_tarea_workflow_simple = "YES"
                                Exit Function
                            End If
                        Else
                            Inicializa_lista_tarea_workflow_simple = "El scrip (INICIO) no devuelve select"
                            Exit Function
                        End If
                    Else
                        Inicializa_lista_tarea_workflow_simple = "El scrip (INICIO) no devuelve YES estado no valido"
                        Exit Function
                    End If
                Else
                    Inicializa_lista_tarea_workflow_simple = "El scrip (INICIO) no devuelve nothing"
                    Exit Function
                End If
            Else
                Result = Me.Lista_numero_tareas_workflow(2, _
                                                        HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"), _
                                                        HttpContext.Current.Session("Id_Usuario_Workflow"), _
                                                        "", _
                                                        HttpContext.Current.Session("WF_RUTAWORKFLOW"), _
                                                        numero_tareas)
                If Result <> "YES" Then
                    Inicializa_lista_tarea_workflow_simple = Result
                    Exit Function
                Else
                    Inicializa_lista_tarea_workflow_simple = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Inicializa_lista_tarea_workflow_simple = "Inconistencia general funcion Inicializa_lista_tarea_workflow_simple " & ex.Message
        End Try
    End Function
    Function Inicializar_la_lista_de_tareas_workflow(ByRef page As Page,
                                                     ByRef Reftable As Table,
                                                     ByRef scripma As GridView,
                                                     ByRef enventgrupo As Integer,
                                                     ByVal tipo_consulta As Integer,
                                                     ByVal valor_consulta As String,
                                                     ByRef colum_order_name As String,
                                                     ByRef order_colum As String,
                                                     ByVal cuenta_numero_tareas As Integer,
                                                     ByVal estado_filtro_grugo_usuario As String) As String
        Try
            '*******************************************
            'Actualizacion 2012-06-04
            'Ingeniero : Miguel Angel Urueta Miranda
            'Actualizacion : Se agrega codigo para 
            'determinar el tab seleccionado y se asigna
            'una refeencia al listview
            '*******************************************
            Dim mEval1 As New ClassEdtiScript
            Dim Expresion As String = ""
            Dim Resultado As String = ""
            Dim Resultado2 As String = ""
            Dim SqlConsulta As String = ""
            'Dim Evalua_Uusario As Integer = 0
            ''------------------------------------
            ''Consulta si se ejecuta script
            ''------------------------------------
            HttpContext.Current.Session("WF_CAMPOS_LISTA_TRAMITE_SCRIPT_HI_WF") = ""
            Dim ref_Class_grupos_workflow As New Class_grupos_workflow
            Resultado2 = ref_Class_grupos_workflow.SolicitaEstadoEjecucionEventoInicio(HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO"),
                                                                                            HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Resultado2 <> "YES" Then
                Inicializar_la_lista_de_tareas_workflow = Resultado2
                Exit Function
            End If

            Dim Result As String = ""
            Dim Refclas_config_list_ruta As New Class_configuracion_listado_ruta
            Result = Refclas_config_list_ruta.Solicita_campos_lista_workflow(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                             HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI_WF"))
            If Result <> "YES" Then
                Inicializar_la_lista_de_tareas_workflow = Result
                Exit Function
            End If
            Dim LabelEspera As Label = page.FindControl("LabelEspera")
            If LabelEspera Is Nothing Then
                Inicializar_la_lista_de_tareas_workflow = "Imposible encontrar el control (LabelEspera)"
                Exit Function
            End If
            Dim UpdatePanelnumeroespera As UpdatePanel = page.FindControl("UpdatePanelnumeroespera")
            If UpdatePanelnumeroespera Is Nothing Then
                Inicializar_la_lista_de_tareas_workflow = "Imposible encontrar el control (UpdatePanelnumeroespera)"
                Exit Function
            End If
            Dim UpdatePanel1 As UpdatePanel = page.FindControl("UpdatePanel1")
            If UpdatePanel1 Is Nothing Then
                Inicializar_la_lista_de_tareas_workflow = "Imposible encontrar el control (UpdatePanel1)"
                Exit Function
            End If
            Dim HiddenSeleccion As Object = page.FindControl("HiddenSeleccion")
            If HiddenSeleccion Is Nothing Then
                Inicializar_la_lista_de_tareas_workflow = "Imposible encontrar el control (HiddenSeleccion)"
                Exit Function
            End If
            Dim Hidden_cantidad_registros As Object = page.FindControl("Hidden_cantidad_registros")
            If Hidden_cantidad_registros Is Nothing Then
                Inicializar_la_lista_de_tareas_workflow = "Imposible encontrar el control (Hidden_cantidad_registros)"
                Exit Function
            End If
            ''-----------------------------
            ''Lista las treas por script
            ''-----------------------------
            Dim Conection_conectro_C As String = "Persist Security Info=" _
                  & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                  & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                 & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                 & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString
            Dim mParamT() As Object = {Conection_conectro_C,
                                       HttpContext.Current.Session("Id_Usuario_Workflow").ToString,
                                       HttpContext.Current.Session("Id_Grupo_Workflow").ToString,
                                       HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD").ToString,
                                       "", HttpContext.Current.Session("Id_Ruta_Workflow")}

            HttpContext.Current.Session.Item("WF_MPARANT_LISTA_TRAMITE") = mParamT
            If HttpContext.Current.Session("INICIO").ToString <> "" And HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 1 And enventgrupo = 1 Then
                HttpContext.Current.Session("WF_TIPO_LISTA_TRAMITE_HI_WF") = 1
                Dim refcla As New ClassEdtiScript
                Dim ResultadoComp As String = ""
                Dim Resultado4 As String = refcla.Compila_Evalua(ResultadoComp,
                                                                 HttpContext.Current.Session("INICIO"),
                                                                 "INICIO",
                                                                 mParamT)
                If Resultado4 <> "YES" Then
                    Inicializar_la_lista_de_tareas_workflow = "EL evento (INICIO) no se compila (" & Resultado4 & ")"
                    Exit Function
                End If
                If ResultadoComp <> "YES" Then
                    Expresion = Left(ResultadoComp, 6)
                    HttpContext.Current.Session("WF_CACHE_CONSULTA_SCRIPT_INICIO") = ResultadoComp
                    If Expresion <> "Select" Then
                        Inicializar_la_lista_de_tareas_workflow = "El evento (INICIO) no devuelve la expresion (SELEC) esperada  para listar la tareas. Contacte a su administrador "
                        Exit Function
                    End If
                Else
                    Inicializar_la_lista_de_tareas_workflow = "La evento (INICIO) no retorna nigúnn dato para listar. Contacte a su administrador"
                    Exit Function
                End If
                Dim Resultado1 = Listar_tareas_workflow_Script(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                               HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                               HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                               scripma,
                                                               Val(HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD")),
                                                               HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI_WF"),
                                                               HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                               LabelEspera,
                                                               HiddenSeleccion,
                                                               UpdatePanel1,
                                                               UpdatePanelnumeroespera,
                                                               tipo_consulta,
                                                               HttpContext.Current.Session("WF_CACHE_CONSULTA_SCRIPT_INICIO"),
                                                               colum_order_name,
                                                               order_colum,
                                                               Hidden_cantidad_registros,
                                                               ResultadoComp,
                                                               estado_filtro_grugo_usuario)
                If Resultado1 <> "YES" Then
                    Inicializar_la_lista_de_tareas_workflow = "inconsistencia listando tareas desde el evento (INICIO), verique que el usuario esté relacionado al grupo workflow (" & HttpContext.Current.Session("Id_Grupo_Workflow").ToString & ")" _
                            & Resultado1
                    Exit Function
                Else
                    If cuenta_numero_tareas = 1 Then
                        Result = Me.Lista_numero_tareas_workflow(1,
                                                                 HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"),
                                                                 HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                 ResultadoComp,
                                                                 HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                 HttpContext.Current.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W"))
                        If Result <> "YES" Then
                            Inicializar_la_lista_de_tareas_workflow = Result
                            Exit Function
                        Else
                            Inicializar_la_lista_de_tareas_workflow = "YES"
                            Exit Function
                        End If
                    End If
                    Inicializar_la_lista_de_tareas_workflow = "YES"
                    Exit Function
                End If
            End If
            '------------------------------------------
            'lista actividades por eventos del sistema
            '------------------------------------------
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 0 Then
                HttpContext.Current.Session("WF_TIPO_LISTA_TRAMITE_HI_WF") = 0
                Resultado = Listar_tareas_workflow(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                   HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                   HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                   scripma,
                                                   Val(HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD")),
                                                   HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI_WF"),
                                                   HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                   LabelEspera,
                                                   HiddenSeleccion,
                                                   UpdatePanel1,
                                                   UpdatePanelnumeroespera,
                                                   tipo_consulta,
                                                   valor_consulta,
                                                   colum_order_name,
                                                   order_colum,
                                                   Hidden_cantidad_registros,
                                                   estado_filtro_grugo_usuario)
                If Resultado = "YES" Then
                    If cuenta_numero_tareas = 1 Then
                        Result = Me.Lista_numero_tareas_workflow(2,
                                                                 HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"),
                                                                 HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                 "",
                                                                 HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                 HttpContext.Current.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W"))
                        If Result <> "YES" Then
                            Inicializar_la_lista_de_tareas_workflow = Result
                            Exit Function
                        Else
                            Inicializar_la_lista_de_tareas_workflow = "YES"
                            Exit Function
                        End If
                    End If
                    Inicializar_la_lista_de_tareas_workflow = "YES"
                    Exit Function
                Else
                    Inicializar_la_lista_de_tareas_workflow = "Error Buscando Tareas" + Resultado
                End If
            End If
            Inicializar_la_lista_de_tareas_workflow = "YES"
        Catch ex As Exception
            Inicializar_la_lista_de_tareas_workflow = "Inconsistencia general función Inicializar_la_lista_de_tareas_workflow " + ex.Message
        End Try
    End Function
    Function Pre_Listar_tareas_workflow(ByRef page As Page,
                                               ByRef Reftable As Table,
                                               ByRef scripma As GridView,
                                               ByRef enventgrupo As Integer,
                                               ByVal tipo_consulta As Integer,
                                               ByVal valor_consulta As String,
                                               ByRef colum_order_name As String,
                                               ByRef order_colum As String,
                                               ByVal cuenta_numero_tareas As Integer,
                                               ByVal estado_filtro_grugo_usuario As String) As String
        Try
            Dim Result As String = ""
            Dim LabelEspera As Label = page.FindControl("LabelEspera")
            If LabelEspera Is Nothing Then
                Pre_Listar_tareas_workflow = "Imposible encontrar el control (LabelEspera)"
                Exit Function
            End If
            Dim UpdatePanelnumeroespera As UpdatePanel = page.FindControl("UpdatePanelnumeroespera")
            If UpdatePanelnumeroespera Is Nothing Then
                Pre_Listar_tareas_workflow = "Imposible encontrar el control (UpdatePanelnumeroespera)"
                Exit Function
            End If
            Dim UpdatePanel1 As UpdatePanel = page.FindControl("UpdatePanel1")
            If UpdatePanel1 Is Nothing Then
                Pre_Listar_tareas_workflow = "Imposible encontrar el control (UpdatePanel1)"
                Exit Function
            End If
            Dim HiddenSeleccion As Object = page.FindControl("HiddenSeleccion")
            If HiddenSeleccion Is Nothing Then
                Pre_Listar_tareas_workflow = "Imposible encontrar el control (HiddenSeleccion)"
                Exit Function
            End If
            Dim Hidden_cantidad_registros As Object = page.FindControl("Hidden_cantidad_registros")
            If Hidden_cantidad_registros Is Nothing Then
                Pre_Listar_tareas_workflow = "Imposible encontrar el control (Hidden_cantidad_registros)"
                Exit Function
            End If
            Result = Listar_tareas_workflow(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                   HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                   HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                   scripma,
                                                   Val(HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD")),
                                                   HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI_WF"),
                                                   HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                   LabelEspera,
                                                   HiddenSeleccion,
                                                   UpdatePanel1,
                                                   UpdatePanelnumeroespera,
                                                   tipo_consulta,
                                                   valor_consulta,
                                                   colum_order_name,
                                                   order_colum,
                                                   Hidden_cantidad_registros,
                                                   estado_filtro_grugo_usuario)
            If Result <> "YES" Then
                Pre_Listar_tareas_workflow = "Error Buscando Tareas" & Result
                Exit Function
            Else
                Pre_Listar_tareas_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Pre_Listar_tareas_workflow = "Inconsistencia general funcion Pre_Listar_tareas_workflow " & ex.Message
        End Try
    End Function
    Function Pre_Listar_tareas_workflow_Script(ByRef page As Page,
                                               ByRef Reftable As Table,
                                               ByRef scripma As GridView,
                                               ByRef enventgrupo As Integer,
                                               ByVal tipo_consulta As Integer,
                                               ByVal valor_consulta As String,
                                               ByRef colum_order_name As String,
                                               ByRef order_colum As String,
                                               ByVal cuenta_numero_tareas As Integer,
                                               ByVal estado_filtro_grugo_usuario As String) As String
        Try
            Dim Result As String = ""
            Dim LabelEspera As Label = page.FindControl("LabelEspera")
            If LabelEspera Is Nothing Then
                Pre_Listar_tareas_workflow_Script = "Imposible encontrar el control (LabelEspera)"
                Exit Function
            End If
            Dim UpdatePanelnumeroespera As UpdatePanel = page.FindControl("UpdatePanelnumeroespera")
            If UpdatePanelnumeroespera Is Nothing Then
                Pre_Listar_tareas_workflow_Script = "Imposible encontrar el control (UpdatePanelnumeroespera)"
                Exit Function
            End If
            Dim UpdatePanel1 As UpdatePanel = page.FindControl("UpdatePanel1")
            If UpdatePanel1 Is Nothing Then
                Pre_Listar_tareas_workflow_Script = "Imposible encontrar el control (UpdatePanel1)"
                Exit Function
            End If
            Dim HiddenSeleccion As Object = page.FindControl("HiddenSeleccion")
            If HiddenSeleccion Is Nothing Then
                Pre_Listar_tareas_workflow_Script = "Imposible encontrar el control (HiddenSeleccion)"
                Exit Function
            End If
            Dim Hidden_cantidad_registros As Object = page.FindControl("Hidden_cantidad_registros")
            If Hidden_cantidad_registros Is Nothing Then
                Pre_Listar_tareas_workflow_Script = "Imposible encontrar el control (Hidden_cantidad_registros)"
                Exit Function
            End If

            Dim Expresion As String = Left(HttpContext.Current.Session("WF_CACHE_CONSULTA_SCRIPT_INICIO"), 6)
            If Expresion <> "Select" Then
                Pre_Listar_tareas_workflow_Script = "El evento (INICIO) no devuelve la expresion (SELEC) esperada  para listar la tareas. Contacte a su administrador "
                Exit Function
            End If
            Result = Listar_tareas_workflow_Script(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                   HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                   HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                   scripma,
                                                   Val(HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD")),
                                                   HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI_WF"),
                                                   HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                   LabelEspera,
                                                   HiddenSeleccion,
                                                   UpdatePanel1,
                                                   UpdatePanelnumeroespera,
                                                   tipo_consulta,
                                                   valor_consulta,
                                                   colum_order_name,
                                                   order_colum,
                                                   Hidden_cantidad_registros,
                                                   HttpContext.Current.Session("WF_CACHE_CONSULTA_SCRIPT_INICIO"),
                                                   estado_filtro_grugo_usuario)
            If Result <> "YES" Then
                Pre_Listar_tareas_workflow_Script = "inconsistencia listando tareas desde el evento (INICIO), verique que el usuario esté relacionado al grupo workflow (" & HttpContext.Current.Session("Id_Grupo_Workflow").ToString & ") error " _
                                & Result
                Exit Function
            Else
                Pre_Listar_tareas_workflow_Script = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Pre_Listar_tareas_workflow_Script = "Inconsistencia general función Pre_Listar_tareas_workflow_Script " & ex.Message
            Exit Function
        End Try
    End Function
    Function Verifica_existencia_campos_listado_ruta(ByVal id_ruta As Integer, _
                                                     ByRef exitencia_campos_ruta As String, _
                                                     ByRef matri_campos() As String) As String
        Try
            Dim Sql_consulta As String = "Select Nombre_Campo " & _
               " from Configuracion_listado_ruta  " & _
               " where Rutas_Workflow_id_Ruta=" & id_ruta
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("Configuracion_listado_ruta")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_campos_listado_ruta = "Error Consultando en tabla " & " Configuracion_listado_ruta " & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                exitencia_campos_ruta = "NO"
                Verifica_existencia_campos_listado_ruta = "YES"
                Exit Function
            Else
                exitencia_campos_ruta = "YES"
                Dim i As Integer = 0
                For i2 As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_campos(i)
                    matri_campos(i) = Datset.Tables(0).Rows(i2).Item(0)
                    i = i + 1
                Next
                Verifica_existencia_campos_listado_ruta = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Verifica_existencia_campos_listado_ruta = "Inconsistencia función Verifica_existencia_campos_listado_ruta  " & ex.Message
        End Try
    End Function
    Function Retorna_id_ruta_usuario_workflow(ByVal id_usuario_wf As Integer, ByRef id_ruta_wf As Integer, ByRef nombre_ruta As String) As String
        Try
            Dim Sql_consulta As String = "Select uw.Grupos_Workflow_Rutas_Workflow_id_Ruta,rw.Nombre_Ruta " & _
                 " from usuario_workflow As uw " & _
                "inner join rutas_workflow As rw On (rw.id_ruta=uw.Grupos_Workflow_Rutas_Workflow_id_Ruta) " & _
               " where idU_suario=" & id_usuario_wf
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("usuario_workflow")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_ruta_usuario_workflow = "Función Retorna_id_ruta_usuario_workflow " & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_ruta_usuario_workflow = "Función Retorna_id_ruta_usuario_workflow dice : imposible encontrar id ruta usuario workflow "
                Exit Function
            Else
                id_ruta_wf = Datset.Tables(0).Rows(0).Item("Grupos_Workflow_Rutas_Workflow_id_Ruta")
                nombre_ruta = Datset.Tables(0).Rows(0).Item("Nombre_Ruta")
                Retorna_id_ruta_usuario_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_ruta_usuario_workflow = "Inconsistencia función Retorna_id_ruta_usuario_workflow  " & ex.Message
        End Try
    End Function
    Function Listar_tareas_workflow_Script(ByVal id_ruta_workflow As Integer,
                                           ByVal Id_Grupo_Workflow As Integer,
                                           ByVal Id_Usuario_Workflow As Integer,
                                           ByRef grediview As GridView,
                                           ByVal Id_actividad As Integer,
                                           ByVal campo_lista_tramite As String,
                                           ByVal nombre_ruta As String,
                                           ByRef reflabel As Label,
                                           ByRef hideselecion As Object,
                                           ByRef update_gred As UpdatePanel,
                                           ByRef update_title As UpdatePanel,
                                           ByVal tipo_consulta As Integer,
                                           ByVal valor_consulta As String,
                                           ByRef colum_order_name As String,
                                           ByRef order_colum As String,
                                           ByRef Hidden_cantidad_registros As Object,
                                           ByVal sql_select As String,
                                           ByVal estado_filtro_grugo_usuario As String) As String
        Try
            Dim Numero_Actividades As Integer = 0
            Dim Fecha_Ini As String = ""
            Dim Fecha_Fin As String = ""
            Dim Sql_Consulta_Listado As String = "Select "
            Dim Sql_consulta As String = ""
            Dim Result As String = ""
            Dim limite_fecha As String = ""
            Dim limite_numero_lista_tareas As String = ""
            Dim ref_clas_seleccion As New Classselecciotarea
            Dim tipo_actividad As String = ""
            Dim seleccion_util_pendiente As String = ""
            Dim valor_etado_tarea As String = ""
            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE") = vbObject
            If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 1 Then
                seleccion_util_pendiente = " and etw.estado_tarea=0"
                valor_etado_tarea = "Pendiente"
            Else
                valor_etado_tarea = "En proceso"
            End If

            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Result = Class_Listado_Actividades_workflow.SolicitaNombreTipoActividadGeneralWorkflow(Id_actividad,
                                                                                                   tipo_actividad)
            If Result <> "YES" Then
                Listar_tareas_workflow_Script = Result
                Exit Function
            End If
            Dim fitro_estado_modulo_correspondencia As String = ""
            If tipo_actividad = "ENLASE" Then
                fitro_estado_modulo_correspondencia = ""
            Else
                fitro_estado_modulo_correspondencia = " and estado_modulo_radicado = 0"
            End If
            '---------------------------------
            'Consultando configuracion usuario
            '---------------------------------
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim ref_Class_configuracion_usuario As New Class_configuracion_usuario
            Result = ref_Class_configuracion_usuario.Retorna_parametros_limite_actividades_fecha_tareas(Numero_Actividades,
                                                                                                        Fecha_Ini,
                                                                                                        Fecha_Fin,
                                                                                                        Id_Usuario_Workflow)
            If Result <> "YES" Then
                Listar_tareas_workflow_Script = Result
                Exit Function
            End If
            If Fecha_Ini <> "" And Fecha_Fin <> "" Then
                ClassGestionFechas.formato_fecha_estructura(Fecha_Ini)
                ClassGestionFechas.formato_fecha_estructura(Fecha_Fin)
                limite_fecha = " and CAST(etw.fecha_inicio AS Date) between '" & Fecha_Ini &
                "' and '" & Fecha_Fin & "'"
            End If
            If colum_order_name = "" Then
                colum_order_name = "etw.fecha_inicio"
                order_colum = "asc"
            End If
            If Numero_Actividades <> 0 Then
                limite_numero_lista_tareas = " LIMIT " & Numero_Actividades
            Else
                limite_numero_lista_tareas = " LIMIT 1000"
            End If
            limite_numero_lista_tareas = ""
            If tipo_consulta = 1 Then
                Dim filtro_usuario_grupo As String = "(etw.id_actividad=" & Id_actividad &
                    "  and etw.fecha_fin is null   " & seleccion_util_pendiente & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                    ") or ( etw.id_actividad=" & Id_actividad & " and etw.fecha_Seleccion is null and etw.fecha_fin is null    and etw.id_usuario is null " & seleccion_util_pendiente & limite_fecha &
                    ")"
                'Dim filtro_usuario_grupo As String = "(etw.id_actividad=" & Id_actividad &
                '    " and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0  " & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                '    ") or ( etw.id_actividad=" & Id_actividad & " and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0  and etw.id_usuario is null " & limite_fecha &
                '    ")"
                If estado_filtro_grugo_usuario = "tareas de grupo" Then
                    filtro_usuario_grupo = "(etw.id_actividad = " & Id_actividad & " And etw.fecha_Seleccion Is null And etw.fecha_fin Is null  And etw.id_usuario Is null " & seleccion_util_pendiente & limite_fecha &
                    ")"

                End If
                'If estado_filtro_grugo_usuario = "Tareas de grupo" Then
                '    filtro_usuario_grupo = "(etw.id_actividad = " & Id_actividad & " And etw.fecha_Seleccion Is null And etw.fecha_fin Is null And etw.estado_tarea = 0 And etw.id_usuario Is null " & limite_fecha &
                '    ")"
                'End If
                If estado_filtro_grugo_usuario = "tareas de usuario" Then
                    filtro_usuario_grupo = "(etw.id_actividad=" & Id_actividad &
                   "  and etw.fecha_fin is null   " & seleccion_util_pendiente & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                   ")"

                End If
                'If estado_filtro_grugo_usuario = "Tareas de usuario" Then
                '    filtro_usuario_grupo = "(etw.id_actividad=" & Id_actividad &
                '    " and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0  " & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                '    ")"
                'End If
                If estado_filtro_grugo_usuario = "tareas en proceso" Then
                    filtro_usuario_grupo = "(etw.id_actividad=" & Id_actividad &
                    "  and etw.fecha_fin is null  and etw.estado_tarea=1  " & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                    ") or ( etw.id_actividad=" & Id_actividad & "  and etw.fecha_fin is null  and etw.estado_tarea=1  and etw.id_usuario is null " & limite_fecha &
                    ")"
                End If
                If estado_filtro_grugo_usuario = "tareas en espera" Then
                    filtro_usuario_grupo = "(etw.id_actividad=" & Id_actividad &
                    "  and etw.fecha_fin is null  and etw.estado_tarea=0  " & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                    ") or ( etw.id_actividad=" & Id_actividad & "  and etw.fecha_fin is null  and etw.estado_tarea=0  and etw.id_usuario is null " & limite_fecha &
                    ")"
                End If
                'Sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_Seleccion,etw.estado_prioridad as prioridad,DAT.FLUJO_INTERNO_WF,etw.id_usuario," &
                '    campo_lista_tramite & ",etw.Estado_Tarea as ESTADO,wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                '    " estados_tarea_workflow etw " &
                '    " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " &
                '    " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & fitro_estado_modulo_correspondencia & " )" &
                '    " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                '    " where " & filtro_usuario_grupo & " order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas
                Sql_consulta = sql_select &
                    " where  " & filtro_usuario_grupo & " order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas
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
                Sql_consulta = sql_select &
                              " where (" & sql_consulta_texto & ") " &
                              " and ((etw.id_actividad=" & Id_actividad &
                              "  and etw.fecha_fin is null   and etw.id_usuario=" & Id_Usuario_Workflow &
                              ") or ( etw.id_actividad=" & Id_actividad & "  and etw.fecha_fin is null    and etw.id_usuario is null " &
                              ")) order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas
                'Sql_consulta = sql_select &
                '              " where (" & sql_consulta_texto & ") " &
                '              " and ((etw.id_actividad=" & Id_actividad &
                '              " and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0 and etw.id_usuario=" & Id_Usuario_Workflow &
                '              ") or ( etw.id_actividad=" & Id_actividad & " and etw.fecha_Seleccion is null and etw.fecha_fin is null  and etw.estado_tarea=0  and etw.id_usuario is null " &
                '              ")) order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas

            End If
            If tipo_consulta = 3 Then
                Sql_consulta = sql_select &
                             " where (" & valor_consulta & ") " &
                             " and etw.id_actividad=" & Id_actividad &
                             " and etw.fecha_fin is null    " & limite_fecha & " or etw.id_usuario=" & Id_Usuario_Workflow &
                             " order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas

            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF") = valor_consulta
            HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_WF") = campo_lista_tramite
            HttpContext.Current.Session.Item("CAHCE_CONSULTA_INERT_WF") = Sql_consulta
            Dim spli_colum() As String = {"OPCIONES", "etw.Inicio_Tareas_Workflow_id_Tarea", "etw.fecha_Seleccion", "etw.estado_prioridad", "DAT.FLUJO_INTERNO_WF", "etw.id_usuario"}
            Dim spli_campos_() As String = campo_lista_tramite.Split(",")
            For i As Integer = 0 To spli_campos_.Length - 1
                Dim nuevo_indice As Integer = spli_colum.Length
                ReDim Preserve spli_colum(nuevo_indice)
                spli_colum(nuevo_indice) = spli_campos_(i)
            Next
            Dim nuevo_indice_ As Integer = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FLUJO_TAREA"
            nuevo_indice_ = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FECHAINICIOTRAMITE"
            nuevo_indice_ = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FECHAFINALTRAMITE"
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido_hi_WF") = spli_colum
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido_hi") = spli_colum
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Listar_tareas_workflow_Script = "Funcion Listar_tareas_workflow Error :  " & Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = Datset
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                Hidden_cantidad_registros.value = Datset.Tables(0).Rows.Count
                reflabel.Text = "(0)"
                HttpContext.Current.Session.Item("NUMEROACTIVIDADES") = 0
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                hideselecion.value = "-1"
                update_gred.Update()
                update_title.Update()
                Listar_tareas_workflow_Script = "YES"
                Exit Function
            Else
                Hidden_cantidad_registros.value = Datset.Tables(0).Rows.Count
                HttpContext.Current.Session.Item("NUMEROACTIVIDADES") = Datset.Tables(0).Rows.Count
                reflabel.Text = "(" & Datset.Tables(0).Rows.Count & ")"
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update_gred.Update()
                update_title.Update()
                Result = Me.pluguin_lista_tarea_workflow(Datset,
                                                      grediview,
                                                      update_gred,
                                                      colum_order_name,
                                                      order_colum,
                                                      valor_etado_tarea)
                Listar_tareas_workflow_Script = Result
                Exit Function
            End If
        Catch ex As Exception
            Listar_tareas_workflow_Script = "Inconsistencia general funcion Listar_tareas_workflow_Script " & ex.Message
        End Try
    End Function
    Function Listar_tareas_workflow(ByVal id_ruta_workflow As Integer,
                                    ByVal Id_Grupo_Workflow As Integer,
                                    ByVal Id_Usuario_Workflow As Integer,
                                    ByRef grediview As GridView,
                                    ByVal Id_actividad As Integer,
                                    ByVal campo_lista_tramite As String,
                                    ByVal nombre_ruta As String,
                                    ByRef reflabel As Label,
                                    ByRef hideselecion As Object,
                                    ByRef update_gred As UpdatePanel,
                                    ByRef update_title As UpdatePanel,
                                    ByVal tipo_consulta As Integer,
                                    ByVal valor_consulta As String,
                                    ByRef colum_order_name As String,
                                    ByRef order_colum As String,
                                    ByRef Hidden_cantidad_registros As Object,
                                    ByVal estado_filtro_grugo_usuario As String) As String
        '-----------------------------------------------------------------
        'Funcion   :Listar Actividades Workflow
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha 2009-07-27
        'Procedimiento : funcion del sistema que lista todos las tareas
        'asignadas a la actividad las cuales heredan los usuarios
        'realaciondos con la actividad
        '-----------------------------------------------------------------

        Try
            Dim Numero_Actividades As Integer = 0
            Dim Fecha_Ini As String = ""
            Dim Fecha_Fin As String = ""
            Dim Sql_Consulta_Listado As String = "Select "
            Dim Matri_Campos_Lista() As String = Nothing
            Dim Sql_consulta As String = ""
            Dim Result As String = ""
            Dim ref_clas_seleccion As New Classselecciotarea
            Dim tipo_actividad As String = ""
            Dim seleccion_util_pendiente As String = ""
            Dim valor_etado_tarea As String = ""
            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE") = vbObject
            If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 1 Then
                seleccion_util_pendiente = " and etw.estado_tarea=0"
                valor_etado_tarea = "Pendiente"
            Else
                valor_etado_tarea = "En proceso"
            End If
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Result = Class_Listado_Actividades_workflow.SolicitaNombreTipoActividadGeneralWorkflow(Id_actividad,
                                                                                                   tipo_actividad)
            If Result <> "YES" Then
                Listar_tareas_workflow = Result
                Exit Function
            End If
            If campo_lista_tramite <> "" Then
                campo_lista_tramite = campo_lista_tramite & ",wraft.DESCRIPCION_TAREA_ACTIVIDAD as ACTIVIDAD "
            Else
                campo_lista_tramite = "wraft.DESCRIPCION_TAREA_ACTIVIDAD as ACTIVIDAD "
            End If
            Dim fitro_estado_modulo_correspondencia As String = ""
            If tipo_actividad = "ENLASE" Then
                fitro_estado_modulo_correspondencia = ""
            Else
                fitro_estado_modulo_correspondencia = " and estado_modulo_radicado = 0"
            End If
            Dim limite_fecha As String = ""
            Dim limite_numero_lista_tareas As String = ""
            '---------------------------------
            'Consultando configuracion usuario
            '---------------------------------
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim ref_Class_configuracion_usuario As New Class_configuracion_usuario
            Result = ref_Class_configuracion_usuario.Retorna_parametros_limite_actividades_fecha_tareas(Numero_Actividades,
                                                                                                        Fecha_Ini,
                                                                                                        Fecha_Fin,
                                                                                                        Id_Usuario_Workflow)
            If Result <> "YES" Then
                Listar_tareas_workflow = Result
                Exit Function
            End If
            If Fecha_Ini <> "" And Fecha_Fin <> "" Then
                ClassGestionFechas.formato_fecha_estructura(Fecha_Ini)
                ClassGestionFechas.formato_fecha_estructura(Fecha_Fin)
                limite_fecha = " and CAST(etw.fecha_inicio AS Date) between '" & Fecha_Ini &
                "' and '" & Fecha_Fin & "'"
            End If
            If colum_order_name = "" Then
                colum_order_name = "etw.fecha_inicio"
                order_colum = "asc"
            End If
            If Numero_Actividades <> 0 Then
                limite_numero_lista_tareas = " LIMIT " & Numero_Actividades
            Else
                limite_numero_lista_tareas = " LIMIT 2000"
            End If
            If tipo_consulta = 1 Then
                Dim filtro_usuario_grupo As String = "(etw.id_actividad=" & Id_actividad &
                    "  and etw.fecha_fin is null  and ESTADO_ACTIVIDA_MODULO_RAD=0 " & seleccion_util_pendiente & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                    ") or ( etw.id_actividad=" & Id_actividad & " and etw.fecha_Seleccion is null and etw.fecha_fin is null    and etw.id_usuario is null and ESTADO_ACTIVIDA_MODULO_RAD=0 " & seleccion_util_pendiente & limite_fecha &
                    ")"

                If estado_filtro_grugo_usuario = "tareas de grupo" Then
                    filtro_usuario_grupo = "(etw.id_actividad = " & Id_actividad & " And etw.fecha_Seleccion Is null And etw.fecha_fin Is null  And etw.id_usuario Is null and ESTADO_ACTIVIDA_MODULO_RAD=0 " & seleccion_util_pendiente & limite_fecha &
                    ")"

                End If
                If estado_filtro_grugo_usuario = "tareas de usuario" Then
                    filtro_usuario_grupo = "(etw.id_actividad=" & Id_actividad &
                   "  and etw.fecha_fin is null  and ESTADO_ACTIVIDA_MODULO_RAD=0 " & seleccion_util_pendiente & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                   ")"

                End If
                If estado_filtro_grugo_usuario = "tareas en proceso" Then
                    filtro_usuario_grupo = "(etw.id_actividad=" & Id_actividad &
                    "  and etw.fecha_fin is null  and etw.estado_tarea=1  and ESTADO_ACTIVIDA_MODULO_RAD=0 " & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                    ") or ( etw.id_actividad=" & Id_actividad & "  and etw.fecha_fin is null  and etw.estado_tarea=1  and etw.id_usuario is null and ESTADO_ACTIVIDA_MODULO_RAD=0 " & limite_fecha &
                    ")"
                End If
                If estado_filtro_grugo_usuario = "tareas en espera" Then
                    filtro_usuario_grupo = "(etw.id_actividad=" & Id_actividad &
                    "  and etw.fecha_fin is null  and etw.estado_tarea=0 and ESTADO_ACTIVIDA_MODULO_RAD=0 " & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow &
                    ") or ( etw.id_actividad=" & Id_actividad & "  and etw.fecha_fin is null  and etw.estado_tarea=0  and etw.id_usuario is null and ESTADO_ACTIVIDA_MODULO_RAD=0 " & limite_fecha &
                    ")"
                End If
                Sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_Seleccion,etw.estado_prioridad as prioridad,DAT.FLUJO_INTERNO_WF,etw.id_usuario," &
                    campo_lista_tramite & ",etw.Estado_Tarea as ESTADO,wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                    " estados_tarea_workflow etw " &
                    " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " &
                    " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & fitro_estado_modulo_correspondencia & " )" &
                    " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                    " left outer join  wf_registro_actividaes_flujos_trabajo as wraft on (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=etw.ID_ACTIVIDAD_FLUJO_TRABAJO)" &
                    " where " & filtro_usuario_grupo & "  " & " order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas
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
                Sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_Seleccion,etw.estado_prioridad as prioridad,DAT.FLUJO_INTERNO_WF,etw.id_usuario," & campo_lista_tramite & ",etw.Estado_Tarea as ESTADO,wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                             " estados_tarea_workflow etw " &
                             " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " &
                             " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & fitro_estado_modulo_correspondencia & " ) " &
                             " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                             " left outer join  wf_registro_actividaes_flujos_trabajo as wraft on (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=etw.ID_ACTIVIDAD_FLUJO_TRABAJO)" &
                             " where (" & sql_consulta_texto & ") " &
                             " and ((etw.id_actividad=" & Id_actividad &
                             "  and etw.fecha_fin is null  and ESTADO_ACTIVIDA_MODULO_RAD=0  and etw.id_usuario=" & Id_Usuario_Workflow &
                             ") or ( etw.id_actividad=" & Id_actividad & "  and etw.fecha_fin is null and ESTADO_ACTIVIDA_MODULO_RAD=0  and etw.id_usuario is null " &
                             ")) order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas

            End If
            If tipo_consulta = 3 Then
                Sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_Seleccion,etw.estado_prioridad as prioridad,DAT.FLUJO_INTERNO_WF,etw.id_usuario," & campo_lista_tramite & ",etw.Estado_Tarea as ESTADO,wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                             " estados_tarea_workflow etw " &
                             " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " &
                             " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & fitro_estado_modulo_correspondencia & " ) " &
                             " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                             " left outer join  wf_registro_actividaes_flujos_trabajo as wraft on (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=etw.ID_ACTIVIDAD_FLUJO_TRABAJO)" &
                             " where (" & valor_consulta & ") " &
                             " and etw.id_actividad=" & Id_actividad &
                             "  and etw.fecha_fin is null  and ESTADO_ACTIVIDA_MODULO_RAD=0  " & limite_fecha & " or etw.id_usuario=" & Id_Usuario_Workflow &
                             " order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas

            End If
            If tipo_consulta = 4 Then
                Sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_Seleccion,etw.estado_prioridad as prioridad,DAT.FLUJO_INTERNO_WF,etw.id_usuario," & campo_lista_tramite & ",etw.Estado_Tarea as ESTADO,wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                             " estados_tarea_workflow etw " &
                             " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " &
                             " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & fitro_estado_modulo_correspondencia & " ) " &
                             " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                             " left outer join  wf_registro_actividaes_flujos_trabajo as wraft on (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=etw.ID_ACTIVIDAD_FLUJO_TRABAJO)" &
                             " where (" & valor_consulta & ") " &
                             " and etw.id_actividad=" & Id_actividad &
                             "  and etw.fecha_fin is null  and ESTADO_ACTIVIDA_MODULO_RAD=0 " & " or etw.id_usuario=" & Id_Usuario_Workflow &
                             " order by " & colum_order_name & " " & order_colum & limite_numero_lista_tareas

            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF") = tipo_consulta
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF") = valor_consulta
            HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE_WF") = campo_lista_tramite & ",etw.Estado_Tarea"
            HttpContext.Current.Session.Item("CAHCE_CONSULTA_INERT_WF") = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_Seleccion,etw.estado_prioridad as prioridad,DAT.FLUJO_INTERNO_WF,etw.id_usuario," &
                    campo_lista_tramite & ",etw.Estado_Tarea as ESTADO,wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " &
                    " estados_tarea_workflow etw " &
                    " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " &
                    " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA )" &
                    " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                    " left outer join  wf_registro_actividaes_flujos_trabajo as wraft on (wraft.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=etw.ID_ACTIVIDAD_FLUJO_TRABAJO) " &
                    " where "
            Dim spli_colum() As String = {"OPCIONES", "etw.Inicio_Tareas_Workflow_id_Tarea", "etw.fecha_Seleccion", "etw.estado_prioridad", "DAT.FLUJO_INTERNO_WF", "etw.id_usuario"}
            Dim spli_campos_() As String = campo_lista_tramite.Split(",")
            For i As Integer = 0 To spli_campos_.Length - 1
                Dim nuevo_indice As Integer = spli_colum.Length
                ReDim Preserve spli_colum(nuevo_indice)
                spli_colum(nuevo_indice) = spli_campos_(i)
            Next
            Dim nuevo_indice_ As Integer = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FLUJO_TAREA"
            nuevo_indice_ = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FECHAINICIOTRAMITE"
            nuevo_indice_ = spli_colum.Length
            ReDim Preserve spli_colum(nuevo_indice_)
            spli_colum(nuevo_indice_) = "FECHAFINALTRAMITE"
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido_hi_WF") = spli_colum
            HttpContext.Current.Session.Item("Sort_matri_colum_compartido_hi") = spli_colum
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Listar_tareas_workflow = "Funcion Listar_tareas_workflow Error :  " & Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = Datset
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                Hidden_cantidad_registros.value = Datset.Tables(0).Rows.Count
                reflabel.Text = "(0)"
                HttpContext.Current.Session.Item("NUMEROACTIVIDADES") = 0
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                hideselecion.value = "-1"
                update_gred.Update()
                update_title.Update()
                Listar_tareas_workflow = "YES"
                Exit Function
            Else
                Hidden_cantidad_registros.value = Datset.Tables(0).Rows.Count
                HttpContext.Current.Session.Item("NUMEROACTIVIDADES") = Datset.Tables(0).Rows.Count
                reflabel.Text = "(" & Datset.Tables(0).Rows.Count & ")"
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                update_title.Update()
                grediview.DataBind()
                update_gred.Update()
                Result = Me.pluguin_lista_tarea_workflow(Datset,
                                                        grediview,
                                                        update_gred,
                                                        colum_order_name,
                                                        order_colum,
                                                        valor_etado_tarea)
                Listar_tareas_workflow = Result
                Exit Function
            End If
        Catch ex As Exception
            Listar_tareas_workflow = "Inconsistencia general funcion Listar_Actividades_workflow " & ex.Message
        End Try
    End Function
    Function pluguin_lista_tarea_workflow(ByVal Datset As DataSet,
                                          ByRef grediview As GridView,
                                          ByRef update_gred As UpdatePanel,
                                          ByVal campo_orden As String,
                                          ByVal orden As String,
                                          ByVal valor_etado_tarea As String) As String
        Try
            Dim idex_colum_fecha_fin As Integer = -1
            Dim index_estado As Integer = -1
            Dim index_id_usuario As Integer = -1
            Dim index_colum_descripcion As Integer = -1
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                If Datset.Tables(0).Columns(z).ColumnName = "FECHAFINALTRAMITE" Then
                    idex_colum_fecha_fin = z + 1
                End If
                If Datset.Tables(0).Columns(z).ColumnName = "ESTADO" Then
                    index_estado = z + 1
                End If
                If Datset.Tables(0).Columns(z).ColumnName = "id_usuario" Then
                    index_id_usuario = z + 1
                End If
                If Datset.Tables(0).Columns(z).ColumnName = "ACTIVIDAD" Then
                    index_colum_descripcion = z + 1
                End If
            Next
            If index_id_usuario = -1 Then
                pluguin_lista_tarea_workflow = "Imposible encontrar el campo (id_usuario)"
                Exit Function
            End If
            If index_estado = -1 Then
                pluguin_lista_tarea_workflow = "Imposible encontrar el campo (ESTADO)"
                Exit Function
            End If
            If idex_colum_fecha_fin = -1 Then
                pluguin_lista_tarea_workflow = "Imposible encontrar el campo (FECHAFINALTRAMITE)"
                Exit Function
            End If
            grediview.DataBind()
            update_gred.Update()
            For i As Integer = 0 To grediview.Rows.Count - 1
                grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                grediview.Rows(i).Attributes.Add("data-workflow-task-id", grediview.Rows(i).Cells(1).Text.ToString())
                Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                ihtml.Style.Add("color", "white")
                ihtml.Attributes.Add("class", "fas fa-folder-open fa-lg")
                Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                ahtml.Attributes.Add("onclick", "prevent_lista_tareas(Event,this);")
                ahtml.Attributes.Add("title", "Ver documentos")
                ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                ahtml.Attributes.Add("tip_event", "documentos_tarea_list")
                ahtml.Style.Add("margin-left", "3px")
                ahtml.Controls.Add(ihtml)
                divhtml.Controls.Add(ahtml)
                If grediview.Rows(i).Cells(4).Text = 1 Then
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "far fa-info fa-lg")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent_lista_tareas(Event,this);")
                    ahtml.Attributes.Add("title", "Detalle tarea")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "detalle_radicado_tarea")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                End If
                ihtml = New HtmlControls.HtmlGenericControl("i")
                ihtml.Style.Add("color", "white")
                Dim valor_gestion As String = ""
                Dim tipo_tarea As String = ""
                'Asigna si la tarea es de grupo o de usuario
                If grediview.Rows(i).Cells(index_id_usuario).Text = "&nbsp;" Then
                    tipo_tarea = " grupo"
                    ihtml.Attributes.Add("class", "fal fa-user-friends fa-lg")
                Else
                    tipo_tarea = " usuario"
                    ihtml.Attributes.Add("class", "fal fa-user fa-lg")
                End If
                ahtml = New HtmlControls.HtmlGenericControl("a")
                If grediview.Rows(i).Cells(index_estado).Text = "0" Then
                    grediview.Rows(i).Attributes.Add("Class", "font-weight-bold")
                    valor_gestion = "Asignar y gestionar tarea de " & tipo_tarea
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm dmt_sel_imput")
                Else
                    grediview.Rows(i).Attributes.Add("Class", "font-weight-light")
                    valor_gestion = "Retomar tarea de " & tipo_tarea & " en proceso"
                    ahtml.Attributes.Add("Class", "btn btn-warning btn-sm dmt_sel_imput")
                End If

                ahtml.Attributes.Add("onclick", "prevent_lista_tareas(Event,this);")
                ahtml.Attributes.Add("title", valor_gestion)
                ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                ahtml.Attributes.Add("tip_event", "seleccion_tarea_wf")
                ahtml.Style.Add("margin-left", "3px")
                ahtml.Controls.Add(ihtml)
                divhtml.Controls.Add(ahtml)
                divhtml.Style.Add("display", "inline-flex")
                grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                If grediview.Rows(i).Cells(index_estado).Text = "0" Then
                    grediview.Rows(i).Cells(index_estado).Text = "Espera"
                End If
                If grediview.Rows(i).Cells(index_estado).Text = "1" Then
                    grediview.Rows(i).Cells(index_estado).Text = valor_etado_tarea
                End If
                For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                    If z > 0 Then
                        grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                        grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(Event,this);")
                    End If
                Next
                'Agrega link asignacion tarea
                If index_colum_descripcion <> -1 Then
                    grediview.Rows(i).Cells(index_colum_descripcion).Attributes.Add("tip_event", "seleccion_tarea_wf")
                    grediview.Rows(i).Cells(index_colum_descripcion).Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    grediview.Rows(i).Cells(index_colum_descripcion).Attributes.Add("onclick", "prevent_lista_tareas(Event,this);")
                    grediview.Rows(i).Cells(index_colum_descripcion).Attributes.Add("title", valor_gestion)
                    grediview.Rows(i).Cells(index_colum_descripcion).Style.Add("color", "blue")
                End If
            Next
            Dim Refclas As New ClassGredview
            Dim Result As String = ""
            Result = Refclas.add_clase_acender_decender(campo_orden,
                                                        HttpContext.Current.Session.Item("Sort_matri_colum_compartido_hi_WF"),
                                                        orden,
                                                        grediview)
            If Result <> "YES" Then
                pluguin_lista_tarea_workflow = "Error listando orden de columnas funcion  Cahche_lista_tareas_workflow detalle : " & Result
                Exit Function
            End If
            pluguin_lista_tarea_workflow = "YES"
            Exit Function
        Catch ex As Exception
            pluguin_lista_tarea_workflow = "Inconsistencia general funcion pluguin_lista_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Cahche_pagin_sorting_lista_tareas_workflow(ByRef grediview As GridView,
                                                        ByRef update_gred As UpdatePanel,
                                                        ByVal valida_sort As Integer,
                                                        ByVal campo_orden As String,
                                                        ByVal orden As String) As String
        Try
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            Datset = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
            Dim seleccion_util_pendiente As String = ""
            Dim valor_etado_tarea As String = ""
            If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 1 Then
                seleccion_util_pendiente = " and etw.estado_tarea=0 "
                valor_etado_tarea = "Pendiente"
            Else
                valor_etado_tarea = "En proceso"
            End If
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                update_gred.Update()
                Cahche_pagin_sorting_lista_tareas_workflow = "YES"
                Exit Function
            Else
                If valida_sort = 1 Then
                    Dim dtTable As DataTable = New DataTable()
                    dtTable = Datset.Tables(0)
                    Dim dv As DataView = dtTable.DefaultView
                    dv.Sort = campo_orden + " " + orden
                    Dim dtSorted As DataTable = New DataTable()
                    dtSorted = dv.ToTable()
                    grediview.DataSource = dtSorted
                    Datset.Tables.Clear()
                    Datset.Tables.Add(dtSorted)
                    HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = Datset
                Else
                    grediview.DataSource = Datset
                End If
                Result = Me.pluguin_lista_tarea_workflow(Datset,
                                                         grediview,
                                                         update_gred,
                                                         campo_orden,
                                                         orden,
                                                         valor_etado_tarea)
                Cahche_pagin_sorting_lista_tareas_workflow = Result
                Exit Function
            End If
        Catch ex As Exception
            Cahche_pagin_sorting_lista_tareas_workflow = "Inconsistencia general funcion Cahche_pagin_sorting_lista_tareas_workflow " & ex.Message
        End Try
    End Function
    Function Cahche_lista_tareas_workflow(ByRef grediview As GridView,
                                          ByRef update_gred As UpdatePanel,
                                          ByVal valida_sort As Integer,
                                          ByVal campo_orden As String,
                                          ByVal orden As String,
                                          ByRef label As Label,
                                          ByRef update_title As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE")
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE") = vbObject
            End If
            Datset = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
            Dim seleccion_util_pendiente As String = ""
            Dim valor_etado_tarea As String = ""
            If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 1 Then
                seleccion_util_pendiente = " and etw.estado_tarea=0 "
                valor_etado_tarea = "Pendiente"
            Else
                valor_etado_tarea = "En proceso"
            End If
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            If Datset.Tables(0).Rows.Count = 0 Then
                'HttpContext.Current.Session.Item("NUMEROACTIVIDADES") = 0
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                update_gred.Update()
                label.Text = "(0)"
                update_title.Update()
                Cahche_lista_tareas_workflow = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset
                grediview.DataBind()
                update_gred.Update()
                label.Text = "(" & Datset.Tables(0).Rows.Count & ")"
                update_title.Update()
                Result = Me.pluguin_lista_tarea_workflow(Datset,
                                                        grediview,
                                                        update_gred,
                                                        campo_orden,
                                                        orden,
                                                        valor_etado_tarea)
                Cahche_lista_tareas_workflow = Result
                Exit Function

            End If
        Catch ex As Exception
            Cahche_lista_tareas_workflow = "Inconsistencia general funcion Cahche_lista_tareas_workflow " & ex.Message
        End Try
    End Function
    Function Cahche_Search_lista_tareas_workflow(ByRef grediview As GridView,
                                                 ByRef update_gred As UpdatePanel,
                                                 ByVal valida_sort As Integer,
                                                 ByVal campo_orden As String,
                                                 ByVal orden As String,
                                                 ByVal valor As String,
                                                 ByRef label As Label,
                                                 ByRef update_title As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Datset_ As DataSet = New DataSet("estados_tarea_workflow_")
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE")
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE") = vbObject
            End If
            Datset = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
            Dim seleccion_util_pendiente As String = ""
            Dim valor_etado_tarea As String = ""
            If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 1 Then
                seleccion_util_pendiente = " and etw.estado_tarea=0 "
                valor_etado_tarea = "Pendiente"
            Else
                valor_etado_tarea = "En proceso"
            End If
            Datset_.Tables.Add("cahce_estados_tarea_workflow")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Datset_.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    If UCase(valor.ToString) = UCase(Datset.Tables(0).Rows(i).Item(z).ToString) Then
                        Datset_.Tables(0).ImportRow(Datset.Tables(0).Rows(i))
                    End If
                Next
            Next
            If Datset_.Tables(0).Rows.Count = 0 Then
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                update_gred.Update()
                label.Text = "(0)"
                update_title.Update()
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE") = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = Datset_
                Cahche_Search_lista_tareas_workflow = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset_
                grediview.DataBind()
                update_gred.Update()
                label.Text = "(" & Datset_.Tables(0).Rows.Count & ")"
                update_title.Update()
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE") = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = Datset_
                Result = Me.pluguin_lista_tarea_workflow(Datset,
                                                         grediview,
                                                         update_gred,
                                                         campo_orden,
                                                         orden,
                                                         valor_etado_tarea)
                Cahche_Search_lista_tareas_workflow = Result
                Exit Function
            End If
        Catch ex As Exception
            Cahche_Search_lista_tareas_workflow = "Inconsistencia general funcion Cahche_Search_lista_tareas_workflow " & ex.Message
        End Try
    End Function
    Function Cache_filtra_lista_tareas_workflow(ByRef grediview As GridView,
                                                ByRef update_gred As UpdatePanel,
                                                ByVal valida_sort As Integer,
                                                ByVal campo_orden As String,
                                                ByVal orden As String,
                                                ByVal valor As String,
                                                ByRef label As Label,
                                                ByRef update_title As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Datset_filtro As DataSet = New DataSet("estados_tarea_workflow_")
            Dim Dat_set_zero As DataSet = New DataSet("estados_tarea_workflow_zero")
            If HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE").GetType.ToString = "System.Data.DataSet" Then
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE")
                Datset = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
            Else
                HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE") = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
                Datset = HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF")
            End If
            Dim seleccion_util_pendiente As String = ""
            Dim valor_etado_tarea As String = ""
            If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 1 Then
                seleccion_util_pendiente = " and etw.estado_tarea=0 "
                valor_etado_tarea = "Pendiente"
            Else
                valor_etado_tarea = "En proceso"
            End If
            Datset_filtro.Tables.Add("cahce_estados_tarea_workflow")
            Dat_set_zero.Tables.Add("cahce_estados_tarea_workflow_zero")
            For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                Datset_filtro.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
                Dat_set_zero.Tables(0).Columns.Add(Datset.Tables(0).Columns(z).ColumnName, Datset.Tables(0).Columns(z).DataType)
            Next
            For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                Select Case valor
                    Case "tareas de grupo"
                        If Datset.Tables(0).Rows(i).Item("id_usuario") Is DBNull.Value Then
                            Datset_filtro.Tables(0).ImportRow(Datset.Tables(0).Rows(i))
                        End If
                    Case "tareas de usuario"
                        If Not Datset.Tables(0).Rows(i).Item("id_usuario") Is DBNull.Value Then
                            Datset_filtro.Tables(0).ImportRow(Datset.Tables(0).Rows(i))
                        End If
                    Case "tareas en proceso"
                        If Val(Datset.Tables(0).Rows(i).Item("ESTADO")) = 1 Then
                            Datset_filtro.Tables(0).ImportRow(Datset.Tables(0).Rows(i))
                        End If
                    Case "tareas en espera"
                        If Val(Datset.Tables(0).Rows(i).Item("ESTADO")) = 0 Then
                            Datset_filtro.Tables(0).ImportRow(Datset.Tables(0).Rows(i))
                        End If
                    Case Else
                        Datset_filtro.Tables(0).ImportRow(Datset.Tables(0).Rows(i))
                        Exit Select
                End Select
            Next
            HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF") = Datset_filtro
            If Datset_filtro.Tables(0).Rows.Count = 0 Then
                label.Text = "(0)"
                Dat_set_zero.Tables(0).Rows.Add(Dat_set_zero.Tables(0).NewRow)
                grediview.DataSource = Dat_set_zero
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                update_gred.Update()
                update_title.Update()
                Cache_filtra_lista_tareas_workflow = "YES"
                Exit Function
            Else
                grediview.DataSource = Datset_filtro
                grediview.DataBind()
                update_gred.Update()
                label.Text = "(" & Datset_filtro.Tables(0).Rows.Count & ")"
                update_title.Update()
                Result = Me.pluguin_lista_tarea_workflow(Datset,
                                                        grediview,
                                                        update_gred,
                                                        campo_orden,
                                                        orden,
                                                        valor_etado_tarea)
                Cache_filtra_lista_tareas_workflow = Result
                Exit Function

            End If
        Catch ex As Exception
            Cache_filtra_lista_tareas_workflow = "Inconsistencia general funcion Cache_filtra_lista_tareas_workflow " & ex.Message
        End Try
    End Function
    Function Lista_numero_tareas_workflow(ByVal tipo_consulta As Integer, _
                                          ByVal Id_actividad As Integer, _
                                          ByVal Id_Usuario_Workflow As Integer, _
                                          ByVal sql_select As String, _
                                          ByVal nombre_ruta As String, _
                                          ByRef numero_tareas As Integer) As String
        Try
            Dim Numero_Actividades As Integer = 0
            Dim Fecha_Ini As String = ""
            Dim Fecha_Fin As String = ""
            Dim Sql_Consulta_Listado As String = "Select "
            Dim Sql_consulta As String = ""
            Dim Result As String = ""
            Dim limite_fecha As String = ""
            Dim limite_numero_lista_tareas As String = ""
            '---------------------------------
            'Consultando configuracion usuario
            '---------------------------------
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim ref_Class_configuracion_usuario As New Class_configuracion_usuario
            Result = ref_Class_configuracion_usuario.Retorna_parametros_limite_actividades_fecha_tareas(Numero_Actividades, _
                                                                                                        Fecha_Ini, _
                                                                                                        Fecha_Fin, _
                                                                                                        HttpContext.Current.Session("Id_Usuario_Workflow"))
            If Result <> "YES" Then
                Lista_numero_tareas_workflow = Result
                Exit Function
            End If
            If Fecha_Ini <> "" And Fecha_Fin <> "" Then
                ClassGestionFechas.formato_fecha_estructura(Fecha_Ini)
                ClassGestionFechas.formato_fecha_estructura(Fecha_Fin)
                limite_fecha = " and CAST(etw.fecha_inicio AS Date) between '" & Fecha_Ini & _
                "' and '" & Fecha_Fin & "'"
            End If
            If Numero_Actividades <> 0 Then
                limite_numero_lista_tareas = " LIMIT " & Numero_Actividades
            Else
                limite_numero_lista_tareas = " LIMIT 2000"
            End If
            '----------------------------
            'Tipo consulta escript
            '---------------------------
            If tipo_consulta = 1 Then
                Sql_consulta = sql_select & _
                  " where (etw.id_actividad=" & Id_actividad & _
                  "  and etw.fecha_fin is null    " & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow & _
                  ") or ( etw.id_actividad=" & Id_actividad & "  and etw.fecha_fin is null    and etw.id_usuario is null" & limite_fecha & _
                  ")  " & limite_numero_lista_tareas
            Else
                Sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea    from " & _
                            " estados_tarea_workflow etw " & _
                            " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " & _
                            " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA  and estado_modulo_radicado = 0 )" & _
                            " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" & _
                            " where (etw.id_actividad=" & Id_actividad & _
                            "  and etw.fecha_fin is null    " & limite_fecha & " and etw.id_usuario=" & Id_Usuario_Workflow & _
                            ") or ( etw.id_actividad=" & Id_actividad & "  and etw.fecha_fin is null    and etw.id_usuario is null" & limite_fecha & _
                    ")   " & limite_numero_lista_tareas
            End If
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ra_cd_usuarios_documentos_compartidos")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                numero_tareas = 0
                Lista_numero_tareas_workflow = "Funcion LLista_numero_tareas_workflow Error :  " & Result
                Exit Function
            Else
                Lista_numero_tareas_workflow = "YES"
                numero_tareas = Datset.Tables(0).Rows.Count
            End If
        Catch ex As Exception
            Lista_numero_tareas_workflow = "Inconsistencia general función Lista_numero_tareas_workflow " & ex.Message
        End Try
    End Function
    'Function Listar_tareas_workflow(ByVal RutActividad As String, _
    '                               ByVal id_grupo As String, _
    '                               ByVal ID_USARIO As String, _
    '                               ByRef table As Table, _
    '                               ByRef paneladrotar As Panel, _
    '                               ByRef scripma As GridView) As String
    '    '-----------------------------------------------------------------
    '    'Funcion   :Listar Actividades Workflow
    '    'Ingeniero : Miguel Angel Urueta Miranda
    '    'Fecha 2009-07-27
    '    'Procedimiento : funcion del sistema que lista todos las tareas
    '    'asignadas a la actividad las cuales heredan los usuarios
    '    'realaciondos con la actividad
    '    '-----------------------------------------------------------------

    '    Try
    '        Dim Nombre_Ruta As String = ""
    '        Dim Id_Actividad As String = ""
    '        Dim Numero_Actividades As String = ""
    '        Dim Fecha_Ini As String = ""
    '        Dim Fecha_Fin As String = ""
    '        Dim Sql_Consulta_Listado As String = "Select "
    '        Dim Matri_Campos_Lista() As String
    '        Dim Sql_consulta As String = ""
    '        '--------------------------------------------------------
    '        'Consulta las tareas posibles para listar en actividades
    '        'es espera
    '        '--------------------------------------------------------
    '        Sql_consulta = "Select NOMBRE_CAMPO from " & _
    '        " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & RutActividad & _
    '        " AND LISTA_TAREA=1 order by id_campo"
    '        Dim Result As String = ""
    '        Dim ref As New conect.Dbase_Conction_Mysql
    '        Dim Datset As DataSet = New DataSet("USUARIO_WORKFLOW")
    '        Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
    '        If Result <> "YES" Then
    '            Listar_tareas_workflow = " Error # 01 Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA" & Result
    '            Exit Function
    '        End If
    '        If Datset.Tables(0).Rows.Count = 0 Then

    '        Else
    '            Erase Matri_Campos_Lista
    '            Dim i As Integer = 0

    '            For z As Integer = 0 To Datset.Tables(0).Rows.Count - 1
    '                ReDim Preserve Matri_Campos_Lista(i)
    '                Matri_Campos_Lista(i) = Datset.Tables(0).Rows(z).Item(0).ToString
    '                Sql_Consulta_Listado = Sql_Consulta_Listado & "DAT." & _
    '                Datset.Tables(0).Rows(z).Item(0).ToString & ","
    '                i = i + 1
    '            Next


    '        End If
    '        Sql_Consulta_Listado = Sql_Consulta_Listado & "wf_fl.NOMBRE_FLUJO_TRABAJO as FLUJO_TAREA, etw.fecha_inicio,etw.estado_prioridad as prioridad,etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea from " & _
    '        "estados_tarea_workflow etw "
    '        '-------------------------------
    '        'Consulta nombre de ruta
    '        '-------------------------------
    '        Dim Ref_calss_wf_ruta As New Class_worflow_rutas
    '        Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(RutActividad, _
    '                                                                 Nombre_Ruta)
    '        If Result <> "YES" Then
    '            Listar_tareas_workflow = "Error #02 Consultando Nombre de Ruta " + Result
    '            Exit Function

    '        End If
    '        '-------------------------------
    '        'Consulta id actividad
    '        '------------------------------
    '        Result = ""
    '        Dim ref_ref_Class_grupos_workflow As New Class_grupos_workflow
    '        Result = ref_ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(Id_Actividad, _
    '                                                                                  Val(id_grupo))
    '        If Result <> "YES" Then
    '            Listar_tareas_workflow = "Error #03 Consultando Id Actividad " + Result
    '            Exit Function

    '        End If
    '        Dim ref_clas_seleccion As New Classselecciotarea
    '        Dim tipo_actividad As String = ""
    '        Result = ref_clas_seleccion.Lita_Tipo_Actividad_General(Id_Actividad, _
    '                                                              tipo_actividad)
    '        If Result <> "YES" Then
    '            Listar_tareas_workflow = "Error #03 Consultando el tipo de actividad " + Result
    '            Exit Function
    '        End If
    '        Dim fitro_estado_modulo_correspondencia As String = ""
    '        If tipo_actividad = "ENLASE" Then
    '            fitro_estado_modulo_correspondencia = ""
    '        Else
    '            fitro_estado_modulo_correspondencia = " and estado_modulo_radicado = 0"
    '        End If
    '        Sql_Consulta_Listado = Sql_Consulta_Listado & "inner join dat_adic_tar" & Nombre_Ruta & " as  DAT on " & _
    '        "(etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & fitro_estado_modulo_correspondencia & " ) " & _
    '        " inner join  configuracion_gabinete as cg on" & _
    '        " (DAT.id_gabinete=cg.id_gabinete) " & _
    '        " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)"
    '        If scripma.ID <> "GridView1" Then
    '            Sql_Consulta_Listado = Sql_Consulta_Listado & "where etw.id_actividad=" & Id_Actividad & _
    '            " and etw.fecha_fin is null and etw.fecha_Seleccion is null and etw.id_usuario is null and etw.estado_tarea=0"
    '        Else
    '            Sql_Consulta_Listado = Sql_Consulta_Listado & "where etw.id_actividad=" & Id_Actividad & _
    '            " and etw.fecha_Seleccion is null and etw.fecha_fin is null and etw.id_usuario=" & ID_USARIO & " and etw.estado_tarea=0"
    '        End If
    '        '---------------------------------
    '        'Consultando configuracion usuario
    '        '---------------------------------
    '        Result = ""
    '        Result = Leer_Datos_Configuracion_Usuario(Numero_Actividades, _
    '                                                  Fecha_Ini, _
    '                                                  Fecha_Fin, _
    '                                                  ID_USARIO)
    '        If Result <> "YES" Then
    '            Listar_tareas_workflow = "Error #04 Consultando configuracion usuario " + Result
    '            Exit Function
    '        End If
    '        If Fecha_Ini <> "" And Fecha_Fin <> "" Then
    '            Fecha_Ini = Formatear_Fecha_Mysql(Fecha_Ini)
    '            Fecha_Fin = Formatear_Fecha_Mysql(Fecha_Fin)
    '            Sql_Consulta_Listado = Sql_Consulta_Listado & " and CAST(etw.fecha_inicio AS Date) between '" & Fecha_Ini & _
    '            "' and '" & Fecha_Fin & "'"
    '        End If
    '        Sql_Consulta_Listado = Sql_Consulta_Listado & "  order by  etw.estado_prioridad desc,etw.fecha_inicio asc  "
    '        If Numero_Actividades <> "" Then
    '            Sql_Consulta_Listado = Sql_Consulta_Listado & " LIMIT " & Numero_Actividades
    '        Else
    '            Sql_Consulta_Listado = Sql_Consulta_Listado & " LIMIT 2000"
    '        End If
    '        '------------------------------------
    '        'Realiza la consulta para listar las
    '        'tareas disponibles para el usuario
    '        '------------------------------------
    '        Result = ref.SELECTION_SELECT_FIELD(Sql_Consulta_Listado, Datset)
    '        If Result <> "YES" Then
    '            Listar_tareas_workflow = "Error #05 Imposible Encontrar datos para listar" & Result
    '            Exit Function
    '        End If
    '        If Datset.Tables(0).Rows.Count = 0 Then
    '            scripma.DataSource = Datset
    '            scripma.DataBind()
    '            Listar_tareas_workflow = ""
    '            Exit Function
    '        Else
    '            scripma.DataSource = Datset
    '            scripma.DataBind()
    '            For i As Integer = 0 To scripma.Rows.Count
    '                Dim tex As String = scripma.Rows(i).RowIndex
    '                scripma.Rows(i).Attributes.Add("id", tex.ToString)
    '                Dim id_value As String = scripma.Rows(i).Cells(scripma.Rows(i).Cells.Count - 1).Text
    '                scripma.Rows(i).Attributes.Add("id_sel", id_value)
    '            Next
    '            Dim Res As String = ""
    '            Res = Listar_Orden_Prioridad(scripma, Datset)
    '            If Res <> "YES" Then
    '                Listar_tareas_workflow = Res
    '                Exit Function
    '            End If
    '            Listar_tareas_workflow = "YES"
    '            Exit Function
    '        End If
    '        Listar_tareas_workflow = "YES"

    '    Catch ex As Exception
    '        Listar_tareas_workflow = "Inconsistencia general funcion Listar_Actividades_workflow " & ex.Message
    '    End Try
    'End Function
    Function Listar_Orden_Prioridad(ByRef Grid As GridView, _
                                    ByRef Datgrid As DataSet) As String
        '******************************************************
        'Funcion : Listar_Orden_Prioridad
        'Descripcion: El sistema lista colores respecto a la
        'prioridad
        'Fecha: 2012-11-15
        'Ing Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Dim IDEX As Integer = -1
            Dim remat As String = ""
            For k As Integer = 0 To Datgrid.Tables(0).Columns.Count - 1
                remat = remat & "-" & Datgrid.Tables(0).Columns(k).ColumnName
                If Datgrid.Tables(0).Columns(k).ColumnName = "prioridad" Then
                    IDEX = k
                    Exit For
                End If
            Next
            Dim cel As Integer = Grid.Rows(0).Cells.Count
            For i As Integer = 0 To Grid.Rows.Count - 1
                Dim Valor As Integer = 0
                Valor = Grid.Rows(i).Cells(IDEX + 1).Text
                If Valor = 5 Then
                    Grid.Rows(i).ForeColor = Color.Red
                End If
                If Valor = 4 Then
                    Grid.Rows(i).ForeColor = Color.Blue
                End If
                If Valor = 3 Then
                    Grid.Rows(i).ForeColor = Color.Green
                End If
            Next
            Listar_Orden_Prioridad = "YES"
        Catch ex As Exception
            Listar_Orden_Prioridad = "Funcion Listar orden prioridad" & ex.Message
        End Try
    End Function

    Function Retorna_Existencia_flujo_workflow(ByVal Parametro_Consulta As String, _
                                               ByRef existencia As String, _
                                               ByRef id_tarea_workflow As Object) As String
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("USUARIO_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Existencia_flujo_workflow = "Error #03 Consultando en tabla " & "GRUPOS_WORKFLOW " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_tarea_workflow = 0
                existencia = "NO"
                Retorna_Existencia_flujo_workflow = "YES"
                Exit Function
            Else
                id_tarea_workflow = Datset.Tables(0).Rows(0).Item(1)
                existencia = "YES"
                Retorna_Existencia_flujo_workflow = "YES"
            End If
        Catch ex As Exception
            Retorna_Existencia_flujo_workflow = "Inconsistencia función Retorna_Existencia_flujo_workflow " & ex.Message
        End Try
    End Function


    Function actualiza_campos_workflow(ByVal sql_actualiza As String) As String
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(sql_actualiza)
            If Result <> "YES" Then
                actualiza_campos_workflow = "Error Actualizando campos workflow " & sql_actualiza _
               & " Descripcion Error  " & Result
                Exit Function
            Else
                actualiza_campos_workflow = "YES"
                Exit Function
            End If

        Catch ex As Exception
            actualiza_campos_workflow = "Inconsistencia función actualiza_campos_workflow " & ex.Message
        End Try
    End Function
    Public Function Intervalo_Alarma_Usuario(ByVal iD_Usuario As Integer, ByRef Parametro As Integer) As String
        '*********************************************************
        'Function :  Solicita Intervalo Alarma Usuario
        'Fecha    : 2010-06-08
        'Ing      : Miguel Angel Urueta Miranda
        'Proced   : Solicta el itervalo de jecucion de alarmas
        'Parameter: Id uaurio, Parametro de confirmacion
        '**********************************************************

        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("INTERVALO_ALARMAS_USUARIO")
            Dim Parametro_Consulta As String = "select INTERVALO from INTERVALO_ALARMAS_USUARIO " & _
            "WHERE USUARIO_WORKFLOW_IDU_SUARIO=" & iD_Usuario
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)

            If Result <> "YES" Then
                Intervalo_Alarma_Usuario = "# 02 Error Verificando solicitando intervalo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Intervalo_Alarma_Usuario = -"YES"
                Parametro = -1
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).Item(0) = -1 Then
                    Intervalo_Alarma_Usuario = -"YES"
                    Parametro = -1
                Else
                    Parametro = Datset.Tables(0).Rows(0).Item(0) * 100000
                    Intervalo_Alarma_Usuario = "YES"
                    Return Intervalo_Alarma_Usuario
                    Exit Function
                End If

            End If
            Intervalo_Alarma_Usuario = "YES"
            Return Intervalo_Alarma_Usuario
        Catch ex As Exception
            Intervalo_Alarma_Usuario = ex.Message
        End Try
    End Function
End Class
