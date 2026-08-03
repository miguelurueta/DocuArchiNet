Public Class WebFormInicioDocuarchiGestion
    Inherits System.Web.UI.Page
    Private Sub WebFormInicioDocuarchiGestion_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If Me.IsPostBack = False Then

                '----------------------------------------------
                'Crea directorio temporal
                '----------------------------------------------
                Dim ref_id_usuario_gestion As Object = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
                Dim ref_Id_Usuario_Workflow As Object = HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString
                If HttpContext.Current.Session.Item("TIPOMODULO") = "DOCUARCHI CONTENEDOR" Then
                    If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = "0" Then
                        ref_id_usuario_gestion = Session.Item("ID_USUARIO_DOCUARCHI")
                    End If
                    If HttpContext.Current.Session.Item("Id_Usuario_Workflow") = 0 Then
                        ref_Id_Usuario_Workflow = Session.Item("ID_USUARIO_DOCUARCHI")
                    End If
                End If
                If HttpContext.Current.Session.Item("TIPOMODULO") = "RADICACION DOCUMENTAL" Then
                    If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = "0" Then
                        ref_id_usuario_gestion = Session.Item("RA_ID_USUARIO")
                    End If
                    If HttpContext.Current.Session.Item("Id_Usuario_Workflow") = 0 Then
                        ref_Id_Usuario_Workflow = Session.Item("RA_ID_USUARIO")
                    End If
                End If
                Dim EstadoError As String = ""
                Dim Refclas As New InicioWorkflow
                Dim refclasgestioninicio As New ClassGagestorInicio
                Dim Result = refclasgestioninicio.Crea_Dir_Temporal_gestion()
                If Result <> "YES" Then
                    EstadoError = "Error creando directorio temporal gestión (" & Result & ")"
                End If
                Result = Refclas.Crea_Dir_Temporal_wf()
                If Result <> "YES" Then
                    EstadoError = " " & EstadoError & " - Error creando directorio temporal workflow (" & Result & ")"
                End If
                Result = Refclas.Inicializa_firma_usuario_workflow()
                If Result = "YES" Then
                    HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL") = HttpContext.Current.Session("WF_RUTA_FIRMA") & ref_Id_Usuario_Workflow & ".bmp"
                Else
                    'EstadoError = " " & EstadoError & " - Error inicializando firma workflow (" & Result & ")"
                End If
                Dim CDcarateres As New List(Of CDcarateres)
                Dim ClassCarateres As New ClassCarateres
                Result = ClassCarateres.SolicitaEstructuraCarateres(1,
                                                                    CDcarateres)

                If Result <> "YES" Then
                    EstadoError = " " & EstadoError & " - Error solcitando caracteres no validos (" & Result & ")"
                Else
                    Session.Item("DG_CDCARACTERES") = CDcarateres
                End If
                If EstadoError <> "" Then
                    HttpContext.Current.Response.Write(EstadoError)
                End If
                Dim refclas2 As New ClassNeodynamic
                refclas2.Firma_transparente()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub WebFormInicioDocuarchiGestion_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
    End Sub
    Private Sub ImageButtonSesion_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonSesion.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim refclas As New ClassGestorSesion
            Dim result As String = ""
            result = refclas.Cerrar_sesion_aplicacion_web()
            If result <> "YES" Then
                'scripjava.Showscripman(result, Me.UpdatePanel1)
            End If
            Session.Abandon()
        Catch ex As Exception
            'scripjava.Showscripman(ex.Message, Me.UpdatePanel1)
        End Try
    End Sub
End Class