Imports System
Imports System.Configuration

' Gate específico de Importar Servicio Web: requiere bandera y audiencia explícita.
Public NotInheritable Class ImportarServicioWebFeatureGate
    Private Const ActiveKey As String = "WorkflowCentroTrabajoModernActive"
    Private Const UsersKey As String = "WorkflowCentroTrabajoModernUsers"
    Private Const GroupsKey As String = "WorkflowCentroTrabajoModernGroups"

    Public Function EstaHabilitado(ByVal contexto As ContextoModuloWorkflow) As Boolean
        If contexto Is Nothing OrElse Not contexto.EsValido() Then Return False
        If Not String.Equals(Read(ActiveKey), "true", StringComparison.OrdinalIgnoreCase) Then Return False

        Dim users As String = Read(UsersKey)
        Dim groups As String = Read(GroupsKey)
        If String.IsNullOrWhiteSpace(users) AndAlso String.IsNullOrWhiteSpace(groups) Then Return False

        Return Contains(users, contexto.LoginUsuario) OrElse Contains(groups, contexto.IdGrupoWorkflow.ToString())
    End Function

    Private Shared Function Read(ByVal key As String) As String
        Return If(ConfigurationManager.AppSettings(key), String.Empty).Trim()
    End Function

    Private Shared Function Contains(ByVal configured As String, ByVal value As String) As Boolean
        If String.IsNullOrWhiteSpace(configured) OrElse String.IsNullOrWhiteSpace(value) Then Return False
        For Each item As String In configured.Split(New Char() {","c, ";"c, ControlChars.Cr, ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)
            If String.Equals(item.Trim(), value.Trim(), StringComparison.OrdinalIgnoreCase) Then Return True
        Next
        Return False
    End Function
End Class
