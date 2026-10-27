Option Strict On
Imports System
Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Services

<ScriptService(), WebService(Namespace:="http://tempuri.org/"), WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class WebServiceImportacionCarpetas
    Inherits WebService

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ListarPerfiles(solicitud As ListarPerfilesSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.Listar(c, New FiltroPerfilesImportacion With {.Pagina = r.pagina, .TamanoPagina = r.tamanoPagina, .Codigo = r.codigo, .Nombre = r.nombre, .Activo = r.activo, .Termino = r.termino}))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ObtenerPerfil(solicitud As ObtenerPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.Obtener(c, r.perfilId, r.versionId))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarHistorial(solicitud As HistorialPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.Historial(c, r.perfilId, r.pagina, r.tamanoPagina))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarCatalogo(solicitud As CatalogoPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) a.Consultar(c, r.catalogo, r.configuracion, r.pagina, r.tamanoPagina, r.referencia))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarProveedores(solicitud As CatalogoProveedoresSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.ListarProveedores(c, r.pagina, r.tamanoPagina, r.proveedorId, r.versionId))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarMetadatosProveedor(solicitud As MetadatosProveedorSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) MetadatosProveedor(c, r))
    End Function
    Private Function MetadatosProveedor(c As ContextoPerfilImportacion, r As MetadatosProveedorSolicitud) As Object
        Dim http = HttpContext.Current
        Dim autorizacion As New PerfilImportacionContextAdapter()
        autorizacion.AutorizarProveedores(http, c)
        Dim politica = PoliticaActual(c)
        Dim consulta As New MetadatosProveedorIndices(politica, Function(aliasOrigen) If(aliasOrigen = "RA", ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), If(aliasOrigen = "DA", ModuleSessionConnectionStringResolver.Resolve(http, "DA_"), Nothing)))
        Return consulta.Consultar(c, r.tipo, r.conexionRef, r.tabla, r.pagina)
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function CrearPerfil(solicitud As CrearPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.Crear(c, r.codigo, r.nombre, r.descripcion))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function GuardarBorrador(solicitud As GuardarPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.Guardar(c, r.perfilId, Version(r)))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function EnsayarReglas(solicitud As EnsayarPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.Ensayar(c, r.perfilId, Version(r), r.ejemplos))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function PublicarVersion(solicitud As PublicarPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.Publicar(c, r.perfilId, r.versionId, r.revisionPerfilEsperada, r.revisionVersionEsperada))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function CrearVersion(solicitud As CrearVersionPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.CrearVersion(c, r.perfilId, r.versionOrigenId, r.revisionPerfilEsperada, r.revisionOrigenEsperada))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ActualizarCabecera(solicitud As CabeceraPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.ActualizarCabecera(c, r.perfilId, r.revisionPerfilEsperada, r.nombre, r.descripcion))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function CambiarDisponibilidad(solicitud As DisponibilidadPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) s.CambiarDisponibilidad(c, r.perfilId, r.revisionPerfilEsperada, r.activo))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ValidarSeleccionPerfil(solicitud As SeleccionPerfilSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) New With {.nivelRef = r.nivelRef, .perfil = s.Seleccionar(c, r.perfilId, r.versionId, r.nivelRef, r.proposito)})
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function CambiarDisponibilidadProveedor(solicitud As SolicitudProveedorAdmin) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarProveedor(c, "DISPONIBILIDAD", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function CrearProveedor(solicitud As SolicitudProveedorAdmin) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarProveedor(c, "CREAR", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function CrearVersionProveedor(solicitud As SolicitudProveedorAdmin) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarProveedor(c, "VERSION", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function EnsayarProveedor(solicitud As SolicitudProveedorAdmin) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarProveedor(c, "ENSAYAR", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function GuardarProveedor(solicitud As SolicitudProveedorAdmin) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarProveedor(c, "GUARDAR", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ListarAdministracionProveedores(solicitud As SolicitudProveedorAdmin) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarProveedor(c, "LISTAR", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ObtenerAdministracionProveedor(solicitud As SolicitudProveedorAdmin) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarProveedor(c, "OBTENER", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function PublicarProveedor(solicitud As SolicitudProveedorAdmin) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarProveedor(c, "PUBLICAR", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ValidarProveedor(solicitud As SolicitudProveedorAdmin) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarProveedor(c, "VALIDAR", r))
    End Function
    Private Function PoliticaActual(c As ContextoPerfilImportacion) As PoliticaFuentesProveedor
        Return New MySqlFuenteImportacionRepository(ModuleSessionConnectionStringResolver.Resolve(HttpContext.Current, "RA_")).CargarPolitica(c, RestriccionTablas())
    End Function
    Private Function RestriccionTablas() As RestriccionTablasFuente
        Return New RestriccionTablasFuente(System.Configuration.ConfigurationManager.AppSettings("Doc110.Fuentes.TablasAdicionales"))
    End Function
    Private Function MetadatosFuente(c As ContextoPerfilImportacion) As MetadatosFuenteImportacion
        Dim http = HttpContext.Current
        Return New MetadatosFuenteImportacion(Function(aliasOrigen) If(aliasOrigen = "RA", ModuleSessionConnectionStringResolver.Resolve(http, "RA_"), If(aliasOrigen = "DA", ModuleSessionConnectionStringResolver.Resolve(http, "DA_"), Nothing)), RestriccionTablas())
    End Function
    Private Function AdministrarFuente(c As ContextoPerfilImportacion, accion As String, r As SolicitudFuenteImportacion) As Object
        Dim autorizacion As New PerfilImportacionContextAdapter()
        autorizacion.AutorizarFuentes(HttpContext.Current, c)
        Dim meta = MetadatosFuente(c)
        Dim servicio As New ServicioFuenteImportacion(New MySqlFuenteImportacionRepository(ModuleSessionConnectionStringResolver.Resolve(HttpContext.Current, "RA_")), AddressOf meta.Validar)
        Return servicio.Ejecutar(c, accion, r)
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarMetadatosFuente(solicitud As MetadatosProveedorSolicitud) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r)
                                      Dim autorizacion As New PerfilImportacionContextAdapter()
                                      autorizacion.AutorizarFuentes(HttpContext.Current, c)
                                      Return MetadatosFuente(c).Consultar(c, r.tipo, r.conexionRef, r.tabla, r.pagina)
                                  End Function)
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ListarFuentes(solicitud As SolicitudFuenteImportacion) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarFuente(c, "LISTAR", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ObtenerFuente(solicitud As SolicitudFuenteImportacion) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarFuente(c, "OBTENER", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function HistorialFuente(solicitud As SolicitudFuenteImportacion) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarFuente(c, "HISTORIAL", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function CrearFuente(solicitud As SolicitudFuenteImportacion) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarFuente(c, "ALTA", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function GuardarFuente(solicitud As SolicitudFuenteImportacion) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarFuente(c, "EDITAR", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ActivarFuente(solicitud As SolicitudFuenteImportacion) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarFuente(c, "ACTIVAR", r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function RevocarFuente(solicitud As SolicitudFuenteImportacion) As RespuestaPerfilImportacion
        Return Ejecutar(solicitud, Function(c, s, a, r) AdministrarFuente(c, "REVOCAR", r))
    End Function
    Private Function AdministrarProveedor(c As ContextoPerfilImportacion, accion As String, r As SolicitudProveedorAdmin) As Object
        Dim http = HttpContext.Current
        Dim autorizacion As New PerfilImportacionContextAdapter()
        autorizacion.AutorizarProveedores(http, c)
        Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_")
        Dim da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
        Dim conexiones As Func(Of String, String) = Function(aliasOrigen) If(aliasOrigen = "RA", ra, If(aliasOrigen = "DA", da, Nothing))
        Dim politica = PoliticaActual(c)
        Dim meta As New MetadatosProveedorIndices(politica, conexiones)
        Dim validador As New ValidadorAdministracionProveedor(politica, meta)
        Dim consulta As New MySqlProveedorDatosIndices(conexiones, Nothing, AddressOf politica.Permite)
        Dim servicio As New ServicioAdministracionProveedor(New MySqlAdministracionProveedorRepository(ra), AddressOf validador.Validar, consulta)
        Return servicio.Ejecutar(c, accion, r)
    End Function
    Private Function Ejecutar(Of T As Class)(solicitud As T, accion As Func(Of ContextoPerfilImportacion, ServicioPerfilImportacion, PerfilImportacionCatalogAdapter, T, Object)) As RespuestaPerfilImportacion
        Dim salida As New RespuestaPerfilImportacion With {.operacionId = Guid.NewGuid().ToString("N")}
        Dim http = HttpContext.Current
        Try
            Dim contexto = New PerfilImportacionContextAdapter().Resolver(http)
            PerfilImportacionRequestGuard.Validar(http, contexto)
            If solicitud Is Nothing Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La solicitud es obligatoria.")
            'El runtime ASMX puede omitir propiedades desconocidas. Volver a comprobar el cuerpo
            'cerrado y usar esa representación, sin aceptar coerciones o claves duplicadas.
            Dim stream = http.Request.InputStream
            If Not stream.CanSeek Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "No se pudo comprobar la solicitud.")
            stream.Position = 0
            Dim texto As String
            Using reader As New StreamReader(stream, Text.Encoding.UTF8, True, 1024, True)
                texto = reader.ReadToEnd()
            End Using
            Dim sobre = PerfilImportacionJson.Leer(Of SobreSolicitudPerfil(Of T))(texto)
            If sobre.solicitud Is Nothing Then Throw New PerfilImportacionException("SOLICITUD_INVALIDA", "La solicitud es obligatoria.")
            Dim ra = ModuleSessionConnectionStringResolver.Resolve(http, "RA_")
            Dim da = ModuleSessionConnectionStringResolver.Resolve(http, "DA_")
            Dim catalogos As New PerfilImportacionCatalogAdapter(ra, da)
            'Alias cerrados administrados por servidor. No se acepta un prefijo de sesion del cliente.
            Dim politica As New Lazy(Of PoliticaFuentesProveedor)(Function() PoliticaActual(contexto))
            Dim proveedor As New MySqlProveedorDatosIndices(Function(aliasOrigen) If(aliasOrigen = "RA", ra, If(aliasOrigen = "DA", da, Nothing)), Nothing,
                Function(c, v) politica.Value.Permite(c, v))
            Dim coordinador As New CoordinadorProveedorIndices(New MySqlProveedorIndicesRepository(ra), proveedor)
            Dim servicio As New ServicioPerfilImportacion(New MySqlPerfilImportacionRepository(ra), catalogos, coordinador)
            salida.datos = accion(contexto, servicio, catalogos, sobre.solicitud)
            salida.ok = True
            salida.codigo = "OK"
            salida.mensaje = "Operación completada."
        Catch ex As ConfiguracionPerfilImportacionException
            salida.codigo = ex.Codigo
            salida.mensaje = ex.Message
            salida.incidencias = ex.Incidencias
        Catch ex As PerfilImportacionException
            salida.codigo = ex.Codigo
            salida.mensaje = ex.Message
        Catch ex As Exception
            salida.codigo = "ERROR_INTERNO"
            salida.mensaje = "No se confirmó la operación. Consulte su estado antes de repetir."
        End Try
        If http IsNot Nothing Then
            http.Response.TrySkipIisCustomErrors = True
            http.Response.SuppressFormsAuthenticationRedirect = True
            http.Response.Cache.SetCacheability(HttpCacheability.NoCache)
            http.Response.Cache.SetNoStore()
            http.Response.StatusCode = EstadoHttp(salida)
        End If
        Return salida
    End Function
    Private Shared Function Version(solicitud As GuardarPerfilSolicitud) As VersionPerfilImportacion
        Return New VersionPerfilImportacion With {.PerfilId = solicitud.perfilId, .VersionId = solicitud.versionId, .Revision = solicitud.revisionVersionEsperada, .Configuracion = solicitud.configuracion, .Reglas = solicitud.reglas}
    End Function
    Private Shared Function EstadoHttp(resultado As RespuestaPerfilImportacion) As Integer
        If resultado.ok Then Return 200
        Select Case resultado.codigo
            Case "SESION_REQUERIDA" : Return 401
            Case "ACCESO_DENEGADO", "SOLICITUD_NO_VERIFICADA" : Return 403
            Case "CONFLICTO_REVISION", "CODIGO_PERFIL_EXISTENTE" : Return 409
            Case "CAPACIDAD_NO_DISPONIBLE", "ESQUEMA_FUENTES_INCOMPLETO" : Return 503
            Case "ERROR_INTERNO" : Return 500
            Case Else : Return 400
        End Select
    End Function
End Class
