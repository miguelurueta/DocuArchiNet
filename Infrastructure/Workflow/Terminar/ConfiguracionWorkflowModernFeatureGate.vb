Imports System

'Politica oficial de disponibilidad moderna. No depende de configuracion, Page ni Session.
Public Class ConfiguracionWorkflowModernFeatureGate
    Implements IWorkflowModernFeatureGate

    Public Function Evaluar(ByVal contexto As ContextoModuloWorkflow) As HabilitacionWorkflowModern Implements IWorkflowModernFeatureGate.Evaluar
        If contexto Is Nothing OrElse Not contexto.EsValido() Then
            Return Crear("inactivo", "WORKFLOW_CONTEXT_INVALID", "La experiencia moderna no esta habilitada para este contexto.")
        End If

        Return Crear("activo", "WORKFLOW_MODERN_OFFICIAL", "La experiencia moderna esta habilitada.")

    End Function

    Private Shared Function Crear(ByVal estado As String, ByVal codigo As String, ByVal mensaje As String) As HabilitacionWorkflowModern
        Return New HabilitacionWorkflowModern With {.Estado = estado, .Codigo = codigo, .MensajeFuncional = mensaje}
    End Function
End Class
