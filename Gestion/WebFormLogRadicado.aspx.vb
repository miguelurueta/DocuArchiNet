Imports System.IO

Public Class WebFormLogRadicado
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If

        Dim Refclas As New Classgestionrespuesta
        Dim Result As String = ""
        If Me.IsPostBack = False Then
            Result = Refclas.Retorna_log_radicado(Me.Page, _
                                                  Session.Item("PU_TRAZABILIDAD"))
            If Result <> "YES" Then
                Label_estado_transac.Text = Result
            End If

        End If

    End Sub

    Protected Sub Button_Exportar_Radicados_Click(sender As Object, e As EventArgs) Handles Button_Exportar_Radicados.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            Dim dat As Date
            dat = Now
            'Refclasreposte.ExportToExcel(Me.GridView_val_radicacion)
            If Me.Hidden_colum_header.Value = "" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman("No hay resultados para exportar", Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((100000 * Rnd()) + 1))
            Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + "/reportes/" + "PUBLICO" + "/")
            If Directory.Exists(ruta_create) = False Then
                Directory.CreateDirectory(ruta_create)
            End If
            Dim ruta_archivo As String = ruta_create + HttpContext.Current.User.Identity.Name + value.ToString + "test.xls"
            Result = Refclasreposte.genera_xls(Me.GridView_val_radicacion, _
                                               ".xls", _
                                               ruta_archivo, _
                                               Hidden_colum_header.Value, _
                                               "REGISTRO LOG RADICADOS", _
                                               Session.Item("GA_LOGINUSUARIOGESTION"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + "/reportes/" + "PUBLICO" + "/" & HttpContext.Current.User.Identity.Name & value.ToString + "test.xls"
                    ifmExcel_.Attributes.Add("src", "../Gestion/WebFormDescargaRadicadogd.aspx")
                    updatapanel_iframe.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            Dim Refclas As New Classgestionrespuesta
            If Me.IsPostBack = False Then
                Result = Refclas.Retorna_log_radicado(Me.Page, Session.Item("PU_TRAZABILIDAD"))
                If Result <> "YES" Then
                    Label_estado_transac.Text = Result
                End If

            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub

End Class