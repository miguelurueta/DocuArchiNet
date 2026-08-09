Public Class WebWorkflowIntervaloAlarma
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.IsPostBack = False Then
            Me.DropDownListIntervalo.Items.Add("-1")
            For i As Integer = 5 To 100 Step 5
                Me.DropDownListIntervalo.Items.Add(i)
            Next
            Dim Refclas As New ClassWorkflowUsuario
            Dim Refscrip As New Classscrripjava
            Dim Itervalo As Integer = -1
            Dim Result = Refclas.Intervalo_Alarma_Usuario(Itervalo)
            If Result <> "YES" Then
                Refscrip.Show(Result)
                Exit Sub
            End If
            Me.DropDownListIntervalo.Text = Itervalo
        Else

        End If
    End Sub

    Protected Sub ButtonAceptar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonAceptar.Click
        Dim Refclas As New ClassWorkflowUsuario
        Dim Refscrip As New Classscrripjava
        Dim Result As String = ""
        Dim Itervalo As Integer = -1
        Try
            Result = Refclas.Intervalo_Alarma_Usuario(Itervalo)
            If Result <> "YES" Then
                Refscrip.Show(Result)
                Exit Sub
            End If
            If Itervalo = -1 Then
                Result = Refclas.S_Insercion_Intervalo_Alarma(Session.Item("Id_Usuario_Workflow"), _
                                                              Session.Item("Login_Usuario_Workfow"), _
                                                              Me.DropDownListIntervalo.Text)
                If Result <> "YES" Then
                    Refscrip.Showscripman(Result, Updatepanel_Boton)
                Else
                    'Refscrip.Show("El sistema cambio el intervalo, Debe cerrar sesión para aplicar cambios")
                End If
            Else
                Result = Refclas.S_Actualizacion_Intervalo_Alarma(Session.Item("Id_Usuario_Workflow"), Me.DropDownListIntervalo.Text)
                If Result <> "YES" Then
                    Refscrip.Showscripman(Result, Updatepanel_Boton)
                Else
                    'Refscrip.Show("El sistema cambio el intervalo, Debe cerrar sesión para aplicar cambios")
                End If
            End If
        Catch ex As Exception
            Refscrip.Showscripman(ex.Message, Updatepanel_Boton)
        End Try
    End Sub
End Class