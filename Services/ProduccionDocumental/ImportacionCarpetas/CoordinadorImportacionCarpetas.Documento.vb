Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Public Partial Class CoordinadorImportacionCarpetas
    Private ReadOnly _almacenador As Func(Of IAlmacenStagingImportacion, IAlmacenamientoDocumentoImportacion)

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), repositorio As IControlLoteImportacionRepository, destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), vista As Func(Of ContextoImportacionCarpetas, String, String, VistaBarreraCreacionImportacion), archivo As Func(Of ContextoImportacionCarpetas, String, String, ArchivoImportacionCarpetas), historico As Func(Of ContextoImportacionCarpetas, ReclamacionImportacionCarpetas, IntentoImportacionCarpetas), staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion), almacenador As Func(Of IAlmacenStagingImportacion, IAlmacenamientoDocumentoImportacion))
        Me.New(identidad, repositorio, destino, autorizar)
        If vista Is Nothing OrElse archivo Is Nothing OrElse historico Is Nothing OrElse staging Is Nothing OrElse almacenador Is Nothing Then Throw New ArgumentNullException()
        _vistaDocumental = vista : _archivoDocumental = archivo : _historicoDocumental = historico : _stagingDocumental = staging : _almacenador = almacenador
    End Sub

    'No crea expedientes ni materializa otra fase dentro de esta solicitud.
    Public Function AlmacenarArchivo(loteId As String, archivoId As String, revisionEsperada As UInteger, claveOperacion As String) As ResultadoFaseDocumentoImportacion
        If _almacenador Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La fase documental no esta compuesta.")
        If Not ValidadorPerfilImportacion.ReferenciaValida(loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(archivoId) OrElse revisionEsperada = 0 OrElse revisionEsperada >= UInteger.MaxValue - 1UI OrElse claveOperacion Is Nothing OrElse Not Regex.IsMatch(claveOperacion, "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant) Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La referencia documental no es valida.")
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContextoControl(contexto, referencia) OrElse Not contexto.PuedeImportar Then Throw Denegado()
        Dim archivo = _archivoDocumental(contexto, loteId, archivoId)
        If archivo Is Nothing OrElse archivo.ArchivoId <> archivoId Then Throw Denegado()
        Dim vista = _vistaDocumental(contexto, loteId, archivo.RaizId)
        ExigirVistaDocumental(vista, contexto, loteId, archivo.RaizId)
        ValidarVistaValores(contexto, New VistaValoresImportacion With {.Lote = vista.Lote, .Raiz = vista.Raiz, .Archivo = archivo})
        ReautorizarFase(contexto, vista.Lote.Snapshot.Configuracion)
        Dim solicitud As New ReclamacionImportacionCarpetas With {.LoteId = loteId, .RecursoId = archivoId, .RecursoTipo = "ARCHIVO", .Fase = "ALMACENAR", .RevisionEsperada = revisionEsperada, .TokenReclamacion = Guid.NewGuid().ToString("N"), .EjecutorRef = "WEB_DOCUMENTO"}
        Using sha = SHA256.Create()
            solicitud.SolicitudHash = BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(New With {.contexto = contexto, .lote = loteId, .archivo = archivoId, .revision = revisionEsperada, .clave = claveOperacion, .fase = solicitud.Fase})))).Replace("-", "").ToLowerInvariant()
        End Using
        Dim previo = _historicoDocumental(contexto, solicitud)
        If previo IsNot Nothing Then Return ProyectarDocumento(vista, archivo, solicitud, previo, Nothing, True)
        If archivo.Revision <> revisionEsperada OrElse Not EvaluarBarreraAlmacenamiento(vista, archivo) Then Throw NoPermitida()
        'Comprobar los originales exigidos antes de reclamar un efecto.
        VerificarStagingCreacion(vista, _archivoDocumental, _stagingDocumental, _autorizar, Nothing, archivo)
        ReautorizarFase(contexto, vista.Lote.Snapshot.Configuracion)
        Dim reserva = _repositorio.ReservarIntento(contexto, solicitud)
        ExigirIntentoDocumento(solicitud, vista.Raiz.RaizId, reserva)
        If Not reserva.EsNuevaReserva Then Return ProyectarDocumento(vista, archivo, solicitud, reserva, Nothing, True)
        If reserva.Estado <> "RESERVADO" OrElse reserva.TokenReclamacion <> solicitud.TokenReclamacion OrElse reserva.FinUtc IsNot Nothing OrElse reserva.Resultado IsNot Nothing Then Throw NoPermitida()
        Dim observado As ResultadoEfectoImportacion = Nothing
        Dim retenciones As RetencionesStagingCreacionImportacion = Nothing
        Try
            Dim actual = _vistaDocumental(contexto, loteId, archivo.RaizId)
            Dim fresco = _archivoDocumental(contexto, loteId, archivoId)
            ExigirVistaDocumental(actual, contexto, loteId, archivo.RaizId)
            ExigirReservaDocumento(actual, fresco, reserva, revisionEsperada)
            ReautorizarFase(contexto, actual.Lote.Snapshot.Configuracion)
            Dim guardias As New List(Of IRetencionVerificadaImportacion)()
            Try
                Dim pruebas = VerificarStagingCreacion(actual, _archivoDocumental, _stagingDocumental, _autorizar, guardias, fresco, reserva.IntentoId)
                retenciones = New RetencionesStagingCreacionImportacion(guardias, pruebas)
            Catch
                Dim cierre As New RetencionesStagingCreacionImportacion(guardias, Nothing)
                cierre.Dispose()
                Throw
            End Try
            ReautorizarFase(contexto, actual.Lote.Snapshot.Configuracion)
            Dim almacen = _stagingDocumental(actual.Lote, fresco)
            If almacen Is Nothing Then Throw NoPermitida()
            Dim adaptador = _almacenador(almacen)
            If adaptador Is Nothing Then Throw NoPermitida()
            observado = adaptador.Almacenar(contexto, actual.Raiz, fresco, actual.Lote.Snapshot, reserva)
        Catch ex As Exception
            'La reserva o el efecto pueden haber quedado persistidos. Consultar
            'el checkpoint; no repetir el escritor para reparar una respuesta.
        Finally
            If retenciones IsNot Nothing Then
                Try
                    retenciones.Dispose()
                Catch ex As Exception
                    'No exponer paths ni sustituir evidencia por un fallo de cierre.
                End Try
            End If
        End Try
        Dim salida As ResultadoFaseDocumentoImportacion
        Try
            Dim ultima = _vistaDocumental(contexto, loteId, archivo.RaizId)
            ExigirVistaDocumental(ultima, contexto, loteId, archivo.RaizId)
            Dim ultimoArchivo = _archivoDocumental(contexto, loteId, archivoId)
            Dim guardado = _historicoDocumental(contexto, solicitud)
            salida = ProyectarDocumento(ultima, ultimoArchivo, solicitud, If(guardado, reserva), observado, False)
        Catch ex As Exception
            salida = ProyectarDocumento(vista, archivo, solicitud, reserva, observado, False)
        End Try
        If retenciones IsNot Nothing Then
            salida.ArchivosVerificados = New List(Of String)(retenciones.Resultado.ArchivosVerificados)
            salida.ArchivosInciertos = New List(Of String)(retenciones.Resultado.ArchivosInciertos)
        End If
        Return salida
    End Function

    Private Shared Sub ExigirIntentoDocumento(solicitud As ReclamacionImportacionCarpetas, raizId As String, intento As IntentoImportacionCarpetas)
        If intento Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.IntentoId) OrElse intento.LoteId <> solicitud.LoteId OrElse intento.RaizId <> raizId OrElse intento.ArchivoId <> solicitud.RecursoId OrElse intento.Fase <> "ALMACENAR" OrElse intento.SolicitudHash <> solicitud.SolicitudHash OrElse Not EstadosImportacionCarpetas.EsIntento(intento.Estado) Then Throw NoPermitida()
    End Sub
    Private Shared Sub ExigirReservaDocumento(vista As VistaBarreraCreacionImportacion, archivo As ArchivoImportacionCarpetas, intento As IntentoImportacionCarpetas, revision As UInteger)
        If vista.Lote.DetencionSolicitada OrElse vista.Lote.Estado <> "EN_CURSO" OrElse archivo Is Nothing OrElse archivo.ArchivoId <> intento.ArchivoId OrElse archivo.RaizId <> intento.RaizId OrElse archivo.Revision <> revision + 1UI OrElse archivo.IntentoActivoId <> intento.IntentoId OrElse archivo.EstadoEfecto <> "EN_CURSO" OrElse Not EvaluarBarreraAlmacenamiento(vista, archivo, intento.IntentoId) Then Throw NoPermitida()
    End Sub

    Private Shared Function ProyectarDocumento(vista As VistaBarreraCreacionImportacion, archivo As ArchivoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas, intento As IntentoImportacionCarpetas, observado As ResultadoEfectoImportacion, repeticion As Boolean) As ResultadoFaseDocumentoImportacion
        ExigirIntentoDocumento(solicitud, vista.Raiz.RaizId, intento)
        If archivo Is Nothing OrElse archivo.ArchivoId <> solicitud.RecursoId OrElse archivo.RaizId <> vista.Raiz.RaizId Then Throw NoPermitida()
        Dim salida As New ResultadoFaseDocumentoImportacion With {.LoteId = solicitud.LoteId, .RaizId = intento.RaizId, .ArchivoId = solicitud.RecursoId, .IntentoId = intento.IntentoId, .Repeticion = repeticion, .Resultado = New ResultadoEfectoImportacion With {.Estado = "INCIERTO", .Codigo = "RESULTADO_INCIERTO", .Mensaje = "Consulte el intento antes de repetir cualquier efecto."}}
        Dim prueba = If(intento.Resultado IsNot Nothing AndAlso {"CONFIRMADO", "INCIERTO"}.Contains(intento.Estado), intento.Resultado, If(observado, intento.Resultado))
        If prueba Is Nothing Then Return salida
        If vista.Raiz.IdentidadExpediente Is Nothing OrElse PerfilImportacionJson.Escribir(prueba.Expediente) <> PerfilImportacionJson.Escribir(vista.Raiz.IdentidadExpediente) OrElse prueba.OrigenGabinete <> vista.Lote.Snapshot.Configuracion.OrigenGabinete OrElse prueba.GabineteId <> vista.Lote.Snapshot.Configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture) OrElse PerfilImportacionJson.Escribir(prueba.ValoresEfectivos) <> PerfilImportacionJson.Escribir(archivo.ValoresEfectivos) Then Return salida
        salida.Resultado.Expediente = PerfilImportacionJson.Leer(Of IdentidadExpedienteImportacion)(PerfilImportacionJson.Escribir(prueba.Expediente))
        salida.Resultado.OrigenGabinete = prueba.OrigenGabinete : salida.Resultado.GabineteId = prueba.GabineteId
        salida.Resultado.ValoresEfectivos = PerfilImportacionJson.Leer(Of List(Of ValorReglaPerfilImportacion))(PerfilImportacionJson.Escribir(prueba.ValoresEfectivos))
        Dim completos As Boolean = True
        For Each campo In New String() {"DocumentoId", "InventarioId"}
            Dim valor = If(campo = "DocumentoId", prueba.DocumentoId, prueba.InventarioId), numero As Long
            If valor Is Nothing Then
                completos = False
            ElseIf Long.TryParse(valor, NumberStyles.None, CultureInfo.InvariantCulture, numero) AndAlso numero > 0 AndAlso numero.ToString(CultureInfo.InvariantCulture) = valor Then
                If campo = "DocumentoId" Then salida.Resultado.DocumentoId = valor Else salida.Resultado.InventarioId = valor
            Else
                completos = False
            End If
        Next
        If completos AndAlso intento.Estado = "CONFIRMADO" AndAlso intento.FinUtc IsNot Nothing AndAlso intento.Resultado IsNot Nothing AndAlso intento.Resultado.Estado = "CONFIRMADO" AndAlso archivo.EstadoEfecto = "COMPLETADO" AndAlso archivo.IntentoActivoId Is Nothing AndAlso archivo.ValoresEfectivosFijados AndAlso PerfilImportacionJson.Escribir(archivo.Resultado) = PerfilImportacionJson.Escribir(intento.Resultado) Then
            salida.Resultado.Estado = "CONFIRMADO" : salida.Resultado.Codigo = "DOCUMENTO_CONFIRMADO"
            salida.Resultado.Mensaje = "Documento e incorporacion confirmados; no implica completitud de raiz o lote."
        End If
        Return salida
    End Function
End Class
