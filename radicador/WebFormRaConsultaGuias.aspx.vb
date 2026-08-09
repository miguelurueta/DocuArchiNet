Imports System.IO

Public Class WebFormRaConsultaGuias
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            Dim Result As String = ""
            Dim scripjava As New Classscrripjava
            Dim refclas As New ClassRaEnvioCorrespondencia
            Dim refclasradicado As New ClassRadicador
            If Me.IsPostBack = False Then
                Dim Refclas_rad As New ClassInicioRadicador
                Result = Refclas_rad.Crea_Dir_Temporal_ra()
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & Result & vbCrLf
                    Label_result.Text = Result & vbCrLf
                Else
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Directorio temporal : " & Result & vbCrLf
                End If
                Result = refclas.Lista_empresa_envio(Me.DropDownList_empresa_envio, Me.DropDownList_empresa_envio.Text)
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas.Retorna_operarios_mensajeria_gestion(Me.DropDownList_mensajero_inerno, Me.DropDownList_mensajero_inerno.Text)
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas.Retorna_areas_permitidas_para_envio_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), Me.DropDownList_areas_depart, Me.UpdatePanelContenido_val_radicacion)
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclasradicado.Lista_Destinatario_Interno(Me.DropDownList_nombre_remitente, "")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Dim refclas_radic As New ClassRadicador
                Result = refclas_radic.agregar_auto_complete(Me.TextBox_Id_guia_envio.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_guia_interna", "Id_guia_envio")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas_radic.agregar_auto_complete(Me.TextBox_Concecutivo_Guia.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_guia_interna", "Concecutivo_Guia")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas_radic.agregar_auto_complete(Me.TextBox_NOMBRE_RAZON_SOCIA.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_guia_interna", "NOMBRE_RAZON_SOCIAL")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
                Result = refclas_radic.agregar_auto_complete(Me.TextBox_NIT_IDENTIFICACION_2.ID.ToString, Me._Panelvalidacion_val_radicacion, "GetGuiaRadicaconasp", "ra_guia_interna", "NIT_IDENTIFICACION")
                If Result <> "YES" Then
                    Label_result.Text = Label_result.Text & Result & vbCrLf
                End If
            End If
        Catch ex As Exception
            Label_result.Text = Label_result.Text & ex.Message
        End Try
    End Sub

    Protected Sub Button_consulta_pendientes_procesar_Click(sender As Object, e As EventArgs) Handles Button_consulta_pendientes_procesar.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Result = refclas.Lista_guia_envio_correspondencia(Me.Page)
        If Result <> "YES" Then
            scripjava.Showscripman(Result, Me.UpdatePanel_botones_validacion)
            Exit Sub
        End If
    End Sub

    Protected Sub Button_lipiar_val_radicacion_Click(sender As Object, e As EventArgs) Handles Button_lipiar_val_radicacion.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Result = refclas.Limpiar_campos_consulta_guia(Me.Page)
        If Result <> "YES" Then
            scripjava.Showscripman(Result, Me.UpdatePanel_botones_validacion)
            Exit Sub
        End If
    End Sub

    Protected Sub Button_descargar_guia_Click(sender As Object, e As EventArgs) Handles Button_descargar_guia.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try

            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman("Debe seleccionar el regitro a procesar", Me.UpdatePanel_botones_radicacion)
                Hidden_procesa_tramite_envio.Value = ""
                Exit Sub
            End If
            Dim id_guia_tramite As Integer = Me.hdnEmailID_VAL.Value
            'Result = refclas.retorna_consecutivo_guia_respuesta_radicado(Me.hdnEmailID_VAL.Value, id_guia_tramite)
            'If Result <> "YES" Then
            '    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
            '    Exit Sub
            'End If

            'If id_guia_tramite = 0 Then
            '    scripjava.Showscripman("No registra guía para exportar", Me.UpdatePanel_botones_radicacion)
            '    Exit Sub
            'End If
            Dim archivo As String = ""
            Result = refclas.genera_documento_guia(id_guia_tramite, archivo)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            Else
                If archivo <> "" Then
                    Dim fileinf As New FileInfo(archivo)
                    If File.Exists(archivo) Then
                        Dim filecopia As String = HttpContext.Current.Session.Item("RA_RUTA_TEMPO_DESCARGA") & "\" & "file_temp" & fileinf.Extension
                        If File.Exists(filecopia) = True Then
                            File.Delete(filecopia)
                        End If
                        File.Copy(archivo, filecopia)
                        File.Delete(archivo)
                        If File.Exists(filecopia) = True Then
                            UpdatePanel_botones_radicacion.Update()
                            Hidden_ruta_archivo.Value = "../Temp_Radicacion/" & HttpContext.Current.Session.Item("RA_ID_USUARIO").ToString & "/DESCARGA/" & "file_temp" & fileinf.Extension
                            ifmExcel_.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                            updatapanel_iframe.Update()

                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub
End Class