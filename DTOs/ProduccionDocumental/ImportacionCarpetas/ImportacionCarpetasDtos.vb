Option Strict On
Imports System.Collections.Generic

'Entradas cerradas de DOC-111. No reciben usuario, empresa, permisos, snapshot
'autoritativo, SQL, ubicación de staging o destino documental final.
Public Class AbrirImportacionSolicitud
    Public Property nivelRef As String
End Class

Public Class ListarPerfilesImportacionSolicitud
    Public Property nivelRef As String
    Public Property pagina As Integer = 1
    Public Property tamanoPagina As Integer = 50
End Class

Public Class PaginaPerfilesImportacionOperativa
    Public Property pagina As Integer
    Public Property tamanoPagina As Integer
    'Indica que la pagina del origen estaba llena, aunque alguna publicacion
    'no sea compatible. No representa un total ni oculta paginas posteriores.
    Public Property puedeHaberMas As Boolean
    Public Property perfiles As New List(Of SnapshotPerfilImportacionOperativa)()
End Class

Public Class PrepararPlanImportacionSolicitud
    Public Property nivelRef As String
    Public Property versionId As String
    Public Property raices As New List(Of RaizManifiestoImportacionSolicitud)()
    Public Property archivos As New List(Of ArchivoManifiestoImportacionSolicitud)()
End Class

Public Class RaizManifiestoImportacionSolicitud
    Public Property seleccionId As String
    Public Property idCliente As String
    Public Property nombre As String
End Class

Public Class ArchivoManifiestoImportacionSolicitud
    Public Property idCliente As String
    Public Property raizIdCliente As String
    Public Property nombre As String
    Public Property rutaRelativa As String
    Public Property bytes As String
    Public Property ultimaModificacionLocal As String
End Class

Public Class ConfirmarPlanImportacionSolicitud
    Public Property planId As String
    Public Property revisionEsperada As UInteger
    Public Property planHashEsperado As String
    Public Property claveConfirmacion As String
End Class

'Solo metadatos de multipart. Los bytes se reciben por stream, no por JSON.
Public Class RecibirArchivoImportacionSolicitud
    Public Property loteId As String
    Public Property archivoId As String
    Public Property claveIntento As String
End Class

Public Class RecepcionArchivoImportacionResultado
    Public Property loteId As String
    Public Property archivoId As String
    Public Property estadoRecepcion As String
    Public Property estadoEfecto As String
    Public Property bytesRecibidos As String
    Public Property contenidoHash As String
    Public Property revision As UInteger
End Class

Public Class ConfirmacionImportacionResultado
    Public Property loteId As String
    Public Property planId As String
    Public Property nivelRef As String
    Public Property estado As String
    Public Property revision As UInteger
    Public Property totalRaices As UInteger
    Public Property totalArchivos As UInteger
    Public Property totalBytes As String
End Class

Public Class DetenerLoteImportacionSolicitud
    Public Property loteId As String
    Public Property revisionEsperada As UInteger
End Class

'Cada llamada avanza solo la fase indicada del recurso persistido.
Public Class AvanzarLoteImportacionSolicitud
    Public Property loteId As String
    Public Property recursoId As String
    Public Property fase As String
    Public Property revisionEsperada As UInteger
    Public Property claveOperacion As String
    'Exclusivos de RETOMAR_CREACION; nunca contienen token ni evidencia.
    Public Property intentoId As String
    Public Property revisionLoteEsperada As UInteger
    Public Property levantarDetencion As Boolean
End Class

Public Class ReconciliarRecursoImportacionSolicitud
    Public Property loteId As String
    Public Property intentoId As String
    Public Property revisionEsperada As UInteger
End Class

'No admite evidencia, IDs documentales, proveedor ni rutas del cliente.
Public Class RecuperarDocumentoV2ImportacionSolicitud
    Public Property loteId As String
    Public Property intentoId As String
    Public Property revisionLoteEsperada As UInteger
    Public Property revisionArchivoEsperada As UInteger
End Class

'El resultado del control es distinto del resultado de almacenar un archivo.
Public Class RecuperacionDocumentoImportacionResultado
    Public Property loteId As String
    Public Property intentoId As String
    Public Property fase As String
    Public Property estado As String
    Public Property codigo As String
    Public Property resuelta As Boolean
    Public Property repeticion As Boolean
    Public Property cierreSinInvocacion As Boolean
    Public Property documentoConfirmado As Boolean
    Public Property expedienteId As String
    Public Property relacionNivelId As String
    Public Property gabineteId As String
    Public Property documentoId As String
    Public Property inventarioId As String
End Class

'Proyeccion publica: no serializar resultados internos de los adaptadores.
Public Class AvanceLoteImportacionResultado
    Public Property loteId As String
    Public Property raizId As String
    Public Property archivoId As String
    Public Property intentoId As String
    Public Property fase As String
    Public Property estado As String
    Public Property codigo As String
    'Materializacion usa el checkpoint de valores y no crea un intento.
    'En esa fase no se clasifica la llamada como nueva/repetida.
    Public Property repeticion As System.Nullable(Of Boolean)
    Public Property expedienteId As String
    Public Property relacionNivelId As String
    Public Property gabineteId As String
    Public Property documentoId As String
    Public Property inventarioId As String
End Class

Public Class ReconciliacionRecursoImportacionResultado
    Inherits AvanceLoteImportacionResultado
    Public Property resuelta As Boolean
End Class

Public Class DetencionLoteImportacionResultado
    Public Property loteId As String
    Public Property estado As String
    Public Property revision As UInteger
    Public Property detencionSolicitada As Boolean
End Class

Public Class LimitesAperturaImportacion
    Public Property maximoRaices As Integer
    Public Property maximoArchivos As Integer
    Public Property maximoBytesArchivo As String
    Public Property maximoBytesLote As String
    Public Property maximoBytesManifiesto As Integer
End Class

Public Class AperturaImportacionResultado
    Public Property nivelRef As String
    Public Property rutaCanonica As String
    Public Property selloDestino As String
    Public Property limites As LimitesAperturaImportacion
    Public Property perfiles As New List(Of SnapshotPerfilImportacionOperativa)()
    Public Property paginaPerfiles As PaginaPerfilesImportacionOperativa
End Class

Public Class PreparacionImportacionResultado
    Public Property planId As String
    Public Property revision As UInteger
    Public Property planHash As String
    Public Property expiraUtc As String
    Public Property nivelRef As String
    Public Property snapshot As SnapshotPerfilImportacionOperativa
    Public Property resultado As ResultadoPreparacionImportacion
End Class

Public Class ConsultarLoteImportacionSolicitud
    Public Property loteId As String
End Class

Public Class ConsultarConfirmacionImportacionSolicitud
    Public Property planId As String
End Class

Public Class ConsultaConfirmacionImportacionResultado
    Public Property planId As String
    Public Property confirmada As Boolean
    Public Property loteId As String
    Public Property nivelRef As String
End Class

Public Class ConsultarRecursosImportacionSolicitud
    Inherits ConsultarLoteImportacionSolicitud
    Public Property tipo As String
    Public Property pagina As Integer = 1
    Public Property tamanoPagina As Integer = 50
    Public Property raizId As String
    Public Property estado As String
    Public Property estadoRecepcion As String
End Class

Public Class EstadoLoteImportacionResultado
    Public Property maximoTamanoPagina As Integer
    Public Property loteId As String
    Public Property nivelRef As String
    Public Property rutaCanonica As String
    Public Property perfilId As String
    Public Property versionId As String
    Public Property estado As String
    Public Property revision As UInteger
    Public Property detencionSolicitada As Boolean
    Public Property hayIntentosActivos As Boolean
    Public Property hayIncertidumbre As Boolean
    Public Property totalRaices As UInteger
    Public Property totalArchivos As UInteger
    Public Property totalBytes As String
    Public Property modoIntegridad As String
    Public Property ambitoIntegridad As String
    Public Property accionesPermitidas As New List(Of String)()
End Class

Public Class RecursoConsultaImportacionResultado
    'Solo metadatos: el efecto vuelve a comprobar permisos, revision y staging.
    Public Property recepcionListaParaCrear As Boolean
    Public Property puedeRetomarCreacion As Boolean
    Public Property rutaRelativa As String
    Public Property ultimaModificacionLocal As String
    Public Property recursoId As String
    Public Property raizId As String
    Public Property idCliente As String
    Public Property nombre As String
    Public Property estado As String
    Public Property estadoRecepcion As String
    Public Property bytesPrevistos As String
    Public Property bytesRecibidos As String
    Public Property revision As UInteger
    Public Property tieneIntentoActivo As Boolean
    'Referencia numerica consultable para recuperar una respuesta perdida.
    'No concede una reclamacion ni transporta su token.
    Public Property intentoActivoId As String
    Public Property valoresEfectivosFijados As Boolean
    Public Property nombreExpediente As String
    Public Property expedienteId As String
    Public Property relacionNivelId As String
    Public Property gabineteId As String
    Public Property documentoId As String
    'Codigo cerrado, sin mensajes libres ni evidencia interna del adaptador.
    Public Property codigoResultado As String
End Class

Public Class PaginaConsultaImportacionResultado
    Public Property loteId As String
    Public Property tipo As String
    Public Property pagina As Integer
    Public Property tamanoPagina As Integer
    Public Property revisionLectura As UInteger
    Public Property totalFiltrado As UInteger
    Public Property totalOriginal As UInteger
    Public Property puedeHaberMas As Boolean
    Public Property elementos As New List(Of RecursoConsultaImportacionResultado)()
End Class

Public Class ExportacionPaginaImportacionResultado
    Public Property pagina As PaginaConsultaImportacionResultado
    Public Property nombreArchivo As String
    Public Property tipoContenido As String = "text/csv; charset=utf-8"
    Public Property contenido As String
End Class

'Diagnostico interno de solo lectura. Ser candidato a revision no autoriza borrar.
Public Class DiagnosticoRetencionImportacionResultado
    Public ReadOnly Property diagnosticoEnSeco As Boolean = True
    Public ReadOnly Property eliminacionHabilitada As Boolean = False
    Public Property loteId As String
    Public Property pagina As Integer
    Public Property tamanoPagina As Integer
    Public Property revisionLectura As UInteger
    Public Property revisionEstable As Boolean
    Public Property totalOriginal As UInteger
    Public Property totalFiltrado As UInteger
    Public Property puedeHaberMas As Boolean
    Public Property retencionConfigurada As Boolean
    Public Property evaluadoUtc As String
    Public Property venceRetencionUtc As String
    Public Property candidatosEnPagina As Integer
    Public Property elementos As New List(Of DiagnosticoArchivoRetencionImportacion)()
End Class

Public Class DiagnosticoArchivoRetencionImportacion
    Public Property archivoId As String
    Public Property raizId As String
    Public Property referenciaTemporalRegistrada As Boolean
    Public Property candidatoRevision As Boolean
    Public Property motivo As String
End Class

'Capacidad interna; no hay WebMethod ni habilitacion desde el cliente.
Public Class LimpiarOriginalImportacionSolicitud
    Public Property loteId As String
    Public Property archivoId As String
    Public Property revisionLoteEsperada As UInteger
    Public Property revisionArchivoEsperada As UInteger
End Class

Public Class LimpiezaOriginalImportacionResultado
    Public Property loteId As String
    Public Property archivoId As String
    Public Property estado As String
    Public Property codigo As String
    Public Property repeticion As Boolean
End Class
