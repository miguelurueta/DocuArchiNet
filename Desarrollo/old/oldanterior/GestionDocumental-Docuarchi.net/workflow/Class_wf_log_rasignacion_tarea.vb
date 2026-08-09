Public Structure Stru_wf_log_rasignacion_tarea
    Dim id_log_rasignacion_tarea As Long
    Dim usuario_workflow_idU_suario As Integer
    Dim id_usuario_workflow_flujo_trabajo As Integer
    Dim usuario_workflow_rasigna As Integer
    Dim id_tarea_workflow As Long
    Dim estados_tarea_workflow_id_Estado As Long
    Dim id_actividad As Integer
    Dim fecha_registro As String
    Dim Direccion_ip_Nombre As String
    Dim id_actividad_flujo_trabajo As Integer
    Dim id_flujo_trabajo As Integer
End Structure
Public Class Class_wf_log_rasignacion_tarea
    Function Registra_log_reasignacion_tarea_workflow(ByVal Stru_wf_log_rasignacion_tarea As Stru_wf_log_rasignacion_tarea) As String
        '---------------------------------------------------------------------------
        'Funcion : Reasigna log de reasignacion de tarea manual
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Stru_wf_log_rasignacion_tarea  : Respresenta la estructura del registro
        'del log de la reasignación de la tarea                              
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-09-26
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_insert As String = "insert into wf_log_rasignacion_tarea (estados_tarea_workflow_id_Estado,id_tarea_workflow,id_actividad_flujo_trabajo" &
              ",id_actividad,usuario_workflow_idU_suario,id_usuario_workflow_flujo_trabajo,id_flujo_trabajo,fecha_registro," &
             "usuario_workflow_rasigna) values (" &
              Stru_wf_log_rasignacion_tarea.estados_tarea_workflow_id_Estado & "," &
              Stru_wf_log_rasignacion_tarea.id_tarea_workflow & "," &
              Stru_wf_log_rasignacion_tarea.id_actividad_flujo_trabajo & "," &
              Stru_wf_log_rasignacion_tarea.id_actividad & "," &
              Stru_wf_log_rasignacion_tarea.usuario_workflow_idU_suario &
              "," & Stru_wf_log_rasignacion_tarea.id_usuario_workflow_flujo_trabajo & "," &
              Stru_wf_log_rasignacion_tarea.id_flujo_trabajo & ",'" &
              Stru_wf_log_rasignacion_tarea.fecha_registro & "'," &
              Stru_wf_log_rasignacion_tarea.usuario_workflow_rasigna & ")"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(Sql_insert)
            Registra_log_reasignacion_tarea_workflow = Result
            Exit Function
        Catch ex As Exception
            Registra_log_reasignacion_tarea_workflow = "Inconsistencia general funcion Registra_log_reasignacion_tarea_workflow " & ex.Message
        End Try
    End Function
End Class
