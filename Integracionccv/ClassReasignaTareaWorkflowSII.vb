Public Class ClassReasignaTareaWorkflowSII
    Function ReasignaTareaUsuarioSIIWorkflow(ByVal IdTareaWorkflow As Long,
                                             ByVal IdActividadWorkflow As Integer,
                                             ByVal IdUsuarioWorflow As Integer,
                                             ByVal AsignaTareaSII As Integer,
                                             ByRef stru_list_usuarios As stru_list_usuarios) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Reasigna tarea usuario workflow con reasignación en el sistema SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'IdActividadWorkflow : Representa la identifcación de la tarea workflow
        'IdUsuarioWorflow    : Representa la identificación del usuario workflow
        'AsignaTareaSII      : Representa el estado si asigna la tarea en el SII
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-03
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            If HttpContext.Current.Session.Item("UTIL_GESTION_REASING_USER") = 0 Then
                ReasignaTareaUsuarioSIIWorkflow = "El usuario no tiene permisos para reasignar tareas del flujo de trabajo relacionadas con la integración al sistema SII. Por favor, contacte al administrador del sistema para solicitar acceso."
                Exit Function
            End If
            '//-----------Solicita la estructura de la tarea asignada-------//
            Dim StruEstado As stru_estado = Nothing
            Result = Class_estados_tarea_workflow.Solicita_estructura_tarea_asignada(IdTareaWorkflow,
                                                                                     StruEstado)
            If Result <> "YES" Then
                ReasignaTareaUsuarioSIIWorkflow = Result
                Exit Function
            End If
            '//----------Solicita la estructura de la actividad de flujo de trabajo---------///
            Dim Class_wf_registro_actividaes_flujos_trabajo As New Class_wf_registro_actividaes_flujos_trabajo
            Dim struregistro_actividaes_flujos_trabajo As struregistro_actividaes_flujos_trabajo = Nothing
            Dim IdActividadWorkflowRef As Integer = IdActividadWorkflow
            If StruEstado.ID_FLUJO_TRABAJO <> 0 Then
                Result = Class_wf_registro_actividaes_flujos_trabajo.Solicita_estructura_actividad_flujo_trabajo(IdActividadWorkflow,
                                                                                                                 struregistro_actividaes_flujos_trabajo)
                If Result <> "YES" Then
                    ReasignaTareaUsuarioSIIWorkflow = Result
                    Exit Function
                End If
                IdActividadWorkflowRef = struregistro_actividaes_flujos_trabajo.listado_actividades_workflow_Id_Actividad
            End If
            '//--------Valida y retorna los parametros para cambio de estado SII------//
            Dim EstadoSII As String = ""
            Dim RadicadoSII As String = ""
            Dim CodigoCortoSII As String = ""
            Dim Class_CambiaEstadoSii As New Class_CambiaEstadoSii
            If AsignaTareaSII = 1 Then
                Result = Class_CambiaEstadoSii.SolicitaDatosCambioEstadoSII(IdUsuarioWorflow,
                                                                            IdActividadWorkflowRef,
                                                                            IdTareaWorkflow,
                                                                            EstadoSII,
                                                                            RadicadoSII,
                                                                            CodigoCortoSII)
                If Result <> "YES" Then
                    ReasignaTareaUsuarioSIIWorkflow = Result
                    Exit Function
                End If
            End If
            Result = Class_estados_tarea_workflow.ReasignaTareaWorflowUsuarioSII(IdTareaWorkflow,
                                                                                 IdActividadWorkflowRef,
                                                                                 IdUsuarioWorflow,
                                                                                 StruEstado,
                                                                                 struregistro_actividaes_flujos_trabajo,
                                                                                 stru_list_usuarios.nombre_usuario,
                                                                                 stru_list_usuarios.cargo_usuario,
                                                                                 stru_list_usuarios.nombre_actividad)
            If Result <> "YES" Then
                ReasignaTareaUsuarioSIIWorkflow = Result
                Exit Function
            End If
            '//--------Cambio de estado SII------//
            If AsignaTareaSII = 1 Then
                Result = Class_CambiaEstadoSii.Cambia_estado_Radicado(EstadoSII,
                                                                      RadicadoSII,
                                                                      CodigoCortoSII)
                If Result <> "YES" Then
                    stru_list_usuarios.reault_cambio_estado = "La tarea se reasignó correctamente en el flujo de trabajo, 
                                                       pero no se logró actualizar el estado en el sistema SII. (" & Result & ")."
                Else
                    stru_list_usuarios.reault_cambio_estado = Result
                End If
            End If
            ReasignaTareaUsuarioSIIWorkflow = "YES"
        Catch ex As Exception
            ReasignaTareaUsuarioSIIWorkflow = "Inconsistencia general función ReasignaTareaUsuarioSIIWorkflow " & ex.Message
        End Try
    End Function
End Class
