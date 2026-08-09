Imports System.Drawing
Imports System.IO

Public Class WebFormReportesWorkflow
    Inherits System.Web.UI.Page

    Dim Conectio_Documnent As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim clasjava As New Classscrripjava
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            Dim Datos_Nodo As String = ""
            Dim Result As String = ""
            Dim Ref As New ClassWorkflowReportes
            Dim ref_class_reportes_workflow As New Class_reportes_workflow
            If Not Page.IsPostBack Then
                Me.TreeView1.Nodes.Clear()
                Result = Ref.Listar_Reportes_Grupos_Treview(Me.TreeView1, _
                                                            Session.Item("Id_Usuario_Workflow"), "")
                If Result <> "YES" Then
                    label_result.Text = Result
                End If
                ifmExcel_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                updatapanel_iframe.Update()
            End If
            If Page.IsPostBack Then
                Dim Matri_Nodo() As String
                Erase Matri_Nodo
                Result = Ref.NodoChild_Selecionado(Me.TreeView1, Datos_Nodo)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, update_tre_principal)
                    Exit Sub
                Else
                    Matri_Nodo = Split(Datos_Nodo, "|")
                End If
                Result = ""
                If Not Matri_Nodo Is Nothing Then
                    If Matri_Nodo.Length = 1 Then
                        Exit Sub
                    End If
                    'consulta el codigo de consulta
                    Result = ref_class_reportes_workflow.Datos_Sql_Reporte(Matri_Nodo(0), _
                                                                           Session.Item("Dato_Sql"))
                    If Result <> "YES" Then
                        clasjava.Showscripman(Result, update_tre_principal)
                        Exit Sub
                    End If
                    If Session.Item("Dato_Sql") <> "" Then
                        Dim MatriSql() As String = Split(Session.Item("Dato_Sql"), "//")
                        Session.Item("Dato_Sql_Consulta") = MatriSql(0)
                        Dim MatriParametros() As String
                        Erase MatriParametros
                        'verifica que tenga parametros la consulta
                        If Not MatriSql Is Nothing And UBound(MatriSql) > 0 Then
                            MatriParametros = Split(MatriSql(1), "#")
                        End If
                        If Not MatriParametros Is Nothing Then
                            If MatriParametros(0) <> "" Then
                                Result = ref_class_reportes_workflow.Crear_Parametro_consulta(MatriParametros, _
                                                                                              Me.Page)
                                If Result <> "YES" Then
                                    clasjava.Showscripman(Result, update_tre_principal)
                                    Exit Sub
                                End If
                            Else
                                Me.Tableparametro.Controls.Clear()
                                Me.Button_reporte.Visible = True
                                Me.UpdatePanel_parametros.Update()
                            End If

                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            label_result.Text = ex.Message
        End Try
    End Sub
    
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            Dim dat As Date
            dat = Now
            If Me.Hidden_colum_header.Value = "" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman("No hay resultados para exportar ", Me.UpdatePanel_consulta)
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((100 * Rnd()) + 1))
            Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + "/reportes/" + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + "/")
            If Directory.Exists(ruta_create) = False Then
                Directory.CreateDirectory(ruta_create)
            End If
            Dim Ref As New ClassReportesRadicado
            Dim nombre_reporte As String = "Reportes workflow"
            Dim Datos_Nodo = ""
            Dim Matri_Nodo() As String
            Erase Matri_Nodo
            Result = Ref.NodoChild_Selecionado(Me.TreeView1, _
                                               Datos_Nodo)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_consulta)
                Exit Sub
            Else
                If Datos_Nodo <> "" Then
                    Matri_Nodo = Split(Datos_Nodo, "|")
                End If
            End If
            Result = ""
            If Not Matri_Nodo Is Nothing Then
                nombre_reporte = UCase(Matri_Nodo(1))
            End If
            Dim ruta_archivo As String = ruta_create + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + value.ToString + "test.xls"
            Result = Refclasreposte.genera_xls(Me.GridView_val_radicacion, ".xls", _
                                               ruta_archivo, Hidden_colum_header.Value, _
                                               nombre_reporte, Session.Item("GA_LOGINUSUARIOGESTION"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.UpdatePanel_consulta)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + "/reportes/" + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + "/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & value.ToString + "test.xls"
                    updatapanel_iframe.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_consulta)
        End Try
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim refjava As New Classscrripjava
        Try
            Dim Ref As New ClassWorkflowReportes
            Dim Result As String = ""
            Result = Ref.Limpiar_Resultado_consulta(Me.Page)
            If Result <> "YES" Then
                'refjava.Showscripman(Result, Me.update_tre_principal)
                'Exit Sub
            End If
        Catch ex As Exception
            refjava.Showscripman(ex.Message, update_tre_principal)
        End Try
    End Sub

    Private Sub Button_reporte_Click(sender As Object, e As EventArgs) Handles Button_reporte.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Datos_Nodo As String = ""
            Dim Result As String = ""
            Dim Ref As New ClassWorkflowReportes
            Dim ref_class_reportes_workflow As New Class_reportes_workflow
            Dim Matri_Nodo() As String
            Erase Matri_Nodo
            Result = Ref.NodoChild_Selecionado(Me.TreeView1, Datos_Nodo)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, update_tre_principal)
                Exit Sub
            Else
                Matri_Nodo = Split(Datos_Nodo, "|")
            End If
            Result = ""
            If Not Matri_Nodo Is Nothing Then
                'consulta el codigo de consulta
                Result = ref_class_reportes_workflow.Datos_Sql_Reporte(Matri_Nodo(0), _
                                                                       Session.Item("Dato_Sql"))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, update_tre_principal)
                    Exit Sub
                End If
                Dim MatriSql() As String = Split(Session.Item("Dato_Sql"), "//")
                Result = Ref.Resultado_consulta(Me.Page, _
                                                Trim(MatriSql(0)), _
                                                Matri_Nodo(1))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, update_tre_principal)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, update_tre_principal)
        End Try
    End Sub
End Class