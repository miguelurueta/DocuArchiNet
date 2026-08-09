Imports System.IO

Public Class WebFormDetalleRadicado
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If IsPostBack = False Then
            Dim Refclas As New Classgestionrespuesta
            Dim Result As String = ""
            If Me.IsPostBack = False Then
                Result = Refclas.Retorna_combo_respuesta_tramite(Session.Item("PU_TRAZABILIDAD"), Me.DropDownList_detalle_respuesta)
                If Result <> "YES" Then
                    Label_estado.Text = Result
                End If
                If Me.DropDownList_detalle_respuesta.Items.Count > 0 Then
                    Result = Refclas.Retorna_detalle_respuesta_radicado(Me.DropDownList_detalle_respuesta.Items(0).Text, _
                                                                        Me.Page, _
                                                                        Session.Item("PU_TRAZABILIDAD"))
                    If Result <> "YES" Then
                        Label_estado.Text = Result
                    End If
                End If
                

            End If
        End If
    End Sub

    Private Sub Button_generar_Click(sender As Object, e As EventArgs) Handles Button_generar.Click
        Dim Result As String = ""
        Dim struc_envio As stru_envio = Nothing
        Dim refclasgestion As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.DropDownList_detalle_respuesta.Text, _
                                                                                        struc_envio)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, Me.UpdatePanel_botones_registro)
                Exit Sub
            End If
            If struc_envio.RADICADO Is Nothing Then
                scrijava.Showscripman("El radicado " & Session.Item("PU_TRAZABILIDAD") & " no tiene una respuesta relacionada", Me.UpdatePanel_botones_registro)
                Exit Sub
            End If
            Dim archivo As String = ""
            Result = refclasgestion.Genera_archivo_detalle_respuesta(struc_envio.ID_RESPUESTA_RADICADO, _
                                                                     archivo)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, Me.UpdatePanel_botones_registro)
                Exit Sub
            End If
            If archivo <> "" Then
                Dim fileinf As New FileInfo(archivo)
                If File.Exists(archivo) Then
                    Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/PUBLIC/DESCARGA/")
                    Dim filecopia As String = ruta_local & fileinf.Name
                    'If File.Exists(filecopia) Then
                    '    Kill(filecopia)
                    'End If
                    'File.Move(archivo, filecopia)
                    Me.Hidden_ruta_archivo.Value = "../Temp_Image/PUBLIC/DESCARGA/" & fileinf.Name
                    ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                    updatapanel_iframe.Update()
                End If
            End If
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, Me.UpdatePanel_botones_registro)
        End Try
    End Sub

    Private Sub Button_descarga_respuesta_Click(sender As Object, e As EventArgs) Handles Button_descarga_respuesta.Click
        Dim Result As String = ""
        Dim struc_envio As stru_envio = Nothing
        Dim refclasgestion As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Dim refclasdescargapublic As New Classdescargapublico
        Try
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(Me.DropDownList_detalle_respuesta.Text, struc_envio)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, Me.UpdatePanel_botones_registro)
                Exit Sub
            End If
            If struc_envio.RADICADO Is Nothing Then
                scrijava.Showscripman("El radicado " & Session.Item("PU_TRAZABILIDAD") & " no tiene una respuesta relacionada", Me.UpdatePanel_botones_registro)
                Exit Sub
            End If
            If struc_envio.ID_IMAGEN_RESPUESTA = 0 Then
                scrijava.Showscripman("El radicado " & Session.Item("PU_TRAZABILIDAD") & " no tiene documento de respuesta relacionado", Me.UpdatePanel_botones_registro)
                Exit Sub
            End If
            Result = refclasdescargapublic.Descarga_documento_respuesta_pdf_public(struc_envio.ID_RESPUESTA_RADICADO,
                                                                                   Me.ifmExcel_,
                                                                                   Me.updatapanel_iframe,
                                                                                   Me.Hidden_ruta_archivo)
            If Result <> "YES" Then
                scrijava.Showscripman(Result, Me.UpdatePanel_botones_registro)
                Exit Sub
            End If
        Catch ex As Exception
            scrijava.Showscripman(ex.Message, Me.UpdatePanel_botones_registro)
        End Try
    End Sub

    Private Sub Button_activa_detalle_tramite_Click(sender As Object, e As EventArgs) Handles Button_activa_detalle_tramite.Click
        Dim Refclas As New Classgestionrespuesta
        Dim Result As String = ""
        Dim refclasjava As New Classscrripjava
        Try
            
            If Me.DropDownList_detalle_respuesta.Items.Count > 0 Then
                Result = Refclas.Retorna_detalle_respuesta_radicado(Me.DropDownList_detalle_respuesta.Text, Me.Page, Session.Item("PU_TRAZABILIDAD"))
                If Result <> "YES" Then
                    refclasjava.Showscripman(Result, UpdatePanel_boton_activa_detalle)
                    Exit Sub
                Else

                End If
                UpdatePanel_detalle.Update()
            End If

        Catch ex As Exception
            refclasjava.Showscripman(ex.Message, UpdatePanel_boton_activa_detalle)
        End Try
    End Sub

    
End Class