Imports System

'Revalida CAMBIO_USUARIO desde el servidor antes de alcanzar el motor legacy.
Public Class WorkflowLegacyEnvioUsuarioAutorizacionAdapter
    Implements IEnvioUsuarioAutorizacionRepository

    Public Function TieneCambioUsuario(ByVal contexto As ContextoModuloWorkflow) As Boolean Implements IEnvioUsuarioAutorizacionRepository.TieneCambioUsuario
        If contexto Is Nothing OrElse Not contexto.EsValido() OrElse contexto.IdUsuarioWorkflow <= 0 Then Return False
        Try
            Dim permisos As String() = Nothing
            Dim resultado As String = New Class_permisos_usuarios_workflow().SolicitaPermisosUsuarioWorkflow(
                contexto.IdUsuarioWorkflow,
                permisos)
            Return String.Equals(resultado, "YES", StringComparison.OrdinalIgnoreCase) AndAlso
                   permisos IsNot Nothing AndAlso permisos.Length > 18 AndAlso
                   String.Equals(permisos(18), "1", StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function
End Class
