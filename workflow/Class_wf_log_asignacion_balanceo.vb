Public Structure stru_registro_balanceo
    Dim id_log_asignacion_balanceo As Long
    Dim estados_tarea_workflow_id_Estado As Long
    Dim id_tarea_workflow As Long
    Dim usuario_workflow_idU_suario As Integer
    Dim id_usuario_workflow_flujo_trabajo As Integer
    Dim id_actividad_flujo_trabajo As Integer
    Dim id_usuario_reasigna As Integer
    Dim id_actividad As Integer
    Dim id_flujo_trabajo As String
    Dim fecha_registro As String
End Structure
Public Class Class_wf_log_asignacion_balanceo
    Function Registro_log_balanceo(ByVal stru_registro_balanceo As stru_registro_balanceo) As String
        '---------------------------------------------------------------------------
        'Funcion : Registra log del registro de balanceo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'stru_registro_balanceo  : Estructura del registro de asignación de balanceo
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        ' 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-02
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "insert into wf_log_asignacion_balanceo (estados_tarea_workflow_id_Estado,id_tarea_workflow," &
                "usuario_workflow_idU_suario,id_flujo_trabajo,fecha_registro,id_actividad_flujo_trabajo,id_actividad,id_usuario_workflow_flujo_trabajo,usuario_workflow_rasigna) values (" &
                stru_registro_balanceo.estados_tarea_workflow_id_Estado & "," & stru_registro_balanceo.id_tarea_workflow & "," &
                stru_registro_balanceo.usuario_workflow_idU_suario & "," & stru_registro_balanceo.id_flujo_trabajo & ",'" &
                stru_registro_balanceo.fecha_registro & "'," & stru_registro_balanceo.id_actividad_flujo_trabajo & "," &
                stru_registro_balanceo.id_actividad & "," & stru_registro_balanceo.id_usuario_workflow_flujo_trabajo & "," & stru_registro_balanceo.id_usuario_reasigna & ")"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(Sql_consulta)
            Registro_log_balanceo = Result
            Exit Function
        Catch ex As Exception
            Registro_log_balanceo = "Inconsistencia general funcion Registro_log_balanceo " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_registro_balanceo(ByVal id_estado_tarea As Long,
                                                   ByRef stru_registro_balanceo As stru_registro_balanceo) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita estructura de registro de asignacion de balanceo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_estado_tarea       : Respresenta el estado de la tarea workflow
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_registro_balanceo  : Estructura del registro de asignación de balanceo
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-02
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_log_asignacion_balanceo,estados_tarea_workflow_id_Estado,id_tarea_workflow," &
                "usuario_workflow_idU_suario,id_flujo_trabajo,fecha_registro,id_actividad_flujo_trabajo,id_actividad,id_usuario_workflow_flujo_trabajo" &
                " FROM  wf_log_asignacion_balanceo " &
                " WHERE  estados_tarea_workflow_id_Estado=" & id_estado_tarea
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_log_asignacion_balanceo")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_registro_balanceo = "Funcion  Solicita_estructura_registro_balanceo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                stru_registro_balanceo.id_log_asignacion_balanceo = 0
                stru_registro_balanceo.estados_tarea_workflow_id_Estado = 0
                stru_registro_balanceo.id_tarea_workflow = 0
                stru_registro_balanceo.usuario_workflow_idU_suario = 0
                stru_registro_balanceo.id_usuario_workflow_flujo_trabajo = 0
                stru_registro_balanceo.id_actividad_flujo_trabajo = 0
                stru_registro_balanceo.id_actividad = 0
                stru_registro_balanceo.id_flujo_trabajo = 0
                stru_registro_balanceo.fecha_registro = ""
                Solicita_estructura_registro_balanceo = "YES"
                Exit Function
            Else
                stru_registro_balanceo.id_log_asignacion_balanceo = Datset.Tables(0).Rows(0).Item(0)
                stru_registro_balanceo.estados_tarea_workflow_id_Estado = Datset.Tables(0).Rows(0).Item(1)
                stru_registro_balanceo.id_tarea_workflow = Datset.Tables(0).Rows(0).Item(2)
                If Datset.Tables(0).Rows(0).IsNull(3) Then
                    stru_registro_balanceo.usuario_workflow_idU_suario = 0
                Else
                    stru_registro_balanceo.usuario_workflow_idU_suario = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) Then
                    stru_registro_balanceo.id_flujo_trabajo = 0
                Else
                    stru_registro_balanceo.id_flujo_trabajo = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) Then
                    stru_registro_balanceo.fecha_registro = ""
                Else
                    stru_registro_balanceo.fecha_registro = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    stru_registro_balanceo.id_actividad_flujo_trabajo = 0
                Else
                    stru_registro_balanceo.id_actividad_flujo_trabajo = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    stru_registro_balanceo.id_actividad = 0
                Else
                    stru_registro_balanceo.id_actividad = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    stru_registro_balanceo.id_usuario_workflow_flujo_trabajo = 0
                Else
                    stru_registro_balanceo.id_usuario_workflow_flujo_trabajo = Datset.Tables(0).Rows(0).Item(8)
                End If
                Solicita_estructura_registro_balanceo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_registro_balanceo = "Inconsistencia general funcion Solicita_estructura_registro_balanceo " & ex.Message
        End Try
    End Function
    Function Solicita_estado_registro_balanceo_tarea_usuario(ByVal id_usuario_workflow As Integer,
                                                             ByVal id_tarea_workflow As Long,
                                                             ByVal estado_tarea As Long,
                                                             ByRef estado_registro_balanceo As Integer) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita exitencia estado registro de la tarea a traves de balanceo
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------------
        'estado_tarea          : Respresenta el estado de la tarea workflow
        'id_usuario_workflow   : Representa el usuario workflow de la tarea
        'id_tarea_workflow     : Representa la identificación de la tarea
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'estado_registro_balanceo : Estado registro de la tarea (1) positivo (0) negativo
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2023-09-29
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_log_asignacion_balanceo" &
                " FROM  wf_log_asignacion_balanceo " &
                " WHERE  estados_tarea_workflow_id_Estado=" & estado_tarea & " and usuario_workflow_idU_suario=" & id_usuario_workflow &
                " and id_tarea_workflow=" & id_tarea_workflow
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_log_asignacion_balanceo")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_registro_balanceo_tarea_usuario = "Funcion  Solicita_estado_registro_balanceo_tarea_usuario " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_registro_balanceo = 0
                Solicita_estado_registro_balanceo_tarea_usuario = "YES"
                Exit Function
            Else
                estado_registro_balanceo = 1
                Solicita_estado_registro_balanceo_tarea_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_registro_balanceo_tarea_usuario = "Inconsistencia general funcion  Solicita_estado_registro_balanceo_tarea_usuario " & ex.Message
        End Try
    End Function
    Function Solicita_estado_registro_balanceo_tarea_usuario_flujo(ByVal id_usuario_workflow As Integer,
                                                                   ByVal id_tarea_workflow As Long,
                                                                   ByVal estado_tarea As Long,
                                                                   ByVal id_actividad_flujo_trabajo As Integer,
                                                                   ByRef estado_registro_balanceo As Integer) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita exitencia estado registro de la tarea a traves de balanceo
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------------
        'estado_tarea          : Respresenta el estado de la tarea workflow
        'id_usuario_workflow   : Representa el usuario workflow de la tarea
        'id_tarea_workflow     : Representa la identificación de la tarea
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'estado_registro_balanceo : Estado registro de la tarea (1) positivo (0) negativo
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2023-10-02
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT id_log_asignacion_balanceo" &
                " FROM  wf_log_asignacion_balanceo " &
                " WHERE  estados_tarea_workflow_id_Estado=" & estado_tarea & " and usuario_workflow_idU_suario=" & id_usuario_workflow &
                " and id_tarea_workflow=" & id_tarea_workflow & " and id_actividad_flujo_trabajo=" & id_actividad_flujo_trabajo
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_log_asignacion_balanceo")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_registro_balanceo_tarea_usuario_flujo = "Funcion  Solicita_estado_registro_balanceo_tarea_usuario_flujo " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_registro_balanceo = 0
                Solicita_estado_registro_balanceo_tarea_usuario_flujo = "YES"
                Exit Function
            Else
                estado_registro_balanceo = 1
                Solicita_estado_registro_balanceo_tarea_usuario_flujo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_registro_balanceo_tarea_usuario_flujo = "Inconsistencia general funcion  Solicita_estado_registro_balanceo_tarea_usuario_flujo " & ex.Message
        End Try
    End Function
End Class
