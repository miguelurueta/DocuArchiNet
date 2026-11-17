Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Text.RegularExpressions

Public Class ServicioPreparacionSegmentadaImportacion
    Private ReadOnly _identidad As Func(Of ContextoImportacionCarpetas)
    Private ReadOnly _destino As Func(Of String, ContextoImportacionCarpetas)
    Private ReadOnly _autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion)
    Private ReadOnly _repo As IPreparacionSegmentadaImportacionRepository
    Private ReadOnly _perfiles As AdaptadorPerfilImportacionOperativa
    Private ReadOnly _calculo As ServicioPreparacionImportacion
    Private ReadOnly _limites As LimitesImportacionCarpetas
    Private ReadOnly _ahora As Func(Of DateTime)

    Public Sub New(identidad As Func(Of ContextoImportacionCarpetas), destino As Func(Of String, ContextoImportacionCarpetas), autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion), repositorio As IPreparacionSegmentadaImportacionRepository, perfiles As AdaptadorPerfilImportacionOperativa, calculo As ServicioPreparacionImportacion, limites As LimitesImportacionCarpetas, Optional reloj As Func(Of DateTime) = Nothing)
        If identidad Is Nothing OrElse destino Is Nothing OrElse autorizar Is Nothing OrElse repositorio Is Nothing OrElse perfiles Is Nothing OrElse calculo Is Nothing Then Throw New ArgumentNullException()
        LimitesImportacionCarpetas.Exigir(limites)
        _identidad = identidad : _destino = destino : _autorizar = autorizar : _repo = repositorio : _perfiles = perfiles : _calculo = calculo : _limites = limites : _ahora = If(reloj, Function() DateTime.UtcNow)
    End Sub

    Public Function Iniciar(s As IniciarPreparacionPlanSolicitud) As EstadoPreparacionPlanResultado
        If s Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(s.nivelRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(s.versionId) OrElse Not Regex.IsMatch(If(s.clavePreparacion, ""), "\A[0-9a-f]{32}\z") OrElse
            s.totalRaices = 0 OrElse s.totalArchivos = 0 OrElse s.totalRaices > _limites.MaximoRaices OrElse s.totalArchivos > _limites.MaximoArchivos OrElse s.partesRaices = 0 OrElse s.partesRaices > s.totalRaices OrElse s.partesArchivos = 0 OrElse s.partesArchivos > s.totalArchivos OrElse DecimalValido(s.totalBytes) > _limites.MaximoBytesLote Then Throw Invalida()
        Dim identidad = _identidad() : ExigirIdentidad(identidad)
        Dim contexto = _destino(s.nivelRef)
        ExigirIdentidad(contexto)
        If contexto.UsuarioRef <> identidad.UsuarioRef OrElse contexto.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        If contexto.NivelRef <> s.nivelRef Then Throw Denegado()
        _repo.VerificarEsquemaPreparacion()
        Dim snapshot = _perfiles.Seleccionar(contexto, s.versionId)
        _autorizar(contexto, snapshot.Configuracion)
        Dim instante = _ahora()
        If instante.Kind <> DateTimeKind.Utc Then Throw Invalida()
        instante = New DateTime(instante.Ticks - instante.Ticks Mod TimeSpan.TicksPerSecond, DateTimeKind.Utc)
        Dim p As New PreparacionSegmentadaImportacion With {.Contexto = contexto, .Snapshot = snapshot, .Clave = s.clavePreparacion, .TotalRaices = s.totalRaices, .TotalArchivos = s.totalArchivos, .TotalBytes = s.totalBytes,
            .PartesRaices = s.partesRaices, .PartesArchivos = s.partesArchivos, .Estado = "BORRADOR", .Revision = 1UI, .CreadoUtc = Utc(instante), .ExpiraUtc = Utc(instante.Add(_limites.DuracionPlan)),
            .CabeceraHash = PlanSegmentadoImportacionJson.HuellaTexto(PerfilImportacionJson.Escribir(New With {.solicitud = s, .contexto = contexto, .perfil = snapshot.Huella}))}
        Return Estado(_repo.IniciarPreparacion(contexto, p), "RESUMEN", 1)
    End Function

    Public Function Registrar(s As RegistrarPartePreparacionSolicitud) As EstadoPreparacionPlanResultado
        If s Is Nothing OrElse s.raices Is Nothing OrElse s.archivos Is Nothing OrElse Not {"RAICES", "ARCHIVOS"}.Contains(s.tipo) OrElse s.numero = 0 Then Throw Invalida()
        Dim p = AutorizarPreparacion(s.preparacionId, True)
        Dim contenido As String
        If s.tipo = "RAICES" Then
            If s.archivos.Count <> 0 OrElse s.raices.Count = 0 OrElse s.raices.Count > PlanSegmentadoImportacionJson.MaximoEntradasParte OrElse s.raices.Any(Function(r) r Is Nothing OrElse Not Correlacion(r.idCliente) OrElse Not Correlacion(r.seleccionId) OrElse String.IsNullOrWhiteSpace(r.nombre)) Then Throw Invalida()
            contenido = PerfilImportacionJson.Escribir(s.raices.Select(Function(r) New RaizManifiestoImportacion With {.IdCliente = r.idCliente, .SeleccionId = r.seleccionId, .Nombre = r.nombre}).ToList())
        Else
            If s.raices.Count <> 0 OrElse s.archivos.Count = 0 OrElse s.archivos.Count > PlanSegmentadoImportacionJson.MaximoEntradasParte OrElse s.archivos.Any(Function(a) a Is Nothing OrElse Not Correlacion(a.idCliente) OrElse Not Correlacion(a.raizIdCliente) OrElse String.IsNullOrEmpty(a.nombre) OrElse String.IsNullOrEmpty(a.rutaRelativa)) Then Throw Invalida()
            For Each archivoParte As ArchivoManifiestoImportacionSolicitud In s.archivos
                If DecimalValido(archivoParte.bytes) > _limites.MaximoBytesArchivo Then Throw Limite()
                DecimalValido(archivoParte.ultimaModificacionLocal)
            Next
            contenido = PerfilImportacionJson.Escribir(s.archivos.Select(Function(a) New ArchivoManifiestoImportacion With {.IdCliente = a.idCliente, .RaizIdCliente = a.raizIdCliente, .Nombre = a.nombre, .RutaRelativa = a.rutaRelativa, .Bytes = a.bytes, .UltimaModificacionLocal = a.ultimaModificacionLocal}).ToList())
        End If
        Dim parte As New PartePreparacionImportacion With {.Tipo = s.tipo, .Numero = s.numero, .Contenido = contenido, .Huella = PlanSegmentadoImportacionJson.HuellaTexto(contenido)}
        Return Estado(_repo.RegistrarParte(p.Contexto, p.PreparacionId, s.revisionEsperada, parte), "PARTES", 1)
    End Function

    Public Function Consultar(s As ConsultarPreparacionPlanSolicitud) As EstadoPreparacionPlanResultado
        If s Is Nothing OrElse Not {"RESUMEN", "PARTES"}.Contains(s.tipo) OrElse s.pagina < 1 Then Throw Invalida()
        If s.preparacionId Is Nothing Then
            If Not ValidadorPerfilImportacion.ReferenciaValida(s.nivelRef) OrElse Not Regex.IsMatch(If(s.clavePreparacion, ""), "\A[0-9a-f]{32}\z") Then Throw Invalida()
            Dim identidad = _identidad() : ExigirIdentidad(identidad)
            Dim contexto = _destino(s.nivelRef) : ExigirIdentidad(contexto)
            If contexto.UsuarioRef <> identidad.UsuarioRef OrElse contexto.AmbitoRef <> identidad.AmbitoRef OrElse contexto.NivelRef <> s.nivelRef Then Throw Denegado()
            Dim p = _repo.BuscarPreparacionPorClave(contexto, s.clavePreparacion)
            If p Is Nothing Then Return New EstadoPreparacionPlanResultado With {.existe = False, .estado = "NO_ENCONTRADA", .pagina = s.pagina}
            Return Estado(AutorizarPreparacion(p.PreparacionId, False), s.tipo, s.pagina)
        End If
        If s.clavePreparacion IsNot Nothing OrElse s.nivelRef IsNot Nothing Then Throw Invalida()
        Return Estado(AutorizarPreparacion(s.preparacionId, False), s.tipo, s.pagina)
    End Function

    Public Function Validar(s As ValidarPartePreparacionSolicitud) As EstadoPreparacionPlanResultado
        If s Is Nothing OrElse Not {"RAICES", "ARCHIVOS"}.Contains(s.tipo) OrElse s.numero = 0 Then Throw Invalida()
        Dim p = AutorizarPreparacion(s.preparacionId, True), partes = _repo.LeerPartesPreparacion(p.Contexto, p.PreparacionId)
        Dim parte = partes.SingleOrDefault(Function(a) a.Tipo = s.tipo AndAlso a.Numero = s.numero)
        If parte Is Nothing Then Throw Conflicto()
        If parte.Estado = "VALIDADA" Then Return Estado(p, "PARTES", 1)
        If p.Revision <> s.revisionEsperada Then Throw Conflicto()
        Dim raices = partes.Where(Function(a) a.Tipo = "RAICES").OrderBy(Function(a) a.Numero).ToList()
        If CULng(raices.Count) <> p.PartesRaices OrElse raices.Where(Function(a, i) a.Numero <> CUInt(i + 1)).Any() Then Throw Conflicto()
        Dim inventario = MySqlImportacionCarpetasRepository.InventarioDePartes(raices)
        If inventario.Raices.Count <> p.TotalRaices Then Throw Conflicto()
        Dim solicitud As New PrepararPlanImportacionSolicitud With {.nivelRef = p.Contexto.NivelRef, .versionId = p.Snapshot.VersionId}
        _calculo.IniciarLecturasPreparacion(p.Contexto, p.Snapshot)
        Dim selloInicial = _calculo.SelloClasificacion(p.Contexto, p.Snapshot)
        Dim plan As PlanImportacionCarpetas
        If s.tipo = "RAICES" Then
            solicitud.raices = PerfilImportacionJson.Leer(Of List(Of RaizManifiestoImportacion))(parte.Contenido).Select(Function(r) New RaizManifiestoImportacionSolicitud With {.idCliente = r.IdCliente, .seleccionId = r.SeleccionId, .nombre = r.Nombre}).ToList()
            plan = _calculo.CalcularRaicesSegmentadas(solicitud)
        Else
            Dim archivos = PerfilImportacionJson.Leer(Of List(Of ArchivoManifiestoImportacion))(parte.Contenido)
            solicitud.archivos = archivos.Select(Function(a) New ArchivoManifiestoImportacionSolicitud With {.idCliente = a.IdCliente, .raizIdCliente = a.RaizIdCliente, .nombre = a.Nombre, .rutaRelativa = a.RutaRelativa, .bytes = a.Bytes, .ultimaModificacionLocal = a.UltimaModificacionLocal}).ToList()
            solicitud.raices = inventario.Raices.Where(Function(r) archivos.Any(Function(a) a.RaizIdCliente = r.IdCliente)).Select(Function(r) New RaizManifiestoImportacionSolicitud With {.idCliente = r.IdCliente, .seleccionId = r.SeleccionId, .nombre = r.Nombre}).ToList()
            plan = _calculo.CalcularPlan(solicitud, 2US)
        End If
        If Not MismoContexto(p.Contexto, plan.Contexto) OrElse Not PerfilImportacionJson.MismaPublicacion(p.Snapshot, plan.Snapshot) Then Throw Conflicto()
        Dim sello = _calculo.SelloClasificacion(p.Contexto, p.Snapshot)
        If sello <> selloInicial Then Throw Conflicto()
        For Each elemento In plan.Resultado.Elementos
            If PerfilImportacionJson.Escribir(elemento).Length > 55000 Then Throw Limite()
        Next
        Return Estado(_repo.GuardarValidacionParte(p.Contexto, p.PreparacionId, p.Revision, s.tipo, s.numero, sello, PlanSegmentadoImportacionJson.Fragmentar(plan)), "PARTES", 1)
    End Function

    Public Function Sellar(s As SellarPreparacionPlanSolicitud) As RevisionPlanSegmentadoResultado
        If s Is Nothing Then Throw Invalida()
        Dim p = AutorizarPreparacion(s.preparacionId, False)
        If s.claveCierre <> p.Clave Then Throw Conflicto()
        If p.Estado = "SELLADA" Then Return Revision(New ConsultarRevisionPlanSolicitud With {.planId = p.PlanId, .revision = _repo.ObtenerPlan(p.Contexto, p.PlanId).Revision})
        ExigirVigencia(p)
        Dim partes = _repo.LeerPartesPreparacion(p.Contexto, p.PreparacionId)
        MySqlImportacionCarpetasRepository.ExigirPartesCompletas(p, partes, True)
        Dim sello = _calculo.SelloClasificacion(p.Contexto, p.Snapshot)
        If partes.Any(Function(a) a.SelloCatalogos <> sello) Then Throw Conflicto()
        Dim plan As New PlanImportacionCarpetas With {.EsquemaDoc = 2US, .PreparacionId = p.PreparacionId, .Contexto = p.Contexto, .Snapshot = p.Snapshot, .CreadoUtc = p.CreadoUtc, .ExpiraUtc = p.ExpiraUtc,
            .Manifiesto = CanonizadorManifiestoImportacion.Canonizar(MySqlImportacionCarpetasRepository.InventarioDePartes(partes), _limites, 2US), .Resultado = New ResultadoPreparacionImportacion()}
        Dim fragmentos = _repo.LeerResultadosPreparacion(p.Contexto, p.PreparacionId)
        For Each parte In partes
            Dim prefijo = MySqlImportacionCarpetasRepository.PrefijoResultadoParte(parte.Tipo, parte.Numero)
            Dim originales = fragmentos.Where(Function(f) f.Tipo.StartsWith(prefijo, StringComparison.Ordinal)).Select(Function(f) New FragmentoPlanImportacion With {.Tipo = f.Tipo.Substring(prefijo.Length), .Numero = f.Numero, .Contenido = f.Contenido, .Huella = f.Huella}).ToList()
            Dim parcial As New PlanImportacionCarpetas With {.EsquemaDoc = 2US}
            PlanSegmentadoImportacionJson.Reconstruir(parcial, originales)
            plan.Resultado.Elementos.AddRange(parcial.Resultado.Elementos.Where(Function(e) e.Ambito = If(parte.Tipo = "RAICES", "RAIZ", "ARCHIVO")))
            plan.Resultado.Incidencias.AddRange(parcial.Resultado.Incidencias.Where(Function(i) String.IsNullOrEmpty(i.RaizIdCliente) OrElse If(parte.Tipo = "RAICES", String.IsNullOrEmpty(i.ArchivoIdCliente), Not String.IsNullOrEmpty(i.ArchivoIdCliente))))
        Next
        plan.Resultado.Incidencias.AddRange(CanonizadorManifiestoImportacion.EvaluarAgrupacion(plan.Manifiesto, p.Snapshot.Configuracion))
        plan.Resultado.Incidencias = plan.Resultado.Incidencias.GroupBy(Function(i) PerfilImportacionJson.Escribir(i), StringComparer.Ordinal).Select(Function(g) g.First()).ToList()
        ServicioPreparacionImportacion.EvaluarElegibilidad(plan.Manifiesto, p.Snapshot.Configuracion, plan.Resultado)
        Dim guardado = _repo.SellarPreparacion(p.Contexto, p.PreparacionId, s.revisionEsperada, s.claveCierre, plan)
        Return ResumenRevision(guardado, "RESUMEN", 1)
    End Function

    Public Function Revision(s As ConsultarRevisionPlanSolicitud) As RevisionPlanSegmentadoResultado
        If s Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(s.planId) OrElse s.revision = 0 OrElse s.pagina < 1 OrElse Not {"RESUMEN", "VALORES", "INCIDENCIAS"}.Contains(s.tipo) OrElse (s.ambito IsNot Nothing AndAlso Not {"RAIZ", "ARCHIVO"}.Contains(s.ambito)) Then Throw Invalida()
        Dim identidad = _identidad() : ExigirIdentidad(identidad)
        Dim referencia = _repo.ObtenerContextoPlan(identidad, s.planId)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContexto(contexto, referencia) Then Throw Denegado()
        Dim plan = _repo.ObtenerPlan(contexto, s.planId)
        If plan Is Nothing OrElse plan.Revision <> s.revision Then Throw Conflicto()
        _autorizar(contexto, plan.Snapshot.Configuracion)
        Dim salida = ResumenRevision(plan, s.tipo, s.pagina)
        If s.tipo = "VALORES" Then
            salida.valores = PaginaAcotada(plan.Resultado.Elementos.Where(Function(e) s.ambito Is Nothing OrElse e.Ambito = s.ambito), s.pagina, salida.puedeHaberMas)
            salida.unidadesElegiblesPagina = salida.valores.Where(Function(e) e.Ambito = "RAIZ" AndAlso plan.Resultado.UnidadesElegibles.Contains(e.RaizIdCliente)).Select(Function(e) e.RaizIdCliente).ToList()
        ElseIf s.tipo = "INCIDENCIAS" Then
            salida.incidencias = PaginaAcotada(plan.Resultado.Incidencias, s.pagina, salida.puedeHaberMas)
        End If
        Return salida
    End Function

    Private Function AutorizarPreparacion(id As String, escritura As Boolean) As PreparacionSegmentadaImportacion
        If Not ValidadorPerfilImportacion.ReferenciaValida(id) Then Throw Invalida()
        Dim identidad = _identidad() : ExigirIdentidad(identidad)
        Dim referencia = _repo.ObtenerReferenciaPreparacion(identidad, id)
        If referencia Is Nothing OrElse referencia.UsuarioRef <> identidad.UsuarioRef OrElse referencia.AmbitoRef <> identidad.AmbitoRef Then Throw Denegado()
        Dim contexto = _destino(referencia.NivelRef)
        If Not MismoContexto(contexto, referencia) Then Throw Denegado()
        Dim p = _repo.ObtenerPreparacion(contexto, id)
        If p Is Nothing OrElse Not MismoContexto(p.Contexto, contexto) Then Throw Denegado()
        _autorizar(contexto, p.Snapshot.Configuracion)
        If escritura Then ExigirVigencia(p)
        Return p
    End Function

    Private Sub ExigirVigencia(p As PreparacionSegmentadaImportacion)
        If p.Estado <> "BORRADOR" Then Throw Conflicto()
        If DateTime.ParseExact(p.ExpiraUtc, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal) <= _ahora() Then Throw New PerfilImportacionException("PLAN_EXPIRADO", "La preparación venció; prepara una nueva revisión.")
        If Not PerfilImportacionJson.MismaPublicacion(_perfiles.Seleccionar(p.Contexto, p.Snapshot.VersionId), p.Snapshot) Then Throw New PerfilImportacionException("PERFIL_CAMBIADO", "La configuración cambió; prepara una nueva revisión.")
    End Sub

    Private Function Estado(p As PreparacionSegmentadaImportacion, tipo As String, pagina As Integer) As EstadoPreparacionPlanResultado
        Dim partes = _repo.LeerPartesPreparacion(p.Contexto, p.PreparacionId)
        Dim salida As New EstadoPreparacionPlanResultado With {.preparacionId = p.PreparacionId, .revision = p.Revision, .estado = p.Estado, .planId = p.PlanId, .expiraUtc = p.ExpiraUtc, .totalRaices = p.TotalRaices, .totalArchivos = p.TotalArchivos, .partesRecibidas = partes.Count, .partesValidadas = partes.Where(Function(a) a.Estado = "VALIDADA").Count(), .pagina = pagina}
        For Each parte In partes.Where(Function(a) a.Tipo = "ARCHIVOS")
            Dim cantidad = PerfilImportacionJson.Leer(Of List(Of ArchivoManifiestoImportacion))(parte.Contenido).Count
            salida.archivosRegistrados += cantidad
            If parte.Estado = "VALIDADA" Then salida.archivosRevisados += cantidad
        Next
        If p.Estado = "BORRADOR" AndAlso DateTime.ParseExact(p.ExpiraUtc, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal) <= _ahora() Then salida.estado = "EXPIRADA"
        If tipo = "PARTES" Then
            salida.partes = partes.OrderBy(Function(a) a.Tipo, StringComparer.Ordinal).ThenBy(Function(a) a.Numero).Skip(CInt(Math.Min(CLng(partes.Count), (CLng(pagina) - 1L) * 25L))).Take(25).Select(Function(a) New EstadoPartePreparacionResultado With {.tipo = a.Tipo, .numero = a.Numero, .hash = a.Huella, .estado = a.Estado}).ToList()
            salida.puedeHaberMas = CLng(pagina) * 25L < partes.Count
        End If
        Return salida
    End Function

    Private Shared Function ResumenRevision(p As PlanImportacionCarpetas, tipo As String, pagina As Integer) As RevisionPlanSegmentadoResultado
        Return New RevisionPlanSegmentadoResultado With {.planId = p.PlanId, .revision = p.Revision, .planHash = p.Huella, .nivelRef = p.Contexto.NivelRef, .expiraUtc = p.ExpiraUtc, .puedeConfirmar = p.Resultado.PuedeConfirmar, .unidadesElegibles = p.Resultado.UnidadesElegibles.Count, .totalArchivos = p.Manifiesto.Archivos.Count, .totalIncidencias = p.Resultado.Incidencias.Count, .tipo = tipo, .pagina = pagina}
    End Function

    Private Shared Function PaginaAcotada(Of T)(filas As IEnumerable(Of T), pagina As Integer, ByRef mas As Boolean) As List(Of T)
        Dim salida As New List(Of T)(), actual = 1, cantidad = 0, caracteres = 4096
        For Each fila In filas
            Dim longitud = PerfilImportacionJson.Escribir(fila).Length + 1
            If longitud > 55000 Then Throw Limite()
            If cantidad = 25 OrElse caracteres + longitud > 61440 Then
                actual += 1 : cantidad = 0 : caracteres = 4096
            End If
            If actual > pagina Then
                mas = True : Exit For
            End If
            If actual = pagina Then salida.Add(fila)
            cantidad += 1 : caracteres += longitud
        Next
        Return salida
    End Function

    Private Shared Sub ExigirIdentidad(c As ContextoImportacionCarpetas)
        If c Is Nothing OrElse Not c.PuedeImportar OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(c.AmbitoRef) Then Throw Denegado()
    End Sub
    Private Shared Function MismoContexto(a As ContextoImportacionCarpetas, b As ContextoImportacionCarpetas) As Boolean
        Return a IsNot Nothing AndAlso b IsNot Nothing AndAlso a.PuedeImportar AndAlso a.UsuarioRef = b.UsuarioRef AndAlso a.AmbitoRef = b.AmbitoRef AndAlso a.NivelRef = b.NivelRef AndAlso a.SelloDestino = b.SelloDestino AndAlso a.RutaCanonica = b.RutaCanonica
    End Function
    Private Shared Function DecimalValido(s As String) As ULong
        Dim valor As ULong
        If Not ULong.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, valor) OrElse valor.ToString(CultureInfo.InvariantCulture) <> s Then Throw Invalida()
        Return valor
    End Function
    Private Shared Function Correlacion(s As String) As Boolean
        Return Regex.IsMatch(If(s, ""), "\A[A-Za-z0-9_-]{1,64}\z", RegexOptions.CultureInvariant)
    End Function
    Private Shared Function Utc(d As DateTime) As String
        Return d.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture)
    End Function
    Private Shared Function Invalida() As PerfilImportacionException
        Return New PerfilImportacionException("SOLICITUD_INVALIDA", "Los datos de la preparación por partes no son válidos.")
    End Function
    Private Shared Function Denegado() As PerfilImportacionException
        Return New PerfilImportacionException("ACCESO_DENEGADO", "La preparación no está disponible para esta cuenta y destino.")
    End Function
    Private Shared Function Conflicto() As PerfilImportacionException
        Return New PerfilImportacionException("CONFLICTO_REVISION", "La preparación cambió o no está completa. Consulta las partes confirmadas antes de continuar.")
    End Function
    Private Shared Function Limite() As PerfilImportacionException
        Return New PerfilImportacionException("LIMITE_EXCEDIDO", "Un elemento de la revisión supera el tamaño permitido. Revisa sus datos; no se enviaron sus archivos.")
    End Function
End Class
