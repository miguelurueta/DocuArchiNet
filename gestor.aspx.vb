Public Class gestor
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If

    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Buttonaceptar.Click
        Dim Refclas As New ClassGestorSesion
        Dim Result As String = ""
        Dim reclas As New Classscrripjava
        Try
            Result = Refclas.InicioAplicacionWebGestorDocumental(Me.DropDownListmodulos.Text,
                                                           TextBoxuser.Text,
                                                           TextBoxpasw.Text,
                                                           Hiddenempresagestion.Value)
            If Result <> "YES" Then
                reclas.Showscripman(Result, Me.UpdatePanel1)
                Exit Sub
            End If
        Catch ex As Exception
            reclas.Showscripman(ex.Message, Me.UpdatePanel1)
        End Try
    End Sub
    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Dim Mesaje As New Classscrripjava
        Dim Refclassprocsecion As New ClassGestorSesion
        Try

            Dim Result As String = ""
            Dim empresa As String = ""
            If Me.IsPostBack = False Then
                Result = Refclassprocsecion.Proced_Listar_empresas(Me,
                                                                   empresa,
                                                                   Me.DropDownListmodulos)
                If Result <> "YES" Then
                    Mesaje.Showscripman(Result, Me.UpdatePanel1)
                    Exit Sub
                End If
                Session.Item("EMPRESA_GESTION") = empresa
                Hiddenempresagestion.Value = empresa
                Hiddenseleccion.Value = "PRIVADO"
                Me.UpdatePanel1.Update()
                Dim ClientIP, Forwaded, RealIP
                RealIP = ""
                ClientIP = Request.ServerVariables("REMOTE_ADDR")
                ClientIP = ClientIP & "-" & Request.ServerVariables("REMOTE_HOST")
                If ClientIP <> "" Then
                    Session.Item("ip_host_name") = ClientIP
                Else
                    Forwaded = Request.ServerVariables("HTTP_X-Forwarded-For")
                    If Forwaded <> "" Then
                        Session.Item("ip_host_name") = Forwaded
                    Else
                        Session.Item("ip_host_name") = "Imposible encontrar ip host"
                    End If
                End If

            End If
        Catch ex As Exception
            Mesaje.Showscripman(ex.Message, Me.UpdatePanel1)
        End Try
    End Sub



    Private Sub Button_sesion_publico_Click(sender As Object, e As EventArgs) Handles Button_sesion_publico.Click
        Dim Mesaje As New Classscrripjava

        Try
            Dim Refclas As New GestorModuleSesion.Gestor_conexion
            Dim Refclassprocsecion As New ClassGestorSesion
            Dim Result As String = ""
            Me.Hidden_resul_ses_public.Value = ""
            Result = Refclas.inicializa_conexiones_modulos_publico()
            If Result <> "YES" Then
                Mesaje.Showscripman("Imposible cargar los detalles del modulo seleccionado " & Result, Me.UpdatePanel1)
                Exit Sub
            End If
            Result = Refclassprocsecion.inicio_adplicacion_web_gestion_publico()
            If Result <> "YES" Then
                Mesaje.Showscripman("Imposible cargar los detalles del servicio web " & Result, Me.UpdatePanel1)
                Exit Sub
            End If
            HttpContext.Current.Session.Item("TIPOMODULO") = "PUBLICO"
            Hiddenseleccion.Value = "PUBLICO"
            Me.Hidden_resul_ses_public.Value = "YES"
        Catch ex As Exception
            Me.Hidden_resul_ses_public.Value = ""
            Mesaje.Showscripman(ex.Message, Me.UpdatePanel1)
        End Try
    End Sub

    Private Sub Button_sesion_empresa_Click(sender As Object, e As EventArgs) Handles Button_sesion_empresa.Click
        Dim Mesaje As New Classscrripjava
        Dim Refclassprocsecion As New ClassGestorSesion
        Try

            Dim Result As String = ""
            Result = Refclassprocsecion.Proced_Listar_empresas(Me,
                                                                   HttpContext.Current.Session.Item("EMPRESA_GESTION"),
                                                                   Me.DropDownListmodulos)
                If Result <> "YES" Then
                    Mesaje.Showscripman(Result, Me.UpdatePanel1)
                    Exit Sub
                End If
                Dim Refclas As New GestorModuleSesion.Gestor_conexion
                Dim reclas As New Classscrripjava
                Hiddenseleccion.Value = "PUBLICO"
            Me.UpdatePanel1.Update()
            Dim ClientIP, Forwaded, RealIP
            RealIP = ""
                ClientIP = Request.ServerVariables("REMOTE_ADDR")
                ClientIP = ClientIP & "-" & Request.ServerVariables("REMOTE_HOST")
                If ClientIP <> "" Then
                    Session.Item("ip_host_name") = ClientIP
                Else
                    Forwaded = Request.ServerVariables("HTTP_X-Forwarded-For")
                    If Forwaded <> "" Then
                        Session.Item("ip_host_name") = Forwaded
                    Else
                        Session.Item("ip_host_name") = "Imposible encontrar ip host"
                    End If
                End If

        Catch ex As Exception
            Mesaje.Showscripman(ex.Message, Me.UpdatePanel1)
        End Try
    End Sub

    Protected Sub LinkButton_recupera_pw_Click(sender As Object, e As EventArgs) Handles LinkButton_recupera_pw.Click
        Dim Mesaje As New Classscrripjava
        Dim Refclassprocsecion As New ClassGestorSesion
        Try
            If Me.DropDownListmodulos.Text = "" Then
                Mesaje.Showscripman("Debe seleccionar el módulo a recuperar la contraseña", Me.UpdatePanel_recupera_pasw)
                Exit Sub
            End If
            Me.TextBox_loguin_usuario.Text = Me.TextBoxuser.Text
            Me.UpdatePanel_recupera_pasw.Update()
            ModalPopupExtender_recupera_pasw.Show()
        Catch ex As Exception
            Mesaje.Showscripman(ex.Message, Me.UpdatePanel1)
        End Try
    End Sub

    Private Sub Button_Aceptar_Click(sender As Object, e As EventArgs) Handles Button_Aceptar.Click
        Dim Mesaje As New Classscrripjava
        Dim Refclas As New ClassGestorSesion
        Try
            Dim Result As String = ""
            Result = Refclas.Recuperar_pasword_usuario(Me.TextBox_loguin_usuario.Text,
                                                       Me.TextBox_correo_electronico.Text,
                                                       Me.DropDownListmodulos.Text,
                                                       Hiddenempresagestion.Value)
            If Result <> "YES" Then
                'Me.TextBox_correo_electronico.Text = ""
                'Me.TextBox_loguin_usuario.Text = ""
                Mesaje.Showscripman(Result, Me.UpdatePanel_recupera_pasw)
                Exit Sub
            Else
                ModalPopupExtender_recupera_pasw.Hide()
            End If
        Catch ex As Exception
            Mesaje.Showscripman(ex.Message, Me.UpdatePanel_recupera_pasw)
        End Try
    End Sub

    Protected Sub Button_Cancelar_Click(sender As Object, e As EventArgs) Handles Button_Cancelar.Click
        ModalPopupExtender_recupera_pasw.Hide()
    End Sub

End Class