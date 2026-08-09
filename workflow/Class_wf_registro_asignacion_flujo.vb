Imports MySql.Data.MySqlClient

Public Class Class_wf_registro_asignacion_flujo

    Function balanceo_asignacion_por_conector_flujo(ByVal id_actividad As Integer,
                                                    ByVal id_registro_actividad_flujo As Integer,
                                                    ByVal fecha_registro As String,
                                                    ByRef id_usuario_wf As Integer) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita el usuario workflow relacionado a la actividad o grupo de flujo workflow con menos actividades asignadas
        '
        '-----------
        'Parametros:
        '-----------
        'id_actividad - identificación de la actividad en la ruta workflow
        'id_registro_actividad_flujo - indentifica la actividad dentro del flujo de trabajo workflow
        '-----------
        'Retorno   ;
        '----------
        'estado_registro_asignacion, valores (NO) no contiene registros
        '(YES) contiene registros
        'id_usuario_wf - Retorna la identificación del usuario workflow con menos carga de trabajo
        '----------
        'Fecha     : 2022-09-07
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassWorkflowUsuario As New ClassWorkflowUsuario
            Dim estado_registro_asignacion As String = "NO"
            Dim id_usuario_wf_ As Integer = 0
            Dim cantidad_tareas As Integer = 0
            '------------------------------------------------------------------------------------------
            'Solicita existencia de registro de usuarios en la tabla de asginación de balanceo de flujo
            '------------------------------------------------------------------------------------------
            Result = Me.Solicita_existencia_registro_de_asignacion_actividad_flujo(id_registro_actividad_flujo,
                                                                                   estado_registro_asignacion)
            If Result <> "YES" Then
                balanceo_asignacion_por_conector_flujo = Result
                Exit Function
            End If
            '----------------------------------------------------------------------------
            'Registra primer usuario de registro de asignación de la actividad del flujo
            '----------------------------------------------------------------------------
            If estado_registro_asignacion = "NO" Then
                Result = ClassWorkflowUsuario.Solicita_usuarios_activos_relacionado_actividad_grupo(id_actividad,
                                                                                                   id_usuario_wf_)
                If Result <> "YES" Then
                    balanceo_asignacion_por_conector_flujo = Result
                    Exit Function
                End If
                If id_usuario_wf_ = 0 Then
                    balanceo_asignacion_por_conector_flujo = "La actividad no tiene usuarios relacionados para el balanceo de cargas de trabajo"
                    Exit Function
                Else
                    '-----------------------------------
                    'Retorna usuario workflow asignación
                    '-----------------------------------
                    id_usuario_wf = id_usuario_wf_
                    Result = Registra_asignacion_de_tarea_usuario_flujo(id_registro_actividad_flujo,
                                                                        id_usuario_wf_,
                                                                        1,
                                                                        fecha_registro)
                    If Result <> "YES" Then
                        balanceo_asignacion_por_conector_flujo = Result
                        Exit Function
                    Else
                        balanceo_asignacion_por_conector_flujo = "YES"
                        Exit Function
                    End If
                End If
            Else
                '----------------------------------------------------------------------
                'Registra usuarios sin registro de asignaciòn de la actividad del flujo 
                '----------------------------------------------------------------------
                Result = ClassWorkflowUsuario.Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_flujo(id_registro_actividad_flujo,
                                                                                                                         id_actividad,
                                                                                                                         id_usuario_wf_)
                If Result <> "YES" Then
                    balanceo_asignacion_por_conector_flujo = Result
                    Exit Function
                End If
                '------------------------------------------------
                'Registra usuarios sin registro de asignaciòn
                '------------------------------------------------
                If id_usuario_wf_ <> 0 Then
                    Result = Solicita_existencia_registro_de_menor_tareas_asignadas_flujo(id_registro_actividad_flujo,
                                                                                          1,
                                                                                          cantidad_tareas)
                    If Result <> "YES" Then
                        balanceo_asignacion_por_conector_flujo = Result
                        Exit Function
                    End If
                    '-----------------------------------
                    'Retorna usuario workflow asignación
                    '-----------------------------------
                    id_usuario_wf = id_usuario_wf_
                    Result = Registra_asignacion_de_tarea_usuario_flujo(id_registro_actividad_flujo,
                                                                        id_usuario_wf_,
                                                                        cantidad_tareas,
                                                                        fecha_registro)
                    If Result <> "YES" Then
                        balanceo_asignacion_por_conector_flujo = Result
                        Exit Function
                    Else
                        balanceo_asignacion_por_conector_flujo = "YES"
                        Exit Function
                    End If
                Else
                    Result = Actualiza_incrementa_registro_asignacion_flujo(id_registro_actividad_flujo,
                                                                            fecha_registro,
                                                                            id_usuario_wf)
                    If Result <> "YES" Then
                        balanceo_asignacion_por_conector_flujo = Result
                        Exit Function
                    Else
                        balanceo_asignacion_por_conector_flujo = "YES"
                        Exit Function
                    End If
                    balanceo_asignacion_por_conector_flujo = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            balanceo_asignacion_por_conector_flujo = "Inconsostencia general funcion balanceo_asignacion_por_conector_flujo " & ex.Message
        End Try
    End Function
    Function Registro_balanceo_asignacion_flujo(ByVal id_actividad_flujo As Integer,
                                                ByVal id_usuario_wf_flujo As Integer,
                                                ByVal id_tarea_workflow As Long,
                                                ByVal estado_tarea As Long,
                                                ByVal estado_incrementa As Integer,
                                                ByVal fecha_registro As String,
                                                ByRef estado_registro_balanceo As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra balanceo reasignación usuario sii por flujo de trabajo
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS   
        '---------------------------------------------------------------------------
        'id_actividad          : Respresenta la indentificacion de la tarea flujo
        'id_usuario_workflow   : Id usuario workflow que se le reasigna
        'fecha_registro        : Representa la fecha de registro
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-08
        'Elabora               : Miguel Angel Urueta Miranda estado_registro_balanceo
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim cantidad_tareas As Integer = 1
            Dim estado_asignacion As String = ""
            'Result = Solicita_existencia_registro_asignacion_actividad_flujo(id_actividad_flujo,
            '                                                                 estado_asignacion)
            'If Result <> "YES" Then
            '    Registro_balanceo_asignacion_flujo = Result
            '    Exit Function
            'End If
            'If estado_asignacion = "NO" Then
            '    estado_registro_balanceo = 0
            '    Registro_balanceo_asignacion_flujo = "YES"
            '    Exit Function
            'End If
            '----------------------------------------------------------
            'Valida que el usuario este activo en el balanceo de ruta
            '----------------------------------------------------------
            Dim estado_balanceo_usuario As Integer = 0
            Dim ClassWorkflowUsuario As New ClassWorkflowUsuario
            Result = ClassWorkflowUsuario.Solicita_estado_balanceo_ruta_usuario_workflow(id_usuario_wf_flujo,
                                                                                         estado_balanceo_usuario)
            If Result <> "YES" Then
                Registro_balanceo_asignacion_flujo = Result
                Exit Function
            End If
            If estado_balanceo_usuario = 0 Then
                estado_registro_balanceo = 0
                Registro_balanceo_asignacion_flujo = "YES"
                Exit Function
            End If
            '--------------------------------------------------------------------------
            'Valida si la tarea fue asignada por balanceo para decrementar la tarea
            'al usuario en registro de balanceo de ruta, solo para el caso que decrementa
            '--------------------------------------------------------------------------
            Dim Class_wf_log_asignacion_balanceo As New Class_wf_log_asignacion_balanceo
            Dim estado_registro_balanceo_tarea_anterior As Integer = 0
            If estado_incrementa = 0 Then
                Result = Class_wf_log_asignacion_balanceo.Solicita_estado_registro_balanceo_tarea_usuario_flujo(id_usuario_wf_flujo,
                                                                                                                id_tarea_workflow,
                                                                                                                estado_tarea,
                                                                                                                id_actividad_flujo,
                                                                                                                estado_registro_balanceo_tarea_anterior)
                If Result <> "YES" Then
                    Registro_balanceo_asignacion_flujo = Result
                    Exit Function
                End If
                If estado_registro_balanceo_tarea_anterior = 0 Then
                    estado_registro_balanceo = 0
                    Registro_balanceo_asignacion_flujo = "YES"
                    Exit Function
                End If
            End If
            Result = Solicita_existencia_registro_asignacion_actividad_usuario_flujo(id_actividad_flujo,
                                                                                     id_usuario_wf_flujo,
                                                                                     estado_asignacion)
            If Result <> "YES" Then
                Registro_balanceo_asignacion_flujo = Result
                Exit Function
            End If
            If estado_asignacion = "NO" Then
                '----------------------------------------------------------------------
                'Hace el registro del usuario en el registro de balanceo para flujo
                '----------------------------------------------------------------------
                Result = Solicita_existencia_registro_de_menor_tareas_asignadas_flujo(id_actividad_flujo,
                                                                                      0,
                                                                                      cantidad_tareas)
                If Result <> "YES" Then
                    Registro_balanceo_asignacion_flujo = Result
                    Exit Function
                End If
                Result = Registra_asignacion_de_tarea_usuario_flujo(id_actividad_flujo,
                                                                    id_usuario_wf_flujo,
                                                                    cantidad_tareas,
                                                                    fecha_registro)
                If Result <> "YES" Then
                    Registro_balanceo_asignacion_flujo = Result
                    Exit Function
                End If
                estado_registro_balanceo = 1
                Registro_balanceo_asignacion_flujo = "YES"
                Exit Function
            Else
                Result = Actualiza_incrementa_registro_usuario_asignacion_flujo(id_actividad_flujo,
                                                                                id_usuario_wf_flujo,
                                                                                estado_incrementa,
                                                                                fecha_registro)
                If Result <> "YES" Then
                    Registro_balanceo_asignacion_flujo = Result
                    Exit Function
                End If
                estado_registro_balanceo = 1
                Registro_balanceo_asignacion_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registro_balanceo_asignacion_flujo = "Inconsistencia general funcion Registro_balanceo_asignacion_flujo " & ex.Message
        End Try
    End Function
    Function Actualiza_incrementa_registro_asignacion_flujo(ByVal id_registro_actividad_flujo As Integer,
                                                            ByVal fecha_registro As String,
                                                            ByRef id_usuario_wf As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : incrementa la cantidad de registro del usuario en registro
        '          de actividades de balanceo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_actividad_flujo : Respresenta la indentificacion de la activida
        '                              del flujo de trabajo 
        'fecha_registro              : Representa la fecha de registro
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-08
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim mySqldatReader As MySqlDataReader
        Dim myTrans As MySqlTransaction
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Try
            Dim Result As String = ref.Returna_Conexion_Mysql(myConnection)
            If Result <> "YES" Then
                Actualiza_incrementa_registro_asignacion_flujo = Result
                Exit Function
            End If
            '-------------------------------------------------------------
            'Solicita usuario con menor numero de actividades asignadas
            '-------------------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = "Select num_tareas,usuario_workflow_idU_suario  " &
                        " From wf_registro_asignacion_flujo " &
                        " WHERE ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_registro_actividad_flujo &
                        " order by num_tareas asc LIMIT 1 for update "
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Actualiza_incrementa_registro_asignacion_flujo = "Imposible conectar con la tabla  wf_registro_asignacion_flujo"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Actualiza_incrementa_registro_asignacion_flujo = "Imposible encontrar registros de la id actividad flujo (" & id_registro_actividad_flujo & ")"
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            Dim cantidad_tareas As Integer = mySqldatReader.Item("num_tareas")
            '-----------------------------------
            'Retorna usuario workflow asignación
            '-----------------------------------
            id_usuario_wf = mySqldatReader.Item("usuario_workflow_idU_suario")
            cantidad_tareas += 1
            mySqldatReader.Close()
            '-----------------------------------------------
            'Actualiza numero  de actividades  del  usuario
            '-----------------------------------------------
            Dim Parametro_Actualiza_System1 As String = "update wf_registro_asignacion_flujo set num_tareas = " & cantidad_tareas &
                        ",fecha_registro='" & fecha_registro & "'" &
                         " where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO =" & id_registro_actividad_flujo &
                         " and usuario_workflow_idU_suario = " & id_usuario_wf
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_incrementa_registro_asignacion_flujo = "Imposible actualizar la tabla wf_registro_asignacion_ruta "
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_incrementa_registro_asignacion_flujo = "YES"
            Exit Function
        Catch ex As Exception
            If Not myTrans.Connection Is Nothing Then
                myConnection.Close()
                Actualiza_incrementa_registro_asignacion_flujo = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            Else
                Actualiza_incrementa_registro_asignacion_flujo = ex.Message
                Exit Function
            End If
        End Try
    End Function
    Function Actualiza_incrementa_registro_usuario_asignacion_flujo(ByVal id_registro_actividad_flujo As Integer,
                                                                    ByVal id_usuario_workflow_flujo As Integer,
                                                                    ByVal estado_incrementa As Integer,
                                                                    ByVal fecha_registro As String) As String
        '---------------------------------------------------------------------------
        'Funcion : incrementa la cantidad de registro del usuario en registro
        '          de actividades de balanceo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_actividad_flujo : Respresenta la indentificacion de la activida
        '                              del flujo de trabajo 
        'id_usuario_workflow_flujo   : Representa la identificacion del usuario
        '                              workflow
        'fecha_registro              : Representa la fecha de registro
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-08
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim mySqldatReader As MySqlDataReader
        Dim myTrans As MySqlTransaction
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Try
            Dim Result As String = ref.Returna_Conexion_Mysql(myConnection)
            If Result <> "YES" Then
                Actualiza_incrementa_registro_usuario_asignacion_flujo = Result
                Exit Function
            End If
            '-------------------------------------------------------------
            'Solicita usuario con menor numero de actividades asignadas
            '-------------------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim Parametro_Select_System1 As String = "Select num_tareas,usuario_workflow_idU_suario  " &
                        " From wf_registro_asignacion_flujo " &
                        " WHERE ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_registro_actividad_flujo &
                        " and usuario_workflow_idU_suario=" & id_usuario_workflow_flujo &
                        " order by num_tareas asc LIMIT 1 for update "
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Actualiza_incrementa_registro_usuario_asignacion_flujo = "Imposible conectar con la tabla  wf_registro_asignacion_flujo"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Actualiza_incrementa_registro_usuario_asignacion_flujo = "Imposible encontrar registros de la id actividad flujo (" & id_registro_actividad_flujo & ")"
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            Dim cantidad_tareas As Integer = mySqldatReader.Item("num_tareas")
            '-----------------------------------
            'Retorna usuario workflow asignación
            '-----------------------------------
            If estado_incrementa = 1 Then
                cantidad_tareas += 1
            Else
                cantidad_tareas -= 1
            End If
            mySqldatReader.Close()
            '-----------------------------------------------
            'Actualiza numero  de actividades  del  usuario
            '-----------------------------------------------
            Dim Parametro_Actualiza_System1 As String = "update wf_registro_asignacion_flujo set num_tareas = " & cantidad_tareas &
                        ",fecha_registro='" & fecha_registro & "'" &
                         " where ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO =" & id_registro_actividad_flujo &
                         " and usuario_workflow_idU_suario = " & id_usuario_workflow_flujo
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_incrementa_registro_usuario_asignacion_flujo = "Imposible actualizar la tabla wf_registro_asignacion_ruta "
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_incrementa_registro_usuario_asignacion_flujo = "YES"
            Exit Function
        Catch ex As Exception
            If Not myTrans.Connection Is Nothing Then
                myConnection.Close()
                Actualiza_incrementa_registro_usuario_asignacion_flujo = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            Else
                Actualiza_incrementa_registro_usuario_asignacion_flujo = ex.Message
                Exit Function
            End If
        End Try
    End Function
    Function Solicita_existencia_registro_asignacion_actividad_flujo(ByVal id_registro_actividad_flujo As Integer,
                                                                     ByRef estado_asignacion As String) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita la existencia registro de asignación actividad flujo
        '
        '-----------
        'Parametros
        '-----------
        'id_registro_actividad_flujo : identificación del registro de la actividad en el flujo
        '-----------
        'Retorno   
        '----------
        'estado_asignacion           : valores (NO) no contiene registros
        '(YES) contiene registros
        '----------
        'Fecha     : 2023-05-16
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select id_registro_asignacion  " &
               " From wf_registro_asignacion_flujo " &
               " WHERE ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_registro_actividad_flujo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_asignacion_flujo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_asignacion_actividad_flujo = "Función Solicita_existencia_registro_asignacion_actividad_flujo dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_asignacion = "NO"
                Solicita_existencia_registro_asignacion_actividad_flujo = "YES"
                Exit Function
            Else
                estado_asignacion = "YES"
                Solicita_existencia_registro_asignacion_actividad_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_asignacion_actividad_flujo = "Inconsistencia general funcion Solicita_existencia_registro_asignacion_actividad_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_registro_asignacion_actividad_usuario_flujo(ByVal id_registro_actividad_flujo As Integer,
                                                                             ByVal id_usuario_workflow As Integer,
                                                                             ByRef estado_asignacion As String) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita la existencia registro de asignación de usuario y flujo
        '
        '-----------
        'Parametros
        '-----------
        'id_registro_actividad_flujo : identificación del registro de la actividad en el flujo
        '-----------
        'Retorno   
        '----------
        'estado_asignacion           : valores (NO) no contiene registros
        '(YES) contiene registros
        '----------
        'Fecha     : 2023-04-29
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select id_registro_asignacion  " &
               " From wf_registro_asignacion_flujo " &
               " WHERE ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_registro_actividad_flujo &
               " AND usuario_workflow_idU_suario=" & id_usuario_workflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_asignacion_flujo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_asignacion_actividad_usuario_flujo = "Función Solicita_existencia_registro_asignacion_actividad_usuario_flujo dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_asignacion = "NO"
                Solicita_existencia_registro_asignacion_actividad_usuario_flujo = "YES"
                Exit Function
            Else
                estado_asignacion = "YES"
                Solicita_existencia_registro_asignacion_actividad_usuario_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_asignacion_actividad_usuario_flujo = "Inconsistencia general funcion Solicita_existencia_registro_asignacion_actividad_usuario_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_registro_de_asignacion_actividad_flujo(ByVal id_registro_actividad_flujo As Integer,
                                                                        ByRef estado_registro_asignacion As String) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita la existencia de registro de usuarios  en la tabla de asignacion de tareas que controla el numero
        'de tareas asignadas por el suuario en el grupo
        '-----------
        'Parametros:
        '-----------
        'id_actividad - identificación del registro de la actividad en el flujo
        '-----------
        'Retorno   ;
        '----------
        'estado_registro_asignacion, valores (NO) no contiene registros
        '(YES) contiene registros
        '----------
        'Fecha     : 2022-09-08
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select id_registro_asignacion  " &
               " From wf_registro_asignacion_flujo " &
               " WHERE ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_registro_actividad_flujo
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_asignacion_flujo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_de_asignacion_actividad_flujo = "Función Solicita_existencia_registro_de_asignacion_actividad_flujo dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_registro_asignacion = "NO"
                Solicita_existencia_registro_de_asignacion_actividad_flujo = "YES"
                Exit Function
            Else
                estado_registro_asignacion = "YES"
                Solicita_existencia_registro_de_asignacion_actividad_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_de_asignacion_actividad_flujo = "Inconsistencia general funcion Solicita_existencia_registro_de_asignacion_actividad_flujo " & ex.Message
        End Try
    End Function
    Function Registra_asignacion_de_tarea_usuario_flujo(ByVal id_registro_actividad_flujo As Integer,
                                                        ByVal id_usuario_wf As Integer,
                                                        ByVal cantidad_tareas As Integer,
                                                        ByVal fecha_registro As String) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Registra asignación de asignación de tarea de un usuario que pertenece a una actividad de flujo
        '
        '-----------
        'Parametros:
        '-----------
        'id_registro_actividad_flujo - identificación de la actividad en el flujo , id_usuario_wf - usuario workflow de asignación, 
        'cantidad_tareas - el numero de tareas asignadas
        '-----------
        'Retorno   ;
        '----------
        'Funcion, valores (error) retorna el error
        '(YES) trnasacción exitosa
        '----------
        'Fecha     : 2022-09-08
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim sql_registro As String = "insert into wf_registro_asignacion_flujo(usuario_workflow_idU_suario,ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO,num_tareas,fecha_registro) values (" &
                      id_usuario_wf & "," & id_registro_actividad_flujo & "," & cantidad_tareas & ",'" & fecha_registro & "')"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ""
            Result = ref.SELECTION_INSERT_COMMAND(sql_registro)
            If Result <> "YES" Then
                Registra_asignacion_de_tarea_usuario_flujo = Result
                Exit Function
            Else
                Registra_asignacion_de_tarea_usuario_flujo = Result
                Exit Function
            End If
        Catch ex As Exception
            Registra_asignacion_de_tarea_usuario_flujo = "Inconsistencia general funcion Registra_asignacion_de_tarea_usuario_flujo " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_registro_de_menor_tareas_asignadas_flujo(ByVal id_registro_actividad_flujo As Integer,
                                                                          ByVal confirma As Integer,
                                                                          ByRef cantidad_tareas As Integer) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita la cantidad menor de tareas asignadas a una actividad de flujo 
        '
        '-----------
        'Parametros:
        '-----------
        'id_registro_actividad_flujo - identificación de la actividad flujo
        '-----------
        'Retorno   ;
        '----------
        'Funcion, valores (error) retorna el error
        '(YES) trnasacción exitosa
        ' cantidad_tareas - menor numero de tareas asignadas  
        '----------
        'Fecha     : 2022-09-08
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select wr.num_tareas  " &
               " From wf_registro_asignacion_flujo as wr " &
               " INNER JOIN    usuario_workflow as uw ON (wr.usuario_workflow_idu_suario=uw.idu_suario and uw.estado_balanceo_grupo = 1)" &
               " WHERE ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO=" & id_registro_actividad_flujo &
               " order by num_tareas asc limit 1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_asignacion_flujo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_de_menor_tareas_asignadas_flujo = "Función Solicita_existencia_registro_de_menor_tareas_asignadas_flujo dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                cantidad_tareas = 1
                If confirma = 1 Then
                    Solicita_existencia_registro_de_menor_tareas_asignadas_flujo = "Imposible encontrar registros de asignación de la actividad (" & id_registro_actividad_flujo & ")"
                    Exit Function
                Else
                    Solicita_existencia_registro_de_menor_tareas_asignadas_flujo = "YES"
                    Exit Function
                End If

            Else
                cantidad_tareas = Datset.Tables(0).Rows(0).Item(0)
                Solicita_existencia_registro_de_menor_tareas_asignadas_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_de_menor_tareas_asignadas_flujo = "Inconsistencia general funcion Solicita_existencia_registro_de_menor_tareas_asignadas_flujo " & ex.Message
        End Try
    End Function
End Class
