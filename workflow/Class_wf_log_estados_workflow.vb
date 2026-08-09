Public Structure Stru_wf_log_estados_workflow
    Dim id_log_estados_workflow As Long
    Dim usuario_workflow_idU_suario As Integer
    Dim id_tarea_workflow As Long
    Dim estados_tarea_workflow_id_Estado As Long
    Dim estados_tarea_siguiente_workflow_id_Estado As Long
    Dim tipo_transacion As String
    Dim id_actividad_siguiente As Integer
    Dim id_usuario_siguiente As Integer
    Dim id_actividad_anterior As Integer
    Dim id_usuario_anterior As Integer
    Dim fecha_registro As String
    Dim Direccion_ip_Nombre As String
End Structure
Public Class Class_wf_log_estados_workflow
    Function Registra_log_estado_tarea_worlkflow(ByVal Stru_wf_log_estados_workflow As Stru_wf_log_estados_workflow) As String
        '---------------------------------------------------------------------------
        'Funcion : Reasigna log de reasignacion de tare funcion SII
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Stru_wf_log_estados_workflow  : Respresenta la estructura del registro
        'del log de la tarea                                  
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-12
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "insert into wf_log_estados_workflow (usuario_workflow_idU_suario,id_tarea_workflow," &
              "estados_tarea_workflow_id_Estado,tipo_transacion,id_actividad_anterior,id_usuario_anterior,fecha_registro,Direccion_ip_Nombre," &
             "id_actividad_siguiente,id_usuario_siguiente,estados_tarea_siguiente_workflow_id_Estado) values (" &
              Stru_wf_log_estados_workflow.usuario_workflow_idU_suario & "," & Stru_wf_log_estados_workflow.id_tarea_workflow & "," &
              Stru_wf_log_estados_workflow.estados_tarea_workflow_id_Estado & ",'" & Stru_wf_log_estados_workflow.tipo_transacion & "'," &
              Stru_wf_log_estados_workflow.id_actividad_anterior & "," & Stru_wf_log_estados_workflow.id_usuario_anterior & ",'" &
              Stru_wf_log_estados_workflow.fecha_registro & "','" & Stru_wf_log_estados_workflow.Direccion_ip_Nombre & "'," &
              Stru_wf_log_estados_workflow.id_actividad_siguiente & "," & Stru_wf_log_estados_workflow.id_usuario_siguiente & "," &
              Stru_wf_log_estados_workflow.estados_tarea_siguiente_workflow_id_Estado & ")"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(Sql_consulta)
            Registra_log_estado_tarea_worlkflow = Result
            Exit Function
        Catch ex As Exception
            Registra_log_estado_tarea_worlkflow = "Inconsistencia general funcion Registra_log_estado_tarea_worlkflow " & ex.Message
        End Try
    End Function
End Class
