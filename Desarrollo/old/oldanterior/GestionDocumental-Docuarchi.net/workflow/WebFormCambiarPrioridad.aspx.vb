Public Class WebFormCambiarPrioridad
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim Ref As New ClassWorkflow
        Dim Result As String = ""
        Dim Valor_Estado As String = ""
        If Page.IsPostBack = False Then
            Result = Ref.Lista_Estado_Prioridad_workflow(Valor_Estado)
            If Result <> "YES" Then
            Else
                Select Case Valor_Estado
                    Case 5
                        Me.RadioButtonUrgente.Checked = True
                    Case 4
                        Me.RadioButtonmediourgente.Checked = True
                    Case 3
                        Me.RadioButtonsemiurgente.Checked = True
                    Case 0
                        Me.RadioButtonEstadonormal.Checked = True
                End Select
            End If
           
        End If
    End Sub

    Protected Sub Buttonaplicar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Buttonaplicar.Click
        Dim Ref As New ClassWorkflow
        Dim Refclaslist As New ClassListandoTareas
        Dim refclaescr As New Classscrripjava
        Dim Result As String = ""
        Dim Valor_Estado As String = ""
        Dim IdAct As String = ""
        Dim Ref_Class_grupos_workflow As New Class_grupos_workflow
        Result = Ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(IdAct, _
                                                                                  HttpContext.Current.Session("Id_Grupo_Workflow"))
        If Result <> "YES" Then
            refclaescr.Showscripman(Result, Me.Updategenral)
            Exit Sub
        End If
        If Me.RadioButtonUrgente.Checked = True Then
            Valor_Estado = 5
        End If
        If Me.RadioButtonmediourgente.Checked = True Then
            Valor_Estado = 4
        End If
        If Me.RadioButtonsemiurgente.Checked = True Then
            Valor_Estado = 3
        End If
        If Me.RadioButtonEstadonormal.Checked = True Then
            Valor_Estado = 0
        End If
        Result = ""
        Result = Ref.Cambiar_Estado_Prioridad_Tarea(IdAct, HttpContext.Current.Session("Id_Usuario_Workflow").ToString, HttpContext.Current.Session("ID_TAREA_SELECCIONDA"), Valor_Estado)
        If Result <> "YES" Then
            refclaescr.Showscripman(Result, Me.Updategenral)
        Else
            Me.hdnEmailID.Value = "1"
            Me.Updategenral.Update()

        End If
    End Sub
End Class