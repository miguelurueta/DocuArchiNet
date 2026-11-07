Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.IO
Imports System.Text.RegularExpressions

'Confirmacion del plan propio. La recepcion de bytes se incorpora mediante
'staging/handler en sus tareas; esta clase no invoca efectos documentales.
Public Class ServicioRecepcionImportacion
    Private ReadOnly _identidad As Func(Of ContextoImportacionCarpetas)
    Private ReadOnly _referencia As Func(Of ContextoImportacionCarpetas, String, ContextoImportacionCarpetas)
    Private ReadOnly _destino As Func(Of String, ContextoImportacionCarpetas)
    Private ReadOnly _plan As Func(Of ContextoImportacionCarpetas, String, PlanImportacionCarpetas)
    Private ReadOnly _autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion)
    Private ReadOnly _perfiles As AdaptadorPerfilImportacionOperativa
    Private ReadOnly _coincidencias As Func(Of ContextoImportacionCarpetas, SnapshotPerfilImportacionOperativa, ManifiestoImportacionCarpetas, ResultadoPreparacionImportacion, List(Of IncidenciaImportacionCarpetas))
    Private ReadOnly _confirmar As Func(Of ContextoImportacionCarpetas, String, UInteger, String, String, LoteImportacionCarpetas)
    Private ReadOnly _limites As LimitesImportacionCarpetas
    Private ReadOnly _ahora As Func(Of DateTime)
    Private ReadOnly _recepciones As IRecepcionArchivosImportacionRepository
    Private ReadOnly _staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion)

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), referencia As Func(Of ContextoImportacionCarpetas, String, ContextoImportacionCarpetas), destino As Func(Of String, ContextoImportacionCarpetas), obtenerPlan As Func(Of ContextoImportacionCarpetas, String, PlanImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), perfiles As AdaptadorPerfilImportacionOperativa, coincidencias As Func(Of ContextoImportacionCarpetas, SnapshotPerfilImportacionOperativa, ManifiestoImportacionCarpetas, ResultadoPreparacionImportacion, List(Of IncidenciaImportacionCarpetas)), confirmar As Func(Of ContextoImportacionCarpetas, String, UInteger, String, String, LoteImportacionCarpetas), limites As LimitesImportacionCarpetas, Optional relojUtc As Func(Of DateTime) = Nothing)
        If identidad Is Nothing OrElse referencia Is Nothing OrElse destino Is Nothing OrElse obtenerPlan Is Nothing OrElse autorizar Is Nothing OrElse perfiles Is Nothing OrElse coincidencias Is Nothing OrElse confirmar Is Nothing Then Throw New ArgumentNullException()
        LimitesImportacionCarpetas.Exigir(limites)
        _identidad = identidad : _referencia = referencia : _destino = destino : _plan = obtenerPlan : _autorizar = autorizar
        _perfiles = perfiles : _coincidencias = coincidencias : _confirmar = confirmar : _limites = limites : _ahora = If(relojUtc, Function() DateTime.UtcNow)
    End Sub

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), repositorio As IRecepcionArchivosImportacionRepository, destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), limites As LimitesImportacionCarpetas, staging As Func(Of LoteImportacionCarpetas, ArchivoImportacionCarpetas, IAlmacenStagingImportacion))
        If identidad Is Nothing OrElse repositorio Is Nothing OrElse destino Is Nothing OrElse autorizar Is Nothing OrElse staging Is Nothing Then Throw New ArgumentNullException()
        LimitesImportacionCarpetas.Exigir(limites)
        _identidad = identidad : _recepciones = repositorio : _destino = destino : _autorizar = autorizar : _limites = limites : _staging = staging
    End Sub

    Public Function Confirmar(solicitud As ConfirmarPlanImportacionSolicitud) As ConfirmacionImportacionResultado
        If _referencia Is Nothing Then Throw NoDisponible()
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.planId) OrElse solicitud.revisionEsperada = 0 OrElse solicitud.revisionEsperada = UInteger.MaxValue OrElse
            solicitud.planHashEsperado Is Nothing OrElse Not Regex.IsMatch(solicitud.planHashEsperado, "\A[0-9a-f]{64}\z", RegexOptions.CultureInvariant) OrElse
            solicitud.claveConfirmacion Is Nothing OrElse Not Regex.IsMatch(solicitud.claveConfirmacion, "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant) Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La referencia de confirmación no es válida.")
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw Denegado()
        Dim referencia = _referencia(identidad, solicitud.planId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef OrElse Not ValidadorPerfilImportacion.ReferenciaValida(referencia.NivelRef) Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If contexto Is Nothing OrElse Not contexto.PuedeImportar OrElse contexto.UsuarioRef <> identidad.UsuarioRef OrElse contexto.AmbitoRef <> identidad.AmbitoRef OrElse contexto.NivelRef <> referencia.NivelRef Then Throw Denegado()
        If contexto.SelloDestino <> referencia.SelloDestino OrElse contexto.RutaCanonica <> referencia.RutaCanonica Then Throw Conflicto()
        Dim plan = _plan(contexto, solicitud.planId)
        If plan Is Nothing OrElse plan.PlanId <> solicitud.planId OrElse plan.Snapshot Is Nothing OrElse plan.Snapshot.Configuracion Is Nothing OrElse plan.Huella <> solicitud.planHashEsperado Then Throw Conflicto()
        _autorizar(contexto, plan.Snapshot.Configuracion)
        If plan.Estado <> "CONFIRMADO" Then
            If plan.Estado <> "REVISABLE" OrElse plan.Revision <> solicitud.revisionEsperada OrElse plan.Resultado Is Nothing OrElse Not plan.Resultado.PuedeConfirmar Then Throw Conflicto()
            Dim vencimiento As DateTime
            Dim ahora = _ahora()
            If Not DateTime.TryParseExact(plan.ExpiraUtc, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, vencimiento) OrElse ahora.Kind <> DateTimeKind.Utc OrElse vencimiento <= ahora Then Throw Conflicto()
            CanonizadorManifiestoImportacion.Canonizar(plan.Manifiesto, _limites)
            Dim actual = _perfiles.Seleccionar(contexto, plan.Snapshot.VersionId)
            If Not PerfilImportacionJson.MismaPublicacion(actual, plan.Snapshot) Then Throw Conflicto()
            Dim incidencias = _coincidencias(contexto, actual, plan.Manifiesto, plan.Resultado)
            If incidencias Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudieron comprobar las coincidencias.")
            If incidencias.Any(Function(i) i.BloqueaUnidad AndAlso (String.IsNullOrEmpty(i.RaizIdCliente) OrElse plan.Resultado.UnidadesElegibles.Contains(i.RaizIdCliente))) Then Throw Conflicto()
        End If
        'El repositorio decide repeticion y conflictos bajo su transaccion. Un
        'plan CONFIRMADO no vuelve a seleccionar perfil ni aplicar su expiracion.
        Dim lote = _confirmar(contexto, solicitud.planId, solicitud.revisionEsperada, solicitud.planHashEsperado, solicitud.claveConfirmacion)
        If lote Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(lote.LoteId) OrElse lote.PlanId <> solicitud.planId Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el lote confirmado.")
        Return New ConfirmacionImportacionResultado With {.loteId = lote.LoteId, .planId = lote.PlanId, .nivelRef = contexto.NivelRef, .estado = lote.Estado, .revision = lote.Revision, .totalRaices = lote.TotalRaices, .totalArchivos = lote.TotalArchivos, .totalBytes = lote.TotalBytes}
    End Function

    Public Function Recibir(solicitud As RecibirArchivoImportacionSolicitud, contenido As Stream) As RecepcionArchivoImportacionResultado
        ValidarSolicitud(solicitud)
        If contenido Is Nothing OrElse Not contenido.CanRead Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "El contenido es obligatorio.")
        Dim recurso = ObtenerRecepcion(solicitud)
        If recurso.Archivo.EstadoRecepcion = "RECIBIDO" Then
            Dim anterior = _recepciones.ObtenerIntentoRecepcion(recurso.Contexto, solicitud.loteId, solicitud.archivoId, Nothing)
            ValidarIntentoServicio(anterior, recurso.Archivo, solicitud.loteId)
            Dim retenido = _staging(recurso.Lote, recurso.Archivo).VerificarReenvio(recurso.Contexto, anterior, contenido)
            Return ResultadoRecepcion(recurso.Archivo, solicitud.loteId, retenido)
        End If
        Dim token = Guid.NewGuid().ToString("N")
        Dim comando As New ReclamacionImportacionCarpetas With {.LoteId = solicitud.loteId, .RecursoTipo = "ARCHIVO", .RecursoId = solicitud.archivoId, .Fase = "RECEPCION", .RevisionEsperada = recurso.Archivo.Revision, .EjecutorRef = "WEB_" & Guid.NewGuid().ToString("N"), .TokenReclamacion = token, .SolicitudHash = HuellaSolicitud(recurso.Contexto, solicitud)}
        Dim intento = _recepciones.ReservarIntento(recurso.Contexto, comando)
        ValidarIntentoServicio(intento, recurso.Archivo, solicitud.loteId)
        If intento.SolicitudHash <> comando.SolicitudHash Then Throw Conflicto()
        If intento.Estado = "RECHAZADO" Then Throw New PerfilImportacionException("ARCHIVO_RECHAZADO", "El intento fue rechazado. Revise la incidencia antes de iniciar otro intento.")
        Dim propio = intento.TokenReclamacion = token
        Dim completoCreado As Boolean = False
        Try
            Dim actual = _recepciones.ObtenerArchivo(recurso.Contexto, solicitud.loteId, solicitud.archivoId)
            ValidarIntentoServicio(intento, actual, solicitud.loteId)
            Dim almacen = _staging(recurso.Lote, actual)
            Dim recepcion As RecepcionVerificadaImportacion
            If propio Then
                If intento.Estado <> "EN_CURSO" OrElse actual.IntentoActivoId <> intento.IntentoId Then Throw Conflicto()
                recepcion = almacen.Recibir(recurso.Contexto, intento, contenido, actual.BytesPrevistos)
                completoCreado = True
                Dim verificada = almacen.Verificar(recurso.Contexto, intento)
                If PerfilImportacionJson.Escribir(recepcion) <> PerfilImportacionJson.Escribir(verificada) Then Throw Incierto()
            Else
                'Un replay solo verifica un completo inmutable. La falta de
                'archivo o una lease vencida nunca autorizan otra escritura.
                recepcion = almacen.VerificarReenvio(recurso.Contexto, intento, contenido)
                completoCreado = True
            End If
            _autorizar(recurso.Contexto, recurso.Lote.Snapshot.Configuracion)
            Dim confirmado = _recepciones.ConfirmarRecepcion(recurso.Contexto, solicitud.loteId, solicitud.archivoId, intento.IntentoId, intento.TokenReclamacion, recepcion)
            Return ResultadoRecepcion(confirmado, solicitud.loteId, recepcion)
        Catch ex As PerfilImportacionException
            If propio Then RegistrarFallo(recurso.Contexto, solicitud, intento, If(completoCreado, "RESULTADO_INCIERTO", ex.Codigo))
            If completoCreado AndAlso ex.Codigo <> "ACCESO_DENEGADO" Then Throw Incierto()
            Throw
        Catch ex As Exception
            If propio Then RegistrarFallo(recurso.Contexto, solicitud, intento, "RESULTADO_INCIERTO")
            Throw Incierto()
        End Try
    End Function

    Public Function ReconciliarRecepcion(solicitud As RecibirArchivoImportacionSolicitud) As RecepcionArchivoImportacionResultado
        ValidarSolicitud(solicitud)
        Dim recurso = ObtenerRecepcion(solicitud)
        Dim intento = _recepciones.ObtenerIntentoRecepcion(recurso.Contexto, solicitud.loteId, solicitud.archivoId, recurso.Archivo.IntentoActivoId)
        ValidarIntentoServicio(intento, recurso.Archivo, solicitud.loteId)
        If recurso.Archivo.EstadoRecepcion <> "RECIBIDO" AndAlso intento.SolicitudHash <> HuellaSolicitud(recurso.Contexto, solicitud) Then Throw Conflicto()
        Dim retenido = _staging(recurso.Lote, recurso.Archivo).Verificar(recurso.Contexto, intento)
        If recurso.Archivo.EstadoRecepcion = "RECIBIDO" Then Return ResultadoRecepcion(recurso.Archivo, solicitud.loteId, retenido)
        _autorizar(recurso.Contexto, recurso.Lote.Snapshot.Configuracion)
        Dim confirmado = _recepciones.ConfirmarRecepcion(recurso.Contexto, solicitud.loteId, solicitud.archivoId, intento.IntentoId, intento.TokenReclamacion, retenido)
        Return ResultadoRecepcion(confirmado, solicitud.loteId, retenido)
    End Function

    Private Class RecursoRecepcion
        Public Property Contexto As ContextoImportacionCarpetas
        Public Property Lote As LoteImportacionCarpetas
        Public Property Archivo As ArchivoImportacionCarpetas
    End Class

    Private Function ObtenerRecepcion(solicitud As RecibirArchivoImportacionSolicitud) As RecursoRecepcion
        If _recepciones Is Nothing OrElse _staging Is Nothing Then Throw NoDisponible()
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw Denegado()
        Dim referencia = _recepciones.ObtenerContextoLote(identidad, solicitud.loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef OrElse Not ValidadorPerfilImportacion.ReferenciaValida(referencia.NivelRef) Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If contexto Is Nothing OrElse Not contexto.PuedeImportar OrElse contexto.UsuarioRef <> identidad.UsuarioRef OrElse contexto.AmbitoRef <> identidad.AmbitoRef OrElse contexto.NivelRef <> referencia.NivelRef Then Throw Denegado()
        If contexto.SelloDestino <> referencia.SelloDestino OrElse contexto.RutaCanonica <> referencia.RutaCanonica Then Throw Conflicto()
        Dim lote = _recepciones.ObtenerLote(contexto, solicitud.loteId)
        If lote Is Nothing OrElse lote.LoteId <> solicitud.loteId OrElse lote.Contexto Is Nothing OrElse lote.Contexto.UsuarioRef <> contexto.UsuarioRef OrElse lote.Contexto.AmbitoRef <> contexto.AmbitoRef OrElse lote.Contexto.NivelRef <> contexto.NivelRef OrElse lote.Contexto.SelloDestino <> contexto.SelloDestino OrElse lote.Snapshot Is Nothing OrElse lote.Snapshot.Configuracion Is Nothing Then Throw Denegado()
        _autorizar(contexto, lote.Snapshot.Configuracion)
        Dim archivo = _recepciones.ObtenerArchivo(contexto, solicitud.loteId, solicitud.archivoId)
        If archivo Is Nothing OrElse archivo.ArchivoId <> solicitud.archivoId OrElse Not ValidadorPerfilImportacion.ReferenciaValida(archivo.RaizId) OrElse archivo.Revision = 0 OrElse Not EstadosImportacionCarpetas.EsRecepcion(archivo.EstadoRecepcion) OrElse archivo.EstadoRecepcion = "CANCELADO" OrElse archivo.EstadoEfecto = "RECHAZADO" OrElse archivo.EstadoEfecto = "CANCELADO" Then Throw Denegado()
        If archivo.EstadoRecepcion <> "RECIBIDO" AndAlso archivo.EstadoEfecto <> "NO_INICIADO" Then Throw Conflicto()
        Dim bytes As ULong
        If Not ULong.TryParse(archivo.BytesPrevistos, NumberStyles.None, CultureInfo.InvariantCulture, bytes) OrElse bytes = 0UL OrElse bytes > _limites.MaximoBytesArchivo Then Throw New PerfilImportacionException("LIMITE_EXCEDIDO", "El archivo no está dentro de los límites vigentes.")
        Return New RecursoRecepcion With {.Contexto = contexto, .Lote = lote, .Archivo = archivo}
    End Function

    Private Shared Sub ValidarSolicitud(solicitud As RecibirArchivoImportacionSolicitud)
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.loteId) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.archivoId) OrElse solicitud.claveIntento Is Nothing OrElse Not Regex.IsMatch(solicitud.claveIntento, "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant) Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La referencia de recepción no es válida.")
    End Sub
    Private Shared Sub ValidarIntentoServicio(intento As IntentoImportacionCarpetas, archivo As ArchivoImportacionCarpetas, loteId As String)
        If archivo Is Nothing OrElse intento Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(intento.IntentoId) OrElse intento.LoteId <> loteId OrElse intento.ArchivoId <> archivo.ArchivoId OrElse intento.RaizId <> archivo.RaizId OrElse intento.Fase <> "RECEPCION" OrElse Not EstadosImportacionCarpetas.EsIntento(intento.Estado) Then Throw Conflicto()
    End Sub
    Private Shared Function HuellaSolicitud(contexto As ContextoImportacionCarpetas, solicitud As RecibirArchivoImportacionSolicitud) As String
        Using sha = Security.Cryptography.SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Text.Encoding.ASCII.GetBytes(PerfilImportacionJson.Escribir(New With {.usuario = contexto.UsuarioRef, .empresa = contexto.AmbitoRef, .destino = contexto.SelloDestino, .lote = solicitud.loteId, .archivo = solicitud.archivoId, .clave = solicitud.claveIntento})))).Replace("-", "").ToLowerInvariant()
        End Using
    End Function
    Private Shared Function ResultadoRecepcion(archivo As ArchivoImportacionCarpetas, loteId As String, verificado As RecepcionVerificadaImportacion) As RecepcionArchivoImportacionResultado
        If archivo Is Nothing OrElse verificado Is Nothing OrElse archivo.Revision = 0 OrElse archivo.EstadoRecepcion <> "RECIBIDO" OrElse archivo.BytesRecibidos <> archivo.BytesPrevistos OrElse archivo.BytesRecibidos <> verificado.BytesRecibidos OrElse archivo.ContenidoHash <> verificado.ContenidoHash OrElse archivo.StagingRef <> verificado.StagingRef OrElse verificado.StagingRef Is Nothing OrElse Not Regex.IsMatch(verificado.StagingRef, "\A[0-9a-f]{32}\z", RegexOptions.CultureInvariant) OrElse verificado.ContenidoHash Is Nothing OrElse Not Regex.IsMatch(verificado.ContenidoHash, "\A[0-9a-f]{64}\z", RegexOptions.CultureInvariant) Then Throw Incierto()
        Return New RecepcionArchivoImportacionResultado With {.loteId = loteId, .archivoId = archivo.ArchivoId, .estadoRecepcion = archivo.EstadoRecepcion, .estadoEfecto = archivo.EstadoEfecto, .bytesRecibidos = archivo.BytesRecibidos, .contenidoHash = archivo.ContenidoHash, .revision = archivo.Revision}
    End Function
    Private Sub RegistrarFallo(contexto As ContextoImportacionCarpetas, solicitud As RecibirArchivoImportacionSolicitud, intento As IntentoImportacionCarpetas, codigo As String)
        Try
            Dim permitido = {"ARCHIVO_RECHAZADO", "LIMITE_EXCEDIDO", "FORMATO_NO_SOPORTADO", "SOLICITUD_INVALIDA", "RECEPCION_OCUPADA", "RESULTADO_INCIERTO", "CAPACIDAD_NO_DISPONIBLE", "ERROR_INTERNO", "ACCESO_DENEGADO", "CONFLICTO_REVISION", "OPERACION_EN_CURSO"}.Contains(codigo)
            _recepciones.RegistrarFalloRecepcion(contexto, solicitud.loteId, solicitud.archivoId, intento.IntentoId, intento.TokenReclamacion, If(permitido, codigo, "RESULTADO_INCIERTO"))
        Catch ex As Exception
            'Si la BD no esta disponible, conservar disco y la reclamacion. No
            'disfrazar el fallo como rechazo sin efectos ni iniciar otro intento.
        End Try
    End Sub
    Private Shared Function NoDisponible() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La capacidad de recepción no está disponible.")
    End Function
    Private Shared Function Incierto() As PerfilImportacionException
        Return New PerfilImportacionException("RESULTADO_INCIERTO", "La recepción no pudo comprobarse. Consulte y reconcilie antes de repetir.")
    End Function

    Private Shared Function Conflicto() As PerfilImportacionException
        Return New PerfilImportacionException("CONFLICTO_REVISION", "El plan o sus recursos cambiaron. Consulte su estado antes de continuar.")
    End Function
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "El plan no está disponible para este contexto.")
    End Function
End Class
