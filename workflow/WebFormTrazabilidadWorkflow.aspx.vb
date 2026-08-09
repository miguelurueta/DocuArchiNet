Imports System.IO

Public Class WebFormTrazabilidadWorkflow
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If

        Dim Refclas As New ClassWorkflow
        Dim Result As String = ""
        If Me.IsPostBack = False Then
            Result = Refclas.Retorna_trazabilidad_radicado(Me.Page, _
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
            Result = Refclasreposte.genera_xls_buton(Me.GridView_val_radicacion, _
                                                     ".xls", ruta_archivo, _
                                                     Hidden_colum_header.Value, _
                                                     "TRAZABILIDAD RADICADO (" & Session.Item("PU_TRAZABILIDAD") & ")", _
                                                     Session.Item("GA_LOGINUSUARIOGESTION"), _
                                                     HttpContext.Current.Session.Item("DATA_SET_SESION_TRAZA_RAD"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("RA_RUTA_TEMPO") + "/reportes/" + "PUBLICO" + "/" & HttpContext.Current.User.Identity.Name & value.ToString + "test.xls"
                    ifmExcel_.Attributes.Add("src", "../workflow/WebFormDescargaRadicadowf.aspx")
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
            Dim Refclas As New ClassWorkflow
            If Me.IsPostBack = False Then
                Result = Refclas.Retorna_trazabilidad_radicado(Me.Page, Session.Item("PU_TRAZABILIDAD"))
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                End If
               
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub

    Private Sub GridView_val_radicacion_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_val_radicacion.RowCreated
        e.Row.Cells(1).Visible = False
    End Sub

    Private Sub Button_detalle_Click(sender As Object, e As EventArgs) Handles Button_detalle.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas_wf_ruta As New Class_worflow_rutas
            If Val(Me.hdnEmailID_VAL.Value) = -1 Then
                Exit Sub
            End If
            Dim id_estado As Long = Val(Me.hdnEmailID_VAL.Value)
            Result = Refclas_wf_ruta.Dibuja_detalle_conector_trazabilidad(id_estado, _
                                                                          Me.TextBox_fecha_inici.Text, _
                                                                          Me.TextBox_Fecha_Seleccion_.Text, _
                                                                          Me.TextBox_Fecha_Fin_.Text, _
                                                                          Me.TextBox_Duracion_Inicio_Seleccion_.Text, _
                                                                          Me.TextBox_Duracion_Seleccion_Fin_.Text, _
                                                                          Me.TextBox_usuario_asignado_.Text, _
                                                                          Me.TextBox_cargo_usuario_asignado_.Text, _
                                                                          Me.TextBox_id_Estado_.Text, _
                                                                          Me.UpdatePane_detalle_conector_trazabilidad)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            Else
                Me.ModalPopupExtender_detalle_conector_trazabilidad.Show()
            End If

        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub
End Class