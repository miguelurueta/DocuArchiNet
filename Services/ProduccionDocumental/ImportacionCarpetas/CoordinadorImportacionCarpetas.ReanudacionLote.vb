Option Strict On
Imports System

Public Partial Class CoordinadorImportacionCarpetas
    'Misma regla para la pista de consulta y el control transaccional.
    Friend Shared Function MotivoControlLote(lote As LoteImportacionCarpetas, activos As Boolean, inciertos As Boolean) As String
        If lote Is Nothing OrElse Not EstadosImportacionCarpetas.EsLote(lote.Estado) Then Return "ESTADO_NO_DISPONIBLE"
        If lote.Estado = "COMPLETADO" OrElse lote.Estado = "FALLIDO" Then Return "LOTE_TERMINAL"
        If inciertos OrElse lote.Estado = "PENDIENTE_VERIFICACION" Then Return "RESULTADO_INCIERTO"
        If activos Then Return "OPERACION_EN_CURSO"
        If lote.Estado = "DETENIDO" AndAlso lote.DetencionSolicitada Then Return "REANUDACION_DISPONIBLE"
        If lote.DetencionSolicitada OrElse lote.Estado = "DETENIDO" Then Return "DETENCION_INCONSISTENTE"
        Return Nothing
    End Function

    Public Function ReanudarLote(solicitud As ReanudarLoteImportacionSolicitud) As DetencionLoteImportacionResultado
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.loteId) OrElse solicitud.revisionEsperada = 0 OrElse solicitud.revisionEsperada >= UInteger.MaxValue - 1UI Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La referencia del lote no es valida.")
        Dim puerto = TryCast(_repositorio, IReanudacionLoteImportacionRepository)
        If puerto Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El control de reanudacion no esta compuesto.")
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, solicitud.loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContextoControl(contexto, referencia) OrElse Not contexto.PuedeImportar Then Throw Denegado()
        Dim lote = _repositorio.ObtenerLote(contexto, solicitud.loteId)
        If lote Is Nothing OrElse lote.LoteId <> solicitud.loteId OrElse Not MismoContextoControl(contexto, lote.Contexto) OrElse lote.Snapshot Is Nothing OrElse lote.Snapshot.Configuracion Is Nothing Then Throw Denegado()
        _autorizar(contexto, lote.Snapshot.Configuracion)
        Dim actual = puerto.ReanudarLote(contexto, lote.LoteId, solicitud.revisionEsperada)
        If actual Is Nothing OrElse actual.LoteId <> lote.LoteId OrElse Not MismoContextoControl(contexto, actual.Contexto) OrElse actual.DetencionSolicitada OrElse actual.Revision <> solicitud.revisionEsperada + 1UI OrElse actual.Estado = "DETENIDO" OrElse Not EstadosImportacionCarpetas.EsLote(actual.Estado) Then Throw NoPermitida()
        Return New DetencionLoteImportacionResultado With {.loteId = actual.LoteId, .revision = actual.Revision, .estado = actual.Estado, .detencionSolicitada = False}
    End Function
End Class
