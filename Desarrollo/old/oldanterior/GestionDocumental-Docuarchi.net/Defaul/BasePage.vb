Namespace RefreshArticle
    Public Class BasePage
        Inherits System.Web.UI.Page

        Protected Overridable Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
            Try
                Response.AddHeader("Refresh", Convert.ToString((HttpContext.Current.Session.Timeout * 60) + 5))

                If HttpContext.Current.Session("SesionActiva") Is Nothing Then
                    FormsAuthentication.RedirectFromLoginPage("../gestor.aspx", False)
                    'Server.Transfer("../gestor.aspx")
                    'Response.Redirect("gestor.aspx")
                    'Response.RedirectLocation = "~/gestor.aspx"
                    'Dim javascript As New StringBuilder()
                    'javascript.Append("var redirectTimeout;")
                    'javascript.Append(" clearTimeout (redirectTimeout);")
                    'javascript.Append(" alert ('Su sesion caduco el sistema lo redirecciona a la pagina principal');")
                    'javascript.Append("window.location.href ='" + "../gestor.aspx" + "';")

                    'Page.ClientScript.RegisterStartupScript(sender.GetType(), "Refresh", "location.href = '../gestor.aspx'", True)
                    '' 'Register código JavaScript en la página web
                    'sender.ClientScript.RegisterStartupScript(sender.[GetType](), "Page_Load", javascript.ToString(), True)
                    'End If
                    'Dim loginPage As String = "../gestor.aspx"
                    '''' Session Timeout (Default 20 minutes)
                    ' Dim sessionTimeout As Integer = HttpContext.Current.Session.Timeout
                    '''' Timeout for Redirect to Login Page (10 milliseconds before)
                    ' Dim redirectTimeout As Integer = 10 * 6000

                    '''' JavaScript Code
                    'Dim javascript As New StringBuilder()
                    ' javascript.Append("var redirectTimeout;")
                    'javascript.Append("clearTimeout(redirectTimeout);")
                    ' javascript.Append([String].Format("setTimeout(""window.location.href='{0}'"",{1});", loginPage, redirectTimeout))

                    '''' Register JavaScript Code on WebPage
                    'Page.ClientScript.RegisterStartupScript(Page.[GetType](), "RegisterRedirectOnSessionEndScript", javascript.ToString(), True)
                End If
            Catch ex As Exception
                ex.Message.ToString()
            Finally
                If HttpContext.Current.Session("SesionActiva") Is Nothing Then
                    Dim loginPage As String = "../gestor.aspx"
                    '''' Session Timeout (Default 20 minutes)
                    Dim sessionTimeout As Integer = HttpContext.Current.Session.Timeout
                    '''' Timeout for Redirect to Login Page (10 milliseconds before)
                    Dim redirectTimeout As Integer = 10 * 6000

                    '''' JavaScript Code
                    Dim javascript As New StringBuilder()
                    javascript.Append("var redirectTimeout;")
                    javascript.Append("clearTimeout(redirectTimeout);")
                    javascript.Append([String].Format("setTimeout(""window.location.href='{0}'"",{1});", loginPage, redirectTimeout))

                    '''' Register JavaScript Code on WebPage
                    Page.ClientScript.RegisterStartupScript(Page.[GetType](), "RegisterRedirectOnSessionEndScript", javascript.ToString(), True)
                End If
            End Try
        End Sub
    End Class
    Public NotInheritable Class Helper
        Private Sub New()
        End Sub
        '''' <summary>
        ' ''' Registers the redirect on session end script.
        ' ''' </summary>
        '''' <param name="page">The page.</param>
        ''<System.Runtime.CompilerServices.Extension()> _
        Public Shared Sub RegisterRedirectOnSessionEndScript(ByVal page As Page)
            '''' Login Page, We can retrieve for configuration file (Web.Config)
            Dim loginPage As String = "Login.aspx"
            '''' Session Timeout (Default 20 minutes)
            Dim sessionTimeout As Integer = HttpContext.Current.Session.Timeout
            '''' Timeout for Redirect to Login Page (10 milliseconds before)
            Dim redirectTimeout As Integer = (sessionTimeout * 60000) - 10

            '''' JavaScript Code
            Dim javascript As New StringBuilder()
            javascript.Append("var redirectTimeout;")
            javascript.Append("clearTimeout(redirectTimeout);")
            javascript.Append([String].Format("setTimeout(""window.location.href='{0}'"",{1});", loginPage, redirectTimeout))

            '''' Register JavaScript Code on WebPage
            page.ClientScript.RegisterStartupScript(page.[GetType](), "RegisterRedirectOnSessionEndScript", javascript.ToString(), True)
        End Sub
    End Class
End Namespace

