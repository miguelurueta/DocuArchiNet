Public Class WebFormAnotacion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then    
            Dim refclas As New Class_anotacion_tarea
            refclas.Listar_Anotaciones_tarea_workflow(Me.GridViewlista, _
                                                      HttpContext.Current.Session("ID_TAREA_SELECCIONDA"))
        End If

    End Sub

    Private Sub WebFormAnotacion_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
       
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim scr As [String] = "$(document).ready(function () {$().cligred();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)

        End If
       
       
    End Sub

    Private Sub ButtonGuardar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonGuardar.Click
        Dim Result As String = ""
        Dim Ref_Class_anotacion_tarea As New Class_anotacion_tarea
        Dim refmensaje As New Classscrripjava
        Try
            Hidden_resultado_nota_add_update.Value = ""
            If Me.HiddenPROMP.Value = "1" Then Exit Sub
            If Me.TextBoxdatos.Text = "" Then
                refmensaje.Showscripman_menu("La anotacion no contiene informacion", Me.UpdatePanel_guardar_nota, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            'Result = Ref_Class_anotacion_tarea.Agregando_Anotacion(Me.TextBoxdatos.Text, _
            '                                                       Me.GridViewlista)
            'If Result <> "YES" Then
            '    refmensaje.Showscripman_menu(Result, Me.UpdatePanel_guardar_nota, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'Else
            '    Hidden_resultado_nota_add_update.Value = "YES"
            '    ModalPopupExtender_edition_nota_respuesta.Hide()
            'End If
        Catch ex As Exception
            refmensaje.Showscripman_menu(ex.Message, Me.UpdatePanel_guardar_nota, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridViewlista_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewlista.RowCreated
        Try
            e.Row.Cells(2).Visible = False
            e.Row.Cells(1).Visible = False
            e.Row.Cells(5).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ButtonActualizar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonActualizar.Click
        Dim refmensaje As New Classscrripjava
        Try
            Hidden_resultado_nota_add_update.Value = ""
            hidden_campos_dinamicos_aleas.Value = "DATO_ANOTACION|"
            hidden_valore_campos.Value = Me.TextBoxdatos.Text
            If Me.HiddenPROMP.Value = "1" Then Exit Sub
            If Me.hdnEmailID.Value <> "0" And Me.hdnEmailID.Value <> "-1" Then
                Dim Ref As New Class_anotacion_tarea
                Dim Remens As New Classscrripjava
                Dim Result As String = ""
                'Result = Ref.Actualizar_Datos_Anotacion(Me.TextBoxdatos.Text, _
                '                                        Me.hdnEmailID.Value)
                'If Result <> "YES" Then
                '    refmensaje.Showscripman_menu(Result, Me.UpdatePanel_guardar_nota, "ModalPopupExtender_mensaje_personalizado")
                '    Exit Sub
                'End If
                ModalPopupExtender_edition_nota_respuesta.Hide()
                Hidden_resultado_nota_add_update.Value = "YES"

            End If
        Catch ex As Exception
            refmensaje.Showscripman_menu(ex.Message, Me.UpdatePanel_guardar_nota, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    'Protected Sub ButtonEliminar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonEliminar.Click
    '    Dim refmensaje As New Classscrripjava
    '    Try
    '        Hidden_resultado_eliminar_guia.Value = ""
    '        If Me.HiddenPROMP.Value = "1" Then Exit Sub
    '        If Me.hdnEmailID.Value <> "0" And Me.hdnEmailID.Value <> "-1" Then
    '            Dim Ref_Class_anotacion_tarea As New Class_anotacion_tarea
    '            Dim Remens As New Classscrripjava
    '            Dim Result As String = ""
    '            Dim Estado_propietario As String = ""
    '            Result = Ref_Class_anotacion_tarea.Solicita_estado_usuario_propietario_nota(HttpContext.Current.Session("Id_Usuario_Workflow"), _
    '                                                                                        Val(Me.hdnEmailID.Value), _
    '                                                                                        Estado_propietario)
    '            If Result <> "YES" Then
    '                refmensaje.Showscripman_menu(Result, Me.Updateboton, "ModalPopupExtender_mensaje_personalizado")
    '                Exit Sub
    '            End If
    '            If Estado_propietario = "NO" Then
    '                refmensaje.Showscripman_menu("El usuario no es el propietario de la nota, imposible eliminar", Me.Updateboton, "ModalPopupExtender_mensaje_personalizado")
    '                Exit Sub
    '            End If
    '            Result = Ref_Class_anotacion_tarea.Eliminar_nota_tarea(HttpContext.Current.Session("Id_Usuario_Workflow"), _
    '                                                                   Val(Me.hdnEmailID.Value))
    '            If Result <> "YES" Then
    '                refmensaje.Showscripman_menu(Result, Me.Updateboton, "ModalPopupExtender_mensaje_personalizado")
    '                Exit Sub
    '            Else
    '                Hidden_resultado_eliminar_guia.Value = "YES"
    '            End If

    '        End If
    '    Catch ex As Exception
    '        refmensaje.Showscripman_menu(ex.Message, Me.Updateboton, "ModalPopupExtender_mensaje_personalizado")
    '    End Try
    'End Sub


    Private Sub Buttonclidatos_Click(sender As Object, e As EventArgs) Handles Buttonclidatos.Click
        Dim Result As String = ""
        Dim Refclas As New Class_anotacion_tarea
        Dim refmensaje As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "0" Then
                Exit Sub
            End If
            Result = Refclas.Solicta_nota_tarea(Val(Me.hdnEmailID.Value), _
                                                Me.TextBoxdatos.Text)
            If Result <> "YES" Then
                refmensaje.Showscripman_menu(Result, Me.Updateboton, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Label_nota_respuesta.Text = "Contenido nota"
            ButtonGuardar.Visible = False
            Me.ButtonActualizar.Visible = True
            UpdatePanel_guardar_nota.Update()
            Me.UpdatePaneltextbos.Update()
            Me.ModalPopupExtender_edition_nota_respuesta.Show()
        Catch ex As Exception
            refmensaje.Showscripman_menu(ex.Message, Me.Updateboton, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_Show_Guardar_Click(sender As Object, e As EventArgs) Handles Button_Show_Guardar.Click
        Try
            Label_nota_respuesta.Text = "Guardar nota"
            ButtonGuardar.Visible = True
            Me.ButtonActualizar.Visible = False
            UpdatePanel_guardar_nota.Update()
            ModalPopupExtender_edition_nota_respuesta.Show()
        Catch ex As Exception

        End Try
       
    End Sub
End Class