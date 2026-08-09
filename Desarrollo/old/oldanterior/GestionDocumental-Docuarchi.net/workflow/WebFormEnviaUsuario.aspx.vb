Public Class WebFormPendiente
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Page.IsPostBack = False Then
            Dim refclas As New ClassWorkflow
            refclas.Lista_Actividades_Usuario_listview(Me.GridViewlista)
            'Me.UpdatePanelgred.Update()
        End If
       
        ' Dim scr As [String] = "$(document).ready(function () {$().cligred();});"
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
    End Sub

    Private Sub WebFormPendiente_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
      
        'Dim comillas As String = Char.ConvertFromUtf32(34)
        'Dim script As [String] = "$(document).ready(function(){$('#" + GridViewlista.ClientID & "').Scrollable({ScrollHeight: 310,IsInUpdatePanel:false});});"
        'ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", GridViewlista.ClientID), script, True)
       
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If

    End Sub

    Private Sub GridViewlista_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowCreated
        e.Row.Cells(4).Visible = False
        e.Row.Cells(5).Visible = False
        e.Row.Cells(6).Visible = False
        Try
            Dim key As String = ""
            If (e.Row.RowType = DataControlRowType.DataRow) Then
                'Dim roc As Integer = GridViewlista.Rows.Count
                'key = e.Row.RowIndex.ToString & "|" & GridViewlista.Rows(e.Row.RowIndex).Cells(4).Text & "|" & GridViewlista.Rows(e.Row.RowIndex).Cells(5).Text
                'key = GridViewlista.DataKeys(e.Row.RowIndex).Value.ToString()
                'e.Row.Attributes.Add("id", key)

            End If
        Catch ex As Exception
            'MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub GridViewlista_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowDataBound
        
    End Sub

    Private Sub GridViewlista_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewlista.SelectedIndexChanged
        
    End Sub

    Private Sub WebFormPendiente_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        
    End Sub
    
    
    Private Sub UpdatePanelgred_Load(sender As Object, e As EventArgs) Handles UpdatePanelgred.Load
        ' Dim scr As [String] = "$(document).ready(function () {$().cligred();});"
        ' ScriptManager.RegisterStartupScript(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
    End Sub

  
End Class