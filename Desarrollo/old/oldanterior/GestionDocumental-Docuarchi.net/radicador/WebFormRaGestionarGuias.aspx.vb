Imports System.IO

Public Class WebFormRaGestionarGuias
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
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = 0
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, nombre_plantilla)
            If Result <> "YES" Then
                Label_result.Text = Label_result.Text & Result & vbCrLf
            Else
                If nombre_plantilla <> "" Then
                    Result = refclasradicado.agregar_auto_complete(Me.TextBox_RADICADO.ID.ToString, _Panelvalidacion_val_radicacion_, "GetGuiaRadicaconasp", nombre_plantilla, "Consecutivo_Rad")
                    If Result <> "YES" Then
                        Label_result.Text = Label_result.Text & Result & vbCrLf
                        Exit Sub
                    End If
                End If
            End If
            Result = refclas.Auto_completar_editar_guia(Me.Page)
            If Result <> "YES" Then
                Label_result.Text = Label_result.Text & Result
                Exit Sub
            End If
        Catch ex As Exception
            Label_result.Text = Label_result.Text & ex.Message
        End Try
    End Sub
    Protected Sub Button_consulta_pendientes_procesar_Click(sender As Object, e As EventArgs) Handles Button_consulta_pendientes_procesar.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Result = refclas.Lista_guia_envio_correspondencia_por_procesar(Me.Page)
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

    Private Sub Button_editar_guia_Click(sender As Object, e As EventArgs) Handles Button_editar_guia.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            If Me.hdnEmailID_VAL.Value = "-1" Then
                scripjava.Showscripman("Debe seleccionar el registro para asignar guía", Me.UpdatePanel_botones_radicacion)
                Exit Sub
            End If

            Me.TextBox_codigo_guia_envio.Enabled = True
            Me.DropDownList_mensajero_interno.Enabled = True
            Me.Label_tramite.Text = "Tipo de operador"
            Me.TextBox_codigo_guia_envio.Text = ""
            Me.TextBox_ANEXO.Text = ""
            Dim id_destinatario As Integer = 0
            Result = refclas.Retorna_id_destinatario_guia_correspondencia(Me.hdnEmailID_VAL.Value, id_destinatario)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            Dim id_guia_tramite As Integer = Val(Me.hdnEmailID_VAL.Value)
            'Result = refclas.retorna_consecutivo_guia_respuesta_radicado(Me.hdnEmailID_VAL.Value, id_guia_tramite)
            'If Result <> "YES" Then
            '    scripjava.Showscripman(Result, Me.UpdatePanel_procesa_tramite_envio)
            '    Exit Sub
            'End If

            Dim stru() As stru_campos_destinatario = Nothing
            If id_guia_tramite = 0 Then
                Result = refclas.Retorna_operarios_mensajeria_gestion(Me.DropDownList_mensajero_interno, Me.DropDownList_mensajero_interno.Text)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_procesa_tramite_envio)
                    Exit Sub
                End If
                Result = refclas.Retorna_datos_guia_envio_destinatario(stru, id_destinatario)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                Result = refclas.Asigna_datos_interface_datos_externos(stru, Me.Page)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                Result = refclas.Lista_empresa_envio(Me.DropDownList_procesa_tramite_envio, Me.DropDownList_procesa_tramite_envio.Text)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                Else
                    Dim id_tipo_empresa As Integer = 0
                    If Me.DropDownList_procesa_tramite_envio.Text <> "" Then
                        Result = refclas.retorna_tipo_empresa_registro_guia(Me.DropDownList_procesa_tramite_envio.Text, id_tipo_empresa)
                        If Result <> "YES" Then
                            scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                            Exit Sub
                        End If
                        If id_tipo_empresa = 1 Then
                            Me.TextBox_codigo_guia_envio.Enabled = False
                            Me.DropDownList_mensajero_interno.Enabled = True
                            Me.Label_tramite.Text = "Operador interno (El sistema genera el consecutivo de guía, seleccione el operador interno de mensajería)"
                        Else
                            Me.TextBox_codigo_guia_envio.Enabled = True
                            Me.DropDownList_mensajero_interno.Enabled = False
                            Me.Label_tramite.Text = "Operador externo (debe digitar el número de guía que suministra el operador externo)"
                        End If

                    End If
                    Me.Button_actualizar_guia.Visible = True
                    Me.Button_anular_guia.Visible = False
                    Me.Button_descarga_guia.Visible = False
                    Me.Button_procesa_tramite_envio.Visible = False
                    'Me.Button_imprimir_guia.Visible = False
                    UpdatePanel_procesa_tramite_envio.Update()
                    Me.ModalPopupExtender_procesa_tramite_envio.Show()
                End If
            Else
                'retorna datos guia
                Dim strus As guia_envio = Nothing
                Result = refclas.Retorna_datos_estructura_guia(id_guia_tramite, strus)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                Dim refclas_rad As New ClassRadicador
                Dim nombre_usuario As String = ""
                Result = refclas_rad.Retorna_nombre_destinatario_interno(strus.Remit_Dest_Interno_id_Remit_Dest_Int, nombre_usuario)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                Result = refclas_rad.Lista_Destinatario_Interno_edicion(Me.DropDownList_remit_dest_interno, nombre_usuario)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                'retorna nombre mensajero asignado
                Dim refclasgetor As New ClassGestorDocumental
                Dim nombre_mensajero As String = ""
                If strus.ID_MENSAJERO_INTERNO <> 0 Then
                    Result = refclasgetor.Retorna_nombre_id_usuario_gestion(strus.ID_MENSAJERO_INTERNO, nombre_mensajero)
                    If Result <> "YES" Then
                        scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                        Exit Sub
                    End If
                End If
                'lista y asigna el mensajero
                Result = refclas.Retorna_operarios_mensajeria_gestion(Me.DropDownList_mensajero_interno, nombre_mensajero)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                'retorna nombre empresa de mensajería
                Dim nombre_operador_mensajeria As String = ""
                Result = refclas.retorna_nombre_empresa_mensajeria(strus.ra_empresa_envio_ID_EMPRESA_ENVIO, nombre_operador_mensajeria)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                'asigna datos interface
                Result = refclas.Asigna_datos_interface_edicio_guia(strus, Me.Page)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                End If
                Dim id_tipo_empresa As Integer = 0
                If nombre_operador_mensajeria <> "" Then
                    Result = refclas.retorna_tipo_empresa_registro_guia(nombre_operador_mensajeria, id_tipo_empresa)
                    If Result <> "YES" Then
                        scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                        Exit Sub
                    End If
                    If id_tipo_empresa = 1 Then
                        Me.TextBox_codigo_guia_envio.Enabled = False
                        Me.DropDownList_mensajero_interno.Enabled = True
                        Me.Label_tramite.Text = "Operador interno (El sistema genera el consecutivo de guía, seleccione el operador interno de mensajería)"
                    Else
                        Me.TextBox_codigo_guia_envio.Enabled = True
                        Me.DropDownList_mensajero_interno.Enabled = False
                        Me.Label_tramite.Text = "Operador externo (debe digitar el número de guía que suministra el operador externo)"
                    End If

                End If


                Result = refclas.Lista_empresa_envio(Me.DropDownList_procesa_tramite_envio, nombre_operador_mensajeria)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                    Exit Sub
                Else
                    Me.Button_actualizar_guia.Visible = True
                    Me.Button_anular_guia.Visible = False
                    Me.Button_procesa_tramite_envio.Visible = False
                    Me.Button_descarga_guia.Visible = False
                    'Me.Button_imprimir_guia.Visible = True
                    UpdatePanel_procesa_tramite_envio.Update()
                    Me.ModalPopupExtender_procesa_tramite_envio.Show()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_botones_radicacion)
        End Try
    End Sub
    Private Sub Button_gestion_remitente_entrante_Click(sender As Object, e As EventArgs) Handles Button_gestion_remitente_entrante.Click

        Dim Clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassRadicador
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Dim refclasraenv As New ClassRaEnvioCorrespondencia
            Dim Result As String = ""
            Dim id_escrip As Integer = -1
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = -1
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, nombre_plantilla)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            Result = Refclas.Retorna_id_script_validacion(id_plantilla, "DINAMICOEXTERNO", "REMITENTE_COR", id_escrip)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            If id_escrip = -1 Or id_escrip = 0 Then
                Clasjava.Showscripman("No hay plantilla relacionada para el campo", UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            Dim nombre_plantilla_validacion As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(id_escrip, nombre_plantilla_validacion)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            Result = Refclas.Verifica_Permisos_usuario_plantilla_validacion(id_escrip, nombre_plantilla_validacion)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            If Me.TextBox_NOMBRE_RAZON_SOCIAL.Text <> "" Then
                Result = refclasraenv.asigna_remitente_destinatario_id_dest(Me.Page, Me.TextBox_NOMBRE_RAZON_SOCIAL.Text, HttpContext.Current.Session("RA_ID_DEST_EXTERNO"))
                If Result <> "YES" Then
                    Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
                    Exit Sub
                End If
            End If
            Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = id_escrip
            Me.Iframe_validacion_plantilla_.Attributes.Add("src", "../radicador/WebFormGestionPlantillasvalidacion.aspx")
            Me.UpdatePanel_validacion_plantilla.Update()
            Me.ModalPopupExtender_valiacion_plantilla.Show()
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePanel_procesa_tramite_envio)
        End Try
    End Sub
    Private Sub Button_Asigana_datos_validacion_edicion_Click(sender As Object, e As EventArgs) Handles Button_Asigana_datos_validacion_edicion.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        Try
            Result = refclas.asigna_remitente_destinatario_interface_guia(Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, updatepanel_Asigana_datos_validacion_edicion)
            End If
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, updatepanel_Asigana_datos_validacion_edicion)
        End Try
    End Sub

    Private Sub Button_Asigana_datos_validacion_edicion_manual_Click(sender As Object, e As EventArgs) Handles Button_Asigana_datos_validacion_edicion_manual.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        Try
            Result = refclas.asigna_remitente_destinatario_interface_guia_manual(Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, updatepanel_Asigana_datos_validacion_edicion)
            End If
            Me.UpdatePanel_procesa_tramite_envio.Update()
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, updatepanel_Asigana_datos_validacion_edicion)
        End Try
    End Sub

    Private Sub Button_Asigana_datos_validacion_edicion_manual__Click(sender As Object, e As EventArgs) Handles Button_Asigana_datos_validacion_edicion_manual_.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        Try
            Result = refclas.asigna_remitente_destinatario_interface_guia_manual(Me.Page)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
            End If
            'Me.UpdatePanel_procesa_tramite_envio.Update()
        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePanel_procesa_tramite_envio)
        End Try
    End Sub

    Private Sub Button_actualizar_guia_Click(sender As Object, e As EventArgs) Handles Button_actualizar_guia.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("RA_PERMISO_ELIMINAR_GUIA") = 0 Then
                Clasjava.Showscripman("El usuario no tiene permiso para editar la guía ", Me.UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            If Me.hdnEmailID_VAL.Value = "-1" Then
                Clasjava.Showscripman("Debe seleccionar el registro para asignar guía", Me.UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            If Me.DropDownList_procesa_tramite_envio.Text = "" Then
                Clasjava.Showscripman("Debe seleccionar la empresa de mensajería", Me.UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            Dim tipo_envio As Integer = 0
            Result = refclas.retorna_tipo_empresa_registro_guia(Me.DropDownList_procesa_tramite_envio.Text, tipo_envio)
            If Result <> "YES" Then
                Clasjava.Showscripman(Result, Me.UpdatePanel_procesa_tramite_envio)
                Exit Sub
            End If
            Result = refclas.Actualiza_guia_envio_manual(Me.DropDownList_procesa_tramite_envio.Text, Me.TextBox_codigo_guia_envio.Text, Me.Page, Me.TextBox_NOMBRE_RAZON_SOCIAL.Text, _
            Me.DropDownList_mensajero_interno.Text, Me.TextBox_NIT_IDENTIFICACION.Text, Me.TextBox_DIRECCION.Text, Me.TextBox_TELEFONO.Text, Me.TextBox_CORREO_ELECTRONICO.Text, _
            tipo_envio, Me.hdnEmailID_VAL.Value, Me.TextBox_ANEXO.Text, Me.TextBox_RADICADO.Text, Me.DropDownList_remit_dest_interno.Text)
            If Result <> "YES" Then
                Hidden_campos_validacion.Value = ""
                Hidden_valores_validacion.Value = ""
                Clasjava.Showscripman(Result, UpdatePanel_procesa_tramite_envio)
            Else

                Me.ModalPopupExtender_procesa_tramite_envio.Hide()
            End If

        Catch ex As Exception
            Clasjava.Showscripman(ex.Message, UpdatePanel_procesa_tramite_envio)
        End Try
    End Sub

    Private Sub Button_tipo_empresa_guia_Click(sender As Object, e As EventArgs) Handles Button_tipo_empresa_guia.Click
        Dim Result As String = ""
        Dim scripjava As New Classscrripjava
        Dim refclas As New ClassRaEnvioCorrespondencia
        Try
            Dim id_tipo_empresa As Integer = 0
            If Me.DropDownList_procesa_tramite_envio.Text <> "" Then
                Result = refclas.retorna_tipo_empresa_registro_guia(Me.DropDownList_procesa_tramite_envio.Text, id_tipo_empresa)
                If Result <> "YES" Then
                    scripjava.Showscripman(Result, Me.UpdatePanel_bonones_ocultos)
                    Exit Sub
                End If
                If id_tipo_empresa = 1 Then
                    Me.TextBox_codigo_guia_envio.Enabled = False
                    Me.DropDownList_mensajero_interno.Enabled = True
                    Me.Label_tramite.Text = "Operador interno (El sistema genera el consecutivo de guía, seleccione el operador interno de mensajería)"
                Else
                    Me.TextBox_codigo_guia_envio.Enabled = True
                    Me.DropDownList_mensajero_interno.Enabled = False
                    Me.Label_tramite.Text = "Operador externo (debe digitar el número de guía que suministra el operador externo)"
                End If
                Me.UpdatePanel_procesa_tramite_envio.Update()
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_bonones_ocultos)
        End Try
    End Sub

    Private Sub Button_show_estado_guia_Click(sender As Object, e As EventArgs) Handles Button_show_estado_guia.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        If Me.hdnEmailID_VAL.Value = "-1" Then
            Clasjava.Showscripman("Debe seleccionar el regitro a procesar", Me.UpdatePanel_botones_radicacion)
            Exit Sub
        End If
        Dim estado_guia As Integer = 0
        Dim fecha_recibido_guia As String = ""
        Dim nota_guia As String = ""
        Result = refclas.Retorna_datos_estado_guia_envio(Me.hdnEmailID_VAL.Value, estado_guia, fecha_recibido_guia, nota_guia)
        If Result <> "YES" Then
            Clasjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
            Exit Sub
        End If
        Result = refclas.Asigna_datos_interface_guia_envio(estado_guia, fecha_recibido_guia, nota_guia, Me.Page)
        If Result <> "YES" Then
            Clasjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
            Exit Sub
        End If
        ModalPopupExtender_procesa_archivo_tramite.Show()
    End Sub

    Protected Sub Button_procesar_archivo_Click(sender As Object, e As EventArgs) Handles Button_procesar_archivo.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim Clasjava As New Classscrripjava
        If Me.hdnEmailID_VAL.Value = "-1" Then
            Clasjava.Showscripman("Debe seleccionar el regitro a procesar", Me.UpdatePanel_botones_procesa_archivo_tramite)
            Exit Sub
        End If
        Result = refclas.Actualiza_datos_estado_envio(Me.hdnEmailID_VAL.Value, Me.Page)
        If Result <> "YES" Then
            Clasjava.Showscripman(Result, Me.UpdatePanel_botones_procesa_archivo_tramite)
            Exit Sub
        Else

        End If
        ModalPopupExtender_procesa_archivo_tramite.Hide()
    End Sub

    Protected Sub Button_procesar_envio_Click(sender As Object, e As EventArgs) Handles Button_procesar_envio.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim scripjava As New Classscrripjava
        If Me.Hidden_alert_respuesta.Value = "NO" Then Exit Sub
        Dim split_sel_actividades() As String = Me.hdnEmailID_sel.Value.ToString.Split(".")
        If split_sel_actividades Is Nothing Then
            scripjava.Showscripman("Debe chekear los registros a enviar, imposible continuar", Me.UpdatePanel_botones_radicacion)
            Exit Sub
        End If
        If split_sel_actividades.Length = 0 Then
            scripjava.Showscripman("Debe chekear los registros a enviar, imposible continuar", Me.UpdatePanel_botones_radicacion)
            Exit Sub
        End If
        For i As Integer = 0 To split_sel_actividades.Length - 1
            Dim estado_actualizacion As String = ""
            Result = refclas.Archiva_guia_manual(split_sel_actividades(i), estado_actualizacion)
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
                Exit Sub
            Else
                If estado_actualizacion = "YES" Then
                    If Me.Hidden_lista_eliminar_tarea.Value = "0" Then
                        Me.Hidden_lista_eliminar_tarea.Value = split_sel_actividades(i)
                    Else
                        Me.Hidden_lista_eliminar_tarea.Value = Me.Hidden_lista_eliminar_tarea.Value & "." & split_sel_actividades(i)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub Button_anular_guia_manual_Click(sender As Object, e As EventArgs) Handles Button_anular_guia_manual.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim scripjava As New Classscrripjava
        If Me.Hidden_alert_respuesta.Value = "NO" Then Exit Sub
        If HttpContext.Current.Session.Item("RA_PERMISO_ELIMINAR_GUIA") = 0 Then
            scripjava.Showscripman("El usuario no tiene permiso para anular la guía ", Me.UpdatePanel_botones_radicacion)
            Exit Sub
        End If
        If Me.hdnEmailID_VAL.Value = "-1" Then
            scripjava.Showscripman("Debe seleccionar el regitro a procesar", Me.UpdatePanel_botones_radicacion)
            Exit Sub
        End If
        Result = refclas.anula_guia_envio_manual(Val(Me.hdnEmailID_VAL.Value))
        If Result <> "YES" Then
            Hidden_anular.Value = ""
            scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
            Exit Sub
        Else
            Hidden_anular.Value = "YES"
            Exit Sub
        End If
    End Sub

    Private Sub Button_imprimir_guia_Click(sender As Object, e As EventArgs) Handles Button_imprimir_guia.Click
        Dim Result As String = ""
        Dim refclas As New ClassRaEnvioCorrespondencia
        Dim scripjava As New Classscrripjava
        Dim ruta_archivo As String = ""
        If HttpContext.Current.Session.Item("RA_PERMISO_IMPRIMIR_GUIA") = 0 Then
            scripjava.Showscripman("El usuario no tiene permiso para imprimir la guía ", Me.UpdatePanel_botones_radicacion)
            Exit Sub
        End If
        If Me.hdnEmailID_VAL.Value = "-1" Then
            scripjava.Showscripman("Debe seleccionar el regitro a imprimir", Me.UpdatePanel_botones_radicacion)
            Exit Sub
        End If
        Result = refclas.genera_documento_guia(Val(Me.hdnEmailID_VAL.Value), ruta_archivo)
        If Result <> "YES" Then
            scripjava.Showscripman(Result, Me.UpdatePanel_botones_radicacion)
            Exit Sub
        Else
            Me.Hiddenruta.Value = ruta_archivo
            Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
            UpdatePaneliframe_2.Update()
            ModalPopupExtenderimpre.Show()
        End If
    End Sub
End Class