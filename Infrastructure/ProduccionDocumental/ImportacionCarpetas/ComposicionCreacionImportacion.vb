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
        Dim repositorio As New MySqlImportacionCarpetasRepository(ModuleSessionConnectionStringResolver.Resolve(http, "RA_"))
        Dim raiz = http.Server.MapPath("~/")
        Return New CoordinadorImportacionCarpetas(Function() autoridad.Resolver(http), repositorio,
            Function(nivel) autoridad.ResolverDestino(http, nivel, "CREAR_EXPEDIENTE"),
            Sub(contexto, configuracion) autoridad.AutorizarRecursos(http, contexto, configuracion, "CREAR_EXPEDIENTE"),
            AddressOf repositorio.ObtenerBarreraCreacion, AddressOf repositorio.ObtenerArchivo, AddressOf repositorio.ObtenerIntentoCreacion,
            Function(lote, archivo) New AlmacenStagingImportacion(raiz, limites, lote, archivo),
            Function(conservar) New AdaptadorExpedienteImportacion(http, conservar))
    End Function
End Class
