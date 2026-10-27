Option Strict On
'Identidades persistidas: INT UNSIGNED AUTO_INCREMENT en RA; transporte decimal
'canónico String (1..4294967295), sin GUID ni ceros iniciales. Referencias opcionales Nothing.
Imports System
Imports System.Collections.Generic

'Contratos propios de DOC-110. No dependen de sesión, transporte ni objetos documentales.
Public Class ContextoPerfilImportacion
    Public Property UsuarioRef As String
    Public Property AmbitoRef As String
    Public Property PuedeAdministrar As Boolean
    Public Property PuedeAdministrarProveedores As Boolean
    Public Property PuedeAdministrarFuentes As Boolean
End Class

Public Class PerfilImportacion
    Public Property PerfilId As String
    Public Property AmbitoRef As String
    Public Property Codigo As String
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property Activo As Boolean
    Public Property VersionPublicadaId As String
    Public Property Revision As UInteger
    Public Property Version As VersionPerfilImportacion
End Class

Public Class VersionPerfilImportacion
    Public Property VersionId As String
    Public Property PerfilId As String
    Public Property Numero As UInteger
    Public Property Revision As UInteger
    Public Property Estado As String
    Public Property Configuracion As New ConfiguracionPerfilImportacion()
    Public Property Reglas As New List(Of ReglaPerfilImportacion)()
End Class

Public Class ConfiguracionPerfilImportacion
    Public Property Esquema As Integer = 1
    Public Property NombreVisible As String
    Public Property GabineteId As Nullable(Of UInteger)
    Public Property OrigenGabinete As String = "DA"
    Public Property HuellaOrigenGabinete As String
    Public Property FondoRef As String
    Public Property InstrumentoRef As String
    Public Property AreaRef As String
    Public Property SerieRef As String
    Public Property SubserieRef As String
    Public Property TipoExpedienteRef As String
    Public Property UnidadDocumentalRef As String
    Public Property ModoIntegridad As String = "COMPLETO_OBLIGATORIO"
    Public Property AmbitoIntegridad As String
    Public Property EstrategiaCarpetas As String
    Public Property PoliticaSubcarpetas As String
    Public Property PoliticaCarpetasVacias As String
    Public Property PoliticaArchivosSueltos As String
    Public Property PoliticaCoincidencias As String = "BLOQUEAR"
    Public Property Extensiones As New List(Of String)()
    Public Property MaximoBytesArchivo As Nullable(Of Long)
End Class

Public Class OpcionPerfilImportacion
    Public Property value As String
    Public Property text As String
    Public Property Tipo As String
    Public Property Longitud As Integer
    Public Property Requerida As Boolean
    Public Property TipoFisico As String
    Public Property Precision As Integer
    Public Property Escala As Integer
    Public Property Charset As String
End Class

Public Class ReglaPerfilImportacion
    'Ausencia conserva exactamente el JSON/huella anterior a AMP-01.
    <Newtonsoft.Json.JsonProperty(NullValueHandling:=Newtonsoft.Json.NullValueHandling.Ignore)>
    Public Property Proveedor As FuenteProveedorIndices
    Public Property ReglaId As String
    Public Property DestinoTipo As String
    Public Property DestinoRef As String
    Public Property FuenteTipo As String
    Public Property EtapaResolucion As String = "PREFLIGHT"
    Public Property Obligatoria As Boolean
    Public Property Orden As UInteger
    Public Property ValorConstante As String
    Public Property PropiedadExpediente As String
    'COMPLETO o SEGMENTO. Posición de segmento de base cero, separador literal.
    Public Property Extraccion As String = "COMPLETO"
    Public Property Separador As String
    Public Property Posicion As Integer
    Public Property QuitarExtension As Boolean
    Public Property RecortarEspacios As Boolean
    Public Property TipoValor As String = "TEXTO"
    Public Property FormatoFecha As String
    Public Property LongitudMaxima As Integer = 160
End Class

Public Class EjemploPerfilImportacion
    Public Property NombreCarpeta As String
    Public Property NombreArchivo As String
    Public Property Expediente As New Dictionary(Of String, String)(StringComparer.Ordinal)
End Class

Public Class IncidenciaPerfilImportacion
    Public Property Campo As String
    Public Property Codigo As String
    Public Property Mensaje As String
    Public Sub New()
    End Sub
    Public Sub New(campo As String, codigo As String, mensaje As String)
        Me.Campo = campo
        Me.Codigo = codigo
        Me.Mensaje = mensaje
    End Sub
End Class

Public Class ValorReglaPerfilImportacion
    <Newtonsoft.Json.JsonProperty(NullValueHandling:=Newtonsoft.Json.NullValueHandling.Ignore)>
    Public Property Procedencia As String
    Public Property FuenteTipo As String
    Public Property DestinoTipo As String
    Public Property DestinoRef As String
    Public Property TipoValor As String
    Public Property Valor As String
    Public Property Diferido As Boolean
    Public Property PropiedadPendiente As String
End Class

Public Class ResultadoEnsayoPerfilImportacion
    Public Property Valores As New List(Of ValorReglaPerfilImportacion)()
    Public Property Incidencias As New List(Of IncidenciaPerfilImportacion)()
End Class

Public Class PaginaPerfilesImportacion
    Public Property Elementos As New List(Of PerfilImportacion)()
    Public Property Total As Long
End Class

Public Class FiltroPerfilesImportacion
    Public Property Termino As String
    Public Property Pagina As Integer = 1
    Public Property TamanoPagina As Integer = 25
    Public Property Codigo As String
    Public Property Nombre As String
    Public Property Activo As Nullable(Of Boolean)
End Class

Public Class PerfilImportacionException
    Inherits Exception
    Public ReadOnly Property Codigo As String
    Public Sub New(codigo As String, mensaje As String)
        MyBase.New(mensaje)
        Me.Codigo = codigo
    End Sub
End Class
