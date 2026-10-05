Public Interface IRegistroTareaRutaSiiRepository
    Function TienePermiso(ByVal contexto As ContextoModulo) As Boolean
    Function ResolverContexto(ByVal contexto As ContextoModulo,
                              ByVal solicitud As SolicitudRegistroTareaRutaSii,
                              ByVal sii As ConsultaAutoritativaRegistroRutaSii) As DatosAutoritativosRegistroRutaSii
    Function RegistrarTareaConOutbox(ByVal contexto As ContextoModulo,
                                     ByVal datos As DatosAutoritativosRegistroRutaSii) As ResultadoRegistroTareaRutaSii
    Sub ConfirmarEvento(ByVal contexto As ContextoModulo, ByVal operationId As String, ByVal estado As String)
    Sub MarcarEventoPendiente(ByVal contexto As ContextoModulo, ByVal operationId As String, ByVal codigo As String)
End Interface

Public Interface IRelacionRutaSiiGateway
    Function Materializar(ByVal contexto As ContextoModulo, ByVal evento As EventoRelacionRutaSii) As String
End Interface

Public Interface IConsultaAutoritativaRutaSii
    Function Consultar(ByVal recibo As String, ByRef datos As ConsultaAutoritativaRegistroRutaSii) As Boolean
End Interface
