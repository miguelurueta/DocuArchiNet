Imports System.Web

Public Class MyUtils

    Public Shared Function GetWebsiteRoot() As String
        Dim req As HttpRequest = HttpContext.Current.Request

        Dim port As String = IIf(req.Url.Port = 80 OrElse req.Url.Port = 443, "", ":" + req.Url.Port.ToString())

        Dim wsroot = req.Url.Scheme + "://" + req.Url.Host + port + req.ApplicationPath

        If (wsroot.EndsWith("/")) Then
		Return wsroot
	Else
		Return wsroot + "/"
	End If 
    End Function


End Class
