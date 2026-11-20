Option Strict On
Imports System
Imports System.Web

'Composicion de una fase interna, sin nuevo endpoint ni modificacion de config.
Public NotInheritable Class ComposicionCreacionImportacion
    Private Sub New()
    End Sub
    Public Shared Function Crear(http As HttpContext) As CoordinadorImportacionCarpetas
        If http Is Nothing OrElse http.Session Is Nothing OrElse Not Object.ReferenceEquals(http, HttpContext.Current) Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesión de Gestión vigente.")
        Dim limites = LimitesImportacionCarpetas.DesdeContexto(http)
        Dim autoridad As New ImportacionCarpetasContextAdapter()
        Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
        Dim repositorio As New MySqlImportacionCarpetasRepository(ra)
        Dim raiz = http.Server.MapPath("~/")
        Return New CoordinadorImportacionCarpetas(Function() autoridad.Resolver(http), repositorio,
            Function(nivel) autoridad.ResolverDestino(http, nivel, "CREAR_EXPEDIENTE"),
            Sub(contexto, configuracion)
                If Not Object.ReferenceEquals(http, HttpContext.Current) OrElse ra <> ModuleSessionConnectionStringResolver.Resolve(http, "RA_") OrElse da <> ModuleSessionConnectionStringResolver.Resolve(http, "DA_") Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El origen documental cambió.")
                autoridad.AutorizarRecursos(http, contexto, configuracion, "CREAR_EXPEDIENTE")
            End Sub,
            AddressOf repositorio.ObtenerBarreraCreacion, AddressOf repositorio.ObtenerArchivo, AddressOf repositorio.ObtenerIntentoCreacion,
            Function(lote, archivo) New AlmacenStagingImportacion(raiz, limites, lote, archivo),
            Function(conservar) New AdaptadorExpedienteImportacion(http, conservar), AddressOf repositorio.ObtenerVistaReconciliacion, AddressOf repositorio.ReanudarReservaCreacion,
            Sub(comando)
                autoridad.AutorizarCreacionLegacy(http, comando)
                Dim datos = New PerfilImportacionCatalogAdapter(ra, da).PrepararDatosCreacionLegacy(comando)
                comando.ValidarDatosLegacy(datos)
                autoridad.AutorizarCreacionLegacy(http, comando)
            End Sub)
    End Function
End Class
