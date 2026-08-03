Public Class Classscrripjava
    Public Function Show(ByRef Mensaje As String) As String
        Try
            Dim pageWEB As Page = HttpContext.Current.CurrentHandler
            If (pageWEB.IsPostBack = True And Not pageWEB.ClientScript.IsClientScriptBlockRegistered("alert")) Then
                Dim cleanMessage As String = Mensaje.Replace("'", "\\'")
                Dim comillas As String = Char.ConvertFromUtf32(34)
                Dim Script2 As String = "<script language=" + comillas + "javascript" + comillas + " type=" + comillas + _
                "text/javascript" + comillas + "> alert(" + comillas + cleanMessage + comillas + "); </script>"
                pageWEB.ClientScript.RegisterClientScriptBlock(pageWEB.GetType(), "alert", Script2, False)
            End If
            Show = "YES"
        Catch ex As Exception
            Show = ex.Message
        End Try
    End Function
    Public Function Show(ByRef Mensaje As String, ByRef page As Page) As String
        Try
            Dim pageWEB As Page = page
            If (Not pageWEB.ClientScript.IsClientScriptBlockRegistered("alerta")) Then
                Dim cleanMessage As String = Mensaje.Replace("'", "\\'")
                cleanMessage = cleanMessage.Replace("\", "-")
                cleanMessage = cleanMessage.Replace("/", "-")
                cleanMessage = cleanMessage.Replace(".", "-")
                Dim comillas As String = Char.ConvertFromUtf32(34)
                Dim Script2 As String = "<script language=" + comillas + "javascript" + comillas + " type=" + comillas + _
                "text/javascript" + comillas + "> alert(" + comillas + cleanMessage + comillas + "); </script>"
                pageWEB.ClientScript.RegisterClientScriptBlock(pageWEB.GetType(), "alerta", Script2, False)
            End If
            Show = "YES"
        Catch ex As Exception
            Show = ex.Message
        End Try
    End Function
    Public Function Showscripman(ByRef Mensaje As String, ByRef update As UpdatePanel) As String
        Try
            Dim pageWEB As Page = HttpContext.Current.CurrentHandler
            If (pageWEB.IsPostBack = True) Then
                Dim cleanMessage As String = Mensaje.Replace("'", "")
                cleanMessage = cleanMessage.Replace("\", "-")
                cleanMessage = cleanMessage.Replace("/", "-")
                cleanMessage = cleanMessage.Replace(".", "-")
                cleanMessage = cleanMessage.Replace("#", "-")
                cleanMessage = cleanMessage.Replace(";", "-")
                cleanMessage = cleanMessage.Replace("/*", "-")
                cleanMessage = cleanMessage.Replace("*/", "-")
                cleanMessage = cleanMessage.Replace("*/", "-")
                cleanMessage = cleanMessage.Replace("//", "-")
                cleanMessage = cleanMessage.Replace("var", "-")
                cleanMessage = cleanMessage.Replace(vbCrLf, "-")
                cleanMessage = Trim(cleanMessage)
                Dim comillas As String = Char.ConvertFromUtf32(34)
                Dim comillas_simple As String = Char.ConvertFromUtf32(39)
                Dim Script2 As String = "<script language=" + comillas + "javascript" + comillas + " type=" + comillas + _
                "text/javascript" + comillas + "> alert(" + comillas_simple + cleanMessage + comillas_simple + "); </script>"
                ScriptManager.RegisterClientScriptBlock(update, _
                                            update.GetType(), _
                                            "IdntificadorBloqueScript", _
                                            Script2, _
                                            False)
            End If
            Showscripman = "YES"
        Catch ex As Exception
            Showscripman = ex.Message
        End Try
    End Function
    Public Function Showscripman_menu(ByRef Mensaje As String, ByRef update As UpdatePanel, ByVal menu_nombre As String) As String
        Try
            Dim pageWEB As Page = HttpContext.Current.CurrentHandler
            If (pageWEB.IsPostBack = True) Then
                Dim cleanMessage As String = Mensaje.Replace("'", "")
                cleanMessage = cleanMessage.Replace("\", "-")
                cleanMessage = cleanMessage.Replace("/", "-")
                cleanMessage = cleanMessage.Replace(".", "-")
                cleanMessage = cleanMessage.Replace("#", "-")
                cleanMessage = cleanMessage.Replace(";", "-")
                cleanMessage = cleanMessage.Replace("/*", "-")
                cleanMessage = cleanMessage.Replace("*/", "-")
                cleanMessage = cleanMessage.Replace("*/", "-")
                cleanMessage = cleanMessage.Replace("//", "-")
                cleanMessage = cleanMessage.Replace("var", "-")
                cleanMessage = cleanMessage.Replace(vbCrLf, "-")
                cleanMessage = Trim(cleanMessage)
                Dim comillas As String = Char.ConvertFromUtf32(34)
                Dim Script2 As String = "<script language=" + comillas + "javascript" + comillas + " type=" + comillas + _
                "text/javascript" + comillas + "> document.getElementById('Label_mensaje_personalizado').innerHTML='" + cleanMessage + "';  $find(" + "'" + menu_nombre + "'" + ").show();   </script>"
                ScriptManager.RegisterClientScriptBlock(update, _
                                            update.GetType(), _
                                            "IdntificadorBloqueScript", _
                                            Script2, _
                                            False)
            End If
            Showscripman_menu = "YES"
        Catch ex As Exception
            Showscripman_menu = ex.Message
        End Try
    End Function
    Public Function ShowScripmanRespuesta(ByRef Mensaje As String, ByRef update As UpdatePanel, ByVal nombre_intercambio As String) As String
        Try
            Dim pageweb As Page = HttpContext.Current.CurrentHandler
            If (pageweb.IsPostBack = True) Then
                Dim cleanmessage As String = Mensaje.Replace("'", "\\'")
                Dim comillas As String = Char.ConvertFromUtf32(34)
                Dim script2 As String = "<script language=" + comillas + "javascript" + comillas + " type=" + comillas + _
                "text/javascript" + comillas + "> " +
                " var  ventana = confirm (" + comillas + cleanmessage + comillas + ") ; " +
                " if (ventana) {document.getElementById('" + nombre_intercambio + "').value=" + comillas + "1" + comillas + "} else {document.getElementById('" + nombre_intercambio + "').value=" + comillas + "0" + comillas + "}" +
                "  </script>"
                ScriptManager.RegisterClientScriptBlock(update, _
                                            update.GetType(), _
                                            "idntificadorbloquescript", _
                                            script2, _
                                            False)
            End If
            ShowScripmanRespuesta = "YES"
        Catch ex As Exception
            ShowScripmanRespuesta = ex.Message
        End Try
    End Function
End Class
