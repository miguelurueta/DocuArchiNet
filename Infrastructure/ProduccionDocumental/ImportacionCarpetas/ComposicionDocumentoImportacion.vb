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
        Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
        Dim validarOrigen = ComposicionReconciliacionImportacion.GuardiaOrigenRecuperacionV2(http, ra, da)
        Dim repositorio As New MySqlImportacionCarpetasRepository(ra)
        Dim raiz = http.Server.MapPath("~/")
        Return New CoordinadorImportacionCarpetas(
            Function()
                validarOrigen()
                Return autoridad.Resolver(http)
            End Function, repositorio,
            Function(nivel)
                validarOrigen()
                Return autoridad.ResolverDestino(http, nivel, "ALMACENAR_DOCUMENTO")
            End Function,
            Sub(contexto, configuracion)
                validarOrigen()
                autoridad.AutorizarRecursos(http, contexto, configuracion, "ALMACENAR_DOCUMENTO")
            End Sub,
            AddressOf repositorio.ObtenerBarreraDocumental, AddressOf repositorio.ObtenerArchivo, AddressOf repositorio.ObtenerIntentoAlmacenamiento,
            Function(lote, archivo) New AlmacenStagingImportacion(raiz, limites, lote, archivo),
            Function(almacen) AdaptadorDocumentoImportacion.EnlazarV2(New AdaptadorDocumentoImportacion(http, almacen), repositorio.CrearPersistenciaDocumentoV2(), AddressOf repositorio.ObtenerLote))
    End Function
End Class
