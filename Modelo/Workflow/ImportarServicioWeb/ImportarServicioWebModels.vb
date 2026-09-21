Imports System
Imports System.Collections.Generic

Public NotInheritable Class ContextoIntencionImportacion
    Public Property OperationId As String
    Public Property CorrelationId As String
    Public Property IdUsuario As Integer
    Public Property IdGrupo As Integer
    Public Property LoginUsuario As String
    Public Property IdTarea As Long
    Public Property IdRuta As Integer
    Public Property IdTramite As Integer
    Public Property ProviderId As String
    Public Property Radicado As String
End Class

Public NotInheritable Class MetadatosAlmacenamientoImportacion
    Public Property NombreRutaWorkflow As String
    Public Property NombreGabinete As String
    Public Property RadicadoSii As String
    Public Property NombreClaseFormatoDocumento As String
End Class

Public NotInheritable Class ResolucionTipoDocumentalImportacion
    Public Property Valida As Boolean
    Public Property Codigo As String
    Public Property IdTipoListaChequeo As Integer
    Public Property IdTipoDocumentalTrd As Integer
    Public Property NombreTipoDocumental As String
End Class

Public NotInheritable Class TipoDocumentalCatalogoImportacion
    Public Property IdTipoDocumentalTrd As Integer
    Public Property Nombre As String
    Public Property Obligatorio As Boolean
    Public Property Orden As Integer
End Class

Public NotInheritable Class EstadoItemListadoImportacion
    Public Property TieneAntecedente As Boolean
    Public Property Confirmado As Boolean
    Public Property TieneNovedad As Boolean
End Class

' Modelos internos del núcleo de importación. No conocen DTOs ni infraestructura web.
Public Class CapacidadProveedorImportacion
    Public Property Codigo As String
    Public Property Habilitada As Boolean
    Public Property TimeoutSeconds As Nullable(Of Integer)
End Class

Public Class ProveedorImportacion
    Public Property IdentidadCanonica As String
    Public Property Habilitado As Boolean
    Public Property Capacidades As IList(Of CapacidadProveedorImportacion)

    Public Sub New()
        Capacidades = New List(Of CapacidadProveedorImportacion)()
    End Sub
End Class

Public Class IdentidadExternaImportacion
    Public Property ProviderId As String
    Public Property ExternalKey As String
End Class

Public Class ElementoExternoImportacion
    Public Property Identidad As IdentidadExternaImportacion
    Public Property NombreVisible As String
    Public Property TipoContenido As String
    Public Property Longitud As Nullable(Of Long)
    Public Property PermitePreview As Boolean
End Class

Public Enum FaseImportacionServicio
    Creada
    Validada
    ExpedientesPlanificados
    ExpedientesResueltos
    RecursoObtenido
    ExpedientePreparado
    DocumentoAlmacenado
    ItemsSiiAlmacenados
    UniversoDocumentalConsultado
    VinculacionesProcesadas
    IndicesActualizados
    IndicesYXmlActualizados
    CacheActualizado
    ResultadoIncierto
    RequiereDecision
    Reconciliada
    Completada
    FallidaAntesDePersistir
    Parcial
    Detenida
    Omitida
End Enum

Public Class RequisitoPlanImportacion
    Public Property Codigo As String
    Public Property Satisfecho As Boolean
    Public Property MensajeVisible As String
End Class

Public Class ComandoDocumentoImportacion
    Public Property ClientItemId As String
    Public Property IdentidadExterna As IdentidadExternaImportacion
    ' Identidad canónica de tipo_doc_series; nunca es ID_TIPO_DOCUMENTAL_CHEQUEO.
    Public Property IdTipoDocumental As Nullable(Of Integer)
    Public Property NombreTipoDocumental As String
    Public Property NombreArchivo As String
    Public Property TipoContenido As String
End Class

Public Class PlanImportacionServicio
    Public Sub New()
        Requisitos = New List(Of RequisitoPlanImportacion)()
        Comandos = New List(Of ComandoDocumentoImportacion)()
    End Sub

    Public Property Valido As Boolean
    Public Property Requisitos As IList(Of RequisitoPlanImportacion)
    Public Property Comandos As IList(Of ComandoDocumentoImportacion)
End Class

Public Class ResultadoElementoImportacion
    Public Property ClientItemId As String
    Public Property ClaveInscripcion As String
    Public Property IdentidadExterna As IdentidadExternaImportacion
    Public Property IdTareaDestino As Long
    Public Property IdTipoDocumental As Nullable(Of Integer)
    Public Property NombreTipoDocumental As String
    Public Property NombreArchivo As String
    Public Property TipoContenido As String
    Public Property Fase As FaseImportacionServicio
    Public Property IdDocumento As Nullable(Of Long)
    Public Property IdExpediente As Nullable(Of Long)
    Public Property EstadoAlmacenamiento As EstadoEfectoExpedienteImportacion
    Public Property EstadoRelacion As EstadoEfectoExpedienteImportacion
    Public Property EstadoIndice As EstadoEfectoExpedienteImportacion
    Public Property EstadoCache As EstadoEfectoExpedienteImportacion
    Public Property CodigoError As String
    Public Property MensajeVisible As String
    Public Property PersistenciaConocida As Boolean
    Public Property Reintentable As Boolean
    Public Property CorrelationId As String
    ' Estado efímero de una ejecución. Nunca se serializa ni se persiste en el repositorio.
    Public Property ContenidoDescargado As Byte()
    Public Property RutaArchivoPreparado As String
    Public Property MetadatosSii As MetadatosDocumentoSii
End Class

Public NotInheritable Class MetadatosDocumentoSii
    Public Property Libro As String
    Public Property Registro As String
    Public Property Duplicado As String
    Public Property Fecha As String
    Public Property Hora As String
    Public Property UsuarioSii As String
    Public Property Acto As String
    Public Property NombreActo As String
    Public Property Matricula As String
    Public Property Proponente As String
    Public Property RazonSocial As String
    Public Property NitCedula As String
    Public Property IdAnexo As String
    Public Property TipoImagen As String
    Public Property TipoAnexo As String
    Public Property TipoSirep As String
    Public Property TipoDigitalizacion As String
    Public Property IdentificadorImagen As String
    Public Property Formato As String
    Public Property FechaDocumento As String
    Public Property Origen As String
    Public Property Observaciones As String
End Class

Public NotInheritable Class IntentoServicioExterno
    Public Property ProviderId As String
    Public Property Operacion As String
    Public Property Exitoso As Boolean
    Public Property CodigoError As String
    Public Property CategoriaError As String
    Public Property CodigoDependencia As String
    Public Property MensajeDiagnostico As String
    Public Property EstadoHttp As Nullable(Of Integer)
    Public Property Reintentable As Boolean
    Public Property TaskId As Nullable(Of Long)
    Public Property Radicado As String
    Public Property CodigoBarras As String
    Public Property ReferenciaProveedor As String
    Public Property IntentId As String
    Public Property ClientItemId As String
    Public Property OperationId As String
    Public Property CorrelationId As String
    Public Property FechaInicioUtc As DateTime
    Public Property FechaFinUtc As DateTime
    Public Property DuracionMs As Long
End Class

Public NotInheritable Class DisponibilidadServicioExterno
    Public Property ProviderId As String
    Public Property Operacion As String
    Public Property DesdeUtc As DateTime
    Public Property HastaUtc As DateTime
    Public Property Total As Long
    Public Property Exitosos As Long
    Public Property Fallidos As Long
    Public Property DisponibilidadPorcentaje As Decimal
    Public Property LatenciaPromedioMs As Decimal
End Class

Public Enum ConsistenciaDocumentoImportacion
    NoConfirmado
    Confirmado
    RelacionAusente
    RelacionDuplicada
    TareaDistinta
    ResultadoIncierto
End Enum

' Instantánea read-only; representa evidencia persistida sin conocer capas de transporte o datos.
Public Class SnapshotReconciliacionImportacion
    Public Sub New()
        Items = New List(Of SnapshotItemReconciliacionImportacion)()
        DocumentosRelacionados = New List(Of DocumentoRelacionadoImportacion)()
    End Sub
    Public Property IntentId As String
    Public Property VersionToken As String
    Public Property Fase As FaseImportacionServicio
    Public Property IdUsuario As Integer
    Public Property IdTareaOriginal As Long
    Public Property ProviderId As String
    Public Property ContextoOriginal As ContextoIntencionImportacion
    Public Property Items As IList(Of SnapshotItemReconciliacionImportacion)
    Public Property DocumentosRelacionados As IList(Of DocumentoRelacionadoImportacion)
End Class

Public Class SnapshotItemReconciliacionImportacion
    Public Property ClientItemId As String
    Public Property ProviderId As String
    Public Property ExternalKey As String
    Public Property IdTareaDestino As Long
    Public Property IdDocumento As Nullable(Of Long)
    Public Property NombreDocumento As String
    Public Property TipoContenido As String
    Public Property Fase As FaseImportacionServicio
    Public Property PersistenciaConocida As Boolean
    Public Property Reintentable As Boolean
    Public Property CodigoError As String
    Public Property MensajeVisible As String
    Public Property CorrelationId As String
    Public Property CantidadDocumentos As Integer
    Public Property CantidadRelaciones As Integer
    Public Property CantidadRelacionesOtraTarea As Integer
End Class

Public Class ResultadoFaseImportacion
    Public Property Exitoso As Boolean
    Public Property PersistenciaConocida As Boolean
    Public Property Reintentable As Boolean
    Public Property IdDocumento As Nullable(Of Long)
    Public Property Codigo As String
    Public Property MensajeVisible As String
End Class

Public Class TransicionImportacion
    Public Property IntentId As String
    Public Property ClientItemId As String
    Public Property FaseAnterior As FaseImportacionServicio
    Public Property FaseNueva As FaseImportacionServicio
    Public Property VersionAnterior As String
    Public Property VersionNueva As String
    Public Property FechaUtc As DateTime
    Public Property CorrelationId As String
    Public Property Codigo As String
End Class

Public Class ResultadoTransicionImportacion
    Public Property Aceptada As Boolean
    Public Property Codigo As String
    Public Property MensajeVisible As String
    Public Property Transicion As TransicionImportacion
End Class

Public Class IntencionImportacionServicio
    Public Sub New()
        Resultados = New List(Of ResultadoElementoImportacion)()
        Requisitos = New List(Of RequisitoPlanImportacion)()
        Inscripciones = New List(Of InscripcionImportacion)()
    End Sub

    Public Property Id As String
    Public Property IdempotencyKey As String
    Public Property HuellaContexto As String
    Public Property ContextoOriginal As ContextoIntencionImportacion
    Public Property VersionToken As String
    Public Property Fase As FaseImportacionServicio
    Public Property FechaCreacionUtc As DateTime
    Public Property FechaActualizacionUtc As DateTime
    Public Property Requisitos As IList(Of RequisitoPlanImportacion)
    Public Property Resultados As IList(Of ResultadoElementoImportacion)
    Public Property Inscripciones As IList(Of InscripcionImportacion)
    Public Property DetencionSolicitada As Boolean
End Class

Public Class ResultadoPersistenciaIntencionImportacion
    Public Property Intencion As IntencionImportacionServicio
    Public Property Reutilizada As Boolean
    Public Property Codigo As String
    Public Property MensajeVisible As String
End Class

Public Class ResultadoGuardIntencionImportacion
    Public Property Adquirido As Boolean
    Public Property Lease As IDisposable
    Public Property Codigo As String
    Public Property MensajeVisible As String
End Class

Public Class ResultadoValidacionContextoImportacion
    Public Property Valido As Boolean
    Public Property Codigo As String
    Public Property MensajeVisible As String

    Public Shared Function Exitoso() As ResultadoValidacionContextoImportacion
        Return New ResultadoValidacionContextoImportacion With {.Valido = True}
    End Function

    Public Shared Function Fallido(ByVal codigo As String,
                                   ByVal mensajeVisible As String) As ResultadoValidacionContextoImportacion
        Return New ResultadoValidacionContextoImportacion With {
            .Valido = False,
            .Codigo = codigo,
            .MensajeVisible = mensajeVisible
        }
    End Function
End Class

Public Class ResultadoResolucionProveedorImportacion
    Public Property Proveedor As IExternalImportProvider
    Public Property Codigo As String
    Public Property MensajeVisible As String

    Public ReadOnly Property Encontrado As Boolean
        Get
            Return Proveedor IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Codigo)
        End Get
    End Property
End Class

Public Class ResultadoResolucionClienteProveedorImportacion
    Public Property Cliente As IExternalImportProviderClient
    Public Property Codigo As String
    Public Property MensajeVisible As String

    Public ReadOnly Property Encontrado As Boolean
        Get
            Return Cliente IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Codigo)
        End Get
    End Property
End Class

Public Enum RolExpedienteImportacion
    Pendiente
    Unico
    Primario
    Secundario
End Enum

Public Enum EstadoEfectoExpedienteImportacion
    Pendiente
    NoAplica
    Confirmado
    Ausente
    Conflicto
    ResultadoIncierto
    Fallido
End Enum

Public Enum ModoExpedienteImportacion
    SinExpediente
    GestionarExpediente
End Enum

Public Enum EstadoRelacionDocumentoExpediente
    NoConsultada
    Ausente
    Correcta
    Duplicada
    Cruzada
    ResultadoIncierto
End Enum

' Agregado persistible: conserva la inscripción y sus documentos sin depender del cliente al reanudar.
Public NotInheritable Class InscripcionImportacion
    Public Sub New()
        ClientItemIds = New List(Of String)()
    End Sub

    Public Property ClaveInscripcion As String
    Public Property Orden As Integer
    Public Property Libro As String
    Public Property Registro As String
    Public Property Matricula As String
    Public Property MatriculaNormalizada As String
    Public Property Proponente As String
    Public Property IdentificacionSujeto As String
    Public Property RazonSocial As String
    Public Property MatriculaPropietario As String
    Public Property IdentificacionPropietario As String
    Public Property NombrePropietario As String
    Public Property NombreGabinete As String
    Public Property RadicadoSii As String
    Public Property IdExpediente As Nullable(Of Long)
    Public Property RolExpediente As RolExpedienteImportacion
    Public Property EstadoExpediente As EstadoEfectoExpedienteImportacion
    Public Property EstadoCache As EstadoEfectoExpedienteImportacion
    Public Property ClientItemIds As IList(Of String)
End Class

Public NotInheritable Class ConfiguracionExpedienteImportacion
    Public Sub New()
        CamposIdentidad = New List(Of CampoIdentidadExpedienteImportacion)()
        TipologiasSecundarias = New List(Of Integer)()
    End Sub

    Public Property NombreGabinete As String
    Public Property IdAutoRegistro As Integer
    Public Property CreacionAutomaticaHabilitada As Boolean
    Public Property ExpedienteObligatorio As Boolean
    Public Property Modo As ModoExpedienteImportacion
    Public Property MultiplesExpedientes As Boolean
    Public Property CamposIdentidad As IList(Of CampoIdentidadExpedienteImportacion)
    Public Property TipologiasSecundarias As IList(Of Integer)
End Class

' Respuesta interna tipada de la consulta moderna de sujeto SII. No conserva token,
' credenciales, cuerpo JSON ni mensajes crudos del proveedor.
Public NotInheritable Class SujetoExpedienteSii
    Public Property MatriculaCanonica As String
    Public Property Identificacion As String
    Public Property RazonSocial As String
    Public Property MatriculaPropietario As String
    Public Property IdentificacionPropietario As String
    Public Property NombrePropietario As String
End Class

Public NotInheritable Class CampoIdentidadExpedienteImportacion
    Public Property NombreCampo As String
    Public Property Valor As String
    Public Property Obligatorio As Boolean
End Class

Public NotInheritable Class IdentidadExpedienteNormalizada
    Public Property Valida As Boolean
    Public Property Codigo As String
    Public Property NombreGabinete As String
    Public Property ValorConsulta As String
    Public Property ValorPersistencia As String
End Class

Public NotInheritable Class DestinoLogicoExpedienteImportacion
    Public Property ClaveInscripcion As String
    Public Property IdTipoDocumental As Nullable(Of Integer)
    Public Property IdExpediente As Long
    Public Property Rol As RolExpedienteImportacion
End Class

Public NotInheritable Class PlanExpedienteImportacion
    Public Sub New()
        Inscripciones = New List(Of InscripcionImportacion)()
        Destinos = New List(Of DestinoLogicoExpedienteImportacion)()
    End Sub

    Public Property IntentId As String
    Public Property Inscripciones As IList(Of InscripcionImportacion)
    Public Property Destinos As IList(Of DestinoLogicoExpedienteImportacion)
    Public Property Estado As EstadoEfectoExpedienteImportacion
    Public Property Codigo As String
    Public Property Modo As ModoExpedienteImportacion
End Class

Public NotInheritable Class DocumentoRelacionadoImportacion
    Public Property IntentId As String
    Public Property IdTarea As Long
    Public Property IdImagen As Long
    Public Property NombreGabinete As String
    Public Property RadicadoSii As String
    Public Property IdTipoDocumental As Nullable(Of Integer)
    Public Property ClaveInscripcion As String
    Public Property IdExpedienteEsperado As Nullable(Of Long)
    Public Property EstadoDestino As EstadoEfectoExpedienteImportacion
    Public Property EstadoRelacion As EstadoRelacionDocumentoExpediente
    Public Property EstadoCache As EstadoEfectoExpedienteImportacion
    Public Property EstadoIndiceGabinete As EstadoEfectoExpedienteImportacion
    Public Property EstadoIndiceSql As EstadoEfectoExpedienteImportacion
    Public Property EstadoIndiceXml As EstadoEfectoExpedienteImportacion
    Public Property EstadoReconciliacion As EstadoEfectoExpedienteImportacion
End Class

Public NotInheritable Class PlanDocumentosRelacionadosImportacion
    Public Sub New()
        Documentos = New List(Of DocumentoRelacionadoImportacion)()
    End Sub

    Public Property IntentId As String
    Public Property Documentos As IList(Of DocumentoRelacionadoImportacion)
    Public Property Estado As EstadoEfectoExpedienteImportacion
    Public Property Codigo As String
End Class

Public NotInheritable Class EntradaCacheVinculoDocumentoImportacion
    Public Property IdTarea As Long
    Public Property IdImagen As Long
    Public Property NombreGabinete As String
    Public Property IdExpedienteEsperado As Long
    Public Property RadicadoSii As String
    Public Property EstadoRelacion As EstadoRelacionDocumentoExpediente
    Public Property FechaCreacionUtc As DateTime
    Public Property FechaVerificacionUtc As Nullable(Of DateTime)
End Class

Public NotInheritable Class EvidenciaIndiceElectronicoImportacion
    Public Property IdExpediente As Long
    Public Property IdImagen As Long
    Public Property SqlConfirmado As Boolean
    Public Property XmlConfirmado As Boolean
    Public Property Estado As EstadoEfectoExpedienteImportacion
End Class

' Resultado seguro por efecto: no expone SQL, rutas físicas, sesión ni mensajes de excepción.
Public NotInheritable Class ResultadoEfectoExpedienteImportacion
    Public Property Estado As EstadoEfectoExpedienteImportacion
    Public Property Codigo As String
    Public Property MensajeVisible As String
    Public Property Reintentable As Boolean
    Public Property IdExpediente As Nullable(Of Long)
    Public Property Relacion As EstadoRelacionDocumentoExpediente
End Class

Public Class ContextoImportacionServicio
    Private ReadOnly _idUsuario As Integer
    Private ReadOnly _idGrupo As Integer
    Private ReadOnly _loginUsuario As String
    Private ReadOnly _idTarea As Long
    Private ReadOnly _idRuta As Integer
    Private ReadOnly _idTramite As Integer
    Private ReadOnly _providerId As String
    Private ReadOnly _permiteImportar As Boolean
    Private ReadOnly _idUsuarioGestion As Integer
    Private ReadOnly _idEmpresaGestion As Integer
    Private ReadOnly _nombreRutaWorkflow As String

    Public Sub New(ByVal idUsuario As Integer,
                   ByVal idGrupo As Integer,
                   ByVal loginUsuario As String,
                   ByVal idTarea As Long,
                   ByVal idRuta As Integer,
                   ByVal idTramite As Integer,
                   ByVal providerId As String,
                   ByVal permiteImportar As Boolean,
                   Optional ByVal idUsuarioGestion As Integer = 0,
                   Optional ByVal idEmpresaGestion As Integer = 0,
                   Optional ByVal nombreRutaWorkflow As String = Nothing)
        _idUsuario = idUsuario
        _idGrupo = idGrupo
        _loginUsuario = loginUsuario
        _idTarea = idTarea
        _idRuta = idRuta
        _idTramite = idTramite
        _providerId = providerId
        _permiteImportar = permiteImportar
        _idUsuarioGestion = idUsuarioGestion
        _idEmpresaGestion = idEmpresaGestion
        _nombreRutaWorkflow = If(nombreRutaWorkflow, String.Empty).Trim()
    End Sub

    Public ReadOnly Property IdUsuario As Integer
        Get
            Return _idUsuario
        End Get
    End Property

    Public ReadOnly Property IdGrupo As Integer
        Get
            Return _idGrupo
        End Get
    End Property

    Public ReadOnly Property LoginUsuario As String
        Get
            Return _loginUsuario
        End Get
    End Property

    Public ReadOnly Property IdTarea As Long
        Get
            Return _idTarea
        End Get
    End Property

    Public ReadOnly Property IdRuta As Integer
        Get
            Return _idRuta
        End Get
    End Property

    Public ReadOnly Property IdTramite As Integer
        Get
            Return _idTramite
        End Get
    End Property

    Public ReadOnly Property ProviderId As String
        Get
            Return _providerId
        End Get
    End Property

    Public ReadOnly Property PermiteImportar As Boolean
        Get
            Return _permiteImportar
        End Get
    End Property

    Public ReadOnly Property IdUsuarioGestion As Integer
        Get
            Return _idUsuarioGestion
        End Get
    End Property

    Public ReadOnly Property IdEmpresaGestion As Integer
        Get
            Return _idEmpresaGestion
        End Get
    End Property

    Public ReadOnly Property NombreRutaWorkflow As String
        Get
            Return _nombreRutaWorkflow
        End Get
    End Property
End Class
