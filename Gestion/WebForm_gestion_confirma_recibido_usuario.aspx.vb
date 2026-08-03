Imports System.IO

Public Class WebForm_gestion_confirma_recibido_usuario
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim parameter As String = ""
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim clasjava As New Classscrripjava
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
               
            End If
            If Page.IsPostBack = False Then
                Dim ruta As String = HttpContext.Current.Application.Item("")
                Dim query_strin As String = Context.Request.QueryString("path_confir").Replace(" ", "+")
                Dim Result = encriptacion.desc_encript_md5(query_strin, _
                                                           "7894561230!", _
                                                           parameter)
                If Result <> "YES" Then
                    Context.Response.Write(Result & parameter & " encript " & query_strin)
                    Exit Sub
                End If
                Dim split_parameter() As String = parameter.Split("|")
                Dim class_gestor_sesion As New GestorModuleSesion.Gestor_conexion
                Result = class_gestor_sesion.Asigna_detalle_inicio_confirmacion(split_parameter(0))
                If Result <> "YES" Then
                    Context.Response.Write(Result)
                    Exit Sub
                End If
                Session.Item("GA_ID_RESPUESTA_CONFIRMACION") = split_parameter(1)
                Me.Label_mensaje.Text = split_parameter(0) & " le invita a confirmar el recibido de la respuesta a su radicado (" & split_parameter(2) & ")"
            End If
        Catch ex As Exception
            Context.Response.Write(ex.Message & " parameter " & parameter)
        End Try
    End Sub

    Private Sub Button_confirma_Click(sender As Object, e As EventArgs) Handles Button_confirma.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclass As New Class_ra_respuesta_radicado
            Dim estado_confirmacion As String = ""
            Result = Refclass.Solicita_estado_recibido_respuesta_usuario(Session.Item("GA_ID_RESPUESTA_CONFIRMACION"), _
                                                                         estado_confirmacion)
            If estado_confirmacion = "YES" Then
                Refclasjava.Showscripman("Ya se había confirmado antes la respuesta, gracias por intentar confirmar", Me.UpdatePanel_confir)
                Exit Sub
            End If
            Result = Refclass.Actualiza_estado_recibido_respuesta(Val(Session.Item("GA_ID_RESPUESTA_CONFIRMACION")))
            If Result = "YES" Then
                Refclasjava.Showscripman("Gracias por confirmar el recibido de la respuesta", Me.UpdatePanel_confir)
            Else
                Refclasjava.Showscripman(Result, Me.UpdatePanel_confir)
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_confir)
        End Try
    End Sub
End Class