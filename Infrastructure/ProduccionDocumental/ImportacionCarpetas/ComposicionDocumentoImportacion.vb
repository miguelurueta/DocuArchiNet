Option Strict On
Imports System
Imports System.Web

Public NotInheritable Class ComposicionDocumentoImportacion
    Private Sub New()
    End Sub
    Public Shared Function Crear(http As HttpContext) As CoordinadorImportacionCarpetas
        If http Is Nothing OrElse http.Session Is Nothing OrElse Not Object.ReferenceEquals(http, HttpContext.Current) Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesion de Gestion vigente.")
        Dim limites = LimitesImportacionCarpetas.DesdeContexto(http)
        Dim autoridad As New ImportacionCarpetasContextAdapter()
        Dim repositorio As New MySqlImportacionCarpetasRepository(ModuleSessionConnectionStringResolver.Resolve(http, "RA_"))
        Dim raiz = http.Server.MapPath("~/")
        Return New CoordinadorImportacionCarpetas(Function() autoridad.Resolver(http), repositorio,
            Function(nivel) autoridad.ResolverDestino(http, nivel, "ALMACENAR_DOCUMENTO"),
            Sub(contexto, configuracion) autoridad.AutorizarRecursos(http, contexto, configuracion, "ALMACENAR_DOCUMENTO"),
            AddressOf repositorio.ObtenerBarreraDocumental, AddressOf repositorio.ObtenerArchivo, AddressOf repositorio.ObtenerIntentoAlmacenamiento,
            Function(lote, archivo) New AlmacenStagingImportacion(raiz, limites, lote, archivo),
            Function(almacen) New AdaptadorDocumentoImportacion(http, almacen))
    End Function
End Class
