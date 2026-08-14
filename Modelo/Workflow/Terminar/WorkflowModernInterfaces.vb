Imports System.Collections.Generic

'Puertos de negocio: sus implementaciones viven en Infrastructure y nunca leen Session.
Public Interface ITareaWorkflowRepository
    Function ObtenerTarea(ByVal contexto As ContextoModuloWorkflow, ByVal idTarea As Long) As TareaWorkflow
End Interface

Public Interface ITransicionFlujoRepository
    Function ObtenerDestinos(ByVal contexto As ContextoModuloWorkflow, ByVal tarea As TareaWorkflow) As ResultadoDestinosTransicion
End Interface

Public Interface ITransicionRutaRepository
    Function ObtenerDestinos(ByVal contexto As ContextoModuloWorkflow, ByVal tarea As TareaWorkflow) As ResultadoDestinosTransicion
End Interface

Public Interface IConfiguracionConectorRepository
    Function EsConectorDisponible(ByVal contexto As ContextoModuloWorkflow,
                                  ByVal tarea As TareaWorkflow,
                                  ByVal idConector As Integer) As Boolean
End Interface

Public Interface IAuditoriaTransicionRepository
    Sub Registrar(ByVal auditoria As AuditoriaTransicion)
End Interface

Public Interface IWorkflowModernFeatureGate
    Function Evaluar(ByVal contexto As ContextoModuloWorkflow) As HabilitacionWorkflowModern
End Interface

Public Interface IWorkflowLegacyExecutor
    Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                      ByVal solicitud As SolicitudTransicionWorkflow) As ResultadoEjecucionWorkflow
End Interface
