Imports Newtonsoft.Json

Public Class class_wf_gestion_tarea_usuario_stru
    Property id_gestion_tarea_usuario As Integer
    Property wf_gestion_tipos_id_tipo_gestion As Integer
    Property estados_tarea_workflow_id_Estado As Long
    Property inicio_tareas_workflow_id_Tarea As Long
    Property usuario_workflow_idU_suario As Integer
    Property content_gestion As String
    Property fecha_registro As String
    Property Estado_envio_correo As Integer
    Property Class_wf_ges_task_user_dir_mail As List(Of class_wf_ges_task_user_dir_mail)
    Property error_result As String
    Property Error_send_mail As String
End Class
Public Class class_wf_ges_task_user_dir_mail
    Property vaule As String
    Property text As String
End Class
Public Class Class_wf_gestion_tarea_usuario
    Function Solicita_estructura_registro_gestion(ByVal id_registro_gestion As Integer,
                                                  ByRef Class_wf_gestion_tarea_usuario_stru As class_wf_gestion_tarea_usuario_stru) As String
        '-------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de la gestión con la identificación del regis
        '          gistro de gestión
        '          
        '       
        '-------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'id_registro_gestion : Representa la identificación del registro de la gestión   
        '                                      
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_wf_gestion_tarea_usuario_stru : Retorna la estructura del registro de
        '                                      la gestión con los datos
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-09-23
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim SQL_consulta As String = "Select id_gestion_tarea_usuario,wf_gestion_tipos_id_tipo_gestion," &
                "estados_tarea_workflow_id_Estado,inicio_tareas_workflow_id_Tarea,usuario_workflow_idU_suario," &
                "content_gestion,fecha_registro,Estado_envio_correo " &
                " from wf_gestion_tarea_usuario where id_gestion_tarea_usuario=" & id_registro_gestion
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_gestion_tarea_usuario")
            Result = ref.SELECTION_SELECT_FIELD(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_registro_gestion = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_registro_gestion = "Imposible encontrar registro para el identificador de gestión (" & id_registro_gestion & ")"
                Exit Function
            Else
                Class_wf_gestion_tarea_usuario_stru.id_gestion_tarea_usuario = Datset.Tables(0).Rows(0).Item("id_gestion_tarea_usuario")
                Class_wf_gestion_tarea_usuario_stru.wf_gestion_tipos_id_tipo_gestion = Datset.Tables(0).Rows(0).Item("wf_gestion_tipos_id_tipo_gestion")
                Class_wf_gestion_tarea_usuario_stru.estados_tarea_workflow_id_Estado = Datset.Tables(0).Rows(0).Item("estados_tarea_workflow_id_Estado")
                Class_wf_gestion_tarea_usuario_stru.inicio_tareas_workflow_id_Tarea = Datset.Tables(0).Rows(0).Item("inicio_tareas_workflow_id_Tarea")
                Class_wf_gestion_tarea_usuario_stru.usuario_workflow_idU_suario = Datset.Tables(0).Rows(0).Item("usuario_workflow_idU_suario")
                Class_wf_gestion_tarea_usuario_stru.content_gestion = Datset.Tables(0).Rows(0).Item("content_gestion")
                Class_wf_gestion_tarea_usuario_stru.fecha_registro = Datset.Tables(0).Rows(0).Item("fecha_registro")
                Class_wf_gestion_tarea_usuario_stru.Estado_envio_correo = Datset.Tables(0).Rows(0).Item("Estado_envio_correo")
                Solicita_estructura_registro_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_registro_gestion = "Inconsistencia general funcion Solicita_estructura_registro_gestion " & ex.Message
        End Try
    End Function
    Function Actualza_registro_gestion_al_usuario(ByVal Class_wf_gestion_tarea_usuario_stru As class_wf_gestion_tarea_usuario_stru) As String
        '-------------------------------------------------------------------------------
        'Funcion : Reliza la actualización del registro de la gestión al usuario
        '          
        '       
        '-------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'class_wf_gestion_tarea_usuario_stru : Representa la estructura con los datos   
        '                                      del registro de la gestión
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_wf_gestion_tarea_usuario_stru : Retorna la estructura del registro de
        '                                      la gestión
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-09-23
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim time1al As String = Date.Now.ToString
            '//------Solicita fecha de registro-------
            ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
            Dim SQL_update As String = "update wf_gestion_tarea_usuario set content_gestion='" & Class_wf_gestion_tarea_usuario_stru.content_gestion & "'" &
            " where id_gestion_tarea_usuario=" & Class_wf_gestion_tarea_usuario_stru.id_gestion_tarea_usuario
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(SQL_update)
            If Result <> "YES" Then
                Actualza_registro_gestion_al_usuario = Result
                Exit Function
            End If
            Actualza_registro_gestion_al_usuario = "YES"
            Exit Function
        Catch ex As Exception
            Actualza_registro_gestion_al_usuario = "Inconsistencia general funcion Actualza_registro_gestion_al_usuario " & ex.Message
        End Try
    End Function
    Function Solicita_lista_head_gestion_al_usuario(ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo BOOTSTRAF para la lista de
        '          gestión al usuario
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Retorna la estructura de campos
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-23
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            item = New class_campos_table_bostra_table
            item.field = "operate"
            item.title = "GESTION"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 0
            item.clickToSelect = False
            item.visible_like_sql = 0
            item.align = "center"
            item.events = "window.operateEventsListaGestionUser"
            item.formatter = "operateFormattertablebootListaGestionUser"
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "id_gestion_tarea_usuario"
            item.field = "id_gestion_tarea_usuario"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "FECHA REGISTRO"
            item.field = "fecha_registro"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            Solicita_lista_head_gestion_al_usuario = "YES"
        Catch ex As Exception
            Solicita_lista_head_gestion_al_usuario = "inconsistencia general funcion Solicita_lista_head_gestion_al_usuario " & ex.Message
        End Try
    End Function
    Function Solicita_SQL_Consulta_gestion_al_usuario(ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                      ByVal tipo_consulta As Integer,
                                                      ByVal valor_consulta As String,
                                                      ByVal id_tarea_wf As Long,
                                                      ByVal id_usuario_wf As Integer,
                                                      ByVal table As String,
                                                      ByVal class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                                      ByRef consulta As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el sql de consulta gestion al usuario
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'consulta  : Retorna comando sql de consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-23
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim condicionsql As String = " where "
            Dim seleccampos As String = "Select "
            Dim campo_clase_documento As String = ""
            Dim campo_expediente As String = ""
            For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                If class_campos_table_bostra_table(i).viisble_sql = "1" Then
                    If seleccampos = "Select " Then
                        seleccampos = seleccampos & class_campos_table_bostra_table(i).field
                    Else
                        seleccampos = seleccampos & "," & class_campos_table_bostra_table(i).field
                    End If
                End If
            Next
            If tipo_consulta = 2 Then
                Dim likeigual As String = " Like"
                For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                    If class_campos_table_bostra_table.Item(i).visible_like_sql = 1 Then
                        If condicionsql = " where " Then
                            condicionsql = condicionsql & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & valor_consulta & "%'"
                        Else
                            condicionsql = condicionsql & " or " & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & valor_consulta & "%'"
                        End If
                    End If
                Next
            End If
            If tipo_consulta = 1 Then
                For i As Integer = 0 To Class_config_general_service.Count - 1
                    Dim campo_plantilla As String = "da." & Class_config_general_service.Item(i).name_campo
                    If Class_config_general_service.Item(i).tipo_campo = "DATE" Or Class_config_general_service.Item(i).tipo_campo = "INT" Then
                        'caso between
                        If Class_config_general_service.Item(i).value_campo <> "" And Class_config_general_service.Item(i).value_campo_beetwen <> "" Then
                            If Class_config_general_service.Item(i).tipo_campo = "DATE" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & " CAST(" & campo_plantilla & " AS DATE) " & " between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                Else
                                    condicionsql = condicionsql & " and " & " CAST(" & campo_plantilla & " AS DATE) " & "  between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                End If
                            Else
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & " between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "  between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                End If
                            End If
                        Else
                            'Caso primer campo
                            If Class_config_general_service.Item(i).value_campo <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo & "'"
                                End If
                            End If
                            'Caso segundo campo
                            If Class_config_general_service.Item(i).value_campo_beetwen <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                End If
                            End If
                        End If
                    Else
                        'Caso primer campo
                        If Class_config_general_service.Item(i).value_campo <> "" Then
                            If condicionsql = " where " Then
                                condicionsql = condicionsql & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo & "'"
                            Else
                                condicionsql = condicionsql & " and " & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo & "'"
                            End If
                        End If
                    End If
                Next
            End If
            If tipo_consulta = 3 Then
                condicionsql = " where inicio_tareas_workflow_id_Tarea=" & id_tarea_wf
                If id_usuario_wf <> 0 And id_usuario_wf <> -1 Then
                    condicionsql = condicionsql & " and usuario_workflow_idU_suario=" & id_usuario_wf
                End If
            End If
            Dim order_colum As String = "DESC"
            Dim colum_order_name As String = "id_gestion_tarea_usuario"
            Dim sqlfrom As String = " From " & table & " as da"
            consulta = seleccampos & " " & sqlfrom & " " & condicionsql & "   " & " ORDER BY " & colum_order_name & " " & order_colum & " LIMIT 5000"
            Solicita_SQL_Consulta_gestion_al_usuario = "YES"
        Catch ex As Exception
            Solicita_SQL_Consulta_gestion_al_usuario = "Inconsistencia general funcion Solicita_SQL_Consulta_gestion_al_usuario " & ex.Message
        End Try
    End Function
    Function Solicita_table_row_bot_gestion_al_usuario(ByVal consulta As String,
                                                       ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '         de gestión al usuario
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'consulta               : Representa la consulta en comando SQL
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_row_gabinete_generic  : Retorna la estructura de datos de la consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-23
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("wf_gestion_tarea_usuario")
            Result = ref.SELECTION_SELECT_FIELD(consulta, Datset)
            If Result <> "YES" Then
                Solicita_table_row_bot_gestion_al_usuario = "Funcion  Solicita_table_row_bot_gestion_al_usuario " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            Solicita_table_row_bot_gestion_al_usuario = "YES"
        Catch ex As Exception
            Solicita_table_row_bot_gestion_al_usuario = "Inconsistencia general fucnion Solicita_table_row_bot_gestion_al_usuario " & ex.Message
        End Try
    End Function
    Function Lista_gestion_al_usuario(ByVal tipo_consulta As Object,
                                      ByVal valor_consulta As String,
                                      ByVal id_tarea_wf As Long,
                                      ByVal id_usuario_wf As Integer,
                                      ByVal Class_config_general_service As List(Of Class_config_general_service),
                                      ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita consulta de la gestion a un usurio externo por parte del 
        '          del usuario workflow- Solo lista las gestion del usuario workflow
        '          informado
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        'id_usuario_wf                 : Representa el usaurio que realiza la gestion
        'id_tarea_wf                   : Representa la tarea a la cual se le hace gestión
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_Row_Gabinete_Generic  : Retorna la estructura con los campos 
        ' y los registros de la consulta de migracion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-23
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try

            Dim Result As String = ""
            ' --------- /// Solicita la estructura de los campos
            Result = Solicita_lista_head_gestion_al_usuario(class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Lista_gestion_al_usuario = Result
                Exit Function
            End If
            Dim sql_consulta As String = ""
            ' --------- /// Solicita el comando SQL para realizar la consulta
            Result = Solicita_SQL_Consulta_gestion_al_usuario(Class_config_general_service,
                                                              tipo_consulta,
                                                              valor_consulta,
                                                              id_tarea_wf,
                                                              id_usuario_wf,
                                                             "wf_gestion_tarea_usuario",
                                                              class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic,
                                                              sql_consulta)
            If Result <> "YES" Then
                Lista_gestion_al_usuario = Result
                Exit Function
            End If
            Result = Solicita_table_row_bot_gestion_al_usuario(sql_consulta,
                                                               class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                Lista_gestion_al_usuario = Result
                Exit Function
            End If
            class_stru_Row_Gabinete_Generic.Error_result = "YES"
            Lista_gestion_al_usuario = "YES"
            Exit Function
        Catch ex As Exception
            Lista_gestion_al_usuario = "Inconsistencia general funcion Lista_gestion_al_usuario " & ex.Message
        End Try
    End Function

    Function Registro_gestion_al_usuario(ByVal id_tarea_wf As Long,
                                         ByVal id_usuario_wf As Integer,
                                         ByRef Class_wf_gestion_tarea_usuario_stru As class_wf_gestion_tarea_usuario_stru) As String
        '-------------------------------------------------------------------------------
        'Funcion : Reliza el registro de la gestión al usuario
        '          
        '       
        '-------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'id_usuario_wf                       : Representa la identificación del usuario
        '                                      workflow que realiza la gestión
        'id_tarea_wf                         : Representa la identificación de la tarea
        'class_wf_gestion_tarea_usuario_stru : Representa la estructura con los datos   
        '                                      del registro de la gestión
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_wf_gestion_tarea_usuario_stru : Retorna la estructura del registro de
        '                                      la gestión
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-09-22
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim stru_estado As stru_estado = Nothing
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            '//------Consulta datos estructura estado tarea-------
            Result = Class_estados_tarea_workflow.Solicita_datos_estructura_tareas_seleccionada(id_tarea_wf,
                                                                                                stru_estado)
            If Result <> "YES" Then
                Registro_gestion_al_usuario = Result
                Exit Function
            End If
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim time1al As String = Date.Now.ToString
            '//------Solicita fecha de registro-------
            ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
            Dim SQLinsert As String = "INSERT INTO wf_gestion_tarea_usuario (wf_gestion_tipos_id_tipo_gestion," &
            "estados_tarea_workflow_id_Estado, inicio_tareas_workflow_id_Tarea, usuario_workflow_idU_suario, content_gestion," &
            "fecha_registro, Estado_envio_correo) VALUES (" &
            Class_wf_gestion_tarea_usuario_stru.wf_gestion_tipos_id_tipo_gestion & "," & stru_estado.id_Estado & "," &
            id_tarea_wf & "," & id_usuario_wf & ",'" & Class_wf_gestion_tarea_usuario_stru.content_gestion & "','" &
            time1al & "'," & Class_wf_gestion_tarea_usuario_stru.Estado_envio_correo & ")"
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_LAST_INSERT_COMMAND(SQLinsert,
                                                       Class_wf_gestion_tarea_usuario_stru.id_gestion_tarea_usuario)
            If Result <> "YES" Then
                Registro_gestion_al_usuario = Result
                Exit Function
            End If
            Registro_gestion_al_usuario = "YES"
            Exit Function
        Catch ex As Exception
            Registro_gestion_al_usuario = "Inconsistencia general funcion Registro_gestion_al_usuario " & ex.Message
        End Try
    End Function
End Class
