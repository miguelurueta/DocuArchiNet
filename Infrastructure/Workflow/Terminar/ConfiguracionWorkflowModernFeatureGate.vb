Imports System
Imports System.Configuration

'La unica implementacion de habilitacion nueva. No depende de Page ni consulta Session.
Public Class ConfiguracionWorkflowModernFeatureGate
    Implements IWorkflowModernFeatureGate

    Private Const ClaveActiva As String = "WorkflowCentroTrabajoModernActive"
    Private Const ClaveUsuarios As String = "WorkflowCentroTrabajoModernUsers"
    Private Const ClaveGrupos As String = "WorkflowCentroTrabajoModernGroups"
    Private Const ClaveUsuariosExcluidos As String = "WorkflowCentroTrabajoModernExcludedUsers"
    Private Const ClaveGruposExcluidos As String = "WorkflowCentroTrabajoModernExcludedGroups"

    Public Function Evaluar(ByVal contexto As ContextoModuloWorkflow) As HabilitacionWorkflowModern Implements IWorkflowModernFeatureGate.Evaluar
        If contexto Is Nothing OrElse Not contexto.EsValido() Then
            Return Crear("inactivo", "WORKFLOW_CONTEXT_INVALID", "La experiencia moderna no esta habilitada para este contexto.")
        End If

        Dim activa As String = Leer(ClaveActiva)
        If String.IsNullOrWhiteSpace(activa) Then
            Return Crear("inactivo", "WORKFLOW_MODERN_INACTIVE", "La experiencia moderna no esta habilitada.")
        End If

        If Not EsBooleanoHabilitado(activa) Then
            Return Crear("inactivo", "WORKFLOW_MODERN_INACTIVE", "La experiencia moderna no esta habilitada.")
        End If

        If Contiene(Leer(ClaveUsuariosExcluidos), contexto.LoginUsuario) OrElse
           Contiene(Leer(ClaveGruposExcluidos), contexto.IdGrupoWorkflow.ToString()) Then
            Return Crear("excluido", "WORKFLOW_MODERN_EXCLUDED", "La experiencia moderna no esta habilitada para este perfil.")
        End If

        Dim usuarios As String = Leer(ClaveUsuarios)
        Dim grupos As String = Leer(ClaveGrupos)
        If Not String.IsNullOrWhiteSpace(usuarios) OrElse Not String.IsNullOrWhiteSpace(grupos) Then
            If Not Contiene(usuarios, contexto.LoginUsuario) AndAlso
               Not Contiene(grupos, contexto.IdGrupoWorkflow.ToString()) Then
                Return Crear("inactivo", "WORKFLOW_MODERN_INACTIVE", "La experiencia moderna no esta habilitada para este perfil.")
            End If
        End If

        Return Crear("activo", "WORKFLOW_MODERN_ACTIVE", "La experiencia moderna esta habilitada.")
    End Function

    Private Shared Function Leer(ByVal clave As String) As String
        Dim valor As String = ConfigurationManager.AppSettings(clave)
        Return If(valor, String.Empty).Trim()
    End Function

    Private Shared Function EsBooleanoHabilitado(ByVal valor As String) As Boolean
        Return String.Equals(valor, "true", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(valor, "1", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(valor, "yes", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function Contiene(ByVal configuracion As String, ByVal valor As String) As Boolean
        If String.IsNullOrWhiteSpace(configuracion) OrElse String.IsNullOrWhiteSpace(valor) Then Return False
        For Each item As String In configuracion.Split(New Char() {","c, ";"c, ControlChars.Cr, ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)
            If String.Equals(item.Trim(), valor.Trim(), StringComparison.OrdinalIgnoreCase) Then Return True
        Next
        Return False
    End Function

    Private Shared Function Crear(ByVal estado As String, ByVal codigo As String, ByVal mensaje As String) As HabilitacionWorkflowModern
        Return New HabilitacionWorkflowModern With {.Estado = estado, .Codigo = codigo, .MensajeFuncional = mensaje}
    End Function
End Class
