'Puertos de Notas Workflow. Ninguno recibe Session, Page, HttpContext, identidad, permiso ni metadatos de ruta del cliente.
Public Interface INotasWorkflowRepository
    Function Listar(ByVal contexto As ContextoModuloWorkflow,
                    ByVal tarea As TareaWorkflow,
                    ByVal solicitud As SolicitudListarNotasWorkflow) As ResultadoNotasWorkflow

    Function Contar(ByVal contexto As ContextoModuloWorkflow,
                    ByVal tarea As TareaWorkflow,
                    ByVal solicitud As SolicitudContarNotasWorkflow) As ResultadoNotasWorkflow

    Function Crear(ByVal contexto As ContextoModuloWorkflow,
                  ByVal tarea As TareaWorkflow,
                  ByVal solicitud As SolicitudCrearNotaWorkflow) As ResultadoNotasWorkflow

    Function Consultar(ByVal contexto As ContextoModuloWorkflow,
                       ByVal tarea As TareaWorkflow,
                       ByVal solicitud As SolicitudConsultarNotaWorkflow) As ResultadoNotasWorkflow

    Function Actualizar(ByVal contexto As ContextoModuloWorkflow,
                        ByVal tarea As TareaWorkflow,
                        ByVal solicitud As SolicitudActualizarNotaWorkflow) As ResultadoNotasWorkflow

    Function Eliminar(ByVal contexto As ContextoModuloWorkflow,
                      ByVal tarea As TareaWorkflow,
                      ByVal solicitud As SolicitudEliminarNotaWorkflow) As ResultadoNotasWorkflow
End Interface

Public Interface INotasWorkflowCursorCodec
    Function Proteger(ByVal contexto As ContextoModuloWorkflow,
                      ByVal tarea As TareaWorkflow,
                      ByVal fechaCreacionUtc As DateTime,
                      ByVal idNota As Long) As String

    Function Validar(ByVal contexto As ContextoModuloWorkflow,
                     ByVal tarea As TareaWorkflow,
                     ByVal cursor As String,
                     ByRef fechaCreacionUtc As DateTime,
                     ByRef idNota As Long) As Boolean
End Interface

Public Interface IServicioNotasWorkflow
    Function Listar(ByVal contexto As ContextoModuloWorkflow,
                    ByVal solicitud As SolicitudListarNotasWorkflow) As ResultadoNotasWorkflow

    Function Contar(ByVal contexto As ContextoModuloWorkflow,
                    ByVal solicitud As SolicitudContarNotasWorkflow) As ResultadoNotasWorkflow

    Function Crear(ByVal contexto As ContextoModuloWorkflow,
                  ByVal solicitud As SolicitudCrearNotaWorkflow) As ResultadoNotasWorkflow

    Function Consultar(ByVal contexto As ContextoModuloWorkflow,
                       ByVal solicitud As SolicitudConsultarNotaWorkflow) As ResultadoNotasWorkflow

    Function Actualizar(ByVal contexto As ContextoModuloWorkflow,
                        ByVal solicitud As SolicitudActualizarNotaWorkflow) As ResultadoNotasWorkflow

    Function Eliminar(ByVal contexto As ContextoModuloWorkflow,
                      ByVal solicitud As SolicitudEliminarNotaWorkflow) As ResultadoNotasWorkflow
End Interface
