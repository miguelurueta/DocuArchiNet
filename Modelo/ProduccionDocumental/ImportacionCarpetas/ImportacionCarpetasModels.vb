Option Strict On
Imports System
Imports System.Collections.Generic

'Contratos operativos DOC-111. El contexto lo construye el servidor; no hereda
'ContextoPerfilImportacion ni representa al importador como administrador.
Public Class VistaLimpiezaOriginalImportacion
    Public Property Inventario As VistaConsolidacionImportacion
    Public Property ArchivoId As String
    Public Property Autorizacion As AutorizacionLimpiezaOriginalImportacion
    Public Property CodigoFinal As String
End Class

Public Class AutorizacionLimpiezaOriginalImportacion
    Public Property LoteId As String
    Public Property ArchivoId As String
    Public Property HuellaInventario As String
    Public Property RetencionTicks As Long
    Public Property PreparadaUtc As String
End Class

Public NotInheritable Class ContextoImportacionCarpetas
    Public Property UsuarioRef As String
    Public Property AmbitoRef As String
    Public Property NivelRef As String
    Public Property RutaCanonica As String
    Public Property SelloDestino As String
    Public Property PuedeImportar As Boolean
End Class

Public Class ManifiestoImportacionCarpetas
    Public Property Raices As New List(Of RaizManifiestoImportacion)()
    Public Property Archivos As New List(Of ArchivoManifiestoImportacion)()
End Class

Public Class RaizManifiestoImportacion
    Public Property SeleccionId As String
    Public Property IdCliente As String
    Public Property Nombre As String
End Class

Public Class ArchivoManifiestoImportacion
    Public Property IdCliente As String
    Public Property RaizIdCliente As String
    Public Property Nombre As String
    Public Property RutaRelativa As String
    'String decimal: JSON/JavaScript no debe redondear BIGINT UNSIGNED.
    Public Property Bytes As String
    Public Property UltimaModificacionLocal As String
End Class

Public Class SnapshotPerfilImportacionOperativa
    Public Property PerfilId As String
    Public Property VersionId As String
    Public Property NumeroVersion As UInteger
    Public Property Huella As String
    Public Property Configuracion As ConfiguracionPerfilImportacion
    Public Property Reglas As New List(Of ReglaPerfilImportacion)()
End Class

Public Class IncidenciaImportacionCarpetas
    Public Property Ambito As String
    Public Property RaizIdCliente As String
    Public Property ArchivoIdCliente As String
    Public Property Campo As String
    Public Property Codigo As String
    Public Property Mensaje As String
    Public Property BloqueaUnidad As Boolean
End Class

Public Class ValoresElementoImportacion
    Public Property Ambito As String
    Public Property RaizIdCliente As String
    Public Property ArchivoIdCliente As String
    'Reutiliza valor/procedencia/diferido de DOC-110; no fabrica IDs futuros.
    Public Property Valores As New List(Of ValorReglaPerfilImportacion)()
End Class

Public Class ResultadoPreparacionImportacion
    Public Property PuedeConfirmar As Boolean
    Public Property UnidadesElegibles As New List(Of String)()
    Public Property Elementos As New List(Of ValoresElementoImportacion)()
    Public Property Incidencias As New List(Of IncidenciaImportacionCarpetas)()
End Class

Public Class PlanImportacionCarpetas
    Public Property PlanId As String
    Public Property Contexto As ContextoImportacionCarpetas
    Public Property Snapshot As SnapshotPerfilImportacionOperativa
    Public Property Manifiesto As ManifiestoImportacionCarpetas
    Public Property Resultado As ResultadoPreparacionImportacion
    Public Property Estado As String
    Public Property Revision As UInteger
    Public Property EsquemaDoc As UShort
    Public Property Huella As String
    Public Property CreadoUtc As String
    Public Property ExpiraUtc As String
End Class

Public Class LoteImportacionCarpetas
    Public Property LoteId As String
    Public Property PlanId As String
    Public Property Contexto As ContextoImportacionCarpetas
    Public Property Snapshot As SnapshotPerfilImportacionOperativa
    Public Property ClaveConfirmacion As String
    Public Property SolicitudHash As String
    Public Property Estado As String
    Public Property DetencionSolicitada As Boolean
    Public Property TotalRaices As UInteger
    Public Property TotalArchivos As UInteger
    Public Property TotalBytes As String
    Public Property Revision As UInteger
    Public Property CreadoUtc As String
    Public Property ActualizadoUtc As String
End Class

Public Class RaizImportacionCarpetas
    'Pista de consulta, nunca persistida ni sustituto de la barrera de escritura.
    <Newtonsoft.Json.JsonIgnore>
    Public Property RecepcionListaParaCrear As Boolean
    <Newtonsoft.Json.JsonIgnore>
    Public Property PuedeRetomarCreacion As Boolean
    Public Property RaizId As String
    Public Property LoteId As String
    Public Property ClienteRaizId As String
    Public Property NombreOriginal As String
    Public Property Estado As String
    Public Property IdentidadExpediente As IdentidadExpedienteImportacion
    Public Property ValoresPlan As New List(Of ValorReglaPerfilImportacion)()
    Public Property ValoresEfectivos As New List(Of ValorReglaPerfilImportacion)()
    Public Property IntentoActivoId As String
    Public Property Revision As UInteger
End Class

Public Class ArchivoImportacionCarpetas
    Public Property ArchivoId As String
    Public Property RaizId As String
    Public Property ClienteArchivoId As String
    Public Property NombreOriginal As String
    Public Property RutaRelativa As String
    Public Property RutaHash As String
    Public Property UltimaModificacionLocal As String
    Public Property BytesPrevistos As String
    Public Property BytesRecibidos As String
    Public Property ContenidoHash As String
    Public Property StagingRef As String
    Public Property EstadoRecepcion As String
    Public Property EstadoEfecto As String
    Public Property ValoresPlan As New List(Of ValorReglaPerfilImportacion)()
    Public Property ValoresEfectivos As New List(Of ValorReglaPerfilImportacion)()
    'Distingue SQL NULL de una coleccion efectiva vacia ya fijada.
    Public Property ValoresEfectivosFijados As Boolean
    Public Property Resultado As ResultadoEfectoImportacion
    Public Property IntentoActivoId As String
    Public Property Revision As UInteger
End Class

'Lectura interna coherente; no es un contrato HTTP ni permite editar el plan.
Public Class VistaValoresImportacion
    Public Property Lote As LoteImportacionCarpetas
    Public Property Raiz As RaizImportacionCarpetas
    Public Property Archivo As ArchivoImportacionCarpetas
End Class

Public Class VistaReconciliacionImportacion
    Public Property Lote As LoteImportacionCarpetas
    Public Property Plan As PlanImportacionCarpetas
    Public Property Raiz As RaizImportacionCarpetas
    Public Property Archivo As ArchivoImportacionCarpetas
    Public Property Intento As IntentoImportacionCarpetas
End Class
Public Class ResultadoReconciliacionImportacion
    Public Property LoteId As String
    Public Property IntentoId As String
    Public Property Fase As String
    Public Property Resuelta As Boolean
    Public Property Repeticion As Boolean
    Public Property Resultado As ResultadoEfectoImportacion
End Class

'Datos internos del inventario completo; no acepta un agregado enviado por HTTP.
Public Class VistaConsolidacionImportacion
    Public Property Lote As LoteImportacionCarpetas
    Public Property Plan As PlanImportacionCarpetas
    Public Property Raices As New List(Of RaizImportacionCarpetas)()
    Public Property Archivos As New List(Of ArchivoImportacionCarpetas)()
    Public Property Intentos As New List(Of IntentoImportacionCarpetas)()
End Class
Public Class EstadoRaizConsolidadaImportacion
    Public Property RaizId As String
    Public Property Revision As UInteger
    Public Property EstadoOriginal As String
    Public Property Estado As String
End Class
Public Class ConsolidacionImportacion
    Public Property LoteId As String
    Public Property Revision As UInteger
    Public Property EstadoOriginal As String
    Public Property Estado As String
    Public Property HayIntentosActivos As Boolean
    Public Property HayIncertidumbre As Boolean
    Public Property HayRechazados As Boolean
    Public Property HayPendientes As Boolean
    Public Property ArchivosConfirmados As UInteger
    Public Property RaicesConfirmadas As UInteger
    Public Property Raices As New List(Of EstadoRaizConsolidadaImportacion)()
End Class

Public Class IntentoImportacionCarpetas
    'Resultado transitorio del comando documental, no evidencia persistida.
    'Un intento recuperado/repetido nunca concede una nueva reserva.
    <Newtonsoft.Json.JsonIgnore>
    Public Property EsNuevaReserva As Boolean
    Public Property IntentoId As String
    Public Property LoteId As String
    Public Property RaizId As String
    Public Property ArchivoId As String
    Public Property Fase As String
    Public Property Estado As String
    Public Property EjecutorRef As String
    Public Property TokenReclamacion As String
    Public Property SolicitudHash As String
    Public Property InicioUtc As String
    Public Property UltimoContactoUtc As String
    Public Property FinUtc As String
    Public Property Resultado As ResultadoEfectoImportacion
End Class

Public Class EventoImportacionCarpetas
    Public Property EventoId As String
    Public Property LoteId As String
    Public Property IntentoId As String
    Public Property RecursoTipo As String
    Public Property RecursoId As String
    Public Property Accion As String
    Public Property Codigo As String
    Public Property ActorRef As String
    Public Property OcurridoUtc As String
    Public Property Resultado As ResultadoEfectoImportacion
End Class

Public Class IdentidadExpedienteImportacion
    Public Property OrigenLogico As String
    Public Property ExpedienteId As String
    Public Property RelacionNivelId As String
    Public Property NivelRef As String
End Class

Public Class ResultadoEfectoImportacion
    Public Property Estado As String
    Public Property Codigo As String
    Public Property Mensaje As String
    <Newtonsoft.Json.JsonProperty(NullValueHandling:=Newtonsoft.Json.NullValueHandling.Ignore)>
    Public Property Recepcion As RecepcionVerificadaImportacion
    Public Property Expediente As IdentidadExpedienteImportacion
    <Newtonsoft.Json.JsonProperty(NullValueHandling:=Newtonsoft.Json.NullValueHandling.Ignore)>
    Public Property Creacion As EvidenciaCreacionExpedienteImportacion
    Public Property OrigenGabinete As String
    Public Property GabineteId As String
    Public Property DocumentoId As String
    <Newtonsoft.Json.JsonProperty(NullValueHandling:=Newtonsoft.Json.NullValueHandling.Ignore)>
    Public Property InventarioId As String
    Public Property ValoresEfectivos As New List(Of ValorReglaPerfilImportacion)()
End Class

Public Class EvidenciaCreacionExpedienteImportacion
    Public Property IntentoId As String
    Public Property CicloArchivoId As String
    Public Property CicloArchivoNombre As String
End Class

Public Class ReclamacionImportacionCarpetas
    Public Property LoteId As String
    Public Property RecursoTipo As String
    Public Property RecursoId As String
    Public Property Fase As String
    Public Property RevisionEsperada As UInteger
    Public Property EjecutorRef As String
    Public Property TokenReclamacion As String
    Public Property SolicitudHash As String
End Class

'Resultado interno/transitorio. Las listas describen esta lectura fisica;
'no son permiso durable de ejecutar ni cambian el inventario confirmado.
Public Class ReservaCreacionVerificadaImportacion
    Public Property Intento As IntentoImportacionCarpetas
    Public Property ArchivosVerificados As New List(Of String)()
    Public Property ArchivosInciertos As New List(Of String)()
End Class

'Vista interna coherente del plan sellado y metadatos del ambito. No DTO HTTP.
Public Class VistaBarreraCreacionImportacion
    Public Property Lote As LoteImportacionCarpetas
    Public Property Plan As PlanImportacionCarpetas
    Public Property Raiz As RaizImportacionCarpetas
    Public Property Raices As List(Of RaizImportacionCarpetas)
    Public Property Archivos As List(Of ArchivoImportacionCarpetas)
End Class

'Salida interna de una sola fase. No afirma completitud del lote.
Public Class ResultadoFaseCreacionImportacion
    Public Property LoteId As String
    Public Property RaizId As String
    Public Property IntentoId As String
    Public Property Repeticion As Boolean
    Public Property Resultado As ResultadoEfectoImportacion
    Public Property ArchivosVerificados As New List(Of String)()
    Public Property ArchivosInciertos As New List(Of String)()
End Class

'Una fase por archivo; CONFIRMADO no declara completitud de raiz/lote.
Public Class ResultadoFaseDocumentoImportacion
    Public Property LoteId As String
    Public Property RaizId As String
    Public Property ArchivoId As String
    Public Property IntentoId As String
    Public Property Repeticion As Boolean
    Public Property Resultado As ResultadoEfectoImportacion
    Public Property ArchivosVerificados As New List(Of String)()
    Public Property ArchivosInciertos As New List(Of String)()
End Class

Public Class RecepcionVerificadaImportacion
    Public Property StagingRef As String
    Public Property BytesRecibidos As String
    Public Property ContenidoHash As String
End Class

Public Class PaginaRecursosImportacion(Of T)
    Public Property Elementos As New List(Of T)()
    Public Property Total As UInteger
    Public Property RevisionLectura As UInteger
End Class

Public Class FiltroConsultaImportacion
    Public Property Pagina As Integer = 1
    Public Property TamanoPagina As Integer = 50
    Public Property RaizId As String
    'Estado de raiz o de efecto documental, segun el recurso consultado.
    Public Property Estado As String
    Public Property EstadoRecepcion As String
End Class

Public Class EstadoLecturaImportacion
    Public Property Lote As LoteImportacionCarpetas
    Public Property HayIntentosActivos As Boolean
    Public Property HayIncertidumbre As Boolean
End Class

Public Class ConfirmacionLecturaImportacion
    Public Property Plan As PlanImportacionCarpetas
    Public Property Lote As LoteImportacionCarpetas
End Class

'Las dimensiones permanecen separadas. Estos conjuntos no autorizan transiciones:
'el coordinador debe comprobar su tabla de transiciones y la evidencia del efecto.
Public NotInheritable Class EstadosImportacionCarpetas
    Private Shared ReadOnly Plan As New HashSet(Of String)(StringComparer.Ordinal) From {"REVISABLE", "BLOQUEADO", "CONFIRMADO", "EXPIRADO"}
    Private Shared ReadOnly Recepcion As New HashSet(Of String)(StringComparer.Ordinal) From {"PENDIENTE", "RECIBIENDO", "RECIBIDO", "RECHAZADO", "INCIERTO", "CANCELADO"}
    Private Shared ReadOnly Efecto As New HashSet(Of String)(StringComparer.Ordinal) From {"NO_INICIADO", "EN_CURSO", "ALMACENADO", "COMPLETADO", "RECHAZADO", "INCIERTO", "CANCELADO"}
    Private Shared ReadOnly Raiz As New HashSet(Of String)(StringComparer.Ordinal) From {"PENDIENTE", "CREANDO", "EXPEDIENTE_CONFIRMADO", "INCIERTO", "BLOQUEADO", "COMPLETADA", "PARCIAL", "CANCELADA"}
    Private Shared ReadOnly Intento As New HashSet(Of String)(StringComparer.Ordinal) From {"RESERVADO", "EN_CURSO", "CONFIRMADO", "RECHAZADO", "INCIERTO"}
    Private Shared ReadOnly Lote As New HashSet(Of String)(StringComparer.Ordinal) From {"RECIBIENDO", "LISTO", "EN_CURSO", "PENDIENTE_VERIFICACION", "DETENIDO", "COMPLETADO", "PARCIAL", "FALLIDO"}
    Private Sub New()
    End Sub
    Public Shared Function EsPlan(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Plan.Contains(valor)
    End Function
    Public Shared Function EsRecepcion(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Recepcion.Contains(valor)
    End Function
    Public Shared Function EsEfecto(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Efecto.Contains(valor)
    End Function
    Public Shared Function EsRaiz(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Raiz.Contains(valor)
    End Function
    Public Shared Function EsIntento(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Intento.Contains(valor)
    End Function
    Public Shared Function EsLote(valor As String) As Boolean
        Return valor IsNot Nothing AndAlso Lote.Contains(valor)
    End Function
End Class
