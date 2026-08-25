' Puertos de devolución; sus implementaciones mantienen la semántica Ruta y Flujo separada.
Public Interface IDevolverActividadTareaRepository
    Function ObtenerTarea(ByVal contexto As ContextoModuloWorkflow,
                          ByVal idTarea As Long) As TareaDevolverActividad
End Interface

Public Interface IDevolverActividadAutorizacionRepository
    Function Evaluar(ByVal contexto As ContextoModuloWorkflow,
                    ByVal tarea As TareaDevolverActividad) As ResultadoAutorizacionDevolverActividad
End Interface

Public Interface IDevolverActividadPreviewRepository
    Function BuscarDestinos(ByVal contexto As ContextoModuloWorkflow,
                            ByVal tarea As TareaDevolverActividad,
                            ByVal solicitud As SolicitudPreviewDevolverActividad) As ResultadoBusquedaDevolverActividad
End Interface

Public Interface IDevolverActividadEjecucionRepository
    Function ResolverDestino(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaDevolverActividad,
                             ByVal idConector As Integer) As ResultadoResolucionDevolverActividad
End Interface

Public Interface IDevolverActividadCursorCodec
    Function Proteger(ByVal contexto As ContextoModuloWorkflow,
                      ByVal tarea As TareaDevolverActividad,
                      ByVal terminoNormalizado As String,
                      ByVal destino As DestinoDevolverActividad) As String

    Function Validar(ByVal contexto As ContextoModuloWorkflow,
                     ByVal tarea As TareaDevolverActividad,
                     ByVal terminoNormalizado As String,
                     ByVal cursor As String,
                     ByRef idConector As Integer,
                     ByRef orden As Integer) As Boolean
End Interface

Public Interface IDevolverActividadConcurrencyLease
    Inherits System.IDisposable
End Interface

Public Interface IDevolverActividadConcurrencyGuard
    Function Adquirir(ByVal contexto As ContextoModuloWorkflow,
                      ByVal idTarea As Long) As ResultadoGuardDevolverActividad
End Interface

Public Interface IDevolverActividadAuditoriaRepository
    Function Registrar(ByVal auditoria As AuditoriaDevolverActividad) As Boolean
End Interface

Public Interface IDevolverActividadLegacyExecutor
    Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                      ByVal tarea As TareaDevolverActividad,
                      ByVal destino As DestinoDevolverActividad) As ResultadoEjecucionDevolverActividad
End Interface
