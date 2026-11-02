Option Strict On
Option Compare Binary
Imports System
Imports System.Globalization
Imports System.Collections.Generic
Imports System.Linq

'La apertura solo consulta. La autoridad y el destino se vuelven a resolver
'en cada llamada; no se conserva seleccion mutable ni contexto de otra peticion.
Public Class ServicioPreparacionImportacion
    Private ReadOnly _destino As Func(Of String, ContextoImportacionCarpetas)
    Private ReadOnly _perfiles As AdaptadorPerfilImportacionOperativa
    Private ReadOnly _limites As LimitesImportacionCarpetas
    Private ReadOnly _esquema As Action
    Private ReadOnly _catalogos As IPerfilImportacionCatalogosOperativos
    Private ReadOnly _resolvedor As CoordinadorProveedorIndices
    Private ReadOnly _coincidencias As Func(Of ContextoImportacionCarpetas, SnapshotPerfilImportacionOperativa, ManifiestoImportacionCarpetas, ResultadoPreparacionImportacion, List(Of IncidenciaImportacionCarpetas))
    Private ReadOnly _guardar As Func(Of ContextoImportacionCarpetas, PlanImportacionCarpetas, PlanImportacionCarpetas)
    Private ReadOnly _ahora As Func(Of DateTime)

    Public Sub New(destino As Func(Of String, ContextoImportacionCarpetas), perfiles As AdaptadorPerfilImportacionOperativa, limites As LimitesImportacionCarpetas, verificarEsquema As Action, Optional catalogos As IPerfilImportacionCatalogosOperativos = Nothing, Optional resolvedor As CoordinadorProveedorIndices = Nothing, Optional coincidencias As Func(Of ContextoImportacionCarpetas, SnapshotPerfilImportacionOperativa, ManifiestoImportacionCarpetas, ResultadoPreparacionImportacion, List(Of IncidenciaImportacionCarpetas)) = Nothing, Optional guardar As Func(Of ContextoImportacionCarpetas, PlanImportacionCarpetas, PlanImportacionCarpetas) = Nothing, Optional relojUtc As Func(Of DateTime) = Nothing)
        If destino Is Nothing OrElse perfiles Is Nothing OrElse verificarEsquema Is Nothing Then Throw New ArgumentNullException()
        LimitesImportacionCarpetas.Exigir(limites)
        _destino = destino : _perfiles = perfiles : _limites = limites : _esquema = verificarEsquema
        _catalogos = catalogos : _resolvedor = resolvedor
        _coincidencias = coincidencias : _guardar = guardar : _ahora = If(relojUtc, Function() DateTime.UtcNow)
    End Sub

    Public Function Abrir(solicitud As AbrirImportacionSolicitud) As AperturaImportacionResultado
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.nivelRef) Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "El nivel de origen es obligatorio.")
        Dim contexto = _destino(solicitud.nivelRef)
        If contexto Is Nothing OrElse contexto.NivelRef <> solicitud.nivelRef OrElse Not contexto.PuedeImportar OrElse
            Not ValidadorPerfilImportacion.ReferenciaValida(contexto.UsuarioRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(contexto.AmbitoRef) OrElse
            String.IsNullOrWhiteSpace(contexto.RutaCanonica) OrElse String.IsNullOrWhiteSpace(contexto.SelloDestino) Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El destino no está disponible.")
        _esquema()
        Dim pagina = _perfiles.ListarPagina(contexto, 1, 50)
        Return New AperturaImportacionResultado With {
            .nivelRef = contexto.NivelRef, .rutaCanonica = contexto.RutaCanonica, .selloDestino = contexto.SelloDestino,
            .limites = New LimitesAperturaImportacion With {
                .maximoRaices = _limites.MaximoRaices, .maximoArchivos = _limites.MaximoArchivos,
                .maximoBytesArchivo = _limites.MaximoBytesArchivo.ToString(CultureInfo.InvariantCulture),
                .maximoBytesLote = _limites.MaximoBytesLote.ToString(CultureInfo.InvariantCulture),
                .maximoBytesManifiesto = _limites.MaximoBytesManifiesto},
            .perfiles = pagina.perfiles, .paginaPerfiles = pagina}
    End Function

    Public Function ListarPerfiles(solicitud As ListarPerfilesImportacionSolicitud) As PaginaPerfilesImportacionOperativa
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.nivelRef) Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "El nivel de origen es obligatorio.")
        Dim contexto = _destino(solicitud.nivelRef)
        If contexto Is Nothing OrElse contexto.NivelRef <> solicitud.nivelRef Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El destino no está disponible.")
        _esquema()
        Return _perfiles.ListarPagina(contexto, solicitud.pagina, solicitud.tamanoPagina)
    End Function

    'Calculo sin persistencia ni efectos. Las coincidencias se consultan en el
    'origen documental, nunca mediante una comparacion por nombre de carpeta.
    Public Function CalcularPlan(solicitud As PrepararPlanImportacionSolicitud) As PlanImportacionCarpetas
        If solicitud Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.nivelRef) OrElse Not ValidadorPerfilImportacion.ReferenciaValida(solicitud.versionId) OrElse
            solicitud.raices Is Nothing OrElse solicitud.archivos Is Nothing OrElse solicitud.raices.Any(Function(r) r Is Nothing) OrElse solicitud.archivos.Any(Function(a) a Is Nothing) Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "El inventario y sus referencias son obligatorios.")
        Dim contexto = _destino(solicitud.nivelRef)
        If contexto Is Nothing OrElse contexto.NivelRef <> solicitud.nivelRef Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El destino no está disponible.")
        If _catalogos Is Nothing OrElse _resolvedor Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La resolución operativa no está disponible.")
        _esquema()
        Dim manifiesto = CanonizadorManifiestoImportacion.Canonizar(New ManifiestoImportacionCarpetas With {
            .Raices = solicitud.raices.Select(Function(r) New RaizManifiestoImportacion With {.SeleccionId = r.seleccionId, .IdCliente = r.idCliente, .Nombre = r.nombre}).ToList(),
            .Archivos = solicitud.archivos.Select(Function(a) New ArchivoManifiestoImportacion With {.IdCliente = a.idCliente, .RaizIdCliente = a.raizIdCliente, .Nombre = a.nombre, .RutaRelativa = a.rutaRelativa, .Bytes = a.bytes, .UltimaModificacionLocal = a.ultimaModificacionLocal}).ToList()}, _limites)
        Dim snapshot = _perfiles.Seleccionar(contexto, solicitud.versionId)
        Dim errores = New ValidadorPerfilImportacion().Validar(snapshot.Configuracion, snapshot.Reglas, True)
        If errores.Count <> 0 Then Throw New ConfiguracionPerfilImportacionException(errores)
        Dim resultado As New ResultadoPreparacionImportacion()
        resultado.Incidencias.AddRange(CanonizadorManifiestoImportacion.EvaluarAgrupacion(manifiesto, snapshot.Configuracion))
        Dim clasificacion = _catalogos.ClasificacionOperativa(contexto, snapshot.Configuracion)
        If clasificacion Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar la clasificación.")
        Dim reglasRaiz = snapshot.Reglas.Where(Function(r) r.DestinoTipo = "CAMPO_EXPEDIENTE").ToList()
        Dim reglasArchivo = snapshot.Reglas.Where(Function(r) r.DestinoTipo <> "CAMPO_EXPEDIENTE").ToList()
        Dim raices = manifiesto.Raices.ToDictionary(Function(r) r.IdCliente, StringComparer.Ordinal)
        Dim ejemplosArchivo = manifiesto.Archivos.Select(Function(a) New EjemploPerfilImportacion With {
            .NombreCarpeta = raices(a.RaizIdCliente).Nombre, .NombreArchivo = a.Nombre,
            .Expediente = New Dictionary(Of String, String)(clasificacion, StringComparer.Ordinal)}).ToList()
        'Una sola resolucion documental por preparacion: el coordinador comparte
        'la respuesta de una version/clave entre campos y archivos de esta llamada.
        Dim documentosResueltos = _resolvedor.ResolverOperativo(contexto, reglasArchivo, ejemplosArchivo)
        Dim resultadosArchivo As New Dictionary(Of String, ResultadoEnsayoPerfilImportacion)(StringComparer.Ordinal)
        For indice = 0 To manifiesto.Archivos.Count - 1
            Dim archivo = manifiesto.Archivos(indice)
            resultadosArchivo.Add(archivo.RaizIdCliente & ":" & archivo.IdCliente, documentosResueltos(indice))
        Next
        For Each raiz In manifiesto.Raices
            Dim ejemplo As New EjemploPerfilImportacion With {.NombreCarpeta = raiz.Nombre, .Expediente = New Dictionary(Of String, String)(clasificacion, StringComparer.Ordinal)}
            Dim resuelto = _resolvedor.ResolverOperativo(contexto, reglasRaiz, New List(Of EjemploPerfilImportacion) From {ejemplo})(0)
            Dim elemento As New ValoresElementoImportacion With {.Ambito = "RAIZ", .RaizIdCliente = raiz.IdCliente, .Valores = resuelto.Valores}
            resultado.Elementos.Add(elemento)
            AgregarIncidencias(resultado, elemento, resuelto.Incidencias)
            For Each valor In elemento.Valores.Where(Function(v) v.DestinoRef = "CODIGO" AndAlso v.Valor IsNot Nothing AndAlso v.Valor.Length > 45)
                AgregarIncidencias(resultado, elemento, New List(Of IncidenciaPerfilImportacion) From {New IncidenciaPerfilImportacion("CAMPO_EXPEDIENTE:CODIGO", "LONGITUD_CODIGO_EXCEDIDA", "El código de expediente admite hasta 45 caracteres en este recorrido.")})
            Next
            For Each valor In elemento.Valores.Where(Function(v) v.DestinoRef = "NOMBRE" AndAlso v.Valor IsNot Nothing AndAlso v.Valor.Length > 120)
                AgregarIncidencias(resultado, elemento, New List(Of IncidenciaPerfilImportacion) From {New IncidenciaPerfilImportacion("CAMPO_EXPEDIENTE:NOMBRE", "LONGITUD_NOMBRE_EXCEDIDA", "El nombre visible de expediente admite hasta 120 caracteres en este recorrido.")})
            Next
            AgregarIncidencias(resultado, elemento, _catalogos.ValidarValoresOperativos(contexto, snapshot.Configuracion, elemento.Valores))
            'Solo clasificacion conocida en preflight. Las propiedades del futuro
            'expediente conservan su dependencia tipada, sin fabricar un ID.
            For Each archivo In manifiesto.Archivos.Where(Function(a) a.RaizIdCliente = raiz.IdCliente)
                Dim salida = resultadosArchivo(archivo.RaizIdCliente & ":" & archivo.IdCliente)
                Dim documento As New ValoresElementoImportacion With {.Ambito = "ARCHIVO", .RaizIdCliente = raiz.IdCliente, .ArchivoIdCliente = archivo.IdCliente, .Valores = salida.Valores}
                resultado.Elementos.Add(documento)
                AgregarIncidencias(resultado, documento, salida.Incidencias)
                AgregarIncidencias(resultado, documento, _catalogos.ValidarValoresOperativos(contexto, snapshot.Configuracion, documento.Valores))
                Dim extension = IO.Path.GetExtension(archivo.Nombre).TrimStart("."c).ToLowerInvariant()
                If snapshot.Configuracion.Extensiones.Count > 0 AndAlso Not snapshot.Configuracion.Extensiones.Contains(extension, StringComparer.OrdinalIgnoreCase) Then
                    AgregarIncidencias(resultado, documento, New List(Of IncidenciaPerfilImportacion) From {New IncidenciaPerfilImportacion("nombre", "EXTENSION_NO_PERMITIDA", "La extensión no está permitida por el perfil.")})
                End If
                If snapshot.Configuracion.MaximoBytesArchivo.HasValue AndAlso ULong.Parse(archivo.Bytes, CultureInfo.InvariantCulture) > CULng(snapshot.Configuracion.MaximoBytesArchivo.Value) Then
                    AgregarIncidencias(resultado, documento, New List(Of IncidenciaPerfilImportacion) From {New IncidenciaPerfilImportacion("bytes", "LIMITE_PERFIL_EXCEDIDO", "El archivo excede el límite del perfil.")})
                End If
            Next
        Next
        Try
            If _coincidencias Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La consulta de coincidencias no está disponible.")
            Dim incidencias = _coincidencias(contexto, snapshot, manifiesto, resultado)
            If incidencias Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La consulta de coincidencias no está disponible.")
            resultado.Incidencias.AddRange(incidencias)
        Catch ex As PerfilImportacionException When ex.Codigo = "CAPACIDAD_NO_DISPONIBLE"
            resultado.Incidencias.Add(New IncidenciaImportacionCarpetas With {.Ambito = "GLOBAL", .Campo = "coincidencias", .Codigo = ex.Codigo, .Mensaje = "No se pudo comprobar la comparación o las coincidencias del origen documental.", .BloqueaUnidad = True})
        End Try
        EvaluarElegibilidad(manifiesto, snapshot.Configuracion, resultado)
        Return New PlanImportacionCarpetas With {.Contexto = contexto, .Snapshot = snapshot, .Manifiesto = manifiesto, .Resultado = resultado, .EsquemaDoc = 1US, .Estado = If(resultado.PuedeConfirmar, "REVISABLE", "BLOQUEADO")}
    End Function

    Public Function Preparar(solicitud As PrepararPlanImportacionSolicitud) As PreparacionImportacionResultado
        If _guardar Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "La persistencia del plan no está disponible.")
        Dim plan = CalcularPlan(solicitud)
        Dim instante = _ahora()
        If instante.Kind <> DateTimeKind.Utc Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "El reloj del servidor debe usar UTC.")
        instante = New DateTime(instante.Ticks - instante.Ticks Mod TimeSpan.TicksPerSecond, DateTimeKind.Utc)
        plan.CreadoUtc = instante.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture)
        plan.ExpiraUtc = instante.Add(_limites.DuracionPlan).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture)
        'GuardarPlan es el repositorio unico ya existente. No confirma lote ni
        'crea efectos documentales; devuelve su representacion persistida.
        Dim guardado = _guardar(plan.Contexto, plan)
        If guardado Is Nothing OrElse Not ValidadorPerfilImportacion.ReferenciaValida(guardado.PlanId) OrElse guardado.Revision = 0 OrElse String.IsNullOrEmpty(guardado.Huella) Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudo comprobar el plan persistido.")
        Return New PreparacionImportacionResultado With {.planId = guardado.PlanId, .revision = guardado.Revision, .planHash = guardado.Huella, .expiraUtc = guardado.ExpiraUtc, .nivelRef = guardado.Contexto.NivelRef, .snapshot = guardado.Snapshot, .resultado = guardado.Resultado}
    End Function

    Friend Shared Sub EvaluarElegibilidad(manifiesto As ManifiestoImportacionCarpetas, configuracion As ConfiguracionPerfilImportacion, resultado As ResultadoPreparacionImportacion)
        resultado.UnidadesElegibles.Clear()
        resultado.PuedeConfirmar = False
        If resultado.Incidencias.Any(Function(i) i.BloqueaUnidad AndAlso String.IsNullOrEmpty(i.RaizIdCliente)) Then Return
        Dim parcial = configuracion.ModoIntegridad = "PARCIAL_PERMITIDO" AndAlso configuracion.AmbitoIntegridad = "NO_APLICA"
        Dim completo = configuracion.ModoIntegridad = "COMPLETO_OBLIGATORIO" AndAlso {"EXPEDIENTE", "LOTE"}.Contains(configuracion.AmbitoIntegridad)
        If Not parcial AndAlso Not completo Then Throw New PerfilImportacionException("CONFIGURACION_INVALIDA", "La política de integridad no está disponible.")
        If completo AndAlso configuracion.AmbitoIntegridad = "LOTE" AndAlso resultado.Incidencias.Any(Function(i) i.BloqueaUnidad) Then Return
        For Each raiz In manifiesto.Raices
            Dim incidencias = resultado.Incidencias.Where(Function(i) i.BloqueaUnidad AndAlso i.RaizIdCliente = raiz.IdCliente).ToList()
            If incidencias.Any(Function(i) String.IsNullOrEmpty(i.ArchivoIdCliente)) Then Continue For
            Dim archivos = manifiesto.Archivos.Where(Function(a) a.RaizIdCliente = raiz.IdCliente).ToList()
            If archivos.Count = 0 Then Continue For
            If completo AndAlso incidencias.Count > 0 Then Continue For
            If parcial AndAlso Not archivos.Any(Function(a) Not incidencias.Any(Function(i) i.ArchivoIdCliente = a.IdCliente)) Then Continue For
            resultado.UnidadesElegibles.Add(raiz.IdCliente)
        Next
        resultado.PuedeConfirmar = resultado.UnidadesElegibles.Count > 0
    End Sub

    'Misma decision del plan confirmado, sin recalcular reglas/proveedores.
    Friend Shared Function EsArchivoElegible(plan As PlanImportacionCarpetas, archivo As ArchivoManifiestoImportacion) As Boolean
        Return plan.Resultado.UnidadesElegibles.Contains(archivo.RaizIdCliente) AndAlso Not plan.Resultado.Incidencias.Any(Function(i) i.BloqueaUnidad AndAlso (String.IsNullOrEmpty(i.RaizIdCliente) OrElse (i.RaizIdCliente = archivo.RaizIdCliente AndAlso (String.IsNullOrEmpty(i.ArchivoIdCliente) OrElse i.ArchivoIdCliente = archivo.IdCliente))))
    End Function

    Private Shared Sub AgregarIncidencias(resultado As ResultadoPreparacionImportacion, elemento As ValoresElementoImportacion, incidencias As List(Of IncidenciaPerfilImportacion))
        If incidencias Is Nothing Then Throw New PerfilImportacionException("CAPACIDAD_NO_DISPONIBLE", "No se pudieron comprobar los valores del plan.")
        For Each incidencia In incidencias
            resultado.Incidencias.Add(New IncidenciaImportacionCarpetas With {.Ambito = elemento.Ambito, .RaizIdCliente = elemento.RaizIdCliente, .ArchivoIdCliente = elemento.ArchivoIdCliente, .Campo = incidencia.Campo, .Codigo = incidencia.Codigo, .Mensaje = incidencia.Mensaje, .BloqueaUnidad = True})
        Next
    End Sub
End Class
