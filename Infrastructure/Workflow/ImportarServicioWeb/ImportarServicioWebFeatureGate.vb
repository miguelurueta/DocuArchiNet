Imports System
Imports System.Configuration

' Gate específico de Importar Servicio Web: requiere bandera activa y una sesión Workflow válida.
Public NotInheritable Class ImportarServicioWebFeatureGate
    Private Const ActiveKey As String = "WorkflowCentroTrabajoModernActive"

    Public Function EstaHabilitado(ByVal contexto As ContextoModuloWorkflow) As Boolean
        If contexto Is Nothing OrElse Not contexto.EsValido() Then Return False
        Return String.Equals(Read(ActiveKey), "true", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function Read(ByVal key As String) As String
        Return If(ConfigurationManager.AppSettings(key), String.Empty).Trim()
    End Function
End Class
