Imports System.Collections.Generic

' Frontera física para el legacy. La implementación se inyecta en proceso; nunca usa ASMX por loopback.
Public Interface IPhysicalExpedientGateway
    Function Localizar(ByVal contexto As ContextoImportacionServicio,
                       ByVal nombreGabinete As String,
                       ByVal valorConsulta As String) As IList(Of ExpedienteFisicoImportacion)
    Function Crear(ByVal contexto As ContextoImportacionServicio,
                   ByVal inscripcion As InscripcionImportacion,
                   ByVal configuracion As ConfiguracionExpedienteImportacion,
                   ByVal identidad As IdentidadExpedienteNormalizada) As ResultadoCreacionExpedienteFisico
    Function Obtener(ByVal contexto As ContextoImportacionServicio,
                     ByVal idExpediente As Long) As ExpedienteFisicoImportacion
End Interface

' Precheck físico autoritativo por la totalidad de campos únicos configurados.
Public Interface IPhysicalExpedientIdentityLookup
    Function Adquirir(ByVal contexto As ContextoImportacionServicio,
                      ByVal configuracion As ConfiguracionExpedienteImportacion) As IDisposable
    Function Buscar(ByVal contexto As ContextoImportacionServicio,
                    ByVal configuracion As ConfiguracionExpedienteImportacion) As IList(Of Long)
    Function BuscarPrincipal(ByVal contexto As ContextoImportacionServicio,
                             ByVal nombreGabinete As String,
                             ByVal valorIdentidad As String) As IList(Of Long)
End Interface

Public NotInheritable Class ExpedienteFisicoImportacion
    Public Sub New()
        Campos = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
    End Sub

    Public Property IdExpediente As Long
    Public Property NombreGabinete As String
    Public Property ValorIdentidad As String
    Public Property Campos As IDictionary(Of String, String)
End Class

Public NotInheritable Class ResultadoCreacionExpedienteFisico
    Public Property RespuestaRecibida As Boolean
    Public Property Aceptada As Boolean
    Public Property IdExpediente As Nullable(Of Long)
    Public Property Codigo As String
End Class
