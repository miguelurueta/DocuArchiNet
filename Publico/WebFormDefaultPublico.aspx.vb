Public Class WebFormDefaultPublico
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.Page.IsPostBack = False Then
            Dim Refclas As New GestorModuleSesion.Gestor_conexion
            Dim Refclassprocsecion As New ClassGestorSesion
            Dim Refclasda As New ClassDaIncioDocuarchi
            Dim refclasgestiondocumental As New ClassGestionDocumental
            Dim Result As String = ""
            Dim query_strin As String = ""
            If Session.Item("EMPRESA_GESTION") = "" Then
                query_strin = Context.Request.QueryString("path_confir")
                Session.Item("EMPRESA_GESTION") = query_strin
            End If
            Result = Refclas.inicializa_conexiones_modulos_publico()
            If Result <> "YES" Then
                Label_title_seleccion.Text = Result
                Exit Sub
            End If
            Result = Refclassprocsecion.inicio_adplicacion_web_gestion_publico()
            If Result <> "YES" Then
                Label_title_seleccion.Text = Result
                Exit Sub
            End If
            HttpContext.Current.Session.Item("TIPOMODULO") = "PUBLICO"
            HttpContext.Current.Session.Item("DA_Login_Usuario") = "CONSULTAPUBLICO"
            Dim id_usuario_da As Integer = 0
            Result = Refclasda.Retorna_id_usuario_docuarchi(id_usuario_da,
                                                            HttpContext.Current.Session.Item("DA_Login_Usuario"))
            If Result <> "YES" Then

                Me.Label_title_seleccion.Text = Result
                Exit Sub
            End If
            '-----------------------------------
            'Retorna id usuario gestion
            '-----------------------------------
            If id_usuario_da = 0 Then
                Me.Label_title_seleccion.Text = "El usuario CONSULTAPUBLICO no esta creado contacte al administrador"
                Exit Sub
            End If
        End If
    End Sub
End Class