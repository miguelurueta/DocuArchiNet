Imports System
Imports System.Configuration
Imports System.Globalization

'La unica implementacion de habilitacion nueva. No depende de Page ni consulta Session.
Public Class ConfiguracionWorkflowModernFeatureGate
    Implements IWorkflowModernFeatureGate

    Private Const ClaveActiva As String = "WorkflowCentroTrabajoModernActive"
    Private Const ClaveModoOficial As String = "WorkflowCentroTrabajoModernOfficialMode"
    Private Const ClaveUsuarios As String = "WorkflowCentroTrabajoModernUsers"
    Private Const ClaveGrupos As String = "WorkflowCentroTrabajoModernGroups"
    Private Const ClaveUsuariosExcluidos As String = "WorkflowCentroTrabajoModernExcludedUsers"
    Private Const ClaveGruposExcluidos As String = "WorkflowCentroTrabajoModernExcludedGroups"
    Private Const ClaveInicioPiloto As String = "WorkflowCentroTrabajoModernPilotStartUtc"
    Private Const ClaveResponsablePiloto As String = "WorkflowCentroTrabajoModernPilotOwner"
    Private Const ClaveMotivoPiloto As String = "WorkflowCentroTrabajoModernPilotReason"
    Private Const ClaveRollbackUtc As String = "WorkflowCentroTrabajoModernRollbackUtc"
    Private Const ClaveResponsableRollback As String = "WorkflowCentroTrabajoModernRollbackOwner"
    Private Const ClaveMotivoRollback As String = "WorkflowCentroTrabajoModernRollbackReason"
    Private Const ClaveCorrelacionRollback As String = "WorkflowCentroTrabajoModernRollbackCorrelation"

    Public Function Evaluar(ByVal contexto As ContextoModuloWorkflow) As HabilitacionWorkflowModern Implements IWorkflowModernFeatureGate.Evaluar
        If contexto Is Nothing OrElse Not contexto.EsValido() Then
            Return Crear("inactivo", "WORKFLOW_CONTEXT_INVALID", "La experiencia moderna no esta habilitada para este contexto.")
        End If

        Dim activa As String = Leer(ClaveActiva)
        If String.IsNullOrWhiteSpace(activa) Then
            If TieneMetadatosRollbackValidos() Then
                Return Crear("fallback-legacy", CodigosBloqueoPrevisualizacion.ModernoRollbackActivo, "La experiencia moderna se encuentra desactivada.")
            End If
            Return Crear("inactivo", "WORKFLOW_MODERN_INACTIVE", "La experiencia moderna no esta habilitada.")
        End If

        If Not EsBooleanoHabilitado(activa) Then
            If TieneMetadatosRollbackValidos() Then
                Return Crear("fallback-legacy", CodigosBloqueoPrevisualizacion.ModernoRollbackActivo, "La experiencia moderna se encuentra desactivada.")
            End If
            Return Crear("inactivo", "WORKFLOW_MODERN_INACTIVE", "La experiencia moderna no esta habilitada.")
        End If

        If Contiene(Leer(ClaveUsuariosExcluidos), contexto.LoginUsuario) OrElse
           Contiene(Leer(ClaveGruposExcluidos), contexto.IdGrupoWorkflow.ToString()) Then
            Return Crear("excluido", "WORKFLOW_MODERN_EXCLUDED", "La experiencia moderna no esta habilitada para este perfil.")
        End If

        Dim usuarios As String = Leer(ClaveUsuarios)
        Dim grupos As String = Leer(ClaveGrupos)
        If EsBooleanoHabilitado(Leer(ClaveModoOficial)) Then
            If Not String.IsNullOrWhiteSpace(usuarios) OrElse Not String.IsNullOrWhiteSpace(grupos) Then
                Return Crear("fallback-legacy", CodigosBloqueoPrevisualizacion.ModernoAlcanceOficialInconsistente, "La experiencia moderna no esta habilitada para este perfil.")
            End If
            If Not TieneMetadatosPilotoValidos() Then
                Return Crear("fallback-legacy", CodigosBloqueoPrevisualizacion.ModernoMetadatosPilotoInvalidos, "La experiencia moderna no esta habilitada para este perfil.")
            End If
            Return Crear("activo", "WORKFLOW_MODERN_ACTIVE", "La experiencia moderna esta habilitada.")
        End If

        If String.IsNullOrWhiteSpace(usuarios) AndAlso String.IsNullOrWhiteSpace(grupos) Then
            Return Crear("fallback-legacy", CodigosBloqueoPrevisualizacion.ModernoAlcancePilotoRequerido, "La experiencia moderna no esta habilitada para este perfil.")
        End If

        If Not Contiene(usuarios, contexto.LoginUsuario) AndAlso
           Not Contiene(grupos, contexto.IdGrupoWorkflow.ToString()) Then
            Return Crear("inactivo", "WORKFLOW_MODERN_INACTIVE", "La experiencia moderna no esta habilitada para este perfil.")
        End If

        If Not TieneMetadatosPilotoValidos() Then
            Return Crear("fallback-legacy", CodigosBloqueoPrevisualizacion.ModernoMetadatosPilotoInvalidos, "La experiencia moderna no esta habilitada para este perfil.")
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

    Private Shared Function TieneMetadatosPilotoValidos() As Boolean
        Dim inicio As String = Leer(ClaveInicioPiloto)
        Dim responsable As String = Leer(ClaveResponsablePiloto)
        Dim motivo As String = Leer(ClaveMotivoPiloto)
        Dim inicioUtc As DateTime

        Return DateTime.TryParseExact(inicio,
                                      "yyyy-MM-ddTHH:mm:ssZ",
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal,
                                      inicioUtc) AndAlso
               Not String.IsNullOrWhiteSpace(responsable) AndAlso
               Not String.IsNullOrWhiteSpace(motivo)
    End Function

    Private Shared Function TieneMetadatosRollbackValidos() As Boolean
        Dim momento As String = Leer(ClaveRollbackUtc)
        Dim responsable As String = Leer(ClaveResponsableRollback)
        Dim motivo As String = Leer(ClaveMotivoRollback)
        Dim correlacion As String = Leer(ClaveCorrelacionRollback)
        Dim momentoUtc As DateTime

        Return DateTime.TryParseExact(momento,
                                      "yyyy-MM-ddTHH:mm:ssZ",
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal,
                                      momentoUtc) AndAlso
               Not String.IsNullOrWhiteSpace(responsable) AndAlso
               Not String.IsNullOrWhiteSpace(motivo) AndAlso
               Not String.IsNullOrWhiteSpace(correlacion)
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
