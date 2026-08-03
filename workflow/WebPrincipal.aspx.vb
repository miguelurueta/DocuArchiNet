
Public Class WorkflowPrincipal
    Inherits RefreshArticle.BasePage
    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Me.IsPostBack = False Then
                If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 1 Then
                    Me.Hidden_url_service.Value = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
                End If
                If HttpContext.Current.Session.Item("RA_ACTIVA_WEB_SERVICE") = 1 Then
                    Me.Hidden_url_service.Value = HttpContext.Current.Session.Item("RA_URL_WEB_SERVICE")
                End If
                If HttpContext.Current.Session.Item("DA_ACTIVA_WEB_SERVICE") = 1 Then
                    Me.Hidden_url_service.Value = HttpContext.Current.Session.Item("DA_URL_WEB_SERVICE")
                End If
                If HttpContext.Current.Session.Item("GA_ACTIVA_WEB_SERVICE") = 1 Then
                    Me.Hidden_url_service.Value = HttpContext.Current.Session.Item("GA_URL_WEB_SERVICE")
                End If
                Dim refclasgestioninicio As New ClassGagestorInicio
                Dim Result = refclasgestioninicio.Crea_Dir_Temporal_gestion()
                If Result <> "YES" Then
                    LabelEstado.Text = Result
                    'Exit Sub
                End If
                Dim Refclas As New InicioWorkflow
                Result = Refclas.Crea_Dir_Temporal_wf()
                If Result <> "YES" Then
                    LabelEstado.Text = LabelEstado.Text & Result & vbCrLf
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub ImageButtonSesion_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonSesion.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim refclas As New ClassGestorSesion
            Dim result As String = ""
            result = refclas.Cerrar_sesion_aplicacion_web()
            If result <> "YES" Then
                scripjava.Showscripman(result, Me.UpdatePanel1)
            End If
            Session.Abandon()
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel1)
        End Try
    End Sub

    Private Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Try
            'Dim OB As New localhost.Service
            'OB.Url = Me.Hidden_url_service.Value
            'If Me.Hidden_url_service.Value <> "" Then
            '    'OB.HelloWorld()
            'End If
        Catch ex As Exception

        End Try
      
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            Dim Result As String = ""
            Dim Ref As New ClassGestorSesion
            Dim reclas As New Classscrripjava
            If Not IsPostBack Then
                If HttpContext.Current.Session.Item("TIPOMODULO") <> "PUBLICO" Then
                    Result = Ref.selecciona_treview_aplicacion_web_gestion(Me.Page, _
                                                                           Me.UpdatePanel1, _
                                                                           Me.LabelEstado, "")
                    If Result <> "YES" Then
                        reclas.Showscripman(Result, Me.UpdatePanel1)
                    End If
                Else
                    Hiddenseleccion.Value = "PUBLICO"
                    If Me.Page.IsPostBack = False Then
                        ifrm_ds_.Attributes.Add("src", "../Publico/WebFormDefaultPublico.aspx")
                    End If

                End If

                Session.Item("WF_URL_SELECCION") = Me.Hidden_selecion_url.Value
            End If

        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button_service_Click(sender As Object, e As EventArgs) Handles Button_service.Click
        Dim scripjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 1 Then
                Me.Hidden_url_service.Value = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
            End If
            If HttpContext.Current.Session.Item("RA_ACTIVA_WEB_SERVICE") = 1 Then
                Me.Hidden_url_service.Value = HttpContext.Current.Session.Item("RA_URL_WEB_SERVICE")
            End If
            If HttpContext.Current.Session.Item("DA_ACTIVA_WEB_SERVICE") = 1 Then
                Me.Hidden_url_service.Value = HttpContext.Current.Session.Item("DA_URL_WEB_SERVICE")
            End If
            If HttpContext.Current.Session.Item("GA_ACTIVA_WEB_SERVICE") = 1 Then
                Me.Hidden_url_service.Value = HttpContext.Current.Session.Item("GA_URL_WEB_SERVICE")
            End If
            'Dim OB As New localhost.Service
            'ob.Url = Me.Hidden_url_service.Value
            'scripjava.Showscripman(Me.Hidden_url_service.Value, Me.UpdatePanel_webservice)
            'If Me.Hidden_url_service.Value <> "" Then
            ' OB.HelloWorld()

            ' End If

        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_webservice)
        End Try
    End Sub

    Protected Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim Ref As New ClassGestorSesion
        Dim reclas As New Classscrripjava
        Try
            Me.TreeView1.SelectedNode.Expand()
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("TIPOMODULO") <> "PUBLICO" Then
                Result = Ref.selecciona_treview_aplicacion_web_gestion(Me.Page, Me.UpdatePanel1, Me.LabelEstado, "")
                If Result <> "YES" Then
                    reclas.Showscripman(Result, Me.update_tre_principal)
                End If
            Else
                Hiddenseleccion.Value = "PUBLICO"
                If Me.Page.IsPostBack = False Then
                    ifrm_ds_.Attributes.Add("src", "../Publico/WebFormDefaultPublico.aspx")
                End If

            End If
            Session.Item("WF_URL_SELECCION") = Me.Hidden_selecion_url.Value
        Catch ex As Exception
            reclas.Showscripman(ex.Message, Me.update_tre_principal)
        End Try
    End Sub

    
    Private Sub ImageButtonSesion_dos_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonSesion_dos.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim refclas As New ClassGestorSesion
            Dim result As String = ""
            result = refclas.Cerrar_sesion_aplicacion_web()
            If result <> "YES" Then
                scripjava.Showscripman(result, Me.UpdatePanel1)
            End If
            'Session.Abandon()
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel1)
        End Try
    End Sub
    Private Sub Button_activa_busqueda_treview_Click(sender As Object, e As EventArgs) Handles Button_activa_busqueda_treview.Click
        Dim reclas As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref As New ClassGestorSesion
            If HttpContext.Current.Session.Item("TIPOMODULO") <> "PUBLICO" Then
                Result = Ref.selecciona_treview_aplicacion_web_gestion(Me.Page, _
                                                                       Me.UpdatePanel1, _
                                                                       Me.LabelEstado, _
                                                                       Me.Hidden_texto_buequeda.Value)
                If Result <> "YES" Then
                    reclas.Showscripman(Result, Me.update_tre_principal)
                End If
            Else
                Hiddenseleccion.Value = "PUBLICO"
                If Me.Page.IsPostBack = False Then
                    ifrm_ds_.Attributes.Add("src", "../Publico/WebFormDefaultPublico.aspx")
                End If

            End If
            Session.Item("WF_URL_SELECCION") = Me.Hidden_selecion_url.Value
        Catch ex As Exception
            reclas.Showscripman(ex.Message, Me.update_tre_principal)
        End Try

    End Sub
End Class