Option Strict On
Option Compare Binary
Imports System
Imports System.Globalization
Imports System.Text
Imports System.Web
Imports System.Web.SessionState
Imports Newtonsoft.Json

'Transporte propio de un archivo. No modifica seleccion global ni llama a
'creacion, almacenamiento definitivo o coordinacion documental.
Public Class ImportacionCarpetasUpload
    Implements IHttpHandler, IRequiresSessionState
    Private ReadOnly _autoridad As ImportacionCarpetasContextAdapter
    Private ReadOnly _limites As Func(Of LimitesImportacionCarpetas)
    Private ReadOnly _servicio As Func(Of HttpContext, ImportacionCarpetasContextAdapter, LimitesImportacionCarpetas, ServicioRecepcionImportacion)

    Public Sub New()
        Me.New(New ImportacionCarpetasContextAdapter(), Function() LimitesImportacionCarpetas.DesdeContexto(HttpContext.Current), AddressOf CrearServicio)
    End Sub
    Friend Sub New(autoridad As ImportacionCarpetasContextAdapter, limites As Func(Of LimitesImportacionCarpetas), servicio As Func(Of HttpContext, ImportacionCarpetasContextAdapter, LimitesImportacionCarpetas, ServicioRecepcionImportacion))
        _autoridad = autoridad : _limites = limites : _servicio = servicio
    End Sub

    Public Sub ProcessRequest(http As HttpContext) Implements IHttpHandler.ProcessRequest
        If http Is Nothing Then Throw New ArgumentNullException("http")
        Dim salida As New RespuestaPerfilImportacion With {.operacionId = Guid.NewGuid().ToString("N")}
        Try
            Dim contexto = _autoridad.Resolver(http)
            Dim politica = _limites()
            Dim solicitud = ImportacionCarpetasRequestGuard.LeerMultipart(http, contexto, politica)
            salida.datos = _servicio(http, _autoridad, politica).Recibir(solicitud, http.Request.Files(0).InputStream)
            salida.ok = True : salida.codigo = "OK" : salida.mensaje = "Recepción verificada; consulte el estado documental por separado."
        Catch ex As PerfilImportacionException
            salida.codigo = ex.Codigo : salida.mensaje = ex.Message
        Catch ex As Exception
            salida.codigo = "RESULTADO_INCIERTO" : salida.mensaje = "La recepción no pudo comprobarse. Consulte su estado antes de repetir."
        End Try
        http.Response.ContentType = "application/json"
        http.Response.ContentEncoding = Encoding.UTF8
        http.Response.TrySkipIisCustomErrors = True
        http.Response.SuppressFormsAuthenticationRedirect = True
        http.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        http.Response.Cache.SetNoStore()
        http.Response.StatusCode = WebServiceImportacionCarpetas.EstadoHttp(salida)
        http.Response.Write(JsonConvert.SerializeObject(salida, New JsonSerializerSettings With {.TypeNameHandling = TypeNameHandling.None, .StringEscapeHandling = StringEscapeHandling.EscapeHtml, .Culture = CultureInfo.InvariantCulture}))
    End Sub

    Friend Shared Function CrearServicio(http As HttpContext, autoridad As ImportacionCarpetasContextAdapter, limites As LimitesImportacionCarpetas) As ServicioRecepcionImportacion
        Dim repositorio As New MySqlImportacionCarpetasRepository(ModuleSessionConnectionStringResolver.Resolve(http, "RA_"))
        Dim raiz = http.Server.MapPath("~/")
        Return New ServicioRecepcionImportacion(Function() autoridad.Resolver(http), repositorio,
            Function(nivel) autoridad.ResolverDestino(http, nivel, "RECIBIR_ARCHIVO"),
            Sub(contexto, configuracion) autoridad.AutorizarRecursos(http, contexto, configuracion, "RECIBIR_ARCHIVO"), limites,
            Function(lote, archivo) New AlmacenStagingImportacion(raiz, limites, lote, archivo))
    End Function
    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
End Class
