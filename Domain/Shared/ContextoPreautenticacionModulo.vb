Imports System

' Contexto restringido a la lectura de identidad posterior a credenciales.
' No debilita ContextoModulo: solo admite IdUsuario=0 antes de resolver el principal.
Public NotInheritable Class ContextoPreautenticacionModulo
    Inherits ContextoModulo

    Public Overrides Function EsValido() As Boolean
        Return Not String.IsNullOrWhiteSpace(CodigoModulo) AndAlso
               IdUsuario = 0 AndAlso
               Not String.IsNullOrWhiteSpace(LoginUsuario)
    End Function
End Class
