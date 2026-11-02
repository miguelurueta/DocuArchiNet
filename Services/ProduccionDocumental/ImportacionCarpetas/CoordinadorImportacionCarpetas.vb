Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Linq

'Legalidad de estados; cada fase debe comprobar ademas autoridad, evidencia,
'barrera y CAS. Incluye señal de detencion autorizada, sin invocar adaptadores.
Public Partial Class CoordinadorImportacionCarpetas
    Private Shared ReadOnly Transiciones As Dictionary(Of String, Dictionary(Of String, HashSet(Of String))) = CrearTransiciones()
    Private ReadOnly _identidad As Func(Of ContextoImportacionCarpetas)
    Private ReadOnly _repositorio As IControlLoteImportacionRepository
    Private ReadOnly _destino As Func(Of String, ContextoImportacionCarpetas)
    Private ReadOnly _autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion)

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), repositorio As IControlLoteImportacionRepository, destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion))
        If identidad Is Nothing OrElse repositorio Is Nothing OrElse destino Is Nothing OrElse autorizar Is Nothing Then Throw New ArgumentNullException()
        _identidad = identidad : _repositorio = repositorio : _destino = destino : _autorizar = autorizar
    End Sub

    'No cancela la peticion en vuelo ni ejecuta otra fase.
    Public Function SolicitarDetencion(solicitud As DetenerLoteImportacionSolicitud) As DetencionLoteImportacionResultado
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.loteId) OrElse solicitud.revisionEsperada = 0 OrElse solicitud.revisionEsperada = UInteger.MaxValue Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La referencia del lote no es válida.")
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, solicitud.loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef OrElse Not ValidadorPerfilImportacion.ReferenciaValida(referencia.NivelRef) Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContextoControl(contexto, referencia) OrElse Not contexto.PuedeImportar Then Throw Denegado()
        Dim lote = _repositorio.ObtenerLote(contexto, solicitud.loteId)
        If lote Is Nothing OrElse lote.LoteId <> solicitud.loteId OrElse Not MismoContextoControl(contexto, lote.Contexto) OrElse lote.Snapshot Is Nothing OrElse lote.Snapshot.Configuracion Is Nothing Then Throw Denegado()
        _autorizar(contexto, lote.Snapshot.Configuracion)
        Dim detenido = _repositorio.SolicitarDetencion(contexto, lote.LoteId, solicitud.revisionEsperada)
        If detenido Is Nothing OrElse detenido.LoteId <> lote.LoteId OrElse Not MismoContextoControl(contexto, detenido.Contexto) OrElse Not EstadosImportacionCarpetas.EsLote(detenido.Estado) OrElse detenido.Revision = 0 OrElse (Not detenido.DetencionSolicitada AndAlso Not {"COMPLETADO", "FALLIDO"}.Contains(detenido.Estado)) Then Throw NoPermitida()
        Return New DetencionLoteImportacionResultado With {.loteId = detenido.LoteId, .estado = detenido.Estado, .revision = detenido.Revision, .detencionSolicitada = detenido.DetencionSolicitada}
    End Function

    Private Shared Function MismoContextoControl(a As ContextoImportacionCarpetas, b As ContextoImportacionCarpetas) As Boolean
        Return a IsNot Nothing AndAlso b IsNot Nothing AndAlso a.UsuarioRef = b.UsuarioRef AndAlso a.AmbitoRef = b.AmbitoRef AndAlso a.NivelRef = b.NivelRef AndAlso a.RutaCanonica = b.RutaCanonica AndAlso a.SelloDestino = b.SelloDestino AndAlso b.PuedeImportar
    End Function

    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "El lote no está disponible para este contexto.")
    End Function

    Public Shared Function EstadosDimension(dimension As String) As List(Of String)
        If dimension Is Nothing OrElse Not Transiciones.ContainsKey(dimension) Then Throw NoPermitida()
        Return Transiciones(dimension).Keys.OrderBy(Function(e) e, StringComparer.Ordinal).ToList()
    End Function

    Public Shared Function SiguientesPermitidos(dimension As String, estado As String) As List(Of String)
        If dimension Is Nothing OrElse estado Is Nothing OrElse Not Transiciones.ContainsKey(dimension) OrElse Not Transiciones(dimension).ContainsKey(estado) Then Throw NoPermitida()
        Return Transiciones(dimension)(estado).OrderBy(Function(e) e, StringComparer.Ordinal).ToList()
    End Function

    Public Shared Function EsTransicionPermitida(dimension As String, anterior As String, siguiente As String) As Boolean
        Return dimension IsNot Nothing AndAlso anterior IsNot Nothing AndAlso siguiente IsNot Nothing AndAlso Transiciones.ContainsKey(dimension) AndAlso Transiciones(dimension).ContainsKey(anterior) AndAlso Transiciones(dimension)(anterior).Contains(siguiente)
    End Function

    Public Shared Sub ValidarTransicion(dimension As String, anterior As String, siguiente As String)
        If Not EsTransicionPermitida(dimension, anterior, siguiente) Then Throw NoPermitida()
    End Sub

    Private Shared Function CrearTransiciones() As Dictionary(Of String, Dictionary(Of String, HashSet(Of String)))
        Return New Dictionary(Of String, Dictionary(Of String, HashSet(Of String)))(StringComparer.Ordinal) From {
            {"PLAN", Tabla("REVISABLE>CONFIRMADO,EXPIRADO", "BLOQUEADO>EXPIRADO", "CONFIRMADO>", "EXPIRADO>")},
            {"RECEPCION", Tabla(
                "PENDIENTE>RECIBIENDO,RECHAZADO,CANCELADO", "RECIBIENDO>RECIBIDO,RECHAZADO,INCIERTO",
                "RECIBIDO>INCIERTO", "RECHAZADO>RECIBIENDO,CANCELADO", "INCIERTO>RECIBIDO,RECHAZADO", "CANCELADO>")},
            {"EFECTO", Tabla(
                "NO_INICIADO>EN_CURSO,RECHAZADO,CANCELADO", "EN_CURSO>ALMACENADO,COMPLETADO,RECHAZADO,INCIERTO",
                "ALMACENADO>EN_CURSO,COMPLETADO,INCIERTO", "COMPLETADO>", "RECHAZADO>EN_CURSO", "INCIERTO>ALMACENADO,COMPLETADO,RECHAZADO", "CANCELADO>")},
            {"RAIZ", Tabla(
                "PENDIENTE>CREANDO,BLOQUEADO,CANCELADA", "CREANDO>EXPEDIENTE_CONFIRMADO,INCIERTO,BLOQUEADO",
                "EXPEDIENTE_CONFIRMADO>COMPLETADA,PARCIAL,INCIERTO", "INCIERTO>EXPEDIENTE_CONFIRMADO,BLOQUEADO",
                "BLOQUEADO>CREANDO,CANCELADA", "COMPLETADA>", "PARCIAL>EXPEDIENTE_CONFIRMADO,COMPLETADA,INCIERTO", "CANCELADA>")},
            {"INTENTO", Tabla(
                "RESERVADO>EN_CURSO,RECHAZADO,INCIERTO", "EN_CURSO>CONFIRMADO,RECHAZADO,INCIERTO",
                "CONFIRMADO>", "RECHAZADO>", "INCIERTO>CONFIRMADO,RECHAZADO")},
            {"LOTE", Tabla(
                "RECIBIENDO>LISTO,EN_CURSO,PENDIENTE_VERIFICACION,DETENIDO,PARCIAL,FALLIDO",
                "LISTO>RECIBIENDO,EN_CURSO,PENDIENTE_VERIFICACION,DETENIDO,COMPLETADO,PARCIAL,FALLIDO",
                "EN_CURSO>RECIBIENDO,LISTO,PENDIENTE_VERIFICACION,DETENIDO,COMPLETADO,PARCIAL,FALLIDO",
                "PENDIENTE_VERIFICACION>RECIBIENDO,LISTO,DETENIDO,COMPLETADO,PARCIAL,FALLIDO",
                "DETENIDO>RECIBIENDO,LISTO,PENDIENTE_VERIFICACION,COMPLETADO,PARCIAL,FALLIDO",
                "COMPLETADO>", "PARCIAL>RECIBIENDO,LISTO,EN_CURSO,PENDIENTE_VERIFICACION,DETENIDO,COMPLETADO", "FALLIDO>")}}
    End Function

    Private Shared Function Tabla(ParamArray filas As String()) As Dictionary(Of String, HashSet(Of String))
        Dim salida As New Dictionary(Of String, HashSet(Of String))(StringComparer.Ordinal)
        For Each fila In filas
            Dim partes = fila.Split(">"c)
            salida.Add(partes(0), New HashSet(Of String)(partes(1).Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries), StringComparer.Ordinal))
        Next
        Return salida
    End Function

    Private Shared Function NoPermitida() As PerfilImportacionException
        Return New PerfilImportacionException("CONFLICTO_REVISION", "La transición no está permitida para esta dimensión y estado.")
    End Function
End Class
