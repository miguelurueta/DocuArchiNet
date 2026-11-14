Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Public Partial Class CoordinadorImportacionCarpetas
    Private ReadOnly _vistaDocumental As Func(Of ContextoImportacionCarpetas, String, String, VistaBarreraCreacionImportacion)
    Private ReadOnly _archivoDocumental As Func(Of ContextoImportacionCarpetas, String, String, ArchivoImportacionCarpetas)
    Private ReadOnly _historicoDocumental As Func(Of ContextoImportacionCarpetas, ReclamacionImportacionCarpetas, IntentoImportacionCarpetas)
    Private ReadOnly _stagingDocumental As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion)
    Private ReadOnly _creador As Func(Of Func(Of IDisposable), ICreacionExpedienteImportacion)

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), repositorio As IControlLoteImportacionRepository, destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), vista As Func(Of ContextoImportacionCarpetas, String, String, VistaBarreraCreacionImportacion), archivo As Func(Of ContextoImportacionCarpetas, String, String, ArchivoImportacionCarpetas), historico As Func(Of ContextoImportacionCarpetas, ReclamacionImportacionCarpetas, IntentoImportacionCarpetas), staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion), creador As Func(Of Func(Of IDisposable), ICreacionExpedienteImportacion), Optional vistaRetomar As Func(Of ContextoImportacionCarpetas, String, String, VistaReconciliacionImportacion) = Nothing, Optional reclamarRetomar As Func(Of VistaReconciliacionImportacion, String, IntentoImportacionCarpetas) = Nothing, Optional preflightRetomar As Action(Of ComandoCreacionExpedienteImportacion) = Nothing)
        Me.New(identidad, repositorio, destino, autorizar)
        If vista Is Nothing OrElse archivo Is Nothing OrElse historico Is Nothing OrElse staging Is Nothing OrElse creador Is Nothing Then Throw New ArgumentNullException()
        _vistaDocumental = vista : _archivoDocumental = archivo : _historicoDocumental = historico : _stagingDocumental = staging : _creador = creador
        _vistaRetomar = vistaRetomar : _reclamarRetomar = reclamarRetomar : _preflightRetomar = preflightRetomar
    End Sub

    'Una raiz/una fase; no hay bucle de efectos, background o endpoint nuevo.
    Public Function CrearExpediente(loteId As String, raizId As String, revisionEsperada As UInteger, claveOperacion As String) As ResultadoFaseCreacionImportacion
        If _creador Is Nothing OrElse _vistaDocumental Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La fase de creación no está compuesta.")
        If Not ValidadorPerfilImportacion.ReferenciaValida(loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(raizId) OrElse revisionEsperada = 0 OrElse revisionEsperada >= UInteger.MaxValue - 1UI OrElse claveOperacion Is Nothing OrElse Not Regex.IsMatch(claveOperacion, "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant) Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La referencia de creación no es válida.")
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContextoControl(contexto, referencia) OrElse Not contexto.PuedeImportar Then Throw Denegado()
        Dim vista = _vistaDocumental(contexto, loteId, raizId)
        ExigirVistaDocumental(vista, contexto, loteId, raizId)
        _autorizar(contexto, vista.Lote.Snapshot.Configuracion)
        Dim solicitud As New ReclamacionImportacionCarpetas With {.LoteId = loteId, .RecursoId = raizId, .RecursoTipo = "RAIZ", .Fase = "CREAR_EXPEDIENTE", .RevisionEsperada = revisionEsperada, .TokenReclamacion = Guid.NewGuid().ToString("N"), .EjecutorRef = "WEB_CREACION"}
        Using sha = SHA256.Create()
            solicitud.SolicitudHash = BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(New With {.contexto = contexto, .lote = loteId, .raiz = raizId, .revision = revisionEsperada, .clave = claveOperacion, .fase = solicitud.Fase})))).Replace("-", "").ToLowerInvariant()
        End Using
        Dim previo = _historicoDocumental(contexto, solicitud)
        If previo IsNot Nothing Then Return ProyectarCreacion(vista, solicitud, previo, Nothing, True)
        If vista.Raiz.Revision <> revisionEsperada Then Throw NoPermitida()
        Dim reserva = ReservarCreacionConStaging(vista, solicitud, _archivoDocumental, _stagingDocumental, _autorizar, AddressOf _repositorio.ReservarIntento)
        If Not reserva.Intento.EsNuevaReserva Then Return ProyectarCreacion(vista, solicitud, reserva.Intento, Nothing, True)
        Return EjecutarCreacionReservada(contexto, vista, solicitud, reserva, revisionEsperada)
    End Function

    Private Function EjecutarCreacionReservada(contexto As ContextoImportacionCarpetas, vista As VistaBarreraCreacionImportacion, solicitud As ReclamacionImportacionCarpetas, reserva As ReservaCreacionVerificadaImportacion, revisionEsperada As UInteger) As ResultadoFaseCreacionImportacion
        Dim loteId = solicitud.LoteId, raizId = solicitud.RecursoId
        Dim resultado As ResultadoEfectoImportacion = Nothing
        Dim retenciones As RetencionesStagingCreacionImportacion = Nothing
        Try
            Dim actual = _vistaDocumental(contexto, loteId, raizId)
            ExigirVistaDocumental(actual, contexto, loteId, raizId)
            ExigirReservaCreacion(actual, reserva.Intento, revisionEsperada)
            ReautorizarFase(contexto, actual.Lote.Snapshot.Configuracion)
            Dim conservar As Func(Of IDisposable) = Function()
                                                       If retenciones IsNot Nothing Then Throw StagingCreacionOcupado()
                                                       Dim fresca = _vistaDocumental(contexto, loteId, raizId)
                                                       ExigirVistaDocumental(fresca, contexto, loteId, raizId)
                                                       ExigirReservaCreacion(fresca, reserva.Intento, revisionEsperada)
                                                       ReautorizarFase(contexto, fresca.Lote.Snapshot.Configuracion)
                                                       retenciones = ConservarStagingCreacion(fresca, _archivoDocumental, _stagingDocumental, _autorizar)
                                                       Return retenciones
                                                   End Function
            Dim adaptador = _creador(conservar)
            If adaptador Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo componer el adaptador de creación.")
            resultado = adaptador.Crear(contexto, actual.Lote, actual.Raiz, reserva.Intento)
        Catch ex As Exception
            'La reserva puede estar activa o el efecto confirmado sin respuesta.
            'No repetir ni transmitir mensajes privados; consultar el checkpoint.
        Finally
            If retenciones IsNot Nothing Then
                Try
                    retenciones.Dispose()
                Catch ex As Exception
                    'El contenedor intenta cerrar todos los handles. Consultar
                    'el checkpoint sin propagar detalles privados del cierre.
                End Try
            End If
        End Try
        Dim salida As ResultadoFaseCreacionImportacion
        Try
            Dim ultima = _vistaDocumental(contexto, loteId, raizId)
            ExigirVistaDocumental(ultima, contexto, loteId, raizId)
            Dim guardado = _historicoDocumental(contexto, solicitud)
            salida = ProyectarCreacion(ultima, solicitud, If(guardado, reserva.Intento), resultado, False)
        Catch ex As Exception
            salida = ProyectarCreacion(vista, solicitud, reserva.Intento, resultado, False)
        End Try
        Dim pruebas = If(retenciones Is Nothing, reserva, retenciones.Resultado)
        salida.ArchivosVerificados = New List(Of String)(pruebas.ArchivosVerificados)
        salida.ArchivosInciertos = New List(Of String)(pruebas.ArchivosInciertos)
        Return salida
    End Function

    Private Sub ReautorizarFase(esperado As ContextoImportacionCarpetas, configuracion As ConfiguracionPerfilImportacion)
        Dim identidad = _identidad(), actual = _destino(esperado.NivelRef)
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse identidad.UsuarioRef <> esperado.UsuarioRef OrElse identidad.AmbitoRef <> esperado.AmbitoRef OrElse Not MismoContextoControl(actual, esperado) OrElse Not actual.PuedeImportar Then Throw Denegado()
        _autorizar(actual, configuracion)
    End Sub

    Private Shared Sub ExigirVistaDocumental(vista As VistaBarreraCreacionImportacion, contexto As ContextoImportacionCarpetas, loteId As String, raizId As String)
        If vista Is Nothing OrElse vista.Lote Is Nothing OrElse vista.Plan Is Nothing OrElse vista.Raiz Is Nothing OrElse vista.Lote.LoteId <> loteId OrElse vista.Raiz.RaizId <> raizId OrElse vista.Raiz.LoteId <> loteId OrElse vista.Raiz.Revision = 0 OrElse vista.Lote.PlanId <> vista.Plan.PlanId OrElse vista.Plan.Estado <> "CONFIRMADO" OrElse Not MismoContextoControl(contexto, vista.Plan.Contexto) OrElse Not MismoContextoControl(contexto, vista.Lote.Contexto) OrElse vista.Lote.Snapshot Is Nothing OrElse vista.Lote.Snapshot.Configuracion Is Nothing OrElse PerfilImportacionJson.Escribir(vista.Lote.Snapshot) <> PerfilImportacionJson.Escribir(vista.Plan.Snapshot) Then Throw Denegado()
    End Sub

    Private Shared Sub ExigirReservaCreacion(vista As VistaBarreraCreacionImportacion, intento As IntentoImportacionCarpetas, revisionOriginal As UInteger)
        If vista.Lote.DetencionSolicitada OrElse vista.Lote.Estado <> "EN_CURSO" OrElse vista.Raiz.Estado <> "CREANDO" OrElse vista.Raiz.IntentoActivoId <> intento.IntentoId OrElse vista.Raiz.Revision <> revisionOriginal + 1UI OrElse vista.Raiz.IdentidadExpediente IsNot Nothing Then Throw StagingCreacionOcupado()
    End Sub

    Private Shared Function ProyectarCreacion(vista As VistaBarreraCreacionImportacion, solicitud As ReclamacionImportacionCarpetas, intento As IntentoImportacionCarpetas, observado As ResultadoEfectoImportacion, repeticion As Boolean) As ResultadoFaseCreacionImportacion
        If intento Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.IntentoId) OrElse intento.LoteId <> solicitud.LoteId OrElse intento.RaizId <> solicitud.RecursoId OrElse intento.Fase <> "CREAR_EXPEDIENTE" OrElse intento.ArchivoId IsNot Nothing OrElse intento.SolicitudHash <> solicitud.SolicitudHash Then Throw NoPermitida()
        Dim salida As New ResultadoFaseCreacionImportacion With {.LoteId = solicitud.LoteId, .RaizId = solicitud.RecursoId, .IntentoId = intento.IntentoId, .Repeticion = repeticion, .Resultado = New ResultadoEfectoImportacion With {.Estado = "INCIERTO", .Codigo = "RESULTADO_INCIERTO", .Mensaje = "Consulte el intento antes de repetir cualquier efecto."}}
        Dim prueba = If(intento.Resultado IsNot Nothing AndAlso {"CONFIRMADO", "INCIERTO"}.Contains(intento.Estado), intento.Resultado, If(observado, intento.Resultado))
        If prueba IsNot Nothing AndAlso prueba.Creacion IsNot Nothing AndAlso prueba.Creacion.IntentoId = intento.IntentoId AndAlso prueba.Creacion.CicloArchivoNombre = "Archivo Gestión" AndAlso ValidadorPerfilImportacion.ReferenciaValida(prueba.Creacion.CicloArchivoId) Then
            salida.Resultado.Creacion = PerfilImportacionJson.Leer(Of EvidenciaCreacionExpedienteImportacion)(PerfilImportacionJson.Escribir(prueba.Creacion))
            Dim id = prueba.Expediente
            If id IsNot Nothing AndAlso id.OrigenLogico = "RA" AndAlso id.NivelRef = vista.Lote.Contexto.NivelRef AndAlso (id.ExpedienteId Is Nothing OrElse ValidadorPerfilImportacion.ReferenciaValida(id.ExpedienteId)) AndAlso (id.RelacionNivelId Is Nothing OrElse ValidadorPerfilImportacion.ReferenciaValida(id.RelacionNivelId)) Then
                salida.Resultado.Expediente = PerfilImportacionJson.Leer(Of IdentidadExpedienteImportacion)(PerfilImportacionJson.Escribir(id))
                If intento.Estado = "CONFIRMADO" AndAlso intento.FinUtc IsNot Nothing AndAlso intento.Resultado IsNot Nothing AndAlso intento.Resultado.Estado = "CONFIRMADO" AndAlso id.ExpedienteId IsNot Nothing AndAlso id.RelacionNivelId IsNot Nothing AndAlso vista.Raiz.IdentidadExpediente IsNot Nothing AndAlso PerfilImportacionJson.Escribir(vista.Raiz.IdentidadExpediente) = PerfilImportacionJson.Escribir(id) Then
                    salida.Resultado.Estado = "CONFIRMADO" : salida.Resultado.Codigo = "EXPEDIENTE_CONFIRMADO" : salida.Resultado.Mensaje = "Expediente y relación confirmados; no implica completitud documental del lote."
                End If
            End If
        End If
        Return salida
    End Function
End Class
