Imports Dynamsoft.DotNet.TWAIN.Barcode

Public Class WebFormPqrsPrincipal
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassPqrs
            If Me.Page.IsPostBack = False Then
                Dim Codigo_Plantilla_radicado As Integer = 0
                Dim Nombre_plantilla_radicado As String = ""
                Dim codigo_script As Integer = 0
                Dim campo_comparacion As String = ""
                Dim id_plantilla_validacion As Integer = 0
                Dim nombre_plantilla_validacion As String = ""
                Dim nombre_campo_nit As String = ""
                Dim nombre_campo_anualidad As String = ""
                Dim nombre_campo_idext As String = ""
                Result = Refclas.Retorna_nombre_codigo_plantilla(Codigo_Plantilla_radicado,
                                                                 Nombre_plantilla_radicado)
                If Result <> "YES" Then
                    Me.Label_estado_inicio.Text = Me.Label_estado_inicio.Text & "-" & Result
                    Exit Sub
                Else
                    Result = Refclas.Retorna_nombre_codigo_plantilla_validacion(Codigo_Plantilla_radicado,
                                                                                campo_comparacion,
                                                                                id_plantilla_validacion,
                                                                                nombre_plantilla_validacion,
                                                                                codigo_script)
                    If Result <> "YES" Then
                        Me.Label_estado_inicio.Text = Me.Label_estado_inicio.Text & "-" & Result
                        Exit Sub
                    End If
                    Result = Refclas.Retorna_nombre_campo_nit_pqr_validacion(id_plantilla_validacion,
                                                                             nombre_campo_nit)
                    If Result <> "YES" Then
                        Me.Label_estado_inicio.Text = Me.Label_estado_inicio.Text & "-" & Result
                        Exit Sub
                    End If
                    Result = Refclas.Retorna_nombre_campo_anualidad_pqr_validacion(id_plantilla_validacion,
                                                                                   nombre_campo_anualidad)
                    If Result <> "YES" Then
                        Me.Label_estado_inicio.Text = Me.Label_estado_inicio.Text & "-" & Result
                        Exit Sub
                    End If
                    Result = Refclas.Retorna_nombre_campo_dest_pqr_validacion(id_plantilla_validacion,
                                                                              nombre_campo_idext)
                    If Result <> "YES" Then
                        Me.Label_estado_inicio.Text = Me.Label_estado_inicio.Text & "-" & Result
                        Exit Sub
                    End If
                End If
                Session.Item("PQRS_CODIGO_PLANTILLA_RADICADO") = Codigo_Plantilla_radicado
                Session.Item("PQRS_NOMBRE_PLANTILLA_RADICADO") = Nombre_plantilla_radicado
                Session.Item("PQRS_CODIGO_PLANTILLA_VALIDACION") = id_plantilla_validacion
                Session.Item("PQRS_NOMBRE_PLANTILLA_VALIDACION") = nombre_plantilla_validacion
                Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA") = codigo_script
                Session.Item("PQRS_CAMPO_NIT_PLANTILLA") = nombre_campo_nit
                Session.Item("PQRS_CAMPO_ANUALIDAD_PLANTILLA") = nombre_campo_anualidad
                Session.Item("PQRS_CAMPO_IDEXT_PLANTILLA") = nombre_campo_idext
            End If

        Catch ex As Exception
            Me.Label_estado_inicio.Text = Me.Label_estado_inicio.Text & "-" & ex.Message
        End Try
    End Sub
    Private Sub UpdatePanelContenido_Load(sender As Object, e As EventArgs) Handles UpdatePanelContenido.Load
        'Dim refclas_consulta As New ClassRaConsultaRadicados
        'Dim refclas_pqr As New ClassPqrs
        'Dim rescrip As New Classscrripjava
        'Try
        '    Dim Result = refclas_pqr.Genera_Interface_Gestion_Plantilla_Validacion_pqr(Me, _
        '                                                                               Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"))
        '    If Result <> "YES" Then
        '        Me.Label_estado_inicio.Text = Me.Label_estado_inicio.Text & "-" & Result
        '        Exit Sub
        '    End If
        'Catch ex As Exception

        'End Try
    End Sub

    Private Sub Button_anonimo_Click(sender As Object, e As EventArgs) Handles Button_anonimo.Click
        Dim java_script As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New ClassPqrs
        Try
            Dim valor_campo_consulta As String = ""
            Dim existencia_registro As String = ""
            Dim campo_null_registro As String = ""
            Dim resultado_anualidad As String = ""
            Dim resultado_codigo_usuario As Integer = 0
            Session.Item("PQRS_TIPO_PQRS") = "Anonímo"
            Result = Refclas.Consulta_existencia_usuario_pqrs_registrado(Session.Item("PQRS_NOMBRE_PLANTILLA_VALIDACION"),
                                                                         Session.Item("PQRS_CAMPO_NIT_PLANTILLA"),
                                                                         "9999999999",
                                                                         valor_campo_consulta,
                                                                         existencia_registro,
                                                                         campo_null_registro,
                                                                         Session.Item("PQRS_CAMPO_ANUALIDAD_PLANTILLA"),
                                                                         resultado_anualidad,
                                                                         Session.Item("PQRS_CAMPO_IDEXT_PLANTILLA"),
                                                                         resultado_codigo_usuario)
            If Result <> "YES" Then
                java_script.Showscripman_menu(Result, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If existencia_registro = "NO" Then
                java_script.Showscripman_menu("Contacte al administrador para activar el usuario anónimo 9999999999-1977-Anónimo",
                                              Me.UpdatePanel_botones,
                                              "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("PQRS_ID_USUARIO_PQRS") = resultado_codigo_usuario
                Hidden_resultado_buscar.Value = "YES"
                Me.UpdatePanel_buton_ingresar.Update()
                Me.UpdatePanel_boton_Panel_validacion_usuario.Update()
            End If
        Catch ex As Exception
            java_script.Showscripman_menu(ex.Message,
                                              Me.UpdatePanel_botones,
                                              "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_buscar_Click(sender As Object, e As EventArgs) Handles Button_buscar.Click
        Dim java_script As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New ClassPqrs
        Try
            If Me.TextBox_nit_identificacion.Text = "" Then
                java_script.Showscripman_menu("Por favor digite su numero de identificación o nit ",
                                              Me.UpdatePanel_boton_Panel_validacion_usuario,
                                              "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Me.DropDownList_anualidad.Text = "" Then
                java_script.Showscripman_menu("Por favor seleccione el año de nacimiento  ", Me.UpdatePanel_boton_Panel_validacion_usuario, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim valor_campo_consulta As String = ""
            Dim existencia_registro As String = ""
            Dim campo_null_registro As String = ""
            Dim resultado_anualidad As String = ""
            Dim resultado_codigo_usuario As Integer = 0
            Result = Refclas.Consulta_existencia_usuario_pqrs_registrado(Session.Item("PQRS_NOMBRE_PLANTILLA_VALIDACION"),
                                                                         Session.Item("PQRS_CAMPO_NIT_PLANTILLA"),
                                                                         Me.TextBox_nit_identificacion.Text,
                                                                         valor_campo_consulta,
                                                                         existencia_registro,
                                                                         campo_null_registro,
                                                                         Session.Item("PQRS_CAMPO_ANUALIDAD_PLANTILLA"),
                                                                         resultado_anualidad,
                                                                         Session.Item("PQRS_CAMPO_IDEXT_PLANTILLA"),
                                                                         resultado_codigo_usuario)
            If Result <> "YES" Then
                java_script.Showscripman_menu(Result, Me.UpdatePanel_boton_Panel_validacion_usuario, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '-----------------------------------------
            'Verifica si existe el registro
            '-----------------------------------------
            If existencia_registro = "NO" Then
                'ejecuta ventana de registro
                Me.UpdatePanel_buton_ingresar.Update()
                Me.UpdatePanelContenido.Update()
                Me.ModalPopupExtender_registro_usuario.Show()
                Me.ModalPopupExtender_validacion_usuario.Hide()
            Else

                If resultado_anualidad = "" Then
                    'ejecuta ventana de actualiazacion de anualidad
                    Result = Refclas.Lista_nombre_usuarios_pqr(Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"),
                                                               Me.DropDownList_usuarios_registro,
                                                               Me.UpdatePanel_boton_Panel_validacion_usuario,
                                                               Me.TextBox_nit_identificacion.Text)
                    If Result <> "YES" Then
                        java_script.Showscripman_menu(Result,
                                                      Me.UpdatePanel_boton_Panel_validacion_usuario,
                                                      "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        Me.UpdatePanel_actualizacion_anualidad.Update()
                        Me.ModalPopupExtender_actualizacion_anualidad.Show()
                        Me.ModalPopupExtender_validacion_usuario.Hide()
                    End If
                Else
                    If resultado_anualidad <> Me.DropDownList_anualidad.Text Then
                        Me.ModalPopupExtender_recuperar_anualidad.Show()
                        Me.ModalPopupExtender_validacion_usuario.Hide()
                        Exit Sub
                    Else
                        If resultado_codigo_usuario = 0 Then
                            java_script.Showscripman_menu("El sistema no registra el código interno del usuario ",
                                                          Me.UpdatePanel_boton_Panel_validacion_usuario,
                                                          "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        Else
                            Session.Item("PQRS_ID_USUARIO_PQRS") = resultado_codigo_usuario
                            Hidden_resultado_buscar.Value = "YES"
                        End If

                    End If
                End If
            End If
        Catch ex As Exception
            java_script.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_Panel_validacion_usuario, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_pagina_web_Click(sender As Object, e As EventArgs) Handles Button_pagina_web.Click
        Session.Item("PQRS_TIPO_PQRS") = "Personal"
        Me.ModalPopupExtender_validacion_usuario.Show()
    End Sub



    Private Sub WebFormPqrsPrincipal_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        Dim java_script As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New ClassPqrs
        Result = Refclas.Asigna_datos_nit_anualidad_plantilla_validacion_pqr(Me, Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"))
        If Result <> "YES" Then
            java_script.Showscripman_menu(Result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        End If
        If Page.IsPostBack = False Then
            Dim anualidad As String = Date.Now.Year.ToString
            Me.DropDownList_anualidad.Items.Clear()
            Me.DropDownList_anualidad.Items.Add("")
            For i As Integer = 1850 To Val(anualidad)
                Me.DropDownList_anualidad.Items.Add(i)
            Next
        End If
    End Sub

    Private Sub Button_registra_usuario_Click(sender As Object, e As EventArgs) Handles Button_registra_usuario.Click
        Dim java_script As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New ClassPqrs
        Try

            UpdatePanelContenido.Update()
            Result = Refclas.Agregar_Nuevo_Registro_plantilla_validacion_edicion_pqr(Me.Page,
                                                                                     Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"),
                                                                                     Session.Item("PQRS_ID_USUARIO_PQRS"))
            If Result <> "YES" Then
                java_script.Showscripman_menu(Result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resultado_registro.Value = "YES"
            End If

        Catch ex As Exception
            java_script.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub UpdatePanel_botones_validacion_Load(sender As Object, e As EventArgs) Handles UpdatePanel_botones_validacion.Load
        'Dim java_script As New Classscrripjava
        'Dim Result As String = ""
        'Dim Refclas As New ClassPqrs
        'Result = Refclas.Genera_Interface_Gestion_Plantilla_Validacion_pqr(Me, Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"))
        'If Result <> "YES" Then
        '    java_script.Showscripman_menu(Result, Me.UpdatePanel_botones_validacion, "ModalPopupExtender_mensaje_personalizado")
        '    Exit Sub
        'End If
    End Sub

    Private Sub Button_regresar_registro_Click(sender As Object, e As EventArgs) Handles Button_regresar_registro.Click
        Me.ModalPopupExtender_registro_usuario.Hide()
        Me.ModalPopupExtender_validacion_usuario.Show()
    End Sub

    Protected Sub Button_regresar_Click(sender As Object, e As EventArgs) Handles Button_regresar.Click
        ModalPopupExtender_actualizacion_anualidad.Hide()
        Me.ModalPopupExtender_validacion_usuario.Show()
    End Sub

    Protected Sub Button_actualizar_anualidad_Click(sender As Object, e As EventArgs) Handles Button_actualizar_anualidad.Click
        'Actualiza_anualidad_usuario_pqr
        Dim java_script As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New ClassPqrs
        Try
            If Me.DropDownList_usuarios_registro.Text = "" Then
                java_script.Showscripman_menu("Por favor seleccione su nombre de la lista", Me.UpdatePanel_actualizacion_anualidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Actualiza_anualidad_usuario_pqr(Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"),
                                                             Me.DropDownList_usuarios_registro.Text,
                                                             Me.TextBox_nit_identificacion.Text,
                                                             Me.DropDownList_anualidad.Text,
                                                             Session.Item("PQRS_ID_USUARIO_PQRS"))
            If Result <> "YES" Then
                java_script.Showscripman_menu(Result, Me.UpdatePanel_actualizacion_anualidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Hidden_resultado_actualizar.Value = "YES"
            End If
        Catch ex As Exception
            java_script.Showscripman_menu(ex.Message, Me.UpdatePanel_actualizacion_anualidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub ButtonRecuperar_Click(sender As Object, e As EventArgs) Handles ButtonRecuperar.Click
        Dim Result As String = ""
        Dim java_script As New Classscrripjava
        Dim Refclas As New ClassPqrs
        Try
            If Me.TextBox_correo_electronico_recuperacion.Text = "" Then
                java_script.Showscripman_menu("Por favor digite el correo electrónico ", Me.UpdatePanel_recuperar_anualidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Recupera_anualidad_usuario_pqrs(Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"), Me.TextBox_nit_identificacion.Text, Me.TextBox_correo_electronico_recuperacion.Text)
            If Result <> "YES" Then
                java_script.Showscripman_menu(Result, Me.UpdatePanel_recuperar_anualidad, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                ModalPopupExtender_recuperar_anualidad.Hide()
                java_script.Showscripman_menu("El correo electrónico se envió correctamente con el registro de su año de nacimiento.", Me.UpdatePanel_recuperar_anualidad, "ModalPopupExtender_mensaje_personalizado")

            End If
        Catch ex As Exception
            java_script.Showscripman_menu(ex.Message, Me.UpdatePanel_recuperar_anualidad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ButtonRegresar_Recuperacion_Click(sender As Object, e As EventArgs) Handles ButtonRegresar_Recuperacion.Click
        ModalPopupExtender_recuperar_anualidad.Hide()
        Me.ModalPopupExtender_validacion_usuario.Show()
    End Sub

    Protected Sub Button_atencion_personal_Click(sender As Object, e As EventArgs) Handles Button_atencion_personal.Click
        Dim Result As String = ""
        Dim java_script As New Classscrripjava
        Dim Refclas As New ClassPqrs
        Try
            Result = Refclas.Retorna_datos_atension_pqrs("atencion_personalizada",
                                                         Me.Label_info_contacto.Text)
            If Result <> "YES" Then
                java_script.Showscripman_menu(Result, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Label_detalle_info.Text = "Centros de atención personalizada"
                UpdatePanel_mensaje_contactos.Update()
                ModalPopupExtender_mensaje_contactos.Show()
            End If
        Catch ex As Exception
            java_script.Showscripman_menu(ex.Message, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_atencion_telefonica_Click(sender As Object, e As EventArgs) Handles Button_atencion_telefonica.Click
        Dim Result As String = ""
        Dim java_script As New Classscrripjava
        Dim Refclas As New ClassPqrs
        Try

            Result = Refclas.Retorna_datos_atension_pqrs("atencion_telefonica", Me.Label_info_contacto.Text)
            If Result <> "YES" Then
                java_script.Showscripman_menu(Result, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else

                Label_detalle_info.Text = "Atención Telefónica "
                UpdatePanel_mensaje_contactos.Update()
                ModalPopupExtender_mensaje_contactos.Show()
            End If
        Catch ex As Exception
            java_script.Showscripman_menu(ex.Message, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub


    Private Sub Button_activa_correo_info_Click(sender As Object, e As EventArgs) Handles Button_activa_correo_info.Click
        Dim Result As String = ""
        Dim java_script As New Classscrripjava
        Dim Refclas As New ClassPqrs
        Try

            Result = Refclas.Retorna_datos_atension_pqrs("correo", Me.Label_info_contacto.Text)
            If Result <> "YES" Then
                java_script.Showscripman_menu(Result, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else

                Label_detalle_info.Text = "Correo(s) Electrónico(s) Oficiale (s) "
                UpdatePanel_mensaje_contactos.Update()
                ModalPopupExtender_mensaje_contactos.Show()
            End If
        Catch ex As Exception
            java_script.Showscripman_menu(ex.Message, Me.UpdatePanel_botones, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_registra_usuario_peticionario_Click(sender As Object, e As EventArgs) Handles Button_registra_usuario_peticionario.Click
        Dim refclas_consulta As New ClassRaConsultaRadicados
        Dim refclas_pqr As New ClassPqrs
        Dim rescrip As New Classscrripjava
        Dim Refclas As New ClassPqrs
        Try
            Session.Item("PQRS_TIPO_PQRS") = "Personal"
            Dim Result = refclas_pqr.Genera_Interface_Gestion_Plantilla_Validacion_pqr(Me,
                                                                                       Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA"))
            If Result <> "YES" Then
                rescrip.Showscripman_menu(Result, Me.UpdatePanel_botones_configuracion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_class_admon As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Dim id_organigrama As Integer = 0
            Result = ref_class_admon.Retorna_Id_Emprea(HttpContext.Current.Session.Item("EMPRESA_GESTION"),
                                                       id_empresa)
            If Result <> "YES" Then
                rescrip.Showscripman_menu(Result, Me.UpdatePanel_botones_configuracion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Dim Reclas_registro_organigrama As New Class_registro_organigrama
                Result = Reclas_registro_organigrama.Retorna_Id_Organigrama_activo_empresa(id_empresa,
                                                                                           id_organigrama)
                If Result <> "YES" Then
                    rescrip.Showscripman_menu(Result, Me.UpdatePanel_botones_configuracion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                HttpContext.Current.Session.Item("GA_IDEMPRESA") = id_empresa
                Result = Refclas.Listar_Tipos_Documentales_pqrs(Me.DropDownList_tipo_tramite,
                                                                Session.Item("PQRS_CODIGO_PLANTILLA_RADICADO"))
                If Result <> "YES" Then
                    rescrip.Showscripman_menu(Result, Me.UpdatePanel_botones_configuracion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                Result = Class_areas_depart_radicacion.Lista_areas_usuario_gestion_permitido_para_gestionar_pqr(id_organigrama,
                                                                                                                DropDownList_area_dependencia)
                If Result <> "YES" Then
                    rescrip.Showscripman_menu(Result, Me.UpdatePanel_botones_configuracion, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

            End If
            Me.ModalPopupExtender_registro_usuario.Show()
        Catch ex As Exception
            rescrip.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_configuracion, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub
    Private Sub Button_anexo_cargar_Click(sender As Object, e As EventArgs) Handles Button_anexo_cargar.Click
        Dim Result As String = ""
        Dim Refclas As New Classgestionrespuesta
        Dim scrijava As New Classscrripjava
        Try

            Me.ModalPopupExtender_edition_sube_anexo_respuesta.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.UpdatePanel_botones_configuracion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class