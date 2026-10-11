Imports System

Public NotInheritable Class LegacyLoginLocalRouteResolver
    Implements ILoginLocalRouteResolver

    Public Const DefaultRoute As String = "~/Defaul/WebFormInicioDocuarchiGestion.aspx"

    Public Function Resolve() As String Implements ILoginLocalRouteResolver.Resolve
        Return DefaultRoute
    End Function
End Class
