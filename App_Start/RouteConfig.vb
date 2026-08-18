Imports System.Web.Routing
Imports Microsoft.AspNet.FriendlyUrls
Imports System.Web.Mvc

Public Module RouteConfig
    Sub RegisterRoutes(ByVal routes As RouteCollection)
        'Dim settings As FriendlyUrlSettings = New FriendlyUrlSettings() With {
        '    .AutoRedirectMode = RedirectMode.Permanent
        '}
        'routes.EnableFriendlyUrls(settings)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")
        routes.IgnoreRoute("{resource}.aspx/{*pathInfo}") ' Ignorar WebForms
        routes.IgnoreRoute("{*allaspx}", New With {.allaspx = ".*\.aspx(/.*)?"})
        routes.MapPageRoute(
            routeName:="Inicio",
            routeUrl:="",
            physicalFile:="~/gestor.aspx")
        ' Rutas MVC
        routes.MapRoute(
            name:="Default",
            url:="{controller}/{action}/{id}",
            defaults:=New With {.controller = "Account/Account", .action = "Login", .id = UrlParameter.Optional}
        )

    End Sub
End Module
