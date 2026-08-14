Imports System

'Contexto comun para infraestructura reutilizable. No depende de Workflow ni de Web Forms.
Public Class ContextoModulo
    Public Property CodigoModulo As String
    Public Property IdUsuario As Integer
    Public Property IdGrupo As Integer
    Public Property LoginUsuario As String

    Public Overridable Function EsValido() As Boolean
        Return Not String.IsNullOrWhiteSpace(CodigoModulo) AndAlso
               IdUsuario > 0 AndAlso
               Not String.IsNullOrWhiteSpace(LoginUsuario)
    End Function
End Class
