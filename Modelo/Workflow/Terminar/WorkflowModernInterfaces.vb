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

Public Interface ITransicionEjecucionRepository
    Function ResolverDestino(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaWorkflow,
                             ByVal idConector As Integer) As ResultadoResolucionDestinoTransicion
End Interface

'Puertos de Enviar a grupo. Mantienen separado el envio directo de la transicion por conector.
Public Interface IEnvioGrupoDestinosRepository
    Function ObtenerDestinos(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaWorkflow) As ResultadoDestinosEnvioGrupo
End Interface

Public Interface IEnvioGrupoBusquedaRepository
    Function BuscarDestinos(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaWorkflow,
                            ByVal solicitud As SolicitudBusquedaDestinosEnvioGrupo) As ResultadoBusquedaDestinosEnvioGrupo
End Interface

Public Interface IEnvioGrupoEjecucionRepository
    Function ResolverDestino(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaWorkflow,
                             ByVal idActividadDestino As Integer) As ResultadoResolucionEnvioGrupo
End Interface

Public Interface IEnvioGrupoRequisitosRepository
    Function Evaluar(ByVal contexto As ContextoModuloWorkflow,
                     ByVal tarea As TareaWorkflow,
                     ByVal destino As DestinoEnvioGrupoWorkflow) As ResultadoRequisitosEnvioGrupo
End Interface

Public Interface IRequisitosTransicionRepository
    Function Evaluar(ByVal contexto As ContextoModuloWorkflow,
                     ByVal tarea As TareaWorkflow,
                     ByVal destino As DestinoEjecucionWorkflow) As ResultadoRequisitosTransicion
End Interface

Public Interface IAuditoriaTransicionRepository
    'El contrato solo recibe telemetría estructurada y sanitizada; nunca Session, SQL ni payloads.
    Function Registrar(ByVal auditoria As AuditoriaTransicion) As Boolean
End Interface

Public Interface ITransicionConcurrencyLease
    Inherits System.IDisposable
End Interface

Public Interface ITransicionConcurrencyGuard
    Function Adquirir(ByVal contexto As ContextoModuloWorkflow,
                      ByVal idTarea As Long,
                      ByVal tokenVersion As String) As ResultadoGuardTransicion
End Interface

Public Interface IWorkflowModernFeatureGate
    Function Evaluar(ByVal contexto As ContextoModuloWorkflow) As HabilitacionWorkflowModern
End Interface

Public Interface IWorkflowLegacyExecutor
    Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                      ByVal tarea As TareaWorkflow,
                      ByVal destino As DestinoEjecucionWorkflow) As ResultadoEjecucionWorkflow
End Interface

Public Interface IEnvioGrupoLegacyExecutor
    Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                      ByVal tarea As TareaWorkflow,
                      ByVal destino As DestinoEnvioGrupoWorkflow) As ResultadoEjecucionWorkflow
End Interface
