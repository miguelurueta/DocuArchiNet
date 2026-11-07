Option Strict On
Option Compare Binary
Imports System
Imports System.Linq
Imports System.Web

Public NotInheritable Class VerificadorLimpiezaOriginalImportacion
    Private ReadOnly _datos As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion)
    Private ReadOnly _comprobar As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, ResultadoEfectoImportacion, Boolean)
    Public Sub New(catalogos As PerfilImportacionCatalogAdapter)
        Me.New(AddressOf catalogos.PrepararDatosAlmacenamiento, AddressOf catalogos.ComprobarAlmacenamiento)
    End Sub
    Friend Sub New(datos As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion), comprobar As Func(Of ComandoAlmacenamientoCarpetas, DatosAlmacenamientoImportacion, ResultadoEfectoImportacion, Boolean))
        If datos Is Nothing OrElse comprobar Is Nothing Then Throw New ArgumentNullException()
        _datos = datos : _comprobar = comprobar
    End Sub
    Public Function Verificar(vista As VistaLimpiezaOriginalImportacion) As Boolean
        Dim v = vista.Inventario, archivo = v.Archivos.Single(Function(f) f.ArchivoId = vista.ArchivoId)
        Dim raiz = v.Raices.Single(Function(r) r.RaizId = archivo.RaizId)
        Dim intento = v.Intentos.Single(Function(i) i.ArchivoId = archivo.ArchivoId AndAlso i.Fase = "ALMACENAR" AndAlso i.Estado = "CONFIRMADO")
        Dim comando = ComandoAlmacenamientoCarpetas.PrepararComprobacionFinal(v.Lote.Contexto, raiz, archivo, v.Lote.Snapshot, intento)
        Return _comprobar(comando, _datos(comando), intento.Resultado)
    End Function
End Class

'Composicion interna. No cambia Web.config, publica endpoints o programa jobs.
Public NotInheritable Class ComposicionLimpiezaOriginalImportacion
    Private Sub New()
    End Sub
    Public Shared Function Crear(http As HttpContext, Optional politica As Func(Of PoliticaLimpiezaOriginalImportacion) = Nothing) As ServicioConsultaImportacion
        If http Is Nothing OrElse http.Session Is Nothing OrElse Not Object.ReferenceEquals(http, HttpContext.Current) Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesion vigente.")
        Dim politicaActual = If(politica, Function() New PoliticaLimpiezaOriginalImportacion(False, TimeSpan.Zero))
        politicaActual().Exigir()
        Dim limites = LimitesImportacionCarpetas.DesdeContexto(http)
        Dim autoridad As New ImportacionCarpetasContextAdapter()
        Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
        Dim repositorio As New MySqlImportacionCarpetasRepository(ra), catalogos As New PerfilImportacionCatalogAdapter(ra, da)
        Dim verificador As New VerificadorLimpiezaOriginalImportacion(catalogos), raiz = http.Server.MapPath("~/")
        Return New ServicioConsultaImportacion(Function() autoridad.Resolver(http), repositorio,
            Function(nivel) autoridad.ResolverDestino(http, nivel, "LIMPIAR_STAGING"),
            Sub(contexto, configuracion)
                If Not Object.ReferenceEquals(http, HttpContext.Current) OrElse ra <> ModuleSessionConnectionStringResolver.Resolve(http, "RA_") OrElse da <> ModuleSessionConnectionStringResolver.Resolve(http, "DA_") Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El origen documental ha cambiado.")
                autoridad.AutorizarRecursos(http, contexto, configuracion, "LIMPIAR_STAGING")
            End Sub, ServicioConsultaImportacion.MaximoPagina,
            politicaActual, repositorio, AddressOf verificador.Verificar,
            Function(lote, archivo) New AlmacenStagingImportacion(raiz, limites, lote, archivo))
    End Function
End Class
