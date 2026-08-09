Public Class WebFormDaPrincipal
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then

            'Dim Refclas_rel_usu_gabi As New Class_permisos_usuarios_gabinetes
            'Dim Refclas_rel_grupo_gabi As New Class_permisos_grupos_gabinetes
            'Dim Result As String = ""
            'Dim Refclas As New ClassDaGabinete
            'Result = Refclas.Retorna_gabinetes_permitidos(Session.Item("DA_gruposusu"), _
            '                                              HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"), _
            '                                              Me.DropDownList_gabinetes)
            'If Result <> "YES" Then
            '    Me.Label_estado.Text = Result
            '    Exit Sub
            'End If
            'Session.Item("DA_GABINETE_CONSULTA") = Me.DropDownList_gabinetes.Text
            'If Me.DropDownList_gabinetes.Text <> "" Then
            '    Result = Refclas.SolicitaPermisosSessionGabinete(Me.DropDownList_gabinetes.Text, _
            '                                               HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"))
            '    If Result <> "YES" Then
            '        Me.Label_estado.Text = Result
            '        Exit Sub
            '    End If
            'End If
        End If
    End Sub

    'Private Sub ButtonConsultar_Click(sender As Object, e As EventArgs) Handles ButtonConsultar.Click
    '    Dim scri As New Classscrripjava
    '    Try
    '        If Me.DropDownList_gabinetes.Text <> "" Then
    '            Dim Result As String = ""
    '            Dim refgabinete As New ClassDaGabinete
    '            Dim stru_permiso As stru_permiso_gabinete = Nothing

    '            If HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = "" Then
    '                'scri.Showscripman("Debe seleccionar una gabinete ", UpdatePanel_gabinetes)
    '                Exit Sub
    '            End If
    '            Result = refgabinete.SolicitaPermisosSessionGabinete(HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"), _
    '                                                                    HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"))
    '            If Result <> "YES" Then
    '                'scri.Showscripman("Imposible consultar " & Result, UpdatePanel_gabinetes)
    '                Exit Sub
    '            End If
    '            If HttpContext.Current.Session.Item("CONSULTA_IMAGEN") = 0 Then
    '                'scri.Showscripman("El usuario no tiene permisos para consultar el gabinete ", UpdatePanel_gabinetes)
    '                Exit Sub
    '            End If
    '            Me.ifimpre_consulta_documento_.Attributes.Add("SRC", "../Docuarchi/WebFormDaConsultaDocumento.aspx")
    '            Me.ModalPopup_consulta_documento.Show()
    '            Me.UpdatePaneliframe_consulta_documento.Update()

    '        End If
    '    Catch ex As Exception
    '        Session.Item("DA_GABINETE_CONSULTA") = ""
    '        'scri.Showscripman(ex.Message, Me.UpdatePanel_gabinetes)
    '    End Try
    'End Sub

    'Private Sub DropDownList_gabinetes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_gabinetes.SelectedIndexChanged
    '    Dim scri As New Classscrripjava
    '    Try
    '        Dim Refclas As New ClassDaGabinete
    '        Dim Result As String = ""
    '        Session.Item("DA_GABINETE_CONSULTA") = Me.DropDownList_gabinetes.Text
    '        Result = Refclas.SolicitaPermisosSessionGabinete(Me.DropDownList_gabinetes.Text, _
    '                                                            HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"))
    '        If Result <> "YES" Then
    '            'scri.Showscripman(Result, Me.UpdatePanel_gabinetes)
    '            Exit Sub
    '        End If
    '    Catch ex As Exception
    '        'scri.Showscripman(ex.Message, Me.UpdatePanel_gabinetes)
    '    End Try
    'End Sub
End Class