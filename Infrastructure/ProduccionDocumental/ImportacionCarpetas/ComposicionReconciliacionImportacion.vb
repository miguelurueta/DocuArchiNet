Option Strict On
Option Compare Binary
Imports System
Imports System.Web

Public NotInheritable Class ComposicionReconciliacionImportacion
    Private Sub New()
    End Sub
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
