Public Class WebFormConsultaPublico
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then
            '-----------------------------------
            'Retorna login usuario docuarchi
            '-----------------------------------
            HttpContext.Current.Session.Item("DA_Login_Usuario") = "CONSULTAPUBLICO"
            Dim id_usuario_da As Integer = 0
            Dim Result As String
            Dim Refclasda As New ClassDaIncioDocuarchi
            Dim refclasgestiondocumental As New ClassGestionDocumental
            Result = Refclasda.Retorna_id_usuario_docuarchi(id_usuario_da, _
                                                            HttpContext.Current.Session.Item("DA_Login_Usuario"))
            If Result <> "YES" Then
                Me.Label_estado.Text = Result
                Exit Sub
            End If
            '-----------------------------------
            'Retorna id usuario gestion
            '-----------------------------------
            If id_usuario_da = 0 Then
                Me.Label_estado.Text = "El usuario CONSULTAPUBLICO no esta creado contacte al administrador"
                Exit Sub
            End If
            HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI") = id_usuario_da
            Dim id_user_gestion_da As Integer = 0
            Dim Refclasgestor As New ClassGestorDocumental
            Dim id_usuario_gestion_wf As Integer = 0
            Result = Refclasgestor.SolicitaIdUsuarioGestionRelacionUsuarioDocuarchi(id_usuario_da, _
                                                                       id_user_gestion_da)
            If Result <> "YES" Then
                'InicioAplicacionWebGestorDocumental = Result
                'Exit Function
            End If
            '-----------------------------------
            'Retorna grupo usuario docuarchi
            '-----------------------------------
            Dim Refclasinicio As New Class_relacion_usu_grup
            Result = Refclasinicio.SolicitaGrupoRelacionadousuarioDocuarchi(HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"), _
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"))
            If Result <> "YES" Then
                Me.Label_estado.Text = Result
                Exit Sub
            End If
            Dim Refclas As New ClassDaGabinete
            Result = Refclas.Retorna_gabinetes_permitidos(Session.Item("DA_gruposusu"), _
                                                          HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"), _
                                                          Me.DropDownList_gabinetes)
            If Result <> "YES" Then
                Me.Label_estado.Text = Result
                Exit Sub
            End If
        End If
    End Sub
    Private Sub ButtonConsultar_Click(sender As Object, e As EventArgs) Handles ButtonConsultar.Click
        Dim scri As New Classscrripjava
        Try
            If Me.DropDownList_gabinetes.Text <> "" Then
                Dim Result As String = ""
                Dim refgabinete As New ClassDaGabinete
                Dim stru_permiso As stru_permiso_gabinete = Nothing
                Session.Item("DA_GABINETE_CONSULTA") = Me.DropDownList_gabinetes.Text
                If HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = "" Then
                    scri.Showscripman("Debe seleccionar una gabinete ", UpdatePanel_gabinetes)
                    Exit Sub
                End If
                Result = refgabinete.SolicitaPermisosGeneralesGabinete(HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"),
                                                                       HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                       HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                       stru_permiso)
                If Result <> "YES" Then
                    scri.Showscripman("Imposible consultar " & Result, UpdatePanel_gabinetes)
                    Exit Sub
                End If
                If stru_permiso.CONSULTA_IMAGEN = 0 Then
                    scri.Showscripman("El usuario no tiene permisos para consultar el gabinete ", UpdatePanel_gabinetes)
                    Exit Sub
                End If
                Me.ifimpre_consulta_documento_.Attributes.Add("SRC", "../Docuarchi/WebFormDaConsultaDocumento.aspx")
                Me.ModalPopup_consulta_documento.Show()
                Me.UpdatePaneliframe_consulta_documento.Update()

            End If
        Catch ex As Exception
            Session.Item("DA_GABINETE_CONSULTA") = ""
            scri.Showscripman(ex.Message, Me.UpdatePanel_gabinetes)
        End Try
    End Sub
End Class