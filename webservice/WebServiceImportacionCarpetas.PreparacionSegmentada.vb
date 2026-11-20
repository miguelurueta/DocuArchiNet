Option Strict On
Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Services

Public Partial Class WebServiceImportacionCarpetas
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarRecursoLote(solicitud As ConsultarRecursoLoteSolicitud) As RespuestaPerfilImportacion
        Return EjecutarEntradaOperativa(solicitud, Function(http, limites, r) _consultaOperativa(http, _autoridadOperativa).ConsultarFocal(r))
    End Function
    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function IniciarPreparacionPlan(solicitud As IniciarPreparacionPlanSolicitud) As RespuestaPerfilImportacion
        Return EjecutarEntradaOperativa(solicitud, Function(http, limites, r) CrearPreparacionSegmentada(http, limites).Iniciar(r))
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function RegistrarPartePreparacion(solicitud As RegistrarPartePreparacionSolicitud) As RespuestaPerfilImportacion
        Return EjecutarEntradaOperativa(solicitud, Function(http, limites, r) CrearPreparacionSegmentada(http, limites).Registrar(r))
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarPreparacionPlan(solicitud As ConsultarPreparacionPlanSolicitud) As RespuestaPerfilImportacion
        Return EjecutarEntradaOperativa(solicitud, Function(http, limites, r) CrearPreparacionSegmentada(http, limites).Consultar(r))
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ValidarPartePreparacion(solicitud As ValidarPartePreparacionSolicitud) As RespuestaPerfilImportacion
        Return EjecutarEntradaOperativa(solicitud, Function(http, limites, r) CrearPreparacionSegmentada(http, limites).Validar(r))
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function SellarPreparacionPlan(solicitud As SellarPreparacionPlanSolicitud) As RespuestaPerfilImportacion
        Return EjecutarEntradaOperativa(solicitud, Function(http, limites, r) CrearPreparacionSegmentada(http, limites).Sellar(r))
    End Function

    <WebMethod(EnableSession:=True), ScriptMethod(UseHttpGet:=False, ResponseFormat:=ResponseFormat.Json)>
    Public Function ConsultarRevisionPlan(solicitud As ConsultarRevisionPlanSolicitud) As RespuestaPerfilImportacion
        Return EjecutarEntradaOperativa(solicitud, Function(http, limites, r) CrearPreparacionSegmentada(http, limites).Revision(r))
    End Function

    Private Function CrearPreparacionSegmentada(http As HttpContext, limites As LimitesImportacionCarpetas) As ServicioPreparacionSegmentadaImportacion
        Dim catalogos As PerfilImportacionCatalogAdapter = Nothing, coordinador As CoordinadorProveedorIndices = Nothing
        Dim perfiles = CrearSeleccionOperativa(http, _autoridadOperativa, catalogos, coordinador)
        Dim repositorio As New MySqlImportacionCarpetasRepository(ModuleSessionConnectionStringResolver.Resolve(http, "RA_"))
        Dim calculo As New ServicioPreparacionImportacion(Function(nivel) _autoridadOperativa.ResolverDestino(http, nivel, "PREPARAR"), perfiles, limites, AddressOf repositorio.VerificarEsquema, catalogos, coordinador, AddressOf repositorio.ComprobarCoincidencias)
        Return New ServicioPreparacionSegmentadaImportacion(Function() _autoridadOperativa.Resolver(http), Function(nivel) _autoridadOperativa.ResolverDestino(http, nivel, "PREPARAR"),
            Sub(contexto, configuracion) _autoridadOperativa.AutorizarRecursos(http, contexto, configuracion, "CONFIRMAR"), repositorio, perfiles, calculo, limites)
    End Function
End Class
