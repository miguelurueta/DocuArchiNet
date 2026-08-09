Public Class WebWorkflowIntervaloActualizacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.IsPostBack = False Then
            Me.DropDownListIntervalo.Items.Add("-1")
            For i As Integer = 5 To 100 Step 5
                Me.DropDownListIntervalo.Items.Add(i)
            Next
            Dim Refclas As New InicioWorkflow
            Dim Refscrip As New Classscrripjava
            Dim Itervalo As Integer = -1
            Dim Result = Refclas.Retorna_Intervalo_Actualizacion_Usario_Rtur(Session.Item("Id_Usuario_Workflow"), Itervalo)
            If Result <> "YES" Then
                Refscrip.Show(Result)
                Exit Sub
            End If
            Me.DropDownListIntervalo.Text = Itervalo
        Else

        End If
    End Sub

    Protected Sub ButtonAceptar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonAceptar.Click
        Dim Refclas As New InicioWorkflow
        Dim Refscrip As New Classscrripjava
        Dim Refclasuser As New ClassWorkflowUsuario
        Try
            Dim Result As String = ""
            Dim Itervalo As Integer = -1
            Result = Refclasuser.Atualizar_Intervalo_Usuario(Session.Item("Id_Usuario_Workflow"), Me.DropDownListIntervalo.Text)
            If Result <> "YES" Then
                Refscrip.Showscripman(Result, Me.Updatepanel_Boton)
                Exit Sub
            Else
                'Refscrip.Show("El sistema cambio el intervalo, Debe cerrar sesión para aplicar cambios")
            End If
        Catch ex As Exception
            Refscrip.Showscripman(ex.Message, Me.Updatepanel_Boton)
        End Try
    End Sub
End Class