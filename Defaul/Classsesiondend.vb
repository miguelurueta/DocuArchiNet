Public Class Classsesiondend

    Public Shared Sub RegisterRedirectOnSessionEndScript(ByRef pgina As Page)
        '' 'Página de Acceso, podemos recuperar para el archivo de configuración (Web.config)
        Dim LoginPage As String = "../workflow/WebForindicevacio.aspx"
        '' 'Tiempo de espera de sesión (por defecto 20 minutos)
        Dim SessionTimeout As Integer = HttpContext.Current.Session.Timeout
        '' 'Tiempo de espera para redirigir a la página de inicio (10 milisegundos antes)
        Dim RedirectTimeout As Integer = (SessionTimeout * 60000) - 20
        '' 'Código JavaScript
        Dim javascript As New StringBuilder()
        javascript.Append("var redirectTimeout;")
        javascript.Append(" clearTimeout (redirectTimeout);")
        javascript.Append(" alert ('Su sesion caduco el sistema lo redirecciona a la pagina principal');")
        javascript.Append([String].Format(" setTimeout (" + "window.location.href = '{0}'" + ", {1});", LoginPage, RedirectTimeout))
        '' 'Register código JavaScript en la página web
        pgina.ClientScript.RegisterStartupScript(pgina.[GetType](), "RegisterRedirectOnSessionEndScript", javascript.ToString(), True)
        
    End Sub
End Class
