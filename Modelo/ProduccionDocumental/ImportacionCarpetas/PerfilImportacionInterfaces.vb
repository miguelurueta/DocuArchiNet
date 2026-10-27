Option Strict On
'Identidades persistidas: INT UNSIGNED AUTO_INCREMENT en RA; transporte decimal
'canónico String (1..4294967295), sin GUID ni ceros iniciales. Referencias opcionales Nothing.
Imports System
Imports System.Collections.Generic

Public Interface IPerfilImportacionRepository
    Sub VerificarEsquema()
    Function Listar(contexto As ContextoPerfilImportacion, filtro As FiltroPerfilesImportacion) As PaginaPerfilesImportacion
    Function Obtener(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String) As PerfilImportacion
    Function Historial(contexto As ContextoPerfilImportacion, perfilId As String, pagina As Integer, tamano As Integer) As List(Of VersionPerfilImportacion)
    Function Crear(contexto As ContextoPerfilImportacion, codigo As String, nombre As String, descripcion As String) As PerfilImportacion
    Function Guardar(contexto As ContextoPerfilImportacion, perfilId As String, version As VersionPerfilImportacion) As PerfilImportacion
    Function Publicar(contexto As ContextoPerfilImportacion, perfilId As String, versionId As String, revisionPerfil As UInteger, revisionVersion As UInteger, huella As Byte()) As PerfilImportacion
    Function CrearVersion(contexto As ContextoPerfilImportacion, perfilId As String, origenId As String, revisionPerfil As UInteger, revisionOrigen As UInteger) As PerfilImportacion
    Function ActualizarCabecera(contexto As ContextoPerfilImportacion, perfilId As String, revision As UInteger, nombre As String, descripcion As String) As PerfilImportacion
    Function CambiarDisponibilidad(contexto As ContextoPerfilImportacion, perfilId As String, revision As UInteger, activo As Boolean) As PerfilImportacion
End Interface

'El adaptador verifica relaciones y permisos en sus orígenes reales; el resolvedor nunca consulta BD.
Public Interface IPerfilImportacionCatalogos
    Function Validar(contexto As ContextoPerfilImportacion, configuracion As ConfiguracionPerfilImportacion, reglas As List(Of ReglaPerfilImportacion), publicar As Boolean) As List(Of IncidenciaPerfilImportacion)
    Function ValidarValores(contexto As ContextoPerfilImportacion, configuracion As ConfiguracionPerfilImportacion, valores As List(Of ValorReglaPerfilImportacion)) As List(Of IncidenciaPerfilImportacion)
    Function NivelCompatible(contexto As ContextoPerfilImportacion, configuracion As ConfiguracionPerfilImportacion, nivelRef As String) As Boolean
End Interface

'Solo proyeccion autorizada para ensayo; no confirma un expediente existente.
Public Interface IClasificacionEnsayoPerfil
    Function ClasificacionPropuesta(contexto As ContextoPerfilImportacion, configuracion As ConfiguracionPerfilImportacion) As Dictionary(Of String, String)
End Interface
