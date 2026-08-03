Public Structure ra_log_error_pqr_publico
    Dim id_log_error_pqr As Integer
    Dim id_plantilla_radicacion As Integer
    Dim id_plantilla_validacion As Integer
    Dim consecutivo_radicado As String
    Dim error_code As String
    Dim tipo_error As Integer
    Dim ruta_file As String
    Dim nombre_peticionario As String
    Dim identificacion_peticionario As String
    Dim descripcion_solictud As String
    Dim asunto As String
    Dim correo_peticionario As String
    Dim date_registro As String
    Dim id_actividad_usuario_workflow As Integer
    Dim id_usuario_workflow As Integer
    Dim id_imagen_registro_tramite As Integer
    Dim id_flujo_trabajo As Integer
    Dim id_registro_actvidad_flujo_trabajo As Integer
    Dim id_usuario_workflow_flujo_trabajo As Integer
    Dim estado_recuperacion_workflow_flujo_trabajo As Integer
    Dim estado_modulo_respuesta As Integer
    Dim id_tarea_workflow As Integer
    Dim fecha_selecion As String
End Structure
Public Class ra_log_error_pqr_publico_
    Public Property id_log_error_pqr As Integer
    Public Property id_plantilla_radicacion As Integer
    Public Property id_plantilla_validacion As Integer
    Public Property consecutivo_radicado As String
    Public Property error_code As String
    Public Property tipo_error As Integer
    Public Property ruta_file As String
    Public Property nombre_peticionario As String
    Public Property identificacion_peticionario As String
    Public Property descripcion_solictud As String
    Public Property asunto As String
    Public Property correo_peticionario As String
    Public Property date_registro As String
    Public Property id_actividad_usuario_workflow As Integer
    Public Property id_usuario_workflow As Integer
    Public Property id_imagen_registro_tramite As Integer
    Public Property id_flujo_trabajo As Integer
    Public Property id_registro_actvidad_flujo_trabajo As Integer
    Public Property id_usuario_workflow_flujo_trabajo As Integer
    Public Property estado_recuperacion_workflow_flujo_trabajo As Integer
    Public Property estado_modulo_respuesta As Integer
    Public Property id_tarea_workflow As Integer
    Public Property fecha_selecion As String
End Class
Public Class Class_ra_log_error_pqr_publico
    Function Registra_error_log_radicado_pqr_publico(ByVal Ra_log_error_pqr_publico() As ra_log_error_pqr_publico) As String
        Try
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim sql_insert As String = ""
            Dim sql_campos_insert As String = "Insert into ra_log_error_pqr_publico (id_plantilla_radicacion,id_plantilla_validacion," &
                "consecutivo_radicado,error_code,tipo_error,ruta_file,nombre_peticionario,identificacion_peticionario,correo_peticionario," &
                "descripcion_solictud,date_registro,asunto,id_actividad_usuario_workflow,id_usuario_workflow,id_imagen_registro_tramite," &
                "id_flujo_trabajo,id_registro_actvidad_flujo_trabajo,id_usuario_workflow_flujo_trabajo,estado_recuperacion_workflow_flujo_trabajo," &
                "estado_modulo_respuesta,id_tarea_workflow,fecha_selecion) values "
            Dim sql_values As String = ""
            For i As Integer = 0 To Ra_log_error_pqr_publico.Length - 1
                If i = 0 Then
                    sql_values = "('" & Ra_log_error_pqr_publico(i).id_plantilla_radicacion & "','" &
                    Ra_log_error_pqr_publico(i).id_plantilla_validacion & "','" &
                    Ra_log_error_pqr_publico(i).consecutivo_radicado & "','" &
                    Ra_log_error_pqr_publico(i).error_code & "','" &
                    Ra_log_error_pqr_publico(i).tipo_error & "','" &
                    Ra_log_error_pqr_publico(i).ruta_file & "','" &
                    Ra_log_error_pqr_publico(i).nombre_peticionario & "','" &
                    Ra_log_error_pqr_publico(i).identificacion_peticionario & "','" &
                    Ra_log_error_pqr_publico(i).correo_peticionario & "','" &
                    Ra_log_error_pqr_publico(i).descripcion_solictud & "','" &
                    Ra_log_error_pqr_publico(i).date_registro & "','" &
                    Ra_log_error_pqr_publico(i).asunto & "','" &
                    Ra_log_error_pqr_publico(i).id_actividad_usuario_workflow & "','" &
                    Ra_log_error_pqr_publico(i).id_usuario_workflow & "','" &
                    Ra_log_error_pqr_publico(i).id_imagen_registro_tramite & "','" &
                    Ra_log_error_pqr_publico(i).id_flujo_trabajo & "','" &
                    Ra_log_error_pqr_publico(i).id_registro_actvidad_flujo_trabajo & "','" &
                    Ra_log_error_pqr_publico(i).id_usuario_workflow_flujo_trabajo & "','" &
                    Ra_log_error_pqr_publico(i).estado_recuperacion_workflow_flujo_trabajo & "','" &
                    Ra_log_error_pqr_publico(i).estado_modulo_respuesta & "','" &
                    Ra_log_error_pqr_publico(i).id_tarea_workflow & "','" &
                    Ra_log_error_pqr_publico(i).fecha_selecion & "')"
                Else
                    sql_values = sql_values & ", ('" & Ra_log_error_pqr_publico(i).id_plantilla_radicacion & "','" &
                    Ra_log_error_pqr_publico(i).id_plantilla_validacion & "','" &
                    Ra_log_error_pqr_publico(i).consecutivo_radicado & "','" &
                    Ra_log_error_pqr_publico(i).error_code & "','" &
                    Ra_log_error_pqr_publico(i).tipo_error & "','" &
                    Ra_log_error_pqr_publico(i).ruta_file & "','" &
                    Ra_log_error_pqr_publico(i).nombre_peticionario & "','" &
                    Ra_log_error_pqr_publico(i).identificacion_peticionario & "','" &
                    Ra_log_error_pqr_publico(i).correo_peticionario & "','" &
                    Ra_log_error_pqr_publico(i).descripcion_solictud & "','" &
                    Ra_log_error_pqr_publico(i).date_registro & "','" &
                    Ra_log_error_pqr_publico(i).asunto & "','" &
                    Ra_log_error_pqr_publico(i).id_actividad_usuario_workflow & "','" &
                    Ra_log_error_pqr_publico(i).id_usuario_workflow & "','" &
                    Ra_log_error_pqr_publico(i).id_imagen_registro_tramite & "','" &
                    Ra_log_error_pqr_publico(i).id_flujo_trabajo & "','" &
                    Ra_log_error_pqr_publico(i).id_registro_actvidad_flujo_trabajo & "','" &
                    Ra_log_error_pqr_publico(i).id_usuario_workflow_flujo_trabajo & "','" &
                    Ra_log_error_pqr_publico(i).estado_recuperacion_workflow_flujo_trabajo & "','" &
                    Ra_log_error_pqr_publico(i).estado_modulo_respuesta & "','" &
                    Ra_log_error_pqr_publico(i).id_tarea_workflow & "','" &
                    Ra_log_error_pqr_publico(i).fecha_selecion & "')"
                End If

            Next
            sql_insert = sql_campos_insert & sql_values
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Registra_error_log_radicado_pqr_publico = "Funcion Registra_error_log_radicado_pqr_publico dice " & Result
                Exit Function
            Else
                Registra_error_log_radicado_pqr_publico = Result
                Exit Function
            End If
        Catch ex As Exception
            Registra_error_log_radicado_pqr_publico = "Inconsistencia general funcion Registra_error_log_radicado_pqr_publico " & ex.Message
        End Try
    End Function
End Class
