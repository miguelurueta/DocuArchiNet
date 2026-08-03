
Imports System.Threading.Tasks
Imports System.Web.Mvc
Public Class AccountController
    Inherits Controller

    <HttpGet>
    Public Function Login() As ActionResult
        Return View()
    End Function
End Class
