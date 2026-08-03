Imports System.Diagnostics.Eventing.Reader
Imports MySql.Data.MySqlClient

Public Class Class_wf_registro_asignacion_ruta
    Function Balanceo_asignacion_por_conector_ruta(ByVal id_actividad As Integer,
                                                   ByVal fecha_registro As String,
                                                   ByRef id_usuario_wf As Integer) As String
        Try
            '---------------------------------------------------------------------------------------------------------------
            '---------
            'Funcion :
            '---------
            'Solicita el usuario workflow relacionado a la actividad o grupo de ruta workflow con menos actividades asignadas
            '
            '-----------
            'Parametros:
            '-----------
            'id_actividad - identificación de la actividad en la ruta workflow
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
            Dim Result As String = ""
            Dim ClassWorkflowUsuario As New ClassWorkflowUsuario
            Dim estado_registro_asignacion As String = "NO"
            Dim id_usuario_wf_ As Integer = 0
            Dim cantidad_tareas As Integer = 0
            Dim Class_relacion_sirep_workflow As New Class_relacion_sirep_workflow
            Dim codigo_sii As String = ""
            '------------------------------------------------------------------------------------------
            'Solicita existencia de registro de usuarios en la tabla de asginación de balanceo de ruta
            '------------------------------------------------------------------------------------------
            Result = Me.Solicita_existencia_registro_de_asignacion_actividad_ruta(id_actividad,
                                                                                  estado_registro_asignacion)
            If Result <> "YES" Then
                Balanceo_asignacion_por_conector_ruta = Result
                Exit Function
            End If

            If estado_registro_asignacion = "NO" Then
                '-------------------------------------------------------------------
                'REGISTRA LA PRIMERA ASIGNACION DE BALANCEO DE LA ACTIVIDAD 
                '-------------------------------------------------------------------
                '-----------------------------------------------------------------------------------
                'Caso actividad que no tiene usuarios registrados en la tabla asignacion y balanceo
                '-----------------------------------------------------------------------------------
                Result = ClassWorkflowUsuario.Solicita_usuarios_activos_relacionado_actividad_grupo(id_actividad,
                                                                                                    id_usuario_wf_)
                If Result <> "YES" Then
                    Balanceo_asignacion_por_conector_ruta = Result
                    Exit Function
                End If
                If id_usuario_wf_ = 0 Then
                    Balanceo_asignacion_por_conector_ruta = "La actividad no tiene usuarios relacionados para el balanceo de cargas de trabajo"
                    Exit Function
                Else
                    '-----------------------------------
                    'Registra usuario sin asignación
                    '-----------------------------------
                    id_usuario_wf = id_usuario_wf_
                    Result = Registra_asignacion_de_tarea_usuario_ruta(id_actividad,
                                                                      id_usuario_wf_,
                                                                      1,
                                                                      fecha_registro)
                    If Result <> "YES" Then
                        Balanceo_asignacion_por_conector_ruta = Result
                        Exit Function
                    Else
                        Balanceo_asignacion_por_conector_ruta = "YES"
                        Exit Function
                    End If
                End If
            Else
                '------------------------------------------------------------------
                'Solicita usuarios sin registro de asignaciòn de la actividad
                '------------------------------------------------------------------
                Result = ClassWorkflowUsuario.Solicita_usuario_activos_relacionado_actividad_grupo_sin_asiginacion_ruta(id_actividad,
                                                                                                                        id_usuario_wf_)
                If Result <> "YES" Then
                    Balanceo_asignacion_por_conector_ruta = Result
                    Exit Function
                End If
                '------------------------------------------------
                'Registra usuarios sin registro de asignaciòn
                '------------------------------------------------
                If id_usuario_wf_ <> 0 Then
                    Result = Solicita_existencia_registro_de_menor_tareas_asignadas(id_actividad,
                                                                                    1,
                                                                                    cantidad_tareas)
                    If Result <> "YES" Then
                        Balanceo_asignacion_por_conector_ruta = Result
                        Exit Function
                    End If
                    '-----------------------------------
                    'Retorna usuario workflow asignación
                    '-----------------------------------
                    id_usuario_wf = id_usuario_wf_
                    Result = Registra_asignacion_de_tarea_usuario_ruta(id_actividad,
                                                                      id_usuario_wf_,
                                                                      cantidad_tareas,
                                                                      fecha_registro)
                    If Result <> "YES" Then
                        Balanceo_asignacion_por_conector_ruta = Result
                        Exit Function
                    Else
                        Balanceo_asignacion_por_conector_ruta = "YES"
                        Exit Function
                    End If
                Else
                    Result = Actualiza_incrementa_registro_asignacion_ruta(id_actividad,
                                                                           fecha_registro,
                                                                           id_usuario_wf)
                    If Result <> "YES" Then
                        Balanceo_asignacion_por_conector_ruta = Result
                        Exit Function
                    Else
                        Balanceo_asignacion_por_conector_ruta = "YES"
                        Exit Function
                    End If

                End If
            End If
        Catch ex As Exception
            Balanceo_asignacion_por_conector_ruta = "Inconsistencia general funcion balanceo_asignacion_por_conector_ruta " & ex.Message
        End Try
    End Function

    Function Registro_balanceo_reasignacion_ruta(ByVal id_actividad As Integer,
                                                     ByVal id_usuario_workflow As Integer,
                                                     ByVal id_tarea_workflow As Long,
                                                     ByVal estado_tarea As Long,
                                                     ByVal estado_incrementa As Integer,
                                                     ByVal fecha_registro As String,
                                                     ByRef estado_registro_balanceo As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra balanceo reasignación usuario
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_actividad          : Respresenta la indentificacion de la tarea workflow
        'id_usuario_workflow   : Id usuario workflow que se le reasigna
        'estado_incrementa     : Representa el estado de incremento o decremento del
        '                        registro de balanceo
        'fecha_registro        : Representa la fecha de registro
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
        Try
            Dim estado_asignacion As String = ""
            Dim Result As String = ""
            '----------------------------------------------------------
            'Valida que el usuario este activo en el balanceo de ruta
            '----------------------------------------------------------
            Dim estado_balanceo_usuario As Integer = 0
            Dim ClassWorkflowUsuario As New ClassWorkflowUsuario
            Result = ClassWorkflowUsuario.Solicita_estado_balanceo_ruta_usuario_workflow(id_usuario_workflow,
                                                                                         estado_balanceo_usuario)
            If Result <> "YES" Then
                Registro_balanceo_reasignacion_ruta = Result
                Exit Function
            End If
            If estado_balanceo_usuario = 0 Then
                estado_registro_balanceo = 0
                Registro_balanceo_reasignacion_ruta = "YES"
                Exit Function
            End If
            '--------------------------------------------------------------------------
            'Valida si la tarea fue asignada por balanceo para decrementar la tarea
            'al usuario en registro de balanceo de ruta, solo para el caso que decrementa
            '--------------------------------------------------------------------------
            Dim Class_wf_log_asignacion_balanceo As New Class_wf_log_asignacion_balanceo
            Dim estado_registro_balanceo_tarea_anterior As Integer = 0
            If estado_incrementa = 0 Then
                Result = Class_wf_log_asignacion_balanceo.Solicita_estado_registro_balanceo_tarea_usuario(id_usuario_workflow,
                                                                                                          id_tarea_workflow,
                                                                                                          estado_tarea,
                                                                                                          estado_registro_balanceo_tarea_anterior)
                If Result <> "YES" Then
                    Registro_balanceo_reasignacion_ruta = Result
                    Exit Function
                End If
                If estado_registro_balanceo_tarea_anterior = 0 Then
                    estado_registro_balanceo = 0
                    Registro_balanceo_reasignacion_ruta = "YES"
                    Exit Function
                End If
            End If
            '----------------------------------------------------------------------------------
            'Valida existencia de asignación del usuario en el registro de balanceo de la ruta
            '----------------------------------------------------------------------------------
            Result = Solicita_existencia_registro_asignacion_actividad_usuario(id_actividad,
                                                                               id_usuario_workflow,
                                                                               estado_asignacion)
            If Result <> "YES" Then
                Registro_balanceo_reasignacion_ruta = Result
                Exit Function
            End If
            Dim cantidad_tareas As Integer = 1
            If estado_asignacion = "NO" Then
                '------------------------------------------------------------------
                'Agrega el registro de balanceo de usuario por primera vez
                '-----------------------------------------------------------------
                Result = Solicita_existencia_registro_de_menor_tareas_asignadas(id_actividad,
                                                                                0,
                                                                                cantidad_tareas)
                If Result <> "YES" Then
                    Registro_balanceo_reasignacion_ruta = Result
                    Exit Function
                End If
                Result = Registra_asignacion_de_tarea_usuario_ruta(id_actividad,
                                                                   id_usuario_workflow,
                                                                   cantidad_tareas,
                                                                   fecha_registro)
                If Result <> "YES" Then
                    Registro_balanceo_reasignacion_ruta = Result
                    Exit Function
                End If
                estado_registro_balanceo = 1
                Registro_balanceo_reasignacion_ruta = "YES"
                Exit Function
            Else
                Result = Actualiza_incrementa_registro_usuario_asignacion_ruta(id_actividad,
                                                                               id_usuario_workflow,
                                                                               estado_incrementa,
                                                                               fecha_registro)
                If Result <> "YES" Then
                    Registro_balanceo_reasignacion_ruta = Result
                    Exit Function
                End If
                estado_registro_balanceo = 1
                Registro_balanceo_reasignacion_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registro_balanceo_reasignacion_ruta = "Inconsistencia general funcion Registro_balanceo_reasignacion_sii_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_registro_de_asignacion_actividad_ruta(ByVal id_actividad As Integer,
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
        'id_actividad - identificación de la actividad workflow -  id_actividad
        '-----------
        'Retorno   ;
        '----------
        'estado_registro_asignacion, valores (NO) no contiene registros
        '(YES) contiene registros
        '----------
        'Fecha     : 2022-09-07
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select id_registro_asignacion  " &
               " From wf_registro_asignacion_ruta " &
               " WHERE listado_actividades_workflow_Id_Actividad=" & id_actividad
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_asignacion_ruta")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_de_asignacion_actividad_ruta = "Función Solicita_existencia_registro_de_asignacion_actividad_ruta dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_registro_asignacion = "NO"
                Solicita_existencia_registro_de_asignacion_actividad_ruta = "YES"
                Exit Function
            Else
                estado_registro_asignacion = "YES"
                Solicita_existencia_registro_de_asignacion_actividad_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_de_asignacion_actividad_ruta = "Inconsistencia general funcion Solicita_existencia_registro_de_asignacion_actividad_ruta " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_registro_de_menor_tareas_asignadas(ByVal id_actividad As Integer,
                                                                    ByVal confir_exitencia As Integer,
                                                                    ByRef cantidad_tareas As Integer) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita la cantidad menor de tareas asignadas a una actividad
        '
        '-----------
        'Parametros:
        '-----------
        'id_actividad - identificación de la actividad workflow 
        '-----------
        'Retorno   ;
        '----------
        'Funcion, valores (error) retorna el error
        '(YES) trnasacción exitosa
        ' cantidad_tareas - menor numero de tareas asignadas  
        '----------
        'Fecha     : 2022-09-07
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select wr.num_tareas  " &
               " From wf_registro_asignacion_ruta as wr " &
               "INNER JOIN    usuario_workflow as uw ON (wr.usuario_workflow_idu_suario=uw.idu_suario and uw.estado_balanceo_grupo = 1)" &
               " WHERE listado_actividades_workflow_Id_Actividad=" & id_actividad &
               " order by num_tareas asc limit 1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_asignacion_ruta")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_de_menor_tareas_asignadas = "Función Solicita_existencia_registro_de_menor_tareas_asignadas dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                cantidad_tareas = 1
                If confir_exitencia = 1 Then
                    Solicita_existencia_registro_de_menor_tareas_asignadas = "Imposible encontrar registros de asignación de la actividad (" & id_actividad & ")"
                    Exit Function
                Else
                    Solicita_existencia_registro_de_menor_tareas_asignadas = "YES"
                    Exit Function
                End If

            Else
                cantidad_tareas = Datset.Tables(0).Rows(0).Item(0)
                Solicita_existencia_registro_de_menor_tareas_asignadas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_de_menor_tareas_asignadas = "Inconsistencia general funcion Solicita_existencia_registro_de_menor_tareas_asignadas " & ex.Message
        End Try
    End Function

    Function Registra_asignacion_de_tarea_usuario_ruta(ByVal id_actividad As Integer,
                                                       ByVal id_usuario_wf As Integer,
                                                       ByVal cantidad_tareas As Integer,
                                                       ByVal fecha_registro As String) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Registra asignación de asignación de tarea de un usuario
        '
        '-----------
        'Parametros:
        '-----------
        'id_actividad - identificación de la actividad workflow , id_usuario_wf - usuario workflow de asignación, 
        'cantidad_tareas - el numero de tareas asignadas
        '-----------
        'Retorno   ;
        '----------
        'Funcion, valores (error) retorna el error
        '(YES) trnasacción exitosa
        '----------
        'Fecha     : 2022-09-07
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim sql_registro As String = "insert into wf_registro_asignacion_ruta(usuario_workflow_idU_suario,listado_actividades_workflow_Id_Actividad,num_tareas,fecha_registro) values (" &
                      id_usuario_wf & "," & id_actividad & "," & cantidad_tareas & ",'" & fecha_registro & "')"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Result As String = ""
            Result = ref.SELECTION_INSERT_COMMAND(sql_registro)
            If Result <> "YES" Then
                Registra_asignacion_de_tarea_usuario_ruta = Result
                Exit Function
            Else
                Registra_asignacion_de_tarea_usuario_ruta = Result
                Exit Function
            End If
        Catch ex As Exception
            Registra_asignacion_de_tarea_usuario_ruta = "Inconsistencia general funcion Registra_asignacion_de_tarea_usuario_ruta " & ex.Message
        End Try
    End Function
    Function Actualiza_incrementa_registro_asignacion_ruta(ByVal id_actividad As Integer,
                                                           ByVal fecha_registro As String,
                                                           ByRef id_usuario_wf As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : incrementa la cantidad de registro del usuario en registro
        '          de actividades de balanceo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_actividad          : Respresenta la indentificacion de la tarea workflow
        'fecha_registro        : Representa la fecha de registro
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-03
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim mySqldatReader As MySqlDataReader
        Dim myTrans As MySqlTransaction
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Try
            Dim Result As String = ref.Returna_Conexion_Mysql(myConnection)
            If Result <> "YES" Then
                Actualiza_incrementa_registro_asignacion_ruta = Result
                Exit Function
            End If
            '-------------------------------------------------------------
            'Solicita usuario con menor numero de actividades asignadas
            '-------------------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim cantidad_tareas As Integer = 0
            Dim Parametro_Select_System1 As String = "Select num_tareas,usuario_workflow_idU_suario  " &
            " From wf_registro_asignacion_ruta  as wr" &
            " INNER JOIN usuario_workflow as uw on (uw.idu_suario=wr.usuario_workflow_idu_suario and uw.estado_balanceo_grupo=1)" &
            " WHERE listado_actividades_workflow_Id_Actividad=" & id_actividad &
            " order by num_tareas, fecha_registro asc LIMIT 1 for update "
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Actualiza_incrementa_registro_asignacion_ruta = "Imposible conectar con la tabla  wf_registro_asignacion_ruta"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Actualiza_incrementa_registro_asignacion_ruta = "Los usuarios registrados en el registro de balanceo de ruta de la actividad (" & id_actividad & ")  no estan activos para balnceo de ruta"
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            cantidad_tareas = mySqldatReader.Item("num_tareas")
            '-----------------------------------
            'Retorna usuario workflow asignación
            '-----------------------------------
            id_usuario_wf = mySqldatReader.Item("usuario_workflow_idU_suario")
            cantidad_tareas += 1
            mySqldatReader.Close()
            '-----------------------------------------------
            'Actualiza numero  de actividades  del  usuario
            '-----------------------------------------------
            Dim Parametro_Actualiza_System1 As String = "update wf_registro_asignacion_ruta set num_tareas = " & cantidad_tareas &
            ",fecha_registro='" & fecha_registro & "'" &
             " where listado_actividades_workflow_Id_Actividad =" & id_actividad &
             " and usuario_workflow_idU_suario = " & id_usuario_wf
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_incrementa_registro_asignacion_ruta = "Imposible actualizar la tabla wf_registro_asignacion_ruta "
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_incrementa_registro_asignacion_ruta = "YES"
            Exit Function
        Catch ex As Exception
            If Not myTrans.Connection Is Nothing Then
                myConnection.Close()
                Actualiza_incrementa_registro_asignacion_ruta = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            Else
                Actualiza_incrementa_registro_asignacion_ruta = ex.Message
                Exit Function
            End If
        End Try
    End Function
    Function Actualiza_incrementa_registro_usuario_asignacion_ruta(ByVal id_actividad As Integer,
                                                                   ByVal id_usuario_wf As Integer,
                                                                   ByVal estado_incrementa As Integer,
                                                                   ByVal fecha_registro As String) As String
        '---------------------------------------------------------------------------
        'Funcion : incrementa la cantidad de registro del usuario en registro
        '          de actividades de balanceo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_actividad          : Respresenta la indentificacion de la tarea workflow
        'id_usuario_wf         : Id usuario workflow al cual se incrementa la tarea
        'fecha_registro        : Representa la fecha de registro
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-03
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim mySqldatReader As MySqlDataReader
        Dim myTrans As MySqlTransaction
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql
        Try
            Dim Result As String = ref.Returna_Conexion_Mysql(myConnection)
            If Result <> "YES" Then
                Actualiza_incrementa_registro_usuario_asignacion_ruta = Result
                Exit Function
            End If
            '-------------------------------------------------------------
            'Solicita usuario con menor numero de actividades asignadas
            '-------------------------------------------------------------
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim cantidad_tareas As Integer = 0
            Dim Parametro_Select_System1 As String = "Select num_tareas,usuario_workflow_idU_suario  " &
            " From wf_registro_asignacion_ruta " &
            " WHERE listado_actividades_workflow_Id_Actividad=" & id_actividad & " and usuario_workflow_idU_suario=" & id_usuario_wf &
            " order by num_tareas asc LIMIT 1 for update "
            myCommand.CommandText = Parametro_Select_System1
            mySqldatReader = myCommand.ExecuteReader()
            If mySqldatReader Is Nothing Then
                Actualiza_incrementa_registro_usuario_asignacion_ruta = "Imposible conectar con la tabla  wf_registro_asignacion_ruta"
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                Actualiza_incrementa_registro_usuario_asignacion_ruta = "Imposible encontrar registros de la id actividad (" & id_actividad & ")"
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            cantidad_tareas = mySqldatReader.Item("num_tareas")
            '-----------------------------------
            'Retorna usuario workflow asignación
            '-----------------------------------
            id_usuario_wf = mySqldatReader.Item("usuario_workflow_idU_suario")
            If estado_incrementa = 1 Then
                cantidad_tareas += 1
            Else
                cantidad_tareas -= 1
            End If
            mySqldatReader.Close()
            '-----------------------------------------------
            'Actualiza numero  de actividades  del  usuario
            '-----------------------------------------------
            Dim Parametro_Actualiza_System1 As String = "update wf_registro_asignacion_ruta set num_tareas = " & cantidad_tareas &
            ",fecha_registro='" & fecha_registro & "'" &
             " where listado_actividades_workflow_Id_Actividad =" & id_actividad &
             " and usuario_workflow_idU_suario = " & id_usuario_wf
            myCommand.CommandText = Parametro_Actualiza_System1
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_incrementa_registro_usuario_asignacion_ruta = "Imposible actualizar la tabla wf_registro_asignacion_ruta "
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_incrementa_registro_usuario_asignacion_ruta = "YES"
            Exit Function
        Catch ex As Exception
            If Not myTrans.Connection Is Nothing Then
                myConnection.Close()
                Actualiza_incrementa_registro_usuario_asignacion_ruta = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            Else
                Actualiza_incrementa_registro_usuario_asignacion_ruta = ex.Message
                Exit Function
            End If
        End Try

    End Function
    Function Solicita_existencia_registro_asignacion_actividad_usuario(ByVal id_actividad As Integer,
                                                                       ByVal id_usuario_worklflow As Integer,
                                                                       ByRef estado_asignacion As String) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita existencia de registro de asignacion usuario actividad
        '
        '-----------
        'Parametros:
        '-----------
        'id_actividad         :  identificación de la actividad workflow 
        'id_usuario_worklflow :  identifca el usuario workflow
        '-----------
        'Retorno   
        '----------
        'estado_asignacion    : YES si el usuario y la activdad tiene registro de asignación
        '
        ' 
        '----------
        'Fecha     : 2023-04-29
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select num_tareas  " &
               " From wf_registro_asignacion_ruta " &
               " WHERE listado_actividades_workflow_Id_Actividad=" & id_actividad &
               " AND  usuario_workflow_idU_suario=" & id_usuario_worklflow &
               " order by num_tareas asc limit 1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_asignacion_ruta")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_asignacion_actividad_usuario = "Función Solicita_existencia_registro_asignacion_actividad_usuario dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_asignacion = "NO"
                Solicita_existencia_registro_asignacion_actividad_usuario = "YES"
                Exit Function
            Else
                estado_asignacion = "YES"
                Solicita_existencia_registro_asignacion_actividad_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_asignacion_actividad_usuario = "Inconsistencia general funcion Solicita_existencia_registro_asignacion_actividad_usuario " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_registro_asignacion_actividad(ByVal id_actividad As Integer,
                                                               ByRef estado_asignacion As String) As String
        '---------------------------------------------------------------------------------------------------------------
        '---------
        'Funcion :
        '---------
        'Solicita existencia de registro de asignacion actividad
        '
        '-----------
        'Parametros:
        '-----------
        'id_actividad         :  identificación de la actividad workflow 
        '-----------
        'Retorno   
        '----------
        'estado_asignacion    : YES si el usuario y la activdad tiene registro de asignación
        '
        ' 
        '----------
        'Fecha     : 2023-04-29
        '----------
        '----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        '------------------------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select num_tareas  " &
               " From wf_registro_asignacion_ruta " &
               " WHERE listado_actividades_workflow_Id_Actividad=" & id_actividad &
               " order by num_tareas asc limit 1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_asignacion_ruta")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_asignacion_actividad = "Función Solicita_existencia_registro_asignacion_actividad dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_asignacion = "NO"
                Solicita_existencia_registro_asignacion_actividad = "YES"
                Exit Function
            Else
                estado_asignacion = "YES"
                Solicita_existencia_registro_asignacion_actividad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_asignacion_actividad = "Inconsistencia general funcion Solicita_existencia_registro_asignacion_actividad " & ex.Message
        End Try
    End Function
    Function Solicita_exitencia_registro_asignacion_usuario_ruta(ByVal id_usuario_wf As Integer,
                                                                 ByRef estado_asignacion As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita registro de usuario de asignacion
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        'id_usuario_wf      : Posible usuario registrado
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'estado_asignacion : Retorna estado de asignacion de usuario 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-08-19
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select *  " &
              " From wf_registro_asignacion_ruta " &
              " WHERE usuario_workflow_idu_suario=" & id_usuario_wf
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_registro_asignacion_ruta")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_exitencia_registro_asignacion_usuario_ruta = "Función Solicita_existencia_registro_asignacion_actividad dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_asignacion = "NO"
                Solicita_exitencia_registro_asignacion_usuario_ruta = "YES"
                Exit Function
            Else
                estado_asignacion = "YES"
                Solicita_exitencia_registro_asignacion_usuario_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_exitencia_registro_asignacion_usuario_ruta = "Inconsistencia general funcion Solicita_exitencia_registro_asignacion_usuario_ruta " & ex.Message
        End Try
    End Function
End Class
