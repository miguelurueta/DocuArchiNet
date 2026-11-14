Option Strict On
Option Compare Binary
Imports System
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions

'Las consultas y el diagnostico siguen siendo de solo lectura. La limpieza
'es una capacidad interna aparte, compuesta explicitamente y sin endpoint.
Public Partial Class ServicioConsultaImportacion
    Private ReadOnly _identidad As Func(Of ContextoImportacionCarpetas)
    Private ReadOnly _repositorio As IConsultaImportacionRepository
    Private ReadOnly _destino As Func(Of String, ContextoImportacionCarpetas)
    Private ReadOnly _autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion)
    Private ReadOnly _maximoPagina As Integer
    Private ReadOnly _ahora As Func(Of DateTime)

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), repositorio As IConsultaImportacionRepository, destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), maximoPagina As Integer, Optional relojUtc As Func(Of DateTime) = Nothing)
        If identidad Is Nothing OrElse repositorio Is Nothing OrElse destino Is Nothing OrElse autorizar Is Nothing Then Throw New ArgumentNullException()
        If maximoPagina < 1 OrElse maximoPagina > 200 Then Throw NoDisponible()
        _identidad = identidad : _repositorio = repositorio : _destino = destino : _autorizar = autorizar : _maximoPagina = maximoPagina
        _ahora = If(relojUtc, Function() DateTime.UtcNow)
    End Sub

    Public Const MaximoPagina As Integer = 200

    Public Function Consultar(solicitud As ConsultarLoteImportacionSolicitud) As EstadoLoteImportacionResultado
        Dim lectura = Autorizar(solicitud)
        Dim lote = lectura.Lote
        'Autoridad vigente ya comprobada. Son solicitudes disponibles: cada
        'fase vuelve a validar recurso, revision, barrera y permisos inmediatos.
        Dim acciones As New List(Of String) From {"CONSULTAR", "EXPORTAR_PAGINA", "CONSULTAR_RAICES"}
        If Not {"COMPLETADO", "FALLIDO"}.Contains(lote.Estado) Then
            If Not lote.DetencionSolicitada Then acciones.Add("SOLICITAR_DETENCION")
            If lectura.HayIncertidumbre Then acciones.Add("SOLICITAR_VERIFICACION")
            If lote.Estado = "EN_CURSO" AndAlso lectura.HayIntentosActivos AndAlso Not lectura.HayIncertidumbre Then acciones.Add("SOLICITAR_REANUDACION")
            If Not lote.DetencionSolicitada AndAlso Not lectura.HayIncertidumbre AndAlso Not lectura.HayIntentosActivos AndAlso Not {"DETENIDO", "PENDIENTE_VERIFICACION"}.Contains(lote.Estado) Then acciones.Add("SOLICITAR_AVANCE")
        End If
        Return New EstadoLoteImportacionResultado With {.maximoTamanoPagina = _maximoPagina, .loteId = lote.LoteId, .nivelRef = lote.Contexto.NivelRef, .rutaCanonica = lote.Contexto.RutaCanonica,
            .perfilId = lote.Snapshot.PerfilId, .versionId = lote.Snapshot.VersionId, .estado = lote.Estado, .revision = lote.Revision,
            .detencionSolicitada = lote.DetencionSolicitada, .hayIntentosActivos = lectura.HayIntentosActivos, .hayIncertidumbre = lectura.HayIncertidumbre,
            .totalRaices = lote.TotalRaices, .totalArchivos = lote.TotalArchivos, .totalBytes = lote.TotalBytes,
            .modoIntegridad = lote.Snapshot.Configuracion.ModoIntegridad, .ambitoIntegridad = lote.Snapshot.Configuracion.AmbitoIntegridad,
            .accionesPermitidas = acciones}
    End Function

    Public Function ConsultarConfirmacion(solicitud As ConsultarConfirmacionImportacionSolicitud) As ConsultaConfirmacionImportacionResultado
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.planId) Then Throw Invalida()
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoPlan(identidad, solicitud.planId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef OrElse Not ValidadorPerfilImportacion.ReferenciaValida(referencia.NivelRef) Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoDestino(contexto, referencia) OrElse Not contexto.PuedeImportar Then Throw Denegado()
        Dim lectura = _repositorio.ConsultarConfirmacion(contexto, solicitud.planId)
        If lectura Is Nothing OrElse lectura.Plan Is Nothing Then Throw NoDisponible()
        Dim plan = lectura.Plan
        If plan.PlanId <> solicitud.planId OrElse Not MismoDestino(contexto, plan.Contexto) OrElse plan.Snapshot Is Nothing OrElse plan.Snapshot.Configuracion Is Nothing Then Throw Denegado()
        _autorizar(contexto, plan.Snapshot.Configuracion)
        If lectura.Lote IsNot Nothing AndAlso (lectura.Lote.PlanId <> plan.PlanId OrElse Not MismoDestino(contexto, lectura.Lote.Contexto) OrElse plan.Estado <> "CONFIRMADO" OrElse lectura.Lote.Snapshot Is Nothing OrElse lectura.Lote.Snapshot.VersionId <> plan.Snapshot.VersionId OrElse Not ValidadorPerfilImportacion.ReferenciaValida(lectura.Lote.LoteId)) Then Throw NoDisponible()
        'Sin lote visible no hay prueba negativa que permita repetir un efecto.
        Return New ConsultaConfirmacionImportacionResultado With {.planId = plan.PlanId, .confirmada = lectura.Lote IsNot Nothing, .loteId = If(lectura.Lote Is Nothing, Nothing, lectura.Lote.LoteId), .nivelRef = contexto.NivelRef}
    End Function

    Public Function ConsultarPagina(solicitud As ConsultarRecursosImportacionSolicitud) As PaginaConsultaImportacionResultado
        If solicitud Is Nothing Then Throw Invalida()
        Dim filtro As New FiltroConsultaImportacion With {.Pagina = solicitud.pagina, .TamanoPagina = solicitud.tamanoPagina, .RaizId = solicitud.raizId, .Estado = solicitud.estado, .EstadoRecepcion = solicitud.estadoRecepcion}
        MySqlImportacionCarpetasRepository.ValidarFiltroConsulta(solicitud.tipo, filtro)
        If filtro.TamanoPagina > _maximoPagina Then Throw Invalida()
        Dim lectura = Autorizar(solicitud), lote = lectura.Lote
        Dim salida As New PaginaConsultaImportacionResultado With {.loteId = lote.LoteId, .tipo = solicitud.tipo, .pagina = filtro.Pagina, .tamanoPagina = filtro.TamanoPagina,
            .totalOriginal = If(solicitud.tipo = "RAICES", lote.TotalRaices, lote.TotalArchivos)}
        If solicitud.tipo = "RAICES" Then
            Dim pagina = _repositorio.ConsultarRaices(lote.Contexto, lote.LoteId, filtro)
            If pagina Is Nothing OrElse pagina.Elementos Is Nothing Then Throw NoDisponible()
            salida.revisionLectura = pagina.RevisionLectura : salida.totalFiltrado = pagina.Total
            For Each raiz In pagina.Elementos
                If raiz Is Nothing OrElse raiz.LoteId <> lote.LoteId OrElse Not EstadosImportacionCarpetas.EsRaiz(raiz.Estado) Then Throw NoDisponible()
                salida.elementos.Add(New RecursoConsultaImportacionResultado With {.recursoId = Referencia(raiz.RaizId), .raizId = Referencia(raiz.RaizId), .idCliente = raiz.ClienteRaizId, .nombre = raiz.NombreOriginal, .estado = raiz.Estado, .revision = raiz.Revision,
                    .recepcionListaParaCrear = raiz.RecepcionListaParaCrear, .puedeRetomarCreacion = raiz.PuedeRetomarCreacion, .tieneIntentoActivo = Not String.IsNullOrEmpty(raiz.IntentoActivoId), .intentoActivoId = IntentoPublico(raiz.IntentoActivoId), .nombreExpediente = NombreExpedientePublico(raiz), .expedienteId = If(raiz.IdentidadExpediente Is Nothing, Nothing, Referencia(raiz.IdentidadExpediente.ExpedienteId)), .relacionNivelId = If(raiz.IdentidadExpediente Is Nothing, Nothing, Referencia(raiz.IdentidadExpediente.RelacionNivelId))})
            Next
        Else
            Dim pagina = _repositorio.ConsultarArchivos(lote.Contexto, lote.LoteId, filtro)
            If pagina Is Nothing OrElse pagina.Elementos Is Nothing Then Throw NoDisponible()
            salida.revisionLectura = pagina.RevisionLectura : salida.totalFiltrado = pagina.Total
            For Each archivo In pagina.Elementos
                If archivo Is Nothing OrElse Not EstadosImportacionCarpetas.EsRecepcion(archivo.EstadoRecepcion) OrElse Not EstadosImportacionCarpetas.EsEfecto(archivo.EstadoEfecto) Then Throw NoDisponible()
                Dim resultado = archivo.Resultado, expediente = If(resultado Is Nothing, Nothing, resultado.Expediente)
                salida.elementos.Add(New RecursoConsultaImportacionResultado With {.rutaRelativa = archivo.RutaRelativa, .ultimaModificacionLocal = archivo.UltimaModificacionLocal, .recursoId = Referencia(archivo.ArchivoId), .raizId = Referencia(archivo.RaizId), .idCliente = archivo.ClienteArchivoId, .nombre = archivo.NombreOriginal,
                    .estado = archivo.EstadoEfecto, .estadoRecepcion = archivo.EstadoRecepcion, .bytesPrevistos = archivo.BytesPrevistos, .bytesRecibidos = archivo.BytesRecibidos, .revision = archivo.Revision, .valoresEfectivosFijados = archivo.ValoresEfectivosFijados, .tieneIntentoActivo = Not String.IsNullOrEmpty(archivo.IntentoActivoId), .intentoActivoId = IntentoPublico(archivo.IntentoActivoId),
                    .expedienteId = If(expediente Is Nothing, Nothing, Referencia(expediente.ExpedienteId)), .relacionNivelId = If(expediente Is Nothing, Nothing, Referencia(expediente.RelacionNivelId)),
                    .gabineteId = If(resultado Is Nothing, Nothing, Referencia(resultado.GabineteId)), .documentoId = If(resultado Is Nothing, Nothing, Referencia(resultado.DocumentoId)), .codigoResultado = CodigoPublico(resultado)})
            Next
        End If
        If salida.revisionLectura = 0 OrElse salida.totalFiltrado > salida.totalOriginal OrElse salida.elementos.Count > filtro.TamanoPagina OrElse CULng(salida.elementos.Count) > salida.totalFiltrado Then Throw NoDisponible()
        salida.puedeHaberMas = (CLng(salida.pagina) - 1L) * salida.tamanoPagina + salida.elementos.Count < salida.totalFiltrado
        Return salida
    End Function

    'Exportacion explicita de UNA pagina, sin bucle sobre todo el lote.
    Public Function ExportarPagina(solicitud As ConsultarRecursosImportacionSolicitud) As ExportacionPaginaImportacionResultado
        Dim pagina = ConsultarPagina(solicitud)
        Dim csv As New StringBuilder()
        csv.AppendLine("""recursoId"",""raizId"",""idCliente"",""nombre"",""estado"",""estadoRecepcion"",""bytesPrevistos"",""bytesRecibidos"",""expedienteId"",""relacionNivelId"",""gabineteId"",""documentoId"",""codigoResultado""")
        For Each recurso In pagina.elementos
            csv.Append(String.Join(",", New String() {recurso.recursoId, recurso.raizId, recurso.idCliente, recurso.nombre, recurso.estado, recurso.estadoRecepcion, recurso.bytesPrevistos, recurso.bytesRecibidos, recurso.expedienteId, recurso.relacionNivelId, recurso.gabineteId, recurso.documentoId, recurso.codigoResultado}.Select(AddressOf CeldaCsv))).Append(vbCrLf)
        Next
        Return New ExportacionPaginaImportacionResultado With {.pagina = pagina, .nombreArchivo = "importacion-" & pagina.loteId & "-" & pagina.tipo.ToLowerInvariant() & "-pagina-" & pagina.pagina.ToString(CultureInfo.InvariantCulture) & ".csv", .contenido = csv.ToString()}
    End Function

    'Sin endpoint ni acceso al disco. La retencion llega de politica servidor,
    'nunca del DTO. Ausencia/invalidez mantiene todo retenido. Solo diagnostica
    'referencias registradas; no inventaria partes o copias huerfanas del disco.
    Public Function DiagnosticarRetencion(solicitud As ConsultarRecursosImportacionSolicitud, Optional retencion As Nullable(Of TimeSpan) = Nothing) As DiagnosticoRetencionImportacionResultado
        If solicitud Is Nothing OrElse solicitud.tipo <> "ARCHIVOS" Then Throw Invalida()
        Dim filtro As New FiltroConsultaImportacion With {.Pagina = solicitud.pagina, .TamanoPagina = solicitud.tamanoPagina, .RaizId = solicitud.raizId, .Estado = solicitud.estado, .EstadoRecepcion = solicitud.estadoRecepcion}
        MySqlImportacionCarpetasRepository.ValidarFiltroConsulta("ARCHIVOS", filtro)
        If filtro.TamanoPagina > _maximoPagina Then Throw Invalida()
        Dim ahora = _ahora()
        If ahora.Kind <> DateTimeKind.Utc Then Throw NoDisponible()
        Dim lectura = Autorizar(solicitud), lote = lectura.Lote
        Dim pagina = _repositorio.ConsultarArchivos(lote.Contexto, lote.LoteId, filtro)
        If pagina Is Nothing OrElse pagina.Elementos Is Nothing OrElse pagina.RevisionLectura = 0 OrElse pagina.Total > lote.TotalArchivos OrElse pagina.Elementos.Count > filtro.TamanoPagina OrElse CULng(pagina.Elementos.Count) > pagina.Total Then Throw NoDisponible()
        Dim salida As New DiagnosticoRetencionImportacionResultado With {.loteId = lote.LoteId, .pagina = filtro.Pagina, .tamanoPagina = filtro.TamanoPagina, .revisionLectura = pagina.RevisionLectura, .revisionEstable = pagina.RevisionLectura = lote.Revision,
            .totalOriginal = lote.TotalArchivos, .totalFiltrado = pagina.Total, .retencionConfigurada = retencion.HasValue AndAlso retencion.Value > TimeSpan.Zero, .evaluadoUtc = ahora.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
            .puedeHaberMas = (CLng(filtro.Pagina) - 1L) * filtro.TamanoPagina + pagina.Elementos.Count < pagina.Total}
        Dim actualizado As DateTime, vencimiento As Nullable(Of DateTime) = Nothing
        If salida.retencionConfigurada AndAlso DateTime.TryParseExact(lote.ActualizadoUtc, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, actualizado) AndAlso actualizado <= ahora Then
            Try
                vencimiento = actualizado.Add(retencion.Value)
                salida.venceRetencionUtc = vencimiento.Value.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture)
            Catch ex As ArgumentOutOfRangeException
                'Una fecha no calculable no habilita revision para eliminar.
            End Try
        End If
        For Each archivo In pagina.Elementos
            If archivo Is Nothing OrElse Not EstadosImportacionCarpetas.EsRecepcion(archivo.EstadoRecepcion) OrElse Not EstadosImportacionCarpetas.EsEfecto(archivo.EstadoEfecto) Then Throw NoDisponible()
            Dim item As New DiagnosticoArchivoRetencionImportacion With {.archivoId = Referencia(archivo.ArchivoId), .raizId = Referencia(archivo.RaizId), .referenciaTemporalRegistrada = Not String.IsNullOrEmpty(archivo.StagingRef)}
            If Not salida.revisionEstable Then
                item.motivo = "REVISION_CAMBIO"
            ElseIf lectura.HayIncertidumbre OrElse archivo.EstadoRecepcion = "INCIERTO" OrElse archivo.EstadoEfecto = "INCIERTO" Then
                item.motivo = "CONSERVAR_INCERTIDUMBRE"
            ElseIf lectura.HayIntentosActivos OrElse Not String.IsNullOrEmpty(archivo.IntentoActivoId) OrElse archivo.EstadoRecepcion = "RECIBIENDO" OrElse archivo.EstadoEfecto = "EN_CURSO" Then
                item.motivo = "CONSERVAR_OPERACION_ACTIVA"
            ElseIf Not salida.retencionConfigurada Then
                item.motivo = "RETENCION_NO_CONFIGURADA"
            ElseIf Not vencimiento.HasValue Then
                item.motivo = "FECHA_NO_VERIFICABLE"
            ElseIf ahora < vencimiento.Value Then
                item.motivo = "RETENCION_VIGENTE"
            ElseIf lote.Estado <> "COMPLETADO" OrElse archivo.EstadoEfecto <> "COMPLETADO" OrElse archivo.EstadoRecepcion <> "RECIBIDO" Then
                item.motivo = "CONSERVAR_TRABAJO_PENDIENTE"
            ElseIf Not EvidenciaRetencionVerificable(lote, archivo) Then
                item.motivo = "EVIDENCIA_NO_VERIFICABLE"
            Else
                item.motivo = "RETENCION_CUMPLIDA_REVISAR" : item.candidatoRevision = True
            End If
            salida.elementos.Add(item)
        Next
        salida.candidatosEnPagina = salida.elementos.Where(Function(item) item.candidatoRevision).Count()
        Return salida
    End Function

    Private Shared Function EvidenciaRetencionVerificable(lote As LoteImportacionCarpetas, archivo As ArchivoImportacionCarpetas) As Boolean
        Dim resultado = archivo.Resultado, configuracion = lote.Snapshot.Configuracion
        Return Regex.IsMatch(If(archivo.StagingRef, ""), "\A[0-9a-f]{32}\z", RegexOptions.CultureInvariant) AndAlso Regex.IsMatch(If(archivo.ContenidoHash, ""), "\A[0-9a-f]{64}\z", RegexOptions.CultureInvariant) AndAlso
            archivo.BytesRecibidos = archivo.BytesPrevistos AndAlso archivo.BytesRecibidos IsNot Nothing AndAlso Regex.IsMatch(archivo.BytesRecibidos, "\A[1-9][0-9]*\z", RegexOptions.CultureInvariant) AndAlso
            resultado IsNot Nothing AndAlso ValidadorPerfilImportacion.ReferenciaValida(resultado.DocumentoId) AndAlso configuracion.GabineteId.HasValue AndAlso resultado.GabineteId = configuracion.GabineteId.Value.ToString(CultureInfo.InvariantCulture) AndAlso resultado.OrigenGabinete = configuracion.OrigenGabinete AndAlso
            resultado.Expediente IsNot Nothing AndAlso resultado.Expediente.NivelRef = lote.Contexto.NivelRef AndAlso resultado.Expediente.OrigenLogico = "RA" AndAlso ValidadorPerfilImportacion.ReferenciaValida(resultado.Expediente.ExpedienteId) AndAlso ValidadorPerfilImportacion.ReferenciaValida(resultado.Expediente.RelacionNivelId)
    End Function

    Private Function Autorizar(solicitud As ConsultarLoteImportacionSolicitud) As EstadoLecturaImportacion
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.loteId) Then Throw Invalida()
        Dim identidad = _identidad()
        If identidad Is Nothing OrElse Not identidad.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(identidad.AmbitoRef) Then Throw Denegado()
        Dim referencia = _repositorio.ObtenerContextoLote(identidad, solicitud.loteId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef OrElse Not ValidadorPerfilImportacion.ReferenciaValida(referencia.NivelRef) Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoDestino(contexto, referencia) OrElse Not contexto.PuedeImportar Then Throw Denegado()
        Dim lectura = _repositorio.ConsultarEstado(contexto, solicitud.loteId)
        If lectura Is Nothing OrElse lectura.Lote Is Nothing Then Throw NoDisponible()
        Dim lote = lectura.Lote
        If lote.LoteId <> solicitud.loteId OrElse Not MismoDestino(contexto, lote.Contexto) OrElse lote.Snapshot Is Nothing OrElse lote.Snapshot.Configuracion Is Nothing OrElse Not EstadosImportacionCarpetas.EsLote(lote.Estado) OrElse lote.Revision = 0 Then Throw Denegado()
        _autorizar(contexto, lote.Snapshot.Configuracion)
        Return lectura
    End Function

    Private Shared Function MismoDestino(a As ContextoImportacionCarpetas, b As ContextoImportacionCarpetas) As Boolean
        Return a IsNot Nothing AndAlso b IsNot Nothing AndAlso a.UsuarioRef = b.UsuarioRef AndAlso a.AmbitoRef = b.AmbitoRef AndAlso a.NivelRef = b.NivelRef AndAlso a.SelloDestino = b.SelloDestino AndAlso a.RutaCanonica = b.RutaCanonica AndAlso b.PuedeImportar
    End Function

    Private Shared Function Referencia(valor As String) As String
        If valor IsNot Nothing AndAlso Not ValidadorPerfilImportacion.ReferenciaValida(valor) Then Throw NoDisponible()
        Return valor
    End Function

    Private Shared Function CodigoPublico(resultado As ResultadoEfectoImportacion) As String
        If resultado Is Nothing Then Return Nothing
        Dim conocidos = New String() {"ARCHIVO_RECIBIDO", "DOCUMENTO_ALMACENADO", "EXPEDIENTE_CONFIRMADO", "COMPLETADO", "RESULTADO_INCIERTO", "RECHAZADO", "ARCHIVO_RECHAZADO", "ACCESO_DENEGADO", "CONFLICTO_REVISION", "CAPACIDAD_NO_DISPONIBLE", "INCORPORACION_PENDIENTE"}
        Return If(conocidos.Contains(resultado.Codigo), resultado.Codigo, "RESULTADO_REGISTRADO")
    End Function

    Private Shared Function IntentoPublico(referencia As String) As String
        Return If(ValidadorPerfilImportacion.ReferenciaValida(referencia), referencia, Nothing)
    End Function

    Private Shared Function NombreExpedientePublico(raiz As RaizImportacionCarpetas) As String
        If raiz.IdentidadExpediente Is Nothing OrElse raiz.ValoresPlan Is Nothing OrElse Not {"EXPEDIENTE_CONFIRMADO", "COMPLETADA", "PARCIAL"}.Contains(raiz.Estado) Then Return Nothing
        Dim nombres = raiz.ValoresPlan.Where(Function(v) v IsNot Nothing AndAlso v.DestinoTipo = "CAMPO_EXPEDIENTE" AndAlso v.DestinoRef = "NOMBRE" AndAlso Not v.Diferido).ToList()
        Return If(nombres.Count = 1, nombres(0).Valor, Nothing)
    End Function

    Friend Shared Function CeldaCsv(valor As String) As String
        Dim texto = If(valor, ""), indice = 0
        While indice < texto.Length AndAlso (Char.IsWhiteSpace(texto(indice)) OrElse Char.IsControl(texto(indice)) OrElse texto(indice) = ChrW(&HFEFF) OrElse texto(indice) = ChrW(&H200B))
            indice += 1
        End While
        If (texto.Length > 0 AndAlso Char.IsControl(texto(0))) OrElse (indice < texto.Length AndAlso "=+-@".Contains(texto(indice))) Then texto = "'" & texto
        Return """" & texto.Replace("""", """""") & """"
    End Function

    Private Shared Function Invalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "La referencia o página de consulta no es válida.")
    End Function
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "La consulta del lote no está autorizada.")
    End Function
    Private Shared Function NoDisponible() As PerfilImportacionException
        Return New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La consulta no está disponible con la política o los datos actuales.")
    End Function
End Class
