Option Strict On
Option Compare Binary
Imports System
Imports System.Web

Public Partial Class WebServiceImportacionCarpetas
    'Binding productivo separado del arnes: este si compila con los bindings legacy.
    Private Shared Function CrearCoordinacionOperativa(http As HttpContext, autoridad As ImportacionCarpetasContextAdapter, accion As String) As CoordinadorImportacionCarpetas
        Select Case accion
            Case "CREAR_EXPEDIENTE" : Return ComposicionCreacionImportacion.Crear(http)
            Case "MATERIALIZAR_VALORES" : Return ComposicionPerfilImportacionOperativa.CrearMaterializacion(http)
            Case "ALMACENAR" : Return ComposicionDocumentoImportacion.Crear(http)
            Case "RECONCILIAR" : Return ComposicionReconciliacionImportacion.Crear(http)
            Case "RECUPERAR_DOCUMENTO_V2" : Return ComposicionReconciliacionImportacion.CrearRecuperacionV2(http)
            Case "DETENER_LOTE"
                If http Is Nothing OrElse http.Session Is Nothing OrElse Not Object.ReferenceEquals(http, HttpContext.Current) Then Throw New PerfilImportacionException("SESION_REQUERIDA", "Se requiere una sesion vigente.")
                Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
                Dim repositorio As New MySqlImportacionCarpetasRepository(ra)
                Return New CoordinadorImportacionCarpetas(Function() autoridad.Resolver(http), repositorio,
                    Function(nivel) autoridad.ResolverDestino(http, nivel, "DETENER_LOTE"),
                    Sub(contexto, configuracion)
                        If Not Object.ReferenceEquals(http, HttpContext.Current) OrElse ra <> ModuleSessionConnectionStringResolver.Resolve(http, "RA_") OrElse da <> ModuleSessionConnectionStringResolver.Resolve(http, "DA_") Then Throw New PerfilImportacionException("ACCESO_DENEGADO", "El origen documental ha cambiado.")
                        autoridad.AutorizarRecursos(http, contexto, configuracion, "DETENER_LOTE")
                    End Sub)
            Case Else : Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La operacion no esta disponible.")
        End Select
    End Function
End Class
