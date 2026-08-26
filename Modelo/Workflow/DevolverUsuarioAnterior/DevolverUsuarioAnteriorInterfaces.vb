'Puertos de devolución a usuario anterior. Ninguno acepta destino ni historial del cliente ni depende de controles de presentación.
Public Interface IDevolverUsuarioAnteriorTareaRepository
    Function ObtenerTarea(ByVal contexto As ContextoModuloWorkflow,
                          ByVal idTarea As Long) As TareaDevolverUsuarioAnterior
End Interface

Public Interface IDevolverUsuarioAnteriorAutorizacionRepository
    Function Evaluar(ByVal contexto As ContextoModuloWorkflow,
                    ByVal tarea As TareaDevolverUsuarioAnterior) As ResultadoAutorizacionDevolverUsuarioAnterior
End Interface

Public Interface IDevolverUsuarioAnteriorHistorialRepository
    Function ObtenerAntecedente(ByVal contexto As ContextoModuloWorkflow,
                                ByVal tarea As TareaDevolverUsuarioAnterior) As ResultadoHistorialDevolverUsuarioAnterior
End Interface

Public Interface IDevolverUsuarioAnteriorTokenCodec
    Function Emitir(ByVal contexto As ContextoModuloWorkflow,
                   ByVal tarea As TareaDevolverUsuarioAnterior,
                   ByVal usuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior) As String

    Function Validar(ByVal contexto As ContextoModuloWorkflow,
                     ByVal tarea As TareaDevolverUsuarioAnterior,
                     ByVal usuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior,
                     ByVal tokenVersion As String) As Boolean
End Interface

Public Interface IDevolverUsuarioAnteriorConcurrencyLease
    Inherits System.IDisposable
End Interface

Public Interface IDevolverUsuarioAnteriorConcurrencyGuard
    Function Adquirir(ByVal contexto As ContextoModuloWorkflow,
                      ByVal idTarea As Long) As ResultadoGuardDevolverUsuarioAnterior
End Interface

Public Interface IDevolverUsuarioAnteriorLegacyExecutor
    Function Ejecutar(ByVal contexto As ContextoModuloWorkflow,
                      ByVal tarea As TareaDevolverUsuarioAnterior,
                      ByVal usuarioHistorico As UsuarioHistoricoDevolverUsuarioAnterior) As ResultadoEjecucionDevolverUsuarioAnterior
End Interface

Public Interface IDevolverUsuarioAnteriorAuditoriaRepository
    Function Registrar(ByVal auditoria As AuditoriaDevolverUsuarioAnterior) As Boolean
End Interface
