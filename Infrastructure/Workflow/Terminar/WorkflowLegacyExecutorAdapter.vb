'Unico limite nuevo reservado para ClassWorkflow.Terminar_Tarea_Workflow y Cambia_Estado.
'DOC-9 no ejecuta el motor: aun no existe endpoint ni una forma segura de reconstruir Page/Session.
Public Class WorkflowLegacyExecutorAdapter
    Implements IWorkflowLegacyExecutor

    Public Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal solicitud As SolicitudTransicionWorkflow) As ResultadoEjecucionWorkflow Implements IWorkflowLegacyExecutor.Ejecutar
        Return New ResultadoEjecucionWorkflow With {
            .Exito = False,
            .EstadoFinal = "pendiente",
            .CodigoBloqueo = "WORKFLOW_MODERN_EXECUTION_PENDING",
            .MensajeFuncional = "La ejecucion moderna aun no esta habilitada.",
            .ReferenciaAuditoria = String.Empty,
            .EsReintentable = False
        }
    End Function
End Class
