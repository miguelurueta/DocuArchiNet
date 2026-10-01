Public Interface IContextoAdjuntoRadicacionRepository
    Function ObtenerAutorizado(ByVal contextoModulo As ContextoModulo,
                               ByVal idRegistroEstado As Long,
                               ByRef contextoAdjunto As ContextoAdjuntoRadicacion) As String
End Interface
