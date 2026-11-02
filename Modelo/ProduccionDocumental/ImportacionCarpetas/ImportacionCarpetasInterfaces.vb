Option Strict On
Imports System.Collections.Generic
Imports System.IO

'Un solo repositorio implementa estos puertos. No se expone directamente a HTTP.
Public Interface IImportacionCarpetasRepository
    Sub VerificarEsquema()
    Function GuardarPlan(contexto As ContextoImportacionCarpetas, plan As PlanImportacionCarpetas) As PlanImportacionCarpetas
    Function ObtenerPlan(contexto As ContextoImportacionCarpetas, planId As String) As PlanImportacionCarpetas
    Function ObtenerContextoPlan(identidad As ContextoImportacionCarpetas, planId As String) As ContextoImportacionCarpetas
    Function ConfirmarPlan(contexto As ContextoImportacionCarpetas, planId As String, revisionEsperada As UInteger, planHashEsperado As String, claveConfirmacion As String) As LoteImportacionCarpetas
    Function ObtenerLote(contexto As ContextoImportacionCarpetas, loteId As String) As LoteImportacionCarpetas
    Function ObtenerArchivo(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String) As ArchivoImportacionCarpetas
    Function ReservarIntento(contexto As ContextoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas) As IntentoImportacionCarpetas
    Function ConfirmarRecepcion(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, intentoId As String, tokenReclamacion As String, recepcion As RecepcionVerificadaImportacion) As ArchivoImportacionCarpetas
    Function RegistrarResultado(contexto As ContextoImportacionCarpetas, intentoId As String, tokenReclamacion As String, resultado As ResultadoEfectoImportacion) As IntentoImportacionCarpetas
    Sub FijarValoresEfectivos(contexto As ContextoImportacionCarpetas, loteId As String, recursoTipo As String, recursoId As String, revisionEsperada As UInteger, valores As List(Of ValorReglaPerfilImportacion))
    Function SolicitarDetencion(contexto As ContextoImportacionCarpetas, loteId As String, revisionEsperada As UInteger) As LoteImportacionCarpetas
    Function ConsultarRaices(contexto As ContextoImportacionCarpetas, loteId As String, pagina As Integer, tamano As Integer) As PaginaRecursosImportacion(Of RaizImportacionCarpetas)
    Function ConsultarArchivos(contexto As ContextoImportacionCarpetas, loteId As String, pagina As Integer, tamano As Integer) As PaginaRecursosImportacion(Of ArchivoImportacionCarpetas)
    Function ConsultarEventos(contexto As ContextoImportacionCarpetas, loteId As String, pagina As Integer, tamano As Integer) As PaginaRecursosImportacion(Of EventoImportacionCarpetas)
End Interface

Public Interface IPoliticaLimitesImportacion
    Function Obtener(contexto As ContextoImportacionCarpetas) As LimitesImportacionCarpetas
End Interface

Public Interface IAlmacenStagingImportacion
    Function Recibir(contexto As ContextoImportacionCarpetas, intento As IntentoImportacionCarpetas, contenido As Stream, bytesPrevistos As String) As RecepcionVerificadaImportacion
    Function Verificar(contexto As ContextoImportacionCarpetas, intento As IntentoImportacionCarpetas) As RecepcionVerificadaImportacion
    Function VerificarRetenido(contexto As ContextoImportacionCarpetas) As RecepcionVerificadaImportacion
    Function ConservarVerificado(contexto As ContextoImportacionCarpetas) As IRetencionVerificadaImportacion
    Function VerificarReenvio(contexto As ContextoImportacionCarpetas, intento As IntentoImportacionCarpetas, contenido As Stream) As RecepcionVerificadaImportacion
    'Ruta interna consumible por legacy, nunca una ruta recibida o devuelta por HTTP.
    Function CrearCopiaTrabajo(contexto As ContextoImportacionCarpetas, stagingRef As String, nombreOriginal As String) As String
End Interface

'Mantiene abierto el original privado sin permitir escritura/borrado.
'No expone rutas ni un stream consumible a los adaptadores.
Public Interface IRetencionVerificadaImportacion
    Inherits System.IDisposable
    ReadOnly Property Recepcion As RecepcionVerificadaImportacion
End Interface

'Capacidad implementada por el mismo repositorio operativo; no otra familia
'de tablas. El puerto completo se incorpora por fases documentales.
Public Interface IRecepcionArchivosImportacionRepository
    Function ObtenerContextoLote(identidad As ContextoImportacionCarpetas, loteId As String) As ContextoImportacionCarpetas
    Function ObtenerLote(contexto As ContextoImportacionCarpetas, loteId As String) As LoteImportacionCarpetas
    Function ObtenerArchivo(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String) As ArchivoImportacionCarpetas
    Function ReservarIntento(contexto As ContextoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas) As IntentoImportacionCarpetas
    Function ObtenerIntentoRecepcion(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, intentoId As String) As IntentoImportacionCarpetas
    Function ConfirmarRecepcion(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, intentoId As String, token As String, recepcion As RecepcionVerificadaImportacion) As ArchivoImportacionCarpetas
    Sub RegistrarFalloRecepcion(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String, intentoId As String, token As String, codigo As String)
End Interface

'Capacidad del mismo repositorio; no invoca operaciones legacy.
Public Interface IControlLoteImportacionRepository
    Function ObtenerContextoLote(identidad As ContextoImportacionCarpetas, loteId As String) As ContextoImportacionCarpetas
    Function ObtenerLote(contexto As ContextoImportacionCarpetas, loteId As String) As LoteImportacionCarpetas
    Function ReservarIntento(contexto As ContextoImportacionCarpetas, solicitud As ReclamacionImportacionCarpetas) As IntentoImportacionCarpetas
    Function SolicitarDetencion(contexto As ContextoImportacionCarpetas, loteId As String, revisionEsperada As UInteger) As LoteImportacionCarpetas
End Interface

'Lecturas del mismo repositorio, sin capacidad de mutar/reconciliar recursos.
Public Interface IConsultaImportacionRepository
    Function ObtenerContextoPlan(identidad As ContextoImportacionCarpetas, planId As String) As ContextoImportacionCarpetas
    Function ConsultarConfirmacion(contexto As ContextoImportacionCarpetas, planId As String) As ConfirmacionLecturaImportacion
    Function ObtenerContextoLote(identidad As ContextoImportacionCarpetas, loteId As String) As ContextoImportacionCarpetas
    Function ConsultarEstado(contexto As ContextoImportacionCarpetas, loteId As String) As EstadoLecturaImportacion
    Function ConsultarRaices(contexto As ContextoImportacionCarpetas, loteId As String, filtro As FiltroConsultaImportacion) As PaginaRecursosImportacion(Of RaizImportacionCarpetas)
    Function ConsultarArchivos(contexto As ContextoImportacionCarpetas, loteId As String, filtro As FiltroConsultaImportacion) As PaginaRecursosImportacion(Of ArchivoImportacionCarpetas)
End Interface

'Puerto estrecho de la fase implementada. El lote aporta el snapshot y
'contexto persistidos; no se fabrica un lote a partir de parametros sueltos.
Public Interface ICreacionExpedienteImportacion
    Function Crear(contexto As ContextoImportacionCarpetas, lote As LoteImportacionCarpetas, raiz As RaizImportacionCarpetas, intento As IntentoImportacionCarpetas) As ResultadoEfectoImportacion
End Interface

Public Interface IAdaptadorExpedienteImportacion
    Inherits ICreacionExpedienteImportacion
    Function Verificar(contexto As ContextoImportacionCarpetas, intento As IntentoImportacionCarpetas) As ResultadoEfectoImportacion
End Interface

Public Interface IAlmacenamientoDocumentoImportacion
    Function Almacenar(contexto As ContextoImportacionCarpetas, raiz As RaizImportacionCarpetas, archivo As ArchivoImportacionCarpetas, snapshot As SnapshotPerfilImportacionOperativa, intento As IntentoImportacionCarpetas) As ResultadoEfectoImportacion
End Interface

Public Interface IAdaptadorDocumentoImportacion
    Inherits IAlmacenamientoDocumentoImportacion
    Function Verificar(contexto As ContextoImportacionCarpetas, intento As IntentoImportacionCarpetas, resultadoAnterior As ResultadoEfectoImportacion) As ResultadoEfectoImportacion
End Interface

'Mismo repositorio/tablas. Los callbacks son internos, nunca DTO/HTTP.
Public Interface ILimpiezaOriginalImportacionRepository
    Function ObtenerVistaLimpieza(contexto As ContextoImportacionCarpetas, loteId As String, archivoId As String) As VistaLimpiezaOriginalImportacion
    Function PrepararLimpieza(esperada As VistaLimpiezaOriginalImportacion, politica As PoliticaLimpiezaOriginalImportacion, comprobar As Action(Of VistaLimpiezaOriginalImportacion)) As VistaLimpiezaOriginalImportacion
    Function AplicarLimpieza(esperada As VistaLimpiezaOriginalImportacion, politica As PoliticaLimpiezaOriginalImportacion, comprobar As Action(Of VistaLimpiezaOriginalImportacion), eliminar As Func(Of Boolean)) As LimpiezaOriginalImportacionResultado
End Interface

Public Interface ILimpiezaStagingImportacion
    Function AbrirRetiro(contexto As ContextoImportacionCarpetas) As IRetiroOriginalImportacion
End Interface

Public Interface IRetiroOriginalImportacion
    Inherits System.IDisposable
    ReadOnly Property Existe As Boolean
    Function Eliminar() As Boolean
End Interface
