Option Strict On
Option Compare Binary
Imports System
Imports System.Web

Public NotInheritable Class ComposicionReconciliacionImportacion
    Private Sub New()
    End Sub

    Friend Shared Function CrearRecuperacionV2(http As HttpContext) As CoordinadorImportacionCarpetas
        If http Is Nothing OrElse http.Session Is Nothing OrElse Not Object.ReferenceEquals(http, HttpContext.Current) Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesion de Gestion vigente.")
        Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
        Dim validarOrigen = GuardiaOrigenRecuperacionV2(http, ra, da)
        Dim autoridad As New ImportacionCarpetasContextAdapter()
        Dim limites = LimitesImportacionCarpetas.DesdeContexto(http)
        Dim raiz = http.Server.MapPath("~/")
        Dim repositorio As New MySqlImportacionCarpetasRepository(ra)
        Dim catalogos As New PerfilImportacionCatalogAdapter(ra, da)
        Dim autorizar As Action(Of ContextoImportacionCarpetas, ConfiguracionPerfilImportacion) =
            Sub(contexto, configuracion)
                validarOrigen()
                autoridad.AutorizarRecursos(http, contexto, configuracion, "RECONCILIAR")
            End Sub
        Dim puerto = repositorio.CrearRecuperacionDocumentoV2(Function(lote, archivo) New AlmacenStagingImportacion(raiz, limites, lote, archivo), catalogos, autorizar)
        Return New CoordinadorImportacionCarpetas(
            Function()
                validarOrigen()
                Return autoridad.Resolver(http)
            End Function, repositorio,
            Function(nivel)
                validarOrigen()
                Return autoridad.ResolverDestino(http, nivel, "RECONCILIAR")
            End Function, autorizar, puerto)
    End Function

    'Captura exclusivamente en memoria los orígenes de esta petición.
    Friend Shared Function GuardiaOrigenRecuperacionV2(http As HttpContext, ra As String, da As String) As Action
        Dim validar As Action = Sub()
                                    If http Is Nothing OrElse http.Session Is Nothing OrElse Not Object.ReferenceEquals(http, HttpContext.Current) Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesion de Gestion vigente.")
                                    If String.IsNullOrWhiteSpace(ra) OrElse String.IsNullOrWhiteSpace(da) OrElse ra <> ModuleSessionConnectionStringResolver.Resolve(http, "RA_") OrElse da <> ModuleSessionConnectionStringResolver.Resolve(http, "DA_") Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El origen documental ha cambiado.")
                                End Sub
        validar()
        Return validar
    End Function

    Public Shared Function Crear(http As HttpContext) As CoordinadorImportacionCarpetas
        If http Is Nothing OrElse http.Session Is Nothing OrElse Not Object.ReferenceEquals(http, HttpContext.Current) Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesion de Gestion vigente.")
        Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
        Dim autoridad As New ImportacionCarpetasContextAdapter()
        Dim repositorio As New MySqlImportacionCarpetasRepository(ra)
        Dim catalogos As New PerfilImportacionCatalogAdapter(ra, da)
        Dim verificador As New VerificadorReconciliacionImportacion(catalogos, repositorio)
        Return New CoordinadorImportacionCarpetas(Function() autoridad.Resolver(http), repositorio,
            Function(nivel) autoridad.ResolverDestino(http, nivel, "RECONCILIAR"),
            Sub(contexto, configuracion)
                If Not Object.ReferenceEquals(http, HttpContext.Current) OrElse ra <> ModuleSessionConnectionStringResolver.Resolve(http, "RA_") OrElse da <> ModuleSessionConnectionStringResolver.Resolve(http, "DA_") Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El origen documental ha cambiado.")
                autoridad.AutorizarRecursos(http, contexto, configuracion, "RECONCILIAR")
            End Sub,
            AddressOf repositorio.ObtenerVistaReconciliacion, AddressOf verificador.Verificar, AddressOf repositorio.GuardarReconciliacion, AddressOf repositorio.CerrarReservaAlmacenamientoSinInicio)
    End Function
End Class
