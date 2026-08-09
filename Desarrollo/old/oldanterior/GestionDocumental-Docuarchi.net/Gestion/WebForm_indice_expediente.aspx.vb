Imports System.IO

Public Class WebForm_indice_expediente
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then
            Dim Class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
            Dim Result As String = ""
            Session.Item("SortExpression_expediente_indice") = "id_cert_indice_expediente"
            Session.Item("SortDirection_expediente_indice") = "ASC"
            Result = Class_ra_cert_indice_expediente.Consulta_indice_expediente(Session.Item("CERT_ID_EXPEDIENTE_INDICE"), _
                                                                                 Me.data_grid_listado_solicitudes, _
                                                                                 Me.Label_titulo_listado_solicitudes, _
                                                                                 Me.hdnEmailID, _
                                                                                 UpdateGeneral, _
                                                                                 "", _
                                                                                 Me.UpdatePanel_title, _
                                                                                 1, _
                                                                                 "", _
                                                                                 Session.Item("SortExpression_expediente_indice"), _
                                                                                 Session.Item("SortDirection_expediente_indice"))
            Me.Label_titulo_listado.Text = "Expediente :" & Session.Item("CERT_ID_EXPEDIENTE_INDICE")
            If Result <> "YES" Then
                Me.Label_titulo_listado.Text = Result
            End If
        End If
    End Sub

    Private Sub data_grid_listado_solicitudes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_solicitudes.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_listado_solicitudes.PageIndex = e.NewPageIndex
            Dim Class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
            Dim Result As String = ""
            Result = Class_ra_cert_indice_expediente.Consulta_indice_expediente(Session.Item("CERT_ID_EXPEDIENTE_INDICE"), _
                                                                                 Me.data_grid_listado_solicitudes, _
                                                                                 Me.Label_titulo_listado_solicitudes, _
                                                                                 Me.hdnEmailID, _
                                                                                 UpdateGeneral, _
                                                                                 "", _
                                                                                 Me.UpdatePanel_title, _
                                                                                 1, _
                                                                                 "", _
                                                                                 Session.Item("SortExpression_expediente_indice"), _
                                                                                 Session.Item("SortDirection_expediente_indice"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdateGeneral)
            Else
                Me.hdnEmailID.Value = "-1"
                Me.UpdateGeneral.Update()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_actualiza_lista_indice_Click(sender As Object, e As EventArgs) Handles Button_actualiza_lista_indice.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
            Dim Result As String = ""
            Result = Class_ra_cert_indice_expediente.Consulta_indice_expediente(Session.Item("CERT_ID_EXPEDIENTE_INDICE"), _
                                                                                 Me.data_grid_listado_solicitudes, _
                                                                                 Me.Label_titulo_listado_solicitudes, _
                                                                                 Me.hdnEmailID, _
                                                                                 UpdateGeneral, _
                                                                                 "", _
                                                                                 Me.UpdatePanel_title, _
                                                                                 1, _
                                                                                 "", _
                                                                                 Session.Item("SortExpression_expediente_indice"), _
                                                                                 Session.Item("SortDirection_expediente_indice"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.update_botonoes_opciones_solicitud_general)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.update_botonoes_opciones_solicitud_general)
        End Try
    End Sub

    Private Sub Button_descargar_archivo_Click(sender As Object, e As EventArgs) Handles Button_descargar_archivo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_ClassGaExpediente As New ClassGaExpediente
            Dim Ruta_expediente As String = ""
            Result = Ref_ClassGaExpediente.Solicita_ruta_xml_expediente(Session.Item("CERT_ID_EXPEDIENTE_INDICE"), _
                                                                       Ruta_expediente)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.update_botonoes_opciones_solicitud_general)
                Exit Sub
            End If
            Hidden_ruta_archivo.Value = Ruta_expediente
            ifmExcel_.Attributes.Add("src", "..\Docuarchi\WebFormDaDescarga.aspx")
            updatapanel_iframe.Update()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.update_botonoes_opciones_solicitud_general)
        End Try
    End Sub

    Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
        Try
            'e.Row.Cells(0).Visible = False
            e.Row.Cells(0).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_descarga_listado_Click(sender As Object, e As EventArgs) Handles Button_descarga_listado.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            Dim dat As Date
            dat = Now
            If Me.Hidden_colum_header.Value = "" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman("No hay resultados para exportar", Me.update_botonoes_opciones_solicitud_general)
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((100 * Rnd()) + 1))
            Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + "/reportes/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + "/")
            If Directory.Exists(ruta_create) = False Then
                Directory.CreateDirectory(ruta_create)
            End If
            Dim Ref As New ClassReportesRadicado
            Dim nombre_reporte As String = "INDICE EXPEDIENTE " & Session.Item("CERT_ID_EXPEDIENTE_INDICE")
            Dim ruta_archivo As String = ruta_create & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "exp" & Session.Item("CERT_ID_EXPEDIENTE_INDICE") & ".xls"
            Result = Refclasreposte.genera_xls(Me.data_grid_listado_solicitudes, ".xls",
                                               ruta_archivo,
                                               Hidden_colum_header.Value,
                                               nombre_reporte,
                                               Session.Item("GA_LOGINUSUARIOGESTION"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.update_botonoes_opciones_solicitud_general)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = ruta_archivo
                    ifmExcel_.Attributes.Add("src", "..\Docuarchi\WebFormDaDescarga.aspx")
                    Me.updatapanel_iframe.Update()

                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.update_botonoes_opciones_solicitud_general)
        End Try
    End Sub
End Class