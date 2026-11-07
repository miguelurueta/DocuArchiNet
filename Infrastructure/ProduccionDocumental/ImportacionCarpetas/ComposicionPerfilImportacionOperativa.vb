Option Strict On
Imports System
Imports System.Web

'La seleccion/preflight y la materializacion comparten exactamente los
'catalogos, publicaciones y politica operativa de fuentes ya existentes.
Public NotInheritable Class ComposicionPerfilImportacionOperativa
    Private Sub New()
    End Sub
    Public Shared Function CrearSeleccion(http As HttpContext, autoridad As ImportacionCarpetasContextAdapter, ByRef catalogos As PerfilImportacionCatalogAdapter, ByRef coordinador As CoordinadorProveedorIndices) As AdaptadorPerfilImportacionOperativa
        Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_")
        Dim da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
        catalogos = New PerfilImportacionCatalogAdapter(ra, da)
        Dim politica As New Lazy(Of PoliticaFuentesProveedor)(Function() New MySqlFuenteImportacionRepository(ra).CargarPoliticaOperativa(autoridad.Resolver(http), New RestriccionTablasFuente(System.Configuration.ConfigurationManager.AppSettings("Doc110.Fuentes.TablasAdicionales"))))
        Dim proveedor As New MySqlProveedorDatosIndices(Function(aliasOrigen) If(aliasOrigen = "RA", ra, If(aliasOrigen = "DA", da, Nothing)), Nothing, Nothing,
                                                       Function(c, v) politica.Value.PermiteOperativa(c, v))
        coordinador = New CoordinadorProveedorIndices(New MySqlProveedorIndicesRepository(ra), proveedor)
        Return New AdaptadorPerfilImportacionOperativa(New MySqlPerfilImportacionRepository(ra), catalogos, AddressOf coordinador.ValidarOperativo)
    End Function

    'Fase interna independiente: no invoca almacenamiento ni crea otra raiz.
    Public Shared Function CrearMaterializacion(http As HttpContext) As CoordinadorImportacionCarpetas
        If http Is Nothing OrElse http.Session Is Nothing OrElse Not Object.ReferenceEquals(http, HttpContext.Current) Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesion de Gestion vigente.")
        Dim autoridad As New ImportacionCarpetasContextAdapter()
        Dim catalogos As PerfilImportacionCatalogAdapter = Nothing, resolvedor As CoordinadorProveedorIndices = Nothing
        CrearSeleccion(http, autoridad, catalogos, resolvedor)
        Dim repositorio As New MySqlImportacionCarpetasRepository(ModuleSessionConnectionStringResolver.Resolve(http, "RA_"))
        Return New CoordinadorImportacionCarpetas(Function() autoridad.Resolver(http), repositorio,
            Function(nivel) autoridad.ResolverDestino(http, nivel, "ALMACENAR_DOCUMENTO"),
            Sub(contexto, configuracion) autoridad.AutorizarRecursos(http, contexto, configuracion, "ALMACENAR_DOCUMENTO"),
            AddressOf repositorio.ObtenerVistaValores, resolvedor, catalogos, AddressOf repositorio.FijarValoresArchivo)
    End Function
End Class
