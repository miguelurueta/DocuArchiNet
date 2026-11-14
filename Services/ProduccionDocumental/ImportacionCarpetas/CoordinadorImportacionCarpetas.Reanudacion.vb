Option Strict On
Option Compare Binary
Imports System
Imports System.Text.RegularExpressions

Public Partial Class CoordinadorImportacionCarpetas
    Private ReadOnly _vistaRetomar As Func(Of ContextoImportacionCarpetas, String, String, VistaReconciliacionImportacion)
    Private ReadOnly _reclamarRetomar As Func(Of VistaReconciliacionImportacion, String, IntentoImportacionCarpetas)
    Private ReadOnly _preflightRetomar As Action(Of ComandoCreacionExpedienteImportacion)

    Public Function RetomarCreacion(solicitud As AvanzarLoteImportacionSolicitud) As ResultadoFaseCreacionImportacion
        If _vistaRetomar Is Nothing OrElse _reclamarRetomar Is Nothing OrElse _preflightRetomar Is Nothing OrElse _creador Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La reanudación no está compuesta.")
        If solicitud Is Nothing OrElse solicitud.fase <> "RETOMAR_CREACION" OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.recursoId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.intentoId) OrElse solicitud.revisionEsperada = 0 OrElse solicitud.revisionLoteEsperada = 0 OrElse solicitud.revisionEsperada >= UInteger.MaxValue - 1UI OrElse solicitud.revisionLoteEsperada >= UInteger.MaxValue - 1UI OrElse Not Regex.IsMatch(If(solicitud.claveOperacion, ""), "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant) Then Throw EntradaOperacionInvalida()
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, solicitud.loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContextoControl(contexto, referencia) OrElse Not contexto.PuedeImportar Then Throw Denegado()
        Dim esperada = _vistaRetomar(contexto, solicitud.loteId, solicitud.intentoId)
        ValidarVistaReconciliacion(esperada, contexto, solicitud.loteId, solicitud.intentoId)
        If esperada.Raiz.RaizId <> solicitud.recursoId OrElse esperada.Raiz.Revision <> solicitud.revisionEsperada OrElse esperada.Lote.Revision <> solicitud.revisionLoteEsperada OrElse esperada.Lote.DetencionSolicitada <> solicitud.levantarDetencion Then Throw NoPermitida()
        ExigirReservaRetomable(esperada)
        ReautorizarFase(contexto, esperada.Lote.Snapshot.Configuracion)
        Dim comando = ComandoCreacionExpedienteImportacion.PrepararReanudacion(contexto, esperada.Lote, esperada.Raiz, esperada.Intento)
        comando.ValidarEntradasLegacy()
        _preflightRetomar(comando)
        Dim vista = _vistaDocumental(contexto, solicitud.loteId, solicitud.recursoId)
        ExigirVistaDocumental(vista, contexto, solicitud.loteId, solicitud.recursoId)
        If PerfilImportacionJson.Escribir(vista.Lote) <> PerfilImportacionJson.Escribir(esperada.Lote) OrElse PerfilImportacionJson.Escribir(vista.Raiz) <> PerfilImportacionJson.Escribir(esperada.Raiz) Then Throw NoPermitida()
        'Solo el preflight explícito tolera la detención observada. La copia
        'no concede reclamación: el repositorio vuelve a comprobarla bajo locks.
        Dim previa = PerfilImportacionJson.Leer(Of VistaBarreraCreacionImportacion)(PerfilImportacionJson.Escribir(vista))
        previa.Lote.DetencionSolicitada = False
        Dim pruebas = VerificarStagingCreacion(previa, _archivoDocumental, _stagingDocumental, _autorizar, Nothing)
        ReautorizarFase(contexto, esperada.Lote.Snapshot.Configuracion)
        Dim token = Guid.NewGuid().ToString("N")
        Dim intento = _reclamarRetomar(PerfilImportacionJson.Leer(Of VistaReconciliacionImportacion)(PerfilImportacionJson.Escribir(esperada)), token)
        If intento Is Nothing OrElse Not intento.EsNuevaReserva OrElse intento.IntentoId <> esperada.Intento.IntentoId OrElse intento.LoteId <> solicitud.loteId OrElse intento.RaizId <> solicitud.recursoId OrElse intento.ArchivoId IsNot Nothing OrElse intento.Fase <> "CREAR_EXPEDIENTE" OrElse intento.Estado <> "RESERVADO" OrElse intento.TokenReclamacion <> token OrElse intento.SolicitudHash <> esperada.Intento.SolicitudHash OrElse intento.InicioUtc <> esperada.Intento.InicioUtc OrElse intento.Resultado IsNot Nothing OrElse intento.FinUtc IsNot Nothing Then Throw NoPermitida()
        pruebas.Intento = intento
        Dim reclamacion As New ReclamacionImportacionCarpetas With {.LoteId = solicitud.loteId, .RecursoId = solicitud.recursoId, .RecursoTipo = "RAIZ", .Fase = "CREAR_EXPEDIENTE", .RevisionEsperada = solicitud.revisionEsperada, .TokenReclamacion = token, .SolicitudHash = intento.SolicitudHash, .EjecutorRef = intento.EjecutorRef}
        Return EjecutarCreacionReservada(contexto, vista, reclamacion, pruebas, solicitud.revisionEsperada)
    End Function

    Friend Shared Sub ExigirReservaRetomable(vista As VistaReconciliacionImportacion)
        If vista Is Nothing OrElse vista.Lote Is Nothing OrElse vista.Raiz Is Nothing OrElse vista.Intento Is Nothing OrElse vista.Archivo IsNot Nothing Then Throw NoPermitida()
        Dim l = vista.Lote, r = vista.Raiz, i = vista.Intento
        If l.Estado <> "EN_CURSO" OrElse l.Revision = 0 OrElse l.Revision >= UInteger.MaxValue - 4UI OrElse r.Revision = 0 OrElse r.Revision >= UInteger.MaxValue - 2UI OrElse r.LoteId <> l.LoteId OrElse r.Estado <> "CREANDO" OrElse r.IdentidadExpediente IsNot Nothing OrElse r.IntentoActivoId <> i.IntentoId OrElse r.ValoresEfectivos Is Nothing OrElse r.ValoresEfectivos.Count <> 0 Then Throw NoPermitida()
        If i.LoteId <> l.LoteId OrElse i.RaizId <> r.RaizId OrElse i.ArchivoId IsNot Nothing OrElse i.Fase <> "CREAR_EXPEDIENTE" OrElse i.Estado <> "RESERVADO" OrElse i.EjecutorRef <> "WEB_CREACION" OrElse i.EsNuevaReserva OrElse i.Resultado IsNot Nothing OrElse i.FinUtc IsNot Nothing OrElse Not Regex.IsMatch(If(i.TokenReclamacion, ""), "\A[0-9a-f]{32}\z") OrElse Not Regex.IsMatch(If(i.SolicitudHash, ""), "\A[0-9a-f]{64}\z") Then Throw NoPermitida()
    End Sub
End Class
