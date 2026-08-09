Public Class Class_inicializa_gestion_correspondencia
    Function Inicializa_gestion_correspondencia(ByVal Id_Usuario_Workflow As Integer,
                                                ByVal Id_Ruta_Workflow As Integer,
                                                ByVal Id_Grupo_Workflow As Integer) As String
        Try
            Dim Refclas_workflow_ruta As New Class_worflow_rutas
            Dim Result As String = ""
            Result = Refclas_workflow_ruta.Solicita_nombre_ruta_workflow(Id_Ruta_Workflow.ToString,
                                                                         HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"))
            If Result <> "YES" Then
                Inicializa_gestion_correspondencia = Result
                Exit Function
            End If
            Dim Refclas_config_list_ruta As New Class_configuracion_listado_ruta
            Result = Refclas_config_list_ruta.Solicita_campos_lista_tramite(Id_Ruta_Workflow,
                                                                            HttpContext.Current.Session.Item("WF_CAMPOS_LISTA_TRAMITE"))
            If Result <> "YES" Then
                Inicializa_gestion_correspondencia = Result
                Exit Function
            End If
            Dim ref_ref_Class_grupos_workflow As New Class_grupos_workflow
            ref_ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"),
                                                                                 Id_Grupo_Workflow)
            Inicializa_gestion_correspondencia = "YES"
            Exit Function
            Dim Refclas_ini_workflow As New InicioWorkflow
            'Result = Refclas_ini_workflow.Crea_Dir_Temporal_wf()
            'If Result <> "YES" Then
            'Inicializa_gestion_correspondencia = Result
            'Exit Function
            'End If
            Result = Refclas_ini_workflow.Inicializa_firma_usuario_workflow()
            If Result <> "YES" Then
                Inicializa_gestion_correspondencia = Result
                Exit Function
            End If
        Catch ex As Exception
            Inicializa_gestion_correspondencia = "Inconsistencia general función Inicializa_gestion_correspondencia " & ex.Message
        End Try
    End Function
End Class
