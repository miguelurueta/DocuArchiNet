Public Class WebFormEnviaActividad
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            If Page.IsPostBack = False Then
                Dim refclas As New ClassWorkflow
                refclas.Lista_Actividades(Me.GridViewlista)
            End If

        Catch ex As Exception

        End Try
       

    End Sub

    Private Sub WebFormPendiente_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        
    End Sub

    Private Sub UpdatePanellista_Load(sender As Object, e As EventArgs) Handles UpdatePanellista.Load
       
    End Sub

    Private Sub GridViewlista_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridViewlista.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try

    End Sub

    
End Class